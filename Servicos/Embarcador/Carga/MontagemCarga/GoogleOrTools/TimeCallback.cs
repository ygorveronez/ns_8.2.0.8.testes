using System;
using System.Collections.Generic;

namespace Servicos.Embarcador.Carga.MontagemCarga.GoogleOrTools
{
    public class TimeCallback
    {
        public TimeCallback(long[][] tempo_atendimento, Position[] locations)
        {
            this.TempoAtendimento = tempo_atendimento;
            this.locations = locations;
            Servicos.Embarcador.Logistica.Roteirizacao rota = new Servicos.Embarcador.Logistica.Roteirizacao("");
            int i = 0;
            int j = 0;

            //Obtendo os pontos dos pedidos para gerar o table de distancias.
            List<Dominio.ObjetosDeValor.Embarcador.Logistica.Roteirizacao.WayPoint> points = new List<Dominio.ObjetosDeValor.Embarcador.Logistica.Roteirizacao.WayPoint>();
            for (i = 0; i < locations.Length; i++)
                points.Add(new Dominio.ObjetosDeValor.Embarcador.Logistica.Roteirizacao.WayPoint()
                {
                    Codigo = locations[i].id,
                    Lat = locations[i].Y,
                    Lng = locations[i].X
                });

            //Obtendo o table de distancias.
            Dominio.ObjetosDeValor.Embarcador.Logistica.Roteirizacao.ResponseRouter table = null;
            try
            {
                table = rota.OsrmTable(points);
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro($"[Arquitetura-CatchNoAction] Erro ao obter tabela OSRM para cálculo de tempo: {ex.ToString()}", "CatchNoAction");
            }

            Tempo = new long[locations.Length][];
            for (i = 0; i < locations.Length; i++)
            {
                Tempo[i] = new long[locations.Length];
                for (j = 0; j < locations.Length; j++)
                {
                    if (i == j)
                        Tempo[i][j] = 0;
                    else
                    {
                        double tmp = 0;
                        if (table != null)
                        {
                            try
                            {
                                tmp = table.Durations[i][j];
                            }
                            catch (Exception ex)
                            {
                                Servicos.Log.TratarErro($"[Arquitetura-CatchNoAction] Erro ao acessar duração na tabela OSRM: {ex.ToString()}", "CatchNoAction");
                            }
                        }

                        if (tmp != 0)
                            Tempo[i][j] = (long)tmp;
                        else
                        {
                            var metros = (long)Servicos.Embarcador.Logistica.Polilinha.CalcularDistancia(locations[i].Y, locations[i].X, locations[j].Y, locations[j].X);
                            Tempo[i][j] = (long)((metros / 1000) * 40) / 60;
                        }
                    }
                }
            }
        }

        public long[][] TempoAtendimento;
        private Position[] locations;
        public long[][] Tempo;

    }
}
