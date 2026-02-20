namespace Dominio.ObjetosDeValor.Embarcador.Enumeradores
{
    public enum TipoObservacaoCTe
    {
        Contribuinte = 0,
        Fisco = 1
    }

    public static class TipoObservacaoCTeHelper
    {
        public static string ObterDescricao(this TipoObservacaoCTe tipoObservacaoCTe)
        {
            switch (tipoObservacaoCTe)
            {
                case TipoObservacaoCTe.Contribuinte: return "Contribuinte";
                case TipoObservacaoCTe.Fisco: return "Fisco";
                default: return "";
            }
        }
    }
}
