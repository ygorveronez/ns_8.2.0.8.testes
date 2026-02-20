using System;
using System.Collections.Generic;

namespace Dominio.ObjetosDeValor.Embarcador.Carga.Redespacho
{
    public class FiltroPesquisaRedespacho
    {
        public int NumeroRedespacho { get; set; }

        public int CodigoCarga { get; set; }

        public DateTime? DataInicio { get; set; }

        public DateTime? DataFim { get; set; }

        public double CodigoExpedidor { get; set; }

        public int CodigoTipoOperacao { get; set; }

        public List<double> CodigosRecebedores { get; set; }

        public List<int> CodigosFilial { get; set; }
    }
}
