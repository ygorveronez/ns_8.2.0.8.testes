using Dominio.ObjetosDeValor.Embarcador.Enumeradores;
using System;

namespace Dominio.ObjetosDeValor.Embarcador.GestaoPatio
{
    public sealed class FiltroPesquisaRelatorioCheckList
    {
        public int CodigoFilial { get; set; }
        public DateTime? DataFinal { get; set; }
        public DateTime? DataInicial { get; set; }
        public string Carga { get; set; }
        public SituacaoCheckList? Situacao { get; set; }
    }
}
