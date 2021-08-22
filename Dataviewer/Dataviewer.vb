Imports System.ComponentModel
Imports System.Data
Imports System.Threading
Imports System.Windows.Forms

#Disable Warning IDE1006 ' Naming Styles

Public Class Dataviewer

    Private Declare Function SQLManageDataSources Lib "ODBCCP32.dll" (hwnd As IntPtr) As Boolean

    Private BindingManager As BindingManagerBase
    Private DataAdapter As Odbc.OdbcDataAdapter
    Private WithEvents SourceTable As DataTable

    Public Sub SetSourceTable(DataTable As DataTable)
        If SourceTable IsNot Nothing Then
            SourceTable.Dispose()
        End If
        SourceTable = Nothing
        DataGridView.DataSource = DataTable
        SourceTable = DataTable
        DataGridView.ReadOnly = False
        btnDisconnect.Enabled = DataTable IsNot Nothing
    End Sub

    Private Sub DataTable_RowChanged(sender As Object, e As EventArgs) _
      Handles _
        SourceTable.RowChanged,
        SourceTable.RowDeleted,
        SourceTable.TableNewRow

        Try
            DataAdapter.Update(SourceTable)

        Catch ex As Exception
            Dim message = ex.GetBaseException().Message
            MessageBox.Show(Me, message, "Error updating data source", MessageBoxButtons.OK, MessageBoxIcon.Exclamation)

        End Try

    End Sub

    Protected Overrides Sub OnClosing(e As CancelEventArgs)
        MyBase.OnClosing(e)

        If BindingManager IsNot Nothing Then
            BindingManager.EndCurrentEdit()
        End If
    End Sub

    Private Sub btnLoad_Click(sender As Object, e As EventArgs) Handles btnLoad.Click

        Enabled = False

        If BindingManager IsNot Nothing Then
            BindingManager.EndCurrentEdit()
            BindingManager = Nothing
        End If

        SetSourceTable(Nothing)

        If DataAdapter IsNot Nothing Then
            DataAdapter.SelectCommand.Connection.Dispose()
            DataAdapter.Dispose()
            DataAdapter = Nothing
        End If

        Dim ConnectionString = tbDatasource.Text
        Dim CommandString = tbTable.Text

        ThreadPool.QueueUserWorkItem(
            Sub()

                Dim Connection As New Odbc.OdbcConnection
                Dim NewSourceTable As New DataTable
                Try
                    Connection.ConnectionString = ConnectionString
                    Connection.ConnectionTimeout = 20
                    Connection.Open()

                    Dim Command As New Odbc.OdbcCommand(CommandString, Connection) With {
                        .CommandTimeout = 45
                    }

                    DataAdapter = New Odbc.OdbcDataAdapter(Command)

                    With New Odbc.OdbcCommandBuilder(DataAdapter)
                    End With

                    DataAdapter.Fill(NewSourceTable)

                    If NewSourceTable.Columns.Count = 0 Then
                        NewSourceTable = Nothing
                        DataAdapter = Nothing
                        BindingManager = Nothing

                        Connection.Dispose()

                        Invoke(Sub()
                                   MessageBox.Show(Me, "Command executed successfully.", Text, MessageBoxButtons.OK, MessageBoxIcon.Information)
                                   Enabled = True
                               End Sub)

                        Exit Sub
                    End If

                Catch ex As Exception
                    NewSourceTable = Nothing
                    DataAdapter = Nothing
                    BindingManager = Nothing

                    Connection.Dispose()

                    Dim message = ex.GetBaseException().Message

                    Invoke(Sub()
                               MessageBox.Show(Me, message, "Error connecting to data source", MessageBoxButtons.OK, MessageBoxIcon.Exclamation)
                               Enabled = True
                           End Sub)

                    Exit Sub
                End Try

                With DataGridView
                    For Each DataGridViewColumn In .Columns
                        .AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.DisplayedCells
                    Next
                End With

                BindingManager = BindingContext(NewSourceTable)

                Invoke(Sub()
                           SetSourceTable(NewSourceTable)
                           Enabled = True
                       End Sub)

            End Sub)

    End Sub

    Private Sub TextBox_LostFocus(sender As Object, e As System.EventArgs) Handles tbDatasource.LostFocus, tbTable.LostFocus
        With DirectCast(sender, TextBox)
            .SelectionStart = 0
            .SelectionLength = .TextLength
        End With
    End Sub

    Protected Overrides Sub OnShown(e As EventArgs)
        MyBase.OnShown(e)

        tbTable.Select()
    End Sub

    Private Sub DataGridView_DataError(sender As Object, e As System.Windows.Forms.DataGridViewDataErrorEventArgs) Handles DataGridView.DataError
        If TypeOf e.Exception Is FormatException Then
            MessageBox.Show(Me, e.Exception.GetBaseException().Message, "Data format error", MessageBoxButtons.OK, MessageBoxIcon.Exclamation)
        End If
    End Sub

    Private Sub btnAbout_Click(sender As System.Object, e As System.EventArgs) Handles btnAbout.Click
        Using AboutBox As New AboutBox
            AboutBox.ShowDialog(Me)
        End Using
    End Sub

    Private Sub btnBrowseDataSources_Click(sender As Object, e As EventArgs) Handles btnBrowseDataSources.Click
        SQLManageDataSources(Handle)
    End Sub

    Private Sub btnDisconnect_Click(sender As Object, e As EventArgs) Handles btnDisconnect.Click
        Try
            DataGridView.ReadOnly = True

            If DataAdapter IsNot Nothing Then
                DataAdapter.SelectCommand.Connection.Dispose()
                DataAdapter = Nothing
                SourceTable.Dispose()
                SourceTable = Nothing
            End If

            btnDisconnect.Enabled = False

        Catch ex As Exception
            MessageBox.Show(Me, ex.GetBaseException().Message, "Error closing data source", MessageBoxButtons.OK, MessageBoxIcon.Exclamation)

        End Try
    End Sub
End Class
