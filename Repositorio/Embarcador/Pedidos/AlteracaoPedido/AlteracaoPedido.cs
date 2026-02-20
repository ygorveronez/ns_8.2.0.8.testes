using Dominio.ObjetosDeValor.Embarcador.Enumeradores;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Dynamic.Core;

namespace Repositorio.Embarcador.Pedidos.AlteracaoPedido
{
    public sealed class AlteracaoPedido : RepositorioBase<Dominio.Entidades.Embarcador.Pedidos.AlteracaoPedido.AlteracaoPedido>
    {
        #region Construtores

        public AlteracaoPedido(UnitOfWork unitOfWork) : base(unitOfWork) { }

        #endregion

        #region Métodos Privados

        private IQueryable<Dominio.ObjetosDeValor.WebService.Pedido.RetornoAlteracaoPedido> ConsultarFinalizadas()
        {
            List<SituacaoAlteracaoPedido> situacoesFinalizadas = SituacaoAlteracaoPedidoHelper.ObterSituacoesFinalizadas();
            var consultaAlteracaoPedido = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Pedidos.AlteracaoPedido.AlteracaoPedido>()
                .Where(o => situacoesFinalizadas.Contains(o.Situacao) && !o.SituacaoConsultada)
                .Select(o => new Dominio.ObjetosDeValor.WebService.Pedido.RetornoAlteracaoPedido()
                {
                    MotivoRejeicao = o.MotivoRejeicao != null ? o.MotivoRejeicao.Descricao : "",
                    ProtocoloIntegracaoPedido = o.Pedido.Codigo,
                    Situacao = o.Situacao
                });

            return consultaAlteracaoPedido;
        }

        #endregion

        #region Métodos Públicos

        public Dominio.Entidades.Embarcador.Pedidos.AlteracaoPedido.AlteracaoPedido BuscarPendentePorCarga(int codigoCarga)
        {
            List<SituacaoAlteracaoPedido> situacoesPendentes = SituacaoAlteracaoPedidoHelper.ObterSituacoesPendentes();
            var consultaAlteracaoPedido = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Pedidos.AlteracaoPedido.AlteracaoPedido>()
                .Where(o => situacoesPendentes.Contains(o.Situacao));

            var consultaCargaPedido = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.CargaPedido>()
                .Where(o => o.Carga.Codigo == codigoCarga);

            consultaAlteracaoPedido = consultaAlteracaoPedido.Where(o => consultaCargaPedido.Any(c => c.Pedido.Codigo == o.Pedido.Codigo));

            return consultaAlteracaoPedido.FirstOrDefault();
        }

        public Dominio.Entidades.Embarcador.Pedidos.AlteracaoPedido.AlteracaoPedido BuscarPendentePorPedido(int codigoPedido)
        {
            List<SituacaoAlteracaoPedido> situacoesPendentes = SituacaoAlteracaoPedidoHelper.ObterSituacoesPendentes();
            var consultaAlteracaoPedido = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Pedidos.AlteracaoPedido.AlteracaoPedido>()
                .Where(o => o.Pedido.Codigo == codigoPedido && situacoesPendentes.Contains(o.Situacao));

            return consultaAlteracaoPedido.FirstOrDefault();
        }

        public void ConfirmarConsultaFinalizadas(List<int> codigosPedidos)
        {
            if (UnitOfWork.IsActiveTransaction())
            {
                UnitOfWork.Sessao
                    .CreateQuery("update AlteracaoPedido set SituacaoConsultada = 1 where Pedido.Codigo in (:codigosPedidos) ")
                    .SetParameterList("codigosPedidos", codigosPedidos)
                    .ExecuteUpdate();
            }
            else
            {
                try
                {
                    UnitOfWork.Start();

                    UnitOfWork.Sessao
                        .CreateQuery("update AlteracaoPedido set SituacaoConsultada = 1 where Pedido.Codigo in (:codigosPedidos) ")
                        .SetParameterList("codigosPedidos", codigosPedidos)
                        .ExecuteUpdate();

                    UnitOfWork.CommitChanges();
                }
                catch
                {
                    UnitOfWork.Rollback();
                    throw;
                }
            }
        }

        public List<Dominio.ObjetosDeValor.WebService.Pedido.RetornoAlteracaoPedido> ConsultarFinalizadas(int inicioRegistros, int limiteRegistros)
        {
            var consultaAlteracaoPedido = ConsultarFinalizadas();

            return consultaAlteracaoPedido
                .Skip(inicioRegistros)
                .Take(limiteRegistros)
                .ToList();
        }

        public int ContarConsultaFinalizadas()
        {
            var consultaAlteracaoPedido = ConsultarFinalizadas();

            return consultaAlteracaoPedido.Count();
        }

        #endregion
    }
}
