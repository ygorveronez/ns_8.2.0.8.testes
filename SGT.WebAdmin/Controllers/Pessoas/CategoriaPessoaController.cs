using Dominio.Excecoes.Embarcador;
using SGTAdmin.Controllers;
using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Mvc;

namespace SGT.WebAdmin.Controllers.Pessoas
{
    [CustomAuthorize("Pessoas/CategoriaPessoa")]
    public class CategoriaPessoaController : BaseController
    {
		#region Construtores

		public CategoriaPessoaController(Conexao conexao) : base(conexao) { }

		#endregion

        #region Métodos Globais

        public async Task<IActionResult> Adicionar()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                Dominio.Entidades.Embarcador.Pessoas.CategoriaPessoa categoriaPessoa = new Dominio.Entidades.Embarcador.Pessoas.CategoriaPessoa();

                PreencherCategoriaPessoa(categoriaPessoa);

                unitOfWork.Start();

                new Repositorio.Embarcador.Pessoas.CategoriaPessoa(unitOfWork).Inserir(categoriaPessoa, Auditado);

                unitOfWork.CommitChanges();

                return new JsonpResult(true);
            }
            catch (ControllerException excecao)
            {
                return new JsonpResult(false, true, excecao.Message);
            }
            catch (Exception excecao)
            {
                unitOfWork.Rollback();
                Servicos.Log.TratarErro(excecao);
                return new JsonpResult(false, "Ocorreu uma falha ao adicionar dados.");
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
                int codigo = Request.GetIntParam("Codigo");
                Repositorio.Embarcador.Pessoas.CategoriaPessoa repositorio = new Repositorio.Embarcador.Pessoas.CategoriaPessoa(unitOfWork);
                Dominio.Entidades.Embarcador.Pessoas.CategoriaPessoa categoriaPessoa = repositorio.BuscarPorCodigo(codigo, auditavel: true);

                if (categoriaPessoa == null)
                    return new JsonpResult(false, true, "Não foi possível encontrar o registro.");

                PreencherCategoriaPessoa(categoriaPessoa);

                unitOfWork.Start();

                Dominio.Entidades.Auditoria.HistoricoObjeto historicoObjeto = repositorio.Atualizar(categoriaPessoa, Auditado);

                unitOfWork.CommitChanges();

                return new JsonpResult(true);
            }
            catch (ControllerException excecao)
            {
                return new JsonpResult(false, true, excecao.Message);
            }
            catch (Exception excecao)
            {
                unitOfWork.Rollback();
                Servicos.Log.TratarErro(excecao);
                return new JsonpResult(false, "Ocorreu uma falha ao atualizar dados.");
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
                int codigo = Request.GetIntParam("Codigo");
                Repositorio.Embarcador.Pessoas.CategoriaPessoa repositorio = new Repositorio.Embarcador.Pessoas.CategoriaPessoa(unitOfWork);
                Dominio.Entidades.Embarcador.Pessoas.CategoriaPessoa categoriaPessoa = repositorio.BuscarPorCodigo(codigo);

                if (categoriaPessoa == null)
                    return new JsonpResult(false, true, "Não foi possível encontrar o registro.");

                return new JsonpResult(new
                {
                    categoriaPessoa.Codigo,
                    categoriaPessoa.Descricao,
                    categoriaPessoa.CodigoIntegracao,
                    categoriaPessoa.Observacao,
                    categoriaPessoa.Cor
                });
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

        public async Task<IActionResult> ExcluirPorCodigo()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                int codigo = Request.GetIntParam("Codigo");
                Repositorio.Embarcador.Pessoas.CategoriaPessoa repositorio = new Repositorio.Embarcador.Pessoas.CategoriaPessoa(unitOfWork);
                Dominio.Entidades.Embarcador.Pessoas.CategoriaPessoa categoriaPessoa = repositorio.BuscarPorCodigo(codigo, auditavel: true);

                if (categoriaPessoa == null)
                    return new JsonpResult(false, true, "Não foi possível encontrar o registro.");

                unitOfWork.Start();

                repositorio.Deletar(categoriaPessoa, Auditado);

                unitOfWork.CommitChanges();

                return new JsonpResult(true);
            }
            catch (Exception excecao)
            {
                unitOfWork.Rollback();
                Servicos.Log.TratarErro(excecao);
                return new JsonpResult(false, "Ocorreu uma falha ao remover dados.");
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        [AllowAuthenticate]
        public async Task<IActionResult> ExportarPesquisa()
        {
            try
            {
                var grid = ObterGridPesquisa();

                byte[] arquivoBinario = grid.GerarExcel();

                if (arquivoBinario != null)
                    return Arquivo(arquivoBinario, "application/octet-stream", $"{grid.tituloExportacao}.{grid.extensaoCSV}");

                return new JsonpResult(false, "Ocorreu uma falha ao gerar arquivo.");
            }
            catch (Exception excecao)
            {
                Servicos.Log.TratarErro(excecao);

                return new JsonpResult(false, "Ocorreu uma falha ao exportar.");
            }
        }

        [AllowAuthenticate]
        public async Task<IActionResult> Pesquisa()
        {
            try
            {
                return new JsonpResult(ObterGridPesquisa());
            }
            catch (Exception excecao)
            {
                Servicos.Log.TratarErro(excecao);

                return new JsonpResult(false, Localization.Resources.Gerais.Geral.OcorreuFalhaAoConsultar);
            }
        }

        #endregion

        #region Métodos Privados

        private void PreencherCategoriaPessoa(Dominio.Entidades.Embarcador.Pessoas.CategoriaPessoa categoriaPessoa)
        {
            categoriaPessoa.Descricao = Request.GetNullableStringParam("Descricao") ?? throw new ControllerException(Localization.Resources.Consultas.CategoriaPessoa.DescricaoObrigatoria);
            categoriaPessoa.Observacao = Request.GetNullableStringParam("Observacao");
            categoriaPessoa.CodigoIntegracao = Request.GetStringParam("CodigoIntegracao");
            categoriaPessoa.Cor = Request.GetStringParam("Cor");
        }

        private Models.Grid.Grid ObterGridPesquisa()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                string descricao = Request.GetStringParam("Descricao");

                Models.Grid.Grid grid = new Models.Grid.Grid(Request)
                {
                    header = new List<Models.Grid.Head>()
                };

                grid.AdicionarCabecalho("Codigo", false);
                grid.AdicionarCabecalho("Cor", false);
                grid.AdicionarCabecalho(Localization.Resources.Gerais.Geral.Descricao, "Descricao", 50, Models.Grid.Align.left, true);
                

                Dominio.ObjetosDeValor.Embarcador.Consulta.ParametroConsulta parametrosConsulta = grid.ObterParametrosConsulta();
                Repositorio.Embarcador.Pessoas.CategoriaPessoa repositorio = new Repositorio.Embarcador.Pessoas.CategoriaPessoa(unitOfWork);
                int totalRegistros = repositorio.ContarConsulta(descricao);
                List<Dominio.Entidades.Embarcador.Pessoas.CategoriaPessoa> listaCategoriaPessoa = totalRegistros > 0 ? repositorio.Consultar(descricao, parametrosConsulta) : new List<Dominio.Entidades.Embarcador.Pessoas.CategoriaPessoa>();

                var listaCategoriaPessoaRetornar = (
                    from categoriaPessoa in listaCategoriaPessoa
                    select new
                    {
                        categoriaPessoa.Codigo,
                        categoriaPessoa.Descricao,
                        categoriaPessoa.Cor
                    }
                ).ToList();

                grid.AdicionaRows(listaCategoriaPessoaRetornar);
                grid.setarQuantidadeTotal(totalRegistros);

                return grid;
            }
            catch (Exception)
            {
                throw;
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        #endregion
    }
}
