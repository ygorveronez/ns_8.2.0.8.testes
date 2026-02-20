using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Servicos.Embarcador.Logistica
{

    public class DistanciasPercorridas
    {
        public double Aproximando = 0;
        public double Afastando = 0;
        public double Total = 0;
    }

    public class Polilinha
    {

        #region Métodos públicos

        // https://gist.github.com/shinyzhu/4617989
        // https://developers.google.com/maps/documentation/utilities/polylinealgorithm
        public static string Codificar(List<Dominio.ObjetosDeValor.Embarcador.Logistica.WayPoint> points)
        {
            var str = new StringBuilder();
            int total = points?.Count ?? 0;
            if (total > 0)
            {
                var encodeDiff = (Action<int>)(diff =>
                {
                    int shifted = diff << 1;
                    if (diff < 0)
                        shifted = ~shifted;
                    int rem = shifted;
                    while (rem >= 0x20)
                    {
                        str.Append((char)((0x20 | (rem & 0x1f)) + 63));
                        rem >>= 5;
                    }
                    str.Append((char)(rem + 63));
                });

                int lastLat = 0;
                int lastLng = 0;
                for (int i = 0; i < total; i++)
                {
                    int lat = (int)Math.Round(points[i].Latitude * 1E5);
                    int lng = (int)Math.Round(points[i].Longitude * 1E5);
                    encodeDiff(lat - lastLat);
                    encodeDiff(lng - lastLng);
                    lastLat = lat;
                    lastLng = lng;
                }
            }
            return str.ToString();
        }

        // https://gist.github.com/shinyzhu/4617989
        // https://developers.google.com/maps/documentation/utilities/polylinealgorithm
        public static List<Dominio.ObjetosDeValor.Embarcador.Logistica.WayPoint> Decodificar(string encodedPoints)
        {
            if (encodedPoints == null || encodedPoints == "") return null;
            List<Dominio.ObjetosDeValor.Embarcador.Logistica.WayPoint> poly = new List<Dominio.ObjetosDeValor.Embarcador.Logistica.WayPoint>();
            char[] polylinechars = encodedPoints.ToCharArray();
            int index = 0;

            int currentLat = 0;
            int currentLng = 0;
            int next5bits;
            int sum;
            int shifter;

            try
            {
                while (index < polylinechars.Length)
                {
                    sum = 0;
                    shifter = 0;
                    do
                    {
                        next5bits = (int)polylinechars[index++] - 63;
                        sum |= (next5bits & 31) << shifter;
                        shifter += 5;
                    } while (next5bits >= 32 && index < polylinechars.Length);

                    if (index >= polylinechars.Length)
                        break;

                    currentLat += (sum & 1) == 1 ? ~(sum >> 1) : (sum >> 1);

                    sum = 0;
                    shifter = 0;
                    do
                    {
                        next5bits = (int)polylinechars[index++] - 63;
                        sum |= (next5bits & 31) << shifter;
                        shifter += 5;
                    } while (next5bits >= 32 && index < polylinechars.Length);

                    if (index >= polylinechars.Length && next5bits >= 32)
                        break;

                    currentLng += (sum & 1) == 1 ? ~(sum >> 1) : (sum >> 1);
                    Dominio.ObjetosDeValor.Embarcador.Logistica.WayPoint p = new Dominio.ObjetosDeValor.Embarcador.Logistica.WayPoint();
                    p.Latitude = Convert.ToDouble(currentLat) / 1E5;
                    p.Longitude = Convert.ToDouble(currentLng) / 1E5;
                    poly.Add(p);
                }
            }
            catch
            {
            }
            return poly;
        }

        public static List<Dominio.ObjetosDeValor.Embarcador.Logistica.WayPoint> DecodificarSimplificada(string encodedPoints)
        {
            List<Dominio.ObjetosDeValor.Embarcador.Logistica.WayPoint> poly = Decodificar(encodedPoints);
            if (poly == null) return null;

            var points = (from obj in poly select new System.Drawing.PointF((float)obj.Longitude, (float)obj.Latitude)).ToList();

            double tolerance = 0.1;
            bool highQualityEnabled = false;
            var simplifiedPoints = Logistica.PolilinhaSimplify.SimplificationHelpers.Simplify<System.Drawing.PointF>(
                            points,
                            (p1, p2) => p1 == p2,
                            (p) => p.X,
                            (p) => p.Y,
                            tolerance,
                            highQualityEnabled
                            );

            return (from obj in simplifiedPoints select new Dominio.ObjetosDeValor.Embarcador.Logistica.WayPoint(obj.Y.ToString("F6").Replace(",", "."), obj.X.ToString("F6").Replace(",", "."))).ToList();
        }

        public static List<Dominio.ObjetosDeValor.Embarcador.Logistica.WayPoint> ObterPontosPolilinha(string polilinha, double distanciaminima)
        {
            var listapontosretorno = new List<Dominio.ObjetosDeValor.Embarcador.Logistica.WayPoint>();
            if (polilinha == "")
                return listapontosretorno;

            var listapontos = Decodificar(polilinha);

            var pontoanterior = new Dominio.ObjetosDeValor.Embarcador.Logistica.WayPoint();

            //adicionar primeiro
            listapontosretorno.Add(listapontos[0]);

            pontoanterior = listapontos[0];

            for (var i = 1; i < listapontos.Count() - 2; i++)
            {
                var ponto = listapontos[i];

                if (distanciaminima > 0)
                {
                    double distancia = CalcularDistancia(pontoanterior.Latitude, pontoanterior.Longitude, ponto.Latitude, ponto.Longitude);

                    if (distancia > distanciaminima)
                    {
                        listapontosretorno.Add(ponto);
                        pontoanterior = ponto;
                    }

                }
                else
                {
                    listapontosretorno.Add(ponto);
                }
            }

            listapontosretorno.Add(listapontos[listapontos.Count - 1]);

            return listapontosretorno;
        }

        public static double CalcularDistancia(double lat1, double lon1, double lat2, double lon2)
        {
            if (lat1 == lat2 && lon1 == lon2)
            {
                return 0;
            }
            else
            {
                double theta = lon1 - lon2;
                double dist = Math.Sin(Deg2rad(lat1)) * Math.Sin(Deg2rad(lat2)) + Math.Cos(Deg2rad(lat1)) * Math.Cos(Deg2rad(lat2)) * Math.Cos(Deg2rad(theta));
                dist = Math.Acos(dist);
                dist = Rad2deg(dist);
                dist = dist * 60 * 1.1515;
                dist = dist * 1.609344 * 1000;//Metros
                return dist;
            }
        }

        public static double CalcularDistancia(Dominio.ObjetosDeValor.Embarcador.Logistica.WayPoint wayPoint1, Dominio.ObjetosDeValor.Embarcador.Logistica.WayPoint wayPoint2)
        {
            return CalcularDistancia(wayPoint1.Latitude, wayPoint1.Longitude, wayPoint2.Latitude, wayPoint2.Longitude);
        }

        /**
         * Calcula uma distância percorrida em uma rota até um ponto de interesse que deve estar na rota.
         */
        public static double CalcularDistancia(List<Dominio.ObjetosDeValor.Embarcador.Logistica.WayPoint> wayPointsRotaPrevista, Dominio.ObjetosDeValor.Embarcador.Logistica.WayPoint wayPoint)
        {

            Dominio.ObjetosDeValor.Embarcador.Logistica.WayPoint wayPointPrevistoAnterior = null;
            double distanciaRealizada = 0;

            // Percorre os pontos realizados
            int total = wayPointsRotaPrevista.Count;
            for (int i = 0; i < total; i++)
            {

                // Sumariza a distância prevista
                if (wayPointPrevistoAnterior != null) distanciaRealizada += Polilinha.CalcularDistancia(wayPointsRotaPrevista[i], wayPointPrevistoAnterior);

                if (wayPointsRotaPrevista[i].Equals(wayPoint)) return distanciaRealizada;

                wayPointPrevistoAnterior = wayPointsRotaPrevista[i];

            }

            return distanciaRealizada;

        }

        /**
         * Calcula uma distância percorrida em uma rota até um ponto de interesse que deve estar na rota.
         */
        public static double CalcularDistancia(List<Dominio.ObjetosDeValor.Embarcador.Logistica.WayPoint> wayPointsRotaPrevista, Dominio.ObjetosDeValor.Embarcador.Logistica.WayPoint wayPointOrigem, Dominio.ObjetosDeValor.Embarcador.Logistica.WayPoint wayPointDestino)
        {

            Dominio.ObjetosDeValor.Embarcador.Logistica.WayPoint wayPointPrevistoAnterior = null;
            double distanciaRealizada = 0;

            // Percorre os pontos realizados
            int total = wayPointsRotaPrevista.Count;
            for (int i = 0; i < total; i++)
            {

                // Identifica o ponto de origem na rota
                if (wayPointPrevistoAnterior == null)
                {
                    if (wayPointsRotaPrevista[i].Equals(wayPointOrigem))
                    {
                        wayPointPrevistoAnterior = wayPointsRotaPrevista[i];
                    }
                }
                else
                {
                    distanciaRealizada += Polilinha.CalcularDistancia(wayPointsRotaPrevista[i], wayPointPrevistoAnterior);

                    // Chegou ao ponto de destino?
                    if (wayPointsRotaPrevista[i].Equals(wayPointDestino))
                    {
                        return distanciaRealizada;
                    }
                    wayPointPrevistoAnterior = wayPointsRotaPrevista[i];
                }
            }
            return distanciaRealizada;
        }

        /**
         * Calcula uma distância percorrida em uma rota
         */
        public static double CalcularDistancia(List<Dominio.ObjetosDeValor.Embarcador.Logistica.WayPoint> wayPointsRota)
        {
            Dominio.ObjetosDeValor.Embarcador.Logistica.WayPoint wayPointPrevistoAnterior = null;
            double distanciaRealizada = 0;

            // Percorre os pontos realizados
            int total = wayPointsRota.Count;
            for (int i = 0; i < total; i++)
            {
                if (wayPointPrevistoAnterior != null)
                {
                    distanciaRealizada += Polilinha.CalcularDistancia(wayPointPrevistoAnterior, wayPointsRota[i]);
                }
                wayPointPrevistoAnterior = wayPointsRota[i];
            }
            return distanciaRealizada;

        }

        /**
         * Calcula uma distância percorrida aproximada considerando uma rota prevista e um ponto de interesse.
         * Localiza o ponto da rota mais próximo do ponto de interesse e considera que, se estivesse na rota, estaria neste ponto.
         * Retorna a distância prevista até o ponto
         */
        public static double CalculaDistanciaRealizadaAproximada(List<Dominio.ObjetosDeValor.Embarcador.Logistica.WayPoint> wayPointsRotaPrevista, Dominio.ObjetosDeValor.Embarcador.Logistica.WayPoint wayPoint)
        {

            // Localiza um ponto na rota mais próximo do ponto de interesse
            Dominio.ObjetosDeValor.Embarcador.Logistica.WayPoint pontoDaRotaMaisProximo = LocalizaPontoMaisProximoDaRota(wayPointsRotaPrevista, wayPoint);

            double distancia = 0;
            if (pontoDaRotaMaisProximo != null)
            {
                // Distância percorrida NA ROTA até o ponto encontrado
                distancia = CalcularDistancia(wayPointsRotaPrevista, pontoDaRotaMaisProximo);

                // Desconta a distância absoluta entre o PONTO DE INTERESSE e o ponto da ROTA DA ROTA
                if (!pontoDaRotaMaisProximo.Equals(wayPoint))
                    distancia -= CalcularDistancia(wayPoint, pontoDaRotaMaisProximo);

            }

            return distancia;

        }

        public static double CalculaDistanciaRealizadaAproximada(string polilinha, Dominio.ObjetosDeValor.Embarcador.Logistica.WayPoint wayPoint)
        {
            return CalculaDistanciaRealizadaAproximada(Decodificar(polilinha), wayPoint);
        }

        /**
         * Busca um ponto da rota prevista que está mais próximo do ponto de interesse
         */
        public static Dominio.ObjetosDeValor.Embarcador.Logistica.WayPoint LocalizaPontoMaisProximoDaRota(List<Dominio.ObjetosDeValor.Embarcador.Logistica.WayPoint> wayPointsRota, Dominio.ObjetosDeValor.Embarcador.Logistica.WayPoint wayPoint, int toleranciaEmMetros = 1000000000)
        {
            int indice = LocalizaPontoMaisProximoDaRotaIndice(wayPointsRota.ToArray(), wayPoint, toleranciaEmMetros);
            return (indice > -1) ? wayPointsRota[indice] : null;
        }

        /**
         * Busca o índice do ponto da rota prevista que está mais próximo do ponto de interesse
         */
        public static int LocalizaPontoMaisProximoDaRotaIndice(Dominio.ObjetosDeValor.Embarcador.Logistica.WayPoint[] wayPointsRota, Dominio.ObjetosDeValor.Embarcador.Logistica.WayPoint wayPoint, int toleranciaEmMetros = 1000000000, int indiceInicial = 0)
        {
            int indice = -1;
            double menorDistancia = toleranciaEmMetros;
            int total = wayPointsRota?.Length ?? 0;
            for (int i = indiceInicial; i < total; i++)
            {
                double distancia = Polilinha.CalcularDistancia(wayPoint, wayPointsRota[i]);
                if (distancia < menorDistancia)
                {
                    menorDistancia = distancia;
                    indice = i;
                }
            }
            return indice;
        }

        /**
         * Calcula uma taxa de similaridade entre uma rota prevista e uma rota realizada. 
         * As rotas são representadas pela lista de pontos geográficos. Considera uma tolerância máxima de distância em metros.
         * O retorno "0" indica nenhuma similaridade, variando até 1, quando indica total similaridade, ou seja, a rota realizada pertence à rota prevista
         */
        public static double CalculaTaxaDeSimilaridadeEntreRotas(List<Dominio.ObjetosDeValor.Embarcador.Logistica.WayPoint> wayPointsRotaPrevista, List<Dominio.ObjetosDeValor.Embarcador.Logistica.WayPoint> wayPointsRotaRealizada, int toleranciaEmMetros)
        {

            int totalPontosNaRota = 0;

            // Arredondamento das coordenadas para simplificar o cálculo da distância
            int units = CasasDecimaisParaAcuraciaMinima(toleranciaEmMetros);

            // Percorre os pontos realizados
            int totalRealizada = wayPointsRotaRealizada.Count;
            int totalPrevista = wayPointsRotaPrevista.Count;
            for (int i = 0; i < totalRealizada; i++)
            {
                double menorDistancia = toleranciaEmMetros + 1;
                double latitudeRealizada = Math.Round(wayPointsRotaRealizada[i].Latitude, units);
                double longitudeRealizada = Math.Round(wayPointsRotaRealizada[i].Latitude, units);

                // Interpola com os pontos previstos
                for (int j = 0; j < totalPrevista; j++)
                {
                    double latitudePrevista = Math.Round(wayPointsRotaPrevista[j].Latitude, units);
                    double longitudePrevista = Math.Round(wayPointsRotaPrevista[j].Latitude, units);
                    if (latitudeRealizada == latitudePrevista && longitudeRealizada == longitudePrevista)
                    {
                        double distanciaAtual = CalcularDistancia(wayPointsRotaPrevista[j].Latitude, wayPointsRotaPrevista[j].Longitude, wayPointsRotaRealizada[i].Latitude, wayPointsRotaRealizada[i].Longitude);
                        if (distanciaAtual < menorDistancia) menorDistancia = distanciaAtual;

                    }
                }

                // O ponto previsto está próximo da rota?
                if (menorDistancia < toleranciaEmMetros) totalPontosNaRota++;

            }

            // Cálculo da taxa
            double taxa = (wayPointsRotaRealizada.Count > 0) ? totalPontosNaRota / wayPointsRotaRealizada.Count : 0;
            return taxa;

        }

        /**
         * Calcula a distância, em linha reta, de um ponto e um rota.
         * Considera todo o trajeto da rota, não apenas os pontos que formam a rota.
         * 
         *                     Rota
         * P0-----P1-------------+---------------------PN
         *                       |                    
         *                       | Distância entre o ponto e a rota
         *                       | 
         *                       X Ponto
         * 
         */
        public static double CalcularDistanciaEntrePontoERota(Dominio.ObjetosDeValor.Embarcador.Logistica.WayPoint wayPoint, List<Dominio.ObjetosDeValor.Embarcador.Logistica.WayPoint> wayPointsRota)
        {

            // Há um ponto exatamente igual na rota
            int indexWayPoint = BuscarIndiceDoPontoNaRota(wayPoint, wayPointsRota);
            if (indexWayPoint != -1)
            {
                return 0;
            }

            // Localiza o ponto na rota mais próximo do ponto de interesse
            Dominio.ObjetosDeValor.Embarcador.Logistica.WayPoint pontoDaRotaMaisProximo = LocalizaPontoMaisProximoDaRota(wayPointsRota, wayPoint);

            // Distância entre o PONTO DE INTERESSE e o ponto da ROTA DA ROTA
            double distanciaPontoMaisProximo = CalcularDistancia(wayPoint, pontoDaRotaMaisProximo);

            // Localiza os pontos vizinhos imediatos (anterior e próximo) do ponto mais próximo na rota
            List<Dominio.ObjetosDeValor.Embarcador.Logistica.WayPoint> pontosVizinhosImediatos = ObterPontosVizinhosImediatos(pontoDaRotaMaisProximo, wayPointsRota);

            // Deve existir pelo menos um vizinho e no máximo dois vizinhos, um anterior e um próximo
            if (pontosVizinhosImediatos.Count == 0)
            {
                return distanciaPontoMaisProximo;
            }
            else
            {

                // O ponto que será usado para completar o triângulo
                Dominio.ObjetosDeValor.Embarcador.Logistica.WayPoint wayPointC;
                double distanciaPontoC;

                // Existe apenas um vizinho?
                if (pontosVizinhosImediatos.Count == 1)
                {
                    wayPointC = pontosVizinhosImediatos[0];
                    distanciaPontoC = CalcularDistancia(wayPoint, pontosVizinhosImediatos[0]);
                }
                else
                {

                    // Verifica se o ponto está antes ou depois do ponto e interesse
                    double distanciaComAnterior = CalcularDistancia(wayPoint, pontosVizinhosImediatos[0]);
                    double distanciaComProximo = CalcularDistancia(wayPoint, pontosVizinhosImediatos[1]);
                    double distanciaPontoMaisProximoComAnterior = CalcularDistancia(pontoDaRotaMaisProximo, pontosVizinhosImediatos[0]);
                    double distanciaPontoMaisProximoComProximo = CalcularDistancia(pontoDaRotaMaisProximo, pontosVizinhosImediatos[1]);

                    // Na hipótese que está antes do ponto mais próximo, verifica a distância com o vizinho anterior
                    if (distanciaPontoMaisProximoComAnterior > Math.Max(distanciaPontoMaisProximo, distanciaComAnterior))
                    {
                        wayPointC = pontosVizinhosImediatos[0];
                        distanciaPontoC = distanciaComAnterior;
                    }

                    // Na hipótese que está depois do ponto mais próximo, verifica a distância com o vizinho posterior
                    //if (distanciaPontoMaisProximoComProximo > Math.Max(distanciaPontoMaisProximo, distanciaComProximo))
                    else
                    {
                        wayPointC = pontosVizinhosImediatos[1];
                        distanciaPontoC = distanciaComProximo;
                    }

                }

                // Distâncias entre os dois pontos da rota
                double distanciaPontoMaisProximoPontoC = CalcularDistancia(pontoDaRotaMaisProximo, wayPointC);

                // Garante que trata-se de um triângulo isósceles. Se não for, não está na rota
                //if (distanciaPontoMaisProximo < distanciaPontoMaisProximoPontoC && distanciaPontoC < distanciaPontoMaisProximoPontoC)

                // Encontrar as distâncias pela Fórmula de Heron 
                double hipotenusa = distanciaPontoMaisProximoPontoC;
                double cateto1 = distanciaPontoMaisProximo;
                double cateto2 = distanciaPontoC;

                // Semiperímetro
                double P = (hipotenusa + cateto1 + cateto2) / 2;

                // Cálculo da área
                double A = Math.Sqrt(P * (P - hipotenusa) * (P - cateto1) * (P - cateto2));

                // Encontra a altura pela fórmula da área A = (b * h) / 2
                double h = (2 * A) / hipotenusa;

                // A altura do triângulo é a distância do ponto de interesse e a rota
                return h;

            }

        }

        public static double CalcularDistanciaEntrePontoERota(Dominio.ObjetosDeValor.Embarcador.Logistica.WayPoint wayPoint, string polilinha)
        {
            return CalcularDistanciaEntrePontoERota(wayPoint, Decodificar(polilinha));
        }

        /*
        * Verificar se um determinado ponto está em uma rota, tolerando uma distãncia mínima.
        * 
        *                        ---
        *                      /     \
        * A) P0---------------|P1-X---|-------------------P2          Distancia(P1, X) < Tolerância = Na rota
        *                      \     /
        *                        ---
        *    O ponto de interesse está próximo de um dos pontos da rota. Com o cálculo de distância 
        *    entre P1 e X, é possível perceber que, dada uma tolerância, é possível considerar que 
        *    o ponto X está na rota.
        * 
        *    
        *                        ---
        *                      /     \                                Distancia(P0, X) > Tolerância
        * C) P0---------------|---X---|------------------P1           Distancia(X, P1) > Tolerância
        *                      \     /                                Distancia(X, Reta(P0, P1)) < Tolerância
        *                        ---
        *    No caso C, a distância entre o ponto X e os pontos da rota mais próximos dele, P0 e P1
        *    são maiores que a tolerância. Entretanto, o ponto X está exatamente sobre a linha que 
        *    une os pontos P0 e P1. O ponto X deve ser considerado como pertencente a rota.
        *    
        * 
        *                        ---                                  Distancia(P0, X) > Tolerância
        * D) P0----------------/-----\-------------------P1           Distancia(X, P1) > Tolerância
        *                     |   X   |                               Distancia(X, Reta(P0, P1)) < Tolerância = Na rota
        *                      \     /                                      
        *                        ---
        *    A distância entre o ponto X e os pontos da rota mais próximos dele, P0 e P0 também 
        *    são maiores que a tolerância. Mas a distância entre o ponto X e a linha que une os pontos 
        *    P0 e P1 é menor que a tolerância. O ponto X deve ser considerado como pertencente a rota.
        *    
        *    
        *                                                             Distancia(P0, X) > Tolerância
        * E) P0------------------------------------------P1           Distancia(X, P1) > Tolerância
        *                        ---                                  Distancia(X, Reta(P0, P1)) > Tolerância = Fora da rota
        *                      /     \
        *                     |   X   |
        *                      \     /
        *                        ---
        *    Novamente a distância entre o ponto X e os pontos da rota mais próximos dele, P0 e P0 
        *    são maiores que a tolerância. Todavia, a distância entre o ponto X e a linha que une os pontos 
        *    P0 e P1 é maior que a tolerância. O ponto X deve ser considerado como não pertencente a rota.
        *
        */
        public static bool VerificarSePontoEstaNaRota(Dominio.ObjetosDeValor.Embarcador.Logistica.WayPoint wayPoint, List<Dominio.ObjetosDeValor.Embarcador.Logistica.WayPoint> wayPointsRota, int toleranciaEmMetros)
        {
            if (wayPoint == null || wayPointsRota == null || wayPointsRota.Count == 0) return false;
            return CalcularDistanciaEntrePontoERota(wayPoint, wayPointsRota) <= toleranciaEmMetros;
        }

        public static bool VerificarSePontoEstaProximoDaRota(string polilinha, Dominio.ObjetosDeValor.Embarcador.Logistica.WayPoint wayPoint, int toleranciaEmMetros)
        {
            return VerificarSePontoEstaNaRota(wayPoint, Decodificar(polilinha), toleranciaEmMetros);
        }

        /**
         * Localiza os pontos vizinhos de um determinado ponto em uma rota
         */
        public static List<Dominio.ObjetosDeValor.Embarcador.Logistica.WayPoint> ObterPontosVizinhos(Dominio.ObjetosDeValor.Embarcador.Logistica.WayPoint wayPoint, List<Dominio.ObjetosDeValor.Embarcador.Logistica.WayPoint> wayPointsRota, bool anteriores = true, bool proximos = true, int nivel = 1)
        {
            List<Dominio.ObjetosDeValor.Embarcador.Logistica.WayPoint> pontosVizinhos = new List<Dominio.ObjetosDeValor.Embarcador.Logistica.WayPoint>();
            if (wayPointsRota != null)
            {
                // Localiza o ponto na rota
                int indexWayPoint = BuscarIndiceDoPontoNaRota(wayPoint, wayPointsRota);
                if (indexWayPoint != -1)
                {

                    // Deve buscar os vizinhos anteriores?
                    if (anteriores)
                    {
                        // Adiciona os vizinhos a partir do nível solicitado
                        int indexLimiteWayPointAnterior = indexWayPoint - nivel;
                        int indexInicial = Math.Max(indexLimiteWayPointAnterior, 0);
                        for (int i = indexInicial; i < indexWayPoint; i++)
                        {
                            pontosVizinhos.Add(wayPointsRota[i]);
                        }
                    }

                    // Deve buscar os vizinhos posteriores?
                    if (proximos)
                    {
                        // Adiciona os vizinhos até o nível solicitado
                        int indexLimiteWayPointPosterior = indexWayPoint + nivel;
                        int indexFinal = Math.Min(indexLimiteWayPointPosterior, wayPointsRota.Count - 1);
                        for (int i = indexWayPoint + 1; i <= indexFinal; i++)
                        {
                            pontosVizinhos.Add(wayPointsRota[i]);
                        }
                    }

                }

            }
            return pontosVizinhos;
        }

        /**
         * Localiza os pontos vizinhos imediatos de um determinado ponto em uma rota, ou seja, o vizinho imediatamente anterior e o vizinho imediatamente posterior
         */
        public static List<Dominio.ObjetosDeValor.Embarcador.Logistica.WayPoint> ObterPontosVizinhosImediatos(Dominio.ObjetosDeValor.Embarcador.Logistica.WayPoint wayPoint, List<Dominio.ObjetosDeValor.Embarcador.Logistica.WayPoint> wayPointsRota)
        {
            return ObterPontosVizinhos(wayPoint, wayPointsRota, true, true, 1);
        }

        /**
         * Localiza o ponto vizinho imediatamente anterior 
         */
        public static Dominio.ObjetosDeValor.Embarcador.Logistica.WayPoint ObterPontoVizinhoAnterior(Dominio.ObjetosDeValor.Embarcador.Logistica.WayPoint wayPoint, List<Dominio.ObjetosDeValor.Embarcador.Logistica.WayPoint> wayPointsRota)
        {
            List<Dominio.ObjetosDeValor.Embarcador.Logistica.WayPoint> vizinhosImediatos = ObterPontosVizinhos(wayPoint, wayPointsRota, true, false, 1);
            return (vizinhosImediatos.Count == 1) ? vizinhosImediatos[0] : null;
        }

        /**
         * Localiza o ponto vizinho imediatamente posterior 
         */
        public static Dominio.ObjetosDeValor.Embarcador.Logistica.WayPoint ObterPontoVizinhoPosterior(Dominio.ObjetosDeValor.Embarcador.Logistica.WayPoint wayPoint, List<Dominio.ObjetosDeValor.Embarcador.Logistica.WayPoint> wayPointsRota)
        {
            List<Dominio.ObjetosDeValor.Embarcador.Logistica.WayPoint> vizinhosImediatos = ObterPontosVizinhos(wayPoint, wayPointsRota, false, true, 1);
            return (vizinhosImediatos.Count == 1) ? vizinhosImediatos[0] : null;
        }

        /**
         * Extrai WayPoints das posições de um determinado tempo
         */
        public static bool VerificaSePercorreuDistanciaMinima(
            List<Dominio.ObjetosDeValor.Embarcador.Logistica.WayPoint> wayPointsRota,
            int distanciaMinimaMetros
        )
        {
            // Calcula a distância percorrida
            double distanciaMetros = CalcularDistancia(wayPointsRota);

            // Confirma ter percorrida a distância mínima
            return (distanciaMetros >= distanciaMinimaMetros);
        }

        /**
         * Calcula uma taxa de certeza de que a ordem das posições indicam que está se APROXIMANDO do ponto de destino 
         */
        public static double CalculaTaxaDeAproximacaoContinua(Dominio.ObjetosDeValor.Embarcador.Logistica.WayPoint wayPoint, List<Dominio.ObjetosDeValor.Embarcador.Logistica.WayPoint> wayPointsRota)
        {
            DistanciasPercorridas distanciasPercorridas = CalcularDistanciasPercorridas(wayPoint, wayPointsRota);
            if (distanciasPercorridas.Total > 0)
            {
                double taxa = distanciasPercorridas.Aproximando / distanciasPercorridas.Total;
                return taxa;
            }
            return 0;
        }

        /**
         * Calcula uma taxa de certeza de que a ordem das posições indicam que está se AFASTANDO do ponto de origem 
         */
        public static double CalculaTaxaDeAfastamentoContinuo(Dominio.ObjetosDeValor.Embarcador.Logistica.WayPoint wayPoint, List<Dominio.ObjetosDeValor.Embarcador.Logistica.WayPoint> wayPointsRota)
        {
            DistanciasPercorridas distanciasPercorridas = CalcularDistanciasPercorridas(wayPoint, wayPointsRota);
            if (distanciasPercorridas.Total > 0)
            {
                double taxa = distanciasPercorridas.Afastando / distanciasPercorridas.Total;
                return taxa;
            }
            return 0;
        }

        /**
         * Exemplo de rota. Letras são pontos na rota, letras maíusculas são pontos que são a origem e os destinos da rota.
         * 
         * Exemplo 1: rota com início e fim diferentes
         *   Pontos: (A)  b  c  d  e  f  (G)  h  i  j  k  l  (M)  n  o  p  q  r  (S)  t  u  v  w  x  (Y)
         *   Índice:  0   1  2  3  4  5   6   7  8  9  10 11 12  13 14 15 16 17  18  19 20 21 22 23  24 
         *   Rota = (A,b,c,d,e,f,G,h,i,j,k,l,M,n,o,p,q,r,S,t,u,v,w,x,Y)
         *   Pontos de passagem = (A,G,M,S,Y)
         *   Lista de trechos:
         *   - De A até G (A,b,c,d,e,f,G) (0:6), 7 pontos
         *   - De G até M (G,h,i,j,k,l,M) (6:12), 7 pontos
         *   - De M até S (M,n,o,p,q,r,S) (12:18), 7 pontos
         *   - De S até Y (S,t,u,v,w,x,Y) (18:24), 7 pontos
         *  
         *  Exemplo 2: rota com início e fim iguais, um ciclo, entrega e volta até a origem
         *   Pontos: (A)  b  c  d  e  f  (G)  h  i  j  k  l  (M)  n  o  p  q  r  (S)  t  u  v  w  x  (A)
         *   Índice:  0   1  2  3  4  5   6   7  8  9  10 11 12  13 14 15 16 17  18  19 20 21 22 23  24 
         *   Rota = (A,b,c,d,e,f,G,h,i,j,k,l,M,n,o,p,q,r,S,t,u,v,w,x,A)
         *   Pontos de passagem = (A,G,M,S,A)
         *   Lista de trechos:
         *   - De A até G (A,b,c,d,e,f,G) (0:6), 7 pontos
         *   - De G até M (G,h,i,j,k,l,M) (6:12), 7 pontos
         *   - De M até S (M,n,o,p,q,r,S) (12:18), 7 pontos
         *   - De S até A (S,t,u,v,w,x,A) (18:24), 7 pontos
         */
        public static List<List<Dominio.ObjetosDeValor.Embarcador.Logistica.WayPoint>> SepararTrechosDaRotaPorPontosDePassagem(List<Dominio.ObjetosDeValor.Embarcador.Logistica.WayPoint> wayPointsPassagem, List<Dominio.ObjetosDeValor.Embarcador.Logistica.WayPoint> wayPointsRota)
        {
            List<List<Dominio.ObjetosDeValor.Embarcador.Logistica.WayPoint>> trechosDaRota = new List<List<Dominio.ObjetosDeValor.Embarcador.Logistica.WayPoint>>();

            Dominio.ObjetosDeValor.Embarcador.Logistica.WayPoint[] wayPointsRotaArray = wayPointsRota.ToArray();

            int totalPontosDePassagem = wayPointsPassagem?.Count ?? 0;
            int totalPontosRota = wayPointsRota?.Count ?? 0;

            if (totalPontosDePassagem > 1 && totalPontosRota > 1)
            {
                int indiceInicial = 0;

                // Percorre todos os pontos
                for (int i = 1; i < totalPontosDePassagem; i++)
                {
                    // Localiza o ponto na rota mais próximo do ponto de interesse, a partir do índice já visitado
                    int indice = LocalizaPontoMaisProximoDaRotaIndice(wayPointsRotaArray, wayPointsPassagem[i], int.MaxValue, indiceInicial + 1);
                    if (indice > -1 && indice > indiceInicial)
                    {

                        // Extrai a fatia da lista de waypoints do indice atuakl até o indice do waypoint encontrado, inclusive
                        List<Dominio.ObjetosDeValor.Embarcador.Logistica.WayPoint> trecho = wayPointsRota.Skip(indiceInicial).Take(indice - indiceInicial + 1).ToList();
                        if (trecho != null && trecho.Count > 0)
                        {
                            // Garante que o primeiro e último pontos sejam exatamente as coordenadas dos pontos de passagem de origem e destino do trecho
                            if (!trecho.First().Equals(wayPointsPassagem[i - 1])) trecho.Insert(0, wayPointsPassagem[i - 1]);
                            if (!trecho.Last().Equals(wayPointsPassagem[i])) trecho.Add(wayPointsPassagem[i]);

                            trechosDaRota.Add(trecho);
                            indiceInicial = indice;

                        }


                    }
                }

            }

            return trechosDaRota;
        }

        #endregion

        #region Métodos privados

        private static double Rad2deg(double rad)
        {
            return (rad / Math.PI * 180.0);
        }

        private static double Deg2rad(double deg)
        {
            return (deg * Math.PI / 180.0);
        }

        /**
         * Retorna a quantidade de casas decimais mínimas necessárias para que uma coordenada possa representar a acurácia desejada.
         * Usado para simplificação de determinação da distância entre duas coordenadas.
         * https://wiki.openstreetmap.org/wiki/Precision_of_coordinates
         */
        private static int CasasDecimaisParaAcuraciaMinima(int toleranciaEmMetros)
        {

            // Coordenadas com acurácia de 1.1m
            if (toleranciaEmMetros < 10)
                return 5;

            // Coordenadas com acurácia de 11m
            else if (toleranciaEmMetros < 10)
                return 4;

            // Coordenadas com acurácia de 110m
            else if (toleranciaEmMetros < 100)
                return 3;

            // Coordenadas com acurácia de 1.1Km
            else if (toleranciaEmMetros < 1000)
                return 2;

            // Coordenadas com acurácia de 11.1Km
            else if (toleranciaEmMetros < 10000)
                return 1;

            // Coordenadas com acurácia de 111Km
            return 0;

        }

        /**
         * Busca o índice do WayPoint na rota
         */
        private static int BuscarIndiceDoPontoNaRota(Dominio.ObjetosDeValor.Embarcador.Logistica.WayPoint wayPoint, List<Dominio.ObjetosDeValor.Embarcador.Logistica.WayPoint> wayPointsRota)
        {
            if (wayPoint != null && wayPointsRota != null && wayPointsRota.Count > 0)
            {
                for (int index = 0; index < wayPointsRota.Count; index++)
                {
                    if (wayPoint.Equals(wayPointsRota[index]))
                        return index;
                }
            }
            return -1;
        }

        /**
         * Identifica qual das distâncias é a maior, considerada a hipotenusa de um triângulo
         */
        private static double IdentificarHipotenusa(double d1, double d2, double d3)
        {
            return Math.Max(d1, Math.Max(d2, d3));
        }

        /**
         * Identifica qual das distâncias é a menor, considerado o um dos catetos de um triângulo
         */
        private static double IdentificarCatetoMenor(double d1, double d2, double d3)
        {
            return Math.Min(d1, Math.Min(d2, d3));
        }

        /**
         * Identifica qual das distâncias é o cateto maior
         */
        private static double IdentificarCatetoMaior(double d1, double d2, double d3)
        {
            double hipotenusa = IdentificarHipotenusa(d1, d2, d3);
            double cateto1 = IdentificarCatetoMenor(d1, d2, d3);
            if (d1 != hipotenusa && d1 != cateto1)
            {
                return d1;
            }
            else if (d2 != hipotenusa && d2 != cateto1)
            {
                return d2;
            }
            else
            {
                return d3;
            }
        }

        /**
         * Identifica qual das distâncias é o cateto maior
         * [0] = Hipotenusa
         * [1] = Cateto 1
         * [2] = Cateto 2
         */
        private static List<double> IdentificarHipotenusaECatetos(double d1, double d2, double d3)
        {
            double hipotenusa = IdentificarHipotenusa(d1, d2, d3);
            double cateto1 = IdentificarCatetoMenor(d1, d2, d3);
            double cateto2;
            if (d1 != hipotenusa && d1 != cateto1)
            {
                cateto2 = d1;
            }
            else if (d2 != hipotenusa && d2 != cateto1)
            {
                cateto2 = d2;
            }
            else
            {
                cateto2 = d3;
            }
            List<double> lados = new List<double>();
            lados.Add(hipotenusa);
            lados.Add(cateto1);
            lados.Add(cateto2);
            return lados;
        }

        /**
         * Calcula as distâncias percorridas na rota que se aproximam e se afastam do destino
         */
        private static DistanciasPercorridas CalcularDistanciasPercorridas(Dominio.ObjetosDeValor.Embarcador.Logistica.WayPoint wayPointDestino, List<Dominio.ObjetosDeValor.Embarcador.Logistica.WayPoint> wayPointsRota)
        {
            DistanciasPercorridas distanciasPercorridas = new DistanciasPercorridas();
            double distanciaPercorrida = 0,
                   distanciaAteDestino = 0,
                   distanciaAteDestinoAnterior = 0;
            int total = wayPointsRota.Count;
            for (int i = 0; i < total; i++)
            {
                distanciaAteDestino = CalcularDistancia(wayPointDestino, wayPointsRota[i]);
                if (distanciaAteDestinoAnterior > 0)
                {

                    // Distância percorrida absoluta
                    distanciaPercorrida = Math.Abs(distanciaAteDestinoAnterior - distanciaAteDestino);

                    // Se aproximou do destino
                    if (distanciaAteDestino < distanciaAteDestinoAnterior)
                    {
                        distanciasPercorridas.Aproximando += distanciaPercorrida;
                    }
                    // Se afastou do destino
                    else
                    {
                        distanciasPercorridas.Afastando += distanciaPercorrida;
                    }
                }
                distanciaAteDestinoAnterior = distanciaAteDestino;
            }
            distanciasPercorridas.Total = distanciasPercorridas.Aproximando + distanciasPercorridas.Afastando;

            return distanciasPercorridas;
        }

        #endregion

    }

}
