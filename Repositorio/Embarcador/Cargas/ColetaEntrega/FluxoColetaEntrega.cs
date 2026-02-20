using NHibernate.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Dynamic.Core;
using System.Threading;
using System.Threading.Tasks;

namespace Repositorio.Embarcador.Cargas.ColetaEntrega
{
    public class FluxoColetaEntrega : RepositorioBase<Dominio.Entidades.Embarcador.Cargas.ColetaEntrega.FluxoColetaEntrega>
    {
        public FluxoColetaEntrega(UnitOfWork unitOfWork) : base(unitOfWork) { }
        public FluxoColetaEntrega(UnitOfWork unitOfWork, CancellationToken cancellationToken) : base(unitOfWork, cancellationToken) { }

        public Dominio.Entidades.Embarcador.Cargas.ColetaEntrega.FluxoColetaEntrega BuscarPorCarga(int carga)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.ColetaEntrega.FluxoColetaEntrega>();
            var result = from obj in query where obj.Carga.Codigo == carga select obj;
            return result.FirstOrDefault();
        }

        private IQueryable<Dominio.Entidades.Embarcador.Cargas.ColetaEntrega.FluxoColetaEntrega> _Consultar(Dominio.ObjetosDeValor.Embarcador.Enumeradores.EtapaFluxoColetaEntrega etapaFluxoColetaEntrega, string numeroCarga, DateTime dataInicial, DateTime dataFinal, int filial, bool exibirCargasCanceladas)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.ColetaEntrega.FluxoColetaEntrega>();

            var result = from obj in query select obj;

            if (!string.IsNullOrWhiteSpace(numeroCarga))
                result = result.Where(o => o.Carga.CodigoCargaEmbarcador == numeroCarga);

            if (etapaFluxoColetaEntrega != Dominio.ObjetosDeValor.Embarcador.Enumeradores.EtapaFluxoColetaEntrega.Todas)
                result = result.Where(o => o.EtapaFluxoColetaEntregaEtapaAtual == etapaFluxoColetaEntrega);

            // Filtros
            if (dataInicial != DateTime.MinValue)
                result = result.Where(o => o.Carga.DataCarregamentoCarga.Value.Date >= dataInicial);

            if (dataFinal != DateTime.MinValue)
                result = result.Where(o => o.Carga.DataCarregamentoCarga.Value.Date < dataFinal.AddDays(1));

            if (!exibirCargasCanceladas)
                result = result.Where(o => o.Carga.SituacaoCarga != Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoCarga.Cancelada && o.Carga.SituacaoCarga != Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoCarga.Anulada);

            if (filial > 0)
                result = result.Where(o => o.Carga.Filial.Codigo == filial);

            return result;
        }

        public Task<List<Dominio.Entidades.Embarcador.Cargas.ColetaEntrega.FluxoColetaEntrega>> ConsultarAsync(Dominio.ObjetosDeValor.Embarcador.Enumeradores.EtapaFluxoColetaEntrega etapaFluxoColetaEntrega, string numeroCarga, DateTime dataInicial, DateTime dataFinal, int filial, bool exibirCargasCanceladas, string propOrdena, string dirOrdena, int inicioRegistros, int maximoRegistros)
        {
            var result = _Consultar(etapaFluxoColetaEntrega, numeroCarga, dataInicial, dataFinal, filial, exibirCargasCanceladas);

            if (!string.IsNullOrWhiteSpace(propOrdena))
                result = result.OrderBy(propOrdena + (dirOrdena == "asc" ? " ascending" : " descending"));

            if (inicioRegistros > 0)
                result = result.Skip(inicioRegistros);

            if (maximoRegistros > 0)
                result = result.Take(maximoRegistros);

            return result
                .Fetch(obj => obj.Carga)
                .ThenFetch(obj => obj.Filial)
                .Fetch(obj => obj.Carga)
                .ThenFetch(obj => obj.DadosSumarizados)
                .ToListAsync(CancellationToken);
        }

        public Task<int> ContarConsultaAsync(Dominio.ObjetosDeValor.Embarcador.Enumeradores.EtapaFluxoColetaEntrega etapaFluxoColetaEntrega, string numeroCarga, DateTime dataInicial, DateTime dataFinal, int filial, bool exibirCargasCanceladas)
        {
            var result = _Consultar(etapaFluxoColetaEntrega, numeroCarga, dataInicial, dataFinal, filial, exibirCargasCanceladas);

            return result.CountAsync(CancellationToken);
        }
    }
}
