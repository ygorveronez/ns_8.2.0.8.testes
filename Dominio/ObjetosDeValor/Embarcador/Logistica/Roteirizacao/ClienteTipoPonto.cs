using System;

namespace Dominio.ObjetosDeValor.Embarcador.Logistica.Roteirizacao
{
    public class ClienteTipoPonto : IEquatable<Dominio.ObjetosDeValor.Embarcador.Logistica.Roteirizacao.ClienteTipoPonto>
    {
        public double Codigo { get; set; }

        public Dominio.Entidades.Cliente Cliente { get; set; }

        public bool ColetaEquipamento { get; set; }

        public Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoPontoPassagem TipoPontoPassagem { get; set; }

        public bool UsarOutroEndereco { get; set; }

        public int SequenciaPreDefinida { get; set; }

        public Dominio.Entidades.Embarcador.Pessoas.ClienteOutroEndereco ClienteOutroEndereco { get; set; }

        public bool Equals(Dominio.ObjetosDeValor.Embarcador.Logistica.Roteirizacao.ClienteTipoPonto other)
        {
            return (other.Codigo == this.Codigo);
        }

        public double Latitude
        {
            get
            {
                return this.UsarOutroEndereco ? ParseDouble(this.ClienteOutroEndereco.Latitude) : ParseDouble(this.Cliente.Latitude);
            }
        }

        public double Longitude
        {
            get
            {
                return this.UsarOutroEndereco ? ParseDouble(this.ClienteOutroEndereco.Longitude) : ParseDouble(this.Cliente.Longitude);
            }
        }

        public bool PrimeiraEntrega { get; set; }

        public override int GetHashCode()
        {
            return this.Codigo.GetHashCode();
        }

        private double ParseDouble(string value)
        {
            return double.Parse(value?.Replace(",", "."), System.Globalization.CultureInfo.InvariantCulture);
        }
    }
}
