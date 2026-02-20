using System;

namespace Dominio.Relatorios.Embarcador.DataSource.RH
{
    public class ComissaoFuncionario
    {

        private string _CPF;
        public virtual string Numero { get; set; }
        public virtual string DataCarregaementoEmissao { get; set; }
        public virtual string Documentos { get; set; }
        public virtual string Motorista { get; set; }
        public virtual string Destino { get; set; }
        public virtual string Origem { get; set; }
        public virtual decimal ValoFreteLiquido { get; set; }
        public virtual decimal ValorComissao { get; set; }
        public virtual string Periodo { get; set; }

        public virtual string CPF
        {
            get => _CPF;
            set
            {   

                string cpf = Utilidades.String.OnlyNumbers(value);

                long.TryParse(cpf, out long cpfCnpj);

                if (cpf.Length == 14)
                    _CPF = String.Format(@"{0:00\.000\.000\/0000\-00}", cpfCnpj);
                else if(cpf.Length == 11)
                    _CPF = String.Format(@"{0:000\.000\.000\-00}", cpfCnpj);
                else
                    _CPF = value;
                
            }
        }

    }
}
