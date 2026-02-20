namespace Servicos.Embarcador.Configuracoes
{
    public sealed class ConfiguracaoMonioramentoInstance
    {
        private static ConfiguracaoMonioramentoInstance _instancia;
        
        private Dominio.Entidades.Embarcador.Configuracoes.ConfiguracaoMonitoramentoMonitorarThreads _configuracaoMonitoramentoMonitorarThreads;
        private Dominio.Entidades.Embarcador.Configuracoes.ConfiguracaoMonitoramentoMonitorarPosicoes _configuracaoMonitoramentoMonitorarPosicoes;
        private Dominio.Entidades.Embarcador.Configuracoes.ConfiguracaoMonitoramentoProcessarMonitoramentos _configuracaoMonitoramentoProcessarMonitoramentos;
        private Dominio.Entidades.Embarcador.Configuracoes.ConfiguracaoMonitoramentoProcessarTrocaAlvo _configuracaoMonitoramentoProcessarTrocaAlvos;
        private Dominio.Entidades.Embarcador.Configuracoes.ConfiguracaoMonitoramentoProcessarEventos _configuracaoMonitoramentoProcessarEventos;
        private Dominio.Entidades.Embarcador.Configuracoes.ConfiguracaoMonitoramentoProcessarEventosSinal _configuracaoMonitoramentoProcessarEventosSinal;
        private Dominio.Entidades.Embarcador.Configuracoes.ConfiguracaoMonitoramentoEnviarNotificacoesAlerta _configuracaoMonitoramentoEnviarNotificacoesAlerta;
        private Dominio.Entidades.Embarcador.Configuracoes.ConfiguracaoMonitoramentoImportarPosicoesPendenteIntegracao _configuracaoMonitoramentoImportarPosicoesPendenteIntegracao;        
        

        private ConfiguracaoMonioramentoInstance() { }

        public static ConfiguracaoMonioramentoInstance GetInstance(Repositorio.UnitOfWork unitOfWork = null)
        {
            if (_instancia == null && unitOfWork != null)
            {
                _instancia = new ConfiguracaoMonioramentoInstance();
                _instancia.CarregarConfiguracoes(unitOfWork);
            }

            return _instancia;
        }

        public void AtualizarConfiguracoes(Repositorio.UnitOfWork unitOfWork)
        {
            CarregarConfiguracoes(unitOfWork);
        }

        public Dominio.Entidades.Embarcador.Configuracoes.ConfiguracaoMonitoramentoMonitorarThreads ObterConfiguracaoMonitoramentoMonitorarThreads()
        {
            return _configuracaoMonitoramentoMonitorarThreads ?? new Dominio.Entidades.Embarcador.Configuracoes.ConfiguracaoMonitoramentoMonitorarThreads();
        }

        public Dominio.Entidades.Embarcador.Configuracoes.ConfiguracaoMonitoramentoMonitorarPosicoes ObterConfiguracaoMonitoramentoMonitorarPosicoes()
        {
            return _configuracaoMonitoramentoMonitorarPosicoes ?? new Dominio.Entidades.Embarcador.Configuracoes.ConfiguracaoMonitoramentoMonitorarPosicoes();
        }

        public Dominio.Entidades.Embarcador.Configuracoes.ConfiguracaoMonitoramentoProcessarMonitoramentos ObterConfiguracaoMonitoramentoProcessarMonitoramentos()
        {
            return _configuracaoMonitoramentoProcessarMonitoramentos ?? new Dominio.Entidades.Embarcador.Configuracoes.ConfiguracaoMonitoramentoProcessarMonitoramentos();
        }

        public Dominio.Entidades.Embarcador.Configuracoes.ConfiguracaoMonitoramentoProcessarTrocaAlvo ObterConfiguracaoMonitoramentoProcessarTrocaAlvo()
        {
            return _configuracaoMonitoramentoProcessarTrocaAlvos ?? new Dominio.Entidades.Embarcador.Configuracoes.ConfiguracaoMonitoramentoProcessarTrocaAlvo();
        }

        public Dominio.Entidades.Embarcador.Configuracoes.ConfiguracaoMonitoramentoProcessarEventos ObterConfiguracaoMonitoramentoProcessarEventos()
        {
            return _configuracaoMonitoramentoProcessarEventos ?? new Dominio.Entidades.Embarcador.Configuracoes.ConfiguracaoMonitoramentoProcessarEventos();
        }

        public Dominio.Entidades.Embarcador.Configuracoes.ConfiguracaoMonitoramentoProcessarEventosSinal ObterConfiguracaoMonitoramentoProcessarEventosSinal()
        {
            return _configuracaoMonitoramentoProcessarEventosSinal ?? new Dominio.Entidades.Embarcador.Configuracoes.ConfiguracaoMonitoramentoProcessarEventosSinal();
        }

        public Dominio.Entidades.Embarcador.Configuracoes.ConfiguracaoMonitoramentoEnviarNotificacoesAlerta ObterConfiguracaoMonitoramentoEnviarNotificacoesAlerta()
        {
            return _configuracaoMonitoramentoEnviarNotificacoesAlerta ?? new Dominio.Entidades.Embarcador.Configuracoes.ConfiguracaoMonitoramentoEnviarNotificacoesAlerta();
        }

        public Dominio.Entidades.Embarcador.Configuracoes.ConfiguracaoMonitoramentoImportarPosicoesPendenteIntegracao ObterConfiguracaoMonitoramentoImportarPosicoesPendenteIntegracao()
        {
            return _configuracaoMonitoramentoImportarPosicoesPendenteIntegracao ?? new Dominio.Entidades.Embarcador.Configuracoes.ConfiguracaoMonitoramentoImportarPosicoesPendenteIntegracao();
        }

        private void CarregarConfiguracoes(Repositorio.UnitOfWork unitOfWork)
        {
            _configuracaoMonitoramentoMonitorarThreads = new Repositorio.Embarcador.Configuracoes.ConfiguracaoMonitoramentoMonitorarThreads(unitOfWork).BuscarPrimeiroRegistro();
            _configuracaoMonitoramentoMonitorarPosicoes = new Repositorio.Embarcador.Configuracoes.ConfiguracaoMonitoramentoMonitorarPosicoes(unitOfWork).BuscarPrimeiroRegistro();
            _configuracaoMonitoramentoProcessarMonitoramentos = new Repositorio.Embarcador.Configuracoes.ConfiguracaoMonitoramentoProcessarMonitoramentos(unitOfWork).BuscarPrimeiroRegistro();
            _configuracaoMonitoramentoProcessarTrocaAlvos = new Repositorio.Embarcador.Configuracoes.ConfiguracaoMonitoramentoProcessarTrocaAlvo(unitOfWork).BuscarPrimeiroRegistro();
            _configuracaoMonitoramentoProcessarEventos = new Repositorio.Embarcador.Configuracoes.ConfiguracaoMonitoramentoProcessarEventos(unitOfWork).BuscarPrimeiroRegistro();
            _configuracaoMonitoramentoProcessarEventosSinal = new Repositorio.Embarcador.Configuracoes.ConfiguracaoMonitoramentoProcessarEventosSinal(unitOfWork).BuscarPrimeiroRegistro();
            _configuracaoMonitoramentoEnviarNotificacoesAlerta = new Repositorio.Embarcador.Configuracoes.ConfiguracaoMonitoramentoEnviarNotificacoesAlerta(unitOfWork).BuscarPrimeiroRegistro();
            _configuracaoMonitoramentoImportarPosicoesPendenteIntegracao = new Repositorio.Embarcador.Configuracoes.ConfiguracaoMonitoramentoImportarPosicoesPendenteIntegracao(unitOfWork).BuscarPrimeiroRegistro();
        }
    }
}
