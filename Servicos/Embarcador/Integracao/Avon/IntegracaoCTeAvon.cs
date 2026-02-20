using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Text;
using Dominio.Entidades.Embarcador.Cargas;
using Dominio.ObjetosDeValor.CrossTalk;
using Dominio.ObjetosDeValor.Embarcador.Integracao.Avon;
using Repositorio;
using Servicos.ServicoAvon;

namespace Servicos.Embarcador.Integracao.Avon
{
    public class IntegracaoCTeAvon
    {
        public static void ConsultarRetorno(Dominio.Entidades.Embarcador.NFS.NFSManualIntegracaoLote lote, List<Dominio.Entidades.Embarcador.NFS.NFSManualCTeIntegracao> ctesDoLote, Repositorio.UnitOfWork unidadeDeTrabalho)
        {
            //Dominio.ObjetosDeValor.Embarcador.Integracao.Avon.InformacoesIntegracaoAvon informacoes = ConsultarRetorno(ctesDoLote.First().CargaCTe.Carga.Empresa, lote.CodigoConsultaIntegradora, unidadeDeTrabalho);
            Dominio.ObjetosDeValor.Embarcador.Integracao.Avon.InformacoesIntegracaoAvon informacoes = ConsultarRetorno(ctesDoLote.Select(o => o.LancamentoNFSManual.CTe).ToList(), unidadeDeTrabalho);
            AtualizarDocumentosLoteConsultaRetorno(informacoes, lote, ctesDoLote, informacoes.Response, unidadeDeTrabalho);
        }

        public static void ConsultarRetorno(Dominio.Entidades.Embarcador.Ocorrencias.OcorrenciaCTeIntegracaoLote lote, List<Dominio.Entidades.Embarcador.Ocorrencias.OcorrenciaCTeIntegracao> ctesDoLote, Repositorio.UnitOfWork unidadeDeTrabalho)
        {
            //Dominio.ObjetosDeValor.Embarcador.Integracao.Avon.InformacoesIntegracaoAvon informacoes = ConsultarRetorno(ctesDoLote.First().CargaCTe.Carga.Empresa, lote.CodigoConsultaIntegradora, unidadeDeTrabalho);
            Dominio.ObjetosDeValor.Embarcador.Integracao.Avon.InformacoesIntegracaoAvon informacoes = ConsultarRetorno(ctesDoLote.Select(o => o.CargaCTe.CTe).ToList(), unidadeDeTrabalho);
            AtualizarDocumentosLoteConsultaRetorno(informacoes, lote, ctesDoLote, informacoes.Response, unidadeDeTrabalho);
        }

        public static void ConsultarRetorno(Dominio.Entidades.Embarcador.Cargas.CargaCTeIntegracaoLote lote, List<Dominio.Entidades.Embarcador.Cargas.CargaCTeIntegracao> ctesDoLote, Repositorio.UnitOfWork unidadeDeTrabalho)
        {
            //Dominio.ObjetosDeValor.Embarcador.Integracao.Avon.InformacoesIntegracaoAvon informacoes = ConsultarRetorno(ctesDoLote.First().CargaCTe.Carga.Empresa, lote.CodigoConsultaIntegradora, unidadeDeTrabalho);
            Dominio.ObjetosDeValor.Embarcador.Integracao.Avon.InformacoesIntegracaoAvon informacoes = ConsultarRetorno(ctesDoLote.Select(o => o.CargaCTe.CTe).ToList(), unidadeDeTrabalho);
            AtualizarDocumentosLoteConsultaRetorno(informacoes, lote, ctesDoLote, informacoes.Response, unidadeDeTrabalho);
        }

        public static void EnviarRetorno(Dominio.Entidades.Embarcador.Ocorrencias.OcorrenciaCTeIntegracaoLote lote, List<Dominio.Entidades.Embarcador.Ocorrencias.OcorrenciaCTeIntegracao> ctesDoLote, Repositorio.UnitOfWork unidadeDeTrabalho)
        {
            int[] ctes = ctesDoLote.Where(o => o.SituacaoIntegracao == Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoIntegracao.ProblemaIntegracao || o.SituacaoIntegracao == Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoIntegracao.AgIntegracao).Select(o => o.CargaCTe.CTe.Codigo).ToArray();
            Dominio.ObjetosDeValor.Embarcador.Integracao.Avon.InformacoesIntegracaoAvon informacoes = EnviarRetorno(ctesDoLote.First().CargaCTe.Carga.Empresa, ctes, unidadeDeTrabalho);

            AtualizarDocumentosLoteEnvioRetorno(informacoes, lote, ctesDoLote, unidadeDeTrabalho);
        }

        public static void EnviarRetorno(Dominio.Entidades.Embarcador.NFS.NFSManualIntegracaoLote lote, List<Dominio.Entidades.Embarcador.NFS.NFSManualCTeIntegracao> ctesDoLote, Repositorio.UnitOfWork unidadeDeTrabalho)
        {
            int[] ctes = ctesDoLote.Where(o => o.SituacaoIntegracao == Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoIntegracao.ProblemaIntegracao || o.SituacaoIntegracao == Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoIntegracao.AgIntegracao).Select(o => o.LancamentoNFSManual.CTe.Codigo).ToArray();
            Dominio.ObjetosDeValor.Embarcador.Integracao.Avon.InformacoesIntegracaoAvon informacoes = EnviarRetorno(ctesDoLote.First().LancamentoNFSManual.Transportador, ctes, unidadeDeTrabalho);

            AtualizarDocumentosLoteEnvioRetorno(informacoes, lote, ctesDoLote, unidadeDeTrabalho);
        }

        public static void EnviarRetorno(Dominio.Entidades.Embarcador.Cargas.CargaCTeIntegracaoLote lote, List<Dominio.Entidades.Embarcador.Cargas.CargaCTeIntegracao> ctesDoLote, Repositorio.UnitOfWork unidadeDeTrabalho)
        {
            int[] ctes = ctesDoLote.Where(o => o.SituacaoIntegracao == Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoIntegracao.ProblemaIntegracao || o.SituacaoIntegracao == Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoIntegracao.AgIntegracao).Select(o => o.CargaCTe.CTe.Codigo).ToArray();
            Dominio.ObjetosDeValor.Embarcador.Integracao.Avon.InformacoesIntegracaoAvon informacoes = EnviarRetorno(ctesDoLote.First().CargaCTe.Carga.Empresa, ctes, unidadeDeTrabalho);
            AtualizarDocumentosLoteEnvioRetorno(informacoes, lote, ctesDoLote, unidadeDeTrabalho);
        }

        //[Obsolete]
        //private static Dominio.ObjetosDeValor.Embarcador.Integracao.Avon.InformacoesIntegracaoAvon ConsultarRetorno(Dominio.Entidades.Empresa empresa, string codigoConsultaIntegradora, Repositorio.UnitOfWork unidadeDeTrabalho)
        //{
        //    Dominio.ObjetosDeValor.Embarcador.Integracao.Avon.InformacoesIntegracaoAvon informacoes = new Dominio.ObjetosDeValor.Embarcador.Integracao.Avon.InformacoesIntegracaoAvon();

        //    Repositorio.Embarcador.Configuracoes.Integracao repConfiguracaoIntegracao = new Repositorio.Embarcador.Configuracoes.Integracao(unidadeDeTrabalho);

        //    Dominio.Entidades.Embarcador.Configuracoes.Integracao configuracaoIntegracao = repConfiguracaoIntegracao.Buscar();

        //    if (configuracaoIntegracao == null || string.IsNullOrWhiteSpace(configuracaoIntegracao.EnterpriseIdAvon) || string.IsNullOrWhiteSpace(configuracaoIntegracao.TokenAvonProducao))
        //    {
        //        informacoes.Mensagem = "A configuração de integração para a Avon é inválida.";
        //        return informacoes;
        //    }

        //    string endpoint, token;

        //    InspectorBehavior inspector = new InspectorBehavior();
        //    ServicoAvon.RequestSoapClient svcAvon = new ServicoAvon.RequestSoapClient();

        //    svcAvon.Endpoint.EndpointBehaviors.Add(inspector);

        //    if (empresa.TipoAmbiente == Dominio.Enumeradores.TipoAmbiente.Producao)
        //    {
        //        token = configuracaoIntegracao.TokenAvonProducao;
        //        endpoint = "http://portalnfe.avon.com/Infinitri.WebService.ExchangeMessage/ExchangeMessage.asmx";
        //    }
        //    else
        //    {
        //        token = configuracaoIntegracao.TokenAvonHomologacao;
        //        endpoint = "http://portalnfeqa.avon.com/Infinitri.WebService.ExchangeMessage/ExchangeMessage.asmx";
        //    }

        //    svcAvon.Endpoint.Address = new System.ServiceModel.EndpointAddress(endpoint);

        //    Dominio.ObjetosDeValor.CrossTalk.CrossTalk_Message mensagem = new Dominio.ObjetosDeValor.CrossTalk.CrossTalk_Message()
        //    {
        //        CrossTalk_Header = new Dominio.ObjetosDeValor.CrossTalk.CrossTalk_Header()
        //        {
        //            ProcessCode = "10005",
        //            MessageType = "100",
        //            ExchangePattern = "8",
        //            EnterpriseId = configuracaoIntegracao.EnterpriseIdAvon,
        //            Token = token,
        //            GUID = codigoConsultaIntegradora
        //        }
        //    };

        //    string request = Servicos.XML.ConvertObjectToXMLString<Dominio.ObjetosDeValor.CrossTalk.CrossTalk_Message>(mensagem);

        //    string response = svcAvon.CompressedSend(request, null);

        //    Dominio.ObjetosDeValor.CrossTalk.CrossTalk_Message retorno = Servicos.XML.ConvertXMLStringToObject<Dominio.ObjetosDeValor.CrossTalk.CrossTalk_Message>(response);

        //    informacoes.CodigoMensagem = retorno.CrossTalk_Header.ResponseCode;
        //    informacoes.GUID = retorno.CrossTalk_Header.GUID;
        //    informacoes.Mensagem = retorno.CrossTalk_Header.ResponseCodeMessage;
        //    informacoes.Requisicao = inspector.LastRequestXML;
        //    informacoes.Resposta = inspector.LastResponseXML;
        //    informacoes.Response = retorno;
        //    return informacoes;
        //}

        private static Dominio.ObjetosDeValor.Embarcador.Integracao.Avon.InformacoesIntegracaoAvon ConsultarRetorno(List<Dominio.Entidades.ConhecimentoDeTransporteEletronico> ctesDoLote, Repositorio.UnitOfWork unidadeDeTrabalho)
        {
            Dominio.Entidades.Empresa empresa = ctesDoLote.First().Empresa;

            Dominio.ObjetosDeValor.Embarcador.Integracao.Avon.InformacoesIntegracaoAvon informacoes = new Dominio.ObjetosDeValor.Embarcador.Integracao.Avon.InformacoesIntegracaoAvon();

            Repositorio.Embarcador.Configuracoes.IntegracaoAvon repConfiguracaoIntegracaoAvon = new Repositorio.Embarcador.Configuracoes.IntegracaoAvon(unidadeDeTrabalho);

            Dominio.Entidades.Embarcador.Configuracoes.IntegracaoAvon configuracaoIntegracaoAvon = repConfiguracaoIntegracaoAvon.BuscarPorEmpresa(empresa.Codigo);

            if (configuracaoIntegracaoAvon == null || string.IsNullOrWhiteSpace(configuracaoIntegracaoAvon.EnterpriseID) || string.IsNullOrWhiteSpace(configuracaoIntegracaoAvon.TokenProducao))
            {
                informacoes.Mensagem = $"A configuração de integração da empresa {empresa.Descricao} para a Avon é inválida.";
                return informacoes;
            }

            string endpoint, token;

            ServicoAvon.RequestSoapClient svcAvon = new Servicos.Embarcador.Integracao.ConfiguracaoWebService(unidadeDeTrabalho).ObterClient< ServicoAvon.RequestSoapClient, ServicoAvon.RequestSoap>(Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoWebServiceIntegracao.Avon_Request, out Servicos.Models.Integracao.InspectorBehavior inspector);

            if (empresa.TipoAmbiente == Dominio.Enumeradores.TipoAmbiente.Producao)
            {
                token = configuracaoIntegracaoAvon.TokenProducao;
                endpoint = "http://portalnfe.avon.com/Infinitri.WebService.ExchangeMessage/ExchangeMessage.asmx";
            }
            else
            {
                token = configuracaoIntegracaoAvon.TokenHomologacao;
                endpoint = "http://portalnfeqa.avon.com/Infinitri.WebService.ExchangeMessage/ExchangeMessage.asmx";
            }

            svcAvon.Endpoint.Address = new System.ServiceModel.EndpointAddress(endpoint);

            Dominio.ObjetosDeValor.CrossTalk.CrossTalk_Message mensagem = new Dominio.ObjetosDeValor.CrossTalk.CrossTalk_Message()
            {
                CrossTalk_Header = new Dominio.ObjetosDeValor.CrossTalk.CrossTalk_Header()
                {
                    ProcessCode = "10005",
                    MessageType = "103",
                    ExchangePattern = "1",
                    EnterpriseId = configuracaoIntegracaoAvon.EnterpriseID,
                    Token = token
                },
                CrossTalk_Body = new CrossTalk_Body
                {
                    CTes = (from obj in ctesDoLote select new Dominio.ObjetosDeValor.CrossTalk.CTe() { AccessKey = obj.Chave }).ToArray()
                }
            };

            string request = Servicos.XML.ConvertObjectToXMLString<Dominio.ObjetosDeValor.CrossTalk.CrossTalk_Message>(mensagem);

            string response = svcAvon.CompressedSend(request, null);

            Dominio.ObjetosDeValor.CrossTalk.CrossTalk_Message retorno = Servicos.XML.ConvertXMLStringToObject<Dominio.ObjetosDeValor.CrossTalk.CrossTalk_Message>(response);

            informacoes.CodigoMensagem = retorno.CrossTalk_Header.ResponseCode;
            informacoes.GUID = retorno.CrossTalk_Header.GUID;
            informacoes.Mensagem = retorno.CrossTalk_Header.ResponseCodeMessage;
            informacoes.Requisicao = inspector.LastRequestXML;
            informacoes.Resposta = inspector.LastResponseXML;
            informacoes.Response = retorno;
            return informacoes;
        }

        private static Dominio.ObjetosDeValor.Embarcador.Integracao.Avon.InformacoesIntegracaoAvon EnviarRetorno(Dominio.Entidades.Empresa empresa, int[] ctesDoLote, Repositorio.UnitOfWork unidadeDeTrabalho)
        {
            Dominio.ObjetosDeValor.Embarcador.Integracao.Avon.InformacoesIntegracaoAvon informacoes = new Dominio.ObjetosDeValor.Embarcador.Integracao.Avon.InformacoesIntegracaoAvon();

            Repositorio.Embarcador.Configuracoes.IntegracaoAvon repConfiguracaoIntegracaoAvon = new Repositorio.Embarcador.Configuracoes.IntegracaoAvon(unidadeDeTrabalho);

            Dominio.Entidades.Embarcador.Configuracoes.IntegracaoAvon configuracaoIntegracaoAvon = repConfiguracaoIntegracaoAvon.BuscarPorEmpresa(empresa.Codigo);

            if (configuracaoIntegracaoAvon == null || string.IsNullOrWhiteSpace(configuracaoIntegracaoAvon.EnterpriseID) || string.IsNullOrWhiteSpace(configuracaoIntegracaoAvon.TokenProducao))
            {
                informacoes.Mensagem = $"A configuração de integração da empresa {empresa.Descricao} para a Avon é inválida.";
                return informacoes;
            }

            string endpoint, token;

            try
            {
                ServicoAvon.RequestSoapClient svcAvon = new Servicos.Embarcador.Integracao.ConfiguracaoWebService(unidadeDeTrabalho).ObterClient<ServicoAvon.RequestSoapClient, ServicoAvon.RequestSoap>(Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoWebServiceIntegracao.Avon_Request, out Servicos.Models.Integracao.InspectorBehavior inspector);

                if (empresa.TipoAmbiente == Dominio.Enumeradores.TipoAmbiente.Producao)
                {
                    token = configuracaoIntegracaoAvon.TokenProducao;
                    endpoint = "http://portalnfe.avon.com/Infinitri.WebService.ExchangeMessage/ExchangeMessage.asmx";
                }
                else
                {
                    token = configuracaoIntegracaoAvon.TokenHomologacao;
                    endpoint = "http://portalnfeqa.avon.com/Infinitri.WebService.ExchangeMessage/ExchangeMessage.asmx";
                }

                svcAvon.Endpoint.Address = new System.ServiceModel.EndpointAddress(endpoint);

                Dominio.ObjetosDeValor.CrossTalk.CrossTalk_Message mensagem = new Dominio.ObjetosDeValor.CrossTalk.CrossTalk_Message()
                {
                    CrossTalk_Header = new Dominio.ObjetosDeValor.CrossTalk.CrossTalk_Header()
                    {
                        ProcessCode = "10005",
                        MessageType = "100",
                        ExchangePattern = "7",
                        EnterpriseId = configuracaoIntegracaoAvon.EnterpriseID,
                        Token = token,
                        ContentEncoding = "utf-8",
                        ContentType = "application/zip"
                    }
                };

                string request = Servicos.XML.ConvertObjectToXMLString<Dominio.ObjetosDeValor.CrossTalk.CrossTalk_Message>(mensagem);

                string response = svcAvon.CompressedSend(request, CriarZip(ctesDoLote, unidadeDeTrabalho));

                Dominio.ObjetosDeValor.CrossTalk.CrossTalk_Message retorno = Servicos.XML.ConvertXMLStringToObject<Dominio.ObjetosDeValor.CrossTalk.CrossTalk_Message>(response);

                informacoes.CodigoMensagem = retorno.CrossTalk_Header.ResponseCode;
                informacoes.GUID = retorno.CrossTalk_Header.GUID;
                informacoes.Mensagem = retorno.CrossTalk_Header.ResponseCodeMessage;
                informacoes.Requisicao = inspector.LastRequestXML;
                informacoes.Resposta = inspector.LastResponseXML;
            }
            catch (Exception excecao)
            {
                Log.TratarErro(excecao);

                informacoes.Mensagem = $"Ocorreu uma falha ao executar a integração com a Avon.";
            }

            return informacoes;
        }

        private static void AtualizarDocumentosLoteEnvioRetorno(InformacoesIntegracaoAvon informacoes, Dominio.Entidades.Embarcador.Ocorrencias.OcorrenciaCTeIntegracaoLote lote, List<Dominio.Entidades.Embarcador.Ocorrencias.OcorrenciaCTeIntegracao> ctesDoLote, UnitOfWork unidadeDeTrabalho)
        {
            Repositorio.Embarcador.Ocorrencias.OcorrenciaCTeIntegracao repOcorrenciaCTeIntegracao = new Repositorio.Embarcador.Ocorrencias.OcorrenciaCTeIntegracao(unidadeDeTrabalho);
            Repositorio.Embarcador.Ocorrencias.OcorrenciaCTeIntegracaoArquivo repOcorrenciaCTeIntegracaoArquivo = new Repositorio.Embarcador.Ocorrencias.OcorrenciaCTeIntegracaoArquivo(unidadeDeTrabalho);
            Repositorio.Embarcador.Ocorrencias.OcorrenciaCTeIntegracaoLote repOcorrenciaCTeIntegracaoLote = new Repositorio.Embarcador.Ocorrencias.OcorrenciaCTeIntegracaoLote(unidadeDeTrabalho);

            Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoIntegracao situacao = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoIntegracao.ProblemaIntegracao;

            if (informacoes.CodigoMensagem == "202")
                situacao = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoIntegracao.AgRetorno;

            string mensagem = informacoes.CodigoMensagem + " - " + informacoes.Mensagem;

            //unidadeDeTrabalho.Start();

            Dominio.Entidades.Embarcador.Ocorrencias.OcorrenciaCTeIntegracaoArquivo arquivoIntegracao = null;

            if (!string.IsNullOrWhiteSpace(informacoes.Requisicao) || !string.IsNullOrWhiteSpace(informacoes.Resposta))
            {
                arquivoIntegracao = new Dominio.Entidades.Embarcador.Ocorrencias.OcorrenciaCTeIntegracaoArquivo();

                arquivoIntegracao.ArquivoRequisicao = Servicos.Embarcador.Integracao.ArquivoIntegracao.SalvarArquivoIntegracao(informacoes.Requisicao, "xml", unidadeDeTrabalho);
                arquivoIntegracao.ArquivoResposta = Servicos.Embarcador.Integracao.ArquivoIntegracao.SalvarArquivoIntegracao(informacoes.Resposta, "xml", unidadeDeTrabalho);
                arquivoIntegracao.Data = informacoes.DataConsulta;
                arquivoIntegracao.Mensagem = mensagem;
                arquivoIntegracao.Tipo = Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoArquivoIntegracaoCTeCarga.EnvioParaProcessamento;

                repOcorrenciaCTeIntegracaoArquivo.Inserir(arquivoIntegracao);
            }

            foreach (Dominio.Entidades.Embarcador.Ocorrencias.OcorrenciaCTeIntegracao ocorrenciaCTeIntegracao in ctesDoLote)
            {
                ocorrenciaCTeIntegracao.SituacaoIntegracao = situacao;
                ocorrenciaCTeIntegracao.ProblemaIntegracao = mensagem;
                ocorrenciaCTeIntegracao.NumeroTentativas++;

                if (arquivoIntegracao != null)
                    ocorrenciaCTeIntegracao.ArquivosTransacao.Add(arquivoIntegracao);

                repOcorrenciaCTeIntegracao.Atualizar(ocorrenciaCTeIntegracao);
            }

            lote.DataEnvio = informacoes.DataConsulta;

            if (situacao == Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoIntegracao.ProblemaIntegracao)
                lote.DataRecebimento = informacoes.DataConsulta;
            else
                lote.DataRecebimento = null;

            lote.NumeroTentativas++;
            lote.ProblemaIntegracao = mensagem;
            lote.SituacaoIntegracao = situacao;
            lote.CodigoConsultaIntegradora = informacoes.GUID;
            lote.DataUltimaConsultaRetorno = DateTime.Now;

            repOcorrenciaCTeIntegracaoLote.Atualizar(lote);

            //unidadeDeTrabalho.CommitChanges();
        }


        private static void AtualizarDocumentosLoteEnvioRetorno(InformacoesIntegracaoAvon informacoes, Dominio.Entidades.Embarcador.NFS.NFSManualIntegracaoLote lote, List<Dominio.Entidades.Embarcador.NFS.NFSManualCTeIntegracao> ctesDoLote, UnitOfWork unidadeDeTrabalho)
        {
            Repositorio.Embarcador.NFS.NFSManualCTeIntegracao repNFSManualCTeIntegracao = new Repositorio.Embarcador.NFS.NFSManualCTeIntegracao(unidadeDeTrabalho);
            Repositorio.Embarcador.NFS.NFSManualIntegracaoArquivo repNFSManualIntegracaoArquivo = new Repositorio.Embarcador.NFS.NFSManualIntegracaoArquivo(unidadeDeTrabalho);
            Repositorio.Embarcador.NFS.NFSManualIntegracaoLote repNFSManualIntegracaoLote = new Repositorio.Embarcador.NFS.NFSManualIntegracaoLote(unidadeDeTrabalho);

            Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoIntegracao situacao = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoIntegracao.ProblemaIntegracao;

            if (informacoes.CodigoMensagem == "202")
                situacao = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoIntegracao.AgRetorno;

            string mensagem = informacoes.CodigoMensagem + " - " + informacoes.Mensagem;

            //unidadeDeTrabalho.Start();

            Dominio.Entidades.Embarcador.NFS.NFSManualIntegracaoArquivo arquivoIntegracao = null;

            if (!string.IsNullOrWhiteSpace(informacoes.Requisicao) || !string.IsNullOrWhiteSpace(informacoes.Resposta))
            {
                arquivoIntegracao = new Dominio.Entidades.Embarcador.NFS.NFSManualIntegracaoArquivo();

                arquivoIntegracao.ArquivoRequisicao = Servicos.Embarcador.Integracao.ArquivoIntegracao.SalvarArquivoIntegracao(informacoes.Requisicao, "xml", unidadeDeTrabalho);
                arquivoIntegracao.ArquivoResposta = Servicos.Embarcador.Integracao.ArquivoIntegracao.SalvarArquivoIntegracao(informacoes.Resposta, "xml", unidadeDeTrabalho);
                arquivoIntegracao.Data = informacoes.DataConsulta;
                arquivoIntegracao.Mensagem = mensagem;
                arquivoIntegracao.Tipo = Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoArquivoIntegracaoCTeCarga.EnvioParaProcessamento;

                repNFSManualIntegracaoArquivo.Inserir(arquivoIntegracao);
            }

            foreach (Dominio.Entidades.Embarcador.NFS.NFSManualCTeIntegracao nfsManualCTeIntegracao in ctesDoLote)
            {
                nfsManualCTeIntegracao.SituacaoIntegracao = situacao;
                nfsManualCTeIntegracao.ProblemaIntegracao = mensagem;
                nfsManualCTeIntegracao.NumeroTentativas++;

                if (arquivoIntegracao != null)
                    nfsManualCTeIntegracao.ArquivosTransacao.Add(arquivoIntegracao);

                repNFSManualCTeIntegracao.Atualizar(nfsManualCTeIntegracao);
            }

            lote.DataEnvio = informacoes.DataConsulta;

            if (situacao == Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoIntegracao.ProblemaIntegracao)
                lote.DataRecebimento = informacoes.DataConsulta;
            else
                lote.DataRecebimento = null;

            lote.NumeroTentativas++;
            lote.ProblemaIntegracao = mensagem;
            lote.SituacaoIntegracao = situacao;
            lote.CodigoConsultaIntegradora = informacoes.GUID;
            lote.DataUltimaConsultaRetorno = DateTime.Now;

            repNFSManualIntegracaoLote.Atualizar(lote);

            //unidadeDeTrabalho.CommitChanges();
        }

        private static void AtualizarDocumentosLoteEnvioRetorno(InformacoesIntegracaoAvon informacoes, CargaCTeIntegracaoLote lote, List<Dominio.Entidades.Embarcador.Cargas.CargaCTeIntegracao> ctesDoLote, UnitOfWork unidadeDeTrabalho)
        {
            Repositorio.Embarcador.Cargas.CargaCTeIntegracao repCargaCTeIntegracao = new Repositorio.Embarcador.Cargas.CargaCTeIntegracao(unidadeDeTrabalho);
            Repositorio.Embarcador.Cargas.CargaCTeIntegracaoArquivo repCargaCTeIntegracaoArquivo = new Repositorio.Embarcador.Cargas.CargaCTeIntegracaoArquivo(unidadeDeTrabalho);
            Repositorio.Embarcador.Cargas.CargaCTeIntegracaoLote repCargaCTeIntegracaoLote = new Repositorio.Embarcador.Cargas.CargaCTeIntegracaoLote(unidadeDeTrabalho);

            Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoIntegracao situacao = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoIntegracao.ProblemaIntegracao;

            if (informacoes.CodigoMensagem == "202")
                situacao = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoIntegracao.AgRetorno;

            string mensagem = informacoes.CodigoMensagem + " - " + informacoes.Mensagem;

            //unidadeDeTrabalho.Start();

            Dominio.Entidades.Embarcador.Cargas.CargaCTeIntegracaoArquivo arquivoIntegracao = null;

            if (!string.IsNullOrWhiteSpace(informacoes.Requisicao) || !string.IsNullOrWhiteSpace(informacoes.Resposta))
            {
                arquivoIntegracao = new CargaCTeIntegracaoArquivo();

                arquivoIntegracao.ArquivoRequisicao = Servicos.Embarcador.Integracao.ArquivoIntegracao.SalvarArquivoIntegracao(informacoes.Requisicao, "xml", unidadeDeTrabalho);
                arquivoIntegracao.ArquivoResposta = Servicos.Embarcador.Integracao.ArquivoIntegracao.SalvarArquivoIntegracao(informacoes.Resposta, "xml", unidadeDeTrabalho);
                arquivoIntegracao.Data = informacoes.DataConsulta;
                arquivoIntegracao.Mensagem = mensagem;
                arquivoIntegracao.Tipo = Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoArquivoIntegracaoCTeCarga.EnvioParaProcessamento;

                repCargaCTeIntegracaoArquivo.Inserir(arquivoIntegracao);
            }

            foreach (Dominio.Entidades.Embarcador.Cargas.CargaCTeIntegracao cargaCTeIntegracao in ctesDoLote)
            {
                cargaCTeIntegracao.SituacaoIntegracao = situacao;
                cargaCTeIntegracao.ProblemaIntegracao = mensagem;
                cargaCTeIntegracao.NumeroTentativas++;

                if (arquivoIntegracao != null)
                    cargaCTeIntegracao.ArquivosTransacao.Add(arquivoIntegracao);

                repCargaCTeIntegracao.Atualizar(cargaCTeIntegracao);
            }

            lote.DataEnvio = informacoes.DataConsulta;

            if (situacao == Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoIntegracao.ProblemaIntegracao)
                lote.DataRecebimento = informacoes.DataConsulta;
            else
                lote.DataRecebimento = null;

            lote.NumeroTentativas++;
            lote.ProblemaIntegracao = mensagem;
            lote.SituacaoIntegracao = situacao;
            lote.CodigoConsultaIntegradora = informacoes.GUID;
            lote.DataUltimaConsultaRetorno = DateTime.Now;

            repCargaCTeIntegracaoLote.Atualizar(lote);

            //unidadeDeTrabalho.CommitChanges();
        }

        private static void AtualizarDocumentosLoteConsultaRetorno(InformacoesIntegracaoAvon informacoes, CargaCTeIntegracaoLote lote, List<Dominio.Entidades.Embarcador.Cargas.CargaCTeIntegracao> ctesDoLote, CrossTalk_Message retorno, UnitOfWork unidadeDeTrabalho)
        {
            Repositorio.Embarcador.Cargas.CargaCTeIntegracao repCargaCTeIntegracao = new Repositorio.Embarcador.Cargas.CargaCTeIntegracao(unidadeDeTrabalho);
            Repositorio.Embarcador.Cargas.CargaCTeIntegracaoArquivo repCargaCTeIntegracaoArquivo = new Repositorio.Embarcador.Cargas.CargaCTeIntegracaoArquivo(unidadeDeTrabalho);
            Repositorio.Embarcador.Cargas.CargaCTeIntegracaoLote repCargaCTeIntegracaoLote = new Repositorio.Embarcador.Cargas.CargaCTeIntegracaoLote(unidadeDeTrabalho);

            string mensagem = informacoes.CodigoMensagem + " - " + informacoes.Mensagem;

            string[] situacoesValidas = new string[] { "0", "1", "2", "3", "4" };

            //unidadeDeTrabalho.Start();

            Dominio.Entidades.Embarcador.Cargas.CargaCTeIntegracaoArquivo arquivoIntegracao = null;

            if (!string.IsNullOrWhiteSpace(informacoes.Requisicao) || !string.IsNullOrWhiteSpace(informacoes.Resposta))
            {
                arquivoIntegracao = new CargaCTeIntegracaoArquivo();

                arquivoIntegracao.ArquivoRequisicao = Servicos.Embarcador.Integracao.ArquivoIntegracao.SalvarArquivoIntegracao(informacoes.Requisicao, "xml", unidadeDeTrabalho);
                arquivoIntegracao.ArquivoResposta = Servicos.Embarcador.Integracao.ArquivoIntegracao.SalvarArquivoIntegracao(informacoes.Resposta, "xml", unidadeDeTrabalho);
                arquivoIntegracao.Data = informacoes.DataConsulta;
                arquivoIntegracao.Mensagem = mensagem;
                arquivoIntegracao.Tipo = Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoArquivoIntegracaoCTeCarga.RetornoDoProcessamento;

                repCargaCTeIntegracaoArquivo.Inserir(arquivoIntegracao);
            }

            if (informacoes.CodigoMensagem != "202" || retorno == null)
            {
                if (retorno == null || retorno.CrossTalk_Header.ResponseCode != "200") //deu pau, falto as lajota, não vai rola piscina olimpica
                {
                    Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoIntegracao situacao = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoIntegracao.ProblemaIntegracao;

                    foreach (Dominio.Entidades.Embarcador.Cargas.CargaCTeIntegracao cargaCTeIntegracao in ctesDoLote)
                    {
                        cargaCTeIntegracao.SituacaoIntegracao = situacao;
                        cargaCTeIntegracao.ProblemaIntegracao = mensagem;

                        if (arquivoIntegracao != null)
                            cargaCTeIntegracao.ArquivosTransacao.Add(arquivoIntegracao);

                        repCargaCTeIntegracao.Atualizar(cargaCTeIntegracao);
                    }

                    lote.DataRecebimento = informacoes.DataConsulta;
                    lote.ProblemaIntegracao = mensagem;
                    lote.SituacaoIntegracao = situacao;
                    lote.DataUltimaConsultaRetorno = DateTime.Now;

                    repCargaCTeIntegracaoLote.Atualizar(lote);
                }
                else //volto
                {
                    for (int i = 0; i < retorno.CrossTalk_Body.Document.Length; i++)
                    {
                        Dominio.ObjetosDeValor.CrossTalk.Document documentoRetornado = retorno.CrossTalk_Body.Document[i];

                        Dominio.Entidades.Embarcador.Cargas.CargaCTeIntegracao cargaCTeIntegracao = (from obj in ctesDoLote where obj.CargaCTe.CTe.Chave == documentoRetornado.AccessKey select obj).FirstOrDefault();

                        if (cargaCTeIntegracao != null)
                        {
                            if (situacoesValidas.Contains(documentoRetornado.Response.InnerCode))
                                cargaCTeIntegracao.SituacaoIntegracao = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoIntegracao.Integrado;
                            else
                                cargaCTeIntegracao.SituacaoIntegracao = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoIntegracao.ProblemaIntegracao;

                            cargaCTeIntegracao.ProblemaIntegracao = documentoRetornado.Response.InnerCode + " - " + documentoRetornado.Response.Description;

                            if (arquivoIntegracao != null)
                                cargaCTeIntegracao.ArquivosTransacao.Add(arquivoIntegracao);

                            repCargaCTeIntegracao.Atualizar(cargaCTeIntegracao);
                        }
                    }

                    if (repCargaCTeIntegracao.ContarPorLote(lote.Codigo, Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoIntegracao.AgRetorno) > 0) //vai consultar denovo depois pois tem documentos que não foram retornados (se acontecer esse caso, acho que vai ficar consultando forévis, mas ta aí, não diga que não ta implementado)
                        lote.SituacaoIntegracao = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoIntegracao.AgRetorno;
                    else if (repCargaCTeIntegracao.ContarPorLote(lote.Codigo, Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoIntegracao.ProblemaIntegracao) > 0) //deu pau
                        lote.SituacaoIntegracao = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoIntegracao.ProblemaIntegracao;
                    else
                        lote.SituacaoIntegracao = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoIntegracao.Integrado;

                    lote.DataRecebimento = informacoes.DataConsulta;
                    lote.ProblemaIntegracao = mensagem;
                    lote.DataUltimaConsultaRetorno = DateTime.Now;

                    repCargaCTeIntegracaoLote.Atualizar(lote);
                }
            }
            else
            {
                lote.DataUltimaConsultaRetorno = DateTime.Now;

                repCargaCTeIntegracaoLote.Atualizar(lote);

                foreach (Dominio.Entidades.Embarcador.Cargas.CargaCTeIntegracao cargaCTeIntegracao in ctesDoLote)
                {
                    cargaCTeIntegracao.ProblemaIntegracao = mensagem;

                    if (arquivoIntegracao != null)
                        cargaCTeIntegracao.ArquivosTransacao.Add(arquivoIntegracao);

                    repCargaCTeIntegracao.Atualizar(cargaCTeIntegracao);
                }
            }

            //unidadeDeTrabalho.CommitChanges();
        }

        private static void AtualizarDocumentosLoteConsultaRetorno(InformacoesIntegracaoAvon informacoes, Dominio.Entidades.Embarcador.NFS.NFSManualIntegracaoLote lote, List<Dominio.Entidades.Embarcador.NFS.NFSManualCTeIntegracao> ctesDoLote, CrossTalk_Message retorno, UnitOfWork unidadeDeTrabalho)
        {
            Repositorio.Embarcador.NFS.NFSManualCTeIntegracao repNFSManualCTeIntegracao = new Repositorio.Embarcador.NFS.NFSManualCTeIntegracao(unidadeDeTrabalho);
            Repositorio.Embarcador.NFS.NFSManualIntegracaoArquivo repNFSManualIntegracaoArquivo = new Repositorio.Embarcador.NFS.NFSManualIntegracaoArquivo(unidadeDeTrabalho);
            Repositorio.Embarcador.NFS.NFSManualIntegracaoLote repNFSManualIntegracaoLote = new Repositorio.Embarcador.NFS.NFSManualIntegracaoLote(unidadeDeTrabalho);

            string mensagem = informacoes.CodigoMensagem + " - " + informacoes.Mensagem;

            string[] situacoesValidas = new string[] { "0", "1", "2", "3", "4" };

            //unidadeDeTrabalho.Start();

            Dominio.Entidades.Embarcador.NFS.NFSManualIntegracaoArquivo arquivoIntegracao = null;

            if (!string.IsNullOrWhiteSpace(informacoes.Requisicao) || !string.IsNullOrWhiteSpace(informacoes.Resposta))
            {
                arquivoIntegracao = new Dominio.Entidades.Embarcador.NFS.NFSManualIntegracaoArquivo();

                arquivoIntegracao.ArquivoRequisicao = Servicos.Embarcador.Integracao.ArquivoIntegracao.SalvarArquivoIntegracao(informacoes.Requisicao, "xml", unidadeDeTrabalho);
                arquivoIntegracao.ArquivoResposta = Servicos.Embarcador.Integracao.ArquivoIntegracao.SalvarArquivoIntegracao(informacoes.Resposta, "xml", unidadeDeTrabalho);
                arquivoIntegracao.Data = informacoes.DataConsulta;
                arquivoIntegracao.Mensagem = mensagem;
                arquivoIntegracao.Tipo = Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoArquivoIntegracaoCTeCarga.RetornoDoProcessamento;

                repNFSManualIntegracaoArquivo.Inserir(arquivoIntegracao);
            }

            if (informacoes.CodigoMensagem != "202" || retorno == null)
            {
                if (retorno == null || retorno.CrossTalk_Header.ResponseCode != "200") //deu pau, falto as lajota, não vai rola piscina olimpica
                {
                    Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoIntegracao situacao = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoIntegracao.ProblemaIntegracao;

                    foreach (Dominio.Entidades.Embarcador.NFS.NFSManualCTeIntegracao nfsManualCTeIntegracao in ctesDoLote)
                    {
                        nfsManualCTeIntegracao.SituacaoIntegracao = situacao;
                        nfsManualCTeIntegracao.ProblemaIntegracao = mensagem;

                        if (arquivoIntegracao != null)
                            nfsManualCTeIntegracao.ArquivosTransacao.Add(arquivoIntegracao);

                        repNFSManualCTeIntegracao.Atualizar(nfsManualCTeIntegracao);
                    }

                    lote.DataRecebimento = informacoes.DataConsulta;
                    lote.ProblemaIntegracao = mensagem;
                    lote.SituacaoIntegracao = situacao;
                    lote.DataUltimaConsultaRetorno = DateTime.Now;

                    repNFSManualIntegracaoLote.Atualizar(lote);
                }
                else //volto
                {
                    for (int i = 0; i < retorno.CrossTalk_Body.Document.Length; i++)
                    {
                        Dominio.ObjetosDeValor.CrossTalk.Document documentoRetornado = retorno.CrossTalk_Body.Document[i];

                        Dominio.Entidades.Embarcador.NFS.NFSManualCTeIntegracao nfsManualCTeIntegracao = (from obj in ctesDoLote where obj.LancamentoNFSManual.CTe.Chave == documentoRetornado.AccessKey select obj).FirstOrDefault();

                        if (nfsManualCTeIntegracao != null)
                        {
                            if (situacoesValidas.Contains(documentoRetornado.Response.InnerCode))
                                nfsManualCTeIntegracao.SituacaoIntegracao = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoIntegracao.Integrado;
                            else
                                nfsManualCTeIntegracao.SituacaoIntegracao = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoIntegracao.ProblemaIntegracao;

                            nfsManualCTeIntegracao.ProblemaIntegracao = documentoRetornado.Response.InnerCode + " - " + documentoRetornado.Response.Description;

                            if (arquivoIntegracao != null)
                                nfsManualCTeIntegracao.ArquivosTransacao.Add(arquivoIntegracao);

                            repNFSManualCTeIntegracao.Atualizar(nfsManualCTeIntegracao);
                        }
                    }

                    if (repNFSManualCTeIntegracao.ContarPorLote(lote.Codigo, Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoIntegracao.AgRetorno) > 0) //vai consultar denovo depois pois tem documentos que não foram retornados (se acontecer esse caso, acho que vai ficar consultando forévis, mas ta aí, não diga que não ta implementado)
                        lote.SituacaoIntegracao = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoIntegracao.AgRetorno;
                    else if (repNFSManualCTeIntegracao.ContarPorLote(lote.Codigo, Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoIntegracao.ProblemaIntegracao) > 0) //deu pau
                        lote.SituacaoIntegracao = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoIntegracao.ProblemaIntegracao;
                    else
                        lote.SituacaoIntegracao = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoIntegracao.Integrado;

                    lote.DataRecebimento = informacoes.DataConsulta;
                    lote.ProblemaIntegracao = mensagem;
                    lote.DataUltimaConsultaRetorno = DateTime.Now;

                    repNFSManualIntegracaoLote.Atualizar(lote);
                }
            }
            else
            {
                lote.DataUltimaConsultaRetorno = DateTime.Now;

                repNFSManualIntegracaoLote.Atualizar(lote);

                foreach (Dominio.Entidades.Embarcador.NFS.NFSManualCTeIntegracao nfsManualCTeIntegracao in ctesDoLote)
                {
                    nfsManualCTeIntegracao.ProblemaIntegracao = mensagem;

                    if (arquivoIntegracao != null)
                        nfsManualCTeIntegracao.ArquivosTransacao.Add(arquivoIntegracao);

                    repNFSManualCTeIntegracao.Atualizar(nfsManualCTeIntegracao);
                }
            }

            //unidadeDeTrabalho.CommitChanges();
        }

        private static void AtualizarDocumentosLoteConsultaRetorno(InformacoesIntegracaoAvon informacoes, Dominio.Entidades.Embarcador.Ocorrencias.OcorrenciaCTeIntegracaoLote lote, List<Dominio.Entidades.Embarcador.Ocorrencias.OcorrenciaCTeIntegracao> ctesDoLote, CrossTalk_Message retorno, UnitOfWork unidadeDeTrabalho)
        {
            Repositorio.Embarcador.Ocorrencias.OcorrenciaCTeIntegracao repOcorrenciaCTeIntegracao = new Repositorio.Embarcador.Ocorrencias.OcorrenciaCTeIntegracao(unidadeDeTrabalho);
            Repositorio.Embarcador.Ocorrencias.OcorrenciaCTeIntegracaoArquivo repOcorrenciaCTeIntegracaoArquivo = new Repositorio.Embarcador.Ocorrencias.OcorrenciaCTeIntegracaoArquivo(unidadeDeTrabalho);
            Repositorio.Embarcador.Ocorrencias.OcorrenciaCTeIntegracaoLote repOcorrenciaCTeIntegracaoLote = new Repositorio.Embarcador.Ocorrencias.OcorrenciaCTeIntegracaoLote(unidadeDeTrabalho);

            string mensagem = informacoes.CodigoMensagem + " - " + informacoes.Mensagem;

            string[] situacoesValidas = new string[] { "0", "1", "2", "3", "4" };

            //unidadeDeTrabalho.Start();

            Dominio.Entidades.Embarcador.Ocorrencias.OcorrenciaCTeIntegracaoArquivo arquivoIntegracao = null;

            if (!string.IsNullOrWhiteSpace(informacoes.Requisicao) || !string.IsNullOrWhiteSpace(informacoes.Resposta))
            {
                arquivoIntegracao = new Dominio.Entidades.Embarcador.Ocorrencias.OcorrenciaCTeIntegracaoArquivo();

                arquivoIntegracao.ArquivoRequisicao = Servicos.Embarcador.Integracao.ArquivoIntegracao.SalvarArquivoIntegracao(informacoes.Requisicao, "xml", unidadeDeTrabalho);
                arquivoIntegracao.ArquivoResposta = Servicos.Embarcador.Integracao.ArquivoIntegracao.SalvarArquivoIntegracao(informacoes.Resposta, "xml", unidadeDeTrabalho);
                arquivoIntegracao.Data = informacoes.DataConsulta;
                arquivoIntegracao.Mensagem = mensagem;
                arquivoIntegracao.Tipo = Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoArquivoIntegracaoCTeCarga.RetornoDoProcessamento;

                repOcorrenciaCTeIntegracaoArquivo.Inserir(arquivoIntegracao);
            }

            if (informacoes.CodigoMensagem != "202" || retorno == null)
            {
                if (retorno == null || retorno.CrossTalk_Header.ResponseCode != "200") //deu pau, falto as lajota, não vai rola piscina olimpica
                {
                    Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoIntegracao situacao = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoIntegracao.ProblemaIntegracao;

                    foreach (Dominio.Entidades.Embarcador.Ocorrencias.OcorrenciaCTeIntegracao ocorrenciaCTeIntegracao in ctesDoLote)
                    {
                        ocorrenciaCTeIntegracao.SituacaoIntegracao = situacao;
                        ocorrenciaCTeIntegracao.ProblemaIntegracao = mensagem;

                        if (arquivoIntegracao != null)
                            ocorrenciaCTeIntegracao.ArquivosTransacao.Add(arquivoIntegracao);

                        repOcorrenciaCTeIntegracao.Atualizar(ocorrenciaCTeIntegracao);
                    }

                    lote.DataRecebimento = informacoes.DataConsulta;
                    lote.ProblemaIntegracao = mensagem;
                    lote.SituacaoIntegracao = situacao;
                    lote.DataUltimaConsultaRetorno = DateTime.Now;

                    repOcorrenciaCTeIntegracaoLote.Atualizar(lote);
                }
                else //volto
                {
                    bool falha = false;
                    for (int i = 0; i < retorno.CrossTalk_Body.Document.Length; i++)
                    {
                        Dominio.ObjetosDeValor.CrossTalk.Document documentoRetornado = retorno.CrossTalk_Body.Document[i];

                        Dominio.Entidades.Embarcador.Ocorrencias.OcorrenciaCTeIntegracao ocorrenciaCTeIntegracao = (from obj in ctesDoLote where obj.CargaCTe.CTe.Chave == documentoRetornado.AccessKey select obj).FirstOrDefault();

                        if (documentoRetornado.Response.InnerCode == "201")
                        {
                            for (int j = 0; j < ctesDoLote.Count; j++)
                            {
                                ctesDoLote[j].SituacaoIntegracao = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoIntegracao.ProblemaIntegracao;
                                ctesDoLote[j].ProblemaIntegracao = string.Join(", ", documentoRetornado.Response.ErrorMessages);
                                repOcorrenciaCTeIntegracao.Atualizar(ctesDoLote[j]);
                            }
                            falha = true;
                            break;
                        }
                        else if (situacoesValidas.Contains(documentoRetornado.Response.InnerCode))
                            ocorrenciaCTeIntegracao.SituacaoIntegracao = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoIntegracao.Integrado;
                        else
                            ocorrenciaCTeIntegracao.SituacaoIntegracao = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoIntegracao.ProblemaIntegracao;

                        ocorrenciaCTeIntegracao.ProblemaIntegracao = documentoRetornado.Response.InnerCode + " - " + documentoRetornado.Response.Description;

                        if (arquivoIntegracao != null)
                            ocorrenciaCTeIntegracao.ArquivosTransacao.Add(arquivoIntegracao);

                        repOcorrenciaCTeIntegracao.Atualizar(ocorrenciaCTeIntegracao);
                    }

                    if (!falha)
                    {
                        if (repOcorrenciaCTeIntegracao.ContarPorLote(lote.Codigo, Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoIntegracao.AgRetorno) > 0) //vai consultar denovo depois pois tem documentos que não foram retornados (se acontecer esse caso, acho que vai ficar consultando forévis, mas ta aí, não diga que não ta implementado)
                            lote.SituacaoIntegracao = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoIntegracao.AgRetorno;
                        else if (repOcorrenciaCTeIntegracao.ContarPorLote(lote.Codigo, Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoIntegracao.ProblemaIntegracao) > 0) //deu pau
                            lote.SituacaoIntegracao = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoIntegracao.ProblemaIntegracao;
                        else
                            lote.SituacaoIntegracao = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoIntegracao.Integrado;
                    }
                    else
                    {
                        lote.SituacaoIntegracao = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoIntegracao.ProblemaIntegracao;
                    }

                    lote.DataRecebimento = informacoes.DataConsulta;
                    lote.ProblemaIntegracao = mensagem;
                    lote.DataUltimaConsultaRetorno = DateTime.Now;

                    repOcorrenciaCTeIntegracaoLote.Atualizar(lote);
                }
            }
            else
            {
                lote.DataUltimaConsultaRetorno = DateTime.Now;

                repOcorrenciaCTeIntegracaoLote.Atualizar(lote);

                foreach (Dominio.Entidades.Embarcador.Ocorrencias.OcorrenciaCTeIntegracao ocorrenciaCTeIntegracao in ctesDoLote)
                {
                    ocorrenciaCTeIntegracao.ProblemaIntegracao = mensagem;

                    if (arquivoIntegracao != null)
                        ocorrenciaCTeIntegracao.ArquivosTransacao.Add(arquivoIntegracao);

                    repOcorrenciaCTeIntegracao.Atualizar(ocorrenciaCTeIntegracao);
                }
            }

            //unidadeDeTrabalho.CommitChanges();
        }

        private static byte[] CriarZip(int[] codigosCTes, Repositorio.UnitOfWork unidadeTrabalho)
        {
            Servicos.CTe serCTe = new Servicos.CTe(unidadeTrabalho);

            Repositorio.ConhecimentoDeTransporteEletronico repCte = new ConhecimentoDeTransporteEletronico(unidadeTrabalho);

            List<Dominio.Entidades.ConhecimentoDeTransporteEletronico> ctes = repCte.BuscarCTesPorCodigos(codigosCTes.ToList());

            //Servicos.Log.TratarErro("Criando ZIP avon " + codigosCTes.Count() + " ctes e " + xmls.Count() + " xmls.");

            using (var memoryStream = new MemoryStream())
            {
                using (var archive = new ZipArchive(memoryStream, ZipArchiveMode.Create, true))
                {
                    foreach (Dominio.Entidades.ConhecimentoDeTransporteEletronico cte in ctes.Where(obj => obj.Status == "A" || obj.Status == "C" || obj.Status == "Z"))
                    {
                        var file = archive.CreateEntry(cte.Chave + ".xml");

                        using (var entryStream = file.Open())
                        using (var streamWriter = new StreamWriter(entryStream))
                            streamWriter.Write(serCTe.ObterStringXMLAutorizacao(cte, unidadeTrabalho));
                    }
                }

                memoryStream.Seek(0, SeekOrigin.Begin);

                return memoryStream.ToArray();
            }
        }
    }
}
