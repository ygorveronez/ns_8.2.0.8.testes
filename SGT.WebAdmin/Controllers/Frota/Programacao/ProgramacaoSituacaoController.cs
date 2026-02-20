using SGTAdmin.Controllers;
using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;

namespace SGT.WebAdmin.Controllers.Frota.Programacao
{
    [CustomAuthorize("Frota/ProgramacaoSituacao")]
    public class ProgramacaoSituacaoController : BaseController
    {
		#region Construtores

		public ProgramacaoSituacaoController(Conexao conexao) : base(conexao) { }

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
                Repositorio.Embarcador.Frota.Programacao.ProgramacaoSituacao repProgramacaoSituacao = new Repositorio.Embarcador.Frota.Programacao.ProgramacaoSituacao(unitOfWork);

                // Parametros
                int.TryParse(Request.Params("Codigo"), out int codigo);

                // Busca informacoes
                Dominio.Entidades.Embarcador.Frota.Programacao.ProgramacaoSituacao programacaoSituacao = repProgramacaoSituacao.BuscarPorCodigo(codigo);

                // Valida
                if (programacaoSituacao == null)
                    return new JsonpResult(false, true, "Não foi possível encontrar o registro.");

                // Formata retorno
                var retorno = new
                {
                    programacaoSituacao.Codigo,
                    programacaoSituacao.Descricao,
                    Status = programacaoSituacao.Ativo,
                    programacaoSituacao.TipoEntidadeProgramacao,
                    programacaoSituacao.Cores
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
                Repositorio.Embarcador.Frota.Programacao.ProgramacaoSituacao repProgramacaoSituacao = new Repositorio.Embarcador.Frota.Programacao.ProgramacaoSituacao(unitOfWork);

                // Busca informacoes
                Dominio.Entidades.Embarcador.Frota.Programacao.ProgramacaoSituacao programacaoSituacao = new Dominio.Entidades.Embarcador.Frota.Programacao.ProgramacaoSituacao();

                // Preenche entidade com dados
                PreencheEntidade(ref programacaoSituacao, unitOfWork);

                // Valida entidade
                if (!ValidaEntidade(programacaoSituacao, out string erro))
                    return new JsonpResult(false, true, erro);

                // Persiste dados
                repProgramacaoSituacao.Inserir(programacaoSituacao, Auditado);
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
                Repositorio.Embarcador.Frota.Programacao.ProgramacaoSituacao repProgramacaoSituacao = new Repositorio.Embarcador.Frota.Programacao.ProgramacaoSituacao(unitOfWork);

                // Parametros
                int.TryParse(Request.Params("Codigo"), out int codigo);

                // Busca informacoes
                Dominio.Entidades.Embarcador.Frota.Programacao.ProgramacaoSituacao programacaoSituacao = repProgramacaoSituacao.BuscarPorCodigo(codigo);

                // Valida
                if (programacaoSituacao == null)
                    return new JsonpResult(false, true, "Não foi possível encontrar o registro.");

                // Preenche entidade com dados
                PreencheEntidade(ref programacaoSituacao, unitOfWork);

                // Valida entidade
                if (!ValidaEntidade(programacaoSituacao, out string erro))
                    return new JsonpResult(false, true, erro);

                // Persiste dados
                repProgramacaoSituacao.Atualizar(programacaoSituacao);
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
                Repositorio.Embarcador.Frota.Programacao.ProgramacaoSituacao repProgramacaoSituacao = new Repositorio.Embarcador.Frota.Programacao.ProgramacaoSituacao(unitOfWork);

                // Parametros
                int.TryParse(Request.Params("Codigo"), out int codigo);

                // Busca informacoes
                Dominio.Entidades.Embarcador.Frota.Programacao.ProgramacaoSituacao programacaoSituacao = repProgramacaoSituacao.BuscarPorCodigo(codigo);

                // Valida
                if (programacaoSituacao == null)
                    return new JsonpResult(false, true, "Não foi possível encontrar o registro.");

                // Persiste dados
                repProgramacaoSituacao.Deletar(programacaoSituacao, Auditado);
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
            grid.Prop("Descricao").Nome("Descrição").Tamanho(60).Align(Models.Grid.Align.left);
            grid.Prop("Ativo").Nome("Status").Tamanho(10).Align(Models.Grid.Align.left);

            return grid;
        }

        /* ExecutaPesquisa
         * Converte os dados recebidos e executa a busca
         * Retorna um dynamic pronto para adicionar ao grid
         */
        private dynamic ExecutaPesquisa(ref int totalRegistros, string propOrdenar, string dirOrdena, int inicio, int limite, Repositorio.UnitOfWork unitOfWork)
        {
            // Instancia repositorios
            Repositorio.Embarcador.Frota.Programacao.ProgramacaoSituacao repProgramacaoSituacao = new Repositorio.Embarcador.Frota.Programacao.ProgramacaoSituacao(unitOfWork);

            // Dados do filtro
            List<Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoEntidadeProgramacao> finalidades = new List<Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoEntidadeProgramacao>();
            int nTipoJustificativa;
            if (int.TryParse(Request.Params("Finalidade"), out nTipoJustificativa))
                finalidades.Add((Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoEntidadeProgramacao)nTipoJustificativa);
            else if (!string.IsNullOrWhiteSpace(Request.Params("Finalidade")))
                finalidades = JsonConvert.DeserializeObject<List<Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoEntidadeProgramacao>>(Request.Params("Finalidade"));

            Enum.TryParse(Request.Params("Status"), out Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoAtivoPesquisa status);

            string descricao = Request.Params("Descricao");

            int codigoEmpresa = 0;
            if (TipoServicoMultisoftware == AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiNFe)
                codigoEmpresa = this.Usuario.Empresa.Codigo;

            // Consulta
            List<Dominio.Entidades.Embarcador.Frota.Programacao.ProgramacaoSituacao> listaGrid = repProgramacaoSituacao.Consultar(finalidades, codigoEmpresa, descricao, status, propOrdenar, dirOrdena, inicio, limite);
            totalRegistros = repProgramacaoSituacao.ContarConsulta(finalidades, codigoEmpresa, descricao, status);

            var lista = from obj in listaGrid
                        select new
                        {
                            obj.Codigo,
                            obj.Descricao,
                            Ativo = obj.DescricaoAtivo
                        };

            return lista.ToList();
        }

        /* PreencheEntidade
         * Recebe uma instancia da entidade
         * Converte parametros recebido por request
         * Atribui a entidade
         */
        private void PreencheEntidade(ref Dominio.Entidades.Embarcador.Frota.Programacao.ProgramacaoSituacao programacaoSituacao, Repositorio.UnitOfWork unitOfWork)
        {
            string descricao = Request.Params("Descricao") ?? string.Empty;
            string observacao = Request.Params("Observacao") ?? string.Empty;

            bool.TryParse(Request.Params("Status"), out bool ativo);

            // Vincula dados
            programacaoSituacao.Descricao = descricao;
            programacaoSituacao.TipoEntidadeProgramacao = Request.GetEnumParam<Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoEntidadeProgramacao>("TipoEntidadeProgramacao");
            programacaoSituacao.Cores = Request.GetEnumParam<Dominio.ObjetosDeValor.Embarcador.Enumeradores.Cores>("Cores");
            programacaoSituacao.Ativo = ativo;
            programacaoSituacao.Empresa = this.Usuario.Empresa;
        }

        /* ValidaEntidade
         * Recebe uma instancia da entidade
         * Valida informacoes
         * Retorna de entidade e valida ou nao e retorna erro (se tiver)
         */
        private bool ValidaEntidade(Dominio.Entidades.Embarcador.Frota.Programacao.ProgramacaoSituacao programacaoSituacao, out string msgErro)
        {
            msgErro = "";

            if (string.IsNullOrWhiteSpace(programacaoSituacao.Descricao))
            {
                msgErro = "Descrição é obrigatória.";
                return false;
            }

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
