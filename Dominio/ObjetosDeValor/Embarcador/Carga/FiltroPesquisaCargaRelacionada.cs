using Dominio.ObjetosDeValor.Embarcador.Enumeradores;
using System;

namespace Dominio.ObjetosDeValor.Embarcador.Carga
{
    public sealed class FiltroPesquisaCargaRelacionada
    {
        public string NumeroCarga { get; set; }
        public DateTime? DataInicio { get; set; }
        public DateTime? DataLimite { get; set; }
        public int CodigoFilial { get; set; }
        public SituacaoCarga? Situacao { get; set; }
        public int CodigoTipoDeCarga { get; set; }
        public int CodigoTipoOperacao { get; set; }
        public int CodigoCanalEntrega { get; set; }
        public int CodigoTransportador { get; set; }
        public RelacionamentoCarga? Relacionada { get; set; }
    }
}
