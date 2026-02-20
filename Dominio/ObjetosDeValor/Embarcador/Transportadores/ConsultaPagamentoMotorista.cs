using System;

namespace Dominio.ObjetosDeValor.Embarcador.Transportadores
{
    public class ConsultaPagamentoMotorista
    {
        public int Codigo { get; set; }
        public int CodigoTipoPagamentoMotorista { get; set; }
        public int CodigoContaDebito { get; set; }
        public int CodigoContaCredito { get; set; }
        public string CPF { get; set; }
        public string Nome { get; set; }
        public string Data { get; set; }
        public string TipoDoPagamento { get; set; }
        public string Valor { get; set; }
        public string Observacao { get; set; }
        public string Veiculos { get; set; }

        public virtual string CPF_Formatado
        {
            get
            {
                string cpf = Utilidades.String.OnlyNumbers(CPF);

                long.TryParse(cpf, out long cpfCnpj);

                if (cpf.Length > 11)
                    return String.Format(@"{0:00\.000\.000\/0000\-00}", cpfCnpj);
                else
                    return String.Format(@"{0:000\.000\.000\-00}", cpfCnpj);
            }
        }
    }
}
