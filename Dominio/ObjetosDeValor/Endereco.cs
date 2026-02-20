namespace Dominio.ObjetosDeValor
{
    public class Endereco
    {
        public virtual Entidades.Localidade Cidade { get; set; }

        public virtual string Logradouro { get; set; }

        public virtual string Numero { get; set; }

        public virtual string Complemento { get; set; }

        public virtual string CEP { get; set; }

        public virtual string Bairro { get; set; }

        public virtual string Telefone { get; set; }

        public virtual string CodigoEnderecoEmbarcador { get; set; }

        public virtual string CodigoIntegracao { get; set; }

        public virtual string CPFCNPJ { get; set; }

        public virtual string NomeFantasia { get; set; }

        public virtual string Latitude { get; set; }

        public virtual string Longitude { get; set; }
    }
}
