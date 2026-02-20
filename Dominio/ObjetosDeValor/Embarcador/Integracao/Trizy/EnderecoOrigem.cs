namespace Dominio.ObjetosDeValor.Embarcador.Integracao.Trizy
{
    public class EnderecoOrigem
    {
        public string uf { get; set; }
        public int municipio { get; set; }
        public string bairro { get; set; }
        public string logradouro { get; set; }
        public string numero { get; set; }
        public string complemento { get; set; }
        public double latitude { get; set; }
        public double longitude { get; set; }
    }
}
