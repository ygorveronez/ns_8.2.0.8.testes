using SGTAdmin.Controllers;
using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Mvc;

namespace SGT.WebAdmin.Controllers.CTe
{
    [CustomAuthorize("CTe/CTeCancelamento", "CTe/ConsultaCTe")]
    public class CTeCancelamentoController : BaseController
    {
		#region Construtores

		public CTeCancelamentoController(Conexao conexao) : base(conexao) { }

		#endregion

        #region Métodos Públicos

        public async Task<IActionResult> Pesquisa()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                Models.Grid.Grid grid = new Models.Grid.Grid(Request);
                grid.header = new List<Models.Grid.Head>();
                grid.AdicionarCabecalho("Codigo", false);
                grid.AdicionarCabecalho("Situacao", false);
                grid.AdicionarCabecalho("Número", "Numero", 8, Models.Grid.Align.center, true);
                grid.AdicionarCabecalho("Serie", "Serie", 5, Models.Grid.Align.center, true);
                grid.AdicionarCabecalho("Emissão", "DataEmissao", 8, Models.Grid.Align.center, true);
                grid.AdicionarCabecalho("Emitente", "Emitente", 12, Models.Grid.Align.left, true);
                grid.AdicionarCabecalho("Remetente", "Remetente", 12, Models.Grid.Align.left, true);
                grid.AdicionarCabecalho("Origem", "Origem", 12, Models.Grid.Align.left, true);
                grid.AdicionarCabecalho("Destinatário", "Destinatario", 12, Models.Grid.Align.left, true);
                grid.AdicionarCabecalho("Destino", "Destino", 12, Models.Grid.Align.left, true);
                grid.AdicionarCabecalho("Valor a Receber", "ValorFrete", 8, Models.Grid.Align.right, true);
                grid.AdicionarCabecalho("Status", "Status", 8, Models.Grid.Align.center, true);
                grid.AdicionarCabecalho("Retorno SEFAZ", "RetornoSEFAZ", 12, Models.Grid.Align.left, false);

                string propOrdenacao = grid.header[grid.indiceColunaOrdena].data;

                if (propOrdenacao == "Remetente" || propOrdenacao == "Destinatario")
                    propOrdenacao += ".Nome";
                else if (propOrdenacao == "Origem")
                    propOrdenacao = "LocalidadeInicioPrestacao.Descricao";
                else if (propOrdenacao == "Destino")
                    propOrdenacao = "LocalidadeTerminoPrestacao.Descricao";

                Repositorio.ConhecimentoDeTransporteEletronico repCTe = new Repositorio.ConhecimentoDeTransporteEletronico(unitOfWork);
                Dominio.ObjetosDeValor.Embarcador.CTe.FiltroPesquisaConsultaCTe filtrosPesquisa = ObterFiltrosPesquisa(unitOfWork);

                List<Dominio.Entidades.ConhecimentoDeTransporteEletronico> ctes = repCTe.Consultar(filtrosPesquisa, propOrdenacao, grid.dirOrdena, grid.inicio, grid.limite);
                int countCTes = repCTe.ContarConsulta(filtrosPesquisa);

                grid.setarQuantidadeTotal(countCTes);

                var lista = (from obj in ctes
                             select new
                             {
                                 obj.Codigo,
                                 Situacao = obj.Status,
                                 obj.Numero,
                                 Serie = obj.Serie.Numero,
                                 DataEmissao = obj.DataEmissao.Value.ToString("dd/MM/yyyy HH:mm"),
                                 Emitente = obj.Empresa.RazaoSocial + "(" + obj.Empresa.CNPJ_Formatado + ")",
                                 Remetente = obj.Remetente.Nome + "(" + obj.Remetente.CPF_CNPJ_Formatado + ")",
                                 Origem = obj.LocalidadeInicioPrestacao.DescricaoCidadeEstado,
                                 Destinatario = obj.Destinatario.Nome + "(" + obj.Destinatario.CPF_CNPJ_Formatado + ")",
                                 Destino = obj.LocalidadeTerminoPrestacao.DescricaoCidadeEstado,
                                 ValorFrete = obj.ValorAReceber.ToString("n2"),
                                 Status = obj.DescricaoStatus,
                                 RetornoSEFAZ = !string.IsNullOrWhiteSpace(obj.MensagemRetornoSefaz) ? obj.MensagemRetornoSefaz != " - " ? System.Web.HttpUtility.HtmlEncode(obj.MensagemRetornoSefaz) : "" : "",
                                 DT_RowColor = filtrosPesquisa.StatusCTe?.Count == 0 ? (obj.Status == "A" ? "#dff0d8" : obj.Status == "R" ? "rgba(193, 101, 101, 1)" : (obj.Status == "C" || obj.Status == "I" || obj.Status == "D" || obj.Status == "Z") ? "#777" : "") : "",
                                 DT_FontColor = filtrosPesquisa.StatusCTe?.Count == 0 ? ((obj.Status == "R" || obj.Status == "C" || obj.Status == "I" || obj.Status == "D" || obj.Status == "Z") ? "#FFFFFF" : "") : "",
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

        public async Task<IActionResult> Cancelar()
        {
            Repositorio.UnitOfWork unidadeTrabalho = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                Repositorio.ConhecimentoDeTransporteEletronico repCTe = new Repositorio.ConhecimentoDeTransporteEletronico(unidadeTrabalho);
                int codigoCTe;
                int.TryParse(Request.Params("Codigo"), out codigoCTe);

                string justificativa = Request.Params("Justificativa");

                string erro = string.Empty;

                unidadeTrabalho.Start();

                Dominio.Entidades.ConhecimentoDeTransporteEletronico cte = repCTe.BuscarPorCodigo(codigoCTe);
                if (!Servicos.Embarcador.CTe.CTe.CancelarOuAnularCTe(out erro, codigoCTe, justificativa, unidadeTrabalho, _conexao.StringConexao, WebServiceConsultaCTe, TipoServicoMultisoftware))
                {
                    unidadeTrabalho.Rollback();
                    return new JsonpResult(false, true, erro);
                }

                Servicos.Auditoria.Auditoria.Auditar(Auditado, cte, null, "Solicitou Cancelamento/Anulação do CT-e", unidadeTrabalho);
                unidadeTrabalho.CommitChanges();

                return new JsonpResult(true);
            }
            catch (Exception ex)
            {
                unidadeTrabalho.Rollback();
                Servicos.Log.TratarErro(ex);
                return new JsonpResult(false, true, "Ocorreu uma falha ao cancelar o documento.");
            }
            finally
            {
                unidadeTrabalho.Dispose();
            }
        }

        #endregion

        #region Métodos Privados

        private Dominio.ObjetosDeValor.Embarcador.CTe.FiltroPesquisaConsultaCTe ObterFiltrosPesquisa(Repositorio.UnitOfWork unitOfWork)
        {
            Dominio.ObjetosDeValor.Embarcador.CTe.FiltroPesquisaConsultaCTe filtrosPesquisaConsultaCTe = new Dominio.ObjetosDeValor.Embarcador.CTe.FiltroPesquisaConsultaCTe()
            {
                DataEmissaoInicial = Request.GetDateTimeParam("DataEmissaoInicial"),
                DataEmissaoFinal = Request.GetDateTimeParam("DataEmissaoFinal"),
                NumeroInicial = Request.GetIntParam("NumeroInicial"),
                NumeroFinal = Request.GetIntParam("NumeroFinal"),
                CodigoOrigem = Request.GetIntParam("Origem"),
                CodigoDestino = Request.GetIntParam("Destino")
            };

            string status = Request.GetStringParam("Status");
            filtrosPesquisaConsultaCTe.StatusCTe = !string.IsNullOrWhiteSpace(status) ? new List<string> { status } : new List<string>();

            Repositorio.Cliente repCliente = new Repositorio.Cliente(unitOfWork);

            double CpfCnpjRemetente = Request.GetDoubleParam("Remetente");
            double CpfCnpjDestinatario = Request.GetDoubleParam("Destinatario");

            if (CpfCnpjRemetente > 0d)
                filtrosPesquisaConsultaCTe.CpfCnpjRemetente = repCliente.BuscarPorCPFCNPJ(CpfCnpjRemetente)?.CPF_CNPJ_SemFormato ?? null;

            if (CpfCnpjDestinatario > 0d)
                filtrosPesquisaConsultaCTe.CpfCnpjDestinatario = repCliente.BuscarPorCPFCNPJ(CpfCnpjDestinatario)?.CPF_CNPJ_SemFormato ?? null;

            return filtrosPesquisaConsultaCTe;
        }

        #endregion
    }
}
