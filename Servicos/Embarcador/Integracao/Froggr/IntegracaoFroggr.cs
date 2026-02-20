using Dominio.Excecoes.Embarcador;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Servicos.Embarcador.Integracao.Froggr
{
    public class IntegracaoFroggr
    {
        #region Atributos
        readonly private Repositorio.UnitOfWork _unitOfWork;
        private Dominio.Entidades.Embarcador.Configuracoes.IntegracaoFroggr _configuracao;

        #endregion

        #region Constructores
        public IntegracaoFroggr(Repositorio.UnitOfWork unitOfWork) : base()
        {
            _unitOfWork = unitOfWork;
            ObterConfiguracaoIntegracaoFroggr();

        }
        #endregion




        #region MetodosPublicos
        public void SolicitaSM(Dominio.Entidades.Embarcador.Cargas.CargaCargaIntegracao integracaCargaCarga)
        {

            Repositorio.Embarcador.Cargas.CargaCargaIntegracao repositorioCargaCargaIntegracao = new Repositorio.Embarcador.Cargas.CargaCargaIntegracao(_unitOfWork);
            ArquivoTransacao<Dominio.Entidades.Embarcador.Cargas.CargaCTeIntegracaoArquivo> servicoArquivoTransacao = new ArquivoTransacao<Dominio.Entidades.Embarcador.Cargas.CargaCTeIntegracaoArquivo>(_unitOfWork);

            integracaCargaCarga.NumeroTentativas++;
            integracaCargaCarga.DataIntegracao = DateTime.Now;

            InspectorBehavior inspector = new InspectorBehavior(false);
            try
            {
                if (string.IsNullOrWhiteSpace(_configuracao.URLIntegracaoFroggr))
                    throw new ServicoException("Não existe URL de integração para Froggr configurada.");

                Servicos.ServicoFROGGR.IntegracaoFrogSoapClient Client = ObterCliente(_configuracao.URLIntegracaoFroggr);
                Client.Endpoint.EndpointBehaviors.Add(inspector);

                string cpfMotorista = "";
                string nomeMotorista = "";
                string dataInicioViagem = "";
                string DataFimViagem = "";
                string placaCavalo = "";
                string placaCarreta2 = "";
                string placaCarreta1 = "";
                string UFOrigem = "";
                string cidadeOrigem = "";
                string UFDestino = "";
                string cidadeDestino = "";
                string notaFiscal = "";
                string valorEmbarque = "";
                string CTRC = "";
                string nManifest = "";
                string nTransporte = "";
                string tecnologia = "";
                string mercadorias = "";
                string GPS = "";
                int qtdpallets = 0;
                bool possuiEscolta = true;

                SetarParametrosRequestSolicitaSM(integracaCargaCarga,
                        ref cpfMotorista, ref nomeMotorista, ref dataInicioViagem, ref DataFimViagem, ref placaCavalo,
                        ref placaCarreta2, ref placaCarreta1, ref UFOrigem, ref cidadeOrigem, ref UFDestino, ref cidadeDestino, ref notaFiscal,
                        ref valorEmbarque, ref CTRC, ref nManifest, ref nTransporte, ref tecnologia, ref mercadorias, ref GPS, ref qtdpallets, ref possuiEscolta);

                string retorno = Client.SolicitaSM(_configuracao.UsuarioIntegracaoFroggr,
                                                    _configuracao.SenhaIntegracaoFroggr,
                                                    cpfMotorista,
                                                    nomeMotorista,
                                                    dataInicioViagem,
                                                    DataFimViagem,
                                                    placaCavalo,
                                                    placaCarreta2,
                                                    placaCarreta1,
                                                    UFOrigem,
                                                    cidadeOrigem,
                                                    UFDestino,
                                                    cidadeDestino,
                                                    notaFiscal,
                                                    valorEmbarque,
                                                    CTRC,
                                                    nManifest,
                                                    nTransporte,
                                                    tecnologia,
                                                    mercadorias,
                                                    GPS,
                                                    qtdpallets,
                                                    possuiEscolta);

                if (retorno.Contains("Enviado com Sucesso"))
                {
                    integracaCargaCarga.SituacaoIntegracao = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoIntegracao.Integrado;
                    integracaCargaCarga.ProblemaIntegracao = "";
                    integracaCargaCarga.Protocolo = string.Empty;
                }
                else
                {
                    integracaCargaCarga.SituacaoIntegracao = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoIntegracao.ProblemaIntegracao;
                    if (retorno.Length > 299)
                        integracaCargaCarga.ProblemaIntegracao = retorno.Substring(0, 299);
                    else
                        integracaCargaCarga.ProblemaIntegracao = retorno;

                }
                servicoArquivoTransacao.Adicionar(integracaCargaCarga, inspector.LastRequestXML, inspector.LastResponseXML, "xml");
            }
            catch (ServicoException ex)
            {
                integracaCargaCarga.SituacaoIntegracao = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoIntegracao.ProblemaIntegracao;
                integracaCargaCarga.ProblemaIntegracao = ex.Message;
            }
            catch (Exception ex)
            {
                Log.TratarErro(ex);
                integracaCargaCarga.SituacaoIntegracao = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoIntegracao.ProblemaIntegracao;
                integracaCargaCarga.ProblemaIntegracao = "Problema ao tentar integrar.";
                servicoArquivoTransacao.Adicionar(integracaCargaCarga, inspector.LastRequestXML, inspector.LastResponseXML, "xml");
            }

            repositorioCargaCargaIntegracao.Atualizar(integracaCargaCarga);
        }
        #endregion


        #region MetodosPrivados
        private void ObterConfiguracaoIntegracaoFroggr()
        {
            Repositorio.Embarcador.Configuracoes.IntegracaoFroggr repIntegracaoFroggr = new Repositorio.Embarcador.Configuracoes.IntegracaoFroggr(_unitOfWork);
            Dominio.Entidades.Embarcador.Configuracoes.IntegracaoFroggr configuracaoIntagracao = repIntegracaoFroggr.BuscarPrimeiroRegistro();

            if ((configuracaoIntagracao == null) || !configuracaoIntagracao.PossuiIntegracaoFroggr)
                throw new ServicoException("Não existe configuração de integração disponível para a Froggr");

            if (string.IsNullOrWhiteSpace(configuracaoIntagracao.SenhaIntegracaoFroggr) || string.IsNullOrWhiteSpace(configuracaoIntagracao.UsuarioIntegracaoFroggr))
                throw new ServicoException("Não existe usuario ou senhas cadastrados para integração Froggr");

            _configuracao = configuracaoIntagracao;
        }

        private void SetarParametrosRequestSolicitaSM(Dominio.Entidades.Embarcador.Cargas.CargaCargaIntegracao integracao,
                        ref string cpfMotorista, ref string nomeMotorista, ref string dataInicioViagem, ref string DataFimViagem, ref string placaCavalo,
                        ref string placaCarreta2, ref string placaCarreta1, ref string UFOrigem, ref string cidadeOrigem, ref string UFDestino, ref string cidadeDestino, ref string notaFiscal,
                        ref string valorEmbarque, ref string CTRC, ref string nManifest, ref string nTransporte, ref string tecnologia, ref string mercadorias, ref string GPS, ref int qtdpallets,
                        ref bool possuiEscolta)
        {
            Repositorio.Embarcador.Cargas.CargaPedido repCargaPedido = new Repositorio.Embarcador.Cargas.CargaPedido(_unitOfWork);
            Repositorio.Embarcador.Cargas.CargaCTe repCargaCTe = new Repositorio.Embarcador.Cargas.CargaCTe(_unitOfWork);
            Dominio.Entidades.Embarcador.Cargas.CargaPedido cargaPedido = repCargaPedido.BuscarPorCarga(integracao.Carga.Codigo).FirstOrDefault();


            //Motoristas
            if ((integracao?.Carga?.Motoristas ?? null) != null)
            {
                foreach (var motorista in integracao.Carga.Motoristas)
                {
                    cpfMotorista = motorista.CPF_Formatado;
                    nomeMotorista = motorista.Nome;
                }
            }

            dataInicioViagem = integracao.Carga?.DadosSumarizados?.DataPrevisaoInicioViagem.ToDateString() == "" ? cargaPedido.Pedido.DataInicialViagemExecutada.ToDateString() : integracao.Carga?.DadosSumarizados?.DataPrevisaoInicioViagem.ToDateString() ?? "";
            DataFimViagem = integracao.Carga?.DadosSumarizados?.DataPrevisaoEntrega.ToDateString() == "" ? cargaPedido.Pedido.PrevisaoEntrega.ToDateString() : integracao.Carga?.DadosSumarizados?.DataPrevisaoEntrega.ToDateString() ?? "";
             
            placaCavalo = integracao.Carga?.Veiculo?.Placa ?? "";
            placaCarreta1 = integracao.Carga.VeiculosVinculados?.FirstOrDefault()?.Placa ?? "";
            //placaCarreta2 =

            if (cargaPedido != null)
            {
                UFOrigem = cargaPedido.Origem.Estado.Sigla;
                cidadeOrigem = cargaPedido.Origem.Descricao;
                UFDestino = cargaPedido.Destino.Estado.Sigla;
                cidadeDestino = cargaPedido.Destino.Descricao;
            }

            List<Dominio.Entidades.Embarcador.Cargas.CargaCTe> cargaCTes = repCargaCTe.BuscarPorCarga(integracao.Carga.Codigo);
            decimal _valorEmbarque = 0;
            foreach (var cargaCTe in cargaCTes)
            {
                CTRC = cargaCTe.CTe.RNTRC;

                foreach (var nota in cargaCTe.CTe.Documentos)
                {
                    notaFiscal += notaFiscal == "" ? nota.ChaveNFE : $" - {nota.ChaveNFE}";
                    _valorEmbarque += nota.ValorProdutos;
                }
            }
            valorEmbarque = _valorEmbarque.ToString();
            nManifest = integracao.Carga.CargaMDFes?.FirstOrDefault()?.MDFe.Numero.ToString() ?? "";

            nTransporte = integracao.Carga.CodigoCargaEmbarcador;
            tecnologia = "MULTIEMBARCADOR";
            mercadorias = "";
            GPS = "";
            qtdpallets = integracao.Carga.DadosSumarizados?.QuantidadeVolumes ?? 0;
            possuiEscolta = false;
        }




        private Servicos.ServicoFROGGR.IntegracaoFrogSoapClient ObterCliente(string url)
        {

            Servicos.ServicoFROGGR.IntegracaoFrogSoapClient client = null;
            System.ServiceModel.EndpointAddress endpointAddress = new System.ServiceModel.EndpointAddress(url);

            System.ServiceModel.BasicHttpBinding binding = new System.ServiceModel.BasicHttpBinding
            {
                MaxReceivedMessageSize = int.MaxValue,
                ReceiveTimeout = new TimeSpan(0, 20, 0),
                SendTimeout = new TimeSpan(0, 20, 0)
            };

            if (url.StartsWith("https"))
                binding.Security.Mode = System.ServiceModel.BasicHttpSecurityMode.Transport;

            client = new Servicos.ServicoFROGGR.IntegracaoFrogSoapClient(binding, endpointAddress);

            return client;
        }


        #endregion

    }
}
