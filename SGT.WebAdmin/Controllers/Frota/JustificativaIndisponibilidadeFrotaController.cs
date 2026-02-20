using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Mvc;
using Dominio.ObjetosDeValor.Embarcador.Enumeradores;
using SGTAdmin.Controllers;

namespace SGT.WebAdmin.Controllers.Frota
{
    [CustomAuthorize("Frota/JustificativaDeIndisponibilidade")]
    public class JustificativaIndisponibilidadeFrotaController : BaseController
    {
		#region Construtores

		public JustificativaIndisponibilidadeFrotaController(Conexao conexao) : base(conexao) { }

		#endregion

        [AllowAuthenticate]
        public async Task<IActionResult> Pesquisa()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);
            try
            {
                //Dominio.ObjetosDeValor.Embarcador.Ocorrencia.FiltroPesquisaJustificativaOcorrencia filtrosPesquisa = ObterFiltrosPesquisa();
                var descricao = Request.GetStringParam("Descricao");
                var status = Request.GetEnumParam("Ativo", SituacaoAtivoPesquisa.Ativo);

                Models.Grid.Grid grid = new Models.Grid.Grid(Request);
                grid.header = new List<Models.Grid.Head>();

                grid.AdicionarCabecalho("Codigo", false);
                grid.AdicionarCabecalho("Descrição", "Descricao", 40, Models.Grid.Align.left, true);

                //if (status == SituacaoAtivoPesquisa.Todos)
                grid.AdicionarCabecalho("Status", "DescricaoAtivo", 15, Models.Grid.Align.center, false);

                var repoJustificativaIndisponibilidadeFrota = new Repositorio.Embarcador.Frotas.JustificativaDeIndisponibilidadeDeFrota(unitOfWork);
                
                Dominio.ObjetosDeValor.Embarcador.Consulta.ParametroConsulta parametrosConsulta = grid.ObterParametrosConsulta();

                var listaConsulta = repoJustificativaIndisponibilidadeFrota.Consultar(descricao, status, parametrosConsulta);
                var quantidadeConsulta = repoJustificativaIndisponibilidadeFrota.ContarConsulta(descricao, status);

                grid.setarQuantidadeTotal(quantidadeConsulta);

                var lista = (from p in listaConsulta
                             select new
                             {
                                 p.Codigo,
                                 p.Descricao,
                                 p.DescricaoAtivo
                             }).ToList();

                grid.AdicionaRows(lista);
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

        public async Task<IActionResult> Salvar()
        {
            var unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);
            try
            {
                var descricao = Request.GetStringParam("Descricao");
                var ativo = Request.GetBoolParam("Ativo");
                var codigo = Request.GetIntParam("Codigo");

                var repositorio = new Repositorio.Embarcador.Frotas.JustificativaDeIndisponibilidadeDeFrota(unitOfWork);
                Dominio.Entidades.Embarcador.Frotas.JustificativaDeIndisponibilidadeDeFrota entidade = null;

                if (codigo > 0)
                    entidade = repositorio.BuscarPorCodigo(codigo, false);
                else
                    entidade = new Dominio.Entidades.Embarcador.Frotas.JustificativaDeIndisponibilidadeDeFrota();
                
                entidade.Descricao = descricao;
                entidade.Ativo = ativo;

                if(entidade.Codigo > 0)
                    repositorio.Atualizar(entidade, Auditado);
                else
                    repositorio.Inserir(entidade, Auditado);

                //Servicos.Auditoria.Auditoria.Auditar(Auditado, entidade, null, "MENSAGEM", unitOfWork);

                return new JsonpResult(true);

            }
            catch(Exception e)
            {
                return new JsonpResult(false, e.Message);
            }
        }

        [AllowAuthenticate]
        public async Task<IActionResult> BuscarPorCodigo()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                int codigo = Request.GetIntParam("Codigo");

                var repositorio = new Repositorio.Embarcador.Frotas.JustificativaDeIndisponibilidadeDeFrota(unitOfWork);
                Dominio.Entidades.Embarcador.Frotas.JustificativaDeIndisponibilidadeDeFrota entidade = repositorio.BuscarPorCodigo(codigo, false);

                if (entidade == null)
                    return new JsonpResult(false, true, "Não foi possível encontrar o registro.");

                var dynamicObj = new
                {
                    entidade.Codigo,
                    entidade.Descricao,
                    entidade.Ativo
                };

                return new JsonpResult(dynamicObj);
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

        public async Task<IActionResult> ExcluirPorCodigo()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                unitOfWork.Start();

                int codigo = Request.GetIntParam("Codigo");

                var repositorio = new Repositorio.Embarcador.Frotas.JustificativaDeIndisponibilidadeDeFrota(unitOfWork);
                Dominio.Entidades.Embarcador.Frotas.JustificativaDeIndisponibilidadeDeFrota entidade = repositorio.BuscarPorCodigo(codigo, true);

                if (entidade == null)
                    return new JsonpResult(false, true, "Não foi possível encontrar o registro.");

                repositorio.Deletar(entidade, Auditado);

                //Servicos.Auditoria.Auditoria.Auditar(Auditado, entidade, null, "Excluiu", unitOfWork);

                unitOfWork.CommitChanges();

                return new JsonpResult(true);
            }
            catch (Exception ex)
            {
                unitOfWork.Rollback();
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

    }
}
