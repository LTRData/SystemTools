Imports System.Text

Public NotInheritable Class AboutBox

    Protected Overrides Sub OnLoad(e As EventArgs)
        MyBase.OnLoad(e)

        ' Set the title of the form.
        Dim ApplicationTitle As String
        If My.Application.Info.Title <> "" Then
            ApplicationTitle = My.Application.Info.Title
        Else
            ApplicationTitle = System.IO.Path.GetFileNameWithoutExtension(My.Application.Info.AssemblyName)
        End If
        Me.Text = String.Format("About {0}", ApplicationTitle)
        ' Initialize all of the text displayed on the About Box.
        ' TODO: Customize the application's assembly information in the "Application" pane of the project 
        '    properties dialog (under the "Project" menu).
        Me.LabelProductName.Text = My.Application.Info.ProductName
        Me.LabelVersion.Text = String.Format("Version {0}", My.Application.Info.Version.ToString())
        Me.LabelCopyright.Text = My.Application.Info.Copyright
        Me.LabelCompanyName.Text = My.Application.Info.CompanyName

        Dim sb As New StringBuilder
        sb.AppendLine(My.Application.Info.Description)
        sb.AppendLine()
        sb.Append("Runtime version ")
        sb.AppendLine(Environment.Version.ToString())
        sb.AppendLine()
        sb.Append("OS version ")
        sb.AppendLine(Environment.OSVersion.ToString())

        Me.TextBoxDescription.Text = sb.ToString()
    End Sub

    Private Sub OKButton_Click(sender As System.Object, e As System.EventArgs) Handles OKButton.Click
        Me.Close()
    End Sub

    Private Sub LinkLabel_LinkClicked(sender As System.Object, e As System.Windows.Forms.LinkLabelLinkClickedEventArgs) Handles LinkLabel.LinkClicked
        Process.Start("http://www.ltr-data.se")
    End Sub
End Class
