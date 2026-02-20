namespace Servicos.Embarcador.Monitoramento.Integracoes.Abstract
{
    public abstract class AbstractIntegracaoWebService : AbstractIntegracao
    {

        #region Métodos públicos

        /**
         * 
         */
        public AbstractIntegracaoWebService(Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoIntegracao tipo, string configSection, AdminMultisoftware.Dominio.Entidades.Pessoas.Cliente cliente) : base(tipo, configSection, cliente)
        {

        }

        #endregion

    }
}


