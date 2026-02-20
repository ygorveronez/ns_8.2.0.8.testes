namespace Servicos.Embarcador.Monitoramento.Integracoes.Abstract
{
    public abstract class AbstractIntegracaoREST : AbstractIntegracaoWebService
    {

        #region Métodos públicos

        public AbstractIntegracaoREST(Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoIntegracao tipo, string configSection, AdminMultisoftware.Dominio.Entidades.Pessoas.Cliente cliente) : base(tipo, configSection, cliente)
        {
            
        }

        #endregion

    }
}


