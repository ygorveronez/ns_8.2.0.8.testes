using Dominio.ObjetosDeValor.Embarcador.Logistica;
using SGTAdmin.Controllers;
using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Mvc;

using Dominio.ObjetosDeValor.Embarcador.Enumeradores;

namespace SGT.WebAdmin.Controllers.Avarias
{
    [CustomAuthorize("Logistica/DiariaAutomatica")]
    public class DiariaAutomaticaController : BaseController
    {
		#region Construtores

		public DiariaAutomaticaController(Conexao conexao) : base(conexao) { }

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
                string propOrdena = grid.header[grid.indiceColunaOrdena].data;
                PropOrdena(ref propOrdena);

                List<Dominio.Entidades.Embarcador.Logistica.DiariaAutomatica> listaDiariasAutomaticas = new List<Dominio.Entidades.Embarcador.Logistica.DiariaAutomatica>();

                int totalRegistro = 0;

                ExecutaPesquisa(ref listaDiariasAutomaticas, ref totalRegistro, propOrdena, grid.dirOrdena, grid.inicio, grid.limite, unitOfWork);

                // Converte os dados recebidos
                var lista = RetornaDyn(listaDiariasAutomaticas, unitOfWork);

                // Retorna Grid
                grid.AdicionaRows(lista);
                grid.setarQuantidadeTotal(totalRegistro);

                // Retorna Dados
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

                // Lista de ocorrencias que vai receber consulta
                List<Dominio.Entidades.Embarcador.Logistica.DiariaAutomatica> listaDiariaAutomatica = new List<Dominio.Entidades.Embarcador.Logistica.DiariaAutomatica>();

                // Variavel com o numero total de resultados
                int totalRegistro = 0;

                // Executa metodo de consutla
                ExecutaPesquisa(ref listaDiariaAutomatica, ref totalRegistro, propOrdenar, grid.dirOrdena, grid.inicio, grid.limite, unitOfWork);

                // Converte os dados recebidos
                var lista = RetornaDyn(listaDiariaAutomatica, unitOfWork);

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
            // Busca a ocorrencia
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);
            try
            {
                // Repositorios
                Repositorio.Embarcador.Logistica.DiariaAutomatica repDiariaAutomatica = new Repositorio.Embarcador.Logistica.DiariaAutomatica(unitOfWork);
                Repositorio.Embarcador.Logistica.DiariaAutomaticaComposicaoFrete repDiariaAutomaticaComposicaoFrete = new Repositorio.Embarcador.Logistica.DiariaAutomaticaComposicaoFrete(unitOfWork);

                // Codigo requisicao
                int codigo = 0;
                int.TryParse(Request.Params("Codigo"), out codigo);

                // Entidades
                Dominio.Entidades.Embarcador.Logistica.DiariaAutomatica diariaAutomatica = repDiariaAutomatica.BuscarPorCodigo(codigo, false);
                var composicaoFrete = repDiariaAutomaticaComposicaoFrete.BuscarPorDiariaAutomatica(diariaAutomatica.Codigo);

                if (diariaAutomatica == null)
                    return new JsonpResult(false, "Ocorreu uma falha ao buscar por código.");

                var dynOcorrencia = new
                {
                    diariaAutomatica.Codigo,
                    LocalFreeTime = diariaAutomatica.LocalFreeTime.ObterDescricao(),
                    ValorDiaria = diariaAutomatica.ValorDiaria,
                    Status = diariaAutomatica.Status.ObterDescricao(),
                    Chamado = diariaAutomatica.Chamado?.Codigo,
                    DataInicioCobranca = diariaAutomatica.DataInicioCobranca.ToString("dd/MM/yyyy HH:mm"),
                    DataUltimaAtualizacao = diariaAutomatica.DataUltimaAtualizacao?.ToString("dd/MM/yyyy HH:mm"),
                    ComposicaoFrete = (from o in composicaoFrete select new
                    {
                        TipoParametro = o.TipoParametro.ObterDescricao(),
                        TipoValor = o.TipoValor.ObterDescricao(),
                        Valor = o.Valor,
                        Formula = o.Formula,
                        DescricaoComponente = o.DescricaoComponente,
                        CodigoComponente = o.CodigoComponente,
                        ValoresFormula = o.ValoresFormula,
                        ValorCalculado = o.ValorCalculado,
                    })

                };

                return new JsonpResult(dynOcorrencia);
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);
                return new JsonpResult(false, "Ocorreu uma falha ao buscar por código.");
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        #endregion

        #region Métodos Privados

        private Models.Grid.Grid GridPesquisa(Repositorio.UnitOfWork unitOfWork)
        {
            Models.Grid.Grid grid = new Models.Grid.Grid(Request);
            grid.header = new List<Models.Grid.Head>();

            grid.AdicionarCabecalho("Codigo", false);
            grid.AdicionarCabecalho("CodigoCarga", false);
            grid.AdicionarCabecalho("Carga", "Carga", 7, Models.Grid.Align.center, true);
            grid.AdicionarCabecalho("Veículo", "Veiculo", 7, Models.Grid.Align.center, false);
            grid.AdicionarCabecalho("Transportador", "Transportador", 7, Models.Grid.Align.center, true);
            grid.AdicionarCabecalho("Tipo", "LocalFreeTime", 10, Models.Grid.Align.left, true);
            grid.AdicionarCabecalho("Situação", "Situacao", 10, Models.Grid.Align.left, true);
            grid.AdicionarCabecalho("Data início", "DataInicio", 10, Models.Grid.Align.center, true);
            grid.AdicionarCabecalho("Última atualização", "DataUltimaAtualizacao", 10, Models.Grid.Align.center, true);
            grid.AdicionarCabecalho("Tempo (minutos)", "Tempo", 8, Models.Grid.Align.left, true);
            grid.AdicionarCabecalho("Valor atual", "ValorDiaria", 10, Models.Grid.Align.left, true);
            grid.AdicionarCabecalho("Atendimento", "Atendimento", 10, Models.Grid.Align.left, false);

            return grid;
        }

        private void PropOrdena(ref string propOrdena)
        {
            if (propOrdena == "Carga")
                propOrdena = "Carga.CodigoCargaEmbarcador";
            else if (propOrdena == "Avaria")
                propOrdena = "NumeroAvaria";
            else if (propOrdena == "Transportador")
                propOrdena = "Carga.Empresa.RazaoSocial";
            else if (propOrdena == "Situacao")
                propOrdena = "Status";
            else if(propOrdena == "Tempo")
                propOrdena = "TempoTotal";
            else if (propOrdena == "DataInicio")
                propOrdena = "DataInicioCobranca";
        }
        /* ExecutaPesquisa
         * Converte os valores vindo por POST 
         * E faz consulta de ocorrencias pendentes de aprovações
         */
        private void ExecutaPesquisa(ref List<Dominio.Entidades.Embarcador.Logistica.DiariaAutomatica> lista, ref int totalRegistros, string propOrdenacao, string dirOrdenacao, int inicioRegistros, int maximoRegistros, Repositorio.UnitOfWork unitOfWork)
        {
            Repositorio.Embarcador.Logistica.DiariaAutomatica repDiariaAutomatica = new Repositorio.Embarcador.Logistica.DiariaAutomatica(unitOfWork);

            FiltroPesquisaDiariaAutomatica filtroPesquisaDiariaAutomatica = ObterFiltro();

            lista = repDiariaAutomatica.Consultar(filtroPesquisaDiariaAutomatica, propOrdenacao, dirOrdenacao, inicioRegistros, maximoRegistros);
            totalRegistros = repDiariaAutomatica.ContarConsulta(filtroPesquisaDiariaAutomatica);
        }

        private FiltroPesquisaDiariaAutomatica ObterFiltro()
        {
            DateTime.TryParseExact(Request.Params("DataInicial"), "dd/MM/yyyy", null, System.Globalization.DateTimeStyles.None, out DateTime dataInicial);
            DateTime.TryParseExact(Request.Params("DataFinal"), "dd/MM/yyyy", null, System.Globalization.DateTimeStyles.None, out DateTime dataFinal);

            return new FiltroPesquisaDiariaAutomatica

            {
                DataInicial = dataInicial,
                DataFinal = dataFinal,
                CodigoTransportador = Request.GetIntParam("Transportador"),
                CodigoFilial = Request.GetIntParam("Filial"),
                CodigoCarga = Request.GetIntParam("Carga"),
                LocalFreeTime = Request.GetEnumParam<LocalFreeTime>("LocalFreeTime"),
                Status = Request.GetEnumParam<StatusDiariaAutomatica>("Status"),
            };
        }

        private dynamic RetornaDyn(List<Dominio.Entidades.Embarcador.Logistica.DiariaAutomatica> listaDiariaAutomatica, Repositorio.UnitOfWork unitOfWork)
        {

            var lista = from diariaAutomatica in listaDiariaAutomatica
                        select new
                        {
                            Codigo = diariaAutomatica.Codigo,
                            CodigoCarga = diariaAutomatica.Carga.Codigo,
                            Carga = diariaAutomatica.Carga.CodigoCargaEmbarcador,
                            Veiculo = diariaAutomatica.Carga.Veiculo?.Placa ?? "???",
                            Transportador = diariaAutomatica.Carga.Empresa?.NomeFantasia ?? "",
                            LocalFreeTime = diariaAutomatica.LocalFreeTime.ObterDescricao(),
                            Situacao = diariaAutomatica.Status.ObterDescricao(),
                            DataInicio = diariaAutomatica.DataInicioCobranca.ToString("dd/MM/yyyy HH:mm"),
                            DataUltimaAtualizacao = diariaAutomatica.DataUltimaAtualizacao?.ToString("dd/MM/yyyy HH:mm") ?? "",
                            Tempo = diariaAutomatica.TempoTotal,
                            ValorDiaria = diariaAutomatica.ValorDiaria,
                            Atendimento = diariaAutomatica.Chamado?.Codigo ?? 0
                        };

            return lista.ToList();
        }

        #endregion
    }
}
