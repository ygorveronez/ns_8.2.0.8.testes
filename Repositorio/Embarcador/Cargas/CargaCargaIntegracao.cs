using Dominio.ObjetosDeValor.Embarcador.Enumeradores;
using MongoDB.Driver;
using NHibernate.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Dynamic.Core;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Repositorio.Embarcador.Cargas
{
    public class CargaCargaIntegracao : RepositorioBase<Dominio.Entidades.Embarcador.Cargas.CargaCargaIntegracao>
    {
        #region Atributos Privados Somente Leitura

        private readonly bool _integracaoTransportador;

        #endregion

        #region Construtores

        public CargaCargaIntegracao(UnitOfWork unitOfWork) : this(unitOfWork, integracaoTransportador: false) { }

        public CargaCargaIntegracao(UnitOfWork unitOfWork, CancellationToken cancellationToken) : base(unitOfWork, cancellationToken) { }

        public CargaCargaIntegracao(UnitOfWork unitOfWork, bool integracaoTransportador) : base(unitOfWork)
        {
            _integracaoTransportador = integracaoTransportador;
        }

        #endregion

        #region Métodos Públicos

        public Dominio.Entidades.Embarcador.Cargas.CargaCargaIntegracao BuscarPorCodigo(int codigo)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.CargaCargaIntegracao>();

            var result = from obj in query where obj.Codigo == codigo select obj;

            return result.FirstOrDefault();
        }

        public bool ExistePorCarga(int codigoCarga, SituacaoIntegracao? situacao = null)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.CargaCargaIntegracao>();

            query = query.Where(x => x.Carga.Codigo == codigoCarga);

            if (situacao != null)
                query = query.Where(x => x.SituacaoIntegracao == situacao);

            return query.Any();
        }
        public bool ExistePorCargaETipo(int codigoCarga, Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoIntegracao tipoIntegracao)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.CargaCargaIntegracao>();

            query = query.Where(x => x.Carga.Codigo == codigoCarga && x.TipoIntegracao.Tipo == tipoIntegracao);

            return query.Any();
        }
        public Task<bool> ExistePorCargaETipoAsync(int codigoCarga, Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoIntegracao tipoIntegracao)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.CargaCargaIntegracao>();
            query = query.Where(x => x.Carga.Codigo == codigoCarga && x.TipoIntegracao.Tipo == tipoIntegracao);
            return query.AnyAsync();
        }
        public Task<bool> ExistePorCargaETipoAsync(int codigoCarga, List<Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoIntegracao> tiposIntegracao)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.CargaCargaIntegracao>();
            query = query.Where(x => x.Carga.Codigo == codigoCarga && tiposIntegracao.Contains(x.TipoIntegracao.Tipo));
            return query.AnyAsync();
        }

        public bool ExisteIntegradoOuAguardandoRetornoPorCargaETipo(int codigoCarga, Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoIntegracao tipoIntegracao)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.CargaCargaIntegracao>();

            query = query.Where(x => x.Carga.Codigo == codigoCarga && x.TipoIntegracao.Tipo == tipoIntegracao && (x.SituacaoIntegracao == SituacaoIntegracao.Integrado || x.SituacaoIntegracao == SituacaoIntegracao.AgRetorno));

            return query.Any();
        }

        public Dominio.Entidades.Embarcador.Cargas.CargaCargaIntegracao BuscarPorProtocoloETipo(string protocolo, int tipoIntegracao)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.CargaCargaIntegracao>();

            var result = from obj in query where obj.Protocolo == protocolo && obj.TipoIntegracao.Codigo == tipoIntegracao select obj;

            return result.FirstOrDefault();
        }

        public int ProximoNumeroSequencialMICDTA(string sigla, string licenca)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.CargaCargaIntegracao>()
                .Where(obj => obj.SiglaPaisOrigemMICDTA == sigla && obj.NumeroLicencaTNTIMICDTA == licenca);

            return (query.Max(c => (int?)c.NumeroSequencialMICDTA) ?? 1) + 1;
        }

        public int ContarPorCarga(int codigoCarga, SituacaoIntegracao situacao, bool integracaoFilialEmissora)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.CargaCargaIntegracao>();

            var result = from obj in query where obj.Carga.Codigo == codigoCarga && obj.SituacaoIntegracao == situacao && obj.IntegracaoFilialEmissora == integracaoFilialEmissora && obj.TipoIntegracao.IntegracaoTransportador == _integracaoTransportador select obj;

            return result.Count();
        }

        public int ContarEtapaTransportadorPorCarga(int codigoCarga, SituacaoIntegracao situacao)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.CargaCargaIntegracao>();

            var result = from obj in query where obj.TipoIntegracao.IntegracaoTransportador && obj.Carga.Codigo == codigoCarga && obj.SituacaoIntegracao == situacao select obj;

            return result.Count();
        }

        public int ContarEtapaTransportadorPorCargaDiferenteColeta(int codigoCarga, SituacaoIntegracao situacao)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.CargaCargaIntegracao>();

            var result = from obj in query where obj.TipoIntegracao.IntegracaoTransportador && obj.Carga.Codigo == codigoCarga && obj.SituacaoIntegracao == situacao && !obj.IntegracaoColeta select obj;

            return result.Count();
        }

        public List<Dominio.Entidades.Embarcador.Cargas.CargaCargaIntegracao> BuscarIntegracoesPendentes(int numeroTentativas, double minutosACadaTentativa, string propOrdenacao, string dirOrdenacao, int numeroRegistrosPorVez)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.CargaCargaIntegracao>();

            var result = from obj in query
                         where !obj.Carga.GerandoIntegracoes && obj.SituacaoIntegracao == SituacaoIntegracao.AgIntegracao ||
                               (obj.SituacaoIntegracao == SituacaoIntegracao.ProblemaIntegracao && obj.NumeroTentativas < numeroTentativas && obj.DataIntegracao <= DateTime.Now.AddMinutes(-minutosACadaTentativa))
                         select obj;

            return result.OrderBy(propOrdenacao + (dirOrdenacao == "asc" ? " ascending" : " descending")).Skip(0).Take(numeroRegistrosPorVez).ToList();
        }

        public List<Dominio.Entidades.Embarcador.Cargas.CargaCargaIntegracao> BuscarPendentesAgurdandoFinalizarCargaAnteriorPorMotorista(List<int> motoristas)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.CargaCargaIntegracao>();

            var result = from obj in query
                         where (obj.SituacaoIntegracao == SituacaoIntegracao.AgIntegracao || obj.SituacaoIntegracao == SituacaoIntegracao.ProblemaIntegracao) &&
                               (obj.AguardarFinalizarCargaAnterior) && obj.Carga.Motoristas.Any(mot => motoristas.Contains(mot.Codigo)) &&
                               obj.CargaPendente == null
                         select obj;

            return result.ToList();
        }

        public List<Dominio.Entidades.Embarcador.Cargas.CargaCargaIntegracao> BuscarPendentesAguardandoFinalizarCargaAnterior(int cargaFinalizada)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.CargaCargaIntegracao>();

            var result = from obj in query
                         where (obj.SituacaoIntegracao == SituacaoIntegracao.ProblemaIntegracao) &&
                               obj.AguardarFinalizarCargaAnterior && (obj.CargaPendente.Codigo == cargaFinalizada)
                         select obj;

            return result.OrderBy(i => i.Carga.DataCriacaoCarga).ToList();
        }


        public Task<List<int>> BuscarIntegracoesPendentesDiferentesDeTipoAsync(int numeroTentativas, double minutosACadaTentativa, string propOrdenacao, string dirOrdenacao, int numeroRegistrosPorVez, Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoIntegracao tipo)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.CargaCargaIntegracao>();

            List<SituacaoIntegracao> situacoesValidas = new List<SituacaoIntegracao>() { SituacaoIntegracao.AgIntegracao, SituacaoIntegracao.AgRetorno };

            var result = from obj in query
                         where !obj.Carga.GerandoIntegracoes && obj.TipoIntegracao.Tipo != tipo &&
                               (situacoesValidas.Contains(obj.SituacaoIntegracao) ||
                               (obj.SituacaoIntegracao == SituacaoIntegracao.ProblemaIntegracao && obj.NumeroTentativas < numeroTentativas && obj.DataIntegracao <= DateTime.Now.AddMinutes(-minutosACadaTentativa)))
                         select obj;

            return result.OrderBy(propOrdenacao + (dirOrdenacao == "asc" ? " ascending" : " descending")).Skip(0).Take(numeroRegistrosPorVez).Select(x => x.Codigo).ToListAsync(CancellationToken);
        }

        public Task<List<int>> BuscarIntegracoesPendentesPorTipoIntegracaoAsync(bool cargasEmLote, int numeroTentativas, double minutosACadaTentativa, string propOrdenacao, string dirOrdenacao, int numeroRegistrosPorVez, Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoIntegracao tipo)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.CargaCargaIntegracao>();

            var result = from obj in query
                         where !obj.Carga.GerandoIntegracoes && obj.TipoIntegracao.Tipo == tipo &&
                               (obj.SituacaoIntegracao == SituacaoIntegracao.AgIntegracao ||
                               (obj.SituacaoIntegracao == SituacaoIntegracao.ProblemaIntegracao && obj.NumeroTentativas < numeroTentativas && obj.DataIntegracao <= DateTime.Now.AddMinutes(-minutosACadaTentativa)))
                         select obj;

            if (!cargasEmLote)
                result = result.Where(o => o.Carga.CalcularFreteLote == null || o.Carga.CalcularFreteLote == Dominio.Enumeradores.LoteCalculoFrete.Padrao);
            else
                result = result.Where(o => o.Carga.CalcularFreteLote == Dominio.Enumeradores.LoteCalculoFrete.Integracao || o.Carga.CalcularFreteLote == Dominio.Enumeradores.LoteCalculoFrete.Reprocessamento);

            return result.OrderBy(propOrdenacao + (dirOrdenacao == "asc" ? " ascending" : " descending")).Skip(0).Take(numeroRegistrosPorVez).Select(x => x.Codigo).ToListAsync(CancellationToken);
        }

        public Task<List<int>> BuscarIntegracoesConcluidasPorTipoIntegracaoAsync(bool cargasEmLote, SituacaoCarga situacao, string propOrdenacao, string dirOrdenacao, int numeroRegistrosPorVez, Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoIntegracao tipo)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.CargaCargaIntegracao>();

            var result = from obj in query
                         where !obj.Carga.GerandoIntegracoes &&
                                obj.TipoIntegracao.Tipo == tipo &&
                                obj.Carga.SituacaoCarga == situacao &&
                                (obj.SituacaoIntegracao == SituacaoIntegracao.Integrado || (obj.SituacaoIntegracao == SituacaoIntegracao.ProblemaIntegracao && !obj.Carga.PossuiPendencia))
                         select obj;

            if (!cargasEmLote)
                result = result.Where(o => o.Carga.CalcularFreteLote == null || o.Carga.CalcularFreteLote == Dominio.Enumeradores.LoteCalculoFrete.Padrao);
            else
                result = result.Where(o => o.Carga.CalcularFreteLote == Dominio.Enumeradores.LoteCalculoFrete.Integracao);

            return result.OrderBy(propOrdenacao + (dirOrdenacao == "asc" ? " ascending" : " descending")).Skip(0).Take(numeroRegistrosPorVez).Select(x => x.Carga.Codigo).ToListAsync(CancellationToken);
        }

        public List<Dominio.Entidades.Embarcador.Cargas.CargaCargaIntegracao> BuscarCargaCargaIntegracaoAguardando(int quantidadeRegistros)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.CargaCargaIntegracao>();

            var result = from obj in query
                         where !obj.Carga.GerandoIntegracoes &&
                                obj.SituacaoIntegracao == SituacaoIntegracao.AgRetorno
                                && obj.DataIntegracao <= DateTime.Now.AddMinutes(obj.TipoIntegracao.TempoConsultaIntegracao)
                         select obj;
            return result.Take(quantidadeRegistros).ToList();
        }

        public List<int> BuscarCodigoIntegracoesPendentes(DateTime dataInicio, DateTime dataFim, int codigoFilial, int numeroTentativas, double minutosACadaTentativa, string propOrdenacao, string dirOrdenacao, int numeroRegistrosPorVez)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.CargaCargaIntegracao>();

            var result = from obj in query
                         where !obj.Carga.GerandoIntegracoes && obj.SituacaoIntegracao == SituacaoIntegracao.AgIntegracao ||
                               (obj.SituacaoIntegracao == SituacaoIntegracao.ProblemaIntegracao && obj.NumeroTentativas < numeroTentativas && obj.DataIntegracao <= DateTime.Now.AddMinutes(-minutosACadaTentativa))
                         select obj;

            if (dataInicio > DateTime.MinValue)
                result = result.Where(o => o.Carga.DataCriacaoCarga >= dataInicio);

            if (dataFim > DateTime.MinValue)
                result = result.Where(o => o.Carga.DataCriacaoCarga <= dataFim);

            if (codigoFilial > 0)
                result = result.Where(o => o.Carga.Filial.Codigo == codigoFilial);

            return result.OrderBy(propOrdenacao + (dirOrdenacao == "asc" ? " ascending" : " descending")).Skip(0).Take(numeroRegistrosPorVez).Select(o => o.Codigo).Distinct().ToList();
        }

        public Task<List<Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoIntegracao>> BuscarTipoIntegracaoPorCargaAsync(int codigoCarga, bool integracaoFilialEmissora)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.CargaCargaIntegracao>();

            var result = from obj in query where obj.Carga.Codigo == codigoCarga && obj.IntegracaoFilialEmissora == integracaoFilialEmissora && obj.TipoIntegracao.IntegracaoTransportador == _integracaoTransportador select obj.TipoIntegracao.Tipo;

            return result.Distinct().ToListAsync(CancellationToken);
        }

        public List<Dominio.Entidades.Embarcador.Cargas.CargaCargaIntegracao> Consultar(int codigoCarga, SituacaoIntegracao? situacao, Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoIntegracao? tipo, string propOrdena, string dirOrdena, int inicio, int limite, bool integracaoFilialEmissora)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.CargaCargaIntegracao>();

            var result = from obj in query where obj.TipoIntegracao.IntegracaoTransportador == _integracaoTransportador && obj.IntegracaoFilialEmissora == integracaoFilialEmissora select obj;

            if (codigoCarga > 0)
                result = result.Where(o => o.Carga.Codigo == codigoCarga);

            if (situacao.HasValue)
                result = result.Where(o => o.SituacaoIntegracao == situacao);

            if (tipo.HasValue)
                result = result.Where(o => o.TipoIntegracao.Tipo == tipo);

            return result.OrderBy(propOrdena + (dirOrdena == "asc" ? " ascending" : " descending")).Skip(inicio).Take(limite)
                .Fetch(obj => obj.TipoIntegracao)
                .ToList();
        }

        public int ContarConsulta(int codigoCarga, SituacaoIntegracao? situacao, Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoIntegracao? tipo, bool integracaoFilialEmissora)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.CargaCargaIntegracao>();

            var result = from obj in query where obj.TipoIntegracao.IntegracaoTransportador == _integracaoTransportador && obj.IntegracaoFilialEmissora == integracaoFilialEmissora select obj;

            if (codigoCarga > 0)
                result = result.Where(o => o.Carga.Codigo == codigoCarga);

            if (situacao.HasValue)
                result = result.Where(o => o.SituacaoIntegracao == situacao);

            if (tipo.HasValue)
                result = result.Where(o => o.TipoIntegracao.Tipo == tipo);

            return result.Count();
        }

        public int ContarPorCarga(int codigoCarga)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.CargaCargaIntegracao>();

            var result = from obj in query where obj.Carga.Codigo == codigoCarga && obj.TipoIntegracao.IntegracaoTransportador == _integracaoTransportador select obj;

            return result.Count();
        }

        public int ContarPorCargaETipoIntegracao(int codigoCarga, int codigoTipoIntegracao)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.CargaCargaIntegracao>();

            var result = from obj in query where obj.Carga.Codigo == codigoCarga && obj.TipoIntegracao.Codigo == codigoTipoIntegracao && obj.TipoIntegracao.IntegracaoTransportador == _integracaoTransportador select obj;

            return result.Count();
        }

        public Dominio.Entidades.Embarcador.Cargas.CargaCargaIntegracao BuscarPorCargaETipoIntegracao(int codigoCarga, int codigoTipoIntegracao)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.CargaCargaIntegracao>();

            var result = from obj in query where obj.Carga.Codigo == codigoCarga && obj.TipoIntegracao.Codigo == codigoTipoIntegracao && obj.TipoIntegracao.IntegracaoTransportador == _integracaoTransportador select obj;

            return result.FirstOrDefault();
        }
        public Task<Dominio.Entidades.Embarcador.Cargas.CargaCargaIntegracao> BuscarPorCargaETipoIntegracaoAsync(int codigoCarga, int codigoTipoIntegracao)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.CargaCargaIntegracao>();
            query = query.Where(obj => obj.Carga.Codigo == codigoCarga && obj.TipoIntegracao.Codigo == codigoTipoIntegracao && obj.TipoIntegracao.IntegracaoTransportador == _integracaoTransportador);
            return query.FirstOrDefaultAsync();
        }

        public Dominio.Entidades.Embarcador.Cargas.CargaCargaIntegracao BuscarPorCargaETipoIntegracao(int codigoCarga, Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoIntegracao tipoIntegracao)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.CargaCargaIntegracao>();

            var result = from obj in query where obj.Carga.Codigo == codigoCarga && obj.TipoIntegracao.Tipo == tipoIntegracao select obj;

            return result.FirstOrDefault();
        }

        public Task<List<Dominio.Entidades.Embarcador.Cargas.CargaCargaIntegracao>> BuscarPorCargaETipoIntegracoesAsync(int codigoCarga, List<Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoIntegracao> tipoIntegracao)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.CargaCargaIntegracao>();

            var result = from obj in query where obj.Carga.Codigo == codigoCarga && tipoIntegracao.Contains(obj.TipoIntegracao.Tipo) select obj;

            return result.ToListAsync();
        }

        public Dominio.Entidades.Embarcador.Cargas.CargaCargaIntegracao BuscarPorCargaETipoIntegracao(int codigoCarga, int codigoTipoIntegracao, bool integracaoColeta)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.CargaCargaIntegracao>();

            var result = from obj in query where obj.Carga.Codigo == codigoCarga && obj.TipoIntegracao.Codigo == codigoTipoIntegracao && obj.TipoIntegracao.IntegracaoTransportador == _integracaoTransportador && obj.IntegracaoColeta == integracaoColeta select obj;

            return result.FirstOrDefault();
        }

        public Dominio.Entidades.Embarcador.Cargas.CargaCargaIntegracao BuscarPorCargaTipoIntegracaoColetaSituacao(int codigoCarga, Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoIntegracao tipoIntegracao, bool coleta, SituacaoIntegracao situacaoIntegracao)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.CargaCargaIntegracao>();

            var result = from obj in query where obj.Carga.Codigo == codigoCarga && obj.TipoIntegracao.Tipo == tipoIntegracao && obj.IntegracaoColeta == coleta && obj.SituacaoIntegracao == situacaoIntegracao select obj;

            return result.FirstOrDefault();
        }

        public int ContarPorCargaESituacaoDiff(int codigoCarga, SituacaoIntegracao situacaoDiff)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.CargaCargaIntegracao>();

            var result = from obj in query where obj.Carga.Codigo == codigoCarga && obj.SituacaoIntegracao != situacaoDiff && obj.TipoIntegracao.IntegracaoTransportador == _integracaoTransportador select obj;

            return result.Count();
        }

        public int ContarPorCarga(int codigoCarga, SituacaoIntegracao[] situacao)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.CargaCargaIntegracao>();

            var result = from obj in query where obj.Carga.Codigo == codigoCarga && situacao.Contains(obj.SituacaoIntegracao) && obj.TipoIntegracao.IntegracaoTransportador == _integracaoTransportador select obj;

            return result.Count();
        }

        public Dominio.Entidades.Embarcador.Cargas.CargaCargaIntegracao BuscarPorCodigoArquivo(int codigoArquivo)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.CargaCargaIntegracao>();

            var result = from obj in query where obj.ArquivosTransacao.Any(o => o.Codigo == codigoArquivo) select obj;

            return result.FirstOrDefault();
        }

        public List<Dominio.Entidades.Embarcador.Cargas.CargaCargaIntegracao> BuscarPorCarga(int codigoCarga)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.CargaCargaIntegracao>();

            var result = from obj in query where obj.Carga.Codigo == codigoCarga && obj.TipoIntegracao.IntegracaoTransportador == _integracaoTransportador select obj;

            return result.ToList();
        }

        public List<Dominio.Entidades.Embarcador.Cargas.CargaCargaIntegracao> BuscarPorCarga(int codigoCarga, SituacaoIntegracao? situacao, Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoIntegracao? tipo)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.CargaCargaIntegracao>();

            var result = from obj in query where obj.Carga.Codigo == codigoCarga && obj.TipoIntegracao.IntegracaoTransportador == _integracaoTransportador select obj;

            if (situacao.HasValue)
                result = result.Where(o => o.SituacaoIntegracao == situacao.Value);

            if (tipo.HasValue)
                result = result.Where(o => o.TipoIntegracao.Tipo == tipo.Value);

            return result.ToList();
        }

        public bool ExisteIntegracaoParaEstaStage(int codigoStage)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.CargaCargaIntegracao>();

            var result = from obj in query where obj.Stage.Codigo == codigoStage select obj;

            return result.Any();
        }

        public bool ExisteProtocoloPorCargaETipoIntegracao(int codigoCarga, Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoIntegracao tipoIntegracao)
        {
            var consultaIntegracao = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.CargaCargaIntegracao>()
                .Where(integracao => integracao.Carga.Codigo == codigoCarga && integracao.TipoIntegracao.Tipo == tipoIntegracao && integracao.Protocolo != null && integracao.Protocolo != "");

            return consultaIntegracao.Any();
        }

        public string BuscarProtocoloPorCargaETipoIntegracao(int codigoCarga, Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoIntegracao tipoIntegracao)
        {
            var consultaIntegracao = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.CargaCargaIntegracao>()
                .Where(integracao => integracao.Carga.Codigo == codigoCarga && integracao.TipoIntegracao.Tipo == tipoIntegracao);

            return consultaIntegracao.Select(integracao => integracao.Protocolo).FirstOrDefault();
        }

        public List<Dominio.Entidades.Embarcador.Cargas.CargaCargaIntegracao> BuscarRegistrosComRetornoPendente(Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoIntegracao? tipo)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.CargaCargaIntegracao>();

            var result = from obj in query where obj.PendenteRetorno select obj;

            if (tipo.HasValue)
                result = result.Where(o => o.TipoIntegracao.Tipo == tipo.Value);

            return result.ToList();
        }

        public Task<int> ContarConsultaGridIntegracoesFalhaAsync(Dominio.ObjetosDeValor.Embarcador.Carga.IntegracoesComFalha.FiltroPesquisaIntegracoesComFalha filtro)
        {
            var sql = QueryIntegracoesComFalha(filtro, true);
            var consulta = this.SessionNHiBernate.CreateSQLQuery(sql);

            return consulta.SetTimeout(600).UniqueResultAsync<int>(CancellationToken);
        }

        public Task<IList<Dominio.Relatorios.Embarcador.DataSource.Cargas.GestaoIntegracoesComFalha.IntegracoesComFalha>> PreencherGridIntegracoesFalhaAsync(Dominio.ObjetosDeValor.Embarcador.Carga.IntegracoesComFalha.FiltroPesquisaIntegracoesComFalha filtro, Dominio.ObjetosDeValor.Embarcador.Consulta.ParametroConsulta parametrosConsulta)
        {
            var sql = QueryIntegracoesComFalha(filtro, false, parametrosConsulta);
            var consulta = this.SessionNHiBernate.CreateSQLQuery(sql);

            consulta.SetResultTransformer(new NHibernate.Transform.AliasToBeanResultTransformer(typeof(Dominio.Relatorios.Embarcador.DataSource.Cargas.GestaoIntegracoesComFalha.IntegracoesComFalha)));

            return consulta.ListAsync<Dominio.Relatorios.Embarcador.DataSource.Cargas.GestaoIntegracoesComFalha.IntegracoesComFalha>(CancellationToken);
        }

        private string QueryIntegracoesComFalha(Dominio.ObjetosDeValor.Embarcador.Carga.IntegracoesComFalha.FiltroPesquisaIntegracoesComFalha filtro, bool somenteContarNumeroRegistros, Dominio.ObjetosDeValor.Embarcador.Consulta.ParametroConsulta parametrosConsulta = null)
        {
            string pattern = "yyyy-MM-dd";
            StringBuilder sql = new();

            sql.Append(@"WITH IntegracoesFalha AS (
                    SELECT 'CDI' AS TabelaOrigem,
                    CDI_CODIGO AS Codigo,
                    I.CAR_CODIGO AS CodigoCarga,
                    CAR_CODIGO_CARGA_EMBARCADOR AS NumeroCarga,
                    CAR_SITUACAO AS EtapaCarga,
                    INT_SITUACAO_INTEGRACAO AS SituacaoIntegracao,
                    TPI_TIPO AS TipoIntegracao,
                    INT_DATA_INTEGRACAO AS DataIntegracao,
                    INT_PROBLEMA_INTEGRACAO AS MensagemRetorno
                    from T_CARGA_DADOS_TRANSPORTE_INTEGRACAO I left join T_CARGA C ON I.CAR_CODIGO = C.CAR_CODIGO 
                    left join T_TIPO_INTEGRACAO T ON T.TPI_CODIGO = I.TPI_CODIGO
                    WHERE INT_SITUACAO_INTEGRACAO = 2

                    UNION ALL

                    SELECT 'CFI' AS TabelaOrigem,
                    CFI_CODIGO AS Codigo,
                    I.CAR_CODIGO AS CodigoCarga,
                    CAR_CODIGO_CARGA_EMBARCADOR AS NumeroCarga,
                    CAR_SITUACAO AS EtapaCarga,
                    INT_SITUACAO_INTEGRACAO AS SituacaoIntegracao,
                    TPI_TIPO AS TipoIntegracao,
                    INT_DATA_INTEGRACAO AS DataIntegracao,
                    INT_PROBLEMA_INTEGRACAO AS MensagemRetorno
                    from T_CARGA_FRETE_INTEGRACAO I left join T_CARGA C ON I.CAR_CODIGO = C.CAR_CODIGO 
                    left join T_TIPO_INTEGRACAO T ON T.TPI_CODIGO = I.TPI_CODIGO
                    WHERE INT_SITUACAO_INTEGRACAO = 2


                    UNION ALL

                    SELECT 'CAI' AS TabelaOrigem,
                    CAI_CODIGO AS Codigo,
                    I.CAR_CODIGO AS CodigoCarga,
                    CAR_CODIGO_CARGA_EMBARCADOR AS NumeroCarga, 
                    CAR_SITUACAO AS EtapaCarga, 
                    INT_SITUACAO_INTEGRACAO AS SituacaoIntegracao, 
                    TPI_TIPO AS TipoIntegracao,
                    INT_DATA_INTEGRACAO AS DataIntegracao,
                    INT_PROBLEMA_INTEGRACAO AS MensagemRetorno
                    from T_CARGA_CARGA_INTEGRACAO I left join T_CARGA C ON I.CAR_CODIGO = C.CAR_CODIGO
                    left join T_TIPO_INTEGRACAO T ON T.TPI_CODIGO = I.TPI_CODIGO
                    WHERE INT_SITUACAO_INTEGRACAO = 2
                    )");

            if (somenteContarNumeroRegistros)
                sql.Append(@" SELECT DISTINCT(count(0) over ()) FROM IntegracoesFalha WHERE(1 = 1)");

            else
                sql.Append(@" SELECT * FROM IntegracoesFalha WHERE(1 = 1)");

            if (!filtro.CodigosCarga.IsNullOrEmpty())
                sql.Append($@" AND CodigoCarga IN ({string.Join(",", filtro.CodigosCarga)})");

            if (filtro.DataInicial.HasValue && filtro.DataInicial > DateTime.MinValue)
                sql.Append($@" AND DataIntegracao >= '{filtro.DataInicial.Value.ToString(pattern)}'");

            if (filtro.DataFim.HasValue && filtro.DataFim > DateTime.MinValue)
                sql.Append($@" AND DataIntegracao < '{filtro.DataFim.Value.AddDays(1).ToString(pattern)}'");

            if (filtro.EtapaCarga.HasValue && filtro.EtapaCarga != SituacaoCarga.Todas)
                sql.Append($@" AND EtapaCarga = {(int)filtro.EtapaCarga.Value}");

            if (filtro.TipoIntegracao.HasValue && filtro.TipoIntegracao != Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoIntegracao.NaoInformada)
                sql.Append($@" AND TipoIntegracao = {(int)filtro.TipoIntegracao.Value}");

            if (!somenteContarNumeroRegistros && !string.IsNullOrWhiteSpace(parametrosConsulta?.PropriedadeOrdenar))
            {
                sql.Append($" ORDER BY {parametrosConsulta.PropriedadeOrdenar} {parametrosConsulta.DirecaoOrdenar}");

                if ((parametrosConsulta.InicioRegistros > 0) || (parametrosConsulta.LimiteRegistros > 0))
                    sql.Append($" OFFSET {parametrosConsulta.InicioRegistros} ROWS FETCH NEXT {parametrosConsulta.LimiteRegistros} ROWS ONLY;");
            }

            return sql.ToString();
        }

        #endregion Métodos Públicos
    }
}
