namespace Dominio.ObjetosDeValor.Relatorios
{
    public class RelatorioMDFeAgrupado
    {
        public string CNPJEmpresa { get; set; }
        public string Empresa { get; set; }
        public int CodigoEmpresa { get; set; }
        public int CountMDFes { get; set; }
        public Dominio.Enumeradores.StatusMDFe Status { get; set; }
        public string CNPJEmpresaPai { get; set; }
        public string EmpresaPai { get; set; }
        public int CodigoEmpresaPai { get; set; }
    }
}
