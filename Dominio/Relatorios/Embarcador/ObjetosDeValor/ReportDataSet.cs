using System.Collections.Generic;

namespace Dominio.Relatorios.Embarcador.ObjetosDeValor
{
    public class ReportDataSet
    {
        public string Key { get; set; }
        public dynamic DataSet { get; set; }
        public List<ReportDataSet> SubReports { get; set; }

        public List<Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro> Parameters { get; set; }

    }
}
