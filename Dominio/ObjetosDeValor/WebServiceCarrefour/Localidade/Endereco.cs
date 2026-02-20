namespace Dominio.ObjetosDeValor.WebServiceCarrefour.Localidade
{
    public sealed class Endereco
    {
        public Localidade Cidade { get; set; }

        public string Logradouro { get; set; }

        public string Numero { get; set; }

        public string Complemento { get; set; }

        public string CEP { get; set; }

        public string CEPSemFormato { get; set; }

        public string Bairro { get; set; }

        public string DDDTelefone { get; set; }

        public string Telefone { get; set; }

        public string Telefone2 { get; set; }

        public string CodigoIntegracao { get; set; }

        public string InscricaoEstadual { get; set; }

        public string Latitude { get; set; }

        public string Longitude { get; set; }

        public string UF { get; set; }
    }
}
