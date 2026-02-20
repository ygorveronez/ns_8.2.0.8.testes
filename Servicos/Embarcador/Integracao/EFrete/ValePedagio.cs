using Dominio.Excecoes.Embarcador;
using Dominio.ObjetosDeValor.Embarcador.Enumeradores;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;

namespace Servicos.Embarcador.Integracao.EFrete
{
    public class ValePedagio
    {
        #region Atributos Globais

        private readonly Repositorio.UnitOfWork _unitOfWork;

        #endregion

        #region Construtores

        public ValePedagio(Repositorio.UnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        #endregion

        #region Métodos Públicos

        public void GerarCompraValePedagio(Dominio.Entidades.Embarcador.Cargas.ValePedagio.CargaIntegracaoValePedagio cargaValePedagio, Dominio.Entidades.Embarcador.Cargas.Carga carga, AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware tipoServicoMultisoftware)
        {
            Servicos.Log.TratarErro("---", "IntegracaoEFreteVP");
            Servicos.Log.TratarErro($"GerarCompraValePedagio - Gerando compra da carga {carga.Codigo} - VP: {cargaValePedagio.Codigo}", "IntegracaoEFreteVP");

            Frota.ValePedagio servicoValePedagio = new Frota.ValePedagio(_unitOfWork);
            Carga.ValePedagio.ValePedagio servicoCargaValePedagio = new Carga.ValePedagio.ValePedagio(_unitOfWork);

            Dominio.Entidades.Embarcador.Configuracoes.IntegracaoEFrete integracaoEFrete = servicoValePedagio.ObterIntegracaoEFrete(carga, tipoServicoMultisoftware);

            string token = Login(integracaoEFrete, cargaValePedagio);
            if (string.IsNullOrWhiteSpace(token))
                return;

            Dominio.Entidades.Embarcador.Configuracoes.IntegracaoGeralEFrete configuracaoIntegracaoEFrete = new Repositorio.Embarcador.Configuracoes.IntegracaoGeralEFrete(_unitOfWork).Buscar();
            if (configuracaoIntegracaoEFrete?.VersaoEFrete == VersaoEFreteEnum.Versao2)
            {
                Servicos.Log.TratarErro($"GerarCompraValePedagio - Usando V2", "IntegracaoEFreteVP");

                ServicoEFrete.ValePedagio.ConsultarPedagiosParceiroV2Response consultarPedagiosResponseV2 = ConsultarPedagiosV2(integracaoEFrete, token, carga, cargaValePedagio);

                if (consultarPedagiosResponseV2 != null)
                    AdicionarValePedagioV2(consultarPedagiosResponseV2, integracaoEFrete, token, carga, cargaValePedagio, configuracaoIntegracaoEFrete);
            }
            else
            {
                Servicos.Log.TratarErro($"GerarCompraValePedagio - Usando V1", "IntegracaoEFreteVP");

                ServicoEFrete.ValePedagio.ConsultarPedagiosParceiroResponse consultarPedagiosResponse = ConsultarPedagios(integracaoEFrete, token, carga, cargaValePedagio);

                if (consultarPedagiosResponse != null)
                    AdicionarValePedagio(consultarPedagiosResponse, integracaoEFrete, token, carga, cargaValePedagio);
            }

            if (cargaValePedagio.SituacaoIntegracao == SituacaoIntegracao.ProblemaIntegracao)
            {
                Servicos.Log.TratarErro($"GerarCompraValePedagio - Relatando falha de compra CP para servico externo", "IntegracaoEFreteVP");
                IntegrarFalhaDeValePedagioPiracanjuba(cargaValePedagio, tipoServicoMultisoftware);
            }

            Servicos.Log.TratarErro($"GerarCompraValePedagio - Realizando logout", "IntegracaoEFreteVP");
            Logout(integracaoEFrete, token);

            servicoCargaValePedagio.EnviarEmailTransportador(cargaValePedagio, integracaoEFrete.NotificarTransportadorPorEmail, tipoServicoMultisoftware);
        }

        public void SolicitarCancelamentoValePedagio(Dominio.Entidades.Embarcador.Cargas.ValePedagio.CargaIntegracaoValePedagio cargaValePedagio, AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware tipoServicoMultisoftware)
        {
            Repositorio.Embarcador.Cargas.ValePedagio.CargaIntegracaoValePedagio repositorioCargaValePedagio = new Repositorio.Embarcador.Cargas.ValePedagio.CargaIntegracaoValePedagio(_unitOfWork);

            Frota.ValePedagio servicoValePedagio = new Frota.ValePedagio(_unitOfWork);
            ArquivoTransacao<Dominio.Entidades.Embarcador.Cargas.ValePedagio.CargaValePedagioIntegracaoArquivo> servicoArquivoTransacao = new ArquivoTransacao<Dominio.Entidades.Embarcador.Cargas.ValePedagio.CargaValePedagioIntegracaoArquivo>(_unitOfWork);

            Dominio.Entidades.Embarcador.Configuracoes.IntegracaoEFrete integracaoEFrete = servicoValePedagio.ObterIntegracaoEFrete(cargaValePedagio.Carga, tipoServicoMultisoftware);

            string token = Login(integracaoEFrete, cargaValePedagio);
            if (string.IsNullOrWhiteSpace(token))
                return;

            ServicoEFrete.ValePedagio.ValePedagioParceiroServiceSoapClient valePedagioParceiroServiceSoapClient = ObterClient(integracaoEFrete.URL);
            InspectorBehavior inspector = new InspectorBehavior();
            valePedagioParceiroServiceSoapClient.Endpoint.EndpointBehaviors.Add(inspector);

            cargaValePedagio.DataIntegracao = DateTime.Now;
            cargaValePedagio.NumeroTentativas++;

            try
            {
                ServicoEFrete.ValePedagio.CancelarValePedagioParceiroRequest cancelarValePedagio = new ServicoEFrete.ValePedagio.CancelarValePedagioParceiroRequest()
                {
                    Integrador = integracaoEFrete.CodigoIntegrador,
                    Versao = 1,
                    Token = token,
                    ClienteCpfCnpj = ObterClienteCpfCnpj(integracaoEFrete, cargaValePedagio.Carga.Empresa),
                    CodigoValePedagio = cargaValePedagio.IdCompraValePedagio,
                    Motivo = "CANCELAMENTO GERADO PELO OPERADOR"
                };

                ServicoEFrete.ValePedagio.CancelarValePedagioParceiroResponse retorno = valePedagioParceiroServiceSoapClient.CancelarValePedagio(cancelarValePedagio);

                if (retorno.Sucesso)
                {
                    cargaValePedagio.SituacaoIntegracao = SituacaoIntegracao.Integrado;
                    cargaValePedagio.SituacaoValePedagio = SituacaoValePedagio.Cancelada;
                    cargaValePedagio.ProblemaIntegracao = "Vale Pedágio Cancelado com Sucesso";
                }
                else
                {
                    cargaValePedagio.SituacaoValePedagio = SituacaoValePedagio.Comprada;
                    cargaValePedagio.ProblemaIntegracao = $"{retorno.Excecao.Codigo} - {retorno.Excecao.Mensagem}";
                }
            }
            catch (Exception ex)
            {
                Log.TratarErro(ex);

                cargaValePedagio.SituacaoValePedagio = SituacaoValePedagio.Comprada;
                cargaValePedagio.ProblemaIntegracao = "Ocorreu uma falha ao realizar o cancelamento de e-Frete";
            }

            servicoArquivoTransacao.Adicionar(cargaValePedagio, inspector.LastRequestXML, inspector.LastResponseXML, "xml");

            repositorioCargaValePedagio.Atualizar(cargaValePedagio);

            Logout(integracaoEFrete, token);
        }

        public byte[] ObterReciboPdf(Dominio.Entidades.Embarcador.Cargas.ValePedagio.CargaIntegracaoValePedagio cargaValePedagio, AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware tipoServicoMultisoftware, ref string mensagemRetorno)
        {
            Repositorio.Embarcador.Cargas.ValePedagio.CargaIntegracaoValePedagio repositorioCargaValePedagio = new Repositorio.Embarcador.Cargas.ValePedagio.CargaIntegracaoValePedagio(_unitOfWork);

            Frota.ValePedagio servicoValePedagio = new Frota.ValePedagio(_unitOfWork);
            ArquivoTransacao<Dominio.Entidades.Embarcador.Cargas.ValePedagio.CargaValePedagioIntegracaoArquivo> servicoArquivoTransacao = new ArquivoTransacao<Dominio.Entidades.Embarcador.Cargas.ValePedagio.CargaValePedagioIntegracaoArquivo>(_unitOfWork);

            Dominio.Entidades.Embarcador.Configuracoes.IntegracaoEFrete integracaoEFrete = servicoValePedagio.ObterIntegracaoEFrete(cargaValePedagio.Carga, tipoServicoMultisoftware);

            string token = Login(integracaoEFrete, cargaValePedagio);
            if (string.IsNullOrWhiteSpace(token))
                return null;

            ServicoEFrete.ValePedagio.ValePedagioParceiroServiceSoapClient valePedagioParceiroServiceSoapClient = ObterClient(integracaoEFrete.URL);
            InspectorBehavior inspector = new InspectorBehavior();
            valePedagioParceiroServiceSoapClient.Endpoint.EndpointBehaviors.Add(inspector);

            byte[] arquivoRetorno = null;

            try
            {
                ServicoEFrete.ValePedagio.ObterReciboPdfParceiroRequest obterReciboPdf = new ServicoEFrete.ValePedagio.ObterReciboPdfParceiroRequest()
                {
                    Integrador = integracaoEFrete.CodigoIntegrador,
                    Versao = 1,
                    Token = token,
                    ClienteCpfCnpj = ObterClienteCpfCnpj(integracaoEFrete, cargaValePedagio.Carga.Empresa),
                    CodigoValePedagio = cargaValePedagio.IdCompraValePedagio
                };

                ServicoEFrete.ValePedagio.ObterReciboPdfParceiroResponse retorno = valePedagioParceiroServiceSoapClient.ObterReciboPdf(obterReciboPdf);

                if (!retorno.Sucesso)
                {
                    mensagemRetorno = $"ObterReciboPdf: {retorno.Excecao.Codigo} - {retorno.Excecao.Mensagem}";

                    servicoArquivoTransacao.Adicionar(cargaValePedagio, inspector.LastRequestXML, inspector.LastResponseXML, "xml", mensagemRetorno);

                    repositorioCargaValePedagio.Atualizar(cargaValePedagio);
                }

                arquivoRetorno = retorno.Pdf;
            }
            catch (Exception ex)
            {
                Log.TratarErro(ex);
                mensagemRetorno = "Falha ObterReciboPdf e-Frete";
            }

            Logout(integracaoEFrete, token);

            return arquivoRetorno;
        }

        public void IntegrarCadastroVeiculo(Dominio.Entidades.Embarcador.Veiculos.VeiculoIntegracao integracao)
        {
            ArquivoTransacao<Dominio.Entidades.Embarcador.Cargas.CargaCTeIntegracaoArquivo> servicoArquivoTransacao = new ArquivoTransacao<Dominio.Entidades.Embarcador.Cargas.CargaCTeIntegracaoArquivo>(_unitOfWork);

            Repositorio.Embarcador.Veiculos.VeiculoIntegracao repVeiculoIntegracao = new Repositorio.Embarcador.Veiculos.VeiculoIntegracao(_unitOfWork);
            Repositorio.Veiculo repVeiculo = new Repositorio.Veiculo(_unitOfWork);

            string mensagem = string.Empty;
            string requestJson = string.Empty;
            string requestLastResponse = string.Empty;
            integracao.NumeroTentativas++;
            integracao.DataIntegracao = DateTime.Now;

            try
            {
                integracao.Veiculo.PossuiTagValePedagio = BuscarSituacaoTag(integracao.Veiculo, ref mensagem, ref requestJson, ref requestLastResponse);
                integracao.SituacaoIntegracao = SituacaoIntegracao.Integrado;
                integracao.ProblemaIntegracao = "Sucesso ao Consultag TAG na Efrete";
            }
            catch (ServicoException e)
            {
                Servicos.Log.TratarErro(e.Message, "IntegracaoEFreteVP");

                integracao.ProblemaIntegracao = e.Message;
                integracao.SituacaoIntegracao = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoIntegracao.ProblemaIntegracao;
                integracao.Veiculo.PossuiTagValePedagio = false;
            }
            catch (Exception e)
            {
                Servicos.Log.TratarErro(e, "IntegracaoEFreteVP");

                integracao.ProblemaIntegracao = "Erro ao consultar cadastro veiculo na SemParar";
                integracao.SituacaoIntegracao = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoIntegracao.ProblemaIntegracao;
                integracao.Veiculo.PossuiTagValePedagio = false;
            }

            repVeiculoIntegracao.Atualizar(integracao);
            repVeiculo.Atualizar(integracao.Veiculo);

            servicoArquivoTransacao.Adicionar(integracao, requestJson, requestLastResponse, "xml", mensagem);
        }

        public decimal ConsultarValorPedagio(Dominio.Entidades.Embarcador.Cargas.ValePedagio.CargaConsultaValorPedagioIntegracao cargaConsultaValePedagio, Dominio.Entidades.Embarcador.Cargas.Carga carga, AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware tipoServicoMultisoftware)
        {
            Repositorio.Embarcador.Cargas.ValePedagio.CargaConsultaValorPedagioIntegracao repCargaConsultaValorPedagioIntegracao = new Repositorio.Embarcador.Cargas.ValePedagio.CargaConsultaValorPedagioIntegracao(_unitOfWork);
            ArquivoTransacao<Dominio.Entidades.Embarcador.Cargas.ValePedagio.CargaValePedagioIntegracaoArquivo> servicoArquivoTransacao = new ArquivoTransacao<Dominio.Entidades.Embarcador.Cargas.ValePedagio.CargaValePedagioIntegracaoArquivo>(_unitOfWork);
            Frota.ValePedagio servicoValePedagio = new Frota.ValePedagio(_unitOfWork);

            cargaConsultaValePedagio.DataIntegracao = DateTime.Now;
            cargaConsultaValePedagio.NumeroTentativas++;
            Dominio.Entidades.Embarcador.Configuracoes.IntegracaoEFrete integracaoEFrete = servicoValePedagio.ObterIntegracaoEFrete(carga, tipoServicoMultisoftware);
            Dominio.Entidades.Embarcador.Configuracoes.IntegracaoGeralEFrete configuracaoIntegracaoEFrete = new Repositorio.Embarcador.Configuracoes.IntegracaoGeralEFrete(_unitOfWork).Buscar();

            Servicos.Models.Integracao.InspectorBehavior inspector = new Servicos.Models.Integracao.InspectorBehavior();

            ServicoEFrete.ValePedagio.ValePedagioParceiroServiceSoapClient valePedagioServiceSoapClient = ObterClient(integracaoEFrete.URL);
            valePedagioServiceSoapClient.Endpoint.EndpointBehaviors.Add(inspector);

            try
            {
                string mensagem = string.Empty;
                string requestJson = string.Empty;
                string requestLastResponse = string.Empty;

                string token = ObterToken(ref mensagem, ref requestJson, ref requestLastResponse, integracaoEFrete);

                if (string.IsNullOrWhiteSpace(token))
                    return -1;

                if (configuracaoIntegracaoEFrete?.VersaoEFrete == VersaoEFreteEnum.Versao2)
                {
                    ServicoEFrete.ValePedagio.ConsultarPedagiosParceiroV2Request consultarPedagiosParceiro = ObterConsultarPedagiosV2(integracaoEFrete, token, carga, null);
                    ServicoEFrete.ValePedagio.ConsultarPedagiosParceiroV2Response retorno = valePedagioServiceSoapClient.ConsultarPedagiosV2(consultarPedagiosParceiro);
                    cargaConsultaValePedagio.ValorValePedagio = retorno.Rotas?.Sum(x => x.Pracas?.Sum(y => y.Valores?.Sum(v => v.Valor))) ?? 0;
                }
                else
                {
                    ServicoEFrete.ValePedagio.ConsultarPedagiosParceiroRequest consultarPedagiosParceiro = ObterConsultarPedagios(integracaoEFrete, token, carga, null);
                    ServicoEFrete.ValePedagio.ConsultarPedagiosParceiroResponse retorno = valePedagioServiceSoapClient.ConsultarPedagios(consultarPedagiosParceiro);
                    cargaConsultaValePedagio.ValorValePedagio = retorno.Valor;
                }


                cargaConsultaValePedagio.DataIntegracao = DateTime.Now;
                cargaConsultaValePedagio.SituacaoIntegracao = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoIntegracao.Integrado;
                cargaConsultaValePedagio.ProblemaIntegracao = "Integrado com sucesso";

                if (cargaConsultaValePedagio.ValorValePedagio == 0)
                    cargaConsultaValePedagio.ProblemaIntegracao = "Rota sem pedágio";

                repCargaConsultaValorPedagioIntegracao.Atualizar(cargaConsultaValePedagio);

                servicoArquivoTransacao.Adicionar(cargaConsultaValePedagio, inspector.LastRequestXML, inspector.LastResponseXML, "xml", "ConsultarValorPedagio");


                return cargaConsultaValePedagio.ValorValePedagio;

            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex, "IntegracaoEFreteVP");
                cargaConsultaValePedagio.ProblemaIntegracao = "Falha ao consultar valor pedagio Efrete";
                cargaConsultaValePedagio.SituacaoIntegracao = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoIntegracao.ProblemaIntegracao;

                return -1;
            }
        }
        #endregion

        #region Métodos Privados

        private ServicoEFrete.ValePedagio.ConsultarPedagiosParceiroResponse ConsultarPedagios(Dominio.Entidades.Embarcador.Configuracoes.IntegracaoEFrete integracaoEFrete, string token, Dominio.Entidades.Embarcador.Cargas.Carga carga, Dominio.Entidades.Embarcador.Cargas.ValePedagio.CargaIntegracaoValePedagio cargaValePedagio)
        {
            Servicos.Log.TratarErro($"ConsultarPedagios - Consultando pedágios V1 da carga {carga.Codigo} - VP: {cargaValePedagio.Codigo}", "IntegracaoEFreteVP");

            Repositorio.Embarcador.Cargas.ValePedagio.CargaIntegracaoValePedagio repositorioCargaValePedagio = new Repositorio.Embarcador.Cargas.ValePedagio.CargaIntegracaoValePedagio(_unitOfWork);
            ArquivoTransacao<Dominio.Entidades.Embarcador.Cargas.ValePedagio.CargaValePedagioIntegracaoArquivo> servicoArquivoTransacao = new ArquivoTransacao<Dominio.Entidades.Embarcador.Cargas.ValePedagio.CargaValePedagioIntegracaoArquivo>(_unitOfWork);

            ServicoEFrete.ValePedagio.ValePedagioParceiroServiceSoapClient valePedagioServiceSoapClient = ObterClient(integracaoEFrete.URL);
            InspectorBehavior inspector = new InspectorBehavior();
            valePedagioServiceSoapClient.Endpoint.EndpointBehaviors.Add(inspector);

            cargaValePedagio.DataIntegracao = DateTime.Now;

            try
            {
                ServicoEFrete.ValePedagio.ConsultarPedagiosParceiroRequest consultarPedagiosParceiro = ObterConsultarPedagios(integracaoEFrete, token, carga, cargaValePedagio);
                ServicoEFrete.ValePedagio.ConsultarPedagiosParceiroResponse retorno = valePedagioServiceSoapClient.ConsultarPedagios(consultarPedagiosParceiro);

                if (!retorno.Sucesso)
                    throw new ServicoException($"{retorno.Excecao.Codigo} - {retorno.Excecao.Mensagem}");

                cargaValePedagio.SituacaoValePedagio = SituacaoValePedagio.RotaGerada;
                cargaValePedagio.RotaTemporaria = retorno.CodigoRota;
                cargaValePedagio.QuantidadeEixos = retorno.NumeroEixos;
                cargaValePedagio.TipoCompra = Dominio.Enumeradores.TipoCompraValePedagio.Tag;



                servicoArquivoTransacao.Adicionar(cargaValePedagio, inspector.LastRequestXML, inspector.LastResponseXML, "xml", "Consultar Pedágios");

                repositorioCargaValePedagio.Atualizar(cargaValePedagio);

                Servicos.Log.TratarErro($"ConsultarPedagios - Sucesso ao consultar", "IntegracaoEFreteVP");

                return retorno;
            }
            catch (ServicoException excecao)
            {
                cargaValePedagio.SituacaoIntegracao = SituacaoIntegracao.ProblemaIntegracao;
                cargaValePedagio.ProblemaIntegracao = "ConsultarPedagios: " + excecao.Message;

                Servicos.Log.TratarErro($"ConsultarPedagios - Servico exception: " + excecao.Message, "IntegracaoEFreteVP");
            }
            catch (Exception excecao)
            {
                Log.TratarErro(excecao);

                cargaValePedagio.SituacaoIntegracao = SituacaoIntegracao.ProblemaIntegracao;
                cargaValePedagio.ProblemaIntegracao = "Ocorreu uma falha ao consultar os pedágios da e-Frete";

                Servicos.Log.TratarErro($"ConsultarPedagios - Falha geral: " + excecao.Message, "IntegracaoEFreteVP");
            }

            cargaValePedagio.NumeroTentativas++;

            servicoArquivoTransacao.Adicionar(cargaValePedagio, inspector.LastRequestXML, inspector.LastResponseXML, "xml");

            repositorioCargaValePedagio.Atualizar(cargaValePedagio);

            return null;
        }

        private ServicoEFrete.ValePedagio.ConsultarPedagiosParceiroV2Response ConsultarPedagiosV2(Dominio.Entidades.Embarcador.Configuracoes.IntegracaoEFrete integracaoEFrete, string token, Dominio.Entidades.Embarcador.Cargas.Carga carga, Dominio.Entidades.Embarcador.Cargas.ValePedagio.CargaIntegracaoValePedagio cargaValePedagio)
        {
            Servicos.Log.TratarErro($"ConsultarPedagiosV2 - Consultando pedágios V2 da carga {carga.Codigo} - VP: {cargaValePedagio.Codigo}", "IntegracaoEFreteVP");

            Repositorio.Embarcador.Cargas.ValePedagio.CargaIntegracaoValePedagio repositorioCargaValePedagio = new Repositorio.Embarcador.Cargas.ValePedagio.CargaIntegracaoValePedagio(_unitOfWork);
            ArquivoTransacao<Dominio.Entidades.Embarcador.Cargas.ValePedagio.CargaValePedagioIntegracaoArquivo> servicoArquivoTransacao = new ArquivoTransacao<Dominio.Entidades.Embarcador.Cargas.ValePedagio.CargaValePedagioIntegracaoArquivo>(_unitOfWork);

            ServicoEFrete.ValePedagio.ValePedagioParceiroServiceSoapClient valePedagioServiceSoapClient = ObterClient(integracaoEFrete.URL);
            InspectorBehavior inspector = new InspectorBehavior();
            valePedagioServiceSoapClient.Endpoint.EndpointBehaviors.Add(inspector);

            cargaValePedagio.DataIntegracao = DateTime.Now;

            try
            {

                ServicoEFrete.ValePedagio.ConsultarPedagiosParceiroV2Request consultarPedagiosParceiro = ObterConsultarPedagiosV2(integracaoEFrete, token, carga, cargaValePedagio);
                ServicoEFrete.ValePedagio.ConsultarPedagiosParceiroV2Response retorno = valePedagioServiceSoapClient.ConsultarPedagiosV2(consultarPedagiosParceiro);

                if (!retorno.Sucesso)
                    throw new ServicoException($"{retorno.Excecao.Codigo} - {retorno.Excecao.Mensagem}");

                cargaValePedagio.SituacaoValePedagio = SituacaoValePedagio.RotaGerada;
                cargaValePedagio.QuantidadeEixos = retorno.NumeroEixos;

                servicoArquivoTransacao.Adicionar(cargaValePedagio, inspector.LastRequestXML, inspector.LastResponseXML, "xml", "Consultar Pedágios");

                repositorioCargaValePedagio.Atualizar(cargaValePedagio);

                Servicos.Log.TratarErro($"ConsultarPedagiosV2 - Sucesso na consulta", "IntegracaoEFreteVP");

                return retorno;
            }
            catch (ServicoException excecao)
            {
                cargaValePedagio.SituacaoIntegracao = SituacaoIntegracao.ProblemaIntegracao;
                cargaValePedagio.ProblemaIntegracao = "ConsultarPedagios: " + excecao.Message;

                Servicos.Log.TratarErro($"ConsultarPedagiosV2 - Serviço exception: {excecao.Message}", "IntegracaoEFreteVP");
            }
            catch (Exception excecao)
            {
                Log.TratarErro(excecao);

                cargaValePedagio.SituacaoIntegracao = SituacaoIntegracao.ProblemaIntegracao;
                cargaValePedagio.ProblemaIntegracao = "Ocorreu uma falha ao consultar os pedágios da e-Frete";

                Servicos.Log.TratarErro($"ConsultarPedagiosV2 - Falha geral: {excecao.Message}", "IntegracaoEFreteVP");
            }

            cargaValePedagio.NumeroTentativas++;

            servicoArquivoTransacao.Adicionar(cargaValePedagio, inspector.LastRequestXML, inspector.LastResponseXML, "xml");

            repositorioCargaValePedagio.Atualizar(cargaValePedagio);

            return null;
        }

        private ServicoEFrete.ValePedagio.ConsultarPedagiosParceiroRequest ObterConsultarPedagios(Dominio.Entidades.Embarcador.Configuracoes.IntegracaoEFrete integracaoEFrete, string token, Dominio.Entidades.Embarcador.Cargas.Carga carga, Dominio.Entidades.Embarcador.Cargas.ValePedagio.CargaIntegracaoValePedagio cargaValePedagio)
        {
            Servicos.Embarcador.Localidades.Localidade servicoLocalidade = new Servicos.Embarcador.Localidades.Localidade(_unitOfWork);

            Dominio.Entidades.Veiculo veiculo = carga.Veiculo;
            Dominio.Entidades.Localidade localidadeOrigem, localidadeDestino;
            List<Dominio.Entidades.Localidade> pontosPassagem;
            servicoLocalidade.ObterLocalidadesValePedagioCarga(cargaValePedagio?.Carga ?? carga, out localidadeOrigem, out localidadeDestino, out pontosPassagem, _unitOfWork);

            ServicoEFrete.ValePedagio.ConsultarPedagiosParceiroRequest request = new ServicoEFrete.ValePedagio.ConsultarPedagiosParceiroRequest()
            {
                Integrador = integracaoEFrete.CodigoIntegrador,
                Versao = 1,
                Token = token,
                ParceiroPedagio = ObterParceiroPedagio(veiculo),
                ClienteCpfCnpj = ObterClienteCpfCnpj(integracaoEFrete, carga.Empresa),
                Origem = new ServicoEFrete.ValePedagio.PontoRotaParceiro()
                {
                    CodigoMunicipio = localidadeOrigem.CodigoIBGE
                },
                Destino = new ServicoEFrete.ValePedagio.PontoRotaParceiro()
                {
                    CodigoMunicipio = localidadeDestino.CodigoIBGE
                },
                PontosInteresse = ObterPontosInteresse(pontosPassagem),
                CodigoRota = carga.Codigo.ToString(),
                TipoVeiculo = ServicoEFrete.ValePedagio.TipoVeiculo.Caminhao,
                NumeroEixos = ObterNumeroEixos(carga),
                Placa = veiculo.Placa,
                TipoRodagem = ServicoEFrete.ValePedagio.EnumTipoRodagem.Dupla
            };

            return request;
        }

        private ServicoEFrete.ValePedagio.ConsultarPedagiosParceiroV2Request ObterConsultarPedagiosV2(Dominio.Entidades.Embarcador.Configuracoes.IntegracaoEFrete integracaoEFrete, string token, Dominio.Entidades.Embarcador.Cargas.Carga carga, Dominio.Entidades.Embarcador.Cargas.ValePedagio.CargaIntegracaoValePedagio cargaValePedagio)
        {
            string[] pontosInteresse = null;
            ServicoEFrete.ValePedagio.Polilinha polilinha = null;

            if (integracaoEFrete.EnviarPontosPassagemRotaFrete)
                pontosInteresse = ObterPontosPassagemRotaFreteV2(carga, cargaValePedagio);
            else if (integracaoEFrete.EnviarPolilinhaRoteirizacaoCarga)
                polilinha = ObterPolilinhaV2(carga.Codigo);
            else
                pontosInteresse = ObterPontosPassagemPorIBGEV2(cargaValePedagio, carga);

            ServicoEFrete.ValePedagio.ConsultarPedagiosParceiroV2Request request = new ServicoEFrete.ValePedagio.ConsultarPedagiosParceiroV2Request()
            {
                Integrador = integracaoEFrete.CodigoIntegrador,
                Versao = 1,
                Token = token,
                ParceiroPedagio = ObterParceiroPedagio(carga.Veiculo),
                ClienteCpfCnpj = ObterClienteCpfCnpj(integracaoEFrete, carga.Empresa),
                TipoVeiculo = ObterTipoVeiculoDeParaValePedagioV2(integracaoEFrete, carga.Veiculo),
                NumeroEixos = ObterNumeroEixos(carga),
                TipoRodagem = ObterTipoRodagemV2(integracaoEFrete, carga.Veiculo),
                PontosInteresse = pontosInteresse,
                Polilinha = polilinha,
            };

            return request;
        }

        private ServicoEFrete.ValePedagio.TipoVeiculo ObterTipoVeiculoDeParaValePedagioV2(Dominio.Entidades.Embarcador.Configuracoes.IntegracaoEFrete integracaoEFrete, Dominio.Entidades.Veiculo veiculo)
        {
            if (integracaoEFrete.EnviarTipoVeiculoNaIntegracao)
            {
                switch (veiculo.TipoRodado)
                {
                    case "00": //Não Aplicado
                        return ServicoEFrete.ValePedagio.TipoVeiculo.Ausente;
                    case "01": //01 - TRUCK
                    case "02": //02 - TOCO
                    case "03": //03 - CAVALO
                        return ServicoEFrete.ValePedagio.TipoVeiculo.Caminhao;
                    case "04": //04 - VAN
                    case "05": //05 - UTILITARIO
                        return ServicoEFrete.ValePedagio.TipoVeiculo.Carro;
                    default:
                        return ServicoEFrete.ValePedagio.TipoVeiculo.Caminhao;
                }
            }

            return ServicoEFrete.ValePedagio.TipoVeiculo.Caminhao;
        }

        private ServicoEFrete.ValePedagio.EnumTipoRodagem ObterTipoRodagemV2(Dominio.Entidades.Embarcador.Configuracoes.IntegracaoEFrete integracaoEFrete, Dominio.Entidades.Veiculo veiculo)
        {

            if (integracaoEFrete.EnviarTipoVeiculoNaIntegracao)
            {
                switch (veiculo.TipoRodado)
                {
                    case "04": //04 - VAN
                    case "05": //05 - UTILITARIO
                        return ServicoEFrete.ValePedagio.EnumTipoRodagem.Simples;
                    default:
                        return ServicoEFrete.ValePedagio.EnumTipoRodagem.Dupla;
                }
            }

            return ServicoEFrete.ValePedagio.EnumTipoRodagem.Dupla;
        }

        private string[] ObterPontosPassagemRotaFreteV2(Dominio.Entidades.Embarcador.Cargas.Carga carga, Dominio.Entidades.Embarcador.Cargas.ValePedagio.CargaIntegracaoValePedagio cargaValePedagio)
        {
            Servicos.Embarcador.Localidades.Localidade servicoLocalidade = new Servicos.Embarcador.Localidades.Localidade(_unitOfWork);

            List<(decimal? Latitude, decimal? Longitude)> pontosPassagem;
            (decimal? Latitude, decimal? Longitude) latitudeLongitudeOrigem, latitudeLongitudeDestino;

            servicoLocalidade.ObterLatitudeLongitudeValePedagioCarga(cargaValePedagio?.Carga ?? carga, out latitudeLongitudeOrigem, out latitudeLongitudeDestino, out pontosPassagem, _unitOfWork);

            List<(decimal? Latitude, decimal? Longitude)> pontosPassagemLatitudeLongitude = BuscarPontosPassagemDaRotaFrete(carga.Rota);

            return ObterPontosInteresseLatitudeLongitudeV2(pontosPassagemLatitudeLongitude, latitudeLongitudeOrigem, latitudeLongitudeDestino);
        }

        private string[] ObterPontosPassagemPorIBGEV2(Dominio.Entidades.Embarcador.Cargas.ValePedagio.CargaIntegracaoValePedagio cargaValePedagio, Dominio.Entidades.Embarcador.Cargas.Carga carga = null)
        {
            Servicos.Embarcador.Localidades.Localidade servicoLocalidade = new Servicos.Embarcador.Localidades.Localidade(_unitOfWork);

            List<Dominio.Entidades.Localidade> pontosPassagem;
            Dominio.Entidades.Localidade localidadeOrigem, localidadeDestino;

            servicoLocalidade.ObterLocalidadesValePedagioCarga(cargaValePedagio?.Carga ?? carga, out localidadeOrigem, out localidadeDestino, out pontosPassagem, _unitOfWork);
            return ObterPontosInteresseCodigoIBGEV2(pontosPassagem, localidadeOrigem, localidadeDestino);
        }

        private ServicoEFrete.ValePedagio.Polilinha ObterPolilinhaV2(int codigoCarga)
        {
            Repositorio.Embarcador.Cargas.CargaRotaFrete ServicoCargaRotaFrete = new Repositorio.Embarcador.Cargas.CargaRotaFrete(_unitOfWork);
            Dominio.Entidades.Embarcador.Cargas.CargaRotaFrete cargaRotaFrete = ServicoCargaRotaFrete.BuscarPorCarga(codigoCarga);

            return new ServicoEFrete.ValePedagio.Polilinha()
            {
                Conteudo = cargaRotaFrete.PolilinhaRota,
                Precisao = 5,
            };
        }

        private int ObterNumeroEixos(Dominio.Entidades.Embarcador.Cargas.Carga carga)
        {
            bool eixosSuspensos = carga.TipoOperacao?.TipoCarregamento == RetornoCargaTipo.Vazio;

            int numeroEixos = 0;
            if (carga.Veiculo.ModeloVeicularCarga != null)
            {
                numeroEixos = carga.Veiculo.ModeloVeicularCarga.NumeroEixos ?? 0;
                if (eixosSuspensos)
                    numeroEixos -= carga.Veiculo.ModeloVeicularCarga.NumeroEixosSuspensos ?? 0;
            }

            if (carga.VeiculosVinculados != null)
            {
                foreach (Dominio.Entidades.Veiculo reboque in carga.VeiculosVinculados.ToList())
                {
                    if (reboque.ModeloVeicularCarga != null && carga.Veiculo.ModeloVeicularCarga != null && reboque.ModeloVeicularCarga != carga.Veiculo.ModeloVeicularCarga)
                    {
                        numeroEixos += reboque.ModeloVeicularCarga.NumeroEixos ?? 0;

                        if (eixosSuspensos)
                            numeroEixos -= reboque.ModeloVeicularCarga.NumeroEixosSuspensos ?? 0;
                    }
                }
            }

            return numeroEixos;
        }

        private string ObterClienteCpfCnpj(Dominio.Entidades.Embarcador.Configuracoes.IntegracaoEFrete integracaoEFrete, Dominio.Entidades.Empresa empresa)
        {
            return integracaoEFrete.Cliente?.CPF_CNPJ_SemFormato ?? empresa.CNPJ;
        }

        private ServicoEFrete.ValePedagio.EnumParceiroPedagio ObterParceiroPedagio(Dominio.Entidades.Veiculo veiculo)
        {
            if (veiculo.ModoCompraValePedagioTarget == ModoCompraValePedagioTarget.PedagioTagVeloe)
                return ServicoEFrete.ValePedagio.EnumParceiroPedagio.Veloe;

            if (veiculo.ModoCompraValePedagioTarget == ModoCompraValePedagioTarget.PedagioTagMoveMais)
                return ServicoEFrete.ValePedagio.EnumParceiroPedagio.MoveMais;

            if (veiculo.ModoCompraValePedagioTarget == ModoCompraValePedagioTarget.PedagioConectCar)
                return ServicoEFrete.ValePedagio.EnumParceiroPedagio.ConectCar;

            if (veiculo.ModoCompraValePedagioTarget == ModoCompraValePedagioTarget.PedagioTagRepom)
                return ServicoEFrete.ValePedagio.EnumParceiroPedagio.Repom;

            if (veiculo.ModoCompraValePedagioTarget == ModoCompraValePedagioTarget.PedagioTagTicketLog)
                return ServicoEFrete.ValePedagio.EnumParceiroPedagio.TicketLog;

            if (veiculo.ModoCompraValePedagioTarget == ModoCompraValePedagioTarget.PedagioTagEdenred)
                return ServicoEFrete.ValePedagio.EnumParceiroPedagio.Edenred;

            if (veiculo.ModoCompraValePedagioTarget == ModoCompraValePedagioTarget.PedagioTagAutomatico)
                return ServicoEFrete.ValePedagio.EnumParceiroPedagio.Automatico;

            if (veiculo.ModoCompraValePedagioTarget == ModoCompraValePedagioTarget.PedagioTagIndefinido)
                return ServicoEFrete.ValePedagio.EnumParceiroPedagio.Indefinido;

            return ServicoEFrete.ValePedagio.EnumParceiroPedagio.SemParar;
        }

        private ServicoEFrete.ValePedagio.PontoRotaParceiro[] ObterPontosInteresse(List<Dominio.Entidades.Localidade> pontosPassagem)
        {
            List<ServicoEFrete.ValePedagio.PontoRotaParceiro> pontosInteresse = new List<ServicoEFrete.ValePedagio.PontoRotaParceiro>();

            foreach (Dominio.Entidades.Localidade localidade in pontosPassagem)
            {
                ServicoEFrete.ValePedagio.PontoRotaParceiro pontoRota = new ServicoEFrete.ValePedagio.PontoRotaParceiro()
                {
                    CodigoMunicipio = localidade.CodigoIBGE
                };

                pontosInteresse.Add(pontoRota);
            }

            return pontosInteresse.ToArray();
        }

        private string[] ObterPontosInteresseCodigoIBGEV2(List<Dominio.Entidades.Localidade> pontosPassagem, Dominio.Entidades.Localidade localidadeOrigem, Dominio.Entidades.Localidade localidadeDestino)
        {
            List<string> pontosInteresse = new List<string> { localidadeOrigem.CodigoIBGE.ToString() };

            pontosInteresse.AddRange(pontosPassagem.Select(localidade => localidade.CodigoIBGE.ToString()));
            pontosInteresse.Add(localidadeDestino.CodigoIBGE.ToString());

            return pontosInteresse.ToArray();
        }

        private string FormatarCoordenadas((decimal? Latitude, decimal? Longitude) latitudeLongitude) => $"{latitudeLongitude.Latitude?.ToString("F6", CultureInfo.InvariantCulture)}, {latitudeLongitude.Longitude?.ToString("F6", CultureInfo.InvariantCulture)}";

        private string[] ObterPontosInteresseLatitudeLongitudeV2(List<(decimal? Latitude, decimal? Longitude)> pontosPassagemLatitudeLongitude, (decimal? Latitude, decimal? Longitude) latitudeLongitudeOrigem, (decimal? Latitude, decimal? Longitude) latitudeLongitudeDestino)
        {
            List<string> pontosInteresse = new List<string>
            {
                FormatarCoordenadas(latitudeLongitudeOrigem)
            };

            pontosInteresse.AddRange(pontosPassagemLatitudeLongitude.Select(o => FormatarCoordenadas(ValueTuple.Create(o.Latitude, o.Longitude))));
            pontosInteresse.Add(FormatarCoordenadas(latitudeLongitudeDestino));

            return pontosInteresse.ToArray();
        }

        private void AdicionarValePedagio(ServicoEFrete.ValePedagio.ConsultarPedagiosParceiroResponse consultarPedagiosResponse, Dominio.Entidades.Embarcador.Configuracoes.IntegracaoEFrete integracaoEFrete, string token, Dominio.Entidades.Embarcador.Cargas.Carga carga, Dominio.Entidades.Embarcador.Cargas.ValePedagio.CargaIntegracaoValePedagio cargaValePedagio)
        {
            Servicos.Log.TratarErro($"AdicionarValePedagio - Adicionando VP da carga {carga.Codigo} - VP: {cargaValePedagio.Codigo}", "IntegracaoEFreteVP");

            Repositorio.Embarcador.Cargas.ValePedagio.CargaIntegracaoValePedagio repositorioCargaValePedagio = new Repositorio.Embarcador.Cargas.ValePedagio.CargaIntegracaoValePedagio(_unitOfWork);

            ArquivoTransacao<Dominio.Entidades.Embarcador.Cargas.ValePedagio.CargaValePedagioIntegracaoArquivo> servicoArquivoTransacao = new ArquivoTransacao<Dominio.Entidades.Embarcador.Cargas.ValePedagio.CargaValePedagioIntegracaoArquivo>(_unitOfWork);

            ServicoEFrete.ValePedagio.ValePedagioParceiroServiceSoapClient valePedagioServiceSoapClient = ObterClient(integracaoEFrete.URL);
            InspectorBehavior inspector = new InspectorBehavior();
            valePedagioServiceSoapClient.Endpoint.EndpointBehaviors.Add(inspector);

            try
            {
                ServicoEFrete.ValePedagio.AdicionarValePedagioParceiroRequest adicionarValePedagio = ObterAdicionarValePedagio(consultarPedagiosResponse, integracaoEFrete, token, carga, cargaValePedagio);
                ServicoEFrete.ValePedagio.AdicionarValePedagioParceiroResponse retorno = valePedagioServiceSoapClient.AdicionarValePedagio(adicionarValePedagio);
                if (!retorno.Sucesso)
                    throw new ServicoException($"{retorno.Excecao.Codigo} - {retorno.Excecao.Mensagem}");

                cargaValePedagio.NumeroValePedagio = "";
                cargaValePedagio.IdCompraValePedagio = retorno.CodigoValePedagio;
                cargaValePedagio.NumeroValePedagio = retorno.CodigoValePedagio;
                cargaValePedagio.ValorValePedagio = retorno.Valor;
                cargaValePedagio.SituacaoValePedagio = SituacaoValePedagio.Comprada;
                cargaValePedagio.SituacaoIntegracao = SituacaoIntegracao.AgRetorno;
                cargaValePedagio.ProblemaIntegracao = "Vale Pedágio Comprado com Sucesso";

                Servicos.Log.TratarErro($"AdicionarValePedagio - Compra realizada com sucesso", "IntegracaoEFreteVP");
            }
            catch (ServicoException excecao)
            {
                cargaValePedagio.SituacaoIntegracao = SituacaoIntegracao.ProblemaIntegracao;
                cargaValePedagio.ProblemaIntegracao = excecao.Message;

                Servicos.Log.TratarErro($"AdicionarValePedagio - Servico exception: {excecao.Message}", "IntegracaoEFreteVP");
            }
            catch (Exception excecao)
            {
                Log.TratarErro(excecao);

                cargaValePedagio.SituacaoIntegracao = SituacaoIntegracao.ProblemaIntegracao;
                cargaValePedagio.ProblemaIntegracao = "Ocorreu uma falha ao realizar a compra do Vale Pedágio da e-Frete";

                Servicos.Log.TratarErro($"AdicionarValePedagio - Falha geral: {excecao.Message}", "IntegracaoEFreteVP");
            }

            cargaValePedagio.DataIntegracao = DateTime.Now;
            cargaValePedagio.NumeroTentativas++;

            servicoArquivoTransacao.Adicionar(cargaValePedagio, inspector.LastRequestXML, inspector.LastResponseXML, "xml");

            repositorioCargaValePedagio.Atualizar(cargaValePedagio);

            Servicos.Log.TratarErro($"AdicionarValePedagio - Atualizado", "IntegracaoEFreteVP");

        }

        private void AdicionarValePedagioV2(ServicoEFrete.ValePedagio.ConsultarPedagiosParceiroV2Response consultarPedagiosResponse, Dominio.Entidades.Embarcador.Configuracoes.IntegracaoEFrete integracaoEFrete, string token, Dominio.Entidades.Embarcador.Cargas.Carga carga, Dominio.Entidades.Embarcador.Cargas.ValePedagio.CargaIntegracaoValePedagio cargaValePedagio, Dominio.Entidades.Embarcador.Configuracoes.IntegracaoGeralEFrete configuracaoIntegracaoEFrete)
        {
            Servicos.Log.TratarErro($"AdicionarValePedagioV2 - Adicionando VP da carga {carga.Codigo} - VP: {cargaValePedagio.Codigo}", "IntegracaoEFreteVP");

            Repositorio.Embarcador.Cargas.ValePedagio.CargaIntegracaoValePedagio repositorioCargaValePedagio = new Repositorio.Embarcador.Cargas.ValePedagio.CargaIntegracaoValePedagio(_unitOfWork);

            ArquivoTransacao<Dominio.Entidades.Embarcador.Cargas.ValePedagio.CargaValePedagioIntegracaoArquivo> servicoArquivoTransacao = new ArquivoTransacao<Dominio.Entidades.Embarcador.Cargas.ValePedagio.CargaValePedagioIntegracaoArquivo>(_unitOfWork);

            ServicoEFrete.ValePedagio.ValePedagioParceiroServiceSoapClient valePedagioServiceSoapClient = ObterClient(integracaoEFrete.URL);
            InspectorBehavior inspector = new InspectorBehavior();
            valePedagioServiceSoapClient.Endpoint.EndpointBehaviors.Add(inspector);

            try
            {
                ServicoEFrete.ValePedagio.AdicionarValePedagioParceiroV2Request adicionarValePedagio = ObterAdicionarValePedagioV2(consultarPedagiosResponse, integracaoEFrete, token, carga, cargaValePedagio, configuracaoIntegracaoEFrete);
                ServicoEFrete.ValePedagio.AdicionarValePedagioParceiroV2Response retorno = valePedagioServiceSoapClient.AdicionarValePedagioV2(adicionarValePedagio);

                Servicos.Log.TratarErro($"AdicionarValePedagioV2 - Resposta result", "IntegracaoEFreteVP");
                Servicos.Log.TratarErro($"AdicionarValePedagioV2 - Valor: {retorno.Valor}, Status: {retorno.StatusValePedagio}, Sucesso: {retorno.Sucesso}", "IntegracaoEFreteVP");
                Servicos.Log.TratarErro($"AdicionarValePedagioV2 - Resposta gravada", "IntegracaoEFreteVP");
                if (!retorno.Sucesso || !string.IsNullOrEmpty(retorno.Excecao?.Mensagem))
                    if (retorno.Excecao?.Codigo?.Contains("Praca.Pedagio.Ausente") ?? false)
                        throw new ServicoException($"{retorno.Excecao?.Codigo ?? string.Empty} - {retorno.Excecao.Mensagem}", Dominio.ObjetosDeValor.Enumerador.CodigoExcecao.NaoTemPedagioNaRota);
                    else
                        throw new ServicoException($"{retorno.Excecao?.Codigo ?? string.Empty} - {retorno.Excecao?.Mensagem ?? "Ocorreu uma falha ao comprar vale pedágio."}");

                cargaValePedagio.NumeroValePedagio = "";
                cargaValePedagio.IdCompraValePedagio = retorno.CodigoValePedagio;
                cargaValePedagio.NumeroValePedagio = retorno.CodigoValePedagio;
                cargaValePedagio.CodigoEmissaoValePedagioANTT = retorno.IdVpoAntt;
                cargaValePedagio.ValorValePedagio = retorno.Valor;
                cargaValePedagio.SituacaoValePedagio = SituacaoValePedagio.Comprada;
                cargaValePedagio.SituacaoIntegracao = SituacaoIntegracao.AgRetorno;
                cargaValePedagio.ProblemaIntegracao = "Vale Pedágio Comprado com Sucesso";

                Servicos.Log.TratarErro($"AdicionarValePedagioV2 - Compra realizada com sucesso", "IntegracaoEFreteVP");

            }
            catch (ServicoException excecao) when (excecao.ErrorCode == Dominio.ObjetosDeValor.Enumerador.CodigoExcecao.NaoTemPedagioNaRota)
            {
                cargaValePedagio.NumeroValePedagio = "";
                cargaValePedagio.SituacaoValePedagio = SituacaoValePedagio.RotaSemCusto;
                cargaValePedagio.SituacaoIntegracao = SituacaoIntegracao.Integrado;
                cargaValePedagio.ProblemaIntegracao = "Rota sem custo";

                Servicos.Log.TratarErro($"AdicionarValePedagioV2 - Servico exception: {excecao.Message}", "IntegracaoEFreteVP");

            }
            catch (ServicoException excecao)
            {
                cargaValePedagio.SituacaoIntegracao = SituacaoIntegracao.ProblemaIntegracao;
                cargaValePedagio.ProblemaIntegracao = excecao.Message;

                Servicos.Log.TratarErro($"AdicionarValePedagioV2 - Servico exception: {excecao.Message}", "IntegracaoEFreteVP");

            }
            catch (Exception excecao)
            {
                Log.TratarErro(excecao);

                cargaValePedagio.SituacaoIntegracao = SituacaoIntegracao.ProblemaIntegracao;
                cargaValePedagio.ProblemaIntegracao = "Ocorreu uma falha ao realizar a compra do Vale Pedágio da e-Frete";

                Servicos.Log.TratarErro($"AdicionarValePedagioV2 - Falha geral: {excecao.Message}", "IntegracaoEFreteVP");
            }

            cargaValePedagio.DataIntegracao = DateTime.Now;
            cargaValePedagio.NumeroTentativas++;

            servicoArquivoTransacao.Adicionar(cargaValePedagio, inspector.LastRequestXML, inspector.LastResponseXML, "xml");

            repositorioCargaValePedagio.Atualizar(cargaValePedagio);

            Servicos.Log.TratarErro($"AdicionarValePedagioV2 - Atualizado valor", "IntegracaoEFreteVP");
        }

        private ServicoEFrete.ValePedagio.AdicionarValePedagioParceiroRequest ObterAdicionarValePedagio(ServicoEFrete.ValePedagio.ConsultarPedagiosParceiroResponse consultarPedagiosResponse, Dominio.Entidades.Embarcador.Configuracoes.IntegracaoEFrete integracaoEFrete, string token, Dominio.Entidades.Embarcador.Cargas.Carga carga, Dominio.Entidades.Embarcador.Cargas.ValePedagio.CargaIntegracaoValePedagio cargaValePedagio)
        {
            Dominio.Entidades.Veiculo veiculo = carga.Veiculo;

            ServicoEFrete.ValePedagio.AdicionarValePedagioParceiroRequest request = new ServicoEFrete.ValePedagio.AdicionarValePedagioParceiroRequest()
            {
                Integrador = integracaoEFrete.CodigoIntegrador,
                Versao = 1,
                Token = token,
                ParceiroPedagio = ObterParceiroPedagio(veiculo),
                ClienteCpfCnpj = ObterClienteCpfCnpj(integracaoEFrete, carga.Empresa),
                EmbarcadorCpfCnpj = carga.Filial?.CNPJ ?? carga.Empresa.CNPJ,
                CodigoMunicipioOrigem = consultarPedagiosResponse.Origem.CodigoMunicipio,
                CodigoMunicipioDestino = consultarPedagiosResponse.Destino.CodigoMunicipio,
                CodigoRota = cargaValePedagio.RotaTemporaria,
                TipoVeiculo = ServicoEFrete.ValePedagio.TipoVeiculo.Caminhao,
                NumeroEixos = cargaValePedagio.QuantidadeEixos,
                Placa = veiculo.Placa,
                CodigoContrato = carga.CodigoCargaEmbarcador,
                Valor = consultarPedagiosResponse.Valor,
                PracasPedagio = ObterPracasPedagio(consultarPedagiosResponse.PracasPedagio),
                DataValidade = DateTime.Now.AddDays(integracaoEFrete.DiasPrazo > 0 ? integracaoEFrete.DiasPrazo : 15),
                TipoRodagem = ServicoEFrete.ValePedagio.EnumTipoRodagem.Dupla
            };

            return request;
        }

        private ServicoEFrete.ValePedagio.AdicionarValePedagioParceiroV2Request ObterAdicionarValePedagioV2(ServicoEFrete.ValePedagio.ConsultarPedagiosParceiroV2Response consultarPedagiosResponse, Dominio.Entidades.Embarcador.Configuracoes.IntegracaoEFrete integracaoEFrete, string token, Dominio.Entidades.Embarcador.Cargas.Carga carga, Dominio.Entidades.Embarcador.Cargas.ValePedagio.CargaIntegracaoValePedagio cargaValePedagio, Dominio.Entidades.Embarcador.Configuracoes.IntegracaoGeralEFrete configuracaoIntegracaoEFrete)
        {
            Repositorio.Embarcador.Pessoas.ModalidadeTransportadoraPessoas repModalidadeTransportadoraPessoas = new Repositorio.Embarcador.Pessoas.ModalidadeTransportadoraPessoas(_unitOfWork);
            Dominio.Entidades.Veiculo veiculo = carga.Veiculo;
            string transportadoraCpfCnpj = carga.Empresa?.CNPJ_SemFormato ?? "";
            string transportadoraRntrc = carga.Empresa?.RegistroANTT ?? "";
            if (veiculo.Tipo == "T" && veiculo.Proprietario != null)
            {
                transportadoraCpfCnpj = veiculo.Proprietario.CPF_CNPJ_SemFormato;
                transportadoraRntrc = repModalidadeTransportadoraPessoas.BuscarRNTCPorPessoa(veiculo.Proprietario.CPF_CNPJ);
            }

            ServicoEFrete.ValePedagio.AdicionarValePedagioParceiroV2Request request = new ServicoEFrete.ValePedagio.AdicionarValePedagioParceiroV2Request()
            {
                Integrador = integracaoEFrete.CodigoIntegrador,
                Versao = (configuracaoIntegracaoEFrete?.EnviarDadosRegulatorioANTT ?? false) ? 2 : 1,
                Token = token,
                ParceiroPedagio = ObterParceiroPedagio(veiculo),
                ClienteCpfCnpj = ObterClienteCpfCnpj(integracaoEFrete, carga.Empresa),
                EmbarcadorCpfCnpj = carga.Filial?.CNPJ ?? carga.Empresa.CNPJ,
                TipoVeiculo = ObterTipoVeiculoDeParaValePedagioV2(integracaoEFrete, veiculo),
                NumeroEixos = cargaValePedagio.QuantidadeEixos,
                Placa = veiculo.Placa,
                CodigoContrato = carga.CodigoCargaEmbarcador,
                PracasPedagio = ObterPracasPedagioV2(consultarPedagiosResponse.Rotas),
                DataValidade = DateTime.Now.AddDays(integracaoEFrete.DiasPrazo > 0 ? integracaoEFrete.DiasPrazo : 15),
                TipoRodagem = ObterTipoRodagemV2(integracaoEFrete, veiculo),
                TransportadoraCpfCnpj = transportadoraCpfCnpj,
                TransportadoraRntrc = transportadoraRntrc ?? string.Empty,
            };

            return request;
        }

        private ServicoEFrete.ValePedagio.PracaPedagioCodigoTarifa[] ObterPracasPedagio(ServicoEFrete.ValePedagio.PracaPedagio1[] pracasPedagio)
        {
            List<ServicoEFrete.ValePedagio.PracaPedagioCodigoTarifa> pracasPedagioCodigoTarifa = new List<ServicoEFrete.ValePedagio.PracaPedagioCodigoTarifa>();

            foreach (ServicoEFrete.ValePedagio.PracaPedagio1 pracaPedagio in pracasPedagio)
            {
                foreach (ServicoEFrete.ValePedagio.PracaPedagioValor pracaPedagioValor in pracaPedagio.Valores)
                {
                    ServicoEFrete.ValePedagio.PracaPedagioCodigoTarifa pontoRota = new ServicoEFrete.ValePedagio.PracaPedagioCodigoTarifa()
                    {
                        Codigo = pracaPedagio.Codigo,
                        TipoTarifa = (ServicoEFrete.ValePedagio.TipoTarifa)pracaPedagioValor.TipoTarifa
                    };

                    pracasPedagioCodigoTarifa.Add(pontoRota);
                }
            }

            return pracasPedagioCodigoTarifa.ToArray();
        }

        private ServicoEFrete.ValePedagio.PracaPedagio[] ObterPracasPedagioV2(ServicoEFrete.ValePedagio.RotaPedagio[] rotasPedagio)
        {
            List<ServicoEFrete.ValePedagio.PracaPedagio> pracasPedagio = new List<ServicoEFrete.ValePedagio.PracaPedagio>();

            foreach (ServicoEFrete.ValePedagio.RotaPedagio rotaPedagio in rotasPedagio)
            {
                foreach (ServicoEFrete.ValePedagio.Praca praca in rotaPedagio.Pracas)
                {
                    ServicoEFrete.ValePedagio.PracaPedagio pontoRota = new ServicoEFrete.ValePedagio.PracaPedagio()
                    {
                        CodigoAntt = praca.CodigoAntt,
                        Sentido = praca.Sentido,
                        Valor = praca.Valores.FirstOrDefault().Valor,
                    };

                    pracasPedagio.Add(pontoRota);
                }
            }

            return pracasPedagio.ToArray();
        }

        private void IntegrarFalhaDeValePedagioPiracanjuba(Dominio.Entidades.Embarcador.Cargas.ValePedagio.CargaIntegracaoValePedagio cargaValePedagio, AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware tipoServicoMultisoftware)
        {
            Repositorio.Embarcador.Cargas.TipoIntegracao repTipoIntegracao = new Repositorio.Embarcador.Cargas.TipoIntegracao(_unitOfWork);
            Piracanjuba.IntegracaoPiracanjuba servicoIntegracaoPiracanjuba = new Piracanjuba.IntegracaoPiracanjuba(_unitOfWork, tipoServicoMultisoftware);

            if (repTipoIntegracao.BuscarPorTipo(Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoIntegracao.Piracanjuba) != null)
                servicoIntegracaoPiracanjuba.IntegrarFalhaDeValePedagio(cargaValePedagio);
        }

        private bool BuscarSituacaoTag(Dominio.Entidades.Veiculo veiculo, ref string mensagem, ref string requestJson, ref string requestLastResponse)
        {
            if (veiculo == null)
                return false;

            Frota.ValePedagio servicoValePedagio = new Frota.ValePedagio(_unitOfWork);

            Dominio.Entidades.Embarcador.Configuracoes.IntegracaoEFrete integracaoEFrete = servicoValePedagio.ObterIntegracaoEFrete();

            string token = ObterToken(ref mensagem, ref requestJson, ref requestLastResponse, integracaoEFrete);
            string mensagemErro = string.Empty;

            if (string.IsNullOrWhiteSpace(token))
                throw new ServicoException($"Erro ao gerar token Efrete");

            ServicoEFrete.ValePedagio.ValePedagioParceiroServiceSoapClient valePedagioServiceSoapClient = ObterClient(integracaoEFrete.URL);
            InspectorBehavior inspector = new InspectorBehavior();
            valePedagioServiceSoapClient.Endpoint.EndpointBehaviors.Add(inspector);

            ServicoEFrete.ValePedagio.ConsultarExistenciaTagParaEmissaoRequest tagParaEmissaoRequest = new ServicoEFrete.ValePedagio.ConsultarExistenciaTagParaEmissaoRequest()
            {
                Placa = veiculo.Placa,
                Versao = 1,
                Integrador = integracaoEFrete.CodigoIntegrador,
                Token = token,
                ClienteCpfCnpj = integracaoEFrete.Cliente?.CPF_CNPJ_SemFormato ?? string.Empty,
            };

            ServicoEFrete.ValePedagio.ConsultarExistenciaTagParaEmissaoResponse retorno = valePedagioServiceSoapClient.ConsultarExistenciaTagParaEmissao(tagParaEmissaoRequest);

            if (retorno == null)
                throw new ServicoException($"Erro ao verificar a situação da tag do vale pedágio.");

            requestJson = inspector.LastRequestXML;
            requestLastResponse = inspector.LastResponseXML;

            if (!retorno.Sucesso)
            {
                Servicos.Log.TratarErro($"{retorno.Excecao.Codigo} - {retorno.Excecao.Mensagem}", "IntegracaoEFreteVP");
                throw new ServicoException(retorno.Excecao.Mensagem);
            }

            foreach (ServicoEFrete.ValePedagio.ParceiroComTagParaEmissao parceiro in retorno.ParceirosComTagParaEmissao)
            {
                if (parceiro.TagParaEmissao)
                    return true;

                if (!string.IsNullOrWhiteSpace(parceiro.Mensagem))
                    mensagemErro = parceiro.Mensagem;
            }

            if (!string.IsNullOrWhiteSpace(mensagemErro))
                throw new ServicoException(mensagemErro);

            return false;
        }

        private List<(decimal? Latitude, decimal? Longitude)> BuscarPontosPassagemDaRotaFrete(Dominio.Entidades.RotaFrete rotaFrete)
        {
            List<(decimal? Latitude, decimal? Longitude)> pontosPassagemLatitudeLongitude = new List<(decimal?, decimal?)>();

            if (rotaFrete == null)
                return pontosPassagemLatitudeLongitude;

            Repositorio.Embarcador.Logistica.PontoPassagemPreDefinido repositorioPontoPassagemPreDefinido = new Repositorio.Embarcador.Logistica.PontoPassagemPreDefinido(_unitOfWork);

            List<Dominio.Entidades.Embarcador.Logistica.PontoPassagemPreDefinido> pontosPassagemPreDefinido = repositorioPontoPassagemPreDefinido.BuscarPorRocartaFrete(rotaFrete.Codigo);

            if (!pontosPassagemPreDefinido.Any())
                return pontosPassagemLatitudeLongitude;

            foreach (Dominio.Entidades.Embarcador.Logistica.PontoPassagemPreDefinido pontoPassagemPreDefinido in pontosPassagemPreDefinido)
            {
                if (pontoPassagemPreDefinido.Cliente != null)
                    pontosPassagemLatitudeLongitude.Add(ValueTuple.Create(pontoPassagemPreDefinido.Cliente.Latitude.ToNullableDecimal(), pontoPassagemPreDefinido.Cliente.Longitude.ToNullableDecimal()));

                if (pontoPassagemPreDefinido.Localidade != null)
                    pontosPassagemLatitudeLongitude.Add(ValueTuple.Create(pontoPassagemPreDefinido.Localidade.Latitude, pontoPassagemPreDefinido.Localidade.Latitude));
            }

            return pontosPassagemLatitudeLongitude;
        }

        #endregion

        #region Métodos Privados - Configurações

        private string Login(Dominio.Entidades.Embarcador.Configuracoes.IntegracaoEFrete integracaoEFrete, Dominio.Entidades.Embarcador.Cargas.ValePedagio.CargaIntegracaoValePedagio cargaValePedagio)
        {
            Repositorio.Embarcador.Cargas.ValePedagio.CargaIntegracaoValePedagio repositorioCargaValePedagio = new Repositorio.Embarcador.Cargas.ValePedagio.CargaIntegracaoValePedagio(_unitOfWork);

            if (integracaoEFrete == null)
            {
                cargaValePedagio.ProblemaIntegracao = "Não possui configuração para e-Frete.";
                cargaValePedagio.SituacaoIntegracao = SituacaoIntegracao.ProblemaIntegracao;
                cargaValePedagio.DataIntegracao = DateTime.Now;
                cargaValePedagio.NumeroTentativas++;
                repositorioCargaValePedagio.Atualizar(cargaValePedagio);
                return null;
            }

            ArquivoTransacao<Dominio.Entidades.Embarcador.Cargas.ValePedagio.CargaValePedagioIntegracaoArquivo> servicoArquivoTransacao = new ArquivoTransacao<Dominio.Entidades.Embarcador.Cargas.ValePedagio.CargaValePedagioIntegracaoArquivo>(_unitOfWork);

            string mensagem = string.Empty;
            string requestJson = string.Empty;
            string requestLastResponse = string.Empty;

            string token = ObterToken(ref mensagem, ref requestJson, ref requestLastResponse, integracaoEFrete);

            if (!string.IsNullOrWhiteSpace(token))
                return token;

            cargaValePedagio.ProblemaIntegracao = mensagem;
            cargaValePedagio.SituacaoIntegracao = SituacaoIntegracao.ProblemaIntegracao;
            cargaValePedagio.DataIntegracao = DateTime.Now;
            cargaValePedagio.NumeroTentativas++;

            servicoArquivoTransacao.Adicionar(cargaValePedagio, requestJson, requestLastResponse, "xml");

            repositorioCargaValePedagio.Atualizar(cargaValePedagio);

            return null;
        }

        private string ObterToken(ref string mensagem, ref string requestJson, ref string requestLastResponse, Dominio.Entidades.Embarcador.Configuracoes.IntegracaoEFrete integracaoEFrete)
        {

            ServicoEFrete.Logon.LogonServiceSoapClient logonServiceSoapClient = ObterClientLogon(integracaoEFrete.URL);
            InspectorBehavior inspector = new InspectorBehavior();
            logonServiceSoapClient.Endpoint.EndpointBehaviors.Add(inspector);

            try
            {
                ServicoEFrete.Logon.LoginRequest request = new ServicoEFrete.Logon.LoginRequest
                {
                    Integrador = integracaoEFrete.CodigoIntegrador,
                    Usuario = integracaoEFrete.Usuario,
                    Senha = integracaoEFrete.Senha,
                    Versao = 1
                };

                ServicoEFrete.Logon.LoginResponse retorno = logonServiceSoapClient.Login(request);
                if (retorno.Sucesso)
                {
                    if (!string.IsNullOrWhiteSpace(retorno.Token))
                        return retorno.Token;

                    mensagem = $"Login retornou sucesso, mas token está vazio!";
                }
                else
                    mensagem = $"Login: {retorno.Excecao.Mensagem}";
            }
            catch (Exception ex)
            {
                Log.TratarErro(ex);
                mensagem = "Ocorreu uma falhar no login na integração com a e-Frete.";
            }

            requestJson = inspector.LastRequestXML;
            requestLastResponse = inspector.LastResponseXML;

            return string.Empty;
        }

        private void Logout(Dominio.Entidades.Embarcador.Configuracoes.IntegracaoEFrete integracaoEFrete, string token)
        {
            ServicoEFrete.Logon.LogonServiceSoapClient logonServiceSoapClient = ObterClientLogon(integracaoEFrete.URL);

            ServicoEFrete.Logon.LogoutRequest request = new ServicoEFrete.Logon.LogoutRequest()
            {
                Integrador = integracaoEFrete.CodigoIntegrador,
                Token = token,
                Versao = 1
            };

            logonServiceSoapClient.Logout(request);
        }

        private ServicoEFrete.Logon.LogonServiceSoapClient ObterClientLogon(string url)
        {
            System.Net.ServicePointManager.SecurityProtocol = System.Net.SecurityProtocolType.Tls12;

            System.ServiceModel.EndpointAddress endpointAddress = new System.ServiceModel.EndpointAddress($"{url}/LogonService.asmx");
            System.ServiceModel.BasicHttpBinding binding = new System.ServiceModel.BasicHttpBinding();

            binding.MaxReceivedMessageSize = int.MaxValue;
            binding.ReceiveTimeout = new TimeSpan(0, 5, 0);
            binding.SendTimeout = new TimeSpan(0, 5, 0);

            if (url.StartsWith("https"))
                binding.Security.Mode = System.ServiceModel.BasicHttpSecurityMode.Transport;

            return new ServicoEFrete.Logon.LogonServiceSoapClient(binding, endpointAddress);
        }

        private ServicoEFrete.ValePedagio.ValePedagioParceiroServiceSoapClient ObterClient(string url)
        {
            System.Net.ServicePointManager.SecurityProtocol = System.Net.SecurityProtocolType.Tls12;

            System.ServiceModel.EndpointAddress endpointAddress = new System.ServiceModel.EndpointAddress($"{url}/ValePedagioParceiroService.asmx");
            System.ServiceModel.BasicHttpBinding binding = new System.ServiceModel.BasicHttpBinding();

            binding.MaxReceivedMessageSize = int.MaxValue;
            binding.ReceiveTimeout = new TimeSpan(0, 7, 0);
            binding.SendTimeout = new TimeSpan(0, 7, 0);

            if (url.StartsWith("https"))
                binding.Security.Mode = System.ServiceModel.BasicHttpSecurityMode.Transport;

            return new ServicoEFrete.ValePedagio.ValePedagioParceiroServiceSoapClient(binding, endpointAddress);
        }

        #endregion
    }
}
