using System;

namespace Dominio.Relatorios.Embarcador.DataSource.Pallets
{
    public class ValorDescarga
    {
        public virtual int Codigo { get; set; }
        public virtual string TipoPessoa { get; set; }
        public virtual double CPFCNPJ { get; set; }
        public virtual string IE { get; set; }
        public virtual string RazaoSocial { get; set; }
        public virtual string Localidade { get; set; }
        public virtual string Estado { get; set; }
        public virtual string Endereco { get; set; }
        public virtual string Bairro { get; set; }
        public virtual string Telefone { get; set; }
        public virtual string CEP { get; set; }
        public virtual decimal ValorDescargaPorPallet { get; set; }
        public virtual string ValorDescargaPorVolume { get; set; }
        public virtual string DescricaoLocalidadeEstado
        {
            get
            {
                return Localidade + " - " + Estado;
            }
        }
        public virtual string CPFCNPJFormatado
        {
            get
            {
                if (TipoPessoa.Equals("E"))
                {
                    return "00.000.000/0000-00";
                }
                else
                {
                    return TipoPessoa.Equals("J") ? String.Format(@"{0:00\.000\.000\/0000\-00}", CPFCNPJ) : String.Format(@"{0:000\.000\.000\-00}", CPFCNPJ);
                }
            }
        }
        public virtual string ValorDescargaConfiguracao { get; set; }
        public virtual string Filial { get; set; }
        public virtual string TipoOperacao { get; set; }
        public virtual string ModeloVeicular { get; set; }

    }
}
