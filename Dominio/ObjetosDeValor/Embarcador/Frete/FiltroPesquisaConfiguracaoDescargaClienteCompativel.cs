using System.Collections.Generic;

namespace Dominio.ObjetosDeValor.Embarcador.Frete
{
    public sealed class FiltroPesquisaConfiguracaoDescargaClienteCompativel
    {
        public int CodigoConfiguracaoDescargaClienteDesconsiderar { get; set; }

        public int CodigoFilial { get; set; }

        public int CodigoModeloVeicularCarga { get; set; }
        
        public int CodigoTipoCarga { get; set; }
        
        public List<int> CodigosGruposClientes { get; set; }
        
        public List<int> CodigosTiposOperacao { get; set; }

        public List<double> CpfCnpjClientes { get; set; }

        public bool SomenteComVigenciaInformada { get; set; }

        public List<int> CodigosTransportadores { get; set; }
    }
}
