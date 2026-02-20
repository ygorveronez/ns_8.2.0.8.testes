using Dominio.Entidades.Embarcador.Cargas;
using Dominio.Excecoes.Embarcador;
using Dominio.ObjetosDeValor.Embarcador.Enumeradores;
using System;
using System.IO;
using System.Linq;
using System.Net;
using System.Xml;
using System.Xml.Linq;

namespace Servicos.Embarcador.Integracao.DigitalCom
{
    public sealed class IntegracaoDigitalComServico : IIntegracaoDigitalCom
    {
        #region Atributos

        public readonly Repositorio.UnitOfWork _unitOfWork;

        private Dominio.Entidades.Embarcador.Configuracoes.IntegracaoDigitalCom _configuracaoIntegracaoDigitalCom;

        #endregion

        #region Construtores

        public IntegracaoDigitalComServico(Repositorio.UnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
            ObterConfiguracaoIntegracao();
        }

        #endregion

        #region Métodos Públicos

        public void IntegrarCarga(CargaDadosTransporteIntegracao cargaDadosTransporteIntegracao)
        {
            Repositorio.Embarcador.Cargas.CargaDadosTransporteIntegracao repCargaDadosTransporteIntegracao = new Repositorio.Embarcador.Cargas.CargaDadosTransporteIntegracao(_unitOfWork);
            Repositorio.Embarcador.Cargas.Carga repCarga = new Repositorio.Embarcador.Cargas.Carga(_unitOfWork);
            ArquivoTransacao<CargaCTeIntegracaoArquivo> servicoArquivoTransacao = new ArquivoTransacao<CargaCTeIntegracaoArquivo>(_unitOfWork);

            cargaDadosTransporteIntegracao.NumeroTentativas += 1;
            cargaDadosTransporteIntegracao.DataIntegracao = DateTime.Now;

            string requisicaoString = string.Empty;
            string xmlRetorno = string.Empty;

            try
            {
                VerificarConfiguracaoIntegracao();

                ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;
                HttpWebRequest requisicao = ObterWebServiceClient();
                XmlDocument xmlRequisicao = ObterRequisicaoXML(cargaDadosTransporteIntegracao);

                string result = string.Empty;
                requisicaoString = xmlRequisicao.OuterXml;

                using (Stream requestStream = requisicao.GetRequestStream())
                {
                    xmlRequisicao.Save(requestStream);
                }

                using (WebResponse response = requisicao.GetResponse())
                using (Stream stream = response.GetResponseStream())
                using (StreamReader reader = new StreamReader(stream))
                {
                    result = reader.ReadToEnd();
                }

                XmlDocument xmlResponse = new XmlDocument();
                xmlResponse.LoadXml(result);
                XDocument doc = XDocument.Parse(result);

                xmlRetorno = doc.ToString();

                if (doc.Descendants("return").ToList().Count > 0)
                {
                    cargaDadosTransporteIntegracao.Carga.TAGPedagio = TAGPedagio.Invalida;

                    foreach (var retorno in doc.Descendants("return").ToList())
                    {
                        if (int.Parse(retorno.Descendants("id").FirstOrDefault().Value) == 214)
                            cargaDadosTransporteIntegracao.Carga.TAGPedagio = TAGPedagio.Valida;

                        cargaDadosTransporteIntegracao.SituacaoIntegracao = SituacaoIntegracao.Integrado;
                        cargaDadosTransporteIntegracao.ProblemaIntegracao = retorno.Descendants("descricao").FirstOrDefault().Value.ToString();
                        MeiosPagamentoDigitalCom? meioPagamentoDigitalCom = retorno.Descendants("meansOfPayment").FirstOrDefault().Value.ToString().ToNullableEnum<MeiosPagamentoDigitalCom>();
                        int idRetornoDigitalCom = int.Parse(retorno.Descendants("id").FirstOrDefault().Value);

                        CargaCTeIntegracaoArquivo arquivoRequisicao = servicoArquivoTransacao.Adicionar(requisicaoString, xmlRetorno, "xml", cargaDadosTransporteIntegracao);

                        GerarIntegracaoDigitalComArquivosTransacao(cargaDadosTransporteIntegracao, arquivoRequisicao, meioPagamentoDigitalCom, idRetornoDigitalCom, _unitOfWork);
                    }

                    repCarga.Atualizar(cargaDadosTransporteIntegracao.Carga);
                }
                else
                {
                    cargaDadosTransporteIntegracao.ProblemaIntegracao = "O WS DigitalCom não respondeu à solicitação.";
                    cargaDadosTransporteIntegracao.SituacaoIntegracao = SituacaoIntegracao.ProblemaIntegracao;

                    servicoArquivoTransacao.Adicionar(cargaDadosTransporteIntegracao, requisicaoString, xmlRetorno, "xml");
                }
            }
            catch (ServicoException excecao)
            {
                cargaDadosTransporteIntegracao.SituacaoIntegracao = SituacaoIntegracao.ProblemaIntegracao;
                cargaDadosTransporteIntegracao.ProblemaIntegracao = excecao.Message;
            }
            catch (Exception excecao)
            {
                Log.TratarErro(excecao);

                cargaDadosTransporteIntegracao.SituacaoIntegracao = SituacaoIntegracao.ProblemaIntegracao;
                cargaDadosTransporteIntegracao.ProblemaIntegracao = "Ocorreu uma falha ao realizar a integração com a DigitalCom";

                servicoArquivoTransacao.Adicionar(cargaDadosTransporteIntegracao, requisicaoString, xmlRetorno, "xml");
            }

            repCargaDadosTransporteIntegracao.Atualizar(cargaDadosTransporteIntegracao);
        }

        public bool PermitirGerarIntegracao()
        {
            return _configuracaoIntegracaoDigitalCom?.ValidacaoTAGDigitalCom ?? false;
        }

        #endregion Métodos Públicos

        #region Métodos Privados

        private HttpWebRequest ObterWebServiceClient()
        {
            HttpWebRequest requisicao = (HttpWebRequest)WebRequest.Create(_configuracaoIntegracaoDigitalCom.EndpointDigitalCom);

            requisicao.Headers.Add("SOAPAction", "https://apim.digitalcomm.com.br:8243/dcloggWsUnilever/v4.0.0/listarMeioPagamentoVeiculo");
            requisicao.Headers.Add("Authorization", "Bearer " + _configuracaoIntegracaoDigitalCom.TokenDigitalCom);
            requisicao.ContentType = "text/xml;charset=\"utf-8\"";
            requisicao.Accept = "text/xml";
            requisicao.Method = "POST";
            requisicao.KeepAlive = true;

            return requisicao;
        }

        private XmlDocument ObterRequisicaoXML(CargaDadosTransporteIntegracao cargaIntegracao)
        {
            Repositorio.Embarcador.Cargas.TipoIntegracao repositorioTipoIntegracao = new Repositorio.Embarcador.Cargas.TipoIntegracao(_unitOfWork);
            XmlDocument xmlRequisicaoEnvelopado = new XmlDocument();

            if (repositorioTipoIntegracao.ExistePorTipo(Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoIntegracao.Unilever) && !string.IsNullOrEmpty(cargaIntegracao.Protocolo))
            {
                xmlRequisicaoEnvelopado.LoadXml($@"
                <soapenv:Envelope xmlns:soapenv=""http://schemas.xmlsoap.org/soap/envelope/"" xmlns:v4=""http://dclogg.digitalcomm.com.br/ValePedagio/v4"">
                    <soapenv:Header/>
                    <soapenv:Body>
                        <v4:listarMeioPagamentoVeiculo>
                            <placaVeiculo>{cargaIntegracao.Protocolo ?? ""}</placaVeiculo>
                            <cnpj>{_configuracaoIntegracaoDigitalCom.CNPJLogin ?? ""}</cnpj>
                        </v4:listarMeioPagamentoVeiculo>
                    </soapenv:Body>
                </soapenv:Envelope>"
                  );

                return xmlRequisicaoEnvelopado;
            }

            xmlRequisicaoEnvelopado.LoadXml($@"
                <soapenv:Envelope xmlns:soapenv=""http://schemas.xmlsoap.org/soap/envelope/"" xmlns:v4=""http://dclogg.digitalcomm.com.br/ValePedagio/v4"">
                    <soapenv:Header/>
                    <soapenv:Body>
                        <v4:listarMeioPagamentoVeiculo>
                            <placaVeiculo>{cargaIntegracao.Carga.Veiculo?.Placa ?? ""}</placaVeiculo>
                            <cnpj>{_configuracaoIntegracaoDigitalCom.CNPJLogin ?? ""}</cnpj>
                        </v4:listarMeioPagamentoVeiculo>
                    </soapenv:Body>
                </soapenv:Envelope>"
            );

            return xmlRequisicaoEnvelopado;
        }

        private void ObterConfiguracaoIntegracao()
        {
            Repositorio.Embarcador.Configuracoes.IntegracaoDigitalCom repositorioConfiguracaoDigitalCom = new Repositorio.Embarcador.Configuracoes.IntegracaoDigitalCom(_unitOfWork);

            _configuracaoIntegracaoDigitalCom ??= repositorioConfiguracaoDigitalCom.Buscar();
        }

        private void VerificarConfiguracaoIntegracao()
        {
            if ((_configuracaoIntegracaoDigitalCom == null) || !_configuracaoIntegracaoDigitalCom.ValidacaoTAGDigitalCom)
                throw new ServicoException("Não existe configuração de integração disponível para a DigitalCom");

            if (string.IsNullOrWhiteSpace(_configuracaoIntegracaoDigitalCom.TokenDigitalCom))
                throw new ServicoException("O Token deve estar preenchido na configuração de integração da DigitalCom");

            if (string.IsNullOrWhiteSpace(_configuracaoIntegracaoDigitalCom.EndpointDigitalCom))
                throw new ServicoException("Não existe URL de integração configurada para DigitalCom");

            if (string.IsNullOrWhiteSpace(_configuracaoIntegracaoDigitalCom.CNPJLogin))
                throw new ServicoException("O CNPJ de Login deve ser informado na configuração da integração da DigitalCom");
        }

        private void GerarIntegracaoDigitalComArquivosTransacao(CargaDadosTransporteIntegracao cargaDadosTransporteIntegracao, CargaCTeIntegracaoArquivo arquivoRequisicao, MeiosPagamentoDigitalCom? meioPagamentoDigitalCom, int id, Repositorio.UnitOfWork unitOfWork)
        {
            Repositorio.Embarcador.Cargas.IntegracaoDigitalComArquivosTransacao repDigitalComArquivos = new Repositorio.Embarcador.Cargas.IntegracaoDigitalComArquivosTransacao(unitOfWork);

            Dominio.Entidades.Embarcador.Cargas.IntegracaoDigitalCom.IntegracaoDigitalComArquivosTransacao integracaoDigitalComArquivos = new Dominio.Entidades.Embarcador.Cargas.IntegracaoDigitalCom.IntegracaoDigitalComArquivosTransacao
            {
                Carga = cargaDadosTransporteIntegracao.Carga,
                CargaCTeIntegracaoArquivo = arquivoRequisicao,
                MeioPagamentoDigitalCom = meioPagamentoDigitalCom,
                IDRetornoDigitalCom = id
            };

            repDigitalComArquivos.Inserir(integracaoDigitalComArquivos);
        }

        #endregion Métodos Privados
    }
}
