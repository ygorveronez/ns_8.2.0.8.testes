using System;

namespace Dominio.ObjetosDeValor.Relatorios
{
    public class CobrancaCTeDocumento
    {
        public int Numero { get; set; }

        public int Serie { get; set; }

        public decimal Valor { get; set; }

        public DateTime DataEmissao { get; set; }

        public string NotaFiscal { get; set; }

        public string Origem { get; set; }

        public string Destino { get; set; }
        public string Destinatario { get; set; }
        public decimal ValorICMS { get; set; }
    }
}
