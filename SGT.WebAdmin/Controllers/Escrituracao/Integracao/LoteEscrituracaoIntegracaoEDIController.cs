using SGTAdmin.Controllers;
using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Mvc;

namespace SGT.WebAdmin.Controllers.Escrituracao.Integracao
{
    [CustomAuthorize(new string[] { "ObterTotais", "Download" }, "Escrituracao/LoteEscrituracao")]
    public class LoteEscrituracaoIntegracaoEDIController : BaseController
    {
		#region Construtores

		public LoteEscrituracaoIntegracaoEDIController(Conexao conexao) : base(conexao) { }

		#endregion

        public async Task<IActionResult> Pesquisa()
        {
            Repositorio.UnitOfWork unidadeDeTrabalho = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoIntegracao? situacao = null;
                Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoIntegracao situacaoAux;
                if (Enum.TryParse(Request.Params("Situacao"), out situacaoAux))
                    situacao = situacaoAux;

                int codigoLoteEscrituracao;
                int.TryParse(Request.Params("LoteEscrituracao"), out codigoLoteEscrituracao);

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

                Repositorio.Embarcador.Escrituracao.LoteEscrituracaoEDIIntegracao repIntegracao = new Repositorio.Embarcador.Escrituracao.LoteEscrituracaoEDIIntegracao(unidadeDeTrabalho);

                int countEDIs = repIntegracao.ContarConsulta(codigoLoteEscrituracao, situacao);

                List<Dominio.Entidades.Embarcador.Escrituracao.LoteEscrituracaoEDIIntegracao> listaIntegracao = new List<Dominio.Entidades.Embarcador.Escrituracao.LoteEscrituracaoEDIIntegracao>();

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

                    listaIntegracao = repIntegracao.Consultar(codigoLoteEscrituracao, situacao, propOrdena, grid.dirOrdena, grid.inicio, grid.limite);
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
                unidadeDeTrabalho.Dispose();
            }
        }

        public async Task<IActionResult> ObterTotais()
        {
            Repositorio.UnitOfWork unidadeDeTrabalho = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                int codigoLoteEscrituracao;
                int.TryParse(Request.Params("LoteEscrituracao"), out codigoLoteEscrituracao);

                Repositorio.Embarcador.Escrituracao.LoteEscrituracaoEDIIntegracao repLoteLoteEscrituracaoEDIIntegracao = new Repositorio.Embarcador.Escrituracao.LoteEscrituracaoEDIIntegracao(unidadeDeTrabalho);

                int totalAguardandoIntegracao = repLoteLoteEscrituracaoEDIIntegracao.ContarPorLoteEscrituracao(codigoLoteEscrituracao, Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoIntegracao.AgIntegracao);
                int totalIntegrado = repLoteLoteEscrituracaoEDIIntegracao.ContarPorLoteEscrituracao(codigoLoteEscrituracao, Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoIntegracao.Integrado);
                int totalProblemaIntegracao = repLoteLoteEscrituracaoEDIIntegracao.ContarPorLoteEscrituracao(codigoLoteEscrituracao, Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoIntegracao.ProblemaIntegracao);

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
                unidadeDeTrabalho.Dispose();
            }
        }

        public async Task<IActionResult> Reenviar()
        {


            Repositorio.UnitOfWork unidadeDeTrabalho = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                List<AdminMultisoftware.Dominio.Enumeradores.PermissaoPersonalizada> permissoesPersonalizadas = ObterPermissoesPersonalizadas("Escrituracao/LoteEscrituracao");
                if (!permissoesPersonalizadas.Contains(AdminMultisoftware.Dominio.Enumeradores.PermissaoPersonalizada.Carga_ReenviarIntegracoes))
                    return new JsonpResult(false, true, "Você não possui permissões para executar esta ação.");

                int codigo;
                int.TryParse(Request.Params("Codigo"), out codigo);

                Repositorio.Embarcador.Escrituracao.LoteEscrituracaoEDIIntegracao repLoteLoteEscrituracaoEDIIntegracao = new Repositorio.Embarcador.Escrituracao.LoteEscrituracaoEDIIntegracao(unidadeDeTrabalho);

                Dominio.Entidades.Embarcador.Escrituracao.LoteEscrituracaoEDIIntegracao cargaEDIIntegracao = repLoteLoteEscrituracaoEDIIntegracao.BuscarPorCodigo(codigo);

                if (cargaEDIIntegracao == null)
                    return new JsonpResult(false, "Integração não encontrada.");


                //List<AdminMultisoftware.Dominio.Enumeradores.PermissaoPersonalizada> permissoesPersonalizadas = ObterPermissoesPersonalizadas("LoteEscrituracaos/LoteEscrituracao", "Logistica/JanelaCarregamento");
                //if (!permissoesPersonalizadas.Contains(AdminMultisoftware.Dominio.Enumeradores.PermissaoPersonalizada.LoteEscrituracao_ReenviarIntegracoes) && cargaEDIIntegracao.SituacaoIntegracao != Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoIntegracao.ProblemaIntegracao)
                //    return new JsonpResult(false, true, "Você não possui permissões para executar esta ação.");
                cargaEDIIntegracao.IniciouConexaoExterna = false;
                cargaEDIIntegracao.SituacaoIntegracao = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoIntegracao.AgIntegracao;
                Servicos.Auditoria.Auditoria.Auditar(Auditado, cargaEDIIntegracao, null, "Reenviou a Integração.", unidadeDeTrabalho);

                repLoteLoteEscrituracaoEDIIntegracao.Atualizar(cargaEDIIntegracao);

                return new JsonpResult(true);
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);

                return new JsonpResult(false, "Ocorreu uma falha ao enviar a integração.");
            }
            finally
            {
                unidadeDeTrabalho.Dispose();
            }
        }

        public async Task<IActionResult> ReenviarTodos()
        {
            //List<AdminMultisoftware.Dominio.Enumeradores.PermissaoPersonalizada> permissoesPersonalizadas = ObterPermissoesPersonalizadas("LoteEscrituracaos/LoteEscrituracao", "Logistica/JanelaCarregamento");
            //if (!permissoesPersonalizadas.Contains(AdminMultisoftware.Dominio.Enumeradores.PermissaoPersonalizada.LoteEscrituracao_ReenviarIntegracoes))
            //    return new JsonpResult(false, true, "Você não possui permissões para executar esta ação.");

            Repositorio.UnitOfWork unidadeDeTrabalho = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                List<AdminMultisoftware.Dominio.Enumeradores.PermissaoPersonalizada> permissoesPersonalizadas = ObterPermissoesPersonalizadas("Escrituracao/LoteEscrituracao");
                if (!permissoesPersonalizadas.Contains(AdminMultisoftware.Dominio.Enumeradores.PermissaoPersonalizada.Carga_ReenviarIntegracoes))
                    return new JsonpResult(false, true, "Você não possui permissões para executar esta ação.");

                Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoIntegracao? situacao = null;
                Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoIntegracao situacaoAux;
                if (Enum.TryParse(Request.Params("Situacao"), out situacaoAux))
                    situacao = situacaoAux;

                int codigoLoteEscrituracao = 0;
                int.TryParse(Request.Params("LoteEscrituracao"), out codigoLoteEscrituracao);

                Repositorio.Embarcador.Escrituracao.LoteEscrituracaoEDIIntegracao repLoteLoteEscrituracaoEDIIntegracao = new Repositorio.Embarcador.Escrituracao.LoteEscrituracaoEDIIntegracao(unidadeDeTrabalho);

                List<Dominio.Entidades.Embarcador.Escrituracao.LoteEscrituracaoEDIIntegracao> integracoes = repLoteLoteEscrituracaoEDIIntegracao.BuscarPorLoteEscrituracao(codigoLoteEscrituracao, situacao);

                foreach (Dominio.Entidades.Embarcador.Escrituracao.LoteEscrituracaoEDIIntegracao integracao in integracoes)
                {
                    if (integracao.TipoIntegracao.Tipo != Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoIntegracao.NaoPossuiIntegracao)
                    {
                        integracao.SituacaoIntegracao = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoIntegracao.AgIntegracao;
                        integracao.IniciouConexaoExterna = false;
                        Servicos.Auditoria.Auditoria.Auditar(Auditado, integracao, null, "Reenviou a Integração.", unidadeDeTrabalho);
                        repLoteLoteEscrituracaoEDIIntegracao.Atualizar(integracao);
                    }
                }

                return new JsonpResult(true);
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);

                return new JsonpResult(false, "Ocorreu uma falha ao enviar a integração.");
            }
            finally
            {
                unidadeDeTrabalho.Dispose();
            }
        }

        public async Task<IActionResult> Download()
        {
            Repositorio.UnitOfWork unidadeDeTrabalho = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                List<AdminMultisoftware.Dominio.Enumeradores.PermissaoPersonalizada> permissoesPersonalizadas = ObterPermissoesPersonalizadas("Escrituracao/LoteEscrituracao");
                if (!permissoesPersonalizadas.Contains(AdminMultisoftware.Dominio.Enumeradores.PermissaoPersonalizada.Carga_DownloadArquivoIntegracoes))
                    return new JsonpResult(false, true, "Você não possui permissões para executar esta ação.");

                int codigo;
                int.TryParse(Request.Params("Codigo"), out codigo);

                Repositorio.Embarcador.Escrituracao.LoteEscrituracaoEDIIntegracao repLoteLoteEscrituracaoEDIIntegracao = new Repositorio.Embarcador.Escrituracao.LoteEscrituracaoEDIIntegracao(unidadeDeTrabalho);

                Dominio.Entidades.Embarcador.Escrituracao.LoteEscrituracaoEDIIntegracao loteEscrituracaoEDIIntegracao = repLoteLoteEscrituracaoEDIIntegracao.BuscarPorCodigo(codigo);

                if (loteEscrituracaoEDIIntegracao == null)
                    return new JsonpResult(false, "Integração não encontrada.");

                string extensao = string.Empty;

                System.IO.MemoryStream edi = Servicos.Embarcador.Integracao.IntegracaoEDI.GerarEDI(loteEscrituracaoEDIIntegracao, TipoServicoMultisoftware, unidadeDeTrabalho, _conexao.StringConexao, out extensao);
                string nomeArquivo = Servicos.Embarcador.Integracao.IntegracaoEDI.ObterNomeArquivoEDI(loteEscrituracaoEDIIntegracao, extensao, unidadeDeTrabalho);

                if ((loteEscrituracaoEDIIntegracao.LayoutEDI.Tipo == Dominio.Enumeradores.TipoLayoutEDI.FISCAL ||
                     loteEscrituracaoEDIIntegracao.LayoutEDI.Tipo == Dominio.Enumeradores.TipoLayoutEDI.UVT_RN) &&
                    loteEscrituracaoEDIIntegracao.TipoIntegracao.Tipo == Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoIntegracao.NaoPossuiIntegracao &&
                    loteEscrituracaoEDIIntegracao.SituacaoIntegracao == Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoIntegracao.AgIntegracao)
                {
                    unidadeDeTrabalho.Start();

                    loteEscrituracaoEDIIntegracao.SituacaoIntegracao = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoIntegracao.Integrado;

                    repLoteLoteEscrituracaoEDIIntegracao.Atualizar(loteEscrituracaoEDIIntegracao);

                    Repositorio.Embarcador.Configuracao.ConfiguracaoTMS repConfiguracao = new Repositorio.Embarcador.Configuracao.ConfiguracaoTMS(unidadeDeTrabalho);

                    Dominio.Entidades.Embarcador.Configuracao.ConfiguracaoTMS configuracao = repConfiguracao.BuscarConfiguracaoPadrao();

                    // Servicos.Embarcador.Integracao.IntegracaoEscrituracao.AtualizarSituacaoEscrituracaoIntegracao(loteEscrituracaoEDIIntegracao.LoteEscrituracao, configuracao, unidadeDeTrabalho, _conexao.StringConexao, TipoServicoMultisoftware);

                    unidadeDeTrabalho.CommitChanges();
                }

                return Arquivo(edi, extensao == ".zip" ? "application/zip" : "plain/text", nomeArquivo);
            }
            catch (Exception ex)
            {
                unidadeDeTrabalho.Rollback();

                Servicos.Log.TratarErro(ex);

                return new JsonpResult(false, "Ocorreu uma falha ao enviar a integração.");
            }
            finally
            {
                unidadeDeTrabalho.Dispose();
            }
        }
    }
}
