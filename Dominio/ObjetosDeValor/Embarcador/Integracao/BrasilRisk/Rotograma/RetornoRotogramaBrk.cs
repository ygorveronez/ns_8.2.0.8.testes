namespace Dominio.ObjetosDeValor.Embarcador.Integracao.BrasilRisk.Rotograma
{
    public class RetornoRotogramaBrk : RetornoRotogramaIntegracoes
    {
        public bool status { get; set; }
        public string link { get; set; }
        public string mensagemErro { get; set; }
        public string Base64 { get; set; }
    }
}
