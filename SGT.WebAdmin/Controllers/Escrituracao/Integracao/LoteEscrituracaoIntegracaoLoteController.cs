using Dominio.ObjetosDeValor.Embarcador.Enumeradores;
using SGTAdmin.Controllers;
using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Mvc;

namespace SGT.WebAdmin.Controllers.Escrituracao.Integracao
{
    [CustomAuthorize(new string[] { "ConsultarHistoricoIntegracao", "DownloadArquivosHistoricoIntegracao", "ObterTotais" }, "Escrituracao/LoteEscrituracao")]
    public class LoteEscrituracaoIntegracaoLoteController : BaseController
    {
		#region Construtores

		public LoteEscrituracaoIntegracaoLoteController(Conexao conexao) : base(conexao) { }

		#endregion

        #region Métodos Globais

        public async Task<IActionResult> Pesquisa()
        {
            Repositorio.UnitOfWork unidadeDeTrabalho = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                int codigoLoteEscrituracao = Request.GetIntParam("LoteEscrituracao");
                TipoIntegracao? tipo = Request.GetNullableEnumParam<TipoIntegracao>("Tipo");
                SituacaoIntegracao? situacao = Request.GetNullableEnumParam<SituacaoIntegracao>("Situacao");

                Models.Grid.Grid grid = new Models.Grid.Grid(Request)
                {
                    header = new List<Models.Grid.Head>()
                };

                grid.AdicionarCabecalho("Codigo", false);
                grid.AdicionarCabecalho("Tipo", false);
                grid.AdicionarCabecalho("SituacaoIntegracao", false);
                grid.AdicionarCabecalho("Integração", "TipoIntegracao", 10, Models.Grid.Align.left, true);
                grid.AdicionarCabecalho("Tentativas", "Tentativas", 10, Models.Grid.Align.left, true);
                grid.AdicionarCabecalho("Data do Envio", "DataEnvio", 10, Models.Grid.Align.left, true);
                grid.AdicionarCabecalho("Situação", "Situacao", 10, Models.Grid.Align.left, true);
                grid.AdicionarCabecalho("Retorno", "Retorno", 20, Models.Grid.Align.left, false);

                Dominio.ObjetosDeValor.Embarcador.Consulta.ParametroConsulta parametrosConsulta = grid.ObterParametrosConsulta(ObterPropriedadeOrdenar);
                Repositorio.Embarcador.Escrituracao.LoteEscrituracaoIntegracao repositorioIntegracao = new Repositorio.Embarcador.Escrituracao.LoteEscrituracaoIntegracao(unidadeDeTrabalho);
                int totalRegistros = repositorioIntegracao.ContarConsulta(codigoLoteEscrituracao, situacao, tipo);
                List<Dominio.Entidades.Embarcador.Escrituracao.LoteEscrituracaoIntegracao> listaIntegracao = (totalRegistros > 0) ? repositorioIntegracao.Consultar(codigoLoteEscrituracao, situacao, tipo, parametrosConsulta) : new List<Dominio.Entidades.Embarcador.Escrituracao.LoteEscrituracaoIntegracao>();

                var listaIntegracaoRetornar = (
                    from o in listaIntegracao
                    select new
                    {
                        o.Codigo,
                        o.SituacaoIntegracao,
                        o.TipoIntegracao.Tipo,
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
                int codigoLoteEscrituracao;
                int.TryParse(Request.Params("LoteEscrituracao"), out codigoLoteEscrituracao);


                Repositorio.Embarcador.Escrituracao.LoteEscrituracaoIntegracao reploteEscrituracaoIntegracao = new Repositorio.Embarcador.Escrituracao.LoteEscrituracaoIntegracao(unidadeDeTrabalho);

                int totalAguardandoIntegracao = reploteEscrituracaoIntegracao.ContarPorLoteEscrituracao(codigoLoteEscrituracao, Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoIntegracao.AgIntegracao);
                int totalIntegrado = reploteEscrituracaoIntegracao.ContarPorLoteEscrituracao(codigoLoteEscrituracao, Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoIntegracao.Integrado);
                int totalProblemaIntegracao = reploteEscrituracaoIntegracao.ContarPorLoteEscrituracao(codigoLoteEscrituracao, Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoIntegracao.ProblemaIntegracao);
                int totalAguardandoRetorno = reploteEscrituracaoIntegracao.ContarPorLoteEscrituracao(codigoLoteEscrituracao, Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoIntegracao.AgRetorno);

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

                Repositorio.Embarcador.Escrituracao.LoteEscrituracaoIntegracao repLoteEscrituracaoIntegracao = new Repositorio.Embarcador.Escrituracao.LoteEscrituracaoIntegracao(unidadeDeTrabalho);

                Dominio.Entidades.Embarcador.Escrituracao.LoteEscrituracaoIntegracao loteEscrituracaoIntegracao = repLoteEscrituracaoIntegracao.BuscarPorCodigo(codigo);


                if (loteEscrituracaoIntegracao == null)
                    return new JsonpResult(false, "Integração não encontrada.");

                if (loteEscrituracaoIntegracao.TipoIntegracao.Tipo == Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoIntegracao.Natura)
                    return new JsonpResult(false, "Não é possível enviar as integrações da natura individualmente. Utilize a opção reenviar todos.");

                //List<AdminMultisoftware.Dominio.Enumeradores.PermissaoPersonalizada> permissoesPersonalizadas = ObterPermissoesPersonalizadas("LoteEscrituracaos/LoteEscrituracao", "Logistica/JanelaCarregamento");
                //if (!permissoesPersonalizadas.Contains(AdminMultisoftware.Dominio.Enumeradores.PermissaoPersonalizada.LoteEscrituracao_ReenviarIntegracoes) && cargaCTeIntegracao.SituacaoIntegracao != Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoIntegracao.ProblemaIntegracao)
                //    return new JsonpResult(false, true, "Você não possui permissões para executar esta ação.");

                loteEscrituracaoIntegracao.SituacaoIntegracao = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoIntegracao.AgIntegracao;
                Servicos.Auditoria.Auditoria.Auditar(Auditado, loteEscrituracaoIntegracao, null, "Reenviou a Integração.", unidadeDeTrabalho);

                repLoteEscrituracaoIntegracao.Atualizar(loteEscrituracaoIntegracao);

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
                Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoIntegracao? situacao = null;
                Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoIntegracao situacaoAux;
                if (Enum.TryParse(Request.Params("Situacao"), out situacaoAux))
                    situacao = situacaoAux;

                Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoIntegracao? tipo = null;
                Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoIntegracao tipoAux;
                if (Enum.TryParse(Request.Params("Tipo"), out tipoAux))
                    tipo = tipoAux;

                int codigoLoteEscrituracao = 0;
                int.TryParse(Request.Params("LoteEscrituracao"), out codigoLoteEscrituracao);

                Repositorio.Embarcador.Escrituracao.LoteEscrituracaoIntegracao repLoteEscrituracaoIntegracao = new Repositorio.Embarcador.Escrituracao.LoteEscrituracaoIntegracao(unidadeDeTrabalho);

                List<Dominio.Entidades.Embarcador.Escrituracao.LoteEscrituracaoIntegracao> integracoes = repLoteEscrituracaoIntegracao.Consultar(codigoLoteEscrituracao, situacao, tipo);

                bool utilizarTransacao = false;
                if (integracoes.Any(o => o.TipoIntegracao.Tipo == Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoIntegracao.Natura))
                    utilizarTransacao = true;

                if (utilizarTransacao)
                    unidadeDeTrabalho.Start();

                foreach (Dominio.Entidades.Embarcador.Escrituracao.LoteEscrituracaoIntegracao integracao in integracoes)
                {
                    if (!utilizarTransacao)
                    {
                        unidadeDeTrabalho.FlushAndClear();
                    }

                    integracao.SituacaoIntegracao = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoIntegracao.AgIntegracao;

                    repLoteEscrituracaoIntegracao.Atualizar(integracao);
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

                Repositorio.Embarcador.Escrituracao.LoteEscrituracaoIntegracao repIntegracao = new Repositorio.Embarcador.Escrituracao.LoteEscrituracaoIntegracao(unidadeDeTrabalho);

                Models.Grid.Grid grid = new Models.Grid.Grid(Request);

                grid.header = new List<Models.Grid.Head>();
                grid.AdicionarCabecalho("Codigo", false);
                grid.AdicionarCabecalho("Data", "Data", 20, Models.Grid.Align.left, false);
                grid.AdicionarCabecalho("Tipo", "DescricaoTipo", 20, Models.Grid.Align.left, false);
                grid.AdicionarCabecalho("Retorno", "Mensagem", 40, Models.Grid.Align.left, false);

                Dominio.Entidades.Embarcador.Escrituracao.LoteEscrituracaoIntegracao integracao = repIntegracao.BuscarPorCodigo(codigo);
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

                Repositorio.Embarcador.Escrituracao.LoteEscrituracaoIntegracao repIntegracao = new Repositorio.Embarcador.Escrituracao.LoteEscrituracaoIntegracao(unidadeDeTrabalho);

                Dominio.Entidades.Embarcador.Escrituracao.LoteEscrituracaoIntegracao integracao = repIntegracao.BuscarPorCodigoArquivo(codigo);

                if (integracao == null)
                    return new JsonpResult(true, false, "Histórico não encontrado.");

                Dominio.Entidades.Embarcador.Cargas.CargaCTeIntegracaoArquivo arquivoIntegracao = integracao.ArquivosTransacao.Where(o => o.Codigo == codigo).FirstOrDefault();

                if (arquivoIntegracao == null || (arquivoIntegracao.ArquivoRequisicao == null && arquivoIntegracao.ArquivoResposta == null))
                    return new JsonpResult(true, false, "Não há registros de arquivos salvos para este histórico de consulta.");

                byte[] arquivo = Servicos.Embarcador.Integracao.ArquivoIntegracao.CriarZip(new List<Dominio.Entidades.Embarcador.Integracao.ArquivoIntegracao>() { arquivoIntegracao.ArquivoRequisicao, arquivoIntegracao.ArquivoResposta });

                return Arquivo(arquivo, "application/zip", "Arquivos Consulta Lote" + integracao.LoteEscrituracao.Numero + ".zip");
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);
                return new JsonpResult(false, false, "Ocorreu uma falha ao realizar o download do arquivo.");
            }
        }

        #endregion

        #region Métodos Privados

        private string ObterPropriedadeOrdenar(string propriedadeOrdenar)
        {

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
