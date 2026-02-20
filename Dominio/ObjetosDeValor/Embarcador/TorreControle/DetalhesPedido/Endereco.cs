namespace Dominio.ObjetosDeValor.Embarcador.TorreControle.DetalhesPedido
{
    public class Endereco
    {
        public string Logradouro { get; set; }

        public string Bairro { get; set; }

        public string Numero { get; set; }

        public string CEP { get; set; }

        public Localidade Localidade { get; set; }

        public string EnderecoCompleto
        {
            get
            {
                return $"{Logradouro}, {Numero} - {Bairro} - {Localidade.Descricao} - {Localidade.Estado.Sigla} - {CEP}";
            }
        }
    }
}
