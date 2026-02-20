namespace Dominio.ObjetosDeValor.Embarcador.Logistica
{
    public class Fronteira
    {
        public double CPF_CNPJ { get; set; }
        public Dominio.ObjetosDeValor.Localidade Localidade { get; set; }
        public string Descricao { get; set; }
        public string CodigoFronteiraEmbarcador { get; set; }
        public string CodigoIntegracao { get; set; }
        public Fronteira FronteiraOutroLado { get; set; }
    }
}
