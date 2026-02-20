namespace Dominio.ObjetosDeValor.Embarcador.Enumeradores
{
    public enum TipoAplicacaoCotacao
    {
        ExcluirTransportador = 1,
        UsarTransportador = 2,
        ValorPercentualCobrancaCliente = 3,
        AdicionarDiasAoFrete = 4,
        FixarDiasDeFrete = 5,
        FixarValorCotacaoFrete = 6,
        ExcluirCotacao = 7,
        UtilizarModeloVeicular = 8,
        ValorParaCobranca = 9
    }

    public static class TipoAplicacaoCotacaoHelper
    {
        public static string ObterDescricao(this TipoAplicacaoCotacao tipoAplicacaoCotacao)
        {
            switch (tipoAplicacaoCotacao)
            {
                case TipoAplicacaoCotacao.AdicionarDiasAoFrete: return "Adicionar Dias ao Frete";
                case TipoAplicacaoCotacao.ExcluirCotacao: return "Excluir Cotação";
                case TipoAplicacaoCotacao.ExcluirTransportador: return "Excluir Transportador";
                case TipoAplicacaoCotacao.FixarDiasDeFrete: return "Fixar Dias de Frete";
                case TipoAplicacaoCotacao.FixarValorCotacaoFrete: return "Fixar Valor da Cotação do Frete";
                case TipoAplicacaoCotacao.UsarTransportador: return "Usar Transportador";
                case TipoAplicacaoCotacao.ValorPercentualCobrancaCliente: return "Valor Percentual para Cobrança";
                case TipoAplicacaoCotacao.UtilizarModeloVeicular: return "Utilizar Modelo Veicular";
                case TipoAplicacaoCotacao.ValorParaCobranca: return "Valor para Cobrança";
                default: return string.Empty;
            }
        }
    }
}
