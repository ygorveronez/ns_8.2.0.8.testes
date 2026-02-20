using System;

namespace Dominio.ObjetosDeValor.EDI.VGM
{
    public class CabecalhoUNB
    {
        public string UNB { get; set; }
        public string UNOA { get; set; }
        public string ALI { get; set; }
        public string CodigoTerminalPortuario { get; set; }
        public DateTime DataHoraGeracao { get; set; }
        public int SequenciaGeracaoArquivo { get; set; }
    }

    public class CabecalhoUNH
    {
        public string UNH { get; set; }
        public int SequenciaGeracaoArquivo { get; set; }
        public string VERMAS { get; set; }
    }

    public class CabecalhoBGM
    {
        public string BGM { get; set; }
        public string VGM { get; set; }
        public int SequenciaGeracaoArquivo { get; set; }
        public string TipoOperacao { get; set; }
    }
}
