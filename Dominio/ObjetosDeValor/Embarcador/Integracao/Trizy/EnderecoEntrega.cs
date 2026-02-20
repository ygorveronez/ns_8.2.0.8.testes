namespace Dominio.ObjetosDeValor.Embarcador.Integracao.Trizy
{
    public class EnderecoEntrega
    {
        public string uf { get; set; }
        public int municipio { get; set; }
        public string bairro { get; set; }
        public string logradouro { get; set; }
        public string numero { get; set; }
        public string complemento { get; set; }
        public double latitude { get; set; }
        public double longitude { get; set; }
        public int ordem { get; set; }
        public string instrucao { get; set; }
        public string data { get; set; }
        public int unidade { get; set; }
        public int quantidade { get; set; }
        //public string mercadoria { get; set; }
        public string mercadoria_id { get; set; }
        public decimal valor_extra { get; set; }
        public decimal valor { get; set; }
        public int tipo { get; set; }
    }
}
