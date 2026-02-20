using System;

namespace Dominio.Relatorios.Embarcador.DataSource.Financeiros
{
    public class DescontoAcrescimoFatura
    {
        public int NumeroFatura { get; set; }
        public DateTime DataInicial { get; set; }
        public DateTime DataFinal { get; set; }
        public decimal TotalAcrescimos { get; set; }
        public decimal TotalDescontos { get; set; }
        public decimal TotalFatura { get; set; }
        public string Observacao { get; set; }
        public string Grupo { get; set; }
        public string Pessoa { get; set; }
        public string Tipo { get; set; }
        public string Justificativa { get; set; }
        public decimal Valor { get; set; }
        public int FAT_SITUACAO { get; set; }
        public int FAT_CODIGO { get; set; }
        public int GRP_CODIGO { get; set; }
        public double CLI_CGCCPF { get; set; }
        public DateTime DataQuitacao { get; set; }
    }
}
