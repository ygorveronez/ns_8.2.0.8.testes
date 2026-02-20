using System;
using System.Collections.Generic;
using System.Linq;

namespace Servicos.Embarcador.Pedido
{
    public class PedidoProduto
    {
        #region Construtores
        private readonly Repositorio.UnitOfWork _unitOfWork;

        public PedidoProduto(Repositorio.UnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        #endregion

        #region Métodos Públicos

        public int ObterSaldoQuantidadeCaixasPendentesAgendamentoColeta(Dominio.Entidades.Embarcador.Pedidos.PedidoProduto pedidoProduto, List<Dominio.Entidades.Embarcador.Logistica.AgendamentoColetaPedidoProduto> listAgendamentoColetaPedidoProduto, List<Dominio.Entidades.Embarcador.Pedidos.AlteracaoPedidoProdutoAgendamento> listaPedidoProdutoAgendamento)
        {
            Repositorio.Embarcador.Logistica.CentroDescarregamento centroDescarregamento = new Repositorio.Embarcador.Logistica.CentroDescarregamento(_unitOfWork);

            if (!centroDescarregamento.BuscarPorDestinatario(pedidoProduto.Pedido.Destinatario.Codigo)?.UsarLayoutAgendamentoPorCaixaItem ?? false)
            {
                int saldoCaixasPendente = (int)pedidoProduto.Quantidade;
                Dominio.Entidades.Embarcador.Pedidos.AlteracaoPedidoProdutoAgendamento alteracaoPedidoProdutoAgendamento = listaPedidoProdutoAgendamento.Find(x => x.PedidoProduto.Codigo == pedidoProduto.Codigo);
                if (alteracaoPedidoProdutoAgendamento != null)
                    saldoCaixasPendente = (int)alteracaoPedidoProdutoAgendamento.NovaQuantidadeProduto;

                foreach (Dominio.Entidades.Embarcador.Logistica.AgendamentoColetaPedidoProduto agendamentoColetaPedidoProduto in listAgendamentoColetaPedidoProduto)
                    saldoCaixasPendente -= agendamentoColetaPedidoProduto.Quantidade;

                int retorno = (int)Math.Ceiling((decimal)saldoCaixasPendente / Math.Max(pedidoProduto.Produto.QuantidadeCaixa, 1));
                return retorno > 0 ? retorno : 0;
            }
            else
                return pedidoProduto.Pedido.SaldoVolumesRestante;
        }

        public int ObterSaldoQuantidadeItensPendentesAgendamentoColeta(Dominio.Entidades.Embarcador.Pedidos.PedidoProduto pedidoProduto, List<Dominio.Entidades.Embarcador.Logistica.AgendamentoColetaPedidoProduto> listAgendamentoColetaPedidoProduto, List<Dominio.Entidades.Embarcador.Pedidos.AlteracaoPedidoProdutoAgendamento> listaPedidoProdutoAgendamento)
        {
            Repositorio.Embarcador.Logistica.CentroDescarregamento centroDescarregamento = new Repositorio.Embarcador.Logistica.CentroDescarregamento(_unitOfWork);

            if (!centroDescarregamento.BuscarPorDestinatario(pedidoProduto.Pedido.Destinatario.Codigo)?.UsarLayoutAgendamentoPorCaixaItem ?? false)
            {
                int saldoItensPendente = (int)pedidoProduto.Quantidade;
                Dominio.Entidades.Embarcador.Pedidos.AlteracaoPedidoProdutoAgendamento alteracaoPedidoProdutoAgendamento = null;
                if (listaPedidoProdutoAgendamento.Count > 0)
                    alteracaoPedidoProdutoAgendamento = listaPedidoProdutoAgendamento.Find(x => x.PedidoProduto.Codigo == pedidoProduto.Codigo);

                if (alteracaoPedidoProdutoAgendamento == null)
                {
                    foreach (Dominio.Entidades.Embarcador.Logistica.AgendamentoColetaPedidoProduto agendamentoColetaPedidoProduto in listAgendamentoColetaPedidoProduto)
                        saldoItensPendente -= agendamentoColetaPedidoProduto.Quantidade;
                }
                else
                    saldoItensPendente = (int)alteracaoPedidoProdutoAgendamento.NovaQuantidadeProduto;

                return saldoItensPendente > 0 ? 1 : 0;
            }
            else
                return 1;

        }

        public int ObterSaldoQuantidadeProdutosPendentesAgendamentoColeta(Dominio.Entidades.Embarcador.Pedidos.PedidoProduto pedidoProduto, List<Dominio.Entidades.Embarcador.Logistica.AgendamentoColetaPedidoProduto> listAgendamentoColetaPedidoProduto, List<Dominio.Entidades.Embarcador.Pedidos.AlteracaoPedidoProdutoAgendamento> listaPedidoProdutoAgendamento)
        {
            int saldoProdutosPendente = (int)pedidoProduto.Quantidade;           

            foreach (Dominio.Entidades.Embarcador.Logistica.AgendamentoColetaPedidoProduto agendamentoColetaPedidoProduto in listAgendamentoColetaPedidoProduto.Where(x => x.PedidoProduto.Codigo == pedidoProduto.Codigo && ValidaCanceladaDesagendada(x.AgendamentoColetaPedido.AgendamentoColeta, _unitOfWork)))            
                    saldoProdutosPendente -= agendamentoColetaPedidoProduto.Quantidade;
            
            return saldoProdutosPendente;
        }

        private bool ValidaCanceladaDesagendada(Dominio.Entidades.Embarcador.Logistica.AgendamentoColeta agendamentoColeta, Repositorio.UnitOfWork unitOfWork)
        {   
            if (agendamentoColeta.Situacao == Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoAgendamentoColeta.Agendado || agendamentoColeta.Situacao == Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoAgendamentoColeta.Finalizado)
                return true;

            return false;
        }

        #endregion Métodos Públicos
    }
}