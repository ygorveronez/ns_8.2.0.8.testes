using System;

namespace Dominio.ObjetosDeValor.Embarcador.CTe
{
    public class CTeTerceiro
    {
        public int Codigo { get; set; }
        public int Numero { get; set; }
        public string Serie { get; set; }
        public DateTime DataEmissao { get; set; }
        public string CST { get; set; }
        public decimal Peso { get; set; }
        public Dominio.ObjetosDeValor.Cliente Remetente { get; set; }
        public Dominio.ObjetosDeValor.Cliente Destinatario { get; set; }
        public decimal ValorAReceber { get; set; }
        public virtual decimal ValorICMS { get; set; }
        public virtual decimal ValorPrestacaoServico { get; set; }
    }
}
