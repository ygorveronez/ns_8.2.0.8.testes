using System.Linq;

namespace Servicos.Embarcador.Logistica
{
    public static class Distancia
    {
        public static Servicos.Embarcador.Logistica.Polilinha Polilinha { get; set; }
        public static bool ValidarNoRaio(Dominio.ObjetosDeValor.Embarcador.Logistica.WayPoint PontoOrigem, Dominio.ObjetosDeValor.Embarcador.Logistica.WayPoint PontoDestino, double raioKM)
        {
            var distancia = CalcularDistanciaKM(PontoOrigem.Latitude, PontoOrigem.Longitude, PontoDestino.Latitude, PontoDestino.Longitude);

            return distancia <= raioKM;
        }

        public static bool ValidarNoRaio(double latitudeOrigem, double longitudeOrigem, double latitudeDestino, double longitudeDestino, double raioKM)
        {
            Dominio.ObjetosDeValor.Embarcador.Logistica.WayPoint origem = new Dominio.ObjetosDeValor.Embarcador.Logistica.WayPoint(latitudeOrigem, longitudeOrigem);
            Dominio.ObjetosDeValor.Embarcador.Logistica.WayPoint destino = new Dominio.ObjetosDeValor.Embarcador.Logistica.WayPoint(latitudeDestino, longitudeDestino);
            return ValidarNoRaio(origem, destino, raioKM);
        }

        /// <summary>
        /// Valida se um ponto está localizando dentro de um poligono
        /// </summary>
        /// <param name="ponto">Ponto a ser analisado</param>
        /// <param name="poligono">Pontos do poligono.</param>
        /// <returns></returns>
        public static bool ValidarPoligono(Dominio.ObjetosDeValor.Embarcador.Logistica.WayPoint ponto, Dominio.ObjetosDeValor.Embarcador.Logistica.WayPoint[] poligono )
        {
            bool result = false;
            int j = poligono.Count() - 1;
            for (int i = 0; i < poligono.Count(); i++)
            {
                if (poligono[i].Latitude < ponto.Latitude && poligono[j].Latitude >= ponto.Latitude || poligono[j].Latitude < ponto.Latitude && poligono[i].Latitude >= ponto.Latitude)
                {
                    if (poligono[i].Longitude + (ponto.Latitude - poligono[i].Latitude) / (poligono[j].Latitude - poligono[i].Latitude) * (poligono[j].Longitude - poligono[i].Longitude) < ponto.Longitude)
                    {
                        result = !result;
                    }
                }
                j = i;
            }
            return result;
        }

        public static double CalcularDistanciaKM(double origemLat, double origemLng, double destinoLat, double destinoLng)
        {
            return Polilinha.CalcularDistancia(origemLat, origemLng, destinoLat, destinoLng) / 1000;

        }

        public static double CalcularDistanciaMetros(double origemLat, double origemLng, double destinoLat, double destinoLng)
        {
            return Polilinha.CalcularDistancia(origemLat, origemLng, destinoLat, destinoLng);

        }

    }
}
