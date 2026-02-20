using System.Collections.Generic;

namespace Dominio.ObjetosDeValor.Embarcador.Financeiro
{
    public class FiltroPesquisaDetalhesCarga
    {
        public List<int> Codigo { get; set; }
        public double Provedor { get; set; }
        public ObjetosDeValor.Embarcador.Enumeradores.TipoDocumentoProvedor DocumentoProvedor { get; set; }
        public int EmpresaTomador { get; set; }
        public int Carga { get; set; }
        public List<int> Localidade { get; set; }
        public decimal ValorTotalPrestacao { get; set; }
        public AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware TipoServicoMultisoftware { get; set; }
        public double CodigoProvedor { get; set; }
    }
}
