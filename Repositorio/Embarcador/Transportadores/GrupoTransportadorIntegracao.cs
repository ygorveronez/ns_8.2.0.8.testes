using System.Collections.Generic;
using System.Linq;
using NHibernate.Linq;
using System.Threading.Tasks;
using System.Threading;

namespace Repositorio.Embarcador.Transportadores
{
    public class GrupoTransportadorIntegracao : RepositorioBase<Dominio.Entidades.Embarcador.Transportadores.GrupoTransportadorIntegracao>
    {
        #region Construtores

        public GrupoTransportadorIntegracao(UnitOfWork unitOfWork) : this(unitOfWork, default) { }

        public GrupoTransportadorIntegracao(UnitOfWork unitOfWork, CancellationToken cancellationToken) : base(unitOfWork, cancellationToken) { }

        #endregion Construtores

        #region Métodos Públicos
        public Task<List<Dominio.Entidades.Embarcador.Transportadores.GrupoTransportadorIntegracao>> BuscarIntegracoesPorGrupoTransportadorAsync(int codigoGrupoTransportador)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Transportadores.GrupoTransportadorIntegracao>()
                .Where(o => o.GrupoTransportador.Codigo == codigoGrupoTransportador);

            return query.ToListAsync(CancellationToken);
        }

        public Task<List<Dominio.Entidades.Embarcador.Transportadores.GrupoTransportadorIntegracao>> BuscarPorTipoAsync(Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoIntegracao tipoIntegracao)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Transportadores.GrupoTransportadorIntegracao>()
                .Where(o => o.Tipo == tipoIntegracao);

            return query.ToListAsync(CancellationToken);
        }

        public Task<List<Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoIntegracao>> BuscarTiposIntegracaoPorTransportadorAsync(int codigoTransportador)
        {
            var gruposDaEmpresa = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Transportadores.GrupoTransportadorEmpresa>()
                .Where(x => x.Empresa.Codigo == codigoTransportador)
                .Select(x => x.GrupoTransportador.Codigo);

            var tiposIntegracao = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Transportadores.GrupoTransportadorIntegracao>()
                .Where(x => gruposDaEmpresa.Contains(x.GrupoTransportador.Codigo))
                .Select(x => x.Tipo);

            return tiposIntegracao.ToListAsync(CancellationToken);
        }

        #endregion
    }
}
