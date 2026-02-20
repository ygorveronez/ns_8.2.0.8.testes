using System.Collections.Generic;

namespace Dominio.ObjetosDeValor
{
    public class CalculoRelacaoCTesEntregues
    {
        public decimal ValorDiaria { get; set; }

        public decimal ValorMeiaDiaria { get; set; }

        public decimal PercentualPorCTe { get; set; }

        public decimal ValorMinimoPorCTe { get; set; }

        public decimal ValorMinimoCTeMesmoDestino { get; set; }

        public decimal FracaoKG { get; set; }

        public decimal ValorPorFracao { get; set; }

        public decimal ValorPorFracaoEmEntregasIguais { get; set; }

        public int FranquiaKM { get; set; }

        public decimal ValorKMExcedente { get; set; }

        public decimal ColetaValorPorEvento { get; set; }

        public decimal ColetaFracao { get; set; }

        public decimal ColetaValorPorFracao { get; set; }

        public List<Dominio.ObjetosDeValor.CalculoRelacaoCTesEntreguesCidade> Cidades { get; set; }
    }
}
