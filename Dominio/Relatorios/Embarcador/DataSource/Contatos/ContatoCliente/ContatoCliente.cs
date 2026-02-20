using System;

namespace Dominio.Relatorios.Embarcador.DataSource.Contatos.ContatoCliente
{
    public class ContatoCliente
    {
        public int Codigo { get; set; }
        public int Numero { get; set; }
        public string Descricao { get; set; }
        public DateTime DataContato { get; set; }
        public string DataPrevistaRetorno { get; set; }
        public string GrupoPessoas { get; set; }
        public string Pessoa { get; set; }
        public string TipoPessoa { get; set; }
        public double CPFCNPJPessoa { get; set; }
        public string Tipo { get; set; }
        public string Situacao { get; set; }
        public string Documento { get; set; }
        public string Usuario { get; set; }
        public string Contato { get; set; }
        public string CPFCNPJPessoaFormatado
        {
            get
            {
                if (CPFCNPJPessoa > 0d)
                {
                    if (TipoPessoa == "J")
                        return String.Format(@"{0:00\.000\.000\/0000\-00}", this.CPFCNPJPessoa);
                    else
                        return String.Format(@"{0:000\.000\.000\-00}", this.CPFCNPJPessoa);
                }

                return string.Empty;
            }
        }
    }
}
