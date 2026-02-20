using Dominio.ObjetosDeValor.Embarcador.Integracao.Bradesco;
using Servicos.ServicoAverbacaoBradesco;
using System;
using System.IO;
using System.Text;
using System.Xml.Serialization;

namespace Servicos.Embarcador.Integracao.Bradesco
{
    public class IntegracaoBradescoAverbacao
    {
        private readonly Repositorio.UnitOfWork _unitOfWork;

        public IntegracaoBradescoAverbacao(Repositorio.UnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public void AverbarDocumento(Dominio.Entidades.Embarcador.Seguros.ApoliceSeguro apolice, Dominio.Entidades.AverbacaoCTe averbacao, ref int tentativas)
        {
            Repositorio.AverbacaoCTe repAverbacaoCTe = new Repositorio.AverbacaoCTe(_unitOfWork);

            IntegrarAverbacaoBradesco(apolice, averbacao, ref tentativas);

            averbacao.Tipo = Dominio.Enumeradores.TipoAverbacaoCTe.Autorizacao;
            averbacao.DataRetorno = DateTime.Now;

            repAverbacaoCTe.Atualizar(averbacao);
        }

        public void CancelarAverbacaoDocumento(Dominio.Entidades.Embarcador.Seguros.ApoliceSeguro apolice, Dominio.Entidades.AverbacaoCTe averbacao, ref int tentativas)
        {
            Repositorio.AverbacaoCTe repAverbacaoCTe = new Repositorio.AverbacaoCTe(_unitOfWork);

            IntegrarAverbacaoBradesco(apolice, averbacao, ref tentativas);

            if (averbacao.Status == Dominio.Enumeradores.StatusAverbacaoCTe.Sucesso)
            {
                averbacao.Status = Dominio.Enumeradores.StatusAverbacaoCTe.Cancelado;
                averbacao.MensagemRetorno = "Cancelado com sucesso";
            }

            averbacao.Tipo = Dominio.Enumeradores.TipoAverbacaoCTe.Cancelamento;
            averbacao.DataRetorno = DateTime.Now;

            repAverbacaoCTe.Atualizar(averbacao);
        }

        #region Métodos Privados Bradesco

        private void IntegrarAverbacaoBradesco(Dominio.Entidades.Embarcador.Seguros.ApoliceSeguro apolice, Dominio.Entidades.AverbacaoCTe averbacao, ref int tentativas)
        {
            Repositorio.Embarcador.Seguros.AverbacaoBradesco repAverbacaoBradesco = new Repositorio.Embarcador.Seguros.AverbacaoBradesco(_unitOfWork);
            Repositorio.Embarcador.Averbacao.AverbacaoIntegracaoArquivo repAverbacaoIntegracaoArquivo = new Repositorio.Embarcador.Averbacao.AverbacaoIntegracaoArquivo(_unitOfWork);

            string protocolo = "";
            string mensagemIntegracao = "";
            string codigoRetorno = "";
            string xmlRequisicao = "";
            string xmlRetorno = "";

            try
            {
                Dominio.Entidades.Embarcador.Seguros.AverbacaoBradesco configuracaoAverbacaoBradesco = repAverbacaoBradesco.BuscarPorApolice(apolice.Codigo);

                EnviarAverbacaoBradesco(averbacao, configuracaoAverbacaoBradesco, out xmlRequisicao, out xmlRetorno);

                RetornoAverbacaoBradesco retornoWebService = ConverterRetornoBradesco(xmlRetorno);

                if (retornoWebService != null && !retornoWebService.PossuiErro && retornoWebService.Documento != null)
                {
                    mensagemIntegracao = "Averbado com sucesso";
                    averbacao.Status = Dominio.Enumeradores.StatusAverbacaoCTe.Sucesso;
                    protocolo = retornoWebService.Documento.Protocolo;
                    averbacao.Averbacao = retornoWebService.Documento.Averbacao;
                    codigoRetorno = "0";
                }
                else if (retornoWebService != null)
                {
                    string erro = retornoWebService.Erro ?? retornoWebService.Documento?.Erro ?? "Erro desconhecido";
                    mensagemIntegracao = $"Erro no processamento - {erro}";
                    codigoRetorno = "1";
                }
                else
                {
                    mensagemIntegracao = "Ocorreu uma falha ao processar a averbação.";
                    codigoRetorno = "2";
                }

                tentativas = 0;
            }
            catch (Exception ex)
            {
                Log.TratarErro(ex);

                if (averbacao.tentativasIntegracao >= 1)
                {
                    codigoRetorno = "999";
                    mensagemIntegracao = "O serviço do Bradesco não está disponível no momento.";
                    averbacao.Status = Dominio.Enumeradores.StatusAverbacaoCTe.Rejeicao;
                    averbacao.tentativasIntegracao = 0;
                }
                else
                {
                    averbacao.tentativasIntegracao++;
                    tentativas++;
                }
            }

            if (!string.IsNullOrEmpty(xmlRequisicao) && !string.IsNullOrEmpty(xmlRetorno))
            {
                Dominio.Entidades.Embarcador.Averbacao.AverbacaoIntegracaoArquivo averbacaoIntegracaoArquivo = new Dominio.Entidades.Embarcador.Averbacao.AverbacaoIntegracaoArquivo
                {
                    ArquivoRequisicao = !string.IsNullOrEmpty(xmlRequisicao) ? ArquivoIntegracao.SalvarArquivoIntegracao(xmlRequisicao, "xml", _unitOfWork) : null,
                    ArquivoResposta = !string.IsNullOrEmpty(xmlRetorno) ? ArquivoIntegracao.SalvarArquivoIntegracao(xmlRetorno, "xml", _unitOfWork) : null,
                    Data = DateTime.Now,
                    Mensagem = codigoRetorno + " - " + mensagemIntegracao,
                    Tipo = Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoArquivoIntegracaoCTeCarga.EnvioParaProcessamento
                };

                repAverbacaoIntegracaoArquivo.Inserir(averbacaoIntegracaoArquivo);
                averbacao.ArquivosTransacao.Add(averbacaoIntegracaoArquivo);
            }

            if (!string.IsNullOrEmpty(protocolo))
            {
                averbacao.Protocolo = protocolo;
                averbacao.Averbacao = protocolo;
            }

            averbacao.CodigoRetorno = codigoRetorno;
            averbacao.MensagemRetorno = mensagemIntegracao;
        }

        private void EnviarAverbacaoBradesco(Dominio.Entidades.AverbacaoCTe averbacao, Dominio.Entidades.Embarcador.Seguros.AverbacaoBradesco averbacaoBradesco, out string xmlRequisicao, out string xmlResponse)
        {
            xmlRequisicao = string.Empty;
            xmlResponse = string.Empty;

            string xmlCTe = ObterXMLCTe(averbacao);
            string nomeArquivo = averbacao.CTe.Chave + ".xml";
            string endpointUrl = averbacaoBradesco?.WSDLQuorum ?? string.Empty;

            if (!string.IsNullOrEmpty(endpointUrl) && !endpointUrl.EndsWith("/soap", StringComparison.OrdinalIgnoreCase) && endpointUrl.EndsWith("/averba", StringComparison.OrdinalIgnoreCase))
                endpointUrl += "/soap";

            using (AverbaClient client = new AverbaClient(AverbaClient.EndpointConfiguration.IAverbaPort, endpointUrl))
            {
                System.ServiceModel.BasicHttpBinding binding = client.Endpoint.Binding as System.ServiceModel.BasicHttpBinding;
                if (binding != null)
                {
                    binding.TextEncoding = Encoding.UTF8;
                    binding.ReaderQuotas.MaxStringContentLength = int.MaxValue;
                    binding.ReaderQuotas.MaxArrayLength = int.MaxValue;
                    binding.ReaderQuotas.MaxBytesPerRead = int.MaxValue;
                }

                Servicos.Models.Integracao.InspectorBehavior inspector = new Servicos.Models.Integracao.InspectorBehavior();
                client.Endpoint.EndpointBehaviors.Add(inspector);

                client.AverbaXML(averbacaoBradesco.Token, nomeArquivo, xmlCTe);

                xmlRequisicao = inspector.LastRequestXML;
                xmlResponse = inspector.LastResponseXML;
            }
        }

        private string ObterXMLCTe(Dominio.Entidades.AverbacaoCTe averbacao)
        {
            Servicos.CTe svcCTE = new Servicos.CTe(_unitOfWork);
            string xmlAutorizado = svcCTE.ObterStringXMLAutorizacao(averbacao.CTe, _unitOfWork);

            if (string.IsNullOrEmpty(xmlAutorizado))
                throw new System.Exception("XML autorizado do CT-e não encontrado.");

            return LimparCDATAsAninhados(xmlAutorizado);
        }

        private string LimparCDATAsAninhados(string xmlContent)
        {
            if (string.IsNullOrEmpty(xmlContent))
                return xmlContent;

            string xmlLimpo = xmlContent.Replace("<![CDATA[", "").Replace("]]>", "");

            return xmlLimpo;
        }

        private RetornoAverbacaoBradesco ConverterRetornoBradesco(string xmlRetorno)
        {
            try
            {
                if (string.IsNullOrEmpty(xmlRetorno))
                    return null;

                string conteudoResposta = ExtrairConteudoCDATA(xmlRetorno);

                if (string.IsNullOrEmpty(conteudoResposta))
                    conteudoResposta = xmlRetorno;

                XmlSerializer serializer = new XmlSerializer(typeof(RetornoAverbacaoBradesco));

                using (StringReader reader = new StringReader(conteudoResposta))
                {
                    return (RetornoAverbacaoBradesco)serializer.Deserialize(reader);
                }
            }
            catch (System.Exception ex)
            {
                Log.TratarErro(ex);
                return null;
            }
        }

        private string ExtrairConteudoCDATA(string xmlRetorno)
        {
            int inicioIndex = xmlRetorno.IndexOf("<![CDATA[");
            if (inicioIndex >= 0)
            {
                inicioIndex += 9;
                int fimIndex = xmlRetorno.IndexOf("]]>");
                if (fimIndex > inicioIndex)
                {
                    return xmlRetorno.Substring(inicioIndex, fimIndex - inicioIndex);
                }
            }

            return xmlRetorno;
        }

        #endregion
    }
}