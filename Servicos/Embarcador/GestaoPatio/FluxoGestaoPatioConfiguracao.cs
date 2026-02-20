namespace Servicos.Embarcador.GestaoPatio
{
    public sealed class FluxoGestaoPatioConfiguracao
    {
        #region Atributos Privados

        private readonly Repositorio.UnitOfWork _unitOfWork;

        #endregion Atributos Privados

        #region Construtores

        public FluxoGestaoPatioConfiguracao(Repositorio.UnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        #endregion Construtores

        #region Métodos Públicos

        public Dominio.Entidades.Embarcador.Filiais.ConfiguracaoGestaoPatio ObterConfiguracao()
        {
            Repositorio.Embarcador.Filiais.ConfiguracaoGestaoPatio repositorioConfiguracaoGestaoPatio = new Repositorio.Embarcador.Filiais.ConfiguracaoGestaoPatio(_unitOfWork);
            Dominio.Entidades.Embarcador.Filiais.ConfiguracaoGestaoPatio configuracao = repositorioConfiguracaoGestaoPatio.BuscarConfiguracao();

            if (configuracao == null)
            {
                configuracao = new Dominio.Entidades.Embarcador.Filiais.ConfiguracaoGestaoPatio();
                repositorioConfiguracaoGestaoPatio.Inserir(configuracao);
            }

            return configuracao;
        }

        #endregion Métodos Públicos
    }
}
