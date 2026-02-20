using NHibernate.Linq;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Dynamic.Core;
using System.Threading;
using System.Threading.Tasks;

namespace Repositorio.Embarcador.Cargas
{
    public class TipoDeCargaCodigoIntegracao : RepositorioBase<Dominio.Entidades.Embarcador.Cargas.TipoDeCargaCodigoIntegracao>
    {
        #region Construtores

        public TipoDeCargaCodigoIntegracao(UnitOfWork unitOfWork) : base(unitOfWork) { }

        public TipoDeCargaCodigoIntegracao(UnitOfWork unitOfWork, CancellationToken cancellationToken) : base(unitOfWork, cancellationToken) { }

        #endregion Construtores

        #region Métodos Públicos

        public Task<List<Dominio.Entidades.Embarcador.Cargas.TipoDeCargaCodigoIntegracao>> BuscarPorTipoDeCargaAsync(int codigoTipoDeCarga)
        {
            IQueryable<Dominio.Entidades.Embarcador.Cargas.TipoDeCargaCodigoIntegracao> query = SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.TipoDeCargaCodigoIntegracao>()
                .Where(obj => obj.TipoDeCarga.Codigo == codigoTipoDeCarga);

            return query.ToListAsync();
        }

        public Task<List<string>> BuscarCodigosIntegracaoPorTipoDeCargaEtapaAsync(int codigoTipoDeCarga, Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoCarga situacaoCarga)
        {
            IQueryable<Dominio.Entidades.Embarcador.Cargas.TipoDeCargaCodigoIntegracao> query = SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.TipoDeCargaCodigoIntegracao>()
                .Where(obj => obj.TipoDeCarga.Codigo == codigoTipoDeCarga);

            if (query.Any(obj => obj.EtapaCarga == situacaoCarga))
                query = query.Where(obj => obj.EtapaCarga == situacaoCarga);
            else
                query = query.Where(obj => obj.EtapaCarga == Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoCarga.Todas || obj.EtapaCarga == null);

            return query
                .Select(obj => obj.CodigoIntegracao)
                .ToListAsync();
        }

        #endregion Métodos Públicos
    }
}
