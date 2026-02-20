using SGTAdmin.Controllers;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Microsoft.AspNetCore.Mvc;
using Dominio.ObjetosDeValor.Embarcador.Enumeradores;

namespace SGT.WebAdmin.Controllers.CTe
{
    [CustomAuthorize("CTe/ConsultaCTe", "CTe/CTeCancelamento", "CTe/ConhecimentoEletronico", "Cargas/Carga")]
    public class ConsultaCTeController : BaseController
    {
		#region Construtores

		public ConsultaCTeController(Conexao conexao) : base(conexao) { }

		#endregion

        #region Métodos Públicos

        [AllowAuthenticate]
        public async Task<IActionResult> Pesquisa()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);
            try
            {
                Repositorio.ConhecimentoDeTransporteEletronico repCTe = new Repositorio.ConhecimentoDeTransporteEletronico(unitOfWork);

                Models.Grid.Grid grid = new Models.Grid.Grid(Request);
                grid.header = new List<Models.Grid.Head>();
                grid.AdicionarCabecalho("Codigo", false);
                grid.AdicionarCabecalho("Descricao", false);
                grid.AdicionarCabecalho("Número", "Numero", 8, Models.Grid.Align.center, true);
                grid.AdicionarCabecalho("Serie", "Serie", 6, Models.Grid.Align.center, true);
                if (ConfiguracaoEmbarcador.UtilizaEmissaoMultimodal)
                    grid.AdicionarCabecalho("Nº Controle", "NumeroControle", 8, Models.Grid.Align.left, true);
                grid.AdicionarCabecalho("Emissão", "DataEmissao", 8, Models.Grid.Align.center, true);
                if (ConfiguracaoEmbarcador.UtilizaEmissaoMultimodal)
                    grid.AdicionarCabecalho("Doc", "Documentos", 8, Models.Grid.Align.left, false);
                else
                    grid.AdicionarCabecalho("Doc", "AbreviacaoModeloDocumentoFiscal", 4, Models.Grid.Align.center, true);
                if (ConfiguracaoEmbarcador.UtilizaEmissaoMultimodal)
                {
                    grid.AdicionarCabecalho("Tipo", "TipoDocumento", 6, Models.Grid.Align.left, false);
                    grid.AdicionarCabecalho("Tipo Serviço", "DescricaoTipoServico", 8, Models.Grid.Align.left, false);
                    grid.AdicionarCabecalho("Viagem", "Viagem", 8, Models.Grid.Align.left, false);
                    grid.AdicionarCabecalho("POL", "PortoOrigem", 8, Models.Grid.Align.left, false);
                    grid.AdicionarCabecalho("POD", "PortoDestino", 8, Models.Grid.Align.left, false);
                }
                if (!ConfiguracaoEmbarcador.UtilizaEmissaoMultimodal)
                {
                    grid.AdicionarCabecalho("Tipo Serviço", "TipoServico", 12, Models.Grid.Align.left, false);
                    grid.AdicionarCabecalho("Emitente", "Emitente", 12, Models.Grid.Align.left, true);
                }
                grid.AdicionarCabecalho("Remetente", "Remetente", 12, Models.Grid.Align.left, true);
                grid.AdicionarCabecalho("Origem", "Origem", 12, Models.Grid.Align.left, true);
                grid.AdicionarCabecalho("Destinatário", "Destinatario", 12, Models.Grid.Align.left, true);
                grid.AdicionarCabecalho("Destino", "Destino", 12, Models.Grid.Align.left, true);
                grid.AdicionarCabecalho("Valor a Receber", "ValorFrete", 8, Models.Grid.Align.right, true);
                grid.AdicionarCabecalho("Status", "Status", 8, Models.Grid.Align.center, true);
                if (ConfiguracaoEmbarcador.UtilizaEmissaoMultimodal)
                    grid.AdicionarCabecalho("Autorização", "DataAutorizacao", 8, Models.Grid.Align.center, true);
                grid.AdicionarCabecalho("Retorno SEFAZ", "RetornoSEFAZ", 12, Models.Grid.Align.left, false);
                grid.AdicionarCabecalho("NumeroModeloDocumentoFiscal", false);
                grid.AdicionarCabecalho("TipoDocumentoEmissao", false);
                grid.AdicionarCabecalho("SituacaoCTe", false);
                grid.AdicionarCabecalho("Chave", false);

                if (!ExecutarPesquisa(out string erro, out List<Dominio.Entidades.ConhecimentoDeTransporteEletronico> ctes, out int countCTes, grid.header[grid.indiceColunaOrdena].data, grid.dirOrdena, grid.inicio, grid.limite, unitOfWork))
                    return new JsonpResult(false, true, erro);

                grid.setarQuantidadeTotal(countCTes);

                var lista = (from obj in ctes
                             select new
                             {
                                 obj.Codigo,
                                 TipoDocumentoEmissao = obj.ModeloDocumentoFiscal?.TipoDocumentoEmissao ?? Dominio.Enumeradores.TipoDocumento.Outros,
                                 Descricao = obj.Numero,
                                 obj.Numero,
                                 SituacaoCTe = obj.Status,
                                 AbreviacaoModeloDocumentoFiscal = obj.ModeloDocumentoFiscal?.Abreviacao ?? string.Empty,
                                 NumeroModeloDocumentoFiscal = obj.ModeloDocumentoFiscal?.Numero ?? string.Empty,
                                 Serie = obj.Serie.Numero,
                                 DataEmissao = obj.DataEmissao.Value.ToString("dd/MM/yyyy HH:mm"),
                                 Emitente = obj.Empresa.RazaoSocial + "(" + obj.Empresa.CNPJ_Formatado + ")",
                                 Remetente = obj.Remetente != null ? obj.Remetente.Nome + "(" + obj.Remetente.CPF_CNPJ_Formatado + ")" : string.Empty,
                                 Origem = obj.LocalidadeInicioPrestacao.DescricaoCidadeEstado,
                                 Destinatario = obj.Destinatario != null ? obj.Destinatario.Nome + "(" + obj.Destinatario.CPF_CNPJ_Formatado + ")" : string.Empty,
                                 Destino = obj.LocalidadeTerminoPrestacao.DescricaoCidadeEstado,
                                 ValorFrete = obj.ValorAReceber.ToString("n2"),
                                 Status = obj.DescricaoStatus,
                                 RetornoSEFAZ = !string.IsNullOrWhiteSpace(obj.MensagemRetornoSefaz) ? obj.MensagemRetornoSefaz != " - " ? System.Web.HttpUtility.HtmlEncode(obj.MensagemRetornoSefaz) : "" : "",
                                 obj.Chave,
                                 TipoServico = Dominio.Enumeradores.TipoServicoHelper.ObterDescricao(obj.TipoServico),

                                 //Emissão Multimodal
                                 obj.NumeroControle,
                                 obj.DescricaoTipoServico,
                                 Viagem = obj.Viagem?.Descricao,
                                 PortoOrigem = obj.PortoOrigem?.Descricao,
                                 PortoDestino = obj.PortoDestino?.Descricao,
                                 DataAutorizacao = obj.DataAutorizacao?.ToString("dd/MM/yyyy HH:mm") ?? string.Empty,
                                 Documentos = obj.ModeloDocumentoFiscal?.Numero != "57" ? "CTM LC" :
                                    obj.TipoServico == Dominio.Enumeradores.TipoServico.ServVinculadoMultimodal ? "SVM" :
                                    obj.TipoModal == TipoModal.Aquaviario ? "CTAC" :
                                    obj.TipoModal == TipoModal.Multimodal ? "CTMC" : "CTRC",
                                 TipoDocumento = obj.TipoCTE == 0 ? "Regular" : "Manual",

                                 DT_RowColor = (obj.Status == "A" ? "#dff0d8" : obj.Status == "R" ? "rgba(193, 101, 101, 1)" : (obj.Status == "C" || obj.Status == "I" || obj.Status == "D" || obj.Status == "Z") ? "#777" : ""),
                                 DT_FontColor = ((obj.Status == "R" || obj.Status == "C" || obj.Status == "I" || obj.Status == "D" || obj.Status == "Z") ? "#FFFFFF" : ""),
                             }).ToList();

                grid.AdicionaRows(lista);

                return new JsonpResult(grid);
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);
                return new JsonpResult(false, "Ocorreu uma falha ao consultar os CT-es.");
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        [AllowAuthenticate]
        public async Task<IActionResult> PesquisaDocumentoAgregado()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);
            try
            {
                Repositorio.ConhecimentoDeTransporteEletronico repCTe = new Repositorio.ConhecimentoDeTransporteEletronico(unitOfWork);
                Repositorio.Cliente repCliente = new Repositorio.Cliente(unitOfWork);

                int codigoMotorista, codigoPagamentoAgregado, numero, codigoTipoOcorrencia;
                int.TryParse(Request.Params("Motorista"), out codigoMotorista);
                int.TryParse(Request.Params("PagamentoAgregado"), out codigoPagamentoAgregado);
                int.TryParse(Request.Params("Numero"), out numero);
                int.TryParse(Request.Params("TipoOcorrencia"), out codigoTipoOcorrencia);

                double cliente, destinatario;
                double.TryParse(Request.Params("Cliente"), out cliente);
                double.TryParse(Request.Params("Destinatario"), out destinatario);

                string cnpjAgregado = "";
                if (cliente > 0)
                    cnpjAgregado = repCliente.BuscarPorCPFCNPJ(cliente).CPF_CNPJ_SemFormato;

                DateTime dataEmissao, dataInicial, dataFinal;
                DateTime.TryParse(Request.Params("DataEmissao"), out dataEmissao);
                DateTime.TryParse(Request.Params("DataInicial"), out dataInicial);
                DateTime.TryParse(Request.Params("DataFinal"), out dataFinal);

                Models.Grid.Grid grid = new Models.Grid.Grid(Request);

                grid.header = new List<Models.Grid.Head>();
                grid.AdicionarCabecalho("Codigo", false);
                grid.AdicionarCabecalho("CodigoDocumentoPagamentoAgregado", false);
                grid.AdicionarCabecalho("CodigoDocumento", false);
                grid.AdicionarCabecalho("CodigoPagamento", false);
                grid.AdicionarCabecalho("Número", "Numero", 8, Models.Grid.Align.center, true);
                grid.AdicionarCabecalho("Série", "Serie", 8, Models.Grid.Align.center, true);
                grid.AdicionarCabecalho("Valor CT-e", "ValorCTe", 12, Models.Grid.Align.right, false);
                grid.AdicionarCabecalho("ValorPagamento", false);
                grid.AdicionarCabecalho("Remetente", false);
                grid.AdicionarCabecalho("CIOT", false);
                grid.AdicionarCabecalho("Destinatário", "Destinatario", 20, Models.Grid.Align.left, false);
                grid.AdicionarCabecalho("Última Ocorrência", "UltimaOcorrencia", 12, Models.Grid.Align.center, false);
                grid.AdicionarCabecalho("Situação do CT-e", "Status", 20, Models.Grid.Align.left, false);
                grid.AdicionarCabecalho("Motorista(s)", "Motorista", 20, Models.Grid.Align.left, false);

                string propOrdenacao = grid.header[grid.indiceColunaOrdena].data;

                var dynListaCarga = repCTe.BuscarPorDocumentoAgregado(false, 0, null, null, false, codigoTipoOcorrencia, numero, dataInicial, dataFinal, codigoMotorista, cnpjAgregado, codigoPagamentoAgregado, destinatario, propOrdenacao, grid.dirOrdena, grid.inicio, grid.limite);
                var dynRetorno = (from obj in dynListaCarga
                                  select new
                                  {
                                      Codigo = obj.Codigo,
                                      CodigoDocumentoPagamentoAgregado = 0,
                                      CodigoDocumento = obj.Codigo,
                                      CodigoPagamento = codigoPagamentoAgregado,
                                      Numero = obj.Numero.ToString("n0"),
                                      Serie = obj.Serie.Numero.ToString("n0"),
                                      ValorCTe = obj.ValorAReceber.ToString("n2"),
                                      ValorPagamento = 0.ToString("n2"),
                                      Remetente = obj.Remetente?.Nome,
                                      Destinatario = obj.Destinatario?.Nome,
                                      CIOT = "",
                                      UltimaOcorrencia = UltimaOcorrenciaDocumento(obj.Codigo, unitOfWork),
                                      Status = obj.DescricaoStatus,
                                      Motorista = obj.Motoristas != null && obj.Motoristas.Count > 0 ? String.Join(",", obj.Motoristas.Select(o => o.NomeMotorista).ToList()) : string.Empty
                                  }).ToList();

                grid.setarQuantidadeTotal(repCTe.ContarBuscarPorDocumentoAgregado(0, null, null, false, codigoTipoOcorrencia, numero, dataInicial, dataFinal, codigoMotorista, cnpjAgregado, codigoPagamentoAgregado, destinatario));
                grid.AdicionaRows(dynRetorno);

                return new JsonpResult(grid);
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);
                return new JsonpResult(false, "Ocorreu uma falha ao consultar.");
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        [AcceptVerbs("POST", "GET")]
        public async Task<IActionResult> DownloadDACTE()
        {
            Repositorio.UnitOfWork unidadeTrabalho = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                int codigoCTe = Request.GetIntParam("CTe");

                Repositorio.ConhecimentoDeTransporteEletronico repCTe = new Repositorio.ConhecimentoDeTransporteEletronico(unidadeTrabalho);
                Dominio.Entidades.ConhecimentoDeTransporteEletronico cte = repCTe.BuscarPorCodigo(codigoCTe);

                if (cte == null)
                    return new JsonpResult(false, true, "CT-e não encontrado, atualize a página e tente novamente.");

                if (cte.Status != "A" && cte.Status != "C" && cte.Status != "K" && cte.Status != "Z")
                    return new JsonpResult(false, true, "O status do CT-e não permite a geração do DACTE.");

                string nomeArquivo = "";

                if (cte.ModeloDocumentoFiscal.TipoDocumentoEmissao != Dominio.Enumeradores.TipoDocumento.CTe && cte.ModeloDocumentoFiscal.TipoDocumentoEmissao != Dominio.Enumeradores.TipoDocumento.NFSe && cte.ModeloDocumentoFiscal.TipoDocumentoEmissao != Dominio.Enumeradores.TipoDocumento.NFS)
                {
                    nomeArquivo = cte.Numero + "_" + cte.Serie.Numero + "_" + cte.ModeloDocumentoFiscal.Abreviacao;

                    if (!string.IsNullOrWhiteSpace(cte.ModeloDocumentoFiscal.Relatorio))
                    {
                        byte[] arquivo = new Servicos.Embarcador.Relatorios.OutrosDocumentos(unidadeTrabalho).ObterPdf(cte);

                        return Arquivo(arquivo, "application/pdf", nomeArquivo + ".pdf");
                    }
                }

                if (string.IsNullOrWhiteSpace(Servicos.Embarcador.Configuracoes.ConfigurationInstance.GetInstance(unidadeTrabalho).ObterConfiguracaoArquivo().CaminhoRelatorios))
                    return new JsonpResult(false, true, "O caminho para o download do DACTE não está disponível. Contate o suporte técnico.");

                string nomeArquivoDownload = Servicos.Embarcador.CTe.CTe.ObterNomeArquivoDownloadCTe(cte, "pdf");

                byte[] pdf = null;

                if (cte.ModeloDocumentoFiscal.TipoDocumentoEmissao == Dominio.Enumeradores.TipoDocumento.NFSe || cte.ModeloDocumentoFiscal.TipoDocumentoEmissao == Dominio.Enumeradores.TipoDocumento.NFS)
                {
                    nomeArquivo = cte.Numero.ToString() + "_" + cte.Serie.Numero.ToString() + ".pdf";

                    Servicos.NFSe svcNFSe = new Servicos.NFSe(unidadeTrabalho);

                    pdf = svcNFSe.ObterDANFSECTe(cte.Codigo);
                }
                else
                {
                    nomeArquivo = cte.Chave;

                    if (cte.ModeloDocumentoFiscal.Numero != "57")
                        nomeArquivo = cte.ModeloDocumentoFiscal.Numero + "_" + cte.Numero + "_" + cte.Serie.Numero + "_" + cte.ModeloDocumentoFiscal.Abreviacao + "_" + cte.Codigo;

                    string caminhoPDF = Utilidades.IO.FileStorageService.Storage.Combine(Servicos.Embarcador.Configuracoes.ConfigurationInstance.GetInstance(unidadeTrabalho).ObterConfiguracaoArquivo().CaminhoRelatorios, cte.Empresa.CNPJ, nomeArquivo) + ".pdf";

                    nomeArquivo = System.IO.Path.GetFileName(caminhoPDF);

                    if (!Utilidades.IO.FileStorageService.Storage.Exists(caminhoPDF))
                    {
                        if (string.IsNullOrWhiteSpace(Servicos.Embarcador.Configuracoes.ConfigurationInstance.GetInstance(unidadeTrabalho).ObterConfiguracaoArquivo().CaminhoGeradorRelatorios))
                            return new JsonpResult(false, true, "O gerador do PDF não está disponível. Contate o suporte técnico.");

                        Servicos.DACTE svcDACTE = new Servicos.DACTE(unidadeTrabalho);

                        pdf = svcDACTE.GerarPorProcesso(cte.Codigo, unidadeTrabalho);
                    }
                    else
                    {
                        pdf = Utilidades.IO.FileStorageService.Storage.ReadAllBytes(caminhoPDF);
                    }
                }

                if (string.IsNullOrWhiteSpace(nomeArquivoDownload))
                    nomeArquivoDownload = nomeArquivo;

                if (pdf != null)
                    return Arquivo(pdf, "application/pdf", nomeArquivoDownload);
                else
                    return new JsonpResult(false, true, "Não foi possível gerar o PDF, atualize a página e tente novamente.");
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);
                return new JsonpResult(false, "Ocorreu uma falha ao realizar o download do PDF.");
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

                Servicos.CTe svcCTe = new Servicos.CTe(unidadeTrabalho);

                if (cte == null)
                    return new JsonpResult(false, false, "CT-e não encontrado, atualize a página e tente novamente.");

                byte[] data = svcCTe.ObterXMLAutorizacao(cte, unidadeTrabalho, Auditado, TipoServicoMultisoftware);

                if (data != null)
                {
                    string nomeArquivoDownload = Servicos.Embarcador.CTe.CTe.ObterNomeArquivoDownloadCTe(cte, "xml");

                    if (string.IsNullOrWhiteSpace(nomeArquivoDownload))
                    {
                        if (cte.ModeloDocumentoFiscal.TipoDocumentoEmissao == Dominio.Enumeradores.TipoDocumento.CTe)
                            nomeArquivoDownload = string.Concat(cte.Chave, ".xml");
                        else
                            nomeArquivoDownload = string.Concat("NFSe_", cte.Numero, ".xml");
                    }

                    return Arquivo(data, "text/xml", nomeArquivoDownload);
                }
                else
                    return new JsonpResult(false, false, "XML não encontrado, atualize a página e tente novamente.");
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);

                return new JsonpResult(false, false, "Ocorreu uma falha ao realizar o download do XML.");
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
                    return new JsonpResult(false, false, "CT-e não encontrado, atualize a página e tente novamente.");

                Servicos.CTe svcCTe = new Servicos.CTe(unidadeTrabalho);

                byte[] data = svcCTe.ObterXMLCancelamento(cte, unidadeTrabalho);

                if (data != null)
                    return Arquivo(data, "text/xml", string.Concat(cte.Chave + "-procCancCTe", ".xml"));
                else
                    return new JsonpResult(false, false, "XML não encontrado, atualize a página e tente novamente.");
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);

                return new JsonpResult(false, false, "Ocorreu uma falha ao realizar o download do XML.");
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
                    return new JsonpResult(false, false, "CT-e não encontrado, atualize a página e tente novamente.");

                Servicos.CTe svcCTe = new Servicos.CTe(unidadeTrabalho);

                byte[] data = svcCTe.ObterXMLInutilizacao(cte, unidadeTrabalho);

                if (data != null)
                    return Arquivo(data, "text/xml", string.Concat(cte.Chave + "-procInutCTe", ".xml"));
                else
                    return new JsonpResult(false, false, "XML não encontrado, atualize a página e tente novamente.");
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);

                return new JsonpResult(false, false, "Ocorreu uma falha ao realizar o download do XML.");
            }
            finally
            {
                unidadeTrabalho.Dispose();
            }
        }

        [AcceptVerbs("POST", "GET")]
        public async Task<IActionResult> DownloadLoteXML()
        {
            Repositorio.UnitOfWork unidadeTrabalho = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                Repositorio.ConhecimentoDeTransporteEletronico repCTe = new Repositorio.ConhecimentoDeTransporteEletronico(unidadeTrabalho);
                Dominio.ObjetosDeValor.Embarcador.CTe.FiltroPesquisaConsultaCTe filtrosPesquisa = ObterFiltrosPesquisa(unidadeTrabalho);

                List<int> listaCodigosCtes = repCTe.BuscarListaCodigosCTes(filtrosPesquisa);

                if (listaCodigosCtes.Count > 20000)
                    return new JsonpResult(false, true, "Quantidade de CT-es para geração de lote inválida (" + listaCodigosCtes.Count + "). É permitido o download de um lote com o máximo de 20000 CT-es.");

                Servicos.CTe svcCTe = new Servicos.CTe(unidadeTrabalho);

                return Arquivo(svcCTe.ObterLoteDeXML(listaCodigosCtes, filtrosPesquisa.CodigoEmpresa, unidadeTrabalho), "application/zip", "LoteXML.zip");
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);

                return new JsonpResult(false, false, "Ocorreu uma falha ao realizar o download dos arquivos.");
            }
            finally
            {
                unidadeTrabalho.Dispose();
            }
        }

        [AcceptVerbs("POST", "GET")]
        public async Task<IActionResult> DownloadLoteDACTE()
        {
            Repositorio.UnitOfWork unidadeTrabalho = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                Repositorio.ConhecimentoDeTransporteEletronico repCTe = new Repositorio.ConhecimentoDeTransporteEletronico(unidadeTrabalho);
                Dominio.ObjetosDeValor.Embarcador.CTe.FiltroPesquisaConsultaCTe filtrosPesquisa = ObterFiltrosPesquisa(unidadeTrabalho);

                List<int> listaCodigosCtes = repCTe.BuscarListaCodigosCTes(filtrosPesquisa);
                List<int> listaCodigosNFSes = repCTe.BuscarListaCodigosNFSe(filtrosPesquisa);

                if (listaCodigosCtes.Count > 5000)
                    return new JsonpResult(false, true, "Quantidade de CT-es para geração de lote inválida (" + listaCodigosCtes.Count + "). É permitido o download de um lote com o máximo de 5000 CT-es.");

                Servicos.CTe svcCTe = new Servicos.CTe(unidadeTrabalho);

                return Arquivo(svcCTe.ObterLoteDeDACTE(listaCodigosCtes, listaCodigosNFSes, unidadeTrabalho), "application/zip", "LoteDACTE.zip");
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);

                return new JsonpResult(false, false, "Ocorreu uma falha ao realizar o download dos arquivos.");
            }
            finally
            {
                unidadeTrabalho.Dispose();
            }
        }

        public async Task<IActionResult> EnviarPorEmail()
        {
            Repositorio.UnitOfWork unidadeTrabalho = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                int codigoCTe = 0;
                int.TryParse(Request.Params("CTe"), out codigoCTe);

                string emails = Request.Params("Emails");

                if (!Utilidades.Email.ValidarEmails(emails, ';'))
                    return new JsonpResult(false, true, "E-mail inválido.");

                Repositorio.ConhecimentoDeTransporteEletronico repCTe = new Repositorio.ConhecimentoDeTransporteEletronico(unidadeTrabalho);
                Repositorio.Embarcador.Email.ConfigEmailDocTransporte repConfigEmailDocTransporte = new Repositorio.Embarcador.Email.ConfigEmailDocTransporte(unidadeTrabalho);

                Dominio.Entidades.Embarcador.Email.ConfigEmailDocTransporte email = repConfigEmailDocTransporte.BuscarEmailEnviaDocumentoAtivo(0);
                Dominio.Entidades.ConhecimentoDeTransporteEletronico cte = repCTe.BuscarPorCodigo(codigoCTe);

                if (cte == null)
                    return new JsonpResult(false, true, "CT-e não encontrado.");

                if (cte.Status == "E" || cte.Status == "R")
                    return new JsonpResult(false, true, "A situação do CT-e não permite o envio por e-mail.");

                List<string> emailsEnviar = new List<string>();
                emailsEnviar = emails.Split(';').ToList();
                emailsEnviar = emailsEnviar.Distinct().ToList();

                if (emailsEnviar == null || emailsEnviar.Count == 0)
                    return new JsonpResult(false, true, "Nenhum e-mail informado.");

                if (ConfiguracaoEmbarcador.UtilizaEmissaoMultimodal)
                {
                    Dictionary<string, byte[]> conteudoCompactar = new Dictionary<string, byte[]>();
                    string mensagemErro = "Erro ao enviar e-mail";
                    string assunto = "Conhecimento Eletrônico " + cte.Empresa.RazaoSocial;

                    string mensagemEmail = string.Empty;
                    mensagemEmail = "<br><br><div>";
                    mensagemEmail += "<table align='center' width='700' cellpadding='0' cellspacing='0' bgcolor='#D3D3D3' border='1'>";
                    mensagemEmail += "<tbody>";
                    mensagemEmail += "<tr>";
                    mensagemEmail += "<td align='center'><span><font face='Arial' color='#000000' size='2'><b>Conhecimento Eletrônico - Arquivos de Autorização de uso</b></font></span></td>";
                    mensagemEmail += "</tr>";
                    mensagemEmail += "</tbody>";
                    mensagemEmail += "</table><br>";

                    mensagemEmail += "<table align='center' width='700' cellpadding='0' cellspacing='0' border='1'>";
                    mensagemEmail += "<tbody>";
                    mensagemEmail += "<tr bgcolor='#D3D3D3'>";
                    mensagemEmail += "<td align='center' colspan='4'><span><font face='Arial' color='#000000' size='2'><b>Relação de CT-e anexo(s)</b></font></span></td>";
                    mensagemEmail += "</tr>";
                    mensagemEmail += "<tr bgcolor='#D3D3D3'>";
                    mensagemEmail += "<td align='center'><span><font face='Arial' color='#000000' size='2'><b>Número</b></font></span></td>";
                    mensagemEmail += "<td align='center'><span><font face='Arial' color='#000000' size='2'><b>Série</b></font></span></td>";
                    mensagemEmail += "<td align='center'><span><font face='Arial' color='#000000' size='2'><b>Data Emissão</b></font></span></td>";
                    mensagemEmail += "<td align='center'><span><font face='Arial' color='#000000' size='2'><b>Chave de Acesso</b></font></span></td>";
                    mensagemEmail += "</tr>";


                    if (cte.Status != "E" && cte.Status != "R")
                    {
                        mensagemEmail += "<tr>";
                        mensagemEmail += "<td align='center'><span><font face='Arial' color='#000000' size='2'>" + cte.Numero.ToString("D") + "</font></span></td>";
                        mensagemEmail += "<td align='center'><span><font face='Arial' color='#000000' size='2'>" + cte.Serie.Numero.ToString("D") + "</font></span></td>";
                        mensagemEmail += "<td align='center'><span><font face='Arial' color='#000000' size='2'>" + cte.DataEmissao.Value.ToString("dd/MM/yyyy") + "</font></span></td>";
                        mensagemEmail += "<td align='center'><span><font face='Arial' color='#000000' size='2'>" + cte.Chave + "</font></span></td>";
                        mensagemEmail += "</tr>";

                        string nomeArquivo = "";
                        byte[] data = null;
                        string caminhoPDF = Utilidades.IO.FileStorageService.Storage.Combine(Servicos.Embarcador.Configuracoes.ConfigurationInstance.GetInstance(unidadeTrabalho).ObterConfiguracaoArquivo().CaminhoRelatorios, cte.Empresa.CNPJ, cte.Chave) + ".pdf";
                        if (Utilidades.IO.FileStorageService.Storage.Exists(caminhoPDF))
                        {
                            nomeArquivo = Path.GetFileName(caminhoPDF);
                            data = Utilidades.IO.FileStorageService.Storage.ReadAllBytes(caminhoPDF);
                            conteudoCompactar.Add(nomeArquivo, data);
                        }

                        if (cte.ModeloDocumentoFiscal.Numero == "39")
                        {
                            nomeArquivo = cte.Numero.ToString() + "_" + cte.Serie.Numero.ToString() + ".xml";
                            Servicos.NFSe svcNFSe = new Servicos.NFSe();
                            data = svcNFSe.ObterXMLAutorizacaoCTe(cte.Codigo, unidadeTrabalho);
                            if (data != null)
                                conteudoCompactar.Add(nomeArquivo, data);
                        }
                        else
                        {
                            nomeArquivo = string.Concat(cte.Chave, ".xml");
                            Servicos.CTe svcCTe = new Servicos.CTe(unidadeTrabalho);
                            data = svcCTe.ObterXMLAutorizacao(cte, unidadeTrabalho);
                            if (data != null)
                                conteudoCompactar.Add(nomeArquivo, data);
                        }
                    }


                    mensagemEmail += "</tbody>";
                    mensagemEmail += "</table>";
                    mensagemEmail += "</div>";
                    mensagemEmail += "<br/><br/>E -mail enviado automaticamente. Por favor, não responda.";

                    if (emailsEnviar.Count > 0)
                    {
                        List<System.Net.Mail.Attachment> attachments = new List<System.Net.Mail.Attachment>();
                        MemoryStream arquivoCompactado = Utilidades.File.GerarArquivoCompactado(conteudoCompactar);
                        if (arquivoCompactado != null)
                            attachments.Add(new System.Net.Mail.Attachment(arquivoCompactado, $"ARQCTE {DateTime.Now.ToString("dd/MM/yyyy HH:mm")}.zip"));

                        bool sucesso = Servicos.Email.EnviarEmail(email.Email, email.Email, email.Senha, null, emailsEnviar.ToArray(), null, assunto, mensagemEmail, email.Smtp, out mensagemErro, email.DisplayEmail,
                                    attachments, "", email.RequerAutenticacaoSmtp, "", email.PortaSmtp, unidadeTrabalho);

                        if (!sucesso)
                            Servicos.Log.TratarErro("Problemas ao enviar os CT-es por e-mail: " + mensagemErro);
                    }

                    return new JsonpResult(true);
                }
                else
                {
                    Servicos.CTe svcCTe = new Servicos.CTe(unidadeTrabalho);

                    if (svcCTe.EnviarEmail(cte.Codigo, emails, unidadeTrabalho))
                        return new JsonpResult(true);
                    else
                        return new JsonpResult(false, true, "Não foi possível enviar o e-mail.");
                }
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);
                return new JsonpResult(false, false, "Ocorreu uma falha ao enviar o e-mail.");
            }
            finally
            {
                unidadeTrabalho.Dispose();
            }
        }

        public async Task<IActionResult> EnviarPorEmailLote()
        {
            Repositorio.UnitOfWork unidadeTrabalho = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                Repositorio.ConhecimentoDeTransporteEletronico repCTe = new Repositorio.ConhecimentoDeTransporteEletronico(unidadeTrabalho);
                Repositorio.Embarcador.Email.ConfigEmailDocTransporte repConfigEmailDocTransporte = new Repositorio.Embarcador.Email.ConfigEmailDocTransporte(unidadeTrabalho);
                Dominio.Entidades.Embarcador.Email.ConfigEmailDocTransporte email = repConfigEmailDocTransporte.BuscarEmailEnviaDocumentoAtivo(0);

                string emails = Request.Params("Emails");

                if (!Utilidades.Email.ValidarEmails(emails, ';'))
                    return new JsonpResult(false, true, "E-mail inválido.");

                if (email == null)
                    return new JsonpResult(false, true, "Não há um e-mail configurado para realizar o envio.");

                List<string> emailsEnviar = new List<string>();
                emailsEnviar = emails.Split(';').ToList();
                emailsEnviar = emailsEnviar.Distinct().ToList();

                if (emailsEnviar == null || emailsEnviar.Count == 0)
                    return new JsonpResult(false, true, "Nenhum e-mail informado.");

                Dominio.ObjetosDeValor.Embarcador.CTe.FiltroPesquisaConsultaCTe filtrosPesquisa = ObterFiltrosPesquisa(unidadeTrabalho);
                List<int> listaCodigosCtes = repCTe.BuscarListaCodigosCTes(filtrosPesquisa);

                if (listaCodigosCtes.Count > 5000)
                    return new JsonpResult(false, true, "Quantidade de CT-es para geração de e-mail em lote inválida (" + listaCodigosCtes.Count + "). É permitido o envio de um lote com o máximo de 5000 CT-es.");

                List<Dominio.Entidades.ConhecimentoDeTransporteEletronico> ctes = repCTe.BuscarPorCodigo(listaCodigosCtes);

                if (ctes == null || ctes.Count == 0)
                    return new JsonpResult(false, true, "CT-es não encontrado.");

                Dictionary<string, byte[]> conteudoCompactar = new Dictionary<string, byte[]>();
                string mensagemErro = "Erro ao enviar e-mail";
                string assunto = "Conhecimento Eletrônico " + ctes[0].Empresa.RazaoSocial;

                string mensagemEmail = string.Empty;
                mensagemEmail = "<br><br><div>";
                mensagemEmail += "<table align='center' width='700' cellpadding='0' cellspacing='0' bgcolor='#D3D3D3' border='1'>";
                mensagemEmail += "<tbody>";
                mensagemEmail += "<tr>";
                mensagemEmail += "<td align='center'><span><font face='Arial' color='#000000' size='2'><b>Conhecimento Eletrônico - Arquivos de Autorização de uso</b></font></span></td>";
                mensagemEmail += "</tr>";
                mensagemEmail += "</tbody>";
                mensagemEmail += "</table><br>";

                mensagemEmail += "<table align='center' width='700' cellpadding='0' cellspacing='0' border='1'>";
                mensagemEmail += "<tbody>";
                mensagemEmail += "<tr bgcolor='#D3D3D3'>";
                mensagemEmail += "<td align='center' colspan='4'><span><font face='Arial' color='#000000' size='2'><b>Relação de CT-e anexo(s)</b></font></span></td>";
                mensagemEmail += "</tr>";
                mensagemEmail += "<tr bgcolor='#D3D3D3'>";
                mensagemEmail += "<td align='center'><span><font face='Arial' color='#000000' size='2'><b>Número</b></font></span></td>";
                mensagemEmail += "<td align='center'><span><font face='Arial' color='#000000' size='2'><b>Série</b></font></span></td>";
                mensagemEmail += "<td align='center'><span><font face='Arial' color='#000000' size='2'><b>Data Emissão</b></font></span></td>";
                mensagemEmail += "<td align='center'><span><font face='Arial' color='#000000' size='2'><b>Chave de Acesso</b></font></span></td>";
                mensagemEmail += "</tr>";

                foreach (var cte in ctes)
                {
                    if (cte.Status != "E" && cte.Status != "R")
                    {
                        mensagemEmail += "<tr>";
                        mensagemEmail += "<td align='center'><span><font face='Arial' color='#000000' size='2'>" + cte.Numero.ToString("D") + "</font></span></td>";
                        mensagemEmail += "<td align='center'><span><font face='Arial' color='#000000' size='2'>" + cte.Serie.Numero.ToString("D") + "</font></span></td>";
                        mensagemEmail += "<td align='center'><span><font face='Arial' color='#000000' size='2'>" + cte.DataEmissao.Value.ToString("dd/MM/yyyy") + "</font></span></td>";
                        mensagemEmail += "<td align='center'><span><font face='Arial' color='#000000' size='2'>" + cte.Chave + "</font></span></td>";
                        mensagemEmail += "</tr>";

                        string nomeArquivo = "";
                        byte[] data = null;
                        string caminhoPDF = Utilidades.IO.FileStorageService.Storage.Combine(Servicos.Embarcador.Configuracoes.ConfigurationInstance.GetInstance(unidadeTrabalho).ObterConfiguracaoArquivo().CaminhoRelatorios, cte.Empresa.CNPJ, cte.Chave) + ".pdf";
                        if (Utilidades.IO.FileStorageService.Storage.Exists(caminhoPDF))
                        {
                            nomeArquivo = Path.GetFileName(caminhoPDF);
                            data = Utilidades.IO.FileStorageService.Storage.ReadAllBytes(caminhoPDF);
                            conteudoCompactar.Add(nomeArquivo, data);
                        }

                        if (cte.ModeloDocumentoFiscal.Numero == "39")
                        {
                            nomeArquivo = cte.Numero.ToString() + "_" + cte.Serie.Numero.ToString() + ".xml";
                            Servicos.NFSe svcNFSe = new Servicos.NFSe();
                            data = svcNFSe.ObterXMLAutorizacaoCTe(cte.Codigo, unidadeTrabalho);
                            if (data != null)
                                conteudoCompactar.Add(nomeArquivo, data);
                        }
                        else
                        {
                            nomeArquivo = string.Concat(cte.Chave, ".xml");
                            Servicos.CTe svcCTe = new Servicos.CTe(unidadeTrabalho);
                            data = svcCTe.ObterXMLAutorizacao(cte, unidadeTrabalho);
                            if (data != null)
                                conteudoCompactar.Add(nomeArquivo, data);
                        }
                    }
                }

                mensagemEmail += "</tbody>";
                mensagemEmail += "</table>";
                mensagemEmail += "</div>";
                mensagemEmail += "<br/><br/>E -mail enviado automaticamente. Por favor, não responda.";

                if (emailsEnviar.Count > 0)
                {
                    List<System.Net.Mail.Attachment> attachments = new List<System.Net.Mail.Attachment>();
                    MemoryStream arquivoCompactado = Utilidades.File.GerarArquivoCompactado(conteudoCompactar);
                    if (arquivoCompactado != null)
                        attachments.Add(new System.Net.Mail.Attachment(arquivoCompactado, $"ARQCTE {DateTime.Now.ToString("dd/MM/yyyy HH:mm")}.zip"));

                    bool sucesso = Servicos.Email.EnviarEmail(email.Email, email.Email, email.Senha, null, emailsEnviar.ToArray(), null, assunto, mensagemEmail, email.Smtp, out mensagemErro, email.DisplayEmail,
                                attachments, "", email.RequerAutenticacaoSmtp, "", email.PortaSmtp, unidadeTrabalho);

                    if (!sucesso)
                        Servicos.Log.TratarErro("Problemas ao enviar os CT-es por e-mail: " + mensagemErro);
                }

                return new JsonpResult(true);
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);
                return new JsonpResult(false, false, "Ocorreu uma falha ao enviar o e-mail.");
            }
            finally
            {
                unidadeTrabalho.Dispose();
            }
        }

        [AcceptVerbs("GET", "POST")]
        public async Task<IActionResult> DownloadCONEMB()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                int.TryParse(Request.Params("LayoutCONEMB"), out int codigoLayoutEDI);

                Repositorio.LayoutEDI repLayoutEDI = new Repositorio.LayoutEDI(unitOfWork);

                Dominio.Entidades.LayoutEDI layoutEDI = repLayoutEDI.BuscarPorCodigo(codigoLayoutEDI);

                if (layoutEDI == null)
                    return new JsonpResult(false, true, "O layout do EDI não foi encontrado.");

                List<Dominio.Enumeradores.TipoLayoutEDI> layoutsPermitidos = new List<Dominio.Enumeradores.TipoLayoutEDI>() {
                    Dominio.Enumeradores.TipoLayoutEDI.CONEMB,
                    Dominio.Enumeradores.TipoLayoutEDI.CONEMB_CT_EXP,
                    Dominio.Enumeradores.TipoLayoutEDI.CONEMB_CT_IMP,
                    Dominio.Enumeradores.TipoLayoutEDI.CONEMB_MB
                };

                if (!layoutsPermitidos.Contains(layoutEDI.Tipo))
                    return new JsonpResult(false, true, "O layout EDI selecionado não permite o download.");

                if (!ExecutarPesquisa(out string erro, out List<Dominio.Entidades.ConhecimentoDeTransporteEletronico> ctes, out int countCTes, "Numero", "asc", 0, 0, unitOfWork, true, 1000))
                    return new JsonpResult(false, true, erro);

                if (countCTes <= 0)
                    return new JsonpResult(false, true, "Nenhum CT-e encontrado.");

                string nomeArquivo = Servicos.Embarcador.Integracao.IntegracaoEDI.ObterNomeArquivoEDI(ctes.First(), layoutEDI, null, unitOfWork);

                if (layoutEDI.Tipo == Dominio.Enumeradores.TipoLayoutEDI.CONEMB_CT_IMP)
                {
                    Servicos.Embarcador.Integracao.EDI.CONEMB svcCONEMB = new Servicos.Embarcador.Integracao.EDI.CONEMB();
                    Dominio.ObjetosDeValor.EDI.CONEMB.EDICONEMBCaterpillarImportacao conemb = svcCONEMB.ConverterCargaCTeParaCONEMB_CaterpillarImportacao(ctes, null, unitOfWork);

                    Servicos.GeracaoEDI serGeracaoEDI = new Servicos.GeracaoEDI(unitOfWork, layoutEDI, ctes.First().Empresa);

                    System.IO.MemoryStream conembRetorno = serGeracaoEDI.GerarArquivoRecursivo(conemb);

                    return Arquivo(conembRetorno, "text/plain", nomeArquivo);
                }
                else if (layoutEDI.Tipo == Dominio.Enumeradores.TipoLayoutEDI.CONEMB_CT_EXP)
                {
                    Servicos.Embarcador.Integracao.EDI.CONEMB svcCONEMB = new Servicos.Embarcador.Integracao.EDI.CONEMB();
                    Dominio.ObjetosDeValor.EDI.CONEMB.EDICONEMBCaterpillarExportacao conemb = svcCONEMB.ConverterCargaCTeParaCONEMB_CaterpillarExportacao(ctes, null, unitOfWork);

                    Servicos.GeracaoEDI serGeracaoEDI = new Servicos.GeracaoEDI(unitOfWork, layoutEDI, ctes.First().Empresa);

                    System.IO.MemoryStream conembRetorno = serGeracaoEDI.GerarArquivoRecursivo(conemb);

                    return Arquivo(conembRetorno, "text/plain", nomeArquivo);
                }
                else if (layoutEDI.Tipo == Dominio.Enumeradores.TipoLayoutEDI.CONEMB_MB)
                {
                    Servicos.Embarcador.Integracao.EDI.CONEMB svcCONEMB = new Servicos.Embarcador.Integracao.EDI.CONEMB();
                    Dominio.ObjetosDeValor.EDI.CONEMB.EDICONEMB conemb = svcCONEMB.ConverterCargaCTeParaCONEMB_MartinBrower(ctes, null, unitOfWork);

                    Servicos.GeracaoEDI serGeracaoEDI = new Servicos.GeracaoEDI(unitOfWork, layoutEDI, ctes.First().Empresa);

                    System.IO.MemoryStream conembRetorno = serGeracaoEDI.GerarArquivoRecursivo(conemb);

                    return Arquivo(conembRetorno, "text/plain", nomeArquivo);
                }
                else
                {
                    Servicos.GeracaoEDI svcEDI = new Servicos.GeracaoEDI(unitOfWork, layoutEDI, ctes.First().Empresa, ctes);

                    System.IO.MemoryStream conemb = svcEDI.GerarArquivo();

                    return Arquivo(conemb, "text/plain", nomeArquivo);
                }
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);
                return new JsonpResult(false, "Ocorreu uma falha ao gerar o CONEMB.");
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        [AcceptVerbs("GET", "POST")]
        public async Task<IActionResult> DownloadOCOREN()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                int.TryParse(Request.Params("LayoutOCOREN"), out int codigoLayoutEDI);

                Repositorio.LayoutEDI repLayoutEDI = new Repositorio.LayoutEDI(unitOfWork);

                Dominio.Entidades.LayoutEDI layoutEDI = repLayoutEDI.BuscarPorCodigo(codigoLayoutEDI);

                if (layoutEDI == null)
                    return new JsonpResult(false, true, "O layout do EDI não foi encontrado.");

                if (layoutEDI.Tipo != Dominio.Enumeradores.TipoLayoutEDI.OCOREN)
                    return new JsonpResult(false, true, "O tipo do layout do EDI selecionado não permite o donwload.");

                if (!ExecutarPesquisa(out string erro, out List<Dominio.Entidades.ConhecimentoDeTransporteEletronico> ctes, out int countCTes, "Numero", "asc", 0, 0, unitOfWork, true, 1000))
                    return new JsonpResult(false, true, erro);

                if (countCTes <= 0)
                    return new JsonpResult(false, true, "Nenhum CT-e encontrado.");

                string nomeArquivo = Servicos.Embarcador.Integracao.IntegracaoEDI.ObterNomeArquivoEDI(ctes.First(), layoutEDI, null, unitOfWork);

                Servicos.Embarcador.Integracao.EDI.OCOREN svcOcoren = new Servicos.Embarcador.Integracao.EDI.OCOREN();

                Dominio.ObjetosDeValor.EDI.OCOREN.EDIOCOREN ediOCOREN = svcOcoren.ConverterParaOCOREN(ctes, unitOfWork);

                Servicos.GeracaoEDI svcEDI = new Servicos.GeracaoEDI(unitOfWork, layoutEDI, ctes?.FirstOrDefault()?.Empresa);

                System.IO.MemoryStream ocoren = svcEDI.GerarArquivoRecursivo(ediOCOREN);

                return Arquivo(ocoren, "text/plain", nomeArquivo);
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);
                return new JsonpResult(false, "Ocorreu uma falha ao gerar o OCOREN.");
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }


        //[AcceptVerbs("POST", "GET")]
        //public async Task<IActionResult> EmitirCTeRejeitado(CancellationToken cancellationToken)
        //{
        //    Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

        //    try
        //    {
        //        return false;
        //    }
        //    catch (Exception ex)
        //    {
        //        Servicos.Log.TratarErro(ex);
        //        return new JsonpResult(false, "Ocorreu uma falha ao emitir");
        //    }
        //    finally
        //    {
        //        unitOfWork.Dispose();
        //    }
        //}

       #endregion

       #region Métodos Privados

            private bool ExecutarPesquisa(out string erro, out List<Dominio.Entidades.ConhecimentoDeTransporteEletronico> listaCTe, out int countCTes, string propOrdena, string dirOrdena, int inicio, int limite, Repositorio.UnitOfWork unitOfWork, bool limitarQuantidadeRegistros = false, int quantidadeRegistrosLimite = 0)
        {
            if (propOrdena == "Remetente" || propOrdena == "Destinatario")
                propOrdena += ".Nome";
            else if (propOrdena == "Origem")
                propOrdena = "LocalidadeInicioPrestacao.Descricao";
            else if (propOrdena == "Destino")
                propOrdena = "LocalidadeTerminoPrestacao.Descricao";
            if (propOrdena == "AbreviacaoModeloDocumentoFiscal")
                propOrdena = "ModeloDocumentoFiscal.Abreviacao";

            Repositorio.ConhecimentoDeTransporteEletronico repCTe = new Repositorio.ConhecimentoDeTransporteEletronico(unitOfWork);
            Dominio.ObjetosDeValor.Embarcador.CTe.FiltroPesquisaConsultaCTe filtrosPesquisa = ObterFiltrosPesquisa(unitOfWork);

            countCTes = repCTe.ContarConsulta(filtrosPesquisa);

            if (limitarQuantidadeRegistros && quantidadeRegistrosLimite < countCTes)
            {
                listaCTe = null;
                erro = "A quantidade de CT-es para esta operação excede " + quantidadeRegistrosLimite.ToString() + " registros. Utilize os filtros para reduzir a quantidade de registros.";
                return false;
            }

            if (countCTes <= 0)
                listaCTe = new List<Dominio.Entidades.ConhecimentoDeTransporteEletronico>();
            else
                listaCTe = repCTe.Consultar(filtrosPesquisa, propOrdena, dirOrdena, inicio, limite);

            erro = string.Empty;
            return true;
        }

        private Dominio.ObjetosDeValor.Embarcador.CTe.FiltroPesquisaConsultaCTe ObterFiltrosPesquisa(Repositorio.UnitOfWork unitOfWork)
        {
            Dominio.ObjetosDeValor.Embarcador.CTe.FiltroPesquisaConsultaCTe filtrosPesquisaConsultaCTe = new Dominio.ObjetosDeValor.Embarcador.CTe.FiltroPesquisaConsultaCTe()
            {
                DataEmissaoInicial = Request.GetDateTimeParam("DataEmissaoInicial"),
                DataEmissaoFinal = Request.GetDateTimeParam("DataEmissaoFinal"),
                NumeroInicial = Request.GetIntParam("NumeroInicial"),
                NumeroFinal = Request.GetIntParam("NumeroFinal"),
                Serie = Request.GetIntParam("Serie"),
                CodigoOrigem = Request.GetIntParam("Origem"),
                CodigoDestino = Request.GetIntParam("Destino"),
                CodigoGrupoPessoas = Request.GetIntParam("GrupoPessoas"),
                CodigoCarga = Request.GetIntParam("Carga"),
                CodigoEmpresa = TipoServicoMultisoftware == AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiCTe ? Empresa.Codigo : Request.GetIntParam("Empresa"),
                CodigoModeloDocumento = Request.GetIntParam("ModeloDocumento"),
                CodigosTipoOperacao = Request.GetListParam<int>("TipoOperacao"),
                CodigosFatura = Request.GetListParam<int>("Fatura"),
                StatusCTe = Request.GetListParam<string>("Status"),
                Chave = Request.GetStringParam("Chave"),
                DescricaoConsulta = Request.GetStringParam("Descricao"),
                CodigosFiliais = new List<int>(),
                CodigosRecebedores = ObterListaCnpjCpfRecebedorPermitidosOperadorLogistica(unitOfWork),
                CodigoTransportador = Request.GetIntParam("Transportador"),
                VinculoCarga = Request.GetNullableBoolParam("VinculoCarga"),
                NumeroBooking = Request.GetStringParam("NumeroBooking"),
                NumeroOS = Request.GetStringParam("NumeroOS"),
                NumeroSerie = Request.GetIntParam("NumeroSerie"),
                NumeroControleCliente = Request.GetStringParam("NumeroControleCliente"),
                NumeroControle = Request.GetStringParam("NumeroControle"),
                TipoProposta = Request.GetListEnumParam<TipoPropostaMultimodal>("TipoProposta"),
                TipoServico = Request.GetListEnumParam<Dominio.Enumeradores.TipoServico>("TipoServico"),
                Documento = Request.GetNullableBoolParam("TipoDocumento"),
                CodigoTerminalOrigem = Request.GetIntParam("TerminalOrigem"),
                CodigoTerminalDestino = Request.GetIntParam("TerminalDestino"),
                CodigoViagem = Request.GetIntParam("Viagem"),
                CodigoViagemTransbordo = Request.GetIntParam("ViagemTransbordo"),
                CodigoPortoTransbordo = Request.GetIntParam("PortoTransbordo"),
                CodigoPortoOrigem = Request.GetIntParam("PortoOrigem"),
                CodigoPortoDestino = Request.GetIntParam("PortoDestino"),
                Placa = Request.GetStringParam("Placa"),
                TipoCTe = Request.GetEnumParam<Dominio.Enumeradores.TipoCTE>("TipoCTe"),
                VeioPorImportacao = Request.GetEnumParam("VeioPorImportacao", Dominio.Enumeradores.OpcaoSimNaoPesquisa.Todos),
                SomenteCTeSubstituido = Request.GetBoolParam("CTeSubstituido"),
                TipoServicoCarga = Request.GetListParam<TipoServicoMultimodal>("TipoServicoCarga"),
                TipoServicoMultisoftware = TipoServicoMultisoftware,
                CodigosCTe = Request.GetListParam<int>("CTe"),
            };

            int filialFiltro = Request.GetIntParam("Filial");

            if (filialFiltro != 0)
                filtrosPesquisaConsultaCTe.CodigosFiliais.Add(filialFiltro);
            else
                filtrosPesquisaConsultaCTe.CodigosFiliais.AddRange(ObterListaCodigoFilialPermitidasOperadorLogistica(unitOfWork));

            if (string.IsNullOrWhiteSpace(filtrosPesquisaConsultaCTe.Chave) && !string.IsNullOrWhiteSpace(filtrosPesquisaConsultaCTe.DescricaoConsulta) && Utilidades.String.OnlyNumbers(filtrosPesquisaConsultaCTe.DescricaoConsulta.Replace(" ", "")).Length == 44)
                filtrosPesquisaConsultaCTe.Chave = Utilidades.String.OnlyNumbers(filtrosPesquisaConsultaCTe.DescricaoConsulta.Replace(" ", ""));

            int numero = Request.GetIntParam("Numero");

            if (numero > 0 && filtrosPesquisaConsultaCTe.NumeroInicial <= 0 && filtrosPesquisaConsultaCTe.NumeroFinal <= 0)
            {
                filtrosPesquisaConsultaCTe.NumeroInicial = numero;
                filtrosPesquisaConsultaCTe.NumeroFinal = numero;
            }

            Repositorio.Cliente repCliente = new Repositorio.Cliente(unitOfWork);

            double CpfCnpjRemetente = Request.GetDoubleParam("Remetente");
            double CpfCnpjDestinatario = Request.GetDoubleParam("Destinatario");
            double CpfCnpjTomador = Request.GetDoubleParam("Tomador");

            if (CpfCnpjRemetente > 0d)
                filtrosPesquisaConsultaCTe.CpfCnpjRemetente = repCliente.BuscarPorCPFCNPJ(CpfCnpjRemetente)?.CPF_CNPJ_SemFormato ?? null;

            if (CpfCnpjDestinatario > 0d)
                filtrosPesquisaConsultaCTe.CpfCnpjDestinatario = repCliente.BuscarPorCPFCNPJ(CpfCnpjDestinatario)?.CPF_CNPJ_SemFormato ?? null;

            if (CpfCnpjTomador > 0d)
                filtrosPesquisaConsultaCTe.CpfCnpjTomador = repCliente.BuscarPorCPFCNPJ(CpfCnpjTomador)?.CPF_CNPJ_SemFormato ?? null;

            if (TipoServicoMultisoftware == AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.Terceiros)
                filtrosPesquisaConsultaCTe.CpfCnpjTransportadorTerceiro = Usuario?.ClienteTerceiro?.CPF_CNPJ_SemFormato ?? null;

            filtrosPesquisaConsultaCTe.CpfCnpjTomadores = ObterListaCnpjCpfSemFormatoClientePermitidosOperadorLogistica(unitOfWork);

            return filtrosPesquisaConsultaCTe;
        }

        private string UltimaOcorrenciaDocumento(int codigoDocumento, Repositorio.UnitOfWork unitOfWork)
        {
            Repositorio.Embarcador.Ocorrencias.CargaOcorrenciaDocumento repCargaOcorrenciaDocumento = new Repositorio.Embarcador.Ocorrencias.CargaOcorrenciaDocumento(unitOfWork);
            Dominio.Entidades.Embarcador.Ocorrencias.CargaOcorrenciaDocumento ocorrencia = repCargaOcorrenciaDocumento.BuscarUltimaOcorrenciaPorDocumento(codigoDocumento);
            if (ocorrencia != null)
                return ocorrencia.CargaOcorrencia.TipoOcorrencia?.Descricao;
            else
                return "";
        }

        #endregion
    }
}
