<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()> _
Partial Class GUI
    Inherits System.Windows.Forms.Form

    'Форма переопределяет dispose для очистки списка компонентов.
    <System.Diagnostics.DebuggerNonUserCode()> _
    Protected Overrides Sub Dispose(ByVal disposing As Boolean)
        Try
            If disposing AndAlso components IsNot Nothing Then
                components.Dispose()
            End If
        Finally
            MyBase.Dispose(disposing)
        End Try
    End Sub

    'Является обязательной для конструктора форм Windows Forms
    Private components As System.ComponentModel.IContainer

    'Примечание: следующая процедура является обязательной для конструктора форм Windows Forms
    'Для ее изменения используйте конструктор форм Windows Form.  
    'Не изменяйте ее в редакторе исходного кода.
    <System.Diagnostics.DebuggerStepThrough()> _
    Private Sub InitializeComponent()
        Me.Save_page_button = New System.Windows.Forms.Button()
        Me.Parse_links_button = New System.Windows.Forms.Button()
        Me.Create_folder_button = New System.Windows.Forms.Button()
        Me.FolderBrowserDialog1 = New System.Windows.Forms.FolderBrowserDialog()
        Me.Run_all = New System.Windows.Forms.Button()
        Me.ProgressBar1 = New System.Windows.Forms.ProgressBar()
        Me.Label1 = New System.Windows.Forms.Label()
        Me.CheckedListBox1 = New System.Windows.Forms.CheckedListBox()
        Me.OpenFileDialog1 = New System.Windows.Forms.OpenFileDialog()
        Me.Button1 = New System.Windows.Forms.Button()
        Me.Button2 = New System.Windows.Forms.Button()
        Me.SuspendLayout()
        '
        'Save_page_button
        '
        Me.Save_page_button.Anchor = CType((System.Windows.Forms.AnchorStyles.Bottom Or System.Windows.Forms.AnchorStyles.Left), System.Windows.Forms.AnchorStyles)
        Me.Save_page_button.Location = New System.Drawing.Point(290, 264)
        Me.Save_page_button.Name = "Save_page_button"
        Me.Save_page_button.Size = New System.Drawing.Size(135, 38)
        Me.Save_page_button.TabIndex = 0
        Me.Save_page_button.Text = "Call and Save page"
        Me.Save_page_button.UseVisualStyleBackColor = True
        '
        'Parse_links_button
        '
        Me.Parse_links_button.Anchor = CType((System.Windows.Forms.AnchorStyles.Bottom Or System.Windows.Forms.AnchorStyles.Left), System.Windows.Forms.AnchorStyles)
        Me.Parse_links_button.Location = New System.Drawing.Point(150, 264)
        Me.Parse_links_button.Name = "Parse_links_button"
        Me.Parse_links_button.Size = New System.Drawing.Size(120, 38)
        Me.Parse_links_button.TabIndex = 1
        Me.Parse_links_button.Text = "Parse links html"
        Me.Parse_links_button.UseVisualStyleBackColor = True
        '
        'Create_folder_button
        '
        Me.Create_folder_button.Anchor = CType((System.Windows.Forms.AnchorStyles.Bottom Or System.Windows.Forms.AnchorStyles.Left), System.Windows.Forms.AnchorStyles)
        Me.Create_folder_button.Location = New System.Drawing.Point(11, 264)
        Me.Create_folder_button.Name = "Create_folder_button"
        Me.Create_folder_button.Size = New System.Drawing.Size(113, 38)
        Me.Create_folder_button.TabIndex = 2
        Me.Create_folder_button.Text = "Create folders"
        Me.Create_folder_button.UseVisualStyleBackColor = True
        '
        'Run_all
        '
        Me.Run_all.BackColor = System.Drawing.Color.Firebrick
        Me.Run_all.ForeColor = System.Drawing.SystemColors.ButtonHighlight
        Me.Run_all.Location = New System.Drawing.Point(12, 12)
        Me.Run_all.Name = "Run_all"
        Me.Run_all.Size = New System.Drawing.Size(414, 153)
        Me.Run_all.TabIndex = 3
        Me.Run_all.Text = "BIG RED BUTTON"
        Me.Run_all.UseVisualStyleBackColor = False
        '
        'ProgressBar1
        '
        Me.ProgressBar1.Location = New System.Drawing.Point(12, 171)
        Me.ProgressBar1.Name = "ProgressBar1"
        Me.ProgressBar1.Size = New System.Drawing.Size(414, 23)
        Me.ProgressBar1.TabIndex = 4
        '
        'Label1
        '
        Me.Label1.AutoSize = True
        Me.Label1.Location = New System.Drawing.Point(194, 197)
        Me.Label1.Name = "Label1"
        Me.Label1.Size = New System.Drawing.Size(39, 13)
        Me.Label1.TabIndex = 5
        Me.Label1.Text = "Label1"
        '
        'CheckedListBox1
        '
        Me.CheckedListBox1.Anchor = CType((((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Bottom) _
            Or System.Windows.Forms.AnchorStyles.Left) _
            Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.CheckedListBox1.CheckOnClick = True
        Me.CheckedListBox1.FormattingEnabled = True
        Me.CheckedListBox1.HorizontalScrollbar = True
        Me.CheckedListBox1.Location = New System.Drawing.Point(432, 12)
        Me.CheckedListBox1.Name = "CheckedListBox1"
        Me.CheckedListBox1.Size = New System.Drawing.Size(169, 289)
        Me.CheckedListBox1.TabIndex = 6
        '
        'OpenFileDialog1
        '
        Me.OpenFileDialog1.FileName = "OpenFileDialog1"
        '
        'Button1
        '
        Me.Button1.Location = New System.Drawing.Point(291, 220)
        Me.Button1.Name = "Button1"
        Me.Button1.Size = New System.Drawing.Size(135, 38)
        Me.Button1.TabIndex = 7
        Me.Button1.Text = "Run translate"
        Me.Button1.UseVisualStyleBackColor = True
        '
        'Button2
        '
        Me.Button2.Location = New System.Drawing.Point(12, 220)
        Me.Button2.Name = "Button2"
        Me.Button2.Size = New System.Drawing.Size(75, 23)
        Me.Button2.TabIndex = 8
        Me.Button2.Text = "Button2"
        Me.Button2.UseVisualStyleBackColor = True
        '
        'GUI
        '
        Me.AutoScaleDimensions = New System.Drawing.SizeF(6.0!, 13.0!)
        Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
        Me.ClientSize = New System.Drawing.Size(609, 312)
        Me.Controls.Add(Me.Button2)
        Me.Controls.Add(Me.Button1)
        Me.Controls.Add(Me.CheckedListBox1)
        Me.Controls.Add(Me.Label1)
        Me.Controls.Add(Me.ProgressBar1)
        Me.Controls.Add(Me.Run_all)
        Me.Controls.Add(Me.Create_folder_button)
        Me.Controls.Add(Me.Parse_links_button)
        Me.Controls.Add(Me.Save_page_button)
        Me.MinimumSize = New System.Drawing.Size(625, 305)
        Me.Name = "GUI"
        Me.Text = "GUI"
        Me.ResumeLayout(False)
        Me.PerformLayout()

    End Sub

    Friend WithEvents Save_page_button As Button
    Friend WithEvents Parse_links_button As Button
    Friend WithEvents Create_folder_button As Button
    Friend WithEvents FolderBrowserDialog1 As FolderBrowserDialog
    Friend WithEvents Run_all As Button
    Friend WithEvents ProgressBar1 As ProgressBar
    Friend WithEvents Label1 As Label
    Friend WithEvents CheckedListBox1 As CheckedListBox
    Friend WithEvents OpenFileDialog1 As OpenFileDialog
    Friend WithEvents Button1 As Button
    Friend WithEvents Button2 As Button
End Class
