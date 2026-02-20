using System;

namespace Dominio.ObjetosDeValor.Embarcador.Integracao
{
    public sealed class FiltroPesquisaIndicadorIntegracaoCTe
    {
        public int CodigoFilial { get; set; }

        public int CodigoTransportador { get; set; }

        public DateTime? DataEmissaoInicio { get; set; }

        public DateTime? DataEmissaoLimite { get; set; }

        public DateTime? DataIntegracaoInicio { get; set; }

        public DateTime? DataIntegracaoLimite { get; set; }

        public bool SomenteIntegrado { get; set; }
    }
}
