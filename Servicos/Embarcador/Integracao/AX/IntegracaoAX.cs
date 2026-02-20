using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading;
using Dominio.ObjetosDeValor.Embarcador.Enumeradores;
using Servicos.ServicoAX.Transportadora;
using System.ServiceModel.Description;
using System.ServiceModel.Dispatcher;
using Servicos.ServicoAX.ContratoFrete;
using Servicos.ServicoAX.Pedido;
using Servicos.ServicoAX.Compensacao;
using Servicos.ServicoAX.OrdemVenda;
using CallContext = Servicos.ServicoAX.OrdemVenda.CallContext;
using Dominio.Excecoes.Embarcador;
using Servicos.ServicoAX.Complemento;
using Servicos.ServicoAX.Cancelamento;

namespace Servicos.Embarcador.Integracao.AX
{
    public class IntegracaoAX
    {
        public static void IntegrarTransportadora(Dominio.Entidades.Embarcador.Cargas.MontagemCarga.CarregamentoIntegracao carregamentoIntegracao, Repositorio.UnitOfWork unitOfWork)
        {
            Repositorio.Embarcador.Configuracao.ConfiguracaoTMS repConfiguracao = new Repositorio.Embarcador.Configuracao.ConfiguracaoTMS(unitOfWork);
            Repositorio.Embarcador.Configuracoes.Integracao repIntegracao = new Repositorio.Embarcador.Configuracoes.Integracao(unitOfWork);
            Repositorio.Embarcador.Cargas.MontagemCarga.CarregamentoIntegracao repCarregamentoIntegracao = new Repositorio.Embarcador.Cargas.MontagemCarga.CarregamentoIntegracao(unitOfWork);
            Repositorio.Embarcador.Cargas.CargaCTeIntegracaoArquivo repCargaCTeIntegracaoArquivo = new Repositorio.Embarcador.Cargas.CargaCTeIntegracaoArquivo(unitOfWork);
            Repositorio.Cliente repCliente = new Repositorio.Cliente(unitOfWork);

            Dominio.Entidades.Embarcador.Configuracao.ConfiguracaoTMS configuracao = repConfiguracao.BuscarConfiguracaoPadrao();
            Dominio.Entidades.Embarcador.Configuracoes.Integracao integracao = repIntegracao.Buscar();

            carregamentoIntegracao.NumeroTentativas += 1;
            carregamentoIntegracao.DataIntegracao = DateTime.Now;

            if (integracao == null || string.IsNullOrWhiteSpace(integracao.URLAX))
            {
                carregamentoIntegracao.SituacaoIntegracao = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoIntegracao.ProblemaIntegracao;
                carregamentoIntegracao.ProblemaIntegracao = "Não existe configuração de integração disponível para a AX";
                repCarregamentoIntegracao.Atualizar(carregamentoIntegracao);

                return;
            }

            if (carregamentoIntegracao.Carregamento == null || carregamentoIntegracao.Carregamento.Veiculo == null || carregamentoIntegracao.Carregamento.Veiculo.Proprietario == null)
            {
                carregamentoIntegracao.SituacaoIntegracao = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoIntegracao.ProblemaIntegracao;
                carregamentoIntegracao.ProblemaIntegracao = "Este carregamento não possui veículo terceiro para realizar a sua integração, verifique no cadastro do veículo.";
                repCarregamentoIntegracao.Atualizar(carregamentoIntegracao);

                return;
            }

            string urlWebService = integracao.URLAX;
            bool situacaoIntegracao = false;
            string mensagemErro = string.Empty;
            string xmlRequest = string.Empty;
            string xmlResponse = string.Empty;
            string accountNum = string.Empty;
            bool integradoComSucesso = false;
            try
            {
                bool cadastroJaInformado = ConsultarTransportadora(carregamentoIntegracao.Carregamento.Veiculo.Proprietario, urlWebService, integracao.UsuarioAX, integracao.SenhaAX, out mensagemErro, out xmlRequest, out xmlResponse, out accountNum);

                if (!cadastroJaInformado)
                    integradoComSucesso = EnviarTransportadora(carregamentoIntegracao.Carregamento.Veiculo.Proprietario, urlWebService, integracao.UsuarioAX, integracao.SenhaAX, out mensagemErro, out xmlRequest, out xmlResponse, out accountNum);

                if (integradoComSucesso || cadastroJaInformado)
                {
                    situacaoIntegracao = true;
                    mensagemErro = string.Empty;

                    if (!string.IsNullOrWhiteSpace(accountNum))
                    {
                        carregamentoIntegracao.Carregamento.Veiculo.Proprietario.CodigoIntegracao = accountNum;
                        carregamentoIntegracao.Carregamento.Veiculo.Proprietario.DataUltimaAtualizacao = DateTime.Now;
                        carregamentoIntegracao.Carregamento.Veiculo.Proprietario.Integrado = false;
                        repCliente.Atualizar(carregamentoIntegracao.Carregamento.Veiculo.Proprietario);
                    }

                    Dominio.Entidades.Embarcador.Cargas.CargaCTeIntegracaoArquivo arquivoIntegracao = new Dominio.Entidades.Embarcador.Cargas.CargaCTeIntegracaoArquivo();
                    arquivoIntegracao.Data = carregamentoIntegracao.DataIntegracao;
                    arquivoIntegracao.Mensagem = mensagemErro;
                    arquivoIntegracao.Tipo = Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoArquivoIntegracaoCTeCarga.EnvioParaProcessamento;

                    arquivoIntegracao.ArquivoRequisicao = Servicos.Embarcador.Integracao.ArquivoIntegracao.SalvarArquivoIntegracao(xmlRequest, "xml", unitOfWork);
                    arquivoIntegracao.ArquivoResposta = Servicos.Embarcador.Integracao.ArquivoIntegracao.SalvarArquivoIntegracao(xmlResponse, "xml", unitOfWork);

                    repCargaCTeIntegracaoArquivo.Inserir(arquivoIntegracao);

                    carregamentoIntegracao.ArquivosTransacao.Add(arquivoIntegracao);
                }
                else
                {
                    if (string.IsNullOrWhiteSpace(mensagemErro))
                        mensagemErro = "Falha na integração com a AX.";
                    else
                        mensagemErro = "Retorno AX: " + mensagemErro;

                    situacaoIntegracao = false;

                    Dominio.Entidades.Embarcador.Cargas.CargaCTeIntegracaoArquivo arquivoIntegracao = new Dominio.Entidades.Embarcador.Cargas.CargaCTeIntegracaoArquivo();
                    arquivoIntegracao.Data = carregamentoIntegracao.DataIntegracao;
                    arquivoIntegracao.Mensagem = mensagemErro;
                    arquivoIntegracao.Tipo = Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoArquivoIntegracaoCTeCarga.EnvioParaProcessamento;
                    arquivoIntegracao.ArquivoRequisicao = Servicos.Embarcador.Integracao.ArquivoIntegracao.SalvarArquivoIntegracao(xmlRequest, "xml", unitOfWork);
                    arquivoIntegracao.ArquivoResposta = Servicos.Embarcador.Integracao.ArquivoIntegracao.SalvarArquivoIntegracao(xmlResponse, "xml", unitOfWork);

                    repCargaCTeIntegracaoArquivo.Inserir(arquivoIntegracao);

                    carregamentoIntegracao.ArquivosTransacao.Add(arquivoIntegracao);

                    carregamentoIntegracao.SituacaoIntegracao = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoIntegracao.ProblemaIntegracao;
                    carregamentoIntegracao.ProblemaIntegracao = mensagemErro;
                    repCarregamentoIntegracao.Atualizar(carregamentoIntegracao);
                }
            }
            catch (Exception excecao)
            {
                Servicos.Log.TratarErro(excecao, "IntegracaoAX");
                Servicos.Log.TratarErro("Request: " + xmlRequest, "IntegracaoAX");
                Servicos.Log.TratarErro("Response: " + xmlResponse, "IntegracaoAX");

                mensagemErro = "Ocorreu uma falha ao comunicar com o Serviço da AX.";
                situacaoIntegracao = false;

                Dominio.Entidades.Embarcador.Cargas.CargaCTeIntegracaoArquivo arquivoIntegracao = new Dominio.Entidades.Embarcador.Cargas.CargaCTeIntegracaoArquivo();
                arquivoIntegracao.Data = carregamentoIntegracao.DataIntegracao;
                arquivoIntegracao.Mensagem = excecao.Message.Length > 400 ? excecao.Message.Substring(0, 400) : excecao.Message;
                arquivoIntegracao.Tipo = Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoArquivoIntegracaoCTeCarga.EnvioParaProcessamento;
                arquivoIntegracao.ArquivoRequisicao = Servicos.Embarcador.Integracao.ArquivoIntegracao.SalvarArquivoIntegracao(xmlRequest, "xml", unitOfWork);
                arquivoIntegracao.ArquivoResposta = Servicos.Embarcador.Integracao.ArquivoIntegracao.SalvarArquivoIntegracao(xmlResponse, "xml", unitOfWork);

                repCargaCTeIntegracaoArquivo.Inserir(arquivoIntegracao);

                carregamentoIntegracao.ArquivosTransacao.Add(arquivoIntegracao);

                carregamentoIntegracao.SituacaoIntegracao = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoIntegracao.ProblemaIntegracao;
                carregamentoIntegracao.ProblemaIntegracao = mensagemErro;
                repCarregamentoIntegracao.Atualizar(carregamentoIntegracao);
                return;
            }

            if (!situacaoIntegracao)
            {
                carregamentoIntegracao.SituacaoIntegracao = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoIntegracao.ProblemaIntegracao;
                carregamentoIntegracao.ProblemaIntegracao = mensagemErro;
                repCarregamentoIntegracao.Atualizar(carregamentoIntegracao);
            }
            else
            {
                carregamentoIntegracao.ProblemaIntegracao = string.Empty;
                carregamentoIntegracao.SituacaoIntegracao = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoIntegracao.Integrado;
                repCarregamentoIntegracao.Atualizar(carregamentoIntegracao);
            }
        }

        public static void IntegrarContratoFrete(Dominio.Entidades.Embarcador.Cargas.CargaCargaIntegracao cargaCargaIntegracao, Repositorio.UnitOfWork unitOfWork, string mensagem = "")
        {
            Repositorio.Embarcador.Cargas.CargaDadosTransporteIntegracao repCargaDadosTransporteIntegracao = new Repositorio.Embarcador.Cargas.CargaDadosTransporteIntegracao(unitOfWork);
            Repositorio.Embarcador.Cargas.CargaCTeIntegracaoArquivo repCargaCTeIntegracaoArquivo = new Repositorio.Embarcador.Cargas.CargaCTeIntegracaoArquivo(unitOfWork);
            Repositorio.Embarcador.Configuracoes.Integracao repositorioConfiguracaoIntegracao = new Repositorio.Embarcador.Configuracoes.Integracao(unitOfWork);
            Repositorio.Embarcador.Terceiros.ContratoFrete repContratoFrete = new Repositorio.Embarcador.Terceiros.ContratoFrete(unitOfWork);
            Repositorio.Embarcador.Cargas.CargaCargaIntegracao repCargaIntegracao = new Repositorio.Embarcador.Cargas.CargaCargaIntegracao(unitOfWork);
            Repositorio.Embarcador.Cargas.CargaCTe repCargaCTe = new Repositorio.Embarcador.Cargas.CargaCTe(unitOfWork);

            //cargaCargaIntegracao.NumeroTentativas += 1;
            //cargaCargaIntegracao.DataIntegracao = DateTime.Now;
            //cargaCargaIntegracao.SituacaoIntegracao = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoIntegracao.Integrado;
            //cargaCargaIntegracao.ProblemaIntegracao = "Método ainda não disponível pela GSW.";
            //repCargaIntegracao.Atualizar(cargaCargaIntegracao);
            //return;

            Dominio.Entidades.Embarcador.Configuracoes.Integracao configuracaoIntegracao = repositorioConfiguracaoIntegracao.Buscar();
            List<Dominio.Entidades.Embarcador.Cargas.CargaCTe> cargasCTe = repCargaCTe.BuscarPorCarga(cargaCargaIntegracao.Carga.Codigo);

            cargaCargaIntegracao.NumeroTentativas += 1;
            cargaCargaIntegracao.DataIntegracao = DateTime.Now;

            if (configuracaoIntegracao == null || string.IsNullOrWhiteSpace(configuracaoIntegracao.URLAXOrdemVenda))
            {
                cargaCargaIntegracao.SituacaoIntegracao = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoIntegracao.ProblemaIntegracao;
                cargaCargaIntegracao.ProblemaIntegracao = "Não existe configuração de integração disponível para a AX";
                repCargaIntegracao.Atualizar(cargaCargaIntegracao);

                return;
            }

            string urlWebService = configuracaoIntegracao.URLAXOrdemVenda;
            bool situacaoIntegracao = false;
            string mensagemErro = string.Empty;
            string xmlRequest = string.Empty;
            string xmlResponse = string.Empty;

            try
            {
                bool integradoComSucesso = EnviarOrdemCompra(cargaCargaIntegracao.Carga, cargasCTe, configuracaoIntegracao.CNPJAX, urlWebService, configuracaoIntegracao.UsuarioAX, configuracaoIntegracao.SenhaAX, out mensagemErro, out xmlRequest, out xmlResponse, unitOfWork);
                if (integradoComSucesso)
                {
                    situacaoIntegracao = true;
                    mensagemErro = string.Empty;

                    Dominio.Entidades.Embarcador.Cargas.CargaCTeIntegracaoArquivo arquivoIntegracao = new Dominio.Entidades.Embarcador.Cargas.CargaCTeIntegracaoArquivo();
                    arquivoIntegracao.Data = cargaCargaIntegracao.DataIntegracao;
                    arquivoIntegracao.Mensagem = mensagemErro;
                    arquivoIntegracao.Tipo = Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoArquivoIntegracaoCTeCarga.EnvioParaProcessamento;

                    arquivoIntegracao.ArquivoRequisicao = Servicos.Embarcador.Integracao.ArquivoIntegracao.SalvarArquivoIntegracao(xmlRequest, "xml", unitOfWork);
                    arquivoIntegracao.ArquivoResposta = Servicos.Embarcador.Integracao.ArquivoIntegracao.SalvarArquivoIntegracao(xmlResponse, "xml", unitOfWork);

                    repCargaCTeIntegracaoArquivo.Inserir(arquivoIntegracao);

                    cargaCargaIntegracao.ArquivosTransacao.Add(arquivoIntegracao);
                }
                else
                {
                    if (string.IsNullOrWhiteSpace(mensagemErro))
                        mensagemErro = "Falha na integração com a AX.";
                    else
                        mensagemErro = "Retorno AX: " + mensagemErro;

                    situacaoIntegracao = false;

                    Dominio.Entidades.Embarcador.Cargas.CargaCTeIntegracaoArquivo arquivoIntegracao = new Dominio.Entidades.Embarcador.Cargas.CargaCTeIntegracaoArquivo();
                    arquivoIntegracao.Data = cargaCargaIntegracao.DataIntegracao;
                    arquivoIntegracao.Mensagem = mensagemErro;
                    arquivoIntegracao.Tipo = Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoArquivoIntegracaoCTeCarga.EnvioParaProcessamento;
                    arquivoIntegracao.ArquivoRequisicao = Servicos.Embarcador.Integracao.ArquivoIntegracao.SalvarArquivoIntegracao(xmlRequest, "xml", unitOfWork);
                    arquivoIntegracao.ArquivoResposta = Servicos.Embarcador.Integracao.ArquivoIntegracao.SalvarArquivoIntegracao(xmlResponse, "xml", unitOfWork);

                    repCargaCTeIntegracaoArquivo.Inserir(arquivoIntegracao);

                    cargaCargaIntegracao.ArquivosTransacao.Add(arquivoIntegracao);

                    cargaCargaIntegracao.SituacaoIntegracao = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoIntegracao.ProblemaIntegracao;
                    cargaCargaIntegracao.ProblemaIntegracao = mensagemErro;
                    repCargaIntegracao.Atualizar(cargaCargaIntegracao);
                }
            }
            catch (Exception excecao)
            {
                Servicos.Log.TratarErro(excecao, "IntegracaoAX");
                Servicos.Log.TratarErro("Request: " + xmlRequest, "IntegracaoAX");
                Servicos.Log.TratarErro("Response: " + xmlResponse, "IntegracaoAX");

                mensagemErro = "Ocorreu uma falha ao comunicar com o Serviço da AX.";
                situacaoIntegracao = false;

                Dominio.Entidades.Embarcador.Cargas.CargaCTeIntegracaoArquivo arquivoIntegracao = new Dominio.Entidades.Embarcador.Cargas.CargaCTeIntegracaoArquivo();
                arquivoIntegracao.Data = cargaCargaIntegracao.DataIntegracao;
                arquivoIntegracao.Mensagem = excecao.Message.Length > 400 ? excecao.Message.Substring(0, 400) : excecao.Message;
                arquivoIntegracao.Tipo = Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoArquivoIntegracaoCTeCarga.EnvioParaProcessamento;
                arquivoIntegracao.ArquivoRequisicao = Servicos.Embarcador.Integracao.ArquivoIntegracao.SalvarArquivoIntegracao(xmlRequest, "xml", unitOfWork);
                arquivoIntegracao.ArquivoResposta = Servicos.Embarcador.Integracao.ArquivoIntegracao.SalvarArquivoIntegracao(xmlResponse, "xml", unitOfWork);

                repCargaCTeIntegracaoArquivo.Inserir(arquivoIntegracao);

                cargaCargaIntegracao.ArquivosTransacao.Add(arquivoIntegracao);

                cargaCargaIntegracao.SituacaoIntegracao = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoIntegracao.ProblemaIntegracao;
                cargaCargaIntegracao.ProblemaIntegracao = mensagemErro;
                repCargaIntegracao.Atualizar(cargaCargaIntegracao);
                return;
            }

            if (!situacaoIntegracao)
            {
                cargaCargaIntegracao.SituacaoIntegracao = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoIntegracao.ProblemaIntegracao;
                cargaCargaIntegracao.ProblemaIntegracao = mensagemErro;
                repCargaIntegracao.Atualizar(cargaCargaIntegracao);
            }
            else
            {
                cargaCargaIntegracao.ProblemaIntegracao = string.Empty;
                cargaCargaIntegracao.SituacaoIntegracao = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoIntegracao.Integrado;
                repCargaIntegracao.Atualizar(cargaCargaIntegracao);
            }
        }

        public static void IntegrarComplementoCTe(Dominio.Entidades.Embarcador.Ocorrencias.OcorrenciaCTeIntegracao integracao, Repositorio.UnitOfWork unitOfWork)
        {
            Repositorio.Embarcador.Configuracoes.Integracao repConfiguracaoIntegracao = new Repositorio.Embarcador.Configuracoes.Integracao(unitOfWork);
            Repositorio.Embarcador.Ocorrencias.OcorrenciaCTeIntegracao repositorioOcorrenciaCTeIntegracao = new Repositorio.Embarcador.Ocorrencias.OcorrenciaCTeIntegracao(unitOfWork);

            Dominio.Entidades.Embarcador.Configuracoes.Integracao configuracaoIntegracao = repConfiguracaoIntegracao.Buscar();

            if ((configuracaoIntegracao == null) || !configuracaoIntegracao.PossuiIntegracaoAX || string.IsNullOrWhiteSpace(configuracaoIntegracao.URLAXComplemento))
            {
                integracao.ProblemaIntegracao = "Configuração para integração com o AX inválida.";
                integracao.DataIntegracao = DateTime.Now;
                integracao.NumeroTentativas++;
                integracao.SituacaoIntegracao = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoIntegracao.ProblemaIntegracao;

                return;
            }

            string mensagemErro = string.Empty;
            integracao.NumeroTentativas += 1;
            integracao.DataIntegracao = DateTime.Now;
            try
            {
                if (IntegrarComplemento(integracao, unitOfWork, configuracaoIntegracao.URLAXComplemento, configuracaoIntegracao.CNPJAX, configuracaoIntegracao.UsuarioAX, configuracaoIntegracao.SenhaAX, out mensagemErro))
                {
                    integracao.SituacaoIntegracao = SituacaoIntegracao.Integrado;
                    integracao.ProblemaIntegracao = "Integrado com sucesso";
                }
                else
                {
                    integracao.SituacaoIntegracao = SituacaoIntegracao.ProblemaIntegracao;
                    integracao.ProblemaIntegracao = mensagemErro;
                }
            }
            catch (ServicoException excecao)
            {
                integracao.SituacaoIntegracao = SituacaoIntegracao.ProblemaIntegracao;
                integracao.ProblemaIntegracao = excecao.Message;
            }
            catch (Exception excecao)
            {
                Log.TratarErro(excecao, "IntegracaoAX");

                integracao.SituacaoIntegracao = SituacaoIntegracao.ProblemaIntegracao;
                integracao.ProblemaIntegracao = "Ocorreu uma falha ao realizar a integração com a AX";
            }
            finally
            {
                repositorioOcorrenciaCTeIntegracao.Atualizar(integracao);
            }
        }

        public static bool CompensacaoContratoFrete(Dominio.Entidades.Embarcador.Terceiros.ContratoFrete contratoFrete, string urlWebServiceContrato, string urlWebService, string usuario, string senha, out string mensagemErro, Repositorio.UnitOfWork unitOfWork)
        {
            ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;

            string msgRetorno = string.Empty;
            string xmlEnvio = string.Empty;
            string xmlRetorno = string.Empty;
            bool envioIntegracaoContratoFrete = false;
            mensagemErro = string.Empty;

            if (contratoFrete.EnviouAcertoContasContratoAXComSucesso && contratoFrete.EnviouContratoAXComSucesso)
            {
                msgRetorno = "Já foi realizado a integração do contrato de terceiro com sucesso para a AX";
                mensagemErro = "Já foi realizado a integração do contrato de terceiro com sucesso para a AX";
                return false;
            }

            VendPaymSettleServiceClient proxy = new VendPaymSettleServiceClient();
            InspectorBehavior inspector = new InspectorBehavior();

            Repositorio.Embarcador.Terceiros.ContratoFrete repContratoFrete = new Repositorio.Embarcador.Terceiros.ContratoFrete(unitOfWork);
            Repositorio.Embarcador.Terceiros.ContratoFreteIntegracaoArquivo repContratoFreteIntegracaoArquivo = new Repositorio.Embarcador.Terceiros.ContratoFreteIntegracaoArquivo(unitOfWork);

            try
            {
                contratoFrete.PossuiIntegracaoAX = true;
                repContratoFrete.Atualizar(contratoFrete);

                bool integradoComSucesso = EnviarContratoFrete(contratoFrete.Carga, contratoFrete, urlWebServiceContrato, usuario, senha, out mensagemErro, out xmlEnvio, out xmlRetorno, unitOfWork, out envioIntegracaoContratoFrete);

                if (!string.IsNullOrWhiteSpace(xmlEnvio) || !string.IsNullOrWhiteSpace(xmlRetorno))
                {
                    Dominio.Entidades.Embarcador.Terceiros.ContratoFreteIntegracaoArquivo integracaoArquivo = new Dominio.Entidades.Embarcador.Terceiros.ContratoFreteIntegracaoArquivo
                    {
                        ArquivoRequisicao = Servicos.Embarcador.Integracao.ArquivoIntegracao.SalvarArquivoIntegracao(xmlEnvio, "json", unitOfWork),
                        ArquivoResposta = Servicos.Embarcador.Integracao.ArquivoIntegracao.SalvarArquivoIntegracao(xmlRetorno, "json", unitOfWork),
                        Data = DateTime.Now,
                        Tipo = Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoArquivoIntegracaoCTeCarga.RetornoDoProcessamento,
                        Mensagem = "Envio de contrato AX"
                    };
                    repContratoFreteIntegracaoArquivo.Inserir(integracaoArquivo);
                    contratoFrete.ArquivosTransacao.Add(integracaoArquivo);

                    repContratoFrete.Atualizar(contratoFrete);
                }

                if (integradoComSucesso)
                {
                    try
                    {
                        if (envioIntegracaoContratoFrete)
                            Thread.Sleep(300000);//solicitação da propria AX para enviar a segunda integração apenas 60 segundos depois

                        string[] dadosAcesso = usuario.Split('\\');
                        proxy.ClientCredentials.Windows.ClientCredential.Domain = dadosAcesso[0];
                        proxy.ClientCredentials.Windows.ClientCredential.UserName = dadosAcesso[1];
                        proxy.ClientCredentials.Windows.ClientCredential.Password = senha;

                        proxy.Endpoint.Address = new System.ServiceModel.EndpointAddress(urlWebService);
                        proxy.Endpoint.EndpointBehaviors.Add(inspector);

                        ServicoAX.Compensacao.CallContext context = new ServicoAX.Compensacao.CallContext();
                        //Empresa
                        context.Company = "nla";

                        string name = "Quitação do contrato de frete número " + Utilidades.String.OnlyNumbers(contratoFrete.NumeroContrato.ToString("n0"));
                        DateTime transDate = DateTime.Now;
                        string contaForencedor = contratoFrete.TransportadorTerceiro?.CodigoIntegracao ?? "";
                        string numeroDocumento = Utilidades.String.OnlyNumbers(contratoFrete.NumeroContrato.ToString("n0"));
                        string txt = "Quitação do contrato de frete número " + Utilidades.String.OnlyNumbers(contratoFrete.NumeroContrato.ToString("n0"));
                        decimal valorQuitacao = contratoFrete.ValorSaldoComAdiantamento + contratoFrete.ValorPedagio;
                        string paymId = Utilidades.String.OnlyNumbers(contratoFrete.Carga.Pedidos.FirstOrDefault().Pedido.Numero.ToString("n0"));

                        try
                        {
                            proxy.createVendPaymSettle(context, name, transDate, contaForencedor, txt, numeroDocumento, valorQuitacao, paymId);
                            contratoFrete.EnviouAcertoContasContratoAXComSucesso = true;
                            msgRetorno = "Integrado com sucesso.";
                            xmlEnvio = inspector.LastRequestXML;
                            xmlRetorno = inspector.LastResponseXML;

                            Dominio.Entidades.Embarcador.Terceiros.ContratoFreteIntegracaoArquivo integracaoArquivo = new Dominio.Entidades.Embarcador.Terceiros.ContratoFreteIntegracaoArquivo
                            {
                                ArquivoRequisicao = Servicos.Embarcador.Integracao.ArquivoIntegracao.SalvarArquivoIntegracao(xmlEnvio, "json", unitOfWork),
                                ArquivoResposta = Servicos.Embarcador.Integracao.ArquivoIntegracao.SalvarArquivoIntegracao(xmlRetorno, "json", unitOfWork),
                                Data = DateTime.Now,
                                Tipo = Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoArquivoIntegracaoCTeCarga.RetornoDoProcessamento,
                                Mensagem = "Envio da confirmação do contrato AX"
                            };
                            contratoFrete.ArquivosTransacao.Add(integracaoArquivo);
                            repContratoFreteIntegracaoArquivo.Inserir(integracaoArquivo);

                            repContratoFrete.Atualizar(contratoFrete);

                            Servicos.Log.TratarErro("Retorno: " + msgRetorno, "IntegracaoAX");
                            Servicos.Log.TratarErro("Request: " + xmlEnvio, "IntegracaoAX");
                            Servicos.Log.TratarErro("Response: " + xmlRetorno, "IntegracaoAX");

                            return true;
                        }
                        catch (System.ServiceModel.FaultException<ServicoAX.Compensacao.AifFault> e)
                        {
                            ServicoAX.Compensacao.InfologMessage[] messages = e.Detail.InfologMessageList;

                            xmlEnvio = inspector.LastRequestXML;
                            xmlRetorno = inspector.LastResponseXML;
                            foreach (var msg in messages)
                                msgRetorno += " " + msg.Message;

                            mensagemErro += msgRetorno;
                            Servicos.Log.TratarErro("Retorno: " + msgRetorno, "IntegracaoAX");
                            Servicos.Log.TratarErro("Request: " + xmlEnvio, "IntegracaoAX");
                            Servicos.Log.TratarErro("Response: " + xmlRetorno, "IntegracaoAX");

                            Dominio.Entidades.Embarcador.Terceiros.ContratoFreteIntegracaoArquivo integracaoArquivo = new Dominio.Entidades.Embarcador.Terceiros.ContratoFreteIntegracaoArquivo
                            {
                                ArquivoRequisicao = Servicos.Embarcador.Integracao.ArquivoIntegracao.SalvarArquivoIntegracao(xmlEnvio, "json", unitOfWork),
                                ArquivoResposta = Servicos.Embarcador.Integracao.ArquivoIntegracao.SalvarArquivoIntegracao(xmlRetorno, "json", unitOfWork),
                                Data = DateTime.Now,
                                Tipo = Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoArquivoIntegracaoCTeCarga.RetornoDoProcessamento,
                                Mensagem = "Falha no envio da confirmação do contrato AX " + mensagemErro
                            };
                            contratoFrete.ArquivosTransacao.Add(integracaoArquivo);
                            repContratoFreteIntegracaoArquivo.Inserir(integracaoArquivo);

                            repContratoFrete.Atualizar(contratoFrete);

                            return false;
                        }
                    }
                    catch (Exception excecao)
                    {
                        Servicos.Log.TratarErro("Retorno: " + excecao, "IntegracaoAX");
                        Servicos.Log.TratarErro("Request: " + xmlEnvio, "IntegracaoAX");
                        Servicos.Log.TratarErro("Response: " + xmlRetorno, "IntegracaoAX");

                        Dominio.Entidades.Embarcador.Terceiros.ContratoFreteIntegracaoArquivo integracaoArquivo = new Dominio.Entidades.Embarcador.Terceiros.ContratoFreteIntegracaoArquivo
                        {
                            ArquivoRequisicao = Servicos.Embarcador.Integracao.ArquivoIntegracao.SalvarArquivoIntegracao(xmlEnvio, "json", unitOfWork),
                            ArquivoResposta = Servicos.Embarcador.Integracao.ArquivoIntegracao.SalvarArquivoIntegracao(xmlRetorno, "json", unitOfWork),
                            Data = DateTime.Now,
                            Tipo = Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoArquivoIntegracaoCTeCarga.RetornoDoProcessamento,
                            Mensagem = "Falha no envio da confirmação do contrato AX"
                        };
                        contratoFrete.ArquivosTransacao.Add(integracaoArquivo);
                        repContratoFreteIntegracaoArquivo.Inserir(integracaoArquivo);

                        repContratoFrete.Atualizar(contratoFrete);

                        return false;
                    }
                }
                else
                {
                    if (string.IsNullOrWhiteSpace(mensagemErro))
                        mensagemErro = "Falha na integração com a AX.";
                    else
                        mensagemErro = "Retorno AX: " + mensagemErro;

                    Servicos.Log.TratarErro("Retorno: " + mensagemErro, "IntegracaoAX");
                    Servicos.Log.TratarErro("Request: " + xmlEnvio, "IntegracaoAX");
                    Servicos.Log.TratarErro("Response: " + xmlRetorno, "IntegracaoAX");

                    Dominio.Entidades.Embarcador.Terceiros.ContratoFreteIntegracaoArquivo integracaoArquivo = new Dominio.Entidades.Embarcador.Terceiros.ContratoFreteIntegracaoArquivo
                    {
                        ArquivoRequisicao = Servicos.Embarcador.Integracao.ArquivoIntegracao.SalvarArquivoIntegracao(xmlEnvio, "json", unitOfWork),
                        ArquivoResposta = Servicos.Embarcador.Integracao.ArquivoIntegracao.SalvarArquivoIntegracao(xmlRetorno, "json", unitOfWork),
                        Data = DateTime.Now,
                        Tipo = Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoArquivoIntegracaoCTeCarga.RetornoDoProcessamento,
                        Mensagem = mensagemErro
                    };
                    contratoFrete.ArquivosTransacao.Add(integracaoArquivo);
                    repContratoFreteIntegracaoArquivo.Inserir(integracaoArquivo);

                    repContratoFrete.Atualizar(contratoFrete);

                    return false;
                }
            }
            catch (Exception excecao)
            {
                Dominio.Entidades.Embarcador.Terceiros.ContratoFreteIntegracaoArquivo integracaoArquivo = new Dominio.Entidades.Embarcador.Terceiros.ContratoFreteIntegracaoArquivo
                {
                    ArquivoRequisicao = Servicos.Embarcador.Integracao.ArquivoIntegracao.SalvarArquivoIntegracao(xmlEnvio, "json", unitOfWork),
                    ArquivoResposta = Servicos.Embarcador.Integracao.ArquivoIntegracao.SalvarArquivoIntegracao(xmlRetorno, "json", unitOfWork),
                    Data = DateTime.Now,
                    Tipo = Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoArquivoIntegracaoCTeCarga.RetornoDoProcessamento,
                    Mensagem = "Falha ao enviar o contrato AX"
                };
                contratoFrete.ArquivosTransacao.Add(integracaoArquivo);
                repContratoFreteIntegracaoArquivo.Inserir(integracaoArquivo);

                repContratoFrete.Atualizar(contratoFrete);

                Servicos.Log.TratarErro("Retorno: " + excecao, "IntegracaoAX");
                Servicos.Log.TratarErro("Request: " + xmlEnvio, "IntegracaoAX");
                Servicos.Log.TratarErro("Response: " + xmlRetorno, "IntegracaoAX");

                return false;
            }
        }

        public static void IntegrarCancelamentoCTe(Dominio.Entidades.Embarcador.Cargas.CargaCancelamentoCargaIntegracao cargaCargaIntegracao, Repositorio.UnitOfWork unitOfWork)
        {
            Repositorio.Embarcador.Cargas.CargaDadosTransporteIntegracao repCargaDadosTransporteIntegracao = new Repositorio.Embarcador.Cargas.CargaDadosTransporteIntegracao(unitOfWork);
            Repositorio.Embarcador.Cargas.CargaCTeIntegracaoArquivo repCargaCTeIntegracaoArquivo = new Repositorio.Embarcador.Cargas.CargaCTeIntegracaoArquivo(unitOfWork);
            Repositorio.Embarcador.Configuracoes.Integracao repositorioConfiguracaoIntegracao = new Repositorio.Embarcador.Configuracoes.Integracao(unitOfWork);
            Repositorio.Embarcador.Cargas.CargaCancelamentoCargaIntegracao repCargaCargaCancelamentoCargaIntegracao = new Repositorio.Embarcador.Cargas.CargaCancelamentoCargaIntegracao(unitOfWork);

            Dominio.Entidades.Embarcador.Configuracoes.Integracao configuracaoIntegracao = repositorioConfiguracaoIntegracao.Buscar();

            cargaCargaIntegracao.NumeroTentativas += 1;
            cargaCargaIntegracao.DataIntegracao = DateTime.Now;

            ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;

            string endPoint = configuracaoIntegracao.URLAXCancelamento;

            bool situacaoIntegracao = false;
            string mensagemErro = string.Empty;
            string jsonRequest = string.Empty;
            string jsonResponse = string.Empty;

            try
            {
                bool integradoComSucesso = IntegrarCancelamentoCTe(cargaCargaIntegracao, unitOfWork, endPoint, configuracaoIntegracao.CNPJAX, configuracaoIntegracao.UsuarioAX, configuracaoIntegracao.SenhaAX, out mensagemErro, out jsonRequest, out jsonResponse);
                if (integradoComSucesso)
                {
                    situacaoIntegracao = true;
                    mensagemErro = string.Empty;

                    Dominio.Entidades.Embarcador.Cargas.CargaCTeIntegracaoArquivo arquivoIntegracao = new Dominio.Entidades.Embarcador.Cargas.CargaCTeIntegracaoArquivo();
                    arquivoIntegracao.Data = cargaCargaIntegracao.DataIntegracao;
                    arquivoIntegracao.Mensagem = mensagemErro;
                    arquivoIntegracao.Tipo = Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoArquivoIntegracaoCTeCarga.EnvioParaProcessamento;

                    arquivoIntegracao.ArquivoRequisicao = Servicos.Embarcador.Integracao.ArquivoIntegracao.SalvarArquivoIntegracao(jsonRequest, "json", unitOfWork);
                    arquivoIntegracao.ArquivoResposta = Servicos.Embarcador.Integracao.ArquivoIntegracao.SalvarArquivoIntegracao(jsonResponse, "json", unitOfWork);

                    repCargaCTeIntegracaoArquivo.Inserir(arquivoIntegracao);

                    cargaCargaIntegracao.ArquivosTransacao.Add(arquivoIntegracao);
                }
                else
                {
                    if (string.IsNullOrWhiteSpace(mensagemErro))
                        mensagemErro = "Falha na integração com a AX.";
                    else
                        mensagemErro = "Retorno AX: " + mensagemErro;

                    situacaoIntegracao = false;

                    Dominio.Entidades.Embarcador.Cargas.CargaCTeIntegracaoArquivo arquivoIntegracao = new Dominio.Entidades.Embarcador.Cargas.CargaCTeIntegracaoArquivo();
                    arquivoIntegracao.Data = cargaCargaIntegracao.DataIntegracao;
                    arquivoIntegracao.Mensagem = mensagemErro;
                    arquivoIntegracao.Tipo = Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoArquivoIntegracaoCTeCarga.EnvioParaProcessamento;
                    arquivoIntegracao.ArquivoRequisicao = Servicos.Embarcador.Integracao.ArquivoIntegracao.SalvarArquivoIntegracao(jsonRequest, "json", unitOfWork);
                    arquivoIntegracao.ArquivoResposta = Servicos.Embarcador.Integracao.ArquivoIntegracao.SalvarArquivoIntegracao(jsonResponse, "json", unitOfWork);

                    repCargaCTeIntegracaoArquivo.Inserir(arquivoIntegracao);

                    cargaCargaIntegracao.ArquivosTransacao.Add(arquivoIntegracao);

                    cargaCargaIntegracao.SituacaoIntegracao = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoIntegracao.ProblemaIntegracao;
                    cargaCargaIntegracao.ProblemaIntegracao = mensagemErro;
                    repCargaCargaCancelamentoCargaIntegracao.Atualizar(cargaCargaIntegracao);
                }
            }
            catch (Exception excecao)
            {
                Servicos.Log.TratarErro(excecao, "IntegracaoAX");
                Servicos.Log.TratarErro("Request: " + jsonRequest, "IntegracaoAX");
                Servicos.Log.TratarErro("Response: " + jsonResponse, "IntegracaoAX");

                mensagemErro = "Ocorreu uma falha ao comunicar com o Serviço da AX: " + excecao.Message;
                situacaoIntegracao = false;

                Dominio.Entidades.Embarcador.Cargas.CargaCTeIntegracaoArquivo arquivoIntegracao = new Dominio.Entidades.Embarcador.Cargas.CargaCTeIntegracaoArquivo();
                arquivoIntegracao.Data = cargaCargaIntegracao.DataIntegracao;
                arquivoIntegracao.Mensagem = excecao.Message.Length > 400 ? excecao.Message.Substring(0, 400) : excecao.Message;
                arquivoIntegracao.Tipo = Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoArquivoIntegracaoCTeCarga.EnvioParaProcessamento;
                arquivoIntegracao.ArquivoRequisicao = Servicos.Embarcador.Integracao.ArquivoIntegracao.SalvarArquivoIntegracao(jsonRequest, "json", unitOfWork);
                arquivoIntegracao.ArquivoResposta = Servicos.Embarcador.Integracao.ArquivoIntegracao.SalvarArquivoIntegracao(jsonResponse, "json", unitOfWork);

                repCargaCTeIntegracaoArquivo.Inserir(arquivoIntegracao);

                cargaCargaIntegracao.ArquivosTransacao.Add(arquivoIntegracao);

                cargaCargaIntegracao.SituacaoIntegracao = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoIntegracao.ProblemaIntegracao;
                cargaCargaIntegracao.ProblemaIntegracao = mensagemErro;
                repCargaCargaCancelamentoCargaIntegracao.Atualizar(cargaCargaIntegracao);
                return;
            }

            if (!situacaoIntegracao)
            {
                cargaCargaIntegracao.SituacaoIntegracao = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoIntegracao.ProblemaIntegracao;
                cargaCargaIntegracao.ProblemaIntegracao = mensagemErro;
                repCargaCargaCancelamentoCargaIntegracao.Atualizar(cargaCargaIntegracao);
            }
            else
            {
                cargaCargaIntegracao.ProblemaIntegracao = string.Empty;
                cargaCargaIntegracao.SituacaoIntegracao = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoIntegracao.Integrado;
                repCargaCargaCancelamentoCargaIntegracao.Atualizar(cargaCargaIntegracao);
            }
        }


        private static bool EnviarTransportadora(Dominio.Entidades.Cliente transportadora, string urlWebService, string usuario, string senha, out string msgRetorno, out string xmlEnvio, out string xmlRetorno, out string accountNum)
        {
            ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;

            msgRetorno = string.Empty;
            xmlEnvio = string.Empty;
            xmlRetorno = string.Empty;
            accountNum = string.Empty;
            CustomerServiceClient proxy = new CustomerServiceClient();
            InspectorBehavior inspector = new InspectorBehavior();

            string[] dadosAcesso = usuario.Split('\\');
            proxy.ClientCredentials.Windows.ClientCredential.Domain = dadosAcesso[0];
            proxy.ClientCredentials.Windows.ClientCredential.UserName = dadosAcesso[1];
            proxy.ClientCredentials.Windows.ClientCredential.Password = senha;

            proxy.Endpoint.Address = new System.ServiceModel.EndpointAddress(urlWebService);
            proxy.Endpoint.EndpointBehaviors.Add(inspector);

            ServicoAX.Transportadora.CallContext context = new ServicoAX.Transportadora.CallContext();
            context.Company = "nla";

            AxdCustomer customer = new AxdCustomer();
            AxdEntity_CustTable custTable = new AxdEntity_CustTable();

            custTable.CNPJCPFNum_BR = transportadora.CPF_CNPJ_Formatado_AX;
            custTable.CustGroup = "TRANSP";
            custTable.Currency = "BRL";
            custTable.PartyCountry = "BRA";
            custTable.IENum_BR = transportadora.IE_RG;
            custTable.State = transportadora.Localidade.Estado.Sigla;
            custTable.Memo = "MultiCTE";

            if (transportadora.Tipo == "J")
            {
                AxdEntity_DirParty_DirOrganization party = new AxdEntity_DirParty_DirOrganization();
                party.NameAlias = transportadora.Nome.Left(20);
                party.Name = transportadora.Nome;

                AxdEntity_OrganizationName organizationName = new AxdEntity_OrganizationName();
                organizationName.Name = transportadora.Nome;
                party.OrganizationName = new AxdEntity_OrganizationName[1] { organizationName };

                custTable.DirParty = new AxdEntity_DirParty_DirPartyTable[1] { party };
            }
            else
            {
                AxdEntity_DirParty_DirPerson party = new AxdEntity_DirParty_DirPerson();
                party.NameAlias = transportadora.Nome.Left(20);

                string[] nomePessoa = transportadora.Nome.Split(' ');

                AxdEntity_PersonName personName = new AxdEntity_PersonName();
                personName.FirstName = nomePessoa.Length >= 1 ? nomePessoa[0] : transportadora.Nome;
                personName.MiddleName = nomePessoa.Length >= 2 ? nomePessoa[1] : "";
                personName.LastName = nomePessoa.Length >= 3 ? nomePessoa[2] : "";

                party.PersonName = new AxdEntity_PersonName[1] { personName };
                party.NameSequence = transportadora.Nome.Left(20);

                custTable.DirParty = new AxdEntity_DirParty_DirPartyTable[1] { party };
            }

            AxdEntity_DirPartyPostalAddressView address = new AxdEntity_DirPartyPostalAddressView();
            address.LocationName = transportadora.Nome.Left(20);
            address.Street = transportadora.Endereco + ", Bairro: " + transportadora.Bairro;
            address.StreetNumber = transportadora.Numero;
            address.City = transportadora.Localidade.Descricao;
            address.State = transportadora.Localidade.Estado.Sigla;
            address.CountryRegionId = "BRA";
            address.ZipCode = transportadora.CEP;
            address.Roles = "Invoice";

            custTable.DirParty[0].DirPartyPostalAddressView = new AxdEntity_DirPartyPostalAddressView[1] { address };
            customer.CustTable = new AxdEntity_CustTable[1] { custTable };

            try
            {
                var ret = proxy.create(context, customer);
                msgRetorno = "Integrado com sucesso.";
                accountNum = ret.FirstOrDefault()?.KeyData?.FirstOrDefault()?.Value ?? "";

                xmlEnvio = inspector.LastRequestXML;
                xmlRetorno = inspector.LastResponseXML;
                return true;
            }
            catch (System.ServiceModel.FaultException<ServicoAX.Transportadora.AifFault> e)
            {
                xmlEnvio = inspector.LastRequestXML;
                xmlRetorno = inspector.LastResponseXML;
                ServicoAX.Transportadora.InfologMessage[] messages = e.Detail.InfologMessageList;
                foreach (var msg in messages)
                    msgRetorno += " " + msg.Message;
                return false;
            }
        }

        private static bool ConsultarTransportadora(Dominio.Entidades.Cliente transportadora, string urlWebService, string usuario, string senha, out string msgRetorno, out string xmlEnvio, out string xmlRetorno, out string accountNum)
        {
            ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;

            msgRetorno = string.Empty;
            xmlEnvio = string.Empty;
            xmlRetorno = string.Empty;
            accountNum = string.Empty;
            CustomerServiceClient proxy = new CustomerServiceClient();
            InspectorBehavior inspector = new InspectorBehavior();

            string[] dadosAcesso = usuario.Split('\\');
            proxy.ClientCredentials.Windows.ClientCredential.Domain = dadosAcesso[0];
            proxy.ClientCredentials.Windows.ClientCredential.UserName = dadosAcesso[1];
            proxy.ClientCredentials.Windows.ClientCredential.Password = senha;

            proxy.Endpoint.Address = new System.ServiceModel.EndpointAddress(urlWebService);
            proxy.Endpoint.EndpointBehaviors.Add(inspector);

            ServicoAX.Transportadora.CallContext context = new ServicoAX.Transportadora.CallContext();
            context.Company = "nla";

            ServicoAX.Transportadora.CriteriaElement[] criteriaElements = new ServicoAX.Transportadora.CriteriaElement[1];
            criteriaElements[0] = new ServicoAX.Transportadora.CriteriaElement();
            criteriaElements[0].DataSourceName = "CustTable";
            if (!string.IsNullOrWhiteSpace(transportadora.CodigoIntegracao))
            {
                criteriaElements[0].FieldName = "AccountNum";
                criteriaElements[0].Operator = ServicoAX.Transportadora.Operator.Equal;
                criteriaElements[0].Value1 = transportadora.CodigoIntegracao;
            }
            else
            {
                criteriaElements[0].FieldName = "CNPJCPFNum_BR";
                criteriaElements[0].Operator = ServicoAX.Transportadora.Operator.Equal;
                criteriaElements[0].Value1 = transportadora.CPF_CNPJ_Formatado_AX;
            }

            ServicoAX.Transportadora.QueryCriteria queryCriteria = new ServicoAX.Transportadora.QueryCriteria();
            queryCriteria.CriteriaElement = criteriaElements;
            try
            {
                AxdCustomer axdCustomer = proxy.find(context, queryCriteria);

                xmlEnvio = inspector.LastRequestXML;
                xmlRetorno = inspector.LastResponseXML;

                if (axdCustomer != null && axdCustomer.CustTable != null && axdCustomer.CustTable.Length > 0) // && axdCustomer.CustTable[0].DirParty != null && axdCustomer.CustTable[0].DirParty.Length > 0)
                {
                    if (axdCustomer.CustTable[0].DirParty == null || axdCustomer.CustTable[0].DirParty.Length <= 0)
                    {
                        msgRetorno = "Integrado com sucesso (já estava cadastrado).";
                        accountNum = transportadora.CodigoIntegracao;

                        xmlEnvio = inspector.LastRequestXML;
                        xmlRetorno = inspector.LastResponseXML;
                        return true;
                    }
                    ServicoAX.Transportadora.KeyField keyfield = new ServicoAX.Transportadora.KeyField() { Field = "AccountNum", Value = transportadora.CodigoIntegracao };

                    ServicoAX.Transportadora.EntityKey entityKey = new ServicoAX.Transportadora.EntityKey();
                    entityKey.KeyData = new ServicoAX.Transportadora.KeyField[1] { keyfield };
                    ServicoAX.Transportadora.EntityKey[] entityKeys = new ServicoAX.Transportadora.EntityKey[1] { entityKey };

                    axdCustomer.CustTable[0].Memo = "TSTUPD";
                    axdCustomer.CustTable[0].action = ServicoAX.Transportadora.AxdEnum_AxdEntityAction.update;
                    axdCustomer.CustTable[0].actionSpecified = true;
                    axdCustomer.CustTable[0].IENum_BR = transportadora?.IE_RG ?? "ISENTO";
                    bool juridica = Utilidades.String.OnlyNumbers(axdCustomer.CustTable[0].CNPJCPFNum_BR).Length >= 14;

                    // Pessoa física
                    dynamic dirparty;
                    if (!juridica)
                        dirparty = axdCustomer.CustTable[0].DirParty[0] as AxdEntity_DirParty_DirPerson;
                    else // Pessoa jurídica                    
                        dirparty = axdCustomer.CustTable[0].DirParty[0] as AxdEntity_DirParty_DirOrganization;

                    if (dirparty == null)
                    {
                        if (juridica)
                            dirparty = axdCustomer.CustTable[0].DirParty[0] as AxdEntity_DirParty_DirPerson;
                        else // Pessoa jurídica                    
                            dirparty = axdCustomer.CustTable[0].DirParty[0] as AxdEntity_DirParty_DirOrganization;
                    }

                    axdCustomer.CustTable[0].DirParty[0].action = ServicoAX.Transportadora.AxdEnum_AxdEntityAction.update;
                    axdCustomer.CustTable[0].DirParty[0].actionSpecified = true;

                    for (int i = 0; i < dirparty.DirPartyPostalAddressView.Length; i++)
                    {
                        dirparty.DirPartyPostalAddressView[i].action = ServicoAX.Transportadora.AxdEnum_AxdEntityAction.update;
                        dirparty.DirPartyPostalAddressView[i].actionSpecified = true;
                        dirparty.DirPartyPostalAddressView[i].updateMode = AxdEnum_ValidTimeStateUpdate.CreateNewTimePeriod;
                        dirparty.DirPartyPostalAddressView[i].updateModeSpecified = true;
                    }

                    if (dirparty.DirPartyContactInfoView != null && dirparty.DirPartyContactInfoView.Length > 0)
                    {
                        dirparty.DirPartyContactInfoView[0].action = ServicoAX.Transportadora.AxdEnum_AxdEntityAction.update;
                        dirparty.DirPartyContactInfoView[0].actionSpecified = true;
                    }

                    // Pessoa física
                    string[] nomePessoa = transportadora.Nome.Split(' ');

                    if (!juridica)
                    {
                        dirparty.PersonName[0].FirstName = nomePessoa.Length >= 1 ? nomePessoa[0] : transportadora.Nome;
                        dirparty.PersonName[0].MiddleName = nomePessoa.Length >= 2 ? nomePessoa[1] : "";
                        dirparty.PersonName[0].LastName = nomePessoa.Length >= 3 ? nomePessoa[2] : "";
                        dirparty.PersonName[0].action = ServicoAX.Transportadora.AxdEnum_AxdEntityAction.update;
                        dirparty.PersonName[0].actionSpecified = true;
                        dirparty.PersonName[0].updateMode = AxdEnum_ValidTimeStateUpdate.CreateNewTimePeriod;
                        dirparty.PersonName[0].updateModeSpecified = true;
                    }
                    else
                    {
                        // Pessoa jurídica
                        dirparty.OrganizationName[0].Name = transportadora.Nome;
                        dirparty.OrganizationName[0].action = ServicoAX.Transportadora.AxdEnum_AxdEntityAction.update;
                        dirparty.OrganizationName[0].actionSpecified = true;
                        dirparty.OrganizationName[0].updateMode = AxdEnum_ValidTimeStateUpdate.CreateNewTimePeriod;
                        dirparty.OrganizationName[0].updateModeSpecified = true;
                    }

                    //AxdEntity_DirPartyPostalAddressView address = new AxdEntity_DirPartyPostalAddressView();
                    dirparty.DirPartyPostalAddressView[0].LocationName = transportadora.Nome.Left(20);
                    dirparty.DirPartyPostalAddressView[0].Street = transportadora.Endereco + ", Bairro: " + transportadora.Bairro;
                    dirparty.DirPartyPostalAddressView[0].StreetNumber = transportadora.Numero;
                    dirparty.DirPartyPostalAddressView[0].City = transportadora.Localidade.Descricao;
                    dirparty.DirPartyPostalAddressView[0].State = transportadora.Localidade.Estado.Sigla;
                    dirparty.DirPartyPostalAddressView[0].CountryRegionId = "BRA";
                    dirparty.DirPartyPostalAddressView[0].ZipCode = transportadora.CEP;
                    dirparty.DirPartyPostalAddressView[0].Roles = "Invoice";
                    dirparty.DirPartyPostalAddressView[0].action = ServicoAX.Transportadora.AxdEnum_AxdEntityAction.update;
                    dirparty.DirPartyPostalAddressView[0].actionSpecified = true;
                    dirparty.DirPartyPostalAddressView[0].updateMode = AxdEnum_ValidTimeStateUpdate.CreateNewTimePeriod;
                    dirparty.DirPartyPostalAddressView[0].updateModeSpecified = true;

                    //dirparty.DirPartyPostalAddressView = new AxdEntity_DirPartyPostalAddressView[1] { address };
                    dirparty.NameAlias = transportadora.Nome.Left(20);
                    dirparty.Name = transportadora.Nome.Left(100);

                    try
                    {
                        proxy.update(context, entityKeys, axdCustomer);

                        msgRetorno = "Integrado com sucesso (já estava cadastrado).";
                        accountNum = transportadora.CodigoIntegracao;

                        xmlEnvio = inspector.LastRequestXML;
                        xmlRetorno = inspector.LastResponseXML;
                        return true;
                    }
                    catch (System.ServiceModel.FaultException<ServicoAX.Transportadora.AifFault> e)
                    {
                        ServicoAX.Transportadora.InfologMessage[] messages = e.Detail.InfologMessageList;

                        foreach (var msg in messages)
                            msgRetorno += " " + msg.Message;

                        xmlEnvio = inspector.LastRequestXML;
                        xmlRetorno = inspector.LastResponseXML;

                        return false;
                    }
                }
                msgRetorno = "Cadastro não localizado.";
                xmlEnvio = inspector.LastRequestXML;
                xmlRetorno = inspector.LastResponseXML;

                return false;
            }
            catch (System.ServiceModel.FaultException<ServicoAX.Transportadora.AifFault> e)
            {
                xmlEnvio = inspector.LastRequestXML;
                xmlRetorno = inspector.LastResponseXML;
                ServicoAX.Transportadora.InfologMessage[] messages = e.Detail.InfologMessageList;
                foreach (var msg in messages)
                    msgRetorno += " " + msg.Message;
                return false;
            }
        }

        private static bool EnviarOrdemCompra(Dominio.Entidades.Embarcador.Cargas.Carga carga, List<Dominio.Entidades.Embarcador.Cargas.CargaCTe> cargasCtes, string cnpjEmpresaAX, string urlWebService, string usuario, string senha, out string msgRetorno, out string xmlEnvio, out string xmlRetorno, Repositorio.UnitOfWork unitOfWork)
        {
            msgRetorno = string.Empty;
            xmlEnvio = string.Empty;
            xmlRetorno = string.Empty;

            ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;

            GSWSalesTableIntegrationServiceClient proxy = new GSWSalesTableIntegrationServiceClient();
            InspectorBehavior inspector = new InspectorBehavior();

            string[] dadosAcesso = usuario.Split('\\');
            proxy.ClientCredentials.Windows.ClientCredential.Domain = dadosAcesso[0];
            proxy.ClientCredentials.Windows.ClientCredential.UserName = dadosAcesso[1];
            proxy.ClientCredentials.Windows.ClientCredential.Password = senha;

            proxy.Endpoint.Address = new System.ServiceModel.EndpointAddress(urlWebService);
            proxy.Endpoint.EndpointBehaviors.Add(inspector);

            CallContext context = new CallContext();
            context.Company = "NLA";

            AxdGSWSalesTableIntegration gSWSalesTable = new AxdGSWSalesTableIntegration();
            AxdEntity_SalesTable SalesTable = new AxdEntity_SalesTable();
            AxdEntity_SalesTable_BR salesTable_br = new AxdEntity_SalesTable_BR();
            List<AxdEntity_GSWSalesTableIntegrationMultiCTE> listGSwSaleTable = new List<AxdEntity_GSWSalesTableIntegrationMultiCTE>();
            List<AxdEntity_SalesLine> listSalesLine = new List<AxdEntity_SalesLine>();
            List<AxdEntity_SalesTable_BR> listSalesTable_br = new List<AxdEntity_SalesTable_BR>();

            foreach (var cargaCTe in cargasCtes)
            {
                Dominio.Entidades.DocumentosCTE documentoCTE = cargaCTe.CTe.Documentos?.FirstOrDefault();
                listGSwSaleTable.Add(new AxdEntity_GSWSalesTableIntegrationMultiCTE()
                {
                    AccountCNPJCPF = cargaCTe.CTe?.TomadorPagador?.CPF_CNPJ_Formatado_AX ?? "001486815-65",
                    IENum_BR = cargaCTe.CTe?.TomadorPagador?.IE_RG ?? "ISENTO",
                    InvoiceAccessKey = cargaCTe.CTe.Chave,
                    InvoiceDate = cargaCTe.CTe.DataEmissao,
                    InvoiceModel = cargaCTe.CTe.ModeloDocumentoFiscal.Numero,
                    InvoiceNumber = Utilidades.String.OnlyNumbers(cargaCTe.CTe.Numero.ToString("n0")),
                    InvoiceSeries = Utilidades.String.OnlyNumbers(cargaCTe.CTe.Serie.Numero.ToString("n0")),
                    SiteCNPJCPF = cargaCTe.CTe?.Empresa?.CPF_CNPJ_Formatado_AX ?? cnpjEmpresaAX,
                    ReasonCode = "",
                    OperationTypeID_BR = !string.IsNullOrWhiteSpace(carga.TipoOperacao?.CodigoIntegracao ?? "") ? carga.TipoOperacao?.CodigoIntegracao ?? "" : "TrMultiCTE",
                    InvoiceAccessKeyRef = !string.IsNullOrWhiteSpace(documentoCTE.ChaveNFE) ? documentoCTE.ChaveNFE : documentoCTE.NumeroOuNumeroDaChave,
                    InvoiceModelRef = !string.IsNullOrWhiteSpace(documentoCTE.ChaveNFE) ? "55" : "01",
                    InvoiceNumberRef = documentoCTE.NumeroOuNumeroDaChave,
                    InvoiceSeriesRef = documentoCTE.SerieOuSerieDaChave
                });
            }

            listSalesTable_br.Add(new AxdEntity_SalesTable_BR()
            {
                SalesPurchOperationType_BR = "0"
            });

            foreach (var cargaCTe in cargasCtes)
            {
                AxdEntity_SalesLine salesLine = new AxdEntity_SalesLine()
                {
                    ItemId = "00002846",//"00002846",
                    SalesPrice = (cargaCTe.CTe?.Peso ?? 0m) > 0 ? Math.Round(cargaCTe.CTe.ValorFrete / (cargaCTe.CTe?.Peso ?? 0m), 7, MidpointRounding.ToEven) : Math.Round((carga.Pedidos.FirstOrDefault().Pedido.ValorFreteToneladaNegociado / 1000), 7, MidpointRounding.ToEven), //carga.Pedidos.FirstOrDefault().Pedido.ValorFreteToneladaNegociado / 1000,  //cargaCTe.CTe.ValorFrete / 1000,
                    SalesQty = (cargaCTe.CTe?.Peso ?? 0m),// / 1000,
                    SalesQtySpecified = true,
                    SalesPriceSpecified = true,
                    SalesType = AxdEnum_SalesType.Sales
                };

                ServicoAX.OrdemVenda.AxdType_DimensionAttributeValue dimCostCenter = new ServicoAX.OrdemVenda.AxdType_DimensionAttributeValue();
                dimCostCenter.Name = "Centros_de_Custos"; //Fixo
                dimCostCenter.Value = carga.Pedidos.FirstOrDefault().Pedido.CentroResultado?.Descricao ?? "701";//"321"; // valor centro de custo

                ServicoAX.OrdemVenda.AxdType_DimensionAttributeValue dimOperationSegments = new ServicoAX.OrdemVenda.AxdType_DimensionAttributeValue();
                dimOperationSegments.Name = "Operating Segments (Produto)";// FIXO
                dimOperationSegments.Value = carga.Pedidos.FirstOrDefault().Produtos?.FirstOrDefault()?.Produto?.CodigoDocumentacao ?? "MIL";//"321"; // valor centro de custo//"MIL";// VALORES DEPENDENDO DO PROCESSO

                ServicoAX.OrdemVenda.AxdType_DimensionAttributeValue dimBusinessUnit = new ServicoAX.OrdemVenda.AxdType_DimensionAttributeValue();
                dimBusinessUnit.Name = "Safra"; // fixo
                dimBusinessUnit.Value = carga.Pedidos.FirstOrDefault().Pedido.Safra;// "15/16"; // valor da safra

                ServicoAX.OrdemVenda.AxdType_DimensionAttributeValueSet valueSet = new ServicoAX.OrdemVenda.AxdType_DimensionAttributeValueSet();

                if (!string.IsNullOrWhiteSpace(dimCostCenter.Value) && !string.IsNullOrWhiteSpace(dimBusinessUnit.Value) && !string.IsNullOrWhiteSpace(dimOperationSegments.Value))
                {
                    valueSet.Values = new ServicoAX.OrdemVenda.AxdType_DimensionAttributeValue[3] { dimCostCenter, dimOperationSegments, dimBusinessUnit };
                    salesLine.DefaultDimension = valueSet;
                }
                else if (!string.IsNullOrWhiteSpace(dimCostCenter.Value) && !string.IsNullOrWhiteSpace(dimBusinessUnit.Value))
                {
                    valueSet.Values = new ServicoAX.OrdemVenda.AxdType_DimensionAttributeValue[2] { dimCostCenter, dimBusinessUnit };
                    salesLine.DefaultDimension = valueSet;
                }
                else if (!string.IsNullOrWhiteSpace(dimCostCenter.Value) && !string.IsNullOrWhiteSpace(dimOperationSegments.Value))
                {
                    valueSet.Values = new ServicoAX.OrdemVenda.AxdType_DimensionAttributeValue[2] { dimCostCenter, dimOperationSegments };
                    salesLine.DefaultDimension = valueSet;
                }
                else if (!string.IsNullOrWhiteSpace(dimBusinessUnit.Value) && !string.IsNullOrWhiteSpace(dimOperationSegments.Value))
                {
                    valueSet.Values = new ServicoAX.OrdemVenda.AxdType_DimensionAttributeValue[2] { dimBusinessUnit, dimOperationSegments };
                    salesLine.DefaultDimension = valueSet;
                }
                else if (!string.IsNullOrWhiteSpace(dimCostCenter.Value))
                {
                    valueSet.Values = new ServicoAX.OrdemVenda.AxdType_DimensionAttributeValue[1] { dimCostCenter };
                    salesLine.DefaultDimension = valueSet;
                }
                else if (!string.IsNullOrWhiteSpace(dimOperationSegments.Value))
                {
                    valueSet.Values = new ServicoAX.OrdemVenda.AxdType_DimensionAttributeValue[1] { dimOperationSegments };
                    salesLine.DefaultDimension = valueSet;
                }
                else if (!string.IsNullOrWhiteSpace(dimBusinessUnit.Value))
                {
                    valueSet.Values = new ServicoAX.OrdemVenda.AxdType_DimensionAttributeValue[1] { dimBusinessUnit };
                    salesLine.DefaultDimension = valueSet;
                }

                listSalesLine.Add(salesLine);
            }

            SalesTable.Payment = "10 DDL";
            SalesTable.SalesType = AxdEnum_SalesType.Sales;
            SalesTable.CTEOriginStateId = cargasCtes.FirstOrDefault().CTe.LocalidadeInicioPrestacao.Estado.Sigla;
            SalesTable.CTEOriginCityName = cargasCtes.FirstOrDefault().CTe.LocalidadeInicioPrestacao.Descricao;
            SalesTable.CTEDestStateId = cargasCtes.FirstOrDefault().CTe.LocalidadeTerminoPrestacao.Estado.Sigla;
            SalesTable.CTEDestCityName = cargasCtes.FirstOrDefault().CTe.LocalidadeTerminoPrestacao.Descricao;

            SalesTable.GSWSalesTableIntegrationMultiCTE = listGSwSaleTable.ToArray();
            SalesTable.SalesTable_BR = listSalesTable_br.ToArray();
            SalesTable.SalesLine = listSalesLine.ToArray();

            gSWSalesTable.SalesTable = new AxdEntity_SalesTable[1] { SalesTable };

            try
            {
                var ret = proxy.create(context, gSWSalesTable);

                msgRetorno = "Integrado com sucesso.";
                xmlEnvio = inspector.LastRequestXML;
                xmlRetorno = inspector.LastResponseXML;

                Servicos.Log.TratarErro("Retorno: " + msgRetorno, "IntegracaoAX");
                Servicos.Log.TratarErro("Request: " + xmlEnvio, "IntegracaoAX");
                Servicos.Log.TratarErro("Response: " + xmlRetorno, "IntegracaoAX");

                return true;
            }
            catch (System.ServiceModel.FaultException<ServicoAX.ContratoFrete.AifFault> e)
            {
                xmlEnvio = inspector.LastRequestXML;
                xmlRetorno = inspector.LastResponseXML;

                Servicos.Log.TratarErro("Retorno: " + msgRetorno, "IntegracaoAX");
                Servicos.Log.TratarErro("Request: " + xmlEnvio, "IntegracaoAX");
                Servicos.Log.TratarErro("Response: " + xmlRetorno, "IntegracaoAX");

                ServicoAX.ContratoFrete.InfologMessage[] messages = e.Detail.InfologMessageList;
                foreach (var msg in messages)
                    msgRetorno += " " + msg.Message;
                return false;
            }
        }

        private static bool EnviarContratoFrete(Dominio.Entidades.Embarcador.Cargas.Carga carga, Dominio.Entidades.Embarcador.Terceiros.ContratoFrete contratoFrete, string urlWebService, string usuario, string senha, out string msgRetorno, out string xmlEnvio, out string xmlRetorno, Repositorio.UnitOfWork unitOfWork, out bool envioIntegracao)
        {
            msgRetorno = string.Empty;
            xmlEnvio = string.Empty;
            xmlRetorno = string.Empty;
            envioIntegracao = false;

            if (contratoFrete.EnviouContratoAXComSucesso)
                return true;

            ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;
            Repositorio.Embarcador.Terceiros.ContratoFrete repContratoFrete = new Repositorio.Embarcador.Terceiros.ContratoFrete(unitOfWork);

            PurchaseInvoiceServiceClient proxy = new Servicos.Embarcador.Integracao.ConfiguracaoWebService(unitOfWork).ObterClient<PurchaseInvoiceServiceClient, PurchaseInvoiceService>(TipoWebServiceIntegracao.AX_PurchaseInvoiceService, out Servicos.Models.Integracao.InspectorBehavior inspector);

            string[] dadosAcesso = usuario.Split('\\');
            proxy.ClientCredentials.Windows.ClientCredential.Domain = dadosAcesso[0];
            proxy.ClientCredentials.Windows.ClientCredential.UserName = dadosAcesso[1];
            proxy.ClientCredentials.Windows.ClientCredential.Password = senha;

            proxy.Endpoint.Address = new System.ServiceModel.EndpointAddress(urlWebService);

            ServicoAX.ContratoFrete.CallContext context = new ServicoAX.ContratoFrete.CallContext();
            context.Company = "nla";

            AxdPurchaseInvoice ledgerGeneralJournal = new AxdPurchaseInvoice();

            //Cabeçalho
            AxdEntity_LedgerJournalTable ledgerJournalTable = new AxdEntity_LedgerJournalTable();

            ledgerJournalTable.JournalName = "TMS Recibo";
            ledgerJournalTable.Name = "Contrato de Frete " + Utilidades.String.OnlyNumbers(contratoFrete.NumeroContrato.ToString("n0"));//Utilidades.String.OnlyNumbers(carga.Pedidos.FirstOrDefault().Pedido.Numero.ToString("n0"));
            ledgerJournalTable.MultiCTEIntegrated = ServicoAX.ContratoFrete.AxdExtType_MultiCTEIntegrated.Yes;
            ledgerJournalTable.MultiCTEIntegratedSpecified = true;
            ServicoAX.ContratoFrete.AxdType_DimensionAttributeValue dimCostCenter = new ServicoAX.ContratoFrete.AxdType_DimensionAttributeValue();
            dimCostCenter.Name = "Centros_de_Custos"; //Fixo
            dimCostCenter.Value = carga.Pedidos.FirstOrDefault().Pedido.CentroResultado?.Descricao ?? "701";//"321"; // valor centro de custo

            ServicoAX.ContratoFrete.AxdType_DimensionAttributeValue dimOperationSegments = new ServicoAX.ContratoFrete.AxdType_DimensionAttributeValue();
            dimOperationSegments.Name = "Operating Segments (Produto)";// FIXO
            dimOperationSegments.Value = carga.Pedidos.FirstOrDefault().Produtos?.FirstOrDefault()?.Produto?.CodigoDocumentacao ?? "MIL";//"321"; // valor centro de custo//"MIL";// VALORES DEPENDENDO DO PROCESSO

            ServicoAX.ContratoFrete.AxdType_DimensionAttributeValue dimBusinessUnit = new ServicoAX.ContratoFrete.AxdType_DimensionAttributeValue();
            dimBusinessUnit.Name = "Safra"; // fixo
            dimBusinessUnit.Value = carga.Pedidos.FirstOrDefault().Pedido.Safra;// "15/16"; // valor da safra

            ServicoAX.ContratoFrete.AxdType_DimensionAttributeValueSet valueSet = new ServicoAX.ContratoFrete.AxdType_DimensionAttributeValueSet();

            if (!string.IsNullOrWhiteSpace(dimCostCenter.Value) && !string.IsNullOrWhiteSpace(dimBusinessUnit.Value) && !string.IsNullOrWhiteSpace(dimOperationSegments.Value))
            {
                valueSet.Values = new ServicoAX.ContratoFrete.AxdType_DimensionAttributeValue[3] { dimCostCenter, dimOperationSegments, dimBusinessUnit };
                ledgerJournalTable.DefaultDimension = valueSet;
            }
            else if (!string.IsNullOrWhiteSpace(dimCostCenter.Value) && !string.IsNullOrWhiteSpace(dimBusinessUnit.Value))
            {
                valueSet.Values = new ServicoAX.ContratoFrete.AxdType_DimensionAttributeValue[2] { dimCostCenter, dimBusinessUnit };
                ledgerJournalTable.DefaultDimension = valueSet;
            }
            else if (!string.IsNullOrWhiteSpace(dimCostCenter.Value) && !string.IsNullOrWhiteSpace(dimOperationSegments.Value))
            {
                valueSet.Values = new ServicoAX.ContratoFrete.AxdType_DimensionAttributeValue[2] { dimCostCenter, dimOperationSegments };
                ledgerJournalTable.DefaultDimension = valueSet;
            }
            else if (!string.IsNullOrWhiteSpace(dimBusinessUnit.Value) && !string.IsNullOrWhiteSpace(dimOperationSegments.Value))
            {
                valueSet.Values = new ServicoAX.ContratoFrete.AxdType_DimensionAttributeValue[2] { dimBusinessUnit, dimOperationSegments };
                ledgerJournalTable.DefaultDimension = valueSet;
            }
            else if (!string.IsNullOrWhiteSpace(dimCostCenter.Value))
            {
                valueSet.Values = new ServicoAX.ContratoFrete.AxdType_DimensionAttributeValue[1] { dimCostCenter };
                ledgerJournalTable.DefaultDimension = valueSet;
            }
            else if (!string.IsNullOrWhiteSpace(dimOperationSegments.Value))
            {
                valueSet.Values = new ServicoAX.ContratoFrete.AxdType_DimensionAttributeValue[1] { dimOperationSegments };
                ledgerJournalTable.DefaultDimension = valueSet;
            }
            else if (!string.IsNullOrWhiteSpace(dimBusinessUnit.Value))
            {
                valueSet.Values = new ServicoAX.ContratoFrete.AxdType_DimensionAttributeValue[1] { dimBusinessUnit };
                ledgerJournalTable.DefaultDimension = valueSet;
            }

            //Linha fornecedor 
            AxdEntity_LedgerJournalTrans ledgerJournalTrans = new AxdEntity_LedgerJournalTrans();
            ledgerJournalTrans.PurchLedgerPosting = ServicoAX.ContratoFrete.AxdExtType_PurchLedgerPosting.Vendor;
            ledgerJournalTrans.PurchLedgerPostingSpecified = true;
            ledgerJournalTrans.AccountType = ServicoAX.ContratoFrete.AxdEnum_LedgerJournalACType.Cust;
            ledgerJournalTrans.AccountTypeSpecified = true;
            ServicoAX.ContratoFrete.AxdType_MultiTypeAccount multiTypeAccount = new ServicoAX.ContratoFrete.AxdType_MultiTypeAccount();
            multiTypeAccount.Account = contratoFrete.TransportadorTerceiro.CodigoIntegracao;
            multiTypeAccount.DisplayValue = contratoFrete.TransportadorTerceiro.CodigoIntegracao;
            ledgerJournalTrans.LedgerDimension = multiTypeAccount;

            ledgerJournalTrans.TaxGroup = !string.IsNullOrWhiteSpace(contratoFrete.CodigoIntegracaoTributaria) ? contratoFrete.CodigoIntegracaoTributaria : "E000006D";
            ledgerJournalTrans.TaxItemGroup = !string.IsNullOrWhiteSpace(contratoFrete.CodigoIntegracaoTributaria) ? contratoFrete.CodigoIntegracaoTributaria : "E000006D";

            //Campos
            ledgerJournalTrans.MultiCTEIntegrationId = Utilidades.String.OnlyNumbers(contratoFrete.NumeroContrato.ToString("n0"));
            ledgerJournalTrans.TransDate = contratoFrete.DataEmissaoContrato;
            ledgerJournalTrans.DocumentDate = contratoFrete.DataEmissaoContrato;
            ledgerJournalTrans.Invoice = Utilidades.String.OnlyNumbers(contratoFrete.NumeroContrato.ToString("n0"));
            ledgerJournalTrans.DocumentNum = Utilidades.String.OnlyNumbers(contratoFrete.NumeroContrato.ToString("n0"));
            ledgerJournalTrans.Due = contratoFrete.DataEmissaoContrato;
            ledgerJournalTrans.Company = "nla";
            ledgerJournalTrans.Txt = "Contrato de Frete " + Utilidades.String.OnlyNumbers(contratoFrete.NumeroContrato.ToString("n0"));//Utilidades.String.OnlyNumbers(carga.Pedidos.FirstOrDefault().Pedido.Numero.ToString("n0"));
            ledgerJournalTrans.AmountCurCredit = contratoFrete.ValorSaldoComAdiantamento + contratoFrete.ValorPedagio;// contratoFrete.ValorSaldoComAdiantamento;
            ledgerJournalTrans.AmountCurCreditSpecified = true;
            ledgerJournalTrans.Prepayment = ServicoAX.ContratoFrete.AxdEnum_NoYes.Yes;
            ledgerJournalTrans.PrepaymentSpecified = true;
            ledgerJournalTrans.PaymId = Utilidades.String.OnlyNumbers(carga.Pedidos.FirstOrDefault().Pedido.Numero.ToString("n0"));

            //ledgerJournalTable.LedgerJournalTrans = new AxdEntity_LedgerJournalTrans[1] { ledgerJournalTrans };
            //ledgerGeneralJournal.LedgerJournalTable = new AxdEntity_LedgerJournalTable[1] { ledgerJournalTable };

            //Linha fornecedor Serviço transporte
            AxdEntity_LedgerJournalTrans ledgerJournalTransSrv = new AxdEntity_LedgerJournalTrans();
            ledgerJournalTransSrv.PurchLedgerPosting = ServicoAX.ContratoFrete.AxdExtType_PurchLedgerPosting.Vendor;
            ledgerJournalTransSrv.PurchLedgerPostingSpecified = true;
            ledgerJournalTransSrv.AccountType = ServicoAX.ContratoFrete.AxdEnum_LedgerJournalACType.Ledger;
            ledgerJournalTransSrv.AccountTypeSpecified = true;
            multiTypeAccount = new ServicoAX.ContratoFrete.AxdType_MultiTypeAccount();
            multiTypeAccount.Account = "3.1.01.01.001";
            multiTypeAccount.DisplayValue = "3.1.01.01.001";

            ledgerJournalTransSrv.LedgerDimension = multiTypeAccount;

            ledgerJournalTransSrv.TaxItemGroup = !string.IsNullOrWhiteSpace(contratoFrete.CodigoIntegracaoTributaria) ? contratoFrete.CodigoIntegracaoTributaria : "ETRANSP04";
            ledgerJournalTransSrv.TaxGroup = !string.IsNullOrWhiteSpace(contratoFrete.CodigoIntegracaoTributaria) ? contratoFrete.CodigoIntegracaoTributaria : "ETRANSP04";

            ledgerJournalTransSrv.MultiCTEIntegrationId = Utilidades.String.OnlyNumbers(contratoFrete.NumeroContrato.ToString("n0"));
            ledgerJournalTransSrv.TransDate = contratoFrete.DataEmissaoContrato;
            ledgerJournalTransSrv.DocumentDate = contratoFrete.DataEmissaoContrato;
            ledgerJournalTransSrv.Invoice = Utilidades.String.OnlyNumbers(contratoFrete.NumeroContrato.ToString("n0"));
            ledgerJournalTransSrv.DocumentNum = Utilidades.String.OnlyNumbers(contratoFrete.NumeroContrato.ToString("n0"));
            ledgerJournalTransSrv.Due = contratoFrete.DataEmissaoContrato;
            ledgerJournalTransSrv.Company = "nla";
            ledgerJournalTransSrv.Txt = "Serviço de transporte " + Utilidades.String.OnlyNumbers(contratoFrete.NumeroContrato.ToString("n0"));//Utilidades.String.OnlyNumbers(carga.Pedidos.FirstOrDefault().Pedido.Numero.ToString("n0"));
            ledgerJournalTransSrv.AmountCurDebit = contratoFrete.ValorSaldoComAdiantamento;
            ledgerJournalTransSrv.AmountCurDebitSpecified = true;
            ledgerJournalTransSrv.Prepayment = ServicoAX.ContratoFrete.AxdEnum_NoYes.Yes;
            ledgerJournalTransSrv.PrepaymentSpecified = true;

            //Linha fornecedor Frete
            AxdEntity_LedgerJournalTrans ledgerJournalTransFreight = new AxdEntity_LedgerJournalTrans();
            ledgerJournalTransFreight.AccountType = ServicoAX.ContratoFrete.AxdEnum_LedgerJournalACType.Ledger;
            ledgerJournalTransFreight.AccountTypeSpecified = true;
            multiTypeAccount = new ServicoAX.ContratoFrete.AxdType_MultiTypeAccount();
            multiTypeAccount.Account = "3.1.01.01.001";
            multiTypeAccount.DisplayValue = "3.1.01.01.001";

            ledgerJournalTransFreight.LedgerDimension = multiTypeAccount;

            //Campos
            ledgerJournalTransFreight.MultiCTEIntegrationId = Utilidades.String.OnlyNumbers(contratoFrete.NumeroContrato.ToString("n0"));
            ledgerJournalTransFreight.TransDate = DateTime.Now;
            ledgerJournalTransFreight.DocumentDate = DateTime.Now;
            ledgerJournalTransFreight.Invoice = Utilidades.String.OnlyNumbers(contratoFrete.NumeroContrato.ToString("n0"));
            ledgerJournalTransFreight.DocumentNum = Utilidades.String.OnlyNumbers(contratoFrete.NumeroContrato.ToString("n0"));
            ledgerJournalTransFreight.Due = contratoFrete.DataEmissaoContrato;
            ledgerJournalTransFreight.Company = "nla";
            ledgerJournalTransFreight.Txt = "Frete " + Utilidades.String.OnlyNumbers(contratoFrete.NumeroContrato.ToString("n0"));//Utilidades.String.OnlyNumbers(carga.Pedidos.FirstOrDefault().Pedido.Numero.ToString("n0"));;
            ledgerJournalTransFreight.AmountCurDebit = contratoFrete.ValorPedagio;// contratoFrete.ValorSaldoComAdiantamento;
            ledgerJournalTransFreight.AmountCurDebitSpecified = true;
            ledgerJournalTransFreight.Prepayment = ServicoAX.ContratoFrete.AxdEnum_NoYes.Yes;
            ledgerJournalTransFreight.PrepaymentSpecified = true;

            ledgerJournalTable.LedgerJournalTrans = new AxdEntity_LedgerJournalTrans[3] { ledgerJournalTrans, ledgerJournalTransSrv, ledgerJournalTransFreight };
            ledgerGeneralJournal.LedgerJournalTable = new AxdEntity_LedgerJournalTable[1] { ledgerJournalTable };

            try
            {
                var ret = proxy.create(context, ledgerGeneralJournal);
                contratoFrete.EnviouContratoAXComSucesso = true;
                repContratoFrete.Atualizar(contratoFrete);

                msgRetorno = "Integrado com sucesso.";
                xmlEnvio = inspector.LastRequestXML;
                xmlRetorno = inspector.LastResponseXML;

                Servicos.Log.TratarErro("Retorno: " + msgRetorno, "IntegracaoAX");
                Servicos.Log.TratarErro("Request: " + xmlEnvio, "IntegracaoAX");
                Servicos.Log.TratarErro("Response: " + xmlRetorno, "IntegracaoAX");
                envioIntegracao = true;
                return true;
            }
            catch (System.ServiceModel.FaultException<ServicoAX.ContratoFrete.AifFault> e)
            {
                xmlEnvio = inspector.LastRequestXML;
                xmlRetorno = inspector.LastResponseXML;

                Servicos.Log.TratarErro("Retorno: " + msgRetorno, "IntegracaoAX");
                Servicos.Log.TratarErro("Request: " + xmlEnvio, "IntegracaoAX");
                Servicos.Log.TratarErro("Response: " + xmlRetorno, "IntegracaoAX");

                ServicoAX.ContratoFrete.InfologMessage[] messages = e.Detail.InfologMessageList;
                foreach (var msg in messages)
                    msgRetorno += " " + msg.Message;
                return false;
            }
        }

        public static void EnviarPedido(ref Dominio.Entidades.Embarcador.Pedidos.PedidoIntegracao pedidoIntegracao, Repositorio.UnitOfWork unitOfWork, string urlWebService, string usuario, string senha)
        {
            Repositorio.Embarcador.Pedidos.PedidoIntegracaoArquivo repPedidoIntegracaoArquivo = new Repositorio.Embarcador.Pedidos.PedidoIntegracaoArquivo(unitOfWork);
            Repositorio.Embarcador.Configuracoes.Integracao repositorioConfiguracaoIntegracao = new Repositorio.Embarcador.Configuracoes.Integracao(unitOfWork);
            Repositorio.Embarcador.Pedidos.Pedido repPedido = new Repositorio.Embarcador.Pedidos.Pedido(unitOfWork);
            Dominio.Entidades.Embarcador.Configuracoes.Integracao configuracaoIntegracao = repositorioConfiguracaoIntegracao.Buscar();

            pedidoIntegracao.Tentativas += 1;
            pedidoIntegracao.DataEnvio = DateTime.Now;

            ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;

            bool situacaoIntegracao = false;
            string mensagemErro = string.Empty;
            string xmlEnvio = string.Empty;
            string xmlRetorno = string.Empty;

            try
            {
                GSWVendPaymJournalServiceClient proxy = new GSWVendPaymJournalServiceClient();
                InspectorBehavior inspector = new InspectorBehavior();

                string[] dadosAcesso = usuario.Split('\\');
                proxy.ClientCredentials.Windows.ClientCredential.Domain = dadosAcesso[0];
                proxy.ClientCredentials.Windows.ClientCredential.UserName = dadosAcesso[1];
                proxy.ClientCredentials.Windows.ClientCredential.Password = senha;

                proxy.Endpoint.Address = new System.ServiceModel.EndpointAddress(urlWebService);
                proxy.Endpoint.EndpointBehaviors.Add(inspector);

                ServicoAX.Pedido.CallContext context = new ServicoAX.Pedido.CallContext();

                //Empresa
                context.Company = "nla";

                //Conta de rede
                AxdGSWVendPaymJournal vendPaymJournal = new AxdGSWVendPaymJournal();

                //Cabeçalho
                AxdEntity_LedgerJournalTable_1 ledgerJournalTable = new AxdEntity_LedgerJournalTable_1();

                ledgerJournalTable.JournalName = "TMS-O";
                ledgerJournalTable.Name = "Criação de pedido de oferta " + Utilidades.String.OnlyNumbers(pedidoIntegracao.Pedido.Numero.ToString("n0"));
                ledgerJournalTable.MultiCTEIntegrated = ServicoAX.Pedido.AxdExtType_MultiCTEIntegrated.Yes;
                ledgerJournalTable.MultiCTEIntegratedSpecified = true;
                ledgerJournalTable.JournalType = ServicoAX.Pedido.AxdEnum_LedgerJournalType.Payment;
                ledgerJournalTable.JournalTypeSpecified = true;

                //Linha
                AxdEntity_LedgerJournalTrans_1 ledgerJournalTrans = new AxdEntity_LedgerJournalTrans_1();

                //Tipo de Linha
                ledgerJournalTrans.PurchLedgerPosting = ServicoAX.Pedido.AxdExtType_PurchLedgerPosting.Vendor;
                ledgerJournalTrans.PurchLedgerPostingSpecified = true;

                //Fornecedor
                ledgerJournalTrans.AccountType = ServicoAX.Pedido.AxdEnum_LedgerJournalACType.Vend;
                ledgerJournalTrans.AccountTypeSpecified = true;

                ServicoAX.Pedido.AxdType_MultiTypeAccount multiTypeAccount = new ServicoAX.Pedido.AxdType_MultiTypeAccount();
                multiTypeAccount.Account = "000001";
                multiTypeAccount.DisplayValue = "000001";
                ledgerJournalTrans.LedgerDimension = multiTypeAccount;

                //Campos
                ledgerJournalTrans.TransDate = DateTime.Now;
                ledgerJournalTrans.DocumentDate = DateTime.Now;
                //ledgerJournalTrans.Invoice = "111111";
                ledgerJournalTrans.DocumentNum = Utilidades.String.OnlyNumbers(pedidoIntegracao.Pedido.Numero.ToString("n0"));
                ledgerJournalTrans.Due = DateTime.Now;
                ledgerJournalTrans.Company = "nla";
                ledgerJournalTrans.Txt = "Criação de pedido de oferta " + Utilidades.String.OnlyNumbers(pedidoIntegracao.Pedido.Numero.ToString("n0"));
                //ledgerJournalTrans.AmountCurDebit = (pedidoIntegracao.Pedido.PesoTotal > 0 ? (pedidoIntegracao.Pedido.PesoTotal / 1000) : pedidoIntegracao.Pedido.PesoTotal) * pedidoIntegracao.Pedido.ValorFreteTransportadorTerceiro;
                //ledgerJournalTrans.AmountCurDebitSpecified = true;
                ledgerJournalTrans.AmountCurCreditSpecified = true;
                ledgerJournalTrans.AmountCurCredit = (pedidoIntegracao.Pedido.PesoTotal > 0 ? (pedidoIntegracao.Pedido.PesoTotal / 1000) : pedidoIntegracao.Pedido.PesoTotal) * pedidoIntegracao.Pedido.ValorFreteToneladaTerceiro;
                ledgerJournalTrans.Prepayment = ServicoAX.Pedido.AxdEnum_NoYes.Yes;
                ledgerJournalTrans.PrepaymentSpecified = false;
                ledgerJournalTrans.PaymId = Utilidades.String.OnlyNumbers(pedidoIntegracao.Pedido.Numero.ToString("n0"));

                ledgerJournalTrans.OffsetAccountType = ServicoAX.Pedido.AxdEnum_LedgerJournalACType.Bank;
                ledgerJournalTrans.OffsetAccountTypeSpecified = true;

                ledgerJournalTable.LedgerJournalTrans_1 = new AxdEntity_LedgerJournalTrans_1[1] { ledgerJournalTrans };
                vendPaymJournal.LedgerJournalTable_1 = new AxdEntity_LedgerJournalTable_1[1] { ledgerJournalTable };

                try
                {
                    var ret = proxy.create(context, vendPaymJournal);
                    mensagemErro = "Integrado com sucesso.";
                    xmlEnvio = inspector.LastRequestXML;
                    xmlRetorno = inspector.LastResponseXML;

                    situacaoIntegracao = true;

                    Dominio.Entidades.Embarcador.Pedidos.PedidoIntegracaoArquivo arquivoIntegracao = new Dominio.Entidades.Embarcador.Pedidos.PedidoIntegracaoArquivo();
                    arquivoIntegracao.Data = pedidoIntegracao.DataEnvio.Value;
                    arquivoIntegracao.Mensagem = mensagemErro;
                    arquivoIntegracao.Tipo = Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoArquivoIntegracaoCTeCarga.EnvioParaProcessamento;

                    arquivoIntegracao.ArquivoRequisicao = Servicos.Embarcador.Integracao.ArquivoIntegracao.SalvarArquivoIntegracao(xmlEnvio, "xml", unitOfWork);
                    arquivoIntegracao.ArquivoResposta = Servicos.Embarcador.Integracao.ArquivoIntegracao.SalvarArquivoIntegracao(xmlRetorno, "xml", unitOfWork);

                    repPedidoIntegracaoArquivo.Inserir(arquivoIntegracao);

                    pedidoIntegracao.ArquivosTransacao.Add(arquivoIntegracao);

                }
                catch (System.ServiceModel.FaultException<ServicoAX.Pedido.AifFault> e)
                {
                    xmlEnvio = inspector.LastRequestXML;
                    xmlRetorno = inspector.LastResponseXML;
                    ServicoAX.Pedido.InfologMessage[] messages = e.Detail.InfologMessageList;
                    foreach (var msg in messages)
                        mensagemErro += " " + msg.Message;

                    situacaoIntegracao = false;

                    Dominio.Entidades.Embarcador.Pedidos.PedidoIntegracaoArquivo arquivoIntegracao = new Dominio.Entidades.Embarcador.Pedidos.PedidoIntegracaoArquivo();
                    arquivoIntegracao.Data = pedidoIntegracao.DataEnvio.Value;
                    arquivoIntegracao.Mensagem = mensagemErro;
                    arquivoIntegracao.Tipo = Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoArquivoIntegracaoCTeCarga.EnvioParaProcessamento;
                    arquivoIntegracao.ArquivoRequisicao = Servicos.Embarcador.Integracao.ArquivoIntegracao.SalvarArquivoIntegracao(xmlEnvio, "xml", unitOfWork);
                    arquivoIntegracao.ArquivoResposta = Servicos.Embarcador.Integracao.ArquivoIntegracao.SalvarArquivoIntegracao(xmlRetorno, "xml", unitOfWork);

                    repPedidoIntegracaoArquivo.Inserir(arquivoIntegracao);

                    pedidoIntegracao.ArquivosTransacao.Add(arquivoIntegracao);

                    pedidoIntegracao.SituacaoIntegracao = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoIntegracao.ProblemaIntegracao;
                    pedidoIntegracao.ProblemaIntegracao = mensagemErro;
                }
            }
            catch (Exception excecao)
            {
                Servicos.Log.TratarErro(excecao, "IntegracaoAX");
                Servicos.Log.TratarErro("Request: " + xmlEnvio, "IntegracaoAX");
                Servicos.Log.TratarErro("Response: " + xmlRetorno, "IntegracaoAX");

                mensagemErro = "Ocorreu uma falha ao comunicar com o Serviço da AX.";
                situacaoIntegracao = false;

                Dominio.Entidades.Embarcador.Pedidos.PedidoIntegracaoArquivo arquivoIntegracao = new Dominio.Entidades.Embarcador.Pedidos.PedidoIntegracaoArquivo();
                arquivoIntegracao.Data = pedidoIntegracao.DataEnvio.Value;
                arquivoIntegracao.Mensagem = excecao.Message.Length > 400 ? excecao.Message.Substring(0, 400) : excecao.Message;
                arquivoIntegracao.Tipo = Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoArquivoIntegracaoCTeCarga.EnvioParaProcessamento;
                arquivoIntegracao.ArquivoRequisicao = Servicos.Embarcador.Integracao.ArquivoIntegracao.SalvarArquivoIntegracao(xmlEnvio, "xml", unitOfWork);
                arquivoIntegracao.ArquivoResposta = Servicos.Embarcador.Integracao.ArquivoIntegracao.SalvarArquivoIntegracao(xmlRetorno, "xml", unitOfWork);

                repPedidoIntegracaoArquivo.Inserir(arquivoIntegracao);

                pedidoIntegracao.ArquivosTransacao.Add(arquivoIntegracao);

                pedidoIntegracao.SituacaoIntegracao = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoIntegracao.ProblemaIntegracao;
                pedidoIntegracao.ProblemaIntegracao = mensagemErro;
                return;
            }

            if (!situacaoIntegracao)
            {
                pedidoIntegracao.SituacaoIntegracao = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoIntegracao.ProblemaIntegracao;
                pedidoIntegracao.ProblemaIntegracao = mensagemErro;
            }
            else
            {
                pedidoIntegracao.ProblemaIntegracao = string.Empty;
                pedidoIntegracao.SituacaoIntegracao = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoIntegracao.Integrado;
            }
        }

        private static bool IntegrarComplemento(Dominio.Entidades.Embarcador.Ocorrencias.OcorrenciaCTeIntegracao integracao, Repositorio.UnitOfWork unitOfWork, string urlWebService, string cnpjEmpresaAX, string usuario, string senha, out string msgRetorno)
        {
            msgRetorno = string.Empty;
            string xmlEnvio = string.Empty;
            string xmlRetorno = string.Empty;
            bool complementoICMS = integracao.CargaOcorrencia.ComponenteFrete.TipoComponenteFrete == TipoComponenteFrete.ICMS;
            ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;

            Repositorio.Embarcador.Terceiros.ContratoFrete repContratoFrete = new Repositorio.Embarcador.Terceiros.ContratoFrete(unitOfWork);
            Repositorio.ConhecimentoDeTransporteEletronico repCTe = new Repositorio.ConhecimentoDeTransporteEletronico(unitOfWork);
            Repositorio.Embarcador.Ocorrencias.OcorrenciaCTeIntegracaoArquivo repositorioOcorrenciaCTeIntegracaoArquivo = new Repositorio.Embarcador.Ocorrencias.OcorrenciaCTeIntegracaoArquivo(unitOfWork);

            MultiCTECompSalesInvServiceClient proxy = new MultiCTECompSalesInvServiceClient();
            InspectorBehavior inspector = new InspectorBehavior();

            string[] dadosAcesso = usuario.Split('\\');
            proxy.ClientCredentials.Windows.ClientCredential.Domain = dadosAcesso[0];
            proxy.ClientCredentials.Windows.ClientCredential.UserName = dadosAcesso[1];
            proxy.ClientCredentials.Windows.ClientCredential.Password = senha;

            proxy.Endpoint.Address = new System.ServiceModel.EndpointAddress(urlWebService);
            proxy.Endpoint.EndpointBehaviors.Add(inspector);

            ServicoAX.Complemento.CallContext context = new ServicoAX.Complemento.CallContext();
            context.Company = "nla";

            AxdMultiCTEComplSalesInv multiCTESalesCompl = new AxdMultiCTEComplSalesInv();

            AxdEntity_SalesCompInvoice salesComplInvoice = new AxdEntity_SalesCompInvoice()
            {
                ComplementaryType = complementoICMS ? AxdEnum_InvoiceComplementaryType_BR.Tax : AxdEnum_InvoiceComplementaryType_BR.Price,
                ComplementaryTypeSpecified = true,
                WSYSAccountCNPJCPF = !string.IsNullOrWhiteSpace(cnpjEmpresaAX) ? cnpjEmpresaAX : "090772520016-70"
            };

            AxdEntity_SalesCompInvLine salesComplInvoiceLine = new AxdEntity_SalesCompInvLine();

            if (!complementoICMS)
            {
                salesComplInvoiceLine.ItemCode = "00002846";
                salesComplInvoiceLine.Amount = integracao.CargaOcorrencia.ValorOcorrencia;
                salesComplInvoiceLine.AmountSpecified = true;
            }
            else
            {
                salesComplInvoiceLine.AdditionalICMSAmount = integracao.CargaOcorrencia.ValorICMS;
                salesComplInvoiceLine.AdditionalICMSAmountSpecified = true;
                salesComplInvoiceLine.AdditionalIPIAmount = 0;
                salesComplInvoiceLine.AdditionalIPIAmountSpecified = false;
                salesComplInvoiceLine.AdditionalICMSSTAmount = 0;
                salesComplInvoiceLine.AdditionalICMSSTAmountSpecified = false;
            }

            Dominio.Entidades.ConhecimentoDeTransporteEletronico cteOriginal = repCTe.BuscarPorChave(integracao.CargaCTe.CTe.ChaveCTESubComp);

            AxdEntity_CTESalesCompInvoice ctesalesComplInvoice = new AxdEntity_CTESalesCompInvoice()
            {
                SiteCNPJ = !string.IsNullOrWhiteSpace(cnpjEmpresaAX) ? cnpjEmpresaAX : "090772520001-93",

                AccessKey = integracao.CargaCTe.CTe.Chave,//chave do cte complementar
                Model = integracao.CargaCTe.CTe.ModeloDocumentoFiscal.Numero,//série do CTe complementar
                Number = Utilidades.String.OnlyNumbers(integracao.CargaCTe.CTe.Numero.ToString("n0")),//numero do cte complementar

                RefAccessKey = cteOriginal?.Chave ?? "",//chave do cte original
                RefNumber = Utilidades.String.OnlyNumbers(cteOriginal?.Numero.ToString("n0") ?? ""),//numero do cte original
                Series = Utilidades.String.OnlyNumbers(cteOriginal?.Serie.Numero.ToString("n0") ?? "")//serie do cte original
            };

            List<AxdEntity_SalesCompInvoice> listsalesComplInvoice = new List<AxdEntity_SalesCompInvoice>();
            List<AxdEntity_SalesCompInvLine> listsalesComplInvoiceLine = new List<AxdEntity_SalesCompInvLine>();
            List<AxdEntity_CTESalesCompInvoice> listctesalesComplInvoice = new List<AxdEntity_CTESalesCompInvoice>();

            listsalesComplInvoiceLine.Add(salesComplInvoiceLine);
            listctesalesComplInvoice.Add(ctesalesComplInvoice);

            salesComplInvoice.CTESalesCompInvoice = listctesalesComplInvoice.ToArray();
            salesComplInvoice.SalesCompInvLine = listsalesComplInvoiceLine.ToArray();

            multiCTESalesCompl.SalesCompInvoice = new AxdEntity_SalesCompInvoice[1] { salesComplInvoice };

            try
            {
                MultiCTECompSalesInvServiceCreateRequest request = new MultiCTECompSalesInvServiceCreateRequest(context, multiCTESalesCompl);
                proxy.create(request.CallContext, multiCTESalesCompl);
                //proxy.create(context, multiCTESalesCompl);

                msgRetorno = "Integrado com sucesso.";
                xmlEnvio = inspector.LastRequestXML;
                xmlRetorno = inspector.LastResponseXML;

                Dominio.Entidades.Embarcador.Ocorrencias.OcorrenciaCTeIntegracaoArquivo arquivoIntegracao = new Dominio.Entidades.Embarcador.Ocorrencias.OcorrenciaCTeIntegracaoArquivo()
                {
                    ArquivoRequisicao = ArquivoIntegracao.SalvarArquivoIntegracao(xmlEnvio, "xml", unitOfWork),
                    ArquivoResposta = ArquivoIntegracao.SalvarArquivoIntegracao(xmlRetorno, "xml", unitOfWork),
                    Data = integracao.DataIntegracao,
                    Mensagem = msgRetorno,
                    Tipo = TipoArquivoIntegracaoCTeCarga.EnvioParaProcessamento,
                };

                repositorioOcorrenciaCTeIntegracaoArquivo.Inserir(arquivoIntegracao);

                if (integracao.ArquivosTransacao == null)
                    integracao.ArquivosTransacao = new List<Dominio.Entidades.Embarcador.Ocorrencias.OcorrenciaCTeIntegracaoArquivo>();

                integracao.ArquivosTransacao.Add(arquivoIntegracao);

                Servicos.Log.TratarErro("Retorno: " + msgRetorno, "IntegracaoAX");
                Servicos.Log.TratarErro("Request: " + xmlEnvio, "IntegracaoAX");
                Servicos.Log.TratarErro("Response: " + xmlRetorno, "IntegracaoAX");

                return true;
            }
            catch (System.ServiceModel.FaultException<ServicoAX.ContratoFrete.AifFault> e)
            {
                xmlEnvio = inspector.LastRequestXML;
                xmlRetorno = inspector.LastResponseXML;

                Servicos.Log.TratarErro("Retorno: " + msgRetorno, "IntegracaoAX");
                Servicos.Log.TratarErro("Request: " + xmlEnvio, "IntegracaoAX");
                Servicos.Log.TratarErro("Response: " + xmlRetorno, "IntegracaoAX");

                ServicoAX.ContratoFrete.InfologMessage[] messages = e.Detail.InfologMessageList;
                foreach (var msg in messages)
                    msgRetorno += " " + msg.Message;

                Dominio.Entidades.Embarcador.Ocorrencias.OcorrenciaCTeIntegracaoArquivo arquivoIntegracao = new Dominio.Entidades.Embarcador.Ocorrencias.OcorrenciaCTeIntegracaoArquivo()
                {
                    ArquivoRequisicao = ArquivoIntegracao.SalvarArquivoIntegracao(xmlEnvio, "xml", unitOfWork),
                    ArquivoResposta = ArquivoIntegracao.SalvarArquivoIntegracao(xmlRetorno, "xml", unitOfWork),
                    Data = integracao.DataIntegracao,
                    Mensagem = msgRetorno,
                    Tipo = TipoArquivoIntegracaoCTeCarga.EnvioParaProcessamento,
                };

                repositorioOcorrenciaCTeIntegracaoArquivo.Inserir(arquivoIntegracao);

                if (integracao.ArquivosTransacao == null)
                    integracao.ArquivosTransacao = new List<Dominio.Entidades.Embarcador.Ocorrencias.OcorrenciaCTeIntegracaoArquivo>();

                integracao.ArquivosTransacao.Add(arquivoIntegracao);

                return false;
            }
        }

        private static bool IntegrarCancelamentoCTe(Dominio.Entidades.Embarcador.Cargas.CargaCancelamentoCargaIntegracao cargaCancelamento, Repositorio.UnitOfWork unitOfWork, string urlWebService, string cnpjEmpresaAX, string usuario, string senha, out string msgRetorno, out string jsonRequest, out string jsonResponse)
        {
            ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;

            msgRetorno = string.Empty;
            jsonRequest = string.Empty;
            jsonResponse = string.Empty;
            bool integracaoRealizadaComSucesso = true;

            Repositorio.Embarcador.Cargas.CargaCTe repCargaCTe = new Repositorio.Embarcador.Cargas.CargaCTe(unitOfWork);
            List<Dominio.Entidades.ConhecimentoDeTransporteEletronico> ctes = repCargaCTe.BuscarCTePorCarga(cargaCancelamento.CargaCancelamento.Carga.Codigo);

            foreach (var cte in ctes)
            {
                if (cte.Status != "C" && cte.Status != "Z")
                    continue;

                GSWMultiCTECancelServiceClient proxy = new Servicos.Embarcador.Integracao.ConfiguracaoWebService(unitOfWork).ObterClient<GSWMultiCTECancelServiceClient, GSWMultiCTECancelService>(Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoWebServiceIntegracao.AX_GSWMultiCTECancelService, out Servicos.Models.Integracao.InspectorBehavior inspector);

                string[] dadosAcesso = usuario.Split('\\');
                proxy.ClientCredentials.Windows.ClientCredential.Domain = dadosAcesso[0];
                proxy.ClientCredentials.Windows.ClientCredential.UserName = dadosAcesso[1];
                proxy.ClientCredentials.Windows.ClientCredential.Password = senha;

                proxy.Endpoint.Address = new System.ServiceModel.EndpointAddress(urlWebService);

                ServicoAX.Cancelamento.CallContext context = new ServicoAX.Cancelamento.CallContext();
                //Empresa
                context.Company = "nla";

                string chaveCTe = cte.Chave;
                try
                {
                    GSWMultiCTECancelServiceCancelCTERequest request = new GSWMultiCTECancelServiceCancelCTERequest(context, chaveCTe);
                    proxy.cancelCTEAsync(request.CallContext, chaveCTe);

                    //proxy.cancelCTE(context, chaveCTe);

                    msgRetorno += " Cancelamento do CT-e " + chaveCTe + " integrado com sucesso.";
                    jsonRequest += inspector.LastRequestXML;
                    jsonResponse += inspector.LastResponseXML;
                }
                catch (System.ServiceModel.FaultException<ServicoAX.Cancelamento.AifFault> e)
                {
                    ServicoAX.Cancelamento.InfologMessage[] messages = e.Detail.InfologMessageList;

                    string mensagemErro = "";
                    jsonRequest += inspector.LastRequestXML;
                    jsonResponse += inspector.LastResponseXML;
                    foreach (var msg in messages)
                        mensagemErro += " " + msg.Message;

                    msgRetorno += " Cancelamento do CT-e " + chaveCTe + " integrado com falha: " + mensagemErro;
                    Servicos.Log.TratarErro("Retorno: " + msgRetorno, "IntegracaoAX");
                    Servicos.Log.TratarErro("Request: " + inspector.LastRequestXML, "IntegracaoAX");
                    Servicos.Log.TratarErro("Response: " + inspector.LastResponseXML, "IntegracaoAX");

                    integracaoRealizadaComSucesso = false;
                }
            }

            return integracaoRealizadaComSucesso;

        }

        public class InspectorBehavior : IEndpointBehavior
        {
            public string LastRequestXML
            {
                get
                {
                    return myMessageInspector.LastRequestXML;
                }
            }

            public string LastResponseXML
            {
                get
                {
                    return myMessageInspector.LastResponseXML;
                }
            }


            private MyMessageInspector myMessageInspector = new MyMessageInspector();
            public void AddBindingParameters(ServiceEndpoint endpoint, System.ServiceModel.Channels.BindingParameterCollection bindingParameters)
            {

            }

            public void ApplyDispatchBehavior(ServiceEndpoint endpoint, EndpointDispatcher endpointDispatcher)
            {

            }

            public void Validate(ServiceEndpoint endpoint)
            {

            }


            public void ApplyClientBehavior(ServiceEndpoint endpoint, ClientRuntime clientRuntime)
            {
                clientRuntime.ClientMessageInspectors.Add(myMessageInspector);
            }
        }

        public class MyMessageInspector : IClientMessageInspector
        {
            public string LastRequestXML { get; private set; }
            public string LastResponseXML { get; private set; }
            public void AfterReceiveReply(ref System.ServiceModel.Channels.Message reply, object correlationState)
            {
                LastResponseXML = reply.ToString();
            }

            public object BeforeSendRequest(ref System.ServiceModel.Channels.Message request, System.ServiceModel.IClientChannel channel)
            {
                LastRequestXML = request.ToString();
                return request;
            }
        }
    }
}
