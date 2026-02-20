using System;
using System.Collections.Generic;
using System.Linq;

namespace Servicos.Embarcador.Monitoramento
{
    public class MonitoramentoEventoParada
    {

        public static string DescricaoParadaNaRota = "Parada na rota";

        #region Métodos públicos

        public static List<Dominio.ObjetosDeValor.Embarcador.Logistica.Parada> ObterParadasUnicasPriorizandoAlertas(Repositorio.UnitOfWork unitOfWork, int codigoVeiculo, DateTime dataInicial, DateTime dataFinal, Dominio.Entidades.Embarcador.Configuracao.ConfiguracaoTMS configuracao, List<int> codigosVeiculos = null)
        {
            List<Dominio.ObjetosDeValor.Embarcador.Logistica.Parada> paradasAlertas = ObterAlertasParadas(unitOfWork, 0, codigoVeiculo, dataInicial, dataFinal, codigosVeiculos);
            List<Dominio.ObjetosDeValor.Embarcador.Logistica.Parada> paradasNaRota = ObterParadasNaRota(unitOfWork, 0, codigoVeiculo, dataInicial, dataFinal, configuracao, codigosVeiculos);

            List<Dominio.ObjetosDeValor.Embarcador.Logistica.Parada> paradas = paradasAlertas.Concat(paradasNaRota).ToList();
            paradas.Sort((x, y) => x.DataInicio.CompareTo(y.DataInicio));

            return UnificarParadasPriorizandoAlertas(paradas);
        }

        public static List<Dominio.ObjetosDeValor.Embarcador.Logistica.Parada> ObterParadasUnicasPriorizandoAlertas(Repositorio.UnitOfWork unitOfWork, Dominio.Entidades.Embarcador.Logistica.Monitoramento monitoramento, DateTime? dataInicial, DateTime? dataFinal, Dominio.Entidades.Embarcador.Configuracao.ConfiguracaoTMS configuracao)
        {
            List<Dominio.ObjetosDeValor.Embarcador.Logistica.Parada> paradasAlertas = ObterAlertasParadas(unitOfWork, monitoramento.Carga.Codigo, 0, dataInicial, dataFinal);
            List<Dominio.ObjetosDeValor.Embarcador.Logistica.Parada> paradasNaRota = ObterParadasNaRota(unitOfWork, monitoramento, dataInicial, dataFinal, configuracao);

            // Agrupa e ordena as paradas do monitoramento
            List<Dominio.ObjetosDeValor.Embarcador.Logistica.Parada> paradas = paradasAlertas.Concat(paradasNaRota).ToList();
            paradas.Sort((x, y) => x.DataInicio.CompareTo(y.DataInicio));

            return UnificarParadasPriorizandoAlertas(paradas);
        }

        public static List<Dominio.ObjetosDeValor.Embarcador.Logistica.Parada> ObterAlertasParadas(Repositorio.UnitOfWork unitOfWork, int codigoCarga, int codigoVeiculo, DateTime? dataInicial, DateTime? dataFinal, List<int> codigosVeiculos = null)
        {
            Repositorio.Embarcador.Logistica.AlertaMonitor repAlertaMonitor = new Repositorio.Embarcador.Logistica.AlertaMonitor(unitOfWork);
            List<Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoAlerta> tiposAlerta = new List<Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoAlerta>();
            tiposAlerta.Add(Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoAlerta.ParadaEmAreaDeRisco);
            tiposAlerta.Add(Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoAlerta.ParadaExcessiva);
            tiposAlerta.Add(Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoAlerta.ParadaNaoProgramada);
            Repositorio.Embarcador.Logistica.Posicao repPosicao = new Repositorio.Embarcador.Logistica.Posicao(unitOfWork);

            Dictionary<int, List<Dominio.Entidades.Embarcador.Logistica.Posicao>> posicoesPorVeiculo = new Dictionary<int, List<Dominio.Entidades.Embarcador.Logistica.Posicao>>();
            Dictionary<int, List<Dominio.Entidades.Embarcador.Logistica.AlertaMonitor>> paradasPorVeiculo = new Dictionary<int, List<Dominio.Entidades.Embarcador.Logistica.AlertaMonitor>>();

            if (codigoCarga > 0)
            {
                paradasPorVeiculo = repAlertaMonitor
                    .BuscarAlertasPorCargaETipoDeAlerta(codigoCarga, tiposAlerta, dataInicial, dataFinal)
                    .GroupBy(posicao => posicao.Veiculo.Codigo)
                    .ToDictionary(group => group.Key, group => group.OrderBy(pos => pos.Data).ToList());
            }
            else if (codigoVeiculo > 0)
            {
                paradasPorVeiculo = repAlertaMonitor
                    .BuscarAlertasPorVeiculoETipoDeAlerta(codigoVeiculo, tiposAlerta, dataInicial, dataFinal)
                    .GroupBy(posicao => posicao.Veiculo.Codigo)
                    .ToDictionary(group => group.Key, group => group.OrderBy(pos => pos.Data).ToList());
            }
            else if (codigosVeiculos != null)
            {
                paradasPorVeiculo = repAlertaMonitor
                    .BuscarAlertasPorVeiculoETipoDeAlerta(codigosVeiculos, tiposAlerta, dataInicial, dataFinal)
                    .GroupBy(posicao => posicao.Veiculo.Codigo)
                    .ToDictionary(group => group.Key, group => group.OrderBy(pos => pos.Data).ToList());

                posicoesPorVeiculo = repPosicao
                    .BuscarPosicaoVeiculos(codigosVeiculos, dataInicial.Value, dataFinal.Value)
                    .GroupBy(posicao => posicao.Veiculo.Codigo)
                    .ToDictionary(group => group.Key, group => group.OrderBy(pos => pos.DataVeiculo).ToList());
            }


            List<Dominio.ObjetosDeValor.Embarcador.Logistica.Parada> paradas = new List<Dominio.ObjetosDeValor.Embarcador.Logistica.Parada>();

            foreach (var paradasNoVeiculo in paradasPorVeiculo.Values)
            {
                if (paradasNoVeiculo.Any())
                {
                    List<Dominio.Entidades.Embarcador.Logistica.Posicao> posicoes = posicoesPorVeiculo
                        .Where(x => x.Key == paradasNoVeiculo.FirstOrDefault().Veiculo.Codigo)
                        .SelectMany(x => x.Value)
                        .ToList();
                    IdentificarAlertasParadas(paradas, paradasNoVeiculo, posicoes, repPosicao);
                }
            }

            return paradas;
        }

        private static void IdentificarAlertasParadas(List<Dominio.ObjetosDeValor.Embarcador.Logistica.Parada> paradas, List<Dominio.Entidades.Embarcador.Logistica.AlertaMonitor> paradasVeiculo, List<Dominio.Entidades.Embarcador.Logistica.Posicao> posicoes, Repositorio.Embarcador.Logistica.Posicao repPosicao)
        {
            int total = paradasVeiculo.Count;
            for (int i = 0; i < total; i++)
            {
                DateTime dataInicio = paradasVeiculo[i].Data;
                DateTime dataFim = paradasVeiculo[i].Data;
                string descricao = "";
                if (!string.IsNullOrEmpty(paradasVeiculo[i].AlertaDescricao))
                {
                    string[] partes = paradasVeiculo[i].AlertaDescricao.Split(new string[] { ": " }, StringSplitOptions.None);
                    if (partes.Length > 0)
                    {
                        descricao = partes[0];
                        partes = descricao.Split(new string[] { " até " }, StringSplitOptions.None);
                        if (partes.Length == 2)
                        {
                            try
                            {
                                dataInicio = DateTime.Parse(partes[0].Replace("De", "").Trim());
                                dataFim = DateTime.Parse(partes[1].Trim());
                            }
                            catch (Exception ex)
                            {
                                Servicos.Log.TratarErro($"[Arquitetura-CatchNoAction] Erro ao converter datas do evento de parada: {ex.ToString()}", "CatchNoAction");
                            }
                        }
                    }
                }
                else
                {
                    descricao = string.IsNullOrEmpty(paradasVeiculo[i].AlertaDescricao) ? "" : paradasVeiculo[i].AlertaDescricao;
                }

                Dominio.Entidades.Embarcador.Logistica.Posicao posicao = null;
                if (posicoes != null)
                    posicao = repPosicao.BuscarUltimaPosicaoVeiculo(paradasVeiculo[i].Veiculo.Codigo, dataInicio, dataFim, posicoes);
                else
                    posicao = repPosicao.BuscarUltimaPosicaoVeiculo(paradasVeiculo[i].Veiculo.Codigo, dataInicio, dataFim);

                if (paradasVeiculo[i].Latitude != null && paradasVeiculo[i].Longitude != null)
                {
                    paradas.Add(new Dominio.ObjetosDeValor.Embarcador.Logistica.Parada
                    {
                        CodigoVeiculo = paradasVeiculo[i].Veiculo.Codigo,
                        Posicao = posicao,
                        Alerta = true,
                        Placa = paradasVeiculo[i].Veiculo?.Placa,
                        Tipo = paradasVeiculo[i].Descricao,
                        Descricao = descricao,
                        Latitude = (double)paradasVeiculo[i].Latitude,
                        Longitude = (double)paradasVeiculo[i].Longitude,
                        DataInicio = dataInicio,
                        DataFim = dataFim,
                        Tempo = dataFim - dataInicio
                    });
                }
            }
        }

        public static List<Dominio.ObjetosDeValor.Embarcador.Logistica.Parada> ObterParadasNaRota(Repositorio.UnitOfWork unitOfWork, Dominio.Entidades.Embarcador.Logistica.Monitoramento monitoramento, DateTime? dataInicial, DateTime? datafinal, Dominio.Entidades.Embarcador.Configuracao.ConfiguracaoTMS configuracao)
        {
            List<Dominio.ObjetosDeValor.Embarcador.Logistica.Parada> paradas = new List<Dominio.ObjetosDeValor.Embarcador.Logistica.Parada>();
            if (monitoramento != null && monitoramento.Carga != null && monitoramento.Veiculo != null)
            {
                DateTime DataInicio = dataInicial.HasValue ? dataInicial.Value : monitoramento.DataInicio.HasValue ? monitoramento.DataInicio.Value : monitoramento.Carga.DataInicioViagem.HasValue ? monitoramento.Carga.DataInicioViagem.Value : monitoramento.Carga.DataCriacaoCarga;
                DateTime DataFim = datafinal.HasValue ? datafinal.Value : monitoramento.DataFim ?? DateTime.Now;
                paradas = ObterParadasNaRota(unitOfWork, monitoramento.Codigo, monitoramento.Veiculo.Codigo, DataInicio, DataFim, configuracao);
            }
            return paradas;
        }

        public static List<Dominio.ObjetosDeValor.Embarcador.Logistica.Parada> ObterParadasNaRota(Repositorio.UnitOfWork unitOfWork, int codigoMonitoramento, int codigoVeiculo, DateTime dataInicial, DateTime datafinal, Dominio.Entidades.Embarcador.Configuracao.ConfiguracaoTMS configuracao, List<int> codigosVeiculo = null)
        {
            List<Dominio.ObjetosDeValor.Embarcador.Logistica.Parada> paradas = new List<Dominio.ObjetosDeValor.Embarcador.Logistica.Parada>();

            if (configuracao.IdentificarVeiculoParado && configuracao.IdentificarVeiculoParadoDistancia > 0 && configuracao.IdentificarVeiculoParadoTempo > 0)
            {
                Dictionary<int, List<Dominio.Entidades.Embarcador.Logistica.Posicao>> posicoesPorVeiculo = new Dictionary<int, List<Dominio.Entidades.Embarcador.Logistica.Posicao>>();

                if (codigoMonitoramento > 0)
                {
                    Repositorio.Embarcador.Logistica.MonitoramentoVeiculoPosicao repMonitoramentoVeiculoPosicao = new Repositorio.Embarcador.Logistica.MonitoramentoVeiculoPosicao(unitOfWork);
                    posicoesPorVeiculo = repMonitoramentoVeiculoPosicao
                        .BuscarPorMonitoramentoEVeiculoDataInicialeFinal(codigoMonitoramento, codigoVeiculo, dataInicial, datafinal)
                        .GroupBy(posicao => posicao.Veiculo.Codigo)
                        .ToDictionary(group => group.Key, group => group.OrderBy(pos => pos.DataVeiculo).ToList());

                }
                else
                {
                    Repositorio.Embarcador.Logistica.Posicao repPosicao = new Repositorio.Embarcador.Logistica.Posicao(unitOfWork);
                    posicoesPorVeiculo = repPosicao
                        .BuscarPorVeiculoDataInicialeFinal(codigoVeiculo, dataInicial, datafinal, "", codigosVeiculo)
                        .GroupBy(posicao => posicao.Veiculo.Codigo)
                        .ToDictionary(group => group.Key, group => group.OrderBy(pos => pos.DataVeiculo).ToList());
                }

                foreach (var posicoesDoVeiculo in posicoesPorVeiculo.Values)
                {
                    if (posicoesDoVeiculo.Any())
                        IdentificarParadas(configuracao, paradas, posicoesDoVeiculo);
                }
            }

            return paradas;
        }

        private static void IdentificarParadas(Dominio.Entidades.Embarcador.Configuracao.ConfiguracaoTMS configuracao, List<Dominio.ObjetosDeValor.Embarcador.Logistica.Parada> paradas, List<Dominio.Entidades.Embarcador.Logistica.Posicao> posicoesDoVeiculo)
        {

            double tempoEntrePosicoes, distanciaEntrePosicoes;
            Dominio.Entidades.Embarcador.Logistica.Posicao posicaoBase, posicaoComparada;
            Dominio.ObjetosDeValor.Embarcador.Logistica.Parada parada = null;
            int total = posicoesDoVeiculo.Count;

            for (int i = 0; i < total; i++)
            {
                posicaoBase = posicoesDoVeiculo[i];

                for (int j = i + 1; j < total; j++)
                {
                    posicaoComparada = posicoesDoVeiculo[j];

                    if (posicaoBase.DataVeiculo <= posicaoComparada.DataVeiculo)
                    {
                        distanciaEntrePosicoes = Servicos.Embarcador.Logistica.Polilinha.CalcularDistancia(posicaoBase.Latitude, posicaoBase.Longitude, posicaoComparada.Latitude, posicaoComparada.Longitude);

                        if (distanciaEntrePosicoes <= configuracao.IdentificarVeiculoParadoDistancia)
                        {
                            tempoEntrePosicoes = (posicaoComparada.DataVeiculo - posicaoBase.DataVeiculo).TotalMinutes;

                            if (tempoEntrePosicoes >= configuracao.IdentificarVeiculoParadoTempo)
                            {
                                if (parada == null)
                                {
                                    parada = new Dominio.ObjetosDeValor.Embarcador.Logistica.Parada
                                    {
                                        CodigoVeiculo = posicaoBase.Veiculo.Codigo,
                                        Posicao = posicaoBase,
                                        Alerta = false,
                                        Tipo = MonitoramentoEventoParada.DescricaoParadaNaRota,
                                        Placa = posicaoBase.Veiculo.Placa,
                                        DataInicio = TrimMinutes(posicaoBase.DataVeiculo),
                                        Latitude = posicaoBase.Latitude,
                                        Longitude = posicaoBase.Longitude,
                                        Descricao = posicaoBase.Descricao
                                    };
                                }

                                parada.DataFim = TrimMinutes(posicaoComparada.DataVeiculo);
                                parada.Tempo = parada.DataFim - parada.DataInicio;
                                parada.Descricao = Servicos.Embarcador.Monitoramento.AlertaMonitor.FormatarTextoParada(parada.DataInicio, parada.DataFim, parada.Tempo);
                            }
                            i = j;
                        }
                        else
                        {
                            if (parada != null)
                            {
                                paradas.Add(parada);
                                parada = null;
                            }

                            break;
                        }
                    }
                }
            }

            if (parada != null)
            {
                paradas.Add(parada);
            }

        }

        public static List<Dominio.Relatorios.Embarcador.DataSource.Logistica.MonitoramentoParadaVeiculo> ConsultarRelatorioParada(Repositorio.UnitOfWork unitOfWork, Dominio.ObjetosDeValor.Embarcador.Logistica.FiltroPesquisaRelatorioMonitoramentoVeiculo filtrosPesquisa, List<Dominio.Relatorios.Embarcador.ObjetosDeValor.PropriedadeAgrupamento> propriedades, Dominio.ObjetosDeValor.Embarcador.Consulta.ParametroConsulta parametrosConsulta, bool apenasMonitoramentosFinalizados)
        {
            // Localiza os veículos
            Repositorio.Embarcador.Logistica.Monitoramento repMonitoramento = new Repositorio.Embarcador.Logistica.Monitoramento(unitOfWork);
            IList<Dominio.Relatorios.Embarcador.DataSource.Logistica.MonitoramentoParadaVeiculo> veiculos = repMonitoramento.ConsultarParadasVeiculos(filtrosPesquisa, propriedades, parametrosConsulta);
            List<Dominio.Relatorios.Embarcador.DataSource.Logistica.MonitoramentoParadaVeiculo> veiculosParadas = new List<Dominio.Relatorios.Embarcador.DataSource.Logistica.MonitoramentoParadaVeiculo>();
            if (veiculos != null && veiculos.Count > 0)
            {
                Repositorio.Embarcador.Configuracao.ConfiguracaoTMS repConfiguracaoTMS = new Repositorio.Embarcador.Configuracao.ConfiguracaoTMS(unitOfWork);
                Dominio.Entidades.Embarcador.Configuracao.ConfiguracaoTMS configuracao = repConfiguracaoTMS.BuscarConfiguracaoPadrao();

                Repositorio.Embarcador.Logistica.PosicaoAlvo repPosicaoAlvo = new Repositorio.Embarcador.Logistica.PosicaoAlvo(unitOfWork);
                Repositorio.Localidade repLocalidade = new Repositorio.Localidade(unitOfWork);
                List<Dominio.Entidades.Localidade> localidades = repLocalidade.BuscarLocalidadesComCoordenadas();
                List<Dominio.ObjetosDeValor.Embarcador.Logistica.Parada> todasAsParadas = new List<Dominio.ObjetosDeValor.Embarcador.Logistica.Parada>();

                if (apenasMonitoramentosFinalizados)
                    todasAsParadas = ObterParadasAlertasFinalizados(unitOfWork, veiculos.Select(x => x.CodigoVeiculo).ToList(), filtrosPesquisa.DataInicial, filtrosPesquisa.DataFinal);

                else
                    todasAsParadas = ObterParadasUnicasPriorizandoAlertas(unitOfWork, -1, filtrosPesquisa.DataInicial, filtrosPesquisa.DataFinal, configuracao, veiculos.Select(x => x.CodigoVeiculo).ToList());

                List<Dominio.Entidades.Embarcador.Logistica.Monitoramento> monitoramentos = repMonitoramento.BuscarUltimoPorVeiculoPeriodo(veiculos.Select(x => x.CodigoVeiculo).ToList(), filtrosPesquisa.DataInicial, filtrosPesquisa.DataFinal);

                int total = veiculos.Count;
                for (int i = 0; i < total; i++)
                {
                    // Identifica as paradas do veículo
                    List<Dominio.ObjetosDeValor.Embarcador.Logistica.Parada> paradasDoVeiculo = todasAsParadas.Where(x => x.CodigoVeiculo == veiculos[i].CodigoVeiculo).ToList();
                    int totalParadas = paradasDoVeiculo.Count;
                    for (int j = 0; j < totalParadas; j++)
                    {

                        // Identifica um possível monitoramento de carga para o veículo no período da parada
                        Dominio.Entidades.Embarcador.Logistica.Monitoramento monitoramento = repMonitoramento.BuscarUltimoPorVeiculoPeriodo(monitoramentos, veiculos[i].CodigoVeiculo, paradasDoVeiculo[j].DataInicio, paradasDoVeiculo[j].DataFim);

                        // Filtro da filial
                        if (filtrosPesquisa.CodigoFilial == 0 || (monitoramento != null && monitoramento.Carga != null && monitoramento.Carga.Filial != null && monitoramento.Carga.Filial.Codigo == filtrosPesquisa.CodigoFilial))
                        {
                            // Descrição da posição com os clientes
                            string descricao = string.Empty;
                            if (paradasDoVeiculo[j].Posicao != null)
                            {
                                List<Dominio.Entidades.Embarcador.Logistica.PosicaoAlvo> posicaoAlvos = repPosicaoAlvo.BuscarPorPosicao(paradasDoVeiculo[j].Posicao.Codigo);
                                int totalPosicoes = posicaoAlvos.Count;
                                if (totalPosicoes > 0)
                                {
                                    for (int k = 0; k < totalPosicoes; k++)
                                    {
                                        descricao += posicaoAlvos[k].Cliente.NomeFantasia + "; ";
                                    }
                                    descricao = descricao.Substring(0, descricao.Length - 2) + ". ";
                                }
                                descricao += paradasDoVeiculo[j].Posicao.Descricao;
                            }

                            string localidade = "Localidade não encontrada";
                            if (paradasDoVeiculo[j].Posicao != null)
                                localidade = localidades.Aggregate((x, y) => Math.Abs(Servicos.Embarcador.Logistica.Polilinha.CalcularDistancia((double)x.Latitude, (double)x.Longitude, paradasDoVeiculo[j].Posicao.Latitude, paradasDoVeiculo[j].Posicao.Longitude)) < Math.Abs(Servicos.Embarcador.Logistica.Polilinha.CalcularDistancia((double)y.Latitude, (double)y.Longitude, paradasDoVeiculo[j].Posicao.Latitude, paradasDoVeiculo[j].Posicao.Longitude)) ? x : y).DescricaoCidadeEstado;

                            if (monitoramento != null && monitoramento.DataInicio != null && paradasDoVeiculo[j].Tipo == MonitoramentoEventoParada.DescricaoParadaNaRota)
                            {

                                // O alerta iniciou antes do início do monitoramento, deve quebrar a parada entre antes e depois do início monitoramento
                                if (paradasDoVeiculo[j].DataInicio < monitoramento.DataInicio.Value && paradasDoVeiculo[j].DataFim > monitoramento.DataInicio.Value)
                                {
                                    // Tempo mínimo FORA do monitoramento
                                    TimeSpan tempoAlertaAntesFora = monitoramento.DataInicio.Value - paradasDoVeiculo[j].DataInicio;

                                    // Tempo mínimo DENTRO do monitoramento
                                    TimeSpan tempoAlertaAntesDentro = paradasDoVeiculo[j].DataFim - monitoramento.DataInicio.Value;

                                    DateTime dataFim = paradasDoVeiculo[j].DataFim;

                                    if (tempoAlertaAntesFora.TotalMinutes > configuracao.IdentificarVeiculoParadoTempo)
                                    {
                                        paradasDoVeiculo[j].DataFim = monitoramento.DataInicio.Value;
                                        paradasDoVeiculo[j].Tempo = TimeSpan.FromMinutes((int)(paradasDoVeiculo[j].DataFim - paradasDoVeiculo[j].DataInicio).TotalMinutes);
                                        paradasDoVeiculo[j].Descricao = Servicos.Embarcador.Monitoramento.AlertaMonitor.FormatarTextoParada(paradasDoVeiculo[j].DataInicio, paradasDoVeiculo[j].DataFim, paradasDoVeiculo[j].Tempo);
                                        AddParada(veiculosParadas, veiculos[i], paradasDoVeiculo[j], descricao, null, localidade);
                                    }

                                    if (tempoAlertaAntesDentro.TotalMinutes > configuracao.IdentificarVeiculoParadoTempo)
                                    {
                                        paradasDoVeiculo[j].DataInicio = monitoramento.DataInicio.Value;
                                        paradasDoVeiculo[j].DataFim = dataFim;
                                        paradasDoVeiculo[j].Tempo = TimeSpan.FromMinutes((int)(paradasDoVeiculo[j].DataFim - paradasDoVeiculo[j].DataInicio).TotalMinutes);
                                        paradasDoVeiculo[j].Descricao = Servicos.Embarcador.Monitoramento.AlertaMonitor.FormatarTextoParada(paradasDoVeiculo[j].DataInicio, paradasDoVeiculo[j].DataFim, paradasDoVeiculo[j].Tempo);
                                        AddParada(veiculosParadas, veiculos[i], paradasDoVeiculo[j], descricao, monitoramento, localidade);
                                    }
                                }
                                // O alerta iniciou durante o monitoramento e acabou depois, deve quebrar a parada entre antes e depois do fim monitoramento
                                else if (monitoramento.DataFim != null && paradasDoVeiculo[j].DataInicio > monitoramento.DataInicio.Value && paradasDoVeiculo[j].DataInicio < monitoramento.DataFim.Value && paradasDoVeiculo[j].DataFim > monitoramento.DataFim.Value)
                                {
                                    // Tempo mínimo DENTRO do monitoramento
                                    TimeSpan tempoAlertaDepoisDentro = monitoramento.DataFim.Value - paradasDoVeiculo[j].DataInicio;

                                    // Tempo mínimo FORA do monitoramento
                                    TimeSpan tempoAlertaDepoisFora = paradasDoVeiculo[j].DataFim - monitoramento.DataFim.Value;

                                    DateTime dataFim = paradasDoVeiculo[j].DataFim;

                                    if (tempoAlertaDepoisDentro.TotalMinutes > configuracao.IdentificarVeiculoParadoTempo)
                                    {
                                        paradasDoVeiculo[j].DataFim = monitoramento.DataFim.Value;
                                        paradasDoVeiculo[j].Tempo = TimeSpan.FromMinutes((int)(paradasDoVeiculo[j].DataFim - paradasDoVeiculo[j].DataInicio).TotalMinutes);
                                        paradasDoVeiculo[j].Descricao = Servicos.Embarcador.Monitoramento.AlertaMonitor.FormatarTextoParada(paradasDoVeiculo[j].DataInicio, paradasDoVeiculo[j].DataFim, paradasDoVeiculo[j].Tempo);
                                        AddParada(veiculosParadas, veiculos[i], paradasDoVeiculo[j], descricao, monitoramento, localidade);
                                    }

                                    if (tempoAlertaDepoisFora.TotalMinutes > configuracao.IdentificarVeiculoParadoTempo)
                                    {
                                        paradasDoVeiculo[j].DataInicio = monitoramento.DataFim.Value;
                                        paradasDoVeiculo[j].DataFim = dataFim;
                                        paradasDoVeiculo[j].Tempo = TimeSpan.FromMinutes((int)(paradasDoVeiculo[j].DataFim - paradasDoVeiculo[j].DataInicio).TotalMinutes);
                                        paradasDoVeiculo[j].Descricao = Servicos.Embarcador.Monitoramento.AlertaMonitor.FormatarTextoParada(paradasDoVeiculo[j].DataInicio, paradasDoVeiculo[j].DataFim, paradasDoVeiculo[j].Tempo);
                                        AddParada(veiculosParadas, veiculos[i], paradasDoVeiculo[j], descricao, null, localidade);
                                    }
                                }
                                else
                                {
                                    AddParada(veiculosParadas, veiculos[i], paradasDoVeiculo[j], descricao, monitoramento, localidade);
                                }

                            }
                            else
                            {
                                AddParada(veiculosParadas, veiculos[i], paradasDoVeiculo[j], descricao, monitoramento, localidade);
                            }
                        }
                    }
                }
            }

            return veiculosParadas;
        }

        #endregion

        #region Métodos privados

        private static List<Dominio.ObjetosDeValor.Embarcador.Logistica.Parada> UnificarParadasPriorizandoAlertas(List<Dominio.ObjetosDeValor.Embarcador.Logistica.Parada> paradas)
        {
            List<Dominio.ObjetosDeValor.Embarcador.Logistica.Parada> paradasUnicas = new List<Dominio.ObjetosDeValor.Embarcador.Logistica.Parada>();
            int total = paradas.Count();
            for (int i = 0; i < total; i++)
            {
                // Todos os alertas serão mantidos
                if (paradas[i].Alerta)
                {
                    paradasUnicas.Add(paradas[i]);
                }
                else
                {
                    // As paradas na rota serão mantidas se não houver uma parada com alerta na mesma localização
                    bool existe = false;
                    for (int j = 0; j < total; j++)
                    {
                        if (!paradas[i].Alerta && paradas[j].Alerta && paradas[j].Latitude == paradas[i].Latitude && paradas[j].Longitude == paradas[i].Longitude)
                        {
                            existe = true;
                            break;
                        }
                    }
                    if (!existe)
                    {
                        paradasUnicas.Add(paradas[i]);
                    }
                }
            }
            return paradasUnicas;
        }

        private static void AddParada(List<Dominio.Relatorios.Embarcador.DataSource.Logistica.MonitoramentoParadaVeiculo> veiculosParadas, Dominio.Relatorios.Embarcador.DataSource.Logistica.MonitoramentoParadaVeiculo veiculo, Dominio.ObjetosDeValor.Embarcador.Logistica.Parada parada, string descricao, Dominio.Entidades.Embarcador.Logistica.Monitoramento monitoramento, string localidade)
        {

            veiculosParadas.Add(new Dominio.Relatorios.Embarcador.DataSource.Logistica.MonitoramentoParadaVeiculo
            {
                CodigoVeiculo = veiculo.CodigoVeiculo,
                Placa = veiculo.Placa,
                Carga = monitoramento?.Carga?.CodigoCargaEmbarcador ?? string.Empty,
                Filial = monitoramento?.Carga?.Filial?.Descricao,
                Transportador = veiculo.Transportador,
                Situacao = "Parada",
                TipoParada = parada.Tipo,
                DescricaoParada = parada.Descricao,
                DescricaoPosicao = descricao,
                Latitude = parada.Latitude,
                Longitude = parada.Longitude,
                DataInicio = parada.DataInicio,
                DataFim = parada.DataFim,
                Tempo = parada.Tempo,
                LocalidadeAproximada = localidade
            });

        }

        public static List<Dominio.ObjetosDeValor.Embarcador.Logistica.Parada> ObterParadasAlertasFinalizados(Repositorio.UnitOfWork unitOfWork, List<int> codigosVeiculo, DateTime dataInicial, DateTime dataFinal)
        {
            Repositorio.Embarcador.Logistica.ParadasMonitoramentosFinalizados repParadasMonitoramentosFinalizados = new Repositorio.Embarcador.Logistica.ParadasMonitoramentosFinalizados(unitOfWork);
            List<Dominio.Entidades.Embarcador.Logistica.ParadasMonitoramentosFinalizados> paradasMonitoramentosFinalizados = repParadasMonitoramentosFinalizados.BuscarPorDataEVeiculo(codigosVeiculo, dataInicial, dataFinal);
            List<Dominio.ObjetosDeValor.Embarcador.Logistica.Parada> paradasAlertasFinalizados = new List<Dominio.ObjetosDeValor.Embarcador.Logistica.Parada>();

            foreach (Dominio.Entidades.Embarcador.Logistica.ParadasMonitoramentosFinalizados parada in paradasMonitoramentosFinalizados)
            {
                paradasAlertasFinalizados.Add(new Dominio.ObjetosDeValor.Embarcador.Logistica.Parada
                {
                    CodigoVeiculo = parada.Veiculo.Codigo,
                    Alerta = parada.Alerta,
                    Posicao = parada.Posicao,
                    Tipo = parada.Tipo,
                    Descricao = parada.Descricao,
                    Latitude = parada.Latitude,
                    Longitude = parada.Longitude,
                    Placa = parada.Placa,
                    DataInicio = parada.DataInicio,
                    DataFim = parada.DataFim,
                    Tempo = parada.Tempo
                });
            }

            return paradasAlertasFinalizados;
        }

        private static DateTime TrimMinutes(DateTime data)
        {
            return new DateTime(data.Year, data.Month, data.Day, data.Hour, data.Minute, 0);
        }

        #endregion

    }
}

