using Dominio.ObjetosDeValor.Embarcador.Enumeradores;
using SGTAdmin.Controllers;
using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Mvc;

namespace SGT.WebAdmin.Controllers.Ocorrencias
{
    [CustomAuthorize("Ocorrencias/MotivoRejeicaoOcorrencia")]
    public class MotivoRejeicaoOcorrenciaController : BaseController
    {
		#region Construtores

		public MotivoRejeicaoOcorrenciaController(Conexao conexao) : base(conexao) { }

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
                Repositorio.Embarcador.Ocorrencias.MotivoRejeicaoOcorrencia repMotivoRejeicaoOcorrencia = new Repositorio.Embarcador.Ocorrencias.MotivoRejeicaoOcorrencia(unitOfWork);

                int codigo = Request.GetIntParam("Codigo");

                Dominio.Entidades.Embarcador.Ocorrencias.MotivoRejeicaoOcorrencia motivoRejeicaoOcorrencia = repMotivoRejeicaoOcorrencia.BuscarPorCodigo(codigo);

                if (motivoRejeicaoOcorrencia == null)
                    return new JsonpResult(false, true, "Não foi possível encontrar o registro.");

                var retorno = new
                {
                    motivoRejeicaoOcorrencia.Codigo,
                    motivoRejeicaoOcorrencia.Descricao,
                    Status = motivoRejeicaoOcorrencia.Ativo,
                    Observacao = motivoRejeicaoOcorrencia.Observacao ?? string.Empty,
                    motivoRejeicaoOcorrencia.Tipo,
                    motivoRejeicaoOcorrencia.NaoPermitirAbrirOcorrenciaDuplicadaRejeicao
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

                Repositorio.Embarcador.Ocorrencias.MotivoRejeicaoOcorrencia repMotivoRejeicaoOcorrencia = new Repositorio.Embarcador.Ocorrencias.MotivoRejeicaoOcorrencia(unitOfWork);

                Dominio.Entidades.Embarcador.Ocorrencias.MotivoRejeicaoOcorrencia motivoRejeicaoOcorrencia = new Dominio.Entidades.Embarcador.Ocorrencias.MotivoRejeicaoOcorrencia();

                PreencheEntidade(motivoRejeicaoOcorrencia, unitOfWork);

                string erro;
                if (!ValidaEntidade(motivoRejeicaoOcorrencia, out erro))
                    return new JsonpResult(false, true, erro);

                repMotivoRejeicaoOcorrencia.Inserir(motivoRejeicaoOcorrencia, Auditado);

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
                unitOfWork.Start();

                Repositorio.Embarcador.Ocorrencias.MotivoRejeicaoOcorrencia repMotivoRejeicaoOcorrencia = new Repositorio.Embarcador.Ocorrencias.MotivoRejeicaoOcorrencia(unitOfWork);

                int codigo = Request.GetIntParam("Codigo");

                Dominio.Entidades.Embarcador.Ocorrencias.MotivoRejeicaoOcorrencia motivoRejeicaoOcorrencia = repMotivoRejeicaoOcorrencia.BuscarPorCodigo(codigo, true);

                if (motivoRejeicaoOcorrencia == null)
                    return new JsonpResult(false, true, "Não foi possível encontrar o registro.");

                PreencheEntidade(motivoRejeicaoOcorrencia, unitOfWork);

                string erro;
                if (!ValidaEntidade(motivoRejeicaoOcorrencia, out erro))
                    return new JsonpResult(false, true, erro);

                repMotivoRejeicaoOcorrencia.Atualizar(motivoRejeicaoOcorrencia, Auditado);

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

                Repositorio.Embarcador.Ocorrencias.MotivoRejeicaoOcorrencia repMotivoRejeicaoOcorrencia = new Repositorio.Embarcador.Ocorrencias.MotivoRejeicaoOcorrencia(unitOfWork);

                int codigo = Request.GetIntParam("Codigo");

                Dominio.Entidades.Embarcador.Ocorrencias.MotivoRejeicaoOcorrencia motivoRejeicaoOcorrencia = repMotivoRejeicaoOcorrencia.BuscarPorCodigo(codigo, true);

                if (motivoRejeicaoOcorrencia == null)
                    return new JsonpResult(false, true, "Não foi possível encontrar o registro.");

                repMotivoRejeicaoOcorrencia.Deletar(motivoRejeicaoOcorrencia, Auditado);

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

        #endregion

        #region Métodos Privados

        private void PreencheEntidade(Dominio.Entidades.Embarcador.Ocorrencias.MotivoRejeicaoOcorrencia motivoRejeicaoOcorrencia, Repositorio.UnitOfWork unitOfWork)
        {
            motivoRejeicaoOcorrencia.Descricao = Request.GetStringParam("Descricao");
            motivoRejeicaoOcorrencia.Ativo = Request.GetBoolParam("Status");
            motivoRejeicaoOcorrencia.Observacao = Request.GetStringParam("Observacao");
            motivoRejeicaoOcorrencia.Tipo = Request.GetEnumParam<AprovacaoRejeicao>("Tipo");
            motivoRejeicaoOcorrencia.NaoPermitirAbrirOcorrenciaDuplicadaRejeicao = Request.GetBoolParam("NaoPermitirAbrirOcorrenciaDuplicadaRejeicao");
        }

        private bool ValidaEntidade(Dominio.Entidades.Embarcador.Ocorrencias.MotivoRejeicaoOcorrencia motivoRejeicaoOcorrencia, out string msgErro)
        {
            /* ValidaEntidade
             * Recebe uma instancia da entidade
             * Valida informacoes
             * Retorna de entidade e valida ou nao e retorna erro (se tiver)
             */
            msgErro = "";


            if (string.IsNullOrWhiteSpace(motivoRejeicaoOcorrencia.Descricao))
            {
                msgErro = "Descrição é obrigatória.";
                return false;
            }

            if (motivoRejeicaoOcorrencia.Descricao.Length > 200)
            {
                msgErro = "Descrição não pode passar de 200 caracteres.";
                return false;
            }

            return true;
        }

        private dynamic ExecutaPesquisa(ref int totalRegistros, string propOrdenar, string dirOrdena, int inicio, int limite, Repositorio.UnitOfWork unitOfWork)
        {
            Repositorio.Embarcador.Ocorrencias.MotivoRejeicaoOcorrencia repMotivoRejeicaoOcorrencia = new Repositorio.Embarcador.Ocorrencias.MotivoRejeicaoOcorrencia(unitOfWork);

            string descricao = Request.GetStringParam("Descricao");

            SituacaoAtivoPesquisa status = Request.GetEnumParam<SituacaoAtivoPesquisa>("Status");
            AprovacaoRejeicao? tipo = Request.GetNullableEnumParam<AprovacaoRejeicao>("Tipo");

            List<Dominio.Entidades.Embarcador.Ocorrencias.MotivoRejeicaoOcorrencia> listaGrid = repMotivoRejeicaoOcorrencia.Consultar(descricao, status, tipo, propOrdenar, dirOrdena, inicio, limite);
            totalRegistros = repMotivoRejeicaoOcorrencia.ContarConsulta(descricao, status, tipo);

            var lista = from obj in listaGrid
                        select new
                        {
                            Codigo = obj.Codigo,
                            Descricao = obj.Descricao,
                            DescricaoAtivo = obj.DescricaoAtivo,
                            DescricaoTipo = obj.Tipo.ObterDescricao()
                        };

            return lista.ToList();
        }

        private void PropOrdena(ref string propOrdenar)
        {
            /* PropOrdena
             * Recebe o campo ordenado na grid
             * Retorna o elemento especifico da entidade para ordenacao
             */

            if (propOrdenar == "DescricaoFinalidade") propOrdenar = "Finalidade";
            else if (propOrdenar == "DescricaoAtivo") propOrdenar = "Ativo";
        }

        private Models.Grid.Grid GridPesquisa(Repositorio.UnitOfWork unitOfWork)
        {
            Models.Grid.Grid grid = new Models.Grid.Grid(Request);
            grid.header = new List<Models.Grid.Head>();

            grid.AdicionarCabecalho("Codigo", false);
            grid.AdicionarCabecalho("Descrição", "Descricao", 30, Models.Grid.Align.left, true);
            grid.AdicionarCabecalho("Tipo", "DescricaoTipo", 15, Models.Grid.Align.left, false);
            if (Request.GetEnumParam<SituacaoAtivoPesquisa>("Status") == SituacaoAtivoPesquisa.Todos)
                grid.AdicionarCabecalho("Status", "DescricaoAtivo", 15, Models.Grid.Align.left, true);

            return grid;
        }

        #endregion
    }
}
