using System;

namespace Dominio.ObjetosDeValor.Embarcador.Escrituracao
{
    public sealed class FiltroPesquisaLiberacaoEscrituracaoPagamentoCarga
    {
        public string CodigoCargaEmbarcador { get; set; }

        public int CodigoFilial { get; set; }

        public int CodigoUsuario { get; set; }

        public DateTime? DataInicio { get; set; }

        public DateTime? DataLimite { get; set; }

        public Enumeradores.SituacaoLiberacaoEscrituracaoPagamentoCarga? SituacaoLiberacaoEscrituracaoPagamentoCarga { get; set; }
    }
}
