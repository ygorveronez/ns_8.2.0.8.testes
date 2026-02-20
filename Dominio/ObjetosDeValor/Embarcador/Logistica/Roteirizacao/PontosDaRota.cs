using System;
using System.Globalization;
using Newtonsoft.Json;
using Utilidades.Json;

namespace Dominio.ObjetosDeValor.Embarcador.Logistica.Roteirizacao
{
    public class PontosDaRota
    {
        #region Construtores
        public PontosDaRota() { }
        public PontosDaRota(double latitude, double longitude)
        {
            lat = latitude;
            lng = longitude;
        }
        public PontosDaRota(Dominio.Entidades.Cliente cliente)
        {
            descricao = cliente.Descricao;
            codigo = cliente.CPF_CNPJ;
            codigo_cliente = cliente.CPF_CNPJ;

            NumberFormatInfo provider = new NumberFormatInfo();
            provider.NumberDecimalSeparator = ".";
            lat = Convert.ToDouble(cliente.Latitude, provider);
            lng = Convert.ToDouble(cliente.Longitude, provider);
        }
        #endregion

        public string descricao { get; set; }

        [JsonConverter(typeof(NullToFalseBoolConverter))]
        public bool pedagio { get; set; }

        [JsonConverter(typeof(NullToFalseBoolConverter))]
        public bool fronteira { get; set; }

        [JsonConverter(typeof(NullToZeroDoubleConverter))]
        public double lat { get; set; }

        [JsonConverter(typeof(NullToZeroDoubleConverter))]
        public double lng { get; set; }

        [JsonConverter(typeof(NullToFalseBoolConverter))]
        public bool pontopassagem { get; set; }

        [JsonConverter(typeof(NullToFalseBoolConverter))]
        public bool coletaEquipamento { get; set; }

        [JsonConverter(typeof(NullToFalseBoolConverter))]
        public bool localDeParqueamento { get; set; }

        [JsonConverter(typeof(NullToFalseBoolConverter))]
        public bool usarOutroEndereco { get; set; }

        [JsonConverter(typeof(NullToZeroIntConverter))]
        public int tempo { get; set; }

        [JsonConverter(typeof(NullToZeroIntConverter))]
        public int tempoEstimadoPermanencia { get; set; }

        [JsonConverter(typeof(NullToZeroIntConverter))]
        public int distancia { get; set; }

        /// <summary>
        /// Distância entre o ponto de partida até esse
        /// </summary>
        [JsonConverter(typeof(NullToZeroIntConverter))]
        public int distanciaDireta { get; set; }

        [JsonConverter(typeof(NullToZeroDoubleConverter))]
        public double codigo { get; set; }

        [JsonConverter(typeof(NullToZeroDoubleConverter))]
        public double codigo_cliente { get; set; }

        public Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoPontoPassagem tipoponto { get; set; }

        [JsonConverter(typeof(NullToFalseBoolConverter))]
        public bool primeiraEntrega { get; set; }

        [JsonConverter(typeof(NullToZeroIntConverter))]
        public int codigoOutroEndereco { get; set; }

        [JsonConverter(typeof(NullToFalseBoolConverter))]
        public bool utilizaLocalidade { get; set; }
    }
}
