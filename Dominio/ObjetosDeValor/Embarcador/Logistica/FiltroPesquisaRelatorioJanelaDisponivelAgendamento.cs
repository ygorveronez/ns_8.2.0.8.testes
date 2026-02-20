using System;

namespace Dominio.ObjetosDeValor.Embarcador.Logistica
{
    public sealed class FiltroPesquisaRelatorioJanelaDisponivelAgendamento
    {
        public DateTime DataInicial { get; set; }
        public DateTime DataFinal { get; set; }
        public int CodigoCentroDescarregamento { get; set; }
        public int CodigoModeloVeicular { get; set; }
    }
}
