using NHibernate.Linq;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Repositorio.Embarcador.Pedidos
{
    public class TipoOperacaoTipoCarga : RepositorioBase<Dominio.Entidades.Embarcador.Pedidos.TipoOperacaoTipoCarga>
    {
        public TipoOperacaoTipoCarga(UnitOfWork unitOfWork) : base(unitOfWork) { }
        public TipoOperacaoTipoCarga(UnitOfWork unitOfWork, CancellationToken cancellationToken) : base(unitOfWork, cancellationToken) { }

        #region Métodos Públicos

        public List<Dominio.Entidades.Embarcador.Pedidos.TipoOperacaoTipoCarga> BuscarPorTipoOperacao(int codigoTipoOperacao)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Pedidos.TipoOperacaoTipoCarga>();

            query = query.Where(o => o.TipoOperacao.Codigo == codigoTipoOperacao);

            return query.ToList();
        }

        public Dominio.Entidades.Embarcador.Pedidos.TipoOperacaoTipoCarga BuscarPorTipoOperacaoETipoCarga(int codigoTipoOperacao, int codigoTipoCarga)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Pedidos.TipoOperacaoTipoCarga>();

            query = query.Where(o => o.TipoOperacao.Codigo == codigoTipoOperacao && o.TipoCarga.Codigo == codigoTipoCarga);

            return query.FirstOrDefault();
        }

        public List<int> BuscarCodigosTiposCargasPorTipoOperacao(int codigoTipoOperacao)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Pedidos.TipoOperacaoTipoCarga>();

            query = query.Where(o => o.TipoOperacao.Codigo == codigoTipoOperacao);

            return query.Select(o => o.TipoCarga.Codigo)
                .ToList();
        }
        public async Task<List<int>> BuscarCodigosTiposCargasPorTipoOperacaoAsync(int codigoTipoOperacao, CancellationToken cancellationToken)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Pedidos.TipoOperacaoTipoCarga>();

            query = query.Where(o => o.TipoOperacao.Codigo == codigoTipoOperacao);

            return await query.Select(o => o.TipoCarga.Codigo)
                .ToListAsync(cancellationToken);
        }

        #endregion
    }
}