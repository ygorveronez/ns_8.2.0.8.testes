namespace Servicos.Embarcador.Carga
{
    public class CargaFreteIntegracao
    {
        #region Métodos Públicos
        public void AplicarFalha(Dominio.Entidades.Embarcador.Cargas.CargaFreteIntegracao cargaFreteIntegracao, string mensagem)
        {
            cargaFreteIntegracao.SituacaoIntegracao = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoIntegracao.ProblemaIntegracao;
            cargaFreteIntegracao.ProblemaIntegracao = mensagem;
            cargaFreteIntegracao.Carga.PendenciaIntegracaoFrete = true;
            cargaFreteIntegracao.Carga.AguardandoIntegracaoFrete = false;
        }
        #endregion Métodos Públicos
    }
}
