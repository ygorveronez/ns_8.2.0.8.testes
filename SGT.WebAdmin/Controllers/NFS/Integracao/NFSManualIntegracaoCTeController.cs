using Dominio.ObjetosDeValor.Embarcador.Enumeradores;
using SGTAdmin.Controllers;
using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Mvc;

namespace SGT.WebAdmin.Controllers.NFS.Integracao
{
    [CustomAuthorize(new string[] { "ConsultarHistoricoIntegracao", "DownloadArquivosHistoricoIntegracao", "ObterTotais" }, "NFS/NFSManual")]
    public class NFSManualIntegracaoCTeController : BaseController
    {
		#region Construtores

		public NFSManualIntegracaoCTeController(Conexao conexao) : base(conexao) { }

		#endregion

        #region Métodos Globais

        public async Task<IActionResult> Pesquisa()
        {
            Repositorio.UnitOfWork unidadeDeTrabalho = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                int codigoLancamentoNFSManual = Request.GetIntParam("LancamentoNFSManual");
                TipoIntegracao? tipo = Request.GetNullableEnumParam<TipoIntegracao>("Tipo");
                SituacaoIntegracao? situacao = Request.GetNullableEnumParam<SituacaoIntegracao>("Situacao");

                Models.Grid.Grid grid = new Models.Grid.Grid(Request)
                {
                    header = new List<Models.Grid.Head>()
                };

                grid.AdicionarCabecalho("Codigo", false);
                grid.AdicionarCabecalho("Tipo", false);
                grid.AdicionarCabecalho("SituacaoIntegracao", false);
                grid.AdicionarCabecalho("CT-e", "CTe", 10, Models.Grid.Align.left, true);
                grid.AdicionarCabecalho("Emissor", "Emissor", 10, Models.Grid.Align.left, true);
                grid.AdicionarCabecalho("Integração", "TipoIntegracao", 10, Models.Grid.Align.left, true);
                grid.AdicionarCabecalho("Tentativas", "Tentativas", 10, Models.Grid.Align.left, true);
                grid.AdicionarCabecalho("Data do Envio", "DataEnvio", 10, Models.Grid.Align.left, true);
                grid.AdicionarCabecalho("Situação", "Situacao", 10, Models.Grid.Align.left, true);
                grid.AdicionarCabecalho("Retorno", "Retorno", 20, Models.Grid.Align.left, false);

                Dominio.ObjetosDeValor.Embarcador.Consulta.ParametroConsulta parametrosConsulta = grid.ObterParametrosConsulta(ObterPropriedadeOrdenar);
                Repositorio.Embarcador.NFS.NFSManualCTeIntegracao repositorioIntegracao = new Repositorio.Embarcador.NFS.NFSManualCTeIntegracao(unidadeDeTrabalho);
                int totalRegistros = repositorioIntegracao.ContarConsulta(codigoLancamentoNFSManual, situacao, tipo);
                List<Dominio.Entidades.Embarcador.NFS.NFSManualCTeIntegracao> listaIntegracao = (totalRegistros > 0) ? repositorioIntegracao.Consultar(codigoLancamentoNFSManual, situacao, tipo, parametrosConsulta) : new List<Dominio.Entidades.Embarcador.NFS.NFSManualCTeIntegracao>();

                var listaIntegracaoRetornar = (
                    from o in listaIntegracao
                    select new
                    {
                        o.Codigo,
                        o.SituacaoIntegracao,
                        o.TipoIntegracao.Tipo,
                        CTe = o.LancamentoNFSManual.CTe?.Numero.ToString() ?? "" ,
                        Emissor = o.LancamentoNFSManual.CTe?.Empresa?.CNPJ_Formatado ?? "",
                        Situacao = o.DescricaoSituacaoIntegracao,
                        TipoIntegracao = o.TipoIntegracao.DescricaoTipo,
                        Retorno = o.ProblemaIntegracao,
                        Tentativas = o.NumeroTentativas,
                        DataEnvio = o.DataIntegracao.ToString("dd/MM/yyyy HH:mm:ss"),
                        DT_RowColor = o.SituacaoIntegracao.ObterCorLinha(),
                        DT_FontColor = o.SituacaoIntegracao.ObterCorFonte(),
                    }
                ).ToList();

                grid.AdicionaRows(listaIntegracaoRetornar);
                grid.setarQuantidadeTotal(totalRegistros);

                return new JsonpResult(grid);
            }
            catch (Exception excecao)
            {
                Servicos.Log.TratarErro(excecao);
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
                int codigoLancamentoNFSManual;
                int.TryParse(Request.Params("LancamentoNFSManual"), out codigoLancamentoNFSManual);
                

                Repositorio.Embarcador.NFS.NFSManualCTeIntegracao repNFSManualCTeIntegracao = new Repositorio.Embarcador.NFS.NFSManualCTeIntegracao(unidadeDeTrabalho);

                int totalAguardandoIntegracao = repNFSManualCTeIntegracao.ContarPorLancamentoNFSManual(codigoLancamentoNFSManual, Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoIntegracao.AgIntegracao);
                int totalIntegrado = repNFSManualCTeIntegracao.ContarPorLancamentoNFSManual(codigoLancamentoNFSManual, Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoIntegracao.Integrado);
                int totalProblemaIntegracao = repNFSManualCTeIntegracao.ContarPorLancamentoNFSManual(codigoLancamentoNFSManual, Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoIntegracao.ProblemaIntegracao);
                int totalAguardandoRetorno = repNFSManualCTeIntegracao.ContarPorLancamentoNFSManual(codigoLancamentoNFSManual, Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoIntegracao.AgRetorno);

                var retorno = new
                {
                    TotalAguardandoIntegracao = totalAguardandoIntegracao,
                    TotalAguardandoRetorno = totalAguardandoRetorno,
                    TotalIntegrado = totalIntegrado,
                    TotalProblemaIntegracao = totalProblemaIntegracao,
                    TotalGeral = totalAguardandoIntegracao + totalIntegrado + totalProblemaIntegracao + totalAguardandoRetorno
                };

                return new JsonpResult(retorno);
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);

                return new JsonpResult(false, "Ocorreu uma falha ao obter os totais das integrações de CT-e.");
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

                Repositorio.Embarcador.NFS.NFSManualCTeIntegracao repLancamentoNFSManualCTeIntegracao = new Repositorio.Embarcador.NFS.NFSManualCTeIntegracao(unidadeDeTrabalho);

                Dominio.Entidades.Embarcador.NFS.NFSManualCTeIntegracao nfsManualCTeIntegracao = repLancamentoNFSManualCTeIntegracao.BuscarPorCodigo(codigo);


                if (nfsManualCTeIntegracao == null)
                    return new JsonpResult(false, "Integração não encontrada.");

                if (nfsManualCTeIntegracao.TipoIntegracao.Tipo == Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoIntegracao.Natura)
                    return new JsonpResult(false, "Não é possível enviar as integrações da natura individualmente. Utilize a opção reenviar todos.");

                //List<AdminMultisoftware.Dominio.Enumeradores.PermissaoPersonalizada> permissoesPersonalizadas = ObterPermissoesPersonalizadas("LancamentoNFSManuals/LancamentoNFSManual", "Logistica/JanelaCarregamento");
                //if (!permissoesPersonalizadas.Contains(AdminMultisoftware.Dominio.Enumeradores.PermissaoPersonalizada.LancamentoNFSManual_ReenviarIntegracoes) && cargaCTeIntegracao.SituacaoIntegracao != Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoIntegracao.ProblemaIntegracao)
                //    return new JsonpResult(false, true, "Você não possui permissões para executar esta ação.");

                nfsManualCTeIntegracao.SituacaoIntegracao = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoIntegracao.AgIntegracao;
                nfsManualCTeIntegracao.Lote = null;
                Servicos.Auditoria.Auditoria.Auditar(Auditado, nfsManualCTeIntegracao, null, "Reenviou a Integração.", unidadeDeTrabalho);

                repLancamentoNFSManualCTeIntegracao.Atualizar(nfsManualCTeIntegracao);

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
            //List<AdminMultisoftware.Dominio.Enumeradores.PermissaoPersonalizada> permissoesPersonalizadas = ObterPermissoesPersonalizadas("LancamentoNFSManuals/LancamentoNFSManual", "Logistica/JanelaCarregamento");
            //if (!permissoesPersonalizadas.Contains(AdminMultisoftware.Dominio.Enumeradores.PermissaoPersonalizada.LancamentoNFSManual_ReenviarIntegracoes))
            //    return new JsonpResult(false, true, "Você não possui permissões para executar esta ação.");

            Repositorio.UnitOfWork unidadeDeTrabalho = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoIntegracao? situacao = null;
                Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoIntegracao situacaoAux;
                if (Enum.TryParse(Request.Params("Situacao"), out situacaoAux))
                    situacao = situacaoAux;

                Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoIntegracao? tipo = null;
                Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoIntegracao tipoAux;
                if (Enum.TryParse(Request.Params("Tipo"), out tipoAux))
                    tipo = tipoAux;

                int codigoLancamentoNFSManual = 0;
                int.TryParse(Request.Params("LancamentoNFSManual"), out codigoLancamentoNFSManual);

                Repositorio.Embarcador.NFS.NFSManualCTeIntegracao repLancamentoNFSManualCTeIntegracao = new Repositorio.Embarcador.NFS.NFSManualCTeIntegracao(unidadeDeTrabalho);

                List<Dominio.Entidades.Embarcador.NFS.NFSManualCTeIntegracao> integracoes = repLancamentoNFSManualCTeIntegracao.Consultar(codigoLancamentoNFSManual, situacao, tipo);

                bool utilizarTransacao = false;
                if (integracoes.Any(o => o.TipoIntegracao.Tipo == Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoIntegracao.Natura))
                    utilizarTransacao = true;

                if (utilizarTransacao)
                    unidadeDeTrabalho.Start();

                foreach (Dominio.Entidades.Embarcador.NFS.NFSManualCTeIntegracao integracao in integracoes)
                {
                    if (!utilizarTransacao)
                    {
                        unidadeDeTrabalho.FlushAndClear();
                    }

                    integracao.SituacaoIntegracao = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoIntegracao.AgIntegracao;
                    integracao.Lote = null;

                    repLancamentoNFSManualCTeIntegracao.Atualizar(integracao);
                    Servicos.Auditoria.Auditoria.Auditar(Auditado, integracao, null, "Reenviou a Integração.", unidadeDeTrabalho);
                }

                if (utilizarTransacao)
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

        public async Task<IActionResult> ConsultarHistoricoIntegracao()
        {
            Repositorio.UnitOfWork unidadeDeTrabalho = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                int codigo = 0;
                int.TryParse(Request.Params("Codigo"), out codigo);

                Repositorio.Embarcador.NFS.NFSManualCTeIntegracao repIntegracao = new Repositorio.Embarcador.NFS.NFSManualCTeIntegracao(unidadeDeTrabalho);

                Models.Grid.Grid grid = new Models.Grid.Grid(Request);

                grid.header = new List<Models.Grid.Head>();
                grid.AdicionarCabecalho("Codigo", false);
                grid.AdicionarCabecalho("Data", "Data", 20, Models.Grid.Align.left, false);
                grid.AdicionarCabecalho("Tipo", "DescricaoTipo", 20, Models.Grid.Align.left, false);
                grid.AdicionarCabecalho("Retorno", "Mensagem", 40, Models.Grid.Align.left, false);

                Dominio.Entidades.Embarcador.NFS.NFSManualCTeIntegracao integracao = repIntegracao.BuscarPorCodigo(codigo);
                grid.setarQuantidadeTotal(integracao.ArquivosTransacao.Count());

                var retorno = (from obj in integracao.ArquivosTransacao.OrderByDescending(o => o.Data).Skip(grid.inicio).Take(grid.limite)
                               select new
                               {
                                   obj.Codigo,
                                   Data = obj.Data.ToString("dd/MM/yyyy HH:mm:ss"),
                                   obj.DescricaoTipo,
                                   obj.Mensagem
                               }).ToList();

                grid.AdicionaRows(retorno);

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

        [AcceptVerbs("POST", "GET")]
        public async Task<IActionResult> DownloadArquivosHistoricoIntegracao()
        {
            Repositorio.UnitOfWork unidadeDeTrabalho = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                int codigo = 0;
                int.TryParse(Request.Params("Codigo"), out codigo);

                Repositorio.Embarcador.NFS.NFSManualCTeIntegracao repIntegracao = new Repositorio.Embarcador.NFS.NFSManualCTeIntegracao(unidadeDeTrabalho);

                Dominio.Entidades.Embarcador.NFS.NFSManualCTeIntegracao integracao = repIntegracao.BuscarPorCodigoArquivo(codigo);

                if (integracao == null)
                    return new JsonpResult(true, false, "Histórico não encontrado.");

                Dominio.Entidades.Embarcador.NFS.NFSManualIntegracaoArquivo arquivoIntegracao = integracao.ArquivosTransacao.Where(o => o.Codigo == codigo).FirstOrDefault();

                if (arquivoIntegracao == null || (arquivoIntegracao.ArquivoRequisicao == null && arquivoIntegracao.ArquivoResposta == null))
                    return new JsonpResult(true, false, "Não há registros de arquivos salvos para este histórico de consulta.");

                byte[] arquivo = Servicos.Embarcador.Integracao.ArquivoIntegracao.CriarZip(new List<Dominio.Entidades.Embarcador.Integracao.ArquivoIntegracao>() { arquivoIntegracao.ArquivoRequisicao, arquivoIntegracao.ArquivoResposta });

                return Arquivo(arquivo, "application/zip", "Arquivos Consulta CT-e " + integracao.LancamentoNFSManual.CTe.Numero + ".zip");
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);
                return new JsonpResult(false, false, "Ocorreu uma falha ao realizar o download da DACTE.");
            }
        }

        #endregion

        #region Métodos Privados

        private string ObterPropriedadeOrdenar(string propriedadeOrdenar)
        {
            if (propriedadeOrdenar == "CTe")
                return "LancamentoNFSManualCTe.CTe.Numero";

            if (propriedadeOrdenar == "Emissor")
                return "LancamentoNFSManualCTe.CTe.Empresa.CNPJ";

            if (propriedadeOrdenar == "TipoIntegracao")
                return "TipoIntegracao.Tipo";

            if (propriedadeOrdenar == "Tentativas")
                return "NumeroTentativas";

            if (propriedadeOrdenar == "DataEnvio")
                return "DataIntegracao";

            if (propriedadeOrdenar == "Situacao")
                return "SituacaoIntegracao";

            return propriedadeOrdenar;
        }

        #endregion
    }
}
