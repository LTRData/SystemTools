Imports System.Reflection
Imports System.Text

Public NotInheritable Class AboutBox

    Protected Overrides Sub OnLoad(e As EventArgs)
        MyBase.OnLoad(e)

        Dim asm = Process.GetCurrentProcess().MainModule.FileVersionInfo

        ' Set the title of the form.
        Dim ApplicationTitle As String
        If asm.ProductName <> "" Then
            ApplicationTitle = asm.ProductName
        Else
            ApplicationTitle = System.IO.Path.GetFileNameWithoutExtension(asm.FileName)
        End If
        Text = $"About {ApplicationTitle }"
        ' Initialize all of the text displayed on the About Box.
        ' TODO: Customize the application's assembly information in the "Application" pane of the project 
        '    properties dialog (under the "Project" menu).
        LabelProductName.Text = asm.ProductName
        LabelVersion.Text = $"Version {asm.FileVersion }"
        LabelCopyright.Text = asm.LegalCopyright
        LabelCompanyName.Text = asm.CompanyName

        Dim sb = New StringBuilder().
            AppendLine(asm.FileDescription).
            AppendLine().
            Append("Runtime version ").
            AppendLine(Environment.Version.ToString()).
            AppendLine().
            Append("OS version ").
            AppendLine(Environment.OSVersion.ToString())

        TextBoxDescription.Text = sb.ToString()
    End Sub

    Private Sub OKButton_Click(sender As System.Object, e As System.EventArgs) Handles OKButton.Click
        Close()
    End Sub

    Private Sub LinkLabel_LinkClicked(sender As System.Object, e As System.Windows.Forms.LinkLabelLinkClickedEventArgs) Handles LinkLabel.LinkClicked
        Process.Start(New ProcessStartInfo With {.FileName = "http://www.ltr-data.se", .UseShellExecute = True})?.Dispose()
    End Sub
End Class
