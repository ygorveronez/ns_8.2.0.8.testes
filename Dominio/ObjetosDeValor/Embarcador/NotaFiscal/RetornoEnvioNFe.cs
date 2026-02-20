using System;

namespace Dominio.ObjetosDeValor.Embarcador.NotaFiscal
{
    public class RetornoEnvioNFe
    {
        public int codigoNFe { get; set; }
        public string nRec { get; set; }
        public string cStat { get; set; }
        public string xMotivo { get; set; }
        public Dominio.Enumeradores.StatusNFe Status { get; set; }
        public string chNFe { get; set; }
        public string nProt { get; set; }
        public DateTime? dhRecbto { get; set; }
        public string XML { get; set; }
        public Dominio.Enumeradores.TipoArquivoXML TipoArquivoXML { get; set; }
        public string Justificativa { get; set; }
    }
}
