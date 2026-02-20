namespace Dominio.ObjetosDeValor.ProcessadorTarefas.Enumeradores
{
    public enum TipoRequest
    {
        AdicionarPedidoEmLote = 0,
        GerarCarregamento = 1,
        GerarCarregamentoComRedespachos = 2,
        EnviarDigitalizacaoCanhotoEmLote = 3,
        AdicionarAtendimentoEmLote = 4,
        GerarCarregamentoRoteirizacaoEmLote = 5,
    }

    public static class TipoRequestExtensions
    {
        public static string ObterDescricao(this TipoRequest tipoRequest)
        {
            return tipoRequest switch
            {
                TipoRequest.AdicionarPedidoEmLote => "Adicionar Pedido em Lote",
                TipoRequest.GerarCarregamento => "Gerar Carregamento",
                TipoRequest.GerarCarregamentoComRedespachos => "Gerar Carregamento com Redespachos",
                TipoRequest.EnviarDigitalizacaoCanhotoEmLote => "Enviar Digitalização Canhoto Em Lote",
                TipoRequest.AdicionarAtendimentoEmLote => "Adicionar Atendimento Em Lote",
                TipoRequest.GerarCarregamentoRoteirizacaoEmLote => "Gerar Carregamento Roteirização Em Lote",
                _ => string.Empty,
            };
        }
    }
}
