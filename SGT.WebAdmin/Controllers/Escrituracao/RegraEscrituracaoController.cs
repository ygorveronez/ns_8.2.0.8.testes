using SGTAdmin.Controllers;
using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Mvc;

namespace SGT.WebAdmin.Controllers.Escrituracao
{
    [CustomAuthorize("Escrituracao/RegraEscrituracao")]
    public class RegraEscrituracaoController : BaseController
    {
		#region Construtores

		public RegraEscrituracaoController(Conexao conexao) : base(conexao) { }

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
                Repositorio.Embarcador.Escrituracao.RegraEscrituracao repRegraEscrituracao = new Repositorio.Embarcador.Escrituracao.RegraEscrituracao(unitOfWork);

                // Parametros
                int codigo = Request.GetIntParam("Codigo");

                // Busca informacoes
                Dominio.Entidades.Embarcador.Escrituracao.RegraEscrituracao regra = repRegraEscrituracao.BuscarPorCodigo(codigo);

                // Valida
                if (regra == null)
                    return new JsonpResult(false, true, "Não foi possível encontrar o registro.");

                // Formata retorno
                var retorno = new
                {
                    regra.Codigo,
                    regra.Descricao,
                    Status = regra.Ativo,
                    Remetente = regra.Remetente != null ? new { regra.Remetente.Codigo, regra.Remetente.Descricao } : null,
                    Destinatario = regra.Destinatario != null ? new { regra.Destinatario.Codigo, regra.Destinatario.Descricao } : null,
                    regra.OrigemFilial,
                    regra.DestinoFilial,
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
                Repositorio.Embarcador.Escrituracao.RegraEscrituracao repRegraEscrituracao = new Repositorio.Embarcador.Escrituracao.RegraEscrituracao(unitOfWork);

                // Busca informacoes
                Dominio.Entidades.Embarcador.Escrituracao.RegraEscrituracao regra = new Dominio.Entidades.Embarcador.Escrituracao.RegraEscrituracao();

                // Preenche entidade com dados
                PreencheEntidade(ref regra, unitOfWork);

                // Valida entidade
                if (!ValidaEntidade(regra, out string erro))
                    return new JsonpResult(false, true, erro);

                // Persiste dados
                repRegraEscrituracao.Inserir(regra, Auditado);
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
                Repositorio.Embarcador.Escrituracao.RegraEscrituracao repRegraEscrituracao = new Repositorio.Embarcador.Escrituracao.RegraEscrituracao(unitOfWork);

                // Parametros
                int codigo = Request.GetIntParam("Codigo");

                // Busca informacoes
                Dominio.Entidades.Embarcador.Escrituracao.RegraEscrituracao regra = repRegraEscrituracao.BuscarPorCodigo(codigo, true);

                // Valida
                if (regra == null)
                    return new JsonpResult(false, true, "Não foi possível encontrar o registro.");

                // Preenche entidade com dados
                PreencheEntidade(ref regra, unitOfWork);

                // Valida entidade
                if (!ValidaEntidade(regra, out string erro))
                    return new JsonpResult(false, true, erro);

                // Persiste dados
                repRegraEscrituracao.Atualizar(regra, Auditado);
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
                Repositorio.Embarcador.Escrituracao.RegraEscrituracao repRegraEscrituracao = new Repositorio.Embarcador.Escrituracao.RegraEscrituracao(unitOfWork);

                // Parametros
                int codigo = Request.GetIntParam("Codigo");

                // Busca informacoes
                Dominio.Entidades.Embarcador.Escrituracao.RegraEscrituracao regra = repRegraEscrituracao.BuscarPorCodigo(codigo);

                // Valida
                if (regra == null)
                    return new JsonpResult(false, true, "Não foi possível encontrar o registro.");

                // Persiste dados
                repRegraEscrituracao.Deletar(regra, Auditado);
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
            grid.Prop("Descricao").Nome("Descrição").Tamanho(30).Align(Models.Grid.Align.left);
            grid.Prop("Remetente").Nome("Remetente").Tamanho(30).Align(Models.Grid.Align.left);
            grid.Prop("Destinatario").Nome("Destinatário").Tamanho(30).Align(Models.Grid.Align.left);
            grid.Prop("Origem").Nome("Origem").Tamanho(10).Align(Models.Grid.Align.left);
            grid.Prop("Destino").Nome("Destino").Tamanho(10).Align(Models.Grid.Align.left);
            grid.Prop("Status").Nome("Status").Tamanho(7).Align(Models.Grid.Align.left);
            

            return grid;
        }

        /* ExecutaPesquisa
         * Converte os dados recebidos e executa a busca
         * Retorna um dynamic pronto para adicionar ao grid
         */
        private dynamic ExecutaPesquisa(ref int totalRegistros, string propOrdenar, string dirOrdena, int inicio, int limite, Repositorio.UnitOfWork unitOfWork)
        {
            // Instancia repositorios
            Repositorio.Embarcador.Escrituracao.RegraEscrituracao repRegraEscrituracao = new Repositorio.Embarcador.Escrituracao.RegraEscrituracao(unitOfWork);

            // Dados do filtro
            string descricao = Request.GetStringParam("Descricao");
            Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoAtivoPesquisa status = Request.GetEnumParam<Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoAtivoPesquisa>("Status");

            // Consulta
            List<Dominio.Entidades.Embarcador.Escrituracao.RegraEscrituracao> listaGrid = repRegraEscrituracao.Consultar(descricao, status, propOrdenar, dirOrdena, inicio, limite);
            totalRegistros = repRegraEscrituracao.ContarConsulta(descricao, status);

            var lista = from obj in listaGrid
                        select new
                        {
                            obj.Codigo,
                            obj.Descricao,
                            Remetente = obj.Remetente?.Descricao ?? string.Empty,
                            Destinatario = obj.Destinatario?.Descricao ?? string.Empty,
                            Origem = obj.OrigemFilial ? "Filial" : "Não Filial",
                            Destino = obj.DestinoFilial ? "Filial" : "Não Filial",
                            Status = obj.Ativo ? "Ativo" : "Inativo",
                        };

            return lista.ToList();
        }

        /* PreencheEntidade
         * Recebe uma instancia da entidade
         * Converte parametros recebido por request
         * Atribui a entidade
         */
        private void PreencheEntidade(ref Dominio.Entidades.Embarcador.Escrituracao.RegraEscrituracao regra, Repositorio.UnitOfWork unitOfWork)
        {
            // Instancia Repositorios
            Repositorio.Cliente repCliente = new Repositorio.Cliente(unitOfWork);

            // Vincula dados
            regra.Descricao = Request.GetStringParam("Descricao");
            regra.Remetente = Request.GetDoubleParam("Remetente") > 0 ? repCliente.BuscarPorCPFCNPJ(Request.GetDoubleParam("Remetente")) : null;
            regra.Destinatario = Request.GetDoubleParam("Destinatario") > 0 ? repCliente.BuscarPorCPFCNPJ(Request.GetDoubleParam("Destinatario")) : null;
            regra.OrigemFilial = Request.GetBoolParam("OrigemFilial");
            regra.DestinoFilial = Request.GetBoolParam("DestinoFilial");
            regra.Ativo = Request.GetBoolParam("Status");
        }

        /* ValidaEntidade
         * Recebe uma instancia da entidade
         * Valida informacoes
         * Retorna de entidade e valida ou nao e retorna erro (se tiver)
         */
        private bool ValidaEntidade(Dominio.Entidades.Embarcador.Escrituracao.RegraEscrituracao regra, out string msgErro)
        {
            msgErro = "";

            if (regra.Descricao == null)
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
            if (propOrdenar == "Origem" || propOrdenar == "Destino") propOrdenar += "Filial";
            else if (propOrdenar == "Remetente" || propOrdenar == "Destinatario") propOrdenar += ".Nome";
            else if (propOrdenar == "Status") propOrdenar = "Ativo";
        }
        #endregion
    }
}
