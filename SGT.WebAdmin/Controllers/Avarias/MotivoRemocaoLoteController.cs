using SGTAdmin.Controllers;
using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Mvc;

namespace SGT.WebAdmin.Controllers.Avarias
{
    [CustomAuthorize("Avarias/MotivoRemocaoLote")]
    public class MotivoRemocaoLoteController : BaseController
    {
		#region Construtores

		public MotivoRemocaoLoteController(Conexao conexao) : base(conexao) { }

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
                Repositorio.Embarcador.Avarias.MotivoRemocaoLote repMotivoRemocaoLote = new Repositorio.Embarcador.Avarias.MotivoRemocaoLote(unitOfWork);

                // Parametros
                int codigo = 0;
                int.TryParse(Request.Params("Codigo"), out codigo);

                // Busca informacoes
                Dominio.Entidades.Embarcador.Avarias.MotivoRemocaoLote motivoRemocaoLote = repMotivoRemocaoLote.BuscarPorCodigo(codigo);

                // Valida
                if (motivoRemocaoLote == null)
                    return new JsonpResult(false, true, "Não foi possível encontrar o registro.");

                // Formata retorno
                var retorno = new
                {
                    motivoRemocaoLote.Codigo,
                    motivoRemocaoLote.Descricao,
                    Status = motivoRemocaoLote.Ativo,
                    Observacao = motivoRemocaoLote.Observacao ?? string.Empty
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
                Repositorio.Embarcador.Avarias.MotivoRemocaoLote repMotivoRemocaoLote = new Repositorio.Embarcador.Avarias.MotivoRemocaoLote(unitOfWork);

                // Busca informacoes
                Dominio.Entidades.Embarcador.Avarias.MotivoRemocaoLote motivoRemocaoLote = new Dominio.Entidades.Embarcador.Avarias.MotivoRemocaoLote();

                // Preenche entidade com dados
                PreencheEntidade(ref motivoRemocaoLote, unitOfWork);

                // Valida entidade
                string erro;
                if (!ValidaEntidade(motivoRemocaoLote, out erro))
                    return new JsonpResult(false, true, erro);

                // Persiste dados
                repMotivoRemocaoLote.Inserir(motivoRemocaoLote, Auditado);
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
                Repositorio.Embarcador.Avarias.MotivoRemocaoLote repMotivoRemocaoLote = new Repositorio.Embarcador.Avarias.MotivoRemocaoLote(unitOfWork);

                // Parametros
                int codigo = 0;
                int.TryParse(Request.Params("Codigo"), out codigo);

                // Busca informacoes
                Dominio.Entidades.Embarcador.Avarias.MotivoRemocaoLote motivoRemocaoLote = repMotivoRemocaoLote.BuscarPorCodigo(codigo, true);

                // Valida
                if (motivoRemocaoLote == null)
                    return new JsonpResult(false, true, "Não foi possível encontrar o registro.");

                // Preenche entidade com dados
                PreencheEntidade(ref motivoRemocaoLote, unitOfWork);

                // Valida entidade
                string erro;
                if (!ValidaEntidade(motivoRemocaoLote, out erro))
                    return new JsonpResult(false, true, erro);

                // Persiste dados
                repMotivoRemocaoLote.Atualizar(motivoRemocaoLote, Auditado);
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
                Repositorio.Embarcador.Avarias.MotivoRemocaoLote repMotivoRemocaoLote = new Repositorio.Embarcador.Avarias.MotivoRemocaoLote(unitOfWork);

                // Parametros
                int codigo = 0;
                int.TryParse(Request.Params("Codigo"), out codigo);

                // Busca informacoes
                Dominio.Entidades.Embarcador.Avarias.MotivoRemocaoLote motivoRemocaoLote = repMotivoRemocaoLote.BuscarPorCodigo(codigo);

                // Valida
                if (motivoRemocaoLote == null)
                    return new JsonpResult(false, true, "Não foi possível encontrar o registro.");

                // Persiste dados
                repMotivoRemocaoLote.Deletar(motivoRemocaoLote, Auditado);
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
        private void PreencheEntidade(ref Dominio.Entidades.Embarcador.Avarias.MotivoRemocaoLote motivoRemocaoLote, Repositorio.UnitOfWork unitOfWork)
        {
            /* PreencheEntidade
             * Recebe uma instancia da entidade
             * Converte parametros recebido por request
             * Atribui a entidade
             */

            // Converte valores
            string descricao = Request.Params("Descricao");
            if (string.IsNullOrWhiteSpace(descricao)) descricao = string.Empty;

            bool ativo = false;
            bool.TryParse(Request.Params("Status"), out ativo);

            string observacao = Request.Params("Observacao");
            if (string.IsNullOrWhiteSpace(observacao)) observacao = string.Empty;

            // Vincula dados
            motivoRemocaoLote.Descricao = descricao;
            motivoRemocaoLote.Ativo = ativo;
            motivoRemocaoLote.Observacao = observacao;
        }

        private bool ValidaEntidade(Dominio.Entidades.Embarcador.Avarias.MotivoRemocaoLote motivoRemocaoLote, out string msgErro)
        {
            /* ValidaEntidade
             * Recebe uma instancia da entidade
             * Valida informacoes
             * Retorna de entidade e valida ou nao e retorna erro (se tiver)
             */
            msgErro = "";

            if (string.IsNullOrWhiteSpace(motivoRemocaoLote.Descricao))
            {
                msgErro = "Descrição é obrigatória.";
                return false;
            }

            if (motivoRemocaoLote.Descricao.Length > 200)
            {
                msgErro = "Descrição não pode passar de 200 caracteres.";
                return false;
            }

            if (motivoRemocaoLote.Observacao.Length > 2000)
            {
                msgErro = "Observação não pode passar de 2000 caracteres.";
                return false;
            }

            return true;
        }

        private dynamic ExecutaPesquisa(ref int totalRegistros, string propOrdenar, string dirOrdena, int inicio, int limite, Repositorio.UnitOfWork unitOfWork)
        {
            // Instancia repositorios
            Repositorio.Embarcador.Avarias.MotivoRemocaoLote repMotivoRemocaoLote = new Repositorio.Embarcador.Avarias.MotivoRemocaoLote(unitOfWork);

            // Dados do filtro
            Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoAtivoPesquisa status;
            if (!string.IsNullOrWhiteSpace(Request.Params("Status")))
                Enum.TryParse(Request.Params("Status"), out status);
            else
                status = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoAtivoPesquisa.Ativo;

            string descricao = Request.Params("Descricao");

            // Consulta
            List<Dominio.Entidades.Embarcador.Avarias.MotivoRemocaoLote> listaGrid = repMotivoRemocaoLote.Consultar(descricao, status, propOrdenar, dirOrdena, inicio, limite);
            totalRegistros = repMotivoRemocaoLote.ContarConsulta(descricao, status);

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

            if (propOrdenar == "DescricaoAtivo") propOrdenar = "Ativo";
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
