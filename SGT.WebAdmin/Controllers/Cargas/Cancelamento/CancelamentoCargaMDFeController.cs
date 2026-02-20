using Dominio.ObjetosDeValor.Embarcador.Enumeradores;
using SGTAdmin.Controllers;
using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Mvc;

namespace SGT.WebAdmin.Controllers.Cargas.Cancelamento
{
    [CustomAuthorize("Cargas/CancelamentoCarga")]
    public class CancelamentoCargaMDFeController : BaseController
    {
		#region Construtores

		public CancelamentoCargaMDFeController(Conexao conexao) : base(conexao) { }

		#endregion

        #region Métodos Globais

        public async Task<IActionResult> Pesquisa()
        {
            Repositorio.UnitOfWork unidadeTrabalho = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                int codigoCarga = Request.GetIntParam("Carga");
                int codigoCancelamentoCarga = Request.GetIntParam("CancelamentoCarga");

                Models.Grid.Grid grid = new Models.Grid.Grid(Request);
                grid.header = new List<Models.Grid.Head>();
                grid.AdicionarCabecalho("Codigo", false);
                grid.AdicionarCabecalho("Status", false);
                grid.AdicionarCabecalho(Localization.Resources.Gerais.Geral.Numero, "Numero", 10, Models.Grid.Align.left, true);
                grid.AdicionarCabecalho(Localization.Resources.Gerais.Geral.Serie, "Serie", 9, Models.Grid.Align.left, true);
                grid.AdicionarCabecalho(Localization.Resources.Cargas.CancelamentoCarga.Emissao, "Emissao", 10, Models.Grid.Align.left, true);
                grid.AdicionarCabecalho(Localization.Resources.Cargas.CancelamentoCarga.UFCarga, "UFCarga", 15, Models.Grid.Align.left, true);
                grid.AdicionarCabecalho(Localization.Resources.Cargas.CancelamentoCarga.UFDescarga, "UFDescarga", 15, Models.Grid.Align.left, true);
                grid.AdicionarCabecalho(Localization.Resources.Gerais.Geral.Situacao, "DescricaoStatus", 12, Models.Grid.Align.left, true);
                grid.AdicionarCabecalho(Localization.Resources.Cargas.CancelamentoCarga.RetornoSEFAZ, "RetornoSEFAZ", 22, Models.Grid.Align.left, false);

                string propOrdenar = grid.header[grid.indiceColunaOrdena].data;

                if (propOrdenar == "Numero")
                    propOrdenar = "MDFe.Numero";
                else if (propOrdenar == "Serie")
                    propOrdenar = "MDFe.Serie.Numero";
                else if (propOrdenar == "Emissao")
                    propOrdenar = "MDFe.DataEmissao";
                else if (propOrdenar == "UFCarga")
                    propOrdenar = "MDFe.EstadoCarregamento.Sigla";
                else if (propOrdenar == "UFDescarga")
                    propOrdenar = "MDFe.EstadoDescarregamento.Sigla";
                else if (propOrdenar == "DescricaoStatus")
                    propOrdenar = "MDFe.Status";

                Repositorio.Embarcador.Cargas.CargaMDFe repCargaMDFe = new Repositorio.Embarcador.Cargas.CargaMDFe(unidadeTrabalho);

                List<Dominio.Entidades.Embarcador.Cargas.CargaMDFe> mdfes = repCargaMDFe.Consultar(codigoCarga, codigoCancelamentoCarga, propOrdenar, grid.dirOrdena, grid.inicio, grid.limite);
                int countMDFes = repCargaMDFe.ContarConsulta(codigoCarga, codigoCancelamentoCarga);

                grid.setarQuantidadeTotal(countMDFes);

                var retorno = (from obj in mdfes
                               select new
                               {
                                   obj.MDFe.Codigo,
                                   obj.MDFe.Status,
                                   obj.MDFe.Numero,
                                   Serie = obj.MDFe.Serie.Numero,
                                   Emissao = obj.MDFe.DataEmissao.Value.ToString("dd/MM/yyyy HH:mm"),
                                   UFCarga = obj.MDFe.EstadoCarregamento.Sigla + " - " + obj.MDFe.EstadoCarregamento.Nome,
                                   UFDescarga = obj.MDFe.EstadoDescarregamento.Sigla + " - " + obj.MDFe.EstadoDescarregamento.Nome,
                                   DescricaoStatus = obj.MDFe.DescricaoStatus,
                                   RetornoSEFAZ = obj.MDFe.MensagemStatus != null ? obj.MDFe.MensagemStatus.MensagemDoErro : System.Web.HttpUtility.HtmlEncode(obj.MDFe.MensagemRetornoSefaz)
                               }).ToList();

                grid.AdicionaRows(retorno);

                return new JsonpResult(grid);
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);

                return new JsonpResult(false, false, Localization.Resources.Cargas.CancelamentoCarga.OcorreuFalhaAoConsultarMDFes);
            }
            finally
            {
                unidadeTrabalho.Dispose();
            }
        }

        [AcceptVerbs("POST", "GET")]
        public async Task<IActionResult> DownloadDAMDFE()
        {
            Repositorio.UnitOfWork unidadeTrabalho = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                int codigoMDFe = 0;
                int.TryParse(Request.Params("MDFe"), out codigoMDFe);

                Repositorio.ManifestoEletronicoDeDocumentosFiscais repMDFe = new Repositorio.ManifestoEletronicoDeDocumentosFiscais(unidadeTrabalho);
                Dominio.Entidades.ManifestoEletronicoDeDocumentosFiscais mdfe = repMDFe.BuscarPorCodigo(codigoMDFe);

                if (mdfe == null)
                    return new JsonpResult(false, false, Localization.Resources.Cargas.CancelamentoCarga.MDFeNaoEncontrado);

                if (mdfe.Status != Dominio.Enumeradores.StatusMDFe.Autorizado &&
                    mdfe.Status != Dominio.Enumeradores.StatusMDFe.Cancelado &&
                    mdfe.Status != Dominio.Enumeradores.StatusMDFe.EmCancelamento &&
                    mdfe.Status != Dominio.Enumeradores.StatusMDFe.EmEncerramento &&
                    mdfe.Status != Dominio.Enumeradores.StatusMDFe.Encerrado)
                    return new JsonpResult(false, false, Localization.Resources.Cargas.CancelamentoCarga.MDFEAutorizadoDownloadDAMDFE);

                Servicos.DAMDFE svcDAMDFE = new Servicos.DAMDFE(unidadeTrabalho);

                byte[] arquivo = null;

                if (!string.IsNullOrWhiteSpace(Servicos.Embarcador.Configuracoes.ConfigurationInstance.GetInstance(unidadeTrabalho).ObterConfiguracaoArquivo().CaminhoRelatorios))
                {
                    string caminhoPDF = Utilidades.IO.FileStorageService.Storage.Combine(Servicos.Embarcador.Configuracoes.ConfigurationInstance.GetInstance(unidadeTrabalho).ObterConfiguracaoArquivo().CaminhoRelatorios, mdfe.Empresa.CNPJ, mdfe.Chave) + ".pdf";
                    if (Utilidades.IO.FileStorageService.Storage.Exists(caminhoPDF))
                        arquivo = Utilidades.IO.FileStorageService.Storage.ReadAllBytes(caminhoPDF);
                }

                if (arquivo == null)
                    arquivo = svcDAMDFE.Gerar(mdfe.Codigo);

                if (arquivo != null)
                    return Arquivo(arquivo, "application/pdf", string.Concat(mdfe.Chave, ".pdf"));
                else
                    return new JsonpResult(false, false, Localization.Resources.Cargas.CancelamentoCarga.NaoFoiPossivelGerarDAMDFE);

            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);

                return new JsonpResult(false, false, Localization.Resources.Cargas.CancelamentoCarga.OcorreuFalhaRealizarDownloadDAMDFE);
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
                int codigoMDFe = 0;
                int.TryParse(Request.Params("MDFe"), out codigoMDFe);

                Repositorio.ManifestoEletronicoDeDocumentosFiscais repMDFe = new Repositorio.ManifestoEletronicoDeDocumentosFiscais(unidadeTrabalho);
                Dominio.Entidades.ManifestoEletronicoDeDocumentosFiscais mdfe = repMDFe.BuscarPorCodigo(codigoMDFe);

                if (mdfe == null)
                    return new JsonpResult(false, false, Localization.Resources.Cargas.CancelamentoCarga.MDFeNaoEncontrado);

                if (mdfe.Status != Dominio.Enumeradores.StatusMDFe.Autorizado &&
                    mdfe.Status != Dominio.Enumeradores.StatusMDFe.Cancelado &&
                    mdfe.Status != Dominio.Enumeradores.StatusMDFe.EmCancelamento &&
                    mdfe.Status != Dominio.Enumeradores.StatusMDFe.EmEncerramento &&
                    mdfe.Status != Dominio.Enumeradores.StatusMDFe.Encerrado)
                    return new JsonpResult(false, false, Localization.Resources.Cargas.CancelamentoCarga.MDFeDeveEstarAutorizado);

                Servicos.MDFe svcMDFe = new Servicos.MDFe();

                byte[] arquivo = svcMDFe.ObterXMLAutorizacao(mdfe, unidadeTrabalho);

                if (arquivo != null)
                    return Arquivo(arquivo, "text/xml", string.Concat(mdfe.Chave, ".xml"));
                else
                    return new JsonpResult(false, false, Localization.Resources.Cargas.CancelamentoCarga.XMLNaoEncontrado);
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);

                return new JsonpResult(false, false, Localization.Resources.Cargas.CancelamentoCarga.OcorreuFalhaRealizarDownloadXML);
            }
            finally
            {
                unidadeTrabalho.Dispose();
            }
        }

        public async Task<IActionResult> LiberarAverbacaoRejeitada()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                List<AdminMultisoftware.Dominio.Enumeradores.PermissaoPersonalizada> permissoesPersonalizadas = ObterPermissoesPersonalizadas("Cargas/CancelamentoCarga");

                if (!permissoesPersonalizadas.Contains(AdminMultisoftware.Dominio.Enumeradores.PermissaoPersonalizada.CargaCancelamento_LiberarAverbacaoRejeitada))
                    return new JsonpResult(false, true, Localization.Resources.Gerais.Geral.VoceNaoPossuiPermissaoParaExecutarEstaAcao);

                Repositorio.Embarcador.Cargas.CargaCancelamento repCargaCancelamento = new Repositorio.Embarcador.Cargas.CargaCancelamento(unitOfWork);

                int codigoCargaCancelamento = Request.GetIntParam("Codigo");

                Dominio.Entidades.Embarcador.Cargas.CargaCancelamento cargaCancelamento = repCargaCancelamento.BuscarPorCodigo(codigoCargaCancelamento);

                if (cargaCancelamento == null)
                    return new JsonpResult(false, true, Localization.Resources.Cargas.CancelamentoCarga.CancelamentoNaoEncontrados);

                if (cargaCancelamento.Situacao != Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoCancelamentoCarga.RejeicaoCancelamento)
                    return new JsonpResult(false, true, Localization.Resources.Cargas.CancelamentoCarga.SituacaoCancelamento + cargaCancelamento.Situacao.Descricao() + Localization.Resources.Cargas.CancelamentoCarga.NaoPermiteLiberacao);

                unitOfWork.Start();

                cargaCancelamento.Situacao = SituacaoCancelamentoCarga.EmCancelamento;
                cargaCancelamento.LiberarCancelamentoComAverbacaoMDFeRejeitada = true;

                repCargaCancelamento.Atualizar(cargaCancelamento);

                Servicos.Auditoria.Auditoria.Auditar(Auditado, cargaCancelamento, null, Localization.Resources.Cargas.CancelamentoCarga.LiberouCancelamentoDaCarga, unitOfWork);

                unitOfWork.CommitChanges();

                return new JsonpResult(true);
            }
            catch (Exception ex)
            {
                unitOfWork.Rollback();
                Servicos.Log.TratarErro(ex);
                return new JsonpResult(false, true, Localization.Resources.Cargas.CancelamentoCarga.OcorreuUmaFalhaAoLiberarCancelamento);
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        #endregion
    }
}
