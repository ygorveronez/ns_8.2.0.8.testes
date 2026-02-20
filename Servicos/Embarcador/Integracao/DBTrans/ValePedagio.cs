using Dominio.Excecoes.Embarcador;
using Dominio.ObjetosDeValor.Embarcador.Enumeradores;
using Infrastructure.Services.HttpClientFactory;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;

namespace Servicos.Embarcador.Integracao.DBTrans
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
            Repositorio.Embarcador.Cargas.ValePedagio.CargaIntegracaoValePedagio repCargaValePedagio = new Repositorio.Embarcador.Cargas.ValePedagio.CargaIntegracaoValePedagio(_unitOfWork);

            Frota.ValePedagio servicoValePedagio = new Frota.ValePedagio(_unitOfWork);
            ArquivoTransacao<Dominio.Entidades.Embarcador.Cargas.ValePedagio.CargaValePedagioIntegracaoArquivo> servicoArquivoTransacao = new ArquivoTransacao<Dominio.Entidades.Embarcador.Cargas.ValePedagio.CargaValePedagioIntegracaoArquivo>(_unitOfWork);

            Dominio.Entidades.Embarcador.Configuracoes.IntegracaoDBTrans integracaoDBTrans = servicoValePedagio.ObterIntegracaoDBTrans(carga, tipoServicoMultisoftware);

            if (integracaoDBTrans == null)
            {
                cargaValePedagio.ProblemaIntegracao = "Não possui configuração para DBTrans/Rodocred.";
                cargaValePedagio.SituacaoIntegracao = SituacaoIntegracao.ProblemaIntegracao;
                cargaValePedagio.DataIntegracao = DateTime.Now;
                cargaValePedagio.NumeroTentativas++;
                repCargaValePedagio.Atualizar(cargaValePedagio);

                return;
            }

            ServicoDBTrans.ValePedagio.IdentificacaoIntegracaoType identificacaoIntegracao = ObterAutenticacao(integracaoDBTrans, cargaValePedagio);
            if (identificacaoIntegracao == null)
                return;

            //Rota temporária
            int codigoRotaTemporaria = ObterCodigoRotaTemporaria(identificacaoIntegracao, integracaoDBTrans, cargaValePedagio, out bool problemaRotaTemporaria);
            if (codigoRotaTemporaria == 0 && problemaRotaTemporaria)
                return;

            //Meio de pagamento
            int meioPagamento = ObterMeioPagamento(identificacaoIntegracao, integracaoDBTrans, cargaValePedagio, carga, out bool problemaMeioPagamento);
            if (meioPagamento == 0 && problemaMeioPagamento)
                return;

            if (!CadastrarTransportador(identificacaoIntegracao, integracaoDBTrans, cargaValePedagio) ||
                !CadastrarMotorista(identificacaoIntegracao, integracaoDBTrans, cargaValePedagio) ||
                !CadastrarVeiculo(identificacaoIntegracao, integracaoDBTrans, cargaValePedagio) ||
                !CadastrarDocumentoTransportador(identificacaoIntegracao, integracaoDBTrans, cargaValePedagio))
                return;

            System.Net.ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;
            ServicoDBTrans.ValePedagio.ValePedagioSoapClient valePedagioSoapClient = ObterClient(integracaoDBTrans.URL);
            InspectorBehavior inspector = new InspectorBehavior();
            valePedagioSoapClient.Endpoint.EndpointBehaviors.Add(inspector);

            bool obterDetalheRota = false;
            int codigoRotaDaCompra = 0;

            try
            {
                ServicoDBTrans.ValePedagio.ComprarPedagioRequest comprarPedagio = ObterCompraPedagio(identificacaoIntegracao, carga, cargaValePedagio, integracaoDBTrans, codigoRotaTemporaria, meioPagamento);
                ServicoDBTrans.ValePedagio.ComprarPedagioResponse retorno = valePedagioSoapClient.ComprarPedagio(comprarPedagio);

                codigoRotaDaCompra = comprarPedagio.RotaViagem.CodigoRota[0];

                if (meioPagamento == 3)
                    cargaValePedagio.TipoCompra = Dominio.Enumeradores.TipoCompraValePedagio.Cartao;
                else if (meioPagamento == 1)
                    cargaValePedagio.TipoCompra = Dominio.Enumeradores.TipoCompraValePedagio.Cupom;
                else
                    cargaValePedagio.TipoCompra = Dominio.Enumeradores.TipoCompraValePedagio.Tag;

                if (retorno.RetornoMensagem.StatusRetorno == ServicoDBTrans.ValePedagio.ComprarPedagioResponseRetornoMensagemStatusRetorno.SUCESSO)
                {
                    cargaValePedagio.NumeroValePedagio = "";
                    cargaValePedagio.IdCompraValePedagio = retorno.NumeroViagem;
                    cargaValePedagio.NumeroValePedagio = retorno.NumeroViagem;
                    cargaValePedagio.ValorValePedagio = retorno.ValorPedagio;
                    cargaValePedagio.SituacaoValePedagio = SituacaoValePedagio.Comprada;
                    cargaValePedagio.SituacaoIntegracao = SituacaoIntegracao.AgRetorno;
                    cargaValePedagio.ProblemaIntegracao = "Vale Pedágio Comprado com Sucesso";
                    cargaValePedagio.Observacao6 = retorno.LinkCertificado;

                    SalvarDadosRetornoCompraValePedagio(retorno, carga);
                }
                else
                {
                    cargaValePedagio.SituacaoIntegracao = SituacaoIntegracao.ProblemaIntegracao;
                    cargaValePedagio.ProblemaIntegracao = retorno.RetornoMensagem.Excecao.CodigoExcecao + " - " + retorno.RetornoMensagem.Excecao.MensagemExcecao;
                    obterDetalheRota = retorno.RetornoMensagem.Excecao.CodigoExcecao == "VIAEXC080";
                }
            }
            catch (Exception excecao)
            {
                Log.TratarErro(excecao);

                cargaValePedagio.SituacaoIntegracao = SituacaoIntegracao.ProblemaIntegracao;
                cargaValePedagio.ProblemaIntegracao = "Ocorreu uma falha ao realizar a compra do Vale Pedágio da DBTrans/Rodocred";
            }

            cargaValePedagio.DataIntegracao = DateTime.Now;
            cargaValePedagio.NumeroTentativas++;
            repCargaValePedagio.Atualizar(cargaValePedagio);

            servicoArquivoTransacao.Adicionar(cargaValePedagio, inspector.LastRequestXML, inspector.LastResponseXML, "xml");

            if (obterDetalheRota)
                DetalharRota(codigoRotaDaCompra, identificacaoIntegracao, integracaoDBTrans, cargaValePedagio);
        }

        public void SolicitarCancelamentoValePedagio(Dominio.Entidades.Embarcador.Cargas.ValePedagio.CargaIntegracaoValePedagio cargaValePedagio, AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware tipoServicoMultisoftware)
        {
            Repositorio.Embarcador.Cargas.ValePedagio.CargaIntegracaoValePedagio repCargaValePedagio = new Repositorio.Embarcador.Cargas.ValePedagio.CargaIntegracaoValePedagio(_unitOfWork);
            ArquivoTransacao<Dominio.Entidades.Embarcador.Cargas.ValePedagio.CargaValePedagioIntegracaoArquivo> servicoArquivoTransacao = new ArquivoTransacao<Dominio.Entidades.Embarcador.Cargas.ValePedagio.CargaValePedagioIntegracaoArquivo>(_unitOfWork);

            Frota.ValePedagio servicoValePedagio = new Frota.ValePedagio(_unitOfWork);
            Dominio.Entidades.Embarcador.Configuracoes.IntegracaoDBTrans integracaoDBTrans = servicoValePedagio.ObterIntegracaoDBTrans(cargaValePedagio.Carga, tipoServicoMultisoftware);

            if (integracaoDBTrans == null)
            {
                cargaValePedagio.ProblemaIntegracao = "Não possui configuração para DBTrans/Rodocred.";
                cargaValePedagio.SituacaoIntegracao = SituacaoIntegracao.ProblemaIntegracao;
                cargaValePedagio.DataIntegracao = DateTime.Now;
                cargaValePedagio.NumeroTentativas++;
                repCargaValePedagio.Atualizar(cargaValePedagio);

                return;
            }

            ServicoDBTrans.ValePedagio.IdentificacaoIntegracaoType identificacaoIntegracao = ObterAutenticacao(integracaoDBTrans, cargaValePedagio);
            if (identificacaoIntegracao == null)
                return;

            System.Net.ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;
            ServicoDBTrans.ValePedagio.ValePedagioSoapClient valePedagioSoapClient = ObterClient(integracaoDBTrans.URL);
            InspectorBehavior inspector = new InspectorBehavior();
            valePedagioSoapClient.Endpoint.EndpointBehaviors.Add(inspector);

            try
            {
                ServicoDBTrans.ValePedagio.CancelarViagemRequest request = new ServicoDBTrans.ValePedagio.CancelarViagemRequest()
                {
                    IdentificacaoIntegracao = identificacaoIntegracao,
                    NumeroViagem = cargaValePedagio.NumeroValePedagio
                };

                ServicoDBTrans.ValePedagio.CancelarViagemResponse retorno = valePedagioSoapClient.CancelarViagem(request);

                if (retorno.RetornoMensagem.StatusRetorno == ServicoDBTrans.ValePedagio.CancelarViagemResponseRetornoMensagemStatusRetorno.SUCESSO)
                {
                    cargaValePedagio.SituacaoIntegracao = SituacaoIntegracao.Integrado;
                    cargaValePedagio.SituacaoValePedagio = SituacaoValePedagio.Cancelada;
                    cargaValePedagio.ProblemaIntegracao = "Vale Pedágio Cancelado com Sucesso";
                }
                else
                {
                    cargaValePedagio.SituacaoValePedagio = SituacaoValePedagio.Comprada;
                    cargaValePedagio.ProblemaIntegracao = retorno.RetornoMensagem.Excecao.CodigoExcecao + " - " + retorno.RetornoMensagem.Excecao.MensagemExcecao;
                }
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);

                cargaValePedagio.SituacaoValePedagio = SituacaoValePedagio.Comprada;
                cargaValePedagio.ProblemaIntegracao = "Ocorreu uma falha ao realizar o cancelamento de Vale Pedágio da DBTrans/Rodocred";
            }

            servicoArquivoTransacao.Adicionar(cargaValePedagio, inspector.LastRequestXML, inspector.LastResponseXML, "xml");

            repCargaValePedagio.Atualizar(cargaValePedagio);
        }

        public decimal ConsultaValorPedagio(Dominio.Entidades.Embarcador.Cargas.ValePedagio.CargaConsultaValorPedagioIntegracao cargaConsultaValePedagio, Dominio.Entidades.Embarcador.Cargas.Carga carga, AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware tipoServicoMultisoftware)
        {
            Repositorio.Embarcador.Cargas.ValePedagio.CargaConsultaValorPedagioIntegracao repCargaConsultaValorPedagio = new Repositorio.Embarcador.Cargas.ValePedagio.CargaConsultaValorPedagioIntegracao(_unitOfWork);
            ArquivoTransacao<Dominio.Entidades.Embarcador.Cargas.ValePedagio.CargaValePedagioIntegracaoArquivo> servicoArquivoTransacao = new ArquivoTransacao<Dominio.Entidades.Embarcador.Cargas.ValePedagio.CargaValePedagioIntegracaoArquivo>(_unitOfWork);

            Frota.ValePedagio servicoValePedagio = new Frota.ValePedagio(_unitOfWork);
            Dominio.Entidades.Embarcador.Configuracoes.IntegracaoDBTrans integracaoDBTrans = servicoValePedagio.ObterIntegracaoDBTrans(carga, tipoServicoMultisoftware);

            if (integracaoDBTrans == null)
            {
                cargaConsultaValePedagio.ProblemaIntegracao = "Não possui configuração para DBTrans/Rodocred.";
                cargaConsultaValePedagio.SituacaoIntegracao = SituacaoIntegracao.ProblemaIntegracao;
                cargaConsultaValePedagio.DataIntegracao = DateTime.Now;
                cargaConsultaValePedagio.NumeroTentativas++;
                repCargaConsultaValorPedagio.Atualizar(cargaConsultaValePedagio);

                return 0;
            }

            ServicoDBTrans.ValePedagio.IdentificacaoIntegracaoType identificacaoIntegracao = ObterAutenticacao(integracaoDBTrans, cargaConsultaValePedagio);
            if (identificacaoIntegracao == null)
                return 0;

            int codigoRotaTemporaria = 0;
            if (integracaoDBTrans.TipoRota == TipoRotaDBTrans.RotaTemporaria)
            {
                codigoRotaTemporaria = ObterRotaTemporaria(identificacaoIntegracao, integracaoDBTrans, cargaConsultaValePedagio);
                if (codigoRotaTemporaria == 0)
                    return 0;
            }

            System.Net.ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;
            ServicoDBTrans.ValePedagio.ValePedagioSoapClient valePedagioSoapClient = ObterClient(integracaoDBTrans.URL);
            InspectorBehavior inspector = new InspectorBehavior();
            valePedagioSoapClient.Endpoint.EndpointBehaviors.Add(inspector);

            decimal valorPedagio = 0;
            try
            {
                ServicoDBTrans.ValePedagio.ConsultarTarifasRequest consultarTarifas = ObterConsultaTarifas(identificacaoIntegracao, cargaConsultaValePedagio, codigoRotaTemporaria);
                ServicoDBTrans.ValePedagio.ConsultarTarifasResponse retorno = valePedagioSoapClient.ConsultarTarifas(consultarTarifas);

                if (retorno.RetornoMensagem.StatusRetorno == ServicoDBTrans.ValePedagio.ConsultarTarifasResponseRetornoMensagemStatusRetorno.SUCESSO)
                {
                    valorPedagio = retorno.ListaTarifas.Sum(o => o.ValorTarifaPedagio);
                    cargaConsultaValePedagio.SituacaoIntegracao = SituacaoIntegracao.Integrado;
                    cargaConsultaValePedagio.ProblemaIntegracao = "Consulta de Vale Pedágio efetuada com Sucesso";
                }
                else if (retorno.RetornoMensagem.Excecao.CodigoExcecao == "CTREXC002")
                {
                    cargaConsultaValePedagio.SituacaoIntegracao = SituacaoIntegracao.Integrado;
                    cargaConsultaValePedagio.ProblemaIntegracao = "Consulta de Vale Pedágio efetuada com Sucesso - " + retorno.RetornoMensagem.Excecao.MensagemExcecao;
                }
                else
                {
                    cargaConsultaValePedagio.SituacaoIntegracao = SituacaoIntegracao.ProblemaIntegracao;
                    cargaConsultaValePedagio.ProblemaIntegracao = retorno.RetornoMensagem.Excecao.CodigoExcecao + " - " + retorno.RetornoMensagem.Excecao.MensagemExcecao;
                }
            }
            catch (ServicoException excecao)
            {
                cargaConsultaValePedagio.SituacaoIntegracao = SituacaoIntegracao.ProblemaIntegracao;
                cargaConsultaValePedagio.ProblemaIntegracao = excecao.Message;
            }
            catch (Exception excecao)
            {
                Log.TratarErro(excecao);

                cargaConsultaValePedagio.SituacaoIntegracao = SituacaoIntegracao.ProblemaIntegracao;
                cargaConsultaValePedagio.ProblemaIntegracao = "Ocorreu uma falha ao realizar a consulta de tarifas do Vale Pedágio da DBTrans/Rodocred";
            }

            cargaConsultaValePedagio.DataIntegracao = DateTime.Now;
            cargaConsultaValePedagio.NumeroTentativas++;
            repCargaConsultaValorPedagio.Atualizar(cargaConsultaValePedagio);

            servicoArquivoTransacao.Adicionar(cargaConsultaValePedagio, inspector.LastRequestXML, inspector.LastResponseXML, "xml");

            return valorPedagio;
        }

        public void ConsultarIdVpo(Dominio.Entidades.Embarcador.Cargas.ValePedagio.CargaIntegracaoValePedagio cargaValePedagio, AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware tipoServicoMultisoftware)
        {
            Repositorio.Embarcador.Cargas.ValePedagio.CargaIntegracaoValePedagio repCargaValePedagio = new Repositorio.Embarcador.Cargas.ValePedagio.CargaIntegracaoValePedagio(_unitOfWork);
            Frota.ValePedagio servicoValePedagio = new Frota.ValePedagio(_unitOfWork);

            Dominio.Entidades.Embarcador.Configuracoes.IntegracaoDBTrans integracaoDBTrans = servicoValePedagio.ObterIntegracaoDBTrans(cargaValePedagio.Carga, tipoServicoMultisoftware);

            if (integracaoDBTrans == null)
            {
                cargaValePedagio.ProblemaIntegracao = "Não possui configuração para DBTrans/Rodocred.";
                cargaValePedagio.SituacaoIntegracao = SituacaoIntegracao.ProblemaIntegracao;
                cargaValePedagio.DataIntegracao = DateTime.Now;
                cargaValePedagio.NumeroTentativas++;
                repCargaValePedagio.Atualizar(cargaValePedagio);

                return;
            }

            ServicoDBTrans.ValePedagio.IdentificacaoIntegracaoType identificacaoIntegracao = ObterAutenticacao(integracaoDBTrans, cargaValePedagio);
            if (identificacaoIntegracao == null)
                return;

            cargaValePedagio.DataIntegracao = DateTime.Now;
            cargaValePedagio.NumeroTentativas++;

            ArquivoTransacao<Dominio.Entidades.Embarcador.Cargas.ValePedagio.CargaValePedagioIntegracaoArquivo> servicoArquivoTransacao = new ArquivoTransacao<Dominio.Entidades.Embarcador.Cargas.ValePedagio.CargaValePedagioIntegracaoArquivo>(_unitOfWork);

            ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;
            ServicoDBTrans.ValePedagio.ValePedagioSoapClient valePedagioSoapClient = ObterClient(integracaoDBTrans.URL);
            InspectorBehavior inspector = new InspectorBehavior();
            valePedagioSoapClient.Endpoint.EndpointBehaviors.Add(inspector);

            try
            {
                ServicoDBTrans.ValePedagio.ConsultarIdVpoRequest consultaIdVpo = ObterConsultaIdVpo(identificacaoIntegracao, cargaValePedagio);
                ServicoDBTrans.ValePedagio.ConsultarIdVpoResponse retorno = valePedagioSoapClient.ConsultarIdVPO(consultaIdVpo);

                ServicoDBTrans.ValePedagio.ConsultarIdVpoResponseRetornoMensagem documentoTranportadorRetorno = retorno.RetornoMensagem;

                if (documentoTranportadorRetorno.StatusRetorno != ServicoDBTrans.ValePedagio.ConsultarIdVpoResponseRetornoMensagemStatusRetorno.SUCESSO)
                    throw new ServicoException($"Consulta ID VPO: {documentoTranportadorRetorno.Excecao.CodigoExcecao} - {documentoTranportadorRetorno.Excecao.MensagemExcecao}");

                cargaValePedagio.ProblemaIntegracao = "Consulta ID VPO efetuada com sucesso";
                cargaValePedagio.CodigoEmissaoValePedagioANTT = retorno.IdentificadorVPO.FirstOrDefault();
                cargaValePedagio.SituacaoIntegracao = SituacaoIntegracao.AgRetorno;
            }
            catch (ServicoException excecao)
            {
                cargaValePedagio.SituacaoIntegracao = SituacaoIntegracao.ProblemaIntegracao;
                cargaValePedagio.ProblemaIntegracao = excecao.Message;
            }
            catch (Exception excecao)
            {
                Log.TratarErro(excecao);

                cargaValePedagio.SituacaoIntegracao = SituacaoIntegracao.ProblemaIntegracao;
                cargaValePedagio.ProblemaIntegracao = "Ocorreu uma falha ao realizar a consulta do ID VPO na DBTrans/Rodocred";
            }

            repCargaValePedagio.Atualizar(cargaValePedagio);

            servicoArquivoTransacao.Adicionar(cargaValePedagio, inspector.LastRequestXML, inspector.LastResponseXML, "xml");
        }

        public byte[] GerarImpressaoValePedagio(Dominio.Entidades.Embarcador.Cargas.ValePedagio.CargaIntegracaoValePedagio cargaValePedagio)
        {
            if (string.IsNullOrWhiteSpace(cargaValePedagio.Observacao6))
                return null;

            HttpClient client = HttpClientFactoryWrapper.GetClient(nameof(ValePedagio));
            byte[] fileBytes = client.GetByteArrayAsync(cargaValePedagio.Observacao6).Result;
            return fileBytes;
        }

        #endregion Métodos Públicos

        #region Métodos Privados

        private ServicoDBTrans.ValePedagio.ComprarPedagioRequest ObterCompraPedagio(ServicoDBTrans.ValePedagio.IdentificacaoIntegracaoType identificacaoIntegracao, Dominio.Entidades.Embarcador.Cargas.Carga carga, Dominio.Entidades.Embarcador.Cargas.ValePedagio.CargaIntegracaoValePedagio cargaValePedagio, Dominio.Entidades.Embarcador.Configuracoes.IntegracaoDBTrans integracaoDBTrans, int codigoRotaTemporaria, int meioPagamento)
        {
            Dominio.Entidades.Embarcador.Configuracoes.ConfiguracaoGeralCarga configuracaoGeralCarga = new Repositorio.Embarcador.Configuracoes.ConfiguracaoGeralCarga(_unitOfWork).BuscarPrimeiroRegistro();

            ServicoDBTrans.ValePedagio.ComprarPedagioRequest request = new ServicoDBTrans.ValePedagio.ComprarPedagioRequest()
            {
                IdentificacaoIntegracao = identificacaoIntegracao,
                OperacaoPedagio = ServicoDBTrans.ValePedagio.TipoOperacaoPedagioType.COM,
                DadosViagemPedagio = new ServicoDBTrans.ValePedagio.DadosViagemPedagioType()
                {
                    Embarque = carga.DataInicioViagem.HasValue && carga.DataInicioViagem.Value > DateTime.Now.Date ? carga.DataInicioViagem.Value : DateTime.Now.Date,
                    EmbarqueSpecified = true,
                    DocumentoRef = carga.CodigoCargaEmbarcador
                },
                Transportador = !integracaoDBTrans.NaoEnviarTransportadorNaIntegracao ? new ServicoDBTrans.ValePedagio.ComprarPedagioRequestTransportador()
                {
                    CNPJCPFTransportador = carga.Empresa.CNPJ_SemFormato,
                    NomeTransportador = carga.Empresa.RazaoSocial,
                    RNTRC = carga.Empresa.RegistroANTT
                } : null,
                RotaViagem = new ServicoDBTrans.ValePedagio.ComprarPedagioRequestRotaViagem()
                {
                    CodigoRota = new int[] { codigoRotaTemporaria > 0 ? codigoRotaTemporaria : cargaValePedagio.CodigoIntegracaoValePedagio.ToInt() }
                },
                VeiculosViagem = ObterVeiculos(carga, cargaValePedagio),
                ValePedagioViagem = new ServicoDBTrans.ValePedagio.ValePedagioViagemType()
                {
                    OperadoraFinanceiraVP = ServicoDBTrans.ValePedagio.ValePedagioViagemTypeOperadoraFinanceiraVP.DBT,
                    MeioPagamentoValePedagio = meioPagamento,
                    MeioPagamentoValePedagioSpecified = true,
                    ModalidadeLocalImpressao = ServicoDBTrans.ValePedagio.ModalidadeImpressaoType.LOC,
                    ModalidadeLocalImpressaoSpecified = true,
                    IDLocalImpressao = integracaoDBTrans.IdLocalImpressao,
                    IDLocalImpressaoSpecified = true,
                    GerarComprovante = ServicoDBTrans.ValePedagio.ValePedagioViagemTypeGerarComprovante.S,
                    GerarComprovanteSpecified = true
                },
                MotoristaViagem = ObterMotoristas(carga, integracaoDBTrans.NaoEnviarMotoristaNaIntegracao),
                GerarComprovante = ServicoDBTrans.ValePedagio.GerarComprovanteType.S
            };

            Repositorio.Embarcador.Cargas.ControleEntrega.CargaEntrega repositorioCargaEntrega = new Repositorio.Embarcador.Cargas.ControleEntrega.CargaEntrega(_unitOfWork);

            DateTime? dataPrevista = repositorioCargaEntrega.BuscarDataPrevistaMaximaPorCarga(carga.Codigo);

            if (dataPrevista.HasValue)
            {
                request.DadosViagemPedagio.PrevisaoEntrega = dataPrevista.Value.AddDays(5);
                request.DadosViagemPedagio.PrevisaoEntregaSpecified = true;
            }

            if (configuracaoGeralCarga.ConsiderarDataEmissaoCTECalculoEmbarquePrevisaoEntrega)
            {
                Repositorio.Embarcador.Cargas.CargaCTe repositorioCargaCTe = new Repositorio.Embarcador.Cargas.CargaCTe(_unitOfWork);

                DateTime? ultimaDataEmissaoCargaCte = repositorioCargaCTe.BuscarDataEmissaoMaximaSubContratacao(carga.Codigo);

                if (ultimaDataEmissaoCargaCte.HasValue)
                {
                    request.DadosViagemPedagio.PrevisaoEntrega = ultimaDataEmissaoCargaCte.Value.AddDays(7);
                    request.DadosViagemPedagio.PrevisaoEntregaSpecified = true;
                }
            }

            // Se a data de embarque continuar a ser maior que a data de previsão de entrega, a previsão de entrega será a data de embarque + 7 dias
            if (request.DadosViagemPedagio.PrevisaoEntrega != DateTime.MinValue && request.DadosViagemPedagio.PrevisaoEntrega < request.DadosViagemPedagio.Embarque)
                request.DadosViagemPedagio.PrevisaoEntrega = request.DadosViagemPedagio.Embarque.AddDays(7);

            return request;
        }

        private ServicoDBTrans.ValePedagio.ComprarPedagioRequestVeiculosViagem[] ObterVeiculos(Dominio.Entidades.Embarcador.Cargas.Carga carga, Dominio.Entidades.Embarcador.Cargas.ValePedagio.CargaIntegracaoValePedagio cargaIntegracaoValePedagio)
        {
            List<ServicoDBTrans.ValePedagio.ComprarPedagioRequestVeiculosViagem> veiculos = new List<ServicoDBTrans.ValePedagio.ComprarPedagioRequestVeiculosViagem>();

            if (carga.Veiculo != null)
            {
                ServicoDBTrans.ValePedagio.ComprarPedagioRequestVeiculosViagem veiculo = new ServicoDBTrans.ValePedagio.ComprarPedagioRequestVeiculosViagem()
                {
                    PlacaVeiculo = carga.Veiculo.Placa,
                    QtdeEixos = ObterNumeroDeEixos(carga.Veiculo, cargaIntegracaoValePedagio.CompraComEixosSuspensos).ToString(),
                    TipoRodagem = ObterTipoRodagem(carga),
                    TipoVeiculo = ObterTipoVeiculo(carga.Veiculo.ModeloVeicularCarga)
                };

                veiculos.Add(veiculo);
            }

            if (carga.VeiculosVinculados?.Count > 0)
            {
                foreach (Dominio.Entidades.Veiculo reboque in carga.VeiculosVinculados)
                {
                    if (carga.Veiculo?.ModeloVeicularCarga?.Codigo == reboque.ModeloVeicularCarga?.Codigo)
                        continue;

                    ServicoDBTrans.ValePedagio.ComprarPedagioRequestVeiculosViagem veiculo = new ServicoDBTrans.ValePedagio.ComprarPedagioRequestVeiculosViagem()
                    {
                        PlacaVeiculo = reboque.Placa,
                        QtdeEixos = ObterNumeroDeEixos(reboque, cargaIntegracaoValePedagio.CompraComEixosSuspensos).ToString(),
                        TipoRodagem = ServicoDBTrans.ValePedagio.TipoRodagemType.D,
                        TipoVeiculo = ObterTipoVeiculo(reboque.ModeloVeicularCarga)
                    };

                    veiculos.Add(veiculo);
                }
            }

            return veiculos.ToArray();
        }

        private ServicoDBTrans.ValePedagio.MotoristaViagemType[] ObterMotoristas(Dominio.Entidades.Embarcador.Cargas.Carga carga, bool naoEnviarMotoristaNaIntegracao)
        {
            if (naoEnviarMotoristaNaIntegracao)
                return null;

            List<ServicoDBTrans.ValePedagio.MotoristaViagemType> motoristas = new List<ServicoDBTrans.ValePedagio.MotoristaViagemType>();

            if (carga.Motoristas?.Count > 0)
            {
                foreach (Dominio.Entidades.Usuario motorista in carga.Motoristas)
                {
                    ServicoDBTrans.ValePedagio.MotoristaViagemType motoristaViagem = new ServicoDBTrans.ValePedagio.MotoristaViagemType()
                    {
                        CPFMotorista = motorista.CPF,
                        NomeMotorista = motorista.Nome
                    };

                    motoristas.Add(motoristaViagem);
                }
            }

            return motoristas.ToArray();
        }

        private int ObterTipoVeiculo(Dominio.Entidades.Embarcador.Cargas.ModeloVeicularCarga modeloVeicular)
        {
            /*Tipos de veículos esperados:
               1 = Automóvel, Pickup ou Utilitários Leve
               2 = Ônibus
               3 = Caminhão
               4 = Caminhão-trator
               5 = Semi-reboque
               6 = Reboque
               7 = Bitrem
               8 = Genérico*/

            if (modeloVeicular != null)
            {
                if (modeloVeicular.Tipo == TipoModeloVeicularCarga.Tracao || modeloVeicular.Tipo == TipoModeloVeicularCarga.Geral)
                {
                    if (modeloVeicular.PadraoEixos == PadraoEixosVeiculo.Simples)
                        return 1;
                    else
                        return 3;
                }
                else
                {
                    if (modeloVeicular.NumeroReboques == 0)
                        return 3;
                    else if (modeloVeicular.NumeroReboques == 1)
                        return 6;
                    else if (modeloVeicular.NumeroReboques == 2)
                        return 7;
                }
            }
            return 8;
        }

        private ServicoDBTrans.ValePedagio.TipoRodagemType ObterTipoRodagem(Dominio.Entidades.Embarcador.Cargas.Carga carga)
        {
            Dominio.Entidades.Veiculo veiculo = carga.Veiculo;

            if (veiculo?.ModeloVeicularCarga != null)
            {
                if (veiculo.ModeloVeicularCarga.PadraoEixos == PadraoEixosVeiculo.Duplo || veiculo.ModeloVeicularCarga.NumeroEixos > 2)
                    return ServicoDBTrans.ValePedagio.TipoRodagemType.D;
                else
                    return ServicoDBTrans.ValePedagio.TipoRodagemType.S;
            }

            if (!string.IsNullOrWhiteSpace(veiculo?.TipoRodado))
            {
                string tipoRodado = veiculo.TipoRodado;
                if (tipoRodado.Equals("01") || tipoRodado.Equals("02") || tipoRodado.Equals("03"))
                    return ServicoDBTrans.ValePedagio.TipoRodagemType.D;
            }

            return ServicoDBTrans.ValePedagio.TipoRodagemType.S;
        }

        private void SalvarDadosRetornoCompraValePedagio(ServicoDBTrans.ValePedagio.ComprarPedagioResponse retornoCompraValePedagio, Dominio.Entidades.Embarcador.Cargas.Carga carga)
        {
            Repositorio.Embarcador.Cargas.ValePedagio.CargaValePedagioDadosCompra repositorioValePedagioDadosCompra = new Repositorio.Embarcador.Cargas.ValePedagio.CargaValePedagioDadosCompra(_unitOfWork);
            Repositorio.Embarcador.Cargas.ValePedagio.CargaValePedagioDadosCompraPraca repositorioValePedagioDadosCompraPraca = new Repositorio.Embarcador.Cargas.ValePedagio.CargaValePedagioDadosCompraPraca(_unitOfWork);

            Dominio.Entidades.Embarcador.Cargas.ValePedagio.CargaValePedagioDadosCompra dadosCompra = new Dominio.Entidades.Embarcador.Cargas.ValePedagio.CargaValePedagioDadosCompra
            {
                Carga = carga,
                CodigoFilialCliente = "",
                CodigoProcessoCliente = carga.CodigoCargaEmbarcador,
                CodigoViagem = retornoCompraValePedagio.NumeroViagem.ToInt(),
                DataEmissao = DateTime.Now,
                ValorTotalPedagios = retornoCompraValePedagio.ValorPedagio
            };

            repositorioValePedagioDadosCompra.Inserir(dadosCompra);

            foreach (ServicoDBTrans.ValePedagio.ArrayOfListaCupomValePedagioTypeCupomCupom cupom in retornoCompraValePedagio.ListaCupomValePedagio)
            {
                Dominio.Entidades.Embarcador.Cargas.ValePedagio.CargaValePedagioDadosCompraPraca praca = new Dominio.Entidades.Embarcador.Cargas.ValePedagio.CargaValePedagioDadosCompraPraca
                {
                    CargaValePedagioDadosCompra = dadosCompra,
                    CodigoPraca = cupom.Praca.IdPraca.ToInt(),
                    ConcessionariaCodigo = 0,
                    ConcessionariaNome = cupom.Praca.Concessionaria,
                    NomePraca = cupom.Praca.NomePraca,
                    NomeRodovia = cupom.Praca.LocalPraca,
                    NumeroEixos = cupom.QtdeEixos.ToInt(),
                    Valor = cupom.ValorUnitarioPedagio
                };

                repositorioValePedagioDadosCompraPraca.Inserir(praca);
            }
        }

        private int ObterRotaTemporaria(ServicoDBTrans.ValePedagio.IdentificacaoIntegracaoType identificacaoIntegracao, Dominio.Entidades.Embarcador.Configuracoes.IntegracaoDBTrans integracaoDBTrans, Dominio.Entidades.Embarcador.Cargas.ValePedagio.CargaIntegracaoValePedagio cargaValePedagio)
        {
            Repositorio.Embarcador.Cargas.ValePedagio.CargaIntegracaoValePedagio repCargaValePedagio = new Repositorio.Embarcador.Cargas.ValePedagio.CargaIntegracaoValePedagio(_unitOfWork);

            string mensagem;
            try
            {
                Dominio.Entidades.Localidade localidadeOrigem, localidadeDestino;
                List<Dominio.Entidades.Localidade> pontosPassagem;

                ObterLocalidadesValePedagioCarga(cargaValePedagio.Carga, out localidadeOrigem, out localidadeDestino, out pontosPassagem, cargaValePedagio.RotaFrete, cargaValePedagio.TipoPercursoVP);

                int codigoRota = ConsultarRotaTemporaria(identificacaoIntegracao, integracaoDBTrans, cargaValePedagio, localidadeOrigem, localidadeDestino);

                if (codigoRota == 0)
                    codigoRota = CadastrarRotaTemporaria(identificacaoIntegracao, integracaoDBTrans, cargaValePedagio, localidadeOrigem, localidadeDestino, pontosPassagem);

                if (codigoRota == 0)
                    codigoRota = CadastrarRotaTemporaria(identificacaoIntegracao, integracaoDBTrans, cargaValePedagio, localidadeOrigem, localidadeDestino, pontosPassagem);

                if (codigoRota == 0)
                    throw new ServicoException("Não foi possível encontrar/cadastrar a rota temporária na DBTrans/Rodocred");

                return codigoRota;
            }
            catch (ServicoException excecao)
            {
                mensagem = excecao.Message;
            }
            catch (Exception excecao)
            {
                Log.TratarErro(excecao);
                mensagem = "Ocorreu uma falha na obtenção da rota temporária da DBTrans/Rodocred";
            }

            cargaValePedagio.SituacaoIntegracao = SituacaoIntegracao.ProblemaIntegracao;
            cargaValePedagio.ProblemaIntegracao = mensagem;
            repCargaValePedagio.Atualizar(cargaValePedagio);

            return 0;
        }

        private int ObterRotaTemporaria(ServicoDBTrans.ValePedagio.IdentificacaoIntegracaoType identificacaoIntegracao, Dominio.Entidades.Embarcador.Configuracoes.IntegracaoDBTrans integracaoDBTrans, Dominio.Entidades.Embarcador.Cargas.ValePedagio.CargaConsultaValorPedagioIntegracao cargaConsultaValePedagio)
        {
            Repositorio.Embarcador.Cargas.ValePedagio.CargaConsultaValorPedagioIntegracao repCargaConsultaValorPedagio = new Repositorio.Embarcador.Cargas.ValePedagio.CargaConsultaValorPedagioIntegracao(_unitOfWork);

            Servicos.Embarcador.Localidades.Localidade servicoLocalidade = new Servicos.Embarcador.Localidades.Localidade(_unitOfWork);

            string mensagem;
            try
            {
                Dominio.Entidades.Localidade localidadeOrigem, localidadeDestino;
                List<Dominio.Entidades.Localidade> pontosPassagem;
                servicoLocalidade.ObterLocalidadesValePedagioCarga(cargaConsultaValePedagio.Carga, out localidadeOrigem, out localidadeDestino, out pontosPassagem, _unitOfWork);

                int codigoRota = ConsultarRotaTemporaria(identificacaoIntegracao, integracaoDBTrans, cargaConsultaValePedagio, localidadeOrigem, localidadeDestino);

                if (codigoRota == 0)
                    codigoRota = CadastrarRotaTemporaria(identificacaoIntegracao, integracaoDBTrans, cargaConsultaValePedagio, localidadeOrigem, localidadeDestino, pontosPassagem);

                if (codigoRota == 0)
                    throw new ServicoException("Não foi possível encontrar/cadastrar a rota temporária na DBTrans/Rodocred");

                return codigoRota;
            }
            catch (ServicoException excecao)
            {
                mensagem = excecao.Message;
            }
            catch (Exception excecao)
            {
                Log.TratarErro(excecao);
                mensagem = "Ocorreu uma falha na obtenção da rota temporária da DBTrans/Rodocred";
            }

            cargaConsultaValePedagio.SituacaoIntegracao = SituacaoIntegracao.ProblemaIntegracao;
            cargaConsultaValePedagio.ProblemaIntegracao = mensagem;
            repCargaConsultaValorPedagio.Atualizar(cargaConsultaValePedagio);

            return 0;
        }

        private int ConsultarRotaTemporaria(ServicoDBTrans.ValePedagio.IdentificacaoIntegracaoType identificacaoIntegracao, Dominio.Entidades.Embarcador.Configuracoes.IntegracaoDBTrans integracaoDBTrans, Dominio.Entidades.Embarcador.Cargas.ValePedagio.CargaIntegracaoValePedagio cargaValePedagio, Dominio.Entidades.Localidade localidadeOrigem, Dominio.Entidades.Localidade localidadeDestino)
        {
            ArquivoTransacao<Dominio.Entidades.Embarcador.Cargas.ValePedagio.CargaValePedagioIntegracaoArquivo> servicoArquivoTransacao = new ArquivoTransacao<Dominio.Entidades.Embarcador.Cargas.ValePedagio.CargaValePedagioIntegracaoArquivo>(_unitOfWork);

            cargaValePedagio.DataIntegracao = DateTime.Now;

            System.Net.ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;
            ServicoDBTrans.ValePedagio.ValePedagioSoapClient valePedagioSoapClient = ObterClient(integracaoDBTrans.URL);
            InspectorBehavior inspector = new InspectorBehavior();
            valePedagioSoapClient.Endpoint.EndpointBehaviors.Add(inspector);

            try
            {
                ServicoDBTrans.ValePedagio.ConsultarRotasRequest consultarRotas = ObterConsultaRota(identificacaoIntegracao, cargaValePedagio.Carga, localidadeOrigem, localidadeDestino);
                ServicoDBTrans.ValePedagio.ConsultarRotasResponse retorno = valePedagioSoapClient.ConsultarRotas(consultarRotas);

                int codigoRota = 0;
                string mensagem;
                if (retorno.RetornoMensagem.StatusRetorno == ServicoDBTrans.ValePedagio.ConsultarRotasResponseRetornoMensagemStatusRetorno.SUCESSO)
                {
                    codigoRota = retorno.ListaRotas[0].CodigoRota;
                    mensagem = "Consulta Rota Temporária efetuada com sucesso";
                }
                else
                    mensagem = "Consulta Rota Temporária: " + retorno.RetornoMensagem.Excecao.CodigoExcecao + " - " + retorno.RetornoMensagem.Excecao.MensagemExcecao;

                servicoArquivoTransacao.Adicionar(cargaValePedagio, inspector.LastRequestXML, inspector.LastResponseXML, "xml", mensagem);

                return codigoRota;
            }
            catch (Exception excecao)
            {
                Log.TratarErro(excecao);
                string mensagem = "Ocorreu uma falha ao realizar a consulta de rota temporária da DBTrans/Rodocred";

                servicoArquivoTransacao.Adicionar(cargaValePedagio, inspector.LastRequestXML, inspector.LastResponseXML, "xml", mensagem);

                throw new ServicoException(mensagem);
            }
        }

        private int ConsultarRotaTemporaria(ServicoDBTrans.ValePedagio.IdentificacaoIntegracaoType identificacaoIntegracao, Dominio.Entidades.Embarcador.Configuracoes.IntegracaoDBTrans integracaoDBTrans, Dominio.Entidades.Embarcador.Cargas.ValePedagio.CargaConsultaValorPedagioIntegracao cargaConsultaValePedagio, Dominio.Entidades.Localidade localidadeOrigem, Dominio.Entidades.Localidade localidadeDestino)
        {
            ArquivoTransacao<Dominio.Entidades.Embarcador.Cargas.ValePedagio.CargaValePedagioIntegracaoArquivo> servicoArquivoTransacao = new ArquivoTransacao<Dominio.Entidades.Embarcador.Cargas.ValePedagio.CargaValePedagioIntegracaoArquivo>(_unitOfWork);

            cargaConsultaValePedagio.DataIntegracao = DateTime.Now;

            System.Net.ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;
            ServicoDBTrans.ValePedagio.ValePedagioSoapClient valePedagioSoapClient = ObterClient(integracaoDBTrans.URL);
            InspectorBehavior inspector = new InspectorBehavior();
            valePedagioSoapClient.Endpoint.EndpointBehaviors.Add(inspector);

            try
            {
                ServicoDBTrans.ValePedagio.ConsultarRotasRequest consultarRotas = ObterConsultaRota(identificacaoIntegracao, cargaConsultaValePedagio.Carga, localidadeOrigem, localidadeDestino);
                ServicoDBTrans.ValePedagio.ConsultarRotasResponse retorno = valePedagioSoapClient.ConsultarRotas(consultarRotas);

                int codigoRota = 0;
                string mensagem;
                if (retorno.RetornoMensagem.StatusRetorno == ServicoDBTrans.ValePedagio.ConsultarRotasResponseRetornoMensagemStatusRetorno.SUCESSO)
                {
                    codigoRota = retorno.ListaRotas[0].CodigoRota;
                    mensagem = "Consulta Rota Temporária efetuada com sucesso";
                }
                else
                    mensagem = "Consulta Rota Temporária: " + retorno.RetornoMensagem.Excecao.CodigoExcecao + " - " + retorno.RetornoMensagem.Excecao.MensagemExcecao;

                servicoArquivoTransacao.Adicionar(cargaConsultaValePedagio, inspector.LastRequestXML, inspector.LastResponseXML, "xml", mensagem);

                return codigoRota;
            }
            catch (Exception excecao)
            {
                Log.TratarErro(excecao);
                string mensagem = "Ocorreu uma falha ao realizar a consulta de rota temporária da DBTrans/Rodocred";

                servicoArquivoTransacao.Adicionar(cargaConsultaValePedagio, inspector.LastRequestXML, inspector.LastResponseXML, "xml", mensagem);

                throw new ServicoException(mensagem);
            }
        }

        private ServicoDBTrans.ValePedagio.ConsultarRotasRequest ObterConsultaRota(ServicoDBTrans.ValePedagio.IdentificacaoIntegracaoType identificacaoIntegracao, Dominio.Entidades.Embarcador.Cargas.Carga carga, Dominio.Entidades.Localidade localidadeOrigem, Dominio.Entidades.Localidade localidadeDestino)
        {
            ServicoDBTrans.ValePedagio.ConsultarRotasRequest request = new ServicoDBTrans.ValePedagio.ConsultarRotasRequest()
            {
                IdentificacaoIntegracao = identificacaoIntegracao,
                FiltroConsultaRota = new ServicoDBTrans.ValePedagio.ConsultarRotasRequestFiltroConsultaRota()
                {
                    NomeRota = carga.Codigo.ToString(),
                    OrigemRota = new ServicoDBTrans.ValePedagio.ConsultarRotasRequestFiltroConsultaRotaOrigemRota()
                    {
                        CodigoIBGEOrigem = localidadeOrigem.CodigoIBGE.ToString()
                    },
                    DestinoRota = new ServicoDBTrans.ValePedagio.ConsultarRotasRequestFiltroConsultaRotaDestinoRota()
                    {
                        CodigoIBGEDestino = localidadeDestino.CodigoIBGE.ToString()
                    }
                }
            };

            return request;
        }

        private int CadastrarRotaTemporaria(ServicoDBTrans.ValePedagio.IdentificacaoIntegracaoType identificacaoIntegracao, Dominio.Entidades.Embarcador.Configuracoes.IntegracaoDBTrans integracaoDBTrans, Dominio.Entidades.Embarcador.Cargas.ValePedagio.CargaIntegracaoValePedagio cargaValePedagio, Dominio.Entidades.Localidade localidadeOrigem, Dominio.Entidades.Localidade localidadeDestino, List<Dominio.Entidades.Localidade> pontosPassagem)
        {
            ArquivoTransacao<Dominio.Entidades.Embarcador.Cargas.ValePedagio.CargaValePedagioIntegracaoArquivo> servicoArquivoTransacao = new ArquivoTransacao<Dominio.Entidades.Embarcador.Cargas.ValePedagio.CargaValePedagioIntegracaoArquivo>(_unitOfWork);

            System.Net.ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;
            ServicoDBTrans.ValePedagio.ValePedagioSoapClient valePedagioSoapClient = ObterClient(integracaoDBTrans.URL);
            InspectorBehavior inspector = new InspectorBehavior();
            valePedagioSoapClient.Endpoint.EndpointBehaviors.Add(inspector);

            try
            {
                ServicoDBTrans.ValePedagio.ManterRotaRequest manterRota = ObterManterRota(identificacaoIntegracao, integracaoDBTrans, cargaValePedagio, localidadeOrigem, localidadeDestino, pontosPassagem);
                ServicoDBTrans.ValePedagio.ManterRotaResponse retorno = valePedagioSoapClient.ManterRota(manterRota);

                int codigoRota = 0;
                string mensagem;
                if (retorno.RetornoMensagem.StatusRetorno == ServicoDBTrans.ValePedagio.ManterRotaResponseRetornoMensagemStatusRetorno.SUCESSO)
                {
                    codigoRota = retorno.Rota.CodigoRota;
                    mensagem = "Cadastro Rota Temporária efetuada com sucesso";
                }
                else
                    mensagem = "Cadastro Rota Temporária: " + retorno.RetornoMensagem.Excecao.CodigoExcecao + " - " + retorno.RetornoMensagem.Excecao.MensagemExcecao;

                cargaValePedagio.DataIntegracao = DateTime.Now;
                servicoArquivoTransacao.Adicionar(cargaValePedagio, inspector.LastRequestXML, inspector.LastResponseXML, "xml", mensagem);

                return codigoRota;
            }
            catch (Exception excecao)
            {
                Log.TratarErro(excecao);
                string mensagem = "Ocorreu uma falha ao realizar o manter rota temporária da DBTrans/Rodocred";

                cargaValePedagio.DataIntegracao = DateTime.Now;
                servicoArquivoTransacao.Adicionar(cargaValePedagio, inspector.LastRequestXML, inspector.LastResponseXML, "xml", mensagem);

                throw new ServicoException(mensagem);
            }
        }

        private int CadastrarRotaTemporaria(ServicoDBTrans.ValePedagio.IdentificacaoIntegracaoType identificacaoIntegracao, Dominio.Entidades.Embarcador.Configuracoes.IntegracaoDBTrans integracaoDBTrans, Dominio.Entidades.Embarcador.Cargas.ValePedagio.CargaConsultaValorPedagioIntegracao cargaConsultaValePedagio, Dominio.Entidades.Localidade localidadeOrigem, Dominio.Entidades.Localidade localidadeDestino, List<Dominio.Entidades.Localidade> pontosPassagem)
        {
            ArquivoTransacao<Dominio.Entidades.Embarcador.Cargas.ValePedagio.CargaValePedagioIntegracaoArquivo> servicoArquivoTransacao = new ArquivoTransacao<Dominio.Entidades.Embarcador.Cargas.ValePedagio.CargaValePedagioIntegracaoArquivo>(_unitOfWork);

            cargaConsultaValePedagio.DataIntegracao = DateTime.Now;

            System.Net.ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;
            ServicoDBTrans.ValePedagio.ValePedagioSoapClient valePedagioSoapClient = ObterClient(integracaoDBTrans.URL);
            InspectorBehavior inspector = new InspectorBehavior();
            valePedagioSoapClient.Endpoint.EndpointBehaviors.Add(inspector);

            try
            {
                ServicoDBTrans.ValePedagio.ManterRotaRequest manterRota = ObterManterRota(identificacaoIntegracao, integracaoDBTrans, cargaConsultaValePedagio, localidadeOrigem, localidadeDestino, pontosPassagem);
                ServicoDBTrans.ValePedagio.ManterRotaResponse retorno = valePedagioSoapClient.ManterRota(manterRota);

                int codigoRota = 0;
                string mensagem;
                if (retorno.RetornoMensagem.StatusRetorno == ServicoDBTrans.ValePedagio.ManterRotaResponseRetornoMensagemStatusRetorno.SUCESSO)
                {
                    codigoRota = retorno.Rota.CodigoRota;
                    mensagem = "Cadastro Rota Temporária efetuada com sucesso";
                }
                else
                    mensagem = "Cadastro Rota Temporária: " + retorno.RetornoMensagem.Excecao.CodigoExcecao + " - " + retorno.RetornoMensagem.Excecao.MensagemExcecao;

                servicoArquivoTransacao.Adicionar(cargaConsultaValePedagio, inspector.LastRequestXML, inspector.LastResponseXML, "xml", mensagem);

                return codigoRota;
            }
            catch (Exception excecao)
            {
                Log.TratarErro(excecao);
                string mensagem = "Ocorreu uma falha ao realizar o manter rota temporária da DBTrans/Rodocred";

                servicoArquivoTransacao.Adicionar(cargaConsultaValePedagio, inspector.LastRequestXML, inspector.LastResponseXML, "xml", mensagem);

                throw new ServicoException(mensagem);
            }
        }

        private ServicoDBTrans.ValePedagio.ManterRotaRequest ObterManterRota(ServicoDBTrans.ValePedagio.IdentificacaoIntegracaoType identificacaoIntegracao, Dominio.Entidades.Embarcador.Configuracoes.IntegracaoDBTrans integracaoDBTrans, Dominio.Entidades.Embarcador.Cargas.ValePedagio.CargaIntegracaoValePedagio cargaValePedagio, Dominio.Entidades.Localidade localidadeOrigem, Dominio.Entidades.Localidade localidadeDestino, List<Dominio.Entidades.Localidade> pontosPassagem)
        {
            int[] listaPontosIntermediarios = ObterListaPontosIntermediarios(identificacaoIntegracao, integracaoDBTrans, cargaValePedagio, pontosPassagem);

            ServicoDBTrans.ValePedagio.ManterRotaRequest request = new ServicoDBTrans.ValePedagio.ManterRotaRequest()
            {
                IdentificacaoIntegracao = identificacaoIntegracao,
                Operacao = ServicoDBTrans.ValePedagio.TipoOperacaoType.INC,
                NomeRota = cargaValePedagio.Carga.Codigo.ToString() + DateTime.Now.Second,
                DescricaoRota = "Rota inserida por integração TMS",
                OrigemRota = new ServicoDBTrans.ValePedagio.ManterRotaRequestOrigemRota()
                {
                    CodigoIBGEOrigem = localidadeOrigem.CodigoIBGE.ToString()
                },
                ListaPontosIntermediarios = listaPontosIntermediarios,
                DestinoRota = new ServicoDBTrans.ValePedagio.ManterRotaRequestDestinoRota
                {
                    CodigoIBGEDestino = localidadeDestino.CodigoIBGE.ToString()
                }
            };

            return request;
        }

        private ServicoDBTrans.ValePedagio.ManterRotaRequest ObterManterRota(ServicoDBTrans.ValePedagio.IdentificacaoIntegracaoType identificacaoIntegracao, Dominio.Entidades.Embarcador.Configuracoes.IntegracaoDBTrans integracaoDBTrans, Dominio.Entidades.Embarcador.Cargas.ValePedagio.CargaConsultaValorPedagioIntegracao cargaConsultaValePedagio, Dominio.Entidades.Localidade localidadeOrigem, Dominio.Entidades.Localidade localidadeDestino, List<Dominio.Entidades.Localidade> pontosPassagem)
        {
            int[] listaPontosIntermediarios = ObterListaPontosIntermediarios(identificacaoIntegracao, integracaoDBTrans, cargaConsultaValePedagio, pontosPassagem);

            ServicoDBTrans.ValePedagio.ManterRotaRequest request = new ServicoDBTrans.ValePedagio.ManterRotaRequest()
            {
                IdentificacaoIntegracao = identificacaoIntegracao,
                Operacao = ServicoDBTrans.ValePedagio.TipoOperacaoType.INC,
                NomeRota = cargaConsultaValePedagio.Carga.Codigo.ToString(),
                DescricaoRota = "Rota inserida por integração TMS",
                OrigemRota = new ServicoDBTrans.ValePedagio.ManterRotaRequestOrigemRota()
                {
                    CodigoIBGEOrigem = localidadeOrigem.CodigoIBGE.ToString()
                },
                ListaPontosIntermediarios = listaPontosIntermediarios,
                DestinoRota = new ServicoDBTrans.ValePedagio.ManterRotaRequestDestinoRota
                {
                    CodigoIBGEDestino = localidadeDestino.CodigoIBGE.ToString()
                }
            };

            return request;
        }

        private int[] ObterListaPontosIntermediarios(ServicoDBTrans.ValePedagio.IdentificacaoIntegracaoType identificacaoIntegracao, Dominio.Entidades.Embarcador.Configuracoes.IntegracaoDBTrans integracaoDBTrans, Dominio.Entidades.Embarcador.Cargas.ValePedagio.CargaIntegracaoValePedagio cargaValePedagio, List<Dominio.Entidades.Localidade> pontosPassagem)
        {
            if (pontosPassagem == null || pontosPassagem.Count == 0)
                return null;

            List<int> pontosIntermediarios = new List<int>();
            List<(int CodigoRodocred, int CodigoPontoPassagem)> pontosIntermediariosComPontoPassagem = new List<(int CodigoRodocred, int CodigoPontoPassagem)>();

            foreach (Dominio.Entidades.Localidade localidade in pontosPassagem)
            {
                int codigoLocalidade = pontosIntermediariosComPontoPassagem.Where(o => o.CodigoPontoPassagem == localidade.Codigo).Select(o => o.CodigoRodocred).FirstOrDefault();

                if (codigoLocalidade == 0)
                    codigoLocalidade = ConsultarLocalidade(identificacaoIntegracao, integracaoDBTrans, cargaValePedagio, localidade);

                if (codigoLocalidade > 0)
                {
                    pontosIntermediarios.Add(codigoLocalidade);
                    pontosIntermediariosComPontoPassagem.Add(ValueTuple.Create(codigoLocalidade, localidade.Codigo));
                }
            }

            return pontosIntermediarios.Count > 0 ? pontosIntermediarios.ToArray() : null;
        }

        private int[] ObterListaPontosIntermediarios(ServicoDBTrans.ValePedagio.IdentificacaoIntegracaoType identificacaoIntegracao, Dominio.Entidades.Embarcador.Configuracoes.IntegracaoDBTrans integracaoDBTrans, Dominio.Entidades.Embarcador.Cargas.ValePedagio.CargaConsultaValorPedagioIntegracao cargaConsultaValePedagio, List<Dominio.Entidades.Localidade> pontosPassagem)
        {
            if (pontosPassagem == null || pontosPassagem.Count == 0)
                return null;

            List<int> pontosIntermediarios = new List<int>();
            List<(int CodigoRodocred, int CodigoPontoPassagem)> pontosIntermediariosComPontoPassagem = new List<(int CodigoRodocred, int CodigoPontoPassagem)>();

            foreach (Dominio.Entidades.Localidade localidade in pontosPassagem)
            {
                int codigoLocalidade = pontosIntermediariosComPontoPassagem.Where(o => o.CodigoPontoPassagem == localidade.Codigo).Select(o => o.CodigoRodocred).FirstOrDefault();

                if (codigoLocalidade == 0)
                    codigoLocalidade = ConsultarLocalidade(identificacaoIntegracao, integracaoDBTrans, cargaConsultaValePedagio, localidade);

                if (codigoLocalidade > 0)
                {
                    pontosIntermediarios.Add(codigoLocalidade);
                    pontosIntermediariosComPontoPassagem.Add(ValueTuple.Create(codigoLocalidade, localidade.Codigo));
                }
            }

            return pontosIntermediarios.Count > 0 ? pontosIntermediarios.ToArray() : null;
        }

        private int ConsultarLocalidade(ServicoDBTrans.ValePedagio.IdentificacaoIntegracaoType identificacaoIntegracao, Dominio.Entidades.Embarcador.Configuracoes.IntegracaoDBTrans integracaoDBTrans, Dominio.Entidades.Embarcador.Cargas.ValePedagio.CargaIntegracaoValePedagio cargaValePedagio, Dominio.Entidades.Localidade localidade)
        {
            ArquivoTransacao<Dominio.Entidades.Embarcador.Cargas.ValePedagio.CargaValePedagioIntegracaoArquivo> servicoArquivoTransacao = new ArquivoTransacao<Dominio.Entidades.Embarcador.Cargas.ValePedagio.CargaValePedagioIntegracaoArquivo>(_unitOfWork);

            cargaValePedagio.DataIntegracao = DateTime.Now;

            System.Net.ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;
            ServicoDBTrans.ValePedagio.ValePedagioSoapClient valePedagioSoapClient = ObterClient(integracaoDBTrans.URL);
            InspectorBehavior inspector = new InspectorBehavior();
            valePedagioSoapClient.Endpoint.EndpointBehaviors.Add(inspector);

            try
            {
                ServicoDBTrans.ValePedagio.ConsultarLocalidadesRequest consultarLocalidades = ObterLocalidade(identificacaoIntegracao, localidade);
                ServicoDBTrans.ValePedagio.ConsultarLocalidadesResponse retorno = valePedagioSoapClient.ConsultarLocalidades(consultarLocalidades);

                int codigoLocalidade = 0;
                string mensagem;
                if (retorno.RetornoMensagem.StatusRetorno == ServicoDBTrans.ValePedagio.ConsultarLocalidadesResponseRetornoMensagemStatusRetorno.SUCESSO)
                {
                    codigoLocalidade = ObterCodigoLocalidadeResponse(localidade, retorno.ListaLocalidades);
                    mensagem = $"Consulta da localidade {localidade.Descricao} de ponto de passagem efetuada com sucesso";
                }
                else
                    mensagem = $"Retorno consulta da localidade {localidade.Descricao} de ponto de passagem: {retorno.RetornoMensagem.Excecao.CodigoExcecao} - {retorno.RetornoMensagem.Excecao.MensagemExcecao}";

                servicoArquivoTransacao.Adicionar(cargaValePedagio, inspector.LastRequestXML, inspector.LastResponseXML, "xml", mensagem);

                if (codigoLocalidade == 0)
                    throw new ServicoException(mensagem);

                return codigoLocalidade;
            }
            catch (Exception excecao)
            {
                Log.TratarErro(excecao);
                string mensagem = "Ocorreu uma falha ao consultar a localidade do ponto de passagem da DBTrans/Rodocred";

                servicoArquivoTransacao.Adicionar(cargaValePedagio, inspector.LastRequestXML, inspector.LastResponseXML, "xml", mensagem);

                throw new ServicoException(mensagem);
            }
        }

        private int ConsultarLocalidade(ServicoDBTrans.ValePedagio.IdentificacaoIntegracaoType identificacaoIntegracao, Dominio.Entidades.Embarcador.Configuracoes.IntegracaoDBTrans integracaoDBTrans, Dominio.Entidades.Embarcador.Cargas.ValePedagio.CargaConsultaValorPedagioIntegracao cargaConsultaValePedagio, Dominio.Entidades.Localidade localidade)
        {
            ArquivoTransacao<Dominio.Entidades.Embarcador.Cargas.ValePedagio.CargaValePedagioIntegracaoArquivo> servicoArquivoTransacao = new ArquivoTransacao<Dominio.Entidades.Embarcador.Cargas.ValePedagio.CargaValePedagioIntegracaoArquivo>(_unitOfWork);

            cargaConsultaValePedagio.DataIntegracao = DateTime.Now;

            System.Net.ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;
            ServicoDBTrans.ValePedagio.ValePedagioSoapClient valePedagioSoapClient = ObterClient(integracaoDBTrans.URL);
            InspectorBehavior inspector = new InspectorBehavior();
            valePedagioSoapClient.Endpoint.EndpointBehaviors.Add(inspector);

            try
            {
                ServicoDBTrans.ValePedagio.ConsultarLocalidadesRequest consultarLocalidades = ObterLocalidade(identificacaoIntegracao, localidade);
                ServicoDBTrans.ValePedagio.ConsultarLocalidadesResponse retorno = valePedagioSoapClient.ConsultarLocalidades(consultarLocalidades);

                int codigoLocalidade = 0;
                string mensagem;
                if (retorno.RetornoMensagem.StatusRetorno == ServicoDBTrans.ValePedagio.ConsultarLocalidadesResponseRetornoMensagemStatusRetorno.SUCESSO)
                {
                    codigoLocalidade = ObterCodigoLocalidadeResponse(localidade, retorno.ListaLocalidades);
                    mensagem = $"Consulta da localidade {localidade.Descricao} de ponto de passagem efetuada com sucesso";
                }
                else
                    mensagem = $"Retorno consulta da localidade {localidade.Descricao} de ponto de passagem: {retorno.RetornoMensagem.Excecao.CodigoExcecao} - {retorno.RetornoMensagem.Excecao.MensagemExcecao}";

                servicoArquivoTransacao.Adicionar(cargaConsultaValePedagio, inspector.LastRequestXML, inspector.LastResponseXML, "xml", mensagem);

                if (codigoLocalidade == 0)
                    throw new ServicoException(mensagem);

                return codigoLocalidade;
            }
            catch (Exception excecao)
            {
                Log.TratarErro(excecao);
                string mensagem = "Ocorreu uma falha ao consultar a localidade do ponto de passagem da DBTrans/Rodocred";

                servicoArquivoTransacao.Adicionar(cargaConsultaValePedagio, inspector.LastRequestXML, inspector.LastResponseXML, "xml", mensagem);

                throw new ServicoException(mensagem);
            }
        }

        private ServicoDBTrans.ValePedagio.ConsultarLocalidadesRequest ObterLocalidade(ServicoDBTrans.ValePedagio.IdentificacaoIntegracaoType identificacaoIntegracao, Dominio.Entidades.Localidade localidade)
        {
            ServicoDBTrans.ValePedagio.ConsultarLocalidadesRequest request = new ServicoDBTrans.ValePedagio.ConsultarLocalidadesRequest()
            {
                IdentificacaoIntegracao = identificacaoIntegracao,
                Localidade = localidade.Descricao,
                TipoLocalidade = new ServicoDBTrans.ValePedagio.TipoLocalidadeType
                {
                    Cidades = true,
                    PontoReferencia = false,
                    Vila = false
                },
                UF = localidade.Estado.Sigla.ToEnum<ServicoDBTrans.ValePedagio.Estado>()
            };

            return request;
        }

        private int ObterCodigoLocalidadeResponse(Dominio.Entidades.Localidade localidade, ServicoDBTrans.ValePedagio.LocalidadesType[] listaLocalidadesRetorno)
        {
            ServicoDBTrans.ValePedagio.LocalidadesType localidadeDefinida = listaLocalidadesRetorno.FirstOrDefault(o => o.CodigoIBGELocalidade == localidade.CodigoIBGE.ToString());

            if (localidadeDefinida == null)
                throw new ServicoException($"Não encontrado a localidade {localidade.Descricao} de código IBGE {localidade.CodigoIBGE} na lista de retorno da consulta");

            return localidadeDefinida.CodigoLocalidade;
        }

        private int ConsultarVeiculoSemParar(ServicoDBTrans.ValePedagio.IdentificacaoIntegracaoType identificacaoIntegracao, Dominio.Entidades.Embarcador.Configuracoes.IntegracaoDBTrans integracaoDBTrans, Dominio.Entidades.Embarcador.Cargas.ValePedagio.CargaIntegracaoValePedagio cargaValePedagio)
        {
            Repositorio.Embarcador.Cargas.ValePedagio.CargaIntegracaoValePedagio repCargaValePedagio = new Repositorio.Embarcador.Cargas.ValePedagio.CargaIntegracaoValePedagio(_unitOfWork);

            ArquivoTransacao<Dominio.Entidades.Embarcador.Cargas.ValePedagio.CargaValePedagioIntegracaoArquivo> servicoArquivoTransacao = new ArquivoTransacao<Dominio.Entidades.Embarcador.Cargas.ValePedagio.CargaValePedagioIntegracaoArquivo>(_unitOfWork);

            ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;
            ServicoDBTrans.ValePedagio.ValePedagioSoapClient valePedagioSoapClient = ObterClient(integracaoDBTrans.URL);
            InspectorBehavior inspector = new InspectorBehavior();
            valePedagioSoapClient.Endpoint.EndpointBehaviors.Add(inspector);

            string mensagem;

            try
            {
                ServicoDBTrans.ValePedagio.ConsultarVeiculoSemPararRequest consultarVeiculo = ObterConsultaVeiculoSemParar(identificacaoIntegracao, cargaValePedagio.Carga.Veiculo);
                ServicoDBTrans.ValePedagio.ConsultarVeiculoSemPararResponse retorno = valePedagioSoapClient.ConsultarVeiculoSemParar(consultarVeiculo);

                int meioPagamento = 0;
                if (retorno.RetornoMensagem.StatusRetorno == ServicoDBTrans.ValePedagio.ConsultarVeiculoSemPararResponseRetornoMensagemStatusRetorno.SUCESSO)
                {
                    meioPagamento = retorno.VeiculoElegivelVPSemParar ? (int)MeioPagamentoDBTrans.ValePedagioAutoExpresso : (int)MeioPagamentoDBTrans.Cupom;
                    mensagem = "Consulta veículo sem parar efetuada com sucesso";
                }
                else
                    throw new ServicoException("Consulta veículo sem parar: " + retorno.RetornoMensagem.Excecao.CodigoExcecao + " - " + retorno.RetornoMensagem.Excecao.MensagemExcecao);

                cargaValePedagio.DataIntegracao = DateTime.Now;
                servicoArquivoTransacao.Adicionar(cargaValePedagio, inspector.LastRequestXML, inspector.LastResponseXML, "xml", mensagem);

                return meioPagamento;
            }
            catch (ServicoException excecao)
            {
                mensagem = excecao.Message;
            }
            catch (Exception excecao)
            {
                Log.TratarErro(excecao);

                mensagem = "Ocorreu uma falha ao realizar a consulta veículo sem parar da DBTrans/Rodocred";
            }

            cargaValePedagio.DataIntegracao = DateTime.Now;
            cargaValePedagio.SituacaoIntegracao = SituacaoIntegracao.ProblemaIntegracao;
            cargaValePedagio.ProblemaIntegracao = mensagem;
            repCargaValePedagio.Atualizar(cargaValePedagio);

            servicoArquivoTransacao.Adicionar(cargaValePedagio, inspector.LastRequestXML, inspector.LastResponseXML, "xml");

            return 0;
        }

        private ServicoDBTrans.ValePedagio.ConsultarVeiculoSemPararRequest ObterConsultaVeiculoSemParar(ServicoDBTrans.ValePedagio.IdentificacaoIntegracaoType identificacaoIntegracao, Dominio.Entidades.Veiculo veiculo)
        {
            ServicoDBTrans.ValePedagio.ConsultarVeiculoSemPararRequest request = new ServicoDBTrans.ValePedagio.ConsultarVeiculoSemPararRequest()
            {
                IdentificacaoIntegracao = identificacaoIntegracao,
                PlacaVeiculo = veiculo.Placa
            };

            return request;
        }

        private ServicoDBTrans.ValePedagio.ConsultarTarifasRequest ObterConsultaTarifas(ServicoDBTrans.ValePedagio.IdentificacaoIntegracaoType identificacaoIntegracao, Dominio.Entidades.Embarcador.Cargas.ValePedagio.CargaConsultaValorPedagioIntegracao cargaConsultaValePedagio, int codigoRotaTemporaria)
        {
            ServicoDBTrans.ValePedagio.ConsultarTarifasRequest request = new ServicoDBTrans.ValePedagio.ConsultarTarifasRequest()
            {
                IdentificacaoIntegracao = identificacaoIntegracao,
                CodigoRota = codigoRotaTemporaria > 0 ? codigoRotaTemporaria : cargaConsultaValePedagio.CodigoIntegracaoValePedagio.ToInt(),
                CodigoRotaSpecified = true,
                QtdeEixos = (cargaConsultaValePedagio.Carga.Veiculo.ModeloVeicularCarga?.NumeroEixos ?? 0).ToString(),
                TipoRodagem = ObterTipoRodagem(cargaConsultaValePedagio.Carga),
                TipoRodagemSpecified = true
            };

            return request;
        }

        private bool CadastrarTransportador(ServicoDBTrans.ValePedagio.IdentificacaoIntegracaoType identificacaoIntegracao, Dominio.Entidades.Embarcador.Configuracoes.IntegracaoDBTrans integracaoDBTrans, Dominio.Entidades.Embarcador.Cargas.ValePedagio.CargaIntegracaoValePedagio cargaValePedagio)
        {
            if (!integracaoDBTrans.CadastrarTransportadorAntesDaCompra)
                return true;

            Repositorio.Embarcador.Cargas.ValePedagio.CargaIntegracaoValePedagio repCargaValePedagio = new Repositorio.Embarcador.Cargas.ValePedagio.CargaIntegracaoValePedagio(_unitOfWork);

            ArquivoTransacao<Dominio.Entidades.Embarcador.Cargas.ValePedagio.CargaValePedagioIntegracaoArquivo> servicoArquivoTransacao = new ArquivoTransacao<Dominio.Entidades.Embarcador.Cargas.ValePedagio.CargaValePedagioIntegracaoArquivo>(_unitOfWork);

            ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;
            ServicoDBTrans.ValePedagio.ValePedagioSoapClient valePedagioSoapClient = ObterClient(integracaoDBTrans.URL);
            InspectorBehavior inspector = new InspectorBehavior();
            valePedagioSoapClient.Endpoint.EndpointBehaviors.Add(inspector);

            string mensagem;

            try
            {
                ServicoDBTrans.ValePedagio.ManterTransportadorRequest manterTransportador = ObterManterTransportador(identificacaoIntegracao, cargaValePedagio.Carga.Empresa);
                ServicoDBTrans.ValePedagio.ManterTransportadorResponse retorno = valePedagioSoapClient.ManterTransportador(manterTransportador);

                ServicoDBTrans.ValePedagio.ManterTransportadorResponseTranportadorRetorno tranportadorRetorno = retorno.TranportadorRetorno[0];
                if (tranportadorRetorno.StatusRetorno == ServicoDBTrans.ValePedagio.StatusRetornoType.SUCESSO)
                    mensagem = "Cadastro de transportador efetuado com sucesso";
                else
                    mensagem = $"Cadastro de transportador: {tranportadorRetorno.Excecao.CodigoExcecao} - {tranportadorRetorno.Excecao.MensagemExcecao}";
                //throw new ServicoException(mensagem);//De momento é para ignorar o bloqueio na rejeição do cadastro

                cargaValePedagio.DataIntegracao = DateTime.Now;
                servicoArquivoTransacao.Adicionar(cargaValePedagio, inspector.LastRequestXML, inspector.LastResponseXML, "xml", mensagem);

                return true;
            }
            catch (ServicoException excecao)
            {
                mensagem = excecao.Message;
            }
            catch (Exception excecao)
            {
                Log.TratarErro(excecao);

                mensagem = "Ocorreu uma falha ao realizar o cadastro do transportador na DBTrans/Rodocred";
            }

            cargaValePedagio.DataIntegracao = DateTime.Now;
            cargaValePedagio.SituacaoIntegracao = SituacaoIntegracao.ProblemaIntegracao;
            cargaValePedagio.ProblemaIntegracao = mensagem;
            repCargaValePedagio.Atualizar(cargaValePedagio);

            servicoArquivoTransacao.Adicionar(cargaValePedagio, inspector.LastRequestXML, inspector.LastResponseXML, "xml");

            return false;
        }

        private ServicoDBTrans.ValePedagio.ManterTransportadorRequest ObterManterTransportador(ServicoDBTrans.ValePedagio.IdentificacaoIntegracaoType identificacaoIntegracao, Dominio.Entidades.Empresa empresa)
        {
            ServicoDBTrans.ValePedagio.ManterTransportadorRequest request = new ServicoDBTrans.ValePedagio.ManterTransportadorRequest()
            {
                IdentificacaoIntegracao = identificacaoIntegracao,
                Operacao = ServicoDBTrans.ValePedagio.TipoOperacaoType.INC,
                ListaTransportador = new ServicoDBTrans.ValePedagio.ManterTransportadorRequestListaTransportador
                {
                    DadosTransportador = ObterDadosTransportador(empresa)
                }
            };

            return request;
        }

        private ServicoDBTrans.ValePedagio.DadosTransportadorType[] ObterDadosTransportador(Dominio.Entidades.Empresa empresa)
        {
            List<ServicoDBTrans.ValePedagio.DadosTransportadorType> listaTransportador = new List<ServicoDBTrans.ValePedagio.DadosTransportadorType>();

            ServicoDBTrans.ValePedagio.DadosTransportadorType transportador = new ServicoDBTrans.ValePedagio.DadosTransportadorType()
            {
                Transportador = new ServicoDBTrans.ValePedagio.DadosTransportadorTypeTransportador
                {
                    CNPJCPFTransportador = empresa.CNPJ,
                    NomeTransportador = empresa.RazaoSocial,
                    RNTRC = empresa.RegistroANTT
                },
                TipoTransportador = ServicoDBTrans.ValePedagio.TipoTransportadorType.ETC,
                Contato = new ServicoDBTrans.ValePedagio.ContatoType
                {
                    TipoContato = ServicoDBTrans.ValePedagio.ContatoTypeTipoContato.C,
                    Nome = empresa.Contato,
                    Telefone = empresa.TelefoneContato.ObterSomenteNumeros(),
                    Email = empresa.Email,
                    MeioComunicacaoPreferido = ServicoDBTrans.ValePedagio.ContatoTypeMeioComunicacaoPreferido.Email,
                    MeioComunicacaoPreferidoSpecified = true
                },
                Endereco = new ServicoDBTrans.ValePedagio.EnderecoType
                {
                    TipoEndereco = ServicoDBTrans.ValePedagio.EnderecoTypeTipoEndereco.C,
                    TipoEnderecoSpecified = true,
                    CEP = empresa.CEP,
                    TipoLogradouro = ServicoDBTrans.ValePedagio.EnderecoTypeTipoLogradouro.Rua,
                    TipoLogradouroSpecified = true,
                    Logradouro = empresa.Endereco,
                    Numero = empresa.Numero,
                    Complemento = empresa.Complemento,
                    Bairro = empresa.Bairro,
                    Cidade = empresa.Localidade.Descricao,
                    Estado = empresa.Localidade.Estado.Sigla.ToEnum<ServicoDBTrans.ValePedagio.Estado>(),
                    EstadoSpecified = true
                },
                TransportadorPJ = new ServicoDBTrans.ValePedagio.TransportadorPJType
                {
                    InscricaoEstadual = empresa.InscricaoEstadual,
                    InscricaoMunicipal = empresa.InscricaoMunicipal
                }
            };

            listaTransportador.Add(transportador);

            return listaTransportador.ToArray();
        }

        private bool CadastrarDocumentoTransportador(ServicoDBTrans.ValePedagio.IdentificacaoIntegracaoType identificacaoIntegracao, Dominio.Entidades.Embarcador.Configuracoes.IntegracaoDBTrans integracaoDBTrans, Dominio.Entidades.Embarcador.Cargas.ValePedagio.CargaIntegracaoValePedagio cargaValePedagio)
        {
            if (!integracaoDBTrans.CadastrarDocumentoTransportadorAntesDaCompra)
                return true;

            Repositorio.Embarcador.Cargas.ValePedagio.CargaIntegracaoValePedagio repCargaValePedagio = new Repositorio.Embarcador.Cargas.ValePedagio.CargaIntegracaoValePedagio(_unitOfWork);

            ArquivoTransacao<Dominio.Entidades.Embarcador.Cargas.ValePedagio.CargaValePedagioIntegracaoArquivo> servicoArquivoTransacao = new ArquivoTransacao<Dominio.Entidades.Embarcador.Cargas.ValePedagio.CargaValePedagioIntegracaoArquivo>(_unitOfWork);

            ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;
            ServicoDBTrans.ValePedagio.ValePedagioSoapClient valePedagioSoapClient = ObterClient(integracaoDBTrans.URL);
            InspectorBehavior inspector = new InspectorBehavior();
            valePedagioSoapClient.Endpoint.EndpointBehaviors.Add(inspector);

            string mensagem;

            try
            {
                ServicoDBTrans.ValePedagio.ManterDocumentoTransportadorRequest manterDocumentoTransportador = ObterManterDocumentoTransportador(identificacaoIntegracao, cargaValePedagio.Carga);
                ServicoDBTrans.ValePedagio.ManterDocumentoTransportadorResponse retorno = valePedagioSoapClient.ManterDocumentoTransportador(manterDocumentoTransportador);

                ServicoDBTrans.ValePedagio.ManterDocumentoTransportadorResponseRetornoMensagem documentoTranportadorRetorno = retorno.RetornoMensagem;

                if (documentoTranportadorRetorno.StatusRetorno == ServicoDBTrans.ValePedagio.ManterDocumentoTransportadorResponseRetornoMensagemStatusRetorno.SUCESSO)
                    mensagem = "Cadastro de documento transportador efetuado com sucesso";
                else
                    mensagem = $"Cadastro de documento transportador: {documentoTranportadorRetorno.Excecao.CodigoExcecao} - {documentoTranportadorRetorno.Excecao.MensagemExcecao}";

                cargaValePedagio.DataIntegracao = DateTime.Now;
                servicoArquivoTransacao.Adicionar(cargaValePedagio, inspector.LastRequestXML, inspector.LastResponseXML, "xml", mensagem);

                return true;
            }
            catch (ServicoException excecao)
            {
                mensagem = excecao.Message;
            }
            catch (Exception excecao)
            {
                Log.TratarErro(excecao);

                mensagem = "Ocorreu uma falha ao realizar o cadastro do documento transportador na DBTrans/Rodocred";
            }

            cargaValePedagio.DataIntegracao = DateTime.Now;
            cargaValePedagio.SituacaoIntegracao = SituacaoIntegracao.ProblemaIntegracao;
            cargaValePedagio.ProblemaIntegracao = mensagem;
            repCargaValePedagio.Atualizar(cargaValePedagio);

            servicoArquivoTransacao.Adicionar(cargaValePedagio, inspector.LastRequestXML, inspector.LastResponseXML, "xml");

            return false;
        }

        private ServicoDBTrans.ValePedagio.ManterDocumentoTransportadorRequest ObterManterDocumentoTransportador(ServicoDBTrans.ValePedagio.IdentificacaoIntegracaoType identificacaoIntegracao, Dominio.Entidades.Embarcador.Cargas.Carga carga)
        {
            string cnpjCPFTransportador = (carga.Veiculo.Tipo == "T" && carga.Veiculo.Proprietario != null) ? carga.Veiculo.Proprietario.CPF_CNPJ_SemFormato : carga.Empresa?.CNPJ_SemFormato;

            ServicoDBTrans.ValePedagio.ManterDocumentoTransportadorRequest request = new ServicoDBTrans.ValePedagio.ManterDocumentoTransportadorRequest()
            {
                IdentificacaoIntegracao = identificacaoIntegracao,
                Operacao = ServicoDBTrans.ValePedagio.TipoOperacaoType.INC,
                PlacaVeiculo = carga.Veiculo.Placa,
                CNPJCPFTransportador = cnpjCPFTransportador,
            };

            return request;
        }

        private bool CadastrarMotorista(ServicoDBTrans.ValePedagio.IdentificacaoIntegracaoType identificacaoIntegracao, Dominio.Entidades.Embarcador.Configuracoes.IntegracaoDBTrans integracaoDBTrans, Dominio.Entidades.Embarcador.Cargas.ValePedagio.CargaIntegracaoValePedagio cargaValePedagio)
        {
            if (!integracaoDBTrans.CadastrarMotoristaAntesDaCompra)
                return true;

            Repositorio.Embarcador.Cargas.ValePedagio.CargaIntegracaoValePedagio repCargaValePedagio = new Repositorio.Embarcador.Cargas.ValePedagio.CargaIntegracaoValePedagio(_unitOfWork);

            ArquivoTransacao<Dominio.Entidades.Embarcador.Cargas.ValePedagio.CargaValePedagioIntegracaoArquivo> servicoArquivoTransacao = new ArquivoTransacao<Dominio.Entidades.Embarcador.Cargas.ValePedagio.CargaValePedagioIntegracaoArquivo>(_unitOfWork);

            ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;
            ServicoDBTrans.ValePedagio.ValePedagioSoapClient valePedagioSoapClient = ObterClient(integracaoDBTrans.URL);
            InspectorBehavior inspector = new InspectorBehavior();
            valePedagioSoapClient.Endpoint.EndpointBehaviors.Add(inspector);

            string mensagem;

            try
            {
                ServicoDBTrans.ValePedagio.ManterMotoristaRequest manterMotorista = ObterManterMotorista(identificacaoIntegracao, cargaValePedagio.Carga.Empresa, cargaValePedagio.Carga);
                ServicoDBTrans.ValePedagio.ManterMotoristaResponse retorno = valePedagioSoapClient.ManterMotorista(manterMotorista);

                ServicoDBTrans.ValePedagio.ManterMotoristaResponseMotoristas motoristaRetorno = retorno.Motoristas[0];
                if (motoristaRetorno.StatusRetorno == ServicoDBTrans.ValePedagio.StatusRetornoType.SUCESSO)
                    mensagem = "Cadastro de motorista efetuado com sucesso";
                else
                    mensagem = $"Cadastro de motorista: {motoristaRetorno.Excecao.CodigoExcecao} - {motoristaRetorno.Excecao.MensagemExcecao}";
                //throw new ServicoException(mensagem);//De momento é para ignorar o bloqueio na rejeição do cadastro

                cargaValePedagio.DataIntegracao = DateTime.Now;
                servicoArquivoTransacao.Adicionar(cargaValePedagio, inspector.LastRequestXML, inspector.LastResponseXML, "xml", mensagem);

                return true;
            }
            catch (ServicoException excecao)
            {
                mensagem = excecao.Message;
            }
            catch (Exception excecao)
            {
                Log.TratarErro(excecao);

                mensagem = "Ocorreu uma falha ao realizar o cadastro do motorista na DBTrans/Rodocred";
            }

            cargaValePedagio.DataIntegracao = DateTime.Now;
            cargaValePedagio.SituacaoIntegracao = SituacaoIntegracao.ProblemaIntegracao;
            cargaValePedagio.ProblemaIntegracao = mensagem;
            repCargaValePedagio.Atualizar(cargaValePedagio);

            servicoArquivoTransacao.Adicionar(cargaValePedagio, inspector.LastRequestXML, inspector.LastResponseXML, "xml");

            return false;
        }

        private ServicoDBTrans.ValePedagio.ManterMotoristaRequest ObterManterMotorista(ServicoDBTrans.ValePedagio.IdentificacaoIntegracaoType identificacaoIntegracao, Dominio.Entidades.Empresa empresa, Dominio.Entidades.Embarcador.Cargas.Carga carga)
        {
            ServicoDBTrans.ValePedagio.ManterMotoristaRequest request = new ServicoDBTrans.ValePedagio.ManterMotoristaRequest
            {
                IdentificacaoIntegracao = identificacaoIntegracao,
                Operacao = ServicoDBTrans.ValePedagio.TipoOperacaoType.INC,
                //Transportador = new ServicoDBTrans.ValePedagio.ManterMotoristaRequestTransportador
                //{
                //    CNPJCPFTransportador = empresa.CNPJ,
                //    NomeTransportador = empresa.RazaoSocial,
                //    RNTRC = empresa.RegistroANTT
                //},
                Motorista = ObterDadosMotoristas(carga)
            };

            return request;
        }

        private ServicoDBTrans.ValePedagio.MotoristaType[] ObterDadosMotoristas(Dominio.Entidades.Embarcador.Cargas.Carga carga)
        {
            List<ServicoDBTrans.ValePedagio.MotoristaType> motoristas = new List<ServicoDBTrans.ValePedagio.MotoristaType>();

            if (carga.Motoristas?.Count > 0)
            {
                foreach (Dominio.Entidades.Usuario motorista in carga.Motoristas)
                {
                    ServicoDBTrans.ValePedagio.MotoristaType motoristaType = new ServicoDBTrans.ValePedagio.MotoristaType()
                    {
                        CPFMotorista = motorista.CPF,
                        NomeMotorista = motorista.Nome
                    };

                    motoristas.Add(motoristaType);
                }
            }

            return motoristas.ToArray();
        }

        private bool CadastrarVeiculo(ServicoDBTrans.ValePedagio.IdentificacaoIntegracaoType identificacaoIntegracao, Dominio.Entidades.Embarcador.Configuracoes.IntegracaoDBTrans integracaoDBTrans, Dominio.Entidades.Embarcador.Cargas.ValePedagio.CargaIntegracaoValePedagio cargaValePedagio)
        {
            if (!integracaoDBTrans.CadastrarVeiculoAntesDaCompra)
                return true;

            Repositorio.Embarcador.Cargas.ValePedagio.CargaIntegracaoValePedagio repCargaValePedagio = new Repositorio.Embarcador.Cargas.ValePedagio.CargaIntegracaoValePedagio(_unitOfWork);

            ArquivoTransacao<Dominio.Entidades.Embarcador.Cargas.ValePedagio.CargaValePedagioIntegracaoArquivo> servicoArquivoTransacao = new ArquivoTransacao<Dominio.Entidades.Embarcador.Cargas.ValePedagio.CargaValePedagioIntegracaoArquivo>(_unitOfWork);

            ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;
            ServicoDBTrans.ValePedagio.ValePedagioSoapClient valePedagioSoapClient = ObterClient(integracaoDBTrans.URL);
            InspectorBehavior inspector = new InspectorBehavior();
            valePedagioSoapClient.Endpoint.EndpointBehaviors.Add(inspector);

            string mensagem;

            try
            {
                ServicoDBTrans.ValePedagio.ManterVeiculoRequest manterVeiculo = ObterManterVeiculo(identificacaoIntegracao, cargaValePedagio.Carga.Empresa, cargaValePedagio.Carga);
                ServicoDBTrans.ValePedagio.ManterVeiculoResponse retorno = valePedagioSoapClient.ManterVeiculo(manterVeiculo);

                ServicoDBTrans.ValePedagio.ManterVeiculoResponseVeiculos veiculoRetorno = retorno.Veiculos[0];
                if (veiculoRetorno.StatusRetorno == ServicoDBTrans.ValePedagio.StatusRetornoType.SUCESSO)
                    mensagem = "Cadastro de veículo efetuado com sucesso";
                else
                    mensagem = $"Cadastro de veículo: {veiculoRetorno.Excecao.CodigoExcecao} - {veiculoRetorno.Excecao.MensagemExcecao}";
                //throw new ServicoException(mensagem);//De momento é para ignorar o bloqueio na rejeição do cadastro

                cargaValePedagio.DataIntegracao = DateTime.Now;
                servicoArquivoTransacao.Adicionar(cargaValePedagio, inspector.LastRequestXML, inspector.LastResponseXML, "xml", mensagem);

                return true;
            }
            catch (ServicoException excecao)
            {
                mensagem = excecao.Message;
            }
            catch (Exception excecao)
            {
                Log.TratarErro(excecao);

                mensagem = "Ocorreu uma falha ao realizar o cadastro do veículo na DBTrans/Rodocred";
            }

            cargaValePedagio.DataIntegracao = DateTime.Now;
            cargaValePedagio.SituacaoIntegracao = SituacaoIntegracao.ProblemaIntegracao;
            cargaValePedagio.ProblemaIntegracao = mensagem;
            repCargaValePedagio.Atualizar(cargaValePedagio);

            servicoArquivoTransacao.Adicionar(cargaValePedagio, inspector.LastRequestXML, inspector.LastResponseXML, "xml");

            return false;
        }

        private ServicoDBTrans.ValePedagio.ManterVeiculoRequest ObterManterVeiculo(ServicoDBTrans.ValePedagio.IdentificacaoIntegracaoType identificacaoIntegracao, Dominio.Entidades.Empresa empresa, Dominio.Entidades.Embarcador.Cargas.Carga carga)
        {
            ServicoDBTrans.ValePedagio.ManterVeiculoRequest request = new ServicoDBTrans.ValePedagio.ManterVeiculoRequest
            {
                IdentificacaoIntegracao = identificacaoIntegracao,
                Operacao = ServicoDBTrans.ValePedagio.TipoOperacaoType.INC,
                Transportador = new ServicoDBTrans.ValePedagio.ManterVeiculoRequestTransportador
                {
                    CNPJCPFTransportador = empresa.CNPJ,
                    NomeTransportador = empresa.RazaoSocial,
                    RNTRC = empresa.RegistroANTT
                },
                ListaVeiculos = new ServicoDBTrans.ValePedagio.ManterVeiculoRequestListaVeiculos
                {
                    Veiculo = ObterDadosVeiculos(carga),
                    ValidaFrotaANTT = true
                }
            };

            return request;
        }

        private ServicoDBTrans.ValePedagio.VeiculoType[] ObterDadosVeiculos(Dominio.Entidades.Embarcador.Cargas.Carga carga)
        {
            List<ServicoDBTrans.ValePedagio.VeiculoType> veiculos = new List<ServicoDBTrans.ValePedagio.VeiculoType>();

            if (carga.Veiculo != null)
            {
                Dominio.Entidades.Veiculo veiculo = carga.Veiculo;
                ServicoDBTrans.ValePedagio.VeiculoType veiculoType = new ServicoDBTrans.ValePedagio.VeiculoType()
                {
                    TipoTransporteVeiculo = veiculo.Tipo.Equals("T") ? ServicoDBTrans.ValePedagio.VeiculoTypeTipoTransporteVeiculo.T : ServicoDBTrans.ValePedagio.VeiculoTypeTipoTransporteVeiculo.P,
                    TipoVeiculo = ObterTipoVeiculo(veiculo.ModeloVeicularCarga),
                    QtdeEixos = (veiculo.ModeloVeicularCarga?.NumeroEixos ?? 0).ToString(),
                    TipoRodagem = ObterTipoRodagem(carga),
                    PlacaVeiculo = veiculo.Placa,
                    UfPlacaVeiculo = veiculo.Estado.Sigla.ToEnum<ServicoDBTrans.ValePedagio.Estado>(),
                    TipoCombustivelVeiculo = 1
                };

                veiculos.Add(veiculoType);
            }

            if (carga.VeiculosVinculados?.Count > 0)
            {
                foreach (Dominio.Entidades.Veiculo reboque in carga.VeiculosVinculados)
                {
                    ServicoDBTrans.ValePedagio.VeiculoType veiculo = new ServicoDBTrans.ValePedagio.VeiculoType()
                    {
                        TipoTransporteVeiculo = reboque.Tipo.Equals("T") ? ServicoDBTrans.ValePedagio.VeiculoTypeTipoTransporteVeiculo.T : ServicoDBTrans.ValePedagio.VeiculoTypeTipoTransporteVeiculo.P,
                        TipoVeiculo = ObterTipoVeiculo(reboque.ModeloVeicularCarga),
                        QtdeEixos = (reboque.ModeloVeicularCarga?.NumeroEixos ?? 0).ToString(),
                        TipoRodagem = ServicoDBTrans.ValePedagio.TipoRodagemType.D,
                        PlacaVeiculo = reboque.Placa,
                        UfPlacaVeiculo = reboque.Estado.Sigla.ToEnum<ServicoDBTrans.ValePedagio.Estado>(),
                        TipoCombustivelVeiculo = 1
                    };

                    veiculos.Add(veiculo);
                }
            }

            return veiculos.ToArray();
        }

        private ServicoDBTrans.ValePedagio.ConsultarIdVpoRequest ObterConsultaIdVpo(ServicoDBTrans.ValePedagio.IdentificacaoIntegracaoType identificacaoIntegracao, Dominio.Entidades.Embarcador.Cargas.ValePedagio.CargaIntegracaoValePedagio cargaValePedagio)
        {
            ServicoDBTrans.ValePedagio.ConsultarIdVpoRequest request = new ServicoDBTrans.ValePedagio.ConsultarIdVpoRequest()
            {
                IdentificacaoIntegracao = identificacaoIntegracao,
                NumeroViagem = cargaValePedagio.NumeroValePedagio
            };

            return request;
        }

        private void DetalharRota(int codigoRotaDaCompra, ServicoDBTrans.ValePedagio.IdentificacaoIntegracaoType identificacaoIntegracao, Dominio.Entidades.Embarcador.Configuracoes.IntegracaoDBTrans integracaoDBTrans, Dominio.Entidades.Embarcador.Cargas.ValePedagio.CargaIntegracaoValePedagio cargaValePedagio)
        {
            Repositorio.Embarcador.Cargas.ValePedagio.CargaIntegracaoValePedagio repCargaValePedagio = new Repositorio.Embarcador.Cargas.ValePedagio.CargaIntegracaoValePedagio(_unitOfWork);

            ArquivoTransacao<Dominio.Entidades.Embarcador.Cargas.ValePedagio.CargaValePedagioIntegracaoArquivo> servicoArquivoTransacao = new ArquivoTransacao<Dominio.Entidades.Embarcador.Cargas.ValePedagio.CargaValePedagioIntegracaoArquivo>(_unitOfWork);

            ServicoDBTrans.ValePedagio.ValePedagioSoapClient valePedagioSoapClient = ObterClient(integracaoDBTrans.URL);
            InspectorBehavior inspector = new InspectorBehavior();
            valePedagioSoapClient.Endpoint.EndpointBehaviors.Add(inspector);

            try
            {
                ServicoDBTrans.ValePedagio.DetalharRotaRequest detalharRota = ObterDetalhamentoRota(identificacaoIntegracao, codigoRotaDaCompra);
                ServicoDBTrans.ValePedagio.DetalharRotaResponse retorno = valePedagioSoapClient.DetalharRota(detalharRota);

                if (retorno.RetornoMensagem.StatusRetorno == ServicoDBTrans.ValePedagio.DetalharRotaResponseRetornoMensagemStatusRetorno.SUCESSO)
                {
                    if (retorno.Rota.ListaPracasRota.Length == 0)
                    {
                        cargaValePedagio.SituacaoValePedagio = SituacaoValePedagio.RotaSemCusto;
                        cargaValePedagio.SituacaoIntegracao = SituacaoIntegracao.Integrado;
                        cargaValePedagio.ProblemaIntegracao = "Rota sem valor de Vale pedágio";
                    }
                    else
                        throw new ServicoException("Rota possui praças de pedágio! Operação precisa validar o cadastro do veículo, pois o VP não pode ser comprado para a rota para aquele veículo.");
                }
                else
                    throw new ServicoException(retorno.RetornoMensagem.Excecao.CodigoExcecao + " - " + retorno.RetornoMensagem.Excecao.MensagemExcecao);
            }
            catch (ServicoException excecao)
            {
                cargaValePedagio.SituacaoIntegracao = SituacaoIntegracao.ProblemaIntegracao;
                cargaValePedagio.ProblemaIntegracao = excecao.Message;
            }
            catch (Exception excecao)
            {
                Log.TratarErro(excecao);

                cargaValePedagio.SituacaoIntegracao = SituacaoIntegracao.ProblemaIntegracao;
                cargaValePedagio.ProblemaIntegracao = "Ocorreu uma falha ao obter detalhes da rota da DBTrans/Rodocred";
            }

            cargaValePedagio.DataIntegracao = DateTime.Now;
            repCargaValePedagio.Atualizar(cargaValePedagio);

            servicoArquivoTransacao.Adicionar(cargaValePedagio, inspector.LastRequestXML, inspector.LastResponseXML, "xml");
        }

        private ServicoDBTrans.ValePedagio.DetalharRotaRequest ObterDetalhamentoRota(ServicoDBTrans.ValePedagio.IdentificacaoIntegracaoType identificacaoIntegracao, int codigoRotaDaCompra)
        {
            return new ServicoDBTrans.ValePedagio.DetalharRotaRequest
            {
                IdentificacaoIntegracao = identificacaoIntegracao,
                CodigoRota = codigoRotaDaCompra
            };
        }

        private int ObterNumeroDeEixos(Dominio.Entidades.Veiculo veiculo, bool temEixoSuspenso)
        {
            if (veiculo == null)
                return 0;

            int numeroEixos = veiculo.ModeloVeicularCarga?.NumeroEixos ?? 0;
            if (temEixoSuspenso)
                numeroEixos -= veiculo.ModeloVeicularCarga?.NumeroEixosSuspensos ?? 0;

            return numeroEixos;
        }

        private void ObterLocalidadesValePedagioCarga(Dominio.Entidades.Embarcador.Cargas.Carga carga, out Dominio.Entidades.Localidade localidadeOrigem, out Dominio.Entidades.Localidade localidadeDestino, out List<Dominio.Entidades.Localidade> pontosPassagem, Dominio.Entidades.RotaFrete rotaFrete, TipoRotaFrete? tipoRotaFrete)
        {
            Repositorio.Embarcador.Cargas.CargaRotaFrete repCargaRotaFrete = new Repositorio.Embarcador.Cargas.CargaRotaFrete(_unitOfWork);
            Repositorio.Embarcador.Cargas.CargaRotaFretePontosPassagem repCargaRotaFretePontosPassagem = new Repositorio.Embarcador.Cargas.CargaRotaFretePontosPassagem(_unitOfWork);
            Servicos.Embarcador.Localidades.Localidade servicoLocalidade = new Servicos.Embarcador.Localidades.Localidade(_unitOfWork);
            localidadeOrigem = null;
            localidadeDestino = null;
            pontosPassagem = new List<Dominio.Entidades.Localidade>();

            Dominio.Entidades.Embarcador.Cargas.CargaRotaFrete cargaRotaFrete = repCargaRotaFrete.BuscarPorCarga(carga.Codigo);
            List<Dominio.Entidades.Embarcador.Cargas.CargaRotaFretePontosPassagem> todosPontosRota = null;
            List<Dominio.Entidades.Embarcador.Cargas.CargaRotaFretePontosPassagem> pontosRota = new List<Dominio.Entidades.Embarcador.Cargas.CargaRotaFretePontosPassagem>();

            if (cargaRotaFrete != null)
                todosPontosRota = repCargaRotaFretePontosPassagem.BuscarPorCargaRotaFrete(cargaRotaFrete.Codigo);

            if (todosPontosRota == null || todosPontosRota.Count == 0)
                throw new ServicoException("Não foi possível definir os pontos da rota.");

            if (tipoRotaFrete == TipoRotaFrete.Ida)
            {
                localidadeOrigem = servicoLocalidade.ObterLocalidade(todosPontosRota.FirstOrDefault());
                localidadeDestino = servicoLocalidade.ObterLocalidade(todosPontosRota.LastOrDefault(pontoRota => pontoRota.TipoPontoPassagem == TipoPontoPassagem.Entrega));

                int indexUltimaEntrega = todosPontosRota.FindLastIndex(pontoRota => pontoRota.TipoPontoPassagem == TipoPontoPassagem.Entrega);
                pontosRota = todosPontosRota.GetRange(0, indexUltimaEntrega + 1);
            }
            else if (tipoRotaFrete == TipoRotaFrete.IdaVolta)
            {
                localidadeOrigem = servicoLocalidade.ObterLocalidade(todosPontosRota.FirstOrDefault());
                localidadeDestino = servicoLocalidade.ObterLocalidade(todosPontosRota.LastOrDefault());
                pontosRota = todosPontosRota;

                TipoUltimoPontoRoteirizacao tipoUltimoPontoRoteirizacao = carga.TipoOperacao?.TipoUltimoPontoRoteirizacao ?? TipoUltimoPontoRoteirizacao.PontoMaisDistante;
                if (tipoUltimoPontoRoteirizacao == TipoUltimoPontoRoteirizacao.PontoMaisDistante)
                {
                    localidadeDestino = localidadeOrigem;
                    pontosRota.AddRange(ObterPontosPassagemInversoSemUltimoPonto(todosPontosRota));
                }
            }
            else
            {
                localidadeOrigem = servicoLocalidade.ObterLocalidade(todosPontosRota.LastOrDefault(pontoRota => pontoRota.TipoPontoPassagem == TipoPontoPassagem.Entrega));
                localidadeDestino = servicoLocalidade.ObterLocalidade(todosPontosRota.LastOrDefault());

                if (rotaFrete?.VoltarPeloMesmoCaminhoIda ?? false)
                    pontosRota = ObterPontosPassagemInversoSemUltimoPonto(todosPontosRota);

                if (localidadeOrigem == localidadeDestino)
                {
                    localidadeDestino = servicoLocalidade.ObterLocalidade(todosPontosRota.FirstOrDefault());

                    if (pontosRota.Count == 0)
                        pontosRota = ObterPontosPassagemInversoSemUltimoPonto(todosPontosRota);
                }
            }

            if (localidadeOrigem == null)
                throw new ServicoException("Não foi possível definir a localidade origem da rota.");

            if (localidadeDestino == null)
                throw new ServicoException("Não foi possível definir a localidade destino da rota.");

            for (int i = 0; i < pontosRota.Count; i++)
            {
                if (i > 0 && i < pontosRota.Count - 1) //Não envia o primeiro e o ultimo ponto                
                {
                    Dominio.Entidades.Localidade localidade = servicoLocalidade.ObterLocalidade(pontosRota[i]);
                    Dominio.Entidades.Localidade localidadeAnterior = servicoLocalidade.ObterLocalidade(pontosRota[i - 1]);

                    if (localidade != null && (localidadeAnterior == null || localidade.Codigo != localidadeAnterior.Codigo) && localidade.Codigo != localidadeOrigem.Codigo && localidade.Codigo != localidadeDestino.Codigo)
                        pontosPassagem.Add(localidade);
                }
            }
        }

        private List<Dominio.Entidades.Embarcador.Cargas.CargaRotaFretePontosPassagem> ObterPontosPassagemInversoSemUltimoPonto(List<Dominio.Entidades.Embarcador.Cargas.CargaRotaFretePontosPassagem> pontosRota)
        {
            return pontosRota.GetRange(0, pontosRota.Count - 1).OrderByDescending(pontoRota => pontoRota.Ordem).ToList();
        }

        private int ObterCodigoRotaTemporaria(ServicoDBTrans.ValePedagio.IdentificacaoIntegracaoType identificacaoIntegracao, Dominio.Entidades.Embarcador.Configuracoes.IntegracaoDBTrans integracaoDBTrans, Dominio.Entidades.Embarcador.Cargas.ValePedagio.CargaIntegracaoValePedagio cargaValePedagio, out bool problemaRotaTemporaria)
        {
            problemaRotaTemporaria = false;
            Dominio.Entidades.RotaFrete rotaFrete = cargaValePedagio.RotaFrete;
            if (integracaoDBTrans.TipoRota == TipoRotaDBTrans.RotaTemporaria && string.IsNullOrWhiteSpace(rotaFrete?.CodigoIntegracaoValePedagio) && string.IsNullOrWhiteSpace(rotaFrete?.CodigoIntegracaoValePedagioRetorno))
            {
                int codigoRotaTemporaria = ObterRotaTemporaria(identificacaoIntegracao, integracaoDBTrans, cargaValePedagio);

                if (codigoRotaTemporaria > 0)
                    return codigoRotaTemporaria;

                problemaRotaTemporaria = true;
                return 0;
            }

            if (!cargaValePedagio.CompraComEixosSuspensos && !string.IsNullOrWhiteSpace(rotaFrete?.CodigoIntegracaoValePedagio))
                return rotaFrete.CodigoIntegracaoValePedagio.ToInt();

            if (cargaValePedagio.CompraComEixosSuspensos && !string.IsNullOrWhiteSpace(rotaFrete?.CodigoIntegracaoValePedagioRetorno))
                return rotaFrete.CodigoIntegracaoValePedagioRetorno.ToInt();

            return 0;
        }

        private int ObterMeioPagamento(ServicoDBTrans.ValePedagio.IdentificacaoIntegracaoType identificacaoIntegracao, Dominio.Entidades.Embarcador.Configuracoes.IntegracaoDBTrans integracaoDBTrans, Dominio.Entidades.Embarcador.Cargas.ValePedagio.CargaIntegracaoValePedagio cargaValePedagio, Dominio.Entidades.Embarcador.Cargas.Carga carga, out bool problemaMeioPagamento)
        {
            problemaMeioPagamento = false;
            int meioPagamento = (int)integracaoDBTrans.MeioPagamento;

            if (integracaoDBTrans.VerificarVeiculoCompraPorTag)
            {
                meioPagamento = ConsultarVeiculoSemParar(identificacaoIntegracao, integracaoDBTrans, cargaValePedagio);
                if (meioPagamento > 0)
                    return meioPagamento;

                problemaMeioPagamento = true;
                return 0;
            }

            if (integracaoDBTrans.MeioPagamento == MeioPagamentoDBTrans.CartaoVVP || integracaoDBTrans.MeioPagamento == MeioPagamentoDBTrans.ValePedagioAutoExpresso)
            {
                if (carga.Veiculo?.PossuiTagValePedagio ?? false)
                    return (int)MeioPagamentoDBTrans.ValePedagioAutoExpresso;

                return (int)MeioPagamentoDBTrans.CartaoVVP;
            }

            if (integracaoDBTrans.MeioPagamento == MeioPagamentoDBTrans.Cupom || integracaoDBTrans.MeioPagamento == MeioPagamentoDBTrans.ValePedagioAutoExpresso)
            {
                if (carga.Veiculo?.PossuiTagValePedagio ?? false)
                    return (int)MeioPagamentoDBTrans.ValePedagioAutoExpresso;

                return (int)MeioPagamentoDBTrans.Cupom;
            }

            return meioPagamento;
        }

        #endregion Métodos Privados

        #region Métodos Privados - Configurações

        private ServicoDBTrans.ValePedagio.IdentificacaoIntegracaoType ObterAutenticacao(Dominio.Entidades.Embarcador.Configuracoes.IntegracaoDBTrans integracaoDBTrans, Dominio.Entidades.Embarcador.Cargas.ValePedagio.CargaIntegracaoValePedagio cargaValePedagio)
        {
            Repositorio.Embarcador.Cargas.ValePedagio.CargaIntegracaoValePedagio repCargaValePedagio = new Repositorio.Embarcador.Cargas.ValePedagio.CargaIntegracaoValePedagio(_unitOfWork);
            ArquivoTransacao<Dominio.Entidades.Embarcador.Cargas.ValePedagio.CargaValePedagioIntegracaoArquivo> servicoArquivoTransacao = new ArquivoTransacao<Dominio.Entidades.Embarcador.Cargas.ValePedagio.CargaValePedagioIntegracaoArquivo>(_unitOfWork);

            System.Net.ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;
            ServicoDBTrans.ValePedagio.ValePedagioSoapClient valePedagioSoapClient = ObterClient(integracaoDBTrans.URL);
            InspectorBehavior inspector = new InspectorBehavior();
            valePedagioSoapClient.Endpoint.EndpointBehaviors.Add(inspector);

            string mensagem;

            try
            {
                ServicoDBTrans.ValePedagio.AutenticarClienteRequest request = new ServicoDBTrans.ValePedagio.AutenticarClienteRequest
                {
                    IdClienteRodocred = integracaoDBTrans.CodigoCliente.ToInt(),
                    LoginIntegracao = integracaoDBTrans.Usuario,
                    ChaveAutenticacao = integracaoDBTrans.Senha
                };

                ServicoDBTrans.ValePedagio.AutenticarClienteResponse retorno = valePedagioSoapClient.AutenticarCliente(request);

                if (retorno.RetornoMensagem.StatusRetorno == ServicoDBTrans.ValePedagio.AutenticarClienteResponseRetornoMensagemStatusRetorno.SUCESSO)
                    return retorno.RetornoMensagem.IdentificacaoIntegracao;
                else
                    mensagem = retorno.RetornoMensagem.Excecao.CodigoExcecao + " - " + retorno.RetornoMensagem.Excecao.MensagemExcecao;
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);
                mensagem = "Ocorreu uma falhar ao autenticar na integração com a DBTrans/Rodocred.";
            }

            cargaValePedagio.ProblemaIntegracao = mensagem;
            cargaValePedagio.SituacaoIntegracao = SituacaoIntegracao.ProblemaIntegracao;
            cargaValePedagio.DataIntegracao = DateTime.Now;
            cargaValePedagio.NumeroTentativas++;
            repCargaValePedagio.Atualizar(cargaValePedagio);

            servicoArquivoTransacao.Adicionar(cargaValePedagio, inspector.LastRequestXML, inspector.LastResponseXML, "xml");

            return null;
        }

        private ServicoDBTrans.ValePedagio.IdentificacaoIntegracaoType ObterAutenticacao(Dominio.Entidades.Embarcador.Configuracoes.IntegracaoDBTrans integracaoDBTrans, Dominio.Entidades.Embarcador.Cargas.ValePedagio.CargaConsultaValorPedagioIntegracao cargaConsultaValePedagio)
        {
            Repositorio.Embarcador.Cargas.ValePedagio.CargaConsultaValorPedagioIntegracao repCargaConsultaValorPedagio = new Repositorio.Embarcador.Cargas.ValePedagio.CargaConsultaValorPedagioIntegracao(_unitOfWork);
            ArquivoTransacao<Dominio.Entidades.Embarcador.Cargas.ValePedagio.CargaValePedagioIntegracaoArquivo> servicoArquivoTransacao = new ArquivoTransacao<Dominio.Entidades.Embarcador.Cargas.ValePedagio.CargaValePedagioIntegracaoArquivo>(_unitOfWork);

            System.Net.ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;
            ServicoDBTrans.ValePedagio.ValePedagioSoapClient valePedagioSoapClient = ObterClient(integracaoDBTrans.URL);
            InspectorBehavior inspector = new InspectorBehavior();
            valePedagioSoapClient.Endpoint.EndpointBehaviors.Add(inspector);

            string mensagem;

            try
            {
                ServicoDBTrans.ValePedagio.AutenticarClienteRequest request = new ServicoDBTrans.ValePedagio.AutenticarClienteRequest
                {
                    IdClienteRodocred = integracaoDBTrans.CodigoCliente.ToInt(),
                    LoginIntegracao = integracaoDBTrans.Usuario,
                    ChaveAutenticacao = integracaoDBTrans.Senha
                };

                ServicoDBTrans.ValePedagio.AutenticarClienteResponse retorno = valePedagioSoapClient.AutenticarCliente(request);

                if (retorno.RetornoMensagem.StatusRetorno == ServicoDBTrans.ValePedagio.AutenticarClienteResponseRetornoMensagemStatusRetorno.SUCESSO)
                    return retorno.RetornoMensagem.IdentificacaoIntegracao;
                else
                    mensagem = retorno.RetornoMensagem.Excecao.CodigoExcecao + " - " + retorno.RetornoMensagem.Excecao.MensagemExcecao;
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);
                mensagem = "Ocorreu uma falhar ao autenticar na integração com a DBTrans/Rodocred.";
            }

            cargaConsultaValePedagio.ProblemaIntegracao = mensagem;
            cargaConsultaValePedagio.SituacaoIntegracao = SituacaoIntegracao.ProblemaIntegracao;
            cargaConsultaValePedagio.DataIntegracao = DateTime.Now;
            cargaConsultaValePedagio.NumeroTentativas++;
            repCargaConsultaValorPedagio.Atualizar(cargaConsultaValePedagio);

            servicoArquivoTransacao.Adicionar(cargaConsultaValePedagio, inspector.LastRequestXML, inspector.LastResponseXML, "xml");

            return null;
        }

        private ServicoDBTrans.ValePedagio.ValePedagioSoapClient ObterClient(string url)
        {
            System.ServiceModel.EndpointAddress endpointAddress = new System.ServiceModel.EndpointAddress(url);
            System.ServiceModel.BasicHttpBinding binding = new System.ServiceModel.BasicHttpBinding();

            binding.MaxReceivedMessageSize = int.MaxValue;
            binding.ReceiveTimeout = new TimeSpan(0, 5, 0);
            binding.SendTimeout = new TimeSpan(0, 5, 0);

            if (url.StartsWith("https"))
                binding.Security.Mode = System.ServiceModel.BasicHttpSecurityMode.Transport;

            return new ServicoDBTrans.ValePedagio.ValePedagioSoapClient(binding, endpointAddress);
        }

        #endregion Métodos Privados - Configurações
    }
}
