using SGTAdmin.Controllers;
using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Mvc;
using Dominio.Excecoes.Embarcador;

namespace SGT.WebAdmin.Controllers.Cargas.CTeAgrupado
{
    [CustomAuthorize("Cargas/CargaCTeAgrupado")]
    public class CargaCTeAgrupadoDocumentoController : BaseController
    {
		#region Construtores

		public CargaCTeAgrupadoDocumentoController(Conexao conexao) : base(conexao) { }

		#endregion

        #region Métodos Globais

        public async Task<IActionResult> Pesquisa()
        {
            try
            {
                return new JsonpResult(ObterGridPesquisa());
            }
            catch (Exception excecao)
            {
                Servicos.Log.TratarErro(excecao);

                return new JsonpResult(false, "Ocorreu uma falha ao consultar.");
            }
        }

        public async Task<IActionResult> EmitirCTeRejeitado()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                int codigoCargaCTeAgrupadoCTe = Request.GetIntParam("Codigo");

                dynamic dynCTe = Newtonsoft.Json.JsonConvert.DeserializeObject<dynamic>((string)Request.Params("CTe"));

                Servicos.Embarcador.CTe.CTe serCTe = new Servicos.Embarcador.CTe.CTe(unitOfWork);

                Repositorio.Embarcador.Cargas.CTeAgrupado.CargaCTeAgrupadoCTe repCargaCTeAgrupadoCTe = new Repositorio.Embarcador.Cargas.CTeAgrupado.CargaCTeAgrupadoCTe(unitOfWork);
                Repositorio.Embarcador.Cargas.CTeAgrupado.CargaCTeAgrupado repCargaCTeAgrupado = new Repositorio.Embarcador.Cargas.CTeAgrupado.CargaCTeAgrupado(unitOfWork);
                Repositorio.ConhecimentoDeTransporteEletronico repCTe = new Repositorio.ConhecimentoDeTransporteEletronico(unitOfWork);

                Dominio.Entidades.Embarcador.Cargas.CTeAgrupado.CargaCTeAgrupadoCTe cargaCTeAgrupadoCTe = repCargaCTeAgrupadoCTe.BuscarPorCodigo(codigoCargaCTeAgrupadoCTe, false);

                Dominio.Entidades.ConhecimentoDeTransporteEletronico cte = cargaCTeAgrupadoCTe?.CTe;

                if (cte == null)
                    return new JsonpResult(false, true, "CT-e não encontrado.");

                if (cte.SituacaoCTeSefaz != Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoCTeSefaz.Rejeitada && cte.SituacaoCTeSefaz != Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoCTeSefaz.ContingenciaFSDA)
                    return new JsonpResult(false, true, $"A situação do CT-e {cte.DescricaoStatus} não permite que o mesmo seja emitido.");

                List<Dominio.ObjetosDeValor.Embarcador.Enumeradores.PermissoesEdicaoCTe> permissoes = BuscarPermissoesEmissaoCTeRejeitado();

                cte.Initialize();

                unitOfWork.Start();

                if (cargaCTeAgrupadoCTe.CargaCTeAgrupado.Situacao == Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoCargaCTeAgrupado.Rejeitado)
                {
                    cargaCTeAgrupadoCTe.CargaCTeAgrupado.Situacao = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoCargaCTeAgrupado.EmEmissao;

                    repCargaCTeAgrupado.Atualizar(cargaCTeAgrupadoCTe.CargaCTeAgrupado);
                }

                Dominio.ObjetosDeValor.Embarcador.CTe.CTe cteIntegracao = serCTe.ConverterDynamicParaCTe(dynCTe, unitOfWork);

                serCTe.SalvarDadosCTe(ref cte, cteIntegracao, cte.SituacaoCTeSefaz, permissoes, this.Usuario, unitOfWork, false, Auditado);

                var alteracoesCTe = cte.GetChanges();

                Servicos.Auditoria.Auditoria.Auditar(Auditado, cargaCTeAgrupadoCTe, alteracoesCTe, "Emitiu o documento rejeitado.", unitOfWork);
                Servicos.Auditoria.Auditoria.Auditar(Auditado, cte, alteracoesCTe, "Emitiu o documento rejeitado.", unitOfWork);

                unitOfWork.CommitChanges();

                string retorno = "";
                if (cte.ModeloDocumentoFiscal.TipoDocumentoEmissao == Dominio.Enumeradores.TipoDocumento.CTe)
                {
                    Servicos.CTe svcCTe = new Servicos.CTe(unitOfWork);

                    if (cte.DataEmissao.HasValue && cte.DataEmissao.Value < DateTime.Now.AddDays(-6))
                    {
                        TimeZoneInfo fusoHorarioEmpresa = TimeZoneInfo.FindSystemTimeZoneById(cte.Empresa.FusoHorario);
                        cte.DataEmissao = TimeZoneInfo.ConvertTime(DateTime.Now, TimeZoneInfo.Local, fusoHorarioEmpresa);
                    }

                    cte.Status = "P";

                    repCTe.Atualizar(cte);

                    if (!svcCTe.Emitir(cte.Codigo, cte.Empresa.Codigo, unitOfWork, "E", WebServiceOracle))
                        retorno = "O CT-e nº " + cte.Numero.ToString() + " da empresa " + cte.Empresa.CNPJ + " foi salvo, porém, ocorreu uma falha ao emiti-lo.";
                }
                else if (cte.ModeloDocumentoFiscal.TipoDocumentoEmissao == Dominio.Enumeradores.TipoDocumento.NFSe)
                {
                    Servicos.NFSe svcNFSe = new Servicos.NFSe();

                    cte.Status = "P";

                    repCTe.Atualizar(cte);

                    if (!svcNFSe.EmitirNFSe(cte.Codigo, unitOfWork))
                        retorno = "A NFS-e nº " + cte.Numero.ToString() + " da empresa " + cte.Empresa.CNPJ + " foi salva, porém, ocorreu uma falha ao emiti-la.";
                }

                if (string.IsNullOrWhiteSpace(retorno))
                    return new JsonpResult(true);
                else
                    return new JsonpResult(false, true, retorno);
            }
            catch (ServicoException ex)
            {
                unitOfWork.Rollback();
                return new JsonpResult(false, true, ex.Message);
            }
            catch (Exception ex)
            {
                unitOfWork.Rollback();
                Servicos.Log.TratarErro(ex);
                return new JsonpResult(false, "Ocorreu uma falha ao tentar emitir o CT-e.");
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        #endregion

        #region Métodos Privados

        private List<Dominio.ObjetosDeValor.Embarcador.Enumeradores.PermissoesEdicaoCTe> BuscarPermissoesEmissaoCTeRejeitado()
        {
            List<Dominio.ObjetosDeValor.Embarcador.Enumeradores.PermissoesEdicaoCTe> permissoes = new List<Dominio.ObjetosDeValor.Embarcador.Enumeradores.PermissoesEdicaoCTe>();

            permissoes.Add(Dominio.ObjetosDeValor.Embarcador.Enumeradores.PermissoesEdicaoCTe.AlterarIEParticipanete);

            return permissoes;
        }

        private Models.Grid.Grid ObterGridPesquisa()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                Repositorio.Embarcador.Cargas.CTeAgrupado.CargaCTeAgrupadoCTe repCargaCTeAgrupadoCTe = new Repositorio.Embarcador.Cargas.CTeAgrupado.CargaCTeAgrupadoCTe(unitOfWork);

                int codigoCargaCTeAgrupado = Request.GetIntParam("Codigo");

                Models.Grid.Grid grid = new Models.Grid.Grid(Request);
                grid.header = new List<Models.Grid.Head>();
                grid.AdicionarCabecalho("Codigo", false);
                grid.AdicionarCabecalho("Situacao", false);
                grid.AdicionarCabecalho("CodigoCTe", false);
                grid.AdicionarCabecalho("CodigoEmpresa", false);
                grid.AdicionarCabecalho("TipoDocumentoEmissao", false);
                grid.AdicionarCabecalho("Número", "Numero", 8, Models.Grid.Align.center, true);
                grid.AdicionarCabecalho("Serie", "Serie", 5, Models.Grid.Align.center, true);
                grid.AdicionarCabecalho("Doc.", "AbreviacaoModeloDocumentoFiscal", 5, Models.Grid.Align.center, true);
                grid.AdicionarCabecalho("Emissão", "DataEmissao", 8, Models.Grid.Align.left, true);
                grid.AdicionarCabecalho("Remetente", "Remetente", 10, Models.Grid.Align.left, true);
                grid.AdicionarCabecalho("Destinatário", "Destinatario", 15, Models.Grid.Align.left, true);
                grid.AdicionarCabecalho("Destino", "Destino", 8, Models.Grid.Align.left, true);
                grid.AdicionarCabecalho("CST", "CST", 5, Models.Grid.Align.left, true, false, false, false, true);
                grid.AdicionarCabecalho("Alíquota", "Aliquota", 5, Models.Grid.Align.right, true, false, false, false, true);
                grid.AdicionarCabecalho("Valor a Receber", "ValorFrete", 8, Models.Grid.Align.right, true, false, false, false, true);
                grid.AdicionarCabecalho("Situação", "DescricaoSituacao", 8, Models.Grid.Align.left, true);
                grid.AdicionarCabecalho("Retorno Sefaz", "RetornoSefaz", 13, Models.Grid.Align.left, false);

                string propOrdenacao = grid.header[grid.indiceColunaOrdena].data;

                SetarPropriedadeOrdenar(ref propOrdenacao);

                dynamic listaCTes = new List<object>();
                int countCTes = repCargaCTeAgrupadoCTe.ContarConsultaPorCargaCTeAgrupado(codigoCargaCTeAgrupado);

                if (countCTes > 0)
                {
                    listaCTes = repCargaCTeAgrupadoCTe.ConsultarPorCargaCTeAgrupado(codigoCargaCTeAgrupado, propOrdenacao, grid.dirOrdena, grid.inicio, grid.limite).Select(obj => new
                    {
                        obj.Codigo,
                        CodigoEmpresa = obj.CTe.Empresa.Codigo,
                        CodigoCTe = obj.CTe.Codigo,
                        obj.CTe.ModeloDocumentoFiscal.TipoDocumentoEmissao,
                        AbreviacaoModeloDocumentoFiscal = obj.CTe.ModeloDocumentoFiscal.Abreviacao,
                        obj.CTe.Numero,
                        obj.CTe.DataEmissao,
                        Situacao = obj.CTe.Status,
                        Serie = obj.CTe.Serie.Numero,
                        Remetente = obj.CTe.Remetente != null ? obj.CTe.Remetente.Nome + (!obj.CTe.Remetente.Exterior ? " (" + obj.CTe.Remetente.CPF_CNPJ_Formatado + ")" : string.Empty) : string.Empty,
                        Destinatario = obj.CTe.Destinatario != null ? obj.CTe.Destinatario.Cliente?.Descricao ?? obj.CTe.Destinatario.Nome : string.Empty,
                        Destino = obj.CTe.LocalidadeTerminoPrestacao.DescricaoCidadeEstado,
                        ValorFrete = obj.CTe.ValorAReceber.ToString("n2"),
                        obj.CTe.CST,
                        Aliquota = obj.CTe.AliquotaICMS.ToString("n2"),
                        DescricaoSituacao = obj.CTe.DescricaoStatus,
                        RetornoSefaz = obj.CTe.MensagemStatus != null ? obj.CTe.MensagemStatus.MensagemDoErro : !string.IsNullOrWhiteSpace(obj.CTe.MensagemRetornoSefaz) ? obj.CTe.MensagemRetornoSefaz != " - " ? System.Web.HttpUtility.HtmlEncode(obj.CTe.MensagemRetornoSefaz) : "" : "",
                        DT_RowColor = obj.CTe.PossuiCTeComplementar ? "#ddd8f0" : obj.CTe.PossuiAnulacaoSubstituicao ? "#f0e9d8" : obj.CTe.PossuiCartaCorrecao ? "#d8e4f0" : obj.CTe.Status == "A" ? "#dff0d8" : obj.CTe.Status == "R" ? "rgba(193, 101, 101, 1)" : (obj.CTe.Status == "C" || obj.CTe.Status == "I" || obj.CTe.Status == "D") ? "#777" : "",
                        DT_FontColor = (obj.CTe.Status == "R" || obj.CTe.Status == "C" || obj.CTe.Status == "I" || obj.CTe.Status == "D") ? "#FFFFFF" : "",
                    }).ToList();
                }

                grid.setarQuantidadeTotal(countCTes);
                grid.AdicionaRows(listaCTes);

                return grid;
            }
            catch
            {
                throw;
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        private void SetarPropriedadeOrdenar(ref string propOrdenacao)
        {
            if (propOrdenacao == "Remetente" || propOrdenacao == "Destinatario")
                propOrdenacao += ".Nome";
            else if (propOrdenacao == "Destino")
                propOrdenacao = "LocalidadeTerminoPrestacao.Descricao";
            else if (propOrdenacao == "AbreviacaoModeloDocumentoFiscal")
                propOrdenacao = "ModeloDocumentoFiscal.Abreviacao";

            propOrdenacao = "CTe." + propOrdenacao;
        }

        #endregion
    }
}
