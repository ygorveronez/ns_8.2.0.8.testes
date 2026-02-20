using SGTAdmin.Controllers;
using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Mvc;

namespace SGT.WebAdmin.Controllers.Produtos
{
    [CustomAuthorize("Produtos/MotivoFalhaGTA")]
    public class MotivoFalhaGTAController : BaseController
    {
		#region Construtores

		public MotivoFalhaGTAController(Conexao conexao) : base(conexao) { }

		#endregion

        #region Métodos Globais
        [AllowAuthenticate]
        public async Task<IActionResult> Pesquisa()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);
            try
            {
                // Manipula grids
                Models.Grid.Grid grid = GridPesquisa(unitOfWork);

                // Ordenacao da grid
                string propOrdenar = grid.header[grid.indiceColunaOrdena].data;
                PropOrdena(ref propOrdenar);

                // Busca Dados
                int totalRegistros = 0;
                var lista = ExecutaPesquisa(ref totalRegistros, propOrdenar, grid.dirOrdena, grid.inicio, grid.limite, unitOfWork);

                // Seta valores na grid
                grid.AdicionaRows(lista);
                grid.setarQuantidadeTotal(totalRegistros);

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

        [AllowAuthenticate]
        public async Task<IActionResult> ExportarPesquisa()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);
            try
            {
                // Manipula grids
                Models.Grid.Grid grid = GridPesquisa(unitOfWork);

                // Ordenacao da grid
                string propOrdenar = grid.header[grid.indiceColunaOrdena].data;
                PropOrdena(ref propOrdenar);

                // Busca Dados
                int totalRegistros = 0;
                var lista = ExecutaPesquisa(ref totalRegistros, propOrdenar, grid.dirOrdena, grid.inicio, grid.limite, unitOfWork);

                // Seta valores na grid
                grid.AdicionaRows(lista);

                // Gera excel
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
                // Instancia repositorios
                Repositorio.Embarcador.Produtos.MotivoFalhaGTA repMotivoFalhaGTA = new Repositorio.Embarcador.Produtos.MotivoFalhaGTA(unitOfWork);

                // Parametros
                int codigo = Request.GetIntParam("Codigo");

                // Busca informacoes
                Dominio.Entidades.Embarcador.Produtos.MotivoFalhaGTA MotivoFalhaGTA = repMotivoFalhaGTA.BuscarPorCodigo(codigo, true);

                // Valida
                if (MotivoFalhaGTA == null)
                    return new JsonpResult(false, true, "Não foi possível encontrar o registro.");

                // Formata retorno
                var retorno = new
                {
                    MotivoFalhaGTA.Codigo,
                    MotivoFalhaGTA.Descricao,
                    Status = MotivoFalhaGTA.Ativo,
                    MotivoFalhaGTA.Ativo,
                    MotivoFalhaGTA.ExigirFotoGTA,
                };

                // Retorna informacoes
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
                // Inicia transacao
                unitOfWork.Start();

                // Instancia repositorios
                Repositorio.Embarcador.Produtos.MotivoFalhaGTA repMotivoFalhaGTA = new Repositorio.Embarcador.Produtos.MotivoFalhaGTA(unitOfWork);

                // Busca informacoes
                Dominio.Entidades.Embarcador.Produtos.MotivoFalhaGTA MotivoFalhaGTA = new Dominio.Entidades.Embarcador.Produtos.MotivoFalhaGTA();

                // Preenche entidade com dados
                PreencheEntidade(ref MotivoFalhaGTA, unitOfWork);

                // Valida entidade
                string erro;
                if (!ValidaEntidade(MotivoFalhaGTA, out erro))
                    return new JsonpResult(false, true, erro);

                // Persiste dados
                repMotivoFalhaGTA.Inserir(MotivoFalhaGTA, Auditado);
                unitOfWork.CommitChanges();

                // Retorna sucesso
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
                // Inicia transacao
                unitOfWork.Start();

                // Instancia repositorios
                Repositorio.Embarcador.Produtos.MotivoFalhaGTA repMotivoFalhaGTA = new Repositorio.Embarcador.Produtos.MotivoFalhaGTA(unitOfWork);

                // Parametros
                int codigo = 0;
                int.TryParse(Request.Params("Codigo"), out codigo);

                // Busca informacoes
                Dominio.Entidades.Embarcador.Produtos.MotivoFalhaGTA MotivoFalhaGTA = repMotivoFalhaGTA.BuscarPorCodigo(codigo, true);

                // Valida
                if (MotivoFalhaGTA == null)
                    return new JsonpResult(false, true, "Não foi possível encontrar o registro.");

                // Preenche entidade com dados
                PreencheEntidade(ref MotivoFalhaGTA, unitOfWork);

                // Valida entidade
                string erro;
                if (!ValidaEntidade(MotivoFalhaGTA, out erro))
                    return new JsonpResult(false, true, erro);

                // Persiste dados
                repMotivoFalhaGTA.Atualizar(MotivoFalhaGTA, Auditado);
                unitOfWork.CommitChanges();

                // Retorna sucesso
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
                // Inicia transacao
                unitOfWork.Start();

                // Instancia repositorios
                Repositorio.Embarcador.Produtos.MotivoFalhaGTA repMotivoFalhaGTA = new Repositorio.Embarcador.Produtos.MotivoFalhaGTA(unitOfWork);

                // Parametros
                int codigo = 0;
                int.TryParse(Request.Params("Codigo"), out codigo);

                // Busca informacoes
                Dominio.Entidades.Embarcador.Produtos.MotivoFalhaGTA MotivoFalhaGTA = repMotivoFalhaGTA.BuscarPorCodigo(codigo, true);

                // Valida
                if (MotivoFalhaGTA == null)
                    return new JsonpResult(false, true, "Não foi possível encontrar o registro.");

                // Persiste dados
                repMotivoFalhaGTA.Deletar(MotivoFalhaGTA, Auditado);
                unitOfWork.CommitChanges();

                // Retorna informacoes
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
        private void PreencheEntidade(ref Dominio.Entidades.Embarcador.Produtos.MotivoFalhaGTA MotivoFalhaGTA, Repositorio.UnitOfWork unitOfWork)
        {
            MotivoFalhaGTA.Descricao = Request.GetStringParam("Descricao");
            MotivoFalhaGTA.Ativo = Request.GetBoolParam("Status");
            MotivoFalhaGTA.ExigirFotoGTA = Request.GetBoolParam("ExigirFotoGTA");
        }

        private bool ValidaEntidade(Dominio.Entidades.Embarcador.Produtos.MotivoFalhaGTA MotivoFalhaGTA, out string msgErro)
        {
            /* ValidaEntidade
             * Recebe uma instancia da entidade
             * Valida informacoes
             * Retorna de entidade e valida ou nao e retorna erro (se tiver)
             */
            msgErro = "";


            if (string.IsNullOrWhiteSpace(MotivoFalhaGTA.Descricao))
            {
                msgErro = "Descrição é obrigatória.";
                return false;
            }

            if (MotivoFalhaGTA.Descricao.Length > 200)
            {
                msgErro = "Descrição não pode passar de 200 caracteres.";
                return false;
            }

            return true;
        }

        private dynamic ExecutaPesquisa(ref int totalRegistros, string propOrdenar, string dirOrdena, int inicio, int limite, Repositorio.UnitOfWork unitOfWork)
        {
            // Instancia repositorios
            Repositorio.Embarcador.Produtos.MotivoFalhaGTA repMotivoFalhaGTA = new Repositorio.Embarcador.Produtos.MotivoFalhaGTA(unitOfWork);

            // Dados do filtro
            Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoAtivoPesquisa status;
            if (!string.IsNullOrWhiteSpace(Request.Params("Status")))
                Enum.TryParse(Request.Params("Status"), out status);
            else
                status = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoAtivoPesquisa.Ativo;

            string descricao = Request.Params("Descricao");

            // Consulta
            List<Dominio.Entidades.Embarcador.Produtos.MotivoFalhaGTA> listaGrid = repMotivoFalhaGTA.Consultar(descricao, status, propOrdenar, dirOrdena, inicio, limite);
            totalRegistros = repMotivoFalhaGTA.ContarConsulta(descricao, status);

            var lista = from obj in listaGrid
                        select new
                        {
                            Codigo = obj.Codigo,
                            Descricao = obj.Descricao,
                            DescricaoAtivo = obj.DescricaoAtivo
                        };

            return lista.ToList();
        }

        private void PropOrdena(ref string propOrdenar)
        {
            /* PropOrdena
             * Recebe o campo ordenado na grid
             * Retorna o elemento especifico da entidade para ordenacao
             */

            if (propOrdenar == "Descricao") propOrdenar = "Descricao";
            else if (propOrdenar == "DescricaoAtivo") propOrdenar = "Ativo";
        }

        private Models.Grid.Grid GridPesquisa(Repositorio.UnitOfWork unitOfWork)
        {
            // Manipula grids
            Models.Grid.Grid grid = new Models.Grid.Grid(Request);
            grid.header = new List<Models.Grid.Head>();

            // Cabecalhos grid
            grid.AdicionarCabecalho("Codigo", false);
            grid.AdicionarCabecalho("Descrição", "Descricao", 15, Models.Grid.Align.left, true);
            grid.AdicionarCabecalho("Status", "DescricaoAtivo", 15, Models.Grid.Align.left, true);

            return grid;
        }
        #endregion
    }
}
