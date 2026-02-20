using Dominio.Enumeradores;
using Dominio.Excecoes.Embarcador;
using Dominio.ObjetosDeValor.Embarcador.Enumeradores;
using Repositorio;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Servicos.Embarcador.Escrituracao
{
    public class Pagamento : ServicoBase
    {
        public Pagamento(UnitOfWork unitOfWork, CancellationToken cancelationToken = default) : base(unitOfWork, cancelationToken)
        {
        }

        #region Métodos Públicos

        public static void FinalizarLotesPagamentoGeradosAutomaticamente(Repositorio.UnitOfWork unitOfWork, string stringConexao, AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware tipoServicoMultisoftware, Dominio.Entidades.Embarcador.Configuracao.ConfiguracaoTMS configuracao)
        {
            if (tipoServicoMultisoftware == AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiTMS)
                return;

            if (configuracao.AutomatizarGeracaoLotePagamento)
            {
                Repositorio.Embarcador.Escrituracao.Pagamento repPagamento = new Repositorio.Embarcador.Escrituracao.Pagamento(unitOfWork);


                List<Dominio.Entidades.Embarcador.Escrituracao.Pagamento> pagamentos = repPagamento.BuscarPagamentosAutomaticosAguardandoFechamento(5);
                foreach (Dominio.Entidades.Embarcador.Escrituracao.Pagamento pagamento in pagamentos)
                {
                    unitOfWork.Start();
                    FinalizarPagamento(pagamento, unitOfWork);
                    unitOfWork.CommitChanges();
                }
            }
        }

        public static void FinalizarPagamento(Dominio.Entidades.Embarcador.Escrituracao.Pagamento pagamento, Repositorio.UnitOfWork unitOfWork)
        {
            Repositorio.Embarcador.ConfiguracaoContabil.DocumentoContabil repDocumentoContabil = new Repositorio.Embarcador.ConfiguracaoContabil.DocumentoContabil(unitOfWork);
            Repositorio.Embarcador.Financeiro.DocumentoFaturamento repDocumentoFaturamento = new Repositorio.Embarcador.Financeiro.DocumentoFaturamento(unitOfWork);
            Repositorio.Embarcador.Escrituracao.DocumentoProvisao repDocumentoProvisao = new Repositorio.Embarcador.Escrituracao.DocumentoProvisao(unitOfWork);
            Repositorio.Embarcador.Financeiro.Titulo repTitulo = new Repositorio.Embarcador.Financeiro.Titulo(unitOfWork);
            Repositorio.Embarcador.Configuracoes.ConfiguracaoFatura repConfiguracaoFatura = new Repositorio.Embarcador.Configuracoes.ConfiguracaoFatura(unitOfWork);
            Repositorio.Embarcador.Pessoas.GrupoPessoas repositorioGrupoPessoas = new Repositorio.Embarcador.Pessoas.GrupoPessoas(unitOfWork);

            if (!pagamento.LotePagamentoLiberado)
            {
                repDocumentoContabil.ConfirmarMovimentoPorPagamento(pagamento.Codigo);
                repDocumentoFaturamento.ConfirmarPagamentoDocumentos(pagamento.Codigo);

                Dominio.Entidades.Embarcador.Configuracoes.ConfiguracaoFatura configuracaoFatura = repConfiguracaoFatura.BuscarConfiguracaoPadrao();
                bool disponbilizarProvisaoContraPartidaParaCancelamento = configuracaoFatura?.DisponbilizarProvisaoContraPartidaParaCancelamento ?? false;

                Servicos.Log.TratarErro($"Liquidando provisões do pagamento {pagamento.Codigo}", "FechamentoPagamento");
                repDocumentoProvisao.LiquidarDocumentosProvisaoPorPagamento(pagamento.Codigo, disponbilizarProvisaoContraPartidaParaCancelamento);

                if (disponbilizarProvisaoContraPartidaParaCancelamento)
                    repDocumentoProvisao.DisponibilizarDocumentosParaCancelamentoContraPartida(pagamento.Codigo);

                repTitulo.LiberarTitulosPagamento(pagamento.Codigo);

                Dominio.Entidades.Embarcador.Pessoas.GrupoPessoas existeGrupoPessoa = repositorioGrupoPessoas.BuscarPorCodigo(pagamento.Tomador?.GrupoPessoas?.Codigo ?? 0);

                if ((pagamento.Tomador?.QuitarDocumentoAutomaticamenteAoGerarLote ?? false) || (existeGrupoPessoa != null && (existeGrupoPessoa?.QuitarDocumentoAutomaticamenteAoGerarLote ?? false))
                    || (pagamento.Carga?.TipoOperacao?.QuitarDocumentoAutomaticamenteAoGerarLote ?? false))
                    repTitulo.QuitarTitulosPagamento(pagamento.Codigo, pagamento.DataCriacao);
            }

            Servicos.Embarcador.Escrituracao.Pagamento.EfetuarIntegracaoPagamento(pagamento, unitOfWork);
        }

        public static void GerarLotePagamento(Repositorio.UnitOfWork unidadeTrabalho, string stringConexao, AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware tipoServicoMultisoftware, Dominio.Entidades.Embarcador.Configuracao.ConfiguracaoTMS configuracao, bool liberarPagamentosDesbloqueados, Dominio.Entidades.Embarcador.Configuracoes.ConfiguracaoFinanceiro configuracaoFinanceiro, Dominio.ObjetosDeValor.Embarcador.Auditoria.Auditado auditado)
        {
            if (tipoServicoMultisoftware == AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiTMS)
                return;

            if (!configuracao.AutomatizarGeracaoLotePagamento)
                return;

            bool existeTipoIntegracaoQueNaoGeraPagamentoAutomaticoParaDocumentoBloqueado = ExisteTipoIntegracaoQueNaoGeraPagamentoAutomaticoParaDocumentoBloqueado(unidadeTrabalho);

            if (liberarPagamentosDesbloqueados && existeTipoIntegracaoQueNaoGeraPagamentoAutomaticoParaDocumentoBloqueado)
                return;

            bool podeGerar = false;
            int diasRetroativos = -2;

            if (DateTime.Now.Hour > 0 || DateTime.Now.Minute >= 10)
                podeGerar = true;

            if (DateTime.Now.Date.Day == 1 && DateTime.Now.Hour >= 3)
                diasRetroativos = -1;

            if (liberarPagamentosDesbloqueados)
                diasRetroativos = -1;

            if (configuracaoFinanceiro.GerarLotesPagamentoIndividuaisPorDocumento)
                diasRetroativos = 0;

            if (configuracaoFinanceiro.GerarLotesAposEmissaoDaCarga)
            {
                podeGerar = true;
                diasRetroativos = 0;
            }

            bool consultarPorDataEmissao = false;

            if (configuracaoFinanceiro.MinutosAguardarGeracaoLotePagamento.HasValue && configuracaoFinanceiro.MinutosAguardarGeracaoLotePagamentoUltimoDiaMes != null)
            {
                podeGerar = true;
                consultarPorDataEmissao = true;
            }

            if (podeGerar)
            {
                Repositorio.Embarcador.Financeiro.DocumentoFaturamento repDocumentoFaturamento = new Repositorio.Embarcador.Financeiro.DocumentoFaturamento(unidadeTrabalho);
                Repositorio.Embarcador.Pedidos.XMLNotaFiscal repositorioXmlNotaFiscal = new Repositorio.Embarcador.Pedidos.XMLNotaFiscal(unidadeTrabalho);
                Repositorio.Embarcador.Configuracoes.ConfiguracaoCanhoto repositorioCanhoto = new Repositorio.Embarcador.Configuracoes.ConfiguracaoCanhoto(unidadeTrabalho);
                Repositorio.ModeloDocumentoFiscal repositorioModeloDocumentoFiscal = new Repositorio.ModeloDocumentoFiscal(unidadeTrabalho);

                Dominio.Entidades.Embarcador.Configuracoes.ConfiguracaoCanhoto configuracaoCanhoto = repositorioCanhoto.BuscarConfiguracaoPadrao();

                DateTime dataFinal;
                bool considerarHorasRetroativas = false;

                if (configuracaoFinanceiro.MinutosAguardarGeracaoLotePagamento.HasValue && configuracaoFinanceiro.MinutosAguardarGeracaoLotePagamentoUltimoDiaMes != null)
                {
                    int ultimoDiaMes = DateTime.DaysInMonth(DateTime.Now.Year, DateTime.Now.Month);
                    double minutosRetroativos = configuracaoFinanceiro.MinutosAguardarGeracaoLotePagamento.Value;
                    dataFinal = DateTime.Now.AddMinutes(-minutosRetroativos);

                    if (DateTime.Now.Day == ultimoDiaMes)
                    {
                        minutosRetroativos = configuracaoFinanceiro.MinutosAguardarGeracaoLotePagamentoUltimoDiaMes.Value;
                        dataFinal = DateTime.Now.AddMinutes(-minutosRetroativos);
                    }

                    considerarHorasRetroativas = true;
                }
                else
                    dataFinal = DateTime.Now.Date.AddDays(diasRetroativos);

                Dominio.ObjetosDeValor.Embarcador.Escrituracao.FiltroPesquisaDocumento filtroPesquisa = new Dominio.ObjetosDeValor.Embarcador.Escrituracao.FiltroPesquisaDocumento()
                {
                    DataFim = dataFinal,
                    ConsiderarHorasRetroativas = considerarHorasRetroativas,
                    SituacaoPagamentoDocumento = SituacaoDocumentoFaturamento.Autorizado,
                    PagamentoFinalizados = false,
                    PagamentoLiberado = liberarPagamentosDesbloqueados,
                    SomenteComDocumentosDesbloqueados = !liberarPagamentosDesbloqueados,
                    NaoGerarAutomaticamenteLotesCancelados = configuracaoFinanceiro.NaoGerarAutomaticamenteLotesCancelados,
                    ConsultarPorDataEmissao = consultarPorDataEmissao,
                    ModeloDocumentoFiscal = configuracaoFinanceiro.GerarLotePagamentoSomenteParaCTe ? (repositorioModeloDocumentoFiscal.BuscarPorModelo("57")?.Codigo ?? 0) : 0
                };

                //if (configuracaoFinanceiro.GerarLotesPagamentoIndividuaisPorDocumento)
                //{
                //    filtroPesquisa.DataContabilizacaoFrete = DateTime.Now.AddHours(DateTime.Now.IsLastDayOfMonth() ? -6 : -12);
                //    filtroPesquisa.DataContabilizacaoComplemento = DateTime.Now.AddHours(-2);
                //}

                IList<int> codigosDocumentosComCanhotoDigitalizado = new List<int>();

                if (configuracaoFinanceiro.GerarLotePagamentoAposDigitalizacaoDoCanhoto)
                {
                    codigosDocumentosComCanhotoDigitalizado = repDocumentoFaturamento.ObterCodigosDocumentosFaturamentoSemPagamentoComCanhotoDigitalizado();
                    codigosDocumentosComCanhotoDigitalizado = codigosDocumentosComCanhotoDigitalizado.OrderByDescending(codigo => codigo).Take(2000).ToList();
                }

                filtroPesquisa.SomenteComProvisaoGerada = ObterConfiguracaoGerarLotePagamentoSomenteComProvisaoGerada(unidadeTrabalho);

                int totalDocumentos = repDocumentoFaturamento.ContarConsulta(null, 0, filtroPesquisa, true, true, (configuracao.GerarSomenteDocumentosDesbloqueados || existeTipoIntegracaoQueNaoGeraPagamentoAutomaticoParaDocumentoBloqueado), codigosDocumentosComCanhotoDigitalizado);
                if (totalDocumentos > 0)
                {
                    Dominio.ObjetosDeValor.Embarcador.Consulta.ParametroConsulta parametroConsulta = new Dominio.ObjetosDeValor.Embarcador.Consulta.ParametroConsulta();

                    if ((configuracaoFinanceiro.QuantidadeDocumentosLotePagamentoGeradoAutomatico ?? 0) > 0)
                        parametroConsulta.LimiteRegistros = configuracaoFinanceiro.QuantidadeDocumentosLotePagamentoGeradoAutomatico ?? 100;

                    List<Dominio.Entidades.Embarcador.Financeiro.DocumentoFaturamento> documentosFaturamento = repDocumentoFaturamento.Consultar(null, 0, filtroPesquisa, true, true, (configuracao.GerarSomenteDocumentosDesbloqueados || existeTipoIntegracaoQueNaoGeraPagamentoAutomaticoParaDocumentoBloqueado), parametroConsulta, codigosDocumentosComCanhotoDigitalizado);

                    if ((configuracaoFinanceiro.QuantidadeMinimaDocumentosLotePagamentoGeradoAutomatico ?? 0) > documentosFaturamento.Count)
                        return;

                    //Unilever vai gerar lotes de pagamento por carga
                    if (configuracaoCanhoto.LiberarParaPagamentoAposDigitalizacaCanhoto)
                    {
                        List<int> cargas = documentosFaturamento.Select(x => x.CargaPagamento.Codigo).Distinct().ToList();
                        List<(int codigoCarga, int quantidadeDocumento)> cargasComDocumentos = new List<(int codigoCarga, int quantidadeDocumento)>();

                        foreach (var carga in cargas)
                            cargasComDocumentos.Add(ValueTuple.Create(carga, repositorioXmlNotaFiscal.BuscarQuantidadesRelevantesParaFretePorCarga(carga)));

                        var documentosProcessar = new Servicos.Embarcador.Financeiro.Pagamento(unidadeTrabalho).ObterDocumentosPagarGeracaoAutomaticaDoPagamento(documentosFaturamento);

                        if (documentosProcessar.Count == 0)
                            return;

                        foreach (var carga in cargas)
                        {
                            var documentosLiberados = documentosProcessar.Where(x => x.CargaPagamento.Codigo == carga).ToList();
                            var registroCargaDocumentos = cargasComDocumentos.Where(x => x.codigoCarga == carga).FirstOrDefault();

                            if (registroCargaDocumentos.quantidadeDocumento != documentosLiberados.Count || documentosLiberados.Count == 0)
                                continue;

                            AdicionarPagamentoPorCarga(documentosLiberados, documentosLiberados.FirstOrDefault().CargaPagamento, liberarPagamentosDesbloqueados, unidadeTrabalho);
                        }
                        return;
                    }

                    List<Dominio.Entidades.Cliente> tomadores = (from obj in documentosFaturamento select obj.Tomador).Distinct().ToList();
                    List<int> codigosDocumentoPagamento = new List<int>();

                    foreach (Dominio.Entidades.Cliente tomador in tomadores)
                    {
                        List<Dominio.Entidades.Embarcador.Financeiro.DocumentoFaturamento> documentosFaturamentoTomador = (from obj in documentosFaturamento where obj.Tomador.CPF_CNPJ == tomador.CPF_CNPJ select obj).ToList();
                        List<Dominio.Entidades.Empresa> empresas = (from obj in documentosFaturamentoTomador select obj.Empresa).Distinct().ToList();
                        foreach (Dominio.Entidades.Empresa empresa in empresas)
                        {
                            List<Dominio.Entidades.Embarcador.Financeiro.DocumentoFaturamento> documentos = (from obj in documentosFaturamentoTomador where obj.Empresa.Codigo == empresa.Codigo select obj).ToList();

                            if (configuracaoFinanceiro.GerarLotesPagamentoIndividuaisPorDocumento)
                            {
                                foreach (Dominio.Entidades.Embarcador.Financeiro.DocumentoFaturamento documentoFaturamento in documentos)
                                    AdicionarPagamentoPorDocumento(documentoFaturamento, dataFinal, tomador, empresa, codigosDocumentoPagamento, liberarPagamentosDesbloqueados, unidadeTrabalho, auditado);
                            }
                            else
                                AdicionarPagamentoPorDocumentos(documentos, dataFinal, tomador, empresa, codigosDocumentoPagamento, liberarPagamentosDesbloqueados, unidadeTrabalho, auditado);
                        }
                    }
                }
            }
        }

        private static bool ObterConfiguracaoGerarLotePagamentoSomenteComProvisaoGerada(Repositorio.UnitOfWork unitOfWork)
        {
            Repositorio.Embarcador.Cargas.TipoIntegracao repTipoIntegracao = new Repositorio.Embarcador.Cargas.TipoIntegracao(unitOfWork);

            List<Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoIntegracao> tiposIntegracaoLiberar = new List<Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoIntegracao>() {
                Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoIntegracao.Camil
            };

            List<Dominio.Entidades.Embarcador.Cargas.TipoIntegracao> tiposIntegracao = repTipoIntegracao.BuscarPorTipos(tiposIntegracaoLiberar);

            if (tiposIntegracao.Count == 0)
                return false;

            return true;
        }

        public static Dominio.Entidades.Embarcador.Escrituracao.Pagamento AdicionarPagamento(DateTime dataInicio, DateTime dataFim, DateTime dataInicioEmissao, DateTime dataFimEmissao, Dominio.Entidades.Cliente tomador, Dominio.Entidades.Embarcador.Filiais.Filial filial, Dominio.Entidades.Empresa empresa, Dominio.Entidades.Embarcador.Cargas.Carga carga, Dominio.Entidades.Embarcador.Ocorrencias.CargaOcorrencia ocorrencia, bool geradoAutomaticamente, bool pagamentoLiberado, Repositorio.UnitOfWork unitOfWork, Dominio.ObjetosDeValor.Embarcador.Auditoria.Auditado auditado)
        {
            Repositorio.Embarcador.Escrituracao.Pagamento repPagamento = new Repositorio.Embarcador.Escrituracao.Pagamento(unitOfWork);

            Dominio.Entidades.Embarcador.Escrituracao.Pagamento pagamento = new Dominio.Entidades.Embarcador.Escrituracao.Pagamento();

            if (dataInicio != DateTime.MinValue)
                pagamento.DataInicial = dataInicio;

            if (dataFim != DateTime.MinValue)
                pagamento.DataFinal = dataFim;


            if (dataInicioEmissao != DateTime.MinValue)
                pagamento.DataInicialEmissao = dataInicioEmissao;

            if (dataFimEmissao != DateTime.MinValue)
                pagamento.DataFinalEmissao = dataFimEmissao;

            pagamento.DataCriacao = DateTime.Now;
            pagamento.Numero = repPagamento.ObterProximoNumero();
            pagamento.Tomador = tomador;
            pagamento.Filial = filial;
            pagamento.Empresa = empresa;
            pagamento.Carga = carga;
            pagamento.LotePagamentoLiberado = pagamentoLiberado;
            pagamento.CargaOcorrencia = ocorrencia;
            pagamento.GeradoAutomaticamente = geradoAutomaticamente;
            pagamento.Situacao = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoPagamento.EmFechamento;
            repPagamento.Inserir(pagamento, auditado);
            return pagamento;
        }

        public static void EfetuarIntegracaoPagamento(Dominio.Entidades.Embarcador.Escrituracao.Pagamento pagamento, Repositorio.UnitOfWork unitOfWork)
        {
            Repositorio.Embarcador.Escrituracao.Pagamento repPagamento = new Repositorio.Embarcador.Escrituracao.Pagamento(unitOfWork);
            Repositorio.Embarcador.Financeiro.DocumentoFaturamento repDocumentoFaturamento = new Repositorio.Embarcador.Financeiro.DocumentoFaturamento(unitOfWork);
            bool agIntegracao = false;

            if (EfetuarIntegracaoEDIPagamentoINTPFAR(pagamento, unitOfWork))
                agIntegracao = true;

            if (EfetuarIntegracaoEDIPagamentoPorTipoOperacao(pagamento, Dominio.Enumeradores.TipoLayoutEDI.ImportsysCTe, unitOfWork))
                agIntegracao = true;

            if (PagamentoTemTipoLayoutEDINoTipoOperacao(pagamento, Dominio.Enumeradores.TipoLayoutEDI.ImportsysVP, unitOfWork) && PagamentoTemValePedagio(pagamento, unitOfWork))
            {
                if (EfetuarIntegracaoEDIPagamentoPorTipoOperacao(pagamento, Dominio.Enumeradores.TipoLayoutEDI.ImportsysVP, unitOfWork))
                    agIntegracao = true;
            }

            if (EfetuarIntegracaoEspecificasPagamento(pagamento, unitOfWork))
                agIntegracao = true;

            if (agIntegracao)
                pagamento.Situacao = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoPagamento.AguardandoIntegracao;
            else
            {
                pagamento.Situacao = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoPagamento.Finalizado;
                repDocumentoFaturamento.LiberarPagamentosAutomaticamentePorPagamento(pagamento.Codigo);
            }

            repPagamento.Atualizar(pagamento);
        }

        public static bool EfetuarIntegracaoEspecificasPagamento(Dominio.Entidades.Embarcador.Escrituracao.Pagamento pagamento, Repositorio.UnitOfWork unitOfWork)
        {
            //todo: se precisar gerar uma integração que não for EDI deve fazer aqui.
            bool possuiIntegracao = false;

            Repositorio.Embarcador.Cargas.TipoIntegracao repTipoIntegracao = new Repositorio.Embarcador.Cargas.TipoIntegracao(unitOfWork);
            Repositorio.Embarcador.Pedidos.XMLNotaFiscal repositorioXmlNotaFiscal = new Repositorio.Embarcador.Pedidos.XMLNotaFiscal(unitOfWork);
            Repositorio.Embarcador.Cargas.CargaPedido repCargaPedido = new Repositorio.Embarcador.Cargas.CargaPedido(unitOfWork);
            Repositorio.Embarcador.Configuracao.ConfiguracaoTMS repConfiguracaoTMS = new Repositorio.Embarcador.Configuracao.ConfiguracaoTMS(unitOfWork);

            Dominio.Entidades.Embarcador.Configuracao.ConfiguracaoTMS configuracaoEmbarcador = repConfiguracaoTMS.BuscarConfiguracaoPadrao();

            List<Dominio.ObjetosDeValor.Embarcador.Carga.CargaPedido> cargaPedidos = repCargaPedido.BuscarCodigosCargaPedidoPorCarga(pagamento.Carga?.Codigo ?? 0, configuracaoEmbarcador.NaoUsarPesoNotasPallet);
            List<Dominio.Entidades.Embarcador.Cargas.TipoIntegracao> tiposIntegracao = repTipoIntegracao.BuscarPorTipos(null, null);

            foreach (Dominio.Entidades.Embarcador.Cargas.TipoIntegracao tipoIntegracao in tiposIntegracao)
            {
                switch (tipoIntegracao.Tipo)
                {
                    case Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoIntegracao.Digibee:
                        Repositorio.Embarcador.Configuracoes.Integracao repIntegracao = new Repositorio.Embarcador.Configuracoes.Integracao(unitOfWork);
                        Dominio.Entidades.Embarcador.Configuracoes.Integracao integracao = repIntegracao.Buscar();
                        if (!string.IsNullOrWhiteSpace(integracao?.URLIntegracaoDadosContabeisCTeDigibee))
                            AdicionarPagamentoParaIntegracao(pagamento, tipoIntegracao, unitOfWork);
                        possuiIntegracao = true;
                        break;
                    case Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoIntegracao.Ultragaz:
                        if (pagamento.Carga?.TipoOperacao?.ConfiguracaoCarga?.PrecisaEsperarNotasFilhaParaGerarPagamento ?? false)
                        {
                            bool recebeuNotasFilhas = true;

                            List<string> numerosControle = cargaPedidos.Select(cp => cp.NumeroControlePedido).Distinct().ToList();
                            List<Dominio.Entidades.Embarcador.Pedidos.XMLNotaFiscal> notasCarga = repositorioXmlNotaFiscal.BuscarPorCarga(pagamento.Carga?.Codigo ?? 0);

                            if (!numerosControle.Any(numero => notasCarga.Any(nc => nc.NumeroControlePedido == numero)))
                                recebeuNotasFilhas = false;

                            if (!recebeuNotasFilhas)
                                possuiIntegracao = false;
                            else
                            {
                                AdicionarDocumentosPagamentoParaIntegracao(pagamento, tipoIntegracao, unitOfWork);
                                possuiIntegracao = true;
                            }
                        }
                        else
                        {
                            AdicionarDocumentosPagamentoParaIntegracao(pagamento, tipoIntegracao, unitOfWork);
                            possuiIntegracao = true;
                        }
                        break;
                    case Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoIntegracao.YPE:
                    case Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoIntegracao.Unilever:
                    case Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoIntegracao.EFretePagamento:
                    case Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoIntegracao.Camil:
                        AdicionarDocumentosPagamentoParaIntegracao(pagamento, tipoIntegracao, unitOfWork);
                        possuiIntegracao = true;
                        break;
                    case Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoIntegracao.GrupoSC:
                        AdicionarTipoDocumentoPagamentoParaIntegracao(pagamento, tipoIntegracao, unitOfWork);
                        possuiIntegracao = true;
                        break;
                    case Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoIntegracao.JDEFaturas:
                        AdicionarPagamentoParaIntegracao(pagamento, tipoIntegracao, unitOfWork);
                        possuiIntegracao = true;
                        break;
                    default:
                        break;
                }
            }

            return possuiIntegracao;
        }

        public static void AdicionarPagamentoParaIntegracao(Dominio.Entidades.Embarcador.Escrituracao.Pagamento pagamento, Dominio.Entidades.Embarcador.Cargas.TipoIntegracao tipoIntegracao, Repositorio.UnitOfWork unitOfWork)
        {
            Repositorio.Embarcador.Escrituracao.PagamentoIntegracao repPagamentoIntegracao = new Repositorio.Embarcador.Escrituracao.PagamentoIntegracao(unitOfWork);

            Dominio.Entidades.Embarcador.Escrituracao.PagamentoIntegracao pagamentoIntegracao = repPagamentoIntegracao.BuscarPorPagamentoETipoIntegracao(pagamento.Codigo, tipoIntegracao.Tipo);

            if (pagamentoIntegracao == null)
            {
                pagamentoIntegracao = new Dominio.Entidades.Embarcador.Escrituracao.PagamentoIntegracao();
                pagamentoIntegracao.Pagamento = pagamento;
                pagamentoIntegracao.DataIntegracao = DateTime.Now;
                pagamentoIntegracao.NumeroTentativas = 0;
                pagamentoIntegracao.ProblemaIntegracao = "";
                pagamentoIntegracao.TipoIntegracao = tipoIntegracao;
                pagamentoIntegracao.SituacaoIntegracao = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoIntegracao.AgIntegracao;

                repPagamentoIntegracao.Inserir(pagamentoIntegracao);
            }
        }

        public static void AdicionarDocumentosPagamentoParaIntegracao(Dominio.Entidades.Embarcador.Escrituracao.Pagamento pagamento, Dominio.Entidades.Embarcador.Cargas.TipoIntegracao tipoIntegracao, Repositorio.UnitOfWork unitOfWork)
        {
            Repositorio.Embarcador.Escrituracao.PagamentoIntegracao repPagamentoIntegracao = new Repositorio.Embarcador.Escrituracao.PagamentoIntegracao(unitOfWork);
            Repositorio.Embarcador.Financeiro.DocumentoFaturamento repDocumentoFaturamento = new Repositorio.Embarcador.Financeiro.DocumentoFaturamento(unitOfWork);

            List<Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoIntegracao> tiposIntegracaoPermitirDuplicar = new List<Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoIntegracao>()
            {
                Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoIntegracao.Camil
            };

            List<Dominio.Entidades.Embarcador.Financeiro.DocumentoFaturamento> documentosFaturamento = repDocumentoFaturamento.BuscarPorPagamento(pagamento.Codigo, false);
            foreach (Dominio.Entidades.Embarcador.Financeiro.DocumentoFaturamento documentoFaturamento in documentosFaturamento)
            {
                Dominio.Entidades.Embarcador.Escrituracao.PagamentoIntegracao pagamentoIntegracao = repPagamentoIntegracao.BuscarPorDocumentoFaturamentoETipoIntegracao(documentoFaturamento.Codigo, tipoIntegracao.Tipo);

                if (pagamentoIntegracao == null || tiposIntegracaoPermitirDuplicar.Contains(tipoIntegracao.Tipo))
                {
                    pagamentoIntegracao = new Dominio.Entidades.Embarcador.Escrituracao.PagamentoIntegracao();
                    pagamentoIntegracao.Pagamento = pagamento;
                    pagamentoIntegracao.DocumentoFaturamento = documentoFaturamento;
                    pagamentoIntegracao.DataIntegracao = DateTime.Now;
                    pagamentoIntegracao.NumeroTentativas = 0;
                    pagamentoIntegracao.ProblemaIntegracao = "";
                    pagamentoIntegracao.TipoIntegracao = tipoIntegracao;
                    pagamentoIntegracao.SituacaoIntegracao = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoIntegracao.AgIntegracao;
                    repPagamentoIntegracao.Inserir(pagamentoIntegracao);
                }
            }
        }

        public static void AdicionarTipoDocumentoPagamentoParaIntegracao(Dominio.Entidades.Embarcador.Escrituracao.Pagamento pagamento, Dominio.Entidades.Embarcador.Cargas.TipoIntegracao tipoIntegracao, Repositorio.UnitOfWork unitOfWork)
        {
            Repositorio.Embarcador.Escrituracao.PagamentoIntegracao repPagamentoIntegracao = new Repositorio.Embarcador.Escrituracao.PagamentoIntegracao(unitOfWork);
            Repositorio.Embarcador.Financeiro.DocumentoFaturamento repDocumentoFaturamento = new Repositorio.Embarcador.Financeiro.DocumentoFaturamento(unitOfWork);

            List<Dominio.Enumeradores.TipoDocumento> tiposDeDocumento = repDocumentoFaturamento.BuscarPorTipoDocumento(pagamento.Codigo, false);

            foreach (Dominio.Enumeradores.TipoDocumento tipoDocumento in tiposDeDocumento)
            {
                if (!repPagamentoIntegracao.ExistePorPagamentoETipoIntegracaoETipoDocumento(pagamento.Codigo, tipoIntegracao.Tipo, tipoDocumento))
                {
                    Dominio.Entidades.Embarcador.Escrituracao.PagamentoIntegracao pagamentoIntegracao = new Dominio.Entidades.Embarcador.Escrituracao.PagamentoIntegracao();
                    pagamentoIntegracao.Pagamento = pagamento;
                    pagamentoIntegracao.DataIntegracao = DateTime.Now;
                    pagamentoIntegracao.NumeroTentativas = 0;
                    pagamentoIntegracao.ProblemaIntegracao = "";
                    pagamentoIntegracao.TipoIntegracao = tipoIntegracao;
                    pagamentoIntegracao.SituacaoIntegracao = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoIntegracao.AgIntegracao;
                    pagamentoIntegracao.TipoDocumento = tipoDocumento;
                    repPagamentoIntegracao.Inserir(pagamentoIntegracao);
                }
            }
        }

        public static List<Dominio.Entidades.Embarcador.Transportadores.TransportadorLayoutEDI> LayoutEDIPagamentoTransportador(Dominio.Entidades.Embarcador.Escrituracao.Pagamento pagamento, Repositorio.UnitOfWork unitOfWork)
        {
            Repositorio.Embarcador.Financeiro.DocumentoFaturamento repDocumentoFaturamento = new Repositorio.Embarcador.Financeiro.DocumentoFaturamento(unitOfWork);

            List<Dominio.Entidades.Empresa> empresas = repDocumentoFaturamento.BuscarTransportadoresPagamento(pagamento.Codigo, pagamento.LotePagamentoLiberado);
            List<Dominio.Entidades.Embarcador.Transportadores.TransportadorLayoutEDI> layouts = new List<Dominio.Entidades.Embarcador.Transportadores.TransportadorLayoutEDI>();

            for (int i = 0, s = empresas.Count(); i < s; i++)
                if (empresas[i].TransportadorLayoutsEDI != null)
                    layouts.AddRange(empresas[i].TransportadorLayoutsEDI.ToList());

            return layouts;
        }

        public async Task<List<Dominio.Entidades.Embarcador.Pessoas.GrupoPessoasLayoutEDI>> LayoutEDIPagamentoAsync(Dominio.Entidades.Embarcador.Escrituracao.Pagamento pagamento)
        {
            Repositorio.Embarcador.Financeiro.DocumentoFaturamento repositorioDocumentoFaturamento = new Repositorio.Embarcador.Financeiro.DocumentoFaturamento(_unitOfWork, _cancellationToken);

            List<Dominio.Entidades.Cliente> tomadores = await repositorioDocumentoFaturamento.BuscarTomadoresPagamentoAsync(pagamento.Codigo, pagamento.LotePagamentoLiberado);

            List<Dominio.Entidades.Embarcador.Pessoas.GrupoPessoas> grupoPessoas = (from tomador in tomadores where tomador.GrupoPessoas != null select tomador.GrupoPessoas).Distinct().ToList();
            List<Dominio.Entidades.Embarcador.Pessoas.GrupoPessoasLayoutEDI> layouts = new List<Dominio.Entidades.Embarcador.Pessoas.GrupoPessoasLayoutEDI>();

            for (int i = 0, s = grupoPessoas.Count; i < s; i++)
                if (grupoPessoas[i].LayoutsEDI != null)
                    layouts.AddRange(grupoPessoas[i].LayoutsEDI.ToList());

            return layouts;
        }

        public async Task<List<Dominio.Entidades.Embarcador.Pessoas.ClienteLayoutEDI>> LayoutEDIPagamentoClienteAsync(Dominio.Entidades.Embarcador.Escrituracao.Pagamento pagamento)
        {
            Repositorio.Embarcador.Financeiro.DocumentoFaturamento repositorioDocumentoFaturamento = new Repositorio.Embarcador.Financeiro.DocumentoFaturamento(_unitOfWork);

            List<Dominio.Entidades.Cliente> tomadores = await repositorioDocumentoFaturamento.BuscarTomadoresPagamentoAsync(pagamento.Codigo, pagamento.LotePagamentoLiberado);

            List<Dominio.Entidades.Embarcador.Pessoas.ClienteLayoutEDI> layouts = new List<Dominio.Entidades.Embarcador.Pessoas.ClienteLayoutEDI>();

            for (int i = 0, s = tomadores.Count; i < s; i++)
                if (tomadores[i].LayoutsEDI != null)
                    layouts.AddRange(tomadores[i].LayoutsEDI.ToList());

            return layouts;
        }

        public static List<Dominio.Entidades.Embarcador.Pedidos.TipoOperacaoLayoutEDI> ObterLayoutEDIsPagamentoTipoOperacao(Dominio.Entidades.Embarcador.Escrituracao.Pagamento pagamento, Repositorio.UnitOfWork unitOfWork)
        {
            List<Dominio.Entidades.Embarcador.Pedidos.TipoOperacaoLayoutEDI> layouts = new List<Dominio.Entidades.Embarcador.Pedidos.TipoOperacaoLayoutEDI>();

            List<Dominio.Entidades.Embarcador.Pedidos.TipoOperacao> tiposOperacao = (from obj in pagamento.DocumentosFaturamento where obj.TipoOperacao != null select obj.TipoOperacao).Distinct().ToList();

            foreach (Dominio.Entidades.Embarcador.Pedidos.TipoOperacao tipoOperacao in tiposOperacao)
            {
                layouts.AddRange(tipoOperacao.LayoutsEDI.ToList());
            }

            return layouts;
        }

        public static void ProcessarPagamentoEmFechamento(Repositorio.UnitOfWork unidadeTrabalho, string stringConexao, AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware tipoServicoMultisoftware, Dominio.Entidades.Embarcador.Configuracao.ConfiguracaoTMS configuracao, string urlAcesso)
        {
            try
            {
                Repositorio.Embarcador.Escrituracao.Pagamento repPagamento = new Repositorio.Embarcador.Escrituracao.Pagamento(unidadeTrabalho);
                List<Dominio.Entidades.Embarcador.Escrituracao.Pagamento> pagamentos = repPagamento.BuscarPagamentoEmFechamento(5);
                foreach (Dominio.Entidades.Embarcador.Escrituracao.Pagamento pagamento in pagamentos)
                {
                    Servicos.Log.TratarErro($"Processando fechamento do pagamento {pagamento.Codigo}", "FechamentoPagamento");
                    GerarFechamentoDocumentosPagamento(pagamento, unidadeTrabalho, stringConexao, tipoServicoMultisoftware, configuracao, urlAcesso);
                }

            }
            catch (Exception ex)
            {
                unidadeTrabalho.Rollback();
                Servicos.Log.TratarErro(ex);
            }
        }

        public void BloquearDocumentoComRestricao(Dominio.Entidades.ConhecimentoDeTransporteEletronico cte, Dominio.Entidades.Embarcador.Escrituracao.Pagamento pagamento, string motivoBloqueio, Repositorio.UnitOfWork unitOfWork)
        {
            Repositorio.Embarcador.Financeiro.DocumentoFaturamento repDocumentoFaturamento = new Repositorio.Embarcador.Financeiro.DocumentoFaturamento(unitOfWork);
            int codigoDocumentoFaturamento = repDocumentoFaturamento.BuscarCodigoDocumentoFaturamentoPorCTeEPagamento(pagamento.Codigo, cte.Codigo);

            GerarBloqueioDocumentoFaturamento(repDocumentoFaturamento, motivoBloqueio, codigoDocumentoFaturamento, unitOfWork);
        }

        public void BloquearDocumentoComRestricao(int codigoCarga, Dominio.Entidades.Embarcador.Escrituracao.Pagamento pagamento, string motivoBloqueio, Repositorio.UnitOfWork unitOfWork)
        {
            Repositorio.Embarcador.Financeiro.DocumentoFaturamento repDocumentoFaturamento = new Repositorio.Embarcador.Financeiro.DocumentoFaturamento(unitOfWork);
            int codigoDocumentoFaturamento = repDocumentoFaturamento.BuscarCodigoDocumentoPorPagamentoECargaAgNotaFilha(pagamento.Codigo, codigoCarga);

            GerarBloqueioDocumentoFaturamento(repDocumentoFaturamento, motivoBloqueio, codigoDocumentoFaturamento, unitOfWork);
        }

        public void LiberarPagamentoEstornandoProvisao(Dominio.ObjetosDeValor.WebService.Rest.Escrituracao.LiberacaoPagamentoRequest request, Dominio.Entidades.Embarcador.Escrituracao.PagamentoIntegracao pagamentoIntegracao, Repositorio.UnitOfWork unitOfWork)
        {
            Repositorio.Embarcador.Escrituracao.PagamentoIntegracao repositorioPagamentoIntegracao = new Repositorio.Embarcador.Escrituracao.PagamentoIntegracao(unitOfWork);
            Repositorio.Embarcador.Escrituracao.Pagamento repositorioPagamento = new Repositorio.Embarcador.Escrituracao.Pagamento(unitOfWork);
            Servicos.Embarcador.Escrituracao.Provisao servicoProvisao = new Servicos.Embarcador.Escrituracao.Provisao(unitOfWork);

            if (pagamentoIntegracao == null)
                throw new Dominio.Excecoes.Embarcador.ServicoException("Registro não encontrado");

            if (pagamentoIntegracao.SituacaoIntegracao != SituacaoIntegracao.AgRetorno)
                throw new Dominio.Excecoes.Embarcador.ServicoException("Registro não está aguardando retorno.");

            if (request.ProcessadoSucesso)
            {
                pagamentoIntegracao.Pagamento.Situacao = SituacaoPagamento.Finalizado;
                pagamentoIntegracao.SituacaoIntegracao = SituacaoIntegracao.Integrado;
            }
            else
            {
                pagamentoIntegracao.SituacaoIntegracao = SituacaoIntegracao.ProblemaIntegracao;
                pagamentoIntegracao.Pagamento.Situacao = SituacaoPagamento.FalhaIntegracao;
            }

            pagamentoIntegracao.DataIntegracao = DateTime.Now;
            pagamentoIntegracao.ProblemaIntegracao = request.MensagemRetorno;

            repositorioPagamentoIntegracao.Atualizar(pagamentoIntegracao);
            repositorioPagamento.Atualizar(pagamentoIntegracao.Pagamento);

            if (pagamentoIntegracao.SituacaoIntegracao == SituacaoIntegracao.Integrado)
                servicoProvisao.GerarEstornoProvisaoAposLiberacaoPagamento(pagamentoIntegracao);
        }

        public static void LiberarPagamentoReentrega(Dominio.Entidades.Embarcador.Cargas.Carga carga, Dominio.Entidades.Embarcador.Configuracao.ConfiguracaoTMS configuracaoTMS, Dominio.Entidades.Embarcador.Configuracoes.ConfiguracaoFinanceiro configuracaoFinanceiro, Repositorio.UnitOfWork unitOfWork)
        {
            if (carga.CargaTransbordo || !(configuracaoTMS.GerarPagamentoBloqueado || configuracaoFinanceiro.DesbloquearPagamentoPorCanhoto))
                return;

            Repositorio.Embarcador.Financeiro.DocumentoFaturamento repositorioDocumentoFaturamento = new Repositorio.Embarcador.Financeiro.DocumentoFaturamento(unitOfWork);

            List<int> codigosDFA = repositorioDocumentoFaturamento.BuscarDocumentoFaturamentoCargaReentregue(carga.Codigo);

            if (configuracaoTMS.GerarPagamentoBloqueado && codigosDFA.Count > 0)
                repositorioDocumentoFaturamento.LiberarPagamentosPorDFAs(codigosDFA, DateTime.Now);
            else if (configuracaoFinanceiro.DesbloquearPagamentoPorCanhoto && codigosDFA.Count > 0)
                GerarEstornoPagamentoPorDFAs(codigosDFA, unitOfWork);
        }

        public static void DesbloquearTituloPagamentoCanhotosPendentes(Repositorio.UnitOfWork unitOfWork)
        {
            try
            {
                Repositorio.Embarcador.Canhotos.Canhoto repositorioCanhoto = new Repositorio.Embarcador.Canhotos.Canhoto(unitOfWork);
                Repositorio.Embarcador.Financeiro.DocumentoFaturamento repositorioDocumentoFaturamento = new Repositorio.Embarcador.Financeiro.DocumentoFaturamento(unitOfWork);
                Repositorio.Embarcador.Escrituracao.PagamentoIntegracao repositorioPagamentoIntegracao = new Repositorio.Embarcador.Escrituracao.PagamentoIntegracao(unitOfWork);
                Repositorio.Embarcador.Escrituracao.DocumentoProvisao repositorioDocumentoProvisao = new Repositorio.Embarcador.Escrituracao.DocumentoProvisao(unitOfWork);
                Repositorio.Embarcador.Configuracoes.ConfiguracaoFinanceiro repConfiguracaoFinanceiro = new Repositorio.Embarcador.Configuracoes.ConfiguracaoFinanceiro(unitOfWork);

                Dominio.Entidades.Embarcador.Configuracoes.ConfiguracaoFinanceiro configuracaoFinanceiro = repConfiguracaoFinanceiro.BuscarConfiguracaoPadrao();

                if (!configuracaoFinanceiro.DesbloquearPagamentoPorCanhoto)
                    return;

                int quantidadeCanhotosProcessar = 50;
                int minutosIntervalo = 5;

                List<Dominio.Entidades.Embarcador.Canhotos.Canhoto> canhotos = repositorioCanhoto.ConsultarCanhotosDigitalizadosPendentesDesbloqueioTitulo(quantidadeCanhotosProcessar, minutosIntervalo);

                foreach (Dominio.Entidades.Embarcador.Canhotos.Canhoto canhoto in canhotos)
                {
                    List<int> codigosNotasFiscais = new List<int>();

                    if (canhoto.TipoCanhoto == TipoCanhoto.Avulso)
                    {
                        Dominio.Entidades.Embarcador.Canhotos.CanhotoAvulso canhotoAvulso = canhoto.CanhotoAvulso;
                        ICollection<Dominio.Entidades.Embarcador.Pedidos.PedidoXMLNotaFiscal> pedidosXMLNotaFiscal = canhotoAvulso?.PedidosXMLNotasFiscais;

                        if (pedidosXMLNotaFiscal != null)
                            codigosNotasFiscais.AddRange(pedidosXMLNotaFiscal
                                .Where(pedido => pedido.XMLNotaFiscal != null)
                                .Select(pedido => pedido.XMLNotaFiscal.Codigo));
                    }
                    else if (canhoto.XMLNotaFiscal != null)
                    {
                        codigosNotasFiscais.Add(canhoto.XMLNotaFiscal.Codigo);
                    }

                    bool bloquearCanhoto = false;
                    List<Dominio.Entidades.Embarcador.Financeiro.DocumentoFaturamento> documentosFaturamento = repositorioDocumentoFaturamento.BuscarDocumentoFaturamentoPagamentoPorCargaNotaFiscal(codigosNotasFiscais, canhoto.Carga.Codigo);

                    foreach (Dominio.Entidades.Embarcador.Financeiro.DocumentoFaturamento documentoFaturamento in documentosFaturamento)
                    {
                        Dominio.Entidades.Embarcador.Escrituracao.Pagamento pagamento = documentoFaturamento.Pagamento;

                        int quantidadeIntegracoes = repositorioPagamentoIntegracao.ContarPorPagamento(pagamento.Codigo);

                        if (quantidadeIntegracoes == 0)
                        {
                            bloquearCanhoto = true;
                            continue;
                        }
                        else if (quantidadeIntegracoes >= 2)
                            continue;

                        Dominio.Entidades.Embarcador.Escrituracao.PagamentoIntegracao pagamentoIntegracao = repositorioPagamentoIntegracao.BuscarPrimeiroPorPagamento(pagamento.Codigo);

                        if (pagamentoIntegracao.SituacaoIntegracao != SituacaoIntegracao.Integrado)
                        {
                            bloquearCanhoto = true;
                            continue;
                        }

                        DateTime dataIntegracao = pagamentoIntegracao.DataIntegracao;
                        if (configuracaoFinanceiro.GerarIntegracaoContabilizacaoCtesApos && configuracaoFinanceiro.DelayIntegracaoContabilizacaoCtes != null)
                            dataIntegracao = dataIntegracao.AddMinutes((double)configuracaoFinanceiro.DelayIntegracaoContabilizacaoCtes);
                        else
                            dataIntegracao = dataIntegracao.AddMinutes(30);

                        if (dataIntegracao >= DateTime.Now)
                        {
                            bloquearCanhoto = true;
                            continue;
                        }

                        if (PodeGerarEstornoPagamentoIntegracao(documentoFaturamento, repositorioDocumentoProvisao, canhoto.TipoCanhoto == TipoCanhoto.Avulso))
                            GerarEstornoPagamento(pagamentoIntegracao, unitOfWork);
                        else
                            bloquearCanhoto = true;

                    }

                    canhoto.DataGeracaoCancelamentoAutomatico = DateTime.Now;

                    if (!bloquearCanhoto)
                        canhoto.SituacaoGeracaoCancelamentoAutomatico = SituacaoGeracaoCancelamentoAutomatico.DesbloqueioTituloGerado;

                    repositorioCanhoto.Atualizar(canhoto);
                }
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex, "GeracaoEstornoPagamento");
            }
        }

        public IList<Dominio.ObjetosDeValor.WebService.Rest.Escrituracao.BuscarDadosPagamento.CargaPagamento> BuscarDadosPagamento(Dominio.ObjetosDeValor.WebService.Rest.Escrituracao.BuscarDadosPagamento.RequisicaoBuscarDadosPagamento request, Repositorio.UnitOfWork unitOfWork)
        {
            Repositorio.Embarcador.Escrituracao.PagamentoIntegracao repositorioPagamentoIntegracao = new(unitOfWork);
            Repositorio.Embarcador.Escrituracao.Pagamento repositorioPagamento = new(unitOfWork);
            Repositorio.Embarcador.Financeiro.DocumentoFaturamento repositorioDocumentoFaturamento = new(unitOfWork);
            Repositorio.Embarcador.Cargas.Carga repositorioCarga = new(unitOfWork);

            if (request.ProtocoloDocumento == null && request.ChaveDocumento == null && request.dataEmissaoDocumentoInicial == null && request.dataEmissaoDocumentoFinal == null)
                throw new ServicoException("Nenhum parâmetro enviado!");

            if (request.Limite - request.Inicio > 50)
                throw new ServicoException("Limite de registros excedido, tente com um intervalo inferior a 50!");

            IList<Dominio.ObjetosDeValor.WebService.Rest.Escrituracao.BuscarDadosPagamento.CargaPagamento> cargasPagamentos = repositorioCarga.BuscarParaAPIDePagamentos(request);

            if (cargasPagamentos.IsNullOrEmpty())
            {
                if (request.ProtocoloDocumento > 0)
                    throw new ServicoException($"Não existe pagamento para o documento de protocolo: {request.ProtocoloDocumento}");
                if (request.ChaveDocumento != null)
                    throw new ServicoException($"Não existe pagamento para o documento de chave: {request.ChaveDocumento}.");
                if (request.dataEmissaoDocumentoFinal < request.dataEmissaoDocumentoInicial || request.dataEmissaoDocumentoInicial == null || request.dataEmissaoDocumentoFinal == null)
                    throw new ServicoException("Intervalo de tempo inválido.");
                else
                    throw new ServicoException("Erro ao buscar pagamentos.");
            }

            List<int> codigosCargas = cargasPagamentos.Select(cpa => cpa.CodigoCarga).ToList();

            IList<Dominio.ObjetosDeValor.WebService.Rest.Escrituracao.BuscarDadosPagamento.Pagamento> pagamentos = repositorioPagamento.BuscarParaAPIDePagamentos(codigosCargas);

            List<int> codigosPagamentos = pagamentos.Select(pag => pag.CodigoPagamento).ToList();

            IList<Dominio.ObjetosDeValor.WebService.Rest.Escrituracao.BuscarDadosPagamento.DocumentoPagamento> documentosPagamentos = repositorioDocumentoFaturamento.BuscarParaAPIDePagamentos(
                codigosPagamentos,
                request.dataEmissaoDocumentoInicial.Value,
                request.dataEmissaoDocumentoFinal.Value
            );

            IList<Dominio.ObjetosDeValor.WebService.Rest.Escrituracao.BuscarDadosPagamento.IntegracaoPagamento> integracoes = repositorioPagamentoIntegracao.BuscarParaAPIDePagamentos(codigosPagamentos);
            List<Dominio.ObjetosDeValor.WebService.Rest.Escrituracao.BuscarDadosPagamento.IntegracaoPagamento> integracoesPagamentos = integracoes.Where(integracao => integracao.CodigoIntDocumentoPagamento < 0).ToList();
            List<Dominio.ObjetosDeValor.WebService.Rest.Escrituracao.BuscarDadosPagamento.IntegracaoPagamento> integracoesDocumentos = integracoes.Where(integracao => integracao.CodigoIntDocumentoPagamento > 0).ToList();

            foreach (Dominio.ObjetosDeValor.WebService.Rest.Escrituracao.BuscarDadosPagamento.CargaPagamento cargaPagamento in cargasPagamentos)
            {
                cargaPagamento.Pagamentos = pagamentos.Where(pagamento => pagamento.CodigoCarga == cargaPagamento.CodigoCarga).ToList();
                List<Dominio.ObjetosDeValor.WebService.Rest.Escrituracao.BuscarDadosPagamento.Pagamento> pagamentosARemover = new();

                foreach (Dominio.ObjetosDeValor.WebService.Rest.Escrituracao.BuscarDadosPagamento.Pagamento pagamento in cargaPagamento.Pagamentos)
                {
                    pagamento.Integracoes = integracoesPagamentos.Where(integracao => integracao.CodigoIntPagamento == pagamento.CodigoPagamento).ToList();
                    pagamento.Documentos = documentosPagamentos.Where(documento => documento.CodigoPagamento == pagamento.CodigoPagamento).ToList();

                    if (pagamento.Documentos.IsNullOrEmpty())
                    {
                        pagamentosARemover.Add(pagamento);
                        continue;
                    }

                    foreach (Dominio.ObjetosDeValor.WebService.Rest.Escrituracao.BuscarDadosPagamento.DocumentoPagamento documento in pagamento.Documentos)
                    {
                        documento.Integracoes = integracoesDocumentos.Where(integracao => integracao.CodigoIntDocumentoPagamento == documento.CodigoDocumento).ToList();
                    }
                }

                cargaPagamento.Pagamentos.RemoveAll(p => pagamentosARemover.Contains(p));
            }

            return cargasPagamentos;
        }

        public static void GerarEstornoPagamento(Dominio.Entidades.Embarcador.Escrituracao.PagamentoIntegracao pagamentoIntegracao, Repositorio.UnitOfWork unitOfWork)
        {
            Repositorio.Embarcador.Escrituracao.PagamentoIntegracao repositorioPagamentoIntegracao = new Repositorio.Embarcador.Escrituracao.PagamentoIntegracao(unitOfWork);
            Repositorio.Embarcador.Escrituracao.Pagamento repositorioPagamento = new Repositorio.Embarcador.Escrituracao.Pagamento(unitOfWork);

            Dominio.Entidades.Embarcador.Escrituracao.PagamentoIntegracao estornoPagamento = pagamentoIntegracao.Clonar();
            Utilidades.Object.DefinirListasGenericasComoNulas(estornoPagamento);
            estornoPagamento.DataIntegracao = pagamentoIntegracao.DataIntegracao > DateTime.Now.AddMinutes(30) ? DateTime.Now : DateTime.Now.AddMinutes(30);
            estornoPagamento.NumeroTentativas = 0;
            estornoPagamento.ProblemaIntegracao = "";
            estornoPagamento.SituacaoIntegracao = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoIntegracao.AgIntegracao;
            estornoPagamento.Pagamento.Situacao = SituacaoPagamento.AguardandoIntegracao;

            repositorioPagamentoIntegracao.Inserir(estornoPagamento);
            repositorioPagamento.Atualizar(estornoPagamento.Pagamento);
        }

        public static void GerarEstornoPagamentoPorDFAs(List<int> codigosDFAs, Repositorio.UnitOfWork unitOfWork)
        {
            Repositorio.Embarcador.Escrituracao.PagamentoIntegracao repositorioPagamentoIntegracao = new Repositorio.Embarcador.Escrituracao.PagamentoIntegracao(unitOfWork);

            List<Dominio.Entidades.Embarcador.Escrituracao.PagamentoIntegracao> pagamentosIntegracao = repositorioPagamentoIntegracao.BuscarPorDocumentosFaturamento(codigosDFAs);

            IEnumerable<IGrouping<int, Dominio.Entidades.Embarcador.Escrituracao.PagamentoIntegracao>> agrupamentosPagamentoIntegracao = pagamentosIntegracao
                .Where(p => p.DocumentoFaturamento != null && codigosDFAs.Contains(p.DocumentoFaturamento.Codigo))
                .GroupBy(p => p.DocumentoFaturamento.Codigo);

            foreach (IGrouping<int, Dominio.Entidades.Embarcador.Escrituracao.PagamentoIntegracao> agrupamento in agrupamentosPagamentoIntegracao)
            {
                List<Dominio.Entidades.Embarcador.Escrituracao.PagamentoIntegracao> integracoes = agrupamento.ToList();

                if (integracoes.Count != 1)
                    continue;

                var integracao = integracoes.First();

                if (integracao.SituacaoIntegracao != SituacaoIntegracao.Integrado)
                    continue;

                GerarEstornoPagamento(integracao, unitOfWork);
            }
        }

        #endregion

        #region Métodos Privados

        private static void GerarEDIsPaginados(Dominio.Entidades.Embarcador.Escrituracao.Pagamento pagamento, Dominio.Entidades.LayoutEDI layout, Dominio.Entidades.Embarcador.Cargas.TipoIntegracao tipoIntegracao, Repositorio.UnitOfWork unitOfWork)
        {
            Repositorio.Embarcador.Escrituracao.PagamentoEDIIntegracao repPagamentoEDIIntegracao = new Repositorio.Embarcador.Escrituracao.PagamentoEDIIntegracao(unitOfWork);
            Repositorio.Embarcador.ConfiguracaoContabil.DocumentoContabil repDocumentoContabil = new Repositorio.Embarcador.ConfiguracaoContabil.DocumentoContabil(unitOfWork);
            Repositorio.Embarcador.Configuracao.ConfiguracaoTMS repConfiguracaoTMS = new Repositorio.Embarcador.Configuracao.ConfiguracaoTMS(unitOfWork);

            var config = repConfiguracaoTMS.BuscarConfiguracaoPadrao();

            int quantidadeDocumentosDoPagamento = 0;
            quantidadeDocumentosDoPagamento += repDocumentoContabil.ContarNFSParaGeracaoEDIPorPagamento(pagamento.Codigo, pagamento.LotePagamentoLiberado);
            quantidadeDocumentosDoPagamento += repDocumentoContabil.ContarCTesParaGeracaoEDIPorPagamento(pagamento.Codigo, pagamento.LotePagamentoLiberado);

            int numeroCTesPorArquivo = (int)Math.Floor((decimal)(config.INTPFAR_LimiteLinhasArquivoEDI - config.INTPFAR_LinhasNecessariasOutrasInformacoes) / config.INTPFAR_NumeroLinhasFeradasPorCTe);
            int numeroDeArquivos = (int)Math.Ceiling((decimal)quantidadeDocumentosDoPagamento / numeroCTesPorArquivo);

            for (int i = 0; i < numeroDeArquivos; i++)
            {
                int inicio = (i * numeroCTesPorArquivo);
                int limite = numeroCTesPorArquivo;

                Dominio.Entidades.Embarcador.Escrituracao.PagamentoEDIIntegracao integracao = GerarEDIPagamento(pagamento, layout, tipoIntegracao, inicio, limite);

                repPagamentoEDIIntegracao.Inserir(integracao);
            }
        }

        private static Dominio.Entidades.Embarcador.Escrituracao.PagamentoEDIIntegracao GerarEDIPagamento(Dominio.Entidades.Embarcador.Escrituracao.Pagamento pagamento, Dominio.Entidades.LayoutEDI layout, Dominio.Entidades.Embarcador.Cargas.TipoIntegracao tipoIntegracao, int inicio = 0, int limite = 0)
        {
            return new Dominio.Entidades.Embarcador.Escrituracao.PagamentoEDIIntegracao
            {
                Pagamento = pagamento,
                LayoutEDI = layout,
                TipoIntegracao = tipoIntegracao,
                Inicio = inicio,
                Limite = limite,

                ProblemaIntegracao = "",
                SequenciaIntegracao = 1,
                NumeroTentativas = 0,
                DataIntegracao = DateTime.Now,
                SituacaoIntegracao = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoIntegracao.AgIntegracao,
            };
        }

        private static bool EfetuarIntegracaoEDIPagamentoINTPFAR(Dominio.Entidades.Embarcador.Escrituracao.Pagamento pagamento, Repositorio.UnitOfWork unitOfWork)
        {
            // Instancia repositorios
            Repositorio.Embarcador.Escrituracao.PagamentoEDIIntegracao repositorioPagamentoEDIIntegracao = new Repositorio.Embarcador.Escrituracao.PagamentoEDIIntegracao(unitOfWork);
            Repositorio.Embarcador.Configuracao.ConfiguracaoTMS repositorioConfiguracaoTMS = new Repositorio.Embarcador.Configuracao.ConfiguracaoTMS(unitOfWork);

            Servicos.Embarcador.Escrituracao.Pagamento servicoPagamento = new Servicos.Embarcador.Escrituracao.Pagamento(unitOfWork);

            var config = repositorioConfiguracaoTMS.BuscarConfiguracaoPadrao();

            // Atualiza status do pagamento
            pagamento.Situacao = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoPagamento.AguardandoIntegracao;

            // Cria entidade para integracao
            List<Dominio.Entidades.Embarcador.Transportadores.TransportadorLayoutEDI> layoutsTransportador = Servicos.Embarcador.Escrituracao.Pagamento.LayoutEDIPagamentoTransportador(pagamento, unitOfWork);
            List<Dominio.Entidades.Embarcador.Transportadores.TransportadorLayoutEDI> EDIsTransportador = (from o in layoutsTransportador where o.LayoutEDI.Tipo == Dominio.Enumeradores.TipoLayoutEDI.INTPFAR select o).ToList();

            List<Dominio.Entidades.Embarcador.Pessoas.GrupoPessoasLayoutEDI> layouts = servicoPagamento.LayoutEDIPagamentoAsync(pagamento).GetAwaiter().GetResult();
            List<Dominio.Entidades.Embarcador.Pessoas.GrupoPessoasLayoutEDI> EDIs = (from o in layouts where o.LayoutEDI.Tipo == Dominio.Enumeradores.TipoLayoutEDI.INTPFAR select o).ToList();

            List<Dominio.Entidades.Embarcador.Pessoas.ClienteLayoutEDI> layoutsCliente = servicoPagamento.LayoutEDIPagamentoClienteAsync(pagamento).GetAwaiter().GetResult();
            List<Dominio.Entidades.Embarcador.Pessoas.ClienteLayoutEDI> EDIsCliente = (from o in layoutsCliente where o.LayoutEDI.Tipo == Dominio.Enumeradores.TipoLayoutEDI.INTPFAR select o).ToList();

            if (EDIsCliente.Count > 0)
            {
                int count = EDIsCliente.Count;
                for (int i = 0; i < count; i++)
                {
                    Dominio.Entidades.Embarcador.Escrituracao.PagamentoEDIIntegracao pagamentoEDIIntegracao = repositorioPagamentoEDIIntegracao.BuscarPorPagamentoELayout(pagamento.Codigo, EDIsCliente[i].LayoutEDI.Codigo);
                    if (pagamentoEDIIntegracao == null)
                    {
                        if (EDIsCliente[i].LayoutEDI.Tipo == Dominio.Enumeradores.TipoLayoutEDI.INTPFAR && config.INTPFAR_LimiteLinhasArquivoEDI > 0)
                        {
                            GerarEDIsPaginados(pagamento, EDIsCliente[i].LayoutEDI, EDIsCliente[i].TipoIntegracao, unitOfWork);
                        }
                        else
                        {
                            pagamentoEDIIntegracao = GerarEDIPagamento(pagamento, EDIsCliente[i].LayoutEDI, EDIsCliente[i].TipoIntegracao);
                            repositorioPagamentoEDIIntegracao.Inserir(pagamentoEDIIntegracao);
                        }
                    }
                }

                return count > 0;
            }
            else if (EDIs.Count > 0)
            {
                int count = EDIs.Count();
                for (int i = 0; i < count; i++)
                {
                    Dominio.Entidades.Embarcador.Escrituracao.PagamentoEDIIntegracao pagamentoEDIIntegracao = repositorioPagamentoEDIIntegracao.BuscarPorPagamentoELayout(pagamento.Codigo, EDIs[i].LayoutEDI.Codigo);
                    if (pagamentoEDIIntegracao == null)
                    {
                        if (EDIs[i].LayoutEDI.Tipo == Dominio.Enumeradores.TipoLayoutEDI.INTPFAR && config.INTPFAR_LimiteLinhasArquivoEDI > 0)
                        {
                            GerarEDIsPaginados(pagamento, EDIs[i].LayoutEDI, EDIs[i].TipoIntegracao, unitOfWork);
                        }
                        else
                        {
                            pagamentoEDIIntegracao = GerarEDIPagamento(pagamento, EDIs[i].LayoutEDI, EDIs[i].TipoIntegracao);
                            repositorioPagamentoEDIIntegracao.Inserir(pagamentoEDIIntegracao);
                        }
                    }
                }

                return count > 0;
            }
            else
            {
                int count = EDIsTransportador.Count();
                for (int i = 0; i < count; i++)
                {
                    Dominio.Entidades.Embarcador.Transportadores.TransportadorLayoutEDI layoutTransportador = EDIsTransportador[i];
                    Dominio.Entidades.Embarcador.Escrituracao.PagamentoEDIIntegracao pagamentoEDIIntegracao = repositorioPagamentoEDIIntegracao.BuscarPorPagamentoELayout(pagamento.Codigo, layoutTransportador.LayoutEDI.Codigo);
                    if (pagamentoEDIIntegracao == null)
                    {
                        if (layoutTransportador.LayoutEDI.Tipo == Dominio.Enumeradores.TipoLayoutEDI.INTPFAR && config.INTPFAR_LimiteLinhasArquivoEDI > 0)
                        {
                            GerarEDIsPaginados(pagamento, layoutTransportador.LayoutEDI, layoutTransportador.TipoIntegracao, unitOfWork);
                        }
                        else
                        {
                            pagamentoEDIIntegracao = GerarEDIPagamento(pagamento, layoutTransportador.LayoutEDI, layoutTransportador.TipoIntegracao);
                            repositorioPagamentoEDIIntegracao.Inserir(pagamentoEDIIntegracao);
                        }
                    }
                }

                return count > 0;
            }
        }

        /// <summary>
        /// Cria uma integração de EDI do tipo ImportsysCTe/ImportsysVP quando algum dos documentos selecionados tem um LayoutEDI do tipo ImportsysCTe/ImportsysVP
        /// em seu tipo de operação
        /// </summary>
        private static bool EfetuarIntegracaoEDIPagamentoPorTipoOperacao(Dominio.Entidades.Embarcador.Escrituracao.Pagamento pagamento, Dominio.Enumeradores.TipoLayoutEDI tipoLayoutEDI, Repositorio.UnitOfWork unitOfWork)
        {
            // Instancia repositorios
            Repositorio.Embarcador.Escrituracao.PagamentoEDIIntegracao repPagamentoEDIIntegracao = new Repositorio.Embarcador.Escrituracao.PagamentoEDIIntegracao(unitOfWork);
            Repositorio.Embarcador.Pessoas.GrupoPessoas repGrupoPessoas = new Repositorio.Embarcador.Pessoas.GrupoPessoas(unitOfWork);
            Repositorio.Embarcador.Configuracao.ConfiguracaoTMS repConfiguracaoTMS = new Repositorio.Embarcador.Configuracao.ConfiguracaoTMS(unitOfWork);

            var config = repConfiguracaoTMS.BuscarConfiguracaoPadrao();

            // Atualiza status do pagamento
            pagamento.Situacao = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoPagamento.AguardandoIntegracao;

            // Cria entidade para integracao
            List<Dominio.Entidades.Embarcador.Pedidos.TipoOperacaoLayoutEDI> layoutsTipoOperacao = ObterLayoutEDIsPagamentoTipoOperacao(pagamento, unitOfWork);
            List<Dominio.Entidades.Embarcador.Pedidos.TipoOperacaoLayoutEDI> EDIsTipoOperacao = (from o in layoutsTipoOperacao where o.LayoutEDI.Tipo == tipoLayoutEDI select o).ToList();

            foreach (var ediTipoOperacao in EDIsTipoOperacao)
            {
                Dominio.Entidades.Embarcador.Escrituracao.PagamentoEDIIntegracao pagamentoEDIIntegracao = repPagamentoEDIIntegracao.BuscarPorPagamentoELayout(pagamento.Codigo, ediTipoOperacao.LayoutEDI.Codigo);

                if (pagamentoEDIIntegracao == null)
                {
                    pagamentoEDIIntegracao = GerarEDIPagamento(pagamento, ediTipoOperacao.LayoutEDI, ediTipoOperacao.TipoIntegracao);
                    repPagamentoEDIIntegracao.Inserir(pagamentoEDIIntegracao);
                }
            }

            return EDIsTipoOperacao.Count > 0;
        }

        private static List<Dominio.ObjetosDeValor.Embarcador.ConfiguracaoContabil.ConfiguracaoContabil> ObterConfiguracoesContabeis(List<Dominio.ObjetosDeValor.Embarcador.ConfiguracaoContabil.ConfiguracaoContaContabilContabilizacao> configuracaoContaContabilContabilizacaos)
        {
            List<Dominio.ObjetosDeValor.Embarcador.ConfiguracaoContabil.ConfiguracaoContabil> configuracoesContabeis = new List<Dominio.ObjetosDeValor.Embarcador.ConfiguracaoContabil.ConfiguracaoContabil>();

            foreach (Dominio.ObjetosDeValor.Embarcador.ConfiguracaoContabil.ConfiguracaoContaContabilContabilizacao configuracaoContabilizacao in configuracaoContaContabilContabilizacaos)
            {
                Dominio.ObjetosDeValor.Embarcador.ConfiguracaoContabil.ConfiguracaoContabil configuracaoContabil = new Dominio.ObjetosDeValor.Embarcador.ConfiguracaoContabil.ConfiguracaoContabil();
                if (configuracaoContabilizacao.CodigoPlanoConta > 0)
                    configuracaoContabil.PlanoConta = configuracaoContabilizacao.CodigoPlanoConta;
                configuracaoContabil.TipoContabilizacao = configuracaoContabilizacao.TipoContabilizacao;
                if (configuracaoContabilizacao.CodigoPlanoContaContraPartidaProvisao > 0)
                    configuracaoContabil.PlanoContaContraPartida = configuracaoContabilizacao.CodigoPlanoContaContraPartidaProvisao;
                configuracaoContabil.TipoContaContabil = configuracaoContabilizacao.TipoContaContabil;
                configuracaoContabil.ComponentesDeFreteDoTipoDescontoNaoDevemSomar = configuracaoContabilizacao.ComponentesDeFreteDoTipoDescontoNaoDevemSomarNaContabilizacao;

                configuracoesContabeis.Add(configuracaoContabil);
            }

            return configuracoesContabeis;
        }

        private static List<Dominio.ObjetosDeValor.Embarcador.ConfiguracaoContabil.ConfiguracaoContabil> ObterConfiguracoesContabeis(List<Dominio.Entidades.CTeContaContabilContabilizacao> cteContaContabilContabilizacao)
        {
            List<Dominio.ObjetosDeValor.Embarcador.ConfiguracaoContabil.ConfiguracaoContabil> configuracoesContabeis = new List<Dominio.ObjetosDeValor.Embarcador.ConfiguracaoContabil.ConfiguracaoContabil>();

            foreach (Dominio.Entidades.CTeContaContabilContabilizacao configuracaoContabilizacao in cteContaContabilContabilizacao)
            {
                Dominio.ObjetosDeValor.Embarcador.ConfiguracaoContabil.ConfiguracaoContabil configuracaoContabil = new Dominio.ObjetosDeValor.Embarcador.ConfiguracaoContabil.ConfiguracaoContabil();
                configuracaoContabil.PlanoConta = configuracaoContabilizacao.PlanoConta.Codigo;
                configuracaoContabil.TipoContabilizacao = configuracaoContabilizacao.TipoContabilizacao;
                //configuracaoContabil.PlanoContaContraPartida = configuracaoContabilizacao.PlanoContaContraPartidaProvisao?.Codigo ?? 0;
                configuracaoContabil.TipoContaContabil = configuracaoContabilizacao.TipoContaContabil;
                configuracoesContabeis.Add(configuracaoContabil);
            }

            return configuracoesContabeis;

        }

        private static void GerarFechamentoDocumentosPagamento(Dominio.Entidades.Embarcador.Escrituracao.Pagamento pagamento, Repositorio.UnitOfWork unidadeTrabalho, string stringConexao, AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware tipoServicoMultisoftware, Dominio.Entidades.Embarcador.Configuracao.ConfiguracaoTMS configuracao, string urlAcesso)
        {
            Repositorio.Embarcador.Financeiro.DocumentoFaturamento repDocumentosFaturamento = new Repositorio.Embarcador.Financeiro.DocumentoFaturamento(unidadeTrabalho);
            Repositorio.Embarcador.Escrituracao.Pagamento repPagamento = new Repositorio.Embarcador.Escrituracao.Pagamento(unidadeTrabalho);
            Repositorio.Embarcador.Pedidos.TipoOperacao repTipoOperacao = new Repositorio.Embarcador.Pedidos.TipoOperacao(unidadeTrabalho);
            Repositorio.Embarcador.Cargas.Carga repCarga = new Repositorio.Embarcador.Cargas.Carga(unidadeTrabalho);
            Repositorio.RotaFrete repRotaFrete = new Repositorio.RotaFrete(unidadeTrabalho);
            Repositorio.Cliente repCliente = new Repositorio.Cliente(unidadeTrabalho);
            Repositorio.Empresa repEmpresa = new Repositorio.Empresa(unidadeTrabalho);
            Repositorio.Embarcador.Configuracoes.ConfiguracaoFatura repConfiguracaoFatura = new Repositorio.Embarcador.Configuracoes.ConfiguracaoFatura(unidadeTrabalho);
            Repositorio.Embarcador.Configuracoes.ConfiguracaoFinanceiro repConfiguracaoFinanceiro = new Repositorio.Embarcador.Configuracoes.ConfiguracaoFinanceiro(unidadeTrabalho);

            Dominio.Entidades.Embarcador.Configuracoes.ConfiguracaoFinanceiro configuracaoFinanceiro = repConfiguracaoFinanceiro.BuscarConfiguracaoPadrao();
            Dominio.Entidades.Embarcador.Configuracoes.ConfiguracaoFatura configuracaoFatura = repConfiguracaoFatura.BuscarConfiguracaoPadrao();

            PagamentoAprovacao servicoPagamentoAprovacao = new PagamentoAprovacao(unidadeTrabalho, urlAcesso);
            Hubs.Pagamento servicoNotificacaoPagamento = new Hubs.Pagamento();

            if (!pagamento.LotePagamentoLiberado)
            {
                Servicos.Embarcador.ConfiguracaoContabil.ConfiguracaoCentroResultado serConfiguracaoCentroResultado = new ConfiguracaoContabil.ConfiguracaoCentroResultado();
                Servicos.Embarcador.ConfiguracaoContabil.ConfiguracaoContaContabil serConfiguracaoContaContabil = new ConfiguracaoContabil.ConfiguracaoContaContabil();

                List<Dominio.ObjetosDeValor.Embarcador.Escrituracao.PagamentoSumarizada> pagamentoSumarizado = repDocumentosFaturamento.BuscarCodigosPorPagamentoEmFechamento(pagamento.Codigo);

                int quantidadeDocumentos = pagamento.QuantidadeDocsPagamento;
                int quantidadeGerados = quantidadeDocumentos - pagamentoSumarizado.Count();

                List<int> empresas = (from obj in pagamentoSumarizado select obj.Empresa).Distinct().ToList();
                int quantidadeTotal = 0;
                for (int e = 0; e < empresas.Count; e++)
                {
                    int codigoEmpresa = empresas[e];

                    Dominio.Entidades.Empresa empresa = repEmpresa.BuscarPorCodigo(codigoEmpresa);

                    if (string.IsNullOrWhiteSpace(empresa.CodigoIntegracao))
                    {
                        pagamento.MotivoRejeicaoFechamentoPagamento = "Não existe um codigo de integração cadastrado para a transportadora " + empresa.Descricao + ".";
                        pagamento.Situacao = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoPagamento.PendenciaFechamento;
                        repPagamento.Atualizar(pagamento);
                        if (unidadeTrabalho.IsActiveTransaction())
                            unidadeTrabalho.CommitChanges();
                        servicoNotificacaoPagamento.InformarPagamentoAtualizada(pagamento.Codigo);
                        return;
                    }

                    decimal aliquotaCOFINS = empresa.EmpresaPai?.Configuracao?.AliquotaCOFINS ?? 0;
                    decimal aliquotaPIS = empresa.EmpresaPai?.Configuracao?.AliquotaPIS ?? 0;

                    List<int?> modelosDocumentosFiscal = (from obj in pagamentoSumarizado where obj.Empresa == codigoEmpresa select obj.ModeloDocumentoFiscal).Distinct().ToList();
                    for (int md = 0; md < modelosDocumentosFiscal.Count; md++)
                    {
                        int? codigoModeloDocumento = modelosDocumentosFiscal[md];
                        Dominio.Entidades.ModeloDocumentoFiscal modeloDocumentoFiscal = codigoModeloDocumento.HasValue ? new Dominio.Entidades.ModeloDocumentoFiscal() { Codigo = codigoModeloDocumento.Value } : null;

                        List<int?> tiposOcorrencia = (from obj in pagamentoSumarizado where obj.ModeloDocumentoFiscal == codigoModeloDocumento && obj.Empresa == codigoEmpresa select obj.TipoOcorrencia).Distinct().ToList();

                        for (int tpo = 0; tpo < tiposOcorrencia.Count; tpo++)
                        {
                            int? codigoTipoOcorrencia = tiposOcorrencia[tpo];
                            Dominio.Entidades.TipoDeOcorrenciaDeCTe tipoOcorrencia = codigoTipoOcorrencia.HasValue ? new Dominio.Entidades.TipoDeOcorrenciaDeCTe() { Codigo = codigoTipoOcorrencia.Value } : null;

                            List<int?> grupoTomadores = (from obj in pagamentoSumarizado where obj.ModeloDocumentoFiscal == codigoModeloDocumento && obj.TipoOcorrencia == codigoTipoOcorrencia && obj.Empresa == codigoEmpresa select obj.GrupoTomador).Distinct().ToList();
                            for (int gt = 0; gt < grupoTomadores.Count; gt++)
                            {
                                int? codigoGrupoTomador = grupoTomadores[gt];
                                List<double> tomadores = (from obj in pagamentoSumarizado where obj.ModeloDocumentoFiscal == codigoModeloDocumento && obj.TipoOcorrencia == codigoTipoOcorrencia && obj.GrupoTomador == codigoGrupoTomador && obj.Empresa == codigoEmpresa select obj.Tomador).Distinct().ToList();
                                for (int t = 0; t < tomadores.Count; t++)
                                {
                                    double cnpjTomador = tomadores[t];
                                    int? categoriaTomador = (from obj in pagamentoSumarizado where obj.ModeloDocumentoFiscal == codigoModeloDocumento && obj.TipoOcorrencia == codigoTipoOcorrencia && obj.GrupoTomador == codigoGrupoTomador && obj.Tomador == cnpjTomador && obj.Empresa == codigoEmpresa select obj.CategoriaTomador).FirstOrDefault();

                                    Dominio.Entidades.Cliente tomador = new Dominio.Entidades.Cliente() { CPF_CNPJ = cnpjTomador };
                                    tomador.Categoria = categoriaTomador.HasValue ? new Dominio.Entidades.Embarcador.Pessoas.CategoriaPessoa() { Codigo = categoriaTomador.Value } : null;
                                    tomador.GrupoPessoas = codigoGrupoTomador.HasValue ? new Dominio.Entidades.Embarcador.Pessoas.GrupoPessoas() { Codigo = codigoGrupoTomador.Value } : null;

                                    List<int?> tiposOperacao = (from obj in pagamentoSumarizado where obj.ModeloDocumentoFiscal == codigoModeloDocumento && obj.TipoOcorrencia == codigoTipoOcorrencia && obj.Tomador == cnpjTomador && obj.Empresa == codigoEmpresa select obj.TipoOperacao).Distinct().ToList();
                                    for (int o = 0; o < tiposOperacao.Count; o++)
                                    {
                                        int? codigoTipoOperacao = tiposOperacao[o];
                                        Dominio.Entidades.Embarcador.Pedidos.TipoOperacao tipoOperacao = codigoTipoOperacao.HasValue ? new Dominio.Entidades.Embarcador.Pedidos.TipoOperacao() { Codigo = codigoTipoOperacao.Value } : null;

                                        List<int?> grupoRemetentes = (from obj in pagamentoSumarizado where obj.ModeloDocumentoFiscal == codigoModeloDocumento && obj.TipoOcorrencia == codigoTipoOcorrencia && obj.Tomador == cnpjTomador && obj.TipoOperacao == codigoTipoOperacao && obj.Empresa == codigoEmpresa select obj.GrupoRemetente).Distinct().ToList();

                                        for (int gr = 0; gr < grupoRemetentes.Count; gr++)
                                        {
                                            int? codigoGrupoRemetente = grupoRemetentes[gr];
                                            List<double> remetentes = (from obj in pagamentoSumarizado where obj.ModeloDocumentoFiscal == codigoModeloDocumento && obj.TipoOcorrencia == codigoTipoOcorrencia && obj.Tomador == cnpjTomador && obj.Empresa == codigoEmpresa && obj.GrupoRemetente == codigoGrupoRemetente && obj.TipoOperacao == codigoTipoOperacao select obj.Remetente).Distinct().ToList();

                                            for (int r = 0; r < remetentes.Count; r++)
                                            {
                                                double cnpjRemetente = remetentes[r];
                                                int? categoriaRemetente = (from obj in pagamentoSumarizado where obj.ModeloDocumentoFiscal == codigoModeloDocumento && obj.TipoOcorrencia == codigoTipoOcorrencia && obj.Tomador == cnpjTomador && obj.Empresa == codigoEmpresa && obj.GrupoRemetente == codigoGrupoRemetente && obj.TipoOperacao == codigoTipoOperacao && obj.Remetente == cnpjRemetente select obj.CategoriaRemetente).FirstOrDefault();

                                                Dominio.Entidades.Cliente remetente = new Dominio.Entidades.Cliente() { CPF_CNPJ = cnpjRemetente };
                                                remetente.Categoria = categoriaRemetente.HasValue ? new Dominio.Entidades.Embarcador.Pessoas.CategoriaPessoa() { Codigo = categoriaRemetente.Value } : null;

                                                remetente.GrupoPessoas = codigoGrupoRemetente.HasValue ? new Dominio.Entidades.Embarcador.Pessoas.GrupoPessoas() { Codigo = codigoGrupoRemetente.Value } : null;

                                                List<int?> grupoDestinatarios = (from obj in pagamentoSumarizado where obj.ModeloDocumentoFiscal == codigoModeloDocumento && obj.TipoOcorrencia == codigoTipoOcorrencia && obj.Tomador == cnpjTomador && obj.Empresa == codigoEmpresa && obj.TipoOperacao == codigoTipoOperacao && obj.Remetente == cnpjRemetente select obj.GrupoDestinatario).Distinct().ToList();
                                                for (int gd = 0; gd < grupoDestinatarios.Count; gd++)
                                                {
                                                    int? codigoGrupoDestinatario = grupoDestinatarios[gd];
                                                    List<double> destinatarios = (from obj in pagamentoSumarizado where obj.ModeloDocumentoFiscal == codigoModeloDocumento && obj.TipoOcorrencia == codigoTipoOcorrencia && obj.Tomador == cnpjTomador && obj.Empresa == codigoEmpresa && obj.Remetente == cnpjRemetente && obj.TipoOperacao == codigoTipoOperacao && obj.GrupoDestinatario == codigoGrupoDestinatario select obj.Destinatario).Distinct().ToList();

                                                    for (int d = 0; d < destinatarios.Count; d++)
                                                    {
                                                        double cnpjDestinatario = destinatarios[d];

                                                        int? categoriaDestinatario = (from obj in pagamentoSumarizado where obj.ModeloDocumentoFiscal == codigoModeloDocumento && obj.TipoOcorrencia == codigoTipoOcorrencia && obj.Tomador == cnpjTomador && obj.Empresa == codigoEmpresa && obj.Remetente == cnpjRemetente && obj.Destinatario == cnpjDestinatario && obj.TipoOperacao == codigoTipoOperacao && obj.GrupoDestinatario == codigoGrupoDestinatario select obj.CategoriaDestinatario).FirstOrDefault();

                                                        Dominio.Entidades.Cliente destinatario = new Dominio.Entidades.Cliente() { CPF_CNPJ = cnpjDestinatario };
                                                        destinatario.Categoria = categoriaDestinatario.HasValue ? new Dominio.Entidades.Embarcador.Pessoas.CategoriaPessoa() { Codigo = categoriaDestinatario.Value } : null;
                                                        destinatario.GrupoPessoas = codigoGrupoDestinatario.HasValue ? new Dominio.Entidades.Embarcador.Pessoas.GrupoPessoas() { Codigo = codigoGrupoDestinatario.Value } : null;

                                                        List<int?> rotasFrete = (from obj in pagamentoSumarizado where obj.ModeloDocumentoFiscal == codigoModeloDocumento && obj.TipoOcorrencia == codigoTipoOcorrencia && obj.Tomador == cnpjTomador && obj.Empresa == codigoEmpresa && obj.Remetente == cnpjRemetente && obj.TipoOperacao == codigoTipoOperacao && obj.Destinatario == cnpjDestinatario select obj.RotaFrete).Distinct().ToList();

                                                        for (int rf = 0; rf < rotasFrete.Count; rf++)
                                                        {
                                                            int? codigoRotaFrete = rotasFrete[rf];
                                                            Dominio.Entidades.RotaFrete rotaFrete = codigoRotaFrete.HasValue ? new Dominio.Entidades.RotaFrete() { Codigo = codigoRotaFrete.Value } : null;

                                                            List<int?> origens = (from obj in pagamentoSumarizado where obj.ModeloDocumentoFiscal == codigoModeloDocumento && obj.TipoOcorrencia == codigoTipoOcorrencia && obj.Tomador == cnpjTomador && obj.Empresa == codigoEmpresa && obj.Remetente == cnpjRemetente && obj.TipoOperacao == codigoTipoOperacao && obj.Destinatario == cnpjDestinatario && obj.RotaFrete == codigoRotaFrete select obj.Origem).Distinct().ToList();
                                                            for (int or = 0; or < origens.Count; or++)
                                                            {
                                                                int? codigoOrigem = origens[or];
                                                                Dominio.Entidades.Localidade origem = codigoOrigem.HasValue ? new Dominio.Entidades.Localidade() { Codigo = codigoOrigem.Value } : null;

                                                                List<int> filiais = (from obj in pagamentoSumarizado where obj.ModeloDocumentoFiscal == codigoModeloDocumento && obj.TipoOcorrencia == codigoTipoOcorrencia && obj.Tomador == cnpjTomador && obj.Empresa == codigoEmpresa && obj.Remetente == cnpjRemetente && obj.TipoOperacao == codigoTipoOperacao && obj.Destinatario == cnpjDestinatario && obj.RotaFrete == codigoRotaFrete && obj.Origem == codigoOrigem select obj.Filial).Distinct().ToList();
                                                                for (int fil = 0; fil < filiais.Count; fil++)
                                                                {
                                                                    int codigoFilial = filiais[fil];
                                                                    Dominio.Entidades.Embarcador.Filiais.Filial filial = new Dominio.Entidades.Embarcador.Filiais.Filial() { Codigo = codigoFilial };
                                                                    //Configuração Conta Contabil não utiliza os parametros dos for abaixo, portanto pode ser consultado menos vezes
                                                                    Dominio.ObjetosDeValor.Embarcador.ConfiguracaoContabil.ConfiguracaoContaContabil configuracaoContaContabil = serConfiguracaoContaContabil.ObterConfiguracaoContaContabil(remetente, destinatario, tomador, null, empresa, tipoOperacao, rotaFrete, modeloDocumentoFiscal, tipoOcorrencia, unidadeTrabalho);

                                                                    List<double?> expedidores = (from obj in pagamentoSumarizado where obj.ModeloDocumentoFiscal == codigoModeloDocumento && obj.TipoOcorrencia == codigoTipoOcorrencia && obj.Tomador == cnpjTomador && obj.Empresa == codigoEmpresa && obj.Remetente == cnpjRemetente && obj.TipoOperacao == codigoTipoOperacao && obj.Destinatario == cnpjDestinatario && obj.RotaFrete == codigoRotaFrete && obj.Origem == codigoOrigem && obj.Filial == codigoFilial select obj.Expedidor).Distinct().ToList();
                                                                    for (int exp = 0; exp < expedidores.Count; exp++)
                                                                    {
                                                                        double? cnpjExpedidor = expedidores[exp];

                                                                        Dominio.Entidades.Cliente expedidor = cnpjExpedidor.HasValue ? new Dominio.Entidades.Cliente() { CPF_CNPJ = cnpjExpedidor.Value } : null;

                                                                        List<double?> recebedores = (from obj in pagamentoSumarizado where obj.ModeloDocumentoFiscal == codigoModeloDocumento && obj.TipoOcorrencia == codigoTipoOcorrencia && obj.Tomador == cnpjTomador && obj.Empresa == codigoEmpresa && obj.Remetente == cnpjRemetente && obj.TipoOperacao == codigoTipoOperacao && obj.Destinatario == cnpjDestinatario && obj.RotaFrete == codigoRotaFrete && obj.Origem == codigoOrigem && obj.Filial == codigoFilial && obj.Expedidor == cnpjExpedidor select obj.Recebedor).Distinct().ToList();
                                                                        for (int rec = 0; rec < recebedores.Count; rec++)
                                                                        {
                                                                            double? cnpjRecebedor = recebedores[rec];

                                                                            Dominio.Entidades.Cliente recebedor = cnpjRecebedor.HasValue ? new Dominio.Entidades.Cliente() { CPF_CNPJ = cnpjRecebedor.Value } : null;

                                                                            Dominio.Entidades.Embarcador.ConfiguracaoContabil.ConfiguracaoCentroResultado configuracaoCentroResultado = serConfiguracaoCentroResultado.ObterConfiguracaoCentroResultado(remetente, destinatario, expedidor, recebedor, tomador, null, null, empresa, tipoOperacao, tipoOcorrencia, rotaFrete, filial, origem, unidadeTrabalho);

                                                                            if (configuracao.CentroResultadoPedidoObrigatorio || (configuracaoContaContabil != null && configuracaoCentroResultado != null && configuracaoCentroResultado.CentroResultadoContabilizacao != null && configuracaoContaContabil.ConfiguracaoContaContabilContabilizacoes.Count > 0))
                                                                            {
                                                                                List<Dominio.ObjetosDeValor.Embarcador.Escrituracao.PagamentoSumarizada> provisoesDocumentosFiltradas = (from obj in pagamentoSumarizado where obj.ModeloDocumentoFiscal == codigoModeloDocumento && obj.TipoOcorrencia == codigoTipoOcorrencia && obj.Tomador == cnpjTomador && obj.Empresa == codigoEmpresa && obj.Remetente == cnpjRemetente && obj.TipoOperacao == codigoTipoOperacao && obj.Destinatario == cnpjDestinatario && obj.RotaFrete == codigoRotaFrete && obj.Filial == codigoFilial && obj.Origem == codigoOrigem && obj.Expedidor == cnpjExpedidor && obj.Recebedor == cnpjRecebedor select obj).ToList();
                                                                                int codigoCentroResultado = configuracaoCentroResultado?.CentroResultadoContabilizacao.Codigo ?? 0;
                                                                                List<Dominio.ObjetosDeValor.Embarcador.ConfiguracaoContabil.ConfiguracaoContabil> configuracoesContabeis = new List<Dominio.ObjetosDeValor.Embarcador.ConfiguracaoContabil.ConfiguracaoContabil>();
                                                                                if (configuracaoContaContabil != null && configuracaoContaContabil.ConfiguracaoContaContabilContabilizacoes != null)
                                                                                    configuracoesContabeis = ObterConfiguracoesContabeis(configuracaoContaContabil.ConfiguracaoContaContabilContabilizacoes.ToList());

                                                                                for (int i = 0; i < provisoesDocumentosFiltradas.Count; i++)
                                                                                {
                                                                                    List<Dominio.ObjetosDeValor.Embarcador.ConfiguracaoContabil.ConfiguracaoContabil> configuracoesContabeisDocumento = configuracoesContabeis;
                                                                                    Dominio.ObjetosDeValor.Embarcador.Escrituracao.PagamentoSumarizada pagamentoSumarizada = provisoesDocumentosFiltradas[i];
                                                                                    Dominio.ObjetosDeValor.Embarcador.Escrituracao.CondicaoPagamento condicaoPagamento = ObterCondicaoPagamento(pagamento, pagamentoSumarizada, empresa.Codigo, unidadeTrabalho);

                                                                                    int codigoCentroResultadoDocumento = codigoCentroResultado;
                                                                                    if (condicaoPagamento == null && tipoServicoMultisoftware != AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiTMS && !configuracaoFinanceiro.NaoValidarCondicaoPagamentoFechamentoLotePagamento)
                                                                                    {
                                                                                        pagamento.MotivoRejeicaoFechamentoPagamento = "Nenhuma condição de pagamento configurada para a empresa ou filial.";
                                                                                        pagamento.Situacao = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoPagamento.PendenciaFechamento;
                                                                                        repPagamento.Atualizar(pagamento);
                                                                                        if (unidadeTrabalho.IsActiveTransaction())
                                                                                            unidadeTrabalho.CommitChanges();
                                                                                        servicoNotificacaoPagamento.InformarPagamentoAtualizada(pagamento.Codigo);
                                                                                        return;
                                                                                    }

                                                                                    if (configuracao.CentroResultadoPedidoObrigatorio && pagamentoSumarizada.CodigoCTe > 0)
                                                                                    {
                                                                                        Repositorio.CTeContaContabilContabilizacao repCTeContaContabilContabilizacao = new Repositorio.CTeContaContabilContabilizacao(unidadeTrabalho);
                                                                                        List<Dominio.Entidades.CTeContaContabilContabilizacao> cTeContaContabilContabilizacaos = repCTeContaContabilContabilizacao.BuscarPorCTe(pagamentoSumarizada.CodigoCTe);
                                                                                        if (cTeContaContabilContabilizacaos.Count > 0)
                                                                                        {
                                                                                            configuracoesContabeisDocumento = ObterConfiguracoesContabeis(cTeContaContabilContabilizacaos);
                                                                                            if (cTeContaContabilContabilizacaos?.FirstOrDefault()?.Cte.CentroResultado != null)
                                                                                                codigoCentroResultadoDocumento = cTeContaContabilContabilizacaos.FirstOrDefault().Cte.CentroResultado.Codigo;
                                                                                        }
                                                                                    }

                                                                                    if (pagamentoSumarizada.CargaPagamento.HasValue)
                                                                                    {
                                                                                        Dominio.Entidades.Embarcador.Cargas.Carga cargaCancelada = repCarga.VerificarCargaCancelada(pagamentoSumarizada.CargaPagamento.Value);
                                                                                        IList<int> cargasLiberadas = repPagamento.BuscarCodigosCargasLiberadasPorPagamento(pagamento.Codigo);

                                                                                        if (cargaCancelada != null && !cargasLiberadas.Contains(cargaCancelada.Codigo))
                                                                                        {
                                                                                            pagamento.MotivoRejeicaoFechamentoPagamento = "Não é possível prosseguir com o pagamento pois há um cancelamento para a carga " + cargaCancelada.CodigoCargaEmbarcador + ".";
                                                                                            pagamento.UltimaCargaEmCancelamento = cargaCancelada;
                                                                                            pagamento.Situacao = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoPagamento.PendenciaFechamento;
                                                                                            repPagamento.Atualizar(pagamento);
                                                                                            if (unidadeTrabalho.IsActiveTransaction())
                                                                                                unidadeTrabalho.CommitChanges();
                                                                                            servicoNotificacaoPagamento.InformarPagamentoAtualizada(pagamento.Codigo);
                                                                                            return;
                                                                                        }
                                                                                    }

                                                                                    bool abriuTransacao = false;
                                                                                    if (!unidadeTrabalho.IsActiveTransaction())
                                                                                    {
                                                                                        unidadeTrabalho.Start();
                                                                                        abriuTransacao = true;
                                                                                    }

                                                                                    GerarDocumentoContabil(pagamentoSumarizada, configuracoesContabeisDocumento, codigoCentroResultadoDocumento, aliquotaCOFINS, aliquotaPIS, pagamento, condicaoPagamento, unidadeTrabalho, tipoServicoMultisoftware, configuracaoFatura.DisponbilizarProvisaoContraPartidaParaCancelamento, configuracaoFinanceiro);
                                                                                    repDocumentosFaturamento.SetarDocumentoMovimentoGeradoPagamento(pagamentoSumarizada.Codigo);

                                                                                    if (abriuTransacao)
                                                                                    {
                                                                                        unidadeTrabalho.CommitChanges();
                                                                                        unidadeTrabalho.FlushAndClear();
                                                                                    }

                                                                                    if (quantidadeDocumentos < 10 || ((quantidadeTotal + 1) % 5) == 0)
                                                                                        servicoNotificacaoPagamento.InformarQuantidadeDocumentosProcessadosFechamentoPagamento(pagamento.Codigo, quantidadeDocumentos, ((quantidadeGerados + quantidadeTotal) + 1));

                                                                                    quantidadeTotal++;
                                                                                }
                                                                            }
                                                                            else
                                                                            {
                                                                                string mensagem = "Não foi possível localizar a configuração de ";
                                                                                if (configuracaoCentroResultado == null)
                                                                                    mensagem += " Centro de Resultado ";
                                                                                if (configuracaoContaContabil == null && configuracaoCentroResultado == null)
                                                                                    mensagem += " e ";
                                                                                if (configuracaoContaContabil == null)
                                                                                    mensagem += " Conta Contábil ";
                                                                                mensagem += " para as configurações a seguir: ";
                                                                                if (empresa != null)
                                                                                    mensagem += " (Empresa " + repEmpresa.BuscarPorCodigo(empresa.Codigo).Descricao + ") ";
                                                                                if (tipoOperacao != null)
                                                                                    mensagem += " (Tipo de Operação " + repTipoOperacao.BuscarPorCodigo(tipoOperacao.Codigo).Descricao + ") ";
                                                                                if (tomador != null)
                                                                                    mensagem += " (Tomador " + repCliente.BuscarPorCPFCNPJ(tomador.CPF_CNPJ).Descricao + ") ";
                                                                                if (remetente != null)
                                                                                    mensagem += " (Remetente " + repCliente.BuscarPorCPFCNPJ(remetente.CPF_CNPJ).Descricao + ") ";
                                                                                if (destinatario != null)
                                                                                    mensagem += " (Destinatário " + repCliente.BuscarPorCPFCNPJ(destinatario.CPF_CNPJ).Descricao + ") ";
                                                                                if (rotaFrete != null)
                                                                                    mensagem += " (Rota " + repRotaFrete.BuscarPorCodigo(rotaFrete.Codigo).CodigoIntegracao + ") ";
                                                                                if (expedidor != null)
                                                                                    mensagem += " (Expedidor " + repCliente.BuscarPorCPFCNPJ(expedidor.CPF_CNPJ).Descricao + ") ";
                                                                                if (recebedor != null)
                                                                                    mensagem += " (Recebedor " + repCliente.BuscarPorCPFCNPJ(recebedor.CPF_CNPJ).Descricao + ") ";

                                                                                pagamento.MotivoRejeicaoFechamentoPagamento = mensagem;
                                                                                pagamento.Situacao = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoPagamento.PendenciaFechamento;
                                                                                repPagamento.Atualizar(pagamento);
                                                                                if (unidadeTrabalho.IsActiveTransaction())
                                                                                    unidadeTrabalho.CommitChanges();
                                                                                servicoNotificacaoPagamento.InformarPagamentoAtualizada(pagamento.Codigo);
                                                                                return;
                                                                            }
                                                                        }
                                                                    }
                                                                }
                                                            }
                                                        }
                                                    }
                                                }
                                            }
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
            }

            pagamento.GerandoMovimentoFinanceiro = false;

            servicoPagamentoAprovacao.CriarAprovacao(pagamento, tipoServicoMultisoftware);
            repPagamento.Atualizar(pagamento);

            servicoNotificacaoPagamento.InformarPagamentoAtualizada(pagamento.Codigo);
        }

        private static void GerarDocumentoContabil(Dominio.ObjetosDeValor.Embarcador.Escrituracao.PagamentoSumarizada pagamentoSumarizada, List<Dominio.ObjetosDeValor.Embarcador.ConfiguracaoContabil.ConfiguracaoContabil> configuracoesContabeis, int codigoCentroResultado, decimal aliquotaCOFINS, decimal aliquotaPIS, Dominio.Entidades.Embarcador.Escrituracao.Pagamento pagamento, Dominio.ObjetosDeValor.Embarcador.Escrituracao.CondicaoPagamento condicaoPagamento, Repositorio.UnitOfWork unidadeTrabalho, AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware tipoServicoMultisoftware, bool disponbilizarProvisaoContraPartidaParaCancelamento, Dominio.Entidades.Embarcador.Configuracoes.ConfiguracaoFinanceiro configuracaoFinanceiro)
        {
            Repositorio.Embarcador.Escrituracao.DocumentoProvisao repDocumentoProvisao = new Repositorio.Embarcador.Escrituracao.DocumentoProvisao(unidadeTrabalho);
            Repositorio.Embarcador.Pedidos.PedidoXMLNotaFiscal repPedidoXMLNotaFiscal = new Repositorio.Embarcador.Pedidos.PedidoXMLNotaFiscal(unidadeTrabalho);
            Repositorio.Embarcador.Pedidos.PedidoCTeParaSubContratacao repPedidoCTeParaSubContratacao = new Repositorio.Embarcador.Pedidos.PedidoCTeParaSubContratacao(unidadeTrabalho);
            Repositorio.Embarcador.Financeiro.Titulo repTitulo = new Repositorio.Embarcador.Financeiro.Titulo(unidadeTrabalho);
            Repositorio.Embarcador.Cargas.CargaCTeComplementoInfo repCargaCteComplementoInfo = new Repositorio.Embarcador.Cargas.CargaCTeComplementoInfo(unidadeTrabalho);
            Repositorio.ConhecimentoDeTransporteEletronico repCTe = new Repositorio.ConhecimentoDeTransporteEletronico(unidadeTrabalho);
            Repositorio.Embarcador.ConfiguracaoContabil.DocumentoContabil repDocumentoContabil = new Repositorio.Embarcador.ConfiguracaoContabil.DocumentoContabil(unidadeTrabalho);
            Repositorio.Embarcador.Cargas.CargaDocumentoParaEmissaoNFSManual repCargaDocumentoParaEmissaoNFSManual = new Repositorio.Embarcador.Cargas.CargaDocumentoParaEmissaoNFSManual(unidadeTrabalho);
            Repositorio.Embarcador.Escrituracao.Pagamento repPagamento = new Repositorio.Embarcador.Escrituracao.Pagamento(unidadeTrabalho);
            Repositorio.DocumentoDeTransporteAnteriorCTe repDocumentoDeTransporteAnteriorCTe = new Repositorio.DocumentoDeTransporteAnteriorCTe(unidadeTrabalho);
            Repositorio.Embarcador.Cargas.Carga repCarga = new Repositorio.Embarcador.Cargas.Carga(unidadeTrabalho);
            Repositorio.Embarcador.Cargas.TipoIntegracao repTipoIntegracao = new Repositorio.Embarcador.Cargas.TipoIntegracao(unidadeTrabalho);

            bool permitirProvisoesEstornadas = repTipoIntegracao.ExistePorTipo(Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoIntegracao.Camil);

            List<Dominio.ObjetosDeValor.Embarcador.Escrituracao.DocumentosPagamento> documentosPagamentos = new List<Dominio.ObjetosDeValor.Embarcador.Escrituracao.DocumentosPagamento>();

            if (pagamentoSumarizada.Fechamento.HasValue)
            {
                Dominio.ObjetosDeValor.Embarcador.Escrituracao.DocumentosPagamento documentoPagamento = new Dominio.ObjetosDeValor.Embarcador.Escrituracao.DocumentosPagamento();
                documentoPagamento.Fechamento = pagamentoSumarizada.Fechamento.Value;
                documentoPagamento.CTe = repCTe.BuscarPorCodigo(pagamentoSumarizada.CodigoCTe);
                documentosPagamentos.Add(documentoPagamento);
            }
            else if (pagamentoSumarizada.LancamentoNFSManual.HasValue)
            {
                List<Dominio.Entidades.Embarcador.Cargas.CargaDocumentoParaEmissaoNFSManual> cargaDocumentoParaEmissaoNFSManuals = repCargaDocumentoParaEmissaoNFSManual.BuscarPorLancamentoNFsManual(pagamentoSumarizada.LancamentoNFSManual.Value);

                for (int i = 0; i < cargaDocumentoParaEmissaoNFSManuals.Count; i++)
                {
                    Dominio.Entidades.Embarcador.Cargas.CargaDocumentoParaEmissaoNFSManual cargaDocumentoParaEmissaoNFSManual = cargaDocumentoParaEmissaoNFSManuals[i];
                    Dominio.ObjetosDeValor.Embarcador.Escrituracao.DocumentosPagamento documentoPagamento = new Dominio.ObjetosDeValor.Embarcador.Escrituracao.DocumentosPagamento();
                    documentoPagamento.pedidoCTeParaSubContratacao = cargaDocumentoParaEmissaoNFSManual.PedidoCTeParaSubContratacao;
                    documentoPagamento.pedidoXMLNotaFiscal = cargaDocumentoParaEmissaoNFSManual.PedidoXMLNotaFiscal;
                    documentoPagamento.cargaDocumentoParaEmissaoNFSManual = cargaDocumentoParaEmissaoNFSManual;
                    documentoPagamento.Carga = cargaDocumentoParaEmissaoNFSManual.CargaOrigem.Codigo;
                    documentoPagamento.Ocorrencia = cargaDocumentoParaEmissaoNFSManual.CargaOcorrencia?.Codigo;
                    documentosPagamentos.Add(documentoPagamento);
                }
            }
            else
            {
                List<Dominio.Entidades.DocumentoDeTransporteAnteriorCTe> documentoDeTransporteAnteriorCTes = repDocumentoDeTransporteAnteriorCTe.BuscarPorCTe(pagamentoSumarizada.CodigoCTe);
                if ((documentoDeTransporteAnteriorCTes.Count > 0 || (!string.IsNullOrWhiteSpace(pagamentoSumarizada.ChaveCTeComplementado) && pagamentoSumarizada.TipoCTe != TipoCTE.Substituto && !pagamentoSumarizada.OcorrenciaPagamento.HasValue)) && !pagamentoSumarizada.ProvisaoPorNotaFiscal)
                {
                    if (documentoDeTransporteAnteriorCTes.Count > 0)
                    {
                        foreach (Dominio.Entidades.DocumentoDeTransporteAnteriorCTe cteAnterior in documentoDeTransporteAnteriorCTes)
                        {
                            List<Dominio.Entidades.Embarcador.Pedidos.PedidoCTeParaSubContratacao> pedidosCTeParaSubContratacao = repPedidoCTeParaSubContratacao.BuscarListaPorChaveECarga(pagamentoSumarizada.CargaPagamento.Value, cteAnterior.Chave);
                            foreach (Dominio.Entidades.Embarcador.Pedidos.PedidoCTeParaSubContratacao pedidoCTeParaSubContratacao in pedidosCTeParaSubContratacao)
                            {
                                Dominio.ObjetosDeValor.Embarcador.Escrituracao.DocumentosPagamento documentoPagamento = new Dominio.ObjetosDeValor.Embarcador.Escrituracao.DocumentosPagamento();
                                documentoPagamento.pedidoCTeParaSubContratacao = pedidoCTeParaSubContratacao;
                                documentoPagamento.Carga = pagamentoSumarizada.CargaPagamento;
                                documentoPagamento.Ocorrencia = pagamentoSumarizada.OcorrenciaPagamento;
                                documentosPagamentos.Add(documentoPagamento);
                            }
                            //Dominio.Entidades.Embarcador.Pedidos.PedidoCTeParaSubContratacao pedidoCTeParaSubContratacao = repPedidoCTeParaSubContratacao.BuscarPorChaveECarga(pagamentoSumarizada.CargaPagamento.Value, cteAnterior.Chave);
                            //if (pedidoCTeParaSubContratacao != null)
                            //{
                            //    Dominio.ObjetosDeValor.Embarcador.Escrituracao.DocumentosPagamento documentoPagamento = new Dominio.ObjetosDeValor.Embarcador.Escrituracao.DocumentosPagamento();
                            //    documentoPagamento.pedidoCTeParaSubContratacao = pedidoCTeParaSubContratacao;
                            //    documentoPagamento.Carga = pagamentoSumarizada.CargaPagamento;
                            //    documentoPagamento.Ocorrencia = pagamentoSumarizada.OcorrenciaPagamento;
                            //    documentosPagamentos.Add(documentoPagamento);
                            //}
                        }
                    }
                    else
                    {
                        Dominio.Entidades.Embarcador.Pedidos.PedidoCTeParaSubContratacao pedidoCTeParaSubContratacao = repPedidoCTeParaSubContratacao.BuscarPorChaveECarga(pagamentoSumarizada.CargaPagamento.Value, pagamentoSumarizada.ChaveCTeComplementado);
                        if (pedidoCTeParaSubContratacao != null)
                        {
                            Dominio.ObjetosDeValor.Embarcador.Escrituracao.DocumentosPagamento documentoPagamento = new Dominio.ObjetosDeValor.Embarcador.Escrituracao.DocumentosPagamento();
                            documentoPagamento.pedidoCTeParaSubContratacao = pedidoCTeParaSubContratacao;
                            documentoPagamento.Carga = pagamentoSumarizada.CargaPagamento;
                            documentoPagamento.Ocorrencia = pagamentoSumarizada.OcorrenciaPagamento;
                            documentosPagamentos.Add(documentoPagamento);
                        }
                    }
                }
                else
                {
                    if (pagamentoSumarizada.CargaPagamento != null)
                    {
                        List<int> codigosXMLNota = repCTe.BuscarXMLNotaFiscalPorId(pagamentoSumarizada.CodigoCTe);
                        foreach (int codigoXMLNota in codigosXMLNota)
                        {
                            Dominio.Entidades.Embarcador.Pedidos.PedidoXMLNotaFiscal pedidoXMLNotaFiscal = null;
                            pedidoXMLNotaFiscal = repPedidoXMLNotaFiscal.BuscarPorXMLNotaFiscalECargaOrigem(codigoXMLNota, pagamentoSumarizada.CargaPagamento.Value);

                            if (pedidoXMLNotaFiscal == null && repCarga.IsCargaAgrupada(pagamentoSumarizada.CargaPagamento.Value))
                                pedidoXMLNotaFiscal = repPedidoXMLNotaFiscal.BuscarPorXMLNotaFiscalECarga(codigoXMLNota, pagamentoSumarizada.CargaPagamento.Value);

                            if (pedidoXMLNotaFiscal != null)
                            {
                                Dominio.ObjetosDeValor.Embarcador.Escrituracao.DocumentosPagamento documentoPagamento = new Dominio.ObjetosDeValor.Embarcador.Escrituracao.DocumentosPagamento();
                                documentoPagamento.pedidoXMLNotaFiscal = pedidoXMLNotaFiscal;
                                documentoPagamento.Carga = pagamentoSumarizada.CargaPagamento;
                                documentoPagamento.Ocorrencia = pagamentoSumarizada.OcorrenciaPagamento;
                                documentosPagamentos.Add(documentoPagamento);
                            }
                        }
                    }
                }
            }

            List<Dominio.Entidades.Embarcador.Escrituracao.DocumentoProvisao> documentoProvisaoEstornar = new List<Dominio.Entidades.Embarcador.Escrituracao.DocumentoProvisao>();
            List<int> cargasNotasServico = (from obj in documentosPagamentos where obj.cargaDocumentoParaEmissaoNFSManual != null && obj.Ocorrencia == null select obj.cargaDocumentoParaEmissaoNFSManual.CargaOrigem.Codigo).Distinct().ToList();
            List<Dominio.Entidades.Embarcador.Escrituracao.DocumentoProvisao> documentosProvisoesNotasManuais = new List<Dominio.Entidades.Embarcador.Escrituracao.DocumentoProvisao>();

            if (cargasNotasServico.Count > 0)
                documentosProvisoesNotasManuais = repDocumentoProvisao.BuscarCodigosPorProvisaoParaPagamentoPorCargas(cargasNotasServico);

            int indice = 0;
            bool existeRegraComponentesDeFreteDoTipoDescontoNaoDevemSomarNaContabilizacao = configuracoesContabeis.Exists(configuracao => configuracao.ComponentesDeFreteDoTipoDescontoNaoDevemSomar);

            foreach (Dominio.ObjetosDeValor.Embarcador.Escrituracao.DocumentosPagamento documentoPagamento in documentosPagamentos)
            {
                indice++;

                Dominio.Entidades.Embarcador.Escrituracao.DocumentoProvisao documentoProvisao = null;

                if (documentoPagamento.Fechamento.HasValue)
                {
                    Servicos.Log.TratarErro($"Pagamento {pagamento.Codigo} - {indice} procurando provisao pelo fechamento", "FechamentoPagamento");
                    documentoProvisao = repDocumentoProvisao.BuscarPorCTeEFechamento(documentoPagamento.CTe.Codigo, documentoPagamento.Fechamento.Value);
                }
                else if (documentoPagamento.Ocorrencia.HasValue)
                {
                    if (documentoPagamento.pedidoXMLNotaFiscal != null || documentoPagamento.pedidoCTeParaSubContratacao != null)
                    {
                        Servicos.Log.TratarErro($"Pagamento {pagamento.Codigo} - {indice} procurando provisao pela ocorrencia 01", "FechamentoPagamento");
                        documentoProvisao = repDocumentoProvisao.BuscarCodigosPorProvisaoParaPagamento(documentoPagamento.pedidoXMLNotaFiscal?.XMLNotaFiscal.Codigo ?? 0, documentoPagamento.pedidoCTeParaSubContratacao?.CTeTerceiro.Codigo ?? 0, documentoPagamento.pedidoCTeParaSubContratacao?.Codigo ?? 0, 0, documentoPagamento.Ocorrencia.Value, permitirProvisoesEstornadas);
                    }
                    else
                    {
                        Servicos.Log.TratarErro($"Pagamento {pagamento.Codigo} - {indice} procurando provisao pela ocorrencia 02", "FechamentoPagamento");
                        documentoProvisao = repDocumentoProvisao.BuscarCodigosPorOcorrenciaNFSParaPagamento(documentoPagamento.Ocorrencia.Value);
                    }
                }
                else if (documentoPagamento.cargaDocumentoParaEmissaoNFSManual != null)
                {
                    if (documentoPagamento.pedidoXMLNotaFiscal != null)
                    {
                        Servicos.Log.TratarErro($"Pagamento {pagamento.Codigo} - {indice} procurando provisao sem ocorrencia PedidoNotaFiscal - NFS Manual", "FechamentoPagamento");
                        documentoProvisao = (from obj in documentosProvisoesNotasManuais where obj.XMLNotaFiscal != null && obj.XMLNotaFiscal.Codigo == documentoPagamento.pedidoXMLNotaFiscal.XMLNotaFiscal.Codigo && obj.Carga.Codigo == documentoPagamento.Carga select obj).FirstOrDefault();
                    }
                    else
                    {
                        Servicos.Log.TratarErro($"Pagamento {pagamento.Codigo} - {indice} procurando provisao sem ocorrencia PedidoCTeSub - NFS Manual", "FechamentoPagamento");
                        documentoProvisao = (from obj in documentosProvisoesNotasManuais where obj.PedidoCTeParaSubContratacao != null && obj.PedidoCTeParaSubContratacao.Codigo == documentoPagamento.pedidoCTeParaSubContratacao.Codigo select obj).FirstOrDefault();
                    }
                }
                else
                {
                    Servicos.Log.TratarErro($"Pagamento {pagamento.Codigo} - {indice} procurando provisao sem ocorrencia", "FechamentoPagamento");
                    documentoProvisao = repDocumentoProvisao.BuscarCodigosPorProvisaoParaPagamento(documentoPagamento.pedidoXMLNotaFiscal?.XMLNotaFiscal.Codigo ?? 0, documentoPagamento.pedidoCTeParaSubContratacao?.CTeTerceiro.Codigo ?? 0, documentoPagamento.pedidoCTeParaSubContratacao?.Codigo ?? 0, documentoPagamento.Carga.Value, 0, permitirProvisoesEstornadas);
                }

                if (documentoProvisao != null)
                {
                    Servicos.Log.TratarErro($"Pagamento {pagamento.Codigo} - Encontrou provisão {documentoProvisao.Codigo}", "FechamentoPagamento");

                    List<Dominio.Entidades.Embarcador.ConfiguracaoContabil.DocumentoContabil> documentosContabeisProvisao = new List<Dominio.Entidades.Embarcador.ConfiguracaoContabil.DocumentoContabil>();

                    if (documentoProvisao.Situacao == SituacaoProvisaoDocumento.Provisionado && !disponbilizarProvisaoContraPartidaParaCancelamento)
                        documentosContabeisProvisao = repDocumentoContabil.BuscarPorDocumentoProvisao(documentoProvisao.Codigo);


                    foreach (Dominio.ObjetosDeValor.Embarcador.ConfiguracaoContabil.ConfiguracaoContabil configuracaoContabil in configuracoesContabeis)
                    {
                        Dominio.Entidades.Embarcador.ConfiguracaoContabil.DocumentoContabil documentoContabil = new Dominio.Entidades.Embarcador.ConfiguracaoContabil.DocumentoContabil();

                        documentoContabil.Carga = documentoProvisao.Carga;
                        documentoContabil.FechamentoFrete = documentoProvisao.FechamentoFrete;
                        documentoContabil.DocumentoProvisaoReferencia = documentoProvisao;
                        documentoContabil.CargaOcorrencia = documentoProvisao.CargaOcorrencia;

                        if (codigoCentroResultado > 0)
                            documentoContabil.CentroResultado = new Dominio.Entidades.Embarcador.Financeiro.CentroResultado() { Codigo = codigoCentroResultado };

                        documentoContabil.CargaDocumentoParaEmissaoNFSManual = documentoPagamento.cargaDocumentoParaEmissaoNFSManual;
                        documentoContabil.CTeTerceiro = documentoPagamento.pedidoCTeParaSubContratacao?.CTeTerceiro;
                        documentoContabil.PedidoCTeParaSubContratacao = documentoPagamento.pedidoCTeParaSubContratacao;
                        documentoContabil.DataEmissaoCTe = pagamentoSumarizada.DataEmissao ?? DateTime.Now;
                        documentoContabil.DataEmissao = documentoProvisao.DataEmissao;
                        documentoContabil.DataLancamento = pagamento.DataCriacao;
                        documentoContabil.DataRegistro = DateTime.Now;
                        documentoContabil.Pagamento = pagamento;
                        documentoContabil.CTe = new Dominio.Entidades.ConhecimentoDeTransporteEletronico() { Codigo = pagamentoSumarizada.CodigoCTe };
                        documentoContabil.Empresa = new Dominio.Entidades.Empresa() { Codigo = pagamentoSumarizada.Empresa };
                        documentoContabil.Filial = documentoProvisao.Filial;
                        documentoContabil.ModeloDocumentoFiscal = documentoProvisao.ModeloDocumentoFiscal;
                        documentoContabil.NumeroDocumento = documentoProvisao.NumeroDocumento;
                        documentoContabil.Remetente = documentoProvisao.Remetente;
                        documentoContabil.Destinatario = documentoProvisao.Destinatario;
                        documentoContabil.Origem = documentoProvisao.Origem;
                        documentoContabil.Destino = documentoProvisao.Destino;
                        documentoContabil.PesoBruto = documentoProvisao.PesoBruto;
                        documentoContabil.TipoOperacao = pagamentoSumarizada.TipoOperacao.HasValue ? new Dominio.Entidades.Embarcador.Pedidos.TipoOperacao() { Codigo = pagamentoSumarizada.TipoOperacao.Value } : null;
                        documentoContabil.Tomador = new Dominio.Entidades.Cliente() { CPF_CNPJ = pagamentoSumarizada.Tomador };
                        documentoContabil.XMLNotaFiscal = documentoPagamento.pedidoXMLNotaFiscal?.XMLNotaFiscal;
                        documentoContabil.ImpostoValorAgregado = documentoProvisao.ImpostoValorAgregado;
                        documentoContabil.Stage = documentoProvisao.Stage;
                        documentoContabil.PlanoConta = new Dominio.Entidades.Embarcador.Financeiro.PlanoConta() { Codigo = configuracaoContabil.PlanoConta };
                        documentoContabil.SerieDocumento = documentoProvisao.SerieDocumento;
                        documentoContabil.Situacao = SituacaoDocumentoContabil.AgConsolidacao;
                        documentoContabil.TipoContabilizacao = configuracaoContabil.TipoContabilizacao;
                        documentoContabil.TipoContaContabil = configuracaoContabil.TipoContaContabil;
                        documentoContabil.AliquotaPis = aliquotaPIS;
                        documentoContabil.AliquotaCofins = aliquotaCOFINS;
                        documentoContabil.AliquotaIss = documentoPagamento.AliquotaISS;
                        documentoContabil.OutrasAliquotas = documentoProvisao.OutrasAliquotas;
                        documentoContabil.AliquotaCBS = documentoProvisao.AliquotaCBS;
                        documentoContabil.AliquotaIBSEstadual = documentoProvisao.AliquotaIBSEstadual;
                        documentoContabil.AliquotaIBSMunicipal = documentoProvisao.AliquotaIBSMunicipal;

                        decimal valorICMS = documentoPagamento.CST != "60" ? documentoPagamento.ValorICMS : 0;
                        decimal valorICMSST = documentoPagamento.CST == "60" ? documentoPagamento.ValorICMS : 0;
                        decimal valorISS = documentoPagamento.ValorISS - documentoPagamento.ValorRetencaoISS;
                        decimal valorISSRetido = documentoPagamento.ValorRetencaoISS;
                        decimal totalReceber = 0;
                        decimal totalPrestacao = 0;
                        bool issInclusoBC = false;

                        bool provisaoNotaServico = (
                            (pagamentoSumarizada.TipoDocumentoEmissao == Dominio.Enumeradores.TipoDocumento.NFS) ||
                            (pagamentoSumarizada.TipoDocumentoEmissao == Dominio.Enumeradores.TipoDocumento.NFSe) ||
                            ((pagamentoSumarizada.TipoDocumentoEmissao == Dominio.Enumeradores.TipoDocumento.Outros) && ((valorISS + valorISSRetido) > 0m))
                        );

                        if (provisaoNotaServico)
                        {
                            issInclusoBC = documentoPagamento.ISSInclusoBC;
                            totalReceber = documentoPagamento.ISSInclusoBC ? documentoPagamento.Valor + valorISS : (documentoPagamento.Valor - valorISSRetido);
                            totalPrestacao = totalReceber + valorISSRetido;
                        }
                        else
                        {
                            totalPrestacao = documentoPagamento.ICMSInclusoBC ? documentoPagamento.Valor + documentoPagamento.ValorICMS : documentoPagamento.Valor;

                            if (documentoPagamento.CST != "60")
                                totalReceber = documentoPagamento.ICMSInclusoBC ? documentoPagamento.Valor + documentoPagamento.ValorICMS : documentoPagamento.Valor;
                            else
                                totalReceber = documentoPagamento.Valor;
                        }

                        //se for complementar não muda o valor em relação ao provisionado nunca.
                        if (pagamentoSumarizada.OcorrenciaPagamento.HasValue)
                        {
                            documentoContabil.AliquotaIss = documentoProvisao.PercentualAliquotaISS;
                            valorICMS = documentoProvisao.CST != "60" ? documentoProvisao.ValorICMS : 0;
                            valorICMSST = documentoProvisao.CST == "60" ? documentoProvisao.ValorICMS : 0;
                            valorISS = documentoProvisao.ValorISS - documentoProvisao.ValorRetencaoISS;
                            valorISSRetido = documentoProvisao.ValorRetencaoISS;

                            provisaoNotaServico = (
                                (documentoProvisao.ModeloDocumentoFiscal.TipoDocumentoEmissao == Dominio.Enumeradores.TipoDocumento.NFS) ||
                                (documentoProvisao.ModeloDocumentoFiscal.TipoDocumentoEmissao == Dominio.Enumeradores.TipoDocumento.NFSe) ||
                                ((documentoProvisao.ModeloDocumentoFiscal.TipoDocumentoEmissao == Dominio.Enumeradores.TipoDocumento.Outros) && ((valorISS + valorISSRetido) > 0m))
                            );

                            if (provisaoNotaServico)
                            {
                                issInclusoBC = documentoProvisao.ISSInclusoBC;
                                totalReceber = documentoProvisao.ISSInclusoBC ? documentoProvisao.ValorProvisao + valorISS : (documentoProvisao.ValorProvisao - valorISSRetido);
                                totalPrestacao = totalReceber + valorISSRetido;
                            }
                            else
                            {
                                totalPrestacao = documentoProvisao.ICMSInclusoBC ? documentoProvisao.ValorProvisao + documentoProvisao.ValorICMS : documentoProvisao.ValorProvisao;

                                if (documentoProvisao.CST != "60")
                                    totalReceber = documentoProvisao.ICMSInclusoBC ? documentoProvisao.ValorProvisao + documentoProvisao.ValorICMS : documentoProvisao.ValorProvisao;
                                else
                                    totalReceber = documentoProvisao.ValorProvisao;
                            }
                        }
                        decimal basePisCofins = ObterBasePisCofins(documentoProvisao, documentoPagamento, configuracaoFinanceiro.NaoIncluirICMSBaseCalculoPisCofins, existeRegraComponentesDeFreteDoTipoDescontoNaoDevemSomarNaContabilizacao, valorICMS, totalPrestacao);

                        decimal valorCOFINS = Math.Round(basePisCofins * (aliquotaCOFINS / 100), 2, MidpointRounding.AwayFromZero);
                        decimal valorPIS = Math.Round(basePisCofins * (aliquotaPIS / 100), 2, MidpointRounding.AwayFromZero);
                        decimal valorProvisionado = 0m;

                        if (configuracaoContabil.PlanoContaContraPartida > 0)
                            valorProvisionado = (from obj in documentosContabeisProvisao where obj.PlanoConta.Codigo == configuracaoContabil.PlanoContaContraPartida && obj.TipoContaContabil == configuracaoContabil.TipoContaContabil select obj.ValorContabilizacao).Sum();

                        if (configuracaoContabil.TipoContaContabil == TipoContaContabil.ICMS)
                        {
                            DefinirValorContabilizacao(documentoContabil, valorICMS, valorProvisionado);

                            if (documentoContabil.ValorContabilizacao == 0)
                                continue;
                        }
                        else if (configuracaoContabil.TipoContaContabil == TipoContaContabil.ICMSST)
                        {
                            DefinirValorContabilizacao(documentoContabil, valorICMSST, valorProvisionado);

                            if (documentoContabil.ValorContabilizacao == 0)
                                continue;
                        }
                        else if (configuracaoContabil.TipoContaContabil == TipoContaContabil.ISS)
                        {
                            DefinirValorContabilizacao(documentoContabil, valorISS, valorProvisionado);

                            if (documentoContabil.ValorContabilizacao == 0)
                                continue;
                        }
                        else if (configuracaoContabil.TipoContaContabil == TipoContaContabil.ISSRetido)
                        {
                            DefinirValorContabilizacao(documentoContabil, valorISSRetido, valorProvisionado);

                            if (documentoContabil.ValorContabilizacao == 0)
                                continue;
                        }
                        else if (configuracaoContabil.TipoContaContabil == TipoContaContabil.PIS)
                        {
                            DefinirValorContabilizacao(documentoContabil, valorPIS, valorProvisionado);

                            if (documentoContabil.ValorContabilizacao == 0)
                                continue;
                        }
                        else if (configuracaoContabil.TipoContaContabil == TipoContaContabil.COFINS)
                        {
                            DefinirValorContabilizacao(documentoContabil, valorCOFINS, valorProvisionado);

                            if (documentoContabil.ValorContabilizacao == 0)
                                continue;
                        }
                        else if (configuracaoContabil.TipoContaContabil == TipoContaContabil.CBS)
                        {
                            DefinirValorContabilizacao(documentoContabil, documentoProvisao.ValorCBS, valorProvisionado);

                            if (documentoContabil.ValorContabilizacao == 0)
                                continue;
                        }
                        else if (configuracaoContabil.TipoContaContabil == TipoContaContabil.IBSEstadual)
                        {
                            DefinirValorContabilizacao(documentoContabil, documentoProvisao.ValorIBSEstadual, valorProvisionado);

                            if (documentoContabil.ValorContabilizacao == 0)
                                continue;
                        }
                        else if (configuracaoContabil.TipoContaContabil == TipoContaContabil.IBSMunicipal)
                        {
                            DefinirValorContabilizacao(documentoContabil, documentoProvisao.ValorIBSMunicipal, valorProvisionado);

                            if (documentoContabil.ValorContabilizacao == 0)
                                continue;
                        }
                        else if (configuracaoContabil.TipoContaContabil == TipoContaContabil.FreteLiquido || configuracaoContabil.TipoContaContabil == TipoContaContabil.FreteLiquido2 || configuracaoContabil.TipoContaContabil == TipoContaContabil.FreteLiquido9)
                        {
                            decimal freteLiquido = totalReceber - valorPIS - valorCOFINS - valorICMS;

                            if (configuracaoFinanceiro.SomarValorISSNoTotalReceberGeracaoLoteProvisao)
                                freteLiquido += ((provisaoNotaServico && issInclusoBC ? 0 : valorISS) + valorISSRetido);
                            else
                                freteLiquido += valorISSRetido;

                            if (configuracaoContabil.ComponentesDeFreteDoTipoDescontoNaoDevemSomar)
                                freteLiquido = Math.Max(0, freteLiquido - documentoProvisao.ValorDesconto);

                            DefinirValorContabilizacao(documentoContabil, freteLiquido, valorProvisionado);

                            if (documentoContabil.ValorContabilizacao == 0)
                                continue;
                        }
                        else if (configuracaoContabil.TipoContaContabil == TipoContaContabil.FreteLiquidoSemComponentesFrete)
                        {
                            decimal freteLiquido = totalReceber - valorPIS - valorCOFINS - valorICMS;

                            if (configuracaoFinanceiro.SomarValorISSNoTotalReceberGeracaoLoteProvisao)
                                freteLiquido += ((provisaoNotaServico && issInclusoBC ? 0 : valorISS) + valorISSRetido);
                            else
                                freteLiquido += valorISSRetido;

                            freteLiquido -= (documentoProvisao.ValorAdValorem + documentoProvisao.ValorDescarga + documentoProvisao.ValorPedagio + documentoProvisao.ValorGris + documentoProvisao.ValorEntrega + documentoProvisao.ValorPernoite);

                            DefinirValorContabilizacao(documentoContabil, freteLiquido, valorProvisionado);

                            if (documentoContabil.ValorContabilizacao == 0)
                                continue;
                        }
                        else if (configuracaoContabil.TipoContaContabil == TipoContaContabil.TotalReceber)
                        {
                            decimal valorTotalReceber = totalReceber;

                            if (configuracaoFinanceiro.SomarValorISSNoTotalReceberGeracaoLoteProvisao)
                                valorTotalReceber += ((provisaoNotaServico && issInclusoBC ? 0 : valorISS) + valorISSRetido);

                            if (configuracaoContabil.ComponentesDeFreteDoTipoDescontoNaoDevemSomar)
                                valorTotalReceber = Math.Max(0, valorTotalReceber - documentoProvisao.ValorDesconto);

                            DefinirValorContabilizacao(documentoContabil, valorTotalReceber, valorProvisionado);

                            if (documentoContabil.ValorContabilizacao == 0)
                                continue;
                        }
                        else if (configuracaoContabil.TipoContaContabil == TipoContaContabil.FreteValor)
                        {
                            decimal freteValor = totalReceber - valorICMS - valorISS + valorISSRetido;
                            DefinirValorContabilizacao(documentoContabil, freteValor, valorProvisionado);

                            if (documentoContabil.ValorContabilizacao == 0)
                                continue;
                        }
                        else if (configuracaoContabil.TipoContaContabil == TipoContaContabil.TotalReceberSemISS)
                        {
                            decimal valorTotalReceber = totalReceber - (issInclusoBC ? valorISS : 0);

                            DefinirValorContabilizacao(documentoContabil, valorTotalReceber, valorProvisionado);

                            if (documentoContabil.ValorContabilizacao == 0)
                                continue;
                        }
                        else if (configuracaoContabil.TipoContaContabil == TipoContaContabil.AdValorem)
                        {
                            DefinirValorContabilizacao(documentoContabil, documentoProvisao.ValorAdValorem, valorProvisionado);

                            if (documentoContabil.ValorContabilizacao == 0)
                                continue;
                        }
                        else if (configuracaoContabil.TipoContaContabil == TipoContaContabil.GRIS)
                        {
                            DefinirValorContabilizacao(documentoContabil, documentoProvisao.ValorGris, valorProvisionado);

                            if (documentoContabil.ValorContabilizacao == 0)
                                continue;
                        }
                        else if (configuracaoContabil.TipoContaContabil == TipoContaContabil.Pedagio)
                        {
                            DefinirValorContabilizacao(documentoContabil, documentoProvisao.ValorPedagio, valorProvisionado);

                            if (documentoContabil.ValorContabilizacao == 0)
                                continue;
                        }
                        else if (configuracaoContabil.TipoContaContabil == TipoContaContabil.TaxaDescarga)
                        {
                            DefinirValorContabilizacao(documentoContabil, documentoProvisao.ValorDescarga, valorProvisionado);

                            if (documentoContabil.ValorContabilizacao == 0)
                                continue;
                        }
                        else if (configuracaoContabil.TipoContaContabil == TipoContaContabil.TaxaEntrega)
                        {
                            DefinirValorContabilizacao(documentoContabil, documentoProvisao.ValorEntrega, valorProvisionado);

                            if (documentoContabil.ValorContabilizacao == 0)
                                continue;
                        }
                        else if (configuracaoContabil.TipoContaContabil == TipoContaContabil.ImpostoValorAgregado)
                        {
                            continue;
                        }
                        else if (configuracaoContabil.TipoContaContabil == TipoContaContabil.CustoFixo)
                        {
                            DefinirValorContabilizacao(documentoContabil, documentoProvisao.ValorContratoFrete, valorProvisionado);

                            if (documentoContabil.ValorContabilizacao == 0)
                                continue;
                        }
                        else if (configuracaoContabil.TipoContaContabil == TipoContaContabil.FreteCaixa)
                        {
                            DefinirValorContabilizacao(documentoContabil, ((documentoProvisao.TipoValorFrete == TipoValorFreteDocumentoProvisao.TipoEmbalagem) ? documentoProvisao.ValorFrete : 0m), valorProvisionado);

                            if (documentoContabil.ValorContabilizacao == 0)
                                continue;
                        }
                        else if (configuracaoContabil.TipoContaContabil == TipoContaContabil.FreteKM)
                        {
                            DefinirValorContabilizacao(documentoContabil, ((documentoProvisao.TipoValorFrete == TipoValorFreteDocumentoProvisao.Distancia) ? documentoProvisao.ValorFrete : 0m), valorProvisionado);

                            if (documentoContabil.ValorContabilizacao == 0)
                                continue;
                        }
                        else if (configuracaoContabil.TipoContaContabil == TipoContaContabil.FretePeso)
                        {
                            DefinirValorContabilizacao(documentoContabil, ((documentoProvisao.TipoValorFrete == TipoValorFreteDocumentoProvisao.Peso) ? documentoProvisao.ValorFrete : 0m), valorProvisionado);

                            if (documentoContabil.ValorContabilizacao == 0)
                                continue;
                        }
                        else if (configuracaoContabil.TipoContaContabil == TipoContaContabil.FreteViagem)
                        {
                            DefinirValorContabilizacao(documentoContabil, ((documentoProvisao.TipoValorFrete == TipoValorFreteDocumentoProvisao.TipoCarga) ? documentoProvisao.ValorFrete : 0m), valorProvisionado);

                            if (documentoContabil.ValorContabilizacao == 0)
                                continue;
                        }
                        else if (configuracaoContabil.TipoContaContabil == TipoContaContabil.TaxaTotal)
                        {
                            decimal taxaTotal = valorICMS + valorPIS + valorCOFINS + valorICMSST;
                            DefinirValorContabilizacao(documentoContabil, taxaTotal, valorProvisionado);

                            if (documentoContabil.ValorContabilizacao == 0)
                                continue;
                        }
                        else if (configuracaoContabil.TipoContaContabil == TipoContaContabil.Pernoite)
                        {
                            DefinirValorContabilizacao(documentoContabil, documentoProvisao.ValorPernoite, valorProvisionado);

                            if (documentoContabil.ValorContabilizacao == 0)
                                continue;
                        }

                        Servicos.Log.TratarErro($"Pagamento {pagamento.Codigo} - Inseriu documento contabil da provisao {documentoProvisao.Codigo}", "FechamentoPagamento");

                        documentoContabil.DocumentoFaturamento = new Dominio.Entidades.Embarcador.Financeiro.DocumentoFaturamento() { Codigo = pagamentoSumarizada.Codigo };
                        repDocumentoContabil.Inserir(documentoContabil);
                    }

                    documentoProvisao.Pagamento = pagamento;
                    documentoProvisao.DocumentoFaturamento = new Dominio.Entidades.Embarcador.Financeiro.DocumentoFaturamento() { Codigo = pagamentoSumarizada.Codigo };
                    repDocumentoProvisao.Atualizar(documentoProvisao);
                }
                else
                {
                    Servicos.Log.TratarErro($"Pagamento {pagamento.Codigo} - Provisão não encontrada", "FechamentoPagamento");
                }
            }

            if (tipoServicoMultisoftware != AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiTMS)
            {
                // Título Pagamento
                Dominio.Entidades.Embarcador.Financeiro.Titulo titulo = new Dominio.Entidades.Embarcador.Financeiro.Titulo
                {
                    Pagamento = pagamento,
                    LiberadoPagamento = false,
                    DataVencimento = CalculaDataPagamento(condicaoPagamento, pagamentoSumarizada.DataEmissao ?? DateTime.Now, pagamento.DataCriacao, unidadeTrabalho),
                    DataProgramacaoPagamento = CalculaDataPagamento(condicaoPagamento, pagamentoSumarizada.DataEmissao ?? DateTime.Now, pagamento.DataCriacao, unidadeTrabalho),
                    Empresa = pagamento.Empresa,
                    Observacao = "",
                    Pessoa = pagamento.Tomador,
                    GrupoPessoas = pagamento.Tomador?.GrupoPessoas ?? null,
                    Sequencia = 1,
                    StatusTitulo = Dominio.ObjetosDeValor.Embarcador.Enumeradores.StatusTitulo.EmAberto,
                    TipoTitulo = Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoTitulo.Receber,
                    ValorOriginal = pagamento.ValorPagamento,
                    ValorPendente = pagamento.ValorPagamento,
                    DataAlteracao = DateTime.Now,
                    DataLancamento = DateTime.Now,
                    Usuario = pagamento.Usuario,
                };

                repTitulo.Inserir(titulo);
                repCTe.SetaTitulo(pagamentoSumarizada.CodigoCTe, titulo.Codigo);
            }

        }

        private static decimal ObterBasePisCofins(Dominio.Entidades.Embarcador.Escrituracao.DocumentoProvisao documentoProvisao, Dominio.ObjetosDeValor.Embarcador.Escrituracao.DocumentosPagamento documentoPagamento, bool naoIncluirICMSBaseCalculoPisCofins, bool componentesDeFreteDoTipoDescontoNaoDevemSomarNaContabilizacao, decimal valorICMS, decimal totalPrestacao)
        {
            decimal basePisCofins = totalPrestacao;

            if (naoIncluirICMSBaseCalculoPisCofins && documentoPagamento.ICMSInclusoBC)
                basePisCofins = totalPrestacao - valorICMS;

            if (componentesDeFreteDoTipoDescontoNaoDevemSomarNaContabilizacao)
                basePisCofins = Math.Max(0, basePisCofins - documentoProvisao.ValorDesconto);

            return basePisCofins;
        }

        private static void DefinirValorContabilizacao(Dominio.Entidades.Embarcador.ConfiguracaoContabil.DocumentoContabil documentoContabil, decimal valorContabilizacao, decimal valorProvisionado)
        {
            documentoContabil.ValorContabilizacao = valorContabilizacao;

            if (valorProvisionado > 0m)
            {
                documentoContabil.ValorContabilizacao -= valorProvisionado;

                if (documentoContabil.ValorContabilizacao < 0m)
                {
                    documentoContabil.TipoContabilizacao = (documentoContabil.TipoContabilizacao == TipoContabilizacao.Credito) ? TipoContabilizacao.Debito : TipoContabilizacao.Credito;
                    documentoContabil.ValorContabilizacao = Math.Abs(documentoContabil.ValorContabilizacao);
                }
            }
        }

        private static DateTime? CalculaDataPagamento(Dominio.ObjetosDeValor.Embarcador.Escrituracao.CondicaoPagamento condicao, DateTime dataDocumento, DateTime dataPagamento, Repositorio.UnitOfWork unitOfWork)
        {
            DateTime database;

            if (condicao == null)
                return null;

            if (condicao.TipoPrazoPagamento == Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoPrazoPagamento.DataDocumento)
                database = dataDocumento;
            else
                database = dataPagamento;

            // 1 - Se pagamento for fora do mês, data base passa a ser o primeiro dia do próximo mês
            if (condicao.VencimentoForaMes)
                database = database.AddMonths(1).AddDays(-database.Day + 1);

            // 2 - Adiciona o tempo do prazo para pagamento
            database = database.AddDays(condicao.DiasDePrazoPagamento ?? 0);

            // 3 - Define pagamento como dia de semana ou dia mes (na prática, somente uma dessas vai set definida)
            int diaSemanaCondicao = condicao.DiaSemana.HasValue ? (int)condicao.DiaSemana.Value : 0;
            int diaSemanaPagamento = (int)database.DayOfWeek + 1; //DayOfWeek começa em 0 e DiaSemana em 1
            if (condicao.DiaSemana.HasValue && diaSemanaCondicao != diaSemanaPagamento)
            {
                int diff = 0;
                if (diaSemanaCondicao > diaSemanaPagamento)
                    diff = diaSemanaCondicao - diaSemanaPagamento;
                else
                    diff = 7 - (diaSemanaPagamento - diaSemanaCondicao);

                database = database.AddDays(diff);
            }

            // 3 - Define pagamento como dia de semana ou dia mes (na prática, somente uma dessas vai set definida)
            int diaMesCondicao = condicao.DiaMes ?? 0;
            if (diaMesCondicao > 0 && diaMesCondicao != database.Day)
            {
                if (diaMesCondicao < database.Day)
                    database = database.AddDays(diaMesCondicao - database.Day).AddMonths(1);
                else
                    database = database.AddDays(diaMesCondicao - database.Day);
            }

            if (condicao?.ConsiderarDiaUtilVencimento ?? false)
            {
                bool dataValida = true;
                Configuracoes.Feriado servicoFeriado = new Configuracoes.Feriado(unitOfWork);

                while (dataValida)
                {
                    if (database.DayOfWeek == DayOfWeek.Saturday || database.DayOfWeek == DayOfWeek.Sunday)
                        database = database.AddDays(1);
                    else if (servicoFeriado.VerificarSePossuiFeriado(database))
                        database = database.AddDays(1);
                    else
                        dataValida = false;
                }
                ;
            }

            return database;
        }

        private static Dominio.ObjetosDeValor.Embarcador.Escrituracao.CondicaoPagamento ObterCondicaoPagamento(Dominio.Entidades.Embarcador.Escrituracao.Pagamento pagamento, Dominio.ObjetosDeValor.Embarcador.Escrituracao.PagamentoSumarizada pagamentoSumarizada, int codigoEmpresa, Repositorio.UnitOfWork unidadeTrabalho)
        {
            DateTime dataDocumento = pagamentoSumarizada.DataEmissao ?? DateTime.Now;
            DateTime dataPagamento = pagamento.DataCriacao;

            Repositorio.Embarcador.Transportadores.CondicaoPagamentoTransportador repositorioCondicaoPagamentoTransportador = new Repositorio.Embarcador.Transportadores.CondicaoPagamentoTransportador(unidadeTrabalho);
            List<Dominio.ObjetosDeValor.Embarcador.Escrituracao.CondicaoPagamento> condicoesPagamentoTransportador = repositorioCondicaoPagamentoTransportador.BuscarObjetoPorEmpresa(codigoEmpresa);
            Dominio.ObjetosDeValor.Embarcador.Escrituracao.CondicaoPagamento condicaoPagamentoTransportador = ObterCondicaoPagamentoFiltrada(condicoesPagamentoTransportador, pagamentoSumarizada, dataDocumento, dataPagamento);

            if (condicaoPagamentoTransportador != null)
                return condicaoPagamentoTransportador;

            Repositorio.Embarcador.Filiais.CondicaoPagamentoFilial repositorioCondicaoPagamentoFilial = new Repositorio.Embarcador.Filiais.CondicaoPagamentoFilial(unidadeTrabalho);
            List<Dominio.ObjetosDeValor.Embarcador.Escrituracao.CondicaoPagamento> condicoesPagamentoFilial = repositorioCondicaoPagamentoFilial.BuscarObjetoPorFilial(pagamentoSumarizada.Filial);

            return ObterCondicaoPagamentoFiltrada(condicoesPagamentoFilial, pagamentoSumarizada, dataDocumento, dataPagamento);
        }

        private static Dominio.ObjetosDeValor.Embarcador.Escrituracao.CondicaoPagamento ObterCondicaoPagamentoFiltrada(List<Dominio.ObjetosDeValor.Embarcador.Escrituracao.CondicaoPagamento> condicoesPagamento, Dominio.ObjetosDeValor.Embarcador.Escrituracao.PagamentoSumarizada pagamentoSumarizada, DateTime dataDocumento, DateTime dataPagamento)
        {
            if (condicoesPagamento.Count() == 0)
                return null;

            List<Dominio.ObjetosDeValor.Embarcador.Escrituracao.CondicaoPagamento> condicoesPagamentoFiltradas = condicoesPagamento;
            int diaDataDocumento = dataDocumento.Day;
            int diaDataPagamento = dataPagamento.Day;

            if (pagamentoSumarizada.TipoCarga.HasValue)
                condicoesPagamentoFiltradas = (from condicao in condicoesPagamentoFiltradas where !condicao.CodigoTipoCarga.HasValue || condicao.CodigoTipoCarga.Value == pagamentoSumarizada.TipoCarga.Value select condicao).ToList();
            else
                condicoesPagamentoFiltradas = (from condicao in condicoesPagamentoFiltradas where !condicao.CodigoTipoCarga.HasValue select condicao).ToList();

            if (pagamentoSumarizada.TipoOperacao.HasValue)
                condicoesPagamentoFiltradas = (from condicao in condicoesPagamentoFiltradas where !condicao.CodigoTipoOperacao.HasValue || condicao.CodigoTipoOperacao.Value == pagamentoSumarizada.TipoOperacao.Value select condicao).ToList();
            else
                condicoesPagamentoFiltradas = (from condicao in condicoesPagamentoFiltradas where !condicao.CodigoTipoOperacao.HasValue select condicao).ToList();

            condicoesPagamentoFiltradas = (
                from condicao in condicoesPagamentoFiltradas
                where (
                    !condicao.DiaEmissaoLimite.HasValue ||
                    ((condicao.TipoPrazoPagamento == Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoPrazoPagamento.DataDocumento) && (diaDataDocumento <= condicao.DiaEmissaoLimite.Value)) ||
                    ((condicao.TipoPrazoPagamento == Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoPrazoPagamento.DataPagamento) && (diaDataPagamento <= condicao.DiaEmissaoLimite.Value))
                )
                select condicao
            ).ToList();

            return condicoesPagamentoFiltradas.FirstOrDefault();
        }

        private static bool PagamentoTemValePedagio(Dominio.Entidades.Embarcador.Escrituracao.Pagamento pagamento, Repositorio.UnitOfWork unitOfWork)
        {
            Repositorio.Embarcador.Cargas.CargaValePedagio repCargaValePedagio = new Repositorio.Embarcador.Cargas.CargaValePedagio(unitOfWork);
            Repositorio.Embarcador.ConfiguracaoContabil.DocumentoContabil repDocumentoContabil = new Repositorio.Embarcador.ConfiguracaoContabil.DocumentoContabil(unitOfWork);

            List<Dominio.Entidades.Embarcador.ConfiguracaoContabil.DocumentoContabil> listaDocumento = repDocumentoContabil.BuscarPorPagamento(pagamento.Codigo, pagamento.LotePagamentoLiberado);

            var codigosCargas = (from o in listaDocumento where o.Carga != null && o.DocumentoFaturamento.LancamentoNFSManual == null select o.Carga.Codigo).Distinct().ToList();

            List<Dominio.Entidades.Embarcador.Cargas.CargaValePedagio> listaValePedagio = repCargaValePedagio.BuscarPorCargas(codigosCargas);

            return listaValePedagio.Count > 0;
        }

        private static bool PagamentoTemTipoLayoutEDINoTipoOperacao(Dominio.Entidades.Embarcador.Escrituracao.Pagamento pagamento, TipoLayoutEDI tipoLayoutEDI, Repositorio.UnitOfWork unitOfWork)
        {
            List<Dominio.Entidades.Embarcador.Pedidos.TipoOperacaoLayoutEDI> layoutsTipoOperacao = ObterLayoutEDIsPagamentoTipoOperacao(pagamento, unitOfWork);
            return layoutsTipoOperacao.Any(o => o.LayoutEDI.Tipo == tipoLayoutEDI);
        }

        private static void AdicionarPagamentoPorDocumento(Dominio.Entidades.Embarcador.Financeiro.DocumentoFaturamento documento, DateTime dataFinal, Dominio.Entidades.Cliente tomador, Dominio.Entidades.Empresa empresa, List<int> codigosDocumentoPagamento, bool liberarPagamentosDesbloqueados, Repositorio.UnitOfWork unitOfWork, Dominio.ObjetosDeValor.Embarcador.Auditoria.Auditado auditado)
        {
            List<Dominio.Entidades.Embarcador.Financeiro.DocumentoFaturamento> documentos = new List<Dominio.Entidades.Embarcador.Financeiro.DocumentoFaturamento>() { documento };

            AdicionarPagamentoPorDocumentos(documentos, dataFinal, tomador, empresa, codigosDocumentoPagamento, liberarPagamentosDesbloqueados, unitOfWork, auditado);
        }

        private static void AdicionarPagamentoPorDocumentos(List<Dominio.Entidades.Embarcador.Financeiro.DocumentoFaturamento> documentos, DateTime dataFinal, Dominio.Entidades.Cliente tomador, Dominio.Entidades.Empresa empresa, List<int> codigosDocumentoPagamento, bool liberarPagamentosDesbloqueados, Repositorio.UnitOfWork unitOfWork, Dominio.ObjetosDeValor.Embarcador.Auditoria.Auditado auditado)
        {
            Repositorio.Embarcador.Escrituracao.Pagamento repPagamento = new Repositorio.Embarcador.Escrituracao.Pagamento(unitOfWork);
            Repositorio.Embarcador.Financeiro.DocumentoFaturamento repDocumentoFaturamento = new Repositorio.Embarcador.Financeiro.DocumentoFaturamento(unitOfWork);
            Repositorio.Embarcador.ConfiguracaoContabil.DocumentoContabil repDocumentoContabil = new Repositorio.Embarcador.ConfiguracaoContabil.DocumentoContabil(unitOfWork);

            unitOfWork.Start();

            Dominio.Entidades.Embarcador.Escrituracao.Pagamento pagamento = Servicos.Embarcador.Escrituracao.Pagamento.AdicionarPagamento(DateTime.MinValue, dataFinal, DateTime.MinValue, DateTime.MinValue, tomador, null, empresa, null, null, true, liberarPagamentosDesbloqueados, unitOfWork, auditado);
            int totalRegistros = 0;
            decimal valorPagamento = 0;
            foreach (Dominio.Entidades.Embarcador.Financeiro.DocumentoFaturamento documentoFaturamento in documentos)
            {
                if (!liberarPagamentosDesbloqueados)
                {
                    documentoFaturamento.Pagamento = pagamento;
                    documentoFaturamento.Situacao = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoDocumentoFaturamento.EmFechamento;
                    repDocumentoFaturamento.Atualizar(documentoFaturamento);
                }
                else
                {
                    documentoFaturamento.PagamentoLiberacao = pagamento;
                    codigosDocumentoPagamento.Add(documentoFaturamento.Codigo);
                }

                totalRegistros++;
                valorPagamento += documentoFaturamento.ValorAFaturar;
            }
            pagamento.ValorPagamento = valorPagamento;
            pagamento.QuantidadeDocsPagamento = totalRegistros;
            pagamento.GerandoMovimentoFinanceiro = true;

            repPagamento.Atualizar(pagamento);

            if (pagamento.LotePagamentoLiberado)
            {
                if (codigosDocumentoPagamento.Count < 2000)
                    repDocumentoContabil.SetarPagamentosLiberacaoDocumentoContabil(codigosDocumentoPagamento, pagamento.Codigo);
                else
                {
                    foreach (int codigo in codigosDocumentoPagamento)
                        repDocumentoContabil.SetarPagamentoLiberacaoDocumentoContabil(codigo, pagamento.Codigo);
                }
            }

            unitOfWork.CommitChanges();
        }

        private static void AdicionarPagamentoPorCarga(List<Dominio.Entidades.Embarcador.Financeiro.DocumentoFaturamento> documentos, Dominio.Entidades.Embarcador.Cargas.Carga carga, bool pagamentoLiberado, Repositorio.UnitOfWork unitOfWork)
        {
            Repositorio.Embarcador.Escrituracao.Pagamento repPagamento = new Repositorio.Embarcador.Escrituracao.Pagamento(unitOfWork);
            Repositorio.Embarcador.Financeiro.DocumentoFaturamento repDocumentoFaturamento = new Repositorio.Embarcador.Financeiro.DocumentoFaturamento(unitOfWork);
            Repositorio.Embarcador.ConfiguracaoContabil.DocumentoContabil repDocumentoContabil = new Repositorio.Embarcador.ConfiguracaoContabil.DocumentoContabil(unitOfWork);

            Dominio.Entidades.Embarcador.Escrituracao.Pagamento pagamento = Servicos.Embarcador.Escrituracao.Pagamento.AdicionarPagamento(DateTime.MinValue, DateTime.MinValue, DateTime.MinValue, DateTime.MinValue, null, carga.Filial, carga.Empresa, carga, null, true, pagamentoLiberado, unitOfWork, null);
            unitOfWork.Start();

            int totalRegistros = 0;
            decimal valorPagamento = 0;
            List<int> codigosDocumentoPagamento = new List<int>();
            foreach (Dominio.Entidades.Embarcador.Financeiro.DocumentoFaturamento documentoFaturamento in documentos)
            {
                totalRegistros++;
                valorPagamento += documentoFaturamento.ValorAFaturar;

                if (!pagamentoLiberado)
                {
                    documentoFaturamento.Pagamento = pagamento;
                    documentoFaturamento.Situacao = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoDocumentoFaturamento.EmFechamento;
                    repDocumentoFaturamento.Atualizar(documentoFaturamento);
                    continue;
                }
                documentoFaturamento.PagamentoLiberacao = pagamento;
                codigosDocumentoPagamento.Add(documentoFaturamento.Codigo);

            }

            pagamento.ValorPagamento = valorPagamento;
            pagamento.QuantidadeDocsPagamento = totalRegistros;
            pagamento.GerandoMovimentoFinanceiro = true;

            repPagamento.Atualizar(pagamento);

            if (pagamento.LotePagamentoLiberado)
            {
                if (codigosDocumentoPagamento.Count < 2000)
                    repDocumentoContabil.SetarPagamentosLiberacaoDocumentoContabil(codigosDocumentoPagamento, pagamento.Codigo);
                else
                {
                    foreach (int codigo in codigosDocumentoPagamento)
                        repDocumentoContabil.SetarPagamentoLiberacaoDocumentoContabil(codigo, pagamento.Codigo);
                }
            }

            unitOfWork.CommitChanges();

        }

        private void GerarBloqueioDocumentoFaturamento(Repositorio.Embarcador.Financeiro.DocumentoFaturamento repDocumentoFaturamento, string motivoBloqueio, int codigoDocumentoFaturamento, Repositorio.UnitOfWork unitOfWork)
        {
            unitOfWork.Rollback();

            if (codigoDocumentoFaturamento == 0)
                return;

            Repositorio.Embarcador.Configuracoes.ConfiguracaoCanhoto repConfigCanhoto = new Repositorio.Embarcador.Configuracoes.ConfiguracaoCanhoto(unitOfWork);
            Dominio.Entidades.Embarcador.Configuracoes.ConfiguracaoCanhoto configuracaoCanhoto = repConfigCanhoto.BuscarPrimeiroRegistro();

            if (!configuracaoCanhoto?.PermitirBloquearDocumentoManualmente ?? false)
                return;

            unitOfWork.Start();

            Dominio.Entidades.Embarcador.Financeiro.DocumentoFaturamento documentoFaturamento = repDocumentoFaturamento.BuscarPorCodigo(codigoDocumentoFaturamento);
            documentoFaturamento.PagamentoDocumentoBloqueado = true;
            documentoFaturamento.MotivoBloqueio = motivoBloqueio;
            repDocumentoFaturamento.Atualizar(documentoFaturamento);

            unitOfWork.CommitChanges();
        }

        private static bool ExisteTipoIntegracaoQueNaoGeraPagamentoAutomaticoParaDocumentoBloqueado(Repositorio.UnitOfWork unitOfWork)
        {
            List<Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoIntegracao> tiposIntegracao = new List<Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoIntegracao>()
            {
                Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoIntegracao.GrupoSC
            };

            Repositorio.Embarcador.Cargas.TipoIntegracao repTipoIntegracao = new Repositorio.Embarcador.Cargas.TipoIntegracao(unitOfWork);
            return tiposIntegracao.Exists(tipo => repTipoIntegracao.ExistePorTipo(tipo));
        }

        private static bool PodeGerarEstornoPagamentoIntegracao(Dominio.Entidades.Embarcador.Financeiro.DocumentoFaturamento documentoFaturamento, Repositorio.Embarcador.Escrituracao.DocumentoProvisao repositorioDocumentoProvisao, bool canhotoAvulso)
        {
            if (documentoFaturamento.CTe.TipoCTE == Dominio.Enumeradores.TipoCTE.Substituto)
                return false;

            if (!canhotoAvulso && (documentoFaturamento.CTe.XMLNotaFiscais?.Any(obj => obj.Canhoto != null && obj.Canhoto.SituacaoDigitalizacaoCanhoto != SituacaoDigitalizacaoCanhoto.Digitalizado) ?? false))
                return false;

            List<Dominio.Entidades.Embarcador.Escrituracao.DocumentoProvisao> documentosProvisao = Servicos.Embarcador.Escrituracao.Provisao.ObterDocumentosProvisaoPorDocumentoFaturamento(documentoFaturamento.Pagamento, documentoFaturamento, repositorioDocumentoProvisao);

            if (documentosProvisao.Count == 0 || documentosProvisao.Exists(documento => !documento.Cancelado))
                return false;

            return true;
        }

        #endregion
    }
}
