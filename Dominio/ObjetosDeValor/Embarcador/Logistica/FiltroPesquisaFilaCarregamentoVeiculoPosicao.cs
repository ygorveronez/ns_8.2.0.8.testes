using System;

namespace Dominio.ObjetosDeValor.Embarcador.Logistica
{
    public sealed class FiltroPesquisaFilaCarregamentoVeiculoPosicao
    {
        public int CodigoCentroCarregamento { get; set; }

        public int CodigoFilaCarregamentoVeiculoDesconsiderar { get; set; }

        public int CodigoFilial { get; set; }

        public int CodigoModeloVeicularCarga { get; set; }

        public bool ConjuntoVeiculoDedicado { get; set; }

        public DateTime? DataEntrada { get; set; }

        public DateTime? DataProgramada { get; set; }

        public bool DataProgramadaAlteradaAutomaticamente { get; set; }
    }
}
