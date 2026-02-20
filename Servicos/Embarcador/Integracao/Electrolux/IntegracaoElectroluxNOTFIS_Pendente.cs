using Dominio.ObjetosDeValor.Embarcador.Enumeradores;
using Dominio.ObjetosDeValor.Embarcador.Integracao;
using Servicos.Servico.Electrolux.NOFIS;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.ServiceModel.Channels;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Linq;
using System.Xml.Serialization;

namespace Servicos.Embarcador.Integracao.Electrolux
{
    public class IntegracaoElectroluxNOTFIS_Pendente : IntegracaoElectroluxBase
    {

        #region Atributos Globais

        private readonly SI_PendNotfis_EDI_Sync_OutboundRequest _request;
        private readonly string _identificadorTransportador;
        private readonly DateTime _dataInicial;
        private readonly DateTime _dataFinal;
        private readonly Repositorio.UnitOfWork _unitOfWork;
        private readonly Dominio.Entidades.Usuario _usuario;

        #endregion Atributos Globais

        #region CONSTRUTOR

        /// <summary>
        /// NOTFIS - Electrolux - NFs Pendentes
        /// </summary>
        /// <param name="unitOfWork"></param>
        /// <param name="identificadorTransportador">Identificação do transportador</param>
        /// <param name="dataInicial">Data Inicial</param>
        /// <param name="dataFinal">DataFinal</param>
        public IntegracaoElectroluxNOTFIS_Pendente(Dominio.Entidades.Usuario usuario, Repositorio.UnitOfWork unitOfWork, string identificadorTransportador, DateTime dataInicial, DateTime dataFinal) : base (unitOfWork)
        {
            _usuario = usuario;
            _unitOfWork = unitOfWork;
            _request = new SI_PendNotfis_EDI_Sync_OutboundRequest();
            _identificadorTransportador = identificadorTransportador;
            _dataInicial = dataInicial;
            _dataFinal = dataFinal;
        }

        #endregion

        #region PÚBLICOS

        public List<object> ConsultarPendentes(Dominio.Entidades.Empresa empresa)
        {
            if (string.IsNullOrWhiteSpace(_identificadorTransportador))
            {
                _httpRequisicaoResposta.sucesso = false;
                _httpRequisicaoResposta.mensagem = "Identificador do agente transportados inválido";
                _result.Add(_httpRequisicaoResposta);
                return _result;
            }

            _configuracaoIntegracaoElectroluxDominio = _configuracaoIntegracaoElectroluxRepositorio.BuscarPrimeiroRegistro();

            if (!_configuracaoIntegracaoElectroluxDominio.ElectroluxPossuiIntegracao)
            {
                _httpRequisicaoResposta.sucesso = false;
                _httpRequisicaoResposta.mensagem = "Configuração da integração com a Electrolux não encontrada";
                _result.Add(_httpRequisicaoResposta);
                return _result;
            }

            ExecutarAsync(empresa).Wait();

            _result.Add(_httpRequisicaoResposta);

            // Retorna o resultado da consulta ao NOTFIS Pendente
            return _result;
        }

        #endregion

        #region PRIVADOS

        private async Task ExecutarAsync(Dominio.Entidades.Empresa empresa)
        {
            try
            {
                Repositorio.Embarcador.Integracao.IntegracaoElectroluxConsultaLog repIntegracaoElectroluxConsultaLog = new Repositorio.Embarcador.Integracao.IntegracaoElectroluxConsultaLog(_unitOfWork);

                using (var httpClient = ObterClient(_configuracaoIntegracaoElectroluxDominio.UrlNotfisLista))
                {
                    Models.Integracao.InspectorBehavior inspectorBehavior = new Models.Integracao.InspectorBehavior();

                    httpClient.Endpoint.EndpointBehaviors.Add(inspectorBehavior);

                    var soapEnvelope = GerarRequestEnvelope();

                    httpClient.ClientCredentials.UserName.UserName = _configuracaoIntegracaoElectroluxDominio.Usuario;
                    httpClient.ClientCredentials.UserName.Password = _configuracaoIntegracaoElectroluxDominio.Senha;

                    Servicos.Servico.Electrolux.NOFIS.SI_PendNotfis_EDI_Sync_OutboundResponse response = await httpClient.SI_PendNotfis_EDI_Sync_OutboundAsync(soapEnvelope);

                    string mensagem = "";

                    if (response.MT_ResultPendNotfis_EDI.Count() > 0)
                    {
                        foreach (var item in response.MT_ResultPendNotfis_EDI)
                        {
                            if (!string.IsNullOrEmpty(item.identNotfis.ident))
                            {
                                mensagem = "Consulta de DTs realizada com sucesso.";

                                Dominio.Entidades.Embarcador.Integracao.IntegracaoElectroluxDocumentoTransporte documentoTransporte = SalvarDocumentoTransporte(empresa, item.identNotfis.ident, item.dataDisp);
                                Servicos.Embarcador.Integracao.Electrolux.IntegracaoElectroluxNOTFIS_Detalhe svcElectrolux = new Servicos.Embarcador.Integracao.Electrolux.IntegracaoElectroluxNOTFIS_Detalhe(_usuario, _unitOfWork, _identificadorTransportador, item.identNotfis.ident);
                                await svcElectrolux.ConsultarNOTFISAsync(empresa, documentoTransporte);
                            }
                        }
                    }
                    else
                    {
                        mensagem = @"Nenhum registro retornou para o critério informado.";
                    }

                    Dominio.Entidades.Embarcador.Integracao.IntegracaoElectroluxConsultaLog integracao = new Dominio.Entidades.Embarcador.Integracao.IntegracaoElectroluxConsultaLog();

                    integracao.ArquivoRequisicao = Servicos.Embarcador.Integracao.ArquivoIntegracao.SalvarArquivoIntegracao(inspectorBehavior.LastRequestXML, "xml", _unitOfWork);
                    integracao.ArquivoResposta = Servicos.Embarcador.Integracao.ArquivoIntegracao.SalvarArquivoIntegracao(inspectorBehavior.LastResponseXML, "xml", _unitOfWork);
                    integracao.DataConsulta = DateTime.Now;
                    integracao.Situacao = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoIntegracaoElectrolux.Sucesso;
                    integracao.Tipo = Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoIntegracaoElectrolux.NotfisPendente;
                    integracao.Retorno = mensagem;
                    integracao.ParametroDataInicial = _dataInicial;
                    integracao.ParametroDataFinal = _dataFinal;
                    integracao.Usuario = _usuario;
                    repIntegracaoElectroluxConsultaLog.Inserir(integracao);

                    _httpRequisicaoResposta.sucesso = true;
                    _httpRequisicaoResposta.mensagem = $"Sucesso";
                    _result.Add(_httpRequisicaoResposta);
                }
            }
            catch (Exception ex)
            {
                _result.Add(_httpRequisicaoResposta);
                Log.TratarErro(ex);
            }
        }

        private DT_SearchPendNotifis_SAP GerarRequestEnvelope() 
        {
            return new DT_SearchPendNotifis_SAP() 
            { 
                identService = new DT_IdentService_SAP() { identCarrier = _identificadorTransportador }, dataInicial = _dataInicial, dataFinal = _dataFinal 
            };
        }
        private static Servicos.Servico.Electrolux.NOFIS.SI_PendNotfis_EDI_Sync_OutboundClient ObterClient(string url)
        {
            var endpointAddress = new System.ServiceModel.EndpointAddress(url);
            var binding = new System.ServiceModel.BasicHttpBinding
            {
                MaxReceivedMessageSize = int.MaxValue,
                ReceiveTimeout = TimeSpan.FromMinutes(5)
            };

            if (url.StartsWith("https", StringComparison.OrdinalIgnoreCase))
            {
                binding.Security.Mode = System.ServiceModel.BasicHttpSecurityMode.Transport;
                binding.Security.Transport.ClientCredentialType = System.ServiceModel.HttpClientCredentialType.Basic; // Ajuste conforme necessário
            }

            return new Servicos.Servico.Electrolux.NOFIS.SI_PendNotfis_EDI_Sync_OutboundClient(binding, endpointAddress);
        }

        private Dominio.Entidades.Embarcador.Integracao.IntegracaoElectroluxDocumentoTransporte SalvarDocumentoTransporte(Dominio.Entidades.Empresa empresa, string identificadorNotfis, DateTime? data)
        {
            Repositorio.Embarcador.Integracao.IntegracaoElectroluxDocumentoTransporte repDocumentoTransporte = new Repositorio.Embarcador.Integracao.IntegracaoElectroluxDocumentoTransporte(_unitOfWork);
            Dominio.Entidades.Embarcador.Integracao.IntegracaoElectroluxDocumentoTransporte documentoTransporte = repDocumentoTransporte.BuscarPorNumero(empresa.Codigo, identificadorNotfis, false);

            if (documentoTransporte == null)
            {
                documentoTransporte = new Dominio.Entidades.Embarcador.Integracao.IntegracaoElectroluxDocumentoTransporte();
                documentoTransporte.Empresa = empresa;
                documentoTransporte.GeradoPorNOTFIS = true;
                documentoTransporte.NumeroNotfis = identificadorNotfis;
                documentoTransporte.Integracoes = new List<Dominio.Entidades.Embarcador.Integracao.IntegracaoElectroluxConsultaLog>();
                documentoTransporte.ValorFrete = 0;
                documentoTransporte.Data = data ?? DateTime.Now;
                documentoTransporte.Numero = 0;
                documentoTransporte.Status = true;
                repDocumentoTransporte.Inserir(documentoTransporte);
            }

            return documentoTransporte;
        }
        #endregion


    }

}
