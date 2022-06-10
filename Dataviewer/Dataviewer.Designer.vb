<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()>
Partial Class Dataviewer
    Inherits System.Windows.Forms.Form

    'Form overrides dispose to clean up the component list.
    <System.Diagnostics.DebuggerNonUserCode()>
    Protected Overrides Sub Dispose(disposing As Boolean)
        Try
            If disposing AndAlso components IsNot Nothing Then
                components.Dispose()
            End If
        Finally
            MyBase.Dispose(disposing)
        End Try
    End Sub

    'Required by the Windows Form Designer
    Private components As System.ComponentModel.IContainer

    'NOTE: The following procedure is required by the Windows Form Designer
    'It can be modified using the Windows Form Designer.  
    'Do not modify it using the code editor.
    <System.Diagnostics.DebuggerStepThrough()>
    Private Sub InitializeComponent()
        Dim resources As System.ComponentModel.ComponentResourceManager = New System.ComponentModel.ComponentResourceManager(GetType(Dataviewer))
        Me.Panel1 = New System.Windows.Forms.Panel()
        Me.btnDisconnect = New System.Windows.Forms.Button()
        Me.btnBrowseDataSources = New System.Windows.Forms.Button()
        Me.tbTable = New System.Windows.Forms.TextBox()
        Me.Label2 = New System.Windows.Forms.Label()
        Me.tbDatasource = New System.Windows.Forms.TextBox()
        Me.Label1 = New System.Windows.Forms.Label()
        Me.btnAbout = New System.Windows.Forms.Button()
        Me.btnLoad = New System.Windows.Forms.Button()
        Me.DataGridView = New System.Windows.Forms.DataGridView()
        Me.Panel1.SuspendLayout()
        CType(Me.DataGridView, System.ComponentModel.ISupportInitialize).BeginInit()
        Me.SuspendLayout()
        '
        'Panel1
        '
        Me.Panel1.Controls.Add(Me.btnDisconnect)
        Me.Panel1.Controls.Add(Me.btnBrowseDataSources)
        Me.Panel1.Controls.Add(Me.tbTable)
        Me.Panel1.Controls.Add(Me.Label2)
        Me.Panel1.Controls.Add(Me.tbDatasource)
        Me.Panel1.Controls.Add(Me.Label1)
        Me.Panel1.Controls.Add(Me.btnAbout)
        Me.Panel1.Controls.Add(Me.btnLoad)
        Me.Panel1.Dock = System.Windows.Forms.DockStyle.Top
        Me.Panel1.Location = New System.Drawing.Point(0, 0)
        Me.Panel1.Name = "Panel1"
        Me.Panel1.Size = New System.Drawing.Size(540, 56)
        Me.Panel1.TabIndex = 6
        '
        'btnDisconnect
        '
        Me.btnDisconnect.Anchor = CType((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.btnDisconnect.Enabled = False
        Me.btnDisconnect.Location = New System.Drawing.Point(456, 30)
        Me.btnDisconnect.Name = "btnDisconnect"
        Me.btnDisconnect.Size = New System.Drawing.Size(80, 20)
        Me.btnDisconnect.TabIndex = 5
        Me.btnDisconnect.Text = "Disconnect"
        Me.btnDisconnect.UseVisualStyleBackColor = True
        '
        'btnBrowseDataSources
        '
        Me.btnBrowseDataSources.Anchor = CType((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.btnBrowseDataSources.Location = New System.Drawing.Point(370, 4)
        Me.btnBrowseDataSources.Name = "btnBrowseDataSources"
        Me.btnBrowseDataSources.Size = New System.Drawing.Size(80, 20)
        Me.btnBrowseDataSources.TabIndex = 2
        Me.btnBrowseDataSources.Text = "Data sources"
        Me.btnBrowseDataSources.UseVisualStyleBackColor = True
        '
        'tbTable
        '
        Me.tbTable.Anchor = CType(((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Left) _
            Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.tbTable.DataBindings.Add(New System.Windows.Forms.Binding("Text", Global.LTR.DataViewer.My_Project.MySettings.Default, "Table", True, System.Windows.Forms.DataSourceUpdateMode.OnPropertyChanged))
        Me.tbTable.Location = New System.Drawing.Point(74, 30)
        Me.tbTable.Name = "tbTable"
        Me.tbTable.Size = New System.Drawing.Size(290, 20)
        Me.tbTable.TabIndex = 1
        Me.tbTable.Text = Global.LTR.DataViewer.My_Project.MySettings.Default.Table
        '
        'Label2
        '
        Me.Label2.AutoSize = True
        Me.Label2.Location = New System.Drawing.Point(3, 33)
        Me.Label2.Name = "Label2"
        Me.Label2.Size = New System.Drawing.Size(57, 13)
        Me.Label2.TabIndex = 14
        Me.Label2.Text = "SQL query"
        '
        'tbDatasource
        '
        Me.tbDatasource.Anchor = CType(((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Left) _
            Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.tbDatasource.DataBindings.Add(New System.Windows.Forms.Binding("Text", Global.LTR.DataViewer.My_Project.MySettings.Default, "Datasource", True, System.Windows.Forms.DataSourceUpdateMode.OnPropertyChanged))
        Me.tbDatasource.Location = New System.Drawing.Point(74, 4)
        Me.tbDatasource.Name = "tbDatasource"
        Me.tbDatasource.Size = New System.Drawing.Size(290, 20)
        Me.tbDatasource.TabIndex = 0
        Me.tbDatasource.Text = Global.LTR.DataViewer.My_Project.MySettings.Default.Datasource
        '
        'Label1
        '
        Me.Label1.AutoSize = True
        Me.Label1.Location = New System.Drawing.Point(3, 7)
        Me.Label1.Name = "Label1"
        Me.Label1.Size = New System.Drawing.Size(61, 13)
        Me.Label1.TabIndex = 13
        Me.Label1.Text = "Connection"
        '
        'btnAbout
        '
        Me.btnAbout.Anchor = CType((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.btnAbout.Location = New System.Drawing.Point(456, 4)
        Me.btnAbout.Name = "btnAbout"
        Me.btnAbout.Size = New System.Drawing.Size(81, 20)
        Me.btnAbout.TabIndex = 4
        Me.btnAbout.Text = "About..."
        Me.btnAbout.UseVisualStyleBackColor = True
        '
        'btnLoad
        '
        Me.btnLoad.Anchor = CType((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.btnLoad.Location = New System.Drawing.Point(369, 30)
        Me.btnLoad.Name = "btnLoad"
        Me.btnLoad.Size = New System.Drawing.Size(81, 20)
        Me.btnLoad.TabIndex = 3
        Me.btnLoad.Text = "Execute"
        Me.btnLoad.UseVisualStyleBackColor = True
        '
        'DataGridView
        '
        Me.DataGridView.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize
        Me.DataGridView.Dock = System.Windows.Forms.DockStyle.Fill
        Me.DataGridView.Location = New System.Drawing.Point(0, 56)
        Me.DataGridView.Name = "DataGridView"
        Me.DataGridView.Size = New System.Drawing.Size(540, 548)
        Me.DataGridView.TabIndex = 0
        '
        'Dataviewer
        '
        Me.AcceptButton = Me.btnLoad
        Me.AutoScaleDimensions = New System.Drawing.SizeF(6.0!, 13.0!)
        Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
        Me.ClientSize = Global.LTR.DataViewer.My_Project.MySettings.Default.Size
        Me.Controls.Add(Me.DataGridView)
        Me.Controls.Add(Me.Panel1)
        Me.DataBindings.Add(New System.Windows.Forms.Binding("Location", Global.LTR.DataViewer.My_Project.MySettings.Default, "WindowLocation", True, System.Windows.Forms.DataSourceUpdateMode.OnPropertyChanged))
        Me.DataBindings.Add(New System.Windows.Forms.Binding("ClientSize", Global.LTR.DataViewer.My_Project.MySettings.Default, "Size", True, System.Windows.Forms.DataSourceUpdateMode.OnPropertyChanged))
        Me.Icon = CType(resources.GetObject("$this.Icon"), System.Drawing.Icon)
        Me.Location = Global.LTR.DataViewer.My_Project.MySettings.Default.WindowLocation
        Me.Name = "Dataviewer"
        Me.Text = "ODBC Dataviewer"
        Me.Panel1.ResumeLayout(False)
        Me.Panel1.PerformLayout()
        CType(Me.DataGridView, System.ComponentModel.ISupportInitialize).EndInit()
        Me.ResumeLayout(False)

    End Sub
    Friend WithEvents Panel1 As System.Windows.Forms.Panel
    Friend WithEvents tbTable As System.Windows.Forms.TextBox
    Friend WithEvents Label2 As System.Windows.Forms.Label
    Friend WithEvents tbDatasource As System.Windows.Forms.TextBox
    Friend WithEvents Label1 As System.Windows.Forms.Label
    Friend WithEvents btnLoad As System.Windows.Forms.Button
    Friend WithEvents DataGridView As System.Windows.Forms.DataGridView
    Friend WithEvents btnAbout As System.Windows.Forms.Button
    Friend WithEvents btnBrowseDataSources As System.Windows.Forms.Button
    Friend WithEvents btnDisconnect As System.Windows.Forms.Button

End Class
