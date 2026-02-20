using System;
using System.Collections.Generic;
using System.Linq;

namespace Servicos.Embarcador.Monitoramento.Eventos
{
    public abstract class AbstractParada : AbstractEvento
    {

        #region Atributos públicos

        public Dominio.Entidades.Embarcador.Logistica.Locais[] LocaisPernoite { get; set; }
        public Dominio.Entidades.Embarcador.Logistica.Locais[] LocaisAreaDeRisco { get; set; }
        public Dominio.Entidades.Embarcador.Logistica.Locais[] LocaisPontoDeApoio { get; set; }

        #endregion

        #region Construtor protegido

        protected AbstractParada(Repositorio.UnitOfWork unitOfWork, Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoAlerta TipoAlerta) : base(unitOfWork, TipoAlerta)
        {

        }

        #endregion

        #region Métodos protegidos

        protected TimeSpan? TempoEmMovimentoContinuoSemParada(Dominio.ObjetosDeValor.Embarcador.Logistica.MonitoramentoProcessarEvento monitoramentoProcessarEvento, List<Dominio.ObjetosDeValor.Embarcador.Logistica.Posicao> posicoesObjetoValor, List<Dominio.ObjetosDeValor.Embarcador.Logistica.AlertaMonitor> alertas, int tempoEmMovimento, int tempoParado, int raioMetros)
        {
            if (monitoramentoProcessarEvento != null && monitoramentoProcessarEvento.DataVeiculoPosicao != null && monitoramentoProcessarEvento.LatitudePosicao != null && monitoramentoProcessarEvento.LongitudePosicao != null)
            {
                List<Dominio.ObjetosDeValor.Embarcador.Logistica.Posicao> posicoesDoPeriodo = ExtrairPosicoesAposAlertaFechadoAtePosicao(monitoramentoProcessarEvento, posicoesObjetoValor, alertas);

                bool noRaio;
                double raioKm = raioMetros / 1000;

                DateTime data = monitoramentoProcessarEvento.DataVeiculoPosicao.Value;
                double latitude = monitoramentoProcessarEvento.LatitudePosicao.Value;
                double longitude = monitoramentoProcessarEvento.LongitudePosicao.Value;

                // Percorre em ordem inversa, das posições mais atuais para as mais antigas
                int ultima = (posicoesDoPeriodo?.Count ?? 0) - 1;
                for (int i = ultima; i >= 0; i--)
                {
                    noRaio = Servicos.Embarcador.Logistica.Distancia.ValidarNoRaio(latitude, longitude, posicoesDoPeriodo[i].Latitude, posicoesDoPeriodo[i].Longitude, raioKm);
                    if (noRaio)
                    {
                        DateTime? dataInicioPermanencia = PermaneceuNoRaio(data, latitude, longitude, posicoesDoPeriodo, tempoParado, raioMetros);
                        if (dataInicioPermanencia != null)
                        {
                            // Ficou parado pelo tempo mínimo
                            return null;
                        }
                    }

                    // Atingiu o limite de movimentação sem paradas?
                    TimeSpan diff = monitoramentoProcessarEvento.DataVeiculoPosicao.Value - data;
                    if (diff.TotalMinutes > tempoEmMovimento)
                    {
                        return diff;
                    }

                    // Avança para a próxima posição da lista
                    data = posicoesDoPeriodo[i].DataVeiculo;
                    latitude = posicoesDoPeriodo[i].Latitude;
                    longitude = posicoesDoPeriodo[i].Longitude;
                }
            }
            return null;
        }

        //seria ao contrario do evento Continuo sem parada, onde tem tempo minimo de direcao e maximo de descanso/paradas;
        protected dynamic TempoEmMovimentoContinuoComParadasMinimas(Dominio.ObjetosDeValor.Embarcador.Logistica.MonitoramentoProcessarEvento monitoramentoProcessarEvento, List<Dominio.ObjetosDeValor.Embarcador.Logistica.Posicao> posicoesObjetoValor, List<Dominio.ObjetosDeValor.Embarcador.Logistica.AlertaMonitor> alertas, int tempoEmMovimento, int tempoParado, int raioMetros)
        {
            if (monitoramentoProcessarEvento != null && monitoramentoProcessarEvento.CodigoMonitoramento.HasValue && monitoramentoProcessarEvento.DataVeiculoPosicao != null && monitoramentoProcessarEvento.LatitudePosicao != null && monitoramentoProcessarEvento.LongitudePosicao != null)
            {
                //POSICOES DO PERIODO APOS O ULTIMO ALERTA PARA O VEICULO
                List<Dominio.ObjetosDeValor.Embarcador.Logistica.Posicao> posicoesDoPeriodo = ExtrairPosicoesAposAlertaFechadoAtePosicao(monitoramentoProcessarEvento, posicoesObjetoValor, alertas);
                DateTime dataFimPeriodo = posicoesDoPeriodo.OrderBy(x => x.DataVeiculo).Select(x => x.DataVeiculo).LastOrDefault();
                DateTime dataInicioPeriodo = posicoesDoPeriodo.OrderBy(x => x.DataVeiculo).Select(x => x.DataVeiculo).FirstOrDefault();

                if (this.MonitoramentoEvento.VerificarStatusViagem == Dominio.ObjetosDeValor.Embarcador.Enumeradores.VerificarStatusViagem.EstarComStatusViagem)
                {
                    //validar data inicial se estava em viagem..
                    var monitoramentoEventoStatusViagem = (
                          from statusViagem in this.MonitoramentoEvento.StatusViagem
                          where statusViagem.MonitoramentoEvento.Ativo
                          select new { statusViagem.MonitoramentoStatusViagem }
                        ).ToList();

                    if (monitoramentoEventoStatusViagem != null && monitoramentoEventoStatusViagem.Any(x => x.MonitoramentoStatusViagem.TipoRegra == Dominio.ObjetosDeValor.Embarcador.Enumeradores.MonitoramentoStatusViagemTipoRegra.EmViagem))
                    {
                        if (!monitoramentoProcessarEvento.DataInicioViagem.HasValue)//se nao tem inicio viagem, sai fora.
                            return null;

                        if (monitoramentoProcessarEvento.DataInicioViagem.HasValue && dataInicioPeriodo < monitoramentoProcessarEvento.DataInicioViagem.Value)//se tem inicio viagem, mas a data inicial de comparacao é menor, sai fora (ainda nao estava em viagem).
                            return null;
                    }
                }


                //OBTER PARADAS NAS POSICOES DO PERIODO..
                List<Dominio.ObjetosDeValor.Embarcador.Logistica.Parada> listParadasPeriodo = ObterParadasEntrePosicoes(posicoesDoPeriodo, raioMetros);

                //AGORA VAMOS SOMAR O TEMPO DE PARADAS. 
                TimeSpan totalTempoParadas = new TimeSpan(0);
                if (listParadasPeriodo.Count > 0)
                    totalTempoParadas = new TimeSpan(listParadasPeriodo.Sum(r => r.Tempo.Ticks));

                //VAMOS OBTER O TEMPO TOTAL DO MOVIMENTO (TEMPO TOTAL - TEMPO PARADAS)
                TimeSpan tempoTotal = dataFimPeriodo - dataInicioPeriodo; //Obtendo o tempo total em movimento

                //OBTER O TEMPO EM MOVIMENTO (TEMPO TOTAL - TEMPO TOTAL DE PARADAS)
                TimeSpan TempoMovimento = tempoTotal - totalTempoParadas;

                //SE O TEMPO EM MOVIMENTO É MAIOR QUE O TEMPO CONFIGURADO E O TEMPO DE PARADAS É MENOR QUE O TEMPO CONFIGURADO
                if (TempoMovimento.TotalMinutes > tempoEmMovimento && totalTempoParadas.TotalMinutes <= tempoParado)
                {
                    //vamos enviar o tempo total para bater as horas com data inicial e data final. (39727)
                    var result = new { DataInicial = dataInicioPeriodo.ToString("dd/MM/yyyy HH:mm:ss"), DataFinal = dataFimPeriodo.ToString("dd/MM/yyyy HH:mm:ss"), Diferenca = tempoTotal };
                    return result;
                }
            }

            return null;
        }


        protected DateTime? PermaneceuNoRaio(Dominio.ObjetosDeValor.Embarcador.Logistica.MonitoramentoProcessarEvento monitoramentoProcessarEvento, List<Dominio.ObjetosDeValor.Embarcador.Logistica.Posicao> posicoesObjetoValor, List<Dominio.ObjetosDeValor.Embarcador.Logistica.AlertaMonitor> alertas, int tempoMinutos, int raioMetros)
        {
            if (monitoramentoProcessarEvento != null && monitoramentoProcessarEvento.DataVeiculoPosicao != null && monitoramentoProcessarEvento.LatitudePosicao != null && monitoramentoProcessarEvento.LongitudePosicao != null)
            {
                List<Dominio.ObjetosDeValor.Embarcador.Logistica.Posicao> posicoesDoPeriodo = ExtrairPosicoesAposAlertaFechadoAtePosicao(monitoramentoProcessarEvento, posicoesObjetoValor, alertas);
                return PermaneceuNoRaio(monitoramentoProcessarEvento.DataVeiculoPosicao, monitoramentoProcessarEvento.LatitudePosicao, monitoramentoProcessarEvento.LongitudePosicao, posicoesDoPeriodo, tempoMinutos, raioMetros);
            }
            return null;
        }

        protected DateTime? PermaneceuNoRaio(DateTime? data, double? latitude, double? longitude, List<Dominio.ObjetosDeValor.Embarcador.Logistica.Posicao> posicoesObjetoValor, int tempoMinutos, int raioMetros)
        {
            if (data.HasValue && latitude.HasValue && longitude.HasValue)
            {

                // Limite da permanência
                DateTime dataInicial = data.Value.AddMinutes(-tempoMinutos);
                double raioKm = (double)raioMetros / 1000;
                bool noRaio;

                // Percorre em ordem inversa, das posições mais atuais para as mais antigas
                int ultima = posicoesObjetoValor.Count - 1;
                for (int i = ultima; i >= 0; i--)
                {
                    if (posicoesObjetoValor[i].DataVeiculo <= data.Value)
                    {
                        noRaio = Servicos.Embarcador.Logistica.Distancia.ValidarNoRaio(latitude.Value, longitude.Value, posicoesObjetoValor[i].Latitude, posicoesObjetoValor[i].Longitude, raioKm);
                        if (noRaio)
                        {
                            // Está no raio e a data é menor que limite inicial?
                            if (posicoesObjetoValor[i].DataVeiculo <= dataInicial)
                            {
                                return posicoesObjetoValor[i].DataVeiculo;
                            }
                        }
                        else
                        {
                            break;
                        }
                    }
                }
            }
            return null;
        }


        protected static List<Dominio.ObjetosDeValor.Embarcador.Logistica.Parada> ObterParadasEntrePosicoes(List<Dominio.ObjetosDeValor.Embarcador.Logistica.Posicao> posicoesDoVeiculo, int raioTolerado)
        {
            List<Dominio.ObjetosDeValor.Embarcador.Logistica.Parada> paradas = new List<Dominio.ObjetosDeValor.Embarcador.Logistica.Parada>();
            int total = posicoesDoVeiculo?.Count ?? 0;
            if (total > 1)
            {
                double tempoEntrePosicoes, distanciaEntrePosicoes;
                Dominio.ObjetosDeValor.Embarcador.Logistica.Posicao posicaoBase, posicaoComparada;
                Dominio.ObjetosDeValor.Embarcador.Logistica.Parada parada = null;
                for (int i = 0; i < total; i++)
                {
                    // Posição base
                    posicaoBase = posicoesDoVeiculo[i];
                    for (int j = i + 1; j < total; j++)
                    {
                        // Posição posterior
                        posicaoComparada = posicoesDoVeiculo[j];
                        if (posicaoBase.DataVeiculo <= posicaoComparada.DataVeiculo)
                        {
                            // Verifica se o deslocamento foi pequeno o suficiente para interpretar como uma parada
                            distanciaEntrePosicoes = Servicos.Embarcador.Logistica.Polilinha.CalcularDistancia(posicaoBase.Latitude, posicaoBase.Longitude, posicaoComparada.Latitude, posicaoComparada.Longitude);
                            if (distanciaEntrePosicoes <= raioTolerado)
                            {
                                // Tempo entre as posições
                                tempoEntrePosicoes = (posicaoComparada.DataVeiculo - posicaoBase.DataVeiculo).TotalMinutes;

                                if (distanciaEntrePosicoes <= raioTolerado && tempoEntrePosicoes >= 5) //se tempo entre posicoes for maior que 5 minutos
                                {
                                    if (parada == null)
                                    {
                                        parada = new Dominio.ObjetosDeValor.Embarcador.Logistica.Parada
                                        {
                                            Placa = posicaoBase.Placa,
                                            DataInicio = TrimMinutes(posicaoBase.DataVeiculo),
                                            Latitude = posicaoBase.Latitude,
                                            Longitude = posicaoBase.Longitude,
                                            Descricao = posicaoBase.Descricao
                                        };
                                    }
                                    parada.DataFim = TrimMinutes(posicaoComparada.DataVeiculo);
                                    parada.Tempo = parada.DataFim - parada.DataInicio;
                                }
                                // Avança a posição base
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
            return paradas;
        }

        #endregion

        #region Métodos privados

        private static DateTime TrimMinutes(DateTime data)
        {
            return new DateTime(data.Year, data.Month, data.Day, data.Hour, data.Minute, 0);
        }


        #endregion

    }
}