using System.Collections.Generic;
using System.Linq;

namespace Servicos.Embarcador.Monitoramento
{
    public class PercentualViagem
    {
        #region Métodos públicos estáticos 

        public static decimal CalcularPercentualViagem(Dominio.Entidades.Embarcador.Logistica.Monitoramento monitoramento, Dominio.Entidades.Embarcador.Logistica.MonitoramentoStatusViagem statusViagem, List<Dominio.Entidades.Embarcador.Logistica.MonitoramentoHistoricoStatusViagem> historicosStatusViagem, List<Dominio.Entidades.Embarcador.Cargas.ControleEntrega.CargaEntrega> cargaEntregas, Dominio.Entidades.Embarcador.Configuracao.ConfiguracaoTMS configuracao)
        {
            decimal percentual;

            // Possui carga mas ainda não iniciou a viagem
            if (monitoramento.Carga != null)
            {
                if (!monitoramento.Carga.DataInicioViagem.HasValue) return 0;
                if (monitoramento.Carga.DataFimViagem.HasValue) return 100;
            }

            Dominio.ObjetosDeValor.Embarcador.Enumeradores.MonitoramentoStatusViagemTipoRegra tipoRegra = (statusViagem != null) ? statusViagem.TipoRegra : 0;
            switch (tipoRegra)
            {

                case Dominio.ObjetosDeValor.Embarcador.Enumeradores.MonitoramentoStatusViagemTipoRegra.Concluida:
                    percentual = 100;
                    break;

                case Dominio.ObjetosDeValor.Embarcador.Enumeradores.MonitoramentoStatusViagemTipoRegra.SemViagem:
                case Dominio.ObjetosDeValor.Embarcador.Enumeradores.MonitoramentoStatusViagemTipoRegra.DeslocamentoParaPlanta:
                    percentual = 0;
                    break;

                default:

                    if (
                        configuracao.MonitoramentoStatusViagemTipoRegraParaCalcularPercentualViagem == null ||
                        configuracao.MonitoramentoStatusViagemTipoRegraParaCalcularPercentualViagem == tipoRegra ||
                        VerificaSeMonitoramentoPassouPeloStatusViagem(configuracao.MonitoramentoStatusViagemTipoRegraParaCalcularPercentualViagem.Value, historicosStatusViagem)
                    ) {
                        switch (configuracao.TipoCalculoPercentualViagem)
                        {
                            case Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoCalculoPercentualViagem.EntregasRealizadas:
                                percentual = CalcularPercentualViagemPorEntregasRealizadas(cargaEntregas);
                                break;
                            case Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoCalculoPercentualViagem.ProximidadeEntrePosicaoVeiculoRotaPrevista:
                                percentual = CalcularPercentualViagemPorProximidadePosicaoRota(monitoramento);
                                break;
                            case Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoCalculoPercentualViagem.DistanciaRotaPrevistaVersusDistanciaRotaRealizada:
                                percentual = CalcularPercentualViagemPorDistanciaPrevistaVersusDistanciaRealizada(monitoramento);
                                break;
                            case Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoCalculoPercentualViagem.DistanciaRotaRestanteAteDestino:
                                percentual = CalcularPercentualViagemPorRotaRestanteAteDestino(monitoramento);
                                break;
                            default:
                                percentual = 0;
                                break;
                        }
                    }
                    else
                    {
                        percentual = 0;
                    }
                    break;
            }
            return percentual;
        }

        #endregion

        #region Métodos privados

        private static decimal CalcularPercentual(decimal valorAtingido, decimal valorTotal)
        {
            decimal percentual = (valorTotal > 0) ? (valorAtingido * 100) / valorTotal : 0;
            if (percentual > 100) percentual = 100;
            else if (percentual < 0) percentual = 0;
            return percentual;
        }

        private static decimal CalcularPercentualViagemPorEntregasRealizadas(List<Dominio.Entidades.Embarcador.Cargas.ControleEntrega.CargaEntrega> cargaEntregas)
        {
            decimal percentual = 0;

            int total = cargaEntregas?.Count ?? 0;
            if (total > 0)
            {
                int totalEntregasIniciadasOuFinalizadas = 0;
                for (int i = 0; i < total; i++)
                {
                    if (cargaEntregas[i].DataInicio != null || cargaEntregas[i].DataConfirmacao != null)
                    {
                        totalEntregasIniciadasOuFinalizadas++;
                    }
                }
                percentual = CalcularPercentual(totalEntregasIniciadasOuFinalizadas, total);
            }
            return percentual;
        }

        private static decimal CalcularPercentualViagemPorProximidadePosicaoRota(Dominio.Entidades.Embarcador.Logistica.Monitoramento monitoramento)
        {
            decimal distanciaPrevista = monitoramento.DistanciaPrevista ?? 0;
            decimal percentual = 0;

            // Deve possuir as informações básicas para o cálculo do percentual
            if (!string.IsNullOrWhiteSpace(monitoramento.PolilinhaPrevista) && !string.IsNullOrWhiteSpace(monitoramento.PolilinhaRealizada) && distanciaPrevista > 0)
            {
                List<Dominio.ObjetosDeValor.Embarcador.Logistica.WayPoint> wayPointsPrevistos = Servicos.Embarcador.Logistica.Polilinha.Decodificar(monitoramento.PolilinhaPrevista);
                List<Dominio.ObjetosDeValor.Embarcador.Logistica.WayPoint> wayPointsRealizados = Servicos.Embarcador.Logistica.Polilinha.Decodificar(monitoramento.PolilinhaRealizada);
                if (wayPointsPrevistos.Count > 0 && wayPointsRealizados.Count > 0)
                {
                    decimal distanciaAproximadaRealizada = (decimal)Servicos.Embarcador.Logistica.Polilinha.CalculaDistanciaRealizadaAproximada(wayPointsPrevistos, wayPointsRealizados.Last());
                    distanciaAproximadaRealizada /= 1000; // m para km
                    if (distanciaAproximadaRealizada <= distanciaPrevista)
                    {
                        percentual = CalcularPercentual(distanciaAproximadaRealizada, distanciaPrevista);
                    }
                    else
                    {
                        percentual = 99;
                    }
                }
            }
            return percentual;
        }

        private static decimal CalcularPercentualViagemPorDistanciaPrevistaVersusDistanciaRealizada(Dominio.Entidades.Embarcador.Logistica.Monitoramento monitoramento)
        {
            return CalcularPercentual(monitoramento.DistanciaRealizada, monitoramento.DistanciaPrevista ?? 0);
        }

        private static decimal CalcularPercentualViagemPorRotaRestanteAteDestino(Dominio.Entidades.Embarcador.Logistica.Monitoramento monitoramento)
        {
            decimal distanciaAteDestino = monitoramento.DistanciaAteDestino ?? 0;

            decimal distanciaTotalConsiderada;
            if (distanciaAteDestino > 0)
            {
                // Considera a distância total a ser realizada como a soma da distância realizada e a distância até o final
                distanciaTotalConsiderada = monitoramento.DistanciaRealizada + distanciaAteDestino;
            }
            else
            {
                distanciaTotalConsiderada = monitoramento.DistanciaPrevista ?? 0;
            }
            return CalcularPercentual(monitoramento.DistanciaRealizada, distanciaTotalConsiderada);
        }

        private static bool VerificaSeMonitoramentoPassouPeloStatusViagem(Dominio.ObjetosDeValor.Embarcador.Enumeradores.MonitoramentoStatusViagemTipoRegra tipoRegra, List<Dominio.Entidades.Embarcador.Logistica.MonitoramentoHistoricoStatusViagem> historicosStatusViagem) 
        {
            int total = historicosStatusViagem?.Count() ?? 0;
            for (int i = 0; i < total; i++)
            {
                if (historicosStatusViagem[i].StatusViagem.TipoRegra == tipoRegra)
                {
                    return true;
                }
            }
            return false;
        }

        #endregion

    }

}
