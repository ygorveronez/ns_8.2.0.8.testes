using Dominio.Excecoes.Embarcador;
using Dominio.ObjetosDeValor.Embarcador.Enumeradores;
using Dominio.ObjetosDeValor.Relatorios;
using Infrastructure.Services.HttpClientFactory;
using Newtonsoft.Json;
using Servicos.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;

namespace Servicos.Embarcador.Integracao.Repom
{
    public class ValePedagio
    {
        #region Atributos Globais

        private readonly Repositorio.UnitOfWork _unitOfWork;
        private Dominio.Entidades.Embarcador.Configuracoes.IntegracaoRepom _integracaoRepom;

        #endregion

        #region Construtores

        public ValePedagio(Repositorio.UnitOfWork unitOfWork)
        {
            this._unitOfWork = unitOfWork;
        }

        #endregion

        #region Métodos Públicos

        public void GerarCompraValePedagio(Dominio.Entidades.Embarcador.Cargas.ValePedagio.CargaIntegracaoValePedagio cargaValePedagio, Dominio.Entidades.Embarcador.Cargas.Carga carga, AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware tipoServicoMultisoftware)
        {
            Repositorio.Embarcador.Cargas.ValePedagio.CargaIntegracaoValePedagio repCargaValePedagio = new Repositorio.Embarcador.Cargas.ValePedagio.CargaIntegracaoValePedagio(_unitOfWork);
            Repositorio.Embarcador.Cargas.Carga repCarga = new Repositorio.Embarcador.Cargas.Carga(_unitOfWork);
            Repositorio.Embarcador.Cargas.ValePedagio.CargaConsultaValorPedagioIntegracao repCargaConsultaValorPedagioIntegracao = new Repositorio.Embarcador.Cargas.ValePedagio.CargaConsultaValorPedagioIntegracao(_unitOfWork);

            try
            {
                ObterConfiguracaoIntegracaoRepom(cargaValePedagio.Carga, tipoServicoMultisoftware);

                if (_integracaoRepom == null)
                {
                    cargaValePedagio.ProblemaIntegracao = "Não possui configuração para Repom.";
                    cargaValePedagio.SituacaoIntegracao = SituacaoIntegracao.ProblemaIntegracao;
                    cargaValePedagio.DataIntegracao = DateTime.Now;
                    cargaValePedagio.NumeroTentativas++;
                    repCargaValePedagio.Atualizar(cargaValePedagio);

                    return;
                }

                System.Net.ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;
                Servicos.Models.Integracao.InspectorBehavior inspector = new Servicos.Models.Integracao.InspectorBehavior();

                string mensagemRetorno = string.Empty;

                Dominio.ObjetosDeValor.Embarcador.Integracao.Repom.CodigoRoteiroPercurso codigoRoteiroEpercurso = repCargaConsultaValorPedagioIntegracao.BuscarRoteiroEPercursoPorCargaTipoIntegracao(carga.Codigo, cargaValePedagio.TipoIntegracao);

                int codigoRoteiro = codigoRoteiroEpercurso.CodigoRoteiro;
                int codigoPercurso = codigoRoteiroEpercurso.CodigoPercurso;
                int quantidadeEixos = 0;
                string numeroCartaoMotorista = string.Empty;
                string codigoRotaEmbarcador = string.Empty;
                bool aguardarCadastroRota = false;

                if (codigoRoteiro == 0)
                {
                    Servicos.Models.Integracao.InspectorBehavior inspectorBuscarRota = new Servicos.Models.Integracao.InspectorBehavior();
                    bool consultaRoteiro = ConsultarRoteiro(carga, cargaValePedagio, ref codigoRotaEmbarcador, ref codigoRoteiro, ref codigoPercurso, ref aguardarCadastroRota, ref mensagemRetorno, ref inspectorBuscarRota);
                    //if (integracaoRepom.TipoIntegracaoRepom != TipoIntegracaoRepom.REsT)
                    //    SalvarXMLIntegracao(ref cargaValePedagio, inspectorBuscarRota, "Consulta Roteiro " + mensagemRetorno);

                    if (!consultaRoteiro)
                    {
                        cargaValePedagio.ProblemaIntegracao = mensagemRetorno;
                        if (aguardarCadastroRota)
                        {
                            cargaValePedagio.SituacaoValePedagio = SituacaoValePedagio.AguardandoCadastroRota;
                            cargaValePedagio.SituacaoIntegracao = SituacaoIntegracao.ProblemaIntegracao;
                        }
                        else
                        {
                            cargaValePedagio.SituacaoValePedagio = SituacaoValePedagio.Pendete;
                            cargaValePedagio.SituacaoIntegracao = SituacaoIntegracao.ProblemaIntegracao;
                        }


                        cargaValePedagio.DataIntegracao = DateTime.Now;
                        cargaValePedagio.NumeroTentativas++;
                        repCargaValePedagio.Atualizar(cargaValePedagio);

                        return;
                    }
                    else
                    {
                        cargaValePedagio.CodigoRoteiro = codigoRoteiro;
                        cargaValePedagio.CodigoPercurso = codigoPercurso;
                        repCargaValePedagio.Atualizar(cargaValePedagio);
                    }
                }

                if (codigoRoteiro <= 0)
                {
                    cargaValePedagio.ProblemaIntegracao = "Não existe rota localizada na Repom " + codigoRotaEmbarcador;
                    cargaValePedagio.SituacaoIntegracao = SituacaoIntegracao.ProblemaIntegracao;
                    cargaValePedagio.DataIntegracao = DateTime.Now;
                    cargaValePedagio.NumeroTentativas++;
                    repCargaValePedagio.Atualizar(cargaValePedagio);

                    return;
                }

                if (_integracaoRepom.TipoIntegracaoRepom == TipoIntegracaoRepom.REsT)
                {
                    ComprarValePedagioREST(carga, cargaValePedagio, codigoRoteiro, codigoPercurso);
                }
                else//Padrão é em SOAP
                {
                    Servicos.Models.Integracao.InspectorBehavior inspectorConsultaRoteiro = new Servicos.Models.Integracao.InspectorBehavior();
                    Dominio.ObjetosDeValor.Embarcador.CIOT.Repom.RetornoConsultaValorPedagio retornoConsultaValorPedagio = null;
                    bool consultarValor = ConsultarValor(carga, codigoRotaEmbarcador, codigoRoteiro, codigoPercurso, ref retornoConsultaValorPedagio, ref mensagemRetorno, ref inspectorConsultaRoteiro);

                    SalvarXMLIntegracao(ref cargaValePedagio, inspectorConsultaRoteiro, "Consulta Valor " + mensagemRetorno);
                    if (!consultarValor)
                    {
                        cargaValePedagio.ProblemaIntegracao = mensagemRetorno;
                        cargaValePedagio.SituacaoIntegracao = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoIntegracao.ProblemaIntegracao;
                        cargaValePedagio.DataIntegracao = DateTime.Now;
                        cargaValePedagio.NumeroTentativas++;
                        repCargaValePedagio.Atualizar(cargaValePedagio);

                        return;
                    }

                    if (retornoConsultaValorPedagio == null)
                    {
                        cargaValePedagio.ProblemaIntegracao = string.IsNullOrWhiteSpace(mensagemRetorno) ? "Consulta valor não teve retorno" : mensagemRetorno;
                        cargaValePedagio.SituacaoIntegracao = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoIntegracao.ProblemaIntegracao;
                        cargaValePedagio.DataIntegracao = DateTime.Now;
                        cargaValePedagio.NumeroTentativas++;
                        repCargaValePedagio.Atualizar(cargaValePedagio);

                        return;
                    }

                    decimal.TryParse(retornoConsultaValorPedagio.ValorTotalVPR, out decimal valorValePedagio);

                    if (valorValePedagio == 0)
                    {
                        cargaValePedagio.ProblemaIntegracao = "Rota sem valor de Vale pedágio na Repom";
                        cargaValePedagio.SituacaoIntegracao = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoIntegracao.Integrado;
                        cargaValePedagio.SituacaoValePedagio = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoValePedagio.RotaSemCusto;
                        cargaValePedagio.DataIntegracao = DateTime.Now;
                        cargaValePedagio.NumeroTentativas++;
                        repCargaValePedagio.Atualizar(cargaValePedagio);

                        carga.PossuiPendencia = false;
                        carga.ProblemaIntegracaoValePedagio = false;
                        carga.IntegrandoValePedagio = true;
                        carga.MotivoPendencia = "";
                        repCarga.Atualizar(carga);

                        return;
                    }

                    Servicos.Models.Integracao.InspectorBehavior inspectorCompraValePedagio = new Servicos.Models.Integracao.InspectorBehavior();
                    Dominio.ObjetosDeValor.Embarcador.CIOT.Repom.RetornoCompraValePedagio retornoCompraValePedagio = null;
                    bool comprarValePedagio = ComprarValePedagio(carga, codigoRotaEmbarcador, codigoRoteiro, codigoPercurso, valorValePedagio, ref retornoCompraValePedagio, ref mensagemRetorno, ref inspectorCompraValePedagio, ref quantidadeEixos, ref numeroCartaoMotorista);

                    cargaValePedagio.TipoCompra = !string.IsNullOrWhiteSpace(numeroCartaoMotorista) ? Dominio.Enumeradores.TipoCompraValePedagio.Cartao : Dominio.Enumeradores.TipoCompraValePedagio.Tag;
                    cargaValePedagio.QuantidadeEixos = quantidadeEixos;

                    SalvarXMLIntegracao(ref cargaValePedagio, inspectorCompraValePedagio, "Compra vale pedágio " + mensagemRetorno);

                    if (!comprarValePedagio)
                    {
                        cargaValePedagio.ProblemaIntegracao = mensagemRetorno;
                        cargaValePedagio.SituacaoIntegracao = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoIntegracao.ProblemaIntegracao;
                        cargaValePedagio.DataIntegracao = DateTime.Now;
                        cargaValePedagio.NumeroTentativas++;
                        repCargaValePedagio.Atualizar(cargaValePedagio);
                    }
                    else if (retornoCompraValePedagio == null)
                    {
                        cargaValePedagio.ProblemaIntegracao = !string.IsNullOrWhiteSpace(mensagemRetorno) ? mensagemRetorno : "Compra não teve retorno";
                        cargaValePedagio.SituacaoIntegracao = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoIntegracao.ProblemaIntegracao;
                        cargaValePedagio.DataIntegracao = DateTime.Now;
                        cargaValePedagio.NumeroTentativas++;
                        repCargaValePedagio.Atualizar(cargaValePedagio);
                    }
                    else
                    {
                        cargaValePedagio.NumeroValePedagio = "";
                        cargaValePedagio.IdCompraValePedagio = retornoCompraValePedagio.CodigoViagem;
                        cargaValePedagio.NumeroValePedagio = retornoCompraValePedagio.CodigoViagem;
                        cargaValePedagio.ValorValePedagio = valorValePedagio;
                        cargaValePedagio.SituacaoValePedagio = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoValePedagio.Comprada;
                        cargaValePedagio.SituacaoIntegracao = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoIntegracao.AgRetorno;
                        cargaValePedagio.ProblemaIntegracao = string.Empty;
                        cargaValePedagio.DataIntegracao = DateTime.Now;
                        cargaValePedagio.NumeroTentativas++;

                        repCargaValePedagio.Atualizar(cargaValePedagio);

                        if (cargaValePedagio.Carga.PossuiPendencia)
                        {
                            carga.PossuiPendencia = false;
                            carga.ProblemaIntegracaoValePedagio = false;
                            carga.IntegrandoValePedagio = true;
                            carga.MotivoPendencia = "";
                            repCarga.Atualizar(carga);
                        }

                        SalvarDadosRetornoCompraValePedagio(retornoCompraValePedagio, carga);
                    }
                }
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);

                cargaValePedagio.ProblemaIntegracao = "Falha no serviço da Repom";
                cargaValePedagio.SituacaoIntegracao = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoIntegracao.ProblemaIntegracao;
                cargaValePedagio.DataIntegracao = DateTime.Now;
                cargaValePedagio.NumeroTentativas++;
                repCargaValePedagio.Atualizar(cargaValePedagio);

                return;
            }
        }

        public byte[] GerarImpressaoValePedagio(Dominio.Entidades.Embarcador.Cargas.ValePedagio.CargaIntegracaoValePedagio cargaValePedagio, AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware tipoServicoMultisoftware)
        {
            return ReportRequest.WithType(ReportType.Repom)
                .WithExecutionType(ExecutionType.Sync)
                .AddExtraData("codigoCargaValePedagio", cargaValePedagio.Codigo.ToString())
                .CallReport()
                .GetContentFile();
        }

        public void SolicitarCancelamentoValePedagio(Dominio.Entidades.Embarcador.Cargas.ValePedagio.CargaIntegracaoValePedagio cargaValePedagio, Dominio.Entidades.Embarcador.Cargas.Carga carga, AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware tipoServicoMultisoftware)
        {
            Repositorio.Embarcador.Cargas.ValePedagio.CargaIntegracaoValePedagio repCargaValePedagio = new Repositorio.Embarcador.Cargas.ValePedagio.CargaIntegracaoValePedagio(_unitOfWork);
            Repositorio.Embarcador.Cargas.CargaCancelamento repCargaCancelamento = new Repositorio.Embarcador.Cargas.CargaCancelamento(_unitOfWork);

            ObterConfiguracaoIntegracaoRepom(cargaValePedagio.Carga, tipoServicoMultisoftware);

            if (_integracaoRepom == null)
            {
                cargaValePedagio.ProblemaIntegracao = "Não possui configuração para Repom.";
                cargaValePedagio.SituacaoIntegracao = SituacaoIntegracao.ProblemaIntegracao;
                cargaValePedagio.DataIntegracao = DateTime.Now;
                cargaValePedagio.NumeroTentativas++;
                repCargaValePedagio.Atualizar(cargaValePedagio);

                return;
            }

            string mensagemErro = string.Empty;

            Dominio.Entidades.Embarcador.Cargas.CargaCancelamento cargaCancelamento = repCargaCancelamento.BuscarPorCarga(cargaValePedagio.Carga.Codigo);

            if (_integracaoRepom.TipoIntegracaoRepom == TipoIntegracaoRepom.REsT)
            {
                string jsonRequisicao = string.Empty;
                string jsonRetorno = string.Empty;

                cargaValePedagio.DataIntegracao = DateTime.Now;
                cargaValePedagio.NumeroTentativas += 1;

                try
                {
                    string authToken = ObterTokenViagem();

                    if (string.IsNullOrWhiteSpace(authToken))
                        throw new ServicoException("Não retornou o token de integração.");

                    string url = _integracaoRepom.URLViagem + "/api/RepomViagem/CancelaViagem";
                    HttpClient requisicao = CriarRequisicao(url, authToken);

                    Dominio.ObjetosDeValor.Embarcador.Integracao.Repom.CancelaViagem cancelaViagem = new Dominio.ObjetosDeValor.Embarcador.Integracao.Repom.CancelaViagem()
                    {
                        ViagemCodigo = cargaValePedagio.IdCompraValePedagio,
                        Usuario = cargaCancelamento?.Usuario?.Nome ?? string.Empty
                    };

                    jsonRequisicao = JsonConvert.SerializeObject(cancelaViagem, Formatting.Indented);
                    StringContent conteudoRequisicao = new StringContent(jsonRequisicao.ToString(), Encoding.UTF8, "application/json");

                    HttpResponseMessage retornoRequisicao = requisicao.PostAsync(url, conteudoRequisicao).Result;
                    jsonRetorno = retornoRequisicao.Content.ReadAsStringAsync().Result;

                    if (retornoRequisicao.StatusCode == HttpStatusCode.OK)
                    {
                        Dominio.ObjetosDeValor.Embarcador.Integracao.Repom.RetornoCancelaViagem retorno = JsonConvert.DeserializeObject<Dominio.ObjetosDeValor.Embarcador.Integracao.Repom.RetornoCancelaViagem>(jsonRetorno);
                        if (retorno.Result.Status)
                        {
                            cargaValePedagio.SituacaoIntegracao = SituacaoIntegracao.Integrado;
                            cargaValePedagio.SituacaoValePedagio = SituacaoValePedagio.Cancelada;
                            cargaValePedagio.ProblemaIntegracao = "Vale Pedágio Cancelado com Sucesso";
                        }
                        else
                            throw new ServicoException(retorno.Result.NumeroErro + " - " + retorno.Result.Descricao);
                    }
                    else
                        throw new ServicoException($"Retorno cancelamento: {(int)retornoRequisicao.StatusCode}");
                }
                catch (ServicoException excecao)
                {
                    cargaValePedagio.SituacaoValePedagio = SituacaoValePedagio.Comprada;
                    cargaValePedagio.ProblemaIntegracao = excecao.Message;
                }
                catch (Exception excecao)
                {
                    Log.TratarErro(excecao);

                    cargaValePedagio.SituacaoValePedagio = SituacaoValePedagio.Comprada;
                    cargaValePedagio.ProblemaIntegracao = "Ocorreu uma falha ao realizar o cancelamento de Vale Pedágio da Repom";
                }

                SalvarJsonIntegracao(cargaValePedagio, jsonRequisicao, jsonRetorno);

                repCargaValePedagio.Atualizar(cargaValePedagio);
            }
            else//Padrão é em SOAP
            {
                Dominio.ObjetosDeValor.Embarcador.CIOT.Repom.CancelaValePedagio cancelaValePedagio = new Dominio.ObjetosDeValor.Embarcador.CIOT.Repom.CancelaValePedagio()
                {
                    CodigoProcessoTransporte = cargaValePedagio.NumeroValePedagio,
                    CodigoProcessoCliente = cargaValePedagio.Carga.CodigoCargaEmbarcador,
                    CodigoProcessoClienteFilial = string.Empty,
                    Login = cargaCancelamento?.Usuario?.Nome.Left(50) ?? string.Empty
                };

                System.Net.ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;

                ServicoRepom.Integracao.IntegracaoSoapClient svcIntegracaoRepom = new Servicos.Embarcador.Integracao.ConfiguracaoWebService(_unitOfWork).ObterClient<ServicoRepom.Integracao.IntegracaoSoapClient, ServicoRepom.Integracao.IntegracaoSoap>(Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoWebServiceIntegracao.Repom_Integracao, out Servicos.Models.Integracao.InspectorBehavior inspectorCancelamento);

                string xmlEnvio = Utilidades.XML.Serializar(cancelaValePedagio);
                string xmlRetorno = string.Empty;

                bool retorno = svcIntegracaoRepom.CancelaViagemVPR(_integracaoRepom.CodigoCliente, _integracaoRepom.AssinaturaDigital, xmlEnvio, ref xmlRetorno);
                if (retorno)
                {
                    cargaValePedagio.SituacaoIntegracao = SituacaoIntegracao.Integrado;
                    cargaValePedagio.SituacaoValePedagio = SituacaoValePedagio.Cancelada;

                    mensagemErro = "Vale Pedágio Cancelado com Sucesso";
                }
                else
                {
                    Dominio.ObjetosDeValor.Embarcador.CIOT.Repom.RetornoIntegracaoErro retornoErro = Utilidades.XML.Deserializar<Dominio.ObjetosDeValor.Embarcador.CIOT.Repom.RetornoIntegracaoErro>(xmlRetorno);

                    mensagemErro = string.Join(", ", retornoErro.Erros.Select(o => o.Codigo + " - " + o.Descricao));

                    cargaValePedagio.SituacaoValePedagio = SituacaoValePedagio.Comprada;
                }

                SalvarXMLIntegracao(ref cargaValePedagio, inspectorCancelamento, "Cancelar Vale Pedagio " + mensagemErro);

                cargaValePedagio.ProblemaIntegracao = "Retorno cancelamento: " + mensagemErro;
                cargaValePedagio.DataIntegracao = DateTime.Now;
                repCargaValePedagio.Atualizar(cargaValePedagio);
            }
        }

        public decimal ConsultarValorPedagio(Dominio.Entidades.Embarcador.Cargas.ValePedagio.CargaConsultaValorPedagioIntegracao cargaConsultaValePedagio, Dominio.Entidades.Embarcador.Cargas.Carga carga, AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware tipoServicoMultisoftware)
        {
            Repositorio.Embarcador.Cargas.ValePedagio.CargaConsultaValorPedagioIntegracao repCargaConsultaValorPedagioIntegracao = new Repositorio.Embarcador.Cargas.ValePedagio.CargaConsultaValorPedagioIntegracao(_unitOfWork);

            ObterConfiguracaoIntegracaoRepom(cargaConsultaValePedagio.Carga, tipoServicoMultisoftware);

            cargaConsultaValePedagio.DataIntegracao = DateTime.Now;
            cargaConsultaValePedagio.NumeroTentativas++;

            string authToken = ObterTokenRota();

            if ((cargaConsultaValePedagio.CodigoRoteiro <= 0) && (cargaConsultaValePedagio.CodigoPercurso <= 0))
                CadastrarRoteiroREST(cargaConsultaValePedagio, authToken);

            ConsultarValorREST(cargaConsultaValePedagio, authToken, tipoServicoMultisoftware);

            repCargaConsultaValorPedagioIntegracao.Atualizar(cargaConsultaValePedagio);

            return cargaConsultaValePedagio.ValorValePedagio;
        }

        #endregion

        #region Métodos Privados

        private bool ConsultarRoteiro(Dominio.Entidades.Embarcador.Cargas.Carga carga, Dominio.Entidades.Embarcador.Cargas.ValePedagio.CargaIntegracaoValePedagio cargaValePedagio, ref string codigoRotaEmbarcador, ref int codigoRoteiro, ref int codigoPercurso, ref bool aguardarCadastroRota, ref string mensagemRetorno, ref Servicos.Models.Integracao.InspectorBehavior inspector)
        {
            System.Net.ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;

            Repositorio.Embarcador.Cargas.CargaRotaFrete repCargaRotaFrete = new Repositorio.Embarcador.Cargas.CargaRotaFrete(_unitOfWork);
            Repositorio.Embarcador.Cargas.CargaRotaFretePontosPassagem repCargaRotaFretePontosPassagem = new Repositorio.Embarcador.Cargas.CargaRotaFretePontosPassagem(_unitOfWork);
            Servicos.Embarcador.Localidades.Localidade servicoLocalidade = new Servicos.Embarcador.Localidades.Localidade(_unitOfWork);
            Dominio.Entidades.Embarcador.Cargas.CargaRotaFrete cargaRotaFrete = repCargaRotaFrete.BuscarPorCarga(carga.Codigo);
            List<Dominio.Entidades.Embarcador.Cargas.CargaRotaFretePontosPassagem> pontosRota = null;

            if (cargaRotaFrete != null)
                pontosRota = repCargaRotaFretePontosPassagem.BuscarPorCargaRotaFrete(cargaRotaFrete.Codigo);

            if (pontosRota == null || pontosRota.Count == 0)
            {
                mensagemRetorno = "Não foi possível definir os pontos da rota.";
                return false;
            }

            Dominio.Entidades.Localidade localidadeOrigem = servicoLocalidade.ObterLocalidade(pontosRota.FirstOrDefault()); 
            Dominio.Entidades.Localidade localidadeDestino = servicoLocalidade.ObterLocalidade(pontosRota.LastOrDefault());

            if (localidadeOrigem == null)
            {
                mensagemRetorno = "Não foi possível definir a localidade origem da rota.";
                return false;
            }

            if (localidadeDestino == null)
            {
                mensagemRetorno = "Não foi possível definir a localidade destino da rota.";
                return false;
            }

            string pontosDeParada = string.Empty;
            List<Dominio.ObjetosDeValor.Embarcador.Integracao.Repom.CadastroRoteiroCidade> listaParadas = new List<Dominio.ObjetosDeValor.Embarcador.Integracao.Repom.CadastroRoteiroCidade>();

            if (_integracaoRepom.TipoIntegracaoRepom == TipoIntegracaoRepom.REsT && cargaValePedagio.RotaFrete != null)
            {
                //#35807 - Se a RotaFrete possui a flag marcada de Rota Exclusiva para compra de vale pedágio, obtem os parametros cadastrados na Rota de frete, aba "Pontos de Passagem", caso contrario segue como estava
                if (cargaValePedagio.RotaFrete.RotaExclusivaCompraValePedagio)
                {
                    Repositorio.Embarcador.Logistica.PontoPassagemPreDefinido repPontoPassagemPreDefinido = new Repositorio.Embarcador.Logistica.PontoPassagemPreDefinido(_unitOfWork);
                    List<Dominio.Entidades.Embarcador.Logistica.PontoPassagemPreDefinido> pontosPassagemPreDefinidos = repPontoPassagemPreDefinido.BuscarPorRocartaFrete(cargaValePedagio.RotaFrete.Codigo);

                    foreach (Dominio.Entidades.Embarcador.Logistica.PontoPassagemPreDefinido pontoPassagemPreDefinido in pontosPassagemPreDefinidos)
                    {
                        Dominio.Entidades.Localidade localidade = pontoPassagemPreDefinido.Cliente?.Localidade;
                        if (pontoPassagemPreDefinido.Localidade != null)
                            localidade = pontoPassagemPreDefinido.Localidade;

                        if (localidade == null)
                            continue;

                        Dominio.ObjetosDeValor.Embarcador.Integracao.Repom.CadastroRoteiroCidade parada = new Dominio.ObjetosDeValor.Embarcador.Integracao.Repom.CadastroRoteiroCidade()
                        {
                            CodigoIBGE = localidade.CodigoIBGE.ToString("00000000").Substring(3, 5).ToInt().ToString(),
                            Estado = localidade.Estado.Sigla
                        };
                        listaParadas.Add(parada);
                    }
                }
                else
                {
                    Repositorio.RotaFretePontosPassagem repRotaFretePontosPassagem = new Repositorio.RotaFretePontosPassagem(_unitOfWork);
                    List<Dominio.Entidades.RotaFretePontosPassagem> pontosPassagem = repRotaFretePontosPassagem.BuscarPorRotaFrete(cargaValePedagio.RotaFrete.Codigo);

                    foreach (Dominio.Entidades.RotaFretePontosPassagem pontoPassagem in pontosPassagem)
                    {
                        if (pontoPassagem.Cliente?.Localidade == null)
                            continue;

                        Dominio.ObjetosDeValor.Embarcador.Integracao.Repom.CadastroRoteiroCidade parada = new Dominio.ObjetosDeValor.Embarcador.Integracao.Repom.CadastroRoteiroCidade()
                        {
                            CodigoIBGE = pontoPassagem.Cliente.Localidade.CodigoIBGE.ToString("00000000").Substring(3, 5).ToInt().ToString(),
                            Estado = pontoPassagem.Cliente.Localidade.Estado.Sigla
                        };
                        listaParadas.Add(parada);
                    }
                }
            }
            else
            {
                for (int i = 0; i < pontosRota.Count; i++)
                {
                    if (i > 0 && i < pontosRota.Count - 1) //Não envia o primeiro e o ultimo ponto                
                    {
                        Dominio.Entidades.Localidade localidade = pontosRota[i].ClienteOutroEndereco != null ? pontosRota[i].ClienteOutroEndereco.Localidade : pontosRota[i].Cliente?.Localidade ?? null;
                        Dominio.Entidades.Localidade localidadeAnterior = null;
                        localidadeAnterior = pontosRota[i - 1].ClienteOutroEndereco != null ? pontosRota[i - 1].ClienteOutroEndereco.Localidade : pontosRota[i - 1].Cliente?.Localidade ?? null;
                        if (localidade != null)
                        {
                            if ((localidadeAnterior == null || localidade.Codigo != localidadeAnterior.Codigo) && localidade.Codigo != localidadeOrigem.Codigo && localidade.Codigo != localidadeDestino.Codigo)
                            {
                                if (_integracaoRepom.TipoIntegracaoRepom == TipoIntegracaoRepom.REsT)
                                {
                                    Dominio.ObjetosDeValor.Embarcador.Integracao.Repom.CadastroRoteiroCidade parada = new Dominio.ObjetosDeValor.Embarcador.Integracao.Repom.CadastroRoteiroCidade()
                                    {
                                        CodigoIBGE = localidade.CodigoIBGE.ToString("00000000").Substring(3, 5).ToInt().ToString(),
                                        Estado = localidade.Estado.Sigla
                                    };
                                    listaParadas.Add(parada);
                                }
                                else
                                {
                                    string ponto = string.Concat(localidade.CodigoIBGE.ToString("00000000").Substring(3, 5).ToInt().ToString(), "/", localidade.Estado.Sigla);
                                    pontosDeParada = string.IsNullOrWhiteSpace(pontosDeParada) ? ponto : string.Concat(pontosDeParada, ";", ponto);
                                }
                            }
                        }
                    }
                }
            }

            if (_integracaoRepom.TipoIntegracaoRepom == TipoIntegracaoRepom.REsT)
            {
                if (string.IsNullOrWhiteSpace(_integracaoRepom.URLRota))
                {
                    mensagemRetorno = "Configuração em REST da Repom não está completa, favor atualizar o cadastro da integração";
                    return false;
                }

                string jsonRetorno = string.Empty;

                try
                {
                    string authToken = ObterTokenRota();
                    if (string.IsNullOrWhiteSpace(authToken))
                    {
                        mensagemRetorno = "Não retornou o token de integração.";
                        return false;
                    }

                    string url = _integracaoRepom.URLRota;
                    if (cargaValePedagio.CodigoRoteiro > 0)
                    {
                        string urlConsulta = url + "/api/Rota?codigoRoteiro=" + cargaValePedagio.CodigoRoteiro;
                        urlConsulta += "&codigoPercurso=" + cargaValePedagio.CodigoPercurso;

                        HttpClient requisicao = CriarRequisicao(urlConsulta, authToken);

                        HttpResponseMessage retornoRequisicao = requisicao.GetAsync(urlConsulta).Result;
                        jsonRetorno = retornoRequisicao.Content.ReadAsStringAsync().Result;

                        if (retornoRequisicao.StatusCode == HttpStatusCode.OK)
                        {
                            Dominio.ObjetosDeValor.Embarcador.Integracao.Repom.RetornoConsultaRoteiro retorno = JsonConvert.DeserializeObject<Dominio.ObjetosDeValor.Embarcador.Integracao.Repom.RetornoConsultaRoteiro>(jsonRetorno);
                            codigoRoteiro = retorno.Roteiro.CodigoRoteiro;

                            if (codigoRoteiro > 0 && retorno.Status)
                            {
                                codigoPercurso = retorno.Roteiro.CodigoPercurso;
                                mensagemRetorno = "Consulta roteiro retornou o código " + codigoRoteiro;
                                SalvarJsonIntegracao(cargaValePedagio, null, jsonRetorno, mensagemRetorno);
                                return true;
                            }
                            else
                                return CadastrarRoteiroREST(url, authToken, localidadeOrigem, localidadeDestino, listaParadas, cargaValePedagio, ref codigoRoteiro, ref codigoPercurso, out mensagemRetorno);
                        }
                        else
                        {
                            mensagemRetorno = $"Retorno Consulta Roteiro: {(int)retornoRequisicao.StatusCode}";
                            SalvarJsonIntegracao(cargaValePedagio, null, jsonRetorno, mensagemRetorno);
                            return false;
                        }
                    }
                    else
                        return CadastrarRoteiroREST(url, authToken, localidadeOrigem, localidadeDestino, listaParadas, cargaValePedagio, ref codigoRoteiro, ref codigoPercurso, out mensagemRetorno);
                }
                catch (Exception excecao)
                {
                    Log.TratarErro(excecao);

                    mensagemRetorno = "Ocorreu uma falha ao realizar a integração com a Repom";

                    SalvarJsonIntegracao(cargaValePedagio, null, jsonRetorno, mensagemRetorno);
                    return false;
                }
            }
            else//Padrão é em SOAP
            {
                codigoRotaEmbarcador = carga.Codigo.ToString(); // string.Concat(localidadeOrigem?.CodigoIBGE, "_", localidadeDestino?.CodigoIBGE); Alterdo conforme orientação da Repom para enviar um codigo por carga

                ServicoRepom.Integracao.ConsultaRoteirosRequest consultaRoteiros = new ServicoRepom.Integracao.ConsultaRoteirosRequest();
                consultaRoteiros.Body = new ServicoRepom.Integracao.ConsultaRoteirosRequestBody
                {
                    strCliente = _integracaoRepom.CodigoCliente,
                    strAssinaturaDigital = _integracaoRepom.AssinaturaDigital,
                    strEstadoOrigem = localidadeOrigem?.Estado.Sigla,
                    strCidadeOrigem = localidadeOrigem?.Descricao,
                    strCEPOrigem = localidadeOrigem?.CEP,
                    strCodigoIBGEOrigem = localidadeOrigem?.CodigoIBGE.ToString("00000000").Substring(3, 5).ToInt().ToString(),
                    strEstadoDestino = localidadeDestino?.Estado.Sigla,
                    strCidadeDestino = localidadeDestino?.Descricao,
                    strCEPDestino = localidadeDestino?.CEP,
                    strCodigoIBGEDestino = localidadeDestino?.CodigoIBGE.ToString("00000000").Substring(3, 5).ToInt().ToString(),
                    strParadas = !string.IsNullOrWhiteSpace(pontosDeParada) ? pontosDeParada : null
                };

                string xmlRetorno = string.Empty;
                string xmlErro = string.Empty;

                ServicoRepom.Integracao.IntegracaoSoapClient svcRepom = new Servicos.Embarcador.Integracao.ConfiguracaoWebService(_unitOfWork).ObterClient<ServicoRepom.Integracao.IntegracaoSoapClient, ServicoRepom.Integracao.IntegracaoSoap>(Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoWebServiceIntegracao.Repom_Integracao, out inspector);

                bool retornoRota = svcRepom.ConsultaRoteiros(consultaRoteiros.Body.strCliente,
                                                             consultaRoteiros.Body.strAssinaturaDigital,
                                                             consultaRoteiros.Body.strEstadoOrigem,
                                                             string.Empty, //consultaRoteiros.Body.strCidadeOrigem,
                                                             string.Empty, //consultaRoteiros.Body.strCEPOrigem,
                                                             consultaRoteiros.Body.strCodigoIBGEOrigem,
                                                             consultaRoteiros.Body.strEstadoDestino,
                                                             string.Empty, //consultaRoteiros.Body.strCidadeDestino,
                                                             string.Empty, //consultaRoteiros.Body.strCEPDestino,
                                                             consultaRoteiros.Body.strCodigoIBGEDestino,
                                                             ref xmlRetorno,
                                                             ref xmlErro,
                                                             !string.IsNullOrWhiteSpace(consultaRoteiros.Body.strParadas) ? consultaRoteiros.Body.strParadas : null);

                SalvarXMLIntegracao(ref cargaValePedagio, inspector, "Consulta Roteiro");

                if (retornoRota)
                {
                    string codigoClienteRoteiro = codigoRotaEmbarcador;

                    Dominio.ObjetosDeValor.Embarcador.CIOT.Repom.RetornoConsultaRoteiro retornoConsultaRoteiro = Utilidades.XML.Deserializar<Dominio.ObjetosDeValor.Embarcador.CIOT.Repom.RetornoConsultaRoteiro>(xmlRetorno);

                    if (retornoConsultaRoteiro.Roteiros == null || retornoConsultaRoteiro.Roteiros.Length <= 0 || !(retornoConsultaRoteiro.Roteiros.Any(o => o.CodigoClienteRoteiro == codigoClienteRoteiro)))
                    {
                        Servicos.Models.Integracao.InspectorBehavior inspectorCadastrarRoteiro = new Servicos.Models.Integracao.InspectorBehavior();

                        if (!CadastrarRoteiro(consultaRoteiros, codigoRotaEmbarcador, carga, out mensagemRetorno, ref inspectorCadastrarRoteiro))
                        {
                            SalvarXMLIntegracao(ref cargaValePedagio, inspectorCadastrarRoteiro, "Cadastrar Roteiro codigo " + codigoClienteRoteiro);
                            return false;
                        }
                        else
                        {
                            SalvarXMLIntegracao(ref cargaValePedagio, inspectorCadastrarRoteiro, "Cadastrar Roteiro " + codigoClienteRoteiro);
                            aguardarCadastroRota = true;
                            mensagemRetorno = "Solicitado o cadastro do roteiro codigo " + codigoClienteRoteiro + " para a Repom, aguarde e reenvie a integração.";
                            return false;
                        }
                    }

                    Dominio.ObjetosDeValor.Embarcador.CIOT.Repom.RetornoConsultaRoteiroRoteiro roteiroUtilizar = retornoConsultaRoteiro.Roteiros.Where(o => o.CodigoClienteRoteiro == codigoClienteRoteiro).FirstOrDefault();
                    if (roteiroUtilizar == null)
                    {
                        mensagemRetorno = "Roteiro codigo cliente " + codigoClienteRoteiro + " não possui cadastro na Repom.";
                        return false;
                    }

                    int.TryParse(roteiroUtilizar?.CodigoRoteiro, out codigoRoteiro);
                    int.TryParse(roteiroUtilizar?.CodigoPercurso, out codigoPercurso);

                    return true;
                }
                else
                {
                    Dominio.ObjetosDeValor.Embarcador.CIOT.Repom.RetornoIntegracaoErro retornoErro = Utilidades.XML.Deserializar<Dominio.ObjetosDeValor.Embarcador.CIOT.Repom.RetornoIntegracaoErro>(xmlErro);

                    mensagemRetorno = string.Join(", ", retornoErro.Erros.Select(o => o.Codigo + " - " + o.Descricao));

                    return false;
                }
            }
        }

        private bool CadastrarRoteiro(ServicoRepom.Integracao.ConsultaRoteirosRequest consultaRoteiros, string codigoRotaEmbarcador, Dominio.Entidades.Embarcador.Cargas.Carga carga, out string mensagemErro, ref Servicos.Models.Integracao.InspectorBehavior inspector)
        {
            mensagemErro = null;

            string xmlEnvio = Utilidades.XML.Serializar(ObterCadastroRoteiro(consultaRoteiros, codigoRotaEmbarcador, carga));
            Servicos.Log.TratarErro("CadastrarRoteiro envio:" + xmlEnvio, "Repom");

            string xmlRetorno = string.Empty;

            ServicoRepom.Integracao.IntegracaoSoapClient svcIntegracaoRepom = new Servicos.Embarcador.Integracao.ConfiguracaoWebService(_unitOfWork).ObterClient<ServicoRepom.Integracao.IntegracaoSoapClient, ServicoRepom.Integracao.IntegracaoSoap>(Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoWebServiceIntegracao.Repom_Integracao, out inspector);

            bool retorno = svcIntegracaoRepom.SolicitaRoteiro(consultaRoteiros.Body.strCliente, consultaRoteiros.Body.strAssinaturaDigital, xmlEnvio, ref xmlRetorno);
            Servicos.Log.TratarErro("CadastrarRoteiro retorno:" + xmlRetorno, "Repom");

            if (!retorno)
                mensagemErro = $"Não foi possível solicitar o cadastro do roteiro de {consultaRoteiros.Body.strCidadeOrigem} até {consultaRoteiros.Body.strCidadeDestino} para a Repom.";

            return retorno;
        }

        private void CadastrarRoteiroREST(Dominio.Entidades.Embarcador.Cargas.ValePedagio.CargaConsultaValorPedagioIntegracao cargaValePedagio, string authToken)
        {
            ArquivoTransacao<Dominio.Entidades.Embarcador.Cargas.ValePedagio.CargaValePedagioIntegracaoArquivo> servicoArquivoTransacao = new ArquivoTransacao<Dominio.Entidades.Embarcador.Cargas.ValePedagio.CargaValePedagioIntegracaoArquivo>(_unitOfWork);

            string jsonRequisicao = string.Empty;
            string jsonRetorno = string.Empty;

            try
            {
                string urlIntegracao = $"{_integracaoRepom.URLRota}/api/Rota";
                HttpClient requisicao = CriarRequisicao(urlIntegracao, authToken);

                Dominio.ObjetosDeValor.Embarcador.Integracao.Repom.CadastroRoteiro cadastraRota = ObterCadastroRoteiroREST(cargaValePedagio);
                jsonRequisicao = JsonConvert.SerializeObject(cadastraRota, Formatting.Indented);
                StringContent conteudoRequisicao = new StringContent(jsonRequisicao.ToString(), Encoding.UTF8, "application/json");
                HttpResponseMessage retornoRequisicao = requisicao.PostAsync(urlIntegracao, conteudoRequisicao).Result;
                jsonRetorno = retornoRequisicao.Content.ReadAsStringAsync().Result;

                if (retornoRequisicao.StatusCode == HttpStatusCode.OK)
                {
                    Dominio.ObjetosDeValor.Embarcador.Integracao.Repom.RetornoCadastroRoteiro retorno = JsonConvert.DeserializeObject<Dominio.ObjetosDeValor.Embarcador.Integracao.Repom.RetornoCadastroRoteiro>(jsonRetorno);
                    if (retorno.Status == 1)
                    {
                        cargaValePedagio.CodigoRoteiro = retorno.CodigoRoteiroRepom.ToInt();
                        cargaValePedagio.CodigoPercurso = retorno.CodigoPercurso.ToInt();

                        cargaValePedagio.ProblemaIntegracao = "Cadastro do Roteiro efetuado com sucesso";
                    }
                    else
                    {
                        cargaValePedagio.ProblemaIntegracao = retorno.DescricaoErro;
                    }
                }
                else
                {
                    try
                    {
                        Dominio.ObjetosDeValor.Embarcador.Integracao.Repom.RetornoCadastroRoteiroErro retornoErro = JsonConvert.DeserializeObject<Dominio.ObjetosDeValor.Embarcador.Integracao.Repom.RetornoCadastroRoteiroErro>(jsonRetorno);
                        if (retornoErro != null)
                            cargaValePedagio.ProblemaIntegracao = string.Join(", ", retornoErro.Errors.Select(o => o.Codigo + " - " + o.Descricao));
                        else
                            cargaValePedagio.ProblemaIntegracao = $"Retorno cadastro: {retornoRequisicao.StatusCode}";
                    }
                    catch (Exception)
                    {
                        cargaValePedagio.ProblemaIntegracao = "Falha na Repom ao cadastrar roteiro";
                    }
                }
            }
            catch (Exception excecao)
            {
                Log.TratarErro(excecao);
                cargaValePedagio.ProblemaIntegracao = "Ocorreu uma falha ao realizar o cadastro de roteiro com a Repom";

            }

            servicoArquivoTransacao.Adicionar(cargaValePedagio, jsonRequisicao, jsonRetorno, "Json");
        }

        private bool CadastrarRoteiroREST(string url, string authToken, Dominio.Entidades.Localidade localidadeOrigem, Dominio.Entidades.Localidade localidadeDestino, List<Dominio.ObjetosDeValor.Embarcador.Integracao.Repom.CadastroRoteiroCidade> listaParadas, Dominio.Entidades.Embarcador.Cargas.ValePedagio.CargaIntegracaoValePedagio cargaValePedagio, ref int codigoRoteiro, ref int codigoPercurso, out string mensagemErro)
        {
            mensagemErro = null;
            string jsonRequisicao = string.Empty;
            string jsonRetorno = string.Empty;

            string urlCadastro = url + "/api/Rota";

            try
            {
                HttpClient requisicao = CriarRequisicao(urlCadastro, authToken);

                Dominio.ObjetosDeValor.Embarcador.Integracao.Repom.CadastroRoteiro cadastraRota = ObterCadastroRoteiroREST(cargaValePedagio, localidadeOrigem, localidadeDestino, listaParadas);
                jsonRequisicao = JsonConvert.SerializeObject(cadastraRota, Formatting.Indented);
                StringContent conteudoRequisicao = new StringContent(jsonRequisicao.ToString(), Encoding.UTF8, "application/json");
                HttpResponseMessage retornoRequisicao = requisicao.PostAsync(urlCadastro, conteudoRequisicao).Result;
                jsonRetorno = retornoRequisicao.Content.ReadAsStringAsync().Result;

                if (retornoRequisicao.StatusCode == HttpStatusCode.OK)
                {
                    Dominio.ObjetosDeValor.Embarcador.Integracao.Repom.RetornoCadastroRoteiro retorno = JsonConvert.DeserializeObject<Dominio.ObjetosDeValor.Embarcador.Integracao.Repom.RetornoCadastroRoteiro>(jsonRetorno);
                    if (retorno.Status == 1)
                    {
                        codigoRoteiro = retorno.CodigoRoteiroRepom.ToInt();
                        codigoPercurso = retorno.CodigoPercurso.ToInt();

                        mensagemErro = "Cadastro do Roteiro efetuado com sucesso";
                        SalvarJsonIntegracao(cargaValePedagio, jsonRequisicao, jsonRetorno, mensagemErro);
                        return true;
                    }
                    else
                    {
                        mensagemErro = retorno.DescricaoErro;
                        SalvarJsonIntegracao(cargaValePedagio, jsonRequisicao, jsonRetorno, mensagemErro);
                        return false;
                    }
                }
                else
                {
                    try
                    {
                        Dominio.ObjetosDeValor.Embarcador.Integracao.Repom.RetornoCadastroRoteiroErro retornoErro = JsonConvert.DeserializeObject<Dominio.ObjetosDeValor.Embarcador.Integracao.Repom.RetornoCadastroRoteiroErro>(jsonRetorno);
                        if (retornoErro != null)
                            mensagemErro = string.Join(", ", retornoErro.Errors.Select(o => o.Codigo + " - " + o.Descricao));
                        else
                            mensagemErro = $"Retorno cadastro: {retornoRequisicao.StatusCode}";
                    }
                    catch (Exception)
                    {
                        mensagemErro = "Falha na Repom ao cadastrar roteiro";
                    }

                    SalvarJsonIntegracao(cargaValePedagio, jsonRequisicao, jsonRetorno, mensagemErro);
                    return false;
                }
            }
            catch (Exception excecao)
            {
                Log.TratarErro(excecao);
                mensagemErro = "Ocorreu uma falha ao realizar o cadastro de roteiro com a Repom";

                SalvarJsonIntegracao(cargaValePedagio, jsonRequisicao, jsonRetorno, mensagemErro);
                return false;
            }
        }

        private Dominio.ObjetosDeValor.Embarcador.CIOT.Repom.CadastroRoteiro ObterCadastroRoteiro(ServicoRepom.Integracao.ConsultaRoteirosRequest consultaRoteiros, string codigoRotaEmbarcador, Dominio.Entidades.Embarcador.Cargas.Carga carga)
        {
            int codigoIBGEOrigem = consultaRoteiros.Body.strCodigoIBGEOrigem.ToInt();
            int codigoIBGEDestino = consultaRoteiros.Body.strCodigoIBGEDestino.ToInt();

            Dominio.ObjetosDeValor.Embarcador.CIOT.Repom.CadastroRoteiro cadastroRoteiroRepom = new Dominio.ObjetosDeValor.Embarcador.CIOT.Repom.CadastroRoteiro()
            {
                Roteiro = new Dominio.ObjetosDeValor.Embarcador.CIOT.Repom.CadastroRoteiroRoteiro()
                {
                    AlteraRoteiro = "1",
                    CEPDestino = "",
                    CEPOrigem = "",
                    CodigoFilial = "",
                    CodigoLocalQuitacao = "",
                    CodigoRota = codigoRotaEmbarcador,
                    EmailUsuario = string.Empty,
                    EstadoDestino = consultaRoteiros.Body.strEstadoDestino,
                    EstadoOrigem = consultaRoteiros.Body.strEstadoOrigem,
                    IBGEDestino = codigoIBGEDestino.ToString(),
                    IBGEOrigem = codigoIBGEOrigem.ToString(),
                    IdaVolta = RotaFreteComVolta(carga) ? "1" : "0",
                    NomeUsuario = "",
                    Observacao = "",
                    TelefoneUsuario = "",
                    TempoPrevistoViagem = "",
                    TipoLocalQuitacao = "",
                    TipoProcessoTransporte = "0",
                    Vias = new Dominio.ObjetosDeValor.Embarcador.CIOT.Repom.CadastroRoteiroRoteiroVia[] {
                        //new Dominio.ObjetosDeValor.Embarcador.CIOT.Repom.CadastroRoteiroRoteiroVia()
                        //{
                        //     Descricao = rota.Detalhes
                        //}
                    },
                    Paradas = !string.IsNullOrWhiteSpace(consultaRoteiros.Body.strParadas) ? consultaRoteiros.Body.strParadas : null
                }
            };

            return cadastroRoteiroRepom;
        }

        private Dominio.ObjetosDeValor.Embarcador.Integracao.Repom.CadastroRoteiro ObterCadastroRoteiroREST(Dominio.Entidades.Embarcador.Cargas.ValePedagio.CargaIntegracaoValePedagio cargaValePedagio, Dominio.Entidades.Localidade localidadeOrigem, Dominio.Entidades.Localidade localidadeDestino, List<Dominio.ObjetosDeValor.Embarcador.Integracao.Repom.CadastroRoteiroCidade> listaParadas)
        {
            string codigoRoteiroCliente = !string.IsNullOrWhiteSpace(cargaValePedagio.RotaFrete?.CodigoIntegracaoValePedagio) ? cargaValePedagio.RotaFrete.CodigoIntegracaoValePedagio : string.Empty;
            bool idaVolta = cargaValePedagio.RotaFrete != null ? cargaValePedagio.RotaFrete.TipoRota == TipoRotaFrete.IdaVolta : RotaFreteComVolta(cargaValePedagio.Carga);
            bool voltarPeloMesmoCaminhoIda = cargaValePedagio?.RotaFrete != null ? cargaValePedagio.RotaFrete?.VoltarPeloMesmoCaminhoIda ?? false : ObterRotaFreteVoltarPeloMesmoCaminhoIda(cargaValePedagio.Carga);

            (localidadeDestino, listaParadas) = AjustarLocalidadesParaIdaVolta(idaVolta, localidadeOrigem, localidadeDestino, listaParadas);

            return CriarCadastroRoteiro(codigoRoteiroCliente, idaVolta, localidadeOrigem, localidadeDestino, listaParadas, voltarPeloMesmoCaminhoIda);
        }

        private Dominio.ObjetosDeValor.Embarcador.Integracao.Repom.CadastroRoteiro ObterCadastroRoteiroREST(Dominio.Entidades.Embarcador.Cargas.ValePedagio.CargaConsultaValorPedagioIntegracao cargaValePedagio)
        {
            Servicos.Embarcador.Localidades.Localidade servicoLocalidade = new Servicos.Embarcador.Localidades.Localidade(_unitOfWork);

            servicoLocalidade.ObterLocalidadesValePedagioCarga(cargaValePedagio.Carga, out Dominio.Entidades.Localidade localidadeOrigem, out Dominio.Entidades.Localidade localidadeDestino, out List<Dominio.Entidades.Localidade> pontosPassagem, _unitOfWork);

            List<Dominio.ObjetosDeValor.Embarcador.Integracao.Repom.CadastroRoteiroCidade> listaParadas = new List<Dominio.ObjetosDeValor.Embarcador.Integracao.Repom.CadastroRoteiroCidade>();
            foreach (Dominio.Entidades.Localidade localidade in pontosPassagem)
            {
                Dominio.ObjetosDeValor.Embarcador.Integracao.Repom.CadastroRoteiroCidade parada = new Dominio.ObjetosDeValor.Embarcador.Integracao.Repom.CadastroRoteiroCidade()
                {
                    CodigoIBGE = localidade.CodigoIBGE.ToString("00000000").Substring(3, 5).ToInt().ToString(),
                    Estado = localidade.Estado.Sigla
                };
                listaParadas.Add(parada);
            }

            string codigoRoteiroCliente = !string.IsNullOrWhiteSpace(cargaValePedagio.RotaFrete?.CodigoIntegracaoValePedagio) ? cargaValePedagio.RotaFrete.CodigoIntegracaoValePedagio : string.Empty;
            bool idaVolta = cargaValePedagio.RotaFrete != null ? cargaValePedagio.RotaFrete.TipoRota == TipoRotaFrete.IdaVolta : RotaFreteComVolta(cargaValePedagio.Carga);
            bool voltarPeloMesmoCaminhoIda = cargaValePedagio?.RotaFrete != null ? cargaValePedagio.RotaFrete?.VoltarPeloMesmoCaminhoIda ?? false : ObterRotaFreteVoltarPeloMesmoCaminhoIda(cargaValePedagio.Carga);

            (localidadeDestino, listaParadas) = AjustarLocalidadesParaIdaVolta(idaVolta, localidadeOrigem, localidadeDestino, listaParadas);

            return CriarCadastroRoteiro(codigoRoteiroCliente, idaVolta, localidadeOrigem, localidadeDestino, listaParadas, voltarPeloMesmoCaminhoIda);
        }

        private (Dominio.Entidades.Localidade localidadeDestino, List<Dominio.ObjetosDeValor.Embarcador.Integracao.Repom.CadastroRoteiroCidade> listaParadas) AjustarLocalidadesParaIdaVolta(bool idaVolta, Dominio.Entidades.Localidade localidadeOrigem, Dominio.Entidades.Localidade localidadeDestino, List<Dominio.ObjetosDeValor.Embarcador.Integracao.Repom.CadastroRoteiroCidade> listaParadas)
        {
            if (idaVolta && _integracaoRepom.ConsiderarRotaFreteDaCargaNoValePedagio && listaParadas.Any() && localidadeOrigem.Codigo == localidadeDestino.Codigo)
            {
                Dominio.ObjetosDeValor.Embarcador.Integracao.Repom.CadastroRoteiroCidade ultimaParada = listaParadas.Last();

                localidadeDestino = new Dominio.Entidades.Localidade
                {
                    CodigoIBGE = int.Parse(ultimaParada.CodigoIBGE),
                    Estado = new Dominio.Entidades.Estado()
                    {
                        Sigla = ultimaParada.Estado
                    }
                };

                listaParadas.Remove(ultimaParada);
            }

            return (localidadeDestino, listaParadas);
        }

        private Dominio.ObjetosDeValor.Embarcador.Integracao.Repom.CadastroRoteiro CriarCadastroRoteiro(string codigoRoteiroCliente, bool idaVolta, Dominio.Entidades.Localidade localidadeOrigem, Dominio.Entidades.Localidade localidadeDestino, List<Dominio.ObjetosDeValor.Embarcador.Integracao.Repom.CadastroRoteiroCidade> listaParadas, bool voltarPeloMesmoCaminhoIda)
        {
            return new Dominio.ObjetosDeValor.Embarcador.Integracao.Repom.CadastroRoteiro()
            {
                CodigoRoteiroCliente = codigoRoteiroCliente,
                IdaVolta = idaVolta,
                Origem = new Dominio.ObjetosDeValor.Embarcador.Integracao.Repom.CadastroRoteiroCidade()
                {
                    CodigoIBGE = localidadeOrigem.CodigoIBGE.ToString("00000000").Substring(3, 5).ToInt().ToString(),
                    Estado = localidadeOrigem.Estado.Sigla
                },
                Paradas = listaParadas,
                Destino = new Dominio.ObjetosDeValor.Embarcador.Integracao.Repom.CadastroRoteiroCidade()
                {
                    CodigoIBGE = localidadeDestino.CodigoIBGE.ToString("00000000").Substring(3, 5).ToInt().ToString(),
                    Estado = localidadeDestino.Estado.Sigla
                },
                VoltarPeloMesmoCaminhoIda = voltarPeloMesmoCaminhoIda
            };
        }

        private bool ObterRotaFreteVoltarPeloMesmoCaminhoIda(Dominio.Entidades.Embarcador.Cargas.Carga carga)
        {
            return _integracaoRepom.ConsiderarRotaFreteDaCargaNoValePedagio ? (carga.Rota != null && carga.Rota.VoltarPeloMesmoCaminhoIda) : false;
        }

        private bool RotaFreteComVolta(Dominio.Entidades.Embarcador.Cargas.Carga carga)
        {
            if (_integracaoRepom.ConsiderarRotaFreteDaCargaNoValePedagio)
                return carga.Rota != null && (carga.Rota.VoltarPeloMesmoCaminhoIda || carga.Rota.TipoUltimoPontoRoteirizacao != TipoUltimoPontoRoteirizacao.PontoMaisDistante);

            if (_integracaoRepom.TipoIntegracaoRepom == TipoIntegracaoRepom.REsT && carga.Empresa != null && carga.Rota != null)
            {
                Repositorio.Embarcador.Transportadores.TransportadorRotaFreteValePedagio repTransportadorRotaFreteValePedagio = new Repositorio.Embarcador.Transportadores.TransportadorRotaFreteValePedagio(_unitOfWork);
                Dominio.Entidades.Embarcador.Transportadores.TransportadorRotaFreteValePedagio rotaFreteValePedagio = repTransportadorRotaFreteValePedagio.BuscarPorEmpresaERotaFrete(carga.Empresa.Codigo, carga.Rota.Codigo);

                if (rotaFreteValePedagio != null)
                    return rotaFreteValePedagio.TipoRotaFrete == TipoRotaFrete.IdaVolta;
            }

            if (_integracaoRepom.TipoRotaFreteRepom == TipoRotaFreteRepom.IdaVolta)
                return true;

            if (_integracaoRepom.TipoRotaFreteRepom == TipoRotaFreteRepom.Ida)
                return false;

            TipoUltimoPontoRoteirizacao? ultimoPontoPorTipoOperacao = new Pedido.TipoOperacao(_unitOfWork).ObterTipoUltimoPontoRoteirizacao(carga.TipoOperacao, carga.Empresa);
            TipoUltimoPontoRoteirizacao tipoUltimoPonto = ultimoPontoPorTipoOperacao ?? TipoUltimoPontoRoteirizacao.PontoMaisDistante;
            TipoRotaFrete tipoRotaFrete = tipoUltimoPonto == TipoUltimoPontoRoteirizacao.PontoMaisDistante ? TipoRotaFrete.Ida : TipoRotaFrete.IdaVolta;

            return tipoRotaFrete == TipoRotaFrete.IdaVolta;
        }

        private bool ConsultarValor(Dominio.Entidades.Embarcador.Cargas.Carga carga, string codigoRotaEmbarcador, int codigoRoteiro, int codigoPercurso, ref Dominio.ObjetosDeValor.Embarcador.CIOT.Repom.RetornoConsultaValorPedagio retornoConsultaValorPedagio, ref string mensagemErro, ref Servicos.Models.Integracao.InspectorBehavior inspector)
        {
            System.Net.ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;

            mensagemErro = null;
            bool eixosSuspensos = false;
            if (carga.TipoOperacao != null)
            {
                if (carga.TipoOperacao.TipoCarregamento.HasValue && carga.TipoOperacao.TipoCarregamento.Value == Dominio.ObjetosDeValor.Embarcador.Enumeradores.RetornoCargaTipo.Vazio)
                    eixosSuspensos = true;
            }

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

            Dominio.ObjetosDeValor.Embarcador.CIOT.Repom.ConsultaValorPedagio consultaValorPedagio = new Dominio.ObjetosDeValor.Embarcador.CIOT.Repom.ConsultaValorPedagio()
            {
                CodigoPercurso = codigoPercurso.ToString(),
                CodigoRoteiro = codigoRoteiro.ToString(),
                CodigoRoteiroCliente = codigoRotaEmbarcador,
                NumeroEixos = numeroEixos.ToString()
            };

            string xmlEnvio = Utilidades.XML.Serializar(consultaValorPedagio);

            ServicoRepom.Integracao.IntegracaoSoapClient svcRepom = new Servicos.Embarcador.Integracao.ConfiguracaoWebService(_unitOfWork).ObterClient<ServicoRepom.Integracao.IntegracaoSoapClient, ServicoRepom.Integracao.IntegracaoSoap>(Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoWebServiceIntegracao.Repom_Integracao, out inspector);

            string xmlSucesso = string.Empty, xmlErro = string.Empty;

            bool retorno = svcRepom.RoteiroValorTotalVPRs(_integracaoRepom.CodigoCliente, _integracaoRepom.AssinaturaDigital, xmlEnvio, ref xmlSucesso, ref xmlErro);

            if (retorno)
            {
                retornoConsultaValorPedagio = Utilidades.XML.Deserializar<Dominio.ObjetosDeValor.Embarcador.CIOT.Repom.RetornoConsultaValorPedagio>(xmlSucesso);

                return true;
            }
            else
            {
                Dominio.ObjetosDeValor.Embarcador.CIOT.Repom.RetornoIntegracaoErro retornoErro = Utilidades.XML.Deserializar<Dominio.ObjetosDeValor.Embarcador.CIOT.Repom.RetornoIntegracaoErro>(xmlErro);

                mensagemErro = string.Join(", ", retornoErro.Erros.Select(o => o.Codigo + " - " + o.Descricao));

                return false;
            }
        }

        private void ConsultarValorREST(Dominio.Entidades.Embarcador.Cargas.ValePedagio.CargaConsultaValorPedagioIntegracao cargaValePedagio, string authToken, AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware tipoServicoMultisoftware)
        {
            ArquivoTransacao<Dominio.Entidades.Embarcador.Cargas.ValePedagio.CargaValePedagioIntegracaoArquivo> servicoArquivoTransacao = new ArquivoTransacao<Dominio.Entidades.Embarcador.Cargas.ValePedagio.CargaValePedagioIntegracaoArquivo>(_unitOfWork);

            cargaValePedagio.DataIntegracao = DateTime.Now;
            cargaValePedagio.NumeroTentativas++;

            string jsonRequisicao = string.Empty;
            string jsonRetorno = string.Empty;

            try
            {
                string urlIntegracao = $"{_integracaoRepom.URLRota}/Api/Roteiro";
                HttpClient requisicao = CriarRequisicao(urlIntegracao, authToken);

                Dominio.ObjetosDeValor.Embarcador.Integracao.Repom.ConsultaValorPedagio consultaValorPedagio = ObterConsultaValePedagioREST(cargaValePedagio, tipoServicoMultisoftware);
                jsonRequisicao = JsonConvert.SerializeObject(consultaValorPedagio, Formatting.Indented);
                StringContent conteudoRequisicao = new StringContent(jsonRequisicao.ToString(), Encoding.UTF8, "application/json");
                HttpResponseMessage retornoRequisicao = requisicao.PostAsync(urlIntegracao, conteudoRequisicao).Result;
                jsonRetorno = retornoRequisicao.Content.ReadAsStringAsync().Result;

                if (retornoRequisicao.StatusCode == HttpStatusCode.OK)
                {
                    Dominio.ObjetosDeValor.Embarcador.Integracao.Repom.RetornoConsultaValorPedagio retorno = JsonConvert.DeserializeObject<Dominio.ObjetosDeValor.Embarcador.Integracao.Repom.RetornoConsultaValorPedagio>(jsonRetorno);

                    cargaValePedagio.ValorValePedagio = retorno.ValorTotalVpr;

                    cargaValePedagio.SituacaoIntegracao = SituacaoIntegracao.Integrado;
                    cargaValePedagio.ProblemaIntegracao = "Integradro com sucesso";

                    if (cargaValePedagio.ValorValePedagio == 0)
                        cargaValePedagio.ProblemaIntegracao = "Rota sem pedágio";
                }
                else
                {
                    dynamic retornoErro = JsonConvert.DeserializeObject<dynamic>(jsonRetorno);

                    string mensagemErro = string.Join(", ", retornoErro.Errors);

                    cargaValePedagio.ProblemaIntegracao = mensagemErro;
                    cargaValePedagio.SituacaoIntegracao = SituacaoIntegracao.ProblemaIntegracao;
                }
            }
            catch (Exception ex)
            {
                Log.TratarErro(ex);

                cargaValePedagio.ProblemaIntegracao = "Ocorreu uma falha ao realizar a integração de consulta com a Repom";
                cargaValePedagio.SituacaoIntegracao = SituacaoIntegracao.ProblemaIntegracao;
            }

            servicoArquivoTransacao.Adicionar(cargaValePedagio, jsonRequisicao, jsonRetorno, "Json");
        }

        private bool ComprarValePedagio(Dominio.Entidades.Embarcador.Cargas.Carga carga, string codigoRotaEmbarcador, int codigoRoteiro, int codigoPercurso, decimal valorValePedagio, ref Dominio.ObjetosDeValor.Embarcador.CIOT.Repom.RetornoCompraValePedagio retornoCompraValePedagio, ref string mensagemErro, ref Servicos.Models.Integracao.InspectorBehavior inspector, ref int numeroEixos, ref string numeroCartao)
        {
            System.Net.ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;

            Dominio.Entidades.Usuario motorista = null;
            if (carga.Motoristas?.Count > 0)
                motorista = carga.Motoristas.FirstOrDefault();

            numeroCartao = motorista != null ? motorista.NumeroCartao : string.Empty;

            Dominio.ObjetosDeValor.Embarcador.CIOT.Repom.CompraValePedagio compraValePedagio = new Dominio.ObjetosDeValor.Embarcador.CIOT.Repom.CompraValePedagio()
            {
                CodigoCliente = carga.CodigoCargaEmbarcador,
                CodigoFilial = !string.IsNullOrWhiteSpace(_integracaoRepom.CodigoFilial) ? _integracaoRepom.CodigoFilial : carga.Filial?.CodigoFilialEmbarcador,
                Cartao = !string.IsNullOrWhiteSpace(numeroCartao) ? numeroCartao : string.Empty,
                Roteiro = new Dominio.ObjetosDeValor.Embarcador.CIOT.Repom.CompraValePedagioRoteiro()
                {
                    CodigoPercurso = codigoPercurso.ToString(),
                    CodigoRoteiro = codigoRoteiro.ToString(),
                    CodigoRoteiroCliente = codigoRotaEmbarcador,
                    IBGECidadeOrigem = string.Empty,
                    CEPCidadeOrigem = string.Empty,
                    EstadoOrigem = string.Empty,
                    IBGECidadeDestino = string.Empty,
                    CEPCidadeDestino = string.Empty,
                    EstadoDestino = string.Empty
                },
                PlacaVeiculo = carga.Veiculo?.Placa,
                EixosVeiculo = carga.Veiculo?.ModeloVeicularCarga?.NumeroEixos.ToString()
            };

            if (motorista != null)
            {
                compraValePedagio.Motorista = new Dominio.ObjetosDeValor.Embarcador.CIOT.Repom.CompraValePedagioMotorista
                {
                    CPF = motorista.CPF,
                    Nome = motorista.Nome,
                    RG = !string.IsNullOrWhiteSpace(motorista.RG) ? motorista.RG : string.Empty,
                    Telefone = Utilidades.String.OnlyNumbers(motorista.Telefone)
                };
            }

            Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoUltimoPontoRoteirizacao? ultimoPontoPorTipoOperacao = new Pedido.TipoOperacao(_unitOfWork).ObterTipoUltimoPontoRoteirizacao(carga.TipoOperacao, carga.Empresa);

            if (carga.VeiculosVinculados?.Count > 0)
            {
                compraValePedagio.PlacaCarreta = carga.VeiculosVinculados.FirstOrDefault().Placa;
                compraValePedagio.EixosCarreta = carga.ModeloVeicularCarga?.NumeroEixos.ToString();
                compraValePedagio.EixosSuspensosIda = carga.ModeloVeicularCarga?.NumeroEixosSuspensos.ToString();
                compraValePedagio.EixosSuspensosVolta = ultimoPontoPorTipoOperacao == Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoUltimoPontoRoteirizacao.PontoMaisDistante ? "0" : carga.ModeloVeicularCarga?.NumeroEixosSuspensos.ToString();
                compraValePedagio.CodigoCentroCusto = string.Empty;
                compraValePedagio.TipoCentroCusto = string.Empty;
            }
            else
            {
                compraValePedagio.PlacaCarreta = string.Empty;
                compraValePedagio.EixosCarreta = string.Empty;
                compraValePedagio.EixosSuspensosIda = string.Empty;
                compraValePedagio.EixosSuspensosVolta = string.Empty;
                compraValePedagio.CodigoCentroCusto = string.Empty;
                compraValePedagio.TipoCentroCusto = string.Empty;
            }

            if (carga.CargaCTes.Count > 0)
            {
                compraValePedagio.Documentos = new Dominio.ObjetosDeValor.Embarcador.CIOT.Repom.CompraValePedagioDocumentos()
                {
                    Documento = carga.CargaCTes.Select(o => new Dominio.ObjetosDeValor.Embarcador.CIOT.Repom.CompraValePedagioDocumento
                    {
                        Numero = o.CTe.Numero.ToString(),
                        Serie = o.CTe.Serie.Numero.ToString(),
                        CodigoFilial = !string.IsNullOrWhiteSpace(_integracaoRepom.CodigoFilial) ? _integracaoRepom.CodigoFilial : carga.Filial?.CodigoFilialEmbarcador
                    }).ToArray()
                };
            }

            numeroEixos = (carga.Veiculo?.ModeloVeicularCarga?.NumeroEixos ?? 0) + (carga.ModeloVeicularCarga?.NumeroEixos ?? 0) - (carga.ModeloVeicularCarga?.NumeroEixosSuspensos ?? 0);
            compraValePedagio.Concessionarias = string.Empty;
            compraValePedagio.Pracas = string.Empty;

            ServicoRepom.Integracao.IntegracaoSoapClient svcRepom = new Servicos.Embarcador.Integracao.ConfiguracaoWebService(_unitOfWork).ObterClient<ServicoRepom.Integracao.IntegracaoSoapClient, ServicoRepom.Integracao.IntegracaoSoap>(Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoWebServiceIntegracao.Repom_Integracao, out inspector);

            string xmlEnvio = Utilidades.XML.Serializar(compraValePedagio);
            string xmlSucesso = string.Empty, xmlErro = string.Empty;

            bool retorno = svcRepom.EmiteViagemVPR(_integracaoRepom.CodigoCliente, _integracaoRepom.AssinaturaDigital, xmlEnvio, ref xmlSucesso, ref xmlErro);

            if (retorno)
            {
                retornoCompraValePedagio = Utilidades.XML.Deserializar<Dominio.ObjetosDeValor.Embarcador.CIOT.Repom.RetornoCompraValePedagio>(xmlSucesso);

                return true;
            }
            else
            {
                Dominio.ObjetosDeValor.Embarcador.CIOT.Repom.RetornoIntegracaoErro retornoErro = Utilidades.XML.Deserializar<Dominio.ObjetosDeValor.Embarcador.CIOT.Repom.RetornoIntegracaoErro>(xmlErro);

                mensagemErro = string.Join(", ", retornoErro.Erros.Select(o => o.Codigo + " - " + o.Descricao));

                return false;
            }

        }

        private void ComprarValePedagioREST(Dominio.Entidades.Embarcador.Cargas.Carga carga, Dominio.Entidades.Embarcador.Cargas.ValePedagio.CargaIntegracaoValePedagio cargaValePedagio, int codigoRoteiro, int codigoPercurso)
        {
            Repositorio.Embarcador.Cargas.ValePedagio.CargaIntegracaoValePedagio repCargaValePedagio = new Repositorio.Embarcador.Cargas.ValePedagio.CargaIntegracaoValePedagio(_unitOfWork);

            string jsonRequisicao = string.Empty;
            string jsonRetorno = string.Empty;

            cargaValePedagio.DataIntegracao = DateTime.Now;
            cargaValePedagio.NumeroTentativas += 1;

            try
            {
                string authToken = ObterTokenViagem();

                if (string.IsNullOrWhiteSpace(authToken))
                    throw new ServicoException("Não retornou o token de integração.");

                string url = _integracaoRepom.URLViagem + "/api/RepomViagem/ValePedagio";
                HttpClient requisicao = CriarRequisicao(url, authToken);

                Dominio.ObjetosDeValor.Embarcador.Integracao.Repom.CompraValePedagio compraValePedagio = ObterCompraValePedagioREST(carga, codigoRoteiro, codigoPercurso);

                cargaValePedagio.TipoCompra = !string.IsNullOrWhiteSpace(compraValePedagio.NumeroCartao) ? Dominio.Enumeradores.TipoCompraValePedagio.Cartao : Dominio.Enumeradores.TipoCompraValePedagio.Tag;
                cargaValePedagio.QuantidadeEixos = compraValePedagio.NumeroEixos + compraValePedagio.CarretaNumeroEixos - (compraValePedagio.Configuracao?.EixosSuspensosIda ?? 0);

                jsonRequisicao = JsonConvert.SerializeObject(compraValePedagio, Formatting.Indented, new JsonSerializerSettings() { NullValueHandling = NullValueHandling.Ignore });
                StringContent conteudoRequisicao = new StringContent(jsonRequisicao.ToString(), Encoding.UTF8, "application/json");

                HttpResponseMessage retornoRequisicao = requisicao.PostAsync(url, conteudoRequisicao).Result;
                jsonRetorno = retornoRequisicao.Content.ReadAsStringAsync().Result;

                if (retornoRequisicao.StatusCode == HttpStatusCode.OK)
                {
                    Dominio.ObjetosDeValor.Embarcador.Integracao.Repom.RetornoCompraValePedagio retorno = JsonConvert.DeserializeObject<Dominio.ObjetosDeValor.Embarcador.Integracao.Repom.RetornoCompraValePedagio>(jsonRetorno);
                    if (retorno.Result.Status)
                    {
                        cargaValePedagio.NumeroValePedagio = "";
                        cargaValePedagio.IdCompraValePedagio = retorno.CodigoViagem.ToString();
                        cargaValePedagio.NumeroValePedagio = retorno.idVpoAntt?.FirstOrDefault() ?? retorno.CodigoViagem.ToString();
                        cargaValePedagio.ValorValePedagio = retorno.Valor;
                        cargaValePedagio.SituacaoValePedagio = SituacaoValePedagio.Comprada;
                        cargaValePedagio.SituacaoIntegracao = SituacaoIntegracao.AgRetorno;
                        cargaValePedagio.ProblemaIntegracao = "Vale Pedágio Comprado com Sucesso";

                        SalvarDadosRetornoCompraValePedagioREST(retorno, carga);
                    }
                    else if (retorno.Result.NumeroErro.Equals("167"))//Quando retorna "167 - VPR: Roteiro/Percurso não cadastrado Roteiro: ..., Percurso: ...", define como sem pedágio no percurso
                    {
                        cargaValePedagio.ProblemaIntegracao = "Rota sem valor de Vale pedágio na Repom";
                        cargaValePedagio.SituacaoIntegracao = SituacaoIntegracao.Integrado;
                        cargaValePedagio.SituacaoValePedagio = SituacaoValePedagio.RotaSemCusto;
                        repCargaValePedagio.Atualizar(cargaValePedagio);
                    }
                    else
                        throw new ServicoException(retorno.Result.NumeroErro + " - " + retorno.Result.Descricao);
                }
                else
                    throw new ServicoException($"Retorno compra: {(int)retornoRequisicao.StatusCode}");
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
                cargaValePedagio.ProblemaIntegracao = "Ocorreu uma falha ao realizar a compra do Vale Pedágio da Repom";
            }

            SalvarJsonIntegracao(cargaValePedagio, jsonRequisicao, jsonRetorno);

            repCargaValePedagio.Atualizar(cargaValePedagio);
        }

        private Dominio.ObjetosDeValor.Embarcador.Integracao.Repom.CompraValePedagio ObterCompraValePedagioREST(Dominio.Entidades.Embarcador.Cargas.Carga carga, int codigoRoteiro, int codigoPercurso)
        {
            Dominio.Entidades.Usuario motorista = null;
            if (carga.Motoristas?.Count > 0)
                motorista = carga.Motoristas.FirstOrDefault();

            Dominio.ObjetosDeValor.Embarcador.Integracao.Repom.CompraValePedagio compraValePedagio = new Dominio.ObjetosDeValor.Embarcador.Integracao.Repom.CompraValePedagio()
            {
                ProcessoCodigoCliente = carga.CodigoCargaEmbarcador,
                Placa = carga.Veiculo?.Placa,
                NumeroEixos = carga.Veiculo?.ModeloVeicularCarga?.NumeroEixos ?? 0,

                NumeroCartao = !string.IsNullOrWhiteSpace(motorista?.NumeroCartao) ? motorista?.NumeroCartao : null,
                NomeMotorista = !string.IsNullOrWhiteSpace(motorista?.Nome) ? motorista?.Nome : string.Empty,
                DocumentoMotorista = !string.IsNullOrWhiteSpace(motorista?.CPF) ? motorista?.CPF : string.Empty,

                NomeTransportador = carga.Empresa.RazaoSocial,
                DocumentoTransportador = carga.Empresa.CNPJ_SemFormato,

                CnpjFilial = _integracaoRepom.FilialCompra?.CNPJ ?? carga.Filial?.CNPJ ?? string.Empty,
                UsuarioEmissao = carga.Operador?.Nome ?? string.Empty,

                Roteiro = new Dominio.ObjetosDeValor.Embarcador.Integracao.Repom.CompraValePedagioRoteiro()
                {
                    RoteiroCodigo = codigoRoteiro,
                    PercursoCodigo = codigoPercurso
                },

                Pracas = new List<int>() { 0 }
            };

            if (carga.Veiculo != null && carga.Veiculo.Tipo == "T")
            {
                if (carga.Veiculo.VeiculoCooperado)
                {
                    compraValePedagio.NomeTransportador = carga.Veiculo.EmpresaVeiculoCooperado?.RazaoSocial;
                    compraValePedagio.DocumentoTransportador = carga.Veiculo.EmpresaVeiculoCooperado?.CNPJ_SemFormato;
                }
                else
                {
                    compraValePedagio.NomeTransportador = carga.Veiculo.Proprietario.Nome;
                    compraValePedagio.DocumentoTransportador = carga.Veiculo.Proprietario.CPF_CNPJ_SemFormato;
                }
            }

            compraValePedagio.Configuracao = new Dominio.ObjetosDeValor.Embarcador.Integracao.Repom.CompraValePedagioConfiguracao();
            if (carga.VeiculosVinculados?.Count > 0)
            {
                TipoUltimoPontoRoteirizacao? ultimoPontoPorTipoOperacao = new Pedido.TipoOperacao(_unitOfWork).ObterTipoUltimoPontoRoteirizacao(carga.TipoOperacao, carga.Empresa);

                compraValePedagio.CarretaPlaca = carga.VeiculosVinculados.FirstOrDefault().Placa;
                compraValePedagio.CarretaNumeroEixos = carga.VeiculosVinculados != null && carga.VeiculosVinculados?.Count > 0 ? carga.VeiculosVinculados.Sum(o => o.ModeloVeicularCarga.NumeroEixos).Value : carga.ModeloVeicularCarga?.NumeroEixos ?? 0;


                if (_integracaoRepom.ConsiderarRotaFreteDaCargaNoValePedagio)
                {
                    (bool pontoMaisDistante, bool cargaVaziaIda, bool cargaVaziaVolta) = ObterUltimoPontoRoteirizacao(carga);
                    (bool eixosSuspensosIda, bool eixosSuspensosVolta, bool eixosSuspensos) = ObterRegraEixoSuspenso(pontoMaisDistante, cargaVaziaIda, cargaVaziaVolta);

                    compraValePedagio.Configuracao.EixosSuspensosIda = eixosSuspensosIda ? (carga.ModeloVeicularCarga?.NumeroEixosSuspensos ?? 0) : 0;
                    compraValePedagio.Configuracao.EixosSuspensosVolta = eixosSuspensosVolta ? (carga.ModeloVeicularCarga?.NumeroEixosSuspensos ?? 0) : 0;
                }
                else
                {
                    compraValePedagio.Configuracao.EixosSuspensosIda = carga.TipoOperacao?.TipoCarregamento == RetornoCargaTipo.Vazio ? (carga.ModeloVeicularCarga?.NumeroEixosSuspensos ?? 0) : 0;
                    compraValePedagio.Configuracao.EixosSuspensosVolta = ultimoPontoPorTipoOperacao == TipoUltimoPontoRoteirizacao.PontoMaisDistante || carga.TipoOperacao?.TipoCarregamento == RetornoCargaTipo.Vazio ? 0 : carga.ModeloVeicularCarga?.NumeroEixosSuspensos ?? 0;
                }

                compraValePedagio.Configuracao.RoteiroPagamentoPedagio = "PagamentoPedagioNormal";
                compraValePedagio.UtilizaCarreta = "True";
            }
            else
            {
                compraValePedagio.CarretaPlaca = string.Empty;
                compraValePedagio.CarretaNumeroEixos = 0;
                compraValePedagio.Configuracao.EixosSuspensosIda = 0;
                compraValePedagio.Configuracao.EixosSuspensosVolta = 0;
                compraValePedagio.Configuracao.RoteiroPagamentoPedagio = "PagamentoPedagioNormal";
                compraValePedagio.UtilizaCarreta = "False";
            }

            compraValePedagio.Documento = new List<Dominio.ObjetosDeValor.Embarcador.Integracao.Repom.CompraValePedagioDocumento>();
            if (carga.CargaCTes.Count > 0)
            {
                foreach (Dominio.Entidades.Embarcador.Cargas.CargaCTe cargaCTe in carga.CargaCTes)
                {
                    compraValePedagio.Documento.Add(new Dominio.ObjetosDeValor.Embarcador.Integracao.Repom.CompraValePedagioDocumento
                    {
                        DocumentoCodigo = cargaCTe.CTe.Numero.ToString(),
                        Serie = cargaCTe.CTe.Serie.Numero.ToString()
                    });
                }
            }

            return compraValePedagio;
        }

        private Dominio.ObjetosDeValor.Embarcador.Integracao.Repom.ConsultaValorPedagio ObterConsultaValePedagioREST(Dominio.Entidades.Embarcador.Cargas.ValePedagio.CargaConsultaValorPedagioIntegracao cargaValePedagio, AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware tipoServicoMultisoftware)
        {
            (bool pontoMaisDistante, bool cargaVaziaIda, bool cargaVaziaVolta) = ObterUltimoPontoRoteirizacao(cargaValePedagio.Carga);

            (bool eixosSuspensosIda, bool eixosSuspensosVolta, bool eixosSuspensos) = ObterRegraEixoSuspenso(pontoMaisDistante, cargaVaziaIda, cargaVaziaVolta);

            (int numeroEixos, int numeroEixosSuspensos) = ObterNumeroEixos(cargaValePedagio.Carga, eixosSuspensos, tipoServicoMultisoftware);

            Dominio.ObjetosDeValor.Embarcador.Integracao.Repom.ConsultaValorPedagio consultaValorPedagio = new Dominio.ObjetosDeValor.Embarcador.Integracao.Repom.ConsultaValorPedagio()
            {
                CodigoPercurso = cargaValePedagio.CodigoPercurso.ToString(),
                CodigoRoteiro = cargaValePedagio.CodigoRoteiro.ToString(),
                NumeroEixos = numeroEixos.ToString(),
                NumeroEixosSuspensosIda = eixosSuspensosIda ? numeroEixosSuspensos.ToString() : "0",
                NumeroEixosSuspensosVolta = eixosSuspensosVolta ? numeroEixosSuspensos.ToString() : "0"
            };

            return consultaValorPedagio;
        }

        private void SalvarDadosRetornoCompraValePedagio(Dominio.ObjetosDeValor.Embarcador.CIOT.Repom.RetornoCompraValePedagio retornoCompraValePedagio, Dominio.Entidades.Embarcador.Cargas.Carga carga)
        {
            Repositorio.Embarcador.Cargas.ValePedagio.CargaValePedagioDadosCompra repositorioValePedagioDadosCompra = new Repositorio.Embarcador.Cargas.ValePedagio.CargaValePedagioDadosCompra(_unitOfWork);
            Repositorio.Embarcador.Cargas.ValePedagio.CargaValePedagioDadosCompraPraca repositorioValePedagioDadosCompraPraca = new Repositorio.Embarcador.Cargas.ValePedagio.CargaValePedagioDadosCompraPraca(_unitOfWork);

            Dominio.Entidades.Embarcador.Cargas.ValePedagio.CargaValePedagioDadosCompra dadosCompra = new Dominio.Entidades.Embarcador.Cargas.ValePedagio.CargaValePedagioDadosCompra
            {
                Carga = carga,
                CodigoFilialCliente = retornoCompraValePedagio.CodigoFilialCliente,
                CodigoProcessoCliente = retornoCompraValePedagio.CodigoProcessoCliente,
                CodigoViagem = retornoCompraValePedagio.CodigoViagem.ToInt(),
                DataEmissao = retornoCompraValePedagio.DataEmissao.ToDateTime(),
                ValorTotalPedagios = retornoCompraValePedagio.ValorTotalPedagios.ToDecimal()
            };

            repositorioValePedagioDadosCompra.Inserir(dadosCompra);

            int quantidadeDePracas = retornoCompraValePedagio.Pedagios.Pedagio.Count();

            for (int i = 0; i < quantidadeDePracas; i++)
            {
                Dominio.ObjetosDeValor.Embarcador.CIOT.Repom.RetornoCompraValePedagioPedagio pracaPedagio = retornoCompraValePedagio.Pedagios.Pedagio[i];
                Dominio.Entidades.Embarcador.Cargas.ValePedagio.CargaValePedagioDadosCompraPraca praca = new Dominio.Entidades.Embarcador.Cargas.ValePedagio.CargaValePedagioDadosCompraPraca
                {
                    CargaValePedagioDadosCompra = dadosCompra,
                    CodigoPraca = pracaPedagio.CodigoPraca.ToInt(),
                    ConcessionariaCodigo = pracaPedagio.ConcessionarioaCodigo.ToInt(),
                    ConcessionariaNome = pracaPedagio.ConcessionariaNome,
                    NomePraca = pracaPedagio.NomePraca,
                    NumeroEixos = pracaPedagio.NumeroEixos.ToInt(),
                    Valor = pracaPedagio.Valor.ToDecimal()
                };

                repositorioValePedagioDadosCompraPraca.Inserir(praca);
            }
        }

        private void SalvarDadosRetornoCompraValePedagioREST(Dominio.ObjetosDeValor.Embarcador.Integracao.Repom.RetornoCompraValePedagio retornoCompraValePedagio, Dominio.Entidades.Embarcador.Cargas.Carga carga)
        {
            Repositorio.Embarcador.Cargas.ValePedagio.CargaValePedagioDadosCompra repositorioValePedagioDadosCompra = new Repositorio.Embarcador.Cargas.ValePedagio.CargaValePedagioDadosCompra(_unitOfWork);
            Repositorio.Embarcador.Cargas.ValePedagio.CargaValePedagioDadosCompraPraca repositorioValePedagioDadosCompraPraca = new Repositorio.Embarcador.Cargas.ValePedagio.CargaValePedagioDadosCompraPraca(_unitOfWork);

            Dominio.Entidades.Embarcador.Cargas.ValePedagio.CargaValePedagioDadosCompra dadosCompra = new Dominio.Entidades.Embarcador.Cargas.ValePedagio.CargaValePedagioDadosCompra
            {
                Carga = carga,
                CodigoFilialCliente = retornoCompraValePedagio.FilialCNPJ,
                CodigoProcessoCliente = carga.CodigoCargaEmbarcador,
                CodigoViagem = retornoCompraValePedagio.CodigoViagem,
                DataEmissao = retornoCompraValePedagio.DataEmissao,
                ValorTotalPedagios = retornoCompraValePedagio.Valor
            };

            repositorioValePedagioDadosCompra.Inserir(dadosCompra);

            foreach (Dominio.ObjetosDeValor.Embarcador.Integracao.Repom.RetornoCompraValePedagioPraca pracaPedagio in retornoCompraValePedagio.Pracas)
            {
                Dominio.Entidades.Embarcador.Cargas.ValePedagio.CargaValePedagioDadosCompraPraca praca = new Dominio.Entidades.Embarcador.Cargas.ValePedagio.CargaValePedagioDadosCompraPraca
                {
                    CargaValePedagioDadosCompra = dadosCompra,
                    CodigoPraca = pracaPedagio.Id,
                    ConcessionariaCodigo = pracaPedagio.ConcessionariaCodigo,
                    ConcessionariaNome = pracaPedagio.NomeConcessionaria,
                    NomePraca = pracaPedagio.NomePraca,
                    NomeRodovia = pracaPedagio.NomeRodovia,
                    NumeroEixos = pracaPedagio.PracaPreco.FirstOrDefault().QuantidadeEixos,
                    Valor = pracaPedagio.PracaPreco.FirstOrDefault().Preco
                };

                repositorioValePedagioDadosCompraPraca.Inserir(praca);
            }
        }

        private void SalvarXMLIntegracao(ref Dominio.Entidades.Embarcador.Cargas.ValePedagio.CargaIntegracaoValePedagio cargaValePedagio, Servicos.Models.Integracao.InspectorBehavior inspector, string mensagemRetorno)
        {
            Repositorio.Embarcador.Cargas.ValePedagio.CargaValePedagioIntegracaoArquivo repCargaValePedagioIntegracaoArquivo = new Repositorio.Embarcador.Cargas.ValePedagio.CargaValePedagioIntegracaoArquivo(_unitOfWork);

            Dominio.Entidades.Embarcador.Cargas.ValePedagio.CargaValePedagioIntegracaoArquivo cargaValePedagioIntegracaoArquivo = new Dominio.Entidades.Embarcador.Cargas.ValePedagio.CargaValePedagioIntegracaoArquivo();
            cargaValePedagioIntegracaoArquivo.ArquivoRequisicao = Servicos.Embarcador.Integracao.ArquivoIntegracao.SalvarArquivoIntegracao(inspector.LastRequestXML, "xml", _unitOfWork);
            cargaValePedagioIntegracaoArquivo.ArquivoResposta = Servicos.Embarcador.Integracao.ArquivoIntegracao.SalvarArquivoIntegracao(inspector.LastResponseXML, "xml", _unitOfWork);
            cargaValePedagioIntegracaoArquivo.Data = DateTime.Now;
            cargaValePedagioIntegracaoArquivo.Mensagem = mensagemRetorno;
            cargaValePedagioIntegracaoArquivo.Tipo = TipoArquivoIntegracaoCTeCarga.EnvioParaProcessamento;

            repCargaValePedagioIntegracaoArquivo.Inserir(cargaValePedagioIntegracaoArquivo);
            cargaValePedagio.ArquivosTransacao.Add(cargaValePedagioIntegracaoArquivo);
        }

        private void SalvarJsonIntegracao(Dominio.Entidades.Embarcador.Cargas.ValePedagio.CargaIntegracaoValePedagio cargaValePedagio, string jsonRequisicao, string jsonRetorno, string mensagemRetorno = null)
        {
            if (string.IsNullOrWhiteSpace(jsonRequisicao) && string.IsNullOrWhiteSpace(jsonRetorno))
                return;

            Repositorio.Embarcador.Cargas.ValePedagio.CargaValePedagioIntegracaoArquivo repCargaValePedagioIntegracaoArquivo = new Repositorio.Embarcador.Cargas.ValePedagio.CargaValePedagioIntegracaoArquivo(_unitOfWork);
            Dominio.Entidades.Embarcador.Cargas.ValePedagio.CargaValePedagioIntegracaoArquivo cargaValePedagioIntegracaoArquivo = new Dominio.Entidades.Embarcador.Cargas.ValePedagio.CargaValePedagioIntegracaoArquivo()
            {
                ArquivoRequisicao = ArquivoIntegracao.SalvarArquivoIntegracao(jsonRequisicao, "json", _unitOfWork),
                ArquivoResposta = ArquivoIntegracao.SalvarArquivoIntegracao(jsonRetorno, "json", _unitOfWork),
                Data = cargaValePedagio.DataIntegracao,
                Mensagem = !string.IsNullOrWhiteSpace(mensagemRetorno) ? mensagemRetorno : cargaValePedagio.ProblemaIntegracao,
                Tipo = TipoArquivoIntegracaoCTeCarga.EnvioParaProcessamento
            };

            repCargaValePedagioIntegracaoArquivo.Inserir(cargaValePedagioIntegracaoArquivo);

            if (cargaValePedagio.ArquivosTransacao == null)
                cargaValePedagio.ArquivosTransacao = new List<Dominio.Entidades.Embarcador.Cargas.ValePedagio.CargaValePedagioIntegracaoArquivo>();

            cargaValePedagio.ArquivosTransacao.Add(cargaValePedagioIntegracaoArquivo);
        }

        private HttpClient CriarRequisicao(string url, string accessToken = null)
        {
            ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;
            HttpClient requisicao = HttpClientFactoryWrapper.GetClient(nameof(ValePedagio));

            requisicao.BaseAddress = new Uri(url);
            requisicao.DefaultRequestHeaders.Accept.Clear();
            requisicao.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            requisicao.Timeout = TimeSpan.FromMinutes(3);

            if (!string.IsNullOrWhiteSpace(accessToken))
                requisicao.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);

            return requisicao;
        }

        private string ObterTokenRota()
        {
            if (string.IsNullOrWhiteSpace(_integracaoRepom.URLAutenticacaoRota))
                return null;

            ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;
            HttpClient client = HttpClientFactoryWrapper.GetClient(nameof(ValePedagio));

            client.DefaultRequestHeaders.Accept.Clear();
            client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/x-www-form-urlencoded"));

            FormUrlEncodedContent content = new FormUrlEncodedContent(new Dictionary<string, string>
            {
                { "client_id", _integracaoRepom.ClientID },
                { "client_secret", _integracaoRepom.ClientSecret },
                { "grant_type", "client_credentials" }
            });

            HttpResponseMessage result = client.PostAsync(_integracaoRepom.URLAutenticacaoRota, content).Result;
            string jsonResponse = result.Content.ReadAsStringAsync().Result;

            if (!result.IsSuccessStatusCode)
                return null;

            dynamic obj = Newtonsoft.Json.JsonConvert.DeserializeObject<dynamic>(jsonResponse);

            return (string)obj.access_token;
        }

        private string ObterTokenViagem()
        {
            if (string.IsNullOrWhiteSpace(_integracaoRepom.URLViagem))
                throw new ServicoException("Configuração em REST da Repom não está completa, favor atualizar o cadastro da integração");

            ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;

            string url = _integracaoRepom.URLViagem + "/api/Token";
            HttpClient requisicao = CriarRequisicao(url);

            Dominio.ObjetosDeValor.Embarcador.Integracao.Repom.Autenticacao autenticacao = new Dominio.ObjetosDeValor.Embarcador.Integracao.Repom.Autenticacao()
            {
                Usuario = _integracaoRepom.Usuario,
                Senha = _integracaoRepom.Senha
            };

            string jsonRequisicao = JsonConvert.SerializeObject(autenticacao, Formatting.Indented);
            StringContent conteudoRequisicao = new StringContent(jsonRequisicao.ToString(), Encoding.UTF8, "application/json");

            HttpResponseMessage retornoRequisicao = requisicao.PostAsync(url, conteudoRequisicao).Result;
            string jsonRetorno = retornoRequisicao.Content.ReadAsStringAsync().Result;

            if (retornoRequisicao.StatusCode == HttpStatusCode.OK)
            {
                dynamic obj = Newtonsoft.Json.JsonConvert.DeserializeObject<dynamic>(jsonRetorno);
                return (string)obj.accessToken;
            }
            else
                return null;
        }

        private (int numeroEixos, int numeroEixosSuspensos) ObterNumeroEixos(Dominio.Entidades.Embarcador.Cargas.Carga carga, bool eixosSuspensos, AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware tipoServicoMultisoftware)
        {
            if (!_integracaoRepom.ConsiderarEixosSuspensosNaConsultaDoValePedagio)
                if (tipoServicoMultisoftware != AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiTMS)
                    return (carga.ModeloVeicularCarga?.NumeroEixos ?? 0, carga.ModeloVeicularCarga?.NumeroEixosSuspensos ?? 0);
                else eixosSuspensos = _integracaoRepom.ConsiderarEixosSuspensosNaConsultaDoValePedagio;

            int numeroEixos = 0;
            int numeroEixosSuspensos = 0;

            Dominio.Entidades.Embarcador.Cargas.ModeloVeicularCarga modeloVeicular = carga.Veiculo != null ? carga.Veiculo.ModeloVeicularCarga : carga.ModeloVeicularCarga;

            if (modeloVeicular != null)
            {
                numeroEixos = modeloVeicular.NumeroEixos ?? 0;

                if (eixosSuspensos)
                    numeroEixosSuspensos = modeloVeicular.NumeroEixosSuspensos ?? 0;

                if (carga.VeiculosVinculados != null)
                {
                    foreach (Dominio.Entidades.Veiculo reboque in carga.VeiculosVinculados.ToList())
                    {
                        numeroEixos += reboque.ModeloVeicularCarga.NumeroEixos ?? 0;

                        if (eixosSuspensos)
                            numeroEixosSuspensos += reboque.ModeloVeicularCarga.NumeroEixosSuspensos ?? 0;
                    }
                }
            }

            return (numeroEixos, numeroEixosSuspensos);
        }

        private void ObterConfiguracaoIntegracaoRepom(Dominio.Entidades.Embarcador.Cargas.Carga carga, AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware tipoServicoMultisoftware)
        {
            if (_integracaoRepom != null)
                return;

            Servicos.Embarcador.Frota.ValePedagio servicoValePedagio = new Servicos.Embarcador.Frota.ValePedagio(_unitOfWork);
            _integracaoRepom = servicoValePedagio.ObterIntegracaoRepom(carga, tipoServicoMultisoftware);
        }

        private (bool pontoMaisDistante, bool cargaVaziaIda, bool cargaVaziaVolta) ObterUltimoPontoRoteirizacao(Dominio.Entidades.Embarcador.Cargas.Carga carga)
        {
            if (_integracaoRepom.ConsiderarRotaFreteDaCargaNoValePedagio)
            {
                bool tipoUltimoPontoRoteirizacao = !carga.Rota.VoltarPeloMesmoCaminhoIda &&  carga.Rota?.TipoUltimoPontoRoteirizacao == TipoUltimoPontoRoteirizacao.PontoMaisDistante;               

                return (
                    tipoUltimoPontoRoteirizacao,
                    carga.Rota?.TipoCarregamentoIda == RetornoCargaTipo.Vazio,
                    !tipoUltimoPontoRoteirizacao && carga.Rota?.TipoCarregamentoVolta == RetornoCargaTipo.Vazio
                );
            }

            bool pontoMaisDistante = new Pedido.TipoOperacao(_unitOfWork).ObterTipoUltimoPontoRoteirizacao(carga.TipoOperacao, carga.Empresa) == TipoUltimoPontoRoteirizacao.PontoMaisDistante;
            bool cargaVazia = carga.TipoOperacao?.TipoCarregamento == RetornoCargaTipo.Vazio;

            return (pontoMaisDistante, cargaVazia, cargaVazia);
        }

        private (bool eixosSuspensosIda, bool eixosSuspensosVolta, bool eixosSuspensos) ObterRegraEixoSuspenso(bool pontoMaisDistante, bool cargaVaziaIda, bool cargaVaziaVolta)
        {
            bool eixosSuspensosIda;
            bool eixosSuspensosVolta;
            bool eixosSuspensos;

            if (_integracaoRepom.ConsiderarRotaFreteDaCargaNoValePedagio)
            {

                eixosSuspensosIda = cargaVaziaIda && !pontoMaisDistante;
                eixosSuspensosVolta = cargaVaziaVolta && !pontoMaisDistante;
                eixosSuspensos = eixosSuspensosIda || eixosSuspensosVolta;
            }
            else
            {
                eixosSuspensos = !(pontoMaisDistante || cargaVaziaVolta);
                eixosSuspensosIda = cargaVaziaIda || cargaVaziaVolta;
                eixosSuspensosVolta = eixosSuspensos;
            }

            return (eixosSuspensosIda, eixosSuspensosVolta, eixosSuspensos);
        }


        #endregion
    }
}
