namespace Dominio.ObjetosDeValor.Embarcador.Enumeradores
{
    public enum TipoOSConvertido
    {
        CargaCheia = 0,
        CustoExtra = 1,
        Nenhum = 2,
        NaoInformado = 9
    }

    public static class EnumTipoOSConvertidoeHelper
    {
        public static string ObterDescricao(this TipoOSConvertido situacao)
        {
            switch (situacao)
            {
                case TipoOSConvertido.CargaCheia: return "Carga Cheia";
                case TipoOSConvertido.CustoExtra: return "Custo Extra";
                case TipoOSConvertido.Nenhum: return "Nenhum";
                case TipoOSConvertido.NaoInformado: return "Não Informado";
                default: return string.Empty;
            }
        }
    }
}
