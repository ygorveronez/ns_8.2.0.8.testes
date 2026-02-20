namespace Dominio.ObjetosDeValor.Embarcador.Integracao.KMM
{
    public class retornoWebService
    {
        public Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoIntegracao SituacaoIntegracao { get; set; }

        public string ProblemaIntegracao { get; set; }

        public string jsonRequisicao { get; set; }

        public string jsonRetorno { get; set; }

        public int CodigoErro { get; set; }

    }
}