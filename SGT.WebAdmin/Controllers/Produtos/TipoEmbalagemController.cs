using SGTAdmin.Controllers;
using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Mvc;

namespace SGT.WebAdmin.Controllers.Produtos

{
    [CustomAuthorize("Produtos/TipoEmbalagem")]
    public class TipoEmbalagemController : BaseController
    {
		#region Construtores

		public TipoEmbalagemController(Conexao conexao) : base(conexao) { }

		#endregion

        #region Métodos Globais

        public async Task<IActionResult> Adicionar()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                Repositorio.Embarcador.Produtos.TipoEmbalagem repTipoEmbalagem = new Repositorio.Embarcador.Produtos.TipoEmbalagem(unitOfWork);

                Dominio.Entidades.Embarcador.Produtos.TipoEmbalagem tipoEmbalagem = new Dominio.Entidades.Embarcador.Produtos.TipoEmbalagem();

                PreencherEntidade(tipoEmbalagem, unitOfWork);

                unitOfWork.Start();

                repTipoEmbalagem.Inserir(tipoEmbalagem, Auditado);

                unitOfWork.CommitChanges();


                return new JsonpResult(true);
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

                Repositorio.Embarcador.Produtos.TipoEmbalagem repTipoEmbalagem = new Repositorio.Embarcador.Produtos.TipoEmbalagem(unitOfWork);

                Dominio.Entidades.Embarcador.Produtos.TipoEmbalagem tipoEmbalagem = repTipoEmbalagem.BuscarPorCodigo(codigo, true);

                if (tipoEmbalagem == null)
                    return new JsonpResult(false, true, "Não foi possível encontrar o registro.");

                PreencherEntidade(tipoEmbalagem, unitOfWork);

                unitOfWork.Start();

                repTipoEmbalagem.Atualizar(tipoEmbalagem, Auditado);

                unitOfWork.CommitChanges();

                return new JsonpResult(true);
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

                Repositorio.Embarcador.Produtos.TipoEmbalagem repTipoEmbalagem = new Repositorio.Embarcador.Produtos.TipoEmbalagem(unitOfWork);

                Dominio.Entidades.Embarcador.Produtos.TipoEmbalagem tipoEmbalagem = repTipoEmbalagem.BuscarPorCodigo(codigo, false);

                if (tipoEmbalagem == null)
                    return new JsonpResult(false, true, "Não foi possível encontrar o registro.");

                return new JsonpResult(new
                {
                    tipoEmbalagem.Codigo,
                    tipoEmbalagem.Descricao,
                    tipoEmbalagem.CodigoIntegracao,
                    tipoEmbalagem.Observacao,
                    Situacao = tipoEmbalagem.Ativo
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

                Repositorio.Embarcador.Produtos.TipoEmbalagem repTipoEmbalagem = new Repositorio.Embarcador.Produtos.TipoEmbalagem(unitOfWork);

                Dominio.Entidades.Embarcador.Produtos.TipoEmbalagem tipoEmbalagem = repTipoEmbalagem.BuscarPorCodigo(codigo, true);

                if (tipoEmbalagem == null)
                    return new JsonpResult(false, true, "Não foi possível encontrar o registro.");

                unitOfWork.Start();


                repTipoEmbalagem.Deletar(tipoEmbalagem, Auditado);

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

                return new JsonpResult(false, "Ocorreu uma falha ao consultar.");
            }
        }

        #endregion

        #region Métodos Privados

        private void PreencherEntidade(Dominio.Entidades.Embarcador.Produtos.TipoEmbalagem tipoEmbalagem, Repositorio.UnitOfWork unitOfWork)
        {
            tipoEmbalagem.Ativo = Request.GetBoolParam("Situacao");
            tipoEmbalagem.Descricao = Request.GetStringParam("Descricao");
            tipoEmbalagem.CodigoIntegracao = Request.GetStringParam("CodigoIntegracao");
            tipoEmbalagem.Observacao = Request.GetStringParam("Observacao");
        }

        private Models.Grid.Grid ObterGridPesquisa()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                string descricao = Request.Params("Descricao");
                Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoAtivoPesquisa status = Request.GetEnumParam("Situacao", Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoAtivoPesquisa.Ativo);

                Models.Grid.Grid grid = new Models.Grid.Grid(Request)
                {
                    header = new List<Models.Grid.Head>()
                };

                grid.AdicionarCabecalho("Codigo", false);
                grid.AdicionarCabecalho(Localization.Resources.Fretes.TabelaFrete.CodigoIntegracao, "CodigoIntegracao", 20, Models.Grid.Align.left, true);
                grid.AdicionarCabecalho(Localization.Resources.Fretes.TabelaFrete.Descricao, "Descricao", 60, Models.Grid.Align.left, true);

                if (status == Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoAtivoPesquisa.Todos)
                    grid.AdicionarCabecalho("Status", "DescricaoAtivo", 20, Models.Grid.Align.left, true);

                string propriedadeOrdenar = ObterPropriedadeOrdenar(grid.header[grid.indiceColunaOrdena].data);

                Repositorio.Embarcador.Produtos.TipoEmbalagem repTipoEmbalagem = new Repositorio.Embarcador.Produtos.TipoEmbalagem(unitOfWork);

                List<Dominio.Entidades.Embarcador.Produtos.TipoEmbalagem> listaTipoEmbalagem = repTipoEmbalagem.Consultar(descricao, status, propriedadeOrdenar, grid.dirOrdena, grid.inicio, grid.limite);
                int totalRegistros = repTipoEmbalagem.ContarConsulta(descricao, status);

                var retorno = listaTipoEmbalagem.Select(tipo => new
                {
                    tipo.Codigo,
                    tipo.Descricao,
                    tipo.CodigoIntegracao,                    
                    tipo.DescricaoAtivo
                }).ToList();

                grid.AdicionaRows(retorno);
                grid.setarQuantidadeTotal(totalRegistros);

                return grid;
            }
            catch
            {
                throw;
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        private string ObterPropriedadeOrdenar(string propriedadeOrdenar)
        {
            if (propriedadeOrdenar == "DescricaoAtivo")
                return "Ativo";

            return propriedadeOrdenar;
        }

        #endregion
    }
}
