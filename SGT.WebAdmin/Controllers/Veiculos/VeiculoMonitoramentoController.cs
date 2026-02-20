using Dominio.ObjetosDeValor.Embarcador.Enumeradores;
using SGTAdmin.Controllers;
using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Mvc;

namespace SGT.WebAdmin.Controllers.Veiculos
{
    [CustomAuthorize("Veiculos/VeiculoMonitoramento")]
    public class VeiculoMonitoramentoController : BaseController
    {
		#region Construtores

		public VeiculoMonitoramentoController(Conexao conexao) : base(conexao) { }

		#endregion

        #region Métodos Globais

        [AllowAuthenticate]
        public async Task<IActionResult> Pesquisa()
        {
            try
            {
                return new JsonpResult(ObterGridPesquisa());
            }
            catch (Exception excecao)
            {
                Servicos.Log.TratarErro(excecao);
                return new JsonpResult(false, Localization.Resources.Gerais.Geral.OcorreuFalhaAoConsultar);
            }
        }

        [AllowAuthenticate]
        public async Task<IActionResult> ExportarPesquisa()
        {
            try
            {
                Models.Grid.Grid grid = ObterGridPesquisa();
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

        #region Métodos Privados

        private Models.Grid.Grid ObterGridPesquisa()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                Models.Grid.Grid grid = new Models.Grid.Grid(Request)
                {
                    header = new List<Models.Grid.Head>()
                };

                grid.AdicionarCabecalho(Localization.Resources.Veiculos.VeiculoMonitoramento.Placa, "Placa", 8, Models.Grid.Align.left, true);
                grid.AdicionarCabecalho(Localization.Resources.Veiculos.VeiculoMonitoramento.TipoVeiculo, "TipoVeiculo", 10, Models.Grid.Align.left, true);
                grid.AdicionarCabecalho(Localization.Resources.Veiculos.VeiculoMonitoramento.TipoRodado, "TipoRodado", 10, Models.Grid.Align.left, true);
                grid.AdicionarCabecalho(Localization.Resources.Veiculos.VeiculoMonitoramento.TipoCarroceria, "TipoCarroceria", 10, Models.Grid.Align.left, true);
                grid.AdicionarCabecalho(Localization.Resources.Veiculos.VeiculoMonitoramento.ModeloVeicular, "ModeloVeiculoDescricao", 10, Models.Grid.Align.left, true);
                grid.AdicionarCabecalho(Localization.Resources.Veiculos.VeiculoMonitoramento.Propriedade, "Propriedade", 10, Models.Grid.Align.left, true);
                grid.AdicionarCabecalho(Localization.Resources.Veiculos.VeiculoMonitoramento.Ano, "Ano", 4, Models.Grid.Align.left, true);
                if (TipoServicoMultisoftware == AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiEmbarcador)
                {
                    grid.AdicionarCabecalho(Localization.Resources.Veiculos.VeiculoMonitoramento.Transportador, "Transportador", 10, Models.Grid.Align.left, true);
                    grid.AdicionarCabecalho(Localization.Resources.Veiculos.VeiculoMonitoramento.CNPJ, "CNPJTransportador", 5, Models.Grid.Align.left, true);
                }
                else
                {
                    grid.AdicionarCabecalho("Transportador", false);
                    grid.AdicionarCabecalho("CNPJTransportador", false);
                }
                grid.AdicionarCabecalho(Localization.Resources.Veiculos.VeiculoMonitoramento.Tecnologia, "Tecnologia", 6, Models.Grid.Align.left, true);
                grid.AdicionarCabecalho(Localization.Resources.Veiculos.VeiculoMonitoramento.Terminal, "Terminal", 7, Models.Grid.Align.left, true);
                grid.AdicionarCabecalho(Localization.Resources.Veiculos.VeiculoMonitoramento.Status, "Status", 6, Models.Grid.Align.left, true);
                grid.AdicionarCabecalho(Localization.Resources.Veiculos.VeiculoMonitoramento.DataPosicao, "DataPosicao", 7, Models.Grid.Align.center, true);
                grid.AdicionarCabecalho(Localization.Resources.Veiculos.VeiculoMonitoramento.Rastreador, "Rastreador", 7, Models.Grid.Align.center, false);
                grid.AdicionarCabecalho(Localization.Resources.Veiculos.VeiculoMonitoramento.MonitoramentosFinalizados, "MonitoramentosFinalizados", 4, Models.Grid.Align.center, true);

                Models.Grid.GridPreferencias gridPreferencias = new Models.Grid.GridPreferencias(unitOfWork, "VeiculoMonitoramento/Pesquisa", "grid-veiculo-monitoramento");
                grid.AplicarPreferenciasGrid(gridPreferencias.ObterPreferenciaGrid(this.Usuario.Codigo, grid.modelo));

                Dominio.ObjetosDeValor.Embarcador.Veiculos.FiltroPesquisaVeiculo filtrosPesquisa = ObterFiltrosPesquisa();
                Dominio.ObjetosDeValor.Embarcador.Consulta.ParametroConsulta parametrosConsulta = grid.ObterParametrosConsulta(ObterPropriedadeOrdenar);
                Repositorio.Veiculo repositorioVeiculo = new Repositorio.Veiculo(unitOfWork);
                IList<Dominio.ObjetosDeValor.Embarcador.Veiculos.VeiculoMonitoramentoTecnologia> listaVeiculo = new List<Dominio.ObjetosDeValor.Embarcador.Veiculos.VeiculoMonitoramentoTecnologia>();
                int totalRegistros = repositorioVeiculo.ContarVeiculoMonitoramento(filtrosPesquisa);

                if (totalRegistros > 0)
                    listaVeiculo = repositorioVeiculo.ConsultarVeiculoMonitoramento(filtrosPesquisa, parametrosConsulta);

                var retorno = (
                    from obj in listaVeiculo
                    select new
                    {
                        DT_RowId = obj.Placa,
                        obj.Placa,
                        TipoVeiculo = obj.DescricaoTipoVeiculo,
                        TipoRodado = obj.DescricaoTipoRodado,
                        TipoCarroceria = obj.DescricaoTipoCarroceria,
                        obj.ModeloVeiculoDescricao,
                        Propriedade = obj.DescricaoTipo,
                        obj.Ano,
                        obj.Transportador,
                        CNPJTransportador = !string.IsNullOrWhiteSpace(obj.CNPJTransportador) ? String.Format(@"{0:00\.000\.000\/0000\-00}", long.Parse(obj.CNPJTransportador)) : string.Empty,
                        obj.Tecnologia,
                        obj.Terminal,
                        Status = obj.Ativo ? "Ativo" : "Inativo",
                        Rastreador = obj.DataPosicaoAtual != null && (DateTime.Now - obj.DataPosicaoAtual).Value.TotalMinutes <= ConfiguracaoEmbarcador.TempoSemPosicaoParaVeiculoPerderSinal,
                        DataPosicao = obj.DataPosicaoAtualFormatada,
                        obj.MonitoramentosFinalizados
                    }
                ).ToList();

                grid.AdicionaRows(retorno);
                grid.setarQuantidadeTotal(totalRegistros);

                return grid;
            }
            catch (Exception excecao)
            {
                Servicos.Log.TratarErro(excecao);
                throw;
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        private string ObterPropriedadeOrdenar(string propriedadeOrdenar)
        {
            if (propriedadeOrdenar == "DataPosicao")
                return "DataPosicaoAtual";

            return propriedadeOrdenar;
        }

        [AllowAuthenticate]
        public async Task<IActionResult> ObterEstatisticasVeiculos()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);
            try
            {
                Dominio.ObjetosDeValor.Embarcador.Veiculos.FiltroPesquisaVeiculo filtrosPesquisa = ObterFiltrosPesquisa();
                Repositorio.Veiculo repositorioVeiculo = new Repositorio.Veiculo(unitOfWork);

                filtrosPesquisa.VeiculoOnlineOffline = null;
                int totalVeiculos = repositorioVeiculo.ContarVeiculoMonitoramento(filtrosPesquisa);

                filtrosPesquisa.VeiculoOnlineOffline = true;
                int veiculosOnline = repositorioVeiculo.ContarVeiculoMonitoramento(filtrosPesquisa);
                int veiculosOffline = totalVeiculos - veiculosOnline;

                var resultado = new
                {
                    VeiculosTotal = totalVeiculos,
                    VeiculosOnline = veiculosOnline,
                    VeiculosOffline = veiculosOffline
                };

                return new JsonpResult(resultado);
            }
            catch (Exception excecao)
            {
                Servicos.Log.TratarErro(excecao);
                return new JsonpResult(false, Localization.Resources.Gerais.Geral.OcorreuFalhaAoConsultar);
            }
        }

        private Dominio.ObjetosDeValor.Embarcador.Veiculos.FiltroPesquisaVeiculo ObterFiltrosPesquisa()
        {
            Dominio.ObjetosDeValor.Embarcador.Veiculos.FiltroPesquisaVeiculo filtrosPesquisa = new Dominio.ObjetosDeValor.Embarcador.Veiculos.FiltroPesquisaVeiculo()
            {
                Placa = Request.GetStringParam("PlacaVeiculo"),
                CodigosEmpresa = Request.GetListParam<int>("Transportador"),
                CodigosTecnologiaRastreador = Request.GetListParam<int>("TecnologiaRastreador"),
                TipoVeiculo = Request.GetStringParam("TipoVeiculo"),
                SituacaoAtivo = Request.GetBoolParam("SomenteVeiculosAtivos") ? SituacaoAtivoPesquisa.Ativo : SituacaoAtivoPesquisa.Todos,
                TipoPropriedade = Request.GetStringParam("TipoPropriedade"),
                Terminal = Request.GetStringParam("Terminal"),
                DataPosicao = Request.GetNullableDateTimeParam("DataPosicao"),
                RastreadorPosicionado = Request.GetBoolParam("RastreadorPosicionado"),
                VeiculoOnlineOffline = Request.GetNullableBoolParam("VeiculoOnlineOffline"),
                TempoSemPosicaoParaVeiculoPerderSinal = ConfiguracaoEmbarcador.TempoSemPosicaoParaVeiculoPerderSinal,
            };

            return filtrosPesquisa;
        }

        #endregion
    }
}
