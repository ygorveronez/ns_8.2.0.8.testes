namespace Servicos.Embarcador.Configuracoes
{
    public static class Ambiente
    {
        public static bool Producao(Repositorio.UnitOfWork unitOfWork = null)
        {
            return ConfigurationInstance.GetInstance(unitOfWork)?.ObterConfiguracaoAmbiente()?.AmbienteProducao ?? false;
        }

        public static bool Seguro(Repositorio.UnitOfWork unitOfWork = null)
        {
            return ConfigurationInstance.GetInstance(unitOfWork).ObterConfiguracaoAmbiente()?.AmbienteSeguro ?? false;
        }
    }
}
