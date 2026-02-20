using System;
using System.Collections.Generic;
using System.Linq;

namespace Servicos.Embarcador.Monitoramento
{
    public class ParadasMonitoramentosFinalizados
    {
        #region Métodos públicos

        public void GerarParadasMonitoramentosFinalizados(Dominio.Entidades.Embarcador.Logistica.Monitoramento monitoramento, Dominio.Entidades.Embarcador.Configuracao.ConfiguracaoTMS configuracao, Repositorio.UnitOfWork unitOfWork)
        {
            Repositorio.Embarcador.Logistica.ParadasMonitoramentosFinalizados repParadasMonitoramentosFinalizados = new Repositorio.Embarcador.Logistica.ParadasMonitoramentosFinalizados(unitOfWork);

            List<Dominio.ObjetosDeValor.Embarcador.Logistica.Parada> paradasAlertas = ObterAlertasParadas(unitOfWork, monitoramento.Veiculo.Codigo, (DateTime)monitoramento.DataInicio, (DateTime)monitoramento.DataFim);
            List<Dominio.ObjetosDeValor.Embarcador.Logistica.Parada> paradasNaRota = ObterParadasNaRota(unitOfWork, monitoramento.Codigo, monitoramento.Veiculo.Codigo, (DateTime)monitoramento.DataInicio, (DateTime)monitoramento.DataFim, configuracao);
            List<Dominio.ObjetosDeValor.Embarcador.Logistica.Parada> paradasUnificadas = UnificareOrdenarParadas(paradasAlertas, paradasNaRota);

            foreach (Dominio.ObjetosDeValor.Embarcador.Logistica.Parada parada in paradasUnificadas)
            {
                try
                {
                    repParadasMonitoramentosFinalizados.Inserir(GetParadasMonitoramentosFinalizados(parada));
                }
                catch (Exception ex)
                {
                    Servicos.Log.TratarErro(ex);
                }
            }
        }

        #endregion

        #region Métodos privados

        private Dominio.Entidades.Embarcador.Logistica.ParadasMonitoramentosFinalizados GetParadasMonitoramentosFinalizados(Dominio.ObjetosDeValor.Embarcador.Logistica.Parada objetoDeValorParada)
        {
            Dominio.Entidades.Embarcador.Logistica.ParadasMonitoramentosFinalizados entidadeParada = new Dominio.Entidades.Embarcador.Logistica.ParadasMonitoramentosFinalizados()
            {
                Veiculo = new Dominio.Entidades.Veiculo { Codigo = objetoDeValorParada.CodigoVeiculo },
                Posicao = objetoDeValorParada.Posicao,
                Alerta = objetoDeValorParada.Alerta,
                Tipo = objetoDeValorParada.Tipo,
                Descricao = objetoDeValorParada.Descricao,
                Placa = objetoDeValorParada.Placa,
                Latitude = objetoDeValorParada.Latitude,
                Longitude = objetoDeValorParada.Longitude,
                DataInicio = objetoDeValorParada.DataInicio,
                DataFim = objetoDeValorParada.DataFim,
                Tempo = objetoDeValorParada.Tempo
            };
            return entidadeParada;
        }

        private List<Dominio.ObjetosDeValor.Embarcador.Logistica.Parada> ObterAlertasParadas(Repositorio.UnitOfWork unitOfWork, int codigoVeiculo, DateTime? dataInicial, DateTime? dataFinal)
        {
            Repositorio.Embarcador.Logistica.AlertaMonitor repAlertaMonitor = new Repositorio.Embarcador.Logistica.AlertaMonitor(unitOfWork);
            List<Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoAlerta> tiposAlerta = new List<Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoAlerta>() {
                Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoAlerta.ParadaEmAreaDeRisco,
                Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoAlerta.ParadaExcessiva,
                Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoAlerta.ParadaNaoProgramada};
            Repositorio.Embarcador.Logistica.Posicao repPosicao = new Repositorio.Embarcador.Logistica.Posicao(unitOfWork);

            List<Dominio.Entidades.Embarcador.Logistica.AlertaMonitor> paradasPorVeiculo = repAlertaMonitor.BuscarAlertasPorVeiculoETipoDeAlerta(codigoVeiculo, tiposAlerta, dataInicial, dataFinal);

            List<Dominio.ObjetosDeValor.Embarcador.Logistica.Parada> paradas = new List<Dominio.ObjetosDeValor.Embarcador.Logistica.Parada>();
            IdentificarAlertasParadas(paradas, paradasPorVeiculo, repPosicao);

            return paradas;
        }
        private void IdentificarAlertasParadas(List<Dominio.ObjetosDeValor.Embarcador.Logistica.Parada> paradas, List<Dominio.Entidades.Embarcador.Logistica.AlertaMonitor> paradasVeiculo, Repositorio.Embarcador.Logistica.Posicao repPosicao)
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
                                Servicos.Log.TratarErro($"[Arquitetura-CatchNoAction] Erro ao converter datas de paradas finalizadas: {ex.ToString()}", "CatchNoAction");
                            }
                        }
                    }
                }
                else
                {
                    descricao = string.IsNullOrEmpty(paradasVeiculo[i].AlertaDescricao) ? "" : paradasVeiculo[i].AlertaDescricao;
                }

                if (paradasVeiculo[i].Latitude != null && paradasVeiculo[i].Longitude != null)
                {
                    paradas.Add(new Dominio.ObjetosDeValor.Embarcador.Logistica.Parada
                    {
                        CodigoVeiculo = paradasVeiculo[i].Veiculo.Codigo,
                        Posicao = new Dominio.Entidades.Embarcador.Logistica.Posicao() { Codigo = repPosicao.BuscarCodigoUltimaPosicaoVeiculo(paradasVeiculo[i].Veiculo.Codigo, dataInicio, dataFim) },
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

        private List<Dominio.ObjetosDeValor.Embarcador.Logistica.Parada> ObterParadasNaRota(Repositorio.UnitOfWork unitOfWork, int codigoMonitoramento, int codigoVeiculo, DateTime dataInicial, DateTime datafinal, Dominio.Entidades.Embarcador.Configuracao.ConfiguracaoTMS configuracao)
        {
            List<Dominio.ObjetosDeValor.Embarcador.Logistica.Parada> paradas = new List<Dominio.ObjetosDeValor.Embarcador.Logistica.Parada>();

            if (configuracao.IdentificarVeiculoParado && configuracao.IdentificarVeiculoParadoDistancia > 0 && configuracao.IdentificarVeiculoParadoTempo > 0)
            {
                Repositorio.Embarcador.Logistica.MonitoramentoVeiculoPosicao repMonitoramentoVeiculoPosicao = new Repositorio.Embarcador.Logistica.MonitoramentoVeiculoPosicao(unitOfWork);

                List<Dominio.Entidades.Embarcador.Logistica.Posicao> posicoesPorVeiculo = repMonitoramentoVeiculoPosicao.BuscarPorMonitoramentoEVeiculoDataInicialeFinal(codigoMonitoramento, codigoVeiculo, dataInicial, datafinal);
                IdentificarParadasNaRota(configuracao, paradas, posicoesPorVeiculo);
            }

            return paradas;
        }

        private void IdentificarParadasNaRota(Dominio.Entidades.Embarcador.Configuracao.ConfiguracaoTMS configuracao, List<Dominio.ObjetosDeValor.Embarcador.Logistica.Parada> paradas, List<Dominio.Entidades.Embarcador.Logistica.Posicao> posicoesDoVeiculo)
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
        private DateTime TrimMinutes(DateTime data)
        {
            return new DateTime(data.Year, data.Month, data.Day, data.Hour, data.Minute, 0);
        }

        private List<Dominio.ObjetosDeValor.Embarcador.Logistica.Parada> UnificareOrdenarParadas(List<Dominio.ObjetosDeValor.Embarcador.Logistica.Parada> paradasAlertas, List<Dominio.ObjetosDeValor.Embarcador.Logistica.Parada> paradasNaRota)
        {
            List<Dominio.ObjetosDeValor.Embarcador.Logistica.Parada> paradas = paradasAlertas.Concat(paradasNaRota).ToList();
            paradas.Sort((x, y) => x.DataInicio.CompareTo(y.DataInicio));

            return UnificarParadasPriorizandoAlertas(paradas);
        }
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
        #endregion

    }
}

