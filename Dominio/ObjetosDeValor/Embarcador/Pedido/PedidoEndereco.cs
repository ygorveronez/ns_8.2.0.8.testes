namespace Dominio.ObjetosDeValor.Embarcador.Pedido
{
    public class PedidoEndereco
    {
        public Dominio.ObjetosDeValor.Localidade Localidade { get; set; }

        public string Logradouro { get; set; }

        public string Numero { get; set; }

        public string Complemento { get; set; }

        public string CEP { get; set; }

        public string Bairro { get; set; }

        public string Telefone { get; set; }

        public string CodigoEnderecoEmbarcador { get; set; }

        public string CPFCNPJ { get; set; }
    }
}
