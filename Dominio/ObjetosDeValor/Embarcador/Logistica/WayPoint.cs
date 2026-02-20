using System;
using System.Globalization;

namespace Dominio.ObjetosDeValor.Embarcador.Logistica
{

    public class WayPoint
    {

        public override string ToString()
        {
            return this.Latitude.ToString() + " " + this.Longitude.ToString();
        }

        #region Atributos públicos

        public Double Latitude { get; set; }
        public Double Longitude { get; set; }

        #endregion

        #region Construtores

        public WayPoint()
        {

        }

        /**
         * Construtor do ponto a partir de um par de coordenadas decimais
         */
        public WayPoint (double latitude, double longitude)
        {
            Latitude = latitude;
            Longitude = longitude;
        }

        public WayPoint(decimal latitude, decimal longitude)
        {
            Latitude = (double) latitude;
            Longitude = (double) longitude;
        }

        /**
         * Construtor do ponto a partir de um par de coordenadas em string
         */
        public WayPoint(string latitude, string longitude)
        {
            NumberFormatInfo provider = new NumberFormatInfo();
            provider.NumberDecimalSeparator = ".";
            try
            {
                Latitude = Convert.ToDouble(latitude, provider);
                Longitude = Convert.ToDouble(longitude, provider);
            }
            catch
            {
                ParseWayPointGMS(latitude, longitude);
            }
        }

        /**
         * Conversão a partir de um par de coordenadas GMS = Graus, Minutos,Segundos(DMS, degree, minutes, seconds)
         * Formatos já suportados:
         *  - Latitude em graus: 023_32_13_0_S, sendo: 023 = Graus, 32 = min, 13 = seg, 0= décimos de segundo e S = orientação
         *  - Longitude em graus: 046_36_40_0_W, sendo: 046 = Graus, 36 = min, 40 = seg, 0 = décimos de seg e W = orientacao
         *  
         * Pendentes de desenvolvimento:
         *  - 12,00º13,00'25,00"W
         *  - 12,00º 13,00' 25,00" W
         */
        public void ParseWayPointGMS(string latitudeGMS, string longitudeGMS)
        {
            Latitude = ConverterGMS1ParaDecimal(latitudeGMS);
            Longitude = ConverterGMS1ParaDecimal(longitudeGMS);
        }

        public bool Equals(WayPoint wp)
        {
            return wp.Latitude == this.Latitude && wp.Longitude == this.Longitude;
        }

        #endregion

        #region Métodos privados

        /**
         * Conversão de uma coordenada no formato GMS para decimal.
         * Exemplo: 023_32_13_0_S, sendo: 023 = Graus, 32 = min, 13 = seg, 0= décimos de segundo e S = orientação
         */
        private double ConverterGMS1ParaDecimal(string coordenadaGMS)
        {
            string[] pieces = coordenadaGMS.Split('_');
            double coordenadaDecimal;
            if (pieces.Length == 5)
            {
                double graus = Int16.Parse(pieces[0]);
                double minutos = Int16.Parse(pieces[1]);
                double segundos = Int16.Parse(pieces[2]);
                double fracaoSegundos = Int16.Parse(pieces[3]);
                string orientacao = pieces[4].Trim().ToUpper();

                if (!ValidaOrientacao(orientacao))
                {
                    throw new BadFormatException($"A orientação {orientacao} da coordenada {coordenadaGMS} é inválida");
                }

                // Conversão de graus, minutos e segundos
                coordenadaDecimal = graus + (minutos / 60) + ((segundos + (fracaoSegundos / 10)) / 3600);

                // Correção do sinal
                if (orientacao == "S" || orientacao == "W")
                {
                    coordenadaDecimal *= -1;
                }
            }
            else
            {
                throw new BadFormatException($"A coordenada {coordenadaGMS} não está no formato correto (999_99_99_9_X)");
            }

            // Neste formato atinge até 6 casas decimais de precisão
            return Math.Round(coordenadaDecimal, 6);
        }

        /**
         * Verifica se a sigla da orientação é válida
         */
        private bool ValidaOrientacao(string orientacao, bool latitude = true, bool longitude = true)
        {
            if (latitude && (orientacao == "N" || orientacao == "S"))
            {
                return true;
            }
            if (longitude && (orientacao == "W" || orientacao == "E"))
            {
                return true;
            }
            return false;
        }

        #endregion

    }

    public class BadFormatException : Exception
    {
        public BadFormatException() {}
        public BadFormatException(string message) : base(message) {}
        public BadFormatException(string message, Exception inner) : base(message, inner) {}
    }

}
