Public Class DraggableLine
    Public Source As NodeThumb
    Public Target As NodeThumb
    Public Line As LineGeometry = Nothing
    Public Label As EdgeLabel

    Private path As Path
    Private parent As Canvas
    Private deleted As Boolean = False

    Private lineColorValue As Color = Colors.RoyalBlue
    Public Property LineColor() As Color
        Get
            Return lineColorValue
        End Get
        Set(ByVal value As Color)
            lineColorValue = value
        End Set
    End Property

    Private showLabelValue As Boolean = True
    Public Property ShowLabel() As Boolean
        Get
            Return showLabelValue
        End Get
        Set(ByVal value As Boolean)
            showLabelValue = value
        End Set
    End Property

    Private distanceValue As Integer
    Public Property Distance() As Integer
        Get
            Return distanceValue
        End Get
        Set(ByVal value As Integer)
            distanceValue = value
        End Set
    End Property

    Public Sub New(ByRef parentCanvas As Canvas)
        parent = parentCanvas
    End Sub

    Private Sub InitializeUIElements()
        If Line Is Nothing Then
            Line = New LineGeometry
            path = CreatePath(Line)
        End If

        If Label Is Nothing AndAlso ShowLabel Then
            Label = New EdgeLabel
            parent.Children.Add(Label)
            Canvas.SetZIndex(Label, 1)
        End If
    End Sub

    Public Sub Update()
        InitializeUIElements()
        Dim dx As Double = (Line.StartPoint.X - Line.EndPoint.X)
        Dim dy As Double = (Line.StartPoint.Y - Line.EndPoint.Y)
        distanceValue = Math.Sqrt(dy ^ 2 + dx ^ 2)

        If ShowLabel Then
            Label.UpdateLayout()
            Canvas.SetLeft(Label, (Line.StartPoint.X + Line.EndPoint.X) / 2 - Label.ActualWidth / 2)
            Canvas.SetTop(Label, (Line.StartPoint.Y + Line.EndPoint.Y) / 2 - Label.ActualHeight / 2)
            Label.Distance = distanceValue
        End If
    End Sub

    Public Sub ConnectStartTo(ByRef source As Control)
        Me.Source = source
        InitializeUIElements()
        source.ApplyTemplate()
        Line.StartPoint = CenterPoint(source)
    End Sub

    Public Sub ConnectEndTo(ByRef target As Control)
        Me.Target = target
        InitializeUIElements()
        Line.EndPoint = CenterPoint(target)
        Update()
    End Sub

    Public Function IsConnectedTo(ByVal control As Control) As Boolean
        Return (control.Equals(Source) OrElse control.Equals(Target))
    End Function

    Private Function CenterPoint(ByRef target As Control) As Point
        Dim left As Double = Canvas.GetLeft(target)
        Dim top As Double = Canvas.GetTop(target)
        Return New Point(left + target.ActualWidth / 2, top + target.ActualHeight / 2)
    End Function

    Sub Delete()
        If Not deleted Then
            If ShowLabel Then
                parent.Children.Remove(Label)
            End If

            parent.Children.Remove(path)
            deleted = True
        End If
    End Sub

    Public ReadOnly Property IsDeleted() As Boolean
        Get
            Return deleted
        End Get
    End Property

    Private Function CreatePath(ByRef line As LineGeometry) As Path
        Dim path As New Path
        path.Stroke = New SolidColorBrush(LineColor)
        path.StrokeThickness = 1
        path.Data = line
        parent.Children.Add(path)
        Return path
    End Function

    Sub Highlight()
        path.Stroke = New SolidColorBrush(Colors.DodgerBlue)
        path.StrokeThickness = 5
        path.UpdateLayout()
    End Sub

    Sub Unhighlight()
        path.Stroke = New SolidColorBrush(LineColor)
        path.StrokeThickness = 1
        path.UpdateLayout()
    End Sub
End Class
