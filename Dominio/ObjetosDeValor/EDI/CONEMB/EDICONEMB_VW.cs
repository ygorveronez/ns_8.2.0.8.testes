using System.Collections.Generic;

namespace Dominio.ObjetosDeValor.EDI.CONEMB.EDICONEMB_VW
{
    public class EDICONEMB_VW
    {
        public List<Cabecalho> Cabecalhos { get; set; }
        public Rodape Rodape { get; set; }
    }

    public partial class Cabecalho
    {
        public string Tipo { get; set; }
        public string CodigoPlanta { get; set; }
        public string NumeroCVA { get; set; }
        public string CodigoTransportador { get; set; }
        public string CodigoFornecedor { get; set; }
        public string Reservado { get; set; }
        public List<Nota> Notas { get; set; }
    }

    public partial class Nota
    {
        public string Tipo { get; set; }
        public string SerieNotaFiscal { get; set; }
        public string NumeroNotaFiscal { get; set; }
        public string Reservado { get; set; }
    }

    public partial class Rodape
    {
        public string Tipo { get; set; }
        public string QtdeCabecalhos { get; set; }
        public string QtdeNotas { get; set; }
    }
}
