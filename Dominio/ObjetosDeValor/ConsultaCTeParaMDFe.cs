using System;

namespace Dominio.ObjetosDeValor
{
    public class ConsultaCTeParaMDFe
    {
        public int Codigo { get; set; }
        public string UFDescarregamento { get; set; }
        public string UFCarregamento { get; set; }
        public decimal ValorTotalMercadoria { get; set; }
        public decimal PesoTotal { get; set; }
        public int Numero { get; set; }
        public int Serie { get; set; }
        public DateTime? DataEmissao { get; set; }
        public string LocalidadeInicioPrestacao { get; set; }
        public string UFInicioPrestacao { get; set; }
        public string LocalidadeTerminoPrestacao { get; set; }
        public string UFTerminoPrestacao { get; set; }
        public decimal ValorFrete { get; set; }
        public decimal PesoKgTotal { get; set; }
        public string Averbado { get; set; }
    }
}
