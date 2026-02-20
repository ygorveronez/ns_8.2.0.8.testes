using System;

namespace Dominio.ObjetosDeValor.Embarcador.GestaoPatio
{
    public sealed class FiltroPesquisaCheckListVigencia
    {
        public int CodigoFilial { get; set; }
        public int CodigoTipoOperacao { get; set; }
        public DateTime? DataFinal { get; set; }
        public DateTime? DataInicial { get; set; }
        public bool? Ativo { get; set; }
    }
}
