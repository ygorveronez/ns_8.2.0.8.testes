using Dominio.ObjetosDeValor.Embarcador.Enumeradores;
using System;
using System.Collections.Generic;
using System.Text;

namespace Dominio.ObjetosDeValor.Embarcador.Logistica
{
    public class FiltroPesquisaHistoricoJanelaCarregamento
    {
        public DateTime? DataInicial { get; set; }
        public DateTime? DataFinal { get; set; }
        public int CodigoCentroCarregamento { get; set; }
        public int CodigoMotivoRecusa { get; set; }
        public int CodigoCarga { get; set; }
        public long CodigoClienteTerceiro { get; set; }
    }
}
