using SGTAdmin.Controllers;
using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Mvc;

namespace SGT.WebAdmin.Controllers.Avarias
{
    [CustomAuthorize("Avarias/MotivoAvaria")]
    public class MotivoAvariaController : BaseController
    {
		#region Construtores

		public MotivoAvariaController(Conexao conexao) : base(conexao) { }

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
                Repositorio.Embarcador.Avarias.MotivoAvaria repMotivoAvaria = new Repositorio.Embarcador.Avarias.MotivoAvaria(unitOfWork);

                int codigo = Request.GetIntParam("Codigo");

                Dominio.Entidades.Embarcador.Avarias.MotivoAvaria motivoAvaria = repMotivoAvaria.BuscarPorCodigo(codigo);

                if (motivoAvaria == null)
                    return new JsonpResult(false, true, "Não foi possível encontrar o registro.");

                var retorno = new
                {
                    motivoAvaria.Codigo,
                    ContaContabil = motivoAvaria.ContaContabil != null ? new { motivoAvaria.ContaContabil.Codigo, motivoAvaria.ContaContabil.Descricao } : null,
                    motivoAvaria.Finalidade,
                    motivoAvaria.Descricao,
                    motivoAvaria.Responsavel,
                    Status = motivoAvaria.Ativo,
                    motivoAvaria.Ativo,
                    Observacao = motivoAvaria.Observacao ?? string.Empty,
                    TipoOcorrencia = motivoAvaria.TipoOcorrencia != null ? new { motivoAvaria.TipoOcorrencia.Codigo, motivoAvaria.TipoOcorrencia.Descricao } : null,
                    motivoAvaria.GerarOcorrenciaAutomaticamente,
                    motivoAvaria.ObrigarInformarValorParaLiberarOcorrencia,
                    motivoAvaria.GerarCTeValorAnteriorTratativaReentrega,
                    motivoAvaria.CalcularOcorrenciaPorTabelaFrete,
                    motivoAvaria.ObrigarAnexo,
                    motivoAvaria.NaoPermitirAberturaAvariasMesmoMotivoECarga,
                    motivoAvaria.PermitirInformarQuantidadeMaiorMercadoriaAvariada,
                    motivoAvaria.DesabilitarBotaoTermo

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
                // Inicia transacao
                unitOfWork.Start();

                // Instancia repositorios
                Repositorio.Embarcador.Avarias.MotivoAvaria repMotivoAvaria = new Repositorio.Embarcador.Avarias.MotivoAvaria(unitOfWork);

                // Busca informacoes
                Dominio.Entidades.Embarcador.Avarias.MotivoAvaria motivoAvaria = new Dominio.Entidades.Embarcador.Avarias.MotivoAvaria();

                // Preenche entidade com dados
                PreencheEntidade(ref motivoAvaria, unitOfWork);

                // Valida entidade
                string erro;
                if (!ValidaEntidade(motivoAvaria, out erro))
                    return new JsonpResult(false, true, erro);

                // Persiste dados
                repMotivoAvaria.Inserir(motivoAvaria, Auditado);
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
                Repositorio.Embarcador.Avarias.MotivoAvaria repMotivoAvaria = new Repositorio.Embarcador.Avarias.MotivoAvaria(unitOfWork);

                // Parametros
                int codigo = 0;
                int.TryParse(Request.Params("Codigo"), out codigo);

                // Busca informacoes
                Dominio.Entidades.Embarcador.Avarias.MotivoAvaria motivoAvaria = repMotivoAvaria.BuscarPorCodigo(codigo, true);

                // Valida
                if (motivoAvaria == null)
                    return new JsonpResult(false, true, "Não foi possível encontrar o registro.");

                // Preenche entidade com dados
                PreencheEntidade(ref motivoAvaria, unitOfWork);

                // Valida entidade
                string erro;
                if (!ValidaEntidade(motivoAvaria, out erro))
                    return new JsonpResult(false, true, erro);

                // Persiste dados
                repMotivoAvaria.Atualizar(motivoAvaria, Auditado);
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
                Repositorio.Embarcador.Avarias.MotivoAvaria repMotivoAvaria = new Repositorio.Embarcador.Avarias.MotivoAvaria(unitOfWork);

                // Parametros
                int codigo = 0;
                int.TryParse(Request.Params("Codigo"), out codigo);

                // Busca informacoes
                Dominio.Entidades.Embarcador.Avarias.MotivoAvaria motivoAvaria = repMotivoAvaria.BuscarPorCodigo(codigo);

                // Valida
                if (motivoAvaria == null)
                    return new JsonpResult(false, true, "Não foi possível encontrar o registro.");

                // Persiste dados
                repMotivoAvaria.Deletar(motivoAvaria, Auditado);
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

        private void PreencheEntidade(ref Dominio.Entidades.Embarcador.Avarias.MotivoAvaria motivoAvaria, Repositorio.UnitOfWork unitOfWork)
        {
            Repositorio.Embarcador.Financeiro.PlanoConta repPlanoConta = new Repositorio.Embarcador.Financeiro.PlanoConta(unitOfWork);
            Repositorio.TipoDeOcorrenciaDeCTe repTipoDeOcorrencia = new Repositorio.TipoDeOcorrenciaDeCTe(unitOfWork);

            int codigoPlanoConta = Request.GetIntParam("ContaContabil");
            int codigoTipoOcorrencia = Request.GetIntParam("TipoOcorrencia");

            Dominio.Entidades.Embarcador.Financeiro.PlanoConta contaContabil = codigoPlanoConta > 0 ? repPlanoConta.BuscarPorCodigo(codigoPlanoConta) : null;
            Dominio.Entidades.TipoDeOcorrenciaDeCTe tipoOcorrencia = codigoTipoOcorrencia > 0 ? repTipoDeOcorrencia.BuscarPorCodigo(codigoTipoOcorrencia) : null;

            string descricao = Request.Params("Descricao");
            if (string.IsNullOrWhiteSpace(descricao)) descricao = string.Empty;

            Dominio.ObjetosDeValor.Embarcador.Enumeradores.ResponsavelAvaria responsavel;
            Enum.TryParse(Request.Params("Responsavel"), out responsavel);

            Dominio.ObjetosDeValor.Embarcador.Enumeradores.FinalidadeMotivoAvaria finalidade;
            Enum.TryParse(Request.Params("Finalidade"), out finalidade);

            bool.TryParse(Request.Params("Status"), out bool ativo);

            string observacao = Request.Params("Observacao");
            if (string.IsNullOrWhiteSpace(observacao)) observacao = string.Empty;

            motivoAvaria.Descricao = descricao;
            motivoAvaria.Responsavel = responsavel;
            motivoAvaria.Finalidade = finalidade;
            motivoAvaria.ContaContabil = contaContabil;
            motivoAvaria.Ativo = ativo;
            motivoAvaria.Observacao = observacao;

            motivoAvaria.TipoOcorrencia = tipoOcorrencia;
            motivoAvaria.GerarOcorrenciaAutomaticamente = Request.GetBoolParam("GerarOcorrenciaAutomaticamente");
            motivoAvaria.ObrigarInformarValorParaLiberarOcorrencia = Request.GetBoolParam("ObrigarInformarValorParaLiberarOcorrencia");
            motivoAvaria.GerarCTeValorAnteriorTratativaReentrega = Request.GetBoolParam("GerarCTeValorAnteriorTratativaReentrega");
            motivoAvaria.CalcularOcorrenciaPorTabelaFrete = Request.GetBoolParam("CalcularOcorrenciaPorTabelaFrete");
            motivoAvaria.ObrigarAnexo = Request.GetBoolParam("ObrigarAnexo");
            motivoAvaria.NaoPermitirAberturaAvariasMesmoMotivoECarga = Request.GetBoolParam("NaoPermitirAberturaAvariasMesmoMotivoECarga");
            motivoAvaria.PermitirInformarQuantidadeMaiorMercadoriaAvariada = Request.GetBoolParam("PermitirInformarQuantidadeMaiorMercadoriaAvariada");
            motivoAvaria.DesabilitarBotaoTermo = Request.GetBoolParam("DesabilitarBotaoTermo");
        }

        private bool ValidaEntidade(Dominio.Entidades.Embarcador.Avarias.MotivoAvaria motivoAvaria, out string msgErro)
        {
            /* ValidaEntidade
             * Recebe uma instancia da entidade
             * Valida informacoes
             * Retorna de entidade e valida ou nao e retorna erro (se tiver)
             */
            msgErro = "";


            if (string.IsNullOrWhiteSpace(motivoAvaria.Descricao))
            {
                msgErro = "Descrição é obrigatória.";
                return false;
            }

            if (motivoAvaria.Descricao.Length > 200)
            {
                msgErro = "Descrição não pode passar de 200 caracteres.";
                return false;
            }

            if (motivoAvaria.Finalidade == Dominio.ObjetosDeValor.Embarcador.Enumeradores.FinalidadeMotivoAvaria.MotivoAvaria && motivoAvaria.ContaContabil == null)
            {
                msgErro = "Nenhuma conta contábil selecionado.";
                return false;
            }

            if (motivoAvaria.Finalidade == Dominio.ObjetosDeValor.Embarcador.Enumeradores.FinalidadeMotivoAvaria.MotivoAvaria && motivoAvaria.Observacao.Length > 2000)
            {
                msgErro = "Observação não pode passar de 2000 caracteres.";
                return false;
            }

            return true;
        }

        private dynamic ExecutaPesquisa(ref int totalRegistros, string propOrdenar, string dirOrdena, int inicio, int limite, Repositorio.UnitOfWork unitOfWork)
        {
            // Instancia repositorios
            Repositorio.Embarcador.Avarias.MotivoAvaria repMotivoAvaria = new Repositorio.Embarcador.Avarias.MotivoAvaria(unitOfWork);

            // Dados do filtro
            Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoAtivoPesquisa status;
            if (!string.IsNullOrWhiteSpace(Request.Params("Status")))
                Enum.TryParse(Request.Params("Status"), out status);
            else
                status = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoAtivoPesquisa.Ativo;

            Dominio.ObjetosDeValor.Embarcador.Enumeradores.FinalidadeMotivoAvaria finalidade;
            Enum.TryParse(Request.Params("Finalidade"), out finalidade);

            string descricao = Request.Params("Descricao");

            // Consulta
            List<Dominio.Entidades.Embarcador.Avarias.MotivoAvaria> listaGrid = repMotivoAvaria.Consultar(descricao, status, finalidade, propOrdenar, dirOrdena, inicio, limite);
            totalRegistros = repMotivoAvaria.ContarConsulta(descricao, status, finalidade);

            var lista = from obj in listaGrid
                        select new
                        {
                            Codigo = obj.Codigo,
                            Descricao = obj.Descricao,
                            DescricaoFinalidade = obj.DescricaoFinalidade,
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

            if (propOrdenar == "DescricaoFinalidade") propOrdenar = "Finalidade";
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
            grid.AdicionarCabecalho("Finalidade", "DescricaoFinalidade", 15, Models.Grid.Align.left, true);
            grid.AdicionarCabecalho("Status", "DescricaoAtivo", 15, Models.Grid.Align.left, true);

            return grid;
        }

        #endregion
    }
}
