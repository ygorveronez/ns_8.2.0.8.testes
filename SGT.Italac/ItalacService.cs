using AdminMultisoftware.Dominio.Enumeradores;
using Dominio.ObjetosDeValor.Embarcador.Enumeradores;
using Repositorio;
using SGT.Italac.Thread;

namespace SGT.Italac
{
    public sealed class ItalacService : AbstractThreadProcessamento
    {
        private AdminMultisoftware.Dominio.Entidades.Pessoas.Cliente? cliente;
        private string? stringConexao;
        private static ItalacService? Instance;

        #region Metodos Publicos
        public static ItalacService GetInstance()
        {
            if (Instance == null)
                Instance = new ItalacService();

            return Instance;
        }


        public void Iniciar(AdminMultisoftware.Dominio.Entidades.Pessoas.Cliente clienteMultisoftware, string stringConexaoCliente)
        {
            cliente = clienteMultisoftware;
            stringConexao = stringConexaoCliente;

            Log("Inicio das integrações italac");

            LoadConfigurations();
            string ambiente = Servicos.Embarcador.Configuracoes.ConfigurationInstance.GetInstance()?.ObterConfiguracaoAmbiente()?.IdentificacaoAmbiente ?? string.Empty;

            if (cliente == null)
            {
                Servicos.Log.TratarErro("Nenhum cliente encontrado.");
                Log("Nenhum cliente encontrado.");
                return;
            }

            Log(cliente.Codigo + " - " + cliente.RazaoSocial);
            Log(cliente.ClienteConfiguracao.TipoServicoMultisoftware.ToString());

            VerificaVersao();
            VerificaAmbiente(ambiente);

            Iniciar();
        }

        public void Iniciar()
        {
            IniciarThread(stringConexao, TipoServicoMultisoftware.MultiEmbarcador, cliente);
        }

        #endregion       

        #region Metodos Privados

        private void VerificaAmbiente(string ambiente)
        {
            Log($"Ambiente {ambiente}");
        }

        private void VerificaVersao()
        {
            System.Reflection.Assembly assembly = System.Reflection.Assembly.GetExecutingAssembly();
            System.Diagnostics.FileVersionInfo fvi = System.Diagnostics.FileVersionInfo.GetVersionInfo(assembly.Location);
            Log($"Versao {fvi.FileVersion}");
        }

        private void Log(string msg)
        {
            Servicos.Log.TratarErro(msg);
        }

        private void LoadConfigurations()
        {
            UnitOfWork unitOfWork = new UnitOfWork(stringConexao);

            try
            {
                Servicos.Embarcador.Configuracoes.ConfigurationInstance.GetInstance(unitOfWork);
                Servicos.Embarcador.Configuracoes.ConfiguracaoMonioramentoInstance.GetInstance(unitOfWork);

            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        #region Métodos protegidos

        protected override void Executar(UnitOfWork unitOfWork, string stringConexao, TipoServicoMultisoftware tipoServicoMultisoftware, AdminMultisoftware.Dominio.Entidades.Pessoas.Cliente clienteMultisoftware)
        {
            IntegracaoItalac integracaoItalac = new(unitOfWork);
            integracaoItalac.IntegrarLoteLiberacaoComercialPedido();
        }

        protected override bool TemRegistrosPendentes(UnitOfWork unitOfWork)
        {
            int numeroTentativas = 2;
            double minutosACadaTentativa = 5;

            return new Repositorio.Embarcador.Pedidos.LoteLiberacaoComercialPedido.LoteLiberacaoComercialPedidoIntegracao(unitOfWork).ExisteIntegracoesPendentesPorTipo(numeroTentativas, minutosACadaTentativa, TipoIntegracao.Italac);
        }

        #endregion

        #endregion
    }
}
