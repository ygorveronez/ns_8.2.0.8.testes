using System;

namespace Dominio.Relatorios.Embarcador.DataSource.NFSManual
{
    public class Documento
    {
        public int Numero { get; set; }
        public string Carga { get; set; }
        public DateTime DataEmissao { get; set; }
        public string Destinatario { get; set; }
        public double CPFCNPJDestinatario { get; set; }
        public string TipoPessoaDestinatario { get; set; }
        public string Localidade { get; set; }
        public string CPFCNPJDestinatarioFormatado
        {
            get
            {
                if (TipoPessoaDestinatario == "F")
                    return string.Format(@"{0:000\.000\.000\-00}", this.CPFCNPJDestinatario);
                else
                    return string.Format(@"{0:00\.000\.000\/0000\-00}", this.CPFCNPJDestinatario);
            }
        }
        public decimal Valor { get; set; }
    }
}
