namespace Dominio.ObjetosDeValor.Embarcador.Enumeradores
{
    public enum TipoCarregamentoNavio
    {
        NaoInformado = 0,
        Comum = 1,
        Reefer = 2
    }

    public static class TipoCarregamentoNavioHelper
    {
        public static string ObterDescricao(this TipoCarregamentoNavio tipoCarregamentoNavio)
        {
            switch (tipoCarregamentoNavio)
            {
                case TipoCarregamentoNavio.NaoInformado: return "Não informado";
                case TipoCarregamentoNavio.Comum: return "Comum";
                case TipoCarregamentoNavio.Reefer: return "Reefer";

                default: return string.Empty;
            }
        }
    }
}
