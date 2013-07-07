Imports Microsoft.Win32
Imports System.IO

Class MainWindow
    Private potentialEdge As DraggableLine = Nothing
    Private pathSelection As DraggableLine = Nothing
    Private panningOrigin As Point = Nothing
    Private allNodes As New Dictionary(Of Integer, NodeThumb)
    Private allEdges As New List(Of DraggableLine)

    Public Sub New()
        InitializeComponent()
        ScaleFactor = 1.0
        allNodes.Add(NodeThumb1.NodeID, NodeThumb1)
        TranslateX = -5000
        TranslateY = -1000
    End Sub

    Private Sub NodeThumb1_EdgeCreated(ByVal sender As Object, ByVal e As NodeThumb.EdgeCreatedEventArgs) Handles NodeThumb1.EdgeCreated
        potentialEdge = e.NewEdge
    End Sub

    Private Sub NodeThumb1_PathSelectionStarted(ByVal sender As Object, ByVal e As NodeThumb.EdgeCreatedEventArgs) Handles NodeThumb1.PathSelectionStarted
        pathSelection = e.NewEdge
    End Sub

    Private Sub NodeThumb1_NodeCreated(ByVal sender As Object, ByVal e As NodeThumb.NodeCreatedEventArgs) Handles NodeThumb1.NodeCreated
        AddHandler e.NewNode.EdgeCreated, AddressOf NodeThumb1_EdgeCreated
        AddHandler e.NewNode.MouseUp, AddressOf NodeThumb1_MouseUp
        AddHandler e.NewNode.NodeCreated, AddressOf NodeThumb1_NodeCreated
        AddHandler e.NewNode.PathSelectionStarted, AddressOf NodeThumb1_PathSelectionStarted
        allNodes.Add(e.NewNode.NodeID, e.NewNode)
        allEdges.Add(e.OriginatingEdge)
    End Sub

    Private Sub NodeThumb1_MouseUp(ByVal sender As System.Object, ByVal e As System.Windows.Input.MouseButtonEventArgs) Handles NodeThumb1.MouseUp
        If Not potentialEdge Is Nothing Then
            Dim node As NodeThumb = sender
            If node.IsValidEndingEdge(potentialEdge) Then
                node.ConnectEndingEdge(potentialEdge)
                allEdges.Add(potentialEdge)
                potentialEdge = Nothing
                e.Handled = True
            End If
        End If

        If Not pathSelection Is Nothing Then
            Dim starting As NodeThumb = pathSelection.Source
            Dim ending As NodeThumb = sender
            Dim network As Network = BuildNetwork()
            Dim shortestPath = network.ShortestPathBetween(starting.NodeID, ending.NodeID)
            HighlightPath(shortestPath)
        End If
    End Sub

    Private Sub nodeCanvas_MouseMove(ByVal sender As Object, ByVal e As MouseEventArgs) Handles nodeCanvas.MouseMove
        If Not potentialEdge Is Nothing Then
            potentialEdge.Line.EndPoint = e.GetPosition(nodeCanvas)
            potentialEdge.Update()
        End If

        If Not pathSelection Is Nothing Then
            pathSelection.Line.EndPoint = e.GetPosition(nodeCanvas)
        End If


        Dim currentPosition = e.GetPosition(nodeCanvas)

        If IsPanning Then
            Dim pos = currentPosition - panningOrigin
            TranslateX += pos.X
            TranslateY += pos.Y
        End If
    End Sub

    Private Sub nodeCanvas_MouseUp(ByVal sender As Object, ByVal e As MouseButtonEventArgs) Handles nodeCanvas.MouseUp
        If Not potentialEdge Is Nothing Then
            potentialEdge.Delete()
            potentialEdge = Nothing
        End If

        If Not pathSelection Is Nothing Then
            pathSelection.Delete()
            pathSelection = Nothing
        End If

        StopPanning()
    End Sub

    Private Sub nodeCanvas_MouseWheel(ByVal sender As Object, ByVal e As MouseWheelEventArgs) Handles nodeCanvas.MouseWheel
        Dim mainWindow = Window.GetWindow(Me)
        Dim windowCenter = PointFromScreen(New Point(mainWindow.Left + mainWindow.ActualWidth / 2, mainWindow.Top + mainWindow.ActualHeight / 2))
        ScaleOriginX = windowCenter.X
        ScaleOriginY = windowCenter.Y
        Const minFactor = 0.1
        Const maxFactor = 2.0
        ScaleFactor = Math.Min(Math.Max(minFactor, ScaleFactor + e.Delta / 10000.0), maxFactor)
    End Sub

    Private Sub nodeCanvas_MouseDown(ByVal sender As Object, ByVal e As MouseButtonEventArgs) Handles nodeCanvas.MouseDown
        If e.MiddleButton And MouseButtonState.Pressed Then
            StartPanning(e.GetPosition(nodeCanvas))
        End If
    End Sub

    Private Sub nodeCanvas_MouseEnter(ByVal sender As Object, ByVal e As MouseEventArgs) Handles nodeCanvas.MouseEnter
        If Not e.MiddleButton And MouseButtonState.Pressed Then
            StopPanning()
        End If
    End Sub

    Private Function BuildNetwork() As Network
        Dim net = New Network

        For Each e In allEdges
            Dim sourceNodeId = CType(e.Source, NodeThumb).NodeID
            Dim targetNodeId = CType(e.Target, NodeThumb).NodeID
            net.DefineEdge(sourceNodeId, targetNodeId, e.Distance)
        Next

        Return net
    End Function

    Private Sub HighlightPath(ByVal shortestPath As List(Of Edge))
        allEdges.ForEach(Sub(e) e.Unhighlight())

        For Each e In shortestPath
            Dim starting = allNodes(e.FromNode.Id)
            Dim ending = allNodes(e.ToNode.Id)

            Dim edge = starting.GetEdgeConnectedTo(ending)
            edge.Highlight()
        Next
    End Sub


    Private ReadOnly Property IsPanning() As Boolean
        Get
            Return (panningOrigin <> Nothing)
        End Get
    End Property

    Private Sub StartPanning(ByVal origin As Point)
        panningOrigin = origin
    End Sub

    Private Sub StopPanning()
        panningOrigin = Nothing
    End Sub


    Public Property ScaleFactor As Decimal
        Get
            Return GetValue(ScaleFactorProperty)
        End Get

        Set(ByVal value As Decimal)
            SetValue(ScaleFactorProperty, value)
        End Set
    End Property

    Public Shared ReadOnly ScaleFactorProperty As DependencyProperty = _
                           DependencyProperty.Register("ScaleFactor", _
                           GetType(Decimal), GetType(MainWindow), _
                           New FrameworkPropertyMetadata(Nothing))



    Public Property TranslateX As Integer
        Get
            Return GetValue(TranslateXProperty)
        End Get

        Set(ByVal value As Integer)
            SetValue(TranslateXProperty, value)
        End Set
    End Property

    Public Shared ReadOnly TranslateXProperty As DependencyProperty = _
                           DependencyProperty.Register("TranslateX", _
                           GetType(Integer), GetType(MainWindow), _
                           New FrameworkPropertyMetadata(Nothing))




    Public Property TranslateY As Integer
        Get
            Return GetValue(TranslateYProperty)
        End Get

        Set(ByVal value As Integer)
            SetValue(TranslateYProperty, value)
        End Set
    End Property

    Public Shared ReadOnly TranslateYProperty As DependencyProperty = _
                           DependencyProperty.Register("TranslateY", _
                           GetType(Integer), GetType(MainWindow), _
                           New FrameworkPropertyMetadata(Nothing))



    Public Property ScaleOriginX As Integer
        Get
            Return GetValue(ScaleOriginXProperty)
        End Get

        Set(ByVal value As Integer)
            SetValue(ScaleOriginXProperty, value)
        End Set
    End Property

    Public Shared ReadOnly ScaleOriginXProperty As DependencyProperty = _
                           DependencyProperty.Register("ScaleOriginX", _
                           GetType(Integer), GetType(MainWindow), _
                           New FrameworkPropertyMetadata(Nothing))



    Public Property ScaleOriginY As Integer
        Get
            Return GetValue(ScaleOriginYProperty)
        End Get

        Set(ByVal value As Integer)
            SetValue(ScaleOriginYProperty, value)
        End Set
    End Property

    Public Shared ReadOnly ScaleOriginYProperty As DependencyProperty = _
                           DependencyProperty.Register("ScaleOriginY", _
                           GetType(Integer), GetType(MainWindow), _
                           New FrameworkPropertyMetadata(Nothing))

    Private Sub saveMenuItem_Click(ByVal sender As System.Object, ByVal e As System.Windows.RoutedEventArgs) Handles saveMenuItem.Click
        Dim dialog As New SaveFileDialog
        dialog.Filter = "Text Files (*.txt)|*.txt"
        If dialog.ShowDialog() Then
            SaveGraphToFile(dialog.FileName)
        End If
    End Sub

    Private Sub loadMenuItem_Click(ByVal sender As System.Object, ByVal e As System.Windows.RoutedEventArgs) Handles loadMenuItem.Click
        Dim dialog As New OpenFileDialog
        If dialog.ShowDialog() Then
            LoadGraphFromFile(dialog.FileName)
        End If
    End Sub

    Private Sub resetMenuItem_Click(ByVal sender As System.Object, ByVal e As System.Windows.RoutedEventArgs) Handles resetMenuItem.Click

    End Sub

    Private Sub SaveGraphToFile(ByVal filename As String)
        Using writer = New StreamWriter(filename, False)
            For Each e In allEdges
                Dim line = String.Join(",", New Object() {
                                        e.Source.NodeID,
                                        Canvas.GetLeft(e.Source),
                                        Canvas.GetTop(e.Source),
                                        e.Target.NodeID,
                                        Canvas.GetLeft(e.Target),
                                        Canvas.GetTop(e.Target)
                                       })
                writer.WriteLine(line)
            Next
        End Using
    End Sub

    Private Sub LoadGraphFromFile(ByVal filename As String)
    End Sub

End Class
