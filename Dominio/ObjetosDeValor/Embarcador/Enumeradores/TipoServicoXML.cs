namespace Dominio.ObjetosDeValor.Embarcador.Enumeradores
{
    public enum TipoServicoXML
    {
        TransporteRodoviario = 0,
        Ova = 1,
        MaterialPeacao = 2,
    }

    public static class EnumTipoServicoXMLHelper
    {
        public static string ObterDescricao(this TipoServicoXML situacao)
        {
            switch (situacao)
            {
                case TipoServicoXML.TransporteRodoviario: return "Transporte Rodoviário";
                case TipoServicoXML.Ova: return "Ova";
                case TipoServicoXML.MaterialPeacao: return "Material de Peação";
                default: return string.Empty;
            }
        }
    }
}
