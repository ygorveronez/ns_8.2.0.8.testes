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
    public class CargaCTe : RepositorioBase<Dominio.Entidades.Embarcador.Cargas.CargaCTe>
    {
        public CargaCTe(UnitOfWork unitOfWork) : base(unitOfWork) { }
        public CargaCTe(UnitOfWork unitOfWork, CancellationToken cancellationToken) : base(unitOfWork, cancellationToken) { }

        #region Métodos Públicos

        public Dominio.Entidades.Embarcador.Cargas.CargaCTe BuscarPorChave(string chaveCTe)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.CargaCTe>();
            query = query.Where(obj => obj.CTe.Chave == chaveCTe);
            return query.FirstOrDefault();
        }

        public List<Dominio.Entidades.Embarcador.Cargas.CargaCTe> BuscarCargasPorProtocoloBookingOrdemServico(int protocoloIntegracaoCarga, int codigoBooking, string numeroBooking, int codigoOrdemServico, string numeroOrgemServico, string chaveCTe)
        {
            IQueryable<Dominio.Entidades.Embarcador.Cargas.CargaPedido> query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.CargaPedido>();

            if (protocoloIntegracaoCarga > 0)
                query = query.Where(o => o.Carga.Protocolo == protocoloIntegracaoCarga);
            if (codigoBooking > 0)
                query = query.Where(o => o.Pedido.CodigoBooking == codigoBooking.ToString("D"));
            if (!string.IsNullOrWhiteSpace(numeroBooking))
                query = query.Where(o => o.Pedido.NumeroBooking == numeroBooking);
            if (codigoOrdemServico > 0)
                query = query.Where(o => o.Pedido.CodigoOS == codigoOrdemServico.ToString("D"));
            if (!string.IsNullOrWhiteSpace(numeroOrgemServico))
                query = query.Where(o => o.Pedido.NumeroOS == numeroOrgemServico);

            IQueryable<Dominio.Entidades.Embarcador.Cargas.CargaCTe> queryCargaCTe = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.CargaCTe>();
            queryCargaCTe = queryCargaCTe.Where(o => query.Any(p => p.Carga == o.Carga));

            return queryCargaCTe.Where(o => o.Carga.SituacaoCarga != Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoCarga.Anulada && o.Carga.SituacaoCarga != Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoCarga.Cancelada).ToList();
        }

        public List<Dominio.Entidades.Embarcador.Cargas.CargaCTe> BuscarCargasCTePendentesTransmissao(int maximoRegistros, string propOrdenacao = "", string dirOrdenacao = "")
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.CargaCTe>();
            query = query.Where(obj => obj.SituacaoProcessamentoThread == SituacaoProcessamentoThread.Pendente);

            if (!string.IsNullOrWhiteSpace(propOrdenacao) && !string.IsNullOrWhiteSpace(dirOrdenacao))
                query = query.OrderBy(propOrdenacao + " " + dirOrdenacao);

            return query.Take(maximoRegistros).ToList();
        }

        public Dominio.Entidades.Embarcador.Cargas.CargaCTe BuscarPorNumeroSerieETipoDocumento(int numero, int serie, Dominio.Enumeradores.TipoDocumento tipoDocumento, string[] status = null, string[] chaveNFe = null)
        {
            IQueryable<Dominio.Entidades.Embarcador.Cargas.CargaCTe> query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.CargaCTe>();

            query = query.Where(o => o.CTe.ModeloDocumentoFiscal.TipoDocumentoEmissao == tipoDocumento && o.CTe.Numero == numero && o.CTe.Serie.Numero == serie && status.Contains(o.CTe.Status) && o.CTe.Documentos.Any(doc => chaveNFe.Contains(doc.ChaveNFE)));

            if (status != null && status.Count() > 0)
                query = query.Where(o => status.Contains(o.CTe.Status));

            if (chaveNFe != null && chaveNFe.Count() > 0)
                query = query.Where(o => o.CTe.Documentos.Any(doc => chaveNFe.Contains(doc.ChaveNFE)));

            return query.FirstOrDefault();
        }

        public int ContarCtesPorCarga(int carga)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.CargaCTe>();
            query = query.Where(obj => obj.Carga.Codigo == carga && obj.CTe != null);
            return query.Count();
        }

        public int ContarCtesPorCarga(List<int> codigoCargas)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.CargaCTe>();
            query = query.Where(obj => codigoCargas.Contains(obj.Carga.Codigo) && obj.CTe != null);
            return query.Count();
        }

        public bool ExistePorCargaConfiguracaoIntegrarXMLCTe(int codigoCarga)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.CargaCTe>();
            query = query.Where(obj => obj.Carga.Codigo == codigoCarga && obj.CTe.TomadorPagador.GrupoPessoas.HabilitarIntegracaoXmlCteMultiEmbarcador.Value == true);
            return query.Count() > 0;
        }

        public List<Dominio.Entidades.Cliente> ObterTomadoresPorCarga(int carga)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.CargaCTe>();
            query = query.Where(obj => obj.CargaOrigem.Codigo == carga && obj.CTe != null && obj.CTe.TomadorPagador != null);
            return query.Select(obj => obj.CTe.TomadorPagador.Cliente).Distinct().ToList();
        }

        public List<Dominio.Entidades.Cliente> ConsultarTomadorCarga(bool destinatariosDaCarga, int carga, string nome, double cpfCnpj, string tipo, Dominio.Entidades.Localidade localidade, string telefone, int codigoGrupoPessoas, int inicioRegistros, int maximoRegistros, string propOrdenacao = "", string dirOrdenacao = "asc")
        {
            var consultaCliente = ConsultarTomadorCarga(destinatariosDaCarga, carga, nome, cpfCnpj, tipo, localidade, telefone, codigoGrupoPessoas);

            return consultaCliente
                .OrderBy((propOrdenacao ?? "Nome") + (dirOrdenacao == "asc" ? " ascending" : " descending"))
                .Skip(inicioRegistros)
                .Take(maximoRegistros)
                .WithOptions(o => { o.SetTimeout(120); })
                .ToList();
        }

        public int ContarConsultarTomadorCarga(bool destinatariosDaCarga, int carga, string nome, double cpfCnpj, string tipo, Dominio.Entidades.Localidade localidade, string telefone, int codigoGrupoPessoas)
        {
            var consultaCliente = ConsultarTomadorCarga(destinatariosDaCarga, carga, nome, cpfCnpj, tipo, localidade, telefone, codigoGrupoPessoas);

            return consultaCliente.Count();
        }

        public List<Dominio.Entidades.Cliente> ConsultarDestinatarioCarga(bool destinatariosDaCarga, int carga, string nome, double cpfCnpj, string tipo, Dominio.Entidades.Localidade localidade, string telefone, int codigoGrupoPessoas, bool buscarPorCargaOrigem, int inicioRegistros, int maximoRegistros, string propOrdenacao = "", string dirOrdenacao = "asc")
        {
            var consultaCliente = ConsultarDestinatarioCarga(destinatariosDaCarga, carga, nome, cpfCnpj, tipo, localidade, telefone, codigoGrupoPessoas, buscarPorCargaOrigem);

            return consultaCliente
                .OrderBy((propOrdenacao ?? "Nome") + (dirOrdenacao == "asc" ? " ascending" : " descending"))
                .Skip(inicioRegistros)
                .Take(maximoRegistros)
                .WithOptions(o => { o.SetTimeout(120); })
                .ToList();
        }

        public int ContarConsultarDestinatarioCarga(bool destinatariosDaCarga, int carga, string nome, double cpfCnpj, string tipo, Dominio.Entidades.Localidade localidade, string telefone, int codigoGrupoPessoas, bool buscarPorCargaOrigem)
        {
            var consultaCliente = ConsultarDestinatarioCarga(destinatariosDaCarga, carga, nome, cpfCnpj, tipo, localidade, telefone, codigoGrupoPessoas, buscarPorCargaOrigem);

            return consultaCliente.Count();
        }

        public List<Dominio.Entidades.ConhecimentoDeTransporteEletronico> BuscarCTesAnterioresPorCarga(int carga)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.CargaCTe>();
            var resut = from obj in query
                        where obj.Carga.Codigo == carga && obj.CargaCTeFilialEmissora == null &&
      (obj.CTe.TipoServico == Dominio.Enumeradores.TipoServico.SubContratacao
      || obj.CTe.TipoServico == Dominio.Enumeradores.TipoServico.Redespacho
      || obj.CTe.TipoServico == Dominio.Enumeradores.TipoServico.RedIntermediario)
                        select obj;

            return resut.Select(obj => obj.CTe).ToList();
        }

        public List<Dominio.Entidades.Embarcador.Cargas.CargaCTe> BuscarPorCargas(List<int> codigoCargas)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.CargaCTe>();
            var resut = from obj in query
                        where codigoCargas.Contains(obj.Carga.Codigo) && obj.CTe.Status == "A" && obj.CargaCTeComplementoInfo == null
                        && obj.CTe.TipoCTE == Dominio.Enumeradores.TipoCTE.Normal
                        && (obj.CTe.ModeloDocumentoFiscal.TipoDocumentoEmissao == Dominio.Enumeradores.TipoDocumento.CTe
                        || obj.CTe.ModeloDocumentoFiscal.TipoDocumentoEmissao == Dominio.Enumeradores.TipoDocumento.NFSe)
                        select obj;

            return resut
                .Fetch(obj => obj.CTe).ThenFetch(obj => obj.Destinatario)
                .Fetch(obj => obj.CTe).ThenFetch(obj => obj.TomadorPagador).ThenFetch(o => o.Cliente)
                .ToList();
        }

        public List<Dominio.Entidades.Embarcador.Cargas.CargaCTe> BuscarTodosCTesPorCargasELancamentoManual(List<int> codigoCargas, int codigoLancamentoManual)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.CargaCTe>();
            var resut = from obj in query
                        where codigoCargas.Contains(obj.Carga.Codigo) && obj.LancamentoNFSManual.Codigo == codigoLancamentoManual
                        select obj;

            return resut.ToList();
        }

        public Dominio.Entidades.Embarcador.Cargas.CargaCTe BuscarPorUnicaCarga(int codigoCarga)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.CargaCTe>();
            var resut = from obj in query where codigoCarga == obj.Carga.Codigo select obj;

            return resut.FirstOrDefault();
        }

        public List<Dominio.Entidades.Embarcador.Cargas.CargaCTe> BuscarPorCargaSemCte(int codigoCarga)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.CargaCTe>();
            var resut = from obj in query
                        where codigoCarga == obj.Carga.Codigo && obj.CTe == null
                        select obj;

            return resut.ToList();
        }

        public List<Dominio.Entidades.Embarcador.Cargas.CargaCTe> BuscarSemCtePorPreCte(List<int> codigoPreCte)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.CargaCTe>();
            var resut = from obj in query
                        where codigoPreCte.Contains(obj.PreCTe.Codigo) && obj.CTe == null && obj.Carga.SituacaoCarga != SituacaoCarga.Cancelada && obj.Carga.SituacaoCarga != SituacaoCarga.Anulada
                        select obj;

            return resut.ToList();
        }

        public List<Dominio.Entidades.Embarcador.Cargas.CargaCTe> BuscarPorPeriodoEmissaoETransportador(int CodEmpresa, DateTime? dataInicial, DateTime? dataFinal)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.CargaCTe>();
            query = query.Where(obj => obj.CTe.Status == "A" && obj.CTe.Empresa.Codigo == CodEmpresa);

            if (dataInicial.HasValue)
                query = query.Where(obj => obj.CTe.DataEmissao.Value.Date >= dataInicial.Value.Date);
            if (dataFinal.HasValue)
                query = query.Where(obj => obj.CTe.DataEmissao.Value.Date < dataFinal.Value.Date.AddDays(1));

            return query.Fetch(obj => obj.CTe).ToList();
        }

        public List<int> BuscarCodigosCTeNFSePorCarga(int codigoCarga)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.CargaCTe>();
            query = from obj in query
                    where obj.CTe.ModeloDocumentoFiscal.TipoDocumentoEmissao == Dominio.Enumeradores.TipoDocumento.NFSe
                    && obj.Carga.Codigo == codigoCarga
                    select obj;

            return query
                .Select(obj => obj.CTe.Codigo)
                .ToList();
        }

        public List<Dominio.Entidades.Embarcador.Cargas.CargaCTe> BuscarAutorizadosPorCargas(IEnumerable<int> codigosCargas)
        {
            IQueryable<Dominio.Entidades.Embarcador.Cargas.CargaCTe> query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.CargaCTe>();

            query = query.Where(o => codigosCargas.Contains(o.Carga.Codigo) && o.CTe.Status == "A" && o.CargaCTeComplementoInfo == null && o.CTe.TipoCTE == Dominio.Enumeradores.TipoCTE.Normal && o.CargaCTeAgrupado == null);

            return query.Fetch(o => o.CTe).ToList();
        }

        public decimal BuscarValorFreteAutorizadoPorCargas(IEnumerable<int> codigosCargas)
        {
            IQueryable<Dominio.Entidades.Embarcador.Cargas.CargaCTe> query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.CargaCTe>();

            query = query.Where(o => codigosCargas.Contains(o.Carga.Codigo) && o.CTe.Status == "A" && o.CargaCTeComplementoInfo == null && o.CTe.TipoCTE == Dominio.Enumeradores.TipoCTE.Normal);

            return query.Sum(o => (decimal?)o.CTe.ValorFrete) ?? 0m;
        }

        public decimal BuscarValorFreteAutorizadoPorCarga(int codigoCarga)
        {
            IQueryable<Dominio.Entidades.Embarcador.Cargas.CargaCTe> query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.CargaCTe>();

            query = query.Where(o => o.Carga.Codigo == codigoCarga && o.CTe.Status == "A" && o.CargaCTeComplementoInfo == null && o.CTe.TipoCTE == Dominio.Enumeradores.TipoCTE.Normal);

            return query.Sum(o => (decimal?)o.CTe.ValorFrete) ?? 0m;
        }

        public List<Dominio.Entidades.Embarcador.Cargas.Carga> BuscarPorCTes(List<int> codigosCtes)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.CargaCTe>();
            var resut = from obj in query
                        where codigosCtes.Contains(obj.CTe.Codigo)
                        && (obj.Carga.CargaTransbordo == false || (obj.Carga.CargaTransbordo == true && obj.CargaCTeComplementoInfo != null))
                        select obj.Carga;

            return resut.Distinct().ToList();
        }

        public Dominio.ObjetosDeValor.Embarcador.Enumeradores.SistemaEmissor ObterSistemaEmissorCTeOcorrencia(int codigoCargaCTeComplementoInfo)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.CargaCTe>();

            query = query.Where(o => o.CargaCTeComplementoInfo.Codigo == codigoCargaCTeComplementoInfo);

            return query.Select(o => o.SistemaEmissor).FirstOrDefault();
        }

        public Dominio.Entidades.Embarcador.Cargas.CargaCTe BuscarPorCargaEChaveCTe(int carga, string chaveCTe)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.CargaCTe>();
            var resut = from obj in query where obj.Carga.Codigo == carga && obj.CTe.Chave == chaveCTe select obj;

            return resut.FirstOrDefault();
        }

        public List<Dominio.Entidades.Embarcador.Cargas.CargaCTe> BuscarCTesParaEmissaoSubContratacaoFilialEmissora(int carga, int empresaFilialEmissora)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.CargaCTe>();
            var resut = from obj in query where obj.Carga.Codigo == carga && obj.CTe.Empresa.Codigo == empresaFilialEmissora && obj.CargaCTeSubContratacaoFilialEmissora == null select obj;

            return resut.ToList();
        }

        public List<int> ObterCodigoCTeParaEmissao(int codigoCarga)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.CargaCTe>();

            var result = from obj in query where obj.Carga.Codigo == codigoCarga && (obj.CTe.Status == "R" || obj.CTe.Status == "S" || obj.CTe.Status == "F") && obj.CargaCTeComplementoInfo == null select obj.CTe.Codigo;

            return result.ToList();
        }

        public List<Dominio.Entidades.Embarcador.Cargas.CargaCTe> BuscarTodosPorCTe(int cte)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.CargaCTe>();
            var resut = from obj in query where obj.CTe.Codigo == cte select obj;

            return resut.ToList();
        }

        public List<Dominio.Entidades.Embarcador.Cargas.CargaCTe> BuscarCTesGeradosPorFatura(int fatura, List<int> emitentes, int inicioRegistros, int maximoRegistros)
        {
            IQueryable<Dominio.Entidades.Embarcador.Fatura.FaturaDocumento> queryFaturaDocumento = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Fatura.FaturaDocumento>();
            queryFaturaDocumento = queryFaturaDocumento.Where(obj => obj.Fatura.Codigo == fatura);

            IQueryable<Dominio.Entidades.Embarcador.Cargas.CargaCTe> query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.CargaCTe>();
            List<Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoCarga> situacoesCargaNaoEmitida = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoCargaHelper.ObterSituacoesCargaNaoEmitida();

            query = query.Where(obj => queryFaturaDocumento.Any(o => o.Documento.Carga == obj.Carga) && obj.CargaCTeComplementoInfo == null && !situacoesCargaNaoEmitida.Contains(obj.Carga.SituacaoCarga));

            if (emitentes != null && emitentes.Count > 0)
                query = query.Where(o => emitentes.Contains(o.CTe.Empresa.Codigo));

            return query
                .Fetch(obj => obj.CTe)
                .ThenFetch(obj => obj.CentroResultado)
                 .Fetch(obj => obj.CTe)
                .ThenFetch(obj => obj.CentroResultadoEscrituracao)
                .Fetch(obj => obj.CTe)
                .ThenFetch(obj => obj.CentroResultadoDestinatario)
                .Fetch(obj => obj.CTe)
                .ThenFetch(obj => obj.ModeloDocumentoFiscal)
                .Fetch(obj => obj.CTe)
                .ThenFetch(obj => obj.Remetente)
                .ThenFetch(obj => obj.Localidade)
                .Fetch(obj => obj.CTe)
                .ThenFetch(obj => obj.Destinatario)
                .ThenFetch(obj => obj.Localidade)
                .Fetch(obj => obj.CTe)
                .ThenFetch(obj => obj.Expedidor)
                .ThenFetch(obj => obj.Localidade)
                .Fetch(obj => obj.CTe)
                .ThenFetch(obj => obj.Recebedor)
                .ThenFetch(obj => obj.Localidade)
                .Fetch(obj => obj.CTe)
                .ThenFetch(obj => obj.OutrosTomador)
                .ThenFetch(obj => obj.Localidade)
                .Fetch(obj => obj.CTe)
                .ThenFetch(obj => obj.TomadorPagador)
                .ThenFetch(obj => obj.Localidade)
                .Fetch(obj => obj.CTe)
                .ThenFetch(obj => obj.TomadorPagador)
                .ThenFetch(obj => obj.GrupoPessoas)
                .Fetch(obj => obj.CTe)
                .ThenFetch(obj => obj.LocalidadeInicioPrestacao)
                .Fetch(obj => obj.CTe)
                .ThenFetch(obj => obj.LocalidadeTerminoPrestacao)
                .Skip(inicioRegistros).Take(maximoRegistros).ToList();
        }

        public int ContarCTesGeradosPorFatura(int fatura, List<int> emitentes)
        {
            IQueryable<Dominio.Entidades.Embarcador.Fatura.FaturaDocumento> queryFaturaDocumento = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Fatura.FaturaDocumento>();
            queryFaturaDocumento = queryFaturaDocumento.Where(obj => obj.Fatura.Codigo == fatura);

            IQueryable<Dominio.Entidades.Embarcador.Cargas.CargaCTe> query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.CargaCTe>();
            List<Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoCarga> situacoesCargaNaoEmitida = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoCargaHelper.ObterSituacoesCargaNaoEmitida();

            query = query.Where(obj => queryFaturaDocumento.Any(o => o.Documento.Carga == obj.Carga) && obj.CargaCTeComplementoInfo == null && !situacoesCargaNaoEmitida.Contains(obj.Carga.SituacaoCarga));

            if (emitentes != null && emitentes.Count > 0)
                query = query.Where(o => emitentes.Contains(o.CTe.Empresa.Codigo));

            return query.Count();
        }

        public List<Dominio.Entidades.Embarcador.Cargas.CargaCTe> BuscarCTesGeradosPorCarga(int carga, List<int> emitentes, int inicioRegistros, int maximoRegistros, AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware tipoServicoMultisoftware)
        {
            IQueryable<Dominio.Entidades.Embarcador.Cargas.CargaCTe> query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.CargaCTe>();
            List<Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoCarga> situacoesCargaNaoEmitida = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoCargaHelper.ObterSituacoesCargaNaoEmitida();

            if (emitentes?.Count > 0)
                query = query.Where(o => o.Carga.Codigo == carga);
            else
                query = query.Where(o => (o.Carga.Codigo == carga || o.CargaOrigem.Codigo == carga));

            query = query.Where(obj => obj.CargaCTeComplementoInfo == null && obj.CTe != null);

            if (emitentes != null && emitentes.Count > 0)
                query = query.Where(o => emitentes.Contains(o.CTe.Empresa.Codigo));
            else if (tipoServicoMultisoftware != AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiTMS)
                query = query.Where(obj => !situacoesCargaNaoEmitida.Contains(obj.Carga.SituacaoCarga));

            return query
                .Fetch(obj => obj.CTe)
                .ThenFetch(obj => obj.CentroResultado)
                .Fetch(obj => obj.CTe)
                .ThenFetch(obj => obj.CentroResultadoEscrituracao)
                .Fetch(obj => obj.CTe)
                .ThenFetch(obj => obj.CentroResultadoDestinatario)
                .Fetch(obj => obj.CTe)
                .ThenFetch(obj => obj.ModeloDocumentoFiscal)
                .Fetch(obj => obj.CTe)
                .ThenFetch(obj => obj.Remetente)
                .ThenFetch(obj => obj.Localidade)
                .Fetch(obj => obj.CTe)
                .ThenFetch(obj => obj.Destinatario)
                .ThenFetch(obj => obj.Localidade)
                .Fetch(obj => obj.CTe)
                .ThenFetch(obj => obj.Expedidor)
                .ThenFetch(obj => obj.Localidade)
                .Fetch(obj => obj.CTe)
                .ThenFetch(obj => obj.Recebedor)
                .ThenFetch(obj => obj.Localidade)
                .Fetch(obj => obj.CTe)
                .ThenFetch(obj => obj.OutrosTomador)
                .ThenFetch(obj => obj.Localidade)
                .Fetch(obj => obj.CTe)
                .ThenFetch(obj => obj.TomadorPagador)
                .ThenFetch(obj => obj.Localidade)
                .Fetch(obj => obj.CTe)
                .ThenFetch(obj => obj.TomadorPagador)
                .ThenFetch(obj => obj.GrupoPessoas)
                .Fetch(obj => obj.CTe)
                .ThenFetch(obj => obj.LocalidadeInicioPrestacao)
                .Fetch(obj => obj.CTe)
                .ThenFetch(obj => obj.LocalidadeTerminoPrestacao)
                .OrderBy(obj => obj.CTe.Codigo)
                .Skip(inicioRegistros).Take(maximoRegistros).ToList();
        }

        public int ContarCTesGeradosPorCarga(int carga, List<int> emitentes)
        {
            IQueryable<Dominio.Entidades.Embarcador.Cargas.CargaCTe> query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.CargaCTe>();
            List<Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoCarga> situacoesCargaNaoEmitida = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoCargaHelper.ObterSituacoesCargaNaoEmitida();

            query = query.Where(obj => (obj.Carga.Codigo == carga || obj.CargaOrigem.Codigo == carga) && obj.CargaCTeComplementoInfo == null && obj.CTe != null);

            if (emitentes != null && emitentes.Count > 0)
                query = query.Where(o => emitentes.Contains(o.CTe.Empresa.Codigo));
            else
                query = query.Where(obj => !situacoesCargaNaoEmitida.Contains(obj.Carga.SituacaoCarga));

            return query.Count();
        }

        public Dominio.Entidades.ConhecimentoDeTransporteEletronico BuscarPorNumeroSerieCTe(int numeroCTe, int serie, int codigoCarga)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.CargaCTe>();
            var resut = from obj in query where obj.CTe.Numero == numeroCTe && obj.CTe.Serie.Numero == serie && obj.Carga.Codigo == codigoCarga select obj;

            return resut.Select(c => c.CTe)?.FirstOrDefault() ?? null;
        }

        public List<Dominio.Entidades.Embarcador.Cargas.CargaCTe> BuscarPorNumeroCTe(int numeroCTe, int empresa, int inicioRegistros, int maximoRegistros)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.CargaCTe>();
            var resut = from obj in query where obj.CTe.Numero == numeroCTe select obj;

            if (empresa > 0)
                resut = resut.Where(obj => obj.CTe.Empresa.Codigo == empresa);

            return resut.ToList();
        }

        public Dominio.Entidades.Embarcador.Cargas.CargaCTe BuscarCTePorNumeroCTe(int numeroCTe)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.CargaCTe>();
            var resut = from obj in query where obj.CTe.Numero == numeroCTe select obj;

            return resut.FirstOrDefault();
        }

        public Task<List<int>> BuscarNumerosCTesPorCargaAsync(int codigoCarga)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.CargaCTe>();
            var resut = from obj in query where obj.Carga.Codigo == codigoCarga select obj.CTe.Numero;

            return resut.ToListAsync();
        }

        public int ContarPorNumeroCTe(int numeroCTe, int empresa)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.CargaCTe>();
            var resut = from obj in query where obj.CTe.Numero == numeroCTe select obj;

            if (empresa > 0)
                resut = resut.Where(obj => obj.CTe.Empresa.Codigo == empresa);

            return resut.Count();
        }

        public Dominio.Entidades.Embarcador.Cargas.CargaCTe BuscarPorCodigo(int codigo)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.CargaCTe>();
            var resut = from obj in query where obj.Codigo == codigo select obj;
            return resut
                   .Fetch(obj => obj.PreCTe)
                   .ThenFetch(obj => obj.Empresa)
                   .ThenFetch(obj => obj.Configuracao)
                   .FirstOrDefault();
        }

        public Dominio.Entidades.Embarcador.Cargas.CargaCTe BuscarPorCodigoCargaCTeFilialEmissora(int codigo)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.CargaCTe>();
            var resut = from obj in query where obj.CargaCTeFilialEmissora.Codigo == codigo select obj;
            return resut.FirstOrDefault();
        }

        public int ContarNaoCanceladosPorCarga(int codigoCarga)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.CargaCTe>();
            var resut = from obj in query where obj.Carga.Codigo == codigoCarga select obj;

            resut = resut.Where(obj => obj.CTe.Status != "C" && obj.CTe.Status != "I");

            return resut.Count();
        }

        public int ContarPendentesPorCarga(int codigoCarga)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.CargaCTe>();
            var resut = from obj in query where obj.Carga.Codigo == codigoCarga select obj;

            resut = resut.Where(obj => obj.CTe.Status != "A" && obj.CTe.Status != "I" && obj.CTe.Status != "C");

            return resut.Count();
        }

        public List<Dominio.Entidades.Embarcador.Cargas.CargaCTe> Consultar(int codigoCarga, int codigoCancelamentoCarga, int numeroCTe, int numeroNotaFiscal, bool cancelamentoUnitario, string propOrdenar, string dirOrdena, int inicio, int limite)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.CargaCTe>();
            query = query.Where(o => o.Carga.Codigo == codigoCarga && o.CTe != null && (o.CargaCancelamento.Codigo == codigoCancelamentoCarga || o.CargaCancelamento == null));

            if (numeroCTe > 0)
                query = query.Where(o => o.CTe.Numero == numeroCTe);

            if (numeroNotaFiscal > 0)
                query = query.Where(o => o.NotasFiscais.Any(nf => nf.PedidoXMLNotaFiscal.XMLNotaFiscal.Numero == numeroNotaFiscal));

            if (cancelamentoUnitario)
            {
                var queryCancelamento = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.CargaCancelamento>();
                int codigoCte = queryCancelamento.Where(x => x.Codigo == codigoCancelamentoCarga).Select(c => c.CTe.Codigo).FirstOrDefault();
                query = query.Where(o => o.CTe.Codigo == codigoCte);
            }

            return query.OrderBy(propOrdenar + " " + dirOrdena).Skip(inicio).Take(limite)
                        .Fetch(o => o.CTe).ToList();
        }

        public int ContarConsulta(int codigoCarga, int codigoCancelamentoCarga, int numeroCTe, int numeroNotaFiscal, bool cancelamentoUnitario)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.CargaCTe>();

            query = query.Where(o => o.Carga.Codigo == codigoCarga && o.CTe != null && (o.CargaCancelamento.Codigo == codigoCancelamentoCarga || o.CargaCancelamento == null));

            if (numeroCTe > 0)
                query = query.Where(o => o.CTe.Numero == numeroCTe);

            if (numeroNotaFiscal > 0)
                query = query.Where(o => o.NotasFiscais.Any(nf => nf.PedidoXMLNotaFiscal.XMLNotaFiscal.Numero == numeroNotaFiscal));

            if (cancelamentoUnitario)
            {
                var queryCancelamento = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.CargaCancelamento>();
                int codigoCte = queryCancelamento.Where(x => x.Codigo == codigoCancelamentoCarga).Select(c => c.CTe.Codigo).FirstOrDefault();
                query = query.Where(o => o.CTe.Codigo == codigoCte);
            }

            return query.Count();
        }

        public List<Dominio.Entidades.Embarcador.Cargas.CargaCTe> ConsultarParaTransbordo(int codigoCarga, int numeroCTe, int numeroNotaFiscal, string numeroPedidoEmbarcador, Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoCarga[] situacoes, string propOrdenar, string dirOrdena, int inicio, int limite)
        {
            IQueryable<Dominio.Entidades.Embarcador.Cargas.CargaCTe> query = ObterQueryConsultaParaTransbordo(codigoCarga, numeroCTe, numeroNotaFiscal, numeroPedidoEmbarcador, situacoes, propOrdenar, dirOrdena, inicio, limite);

            return query.Fetch(o => o.CTe).ToList();
        }

        public int ContarConsultaParaTransbordo(int codigoCarga, int numeroCTe, int numeroNotaFiscal, string numeroPedidoEmbarcador, Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoCarga[] situacoes)
        {
            IQueryable<Dominio.Entidades.Embarcador.Cargas.CargaCTe> query = ObterQueryConsultaParaTransbordo(codigoCarga, numeroCTe, numeroNotaFiscal, numeroPedidoEmbarcador, situacoes);

            return query.Count();
        }

        public List<Dominio.Entidades.Embarcador.Pedidos.TipoTerminalImportacao> ConsultarTerminaisDestino(int codigoPedidoViagemNavio, int codigoTerminalOrigem, Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoCarga[] situacoes)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.CargaCTe>();

            query = query.Where(o => o.CTe != null && !o.Carga.CargaTransbordo && o.CTe.TipoCTE == Dominio.Enumeradores.TipoCTE.Normal && o.CTe.TipoModal == Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoModal.Multimodal && o.CTe.Status == "A" && o.CTe.TerminalDestino != null);

            if (codigoPedidoViagemNavio > 0)
                query = query.Where(o => o.CTe.Viagem.Codigo == codigoPedidoViagemNavio);

            if (situacoes != null && situacoes.Length > 0)
                query = query.Where(o => situacoes.Contains(o.Carga.SituacaoCarga));

            if (codigoTerminalOrigem > 0)
                query = query.Where(o => o.CTe.TerminalOrigem.Codigo == codigoTerminalOrigem);

            return query.Fetch(o => o.CTe).Select(o => o.CTe.TerminalDestino).Distinct().ToList();
        }

        public List<Dominio.Entidades.Embarcador.Pedidos.TipoTerminalImportacao> ConsultarTerminalDestinoTransbordo(int codigoPedidoViagemNavio, Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoCarga[] situacoes)
        {
            var queryPedidoTransbordo = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Pedidos.PedidoTransbordo>();
            if (codigoPedidoViagemNavio > 0)
                queryPedidoTransbordo = queryPedidoTransbordo.Where(o => o.PedidoViagemNavio.Codigo == codigoPedidoViagemNavio);

            var queryCargaCTe = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.CargaCTe>();
            queryCargaCTe = queryCargaCTe.Where(o => o.CTe != null && !o.Carga.CargaTransbordo && o.CTe.TipoCTE == Dominio.Enumeradores.TipoCTE.Normal && o.CTe.TipoModal == Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoModal.Multimodal && o.CTe.Status == "A");
            if (situacoes != null && situacoes.Length > 0)
                queryCargaCTe = queryCargaCTe.Where(o => situacoes.Contains(o.Carga.SituacaoCarga));

            var queryCargaPedido = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.CargaPedido>();
            queryCargaPedido = queryCargaPedido.Where(o => queryCargaCTe.Any(c => c.Carga.Codigo == o.Carga.Codigo));

            queryPedidoTransbordo = queryPedidoTransbordo.Where(o => queryCargaPedido.Any(p => p.Pedido.Codigo == o.Pedido.Codigo));

            return queryPedidoTransbordo.Timeout(7000).Select(o => o.Terminal).Distinct().ToList();
        }

        public List<Dominio.Entidades.Embarcador.Cargas.Carga> ConsultarCargaMultiModal(bool somenteCargaPerigosa, string numeroBooking, Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoPropostaMultimodal? tipoPropostaMultimodal,
            int codigoPedidoViagemNavio, int codigoTerminalOrigem, int codigoTerminalDestino, bool buscaCargaMTL = false)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.CargaPedido>();

            query = query.Where(o => !o.Carga.CargaTransbordo);

            query = query.Where(obj => obj.Carga.SituacaoCarga != Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoCarga.Cancelada && obj.Carga.SituacaoCarga != Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoCarga.Anulada);

            if (!buscaCargaMTL)
                query = query.Where(obj => obj.Carga.SituacaoCarga == Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoCarga.NaLogistica
                    || obj.Carga.SituacaoCarga == Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoCarga.Nova
                    || obj.Carga.SituacaoCarga == Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoCarga.AgNFe
                    || obj.Carga.SituacaoCarga == Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoCarga.PendeciaDocumentos
                    || obj.Carga.SituacaoCarga == Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoCarga.CalculoFrete);

            if (codigoPedidoViagemNavio > 0)
                query = query.Where(o => o.Pedido.PedidoViagemNavio.Codigo == codigoPedidoViagemNavio);

            if (codigoTerminalOrigem > 0)
                query = query.Where(o => o.Pedido.TerminalOrigem.Codigo == codigoTerminalOrigem);

            if (codigoTerminalDestino > 0)
                query = query.Where(o => o.Pedido.TerminalDestino.Codigo == codigoTerminalDestino);

            if (!string.IsNullOrWhiteSpace(numeroBooking))
                query = query.Where(o => o.Pedido.NumeroBooking == numeroBooking);

            var queryCargaPedido = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.CargaPedido>();
            if (somenteCargaPerigosa)
            {
                queryCargaPedido = queryCargaPedido.Where(o => o.Pedido.PossuiCargaPerigosa == true);
                query = query.Where(o => queryCargaPedido.Any(p => p.Carga.Codigo == o.Carga.Codigo));
            }

            if (tipoPropostaMultimodal.HasValue)
            {
                if (tipoPropostaMultimodal == Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoPropostaMultimodal.CargaFracionada)
                {
                    queryCargaPedido = queryCargaPedido.Where(o => o.TipoPropostaMultimodal == Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoPropostaMultimodal.CargaFracionada);
                    query = query.Where(o => queryCargaPedido.Any(p => p.Carga.Codigo == o.Codigo));
                }
                else
                {
                    queryCargaPedido = queryCargaPedido.Where(o => o.TipoPropostaMultimodal != Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoPropostaMultimodal.CargaFracionada);
                    query = query.Where(o => queryCargaPedido.Any(p => p.Carga.Codigo == o.Codigo));
                }
            }

            return query.Select(o => o.Carga).ToList();
        }

        public List<Dominio.Entidades.Embarcador.Cargas.Carga> ConsultarCargaMultiModalTransbordo(bool somenteCargaPerigosa, string numeroBooking, Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoPropostaMultimodal? tipoPropostaMultimodal, int codigoPedidoViagemNavio, int codigoTerminalOrigem, int codigoTerminalDestino)
        {
            var queryPedidoTransbordo = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Pedidos.PedidoTransbordo>();
            if (codigoPedidoViagemNavio > 0)
                queryPedidoTransbordo = queryPedidoTransbordo.Where(o => o.PedidoViagemNavio.Codigo == codigoPedidoViagemNavio);
            if (codigoTerminalDestino > 0)
                queryPedidoTransbordo = queryPedidoTransbordo.Where(o => o.Terminal.Codigo == codigoTerminalDestino);

            if (!string.IsNullOrWhiteSpace(numeroBooking))
                queryPedidoTransbordo = queryPedidoTransbordo.Where(o => o.Pedido.NumeroBooking == numeroBooking);

            var queryCargaPedido = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.CargaPedido>();
            queryCargaPedido = queryCargaPedido.Where(o => queryPedidoTransbordo.Any(p => p.Pedido.Codigo == o.Pedido.Codigo));

            queryCargaPedido = queryCargaPedido.Where(o => !o.Carga.CargaTransbordo);

            queryCargaPedido = queryCargaPedido.Where(obj => obj.Carga.SituacaoCarga == Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoCarga.NaLogistica
            || obj.Carga.SituacaoCarga == Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoCarga.Nova
            || obj.Carga.SituacaoCarga == Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoCarga.AgNFe
            || obj.Carga.SituacaoCarga == Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoCarga.PendeciaDocumentos
            || obj.Carga.SituacaoCarga == Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoCarga.CalculoFrete);

            if (codigoPedidoViagemNavio > 0)
                queryCargaPedido = queryCargaPedido.Where(o => o.Pedido.PedidoViagemNavio.Codigo == codigoPedidoViagemNavio);

            if (codigoTerminalOrigem > 0)
                queryCargaPedido = queryCargaPedido.Where(o => o.Pedido.TerminalOrigem.Codigo == codigoTerminalOrigem);

            if (codigoTerminalDestino > 0)
                queryCargaPedido = queryCargaPedido.Where(o => o.Pedido.TerminalDestino.Codigo == codigoTerminalDestino);

            if (!string.IsNullOrWhiteSpace(numeroBooking))
                queryCargaPedido = queryCargaPedido.Where(o => o.Pedido.NumeroBooking == numeroBooking);

            if (somenteCargaPerigosa)
                queryCargaPedido = queryCargaPedido.Where(o => o.Pedido.PossuiCargaPerigosa == true);

            if (tipoPropostaMultimodal.HasValue)
            {
                if (tipoPropostaMultimodal == Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoPropostaMultimodal.CargaFracionada)
                    queryCargaPedido = queryCargaPedido.Where(o => o.TipoPropostaMultimodal == Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoPropostaMultimodal.CargaFracionada);
                else
                    queryCargaPedido = queryCargaPedido.Where(o => o.TipoPropostaMultimodal != Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoPropostaMultimodal.CargaFracionada);
            }

            return queryCargaPedido.Timeout(7000).Select(o => o.Carga).ToList();
        }

        public bool BookingPendenteEmissaoSVM(bool somenteCargaPerigosa, string numeroBooking, Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoPropostaMultimodal? tipoPropostaMultimodal, int codigoPedidoViagemNavio, int codigoTerminalOrigem, int codigoTerminalDestino, Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoCarga[] situacoes)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.CargaPedido>();
            query = query.Where(o => o.Carga.CargaSVM == true);

            if (codigoPedidoViagemNavio > 0)
                query = query.Where(o => o.Pedido.PedidoViagemNavio.Codigo == codigoPedidoViagemNavio);

            if (situacoes != null && situacoes.Length > 0)
                query = query.Where(o => situacoes.Contains(o.Carga.SituacaoCarga));

            if (codigoTerminalOrigem > 0)
                query = query.Where(o => o.Pedido.TerminalOrigem.Codigo == codigoTerminalOrigem);

            if (codigoTerminalDestino > 0)
                query = query.Where(o => o.Pedido.TerminalDestino.Codigo == codigoTerminalDestino);

            if (!string.IsNullOrWhiteSpace(numeroBooking))
                query = query.Where(o => o.Pedido.NumeroBooking == numeroBooking);

            if (somenteCargaPerigosa)
                query = query.Where(o => o.Pedido.PossuiCargaPerigosa == true);

            //if (tipoPropostaMultimodal.HasValue)
            //{
            //    if (tipoPropostaMultimodal == Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoPropostaMultimodal.CargaFracionada)
            //    {
            //        query = query.Where(o => o.TipoPropostaMultimodal == Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoPropostaMultimodal.CargaFracionada);                    
            //    }
            //    else 
            //    {
            //        query = query.Where(o => o.TipoPropostaMultimodal != Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoPropostaMultimodal.CargaFracionada);
            //    }
            //}

            return query.Any();
        }

        public bool BookingPendenteEmissaoSVMTransbordo(bool somenteCargaPerigosa, string numeroBooking, Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoPropostaMultimodal? tipoPropostaMultimodal, int codigoPedidoViagemNavio, int codigoTerminalOrigem, int codigoTerminalDestino, Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoCarga[] situacoes)
        {
            var queryPedidoTransbordo = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Pedidos.PedidoTransbordo>();
            if (codigoPedidoViagemNavio > 0)
                queryPedidoTransbordo = queryPedidoTransbordo.Where(o => o.PedidoViagemNavio.Codigo == codigoPedidoViagemNavio);
            if (codigoTerminalDestino > 0)
                queryPedidoTransbordo = queryPedidoTransbordo.Where(o => o.Terminal.Codigo == codigoTerminalDestino);

            if (!string.IsNullOrWhiteSpace(numeroBooking))
                queryPedidoTransbordo = queryPedidoTransbordo.Where(o => o.Pedido.NumeroBooking == numeroBooking);

            var queryCargaPedido = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.CargaPedido>();
            queryCargaPedido = queryCargaPedido.Where(o => queryPedidoTransbordo.Any(p => p.Pedido.Codigo == o.Pedido.Codigo));
            queryCargaPedido = queryCargaPedido.Where(o => o.Carga.CargaSVM == true);

            if (situacoes != null && situacoes.Length > 0)
                queryCargaPedido = queryCargaPedido.Where(o => situacoes.Contains(o.Carga.SituacaoCarga));

            if (somenteCargaPerigosa)
                queryCargaPedido = queryCargaPedido.Where(o => o.Pedido.PossuiCargaPerigosa == true);

            //if (tipoPropostaMultimodal.HasValue)
            //{
            //    if (tipoPropostaMultimodal == Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoPropostaMultimodal.CargaFracionada)
            //        queryCargaPedido = queryCargaPedido.Where(o => o.TipoPropostaMultimodal == Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoPropostaMultimodal.CargaFracionada);
            //    else
            //        queryCargaPedido = queryCargaPedido.Where(o => o.TipoPropostaMultimodal != Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoPropostaMultimodal.CargaFracionada);
            //}

            return queryCargaPedido.Any();
        }

        public List<Dominio.Entidades.Embarcador.Cargas.CargaCTe> ConsultarAquaviarioMercante(int codigoPedidoViagemNavio, int codigoTerminalOrigem, int codigoTerminalDestino, bool somenteCargaPerigosa, bool semCargaPerigosa, bool comConhecimentosCancelados, bool semNumeroManifesto, int codigoBalsa = 0)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.CargaCTe>();

            query = query.Where(o => o.CTe != null && o.CTe.NaoEnviarParaMercante != true && !o.Carga.CargaTransbordo && o.CTe.TipoCTE == Dominio.Enumeradores.TipoCTE.Normal && o.CTe.TipoModal == Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoModal.Aquaviario);
            if (!comConhecimentosCancelados)
                query = query.Where(o => o.CTe != null && o.CTe.Status == "A");

            if (semNumeroManifesto)
                query = query.Where(o => o.CTe.NumeroManifesto == null || o.CTe.NumeroManifesto == "");

            if (codigoPedidoViagemNavio > 0)
                query = query.Where(o => o.CTe.Viagem.Codigo == codigoPedidoViagemNavio);

            if (codigoTerminalOrigem > 0)
                query = query.Where(o => o.CTe.TerminalOrigem.Codigo == codigoTerminalOrigem);

            if (codigoTerminalDestino > 0)
                query = query.Where(o => o.CTe.TerminalDestino.Codigo == codigoTerminalDestino);

            var queryCargaPedido = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.CargaPedido>();
            queryCargaPedido = queryCargaPedido.Where(o => o.TipoPropostaMultimodal != Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoPropostaMultimodal.Feeder
            && o.TipoPropostaMultimodal != Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoPropostaMultimodal.NoShowCabotagem
            && o.TipoPropostaMultimodal != Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoPropostaMultimodal.FaturamentoContabilidade
            && o.TipoPropostaMultimodal != Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoPropostaMultimodal.DemurrageCabotagem
            && o.TipoPropostaMultimodal != Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoPropostaMultimodal.DetentionCabotagem
            && o.TipoPropostaMultimodal != Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoPropostaMultimodal.TAkePayCabotagem
            && o.TipoPropostaMultimodal != Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoPropostaMultimodal.TakePayFeeder && o.TipoPropostaMultimodal != Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoPropostaMultimodal.VAS);

            if (somenteCargaPerigosa)
                queryCargaPedido = queryCargaPedido.Where(o => o.Pedido.PossuiCargaPerigosa == true);
            else if (semCargaPerigosa)
                queryCargaPedido = queryCargaPedido.Where(o => o.Pedido.PossuiCargaPerigosa == false);

            var queryPedidoTransbordo = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Pedidos.PedidoTransbordo>();
            queryCargaPedido = queryCargaPedido.Where(o => !queryPedidoTransbordo.Any(p => p.Pedido.Codigo == o.Pedido.Codigo));

            query = query.Where(o => queryCargaPedido.Any(p => p.Carga.Codigo == o.Carga.Codigo));

            if (codigoBalsa > 0)
                query = query.Where(o => o.CTe.Balsa.Codigo == codigoBalsa);

            return query.Timeout(7000).Fetch(o => o.CTe).ToList();
        }

        public List<Dominio.Entidades.Embarcador.Cargas.CargaCTe> ConsultarCTesSemNumeroManifesto(int codigoPedidoViagemNavio, int codigoTerminalOrigem, int codigoTerminalDestino, bool somenteCargaPerigosa, bool semCargaPerigosa, int codigoTerminalTransbordo, int codigoPedidoViagemNavioTransbordo, bool comConhecimentosCancelados)
        {
            var queryPedidoTransbordo = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Pedidos.PedidoTransbordo>();

            if (codigoTerminalTransbordo > 0)
                queryPedidoTransbordo = queryPedidoTransbordo.Where(o => o.Terminal.Codigo == codigoTerminalTransbordo);

            if (codigoPedidoViagemNavioTransbordo > 0)
                queryPedidoTransbordo = queryPedidoTransbordo.Where(o => o.PedidoViagemNavio.Codigo == codigoPedidoViagemNavioTransbordo);

            var queryCargaCTe = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.CargaCTe>();
            queryCargaCTe = queryCargaCTe.Where(o => o.CTe != null && !o.Carga.CargaTransbordo && o.CTe.TipoCTE == Dominio.Enumeradores.TipoCTE.Normal && o.CTe.TipoModal == Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoModal.Aquaviario);
            if (!comConhecimentosCancelados)
                queryCargaCTe = queryCargaCTe.Where(o => o.CTe != null && o.CTe.Status == "A");

            queryCargaCTe = queryCargaCTe.Where(o => o.CTe.NumeroManifesto == "" || o.CTe.NumeroManifesto == null);

            if (codigoPedidoViagemNavio > 0)
                queryCargaCTe = queryCargaCTe.Where(o => o.CTe.Viagem.Codigo == codigoPedidoViagemNavio);

            if (codigoTerminalOrigem > 0)
                queryCargaCTe = queryCargaCTe.Where(o => o.CTe.TerminalOrigem.Codigo == codigoTerminalOrigem);

            if (codigoTerminalDestino > 0)
                queryCargaCTe = queryCargaCTe.Where(o => o.CTe.TerminalDestino.Codigo == codigoTerminalDestino);


            var queryCargaPedido = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.CargaPedido>();
            queryCargaPedido = queryCargaPedido.Where(o => queryPedidoTransbordo.Any(p => p.Pedido.Codigo == o.Pedido.Codigo));
            queryCargaPedido = queryCargaPedido.Where(o => o.TipoPropostaMultimodal != Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoPropostaMultimodal.Feeder
            && o.TipoPropostaMultimodal != Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoPropostaMultimodal.NoShowCabotagem
            && o.TipoPropostaMultimodal != Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoPropostaMultimodal.FaturamentoContabilidade
            && o.TipoPropostaMultimodal != Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoPropostaMultimodal.DemurrageCabotagem
            && o.TipoPropostaMultimodal != Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoPropostaMultimodal.DetentionCabotagem
            && o.TipoPropostaMultimodal != Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoPropostaMultimodal.TAkePayCabotagem
            && o.TipoPropostaMultimodal != Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoPropostaMultimodal.TakePayFeeder && o.TipoPropostaMultimodal != Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoPropostaMultimodal.VAS);

            if (somenteCargaPerigosa)
                queryCargaPedido = queryCargaPedido.Where(o => o.Pedido.PossuiCargaPerigosa == true);
            else if (semCargaPerigosa)
                queryCargaPedido = queryCargaPedido.Where(o => o.Pedido.PossuiCargaPerigosa == false);

            queryCargaCTe = queryCargaCTe.Where(o => queryCargaPedido.Any(c => c.Carga.Codigo == o.Carga.Codigo));

            return queryCargaCTe.Timeout(7000).Fetch(o => o.CTe).ToList();
        }

        public List<Dominio.Entidades.Embarcador.Cargas.CargaCTe> ConsultarAquaviarioMercanteTransbordo(int codigoPedidoViagemNavio, int codigoTerminalOrigem, int codigoTerminalDestino, bool somenteCargaPerigosa, bool semCargaPerigosa, int codigoTerminalTransbordo, int codigoPedidoViagemNavioTransbordo, bool comConhecimentosCancelados, bool semNumeroManifesto, int codigoBalsa = 0)
        {
            var queryPedidoTransbordo = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Pedidos.PedidoTransbordo>();

            if (codigoTerminalTransbordo > 0)
                queryPedidoTransbordo = queryPedidoTransbordo.Where(o => o.Terminal.Codigo == codigoTerminalTransbordo);

            if (codigoPedidoViagemNavioTransbordo > 0)
                queryPedidoTransbordo = queryPedidoTransbordo.Where(o => o.PedidoViagemNavio.Codigo == codigoPedidoViagemNavioTransbordo);

            var queryCargaCTe = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.CargaCTe>();
            queryCargaCTe = queryCargaCTe.Where(o => o.CTe != null && o.CTe.NaoEnviarParaMercante != true && !o.Carga.CargaTransbordo && o.CTe.TipoCTE == Dominio.Enumeradores.TipoCTE.Normal && o.CTe.TipoModal == Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoModal.Aquaviario);
            if (!comConhecimentosCancelados)
                queryCargaCTe = queryCargaCTe.Where(o => o.CTe != null && o.CTe.Status == "A");

            if (semNumeroManifesto)
                queryCargaCTe = queryCargaCTe.Where(o => o.CTe.NumeroManifesto == null || o.CTe.NumeroManifesto == "");

            if (codigoPedidoViagemNavio > 0)
                queryCargaCTe = queryCargaCTe.Where(o => o.CTe.Viagem.Codigo == codigoPedidoViagemNavio);

            if (codigoTerminalOrigem > 0)
                queryCargaCTe = queryCargaCTe.Where(o => o.CTe.TerminalOrigem.Codigo == codigoTerminalOrigem);

            if (codigoTerminalDestino > 0)
                queryCargaCTe = queryCargaCTe.Where(o => o.CTe.TerminalDestino.Codigo == codigoTerminalDestino);


            var queryCargaPedido = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.CargaPedido>();
            queryCargaPedido = queryCargaPedido.Where(o => queryPedidoTransbordo.Any(p => p.Pedido.Codigo == o.Pedido.Codigo));
            queryCargaPedido = queryCargaPedido.Where(o => o.TipoPropostaMultimodal != Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoPropostaMultimodal.Feeder
            && o.TipoPropostaMultimodal != Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoPropostaMultimodal.NoShowCabotagem
            && o.TipoPropostaMultimodal != Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoPropostaMultimodal.FaturamentoContabilidade
            && o.TipoPropostaMultimodal != Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoPropostaMultimodal.DemurrageCabotagem
            && o.TipoPropostaMultimodal != Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoPropostaMultimodal.DetentionCabotagem
            && o.TipoPropostaMultimodal != Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoPropostaMultimodal.TAkePayCabotagem
            && o.TipoPropostaMultimodal != Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoPropostaMultimodal.TakePayFeeder && o.TipoPropostaMultimodal != Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoPropostaMultimodal.VAS);

            if (somenteCargaPerigosa)
                queryCargaPedido = queryCargaPedido.Where(o => o.Pedido.PossuiCargaPerigosa == true);
            else if (semCargaPerigosa)
                queryCargaPedido = queryCargaPedido.Where(o => o.Pedido.PossuiCargaPerigosa == false);

            queryCargaCTe = queryCargaCTe.Where(o => queryCargaPedido.Any(c => c.Carga.Codigo == o.Carga.Codigo));

            if (codigoBalsa > 0)
                queryCargaCTe = queryCargaCTe.Where(o => o.CTe.Balsa.Codigo == codigoBalsa);

            return queryCargaCTe.Timeout(7000).Fetch(o => o.CTe).ToList();
        }

        public List<Dominio.Entidades.Embarcador.Cargas.Carga> ConsultarAquaviarioMercantePendenteAutorizacao(int codigoPedidoViagemNavio, int codigoTerminalOrigem, int codigoTerminalDestino, bool somenteCargaPerigosa, bool semCargaPerigosa)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.CargaPedido>();

            query = query.Where(o => o.Carga.SituacaoCarga != Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoCarga.Cancelada && o.Carga.SituacaoCarga != Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoCarga.EmTransporte
            && o.Carga.SituacaoCarga != Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoCarga.Anulada && o.Carga.SituacaoCarga != Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoCarga.Encerrada);

            if (codigoPedidoViagemNavio > 0)
                query = query.Where(o => o.Pedido.PedidoViagemNavio.Codigo == codigoPedidoViagemNavio);

            if (codigoTerminalOrigem > 0)
                query = query.Where(o => o.Pedido.TerminalOrigem.Codigo == codigoTerminalOrigem);

            if (codigoTerminalDestino > 0)
                query = query.Where(o => o.Pedido.TerminalDestino.Codigo == codigoTerminalDestino);

            query = query.Where(o => o.TipoPropostaMultimodal != Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoPropostaMultimodal.Feeder
            && o.TipoPropostaMultimodal != Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoPropostaMultimodal.NoShowCabotagem
            && o.TipoPropostaMultimodal != Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoPropostaMultimodal.FaturamentoContabilidade
            && o.TipoPropostaMultimodal != Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoPropostaMultimodal.DemurrageCabotagem
            && o.TipoPropostaMultimodal != Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoPropostaMultimodal.DetentionCabotagem
            && o.TipoPropostaMultimodal != Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoPropostaMultimodal.TAkePayCabotagem
            && o.TipoPropostaMultimodal != Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoPropostaMultimodal.TakePayFeeder && o.TipoPropostaMultimodal != Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoPropostaMultimodal.VAS);

            if (somenteCargaPerigosa)
                query = query.Where(o => o.Pedido.PossuiCargaPerigosa == true);
            else if (semCargaPerigosa)
                query = query.Where(o => o.Pedido.PossuiCargaPerigosa == false);

            var queryPedidoTransbordo = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Pedidos.PedidoTransbordo>();
            query = query.Where(o => !queryPedidoTransbordo.Any(p => p.Pedido.Codigo == o.Codigo));

            return query.Timeout(7000).Select(o => o.Carga).ToList();
        }

        public List<Dominio.Entidades.Embarcador.Cargas.Carga> ConsultarAquaviarioMercanteTransbordoPendenteAutorizacao(int codigoPedidoViagemNavio, int codigoTerminalOrigem, int codigoTerminalDestino, bool somenteCargaPerigosa, bool semCargaPerigosa, int codigoTerminalTransbordo, int codigoPedidoViagemNavioTransbordo)
        {
            var queryPedidoTransbordo = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Pedidos.PedidoTransbordo>();

            if (codigoTerminalTransbordo > 0)
                queryPedidoTransbordo = queryPedidoTransbordo.Where(o => o.Terminal.Codigo == codigoTerminalTransbordo);

            if (codigoPedidoViagemNavioTransbordo > 0)
                queryPedidoTransbordo = queryPedidoTransbordo.Where(o => o.PedidoViagemNavio.Codigo == codigoPedidoViagemNavio);

            var queryCargaPedido = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.CargaPedido>();

            queryCargaPedido = queryCargaPedido.Where(o => queryPedidoTransbordo.Any(p => p.Pedido.Codigo == o.Pedido.Codigo));
            queryCargaPedido = queryCargaPedido.Where(o => o.TipoPropostaMultimodal != Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoPropostaMultimodal.Feeder
            && o.TipoPropostaMultimodal != Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoPropostaMultimodal.NoShowCabotagem
            && o.TipoPropostaMultimodal != Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoPropostaMultimodal.FaturamentoContabilidade
            && o.TipoPropostaMultimodal != Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoPropostaMultimodal.DemurrageCabotagem
            && o.TipoPropostaMultimodal != Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoPropostaMultimodal.DetentionCabotagem
            && o.TipoPropostaMultimodal != Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoPropostaMultimodal.TAkePayCabotagem
            && o.TipoPropostaMultimodal != Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoPropostaMultimodal.TakePayFeeder && o.TipoPropostaMultimodal != Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoPropostaMultimodal.VAS);

            if (somenteCargaPerigosa)
                queryCargaPedido = queryCargaPedido.Where(o => o.Pedido.PossuiCargaPerigosa == true);
            else if (semCargaPerigosa)
                queryCargaPedido = queryCargaPedido.Where(o => o.Pedido.PossuiCargaPerigosa == false);

            if (codigoPedidoViagemNavio > 0)
                queryCargaPedido = queryCargaPedido.Where(o => o.Pedido.PedidoViagemNavio.Codigo == codigoPedidoViagemNavio);

            if (codigoTerminalOrigem > 0)
                queryCargaPedido = queryCargaPedido.Where(o => o.Pedido.TerminalOrigem.Codigo == codigoTerminalOrigem);

            if (codigoTerminalDestino > 0)
                queryCargaPedido = queryCargaPedido.Where(o => o.Pedido.TerminalDestino.Codigo == codigoTerminalDestino);

            queryCargaPedido = queryCargaPedido.Where(o => o.Carga.SituacaoCarga != Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoCarga.Cancelada && o.Carga.SituacaoCarga != Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoCarga.EmTransporte
            && o.Carga.SituacaoCarga != Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoCarga.Anulada && o.Carga.SituacaoCarga != Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoCarga.Encerrada);

            return queryCargaPedido.Timeout(7000).Select(o => o.Carga).ToList();
        }

        public Dominio.Entidades.Embarcador.Pedidos.Pedido ConsultarPedidoMultiModal(bool somenteCargaPerigosa, string numeroBooking, Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoPropostaMultimodal? tipoPropostaMultimodal, int codigoPedidoViagemNavio, int codigoTerminalOrigem, int codigoTerminalDestino, Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoCarga[] situacoes, bool conhecimentosPendentesAutorizacao = false, bool somenteComContainer = true)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.CargaCTe>();

            query = query.Where(o => o.CTe != null && !o.Carga.CargaTransbordo && o.CTe.TipoCTE == Dominio.Enumeradores.TipoCTE.Normal && o.CTe.TipoModal == Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoModal.Multimodal);

            if (!conhecimentosPendentesAutorizacao)
                query = query.Where(o => o.CTe != null && o.CTe.Status == "A");
            else
                query = query.Where(o => o.CTe != null && (o.CTe.Status == "P" || o.CTe.Status == "E" || o.CTe.Status == "S") && o.Carga != null && o.Carga.SituacaoCarga != Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoCarga.Anulada);

            if (codigoPedidoViagemNavio > 0)
                query = query.Where(o => o.CTe.Viagem.Codigo == codigoPedidoViagemNavio);

            if (situacoes != null && situacoes.Length > 0)
                query = query.Where(o => situacoes.Contains(o.Carga.SituacaoCarga));

            if (codigoTerminalOrigem > 0)
                query = query.Where(o => o.CTe.TerminalOrigem.Codigo == codigoTerminalOrigem);

            if (codigoTerminalDestino > 0)
                query = query.Where(o => o.CTe.TerminalDestino.Codigo == codigoTerminalDestino);

            if (!string.IsNullOrWhiteSpace(numeroBooking))
                query = query.Where(o => o.CTe.NumeroBooking == numeroBooking);

            var queryCargaPedido = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.CargaPedido>();
            if (somenteComContainer)
                queryCargaPedido = queryCargaPedido.Where(o => o.Pedido.Container != null);
            else
                queryCargaPedido = queryCargaPedido.Where(o => o.Pedido.Container == null);

            if (somenteCargaPerigosa)
                queryCargaPedido = queryCargaPedido.Where(o => o.Pedido.PossuiCargaPerigosa == true);

            query = query.Where(o => queryCargaPedido.Any(p => p.Carga.Codigo == o.Carga.Codigo));

            if (tipoPropostaMultimodal.HasValue)
            {
                if (tipoPropostaMultimodal == Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoPropostaMultimodal.CargaFracionada)
                {
                    queryCargaPedido = queryCargaPedido.Where(o => o.TipoPropostaMultimodal == Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoPropostaMultimodal.CargaFracionada);
                    query = query.Where(o => queryCargaPedido.Any(p => p.Carga.Codigo == o.Carga.Codigo));
                }
                else
                {
                    queryCargaPedido = queryCargaPedido.Where(o => o.TipoPropostaMultimodal != Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoPropostaMultimodal.CargaFracionada);
                    query = query.Where(o => queryCargaPedido.Any(p => p.Carga.Codigo == o.Carga.Codigo));
                }
            }

            var queryCTeParaSubContratacao = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Pedidos.PedidoCTeParaSubContratacao>();
            query = query.Where(o => !queryCTeParaSubContratacao.Any(p => p.CTeTerceiro.ChaveAcesso == o.CTe.Chave && p.CargaPedido.Carga.SituacaoCarga != Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoCarga.Cancelada && p.CargaPedido.Carga.SituacaoCarga != Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoCarga.Anulada));


            var queryPedido = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.CargaPedido>();
            queryPedido = queryPedido.Where(o => query.Any(p => p.Carga.Codigo == o.Carga.Codigo));

            return queryPedido.Select(p => p.Pedido)?.FirstOrDefault() ?? null;
        }

        public Dominio.Entidades.Embarcador.Pedidos.Pedido ConsultarPedidoMultiModalTransbordo(bool somenteCargaPerigosa, string numeroBooking, Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoPropostaMultimodal? tipoPropostaMultimodal, int codigoPedidoViagemNavio, int codigoTerminalOrigem, int codigoTerminalDestino, Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoCarga[] situacoes, bool conhecimentosPendentesAutorizacao = false, bool somenteComContainer = true)
        {
            var queryPedidoTransbordo = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Pedidos.PedidoTransbordo>();
            if (codigoPedidoViagemNavio > 0)
                queryPedidoTransbordo = queryPedidoTransbordo.Where(o => o.PedidoViagemNavio.Codigo == codigoPedidoViagemNavio);
            if (codigoTerminalDestino > 0)
                queryPedidoTransbordo = queryPedidoTransbordo.Where(o => o.Terminal.Codigo == codigoTerminalDestino);

            if (!string.IsNullOrWhiteSpace(numeroBooking))
                queryPedidoTransbordo = queryPedidoTransbordo.Where(o => o.Pedido.NumeroBooking == numeroBooking);

            var queryCargaCTe = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.CargaCTe>();
            queryCargaCTe = queryCargaCTe.Where(o => o.CTe != null && !o.Carga.CargaTransbordo && o.CTe.TipoCTE == Dominio.Enumeradores.TipoCTE.Normal && o.CTe.TipoModal == Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoModal.Multimodal);

            if (!conhecimentosPendentesAutorizacao)
                queryCargaCTe = queryCargaCTe.Where(o => o.CTe != null && o.CTe.Status == "A");
            else
                queryCargaCTe = queryCargaCTe.Where(o => o.CTe != null && (o.CTe.Status == "P" || o.CTe.Status == "E" || o.CTe.Status == "S") && o.Carga != null && o.Carga.SituacaoCarga != Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoCarga.Anulada);

            if (codigoTerminalOrigem > 0)
                queryCargaCTe = queryCargaCTe.Where(o => o.CTe.TerminalOrigem.Codigo == codigoTerminalOrigem);

            if (situacoes != null && situacoes.Length > 0)
                queryCargaCTe = queryCargaCTe.Where(o => situacoes.Contains(o.Carga.SituacaoCarga));

            var queryCargaPedido = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.CargaPedido>();
            queryCargaPedido = queryCargaPedido.Where(o => queryPedidoTransbordo.Any(p => p.Pedido.Codigo == o.Pedido.Codigo));

            if (somenteComContainer)
                queryCargaPedido = queryCargaPedido.Where(o => o.Pedido.Container != null);
            else
                queryCargaPedido = queryCargaPedido.Where(o => o.Pedido.Container == null);

            if (somenteCargaPerigosa)
                queryCargaPedido = queryCargaPedido.Where(o => o.Pedido.PossuiCargaPerigosa == true);

            if (tipoPropostaMultimodal.HasValue)
            {
                if (tipoPropostaMultimodal == Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoPropostaMultimodal.CargaFracionada)
                    queryCargaPedido = queryCargaPedido.Where(o => o.TipoPropostaMultimodal == Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoPropostaMultimodal.CargaFracionada);
                else
                    queryCargaPedido = queryCargaPedido.Where(o => o.TipoPropostaMultimodal != Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoPropostaMultimodal.CargaFracionada);
            }

            queryCargaCTe = queryCargaCTe.Where(o => queryCargaPedido.Any(c => c.Carga.Codigo == o.Carga.Codigo));

            var queryCTeParaSubContratacao = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Pedidos.PedidoCTeParaSubContratacao>();
            queryCargaCTe = queryCargaCTe.Where(o => !queryCTeParaSubContratacao.Any(p => p.CTeTerceiro.ChaveAcesso == o.CTe.Chave && p.CargaPedido.Carga.SituacaoCarga != Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoCarga.Cancelada && p.CargaPedido.Carga.SituacaoCarga != Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoCarga.Anulada));

            var queryPedido = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.CargaPedido>();
            queryPedido = queryPedido.Where(o => queryCargaCTe.Any(p => p.Carga.Codigo == o.Carga.Codigo));

            return queryPedido.Timeout(7000).Select(p => p.Pedido)?.FirstOrDefault() ?? null;
        }

        public List<Dominio.Entidades.Embarcador.Cargas.CargaCTe> ConsultarMultiModal(int codigoPedidoViagemNavio, List<int> codigoTerminalOrigem, List<int> codigoTerminalDestino, Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoCarga[] situacoes)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.CargaCTe>();

            query = query.Where(o => o.CTe != null && !o.Carga.CargaTransbordo && o.CTe.TipoCTE == Dominio.Enumeradores.TipoCTE.Normal && o.CTe.TipoModal == Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoModal.Multimodal);

            query = query.Where(o => o.CTe != null && o.CTe.Status == "A");

            if (codigoPedidoViagemNavio > 0)
                query = query.Where(o => o.CTe.Viagem.Codigo == codigoPedidoViagemNavio);

            if (situacoes != null && situacoes.Length > 0)
                query = query.Where(o => situacoes.Contains(o.Carga.SituacaoCarga));

            if (codigoTerminalOrigem != null && codigoTerminalOrigem.Count > 0)
                query = query.Where(o => codigoTerminalOrigem.Contains(o.CTe.TerminalOrigem.Codigo));

            if (codigoTerminalDestino != null && codigoTerminalDestino.Count > 0)
                query = query.Where(o => codigoTerminalDestino.Contains(o.CTe.TerminalDestino.Codigo));

            var queryCargaPedido = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.CargaPedido>();
            queryCargaPedido = queryCargaPedido.Where(o => o.Pedido.Container != null);

            query = query.Where(o => queryCargaPedido.Any(p => p.Carga.Codigo == o.Carga.Codigo));

            var queryCTeParaSubContratacao = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Pedidos.PedidoCTeParaSubContratacao>();
            query = query.Where(o => !queryCTeParaSubContratacao.Any(p => p.CTeTerceiro.ChaveAcesso == o.CTe.Chave && p.CargaPedido.Carga.SituacaoCarga != Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoCarga.Cancelada && p.CargaPedido.Carga.SituacaoCarga != Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoCarga.Anulada));

            return query
                .Fetch(o => o.CTe).ThenFetch(o => o.Remetente).ThenFetch(o => o.Cliente)
                .Fetch(o => o.CTe).ThenFetch(o => o.Destinatario).ThenFetch(o => o.Cliente)
                .Fetch(o => o.CTe).ThenFetch(o => o.TomadorPagador).ThenFetch(o => o.Cliente)
                .Fetch(o => o.Carga).ToList();
        }

        public List<Dominio.Entidades.Embarcador.Cargas.CargaCTe> ConsultarMultiModalTransbordo(int codigoPedidoViagemNavio, List<int> codigoTerminalOrigem, List<int> codigoTerminalDestino, Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoCarga[] situacoes)
        {
            var queryPedidoTransbordo = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Pedidos.PedidoTransbordo>();
            if (codigoPedidoViagemNavio > 0)
                queryPedidoTransbordo = queryPedidoTransbordo.Where(o => o.PedidoViagemNavio.Codigo == codigoPedidoViagemNavio);
            if (codigoTerminalOrigem != null && codigoTerminalOrigem.Count > 0)
                queryPedidoTransbordo = queryPedidoTransbordo.Where(o => codigoTerminalOrigem.Contains(o.Terminal.Codigo));

            var queryCargaCTe = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.CargaCTe>();
            queryCargaCTe = queryCargaCTe.Where(o => o.CTe != null && !o.Carga.CargaTransbordo && o.CTe.TipoCTE == Dominio.Enumeradores.TipoCTE.Normal && o.CTe.TipoModal == Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoModal.Multimodal);

            queryCargaCTe = queryCargaCTe.Where(o => o.CTe != null && o.CTe.Status == "A");

            if (codigoTerminalDestino != null && codigoTerminalDestino.Count > 0)
                queryCargaCTe = queryCargaCTe.Where(o => codigoTerminalDestino.Contains(o.CTe.TerminalDestino.Codigo));

            if (situacoes != null && situacoes.Length > 0)
                queryCargaCTe = queryCargaCTe.Where(o => situacoes.Contains(o.Carga.SituacaoCarga));

            var queryCargaPedido = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.CargaPedido>();
            queryCargaPedido = queryCargaPedido.Where(o => queryPedidoTransbordo.Any(p => p.Pedido.Codigo == o.Pedido.Codigo));

            queryCargaPedido = queryCargaPedido.Where(o => o.Pedido.Container != null);
            queryCargaCTe = queryCargaCTe.Where(o => queryCargaPedido.Any(c => c.Carga.Codigo == o.Carga.Codigo));

            var queryCTeParaSubContratacao = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Pedidos.PedidoCTeParaSubContratacao>();
            queryCargaCTe = queryCargaCTe.Where(o => !queryCTeParaSubContratacao.Any(p => p.CTeTerceiro.ChaveAcesso == o.CTe.Chave && p.CargaPedido.Carga.SituacaoCarga != Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoCarga.Cancelada && p.CargaPedido.Carga.SituacaoCarga != Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoCarga.Anulada));

            return queryCargaCTe.Timeout(7000)
                .Fetch(o => o.CTe).ThenFetch(o => o.Remetente).ThenFetch(o => o.Cliente)
                .Fetch(o => o.CTe).ThenFetch(o => o.Destinatario).ThenFetch(o => o.Cliente)
                .Fetch(o => o.CTe).ThenFetch(o => o.TomadorPagador).ThenFetch(o => o.Cliente)
                .Fetch(o => o.Carga).ToList();
        }

        public List<Dominio.Entidades.Embarcador.Cargas.CargaCTe> ConsultarMultiModal(bool somenteCargaPerigosa, string numeroBooking, Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoPropostaMultimodal? tipoPropostaMultimodal, int codigoPedidoViagemNavio, int codigoTerminalOrigem, int codigoTerminalDestino, Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoCarga[] situacoes, bool conhecimentosPendentesAutorizacao = false, bool somenteComContainer = true, int codigoCargaSVM = 0)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.CargaCTe>();

            query = query.Where(o => o.CTe != null && !o.Carga.CargaTransbordo && o.CTe.TipoCTE == Dominio.Enumeradores.TipoCTE.Normal && o.CTe.TipoModal == Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoModal.Multimodal);

            if (!conhecimentosPendentesAutorizacao)
                query = query.Where(o => o.CTe != null && o.CTe.Status == "A");
            else
                query = query.Where(o => o.CTe != null && (o.CTe.Status == "P" || o.CTe.Status == "E" || o.CTe.Status == "S") && o.Carga != null && o.Carga.SituacaoCarga != Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoCarga.Anulada);

            if (codigoCargaSVM > 0)
                query = query.Where(o => o.Carga.Codigo == codigoCargaSVM);

            if (codigoPedidoViagemNavio > 0)
                query = query.Where(o => o.CTe.Viagem.Codigo == codigoPedidoViagemNavio);

            if (situacoes != null && situacoes.Length > 0)
                query = query.Where(o => situacoes.Contains(o.Carga.SituacaoCarga));

            if (codigoTerminalOrigem > 0)
                query = query.Where(o => o.CTe.TerminalOrigem.Codigo == codigoTerminalOrigem);

            if (codigoTerminalDestino > 0)
                query = query.Where(o => o.CTe.TerminalDestino.Codigo == codigoTerminalDestino);

            if (!string.IsNullOrWhiteSpace(numeroBooking))
                query = query.Where(o => o.CTe.NumeroBooking == numeroBooking);

            var queryCargaPedido = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.CargaPedido>();
            if (somenteComContainer)
                queryCargaPedido = queryCargaPedido.Where(o => o.Pedido.Container != null);
            else
                queryCargaPedido = queryCargaPedido.Where(o => o.Pedido.Container == null);

            if (somenteCargaPerigosa)
                queryCargaPedido = queryCargaPedido.Where(o => o.Pedido.PossuiCargaPerigosa == true);

            query = query.Where(o => queryCargaPedido.Any(p => p.Carga.Codigo == o.Carga.Codigo));

            if (tipoPropostaMultimodal.HasValue)
            {
                if (tipoPropostaMultimodal == Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoPropostaMultimodal.CargaFracionada)
                {
                    queryCargaPedido = queryCargaPedido.Where(o => o.TipoPropostaMultimodal == Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoPropostaMultimodal.CargaFracionada);
                    query = query.Where(o => queryCargaPedido.Any(p => p.Carga.Codigo == o.Carga.Codigo));
                }
                else
                {
                    queryCargaPedido = queryCargaPedido.Where(o => o.TipoPropostaMultimodal != Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoPropostaMultimodal.CargaFracionada);
                    query = query.Where(o => queryCargaPedido.Any(p => p.Carga.Codigo == o.Carga.Codigo));
                }
            }

            var queryCTeParaSubContratacao = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Pedidos.PedidoCTeParaSubContratacao>();
            query = query.Where(o => !queryCTeParaSubContratacao.Any(p => p.CTeTerceiro.ChaveAcesso == o.CTe.Chave && p.CargaPedido.Carga.SituacaoCarga != Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoCarga.Cancelada && p.CargaPedido.Carga.SituacaoCarga != Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoCarga.Anulada));

            return query
                .Fetch(o => o.CTe).ThenFetch(o => o.Remetente).ThenFetch(o => o.Cliente)
                .Fetch(o => o.CTe).ThenFetch(o => o.Destinatario).ThenFetch(o => o.Cliente)
                .Fetch(o => o.CTe).ThenFetch(o => o.TomadorPagador).ThenFetch(o => o.Cliente)
                .Fetch(o => o.Carga).ToList();
        }

        public List<Dominio.Entidades.Embarcador.Cargas.CargaCTe> ConsultarMultiModalTransbordo(bool somenteCargaPerigosa, string numeroBooking, Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoPropostaMultimodal? tipoPropostaMultimodal, int codigoPedidoViagemNavio, int codigoTerminalOrigem, int codigoTerminalDestino, Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoCarga[] situacoes, bool conhecimentosPendentesAutorizacao = false, bool somenteComContainer = true, int codigoCargaSVM = 0)
        {
            var queryPedidoTransbordo = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Pedidos.PedidoTransbordo>();
            if (codigoPedidoViagemNavio > 0)
                queryPedidoTransbordo = queryPedidoTransbordo.Where(o => o.PedidoViagemNavio.Codigo == codigoPedidoViagemNavio);
            if (codigoTerminalDestino > 0)
                queryPedidoTransbordo = queryPedidoTransbordo.Where(o => o.Terminal.Codigo == codigoTerminalDestino);

            if (!string.IsNullOrWhiteSpace(numeroBooking))
                queryPedidoTransbordo = queryPedidoTransbordo.Where(o => o.Pedido.NumeroBooking == numeroBooking);

            var queryCargaCTe = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.CargaCTe>();
            queryCargaCTe = queryCargaCTe.Where(o => o.CTe != null && !o.Carga.CargaTransbordo && o.CTe.TipoCTE == Dominio.Enumeradores.TipoCTE.Normal && o.CTe.TipoModal == Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoModal.Multimodal);

            if (!conhecimentosPendentesAutorizacao)
                queryCargaCTe = queryCargaCTe.Where(o => o.CTe != null && o.CTe.Status == "A");
            else
                queryCargaCTe = queryCargaCTe.Where(o => o.CTe != null && (o.CTe.Status == "P" || o.CTe.Status == "E" || o.CTe.Status == "S") && o.Carga != null && o.Carga.SituacaoCarga != Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoCarga.Anulada);

            if (codigoCargaSVM > 0)
                queryCargaCTe = queryCargaCTe.Where(o => o.Carga.Codigo == codigoCargaSVM);

            if (codigoTerminalOrigem > 0)
                queryCargaCTe = queryCargaCTe.Where(o => o.CTe.TerminalOrigem.Codigo == codigoTerminalOrigem);

            if (situacoes != null && situacoes.Length > 0)
                queryCargaCTe = queryCargaCTe.Where(o => situacoes.Contains(o.Carga.SituacaoCarga));

            var queryCargaPedido = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.CargaPedido>();
            queryCargaPedido = queryCargaPedido.Where(o => queryPedidoTransbordo.Any(p => p.Pedido.Codigo == o.Pedido.Codigo));

            if (somenteComContainer)
                queryCargaPedido = queryCargaPedido.Where(o => o.Pedido.Container != null);
            else
                queryCargaPedido = queryCargaPedido.Where(o => o.Pedido.Container == null);

            if (somenteCargaPerigosa)
                queryCargaPedido = queryCargaPedido.Where(o => o.Pedido.PossuiCargaPerigosa == true);

            if (tipoPropostaMultimodal.HasValue)
            {
                if (tipoPropostaMultimodal == Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoPropostaMultimodal.CargaFracionada)
                    queryCargaPedido = queryCargaPedido.Where(o => o.TipoPropostaMultimodal == Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoPropostaMultimodal.CargaFracionada);
                else
                    queryCargaPedido = queryCargaPedido.Where(o => o.TipoPropostaMultimodal != Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoPropostaMultimodal.CargaFracionada);
            }

            queryCargaCTe = queryCargaCTe.Where(o => queryCargaPedido.Any(c => c.Carga.Codigo == o.Carga.Codigo));

            var queryCTeParaSubContratacao = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Pedidos.PedidoCTeParaSubContratacao>();
            queryCargaCTe = queryCargaCTe.Where(o => !queryCTeParaSubContratacao.Any(p => p.CTeTerceiro.ChaveAcesso == o.CTe.Chave && p.CargaPedido.Carga.SituacaoCarga != Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoCarga.Cancelada && p.CargaPedido.Carga.SituacaoCarga != Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoCarga.Anulada));

            return queryCargaCTe.Timeout(7000)
                .Fetch(o => o.CTe).ThenFetch(o => o.Remetente).ThenFetch(o => o.Cliente)
                .Fetch(o => o.CTe).ThenFetch(o => o.Destinatario).ThenFetch(o => o.Cliente)
                .Fetch(o => o.CTe).ThenFetch(o => o.TomadorPagador).ThenFetch(o => o.Cliente)
                .Fetch(o => o.Carga).ToList();
        }

        public List<Dominio.Entidades.Embarcador.Cargas.CargaCTe> ConsultarMultiModal(int codigoCarga, int numeroCTe, int numeroNotaFiscal, Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoCarga[] situacoes, bool consultaInicial, string propOrdenar, string dirOrdena, int inicio, int limite)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.CargaCTe>();

            if (consultaInicial)
            {
                query = query.Where(o => o.Codigo == 0);
                return query.ToList();
            }


            query = query.Where(o => o.CTe != null && !o.Carga.CargaTransbordo && o.CTe.TipoCTE == Dominio.Enumeradores.TipoCTE.Normal && o.CTe.TipoModal == Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoModal.Multimodal);

            if (codigoCarga > 0)
                query = query.Where(o => o.Carga.Codigo == codigoCarga);

            if (situacoes != null && situacoes.Length > 0)
                query = query.Where(o => situacoes.Contains(o.Carga.SituacaoCarga));

            if (numeroCTe > 0)
                query = query.Where(o => o.CTe.Numero == numeroCTe);

            if (numeroNotaFiscal > 0)
                query = query.Where(o => o.NotasFiscais.Any(nf => nf.PedidoXMLNotaFiscal.XMLNotaFiscal.Numero == numeroNotaFiscal));


            return query.OrderBy(propOrdenar + " " + dirOrdena).Skip(inicio).Take(limite)
                        .Fetch(o => o.CTe).ToList();
        }

        public int ContarConsultaMultiModal(int codigoCarga, int numeroCTe, int numeroNotaFiscal, Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoCarga[] situacoes, bool consultaInicial)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.CargaCTe>();

            if (consultaInicial)
            {
                query = query.Where(o => o.Codigo == 0);
                return query.Count();
            }


            query = query.Where(o => o.CTe != null && !o.Carga.CargaTransbordo && o.CTe.TipoCTE == Dominio.Enumeradores.TipoCTE.Normal && o.CTe.TipoModal == Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoModal.Multimodal);

            if (codigoCarga > 0)
                query = query.Where(o => o.Carga.Codigo == codigoCarga);

            if (situacoes != null && situacoes.Length > 0)
                query = query.Where(o => situacoes.Contains(o.Carga.SituacaoCarga));

            if (numeroCTe > 0)
                query = query.Where(o => o.CTe.Numero == numeroCTe);

            if (numeroNotaFiscal > 0)
                query = query.Where(o => o.NotasFiscais.Any(nf => nf.PedidoXMLNotaFiscal.XMLNotaFiscal.Numero == numeroNotaFiscal));


            return query.Count();
        }

        public List<Dominio.Entidades.Embarcador.Cargas.CargaCTe> BuscarPorCargaCanceladosManualmente(int codigoCarga)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.CargaCTe>();
            var resut = from obj in query where obj.Carga.Codigo == codigoCarga select obj;

            resut = resut.Where(obj => obj.CTe.Status == "C" || obj.CTe.Status == "I");

            return resut.Fetch(o => o.CTe).ToList();
        }

        public List<Dominio.Entidades.Embarcador.Cargas.CargaCTe> BuscarPorCargaParaMDFePorTipoModal(int codigoCarga, int empresa, AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware tipoServicoMultisoftware, bool semComplementares = false, bool somenteEmDigitacao = false, bool somenteAutorizados = false, bool somenteModeloCTe = false, Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoModal tipoModal = Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoModal.Todos)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.CargaCTe>();
            var resut = from obj in query where obj.Carga.Codigo == codigoCarga && obj.CTe.Empresa.Codigo == empresa select obj;

            if (semComplementares)
            {
                resut = resut.Where(obj => obj.CargaCTeComplementoInfo == null);
            }

            if (somenteEmDigitacao)
                resut = resut.Where(obj => obj.CTe.Status == "S");

            if (somenteAutorizados)
                resut = resut.Where(obj => obj.CTe.Status == "A");

            if (somenteModeloCTe)
                resut = resut.Where(obj => obj.CTe.ModeloDocumentoFiscal.Numero == "57");

            if (tipoModal != Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoModal.Todos)
                resut = resut.Where(obj => obj.CTe.TipoModal == tipoModal);

            return resut.Fetch(o => o.CTe).ToList();
        }

        public List<Dominio.Entidades.Embarcador.Cargas.CargaCTe> BuscarPorCargaParaMDFe(int codigoCarga, int empresa, AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware tipoServicoMultisoftware, bool semComplementares = false, bool somenteEmDigitacao = false, bool somenteAutorizados = false, bool somenteModeloCTe = false)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.CargaCTe>();
            var resut = from obj in query where obj.Carga.Codigo == codigoCarga /*&& obj.CTe.Empresa.Codigo == empresa*/ select obj;

            if (semComplementares)
            {
                resut = resut.Where(obj => obj.CargaCTeComplementoInfo == null);

                //if (tipoServicoMultisoftware == AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiTMS)
                //    resut = resut.Where(obj => obj.CTe.TipoCTE == Dominio.Enumeradores.TipoCTE.Normal);

                //var queryComplementos = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.CargaCTeComplementoInfo>();
                //resut = resut.Where(o => !(from obj in queryComplementos where obj.CargaCTe.Codigo == o.Codigo select obj.CargaCTe.Codigo).Contains(o.Codigo));
            }

            if (somenteEmDigitacao)
                resut = resut.Where(obj => obj.CTe.Status == "S");

            if (somenteAutorizados)
                resut = resut.Where(obj => obj.CTe.Status == "A");

            if (somenteModeloCTe)
                resut = resut.Where(obj => obj.CTe.ModeloDocumentoFiscal.Numero == "57");

            return resut
                .Fetch(o => o.CTe)
                .ThenFetch(obj => obj.LocalidadeInicioPrestacao)
                .ThenFetch(obj => obj.Estado)
                .Fetch(o => o.CTe)
                .ThenFetch(obj => obj.LocalidadeTerminoPrestacao)
                .ThenFetch(obj => obj.Estado)
                .ToList();
        }

        public List<Dominio.Entidades.Embarcador.Cargas.CargaCTe> BuscarPorCargaOrigemParaMDFe(int codigoCarga, int empresa, AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware tipoServicoMultisoftware, bool semComplementares = false, bool somenteEmDigitacao = false, bool somenteAutorizados = false, bool somenteModeloCTe = false)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.CargaCTe>();
            var resut = from obj in query where obj.CargaOrigem.Codigo == codigoCarga && obj.CTe.Empresa.Codigo == empresa select obj;

            if (semComplementares)
            {
                resut = resut.Where(obj => obj.CargaCTeComplementoInfo == null);

                //if (tipoServicoMultisoftware == AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiTMS)
                //    resut = resut.Where(obj => obj.CTe.TipoCTE == Dominio.Enumeradores.TipoCTE.Normal);

                //var queryComplementos = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.CargaCTeComplementoInfo>();
                //resut = resut.Where(o => !(from obj in queryComplementos where obj.CargaCTe.Codigo == o.Codigo select obj.CargaCTe.Codigo).Contains(o.Codigo));
            }

            if (somenteEmDigitacao)
                resut = resut.Where(obj => obj.CTe.Status == "S");

            if (somenteAutorizados)
                resut = resut.Where(obj => obj.CTe.Status == "A" || obj.CTe.Status == "F");

            if (somenteModeloCTe)
                resut = resut.Where(obj => obj.CTe.ModeloDocumentoFiscal.Numero == "57");

            return resut
                .Fetch(o => o.CTe)
                .ThenFetch(obj => obj.LocalidadeInicioPrestacao)
                .ThenFetch(obj => obj.Estado)
                .Fetch(o => o.CTe)
                .ThenFetch(obj => obj.LocalidadeTerminoPrestacao)
                .ThenFetch(obj => obj.Estado)
                .ToList();
        }

        public List<Dominio.Entidades.Embarcador.Cargas.CargaCTe> BuscarCTeCargasOcorrenciaPorPeriodo(DateTime periodoInicial, DateTime periodoFim, int transportador, double proprietario)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.CargaCTe>();

            query = query.Where(o => o.Carga.DataCriacaoCarga.Date >= periodoInicial && o.Carga.DataCriacaoCarga.Date <= periodoFim.AddDays(1).Date);

            if (transportador > 0 && proprietario > 0)
                query = query.Where(o => o.Carga.Empresa.Codigo == transportador || o.Carga.Veiculo.Proprietario.CPF_CNPJ == proprietario);
            else if (transportador > 0)
                query = query.Where(o => o.Carga.Empresa.Codigo == transportador);
            else if (proprietario > 0)
                query = query.Where(o => o.Carga.Veiculo.Proprietario.CPF_CNPJ == proprietario);

            query = query.Where(o => o.Carga.SituacaoCarga == Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoCarga.Encerrada);

            return query.Fetch(o => o.CTe).ToList();
        }

        public List<Dominio.Entidades.Embarcador.Cargas.CargaCTe> BuscarPorCargaCTeComplementoInfo(int cargaCteComplementoInfo)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.CargaCTe>();
            var resut = from obj in query where obj.CargaCTeComplementoInfo.Codigo == cargaCteComplementoInfo select obj;

            return resut
                .Fetch(obj => obj.PreCTe)
                .ToList();
        }

        public Dominio.Entidades.Embarcador.Cargas.CargaCTe BuscarCargaCTePorCargaCTeComplementoInfo(int cargaCteComplementoInfo)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.CargaCTe>();

            query = query.Where(obj => obj.CargaCTeComplementoInfo.Codigo == cargaCteComplementoInfo);

            return query.FirstOrDefault();
        }

        public List<string> BuscarNumerosCteComplementares(List<int> codigoCte)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.CargaCTe>();

            query = query.Where(obj => codigoCte.Contains(obj.CTe.Codigo) && obj.CargaCTeComplementoInfo.Codigo != null);

            return query.Select(x => $"{x.CargaCTeComplementoInfo.CTeComplementado.Numero}").ToList();
        }

        public List<Dominio.Entidades.Embarcador.Cargas.CargaCTe> BuscarPorCargaCTesSemSubcontratacaoFilialEmissora(int codigoCarga, List<int> empresas, List<int> codigosModelosDocumentos = null)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.CargaCTe>();
            var resut = from obj in query where obj.Carga.Codigo == codigoCarga select obj;


            resut = resut.Where(obj => obj.CargaCTeComplementoInfo == null);
            if (empresas.Count > 0)
                resut = resut.Where(obj => empresas.Contains(obj.CTe.Empresa.Codigo));
            else
                resut = resut.Where(obj => (obj.CargaCTeFilialEmissora == null && obj.CargaCTeSubContratacaoFilialEmissora != null) || (obj.CargaCTeFilialEmissora == null && obj.CargaCTeSubContratacaoFilialEmissora == null && (empresas.Count == 0 || empresas.Contains(obj.CTe.Empresa.Codigo))));

            resut = resut.Where(obj => obj.CTe.Status == "A");

            if (codigosModelosDocumentos != null && codigosModelosDocumentos.Count > 0)
                resut = resut.Where(o => codigosModelosDocumentos.Contains(o.CTe.ModeloDocumentoFiscal.Codigo));

            return resut.Fetch(o => o.CTe).ToList();
        }

        public List<Dominio.Entidades.Embarcador.Cargas.CargaCTe> BuscarPorCargaCTesCanceladosSemSubcontratacaoFilialEmissora(int codigoCarga, int empresa)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.CargaCTe>();
            var resut = from obj in query where obj.Carga.Codigo == codigoCarga select obj;


            resut = resut.Where(obj => obj.CargaCTeComplementoInfo == null);

            if (empresa > 0)
                resut = resut.Where(obj => obj.CTe.Empresa.Codigo == empresa);
            else
                resut = resut.Where(obj => (obj.CargaCTeFilialEmissora == null && obj.CargaCTeSubContratacaoFilialEmissora != null) || (obj.CargaCTeFilialEmissora == null && obj.CargaCTeSubContratacaoFilialEmissora == null && (empresa == 0 || obj.CTe.Empresa.Codigo == empresa)));


            resut = resut.Where(obj => obj.CTe.Status == "C" || obj.CTe.Status == "I");


            return resut.Fetch(o => o.CTe).ToList();
        }

        public List<int> BuscarPorCargaCTesFilialEmissora(int codigoCarga, List<int> empresasFiliaisEmissoras)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.CargaCTe>();
            var resut = from obj in query where obj.Carga.Codigo == codigoCarga && empresasFiliaisEmissoras.Contains(obj.CTe.Empresa.Codigo) select obj;

            resut = resut.Where(obj => obj.CTe.Status == "A");

            return resut.Select(o => o.Codigo).ToList();
        }

        public List<Dominio.Entidades.Embarcador.Cargas.CargaCTe> BuscarPorCargaCTesSemCTeFilialEmissora(int codigoCarga)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.CargaCTe>();
            var resut = from obj in query where obj.Carga.Codigo == codigoCarga select obj;


            resut = resut.Where(obj => obj.CargaCTeComplementoInfo == null);
            resut = resut.Where(obj => obj.CargaCTeSubContratacaoFilialEmissora == null);


            resut = resut.Where(obj => obj.CTe.Status == "A");


            return resut.Fetch(o => o.CTe).ToList();
        }

        public List<Dominio.Entidades.Embarcador.Cargas.CargaCTe> BuscarPorCarga(int codigoCarga, bool somenteModeloCTe, bool ctesSubContratacaoFilialEmissora, double remetente, double destinatario, bool retornarPreCtes)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.CargaCTe>();
            var resut = from obj in query where obj.Carga.Codigo == codigoCarga select obj;


            resut = resut.Where(obj => obj.CargaCTeComplementoInfo == null);


            if (ctesSubContratacaoFilialEmissora)
                resut = resut.Where(obj => obj.CargaCTeFilialEmissora != null || (obj.CargaCTeFilialEmissora == null && obj.CargaCTeSubContratacaoFilialEmissora == null));


            if (!retornarPreCtes)
                resut = resut.Where(obj => obj.CTe.Status == "A");
            else
                resut = resut.Where(obj => obj.CTe.Status == "A" || obj.CTe == null);


            if (somenteModeloCTe)
            {
                if (!retornarPreCtes)
                    resut = resut.Where(obj => obj.CTe.ModeloDocumentoFiscal.TipoDocumentoEmissao == Dominio.Enumeradores.TipoDocumento.CTe);
                else
                    resut = resut.Where(obj => obj.CTe.ModeloDocumentoFiscal.TipoDocumentoEmissao == Dominio.Enumeradores.TipoDocumento.CTe || obj.CTe == null);
            }

            if (destinatario > 0)
            {
                if (!retornarPreCtes)
                    resut = resut.Where(o => o.CTe.Destinatario.Cliente.CPF_CNPJ == destinatario);
                else
                    resut = resut.Where(o => (o.CTe.Destinatario.Cliente.CPF_CNPJ == destinatario || o.CTe.IndicadorGlobalizado == Dominio.Enumeradores.OpcaoSimNao.Sim) || (o.PreCTe.Destinatario.Cliente.CPF_CNPJ == destinatario && o.CTe == null));
            }

            if (remetente > 0)
            {
                if (!retornarPreCtes)
                    resut = resut.Where(o => o.CTe.Remetente.Cliente.CPF_CNPJ == remetente);
                else
                    resut = resut.Where(o => o.CTe.Remetente.Cliente.CPF_CNPJ == remetente || (o.PreCTe.Remetente.Cliente.CPF_CNPJ == remetente && o.CTe == null));
            }

            return resut.Fetch(o => o.CTe).ToList();
        }

        public List<Dominio.Entidades.Embarcador.Cargas.CargaCTe> BuscarPorCargaENotaFiscal(int codigoCarga, bool somenteModeloCTe, bool ctesSubContratacaoFilialEmissora, double remetente, double destinatario, bool retornarPreCtes, List<string> numerosNota, string serieNotaFiscal)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.CargaCTe>();
            var resut = from obj in query where obj.Carga.Codigo == codigoCarga select obj;


            resut = resut.Where(obj => obj.CargaCTeComplementoInfo == null);


            if (ctesSubContratacaoFilialEmissora)
                resut = resut.Where(obj => obj.CargaCTeFilialEmissora != null || (obj.CargaCTeFilialEmissora == null && obj.CargaCTeSubContratacaoFilialEmissora == null));


            if (!retornarPreCtes)
                resut = resut.Where(obj => obj.CTe.Status == "A");
            else
                resut = resut.Where(obj => obj.CTe.Status == "A" || obj.CTe == null);


            if (somenteModeloCTe)
            {
                if (!retornarPreCtes)
                    resut = resut.Where(obj => obj.CTe.ModeloDocumentoFiscal.TipoDocumentoEmissao == Dominio.Enumeradores.TipoDocumento.CTe);
                else
                    resut = resut.Where(obj => obj.CTe.ModeloDocumentoFiscal.TipoDocumentoEmissao == Dominio.Enumeradores.TipoDocumento.CTe || obj.CTe == null);
            }

            if (destinatario > 0)
            {
                if (!retornarPreCtes)
                    resut = resut.Where(o => o.CTe.Destinatario.Cliente.CPF_CNPJ == destinatario);
                else
                    resut = resut.Where(o => (o.CTe.Destinatario.Cliente.CPF_CNPJ == destinatario || o.CTe.IndicadorGlobalizado == Dominio.Enumeradores.OpcaoSimNao.Sim) || (o.PreCTe.Destinatario.Cliente.CPF_CNPJ == destinatario && o.CTe == null));
            }

            if (remetente > 0)
            {
                if (!retornarPreCtes)
                    resut = resut.Where(o => o.CTe.Remetente.Cliente.CPF_CNPJ == remetente);
                else
                    resut = resut.Where(o => o.CTe.Remetente.Cliente.CPF_CNPJ == remetente || (o.PreCTe.Remetente.Cliente.CPF_CNPJ == remetente && o.CTe == null));
            }

            if (numerosNota.Count > 0 && numerosNota.Any())
                resut = resut.Where(o => o.CTe.Documentos.Any(nf => numerosNota.Contains(nf.Numero)));

            if (!string.IsNullOrEmpty(serieNotaFiscal))
                resut = resut.Where(o => o.CTe.Documentos.Any(nf => nf.Serie.Equals(serieNotaFiscal)));

            return resut.Fetch(o => o.CTe).ToList();
        }

        public Dominio.Entidades.Embarcador.Cargas.CargaCTe BuscarPrimeiroPorCargaEXMLNotaFiscal(int codigoCarga, int codigoNotaFiscal)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.CargaCTe>();

            query = query.Where(o => o.Carga.Codigo == codigoCarga && o.CargaCTeComplementoInfo == null && o.CTe.XMLNotaFiscais.Any(obj => obj.Codigo == codigoNotaFiscal));

            return query.FirstOrDefault();
        }

        public Dominio.Entidades.Embarcador.Cargas.CargaCTe BuscarPorCargaECTe(int codigoCarga, int codigoCTe)
        {
            IQueryable<Dominio.Entidades.Embarcador.Cargas.CargaCTe> query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.CargaCTe>();

            query = query.Where(o => o.Carga.Codigo == codigoCarga && o.CTe.Codigo == codigoCTe);

            return query.FirstOrDefault();
        }

        public List<Dominio.Entidades.Embarcador.Cargas.CargaCTe> BuscarPorCarga(int codigoCarga, double cpfCnpjRemetente)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.CargaCTe>();

            query = query.Where(o => o.Carga.Codigo == codigoCarga && o.CargaCTeComplementoInfo == null && o.CTe.Status == "A");

            if (cpfCnpjRemetente > 0D)
                query = query.Where(o => o.CTe.Remetente.Cliente.CPF_CNPJ == cpfCnpjRemetente);

            return query.Fetch(o => o.CTe).ToList();
        }

        public Task<Dominio.Entidades.Embarcador.Cargas.CargaCTe> BuscarPrimeiroPorCargaAsync(int codigoCarga)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.CargaCTe>();

            query = query.Where(o => o.Carga.Codigo == codigoCarga);

            return query.Fetch(o => o.CTe).FirstAsync();
        }

        public List<Dominio.Entidades.Embarcador.Cargas.CargaCTe> BuscarPorCargaEClienteDestino(int codigoCarga, double cpfCnpjDestino)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.CargaCTe>();

            query = query.Where(o => o.Carga.Codigo == codigoCarga && o.CargaCTeComplementoInfo == null && o.CTe.Status == "A");

            if (cpfCnpjDestino > 0D)
                query = query.Where(o => o.CTe.Destinatario.Cliente.CPF_CNPJ == cpfCnpjDestino);

            return query.Fetch(o => o.CTe).ToList();
        }

        public List<Dominio.Entidades.Embarcador.Cargas.CargaCTe> BuscarPorCargaEModeloDocumentoFiscal(int codigoCarga, Dominio.Enumeradores.TipoDocumento modeloDocumentoFiscal)
        {
            IQueryable<Dominio.Entidades.Embarcador.Cargas.CargaCTe> query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.CargaCTe>()
                .Where(o => o.Carga.Codigo == codigoCarga && o.CTe.ModeloDocumentoFiscal.TipoDocumentoEmissao == modeloDocumentoFiscal);

            return query.ToList();
        }

        public List<Dominio.Entidades.Embarcador.Cargas.CargaCTe> BuscarPorCarga(int codigoCarga, bool semComplementares = false, bool somenteEmDigitacao = false, bool somenteAutorizados = false, bool somenteModeloCTe = false, bool ctesSubContratacaoFilialEmissora = false, int numeroNF = 0, double destinatario = 0, bool buscarPorCargaOrigem = false, bool retornarPreCtes = false, int numeroDocumento = 0, int codigoCTeCancelamentoUnitario = 0, bool diferenteDeModeloCTe = false)
        {
            IQueryable<Dominio.Entidades.Embarcador.Cargas.CargaCTe> query = QueryBuscarPorCargaBase(codigoCarga, semComplementares, somenteEmDigitacao, somenteAutorizados, somenteModeloCTe, ctesSubContratacaoFilialEmissora, numeroNF, destinatario, buscarPorCargaOrigem, retornarPreCtes, numeroDocumento, codigoCTeCancelamentoUnitario, diferenteDeModeloCTe);
            return query.ToList();
        }

        public Task<List<Dominio.Entidades.Embarcador.Cargas.CargaCTe>> BuscarPorCargaAsync(int codigoCarga, bool semComplementares = false, bool somenteEmDigitacao = false, bool somenteAutorizados = false, bool somenteModeloCTe = false, bool ctesSubContratacaoFilialEmissora = false, int numeroNF = 0, double destinatario = 0, bool buscarPorCargaOrigem = false, bool retornarPreCtes = false, int numeroDocumento = 0, int codigoCTeCancelamentoUnitario = 0, bool diferenteDeModeloCTe = false)
        {
            IQueryable<Dominio.Entidades.Embarcador.Cargas.CargaCTe> query = QueryBuscarPorCargaBase(codigoCarga, semComplementares, somenteEmDigitacao, somenteAutorizados, somenteModeloCTe, ctesSubContratacaoFilialEmissora, numeroNF, destinatario, buscarPorCargaOrigem, retornarPreCtes, numeroDocumento, codigoCTeCancelamentoUnitario, diferenteDeModeloCTe);
            return query.ToListAsync();
        }

        public List<Dominio.Entidades.Embarcador.Cargas.CargaCTe> BuscarPorCargaSemComplementares(int codigoCarga)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.CargaCTe>();

            query = query.Where(obj => obj.Carga.Codigo == codigoCarga && obj.CargaCTeComplementoInfo == null);

            return query.Fetch(o => o.CTe).ToList();
        }

        public List<int> BuscarCodigosPorCargaSemComplementaresESemMovimentoCancelamento(int codigoCarga)
        {
            IQueryable<Dominio.Entidades.Embarcador.Cargas.CargaCTe> query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.CargaCTe>();

            query = query.Where(obj => !obj.GerouMovimentacaoCancelamento && obj.Carga.Codigo == codigoCarga && obj.CargaCTeComplementoInfo == null && obj.LancamentoNFSManual == null);

            return query.Select(o => o.Codigo).Distinct().ToList();
        }

        public List<Dominio.Entidades.Embarcador.Cargas.CargaCTe> BuscarPorCargaEStatusSemComplementares(int codigoCarga, string[] statusCTe)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.CargaCTe>();

            query = query.Where(obj => obj.Carga.Codigo == codigoCarga && statusCTe.Contains(obj.CTe.Status) && obj.CargaCTeComplementoInfo == null);

            return query.Fetch(o => o.CTe).ToList();
        }

        public List<int> BuscarCodigosPorCargaSemComplementaresEQueNaoGeraramMovimentosAutorizacao(int codigoCarga)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.CargaCTe>();

            query = query.Where(obj => obj.Carga.Codigo == codigoCarga && !obj.GerouMovimentacaoAutorizacao && obj.CargaCTeComplementoInfo == null);

            return query.Select(o => o.Codigo).ToList();
        }

        public List<int> BuscarCodigosPorCargaSemComplementaresEQueNaoGeraramCanhotos(int codigoCarga)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.CargaCTe>();

            query = query.Where(obj => obj.Carga.Codigo == codigoCarga && !obj.GerouCanhoto && obj.CargaCTeComplementoInfo == null);

            return query.Select(o => o.Codigo).ToList();
        }

        public List<int> BuscarCodigosPorCargaSemComplementaresComFaturamentoEQueNaoGeraramTitulos(int codigoCarga)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.CargaCTe>();

            query = query.Where(obj => obj.Carga.Codigo == codigoCarga && !obj.GerouTituloAutorizacao && obj.CargaCTeComplementoInfo == null && !obj.CTe.ModeloDocumentoFiscal.NaoGerarFaturamento);

            return query.Select(o => o.Codigo).ToList();
        }

        public List<int> BuscarCodigosPorCargaSemComplementaresComFaturamentoEQueNaoGeraramTitulosGNRE(int codigoCarga)
        {
            IQueryable<Dominio.Entidades.Embarcador.Cargas.CargaCTe> query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.CargaCTe>();

            query = query.Where(obj => obj.Carga.Codigo == codigoCarga && !obj.GerouTituloGNREAutorizacao && obj.CargaCTeComplementoInfo == null && !obj.CTe.ModeloDocumentoFiscal.NaoGerarFaturamento);

            return query.Select(o => o.Codigo).ToList();
        }

        public List<int> BuscarCodigosPorCargaComplementaresComFaturamentoEQueNaoGeraramTitulosGNRE(int codigoCarga)
        {
            IQueryable<Dominio.Entidades.Embarcador.Cargas.CargaCTe> query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.CargaCTe>();

            query = query.Where(obj => obj.Carga.Codigo == codigoCarga && !obj.GerouTituloGNREAutorizacao && obj.CargaCTeComplementoInfo != null && !obj.CTe.ModeloDocumentoFiscal.NaoGerarFaturamento);

            return query.Select(o => o.Codigo).ToList();
        }

        public List<int> BuscarCodigosComCheckinRecusadoPorCarga(int codigoCarga)
        {
            IQueryable<Dominio.Entidades.Embarcador.Cargas.CargaCTe> consultaCargaCTe = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.CargaCTe>()
                .Where(cargaCTe => cargaCTe.Carga.Codigo == codigoCarga && cargaCTe.SituacaoCheckin == SituacaoCheckin.RecusaAprovada);

            return consultaCargaCTe.Select(cargaCTe => cargaCTe.Codigo).ToList();
        }

        public bool VerificarSeExisteCargaCTeParaGerarGNRE(int codigoCarga)
        {
            IQueryable<Dominio.Entidades.Embarcador.Cargas.CargaCTe> query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.CargaCTe>();

            query = query.Where(obj => obj.Carga.Codigo == codigoCarga && !obj.GerouTituloGNREAutorizacao && obj.CargaCTeComplementoInfo == null && !obj.CTe.ModeloDocumentoFiscal.NaoGerarFaturamento);

            return query.Count() > 0;
        }

        public List<int> BuscarPorCargaSemComplementaresComFaturamentoEQueNaoGeraramControleFaturamento(int codigoCarga)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.CargaCTe>();

            query = query.Where(obj => obj.Carga.Codigo == codigoCarga && !obj.GerouControleFaturamento && obj.CargaCTeComplementoInfo == null && !obj.CTe.ModeloDocumentoFiscal.NaoGerarFaturamento && obj.CTe.ModeloDocumentoFiscal.TipoDocumentoEmissao != Dominio.Enumeradores.TipoDocumento.NFS);
            //query = query.Where(o => o.Carga.TipoOperacao == null || !o.Carga.TipoOperacao.NaoGerarFaturamento);

            return query.Select(o => o.Codigo).ToList();
        }

        public List<int> BuscarPorCargaSemComplementaresComFaturamentoQueGeram(int codigoCarga)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.CargaCTe>();

            query = query.Where(obj => obj.Carga.Codigo == codigoCarga && !obj.GerouControleFaturamento && obj.CargaCTeComplementoInfo == null && obj.CTe.ModeloDocumentoFiscal.TipoDocumentoEmissao != Dominio.Enumeradores.TipoDocumento.NFS);

            return query.Select(o => o.Codigo).ToList();
        }

        public List<Dominio.Entidades.Embarcador.Cargas.CargaCTe> BuscarPorCargaEmitidosEmOutroSistema(int codigoCarga)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.CargaCTe>();

            query = query.Where(o => o.CTe.Status == "A" && o.Carga.Codigo == codigoCarga && o.SistemaEmissor != Dominio.ObjetosDeValor.Embarcador.Enumeradores.SistemaEmissor.MultiCTe);

            return query.Fetch(o => o.CTe).ToList();
        }

        public List<Dominio.Entidades.Embarcador.Cargas.CargaCTe> BuscarPorOcorrenciaEmitidosEmOutroSistema(int codigoOcorrencia)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.CargaCTe>();

            query = query.Where(o => o.CTe.Status == "A" && o.CargaCTeComplementoInfo.CargaOcorrencia.Codigo == codigoOcorrencia && o.SistemaEmissor != Dominio.ObjetosDeValor.Embarcador.Enumeradores.SistemaEmissor.MultiCTe);

            return query.Fetch(o => o.CTe).ToList();
        }

        public List<int> BuscarCodigosCTeTransportadorPorCarga(int codigoCarga)
        {
            IQueryable<Dominio.Entidades.Embarcador.Cargas.CargaCTe> query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.CargaCTe>();

            query = query.Where(obj => obj.CargaOrigem.Codigo == codigoCarga && obj.CargaCTeComplementoInfo == null && obj.CargaCTeSubContratacaoFilialEmissora == null && obj.CTe.ModeloDocumentoFiscal.NaoGerarEscrituracao == false && obj.CTe.ModeloDocumentoFiscal.TipoDocumentoEmissao != Dominio.Enumeradores.TipoDocumento.NFS);

            return query.Select(o => o.CTe.Codigo).ToList();
        }

        public List<Dominio.Entidades.ConhecimentoDeTransporteEletronico> BuscarCTesTransportadorPorCarga(int codigoCarga)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.CargaCTe>();

            var resut = from obj in query where obj.Carga.Codigo == codigoCarga && obj.CargaCTeSubContratacaoFilialEmissora == null select obj.CTe;

            return resut.ToList();
        }

        public List<int> BuscarCodigosPorCarga(int codigoCarga)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.CargaCTe>();

            var resut = from obj in query where obj.Carga.Codigo == codigoCarga select obj.Codigo;

            return resut.ToList();
        }

        public List<int> BuscarCodigosPorCargaPrechekinAceito(int codigoCarga)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.CargaCTe>();

            var resut = from obj in query where obj.Carga.Codigo == codigoCarga && SituacaoCheckinHelper.ObterSituacoesLiberadasPreCheckin().Contains(obj.SituacaoCheckin) select obj.Codigo;

            return resut.ToList();
        }

        public List<int> BuscarCodigosPorNFSManualSemIntegracao(int codigoLancamenetoManual, Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoIntegracao tipoIntegracao, int quantidadeRegistros)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.NFS.LancamentoNFSManual>();

            var subQueryIntegracoes = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.NFS.NFSManualCTeIntegracao>().Where(o => o.LancamentoNFSManual.Codigo == codigoLancamenetoManual && o.TipoIntegracao.Tipo == tipoIntegracao).Select(o => o.LancamentoNFSManual.Codigo);

            query = query.Where(o => o.Codigo == codigoLancamenetoManual && !subQueryIntegracoes.Contains(o.Codigo));

            return query.Select(o => o.Codigo).Take(quantidadeRegistros).ToList();
        }

        public List<int> BuscarCodigosPorCargaSemIntegracao(int codigoCarga, Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoIntegracao tipoIntegracao, bool apenasCTe, int? quantidadeRegistros = null)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.CargaCTe>();

            var subQueryIntegracoes = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.CargaCTeIntegracao>().Where(o => o.CargaCTe.Carga.Codigo == codigoCarga && o.TipoIntegracao.Tipo == tipoIntegracao);

            if (tipoIntegracao == Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoIntegracao.Unilever)
            {
                query = query.Where(x => x.PreCTe == null && x.CTe.Status != "C");
                subQueryIntegracoes = subQueryIntegracoes.Where(s => s.CargaCTe.PreCTe != null);
            }

            query = query.Where(o => o.Carga.Codigo == codigoCarga && !subQueryIntegracoes.Select(x => x.CargaCTe.Codigo).Contains(o.Codigo));

            if (apenasCTe)
                query = query.Where(o => o.CTe.ModeloDocumentoFiscal.TipoDocumentoEmissao == Dominio.Enumeradores.TipoDocumento.CTe);

            if (quantidadeRegistros.HasValue && quantidadeRegistros.Value > 0)
                return query.Select(o => o.Codigo).Take(quantidadeRegistros.Value).ToList();

            return query.Select(o => o.Codigo).ToList();
        }


        public List<int> BuscarCodigosPorOcorrenciaSemIntegracao(int codigoOcorrencia, Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoIntegracao tipoIntegracao, int quantidadeRegistros)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.CargaCTe>();

            var subQueryIntegracoes = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.CargaCTeIntegracao>().Where(o => o.CargaCTe.CargaCTeComplementoInfo.CargaOcorrencia.Codigo == codigoOcorrencia && o.TipoIntegracao.Tipo == tipoIntegracao).Select(o => o.CargaCTe.Codigo);

            query = query.Where(o => o.CargaCTeComplementoInfo.CargaOcorrencia.Codigo == codigoOcorrencia && !subQueryIntegracoes.Contains(o.Codigo));

            if (tipoIntegracao == Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoIntegracao.Natura)
                query = query.Where(o => o.CTe.ModeloDocumentoFiscal.TipoDocumentoEmissao != Dominio.Enumeradores.TipoDocumento.Outros);

            return query.Select(o => o.Codigo).Take(quantidadeRegistros).ToList();
        }

        public int ContarPorCargaSemIntegracao(int codigoCarga)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.CargaCTe>();

            var subQueryIntegracoes = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.CargaCTeIntegracao>().Where(o => o.CargaCTe.Carga.Codigo == codigoCarga).Select(o => o.CargaCTe.Codigo);

            query = query.Where(o => o.Carga.Codigo == codigoCarga && !subQueryIntegracoes.Contains(o.Codigo));

            return query.Select(o => o.Codigo).Count();
        }

        public bool CargaRecebidaPorIntegracao(int codigoCTe)
        {
            IQueryable<Dominio.Entidades.Embarcador.Cargas.CargaCTe> query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.CargaCTe>();

            query = query.Where(o => o.CTe.Codigo == codigoCTe && o.Carga.CargaRecebidaDeIntegracao == true);

            return query.Any();
        }

        public bool PossuiCTeIntegradoComTotvs(int carga)
        {
            IQueryable<Dominio.Entidades.Embarcador.Cargas.CargaCTe> query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.CargaCTe>();

            query = query.Where(o => o.Carga.Codigo == carga && o.CTe.CodigoIntegracao != "" && o.CTe.CodigoIntegracao != null && o.CTe.CodigoCompanhia != null && o.CTe.CodigoCompanhia != "");

            return query.Any();
        }

        public bool PossuiCTeSemIntegracaoOcorrencia(int codigoOcorrencia, Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoIntegracao tipoIntegracao)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.CargaCTe>();

            var subQueryIntegracoes = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Ocorrencias.OcorrenciaCTeIntegracao>().Where(o => o.CargaCTe.CargaCTeComplementoInfo.CargaOcorrencia.Codigo == codigoOcorrencia && o.TipoIntegracao.Tipo == tipoIntegracao).Select(o => o.CargaCTe.Codigo);

            query = query.Where(o => o.CargaCTeComplementoInfo.CargaOcorrencia.Codigo == codigoOcorrencia && !subQueryIntegracoes.Contains(o.Codigo));

            return query.Select(o => o.Codigo).Any();
        }

        public bool PossuiCTeComIntegracaoOcorrencia(int codigoOcorrencia, Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoIntegracao tipoIntegracao)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.CargaCTe>();

            var subQueryIntegracoes = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Ocorrencias.OcorrenciaCTeIntegracao>().Where(o => o.CargaCTe.CargaCTeComplementoInfo.CargaOcorrencia.Codigo == codigoOcorrencia && o.TipoIntegracao.Tipo == tipoIntegracao).Select(o => o.CargaCTe.Codigo);

            query = query.Where(o => o.CargaCTeComplementoInfo.CargaOcorrencia.Codigo == codigoOcorrencia && subQueryIntegracoes.Contains(o.Codigo));

            return query.Select(o => o.Codigo).Any();
        }

        public bool PossuiCTeSemIntegracao(int codigoCarga, Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoIntegracao tipoIntegracao, bool validarCTERecusado = false)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.CargaCTe>();

            var subQueryIntegracoes = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.CargaCTeIntegracao>().Where(o => o.CargaCTe.Carga.Codigo == codigoCarga && o.TipoIntegracao.Tipo == tipoIntegracao).Select(o => o.CargaCTe.Codigo);

            if (validarCTERecusado)
                query = query.Where(o => o.SituacaoCheckin != SituacaoCheckin.RecusaAprovada);

            query = query.Where(o => o.Carga.Codigo == codigoCarga && o.CTe != null && !subQueryIntegracoes.Contains(o.Codigo));

            return query.Select(o => o.Codigo).Any();
        }

        public bool PossuiDocumentoSemIntegracao(int codigoCarga, Dominio.Enumeradores.TipoDocumento tipoDocumento, Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoIntegracao tipoIntegracao)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.CargaCTe>();

            var subQueryIntegracoes = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.CargaCTeIntegracao>().Where(o => o.CargaCTe.Carga.Codigo == codigoCarga && o.TipoIntegracao.Tipo == tipoIntegracao).Select(o => o.CargaCTe.Codigo);

            query = query.Where(o => o.CTe.ModeloDocumentoFiscal.TipoDocumentoEmissao == tipoDocumento && o.Carga.Codigo == codigoCarga && !subQueryIntegracoes.Contains(o.Codigo));

            return query.Select(o => o.Codigo).Any();
        }

        public bool PossuiCTeComIntegracao(int codigoCarga, Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoIntegracao tipoIntegracao)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.CargaCTe>();

            var subQueryIntegracoes = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.CargaCTeIntegracao>().Where(o => o.CargaCTe.Carga.Codigo == codigoCarga && o.TipoIntegracao.Tipo == tipoIntegracao).Select(o => o.CargaCTe.Codigo);

            query = query.Where(o => o.Carga.Codigo == codigoCarga && subQueryIntegracoes.Contains(o.Codigo));

            return query.Select(o => o.Codigo).Any();
        }

        public bool PossuiCTePendenteDeAutorizacao(int codigoCarga)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.CargaCTe>();

            query = query.Where(o => o.Carga.Codigo == codigoCarga && o.CTe == null && o.PreCTe != null);

            return query.Select(o => o.Codigo).Any();
        }

        public bool PossuiPreCTe(int codigoCarga)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.CargaCTe>();

            query = query.Where(o => o.Carga.Codigo == codigoCarga && o.PreCTe != null);

            return query.Select(o => o.Codigo).Any();
        }

        public Dominio.Entidades.Embarcador.Cargas.CargaCTe BuscarPrimeirPorCarga(int codigoCarga)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.CargaCTe>();

            var resut = from obj in query where obj.Carga.Codigo == codigoCarga select obj;

            return resut.FirstOrDefault();
        }

        public int BuscarPrimeiroCodigoPorCarga(int codigoCarga)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.CargaCTe>();

            var resut = from obj in query where obj.Carga.Codigo == codigoCarga select obj.Codigo;

            return resut.FirstOrDefault();
        }

        public int BuscarCodigoPorCte(int codigoCte, int codigoCarga)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.CargaCTe>();

            var resut = from obj in query where obj.Carga.Codigo == codigoCarga && obj.CTe.Codigo == codigoCte select obj.Codigo;

            return resut.FirstOrDefault();
        }

        public int BuscarPrimeiroCodigoPorOcorrencia(int codigoOcorrencia)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.CargaCTe>();

            var resut = from obj in query where obj.CargaCTeComplementoInfo.CargaOcorrencia.Codigo == codigoOcorrencia select obj.Codigo;

            return resut.FirstOrDefault();
        }

        public int ContarPorCarga(int codigoCarga, string[] statusCTe, int minutosLimiteEmissao = 0)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.CargaCTe>();

            var result = from obj in query where obj.Carga.Codigo == codigoCarga && statusCTe.Contains(obj.CTe.Status) select obj;

            if (minutosLimiteEmissao > 0)
                result = result.Where(o => o.CTe.DataIntegracao < DateTime.Now.AddMinutes(-minutosLimiteEmissao));

            return result.Count();
        }

        public List<Dominio.Entidades.ConhecimentoDeTransporteEletronico> BuscarCTesPorCargaETempoLimiteEmissao(int codigoCarga, bool ctesSubContratacaoFilialEmissora, int minutosLimiteEmissao)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.CargaCTe>();

            var result = from obj in query
                         where obj.Carga.Codigo == codigoCarga &&
                               obj.CTe.DataIntegracao < DateTime.Now.AddMinutes(-minutosLimiteEmissao) && obj.CTe.Status.Equals("E")
                         select obj;

            if (ctesSubContratacaoFilialEmissora)
                result = result.Where(obj => obj.CargaCTeFilialEmissora != null || (obj.CargaCTeFilialEmissora == null && obj.CargaCTeSubContratacaoFilialEmissora == null));
            else
                result = result.Where(obj => obj.CargaCTeSubContratacaoFilialEmissora == null);

            return result.Select(obj => obj.CTe).ToList();
        }

        public List<Dominio.Entidades.Estado> BuscarEstadosDestinoPrestacao(int codigoCarga)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.CargaCTe>();

            var result = from obj in query where obj.Carga.Codigo == codigoCarga && obj.CTe != null select obj.CTe.LocalidadeTerminoPrestacao.Estado;

            return result.Distinct().ToList();
        }

        public List<Dominio.Entidades.Estado> BuscarEstadosOrigemPrestacao(int codigoCarga)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.CargaCTe>();

            var result = from obj in query where obj.Carga.Codigo == codigoCarga && obj.CTe != null select obj.CTe.LocalidadeInicioPrestacao.Estado;

            return result.Distinct().ToList();
        }

        public int ContarPorCargaEComEmissaoMDFe(int codigoCarga)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.CargaCTe>();

            var result = from obj in query where obj.Carga.Codigo == codigoCarga && obj.CTe != null && obj.CTe.LocalidadeInicioPrestacao.Estado.Sigla != obj.CTe.LocalidadeTerminoPrestacao.Estado.Sigla select obj.Codigo;

            return result.Count();
        }

        public List<Dominio.Entidades.Embarcador.Cargas.CargaCTe> BuscarCTeComplmentarPorCarga(int codigoCarga)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.CargaCTe>();
            var resut = from obj in query where obj.Carga.Codigo == codigoCarga select obj;

            resut = resut.Where(obj => obj.CTe.TipoCTE == Dominio.Enumeradores.TipoCTE.Complemento);

            return resut.ToList();
        }

        public List<Dominio.Entidades.Embarcador.Cargas.CargaCTe> BuscarCTesComplementaresNasIntegrados(int inicio, int limite)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.CargaCTe>();
            var resut = from obj in query where obj.CTe.TipoCTE == Dominio.Enumeradores.TipoCTE.Complemento && (obj.CTe.Status == "A" || obj.CTe.Status == "C") && !obj.CargaCTeComplementoInfo.ComplementoIntegradoEmbarcador select obj;

            return resut.Skip(inicio).Take(limite).ToList();
        }

        public List<Dominio.Entidades.ConhecimentoDeTransporteEletronico> BuscarPorLocalidadeTerminoPrestacao(int codigoCarga, int empresa, int codigoTerminoPrestacao, AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware tipoServicoMultisoftware)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.CargaCTe>();

            var resut = from obj in query where obj.Carga.Codigo == codigoCarga && obj.CTe.Status == "A" && obj.CTe.LocalidadeTerminoPrestacao.Codigo == codigoTerminoPrestacao && obj.CTe.ModeloDocumentoFiscal.Numero == "57" select obj.CTe;

            if (empresa > 0)
                resut = resut.Where(obj => obj.Empresa.Codigo == empresa);

            return resut
                .Fetch(obj => obj.LocalidadeInicioPrestacao)
                .ThenFetch(obj => obj.Estado)
                .Fetch(obj => obj.LocalidadeTerminoPrestacao)
                .ThenFetch(obj => obj.Estado)
                .ToList();
        }

        public List<Dominio.Entidades.Embarcador.Cargas.CargaCTe> BuscarCTesSemCanhotoGeradoPorGrupoPessoas(int codigoGrupoPessoas)
        {
            Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoCarga[] situacoes = new Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoCarga[]
            {
                  Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoCarga.EmTransporte,
                   Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoCarga.Encerrada
            };

            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.CargaCTe>();
            var queryCanhotos = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Canhotos.Canhoto>();

            query = query.Where(o => o.CTe.Status == "A" && situacoes.Contains(o.Carga.SituacaoCarga) && !o.Carga.CargaTransbordo && o.CTe.TomadorPagador.GrupoPessoas.Codigo == codigoGrupoPessoas && (o.CTe.TomadorPagador.Cliente.ArmazenaCanhotoFisicoCTe == true || o.CTe.TomadorPagador.Cliente.GrupoPessoas.ArmazenaCanhotoFisicoCTe == true));
            query = query.Where(o => !queryCanhotos.Any(c => c.CargaCTe.Codigo == o.Codigo));

            return query.WithOptions(o => { o.SetTimeout(6000); }).ToList();
        }

        public List<Dominio.Entidades.Embarcador.Cargas.CargaCTe> BuscarCTesSemCanhotoGerado(bool gerarCanhotoSempre)
        {
            Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoCarga[] situacoes = new Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoCarga[]
            {
                  Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoCarga.EmTransporte,
                   Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoCarga.Encerrada
            };

            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.CargaCTe>();
            var queryCanhotos = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Canhotos.Canhoto>();

            query = query.Where(o => o.CTe.ModeloDocumentoFiscal.TipoDocumentoEmissao != Dominio.Enumeradores.TipoDocumento.NFS &&
                                     o.CTe.Status == "A" && situacoes.Contains(o.Carga.SituacaoCarga) &&
                                     !o.Carga.CargaTransbordo &&
                                     (gerarCanhotoSempre || o.CTe.TomadorPagador.Cliente.ArmazenaCanhotoFisicoCTe == true || o.CTe.TomadorPagador.Cliente.GrupoPessoas.ArmazenaCanhotoFisicoCTe == true));

            query = query.Where(o => !queryCanhotos.Any(c => c.CargaCTe.Codigo == o.Codigo));

            return query.WithOptions(o => { o.SetTimeout(6000); }).ToList();
        }

        public List<Dominio.Entidades.ConhecimentoDeTransporteEletronico> BuscarPorTipoModal(int codigoCarga, int empresa, AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware tipoServicoMultisoftware, Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoModal tipoModal)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.CargaCTe>();

            var resut = from obj in query where obj.Carga.Codigo == codigoCarga && obj.CTe.Empresa.Codigo == empresa && obj.CTe.TipoModal == tipoModal select obj.CTe;

            if (tipoServicoMultisoftware == AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiTMS)
                resut = resut.Where(obj => obj.TipoServico != Dominio.Enumeradores.TipoServico.SubContratacao);

            return resut.ToList();
        }

        public List<Dominio.Entidades.ConhecimentoDeTransporteEletronico> BuscarPorEstadoTerminoPrestacao(int codigoCarga, int empresa, string estadoTerminoPrestacao, AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware tipoServicoMultisoftware)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.CargaCTe>();

            var resut = from obj in query
                        where obj.Carga.Codigo == codigoCarga &&
                              obj.CTe.LocalidadeTerminoPrestacao.Estado.Sigla == estadoTerminoPrestacao && obj.CTe.Status == "A" && obj.CTe.ModeloDocumentoFiscal.Numero == "57" &&
                              obj.CargaCTeComplementoInfo == null
                        select obj.CTe;

            if (empresa > 0)
                resut = resut.Where(obj => obj.Empresa.Codigo == empresa);

            return resut
                .Fetch(obj => obj.LocalidadeInicioPrestacao)
                .ThenFetch(obj => obj.Estado)
                .Fetch(obj => obj.LocalidadeTerminoPrestacao)
                .ThenFetch(obj => obj.Estado)
                .ToList();
        }

        public List<Dominio.Entidades.ConhecimentoDeTransporteEletronico> BuscarPorEstadoTerminoPrestacaoCargaOrigem(int codigoCarga, int empresa, string estadoTerminoPrestacao, AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware tipoServicoMultisoftware)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.CargaCTe>();

            var resut = from obj in query
                        where obj.CargaOrigem.Codigo == codigoCarga &&
                              obj.CTe.LocalidadeTerminoPrestacao.Estado.Sigla == estadoTerminoPrestacao && obj.CTe.Status == "A" && obj.CTe.ModeloDocumentoFiscal.Numero == "57" &&
                              obj.CargaCTeComplementoInfo == null
                        select obj.CTe;

            if (empresa > 0)
                resut = resut.Where(obj => obj.Empresa.Codigo == empresa);

            return resut
                .Fetch(obj => obj.LocalidadeInicioPrestacao)
                .ThenFetch(obj => obj.Estado)
                .Fetch(obj => obj.LocalidadeTerminoPrestacao)
                .ThenFetch(obj => obj.Estado)
                .ToList();
        }

        public int ContarCTesComplementaresNaoIntegrados()
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.CargaCTe>();
            var resut = from obj in query where obj.CTe.TipoCTE == Dominio.Enumeradores.TipoCTE.Complemento && (obj.CTe.Status == "A" || obj.CTe.Status == "C") && !obj.CargaCTeComplementoInfo.ComplementoIntegradoEmbarcador select obj;

            return resut.Count();
        }

        public int ContarPorCarga(int codigoCarga)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.CargaCTe>();
            var resut = from obj in query where obj.Carga.Codigo == codigoCarga select obj;
            return resut.Count();
        }

        public int ContarPorCargaQueGeraFaturamento(int codigoCarga)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.CargaCTe>();

            query = query.Where(obj => obj.Carga.Codigo == codigoCarga && !obj.CTe.ModeloDocumentoFiscal.NaoGerarFaturamento);
            //query = query.Where(o => o.Carga.TipoOperacao == null || !o.Carga.TipoOperacao.NaoGerarFaturamento);           

            return query.Count();
        }

        public bool CargaGeraFaturamento(int codigoCarga)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.CargaCTe>();

            query = query.Where(obj => obj.Carga.Codigo == codigoCarga && !obj.CTe.ModeloDocumentoFiscal.NaoGerarFaturamento);

            return query.Any();
        }

        public int ContarPorCargaETomador(int codigoCarga, double tomador)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.CargaCTe>();
            var resut = from obj in query where obj.Carga.Codigo == codigoCarga select obj;

            if (tomador > 0)
                resut = resut.Where(obj => obj.CTe.TomadorPagador.Cliente.CPF_CNPJ == tomador);

            return resut.Count();
        }

        public int ContarPorOcorrencia(int codigoOcorrencia)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.CargaCTe>();
            var resut = from obj in query where obj.CargaCTeComplementoInfo.CargaOcorrencia.Codigo == codigoOcorrencia select obj;
            return resut.Count();
        }

        public Dominio.Entidades.Embarcador.Cargas.CargaCTe BuscarPorPreCte(int codigoPreCTe)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.CargaCTe>();
            var resut = from obj in query where obj.PreCTe.Codigo == codigoPreCTe select obj;
            return resut.FirstOrDefault();
        }

        public Dominio.Entidades.Embarcador.Cargas.CargaCTe BuscarPorChaveCTe(string chave)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.CargaCTe>();
            var resut = from obj in query where obj.CTe.Chave == chave select obj;
            return resut.FirstOrDefault();
        }
        public Dominio.Entidades.Embarcador.Cargas.CargaCTe BuscarPorChaveCTeSituacao(string chave, string[] status = null)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.CargaCTe>();
            var resut = from obj in query where obj.CTe.Chave == chave && status.Contains(obj.CTe.Status) select obj;
            return resut.FirstOrDefault();
        }
        public Dominio.Entidades.Embarcador.Cargas.CargaCTe BuscarPorChaveCTeComCargaAtiva(string chave)
        {
            IQueryable<Dominio.Entidades.Embarcador.Cargas.CargaCTe> query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.CargaCTe>();

            query = query.Where(o => o.CTe.Chave == chave && !o.Carga.CargaTransbordo &&
                                     (o.Carga.SituacaoCarga == Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoCarga.Encerrada ||
                                      o.Carga.SituacaoCarga == Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoCarga.EmTransporte ||
                                      o.Carga.SituacaoCarga == Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoCarga.AgIntegracao ||
                                      o.Carga.SituacaoCarga == Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoCarga.AgImpressaoDocumentos));

            return query.FirstOrDefault();
        }
        public List<Dominio.Entidades.Embarcador.Cargas.CargaCTe> BuscarPorChaveDocumentoAnterior(int codigoCarga)
        {
            IQueryable<Dominio.Entidades.Embarcador.Cargas.CargaCTe> query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.CargaCTe>();

            query = query.Where(obj => obj.Carga.Codigo == codigoCarga);

            return query.ToList();
        }

        public Dominio.Entidades.Embarcador.Cargas.CargaCTe BuscarPreCTePorCargaEChaveCTeSub(int carga, string chaveCTE)
        {
            return BuscarPorCodigoDeCargaEChaveCTeSub(carga, chaveCTE).FirstOrDefault();
        }

        public Dominio.Entidades.Embarcador.Cargas.CargaCTe BuscarPreCTePorCargaEChaveCTeSub(int carga, string chaveCTE, string cnpj)
        {
            var result = BuscarPorCodigoDeCargaEChaveCTeSub(carga, chaveCTE);

            if (cnpj != null)
                result = result.Where(ccte => ccte.PreCTe.Empresa.CNPJ.Equals(cnpj));

            return result.FirstOrDefault();
        }

        public Dominio.Entidades.Embarcador.Cargas.CargaCTe BuscarPreCTePorCargaEChaveNFE(int carga, string chaveNFe)
        {
            return BuscarPorCodigoDeCargaEChaveNFE(carga, chaveNFe).FirstOrDefault();
        }

        public Dominio.Entidades.Embarcador.Cargas.CargaCTe BuscarPreCTePorCargaEChaveNFE(int carga, string chaveNFe, string cnpj)
        {
            var result = BuscarPorCodigoDeCargaEChaveNFE(carga, chaveNFe);

            if (cnpj != null)
                result = result.Where(ccte => ccte.PreCTe.Empresa.CNPJ.Equals(cnpj));

            return result.FirstOrDefault();
        }

        public Dominio.Entidades.Embarcador.Cargas.CargaCTe BuscarPreCTePorCargaENumeroOutroDoc(int carga, string numero)
        {
            return BuscarPorCodigoDeCargaENumeroOutroDoc(carga, numero).FirstOrDefault();
        }

        public Dominio.Entidades.Embarcador.Cargas.CargaCTe BuscarPreCTePorCargaENumeroOutroDoc(int carga, string numero, string cnpj)
        {
            var result = BuscarPorCodigoDeCargaENumeroOutroDoc(carga, numero);

            if (cnpj != null)
                result = result.Where(ccte => ccte.PreCTe.Empresa.CNPJ.Equals(cnpj));

            return result.FirstOrDefault();
        }

        public List<Dominio.Entidades.Embarcador.Cargas.CargaCTe> BuscarPorCodigosCTes(List<int> codigoCTe)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.CargaCTe>();
            var resut = from obj in query where codigoCTe.Contains(obj.CTe.Codigo) select obj;
            return resut
                .Fetch(obj => obj.CTe)
                .ThenFetch(obj => obj.CentroResultado)
                .Fetch(obj => obj.CTe)
                .ThenFetch(obj => obj.CentroResultadoEscrituracao)
                .Fetch(obj => obj.CTe)
                .ThenFetch(obj => obj.CentroResultadoDestinatario)
                .Fetch(obj => obj.CTe)
                .ThenFetch(obj => obj.ModeloDocumentoFiscal)
                .Fetch(obj => obj.CTe)
                .ThenFetch(obj => obj.Remetente)
                .ThenFetch(obj => obj.Localidade)
                .Fetch(obj => obj.CTe)
                .ThenFetch(obj => obj.Destinatario)
                .ThenFetch(obj => obj.Localidade)
                .Fetch(obj => obj.CTe)
                .ThenFetch(obj => obj.Expedidor)
                .ThenFetch(obj => obj.Localidade)
                .Fetch(obj => obj.CTe)
                .ThenFetch(obj => obj.Recebedor)
                .ThenFetch(obj => obj.Localidade)
                .Fetch(obj => obj.CTe)
                .ThenFetch(obj => obj.OutrosTomador)
                .ThenFetch(obj => obj.Localidade)
                .Fetch(obj => obj.CTe)
                .ThenFetch(obj => obj.TomadorPagador)
                .ThenFetch(obj => obj.Localidade)
                .Fetch(obj => obj.CTe)
                .ThenFetch(obj => obj.TomadorPagador)
                .ThenFetch(obj => obj.GrupoPessoas)
                .Fetch(obj => obj.CTe)
                .ThenFetch(obj => obj.LocalidadeInicioPrestacao)
                .Fetch(obj => obj.CTe)
                .ThenFetch(obj => obj.LocalidadeTerminoPrestacao)
                .ToList();
        }

        public List<Dominio.Entidades.Embarcador.Cargas.CargaCTe> BuscarPorCodigoCTe(int codigoCTe)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.CargaCTe>();
            var resut = from obj in query where obj.CTe.Codigo == codigoCTe select obj;
            return resut.ToList();
        }

        public Dominio.Entidades.Embarcador.Cargas.CargaCTe BuscarPorCTe(int codigoCTe, int codigoEmpresa = 0)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.CargaCTe>();
            var resut = from obj in query where obj.CTe.Codigo == codigoCTe select obj;

            if (codigoEmpresa > 0)
                resut = resut.Where(c => c.CTe.Empresa.Codigo == codigoEmpresa);

            return resut
                .Fetch(obj => obj.CTe)
                .ThenFetch(obj => obj.CentroResultado)
                .Fetch(obj => obj.CTe)
                .ThenFetch(obj => obj.CentroResultadoEscrituracao)
                .Fetch(obj => obj.CTe)
                .ThenFetch(obj => obj.CentroResultadoDestinatario)
                .Fetch(obj => obj.CTe)
                .ThenFetch(obj => obj.ModeloDocumentoFiscal)
                .Fetch(obj => obj.CTe)
                .ThenFetch(obj => obj.Remetente)
                .ThenFetch(obj => obj.Localidade)
                .Fetch(obj => obj.CTe)
                .ThenFetch(obj => obj.Destinatario)
                .ThenFetch(obj => obj.Localidade)
                .Fetch(obj => obj.CTe)
                .ThenFetch(obj => obj.Expedidor)
                .ThenFetch(obj => obj.Localidade)
                .Fetch(obj => obj.CTe)
                .ThenFetch(obj => obj.Recebedor)
                .ThenFetch(obj => obj.Localidade)
                .Fetch(obj => obj.CTe)
                .ThenFetch(obj => obj.OutrosTomador)
                .ThenFetch(obj => obj.Localidade)
                .Fetch(obj => obj.CTe)
                .ThenFetch(obj => obj.TomadorPagador)
                .ThenFetch(obj => obj.Localidade)
                .Fetch(obj => obj.CTe)
                .ThenFetch(obj => obj.TomadorPagador)
                .ThenFetch(obj => obj.GrupoPessoas)
                .Fetch(obj => obj.CTe)
                .ThenFetch(obj => obj.LocalidadeInicioPrestacao)
                .Fetch(obj => obj.CTe)
                .ThenFetch(obj => obj.LocalidadeTerminoPrestacao)
                .FirstOrDefault();
        }

        public Task<Dominio.Entidades.Embarcador.Cargas.CargaCTe> BuscarPorCTeAsync(int codigoCTe, CancellationToken cancellationToken, int codigoEmpresa = 0)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.CargaCTe>();
            var resut = from obj in query where obj.CTe.Codigo == codigoCTe select obj;

            if (codigoEmpresa > 0)
                resut = resut.Where(c => c.CTe.Empresa.Codigo == codigoEmpresa);

            return resut
                .Fetch(obj => obj.CTe)
                .ThenFetch(obj => obj.CentroResultado)
                .Fetch(obj => obj.CTe)
                .ThenFetch(obj => obj.CentroResultadoEscrituracao)
                .Fetch(obj => obj.CTe)
                .ThenFetch(obj => obj.CentroResultadoDestinatario)
                .Fetch(obj => obj.CTe)
                .ThenFetch(obj => obj.ModeloDocumentoFiscal)
                .Fetch(obj => obj.CTe)
                .ThenFetch(obj => obj.Remetente)
                .ThenFetch(obj => obj.Localidade)
                .Fetch(obj => obj.CTe)
                .ThenFetch(obj => obj.Destinatario)
                .ThenFetch(obj => obj.Localidade)
                .Fetch(obj => obj.CTe)
                .ThenFetch(obj => obj.Expedidor)
                .ThenFetch(obj => obj.Localidade)
                .Fetch(obj => obj.CTe)
                .ThenFetch(obj => obj.Recebedor)
                .ThenFetch(obj => obj.Localidade)
                .Fetch(obj => obj.CTe)
                .ThenFetch(obj => obj.OutrosTomador)
                .ThenFetch(obj => obj.Localidade)
                .Fetch(obj => obj.CTe)
                .ThenFetch(obj => obj.TomadorPagador)
                .ThenFetch(obj => obj.Localidade)
                .Fetch(obj => obj.CTe)
                .ThenFetch(obj => obj.TomadorPagador)
                .ThenFetch(obj => obj.GrupoPessoas)
                .Fetch(obj => obj.CTe)
                .ThenFetch(obj => obj.LocalidadeInicioPrestacao)
                .Fetch(obj => obj.CTe)
                .ThenFetch(obj => obj.LocalidadeTerminoPrestacao)
                .FirstOrDefaultAsync(cancellationToken);
        }

        public List<Dominio.Entidades.Embarcador.Pedidos.PedidoAnexo> BuscarAnexosPorCTe(int codigoCTe)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.CargaCTe>();
            query = query.Where(obj => obj.CTe.Codigo == codigoCTe);

            var queryPedido = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.CargaPedido>();
            queryPedido = queryPedido.Where(obj => query.Any(c => c.Carga == obj.Carga));

            var queryAnexo = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Pedidos.PedidoAnexo>();
            queryAnexo = queryAnexo.Where(obj => queryPedido.Any(c => c.Pedido.Codigo == obj.EntidadeAnexo.Codigo));

            return queryAnexo.ToList();
        }

        public List<Dominio.Entidades.Embarcador.Cargas.CargaCTe> BuscarPorPedidosNotasFiscais(List<int> notasFiscais)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.CargaCTe>();
            var resut = from obj in query where obj.NotasFiscais.Any(o => notasFiscais.Contains(o.PedidoXMLNotaFiscal.Codigo)) select obj;
            return resut.ToList();
        }

        public List<Dominio.Entidades.Embarcador.Cargas.CargaCTe> BuscarPorNotasFiscais(List<int> notasFiscais)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.CargaCTe>();
            var resut = from obj in query where obj.NotasFiscais.Any(o => notasFiscais.Contains(o.PedidoXMLNotaFiscal.XMLNotaFiscal.Codigo)) select obj;
            return resut.ToList();
        }

        public Dominio.Entidades.Embarcador.Cargas.CargaCTe BuscarUltimaCargaDoCTe(int codigoCTe)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.CargaCTe>();
            var resut = from obj in query where obj.CTe.Codigo == codigoCTe select obj;
            return resut.OrderBy("Carga.Codigo descending").FirstOrDefault();
        }

        public List<Dominio.Entidades.Cliente> BuscarRemetentesPorCargaPedido(IEnumerable<int> codigosCargaPedido)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.CargaCTe>();

            query = query.Where(o => o.NotasFiscais.Any(nf => codigosCargaPedido.Contains(nf.PedidoXMLNotaFiscal.CargaPedido.Codigo)));

            return query.Select(o => o.CTe.Remetente.Cliente).Distinct().ToList();
        }

        public List<Dominio.Entidades.Embarcador.Cargas.CargaCTe> BuscarPorCTeCarga(int codigoCTe, int carga)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.CargaCTe>();
            var resut = from obj in query where obj.CTe.Codigo == codigoCTe && obj.Carga.Codigo == carga select obj;
            return resut.ToList();
        }

        public Dominio.Entidades.Embarcador.Cargas.CargaCTe BuscarPorCTeECarga(int codigoCTe, int carga)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.CargaCTe>();
            var resut = from obj in query where obj.CTe.Codigo == codigoCTe && obj.Carga.Codigo == carga select obj;
            return resut.FirstOrDefault();
        }

        public Dominio.Entidades.Embarcador.Pedidos.Pedido BuscarPedidoPorCTe(int codigoCTe)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.CargaCTe>();
            query = query.Where(o => o.CTe.Codigo == codigoCTe);

            var queryCargaPedido = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.CargaPedido>();
            queryCargaPedido = queryCargaPedido.Where(o => query.Any(c => c.Carga == o.Carga));

            return queryCargaPedido.Select(p => p.Pedido)?.FirstOrDefault() ?? null;
        }

        public Dominio.Entidades.Embarcador.Cargas.CargaCTe BuscarPorCargaPedido(int codigoCargaPedido)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.CargaCTe>();
            query = query.Where(o => o.NotasFiscais.Any(nf => nf.PedidoXMLNotaFiscal.CargaPedido.Codigo == codigoCargaPedido));

            return query.FirstOrDefault();
        }

        public List<Dominio.Entidades.Embarcador.Cargas.CargaCTe> BuscarTodosPorCargaPedido(int codigoCargaPedido, bool apenasCTesNormais)
        {
            var consultaCargaCTe = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.CargaCTe>()
                .Where(o => o.NotasFiscais.Any(nf => nf.PedidoXMLNotaFiscal.CargaPedido.Codigo == codigoCargaPedido));

            if (apenasCTesNormais)
                consultaCargaCTe = consultaCargaCTe.Where(obj => obj.CargaCTeComplementoInfo == null);

            return consultaCargaCTe.ToList();
        }

        public bool ContemCTeCanceladoInutilizadoPorCarga(int codigoCarga)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.CargaCTe>();

            query = query.Where(obj => obj.Carga.Codigo == codigoCarga && (obj.CTe.Status == "C" || obj.CTe.Status == "I" || obj.CTe.Status == "E" || obj.CTe.Status == "D" || obj.CTe.Status == "K" || obj.CTe.Status == "L" || obj.CTe.Status == "Z"));

            return query.Any();
        }

        public bool ContemCTeAutorizadoPorCarga(int codigoCarga)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.CargaCTe>();

            query = query.Where(obj => obj.Carga.Codigo == codigoCarga && obj.CTe.Status == "A");

            return query.Any();
        }

        public bool ContemCTeAquaviario(int codigoCarga)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.CargaCTe>();

            query = query.Where(obj => obj.Carga.Codigo == codigoCarga && obj.CTe.TipoModal == TipoModal.Aquaviario);

            return query.Any();
        }

        public List<Dominio.Entidades.ConhecimentoDeTransporteEletronico> BuscarCTePorCarga(int codigoCarga)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.CargaCTe>();

            query = query.Where(obj => obj.Carga.Codigo == codigoCarga);

            return query.Select(o => o.CTe).ToList();
        }

        public async Task<List<Dominio.Entidades.ConhecimentoDeTransporteEletronico>> BuscarCTePorCargaAsync(int codigoCarga, CancellationToken cancellationToken)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.CargaCTe>();

            query = query.Where(obj => obj.Carga.Codigo == codigoCarga);

            return await query.Select(o => o.CTe).ToListAsync(cancellationToken);
        }

        public List<Dominio.Entidades.Embarcador.Cargas.CargaCTe> BuscarCargaCTePorCarga(int codigoCarga)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.CargaCTe>();

            query = query.Where(obj => obj.Carga.Codigo == codigoCarga);

            return query.ToList();
        }

        public Dominio.Entidades.ModeloDocumentoFiscal BuscarPrimeirModeloDocumentoPorCarga(int codigoCarga)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.CargaCTe>();

            query = query.Where(obj => obj.Carga.Codigo == codigoCarga);

            return query.Select(o => o.CTe.ModeloDocumentoFiscal)?.FirstOrDefault() ?? null;
        }

        public Dominio.Entidades.ConhecimentoDeTransporteEletronico BuscarPrimeiroCTePorCarga(int codigoCarga)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.CargaCTe>();

            query = query.Where(obj => obj.CargaOrigem.Codigo == codigoCarga && obj.CargaCTeComplementoInfo == null);

            return query.Select(o => o.CTe).FirstOrDefault();
        }

        public Dominio.Entidades.ConhecimentoDeTransporteEletronico BuscarCTePorCTeECarga(int codigoCTe, int codigoCarga)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.CargaCTe>();

            query = query.Where(obj => obj.CTe.Codigo == codigoCTe && obj.Carga.Codigo == codigoCarga);

            return query.Select(o => o.CTe).FirstOrDefault();
        }

        public Dominio.ObjetosDeValor.Embarcador.Enumeradores.SistemaEmissor BuscarSistemaEmissorPorCarga(int codigoCarga)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.CargaCTe>();

            var resut = from obj in query
                        where obj.Carga.Codigo == codigoCarga && ((obj.Carga.Filial != null && obj.Carga.Filial.EmiteMDFeFilialEmissora == false && obj.CargaCTeSubContratacaoFilialEmissora == null) || (obj.Carga.Filial != null && (obj.Carga.Filial.EmiteMDFeFilialEmissora || obj.Carga.Filial.EmiteMDFeFilialEmissoraPorEstadoDestino) && obj.CargaCTeFilialEmissora == null) || (obj.Carga.Filial == null))
                        select obj.SistemaEmissor;

            return resut.FirstOrDefault();
        }

        public int BuscarNumeroPrimeiroCTeCarga(int codigoCarga)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.CargaCTe>();
            query = query.Where(o => o.Carga.Codigo == codigoCarga);
            return query.OrderBy(o => o.Codigo).Select(o => o.CTe.Numero).FirstOrDefault();
        }

        public List<Dominio.Entidades.Embarcador.Cargas.CargaCTe> ConsultarPreCTes(int carga, int numeroNF, bool ctesSubContratacaoFilialEmissora, bool ctesSemSubContratacaoFilialEmissora, string propOrdenacao, string dirOrdenacao, int inicioRegistros, int maximoRegistros)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.CargaCTe>();

            var result = from obj in query where obj.Carga.Codigo == carga && obj.PreCTe != null select obj;

            if (numeroNF > 0)
                result = result.Where(obj => obj.NotasFiscais.Any(doc => doc.PedidoXMLNotaFiscal.XMLNotaFiscal.Numero == numeroNF));

            return result.Fetch(o => o.PreCTe).ThenFetch(o => o.Remetente)
                         .Fetch(o => o.PreCTe).ThenFetch(o => o.Destinatario)
                         .Fetch(o => o.PreCTe).ThenFetch(o => o.LocalidadeTerminoPrestacao)
                         .Fetch(o => o.PreCTe).ThenFetch(o => o.ModeloDocumentoFiscal)
                         .OrderBy(propOrdenacao + (dirOrdenacao == "asc" ? " ascending" : " descending")).Skip(inicioRegistros).Take(maximoRegistros).ToList();
        }

        public int ContarConsultaPreCTes(int carga, int numeroNF, bool ctesSubContratacaoFilialEmissora, bool ctesSemSubContratacaoFilialEmissora)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.CargaCTe>();

            var result = from obj in query where obj.Carga.Codigo == carga && obj.PreCTe != null select obj;

            if (numeroNF > 0)
                result = result.Where(obj => obj.NotasFiscais.Any(doc => doc.PedidoXMLNotaFiscal.XMLNotaFiscal.Numero == numeroNF));

            if (ctesSubContratacaoFilialEmissora)
                result = result.Where(o => o.CargaCTeFilialEmissora != null || (o.CargaCTeFilialEmissora == null && o.CargaCTeSubContratacaoFilialEmissora == null));

            if (ctesSemSubContratacaoFilialEmissora)
                result = result.Where(o => o.CargaCTeFilialEmissora == null);


            return result.Count();
        }

        public List<Dominio.Entidades.Embarcador.Pedidos.Pedido> ConsultarPedidosPendenteParaEmissaoMDFeAquaviario(bool somenteCTesAquaviarios, int codigoPedidoViagemNavio, int cnpjPortoOrigem, int cnpjPortoDestinos)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Pedidos.Pedido>();

            var queryCargaPedido = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.CargaPedido>();
            queryCargaPedido = queryCargaPedido.Where(obj => obj.Carga.SituacaoCarga != SituacaoCarga.Anulada && obj.Carga.SituacaoCarga != SituacaoCarga.Cancelada);

            var queryCargaPedidoAtivo = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.CargaPedido>();
            queryCargaPedidoAtivo = queryCargaPedidoAtivo.Where(obj => obj.Carga.SituacaoCarga == Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoCarga.NaLogistica
                || obj.Carga.SituacaoCarga == Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoCarga.Nova
                || obj.Carga.SituacaoCarga == Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoCarga.AgNFe
                || obj.Carga.SituacaoCarga == Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoCarga.PendeciaDocumentos
                || obj.Carga.SituacaoCarga == Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoCarga.AgIntegracao
                || obj.Carga.SituacaoCarga == Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoCarga.CalculoFrete);

            query = query.Where(obj => !queryCargaPedido.Any(c => c.Pedido == obj) || queryCargaPedidoAtivo.Any(a => a.Pedido == obj));

            query = query.Where(obj => obj.TipoOperacao != null && obj.TipoOperacao.TipoPropostaMultimodal != TipoPropostaMultimodal.NoShowCabotagem
            && obj.TipoOperacao.TipoPropostaMultimodal != TipoPropostaMultimodal.FaturamentoContabilidade
            && obj.TipoOperacao.TipoPropostaMultimodal != TipoPropostaMultimodal.DemurrageCabotagem
            && obj.TipoOperacao.TipoPropostaMultimodal != TipoPropostaMultimodal.DetentionCabotagem
            && obj.TipoOperacao.TipoPropostaMultimodal != TipoPropostaMultimodal.TAkePayCabotagem && obj.TipoOperacao.TipoPropostaMultimodal != TipoPropostaMultimodal.TakePayFeeder);

            if (codigoPedidoViagemNavio > 0)
                query = query.Where(obj => obj.PedidoViagemNavio.Codigo == codigoPedidoViagemNavio);

            if (cnpjPortoOrigem > 0)
                query = query.Where(obj => obj.Porto.Codigo == cnpjPortoOrigem);

            if (cnpjPortoDestinos > 0)
                query = query.Where(obj => obj.PortoDestino.Codigo == cnpjPortoDestinos);

            query = query.Where(obj => obj.SituacaoPedido != Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoPedido.Cancelado
            && obj.SituacaoPedido != Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoPedido.Finalizado);

            return query.ToList();
        }

        public List<Dominio.Entidades.Embarcador.Cargas.Carga> ConsultarCargaPendenteParaEmissaoMDFeAquaviario(bool somenteCTesAquaviarios, int codigoPedidoViagemNavio, int cnpjPortoOrigem, int cnpjPortoDestinos)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.Carga>();

            query = query.Where(obj => obj.TipoOperacao != null && obj.TipoOperacao.TipoPropostaMultimodal != TipoPropostaMultimodal.NoShowCabotagem
            && obj.TipoOperacao.TipoPropostaMultimodal != TipoPropostaMultimodal.FaturamentoContabilidade
            && obj.TipoOperacao.TipoPropostaMultimodal != TipoPropostaMultimodal.DemurrageCabotagem
            && obj.TipoOperacao.TipoPropostaMultimodal != TipoPropostaMultimodal.DetentionCabotagem
            && obj.TipoOperacao.TipoPropostaMultimodal != TipoPropostaMultimodal.TAkePayCabotagem && obj.TipoOperacao.TipoPropostaMultimodal != TipoPropostaMultimodal.TakePayFeeder);

            if (codigoPedidoViagemNavio > 0)
                query = query.Where(obj => obj.PedidoViagemNavio.Codigo == codigoPedidoViagemNavio);

            if (cnpjPortoOrigem > 0)
                query = query.Where(obj => obj.PortoOrigem.Codigo == cnpjPortoOrigem);

            if (cnpjPortoDestinos > 0)
                query = query.Where(obj => obj.PortoDestino.Codigo == cnpjPortoDestinos);

            query = query.Where(obj => obj.SituacaoCarga == Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoCarga.NaLogistica
            || obj.SituacaoCarga == Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoCarga.Nova
            || obj.SituacaoCarga == Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoCarga.AgNFe
            || obj.SituacaoCarga == Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoCarga.PendeciaDocumentos
            || obj.SituacaoCarga == Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoCarga.AgIntegracao
            || obj.SituacaoCarga == Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoCarga.CalculoFrete);

            return query.ToList();
        }

        public List<Dominio.Entidades.Embarcador.Cargas.CargaCTe> ConsultarCTesParaEmissaoMDFeAquaviario(bool somenteCTesAquaviarios, int codigoPedidoViagemNavio, int cnpjPortoOrigem, int cnpjPortoDestinos, bool somenteAutorizados = true)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.CargaCTe>();

            query = query.Where(obj => obj.CTe != null && obj.CTe.Remetente != null && obj.CTe.TipoCTE == Dominio.Enumeradores.TipoCTE.Normal);

            if (!somenteAutorizados)
                query = query.Where(o => o.CTe != null && (o.CTe.Status == "P" || o.CTe.Status == "E" || o.CTe.Status == "S" || o.CTe.Status == "R") && o.Carga != null && o.Carga.SituacaoCarga != Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoCarga.Anulada && o.Carga.SituacaoCarga != Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoCarga.Cancelada);
            else
                query = query.Where(o => o.CTe != null && (o.CTe.Status == "A"));

            if (codigoPedidoViagemNavio > 0)
                query = query.Where(obj => obj.CTe.Viagem.Codigo == codigoPedidoViagemNavio);

            if (cnpjPortoOrigem > 0)
                query = query.Where(obj => obj.CTe.PortoOrigem.Codigo == cnpjPortoOrigem);

            if (cnpjPortoDestinos > 0)
                query = query.Where(obj => obj.CTe.PortoDestino.Codigo == cnpjPortoDestinos);

            if (somenteCTesAquaviarios)
                query = query.Where(obj => obj.CTe.TipoModal == Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoModal.Aquaviario);

            var queryMDFeManual = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.CargaMDFeManual>();
            queryMDFeManual = queryMDFeManual.Where(obj => obj.Situacao != Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoMDFeManual.Cancelado && obj.Situacao != Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoMDFeManual.Finalizado && obj.Situacao != Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoMDFeManual.EmDigitacao && obj.Situacao != Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoMDFeManual.Rejeicao);
            query = query.Where(obj => !queryMDFeManual.Any(o => o.CTes.Any(c => c == obj)));

            var queryCargaPedido = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.CargaPedido>();
            var queryPedidoTransbordo = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Pedidos.PedidoTransbordo>();
            queryCargaPedido = queryCargaPedido.Where(o => !queryPedidoTransbordo.Any(p => p.Pedido.Codigo == o.Pedido.Codigo));
            query = query.Where(o => queryCargaPedido.Any(p => p.Carga.Codigo == o.Carga.Codigo));

            return query.Timeout(7000).Fetch(o => o.CTe).ThenFetch(o => o.Remetente)
                        .Fetch(o => o.CTe).ThenFetch(o => o.Destinatario)
                        .Fetch(o => o.CTe).ThenFetch(o => o.Serie)
                        .Fetch(o => o.CTe).ThenFetch(o => o.LocalidadeTerminoPrestacao)
                        .Fetch(o => o.CTe).ThenFetch(o => o.ModeloDocumentoFiscal)
                        .ToList();
        }

        public List<Dominio.Entidades.Embarcador.Cargas.CargaCTe> ConsultarCTesParaEmissaoMDFeAquaviarioUltimaPerna(bool somenteCTesAquaviarios, int codigoPedidoViagemNavio, int cnpjPortoDestinos, int cnpjPortoOrigem, bool somenteAutorizados = true)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.CargaCTe>();

            var queryTransbordo = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Pedidos.PedidoTransbordo>();

            if (codigoPedidoViagemNavio > 0)
                queryTransbordo = queryTransbordo.Where(obj => obj.PedidoViagemNavio.Codigo == codigoPedidoViagemNavio);

            if (cnpjPortoOrigem > 0)
                queryTransbordo = queryTransbordo.Where(obj => obj.Porto.Codigo == cnpjPortoOrigem);

            var queryCargaPedido = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.CargaPedido>();
            queryCargaPedido = queryCargaPedido.Where(obj => queryTransbordo.Any(o => o.Pedido == obj.Pedido));

            query = query.Where(obj => obj.CTe != null && obj.CTe.Remetente != null && obj.CTe.TipoCTE == Dominio.Enumeradores.TipoCTE.Normal);
            if (!somenteAutorizados)
                query = query.Where(o => o.CTe != null && (o.CTe.Status == "P" || o.CTe.Status == "E" || o.CTe.Status == "S") && o.Carga != null && o.Carga.SituacaoCarga != Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoCarga.Anulada && o.Carga.SituacaoCarga != Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoCarga.Cancelada);
            else
                query = query.Where(o => o.CTe != null && (o.CTe.Status == "A"));
            query = query.Where(obj => queryCargaPedido.Any(o => o.Carga == obj.Carga));

            if (cnpjPortoDestinos > 0)
                query = query.Where(obj => obj.CTe.PortoDestino.Codigo == cnpjPortoDestinos);

            if (somenteCTesAquaviarios)
                query = query.Where(obj => obj.CTe.TipoModal == Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoModal.Aquaviario);

            return query.Timeout(7000).Fetch(o => o.CTe).ThenFetch(o => o.Remetente)
                        .Fetch(o => o.CTe).ThenFetch(o => o.Destinatario)
                        .Fetch(o => o.CTe).ThenFetch(o => o.Serie)
                        .Fetch(o => o.CTe).ThenFetch(o => o.LocalidadeTerminoPrestacao)
                        .Fetch(o => o.CTe).ThenFetch(o => o.ModeloDocumentoFiscal)
                        .ToList();
        }

        public List<Dominio.Entidades.Embarcador.Cargas.Carga> ConsultarCargasPendenteParaEmissaoMDFeAquaviarioUltimaPerna(bool somenteCTesAquaviarios, int codigoPedidoViagemNavio, int cnpjPortoDestinos, int cnpjPortoOrigem)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.Carga>();

            var queryTransbordo = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Pedidos.PedidoTransbordo>();
            queryTransbordo = queryTransbordo.Where(obj => obj.Pedido.TipoOperacao != null && obj.Pedido.TipoOperacao.TipoPropostaMultimodal != TipoPropostaMultimodal.NoShowCabotagem
            && obj.Pedido.TipoOperacao.TipoPropostaMultimodal != TipoPropostaMultimodal.FaturamentoContabilidade
            && obj.Pedido.TipoOperacao.TipoPropostaMultimodal != TipoPropostaMultimodal.DemurrageCabotagem
            && obj.Pedido.TipoOperacao.TipoPropostaMultimodal != TipoPropostaMultimodal.DetentionCabotagem
            && obj.Pedido.TipoOperacao.TipoPropostaMultimodal != TipoPropostaMultimodal.TAkePayCabotagem && obj.Pedido.TipoOperacao.TipoPropostaMultimodal != TipoPropostaMultimodal.TakePayFeeder);

            if (codigoPedidoViagemNavio > 0)
                queryTransbordo = queryTransbordo.Where(obj => obj.PedidoViagemNavio.Codigo == codigoPedidoViagemNavio);

            if (cnpjPortoOrigem > 0)
                queryTransbordo = queryTransbordo.Where(obj => obj.Porto.Codigo == cnpjPortoOrigem);

            var queryCargaPedido = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.CargaPedido>();
            queryCargaPedido = queryCargaPedido.Where(obj => queryTransbordo.Any(o => o.Pedido == obj.Pedido));

            query = query.Where(obj => obj.SituacaoCarga == Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoCarga.NaLogistica
          || obj.SituacaoCarga == Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoCarga.Nova
          || obj.SituacaoCarga == Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoCarga.AgNFe
          || obj.SituacaoCarga == Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoCarga.PendeciaDocumentos
          || obj.SituacaoCarga == Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoCarga.AgIntegracao
          || obj.SituacaoCarga == Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoCarga.CalculoFrete);
            query = query.Where(obj => queryCargaPedido.Any(o => o.Carga == obj));

            if (cnpjPortoDestinos > 0)
                query = query.Where(obj => obj.PortoDestino.Codigo == cnpjPortoDestinos);


            return query.Timeout(7000).ToList();
        }

        public List<Dominio.Entidades.Embarcador.Pedidos.Pedido> ConsultarPedidosPendenteParaEmissaoMDFeAquaviarioUltimaPerna(bool somenteCTesAquaviarios, int codigoPedidoViagemNavio, int cnpjPortoDestinos, int cnpjPortoOrigem)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Pedidos.Pedido>();
            //query = query.Where(obj => obj.CodigoCargaEmbarcador == null || obj.CodigoCargaEmbarcador == "");
            var queryCargaPedido = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.CargaPedido>();
            queryCargaPedido = queryCargaPedido.Where(obj => obj.Carga.SituacaoCarga != SituacaoCarga.Anulada && obj.Carga.SituacaoCarga != SituacaoCarga.Cancelada);

            var queryCargaPedidoAtivo = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.CargaPedido>();
            queryCargaPedidoAtivo = queryCargaPedidoAtivo.Where(obj => obj.Carga.SituacaoCarga == Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoCarga.NaLogistica
                || obj.Carga.SituacaoCarga == Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoCarga.Nova
                || obj.Carga.SituacaoCarga == Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoCarga.AgNFe
                || obj.Carga.SituacaoCarga == Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoCarga.PendeciaDocumentos
                || obj.Carga.SituacaoCarga == Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoCarga.AgIntegracao
                || obj.Carga.SituacaoCarga == Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoCarga.CalculoFrete);

            query = query.Where(obj => !queryCargaPedido.Any(c => c.Pedido == obj) || queryCargaPedidoAtivo.Any(a => a.Pedido == obj));

            query = query.Where(obj => obj.TipoOperacao != null && obj.TipoOperacao.TipoPropostaMultimodal != TipoPropostaMultimodal.NoShowCabotagem
            && obj.TipoOperacao.TipoPropostaMultimodal != TipoPropostaMultimodal.FaturamentoContabilidade
            && obj.TipoOperacao.TipoPropostaMultimodal != TipoPropostaMultimodal.DemurrageCabotagem
            && obj.TipoOperacao.TipoPropostaMultimodal != TipoPropostaMultimodal.DetentionCabotagem
            && obj.TipoOperacao.TipoPropostaMultimodal != TipoPropostaMultimodal.TAkePayCabotagem && obj.TipoOperacao.TipoPropostaMultimodal != TipoPropostaMultimodal.TakePayFeeder);

            var queryTransbordo = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Pedidos.PedidoTransbordo>();

            if (codigoPedidoViagemNavio > 0)
                queryTransbordo = queryTransbordo.Where(obj => obj.PedidoViagemNavio.Codigo == codigoPedidoViagemNavio);

            if (cnpjPortoOrigem > 0)
                queryTransbordo = queryTransbordo.Where(obj => obj.Porto.Codigo == cnpjPortoOrigem);

            //var queryCargaPedido = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.CargaPedido>();
            queryCargaPedido = queryCargaPedido.Where(obj => queryTransbordo.Any(o => o.Pedido == obj.Pedido));

            query = query.Where(obj => obj.SituacaoPedido != Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoPedido.Cancelado
             && obj.SituacaoPedido != Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoPedido.Finalizado);


            query = query.Where(obj => queryCargaPedido.Any(o => o.Pedido == obj));

            if (cnpjPortoDestinos > 0)
                query = query.Where(obj => obj.PortoDestino.Codigo == cnpjPortoDestinos);


            return query.Timeout(7000).ToList();
        }

        public List<Dominio.Entidades.Embarcador.Pedidos.Pedido> ConsultarPedidosPendentesTransbordoParaEmissaoMDFeAquaviario(int sequencia, bool somenteCTesAquaviarios, int codigoPedidoViagemNavio, List<int> terminaisOrigem, int cnpjPortoOrigem, List<int> terminaisDestino, int cnpjPortoDestino)
        {
            var queryTransbordo = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Pedidos.PedidoTransbordo>();
            queryTransbordo = queryTransbordo.Where(obj => obj.Sequencia == sequencia);
            queryTransbordo = queryTransbordo.Where(obj => obj.Pedido.TipoOperacao != null && obj.Pedido.TipoOperacao.TipoPropostaMultimodal != TipoPropostaMultimodal.NoShowCabotagem
                && obj.Pedido.TipoOperacao.TipoPropostaMultimodal != TipoPropostaMultimodal.FaturamentoContabilidade
                && obj.Pedido.TipoOperacao.TipoPropostaMultimodal != TipoPropostaMultimodal.DemurrageCabotagem
                && obj.Pedido.TipoOperacao.TipoPropostaMultimodal != TipoPropostaMultimodal.DetentionCabotagem
                && obj.Pedido.TipoOperacao.TipoPropostaMultimodal != TipoPropostaMultimodal.TAkePayCabotagem && obj.Pedido.TipoOperacao.TipoPropostaMultimodal != TipoPropostaMultimodal.TakePayFeeder);

            if (sequencia >= 1)
            {
                if (cnpjPortoDestino > 0)
                    queryTransbordo = queryTransbordo.Where(obj => obj.Porto.Codigo == cnpjPortoDestino);
                if (terminaisDestino != null && terminaisDestino.Count > 0)
                    queryTransbordo = queryTransbordo.Where(obj => terminaisDestino.Contains(obj.Terminal.Codigo));
                if (sequencia > 1)
                {
                    if (codigoPedidoViagemNavio > 0)
                        queryTransbordo = queryTransbordo.Where(obj => obj.PedidoViagemNavio.Codigo == codigoPedidoViagemNavio);
                }
            }
            if (sequencia > 100)
            {
                if (codigoPedidoViagemNavio > 0)
                    queryTransbordo = queryTransbordo.Where(obj => obj.PedidoViagemNavio.Codigo == codigoPedidoViagemNavio);
                if (cnpjPortoOrigem > 0)
                    queryTransbordo = queryTransbordo.Where(obj => obj.Porto.Codigo == cnpjPortoOrigem);
                if (terminaisOrigem != null && terminaisOrigem.Count > 0)
                    queryTransbordo = queryTransbordo.Where(obj => terminaisOrigem.Contains(obj.Terminal.Codigo));
            }

            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Pedidos.Pedido>();
            //query = query.Where(obj => obj.CodigoCargaEmbarcador == null || obj.CodigoCargaEmbarcador == "");
            var queryCargaPedido = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.CargaPedido>();
            queryCargaPedido = queryCargaPedido.Where(obj => obj.Carga.SituacaoCarga != SituacaoCarga.Anulada && obj.Carga.SituacaoCarga != SituacaoCarga.Cancelada);

            var queryCargaPedidoAtivo = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.CargaPedido>();
            queryCargaPedidoAtivo = queryCargaPedidoAtivo.Where(obj => obj.Carga.SituacaoCarga == Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoCarga.NaLogistica
                || obj.Carga.SituacaoCarga == Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoCarga.Nova
                || obj.Carga.SituacaoCarga == Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoCarga.AgNFe
                || obj.Carga.SituacaoCarga == Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoCarga.PendeciaDocumentos
                || obj.Carga.SituacaoCarga == Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoCarga.AgIntegracao
                || obj.Carga.SituacaoCarga == Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoCarga.CalculoFrete);

            query = query.Where(obj => !queryCargaPedido.Any(c => c.Pedido == obj) || queryCargaPedidoAtivo.Any(a => a.Pedido == obj));

            //query = query.Where(obj => !queryCargaPedido.Any(c => c.Pedido == obj));

            query = query.Where(obj => obj.SituacaoPedido != Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoPedido.Cancelado
            && obj.SituacaoPedido != Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoPedido.Finalizado);

            query = query.Where(obj => queryTransbordo.Any(o => o.Pedido == obj));

            if (sequencia == 1 && codigoPedidoViagemNavio > 0)
                query = query.Where(obj => obj.PedidoViagemNavio.Codigo == codigoPedidoViagemNavio);

            if (sequencia == 1 && cnpjPortoOrigem > 0)
                query = query.Where(obj => obj.Porto.Codigo == cnpjPortoOrigem);

            if (sequencia == 5 && cnpjPortoDestino > 0)
                query = query.Where(obj => obj.PortoDestino.Codigo == cnpjPortoDestino);

            return query.Timeout(7000).ToList();
        }

        public List<Dominio.Entidades.Embarcador.Cargas.Carga> ConsultarCargasPendentesTransbordoParaEmissaoMDFeAquaviario(int sequencia, bool somenteCTesAquaviarios, int codigoPedidoViagemNavio, List<int> terminaisOrigem, int cnpjPortoOrigem, List<int> terminaisDestino, int cnpjPortoDestino)
        {
            var queryTransbordo = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Pedidos.PedidoTransbordo>();
            queryTransbordo = queryTransbordo.Where(obj => obj.Sequencia == sequencia);
            queryTransbordo = queryTransbordo.Where(obj => obj.Pedido.TipoOperacao != null && obj.Pedido.TipoOperacao.TipoPropostaMultimodal != TipoPropostaMultimodal.NoShowCabotagem
             && obj.Pedido.TipoOperacao.TipoPropostaMultimodal != TipoPropostaMultimodal.FaturamentoContabilidade
             && obj.Pedido.TipoOperacao.TipoPropostaMultimodal != TipoPropostaMultimodal.DemurrageCabotagem
             && obj.Pedido.TipoOperacao.TipoPropostaMultimodal != TipoPropostaMultimodal.DetentionCabotagem
             && obj.Pedido.TipoOperacao.TipoPropostaMultimodal != TipoPropostaMultimodal.TAkePayCabotagem && obj.Pedido.TipoOperacao.TipoPropostaMultimodal != TipoPropostaMultimodal.TakePayFeeder);

            if (sequencia >= 1)
            {
                if (cnpjPortoDestino > 0)
                    queryTransbordo = queryTransbordo.Where(obj => obj.Porto.Codigo == cnpjPortoDestino);
                if (terminaisDestino != null && terminaisDestino.Count > 0)
                    queryTransbordo = queryTransbordo.Where(obj => terminaisDestino.Contains(obj.Terminal.Codigo));
                if (sequencia > 1)
                {
                    if (codigoPedidoViagemNavio > 0)
                        queryTransbordo = queryTransbordo.Where(obj => obj.PedidoViagemNavio.Codigo == codigoPedidoViagemNavio);
                }
            }
            if (sequencia > 100)
            {
                if (codigoPedidoViagemNavio > 0)
                    queryTransbordo = queryTransbordo.Where(obj => obj.PedidoViagemNavio.Codigo == codigoPedidoViagemNavio);
                if (cnpjPortoOrigem > 0)
                    queryTransbordo = queryTransbordo.Where(obj => obj.Porto.Codigo == cnpjPortoOrigem);
                if (terminaisOrigem != null && terminaisOrigem.Count > 0)
                    queryTransbordo = queryTransbordo.Where(obj => terminaisOrigem.Contains(obj.Terminal.Codigo));
            }

            var queryCargaPedido = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.CargaPedido>();
            queryCargaPedido = queryCargaPedido.Where(obj => queryTransbordo.Any(o => o.Pedido == obj.Pedido));

            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.Carga>();

            query = query.Where(obj => obj.SituacaoCarga == Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoCarga.NaLogistica
           || obj.SituacaoCarga == Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoCarga.Nova
           || obj.SituacaoCarga == Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoCarga.AgNFe
           || obj.SituacaoCarga == Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoCarga.PendeciaDocumentos
           || obj.SituacaoCarga == Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoCarga.AgIntegracao
           || obj.SituacaoCarga == Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoCarga.CalculoFrete);

            query = query.Where(obj => queryCargaPedido.Any(o => o.Carga == obj));

            if (sequencia == 1 && codigoPedidoViagemNavio > 0)
                query = query.Where(obj => obj.PedidoViagemNavio.Codigo == codigoPedidoViagemNavio);

            if (sequencia == 1 && cnpjPortoOrigem > 0)
                query = query.Where(obj => obj.PortoOrigem.Codigo == cnpjPortoOrigem);

            if (sequencia == 5 && cnpjPortoDestino > 0)
                query = query.Where(obj => obj.PortoDestino.Codigo == cnpjPortoDestino);

            return query.Timeout(7000).ToList();
        }

        public List<Dominio.Entidades.Embarcador.Cargas.CargaCTe> ConsultarCTesTransbordoParaEmissaoMDFeAquaviario(int sequencia, bool somenteCTesAquaviarios, int codigoPedidoViagemNavio, List<int> terminaisOrigem, int cnpjPortoOrigem, List<int> terminaisDestino, int cnpjPortoDestino, bool somenteAutorizados = true)
        {
            var queryTransbordo = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Pedidos.PedidoTransbordo>();
            queryTransbordo = queryTransbordo.Where(obj => obj.Sequencia == sequencia);

            if (sequencia == 1)
            {
                if (cnpjPortoDestino > 0)
                    queryTransbordo = queryTransbordo.Where(obj => obj.Porto.Codigo == cnpjPortoDestino);
                if (terminaisDestino != null && terminaisDestino.Count > 0)
                    queryTransbordo = queryTransbordo.Where(obj => terminaisDestino.Contains(obj.Terminal.Codigo));
                //if (sequencia > 1)
                //{
                //    if (codigoPedidoViagemNavio > 0)
                //        queryTransbordo = queryTransbordo.Where(obj => obj.PedidoViagemNavio.Codigo == codigoPedidoViagemNavio);
                //}
            }
            if (sequencia > 100)
            {
                if (codigoPedidoViagemNavio > 0)
                    queryTransbordo = queryTransbordo.Where(obj => obj.PedidoViagemNavio.Codigo == codigoPedidoViagemNavio);
                if (cnpjPortoOrigem > 0)
                    queryTransbordo = queryTransbordo.Where(obj => obj.Porto.Codigo == cnpjPortoOrigem);
                if (terminaisOrigem != null && terminaisOrigem.Count > 0)
                    queryTransbordo = queryTransbordo.Where(obj => terminaisOrigem.Contains(obj.Terminal.Codigo));
            }

            var queryCargaPedido = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.CargaPedido>();
            queryCargaPedido = queryCargaPedido.Where(obj => queryTransbordo.Any(o => o.Pedido == obj.Pedido));

            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.CargaCTe>();
            query = query.Where(obj => obj.CTe != null && obj.CTe.Remetente != null && obj.CTe.TipoCTE == Dominio.Enumeradores.TipoCTE.Normal);
            if (!somenteAutorizados)
                query = query.Where(o => o.CTe != null && (o.CTe.Status == "P" || o.CTe.Status == "E" || o.CTe.Status == "S") && o.Carga != null && o.Carga.SituacaoCarga != Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoCarga.Anulada && o.Carga.SituacaoCarga != Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoCarga.Cancelada);
            else
                query = query.Where(o => o.CTe != null && (o.CTe.Status == "A"));
            query = query.Where(obj => queryCargaPedido.Any(o => o.Carga == obj.Carga));

            if (sequencia == 1 && codigoPedidoViagemNavio > 0)
                query = query.Where(obj => obj.CTe.Viagem.Codigo == codigoPedidoViagemNavio);
            if (sequencia == 1 && cnpjPortoOrigem > 0)
                query = query.Where(obj => obj.CTe.PortoOrigem.Codigo == cnpjPortoOrigem);

            if (sequencia == 2 && codigoPedidoViagemNavio > 0 && cnpjPortoOrigem > 0 && cnpjPortoDestino > 0)
            {
                query = query.Where(obj => obj.CTe.ViagemPassagemUm.Codigo == codigoPedidoViagemNavio);
                query = query.Where(obj => obj.CTe.PortoPassagemUm.Codigo == cnpjPortoOrigem);
                query = query.Where(obj => obj.CTe.PortoPassagemDois.Codigo == cnpjPortoDestino);
            }
            else if (sequencia == 3 && codigoPedidoViagemNavio > 0 && cnpjPortoOrigem > 0 && cnpjPortoDestino > 0)
            {
                query = query.Where(obj => obj.CTe.ViagemPassagemDois.Codigo == codigoPedidoViagemNavio);
                query = query.Where(obj => obj.CTe.PortoPassagemDois.Codigo == cnpjPortoOrigem);
                query = query.Where(obj => obj.CTe.PortoPassagemTres.Codigo == cnpjPortoDestino);
            }
            else if (sequencia == 4 && codigoPedidoViagemNavio > 0 && cnpjPortoOrigem > 0 && cnpjPortoDestino > 0)
            {
                query = query.Where(obj => obj.CTe.ViagemPassagemTres.Codigo == codigoPedidoViagemNavio);
                query = query.Where(obj => obj.CTe.PortoPassagemTres.Codigo == cnpjPortoOrigem);
                query = query.Where(obj => obj.CTe.PortoPassagemQuatro.Codigo == cnpjPortoDestino);
            }
            else if (sequencia == 5 && cnpjPortoDestino > 0)
                query = query.Where(obj => obj.CTe.PortoDestino.Codigo == cnpjPortoDestino);

            if (somenteCTesAquaviarios)
                query = query.Where(obj => obj.CTe.TipoModal == Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoModal.Aquaviario);

            return query.Timeout(7000).Fetch(o => o.CTe).ThenFetch(o => o.Remetente)
                        .Fetch(o => o.CTe).ThenFetch(o => o.Destinatario)
                        .Fetch(o => o.CTe).ThenFetch(o => o.Serie)
                        .Fetch(o => o.CTe).ThenFetch(o => o.LocalidadeTerminoPrestacao)
                        .Fetch(o => o.CTe).ThenFetch(o => o.ModeloDocumentoFiscal)
                        .ToList();
        }

        public Task<List<Dominio.Entidades.Embarcador.Cargas.CargaCTe>> ConsultarCTesParaEmissaoMDFeAsync(Dominio.ObjetosDeValor.Embarcador.MDFe.FiltroPesquisaCTeCargaMDFeManual filtrosPesquisa, Dominio.ObjetosDeValor.Embarcador.Consulta.ParametroConsulta parametroConsulta)
        {
            var result = QueryCTesParaEmissaoMDFe(filtrosPesquisa, parametroConsulta);

            result.Fetch(o => o.CTe).ThenFetch(o => o.Remetente)
                        .Fetch(o => o.CTe).ThenFetch(o => o.Destinatario)
                        .Fetch(o => o.CTe).ThenFetch(o => o.Serie)
                        .Fetch(o => o.CTe).ThenFetch(o => o.LocalidadeTerminoPrestacao)
                        .Fetch(o => o.CTe).ThenFetch(o => o.ModeloDocumentoFiscal);

            return ObterListaAsync(result, parametroConsulta);
        }

        public IQueryable<Dominio.Entidades.Embarcador.Cargas.CargaCTe> QueryCTesParaEmissaoMDFe(Dominio.ObjetosDeValor.Embarcador.MDFe.FiltroPesquisaCTeCargaMDFeManual filtrosPesquisa, Dominio.ObjetosDeValor.Embarcador.Consulta.ParametroConsulta parametroConsulta)
        {
            List<Dominio.Enumeradores.TipoCTE> tiposCTes = new List<Dominio.Enumeradores.TipoCTE>() { Dominio.Enumeradores.TipoCTE.Normal, Dominio.Enumeradores.TipoCTE.Substituto };

            IQueryable<Dominio.Entidades.Embarcador.Cargas.CargaCTe> query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.CargaCTe>();

            query = query.Where(obj => obj.CTe != null && obj.CTe.Status == "A" && tiposCTes.Contains(obj.CTe.TipoCTE));

            if (!filtrosPesquisa.TodosOsCTes)
                query = query.Where(o => o.Carga.NaoGerarMDFe || o.Carga.NaoExigeVeiculoParaEmissao);

            if (filtrosPesquisa.Carga > 0)
                query = query.Where(obj => obj.Carga.Codigo == filtrosPesquisa.Carga);

            if (filtrosPesquisa.NumeroNF > 0)
                query = query.Where(obj => obj.NotasFiscais.Any(doc => doc.PedidoXMLNotaFiscal.XMLNotaFiscal.Numero == filtrosPesquisa.NumeroNF));


            if (filtrosPesquisa.Empresa > 0)

                query = query.Where(obj => obj.CTe.Empresa.Codigo == filtrosPesquisa.Empresa || obj.CTe.Empresa.CNPJ.Contains(filtrosPesquisa.Raiz));

            if (filtrosPesquisa.CTe > 0)
                query = query.Where(obj => obj.CTe.Numero == filtrosPesquisa.CTe);

            if (filtrosPesquisa.SomenteCTesAquaviarios)
                query = query.Where(obj => obj.CTe.TipoModal == Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoModal.Aquaviario);

            if (filtrosPesquisa.RotasFrete?.Count > 0)
            {
                query = query.Where(ccte =>
                    ccte.NotasFiscais.Any(cpxml =>
                        cpxml.PedidoXMLNotaFiscal != null &&
                        cpxml.PedidoXMLNotaFiscal.CargaPedido != null &&
                        cpxml.PedidoXMLNotaFiscal.CargaPedido.Pedido != null &&
                        cpxml.PedidoXMLNotaFiscal.CargaPedido.Pedido.RotaFrete != null &&
                        filtrosPesquisa.RotasFrete.Contains(
                            cpxml.PedidoXMLNotaFiscal.CargaPedido.Pedido.RotaFrete.Codigo
                        )
                    )
                );
            }

            return query;
        }

        public Task<int> ContarConsultaCTesParaEmissaoMDFeAsync(Dominio.ObjetosDeValor.Embarcador.MDFe.FiltroPesquisaCTeCargaMDFeManual filtrosPesquisa, Dominio.ObjetosDeValor.Embarcador.Consulta.ParametroConsulta parametroConsulta)
        {
            IQueryable<Dominio.Entidades.Embarcador.Cargas.CargaCTe> result = QueryCTesParaEmissaoMDFe(filtrosPesquisa, parametroConsulta);

            return result.CountAsync(CancellationToken);
        }

        public Task<List<Dominio.Entidades.Embarcador.Cargas.CargaCTe>> ConsultarCTesParaEmissaoMDFeMultiCTeAsync(Dominio.ObjetosDeValor.Embarcador.MDFe.FiltroPesquisaCTeCargaMDFeManualMultiCTe filtrosPesquisa, Dominio.ObjetosDeValor.Embarcador.Consulta.ParametroConsulta parametroConsulta)
        {
            var result = QueryCTesParaEmissaoMDFeMultiCTe(filtrosPesquisa);

            result = result.Fetch(o => o.CTe).ThenFetch(o => o.Remetente)
                         .Fetch(o => o.CTe).ThenFetch(o => o.Destinatario)
                         .Fetch(o => o.CTe).ThenFetch(o => o.Serie)
                         .Fetch(o => o.CTe).ThenFetch(o => o.LocalidadeTerminoPrestacao)
                         .Fetch(o => o.CTe).ThenFetch(o => o.ModeloDocumentoFiscal);

            return ObterListaAsync(result, parametroConsulta);
        }

        public Task<int> ContarConsultaCTesParaEmissaoMDFeMultiCTeAsync(Dominio.ObjetosDeValor.Embarcador.MDFe.FiltroPesquisaCTeCargaMDFeManualMultiCTe filtrosPesquisa)
        {
            var result = QueryCTesParaEmissaoMDFeMultiCTe(filtrosPesquisa);

            return result.CountAsync(CancellationToken);
        }

        public IQueryable<Dominio.Entidades.Embarcador.Cargas.CargaCTe> QueryCTesParaEmissaoMDFeMultiCTe(Dominio.ObjetosDeValor.Embarcador.MDFe.FiltroPesquisaCTeCargaMDFeManualMultiCTe filtrosPesquisa)
        {
            IQueryable<Dominio.Entidades.Embarcador.Cargas.CargaCTe> query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.CargaCTe>();

            query = query.Where(obj => obj.CTe != null && obj.CTe.Remetente != null && obj.CTe.Status == "A" && obj.CTe.TipoCTE == Dominio.Enumeradores.TipoCTE.Normal);

            if (filtrosPesquisa.Carga > 0)
                query = query.Where(obj => obj.Carga.Codigo == filtrosPesquisa.Carga);

            if (filtrosPesquisa.NumeroNF > 0)
                query = query.Where(obj => obj.NotasFiscais.Any(doc => doc.PedidoXMLNotaFiscal.XMLNotaFiscal.Numero == filtrosPesquisa.NumeroNF));

            if (filtrosPesquisa.Empresa > 0)
                query = query.Where(obj => obj.CTe.Empresa.Codigo == filtrosPesquisa.Empresa);

            if (filtrosPesquisa.CTe > 0)
                query = query.Where(obj => obj.CTe.Numero == filtrosPesquisa.CTe);

            if (filtrosPesquisa.RotasFrete?.Count > 0)
            {
                query = query.Where(ccte =>
                    ccte.NotasFiscais.Any(cpxml =>
                        cpxml.PedidoXMLNotaFiscal != null &&
                        cpxml.PedidoXMLNotaFiscal.CargaPedido != null &&
                        cpxml.PedidoXMLNotaFiscal.CargaPedido.Pedido != null &&
                        cpxml.PedidoXMLNotaFiscal.CargaPedido.Pedido.RotaFrete != null &&
                        filtrosPesquisa.RotasFrete.Contains(
                            cpxml.PedidoXMLNotaFiscal.CargaPedido.Pedido.RotaFrete.Codigo
                        )
                    )
                );
            }

            return query;
        }

        private async Task<IQueryable<Dominio.Entidades.Embarcador.Cargas.CargaCTe>> MontarConsultaCTes(Dominio.ObjetosDeValor.Embarcador.Carga.FiltroPesquisaMontarConsultaCtes filtro)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.CargaCTe>();

            if (!filtro.RetornarPreCtes)
                query = query.Where(obj => obj.CTe != null);
            else
                query = query.Where(obj => obj.CTe != null || obj.PreCTe != null);

            if (filtro.BuscarPorCargaOrigem)
                query = query.Where(obj => obj.CargaOrigem.Codigo == filtro.Carga);
            else
                query = query.Where(obj => obj.Carga.Codigo == filtro.Carga);

            if (filtro.StatusCTe != null && filtro.StatusCTe.Count() > 0)
            {
                if (!filtro.RetornarPreCtes)
                    query = query.Where(obj => filtro.StatusCTe.Contains(obj.CTe.Status));
                else
                    query = query.Where(obj => filtro.StatusCTe.Contains(obj.CTe.Status) || (obj.CTe == null && obj.PreCTe != null));
            }

            if (filtro.NumeroNF > 0)
                query = query.Where(obj => obj.NotasFiscais.Any(doc => doc.PedidoXMLNotaFiscal.XMLNotaFiscal.Numero == filtro.NumeroNF));

            if (filtro.ApenasCTesNormais)
                query = query.Where(obj => obj.CargaCTeComplementoInfo == null && (!obj.LancamentoManualPossuiNFSOcorrencia.Value || obj.LancamentoManualPossuiNFSOcorrencia == null));

            if (!string.IsNullOrWhiteSpace(filtro.ProprietarioVeiculo))
                query = query.Where(obj => (obj.Carga.Veiculo.Proprietario.CPF_CNPJ == double.Parse(filtro.ProprietarioVeiculo) || obj.Carga.Empresa.CNPJ == filtro.ProprietarioVeiculo || obj.Carga.Veiculo.Empresa.CNPJ == filtro.ProprietarioVeiculo || obj.CTe.Empresa.CNPJ == filtro.ProprietarioVeiculo));

            if (filtro.Destinatario > 0)
            {
                if (!filtro.RetornarPreCtes)
                    query = query.Where(o => o.CTe.Destinatario.Cliente.CPF_CNPJ == filtro.Destinatario);
                else
                    query = query.Where(o => o.CTe.Destinatario.Cliente.CPF_CNPJ == filtro.Destinatario || (o.PreCTe.Destinatario.Cliente.CPF_CNPJ == filtro.Destinatario && o.CTe == null));
            }

            if (filtro.NumeroDocumento > 0 && !filtro.RetornarPreCtes)
                query = query.Where(o => o.CTe.Numero == filtro.NumeroDocumento);

            if (filtro.EmitirDocumentoParaFilialEmissoraComPreCTe)
                query = query.Where(o => o.CargaCTeFilialEmissora == null && o.CargaCTeSubContratacaoFilialEmissora != null && o.CargaCTeSubContratacaoFilialEmissora.CargaCTeFilialEmissora != null);

            if (filtro.CtesSubContratacaoFilialEmissora)
                query = query.Where(o => o.CargaCTeFilialEmissora != null || (o.CargaCTeFilialEmissora == null && o.CargaCTeSubContratacaoFilialEmissora == null));

            if (filtro.CtesSemSubContratacaoFilialEmissora)
            {
                if (!filtro.RetornarPreCtes)
                    query = query.Where(o => o.CargaCTeFilialEmissora == null && (o.Carga.EmpresaFilialEmissora != null && filtro.EmpresasFilialEmissora.Contains(o.CTe.Empresa.Codigo)));
                else
                    query = query.Where(o => o.CargaCTeFilialEmissora == null && (o.Carga.EmpresaFilialEmissora != null && (filtro.EmpresasFilialEmissora.Contains(o.CTe.Empresa.Codigo) || (filtro.EmpresasFilialEmissora.Contains(o.PreCTe.Empresa.Codigo) && o.CTe == null))));
            }

            if (filtro.CTesFactura)
                query = query.Where(obj => obj.NotasFiscais.Any(doc => doc.PedidoXMLNotaFiscal.XMLNotaFiscal.TipoFatura == true));
            else if (filtro.CargaMercosul)
                query = query.Where(obj => obj.NotasFiscais.Any(doc => ((bool?)doc.PedidoXMLNotaFiscal.XMLNotaFiscal.TipoFatura ?? false) == false));

            if (filtro.TiposDocumentosDoCte != null)
            {
                query = query.Where(o => filtro.TiposDocumentosDoCte.Contains(o.CTe.ModeloDocumentoFiscal.TipoDocumentoEmissao));
            }

            if (filtro.CodigoChamado > 0 && filtro.PermitirSelecionarCteApenasComNfeVinculadaOcorrencia)
            {
                IQueryable<int> codigosXmlNotaFiscal = SessionNHiBernate
                    .Query<Dominio.Entidades.Embarcador.Chamados.Chamado>()
                    .Where(chamado => chamado.Codigo == filtro.CodigoChamado)
                    .SelectMany(chamado => chamado.XMLNotasFiscais.Select(xmlNotaFiscal => xmlNotaFiscal.Codigo));

                query = query.Where(cargaCte => cargaCte.NotasFiscais.Any(cargaPedidoXmlNotaFiscalCte => codigosXmlNotaFiscal.Contains(cargaPedidoXmlNotaFiscalCte.PedidoXMLNotaFiscal.XMLNotaFiscal.Codigo)));
            }

            if (!filtro.RetornarPreCtes)
            {
                if (filtro.RetornarDocumentoOperacaoContainer)
                    query = query.Where(obj => obj.CTe.DocumentoOperacaoContainer.Value);
                else
                    query = query.Where(obj => !obj.CTe.DocumentoOperacaoContainer.Value || obj.CTe.DocumentoOperacaoContainer == null);

                if (!string.IsNullOrEmpty(filtro.NumeroContainer))
                    query = query.Where(obj => obj.CTe.Container == filtro.NumeroContainer);
            }


            return query;
        }

        public async Task<List<Dominio.Entidades.Embarcador.Cargas.CargaCTe>> ObterRequisicoesConsultarCTes(Dominio.ObjetosDeValor.Embarcador.Carga.FiltroPesquisaMontarConsultaCtes filtro, bool selecionarTodos, List<int> codigosRequisicoes)
        {
            IQueryable<Dominio.Entidades.Embarcador.Cargas.CargaCTe> result = await MontarConsultaCTes(filtro);

            if (selecionarTodos)
                result = result.Where(o => !codigosRequisicoes.Contains(o.Codigo));
            else
                result = result.Where(o => codigosRequisicoes.Contains(o.Codigo));

            return result.ToList();
        }


        public async Task<List<Dominio.Entidades.Embarcador.Cargas.CargaCTe>> ConsultarCTes(Dominio.ObjetosDeValor.Embarcador.Carga.FiltroPesquisaMontarConsultaCtes filtro, string propOrdenacao, string dirOrdenacao, int inicioRegistros, int maximoRegistros)
        {
            IQueryable<Dominio.Entidades.Embarcador.Cargas.CargaCTe> query = await MontarConsultaCTes(filtro);

            if (filtro.RetornarPreCtes)
            {
                query = query.Fetch(o => o.PreCTe).ThenFetch(o => o.Remetente).ThenFetch(o => o.Cliente)
                         .Fetch(o => o.PreCTe).ThenFetch(o => o.Destinatario).ThenFetch(o => o.Cliente)
                         .Fetch(o => o.PreCTe).ThenFetch(o => o.OutrosTomador).ThenFetch(o => o.Cliente)
                         .Fetch(o => o.PreCTe).ThenFetch(o => o.Expedidor).ThenFetch(o => o.Cliente)
                         .Fetch(o => o.PreCTe).ThenFetch(o => o.Recebedor).ThenFetch(o => o.Cliente)
                         .Fetch(o => o.PreCTe).ThenFetch(o => o.LocalidadeTerminoPrestacao);
            }

            return query.Fetch(o => o.CTe).ThenFetch(o => o.Remetente).ThenFetch(o => o.Cliente)
                         .Fetch(o => o.CTe).ThenFetch(o => o.Destinatario).ThenFetch(o => o.Cliente)
                         .Fetch(o => o.CTe).ThenFetch(o => o.OutrosTomador).ThenFetch(o => o.Cliente)
                         .Fetch(o => o.CTe).ThenFetch(o => o.Expedidor).ThenFetch(o => o.Cliente)
                         .Fetch(o => o.CTe).ThenFetch(o => o.Recebedor).ThenFetch(o => o.Cliente)
                         .Fetch(o => o.CTe).ThenFetch(o => o.MensagemStatus)
                         .Fetch(o => o.CTe).ThenFetch(o => o.Serie)
                         .Fetch(o => o.CTe).ThenFetch(o => o.LocalidadeTerminoPrestacao)
                         .Fetch(o => o.CTe).ThenFetch(o => o.ModeloDocumentoFiscal)
                         .OrderBy(propOrdenacao + (dirOrdenacao == "asc" ? " ascending" : " descending"))
                         .Skip(inicioRegistros)
                         .Take(maximoRegistros)
                         .ToList();
        }

        public async Task<int> ContarConsultaCTes(Dominio.ObjetosDeValor.Embarcador.Carga.FiltroPesquisaMontarConsultaCtes filtro)
        {
            IQueryable<Dominio.Entidades.Embarcador.Cargas.CargaCTe> query = await MontarConsultaCTes(filtro);

            return query.Count();
        }

        public List<Dominio.Entidades.Embarcador.Cargas.CargaCTe> BuscarConhecimentoPorCarga(int codigoCarga)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.CargaCTe>();
            var result = from obj in query where obj.Carga.Codigo == codigoCarga && obj.CargaCTeComplementoInfo == null select obj;
            return result.ToList();
        }

        public List<Dominio.Entidades.Embarcador.Cargas.CargaCTe> BuscarConhecimentoPorCarga(int codigoCarga, string propOrdenacao, string dirOrdenacao, int inicioRegistros, int maximoRegistros)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.CargaCTe>();
            var result = from obj in query where obj.Carga.Codigo == codigoCarga && obj.CargaCTeComplementoInfo == null select obj;
            return result.OrderBy(propOrdenacao + (dirOrdenacao == "asc" ? " ascending" : " descending")).Skip(inicioRegistros).Take(maximoRegistros).Distinct().ToList();
        }

        public int ContarConhecimentoPorCarga(int codigoCarga)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.CargaCTe>();
            var result = from obj in query where obj.Carga.Codigo == codigoCarga && obj.CargaCTeComplementoInfo == null select obj;
            return result.Count();
        }

        public int ContarCTePorSituacao(int codigoCarga, string situacao)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.CargaCTe>();

            var result = from obj in query where obj.Carga.Codigo == codigoCarga && obj.CTe != null && obj.CTe.Status == situacao select obj.Codigo;

            return result.Count();
        }

        public int ContarCTeEnviadoSemCodigoOracle(int codigoCarga)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.CargaCTe>();

            var result = from obj in query where obj.Carga.Codigo == codigoCarga && obj.CTe != null && obj.CTe.Status == "E" && obj.CTe.CodigoCTeIntegrador == 0 && !obj.Carga.EmitindoCTes && (obj.CTe.SistemaEmissor == null || obj.CTe.SistemaEmissor == TipoEmissorDocumento.Integrador) select obj.Codigo;

            return result.Count();
        }

        public List<Dominio.Entidades.ConhecimentoDeTransporteEletronico> ObterCTeEnviadoSemCodigoOracle(int codigoCarga)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.CargaCTe>();

            var result = from obj in query
                         where obj.Carga.Codigo == codigoCarga
                            && obj.CTe != null
                            && obj.CTe.Status == "E"
                            && obj.CTe.CodigoCTeIntegrador == 0
                            && !obj.Carga.EmitindoCTes
                            && obj.CTe.SistemaEmissor != TipoEmissorDocumento.Migrate
                            && obj.CTe.SistemaEmissor != TipoEmissorDocumento.NSTech
                         select obj.CTe;

            return result.ToList();
        }

        public int ContarCTePorListaSituacao(int codigoCarga, bool cteSubContratacaoFilialEmissora, string[] situacao)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.CargaCTe>();

            var result = from obj in query where obj.Carga.Codigo == codigoCarga && obj.CTe != null && situacao.Contains(obj.CTe.Status) && !obj.Carga.EmitindoCTes select obj;

            if (cteSubContratacaoFilialEmissora)
                result = result.Where(obj => obj.CargaCTeFilialEmissora != null || (obj.CargaCTeFilialEmissora == null && obj.CargaCTeSubContratacaoFilialEmissora == null));
            else
                result = result.Where(obj => obj.CargaCTeSubContratacaoFilialEmissora == null);

            return result.Count();
        }

        public int ContarCTePorListaSituacaoDiff(int codigoCarga, string[] situacao)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.CargaCTe>();

            var result = from obj in query where obj.Carga.Codigo == codigoCarga && obj.CTe != null && !situacao.Contains(obj.CTe.Status) && obj.CTe.ModeloDocumentoFiscal.TipoDocumentoEmissao == Dominio.Enumeradores.TipoDocumento.CTe select obj.Codigo;

            return result.Count();
        }

        public int ContarCTePorSituacaoDiff(int codigoCarga, bool ctesSubContratacaoFilialEmissora, string[] situacao)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.CargaCTe>();

            var result = from obj in query where obj.Carga.Codigo == codigoCarga && obj.CTe != null && !situacao.Contains(obj.CTe.Status) select obj;

            if (ctesSubContratacaoFilialEmissora)
                result = result.Where(obj => obj.CargaCTeFilialEmissora != null || (obj.CargaCTeFilialEmissora == null && obj.CargaCTeSubContratacaoFilialEmissora == null));
            else
                result = result.Where(obj => obj.CargaCTeSubContratacaoFilialEmissora == null);

            return result.Count();
        }

        public List<Dominio.Entidades.Embarcador.Cargas.CargaCTe> BuscarPreCTePorCarga(int codigoCarga)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.CargaCTe>();

            var result = from obj in query where obj.Carga.Codigo == codigoCarga && obj.PreCTe != null select obj;

            return result.ToList();
        }

        public int ContarPreCTePorCarga(int codigoCarga)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.CargaCTe>();

            var result = from obj in query where obj.Carga.Codigo == codigoCarga && obj.PreCTe != null select obj.Codigo;

            return result.Count();
        }

        public int ContarCargaPreCTePorNaoEnviados(int codigoCarga)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.CargaCTe>();

            var result = from obj in query where obj.Carga.Codigo == codigoCarga && obj.PreCTe != null && obj.CTe == null select obj.Codigo;

            return result.Count();
        }

        public List<int> BuscarCodigoModeloDocumentoPorCarga(int codigoCarga, string statusCTe)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.CargaCTe>();

            query = query.Where(o => o.Carga.Codigo == codigoCarga && o.CTe.Status == statusCTe && o.CargaCTeComplementoInfo == null);

            return query.Select(o => o.CTe.ModeloDocumentoFiscal.Codigo).Distinct().ToList();
        }

        public List<int> BuscarCodigoModeloDocumentoPorCarga(int codigoCarga, string[] statusCTe)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.CargaCTe>();

            query = query.Where(o => o.Carga.Codigo == codigoCarga && statusCTe.Contains(o.CTe.Status));

            return query.Select(o => o.CTe.ModeloDocumentoFiscal.Codigo).Distinct().ToList();
        }

        public decimal BuscarValorTotalReceberPorCargaEModeloDocumento(int codigoCarga, int codigoModeloDocumento, string statusCTe, DateTime? dataAutorizacao, DateTime? dataCancelamento, DateTime? dataAnulacao)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.CargaCTe>();

            query = query.Where(o => o.Carga.Codigo == codigoCarga && o.CTe.Status == statusCTe && o.CTe.ModeloDocumentoFiscal.Codigo == codigoModeloDocumento && o.CargaCTeComplementoInfo == null);

            if (dataAutorizacao.HasValue)
                query = query.Where(o => o.CTe.DataAutorizacao >= dataAutorizacao.Value.Date && o.CTe.DataAutorizacao < dataAutorizacao.Value.AddDays(1).Date);

            if (dataCancelamento.HasValue)
                query = query.Where(o => o.CTe.DataCancelamento >= dataCancelamento.Value.Date && o.CTe.DataCancelamento < dataCancelamento.Value.AddDays(1).Date);

            if (dataAnulacao.HasValue)
                query = query.Where(o => o.CTe.DataAnulacao >= dataAnulacao.Value.Date && o.CTe.DataAnulacao < dataAnulacao.Value.AddDays(1).Date);

            return query.Select(o => (decimal?)o.CTe.ValorAReceber).Sum() ?? 0m;
        }

        public decimal BuscarValorTotalReceberPorCargaEModeloDocumento(int codigoCarga, int codigoModeloDocumento, string[] statusCTe, DateTime? dataAutorizacao, DateTime? dataCancelamento, DateTime? dataAnulacao)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.CargaCTe>();

            query = query.Where(o => o.Carga.Codigo == codigoCarga && statusCTe.Contains(o.CTe.Status) && o.CTe.ModeloDocumentoFiscal.Codigo == codigoModeloDocumento && o.CargaCTeComplementoInfo == null);

            if (dataAutorizacao.HasValue)
                query = query.Where(o => o.CTe.DataAutorizacao >= dataAutorizacao.Value.Date && o.CTe.DataAutorizacao < dataAutorizacao.Value.AddDays(1).Date);

            if (dataCancelamento.HasValue)
                query = query.Where(o => o.CTe.DataCancelamento >= dataCancelamento.Value.Date && o.CTe.DataCancelamento < dataCancelamento.Value.AddDays(1).Date);

            if (dataAnulacao.HasValue)
                query = query.Where(o => o.CTe.DataAnulacao >= dataAnulacao.Value.Date && o.CTe.DataAnulacao < dataAnulacao.Value.AddDays(1).Date);

            return query.Select(o => o.CTe.ValorAReceber).Sum();
        }

        public decimal BuscarValorTotalReceberPorCarga(int codigoCarga, string statusCTe)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.CargaCTe>();

            query = query.Where(o => o.Carga.Codigo == codigoCarga && o.CTe.Status == statusCTe && o.CargaCTeComplementoInfo == null && !o.CTe.ModeloDocumentoFiscal.NaoGerarFaturamento);
            //query = query.Where(o => o.Carga.TipoOperacao == null || !o.Carga.TipoOperacao.NaoGerarFaturamento);

            return query.Select(o => (decimal?)o.CTe.ValorAReceber).Sum() ?? 0m;
        }

        public decimal? BuscarValorTotalMoedaPorCarga(int codigoCarga, string statusCTe)
        {
            IQueryable<Dominio.Entidades.Embarcador.Cargas.CargaCTe> query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.CargaCTe>();

            query = query.Where(o => o.Carga.Codigo == codigoCarga && o.CTe.Status == statusCTe && o.CargaCTeComplementoInfo == null && !o.CTe.ModeloDocumentoFiscal.NaoGerarFaturamento);
            //query = query.Where(o => o.Carga.TipoOperacao == null || !o.Carga.TipoOperacao.NaoGerarFaturamento);

            return query.Select(o => o.CTe.ValorTotalMoeda).Sum();
        }

        public Dominio.ObjetosDeValor.Embarcador.Enumeradores.MoedaCotacaoBancoCentral? BuscarMoedaPorCarga(int codigoCarga, string statusCTe)
        {
            IQueryable<Dominio.Entidades.Embarcador.Cargas.CargaCTe> query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.CargaCTe>();

            query = query.Where(o => o.Carga.Codigo == codigoCarga && o.CTe.Status == statusCTe && o.CargaCTeComplementoInfo == null && !o.CTe.ModeloDocumentoFiscal.NaoGerarFaturamento);
            //query = query.Where(o => o.Carga.TipoOperacao == null || !o.Carga.TipoOperacao.NaoGerarFaturamento);

            return query.Select(o => o.CTe.Moeda).FirstOrDefault();
        }

        public decimal? BuscarCotacaoMoedaPorCarga(int codigoCarga, string statusCTe)
        {
            IQueryable<Dominio.Entidades.Embarcador.Cargas.CargaCTe> query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.CargaCTe>();

            query = query.Where(o => o.Carga.Codigo == codigoCarga && o.CTe.Status == statusCTe && o.CargaCTeComplementoInfo == null && !o.CTe.ModeloDocumentoFiscal.NaoGerarFaturamento);
            //query = query.Where(o => o.Carga.TipoOperacao == null || !o.Carga.TipoOperacao.NaoGerarFaturamento);

            return query.Select(o => o.CTe.ValorCotacaoMoeda).FirstOrDefault();
        }

        public decimal BuscarValorFreteLiquidoPorCarga(int codigoCarga, string statusCTe)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.CargaCTe>();

            query = query.Where(o => o.Carga.Codigo == codigoCarga && o.CTe.Status == statusCTe && o.CargaCTeComplementoInfo == null && !o.CTe.ModeloDocumentoFiscal.NaoGerarFaturamento);
            //query = query.Where(o => o.Carga.TipoOperacao == null || !o.Carga.TipoOperacao.NaoGerarFaturamento);

            return query.Select(o => (decimal?)o.CTe.ValorFrete).Sum() ?? 0m;
        }

        public decimal BuscarValorICMSPorCarga(int codigoCarga, string statusCTe)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.CargaCTe>();

            query = query.Where(o => o.Carga.Codigo == codigoCarga && o.CTe.Status == statusCTe && o.CargaCTeComplementoInfo == null && !o.CTe.ModeloDocumentoFiscal.NaoGerarFaturamento);
            //query = query.Where(o => o.Carga.TipoOperacao == null || !o.Carga.TipoOperacao.NaoGerarFaturamento);

            return query.Select(o => (decimal?)o.CTe.ValorICMS).Sum() ?? 0m;
        }

        public decimal BuscarValorICMSPorCargaSemAnulacao(int codigoCarga, string statusCTe)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.CargaCTe>();

            query = query.Where(o => o.Carga.Codigo == codigoCarga && o.CTe.Status == statusCTe && o.CTe.TipoCTE != Dominio.Enumeradores.TipoCTE.Anulacao);

            return query.Select(o => (decimal?)o.CTe.ValorICMS).Sum() ?? 0m;
        }

        public decimal BuscarValorISSPorCarga(int codigoCarga, string statusCTe)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.CargaCTe>();

            query = query.Where(o => o.Carga.Codigo == codigoCarga && o.CTe.Status == statusCTe && o.CargaCTeComplementoInfo == null && !o.CTe.ModeloDocumentoFiscal.NaoGerarFaturamento);
            //query = query.Where(o => o.Carga.TipoOperacao == null || !o.Carga.TipoOperacao.NaoGerarFaturamento);

            return query.Select(o => (decimal?)o.CTe.ValorISS).Sum() ?? 0m;
        }

        public decimal BuscarAliquotaICMSPorCarga(int codigoCarga, string statusCTe)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.CargaCTe>();

            query = query.Where(o => o.Carga.Codigo == codigoCarga && o.CTe.Status == statusCTe && o.CargaCTeComplementoInfo == null && !o.CTe.ModeloDocumentoFiscal.NaoGerarFaturamento);
            //query = query.Where(o => o.Carga.TipoOperacao == null || !o.Carga.TipoOperacao.NaoGerarFaturamento);

            return query.Select(o => (decimal?)o.CTe.AliquotaICMS).FirstOrDefault() ?? 0m;
        }

        public string BuscarProdutoPredominantePorCarga(int codigoCarga, string statusCTe)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.CargaCTe>();

            query = query.Where(o => o.Carga.Codigo == codigoCarga && o.CTe.Status == statusCTe && o.CargaCTeComplementoInfo == null && !o.CTe.ModeloDocumentoFiscal.NaoGerarFaturamento);
            //query = query.Where(o => o.Carga.TipoOperacao == null || !o.Carga.TipoOperacao.NaoGerarFaturamento);

            return query.Select(o => o.CTe.ProdutoPredominanteCTe).FirstOrDefault();
        }

        public decimal BuscarAliquotaISSPorCarga(int codigoCarga, string statusCTe)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.CargaCTe>();

            query = query.Where(o => o.Carga.Codigo == codigoCarga && o.CTe.Status == statusCTe && o.CargaCTeComplementoInfo == null && !o.CTe.ModeloDocumentoFiscal.NaoGerarFaturamento);
            //query = query.Where(o => o.Carga.TipoOperacao == null || !o.Carga.TipoOperacao.NaoGerarFaturamento);

            return query.Select(o => (decimal?)o.CTe.AliquotaISS).FirstOrDefault() ?? 0m;
        }

        public decimal BuscarPercentualRetencaoISSPorCarga(int codigoCarga, string statusCTe)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.CargaCTe>();

            query = query.Where(o => o.Carga.Codigo == codigoCarga && o.CTe.Status == statusCTe && o.CargaCTeComplementoInfo == null && !o.CTe.ModeloDocumentoFiscal.NaoGerarFaturamento);
            //query = query.Where(o => o.Carga.TipoOperacao == null || !o.Carga.TipoOperacao.NaoGerarFaturamento);

            return query.Select(o => (decimal?)o.CTe.PercentualISSRetido).FirstOrDefault() ?? 0m;
        }

        public DateTime BuscarUltimaDataEmissaoPorCarga(int codigoCarga, string statusCTe)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.CargaCTe>();

            query = query.Where(o => o.Carga.Codigo == codigoCarga && o.CTe.Status == statusCTe && o.CargaCTeComplementoInfo == null && !o.CTe.ModeloDocumentoFiscal.NaoGerarFaturamento);
            //query = query.Where(o => o.Carga.TipoOperacao == null || !o.Carga.TipoOperacao.NaoGerarFaturamento);

            return query.OrderByDescending(o => o.CTe.DataEmissao).Select(o => o.CTe.DataEmissao.Value).FirstOrDefault();
        }

        public DateTime BuscarDataUltimaEmissaoPorCarga(int codigoCarga, string statusCTe)
        {
            IQueryable<Dominio.Entidades.Embarcador.Cargas.CargaCTe> query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.CargaCTe>();

            query = query.Where(o => o.Carga.Codigo == codigoCarga && o.CTe.Status == statusCTe);

            return query.OrderByDescending(o => o.CTe.DataEmissao).Select(o => o.CTe.DataEmissao.Value).FirstOrDefault();
        }

        public DateTime? BuscarUltimaDataEmissaoNullablePorCarga(int codigoCarga, string statusCTe)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.CargaCTe>();

            query = query.Where(o => o.Carga.Codigo == codigoCarga && o.CTe.Status == statusCTe && o.CargaCTeComplementoInfo == null && !o.CTe.ModeloDocumentoFiscal.NaoGerarFaturamento);
            //query = query.Where(o => o.Carga.TipoOperacao == null || !o.Carga.TipoOperacao.NaoGerarFaturamento);

            return query.OrderByDescending(o => o.CTe.DataEmissao).Select(o => o.CTe.DataEmissao).FirstOrDefault();
        }

        public DateTime BuscarUltimaDataAutorizacaoPorCarga(int codigoCarga, string statusCTe)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.CargaCTe>();

            query = query.Where(o => o.Carga.Codigo == codigoCarga && o.CTe.Status == statusCTe && o.CargaCTeComplementoInfo == null && !o.CTe.ModeloDocumentoFiscal.NaoGerarFaturamento);
            //query = query.Where(o => o.Carga.TipoOperacao == null || !o.Carga.TipoOperacao.NaoGerarFaturamento);

            return query.OrderByDescending(o => o.CTe.DataAutorizacao).Select(o => o.CTe.DataAutorizacao.Value).FirstOrDefault();
        }

        public DateTime? BuscarUltimaDataVinculoCargaPorCarga(int codigoCarga, string statusCTe)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.CargaCTe>();

            query = query.Where(o => o.Carga.Codigo == codigoCarga && o.CTe.Status == statusCTe && o.CargaCTeComplementoInfo == null && !o.CTe.ModeloDocumentoFiscal.NaoGerarFaturamento);
            //query = query.Where(o => o.Carga.TipoOperacao == null || !o.Carga.TipoOperacao.NaoGerarFaturamento);

            return query.OrderByDescending(o => o.DataVinculoCarga).Select(o => o.DataVinculoCarga).FirstOrDefault();
        }

        public Dominio.ObjetosDeValor.Embarcador.Enumeradores.SistemaEmissor BuscarUltimoSistemaEmissorPorCarga(int codigoCarga, string statusCTe)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.CargaCTe>();

            query = query.Where(o => o.Carga.Codigo == codigoCarga && o.CTe.Status == statusCTe && o.CargaCTeComplementoInfo == null && !o.CTe.ModeloDocumentoFiscal.NaoGerarFaturamento);
            //query = query.Where(o => o.Carga.TipoOperacao == null || !o.Carga.TipoOperacao.NaoGerarFaturamento);

            return query.Select(o => o.SistemaEmissor).FirstOrDefault();
        }

        public DateTime? BuscarUltimaDataVinculoCargaPorCTe(int codigoCTe)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.CargaCTe>();

            query = query.Where(o => o.CTe.Codigo == codigoCTe && !o.CTe.ModeloDocumentoFiscal.NaoGerarFaturamento);
            //query = query.Where(o => o.Carga.TipoOperacao == null || !o.Carga.TipoOperacao.NaoGerarFaturamento);

            return query.OrderByDescending(o => o.DataVinculoCarga).Select(o => o.DataVinculoCarga).FirstOrDefault();
        }

        public Dominio.ObjetosDeValor.Embarcador.Enumeradores.SistemaEmissor BuscarUltimoSistemaEmissorPorCTe(int codigoCTe)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.CargaCTe>();

            query = query.Where(o => o.CTe.Codigo == codigoCTe && !o.CTe.ModeloDocumentoFiscal.NaoGerarFaturamento);
            //query = query.Where(o => o.Carga.TipoOperacao == null || !o.Carga.TipoOperacao.NaoGerarFaturamento);

            return query.Select(o => o.SistemaEmissor).FirstOrDefault();
        }

        public DateTime BuscarUltimaDataCancelamentoPorCarga(int codigoCarga, string statusCTe)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.CargaCTe>();

            query = query.Where(o => o.Carga.Codigo == codigoCarga && o.CTe.Status == statusCTe);

            return query.OrderByDescending(o => o.CTe.DataCancelamento).Select(o => o.CTe.DataCancelamento.Value).FirstOrDefault();
        }

        public DateTime BuscarUltimaDataAnulacaoPorCarga(int codigoCarga, string statusCTe)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.CargaCTe>();

            query = query.Where(o => o.Carga.Codigo == codigoCarga && o.CTe.Status == statusCTe);

            return query.OrderByDescending(o => o.CTe.DataAnulacao).Select(o => o.CTe.DataAnulacao.Value).FirstOrDefault();
        }

        public List<DateTime> BuscarDatasAutorizacaoPorCarga(int codigoCarga, string[] statusCTe)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.CargaCTe>();

            query = query.Where(o => o.Carga.Codigo == codigoCarga && statusCTe.Contains(o.CTe.Status));

            return query.Select(o => o.CTe.DataAutorizacao.Value.Date).Distinct().ToList();
        }

        public List<DateTime> BuscarDatasEmissaoPorCarga(int codigoCarga)
        {
            IQueryable<Dominio.Entidades.Embarcador.Cargas.CargaCTe> query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.CargaCTe>();

            query = query.Where(o => o.Carga.Codigo == codigoCarga);

            return query.Select(o => o.CTe.DataEmissao.Value.Date).Distinct().ToList();
        }

        public List<DateTime> BuscarDatasCancelamentoPorCarga(int codigoCarga, string statusCTe)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.CargaCTe>();

            query = query.Where(o => o.Carga.Codigo == codigoCarga && o.CTe.Status == statusCTe && o.CargaCTeComplementoInfo == null);

            return query.Select(o => o.CTe.DataCancelamento.Value.Date).Distinct().ToList();
        }

        public List<DateTime> BuscarDatasAnulacaoPorCarga(int codigoCarga, string statusCTe)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.CargaCTe>();

            query = query.Where(o => o.Carga.Codigo == codigoCarga && o.CTe.Status == statusCTe && o.CargaCTeComplementoInfo == null);

            return query.Select(o => o.CTe.DataAnulacao.Value.Date).Distinct().ToList();
        }

        public decimal BuscarValorICMSPorCargaEModeloDocumento(int codigoCarga, int codigoModeloDocumento, string statusCTe, DateTime? dataAutorizacao, DateTime? dataCancelamento, DateTime? dataAnulacao)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.CargaCTe>();

            query = query.Where(o => o.Carga.Codigo == codigoCarga && o.CTe.Status == statusCTe && o.CTe.ModeloDocumentoFiscal.Codigo == codigoModeloDocumento && o.CargaCTeComplementoInfo == null);

            if (dataAutorizacao.HasValue)
                query = query.Where(o => o.CTe.DataAutorizacao >= dataAutorizacao.Value.Date && o.CTe.DataAutorizacao < dataAutorizacao.Value.AddDays(1).Date);

            if (dataCancelamento.HasValue)
                query = query.Where(o => o.CTe.DataCancelamento >= dataCancelamento.Value.Date && o.CTe.DataCancelamento < dataCancelamento.Value.AddDays(1).Date);

            if (dataAnulacao.HasValue)
                query = query.Where(o => o.CTe.DataAnulacao >= dataAnulacao.Value.Date && o.CTe.DataAnulacao < dataAnulacao.Value.AddDays(1).Date);

            return query.Select(o => o.CTe.ValorICMS).Sum();
        }

        public decimal BuscarValorPISPorCargaEModeloDocumento(int codigoCarga, int codigoModeloDocumento, string statusCTe, DateTime? dataAutorizacao, DateTime? dataCancelamento, DateTime? dataAnulacao)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.CargaCTe>();

            query = query.Where(o => o.Carga.Codigo == codigoCarga && o.CTe.Status == statusCTe && o.CTe.ModeloDocumentoFiscal.Codigo == codigoModeloDocumento && o.CargaCTeComplementoInfo == null);

            if (dataAutorizacao.HasValue)
                query = query.Where(o => o.CTe.DataAutorizacao >= dataAutorizacao.Value.Date && o.CTe.DataAutorizacao < dataAutorizacao.Value.AddDays(1).Date);

            if (dataCancelamento.HasValue)
                query = query.Where(o => o.CTe.DataCancelamento >= dataCancelamento.Value.Date && o.CTe.DataCancelamento < dataCancelamento.Value.AddDays(1).Date);

            if (dataAnulacao.HasValue)
                query = query.Where(o => o.CTe.DataAnulacao >= dataAnulacao.Value.Date && o.CTe.DataAnulacao < dataAnulacao.Value.AddDays(1).Date);

            return query.Select(o => o.CTe.ValorPIS).Sum();
        }

        public decimal BuscarValorCOFINSPorCargaEModeloDocumento(int codigoCarga, int codigoModeloDocumento, string statusCTe, DateTime? dataAutorizacao, DateTime? dataCancelamento, DateTime? dataAnulacao)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.CargaCTe>();

            query = query.Where(o => o.Carga.Codigo == codigoCarga && o.CTe.Status == statusCTe && o.CTe.ModeloDocumentoFiscal.Codigo == codigoModeloDocumento && o.CargaCTeComplementoInfo == null);

            if (dataAutorizacao.HasValue)
                query = query.Where(o => o.CTe.DataAutorizacao >= dataAutorizacao.Value.Date && o.CTe.DataAutorizacao < dataAutorizacao.Value.AddDays(1).Date);

            if (dataCancelamento.HasValue)
                query = query.Where(o => o.CTe.DataCancelamento >= dataCancelamento.Value.Date && o.CTe.DataCancelamento < dataCancelamento.Value.AddDays(1).Date);

            if (dataAnulacao.HasValue)
                query = query.Where(o => o.CTe.DataAnulacao >= dataAnulacao.Value.Date && o.CTe.DataAnulacao < dataAnulacao.Value.AddDays(1).Date);

            return query.Select(o => o.CTe.ValorCOFINS).Sum();
        }

        public decimal BuscarValorIRPorCargaEModeloDocumento(int codigoCarga, int codigoModeloDocumento, string statusCTe, DateTime? dataAutorizacao, DateTime? dataCancelamento, DateTime? dataAnulacao)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.CargaCTe>();

            query = query.Where(o => o.Carga.Codigo == codigoCarga && o.CTe.Status == statusCTe && o.CTe.ModeloDocumentoFiscal.Codigo == codigoModeloDocumento && o.CargaCTeComplementoInfo == null);

            if (dataAutorizacao.HasValue)
                query = query.Where(o => o.CTe.DataAutorizacao >= dataAutorizacao.Value.Date && o.CTe.DataAutorizacao < dataAutorizacao.Value.AddDays(1).Date);

            if (dataCancelamento.HasValue)
                query = query.Where(o => o.CTe.DataCancelamento >= dataCancelamento.Value.Date && o.CTe.DataCancelamento < dataCancelamento.Value.AddDays(1).Date);

            if (dataAnulacao.HasValue)
                query = query.Where(o => o.CTe.DataAnulacao >= dataAnulacao.Value.Date && o.CTe.DataAnulacao < dataAnulacao.Value.AddDays(1).Date);

            return query.Select(o => o.CTe.ValorIR).Sum();
        }

        public decimal BuscarValorCSLLPorCargaEModeloDocumento(int codigoCarga, int codigoModeloDocumento, string statusCTe, DateTime? dataAutorizacao, DateTime? dataCancelamento, DateTime? dataAnulacao)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.CargaCTe>();

            query = query.Where(o => o.Carga.Codigo == codigoCarga && o.CTe.Status == statusCTe && o.CTe.ModeloDocumentoFiscal.Codigo == codigoModeloDocumento && o.CargaCTeComplementoInfo == null);

            if (dataAutorizacao.HasValue)
                query = query.Where(o => o.CTe.DataAutorizacao >= dataAutorizacao.Value.Date && o.CTe.DataAutorizacao < dataAutorizacao.Value.AddDays(1).Date);

            if (dataCancelamento.HasValue)
                query = query.Where(o => o.CTe.DataCancelamento >= dataCancelamento.Value.Date && o.CTe.DataCancelamento < dataCancelamento.Value.AddDays(1).Date);

            if (dataAnulacao.HasValue)
                query = query.Where(o => o.CTe.DataAnulacao >= dataAnulacao.Value.Date && o.CTe.DataAnulacao < dataAnulacao.Value.AddDays(1).Date);

            return query.Select(o => o.CTe.ValorCSLL).Sum();
        }

        public decimal BuscarValorICMSPorCargaEModeloDocumento(int codigoCarga, int codigoModeloDocumento, string[] statusCTe, DateTime? dataAutorizacao, DateTime? dataCancelamento, DateTime? dataAnulacao)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.CargaCTe>();

            query = query.Where(o => o.Carga.Codigo == codigoCarga && statusCTe.Contains(o.CTe.Status) && o.CTe.ModeloDocumentoFiscal.Codigo == codigoModeloDocumento && o.CargaCTeComplementoInfo == null);

            if (dataAutorizacao.HasValue)
                query = query.Where(o => o.CTe.DataAutorizacao >= dataAutorizacao.Value.Date && o.CTe.DataAutorizacao < dataAutorizacao.Value.AddDays(1).Date);

            if (dataCancelamento.HasValue)
                query = query.Where(o => o.CTe.DataCancelamento >= dataCancelamento.Value.Date && o.CTe.DataCancelamento < dataCancelamento.Value.AddDays(1).Date);

            if (dataAnulacao.HasValue)
                query = query.Where(o => o.CTe.DataAnulacao >= dataAnulacao.Value.Date && o.CTe.DataAnulacao < dataAnulacao.Value.AddDays(1).Date);

            return query.Select(o => o.CTe.ValorICMS).Sum();
        }

        public decimal BuscarValorPISPorCargaEModeloDocumento(int codigoCarga, int codigoModeloDocumento, string[] statusCTe, DateTime? dataAutorizacao, DateTime? dataCancelamento, DateTime? dataAnulacao)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.CargaCTe>();

            query = query.Where(o => o.Carga.Codigo == codigoCarga && statusCTe.Contains(o.CTe.Status) && o.CTe.ModeloDocumentoFiscal.Codigo == codigoModeloDocumento && o.CargaCTeComplementoInfo == null);

            if (dataAutorizacao.HasValue)
                query = query.Where(o => o.CTe.DataAutorizacao >= dataAutorizacao.Value.Date && o.CTe.DataAutorizacao < dataAutorizacao.Value.AddDays(1).Date);

            if (dataCancelamento.HasValue)
                query = query.Where(o => o.CTe.DataCancelamento >= dataCancelamento.Value.Date && o.CTe.DataCancelamento < dataCancelamento.Value.AddDays(1).Date);

            if (dataAnulacao.HasValue)
                query = query.Where(o => o.CTe.DataAnulacao >= dataAnulacao.Value.Date && o.CTe.DataAnulacao < dataAnulacao.Value.AddDays(1).Date);

            return query.Select(o => o.CTe.ValorPIS).Sum();
        }

        public decimal BuscarValorCOFINSPorCargaEModeloDocumento(int codigoCarga, int codigoModeloDocumento, string[] statusCTe, DateTime? dataAutorizacao, DateTime? dataCancelamento, DateTime? dataAnulacao)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.CargaCTe>();

            query = query.Where(o => o.Carga.Codigo == codigoCarga && statusCTe.Contains(o.CTe.Status) && o.CTe.ModeloDocumentoFiscal.Codigo == codigoModeloDocumento && o.CargaCTeComplementoInfo == null);

            if (dataAutorizacao.HasValue)
                query = query.Where(o => o.CTe.DataAutorizacao >= dataAutorizacao.Value.Date && o.CTe.DataAutorizacao < dataAutorizacao.Value.AddDays(1).Date);

            if (dataCancelamento.HasValue)
                query = query.Where(o => o.CTe.DataCancelamento >= dataCancelamento.Value.Date && o.CTe.DataCancelamento < dataCancelamento.Value.AddDays(1).Date);

            if (dataAnulacao.HasValue)
                query = query.Where(o => o.CTe.DataAnulacao >= dataAnulacao.Value.Date && o.CTe.DataAnulacao < dataAnulacao.Value.AddDays(1).Date);

            return query.Select(o => o.CTe.ValorCOFINS).Sum();
        }

        public decimal BuscarValorIRPorCargaEModeloDocumento(int codigoCarga, int codigoModeloDocumento, string[] statusCTe, DateTime? dataAutorizacao, DateTime? dataCancelamento, DateTime? dataAnulacao)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.CargaCTe>();

            query = query.Where(o => o.Carga.Codigo == codigoCarga && statusCTe.Contains(o.CTe.Status) && o.CTe.ModeloDocumentoFiscal.Codigo == codigoModeloDocumento && o.CargaCTeComplementoInfo == null);

            if (dataAutorizacao.HasValue)
                query = query.Where(o => o.CTe.DataAutorizacao >= dataAutorizacao.Value.Date && o.CTe.DataAutorizacao < dataAutorizacao.Value.AddDays(1).Date);

            if (dataCancelamento.HasValue)
                query = query.Where(o => o.CTe.DataCancelamento >= dataCancelamento.Value.Date && o.CTe.DataCancelamento < dataCancelamento.Value.AddDays(1).Date);

            if (dataAnulacao.HasValue)
                query = query.Where(o => o.CTe.DataAnulacao >= dataAnulacao.Value.Date && o.CTe.DataAnulacao < dataAnulacao.Value.AddDays(1).Date);

            return query.Select(o => o.CTe.ValorIR).Sum();
        }

        public decimal BuscarValorCSLLPorCargaEModeloDocumento(int codigoCarga, int codigoModeloDocumento, string[] statusCTe, DateTime? dataAutorizacao, DateTime? dataCancelamento, DateTime? dataAnulacao)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.CargaCTe>();

            query = query.Where(o => o.Carga.Codigo == codigoCarga && statusCTe.Contains(o.CTe.Status) && o.CTe.ModeloDocumentoFiscal.Codigo == codigoModeloDocumento && o.CargaCTeComplementoInfo == null);

            if (dataAutorizacao.HasValue)
                query = query.Where(o => o.CTe.DataAutorizacao >= dataAutorizacao.Value.Date && o.CTe.DataAutorizacao < dataAutorizacao.Value.AddDays(1).Date);

            if (dataCancelamento.HasValue)
                query = query.Where(o => o.CTe.DataCancelamento >= dataCancelamento.Value.Date && o.CTe.DataCancelamento < dataCancelamento.Value.AddDays(1).Date);

            if (dataAnulacao.HasValue)
                query = query.Where(o => o.CTe.DataAnulacao >= dataAnulacao.Value.Date && o.CTe.DataAnulacao < dataAnulacao.Value.AddDays(1).Date);

            return query.Select(o => o.CTe.ValorCSLL).Sum();
        }

        public decimal BuscarValorFreteLiquidoPorCargaEModeloDocumento(int codigoCarga, int codigoModeloDocumento, string statusCTe, DateTime? dataAutorizacao, DateTime? dataCancelamento, DateTime? dataAnulacao)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.CargaCTe>();

            query = query.Where(o => o.Carga.Codigo == codigoCarga && o.CTe.Status == statusCTe && o.CTe.ModeloDocumentoFiscal.Codigo == codigoModeloDocumento && o.CargaCTeComplementoInfo == null);

            if (dataAutorizacao.HasValue)
                query = query.Where(o => o.CTe.DataAutorizacao >= dataAutorizacao.Value.Date && o.CTe.DataAutorizacao < dataAutorizacao.Value.AddDays(1).Date);

            if (dataCancelamento.HasValue)
                query = query.Where(o => o.CTe.DataCancelamento >= dataCancelamento.Value.Date && o.CTe.DataCancelamento < dataCancelamento.Value.AddDays(1).Date);

            if (dataAnulacao.HasValue)
                query = query.Where(o => o.CTe.DataAnulacao >= dataAnulacao.Value.Date && o.CTe.DataAnulacao < dataAnulacao.Value.AddDays(1).Date);

            return query.Select(o => o.CTe.ValorFrete).Sum();
        }

        public decimal BuscarValorFreteLiquidoPorCargaEModeloDocumento(int codigoCarga, int codigoModeloDocumento, string[] statusCTe, DateTime? dataAutorizacao, DateTime? dataCancelamento, DateTime? dataAnulacao)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.CargaCTe>();

            query = query.Where(o => o.Carga.Codigo == codigoCarga && statusCTe.Contains(o.CTe.Status) && o.CTe.ModeloDocumentoFiscal.Codigo == codigoModeloDocumento && o.CargaCTeComplementoInfo == null);

            if (dataAutorizacao.HasValue)
                query = query.Where(o => o.CTe.DataAutorizacao >= dataAutorizacao.Value.Date && o.CTe.DataAutorizacao < dataAutorizacao.Value.AddDays(1).Date);

            if (dataCancelamento.HasValue)
                query = query.Where(o => o.CTe.DataCancelamento >= dataCancelamento.Value.Date && o.CTe.DataCancelamento < dataCancelamento.Value.AddDays(1).Date);

            if (dataAnulacao.HasValue)
                query = query.Where(o => o.CTe.DataAnulacao >= dataAnulacao.Value.Date && o.CTe.DataAnulacao < dataAnulacao.Value.AddDays(1).Date);

            return query.Select(o => o.CTe.ValorFrete).Sum();
        }

        public decimal BuscarValorFreteLiquidoPorCarga(int codigoCarga)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.CargaCTe>();

            query = query.Where(o => o.Carga.Codigo == codigoCarga);

            return query.Select(o => o.CTe.ValorFrete).Sum();
        }

        public decimal BuscarValorComponentesNegativosPorCarga(int codigoCarga)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.CargaComponentesFrete>();
            query = query.Where(o => o.Carga.Codigo == codigoCarga && o.ValorComponente < 0);

            if (query.Count() > 0)
                return query.Select(o => o.ValorComponente).Sum();
            else
                return 0;
        }

        public decimal BuscarValorAReceberPorCarga(int codigoCarga)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.CargaCTe>();

            query = query.Where(o => o.Carga.Codigo == codigoCarga && o.CargaCTeComplementoInfo == null && o.CTe.TipoCTE != Dominio.Enumeradores.TipoCTE.Anulacao && o.CTe.Status != "Z" && o.CTe.Status != "C");

            return query.Select(o => (decimal?)o.CTe.ValorAReceber).Sum() ?? 0m;
        }

        public decimal BuscarValorAReceberPorCarga(int codigoCarga, string status)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.CargaCTe>();

            query = query.Where(o => o.Carga.Codigo == codigoCarga && o.CTe.Status == status && o.CargaCTeComplementoInfo == null && !o.CTe.ModeloDocumentoFiscal.NaoGerarFaturamento);
            //query = query.Where(o => o.Carga.TipoOperacao == null || !o.Carga.TipoOperacao.NaoGerarFaturamento);

            return query.Select(o => (decimal?)o.CTe.ValorAReceber).Sum() ?? 0m;
        }

        public List<Dominio.Entidades.ConhecimentoDeTransporteEletronico> BuscarCTesAutorizadosPorCarga(int codigoCarga)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.CargaCTe>();

            query = query.Where(o => o.Carga.Codigo == codigoCarga && o.CTe.ModeloDocumentoFiscal.Numero == "57" && o.CTe.Status == "A");

            return query.Select(o => o.CTe).ToList();
        }

        public List<int> BuscarCodigosCTesAutorizadosPorCarga(List<int> codigosCargas, bool ctesSubContratacaoFilialEmissora, bool ctesSemSubContratacaoFilialEmissora)
        {
            Dominio.Enumeradores.TipoDocumento[] tiposDocumentosAutorizados = new Dominio.Enumeradores.TipoDocumento[]
            {
                 Dominio.Enumeradores.TipoDocumento.CTe,
                  Dominio.Enumeradores.TipoDocumento.Outros
            };

            string[] statusPermitido = new string[] { "A", "Z" };
            //"C" Foi removido a opção de imprimir documento cancelados  #72486 (Unilever)
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.CargaCTe>();

            query = query.Where(o => codigosCargas.Contains(o.Carga.Codigo) && tiposDocumentosAutorizados.Contains(o.CTe.ModeloDocumentoFiscal.TipoDocumentoEmissao) && statusPermitido.Contains(o.CTe.Status));

            if (ctesSubContratacaoFilialEmissora)
                query = query.Where(o => o.CargaCTeFilialEmissora != null || (o.CargaCTeFilialEmissora == null && o.CargaCTeSubContratacaoFilialEmissora == null));

            if (ctesSemSubContratacaoFilialEmissora)
                query = query.Where(o => o.CargaCTeFilialEmissora == null);

            query = query.OrderBy("CTe.Numero");
            return query.Select(o => o.CTe.Codigo).ToList();
        }

        public List<int> BuscarCodigosCTesAutorizadosPorCarga(int codigoCarga, bool ctesSubContratacaoFilialEmissora, bool ctesSemSubContratacaoFilialEmissora, bool ctesComplementares = true)
        {
            Dominio.Enumeradores.TipoDocumento[] tiposDocumentosAutorizados = new Dominio.Enumeradores.TipoDocumento[]
            {
                 Dominio.Enumeradores.TipoDocumento.CTe,
                  Dominio.Enumeradores.TipoDocumento.Outros
            };

            string[] statusPermitido = new string[] { "A", "Z" };
            //"C" Foi removido a opção de imprimir documento cancelados  #72486 (Unilever)

            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.CargaCTe>();

            query = query.Where(o => o.Carga.Codigo == codigoCarga && tiposDocumentosAutorizados.Contains(o.CTe.ModeloDocumentoFiscal.TipoDocumentoEmissao) && statusPermitido.Contains(o.CTe.Status));

            if (ctesSubContratacaoFilialEmissora)
                query = query.Where(o => o.CargaCTeFilialEmissora != null || (o.CargaCTeFilialEmissora == null && o.CargaCTeSubContratacaoFilialEmissora == null));

            if (ctesSemSubContratacaoFilialEmissora)
                query = query.Where(o => o.CargaCTeFilialEmissora == null);

            if (!ctesComplementares)
                query = query.Where(o => o.CargaCTeComplementoInfo == null);

            query = query.OrderBy("CTe.Numero");
            return query.Select(o => o.CTe.Codigo).ToList();
        }

        public List<int> BuscarCodigosCTeAutorizadoPorCarga(int codigoCarga, bool ctesSubContratacaoFilialEmissora, bool ctesSemSubContratacaoFilialEmissora, bool ctesComplementares = true)
        {
            Dominio.Enumeradores.TipoDocumento[] tiposDocumentosAutorizados = new Dominio.Enumeradores.TipoDocumento[]
            {
                 Dominio.Enumeradores.TipoDocumento.CTe
            };

            string[] statusPermitido = new string[] { "A", "Z" };
            //"C" Foi removido a opção de imprimir documento cancelados  #72486 (Unilever)

            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.CargaCTe>();

            query = query.Where(o => o.Carga.Codigo == codigoCarga && tiposDocumentosAutorizados.Contains(o.CTe.ModeloDocumentoFiscal.TipoDocumentoEmissao) && statusPermitido.Contains(o.CTe.Status));

            if (ctesSubContratacaoFilialEmissora)
                query = query.Where(o => o.CargaCTeFilialEmissora != null || (o.CargaCTeFilialEmissora == null && o.CargaCTeSubContratacaoFilialEmissora == null));

            if (ctesSemSubContratacaoFilialEmissora)
                query = query.Where(o => o.CargaCTeFilialEmissora == null);

            if (!ctesComplementares)
                query = query.Where(o => o.CargaCTeComplementoInfo == null);

            query = query.OrderBy("CTe.Numero");
            return query.Select(o => o.CTe.Codigo).ToList();
        }

        public List<int> BuscarCodigosCTesNFesAutorizadosPorCarga(int codigoCarga, bool ctesSubContratacaoFilialEmissora, bool ctesSemSubContratacaoFilialEmissora, bool ctesComplementares = true)
        {
            Dominio.Enumeradores.TipoDocumento[] tiposDocumentosAutorizados = new Dominio.Enumeradores.TipoDocumento[]
            {
                 Dominio.Enumeradores.TipoDocumento.CTe,
                 Dominio.Enumeradores.TipoDocumento.NFSe,
                  Dominio.Enumeradores.TipoDocumento.Outros
            };

            string[] statusPermitido = new string[] { "A", "Z" };
            //"C" Removido na tarefa #72486 (Unilever) para não imprimir documento Cancelados

            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.CargaCTe>();

            query = query.Where(o => o.CargaCTeComplementoInfo == null && o.Carga.Codigo == codigoCarga && tiposDocumentosAutorizados.Contains(o.CTe.ModeloDocumentoFiscal.TipoDocumentoEmissao) && statusPermitido.Contains(o.CTe.Status));

            if (ctesSubContratacaoFilialEmissora)
                query = query.Where(o => o.CargaCTeFilialEmissora != null || (o.CargaCTeFilialEmissora == null && o.CargaCTeSubContratacaoFilialEmissora == null));

            if (ctesSemSubContratacaoFilialEmissora)
                query = query.Where(o => o.CargaCTeFilialEmissora == null);

            if (!ctesComplementares)
                query = query.Where(o => o.CargaCTeComplementoInfo == null);

            query = query.OrderBy("CTe.Numero");
            return query.Select(o => o.CTe.Codigo).ToList();
        }

        public List<Dominio.Entidades.Embarcador.Cargas.CargaCTe> ObterDocumentosModificadosPorUltimaConsulta(int motorista, DateTime dataUltimaConsulta)
        {
            var query = SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.CargaCTe>();
            var result = query.Where(obj => obj.Carga.Motoristas.Any(mot => mot.Codigo == motorista));
            return result.ToList();
        }

        public List<Dominio.Entidades.ConhecimentoDeTransporteEletronico> ConsultarSAC(string numeroPedidoEmbarcador, double codigoRecebedor, string numeroPedidoCF, double cnpjClienteUsuario, string numeroSolicitacao, DateTime dataEmissaoNotaDe, DateTime dataEmissaoNotaAte, DateTime dataEmissaoCTeDe, DateTime dataEmissaoCTeAte,
            string numeroNotaDe, int serieNota, int numeroCTeDe, int numeroCTeAte, int serieCTe, int numeroFaturaDe, int numeroFaturaAte, int numeroPreFatura,
            string numeroCarga, int numeroPedido,
            int codigoEmpresaOrigem, int codigoEmpresaDestino, int codigoCidadeOrigem, int codigoCidadeDestino, int codigoGrupoPessoa, int codigoTipoOperacao, int codigoVeiculo, int codigoMotorista, int codigoTipoCarga,
            double codigoCliente, double codigoRemetente, double codigoDestinatario, string propOrdenar, string dirOrdena, int inicio, int limite)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.CargaCTe>();

            query = query.Where(o => o.Carga.CargaTransbordo == false);

            if (cnpjClienteUsuario > 0)
                query = query.Where(o => o.NotasFiscais.Any(nf => nf.PedidoXMLNotaFiscal.XMLNotaFiscal.Destinatario.CPF_CNPJ == cnpjClienteUsuario || nf.PedidoXMLNotaFiscal.XMLNotaFiscal.Recebedor.CPF_CNPJ == cnpjClienteUsuario || nf.PedidoXMLNotaFiscal.XMLNotaFiscal.Emitente.CPF_CNPJ == cnpjClienteUsuario));

            if (dataEmissaoNotaDe != DateTime.MinValue)
                query = query.Where(o => o.CTe.Documentos.Any(nf => nf.DataEmissao.Date >= dataEmissaoNotaDe));
            if (dataEmissaoNotaAte != DateTime.MinValue)
                query = query.Where(o => o.CTe.Documentos.Any(nf => nf.DataEmissao.Date <= dataEmissaoNotaAte));

            if (dataEmissaoCTeDe != DateTime.MinValue)
                query = query.Where(o => o.CTe.DataEmissao.Value.Date >= dataEmissaoCTeDe);
            if (dataEmissaoCTeAte != DateTime.MinValue)
                query = query.Where(o => o.CTe.DataEmissao.Value.Date <= dataEmissaoCTeAte);

            if (!string.IsNullOrWhiteSpace(numeroNotaDe))
                query = query.Where(o => o.CTe.Documentos.Any(nf => nf.Numero.Equals(numeroNotaDe)));
            if (serieNota > 0)
                query = query.Where(o => o.CTe.Documentos.Any(nf => nf.Serie.Equals(serieNota.ToString())));

            if (numeroCTeDe > 0)
                query = query.Where(o => o.CTe.Numero >= numeroCTeDe);
            if (numeroCTeAte > 0)
                query = query.Where(o => o.CTe.Numero <= numeroCTeAte);
            if (serieCTe > 0)
                query = query.Where(o => o.CTe.Serie.Numero == serieCTe);

            var queryFatura = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Fatura.FaturaDocumento>();
            var resultFatura = from obj in queryFatura where obj.Fatura.Situacao != Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoFatura.Cancelado select obj;

            if (numeroFaturaDe > 0)
                query = query.Where(o => resultFatura.Where(r => r.Fatura.Numero >= numeroFaturaDe).Select(r => r.Documento.CTe).Any(c => c.Codigo == o.CTe.Codigo));
            if (numeroFaturaAte > 0)
                query = query.Where(o => resultFatura.Where(r => r.Fatura.Numero <= numeroFaturaAte).Select(r => r.Documento.CTe).Any(c => c.Codigo == o.CTe.Codigo));
            if (numeroPreFatura > 0)
                query = query.Where(o => resultFatura.Where(r => r.Fatura.NumeroPreFatura == numeroPreFatura).Select(r => r.Documento.CTe).Any(c => c.Codigo == o.CTe.Codigo));

            if (!string.IsNullOrWhiteSpace(numeroSolicitacao))
                query = query.Where(o => o.NotasFiscais.Any(nf => nf.PedidoXMLNotaFiscal.XMLNotaFiscal.NumeroSolicitacao == numeroSolicitacao));

            if (!string.IsNullOrWhiteSpace(numeroPedidoCF))
                query = query.Where(o => o.CTe.Documentos.Any(r => r.NumeroPedido == numeroPedidoCF));

            if (!string.IsNullOrWhiteSpace(numeroCarga))
                query = query.Where(o => o.Carga.CodigoCargaEmbarcador == numeroCarga);

            if (numeroPedido > 0)
                query = query.Where(o => o.Carga.Pedidos.Any(ped => ped.Pedido.Numero == numeroPedido));

            if (!string.IsNullOrWhiteSpace(numeroPedidoEmbarcador))
                query = query.Where(o => o.Carga.Pedidos.Any(ped => ped.Pedido.NumeroPedidoEmbarcador == numeroPedidoEmbarcador));

            if (codigoEmpresaOrigem > 0)
                query = query.Where(o => o.Carga.EmpresaFilialEmissora.Codigo == codigoEmpresaOrigem);
            if (codigoEmpresaDestino > 0)
                query = query.Where(o => o.Carga.Empresa.Codigo == codigoEmpresaDestino);
            if (codigoCidadeOrigem > 0)
                query = query.Where(o => o.CTe.LocalidadeInicioPrestacao.Codigo == codigoCidadeOrigem);
            if (codigoCidadeDestino > 0)
                query = query.Where(o => o.CTe.LocalidadeTerminoPrestacao.Codigo == codigoCidadeDestino);
            if (codigoGrupoPessoa > 0)
                query = query.Where(o => o.Carga.GrupoPessoaPrincipal.Codigo == codigoGrupoPessoa);
            if (codigoTipoOperacao > 0)
                query = query.Where(o => o.Carga.TipoOperacao.Codigo == codigoTipoOperacao);
            if (codigoVeiculo > 0)
                query = query.Where(o => o.Carga.Veiculo.Codigo == codigoVeiculo);
            if (codigoMotorista > 0)
                query = query.Where(o => o.Carga.Motoristas.Any(mot => mot.Codigo == codigoMotorista));
            if (codigoTipoCarga > 0)
                query = query.Where(o => o.Carga.TipoDeCarga.Codigo == codigoTipoCarga);

            if (codigoCliente > 0)
                query = query.Where(o => o.NotasFiscais.Any(nf => nf.PedidoXMLNotaFiscal.XMLNotaFiscal.Destinatario.CPF_CNPJ == codigoCliente));
            if (codigoRemetente > 0)
                query = query.Where(o => o.CTe.Remetente.Cliente.CPF_CNPJ == codigoRemetente);
            if (codigoDestinatario > 0)
                query = query.Where(o => o.CTe.Destinatario.Cliente.CPF_CNPJ == codigoDestinatario);
            if (codigoRecebedor > 0)
                query = query.Where(o => o.CTe.Recebedor.Cliente.CPF_CNPJ == codigoRecebedor);

            var queryCTe = this.SessionNHiBernate.Query<Dominio.Entidades.ConhecimentoDeTransporteEletronico>();
            var queryFinal = query
                .Fetch(o => o.CTe).ThenFetch(o => o.Serie)
                .Fetch(o => o.CTe).ThenFetch(o => o.ModeloDocumentoFiscal)
                .Fetch(o => o.CTe).ThenFetch(o => o.Empresa)
                .Fetch(o => o.CTe).ThenFetch(o => o.Remetente)
                .Fetch(o => o.CTe).ThenFetch(o => o.Destinatario)
                .Fetch(o => o.CTe).ThenFetch(o => o.LocalidadeTerminoPrestacao)
                .Join(queryCTe, vei => vei.CTe.Codigo, emp => emp.Codigo, (vei, emp) => emp);

            return queryFinal
                .OrderBy(propOrdenar + " " + dirOrdena)
                .Skip(inicio).Take(limite)
                .ToList();
        }

        public int ContarConsultaSAC(string numeroPedidoEmbarcador, double codigoRecebedor, string numeroPedidoCF, double cnpjClienteUsuario, string numeroSolicitacao, DateTime dataEmissaoNotaDe, DateTime dataEmissaoNotaAte, DateTime dataEmissaoCTeDe, DateTime dataEmissaoCTeAte,
            string numeroNotaDe, int serieNota, int numeroCTeDe, int numeroCTeAte, int serieCTe, int numeroFaturaDe, int numeroFaturaAte, int numeroPreFatura,
            string numeroCarga, int numeroPedido,
            int codigoEmpresaOrigem, int codigoEmpresaDestino, int codigoCidadeOrigem, int codigoCidadeDestino, int codigoGrupoPessoa, int codigoTipoOperacao, int codigoVeiculo, int codigoMotorista, int codigoTipoCarga,
            double codigoCliente, double codigoRemetente, double codigoDestinatario)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.CargaCTe>();

            query = query.Where(o => o.Carga.CargaTransbordo == false);

            if (cnpjClienteUsuario > 0)
                query = query.Where(o => o.NotasFiscais.Any(nf => nf.PedidoXMLNotaFiscal.XMLNotaFiscal.Destinatario.CPF_CNPJ == cnpjClienteUsuario || nf.PedidoXMLNotaFiscal.XMLNotaFiscal.Recebedor.CPF_CNPJ == cnpjClienteUsuario || nf.PedidoXMLNotaFiscal.XMLNotaFiscal.Emitente.CPF_CNPJ == cnpjClienteUsuario));

            if (dataEmissaoNotaDe != DateTime.MinValue)
                query = query.Where(o => o.CTe.Documentos.Any(nf => nf.DataEmissao.Date >= dataEmissaoNotaDe));
            if (dataEmissaoNotaAte != DateTime.MinValue)
                query = query.Where(o => o.CTe.Documentos.Any(nf => nf.DataEmissao.Date <= dataEmissaoNotaAte));

            if (dataEmissaoCTeDe != DateTime.MinValue)
                query = query.Where(o => o.CTe.DataEmissao.Value.Date >= dataEmissaoCTeDe);
            if (dataEmissaoCTeAte != DateTime.MinValue)
                query = query.Where(o => o.CTe.DataEmissao.Value.Date <= dataEmissaoCTeAte);

            if (!string.IsNullOrWhiteSpace(numeroNotaDe))
                query = query.Where(o => o.CTe.Documentos.Any(nf => nf.Numero.Equals(numeroNotaDe)));
            if (serieNota > 0)
                query = query.Where(o => o.CTe.Documentos.Any(nf => nf.Serie.Equals(serieNota.ToString())));

            if (numeroCTeDe > 0)
                query = query.Where(o => o.CTe.Numero >= numeroCTeDe);
            if (numeroCTeAte > 0)
                query = query.Where(o => o.CTe.Numero <= numeroCTeAte);
            if (serieCTe > 0)
                query = query.Where(o => o.CTe.Serie.Numero == serieCTe);

            var queryFatura = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Fatura.FaturaDocumento>();
            var resultFatura = from obj in queryFatura where obj.Fatura.Situacao != Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoFatura.Cancelado select obj;

            if (numeroFaturaDe > 0)
                query = query.Where(o => resultFatura.Where(r => r.Fatura.Numero >= numeroFaturaDe).Select(r => r.Documento.CTe).Any(c => c.Codigo == o.CTe.Codigo));
            if (numeroFaturaAte > 0)
                query = query.Where(o => resultFatura.Where(r => r.Fatura.Numero <= numeroFaturaAte).Select(r => r.Documento.CTe).Any(c => c.Codigo == o.CTe.Codigo));
            if (numeroPreFatura > 0)
                query = query.Where(o => resultFatura.Where(r => r.Fatura.NumeroPreFatura == numeroPreFatura).Select(r => r.Documento.CTe).Any(c => c.Codigo == o.CTe.Codigo));

            if (!string.IsNullOrWhiteSpace(numeroSolicitacao))
                query = query.Where(o => o.NotasFiscais.Any(nf => nf.PedidoXMLNotaFiscal.XMLNotaFiscal.NumeroSolicitacao == numeroSolicitacao));

            if (!string.IsNullOrWhiteSpace(numeroPedidoCF))
                query = query.Where(o => o.CTe.Documentos.Any(r => r.NumeroPedido == numeroPedidoCF));

            if (!string.IsNullOrWhiteSpace(numeroCarga))
                query = query.Where(o => o.Carga.CodigoCargaEmbarcador == numeroCarga);

            if (numeroPedido > 0)
                query = query.Where(o => o.Carga.Pedidos.Any(ped => ped.Pedido.Numero == numeroPedido));

            if (!string.IsNullOrWhiteSpace(numeroPedidoEmbarcador))
                query = query.Where(o => o.Carga.Pedidos.Any(ped => ped.Pedido.NumeroPedidoEmbarcador == numeroPedidoEmbarcador));

            if (codigoEmpresaOrigem > 0)
                query = query.Where(o => o.Carga.EmpresaFilialEmissora.Codigo == codigoEmpresaOrigem);
            if (codigoEmpresaDestino > 0)
                query = query.Where(o => o.Carga.Empresa.Codigo == codigoEmpresaDestino);
            if (codigoCidadeOrigem > 0)
                query = query.Where(o => o.CTe.LocalidadeInicioPrestacao.Codigo == codigoCidadeOrigem);
            if (codigoCidadeDestino > 0)
                query = query.Where(o => o.CTe.LocalidadeTerminoPrestacao.Codigo == codigoCidadeDestino);
            if (codigoGrupoPessoa > 0)
                query = query.Where(o => o.Carga.GrupoPessoaPrincipal.Codigo == codigoGrupoPessoa);
            if (codigoTipoOperacao > 0)
                query = query.Where(o => o.Carga.TipoOperacao.Codigo == codigoTipoOperacao);
            if (codigoVeiculo > 0)
                query = query.Where(o => o.Carga.Veiculo.Codigo == codigoVeiculo);
            if (codigoMotorista > 0)
                query = query.Where(o => o.Carga.Motoristas.Any(mot => mot.Codigo == codigoMotorista));
            if (codigoTipoCarga > 0)
                query = query.Where(o => o.Carga.TipoDeCarga.Codigo == codigoTipoCarga);

            if (codigoCliente > 0)
                query = query.Where(o => o.NotasFiscais.Any(nf => nf.PedidoXMLNotaFiscal.XMLNotaFiscal.Destinatario.CPF_CNPJ == codigoCliente));
            if (codigoRemetente > 0)
                query = query.Where(o => o.CTe.Remetente.Cliente.CPF_CNPJ == codigoRemetente);
            if (codigoDestinatario > 0)
                query = query.Where(o => o.CTe.Destinatario.Cliente.CPF_CNPJ == codigoDestinatario);
            if (codigoRecebedor > 0)
                query = query.Where(o => o.CTe.Recebedor.Cliente.CPF_CNPJ == codigoRecebedor);

            var queryCTe = this.SessionNHiBernate.Query<Dominio.Entidades.ConhecimentoDeTransporteEletronico>();
            var queryFinal = query.Join(queryCTe, vei => vei.CTe.Codigo, emp => emp.Codigo, (vei, emp) => emp);

            return queryFinal.Count();
        }

        //public List<int> BuscarCodigoCTePorCarga(int codigoCarga)
        //{
        //    var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.CargaCTe>();

        //    query = query.Where(obj => obj.Carga.Codigo == codigoCarga);

        //    return query.Select(o => o.CTe.Codigo).ToList();
        //}

        public List<Dominio.Entidades.Embarcador.Cargas.CargaCTe> BuscarCTesPorFechamento(int fechamento)
        {
            var queryFechamento = (from obj in SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Fechamento.FechamentoFreteCarga>() where obj.Fechamento.Codigo == fechamento select obj.Carga);
            var query = SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.CargaCTe>();

            var result = from obj in query where queryFechamento.Contains(obj.Carga) select obj;
            return result.ToList();
        }

        public bool PossuiCTesInvalidosParaAnulacao(int codigoCarga)
        {
            IQueryable<Dominio.Entidades.Embarcador.Cargas.CargaCTe> query = SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.CargaCTe>();

            query = query.Where(o => o.Carga.Codigo == codigoCarga &&
                                     o.CTe != null &&
                                     (o.CTe.ModeloDocumentoFiscal.Numero == "57" ||
                                      o.CTe.ModeloDocumentoFiscal.Numero == "39") &&
                                     (o.CTe.Status == "E" ||
                                      o.CTe.Status == "P" ||
                                      o.CTe.Status == "K" ||
                                      o.CTe.Status == "L"));

            return query.Any();
        }

        public bool PossuiCTesEmitidosPeloEmbarcador(int codigoCarga)
        {
            IQueryable<Dominio.Entidades.Embarcador.Cargas.CargaCTe> query = SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.CargaCTe>();

            query = query.Where(o => o.Carga.Codigo == codigoCarga &&
                                     o.SistemaEmissor != Dominio.ObjetosDeValor.Embarcador.Enumeradores.SistemaEmissor.MultiCTe);

            return query.Any();
        }

        public bool PossuiCTesComPrazoCancelamentoEsgotado(int codigoCarga)
        {
            IQueryable<Dominio.Entidades.Embarcador.Cargas.CargaCTe> query = SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.CargaCTe>();

            query = query.Where(o => o.Carga.Codigo == codigoCarga &&
                                     o.CTe != null &&
                                     o.SistemaEmissor == Dominio.ObjetosDeValor.Embarcador.Enumeradores.SistemaEmissor.MultiCTe &&
                                     o.CTe.ModeloDocumentoFiscal.Numero == "57" &&
                                     o.CTe.Status == "A" &&
                                     o.CTe.DataRetornoSefaz < DateTime.Now.AddDays(-7));

            return query.Any();
        }

        public bool PossuiCTesAutorizadosEmitidosPeloEmbarcador(int codigoCarga)
        {
            IQueryable<Dominio.Entidades.Embarcador.Cargas.CargaCTe> query = SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.CargaCTe>();

            query = query.Where(o => o.Carga.Codigo == codigoCarga &&
                                     o.CTe != null &&
                                     o.SistemaEmissor != Dominio.ObjetosDeValor.Embarcador.Enumeradores.SistemaEmissor.MultiCTe &&
                                     o.CTe.Status == "A");

            return query.Any();
        }

        public bool ExisteCTeCanceladoInutilizadoOuDenegado(int codigoCarga)
        {
            var consultaCargaCte = SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.CargaCTe>()
                .Where(o =>
                    o.Carga.Codigo == codigoCarga &&
                    o.CTe != null &&
                    (o.CTe.ModeloDocumentoFiscal.TipoDocumentoEmissao == Dominio.Enumeradores.TipoDocumento.CTe || o.CTe.ModeloDocumentoFiscal.TipoDocumentoEmissao == Dominio.Enumeradores.TipoDocumento.NFSe) &&
                    (o.CTe.Status == "C" || o.CTe.Status == "I" || o.CTe.Status == "D")
                );

            return consultaCargaCte.Any();
        }

        public bool ExisteCTeEmCancelamento(int codigoCarga)
        {
            IQueryable<Dominio.Entidades.Embarcador.Cargas.CargaCTe> query = SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.CargaCTe>();

            query = query.Where(o => o.Carga.Codigo == codigoCarga &&
                                     o.CTe != null &&
                                     o.SistemaEmissor == Dominio.ObjetosDeValor.Embarcador.Enumeradores.SistemaEmissor.MultiCTe &&
                                     (o.CTe.ModeloDocumentoFiscal.TipoDocumentoEmissao == Dominio.Enumeradores.TipoDocumento.CTe || o.CTe.ModeloDocumentoFiscal.TipoDocumentoEmissao == Dominio.Enumeradores.TipoDocumento.NFSe) &&
                                     (o.CTe.Status == "K" || o.CTe.Status == "L"));

            return query.Any();
        }

        public bool ExisteCTeNaoCancelado(int codigoCarga, AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware tipoServicoMultisoftware, bool liberadoSemInutilizacao, bool liberarNaoEmitidosEmbarcador)
        {
            IQueryable<Dominio.Entidades.Embarcador.Cargas.CargaCTe> query = SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.CargaCTe>();

            query = query.Where(o => o.Carga.Codigo == codigoCarga &&
                                     o.CTe != null &&
                                     (o.CTe.ModeloDocumentoFiscal.TipoDocumentoEmissao == Dominio.Enumeradores.TipoDocumento.CTe || o.CTe.ModeloDocumentoFiscal.TipoDocumentoEmissao == Dominio.Enumeradores.TipoDocumento.NFSe) &&
                                     o.CargaCTeTrechoAnterior == null &&
                                     o.CTe.Status != "C" &&
                                     o.CTe.Status != "I" &&
                                     o.CTe.Status != "D" &&
                                     o.CTe.Status != "Z");

            if (liberadoSemInutilizacao)
                query = query.Where(obj => obj.CTe.Status != "R" || (obj.CTe.Status == "R" && ((obj.CTe.MensagemStatus == null && !obj.CTe.MensagemRetornoSefaz.Contains("Data de Validade do Certificado já expirou")) || !obj.CTe.MensagemStatus.PermiteLiberarSemInutilizacao)));

            if (tipoServicoMultisoftware == AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiTMS || liberarNaoEmitidosEmbarcador)
                query = query.Where(o => o.SistemaEmissor == Dominio.ObjetosDeValor.Embarcador.Enumeradores.SistemaEmissor.MultiCTe);

            return query.Any();
        }

        public Dominio.Entidades.Embarcador.Cargas.CargaCTe BuscarPorCodigoComFetch(int codigoCargaCTe)
        {
            IQueryable<Dominio.Entidades.Embarcador.Cargas.CargaCTe> query = SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.CargaCTe>();

            query = query.Where(o => o.Codigo == codigoCargaCTe);

            return query.Fetch(o => o.CTe).ThenFetch(o => o.Empresa)
                        .Fetch(o => o.CTe).ThenFetch(o => o.ModeloDocumentoFiscal)
                        .FirstOrDefault();
        }

        public List<int> BuscarCodigosPorCargaParaCancelamento(int codigoCarga, AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware tipoServicoMultisoftware, bool obterDocumentosEmitidosEmbarcador = false, int codigoCTeCancelamentoUnitario = 0)
        {
            IQueryable<Dominio.Entidades.Embarcador.Cargas.CargaCTe> query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.CargaCTe>();

            query = query.Where(o => o.Carga.Codigo == codigoCarga && o.CargaCTeComplementoInfo == null && o.CargaCTeTrechoAnterior == null);

            if (tipoServicoMultisoftware == AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiTMS && !obterDocumentosEmitidosEmbarcador)
                query = query.Where(o => o.SistemaEmissor == Dominio.ObjetosDeValor.Embarcador.Enumeradores.SistemaEmissor.MultiCTe);

            if (codigoCTeCancelamentoUnitario > 0)
                query = query.Where(o => o.CTe.Codigo == codigoCTeCancelamentoUnitario);

            return query.Select(o => o.Codigo).ToList();
        }

        public bool ExisteModeloDeDocumentoEmitidoNaCarga(int codigoCarga, List<int> codigosModelosDocumentos)
        {
            IQueryable<Dominio.Entidades.Embarcador.Cargas.CargaCTe> query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.CargaCTe>();

            query = query.Where(o => o.Carga.Codigo == codigoCarga && codigosModelosDocumentos.Contains(o.CTe.ModeloDocumentoFiscal.Codigo));

            return query.Any();
        }

        public List<Dominio.Entidades.Embarcador.Cargas.CargaCTe> BuscarPorCargaEModelosDocumentos(int codigoCarga, List<int> codigosModelosDocumentos)
        {
            IQueryable<Dominio.Entidades.Embarcador.Cargas.CargaCTe> query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.CargaCTe>();

            query = query.Where(o => o.Carga.Codigo == codigoCarga && codigosModelosDocumentos.Contains(o.CTe.ModeloDocumentoFiscal.Codigo));

            return query.Fetch(o => o.CTe).ToList();
        }

        public List<string> BuscarCodigosCTesPorOcorrencia(int codigoOcorrencia)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.CargaCTe>();

            query = query.Where(o => o.CargaCTeComplementoInfo.CargaOcorrencia.Codigo == codigoOcorrencia);

            return query.Select(o => o.CTe.Codigo.ToString()).ToList();
        }

        public List<Dominio.Entidades.Embarcador.Cargas.CargaCTe> BuscarCTesPorOcorrencia(int codigoOcorrencia)
        {
            IQueryable<Dominio.Entidades.Embarcador.Cargas.CargaCTe> query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.CargaCTe>();

            query = query.Where(o => o.CargaCTeComplementoInfo.CargaOcorrencia.Codigo == codigoOcorrencia);

            return query.Fetch(o => o.CTe).ToList();
        }

        public List<Dominio.Entidades.Embarcador.Cargas.CargaCTe> BuscarPorCargaAutorizadosSemAnulacao(int codigoCarga)
        {
            IQueryable<Dominio.Entidades.Embarcador.Cargas.CargaCTe> query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.CargaCTe>();

            query = query.Where(o => o.Carga.Codigo == codigoCarga && o.CTe.TipoCTE != Dominio.Enumeradores.TipoCTE.Anulacao && o.CTe.Status == "A");

            return query.Fetch(o => o.CTe).ToList();
        }

        public bool ExistePorCTe(int codigoCTe)
        {
            IQueryable<Dominio.Entidades.Embarcador.Cargas.CargaCTe> query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.CargaCTe>();

            query = query.Where(o => o.CTe.Codigo == codigoCTe);

            return query.Select(o => o.Codigo).Any();
        }

        public bool ExistePorCargaECTe(int codigoCarga, int codigoCTe)
        {
            IQueryable<Dominio.Entidades.Embarcador.Cargas.CargaCTe> query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.CargaCTe>();

            query = query.Where(o => o.Carga.Codigo == codigoCarga && o.CTe.Codigo == codigoCTe);

            return query.Select(o => o.Codigo).Any();
        }

        public bool ExisteAutorizadoPorCTe(IEnumerable<int> codigosCTes)
        {
            IQueryable<Dominio.Entidades.Embarcador.Cargas.CargaCTe> query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.CargaCTe>();

            query = query.Where(o => codigosCTes.Contains(o.CTe.Codigo) && o.Carga.SituacaoCarga != Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoCarga.Cancelada && o.Carga.SituacaoCarga != Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoCarga.Anulada);

            return query.Select(o => o.Codigo).Any();
        }

        public Dominio.Entidades.Embarcador.Cargas.CargaCTe BuscarAutorizadoPorCTe(List<int> codigosCTes)
        {
            IQueryable<Dominio.Entidades.Embarcador.Cargas.CargaCTe> query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.CargaCTe>();

            int quantidadeRegistrosConsultarPorVez = 1000;
            int quantidadeConsultas = codigosCTes.Count / quantidadeRegistrosConsultarPorVez;

            for (int i = 0; i <= quantidadeConsultas; i++)
            {
                Dominio.Entidades.Embarcador.Cargas.CargaCTe cargaCTe = query.Where(o => o.Carga.SituacaoCarga != Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoCarga.Anulada &&
                                                                                                                       o.Carga.SituacaoCarga != Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoCarga.Cancelada &&
                                                                                                                       codigosCTes.Skip(i * quantidadeRegistrosConsultarPorVez).Take(quantidadeRegistrosConsultarPorVez).Contains(o.CTe.Codigo)).FirstOrDefault();

                if (cargaCTe != null)
                    return cargaCTe;
            }

            return null;
        }

        public List<Dominio.Entidades.ConhecimentoDeTransporteEletronico> RetornarCTesExisteAutorizadoPorCTe(IEnumerable<int> codigosCTes)
        {
            IQueryable<Dominio.Entidades.Embarcador.Cargas.CargaCTe> query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.CargaCTe>();

            query = query.Where(o => codigosCTes.Contains(o.CTe.Codigo) && o.Carga.SituacaoCarga != Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoCarga.Cancelada && o.Carga.SituacaoCarga != Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoCarga.Anulada);

            return query.Select(o => o.CTe).ToList();
        }

        public bool ExisteAutorizadoPorCTe(int codigoCTe)
        {
            IQueryable<Dominio.Entidades.Embarcador.Cargas.CargaCTe> query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.CargaCTe>();

            query = query.Where(o => o.CTe.Codigo == codigoCTe && o.Carga.SituacaoCarga != Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoCarga.Cancelada && o.Carga.SituacaoCarga != Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoCarga.Anulada);

            return query.Select(o => o.Codigo).Any();
        }

        public bool ExisteAutorizadoPorCarga(int carga)
        {
            IQueryable<Dominio.Entidades.Embarcador.Cargas.CargaCTe> query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.CargaCTe>();

            query = query.Where(o => o.Carga.Codigo == carga);

            return query.Select(o => o.Codigo).Any();
        }

        public List<string> BuscarCodigosCTePorOcorrencia(int codigoOcorrencia)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.CargaCTe>();

            query = query.Where(o => o.CargaCTeComplementoInfo.CargaOcorrencia.Codigo == codigoOcorrencia && o.CargaCTeComplementoInfo.CTe.ModeloDocumentoFiscal.Numero == "57");

            return query.Select(o => o.CTe.Codigo.ToString()).ToList();
        }

        public List<string> BuscarCodigosNFSePorOcorrencia(int codigoOcorrencia)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.CargaCTe>();

            query = query.Where(o => o.CargaCTeComplementoInfo.CargaOcorrencia.Codigo == codigoOcorrencia && o.CargaCTeComplementoInfo.CTe.ModeloDocumentoFiscal.Numero == "39");

            return query.Select(o => o.CTe.Codigo.ToString()).ToList();
        }

        public List<string> BuscarCodigosCTePorCarga(int codigoCarga)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.CargaCTe>();

            query = query.Where(o => o.CargaCTeComplementoInfo == null && o.Carga.Codigo == codigoCarga && o.CTe.ModeloDocumentoFiscal.Numero == "57");

            return query.Select(o => o.CTe.Codigo.ToString()).ToList();
        }

        public List<int> BuscarCodigosInteiroCTePorCarga(int codigoCarga)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.CargaCTe>();

            query = query.Where(o => o.Carga.Codigo == codigoCarga && o.CTe.ModeloDocumentoFiscal.Numero == "57");

            return query.Select(o => o.CTe.Codigo).ToList();
        }

        public List<int> BuscarCodigosNumericosCTePorCarga(int codigoCarga)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.CargaCTe>();

            query = query.Where(o => o.CargaCTeComplementoInfo == null && o.Carga.SituacaoCarga != SituacaoCarga.Cancelada && o.Carga.Codigo == codigoCarga
            && o.CTe.ModeloDocumentoFiscal.Numero == "57" && o.CTe.Status == "C");

            return query.Select(o => o.CTe.Codigo).ToList();
        }

        public List<int> BuscarCodigosPorModeloDocumentoFiscalECarga(List<Dominio.Enumeradores.TipoDocumento> modelosDocumentos, int codigoCarga)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.CargaCTe>();

            query = query.Where(o => o.CargaCTeComplementoInfo == null && o.Carga.Codigo == codigoCarga && modelosDocumentos.Contains(o.CTe.ModeloDocumentoFiscal.TipoDocumentoEmissao));

            return query.Select(o => o.CTe.Codigo).ToList();
        }

        public List<int> BuscarCodigosNFSePorCarga(int codigoCarga)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.CargaCTe>();

            query = query.Where(o => o.CargaCTeComplementoInfo == null && o.Carga.Codigo == codigoCarga && o.CTe.ModeloDocumentoFiscal.Numero == "39");

            return query.Select(o => o.CTe.Codigo).ToList();
        }

        public List<int> BuscarCodigosNFSeAutorizadosPorCarga(int codigoCarga)
        {
            string[] statusPermitido = new string[] { "A", "C", "Z" };

            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.CargaCTe>();

            query = query.Where(o => o.CargaCTeComplementoInfo == null && o.Carga.Codigo == codigoCarga && o.CTe.ModeloDocumentoFiscal.Numero == "39" && statusPermitido.Contains(o.CTe.Status));

            return query.Select(o => o.CTe.Codigo).ToList();
        }

        public List<string> BuscarCodigosStringNFSePorCarga(int codigoCarga)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.CargaCTe>();

            query = query.Where(o => o.CargaCTeComplementoInfo == null && o.Carga.Codigo == codigoCarga && o.CTe.ModeloDocumentoFiscal.Numero == "39");

            return query.Select(o => o.CTe.Codigo.ToString()).ToList();
        }

        public decimal BuscarVolumesPorCarga(int codigoCarga, string status)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.CargaCTe>();

            query = query.Where(o => o.Carga.Codigo == codigoCarga && o.CTe.Status == status && o.CargaCTeComplementoInfo == null && !o.CTe.ModeloDocumentoFiscal.NaoGerarFaturamento);
            //query = query.Where(o => o.Carga.TipoOperacao == null || !o.Carga.TipoOperacao.NaoGerarFaturamento);

            return query.Select(o => (decimal?)o.CTe.Volumes).Sum() ?? 0m;
        }

        public bool ExisteCanceladoPorCarga(int codigoCarga)
        {
            string[] status = new string[] { "C", "I", "Z" };

            IQueryable<Dominio.Entidades.Embarcador.Cargas.CargaCTe> query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.CargaCTe>();

            query = query.Where(o => o.Carga.Codigo == codigoCarga && status.Contains(o.CTe.Status));

            return query.Select(o => o.Codigo).Any();
        }

        public bool ExisteNaoCanceladoPorCarga(int codigoCarga)
        {
            string[] status = new string[] { "C", "I", "Z" };

            IQueryable<Dominio.Entidades.Embarcador.Cargas.CargaCTe> query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.CargaCTe>();

            query = query.Where(o => o.Carga.Codigo == codigoCarga && !status.Contains(o.CTe.Status));

            return query.Select(o => o.Codigo).Any();
        }

        public bool ExisteCTeVinculadoATransbordo(int codigoCarga)
        {
            IQueryable<Dominio.Entidades.Embarcador.Cargas.CargaCTe> queryCargaCTe = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.CargaCTe>();

            queryCargaCTe = queryCargaCTe.Where(o => o.Carga.Codigo == codigoCarga);

            IQueryable<Dominio.Entidades.Embarcador.Cargas.CargaPedidoDocumentoCTe> queryCargaPedidoDocumentoCTe = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.CargaPedidoDocumentoCTe>();

            queryCargaPedidoDocumentoCTe = queryCargaPedidoDocumentoCTe.Where(o => o.CargaPedido.Carga.CargaTransbordo &&
                                                                                   o.CargaPedido.Carga.SituacaoCarga != Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoCarga.Cancelada &&
                                                                                   o.CargaPedido.Carga.SituacaoCarga != Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoCarga.Anulada &&
                                                                                   o.CargaPedido.Carga.Codigo != codigoCarga &&
                                                                                   queryCargaCTe.Any(cct => cct.CTe.Codigo == o.CTe.Codigo));

            return queryCargaPedidoDocumentoCTe.Select(o => o.Codigo).Any();
        }

        public decimal BuscarValorTotalMoedaPorCarga(int codigoCarga)
        {
            IQueryable<Dominio.Entidades.Embarcador.Cargas.CargaCTe> query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.CargaCTe>();

            query = query.Where(o => o.Carga.Codigo == codigoCarga && o.CargaCTeComplementoInfo == null);

            return query.Select(o => o.CTe.ValorTotalMoeda).Sum() ?? 0m;
        }

        public bool CargaDoCTeNaoEstaEmitida(int codigoCTe)
        {
            Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoCarga[] situacoesPermitidas = new Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoCarga[]
            {
                Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoCarga.AgIntegracao,
                Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoCarga.Anulada,
                Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoCarga.EmTransporte,
                Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoCarga.Encerrada
            };

            IQueryable<Dominio.Entidades.Embarcador.Cargas.CargaCTe> query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.CargaCTe>();

            query = query.Where(o => o.CTe.Codigo == codigoCTe && !o.Carga.CargaTransbordo && !situacoesPermitidas.Contains(o.Carga.SituacaoCarga));

            return query.Select(o => o.Codigo).Any();
        }

        public bool ExisteCTeEmCarga(int codigoCTe)
        {
            IQueryable<Dominio.Entidades.Embarcador.Cargas.CargaCTe> query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.CargaCTe>();

            query = query.Where(o => o.CTe.Codigo == codigoCTe && !o.Carga.CargaTransbordo);

            return query.Select(o => o.Codigo).Any();
        }

        public bool ExistePorCargaEModeloDocumentoFiscal(int codigoCarga, Dominio.Enumeradores.TipoDocumento modeloDocumentoFiscal)
        {
            return this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.CargaCTe>()
                .Any(o => o.Carga.Codigo == codigoCarga && o.CTe.ModeloDocumentoFiscal.TipoDocumentoEmissao == modeloDocumentoFiscal);
        }

        public Dominio.Entidades.Embarcador.Cargas.CargaCTe BuscarPorChaveCTeSubCompECarga(string chaveCTeSubComp, int codigoCarga)
        {
            IQueryable<Dominio.Entidades.Embarcador.Cargas.CargaCTe> query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.CargaCTe>();

            query = query.Where(obj => obj.CTe.ChaveCTESubComp == chaveCTeSubComp && obj.CTe.Status == "A" && obj.Carga.Codigo == codigoCarga);

            return query.FirstOrDefault();
        }

        public Dominio.Entidades.Embarcador.Cargas.CargaCTe BuscarPorChaveDocumentoAnterior(string chaveDocumentoAnterior, int codigoCarga)
        {
            IQueryable<Dominio.Entidades.Embarcador.Cargas.CargaCTe> query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.CargaCTe>();

            query = query.Where(obj => obj.CTe.DocumentosTransporteAnterior.Any(doc => doc.Chave == chaveDocumentoAnterior) && obj.CTe.Status == "A" && obj.Carga.Codigo == codigoCarga);

            return query.FirstOrDefault();
        }

        public List<Dominio.Entidades.Embarcador.Cargas.CargaCTe> BuscarCargaCtePorCarga(int codigoCarga)
        {
            IQueryable<Dominio.Entidades.Embarcador.Cargas.CargaCTe> query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.CargaCTe>();
            query = from obj in query where obj.Carga.Codigo == codigoCarga select obj;
            return query.ToList();
        }

        public List<Dominio.Entidades.Embarcador.Cargas.CargaCTe> BuscarCargaCtePorCargaAtivos(int codigoCarga)
        {
            IQueryable<Dominio.Entidades.Embarcador.Cargas.CargaCTe> query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.CargaCTe>();
            query = from obj in query where obj.Carga.Codigo == codigoCarga && (obj.CTe.Status == "A" || obj.CTe == null) select obj;
            return query.ToList();
        }

        public List<Dominio.Entidades.Embarcador.Cargas.CargaCTe> BuscarCargaCteRejeitadoPorCarga(int codigoCarga)
        {
            IQueryable<Dominio.Entidades.Embarcador.Cargas.CargaCTe> query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.CargaCTe>();
            query = from obj in query where obj.Carga.Codigo == codigoCarga && obj.CTe.Status == "R" select obj;
            return query.ToList();
        }

        public Dominio.Entidades.Embarcador.Cargas.Carga BuscarCargaFilhaRetornarPorCte(int codigoCte, int codigoCarga)
        {
            IQueryable<Dominio.Entidades.Embarcador.Cargas.CargaCTe> query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.CargaCTe>();
            query = from obj in query where obj.Carga.Codigo != codigoCarga && obj.CTe.Codigo == codigoCte && obj.Carga.ExigeNotaFiscalParaCalcularFrete select obj;
            return query.Select(x => x.Carga).FirstOrDefault();
        }

        public bool ExisteCTeComTituloPorCarga(int carga)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.CargaCTe>();

            var result = from obj in query where obj.CTe.Titulo != null && obj.Carga.Codigo == carga select obj;

            return result.Count() > 0;
        }

        public bool TodosCTesNaoContemMercante(int codigoCarga)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.CargaCTe>();
            var result = from obj in query
                         where obj.Carga.Codigo == codigoCarga
                               && (obj.CTe.NumeroCEMercante == "" || obj.CTe.NumeroCEMercante == null)
                               && obj.CTe.TipoCTE != Dominio.Enumeradores.TipoCTE.Anulacao
                               && obj.CTe.TipoModal != TipoModal.Multimodal
                               && obj.CTe.NaoEnviarParaMercante != true
                         select obj;

            return result.Any();
        }

        public bool TodosCTesNaoContemManifesto(int codigoCarga)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.CargaCTe>();
            var result = from obj in query
                         where obj.Carga.Codigo == codigoCarga
                               && (obj.CTe.NumeroManifesto == "" || obj.CTe.NumeroManifesto == null)
                               && obj.CTe.TipoCTE != Dominio.Enumeradores.TipoCTE.Anulacao
                               && obj.CTe.TipoModal != TipoModal.Multimodal
                               && obj.CTe.NaoEnviarParaMercante != true
                         select obj;

            return result.Any();
        }

        public Dominio.Entidades.Embarcador.Cargas.Carga BuscarCargaPorCTe(int codigoCTe)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.CargaCTe>();
            var result = from obj in query
                         where obj.CTe.Codigo == codigoCTe && !obj.Carga.CargaTransbordo &&
                             obj.Carga.SituacaoCarga != Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoCarga.Cancelada &&
                             obj.Carga.SituacaoCarga != Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoCarga.Anulada
                         select obj.Carga;

            return result.FirstOrDefault();
        }

        public Task<Dominio.Entidades.Embarcador.Cargas.Carga> BuscarCargaPorCodigoCTeAsync(int codigoCTe, CancellationToken cancellationToken)
        {
            IQueryable<Dominio.Entidades.Embarcador.Cargas.CargaCTe> query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.CargaCTe>()
                .Where(cargaCTe => cargaCTe.CTe.Codigo == codigoCTe);

            return query.Select(cargaCte => cargaCte.Carga).FirstOrDefaultAsync(cancellationToken);
        }

        public Dominio.Entidades.Embarcador.Cargas.CargaCTe BuscarCargaChaveCTeEProtocoloCarga(int protocoloCarga, string chaveCte)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.CargaCTe>();
            var result = from obj in query
                         where obj.CTe.Chave == chaveCte && obj.Carga.Protocolo == protocoloCarga
                         select obj;

            return result.FirstOrDefault();
        }

        public Dominio.Entidades.ConhecimentoDeTransporteEletronico BuscarNFSePorCarga(int codigoCarga, int numero, int serie, int codigoEmpresa, string protocolo)
        {
            IQueryable<Dominio.Entidades.Embarcador.Cargas.CargaCTe> query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.CargaCTe>();

            query = query.Where(o => o.Carga.Codigo == codigoCarga && o.CTe.Numero == numero && o.CTe.Serie.Numero == serie && o.CTe.Empresa.Codigo == codigoEmpresa && o.CTe.ModeloDocumentoFiscal.Numero == "39" && o.CTe.Protocolo == protocolo);

            return query.Select(o => o.CTe).FirstOrDefault();
        }

        public void RemoverVinculoCargaCTeComplemento(int codigoCargaCTeComplemento)
        {
            try
            {
                if (UnitOfWork.IsActiveTransaction())
                {
                    UnitOfWork.Sessao.CreateQuery("update CargaCTe set CargaCTeComplementoInfo.Codigo = null where CargaCTeComplementoInfo.Codigo = :codigoCargaCTeComplemento").SetInt32("codigoCargaCTeComplemento", codigoCargaCTeComplemento).ExecuteUpdate();
                }
                else
                {
                    try
                    {
                        UnitOfWork.Start();

                        UnitOfWork.Sessao.CreateQuery("update CargaCTe set CargaCTeComplementoInfo.Codigo = null where CargaCTeComplementoInfo.Codigo = :codigoCargaCTeComplemento").SetInt32("codigoCargaCTeComplemento", codigoCargaCTeComplemento).ExecuteUpdate();

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

        public List<Dominio.Entidades.ConhecimentoDeTransporteEletronico> BuscarDocumentosPorCarga(int codigoCarga)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.CargaCTe>()
                .Where(o => o.Carga.Codigo == codigoCarga);

            return query.Select(o => o.CTe).ToList();
        }

        public List<Dominio.Entidades.ConhecimentoDeTransporteEletronico> BuscarCTesPorCarga(int codigoCarga)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.CargaCTe>()
                .Where(o => o.Carga.Codigo == codigoCarga && o.CTe.ModeloDocumentoFiscal.Numero == "57");

            return query.Select(o => o.CTe).ToList();
        }

        public List<Dominio.Entidades.ConhecimentoDeTransporteEletronico> BuscarTodosCTesPorCarga(int codigoCarga)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.CargaCTe>()
                .Where(o => o.Carga.Codigo == codigoCarga);

            return query.Select(o => o.CTe).ToList();
        }

        public Task<List<Dominio.Entidades.ConhecimentoDeTransporteEletronico>> BuscarTodosCTesPorCargaAsync(int codigoCarga, CancellationToken cancellationToken)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.CargaCTe>()
                .Where(o => o.Carga.Codigo == codigoCarga);

            return query.Select(o => o.CTe).ToListAsync(cancellationToken);
        }

        public Task<bool> ExisteCTeNaoAutorizadoPorCargaAsync(int codigoCarga, CancellationToken cancellationToken)
        {
            IQueryable<Dominio.Entidades.Embarcador.Cargas.CargaCTe> query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.CargaCTe>()
                .Where(o => o.Carga.Codigo == codigoCarga && o.CTe.Status != "A");

            return query.AnyAsync(cancellationToken);
        }

        public bool CargaSVMDoCTe(int codigoCTe)
        {
            IQueryable<Dominio.Entidades.Embarcador.Cargas.CargaCTe> query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.CargaCTe>();

            query = query.Where(o => o.CTe.Codigo == codigoCTe && o.Carga.CargaSVM);

            return query.Any();
        }

        public bool EstaCargaPossuiCte(int codigoCarga)
        {
            IQueryable<Dominio.Entidades.Embarcador.Cargas.CargaCTe> query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.CargaCTe>();
            query = from obj in query where obj.Carga.Codigo == codigoCarga && obj.CTe != null select obj;
            return query.Any();
        }

        public int BuscarCodigoStageRelevantePorCargaCTe(int CodigoCargaCte)
        {
            string sql = $@"
              select top 1 stage.STA_CODIGO stage
              FROM T_STAGE stage
              inner join T_PEDIDO_STAGE pedStage on pedStage.STA_CODIGO = stage.STA_CODIGO
              inner join T_CARGA_PEDIDO CargaPedido on CargaPedido.STA_CODIGO_RELEVANTE_CUSTO = stage.STA_CODIGO
              inner join T_PEDIDO_XML_NOTA_FISCAL pedNota on pedNota.CPE_CODIGO = CargaPedido.CPE_CODIGO
              inner join T_CARGA_PEDIDO_XML_NOTA_FISCAL_CTE pedNotaCte on pedNotaCte.PNF_CODIGO = pedNota.PNF_CODIGO
              inner join T_CARGA_CTE cargaCte on cargaCte.CCT_CODIGO = pedNotaCte.CCT_CODIGO
              where cargaCte.CCT_CODIGO = {CodigoCargaCte}
            ";

            var consulta = this.SessionNHiBernate.CreateSQLQuery(sql);

            return consulta.SetTimeout(600).UniqueResult<int>();

        }

        public bool CargaPossuiCtesInviaveisParaIntegracao(int codigoCarga)
        {
            IQueryable<Dominio.Entidades.Embarcador.Cargas.CargaCTe> query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.CargaCTe>();
            query = from obj in query where obj.Carga.Codigo == codigoCarga && obj.CTe != null && SituacaoCheckinHelper.ObterSituacoesNaoLiberadasPreCheckin().Contains(obj.SituacaoCheckin) && obj.CTe.Status == "A" select obj;
            return query.Any();
        }

        public DateTime? BuscarDataEmissaoMaximaSubContratacao(int codigoCarga)
        {
            IQueryable<Dominio.Entidades.Embarcador.Cargas.CargaCTe> query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.CargaCTe>()
                .Where(cargaCTe => cargaCTe.Carga.Codigo == codigoCarga && cargaCTe.CTe.TipoServico == Dominio.Enumeradores.TipoServico.SubContratacao);

            return query.Max(cargaCTe => cargaCTe.CTe.DataEmissao);
        }

        public List<Dominio.Entidades.Embarcador.Cargas.CargaCTe> BuscarPreCteSemCTeAtivoPorTransportadoraETipoDocumento(int transportadora, Dominio.Enumeradores.TipoDocumento tipoDocumento, int inicio, int limite)
        {
            IQueryable<Dominio.Entidades.PreConhecimentoDeTransporteEletronico> queryPCO = this.SessionNHiBernate.Query<Dominio.Entidades.PreConhecimentoDeTransporteEletronico>();
            IQueryable<Dominio.Entidades.Embarcador.Cargas.CargaCTe> queryCCE = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.CargaCTe>();

            List<int> preCTes = queryPCO.Where(o => o.Empresa.Codigo == transportadora && o.ModeloDocumentoFiscal.TipoDocumentoEmissao == tipoDocumento).Select(o => o.Codigo).ToList();
            queryCCE = queryCCE.Where(o => preCTes.Contains(o.PreCTe.Codigo) && (o.CTe == null || o.CTe.Status == "C"));

            if (inicio > 0 || limite > 0)
                queryCCE = queryCCE.Skip(inicio).Take(limite);

            return queryCCE.ToList();
        }

        public List<int> BuscarCodigosCTesPorCarga(Dominio.Entidades.Embarcador.Cargas.Carga carga)
        {
            IQueryable<Dominio.Entidades.Embarcador.Cargas.CargaCTe> query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.CargaCTe>();

            query = query.Where(o => o.Carga == carga);

            return query.Select(o => o.CTe.Codigo).ToList();
        }

        public bool BuscarPorCargaValidaEmProcessamentoCte(Dominio.Entidades.Embarcador.Cargas.Carga carga)
        {
            IQueryable<Dominio.Entidades.Embarcador.Cargas.CargaCTe> query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.CargaCTe>();

            query = query.Where(o => o.Carga == carga);

            List<string> situacoes = new List<string>() { "P", "S", "K", "L", "E" };

            query = query.Where(o => situacoes.Contains(o.CTe.Status));

            return query.Any();
        }

        public List<int> BuscarPorCargaValidaRejeicaoCteNaoPermiteCancelamento(Dominio.Entidades.Embarcador.Cargas.Carga carga)
        {
            IQueryable<Dominio.Entidades.Embarcador.Cargas.CargaCTe> query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.CargaCTe>();

            query = query.Where(o => o.Carga == carga);

            // 100 - Autorizado o uso, exemplo: 100 -  Erro ao salvar. Cannot create file ""F:\Integradores\XML\2744014-pro-lot.xml"". O arquivo já está sendo usado por outro processo"
            // 204 - Erro de duplicidade já existe documento na sefaz, exemplo: Rejeicao: Existe CT-e ja autorizado com a mesma serie e numero[nProt:131242648000810][dhAut:2024 - 10 - 17 17:02:16.0]
            // 999 - Erro generico da sefaz, exemplo: Rejeicao: Erro nao catalogado
            // 9999 - Falhas de acessos a recursos, exemplo: 9999 - Erro ao salvar. Cannot create file "F:\Integradores\XML\1-ped-eve.xml".The process cannot access the file because it is being used by another process
            // 8888 - Erros de conexão com a sefaz ou certificado digital vencido
            List<string> codigoStatusProtocoloNaoCancelar = new List<string>() { "204", "100", "999", "9999", "8888" };

            query = query.Where(o => o.CTe.Status == "R" && codigoStatusProtocoloNaoCancelar.Contains(o.CTe.CodStatusProtocolo) || o.CTe.MensagemRetornoSefaz.Contains("Chave do CTe já autorizada com outros dados"));

            return query.Select(o => o.CTe.Numero).ToList();
        }

        public List<string> BuscarNumerosCtePorCarga(int codigoCarga)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.CargaCTe>();

            query = query.Where(o => o.Carga.Codigo == codigoCarga);

            return query.Select(o => o.CTe.Chave).ToList();
        }

        public List<Dominio.Entidades.Embarcador.Cargas.CargaCTe> BuscarPorCargaSemPreCte(int codigoCarga)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.CargaCTe>();
            var resut = from obj in query
                        where codigoCarga == obj.Carga.Codigo && obj.CTe != null
                        select obj;

            return resut.ToList();
        }

        public int BuscarProtocoloCargaPorCTe(int codigoCTe, int codigoEmpresa = 0)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.CargaCTe>();
            var resut = from obj in query where obj.CTe.Codigo == codigoCTe select obj;

            if (codigoEmpresa > 0)
                resut = resut.Where(c => c.CTe.Empresa.Codigo == codigoEmpresa);

            return resut.Select(obj => obj.Carga.Protocolo).FirstOrDefault();
        }

        public Task<List<(string DescricaoRotaFrete, int CodigoCargaCTe)>> BuscarDescricaoRotaFretePorCargaCTeAsync(List<int> codigosCargaCTe)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.CargaCTe>();

            return query
                .Where(obj => codigosCargaCTe.Contains(obj.Codigo))
                .SelectMany(obj => obj.NotasFiscais
                .Where(nf => nf.PedidoXMLNotaFiscal != null &&
                    nf.PedidoXMLNotaFiscal.CargaPedido != null &&
                    nf.PedidoXMLNotaFiscal.CargaPedido.Pedido != null &&
                    nf.PedidoXMLNotaFiscal.CargaPedido.Pedido.RotaFrete != null
                )
                .Select(nf =>
                    ValueTuple.Create(
                        nf.PedidoXMLNotaFiscal.CargaPedido.Pedido.RotaFrete.Descricao,
                        obj.Codigo
                        )
                    )
                ).ToListAsync(CancellationToken);
        }

        public List<Dominio.Entidades.Embarcador.Cargas.CargaCTe> BuscarPorCargaPedidoComComponentes(int cargaPedidoId)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.CargaCTe>()
                .Where(c =>
                    c.NotasFiscais.Any(n => n.PedidoXMLNotaFiscal.CargaPedido.Codigo == cargaPedidoId)
                )
                .FetchMany(c => c.Componentes);
            var lista = query
                .GroupBy(c => c.Codigo) 
                .Select(g => g.First())
                .ToList();

            return lista;
        }

        #endregion

        #region Métodos Privados

        private IQueryable<Dominio.Entidades.Embarcador.Cargas.CargaCTe> ObterQueryConsultaParaTransbordo(int codigoCarga, int numeroCTe, int numeroNotaFiscal, string numeroPedidoEmbarcador, Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoCarga[] situacoes, string propOrdenar = null, string dirOrdena = null, int inicio = 0, int limite = 0)
        {
            IQueryable<Dominio.Entidades.Embarcador.Cargas.CargaCTe> query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.CargaCTe>();

            if (codigoCarga <= 0 && numeroCTe <= 0 && numeroNotaFiscal <= 0 && string.IsNullOrWhiteSpace(numeroPedidoEmbarcador))
            {
                query = query.Where(o => o.Codigo == 0);
            }
            else
            {
                query = query.Where(o => o.CTe != null && !o.Carga.CargaTransbordo && o.CTe.TipoCTE == Dominio.Enumeradores.TipoCTE.Normal);

                if (codigoCarga > 0)
                    query = query.Where(o => o.Carga.Codigo == codigoCarga);

                if (situacoes != null && situacoes.Length > 0)
                    query = query.Where(o => situacoes.Contains(o.Carga.SituacaoCarga));

                if (numeroCTe > 0)
                    query = query.Where(o => o.CTe.Numero == numeroCTe);

                if (numeroNotaFiscal > 0)
                    query = query.Where(o => o.NotasFiscais.Any(nf => nf.PedidoXMLNotaFiscal.XMLNotaFiscal.Numero == numeroNotaFiscal));

                if (!string.IsNullOrWhiteSpace(numeroPedidoEmbarcador))
                    query = query.Where(o => o.Carga.Pedidos.Any(cpe => cpe.Pedido.NumeroPedidoEmbarcador == numeroPedidoEmbarcador));

                if (!string.IsNullOrWhiteSpace(propOrdenar) && !string.IsNullOrWhiteSpace(dirOrdena))
                    query = query.OrderBy($"{propOrdenar} {dirOrdena}");

                if (inicio > 0 || limite > 0)
                    query = query.Skip(inicio).Take(limite);
            }

            return query;
        }

        private IQueryable<Dominio.Entidades.Embarcador.Cargas.CargaCTe> QueryBuscarPorCargaBase(int codigoCarga, bool semComplementares = false, bool somenteEmDigitacao = false, bool somenteAutorizados = false, bool somenteModeloCTe = false, bool ctesSubContratacaoFilialEmissora = false, int numeroNF = 0, double destinatario = 0, bool buscarPorCargaOrigem = false, bool retornarPreCtes = false, int numeroDocumento = 0, int codigoCTeCancelamentoUnitario = 0, bool diferenteDeModeloCTe = false)
        {
            IQueryable<Dominio.Entidades.Embarcador.Cargas.CargaCTe> query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.CargaCTe>();

            if (buscarPorCargaOrigem)
                query = query.Where(obj => obj.CargaOrigem.Codigo == codigoCarga);
            else
                query = query.Where(obj => obj.Carga.Codigo == codigoCarga);

            if (semComplementares)
                query = query.Where(obj => obj.CargaCTeComplementoInfo == null);

            if (ctesSubContratacaoFilialEmissora)
                query = query.Where(obj => obj.CargaCTeFilialEmissora != null || (obj.CargaCTeFilialEmissora == null && obj.CargaCTeSubContratacaoFilialEmissora == null));

            if (somenteEmDigitacao)
                query = query.Where(obj => obj.CTe.Status == "S");

            if (somenteAutorizados)
            {
                if (!retornarPreCtes)
                    query = query.Where(obj => obj.CTe.Status == "A");
                else
                    query = query.Where(obj => obj.CTe.Status == "A" || obj.CTe == null);
            }

            if (somenteModeloCTe)
            {
                if (!retornarPreCtes)
                    query = query.Where(obj => obj.CTe.ModeloDocumentoFiscal.Numero == "57");
                else
                    query = query.Where(obj => obj.CTe.ModeloDocumentoFiscal.Numero == "57" || obj.CTe == null);
            }

            if (numeroNF > 0)
                query = query.Where(obj => obj.NotasFiscais.Any(doc => doc.PedidoXMLNotaFiscal.XMLNotaFiscal.Numero == numeroNF));

            if (numeroDocumento > 0)
                query = query.Where(o => o.CTe.Numero == numeroDocumento);

            if (destinatario > 0)
            {
                if (!retornarPreCtes)
                    query = query.Where(o => o.CTe.Destinatario.Cliente.CPF_CNPJ == destinatario);
                else
                    query = query.Where(o => o.CTe.Destinatario.Cliente.CPF_CNPJ == destinatario || (o.CTe == null && o.PreCTe.Destinatario.Cliente.CPF_CNPJ == destinatario));
            }

            if (codigoCTeCancelamentoUnitario > 0)
                query = query.Where(o => o.CTe.Codigo == codigoCTeCancelamentoUnitario);

            if (diferenteDeModeloCTe)
            {
                if (!retornarPreCtes)
                    query = query.Where(obj => obj.CTe.ModeloDocumentoFiscal.Numero != "57");
                else
                    query = query.Where(obj => obj.CTe.ModeloDocumentoFiscal.Numero != "57" || obj.CTe == null);
            }

            return query
                .Fetch(o => o.CTe)
                .ThenFetch(o => o.ModeloDocumentoFiscal)
                .Fetch(o => o.CTe)
                .ThenFetch(o => o.Serie);
        }

        private IQueryable<Dominio.Entidades.Embarcador.Cargas.CargaCTe> BuscarPorCodigoDeCargaENumeroOutroDoc(int carga, string numero)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.CargaCTe>();
            var result = from obj in query where obj.PreCTe.Documentos.Any(nf => nf.Numero == numero) && obj.Carga.SituacaoCarga != Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoCarga.Cancelada && obj.Carga.SituacaoCarga != Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoCarga.Anulada select obj;

            if (carga > 0)
                result = result.Where(obj => obj.Carga.Codigo == carga);

            return result;
        }

        private IQueryable<Dominio.Entidades.Embarcador.Cargas.CargaCTe> BuscarPorCodigoDeCargaEChaveNFE(int carga, string chaveNFe)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.CargaCTe>();
            var result = from obj in query where obj.PreCTe.Documentos.Any(nf => nf.ChaveNFE == chaveNFe) && obj.Carga.SituacaoCarga != Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoCarga.Cancelada && obj.Carga.SituacaoCarga != Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoCarga.Anulada select obj;

            if (carga > 0)
                result = result.Where(obj => obj.Carga.Codigo == carga);

            return result;
        }

        private IQueryable<Dominio.Entidades.Embarcador.Cargas.CargaCTe> BuscarPorCodigoDeCargaEChaveCTeSub(int carga, string chaveCTE)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.CargaCTe>();
            var result = from obj in query where obj.PreCTe.DocumentosTransporteAnterior.Any(cteant => cteant.Chave == chaveCTE) && obj.Carga.SituacaoCarga != Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoCarga.Cancelada && obj.Carga.SituacaoCarga != Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoCarga.Anulada select obj;

            if (carga > 0)
                result = result.Where(obj => obj.Carga.Codigo == carga);

            return result;
        }

        private IQueryable<Dominio.Entidades.Cliente> ConsultarTomadorCarga(bool destinatariosDaCarga, int carga, string nome, double cpfCnpj, string tipo, Dominio.Entidades.Localidade localidade, string telefone, int codigoGrupoPessoas)
        {
            var consultaCargaCTe = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.CargaCTe>();

            consultaCargaCTe = consultaCargaCTe.Where(obj => obj.CargaOrigem.Codigo == carga && obj.CTe != null);

            if (!string.IsNullOrWhiteSpace(nome))
                consultaCargaCTe = consultaCargaCTe.Where(o => o.CTe.TomadorPagador.Nome.Contains(nome));

            if (!string.IsNullOrWhiteSpace(tipo))
                consultaCargaCTe = consultaCargaCTe.Where(o => o.CTe.TomadorPagador.Tipo.Equals(tipo));

            if (localidade != null)
                consultaCargaCTe = consultaCargaCTe.Where(o => o.CTe.TomadorPagador.Localidade == localidade);

            if (!string.IsNullOrWhiteSpace(telefone))
                consultaCargaCTe = consultaCargaCTe.Where(o => o.CTe.TomadorPagador.Telefone1.Equals(telefone));

            if (cpfCnpj > 0)
                consultaCargaCTe = consultaCargaCTe.Where(o => o.CTe.TomadorPagador.Cliente.CPF_CNPJ == cpfCnpj);

            if (codigoGrupoPessoas > 0)
                consultaCargaCTe = consultaCargaCTe.Where(o => o.CTe.TomadorPagador.GrupoPessoas.Codigo == codigoGrupoPessoas || o.CTe.TomadorPagador.Cliente.GrupoPessoas.Codigo == codigoGrupoPessoas);

            var consultaCliente = this.SessionNHiBernate.Query<Dominio.Entidades.Cliente>()
                .Where(cliente => consultaCargaCTe.Any(cargaCte => cargaCte.CTe.TomadorPagador.Cliente.CPF_CNPJ == cliente.CPF_CNPJ));

            return consultaCliente;
        }

        private IQueryable<Dominio.Entidades.Cliente> ConsultarDestinatarioCarga(bool destinatariosDaCarga, int carga, string nome, double cpfCnpj, string tipo, Dominio.Entidades.Localidade localidade, string telefone, int codigoGrupoPessoas, bool buscarPorCargaOrigem)
        {
            var consultaCargaCTe = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.CargaCTe>();

            consultaCargaCTe = consultaCargaCTe.Where(obj => obj.CTe != null);

            if (buscarPorCargaOrigem)
                consultaCargaCTe = consultaCargaCTe.Where(obj => obj.CargaOrigem.Codigo == carga);
            else
                consultaCargaCTe = consultaCargaCTe.Where(obj => obj.Carga.Codigo == carga);

            if (!string.IsNullOrWhiteSpace(nome))
                consultaCargaCTe = consultaCargaCTe.Where(o => o.CTe.Destinatario.Nome.Contains(nome));

            if (!string.IsNullOrWhiteSpace(tipo))
                consultaCargaCTe = consultaCargaCTe.Where(o => o.CTe.Destinatario.Tipo.Equals(tipo));

            if (localidade != null)
                consultaCargaCTe = consultaCargaCTe.Where(o => o.CTe.Destinatario.Localidade == localidade);

            if (!string.IsNullOrWhiteSpace(telefone))
                consultaCargaCTe = consultaCargaCTe.Where(o => o.CTe.Destinatario.Telefone1.Equals(telefone));

            if (cpfCnpj > 0)
                consultaCargaCTe = consultaCargaCTe.Where(o => o.CTe.Destinatario.Cliente.CPF_CNPJ == cpfCnpj);

            if (codigoGrupoPessoas > 0)
                consultaCargaCTe = consultaCargaCTe.Where(o => o.CTe.Destinatario.GrupoPessoas.Codigo == codigoGrupoPessoas || o.CTe.Destinatario.Cliente.GrupoPessoas.Codigo == codigoGrupoPessoas);

            var consultaCliente = this.SessionNHiBernate.Query<Dominio.Entidades.Cliente>()
                .Where(cliente => consultaCargaCTe.Any(cargaCte => cargaCte.CTe.Destinatario.Cliente.CPF_CNPJ == cliente.CPF_CNPJ));

            return consultaCliente;
        }

        #endregion
    }
}