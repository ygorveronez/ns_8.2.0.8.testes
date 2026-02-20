using Dominio.ObjetosDeValor.Embarcador.Enumeradores;
using NHibernate.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Dynamic.Core;
using System.Threading;
using System.Threading.Tasks;

namespace Repositorio.Embarcador.Cargas
{
    public class CargaCTeComplementoInfo : RepositorioBase<Dominio.Entidades.Embarcador.Cargas.CargaCTeComplementoInfo>
    {
        #region Construtores

        public CargaCTeComplementoInfo(UnitOfWork unitOfWork) : base(unitOfWork) { }
        public CargaCTeComplementoInfo(UnitOfWork unitOfWork, CancellationToken cancellationToken) : base(unitOfWork, cancellationToken) { }

        #endregion Construtores

        #region Métodos Públicos

        public List<int> BuscarCodigosCTePorOcorrencia(int ocorrencia, bool filialEmissora)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.CargaCTeComplementoInfo>();

            var resut = from obj in query where obj.CargaOcorrencia.Codigo == ocorrencia && obj.ComplementoFilialEmissora == filialEmissora && !obj.CTe.ModeloDocumentoFiscal.NaoGerarEscrituracao select obj.CTe.Codigo;

            return resut.ToList();
        }

        public List<Dominio.Entidades.ConhecimentoDeTransporteEletronico> BuscarCTePorOcorrencia(int ocorrencia)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.CargaCTeComplementoInfo>();

            var resut = from obj in query where obj.CargaOcorrencia.Codigo == ocorrencia && !obj.CTe.ModeloDocumentoFiscal.NaoGerarEscrituracao select obj.CTe;

            return resut.ToList();
        }

        public bool ExisteCTesParaLiberarSemInutilizacao(int codigoOcorrencia)
        {
            IQueryable<Dominio.Entidades.Embarcador.Cargas.CargaCTeComplementoInfo> query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.CargaCTeComplementoInfo>();

            query = query.Where(o => o.CargaOcorrencia.Codigo == codigoOcorrencia);
            query = query.Where(obj => obj.CTe != null && obj.CTe.Status == "R" && ((obj.CTe.MensagemStatus != null && obj.CTe.MensagemStatus.PermiteLiberarSemInutilizacao) || obj.CTe.MensagemStatus == null));

            return query.Any();
        }

        public bool ExisteModeloDeDocumentoEmitidoNaOcorrencia(int codigoOcorrencia, List<int> codigosModelosDocumentos)
        {
            IQueryable<Dominio.Entidades.Embarcador.Cargas.CargaCTeComplementoInfo> query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.CargaCTeComplementoInfo>();

            query = query.Where(o => o.CargaOcorrencia.Codigo == codigoOcorrencia && codigosModelosDocumentos.Contains(o.CTe.ModeloDocumentoFiscal.Codigo));

            return query.Any();
        }

        public List<Dominio.Entidades.Cliente> BuscarTomadoresPorOcorrencia(int ocorrencia, bool filialEmissora)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.CargaCTeComplementoInfo>();

            var resut = from obj in query where obj.CargaOcorrencia.Codigo == ocorrencia && obj.ComplementoFilialEmissora == filialEmissora && obj.CTe != null select obj.CTe.TomadorPagador.Cliente;

            return resut.Distinct().ToList();
        }

        public List<Dominio.Entidades.Embarcador.Cargas.CargaCTeComplementoInfo> BuscarDadosCTesPorOcorrencia(List<int> codigosOcorrencia)
        {
            List<Dominio.Entidades.Embarcador.Cargas.CargaCTeComplementoInfo> result = new List<Dominio.Entidades.Embarcador.Cargas.CargaCTeComplementoInfo>();

            int take = 1000;
            int start = 0;

            while (start < codigosOcorrencia?.Count)
            {
                List<int> temp = codigosOcorrencia.Skip(start).Take(take).ToList();

                var consultaCargaCTeComplementoInfo = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.CargaCTeComplementoInfo>()
                    .Where(o => temp.Contains(o.CargaOcorrencia.Codigo) && o.CTe != null).ToList();

                result.AddRange(consultaCargaCTeComplementoInfo.ToList());
                start += take;
            }

            return result;
        }

        public List<Dominio.Entidades.ConhecimentoDeTransporteEletronico> BuscarCTesPorOcorrencia(int ocorrencia)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.CargaCTeComplementoInfo>();

            var resut = from obj in query where obj.CargaOcorrencia.Codigo == ocorrencia && obj.CTe != null select obj.CTe;

            return resut.ToList();
        }

        public Dominio.Entidades.Embarcador.Cargas.CargaCTeComplementoInfo BuscarPorCodigo(int codigo)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.CargaCTeComplementoInfo>();
            var resut = from obj in query where obj.Codigo == codigo select obj;
            return resut.FirstOrDefault();
        }

        public Dominio.Entidades.Embarcador.Cargas.CargaCTeComplementoInfo BuscarPorChaveCTeAnterior(string chave)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.CargaCTeComplementoInfo>();
            var resut = from obj in query where obj.CargaCTeComplementado.CTe.Chave == chave select obj;
            return resut.FirstOrDefault();
        }

        public List<Dominio.Entidades.Embarcador.Cargas.CargaCTeComplementoInfo> BuscarPorChaveCTesAnterior(string chave)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.CargaCTeComplementoInfo>();
            var resut = from obj in query where obj.CargaCTeComplementado.CTe.Chave == chave select obj;
            return resut.ToList();
        }

        public List<Dominio.Entidades.Embarcador.Cargas.CargaCTeComplementoInfo> BuscarPorFechamento(int codigoFechamento)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.CargaCTeComplementoInfo>();
            var resut = from obj in query where obj.FechamentoFrete.Codigo == codigoFechamento && (obj.CTe == null || obj.CTe.Status != "C") select obj;
            return resut
                       .Fetch(obj => obj.CargaCTeComplementado)
                           .ThenFetch(obj => obj.CTe)
                           .ThenFetch(obj => obj.Empresa)
                           .ThenFetch(obj => obj.Configuracao)
                       .Fetch(obj => obj.CargaCTeComplementado)
                           .ThenFetch(obj => obj.Carga)
                           .ThenFetch(obj => obj.Empresa)
                           .ThenFetch(obj => obj.Configuracao)
                       .Fetch(obj => obj.CargaOcorrencia)
                           .ThenFetch(obj => obj.Emitente)
                           .ThenFetch(obj => obj.Configuracao)
                       .ToList();
        }

        public List<Dominio.Entidades.Embarcador.Cargas.CargaCTeComplementoInfo> BuscarPorOcorrencia(int codigoOcorrencia)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.CargaCTeComplementoInfo>();
            var resut = from obj in query where obj.CargaOcorrencia.Codigo == codigoOcorrencia select obj;
            return resut.Fetch(obj => obj.CTe).ToList();
        }

        public Dominio.Entidades.Embarcador.Cargas.CargaCTeComplementoInfo BuscarNFSManualASerComplementadaPorOcorrencia(int codigoOcorrencia)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.CargaCTeComplementoInfo>();
            var resut = from obj in query where obj.CargaOcorrencia.Codigo == codigoOcorrencia && obj.CargaDocumentoParaEmissaoNFSManualGerado != null && obj.CTe == null select obj;
            return resut.Fetch(obj => obj.CTe).FirstOrDefault();
        }

        public List<Dominio.Entidades.Embarcador.Cargas.CargaCTeComplementoInfo> BuscarPorDocumentoParaLancamentoNFSManual(List<int> codigosCargaDocumentoParaEmissaoNFSManuals)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.CargaCTeComplementoInfo>();
            var resut = from obj in query where codigosCargaDocumentoParaEmissaoNFSManuals.Contains(obj.CargaDocumentoParaEmissaoNFSManualGerado.Codigo) select obj;
            return resut.Fetch(obj => obj.CTe).ToList();
        }


        public List<Dominio.Entidades.Embarcador.Cargas.CargaCTeComplementoInfo> BuscarPorNumeroOcorrencia(int numeroOcorrencia)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.CargaCTeComplementoInfo>();
            var resut = from obj in query where obj.CargaOcorrencia.NumeroOcorrencia == numeroOcorrencia select obj;
            return resut.Fetch(obj => obj.CTe).ToList();
        }

        public List<Dominio.Entidades.Embarcador.Cargas.CargaCTeComplementoInfo> BuscarPorOcorrencias(List<int> codigosOcorrencia)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.CargaCTeComplementoInfo>();
            var resut = from obj in query where codigosOcorrencia.Contains(obj.CargaOcorrencia.Codigo) select obj;
            return resut.ToList();
        }

        public List<Dominio.Entidades.Embarcador.Cargas.CargaCTeComplementoInfo> BuscarPorOcorrencia(int codigoOcorrencia, bool complementoCteFilialEmissora)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.CargaCTeComplementoInfo>();
            var resut = from obj in query where obj.CargaOcorrencia.Codigo == codigoOcorrencia && obj.ComplementoFilialEmissora == complementoCteFilialEmissora select obj;
            return resut
                .Fetch(obj => obj.ComponenteFrete)
                .Fetch(obj => obj.CargaOcorrencia)
                    .ThenFetch(obj => obj.Carga)
                .Fetch(obj => obj.CargaOcorrencia)
                    .ThenFetch(obj => obj.TipoOcorrencia)
                .Fetch(obj => obj.CargaCTeComplementado)
                    .ThenFetch(obj => obj.CTe)
                    .ThenFetch(obj => obj.ModeloDocumentoFiscal)
                .Fetch(obj => obj.CargaCTeComplementado)
                    .ThenFetch(obj => obj.CTe)
                    .ThenFetch(obj => obj.Remetente)
                    .ThenFetch(obj => obj.Cliente)
                    .ThenFetch(obj => obj.Localidade)
                .Fetch(obj => obj.CargaCTeComplementado)
                    .ThenFetch(obj => obj.CTe)
                    .ThenFetch(obj => obj.OutrosTomador)
                    .ThenFetch(obj => obj.Cliente)
                    .ThenFetch(obj => obj.Localidade)
                .Fetch(obj => obj.CargaCTeComplementado)
                    .ThenFetch(obj => obj.CTe)
                    .ThenFetch(obj => obj.Serie)
                .Fetch(obj => obj.CargaCTeComplementado)
                    .ThenFetch(obj => obj.CTe)
                    .ThenFetch(obj => obj.LocalidadeInicioPrestacao)
                .Fetch(obj => obj.CargaCTeComplementado)
                    .ThenFetch(obj => obj.CTe)
                    .ThenFetch(obj => obj.LocalidadeTerminoPrestacao)
                .Fetch(obj => obj.CargaCTeComplementado)
                    .ThenFetch(obj => obj.CTe)
                    .ThenFetch(obj => obj.Empresa)
                    .ThenFetch(obj => obj.Configuracao)
                .Fetch(obj => obj.CargaCTeComplementado)
                    .ThenFetch(obj => obj.Carga)
                    .ThenFetch(obj => obj.Empresa)
                    .ThenFetch(obj => obj.Configuracao)
                .Fetch(obj => obj.CargaOcorrencia)
                    .ThenFetch(obj => obj.Emitente)
                    .ThenFetch(obj => obj.Configuracao)
                .ToList();
        }

        public Dominio.Entidades.Embarcador.Cargas.CargaCTeComplementoInfo BuscarPorNumeroOcorrenciaEValor(int numeroOcorrencia, decimal valor)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.CargaCTeComplementoInfo>();
            var resut = from obj in query
                        where
                            obj.PreCTe != null
                            && obj.CTe == null
                            && obj.CargaOcorrencia.NumeroOcorrencia == numeroOcorrencia
                            && obj.PreCTe.ValorAReceber == valor
                        select obj;
            return resut.FirstOrDefault();
        }

        public List<Dominio.Entidades.Embarcador.Cargas.CargaCTeComplementoInfo> BuscarCTesPorOcorrencia(int codigoOcorrencia, bool somenteCTeFilialEmissora)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.CargaCTeComplementoInfo>();
            var resut = from obj in query where obj.CargaOcorrencia.Codigo == codigoOcorrencia && obj.CTe != null select obj;

            if (somenteCTeFilialEmissora)
                resut = resut.Where(obj => obj.ComplementoFilialEmissora);

            return resut.ToList();
        }

        public Dominio.Entidades.Embarcador.Cargas.CargaCTeComplementoInfo BuscarPreCTePorOcorrenciaEChaveNFE(int ocorrencia, string chaveNFe)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.CargaCTeComplementoInfo>();
            var resut = from obj in query where obj.CargaOcorrencia.Codigo == ocorrencia && obj.PreCTe.Documentos.Any(nf => nf.ChaveNFE == chaveNFe) select obj;
            return resut.FirstOrDefault();
        }

        public Dominio.Entidades.Embarcador.Cargas.CargaCTeComplementoInfo BuscarPreCTePorOcorrenciaENumeroOutroDoc(int ocorrencia, string numero)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.CargaCTeComplementoInfo>();
            var resut = from obj in query where obj.CargaOcorrencia.Codigo == ocorrencia && obj.PreCTe.Documentos.Any(nf => nf.Numero == numero) select obj;
            return resut.FirstOrDefault();
        }

        public Dominio.Entidades.Embarcador.Cargas.CargaCTeComplementoInfo BuscarPorCTe(int codigoCTe)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.CargaCTeComplementoInfo>();
            var resut = from obj in query where obj.CTe.Codigo == codigoCTe select obj;
            return resut.FirstOrDefault();
        }

        public int BuscarQuantidadeParcelaPorCTe(int codigoCTe)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.CargaCTeComplementoInfo>()
                .Where(obj => obj.CTe.Codigo == codigoCTe && obj.CargaOcorrencia.QuantidadeParcelas > 0);

            return query.Select(obj => (int?)obj.CargaOcorrencia.QuantidadeParcelas).FirstOrDefault() ?? 0;
        }

        public decimal BuscarTotalICMSPorOcorrencia(int codigoOcorrencia)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.CargaCTeComplementoInfo>();
            var resut = from obj in query where obj.CargaOcorrencia.Codigo == codigoOcorrencia select obj;
            return resut.Select(obj => (decimal?)obj.CTe.ValorICMS).Sum() ?? 0m;
        }

        public decimal BuscarTotalFreteLiquidoPorOcorrencia(int codigoOcorrencia)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.CargaCTeComplementoInfo>();
            var resut = from obj in query where obj.CargaOcorrencia.Codigo == codigoOcorrencia select obj;
            return resut.Select(obj => (decimal?)obj.CTe.ValorFrete).Sum() ?? 0m;
        }

        public decimal BuscarTotalFretePorOcorrencia(int codigoOcorrencia)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.CargaCTeComplementoInfo>();
            var resut = from obj in query where obj.CargaOcorrencia.Codigo == codigoOcorrencia select obj;
            return resut.Select(obj => (decimal?)obj.CTe.ValorAReceber).Sum() ?? 0m;
        }

        public List<Dominio.Entidades.Embarcador.Cargas.CargaCTeComplementoInfo> BuscarPorComplementoDeFrete(int codigoComplementoFrete)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.CargaCTeComplementoInfo>();
            var resut = from obj in query where obj.CargaComplementoFrete.Codigo == codigoComplementoFrete select obj;
            return resut
                    .Fetch(obj => obj.CargaCTeComplementado)
                        .ThenFetch(obj => obj.CTe)
                        .ThenFetch(obj => obj.Empresa)
                        .ThenFetch(obj => obj.Configuracao)
                    .Fetch(obj => obj.CargaCTeComplementado)
                        .ThenFetch(obj => obj.Carga)
                        .ThenFetch(obj => obj.Empresa)
                        .ThenFetch(obj => obj.Configuracao)
                    .Fetch(obj => obj.CargaOcorrencia)
                        .ThenFetch(obj => obj.Emitente)
                        .ThenFetch(obj => obj.Configuracao)
                    .ToList();
        }

        public List<Dominio.Entidades.Embarcador.Cargas.CargaCTeComplementoInfo> BuscarPreCTesPorOcorrencia(int codigoOcorrencia, string propOrdenacao, string dirOrdenacao, int inicioRegistros, int maximoRegistros)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.CargaCTeComplementoInfo>();
            var resut = from obj in query where obj.CargaOcorrencia.Codigo == codigoOcorrencia where obj.PreCTe != null select obj;

            return resut.OrderBy(propOrdenacao + (dirOrdenacao == "asc" ? " ascending" : " descending")).Skip(inicioRegistros).Take(maximoRegistros).ToList();
        }

        public List<Dominio.Entidades.Embarcador.Cargas.CargaCTeComplementoInfo> BuscarPreCTesPorOcorrenciaPendenteIntegracapGPA(int codigoOcorrencia)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.CargaCTeComplementoInfo>();
            var resut = from obj in query
                        where
                            obj.CargaOcorrencia.Codigo == codigoOcorrencia
                            && obj.PreCTe != null
                            && obj.IntegradoComGPA == false
                        select obj;

            return resut.ToList();
        }

        public Dominio.Entidades.Embarcador.Cargas.CargaCTeComplementoInfo BuscarPreCTePorChaveAnterior(int ocorrencia, string chaveChave)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.CargaCTeComplementoInfo>();
            var resut = from obj in query where obj.CargaOcorrencia.Codigo == ocorrencia && obj.PreCTe.ChaveCTESubComp == chaveChave select obj;
            return resut.FirstOrDefault();
        }

        public List<Dominio.Entidades.Embarcador.Cargas.CargaCTeComplementoInfo> BuscarCTesPorFechamento(int codigoFechamento)
        {
            return BuscarCTesPorFechamento(codigoFechamento, parametrosConsulta: null);
        }

        public List<Dominio.Entidades.Embarcador.Cargas.CargaCTeComplementoInfo> BuscarCTesPorFechamento(int codigoFechamento, Dominio.ObjetosDeValor.Embarcador.Consulta.ParametroConsulta parametrosConsulta)
        {
            var consultaCargaCTeComplementoInfo = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.CargaCTeComplementoInfo>()
                .Where(o => o.FechamentoFrete.Codigo == codigoFechamento && o.CTe != null);

            return ObterLista(consultaCargaCTeComplementoInfo, parametrosConsulta);
        }

        public List<Dominio.Entidades.Embarcador.Cargas.CargaCTeComplementoInfo> BuscarCTesRejeitadosPorFechamento(int codigoFechamento)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.CargaCTeComplementoInfo>()
                .Where(obj => obj.FechamentoFrete.Codigo == codigoFechamento && obj.CTe.Status == "R");

            return query.ToList();
        }

        public List<Dominio.Entidades.Embarcador.Cargas.CargaCTeComplementoInfo> BuscarCTesPorOcorrencia(int codigoOcorrencia, string propOrdenacao, string dirOrdenacao, int inicioRegistros, int maximoRegistros)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.CargaCTeComplementoInfo>();
            var resut = from obj in query where obj.CargaOcorrencia.Codigo == codigoOcorrencia where obj.CTe != null select obj;

            if (!string.IsNullOrWhiteSpace(propOrdenacao))
                resut = resut.OrderBy(propOrdenacao + (dirOrdenacao == "asc" ? " ascending" : " descending"));

            if (inicioRegistros > 0)
                resut = resut.Skip(inicioRegistros);

            if (maximoRegistros > 0)
                resut = resut.Take(maximoRegistros);

            return resut.Fetch(o => o.CTe).ThenFetch(o => o.Remetente).ThenFetch(o => o.Cliente)
                         .Fetch(o => o.CTe).ThenFetch(o => o.Destinatario).ThenFetch(o => o.Cliente)
                         .Fetch(o => o.CTe).ThenFetch(o => o.OutrosTomador).ThenFetch(o => o.Cliente)
                         .Fetch(o => o.CTe).ThenFetch(o => o.Expedidor).ThenFetch(o => o.Cliente)
                         .Fetch(o => o.CTe).ThenFetch(o => o.Recebedor).ThenFetch(o => o.Cliente)
                         .Fetch(o => o.CTe).ThenFetch(o => o.MensagemStatus)
                         .Fetch(o => o.CTe).ThenFetch(o => o.Serie)
                         .Fetch(o => o.CTe).ThenFetch(o => o.LocalidadeTerminoPrestacao)
                         .Fetch(o => o.CTe).ThenFetch(o => o.ModeloDocumentoFiscal)
                .ToList();
        }

        public async Task<List<Dominio.Entidades.Embarcador.Cargas.CargaCTeComplementoInfo>> BuscarCTesPorOcorrenciaAsync(int codigoOcorrencia, string propOrdenacao,
            string dirOrdenacao, int inicioRegistros, int maximoRegistros)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.CargaCTeComplementoInfo>();
            var resut = from obj in query where obj.CargaOcorrencia.Codigo == codigoOcorrencia where obj.CTe != null select obj;

            if (!string.IsNullOrWhiteSpace(propOrdenacao))
                resut = resut.OrderBy(propOrdenacao + (dirOrdenacao == "asc" ? " ascending" : " descending"));

            if (inicioRegistros > 0)
                resut = resut.Skip(inicioRegistros);

            if (maximoRegistros > 0)
                resut = resut.Take(maximoRegistros);

            return await resut.Fetch(o => o.CTe).ThenFetch(o => o.Remetente).ThenFetch(o => o.Cliente)
                         .Fetch(o => o.CTe).ThenFetch(o => o.Destinatario).ThenFetch(o => o.Cliente)
                         .Fetch(o => o.CTe).ThenFetch(o => o.OutrosTomador).ThenFetch(o => o.Cliente)
                         .Fetch(o => o.CTe).ThenFetch(o => o.Expedidor).ThenFetch(o => o.Cliente)
                         .Fetch(o => o.CTe).ThenFetch(o => o.Recebedor).ThenFetch(o => o.Cliente)
                         .Fetch(o => o.CTe).ThenFetch(o => o.MensagemStatus)
                         .Fetch(o => o.CTe).ThenFetch(o => o.Serie)
                         .Fetch(o => o.CTe).ThenFetch(o => o.LocalidadeTerminoPrestacao)
                         .Fetch(o => o.CTe).ThenFetch(o => o.ModeloDocumentoFiscal)
                .ToListAsync();
        }

        public List<Dominio.Entidades.Embarcador.Cargas.CargaCTeComplementoInfo> BuscarCTesPorOcorrencia(int codigoOcorrencia, bool cteFilialEmissora, string propOrdenacao, string dirOrdenacao, int inicioRegistros, int maximoRegistros)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.CargaCTeComplementoInfo>();
            var resut = from obj in query where obj.CargaOcorrencia.Codigo == codigoOcorrencia where obj.CTe != null select obj;

            if (!string.IsNullOrWhiteSpace(propOrdenacao))
                resut = resut.OrderBy(propOrdenacao + (dirOrdenacao == "asc" ? " ascending" : " descending"));

            if (cteFilialEmissora)
                resut = resut.Where(obj => obj.CargaCTeComplementado.CargaCTeSubContratacaoFilialEmissora != null);
            else
                resut = resut.Where(obj => obj.CargaCTeComplementado.CargaCTeFilialEmissora != null || (obj.CargaCTeComplementado.CargaCTeFilialEmissora == null && obj.CargaCTeComplementado.CargaCTeSubContratacaoFilialEmissora == null));

            if (inicioRegistros > 0)
                resut = resut.Skip(inicioRegistros);

            if (maximoRegistros > 0)
                resut = resut.Take(maximoRegistros);

            return resut.Fetch(o => o.CTe).ThenFetch(o => o.Remetente).ThenFetch(o => o.Cliente)
                         .Fetch(o => o.CTe).ThenFetch(o => o.Destinatario).ThenFetch(o => o.Cliente)
                         .Fetch(o => o.CTe).ThenFetch(o => o.OutrosTomador).ThenFetch(o => o.Cliente)
                         .Fetch(o => o.CTe).ThenFetch(o => o.Expedidor).ThenFetch(o => o.Cliente)
                         .Fetch(o => o.CTe).ThenFetch(o => o.Recebedor).ThenFetch(o => o.Cliente)
                         .Fetch(o => o.CTe).ThenFetch(o => o.TomadorPagador).ThenFetch(o => o.Cliente)
                         .Fetch(o => o.CTe).ThenFetch(o => o.MensagemStatus)
                         .Fetch(o => o.CTe).ThenFetch(o => o.Serie)
                         .Fetch(o => o.CTe).ThenFetch(o => o.LocalidadeTerminoPrestacao)
                         .Fetch(o => o.CTe).ThenFetch(o => o.ModeloDocumentoFiscal)
                         .ToList();
        }

        public List<Dominio.Entidades.ConhecimentoDeTransporteEletronico> BuscarCTesPorOcorrenciaSemFilialEmissora(int codigoOcorrencia)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.CargaCTeComplementoInfo>();
            var resut = from obj in query where obj.CargaOcorrencia.Codigo == codigoOcorrencia where obj.CTe != null select obj;

            resut = resut.Where(obj => obj.CargaCTeComplementado.CargaCTeFilialEmissora != null || (obj.CargaCTeComplementado.CargaCTeFilialEmissora == null && obj.CargaCTeComplementado.CargaCTeSubContratacaoFilialEmissora == null));

            return resut.Select(o => o.CTe).ToList();
        }

        public List<Dominio.Entidades.Embarcador.Cargas.CargaCTeComplementoInfo> BuscarCTesAguardandoIntegracao(int inicioRegistros, int maximoRegistros, DateTime? dataInicial, DateTime? dataFinal)
        {
            var query = SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.CargaCTeComplementoInfo>();
            var resut = from obj in query
                        where !obj.ComplementoIntegradoEmbarcador &&
                        obj.CTe != null &&
                        obj.CTe.Status == "A" &&
                        (obj.CTe.ModeloDocumentoFiscal.TipoDocumentoEmissao == Dominio.Enumeradores.TipoDocumento.CTe || obj.CTe.ModeloDocumentoFiscal.TipoDocumentoEmissao == Dominio.Enumeradores.TipoDocumento.Outros)
                        select obj;

            if (dataInicial != DateTime.MinValue)
                resut = resut.Where(obj => obj.CTe.DataEmissao >= dataInicial);

            if (dataFinal != DateTime.MinValue)
                resut = resut.Where(obj => obj.CTe.DataEmissao <= dataFinal);

            return resut.OrderBy(obj => obj.Codigo).Skip(inicioRegistros).Take(maximoRegistros).ToList();
        }

        public List<Dominio.ObjetosDeValor.WebService.CTe.CTeComplementar> BuscarCTesSubstitutosAguardandoIntegracao()
        {
            var result = MontarQueryConsultaCTeSubstitutoAguardandoIntegracao();

            List<Dominio.ObjetosDeValor.WebService.CTe.CTeComplementar> cteSubstitutos = result.Select(cteSubstituto => new Dominio.ObjetosDeValor.WebService.CTe.CTeComplementar()
            {
                ProtocoloCTeComplementado = cteSubstituto.Codigo,
                CTe = new Dominio.ObjetosDeValor.WebService.CTe.CTe
                {

                    ProtocoloCarga = cteSubstituto.CargaOcorrencia.Carga == null ? 0 : cteSubstituto.CargaOcorrencia.Carga.Protocolo,
                    Chave = cteSubstituto.CTe.Chave,
                    CFOP = cteSubstituto.CTe.CFOP.CodigoCFOP,
                    DataEmissao = cteSubstituto.CTe.DataEmissao.HasValue ? cteSubstituto.CTe.DataEmissao.Value.ToString("dd/MM/yyyy HH:mm") : string.Empty,
                    Lotacao = cteSubstituto.CTe.Lotacao == Dominio.Enumeradores.OpcaoSimNao.Sim ? true : false,
                    Modelo = cteSubstituto.CTe.ModeloDocumentoFiscal.Numero,
                    Numero = cteSubstituto.CTe.Numero,
                    NumeroControle = cteSubstituto.CTe.NumeroControle,
                    Protocolo = cteSubstituto.CTe.Codigo,
                    Serie = cteSubstituto.CTe.Serie.Numero,
                    SituacaoCTeSefaz = cteSubstituto.CTe.SituacaoCTeSefaz,
                    TipoCTE = cteSubstituto.CTe.TipoCTE,
                    TipoServico = cteSubstituto.CTe.TipoServico,
                    TipoTomador = cteSubstituto.CTe.TipoTomador,
                    ProtocoloAutorizacao = cteSubstituto.CTe.Protocolo,
                    DataAutorizacao = cteSubstituto.CTe.DataAutorizacao.HasValue ? cteSubstituto.CTe.DataAutorizacao.Value.ToString("dd/MM/yyyy HH:mm") : string.Empty,
                    DataEmbarque = cteSubstituto.CTe.DataInicioPrestacaoServico.HasValue ? cteSubstituto.CTe.DataInicioPrestacaoServico.Value.ToString("dd/MM/yyyy HH:mm") : string.Empty,
                    DataPreviaVencimento = cteSubstituto.CTe.DataPreviaVencimento.HasValue ? cteSubstituto.CTe.DataPreviaVencimento.Value.ToString("dd/MM/yyyy") : string.Empty,
                    ValorTotalMercadoria = cteSubstituto.CTe.ValorTotalMercadoria,
                    VersaoCTE = cteSubstituto.CTe.Versao,
                    MotivoCancelamento = cteSubstituto.CTe.ObservacaoCancelamento
                },
                Ocorrencia = new Dominio.ObjetosDeValor.WebService.Ocorrencia.Ocorrencia
                {
                    Protocolo = cteSubstituto.CargaOcorrencia.TipoOcorrencia.Codigo,
                    CodigoIntegracao = cteSubstituto.CargaOcorrencia.TipoOcorrencia.CodigoProceda,
                    Descricao = cteSubstituto.CargaOcorrencia.TipoOcorrencia.Descricao,
                    NumeroOcorrencia = cteSubstituto.CargaOcorrencia.NumeroOcorrencia,
                    ProtocoloOcorrencia = cteSubstituto.CargaOcorrencia.Codigo,
                    CPFResponsavel = cteSubstituto.CargaOcorrencia.UsuarioResponsavelAprovacao.CPF,
                    NumeroCargaEmbarcador = cteSubstituto.CargaOcorrencia.Carga.CodigoCargaEmbarcador,
                    NomeResponsavel = cteSubstituto.CargaOcorrencia.UsuarioResponsavelAprovacao.Nome,
                    ProtocoloCarga = cteSubstituto.CargaOcorrencia.Carga.Protocolo
                }

            }).ToList();

            return cteSubstitutos.OrderBy(obj => obj.ProtocoloCTeComplementado).ToList();
        }

        public IQueryable<Dominio.Entidades.Embarcador.Cargas.CargaCTeComplementoInfo> MontarQueryConsultaCTeSubstitutoAguardandoIntegracao()
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.CargaCTeComplementoInfo>();

            return from obj in query
                   where !obj.ComplementoIntegradoEmbarcador
                   where obj.CTe != null && obj.CTe.Status == "A" && obj.CTe.TipoCTE == Dominio.Enumeradores.TipoCTE.Substituto && (obj.CTe.ModeloDocumentoFiscal.TipoDocumentoEmissao == Dominio.Enumeradores.TipoDocumento.CTe || obj.CTe.ModeloDocumentoFiscal.TipoDocumentoEmissao == Dominio.Enumeradores.TipoDocumento.Outros)
                   select obj;
        }


        public int ContarCTesAguardandoIntegracao(DateTime? dataInicial, DateTime? dataFinal)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.CargaCTeComplementoInfo>();
            var resut = from obj in query
                        where !obj.ComplementoIntegradoEmbarcador &&
                        obj.CTe != null && obj.CTe.Status == "A" &&
                        (obj.CTe.ModeloDocumentoFiscal.TipoDocumentoEmissao == Dominio.Enumeradores.TipoDocumento.CTe || obj.CTe.ModeloDocumentoFiscal.TipoDocumentoEmissao == Dominio.Enumeradores.TipoDocumento.Outros)
                        select obj;

            if (dataInicial != DateTime.MinValue)
                resut = resut.Where(obj => obj.CTe.DataEmissao >= dataInicial);

            if (dataFinal != DateTime.MinValue)
                resut = resut.Where(obj => obj.CTe.DataEmissao <= dataFinal);

            return resut.Count();
        }

        public int ContarCTesSubstitutosAguardandoIntegracao()
        {
            var quantidadeRegistros = MontarQueryConsultaCTeSubstitutoAguardandoIntegracao();

            return quantidadeRegistros.Count();
        }

        public List<Dominio.Entidades.Embarcador.Cargas.CargaCTeComplementoInfo> BuscarNFSsAguardandoIntegracao(int inicioRegistros, int maximoRegistros)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.CargaCTeComplementoInfo>();
            var resut = from obj in query where !obj.ComplementoIntegradoEmbarcador where obj.CTe != null && obj.CTe.Status == "A" && (obj.CTe.ModeloDocumentoFiscal.TipoDocumentoEmissao == Dominio.Enumeradores.TipoDocumento.NFSe || obj.CTe.ModeloDocumentoFiscal.TipoDocumentoEmissao == Dominio.Enumeradores.TipoDocumento.NFS) select obj;

            resut = resut
                .Fetch(o => o.CargaCTeComplementado).ThenFetch(o => o.CTe)
                .Fetch(o => o.CargaOcorrencia).ThenFetch(o => o.TipoOcorrencia)
                .Fetch(o => o.CargaOcorrencia).ThenFetch(o => o.Carga)
                .Fetch(o => o.CTe).ThenFetch(o => o.LocalidadeInicioPrestacao)
                .Fetch(o => o.CTe).ThenFetch(o => o.NaturezaNFSe)
                .Fetch(o => o.CTe).ThenFetch(o => o.RPS)
                .Fetch(o => o.CTe).ThenFetch(o => o.Serie)
                .Fetch(o => o.CTe).ThenFetch(o => o.Remetente).ThenFetch(o => o.Cliente)
                .Fetch(o => o.CTe).ThenFetch(o => o.Destinatario).ThenFetch(o => o.Cliente)
                .Fetch(o => o.CTe).ThenFetch(o => o.OutrosTomador).ThenFetch(o => o.Cliente)
                .Fetch(o => o.CTe).ThenFetch(o => o.Expedidor).ThenFetch(o => o.Cliente)
                .Fetch(o => o.CTe).ThenFetch(o => o.Recebedor).ThenFetch(o => o.Cliente)
                .Fetch(o => o.CTe).ThenFetch(o => o.TomadorPagador).ThenFetch(o => o.Cliente)
                .Fetch(o => o.CTe).ThenFetch(o => o.Empresa).ThenFetch(o => o.Localidade);

            return resut.OrderBy(obj => obj.Codigo).Skip(inicioRegistros).Take(maximoRegistros).ToList();
        }

        public int ContarNFSsAguardandoIntegracao()
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.CargaCTeComplementoInfo>();
            var resut = from obj in query where !obj.ComplementoIntegradoEmbarcador where obj.CTe != null && obj.CTe.Status == "A" && (obj.CTe.ModeloDocumentoFiscal.TipoDocumentoEmissao == Dominio.Enumeradores.TipoDocumento.NFSe || obj.CTe.ModeloDocumentoFiscal.TipoDocumentoEmissao == Dominio.Enumeradores.TipoDocumento.NFS) select obj;

            return resut.Count();
        }

        public int ContarPorCTEsOcorrencia(int codigoOcorrencia, bool cteFilialEmissora)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.CargaCTeComplementoInfo>();
            var resut = from obj in query where obj.CargaOcorrencia.Codigo == codigoOcorrencia where obj.CTe != null select obj;

            if (cteFilialEmissora)
                resut = resut.Where(obj => obj.CargaCTeComplementado.CargaCTeSubContratacaoFilialEmissora != null);
            else
                resut = resut.Where(obj => obj.CargaCTeComplementado.CargaCTeFilialEmissora != null || (obj.CargaCTeComplementado.CargaCTeFilialEmissora == null && obj.CargaCTeComplementado.CargaCTeSubContratacaoFilialEmissora == null));


            return resut.Count();
        }

        public int ContarPorCTEsOcorrencia(int codigoOcorrencia)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.CargaCTeComplementoInfo>();
            var resut = from obj in query where obj.CargaOcorrencia.Codigo == codigoOcorrencia where obj.CTe != null select obj;

            return resut.Count();
        }

        public int ContarPorOcorrencia(int codigoOcorrencia)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.CargaCTeComplementoInfo>();
            var resut = from obj in query where obj.CargaOcorrencia.Codigo == codigoOcorrencia select obj;

            return resut.Count();
        }

        public int ContarPorNumeroOcorrencia(int numeroOcorrencia)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.CargaCTeComplementoInfo>();
            var resut = from obj in query where obj.CargaOcorrencia.NumeroOcorrencia == numeroOcorrencia select obj;

            return resut.Count();
        }

        public int ContarPorCTEsFechamento(int codigoFechamento)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.CargaCTeComplementoInfo>();
            var resut = from obj in query where obj.FechamentoFrete.Codigo == codigoFechamento where obj.CTe != null select obj;

            return resut.Count();
        }

        public int ContarPorPreCTEsOcorrencia(int codigoOcorrencia)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.CargaCTeComplementoInfo>();
            var resut = from obj in query where obj.CargaOcorrencia.Codigo == codigoOcorrencia && obj.PreCTe != null select obj;

            return resut.Count();
        }

        public bool CargaCTeComplementoInfoPorOcorrencia(int codigoOcorrencia)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.CargaCTeComplementoInfo>();
            var resut = from obj in query where obj.CargaOcorrencia.Codigo == codigoOcorrencia select obj;

            return resut.Count() > 0;
        }

        public bool ExisteCargaCTeComplementoInfoPorOcorrencia(int codigoOcorrencia)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.CargaCTeComplementoInfo>();

            query = query.Where(o => o.CargaOcorrencia.Codigo == codigoOcorrencia);

            return query.Any();
        }

        public List<Dominio.Entidades.Embarcador.Cargas.CargaCTeComplementoInfo> BuscarPorCargaCTeComplementado(int codigoCargaCTeComplementado)
        {
            IQueryable<Dominio.Entidades.Embarcador.Cargas.CargaCTeComplementoInfo> query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.CargaCTeComplementoInfo>();

            query = query.Where(o => o.CargaCTeComplementado.Codigo == codigoCargaCTeComplementado);

            return query.ToList();
        }

        public List<Dominio.Entidades.Embarcador.Cargas.CargaCTeComplementoInfo> BuscarCTesRejeitadosPorOcorrencia(int codigoOcorrencia)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.CargaCTeComplementoInfo>()
                .Where(obj => obj.CargaOcorrencia.Codigo == codigoOcorrencia && obj.CTe.Status == "R");

            return query.ToList();
        }

        public List<Dominio.Entidades.Embarcador.Cargas.CargaCTeComplementoInfo> BuscarCTesOutrosDocumentosPendentesPorOcorrencia(int codigoOcorrencia)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.CargaCTeComplementoInfo>()
                .Where(obj => obj.CargaOcorrencia.Codigo == codigoOcorrencia && obj.CTe.Status == "S" && obj.CTe.ModeloDocumentoFiscal.TipoDocumentoEmissao == Dominio.Enumeradores.TipoDocumento.Outros);

            return query.ToList();
        }

        public void DeletarPorOcorrencia(int codigoOcorrencia)
        {
            try
            {
                if (UnitOfWork.IsActiveTransaction())
                {
                    UnitOfWork.Sessao.CreateQuery("DELETE FROM CargaCTeComplementoInfo WHERE CargaOcorrencia.Codigo = :codigoOcorrencia").SetInt32("codigoOcorrencia", codigoOcorrencia).ExecuteUpdate();
                }
                else
                {
                    try
                    {
                        UnitOfWork.Start();

                        UnitOfWork.Sessao.CreateQuery("DELETE FROM CargaCTeComplementoInfo WHERE CargaOcorrencia.Codigo = :codigoOcorrencia").SetInt32("codigoOcorrencia", codigoOcorrencia).ExecuteUpdate();

                        UnitOfWork.CommitChanges();
                    }
                    catch
                    {
                        UnitOfWork.Rollback();
                        throw;
                    }
                }
            }
            catch (NHibernate.Exceptions.GenericADOException excecao)
            {
                if ((excecao.InnerException != null) && object.ReferenceEquals(excecao.InnerException.GetType(), typeof(System.Data.SqlClient.SqlException)))
                {
                    System.Data.SqlClient.SqlException excecaoSql = (System.Data.SqlClient.SqlException)excecao.InnerException;

                    if (excecaoSql.Number == 547)
                        throw new Exception("O registro possui dependências e não pode ser excluido.", excecaoSql);
                }

                throw;
            }
        }
        public async Task DeletarPorOcorrenciaAsync(int codigoOcorrencia)
        {
            try
            {
                if (UnitOfWork.IsActiveTransaction())
                {
                    await UnitOfWork.Sessao.CreateQuery("DELETE FROM CargaCTeComplementoInfo WHERE CargaOcorrencia.Codigo = :codigoOcorrencia").SetInt32("codigoOcorrencia", codigoOcorrencia).ExecuteUpdateAsync();
                }
                else
                {
                    try
                    {
                        await UnitOfWork.StartAsync();

                        await UnitOfWork.Sessao.CreateQuery("DELETE FROM CargaCTeComplementoInfo WHERE CargaOcorrencia.Codigo = :codigoOcorrencia").SetInt32("codigoOcorrencia", codigoOcorrencia).ExecuteUpdateAsync();

                        await UnitOfWork.CommitChangesAsync();
                    }
                    catch
                    {
                        await UnitOfWork.RollbackAsync();
                        throw;
                    }
                }
            }
            catch (NHibernate.Exceptions.GenericADOException excecao)
            {
                if ((excecao.InnerException != null) && object.ReferenceEquals(excecao.InnerException.GetType(), typeof(System.Data.SqlClient.SqlException)))
                {
                    System.Data.SqlClient.SqlException excecaoSql = (System.Data.SqlClient.SqlException)excecao.InnerException;

                    if (excecaoSql.Number == 547)
                        throw new Exception("O registro possui dependências e não pode ser excluido.", excecaoSql);
                }

                throw;
            }
        }


        #endregion Métodos Públicos

        #region Métodos Públicos - Reemissão de CT-e

        public int DefinirCteComplementarComoReemitido(int codigoComplementoInfo)
        {
            string sql = @"
                UPDATE T_CARGA_CTE_COMPLEMENTO_INFO 
                   SET CCC_REEMITIR_CTE = 0
                 WHERE CCC_CODIGO = :codigoComplementoInfo";

            var query = this.SessionNHiBernate.CreateSQLQuery(sql)
                .SetParameter("codigoComplementoInfo", codigoComplementoInfo);

            return query.ExecuteUpdate();
        }

        public int DefinirCtesComplementaresRejeitadosParaReemissao()
        {
            string sql = @"
                UPDATE T_CARGA_CTE_COMPLEMENTO_INFO 
                   SET CCC_REEMITIR_CTE = 1
                 WHERE CCC_CODIGO IN (
                           SELECT CargaCTeComplementoInfo.CCC_CODIGO
                             FROM T_CARGA_CTE_COMPLEMENTO_INFO CargaCTeComplementoInfo
                             JOIN T_CTE CTe ON CTe.CON_CODIGO = CargaCTeComplementoInfo.CON_CODIGO
                             JOIN T_CARGA_OCORRENCIA CargaOcorrencia ON CargaOcorrencia.COC_CODIGO = CargaCTeComplementoInfo.COC_CODIGO
                            WHERE COALESCE(CargaCTeComplementoInfo.CCC_REEMITIR_CTE, 0) = 0
                              AND CTe.CON_STATUS = 'R'
                              AND CTe.CON_DATAHORAEMISSAO >= :dataEmissaoMinima
                              AND CargaOcorrencia.COC_SITUACAO_OCORRENCIA = :situacaoOcorrencia
                       )";

            var query = this.SessionNHiBernate.CreateSQLQuery(sql)
                .SetParameter("dataEmissaoMinima", DateTime.Now.AddDays(-7).Date)
                .SetParameter("situacaoOcorrencia", SituacaoOcorrencia.PendenciaEmissao);

            return query.ExecuteUpdate();
        }

        public IList<Dominio.ObjetosDeValor.Embarcador.Carga.CteComplementarReemissao> ObterCtesComplementaresParaReemissao(int numeroMaximoRegistros)
        {
            string sql = @"
                SELECT CargaCTeComplementoInfo.CCC_CODIGO as CodigoComplementoInfo,
                       CargaCTeComplementoInfo.COC_CODIGO as CodigoOcorrencia,
                       CTe.CON_CODIGO as CodigoCte
                  FROM T_CARGA_CTE_COMPLEMENTO_INFO CargaCTeComplementoInfo
                  JOIN T_CTE CTe ON CTe.CON_CODIGO = CargaCTeComplementoInfo.CON_CODIGO
                 WHERE CargaCTeComplementoInfo.CCC_REEMITIR_CTE = 1
                   AND CTe.CON_STATUS = 'R'
                 ORDER BY CTe.CON_CODIGO 
                OFFSET 0 ROWS FETCH NEXT :numeroMaximoRegistros ROWS ONLY";

            var query = this.SessionNHiBernate.CreateSQLQuery(sql)
                .SetParameter("numeroMaximoRegistros", numeroMaximoRegistros);

            query.SetResultTransformer(new NHibernate.Transform.AliasToBeanResultTransformer(typeof(Dominio.ObjetosDeValor.Embarcador.Carga.CteComplementarReemissao)));

            return query.SetTimeout(600).List<Dominio.ObjetosDeValor.Embarcador.Carga.CteComplementarReemissao>();
        }

        public IList<Dominio.ObjetosDeValor.Embarcador.Carga.CteComplementarReemissao> ObterCtesComplementaresRejeitados(int numeroMaximoRegistros, int numeroMaximoTentativasReenvio, DateTime dataLimiteReenvio)
        {
            string sql = @"
                SELECT CargaCTeComplementoInfo.CCC_CODIGO as CodigoComplementoInfo,
                       CargaOcorrencia.COC_CODIGO as CodigoOcorrencia,
                       CTe.CON_CODIGO as CodigoCte
                  FROM T_CARGA_CTE_COMPLEMENTO_INFO CargaCTeComplementoInfo
                  JOIN T_CTE CTe ON CTe.CON_CODIGO = CargaCTeComplementoInfo.CON_CODIGO
                  JOIN T_CARGA_OCORRENCIA CargaOcorrencia ON CargaOcorrencia.COC_CODIGO = CargaCTeComplementoInfo.COC_CODIGO
                  LEFT JOIN T_ERRO_SEFAZ ErroSefaz ON ErroSefaz.ERR_CODIGO = CTe.ERR_CODIGO
                 WHERE COALESCE(CargaCTeComplementoInfo.CCC_REEMITIR_CTE, 0) = 0
                   AND CTe.CON_STATUS = 'R'
                   AND CTe.CON_DATAHORAEMISSAO >= :dataEmissaoMinima
                   AND CTe.CON_RETORNOCTEDATA <= :dataLimiteReenvio
                   AND CTe.CON_TENTATIVA_REENVIO <= :numeroMaximoTentativasReenvio
                   AND CargaOcorrencia.COC_SITUACAO_OCORRENCIA = :situacaoOcorrencia
                   AND (
                           ErroSefaz.ERR_CODIGO_ERRO = '8888' or  
                           ErroSefaz.ERR_CODIGO_ERRO = '678' or  
                           ErroSefaz.ERR_CODIGO_ERRO = '109' or  
                           ErroSefaz.ERR_CODIGO_ERRO = '105' or  
                           ErroSefaz.ERR_CODIGO_ERRO = '217' or 
                           CTe.CON_RETORNOCTE like '%INDEVIDO%' or 
                           CTe.CON_RETORNOCTE like '%HTTP: 0%' or 
                           CTe.CON_RETORNOCTE like '%HTTP: 404%' or 
                           CTe.CON_RETORNOCTE like '%HTTP: 503%' or 
                           CTe.CON_RETORNOCTE like '%paralisado%' or 
                           CTe.CON_RETORNOCTE like '%nao consta%' or 
                           CTe.CON_RETORNOCTE like '%lote em %'
                       )
                 ORDER BY CTe.CON_CODIGO 
                OFFSET 0 ROWS FETCH NEXT :numeroMaximoRegistros ROWS ONLY";

            var query = this.SessionNHiBernate.CreateSQLQuery(sql)
                .SetParameter("dataEmissaoMinima", DateTime.Now.AddDays(-7).Date)
                .SetParameter("dataLimiteReenvio", dataLimiteReenvio)
                .SetParameter("numeroMaximoRegistros", numeroMaximoRegistros)
                .SetParameter("numeroMaximoTentativasReenvio", numeroMaximoTentativasReenvio)
                .SetParameter("situacaoOcorrencia", SituacaoOcorrencia.PendenciaEmissao);

            query.SetResultTransformer(new NHibernate.Transform.AliasToBeanResultTransformer(typeof(Dominio.ObjetosDeValor.Embarcador.Carga.CteComplementarReemissao)));

            return query.SetTimeout(600).List<Dominio.ObjetosDeValor.Embarcador.Carga.CteComplementarReemissao>();
        }

        #endregion Métodos Públicos - Reemissão de CT-e
    }
}
