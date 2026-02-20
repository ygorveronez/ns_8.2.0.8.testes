namespace Dominio.ObjetosDeValor.WebService.Rest.ModeloDados
{
    public class Endereco
    {
        public string Logradouro { get; set; }

        public string Bairro { get; set; }

        public string Numero { get; set; }

        public string Latitude { get; set; }

        public string Longitude { get; set; }

        public Localidade Localidade { get; set; }
    }
}
