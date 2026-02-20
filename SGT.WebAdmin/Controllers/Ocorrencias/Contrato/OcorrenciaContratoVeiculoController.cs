using SGTAdmin.Controllers;
using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Mvc;

namespace SGT.WebAdmin.Controllers.Ocorrencias
{
    [CustomAuthorize(new string[] { "Pesquisa", "ConsultaVeiculos" }, "Ocorrencias/Ocorrencia", "Ocorrencias/AutorizacaoOcorrencia")]
    public class OcorrenciaContratoVeiculoController : BaseController
    {
		#region Construtores

		public OcorrenciaContratoVeiculoController(Conexao conexao) : base(conexao) { }

		#endregion


        #region Métodos Globais
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

        public async Task<IActionResult> ConsultaVeiculos()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);
            try
            {
                // Busca Dados
                int totalRegistros = 0;
                var lista = ExecutaPesquisa(ref totalRegistros, "", "", 0, 0, unitOfWork);

                return new JsonpResult(lista);
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
            grid.AdicionarCabecalho("Codigo", false);
            grid.AdicionarCabecalho("CodigoVeiculo", false);
            grid.AdicionarCabecalho("QuantidadeDocumentos", false);
            grid.AdicionarCabecalho("ValorDocumentos", false);
            grid.AdicionarCabecalho("Veículo", "Veiculo", 15, Models.Grid.Align.center, true);
            grid.AdicionarCabecalho("Valor Diária", "ValorDiaria", 20, Models.Grid.Align.right, true);
            grid.AdicionarCabecalho("Valor Quinzena", "ValorQuinzena", 20, Models.Grid.Align.right, true);
            grid.AdicionarCabecalho("Dias", "QuantidadeDias", 15, Models.Grid.Align.right, true);
            grid.AdicionarCabecalho("Total", "Total", 15, Models.Grid.Align.right, true);

            return grid;
        }

        /* ExecutaPesquisa
         * Converte os dados recebidos e executa a busca
         * Retorna um dynamic pronto para adicionar ao grid
         */
        private dynamic ExecutaPesquisa(ref int totalRegistros, string propOrdenar, string dirOrdena, int inicio, int limite, Repositorio.UnitOfWork unitOfWork)
        {
            // Instancia repositorios
            Repositorio.Embarcador.Ocorrencias.OcorrenciaContratoVeiculo repOcorrenciaContratoVeiculo = new Repositorio.Embarcador.Ocorrencias.OcorrenciaContratoVeiculo(unitOfWork);
            Repositorio.Embarcador.Frete.ContratoFreteTransportadorVeiculo repContratoFreteVeiculo = new Repositorio.Embarcador.Frete.ContratoFreteTransportadorVeiculo(unitOfWork);
            Repositorio.Embarcador.Frete.ContratoFreteTransportador repContratoFreteTransportador = new Repositorio.Embarcador.Frete.ContratoFreteTransportador(unitOfWork);
            Repositorio.Empresa repEmpresa = new Repositorio.Empresa(unitOfWork);

            // Dados do filtro
            DateTime.TryParse(Request.Params("PeriodoInicio"), out DateTime periodoInicial);
            DateTime.TryParse(Request.Params("PeriodoFim"), out DateTime periodoFinal);

            int.TryParse(Request.Params("Ocorrencia"), out int ocorrencia);
            int.TryParse(Request.Params("TipoOcorrencia"), out int tipoOcorrencia);

            // Consulta
            Dominio.Entidades.Empresa empresa = repEmpresa.BuscarPorCNPJ(this.Usuario.ClienteTerceiro?.CPF_CNPJ_SemFormato ?? string.Empty);
            int transportador = empresa?.Codigo ?? 0;

            IList<Dominio.ObjetosDeValor.Embarcador.Ocorrencia.ContratoVeiculo> listaGrid = new List<Dominio.ObjetosDeValor.Embarcador.Ocorrencia.ContratoVeiculo>();
            if (ocorrencia > 0)
            {
                listaGrid = repOcorrenciaContratoVeiculo.ConsultarDeOcorrencia(ocorrencia, propOrdenar, dirOrdena, inicio, limite);
                totalRegistros = repOcorrenciaContratoVeiculo.ContarConsultaDeOcorrencia(ocorrencia);
            }
            else
            {
                Dominio.Entidades.Embarcador.Frete.ContratoFreteTransportador contrato = repContratoFreteTransportador.BuscarContratoAtivo(transportador, tipoOcorrencia, DateTime.Now.Date);
                listaGrid = repContratoFreteVeiculo.Consultar(contrato.Codigo, periodoInicial, periodoFinal, propOrdenar, dirOrdena, inicio, limite);
            }

            var lista = from obj in listaGrid
                        select new
                        {
                            DT_Enable = true,
                            Codigo = obj.Codigo,
                            CodigoVeiculo = obj.CodigoVeiculo,
                            QuantidadeDocumentos = obj.QuantidadeDocumentos,
                            ValorDocumentos = obj.ValorDocumentos,
                            Veiculo = obj.Veiculo,
                            ValorDiaria = obj.ValorDiaria.ToString("n2"),
                            ValorQuinzena = obj.ValorQuinzena.ToString("n2"),
                            QuantidadeDias = obj.QuantidadeDias,
                            Total = obj.Total.ToString("n2"),
                        };

            return lista.ToList();
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
