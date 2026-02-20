using System;

namespace Dominio.ObjetosDeValor.Embarcador.Pallets
{
    public sealed class DadosMovimentacaoEstoquePallet
    {
        public int CodigoAvaria { get; set; }

        public int CodigoFilial { get; set; }

        public int CodigoReforma { get; set; }

        public int CodigoSetor { get; set; }

        public int CodigoTransportador { get; set; }

        public double CpfCnpjCliente { get; set; }

        public Entidades.Embarcador.Pallets.DevolucaoPallet DevolucaoPallet { get; set; }

        public Entidades.Embarcador.Pallets.CancelamentoBaixaPallets CancelamentoBaixaPallets { get; set; }

        public string Observacao { get; set; }

        public int Quantidade { get; set; }

        public int QuantidadeDescartada { get; set; }

        public Enumeradores.TipoLancamento TipoLancamento { get; set; }

        public Enumeradores.TipoOperacaoMovimentacaoEstoquePallet TipoOperacaoMovimentacao { get; set; }

        public AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware? TipoServicoMultisoftware { get; set; }

        public DateTime? DataMovimento { get; set; }

        public int CodigoGrupoPessoas { get; set; }
    }
}
