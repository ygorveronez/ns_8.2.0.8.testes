using Dominio.Excecoes.Embarcador;
using Dominio.ObjetosDeValor.Embarcador.Enumeradores;
using SGTAdmin.Controllers;
using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Mvc;

namespace SGT.WebAdmin.Controllers.Ocorrencia
{
    [CustomAuthorize("Ocorrencias/GrupoTipoDeOcorrenciaDeCTe", "Ocorrencias/TipoOcorrencia")]
    public class GrupoTipoDeOcorrenciaDeCTeController : BaseController
    {
		#region Construtores

		public GrupoTipoDeOcorrenciaDeCTeController(Conexao conexao) : base(conexao) { }

		#endregion

        public async Task<IActionResult> Adicionar()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);
            try
            {
                Repositorio.Embarcador.Ocorrencias.GrupoTipoDeOcorrenciaDeCTe repositorio = new Repositorio.Embarcador.Ocorrencias.GrupoTipoDeOcorrenciaDeCTe(unitOfWork);

                Dominio.Entidades.Embarcador.Ocorrencias.GrupoTipoDeOcorrenciaDeCTe entidade = new Dominio.Entidades.Embarcador.Ocorrencias.GrupoTipoDeOcorrenciaDeCTe();
                PreencherEntidade(entidade);
                
                repositorio.Inserir(entidade);

                return new JsonpResult(true, true, "Registro inserido com sucesso.");
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);
                unitOfWork.Rollback();
                return new JsonpResult(false, "Ocorreu uma falha ao realizar a consulta.");
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
                Repositorio.Embarcador.Ocorrencias.GrupoTipoDeOcorrenciaDeCTe repositorio = new Repositorio.Embarcador.Ocorrencias.GrupoTipoDeOcorrenciaDeCTe(unitOfWork);

                Dominio.Entidades.Embarcador.Ocorrencias.GrupoTipoDeOcorrenciaDeCTe entidade = BuscarEntidade(unitOfWork);
                PreencherEntidade(entidade);

                repositorio.Atualizar(entidade);
                
                return new JsonpResult(true, true, "Registro atualizado com sucesso.");
            }
            catch (BaseException ex)
            {
                unitOfWork.Rollback();
                return new JsonpResult(false, ex.Message);
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);
                unitOfWork.Rollback();
                return new JsonpResult(false, "Ocorreu uma falha ao realizar a consulta.");
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        private void PreencherEntidade(Dominio.Entidades.Embarcador.Ocorrencias.GrupoTipoDeOcorrenciaDeCTe grupoOcorrencia)
        {
            grupoOcorrencia.Descricao = Request.GetStringParam("Descricao");
            grupoOcorrencia.CodigoIntegracao = Request.GetStringParam("CodigoIntegracao");
            grupoOcorrencia.Ativo = Request.GetBoolParam("Situacao");
        }

        public async Task<IActionResult> BuscarPorCodigo()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);
            try
            {
                Dominio.Entidades.Embarcador.Ocorrencias.GrupoTipoDeOcorrenciaDeCTe entidade = BuscarEntidade(unitOfWork);
                
                return new JsonpResult(new
                {
                    entidade.Codigo,
                    Situacao = entidade.Ativo,
                    entidade.CodigoIntegracao,
                    entidade.Descricao
                });
            }
            catch(BaseException ex)
            {
                return new JsonpResult(false, ex.Message);
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);
                return new JsonpResult(false, "Ocorreu uma falha ao realizar a consulta.");
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        private Dominio.Entidades.Embarcador.Ocorrencias.GrupoTipoDeOcorrenciaDeCTe BuscarEntidade(Repositorio.UnitOfWork unitOfWork)
        {
            Repositorio.Embarcador.Ocorrencias.GrupoTipoDeOcorrenciaDeCTe repositorio = new Repositorio.Embarcador.Ocorrencias.GrupoTipoDeOcorrenciaDeCTe(unitOfWork);
            int codigo = Request.GetIntParam("Codigo");

            Dominio.Entidades.Embarcador.Ocorrencias.GrupoTipoDeOcorrenciaDeCTe entidade = repositorio.BuscarPorCodigo(codigo, false);

            if (entidade == null)
                throw new ControllerException("O registro não foi encontrado.");

            return entidade;
        }

        public async Task<IActionResult> Pesquisa()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);
            try
            {
                Models.Grid.Grid grid = ObterGridPesquisa();
                PreencherGridPesquisa(grid, unitOfWork);

                return new JsonpResult(grid);
            }
            catch(Exception ex)
            {
                Servicos.Log.TratarErro(ex);
                return new JsonpResult(false, "Ocorreu uma falha ao realizar a consulta.");
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }
        
        private Models.Grid.Grid ObterGridPesquisa()
        {
            Models.Grid.Grid grid = new Models.Grid.Grid(Request);

            grid.header = new List<Models.Grid.Head>();

            grid.AdicionarCabecalho("Codigo", false);
            grid.AdicionarCabecalho("Código Integração", "CodigoIntegracao", 1.5m, Models.Grid.Align.left, false, false, false, false, true);
            grid.AdicionarCabecalho("Descrição", "Descricao", 1.5m, Models.Grid.Align.left, false, false, false, false, true);
            grid.AdicionarCabecalho("Situação", "DescricaoAtivo", 1.5m, Models.Grid.Align.left, false, false, false, false, true);
            
            return grid;
        }

        private void PreencherGridPesquisa(Models.Grid.Grid grid, Repositorio.UnitOfWork unitOfWork)
        {
            Repositorio.Embarcador.Ocorrencias.GrupoTipoDeOcorrenciaDeCTe repositorio = new Repositorio.Embarcador.Ocorrencias.GrupoTipoDeOcorrenciaDeCTe(unitOfWork);

            Dominio.ObjetosDeValor.Embarcador.Ocorrencia.FiltroPesquisaGrupoTipoDeOcorrenciaDeCTe filtrosPesquisa = ObterFiltrosPesquisa();
            Dominio.ObjetosDeValor.Embarcador.Consulta.ParametroConsulta parametrosConsulta = grid.ObterParametrosConsulta();

            int contagemRegistros = repositorio.ContarConsulta(filtrosPesquisa);
            List<Dominio.Entidades.Embarcador.Ocorrencias.GrupoTipoDeOcorrenciaDeCTe> registros = contagemRegistros > 0 ? repositorio.Consultar(filtrosPesquisa, parametrosConsulta) : new List<Dominio.Entidades.Embarcador.Ocorrencias.GrupoTipoDeOcorrenciaDeCTe>();

            var registrosGrid = (from obj in registros
                                 select new
                                 {
                                     obj.Codigo,
                                     obj.CodigoIntegracao,
                                     obj.Descricao,
                                     obj.DescricaoAtivo
                                 }).ToList();

            grid.AdicionaRows(registrosGrid);
            grid.setarQuantidadeTotal(contagemRegistros);
        }

        private Dominio.ObjetosDeValor.Embarcador.Ocorrencia.FiltroPesquisaGrupoTipoDeOcorrenciaDeCTe ObterFiltrosPesquisa()
        {
            return new Dominio.ObjetosDeValor.Embarcador.Ocorrencia.FiltroPesquisaGrupoTipoDeOcorrenciaDeCTe()
            {
                Descricao = Request.GetStringParam("Descricao"),
                CodigoIntegracao = Request.GetStringParam("CodigoIntegracao"),
                SituacaoAtivo = Request.GetEnumParam<SituacaoAtivaPesquisa>("Situacao")
            };
        }
    }
}
