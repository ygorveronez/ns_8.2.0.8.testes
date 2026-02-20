using System.Collections.Generic;

namespace Dominio.ObjetosDeValor.EDI.Notfis
{
    public class CabecalhoDocumento
    {
        public string IdDocumento { get; set; }
        public string IdCTe { get; set; }
        public string Filler { get; set; }
        public List<Embarcador> Embarcadores { get; set; }
        public Totais Totais { get; set; }
        public decimal Distancia { get; set; }
        public string NumeroRomaneio { get; set; }
        public Embarcador Embarcador { get; set; }
        public string DataEmbarque { get; set; }
        public string HoraEmbarque { get; set; }
        public Empresa Empresa { get; set; }
        public string CodigoRemessa { get; set; }
        public Veiculo Veiculo { get; set; }
        public Motorista Motorista { get; set; }
        public string TipoViagem { get; set; }
        public string TipoOperacao { get; set; }
        public Dominio.ObjetosDeValor.EDI.Notfis.ComplementoNotaFiscal ComplementoNotaFiscal { get; set; }
        public List<NotaFiscal> NotasFiscais { get; set; }
        public List<Destinatario> Destinatarios { get; set; }

    }
}
