Public Class Network
    Private nodeDictionary As New Dictionary(Of Integer, Node)
    Private edgeList As New List(Of Edge)
    Private nextAvailableEdgeId As Integer = 1

    Public Function DefineEdge(ByVal fromNodeId As Integer, ByVal toNodeId As Integer, ByVal length As Integer) As Edge
        Dim fromNode = MakeNode(fromNodeId)
        Dim toNode = MakeNode(toNodeId)

        Dim edge As New Edge With {
            .Id = NextEdgeId(),
            .Length = length,
            .FromNode = fromNode,
            .ToNode = toNode
        }

        fromNode.Edges.Add(edge)
        toNode.Edges.Add(edge)
        Me.edgeList.Add(edge)

        Return edge
    End Function

    Private Function MakeNode(ByVal id As Integer) As Node
        Dim node As Node
        If nodeDictionary.ContainsKey(id) Then
            node = nodeDictionary(id)
        Else
            node = New Node With {.Id = id, .Edges = New List(Of Edge)}
            nodeDictionary.Add(id, node)
        End If

        Return node
    End Function

    Public Function ShortestPathBetween(ByVal node1 As Integer, ByVal node2 As Integer) As List(Of Edge)
        Dim path As New List(Of Edge)
        Dim startNode As Node = nodeDictionary(node1)
        Dim endNode As Node = nodeDictionary(node2)
        Dim unvisitedList As List(Of Node) = nodeDictionary.Values.ToList()

        For Each node In nodeDictionary.Values
            node.Distance = Integer.MaxValue
            node.Visited = False
        Next

        startNode.Distance = 0
        Dim currentNode = startNode

        Do
            ComputeDistancesForUnvisitedNeighbors(currentNode)
            unvisitedList.Remove(currentNode)
            currentNode.Visited = True
            currentNode = Node.ClosestOf(unvisitedList)
        Loop While unvisitedList.Count > 0

        Return BuildShortestPathList(startNode, endNode)
    End Function

    Private Sub ComputeDistancesForUnvisitedNeighbors(ByVal currentNode As Node)
        For Each unvisitedNeighbor In currentNode.UnvisitedNeighbors
            Dim tenativeDistance As Integer

            Dim edgeToNeighbor = currentNode.GetEdgeAdjacentTo(unvisitedNeighbor)
            tenativeDistance = currentNode.Distance + edgeToNeighbor.Length

            If tenativeDistance < unvisitedNeighbor.Distance Then
                unvisitedNeighbor.Distance = tenativeDistance
                unvisitedNeighbor.NextInRoute = currentNode
            End If
        Next
    End Sub

    Private Function BuildShortestPathList(ByVal startNode As Node, ByVal endNode As Node) As List(Of Edge)
        Dim shortestPath As New List(Of Edge)

        Dim currentNode = endNode
        Do While currentNode.Id <> startNode.Id
            If currentNode.NextInRoute Is Nothing Then
                Throw New Exception(String.Format("No route could be found between nodes {0} and {1}", startNode.Id, endNode.Id))
            End If

            shortestPath.Insert(0, currentNode.NextInRoute.GetEdgeAdjacentTo(currentNode))
            currentNode = currentNode.NextInRoute
        Loop

        Return shortestPath
    End Function

    Private Function NextEdgeId() As Integer
        NextEdgeId = nextAvailableEdgeId
        nextAvailableEdgeId += 1
    End Function

    Public ReadOnly Property Nodes() As IEnumerable(Of Node)
        Get
            Return nodeDictionary.Values
        End Get
    End Property

    Public ReadOnly Property Edges() As IEnumerable(Of Edge)
        Get
            Return edgeList
        End Get
    End Property
End Class
