Public Class EdgeLabel
    Public Property Distance As Integer
        Get
            Return GetValue(DistanceProperty)
        End Get

        Set(ByVal value As Integer)
            SetValue(DistanceProperty, value)
        End Set
    End Property

    Public Shared ReadOnly DistanceProperty As DependencyProperty = _
                           DependencyProperty.Register("Distance", _
                           GetType(Integer), GetType(EdgeLabel), _
                           New FrameworkPropertyMetadata(Nothing))
End Class




