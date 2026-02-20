using System;

namespace Dominio.Relatorios.Embarcador.DataSource.Documentos
{
    public class FaturaCIOT
    {
        public DateTime Fechamento { get; set; }

        public DateTime Vencimento { get; set; }

        public string Transportadora { get; set; }

        public long Numero { get; set; }

        public decimal Taxa { get; set; }

        public decimal Tarifa { get; set; }

        public Dominio.ObjetosDeValor.Embarcador.Enumeradores.FaturamentoEFreteTipo Tipo { get; set; }

        public Dominio.ObjetosDeValor.Embarcador.Enumeradores.FaturamentoEFreteStatus Status { get; set; }


        public string FechamentoFormatado
        {
            get
            {
                return this.Fechamento.ToString("dd/MM/yyyy");
            }
        }

        public string VencimentoFormatado
        {
            get
            {
                return this.Vencimento.ToString("dd/MM/yyyy");
            }
        }

        public string DescricaoTipo
        {
            get
            {
                switch (this.Tipo)
                {
                    case Dominio.ObjetosDeValor.Embarcador.Enumeradores.FaturamentoEFreteTipo.Adiantamentos:
                        return "Adiantamentos";
                    case Dominio.ObjetosDeValor.Embarcador.Enumeradores.FaturamentoEFreteTipo.Frota:
                        return "Frota";
                    case Dominio.ObjetosDeValor.Embarcador.Enumeradores.FaturamentoEFreteTipo.PrePago:
                        return "Pré Pago";
                    case Dominio.ObjetosDeValor.Embarcador.Enumeradores.FaturamentoEFreteTipo.Quitacao:
                        return "Quitação";
                    default:
                        return "";
                }
            }
        }

        public string DescricaoStatus
        {
            get
            {
                switch (this.Status)
                {
                    case Dominio.ObjetosDeValor.Embarcador.Enumeradores.FaturamentoEFreteStatus.Aberto:
                        return "Aberto";
                    case Dominio.ObjetosDeValor.Embarcador.Enumeradores.FaturamentoEFreteStatus.Cancelado:
                        return "Cancelado";
                    case Dominio.ObjetosDeValor.Embarcador.Enumeradores.FaturamentoEFreteStatus.Encerrado:
                        return "Encerrado";
                    default:
                        return "";
                }
            }
        }
    }
}
