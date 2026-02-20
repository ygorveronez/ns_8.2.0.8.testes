using SGTAdmin.Controllers;
using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Mvc;

namespace SGT.WebAdmin.Controllers.Cargas.ControleEntrega
{
    [CustomAuthorize("Cargas/MotivoRejeicaoColeta")]
    public class MotivoRejeicaoColetaController : BaseController
    {
		#region Construtores

		public MotivoRejeicaoColetaController(Conexao conexao) : base(conexao) { }

		#endregion

        #region Métodos Globais

        public async Task<IActionResult> Adicionar()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                Repositorio.Embarcador.Cargas.ControleEntrega.MotivoRejeicaoColeta repMotivoRejeicaoColeta = new Repositorio.Embarcador.Cargas.ControleEntrega.MotivoRejeicaoColeta(unitOfWork);

                Dominio.Entidades.Embarcador.Cargas.ControleEntrega.MotivoRejeicaoColeta motivoRejeicaoColeta = new Dominio.Entidades.Embarcador.Cargas.ControleEntrega.MotivoRejeicaoColeta();

                PreencherEntidade(motivoRejeicaoColeta, unitOfWork);

                unitOfWork.Start();

                repMotivoRejeicaoColeta.Inserir(motivoRejeicaoColeta, Auditado);

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

                Repositorio.Embarcador.Cargas.ControleEntrega.MotivoRejeicaoColeta repMotivoRejeicaoColeta = new Repositorio.Embarcador.Cargas.ControleEntrega.MotivoRejeicaoColeta(unitOfWork);

                Dominio.Entidades.Embarcador.Cargas.ControleEntrega.MotivoRejeicaoColeta motivoRejeicaoColeta = repMotivoRejeicaoColeta.BuscarPorCodigo(codigo, true);

                if (motivoRejeicaoColeta == null)
                    return new JsonpResult(false, true, "Não foi possível encontrar o registro.");

                PreencherEntidade(motivoRejeicaoColeta, unitOfWork);

                unitOfWork.Start();

                repMotivoRejeicaoColeta.Atualizar(motivoRejeicaoColeta, Auditado);

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

                Repositorio.Embarcador.Cargas.ControleEntrega.MotivoRejeicaoColeta repMotivoRejeicaoColeta = new Repositorio.Embarcador.Cargas.ControleEntrega.MotivoRejeicaoColeta(unitOfWork);

                Dominio.Entidades.Embarcador.Cargas.ControleEntrega.MotivoRejeicaoColeta motivoRejeicaoColeta = repMotivoRejeicaoColeta.BuscarPorCodigo(codigo, false);

                if (motivoRejeicaoColeta == null)
                    return new JsonpResult(false, true, "Não foi possível encontrar o registro.");

                return new JsonpResult(new
                {
                    motivoRejeicaoColeta.Codigo,
                    motivoRejeicaoColeta.Descricao,
                    Situacao = motivoRejeicaoColeta.Ativo,
                    Produtos = motivoRejeicaoColeta.Produtos.Select(obj => new
                    {
                        Codigo = obj.Codigo,
                        Descricao = obj.Descricao
                    }).ToList()
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

                Repositorio.Embarcador.Cargas.ControleEntrega.MotivoRejeicaoColeta repMotivoRejeicaoColeta = new Repositorio.Embarcador.Cargas.ControleEntrega.MotivoRejeicaoColeta(unitOfWork);

                Dominio.Entidades.Embarcador.Cargas.ControleEntrega.MotivoRejeicaoColeta motivoRejeicaoColeta = repMotivoRejeicaoColeta.BuscarPorCodigo(codigo, true);

                if (motivoRejeicaoColeta == null)
                    return new JsonpResult(false, true, "Não foi possível encontrar o registro.");

                unitOfWork.Start();

                motivoRejeicaoColeta.Produtos = null;

                repMotivoRejeicaoColeta.Deletar(motivoRejeicaoColeta, Auditado);

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

        private void PreencherEntidade(Dominio.Entidades.Embarcador.Cargas.ControleEntrega.MotivoRejeicaoColeta motivoRejeicaoColeta, Repositorio.UnitOfWork unitOfWork)
        {
            motivoRejeicaoColeta.Ativo = Request.GetBoolParam("Situacao");
            motivoRejeicaoColeta.Descricao = Request.GetStringParam("Descricao");

            SalvarProdutos(motivoRejeicaoColeta, unitOfWork);
        }

        private void SalvarProdutos(Dominio.Entidades.Embarcador.Cargas.ControleEntrega.MotivoRejeicaoColeta motivoRejeicaoColeta, Repositorio.UnitOfWork unidadeDeTrabalho)
        {
            Repositorio.Embarcador.Produtos.ProdutoEmbarcador repProdutoEmbarcador = new Repositorio.Embarcador.Produtos.ProdutoEmbarcador(unidadeDeTrabalho);

            dynamic produtos = Newtonsoft.Json.JsonConvert.DeserializeObject<dynamic>(Request.Params("Produtos"));

            if (motivoRejeicaoColeta.Produtos == null)
            {
                motivoRejeicaoColeta.Produtos = new List<Dominio.Entidades.Embarcador.Produtos.ProdutoEmbarcador>();
            }
            else
            {
                List<int> codigos = new List<int>();

                foreach (dynamic produto in produtos)
                    codigos.Add((int)produto.Codigo);

                List<Dominio.Entidades.Embarcador.Produtos.ProdutoEmbarcador> produtosDeletar = motivoRejeicaoColeta.Produtos.Where(o => !codigos.Contains(o.Codigo)).ToList();

                foreach (Dominio.Entidades.Embarcador.Produtos.ProdutoEmbarcador produtoDeletar in produtosDeletar)
                    motivoRejeicaoColeta.Produtos.Remove(produtoDeletar);
            }

            foreach (var produto in produtos)
            {
                if (!motivoRejeicaoColeta.Produtos.Any(o => o.Codigo == (int)produto.Codigo))
                {
                    Dominio.Entidades.Embarcador.Produtos.ProdutoEmbarcador produtoObj = repProdutoEmbarcador.BuscarPorCodigo((int)produto.Codigo);
                    motivoRejeicaoColeta.Produtos.Add(produtoObj);
                }
            }
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
                grid.AdicionarCabecalho("Descrição", "Descricao", 60, Models.Grid.Align.left, true);

                if (status == Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoAtivoPesquisa.Todos)
                    grid.AdicionarCabecalho("Status", "DescricaoAtivo", 25, Models.Grid.Align.left, true);

                string propriedadeOrdenar = ObterPropriedadeOrdenar(grid.header[grid.indiceColunaOrdena].data);

                Repositorio.Embarcador.Cargas.ControleEntrega.MotivoRejeicaoColeta repMotivoRejeicaoColeta = new Repositorio.Embarcador.Cargas.ControleEntrega.MotivoRejeicaoColeta(unitOfWork);

                List<Dominio.Entidades.Embarcador.Cargas.ControleEntrega.MotivoRejeicaoColeta> listaMotivoRejeicaoColeta = repMotivoRejeicaoColeta.Consultar(descricao, status, propriedadeOrdenar, grid.dirOrdena, grid.inicio, grid.limite);
                int totalRegistros = repMotivoRejeicaoColeta.ContarConsulta(descricao, status);

                var retorno = listaMotivoRejeicaoColeta.Select(motivo => new
                {
                    motivo.Codigo,
                    motivo.Descricao,
                    motivo.DescricaoAtivo
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
