using Dominio.Excecoes.Embarcador;
using Dominio.ObjetosDeValor.Embarcador.Enumeradores;
using System;
using System.Net;

namespace Servicos.Embarcador.Integracao.Atlas
{
    public class IntegracaoAtlas
    {
        private Dominio.Entidades.Embarcador.Cargas.CargaCargaIntegracao _cargaIntegracao;
        private Repositorio.UnitOfWork _unitOfWork;
        private AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware _tipoServicoMultisoftware;

        private Repositorio.Embarcador.Cargas.CargaCargaIntegracao _repCargaIntegracao;
        private Repositorio.Embarcador.Configuracoes.IntegracaoAtlas _repConfiguracaoIntegracao;
        private Repositorio.Embarcador.Cargas.Carga _repCarga;
        private Repositorio.Embarcador.Cargas.CargaPedido _repCargaPedido;
        private Dominio.Entidades.Embarcador.Configuracoes.IntegracaoAtlas _configuracaoIntegracao;
        private ServicoAtlas.AtlasPortTypeClient _clientWs;
        private string _Menssagem = "";
        private bool Sucesso = true;
        private InspectorBehavior _inspector = new InspectorBehavior();
        private ArquivoTransacao<Dominio.Entidades.Embarcador.Cargas.CargaCTeIntegracaoArquivo> _servicoArquivoTransacao;
        private Repositorio.Embarcador.Cargas.CargaCargaIntegracao _repCargaCargaIntegracao;
        private string _placaVeiculo;
        private DateTime _dataInicio;
        private DateTime _dataFim;

        public IntegracaoAtlas(Dominio.Entidades.Embarcador.Cargas.CargaCargaIntegracao cargaIntegracao, Repositorio.UnitOfWork unitOfWork, AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware tipoServicoMultisoftware)
        {
            // Criar o objeto como  se foce uma fabrica criamos o objeto com todo que é necessario para realizar as operações 
            // Conceito   Preparar , Apontar , Fogo 
            // Instancia repositorios e prepara objetos Listas parametros variaveis, etc quaisquer principalmente dados vindos da base de dados. Obs traga apenas colunas que serão necessarias. 
            _cargaIntegracao = cargaIntegracao;
            _unitOfWork = unitOfWork;
            _tipoServicoMultisoftware = tipoServicoMultisoftware;
            _repCargaIntegracao = new Repositorio.Embarcador.Cargas.CargaCargaIntegracao(unitOfWork);
            _repConfiguracaoIntegracao = new Repositorio.Embarcador.Configuracoes.IntegracaoAtlas(unitOfWork);
            _repCarga = new Repositorio.Embarcador.Cargas.Carga(unitOfWork);
            _repCargaPedido = new Repositorio.Embarcador.Cargas.CargaPedido(unitOfWork);
            _configuracaoIntegracao = _repConfiguracaoIntegracao.BuscarPrimeiroRegistro();
            _servicoArquivoTransacao = new ArquivoTransacao<Dominio.Entidades.Embarcador.Cargas.CargaCTeIntegracaoArquivo>(_unitOfWork);
            _repCargaCargaIntegracao = new Repositorio.Embarcador.Cargas.CargaCargaIntegracao(_unitOfWork);
            _placaVeiculo = _cargaIntegracao?.Carga?.Veiculo?.Placa ?? "";
            if (_placaVeiculo == "")
            {
                if (_cargaIntegracao.Carga?.VeiculosVinculados != null)
                    foreach (var veiculo in _cargaIntegracao.Carga?.VeiculosVinculados)
                    {
                        if (_placaVeiculo == "" && veiculo.Placa != null && veiculo.Placa != "")
                            _placaVeiculo = veiculo.Placa;
                    }
            }
            _dataInicio = _cargaIntegracao?.Carga?.DataInicioViagem ?? DateTime.Now;
            _dataFim = _cargaIntegracao?.Carga?.DataFimViagem ?? DateTime.Now;
            // valide as dados necessarios para a execução do processo de um metodo ou mais  caso ajam conflitos entre metodos crie novos construtures 
            // sempre que um novo construtor for criado avalie criação de novas funções para nao termos repetição de codigo porem apenas crie funções sobre demanda não tente imaginar que sera necessario 
            // criar uma função para cada possivel situação pois voce esta errado e vai acabar deixando o codigo muito distante da margem. Codigos longe da margem são caros para manter.
        }

        public void IntegrarCarga()
        {
            try
            {
                _cargaIntegracao.DataIntegracao = DateTime.Now;
                _cargaIntegracao.NumeroTentativas++;

                if (_configuracaoIntegracao == null)
                    _Menssagem = "Não existe configuração de integração disponível para a BrasilRisk.";

                if (!_configuracaoIntegracao.Ativa)
                    _Menssagem = "Integração nao esta ativa.";

                if (string.IsNullOrWhiteSpace(_configuracaoIntegracao.Usuario))
                    _Menssagem = "Usuario não esta configurado.";

                if (string.IsNullOrWhiteSpace(_configuracaoIntegracao.Usuario))
                    _Menssagem = "Senha não esta configurada.";

                if (string.IsNullOrWhiteSpace(_configuracaoIntegracao.URLAcesso))
                    _Menssagem = "URL não esta configurada.";

                if (_placaVeiculo == "")
                    _Menssagem = "Veiculo não informado.";

                if (_Menssagem != "")
                {
                    _cargaIntegracao.SituacaoIntegracao = SituacaoIntegracao.ProblemaIntegracao;
                    _cargaIntegracao.ProblemaIntegracao = _Menssagem;
                    _repCargaIntegracao.Atualizar(_cargaIntegracao);
                    return;
                }
               
                if (this.PreparaClient().ExisteSM().Sucesso)
                    _cargaIntegracao.SituacaoIntegracao = SituacaoIntegracao.Integrado;
                else
                    _cargaIntegracao.SituacaoIntegracao = SituacaoIntegracao.ProblemaIntegracao;
                _cargaIntegracao.ProblemaIntegracao = this._Menssagem;
            }
            catch (ServicoException excecao)
            {
                _cargaIntegracao.SituacaoIntegracao = SituacaoIntegracao.ProblemaIntegracao;
                _Menssagem = excecao.Message;
            }
            catch (Exception excecao)
            {
                Log.TratarErro(excecao);
                _cargaIntegracao.SituacaoIntegracao = SituacaoIntegracao.ProblemaIntegracao;
                _Menssagem = "Problema ao integrar com Atlas";
            }
            _servicoArquivoTransacao.Adicionar(_inspector.LastRequestXML, _inspector.LastResponseXML, "xml", _cargaIntegracao);
            _cargaIntegracao.ProblemaIntegracao = _Menssagem;
            _repCargaCargaIntegracao.Atualizar(_cargaIntegracao);
        }


        private IntegracaoAtlas PreparaClient()
        {
            System.ServiceModel.EndpointAddress endpointAddress = new System.ServiceModel.EndpointAddress(_configuracaoIntegracao.URLAcesso);
            System.ServiceModel.BasicHttpBinding binding = new System.ServiceModel.BasicHttpBinding();
            binding.MaxReceivedMessageSize = int.MaxValue;
            binding.ReceiveTimeout = new TimeSpan(0, 5, 0);
            binding.SendTimeout = new TimeSpan(0, 5, 0);

            if (_configuracaoIntegracao.URLAcesso.StartsWith("https"))
                binding.Security.Mode = System.ServiceModel.BasicHttpSecurityMode.Transport;
            else
                binding.Security.Mode = System.ServiceModel.BasicHttpSecurityMode.TransportCredentialOnly;
            binding.Security.Transport.ClientCredentialType = System.ServiceModel.HttpClientCredentialType.Basic;
            _clientWs = new ServicoAtlas.AtlasPortTypeClient(binding, endpointAddress);
            _clientWs.ClientCredentials.UserName.UserName = _configuracaoIntegracao.Usuario;
            _clientWs.ClientCredentials.UserName.Password = _configuracaoIntegracao.Senha;
            _clientWs.Endpoint.EndpointBehaviors.Add(_inspector);
            ServicePointManager.Expect100Continue = true;
            ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;
            return this;
        }


        private IntegracaoAtlas ExisteSM()
        {
            int usuario,codigoCliente;
            int.TryParse(_configuracaoIntegracao.Usuario, out usuario);
            int.TryParse(_configuracaoIntegracao.CodigoCliente, out codigoCliente);
            Servicos.ServicoAtlas.SMV2Response response = _clientWs.getSMV2(codigoCliente, usuario, _configuracaoIntegracao.Senha, "", _placaVeiculo, _dataInicio.ToString("dd/MM/yyyy"), _dataFim.ToString("dd/MM/yyyy"), "2");
            this.Sucesso = false;
            _Menssagem = "Erro geral";
            if (response.responseMessage != null)
                _Menssagem = response.responseMessage;

            if (response.dados != null)
            {
                foreach (var dados in response.dados)
                {
                    if (dados.status_sm == "INICIADA" )
                    {
                        this.Sucesso = true;
                        _Menssagem = "SM Encontrada";
                    }
                }
            }
            return this;
        }
    }
}
