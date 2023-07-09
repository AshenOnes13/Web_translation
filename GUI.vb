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

Public Class GUI

    Dim pathList As New List(Of String)     'перелік шляхів всіх сторінок
    Dim linksList As New List(Of String)    'перелік посилань на сайт https://help.autodesk.com/
    Dim NamesList As New List(Of String)    'перелік назв сторінок

    Const GoogleCloudApiKey = "AIzaSyA_lbC-g1PfLkg6nskzmd0tJ0NagGhQ-D0"

    Dim connectionString As String = "server=localhost;user=lostsoul;password=elparol3572765;database=fusion_translate_db;"

    Sub start_import(filePath As String, link As String)

        Dim connection As New MySqlConnection(connectionString)
        Dim Filename As String
        connection.Open()

        '  Dim query As String = "SELECT * FROM `wp_posts` ORDER BY `post_title` ASC;"

        Dim htmlContent As String               'документ, що обробляється


        ' If OpenFileDialog1.ShowDialog = DialogResult.OK Then
        '  filePath = OpenFileDialog1.FileName
        '  Filename = OpenFileDialog1.SafeFileName


        Using reader1 As New StreamReader(filePath, Encoding.Default)
                ' Read the entire contents of the file
                htmlContent = reader1.ReadToEnd()
            End Using

            Dim htmlDoc As New HtmlDocument()
            htmlDoc.LoadHtml(htmlContent)

            ' отримання початкового елементу вкладеного переліку
            Dim rootUl As HtmlNode = htmlDoc.DocumentNode.SelectSingleNode("//h1[@itemprop='name']")

            Filename = rootUl.InnerText


            Dim ID As String = "NULL"
            Dim post_author As String = 1
            Dim post_date As String = "2023-07-01 21:27:58"
            Dim post_date_gmt As String = "2023-07-01 21:27:58"
            Dim post_content As String = htmlContent
            Dim post_title As String = Filename
            Dim post_excerpt As String = ""
            Dim post_status As String = "publish"
            Dim comment_status As String = "closed"
            Dim ping_status As String = "closed"
            Dim post_password As String = ""
            Dim post_name As String = link
            Dim to_ping As String = ""
            Dim pinged As String = ""
            Dim post_modified As String = "2023-07-01 21:27:58"
            Dim post_modified_gmt As String = "2023-07-01 21:27:58"
            Dim post_content_filtered As String = ""
            Dim post_parent As String = "7"
            Dim guid As String = ""
            Dim menu_order As String = "0"
            Dim post_type As String = "page"
            Dim post_mime_type As String = ""
            Dim comment_count As String = "0"



            Dim query As String = "INSERT INTO `wp_posts` (`ID`, `post_author`, `post_date`, `post_date_gmt`, `post_content`, `post_title`, `post_excerpt`, `post_status`, `comment_status`, `ping_status`, `post_password`, `post_name`, `to_ping`, `pinged`, `post_modified`, `post_modified_gmt`, `post_content_filtered`, `post_parent`, `guid`, `menu_order`, `post_type`, `post_mime_type`, `comment_count`) 
    VALUES (" & ID & ", '" & post_author & "', '" & post_date & "', '" & post_date_gmt & "', '" & post_content & "', '" & post_title & "', '" & post_excerpt & "', '" & post_status & "', '" & comment_status & "', '" & ping_status & "', '" & post_password & "', '" & post_name & "', '" & to_ping & "', '" & pinged & "', '" & post_modified & "', '" & post_modified_gmt & "', '" & post_content_filtered & "', '" & post_parent & "', '" & guid & "', '" & menu_order & "', '" & post_type & "', '" & post_mime_type & "', '" & comment_count & "');"

            Dim command As New MySqlCommand(query, connection)
            Dim reader As MySqlDataReader = command.ExecuteReader()

            reader.Close()



        ' End If

    End Sub













    Sub translate(htmlContent As String, savePath As String)

        'ініціалізація перекладача
        Dim translateClient As TranslationClient        'перекладач
        Dim response As TranslationResult               'місце для зберігання результату перекладу
        Dim temp_row As String
        Dim htmlSave As New HtmlDocument()
        translateClient = TranslationClient.CreateFromApiKey(GoogleCloudApiKey)


        response = translateClient.TranslateHtml(htmlContent, LanguageCodes.Ukrainian)

        temp_row = response.TranslatedText


        ' зберігання файлу
        htmlSave.LoadHtml(temp_row)

        htmlSave.Save(savePath)


    End Sub




    'збереження html файлів з використанням Chrome та бібліотеки Selenium
    Public Sub SavePageAsHtml(url As String, savePath As String, saveName As String)

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
            rootUl = htmlDoc.DocumentNode.SelectSingleNode("//div[@class='body_content']")
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
            SavePageAsHtml(url, savePath, saveName)

        Else

            ' зберігання файлу
            htmlSave.DocumentNode.AppendChild(rootUl)

            Dim nodeToDelete As HtmlNode = htmlSave.DocumentNode.SelectSingleNode("//div[@data-type='Sharing']")

            If nodeToDelete IsNot Nothing Then
                ' Delete the node
                nodeToDelete.Remove()
            End If

            savePath += "\" + saveName + ".html"
            htmlSave.Save(savePath)


            ' Close the browser
            driver.Quit()

        End If

    End Sub


    'рекурсивний пошук вкладених елементів
    Private Shared Function GetPathList(node As HtmlNode, Optional parentPath As String = "") As (List(Of String), List(Of String), List(Of String))
        'оголошення відповідних переліків для парсингу
        Dim pathList_f As New List(Of String)()
        Dim LinksList_f As New List(Of String)()
        Dim NamesList_f As New List(Of String)()
        'ім'я поточного вузла
        Dim nodeText As String

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
                        pathList_f.AddRange(GetPathList(node_ul, path).Item1)
                        LinksList_f.AddRange(GetPathList(node_ul, path).Item2)
                        NamesList_f.AddRange(GetPathList(node_ul, path).Item3)
                    End If
                Next

            End If
        Next

        ' повернення сформованих переліків
        Return (pathList_f, LinksList_f, NamesList_f)
    End Function


    'функція пошуку коренних вузлів переліку у html документі
    Public Shared Function ConvertToPathList(html As String) As (List(Of String), List(Of String), List(Of String))
        'оголошення відповідних переліків для парсингу
        Dim pathList_f As New List(Of String)()
        Dim linksList_f As New List(Of String)()
        Dim NamesList_f As New List(Of String)()

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
                pathList_f.AddRange(GetPathList(node).Item1)
                linksList_f.AddRange(GetPathList(node).Item2)
                NamesList_f.AddRange(GetPathList(node).Item3)
            End If
        Next

        ' повернення сформованих переліків
        Return (pathList_f, linksList_f, NamesList_f)

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

        ' Create a StreamReader object to read the file
        Using reader As New StreamReader(filePath)
            ' Read the entire contents of the file
            htmlContent = reader.ReadToEnd()
        End Using

        'виклик рекурсивної функції парсингу 
        pathList = ConvertToPathList(htmlContent).Item1
        linksList = ConvertToPathList(htmlContent).Item2
        NamesList = ConvertToPathList(htmlContent).Item3


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

        SavePageAsHtml("https://help.autodesk.com/view/fusion360/ENU/?guid=Update_Desktop_Connector", "d:\test\", "asd")

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

        parse_links()       'розбір файлу з посиланнями

        count_rows = pathList.Count

        'створення каталогу папок на основі отриманного файлу
        If FolderBrowserDialog1.ShowDialog() = DialogResult.OK Then
            Create_folders(FolderBrowserDialog1.SelectedPath)

            'для кожного рядка виконання функції зберігання файлу
            If count_rows > 0 Then
                For i = 0 To count_rows - 1
                    If linksList(i) IsNot "null" Then
                        SavePageAsHtml(linksList(i), FolderBrowserDialog1.SelectedPath & "\" & pathList(i), NamesList(i))       'виклик збереження сторінок

                        'індикація процесу
                        ProgressBar1.Value = Math.Round(100 * (i + 1) / count_rows)
                        Label1.Text = CStr((i + 1) & " / " & count_rows)

                        Application.DoEvents()

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

    Private Sub Button2_Click(sender As Object, e As EventArgs) Handles Button2.Click

        Dim files_count As Integer
        Dim cur_file As Integer = 0
        Dim rowcount As Integer
        Dim link As String


        'діалог відкриття файлу
        If FolderBrowserDialog1.ShowDialog() = DialogResult.OK Then


            Dim allFiles As IEnumerable(Of String) = Directory.EnumerateFiles(FolderBrowserDialog1.SelectedPath, "*.html", SearchOption.AllDirectories)
            Dim filePaths As IList(Of String) = allFiles.ToList()

            Create_folders(FolderBrowserDialog1.SelectedPath & "_ukrainian")

            files_count = filePaths.Count
            rowcount = NamesList.Count

            If files_count = 0 Then
                MessageBox.Show("File not found")
            Else
                For Each filePath As String In filePaths

                    Label1.Text = CStr((cur_file + 1) & " / " & files_count)


                    For i = 0 To rowcount - 1




                    Next


                    start_import(filePath, link)

                    cur_file += 1

                    Application.DoEvents()

                Next
            End If

        End If




    End Sub

End Class