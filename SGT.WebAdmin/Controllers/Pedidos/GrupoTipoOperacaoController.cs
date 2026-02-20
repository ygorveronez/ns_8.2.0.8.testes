using Dominio.ObjetosDeValor.Embarcador.Enumeradores;
using SGTAdmin.Controllers;
using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Mvc;

namespace SGT.WebAdmin.Controllers.Pedidos
{

    [CustomAuthorize("Pedidos/GrupoTipoOperacao")]
    public class GrupoTipoOperacaoController : BaseController
    {
		#region Construtores

		public GrupoTipoOperacaoController(Conexao conexao) : base(conexao) { }

		#endregion


        private const string CADASTRO_DESATIVADO = "Cadastro de grupo de tipo de operação desativado";

        #region Métodos Globais

        [AllowAuthenticate]
        public async Task<IActionResult> Pesquisa()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);
            try
            {
                if (!VerificarGrupoTipoOperacaoAtivado(unitOfWork)) return new JsonpResult(false, CADASTRO_DESATIVADO);

                Models.Grid.Grid grid = new Models.Grid.Grid(Request);
                grid.header = new List<Models.Grid.Head>();
                grid.AdicionarCabecalho("Codigo", false);
                grid.AdicionarCabecalho("Ordem", "Ordem", 5, Models.Grid.Align.right, true);
                grid.AdicionarCabecalho("Descrição", "Descricao", 60, Models.Grid.Align.left, true);
                grid.AdicionarCabecalho("Situação", "DescricaoAtivo", 35, Models.Grid.Align.center, false);

                Dominio.ObjetosDeValor.Embarcador.Consulta.ParametroConsulta parametrosConsulta = grid.ObterParametrosConsulta();
                Repositorio.Embarcador.Pedidos.GrupoTipoOperacao repGrupoTipoOperacao = new Repositorio.Embarcador.Pedidos.GrupoTipoOperacao(unitOfWork);

                Dominio.ObjetosDeValor.Embarcador.Pedido.FiltroPesquisaGrupoTipoOperacao filtrosPesquisa = ObterFiltrosPesquisa();
                int totalRegistros = repGrupoTipoOperacao.ContarConsulta(filtrosPesquisa);
                List<Dominio.Entidades.Embarcador.Pedidos.GrupoTipoOperacao> lista = totalRegistros > 0 ? repGrupoTipoOperacao.Consultar(filtrosPesquisa, parametrosConsulta) : new List<Dominio.Entidades.Embarcador.Pedidos.GrupoTipoOperacao>();

                var retorno = (
                    from obj in lista
                    select new
                    {
                        obj.Codigo,
                        obj.Ordem,
                        obj.Descricao,
                        obj.DescricaoAtivo
                    }
                ).ToList();

                grid.AdicionaRows(retorno);
                grid.setarQuantidadeTotal(totalRegistros);

                return new JsonpResult(grid);
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);
                return new JsonpResult(false, "Ocorreu uma falha ao consultar os grupos de tipos de operação.");
            }
        }

        public async Task<IActionResult> Adicionar()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);
            try
            {
                if (!VerificarGrupoTipoOperacaoAtivado(unitOfWork)) return new JsonpResult(false, CADASTRO_DESATIVADO);

                string descricao = Request.GetStringParam("Descricao");
                int ordem = Request.GetIntParam("Ordem");
                bool ativo = Request.GetBoolParam("Ativo");
                string cor = Request.GetStringParam("Cor");

                Dominio.Entidades.Embarcador.Pedidos.GrupoTipoOperacao grupoTipoOperacao = new Dominio.Entidades.Embarcador.Pedidos.GrupoTipoOperacao()
                {
                    Descricao = descricao,
                    Ordem = ordem,
                    Cor = cor,
                    Ativo = ativo,
                    DataCadastro = DateTime.Now
                };

                Repositorio.Embarcador.Pedidos.GrupoTipoOperacao repGrupoTipoOperacao = new Repositorio.Embarcador.Pedidos.GrupoTipoOperacao(unitOfWork);
                unitOfWork.Start();
                repGrupoTipoOperacao.Inserir(grupoTipoOperacao, Auditado);
                unitOfWork.CommitChanges();
                return new JsonpResult(true);
                
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);
                unitOfWork.Rollback();
                return new JsonpResult(false, "Ocorreu uma falha ao adicionar o grupo de tipo de operação.");
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        public async Task<IActionResult> Atualizar()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                if (!VerificarGrupoTipoOperacaoAtivado(unitOfWork)) return new JsonpResult(false, CADASTRO_DESATIVADO);

                int codigo = Request.GetIntParam("Codigo");
                string descricao = Request.GetStringParam("Descricao");
                int ordem = Request.GetIntParam("Ordem");
                string cor = Request.GetStringParam("Cor");
                bool ativo = Request.GetBoolParam("Ativo");

                Repositorio.Embarcador.Pedidos.GrupoTipoOperacao repGrupoTipoOperacao = new Repositorio.Embarcador.Pedidos.GrupoTipoOperacao(unitOfWork);
                Dominio.Entidades.Embarcador.Pedidos.GrupoTipoOperacao grupoTipoOperacao = repGrupoTipoOperacao.BuscarPorCodigo(codigo, true);
                if (grupoTipoOperacao == null)
                    return new JsonpResult(false, "Registro não encontrado.");

                unitOfWork.Start();
                grupoTipoOperacao.Descricao = descricao;
                grupoTipoOperacao.Ordem = ordem;
                grupoTipoOperacao.Cor = cor;
                grupoTipoOperacao.Ativo = ativo;
                repGrupoTipoOperacao.Atualizar(grupoTipoOperacao, Auditado);
                unitOfWork.CommitChanges();
                return new JsonpResult(true);

            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);
                unitOfWork.Rollback();
                return new JsonpResult(false, "Ocorreu uma falha ao atualizar o grupo de tipo de operação.");
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        public async Task<IActionResult> BuscarPorCodigo()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);
            try
            {
                if (!VerificarGrupoTipoOperacaoAtivado(unitOfWork)) return new JsonpResult(false, CADASTRO_DESATIVADO);

                int codigo = Request.GetIntParam("Codigo");
                Repositorio.Embarcador.Pedidos.GrupoTipoOperacao repGrupoTipoOperacao = new Repositorio.Embarcador.Pedidos.GrupoTipoOperacao(unitOfWork);
                Dominio.Entidades.Embarcador.Pedidos.GrupoTipoOperacao grupoTipoOperacao = repGrupoTipoOperacao.BuscarPorCodigo(codigo, true);
                if (grupoTipoOperacao == null)
                    return new JsonpResult(false, "Registro não encontrado.");

                var retorno = new
                {
                    grupoTipoOperacao.Codigo,
                    grupoTipoOperacao.Descricao,
                    grupoTipoOperacao.Ordem,
                    grupoTipoOperacao.Cor,
                    grupoTipoOperacao.Ativo,
                };
                unitOfWork.Dispose();
                return new JsonpResult(retorno);
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);
                unitOfWork.Dispose();
                return new JsonpResult(false, "Ocorreu uma falha ao buscar o grupo de tipo de operação por código.");
            }
        }

        public async Task<IActionResult> ExcluirPorCodigo()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                if (!VerificarGrupoTipoOperacaoAtivado(unitOfWork)) return new JsonpResult(false, CADASTRO_DESATIVADO);

                int codigo = Request.GetIntParam("Codigo");
                Repositorio.Embarcador.Pedidos.GrupoTipoOperacao repGrupoTipoOperacao = new Repositorio.Embarcador.Pedidos.GrupoTipoOperacao(unitOfWork);
                Dominio.Entidades.Embarcador.Pedidos.GrupoTipoOperacao grupoTipoOperacao = repGrupoTipoOperacao.BuscarPorCodigo(codigo, true);
                if (grupoTipoOperacao == null)
                    return new JsonpResult(false, "Registro não encontrado.");

                unitOfWork.Start();
                repGrupoTipoOperacao.Deletar(grupoTipoOperacao, Auditado);
                unitOfWork.CommitChanges();
                unitOfWork.Dispose();
                return new JsonpResult(true);
            }
            catch (Exception ex)
            {
                unitOfWork.Dispose();
                if (ExcessaoPorPossuirDependeciasNoBanco(ex))
                    return new JsonpResult(false, true, "Não foi possível excluir o registro pois o mesmo já possui vínculo com outros recursos do sistema, recomendamos que você inative o registro caso não deseja mais utilizá-lo.");
                else
                {
                    Servicos.Log.TratarErro(ex);
                    return new JsonpResult(false, "Ocorreu uma falha ao excluir o grupo de tipo de operação.");
                }
            }
        }

        [AllowAuthenticate]
        public async Task<IActionResult> BuscarTodos()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);
            try
            {
                if (!VerificarGrupoTipoOperacaoAtivado(unitOfWork)) return new JsonpResult(false, CADASTRO_DESATIVADO);

                Repositorio.Embarcador.Pedidos.GrupoTipoOperacao repGrupoTipoOperacao = new Repositorio.Embarcador.Pedidos.GrupoTipoOperacao(unitOfWork);
                List<Dominio.Entidades.Embarcador.Pedidos.GrupoTipoOperacao> rows = repGrupoTipoOperacao.BuscarAtivos();

                var result = (from r in rows
                              select new
                              {
                                  value = r.Codigo,
                                  text = r.Descricao,
                                  selected = "selected"
                              }).ToList();

                result.Add(new
                {
                    value = -1,
                    text = "Nenhum",
                    selected = "selected"
                });

                return new JsonpResult(new
                {
                    GrupoTipoOperacao = result
                });
            }
            catch (Exception excecao)
            {
                Servicos.Log.TratarErro(excecao);
                return new JsonpResult(false, "Ocorreu uma falha ao consultar todos os grupos de tipo de operação.");
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        #endregion

        #region Métodos Privados

        private Dominio.ObjetosDeValor.Embarcador.Pedido.FiltroPesquisaGrupoTipoOperacao ObterFiltrosPesquisa()
        {
            Dominio.ObjetosDeValor.Embarcador.Pedido.FiltroPesquisaGrupoTipoOperacao filtrosPesquisa = new Dominio.ObjetosDeValor.Embarcador.Pedido.FiltroPesquisaGrupoTipoOperacao()
            {
                Ativo = Request.GetEnumParam("Ativo", SituacaoAtivoPesquisa.Ativo),
                Descricao = Request.GetStringParam("Descricao")
            };
            return filtrosPesquisa;
        }

        private bool VerificarGrupoTipoOperacaoAtivado(Repositorio.UnitOfWork unitOfWork)
        {
            Repositorio.Embarcador.Configuracao.ConfiguracaoTMS repConfiguracaoTMS = new Repositorio.Embarcador.Configuracao.ConfiguracaoTMS(unitOfWork);
            Dominio.Entidades.Embarcador.Configuracao.ConfiguracaoTMS configuracao = repConfiguracaoTMS.BuscarConfiguracaoPadrao();
            return configuracao.UsarGrupoDeTipoDeOperacaoNoMonitoramento;
        }

        #endregion
    }
}
