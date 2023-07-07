Imports System.IO
Imports System.Net
Imports HtmlAgilityPack

Imports System.Collections.Generic

Imports OpenQA.Selenium
Imports OpenQA.Selenium.Chrome
Imports OpenQA.Selenium.Support.UI

Imports System.Text.RegularExpressions

Public Class GUI

    Dim pathList As New List(Of String)     'перелік шляхів всіх сторінок
    Dim linksList As New List(Of String)    'перелік посилань на сайт https://help.autodesk.com/
    Dim NamesList As New List(Of String)    'перелік назв сторінок


    Public Sub SavePageAsHtml(url As String, savePath As String, saveName As String)

        Dim options As New ChromeOptions()
        Dim isDisplayed As Boolean = False
        Dim rootUl As HtmlNode

        Dim htmlDoc As New HtmlDocument()
        Dim htmlSave As New HtmlDocument()

        ' options.AddArgument("--headless") ' Run Chrome in headless mode (without GUI)
        Dim driver As New ChromeDriver(options)

        driver.Navigate().GoToUrl(url)

        ' Wait for the page to load 
        Threading.Thread.Sleep(1000)


        While isDisplayed = False

            ' Capture the rendered HTML
            Dim html As String = driver.PageSource
            htmlDoc.LoadHtml(html)

            rootUl = htmlDoc.DocumentNode.SelectSingleNode("//div[@class='body_content']")

            If rootUl IsNot Nothing Then
                isDisplayed = True
            Else
                isDisplayed = False
                Threading.Thread.Sleep(1000)
            End If

        End While

        ' Save the HTML to a file
        htmlSave.DocumentNode.AppendChild(rootUl)
        savePath += saveName + ".html"
        htmlSave.Save(savePath)


        ' Close the browser
        driver.Quit()
    End Sub

    Private Sub Save_page_button_Click(sender As Object, e As EventArgs) Handles Save_page_button.Click

        SavePageAsHtml("https://help.autodesk.com/view/fusion360/ENU/?guid=Update_Desktop_Connector", "d:\test\", "asd")

    End Sub



    Private Shared Function GetPathList(node As HtmlNode, Optional parentPath As String = "") As (List(Of String), List(Of String), List(Of String))
        Dim pathList_f As New List(Of String)()
        Dim LinksList_f As New List(Of String)()
        Dim NamesList_f As New List(Of String)()
        Dim nodeText As String

        ' Get the text of the current node


        Dim Childs_nodes As HtmlNodeCollection = node.ChildNodes

        Dim selectedNode As HtmlNode = Childs_nodes.FirstOrDefault(Function(n) n.Name = "a")

        nodeText = selectedNode.InnerText.Trim().Replace(":", "")


        Dim dataUrlValue As String = selectedNode.GetAttributeValue("data-url", "null")

        If dataUrlValue IsNot "null" Then
            dataUrlValue = ("https://help.autodesk.com" & dataUrlValue)
        End If

        LinksList_f.Add(dataUrlValue)

        NamesList_f.Add(nodeText)

        ' Build the path by appending the current node text to the parent path
        Dim path As String = If(String.IsNullOrEmpty(parentPath), nodeText, parentPath & "\" & nodeText)

        ' Add the path to the path list
        pathList_f.Add(path)

        ' Recursively process the child nodes
        For Each child As HtmlNode In Childs_nodes
            If child.Name = "ul" Then

                Dim Childs_UL As HtmlNodeCollection = child.ChildNodes

                For Each node_ul As HtmlNode In Childs_UL
                    If node_ul.Name = "li" Then
                        pathList_f.AddRange(GetPathList(node_ul, path).Item1)
                        LinksList_f.AddRange(GetPathList(node_ul, path).Item2)
                        NamesList_f.AddRange(GetPathList(node_ul, path).Item3)
                    End If
                Next

            End If
        Next

        Return (pathList_f, LinksList_f, NamesList_f)
    End Function



    Public Shared Function ConvertToPathList(html As String) As (List(Of String), List(Of String), List(Of String))
        Dim pathList_f As New List(Of String)()
        Dim linksList_f As New List(Of String)()
        Dim NamesList_f As New List(Of String)()

        ' Parse the HTML string into an HTML document
        Dim htmlDoc As New HtmlDocument()
        htmlDoc.LoadHtml(html)

        ' Get the root <ul> element
        Dim rootUl As HtmlNode = htmlDoc.DocumentNode.SelectSingleNode("//ul[@class='node-tree']")

        '   Dim rootUl As HtmlNodeCollection = htmlDoc.DocumentNode.SelectNodes("//ul[@class='node-tree']")

        Dim Childs_nodes As HtmlNodeCollection = rootUl.ChildNodes


        For Each node As HtmlNode In Childs_nodes
            If node.Name = "li" Then
                pathList_f.AddRange(GetPathList(node).Item1)
                linksList_f.AddRange(GetPathList(node).Item2)
                NamesList_f.AddRange(GetPathList(node).Item3)
            End If
        Next

        ' Process the root <ul> element
        Return (pathList_f, linksList_f, NamesList_f)

    End Function


    Sub parse_links()

        Dim htmlContent As String
        Dim myList As New List(Of String)()
        Dim modifiedString As String
        ' Dim filePath As String = "D:\Work\Programing\2023_07_Trans_web\links_test.html"
        Dim filePath As String = "D:\Work\Programing\2023_07_Trans_web\links2.html"

        pathList.Clear()
        linksList.Clear()
        NamesList.Clear()

        ' Create a StreamReader object to read the file
        Using reader As New StreamReader(filePath)
            ' Read the entire contents of the file
            htmlContent = reader.ReadToEnd()
        End Using

        pathList = ConvertToPathList(htmlContent).Item1
        linksList = ConvertToPathList(htmlContent).Item2
        NamesList = ConvertToPathList(htmlContent).Item3

        Dim count_rows As Integer = pathList.Count

        'If count_rows > 0 Then
        '    For i = 0 To count_rows - 1
        '        modifiedString = pathList(i).Replace(NamesList(i), "")
        '        myList.Add(modifiedString)
        '    Next
        'End If

        If count_rows > 0 Then
            For i = 0 To count_rows - 1

                Dim index As Integer = pathList(i).LastIndexOf("\")
                Dim charactersToRemove As Integer = pathList(i).Length - index
                If index > 0 Then
                    ' Delete the substring from the beginning of the string up to the symbol
                    modifiedString = pathList(i).Remove(index, charactersToRemove)
                    myList.Add(modifiedString)
                Else
                    modifiedString = ""
                    myList.Add(modifiedString)
                End If

                '      Console.WriteLine(pathList(i) & " ||| " & modifiedString)
            Next
        End If


        pathList = myList

    End Sub



    Sub draw_checkbox()

        Dim count_rows As Integer
        Dim modifiedString As String
        Dim myList As New List(Of String)


        count_rows = pathList.Count


        If count_rows > 0 Then
            For i = 0 To count_rows - 1

                Dim index As Integer = pathList(i).IndexOf("\")
                Dim charactersToRemove As Integer = pathList(i).Length - index
                If index > 0 Then
                    ' Delete the substring from the beginning of the string up to the symbol
                    modifiedString = pathList(i).Remove(index, charactersToRemove)
                    myList.Add(modifiedString)
                Else
                    modifiedString = pathList(i)
                    myList.Add(modifiedString)
                End If

            Next
        End If

        myList = myList.Distinct().ToList()

        For Each n In myList
            If n <> "" Then
                CheckedListBox1.Items.Add(n)
            End If

        Next


    End Sub



    Sub Create_folders(start_folder As String)

        Dim myList As New List(Of String)()
        Dim roots_list As New List(Of String)()

        myList = pathList.Distinct().ToList()

        For Each path As String In myList
            Directory.CreateDirectory(start_folder & "\" & path)
        Next

    End Sub


    'запуск вичитування файлу з посиланнями та відображення переліку кореневих папок 
    Private Sub Parse_links_button_Click(sender As Object, e As EventArgs) Handles Parse_links_button.Click

        parse_links()
        draw_checkbox()

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
                        SavePageAsHtml(linksList(i), FolderBrowserDialog1.SelectedPath & "\" & pathList(i), NamesList(i))

                        'індикація процесу
                        ProgressBar1.Value = Math.Round(100 * (i + 1) / count_rows)
                        Label1.Text = CStr((i + 1) & " / " & count_rows)

                        Application.DoEvents()

                    End If
                Next
            End If
        End If

    End Sub


End Class