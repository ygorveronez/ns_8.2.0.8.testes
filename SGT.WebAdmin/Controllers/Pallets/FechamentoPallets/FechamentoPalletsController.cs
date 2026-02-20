using SGTAdmin.Controllers;
using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Mvc;

namespace SGT.WebAdmin.Controllers.Pallets
{
    [CustomAuthorize("Pallets/FechamentoPallets")]
    public class FechamentoPalletsController : BaseController
    {
		#region Construtores

		public FechamentoPalletsController(Conexao conexao) : base(conexao) { }

		#endregion


        #region Métodos Globais

        public async Task<IActionResult> BuscarPorCodigo()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);
            try
            {
                // Instancia repositorios
                Repositorio.Embarcador.Pallets.FechamentoPallets repFechamentoPallets = new Repositorio.Embarcador.Pallets.FechamentoPallets(unitOfWork);

                // Parametros
                int codigo = Request.GetIntParam("Codigo");

                // Busca informacoes
                Dominio.Entidades.Embarcador.Pallets.FechamentoPallets fechamento = repFechamentoPallets.BuscarPorCodigo(codigo);

                // Valida
                if (fechamento == null)
                    return new JsonpResult(false, true, "Não foi possível encontrar o registro.");

                // Formata retorno
                var retorno = new
                {
                    fechamento.Codigo,
                    fechamento.Situacao,

                    DadosFechamento = new
                    {
                        DataInicio = fechamento.DataInicial.ToString("dd/MM/yyyy"),
                        DataFim = fechamento.DataFinal.ToString("dd/MM/yyyy"),
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

        public async Task<IActionResult> ExcluirPorCodigo()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                int codigo = Request.GetIntParam("Codigo");
                Repositorio.Embarcador.Pallets.FechamentoPallets repositorioFechamentoPallets = new Repositorio.Embarcador.Pallets.FechamentoPallets(unitOfWork);
                Dominio.Entidades.Embarcador.Pallets.FechamentoPallets fechamento = repositorioFechamentoPallets.BuscarPorCodigo(codigo);

                if (fechamento == null)
                    return new JsonpResult(false, true, "Não foi possível encontrar o registro.");

                if (fechamento.Situacao != Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoFechamentoPallets.Finalizado)
                    return new JsonpResult(false, true, "A situação do fechamento não permite a remoção.");

                unitOfWork.Start();

                new Servicos.Embarcador.Pallets.Fechamento(unitOfWork).CancelarFinalizacaoFechamento(fechamento);

                repositorioFechamentoPallets.Deletar(fechamento, Auditado);

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

        public async Task<IActionResult> Finalizar()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);
            try
            {
                // Instancia repositorios
                Repositorio.Embarcador.Pallets.FechamentoPallets repFechamentoPallets = new Repositorio.Embarcador.Pallets.FechamentoPallets(unitOfWork);

                // Parametros
                int codigo = Request.GetIntParam("Codigo");

                // Busca informacoes
                Dominio.Entidades.Embarcador.Pallets.FechamentoPallets fechamento = repFechamentoPallets.BuscarPorCodigo(codigo);

                // Valida
                if (fechamento == null)
                    return new JsonpResult(false, true, "Não foi possível encontrar o registro.");

                if (fechamento.Situacao != Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoFechamentoPallets.Aberto)
                    return new JsonpResult(false, true, "Só é possível finalizar fechamentos abertos.");

                // Preenche entidade com dados
                fechamento.Situacao = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoFechamentoPallets.Finalizado;
                fechamento.DataFinalizacao = DateTime.Now;

                // Persiste dados
                unitOfWork.Start();

                repFechamentoPallets.Atualizar(fechamento);
                new Servicos.Embarcador.Pallets.Fechamento(unitOfWork).FinalizarFechamento(fechamento);
                Servicos.Auditoria.Auditoria.Auditar(Auditado, fechamento, "Finalizou o fechamento", unitOfWork);

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

        public async Task<IActionResult> GerarFechamento()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);
            try
            {
                // Instancia repositorios
                Repositorio.Embarcador.Pallets.FechamentoPallets repFechamentoPallets = new Repositorio.Embarcador.Pallets.FechamentoPallets(unitOfWork);

                // Busca informacoes
                Dominio.Entidades.Embarcador.Pallets.FechamentoPallets fechamento = new Dominio.Entidades.Embarcador.Pallets.FechamentoPallets();

                // Preenche entidade com dados
                PreencheEntidade(ref fechamento, unitOfWork);

                // Valida entidade
                if (!ValidaEntidade(fechamento, out string erro))
                    return new JsonpResult(false, true, erro);

                unitOfWork.Start();

                repFechamentoPallets.Inserir(fechamento, Auditado);
                new Servicos.Embarcador.Pallets.Fechamento(unitOfWork).GerarComposicaoFechamento(fechamento);

                unitOfWork.CommitChanges();

                // Retorna sucesso
                return new JsonpResult(new {
                    Codigo = fechamento.Codigo
                });
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
            grid.Prop("Numero").Nome("Número").Tamanho(10).Align(Models.Grid.Align.right);
            grid.Prop("DataInicial").Nome("Data Inicial").Tamanho(15).Align(Models.Grid.Align.center);
            grid.Prop("DataFinal").Nome("Data Final").Tamanho(15).Align(Models.Grid.Align.center);
            grid.Prop("Situacao").Nome("Situação").Tamanho(16).Align(Models.Grid.Align.left);

            return grid;
        }

        /* ExecutaPesquisa
         * Converte os dados recebidos e executa a busca
         * Retorna um dynamic pronto para adicionar ao grid
         */
        private dynamic ExecutaPesquisa(ref int totalRegistros, string propOrdenar, string dirOrdena, int inicio, int limite, Repositorio.UnitOfWork unitOfWork)
        {
            // Instancia repositorios
            Repositorio.Embarcador.Pallets.FechamentoPallets repFechamentoPallets = new Repositorio.Embarcador.Pallets.FechamentoPallets(unitOfWork);

            // Dados do filtro
            int numero = Request.GetIntParam("Numero");
            DateTime dataInicio = Request.GetDateTimeParam("DataInicio");
            DateTime dataFinal = Request.GetDateTimeParam("DataFim");
            Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoFechamentoPallets? situacao = Request.GetNullableEnumParam<Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoFechamentoPallets>("Situacao");

            // Consulta
            List<Dominio.Entidades.Embarcador.Pallets.FechamentoPallets> listaGrid = repFechamentoPallets.Consultar(numero, dataInicio, dataFinal, situacao, propOrdenar, dirOrdena, inicio, limite);
            totalRegistros = repFechamentoPallets.ContarConsulta(numero, dataInicio, dataFinal, situacao);

            var lista = from obj in listaGrid
                        select new
                        {
                            obj.Codigo,
                            obj.Numero,
                            DataInicial = obj.DataInicial.ToString("dd/MM/yyyy"),
                            DataFinal = obj.DataFinal.ToString("dd/MM/yyyy"),
                            Situacao = obj.SituacaoDescricao,
                        };

            return lista.ToList();
        }

        /* PreencheEntidade
         * Recebe uma instancia da entidade
         * Converte parametros recebido por request
         * Atribui a entidade
         */
        private void PreencheEntidade(ref Dominio.Entidades.Embarcador.Pallets.FechamentoPallets fechamento, Repositorio.UnitOfWork unitOfWork)
        {
            Repositorio.Embarcador.Pallets.FechamentoPallets repFechamentoPallets = new Repositorio.Embarcador.Pallets.FechamentoPallets(unitOfWork);

            // Vincula dados
            fechamento.DataInicial = Request.GetDateTimeParam("DataInicio");
            fechamento.DataFinal = Request.GetDateTimeParam("DataFim");
            fechamento.DataCriacao = DateTime.Now;
            fechamento.Numero = repFechamentoPallets.BuscarProximoNumero();
            fechamento.Situacao = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoFechamentoPallets.Aberto;
        }

        /* ValidaEntidade
         * Recebe uma instancia da entidade
         * Valida informacoes
         * Retorna de entidade e valida ou nao e retorna erro (se tiver)
         */
        private bool ValidaEntidade(Dominio.Entidades.Embarcador.Pallets.FechamentoPallets fechamento, out string msgErro)
        {
            msgErro = "";

            if (fechamento.DataInicial == DateTime.MinValue)
            {
                msgErro = "Data Inicial é obrigatório.";
                return false;
            }

            if (fechamento.DataFinal == DateTime.MinValue)
            {
                msgErro = "Data Final é obrigatório.";
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
            //if (propOrdenar == "Relacional") propOrdenar = "Relacional.Codigo";
        }
        #endregion
    }
}
