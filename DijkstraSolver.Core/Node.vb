Public Class Node
    Public Id As Integer
    Public Edges As List(Of Edge)
    Public Visited As Boolean
    Public Distance As Integer
    Public NextInRoute As Node = Nothing

    Function UnvisitedNeighbors() As List(Of Node)
        Return (From e In Edges
                Where e.AdjacentNode(Me).Visited = False
                Select e.AdjacentNode(Me)).ToList()
    End Function

    Shared Function ClosestOf(ByVal nodeList As List(Of Node)) As Node
        Dim minDistance = Integer.MaxValue
        Dim closetNode As Node = Nothing

        For Each node In nodeList
            If node.Distance <= minDistance Then
                minDistance = node.Distance
                closetNode = node
            End If
        Next

        Return closetNode
    End Function

    Function GetEdgeAdjacentTo(ByVal node As Node) As Edge
        Dim adjacentEdge = Me.Edges.First(Function(e) e.IsConnectedTo(node))
        Return New Edge With {
            .FromNode = Me,
            .ToNode = node,
            .Length = adjacentEdge.Length,
            .Id = adjacentEdge.Id
        }
    End Function

End Class
