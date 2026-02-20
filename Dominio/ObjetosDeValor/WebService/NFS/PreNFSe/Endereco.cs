namespace Dominio.ObjetosDeValor.WebService.NFS.PreNFSe
{
    public class Endereco
    {
        public string Logradouro { get; set; }

        public string Numero { get; set; }

        public string Complemento { get; set; }

        public string Bairro { get; set; }

        public string Cep { get; set; }

        public Localidade Localidade { get; set; }
    }
}
