using System;

namespace Dominio.ObjetosDeValor.Embarcador.Carga
{
    public class FiltroPesquisaRelatorioPacotes
    {
        public string LogKey { get; set; }
        public int CodigoPedido { get; set; }
        public int CodigoCarga { get; set; }
        public int CodigoTipoOperacao { get; set; }
        public double CodigoOrigem { get; set; }
        public double CodigoDestino { get; set; }
        public double CodigoContratante { get; set; }
        public DateTime DataRecebimentoInicial { get; set; }
        public DateTime DataRecebimentoFinal { get; set; }
        public decimal Cubagem { get; set; }
        public decimal Peso { get; set; }
        public int NumeroCTe { get; set; }
        public string ChaveCTe { get; set; }


    }
}
