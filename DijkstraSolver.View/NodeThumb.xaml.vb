Imports System.Windows.Controls.Primitives
Imports System.Threading

Public Class NodeThumb
    Private Const BaseNodeSize As Integer = 15

    Public Class EdgeCreatedEventArgs
        Inherits EventArgs
        Public NewEdge As DraggableLine
    End Class

    Public Class NodeCreatedEventArgs
        Inherits EventArgs
        Public NewNode As NodeThumb
        Public OriginatingEdge As DraggableLine
    End Class

    Public Delegate Sub EdgeCreatedEventHandler(ByVal sender As Object, ByVal e As EdgeCreatedEventArgs)
    Public Delegate Sub NodeCreatedEventHandler(ByVal sender As Object, ByVal e As NodeCreatedEventArgs)

    Public Event EdgeCreated As EdgeCreatedEventHandler
    Public Event NodeCreated As NodeCreatedEventHandler
    Public Event PathSelectionStarted As EdgeCreatedEventHandler

    Private random As New Random
    Private startEdges As New List(Of DraggableLine)
    Private endEdges As New List(Of DraggableLine)
    Private Shared nextNodeId As Integer = 0

    Public Sub New()
        InitializeComponent()
        NodeID = Interlocked.Increment(nextNodeId)
        Canvas.SetZIndex(Me, 2)
    End Sub


    Private Sub nodeThumb_DragDelta(ByVal sender As Object, ByVal e As DragDeltaEventArgs) Handles nodeThumb.DragDelta
        Canvas.SetLeft(Me, Canvas.GetLeft(Me) + e.HorizontalChange)
        Canvas.SetTop(Me, Canvas.GetTop(Me) + e.VerticalChange)
        UpdateEdges(Me)
    End Sub

    Private Sub nodeThumb_PreviewMouseDown(ByVal sender As Object, ByVal e As MouseButtonEventArgs) Handles nodeThumb.PreviewMouseDown
        If (e.RightButton And MouseButtonState.Pressed) OrElse _
            ((e.LeftButton And MouseButtonState.Pressed) AndAlso (Keyboard.IsKeyDown(Key.LeftAlt) OrElse Keyboard.IsKeyDown(Key.RightAlt))) Then

            Dim newEdge As DraggableLine = CreateNewEdge(Me)
            newEdge.Line.EndPoint = e.GetPosition(Me.Parent)
            newEdge.Update()

            RaiseEvent EdgeCreated(Me, New EdgeCreatedEventArgs With {.NewEdge = newEdge})
            e.Handled = True
        End If
    End Sub

    Private Sub nodeThumb_PreviewMouseLeftButtonDown(ByVal sender As System.Object, ByVal e As System.Windows.Input.MouseButtonEventArgs) Handles MyBase.PreviewMouseLeftButtonDown
        If Keyboard.IsKeyDown(Key.LeftShift) OrElse Keyboard.IsKeyDown(Key.RightShift) Then
            Dim pathSelectionEdge As New DraggableLine(CType(Me.Parent, Canvas))
            pathSelectionEdge.LineColor = Colors.GreenYellow
            pathSelectionEdge.ShowLabel = False
            pathSelectionEdge.ConnectStartTo(sender)
            pathSelectionEdge.Line.EndPoint = e.GetPosition(Me.Parent)
            pathSelectionEdge.Update()
            RaiseEvent PathSelectionStarted(Me, New EdgeCreatedEventArgs With {.NewEdge = pathSelectionEdge})
            e.Handled = True
        End If

        If Keyboard.IsKeyDown(Key.LeftCtrl) OrElse Keyboard.IsKeyDown(Key.RightCtrl) Then
            CreateNewNode()
            e.Handled = True
        End If
    End Sub

    Private Sub nodeThumb_MouseDoubleClick(ByVal sender As System.Object, ByVal e As MouseButtonEventArgs) Handles MyBase.MouseDoubleClick
        If (e.LeftButton And MouseButtonState.Pressed) Then
            CreateNewNode()
        End If
    End Sub

    Public Sub CreateNewNode()
        Const DisplacementDistance As Integer = 75

        Dim newNode As New NodeThumb
        Dim randomAngle As Double = random.NextDouble() * (Math.PI / 2) - Math.PI / 8
        Canvas.SetLeft(newNode, Canvas.GetLeft(Me) + DisplacementDistance * (1 + random.NextDouble()))
        Canvas.SetTop(newNode, Canvas.GetTop(Me) + Math.Sin(randomAngle) * DisplacementDistance)
        Canvas.SetZIndex(newNode, 2)

        AddUIElement(newNode)
        UpdateLayout()

        Dim edge As DraggableLine = CreateNewEdge(Me)
        newNode.endEdges.Add(edge)
        edge.ConnectEndTo(newNode)

        RaiseEvent NodeCreated(Me, New NodeCreatedEventArgs With {.NewNode = newNode, .OriginatingEdge = edge})
    End Sub

    Private Function CreateNewEdge(ByRef startNode As NodeThumb)
        Dim edge As New DraggableLine(CType(Me.Parent, Canvas))
        startNode.startEdges.Add(edge)
        edge.ConnectStartTo(startNode)
        Return edge
    End Function

    Private Sub AddUIElement(ByRef element As UIElement)
        CType(Me.Parent, Canvas).Children.Add(element)
    End Sub

    Private Sub UpdateEdges(ByVal control As NodeThumb)
        RemoveDeletedEdges(control)

        For Each edge In control.startEdges
            edge.ConnectStartTo(control)
            edge.Update()
        Next

        For Each edge In control.endEdges
            edge.ConnectEndTo(control)
            edge.Update()
        Next
    End Sub

    Private Sub RemoveDeletedEdges(ByRef control As NodeThumb)
        Dim whereDeleted = Function(edge As DraggableLine)
                               Return edge.IsDeleted
                           End Function

        control.startEdges.RemoveAll(whereDeleted)
        control.endEdges.RemoveAll(whereDeleted)
    End Sub

    Sub ConnectEndingEdge(ByVal edge As DraggableLine)
        endEdges.Add(edge)
        edge.ConnectEndTo(Me)
    End Sub

    Function IsValidEndingEdge(ByVal potentialEdge As DraggableLine) As Boolean
        If endEdges.Contains(potentialEdge) Then Return False
        If startEdges.Contains(potentialEdge) Then Return False
        Return True
    End Function

    Function GetEdgeConnectedTo(ByVal targetNode As NodeThumb) As DraggableLine
        For Each e In startEdges
            If e.IsConnectedTo(targetNode) Then Return e
        Next

        For Each e In endEdges
            If e.IsConnectedTo(targetNode) Then Return e
        Next

        Throw New Exception("Edge not connected to node")
    End Function

    Public Property NodeID As Integer
        Get
            Return GetValue(NodeIDProperty)
        End Get

        Set(ByVal value As Integer)
            SetValue(NodeIDProperty, value)
        End Set
    End Property

    Public Shared ReadOnly NodeIDProperty As DependencyProperty = _
                           DependencyProperty.Register("NodeID", _
                           GetType(Integer), GetType(NodeThumb), _
                           New FrameworkPropertyMetadata(Nothing))
End Class
