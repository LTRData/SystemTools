Imports System.Reflection
Imports System.Text

Public NotInheritable Class AboutBox

    Protected Overrides Sub OnLoad(e As EventArgs)
        MyBase.OnLoad(e)

        Dim asm = Assembly.GetExecutingAssembly()
        Dim productName = CType(asm.GetCustomAttributes(GetType(AssemblyProductAttribute), False)(0), AssemblyProductAttribute).Product
        Dim title = CType(asm.GetCustomAttributes(GetType(AssemblyTitleAttribute), False)(0), AssemblyTitleAttribute).Title
        Dim fileVersion = CType(asm.GetCustomAttributes(GetType(AssemblyFileVersionAttribute), False)(0), AssemblyFileVersionAttribute).Version
        Dim legalCopyright = CType(asm.GetCustomAttributes(GetType(AssemblyCopyrightAttribute), False)(0), AssemblyCopyrightAttribute).Copyright
        Dim fileDescription = CType(asm.GetCustomAttributes(GetType(AssemblyDescriptionAttribute), False)(0), AssemblyDescriptionAttribute).Description

        ' Set the title of the form.
        Dim ApplicationTitle As String
        If title <> "" Then
            ApplicationTitle = title
        Else
            ApplicationTitle = IO.Path.GetFileNameWithoutExtension(asm.Location)
        End If
        Text = $"About {ApplicationTitle}"
        ' Initialize all of the text displayed on the About Box.
        ' TODO: Customize the application's assembly information in the "Application" pane of the project 
        '    properties dialog (under the "Project" menu).
        LabelProductName.Text = productName
        LabelVersion.Text = $"Version {fileVersion}"
        LabelCopyright.Text = legalCopyright
        LabelCompanyName.Text = CompanyName

        Dim sb = New StringBuilder().
            AppendLine(fileDescription).
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
