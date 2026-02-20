using System;
using System.Globalization;

namespace Dominio.ObjetosDeValor.Embarcador.Logistica.Roteirizacao
{

    public class WayPoint
    {

        #region Construtores
        public WayPoint() { }
        public WayPoint(double latitude, double longitude)
        {
            Lat = latitude;
            Lng = longitude;
        }
        public WayPoint(string latitude, string longitude)
        {
            NumberFormatInfo provider = new NumberFormatInfo();
            provider.NumberDecimalSeparator = ".";
            Lat = Convert.ToDouble(latitude, provider);
            Lng = Convert.ToDouble(longitude, provider);
        }
        #endregion

        public string Descricao { get; set; }
        public double Lat { get; set; }
        public double Lng { get; set; }
        public bool Pedagio { get; set; }
        public bool Fronteira { get; set; }
        public bool UsarOutroEndereco { get; set; }
        public bool ColetaEquipamento { get; set; }
        public bool LocalDeParqueamento { get; set; }
        public int Index { get; set; }
        public int Distancia { get; set; }
        public int Tempo { get; set; }
        public double Codigo { get; set; }
        public double CodigoCliente { get; set; }
        public int Sequencia { get; set; }
        public int SequenciaPredefinida { get; set; }
        public string Informacao { get; set; }
        public Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoPontoPassagem TipoPonto { get; set; }
        public int tempoEstimadoPermanencia { get; set; }

        public bool PrimeiraEntrega { get; set; }

        public int CodigoOutroEndereco { get; set; }
        public bool UtilizaLocalidade { get; set; }
    }
}
