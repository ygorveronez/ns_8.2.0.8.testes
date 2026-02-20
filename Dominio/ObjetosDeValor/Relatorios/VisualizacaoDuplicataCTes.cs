using System;

namespace Dominio.ObjetosDeValor.Relatorios
{
    public class VisualizacaoDuplicataCTes
    {
        public int CodigoDuplicata { get; set; }

        public int CodigoCTe { get; set; }

        public int Numero { get; set; }

        public int Serie { get; set; }

        public DateTime? Data { get; set; }
        
        public string Remetente { get; set; }

        public string Destinatario { get; set; }

        public string Veiculo1 { get; set; }

        public string Motorista1 { get; set; }

        public decimal Volume { get; set; }

        public decimal Peso { get; set; }

        public decimal ValorMercadoria { get; set; }

        public decimal ValorFrete { get; set; }

        public decimal ValorAReceber { get; set; }

        public string Notas { get; set; }

        public decimal ValorICMS { get; set; }
    }
}
