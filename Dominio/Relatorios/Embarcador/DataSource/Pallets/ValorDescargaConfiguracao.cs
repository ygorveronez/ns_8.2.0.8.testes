using System;
using System.Collections.Generic;

namespace Dominio.Relatorios.Embarcador.DataSource.Pallets
{
    public class ValorDescargaConfiguracao
    {
        public virtual List<double> CPFCNPJs { get; set; }
        public virtual string Valor { get; set; }
        public virtual string Filial { get; set; }
        public virtual string TipoOperacao { get; set; }
        public virtual string ModeloVeicular { get; set; }

    }
}
