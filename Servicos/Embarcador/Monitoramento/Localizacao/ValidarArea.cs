using System.Collections.Generic;
using System.Linq;

namespace Servicos.Embarcador.Monitoramento.Localizacao
{
    public static class ValidarArea
    {

        #region Métodos públicos

        public static Dominio.Entidades.Embarcador.Logistica.Locais BuscarLocalEmArea(Dominio.Entidades.Embarcador.Logistica.Locais[] locais, Dominio.Entidades.Embarcador.Logistica.PosicaoAtual PosicaoAtual)
        {
            return BuscarLocalEmArea(locais, PosicaoAtual.Latitude, PosicaoAtual.Longitude);
        }

        public static Dominio.Entidades.Embarcador.Logistica.Locais BuscarLocalEmArea(Dominio.Entidades.Embarcador.Logistica.Locais[] locais, double latitude, double longitude)
        {
            int total = locais?.Length ?? 0;
            for (int i = 0; i < total; i++)
            {
                if (!string.IsNullOrWhiteSpace(locais[i].Area))
                {
                    bool emArea = Servicos.Embarcador.Logistica.Monitoramento.Monitoramento.EmArea(locais[i].Area, latitude, longitude);
                    if (emArea) return locais[i];
                }
            }
            return null;
        }

        public static Dominio.Entidades.Embarcador.Logistica.RaioProximidade BuscarRaioVeiculoEmAreaRaioProximidade(Dominio.Entidades.Embarcador.Logistica.RaioProximidade[] raios, double latitude, double longitude)
        {
            int total = raios?.Length ?? 0;
            for (int i = 0; i < total; i++)
            {
                if (raios[i] != null)
                {
                    bool emArea = Servicos.Embarcador.Logistica.Monitoramento.Monitoramento.EmRaioProximidadeLocal(raios[i], latitude, longitude);
                    if (emArea) return raios[i];
                }
            }
            return null;
        }

        public static List<Dominio.Entidades.Embarcador.Logistica.Locais> BuscarLocaisEmArea(Dominio.Entidades.Embarcador.Logistica.Locais[] locais, double latitude, double longitude)
        {
            List<Dominio.Entidades.Embarcador.Logistica.Locais> locaisEmArea = new List<Dominio.Entidades.Embarcador.Logistica.Locais>();
            int total = locais?.Length ?? 0;
            for (int i = 0; i < total; i++)
            {
                if (!string.IsNullOrWhiteSpace(locais[i].Area))
                {
                    bool emArea = Servicos.Embarcador.Logistica.Monitoramento.Monitoramento.EmArea(locais[i].Area, latitude, longitude);
                    if (emArea) locaisEmArea.Add(locais[i]);
                }
            }
            return locaisEmArea;
        }

        public static Dominio.ObjetosDeValor.Embarcador.Logistica.SubareaCliente[] BuscarSubareasClienteEmArea(Dominio.ObjetosDeValor.Embarcador.Logistica.SubareaCliente[] subareas, double latitude, double longitude)
        {
            List<Dominio.ObjetosDeValor.Embarcador.Logistica.SubareaCliente> subareasEmAlvo = new List<Dominio.ObjetosDeValor.Embarcador.Logistica.SubareaCliente>();
            int total = subareas?.Length ?? 0;
            for (int i = 0; i < total; i++)
            {
                if (!string.IsNullOrWhiteSpace(subareas[i].Area))
                {
                    bool emArea = Servicos.Embarcador.Logistica.Monitoramento.Monitoramento.EmArea(subareas[i].Area, latitude, longitude);
                    if (emArea) subareasEmAlvo.Add(subareas[i]);
                }
            }
            return subareasEmAlvo.ToArray();
        }

        public static List<Dominio.ObjetosDeValor.Cliente> BuscarClientesEmArea(Dominio.ObjetosDeValor.Cliente[] clientes, double latitude, double longitude, Dominio.Entidades.Embarcador.Configuracao.ConfiguracaoTMS configuracaoEmbarcador)
        {
            List<Dominio.ObjetosDeValor.Cliente> clientesEmAlvo = new List<Dominio.ObjetosDeValor.Cliente>();
            int total = clientes.Length;
            for (int i = 0; i < total; i++)
            {
                bool estaNoRaio = Servicos.Embarcador.Logistica.Monitoramento.Monitoramento.ValidarEmRaioOuAreaCliente(clientes[i], latitude, longitude, configuracaoEmbarcador.RaioPadrao);
                if (estaNoRaio) clientesEmAlvo.Add(clientes[i]);
            }
            return clientesEmAlvo;
        }

        public static Dominio.ObjetosDeValor.Embarcador.Logistica.WayPoint CalcularCentroDaArea(List<Dominio.ObjetosDeValor.Embarcador.Logistica.Path> coordenadas)
        {
            if (coordenadas == null || coordenadas.Count == 0) return null;

            double somaLat = coordenadas.Sum(c => c.lat);
            double somaLng = coordenadas.Sum(c => c.lng);
            int numeroDeCoordenadas = coordenadas.Count;

            return new Dominio.ObjetosDeValor.Embarcador.Logistica.WayPoint
            {
                Latitude = somaLat / numeroDeCoordenadas,
                Longitude = somaLng / numeroDeCoordenadas
            };
        }

        #endregion

    }
}