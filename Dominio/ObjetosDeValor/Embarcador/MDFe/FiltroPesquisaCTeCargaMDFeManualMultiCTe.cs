using System.Collections.Generic;

namespace Dominio.ObjetosDeValor.Embarcador.MDFe;

public sealed class FiltroPesquisaCTeCargaMDFeManualMultiCTe
{
    public int NumeroNF { get; set; }
    public int CTe { get; set; }
    public int Carga { get; set; }
    public int Empresa { get; set; }
    public List<int> RotasFrete { get; set; }
}
