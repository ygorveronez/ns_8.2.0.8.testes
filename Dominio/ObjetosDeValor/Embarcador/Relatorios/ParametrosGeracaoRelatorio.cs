using System.Collections.Generic;

namespace Dominio.ObjetosDeValor.Embarcador.Relatorios
{
    public class ParametrosGeracaoRelatorio
    {
        public Dominio.ObjetosDeValor.Embarcador.Relatorios.ConfiguracaoRelatorio ConfiguracaoRelatorio { get; set; }
        public List<dynamic> ListaRegistros { get; set; }
        public dynamic FiltrosPesquisa { get; set; }
        public string CaminhoRelatorio { get; set; }
        public Dominio.ObjetosDeValor.Embarcador.Consulta.ParametroConsulta ParametrosConsulta { get; set; }
        public List<Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro> Parametros { get; set; }
        public List<KeyValuePair<string, dynamic>> SubReportDataSources { get; set; }
    }
}
