using System;

namespace Dominio.ObjetosDeValor.Embarcador.CTe
{
    public sealed class FiltroPesquisaAutorizacaoIntegracaoCTe
    {
        public string CodigoCargaEmbarcador { get; set; }

        public int CodigoFilial { get; set; }

        public int CodigoUsuario { get; set; }

        public int CodigoTipoOperacao { get; set; }

        public DateTime? DataInicio { get; set; }

        public DateTime? DataLimite { get; set; }

        public Enumeradores.SituacaoAutorizacaoIntegracaoCTe? SituacaoAutorizacaoIntegracaoCTe { get; set; }
    }
}
