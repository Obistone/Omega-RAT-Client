Imports System.IO
Imports System.Text

'I'm sorry if this is sloppy code, I spent a maximum of 5 minutes creating this application.
Public Class MainForm

    Public Shared Instance As MainForm

    Public Sub New()
        InitializeComponent()

        Instance = Me

    End Sub

    Private Sub BrowseLink_LinkClicked(sender As Object, e As LinkLabelLinkClickedEventArgs) Handles BrowseLink.LinkClicked
        Using dialog As OpenFileDialog = New OpenFileDialog
            dialog.Filter = "Source Files|*.*"
            dialog.Title = "Select Source File"

            If dialog.ShowDialog() = DialogResult.OK Then
                PathTextBox.Text = dialog.FileName
            End If
        End Using
    End Sub

    Private Async Sub DecompileButton_Click(sender As Object, e As EventArgs) Handles DecompileButton.Click
        Await Task.Run(Sub()
                           If Not String.IsNullOrWhiteSpace(PathTextBox.Text) Then
                               If File.Exists(PathTextBox.Text) Then
                                   MainForm.Instance.Invoke(Sub() 'Note: this entire sequence is taken directly from dnSpy. 
                                                                Dim originalSource As String = Encoding.UTF8.GetString(Convert.FromBase64String(File.ReadAllText(PathTextBox.Text)))
                                                                Dim sourceSplit() As String = originalSource.Split(" ")
                                                                Dim sourceSplitBytes As Byte() = New Byte(sourceSplit.Length - 1 - 1) {}

                                                                For i As Integer = 0 To sourceSplit.Length - 1 - 1
                                                                    sourceSplitBytes(i) = Convert.ToByte(Integer.Parse(sourceSplit(i)) / 69)
                                                                Next

                                                                CodeTextBox.Text = Encoding.UTF8.GetString(sourceSplitBytes)
                                                            End Sub)
                               End If
                           End If
                       End Sub)
    End Sub
End Class