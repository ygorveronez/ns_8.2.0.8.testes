namespace Servicos.Embarcador.Monitoramento.Integracoes.Abstract
{
    public abstract class AbstractIntegracaoDatabase : AbstractIntegracao
    {

        #region Métodos públicos

        public AbstractIntegracaoDatabase(Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoIntegracao tipo, string configSection, AdminMultisoftware.Dominio.Entidades.Pessoas.Cliente cliente) : base(tipo, configSection, cliente)
        {

        }

        #endregion

    }
}
