namespace Dominio.ObjetosDeValor.Relatorios
{
    public class RelatorioDocumentosEntradaCFOP
    {
        public int CodigoDocumentoEntrada { get; set; }
        
        public int CFOPCodigo { get; set; }

        public string CFOPDescricao { get; set; }

        public decimal ValorTotalItem { get; set; }
    }
}
