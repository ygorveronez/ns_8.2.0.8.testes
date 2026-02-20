using Dominio.Enumeradores;
using System;

namespace Dominio.Relatorios.Embarcador.DataSource.Fretes
{


    public class FreteComponentes
    {
        public string Remetente { get; set; }
        public double RemetenteCGC { get; set; }
        public string RemetenteLocalidade { get; set; }
        public string Destinatario { get; set; }
        public double DestinatarioCGC { get; set; }
        public string DestinatarioLocalidade { get; set; }
        public string Tomador { get; set; }
        public double TomadorCGC { get; set; }
        public string TomadorLocalidade { get; set; }
        public string Expedidor { get; set; }
        public double ExpedidorCGC { get; set; }
        public string ExpedidorLocalidade { get; set; }
        public string Recebedor { get; set; }
        public double RecebedorCGC { get; set; }
        public string RecebedorLocalidade { get; set; }
        public string Transportador { get; set; }
        public string TransportadorCGC { get; set; }
        public string TransportadorLocalidade { get; set; }
        public string Filial { get; set; }
        public decimal ValorFrete { get; set; }
        public decimal ValorICMS { get; set; }
        public int ICMSPagoPorST { get; set; }
        public decimal ValorAReceber { get; set; }
        public decimal ValorDescarga { get; set; }
        public decimal ValorPedagio { get; set; }
        public decimal ValorAdValorem { get; set; }
        public decimal ValorOutros { get; set; }
        public decimal ValorComplementoFrete { get; set; }
        public decimal ValorComplementoICMS { get; set; }
        public decimal ValorComponente1 { get; set; }
        public decimal ValorComponente2 { get; set; }
        public decimal ValorComponente3 { get; set; }
        public decimal ValorComponente4 { get; set; }
        public decimal ValorComponente5 { get; set; }
        public decimal ValorComponente6 { get; set; }
        public decimal ValorComponente7 { get; set; }
        public decimal ValorComponente8 { get; set; }
        public decimal ValorComponente9 { get; set; }
        public decimal ValorComponente10 { get; set; }
        public decimal ValorComponente11 { get; set; }
        public decimal ValorComponente12 { get; set; }
        public decimal ValorComponente13 { get; set; }
        public decimal ValorComponente14 { get; set; }
        public decimal ValorComponente15 { get; set; }
        public TipoPagamento TipoPagamento { get; set; }
        public string Frota { get; set; }
        public string DescricaoTipoPagamento
        {
            get { return TipoPagamento.ObterDescricao(); }
        }
        public virtual string DescricaoPagoPorST
        {
            get
            {
                if (this.ICMSPagoPorST == 1)
                    return "Sim";
                else
                    return "Não";
            }
        }
    }
}
