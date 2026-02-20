using NHibernate.Linq;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;


namespace Repositorio.Embarcador.Cargas
{
    public class CargaOfertaIntegracao : RepositorioBase<Dominio.Entidades.Embarcador.Cargas.CargaOfertaIntegracao>
    {
        public CargaOfertaIntegracao(UnitOfWork unitOfWork) : this(unitOfWork, default) { }

        public CargaOfertaIntegracao(UnitOfWork unitOfWork, CancellationToken cancellationToken) : base(unitOfWork, cancellationToken) { }

        #region Métodos Públicos

        public Task<List<Dominio.Entidades.Embarcador.Cargas.CargaOfertaIntegracao>> BuscarIntegracaoPorCargaOfertaAsync(long cargaOfertaCodigo, CancellationToken cancellationToken)
        {
            IQueryable<Dominio.Entidades.Embarcador.Cargas.CargaOfertaIntegracao> buscaintegracoes = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.CargaOfertaIntegracao>()
                .Where(integracao => integracao.CargaOferta.Codigo == cargaOfertaCodigo);

            return buscaintegracoes.ToListAsync(cancellationToken);
        }

        public Task<List<int>> BuscarIntegracoesAguardandoAsync(int numeroRegistrosPorVez, CancellationToken cancellationToken)
        {
            IQueryable<Dominio.Entidades.Embarcador.Cargas.CargaOfertaIntegracao> query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.CargaOfertaIntegracao>()
                .Where(cargaOfertaIntegracao => cargaOfertaIntegracao.SituacaoIntegracao == Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoIntegracao.AgIntegracao);

            return query.Select(x => (int)x.Codigo)
                .Skip(0)
                .Take(numeroRegistrosPorVez)
                .ToListAsync<int>(cancellationToken);
        }

        public Task<List<Dominio.Entidades.Embarcador.Cargas.CargaOfertaIntegracao>> BuscarPorCodigoCargaOfertaEPorTipo(List<long> codigosCargasOfertas, Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoIntegracaoOfertaCarga tipo, CancellationToken cancellationToken)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.CargaOfertaIntegracao>()
                .Where(integracao => integracao.Tipo == tipo && codigosCargasOfertas.Contains(integracao.CargaOferta.Codigo));

            return query.ToListAsync(cancellationToken);
        }

        #endregion Métodos Públicos

    }
}