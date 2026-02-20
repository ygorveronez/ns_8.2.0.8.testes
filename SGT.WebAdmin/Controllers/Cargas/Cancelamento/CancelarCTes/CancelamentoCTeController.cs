using Dominio.Excecoes.Embarcador;
using Dominio.ObjetosDeValor.Embarcador.Enumeradores;
using SGTAdmin.Controllers;
using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Mvc;

namespace SGT.WebAdmin.Controllers.Cargas.Cancelamento
{
    [CustomAuthorize("Cargas/CancelamentoCTe")]
    public class CancelamentoCTeController : BaseController
    {
		#region Construtores

		public CancelamentoCTeController(Conexao conexao) : base(conexao) { }

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

                grid.setarQuantidadeTotal(countCTes);

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

        [AcceptVerbs("POST", "GET")]
        public async Task<IActionResult> Adicionar()
        {
            Repositorio.UnitOfWork unidadeTrabalho = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                List<AdminMultisoftware.Dominio.Enumeradores.PermissaoPersonalizada> permissoesPersonalizadas = ObterPermissoesPersonalizadas("Cargas/CancelamentoCTes");

                if(permissoesPersonalizadas.Count > 0)
                {
                    int codigoCTe;
                    int.TryParse(Request.Params("CodigoCTe"), out codigoCTe);

                    Servicos.Embarcador.Carga.Cancelamento servicoCancelamento = new Servicos.Embarcador.Carga.Cancelamento();

                    int codigoCargaCancelamento = 0;//AdicionarCancelamento(unidadeTrabalho, permissoesPersonalizadas);

                    return new JsonpResult(codigoCargaCancelamento);
                }
                return new JsonpResult("Não possui permissão para realizar esse processo");
            }
            catch (BaseException excecao)
            {
                unidadeTrabalho.Rollback();
                return new JsonpResult(false, true, excecao.Message);
            }
            catch (Exception excecao)
            {
                unidadeTrabalho.Rollback();
                Servicos.Log.TratarErro(excecao);
                return new JsonpResult(false, false, Localization.Resources.Cargas.CancelamentoCarga.FalhaGerarCancelamento);
            }
            finally
            {
                unidadeTrabalho.Dispose();
            }
        }

        #endregion
    }
}
