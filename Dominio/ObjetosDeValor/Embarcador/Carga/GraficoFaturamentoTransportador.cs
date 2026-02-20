using System;

namespace Dominio.ObjetosDeValor.Embarcador.Carga
{
    public class GraficoFaturamentoTransportador
    {
        public int QuantidadeCargas { get; set; }
        public decimal ValorFrete { get; set; }
        public string Transportador { get; set; }
        public string CNPJTransportador { get; set; }
        public string CNPJ_Formatado
        {
            get
            {
                return String.Format(@"{0:00\.000\.000\/0000\-00}", long.Parse(this.CNPJTransportador));
            }
        }
    }
}
