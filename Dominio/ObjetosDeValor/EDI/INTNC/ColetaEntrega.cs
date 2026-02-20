namespace Dominio.ObjetosDeValor.EDI.INTNC
{
    public class ColetaEntrega
    {
        public string NumeroLinha { get; set; }
        public string NumeroMensagemLegal { get; set; }

        public string CPFCNPJColeta { get; set; }
        public string TipoPessoaColeta { get; set; }
        public string CNPJColeta { get; set; }
        public string FilialCNPJColeta { get; set; }
        public string DigitoCNPJColeta { get; set; }
        public string IEColeta { get; set; }
        public string PaisColeta { get; set; }
        public string UFColeta { get; set; }
        public int IBGEColeta { get; set; }

        public string CPFCNPJEntrega { get; set; }
        public string TipoPessoaEntrega { get; set; }
        public string CNPJEntrega { get; set; }
        public string FilialCNPJEntrega { get; set; }
        public string DigitoCNPJEntrega { get; set; }
        public string IEEntrega { get; set; }
        public string PaisEntrega { get; set; }
        public string UFEntrega { get; set; }
        public int IBGEEntrega { get; set; }
    }
}
