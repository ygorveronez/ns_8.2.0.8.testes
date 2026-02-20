using Dominio.ObjetosDeValor.Embarcador.Enumeradores;
using NHibernate.Linq;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Repositorio.Embarcador.Roteirizador
{
    public class RoteirizadorIntegracaoPedido : RepositorioBase<Dominio.Entidades.Embarcador.Roteirizador.RoteirizadorIntegracaoPedido>
    {
        #region Construtores

        public RoteirizadorIntegracaoPedido(UnitOfWork unitOfWork) : base(unitOfWork) { }

        public RoteirizadorIntegracaoPedido(UnitOfWork unitOfWork, CancellationToken cancellationToken) : base(unitOfWork, cancellationToken) { }

        #endregion Construtores

        #region Métodos Públicos

        public List<Dominio.Entidades.Embarcador.Pedidos.Pedido> BuscarPedidosPorRoteirizadorIntegracao(int codigoRoteirizadorIntegracao)
        {
            var consultaRoteirizadorIntegracaoPedido = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Roteirizador.RoteirizadorIntegracaoPedido>()
                .Where(integracaoPedido => integracaoPedido.RoteirizadorIntegracao.Codigo == codigoRoteirizadorIntegracao);

            return consultaRoteirizadorIntegracaoPedido.Select(integracaoPedido => integracaoPedido.Pedido).ToList();
        }

        public List<int> BuscarCodigoPedidosIntegradosRoteirizador(List<int> codigosPedidos)
        {
            int limiteRegistros = 1000;
            List<int> codigosPedidosIntegrados = new List<int>();

            for (int registroInicial = 0; registroInicial < codigosPedidos.Count; registroInicial += limiteRegistros)
            {
                List<int> codigosPedidosPaginado = codigosPedidos.Skip(registroInicial).Take(limiteRegistros).ToList();

                var consultaPedido = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Pedidos.Pedido>()
                    .Where(pedido => pedido.SituacaoRoteirizadorIntegracao == SituacaoRoteirizadorIntegracao.Integrado && codigosPedidosPaginado.Contains(pedido.Codigo));

                codigosPedidosIntegrados.AddRange(consultaPedido.Select(pedido => pedido.Codigo).ToList());
            }

            return codigosPedidosIntegrados;
        }

        public void InserirPedidosPorRoteirizadorIntegracao(int codigoRoteirizadorIntegracao, List<int> codigosPedidos)
        {
            int limiteRegistros = 1000;

            for (int registroInicial = 0; registroInicial < codigosPedidos.Count; registroInicial += limiteRegistros)
            {
                List<int> codigosPedidosPaginado = codigosPedidos.Skip(registroInicial).Take(limiteRegistros).ToList();

                UnitOfWork.Sessao
                    .CreateSQLQuery($@"
                        insert into T_ROTEIRIZADOR_INTEGRACAO_PEDIDO (RIN_CODIGO, PED_CODIGO)
                        select {codigoRoteirizadorIntegracao}, PED_CODIGO
                          from T_PEDIDO
                         where PED_CODIGO in ({string.Join(", ", codigosPedidosPaginado)})"
                    )
                    .ExecuteUpdate();
            }
        }

        public void AtualizarSituacaoRoteirizacaoPedidosPorRoteirizadorIntegracao(int codigoRoteirizadorIntegracao, SituacaoRoteirizadorIntegracao situacao)
        {
            UnitOfWork.Sessao
                .CreateSQLQuery($@"
                    update T_PEDIDO
                       set PED_SITUACAO_ROTEIRIZADOR_INTEGRACAO = {(int)situacao}
                     where PED_CODIGO in (
                               select PED_CODIGO
                                 from T_ROTEIRIZADOR_INTEGRACAO_PEDIDO
                                where RIN_CODIGO = {codigoRoteirizadorIntegracao}
                           )"
                )
                .ExecuteUpdate();
        }

        public Task<List<Dominio.Entidades.Embarcador.Roteirizador.RoteirizadorIntegracao>> BuscarRoteirizadorIntegracaoPorPedidoAsync(int codigoPedido)
        {
            var consultaRoteirizadorIntegracaoPedido = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Roteirizador.RoteirizadorIntegracaoPedido>()
                .Where(integracaoPedido => integracaoPedido.Pedido.Codigo == codigoPedido);

            return consultaRoteirizadorIntegracaoPedido.Select(integracaoPedido => integracaoPedido.RoteirizadorIntegracao).ToListAsync(CancellationToken);
        }

        #endregion Métodos Públicos
    }
}
