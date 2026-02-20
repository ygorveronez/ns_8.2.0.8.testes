using Microsoft.AspNetCore.Mvc;
using SGTAdmin.Controllers;

namespace SGT.WebAdmin.Controllers.Financeiros
{
    [CustomAuthorize(new string[] { "ObterTotais", "Download" }, "Financeiros/LoteContabilizacao")]
    public class LoteContabilizacaoIntegracaoEDIController : BaseController
    {
        #region Construtores

        public LoteContabilizacaoIntegracaoEDIController(Conexao conexao) : base(conexao) { }

        #endregion

        public async Task<IActionResult> Pesquisa()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoIntegracao? situacao = Request.GetNullableEnumParam<Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoIntegracao>("Situacao");

                int codigoLoteContabilizacao = Request.GetIntParam("LoteContabilizacao");

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

                Repositorio.Embarcador.Financeiro.LoteContabilizacaoIntegracaoEDI repIntegracao = new Repositorio.Embarcador.Financeiro.LoteContabilizacaoIntegracaoEDI(unitOfWork);

                int countEDIs = repIntegracao.ContarConsulta(codigoLoteContabilizacao, situacao);

                List<Dominio.Entidades.Embarcador.Financeiro.LoteContabilizacaoIntegracaoEDI> listaIntegracao = new List<Dominio.Entidades.Embarcador.Financeiro.LoteContabilizacaoIntegracaoEDI>();

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

                    listaIntegracao = repIntegracao.Consultar(codigoLoteContabilizacao, situacao, propOrdena, grid.dirOrdena, grid.inicio, grid.limite);
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

        public async Task<IActionResult> ObterTotais(CancellationToken cancellationToken)
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                int codigoLoteContabilizacao = Request.GetIntParam("LoteContabilizacao");

                Repositorio.Embarcador.Financeiro.LoteContabilizacaoIntegracaoEDI repositorioLoteLoteContabilizacaoIntegracaoEDI = new Repositorio.Embarcador.Financeiro.LoteContabilizacaoIntegracaoEDI(unitOfWork, cancellationToken);

                int totalAguardandoIntegracao = await repositorioLoteLoteContabilizacaoIntegracaoEDI.ContarPorLoteContabilizacaoAsync(codigoLoteContabilizacao, Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoIntegracao.AgIntegracao);
                int totalIntegrado = await repositorioLoteLoteContabilizacaoIntegracaoEDI.ContarPorLoteContabilizacaoAsync(codigoLoteContabilizacao, Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoIntegracao.Integrado);
                int totalProblemaIntegracao = await repositorioLoteLoteContabilizacaoIntegracaoEDI.ContarPorLoteContabilizacaoAsync(codigoLoteContabilizacao, Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoIntegracao.ProblemaIntegracao);

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
                List<AdminMultisoftware.Dominio.Enumeradores.PermissaoPersonalizada> permissoesPersonalizadas = ObterPermissoesPersonalizadas("Financeiros/LoteContabilizacao");
                if (!permissoesPersonalizadas.Contains(AdminMultisoftware.Dominio.Enumeradores.PermissaoPersonalizada.Carga_ReenviarIntegracoes))
                    return new JsonpResult(false, true, "Você não possui permissões para executar esta ação.");

                int codigo = Request.GetIntParam("Codigo");

                Repositorio.Embarcador.Financeiro.LoteContabilizacaoIntegracaoEDI repLoteContabilizacaoIntegracaoEDI = new Repositorio.Embarcador.Financeiro.LoteContabilizacaoIntegracaoEDI(unitOfWork);

                Dominio.Entidades.Embarcador.Financeiro.LoteContabilizacaoIntegracaoEDI loteContabilizacaoIntegracaoEDI = repLoteContabilizacaoIntegracaoEDI.BuscarPorCodigo(codigo, false);

                if (loteContabilizacaoIntegracaoEDI == null)
                    return new JsonpResult(false, "Integração não encontrada.");

                //List<AdminMultisoftware.Dominio.Enumeradores.PermissaoPersonalizada> permissoesPersonalizadas = ObterPermissoesPersonalizadas("LoteEscrituracaos/LoteEscrituracao", "Logistica/JanelaCarregamento");
                //if (!permissoesPersonalizadas.Contains(AdminMultisoftware.Dominio.Enumeradores.PermissaoPersonalizada.LoteEscrituracao_ReenviarIntegracoes) && cargaEDIIntegracao.SituacaoIntegracao != Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoIntegracao.ProblemaIntegracao)
                //    return new JsonpResult(false, true, "Você não possui permissões para executar esta ação.");

                loteContabilizacaoIntegracaoEDI.IniciouConexaoExterna = false;
                loteContabilizacaoIntegracaoEDI.SituacaoIntegracao = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoIntegracao.AgIntegracao;

                Servicos.Auditoria.Auditoria.Auditar(Auditado, loteContabilizacaoIntegracaoEDI, null, "Reenviou a Integração.", unitOfWork);

                repLoteContabilizacaoIntegracaoEDI.Atualizar(loteContabilizacaoIntegracaoEDI);

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

        public async Task<IActionResult> ReenviarTodos()
        {
            //List<AdminMultisoftware.Dominio.Enumeradores.PermissaoPersonalizada> permissoesPersonalizadas = ObterPermissoesPersonalizadas("LoteEscrituracaos/LoteEscrituracao", "Logistica/JanelaCarregamento");
            //if (!permissoesPersonalizadas.Contains(AdminMultisoftware.Dominio.Enumeradores.PermissaoPersonalizada.LoteEscrituracao_ReenviarIntegracoes))
            //    return new JsonpResult(false, true, "Você não possui permissões para executar esta ação.");

            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                List<AdminMultisoftware.Dominio.Enumeradores.PermissaoPersonalizada> permissoesPersonalizadas = ObterPermissoesPersonalizadas("Financeiros/LoteContabilizacao");
                if (!permissoesPersonalizadas.Contains(AdminMultisoftware.Dominio.Enumeradores.PermissaoPersonalizada.Carga_ReenviarIntegracoes))
                    return new JsonpResult(false, true, "Você não possui permissões para executar esta ação.");

                Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoIntegracao? situacao = Request.GetNullableEnumParam<Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoIntegracao>("Situacao");

                int codigoLoteContabilizacao = Request.GetIntParam("LoteContabilizacao");

                Repositorio.Embarcador.Financeiro.LoteContabilizacaoIntegracaoEDI repLoteContabilizacaoIntegracaoEDI = new Repositorio.Embarcador.Financeiro.LoteContabilizacaoIntegracaoEDI(unitOfWork);

                List<Dominio.Entidades.Embarcador.Financeiro.LoteContabilizacaoIntegracaoEDI> integracoes = repLoteContabilizacaoIntegracaoEDI.BuscarPorLoteContabilizacao(codigoLoteContabilizacao, situacao);

                foreach (Dominio.Entidades.Embarcador.Financeiro.LoteContabilizacaoIntegracaoEDI integracao in integracoes)
                {
                    if (integracao.TipoIntegracao.Tipo != Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoIntegracao.NaoPossuiIntegracao)
                    {
                        unitOfWork.Start();

                        integracao.SituacaoIntegracao = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoIntegracao.AgIntegracao;
                        integracao.IniciouConexaoExterna = false;

                        Servicos.Auditoria.Auditoria.Auditar(Auditado, integracao, null, "Reenviou a integração.", unitOfWork);

                        repLoteContabilizacaoIntegracaoEDI.Atualizar(integracao);

                        unitOfWork.CommitChanges();
                    }
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
                List<AdminMultisoftware.Dominio.Enumeradores.PermissaoPersonalizada> permissoesPersonalizadas = ObterPermissoesPersonalizadas("Financeiros/LoteContabilizacao");
                if (!permissoesPersonalizadas.Contains(AdminMultisoftware.Dominio.Enumeradores.PermissaoPersonalizada.Carga_DownloadArquivoIntegracoes))
                    return new JsonpResult(false, true, "Você não possui permissões para executar esta ação.");

                int codigo = Request.GetIntParam("Codigo");

                Repositorio.Embarcador.Financeiro.LoteContabilizacaoIntegracaoEDI repLoteLoteContabilizacaoIntegracaoEDI = new Repositorio.Embarcador.Financeiro.LoteContabilizacaoIntegracaoEDI(unitOfWork);

                Dominio.Entidades.Embarcador.Financeiro.LoteContabilizacaoIntegracaoEDI loteContabilizacaoIntegracaoEDI = repLoteLoteContabilizacaoIntegracaoEDI.BuscarPorCodigo(codigo, false);

                if (loteContabilizacaoIntegracaoEDI == null)
                    return new JsonpResult(false, "Integração não encontrada.");

                string extensao = string.Empty;

                System.IO.MemoryStream edi = Servicos.Embarcador.Integracao.IntegracaoEDI.GerarEDI(loteContabilizacaoIntegracaoEDI, TipoServicoMultisoftware, unitOfWork, out extensao);
                string nomeArquivo = Servicos.Embarcador.Integracao.IntegracaoEDI.ObterNomeArquivoEDI(loteContabilizacaoIntegracaoEDI, extensao, unitOfWork);

                if ((loteContabilizacaoIntegracaoEDI.LayoutEDI.Tipo == Dominio.Enumeradores.TipoLayoutEDI.FISCAL ||
                     loteContabilizacaoIntegracaoEDI.LayoutEDI.Tipo == Dominio.Enumeradores.TipoLayoutEDI.UVT_RN) &&
                    loteContabilizacaoIntegracaoEDI.TipoIntegracao.Tipo == Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoIntegracao.NaoPossuiIntegracao &&
                    loteContabilizacaoIntegracaoEDI.SituacaoIntegracao == Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoIntegracao.AgIntegracao)
                {
                    loteContabilizacaoIntegracaoEDI.SituacaoIntegracao = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoIntegracao.Integrado;

                    repLoteLoteContabilizacaoIntegracaoEDI.Atualizar(loteContabilizacaoIntegracaoEDI);
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
