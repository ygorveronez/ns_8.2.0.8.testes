namespace Servicos.Embarcador.Hubs
{
    public class Pagamento : HubBase<Pagamento>
    {
        public void InformarPagamentoAtualizada(int codigoPagamento)
        {
            var retorno = new
            {
                CodigoPagamento = codigoPagamento
            };

            SendToAll("informarPagamentoAtualizada", retorno);
        }

        public void InformarQuantidadeDocumentosProcessadosFechamentoPagamento(int codigoPagamento, int quantidadeTotal, int quantidadeProcessada)
        {
            var retorno = new
            {
                CodigoPagamento = codigoPagamento,
                QuantidadeProcessada = quantidadeProcessada,
                QuantidadeTotal = quantidadeTotal
            };

			SendToAll("informarQuantidadeDocumentosProcessadosFechamentoPagamento", retorno);
        }

        public void InformarCancelamentoPagamentoAtualizada(int codigo)
        {
            var retorno = new
            {
                CodigoCancelamentoPagamento = codigo
            };

			SendToAll("informarCancelamentoPagamentoAtualizada", retorno);
        }

        public void InformarQuantidadeDocumentosProcessadosFechamentoCancelamentoPagamento(int codigo, int quantidadeTotal, int quantidadeProcessada)
        {
            var retorno = new
            {
                CodigoCancelamentoPagamento = codigo,
                QuantidadeProcessada = quantidadeProcessada,
                QuantidadeTotal = quantidadeTotal
            };

			SendToAll("informarQuantidadeDocumentosProcessadosFechamentoCancelamentoPagamento", retorno);
        }
    }
}
