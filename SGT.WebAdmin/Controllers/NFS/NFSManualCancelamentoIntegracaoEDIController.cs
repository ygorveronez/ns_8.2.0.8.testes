using SGTAdmin.Controllers;
using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Mvc;

namespace SGT.WebAdmin.Controllers.NFS
{
    [CustomAuthorize(new string[] { "ObterTotais", "Download" }, "NFS/NFSManualCancelamentoIntegracaoEDI")]
    public class NFSManualCancelamentoIntegracaoEDIController : BaseController
    {
		#region Construtores

		public NFSManualCancelamentoIntegracaoEDIController(Conexao conexao) : base(conexao) { }

		#endregion

        public async Task<IActionResult> Pesquisa()
        {
            Repositorio.UnitOfWork unidadeDeTrabalho = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoIntegracao? situacao = Request.GetNullableEnumParam<Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoIntegracao>("Situacao");

                int codigoNFSManualCancelamento = Request.GetIntParam("NFSManualCancelamento");

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

                Repositorio.Embarcador.NFS.NFSManualCancelamentoIntegracaoEDI repIntegracao = new Repositorio.Embarcador.NFS.NFSManualCancelamentoIntegracaoEDI(unidadeDeTrabalho);

                int countEDIs = repIntegracao.ContarConsulta(codigoNFSManualCancelamento, situacao);

                List<Dominio.Entidades.Embarcador.NFS.NFSManualCancelamentoIntegracaoEDI> listaIntegracao = new List<Dominio.Entidades.Embarcador.NFS.NFSManualCancelamentoIntegracaoEDI>();

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

                    listaIntegracao = repIntegracao.Consultar(codigoNFSManualCancelamento, situacao, propOrdena, grid.dirOrdena, grid.inicio, grid.limite);
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
                int codigoNFSManualCancelamento = Request.GetIntParam("NFSManualCancelamento");

                Repositorio.Embarcador.NFS.NFSManualCancelamentoIntegracaoEDI repNFSManualCancelamentoIntegracaoEDI = new Repositorio.Embarcador.NFS.NFSManualCancelamentoIntegracaoEDI(unidadeDeTrabalho);

                int totalAguardandoIntegracao = repNFSManualCancelamentoIntegracaoEDI.ContarPorNFSManualCancelamento(codigoNFSManualCancelamento, Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoIntegracao.AgIntegracao);
                int totalIntegrado = repNFSManualCancelamentoIntegracaoEDI.ContarPorNFSManualCancelamento(codigoNFSManualCancelamento, Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoIntegracao.Integrado);
                int totalProblemaIntegracao = repNFSManualCancelamentoIntegracaoEDI.ContarPorNFSManualCancelamento(codigoNFSManualCancelamento, Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoIntegracao.ProblemaIntegracao);

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
                int codigo;
                int.TryParse(Request.Params("Codigo"), out codigo);

                Repositorio.Embarcador.NFS.NFSManualCancelamentoIntegracaoEDI repNFSManualCancelamentoIntegracaoEDI = new Repositorio.Embarcador.NFS.NFSManualCancelamentoIntegracaoEDI(unidadeDeTrabalho);

                Dominio.Entidades.Embarcador.NFS.NFSManualCancelamentoIntegracaoEDI cargaEDIIntegracao = repNFSManualCancelamentoIntegracaoEDI.BuscarPorCodigo(codigo, false);

                if (cargaEDIIntegracao == null)
                    return new JsonpResult(false, "Integração não encontrada.");

                unidadeDeTrabalho.Start();

                Servicos.Auditoria.Auditoria.Auditar(Auditado, cargaEDIIntegracao, null, "Reenviou a Integração.", unidadeDeTrabalho);

                cargaEDIIntegracao.IniciouConexaoExterna = false;
                cargaEDIIntegracao.SituacaoIntegracao = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoIntegracao.AgIntegracao;

                repNFSManualCancelamentoIntegracaoEDI.Atualizar(cargaEDIIntegracao);

                unidadeDeTrabalho.CommitChanges();

                return new JsonpResult(true);
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

        public async Task<IActionResult> ReenviarTodos()
        {
            Repositorio.UnitOfWork unidadeDeTrabalho = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoIntegracao? situacao = Request.GetNullableEnumParam<Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoIntegracao>("Situacao");

                int codigoNFSManualCancelamento = Request.GetIntParam("NFSManualCancelamento");

                Repositorio.Embarcador.NFS.NFSManualCancelamentoIntegracaoEDI repNFSManualCancelamentoIntegracaoEDI = new Repositorio.Embarcador.NFS.NFSManualCancelamentoIntegracaoEDI(unidadeDeTrabalho);

                List<Dominio.Entidades.Embarcador.NFS.NFSManualCancelamentoIntegracaoEDI> integracoes = repNFSManualCancelamentoIntegracaoEDI.BuscarPorNFSManualCancelamento(codigoNFSManualCancelamento, situacao);

                unidadeDeTrabalho.Start();

                foreach (Dominio.Entidades.Embarcador.NFS.NFSManualCancelamentoIntegracaoEDI integracao in integracoes)
                {
                    if (integracao.TipoIntegracao.Tipo == Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoIntegracao.NaoPossuiIntegracao)
                        continue;

                    Servicos.Auditoria.Auditoria.Auditar(Auditado, integracao, null, "Reenviou a Integração.", unidadeDeTrabalho);

                    integracao.SituacaoIntegracao = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoIntegracao.AgIntegracao;
                    integracao.IniciouConexaoExterna = false;

                    repNFSManualCancelamentoIntegracaoEDI.Atualizar(integracao);
                }

                unidadeDeTrabalho.CommitChanges();

                return new JsonpResult(true);
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

        public async Task<IActionResult> Download()
        {
            Repositorio.UnitOfWork unidadeDeTrabalho = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                int codigo;
                int.TryParse(Request.Params("Codigo"), out codigo);

                Repositorio.Embarcador.NFS.NFSManualCancelamentoIntegracaoEDI repNFSManualCancelamentoIntegracaoEDI = new Repositorio.Embarcador.NFS.NFSManualCancelamentoIntegracaoEDI(unidadeDeTrabalho);

                Dominio.Entidades.Embarcador.NFS.NFSManualCancelamentoIntegracaoEDI nfsManualEDIIntegracao = repNFSManualCancelamentoIntegracaoEDI.BuscarPorCodigo(codigo, false);

                if (nfsManualEDIIntegracao == null)
                    return new JsonpResult(false, "Integração não encontrada.");

                string extensao = string.Empty;

                System.IO.MemoryStream edi = Servicos.Embarcador.Integracao.IntegracaoEDI.GerarEDI(nfsManualEDIIntegracao, TipoServicoMultisoftware, unidadeDeTrabalho, out extensao);

                string nomeArquivo = Servicos.Embarcador.Integracao.IntegracaoEDI.ObterNomeArquivoEDI(nfsManualEDIIntegracao, extensao, unidadeDeTrabalho);

                if ((nfsManualEDIIntegracao.LayoutEDI.Tipo == Dominio.Enumeradores.TipoLayoutEDI.FISCAL ||
                     nfsManualEDIIntegracao.LayoutEDI.Tipo == Dominio.Enumeradores.TipoLayoutEDI.UVT_RN) &&
                    nfsManualEDIIntegracao.TipoIntegracao.Tipo == Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoIntegracao.NaoPossuiIntegracao &&
                    nfsManualEDIIntegracao.SituacaoIntegracao == Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoIntegracao.AgIntegracao)
                {
                    unidadeDeTrabalho.Start();

                    nfsManualEDIIntegracao.SituacaoIntegracao = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoIntegracao.Integrado;

                    repNFSManualCancelamentoIntegracaoEDI.Atualizar(nfsManualEDIIntegracao);

                    Repositorio.Embarcador.Configuracao.ConfiguracaoTMS repConfiguracao = new Repositorio.Embarcador.Configuracao.ConfiguracaoTMS(unidadeDeTrabalho);

                    Dominio.Entidades.Embarcador.Configuracao.ConfiguracaoTMS configuracao = repConfiguracao.BuscarConfiguracaoPadrao();

                    Servicos.Embarcador.Integracao.IntegracaoNFSManualCancelamento.AtualizarSituacaoNFSManualCancelamentoIntegracao(nfsManualEDIIntegracao.NFSManualCancelamento, configuracao, unidadeDeTrabalho, TipoServicoMultisoftware);

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
