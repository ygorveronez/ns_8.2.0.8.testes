namespace Dominio.ObjetosDeValor.Embarcador.Enumeradores
{
    public enum SituacaoCargaJanelaCarregamento
    {
        AgAprovacaoComercial = 1,
        SemValorFrete = 2,
        SemTransportador = 3,
        AgConfirmacaoTransportador = 4,
        ProntaParaCarregamento = 5,
        ReprovacaoComercial = 6,
        AgLiberacaoParaTransportadores = 7,
        LiberarAutomaticamenteFaturamento = 8,
        AgEncosta = 9,
        AgAceiteTransportador = 10
    }

    public static class SituacaoCargaJanelaCarregamentoHelper
    {
        public static string ObterCorLinha(this SituacaoCargaJanelaCarregamento situacao)
        {
            switch (situacao)
            {
                case SituacaoCargaJanelaCarregamento.AgAceiteTransportador: return "#e4c49b";
                case SituacaoCargaJanelaCarregamento.AgAprovacaoComercial: return "#fbfbfb";
                case SituacaoCargaJanelaCarregamento.AgConfirmacaoTransportador: return "#e29e23";
                case SituacaoCargaJanelaCarregamento.AgEncosta: return "#e699ff";
                case SituacaoCargaJanelaCarregamento.AgLiberacaoParaTransportadores: return "#1667c6";
                case SituacaoCargaJanelaCarregamento.ProntaParaCarregamento: return "#85de7b";
                case SituacaoCargaJanelaCarregamento.ReprovacaoComercial: return "#d13636";
                case SituacaoCargaJanelaCarregamento.SemTransportador: return "#c8e8ff";
                case SituacaoCargaJanelaCarregamento.SemValorFrete: return "#ddd855";
                default: return string.Empty;
            }
        }

        public static string ObterCorFonte(this SituacaoCargaJanelaCarregamento situacao)
        {
            switch (situacao)
            {
                case SituacaoCargaJanelaCarregamento.AgLiberacaoParaTransportadores: return "#ffffff";
                default: return "#666";
            }
        }

        public static string ObterDescricao(this SituacaoCargaJanelaCarregamento situacao)
        {
            switch (situacao)
            {
                case SituacaoCargaJanelaCarregamento.AgAceiteTransportador: return "Ag. Aceite do Transportador";
                case SituacaoCargaJanelaCarregamento.AgAprovacaoComercial: return "Ag. Aprovação do Comercial";
                case SituacaoCargaJanelaCarregamento.AgConfirmacaoTransportador: return "Ag. Confirmação do Transportador";
                case SituacaoCargaJanelaCarregamento.AgEncosta: return "Ag. Encosta";
                case SituacaoCargaJanelaCarregamento.AgLiberacaoParaTransportadores: return "Ag. Liberação para Transportadores";
                case SituacaoCargaJanelaCarregamento.LiberarAutomaticamenteFaturamento: return "Liberação automatica para o Faturamento.";
                case SituacaoCargaJanelaCarregamento.ProntaParaCarregamento: return "Pronta para Carregamento";
                case SituacaoCargaJanelaCarregamento.ReprovacaoComercial: return "Reprovada pelo Comercial";
                case SituacaoCargaJanelaCarregamento.SemTransportador: return "Sem Transportador";
                case SituacaoCargaJanelaCarregamento.SemValorFrete: return "Sem Valor de Frete";
                default: return string.Empty;
            }
        }
    }
}