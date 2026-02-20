using Dominio.Entidades.Embarcador.Cargas;
using Dominio.Entidades.Embarcador.Terceiros;
using Dominio.Excecoes.Embarcador;
using Dominio.ObjetosDeValor.Embarcador.Enumeradores;
using Servicos.ServicoSAP_ST;
using Servicos.ServicoSAP_ST_Cancelamento;
using System;
using System.Collections.Generic;
using System.Net;

namespace Servicos.Embarcador.Integracao.SAPST
{
    public class IntegracaoSAPST
    {

        #region Atributos Privados Somente Leitura

        private readonly Repositorio.UnitOfWork _unitOfWork;

        #endregion

        #region Construtores

        public IntegracaoSAPST(Repositorio.UnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        #endregion

        #region Métodos Públicos

        public void IntegrarAdiantamentoContrato(Dominio.Entidades.Embarcador.Cargas.CargaCargaIntegracao integracaoCarga)
        {
            ServicePointManager.Expect100Continue = true;
            ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;

            Repositorio.Embarcador.Cargas.CargaCargaIntegracao repCargaCargaIntegracao = new Repositorio.Embarcador.Cargas.CargaCargaIntegracao(_unitOfWork);
            ArquivoTransacao<Dominio.Entidades.Embarcador.Cargas.CargaCTeIntegracaoArquivo> servicoArquivoTransacao = new ArquivoTransacao<Dominio.Entidades.Embarcador.Cargas.CargaCTeIntegracaoArquivo>(_unitOfWork);

            InspectorBehavior inspector = new InspectorBehavior();

            integracaoCarga.DataIntegracao = DateTime.Now;
            integracaoCarga.NumeroTentativas += 1;

            string mensagemRetorno = string.Empty;

            try
            {
                Repositorio.Embarcador.Configuracoes.IntegracaoSAPST repConfiguracaoSAPST = new Repositorio.Embarcador.Configuracoes.IntegracaoSAPST(_unitOfWork);
                Dominio.Entidades.Embarcador.Configuracoes.IntegracaoSAPST configuracaoSAPST = repConfiguracaoSAPST.BuscarPrimeiroRegistro();

                if (!configuracaoSAPST.PossuiIntegracaoSAPST)
                    throw new ServicoException("Integração com SAP - ST não está habilitada");

                if (string.IsNullOrWhiteSpace(configuracaoSAPST.URLCriarAtendimento))
                    throw new ServicoException("Não há URL configurada para a integração com SAP - ST");

                if (string.IsNullOrWhiteSpace(configuracaoSAPST.Usuario) || string.IsNullOrWhiteSpace(configuracaoSAPST.Senha))
                    throw new ServicoException("Usuário e senha não configurados para integração SAP - ST");




                Repositorio.Embarcador.Terceiros.ContratoFrete repContratoFrete = new Repositorio.Embarcador.Terceiros.ContratoFrete(_unitOfWork);
                Dominio.Entidades.Embarcador.Terceiros.ContratoFrete contratoFrete = repContratoFrete.BuscarPorCarga(integracaoCarga.Carga.Codigo);


                if ((contratoFrete?.ValorAdiantamento ?? 0) <= 0)
                {
                    integracaoCarga.Protocolo = "";
                    integracaoCarga.SituacaoIntegracao = SituacaoIntegracao.Integrado;
                    mensagemRetorno = $"Integração não enviada, não existe valor de adiantamento para o contrato.";
                }
                else
                {
                    ServicoSAP_ST.SI_Criar_Adiantamento_Sync_OUTClient cliente = ObterClient(configuracaoSAPST);

                    cliente.Endpoint.EndpointBehaviors.Add(inspector);

                    Servicos.ServicoSAP_ST.DT_Criar_Adiantamento_req request = PreencherRequisicao(integracaoCarga, _unitOfWork, contratoFrete);
                    Servicos.ServicoSAP_ST.DT_Criar_Adiantamento_resp response = cliente.SI_Criar_Adiantamento_Sync_OUT(request);

                    if (response.T_RETURN[0].TIPO.Equals("E"))
                    {
                        integracaoCarga.SituacaoIntegracao = SituacaoIntegracao.ProblemaIntegracao;
                        mensagemRetorno = $"SAP: {response.T_RETURN[0].TEXTO_MSG}";
                    }
                    else
                    {
                        integracaoCarga.Protocolo = "00" + response.T_RETURN[0].TEXTO_MSG;
                        integracaoCarga.SituacaoIntegracao = SituacaoIntegracao.Integrado;
                        mensagemRetorno = $"Integrado com sucesso. {response.T_RETURN[0].DOC_CONT}";
                    }
                }
            }
            catch (ServicoException excecao)
            {
                integracaoCarga.SituacaoIntegracao = SituacaoIntegracao.ProblemaIntegracao;
                mensagemRetorno = excecao.Message;
            }
            catch (Exception excecao)
            {
                Log.TratarErro(excecao);

                integracaoCarga.SituacaoIntegracao = SituacaoIntegracao.ProblemaIntegracao;
                mensagemRetorno = "Problema ao integrar com o SAP - ST";
            }
            servicoArquivoTransacao.Adicionar(inspector.LastRequestXML, inspector.LastResponseXML, "xml", integracaoCarga);

            integracaoCarga.ProblemaIntegracao = mensagemRetorno;
            repCargaCargaIntegracao.Atualizar(integracaoCarga);
        }
        public void IntegrarCancelamentoST(Dominio.Entidades.Embarcador.Cargas.CargaCancelamentoCargaIntegracao cargaCancelamentoCargaIntegracao)
        {
            ServicePointManager.Expect100Continue = true;
            ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;
            Repositorio.Embarcador.Cargas.CargaCargaIntegracao repCargaCargaIntegracao = new Repositorio.Embarcador.Cargas.CargaCargaIntegracao(_unitOfWork);
            Dominio.Entidades.Embarcador.Cargas.CargaCargaIntegracao cargaIntegracao = new Dominio.Entidades.Embarcador.Cargas.CargaCargaIntegracao();
            Repositorio.Embarcador.Cargas.CargaCancelamentoCargaIntegracao repCargaCancelamentoCargaIntegracao = new Repositorio.Embarcador.Cargas.CargaCancelamentoCargaIntegracao(_unitOfWork);
            ArquivoTransacao<Dominio.Entidades.Embarcador.Cargas.CargaCTeIntegracaoArquivo> servicoArquivoTransacao = new ArquivoTransacao<Dominio.Entidades.Embarcador.Cargas.CargaCTeIntegracaoArquivo>(_unitOfWork);
            
            cargaIntegracao = repCargaCargaIntegracao.BuscarPorCargaETipoIntegracao(cargaCancelamentoCargaIntegracao.CargaCancelamento.Carga.Codigo, Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoIntegracao.SAP_ST);
            
            InspectorBehavior inspector = new InspectorBehavior();

            cargaCancelamentoCargaIntegracao.DataIntegracao = DateTime.Now;
            cargaCancelamentoCargaIntegracao.NumeroTentativas += 1;

            string mensagemRetorno = string.Empty;

            try
            {
                if(cargaIntegracao != null && !string.IsNullOrEmpty(cargaIntegracao.ProblemaIntegracao) && cargaIntegracao.ProblemaIntegracao.Equals("Integração não enviada, não existe valor de adiantamento para o contrato."))
                {
                    cargaCancelamentoCargaIntegracao.Protocolo = " ";
                    cargaCancelamentoCargaIntegracao.SituacaoIntegracao = SituacaoIntegracao.Integrado;
                    mensagemRetorno = "Integração não enviada, não existe valor de adiantamento para o contrato.";
                    return;
                }

                Repositorio.Embarcador.Configuracoes.IntegracaoSAPST repConfiguracaoSAPST = new Repositorio.Embarcador.Configuracoes.IntegracaoSAPST(_unitOfWork);
                Dominio.Entidades.Embarcador.Configuracoes.IntegracaoSAPST configuracaoSAPST = repConfiguracaoSAPST.BuscarPrimeiroRegistro();

                if (!configuracaoSAPST.PossuiIntegracaoSAPST)
                    throw new ServicoException("Integração com SAP - ST não está habilitada");

                if (string.IsNullOrWhiteSpace(configuracaoSAPST.URLCancelamentoST))
                    throw new ServicoException("Não há URL configurada para a integração de cancelamento SAP - ST");

                if (string.IsNullOrWhiteSpace(configuracaoSAPST.Usuario) || string.IsNullOrWhiteSpace(configuracaoSAPST.Senha))
                    throw new ServicoException("Usuário e senha não configurados para integração SAP - ST");

                Servicos.ServicoSAP_ST_Cancelamento.SI_Cancel_ST_Sync_OUTClient cliente = ObterClientCancelamento(configuracaoSAPST);

                cliente.Endpoint.EndpointBehaviors.Add(inspector);

                Servicos.ServicoSAP_ST_Cancelamento.DT_Cancel_ST_req request = PreencherRequisicaoCancelamento(cargaCancelamentoCargaIntegracao, _unitOfWork);

                Servicos.ServicoSAP_ST_Cancelamento.DT_Cancel_ST_resp response = cliente.SI_Cancel_ST_Sync_OUT(request);
                //Servicos.ServicoSAP_ST_Cancelamento.SI_Cancel_ST_Sync_OUTResponse response = cliente.SI_Cancel_ST_Sync_OUTAsync(request).Result;

                if (response.T_RETURN[0].TIPO.Equals("S") || response.T_RETURN[0].TIPO.Equals("1"))
                {
                    cargaCancelamentoCargaIntegracao.Protocolo = "00" + response.T_RETURN[0].TEXTO_MSG;
                    cargaCancelamentoCargaIntegracao.SituacaoIntegracao = SituacaoIntegracao.Integrado;
                    mensagemRetorno = $"Integrado com sucesso. {response.T_RETURN[0].DOC_CONT}";
                }
                else
                {
                    cargaCancelamentoCargaIntegracao.SituacaoIntegracao = SituacaoIntegracao.ProblemaIntegracao;
                    mensagemRetorno = $"SAP: {response.T_RETURN[0].TEXTO_MSG}";
                }
            }
            catch (ServicoException excecao)
            {
                cargaCancelamentoCargaIntegracao.SituacaoIntegracao = SituacaoIntegracao.ProblemaIntegracao;
                mensagemRetorno = excecao.Message;
            }
            catch (Exception excecao)
            {
                Log.TratarErro(excecao);

                cargaCancelamentoCargaIntegracao.SituacaoIntegracao = SituacaoIntegracao.ProblemaIntegracao;
                mensagemRetorno = "Problema ao integrar cancelamento com o SAP - ST";
            }
            servicoArquivoTransacao.Adicionar(cargaCancelamentoCargaIntegracao, inspector.LastRequestXML, inspector.LastResponseXML, "xml");
            cargaCancelamentoCargaIntegracao.ProblemaIntegracao = mensagemRetorno;
            repCargaCancelamentoCargaIntegracao.Atualizar(cargaCancelamentoCargaIntegracao);
        }

        #endregion

        #region Métodos Privados
        private Servicos.ServicoSAP_ST_Cancelamento.SI_Cancel_ST_Sync_OUTClient ObterClientCancelamento(Dominio.Entidades.Embarcador.Configuracoes.IntegracaoSAPST configuracao)
        {
            System.ServiceModel.EndpointAddress endpointAddress = new System.ServiceModel.EndpointAddress(configuracao.URLCancelamentoST);
            System.ServiceModel.BasicHttpBinding binding = new System.ServiceModel.BasicHttpBinding();

            binding.MaxReceivedMessageSize = int.MaxValue;
            binding.ReceiveTimeout = new TimeSpan(0, 5, 0);
            binding.SendTimeout = new TimeSpan(0, 5, 0);

            if (configuracao.URLCancelamentoST.StartsWith("https"))
                binding.Security.Mode = System.ServiceModel.BasicHttpSecurityMode.Transport;
            else
                binding.Security.Mode = System.ServiceModel.BasicHttpSecurityMode.TransportCredentialOnly;

            binding.Security.Transport.ClientCredentialType = System.ServiceModel.HttpClientCredentialType.Basic;

            Servicos.ServicoSAP_ST_Cancelamento.SI_Cancel_ST_Sync_OUTClient client = new Servicos.ServicoSAP_ST_Cancelamento.SI_Cancel_ST_Sync_OUTClient(binding, endpointAddress);

            client.ClientCredentials.UserName.UserName = configuracao.Usuario;
            client.ClientCredentials.UserName.Password = configuracao.Senha;

            return client;
        }


        private ServicoSAP_ST.SI_Criar_Adiantamento_Sync_OUTClient ObterClient(Dominio.Entidades.Embarcador.Configuracoes.IntegracaoSAPST configuracao)
        {
            System.ServiceModel.EndpointAddress endpointAddress = new System.ServiceModel.EndpointAddress(configuracao.URLCriarAtendimento);
            System.ServiceModel.BasicHttpBinding binding = new System.ServiceModel.BasicHttpBinding();

            binding.MaxReceivedMessageSize = int.MaxValue;
            binding.ReceiveTimeout = new TimeSpan(0, 5, 0);
            binding.SendTimeout = new TimeSpan(0, 5, 0);

            if (configuracao.URLCriarAtendimento.StartsWith("https"))
                binding.Security.Mode = System.ServiceModel.BasicHttpSecurityMode.Transport;
            else
                binding.Security.Mode = System.ServiceModel.BasicHttpSecurityMode.TransportCredentialOnly;

            binding.Security.Transport.ClientCredentialType = System.ServiceModel.HttpClientCredentialType.Basic;

            ServicoSAP_ST.SI_Criar_Adiantamento_Sync_OUTClient client = new ServicoSAP_ST.SI_Criar_Adiantamento_Sync_OUTClient(binding, endpointAddress);

            client.ClientCredentials.UserName.UserName = configuracao.Usuario;
            client.ClientCredentials.UserName.Password = configuracao.Senha;

            return client;
        }

        private ServicoSAP_ST.DT_Criar_Adiantamento_req PreencherRequisicao(Dominio.Entidades.Embarcador.Cargas.CargaCargaIntegracao integracaoCarga, Repositorio.UnitOfWork unitOfWork, Dominio.Entidades.Embarcador.Terceiros.ContratoFrete contratoFrete)
        {
            Repositorio.Embarcador.Configuracoes.IntegracaoSAPV9 repIntegracaoSAPV9 = new Repositorio.Embarcador.Configuracoes.IntegracaoSAPV9(unitOfWork);

            //string protocolo = integracaoCarga.Carga.Terceiro?.CodigoSap ?? string.Empty;
            string protocolo = repIntegracaoSAPV9.BuscarProtocoloPorCarga(integracaoCarga.Carga.Codigo, Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoIntegracao.SAP_ST);

            ServicoSAP_ST.DT_Criar_Adiantamento_req request = new ServicoSAP_ST.DT_Criar_Adiantamento_req();
            request.T_HEADER = PreencherTHeader(integracaoCarga, contratoFrete, protocolo);
            request.T_ITEM_CARUANA = PreencherTItemCaruana(integracaoCarga, contratoFrete);
            request.T_ITEM_PROP = PreencherTProp(integracaoCarga, contratoFrete);

            return request;
        }
        private Servicos.ServicoSAP_ST_Cancelamento.DT_Cancel_ST_req PreencherRequisicaoCancelamento(Dominio.Entidades.Embarcador.Cargas.CargaCancelamentoCargaIntegracao cargaCancelamentoCargaIntegracao, Repositorio.UnitOfWork unitOfWork)
        {
            Repositorio.Embarcador.Configuracoes.IntegracaoSAPV9 repIntegracaoSAPV9 = new Repositorio.Embarcador.Configuracoes.IntegracaoSAPV9(unitOfWork);
            var integracao = repIntegracaoSAPV9.BuscarProtocoloPorCargaTipo(cargaCancelamentoCargaIntegracao.CargaCancelamento.Carga.Codigo, Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoIntegracao.SAP_ST);

            List<Servicos.ServicoSAP_ST_Cancelamento.DT_Cancel_ST_reqItem> T_DOC = new List<Servicos.ServicoSAP_ST_Cancelamento.DT_Cancel_ST_reqItem>();
            T_DOC.Add(new DT_Cancel_ST_reqItem
            {
                DOC_CONT_ST = integracao.Protocolo,
                EMPRESA = cargaCancelamentoCargaIntegracao.CargaCancelamento.Carga.Empresa?.CodigoIntegracao ?? string.Empty,
                EXERCICIO = integracao.DataIntegracao.Year.ToString(),
                FORNECEDOR = "1000J027"
            });

            Servicos.ServicoSAP_ST_Cancelamento.DT_Cancel_ST_req request = new Servicos.ServicoSAP_ST_Cancelamento.DT_Cancel_ST_req();
            request.T_DOC = T_DOC.ToArray();

            return request;
        }


        private DT_Criar_Adiantamento_reqItem[] PreencherTHeader(CargaCargaIntegracao integracaoCarga, ContratoFrete contratoFrete, string protocolo)
        {
            List<DT_Criar_Adiantamento_reqItem> t_header = new List<DT_Criar_Adiantamento_reqItem>();

            t_header.Add(new ServicoSAP_ST.DT_Criar_Adiantamento_reqItem
            {
                DOC_CONT = protocolo,
                DAT_DOC = DateTime.Now,
                TIPO_DOC = "ST",
                EMPRESA = integracaoCarga.Carga.Empresa?.CodigoIntegracao ?? string.Empty,
                DATA_LANC = DateTime.Now,
                MOEDA = "BRL",
                REFERENCIA = contratoFrete?.NumeroContrato.ToString() ?? string.Empty,
                TEXTO_CAB = $"ADIA RECIBO {contratoFrete?.NumeroContrato.ToString() ?? string.Empty}",
                TEXTO_COMP = $"SALDO RECIBO {contratoFrete?.NumeroContrato.ToString() ?? string.Empty}",
                NUM_FILIAL = integracaoCarga.Carga.Empresa?.CodigoCentroCusto ?? string.Empty,
            });

            return t_header.ToArray();
        }

        private DT_Criar_Adiantamento_reqItem1[] PreencherTItemCaruana(CargaCargaIntegracao integracaoCarga, ContratoFrete contratoFrete)
        {
            List<DT_Criar_Adiantamento_reqItem1> t_header = new List<DT_Criar_Adiantamento_reqItem1>();

            Repositorio.Cliente repCliente = new Repositorio.Cliente(_unitOfWork);
            Dominio.Entidades.Cliente cliente = repCliente.BuscarPorCPFCNPJ(contratoFrete.TransportadorTerceiro.CPF_CNPJ);

            Dominio.Entidades.Embarcador.Pessoas.ModalidadeTransportadoraPessoas modalidadeTerceiro = Servicos.Embarcador.CIOT.CIOT.ObterModalidadeTransportador(contratoFrete.TransportadorTerceiro, _unitOfWork);

            t_header.Add(new DT_Criar_Adiantamento_reqItem1
            {
                CHAVE_LANC = "31",
                COD_CARTAO = "1000J027",
                MONTANTE = contratoFrete?.ValorAdiantamento ?? 0,
                LOCAL_NEG = integracaoCarga.Carga.Empresa?.CodigoCentroCusto ?? string.Empty,
                COND_PAG = "Z000",
                DATA_BASE = proximoDiaUltil(),
                BLOQ_PAG = "C",
                FORMA_PAG = "U",
                REF_PAG = contratoFrete?.NumeroContrato.ToString() ?? string.Empty,
                ATRIBUICAO = contratoFrete.TransportadorTerceiro?.CodigoIntegracao ?? string.Empty,
                BANCO_EMPRESA = integracaoCarga?.Carga?.Empresa?.Banco?.CodigoIntegracao ?? string.Empty,
                ID_CONTA = integracaoCarga?.Carga?.Empresa?.Banco?.CodigoIntegracao ?? string.Empty,
                TEXTO = $"BAIXA RECIBO {contratoFrete?.NumeroContrato.ToString() ?? string.Empty}"
            });

            return t_header.ToArray();
        }

        private DT_Criar_Adiantamento_reqItem2[] PreencherTProp(CargaCargaIntegracao integracaoCarga, ContratoFrete contratoFrete)
        {
            List<DT_Criar_Adiantamento_reqItem2> t_header = new List<DT_Criar_Adiantamento_reqItem2>();

            t_header.Add(new DT_Criar_Adiantamento_reqItem2
            {
                CHAVE_LANC = "31",
                MONTANTE = contratoFrete?.SaldoAReceber ?? 0,
                COND_PAG = "Z000",
                DATA_BASE = proximoDiaUltil(),
                BLOQ_PAG = "C",
                //FORMA_PAG = "U",
                REF_PAG = contratoFrete?.NumeroContrato.ToString() ?? string.Empty,
                ATRIBUICAO = contratoFrete.TransportadorTerceiro?.CodigoIntegracao ?? string.Empty,
                TEXTO = $"BAIXA RECIBO {contratoFrete?.NumeroContrato.ToString() ?? string.Empty}",
                LOCAL_NEGOCIO = integracaoCarga.Carga.Empresa?.CodigoCentroCusto ?? string.Empty,
                COD_PROP = contratoFrete.TransportadorTerceiro?.CodigoIntegracao ?? string.Empty
            });

            return t_header.ToArray();
        }

        private DateTime proximoDiaUltil()
        {
            DateTime data = DateTime.Now;
            Servicos.Embarcador.Configuracoes.Feriado servicoFeriado = new Configuracoes.Feriado(_unitOfWork);
            List<DateTime> datasComFeriado = servicoFeriado.ObterDatasComFeriado(data, data.AddDays(60));
            do
            {
                data = data.AddDays(1);
            } while (data.DayOfWeek == DayOfWeek.Saturday || data.DayOfWeek == DayOfWeek.Sunday || datasComFeriado.Contains(data.Date));
            return data;
        }


        #endregion
    }
}
