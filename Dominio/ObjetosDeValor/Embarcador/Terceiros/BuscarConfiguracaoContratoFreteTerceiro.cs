using System;

namespace Dominio.ObjetosDeValor.Embarcador.Terceiros
{
    public class BuscarConfiguracaoContratoFreteTerceiro
    {
        public decimal PercentualAdiantamentoFreteTerceiro { get; set; }

        public decimal PercentualAbastecimentoFreteTerceiro { get; set; }

        public int DiasVencimentoAdiantamento { get; set; }

        public int DiasVencimentoSaldo { get; set; }

        public bool DataFixaVencimentoSaldo { get; set; }
    }
}