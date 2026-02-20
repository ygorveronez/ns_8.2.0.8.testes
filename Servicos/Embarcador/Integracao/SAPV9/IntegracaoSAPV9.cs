using Dominio.Entidades.Embarcador.Terceiros;
using Dominio.Excecoes.Embarcador;
using Dominio.ObjetosDeValor.Embarcador.Enumeradores;
using Servicos.ServicoSAP_V9;
using System;
using System.Collections.Generic;
using System.Net;

namespace Servicos.Embarcador.Integracao.SAPV9
{
    public class IntegracaoSAPV9
    {
        #region Atributos Privados Somente Leitura

        private readonly Repositorio.UnitOfWork _unitOfWork;

        #endregion

        #region Construtores

        public IntegracaoSAPV9(Repositorio.UnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        #endregion

        #region Métodos Públicos

        public void IntegrarContratoFreteTerceiro(Dominio.Entidades.Embarcador.Cargas.CargaCargaIntegracao integracaoCarga)
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
                Repositorio.Embarcador.Configuracoes.IntegracaoSAPV9 repConfiguracaoSAPV9 = new Repositorio.Embarcador.Configuracoes.IntegracaoSAPV9(_unitOfWork);
                Dominio.Entidades.Embarcador.Configuracoes.IntegracaoSAPV9 configuracaoSAPV9 = repConfiguracaoSAPV9.BuscarPrimeiroRegistro();

                if (!configuracaoSAPV9.PossuiIntegracaoSAPV9)
                    throw new ServicoException("Integração com SAP - V9 não está habilitada");

                if (string.IsNullOrWhiteSpace(configuracaoSAPV9.URLReciboFrete))
                    throw new ServicoException("Não há URL configurada para a integração com SAP - V9");

                if (string.IsNullOrWhiteSpace(configuracaoSAPV9.Usuario) || string.IsNullOrWhiteSpace(configuracaoSAPV9.Senha))
                    throw new ServicoException("Usuário e senha não configurados para integração SAP - V9");

                ServicoSAP_V9.SI_Recibo_Frete_Sync_OUTClient cliente = ObterClient(configuracaoSAPV9);

                cliente.Endpoint.EndpointBehaviors.Add(inspector);

                Servicos.ServicoSAP_V9.DT_Recibo_Frete_req request = PreencherRequisicao(integracaoCarga, _unitOfWork);
                Servicos.ServicoSAP_V9.DT_Recibo_Frete_resp response = cliente.SI_Recibo_Frete_Sync_OUT(request);

                if (response.T_RETURN[0].TIPO.Equals("E"))
                {
                    integracaoCarga.SituacaoIntegracao = SituacaoIntegracao.ProblemaIntegracao;
                    mensagemRetorno = $"SAP: {response.T_RETURN[0].NATXT}";
                }
                else
                {
                    integracaoCarga.Protocolo = response.T_RETURN[0].BELNR;
                    integracaoCarga.SituacaoIntegracao = SituacaoIntegracao.Integrado;
                    mensagemRetorno = $"Integrado com sucesso. {response.T_RETURN[0].BELNR}";
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
                mensagemRetorno = "Problema ao integrar com o SAP - V9";
            }
            servicoArquivoTransacao.Adicionar(inspector.LastRequestXML, inspector.LastResponseXML, "xml", integracaoCarga);

            integracaoCarga.ProblemaIntegracao = mensagemRetorno;
            repCargaCargaIntegracao.Atualizar(integracaoCarga);
        }
        public void IntegrarCancelamentoV9(Dominio.Entidades.Embarcador.Cargas.CargaCancelamentoCargaIntegracao cargaCancelamentoCargaIntegracao)
        {
            ServicePointManager.Expect100Continue = true;
            ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;

            Repositorio.Embarcador.Cargas.CargaCancelamentoCargaIntegracao repCargaCancelamentoCargaIntegracao = new Repositorio.Embarcador.Cargas.CargaCancelamentoCargaIntegracao(_unitOfWork);
            ArquivoTransacao<Dominio.Entidades.Embarcador.Cargas.CargaCTeIntegracaoArquivo> servicoArquivoTransacao = new ArquivoTransacao<Dominio.Entidades.Embarcador.Cargas.CargaCTeIntegracaoArquivo>(_unitOfWork);

            InspectorBehavior inspector = new InspectorBehavior();

            cargaCancelamentoCargaIntegracao.DataIntegracao = DateTime.Now;
            cargaCancelamentoCargaIntegracao.NumeroTentativas += 1;

            string mensagemRetorno = string.Empty;

            try
            {
                Repositorio.Embarcador.Configuracoes.IntegracaoSAPV9 repConfiguracaoSAPV9 = new Repositorio.Embarcador.Configuracoes.IntegracaoSAPV9(_unitOfWork);
                Dominio.Entidades.Embarcador.Configuracoes.IntegracaoSAPV9 configuracaoSAPV9 = repConfiguracaoSAPV9.BuscarPrimeiroRegistro();

                if (!configuracaoSAPV9.PossuiIntegracaoSAPV9)
                    throw new ServicoException("Integração com SAP - V9 não está habilitada");

                if (string.IsNullOrWhiteSpace(configuracaoSAPV9.URLCancelamento))
                    throw new ServicoException("Não há URL configurada para a integração cancelamento com SAP - V9");

                if (string.IsNullOrWhiteSpace(configuracaoSAPV9.Usuario) || string.IsNullOrWhiteSpace(configuracaoSAPV9.Senha))
                    throw new ServicoException("Usuário e senha não configurados para integração SAP - V9");

                Servicos.ServicoSAP_V9_Cancelar.SI_Cancel_V9_Sync_OUTClient cliente = ObterClientCancelamento(configuracaoSAPV9);

                cliente.Endpoint.EndpointBehaviors.Add(inspector);

                Servicos.ServicoSAP_V9_Cancelar.DT_Cancel_V9_req request = PreencherRequisicaoCancelamento(cargaCancelamentoCargaIntegracao, _unitOfWork);
                Servicos.ServicoSAP_V9_Cancelar.DT_Cancel_V9_resp response = cliente.SI_Cancel_V9_Sync_OUT(request);

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
                mensagemRetorno = "Problema ao integrar cancelamento com o SAP - v9";
            }
            servicoArquivoTransacao.Adicionar(cargaCancelamentoCargaIntegracao, inspector.LastRequestXML, inspector.LastResponseXML, "xml");
            cargaCancelamentoCargaIntegracao.ProblemaIntegracao = mensagemRetorno;
            repCargaCancelamentoCargaIntegracao.Atualizar(cargaCancelamentoCargaIntegracao);
        }

        #endregion

        #region Métodos Privados

        private Servicos.ServicoSAP_V9_Cancelar.SI_Cancel_V9_Sync_OUTClient ObterClientCancelamento(Dominio.Entidades.Embarcador.Configuracoes.IntegracaoSAPV9 configuracao)
        {
            System.ServiceModel.EndpointAddress endpointAddress = new System.ServiceModel.EndpointAddress(configuracao.URLCancelamento);
            System.ServiceModel.BasicHttpBinding binding = new System.ServiceModel.BasicHttpBinding();

            binding.MaxReceivedMessageSize = int.MaxValue;
            binding.ReceiveTimeout = new TimeSpan(0, 5, 0);
            binding.SendTimeout = new TimeSpan(0, 5, 0);

            if (configuracao.URLCancelamento.StartsWith("https"))
                binding.Security.Mode = System.ServiceModel.BasicHttpSecurityMode.Transport;
            else
                binding.Security.Mode = System.ServiceModel.BasicHttpSecurityMode.TransportCredentialOnly;

            binding.Security.Transport.ClientCredentialType = System.ServiceModel.HttpClientCredentialType.Basic;

            Servicos.ServicoSAP_V9_Cancelar.SI_Cancel_V9_Sync_OUTClient client = new Servicos.ServicoSAP_V9_Cancelar.SI_Cancel_V9_Sync_OUTClient(binding, endpointAddress);

            client.ClientCredentials.UserName.UserName = configuracao.Usuario;
            client.ClientCredentials.UserName.Password = configuracao.Senha;

            return client;
        }

        private Servicos.ServicoSAP_V9_Cancelar.DT_Cancel_V9_req PreencherRequisicaoCancelamento(Dominio.Entidades.Embarcador.Cargas.CargaCancelamentoCargaIntegracao cargaCancelamentoCargaIntegracao, Repositorio.UnitOfWork unitOfWork)
        {
            Repositorio.Embarcador.Configuracoes.IntegracaoSAPV9 repIntegracaoSAPV9 = new Repositorio.Embarcador.Configuracoes.IntegracaoSAPV9(unitOfWork);
            var integracao = repIntegracaoSAPV9.BuscarProtocoloPorCargaTipo(cargaCancelamentoCargaIntegracao.CargaCancelamento.Carga.Codigo, Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoIntegracao.SAP_V9);

            List<Servicos.ServicoSAP_V9_Cancelar.DT_Cancel_V9_reqItem> T_DOC = new List<Servicos.ServicoSAP_V9_Cancelar.DT_Cancel_V9_reqItem>();
            T_DOC.Add(new Servicos.ServicoSAP_V9_Cancelar.DT_Cancel_V9_reqItem
            {
                DOC_CONT_V9 = integracao.Protocolo,
                EMPRESA = cargaCancelamentoCargaIntegracao.CargaCancelamento.Carga.Empresa?.CodigoIntegracao ?? string.Empty,
                EXERCICIO = integracao.DataIntegracao.Year.ToString(),
                FORNECEDOR = "1000J027"
            });

            Servicos.ServicoSAP_V9_Cancelar.DT_Cancel_V9_req request = new Servicos.ServicoSAP_V9_Cancelar.DT_Cancel_V9_req();
            request.T_DOC = T_DOC.ToArray();

            return request;
        }
        private ServicoSAP_V9.SI_Recibo_Frete_Sync_OUTClient ObterClient(Dominio.Entidades.Embarcador.Configuracoes.IntegracaoSAPV9 configuracao)
        {
            System.ServiceModel.EndpointAddress endpointAddress = new System.ServiceModel.EndpointAddress(configuracao.URLReciboFrete);
            System.ServiceModel.BasicHttpBinding binding = new System.ServiceModel.BasicHttpBinding();

            binding.MaxReceivedMessageSize = int.MaxValue;
            binding.ReceiveTimeout = new TimeSpan(0, 5, 0);
            binding.SendTimeout = new TimeSpan(0, 5, 0);

            if (configuracao.URLReciboFrete.StartsWith("https"))
                binding.Security.Mode = System.ServiceModel.BasicHttpSecurityMode.Transport;
            else
                binding.Security.Mode = System.ServiceModel.BasicHttpSecurityMode.TransportCredentialOnly;

            binding.Security.Transport.ClientCredentialType = System.ServiceModel.HttpClientCredentialType.Basic;

            ServicoSAP_V9.SI_Recibo_Frete_Sync_OUTClient client = new ServicoSAP_V9.SI_Recibo_Frete_Sync_OUTClient(binding, endpointAddress);

            client.ClientCredentials.UserName.UserName = configuracao.Usuario;
            client.ClientCredentials.UserName.Password = configuracao.Senha;

            return client;
        }

        private Servicos.ServicoSAP_V9.DT_Recibo_Frete_req PreencherRequisicao(Dominio.Entidades.Embarcador.Cargas.CargaCargaIntegracao integracaoCarga, Repositorio.UnitOfWork unitOfWork)
        {
            Repositorio.Embarcador.Terceiros.ContratoFrete repContratoFrete = new Repositorio.Embarcador.Terceiros.ContratoFrete(unitOfWork);
            Dominio.Entidades.Embarcador.Terceiros.ContratoFrete contratoFrete = repContratoFrete.BuscarPorCarga(integracaoCarga.Carga.Codigo);

            ServicoSAP_V9.DT_Recibo_Frete_req request = new ServicoSAP_V9.DT_Recibo_Frete_req();

            request.T_ACCOUNTGL = PreencherTAccountGL(integracaoCarga.Carga, contratoFrete);
            request.T_ACCOUNTPAYABLE = PreencherTAccountPayable(integracaoCarga.Carga, contratoFrete);
            request.T_ACCOUNTWT = PreencherAccountWT(integracaoCarga.Carga, contratoFrete);
            request.T_CURRENCYAMOUNT = PreencherCurrencyAmount(contratoFrete);
            request.T_HEADER = PreencherTHeader(integracaoCarga.Carga, contratoFrete);

            return request;
        }

        private ZBRFIAPS0011[] PreencherTHeader(Dominio.Entidades.Embarcador.Cargas.Carga carga, ContratoFrete contratoFrete)
        {
            List<ZBRFIAPS0011> t_header = new List<ZBRFIAPS0011>();

            t_header.Add(new ZBRFIAPS0011
            {
                ID_SEQ = "1",
                OBSERV = $"RECIBO DE FRETE {contratoFrete?.NumeroContrato.ToString() ?? string.Empty}",
                NUM_CONTABIL = contratoFrete?.NumeroContrato.ToString() ?? string.Empty,
                NOME_LEGADO = "MultiTMS",
                BUKRS = carga.Empresa?.CodigoIntegracao ?? string.Empty,
                BLDAT = DateTime.Now.AddDays(1).ToString("yyy-MM-dd"),
                NRCGCCIC = contratoFrete.TransportadorTerceiro?.CPF_CNPJ.ToString() ?? string.Empty,
                TIPO = "C"
            });

            return t_header.ToArray();
        }

        private ZBRFIAPS0013[] PreencherCurrencyAmount(ContratoFrete contratoFrete)
        {
            List<ZBRFIAPS0013> t_currencyamount = new List<ZBRFIAPS0013>();
            t_currencyamount.Add(new ZBRFIAPS0013
            {
                ID_SEQ = "1",
                VALOR_LANC_RS =  (contratoFrete.ValorLiquidoSemAdiantamentoEImpostos * -1),
                VALOR_LANC_RSSpecified = true,
                CURRENCY = "BRL"
            });

            t_currencyamount.Add(new ZBRFIAPS0013
            {
                ID_SEQ = "1",
                VALOR_LANC_RS = contratoFrete.ValorLiquidoSemAdiantamentoEImpostos,
                VALOR_LANC_RSSpecified = true,
                CURRENCY = "BRL"
            });

            return t_currencyamount.ToArray();
        }

        private ZBRFIAPS0015[] PreencherAccountWT(Dominio.Entidades.Embarcador.Cargas.Carga carga, ContratoFrete contratoFrete)
        {
            List<ZBRFIAPS0015> t_accountwt = new List<ZBRFIAPS0015>();

            if (contratoFrete.TransportadorTerceiro.Tipo != null && contratoFrete.TransportadorTerceiro.Tipo.Equals("F"))
            {
                t_accountwt.Add(new ZBRFIAPS0015
                {
                    ID_SEQ = "1",
                    WT_TYPE = "YE",
                    WT_CODE = "E2",
                    VALOR_LANC_RS = 0,
                });
            }

            if (contratoFrete.ValorINSS > 0)
                t_accountwt.Add(new ZBRFIAPS0015
                {
                    ID_SEQ = "1",
                    WT_TYPE = "YI",
                    WT_CODE = "F1",
                    VALOR_LANC_RS = contratoFrete.ValorINSS,
                    VALOR_LANC_RSSpecified = true,
                });

            if (contratoFrete.ValorSEST > 0 || contratoFrete.ValorSENAT > 0)
                t_accountwt.Add(new ZBRFIAPS0015
                {
                    ID_SEQ = "1",
                    WT_TYPE = "YS",
                    WT_CODE = "S1",
                    VALOR_LANC_RS = contratoFrete.ValorSEST + contratoFrete.ValorSENAT,
                    VALOR_LANC_RSSpecified = true,
                });

            if (contratoFrete.ValorIRRF > 0)
                t_accountwt.Add(new ZBRFIAPS0015
                {
                    ID_SEQ = "1",
                    WT_TYPE = "YF",
                    WT_CODE = "C0",
                    VALOR_LANC_RS = contratoFrete.ValorIRRF,
                    VALOR_LANC_RSSpecified = true,
                });

            t_accountwt.Add(new ZBRFIAPS0015
            {
                VALOR_LANC_RS = 0,
            });

            return t_accountwt.ToArray();
        }

        private ZBRFIAPS0012[] PreencherTAccountPayable(Dominio.Entidades.Embarcador.Cargas.Carga carga, ContratoFrete contratoFrete)
        {
            List<ZBRFIAPS0012> t_accountpayable = new List<ZBRFIAPS0012>();

            t_accountpayable.Add(new ZBRFIAPS0012
            {
                ID_SEQ = "1",
                OBSERV = $"RECIBO DE FRETE {contratoFrete?.NumeroContrato.ToString() ?? string.Empty}",
                NUM_CONTABIL = contratoFrete?.NumeroContrato.ToString() ?? string.Empty,
                VENC_DATE = DateTime.Now.AddDays(1).ToString("yyy-MM-dd"),
                LIFNR = contratoFrete.TransportadorTerceiro?.CodigoIntegracao ?? string.Empty,
                ALLOC_NMBR = contratoFrete.TransportadorTerceiro?.CPF_CNPJ.ToString() ?? string.Empty,
                BUSINESSPLACE = carga.Empresa?.CodigoCentroCusto ?? string.Empty,
            });

            return t_accountpayable.ToArray();
        }

        private ZBRFIAPS0014[] PreencherTAccountGL(Dominio.Entidades.Embarcador.Cargas.Carga carga, ContratoFrete contratoFrete)
        {
            List<ZBRFIAPS0014> t_accontgls = new List<ZBRFIAPS0014>();

            Repositorio.Embarcador.Cargas.CargaCTe repCargaCte = new Repositorio.Embarcador.Cargas.CargaCTe(_unitOfWork);
            Repositorio.Embarcador.Cargas.CargaPedido repCargaPedido = new Repositorio.Embarcador.Cargas.CargaPedido(_unitOfWork);

            List<Dominio.Entidades.ConhecimentoDeTransporteEletronico> ctes = repCargaCte.BuscarCTePorCarga(carga.Codigo);
            Dominio.Entidades.Embarcador.Financeiro.CentroResultado centroResultado = repCargaPedido.BuscarCentroResultadoPorCarga(carga.Codigo);

            //foreach (Dominio.Entidades.ConhecimentoDeTransporteEletronico cte in ctes)
            //{
            //    t_accontgls.Add(new ZBRFIAPS0014
            //    {
            //        ID_SEQ = "1",
            //        HKONT = carga.Empresa.Tipo != null ? (carga.Empresa.Tipo.Equals("F") ? "4111301001" : "4111301003") : string.Empty,
            //        OBSERV = $"RECIBO DE FRETE {contratoFrete?.NumeroContrato.ToString() ?? string.Empty}",
            //        NUM_CONTABIL = contratoFrete?.NumeroContrato.ToString() ?? string.Empty,
            //        VENC_DATE = DateTime.Now.AddDays(1).ToString("yyy-MM-dd"),
            //        LIFNR = contratoFrete.TransportadorTerceiro?.CodigoIntegracao ?? string.Empty,
            //        ALLOC_NMBR = contratoFrete.TransportadorTerceiro?.CPF_CNPJ.ToString() ?? string.Empty,
            //        KOSTL = centroResultado?.PlanoContabilidade ?? string.Empty,
            //        PLANT = carga.Empresa?.CodigoCentroCusto ?? string.Empty,
            //    });
            //}

            t_accontgls.Add(new ZBRFIAPS0014
            {
                ID_SEQ = "1",
                HKONT = carga.Empresa.Tipo != null ? (carga.Empresa.Tipo.Equals("F") ? "4111301001" : "4111301003") : string.Empty,
                OBSERV = $"RECIBO DE FRETE {contratoFrete?.NumeroContrato.ToString() ?? string.Empty}",
                NUM_CONTABIL = contratoFrete?.NumeroContrato.ToString() ?? string.Empty,
                VENC_DATE = DateTime.Now.AddDays(1).ToString("yyy-MM-dd"),
                LIFNR = contratoFrete.TransportadorTerceiro?.CodigoIntegracao ?? string.Empty,
                ALLOC_NMBR = contratoFrete.TransportadorTerceiro?.CPF_CNPJ.ToString() ?? string.Empty,
                KOSTL = centroResultado?.PlanoContabilidade ?? string.Empty,
                PLANT = carga.Empresa?.CodigoCentroCusto ?? string.Empty,
            });

            return t_accontgls.ToArray();
        }

        #endregion
    }
}
