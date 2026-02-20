using System;

namespace Dominio.ObjetosDeValor.Embarcador.Logistica
{
    public class FiltroPesquisaMonitoramentoVeiculoAlvo
    {

        public string CodigoCargaEmbarcador { get; set; }

        public string PlacaVeiculo { get; set; }

        public DateTime? DataInicial { get; set; }

        public DateTime? DataFinal { get; set; }

    }
}
