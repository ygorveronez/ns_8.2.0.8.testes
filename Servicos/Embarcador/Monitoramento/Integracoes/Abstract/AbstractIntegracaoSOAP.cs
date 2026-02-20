namespace Servicos.Embarcador.Monitoramento.Integracoes.Abstract
{
    public abstract class AbstractIntegracaoSOAP : AbstractIntegracaoWebService
    {

        #region Métodos públicos

        public AbstractIntegracaoSOAP(Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoIntegracao tipo, string configSection, AdminMultisoftware.Dominio.Entidades.Pessoas.Cliente cliente) : base(tipo, configSection, cliente)
        {

        }

        #endregion

    }
}


