Imports Newtonsoft.Json

Public Class JsonNetResult
    Inherits JsonResult

    Public Property SerializerSettings As JsonSerializerSettings
    Public Property Formatting As Formatting

    Public Sub New()
        SerializerSettings = New JsonSerializerSettings()
        SerializerSettings.Converters.Add(New Newtonsoft.Json.Converters.IsoDateTimeConverter())
    End Sub


    Public Overrides Sub ExecuteResult(context As System.Web.Mvc.ControllerContext)

        If context Is Nothing Then
            Throw New ArgumentNullException("context")
        End If

        Dim response = context.HttpContext.Response

        response.ContentType = If(String.IsNullOrEmpty(ContentType) = False, ContentType, "application/json")

        If ContentEncoding IsNot Nothing Then
            response.ContentEncoding = ContentEncoding
        End If

        If Data IsNot Nothing Then
            Dim writer As New JsonTextWriter(response.Output) With {.Formatting = Formatting}

            Dim serializer = JsonSerializer.Create(SerializerSettings)
            serializer.Serialize(writer, Data)

            writer.Flush()
        End If
    End Sub

End Class
