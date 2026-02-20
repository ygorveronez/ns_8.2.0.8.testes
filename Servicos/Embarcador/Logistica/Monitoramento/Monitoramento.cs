using Dominio.Entidades.Embarcador.Logistica;
using System;
using System.Collections.Generic;

namespace Servicos.Embarcador.Logistica.Monitoramento
{
    public static class Monitoramento
    {
        public static bool EmArea(string desenhoArea, double latitude, double longitude)
        {
            var areas = Newtonsoft.Json.JsonConvert.DeserializeObject<List<Dominio.ObjetosDeValor.Embarcador.Logistica.LocalArea>>(desenhoArea);

            if (areas == null)
                return false;


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
                        Dominio.ObjetosDeValor.Embarcador.Logistica.WayPoint pontoSouthEast = new Dominio.ObjetosDeValor.Embarcador.Logistica.WayPoint { Latitude = area.bounds.south, Longitude = area.bounds.east };

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

            return false;
        }

        public static bool EmRaioProximidadeLocal(RaioProximidade raio, double latitude, double longitude)
        {
            var areas = Newtonsoft.Json.JsonConvert.DeserializeObject<List<Dominio.ObjetosDeValor.Embarcador.Logistica.LocalArea>>(raio.Local.Area);

            if (areas == null)
                return false;


            foreach (var area in areas)
            {
                var pontoOrigem = new Dominio.ObjetosDeValor.Embarcador.Logistica.WayPoint { Latitude = latitude, Longitude = longitude };

                var emArea = false;
                switch (area.type)
                {
                    case "marker":
                        var pontoDestino = new Dominio.ObjetosDeValor.Embarcador.Logistica.WayPoint { Latitude = area.position.lat, Longitude = area.position.lng };
                        emArea = Servicos.Embarcador.Logistica.Distancia.ValidarNoRaio(pontoOrigem, pontoDestino, raio.Raio);
                        break;
                }

                if (emArea)
                    return true;
            }

            return false;
        }

        public static bool EmRaioCliente(Dominio.Entidades.Cliente cliente, double latitude, double longitude, int raioPadrao = 300)
        {
            var lat = double.Parse(cliente.Latitude.Replace(',', '.'), System.Globalization.CultureInfo.InvariantCulture);
            var lng = double.Parse(cliente.Longitude.Replace(',', '.'), System.Globalization.CultureInfo.InvariantCulture);

            Dominio.ObjetosDeValor.Cliente clienteOV = new Dominio.ObjetosDeValor.Cliente
            {
                Latitude = lat,
                Longitude = lng,
                TipoArea = cliente.TipoArea ?? Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoArea.Raio,
                Raio = cliente.RaioEmMetros.HasValue ? cliente.RaioEmMetros.Value : raioPadrao,
                Area = cliente.Area,
            };
            return EmRaioCliente(clienteOV, latitude, longitude, raioPadrao);
        }

        public static bool EmRaioCliente(Dominio.ObjetosDeValor.Cliente cliente, double latitude, double longitude, int raioPadrao = 300)
        {
            if (cliente != null)
            {
                var raio = (cliente.Raio > 0) ? cliente.Raio : raioPadrao;
                double distancia = Servicos.Embarcador.Logistica.Distancia.CalcularDistanciaMetros(latitude, longitude, cliente.Latitude, cliente.Longitude);
                if (distancia < raio)
                    return true;
            }
            return false;
        }

        public static bool ValidarEmRaioOuAreaCliente(Dominio.Entidades.Cliente cliente, double latitude, double longitude, int raioPadrao = 300)
        {
            if (cliente == null || string.IsNullOrWhiteSpace(cliente.Latitude) || string.IsNullOrWhiteSpace(cliente.Longitude) || latitude == 0 || longitude == 0) return false;

            if (cliente.TipoArea == Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoArea.Poligono)
                return EmArea(cliente.Area, latitude, longitude);
            else
                return EmRaioCliente(cliente, latitude, longitude, raioPadrao);
        }

        public static bool ValidarAreaLocal(Dominio.Entidades.Embarcador.Logistica.Locais local, double latitude, double longitude)
        {
            if (local == null || latitude == 0 || longitude == 0) return false;
            return EmArea(local.Area, latitude, longitude);
        }

        public static bool ValidarEmRaioOuAreaCliente(Dominio.ObjetosDeValor.Cliente cliente, double latitude, double longitude, int raioPadrao = 300)
        {
            if (cliente == null || cliente.Latitude == 0 || cliente.Longitude == 0 || latitude == 0 || longitude == 0) return false;

            if (cliente.TipoArea == Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoArea.Poligono)
                return EmArea(cliente.Area, latitude, longitude);
            else
                return EmRaioCliente(cliente, latitude, longitude, raioPadrao);
        }

        public static bool ValidarEntrouESaiuDoRaioSemEntregar(Dominio.Entidades.Embarcador.Cargas.ControleEntrega.CargaEntrega cargaEntrega, Dominio.Entidades.Embarcador.Configuracao.ConfiguracaoTMS configuracao)
        {
            if (cargaEntrega.DataEntradaRaio == null || cargaEntrega.DataSaidaRaio == null || cargaEntrega.DataEntradaRaio == DateTime.MinValue || cargaEntrega.DataSaidaRaio == DateTime.MinValue)
            {
                return false;
            }

            // Se ele entrou e saiu do raio e ficou pelo menos X minutos dentro dele, mas não entregou, retorna true
            double tempoNoRaio = (cargaEntrega.DataSaidaRaio - cargaEntrega.DataEntradaRaio).Value.TotalMinutes;
            bool ficouTempoMinimoNecessario = tempoNoRaio > configuracao.TempoMinimoAcionarPassouRaioSemConfirmar;
            bool naoEntregou = cargaEntrega.Situacao == Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoEntrega.NaoEntregue || cargaEntrega.Situacao == Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoEntrega.EmCliente;
            return naoEntregou && ficouTempoMinimoNecessario;
        }

    }
}
