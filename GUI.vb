Imports System.IO
Imports System.Text
Imports HtmlAgilityPack

Imports System.Collections.Generic

Imports OpenQA.Selenium
Imports OpenQA.Selenium.Chrome
Imports OpenQA.Selenium.Support.UI

Imports System.Text.RegularExpressions

Imports Google.Cloud.Translation.V2

Imports MySql.Data.MySqlClient

Imports System.Threading.Tasks

Public Class GUI

    Dim pathList As New List(Of String)     'перелік шляхів всіх сторінок
    Dim abs_pathList As New List(Of String)     'перелік повних шляхів всіх сторінок
    Dim linksList As New List(Of String)    'перелік посилань на сайт https://help.autodesk.com/
    Dim NamesList As New List(Of String)    'перелік назв сторінок
    Dim guidlist As New List(Of String)    'перелік guid сторінок

    Const GoogleCloudApiKey = "AIzaSyA_lbC-g1PfLkg6nskzmd0tJ0NagGhQ-D0"

    Dim connectionString As String = "server=localhost;user=root;password=;database=f360_short;"
    ' Dim connectionString As String = "server=localhost;user=root;password=;database=fusion_translate_db;"

    Sub start_import(filePath As String, a_guid As String, link_name As String)

        Dim connection As New MySqlConnection(connectionString)
        Dim Filename As String
        connection.Open()

        '  Dim query As String = "SELECT * FROM `wp_posts` ORDER BY `post_title` ASC;"

        Dim htmlContent As String               'документ, що обробляється

        Using reader_html As New StreamReader(filePath, Encoding.Default)
            ' Read the entire contents of the file
            htmlContent = reader_html.ReadToEnd()
        End Using

        Dim htmlDoc As New HtmlDocument()
        htmlDoc.LoadHtml(htmlContent)

        ' отримання початкового елементу вкладеного переліку
        Dim rootUl As HtmlNode = htmlDoc.DocumentNode.SelectSingleNode("//h1[@itemprop='name']")

        If rootUl IsNot Nothing Then
            Filename = rootUl.InnerText
        Else
            Filename = ""
        End If

        Dim currentDate As DateTime = DateTime.Now
        Dim formattedDate As String = currentDate.ToString("yyyy-MM-dd HH:mm:ss")



        Dim ID As String = "NULL"
        Dim post_author As Integer = 1
        Dim post_date As String = formattedDate            '     "2023-07-01 21:27:58"
        Dim post_date_gmt As String = formattedDate        '     "2023-07-01 21:27:58"
        Dim post_content As String = htmlContent
        Dim post_title As String = Filename
        Dim post_excerpt As String = ""
        Dim post_status As String = "publish"
        Dim comment_status As String = "closed"
        Dim ping_status As String = "closed"
        Dim post_password As String = ""
        Dim post_name As String = link_name
        Dim to_ping As String = ""
        Dim pinged As String = ""
        Dim post_modified As String = formattedDate        '     "2023-07-01 21:27:58"
        Dim post_modified_gmt As String = formattedDate    '     "2023-07-01 21:27:58"
        Dim post_content_filtered As String = ""
        Dim post_parent As String = "0"
        Dim guid As String = ""
        Dim menu_order As String = "0"
        Dim post_type As String = "page"
        Dim post_mime_type As String = ""
        Dim comment_count As String = "0"
        Dim adsk_guid As String = a_guid



        Dim query As String = "INSERT INTO `wp_posts` (`ID`, `post_author`, `post_date`, `post_date_gmt`, `post_content`, `post_title`, `post_excerpt`, `post_status`, `comment_status`, `ping_status`, `post_password`, `post_name`, `to_ping`, `pinged`, `post_modified`, `post_modified_gmt`, `post_content_filtered`, `post_parent`, `guid`, `menu_order`, `post_type`, `post_mime_type`, `comment_count`, `adsk_guid`) 
        VALUES (" & ID & ", '" & post_author & "', '" & post_date & "', '" & post_date_gmt & "', '" & post_content & "', '" & post_title & "', '" & post_excerpt & "', '" & post_status & "', '" & comment_status & "', '" & ping_status & "', '" & post_password & "', '" & post_name & "', '" & to_ping & "', '" & pinged & "', '" & post_modified & "', '" & post_modified_gmt & "', '" & post_content_filtered & "', '" & post_parent & "', '" & guid & "', '" & menu_order & "', '" & post_type & "', '" & post_mime_type & "', '" & comment_count & "', '" & adsk_guid & "');"

        Dim command As New MySqlCommand(query, connection)
        Dim reader As MySqlDataReader = command.ExecuteReader()

        reader.Close()


    End Sub


    Sub hierarchical_structure_SQL()


        Dim connection As New MySqlConnection(connectionString)
        Dim rowcount As Integer
        Dim row_ID As Integer
        Dim parent_ID As String
        Dim parent_name As String
        Dim parent_guid As String
        Dim cur_file As Integer = 0

        connection.Open()

        Dim itemList As New List(Of String)()

        'отримання таблиці
        Dim query As String = "SELECT * FROM `wp_posts`;"

        Using command As New MySqlCommand(query, connection)
            Using reader As MySqlDataReader = command.ExecuteReader()
                'отримання переліку значень стовпця adsk_guid 
                While reader.Read()
                    Dim item As String = reader.GetString("adsk_guid")
                    itemList.Add(item)
                End While
            End Using
        End Using

        rowcount = guidlist.Count
        Dim progress = itemList.Count

        For Each item In itemList

            Label1.Text = CStr((cur_file + 1) & " / " & progress)
            ProgressBar1.Value = Math.Round(100 * (cur_file + 1) / progress)

            parent_ID = 0

            For i = 0 To rowcount - 1
                If item = guidlist(i) Then
                    parent_name = abs_pathList(i)
                    Dim index As Integer = parent_name.LastIndexOf("\")
                    Dim cut_part As Integer = parent_name.Length - index
                    'видалення зайвої частини
                    If index > 0 Then
                        parent_name = parent_name.Remove(index, cut_part)
                    End If
                    Exit For
                End If
            Next

            For i = 0 To rowcount - 1
                If parent_name = abs_pathList(i) Then
                    parent_guid = guidlist(i)
                    Exit For
                End If
            Next

            If parent_guid IsNot Nothing And parent_guid <> item Then

                'пошук за значенням adsk_guid та отримання ID певного рядка
                query = "SELECT * FROM `wp_posts` WHERE `adsk_guid` LIKE '" & parent_guid & "';"
                Using command As New MySqlCommand(query, connection)

                    Using reader As MySqlDataReader = command.ExecuteReader()
                        If reader.Read() Then
                            parent_ID = reader.GetString("ID")
                        End If
                    End Using
                End Using

                query = "SELECT * FROM `wp_posts` WHERE `adsk_guid` LIKE '" & item & "';"
                Using command As New MySqlCommand(query, connection)

                    Using reader As MySqlDataReader = command.ExecuteReader()
                        If reader.Read() Then
                            row_ID = reader.GetString("ID")
                        End If
                    End Using
                End Using

                'оновлення значення батьківського посту
                '   query = "UPDATE `wp_posts` SET `post_parent` = '7' WHERE `wp_posts`.`ID` = 61;"
                query = "UPDATE `wp_posts` SET `post_parent` = '" & parent_ID & "' WHERE `wp_posts`.`ID` = " & row_ID & ";"
                Using command As New MySqlCommand(query, connection)
                    Using reader As MySqlDataReader = command.ExecuteReader()
                    End Using
                End Using


            End If

            cur_file += 1

            Application.DoEvents()
        Next


        ' reader.Close()
        connection.Close()

    End Sub



    Sub translate(htmlContent As String, savePath As String)

        'ініціалізація перекладача
        Dim translateClient As TranslationClient        'перекладач
        Dim response As TranslationResult               'місце для зберігання результату перекладу
        Dim temp_row As String
        Dim htmlSave As New HtmlDocument()
        translateClient = TranslationClient.CreateFromApiKey(GoogleCloudApiKey)


        Dim chunks As New List(Of String)()
        Dim chunkSize As Integer = 100000

        Dim encoding As Encoding = Encoding.Default

        Dim byteCount As Integer = encoding.GetByteCount(htmlContent)

        If byteCount < chunkSize Then

            response = translateClient.TranslateHtml(htmlContent, LanguageCodes.Ukrainian)

            temp_row = response.TranslatedText

        Else

            Dim startIndex As Integer = 0
            Dim endIndex As Integer = chunkSize
            temp_row = ""

            While startIndex < htmlContent.Length

                If endIndex > htmlContent.Length Then
                    endIndex = htmlContent.Length
                End If


                chunks.Add(htmlContent.Substring(startIndex, endIndex - startIndex))

                startIndex = endIndex
                endIndex += chunkSize
            End While

            For Each chunk In chunks

                response = translateClient.TranslateHtml(chunk, LanguageCodes.Ukrainian)
                temp_row += response.TranslatedText

            Next

        End If

        ' зберігання файлу
        htmlSave.LoadHtml(temp_row)

        htmlSave.Save(savePath)


    End Sub


    'збереження html файлів з використанням Chrome та бібліотеки Selenium
    Public Sub SavePageAsHtml(url As String, savePath As String)

        Dim options As New ChromeOptions()      'ініціалізація виклику браузера та опцій виклику
        Dim isDisplayed As Boolean = False      'змінна для перевірки відображення необхідного елементу сторінки
        Dim rootUl As HtmlNode                  'вузол з вмістом текстової частини сторінки

        Dim htmlDoc As New HtmlDocument()       'документ, що відкривається у браузері
        Dim htmlSave As New HtmlDocument()      'зберігаємий документ тільки з текстовою частиною

        ' options.AddArgument("--headless")     'опція Chrome для зупуску без відображення інтерфейсу
        Dim driver As New ChromeDriver(options) 'запуск Chrome

        Dim counter As Integer = 0              'змінна для контролю кількості циклів очікування

        'перехід за посиланням
        driver.Navigate().GoToUrl(url)

        ' Wait for the page to load 
        Threading.Thread.Sleep(1000)

        'функція перевірки наявності відображення необхідного елементу, який містить текстову частину
        While isDisplayed = False

            'отримання поточного html
            Dim html As String = driver.PageSource
            htmlDoc.LoadHtml(html)
            'обирання вузла з текстом
            rootUl = htmlDoc.DocumentNode.SelectSingleNode("//div[@Class='body_content']")
            'перевірка на наявність вмісту
            If rootUl IsNot Nothing Then
                isDisplayed = True
            Else
                isDisplayed = False
                Threading.Thread.Sleep(1000)
            End If

            'перезавантаження сторінки
            If counter = 10 Then
                driver.Navigate().GoToUrl(url)
            End If

            'примусовий вихід з циклу
            If counter > 20 Then
                Exit While
            End If

            counter += 1
        End While


        If rootUl Is Nothing Then

            driver.Quit()
            SavePageAsHtml(url, savePath)

        Else

            ' зберігання файлу
            htmlSave.DocumentNode.AppendChild(rootUl)

            Dim nodeToDelete As HtmlNode = htmlSave.DocumentNode.SelectSingleNode("//div[@data-type='Sharing']")

            If nodeToDelete IsNot Nothing Then
                ' Delete the node
                nodeToDelete.Remove()
            End If

            htmlSave.Save(savePath)

            ' Close the browser
            driver.Quit()

        End If

    End Sub


    'рекурсивний пошук вкладених елементів
    'Private Shared Function GetPathList(node As HtmlNode, Optional parentPath As String = "") As (List(Of String), List(Of String), List(Of String), List(Of String))
    Private Shared Function GetPathList(node As HtmlNode, Optional parentPath As String = "") As Tuple(Of List(Of String), List(Of String), List(Of String), List(Of String))
        'оголошення відповідних переліків для парсингу
        Dim pathList_f As New List(Of String)()
        Dim LinksList_f As New List(Of String)()
        Dim NamesList_f As New List(Of String)()
        Dim guidList_f As New List(Of String)()
        'ім'я поточного вузла
        Dim nodeText As String
        Dim guid_text As String


        guid_text = node.GetAttributeValue("data-id", "null")

        guidList_f.Add(guid_text)

        'отримання колекції вузлів поточного вузла для пошуку елементу, який відповідає за ім'я
        Dim Childs_nodes As HtmlNodeCollection = node.ChildNodes
        'обирання необхідного вузла
        Dim selectedNode As HtmlNode = Childs_nodes.FirstOrDefault(Function(n) n.Name = "a")
        'заміна заборонених у назвах файлу символів
        nodeText = Regex.Replace(selectedNode.InnerText.Trim(), "[<>:""/\|?*]", "")

        'пошук атрібуту, у якому знаходиться посилання, якщо його немає, то призначеється значення "null"
        Dim dataUrlValue As String = selectedNode.GetAttributeValue("data-url", "null")
        'додавання "шапки" перед посиланням
        If dataUrlValue IsNot "null" Then
            dataUrlValue = ("https://help.autodesk.com" & dataUrlValue)
        End If

        'запис посилання у перелік
        LinksList_f.Add(dataUrlValue)
        'запис імені файлу у перелік
        NamesList_f.Add(nodeText)
        'накопичувальна змінна, яка формує повний шлях за ієрархією
        'перевірка, чи порожнє значення попередньої вкладеності. Якщо так, то додавання назви вузлу, у іншому випадку - додавання назви поточного вузла до ієрархії
        Dim path As String = If(String.IsNullOrEmpty(parentPath), nodeText, parentPath & "\" & nodeText)

        'запис шляху у перелік
        pathList_f.Add(path)

        'рекурсивний пошук на наявність вкладений вузлів
        For Each child As HtmlNode In Childs_nodes
            'перевірка наявності вкладеності
            If child.Name = "ul" Then

                Dim Childs_UL As HtmlNodeCollection = child.ChildNodes
                'пошук вкладених вузлів
                For Each node_ul As HtmlNode In Childs_UL
                    If node_ul.Name = "li" Then
                        'pathList_f.AddRange(GetPathList(node_ul, path).Item1)
                        'LinksList_f.AddRange(GetPathList(node_ul, path).Item2)
                        'NamesList_f.AddRange(GetPathList(node_ul, path).Item3)
                        'guidList_f.AddRange(GetPathList(node_ul, path).Item4)


                        Dim result As Tuple(Of List(Of String), List(Of String), List(Of String), List(Of String)) = GetPathList(node_ul, path)

                        pathList_f.AddRange(result.Item1)
                        LinksList_f.AddRange(result.Item2)
                        NamesList_f.AddRange(result.Item3)
                        guidList_f.AddRange(result.Item4)



                    End If
                Next

            End If
        Next

        ' повернення сформованих переліків
        ' Return (pathList_f, LinksList_f, NamesList_f, guidList_f)
        Return Tuple.Create(pathList_f, LinksList_f, NamesList_f, guidList_f)

    End Function


    'функція пошуку коренних вузлів переліку у html документі
    Public Shared Function ConvertToPathList(html As String) As Tuple(Of List(Of String), List(Of String), List(Of String), List(Of String))
        'оголошення відповідних переліків для парсингу
        Dim pathList_f As New List(Of String)()
        Dim linksList_f As New List(Of String)()
        Dim NamesList_f As New List(Of String)()
        Dim guidlist_f As New List(Of String)()

        ' Parse the HTML string into an HTML document
        Dim htmlDoc As New HtmlDocument()
        htmlDoc.LoadHtml(html)

        ' отримання початкового елементу вкладеного переліку
        Dim rootUl As HtmlNode = htmlDoc.DocumentNode.SelectSingleNode("//ul[@class='node-tree']")

        'отримання коренних вузлів переліку
        Dim Childs_nodes As HtmlNodeCollection = rootUl.ChildNodes

        'для кожного вузла, виклик рекурсивного пошуку, якщо вузол має необхідку назву
        For Each node As HtmlNode In Childs_nodes
            If node.Name = "li" Then
                'pathList_f.AddRange(GetPathList(node).Item1)
                'linksList_f.AddRange(GetPathList(node).Item2)
                'NamesList_f.AddRange(GetPathList(node).Item3)
                'guidlist_f.AddRange(GetPathList(node).Item4)
                Dim result As Tuple(Of List(Of String), List(Of String), List(Of String), List(Of String)) = GetPathList(node)

                pathList_f.AddRange(result.Item1)
                linksList_f.AddRange(result.Item2)
                NamesList_f.AddRange(result.Item3)
                guidlist_f.AddRange(result.Item4)


                Application.DoEvents()
            End If
        Next

        ' повернення сформованих переліків
        ' Return (pathList_f, linksList_f, NamesList_f, guidlist_f)
        Return Tuple.Create(pathList_f, linksList_f, NamesList_f, guidlist_f)

    End Function

    'функція вичитування html файлу та побудови шляхів для файлів/папок
    Sub parse_links()

        Dim htmlContent As String               'документ, що обробляється
        Dim myList As New List(Of String)()     'тимчасовий перелік шляхів для видалення останньої частини
        Dim modifiedString As String            'змінений рядок шляху, без назви файлу

        Dim filePath As String = "D:\Work\Programing\2023_07_Trans_web\links_test.html"
        ' Dim filePath As String = "D:\Work\Programing\2023_07_Trans_web\links2.html"

        'діалог відкриття файлу
        If OpenFileDialog1.ShowDialog = DialogResult.OK Then
            filePath = OpenFileDialog1.FileName
        End If

        'очищення переліків
        pathList.Clear()
        linksList.Clear()
        NamesList.Clear()
        guidlist.Clear()

        ' Create a StreamReader object to read the file
        Using reader As New StreamReader(filePath)
            ' Read the entire contents of the file
            htmlContent = reader.ReadToEnd()
        End Using

        'виклик рекурсивної функції парсингу 
        'pathList = ConvertToPathList(htmlContent).Item1
        'linksList = ConvertToPathList(htmlContent).Item2
        'NamesList = ConvertToPathList(htmlContent).Item3
        'guidlist = ConvertToPathList(htmlContent).Item4

        Dim result As Tuple(Of List(Of String), List(Of String), List(Of String), List(Of String)) = ConvertToPathList(htmlContent)

        pathList = result.Item1
        linksList = result.Item2
        NamesList = result.Item3
        guidlist = result.Item4

        abs_pathList = pathList

        Dim count_rows As Integer = pathList.Count

        'видалення із переліку шляхів назв файлів
        If count_rows > 0 Then
            For i = 0 To count_rows - 1
                'пошук останнього індексу входження розділяючого символу
                Dim index As Integer = pathList(i).LastIndexOf("\")
                'підрахунок довжини, що залишається
                Dim charactersToRemove As Integer = pathList(i).Length - index
                If index > 0 Then
                    'аидалення зайвої частини
                    modifiedString = pathList(i).Remove(index, charactersToRemove)
                    myList.Add(modifiedString)
                Else
                    modifiedString = ""
                    myList.Add(modifiedString)
                End If

            Next
        End If

        'оновленя переліку модифікованими шляхами
        pathList = myList

    End Sub


    'функція відображення переліку кореневих папок
    Sub draw_checkbox()

        Dim count_rows As Integer
        Dim modifiedString As String
        Dim myList As New List(Of String)

        count_rows = pathList.Count

        If count_rows > 0 Then
            For i = 0 To count_rows - 1

                'пошук індексу першого входження символу \, який поділяє шлях на папки
                Dim index As Integer = pathList(i).IndexOf("\")
                'підрахунок довжини символів, яку треба видалити після символу
                Dim charactersToRemove As Integer = pathList(i).Length - index
                If index > 0 Then
                    'видалення зайвих символів
                    modifiedString = pathList(i).Remove(index, charactersToRemove)
                    myList.Add(modifiedString)
                Else
                    'якщо це коренева папка, то залишається такою ж
                    modifiedString = pathList(i)
                    myList.Add(modifiedString)
                End If
            Next
        End If

        'створення унікального переліку імен
        myList = myList.Distinct().ToList()

        'очищення попереднього переліку
        CheckedListBox1.Items.Clear()

        'створення пункту у CheckedListBox для кожного імені
        For Each n In myList
            If n <> "" Then
                CheckedListBox1.Items.Add(n)
            End If
        Next

    End Sub


    'операція створення бібліотеки папок
    Sub Create_folders(start_folder As String)

        Dim myList As New List(Of String)()

        'створення унікального переліку шляхів
        myList = pathList.Distinct().ToList()

        For Each path As String In myList
            Directory.CreateDirectory(start_folder & "\" & path)
        Next

    End Sub


    'запуск вичитування файлу з посиланнями та відображення переліку кореневих папок 
    Private Sub Parse_links_button_Click(sender As Object, e As EventArgs) Handles Parse_links_button.Click

        parse_links()       'розбирання посилань
        draw_checkbox()     'відображення чекбоксу

    End Sub


    'тестовий виклик збереження певного файлу за певним посиланням
    Private Sub Save_page_button_Click(sender As Object, e As EventArgs) Handles Save_page_button.Click

        SavePageAsHtml("https://help.autodesk.com/view/fusion360/ENU/?guid=Update_Desktop_Connector", "d:\test\123.html")

    End Sub


    'окремий запуск створення каталогу папок
    Private Sub Create_folder_button_Click(sender As Object, e As EventArgs) Handles Create_folder_button.Click

        If FolderBrowserDialog1.ShowDialog() = DialogResult.OK Then
            Create_folders(FolderBrowserDialog1.SelectedPath)
        End If

    End Sub


    'запуск вичитки з файлу, створення папок та збереження сторінок
    Private Sub Run_all_Click(sender As Object, e As EventArgs) Handles Run_all.Click

        Dim count_rows As Integer
        Dim savePath As String
        Dim check_exist As Boolean

        parse_links()       'розбір файлу з посиланнями

        count_rows = pathList.Count

        'створення каталогу папок на основі отриманного файлу
        If FolderBrowserDialog1.ShowDialog() = DialogResult.OK Then
            Create_folders(FolderBrowserDialog1.SelectedPath)

            Dim allFiles As IEnumerable(Of String) = Directory.EnumerateFiles(FolderBrowserDialog1.SelectedPath, "*.html", SearchOption.AllDirectories)
            Dim filePaths As IList(Of String) = allFiles.ToList()


            'для кожного рядка виконання функції зберігання файлу
            If count_rows > 0 Then
                For i = 0 To count_rows - 1

                    'індикація процесу
                    ProgressBar1.Value = Math.Round(100 * (i + 1) / count_rows)
                    Label1.Text = CStr((i + 1) & " / " & count_rows)

                    If linksList(i) IsNot "null" Then

                        savePath = FolderBrowserDialog1.SelectedPath & "\" & pathList(i) & "\" & NamesList(i) & ".html"
                        check_exist = False

                        For Each file As String In filePaths
                            If file = savePath Then
                                check_exist = True
                            End If
                        Next

                        If check_exist = False Then
                            SavePageAsHtml(linksList(i), savePath)       'виклик збереження сторінок
                        End If

                        Application.DoEvents()

                    Else

                        If pathList(i) = "" Then
                            savePath = FolderBrowserDialog1.SelectedPath & "\" & NamesList(i) & ".html"
                        Else
                            savePath = FolderBrowserDialog1.SelectedPath & "\" & pathList(i) & "\" & NamesList(i) & ".html"
                        End If


                        check_exist = False

                        For Each file As String In filePaths
                            If file = savePath Then
                                check_exist = True
                            End If
                        Next

                        If check_exist = False Then

                            Dim htmlContent As String = "<div class=""body_content"" id=""body-content""><div class=""head-block""><h1 itemprop=""name"">" & NamesList(i) & "</h1></div>"

                            Using writer As New StreamWriter(savePath)
                                ' Write the HTML content to the file
                                writer.Write(htmlContent)
                            End Using

                        End If



                    End If
                Next
            End If
        End If

    End Sub

    Private Sub Button1_Click(sender As Object, e As EventArgs) Handles Button1.Click

        Dim files_count As Integer
        Dim cur_file As Integer = 0
        Dim htmlContent As String
        Dim save_path As String


        'діалог відкриття файлу
        If FolderBrowserDialog1.ShowDialog() = DialogResult.OK Then


            Dim allFiles As IEnumerable(Of String) = Directory.EnumerateFiles(FolderBrowserDialog1.SelectedPath, "*.html", SearchOption.AllDirectories)
            Dim filePaths As IList(Of String) = allFiles.ToList()

            Create_folders(FolderBrowserDialog1.SelectedPath & "_ukrainian")


            files_count = filePaths.Count

            If files_count = 0 Then
                MessageBox.Show("File not found")
            Else
                For Each filePath As String In filePaths

                    Label1.Text = CStr((cur_file + 1) & " / " & files_count)
                    ProgressBar1.Value = Math.Round(100 * (cur_file + 1) / files_count)



                    Using reader As New StreamReader(filePath)
                        ' Read the entire contents of the file
                        htmlContent = reader.ReadToEnd()
                    End Using

                    Dim pattern As String = FolderBrowserDialog1.SelectedPath & "\"
                    Dim replacement As String = FolderBrowserDialog1.SelectedPath & "_ukrainian\"

                    save_path = filePath.Replace(pattern, replacement)

                    translate(htmlContent, save_path)

                    cur_file += 1

                    Application.DoEvents()

                Next
            End If

        End If



    End Sub

    Private Sub Button2_Click(sender As Object, e As EventArgs) Handles SQL_Import.Click

        Dim files_count As Integer
        Dim cur_file As Integer = 0
        Dim rowcount As Integer
        Dim link_name As String
        Dim guid As String
        Dim file_name As String
        Dim checked_path As String


        parse_links()

        'діалог відкриття файлу
        If FolderBrowserDialog1.ShowDialog() = DialogResult.OK Then


            Dim allFiles As IEnumerable(Of String) = Directory.EnumerateFiles(FolderBrowserDialog1.SelectedPath, "*.html", SearchOption.AllDirectories)
            Dim filePaths As IList(Of String) = allFiles.ToList()


            files_count = filePaths.Count
            rowcount = NamesList.Count

            If files_count = 0 Then
                MessageBox.Show("File not found")
            Else
                For Each filePath As String In filePaths

                    Label1.Text = CStr((cur_file + 1) & " / " & files_count)
                    ProgressBar1.Value = Math.Round(100 * (cur_file + 1) / files_count)


                    '  Dim index As Integer = filePath.LastIndexOf("\")
                    'видалення зайвої частини
                    '  file_name = filePath.Remove(0, index + 1)
                    file_name = filePath.Replace(FolderBrowserDialog1.SelectedPath & "\", "")


                    For i = 0 To rowcount - 1

                        checked_path = abs_pathList(i) & ".html"

                        If file_name = checked_path Then

                            guid = guidlist(i)
                            link_name = NamesList(i).Replace(" ", "_")
                            link_name = link_name.ToLower()
                            link_name = Regex.Replace(link_name, "[^a-zA-Z0-9_]", "")
                            start_import(filePath, guid, link_name)
                        End If

                    Next

                    cur_file += 1

                    Application.DoEvents()

                Next
            End If

        End If


    End Sub

    Private Sub Sort_sql_Click(sender As Object, e As EventArgs) Handles Sort_sql.Click

        parse_links()
        hierarchical_structure_SQL()

    End Sub

    Private Sub Button2_Click_1(sender As Object, e As EventArgs) Handles Button2.Click

        Dim filePath As String = "C:\Users\Lost\Desktop\Fusion help\Electronics\Reference\Electronics command-line reference.html"
        Dim htmlContent As String
        Dim chunks As New List(Of String)()
        Dim chunkSize As Integer = 180000


        Using reader As New StreamReader(filePath)
            ' Read the entire contents of the file
            htmlContent = reader.ReadToEnd()
        End Using


        translate(htmlContent, "D:\test\splited.html")




    End Sub
End Class