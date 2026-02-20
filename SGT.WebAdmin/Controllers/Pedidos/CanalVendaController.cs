using SGTAdmin.Controllers;
using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Mvc;

namespace SGT.WebAdmin.Controllers.Pedidos
{
    [CustomAuthorize("Pedidos/CanalVenda")]
    public class CanalVendaController : BaseController
    {
		#region Construtores

		public CanalVendaController(Conexao conexao) : base(conexao) { }

		#endregion

        #region Métodos Globais
        [AllowAuthenticate]
        public async Task<IActionResult> Pesquisa()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);
            try
            {
                Models.Grid.Grid grid = GridPesquisa(unitOfWork);

                string propOrdenar = grid.header[grid.indiceColunaOrdena].data;
                PropOrdena(ref propOrdenar);

                int totalRegistros = 0;
                var lista = ExecutaPesquisa(ref totalRegistros, propOrdenar, grid.dirOrdena, grid.inicio, grid.limite, unitOfWork);

                grid.AdicionaRows(lista);
                grid.setarQuantidadeTotal(totalRegistros);

                return new JsonpResult(grid);
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);
                return new JsonpResult(false, Localization.Resources.Gerais.Geral.OcorreuFalhaAoConsultar);
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        [AllowAuthenticate]
        public async Task<IActionResult> ExportarPesquisa()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);
            try
            {
                Models.Grid.Grid grid = GridPesquisa(unitOfWork);

                string propOrdenar = grid.header[grid.indiceColunaOrdena].data;
                PropOrdena(ref propOrdenar);

                int totalRegistros = 0;
                var lista = ExecutaPesquisa(ref totalRegistros, propOrdenar, grid.dirOrdena, grid.inicio, grid.limite, unitOfWork);

                grid.AdicionaRows(lista);

                byte[] bArquivo = grid.GerarExcel();

                if (bArquivo != null)
                    return Arquivo(bArquivo, "application/octet-stream", grid.tituloExportacao + "." + grid.extensaoCSV);
                else
                    return new JsonpResult(false, "Ocorreu uma falha ao gerar arquivo.");
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);
                return new JsonpResult(false, "Ocorreu uma falha ao exportar.");
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
                Repositorio.Embarcador.Pedidos.CanalVenda repositorioCanalVenda = new Repositorio.Embarcador.Pedidos.CanalVenda(unitOfWork);

                int codigo = Request.GetIntParam("Codigo");

                Dominio.Entidades.Embarcador.Pedidos.CanalVenda canalVenda = repositorioCanalVenda.BuscarPorCodigo(codigo);

                if (canalVenda == null)
                    return new JsonpResult(false, true, "Não foi possível encontrar o registro.");

                var retorno = new
                {
                    canalVenda.Codigo,
                    canalVenda.CodigoIntegracao,
                    canalVenda.Descricao,
                    canalVenda.NivelPrioridade,
                    canalVenda.Ativo,
                    Filial = canalVenda.Filial != null ? new { canalVenda.Filial.Codigo, canalVenda.Filial.Descricao } : null
                };

                return new JsonpResult(retorno);
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

        public async Task<IActionResult> Adicionar()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);
            try
            {
                unitOfWork.Start();

                Repositorio.Embarcador.Pedidos.CanalVenda repositorioCanalVenda = new Repositorio.Embarcador.Pedidos.CanalVenda(unitOfWork);

                Dominio.Entidades.Embarcador.Pedidos.CanalVenda canalVenda = new Dominio.Entidades.Embarcador.Pedidos.CanalVenda();

                PreencheEntidade(ref canalVenda, unitOfWork);

                if (!string.IsNullOrWhiteSpace(canalVenda.CodigoIntegracao))
                {
                    Dominio.Entidades.Embarcador.Pedidos.CanalVenda canalVendaExiste = repositorioCanalVenda.BuscarPorCodigoIntegracao(canalVenda.CodigoIntegracao);
                    if (canalVendaExiste != null)
                        return new JsonpResult(false, true, "Já existe um Canal de Venda cadastrado com o código de integração ." + canalVenda.CodigoIntegracao + ".");
                }

                repositorioCanalVenda.Inserir(canalVenda, Auditado);
                unitOfWork.CommitChanges();

                return new JsonpResult(true);
            }
            catch (Exception ex)
            {
                unitOfWork.Rollback();
                Servicos.Log.TratarErro(ex);
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
                Repositorio.Embarcador.Pedidos.CanalVenda repositorioCanalVenda = new Repositorio.Embarcador.Pedidos.CanalVenda(unitOfWork);

                int codigo = Request.GetIntParam("Codigo");

                Dominio.Entidades.Embarcador.Pedidos.CanalVenda canalVenda = repositorioCanalVenda.BuscarPorCodigo(codigo, true);

                if (canalVenda == null)
                    return new JsonpResult(false, true, "Não foi possível encontrar o registro.");

                PreencheEntidade(ref canalVenda, unitOfWork);
                if (!string.IsNullOrWhiteSpace(canalVenda.CodigoIntegracao))
                {
                    Dominio.Entidades.Embarcador.Pedidos.CanalVenda canalVendaExiste = repositorioCanalVenda.BuscarPorCodigoIntegracao(canalVenda.CodigoIntegracao);
                    if (canalVendaExiste != null && canalVendaExiste.Codigo != canalVenda.Codigo)
                        return new JsonpResult(false, true, "Já existe um Canal de Venda cadastrado com o código de integração ." + canalVenda.CodigoIntegracao + ".");

                }

                unitOfWork.Start();

                repositorioCanalVenda.Atualizar(canalVenda, Auditado);
                unitOfWork.CommitChanges();

                return new JsonpResult(true);
            }
            catch (Exception ex)
            {
                unitOfWork.Rollback();
                Servicos.Log.TratarErro(ex);
                return new JsonpResult(false, "Ocorreu uma falha ao atualizar dados.");
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

                Repositorio.Embarcador.Pedidos.CanalVenda repositorioCanalVenda = new Repositorio.Embarcador.Pedidos.CanalVenda(unitOfWork);

                int codigo = Request.GetIntParam("Codigo");

                Dominio.Entidades.Embarcador.Pedidos.CanalVenda canalVenda = repositorioCanalVenda.BuscarPorCodigo(codigo);

                if (canalVenda == null)
                    return new JsonpResult(false, true, "Não foi possível encontrar o registro.");

                repositorioCanalVenda.Deletar(canalVenda, Auditado);
                unitOfWork.CommitChanges();

                return new JsonpResult(true);
            }
            catch (Exception ex)
            {
                unitOfWork.Rollback();
                Servicos.Log.TratarErro(ex);
                return new JsonpResult(false, "Ocorreu uma falha ao remover dados.");
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        #endregion

        #region Métodos Privados

        private void PreencheEntidade(ref Dominio.Entidades.Embarcador.Pedidos.CanalVenda canalVenda, Repositorio.UnitOfWork unitOfWork)
        {
            Repositorio.Embarcador.Filiais.Filial repositorioFilialCanalVenda = new Repositorio.Embarcador.Filiais.Filial(unitOfWork);

            int codigoFilial = Request.GetIntParam("Filial");

            canalVenda.Descricao = Request.GetStringParam("Descricao");
            canalVenda.CodigoIntegracao = Request.GetStringParam("CodigoIntegracao");
            canalVenda.Ativo = Request.GetBoolParam("Ativo");
            canalVenda.NivelPrioridade = Request.GetIntParam("NivelPrioridade");
            canalVenda.Filial = codigoFilial > 0 ? repositorioFilialCanalVenda.BuscarPorCodigo(codigoFilial) : null;
        }
        
        private dynamic ExecutaPesquisa(ref int totalRegistros, string propOrdenar, string dirOrdena, int inicio, int limite, Repositorio.UnitOfWork unitOfWork)
        {
            Repositorio.Embarcador.Pedidos.CanalVenda repositorioCanalVenda = new Repositorio.Embarcador.Pedidos.CanalVenda(unitOfWork);

            Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoAtivoPesquisa ativo = Request.GetEnumParam<Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoAtivoPesquisa>("Ativo");
            string descricao = Request.Params("Descricao");
            string codigoIntegracao = Request.Params("CodigoIntegracao");
            int codigoFilial = Request.GetIntParam("Filial");


            List<Dominio.Entidades.Embarcador.Pedidos.CanalVenda> listaGrid = repositorioCanalVenda.Consultar(codigoFilial, descricao, codigoIntegracao, ativo, propOrdenar, dirOrdena, inicio, limite);
            totalRegistros = repositorioCanalVenda.ContarConsulta(codigoFilial, descricao, codigoIntegracao, ativo);

            var lista = from obj in listaGrid
                        select new
                        {
                            Codigo = obj.Codigo,
                            Descricao = obj.Descricao,
                            obj.CodigoIntegracao,
                            obj.NivelPrioridade,
                            DescricaoAtivo = obj.DescricaoAtivo,
                            DescricaoFilial = obj.Filial?.Descricao ?? "",
                        };

            return lista.ToList();
        }

        private void PropOrdena(ref string propOrdenar)
        {
            if (propOrdenar == "DescricaoAtivo") propOrdenar = "Ativo";
        }

        private Models.Grid.Grid GridPesquisa(Repositorio.UnitOfWork unitOfWork)
        {
            Models.Grid.Grid grid = new Models.Grid.Grid(Request);
            grid.header = new List<Models.Grid.Head>();

            grid.AdicionarCabecalho("Codigo", false);
            grid.AdicionarCabecalho(Localization.Resources.Pedidos.CanalVenda.Descricao, "Descricao", 40, Models.Grid.Align.left, true);
            grid.AdicionarCabecalho(Localization.Resources.Pedidos.CanalVenda.CodigoIntegracao, "CodigoIntegracao", 10, Models.Grid.Align.left, true);
            grid.AdicionarCabecalho(Localization.Resources.Pedidos.CanalVenda.NivelPrioridade, "NivelPrioridade", 10, Models.Grid.Align.center, true);
            grid.AdicionarCabecalho(Localization.Resources.Gerais.Geral.Status, "DescricaoAtivo", 10, Models.Grid.Align.left, true);
            grid.AdicionarCabecalho(Localization.Resources.Pedidos.CanalVenda.Filial, "DescricaoFilial", 15, Models.Grid.Align.left, false);

            return grid;
        }

        #endregion
    }
}
