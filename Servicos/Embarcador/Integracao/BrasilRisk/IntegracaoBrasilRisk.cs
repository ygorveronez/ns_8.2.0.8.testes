using Dominio.Excecoes.Embarcador;
using Dominio.ObjetosDeValor.Embarcador.Enumeradores;
using Dominio.ObjetosDeValor.Enumerador;
using Infrastructure.Services.HttpClientFactory;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Xml.Linq;

namespace Servicos.Embarcador.Integracao.BrasilRisk
{
    public class IntegracaoBrasilRisk
    {
        #region Atributos Privados

        private readonly Repositorio.UnitOfWork _unitOfWork;

        #endregion Atributos Privados

        #region Construtores

        public IntegracaoBrasilRisk(Repositorio.UnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        #endregion Construtores

        #region Métodos Públicos

        public static void IntegrarCargaCancelamento(Dominio.Entidades.Embarcador.Cargas.CargaCancelamentoCargaIntegracao cargaIntegracao, Repositorio.UnitOfWork unitOfWork)
        {
            Repositorio.Embarcador.Cargas.CargaCancelamentoCargaIntegracao repositorioCargaIntegracao = new Repositorio.Embarcador.Cargas.CargaCancelamentoCargaIntegracao(unitOfWork);
            ArquivoTransacao<Dominio.Entidades.Embarcador.Cargas.CargaCTeIntegracaoArquivo> servicoArquivoTransacao = new ArquivoTransacao<Dominio.Entidades.Embarcador.Cargas.CargaCTeIntegracaoArquivo>(unitOfWork);
            Servicos.Models.Integracao.InspectorBehavior inspector = new Servicos.Models.Integracao.InspectorBehavior();

            cargaIntegracao.NumeroTentativas += 1;
            cargaIntegracao.DataIntegracao = DateTime.Now;

            try
            {
                Repositorio.Embarcador.Cargas.CargaCargaIntegracao repositorioCargaCargaIntegracao = new Repositorio.Embarcador.Cargas.CargaCargaIntegracao(unitOfWork);
                string protocoloIntegracaoCarga = repositorioCargaCargaIntegracao.BuscarProtocoloPorCargaETipoIntegracao(cargaIntegracao.CargaCancelamento.Carga.Codigo, Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoIntegracao.BrasilRisk);

                if (string.IsNullOrEmpty(protocoloIntegracaoCarga))
                    throw new ServicoException("Carga sem protocolo de integração com a BRK.", CodigoExcecao.ProtocoloNaoEncontrado);

                Dominio.ObjetosDeValor.Embarcador.Integracao.BrasilRisk.ConfiguracaoIntegracaoBrasilRisk configuracaoIntegracao = ObterConfiguracaoIntegracao(cargaIntegracao.CargaCancelamento.Carga, unitOfWork);
                ServicoBrasilRisk.Monitoramento.ServiceSoapClient servicoMonitoramento = new Servicos.Embarcador.Integracao.ConfiguracaoWebService(unitOfWork).ObterClient<ServicoBrasilRisk.Monitoramento.ServiceSoapClient, ServicoBrasilRisk.Monitoramento.ServiceSoap>(Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoWebServiceIntegracao.BrasilRisk_Monitoramento, out inspector);

                servicoMonitoramento.Endpoint.Address = new System.ServiceModel.EndpointAddress(configuracaoIntegracao.Url);

                ServicoBrasilRisk.Monitoramento.Credenciais credenciais = new ServicoBrasilRisk.Monitoramento.Credenciais()
                {
                    Usuario = configuracaoIntegracao.Usuario,
                    Senha = configuracaoIntegracao.Senha
                };

                ServicoBrasilRisk.Monitoramento.RetornoMonitoramento retorno = servicoMonitoramento.CancelarMonitoramento(credenciais, protocoloIntegracaoCarga.ToInt());

                if (retorno.StatusServico)
                {
                    cargaIntegracao.SituacaoIntegracao = SituacaoIntegracao.ProblemaIntegracao;
                    cargaIntegracao.ProblemaIntegracao = retorno.Mensagem;
                }
                else
                {
                    cargaIntegracao.Protocolo = retorno.MonitoramentoId.ToString();
                    cargaIntegracao.SituacaoIntegracao = SituacaoIntegracao.Integrado;
                    cargaIntegracao.ProblemaIntegracao = $"{retorno.Mensagem} - Protocolo {cargaIntegracao.Protocolo}.";
                }

                servicoArquivoTransacao.Adicionar(cargaIntegracao, inspector.LastRequestXML, inspector.LastResponseXML, "xml");
            }
            catch (ServicoException excecao)
            {
                cargaIntegracao.SituacaoIntegracao = (excecao.ErrorCode == CodigoExcecao.ProtocoloNaoEncontrado) ? SituacaoIntegracao.Integrado : SituacaoIntegracao.ProblemaIntegracao;
                cargaIntegracao.ProblemaIntegracao = excecao.Message;
            }
            catch (Exception excecao)
            {
                Log.TratarErro(excecao, "IntegracaoBrasilRisk");
                cargaIntegracao.SituacaoIntegracao = SituacaoIntegracao.ProblemaIntegracao;
                cargaIntegracao.ProblemaIntegracao = "Ocorreu uma falha ao realizar a integração com a Brasil Risk";
                servicoArquivoTransacao.Adicionar(cargaIntegracao, inspector.LastRequestXML, inspector.LastResponseXML, "xml");
            }

            repositorioCargaIntegracao.Atualizar(cargaIntegracao);
        }

        public static void IntegrarSMLVeiculoVazio(Dominio.Entidades.Embarcador.Cargas.CargaDadosTransporteIntegracao integracaoCargaDadosTransporteIntegracao, Repositorio.UnitOfWork unitOfWork, AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware tipoServicoMultisoftware)
        {
            if (!PermitirIntegrarCarga(integracaoCargaDadosTransporteIntegracao.Carga))
            {
                Repositorio.Embarcador.Cargas.CargaDadosTransporteIntegracao repositorioCargaDadosTransporteIntegracao = new Repositorio.Embarcador.Cargas.CargaDadosTransporteIntegracao(unitOfWork);

                integracaoCargaDadosTransporteIntegracao.DataIntegracao = DateTime.Now;
                integracaoCargaDadosTransporteIntegracao.NumeroTentativas++;
                integracaoCargaDadosTransporteIntegracao.SituacaoIntegracao = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoIntegracao.ProblemaIntegracao;
                integracaoCargaDadosTransporteIntegracao.ProblemaIntegracao = "Critérios necessários para integração com a BrasilRisk não foram cumpridos.";

                repositorioCargaDadosTransporteIntegracao.Atualizar(integracaoCargaDadosTransporteIntegracao);

                return;
            }

            IntegrarCargaBrasilRiskSML(integracaoCargaDadosTransporteIntegracao, true, unitOfWork);
        }

        public static void IntegrarCarga(Dominio.Entidades.Embarcador.Cargas.CargaCargaIntegracao cargaIntegracao, Repositorio.UnitOfWork unitOfWork, AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware tipoServicoMultisoftware)
        {
            Repositorio.Embarcador.Cargas.CargaCargaIntegracao repositorioCargaIntegracao = new Repositorio.Embarcador.Cargas.CargaCargaIntegracao(unitOfWork);
            try
            {
                Repositorio.Embarcador.Configuracoes.Integracao repositorioConfiguracaoIntegracao = new Repositorio.Embarcador.Configuracoes.Integracao(unitOfWork);
                Dominio.Entidades.Embarcador.Configuracoes.Integracao configuracaoIntegracao = repositorioConfiguracaoIntegracao.Buscar();

                if (configuracaoIntegracao == null)
                {
                    cargaIntegracao.DataIntegracao = DateTime.Now;
                    cargaIntegracao.NumeroTentativas++;
                    cargaIntegracao.SituacaoIntegracao = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoIntegracao.ProblemaIntegracao;
                    cargaIntegracao.ProblemaIntegracao = "Não existe configuração de integração disponível para a BrasilRisk.";

                    repositorioCargaIntegracao.Atualizar(cargaIntegracao);

                    return;
                }

                Repositorio.Embarcador.Cargas.CargaPedido repositorioCargaPedido = new Repositorio.Embarcador.Cargas.CargaPedido(unitOfWork);
                List<Dominio.ObjetosDeValor.Embarcador.Carga.CargaPedido> cargaPedidos = repositorioCargaPedido.BuscarCodigosCargaPedidoPorCarga(cargaIntegracao.Carga.Codigo, false);
                decimal valorMercadoria = 1;

                if (cargaPedidos.Count > 0)
                {
                    var notasFiscais = cargaPedidos.SelectMany(cargaPedido => cargaPedido.NotasFiscais).Distinct();
                    valorMercadoria = notasFiscais.Sum(notaFiscal => notaFiscal.Valor);
                }

                if (valorMercadoria < configuracaoIntegracao.ValorBaseBrasilRisk)
                {
                    cargaIntegracao.DataIntegracao = DateTime.Now;
                    cargaIntegracao.NumeroTentativas = 3;
                    cargaIntegracao.SituacaoIntegracao = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoIntegracao.ProblemaIntegracao;
                    cargaIntegracao.ProblemaIntegracao = "Valor abaixo do valor configurado para integrar.";

                    repositorioCargaIntegracao.Atualizar(cargaIntegracao);

                    return;
                }

                if (configuracaoIntegracao.IntegrarRotaBrasilRisk)
                    IntegrarRotaBrasilRisk(cargaIntegracao, unitOfWork);

                if (!cargaIntegracao.TipoIntegracao.IntegrarComPlataformaNstech)
                    IntegrarCargaBrasilRisk(cargaIntegracao, false, unitOfWork);
                else
                {
                    Servicos.Embarcador.Integracao.Nstech.IntegracaoSM svcIntegracaoSMNstech = new Servicos.Embarcador.Integracao.Nstech.IntegracaoSM(tipoServicoMultisoftware, unitOfWork);
                    svcIntegracaoSMNstech.IntegrarSM(cargaIntegracao);
                }
            }
            catch (Exception ex)
            {
                Log.TratarErro(ex);
                cargaIntegracao.DataIntegracao = DateTime.Now;
                cargaIntegracao.NumeroTentativas = 3;
                cargaIntegracao.SituacaoIntegracao = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoIntegracao.ProblemaIntegracao;
                cargaIntegracao.ProblemaIntegracao = "Ocorreu uma falha ao enviar a integração";

                repositorioCargaIntegracao.Atualizar(cargaIntegracao);
            }
        }

        public static ServicoBrasilRisk.GestaoAnaliseDePerfil.RetornoConsulta ConsultaMotorista(string cpfMotorista, ref string mensagemErro, ref string xmlRequest, ref string xmlResponse, Repositorio.UnitOfWork unitOfWork)
        {
            try
            {
                Repositorio.Embarcador.Configuracoes.Integracao repIntegracao = new Repositorio.Embarcador.Configuracoes.Integracao(unitOfWork);

                Dominio.Entidades.Embarcador.Configuracoes.Integracao integracao = repIntegracao.Buscar();

                if (integracao == null || string.IsNullOrWhiteSpace(integracao.UsuarioBrasilRisk) || string.IsNullOrWhiteSpace(integracao.SenhaBrasilRisk) || string.IsNullOrWhiteSpace(integracao.URLBrasilRiskGestao))
                {
                    mensagemErro = "Não existe configuração de integração disponível para a BrasilRisk.";
                    return null;
                }

                string urlWebService = integracao.URLBrasilRiskGestao;

                ServicoBrasilRisk.GestaoAnaliseDePerfil.ServiceSoapClient svcBrasilRisk = new ConfiguracaoWebService(unitOfWork).ObterClient<ServicoBrasilRisk.GestaoAnaliseDePerfil.ServiceSoapClient, ServicoBrasilRisk.GestaoAnaliseDePerfil.ServiceSoap>(Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoWebServiceIntegracao.BrasilRisk_GestaoAnalisePerfil, out Servicos.Models.Integracao.InspectorBehavior inspector);
                svcBrasilRisk.Endpoint.Address = new System.ServiceModel.EndpointAddress(urlWebService);

                ServicoBrasilRisk.GestaoAnaliseDePerfil.Credenciais credenciais = new ServicoBrasilRisk.GestaoAnaliseDePerfil.Credenciais()
                {
                    Usuario = integracao.UsuarioBrasilRisk,
                    Senha = integracao.SenhaBrasilRisk
                };

                ServicoBrasilRisk.GestaoAnaliseDePerfil.ConsultarMotoristaRequest requisicao = new Servicos.ServicoBrasilRisk.GestaoAnaliseDePerfil.ConsultarMotoristaRequest
                {
                    Credenciais = credenciais,
                    cpf = Utilidades.String.OnlyNumbers(cpfMotorista)
                };

                var retornoConsulta = svcBrasilRisk.ConsultarMotorista(requisicao);

                xmlRequest = inspector.LastRequestXML;
                xmlResponse = inspector.LastResponseXML;

                return retornoConsulta.ConsultarMotoristaResult;
            }
            catch (Exception excecao)
            {
                Servicos.Log.TratarErro("ConsultaMotorista exceção: " + excecao, "BrasilRisk");
                mensagemErro = "SERVIÇO DA BRASILRISK INDISPONÍVEL";
                return null;
            }
        }

        public static ServicoBrasilRisk.GestaoAnaliseDePerfil.RetornoConsulta ConsultaVeiculo(string placa, ref string mensagemErro, ref string xmlRequest, ref string xmlResponse, Repositorio.UnitOfWork unitOfWork)
        {
            try
            {
                Repositorio.Embarcador.Configuracoes.Integracao repIntegracao = new Repositorio.Embarcador.Configuracoes.Integracao(unitOfWork);

                Dominio.Entidades.Embarcador.Configuracoes.Integracao integracao = repIntegracao.Buscar();

                if (integracao == null || string.IsNullOrWhiteSpace(integracao.UsuarioBrasilRisk) || string.IsNullOrWhiteSpace(integracao.SenhaBrasilRisk) || string.IsNullOrWhiteSpace(integracao.URLBrasilRiskGestao))
                {
                    mensagemErro = "Não existe configuração de integração disponível para a BrasilRisk.";
                    return null;
                }

                string urlWebService = integracao.URLBrasilRiskGestao;

                ServicoBrasilRisk.GestaoAnaliseDePerfil.ServiceSoapClient svcBrasilRisk = new ConfiguracaoWebService(unitOfWork).ObterClient<ServicoBrasilRisk.GestaoAnaliseDePerfil.ServiceSoapClient, ServicoBrasilRisk.GestaoAnaliseDePerfil.ServiceSoap>(Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoWebServiceIntegracao.BrasilRisk_GestaoAnalisePerfil, out Servicos.Models.Integracao.InspectorBehavior inspector);
                svcBrasilRisk.Endpoint.Address = new System.ServiceModel.EndpointAddress(urlWebService);

                ServicoBrasilRisk.GestaoAnaliseDePerfil.Credenciais credenciais = new ServicoBrasilRisk.GestaoAnaliseDePerfil.Credenciais()
                {
                    Usuario = integracao.UsuarioBrasilRisk,
                    Senha = integracao.SenhaBrasilRisk
                };

                ServicoBrasilRisk.GestaoAnaliseDePerfil.ConsultarVeiculoRequest requisicao = new ServicoBrasilRisk.GestaoAnaliseDePerfil.ConsultarVeiculoRequest
                {
                    Credenciais = credenciais,
                    placa = placa
                };

                ServicoBrasilRisk.GestaoAnaliseDePerfil.ConsultarVeiculoResponse retornoConsulta = svcBrasilRisk.ConsultarVeiculo(requisicao);

                xmlRequest = inspector.LastRequestXML;
                xmlResponse = inspector.LastResponseXML;

                return retornoConsulta.ConsultarVeiculoResult;
            }
            catch (Exception excecao)
            {
                Log.TratarErro("ConsultarVeiculo exceção: " + excecao, "BrasilRisk");
                mensagemErro = "SERVIÇO DA BRASILRISK INDISPONÍVEL";
                return null;
            }
        }

        public static Dominio.ObjetosDeValor.Embarcador.Integracao.Nstech.SM.RequisicaoSM RetornarObjetoRequisicaoSM(Dominio.Entidades.Embarcador.Cargas.CargaCargaIntegracao cargaIntegracao, Repositorio.UnitOfWork unitOfWork, AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware tipoServicoMultisoftware)
        {
            Dominio.ObjetosDeValor.Embarcador.Integracao.BrasilRisk.ConfiguracaoIntegracaoBrasilRisk configuracaoIntegracaoBrasilRisk = ObterConfiguracaoIntegracao(cargaIntegracao.Carga, unitOfWork);
            Repositorio.Embarcador.Pedidos.PedidoXMLNotaFiscal repPedidoXMLNotaFiscal = new Repositorio.Embarcador.Pedidos.PedidoXMLNotaFiscal(unitOfWork);
            Repositorio.Embarcador.Cargas.CargaPedido repCargaPedido = new Repositorio.Embarcador.Cargas.CargaPedido(unitOfWork);
            Repositorio.Embarcador.Cargas.CargaMotorista repCargaMotorista = new Repositorio.Embarcador.Cargas.CargaMotorista(unitOfWork);
            Repositorio.Embarcador.Cargas.ControleEntrega.CargaEntregaPedido repCargaEntregaPedido = new Repositorio.Embarcador.Cargas.ControleEntrega.CargaEntregaPedido(unitOfWork);
            Repositorio.Embarcador.Configuracoes.Integracao repConfiguracaoIntegracao = new Repositorio.Embarcador.Configuracoes.Integracao(unitOfWork);
            Repositorio.Embarcador.Cargas.ControleEntrega.CargaEntrega repCargaEntrega = new Repositorio.Embarcador.Cargas.ControleEntrega.CargaEntrega(unitOfWork);

            Dominio.Entidades.Embarcador.Configuracoes.Integracao configuracaoIntegracao = repConfiguracaoIntegracao.Buscar();

            Dominio.Entidades.Usuario motorista = new Dominio.Entidades.Usuario();

            List<Dominio.Entidades.Usuario> motoristas = repCargaMotorista.BuscarMotoristasPorCarga(cargaIntegracao.Carga.Codigo);
            if (cargaIntegracao.Carga.Motoristas != null && cargaIntegracao.Carga.Motoristas.Count > 0)
                motorista = cargaIntegracao.Carga.Motoristas.FirstOrDefault();
            else if (motoristas != null && motoristas.Count > 0)
                motorista = motoristas.FirstOrDefault();

            Dominio.Entidades.Embarcador.Cargas.CargaPedido cargaPedido = repCargaPedido.BuscarPrimeiraPorCarga(cargaIntegracao.Carga.Codigo);
            Dominio.Entidades.Embarcador.Cargas.CargaPedido ultimaCargaPedido = repCargaPedido.BuscarUltimaEntregaCarga(cargaIntegracao.Carga.Codigo);

            List<Dominio.Entidades.Embarcador.Cargas.ControleEntrega.CargaEntregaPedido> cargaEntregaPedidos = repCargaEntregaPedido.BuscarPorCarga(cargaIntegracao.Carga.Codigo);

            List<Dominio.Entidades.Embarcador.Cargas.ControleEntrega.CargaEntregaPedido> cargasPedidosFiltrados = new List<Dominio.Entidades.Embarcador.Cargas.ControleEntrega.CargaEntregaPedido>();
            List<Dominio.Entidades.Embarcador.Cargas.CargaPedido> cargasPedidos = new List<Dominio.Entidades.Embarcador.Cargas.CargaPedido>();
            List<Dominio.Entidades.Embarcador.Cargas.ControleEntrega.CargaEntrega> cargaEntregas = new List<Dominio.Entidades.Embarcador.Cargas.ControleEntrega.CargaEntrega>();

            foreach (Dominio.Entidades.Embarcador.Cargas.ControleEntrega.CargaEntregaPedido cargaEntregaPedido in cargaEntregaPedidos)
                if (cargasPedidosFiltrados.Any(obj => obj.CargaEntrega.Codigo == cargaEntregaPedido.CargaEntrega.Codigo))
                    continue;
                else
                    cargasPedidosFiltrados.Add(cargaEntregaPedido);

            cargasPedidos.AddRange(cargasPedidosFiltrados.Select(obj => obj.CargaPedido).ToList());
            cargaEntregas.AddRange(cargasPedidosFiltrados.Select(obj => obj.CargaEntrega).ToList());

            decimal peso = repPedidoXMLNotaFiscal.ObterPesoTotalPorCarga(cargaIntegracao.Carga.Codigo);
            decimal valorTotalNotas = repPedidoXMLNotaFiscal.ObterValorTotalPorCarga(cargaIntegracao.Carga.Codigo);

            int numeroPrimeiraNota = 0;
            string seriePrimeiraNota = "";
            if (cargaPedido != null && cargaPedido.NotasFiscais != null && cargaPedido.NotasFiscais.Count() > 0 && cargaPedido.NotasFiscais.First().XMLNotaFiscal != null)
            {
                numeroPrimeiraNota = cargaPedido.NotasFiscais.First().XMLNotaFiscal.Numero;
                if (cargaPedido.NotasFiscais.First().XMLNotaFiscal.SerieOuSerieDaChave != null)
                    seriePrimeiraNota = cargaPedido.NotasFiscais.First().XMLNotaFiscal.SerieOuSerieDaChave;
            }

            Dominio.ObjetosDeValor.Embarcador.Integracao.Nstech.SM.RequisicaoSM retorno = new Dominio.ObjetosDeValor.Embarcador.Integracao.Nstech.SM.RequisicaoSM();
            retorno.cliente_documento = !string.IsNullOrEmpty(cargaIntegracao.Carga.TipoOperacao?.CNPJClienteBrasilRisk) ? cargaIntegracao.Carga.TipoOperacao?.CNPJClienteBrasilRisk : cargaPedido.Pedido.Remetente.CPF_CNPJ.ToString();
            retorno.fornecedor_id = ((int)Dominio.ObjetosDeValor.Embarcador.Enumeradores.EnumTecnologiaGerenciadora.BRK).ToString();

            Dominio.ObjetosDeValor.Embarcador.Integracao.Nstech.SM.DocumentoEntrega documentoEntregaBase = new Dominio.ObjetosDeValor.Embarcador.Integracao.Nstech.SM.DocumentoEntrega();
            documentoEntregaBase.peso_entregue = (double)peso;
            documentoEntregaBase.valor_entregue = valorTotalNotas;
            documentoEntregaBase.numero_documento = numeroPrimeiraNota.ToString();
            documentoEntregaBase.produto_entregue = cargaIntegracao.Carga.TipoOperacao?.ProdutoBrasilRisk;
            documentoEntregaBase.centro_custo = cargaIntegracao.Carga.TipoOperacao?.CentroCustoBrasilRisk;
            documentoEntregaBase.serie_documento = seriePrimeiraNota;

            retorno.ponto_entrega = new List<Dominio.ObjetosDeValor.Embarcador.Integracao.Nstech.SM.PontoEntrega>();
            int sequencia = 2;

            Dominio.Entidades.Cliente clienteOrigem = cargaPedido.Pedido.Expedidor ?? cargaPedido.Pedido.Remetente;

            Dominio.Entidades.Cliente clienteDestino = cargaPedido.Pedido.Recebedor ?? cargaPedido.Pedido.Destinatario;

            Dominio.ObjetosDeValor.Embarcador.Integracao.Nstech.SM.Origem origem = new Dominio.ObjetosDeValor.Embarcador.Integracao.Nstech.SM.Origem();
            origem.cidade = clienteOrigem.Localidade.Descricao;
            origem.uf = clienteOrigem.Localidade.Estado.Sigla;
            origem.nome = !string.IsNullOrWhiteSpace(clienteOrigem.CodigoIntegracao) ? clienteOrigem.CodigoIntegracao : clienteOrigem.Localidade.Descricao;
            origem.rua = clienteOrigem.Endereco;
            origem.numero = clienteOrigem.Numero;
            origem.bairro = clienteOrigem.Bairro;
            retorno.origem = origem;
            retorno.sm_observacoes = PermitirEnviarDadosTransportadoraSubContratada(cargaIntegracao, unitOfWork);

            Dominio.ObjetosDeValor.Embarcador.Integracao.Nstech.SM.DocumentoColeta documentoColeta = new Dominio.ObjetosDeValor.Embarcador.Integracao.Nstech.SM.DocumentoColeta();
            documentoColeta.produto_coletado = cargaIntegracao.Carga.TipoOperacao?.ProdutoBrasilRisk;

            Dominio.ObjetosDeValor.Embarcador.Integracao.Nstech.SM.PontoColeta pontoColeta = new Dominio.ObjetosDeValor.Embarcador.Integracao.Nstech.SM.PontoColeta();
            pontoColeta.documento_coleta = new List<Dominio.ObjetosDeValor.Embarcador.Integracao.Nstech.SM.DocumentoColeta>();
            pontoColeta.documento_coleta.Add(documentoColeta);
            pontoColeta.sequencia = "1";
            pontoColeta.endereco_cep = Utilidades.String.OnlyNumbers(clienteOrigem.CEP);
            pontoColeta.endereco_uf = origem.uf;
            pontoColeta.nome_rua = origem.rua;
            int numeroOrigem = 0;
            int.TryParse(origem.numero, out numeroOrigem);
            pontoColeta.numero = numeroOrigem;
            pontoColeta.endereco_codigo_cidade_ibge = clienteOrigem.Localidade.CodigoIBGE;
            pontoColeta.nome = origem.nome;
            pontoColeta.codigo_destino = clienteOrigem.CodigoIntegracao;
            retorno.ponto_coleta = new List<Dominio.ObjetosDeValor.Embarcador.Integracao.Nstech.SM.PontoColeta>
            {
                pontoColeta
            };

            if (configuracaoIntegracaoBrasilRisk.EnviarTodosDestinos)
            {
                List<Dominio.Entidades.Cliente> destinos = cargaEntregas
                    .Where(cargaEntrega => (cargaEntrega.ClienteOutroEndereco?.Cliente.Codigo ?? cargaEntrega.Cliente.Codigo) != clienteOrigem.Codigo) //Remove a CargaEntrega que já foi enviada como ponto de origem
                    .Select(obj => obj.ClienteOutroEndereco?.Cliente ?? obj.Cliente)
                    .Distinct()
                    .ToList();

                foreach (Dominio.Entidades.Cliente destinatario in destinos)
                {
                    Dominio.ObjetosDeValor.Embarcador.Integracao.Nstech.SM.DocumentoEntrega documentoEntregaPorPedido = documentoEntregaBase.Clonar();

                    if (sequencia != 2)
                    {
                        documentoEntregaPorPedido.peso_entregue = 0d;
                        documentoEntregaPorPedido.valor_entregue = 0m;
                    }

                    Dominio.ObjetosDeValor.Embarcador.Integracao.Nstech.SM.PontoEntrega pontoEntrega = new Dominio.ObjetosDeValor.Embarcador.Integracao.Nstech.SM.PontoEntrega();
                    pontoEntrega.sequencia = sequencia.ToString();
                    pontoEntrega.endereco_cep = Utilidades.String.OnlyNumbers(destinatario.CEP);
                    pontoEntrega.endereco_uf = destinatario.Localidade.Estado.Sigla;
                    pontoEntrega.nome_rua = destinatario.Endereco;
                    pontoEntrega.numero = destinatario.Numero;
                    pontoEntrega.documento_entrega = new List<Dominio.ObjetosDeValor.Embarcador.Integracao.Nstech.SM.DocumentoEntrega>
                    {
                        documentoEntregaPorPedido
                    };
                    pontoEntrega.endereco_codigo_cidade_ibge = destinatario.Localidade.CodigoIBGE;
                    pontoEntrega.nome = !string.IsNullOrWhiteSpace(destinatario.CodigoIntegracao) ? destinatario.CodigoIntegracao : destinatario.Localidade.Descricao;
                    pontoEntrega.nome_cliente = destinatario.Nome;//criado..
                    pontoEntrega.codigo_destino = destinatario.CodigoIntegracao;
                    retorno.ponto_entrega.Add(pontoEntrega);

                    sequencia++;
                }
            }
            else
            {
                Dominio.ObjetosDeValor.Embarcador.Integracao.Nstech.SM.DocumentoEntrega documentoEntrega = documentoEntregaBase.Clonar();
                Dominio.ObjetosDeValor.Embarcador.Integracao.Nstech.SM.PontoEntrega pontoEntrega = new Dominio.ObjetosDeValor.Embarcador.Integracao.Nstech.SM.PontoEntrega();
                pontoEntrega.sequencia = sequencia.ToString();
                pontoEntrega.endereco_cep = Utilidades.String.OnlyNumbers(clienteDestino.CEP);
                pontoEntrega.endereco_uf = clienteDestino.Localidade.Estado.Sigla;
                pontoEntrega.nome_rua = clienteDestino.Endereco;
                pontoEntrega.numero = clienteDestino.Numero;
                pontoEntrega.documento_entrega = new List<Dominio.ObjetosDeValor.Embarcador.Integracao.Nstech.SM.DocumentoEntrega>
                {
                    documentoEntrega
                };
                pontoEntrega.endereco_codigo_cidade_ibge = clienteDestino.Localidade.CodigoIBGE;
                pontoEntrega.nome = !string.IsNullOrWhiteSpace(clienteDestino.CodigoIntegracao) ? clienteDestino.CodigoIntegracao : clienteDestino.Localidade.Descricao;
                pontoEntrega.nome_cliente = clienteDestino.Nome;//criado..
                pontoEntrega.codigo_destino = clienteDestino.CodigoIntegracao;

                retorno.ponto_entrega.Add(pontoEntrega);
                sequencia++;
            }

            if (configuracaoIntegracaoBrasilRisk.EnviarTodosDestinos)
                cargaPedido = cargasPedidos.LastOrDefault();

            int contratacao = 0;
            if (cargaPedido.Carga.ProcedimentoEmbarque != null)
                contratacao = cargaPedido.Carga.ProcedimentoEmbarque.CodigoModeloContratacao;
            else
                contratacao = cargaPedido.Carga.TipoOperacao?.CodigoModeloContratacao ?? 0;

            Dominio.ObjetosDeValor.Embarcador.Integracao.Nstech.SM.Destino destino = new Dominio.ObjetosDeValor.Embarcador.Integracao.Nstech.SM.Destino();
            if (contratacao == 2 && !(cargaIntegracao.Carga.TipoOperacao?.NaoEnviarOrigemComoUltimoPontoRota ?? false))
            {
                Dominio.ObjetosDeValor.Embarcador.Integracao.Nstech.SM.DocumentoEntrega documentoEntregaPorContratacao = documentoEntregaBase.Clonar();

                documentoEntregaPorContratacao.peso_entregue = 0d;
                documentoEntregaPorContratacao.valor_entregue = 0m;

                clienteDestino = cargaPedido.Pedido.Recebedor ?? cargaPedido.Pedido.Destinatario;

                destino.cidade = clienteDestino.Localidade.Descricao;
                destino.uf = clienteDestino.Localidade.Estado.Sigla;
                destino.nome = !string.IsNullOrWhiteSpace(clienteDestino.CodigoIntegracao) ? clienteDestino.CodigoIntegracao : clienteDestino.Localidade.Descricao;
                destino.rua = clienteDestino.Endereco;
                destino.numero = clienteDestino.Numero;
                destino.bairro = clienteDestino.Bairro;

                Dominio.ObjetosDeValor.Embarcador.Integracao.Nstech.SM.PontoEntrega pontoEntrega = new Dominio.ObjetosDeValor.Embarcador.Integracao.Nstech.SM.PontoEntrega();
                pontoEntrega.sequencia = sequencia.ToString();
                pontoEntrega.endereco_cep = Utilidades.String.OnlyNumbers(clienteDestino.CEP);
                pontoEntrega.endereco_uf = destino.uf;
                pontoEntrega.nome_rua = destino.rua;
                pontoEntrega.numero = destino.numero;
                pontoEntrega.documento_entrega = new List<Dominio.ObjetosDeValor.Embarcador.Integracao.Nstech.SM.DocumentoEntrega>();
                pontoEntrega.documento_entrega.Add(documentoEntregaPorContratacao);
                pontoEntrega.endereco_codigo_cidade_ibge = clienteDestino.Localidade.CodigoIBGE;
                pontoEntrega.nome = destino.nome;
                pontoEntrega.nome_cliente = clienteDestino.Nome;//criado..
                pontoEntrega.codigo_destino = clienteDestino.CodigoIntegracao;
                retorno.ponto_entrega.Add(pontoEntrega);
            }
            else if (cargaPedido.Recebedor != null)
            {
                destino.cidade = cargaPedido.Recebedor.Localidade.Descricao ?? "";
                destino.uf = cargaPedido.Recebedor.Localidade.Estado.Sigla ?? "";
                destino.nome = !string.IsNullOrWhiteSpace(cargaPedido.Recebedor.CodigoIntegracao) ? cargaPedido.Recebedor.CodigoIntegracao : cargaPedido.Recebedor.Localidade.Descricao ?? "";
                destino.rua = cargaPedido.Recebedor.Endereco ?? "";
                destino.numero = cargaPedido.Recebedor.Numero ?? "";
                destino.bairro = cargaPedido.Recebedor.Bairro ?? "";
            }
            else if (cargaPedido.Pedido.Destinatario != null)
            {
                destino.cidade = cargaPedido.Pedido.Destinatario.Localidade.Descricao ?? "";
                destino.uf = cargaPedido.Pedido.Destinatario.Localidade.Estado.Sigla ?? "";
                destino.nome = cargaPedido.Pedido.Destinatario.Localidade.Descricao ?? "";
                destino.rua = cargaPedido.Pedido.Destinatario.Endereco ?? "";
                destino.numero = cargaPedido.Pedido.Destinatario.Numero ?? "";
                destino.bairro = cargaPedido.Pedido.Destinatario.Bairro ?? "";
            }
            retorno.destino = destino;

            retorno.reboque = new List<Dominio.ObjetosDeValor.Embarcador.Integracao.Nstech.SM.Reboque>();
            foreach (var reboquecarga in cargaIntegracao.Carga.VeiculosVinculados)
            {
                Dominio.ObjetosDeValor.Embarcador.Integracao.Nstech.SM.Reboque reboque = new Dominio.ObjetosDeValor.Embarcador.Integracao.Nstech.SM.Reboque();
                reboque.identificador = reboquecarga?.Placa;
                retorno.reboque.Add(reboque);
            }

            Dominio.ObjetosDeValor.Embarcador.Integracao.Nstech.SM.Tracionador tracionador = new Dominio.ObjetosDeValor.Embarcador.Integracao.Nstech.SM.Tracionador();
            tracionador.identificador = cargaIntegracao.Carga.Veiculo?.Placa;
            retorno.tracionador = tracionador;

            retorno.transportador = new Dominio.ObjetosDeValor.Embarcador.Integracao.Nstech.SM.Transportador();
            retorno.transportador.documento_identificador = !string.IsNullOrEmpty(cargaIntegracao.Carga.TipoOperacao?.CNPJTransportadoraBrasilRisk) ? cargaIntegracao.Carga.TipoOperacao?.CNPJTransportadoraBrasilRisk : cargaIntegracao.Carga.Empresa.CNPJ_SemFormato;

            Dominio.ObjetosDeValor.Embarcador.Integracao.Nstech.SM.Condutor condutor = new Dominio.ObjetosDeValor.Embarcador.Integracao.Nstech.SM.Condutor();
            condutor.documento_identificador = motorista?.CPF;
            condutor.nome = motorista?.Nome;
            condutor.numero_telefone = motorista?.Telefone;
            condutor.ddd_telefone = motorista?.Telefone;
            condutor.tipo_contato = "1";
            condutor.uf_rg = motorista?.EstadoRG != null ? motorista.EstadoRG.CodigoIBGE : 42;

            retorno.condutor = new List<Dominio.ObjetosDeValor.Embarcador.Integracao.Nstech.SM.Condutor>();
            retorno.condutor.Add(condutor);

            retorno.sm_numero_viagem = cargaIntegracao.Carga.CodigoCargaEmbarcador;

            if (cargaIntegracao.Carga.Rota != null)
            {
                retorno.rota = new Dominio.ObjetosDeValor.Embarcador.Integracao.Nstech.SM.Rota()
                {
                    codigo_gr = !string.IsNullOrWhiteSpace(cargaIntegracao.Carga.Rota.CodigoIntegracaoGerenciadoraRisco) ? cargaIntegracao.Carga.Rota.CodigoIntegracaoGerenciadoraRisco : cargaIntegracao.Carga.Rota.CodigoIntegracao
                };
            }

            DateTime dataFim = ultimaCargaPedido.PrevisaoEntrega > cargaIntegracao.DataIntegracao ? ultimaCargaPedido.PrevisaoEntrega.Value : cargaIntegracao.DataIntegracao;
            DateTime dataInicio = cargaIntegracao.Carga.DataInicialPrevisaoCarregamento > dataFim
                ? cargaIntegracao.Carga.DataInicialPrevisaoCarregamento.Value
                : dataFim;

            DateTime previsaoInicio = configuracaoIntegracao.InicioViagemFixoHoraAtualMaisMinutos
                ? DateTime.Now.AddMinutes(configuracaoIntegracao.MinutosAMaisInicioViagem)
                : dataInicio.AddHours(1);

            retorno.data_previsao_inicio = previsaoInicio.ToString("dd/MM/yyyy HH:mm");

            if (previsaoInicio > dataFim && (cargaIntegracao.TipoIntegracao.Tipo == TipoIntegracao.BrasilRisk))
                dataFim = previsaoInicio.AddHours(24);

            retorno.data_previsao_fim = dataFim.AddDays(3).ToString("dd/MM/yyyy HH:mm");

            retorno.usuario = configuracaoIntegracaoBrasilRisk.Usuario;
            retorno.senha = configuracaoIntegracaoBrasilRisk.Senha;
            retorno.token = "";

            return retorno;
        }

        public bool PossuiIntegracaoCancelamentoCarga(Dominio.Entidades.Embarcador.Cargas.Carga carga)
        {
            Repositorio.Embarcador.Cargas.CargaCancelamentoCargaIntegracao repCancelamentoIntegracao = new Repositorio.Embarcador.Cargas.CargaCancelamentoCargaIntegracao(_unitOfWork);
            Dominio.Entidades.Embarcador.Cargas.CargaCancelamentoCargaIntegracao cargaIntegracaoCancelamento = repCancelamentoIntegracao.BuscarPorCargaETipo(carga.Codigo, Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoIntegracao.BrasilRisk);
            if (cargaIntegracaoCancelamento == null)
                return false;


            return true;
        }

        public void CadastrarMotoristaAnalisePerfil(Dominio.Entidades.Embarcador.Transportadores.MotoristaIntegracao motoristaIntegracao)
        {
            Repositorio.Embarcador.Transportadores.MotoristaIntegracao repositorioMotoristaIntegracao = new Repositorio.Embarcador.Transportadores.MotoristaIntegracao(_unitOfWork);
            ArquivoTransacao<Dominio.Entidades.Embarcador.Cargas.CargaCTeIntegracaoArquivo> servicoArquivoTransacao = new ArquivoTransacao<Dominio.Entidades.Embarcador.Cargas.CargaCTeIntegracaoArquivo>(_unitOfWork);
            Models.Integracao.InspectorBehavior inspector = new Models.Integracao.InspectorBehavior(true);

            motoristaIntegracao.NumeroTentativas++;
            motoristaIntegracao.DataIntegracao = DateTime.Now;

            try
            {
                Repositorio.Embarcador.Configuracoes.Integracao repositorioIntegracao = new Repositorio.Embarcador.Configuracoes.Integracao(_unitOfWork);

                Dominio.Entidades.Embarcador.Configuracoes.Integracao configuracaoIntegracao = repositorioIntegracao.Buscar();

                if (configuracaoIntegracao == null || string.IsNullOrWhiteSpace(configuracaoIntegracao.UsuarioBrasilRisk) || string.IsNullOrWhiteSpace(configuracaoIntegracao.SenhaBrasilRisk) || string.IsNullOrWhiteSpace(configuracaoIntegracao.URLBrasilRiskGestao))
                    throw new ServicoException("Não existe configuração de integração disponível para a BrasilRisk.");

                ServicoBrasilRisk.GestaoAnaliseDePerfil.ServiceSoapClient servicoClient = new ConfiguracaoWebService(_unitOfWork).ObterClient<ServicoBrasilRisk.GestaoAnaliseDePerfil.ServiceSoapClient, ServicoBrasilRisk.GestaoAnaliseDePerfil.ServiceSoap>(TipoWebServiceIntegracao.BrasilRisk_GestaoAnalisePerfil, out inspector);
                servicoClient.Endpoint.Address = new System.ServiceModel.EndpointAddress(configuracaoIntegracao.URLBrasilRiskGestao);

                ServicoBrasilRisk.GestaoAnaliseDePerfil.CadastrarMotoristaRequest requisicao = ObterDadosRequestAnalisePerfilCadastroMotorista(configuracaoIntegracao, motoristaIntegracao.Motorista);
                ServicoBrasilRisk.GestaoAnaliseDePerfil.CadastrarMotoristaResponse retorno = servicoClient.CadastrarMotorista(requisicao);

                if (retorno.CadastrarMotoristaResult == null)
                    throw new ServicoException("Retorno do serviço da BrasilRisk não foi encontrado.");

                if (retorno.CadastrarMotoristaResult.Status == ServicoBrasilRisk.GestaoAnaliseDePerfil.StatusPesquisa.ErroValidacao)
                    throw new ServicoException(retorno.CadastrarMotoristaResult.Mensagem ?? "Ocorreu um problema ao integrar com Brasil Risk.");

                if (retorno.CadastrarMotoristaResult.Status == ServicoBrasilRisk.GestaoAnaliseDePerfil.StatusPesquisa.Apto || retorno.CadastrarMotoristaResult.Status == ServicoBrasilRisk.GestaoAnaliseDePerfil.StatusPesquisa.Inapto)
                {
                    motoristaIntegracao.SituacaoIntegracao = SituacaoIntegracao.Integrado;
                    motoristaIntegracao.ProblemaIntegracao = retorno.CadastrarMotoristaResult.Mensagem ?? "Integrado com sucesso.";

                    return;
                }

                if (retorno.CadastrarMotoristaResult.Status == ServicoBrasilRisk.GestaoAnaliseDePerfil.StatusPesquisa.EnviadoParaPesquisa)
                {
                    motoristaIntegracao.NumeroTentativas = 0;
                    motoristaIntegracao.ProblemaIntegracao = retorno.CadastrarMotoristaResult.Mensagem;
                    motoristaIntegracao.SituacaoIntegracao = SituacaoIntegracao.AgRetorno;

                    return;
                }

                motoristaIntegracao.SituacaoIntegracao = SituacaoIntegracao.AgIntegracao;
                motoristaIntegracao.ProblemaIntegracao = retorno.CadastrarMotoristaResult.Mensagem;
            }
            catch (BaseException excecao)
            {
                motoristaIntegracao.SituacaoIntegracao = SituacaoIntegracao.ProblemaIntegracao;
                motoristaIntegracao.ProblemaIntegracao = excecao.Message;
            }
            catch (Exception excecao)
            {
                Log.TratarErro(excecao, "BrasilRisk");

                motoristaIntegracao.SituacaoIntegracao = SituacaoIntegracao.ProblemaIntegracao;
                motoristaIntegracao.ProblemaIntegracao = "Ocorreu um problema ao integrar com Brasil Risk.";
            }
            finally
            {
                servicoArquivoTransacao.Adicionar(motoristaIntegracao, inspector.LastRequestXML, inspector.LastResponseXML, "xml");
                repositorioMotoristaIntegracao.Atualizar(motoristaIntegracao);
            }
        }

        public void ConstultarStatusMotoristaAnalisePerfil(Dominio.Entidades.Embarcador.Transportadores.MotoristaIntegracao motoristaIntegracao)
        {
            Repositorio.Embarcador.Transportadores.MotoristaIntegracao repositorioMotoristaIntegracao = new Repositorio.Embarcador.Transportadores.MotoristaIntegracao(_unitOfWork);
            ArquivoTransacao<Dominio.Entidades.Embarcador.Cargas.CargaCTeIntegracaoArquivo> servicoArquivoTransacao = new ArquivoTransacao<Dominio.Entidades.Embarcador.Cargas.CargaCTeIntegracaoArquivo>(_unitOfWork);
            Models.Integracao.InspectorBehavior inspector = new Models.Integracao.InspectorBehavior(true);

            motoristaIntegracao.NumeroTentativas++;
            motoristaIntegracao.DataIntegracao = DateTime.Now;

            try
            {
                Repositorio.Embarcador.Configuracoes.Integracao repositorioIntegracao = new Repositorio.Embarcador.Configuracoes.Integracao(_unitOfWork);

                Dominio.Entidades.Embarcador.Configuracoes.Integracao configuracaoIntegracao = repositorioIntegracao.Buscar();

                if (configuracaoIntegracao == null || string.IsNullOrWhiteSpace(configuracaoIntegracao.UsuarioBrasilRisk) || string.IsNullOrWhiteSpace(configuracaoIntegracao.SenhaBrasilRisk) || string.IsNullOrWhiteSpace(configuracaoIntegracao.URLBrasilRiskGestao))
                    throw new ServicoException("Não existe configuração de integração disponível para a BrasilRisk.");

                ServicoBrasilRisk.GestaoAnaliseDePerfil.ServiceSoapClient servicoClient = new ConfiguracaoWebService(_unitOfWork).ObterClient<ServicoBrasilRisk.GestaoAnaliseDePerfil.ServiceSoapClient, ServicoBrasilRisk.GestaoAnaliseDePerfil.ServiceSoap>(TipoWebServiceIntegracao.BrasilRisk_GestaoAnalisePerfil, out inspector);
                servicoClient.Endpoint.Address = new System.ServiceModel.EndpointAddress(configuracaoIntegracao.URLBrasilRiskGestao);

                ServicoBrasilRisk.GestaoAnaliseDePerfil.ConsultarMotoristaRequest requisicao = ObterDadosRequestAnalisePerfilConsultarStatusMotorista(configuracaoIntegracao, motoristaIntegracao.Motorista);
                ServicoBrasilRisk.GestaoAnaliseDePerfil.ConsultarMotoristaResponse retorno = servicoClient.ConsultarMotorista(requisicao);

                if (retorno.ConsultarMotoristaResult == null)
                    throw new ServicoException("Retorno do serviço da BrasilRisk não foi encontrado.");

                if (retorno.ConsultarMotoristaResult.CodigoRetorno == 2)
                {
                    motoristaIntegracao.ProblemaIntegracao = retorno.ConsultarMotoristaResult.Mensagem;
                    return;
                }

                if (retorno.ConsultarMotoristaResult.CodigoRetorno != 5)
                    throw new ServicoException(retorno.ConsultarMotoristaResult.Mensagem);

                motoristaIntegracao.SituacaoIntegracao = SituacaoIntegracao.Integrado;
                motoristaIntegracao.ProblemaIntegracao = retorno.ConsultarMotoristaResult.Mensagem;
            }
            catch (BaseException excecao)
            {
                motoristaIntegracao.SituacaoIntegracao = SituacaoIntegracao.ProblemaIntegracao;
                motoristaIntegracao.ProblemaIntegracao = excecao.Message;
            }
            catch (Exception excecao)
            {
                Log.TratarErro(excecao, "BrasilRisk");

                motoristaIntegracao.SituacaoIntegracao = SituacaoIntegracao.ProblemaIntegracao;
                motoristaIntegracao.ProblemaIntegracao = "Ocorreu um problema ao integrar com Brasil Risk.";
            }
            finally
            {
                servicoArquivoTransacao.Adicionar(motoristaIntegracao, inspector.LastRequestXML, inspector.LastResponseXML, "xml", TipoArquivoIntegracaoCTeCarga.RetornoDoProcessamento);
                repositorioMotoristaIntegracao.Atualizar(motoristaIntegracao);
            }
        }

        public void CadastrarVeiculoAnalisePerfil(Dominio.Entidades.Embarcador.Veiculos.VeiculoIntegracao veiculoIntegracao)
        {
            Repositorio.Embarcador.Veiculos.VeiculoIntegracao repositorioVeiculoIntegracao = new Repositorio.Embarcador.Veiculos.VeiculoIntegracao(_unitOfWork);
            ArquivoTransacao<Dominio.Entidades.Embarcador.Cargas.CargaCTeIntegracaoArquivo> servicoArquivoTransacao = new ArquivoTransacao<Dominio.Entidades.Embarcador.Cargas.CargaCTeIntegracaoArquivo>(_unitOfWork);
            Models.Integracao.InspectorBehavior inspector = new Models.Integracao.InspectorBehavior(true);

            veiculoIntegracao.NumeroTentativas++;
            veiculoIntegracao.DataIntegracao = DateTime.Now;

            try
            {
                Repositorio.Embarcador.Configuracoes.Integracao repositorioIntegracao = new Repositorio.Embarcador.Configuracoes.Integracao(_unitOfWork);

                Dominio.Entidades.Embarcador.Configuracoes.Integracao configuracaoIntegracao = repositorioIntegracao.Buscar();

                if (configuracaoIntegracao == null || string.IsNullOrWhiteSpace(configuracaoIntegracao.UsuarioBrasilRisk) || string.IsNullOrWhiteSpace(configuracaoIntegracao.SenhaBrasilRisk) || string.IsNullOrWhiteSpace(configuracaoIntegracao.URLBrasilRiskGestao))
                    throw new ServicoException("Não existe configuração de integração disponível para a BrasilRisk.");

                ServicoBrasilRisk.GestaoAnaliseDePerfil.ServiceSoapClient servicoClient = new ConfiguracaoWebService(_unitOfWork).ObterClient<ServicoBrasilRisk.GestaoAnaliseDePerfil.ServiceSoapClient, ServicoBrasilRisk.GestaoAnaliseDePerfil.ServiceSoap>(TipoWebServiceIntegracao.BrasilRisk_GestaoAnalisePerfil, out inspector);
                servicoClient.Endpoint.Address = new System.ServiceModel.EndpointAddress(configuracaoIntegracao.URLBrasilRiskGestao);

                ServicoBrasilRisk.GestaoAnaliseDePerfil.CadastrarVeiculoRequest requisicao = ObterDadosRequestAnalisePerfilVeiculo(configuracaoIntegracao, veiculoIntegracao.Veiculo);
                ServicoBrasilRisk.GestaoAnaliseDePerfil.CadastrarVeiculoResponse retorno = servicoClient.CadastrarVeiculo(requisicao);

                if (retorno.CadastrarVeiculoResult == null)
                    throw new ServicoException("Retorno do serviço da BrasilRisk não foi encontrado.");

                if (retorno.CadastrarVeiculoResult.Status == ServicoBrasilRisk.GestaoAnaliseDePerfil.StatusPesquisa.ErroValidacao)
                    throw new ServicoException(retorno.CadastrarVeiculoResult.Mensagem ?? "Ocorreu um problema ao integrar com Brasil Risk.");

                if (retorno.CadastrarVeiculoResult.Status == ServicoBrasilRisk.GestaoAnaliseDePerfil.StatusPesquisa.Apto || retorno.CadastrarVeiculoResult.Status == ServicoBrasilRisk.GestaoAnaliseDePerfil.StatusPesquisa.Inapto)
                {
                    veiculoIntegracao.SituacaoIntegracao = SituacaoIntegracao.Integrado;
                    veiculoIntegracao.ProblemaIntegracao = retorno.CadastrarVeiculoResult.Mensagem ?? "Integrado com sucesso.";

                    return;
                }

                if (retorno.CadastrarVeiculoResult.Status == ServicoBrasilRisk.GestaoAnaliseDePerfil.StatusPesquisa.EnviadoParaPesquisa)
                {
                    veiculoIntegracao.NumeroTentativas = 0;
                    veiculoIntegracao.ProblemaIntegracao = retorno.CadastrarVeiculoResult.Mensagem;
                    veiculoIntegracao.SituacaoIntegracao = SituacaoIntegracao.AgRetorno;

                    return;
                }

                veiculoIntegracao.SituacaoIntegracao = SituacaoIntegracao.AgIntegracao;
                veiculoIntegracao.ProblemaIntegracao = retorno.CadastrarVeiculoResult.Mensagem;
            }
            catch (BaseException excecao)
            {
                veiculoIntegracao.SituacaoIntegracao = SituacaoIntegracao.ProblemaIntegracao;
                veiculoIntegracao.ProblemaIntegracao = excecao.Message;
            }
            catch (Exception excecao)
            {
                Log.TratarErro(excecao, "BrasilRisk");

                veiculoIntegracao.SituacaoIntegracao = SituacaoIntegracao.ProblemaIntegracao;
                veiculoIntegracao.ProblemaIntegracao = "Ocorreu um problema ao integrar com Brasil Risk.";
            }
            finally
            {
                servicoArquivoTransacao.Adicionar(veiculoIntegracao, inspector.LastRequestXML, inspector.LastResponseXML, "xml");
                repositorioVeiculoIntegracao.Atualizar(veiculoIntegracao);
            }
        }

        public void ConsultarStatusVeiculoAnalisePerfil(Dominio.Entidades.Embarcador.Veiculos.VeiculoIntegracao veiculoIntegracao)
        {
            Repositorio.Embarcador.Veiculos.VeiculoIntegracao repositorioVeiculoIntegracao = new Repositorio.Embarcador.Veiculos.VeiculoIntegracao(_unitOfWork);
            ArquivoTransacao<Dominio.Entidades.Embarcador.Cargas.CargaCTeIntegracaoArquivo> servicoArquivoTransacao = new ArquivoTransacao<Dominio.Entidades.Embarcador.Cargas.CargaCTeIntegracaoArquivo>(_unitOfWork);
            Models.Integracao.InspectorBehavior inspector = new Models.Integracao.InspectorBehavior(true);

            veiculoIntegracao.NumeroTentativas++;
            veiculoIntegracao.DataIntegracao = DateTime.Now;

            try
            {
                Repositorio.Embarcador.Configuracoes.Integracao repositorioIntegracao = new Repositorio.Embarcador.Configuracoes.Integracao(_unitOfWork);

                Dominio.Entidades.Embarcador.Configuracoes.Integracao configuracaoIntegracao = repositorioIntegracao.Buscar();

                if (configuracaoIntegracao == null || string.IsNullOrWhiteSpace(configuracaoIntegracao.UsuarioBrasilRisk) || string.IsNullOrWhiteSpace(configuracaoIntegracao.SenhaBrasilRisk) || string.IsNullOrWhiteSpace(configuracaoIntegracao.URLBrasilRiskGestao))
                    throw new ServicoException("Não existe configuração de integração disponível para a BrasilRisk.");

                ServicoBrasilRisk.GestaoAnaliseDePerfil.ServiceSoapClient servicoClient = new ConfiguracaoWebService(_unitOfWork).ObterClient<ServicoBrasilRisk.GestaoAnaliseDePerfil.ServiceSoapClient, ServicoBrasilRisk.GestaoAnaliseDePerfil.ServiceSoap>(TipoWebServiceIntegracao.BrasilRisk_GestaoAnalisePerfil, out inspector);
                servicoClient.Endpoint.Address = new System.ServiceModel.EndpointAddress(configuracaoIntegracao.URLBrasilRiskGestao);

                ServicoBrasilRisk.GestaoAnaliseDePerfil.ConsultarVeiculoRequest requisicao = ObterDadosRequestAnalisePerfilConsultarStatusVeiculo(configuracaoIntegracao, veiculoIntegracao.Veiculo);
                ServicoBrasilRisk.GestaoAnaliseDePerfil.ConsultarVeiculoResponse retorno = servicoClient.ConsultarVeiculo(requisicao);

                if (retorno.ConsultarVeiculoResult == null)
                    throw new ServicoException("Retorno do serviço da BrasilRisk não foi encontrado.");

                if (retorno.ConsultarVeiculoResult.CodigoRetorno == 2)
                {
                    veiculoIntegracao.ProblemaIntegracao = retorno.ConsultarVeiculoResult.Mensagem;
                    return;
                }

                if (retorno.ConsultarVeiculoResult.CodigoRetorno != 5)
                    throw new ServicoException(retorno.ConsultarVeiculoResult.Mensagem);

                veiculoIntegracao.SituacaoIntegracao = SituacaoIntegracao.Integrado;
                veiculoIntegracao.ProblemaIntegracao = retorno.ConsultarVeiculoResult.Mensagem;
            }
            catch (BaseException excecao)
            {
                veiculoIntegracao.SituacaoIntegracao = SituacaoIntegracao.ProblemaIntegracao;
                veiculoIntegracao.ProblemaIntegracao = excecao.Message;
            }
            catch (Exception excecao)
            {
                Log.TratarErro(excecao, "BrasilRisk");

                veiculoIntegracao.SituacaoIntegracao = SituacaoIntegracao.ProblemaIntegracao;
                veiculoIntegracao.ProblemaIntegracao = "Ocorreu um problema ao integrar com Brasil Risk.";
            }
            finally
            {
                servicoArquivoTransacao.Adicionar(veiculoIntegracao, inspector.LastRequestXML, inspector.LastResponseXML, "xml", TipoArquivoIntegracaoCTeCarga.RetornoDoProcessamento);
                repositorioVeiculoIntegracao.Atualizar(veiculoIntegracao);
            }
        }

        #endregion Métodos Públicos

        #region Metodos Privados

        private static void IntegrarCargaBrasilRisk(Dominio.Entidades.Embarcador.Cargas.CargaCargaIntegracao cargaIntegracao, bool SMLCargaVazia, Repositorio.UnitOfWork unitOfWork)
        {
            Repositorio.Embarcador.Cargas.CargaCargaIntegracao repositorioCargaIntegracao = new Repositorio.Embarcador.Cargas.CargaCargaIntegracao(unitOfWork);
            ArquivoTransacao<Dominio.Entidades.Embarcador.Cargas.CargaCTeIntegracaoArquivo> servicoArquivoTransacao = new ArquivoTransacao<Dominio.Entidades.Embarcador.Cargas.CargaCTeIntegracaoArquivo>(unitOfWork);

            cargaIntegracao.NumeroTentativas += 1;
            cargaIntegracao.DataIntegracao = DateTime.Now;

            Servicos.Models.Integracao.InspectorBehavior inspector = new Servicos.Models.Integracao.InspectorBehavior(true);

            try
            {
                Dominio.ObjetosDeValor.Embarcador.Integracao.BrasilRisk.ConfiguracaoIntegracaoBrasilRisk configuracaoIntegracao = ObterConfiguracaoIntegracao(cargaIntegracao.Carga, unitOfWork);
                ServicoBrasilRisk.Monitoramento.ServiceSoapClient servicoMonitoramento = new Servicos.Embarcador.Integracao.ConfiguracaoWebService(unitOfWork).ObterClient<ServicoBrasilRisk.Monitoramento.ServiceSoapClient, ServicoBrasilRisk.Monitoramento.ServiceSoap>(Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoWebServiceIntegracao.BrasilRisk_Monitoramento, out inspector);

                servicoMonitoramento.Endpoint.Address = new System.ServiceModel.EndpointAddress(configuracaoIntegracao.Url);

                ServicoBrasilRisk.Monitoramento.RequisicaoMonitoramento requisicaoMonitoramento = ObterRequisicaoMonitoramento(cargaIntegracao.Carga, SMLCargaVazia, unitOfWork);
                ServicoBrasilRisk.Monitoramento.Credenciais credenciais = new ServicoBrasilRisk.Monitoramento.Credenciais()
                {
                    Usuario = configuracaoIntegracao.Usuario,
                    Senha = configuracaoIntegracao.Senha
                };

                ServicoBrasilRisk.Monitoramento.RetornoMonitoramento retorno = servicoMonitoramento.SolicitarMonitoramento(credenciais, requisicaoMonitoramento);
                bool retornoComProblemaIntegracao = retorno.StatusServico;

                if (retornoComProblemaIntegracao)
                {
                    cargaIntegracao.SituacaoIntegracao = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoIntegracao.ProblemaIntegracao;
                    cargaIntegracao.ProblemaIntegracao = retorno.Mensagem;
                }
                else
                {
                    cargaIntegracao.Protocolo = retorno.MonitoramentoId.ToString();
                    cargaIntegracao.SituacaoIntegracao = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoIntegracao.Integrado;
                    cargaIntegracao.ProblemaIntegracao = $"{retorno.Mensagem} - Protocolo {cargaIntegracao.Protocolo}.";
                }

                servicoArquivoTransacao.Adicionar(cargaIntegracao, inspector.LastRequestXML, inspector.LastResponseXML, "xml");
            }
            catch (ServicoException excecao)
            {
                cargaIntegracao.SituacaoIntegracao = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoIntegracao.ProblemaIntegracao;
                cargaIntegracao.ProblemaIntegracao = excecao.Message;
            }
            catch (Exception excecao)
            {
                Log.TratarErro(excecao);

                cargaIntegracao.SituacaoIntegracao = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoIntegracao.ProblemaIntegracao;
                cargaIntegracao.ProblemaIntegracao = "Ocorreu uma falha ao realizar a integração com a Brasil Risk";

                servicoArquivoTransacao.Adicionar(cargaIntegracao, inspector.LastRequestXML, inspector.LastResponseXML, "xml");
            }

            repositorioCargaIntegracao.Atualizar(cargaIntegracao);
        }

        private static void IntegrarCargaBrasilRiskSML(Dominio.Entidades.Embarcador.Cargas.CargaDadosTransporteIntegracao cargaIntegracao, bool SMLCargaVazia, Repositorio.UnitOfWork unitOfWork)
        {
            Repositorio.Embarcador.Cargas.CargaDadosTransporteIntegracao repositorioCargaDadosTransporteIntegracao = new Repositorio.Embarcador.Cargas.CargaDadosTransporteIntegracao(unitOfWork);
            ArquivoTransacao<Dominio.Entidades.Embarcador.Cargas.CargaCTeIntegracaoArquivo> servicoArquivoTransacao = new ArquivoTransacao<Dominio.Entidades.Embarcador.Cargas.CargaCTeIntegracaoArquivo>(unitOfWork);

            cargaIntegracao.NumeroTentativas += 1;
            cargaIntegracao.DataIntegracao = DateTime.Now;

            Servicos.Models.Integracao.InspectorBehavior inspector = new Servicos.Models.Integracao.InspectorBehavior(true);

            try
            {
                Dominio.ObjetosDeValor.Embarcador.Integracao.BrasilRisk.ConfiguracaoIntegracaoBrasilRisk configuracaoIntegracao = ObterConfiguracaoIntegracao(cargaIntegracao.Carga, unitOfWork);
                ServicoBrasilRisk.Monitoramento.ServiceSoapClient servicoMonitoramento = new Servicos.Embarcador.Integracao.ConfiguracaoWebService(unitOfWork).ObterClient<ServicoBrasilRisk.Monitoramento.ServiceSoapClient, ServicoBrasilRisk.Monitoramento.ServiceSoap>(Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoWebServiceIntegracao.BrasilRisk_Monitoramento, out inspector);

                servicoMonitoramento.Endpoint.Address = new System.ServiceModel.EndpointAddress(configuracaoIntegracao.Url);

                ServicoBrasilRisk.Monitoramento.RequisicaoMonitoramento requisicaoMonitoramento = ObterRequisicaoMonitoramento(cargaIntegracao.Carga, SMLCargaVazia, unitOfWork);
                ServicoBrasilRisk.Monitoramento.Credenciais credenciais = new ServicoBrasilRisk.Monitoramento.Credenciais()
                {
                    Usuario = configuracaoIntegracao.Usuario,
                    Senha = configuracaoIntegracao.Senha
                };

                ServicoBrasilRisk.Monitoramento.RetornoMonitoramento retorno = servicoMonitoramento.SolicitarMonitoramento(credenciais, requisicaoMonitoramento);
                bool retornoComProblemaIntegracao = retorno.StatusServico;

                if (retornoComProblemaIntegracao)
                {
                    cargaIntegracao.SituacaoIntegracao = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoIntegracao.ProblemaIntegracao;
                    cargaIntegracao.ProblemaIntegracao = retorno.Mensagem;
                }
                else
                {
                    cargaIntegracao.Protocolo = retorno.MonitoramentoId.ToString();
                    cargaIntegracao.SituacaoIntegracao = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoIntegracao.Integrado;
                    cargaIntegracao.ProblemaIntegracao = $"{retorno.Mensagem} - Protocolo {cargaIntegracao.Protocolo}.";
                }

                servicoArquivoTransacao.Adicionar(cargaIntegracao, inspector.LastRequestXML, inspector.LastResponseXML, "xml");
            }
            catch (ServicoException excecao)
            {
                cargaIntegracao.SituacaoIntegracao = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoIntegracao.ProblemaIntegracao;
                cargaIntegracao.ProblemaIntegracao = excecao.Message;
            }
            catch (Exception excecao)
            {
                Log.TratarErro(excecao);

                cargaIntegracao.SituacaoIntegracao = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoIntegracao.ProblemaIntegracao;
                cargaIntegracao.ProblemaIntegracao = "Ocorreu uma falha ao realizar a integração com a Brasil Risk";

                servicoArquivoTransacao.Adicionar(cargaIntegracao, inspector.LastRequestXML, inspector.LastResponseXML, "xml");
            }

            repositorioCargaDadosTransporteIntegracao.Atualizar(cargaIntegracao);
        }

        private static bool PermitirIntegrarCarga(Dominio.Entidades.Embarcador.Cargas.Carga carga)
        {
            return (
                (carga.ModeloVeicularCarga?.IntegrarDadosTransporteBrasilRiskAoAtualizarVeiculoNaCarga ?? false) &&
                (carga.TipoOperacao?.IntegrarDadosTransporteBrasilRiskAoAtualizarVeiculoMotorista ?? false) &&
                (carga.Veiculo != null) &&
                (carga.Motoristas?.Count > 0)
            );
        }

        private static Dominio.ObjetosDeValor.Embarcador.Integracao.BrasilRisk.ConfiguracaoIntegracaoBrasilRisk ObterConfiguracaoIntegracao(Dominio.Entidades.Embarcador.Cargas.Carga carga, Repositorio.UnitOfWork unitOfWork)
        {
            Repositorio.Embarcador.Configuracoes.Integracao repositorioConfiguracaoIntegracao = new Repositorio.Embarcador.Configuracoes.Integracao(unitOfWork);
            Dominio.Entidades.Embarcador.Configuracoes.Integracao configuracaoIntegracao = repositorioConfiguracaoIntegracao.Buscar();

            if ((configuracaoIntegracao == null) || !configuracaoIntegracao.PossuiIntegracaoBrasilRisk)
                throw new ServicoException("Não existe configuração de integração disponível para a Brasil Risk");

            string url = (carga.Empresa.TipoAmbiente == Dominio.Enumeradores.TipoAmbiente.Producao) ? configuracaoIntegracao.URLProducaoBrasilRisk : configuracaoIntegracao.URLHomologacaoBrasilRisk;

            if (string.IsNullOrWhiteSpace(url) || string.IsNullOrWhiteSpace(configuracaoIntegracao.UsuarioBrasilRisk) || string.IsNullOrWhiteSpace(configuracaoIntegracao.SenhaBrasilRisk))
                throw new ServicoException("Não existe configuração de integração disponível para a Brasil Risk");

            return new Dominio.ObjetosDeValor.Embarcador.Integracao.BrasilRisk.ConfiguracaoIntegracaoBrasilRisk
            {
                Url = url,
                Usuario = configuracaoIntegracao.UsuarioBrasilRisk,
                Senha = configuracaoIntegracao.SenhaBrasilRisk,
                EnviarTodosDestinos = (configuracaoIntegracao?.EnviarTodosDestinosBrasilRisk ?? false),
            };
        }

        private static ServicoBrasilRisk.Monitoramento.RequisicaoMonitoramento ObterRequisicaoMonitoramento(Dominio.Entidades.Embarcador.Cargas.Carga carga, bool SMLCargaVazia, Repositorio.UnitOfWork unitOfWork)
        {
            Repositorio.Embarcador.Configuracoes.Integracao repConfiguracaoIntegracao = new Repositorio.Embarcador.Configuracoes.Integracao(unitOfWork);
            Repositorio.Embarcador.Configuracoes.ConfiguracaoControleEntrega repositorioConfiguracaoControleEntrega = new Repositorio.Embarcador.Configuracoes.ConfiguracaoControleEntrega(unitOfWork);

            Dominio.Entidades.Embarcador.Configuracoes.ConfiguracaoControleEntrega configuracaoControleEntrega = repositorioConfiguracaoControleEntrega.BuscarPrimeiroRegistro();
            Dominio.Entidades.Embarcador.Configuracoes.Integracao configuracaoIntegracao = repConfiguracaoIntegracao.Buscar();
            Dominio.Entidades.Embarcador.Cargas.CargaPedido cargaPedido = carga.Pedidos.FirstOrDefault() ?? throw new ServicoException("Carga sem pedido vinculado.");
            List<Dominio.Entidades.Embarcador.Cargas.ControleEntrega.CargaEntrega> cargaEntregas = carga.Entregas?.ToList();
            Dominio.Entidades.Usuario motorista = carga.Motoristas.FirstOrDefault() ?? throw new ServicoException("Carga sem motorista vinculado.");

            bool calcularDataAgendamentoAutomaticamenteDataFaturamento = (configuracaoControleEntrega?.CalcularDataAgendamentoAutomaticamenteDataFaturamento ?? false);
            DateTime? dataPrevisaoInicio = configuracaoIntegracao.InicioViagemFixoHoraAtualMaisMinutos ? DateTime.Now.AddMinutes(configuracaoIntegracao.MinutosAMaisInicioViagem) : cargaPedido.Pedido.DataPrevisaoInicioViagem ?? DateTime.Now;
            DateTime? dataPrevisaoFim = cargaPedido.Pedido.PrevisaoEntrega;

            if (calcularDataAgendamentoAutomaticamenteDataFaturamento && cargaEntregas?.Count > 0)
                dataPrevisaoFim = cargaEntregas.Max(entrega => entrega.DataPrevista);

            if (dataPrevisaoInicio > dataPrevisaoFim)
                dataPrevisaoFim = dataPrevisaoInicio.Value.AddHours(24);

            ServicoBrasilRisk.Monitoramento.RequisicaoMonitoramento requisicaoMonitoramento = new ServicoBrasilRisk.Monitoramento.RequisicaoMonitoramento()
            {
                ControleCliente = carga.Codigo.ToString(),
                CnpjCliente = carga.TipoOperacao?.CNPJClienteBrasilRisk,
                CnpjTransportadora = !string.IsNullOrEmpty(carga.TipoOperacao?.CNPJTransportadoraBrasilRisk ?? "") ? carga.TipoOperacao.CNPJTransportadoraBrasilRisk : carga.Empresa?.CNPJ,
                CentroDeCusto = (SMLCargaVazia && calcularDataAgendamentoAutomaticamenteDataFaturamento) ? "PREVIAGEM" : carga.TipoOperacao?.CentroCustoBrasilRisk,
                Produto = (SMLCargaVazia && calcularDataAgendamentoAutomaticamenteDataFaturamento) ? "PREVIAGEM" : carga.TipoOperacao?.ProdutoBrasilRisk,
                CpfMotorista = motorista.CPF,
                PlacaVeiculo = carga.Veiculo?.Placa,
                PlacaCarreta = carga.VeiculosVinculados.FirstOrDefault()?.Placa,
                EnderecoOrigem = SMLCargaVazia ? "Av Cipriano Santos" : cargaPedido.Pedido.Remetente.Endereco,
                BairroOrigem = SMLCargaVazia ? "PATOS" : cargaPedido.Pedido.Remetente.Bairro,
                CepOrigem = SMLCargaVazia ? "78115200" : cargaPedido.Pedido.Remetente.CEP,
                NumeroOrigem = SMLCargaVazia ? "100" : cargaPedido.Pedido.Remetente.Endereco,
                CidadeOrigem = SMLCargaVazia ? "Belém" : cargaPedido.Pedido.Remetente.Localidade.Descricao,
                EstadoOrigem = SMLCargaVazia ? "PA" : cargaPedido.Pedido.Remetente.Localidade.Estado.Sigla,
                LocalOrigem = SMLCargaVazia ? "Belém" : cargaPedido.Pedido.Remetente.Localidade.Descricao,
                PrevisaoInicio = dataPrevisaoInicio ?? DateTime.Now,
                PrevisaoTermino = dataPrevisaoFim ?? DateTime.Now.AddDays(3),
                PedidoLogistico = carga.TipoOperacao?.PedidoLogisticoBrasilRisk ?? false
            };

            if (SMLCargaVazia)
            {
                requisicaoMonitoramento.ControleCliente = cargaPedido.Carga.Protocolo.ToString();
                requisicaoMonitoramento.PedidoLogistico = !calcularDataAgendamentoAutomaticamenteDataFaturamento;
            }

            if (carga.TipoOperacao?.EnviarNumeroPedidoEmbarcadorNoCodigoControleBrasilRisk ?? false)
                requisicaoMonitoramento.ControleCliente = cargaPedido.Pedido.NumeroPedidoEmbarcador;

            requisicaoMonitoramento.Destinos = new ServicoBrasilRisk.Monitoramento.Destino[]
            {
                ObterRequisicaoMonitoramentoDestino(carga, cargaPedido, SMLCargaVazia, unitOfWork, calcularDataAgendamentoAutomaticamenteDataFaturamento)
            };

            return requisicaoMonitoramento;
        }

        private static ServicoBrasilRisk.Monitoramento.Destino ObterRequisicaoMonitoramentoDestino(Dominio.Entidades.Embarcador.Cargas.Carga carga, Dominio.Entidades.Embarcador.Cargas.CargaPedido cargaPedido, bool SMLCargaVazia, Repositorio.UnitOfWork unitOfWork, bool calcularDataAgendamentoAutomaticamenteDataFaturamento)
        {
            if (SMLCargaVazia)
                return new ServicoBrasilRisk.Monitoramento.Destino()
                {
                    Peso = 1000,
                    Valor = calcularDataAgendamentoAutomaticamenteDataFaturamento ? 150000 : 10000,
                    Documento1 = "10",
                    SerieNF = null,
                    CEP = null,
                    UF = "AC",
                    Cidade = "Rio Branco",
                    LocalDestino = "RIO BRANCO",
                    Logradouro = null,
                    Numero = null,
                    Bairro = null,
                    NomeCliente = null,
                    Latitude = "-9.9765362",
                    Longitude = "-67.8220778"
                };

            Dominio.Entidades.Cliente destinatario = cargaPedido.Recebedor ?? cargaPedido.Pedido.Destinatario;

            if (destinatario == null)
                return new ServicoBrasilRisk.Monitoramento.Destino();

            Repositorio.Embarcador.Pedidos.PedidoXMLNotaFiscal repositorioPedidoXMLNotaFiscal = new Repositorio.Embarcador.Pedidos.PedidoXMLNotaFiscal(unitOfWork);
            Dominio.Entidades.Embarcador.Pedidos.PedidoXMLNotaFiscal primeiroPedidoXMLNotaFiscal = cargaPedido.NotasFiscais.FirstOrDefault();
            decimal peso = repositorioPedidoXMLNotaFiscal.ObterPesoTotalPorCarga(carga.Codigo);
            decimal valorTotalNotas = repositorioPedidoXMLNotaFiscal.ObterValorTotalPorCarga(carga.Codigo);
            int numeroPrimeiraNota = primeiroPedidoXMLNotaFiscal?.XMLNotaFiscal.Numero ?? 0;
            string seriePrimeiraNota = primeiroPedidoXMLNotaFiscal?.XMLNotaFiscal.SerieOuSerieDaChave;

            return new ServicoBrasilRisk.Monitoramento.Destino()
            {
                Peso = (double)peso,
                Valor = valorTotalNotas,
                Documento1 = numeroPrimeiraNota.ToString(),
                SerieNF = seriePrimeiraNota,
                CEP = Utilidades.String.OnlyNumbers(destinatario.CEP ?? ""),
                UF = destinatario.Localidade.Estado.Sigla ?? "",
                Cidade = destinatario.Localidade.Descricao ?? "",
                LocalDestino = destinatario.Localidade.Descricao ?? "",
                Logradouro = destinatario.Endereco ?? "",
                Numero = destinatario.Numero ?? "",
                Bairro = destinatario.Bairro ?? "",
                NomeCliente = destinatario.Nome ?? "",
                Latitude = destinatario.Latitude ?? "",
                Longitude = destinatario.Longitude ?? ""
            };
        }

        private static string PermitirEnviarDadosTransportadoraSubContratada(Dominio.Entidades.Embarcador.Cargas.CargaCargaIntegracao cargaIntegracao, Repositorio.UnitOfWork unitOfWork)
        {
            Repositorio.Embarcador.Configuracoes.Integracao repositorioIntegracao = new Repositorio.Embarcador.Configuracoes.Integracao(unitOfWork);

            Dominio.Entidades.Embarcador.Configuracoes.Integracao integracao = repositorioIntegracao.BuscarPrimeiroRegistro();
            Dominio.Entidades.Veiculo veiculo = cargaIntegracao.Carga?.Veiculo;

            if (!(integracao.EnviarDadosTransportadoraSubContratadaNasObservacoes))
                return null;

            if (veiculo?.Proprietario == null || veiculo.TipoVeiculo == "1")
                return null;

            Dominio.Entidades.Cliente proprietario = veiculo.Proprietario;

            bool isCNPJ = Utilidades.Validate.ValidarCNPJ(proprietario.CPF_CNPJ_Formatado);

            string labelCPFCNPJ = isCNPJ ? "CNPJ:" : "CPF:";

            return $"Nome do Subcontratado: {proprietario.Nome} - {labelCPFCNPJ}: {proprietario.CPF_CNPJ_Formatado} - TELEFONE: {proprietario.Telefone1} - E-MAIL: {proprietario.Email}";
        }

        private static void IntegrarRotaBrasilRisk(Dominio.Entidades.Embarcador.Cargas.CargaCargaIntegracao cargaIntegracao, Repositorio.UnitOfWork unitOfWork)
        {
            ArquivoTransacao<Dominio.Entidades.Embarcador.Cargas.CargaCTeIntegracaoArquivo> servicoArquivoTransacao = new ArquivoTransacao<Dominio.Entidades.Embarcador.Cargas.CargaCTeIntegracaoArquivo>(unitOfWork);
            Models.Integracao.InspectorBehavior inspector = new Models.Integracao.InspectorBehavior(true);

            try
            {
                Repositorio.RotaFrete repositorioRotaFrete = new Repositorio.RotaFrete(unitOfWork);
                Repositorio.RotaFretePontosPassagem repositorioRotaFretePontosPassagem = new Repositorio.RotaFretePontosPassagem(unitOfWork);
                Repositorio.Embarcador.Cargas.CargaRotaFrete repCargaRotaFrete = new Repositorio.Embarcador.Cargas.CargaRotaFrete(unitOfWork);

                Dominio.Entidades.RotaFrete rotaFrete = repositorioRotaFrete.BuscarPorCodigo(cargaIntegracao.Carga.Rota?.Codigo ?? 0);
                Dominio.Entidades.Embarcador.Cargas.CargaRotaFrete cargaRotaFrete = repCargaRotaFrete.BuscarPorCarga(cargaIntegracao.Carga.Codigo);

                Dominio.ObjetosDeValor.Embarcador.Integracao.BrasilRisk.ConfiguracaoIntegracaoBrasilRisk configuracaoIntegracao = ObterConfiguracaoIntegracao(cargaIntegracao.Carga, unitOfWork);

                if (rotaFrete == null)
                    return;

                if (!string.IsNullOrEmpty(rotaFrete.CodigoIntegracaoGerenciadoraRisco))
                    return;

                if (string.IsNullOrEmpty(cargaRotaFrete.PolilinhaRota))
                    return;

                List<Dominio.Entidades.RotaFretePontosPassagem> pontosPassagem = repositorioRotaFretePontosPassagem.BuscarPorRotaFrete(rotaFrete.Codigo);

                if (pontosPassagem == null || pontosPassagem.Count == 0)
                    return;

                Dominio.Entidades.Localidade localidadeOrigem = null;
                Dominio.Entidades.Localidade localidadeDestino = null;

                localidadeOrigem = ObterLocalidade(pontosPassagem.FirstOrDefault());
                localidadeDestino = ObterLocalidade(pontosPassagem.LastOrDefault());

                ServicosBrasilRisk.ServiceSoapClient servicoBrasilRisk = ObterClientIntegrarRota(configuracaoIntegracao.Url, configuracaoIntegracao.Usuario, configuracaoIntegracao.Senha);
                servicoBrasilRisk.Endpoint.EndpointBehaviors.Add(inspector);

                ServicosBrasilRisk.CriarRotaRequest1 requisicaoCriarRota = new ServicosBrasilRisk.CriarRotaRequest1
                {
                    Credenciais = ObterDadosCredenciaisBrasilRisk(configuracaoIntegracao),
                    dados = ObterDadosCriarRotaRequest(rotaFrete, localidadeOrigem, localidadeDestino, cargaRotaFrete.PolilinhaRota)
                };

                ServicosBrasilRisk.CriarRotaResponse1 retorno = servicoBrasilRisk.CriarRota(requisicaoCriarRota);

                if (retorno.CriarRotaResult.Sucesso)
                {
                    rotaFrete.CodigoIntegracaoGerenciadoraRisco = retorno.CriarRotaResult.CodRota.ToString();
                    repositorioRotaFrete.Atualizar(rotaFrete);
                }
            }
            catch (Exception excecao)
            {
                Log.TratarErro(excecao, "IntegrarRotaBrasilRisk");
            }

            servicoArquivoTransacao.Adicionar(cargaIntegracao, inspector.LastRequestXML, inspector.LastResponseXML, "xml", "Criar Rota");
        }

        private static ServicosBrasilRisk.ServiceSoapClient ObterClientIntegrarRota(string url, string usuario, string senha)
        {
            ServicosBrasilRisk.ServiceSoapClient cliente = null;
            System.ServiceModel.EndpointAddress endpointAddress = new System.ServiceModel.EndpointAddress(url);

            if (url.StartsWith("https"))
            {
                System.ServiceModel.BasicHttpBinding binding = new System.ServiceModel.BasicHttpBinding();

                binding.MaxReceivedMessageSize = int.MaxValue;
                binding.ReceiveTimeout = new TimeSpan(0, 5, 0);
                binding.Security.Mode = System.ServiceModel.BasicHttpSecurityMode.Transport;
                binding.Security.Transport.ClientCredentialType = System.ServiceModel.HttpClientCredentialType.Basic;
                cliente = new ServicosBrasilRisk.ServiceSoapClient(binding, endpointAddress);
            }
            else
            {
                System.ServiceModel.BasicHttpBinding binding = new System.ServiceModel.BasicHttpBinding();

                binding.MaxReceivedMessageSize = int.MaxValue;
                binding.ReceiveTimeout = new TimeSpan(0, 5, 0);
                cliente = new ServicosBrasilRisk.ServiceSoapClient(binding, endpointAddress);
            }

            cliente.ClientCredentials.UserName.UserName = usuario;
            cliente.ClientCredentials.UserName.Password = senha;

            return cliente;
        }

        private static Dominio.Entidades.Localidade ObterLocalidade(Dominio.Entidades.RotaFretePontosPassagem cargaRotaFretePontosPassagem)
        {
            if (cargaRotaFretePontosPassagem == null)
                return null;

            if (cargaRotaFretePontosPassagem.ClienteOutroEndereco != null)
                return cargaRotaFretePontosPassagem.ClienteOutroEndereco.Localidade;

            if (cargaRotaFretePontosPassagem.Cliente != null)
                return cargaRotaFretePontosPassagem.Cliente.Localidade;

            if (cargaRotaFretePontosPassagem.Localidade != null)
                return cargaRotaFretePontosPassagem.Localidade;

            return null;
        }

        private static ServicosBrasilRisk.Credenciais ObterDadosCredenciaisBrasilRisk(Dominio.ObjetosDeValor.Embarcador.Integracao.BrasilRisk.ConfiguracaoIntegracaoBrasilRisk configuracaoIntegracao)
        {
            return new ServicosBrasilRisk.Credenciais()
            {
                Usuario = configuracaoIntegracao.Usuario,
                Senha = configuracaoIntegracao.Senha
            };
        }

        private static ServicoBrasilRisk.GestaoAnaliseDePerfil.Credenciais ObterDadosCredenciaisBrasilRisk(Dominio.Entidades.Embarcador.Configuracoes.Integracao configuracaoIntegracao)
        {
            return new Servicos.ServicoBrasilRisk.GestaoAnaliseDePerfil.Credenciais()
            {
                Usuario = configuracaoIntegracao.UsuarioBrasilRisk,
                Senha = configuracaoIntegracao.SenhaBrasilRisk
            };
        }

        private static ServicosBrasilRisk.CriarRotaRequest ObterDadosCriarRotaRequest(Dominio.Entidades.RotaFrete rotaFrete, Dominio.Entidades.Localidade localidadeOrigem, Dominio.Entidades.Localidade localidadeDestino, string polilinha)
        {
            return new ServicosBrasilRisk.CriarRotaRequest
            {
                Polyline = polilinha,
                NomeOrigem = localidadeOrigem.DescricaoCidadeEstado,
                PaisOrigem = localidadeOrigem.Pais.Nome,
                UFOrigem = localidadeOrigem.Estado.Sigla,
                CidadeOrigem = localidadeOrigem.Descricao,
                LatitudeOrigem = localidadeOrigem.Latitude.ToString(),
                LongitudeOrigem = localidadeOrigem.Longitude.ToString(),
                NomeDestino = localidadeDestino.DescricaoCidadeEstado,
                PaisDestino = localidadeDestino.Pais.Nome,
                UFDestino = localidadeDestino.Estado.Sigla,
                CidadeDestino = localidadeDestino.Descricao,
                LatitudeDestino = localidadeDestino.Latitude.ToString(),
                LongitudeDestino = localidadeDestino.Longitude.ToString(),
                NomeRota = rotaFrete.Descricao
            };
        }

        private static ServicoBrasilRisk.GestaoAnaliseDePerfil.CadastrarMotoristaRequest ObterDadosRequestAnalisePerfilCadastroMotorista(Dominio.Entidades.Embarcador.Configuracoes.Integracao configuracaoIntegracao, Dominio.Entidades.Usuario motorista)
        {
            if (string.IsNullOrWhiteSpace(configuracaoIntegracao.CNPJEmbarcadorBrasilRisk))
                throw new ServicoException("É necessário informar o CNPJ da empresa de vínculo Brasil Risk.");

            ServicoBrasilRisk.GestaoAnaliseDePerfil.CadastrarMotoristaRequest cadastrarMotoristaRequest = new ServicoBrasilRisk.GestaoAnaliseDePerfil.CadastrarMotoristaRequest
            {
                Credenciais = ObterDadosCredenciaisBrasilRisk(configuracaoIntegracao),
                motorista = new ServicoBrasilRisk.GestaoAnaliseDePerfil.Motorista
                {
                    Funcao = motorista.Cargo,
                    Perfil = "FR",
                    DataCriacao = motorista.DataCadastro,
                    Nome = motorista.Nome,
                    CPF = Utilidades.String.OnlyNumbers(motorista.CPF).ToDouble(),
                    RG = motorista.RG,
                    OrgaoExp = motorista.OrgaoEmissorRG?.ObterDescricao(),
                    DataEmissao = motorista.DataEmissaoRG,
                    DataNascimento = motorista.DataNascimento,
                    NomePai = motorista.FiliacaoMotoristaPai,
                    NomeMae = motorista.FiliacaoMotoristaMae,
                    Endereco = motorista.Endereco,
                    Numero = Utilidades.String.OnlyNumbers(motorista.NumeroEndereco).ToInt(),
                    Complemento = motorista.Complemento,
                    Bairro = motorista.Bairro,
                    CEP = Utilidades.String.OnlyNumbers(motorista.CEP),
                    Telefone = motorista.Telefone,
                    Celular = motorista.Celular,
                    CnpjEmpresaDeVinculo = Utilidades.String.OnlyNumbers(configuracaoIntegracao.CNPJEmbarcadorBrasilRisk),
                    CidadeOrigem = motorista.Localidade?.Descricao,
                    EstadoOrigem = motorista.Localidade?.Estado?.Sigla,
                    DataValidade = motorista.DataValidadeGR,
                    GR = motorista.CNPJEmbarcador
                }
            };

            if (!string.IsNullOrEmpty(motorista.Cargo) && motorista.Cargo != "A")
            {
                cadastrarMotoristaRequest.motorista.CNHRegistro = motorista.NumeroRegistroHabilitacao;
                cadastrarMotoristaRequest.motorista.CNHEmissao = motorista.DataHabilitacao.ToDateTimeString();
                cadastrarMotoristaRequest.motorista.CNHCategoria = motorista.Categoria;
                cadastrarMotoristaRequest.motorista.CNHValidade = motorista.DataVencimentoHabilitacao;
                cadastrarMotoristaRequest.motorista.CNHUf = motorista.UFEmissaoCNH?.Sigla;
            }

            return cadastrarMotoristaRequest;
        }

        private static ServicoBrasilRisk.GestaoAnaliseDePerfil.ConsultarMotoristaRequest ObterDadosRequestAnalisePerfilConsultarStatusMotorista(Dominio.Entidades.Embarcador.Configuracoes.Integracao configuracaoIntegracao, Dominio.Entidades.Usuario motorista)
        {
            ServicoBrasilRisk.GestaoAnaliseDePerfil.ConsultarMotoristaRequest consultarMotoristaRequest = new ServicoBrasilRisk.GestaoAnaliseDePerfil.ConsultarMotoristaRequest
            {
                Credenciais = ObterDadosCredenciaisBrasilRisk(configuracaoIntegracao),
                cpf = Utilidades.String.OnlyNumbers(motorista.CPF)
            };

            return consultarMotoristaRequest;
        }

        private static ServicoBrasilRisk.GestaoAnaliseDePerfil.ConsultarVeiculoRequest ObterDadosRequestAnalisePerfilConsultarStatusVeiculo(Dominio.Entidades.Embarcador.Configuracoes.Integracao configuracaoIntegracao, Dominio.Entidades.Veiculo veiculo)
        {
            ServicoBrasilRisk.GestaoAnaliseDePerfil.ConsultarVeiculoRequest consultarMotoristaRequest = new ServicoBrasilRisk.GestaoAnaliseDePerfil.ConsultarVeiculoRequest
            {
                Credenciais = ObterDadosCredenciaisBrasilRisk(configuracaoIntegracao),
                placa = veiculo.Placa
            };

            return consultarMotoristaRequest;
        }

        private static ServicoBrasilRisk.GestaoAnaliseDePerfil.CadastrarVeiculoRequest ObterDadosRequestAnalisePerfilVeiculo(Dominio.Entidades.Embarcador.Configuracoes.Integracao configuracaoIntegracao, Dominio.Entidades.Veiculo veiculo)
        {
            if (string.IsNullOrWhiteSpace(configuracaoIntegracao.CNPJEmbarcadorBrasilRisk))
                throw new ServicoException("É necessário informar o CNPJ da empresa de vínculo Brasil Risk.");

            ServicoBrasilRisk.GestaoAnaliseDePerfil.CadastrarVeiculoRequest cadastrarVeiculoRequest = new ServicoBrasilRisk.GestaoAnaliseDePerfil.CadastrarVeiculoRequest
            {
                Credenciais = ObterDadosCredenciaisBrasilRisk(configuracaoIntegracao),
                veiculo = new ServicoBrasilRisk.GestaoAnaliseDePerfil.Veiculo
                {
                    Placa = veiculo.Placa,
                    Renavam = veiculo.Renavam,
                    Chassi = veiculo.Chassi,
                    AnoFabricacao = veiculo.AnoFabricacao,
                    Marca = veiculo.DescricaoMarca,
                    Modelo = veiculo.DescricaoModelo,
                    Cor = veiculo.CorVeiculo?.Descricao,
                    StatusEquipamentoRastreamento = "F",
                    NumeroTerminal = veiculo.NumeroEquipamentoRastreador,
                    TecnologiaRastreamento = veiculo.TecnologiaRastreador?.Descricao,
                    TipoVeiculo = veiculo.DescricaoTipoRodado,
                    CnpjEmpresaDeVinculo = Utilidades.String.OnlyNumbers(configuracaoIntegracao.CNPJEmbarcadorBrasilRisk),
                }
            };

            if (veiculo.Tipo == "T") // TERCEIRO
            {
                bool proprietarioPessoaFisica = veiculo.Proprietario?.Tipo == "F";

                cadastrarVeiculoRequest.veiculo.TipoProprietario = proprietarioPessoaFisica ? ServicoBrasilRisk.GestaoAnaliseDePerfil.TipoProprietario.Fisica : ServicoBrasilRisk.GestaoAnaliseDePerfil.TipoProprietario.Juridica;
                cadastrarVeiculoRequest.veiculo.ProprietarioNome = veiculo.Proprietario?.Nome;
                cadastrarVeiculoRequest.veiculo.ProprietarioCNPJ_CPF = veiculo.Proprietario?.CPF_CNPJ_SemFormato.ToDouble();
                cadastrarVeiculoRequest.veiculo.ProprietarioIE_RG = veiculo.Proprietario?.IE_RG;

                if (proprietarioPessoaFisica)
                {
                    cadastrarVeiculoRequest.veiculo.ProprietarioRG_UF = veiculo.Proprietario?.EstadoRG?.Sigla;
                    cadastrarVeiculoRequest.veiculo.ProprietarioDataNascimento = veiculo.Proprietario?.DataNascimento;
                }

                cadastrarVeiculoRequest.veiculo.CidadeOrigem = veiculo.Proprietario?.Localidade.Descricao;
                cadastrarVeiculoRequest.veiculo.EstadoOrigem = veiculo.Proprietario?.Localidade.Estado.Sigla;
            }
            else
            {
                cadastrarVeiculoRequest.veiculo.TipoProprietario = ServicoBrasilRisk.GestaoAnaliseDePerfil.TipoProprietario.Juridica;
                cadastrarVeiculoRequest.veiculo.ProprietarioNome = veiculo.Empresa?.RazaoSocial;
                cadastrarVeiculoRequest.veiculo.ProprietarioCNPJ_CPF = veiculo.Empresa?.CNPJ_SemFormato.ToDouble();
                cadastrarVeiculoRequest.veiculo.ProprietarioIE_RG = veiculo.Empresa?.InscricaoEstadual;
                cadastrarVeiculoRequest.veiculo.CidadeOrigem = veiculo.Empresa?.Localidade.Descricao;
                cadastrarVeiculoRequest.veiculo.EstadoOrigem = veiculo.Empresa?.Localidade.Estado.Sigla;
            }

            return cadastrarVeiculoRequest;
        }

        #endregion Metodos Privados

        public static Dominio.ObjetosDeValor.Embarcador.Integracao.BrasilRisk.Rotograma.RetornoRotogramaBrk ObterRotogramaBrk(
                      Dominio.Entidades.Embarcador.Cargas.CargaCargaIntegracao cargaIntegracao,
                      AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware tipoServicoMultisoftware,
                      Repositorio.UnitOfWork unitOfWork)
        {
            ArquivoTransacao<Dominio.Entidades.Embarcador.Cargas.CargaCTeIntegracaoArquivo> servicoArquivoTransacao =
                new ArquivoTransacao<Dominio.Entidades.Embarcador.Cargas.CargaCTeIntegracaoArquivo>(unitOfWork);
            Models.Integracao.InspectorBehavior inspector = new Models.Integracao.InspectorBehavior(true);

            try
            {
                var configuracaoIntegracao = ObterConfiguracaoIntegracao(cargaIntegracao.Carga, unitOfWork);

                if (string.IsNullOrEmpty(cargaIntegracao.Protocolo))
                {
                    return new Dominio.ObjetosDeValor.Embarcador.Integracao.BrasilRisk.Rotograma.RetornoRotogramaBrk
                    {
                        status = false,
                        mensagemErro = "Protocolo da viagem não informado"
                    };
                }

                var xmlRequest = $@"<?xml version=""1.0"" encoding=""utf-8""?>
                                    <soap:Envelope xmlns:xsi=""http://www.w3.org/2001/XMLSchema-instance"" 
                                                  xmlns:xsd=""http://www.w3.org/2001/XMLSchema"" 
                                                  xmlns:soap=""http://schemas.xmlsoap.org/soap/envelope/"">
                                        <soap:Header>
                                            <ValidationSoapHeader xmlns=""http://tempuri.org"">
                                                <userCod>{configuracaoIntegracao.Usuario}</userCod>
                                                <userPwd>{configuracaoIntegracao.Senha}</userPwd>
                                            </ValidationSoapHeader>
                                        </soap:Header>
                                        <soap:Body>
                                            <GetRotograma xmlns=""http://tempuri.org"">
                                                <Viagem>{cargaIntegracao.Protocolo}</Viagem>
                                            </GetRotograma>
                                        </soap:Body>
                                    </soap:Envelope>";

                var httpClient = HttpClientFactoryWrapper.GetClient(nameof(IntegracaoBrasilRisk));

                var request = new HttpRequestMessage(HttpMethod.Post, configuracaoIntegracao.Url);
                request.Content = new StringContent(xmlRequest, Encoding.UTF8, "text/xml");
                request.Headers.Add("SOAPAction", "http://tempuri.org/GetRotograma");

                var response = httpClient.SendAsync(request).Result;
                var responseContent = response.Content.ReadAsStringAsync().Result;

                if (!response.IsSuccessStatusCode)
                {
                    return new Dominio.ObjetosDeValor.Embarcador.Integracao.BrasilRisk.Rotograma.RetornoRotogramaBrk
                    {
                        status = false,
                        mensagemErro = $"Erro na requisição: {response.StatusCode} - {responseContent}"
                    };
                }

                var xmlDoc = XDocument.Parse(responseContent);
                var result = xmlDoc.Descendants()
                    .FirstOrDefault(x => x.Name.LocalName == "GetRotogramaResult");

                if (result != null)
                {
                    var codigo = result.Elements()
                        .FirstOrDefault(x => x.Name.LocalName == "codigo")?.Value;
                    var descricao = result.Elements()
                        .FirstOrDefault(x => x.Name.LocalName == "descricao")?.Value;

                    if (codigo == "200")
                    {
                        return new Dominio.ObjetosDeValor.Embarcador.Integracao.BrasilRisk.Rotograma.RetornoRotogramaBrk
                        {
                            status = true,
                            Base64 = descricao,
                            mensagemErro = string.Empty
                        };
                    }

                    return new Dominio.ObjetosDeValor.Embarcador.Integracao.BrasilRisk.Rotograma.RetornoRotogramaBrk
                    {
                        status = false,
                        mensagemErro = descricao ?? "Erro ao obter rotograma"
                    };
                }

                return new Dominio.ObjetosDeValor.Embarcador.Integracao.BrasilRisk.Rotograma.RetornoRotogramaBrk
                {
                    status = false,
                    mensagemErro = "Resposta inválida do servidor"
                };
            }
            catch (ServicoException excecao)
            {
                Log.TratarErro(excecao, "BrasilRisk");
                return new Dominio.ObjetosDeValor.Embarcador.Integracao.BrasilRisk.Rotograma.RetornoRotogramaBrk
                {
                    status = false,
                    mensagemErro = excecao.Message
                };
            }
            catch (Exception excecao)
            {
                Log.TratarErro(excecao, "BrasilRisk");
                return new Dominio.ObjetosDeValor.Embarcador.Integracao.BrasilRisk.Rotograma.RetornoRotogramaBrk
                {
                    status = false,
                    mensagemErro = "Ocorreu uma falha ao obter rotograma da Brasil Risk"
                };
            }
        }
    }
}

