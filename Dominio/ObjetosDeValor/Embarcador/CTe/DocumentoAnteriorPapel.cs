using System;

namespace Dominio.ObjetosDeValor.Embarcador.CTe
{
    public class DocumentoAnteriorPapel
    {
        public Dominio.ObjetosDeValor.Embarcador.Pessoas.Pessoa Emitente { get; set; }
        public string TipoDocumentoTransportaAnteriorPapel { get; set; }
        public DateTime DataEmissao { get; set; }
        public string Serie { get; set; }
        public string Numero { get; set; }
        
    }
}
