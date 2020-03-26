<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()> _
Partial Class MainForm
    Inherits System.Windows.Forms.Form

    'Form overrides dispose to clean up the component list.
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

    'Required by the Windows Form Designer
    Private components As System.ComponentModel.IContainer

    'NOTE: The following procedure is required by the Windows Form Designer
    'It can be modified using the Windows Form Designer.  
    'Do not modify it using the code editor.
    <System.Diagnostics.DebuggerStepThrough()> _
    Private Sub InitializeComponent()
        Me.components = New System.ComponentModel.Container()
        Dim resources As System.ComponentModel.ComponentResourceManager = New System.ComponentModel.ComponentResourceManager(GetType(MainForm))
        Me.CodeTextBox = New FastColoredTextBoxNS.FastColoredTextBox()
        Me.DecompileButton = New System.Windows.Forms.Button()
        Me.PathTextBox = New System.Windows.Forms.TextBox()
        Me.BrowseLink = New System.Windows.Forms.LinkLabel()
        Me.Label1 = New System.Windows.Forms.Label()
        CType(Me.CodeTextBox, System.ComponentModel.ISupportInitialize).BeginInit()
        Me.SuspendLayout()
        '
        'CodeTextBox
        '
        Me.CodeTextBox.Anchor = CType((((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Bottom) _
            Or System.Windows.Forms.AnchorStyles.Left) _
            Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.CodeTextBox.AutoCompleteBracketsList = New Char() {Global.Microsoft.VisualBasic.ChrW(40), Global.Microsoft.VisualBasic.ChrW(41), Global.Microsoft.VisualBasic.ChrW(123), Global.Microsoft.VisualBasic.ChrW(125), Global.Microsoft.VisualBasic.ChrW(91), Global.Microsoft.VisualBasic.ChrW(93), Global.Microsoft.VisualBasic.ChrW(34), Global.Microsoft.VisualBasic.ChrW(34), Global.Microsoft.VisualBasic.ChrW(39), Global.Microsoft.VisualBasic.ChrW(39)}
        Me.CodeTextBox.AutoScrollMinSize = New System.Drawing.Size(27, 14)
        Me.CodeTextBox.BackBrush = Nothing
        Me.CodeTextBox.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle
        Me.CodeTextBox.CharHeight = 14
        Me.CodeTextBox.CharWidth = 8
        Me.CodeTextBox.Cursor = System.Windows.Forms.Cursors.IBeam
        Me.CodeTextBox.DisabledColor = System.Drawing.Color.FromArgb(CType(CType(100, Byte), Integer), CType(CType(180, Byte), Integer), CType(CType(180, Byte), Integer), CType(CType(180, Byte), Integer))
        Me.CodeTextBox.Font = New System.Drawing.Font("Courier New", 9.75!)
        Me.CodeTextBox.IsReplaceMode = False
        Me.CodeTextBox.Location = New System.Drawing.Point(12, 12)
        Me.CodeTextBox.Name = "CodeTextBox"
        Me.CodeTextBox.Paddings = New System.Windows.Forms.Padding(0)
        Me.CodeTextBox.ReadOnly = True
        Me.CodeTextBox.SelectionColor = System.Drawing.Color.FromArgb(CType(CType(60, Byte), Integer), CType(CType(0, Byte), Integer), CType(CType(0, Byte), Integer), CType(CType(255, Byte), Integer))
        Me.CodeTextBox.ServiceColors = CType(resources.GetObject("CodeTextBox.ServiceColors"), FastColoredTextBoxNS.ServiceColors)
        Me.CodeTextBox.Size = New System.Drawing.Size(663, 366)
        Me.CodeTextBox.TabIndex = 0
        Me.CodeTextBox.Zoom = 100
        '
        'DecompileButton
        '
        Me.DecompileButton.Anchor = CType((System.Windows.Forms.AnchorStyles.Bottom Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.DecompileButton.Location = New System.Drawing.Point(573, 384)
        Me.DecompileButton.Name = "DecompileButton"
        Me.DecompileButton.Size = New System.Drawing.Size(102, 23)
        Me.DecompileButton.TabIndex = 1
        Me.DecompileButton.Text = "Decompile"
        Me.DecompileButton.UseVisualStyleBackColor = True
        '
        'PathTextBox
        '
        Me.PathTextBox.Anchor = CType((System.Windows.Forms.AnchorStyles.Bottom Or System.Windows.Forms.AnchorStyles.Left), System.Windows.Forms.AnchorStyles)
        Me.PathTextBox.Location = New System.Drawing.Point(137, 386)
        Me.PathTextBox.Name = "PathTextBox"
        Me.PathTextBox.Size = New System.Drawing.Size(175, 21)
        Me.PathTextBox.TabIndex = 2
        '
        'BrowseLink
        '
        Me.BrowseLink.Anchor = CType((System.Windows.Forms.AnchorStyles.Bottom Or System.Windows.Forms.AnchorStyles.Left), System.Windows.Forms.AnchorStyles)
        Me.BrowseLink.AutoSize = True
        Me.BrowseLink.Location = New System.Drawing.Point(318, 389)
        Me.BrowseLink.Name = "BrowseLink"
        Me.BrowseLink.Size = New System.Drawing.Size(49, 13)
        Me.BrowseLink.TabIndex = 4
        Me.BrowseLink.TabStop = True
        Me.BrowseLink.Text = "Browse"
        '
        'Label1
        '
        Me.Label1.Anchor = CType((System.Windows.Forms.AnchorStyles.Bottom Or System.Windows.Forms.AnchorStyles.Left), System.Windows.Forms.AnchorStyles)
        Me.Label1.AutoSize = True
        Me.Label1.Location = New System.Drawing.Point(9, 389)
        Me.Label1.Name = "Label1"
        Me.Label1.Size = New System.Drawing.Size(119, 13)
        Me.Label1.TabIndex = 5
        Me.Label1.Text = "Path to Source File:"
        '
        'MainForm
        '
        Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.None
        Me.BackColor = System.Drawing.Color.White
        Me.ClientSize = New System.Drawing.Size(687, 419)
        Me.Controls.Add(Me.Label1)
        Me.Controls.Add(Me.BrowseLink)
        Me.Controls.Add(Me.PathTextBox)
        Me.Controls.Add(Me.DecompileButton)
        Me.Controls.Add(Me.CodeTextBox)
        Me.Font = New System.Drawing.Font("Verdana", 8.25!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.Name = "MainForm"
        Me.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen
        Me.Text = "Omega Client Decompiler"
        CType(Me.CodeTextBox, System.ComponentModel.ISupportInitialize).EndInit()
        Me.ResumeLayout(False)
        Me.PerformLayout()

    End Sub

    Friend WithEvents CodeTextBox As FastColoredTextBoxNS.FastColoredTextBox
    Friend WithEvents DecompileButton As Button
    Friend WithEvents BrowseLink As LinkLabel
    Friend WithEvents Label1 As Label
    Friend WithEvents PathTextBox As TextBox
End Class
