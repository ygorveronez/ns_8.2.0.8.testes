using System;
using System.Collections.Generic;

namespace Dominio.ObjetosDeValor
{
    public class Cliente : IEquatable<Cliente>
    {
        public double Codigo { get; set; }
        public string CPFCNPJ { get; set; }
        public string CodigoIntegracao { get; set; }
        public int CodigoAtividade { get; set; }
        public string DescricaoAtividade { get; set; }
        public string DataCadastro { get; set; }
        public string NumeroDocumentoExportacao { get; set; }
        public string RGIE { get; set; }
        public string IM { get; set; }
        public string Nome { get; set; }
        public string RazaoSocial { get; set; }
        public string NomeFantasia { get; set; }
        public string Telefone1 { get; set; }
        public string Telefone2 { get; set; }
        public string Endereco { get; set; }
        public string Numero { get; set; }
        public string Bairro { get; set; }
        public string Complemento { get; set; }
        public string CEP { get; set; }
        public string Emails { get; set; }
        public bool StatusEmails { get; set; }
        public string EmailsContato { get; set; }
        public bool StatusEmailsContato { get; set; }
        public string EmailsContador { get; set; }
        public bool StatusEmailsContador { get; set; }
        public string UF { get; set; }
        public int Localidade { get; set; }
        public IEnumerable<object> Cidades { get; set; }
        public bool Exportacao { get; set; }
        public string Cidade { get; set; }
        public int CodigoPais { get; set; }
        public string DescricaoPais { get; set; }
        public string SiglaPais { get; set; }
        public bool SalvarEndereco { get; set; }
        public string EmailsTransportador { get; set; }
        public bool StatusEmailsTransportador { get; set; }
        public string InscricaoSuframa { get; set; }
        public string ClassificacaoPessoaCor { get; set; }
        public double Latitude { get; set; }
        public double Longitude { get; set; }
        public Embarcador.Enumeradores.TipoArea TipoArea { get; set; }
        public int Raio { get; set; }
        public string Area { get; set; }
        public Dominio.ObjetosDeValor.Embarcador.Logistica.SubareaCliente[] Subareas { get; set; }
        public bool PontoTransbordo { get; set; }
        public string Tipo { get; set; }
        public string Regiao { get; set; }
        public string Mesorregiao { get; set; }
        public int CodigoRegiao { get; set; }


        public bool Equals(Cliente other)
        {
            return (other != null || Codigo == other.Codigo);
        }

        public virtual string Descricao
        {
            get
            {
                string descricao = "";
                string nome = this.Nome;

                if (this.PontoTransbordo)
                    nome = this.NomeFantasia;

                if (!string.IsNullOrWhiteSpace(this.CodigoIntegracao))
                    descricao += this.CodigoIntegracao + " - ";

                if (!string.IsNullOrWhiteSpace(nome))
                    descricao += nome;

                if (!string.IsNullOrWhiteSpace(this.Tipo))
                    descricao += " (" + this.Codigo.ToString().ObterCpfOuCnpjFormatado(this.Tipo) + ")";

                return descricao;
            }
        }

        public virtual string EnderecoCompleto
        {
            get
            {
                List<string> dadosEndereco = new List<string>();

                if (!string.IsNullOrWhiteSpace(Endereco))
                    dadosEndereco.Add(Endereco);

                if (!string.IsNullOrWhiteSpace(Bairro))
                    dadosEndereco.Add(Bairro);

                if (!string.IsNullOrWhiteSpace(Numero))
                    dadosEndereco.Add(Numero);

                return string.Join(", ", dadosEndereco);
            }
        }
    }
}
