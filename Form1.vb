Imports System.IO
Imports System.Net
Imports HtmlAgilityPack

Imports OpenQA.Selenium
Imports OpenQA.Selenium.Chrome
Imports OpenQA.Selenium.Support.UI

Public Class Form1

    Sub testing_save()

        Dim request As WebRequest = WebRequest.Create("https://help.autodesk.com/view/fusion360/ENU/?guid=Send_Feedback")

        Using response As WebResponse = request.GetResponse()
            Using reader As New StreamReader(response.GetResponseStream())
                Dim html As String = reader.ReadToEnd()
                File.WriteAllText("D:\test.html", html)
            End Using
        End Using

    End Sub

    Sub test_2()

        Dim LocalFilePath As String = "d:\lcal.html"
        Dim objWebClient As New System.Net.WebClient
        objWebClient.DownloadFile("https://help.autodesk.com/view/fusion360/ENU/?guid=Update_Desktop_Connector", LocalFilePath)

    End Sub

    Private Sub navigation_complete(ByVal sender As System.Object, ByVal e As System.Windows.Forms.WebBrowserDocumentCompletedEventArgs)

        Dim HTMlAuthorCode As String = sender.DocumentText
        My.Computer.FileSystem.WriteAllText("d:\tempAuthorCode.html", HTMlAuthorCode, True)

        Dim strAuthorCode As String = sender.Document.Body.InnerText
        My.Computer.FileSystem.WriteAllText("d:\tempAuthorCode.txt", strAuthorCode, True)
        sender.Dispose()
    End Sub

    Sub test3_5()

        Dim objwebbrowser As New WebBrowser
        objwebbrowser.Navigate("https://help.autodesk.com/view/fusion360/ENU/?guid=Update_Desktop_Connector")
        AddHandler objwebbrowser.DocumentCompleted, AddressOf navigation_complete

    End Sub

    Private Sub Button1_Click(sender As Object, e As EventArgs) Handles Button1.Click

        test4()

    End Sub

    Sub test4()
        ' Create a new HtmlWeb instance
        Dim web As New HtmlWeb()

        ' Load the dynamic web page
        Dim doc As HtmlDocument = web.Load("https://help.autodesk.com/view/fusion360/ENU/?guid=Update_Desktop_Connector")

        Dim renderedHtml As String = doc.DocumentNode.OuterHtml

        Using writer As New StreamWriter("d:\test.html")
            writer.Write(renderedHtml)

        End Using
        ' Save the HTML content to a file
        '  doc.Save("d:\test.html")

    End Sub


    Public Sub SavePageAsHtml(url As String, savePath As String)
        Dim options As New ChromeOptions()
        '   options.AddArgument("--headless") ' Run Chrome in headless mode (without GUI)
        Dim driver As New ChromeDriver(options)

        driver.Navigate().GoToUrl(url)

        ' Wait for the page to fully load and execute JavaScript

        Dim wait As New WebDriverWait(driver, TimeSpan.FromSeconds(10))
        System.Threading.Thread.Sleep(1000)
        ' wait.Until(Function(d) CType(d, IJavaScriptExecutor).ExecuteScript("return document.readyState").Equals("complete"))


        wait.Until(Function(f) driver.FindElement(By.Id("body-content")).Displayed)
        ' wait.Until(Function(f) driver.FindElement(By.ClassName("caas")).Displayed)

        '        Help.autodesk.com###body-content
        'Help.autodesk.com##.body_content


        ' Capture the rendered HTML
        Dim html As String = driver.PageSource

        ' Save the HTML to a file
        System.IO.File.WriteAllText(savePath, html)

        ' Close the browser
        driver.Quit()
    End Sub

    Private Sub Button2_Click(sender As Object, e As EventArgs) Handles Button2.Click

        SavePageAsHtml("https://help.autodesk.com/view/fusion360/ENU/?guid=Update_Desktop_Connector", "d:\test.html")

    End Sub


End Class
