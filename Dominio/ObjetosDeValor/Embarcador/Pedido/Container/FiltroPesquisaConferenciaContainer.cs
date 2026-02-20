using Dominio.ObjetosDeValor.Embarcador.Enumeradores;
using System;

namespace Dominio.ObjetosDeValor.Embarcador.Pedido
{
    public sealed class FiltroPesquisaConferenciaContainer
    {
        public string CodigoCargaEmbarcador { get; set; }

        public DateTime? DataInicial { get; set; }

        public DateTime? DataFinal { get; set; }

        public SituacaoConferenciaContainer? Situacao { get; set; }
    }
}
