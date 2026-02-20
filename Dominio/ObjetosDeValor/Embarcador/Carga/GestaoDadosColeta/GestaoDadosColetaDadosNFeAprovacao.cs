using System;

namespace Dominio.ObjetosDeValor.Embarcador.Carga.GestaoDadosColeta
{
    public class GestaoDadosColetaDadosNFeAprovacao
    {
        public string Chave { get; set; }

        public int Numero { get; set; }

        public string Serie { get; set; }

        public DateTime DataEmissao { get; set; }

        public decimal Peso { get; set; }

        public int Volumes { get; set; }

        public decimal Valor { get; set; }

        public double CpfCnpjEmitente { get; set; }

        public double CpfCnpjDestinatario { get; set; }
    }
}
