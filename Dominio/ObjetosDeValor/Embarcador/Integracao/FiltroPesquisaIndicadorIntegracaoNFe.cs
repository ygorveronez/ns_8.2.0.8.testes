using Dominio.ObjetosDeValor.Embarcador.Enumeradores;
using System;

namespace Dominio.ObjetosDeValor.Embarcador.Integracao
{
    public sealed class FiltroPesquisaIndicadorIntegracaoNFe
    {
        public int CodigoFilial { get; set; }

        public DateTime? DataInicio { get; set; }

        public DateTime? DataLimite { get; set; }

        public SituacaoIndicadorIntegracaoNFe? Situacao { get; set; }

        public TipoIndicadorIntegracaoNFe? Tipo { get; set; }
    }
}
