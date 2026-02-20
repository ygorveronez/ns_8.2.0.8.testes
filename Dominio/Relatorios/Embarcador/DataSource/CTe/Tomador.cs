using System;

namespace Dominio.Relatorios.Embarcador.DataSource.CTe
{
    public class Tomador
    {
        public int Codigo { get; set; }
        public double CPFCNPJ { get; set; }
        public string InscricaoEstadual { get; set; }
        public string RazaoSocial { get; set; }
        public string NomeFantasia { get; set; }
        public string Telefone { get; set; }
        public string CEP { get; set; }
        public string Endereco { get; set; }
        public string Bairro { get; set; }
        public string Complemento { get; set; }
        public string Numero { get; set; }
        public string Cidade { get; set; }
        public string Estado { get; set; }
        public DateTime DataCadastro { get; set; }
        public string Email { get; set; }
        public string SituacaoEmail { get; set; }
        public string GrupoPessoas { get; set; }
        public string EmailGrupo { get; set; }
        public string SituacaoEmailGrupo { get; set; }
        public string TipoPessoa { get; set; }
        public string CPFCNPJFormatado
        {
            get
            {
                if (this.TipoPessoa == "E")
                {
                    return "00.000.000/0000-00";
                }
                else
                {
                    return this.TipoPessoa == "J" ? string.Format(@"{0:00\.000\.000\/0000\-00}", this.CPFCNPJ) : String.Format(@"{0:000\.000\.000\-00}", this.CPFCNPJ);
                }
            }
        }

    }
}
