using NHibernate.Linq;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Repositorio.Embarcador.PainelNFeTransporte
{
    public class CargaNotasPendentesIntegracaoMercadoLivre : RepositorioBase<Dominio.Entidades.Embarcador.PainelNFeTransportador.CargaNotasPendentesIntegracaoMercadoLivre>
    {
        public CargaNotasPendentesIntegracaoMercadoLivre(UnitOfWork unitOfWork) : base(unitOfWork) { }

        public CargaNotasPendentesIntegracaoMercadoLivre(UnitOfWork unitOfWork, CancellationToken cancellationToken) : base(unitOfWork, cancellationToken) { }

        public async Task<List<Dominio.Entidades.Embarcador.PainelNFeTransportador.CargaNotasPendentesIntegracaoMercadoLivre>> BuscarPorCargaSituacaoAsync(int codigoCarga, Dominio.ObjetosDeValor.Embarcador.Enumeradores.EnumSituacaoNotasPendetesIntegracaoMercadoLivre situacao)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.PainelNFeTransportador.CargaNotasPendentesIntegracaoMercadoLivre>();

            if (situacao != Dominio.ObjetosDeValor.Embarcador.Enumeradores.EnumSituacaoNotasPendetesIntegracaoMercadoLivre.Todas)
                query = query.Where(o => o.SituacaoDownloadNotas == situacao);

            if (codigoCarga > 0) 
                query = query.Where(o => o.Carga.Codigo == codigoCarga);

            return await query.ToListAsync();
        }

        public async Task<bool> ExisteCargaParaDownloadDeNotaAsync(int codigoCarga)
        {
            Dominio.Entidades.Embarcador.PainelNFeTransportador.CargaNotasPendentesIntegracaoMercadoLivre cargaNotasPendentesIntegracaoMercadoLivre = new Dominio.Entidades.Embarcador.PainelNFeTransportador.CargaNotasPendentesIntegracaoMercadoLivre();
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.PainelNFeTransportador.CargaNotasPendentesIntegracaoMercadoLivre>();

            if (codigoCarga > 0)
                query = query.Where(o => o.Carga.Codigo == codigoCarga);

            cargaNotasPendentesIntegracaoMercadoLivre = await query.FirstOrDefaultAsync();

            return cargaNotasPendentesIntegracaoMercadoLivre?.Codigo > 0;
        }

        public async Task<List<Dominio.Entidades.Embarcador.PainelNFeTransportador.CargaNotasPendentesIntegracaoMercadoLivre>> BuscarPorSituacaoPendenteDownloadAsync()
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.PainelNFeTransportador.CargaNotasPendentesIntegracaoMercadoLivre>();
            query = query.Where(o => o.SituacaoDownloadNotas == Dominio.ObjetosDeValor.Embarcador.Enumeradores.EnumSituacaoNotasPendetesIntegracaoMercadoLivre.Pendente);
            return await query.ToListAsync();
        }
    }
}
