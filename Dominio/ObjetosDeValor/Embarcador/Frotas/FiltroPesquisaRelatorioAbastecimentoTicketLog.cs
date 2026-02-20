using System;

namespace Dominio.ObjetosDeValor.Embarcador.Frotas
{
    public class FiltroPesquisaRelatorioAbastecimentoTicketLog
    {
        public DateTime DataInicial { get; set; }
        public DateTime DataFinal { get; set; }
        public int CodigoVeiculo { get; set; }
        public double CNPJFornecedor { get; set; }
        public int CodigoTransacao { get; set; }
    }
}
