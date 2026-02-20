using Dominio.Excecoes.Embarcador;
using Dominio.ObjetosDeValor.Embarcador.Enumeradores;
using SGTAdmin.Controllers;
using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Mvc;

namespace SGT.WebAdmin.Controllers.Cargas.Cancelamento
{
    [CustomAuthorize("Cargas/CancelamentoCarga")]
    public class CancelamentoCargaCTeController : BaseController
    {
		#region Construtores

		public CancelamentoCargaCTeController(Conexao conexao) : base(conexao) { }

		#endregion

        #region Métodos Globais

        public async Task<IActionResult> Pesquisa()
        {
            Repositorio.UnitOfWork unidadeTrabalho = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                int codigoCarga = Request.GetIntParam("Carga");
                int codigoCancelamentoCarga = Request.GetIntParam("CancelamentoCarga");

                Repositorio.Embarcador.Cargas.Carga repCarga = new Repositorio.Embarcador.Cargas.Carga(unidadeTrabalho);
                Dominio.Entidades.Embarcador.Cargas.Carga carga = repCarga.BuscarPorCodigo(codigoCarga);

                Models.Grid.Grid grid = new Models.Grid.Grid(Request);
                grid.header = new List<Models.Grid.Head>();
                grid.AdicionarCabecalho("Codigo", false);
                grid.AdicionarCabecalho("SituacaoCTe", false);
                grid.AdicionarCabecalho("NumeroModeloDocumentoFiscal", false);
                grid.AdicionarCabecalho(Localization.Resources.Gerais.Geral.Numero, "Numero", 8, Models.Grid.Align.center, true);
                grid.AdicionarCabecalho("Serie", "Serie", 5, Models.Grid.Align.center, true);
                grid.AdicionarCabecalho("CodigoEmpresa", false);
                grid.AdicionarCabecalho("TipoDocumentoEmissao", false);

                if (TipoServicoMultisoftware == AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiTMS)
                {
                    grid.AdicionarCabecalho("Doc.", "AbreviacaoModeloDocumentoFiscal", 8, Models.Grid.Align.center, true);
                    grid.AdicionarCabecalho("T. Pagamento", "DescricaoTipoPagamento", 8, Models.Grid.Align.center, true);
                    grid.AdicionarCabecalho("Remetente", "Remetente", 16, Models.Grid.Align.left, true);
                }

                grid.AdicionarCabecalho("T. Serviço", "DescricaoTipoServico", 9, Models.Grid.Align.center, true);

                grid.AdicionarCabecalho(Localization.Resources.Gerais.Geral.Destinatario, "Destinatario", 18, Models.Grid.Align.left, true);
                grid.AdicionarCabecalho(Localization.Resources.Gerais.Geral.Destino, "Destino", 15, Models.Grid.Align.left, true);
                grid.AdicionarCabecalho(Localization.Resources.Cargas.CancelamentoCarga.ValorAReceber, "ValorFrete", 8, Models.Grid.Align.right, true);
                grid.AdicionarCabecalho(Localization.Resources.Cargas.CancelamentoCarga.Aliquota, "Aliquota", 5, Models.Grid.Align.right, false);
                grid.AdicionarCabecalho(Localization.Resources.Gerais.Geral.Status, "Status", 8, Models.Grid.Align.center, true);
                grid.AdicionarCabecalho(Localization.Resources.Cargas.CancelamentoCarga.RetornoSEFAZ, "RetornoSEFAZ", 15, Models.Grid.Align.left, false);
                grid.AdicionarCabecalho("HabilitarSincronizarDocumento", false);

                string propOrdenacao = grid.header[grid.indiceColunaOrdena].data;

                if (propOrdenacao == "Remetente" || propOrdenacao == "Destinatario")
                    propOrdenacao += ".Nome";
                if (propOrdenacao == "Destino")
                    propOrdenacao = "LocalidadeTerminoPrestacao.Descricao";

                if (propOrdenacao == "DescricaoTipoPagamento")
                    propOrdenacao = "TipoPagamento";

                if (propOrdenacao == "DescricaoTipoServico")
                    propOrdenacao = "TipoServico";

                if (propOrdenacao == "AbreviacaoModeloDocumentoFiscal")
                    propOrdenacao = "ModeloDocumentoFiscal.Abreviacao";

                propOrdenacao = "CTe." + propOrdenacao;

                Repositorio.Embarcador.Cargas.CargaCTe repCargaCTe = new Repositorio.Embarcador.Cargas.CargaCTe(unidadeTrabalho);
                Repositorio.Embarcador.Cargas.CargaCancelamento repositorioCargaCancelamento = new Repositorio.Embarcador.Cargas.CargaCancelamento(unidadeTrabalho);

                bool eTipoCancelamentoUnitario = repositorioCargaCancelamento.VerificarTipoCancelamentoUnitario(codigoCancelamentoCarga);
                List<Dominio.Entidades.Embarcador.Cargas.CargaCTe> ctes = repCargaCTe.Consultar(codigoCarga, codigoCancelamentoCarga, 0, 0, eTipoCancelamentoUnitario, propOrdenacao, grid.dirOrdena, grid.inicio, grid.limite);
                int countCTes = repCargaCTe.ContarConsulta(codigoCarga, codigoCancelamentoCarga, 0, 0, eTipoCancelamentoUnitario);

                Dominio.Entidades.Embarcador.Cargas.CargaCancelamento cargaCancelamento = repositorioCargaCancelamento.BuscarPorCodigo(codigoCancelamentoCarga);

                grid.setarQuantidadeTotal(countCTes);

                Servicos.Embarcador.Carga.CargaCTe servicoCargaCTe = new Servicos.Embarcador.Carga.CargaCTe(unidadeTrabalho);

                var retorno = (from obj in ctes
                               select new
                               {
                                   obj.CTe.Codigo,
                                   CodigoEmpresa = obj.CTe.Empresa.Codigo,
                                   SituacaoCTe = obj.CTe.Status,
                                   NumeroModeloDocumentoFiscal = obj.CTe.ModeloDocumentoFiscal.Numero,
                                   obj.CTe.Numero,
                                   Serie = obj.CTe.Serie.Numero,
                                   AbreviacaoModeloDocumentoFiscal = obj.CTe.ModeloDocumentoFiscal.Abreviacao,
                                   obj.CTe.ModeloDocumentoFiscal.TipoDocumentoEmissao,
                                   obj.CTe.DescricaoTipoPagamento,
                                   Remetente = obj.CTe.Remetente != null ? obj.CTe.Remetente.Nome + "(" + obj.CTe.Remetente.CPF_CNPJ_Formatado + ")" : string.Empty,
                                   obj.CTe.DescricaoTipoServico,
                                   Destinatario = obj.CTe.Destinatario != null ? obj.CTe.Destinatario.Nome + "(" + obj.CTe.Destinatario.CPF_CNPJ_Formatado + ")" : string.Empty,
                                   Destino = obj.CTe.LocalidadeTerminoPrestacao.DescricaoCidadeEstado,
                                   ValorFrete = obj.CTe.ValorAReceber.ToString("n2"),
                                   Aliquota = obj.CTe.AliquotaICMS.ToString("n2"),
                                   Status = obj.CTe.DescricaoStatus,
                                   RetornoSEFAZ = !string.IsNullOrWhiteSpace(obj.CTe.MensagemRetornoSefaz) ? obj.CTe.MensagemRetornoSefaz != " - " ? System.Web.HttpUtility.HtmlEncode(obj.CTe.MensagemRetornoSefaz) : "" : "",
                                   HabilitarSincronizarDocumento = servicoCargaCTe.ObterHabilitarSincronizarDocumento(obj.CTe, cargaCancelamento),
                                   DT_RowColor = obj.CTe.Status == "A" ? "#dff0d8" :
                                                 obj.CTe.Status == "R" ? "rgba(193, 101, 101, 1)" :
                                                 (obj.CTe.Status == "C" || obj.CTe.Status == "I" || obj.CTe.Status == "D") ? "#777" :
                                                 "",
                                   DT_FontColor = (obj.CTe.Status == "R" || obj.CTe.Status == "C" || obj.CTe.Status == "I" || obj.CTe.Status == "D") ? "#FFFFFF" : "",
                               }).ToList();

                grid.AdicionaRows(retorno);

                return new JsonpResult(grid);
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);

                return new JsonpResult(false, false, Localization.Resources.Cargas.CancelamentoCarga.OcorreuFalhaConsultarCTes);
            }
            finally
            {
                unidadeTrabalho.Dispose();
            }
        }

        [AcceptVerbs("POST", "GET")]
        public async Task<IActionResult> DownloadDACTE()
        {
            Repositorio.UnitOfWork unidadeTrabalho = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                int codigoCTe = 0;
                int.TryParse(Request.Params("CTe"), out codigoCTe);

                Repositorio.ConhecimentoDeTransporteEletronico repCTe = new Repositorio.ConhecimentoDeTransporteEletronico(unidadeTrabalho);
                Dominio.Entidades.ConhecimentoDeTransporteEletronico cte = repCTe.BuscarPorCodigo(codigoCTe);
                Repositorio.Embarcador.Configuracao.ConfiguracaoTMS repConfiguracaoTMS = new Repositorio.Embarcador.Configuracao.ConfiguracaoTMS(unidadeTrabalho);
                Dominio.Entidades.Embarcador.Configuracao.ConfiguracaoTMS configuracaoTMS = repConfiguracaoTMS.BuscarConfiguracaoPadrao();

                if (cte == null)
                    return new JsonpResult(true, false, Localization.Resources.Cargas.CancelamentoCarga.CTeNaoEncontrado);

                if (cte.Status != "A" && cte.Status != "C" && cte.Status != "K" && cte.Status != "Z")
                    return new JsonpResult(true, false, Localization.Resources.Cargas.CancelamentoCarga.StatusDoCTeNaoPermite);

                if (string.IsNullOrWhiteSpace(Servicos.Embarcador.Configuracoes.ConfigurationInstance.GetInstance(unidadeTrabalho).ObterConfiguracaoArquivo().CaminhoRelatorios))
                    return new JsonpResult(true, false, Localization.Resources.Cargas.CancelamentoCarga.DownloadDACTENaoDisponivel);

                string nomeArquivo = cte.Chave;
                if (configuracaoTMS.GerarPDFCTeCancelado && cte.Status == "C" && cte.ModeloDocumentoFiscal.TipoDocumentoEmissao == Dominio.Enumeradores.TipoDocumento.CTe)
                    nomeArquivo = nomeArquivo + "_Canc";

                string caminhoPDF = Utilidades.IO.FileStorageService.Storage.Combine(Servicos.Embarcador.Configuracoes.ConfigurationInstance.GetInstance(unidadeTrabalho).ObterConfiguracaoArquivo().CaminhoRelatorios, cte.Empresa.CNPJ, nomeArquivo) + ".pdf";

                byte[] dacte = null;

                if (!Utilidades.IO.FileStorageService.Storage.Exists(caminhoPDF))
                {
                    if (string.IsNullOrWhiteSpace(Servicos.Embarcador.Configuracoes.ConfigurationInstance.GetInstance(unidadeTrabalho).ObterConfiguracaoArquivo().CaminhoGeradorRelatorios))
                        return new JsonpResult(true, false, Localization.Resources.Cargas.CancelamentoCarga.GeradorDACTENaoDisponivel);

                    Servicos.DACTE svcDACTE = new Servicos.DACTE(unidadeTrabalho);

                    dacte = svcDACTE.GerarPorProcesso(cte.Codigo, unidadeTrabalho, configuracaoTMS.GerarPDFCTeCancelado);
                }
                else
                {
                    dacte = Utilidades.IO.FileStorageService.Storage.ReadAllBytes(caminhoPDF);
                }

                if (dacte != null)
                    return Arquivo(dacte, "application/pdf", System.IO.Path.GetFileName(caminhoPDF));
                else
                    return new JsonpResult(false, false, Localization.Resources.Cargas.CancelamentoCarga.NaoFoiPossivelGerarDACTE);
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);

                return new JsonpResult(false, false, Localization.Resources.Cargas.CancelamentoCarga.FalhaDownloadDACTE);
            }
            finally
            {
                unidadeTrabalho.Dispose();
            }
        }

        [AcceptVerbs("POST", "GET")]
        public async Task<IActionResult> DownloadXML()
        {
            Repositorio.UnitOfWork unidadeTrabalho = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                int codigoCTe = 0;
                int.TryParse(Request.Params("CTe"), out codigoCTe);

                Repositorio.ConhecimentoDeTransporteEletronico repCTe = new Repositorio.ConhecimentoDeTransporteEletronico(unidadeTrabalho);
                Dominio.Entidades.ConhecimentoDeTransporteEletronico cte = repCTe.BuscarPorCodigo(codigoCTe);

                if (cte == null)
                    return new JsonpResult(false, false, "CT-e " + Localization.Resources.Cargas.CancelamentoCarga.NaoEncontrado);

                Servicos.CTe svcCTe = new Servicos.CTe(unidadeTrabalho);

                byte[] data = svcCTe.ObterXMLAutorizacao(cte);

                if (data != null)
                    return Arquivo(data, "text/xml", string.Concat(cte.Chave, ".xml"));
                else
                    return new JsonpResult(false, false, "XML " + Localization.Resources.Cargas.CancelamentoCarga.NaoEncontrado);
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);

                return new JsonpResult(false, false, Localization.Resources.Cargas.CancelamentoCarga.FalhaAoRealizarDownloadXML);
            }
            finally
            {
                unidadeTrabalho.Dispose();
            }
        }

        [AcceptVerbs("POST", "GET")]
        public async Task<IActionResult> DownloadXMLCancelamento()
        {
            Repositorio.UnitOfWork unidadeTrabalho = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                int codigoCTe = 0;
                int.TryParse(Request.Params("CTe"), out codigoCTe);

                Repositorio.ConhecimentoDeTransporteEletronico repCTe = new Repositorio.ConhecimentoDeTransporteEletronico(unidadeTrabalho);
                Dominio.Entidades.ConhecimentoDeTransporteEletronico cte = repCTe.BuscarPorCodigo(codigoCTe);

                if (cte == null)
                    return new JsonpResult(false, false, "CT-e " + Localization.Resources.Cargas.CancelamentoCarga.NaoEncontrado);

                Servicos.CTe svcCTe = new Servicos.CTe(unidadeTrabalho);

                byte[] data = svcCTe.ObterXMLCancelamento(cte, unidadeTrabalho);

                if (data != null)
                    return Arquivo(data, "text/xml", string.Concat(cte.Chave + "-procCancCTe", ".xml"));
                else
                    return new JsonpResult(false, false, "XML " + Localization.Resources.Cargas.CancelamentoCarga.NaoEncontrado);
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);

                return new JsonpResult(false, false, Localization.Resources.Cargas.CancelamentoCarga.FalhaAoRealizarDownloadXML);
            }
            finally
            {
                unidadeTrabalho.Dispose();
            }
        }

        [AcceptVerbs("POST", "GET")]
        public async Task<IActionResult> DownloadXMLInutilizacao()
        {
            Repositorio.UnitOfWork unidadeTrabalho = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                int codigoCTe = 0;
                int.TryParse(Request.Params("CTe"), out codigoCTe);

                Repositorio.ConhecimentoDeTransporteEletronico repCTe = new Repositorio.ConhecimentoDeTransporteEletronico(unidadeTrabalho);
                Dominio.Entidades.ConhecimentoDeTransporteEletronico cte = repCTe.BuscarPorCodigo(codigoCTe);

                if (cte == null)
                    return new JsonpResult(false, false, "CT-e " + Localization.Resources.Cargas.CancelamentoCarga.NaoEncontrado);

                Servicos.CTe svcCTe = new Servicos.CTe(unidadeTrabalho);

                byte[] data = svcCTe.ObterXMLInutilizacao(cte, unidadeTrabalho);

                if (data != null)
                    return Arquivo(data, "text/xml", $"{cte.Empresa.CNPJ}_{cte.Numero}_{cte.Serie.Numero}-procInutCTe.xml");
                else
                    return new JsonpResult(false, false, "XML " + Localization.Resources.Cargas.CancelamentoCarga.NaoEncontrado);
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);

                return new JsonpResult(false, false, Localization.Resources.Cargas.CancelamentoCarga.FalhaAoRealizarDownloadXML);
            }
            finally
            {
                unidadeTrabalho.Dispose();
            }
        }

        [AcceptVerbs("POST", "GET")]
        public async Task<IActionResult> DownloadPDF()
        {
            Repositorio.UnitOfWork unidadeTrabalho = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                int codigoCTe;
                int.TryParse(Request.Params("CTe"), out codigoCTe);

                Repositorio.ConhecimentoDeTransporteEletronico repCTe = new Repositorio.ConhecimentoDeTransporteEletronico(unidadeTrabalho);
                Dominio.Entidades.ConhecimentoDeTransporteEletronico cte = repCTe.BuscarPorCodigo(codigoCTe);

                if (cte == null)
                    return new JsonpResult(true, false, "CT-e " + Localization.Resources.Cargas.CancelamentoCarga.NaoEncontrado);

                string nomeArquivo = cte.ModeloDocumentoFiscal.Numero + "_" + cte.Numero + "_" + cte.Serie.Numero + "_" + cte.ModeloDocumentoFiscal.Abreviacao;

                if (cte.Status != "A" && cte.Status != "C" && cte.Status != "K")
                    return new JsonpResult(true, false, Localization.Resources.Cargas.CancelamentoCarga.StatusCTENaoPermiteGeracao + cte.ModeloDocumentoFiscal.Abreviacao + ".");

                if (string.IsNullOrWhiteSpace(Servicos.Embarcador.Configuracoes.ConfigurationInstance.GetInstance(unidadeTrabalho).ObterConfiguracaoArquivo().CaminhoRelatorios))
                    return new JsonpResult(true, false, Localization.Resources.Cargas.CancelamentoCarga.CaminhoParaDownload + cte.ModeloDocumentoFiscal.Abreviacao + Localization.Resources.Cargas.CancelamentoCarga.NaoDisponivelContateSuporte);

                string caminhoPDF = Utilidades.IO.FileStorageService.Storage.Combine(Servicos.Embarcador.Configuracoes.ConfigurationInstance.GetInstance(unidadeTrabalho).ObterConfiguracaoArquivo().CaminhoRelatorios, cte.Empresa.CNPJ, nomeArquivo) + ".pdf";

                byte[] pdf = null;

                if (!Utilidades.IO.FileStorageService.Storage.Exists(caminhoPDF))
                {
                    if (string.IsNullOrWhiteSpace(Servicos.Embarcador.Configuracoes.ConfigurationInstance.GetInstance(unidadeTrabalho).ObterConfiguracaoArquivo().CaminhoGeradorRelatorios))
                        return new JsonpResult(true, false, Localization.Resources.Cargas.CancelamentoCarga.Gerador + cte.ModeloDocumentoFiscal.Abreviacao + Localization.Resources.Cargas.CancelamentoCarga.Indisponivel);

                    Servicos.DACTE svcDACTE = new Servicos.DACTE(unidadeTrabalho);

                    pdf = svcDACTE.GerarPorProcesso(cte.Codigo, unidadeTrabalho);
                }
                else
                {
                    pdf = Utilidades.IO.FileStorageService.Storage.ReadAllBytes(caminhoPDF);
                }

                if (pdf != null)
                    return Arquivo(pdf, "application/pdf", System.IO.Path.GetFileName(caminhoPDF));
                else
                    return new JsonpResult(false, false, Localization.Resources.Cargas.CancelamentoCarga.NaoFoiPossivelGerar + cte.ModeloDocumentoFiscal.Abreviacao + Localization.Resources.Cargas.CancelamentoCarga.Atualize);
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);

                return new JsonpResult(false, false, Localization.Resources.Cargas.CancelamentoCarga.FalhaDownload);
            }
            finally
            {
                unidadeTrabalho.Dispose();
            }
        }

        public async Task<IActionResult> LiberarSemInutilizarCTes()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                List<AdminMultisoftware.Dominio.Enumeradores.PermissaoPersonalizada> permissoesPersonalizadas = ObterPermissoesPersonalizadas("Cargas/CancelamentoCarga");

                if (!permissoesPersonalizadas.Contains(AdminMultisoftware.Dominio.Enumeradores.PermissaoPersonalizada.CargaCancelamento_LiberarCancelamentoComCTeNaoInutilizado))
                    return new JsonpResult(false, true, Localization.Resources.Cargas.CancelamentoCarga.NaoPossuiPermissão);

                Repositorio.Embarcador.Cargas.CargaCancelamento repCargaCancelamento = new Repositorio.Embarcador.Cargas.CargaCancelamento(unitOfWork);

                int codigoCargaCancelamento = Request.GetIntParam("Codigo");

                Dominio.Entidades.Embarcador.Cargas.CargaCancelamento cargaCancelamento = repCargaCancelamento.BuscarPorCodigo(codigoCargaCancelamento);

                if (cargaCancelamento == null)
                    return new JsonpResult(false, true, Localization.Resources.Cargas.CancelamentoCarga.CancelamentoNaoEncontrado);

                if (cargaCancelamento.Situacao != Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoCancelamentoCarga.RejeicaoCancelamento)
                    return new JsonpResult(false, true, Localization.Resources.Cargas.CancelamentoCarga.SituacaoCancelamento + cargaCancelamento.Situacao.Descricao() + Localization.Resources.Cargas.CancelamentoCarga.NaoPermiteLiberacao);

                unitOfWork.Start();

                cargaCancelamento.Situacao = SituacaoCancelamentoCarga.EmCancelamento;
                cargaCancelamento.LiberarCancelamentoComCTeNaoInutilizado = true;

                repCargaCancelamento.Atualizar(cargaCancelamento);

                Servicos.Auditoria.Auditoria.Auditar(Auditado, cargaCancelamento, null, Localization.Resources.Cargas.CancelamentoCarga.LiberouCancelamentoDaCargaCTes, unitOfWork);

                unitOfWork.CommitChanges();

                return new JsonpResult(true);
            }
            catch (Exception ex)
            {
                unitOfWork.Rollback();
                Servicos.Log.TratarErro(ex);
                return new JsonpResult(false, true, Localization.Resources.Cargas.CancelamentoCarga.FalhaLiberarCancelamento);
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        public async Task<IActionResult> LiberarAverbacaoRejeitada()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                List<AdminMultisoftware.Dominio.Enumeradores.PermissaoPersonalizada> permissoesPersonalizadas = ObterPermissoesPersonalizadas("Cargas/CancelamentoCarga");

                if (!permissoesPersonalizadas.Contains(AdminMultisoftware.Dominio.Enumeradores.PermissaoPersonalizada.CargaCancelamento_LiberarAverbacaoRejeitada))
                    return new JsonpResult(false, true, Localization.Resources.Cargas.CancelamentoCarga.NaoPossuiPermissão);

                Repositorio.Embarcador.Cargas.CargaCancelamento repCargaCancelamento = new Repositorio.Embarcador.Cargas.CargaCancelamento(unitOfWork);

                int codigoCargaCancelamento = Request.GetIntParam("Codigo");

                Dominio.Entidades.Embarcador.Cargas.CargaCancelamento cargaCancelamento = repCargaCancelamento.BuscarPorCodigo(codigoCargaCancelamento);

                if (cargaCancelamento == null)
                    return new JsonpResult(false, true, Localization.Resources.Cargas.CancelamentoCarga.CancelamentoNaoEncontrado);

                if (cargaCancelamento.Situacao != Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoCancelamentoCarga.RejeicaoCancelamento)
                    return new JsonpResult(false, true, Localization.Resources.Cargas.CancelamentoCarga.SituacaoCancelamento + cargaCancelamento.Situacao.Descricao() + Localization.Resources.Cargas.CancelamentoCarga.NaoPermiteLiberacao);

                unitOfWork.Start();

                cargaCancelamento.Situacao = SituacaoCancelamentoCarga.EmCancelamento;
                cargaCancelamento.LiberarCancelamentoComAverbacaoCTeRejeitada = true;

                repCargaCancelamento.Atualizar(cargaCancelamento);

                Servicos.Auditoria.Auditoria.Auditar(Auditado, cargaCancelamento, null, Localization.Resources.Cargas.CancelamentoCarga.LiberouCancelamentoSemCancelarAverbacaoCTe, unitOfWork);

                unitOfWork.CommitChanges();

                return new JsonpResult(true);
            }
            catch (Exception ex)
            {
                unitOfWork.Rollback();
                Servicos.Log.TratarErro(ex);
                return new JsonpResult(false, true, Localization.Resources.Cargas.CancelamentoCarga.FalhaLiberarCancelamento);
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        public async Task<IActionResult> InformarCancelamentoPrefeitura()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                int codigoCargaCancelamento = Request.GetIntParam("Cancelamento");
                Repositorio.Embarcador.Cargas.CargaCancelamento repositorioCargaCancelamento = new Repositorio.Embarcador.Cargas.CargaCancelamento(unitOfWork);
                Dominio.Entidades.Embarcador.Cargas.CargaCancelamento cargaCancelamento = repositorioCargaCancelamento.BuscarPorCodigo(codigoCargaCancelamento);

                if (cargaCancelamento == null)
                    return new JsonpResult(false, true, Localization.Resources.Cargas.CancelamentoCarga.CancelamentoNaoEncontrado);

                int codigoCte = Request.GetIntParam("CTe");
                Repositorio.ConhecimentoDeTransporteEletronico repositorioCTe = new Repositorio.ConhecimentoDeTransporteEletronico(unitOfWork);
                Dominio.Entidades.ConhecimentoDeTransporteEletronico cte = repositorioCTe.BuscarPorCodigo(codigoCte);

                if (cte == null)
                    throw new ControllerException(Localization.Resources.Cargas.CancelamentoCarga.CTeNaoEncontrado);

                if (cte.Status != "A")
                    throw new ControllerException($"{Localization.Resources.Cargas.CancelamentoCarga.SituacaoCTe}{ cte.DescricaoStatus } {Localization.Resources.Cargas.CancelamentoCarga.NaoPermiteAcao}");

                if (cte.ModeloDocumentoFiscal == null || cte.ModeloDocumentoFiscal.TipoDocumentoEmissao != Dominio.Enumeradores.TipoDocumento.NFSe)
                    throw new ControllerException($" {Localization.Resources.Cargas.CancelamentoCarga.ModeloDocumento}{cte.ModeloDocumentoFiscal?.Abreviacao ?? string.Empty}{Localization.Resources.Cargas.CancelamentoCarga.NaoPermiteAcao}");

                unitOfWork.Start();

                cte.Status = "C";
                cte.DataCancelamento = DateTime.Now;

                repositorioCTe.Atualizar(cte);

                Servicos.Auditoria.Auditoria.Auditar(Auditado, cte, Localization.Resources.Cargas.CancelamentoCarga.InformadoCancelamentoPrefeitura, unitOfWork);
                Servicos.Auditoria.Auditoria.Auditar(Auditado, cargaCancelamento.Carga, $"{Localization.Resources.Cargas.CancelamentoCarga.InformadoCancelamentoPrefeituraCTe} {cte.Numero}", unitOfWork);

                unitOfWork.CommitChanges();

                return new JsonpResult(true);
            }
            catch (ControllerException excecao)
            {
                unitOfWork.Rollback();
                return new JsonpResult(false, true, excecao.Message);
            }
            catch (Exception excecao)
            {
                unitOfWork.Rollback();

                Servicos.Log.TratarErro(excecao);

                return new JsonpResult(false, Localization.Resources.Cargas.CancelamentoCarga.FalhaAoInformarCancelamentoPrefeitura);
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        public async Task<IActionResult> LiberarValePedagioRejeitado()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                List<AdminMultisoftware.Dominio.Enumeradores.PermissaoPersonalizada> permissoesPersonalizadas = ObterPermissoesPersonalizadas("Cargas/CancelamentoCarga");

                if (!permissoesPersonalizadas.Contains(AdminMultisoftware.Dominio.Enumeradores.PermissaoPersonalizada.CargaCancelamento_LiberarAverbacaoRejeitada))
                    return new JsonpResult(false, true, Localization.Resources.Cargas.CancelamentoCarga.NaoPossuiPermissão);

                Repositorio.Embarcador.Cargas.CargaCancelamento repCargaCancelamento = new Repositorio.Embarcador.Cargas.CargaCancelamento(unitOfWork);

                int codigoCargaCancelamento = Request.GetIntParam("Codigo");

                Dominio.Entidades.Embarcador.Cargas.CargaCancelamento cargaCancelamento = repCargaCancelamento.BuscarPorCodigo(codigoCargaCancelamento);

                if (cargaCancelamento == null)
                    return new JsonpResult(false, true, Localization.Resources.Cargas.CancelamentoCarga.CancelamentoNaoEncontrado);

                if (cargaCancelamento.Situacao != Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoCancelamentoCarga.RejeicaoCancelamento)
                    return new JsonpResult(false, true, Localization.Resources.Cargas.CancelamentoCarga.SituacaoCancelamento + cargaCancelamento.Situacao.Descricao() + Localization.Resources.Cargas.CancelamentoCarga.NaoPermiteLiberacao);

                unitOfWork.Start();

                cargaCancelamento.Situacao = SituacaoCancelamentoCarga.EmCancelamento;
                cargaCancelamento.LiberarCancelamentoComValePedagioRejeitado = true;

                repCargaCancelamento.Atualizar(cargaCancelamento);

                Servicos.Auditoria.Auditoria.Auditar(Auditado, cargaCancelamento, null, Localization.Resources.Cargas.CancelamentoCarga.LiberouSemCancelarValePedágio, unitOfWork);

                unitOfWork.CommitChanges();

                return new JsonpResult(true);
            }
            catch (Exception ex)
            {
                unitOfWork.Rollback();
                Servicos.Log.TratarErro(ex);
                return new JsonpResult(false, true, Localization.Resources.Cargas.CancelamentoCarga.FalhaLiberarCancelamento);
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        [AcceptVerbs("POST")]
        public async Task<IActionResult> ReverterAnulacaoGerencialCTe()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);
            try
            {
                int codigoCte = Request.GetIntParam("CodigoCte");
                int codigoCargaCancelamento = Request.GetIntParam("Cancelamento");

                if (codigoCte == 0)
                    return new JsonpResult(false, true, "Nenhum registro selecionado.");

                Repositorio.Embarcador.Configuracoes.ExecucaoComandos repExecucaoComandos = new Repositorio.Embarcador.Configuracoes.ExecucaoComandos(unitOfWork);
                Repositorio.ConhecimentoDeTransporteEletronico repositorioCte = new Repositorio.ConhecimentoDeTransporteEletronico(unitOfWork);

                Repositorio.Embarcador.Cargas.CargaCancelamento repositorioCargaCancelamento = new Repositorio.Embarcador.Cargas.CargaCancelamento(unitOfWork);
                Dominio.Entidades.Embarcador.Cargas.CargaCancelamento cargaCancelamento = repositorioCargaCancelamento.BuscarPorCodigo(codigoCargaCancelamento);

                Dominio.Entidades.ConhecimentoDeTransporteEletronico cte = repositorioCte.BuscarPorCodigo(codigoCte);

                if (cte.Status != "Z")
                    return new JsonpResult(false, true, "O status do CT-e não permitida para essa alteração.");

                cargaCancelamento.Situacao = SituacaoCancelamentoCarga.RejeicaoCancelamento;

                unitOfWork.Start();

                repositorioCte.AlterarStatusCTes(cte.Codigo, "A");
                repositorioCte.AutorizarFaturamentosCTe(cte.Codigo);
                repositorioCargaCancelamento.Atualizar(cargaCancelamento);

                Servicos.Auditoria.Auditoria.Auditar(Auditado, cte, null, "Status do CT-e alterado de Anulado para Autorizado", unitOfWork);
                Servicos.Auditoria.Auditoria.Auditar(Auditado, cargaCancelamento, null, "Situação do cancelamento alterado para Rejeição Cancelamento", unitOfWork);

                unitOfWork.CommitChanges();

                return new JsonpResult(true);
            }
            catch (Exception ex)
            {
                unitOfWork.Rollback();
                Servicos.Log.TratarErro(ex);
                return new JsonpResult(false, "Ocorreu uma falha ao tentar alterar o(s) registro(s).");
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        [AllowAuthenticate]
        public IActionResult SincronizarLoteDocumentoEmProcessamento()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                int codigoCarga = Request.GetIntParam("Carga");
                int codigoCancelamentoCarga = Request.GetIntParam("CancelamentoCarga");

                if (codigoCarga == 0)
                    return new JsonpResult(false, true, "Carga não informada.");

                Servicos.Embarcador.Carga.CargaCTe servicoCargaCTe = new Servicos.Embarcador.Carga.CargaCTe(unitOfWork);

                int? codigoCancelamentoCargaNullable = codigoCancelamentoCarga > 0 ? (int?)codigoCancelamentoCarga : null;

                Dominio.ObjetosDeValor.Embarcador.Carga.SincronizacaoLoteResultado resultado = servicoCargaCTe.SincronizarLoteDocumentoEmProcessamento(
                    codigoCarga,
                    codigoCancelamentoCargaNullable,
                    Auditado,
                    TipoServicoMultisoftware);

                if (!resultado.Sucesso)
                    return new JsonpResult(false, true, resultado.Mensagem);

                return new JsonpResult(true);
            }
            catch (ServicoException ex)
            {
                Servicos.Log.TratarErro(ex);
                return new JsonpResult(false, ex.Message);
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);
                return new JsonpResult(false, "Ocorreu uma falha ao tentar sincronizar o Documento.");
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        #endregion
    }

    
}
