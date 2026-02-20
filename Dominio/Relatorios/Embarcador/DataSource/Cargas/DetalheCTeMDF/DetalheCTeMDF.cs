using System.Collections.Generic;

namespace Dominio.Relatorios.Embarcador.DataSource.Cargas
{
    public class DetalheCTeMDF
    {
        public string Filial { get; set; }
        public string NumeroCarga { get; set; }

        public string ValePedagio { get; set; }
        public decimal ValorPedagio { get; set; }
        public string Transportadora { get; set; }
        public string Placa { get; set; }

        public List<NotaFiscal> NFes { get; set; }
        public List<CTe> CTes { get; set; }
        public List<MDFe> MDFes { get; set; }
        public int TotalCaixas { get; set; }
        public decimal PesoBruto { get; set; }
        public decimal QuantidadePaletes { get; set; }
        public string Destino { get; set; }
        public string Motorista { get; set; }
        public decimal ValorFrete { get; set; }
        public decimal ValorNF { get; set; }
        public int QuantidadeVolumes { get; set; }
    }
}
