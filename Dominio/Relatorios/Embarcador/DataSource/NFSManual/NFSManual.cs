namespace Dominio.Relatorios.Embarcador.DataSource.NFSManual
{
    public class NFSManual
    {
        public string CNPJEmpresa { get; set; }
        public string CNPJEmpresaFormatado
        {
            get
            {
                return string.Format(@"{0:00\.000\.000\/0000\-00}", long.Parse(CNPJEmpresa));
            }
        }
        public string Empresa { get; set; }
        public string Tomador { get; set; }
        public double CPFCNPJTomador { get; set; }
        public string TipoPessoaTomador { get; set; }
        public string CPFCNPJTomadorFormatado
        {
            get
            {
                if (TipoPessoaTomador == "F")
                    return string.Format(@"{0:000\.000\.000\-00}", this.CPFCNPJTomador);
                else
                    return string.Format(@"{0:00\.000\.000\/0000\-00}", this.CPFCNPJTomador);
            }
        }
    }
}
