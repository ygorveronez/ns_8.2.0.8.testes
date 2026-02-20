using System;

namespace Servicos.Embarcador.Configuracoes
{
    public sealed class ConfigurationInstance
    {
        //private static ConfigurationInstance _instancia;
        private static readonly Lazy<ConfigurationInstance> _instancia = new Lazy<ConfigurationInstance>(() => new ConfigurationInstance());


        private Dominio.ObjetosDeValor.Embarcador.Configuracoes.ConfiguracaoArquivo _configuracaoArquivo;
        private Dominio.ObjetosDeValor.Embarcador.Configuracoes.ConfiguracaoAmbiente _configuracaoAmbiente;


        private ConfigurationInstance() { }

        public static ConfigurationInstance GetInstance(Repositorio.UnitOfWork unitOfWork = null)
        {
            if (_instancia.Value._configuracaoArquivo == null && unitOfWork != null)
                _instancia.Value.CarregarConfiguracoes(unitOfWork);

            // Retornar a instância singleton
            return _instancia.Value;
        }

        public void AtualizarConfiguracoes(Repositorio.UnitOfWork unitOfWork)
        {
            CarregarConfiguracoes(unitOfWork);
        }

        public string ObterCaminhoArquivos()
        {
            return ObterConfiguracaoArquivo().CaminhoArquivos ?? throw new Dominio.Excecoes.Embarcador.ServicoException("O caminho de arquivos não foi informado. Contate o Suporte.");
        }

        public Dominio.ObjetosDeValor.Embarcador.Configuracoes.ConfiguracaoArquivo ObterConfiguracaoArquivo()
        {
            return _configuracaoArquivo ?? new Dominio.ObjetosDeValor.Embarcador.Configuracoes.ConfiguracaoArquivo();
        }

        public Dominio.ObjetosDeValor.Embarcador.Configuracoes.ConfiguracaoAmbiente ObterConfiguracaoAmbiente()
        {
            return _configuracaoAmbiente ?? new Dominio.ObjetosDeValor.Embarcador.Configuracoes.ConfiguracaoAmbiente();
        }

        private void CarregarConfiguracoes(Repositorio.UnitOfWork unitOfWork)
        {
#if DEBUG

            _configuracaoArquivo = new Repositorio.Embarcador.Configuracoes.ConfiguracaoArquivo(unitOfWork).BuscarConfiguracaoDebugLocal();
            _configuracaoAmbiente = new Repositorio.Embarcador.Configuracoes.ConfiguracaoAmbiente(unitOfWork).BuscarConfiguracaoDebugLocal();
#else
            _configuracaoArquivo = new Repositorio.Embarcador.Configuracoes.ConfiguracaoArquivo(unitOfWork).BuscarConfiguracaoPadrao();
            _configuracaoAmbiente = new Repositorio.Embarcador.Configuracoes.ConfiguracaoAmbiente(unitOfWork).BuscarConfiguracaoPadrao();
#endif
        }
    }
}
