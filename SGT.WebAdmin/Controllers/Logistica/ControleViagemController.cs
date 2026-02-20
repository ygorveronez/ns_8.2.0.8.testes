using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Mvc;
using SGTAdmin.Controllers;

namespace SGT.WebAdmin.Controllers.Logistica
{
    [CustomAuthorize("Logistica/ControleViagem")]
    public class ControleViagemController : MonitoramentoControllerBase
    {
		#region Construtores

		public ControleViagemController(Conexao conexao) : base(conexao) { }

		#endregion

		#region Métodos Globais

		[AllowAuthenticate]
        public async Task<IActionResult> Pesquisa()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);
            try
            {
                Models.Grid.Grid grid = CarregarGrid(unitOfWork);

                Repositorio.Embarcador.Configuracao.ConfiguracaoTMS repConfiguracaoTMS = new Repositorio.Embarcador.Configuracao.ConfiguracaoTMS(unitOfWork);
                Dominio.Entidades.Embarcador.Configuracao.ConfiguracaoTMS configuracao = repConfiguracaoTMS.BuscarConfiguracaoPadrao();

                Dominio.ObjetosDeValor.Embarcador.Logistica.FiltroPesquisaControleViagem filtroPesquisa = ObterFiltrosPesquisa(unitOfWork);
                Dominio.ObjetosDeValor.Embarcador.Consulta.ParametroConsulta parametrosConsulta = grid.ObterParametrosConsulta();
                Repositorio.Embarcador.Logistica.ControleViagem repControleViagem = new Repositorio.Embarcador.Logistica.ControleViagem(unitOfWork);
                IList<Dominio.ObjetosDeValor.Embarcador.Logistica.ControleViagem> listaConsulta = new List<Dominio.ObjetosDeValor.Embarcador.Logistica.ControleViagem>();

                string colunaOrdenacaoManual = String.Empty;
                string direcaoOrdenar = parametrosConsulta.DirecaoOrdenar.ToLower();
                int totalRegistros = repControleViagem.ContarConsulta(filtroPesquisa);
                if (totalRegistros > 0)
                {
                    if (parametrosConsulta.PropriedadeOrdenar.Equals("Status") || parametrosConsulta.PropriedadeOrdenar.Equals("Rastreador"))
                    {
                        colunaOrdenacaoManual = parametrosConsulta.PropriedadeOrdenar;
                        parametrosConsulta.PropriedadeOrdenar = "CargaCodigo";
                    }
                    listaConsulta = repControleViagem.Consultar(filtroPesquisa, parametrosConsulta);
                }

                var listaRetornar = (
                    from reg in listaConsulta
                    select new
                    {
                        reg.CargaCodigo,
                        reg.MonitoramentoCodigo,
                        reg.CargaCodigoEmbarcador,
                        DataCriacaoCarga = reg.DataCriacaoCarga.ToString(MASCARA_DATA_HM),
                        Pedido = reg.Pedido != null ? reg.Pedido.Contains('_') ? reg.Pedido.Split('_')[1].ToString() : reg.Pedido : "",
                        reg.FilialCodigo,
                        reg.FilialDescricao,
                        reg.TransportadorCodigo,
                        reg.TransportadorNomeFantasia,
                        reg.VeiculoTracaoPlaca,
                        reg.VeiculoReboquePlaca,
                        reg.TecnologiaRastreadorDescricao,
                        reg.Processo,
                        reg.Operacao,
                        DataCarregamentoPedido = (reg.DataCarregamentoPedido != null) ? reg.DataCarregamentoPedido.Value.ToString(MASCARA_DATA_HM) : string.Empty,
                        DataPrevisaoInicioViagem = (reg.DataPrevisaoInicioViagem != null) ? reg.DataPrevisaoInicioViagem.Value.ToString(MASCARA_DATA_HM) : string.Empty,
                        PrevisaoChegada = (reg.DataPrevisaoChegadaPlanta != null) ? reg.DataPrevisaoChegadaPlanta.Value.ToString(MASCARA_DATA_HM) : string.Empty,
                        reg.DataPrevista,
                        reg.ClienteDestinoCodigo,
                        reg.ClienteDestinoNome,
                        reg.ClienteDestinoLocalidade,
                        reg.StatusViagem,
                        Status = IdentificarStatus(reg),
                        reg.TempoTransito,
                        NaUnidade = "?",
                        PosicaoDataVeiculo = reg.PosicaoDataVeiculo,
                        Rastreador = (reg.PosicaoDataVeiculo != null && (DateTime.Now - reg.PosicaoDataVeiculo.Value).TotalMinutes <= configuracao.TempoSemPosicaoParaVeiculoPerderSinal).ToString(),
                        reg.PosicaoDescricao,
                        DistanciaPrevista = (reg.DistanciaPrevista != null) ? String.Format("{0:n1} Km", reg.DistanciaPrevista) : string.Empty,
                        DistanciaRealizada = (reg.DistanciaRealizada != null) ? String.Format("{0:n1} Km", reg.DistanciaRealizada) : string.Empty,
                        DistanciaAteOrigem = (reg.DistanciaAteOrigem != null) ? String.Format("{0:n1} Km", reg.DistanciaAteOrigem) : string.Empty,
                        DistanciaAteDestino = (reg.DistanciaAteDestino != null) ? String.Format("{0:n1} Km", reg.DistanciaAteDestino) : string.Empty
                    }).ToList();

                // Ordenação de colunas que não são do banco de dados
                switch (colunaOrdenacaoManual)
                {
                    case "Status":
                        if (direcaoOrdenar == "asc") listaRetornar.Sort((x, y) => String.Compare(x.Status, y.Status)); else listaRetornar.Sort((x, y) => String.Compare(y.Status, x.Status));
                        break;
                    case "Rastreador":
                        if (direcaoOrdenar == "asc") listaRetornar.Sort((x, y) => String.Compare(x.Rastreador, y.Rastreador)); else listaRetornar.Sort((x, y) => String.Compare(y.Rastreador, x.Rastreador));
                        break;
                }

                grid.AdicionaRows(listaRetornar);
                grid.setarQuantidadeTotal(totalRegistros);
                return new JsonpResult(grid);
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
        public async Task<IActionResult> ObterDadosMapa()
        {
            var unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);
            try
            {
                int codigoMonitoramento = Request.GetIntParam("codigo");
                int codigoCarga = Request.GetIntParam("Carga");
                int codigoVeiculo = Request.GetIntParam("Veiculo");
                return base.DadosMapa(unitOfWork, codigoMonitoramento, codigoCarga, codigoVeiculo);
            }
            catch (Exception excecao)
            {
                Servicos.Log.TratarErro(excecao);
                return new JsonpResult(false, "Ocorreu uma falha ao obter dados do mapa.");
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        #endregion

        #region Métodos Privados

        private Dominio.ObjetosDeValor.Embarcador.Logistica.FiltroPesquisaControleViagem ObterFiltrosPesquisa(Repositorio.UnitOfWork unitOfWork)
        {
            Dominio.ObjetosDeValor.Embarcador.Logistica.FiltroPesquisaControleViagem filtro = new Dominio.ObjetosDeValor.Embarcador.Logistica.FiltroPesquisaControleViagem()
            {
                DataInicialCarga = Request.GetNullableDateTimeParam("DataInicialCarga"),
                DataFinalCarga = Request.GetNullableDateTimeParam("DataFinalCarga"),
                CodigosTransportador = Request.GetListParam<int>("Transportador"),
                CodigosVeiculos = Request.GetListParam<int>("Veiculo"),
                CodigosFilial = Request.GetListParam<int>("Filial"),
                CodigoClienteDestino = Request.GetDoubleParam("Cliente"),
                CodigosStatusViagem = Request.GetListParam<int>("StatusViagem")
            };
            return filtro;
        }

        private Models.Grid.Grid CarregarGrid(Repositorio.UnitOfWork unitOfWork)
        {
            Models.Grid.Grid grid = new Models.Grid.Grid(Request)
            {
                header = new List<Models.Grid.Head>()
            };

            grid.AdicionarCabecalho("CargaCodigo", false);
            grid.AdicionarCabecalho("SM", "MonitoramentoCodigo", 5, Models.Grid.Align.center, false, false);
            grid.AdicionarCabecalho("Carga", "CargaCodigoEmbarcador", 8, Models.Grid.Align.center, true, false);
            grid.AdicionarCabecalho("Data da carga", "DataCriacaoCarga", 8, Models.Grid.Align.center, true, false);
            grid.AdicionarCabecalho("Pedido", "Pedido", 10, Models.Grid.Align.center, true);
            grid.AdicionarCabecalho("Código", "FilialCodigo", 5, Models.Grid.Align.center, true, false);
            grid.AdicionarCabecalho("Origem", "FilialDescricao", 5, Models.Grid.Align.left, true, false);
            grid.AdicionarCabecalho("TransportadorCodigo", false);
            grid.AdicionarCabecalho("Transportador", "TransportadorNomeFantasia", 10, Models.Grid.Align.left, true);
            grid.AdicionarCabecalho("Cavalo", "VeiculoTracaoPlaca", 4, Models.Grid.Align.left, true);
            grid.AdicionarCabecalho("Carreta", "VeiculoReboquePlaca", 4, Models.Grid.Align.left, true, false);
            grid.AdicionarCabecalho("Tecnologia", "TecnologiaRastreadorDescricao", 5, Models.Grid.Align.left, true, false);
            grid.AdicionarCabecalho("Processo", "Processo", 5, Models.Grid.Align.left, true, false);
            grid.AdicionarCabecalho("Operação", "Operacao", 10, Models.Grid.Align.left, true, false);
            grid.AdicionarCabecalho("Janela chegada", "DataCarregamentoPedido", 8, Models.Grid.Align.center, true);
            grid.AdicionarCabecalho("Previsão Chegada", "PrevisaoChegada", 8, Models.Grid.Align.right, true);
            grid.AdicionarCabecalho("Janela carregamento", "DataPrevisaoInicioViagem", 8, Models.Grid.Align.center, true, false);
            grid.AdicionarCabecalho("Previsto", "DataPrevista", 5, Models.Grid.Align.left, true, false);
            grid.AdicionarCabecalho("ClienteDestinoCodigo", false);
            grid.AdicionarCabecalho("Destino", "ClienteDestinoNome", 18, Models.Grid.Align.left, true);
            grid.AdicionarCabecalho("Cidade destino", "ClienteDestinoLocalidade", 15, Models.Grid.Align.left, true, false);
            grid.AdicionarCabecalho("Status Viagem", "StatusViagem", 5, Models.Grid.Align.left, true, false);
            grid.AdicionarCabecalho("Status", "Status", 8, Models.Grid.Align.left, true);
            grid.AdicionarCabecalho("Em trânsito", "TempoTransito", 5, Models.Grid.Align.left, true, false);
            grid.AdicionarCabecalho("Na unidade", "NaUnidade", 5, Models.Grid.Align.center, true, false);
            grid.AdicionarCabecalho("Rastreador", "Rastreador", 4, Models.Grid.Align.left, true);
            grid.AdicionarCabecalho("Data da posição", "PosicaoDataVeiculo", 5, Models.Grid.Align.left, true, false);
            grid.AdicionarCabecalho("Posição", "PosicaoDescricao", 30, Models.Grid.Align.center, true, false);
            grid.AdicionarCabecalho("Distância prevista", "DistanciaPrevista", 5, Models.Grid.Align.right, true, false);
            grid.AdicionarCabecalho("Distância realizada", "DistanciaRealizada", 5, Models.Grid.Align.right, true, false);
            grid.AdicionarCabecalho("Distância até origem", "DistanciaAteOrigem", 5, Models.Grid.Align.right, true);
            grid.AdicionarCabecalho("Distância até destino", "DistanciaAteDestino", 5, Models.Grid.Align.right, true, false);

            Models.Grid.GridPreferencias gridPreferencias = new Models.Grid.GridPreferencias(unitOfWork, "ControleViagem/Pesquisa", "grid-controle-viagem");
            grid.AplicarPreferenciasGrid(gridPreferencias.ObterPreferenciaGrid(this.Usuario.Codigo, grid.modelo));

            return grid;
        }

        private string IdentificarStatus(Dominio.ObjetosDeValor.Embarcador.Logistica.ControleViagem controleViagem)
        {
            if (controleViagem.EmAlvo)
            {
                double cnpjFilial = double.Parse(controleViagem.FilialCNPJ);
                string[] codigosClientes = controleViagem.CodigosClientesAlvos.Split(',');
                int total = codigosClientes.Length;
                for (int i = 0; i < total; i++)
                {
                    double codigo = double.Parse(codigosClientes[i]);
                    if (cnpjFilial == codigo)
                    {
                        if (controleViagem.CargaSituacao == Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoCarga.EmTransporte)
                        {
                            return "Carregado";
                        }
                        else
                        {
                            return "No pátio";
                        }
                    }
                }
            }
            else if (controleViagem.CargaSituacao == Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoCarga.EmTransporte)
            {
                return "Em transporte";
            }
            return "Em deslocamento";
        }

        #endregion
    }
}
