using System;

namespace Dominio.Relatorios.Embarcador.DataSource.FaturamentoMensal
{
    public class CobrancaMensal
    {
        public DateTime DataVencimento { get; set; }
        public DateTime DataFinalizacao { get; set; }
        public DateTime DataFatura { get; set; }
        public int DiaFatura { get; set; }
        public decimal ValorFatura { get; set; }
        public int CodigoTitulo { get; set; }
        public string Boleto { get; set; }
        public int Nota { get; set; }
        public int NotaServico { get; set; }
        public string Pessoa { get; set; }
        public string GrupoFaturamento { get; set; }
        public string Observacao { get; set; }
        public string DescricaoStatus { get; set; }
    }
}
