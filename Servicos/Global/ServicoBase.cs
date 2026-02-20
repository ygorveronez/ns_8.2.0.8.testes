using Dominio.Interfaces.Database;
using Repositorio;
using System.Threading;
using System.Threading.Tasks;

namespace Servicos
{
    public class ServicoBase
    {
        #region Propriedades

        private readonly string _stringConexao;
        protected readonly CancellationToken _cancellationToken;
        protected readonly UnitOfWork _unitOfWork;
        protected readonly AdminMultisoftware.Dominio.Entidades.Pessoas.Cliente _clienteMultisoftware;
        protected readonly AdminMultisoftware.Dominio.Entidades.Pessoas.ClienteURLAcesso _clienteURLAcesso;
        protected readonly AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware _tipoServicoMultisoftware;

        private Dominio.Entidades.Embarcador.Configuracao.ConfiguracaoTMS _configuracaoEmbarcador;
        private Dominio.Entidades.Embarcador.Configuracoes.ConfiguracaoGeralCarga _configuracaoGeralCarga;
        private Dominio.Entidades.Embarcador.Configuracoes.ConfiguracaoGeral _configuracaoGeral;
        private Dominio.Entidades.Embarcador.Configuracoes.ConfiguracaoMontagemCarga _configuracaoMontagemCarga;
        private Dominio.Entidades.Embarcador.Configuracoes.ConfiguracaoMonitoramento _configuracaoMonitoramento;
        private Dominio.Entidades.Embarcador.Configuracoes.ConfiguracaoPedido _configuracaoPedido;
        private Dominio.Entidades.Embarcador.Configuracoes.ConfiguracaoCanhoto _configuracaoCanhoto;
        private Dominio.Entidades.Embarcador.Configuracoes.ConfiguracaoFinanceiraAbastecimento _configuracaoFinanceiraAbastecimento;

        public string StringConexao
        {
            get
            {
                return _stringConexao;
            }
        }

        #endregion

        #region Construtores

        public ServicoBase() { }        

        public ServicoBase(UnitOfWork unitOfWork, CancellationToken cancelationToken = default)
        {
            _unitOfWork = unitOfWork;
            _cancellationToken = cancelationToken;
            _stringConexao = unitOfWork.StringConexao;
        }

        public ServicoBase(UnitOfWork unitOfWork, AdminMultisoftware.Dominio.Entidades.Pessoas.Cliente clienteMultisoftware, CancellationToken cancelationToken = default)
        {
            _unitOfWork = unitOfWork;
            _cancellationToken = cancelationToken;
            _stringConexao = unitOfWork.StringConexao;
            _clienteMultisoftware = clienteMultisoftware;
        }

        public ServicoBase(UnitOfWork unitOfWork, AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware tipoServicoMultisoftware, CancellationToken cancelationToken = default)
        {
            _unitOfWork = unitOfWork;
            _cancellationToken = cancelationToken;
            _stringConexao = unitOfWork.StringConexao;
            _tipoServicoMultisoftware = tipoServicoMultisoftware;
        }

        public ServicoBase(UnitOfWork unitOfWork, AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware tipoServicoMultisoftware, AdminMultisoftware.Dominio.Entidades.Pessoas.Cliente clienteMultisoftware, CancellationToken cancelationToken = default)
        {
            _unitOfWork = unitOfWork;
            _cancellationToken = cancelationToken;
            _stringConexao = unitOfWork.StringConexao;
            _tipoServicoMultisoftware = tipoServicoMultisoftware;
            _clienteMultisoftware = clienteMultisoftware;
        }

        public ServicoBase(UnitOfWork unitOfWork, AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware tipoServicoMultisoftware, AdminMultisoftware.Dominio.Entidades.Pessoas.ClienteURLAcesso clienteURLAcesso, CancellationToken cancelationToken = default)
        {
            _unitOfWork = unitOfWork;
            _cancellationToken = cancelationToken;
            _stringConexao = unitOfWork.StringConexao;
            _tipoServicoMultisoftware = tipoServicoMultisoftware;
            _clienteURLAcesso = clienteURLAcesso;
        }

        public ServicoBase(UnitOfWork unitOfWork, AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware tipoServicoMultisoftware, AdminMultisoftware.Dominio.Entidades.Pessoas.ClienteURLAcesso clienteURLAcesso, AdminMultisoftware.Dominio.Entidades.Pessoas.Cliente clienteMultisoftware, CancellationToken cancelationToken = default)
        {
            _unitOfWork = unitOfWork;
            _cancellationToken = cancelationToken;
            _stringConexao = unitOfWork.StringConexao;
            _tipoServicoMultisoftware = tipoServicoMultisoftware;
            _clienteURLAcesso = clienteURLAcesso;
            _clienteMultisoftware = clienteMultisoftware;
        }

        #endregion 
    }
}
