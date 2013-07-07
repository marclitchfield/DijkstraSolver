Public Class Edge
    Public Id As Integer
    Public FromNode As Node
    Public ToNode As Node
    Public Length As Integer

    Public Function IsConnectedTo(ByRef theNode As Node) As Boolean
        Return theNode.Id = FromNode.Id OrElse theNode.Id = ToNode.Id
    End Function

    Function AdjacentNode(ByVal node As Node) As Node
        If node.Id = FromNode.Id Then Return ToNode
        If node.Id = ToNode.Id Then Return FromNode

        Throw New Exception(String.Format("Node {0} not adjacent to edge {1}", node.Id, Me.Id))
    End Function
End Class
