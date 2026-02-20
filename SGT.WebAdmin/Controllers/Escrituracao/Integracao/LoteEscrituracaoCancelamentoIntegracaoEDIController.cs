using SGTAdmin.Controllers;
using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Mvc;

namespace SGT.WebAdmin.Controllers.Escrituracao.Integracao
{
    [CustomAuthorize(new string[] { "ObterTotais", "Download" }, "Escrituracao/LoteEscrituracaoCancelamento")]
    public class LoteEscrituracaoCancelamentoIntegracaoEDIController : BaseController
    {
		#region Construtores

		public LoteEscrituracaoCancelamentoIntegracaoEDIController(Conexao conexao) : base(conexao) { }

		#endregion

        public async Task<IActionResult> Pesquisa()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoIntegracao? situacao = Request.GetNullableEnumParam<Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoIntegracao>("Situacao");

                int codigoLoteEscrituracaoCancelamento = Request.GetIntParam("LoteEscrituracaoCancelamento");

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

                Repositorio.Embarcador.Escrituracao.LoteEscrituracaoCancelamentoEDIIntegracao repLoteEscrituracaoCancelamentoEDIIntegracao = new Repositorio.Embarcador.Escrituracao.LoteEscrituracaoCancelamentoEDIIntegracao(unitOfWork);

                int countEDIs = repLoteEscrituracaoCancelamentoEDIIntegracao.ContarConsulta(codigoLoteEscrituracaoCancelamento, situacao);

                List<Dominio.Entidades.Embarcador.Escrituracao.LoteEscrituracaoCancelamentoEDIIntegracao> listaIntegracao = new List<Dominio.Entidades.Embarcador.Escrituracao.LoteEscrituracaoCancelamentoEDIIntegracao>();

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

                    listaIntegracao = repLoteEscrituracaoCancelamentoEDIIntegracao.Consultar(codigoLoteEscrituracaoCancelamento, situacao, propOrdena, grid.dirOrdena, grid.inicio, grid.limite);
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
                                       DT_RowColor = obj.SituacaoIntegracao == Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoIntegracao.Integrado ? Dominio.ObjetosDeValor.Embarcador.Enumeradores.CorGrid.Verde :
                                                     obj.SituacaoIntegracao == Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoIntegracao.ProblemaIntegracao ? Dominio.ObjetosDeValor.Embarcador.Enumeradores.CorGrid.Vermelho :
                                                     Dominio.ObjetosDeValor.Embarcador.Enumeradores.CorGrid.Azul,
                                       DT_FontColor = obj.SituacaoIntegracao == Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoIntegracao.ProblemaIntegracao ? Dominio.ObjetosDeValor.Embarcador.Enumeradores.CorGrid.Branco : "",
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
                int codigoLoteEscrituracaoCancelamento = Request.GetIntParam("LoteEscrituracaoCancelamento");

                Repositorio.Embarcador.Escrituracao.LoteEscrituracaoCancelamentoEDIIntegracao repLoteEscrituracaoCancelamentoEDIIntegracao = new Repositorio.Embarcador.Escrituracao.LoteEscrituracaoCancelamentoEDIIntegracao(unitOfWork);

                int totalAguardandoIntegracao = repLoteEscrituracaoCancelamentoEDIIntegracao.ContarPorLoteEscrituracao(codigoLoteEscrituracaoCancelamento, Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoIntegracao.AgIntegracao);
                int totalIntegrado = repLoteEscrituracaoCancelamentoEDIIntegracao.ContarPorLoteEscrituracao(codigoLoteEscrituracaoCancelamento, Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoIntegracao.Integrado);
                int totalProblemaIntegracao = repLoteEscrituracaoCancelamentoEDIIntegracao.ContarPorLoteEscrituracao(codigoLoteEscrituracaoCancelamento, Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoIntegracao.ProblemaIntegracao);

                var retorno = new
                {
                    TotalAguardandoIntegracao = totalAguardandoIntegracao,
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
                List<AdminMultisoftware.Dominio.Enumeradores.PermissaoPersonalizada> permissoesPersonalizadas = ObterPermissoesPersonalizadas("Escrituracao/LoteEscrituracaoCancelamento");
                if (!permissoesPersonalizadas.Contains(AdminMultisoftware.Dominio.Enumeradores.PermissaoPersonalizada.Carga_ReenviarIntegracoes))
                    return new JsonpResult(false, true, "Você não possui permissões para executar esta ação.");

                int codigo = Request.GetIntParam("Codigo");

                Repositorio.Embarcador.Escrituracao.LoteEscrituracaoCancelamentoEDIIntegracao repLoteEscrituracaoCancelamentoEDIIntegracao = new Repositorio.Embarcador.Escrituracao.LoteEscrituracaoCancelamentoEDIIntegracao(unitOfWork);

                Dominio.Entidades.Embarcador.Escrituracao.LoteEscrituracaoCancelamentoEDIIntegracao loteEscrituracaoCancelamentoEDIIntegracao = repLoteEscrituracaoCancelamentoEDIIntegracao.BuscarPorCodigo(codigo);

                if (loteEscrituracaoCancelamentoEDIIntegracao == null)
                    return new JsonpResult(false, "Integração não encontrada.");

                unitOfWork.Start();

                loteEscrituracaoCancelamentoEDIIntegracao.IniciouConexaoExterna = false;
                loteEscrituracaoCancelamentoEDIIntegracao.SituacaoIntegracao = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoIntegracao.AgIntegracao;

                Servicos.Auditoria.Auditoria.Auditar(Auditado, loteEscrituracaoCancelamentoEDIIntegracao, null, "Reenviou a Integração.", unitOfWork);

                repLoteEscrituracaoCancelamentoEDIIntegracao.Atualizar(loteEscrituracaoCancelamentoEDIIntegracao);

                unitOfWork.CommitChanges();

                return new JsonpResult(true);
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

        public async Task<IActionResult> ReenviarTodos()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                List<AdminMultisoftware.Dominio.Enumeradores.PermissaoPersonalizada> permissoesPersonalizadas = ObterPermissoesPersonalizadas("Escrituracao/LoteEscrituracaoCancelamento");

                if (!permissoesPersonalizadas.Contains(AdminMultisoftware.Dominio.Enumeradores.PermissaoPersonalizada.Carga_ReenviarIntegracoes))
                    return new JsonpResult(false, true, "Você não possui permissões para executar esta ação.");

                Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoIntegracao? situacao = Request.GetNullableEnumParam<Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoIntegracao>("Situacao");

                int codigoLoteEscrituracaoCancelamento = Request.GetIntParam("LoteEscrituracaoCancelamento");

                Repositorio.Embarcador.Escrituracao.LoteEscrituracaoCancelamentoEDIIntegracao repLoteEscrituracaoCancelamentoEDIIntegracao = new Repositorio.Embarcador.Escrituracao.LoteEscrituracaoCancelamentoEDIIntegracao(unitOfWork);

                List<Dominio.Entidades.Embarcador.Escrituracao.LoteEscrituracaoCancelamentoEDIIntegracao> integracoes = repLoteEscrituracaoCancelamentoEDIIntegracao.BuscarPorLoteEscrituracao(codigoLoteEscrituracaoCancelamento, situacao);

                foreach (Dominio.Entidades.Embarcador.Escrituracao.LoteEscrituracaoCancelamentoEDIIntegracao integracao in integracoes)
                {
                    unitOfWork.Start();

                    if (integracao.TipoIntegracao.Tipo != Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoIntegracao.NaoPossuiIntegracao &&
                        integracao.TipoIntegracao.Tipo != Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoIntegracao.NaoInformada)
                    {
                        integracao.SituacaoIntegracao = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoIntegracao.AgIntegracao;
                        integracao.IniciouConexaoExterna = false;

                        Servicos.Auditoria.Auditoria.Auditar(Auditado, integracao, null, "Reenviou a Integração.", unitOfWork);

                        repLoteEscrituracaoCancelamentoEDIIntegracao.Atualizar(integracao);
                    }

                    unitOfWork.CommitChanges();
                }

                return new JsonpResult(true);
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

        public async Task<IActionResult> Download()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                List<AdminMultisoftware.Dominio.Enumeradores.PermissaoPersonalizada> permissoesPersonalizadas = ObterPermissoesPersonalizadas("Escrituracao/LoteEscrituracaoCancelamento");

                if (!permissoesPersonalizadas.Contains(AdminMultisoftware.Dominio.Enumeradores.PermissaoPersonalizada.Carga_DownloadArquivoIntegracoes))
                    return new JsonpResult(false, true, "Você não possui permissões para executar esta ação.");

                int codigo = Request.GetIntParam("Codigo");

                Repositorio.Embarcador.Escrituracao.LoteEscrituracaoCancelamentoEDIIntegracao repLoteEscrituracaoCancelamentoEDIIntegracao = new Repositorio.Embarcador.Escrituracao.LoteEscrituracaoCancelamentoEDIIntegracao(unitOfWork);

                Dominio.Entidades.Embarcador.Escrituracao.LoteEscrituracaoCancelamentoEDIIntegracao loteEscrituracaoCancelamentoEDIIntegracao = repLoteEscrituracaoCancelamentoEDIIntegracao.BuscarPorCodigo(codigo);

                if (loteEscrituracaoCancelamentoEDIIntegracao == null)
                    return new JsonpResult(false, "Integração não encontrada.");

                string extensao = string.Empty;

                System.IO.MemoryStream edi = Servicos.Embarcador.Integracao.IntegracaoEDI.GerarEDI(loteEscrituracaoCancelamentoEDIIntegracao, TipoServicoMultisoftware, unitOfWork, _conexao.StringConexao, out extensao);
                string nomeArquivo = Servicos.Embarcador.Integracao.IntegracaoEDI.ObterNomeArquivoEDI(loteEscrituracaoCancelamentoEDIIntegracao, extensao, unitOfWork);

                if ((loteEscrituracaoCancelamentoEDIIntegracao.LayoutEDI.Tipo == Dominio.Enumeradores.TipoLayoutEDI.FISCAL ||
                     loteEscrituracaoCancelamentoEDIIntegracao.LayoutEDI.Tipo == Dominio.Enumeradores.TipoLayoutEDI.UVT_RN) &&
                    loteEscrituracaoCancelamentoEDIIntegracao.TipoIntegracao.Tipo == Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoIntegracao.NaoPossuiIntegracao &&
                    loteEscrituracaoCancelamentoEDIIntegracao.SituacaoIntegracao == Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoIntegracao.AgIntegracao)
                {
                    loteEscrituracaoCancelamentoEDIIntegracao.SituacaoIntegracao = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoIntegracao.Integrado;

                    repLoteEscrituracaoCancelamentoEDIIntegracao.Atualizar(loteEscrituracaoCancelamentoEDIIntegracao);
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
    }
}
