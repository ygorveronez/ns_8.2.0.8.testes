using SGTAdmin.Controllers;
using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Mvc;

namespace SGT.WebAdmin.Controllers.Ocorrencias
{
    [CustomAuthorize("Ocorrencias/ParametroOcorrencia")]
    public class ParametroOcorrenciaController : BaseController
    {
		#region Construtores

		public ParametroOcorrenciaController(Conexao conexao) : base(conexao) { }

		#endregion

        #region Métodos Globais

        [AllowAuthenticate]
        public async Task<IActionResult> Pesquisa()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                Dominio.ObjetosDeValor.Embarcador.Ocorrencia.FiltroPesquisaParametroOcorrencia filtrosPesquisa = ObterFiltrosPesquisa();

                Models.Grid.Grid grid = new Models.Grid.Grid(Request)
                {
                    header = new List<Models.Grid.Head>()
                };

                grid.AdicionarCabecalho("Codigo", false);
                grid.AdicionarCabecalho("TipoParametro", false);
                grid.AdicionarCabecalho("Descrição", "Descricao", 50, Models.Grid.Align.left, true);
                grid.AdicionarCabecalho("Tipo", "DescricaoTipo", 15, Models.Grid.Align.left, true);

                if (filtrosPesquisa.SituacaoAtivo == Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoAtivoPesquisa.Todos)
                    grid.AdicionarCabecalho("Situação", "DescricaoAtivo", 10, Models.Grid.Align.center, false);

                Dominio.ObjetosDeValor.Embarcador.Consulta.ParametroConsulta parametrosConsulta = grid.ObterParametrosConsulta(ObterPropriedadeOrdenar);
                Repositorio.Embarcador.Ocorrencias.ParametroOcorrencia repositorioParametroOcorrencia = new Repositorio.Embarcador.Ocorrencias.ParametroOcorrencia(unitOfWork);
                int quantidadeTotal = repositorioParametroOcorrencia.ContarConsulta(filtrosPesquisa);
                List<Dominio.Entidades.Embarcador.Ocorrencias.ParametroOcorrencia> listaParametrosOcorrencia = (quantidadeTotal > 0) ? repositorioParametroOcorrencia.Consultar(filtrosPesquisa, parametrosConsulta) : new List<Dominio.Entidades.Embarcador.Ocorrencias.ParametroOcorrencia>();

                var listaParametrosOcorrenciaRetornar = (
                    from parametro in listaParametrosOcorrencia
                    select new
                    {
                        parametro.Codigo,
                        parametro.TipoParametro,
                        parametro.Descricao,
                        parametro.DescricaoAtivo,
                        DescricaoTipo = parametro.DescricaoTipoParametro
                    }
                ).ToList();

                grid.AdicionaRows(listaParametrosOcorrenciaRetornar);
                grid.setarQuantidadeTotal(quantidadeTotal);

                return new JsonpResult(grid);
            }
            catch (Exception excecao)
            {
                Servicos.Log.TratarErro(excecao);
                return new JsonpResult(false, "Ocorreu uma falha ao consultar.");
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
                int codigo = int.Parse(Request.Params("Codigo"));
                Repositorio.Embarcador.Ocorrencias.ParametroOcorrencia repParametroOcorrencia = new Repositorio.Embarcador.Ocorrencias.ParametroOcorrencia(unitOfWork);
                Dominio.Entidades.Embarcador.Ocorrencias.ParametroOcorrencia parametro = repParametroOcorrencia.BuscarPorCodigo(codigo);
                var dynProcessoMovimento = new
                {
                    parametro.Codigo,
                    parametro.Descricao,
                    parametro.DescricaoParametro,
                    parametro.DescricaoParametroFinal,
                    Tipo = parametro.TipoParametro,
                    parametro.Ativo
                };
                return new JsonpResult(dynProcessoMovimento);
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);
                return new JsonpResult(false, "Ocorreu uma falha ao buscar por código.");
            }
            finally
            {
                unitOfWork.Dispose();
            }

        }

        public async Task<IActionResult> Adicionar()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);
            try
            {

                unitOfWork.Start();

                Repositorio.Embarcador.Ocorrencias.ParametroOcorrencia repParametroOcorrencia = new Repositorio.Embarcador.Ocorrencias.ParametroOcorrencia(unitOfWork);

                Dominio.Entidades.Embarcador.Ocorrencias.ParametroOcorrencia parametroOcorrencia = new Dominio.Entidades.Embarcador.Ocorrencias.ParametroOcorrencia();
                bool ativo;
                bool.TryParse(Request.Params("Ativo"), out ativo);

                Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoParametroOcorrencia tipoParametro;
                Enum.TryParse(Request.Params("tipo"), out tipoParametro);

                parametroOcorrencia.Descricao = Request.Params("Descricao");
                parametroOcorrencia.DescricaoParametro = Request.GetStringParam("DescricaoParametro");
                parametroOcorrencia.DescricaoParametroFinal = Request.GetStringParam("DescricaoParametroFinal");
                parametroOcorrencia.Ativo = ativo;
                parametroOcorrencia.TipoParametro = tipoParametro;

                repParametroOcorrencia.Inserir(parametroOcorrencia, Auditado);
                unitOfWork.CommitChanges();
                return new JsonpResult(true);
            }
            catch (Exception ex)
            {
                unitOfWork.Rollback();
                Servicos.Log.TratarErro(ex);
                return new JsonpResult(false, "Ocorreu uma falha ao adicionar.");
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
                unitOfWork.Start();
                Repositorio.Embarcador.Ocorrencias.ParametroOcorrencia repParametroOcorrencia = new Repositorio.Embarcador.Ocorrencias.ParametroOcorrencia(unitOfWork);
                Dominio.Entidades.Embarcador.Ocorrencias.ParametroOcorrencia parametroOcorrencia = repParametroOcorrencia.BuscarPorCodigo(int.Parse(Request.Params("Codigo")), true);

                bool ativo;
                bool.TryParse(Request.Params("Ativo"), out ativo);

                Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoParametroOcorrencia tipoParametro;
                Enum.TryParse(Request.Params("tipo"), out tipoParametro);

                parametroOcorrencia.Descricao = Request.Params("Descricao");
                parametroOcorrencia.DescricaoParametro = Request.GetStringParam("DescricaoParametro");
                parametroOcorrencia.DescricaoParametroFinal = Request.GetStringParam("DescricaoParametroFinal");
                parametroOcorrencia.Ativo = ativo;
                parametroOcorrencia.TipoParametro = tipoParametro;

                repParametroOcorrencia.Atualizar(parametroOcorrencia, Auditado);
                unitOfWork.CommitChanges();

                return new JsonpResult(true);
            }
            catch (Exception ex)
            {
                unitOfWork.Rollback();
                Servicos.Log.TratarErro(ex);
                return new JsonpResult(false, "Ocorreu uma falha ao atualizar.");
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        public async Task<IActionResult> ExcluirPorCodigo()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);
            try
            {
                int codigo = int.Parse(Request.Params("codigo"));
                Repositorio.Embarcador.Ocorrencias.ParametroOcorrencia repParametroOcorrencia = new Repositorio.Embarcador.Ocorrencias.ParametroOcorrencia(unitOfWork);
                Dominio.Entidades.Embarcador.Ocorrencias.ParametroOcorrencia parametroOcorrencia = repParametroOcorrencia.BuscarPorCodigo(codigo);
                repParametroOcorrencia.Deletar(parametroOcorrencia, Auditado);
                return new JsonpResult(true);
            }
            catch (Exception ex)
            {
                if (ExcessaoPorPossuirDependeciasNoBanco(ex))
                    return new JsonpResult(false, true, "Não foi possível excluir o registro pois o mesmo já possui vínculo com outros recursos do sistema, recomendamos que você inative o registro caso não deseja mais utilizá-lo.");
                else
                {
                    Servicos.Log.TratarErro(ex);
                    return new JsonpResult(false, "Ocorreu uma falha ao excluir.");
                }
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        #endregion

        #region Métodos Privados

        private Dominio.ObjetosDeValor.Embarcador.Ocorrencia.FiltroPesquisaParametroOcorrencia ObterFiltrosPesquisa()
        {
            return new Dominio.ObjetosDeValor.Embarcador.Ocorrencia.FiltroPesquisaParametroOcorrencia()
            {
                Descricao = Request.GetStringParam("Descricao"),
                SituacaoAtivo = Request.GetEnumParam("Ativo", Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoAtivoPesquisa.Ativo),
                Tipo = Request.GetNullableEnumParam<Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoParametroOcorrencia>("Tipo"),
                FiltrarParametrosPeriodo = Request.GetBoolParam("FiltrarParametrosPeriodo")
            };
        }

        private string ObterPropriedadeOrdenar(string propriedadeOrdenar)
        {
            if (propriedadeOrdenar == "DescricaoTipo")
                return "Tipo";

            return propriedadeOrdenar;
        }

        #endregion
    }
}
