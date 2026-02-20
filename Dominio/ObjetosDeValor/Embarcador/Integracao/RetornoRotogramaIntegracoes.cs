using Dominio.Enumeradores;

namespace Dominio.ObjetosDeValor.Embarcador.Integracao
{
    public class RetornoRotogramaIntegracoes
    {
        public bool status { get; set; }
        public string mensagemErro { get; set; }
        public TipoIntegracao TipoIntegracao { get; set; }
    }
}
