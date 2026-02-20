using NHibernate.Linq;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Dynamic.Core;
using System.Threading;
using System.Threading.Tasks;

namespace Repositorio.Embarcador.Pedidos
{
    public class CodigoIntegracaoGerenciadoraRisco : RepositorioBase<Dominio.Entidades.Embarcador.Pedidos.CodigoIntegracaoGerenciadoraRisco>
    {
        #region Construtores

        public CodigoIntegracaoGerenciadoraRisco(UnitOfWork unitOfWork) : base(unitOfWork) { }

        public CodigoIntegracaoGerenciadoraRisco(UnitOfWork unitOfWork, CancellationToken cancellationToken) : base(unitOfWork, cancellationToken) { }

        #endregion Construtores

        #region Métodos Públicos

        public Task<List<Dominio.Entidades.Embarcador.Pedidos.CodigoIntegracaoGerenciadoraRisco>> BuscarPorTipoOperacaoAsync(int codigo, CancellationToken cancellationToken)
        {
            IQueryable<Dominio.Entidades.Embarcador.Pedidos.CodigoIntegracaoGerenciadoraRisco> query = SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Pedidos.CodigoIntegracaoGerenciadoraRisco>()
                .Where(obj => obj.TipoOperacao.Codigo == codigo);

            return query.ToListAsync(cancellationToken);
        }

        public Task<List<string>> BuscarCodigosIntegracaoPorTipoOperacaoEtapaAsync(int codigo, Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoCarga etapaIntegracao)
        {
            IQueryable<Dominio.Entidades.Embarcador.Pedidos.CodigoIntegracaoGerenciadoraRisco> query = SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Pedidos.CodigoIntegracaoGerenciadoraRisco>()
                .Where(obj => obj.TipoOperacao.Codigo == codigo);

            if (query.Any(obj => obj.EtapaCarga == etapaIntegracao))
                query = query.Where(obj => obj.EtapaCarga == etapaIntegracao);
            else
                query = query.Where(obj => obj.EtapaCarga == Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoCarga.Todas || obj.EtapaCarga == null);

            return query
                .Select(obj => obj.CodigoIntegracao)
                .ToListAsync();
        }

        #endregion Métodos Públicos
    }
}
