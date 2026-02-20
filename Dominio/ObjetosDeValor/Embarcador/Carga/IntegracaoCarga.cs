using System.Collections.Generic;

namespace Dominio.ObjetosDeValor.Embarcador.Carga
{
    public class IntegracaoCarga
    {
        public int CodigoCarga { get; set; }

        public string CodigoTipoVeiculoEmbarcador { get; set; }

        public string CodigoTipoCargaEmbarcador { get; set; }

        public string CNPJTransportador { get; set; }

        public List<Dominio.ObjetosDeValor.Embarcador.Carga.Motorista> Motoristas { get; set; }

        public string PlacaVeiculo { get; set; }

        public bool Redespacho { get; set; }


    }
}
