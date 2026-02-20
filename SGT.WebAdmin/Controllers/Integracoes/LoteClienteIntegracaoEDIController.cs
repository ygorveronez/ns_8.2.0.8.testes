using Dominio.ObjetosDeValor.Embarcador.Enumeradores;
using SGTAdmin.Controllers;
using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Mvc;

namespace SGT.WebAdmin.Controllers.Integracoes
{
    [CustomAuthorize(new string[] { "ObterTotais", "Download" }, "Integracoes/LoteCliente")]
    public class LoteClienteIntegracaoEDIController : BaseController
    {
		#region Construtores

		public LoteClienteIntegracaoEDIController(Conexao conexao) : base(conexao) { }

		#endregion

        #region Métodos Globais

        public async Task<IActionResult> Pesquisa()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                SituacaoIntegracao? situacao = Request.GetNullableEnumParam<SituacaoIntegracao>("Situacao");

                int codigoLoteCliente = Request.GetIntParam("Codigo");

                Models.Grid.Grid grid = new Models.Grid.Grid(Request);

                grid.header = new List<Models.Grid.Head>();
                grid.AdicionarCabecalho("Codigo", false);
                grid.AdicionarCabecalho("Tipo", false);
                grid.AdicionarCabecalho("SituacaoIntegracao", false);
                grid.AdicionarCabecalho("Layout", "Layout", 15, Models.Grid.Align.left, true);
                grid.AdicionarCabecalho("Integração", "TipoIntegracao", 10, Models.Grid.Align.left, true);
                grid.AdicionarCabecalho("Tentativas", "Tentativas", 10, Models.Grid.Align.left, true);
                grid.AdicionarCabecalho("Data do Envio", "DataEnvio", 10, Models.Grid.Align.left, true);
                grid.AdicionarCabecalho("Situação", "Situacao", 10, Models.Grid.Align.left, true);
                grid.AdicionarCabecalho("Retorno", "Retorno", 25, Models.Grid.Align.left, false);

                string propOrdena = grid.header[grid.indiceColunaOrdena].data;

                Repositorio.Embarcador.Integracao.LoteClienteIntegracaoEDI repIntegracao = new Repositorio.Embarcador.Integracao.LoteClienteIntegracaoEDI(unitOfWork);

                int countEDIs = codigoLoteCliente > 0 ? repIntegracao.ContarConsulta(codigoLoteCliente, situacao) : 0;

                List<Dominio.Entidades.Embarcador.Integracao.LoteCliente.LoteClienteIntegracaoEDI> listaIntegracao = new List<Dominio.Entidades.Embarcador.Integracao.LoteCliente.LoteClienteIntegracaoEDI>();

                if (countEDIs > 0)
                {
                    if (propOrdena == "Layout")
                        propOrdena = "LayoutEDI.Descricao";
                    else if (propOrdena == "TipoIntegracao")
                        propOrdena = "TipoIntegracao.Tipo";
                    else if (propOrdena == "Tentativas")
                        propOrdena = "NumeroTentativas";
                    else if (propOrdena == "DataEnvio")
                        propOrdena = "DataIntegracao";
                    else if (propOrdena == "Situacao")
                        propOrdena = "SituacaoIntegracao";

                    listaIntegracao = repIntegracao.Consultar(codigoLoteCliente, situacao, propOrdena, grid.dirOrdena, grid.inicio, grid.limite);
                }

                grid.AdicionaRows((from obj in listaIntegracao
                                   select new
                                   {
                                       obj.Codigo,
                                       obj.TipoIntegracao.Tipo,
                                       Layout = obj.LayoutEDI.Descricao,
                                       obj.SituacaoIntegracao,
                                       Situacao = obj.DescricaoSituacaoIntegracao,
                                       TipoIntegracao = obj.TipoIntegracao.DescricaoTipo,
                                       Retorno = obj.ProblemaIntegracao,
                                       Tentativas = obj.NumeroTentativas,
                                       DataEnvio = obj.DataIntegracao.ToString("dd/MM/yyyy HH:mm:ss"),
                                       DT_RowColor = obj.SituacaoIntegracao == SituacaoIntegracao.Integrado ? CorGrid.Verde : obj.SituacaoIntegracao == SituacaoIntegracao.ProblemaIntegracao ? CorGrid.Vermelho : CorGrid.Azul,
                                       DT_FontColor = obj.SituacaoIntegracao == SituacaoIntegracao.ProblemaIntegracao ? CorGrid.Branco : "",
                                   }).ToList());

                grid.setarQuantidadeTotal(countEDIs);

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

        public async Task<IActionResult> ObterTotais()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                int codigoLoteCliente = Request.GetIntParam("Codigo");

                Repositorio.Embarcador.Integracao.LoteClienteIntegracaoEDI repLoteLoteClienteIntegracaoEDI = new Repositorio.Embarcador.Integracao.LoteClienteIntegracaoEDI(unitOfWork);

                int totalAguardandoIntegracao = repLoteLoteClienteIntegracaoEDI.ContarPorLoteCliente(codigoLoteCliente, SituacaoIntegracao.AgIntegracao);
                int totalAguardandoRetorno = repLoteLoteClienteIntegracaoEDI.ContarPorLoteCliente(codigoLoteCliente, SituacaoIntegracao.AgRetorno);
                int totalIntegrado = repLoteLoteClienteIntegracaoEDI.ContarPorLoteCliente(codigoLoteCliente, SituacaoIntegracao.Integrado);
                int totalProblemaIntegracao = repLoteLoteClienteIntegracaoEDI.ContarPorLoteCliente(codigoLoteCliente, SituacaoIntegracao.ProblemaIntegracao);

                var retorno = new
                {
                    TotalAguardandoIntegracao = totalAguardandoIntegracao,
                    TotalAguardandoRetorno = totalAguardandoRetorno,
                    TotalIntegrado = totalIntegrado,
                    TotalProblemaIntegracao = totalProblemaIntegracao,
                    TotalGeral = totalAguardandoIntegracao + totalIntegrado + totalProblemaIntegracao
                };

                return new JsonpResult(retorno);
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);

                return new JsonpResult(false, "Ocorreu uma falha ao obter os totais das integrações de EDI.");
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        public async Task<IActionResult> Reenviar()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                int codigo = Request.GetIntParam("Codigo");

                Repositorio.Embarcador.Integracao.LoteClienteIntegracaoEDI repLoteClienteIntegracaoEDI = new Repositorio.Embarcador.Integracao.LoteClienteIntegracaoEDI(unitOfWork);

                Dominio.Entidades.Embarcador.Integracao.LoteCliente.LoteClienteIntegracaoEDI loteClienteIntegracaoEDI = repLoteClienteIntegracaoEDI.BuscarPorCodigo(codigo, false);

                if (loteClienteIntegracaoEDI == null)
                    return new JsonpResult(false, "Integração não encontrada.");

                loteClienteIntegracaoEDI.IniciouConexaoExterna = false;
                loteClienteIntegracaoEDI.SituacaoIntegracao = SituacaoIntegracao.AgIntegracao;

                Servicos.Auditoria.Auditoria.Auditar(Auditado, loteClienteIntegracaoEDI, null, "Reenviou a Integração.", unitOfWork);

                repLoteClienteIntegracaoEDI.Atualizar(loteClienteIntegracaoEDI);

                return new JsonpResult(true);
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);

                return new JsonpResult(false, "Ocorreu uma falha ao enviar a integração.");
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        public async Task<IActionResult> Download()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                int codigo = Request.GetIntParam("Codigo");

                Repositorio.Embarcador.Integracao.LoteClienteIntegracaoEDI repLoteClienteIntegracaoEDI = new Repositorio.Embarcador.Integracao.LoteClienteIntegracaoEDI(unitOfWork);

                Dominio.Entidades.Embarcador.Integracao.LoteCliente.LoteClienteIntegracaoEDI loteClienteIntegracaoEDI = repLoteClienteIntegracaoEDI.BuscarPorCodigo(codigo, false);

                if (loteClienteIntegracaoEDI == null)
                    return new JsonpResult(false, "Integração não encontrada.");

                string extensao = string.Empty;

                System.IO.MemoryStream edi = Servicos.Embarcador.Integracao.IntegracaoEDI.GerarEDI(loteClienteIntegracaoEDI, unitOfWork, out extensao);
                string nomeArquivo = Servicos.Embarcador.Integracao.IntegracaoEDI.ObterNomeArquivoEDI(loteClienteIntegracaoEDI, extensao, unitOfWork);

                if (loteClienteIntegracaoEDI.LayoutEDI.Tipo == Dominio.Enumeradores.TipoLayoutEDI.Cliente &&
                    loteClienteIntegracaoEDI.TipoIntegracao.Tipo == TipoIntegracao.NaoPossuiIntegracao &&
                    loteClienteIntegracaoEDI.SituacaoIntegracao == SituacaoIntegracao.AgIntegracao)
                {
                    loteClienteIntegracaoEDI.SituacaoIntegracao = SituacaoIntegracao.Integrado;

                    repLoteClienteIntegracaoEDI.Atualizar(loteClienteIntegracaoEDI);
                }

                return Arquivo(edi, extensao == ".zip" ? "application/zip" : "plain/text", nomeArquivo);
            }
            catch (Exception ex)
            {
                unitOfWork.Rollback();

                Servicos.Log.TratarErro(ex);

                return new JsonpResult(false, "Ocorreu uma falha ao enviar a integração.");
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        #endregion
    }
}
