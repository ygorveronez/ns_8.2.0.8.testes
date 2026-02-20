namespace Dominio.ObjetosDeValor.LsTranslog
{
    public class AtividadeReturn
    {
        public int IDAtividade { get; set; }
        public int IDStatusAtividade { get; set; }
        public string IDPolo { get; set; }
        public string Tipo { get; set; }
        public string Lote { get; set; }
        public string Rastreio { get; set; }
        public string NotaFiscal { get; set; }
        public string Identificador { get; set; }
        public string OS { get; set; }
        public string DataEntrada { get; set; }
        public string CriadaPara { get; set; }
        public string DataInicio { get; set; }
        public string DataTermino { get; set; }
        public string Datalimite { get; set; }
        public string NumeroCTe { get; set; }
        public string NumeroDT { get; set; }
        public string Agente { get; set; }
        public string Status { get; set; }
        public string Polo { get; set; }
        public string Observacoes { get; set; }
        public Cliente Cliente { get; set; }
        //public decimal Valor { get; set; }
    }
}
