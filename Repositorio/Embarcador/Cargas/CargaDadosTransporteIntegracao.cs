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
    public class CargaDadosTransporteIntegracao : RepositorioBase<Dominio.Entidades.Embarcador.Cargas.CargaDadosTransporteIntegracao>
    {
        #region Construtores

        public CargaDadosTransporteIntegracao(UnitOfWork unitOfWork) : base(unitOfWork) { }
        public CargaDadosTransporteIntegracao(UnitOfWork unitOfWork, CancellationToken cancellationToken) : base(unitOfWork, cancellationToken) { }

        #endregion

        #region Métodos Públicos

        public Dominio.Entidades.Embarcador.Cargas.CargaDadosTransporteIntegracao BuscarPorCodigo(int codigo)
        {
            IQueryable<Dominio.Entidades.Embarcador.Cargas.CargaDadosTransporteIntegracao> query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.CargaDadosTransporteIntegracao>();

            query = query.Where(o => o.Codigo == codigo);

            return query.FirstOrDefault();
        }

        public Task<Dominio.Entidades.Embarcador.Cargas.CargaDadosTransporteIntegracao> BuscarPorCodigoAsync(int codigo)
        {
            IQueryable<Dominio.Entidades.Embarcador.Cargas.CargaDadosTransporteIntegracao> query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.CargaDadosTransporteIntegracao>();

            query = query.Where(o => o.Codigo == codigo);

            return query.FirstOrDefaultAsync(CancellationToken);
        }

        public bool ExistePorProtocoloECarga(string protocolo, int carga)
        {
            IQueryable<Dominio.Entidades.Embarcador.Cargas.CargaDadosTransporteIntegracao> query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.CargaDadosTransporteIntegracao>();

            query = query.Where(obj => obj.Protocolo == protocolo && obj.Carga.Codigo == carga);

            return query.Any();
        }


        public Dominio.Entidades.Embarcador.Cargas.TipoIntegracao ExisteComProtocoloPorCargaETipoIntegracao(int carga, Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoIntegracao enumTipoIntegracao)
        {
            IQueryable<Dominio.Entidades.Embarcador.Cargas.CargaDadosTransporteIntegracao> query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.CargaDadosTransporteIntegracao>();

            query = query.Where(obj => obj.TipoIntegracao.Tipo == enumTipoIntegracao && obj.Carga.Codigo == carga && ((obj.PreProtocolo != null && obj.PreProtocolo != "") || (obj.Protocolo != null && obj.Protocolo != "")));

            return query.Select(t => t.TipoIntegracao).FirstOrDefault();
        }


        public Dominio.Entidades.Embarcador.Cargas.CargaDadosTransporteIntegracao ExisteComProtocoloPorCargaETipoIntegracaoRetornoCargaDadosTransporte(int carga, Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoIntegracao enumTipoIntegracao)
        {
            IQueryable<Dominio.Entidades.Embarcador.Cargas.CargaDadosTransporteIntegracao> query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.CargaDadosTransporteIntegracao>();

            query = query.Where(obj => obj.TipoIntegracao.Tipo == enumTipoIntegracao && obj.Carga.Codigo == carga && ((obj.PreProtocolo != null && obj.PreProtocolo != "") || (obj.Protocolo != null && obj.Protocolo != "")));

            return query.FirstOrDefault();
        }

        public Dominio.Entidades.Embarcador.Cargas.TipoIntegracao ExistePorCargaTipoIntegracaoESituacao(int carga, Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoIntegracao enumTipoIntegracao, Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoIntegracao situacaoIntegracao)
        {
            IQueryable<Dominio.Entidades.Embarcador.Cargas.CargaDadosTransporteIntegracao> query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.CargaDadosTransporteIntegracao>();

            query = query.Where(obj => obj.TipoIntegracao.Tipo == enumTipoIntegracao && obj.Carga.Codigo == carga && obj.SituacaoIntegracao == situacaoIntegracao);

            return query.Select(t => t.TipoIntegracao).FirstOrDefault();
        }

        public int ContarPorCarga(int codigoCarga, Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoIntegracao situacao)
        {
            IQueryable<Dominio.Entidades.Embarcador.Cargas.CargaDadosTransporteIntegracao> query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.CargaDadosTransporteIntegracao>();

            query = query.Where(obj => obj.Carga.Codigo == codigoCarga && obj.SituacaoIntegracao == situacao);

            return query.Count();
        }

        public bool ExistePorCargaETipoIntegracao(int codigoCarga, Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoIntegracao tipo)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.CargaDadosTransporteIntegracao>();

            query = query.Where(obj => obj.Carga.Codigo == codigoCarga && obj.TipoIntegracao.Tipo == tipo);

            return query.Any();
        }

        public int ContarEtapaTransportadorPorCarga(int codigoCarga, Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoIntegracao situacao)
        {
            IQueryable<Dominio.Entidades.Embarcador.Cargas.CargaDadosTransporteIntegracao> query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.CargaDadosTransporteIntegracao>();

            query = query.Where(o => o.Carga.Codigo == codigoCarga && o.SituacaoIntegracao == situacao);

            return query.Count();
        }

        public int ContarEtapaTransportadorPorCargaDiferenteColeta(int codigoCarga, Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoIntegracao situacao)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.CargaDadosTransporteIntegracao>();

            var result = from obj in query where obj.TipoIntegracao.IntegracaoTransportador && obj.Carga.Codigo == codigoCarga && obj.SituacaoIntegracao == situacao && !obj.IntegracaoColeta select obj;

            return result.Count();
        }

        public Task<List<int>> BuscarIntegracoesPendentesAsync(int numeroTentativas, double minutosACadaTentativa, string propOrdenacao, string dirOrdenacao, int numeroRegistrosPorVez)
        {
            IQueryable<Dominio.Entidades.Embarcador.Cargas.CargaDadosTransporteIntegracao> query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.CargaDadosTransporteIntegracao>()
                .Where(o =>
                    (o.SituacaoIntegracao == Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoIntegracao.AgIntegracao) ||
                    (o.SituacaoIntegracao == Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoIntegracao.ProblemaIntegracao && o.NumeroTentativas < numeroTentativas && o.DataIntegracao <= DateTime.Now.AddMinutes(-minutosACadaTentativa)) &&
                    (o.TipoIntegracao.Tipo != Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoIntegracao.SaintGobain || o.Carga.CarregamentoIntegradoERP) && o.Carga.CargaFechada == true
                );

            return query.OrderBy(propOrdenacao + (dirOrdenacao == "asc" ? " ascending" : " descending")).Skip(0).Take(numeroRegistrosPorVez).Select(x => x.Codigo).ToListAsync(CancellationToken);
        }

        public Dominio.Entidades.Embarcador.Cargas.CargaDadosTransporteIntegracao BuscarIntegracaoAguardandoPorCarga(int codigoCarga)
        {
            IQueryable<Dominio.Entidades.Embarcador.Cargas.CargaDadosTransporteIntegracao> query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.CargaDadosTransporteIntegracao>();

            query = query.Where(o => o.Carga.Codigo == codigoCarga
                            && o.TipoIntegracao.Tipo == Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoIntegracao.UnileverDadosValePedagio
                            );

            return query.FirstOrDefault();
        }

        public Dominio.Entidades.Embarcador.Cargas.CargaDadosTransporteIntegracao BuscarIntegracaoBoticario(int codigoCarga)
        {
            IQueryable<Dominio.Entidades.Embarcador.Cargas.CargaDadosTransporteIntegracao> query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.CargaDadosTransporteIntegracao>();

            query = query.Where(o => o.Carga.Codigo == codigoCarga
                            && o.TipoIntegracao.Tipo == Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoIntegracao.Boticario
                            );

            return query.FirstOrDefault();
        }

        public List<Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoIntegracao> BuscarTipoIntegracaoPorCarga(int codigoCarga, bool integracaoFilialEmissora)
        {
            IQueryable<Dominio.Entidades.Embarcador.Cargas.CargaDadosTransporteIntegracao> query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.CargaDadosTransporteIntegracao>();

            query = query.Where(o => o.Carga.Codigo == codigoCarga);

            return query.Select(o => o.TipoIntegracao.Tipo).Distinct().ToList();
        }

        public List<Dominio.Entidades.Embarcador.Cargas.CargaDadosTransporteIntegracao> Consultar(int codigoCarga, Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoIntegracao? situacao, Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoIntegracao? tipo, bool etapaIncioSemNota, string propOrdena, string dirOrdena, int inicio, int limite)
        {
            IQueryable<Dominio.Entidades.Embarcador.Cargas.CargaDadosTransporteIntegracao> query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.CargaDadosTransporteIntegracao>();

            if (codigoCarga > 0)
                query = query.Where(o => o.Carga.Codigo == codigoCarga);

            if (situacao.HasValue)
                query = query.Where(o => o.SituacaoIntegracao == situacao);

            if (tipo.HasValue)
                query = query.Where(o => o.TipoIntegracao.Tipo == tipo);

            if (query.Any(c => !c.Carga.TipoOperacao.ExigeNotaFiscalParaCalcularFrete))
            {
                if (!etapaIncioSemNota || query.Any(c => c.Carga.TipoOperacao.ConfiguracaoCarga.PermiteInformarTransportadorEtapaUmQuandoNotaFiscalNaoRecebidaAntesCalculoFrete))
                    query = query.Where(o => o.TipoIntegracao.Tipo != Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoIntegracao.Unilever);
                else
                    query = query.Where(o => o.TipoIntegracao.Tipo == Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoIntegracao.Unilever);
            }

            return query.OrderBy(propOrdena + (dirOrdena == "asc" ? " ascending" : " descending"))
                        .Skip(inicio)
                        .Take(limite)
                        .Fetch(obj => obj.TipoIntegracao)
                        .ToList();
        }

        public int ContarConsulta(int codigoCarga, Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoIntegracao? situacao, Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoIntegracao? tipo, bool etapaIncioSemNota)
        {
            IQueryable<Dominio.Entidades.Embarcador.Cargas.CargaDadosTransporteIntegracao> query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.CargaDadosTransporteIntegracao>();
            IQueryable<Dominio.Entidades.Embarcador.Pedidos.StageAgrupamento> subQuery = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Pedidos.StageAgrupamento>();

            if (codigoCarga > 0)
                query = query.Where(o => o.Carga.Codigo == codigoCarga && !subQuery.Any(s => s.CargaGerada.Codigo == codigoCarga));

            if (situacao.HasValue)
                query = query.Where(o => o.SituacaoIntegracao == situacao);

            if (tipo.HasValue)
                query = query.Where(o => o.TipoIntegracao.Tipo == tipo);

            if (query.Any(c => !c.Carga.TipoOperacao.ExigeNotaFiscalParaCalcularFrete))
            {
                if (!etapaIncioSemNota || query.Any(c => c.Carga.TipoOperacao.ConfiguracaoCarga.PermiteInformarTransportadorEtapaUmQuandoNotaFiscalNaoRecebidaAntesCalculoFrete))
                    query = query.Where(o => o.TipoIntegracao.Tipo != Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoIntegracao.Unilever);
                else
                    query = query.Where(o => o.TipoIntegracao.Tipo == Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoIntegracao.Unilever);
            }

            return query.Count();
        }

        public int ContarPorCarga(int codigoCarga)
        {
            IQueryable<Dominio.Entidades.Embarcador.Cargas.CargaDadosTransporteIntegracao> query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.CargaDadosTransporteIntegracao>();

            query = query.Where(o => o.Carga.Codigo == codigoCarga);

            return query.Count();
        }

        public int ContarPorCargaETipoIntegracao(int codigoCarga, int codigoTipoIntegracao)
        {
            IQueryable<Dominio.Entidades.Embarcador.Cargas.CargaDadosTransporteIntegracao> query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.CargaDadosTransporteIntegracao>();

            query = query.Where(o => o.Carga.Codigo == codigoCarga && o.TipoIntegracao.Codigo == codigoTipoIntegracao);

            return query.Count();
        }

        public Dominio.Entidades.Embarcador.Cargas.CargaDadosTransporteIntegracao BuscarPorCargaETipoIntegracao(int codigoCarga, Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoIntegracao tipoIntegracao)
        {
            IQueryable<Dominio.Entidades.Embarcador.Cargas.CargaDadosTransporteIntegracao> query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.CargaDadosTransporteIntegracao>();

            var result = from obj in query where obj.Carga.Codigo == codigoCarga && obj.TipoIntegracao.Tipo == tipoIntegracao select obj;

            return result.FirstOrDefault();
        }

        public async Task<Dominio.Entidades.Embarcador.Cargas.CargaDadosTransporteIntegracao> BuscarPorCargaETipoIntegracaoAsync(int codigoCarga, Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoIntegracao tipoIntegracao)
        {
            IQueryable<Dominio.Entidades.Embarcador.Cargas.CargaDadosTransporteIntegracao> query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.CargaDadosTransporteIntegracao>();

            var result = from obj in query where obj.Carga.Codigo == codigoCarga && obj.TipoIntegracao.Tipo == tipoIntegracao select obj;

            return await result.FirstOrDefaultAsync(CancellationToken);
        }

        public List<Dominio.Entidades.Embarcador.Cargas.CargaDadosTransporteIntegracao> BuscarPorCargaETipoIntegracao(int codigoCarga, List<Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoIntegracao> tiposIntegracao)
        {
            IQueryable<Dominio.Entidades.Embarcador.Cargas.CargaDadosTransporteIntegracao> query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.CargaDadosTransporteIntegracao>();

            var result = from obj in query where obj.Carga.Codigo == codigoCarga && tiposIntegracao.Contains(obj.TipoIntegracao.Tipo) select obj;

            return result.ToList();
        }

        public Dominio.Entidades.Embarcador.Cargas.CargaDadosTransporteIntegracao BuscarPorCargaETipoIntegracao(int codigoCarga, int codigoTipoIntegracao, bool integracaoColeta)
        {
            IQueryable<Dominio.Entidades.Embarcador.Cargas.CargaDadosTransporteIntegracao> query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.CargaDadosTransporteIntegracao>();

            query = query.Where(o => o.Carga.Codigo == codigoCarga && o.TipoIntegracao.Codigo == codigoTipoIntegracao && o.IntegracaoColeta == integracaoColeta);

            return query.FirstOrDefault();
        }

        public List<Dominio.Entidades.Embarcador.Cargas.CargaDadosTransporteIntegracao> BuscarPedidosPorCargaETipoIntegracao(int codigoCarga, int codigoTipoIntegracao, bool integracaoColeta)
        {
            IQueryable<Dominio.Entidades.Embarcador.Cargas.CargaDadosTransporteIntegracao> query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.CargaDadosTransporteIntegracao>();

            query = query.Where(o => o.Carga.Codigo == codigoCarga && o.TipoIntegracao.Codigo == codigoTipoIntegracao && o.IntegracaoColeta == integracaoColeta);

            return query.ToList();
        }

        public Dominio.Entidades.Embarcador.Cargas.CargaDadosTransporteIntegracao BuscarIntegracaoAguardandoVencedor(int codigoCarga)
        {
            IQueryable<Dominio.Entidades.Embarcador.Cargas.CargaDadosTransporteIntegracao> query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.CargaDadosTransporteIntegracao>();

            query = query.Where(o => o.Carga.Codigo == codigoCarga && o.TipoIntegracao.Tipo == Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoIntegracao.UnileverLeilaoManual);

            return query.FirstOrDefault();
        }

        public int ContarPorCargaESituacaoDiff(int codigoCarga, Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoIntegracao situacaoDiff)
        {
            IQueryable<Dominio.Entidades.Embarcador.Cargas.CargaDadosTransporteIntegracao> query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.CargaDadosTransporteIntegracao>();

            query = query.Where(o => o.Carga.Codigo == codigoCarga && o.SituacaoIntegracao != situacaoDiff);

            return query.Count();
        }

        public int ContarPorCarga(int codigoCarga, Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoIntegracao[] situacao)
        {
            IQueryable<Dominio.Entidades.Embarcador.Cargas.CargaDadosTransporteIntegracao> query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.CargaDadosTransporteIntegracao>();

            query = query.Where(o => o.Carga.Codigo == codigoCarga && situacao.Contains(o.SituacaoIntegracao));

            return query.Count();
        }

        public Dominio.Entidades.Embarcador.Cargas.CargaDadosTransporteIntegracao BuscarPorCodigoArquivo(int codigoArquivo)
        {
            IQueryable<Dominio.Entidades.Embarcador.Cargas.CargaDadosTransporteIntegracao> query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.CargaDadosTransporteIntegracao>();

            query = query.Where(obj => obj.ArquivosTransacao.Any(o => o.Codigo == codigoArquivo));

            return query.FirstOrDefault();
        }

        public List<Dominio.Entidades.Embarcador.Cargas.CargaDadosTransporteIntegracao> BuscarPorCarga(int codigoCarga)
        {
            IQueryable<Dominio.Entidades.Embarcador.Cargas.CargaDadosTransporteIntegracao> query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.CargaDadosTransporteIntegracao>();

            query = query.Where(o => o.Carga.Codigo == codigoCarga);

            return query.ToList();
        }

        public string BuscarProtocoloPorCargaETipoIntegracao(int codigoCarga, Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoIntegracao tipoIntegracao)
        {
            IQueryable<Dominio.Entidades.Embarcador.Cargas.CargaDadosTransporteIntegracao> query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.CargaDadosTransporteIntegracao>();

            var result = from obj in query where obj.Carga.Codigo == codigoCarga && obj.TipoIntegracao.Tipo == tipoIntegracao select obj;

            return result.Select(o => o.Protocolo).FirstOrDefault();
        }

        public async Task<List<Dominio.Entidades.Embarcador.Cargas.CargaDadosTransporteIntegracao>> BuscarPorCargaAsync(int codigoCarga)
        {
            IQueryable<Dominio.Entidades.Embarcador.Cargas.CargaDadosTransporteIntegracao> query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.CargaDadosTransporteIntegracao>();

            query = query.Where(o => o.Carga.Codigo == codigoCarga);

            return await query.ToListAsync();
        }

        public bool VerificarSeIntegrouPorCarga(int codigoCarga, List<Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoIntegracao> tiposIntegracoesLiberadas)
        {
            IQueryable<Dominio.Entidades.Embarcador.Cargas.CargaDadosTransporteIntegracao> query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.CargaDadosTransporteIntegracao>();

            query = query.Where(o => o.Carga.Codigo == codigoCarga && tiposIntegracoesLiberadas.Contains(o.TipoIntegracao.Tipo) && o.SituacaoIntegracao == Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoIntegracao.Integrado);

            return query.Any();
        }

        public bool ExistePorCarga(int codigoCarga)
        {
            IQueryable<Dominio.Entidades.Embarcador.Cargas.CargaDadosTransporteIntegracao> query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.CargaDadosTransporteIntegracao>();
            IQueryable<Dominio.Entidades.Embarcador.Pedidos.StageAgrupamento> subQuery = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Pedidos.StageAgrupamento>();

            query = query.Where(o => o.Carga.Codigo == codigoCarga && !subQuery.Any(s => s.CargaGerada.Codigo == codigoCarga));

            return query.Select(o => o.Codigo).Any();
        }

        public List<Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoIntegracao> BuscarSituacoesPorCarga(int codigoCarga)
        {
            IQueryable<Dominio.Entidades.Embarcador.Cargas.CargaDadosTransporteIntegracao> query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.CargaDadosTransporteIntegracao>();

            query = query.Where(o => o.Carga.Codigo == codigoCarga);

            return query.Select(o => o.SituacaoIntegracao).Distinct().ToList();
        }

        public List<Dominio.Entidades.Embarcador.Cargas.CargaDadosTransporteIntegracao> BuscarPorCarga(int codigoCarga, Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoIntegracao? situacao, Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoIntegracao? tipo)
        {
            IQueryable<Dominio.Entidades.Embarcador.Cargas.CargaDadosTransporteIntegracao> query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.CargaDadosTransporteIntegracao>();

            query = query.Where(o => o.Carga.Codigo == codigoCarga);

            if (situacao.HasValue)
                query = query.Where(o => o.SituacaoIntegracao == situacao.Value);

            if (tipo.HasValue)
                query = query.Where(o => o.TipoIntegracao.Tipo == tipo.Value);

            return query.ToList();
        }

        public Task<List<Dominio.Entidades.Embarcador.Cargas.CargaDadosTransporteIntegracao>> BuscarPorCargaAsync(int codigoCarga, Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoIntegracao? situacao, Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoIntegracao? tipo)
        {
            IQueryable<Dominio.Entidades.Embarcador.Cargas.CargaDadosTransporteIntegracao> query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.CargaDadosTransporteIntegracao>();

            query = query.Where(o => o.Carga.Codigo == codigoCarga);

            if (situacao.HasValue)
                query = query.Where(o => o.SituacaoIntegracao == situacao.Value);

            if (tipo.HasValue)
                query = query.Where(o => o.TipoIntegracao.Tipo == tipo.Value);

            return query.ToListAsync(CancellationToken);
        }

        public Dominio.Entidades.Embarcador.Cargas.CargaDadosTransporteIntegracao BuscarPorCargaTipoIntegracaoColetaSituacao(int codigoCarga, Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoIntegracao tipoIntegracao, bool coleta, Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoIntegracao situacaoIntegracao)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.CargaDadosTransporteIntegracao>();

            var result = from obj in query where obj.Carga.Codigo == codigoCarga && obj.TipoIntegracao.Tipo == tipoIntegracao && obj.IntegracaoColeta == coleta && obj.SituacaoIntegracao == situacaoIntegracao select obj;

            return result.FirstOrDefault();
        }

        public List<Dominio.Entidades.Embarcador.Cargas.CargaDadosTransporteIntegracao> BuscarPendentesAguardandoFinalizarCargaAnterior(int cargaFinalizada)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.CargaDadosTransporteIntegracao>();

            var result = from obj in query
                         where (obj.SituacaoIntegracao == Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoIntegracao.ProblemaIntegracao) &&
                               obj.AguardarFinalizarCargaAnterior && (obj.CargaPendente.Codigo == cargaFinalizada)
                         select obj;

            return result.OrderBy(i => i.Carga.DataCriacaoCarga).ToList();
        }
        #endregion
    }
}
