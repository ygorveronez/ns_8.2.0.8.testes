using System;

namespace Dominio.ObjetosDeValor.Embarcador.CIOT
{
    public class FaturamentoEFreteItem
    {
        public DateTime DataDeclaracao { get; set; }

        public string CIOT { get; set; }

        public string IdOperacaoCliente { get; set; }

        public decimal Adiantamento { get; set; }

        public decimal Livre { get; set; }

        public decimal Estadia { get; set; }

        public decimal Quitacao { get; set; }

        public decimal Frota { get; set; }

        public decimal Servico { get; set; }

        public FaturamentoEFreteItemTransacao Transacao { get; set; }

        public Dominio.ObjetosDeValor.Embarcador.Enumeradores.FaturamentoEFreteItemTipo Tipo { get; set; }
    }
}
