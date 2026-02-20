using System;
using System.Linq;

namespace Servicos.Embarcador.Integracao.NOX
{
    public class IntegracaoNOX
    {
        public static string[] CodigosSucesso = new string[]
        {
             "OK0001",
             "OK0002",
             "OK0003",
             "OK0004",
             "OK0005",
             "OK0006",
             "OK0007",
             "OK0008",
             "OK0009",
             "OK0010",
             "OK0103"
        };

        #region Métodos Globais


        public static void IntegrarCargaDadosTransporte(Dominio.Entidades.Embarcador.Cargas.CargaDadosTransporteIntegracao cargaDadosTransporteIntegracao, Repositorio.UnitOfWork unitOfWork) 
        {
            Repositorio.Embarcador.Configuracoes.Integracao repConfiguracaoIntegracao = new Repositorio.Embarcador.Configuracoes.Integracao(unitOfWork);
            Repositorio.Embarcador.Cargas.CargaDadosTransporteIntegracao repCargaDadosTransporteIntegracao = new Repositorio.Embarcador.Cargas.CargaDadosTransporteIntegracao(unitOfWork);
            Repositorio.Embarcador.Cargas.CargaCTeIntegracaoArquivo repCargaCTeIntegracaoArquivo = new Repositorio.Embarcador.Cargas.CargaCTeIntegracaoArquivo(unitOfWork);


            Dominio.Entidades.Embarcador.Configuracoes.Integracao configuracaoIntegracao = repConfiguracaoIntegracao.Buscar();

            cargaDadosTransporteIntegracao.NumeroTentativas += 1;
            cargaDadosTransporteIntegracao.DataIntegracao = DateTime.Now;

            if (configuracaoIntegracao == null || !configuracaoIntegracao.PossuiIntegracaoNOX || string.IsNullOrWhiteSpace(configuracaoIntegracao.UsuarioNOX) || string.IsNullOrWhiteSpace(configuracaoIntegracao.SenhaNOX) || string.IsNullOrWhiteSpace(configuracaoIntegracao.TokenNOX) || string.IsNullOrWhiteSpace(configuracaoIntegracao.URLProducaoNOX))
            {
                cargaDadosTransporteIntegracao.SituacaoIntegracao = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoIntegracao.ProblemaIntegracao;
                cargaDadosTransporteIntegracao.ProblemaIntegracao = "Não existe configuração de integração disponível para a NOX.";

                repCargaDadosTransporteIntegracao.Atualizar(cargaDadosTransporteIntegracao);

                return;
            }
            string urlWebService = configuracaoIntegracao.URLHomologacaoNOX;

            if (cargaDadosTransporteIntegracao.Carga.Empresa.TipoAmbiente == Dominio.Enumeradores.TipoAmbiente.Producao)
                urlWebService = configuracaoIntegracao.URLProducaoNOX;

            string mensagem = string.Empty;
            string request = string.Empty;
            string response = string.Empty;

            Servicos.Models.Integracao.InspectorBehavior inspector = new Servicos.Models.Integracao.InspectorBehavior();
            try
            {
                Dominio.Entidades.Embarcador.Cargas.CargaPedido cargaPedido = cargaDadosTransporteIntegracao.Carga.Pedidos.FirstOrDefault();
                Dominio.Entidades.Usuario condutorPrincipal = cargaDadosTransporteIntegracao.Carga.Motoristas.FirstOrDefault();
                Dominio.Entidades.Usuario condutorAuxiliar = cargaDadosTransporteIntegracao.Carga.Motoristas.Count > 1 ? cargaDadosTransporteIntegracao.Carga.Motoristas[1] : null;
                ServicoNOX.IntegraGRSoapTypeClient svcMonitoramento = new Servicos.Embarcador.Integracao.ConfiguracaoWebService(unitOfWork).ObterClient<ServicoNOX.IntegraGRSoapTypeClient, ServicoNOX.IntegraGRSoapType>(Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoWebServiceIntegracao.NOX_IntegraGR, urlWebService, out inspector);

                ServicoNOX.stLogin login = new ServicoNOX.stLogin()
                {
                    sUserName = configuracaoIntegracao.UsuarioNOX,
                    sPassWord = configuracaoIntegracao.SenhaNOX,
                    sToken = configuracaoIntegracao.TokenNOX
                };


                decimal valorTotalNotas = 0;

                ServicoNOX.ArrayOfString veiculos = new ServicoNOX.ArrayOfString();
                int idFormacaoCVC = 0;
                if (cargaDadosTransporteIntegracao.Carga.Veiculo != null)
                    idFormacaoCVC = cargaDadosTransporteIntegracao.Carga.Veiculo.ModeloVeicularCarga?.CodigoIntegracaoGerenciadoraRisco?.ToInt() ?? 0;

                if (cargaDadosTransporteIntegracao.Carga.VeiculosVinculados.Any())
                    veiculos.Add(cargaDadosTransporteIntegracao.Carga.VeiculosVinculados.First().Placa_Formatada);

                string codigoProduto = cargaPedido.Pedido?.Produtos?.Select(o => o.Produto?.CodigoProdutoEmbarcador).FirstOrDefault() ?? "1";

                Dominio.Entidades.Cliente clienteOrigem = null;
                Dominio.Entidades.Cliente clienteDestino = null;

                if (cargaPedido.PontoPartida != null)
                {
                    clienteOrigem = cargaPedido.PontoPartida;
                    clienteDestino = cargaPedido.Expedidor ?? cargaPedido.Pedido.Expedidor ?? cargaPedido.Pedido.Remetente;
                }
                else
                {
                    clienteOrigem = cargaPedido.Expedidor ?? cargaPedido.Pedido.Expedidor ?? cargaPedido.Pedido.Remetente;
                    clienteDestino = cargaPedido.Recebedor ?? cargaPedido.Pedido.Recebedor ?? cargaPedido.Pedido.Destinatario;
                }

                ServicoNOX.stDadosSM dadosMonitoramento = new ServicoNOX.stDadosSM()
                {
                    CondutoresViagem = new ServicoNOX.stCondutoresViagem()
                    {
                        sNo_CpfCondutorPrinc = condutorPrincipal?.CPF ?? string.Empty,
                        sNo_CpfCondutorAux = condutorAuxiliar?.CPF ?? string.Empty
                    },
                    ConjuntoViagem = new ServicoNOX.stConjuntoViagem()
                    {
                        iId_FormacaoCVC = idFormacaoCVC,
                        sCd_PlacaCarretaCV = veiculos
                    },
                    DadosViagem = new ServicoNOX.stDadosViagem()
                    {
                        sCd_Rota = cargaDadosTransporteIntegracao.Carga.Rota?.Codigo.ToString() ?? string.Empty,
                        sCd_MunicipioOrigem = (cargaPedido.Origem?.CodigoIBGE ?? 0).ToString("D7"),
                        sCd_MunicipioDestino = !string.IsNullOrEmpty(cargaPedido.Destino?.CodigoIntegracao) ? cargaPedido.Destino.CodigoIntegracao : (cargaPedido.Destino?.CodigoIBGE != null ? cargaPedido.Destino.CodigoIBGE.ToString("D7") : "0000000"),
                        dDh_PrevInicio = cargaPedido.Pedido.DataPrevisaoSaida ?? DateTime.Now,
                        dDh_PrevFim = cargaPedido.Pedido.PrevisaoEntrega ?? DateTime.Now,
                        sId_OperTransp = cargaDadosTransporteIntegracao.Carga.TipoOperacao?.CodigoIntegracaoGerenciadoraRisco ?? string.Empty,
                        nVl_Carga = valorTotalNotas,
                        sCd_CnpjEmbarcViagem = cargaPedido.Pedido.Remetente.CPF_CNPJ_SemFormato,
                        EnderecoOrigem = new ServicoNOX.stEnderecos()
                        {
                            sCEP = Utilidades.String.OnlyNumbers(clienteOrigem?.CEP),
                            sNo_Endereco = string.IsNullOrWhiteSpace(clienteOrigem?.Numero) ? "S/N" : clienteOrigem.Numero,
                            sNm_Endereco = clienteOrigem?.Endereco ?? string.Empty,
                            sNm_Bairro = clienteOrigem?.Bairro ?? string.Empty
                        },
                        EnderecoDestino = new ServicoNOX.stEnderecos()
                        {
                            sCEP = Utilidades.String.OnlyNumbers(clienteDestino?.CEP),
                            sNo_Endereco = string.IsNullOrWhiteSpace(clienteDestino?.Numero) ? "S/N" : clienteDestino.Numero,
                            sNm_Endereco = clienteDestino?.Endereco ?? string.Empty,
                            sNm_Bairro = clienteDestino?.Bairro ?? string.Empty
                        },
                        DadosTranspTerceiro = new ServicoNOX.stDadosTranspTerceiro()
                        {
                            sCd_CnpjTransp = string.Empty,
                            sId_TranspTerceiro = string.Empty
                        },
                        SequenciaOperacao = (from pedido in cargaDadosTransporteIntegracao.Carga.Pedidos
                                             select new ServicoNOX.stSequenciaOperacao()
                                             {
                                                 dDh_PrevisaoChegada = cargaPedido.Pedido.PrevisaoEntrega ?? DateTime.Now,
                                                 nVl_Produto = 0,
                                                 sCd_CnpjEmbarcCliente = pedido.Pedido.Destinatario.CPF_CNPJ_SemFormato,
                                                 sCd_MunicipioOper = pedido.Pedido.Destino?.CodigoIBGE.ToString("D7") ?? string.Empty,
                                                 sCd_Produto = codigoProduto,
                                                 sDc_LocalOperacao = string.Empty,
                                                 sId_DetNF = "N",
                                                 sId_Operacao = "E",
                                                 iNo_NotaFiscal = new ServicoNOX.ArrayOfInt(),
                                                 sCEP = Utilidades.String.OnlyNumbers(pedido.Pedido.Recebedor?.CEP ?? pedido.Pedido.Destinatario?.CEP),
                                                 sNm_Bairro = pedido.Pedido.Recebedor?.Bairro ?? pedido.Pedido.Destinatario?.Bairro ?? string.Empty,
                                                 sNm_Endereco = pedido.Pedido.Recebedor?.Endereco ?? pedido.Pedido.Destinatario?.Endereco ?? string.Empty,
                                                 sNo_Endereco = pedido.Pedido.Recebedor?.Numero ?? pedido.Pedido.Destinatario?.Numero ?? string.Empty
                                             }).ToArray(),
                        DadosObsViagem = new ServicoNOX.stDadosObsViagem()
                        {
                            sDc_ObsGEN = string.Empty,
                            sDc_ObsOFV = string.Empty,
                            sDc_ObsOGR = string.Empty
                        }
                    },
                    ControleAgendamento = new ServicoNOX.stControleAgendamento()
                    {
                        sId_AutorizaCklNeg = "N",
                        sId_CtrlCarga = "N",
                        sId_CtrlDescarga = "N",
                        sId_UtilizaCondPadrao = "N",
                        sId_UtilizaFormPadrao = "N"
                    },
                    sCd_CnpjUnidNeg = !string.IsNullOrWhiteSpace(configuracaoIntegracao.CNPJMatrizNOX) ? configuracaoIntegracao.CNPJMatrizNOX : (cargaDadosTransporteIntegracao.Carga.Empresa?.CNPJ ?? string.Empty),
                    sCd_Placa = cargaDadosTransporteIntegracao.Carga.Veiculo?.Placa_Formatada ?? string.Empty,
                    sNm_UserCliente = string.Empty,
                    ControleDDR = new ServicoNOX.stControleDDR()
                    {
                        iCd_SeguradoraDdr = 0,
                        sCd_CnpjEmbarcDdr = string.Empty,
                        sId_Ddr = string.Empty
                    },
                    DadosAjudantes = new ServicoNOX.stDadosAjudantes()
                    {
                        iQt_Ajudantes = 0,
                        sNo_CpfAjudante = new ServicoNOX.ArrayOfString()
                    },
                    DadosEscolta = new ServicoNOX.stDadosEscolta()
                    {
                        sCd_CnpjEmpEscolta = string.Empty,
                        sCd_CpfAgenteEscolta = new ServicoNOX.ArrayOfString(),
                        sCd_PlacaEscolta = string.Empty,
                        sId_PossuiEscolta = "N"
                    },
                    FaixasTemperatura = new ServicoNOX.stFaixasTemperatura()
                    {
                        nFx_TemperMax = cargaDadosTransporteIntegracao.Carga?.TipoDeCarga?.FaixaDeTemperatura?.FaixaFinal ?? 0m,
                        nFx_TemperMin = cargaDadosTransporteIntegracao.Carga?.TipoDeCarga?.FaixaDeTemperatura?.FaixaInicial ?? 0m,
                        sId_CtrlTemperatura = string.Empty
                    },
                    RastreadorAuxiliar = new ServicoNOX.stRastreadorAuxiliar()
                    {
                        sId_UtilizaRLAux = "N",
                        Rastreadores = new ServicoNOX.stRastreadores[] {
                        new ServicoNOX.stRastreadores()
                        {
                            iCd_ModRLAux = 0,
                            iCd_TecnologiaRLAux = 0,
                            sCd_SerialRLAux = string.Empty,
                            sTp_InstRastrLoc = string.Empty
                        }
                    }
                    }
                };

                ServicoNOX.stSet_SolicitaMonitoramentoResultV2 retorno = svcMonitoramento.Set_SolicitaMonitoramentoAgenda(login, dadosMonitoramento);

                if (retorno != null && retorno.Retorno != null)
                    mensagem = string.Join(" / ", retorno.Retorno.Select(o => o.sCode + " - " + o.sResult));

                if (retorno != null && retorno.iCd_Viagem > 0)
                {
                    cargaDadosTransporteIntegracao.Protocolo = retorno.iCd_Viagem.ToString();
                    cargaDadosTransporteIntegracao.SituacaoIntegracao = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoIntegracao.Integrado;
                    cargaDadosTransporteIntegracao.ProblemaIntegracao = mensagem + " - Protocolo " + cargaDadosTransporteIntegracao.Protocolo + ".";
                }
                else
                {
                    cargaDadosTransporteIntegracao.SituacaoIntegracao = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoIntegracao.ProblemaIntegracao;
                    cargaDadosTransporteIntegracao.ProblemaIntegracao = mensagem;
                }


            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);
                cargaDadosTransporteIntegracao.ProblemaIntegracao = "Ocorreu uma falha ao comunicar com o Web Service da NOX.";
                cargaDadosTransporteIntegracao.SituacaoIntegracao = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoIntegracao.ProblemaIntegracao;
            }

            Dominio.Entidades.Embarcador.Cargas.CargaCTeIntegracaoArquivo arquivoIntegracao = new Dominio.Entidades.Embarcador.Cargas.CargaCTeIntegracaoArquivo
            {
                ArquivoRequisicao = Servicos.Embarcador.Integracao.ArquivoIntegracao.SalvarArquivoIntegracao(inspector.LastRequestXML, "xml", unitOfWork),
                ArquivoResposta = Servicos.Embarcador.Integracao.ArquivoIntegracao.SalvarArquivoIntegracao(inspector.LastResponseXML, "xml", unitOfWork),
                Data = DateTime.Now,
                Mensagem = cargaDadosTransporteIntegracao.ProblemaIntegracao,
                Tipo = Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoArquivoIntegracaoCTeCarga.EnvioParaProcessamento
            };

            repCargaCTeIntegracaoArquivo.Inserir(arquivoIntegracao);

            cargaDadosTransporteIntegracao.ArquivosTransacao.Add(arquivoIntegracao);

            repCargaDadosTransporteIntegracao.Atualizar(cargaDadosTransporteIntegracao);

        }

        public static void IntegrarCarga(Dominio.Entidades.Embarcador.Cargas.CargaCargaIntegracao cargaIntegracao, Repositorio.UnitOfWork unitOfWork)
        {
            Repositorio.Embarcador.Configuracoes.Integracao repConfiguracaoIntegracao = new Repositorio.Embarcador.Configuracoes.Integracao(unitOfWork);
            Repositorio.Embarcador.Cargas.CargaCTeIntegracaoArquivo repCargaCTeIntegracaoArquivo = new Repositorio.Embarcador.Cargas.CargaCTeIntegracaoArquivo(unitOfWork);
            Repositorio.Embarcador.Pedidos.PedidoXMLNotaFiscal repPedidoXMLNotaFiscal = new Repositorio.Embarcador.Pedidos.PedidoXMLNotaFiscal(unitOfWork);
            Repositorio.Embarcador.Cargas.CargaCTe repCargaCTe = new Repositorio.Embarcador.Cargas.CargaCTe(unitOfWork);
            Repositorio.Embarcador.Cargas.CargaCargaIntegracao repCargaIntegracao = new Repositorio.Embarcador.Cargas.CargaCargaIntegracao(unitOfWork);

            Dominio.Entidades.Embarcador.Configuracoes.Integracao configuracaoIntegracao = repConfiguracaoIntegracao.Buscar();

            cargaIntegracao.NumeroTentativas += 1;
            cargaIntegracao.DataIntegracao = DateTime.Now;

            if (configuracaoIntegracao == null || !configuracaoIntegracao.PossuiIntegracaoNOX || string.IsNullOrWhiteSpace(configuracaoIntegracao.UsuarioNOX) || string.IsNullOrWhiteSpace(configuracaoIntegracao.SenhaNOX) || string.IsNullOrWhiteSpace(configuracaoIntegracao.TokenNOX) || string.IsNullOrWhiteSpace(configuracaoIntegracao.URLProducaoNOX))
            {
                cargaIntegracao.SituacaoIntegracao = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoIntegracao.ProblemaIntegracao;
                cargaIntegracao.ProblemaIntegracao = "Não existe configuração de integração disponível para a NOX.";

                repCargaIntegracao.Atualizar(cargaIntegracao);

                return;
            }

            string urlWebService = configuracaoIntegracao.URLHomologacaoNOX;

            if (cargaIntegracao.Carga.Empresa.TipoAmbiente == Dominio.Enumeradores.TipoAmbiente.Producao)
                urlWebService = configuracaoIntegracao.URLProducaoNOX;

            string mensagem = string.Empty;
            string request = string.Empty;
            string response = string.Empty;

            Servicos.Models.Integracao.InspectorBehavior inspector = new Servicos.Models.Integracao.InspectorBehavior();

            try
            {
                ServicoNOX.IntegraGRSoapTypeClient svcMonitoramento = new Servicos.Embarcador.Integracao.ConfiguracaoWebService(unitOfWork).ObterClient<ServicoNOX.IntegraGRSoapTypeClient, ServicoNOX.IntegraGRSoapType>(Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoWebServiceIntegracao.NOX_IntegraGR, urlWebService, out inspector);

                ServicoNOX.stLogin login = new ServicoNOX.stLogin()
                {
                    sUserName = configuracaoIntegracao.UsuarioNOX,
                    sPassWord = configuracaoIntegracao.SenhaNOX,
                    sToken = configuracaoIntegracao.TokenNOX
                };

                Dominio.Entidades.Embarcador.Cargas.CargaPedido cargaPedido = cargaIntegracao.Carga.Pedidos.FirstOrDefault();
                Dominio.Entidades.Usuario condutorPrincipal = cargaIntegracao.Carga.Motoristas.FirstOrDefault();
                Dominio.Entidades.Usuario condutorAuxiliar = cargaIntegracao.Carga.Motoristas.Count > 1 ? cargaIntegracao.Carga.Motoristas[1] : null;

                decimal valorTotalNotas = repPedidoXMLNotaFiscal.ObterValorTotalPorCarga(cargaIntegracao.Carga.Codigo);

                ServicoNOX.ArrayOfString veiculos = new ServicoNOX.ArrayOfString();
                int idFormacaoCVC = 0;

                if (cargaIntegracao.Carga.Veiculo != null)
                    idFormacaoCVC = cargaIntegracao.Carga.Veiculo.ModeloVeicularCarga?.CodigoIntegracaoGerenciadoraRisco?.ToInt() ?? 0;

                if (cargaIntegracao.Carga.VeiculosVinculados.Any())
                    veiculos.Add(cargaIntegracao.Carga.VeiculosVinculados.First().Placa_Formatada);

                string codigoProduto = cargaPedido.Pedido?.Produtos?.Select(o => o.Produto?.CodigoProdutoEmbarcador).FirstOrDefault() ?? "1";

                Dominio.Entidades.Cliente clienteOrigem = null;
                Dominio.Entidades.Cliente clienteDestino = null;

                if (cargaPedido.TipoEmissaoCTeParticipantes == Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoEmissaoCTeParticipantes.ComExpedidor ||
                    cargaPedido.TipoEmissaoCTeParticipantes == Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoEmissaoCTeParticipantes.ComExpedidorERecebedor)
                    clienteOrigem = cargaPedido.Expedidor;
                else
                    clienteOrigem = cargaPedido.Pedido.Remetente;

                if (cargaPedido.TipoEmissaoCTeParticipantes == Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoEmissaoCTeParticipantes.ComRecebedor ||
                    cargaPedido.TipoEmissaoCTeParticipantes == Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoEmissaoCTeParticipantes.ComExpedidorERecebedor)
                    clienteDestino = cargaPedido.Recebedor;
                else
                    clienteDestino = cargaPedido.Pedido.Destinatario;

                ServicoNOX.stDadosSM dadosMonitoramento = new ServicoNOX.stDadosSM()
                {
                    CondutoresViagem = new ServicoNOX.stCondutoresViagem()
                    {
                        sNo_CpfCondutorPrinc = condutorPrincipal?.CPF ?? string.Empty,
                        sNo_CpfCondutorAux = condutorAuxiliar?.CPF ?? string.Empty
                    },
                    ConjuntoViagem = new ServicoNOX.stConjuntoViagem()
                    {
                        iId_FormacaoCVC = idFormacaoCVC,
                        sCd_PlacaCarretaCV = veiculos
                    },
                    DadosViagem = new ServicoNOX.stDadosViagem()
                    {
                        sCd_Rota = cargaIntegracao.Carga.Rota?.Codigo.ToString() ?? string.Empty,
                        sCd_MunicipioOrigem = (cargaPedido.Origem?.CodigoIBGE ?? 0).ToString("D7"),
                        sCd_MunicipioDestino = !string.IsNullOrEmpty(cargaPedido.Destino?.CodigoIntegracao) ? cargaPedido.Destino.CodigoIntegracao : (cargaPedido.Destino?.CodigoIBGE != null ? cargaPedido.Destino.CodigoIBGE.ToString("D7") : "0000000"),
                        dDh_PrevInicio = cargaPedido.Pedido.DataPrevisaoSaida ?? DateTime.Now,
                        dDh_PrevFim = cargaPedido.Pedido.PrevisaoEntrega ?? DateTime.Now,
                        sId_OperTransp = cargaIntegracao.Carga.TipoOperacao?.CodigoIntegracaoGerenciadoraRisco ?? string.Empty,
                        nVl_Carga = valorTotalNotas,
                        sCd_CnpjEmbarcViagem = cargaPedido.Pedido.Remetente.CPF_CNPJ_SemFormato,
                        EnderecoOrigem = new ServicoNOX.stEnderecos()
                        {
                            sCEP = Utilidades.String.OnlyNumbers(clienteOrigem?.CEP),
                            sNo_Endereco = string.IsNullOrWhiteSpace(clienteOrigem?.Numero) ? "S/N" : clienteOrigem.Numero,
                            sNm_Endereco = clienteOrigem?.Endereco ?? string.Empty,
                            sNm_Bairro = clienteOrigem?.Bairro ?? string.Empty
                        },
                        EnderecoDestino = new ServicoNOX.stEnderecos()
                        {
                            sCEP = Utilidades.String.OnlyNumbers(clienteDestino?.CEP),
                            sNo_Endereco = string.IsNullOrWhiteSpace(clienteDestino?.Numero) ? "S/N" : clienteDestino.Numero,
                            sNm_Endereco = clienteDestino?.Endereco ?? string.Empty,
                            sNm_Bairro = clienteDestino?.Bairro ?? string.Empty
                        },
                        DadosTranspTerceiro = new ServicoNOX.stDadosTranspTerceiro()
                        {
                            sCd_CnpjTransp = string.Empty,
                            sId_TranspTerceiro = string.Empty
                        },
                        SequenciaOperacao = (from cte in cargaIntegracao.Carga.CargaCTes
                                             select new ServicoNOX.stSequenciaOperacao()
                                             {
                                                 dDh_PrevisaoChegada = cargaPedido.Pedido.PrevisaoEntrega ?? DateTime.Now,
                                                 nVl_Produto = cte.CTe.ValorTotalMercadoria,
                                                 sCd_CnpjEmbarcCliente = cte.CTe.Destinatario.CPF_CNPJ_SemFormato,
                                                 sCd_MunicipioOper = cte.CTe.LocalidadeTerminoPrestacao.CodigoIBGE.ToString("D7"),
                                                 sCd_Produto = codigoProduto,
                                                 sDc_LocalOperacao = string.Empty,
                                                 sId_DetNF = "N",
                                                 sId_Operacao = "E",
                                                 iNo_NotaFiscal = new ServicoNOX.ArrayOfInt(),
                                                 sCEP = Utilidades.String.OnlyNumbers(cte.CTe.Recebedor?.CEP ?? cte.CTe.Destinatario?.CEP),
                                                 sNm_Bairro = cte.CTe.Recebedor?.Bairro ?? cte.CTe.Destinatario?.Bairro ?? string.Empty,
                                                 sNm_Endereco = cte.CTe.Recebedor?.Endereco ?? cte.CTe.Destinatario?.Endereco ?? string.Empty,
                                                 sNo_Endereco = cte.CTe.Recebedor?.Numero ?? cte.CTe.Destinatario?.Numero ?? string.Empty
                                             }).ToArray(),
                        DadosObsViagem = new ServicoNOX.stDadosObsViagem()
                        {
                            sDc_ObsGEN = string.Empty,
                            sDc_ObsOFV = string.Empty,
                            sDc_ObsOGR = string.Empty
                        }
                    },
                    ControleAgendamento = new ServicoNOX.stControleAgendamento()
                    {
                        sId_AutorizaCklNeg = "N",
                        sId_CtrlCarga = "N",
                        sId_CtrlDescarga = "N",
                        sId_UtilizaCondPadrao = "N",
                        sId_UtilizaFormPadrao = "N"
                    },
                    sCd_CnpjUnidNeg = !string.IsNullOrWhiteSpace(configuracaoIntegracao.CNPJMatrizNOX) ? configuracaoIntegracao.CNPJMatrizNOX : (cargaIntegracao.Carga.Empresa?.CNPJ ?? string.Empty),
                    sCd_Placa = cargaIntegracao.Carga.Veiculo?.Placa_Formatada ?? string.Empty,
                    sNm_UserCliente = string.Empty,
                    ControleDDR = new ServicoNOX.stControleDDR()
                    {
                        iCd_SeguradoraDdr = 0,
                        sCd_CnpjEmbarcDdr = string.Empty,
                        sId_Ddr = string.Empty
                    },
                    DadosAjudantes = new ServicoNOX.stDadosAjudantes()
                    {
                        iQt_Ajudantes = 0,
                        sNo_CpfAjudante = new ServicoNOX.ArrayOfString()
                    },
                    DadosEscolta = new ServicoNOX.stDadosEscolta()
                    {
                        sCd_CnpjEmpEscolta = string.Empty,
                        sCd_CpfAgenteEscolta = new ServicoNOX.ArrayOfString(),
                        sCd_PlacaEscolta = string.Empty,
                        sId_PossuiEscolta = "N"
                    },
                    FaixasTemperatura = new ServicoNOX.stFaixasTemperatura()
                    {
                        nFx_TemperMax = cargaIntegracao.Carga?.TipoDeCarga?.FaixaDeTemperatura?.FaixaFinal ?? 0m,
                        nFx_TemperMin = cargaIntegracao.Carga?.TipoDeCarga?.FaixaDeTemperatura?.FaixaInicial ?? 0m,
                        sId_CtrlTemperatura = string.Empty
                    },
                    RastreadorAuxiliar = new ServicoNOX.stRastreadorAuxiliar()
                    {
                        sId_UtilizaRLAux = "N",
                        Rastreadores = new ServicoNOX.stRastreadores[] {
                        new ServicoNOX.stRastreadores()
                        {
                            iCd_ModRLAux = 0,
                            iCd_TecnologiaRLAux = 0,
                            sCd_SerialRLAux = string.Empty,
                            sTp_InstRastrLoc = string.Empty
                        }
                    }
                    }
                };

                ServicoNOX.stSet_SolicitaMonitoramentoResult retorno = svcMonitoramento.Set_SolicitaMonitoramento(login, dadosMonitoramento);

                if (retorno != null && retorno.Retorno != null)
                    mensagem = string.Join(" / ", retorno.Retorno.Select(o => o.sCode + " - " + o.sResult));

                if (retorno != null && retorno.iCd_Viagem > 0)
                {
                    cargaIntegracao.Protocolo = retorno.iCd_Viagem.ToString();
                    cargaIntegracao.SituacaoIntegracao = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoIntegracao.Integrado;
                    cargaIntegracao.ProblemaIntegracao = mensagem + " - Protocolo " + cargaIntegracao.Protocolo + ".";
                }
                else
                {
                    cargaIntegracao.SituacaoIntegracao = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoIntegracao.ProblemaIntegracao;
                    cargaIntegracao.ProblemaIntegracao = mensagem;
                }
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);
                cargaIntegracao.ProblemaIntegracao = "Ocorreu uma falha ao comunicar com o Web Service da NOX.";
                cargaIntegracao.SituacaoIntegracao = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoIntegracao.ProblemaIntegracao;
            }

            Dominio.Entidades.Embarcador.Cargas.CargaCTeIntegracaoArquivo arquivoIntegracao = new Dominio.Entidades.Embarcador.Cargas.CargaCTeIntegracaoArquivo
            {
                ArquivoRequisicao = Servicos.Embarcador.Integracao.ArquivoIntegracao.SalvarArquivoIntegracao(inspector.LastRequestXML, "xml", unitOfWork),
                ArquivoResposta = Servicos.Embarcador.Integracao.ArquivoIntegracao.SalvarArquivoIntegracao(inspector.LastResponseXML, "xml", unitOfWork),
                Data = DateTime.Now,
                Mensagem = cargaIntegracao.ProblemaIntegracao,
                Tipo = Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoArquivoIntegracaoCTeCarga.EnvioParaProcessamento
            };

            repCargaCTeIntegracaoArquivo.Inserir(arquivoIntegracao);

            cargaIntegracao.ArquivosTransacao.Add(arquivoIntegracao);

            repCargaIntegracao.Atualizar(cargaIntegracao);
        }

        public static void IntegrarCancelamentoCarga(Dominio.Entidades.Embarcador.Cargas.CargaCancelamentoCargaIntegracao cargaCancelamentoIntegracao, Repositorio.UnitOfWork unitOfWork)
        {
            Repositorio.Embarcador.Configuracoes.Integracao repConfiguracaoIntegracao = new Repositorio.Embarcador.Configuracoes.Integracao(unitOfWork);
            Repositorio.Embarcador.Cargas.CargaCTeIntegracaoArquivo repCargaCTeIntegracaoArquivo = new Repositorio.Embarcador.Cargas.CargaCTeIntegracaoArquivo(unitOfWork);
            Repositorio.Embarcador.Pedidos.PedidoXMLNotaFiscal repPedidoXMLNotaFiscal = new Repositorio.Embarcador.Pedidos.PedidoXMLNotaFiscal(unitOfWork);
            Repositorio.Embarcador.Cargas.CargaCTe repCargaCTe = new Repositorio.Embarcador.Cargas.CargaCTe(unitOfWork);
            Repositorio.Embarcador.Cargas.CargaCargaIntegracao repCargaIntegracao = new Repositorio.Embarcador.Cargas.CargaCargaIntegracao(unitOfWork);
            Repositorio.Embarcador.Cargas.CargaCancelamentoCargaIntegracao repCargaCancelamentoCargaIntegracao = new Repositorio.Embarcador.Cargas.CargaCancelamentoCargaIntegracao(unitOfWork);

            Dominio.Entidades.Embarcador.Configuracoes.Integracao configuracaoIntegracao = repConfiguracaoIntegracao.Buscar();
            Dominio.Entidades.Embarcador.Cargas.CargaCargaIntegracao cargaCargaIntegracao = repCargaIntegracao.BuscarPorCargaETipoIntegracao(cargaCancelamentoIntegracao.CargaCancelamento.Carga.Codigo, cargaCancelamentoIntegracao.TipoIntegracao.Codigo);

            cargaCancelamentoIntegracao.NumeroTentativas++;
            cargaCancelamentoIntegracao.DataIntegracao = DateTime.Now;

            if (configuracaoIntegracao == null || !configuracaoIntegracao.PossuiIntegracaoNOX || string.IsNullOrWhiteSpace(configuracaoIntegracao.UsuarioNOX) || string.IsNullOrWhiteSpace(configuracaoIntegracao.SenhaNOX) || string.IsNullOrWhiteSpace(configuracaoIntegracao.TokenNOX) || string.IsNullOrWhiteSpace(configuracaoIntegracao.URLProducaoNOX))
            {
                cargaCancelamentoIntegracao.SituacaoIntegracao = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoIntegracao.ProblemaIntegracao;
                cargaCancelamentoIntegracao.ProblemaIntegracao = "Não existe configuração de integração disponível para a NOX.";

                repCargaCancelamentoCargaIntegracao.Atualizar(cargaCancelamentoIntegracao);

                return;
            }

            int codigoViagemNOX = cargaCargaIntegracao?.Protocolo?.ToInt() ?? 0;

            if (cargaCargaIntegracao == null || codigoViagemNOX <= 0)
            {
                cargaCancelamentoIntegracao.SituacaoIntegracao = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoIntegracao.Integrado;
                cargaCancelamentoIntegracao.ProblemaIntegracao = "Não foi encontrada integração realizada com sucesso para solicitar o cancelamento.";

                repCargaCancelamentoCargaIntegracao.Atualizar(cargaCancelamentoIntegracao);

                return;
            }

            string urlWebService = configuracaoIntegracao.URLHomologacaoNOX;

            if (cargaCancelamentoIntegracao.CargaCancelamento.Carga.Empresa.TipoAmbiente == Dominio.Enumeradores.TipoAmbiente.Producao)
                urlWebService = configuracaoIntegracao.URLProducaoNOX;

            ServicoNOX.IntegraGRSoapTypeClient svcMonitoramento = new Servicos.Embarcador.Integracao.ConfiguracaoWebService(unitOfWork).ObterClient<ServicoNOX.IntegraGRSoapTypeClient, ServicoNOX.IntegraGRSoapType>(Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoWebServiceIntegracao.NOX_IntegraGR, out Servicos.Models.Integracao.InspectorBehavior inspector);

            svcMonitoramento.Endpoint.Address = new System.ServiceModel.EndpointAddress(urlWebService);

            ServicoNOX.stLogin login = new ServicoNOX.stLogin()
            {
                sUserName = configuracaoIntegracao.UsuarioNOX,
                sPassWord = configuracaoIntegracao.SenhaNOX,
                sToken = configuracaoIntegracao.TokenNOX
            };

            ServicoNOX.stDadosCV dadosCancelamento = new ServicoNOX.stDadosCV()
            {
                iCd_Viagem = codigoViagemNOX,
                sCd_CnpjUnidNeg = string.IsNullOrWhiteSpace(configuracaoIntegracao.CNPJMatrizNOX) ? cargaCancelamentoIntegracao.CargaCancelamento.Carga.Empresa?.CNPJ : configuracaoIntegracao.CNPJMatrizNOX,
                sMotivoCancela = cargaCancelamentoIntegracao.CargaCancelamento.MotivoCancelamento
            };

            ServicoNOX.stSet_CancelaViagemResponse retorno = svcMonitoramento.Set_CancelaViagem(login, dadosCancelamento);

            string mensagem = string.Empty;
            bool sucesso = false;

            if (retorno.Retorno != null)
            {
                mensagem = string.Join(" / ", retorno.Retorno.Select(o => o.sCode + " - " + o.sResult));
                sucesso = retorno.Retorno.Any(o => CodigosSucesso.Contains(o.sCode));
            }

            Dominio.Entidades.Embarcador.Cargas.CargaCTeIntegracaoArquivo arquivoIntegracao = new Dominio.Entidades.Embarcador.Cargas.CargaCTeIntegracaoArquivo
            {
                ArquivoRequisicao = Servicos.Embarcador.Integracao.ArquivoIntegracao.SalvarArquivoIntegracao(inspector.LastRequestXML, "xml", unitOfWork),
                ArquivoResposta = Servicos.Embarcador.Integracao.ArquivoIntegracao.SalvarArquivoIntegracao(inspector.LastResponseXML, "xml", unitOfWork),
                Data = cargaCancelamentoIntegracao.DataIntegracao,
                Mensagem = mensagem,
                Tipo = Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoArquivoIntegracaoCTeCarga.EnvioParaProcessamento
            };

            repCargaCTeIntegracaoArquivo.Inserir(arquivoIntegracao);

            cargaCancelamentoIntegracao.ArquivosTransacao.Add(arquivoIntegracao);

            if (sucesso)
            {
                cargaCancelamentoIntegracao.SituacaoIntegracao = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoIntegracao.Integrado;
                cargaCancelamentoIntegracao.ProblemaIntegracao = mensagem;
            }
            else
            {
                cargaCancelamentoIntegracao.SituacaoIntegracao = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoIntegracao.ProblemaIntegracao;
                cargaCancelamentoIntegracao.ProblemaIntegracao = mensagem;
            }

            repCargaCancelamentoCargaIntegracao.Atualizar(cargaCancelamentoIntegracao);
        }

        #endregion

        #region Métodos Privados

        private static bool IntegrarCheckList(string urlWebService, Dominio.Entidades.Embarcador.Configuracoes.Integracao configuracaoIntegracao, Dominio.Entidades.Embarcador.Cargas.CargaCargaIntegracao cargaIntegracao, Repositorio.UnitOfWork unitOfWork)
        {
            Repositorio.Embarcador.Cargas.CargaCargaIntegracao repCargaIntegracao = new Repositorio.Embarcador.Cargas.CargaCargaIntegracao(unitOfWork);
            Repositorio.Embarcador.Cargas.CargaCTeIntegracaoArquivo repCargaCTeIntegracaoArquivo = new Repositorio.Embarcador.Cargas.CargaCTeIntegracaoArquivo(unitOfWork);

            string mensagem = string.Empty;
            string request = string.Empty;
            string response = string.Empty;

            bool sucesso = false;

            Servicos.Models.Integracao.InspectorBehavior inspector = new Servicos.Models.Integracao.InspectorBehavior();

            try
            {
                ServicoNOX.IntegraGRSoapTypeClient svcMonitoramento = new Servicos.Embarcador.Integracao.ConfiguracaoWebService(unitOfWork).ObterClient<ServicoNOX.IntegraGRSoapTypeClient, ServicoNOX.IntegraGRSoapType>(Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoWebServiceIntegracao.NOX_IntegraGR, urlWebService, out inspector);

                ServicoNOX.stLogin login = new ServicoNOX.stLogin()
                {
                    sUserName = configuracaoIntegracao.UsuarioNOX,
                    sPassWord = configuracaoIntegracao.SenhaNOX,
                    sToken = configuracaoIntegracao.TokenNOX
                };

                ServicoNOX.ArrayOfString veiculos = new ServicoNOX.ArrayOfString();

                if (cargaIntegracao.Carga.VeiculosVinculados.Any())
                    veiculos.Add(cargaIntegracao.Carga.VeiculosVinculados.First().Placa_Formatada);

                ServicoNOX.stDadosCK dados = new ServicoNOX.stDadosCK()
                {
                    sCd_CnpjUnidNeg = !string.IsNullOrWhiteSpace(configuracaoIntegracao.CNPJMatrizNOX) ? configuracaoIntegracao.CNPJMatrizNOX : (cargaIntegracao.Carga.Empresa?.CNPJ ?? string.Empty),
                    sCd_Placa = cargaIntegracao.Carga.Veiculo?.Placa_Formatada ?? string.Empty,
                    sCd_PlacaCarreta = veiculos,
                    sSolicitaCheckList = "S"
                };

                Servicos.ServicoNOX.stSet_SolicitaChecklistResponse retorno = svcMonitoramento.Set_SolicitaChecklist(login, dados);

                if (retorno != null && retorno.Retorno != null)
                {
                    mensagem = string.Join(" / ", retorno.Retorno.Select(o => o.sCode + " - " + o.sResult));
                    sucesso = retorno.Retorno.Any(o => !string.IsNullOrWhiteSpace(o.sCode) && o.sCode.StartsWith("OK"));
                }
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);
                mensagem = "Ocorreu uma falha ao comunicar com o Web Service da NOX.";
            }

            if (!sucesso)
            {
                cargaIntegracao.SituacaoIntegracao = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoIntegracao.ProblemaIntegracao;
                cargaIntegracao.ProblemaIntegracao = mensagem;
            }

            Dominio.Entidades.Embarcador.Cargas.CargaCTeIntegracaoArquivo arquivoIntegracao = new Dominio.Entidades.Embarcador.Cargas.CargaCTeIntegracaoArquivo
            {
                ArquivoRequisicao = Servicos.Embarcador.Integracao.ArquivoIntegracao.SalvarArquivoIntegracao(inspector.LastRequestXML, "xml", unitOfWork),
                ArquivoResposta = Servicos.Embarcador.Integracao.ArquivoIntegracao.SalvarArquivoIntegracao(inspector.LastResponseXML, "xml", unitOfWork),
                Data = DateTime.Now,
                Mensagem = mensagem,
                Tipo = Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoArquivoIntegracaoCTeCarga.EnvioParaProcessamento
            };

            repCargaCTeIntegracaoArquivo.Inserir(arquivoIntegracao);

            cargaIntegracao.ArquivosTransacao.Add(arquivoIntegracao);

            repCargaIntegracao.Atualizar(cargaIntegracao);

            return sucesso;
        }

        #endregion
    }
}
