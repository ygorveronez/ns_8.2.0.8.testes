using SGTAdmin.Controllers;
using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Mvc;

namespace SGT.WebAdmin.Controllers.ICMS
{
    [CustomAuthorize("ICMS/AliquotaICMS")]
    public class AliquotaICMSController : BaseController
    {
		#region Construtores

		public AliquotaICMSController(Conexao conexao) : base(conexao) { }

		#endregion


        #region Métodos Globais
        [AllowAuthenticate]
        public async Task<IActionResult> Pesquisa()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);
            try
            {
                // Manipula grids
                Models.Grid.Grid grid = GridPesquisa();

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

        public async Task<IActionResult> BuscarPorCodigo()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);
            try
            {
                // Instancia repositorios
                Repositorio.Aliquota repAliquotaICMS = new Repositorio.Aliquota(unitOfWork);

                // Parametros
                int.TryParse(Request.Params("Codigo"), out int codigo);

                // Busca informacoes
                Dominio.Entidades.Aliquota condicao = repAliquotaICMS.BuscarPorCodigo(codigo);

                // Valida
                if (condicao == null)
                    return new JsonpResult(false, true, "Não foi possível encontrar o registro.");

                // Formata retorno
                var retorno = new
                {
                    condicao.Codigo,
                    EstadoEmpresa = condicao.EstadoEmpresa?.Sigla ?? "",
                    EstadoOrigem = condicao.EstadoOrigem?.Sigla ?? "",
                    EstadoDestino = condicao.EstadoDestino?.Sigla ?? "",
                    AtividadeTomador = condicao.AtividadeTomador?.Descricao ?? "",
                    AtividadeDestinatario = condicao.AtividadeDestinatario?.Descricao ?? "",
                    CFOP = condicao.CFOP?.CodigoCFOP ?? 0,
                    condicao.Percentual,
                    condicao.CST,
                    condicao.Observacao,
                    condicao.AliquotaFCP
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
                return new JsonpResult(false, "Não é possível inserir uma nova Alíquota.");

                //// Inicia transacao
                //unitOfWork.Start();

                //// Instancia repositorios
                //Repositorio.Embarcador.ICMS.AliquotaICMS repAliquotaICMS = new Repositorio.Embarcador.ICMS.AliquotaICMS(unitOfWork);

                //// Busca informacoes
                //Dominio.Entidades.Embarcador.ICMS.AliquotaICMS condicao = new Dominio.Entidades.Embarcador.ICMS.AliquotaICMS();

                //// Preenche entidade com dados
                //PreencheEntidade(ref condicao, unitOfWork);

                //// Valida entidade
                //if (!ValidaEntidade(condicao, out string erro))
                //    return new JsonpResult(false, true, erro);

                //// Persiste dados
                //repAliquotaICMS.Inserir(condicao, Auditado);
                //unitOfWork.CommitChanges();

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
                Repositorio.Aliquota repAliquotaICMS = new Repositorio.Aliquota(unitOfWork);

                // Parametros
                int.TryParse(Request.Params("Codigo"), out int codigo);

                // Busca informacoes
                Dominio.Entidades.Aliquota aliquota = repAliquotaICMS.BuscarPorCodigo(codigo, true);

                // Valida
                if (aliquota == null)
                    return new JsonpResult(false, true, "Não foi possível encontrar o registro.");

                // Preenche entidade com dados
                PreencheEntidade(ref aliquota, unitOfWork);

                // Valida entidade
                if (!ValidaEntidade(aliquota, out string erro))
                    return new JsonpResult(false, true, erro);

                // Persiste dados
                repAliquotaICMS.Atualizar(aliquota, Auditado);
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
                return new JsonpResult(false, "Não é possível excluir uma alíquota de ICMS.");
                //// Inicia transacao
                //unitOfWork.Start();

                //// Instancia repositorios
                //Repositorio.Embarcador.ICMS.AliquotaICMS repAliquotaICMS = new Repositorio.Embarcador.ICMS.AliquotaICMS(unitOfWork);

                //// Parametros
                //int.TryParse(Request.Params("Codigo"), out int codigo);

                //// Busca informacoes
                //Dominio.Entidades.Embarcador.ICMS.AliquotaICMS condicao = repAliquotaICMS.BuscarPorCodigo(codigo);

                //// Valida
                //if (condicao == null)
                //    return new JsonpResult(false, true, "Não foi possível encontrar o registro.");

                //// Persiste dados
                //repAliquotaICMS.Deletar(condicao, Auditado);
                //unitOfWork.CommitChanges();

                //// Retorna informacoes
                //return new JsonpResult(true);
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


        /* GridPesquisa
         * Retorna o model de Grid para a o módulo
         */
        private Models.Grid.Grid GridPesquisa()
        {
            // Manipula grids
            Models.Grid.Grid grid = new Models.Grid.Grid(Request)
            {
                header = new List<Models.Grid.Head>()
            };

            // Cabecalhos grid
            grid.Prop("Codigo");
            grid.Prop("EstadoEmpresa").Nome("UF Empresa").Tamanho(10).Align(Models.Grid.Align.left);
            grid.Prop("EstadoOrigem").Nome("UF Origem").Tamanho(10).Align(Models.Grid.Align.left);
            grid.Prop("EstadoDestino").Nome("UF Destino").Tamanho(10).Align(Models.Grid.Align.left);
            grid.Prop("Percentual").Nome("Percentual").Tamanho(15).Align(Models.Grid.Align.left);

            return grid;
        }

        /* ExecutaPesquisa
         * Converte os dados recebidos e executa a busca
         * Retorna um dynamic pronto para adicionar ao grid
         */
        private dynamic ExecutaPesquisa(ref int totalRegistros, string propOrdenar, string dirOrdena, int inicio, int limite, Repositorio.UnitOfWork unitOfWork)
        {
            // Instancia repositorios
            Repositorio.Aliquota repAliquotaICMS = new Repositorio.Aliquota(unitOfWork);            

            string ufEmpresa = Request.GetStringParam("EstadoEmpresa");
            string ufOrigem = Request.GetStringParam("EstadoOrigem");
            string ufDestino = Request.GetStringParam("EstadoDestino");
            string atividadeTomador = Request.GetStringParam("AtividadeTomador");
            int CFOP = Request.GetIntParam("CFOP");

            int codigoEmpresa = 0;
            if (TipoServicoMultisoftware == AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiNFe)
                codigoEmpresa = this.Usuario.Empresa.Codigo;

            // Consulta
            List<Dominio.Entidades.Aliquota> listaGrid = repAliquotaICMS.Consultar(ufEmpresa, ufOrigem, ufDestino, atividadeTomador, CFOP, propOrdenar, dirOrdena, inicio, limite);
            totalRegistros = repAliquotaICMS.ContarConsulta(ufEmpresa, ufOrigem, ufDestino, atividadeTomador, CFOP);

            var lista = from obj in listaGrid
                        select new
                        {
                            obj.Codigo,
                            EstadoEmpresa = obj.EstadoEmpresa.Sigla,
                            EstadoOrigem = obj.EstadoOrigem.Sigla,
                            EstadoDestino = obj.EstadoDestino.Sigla,
                            Percentual = obj.Percentual.ToString("n2")
                        };

            return lista.ToList();
        }

        /* PreencheEntidade
         * Recebe uma instancia da entidade
         * Converte parametros recebido por request
         * Atribui a entidade
         */
        private void PreencheEntidade(ref Dominio.Entidades.Aliquota aliquota, Repositorio.UnitOfWork unitOfWork)
        {
            string descricao = Request.Params("Descricao") ?? string.Empty;
            string observacao = Request.Params("Observacao") ?? string.Empty;

            decimal.TryParse(Request.Params("Percentual"), out decimal percentual);

            // Vincula dados
            aliquota.Percentual = percentual;
            aliquota.AliquotaFCP = Request.GetDecimalParam("AliquotaFCP");
            aliquota.CST = Request.GetStringParam("CST");
        }

        /* ValidaEntidade
         * Recebe uma instancia da entidade
         * Valida informacoes
         * Retorna de entidade e valida ou nao e retorna erro (se tiver)
         */
        private bool ValidaEntidade(Dominio.Entidades.Aliquota aliquota, out string msgErro)
        {
            msgErro = "";

            //if (string.IsNullOrWhiteSpace(condicao.Descricao))
            //{
            //    msgErro = "Descrição é obrigatória.";
            //    return false;
            //}

            return true;
        }

        /* PropOrdena
         * Recebe o campo ordenado na grid
         * Retorna o elemento especifico da entidade para ordenacao
         */
        private void PropOrdena(ref string propOrdenar)
        {
        }
        #endregion
    }
}
