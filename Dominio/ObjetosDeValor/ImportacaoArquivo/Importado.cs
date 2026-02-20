namespace Dominio.ObjetosDeValor.ImportacaoArquivo
{
    public class Importado
    {
        public string FileName { get; set; }
        public string SalvoComo { get; set; }
        public long ContentLength { get; set; }
        public string ContentType { get; set; }
        public object Content { get; set; }
    }
}
