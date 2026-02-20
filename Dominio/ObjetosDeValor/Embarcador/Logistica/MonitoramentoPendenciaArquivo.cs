using System.Collections.Generic;

namespace Dominio.ObjetosDeValor.Embarcador.Logistica
{
    public class MonitoramentoPendenciaArquivo
    {
        public string NomeArquivo { get; set; }
        public string CaminhoArquivo { get; set; }
        public List<MonitoramentoPendencia> Pendencias { get; set; }
    }
}
