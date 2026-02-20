using System;
using System.Collections.Generic;

namespace Dominio.ObjetosDeValor.Embarcador.Carga
{
    public class FiltroPesquisaRelatorioCargaViagemEventos
    {
        
        public int CodigoFilial { get; set; }

        public double CodigoClienteOrigem { get; set; }

        public double CodigoClienteDestino { get; set; }

        public int CodigoLocalidadeOrigem { get; set; }

        public int CodigoLocalidadeDestino { get; set; }

        public List<int> CodigoCargaEmbarcador { get; set; }

        public DateTime? DataInicial { get; set; }
        public DateTime? DataFinal { get; set; }
    }
}
