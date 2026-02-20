using Dominio.ObjetosDeValor.Embarcador.Enumeradores;
using NHibernate.Linq;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Repositorio.Embarcador.Cargas
{
    public class CargaPedidoHistorico : RepositorioBase<Dominio.Entidades.Embarcador.Cargas.CargaPedidoHistorico>
    {
        public CargaPedidoHistorico(UnitOfWork unitOfWork) : base(unitOfWork) { }
        public CargaPedidoHistorico(UnitOfWork unitOfWork, CancellationToken cancellationToken) : base(unitOfWork, cancellationToken) { }

        public async Task<bool> ExistemRegistrosParaIntegrar(int codigoCarga)
        {
            var query = this.SessionNHiBernate
                .Query<Dominio.Entidades.Embarcador.Cargas.CargaPedidoHistorico>()
                .Where(cargaPedidoHistorico =>
                    cargaPedidoHistorico.Carga.Codigo == codigoCarga &&
                    cargaPedidoHistorico.TipoAcao == CargaPedidoHistoricoTipoAcao.Exclusao &&
                    cargaPedidoHistorico.SituacaoIntegracao == CargaPedidoHistoricoSituacaoIntegracao.Aguardando
                );

            return await query.Select(cargaPedidoHistorico => cargaPedidoHistorico.Codigo).AnyAsync(this.CancellationToken);
        }

        public async Task<List<Dominio.Entidades.Embarcador.Cargas.CargaPedidoHistorico>> BuscarParaIntegracaoRouteasy(int codigoCarga)
        {
            var query = this.SessionNHiBernate
                .Query<Dominio.Entidades.Embarcador.Cargas.CargaPedidoHistorico>()
                .Where(cargaPedidoHistorico =>
                    cargaPedidoHistorico.Carga.Codigo == codigoCarga &&
                    cargaPedidoHistorico.TipoAcao == CargaPedidoHistoricoTipoAcao.Exclusao &&
                    cargaPedidoHistorico.SituacaoIntegracao == CargaPedidoHistoricoSituacaoIntegracao.Aguardando
                )
                .Fetch(cargaPedidoHistorico => cargaPedidoHistorico.Pedido)
                .Fetch(cargaPedidoHistorico => cargaPedidoHistorico.Carga);

            return await query.ToListAsync(this.CancellationToken);
        }

        public async Task<Dominio.Entidades.Embarcador.Cargas.CargaPedidoHistorico> BuscarPorPedido(int codigoPedido)
        {
            var query = this.SessionNHiBernate
                .Query<Dominio.Entidades.Embarcador.Cargas.CargaPedidoHistorico>()
                .Where(cargaPedidoHistorico =>
                    cargaPedidoHistorico.Pedido.Codigo == codigoPedido
                )
                .Fetch(cargaPedidoHistorico => cargaPedidoHistorico.Pedido)
                .Fetch(cargaPedidoHistorico => cargaPedidoHistorico.Carga)
                .Fetch(cargaPedidoHistorico => cargaPedidoHistorico.Carga.Filial);

            return await query.FirstOrDefaultAsync(this.CancellationToken);
        }
    }
}
