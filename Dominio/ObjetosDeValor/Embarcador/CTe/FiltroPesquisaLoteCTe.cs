using Dominio.ObjetosDeValor.Embarcador.Enumeradores;
using System;

namespace Dominio.ObjetosDeValor.Embarcador.CTe
{
    public sealed class FiltroPesquisaLoteCTe
    {
        public SituacaoDownloadLoteCTe Situacao { get; set; }
        public DateTime? DataSolicitacao { get; set; }
        public DateTime? DataTermino { get; set; }
    }
}
