using SGT.BackgroundWorkers.Utils;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace SGT.BackgroundWorkers
{
    [RunningConfig(DuracaoPadrao = 60000)]

    public class ExclusaoPedido : LongRunningProcessBase<ExclusaoPedido>
    {
        protected override async Task ExecuteInternalAsync(Repositorio.UnitOfWork unitOfWork, AdminMultisoftware.Repositorio.UnitOfWork unitOfWorkAdmin, CancellationToken cancellationToken)
        {
            ExcluirPedidosEmCotacao(unitOfWork, unitOfWorkAdmin);
        }

        private void ExcluirPedidosEmCotacao(Repositorio.UnitOfWork unitOfWork, AdminMultisoftware.Repositorio.UnitOfWork unitOfWorkAdmin)
        {
            try
            {
                Repositorio.Embarcador.Configuracoes.ConfiguracaoPedido repositorioConfiguracaoPedido = new Repositorio.Embarcador.Configuracoes.ConfiguracaoPedido(unitOfWork);
                Dominio.Entidades.Embarcador.Configuracoes.ConfiguracaoPedido configuracaoPedido = repositorioConfiguracaoPedido.BuscarConfiguracaoPadrao();
                if (configuracaoPedido.DiasExcluirPedidoEmCotacao > 0)
                {
                    List<int> pedidosErroExcluir = new List<int>();
                    string msgErro = string.Empty;
                    Servicos.Embarcador.Pedido.Cotacao servicoCotacao = new Servicos.Embarcador.Pedido.Cotacao(unitOfWork);
                    servicoCotacao.LimparBasePedidosContacao(configuracaoPedido.ExcluirTambemACotacaoDoPedido, configuracaoPedido.DiasExcluirPedidoEmCotacao, pedidosErroExcluir, ref msgErro);

                    if (pedidosErroExcluir.Count > 0)
                        Servicos.Log.TratarErro("Não foi possível excluir os seguintes pedidos.: " + string.Join(",", pedidosErroExcluir) + Environment.NewLine + msgErro, "ExclusaoPedido");
                }
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);
            }
        }
    }
}