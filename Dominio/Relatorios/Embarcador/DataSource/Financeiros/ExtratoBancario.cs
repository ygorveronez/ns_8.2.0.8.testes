using System;

namespace Dominio.Relatorios.Embarcador.DataSource.Financeiros
{
    public class ExtratoBancario
    {
        public int Codigo { get; set; }
        public DateTime DataMovimento { get; set; }
        public string Observacao { get; set; }
        public Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoDocumentoMovimento TipoDocumentoMovimento { get; set; }
        public string Documento { get; set; }
        public string CodigoLancamento { get; set; }
        public string PlanoConta { get; set; }
        public string PlanoContaDescricao { get; set; }
        public decimal ValorDebito { get; set; }
        public decimal ValorCredito { get; set; }
        public decimal Saldo { get; set; }
        public decimal SaldoAnterior { get; set; }
        public int CodigoPlanoConta { get; set; }

        public string DescricaoDataMovimento
        {
            get { return DataMovimento != DateTime.MinValue ? DataMovimento.ToString("dd/MM/yyyy") : string.Empty; }
        }

        public string DescricaoTipoDocumentoMovimento
        {
            get
            {
                switch (this.TipoDocumentoMovimento)
                {
                    case Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoDocumentoMovimento.CTe:
                        return "Documento Emitido";
                    case Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoDocumentoMovimento.Manual:
                        return "Manual";
                    case Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoDocumentoMovimento.NotaEntrada:
                        return "Nota de Entrada";
                    case Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoDocumentoMovimento.NotaSaida:
                        return "Nota de Saída";
                    case Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoDocumentoMovimento.Outros:
                        return "Outros";
                    case Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoDocumentoMovimento.Pagamento:
                        return "Pagamento";
                    case Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoDocumentoMovimento.Recebimento:
                        return "Recebimento";
                    case Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoDocumentoMovimento.Recibo:
                        return "Recibo";
                    case Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoDocumentoMovimento.Faturamento:
                        return "Faturamento";
                    case Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoDocumentoMovimento.Acerto:
                        return "Acerto de Viagem";
                    case Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoDocumentoMovimento.ContratoFrete:
                        return "Contrato de Frete";
                    case Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoDocumentoMovimento.AdiantamentoMotorista:
                        return "Adiantamento Motorista";
                    default:
                        return "";
                }
            }
        }
    }
}
