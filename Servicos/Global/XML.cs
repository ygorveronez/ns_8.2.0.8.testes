namespace Servicos
{
    public static class XML
    {
        public static string ConvertObjectToXMLString<T>(T objeto, bool indentar = true)
        {
            return Utilidades.XML.Serializar(objeto, indentar);
        }

        public static T ConvertXMLStringToObject<T>(string xml)
        {
            return Utilidades.XML.Deserializar<T>(xml);
        }
    }
}
