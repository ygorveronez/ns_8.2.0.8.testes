namespace Dominio.ObjetosDeValor.LsTranslog
{
    public class Atividade
    {
        public string Identificador { get; set; }
        public string OS { get; set; }
        public string Observacoes { get; set; }
        public string IDTipo { get; set; }
        //public string IDPolo { get; set; }
        public string Rastreio { get; set; }
        public string NotaFiscal { get; set; }
        public string Datalimite { get; set; }
        public string NumeroCTE { get; set; }
        public string NumeroDT { get; set; }
        public string Lote { get; set; }
        public string Peso { get; set; }
        public string CNPJEmissor { get; set; }
        public string ChaveNFE { get; set; }
        public Cliente Cliente { get; set; }
        public decimal Valor { get; set; }
        public int IDLocal { get; set; }
        public string Tipo { get; set; }
    }
}

