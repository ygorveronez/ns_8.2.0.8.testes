using System;
using System.Collections.Generic;
using System.Text;

namespace Dominio.ObjetosDeValor.Embarcador.CTe
{
    public sealed class FiltroPesquisaRelatorioCTeEmitido
    {
        public int CodigoTransportador { get; set; }
        public int CodigoEmpresaPai { get; set; }
        public double CpfCnpjEmbarcador { get; set; }
        public DateTime DataAutorizacaoInicial { get; set; }
        public DateTime DataAutorizacaoFinal { get; set; }
        public DateTime DataEmissaoInicial { get; set; }
        public DateTime DataEmissaoFinal { get; set; }
    }
}
