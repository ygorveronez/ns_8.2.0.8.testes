using System;

namespace Dominio.ObjetosDeValor.Embarcador.Financeiro
{
    public class GraficoFaturamento
    {
        public int Codigo { get; set; }

        public virtual decimal Valor { get; set; }

        public Int32 Quantidade { get; set; }

        public virtual Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoGraficoFaturamento Tipo { get; set; }

        public virtual string DescricaoTipo
        {
            get
            {
                switch (Tipo)
                {
                    case Enumeradores.TipoGraficoFaturamento.FaturamentoAnual:
                        return "Faturamento";
                    case Enumeradores.TipoGraficoFaturamento.SaldoDisponivel:
                        return "Saldo";
                    case Enumeradores.TipoGraficoFaturamento.ValorAPagar:
                        return "Valor a Pagar";
                    case Enumeradores.TipoGraficoFaturamento.ValorAReceber:
                        return "Valor a Receber";
                    case Enumeradores.TipoGraficoFaturamento.QtdPagar:
                        return "Qtd a Pagar";
                    case Enumeradores.TipoGraficoFaturamento.QtdReceberVencidas:
                        return "Receber Vencidos";
                    default:
                        return string.Empty;
                }
            }
        }

        public virtual string Cor
        {
            get
            {
                switch (Tipo)
                {
                    case Enumeradores.TipoGraficoFaturamento.FaturamentoAnual:
                        return "#e29e23";
                    case Enumeradores.TipoGraficoFaturamento.SaldoDisponivel:
                        return "#1667c6";
                    case Enumeradores.TipoGraficoFaturamento.ValorAPagar:
                        return "#b94747";
                    case Enumeradores.TipoGraficoFaturamento.ValorAReceber:
                        return "#739e73";
                    case Enumeradores.TipoGraficoFaturamento.QtdPagar:
                        return "#66CDAA";
                    case Enumeradores.TipoGraficoFaturamento.QtdReceberVencidas:
                        return "#3d613c";
                    default:
                        return string.Empty;
                }
            }
        }

        public virtual string Icone
        {
            get
            {
                switch (Tipo)
                {
                    case Enumeradores.TipoGraficoFaturamento.FaturamentoAnual:
                        return "fa-money";
                    case Enumeradores.TipoGraficoFaturamento.SaldoDisponivel:
                        return "fa-dollar";
                    case Enumeradores.TipoGraficoFaturamento.ValorAPagar:
                        return "fa-shopping-cart";
                    case Enumeradores.TipoGraficoFaturamento.ValorAReceber:
                        return "fa-barcode";
                    default:
                        return string.Empty;
                }
            }
        }

        public virtual string Imagem
        {
            get
            {
                switch (Tipo)
                {
                    case Enumeradores.TipoGraficoFaturamento.FaturamentoAnual:
                        return "faturamento.png";
                    case Enumeradores.TipoGraficoFaturamento.SaldoDisponivel:
                        return "disponivel.png";
                    case Enumeradores.TipoGraficoFaturamento.ValorAPagar:
                        return "pagar.png";
                    case Enumeradores.TipoGraficoFaturamento.ValorAReceber:
                        return "receber.png";
                    default:
                        return string.Empty;
                }
            }
        }
    }
}
