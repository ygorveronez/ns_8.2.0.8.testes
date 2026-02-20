namespace Dominio.ObjetosDeValor.Embarcador.Enumeradores
{
    public enum TipoAcaoParcial
    {
        EntregueParcialmente = 1,
        DevolvidaParcialmente = 2
    }

    public static class TipoAcaoParcialHelper
    {
        public static string ObterDescricao(this TipoAcaoParcial acao)
        {
            switch (acao)
            {
                case TipoAcaoParcial.EntregueParcialmente: return "Entregue Parcialmente";
                case TipoAcaoParcial.DevolvidaParcialmente: return "Devolvida Parcialmente";
                default: return string.Empty;
            }
        }

        public static string ObterCorLinha(this TipoAcaoParcial acao)
        {
            switch (acao)
            {
                case TipoAcaoParcial.EntregueParcialmente: return "#7a7a7a";
                case TipoAcaoParcial.DevolvidaParcialmente: return "#baba7b";
                default: return string.Empty;
            }
        }

        public static string ObterCorFonte(this TipoAcaoParcial acao)
        {
            switch (acao)
            {
                case TipoAcaoParcial.EntregueParcialmente: return "#edeef0";
                default: return "#666";
            }
        }
    }
}
