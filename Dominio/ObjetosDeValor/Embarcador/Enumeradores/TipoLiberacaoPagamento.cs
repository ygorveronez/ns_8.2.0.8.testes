namespace Dominio.ObjetosDeValor.Embarcador.Enumeradores
{
    public enum TipoLiberacaoPagamento
    {
        Nenhum = 0,
        ReceberEscrituracaoNotaProduto = 1,
        DigitalizacaoImagemCanhoto = 2,
        AprovacaoImagemCanhoto = 3,
    }

    public static class TipoLiberacaoPagamentoHelper
    {
        public static string ObterDescricao(this TipoLiberacaoPagamento status)
        {
            switch (status)
            {
                case TipoLiberacaoPagamento.Nenhum: return Localization.Resources.Gerais.Geral.Nenhum;
                case TipoLiberacaoPagamento.ReceberEscrituracaoNotaProduto: return Localization.Resources.Enumeradores.TipoLiberacaoPagamento.ReceberEscrituracaoNotaProduto;
                case TipoLiberacaoPagamento.DigitalizacaoImagemCanhoto: return Localization.Resources.Enumeradores.TipoLiberacaoPagamento.DigitalizacaoImagemCanhoto;
                case TipoLiberacaoPagamento.AprovacaoImagemCanhoto: return Localization.Resources.Enumeradores.TipoLiberacaoPagamento.AprovacaoImagemCanhoto;
                default: return string.Empty;
            }
        }
    }
}
