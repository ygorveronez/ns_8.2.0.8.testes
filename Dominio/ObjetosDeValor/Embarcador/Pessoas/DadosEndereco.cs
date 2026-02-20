namespace Dominio.ObjetosDeValor.Embarcador.Pessoas
{
    public class DadosEndereco
    {
        public string endereco { get; set; }
        public string numero { get; set; }
        public string cidade { get; set; }
        public string estado { get; set; }
        public string bairro { get; set; }
        public string cep { get; set; }
        public string complemento { get; set; }
        public Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoLogradouro? TipoLogradouro { get; set; }
        public bool SempreGoogle { get; set; }
        public string pais { get; set; }
    }
}
