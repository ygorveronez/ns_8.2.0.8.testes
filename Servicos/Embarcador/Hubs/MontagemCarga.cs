namespace Servicos.Embarcador.Hubs
{
    public class MontagemCarga : HubBase<MontagemCarga>
    {
        public void InformarQuantidadeProcessadosCarregamentoAutomatico(int quantidadeTotal, int quantidadeProcessada, int codigoSessaoRoteirizador, string descricao)
        {
            var retorno = new
            {
                QuantidadeProcessada = quantidadeProcessada,
                QuantidadeTotal = quantidadeTotal,
                SessaoRoterizador = codigoSessaoRoteirizador,
                Descricao = descricao
            };

            SendToAll("informarQuantidadeProcessadosCarregamentoAutomatico", retorno);
        }

        public void InformarCarregamentoAutomaticoFinalizado(string erro, int codigoSessaoRoteirizador, int qtdePedidosPesoMaior = 0, decimal maiorCapacidadeVeicular = 0)
        {
            var retorno = new
            {
                erro,
                SessaoRoterizador = codigoSessaoRoteirizador,
                QtdePedidosPesoMaior = qtdePedidosPesoMaior,
                MaiorCapacidadeVeicular = maiorCapacidadeVeicular
            };

			SendToAll("informarCarregamentoAutomaticoFinalizado", retorno);
        }

        public void InformarQuantidadeProcessadosCargaEmLote(int quantidadeTotal, int quantidadeProcessada, int codigoSessaoRoteirizador)
        {
            var retorno = new
            {
                QuantidadeProcessada = quantidadeProcessada,
                QuantidadeTotal = quantidadeTotal,
                SessaoRoterizador = codigoSessaoRoteirizador
            };

			SendToAll("informarQuantidadeProcessadosCargaEmLote", retorno);
        }

        public void InformarCargaEmLoteFinalizado(string erro, int codigoSessaoRoteirizador)
        {
            var retorno = new
            {
                erro,
                SessaoRoterizador = codigoSessaoRoteirizador
            };

			SendToAll("informarCargaEmLoteFinalizado", retorno);
        }

        public void InformarCargaBackgroundFinalizado(string erro, int codigoSessaoRoteirizador, int codigoCarregamento, string numeroCarregamento)
        {
            var retorno = new
            {
                erro,
                SessaoRoterizador = codigoSessaoRoteirizador,
                Carregamento = codigoCarregamento,
                NumeroCarregamento = numeroCarregamento
            };

			SendToAll("informarCargaBackgroundFinalizado", retorno);
        }

        public void InformarFiltroPesquisaGestaoPedidoSessaoRoteirizador(dynamic body)
        {
            SendToAll("informarFiltroPesquisaGestaoPedidoSessaoRoteirizador", body);
        }
    }
}
