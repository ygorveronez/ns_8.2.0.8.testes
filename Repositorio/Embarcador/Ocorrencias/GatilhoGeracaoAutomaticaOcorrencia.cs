using Dominio.ObjetosDeValor.Embarcador.Enumeradores;
using NHibernate.Linq;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Dynamic.Core;
using System.Threading;

namespace Repositorio.Embarcador.Ocorrencias
{
    public sealed class GatilhoGeracaoAutomaticaOcorrencia : RepositorioBase<Dominio.Entidades.Embarcador.Ocorrencias.GatilhoGeracaoAutomaticaOcorrencia>
    {
        #region Construtores

        public GatilhoGeracaoAutomaticaOcorrencia(UnitOfWork unitOfWork) : base(unitOfWork) { }
        public GatilhoGeracaoAutomaticaOcorrencia(UnitOfWork unitOfWork, CancellationToken cancellationToken) : base(unitOfWork, cancellationToken) { }

        #endregion

        #region Métodos Públicos

        public List<Dominio.Entidades.Embarcador.Ocorrencias.GatilhoGeracaoAutomaticaOcorrencia> BuscarPorGatilhoAutomaticoFinalFluxoPatio(EtapaFluxoGestaoPatio gatilhoFinalFluxoPatio, int codigoFilial, int codigoTransportador, int codigoTipoOperacao)
        {
            var query = FiltrarGatilhos(codigoFilial, codigoTransportador, codigoTipoOperacao);

            query = query.Where(o => o.Tipo == TipoGatilhoOcorrencia.FluxoPatio);
            query = query.Where(o => o.GatilhoFinalFluxoPatio == gatilhoFinalFluxoPatio);
            query = query.Where(o => o.GerarAutomaticamente);

            return query.ToList();
        }

        public List<Dominio.Entidades.Embarcador.Ocorrencias.GatilhoGeracaoAutomaticaOcorrencia> BuscarPorGatilhoAutomaticoFinalTracking(GatilhoFinalTraking gatilhoFinal, int codigoFilial, int codigoTransportador, int codigoTipoOperacao, Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoAplicacaoGatilhoTracking tipoAplicacaoGatilhoTracking)
        {
            var query = FiltrarGatilhos(codigoFilial, codigoTransportador, codigoTipoOperacao);

            query = query.Where(o => o.Tipo == TipoGatilhoOcorrencia.Tracking);
            query = query.Where(o => o.GatilhoFinalTraking == gatilhoFinal);
            query = query.Where(o => o.GerarAutomaticamente);
            query = query.Where(o => o.TipoAplicacaoGatilhoTracking == TipoAplicacaoGatilhoTracking.AplicarSempre || o.TipoAplicacaoGatilhoTracking == tipoAplicacaoGatilhoTracking);

            return query.ToList();
        }

        public List<Dominio.Entidades.Embarcador.Ocorrencias.GatilhoGeracaoAutomaticaOcorrencia> BuscarPorGatilhoAutomaticoAlteracaoData(TipoDataAlteracaoGatilho dataGatilho, int codigoFilial, int codigoTransportador, int codigoTipoOperacao)
        {
            var query = FiltrarGatilhos(codigoFilial, codigoTransportador, codigoTipoOperacao);

            query = query.Where(o => o.Tipo == TipoGatilhoOcorrencia.AlteracaoData);
            query = query.Where(o => o.TipoDataAlteracaoGatilho == dataGatilho);
            query = query.Where(o => o.GerarAutomaticamente);

            return query.ToList();
        }

        public List<Dominio.Entidades.Embarcador.Ocorrencias.GatilhoGeracaoAutomaticaOcorrencia> BuscarPorGatilhoDataTransportador( int codigoFilial, int codigoTransportador, int codigoTipoOperacao)
        {
            var query = FiltrarGatilhos(codigoFilial, codigoTransportador, codigoTipoOperacao);

            query = query.Where(o => o.AtribuirDataOcorrenciaNaDataAgendamentoTransportador);
            query = query.Where(o => o.GerarAutomaticamente);

            return query.ToList();
        }

        public List<Dominio.Entidades.Embarcador.Ocorrencias.GatilhoGeracaoAutomaticaOcorrencia> BuscarPorGatilhoAutomaticoAtingirData()
        {
            IQueryable<Dominio.Entidades.Embarcador.Ocorrencias.GatilhoGeracaoAutomaticaOcorrencia> queryGatilhoGeracaoAutomaticaOcorrencia = SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Ocorrencias.GatilhoGeracaoAutomaticaOcorrencia>()
                .Where(gatilhoGeracaoAutomaticaOcorrencia => gatilhoGeracaoAutomaticaOcorrencia.TipoOcorrencia.Ativo &&
                gatilhoGeracaoAutomaticaOcorrencia.Tipo == TipoGatilhoOcorrencia.AtingirData &&
                gatilhoGeracaoAutomaticaOcorrencia.GerarAutomaticamente);

            return queryGatilhoGeracaoAutomaticaOcorrencia
                .Fetch(gatilhoGeracaoAutomaticaOcorrencia => gatilhoGeracaoAutomaticaOcorrencia.Filiais)
                .Fetch(gatilhoGeracaoAutomaticaOcorrencia => gatilhoGeracaoAutomaticaOcorrencia.Transportadores)
                .Fetch(gatilhoGeracaoAutomaticaOcorrencia => gatilhoGeracaoAutomaticaOcorrencia.TiposOperacoes)
                .Fetch(gatilhoGeracaoAutomaticaOcorrencia => gatilhoGeracaoAutomaticaOcorrencia.TipoOcorrencia)
                .ToList();
        }

        public Dominio.Entidades.Embarcador.Ocorrencias.GatilhoGeracaoAutomaticaOcorrencia BuscarPorTipoOcorrencia(int codigoTipoOcorrencia)
        {
            var consultaGatilhoGeracaoAutomaticaOcorrencia = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Ocorrencias.GatilhoGeracaoAutomaticaOcorrencia>()
                .Where(o => o.TipoOcorrencia.Codigo == codigoTipoOcorrencia);

            return consultaGatilhoGeracaoAutomaticaOcorrencia.FirstOrDefault();
        }

        #endregion

        #region Métodos Privadp

        private IQueryable<Dominio.Entidades.Embarcador.Ocorrencias.GatilhoGeracaoAutomaticaOcorrencia> FiltrarGatilhos(int codigoFilial, int codigoTransportador, int codigoTipoOperacao)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Ocorrencias.GatilhoGeracaoAutomaticaOcorrencia>();

            query = query.Where(o => o.TipoOcorrencia.Ativo == true);

            if (codigoFilial > 0)
                query = query.Where(o => o.Filiais.Count == 0 || o.Filiais.Any(f => f.Codigo == codigoFilial));

            if (codigoTransportador > 0)
                query = query.Where(o => o.Transportadores.Count == 0 || o.Transportadores.Any(t => t.Codigo == codigoTransportador));

            if (codigoTipoOperacao > 0)
                query = query.Where(o => o.TiposOperacoes.Count == 0 || o.TiposOperacoes.Any(t => t.Codigo == codigoTipoOperacao));

            return query;
        }

        #endregion
    }
}
