using System;

namespace Dominio.ObjetosDeValor.Embarcador.Financeiro
{
    public class MovimentoConciliacaoBancaria
    {
        public int Codigo { get; set; }
        public int CodigoPlano { get; set; }
        public DateTime DataMovimento { get; set; }
        public decimal ValorDebito { get; set; }
        public decimal ValorCredito { get; set; }
        public string Documento { get; set; }
        public string Cheques { get; set; }
        public string Observacao { get; set; }
        public ObjetosDeValor.Embarcador.Enumeradores.TipoDocumentoMovimento Tipo { get; set; }
        public string Pessoa { get; set; }
        public bool MovimentoConcolidado { get; set; }
        public virtual string DescricaoDataMovimento
        {
            get
            {
                return this.DataMovimento > DateTime.MinValue ? this.DataMovimento.ToString("dd/MM/yyyy") : "";
            }
        }
        public virtual string DescricaoTipoDocumentoMovimento
        {
            get
            {
                switch (this.Tipo)
                {
                    case ObjetosDeValor.Embarcador.Enumeradores.TipoDocumentoMovimento.CTe:
                        return "Documento Emitido";
                    case ObjetosDeValor.Embarcador.Enumeradores.TipoDocumentoMovimento.Manual:
                        return "Manual";
                    case ObjetosDeValor.Embarcador.Enumeradores.TipoDocumentoMovimento.NotaEntrada:
                        return "Nota de Entrada";
                    case ObjetosDeValor.Embarcador.Enumeradores.TipoDocumentoMovimento.NotaSaida:
                        return "Nota de Saída";
                    case ObjetosDeValor.Embarcador.Enumeradores.TipoDocumentoMovimento.Outros:
                        return "Outros";
                    case ObjetosDeValor.Embarcador.Enumeradores.TipoDocumentoMovimento.Pagamento:
                        return "Pagamento";
                    case ObjetosDeValor.Embarcador.Enumeradores.TipoDocumentoMovimento.Recebimento:
                        return "Recebimento";
                    case ObjetosDeValor.Embarcador.Enumeradores.TipoDocumentoMovimento.Recibo:
                        return "Recibo";
                    case ObjetosDeValor.Embarcador.Enumeradores.TipoDocumentoMovimento.Faturamento:
                        return "Faturamento";
                    case ObjetosDeValor.Embarcador.Enumeradores.TipoDocumentoMovimento.Acerto:
                        return "Acerto de Viagem";
                    case ObjetosDeValor.Embarcador.Enumeradores.TipoDocumentoMovimento.ContratoFrete:
                        return "Contrato de Frete";
                    case ObjetosDeValor.Embarcador.Enumeradores.TipoDocumentoMovimento.AdiantamentoMotorista:
                        return "Adiantamento Motorista";
                    default:
                        return "";
                }
            }
        }
    }
}
