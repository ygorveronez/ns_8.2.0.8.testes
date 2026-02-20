using System;
using System.Collections.Generic;
using System.IO;
using System.Net;

namespace Servicos
{
    public class IntegracaoMultiEmbarcadorTMS
    {
        public static void IntegrarCTe(ref Dominio.Entidades.CTeIntegracaoRetorno cteIntegracaoRetorno, Repositorio.UnitOfWork unitOfWork)
        {
            ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;
            InspectorBehavior inspector = new InspectorBehavior();

            try
            {
                SGT.WebService.CTe.CTeClient svcCTe = ObterClientCTe(cteIntegracaoRetorno.CTe.Empresa.Configuracao.WsIntegracaoEnvioCTeEmbarcadorTMS, cteIntegracaoRetorno.CTe.Empresa.Configuracao.TokenIntegracaoEmbarcadorTMS);
                svcCTe.Endpoint.EndpointBehaviors.Add(inspector);

                Repositorio.XMLCTe repXMLCTe = new Repositorio.XMLCTe(unitOfWork);
                Repositorio.CTeIntegracaoRetornoLog repCTeIntegracaoRetornoLog = new Repositorio.CTeIntegracaoRetornoLog(unitOfWork);

                MemoryStream memoryStream = null;

                if (cteIntegracaoRetorno.CTe.Status == "A")
                {
                    Dominio.Entidades.XMLCTe xmlCTe = repXMLCTe.BuscarPorCTe(cteIntegracaoRetorno.CTe.Codigo, Dominio.Enumeradores.TipoXMLCTe.Autorizacao);
                    if (xmlCTe != null)
                    {
                        if (xmlCTe.XMLArmazenadoEmArquivo)
                        {
                            Servicos.CTe serCTe = new Servicos.CTe(unitOfWork);

                            string caminho = serCTe.ObterCaminhoArmazenamentoXMLCTeArquivo(xmlCTe.CTe, "A", unitOfWork);

                            if (Utilidades.IO.FileStorageService.Storage.Exists(caminho))
                            {
                                using (Stream file = Utilidades.IO.FileStorageService.Storage.OpenRead(caminho))
                                {
                                    memoryStream = new MemoryStream();
                                    file.CopyTo(memoryStream);
                                }
                            }
                        }
                        else
                        {
                            memoryStream = Utilidades.String.ToStream(xmlCTe.XML);
                        }
                    }
                }
                else
                {
                    Dominio.Entidades.XMLCTe xmlCTe = repXMLCTe.BuscarPorCTe(cteIntegracaoRetorno.CTe.Codigo, Dominio.Enumeradores.TipoXMLCTe.Cancelamento);
                    if (xmlCTe != null)
                    {
                        if (xmlCTe.XMLArmazenadoEmArquivo)
                        {
                            Servicos.CTe serCTe = new Servicos.CTe(unitOfWork);

                            string caminho = serCTe.ObterCaminhoArmazenamentoXMLCTeArquivo(xmlCTe.CTe, "C", unitOfWork);

                            if (Utilidades.IO.FileStorageService.Storage.Exists(caminho))
                            {
                                using (Stream file = Utilidades.IO.FileStorageService.Storage.OpenRead(caminho))
                                {
                                    memoryStream = new MemoryStream();
                                    file.CopyTo(memoryStream);
                                }
                            }
                        }
                        else
                        {
                            memoryStream = Utilidades.String.ToStream(xmlCTe.XML);
                        }
                    }
                }

                memoryStream.Position = 0;
                SGT.WebService.CTe.RetornoOfstring retorno = svcCTe.EnviarArquivoXMLCTe(memoryStream);
                if (retorno.Status)
                {
                    cteIntegracaoRetorno.DataIntegracao = DateTime.Now;
                    cteIntegracaoRetorno.SituacaoIntegracao = Dominio.Enumeradores.SituacaoCTeIntegracaoRetorno.Sucesso;
                    cteIntegracaoRetorno.ProblemaIntegracao = "Sucesso  " + retorno.Objeto;
                    cteIntegracaoRetorno.NumeroTentativas += 1;
                    cteIntegracaoRetorno.DataIntegracao = DateTime.Now;

                    Dominio.Entidades.CTeIntegracaoRetornoLog cteIntegracaoRetornoLog = new Dominio.Entidades.CTeIntegracaoRetornoLog();
                    cteIntegracaoRetornoLog.CTeIntegracaoRetorno = cteIntegracaoRetorno;
                    cteIntegracaoRetornoLog.Data = DateTime.Now;
                    cteIntegracaoRetornoLog.Mensagem = "Sucesso  " + retorno.Objeto;
                    cteIntegracaoRetornoLog.Request = inspector.LastRequestXML != null ? inspector.LastRequestXML : string.Empty;
                    cteIntegracaoRetornoLog.Response = inspector.LastResponseXML != null ? inspector.LastResponseXML : string.Empty;
                    repCTeIntegracaoRetornoLog.Inserir(cteIntegracaoRetornoLog);
                }
                else
                {
                    cteIntegracaoRetorno.SituacaoIntegracao = Dominio.Enumeradores.SituacaoCTeIntegracaoRetorno.Falha;
                    cteIntegracaoRetorno.ProblemaIntegracao = retorno.Mensagem;
                    cteIntegracaoRetorno.NumeroTentativas += 1;
                    cteIntegracaoRetorno.DataIntegracao = DateTime.Now;

                    Dominio.Entidades.CTeIntegracaoRetornoLog cteIntegracaoRetornoLog = new Dominio.Entidades.CTeIntegracaoRetornoLog();
                    cteIntegracaoRetornoLog.CTeIntegracaoRetorno = cteIntegracaoRetorno;
                    cteIntegracaoRetornoLog.Data = DateTime.Now;
                    cteIntegracaoRetornoLog.Mensagem = retorno.Mensagem;
                    cteIntegracaoRetornoLog.Request = inspector.LastRequestXML != null ? inspector.LastRequestXML : string.Empty;
                    cteIntegracaoRetornoLog.Response = inspector.LastResponseXML != null ? inspector.LastResponseXML : string.Empty;
                    repCTeIntegracaoRetornoLog.Inserir(cteIntegracaoRetornoLog);
                }
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro("IntegrarCTe: " + ex, "IntegracaoMultiEmbarcadorTMS");
                if (inspector?.LastRequestXML != null)
                    Servicos.Log.TratarErro(inspector.LastRequestXML, "IntegracaoMultiEmbarcadorTMS");
                if (inspector?.LastResponseXML != null)
                    Servicos.Log.TratarErro(inspector.LastResponseXML, "IntegracaoMultiEmbarcadorTMS");

                cteIntegracaoRetorno.NumeroTentativas += 1;
                cteIntegracaoRetorno.ProblemaIntegracao = "Falha ao retornar integração";
                cteIntegracaoRetorno.SituacaoIntegracao = Dominio.Enumeradores.SituacaoCTeIntegracaoRetorno.Falha;
            }

        }


        public static void IntegrarNFSe(ref Dominio.Entidades.NFSeIntegracaoRetorno nfseIntegracaoRetorno, Repositorio.UnitOfWork unitOfWork)
        {
            ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;

            InspectorBehavior inspector = new InspectorBehavior();
            try
            {
                SGT.WebService.NFS.NFSClient svcNFS = ObterClientNFSe(nfseIntegracaoRetorno.NFSe.Empresa.Configuracao.WsIntegracaoEnvioNFSeEmbarcadorTMS, nfseIntegracaoRetorno.NFSe.Empresa.Configuracao.TokenIntegracaoEmbarcadorTMS);
                svcNFS.Endpoint.EndpointBehaviors.Add(inspector);

                Repositorio.NFSeIntegracaoRetornoLog repNFSeIntegracaoRetornoLog = new Repositorio.NFSeIntegracaoRetornoLog(unitOfWork);

                Dominio.ObjetosDeValor.WebService.NFS.NFS nfse = new Dominio.ObjetosDeValor.WebService.NFS.NFS();

                nfse.TransportadoraEmitente = new Dominio.ObjetosDeValor.Embarcador.Pessoas.Empresa()
                {
                    CNPJ = nfseIntegracaoRetorno.NFSe.Empresa.CNPJ
                };

                nfse.Tomador = new Dominio.ObjetosDeValor.Embarcador.Pessoas.Pessoa()
                {
                    CPFCNPJ = nfseIntegracaoRetorno.NFSe.Tomador.CPF_CNPJ,
                    NomeFantasia = nfseIntegracaoRetorno.NFSe.Tomador.NomeFantasia,
                    RazaoSocial = nfseIntegracaoRetorno.NFSe.Tomador.Nome,
                    Endereco = new Dominio.ObjetosDeValor.Embarcador.Localidade.Endereco()
                    {
                        Logradouro = nfseIntegracaoRetorno.NFSe.Tomador.Endereco,
                        Bairro = nfseIntegracaoRetorno.NFSe.Tomador.Bairro,
                        CEP = nfseIntegracaoRetorno.NFSe.Tomador.CEP,
                        Numero = nfseIntegracaoRetorno.NFSe.Tomador.Numero,
                        Cidade = new Dominio.ObjetosDeValor.Localidade()
                        {
                            IBGE = nfseIntegracaoRetorno.NFSe.Tomador.Localidade.CodigoIBGE
                        },
                    },
                    CodigoAtividade = nfseIntegracaoRetorno.NFSe.Tomador.Atividade.Codigo,
                    AtualizarEnderecoPessoa = false,
                    RGIE = nfseIntegracaoRetorno.NFSe.Tomador.IE_RG
                };

                nfse.ValoresNFS = new Dominio.ObjetosDeValor.WebService.NFS.ValorNFS()
                {
                    AliquotaISS = nfseIntegracaoRetorno.NFSe.AliquotaISS,
                    ValorServicos = nfseIntegracaoRetorno.NFSe.ValorServicos,
                    BaseCalculoISS = nfseIntegracaoRetorno.NFSe.BaseCalculoISS,
                    ValorCOFINS = nfseIntegracaoRetorno.NFSe.ValorCOFINS,
                    ValorDeducoes = nfseIntegracaoRetorno.NFSe.ValorDeducoes,
                    ValorCSLL = nfseIntegracaoRetorno.NFSe.ValorCSLL,
                    ValorDescontoCondicionado = nfseIntegracaoRetorno.NFSe.ValorDescontoCondicionado,
                    ValorDescontoIncondicionado = nfseIntegracaoRetorno.NFSe.ValorDescontoIncondicionado,
                    ValorINSS = nfseIntegracaoRetorno.NFSe.ValorINSS,
                    ValorIR = nfseIntegracaoRetorno.NFSe.ValorIR,
                    ValorISS = nfseIntegracaoRetorno.NFSe.ValorISS,
                    ValorPIS = nfseIntegracaoRetorno.NFSe.ValorPIS,
                    ValorISSRetido = nfseIntegracaoRetorno.NFSe.ValorISSRetido,
                    ValorOutrasRetencoes = nfseIntegracaoRetorno.NFSe.ValorOutrasRetencoes,
                    ISSRetido = nfseIntegracaoRetorno.NFSe.ISSRetido
                };

                nfse.NFSe = new Dominio.ObjetosDeValor.WebService.NFS.NFSe()
                {
                    DataEmissao = nfseIntegracaoRetorno.NFSe.DataEmissao.ToString("dd/MM/yyyy HH:mm:ss"),
                    Numero = nfseIntegracaoRetorno.NFSe.Numero,
                    CodigoVerificacao = nfseIntegracaoRetorno.NFSe.CodigoVerificacao,
                    Serie = nfseIntegracaoRetorno.NFSe.Serie.Numero,
                    NumeroRPS = nfseIntegracaoRetorno.NFSe.RPS?.Numero ?? 0,
                    SerieRPS = nfseIntegracaoRetorno.NFSe.RPS?.Serie ?? string.Empty
                };

                if (nfseIntegracaoRetorno.NFSe.Status == Dominio.Enumeradores.StatusNFSe.Cancelado)
                    nfse.Cancelada = true;

                if (nfseIntegracaoRetorno.NFSe.Documentos != null && nfseIntegracaoRetorno.NFSe.Documentos.Count > 0)
                {
                    nfse.NFSe.Documentos = new List<Dominio.ObjetosDeValor.WebService.CTe.DocumentosCTe>();

                    foreach (var documento in nfseIntegracaoRetorno.NFSe.Documentos)
                    {
                        Dominio.ObjetosDeValor.WebService.CTe.DocumentosCTe doc = new Dominio.ObjetosDeValor.WebService.CTe.DocumentosCTe()
                        {
                            ChaveNFe = documento.Chave,
                            Numero = documento.Numero,
                            Serie = documento.Serie
                        };
                        nfse.NFSe.Documentos.Add(doc);
                    }
                }

                Dominio.ObjetosDeValor.WebService.NFS.NFS[] notasRetorno = new Dominio.ObjetosDeValor.WebService.NFS.NFS[] { nfse };

                var retorno = svcNFS.EnviarNFSe(notasRetorno);

                if (retorno.Status)
                {
                    nfseIntegracaoRetorno.DataIntegracao = DateTime.Now;
                    nfseIntegracaoRetorno.SituacaoIntegracao = Dominio.Enumeradores.SituacaoCTeIntegracaoRetorno.Sucesso;
                    nfseIntegracaoRetorno.ProblemaIntegracao = string.Empty;
                    nfseIntegracaoRetorno.NumeroTentativas += 1;
                    nfseIntegracaoRetorno.DataIntegracao = DateTime.Now;

                    Dominio.Entidades.NFSeIntegracaoRetornoLog nfseIntegracaoRetornoLog = new Dominio.Entidades.NFSeIntegracaoRetornoLog();
                    nfseIntegracaoRetornoLog.NFSeIntegracaoRetorno = nfseIntegracaoRetorno;
                    nfseIntegracaoRetornoLog.Data = DateTime.Now;
                    nfseIntegracaoRetornoLog.Mensagem = "Sucesso ";
                    nfseIntegracaoRetornoLog.Request = inspector.LastRequestXML != null ? inspector.LastRequestXML : string.Empty;
                    nfseIntegracaoRetornoLog.Response = inspector.LastResponseXML != null ? inspector.LastResponseXML : string.Empty;
                    repNFSeIntegracaoRetornoLog.Inserir(nfseIntegracaoRetornoLog);
                }
                else
                {
                    nfseIntegracaoRetorno.SituacaoIntegracao = Dominio.Enumeradores.SituacaoCTeIntegracaoRetorno.Falha;
                    nfseIntegracaoRetorno.ProblemaIntegracao = retorno.Mensagem;
                    nfseIntegracaoRetorno.NumeroTentativas += 1;
                    nfseIntegracaoRetorno.DataIntegracao = DateTime.Now;

                    Dominio.Entidades.NFSeIntegracaoRetornoLog nfseIntegracaoRetornoLog = new Dominio.Entidades.NFSeIntegracaoRetornoLog();
                    nfseIntegracaoRetornoLog.NFSeIntegracaoRetorno = nfseIntegracaoRetorno;
                    nfseIntegracaoRetornoLog.Data = DateTime.Now;
                    nfseIntegracaoRetornoLog.Mensagem = retorno.Mensagem;
                    nfseIntegracaoRetornoLog.Request = inspector.LastRequestXML != null ? inspector.LastRequestXML : string.Empty;
                    nfseIntegracaoRetornoLog.Response = inspector.LastResponseXML != null ? inspector.LastResponseXML : string.Empty;
                    repNFSeIntegracaoRetornoLog.Inserir(nfseIntegracaoRetornoLog);
                }
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro("IntegrarNFSe: " + ex, "IntegracaoMultiEmbarcadorTMS");
                if (inspector?.LastRequestXML != null)
                    Servicos.Log.TratarErro(inspector.LastRequestXML, "IntegracaoMultiEmbarcadorTMS");
                if (inspector?.LastResponseXML != null)
                    Servicos.Log.TratarErro(inspector.LastResponseXML, "IntegracaoMultiEmbarcadorTMS");

                nfseIntegracaoRetorno.NumeroTentativas += 1;
                nfseIntegracaoRetorno.ProblemaIntegracao = "Falha ao retornar integração";
                nfseIntegracaoRetorno.SituacaoIntegracao = Dominio.Enumeradores.SituacaoCTeIntegracaoRetorno.Falha;
            }

        }


        public static void IntegrarMDFe(ref Dominio.Entidades.MDFeIntegracaoRetorno mdfeIntegracaoRetorno, Repositorio.UnitOfWork unitOfWork)
        {
            ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;
            InspectorBehavior inspector = new InspectorBehavior();

            try
            {
                SGT.WebService.MDFe.MDFeClient svcMDFe = ObterClientMDFe(mdfeIntegracaoRetorno.MDFe.Empresa.Configuracao.WsIntegracaoEnvioMDFeEmbarcadorTMS, mdfeIntegracaoRetorno.MDFe.Empresa.Configuracao.TokenIntegracaoEmbarcadorTMS);
                svcMDFe.Endpoint.EndpointBehaviors.Add(inspector);

                Repositorio.XMLMDFe repXMLMDFe = new Repositorio.XMLMDFe(unitOfWork);
                Repositorio.MDFeIntegracaoRetornoLog repMDFeIntegracaoRetornoLog = new Repositorio.MDFeIntegracaoRetornoLog(unitOfWork);

                MemoryStream memoryStream = null;

                if (mdfeIntegracaoRetorno.MDFe.Status == Dominio.Enumeradores.StatusMDFe.Autorizado)
                {
                    Dominio.Entidades.XMLMDFe xmlMDFe = repXMLMDFe.BuscarPorMDFe(mdfeIntegracaoRetorno.MDFe.Codigo, Dominio.Enumeradores.TipoXMLMDFe.Autorizacao);
                    if (xmlMDFe != null)
                        memoryStream = Utilidades.String.ToStream(xmlMDFe.XML);
                }
                else if (mdfeIntegracaoRetorno.MDFe.Status == Dominio.Enumeradores.StatusMDFe.Cancelado)
                {
                    Dominio.Entidades.XMLMDFe xmlMDFe = repXMLMDFe.BuscarPorMDFe(mdfeIntegracaoRetorno.MDFe.Codigo, Dominio.Enumeradores.TipoXMLMDFe.Cancelamento);
                    if (xmlMDFe != null)
                        memoryStream = Utilidades.String.ToStream(xmlMDFe.XML);
                }
                else if (mdfeIntegracaoRetorno.MDFe.Status == Dominio.Enumeradores.StatusMDFe.Encerrado)
                {
                    Dominio.Entidades.XMLMDFe xmlMDFe = repXMLMDFe.BuscarPorMDFe(mdfeIntegracaoRetorno.MDFe.Codigo, Dominio.Enumeradores.TipoXMLMDFe.Encerramento);
                    if (xmlMDFe != null)
                        memoryStream = Utilidades.String.ToStream(xmlMDFe.XML);
                }

                memoryStream.Position = 0;
                SGT.WebService.MDFe.RetornoOfstring retorno = svcMDFe.EnviarArquivoXMLMDFe(memoryStream);
                if (retorno.Status)
                {
                    mdfeIntegracaoRetorno.DataIntegracao = DateTime.Now;
                    mdfeIntegracaoRetorno.SituacaoIntegracao = Dominio.Enumeradores.SituacaoCTeIntegracaoRetorno.Sucesso;
                    mdfeIntegracaoRetorno.ProblemaIntegracao = "Sucesso  " + retorno.Objeto;
                    mdfeIntegracaoRetorno.NumeroTentativas += 1;
                    mdfeIntegracaoRetorno.DataIntegracao = DateTime.Now;

                    Dominio.Entidades.MDFeIntegracaoRetornoLog mdfeIntegracaoRetornoLog = new Dominio.Entidades.MDFeIntegracaoRetornoLog();
                    mdfeIntegracaoRetornoLog.MDFeIntegracaoRetorno = mdfeIntegracaoRetorno;
                    mdfeIntegracaoRetornoLog.Data = DateTime.Now;
                    mdfeIntegracaoRetornoLog.Mensagem = "Sucesso  " + retorno.Objeto;
                    mdfeIntegracaoRetornoLog.Request = inspector.LastRequestXML != null ? inspector.LastRequestXML : string.Empty;
                    mdfeIntegracaoRetornoLog.Response = inspector.LastResponseXML != null ? inspector.LastResponseXML : string.Empty;
                    repMDFeIntegracaoRetornoLog.Inserir(mdfeIntegracaoRetornoLog);
                }
                else
                {
                    mdfeIntegracaoRetorno.SituacaoIntegracao = Dominio.Enumeradores.SituacaoCTeIntegracaoRetorno.Falha;
                    mdfeIntegracaoRetorno.ProblemaIntegracao = retorno.Mensagem;
                    mdfeIntegracaoRetorno.NumeroTentativas += 1;
                    mdfeIntegracaoRetorno.DataIntegracao = DateTime.Now;

                    Dominio.Entidades.MDFeIntegracaoRetornoLog mdfeIntegracaoRetornoLog = new Dominio.Entidades.MDFeIntegracaoRetornoLog();
                    mdfeIntegracaoRetornoLog.MDFeIntegracaoRetorno = mdfeIntegracaoRetorno;
                    mdfeIntegracaoRetornoLog.Data = DateTime.Now;
                    mdfeIntegracaoRetornoLog.Mensagem = retorno.Mensagem;
                    mdfeIntegracaoRetornoLog.Request = inspector.LastRequestXML != null ? inspector.LastRequestXML : string.Empty;
                    mdfeIntegracaoRetornoLog.Response = inspector.LastResponseXML != null ? inspector.LastResponseXML : string.Empty;
                    repMDFeIntegracaoRetornoLog.Inserir(mdfeIntegracaoRetornoLog);
                }
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro("IntegrarMDFe: " + ex, "IntegracaoMultiEmbarcadorTMS");
                if (inspector?.LastRequestXML != null)
                    Servicos.Log.TratarErro(inspector.LastRequestXML, "IntegracaoMultiEmbarcadorTMS");
                if (inspector?.LastResponseXML != null)
                    Servicos.Log.TratarErro(inspector.LastResponseXML, "IntegracaoMultiEmbarcadorTMS");

                mdfeIntegracaoRetorno.NumeroTentativas += 1;
                mdfeIntegracaoRetorno.ProblemaIntegracao = "Falha ao retornar integração";
                mdfeIntegracaoRetorno.SituacaoIntegracao = Dominio.Enumeradores.SituacaoCTeIntegracaoRetorno.Falha;
            }

        }


        public static SGT.WebService.CTe.CTeClient ObterClientCTe(string url, string token)
        {
            url = url.ToLower();

            if (!url.EndsWith("/"))
                url += "/";

            SGT.WebService.CTe.CTeClient client = null;
            System.ServiceModel.EndpointAddress endpointAddress = new System.ServiceModel.EndpointAddress(url);

            if (url.StartsWith("https"))
            {
                //System.ServiceModel.WSHttpBinding binding = new System.ServiceModel.WSHttpBinding(System.ServiceModel.SecurityMode.None);
                System.ServiceModel.BasicHttpBinding binding = new System.ServiceModel.BasicHttpBinding();

                binding.MaxReceivedMessageSize = int.MaxValue;
                binding.ReceiveTimeout = new TimeSpan(0, 5, 0);
                binding.SendTimeout = new TimeSpan(0, 5, 0);

                binding.Security.Mode = System.ServiceModel.BasicHttpSecurityMode.Transport;
                //binding.Security.Message.ClientCredentialType = System.ServiceModel.BasicHttpMessageCredentialType.Certificate;

                client = new SGT.WebService.CTe.CTeClient(binding, endpointAddress);
            }
            else
            {
                System.ServiceModel.BasicHttpBinding binding = new System.ServiceModel.BasicHttpBinding();

                binding.MaxReceivedMessageSize = int.MaxValue;
                binding.ReceiveTimeout = new TimeSpan(0, 5, 0);
                binding.SendTimeout = new TimeSpan(0, 5, 0);

                client = new SGT.WebService.CTe.CTeClient(binding, endpointAddress);
            }

            System.ServiceModel.OperationContextScope scope = new System.ServiceModel.OperationContextScope(client.InnerChannel);
            System.ServiceModel.Channels.MessageHeader header = System.ServiceModel.Channels.MessageHeader.CreateHeader("Token", "Token", token);
            System.ServiceModel.OperationContext.Current.OutgoingMessageHeaders.Add(header);

            return client;
        }

        public static SGT.WebService.NFS.NFSClient ObterClientNFSe(string url, string token)
        {
            url = url.ToLower();

            if (!url.EndsWith("/"))
                url += "/";

            SGT.WebService.NFS.NFSClient client = null;
            System.ServiceModel.EndpointAddress endpointAddress = new System.ServiceModel.EndpointAddress(url);

            if (url.StartsWith("https"))
            {
                //System.ServiceModel.WSHttpBinding binding = new System.ServiceModel.WSHttpBinding(System.ServiceModel.SecurityMode.None);
                System.ServiceModel.BasicHttpBinding binding = new System.ServiceModel.BasicHttpBinding();

                binding.MaxReceivedMessageSize = int.MaxValue;
                binding.ReceiveTimeout = new TimeSpan(0, 5, 0);
                binding.SendTimeout = new TimeSpan(0, 5, 0);

                binding.Security.Mode = System.ServiceModel.BasicHttpSecurityMode.Transport;
                //binding.Security.Message.ClientCredentialType = System.ServiceModel.BasicHttpMessageCredentialType.Certificate;

                client = new SGT.WebService.NFS.NFSClient(binding, endpointAddress);
            }
            else
            {
                System.ServiceModel.BasicHttpBinding binding = new System.ServiceModel.BasicHttpBinding();

                binding.MaxReceivedMessageSize = int.MaxValue;
                binding.ReceiveTimeout = new TimeSpan(0, 5, 0);
                binding.SendTimeout = new TimeSpan(0, 5, 0);

                client = new SGT.WebService.NFS.NFSClient(binding, endpointAddress);
            }

            System.ServiceModel.OperationContextScope scope = new System.ServiceModel.OperationContextScope(client.InnerChannel);
            System.ServiceModel.Channels.MessageHeader header = System.ServiceModel.Channels.MessageHeader.CreateHeader("Token", "Token", token);
            System.ServiceModel.OperationContext.Current.OutgoingMessageHeaders.Add(header);

            return client;
        }

        public static SGT.WebService.MDFe.MDFeClient ObterClientMDFe(string url, string token)
        {
            url = url.ToLower();

            if (!url.EndsWith("/"))
                url += "/";

            SGT.WebService.MDFe.MDFeClient client = null;
            System.ServiceModel.EndpointAddress endpointAddress = new System.ServiceModel.EndpointAddress(url);

            if (url.StartsWith("https"))
            {
                //System.ServiceModel.WSHttpBinding binding = new System.ServiceModel.WSHttpBinding(System.ServiceModel.SecurityMode.None);
                System.ServiceModel.BasicHttpBinding binding = new System.ServiceModel.BasicHttpBinding();

                binding.MaxReceivedMessageSize = int.MaxValue;
                binding.ReceiveTimeout = new TimeSpan(0, 5, 0);
                binding.SendTimeout = new TimeSpan(0, 5, 0);

                binding.Security.Mode = System.ServiceModel.BasicHttpSecurityMode.Transport;
                //binding.Security.Message.ClientCredentialType = System.ServiceModel.BasicHttpMessageCredentialType.Certificate;

                client = new SGT.WebService.MDFe.MDFeClient(binding, endpointAddress);
            }
            else
            {
                System.ServiceModel.BasicHttpBinding binding = new System.ServiceModel.BasicHttpBinding();

                binding.MaxReceivedMessageSize = int.MaxValue;
                binding.ReceiveTimeout = new TimeSpan(0, 5, 0);
                binding.SendTimeout = new TimeSpan(0, 5, 0);

                client = new SGT.WebService.MDFe.MDFeClient(binding, endpointAddress);
            }

            System.ServiceModel.OperationContextScope scope = new System.ServiceModel.OperationContextScope(client.InnerChannel);
            System.ServiceModel.Channels.MessageHeader header = System.ServiceModel.Channels.MessageHeader.CreateHeader("Token", "Token", token);
            System.ServiceModel.OperationContext.Current.OutgoingMessageHeaders.Add(header);

            return client;
        }
    }
}
