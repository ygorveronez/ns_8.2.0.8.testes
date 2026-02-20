using Dominio.ObjetosDeValor.Embarcador.Enumeradores;
using SGTAdmin.Controllers;
using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Mvc;

namespace SGT.WebAdmin.Controllers.Documentos
{
    [CustomAuthorize("Documentos/CIOT")]
    public class CIOTCTeController : BaseController
    {
		#region Construtores

		public CIOTCTeController(Conexao conexao) : base(conexao) { }

		#endregion

        #region Métodos Globais

        public async Task<IActionResult> Pesquisa()
        {
            Repositorio.UnitOfWork unidadeTrabalho = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                int codigoCIOT = 0;
                int.TryParse(Request.Params("CIOT"), out codigoCIOT);

                Models.Grid.Grid grid = new Models.Grid.Grid(Request);
                grid.header = new List<Models.Grid.Head>();
                grid.AdicionarCabecalho("Codigo", false);
                grid.AdicionarCabecalho("SituacaoCTe", false);
                grid.AdicionarCabecalho("Número", "Numero", 10, Models.Grid.Align.center, true);
                grid.AdicionarCabecalho("Serie", "Serie", 8, Models.Grid.Align.center, true);
                grid.AdicionarCabecalho("Destinatário", "Destinatario", 20, Models.Grid.Align.left, true);
                grid.AdicionarCabecalho("Destino", "Destino", 18, Models.Grid.Align.left, true);
                grid.AdicionarCabecalho("Valor a Receber", "ValorFrete", 10, Models.Grid.Align.right, true);
                grid.AdicionarCabecalho("Status", "Status", 10, Models.Grid.Align.center, true);
                grid.AdicionarCabecalho("Retorno SEFAZ", "RetornoSEFAZ", 18, Models.Grid.Align.left, false);

                string propOrdenacao = grid.header[grid.indiceColunaOrdena].data;

                if (propOrdenacao == "Destinatario")
                    propOrdenacao += ".Nome";
                if (propOrdenacao == "Destino")
                    propOrdenacao = "LocalidadeTerminoPrestacao.Descricao";

                propOrdenacao = "CargaCTe.CTe." + propOrdenacao;

                Repositorio.Embarcador.Documentos.CIOTCTe repCIOTCTe = new Repositorio.Embarcador.Documentos.CIOTCTe(unidadeTrabalho);

                List<Dominio.Entidades.Embarcador.Documentos.CIOTCTe> ctes = repCIOTCTe.Consultar(codigoCIOT, propOrdenacao, grid.dirOrdena, grid.inicio, grid.limite);
                int countCTes = repCIOTCTe.ContarConsulta(codigoCIOT);

                grid.setarQuantidadeTotal(countCTes);

                var retorno = (from obj in ctes
                               select new
                               {
                                   obj.CargaCTe.CTe.Codigo,
                                   SituacaoCTe = obj.CargaCTe.CTe.Status,
                                   obj.CargaCTe.CTe.Numero,
                                   Serie = obj.CargaCTe.CTe.Serie.Numero,
                                   Destinatario = obj.CargaCTe.CTe.Destinatario.Nome + "(" + obj.CargaCTe.CTe.Destinatario.CPF_CNPJ_Formatado + ")",
                                   Destino = obj.CargaCTe.CTe.LocalidadeTerminoPrestacao.DescricaoCidadeEstado,
                                   ValorFrete = obj.CargaCTe.CTe.ValorAReceber.ToString("n2"),
                                   Status = obj.CargaCTe.CTe.DescricaoStatus,
                                   RetornoSEFAZ = !string.IsNullOrWhiteSpace(obj.CargaCTe.CTe.MensagemRetornoSefaz) ? obj.CargaCTe.CTe.MensagemRetornoSefaz != " - " ? System.Web.HttpUtility.HtmlEncode(obj.CargaCTe.CTe.MensagemRetornoSefaz) : "" : "",
                                   DT_RowColor = obj.CargaCTe.CTe.Status == "A" ? "#dff0d8" :
                                                 obj.CargaCTe.CTe.Status == "R" ? "rgba(193, 101, 101, 1)" :
                                                 (obj.CargaCTe.CTe.Status == "C" || obj.CargaCTe.CTe.Status == "I" || obj.CargaCTe.CTe.Status == "D") ? "#777" :
                                                 "",
                                   DT_FontColor = (obj.CargaCTe.CTe.Status == "R" || obj.CargaCTe.CTe.Status == "C" || obj.CargaCTe.CTe.Status == "I" || obj.CargaCTe.CTe.Status == "D") ? "#FFFFFF" : "",
                               }).ToList();

                grid.AdicionaRows(retorno);

                return new JsonpResult(grid);
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);

                return new JsonpResult(false, false, "Ocorreu uma falha ao consultar os CT-es.");
            }
            finally
            {
                unidadeTrabalho.Dispose();
            }
        }

        public async Task<IActionResult> PesquisaCargas()
        {
            Repositorio.UnitOfWork unidadeTrabalho = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                Repositorio.Embarcador.Cargas.CargaCIOT repCargaCIOT = new Repositorio.Embarcador.Cargas.CargaCIOT(unidadeTrabalho);
                int.TryParse(Request.Params("CIOT"), out int codigoCIOT);

                Models.Grid.Grid grid = new Models.Grid.Grid(Request)
                {
                    header = new List<Models.Grid.Head>()
                };

                grid.AdicionarCabecalho("Codigo", false);
                grid.AdicionarCabecalho("Carga", "CodigoCargaEmbarcador", 10, Models.Grid.Align.center, true);
                grid.AdicionarCabecalho("Origem", "Origem", 20, Models.Grid.Align.left, false);
                grid.AdicionarCabecalho("Destino", "Destino", 18, Models.Grid.Align.left, false);
                grid.AdicionarCabecalho("Frete", "ValorFrete", 10, Models.Grid.Align.right, true);
                grid.AdicionarCabecalho("Situação", "Situacao", 10, Models.Grid.Align.center, true);

                string propOrdenacao = grid.header[grid.indiceColunaOrdena].data;

                if (propOrdenacao == "Situacao")
                    propOrdenacao = "SituacaoCarga";
                propOrdenacao = "Carga." + propOrdenacao;

                List<Dominio.Entidades.Embarcador.Cargas.Carga> cargas = repCargaCIOT.ConsultarCargasCIOT(codigoCIOT, propOrdenacao, grid.dirOrdena, grid.inicio, grid.limite);
                int totalRegistros = repCargaCIOT.ContarConsultaCargasCIOT(codigoCIOT);

                var retorno = (from obj in cargas
                               select new
                               {
                                   obj.Codigo,
                                   obj.CodigoCargaEmbarcador,
                                   Origem = obj.DadosSumarizados.Origens,
                                   Destino = obj.DadosSumarizados.Destinos,
                                   obj.ValorFrete,
                                   Situacao = obj.DescricaoSituacaoCarga
                               }).ToList();

                grid.AdicionaRows(retorno);
                grid.setarQuantidadeTotal(totalRegistros);

                return new JsonpResult(grid);
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);

                return new JsonpResult(false, false, "Ocorreu uma falha ao consultar os CT-es.");
            }
            finally
            {
                unidadeTrabalho.Dispose();
            }
        }

        public async Task<IActionResult> PesquisaAcrescimoDescontoContratoFrete()
        {
            Repositorio.UnitOfWork unidadeTrabalho = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                int codigoCIOT = Request.GetIntParam("CIOT");

                Models.Grid.Grid grid = new Models.Grid.Grid(Request)
                {
                    header = new List<Models.Grid.Head>()
                };

                grid.AdicionarCabecalho("Codigo", false);
                grid.AdicionarCabecalho("Contrato", "NumeroContrato", 10, Models.Grid.Align.left, false);
                grid.AdicionarCabecalho("Justificativa", "Justificativa", 20, Models.Grid.Align.left, false);
                grid.AdicionarCabecalho("Data", "Data", 10, Models.Grid.Align.center, false);
                grid.AdicionarCabecalho("Situação", "DescricaoSituacao", 10, Models.Grid.Align.center, false);
                grid.AdicionarCabecalho("Valor", "Valor", 10, Models.Grid.Align.right, false);
                grid.AdicionarCabecalho("Observação", "Observacao", 20, Models.Grid.Align.left, false);

                Dominio.ObjetosDeValor.Embarcador.Consulta.ParametroConsulta parametrosConsulta = grid.ObterParametrosConsulta();
                Repositorio.Embarcador.Terceiros.ContratoFreteAcrescimoDesconto repContratoFreteAcrescimoDesconto = new Repositorio.Embarcador.Terceiros.ContratoFreteAcrescimoDesconto(unidadeTrabalho);
                List<Dominio.Entidades.Embarcador.Terceiros.ContratoFreteAcrescimoDesconto> acrescimosDescontos = repContratoFreteAcrescimoDesconto.ConsultarParaCIOT(codigoCIOT, parametrosConsulta);
                int totalRegistros = repContratoFreteAcrescimoDesconto.ContarConsultaParaCIOT(codigoCIOT);

                var retorno = (from obj in acrescimosDescontos
                               select new
                               {
                                   obj.Codigo,
                                   Data = obj.Data.ToDateTimeString(),
                                   obj.ContratoFrete.NumeroContrato,
                                   Justificativa = obj.Justificativa.Descricao,
                                   DescricaoSituacao = obj.Situacao.ObterDescricao(),
                                   Valor = obj.Valor.ToString("n2"),
                                   obj.Observacao
                               }).ToList();

                grid.AdicionaRows(retorno);
                grid.setarQuantidadeTotal(totalRegistros);

                return new JsonpResult(grid);
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);

                return new JsonpResult(false, "Ocorreu uma falha ao consultar os acréscimos/desconto do contrato.");
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

                if (cte == null)
                    return new JsonpResult(true, false, "CT-e não encontrado, atualize a página e tente novamente.");

                if (cte.Status != "A" && cte.Status != "C" && cte.Status != "K")
                    return new JsonpResult(true, false, "O status do CT-e não permite a geração do DACTE.");

                if (string.IsNullOrWhiteSpace(Servicos.Embarcador.Configuracoes.ConfigurationInstance.GetInstance(unidadeTrabalho).ObterConfiguracaoArquivo().CaminhoRelatorios))
                    return new JsonpResult(true, false, "O caminho para o download do DACTE não está disponível. Contate o suporte técnico.");

                string caminhoPDF = Utilidades.IO.FileStorageService.Storage.Combine(Servicos.Embarcador.Configuracoes.ConfigurationInstance.GetInstance(unidadeTrabalho).ObterConfiguracaoArquivo().CaminhoRelatorios, cte.Empresa.CNPJ, cte.Chave) + ".pdf";

                byte[] dacte = null;

                if (!Utilidades.IO.FileStorageService.Storage.Exists(caminhoPDF))
                {
                    if (string.IsNullOrWhiteSpace(Servicos.Embarcador.Configuracoes.ConfigurationInstance.GetInstance(unidadeTrabalho).ObterConfiguracaoArquivo().CaminhoGeradorRelatorios))
                        return new JsonpResult(true, false, "O gerador do DACTE não está disponível. Contate o suporte técnico.");

                    Servicos.DACTE svcDACTE = new Servicos.DACTE(unidadeTrabalho);

                    dacte = svcDACTE.GerarPorProcesso(cte.Codigo, unidadeTrabalho);
                }
                else
                {
                    dacte = Utilidades.IO.FileStorageService.Storage.ReadAllBytes(caminhoPDF);
                }

                if (dacte != null)
                    return Arquivo(dacte, "application/pdf", System.IO.Path.GetFileName(caminhoPDF));
                else
                    return new JsonpResult(false, false, "Não foi possível gerar o DACTE, atualize a página e tente novamente.");
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);

                return new JsonpResult(false, false, "Ocorreu uma falha ao realizar o download do DACTE.");
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
                    return new JsonpResult(false, false, "CT-e não encontrado, atualize a página e tente novamente.");

                Servicos.CTe svcCTe = new Servicos.CTe(unidadeTrabalho);

                byte[] data = svcCTe.ObterXMLAutorizacao(cte);

                if (data != null)
                    return Arquivo(data, "text/xml", string.Concat(cte.Chave, ".xml"));
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

        #endregion
    }
}
