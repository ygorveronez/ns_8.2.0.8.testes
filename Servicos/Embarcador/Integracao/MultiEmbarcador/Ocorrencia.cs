using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;

namespace Servicos.Embarcador.Integracao.MultiEmbarcador
{
    public class Ocorrencia
    {
        #region Atributos

        private readonly Repositorio.UnitOfWork _unitOfWork;

        #endregion

        #region Construtores

        public Ocorrencia(Repositorio.UnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        #endregion

        #region Métodos Públicos

        public static void IntegracarOcorrencia(Dominio.Entidades.Embarcador.Ocorrencias.OcorrenciaCTeIntegracao ocorrenciaCTeIntegracao, Repositorio.UnitOfWork unitOfWork)
        {
            Repositorio.Embarcador.Ocorrencias.OcorrenciaCTeIntegracao repositorioOcorrenciaCTeIntegracao = new Repositorio.Embarcador.Ocorrencias.OcorrenciaCTeIntegracao(unitOfWork);
            Repositorio.Embarcador.Ocorrencias.OcorrenciaCTeIntegracaoArquivo repositorioOcorrenciaCTeIntegracaoArquivo = new Repositorio.Embarcador.Ocorrencias.OcorrenciaCTeIntegracaoArquivo(unitOfWork);
            Repositorio.Cliente repositorioCliente = new Repositorio.Cliente(unitOfWork);
            Repositorio.Embarcador.Cargas.CargaPedidoXMLNotaFiscalCTe repositorioCargaPedidoXMLNotaFiscalCTe = new Repositorio.Embarcador.Cargas.CargaPedidoXMLNotaFiscalCTe(unitOfWork);
            Repositorio.Embarcador.Ocorrencias.CargaOcorrenciaDocumento repCargaOcorrenciaDocumento = new Repositorio.Embarcador.Ocorrencias.CargaOcorrenciaDocumento(unitOfWork);

            Dominio.Entidades.Cliente tomador = repositorioCliente.BuscarPorCPFCNPJ(double.Parse(ocorrenciaCTeIntegracao.CargaCTe.CTe?.Tomador.CPF_CNPJ ?? ocorrenciaCTeIntegracao.CargaCTe.PreCTe.Tomador.CPF_CNPJ));
            List<Dominio.Entidades.Embarcador.Pedidos.XMLNotaFiscal> notasFiscais = null;

            if (ocorrenciaCTeIntegracao.CargaOcorrencia.UtilizarSelecaoPorNotasFiscaisCTe)
            {
                Dominio.Entidades.Embarcador.Ocorrencias.CargaOcorrenciaDocumento cargaOcorrenciaDocumento = repCargaOcorrenciaDocumento.BuscarPorCargaCTeEOcorrencia(ocorrenciaCTeIntegracao.CargaCTe.Codigo, ocorrenciaCTeIntegracao.CargaOcorrencia.Codigo);
                notasFiscais = cargaOcorrenciaDocumento.XMLNotaFiscais?.ToList();

            }
            else
            {
                notasFiscais = repositorioCargaPedidoXMLNotaFiscalCTe.BuscarXMLNotasFiscaisPorCTe(ocorrenciaCTeIntegracao.CargaCTe.Codigo);
            }

            string codigoTipoOcorrenciaIntegracao = !string.IsNullOrEmpty(ocorrenciaCTeIntegracao.CargaOcorrencia.CodigoTipoOcorrenciaParaIntegracao) ? ocorrenciaCTeIntegracao.CargaOcorrencia.CodigoTipoOcorrenciaParaIntegracao : ocorrenciaCTeIntegracao.CargaOcorrencia.TipoOcorrencia.CodigoIntegracao;
            Dominio.Entidades.Empresa empresa = ocorrenciaCTeIntegracao.CargaCTe.CTe?.Empresa ?? ocorrenciaCTeIntegracao.CargaCTe.PreCTe.Empresa;

            foreach (var nota in notasFiscais)
            {
                Dominio.ObjetosDeValor.Embarcador.Integracao.HttpRequisicaoResposta httpRequisicaoResposta = AdicionarOcorrencia(tomador, empresa, new List<Dominio.Entidades.Embarcador.Pedidos.XMLNotaFiscal> { nota }, codigoTipoOcorrenciaIntegracao, ocorrenciaCTeIntegracao.CargaOcorrencia.DataOcorrencia, unitOfWork);

                Dominio.Entidades.Embarcador.Ocorrencias.OcorrenciaCTeIntegracaoArquivo arquivoIntegracao = new Dominio.Entidades.Embarcador.Ocorrencias.OcorrenciaCTeIntegracaoArquivo();
                arquivoIntegracao.Data = ocorrenciaCTeIntegracao.DataIntegracao;
                arquivoIntegracao.Mensagem = ocorrenciaCTeIntegracao.ProblemaIntegracao ?? "";
                arquivoIntegracao.Tipo = Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoArquivoIntegracaoCTeCarga.EnvioParaProcessamento;
                arquivoIntegracao.ArquivoRequisicao = Servicos.Embarcador.Integracao.ArquivoIntegracao.SalvarArquivoIntegracao(httpRequisicaoResposta.conteudoRequisicao, httpRequisicaoResposta.extensaoRequisicao, unitOfWork);
                arquivoIntegracao.ArquivoResposta = Servicos.Embarcador.Integracao.ArquivoIntegracao.SalvarArquivoIntegracao(httpRequisicaoResposta.conteudoResposta, httpRequisicaoResposta.extensaoResposta, unitOfWork);
                repositorioOcorrenciaCTeIntegracaoArquivo.Inserir(arquivoIntegracao);

                ocorrenciaCTeIntegracao.ArquivosTransacao.Add(arquivoIntegracao);

                if (httpRequisicaoResposta.sucesso)
                {
                    ocorrenciaCTeIntegracao.SituacaoIntegracao = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoIntegracao.Integrado;
                    ocorrenciaCTeIntegracao.ProblemaIntegracao = httpRequisicaoResposta.mensagem;

                }
                else
                {
                    ocorrenciaCTeIntegracao.SituacaoIntegracao = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoIntegracao.ProblemaIntegracao;
                    ocorrenciaCTeIntegracao.ProblemaIntegracao = httpRequisicaoResposta.mensagem;
                    break;
                }

            }

            ocorrenciaCTeIntegracao.DataIntegracao = DateTime.Now;
            ocorrenciaCTeIntegracao.NumeroTentativas++;

            repositorioOcorrenciaCTeIntegracao.Atualizar(ocorrenciaCTeIntegracao);

        }

        public static Dominio.ObjetosDeValor.Embarcador.Integracao.HttpRequisicaoResposta AdicionarOcorrencia(Dominio.Entidades.Cliente cliente, Dominio.Entidades.Empresa empresa, List<Dominio.Entidades.Embarcador.Pedidos.XMLNotaFiscal> notasFiscais, string codigoTipoOcorrenciaIntegracao, DateTime data, Repositorio.UnitOfWork unitOfWork)
        {

            Dominio.ObjetosDeValor.Embarcador.Integracao.HttpRequisicaoResposta httpRequisicaoResposta = new Dominio.ObjetosDeValor.Embarcador.Integracao.HttpRequisicaoResposta()
            {
                conteudoRequisicao = string.Empty,
                extensaoRequisicao = "xml",
                conteudoResposta = string.Empty,
                extensaoResposta = "xml",
                sucesso = false,
                mensagem = string.Empty,
            };

            try
            {
                InspectorBehavior inspector = new InspectorBehavior();
                if (cliente?.GrupoPessoas != null)
                {
                    if (notasFiscais.Count > 0)
                    {
                        ServicoSGT.Ocorrencia.OcorrenciasClient serOcorrenciasClient = ObterClientOcorrencia(cliente.GrupoPessoas.URLIntegracaoMultiEmbarcador, cliente.GrupoPessoas.TokenIntegracaoMultiEmbarcador);
                        serOcorrenciasClient.Endpoint.EndpointBehaviors.Add(inspector);

                        Dominio.ObjetosDeValor.Embarcador.Ocorrencia.Ocorrencia ocorrencia = new Dominio.ObjetosDeValor.Embarcador.Ocorrencia.Ocorrencia();
                        ocorrencia.DataOcorrencia = data.ToString("dd/MM/yyyy HH:mm:ss");
                        ocorrencia.TipoOcorrencia = new Dominio.ObjetosDeValor.Embarcador.Ocorrencia.TipoOcorrencia() { CodigoIntegracao = codigoTipoOcorrenciaIntegracao };
                        ocorrencia.Empresa = new Dominio.ObjetosDeValor.Embarcador.Pessoas.Empresa() { CNPJ = empresa.CNPJ_SemFormato };
                        ocorrencia.NumeroNotaFiscal = notasFiscais.FirstOrDefault().Numero;
                        ocorrencia.NumerosNotasFiscais = notasFiscais.Select(o => o.Numero).ToList();
                        ocorrencia.Remetente = new Dominio.ObjetosDeValor.Embarcador.Pessoas.Pessoa() { CPFCNPJ = notasFiscais.FirstOrDefault().Emitente.CPF_CNPJ_SemFormato };
                        ocorrencia.Destinatario = new Dominio.ObjetosDeValor.Embarcador.Pessoas.Pessoa() { CPFCNPJ = notasFiscais.FirstOrDefault().Destinatario.CPF_CNPJ_SemFormato };

                        Servicos.ServicoSGT.Ocorrencia.RetornoOfint retorno = serOcorrenciasClient.AdicionarOcorrencia(ocorrencia);
                        httpRequisicaoResposta.sucesso = retorno.Status;
                        httpRequisicaoResposta.mensagem = (retorno.Status) ? "Integrado com sucesso." : retorno.Mensagem;
                        httpRequisicaoResposta.conteudoRequisicao = string.IsNullOrEmpty(inspector.LastRequestXML) ? JsonSerializer.Serialize(ocorrencia) : "";
                        httpRequisicaoResposta.conteudoResposta = string.IsNullOrEmpty(inspector.LastResponseXML) ? JsonSerializer.Serialize(retorno) : "";
                        if (string.IsNullOrEmpty(inspector.LastRequestXML)) httpRequisicaoResposta.extensaoRequisicao = "json";
                        if (string.IsNullOrEmpty(inspector.LastResponseXML)) httpRequisicaoResposta.extensaoResposta = "json";
                    }
                    else
                    {
                        httpRequisicaoResposta.mensagem = "Não foram localizadas notas fiscais da ocorrência.";
                    }
                }
                else
                {
                    httpRequisicaoResposta.mensagem = "O tomador da ocorrência não está configurado para integrar com o MultiEmbarcador.";
                }
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);
                httpRequisicaoResposta.mensagem = "Ocorreu uma falha ao integrar: " + ex.Message;
            }

            return httpRequisicaoResposta;
        }

        public static ServicoSGT.Ocorrencia.OcorrenciasClient ObterClientOcorrencia(string url, string token)
        {
            url = url.ToLower();

            if (!url.EndsWith("/"))
                url += "/";

            url += "Ocorrencias.svc";

            ServicoSGT.Ocorrencia.OcorrenciasClient client = null;
            System.ServiceModel.EndpointAddress endpointAddress = new System.ServiceModel.EndpointAddress(url);

            if (url.StartsWith("https"))
            {
                System.ServiceModel.BasicHttpBinding binding = new System.ServiceModel.BasicHttpBinding();

                binding.MaxReceivedMessageSize = int.MaxValue;
                binding.ReceiveTimeout = new TimeSpan(0, 5, 0);
                binding.SendTimeout = new TimeSpan(0, 5, 0);

                binding.Security.Mode = System.ServiceModel.BasicHttpSecurityMode.Transport;

                client = new ServicoSGT.Ocorrencia.OcorrenciasClient(binding, endpointAddress);
            }
            else
            {
                System.ServiceModel.BasicHttpBinding binding = new System.ServiceModel.BasicHttpBinding();

                binding.MaxReceivedMessageSize = int.MaxValue;
                binding.ReceiveTimeout = new TimeSpan(0, 5, 0);
                binding.SendTimeout = new TimeSpan(0, 5, 0);

                client = new ServicoSGT.Ocorrencia.OcorrenciasClient(binding, endpointAddress);
            }

            System.ServiceModel.OperationContextScope scope = new System.ServiceModel.OperationContextScope(client.InnerChannel);
            System.ServiceModel.Channels.MessageHeader header = System.ServiceModel.Channels.MessageHeader.CreateHeader("Token", "Token", token);
            System.ServiceModel.OperationContext.Current.OutgoingMessageHeaders.Add(header);

            return client;
        }

        #endregion
    }
}