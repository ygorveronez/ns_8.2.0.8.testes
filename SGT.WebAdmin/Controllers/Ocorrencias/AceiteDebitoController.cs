using SGTAdmin.Controllers;
using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Mvc;

namespace SGT.WebAdmin.Controllers.Ocorrencia
{
    [CustomAuthorize("Ocorrencias/AceiteDebito")]
    public class AceiteDebitoController : BaseController
    {
		#region Construtores

		public AceiteDebitoController(Conexao conexao) : base(conexao) { }

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

        [AllowAuthenticate]
        public async Task<IActionResult> DetalhesAceite()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);
            try
            {
                // Instancia repositorios
                Repositorio.Embarcador.Ocorrencias.AceiteDebito repAceiteDebito = new Repositorio.Embarcador.Ocorrencias.AceiteDebito(unitOfWork);

                // Parametros
                int.TryParse(Request.Params("Codigo"), out int ocorrencia);

                // Busca informacoes
                Dominio.Entidades.Embarcador.Ocorrencias.AceiteDebito aceite = repAceiteDebito.BuscarPorOcorrencia(ocorrencia);

                // Valida
                if (aceite == null)
                    return new JsonpResult(false, true, "Não foi possível encontrar o registro.");

                // Formata retorno
                var retorno = new
                {
                    aceite.Codigo,
                    Usuario = aceite.Usuario?.Nome ?? string.Empty,
                    aceite.Observacao,
                    DataRetorno = aceite.DataRetorno?.ToString("dd/MM/yyyy") ?? string.Empty,
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

        public async Task<IActionResult> BuscarPorCodigo()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);
            try
            {
                // Instancia repositorios
                Repositorio.Embarcador.Ocorrencias.AceiteDebito repAceiteDebito = new Repositorio.Embarcador.Ocorrencias.AceiteDebito(unitOfWork);

                // Parametros
                int.TryParse(Request.Params("Codigo"), out int codigo);

                // Busca informacoes
                Dominio.Entidades.Embarcador.Ocorrencias.AceiteDebito aceite = repAceiteDebito.BuscarPorCodigo(codigo);

                // Valida
                if (aceite == null)
                    return new JsonpResult(false, true, "Não foi possível encontrar o registro.");

                // Formata retorno
                var retorno = new
                {
                    aceite.Codigo,
                    aceite.Situacao,
                    aceite.Observacao,
                    Ocorrencia = aceite.Ocorrencia.Codigo,
                    DetalhesOcorrencia = new {
                        Numero = aceite.Ocorrencia.NumeroOcorrencia,
                        DataOcorrencia = aceite.Ocorrencia.DataOcorrencia.ToString("dd/MM/yyyy HH:mm"),
                        TipoOcorrencia = aceite.Ocorrencia.TipoOcorrencia.Descricao,
                        Observacao = aceite.Ocorrencia.Observacao,
                    }
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

        public async Task<IActionResult> ResponderAceite()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);
            try
            {
                // Inicia transacao
                unitOfWork.Start();

                // Instancia repositorios
                Repositorio.Embarcador.Ocorrencias.AceiteDebito repAceiteDebito = new Repositorio.Embarcador.Ocorrencias.AceiteDebito(unitOfWork);

                // Parametros
                int.TryParse(Request.Params("Codigo"), out int codigo);

                // Busca informacoes
                Dominio.Entidades.Embarcador.Ocorrencias.AceiteDebito aceite = repAceiteDebito.BuscarPorCodigo(codigo);

                // Valida
                if (aceite == null)
                    return new JsonpResult(false, true, "Não foi possível encontrar o registro.");

                if (aceite.Situacao != Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoAceiteDebito.AgAceite)
                    return new JsonpResult(false, true, "O aceite já foi respondido.");

                // Preenche entidade com dados
                PreencheEntidade(ref aceite, unitOfWork);

                // Valida entidade
                if (!ValidaEntidade(aceite, out string erro))
                    return new JsonpResult(false, true, erro);

                // Persiste dados
                repAceiteDebito.Atualizar(aceite);

                Dominio.Entidades.Embarcador.Ocorrencias.CargaOcorrencia ocorrencia = aceite.Ocorrencia;
                string msg = (aceite.Situacao == Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoAceiteDebito.Aprovado ? "Aprovou" : "Rejeitou") + " a nota de débito.";

                Servicos.Embarcador.Carga.Ocorrencia.AvancarEtapaOcorrenciaPosEmissao(ref ocorrencia, Auditado, TipoServicoMultisoftware, unitOfWork, Cliente);
                Servicos.Auditoria.Auditoria.Auditar(Auditado, ocorrencia, null, msg, unitOfWork);
                Servicos.Auditoria.Auditoria.Auditar(Auditado, aceite, null, msg, unitOfWork);

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
            grid.Prop("Ocorrencia").Nome("Ocorrência").Tamanho(15).Align(Models.Grid.Align.left);
            grid.Prop("DataCriacao").Nome("Data Criação").Tamanho(10).Align(Models.Grid.Align.center);
            grid.Prop("DataRetorno").Nome("Data Retorno").Tamanho(10).Align(Models.Grid.Align.center);
            grid.Prop("Situacao").Nome("Situação").Tamanho(15).Align(Models.Grid.Align.left);

            return grid;
        }

        /* ExecutaPesquisa
         * Converte os dados recebidos e executa a busca
         * Retorna um dynamic pronto para adicionar ao grid
         */
        private dynamic ExecutaPesquisa(ref int totalRegistros, string propOrdenar, string dirOrdena, int inicio, int limite, Repositorio.UnitOfWork unitOfWork)
        {
            // Instancia repositorios
            Repositorio.Embarcador.Ocorrencias.AceiteDebito repAceiteDebito = new Repositorio.Embarcador.Ocorrencias.AceiteDebito(unitOfWork);

            // Dados do filtro
            int.TryParse(Request.Params("NumeroOcorrencia"), out int numeroOcorrencia);

            DateTime.TryParse(Request.Params("DataCriacaoInicio"), out DateTime dataInicio);
            DateTime.TryParse(Request.Params("DataCriacaoFim"), out DateTime dataFim);

            Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoAceiteDebito? situacao = null;
            if(Enum.TryParse(Request.Params("Situacao"), out Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoAceiteDebito situacaoAux))
                situacao = situacaoAux;

            int codigoTransportador = 0;
            if (TipoServicoMultisoftware == AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiCTe)
                codigoTransportador = Usuario.Empresa.Codigo;

            // Consulta
            List<Dominio.Entidades.Embarcador.Ocorrencias.AceiteDebito> listaGrid = repAceiteDebito.Consultar(numeroOcorrencia, dataInicio, dataFim, situacao, codigoTransportador, propOrdenar, dirOrdena, inicio, limite);
            totalRegistros = repAceiteDebito.ContarConsulta(numeroOcorrencia, dataInicio, dataFim, situacao, codigoTransportador);

            var lista = from obj in listaGrid
                        select new
                        {
                            Codigo = obj.Codigo,
                            Ocorrencia = obj.Ocorrencia.NumeroOcorrencia.ToString(),
                            DataCriacao = obj.DataCriacao.ToString("dd/MM/yyyy"),
                            DataRetorno = obj.DataRetorno?.ToString("dd/MM/yyyy") ?? string.Empty,
                            Situacao = obj.DescricaoSituacao
                        };

            return lista.ToList();
        }

        /* PreencheEntidade
         * Recebe uma instancia da entidade
         * Converte parametros recebido por request
         * Atribui a entidade
         */
        private void PreencheEntidade(ref Dominio.Entidades.Embarcador.Ocorrencias.AceiteDebito aceite, Repositorio.UnitOfWork unitOfWork)
        {
            string observacao = Request.Params("Observacao") ?? string.Empty;
            bool.TryParse(Request.Params("Resposta"), out bool resposta);

            // Vincula dados
            aceite.Usuario = this.Usuario;
            aceite.DataRetorno = DateTime.Now;
            aceite.Observacao = observacao;

            if (resposta)
                aceite.Situacao = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoAceiteDebito.Aprovado;
            else
                aceite.Situacao = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoAceiteDebito.Rejeitado;
        }

        /* ValidaEntidade
         * Recebe uma instancia da entidade
         * Valida informacoes
         * Retorna de entidade e valida ou nao e retorna erro (se tiver)
         */
        private bool ValidaEntidade(Dominio.Entidades.Embarcador.Ocorrencias.AceiteDebito aceite, out string msgErro)
        {
            msgErro = "";

            if (aceite.Situacao == Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoAceiteDebito.Rejeitado && string.IsNullOrWhiteSpace(aceite.Observacao))
            {
                msgErro = "Observação é obrigatório.";
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
            if (propOrdenar == "Ocorrencia") propOrdenar = "Ocorrencia.NumeroOcorrencia";
        }
        #endregion
    }
}
