using System;
using System.Collections.Generic;

namespace Servicos.Embarcador.Logistica.Monitoramento
{
    public class ControleDistancia
    {

        #region Propriedades privadas


        #endregion

        #region Métodos públicos

        public static Dominio.ObjetosDeValor.Embarcador.Logistica.Roteirizacao.RespostaRoteirizacao ObterRespostaRoteirizacao(IList<Dominio.ObjetosDeValor.Embarcador.Logistica.Posicao> posicoes, string servidorOSRM, bool pontosNaRota = true, bool agruparPosicoesPorDistancia = false)
        {
            if (posicoes == null || posicoes.Count == 0) return null;

            var listaPontos = ObterListaPontos(posicoes);
            try
            {
                Roteirizacao rota = new Roteirizacao(servidorOSRM);
                OpcoesRoteirizar opcoes = new OpcoesRoteirizar();
                if (pontosNaRota)
                {
                    opcoes.PontosNaRota = true;
                    opcoes.MathRaio = 25;
                    opcoes.AgruparPosicoesPorDistancia = agruparPosicoesPorDistancia;
                }
                rota.Add(listaPontos);
                Dominio.ObjetosDeValor.Embarcador.Logistica.Roteirizacao.RespostaRoteirizacao resposta = rota.Roteirizar(opcoes);
                if ((resposta.Status == "OK") && (resposta.Distancia > 0)) return resposta;
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);
            }

            return null;
        }

        public static Dominio.ObjetosDeValor.Embarcador.Logistica.Roteirizacao.RespostaRoteirizacao ObterRespostaRoteirizacao(List<Dominio.ObjetosDeValor.Embarcador.Logistica.WayPoint> wayPoints, string servidorOSRM, bool pontosNaRota = true, bool agruparPosicoesPorDistancia = false)
        {
            List<Dominio.ObjetosDeValor.Embarcador.Logistica.Posicao> posicoes = new List<Dominio.ObjetosDeValor.Embarcador.Logistica.Posicao>();
            int total = wayPoints?.Count ?? 0;
            for (int i = 0; i < total; i++)
            {
                posicoes.Add(new Dominio.ObjetosDeValor.Embarcador.Logistica.Posicao
                {
                    Latitude = wayPoints[i].Latitude,
                    Longitude = wayPoints[i].Longitude
                });
            }
            return ObterRespostaRoteirizacao(posicoes, servidorOSRM, pontosNaRota, agruparPosicoesPorDistancia);

        }

        public static Dominio.ObjetosDeValor.Embarcador.Logistica.Roteirizacao.RespostaRoteirizacao ObterRespostaRoteirizacao(Dominio.Entidades.Embarcador.Logistica.Monitoramento cargaMonitorada, string servidorOSRM, Repositorio.UnitOfWork unitOfWork)
        {

            if (cargaMonitorada?.Veiculo == null) return null;

            DateTime dataInicio = cargaMonitorada.DataInicio ?? cargaMonitorada.DataCriacao ?? DateTime.Now;
            DateTime dataFim = cargaMonitorada.DataFim ?? DateTime.Now;

            Repositorio.Embarcador.Logistica.Posicao repPosicao = new Repositorio.Embarcador.Logistica.Posicao(unitOfWork);
            IList<Dominio.ObjetosDeValor.Embarcador.Logistica.Posicao> posicoesVeiculo = repPosicao.BuscarWaypointsPorVeiculoDataInicialeFinal(cargaMonitorada.Veiculo.Codigo, dataInicio, dataFim);

            return ObterRespostaRoteirizacao(posicoesVeiculo, servidorOSRM);
        }

        #endregion

        #region Métodos privados

        private static List<Dominio.ObjetosDeValor.Embarcador.Logistica.Roteirizacao.WayPoint> ObterListaPontos(IList<Dominio.ObjetosDeValor.Embarcador.Logistica.Posicao> posicoesVeiculo)
        {
            var listaPontos = new List<Dominio.ObjetosDeValor.Embarcador.Logistica.Roteirizacao.WayPoint>();

            int MaxPontos = 300;
            int cont = posicoesVeiculo.Count;
            if (posicoesVeiculo.Count < 500)
            {
                for (int i = 0; i < cont; i++)
                {
                    listaPontos.Add(new Dominio.ObjetosDeValor.Embarcador.Logistica.Roteirizacao.WayPoint() { Lat = posicoesVeiculo[i].Latitude, Lng = posicoesVeiculo[i].Longitude });
                }
            }
            else
            {

                double LngAnterior;
                double LatAnterior;
                int distanciaMinima = 20;
                int total = posicoesVeiculo.Count;
                while (true)
                {
                    // Garante sempre o primeiro ponto da rota
                    listaPontos.Add(new Dominio.ObjetosDeValor.Embarcador.Logistica.Roteirizacao.WayPoint(posicoesVeiculo[0].Latitude, posicoesVeiculo[0].Longitude));
                    LatAnterior = posicoesVeiculo[0].Latitude;
                    LngAnterior = posicoesVeiculo[0].Longitude;

                    // A partir do segundo até o penúltimo
                    for (int i = 1; i < total - 1; i++)
                    {
                        // Adicionar posicoes a partir da distância mínima
                        var distancia = Distancia.CalcularDistanciaMetros(LatAnterior, LngAnterior, posicoesVeiculo[i].Latitude, posicoesVeiculo[i].Longitude);
                        if (distancia > distanciaMinima)
                        {
                            listaPontos.Add(new Dominio.ObjetosDeValor.Embarcador.Logistica.Roteirizacao.WayPoint(posicoesVeiculo[i].Latitude, posicoesVeiculo[i].Longitude));
                            LatAnterior = posicoesVeiculo[i].Latitude;
                            LngAnterior = posicoesVeiculo[i].Longitude;
                        }
                    }

                    // Garante sempre o último ponto da rota
                    listaPontos.Add(new Dominio.ObjetosDeValor.Embarcador.Logistica.Roteirizacao.WayPoint(posicoesVeiculo[total - 1].Latitude, posicoesVeiculo[total - 1].Longitude));

                    // Foi possível reduzir a quantidade de pontos para não ultrapassar o limite máximo
                    if (listaPontos.Count < MaxPontos) break;

                    // ... ainda está maior que o limite
                    // Aumenta a distância mínima entre os pontos para simplificar ainda mais
                    distanciaMinima += 50;
                    listaPontos.Clear();
                }

            }

            return listaPontos;
        }

        #endregion

    }

}
