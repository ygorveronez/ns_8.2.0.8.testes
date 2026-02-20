using System;
using System.Collections.Generic;
using System.Linq;

namespace SGT.WebAdmin.Controllers.Logistica
{

    public class GridTorreMonitoramento
    {

        #region Métodos públicos

        public Models.Grid.Grid ObterGrid(HttpRequest request, int codigoUsuario, Dominio.ObjetosDeValor.Embarcador.Logistica.FiltroPesquisaMonitoramento filtrosPesquisa, Dominio.Entidades.Embarcador.Configuracao.ConfiguracaoTMS configuracao, Repositorio.UnitOfWork unitOfWork)
        {
            Models.Grid.Grid grid = new Models.Grid.Grid(request)
            {
                header = new List<Models.Grid.Head>()
            };

            grid.AdicionarCabecalho("Codigo", false);
            grid.AdicionarCabecalho("Carga", false);
            grid.AdicionarCabecalho("DataInicioMonitoramento", false);
            grid.AdicionarCabecalho("DataFimMonitoramento", false);
            grid.AdicionarCabecalho("Veiculo", false);
            grid.AdicionarCabecalho("IDEquipamento", false);
            grid.AdicionarCabecalho("Data da Necessidade", "DataCriacaoCarga", 5, Models.Grid.Align.left, true);
            grid.AdicionarCabecalho("Carga", "CargaEmbarcador", 6, Models.Grid.Align.left, true);
            grid.AdicionarCabecalho("Data Carregamento", "DataCarregamento", 5, Models.Grid.Align.left, true);
            grid.AdicionarCabecalho("Antecedência Grade", "AntecedenciaGrade", 5, Models.Grid.Align.left, true);
            grid.AdicionarCabecalho("Destinos", "Destinos", 10, Models.Grid.Align.left, true);
            grid.AdicionarCabecalho("Cidade", "CidadeDestino", 6, Models.Grid.Align.left, true);
            grid.AdicionarCabecalho("Tipo Carga", "TipoCarga", 6, Models.Grid.Align.left, true);
            grid.AdicionarCabecalho("Transportador", "Transportador", 10, Models.Grid.Align.left, true);
            grid.AdicionarCabecalho("Tração", "Tracao", 5, Models.Grid.Align.left, true);
            grid.AdicionarCabecalho("Reboques", "Reboques", 5, Models.Grid.Align.left, true);
            grid.AdicionarCabecalho("Pedidos", "Pedidos", 5, Models.Grid.Align.left, true);
            grid.AdicionarCabecalho("Ordem Embarque", "Ordens", 5, Models.Grid.Align.left, true);
            grid.AdicionarCabecalho("Filial", "Filial", 8, Models.Grid.Align.left, true);
            grid.AdicionarCabecalho("Insumos", "Insumos", 8, Models.Grid.Align.left, true);
            grid.AdicionarCabecalho("Espelhado", "Rastreador", 4, Models.Grid.Align.left, true);
            grid.AdicionarCabecalho("Posicão", "Posicao", 5, Models.Grid.Align.left, true);
            grid.AdicionarCabecalho("Valor Frete", "ValorFrete", 5, Models.Grid.Align.left, true);
            grid.AdicionarCabecalho("Oc. Deslocamento", "OcorrenciasDeslocamento", 8, Models.Grid.Align.left, true);
            grid.AdicionarCabecalho("Oc. Frete Negociado", "OcorrenciasFreteNegociado", 8, Models.Grid.Align.left, true);
            grid.AdicionarCabecalho("Outras Ocorrências", "OutrasOcorrencias", 8, Models.Grid.Align.left, true);
            grid.AdicionarCabecalho("Data início viagem", "DataInicioViagem", 5, Models.Grid.Align.left, true);
            grid.AdicionarCabecalho("Data Prevista Chegada", "DataPrevisaoChegadaPlanta", 5, Models.Grid.Align.left, true);
            grid.AdicionarCabecalho("KM Rodado Dia", "kmRodadoDia", 5, Models.Grid.Align.left, true);
            grid.AdicionarCabecalho("KM Chegada Unidade", "DistanciaAteOrigem", 5, Models.Grid.Align.left, true);
            grid.AdicionarCabecalho("Tempo Viagem", "TempoViagem", 5, Models.Grid.Align.left, true);
            grid.AdicionarCabecalho("Status Previsto Chegada", "StatusPrevisaoChegada", 5, Models.Grid.Align.left, true);
            grid.AdicionarCabecalho("Data Checkin Descarga", "DataCheckinDescargaFormatada", 5, Models.Grid.Align.left, true);
            grid.AdicionarCabecalho("Data InÍcio Descarga", "DataInicioDescarga", 5, Models.Grid.Align.left, true);
            grid.AdicionarCabecalho("Data Fim Descarga", "DataFimDescarga", 5, Models.Grid.Align.left, true);
            grid.AdicionarCabecalho("Tempo Descarga", "TempoDescarga", 5, Models.Grid.Align.left, true);
            grid.AdicionarCabecalho("Data Real Chegada", "DataChegadaPlanta", 5, Models.Grid.Align.left, true);
            grid.AdicionarCabecalho("Status Real Chegada", "StatusRealChegada", 5, Models.Grid.Align.left, true);
            grid.AdicionarCabecalho("KM Rodado", "KMRodado", 7, Models.Grid.Align.left, true, false);
            grid.AdicionarCabecalho("Calculo Diária", "CalculoDiaria", 7, Models.Grid.Align.left, true, false);
            grid.AdicionarCabecalho("Valor Diária", "ValorDiaria", 7, Models.Grid.Align.left, true, false);
            grid.AdicionarCabecalho("Status", "StatusAtual", 7, Models.Grid.Align.left, true, false);
            grid.AdicionarCabecalho("Situação Monitoramento", "Status", 7, Models.Grid.Align.left, true);

            Models.Grid.GridPreferencias gridPreferencias = new Models.Grid.GridPreferencias(unitOfWork, "Monitoramento/TorreMonitoramentoPesquisa", "grid-torre-monitoramento");
            grid.AplicarPreferenciasGrid(gridPreferencias.ObterPreferenciaGrid(codigoUsuario, grid.modelo));

            return grid;

        }

        public IList<Dominio.ObjetosDeValor.Embarcador.Logistica.TorreMonitoramento> ObterRegistrosPesquisa(HttpRequest request, int codigoUsuario, int totalRegistros, Dominio.ObjetosDeValor.Embarcador.Logistica.FiltroPesquisaMonitoramento filtrosPesquisa, Dominio.Entidades.Embarcador.Configuracao.ConfiguracaoTMS configuracao, Repositorio.UnitOfWork unitOfWork)
        {
            IList<Dominio.ObjetosDeValor.Embarcador.Logistica.TorreMonitoramento> listaConsulta = new List<Dominio.ObjetosDeValor.Embarcador.Logistica.TorreMonitoramento>();

            Models.Grid.Grid grid = ObterGrid(request, codigoUsuario, filtrosPesquisa, configuracao, unitOfWork);
            Dominio.ObjetosDeValor.Embarcador.Consulta.ParametroConsulta parametrosConsulta = grid.ObterParametrosConsulta(null);

            Repositorio.Embarcador.Logistica.Monitoramento repMonitoramento = new Repositorio.Embarcador.Logistica.Monitoramento(unitOfWork);
            listaConsulta = repMonitoramento.ConsultarTorre(filtrosPesquisa, parametrosConsulta, configuracao);

            return listaConsulta;
        }

        public int ObterTotalRegistrosPesquisa(Dominio.ObjetosDeValor.Embarcador.Logistica.FiltroPesquisaMonitoramento filtrosPesquisa, Dominio.Entidades.Embarcador.Configuracao.ConfiguracaoTMS configuracao, Repositorio.UnitOfWork unitOfWork)
        {
            Repositorio.Embarcador.Logistica.Monitoramento repositorio = new Repositorio.Embarcador.Logistica.Monitoramento(unitOfWork);
            int totalRegistros = repositorio.ContarConsultaTorre(filtrosPesquisa, configuracao);
            return totalRegistros;
        }

        public Models.Grid.Grid ObterGridComPesquisa(HttpRequest request, int codigoUsuario, Dominio.ObjetosDeValor.Embarcador.Logistica.FiltroPesquisaMonitoramento filtrosPesquisa, Dominio.Entidades.Embarcador.Configuracao.ConfiguracaoTMS configuracao, Repositorio.UnitOfWork unitOfWork)
        {
            Models.Grid.Grid grid = ObterGrid(request, codigoUsuario, filtrosPesquisa, configuracao, unitOfWork);

            int totalRegistros = ObterTotalRegistrosPesquisa(filtrosPesquisa, configuracao, unitOfWork);
            IList<Dominio.ObjetosDeValor.Embarcador.Logistica.TorreMonitoramento> listaConsulta = (totalRegistros > 0) ? ObterRegistrosPesquisa(request, codigoUsuario, totalRegistros, filtrosPesquisa, configuracao, unitOfWork) : new List<Dominio.ObjetosDeValor.Embarcador.Logistica.TorreMonitoramento>();

            AdicionarRegistrosAoGrid(listaConsulta, grid, configuracao, unitOfWork);
            grid.setarQuantidadeTotal(totalRegistros);
            return grid;
        }

        public void AdicionarRegistrosAoGrid(IList<Dominio.ObjetosDeValor.Embarcador.Logistica.TorreMonitoramento> lista, Models.Grid.Grid grid, Dominio.Entidades.Embarcador.Configuracao.ConfiguracaoTMS configuracao, Repositorio.UnitOfWork unitOfWork)
        {
            var listaRetornar = (
                from reg in lista
                select new
                {
                    Codigo = reg.Codigo,
                    reg.Carga,
                    reg.Veiculo,
                    reg.IDEquipamento,
                    DataInicioMonitoramento = reg.DataInicioMonitoramentoFormatada,
                    DataFimMonitoramento = reg.DataFimMonitoramentoFormatada,
                    reg.CargaEmbarcador,
                    DataCriacaoCarga = reg.DataCriacaoCargaFormatada,
                    DataInicioViagem = reg.DataInicioViagemFormatada,
                    DataCarregamentoCarga = reg.DataCarregamentoCargaFormatada,
                    AntecedenciaGrade = reg.AntecedenciaGrade > 0 ? "OK" : "NOK",
                    reg.Destinos,
                    reg.CidadeDestino,
                    reg.TipoCarga,
                    reg.Posicao,
                    reg.Tracao,
                    reg.Reboques,
                    reg.Transportador,
                    reg.Pedidos,
                    Ordens = reg.Ordens != null ? reg.Ordens : "",
                    reg.Filial,
                    TempoViagem = reg.DataPosicaoAtual.HasValue && reg.DataInicioViagem.HasValue ? FormatarTempo((reg.DataInicioViagem.Value - reg.DataPosicaoAtual.Value)) : "",
                    reg.DataCheckinDescargaFormatada,
                    ValorFrete = reg.ValorFrete.HasValue ? reg.ValorFrete.Value.ToString("f2") : "0,00",
                    DistanciaAteOrigem = (reg.DistanciaAteOrigem != 0 && reg.DistanciaAteOrigem != null) ? reg.DistanciaAteOrigem?.ToString("f1") : "0",
                    DistanciaPercorrida = (reg.DistanciaRealizada != 0) ? reg.DistanciaRealizada.ToString("f1") : "0",
                    DataCarregamento = ObterDataCarregamento(reg, configuracao),
                    OcorrenciasDeslocamento = reg.OcorrenciasDeslocamento.HasValue ? reg.OcorrenciasDeslocamento.Value.ToString("f2") : "0,00",
                    OcorrenciasFreteNegociado = reg.OcorrenciasFreteNegociado.HasValue ? reg.OcorrenciasDeslocamento.Value.ToString("f2") : "0,00",
                    OutrasOcorrencias = reg.OutrasOcorrencias.HasValue ? reg.OutrasOcorrencias.Value.ToString("f2") : "0,00",
                    TempoDescarga = reg.TempoDescarga.HasValue ? FormatarTempo(TimeSpan.FromSeconds(reg.TempoDescarga.Value)) : "0",
                    DataInicioDescarga = reg.DataInicioDescargaFormatada,
                    DataFimDescarga = reg.DataFimDescargaFormatada,
                    DataChegadaPlanta = reg.DataChegadaPlantaFormatada,
                    Insumos = reg.DataPosicaoAtual.HasValue ? "" : VerificarCargaMonitoradaVeiculoInsumos(reg.Veiculo, reg.Carga),
                    CalculoDiaria = "",
                    ValorDiaria = 0,
                    StatusAtual = ObterStatusAtualCarga(reg, configuracao, unitOfWork),
                    KMRodado = "",//kmViagem(reg.PolilinhaPrevista),
                    kmRodadoDia = "550 km",
                    DataPrevisaoChegadaPlanta = reg.DataPrevisaoChegadaPlantaFormatada,
                    StatusPrevisaoChegada = reg.DataPrevisaoChegadaPlanta.HasValue && reg.DataInicioCarregamentoJanela.HasValue ? reg.DataPrevisaoChegadaPlanta.Value <= reg.DataInicioCarregamentoJanela.Value ? "NA DATA" : "ATRASO" : "SEM DATA",
                    StatusRealChegada = reg.DataPrevisaoChegadaPlanta.HasValue && reg.DataInicioCarregamentoJanela.HasValue ? reg.DataPrevisaoChegadaPlanta.Value <= reg.DataInicioCarregamentoJanela.Value ? "NA DATA" : "ATRASO" : "SEM DATA",
                    Rastreador = (reg.DataPosicaoAtual.HasValue && (DateTime.Now - reg.DataPosicaoAtual.Value).TotalMinutes <= configuracao.TempoSemPosicaoParaVeiculoPerderSinal).ToString(),
                    Status = Dominio.ObjetosDeValor.Embarcador.Enumeradores.MonitoramentoStatusHelper.ObterDescricao(reg.Status),
                }).ToList();

            // Ordenação de colunas que não são do banco de dados
            Dominio.ObjetosDeValor.Embarcador.Consulta.ParametroConsulta parametrosConsulta = grid.ObterParametrosConsulta(null);
            string direcaoOrdenar = parametrosConsulta.DirecaoOrdenar?.ToLower();
            string colunaOrdenacaoManual = (parametrosConsulta.PropriedadeOrdenar != null) ? parametrosConsulta.PropriedadeOrdenar : String.Empty;
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

        }

        #endregion

        #region Métodos privados

        public static string FormatarTempo(TimeSpan tempo)
        {
            string formato = String.Empty;
            if (tempo.Days > 0)
            {
                formato = $"{tempo.Days}d";
            }
            formato += tempo.ToString(@"hh\:mm");
            return formato;
        }

        private string ObterStatusAtualCarga(Dominio.ObjetosDeValor.Embarcador.Logistica.TorreMonitoramento monitoramento, Dominio.Entidades.Embarcador.Configuracao.ConfiguracaoTMS configuracao, Repositorio.UnitOfWork unitOfWork)
        {
            Repositorio.Embarcador.Cargas.Carga repcarga = new Repositorio.Embarcador.Cargas.Carga(unitOfWork);
            Dominio.Entidades.Embarcador.Cargas.Carga Carga = repcarga.BuscarPorCodigo(monitoramento.Carga);

            if (Carga.Empresa == null)
            {
                //sem transportador
                return "Necessidade";
            }
            else if (Carga.Empresa != null && Carga.Veiculo == null && Carga.Motoristas.Count <= 0)
            {
                return "Contratação";
            }
            else if (Carga.Empresa != null && Carga.Veiculo != null && Carga.Motoristas.Count > 0)
            {
                return "Em trânsito";
            }
            else if (Carga.Empresa != null && Carga.Veiculo != null && Carga.Motoristas.Count > 0 && monitoramento.DataEntradaOrigem.HasValue)
            {
                return "Em planta";
            }
            else
                return "";
        }

        private string ObterDataCarregamento(Dominio.ObjetosDeValor.Embarcador.Logistica.TorreMonitoramento monitoramento, Dominio.Entidades.Embarcador.Configuracao.ConfiguracaoTMS configuracao)
        {
            DateTime? data;
            switch (configuracao.DataBaseCalculoPrevisaoControleEntrega)
            {
                case Dominio.ObjetosDeValor.Embarcador.Enumeradores.DataBaseCalculoPrevisaoControleEntrega.DataPrevisaoTerminoCarga:
                    data = monitoramento.DataPrevisaoTerminoCarga;
                    break;
                case Dominio.ObjetosDeValor.Embarcador.Enumeradores.DataBaseCalculoPrevisaoControleEntrega.DataInicioViagemPrevista:
                    data = monitoramento.DataInicioViagemPrevista;
                    break;
                case Dominio.ObjetosDeValor.Embarcador.Enumeradores.DataBaseCalculoPrevisaoControleEntrega.DataCarregamentoCarga:
                    data = monitoramento.DataCarregamentoCarga;
                    break;
                case Dominio.ObjetosDeValor.Embarcador.Enumeradores.DataBaseCalculoPrevisaoControleEntrega.DataInicioCarregamentoJanela:
                    data = monitoramento.DataInicioCarregamentoJanela;
                    break;
                case Dominio.ObjetosDeValor.Embarcador.Enumeradores.DataBaseCalculoPrevisaoControleEntrega.DataCriacaoCarga:
                default:
                    data = monitoramento.DataCriacaoCarga;
                    break;
            }
            if (data.HasValue)
                return data.Value.ToString("dd/MM/yyyy HH:mm");
            else
                return string.Empty;
        }

        private string VerificarCargaMonitoradaVeiculoInsumos(int codVeiculo, int CodCargaOrigem)
        {
            //TODO: BUSCAR CARGA EM MONITORAMENTO DO VEICULO E VERIFICAR SE CARGA É DO TIPO INSUMO E SE O DESTINO DESSA CARGA É IGUAL A ORIGEM DA CARGA CODCARGAORIGEM, MOSTRAR N CARGA.

            return "";
        }

        //private decimal kmViagem(string polilinhaPrevista, string polilinhaRealizada)
        //{
        //    decimal distanciaAproximadaRealizada = 0;

        //   if (!string.IsNullOrWhiteSpace(polilinhaPrevista))
        //    {

        //        List<Dominio.ObjetosDeValor.Embarcador.Logistica.WayPoint> wayPointsPrevistos = Servicos.Embarcador.Logistica.Polilinha.Decodificar(polilinhaPrevista);
        //        List<Dominio.ObjetosDeValor.Embarcador.Logistica.WayPoint> wayPointsRealizados = Servicos.Embarcador.Logistica.Polilinha.Decodificar(polilinhaRealizada);


        //        if (wayPointsRealizados.Count > 0)
        //        {
        //            distanciaAproximadaRealizada = (decimal)Servicos.Embarcador.Logistica.Polilinha.CalcularDistancia(wayPointsPrevistos, wayPointsRealizados.First());
        //            distanciaAproximadaRealizada /= 1000; // m para km
        //        }
        //    }

        //    return distanciaAproximadaRealizada;
        //}

        #endregion

    }
}