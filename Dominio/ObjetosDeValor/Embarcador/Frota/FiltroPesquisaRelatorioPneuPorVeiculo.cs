using System.Collections.Generic;

namespace Dominio.ObjetosDeValor.Embarcador.Frota
{
    public sealed class FiltroPesquisaRelatorioPneuPorVeiculo
    {
        public int CodigoVeiculo { get; set; }
        public bool CodigoMostrarSomentePosicoesVazias { get; set; }
        public int CodigoMotorista { get; set; }
        public int CodigoModeloVeicular { get; set; }
        public int CodigoMarcaPneu { get; set; }
        public int CodigoModeloPneu { get; set; }
        public int CodigoReboque { get; set; }
        public List<int> CodigoSegmento { get; set; }
        public List<int> CodigoCentroResultado { get; set; }
   }
}
