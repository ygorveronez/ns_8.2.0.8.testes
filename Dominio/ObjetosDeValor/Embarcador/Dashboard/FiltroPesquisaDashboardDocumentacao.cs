using System;
using System.Collections.Generic;

namespace Dominio.ObjetosDeValor.Embarcador.Dashboard
{
    public class FiltroPesquisaDashboardDocumentacao
    {
        public DateTime? DataInicial { get; set; }
        public DateTime? DataFinal { get; set; }
        public string Regiao { get; set; }
        public List<string> Embarques { get; set; }
        public bool? NavioAberto { get; set; }
        public bool? NavioFechado { get; set; }
    }
}