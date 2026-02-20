using System;

namespace Dominio.Relatorios.Embarcador.DataSource.Financeiros
{
    public class DescontoAcrescimoCTe
    {
        public int NumeroCTe { get; set; }
        public int SerieCTe { get; set; }
        public string Tomador { get; set; }
        public string Grupo { get; set; }
        public DateTime DataEmissaoCTe { get; set; }
        public DateTime DataPagamentoTitulo { get; set; }
        public int NumeroFatura { get; set; }
        public string NumeroTitulo { get; set; }
        public decimal ValorReceber { get; set; }
        public decimal ValorAcrescimo { get; set; }
        public string MotivoAcrescimo { get; set; }
        public decimal ValorDesconto { get; set; }
        public string MotivoDesconto { get; set; }
        public decimal ValorPago { get; set; }
        public string StatusTitulo { get; set; }
    }
}
