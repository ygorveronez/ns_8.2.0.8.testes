namespace Dominio.ObjetosDeValor.Relatorios
{
    public class DetalheFaturaAvonDocumento
    {
        public int NumeroMinuta { get; set; }
        public int NumeroManifesto { get; set; }
        public string Destino { get; set; }
        public string CTes { get; set; }
        public decimal ValorFrete { get; set; }
        public decimal ValorICMS { get; set; }
        public decimal ValorTotal { get; set; }
    }
}
