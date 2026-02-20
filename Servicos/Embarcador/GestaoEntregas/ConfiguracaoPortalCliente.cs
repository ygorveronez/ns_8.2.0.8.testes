namespace Servicos.Embarcador.GestaoEntregas
{
    public class ConfiguracaoPortalCliente
    {
        public static Dominio.Entidades.Embarcador.GestaoEntregas.ConfiguracaoPortalCliente ObterConfiguracao(Repositorio.UnitOfWork unitOfWork)
        {
            Repositorio.Embarcador.GestaoEntregas.ConfiguracaoPortalCliente repConfiguracaoPortalCliente = new Repositorio.Embarcador.GestaoEntregas.ConfiguracaoPortalCliente(unitOfWork);
            Dominio.Entidades.Embarcador.GestaoEntregas.ConfiguracaoPortalCliente configuracao = repConfiguracaoPortalCliente.BuscarConfiguracao();

            if (configuracao == null)
            {
                configuracao = new Dominio.Entidades.Embarcador.GestaoEntregas.ConfiguracaoPortalCliente();

                repConfiguracaoPortalCliente.Inserir(configuracao);
            }

            return configuracao;
        }

    }
}
