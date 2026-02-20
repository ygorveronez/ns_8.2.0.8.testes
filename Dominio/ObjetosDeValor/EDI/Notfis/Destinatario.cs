using System.Collections.Generic;

namespace Dominio.ObjetosDeValor.EDI.Notfis
{
    public class Destinatario
    {
        public Dominio.ObjetosDeValor.Embarcador.Pessoas.Pessoa Pessoa { get; set; }
        public string Area { get; set; }
        public string IdDestinatario { get; set; }
        public string Filler { get; set; }
        public Recebedor Recebedor { get; set; }
        public NotaFiscal NotaFiscal { get; set; }
        public List<NotaFiscal> NotasFiscais { get; set; }
        public List<Dominio.ObjetosDeValor.Embarcador.NFe.Produtos> Produtos { get; set; }
        public List<Dominio.ObjetosDeValor.Embarcador.CTe.CTe> CTes { get; set; }
        public ResponsavelFrete ResponsavelFrete { get; set; }

        public virtual Destinatario Clonar()
        {
            return (Destinatario)this.MemberwiseClone();
        }
    }
}
