Imports System.Text
Imports DijkstraSolver

<TestClass()>
Public Class NetworkTests

    <TestMethod()>
    Public Sub DefiningEdgesCreatesNetworkObjects()
        Dim network As New Network
        Dim edge = network.DefineEdge(1, 2, 9)

        ' Edges
        Assert.AreEqual(1, network.Edges.Count())
        Assert.AreEqual(1, network.Edges.First().Id)
        Assert.AreEqual(9, network.Edges.First().Length)
        Assert.AreEqual(1, network.Edges.First().FromNode.Id)
        Assert.AreEqual(2, network.Edges.First().ToNode.Id)
        Assert.AreEqual(1, network.Edges.First().FromNode.Edges.Count)
        Assert.AreEqual(1, network.Edges.First().ToNode.Edges.Count)
        Assert.AreEqual(edge.Id, network.Edges.First().FromNode.Edges.First().Id)
        Assert.AreEqual(edge.Id, network.Edges.First().ToNode.Edges.First().Id)

        ' Nodes
        Assert.AreEqual(2, network.Nodes.Count())
        Assert.AreEqual(network.Nodes.First().Id, network.Edges.First().FromNode.Id)
        Assert.AreEqual(network.Nodes.Last().Id, network.Edges.First().ToNode.Id)
    End Sub

    <TestMethod()>
    Public Sub OnlyPathBetweenTwoNodes()
        Dim network As New Network
        network.DefineEdge(1, 2, 4)

        Dim path = network.ShortestPathBetween(1, 2)
        AssertPath(path, 1, 2)
    End Sub

    <TestMethod()>
    Public Sub OnlyPathBetweenTwoNodes_Reversed()
        Dim network As New Network
        network.DefineEdge(1, 2, 5)

        Dim path = network.ShortestPathBetween(2, 1)
        AssertPath(path, 2, 1)
    End Sub

    <TestMethod()>
    Public Sub SelectShortestPathFromTwoPossibleRoutes_3Nodes()
        Dim network As New Network
        network.DefineEdge(1, 2, 20)
        network.DefineEdge(2, 3, 10)
        network.DefineEdge(1, 3, 25) '1,3 is 5 shorter than 1,2,3

        Dim path = network.ShortestPathBetween(1, 3)
        AssertPath(path, 1, 3)
    End Sub

    <TestMethod()>
    Public Sub ReverseTransitiveClosure()
        Dim network As New Network
        network.DefineEdge(1, 2, 10)
        network.DefineEdge(2, 3, 20)

        Dim path = network.ShortestPathBetween(2, 1)
        AssertPath(path, 2, 1)
    End Sub

    <TestMethod()>
    Public Sub SelectShortestPathFromTwoPossibleRoutes_4Nodes()
        Dim network As New Network
        network.DefineEdge(1, 2, 10)
        network.DefineEdge(1, 3, 10)
        network.DefineEdge(2, 4, 90)
        network.DefineEdge(3, 4, 80) '1,3,4 is 10 shorter than 1,2,4

        Dim path = network.ShortestPathBetween(1, 4)
        AssertPath(path, 1, 3, 4)
    End Sub

    <TestMethod()>
    Public Sub SelectShortestPathFromTwoPossibleRoutes_4Nodes_DefinedInReverse()
        Dim network As New Network
        network.DefineEdge(2, 1, 10)
        network.DefineEdge(3, 1, 10)
        network.DefineEdge(4, 2, 90)
        network.DefineEdge(4, 3, 80)

        Dim path = network.ShortestPathBetween(1, 4)
        AssertPath(path, 1, 3, 4)
    End Sub

    <TestMethod()>
    Public Sub LabAssignment()
        Dim network As New Network
        network.DefineEdge(1, 2, 5)
        network.DefineEdge(2, 3, 8)
        network.DefineEdge(3, 5, 6)
        network.DefineEdge(2, 5, 9)
        network.DefineEdge(1, 4, 6)
        network.DefineEdge(4, 6, 4)
        network.DefineEdge(2, 6, 8)
        network.DefineEdge(4, 7, 9)
        network.DefineEdge(6, 7, 6)
        network.DefineEdge(6, 8, 14)
        network.DefineEdge(3, 8, 15)
        network.DefineEdge(7, 9, 11)
        network.DefineEdge(8, 9, 9)
        network.DefineEdge(9, 10, 6)
        network.DefineEdge(9, 11, 5)
        network.DefineEdge(9, 12, 12)

        ' From route table
        AssertPath(network.ShortestPathBetween(1, 12), 1, 4, 7, 9, 12)
    End Sub

    Private Sub AssertPath(ByRef path As List(Of Edge), ByVal ParamArray ids As Integer())
        Assert.AreEqual(ids.Length - 1, path.Count)

        For i = 0 To ids.Length - 2
            Assert.AreEqual(ids(i), path(i).FromNode.Id)
            Assert.AreEqual(ids(i + 1), path(i).ToNode.Id)
        Next
    End Sub
End Class
