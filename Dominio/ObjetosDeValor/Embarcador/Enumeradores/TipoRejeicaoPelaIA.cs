namespace Dominio.ObjetosDeValor.Embarcador.Enumeradores
{
    public enum TipoRejeicaoPelaIA
    {
        Todos = 0,
        Comprovante = 1,
        NumeroDoc = 2,
        Data = 3,
        Assinatura = 4
    }

    public static class TipoRejeicaoPelaIAHelper
    {
        public static string ObterDescricao(this TipoRejeicaoPelaIA tipoRejeicaoPelaIA)
        {
            switch (tipoRejeicaoPelaIA)
            {
                case TipoRejeicaoPelaIA.Comprovante: return "Comprovante";
                case TipoRejeicaoPelaIA.NumeroDoc: return "Número Doc.";
                case TipoRejeicaoPelaIA.Data: return "Data";
                case TipoRejeicaoPelaIA.Assinatura: return "Assinatura";
                default: return "";
            }
        }
    }
}
