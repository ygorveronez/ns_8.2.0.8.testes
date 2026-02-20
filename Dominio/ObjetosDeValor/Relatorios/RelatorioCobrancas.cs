namespace Dominio.ObjetosDeValor.Relatorios
{
    public class RelatorioCobrancas
    {
        // Pai
        public string CNPJEmpresaPai { get; set; }
        public string NomeEmpresaPai { get; set; }
        // Empresa
        public string CNPJEmpresa { get; set; }
        public string NomeEmpresa { get; set; }
        // Periodos
        public string Periodo { get; set; }
        public int CTesEmitidos { get; set; }
        public int MDFesEmitidos { get; set; }
        public int TotalDeDocumentos { get; set; }
        public decimal ValorCobranca { get; set; }
    }
}
