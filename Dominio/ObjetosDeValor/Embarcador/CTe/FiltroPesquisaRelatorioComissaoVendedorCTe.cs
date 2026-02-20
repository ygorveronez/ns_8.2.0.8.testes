using System;

namespace Dominio.ObjetosDeValor.Embarcador.CTe
{
    public class FiltroPesquisaRelatorioComissaoVendedorCTe
    {
        public DateTime DataEmissaoInicial { get; set; }
        public DateTime DataEmissaoFinal { get; set; }
        public int CodigoGrupoPessoa { get; set; }
        public int CodigoVendedor { get; set; }
    }
}
