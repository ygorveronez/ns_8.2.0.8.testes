using System;

namespace Dominio.ObjetosDeValor.Embarcador.GestaoPatio
{
   public sealed class FluxoCaixaInformarCarga
    {
        public int CodigoCarga { get; set; }

        public int CodigoFilaCarregamento { get; set; }

        public int CodigoModeloVeicular { get; set; }

        public int CodigoMotivoSelecaoMotoristaForaOrdem { get; set; }

        public DateTime DataCarregamento { get; set; }

        public string Doca { get; set; }

        public string Lacre { get; set; }

        public bool PrimeiroNaFila { get; set; }
    }
}
