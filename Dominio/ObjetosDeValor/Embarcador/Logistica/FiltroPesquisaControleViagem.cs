using System;
using System.Collections.Generic;

namespace Dominio.ObjetosDeValor.Embarcador.Logistica
{
    public class FiltroPesquisaControleViagem
    {
        public DateTime? DataInicialCarga { get; set; }
        public DateTime? DataFinalCarga { get; set; }
        public List<int> CodigosTransportador { get; set; }
        public List<int> CodigosVeiculos { get; set; }
        public List<int> CodigosFilial { get; set; }
        public double CodigoClienteDestino { get; set; }
        public List<int> CodigosStatusViagem { get; set; }
    }
}
