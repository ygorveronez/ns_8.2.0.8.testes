using SGTAdmin.Controllers;
using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Mvc;

namespace SGT.WebAdmin.Controllers.Cargas.AlertasTransportador
{
    [CustomAuthorize("Cargas/AlertasTransportador")]
    public class AlertasTransportadorController : BaseController
    {
		#region Construtores

		public AlertasTransportadorController(Conexao conexao) : base(conexao) { }

		#endregion

        #region Métodos Globais

        [AllowAuthenticate]
        public async Task<IActionResult> Pesquisa()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);
            try
            {
                return new JsonpResult(ObterGridPesquisa());
            }
            catch (Exception excecao)
            {
                Servicos.Log.TratarErro(excecao);
                return new JsonpResult(false, "Ocorreu uma falha ao consultar.");
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }
        
        [AllowAuthenticate]
        public async Task<IActionResult> BuscarPorCodigo()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                int codigo = Request.GetIntParam("Codigo");
                Repositorio.Embarcador.Logistica.AlertaMonitor repositorio = new Repositorio.Embarcador.Logistica.AlertaMonitor(unitOfWork);
                Dominio.Entidades.Embarcador.Logistica.AlertaMonitor reg = repositorio.BuscarPorCodigo(codigo);

                if (reg == null)
                    return new JsonpResult(false, true, "Não foi possível encontrar o registro.");

                return new JsonpResult(new
                {
                    reg.Codigo,
                    reg.Descricao,
                    reg.TipoAlerta,
                });
            }
            catch (Exception excecao)
            {
                Servicos.Log.TratarErro(excecao);

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
            try
            {
                var grid = ObterGridPesquisa();
                byte[] arquivoBinario = grid.GerarExcel();
                if (arquivoBinario != null)
                    return Arquivo(arquivoBinario, "application/octet-stream", $"{grid.tituloExportacao}.{grid.extensaoCSV}");
                return new JsonpResult(false, Localization.Resources.Gerais.Geral.OcorreuUmaFalhaAoGerarArquivo);
            }
            catch (Exception excecao)
            {
                Servicos.Log.TratarErro(excecao);
                return new JsonpResult(false, Localization.Resources.Gerais.Geral.OcorreuFalhaAoExportar);
            }
        }

        #endregion

        #region Métodos privados
        private Models.Grid.Grid ObterGridPesquisa()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                Models.Grid.Grid grid = new Models.Grid.Grid(Request)
                {
                    header = new List<Models.Grid.Head>()
                };

                grid.AdicionarCabecalho("Codigo", false, true);
                grid.AdicionarCabecalho("LatitudeFormatada", false, true);
                grid.AdicionarCabecalho("LongitudeFormatada", false, true);
                grid.AdicionarCabecalho("DataFimTratativaFormatada", false, true);
                grid.AdicionarCabecalho("Observacao", false, true);
                grid.AdicionarCabecalho("Alerta", "NomeAlerta", 10, Models.Grid.Align.left, true, true);
                grid.AdicionarCabecalho("Valor", "Descricao", 10, Models.Grid.Align.left, true, true);
                grid.AdicionarCabecalho("Status", "StatusDescricao", 5, Models.Grid.Align.center, true, true);
                grid.AdicionarCabecalho("Data do alerta", "Data", 10, Models.Grid.Align.center, true, true);
                grid.AdicionarCabecalho("Carga", "CodigoCargaEmbarcador", 5, Models.Grid.Align.center, true, true);
                grid.AdicionarCabecalho("Tipo de operação", "TipoOperacao", 5, Models.Grid.Align.left, true, true);
                grid.AdicionarCabecalho("Placa", "Placa", 5, Models.Grid.Align.center, true, true);
                grid.AdicionarCabecalho("Motorista", "Motorista", 5, Models.Grid.Align.left, true, true);
                grid.AdicionarCabecalho("Transportador", "Transportador", 5, Models.Grid.Align.left, true, true);
                grid.AdicionarCabecalho("Responsável", "Responsavel", 10, Models.Grid.Align.center, true, true);
                grid.AdicionarCabecalho("Data da tratativa", "DataTratativa", 10, Models.Grid.Align.center, true, true);
                grid.AdicionarCabecalho("Tratativa", "Acao", 5, Models.Grid.Align.left, true, true);


                Models.Grid.GridPreferencias preferenciaGrid = new Models.Grid.GridPreferencias(unitOfWork, "AlertasTransportador/Pesquisa", "grid-alertas-transportador");
                grid.AplicarPreferenciasGrid(preferenciaGrid.ObterPreferenciaGrid(this.Usuario.Codigo, grid.modelo));

                Repositorio.Embarcador.Cargas.AlertasTransportador repositorioAlertas = new Repositorio.Embarcador.Cargas.AlertasTransportador(unitOfWork);
                Dominio.ObjetosDeValor.Embarcador.Logistica.FiltroPesquisaMonitoramentoAlerta filtrosPesquisa = ObterFiltrosPesquisa(unitOfWork);

                Dominio.ObjetosDeValor.Embarcador.Consulta.ParametroConsulta parametrosConsulta = grid.ObterParametrosConsulta();

                int totalRegistros = repositorioAlertas.ContarConsulta(filtrosPesquisa, this.Empresa.Codigo);

                IList<Dominio.Relatorios.Embarcador.DataSource.Logistica.MonitoramentoAlerta> listaRegistros = totalRegistros > 0 ? repositorioAlertas.Consultar(filtrosPesquisa, parametrosConsulta, false, this.Empresa.Codigo) : new List<Dominio.Relatorios.Embarcador.DataSource.Logistica.MonitoramentoAlerta>();

                var lista = (from p in listaRegistros
                             select new
                             {
                                 p.Codigo,
                                 p.NomeAlerta,
                                 p.Tipo,
                                 p.Descricao,
                                 p.StatusDescricao,
                                 Data = p.DataFormatada,
                                 p.DataFimTratativaFormatada,
                                 p.LatitudeFormatada,
                                 p.LongitudeFormatada,
                                 p.CodigoCargaEmbarcador,
                                 p.TipoOperacao,
                                 p.Placa,
                                 p.Motorista,
                                 p.Transportador,
                                 DataTratativa = p.DataTratativaFormatada,
                                 p.Responsavel,
                                 p.Acao,
                                 p.Observacao
                             }).ToList();


                grid.AdicionaRows(lista);
                grid.setarQuantidadeTotal(totalRegistros);

                return grid;
            }
            catch (Exception ex)
            {
                throw;
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }
        private Dominio.ObjetosDeValor.Embarcador.Logistica.FiltroPesquisaMonitoramentoAlerta ObterFiltrosPesquisa(Repositorio.UnitOfWork unitOfWork)
        {
            Dominio.ObjetosDeValor.Embarcador.Logistica.FiltroPesquisaMonitoramentoAlerta filtrosPesquisa = new Dominio.ObjetosDeValor.Embarcador.Logistica.FiltroPesquisaMonitoramentoAlerta()
            {
                CodigoCargaEmbarcador = Request.GetStringParam("CodigoCargaEmbarcador"),
                Placa = Request.GetStringParam("Placa"),
                DataInicial = Request.GetNullableDateTimeParam("DataInicial"),
                DataFinal = Request.GetNullableDateTimeParam("DataFinal"),
                Motorista = Request.GetIntParam("Motorista"),
                Transportador = Request.GetIntParam("Transportador"),
                AlertaMonitorStatus = Request.GetEnumParam<Dominio.ObjetosDeValor.Embarcador.Enumeradores.AlertaMonitorStatus>("AlertaMonitorStatus"),
                TipoAlerta = Request.GetEnumParam<Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoAlerta>("TipoAlerta"),
                ApenasComPosicaoTardia = Request.GetBoolParam("ExibirApenasComPosicaoTardia"),
                Filiais = ObterListaCodigoFilialPermitidasOperadorLogistica(unitOfWork),
                Recebedores = ObterListaCnpjCpfRecebedorPermitidosOperadorLogistica(unitOfWork),
                FiltrarCargasPorParteDoNumero = ConfiguracaoEmbarcador?.FiltrarCargasPorParteDoNumero ?? false
            };

            return filtrosPesquisa;
        }

        #endregion
    }
}
