using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SGT.GerenciadorApp.Monitoramento
{
    public static class MonitoramentoUtils
    {
        public static void AtualizarUltimoAlerta(List<Dominio.ObjetosDeValor.Embarcador.Logistica.UltimoAlerta> ultimoAlerta, Dominio.Entidades.Embarcador.Logistica.AlertaMonitor novoAlerta)
        {

            var novoUltimoAlerta = new Dominio.ObjetosDeValor.Embarcador.Logistica.UltimoAlerta
            {
                Codigo = novoAlerta.Codigo,
                DataCadastro = novoAlerta.DataCadastro,
                Data = novoAlerta.Data,
                Veiculo = novoAlerta.Veiculo.Codigo,
                TipoAlerta = novoAlerta.TipoAlerta,
                Latitude = novoAlerta.Posicao.Latitude,
                Longitude = novoAlerta.Posicao.Longitude
            };

            if (ultimoAlerta.Count == 0)
            {
                ultimoAlerta.Add(novoUltimoAlerta);
                return;
            }


            var alerta = ultimoAlerta.Where(i => i.Veiculo == novoAlerta.Veiculo.Codigo).FirstOrDefault();

            var index = ultimoAlerta.IndexOf(alerta);

            if (index != -1)
                ultimoAlerta[index] = novoUltimoAlerta;
            else
                ultimoAlerta.Add(novoUltimoAlerta);
        }
        public static void GravarLogTracking(string mensagem, string prefixo)
        {
            try
            {
                DateTime dateTime = DateTime.Now;
                string arquivo = (string.IsNullOrWhiteSpace(prefixo) ? "" : prefixo + "-") + dateTime.Day + "-" + dateTime.Month + "-" + dateTime.Year + ".txt";
                string path = System.IO.Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "LogTracking");
                string file = System.IO.Path.Combine(path, arquivo);

                if (!System.IO.Directory.Exists(path))
                    System.IO.Directory.CreateDirectory(path);

                System.IO.StreamWriter strw = new System.IO.StreamWriter(file, true);
                try
                {
                    strw.WriteLine(DateTime.Now.ToLongTimeString() + " - " + mensagem);
                }
                catch
                {
                }
                finally
                {
                    strw.Close();
                }
            }
            catch
            {
            }

        }
        public static bool InRange(decimal inicial, decimal final, decimal valor)
        {
            return valor >= inicial && valor <= final;
        }
        public static bool VerificarLocais(List<Dominio.Entidades.Embarcador.Logistica.Locais> listaLocais, Dominio.Entidades.Embarcador.Logistica.Posicao posicao, Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoLocal tipoLocal)
        {
            double latitude = posicao.Latitude;
            double longitude = posicao.Longitude;

            List<Dominio.Entidades.Embarcador.Logistica.Locais> locais = listaLocais.Where(o => o.Tipo == tipoLocal).ToList();

            foreach (var local in listaLocais)
            {
                if (string.IsNullOrWhiteSpace(local.Area))
                    continue;

                var areas = Newtonsoft.Json.JsonConvert.DeserializeObject<List<Dominio.ObjetosDeValor.Embarcador.Logistica.LocalArea>>(local.Area);


                foreach (var area in areas)
                {
                    var pontoOrigem = new Dominio.ObjetosDeValor.Embarcador.Logistica.WayPoint { Latitude = latitude, Longitude = longitude };

                    var emArea = false;
                    switch (area.type)
                    {
                        case "circle":
                            var pontoDestino = new Dominio.ObjetosDeValor.Embarcador.Logistica.WayPoint { Latitude = area.center.lat, Longitude = area.center.lng };
                            emArea = Servicos.Embarcador.Logistica.Distancia.ValidarNoRaio(pontoOrigem, pontoDestino, area.radius / 1000);
                            break;
                        case "rectangle":
                            var listaPontosRetangulo = new List<Dominio.ObjetosDeValor.Embarcador.Logistica.WayPoint>();

                            Dominio.ObjetosDeValor.Embarcador.Logistica.WayPoint pontoNorthEast = new Dominio.ObjetosDeValor.Embarcador.Logistica.WayPoint { Latitude = area.bounds.north, Longitude = area.bounds.east };
                            Dominio.ObjetosDeValor.Embarcador.Logistica.WayPoint pontoNorthWest = new Dominio.ObjetosDeValor.Embarcador.Logistica.WayPoint { Latitude = area.bounds.north, Longitude = area.bounds.west };
                            Dominio.ObjetosDeValor.Embarcador.Logistica.WayPoint pontoSouthWest = new Dominio.ObjetosDeValor.Embarcador.Logistica.WayPoint { Latitude = area.bounds.south, Longitude = area.bounds.west };
                            Dominio.ObjetosDeValor.Embarcador.Logistica.WayPoint pontoSouthEast= new Dominio.ObjetosDeValor.Embarcador.Logistica.WayPoint { Latitude = area.bounds.south, Longitude = area.bounds.east };

                            listaPontosRetangulo.Add(pontoNorthEast);
                            listaPontosRetangulo.Add(pontoNorthWest);
                            listaPontosRetangulo.Add(pontoSouthWest);
                            listaPontosRetangulo.Add(pontoSouthEast);

                            emArea = Servicos.Embarcador.Logistica.Distancia.ValidarPoligono(pontoOrigem, listaPontosRetangulo.ToArray());
                            break;
                        case "polygon":

                            var listaPontos = new List<Dominio.ObjetosDeValor.Embarcador.Logistica.WayPoint>();
                            foreach (var ponto in area.paths)
                            {
                                listaPontos.Add(new Dominio.ObjetosDeValor.Embarcador.Logistica.WayPoint { Latitude = ponto.lat, Longitude = ponto.lng });
                            }

                            emArea = Servicos.Embarcador.Logistica.Distancia.ValidarPoligono(pontoOrigem, listaPontos.ToArray());
                            break;
                    }

                    if (emArea)
                        return true;
                }

            }
            return false;
        }
    }
}