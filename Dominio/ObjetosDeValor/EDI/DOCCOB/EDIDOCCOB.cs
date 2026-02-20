using System;

namespace Dominio.ObjetosDeValor.EDI.DOCCOB
{
    public class EDIDOCCOB
    {
        public string Remetente { get; set; }
        public string Destinatario { get; set; }
        public string DestinatarioCodigoCompanhia { get; set; }
        public string RemetenteCNPJ { get; set; }
        public string TomadorCNPJ { get; set; }
        public DateTime Data { get; set; }
        public string Intercambio { get; set; }
        public string Filler { get; set; }
        public CabecalhoDocumento CabecalhoDocumento { get; set; }
        public int SequenciaGeracaoArquivo { get; set; }
        public Dominio.ObjetosDeValor.Embarcador.Pessoas.Pessoa RemetenteDetalhes { get; set; }
        public Dominio.ObjetosDeValor.Embarcador.Pessoas.Pessoa DestinatarioDetalhes { get; set; }
    }
}
