namespace Dominio.ObjetosDeValor.EDI.VGM
{
    public class RodapeUNT
    {
        public string UNT { get; set; }
        public int QuantidadeLinhasArquivo { get; set; }
        public int SequenciaGeracaoArquivo { get; set; }
    }

    public class RodapeUNZ
    {
        public string UNZ { get; set; }
        public int QuantidadeContaineresArquivo { get; set; }
        public int SequenciaGeracaoArquivo { get; set; }
    }
}
