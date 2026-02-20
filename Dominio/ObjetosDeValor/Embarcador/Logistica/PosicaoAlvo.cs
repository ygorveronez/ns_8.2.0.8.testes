using System;

namespace Dominio.ObjetosDeValor.Embarcador.Logistica
{
    public class PosicaoAlvo
    {
        public long CodigoPosicao { get; set; }

        public double CodigoCliente { get; set; }

        public int CodigoClienteSubArea { get; set; }

        public int CodigoVeiculo { get; set; }

        public DateTime DataVeiculo { get; set; }

        public double Latitude { get; set; }

        public double Longitude { get; set; }
    }
}
