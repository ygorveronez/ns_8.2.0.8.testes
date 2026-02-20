using Dominio.ObjetosDeValor.Embarcador.Enumeradores;
using Infrastructure.Services.HttpClientFactory;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Security.Cryptography.X509Certificates;
using System.Security.Cryptography.Xml;
using System.Text;
using System.Threading;
using System.Xml;
using System.Xml.Serialization;
using NDDCargoEnumeradores = Dominio.ObjetosDeValor.Embarcador.Integracao.NDDCargo.Enumeradores;
using NDDCargoRequest = Dominio.ObjetosDeValor.Embarcador.Integracao.NDDCargo.Request;

namespace Servicos.Embarcador.Integracao.NDDCargo
{
    public class ValePedagio
    {
        #region Atributos Globais

        private readonly Repositorio.UnitOfWork _unitOfWork;
        private Dominio.Entidades.Embarcador.Cargas.TipoIntegracao _tipoIntegracao;
        private Dominio.Entidades.Embarcador.Configuracoes.IntegracaoNDDCargo _integracaoNDDCargo;

        #endregion

        #region Construtores

        public ValePedagio(Repositorio.UnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        #endregion

        #region Métodos para Cancelar Vale Pedágio

        public void CancelarValePedagio(Dominio.Entidades.Embarcador.Cargas.ValePedagio.CargaIntegracaoValePedagio cargaValePedagio, AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware tipoServicoMultisoftware)
        {
            Dominio.Entidades.Embarcador.Cargas.Carga carga = cargaValePedagio.Carga;
            Dominio.Entidades.Empresa transportador = carga.Empresa;

            LogCancelamento($"Cancelamento VP: {cargaValePedagio.Codigo} | Carga: {carga.Codigo} |GUID: {cargaValePedagio.CodigoIntegracaoValePedagio} | Numero: {cargaValePedagio.NumeroValePedagio}");

            try
            {
                ArquivoTransacao<Dominio.Entidades.Embarcador.Cargas.ValePedagio.CargaValePedagioIntegracaoArquivo> servicoArquivoTransacao = new ArquivoTransacao<Dominio.Entidades.Embarcador.Cargas.ValePedagio.CargaValePedagioIntegracaoArquivo>(_unitOfWork);

                Dominio.Entidades.Embarcador.Filiais.Filial empresa = carga.Filial;

                long numeroAutorizacao = 0, verificadorAutorizacao = 0;

                if (!string.IsNullOrEmpty(cargaValePedagio.NumeroValePedagio) && cargaValePedagio.NumeroValePedagio.IndexOf("-") >= 0)
                {
                    string[] informacoes = cargaValePedagio.NumeroValePedagio.Split('-');

                    if (informacoes.Length == 2)
                    {
                        long.TryParse(informacoes[0], out numeroAutorizacao);
                        long.TryParse(informacoes[1], out verificadorAutorizacao);
                    }
                }

                if (numeroAutorizacao == 0 || verificadorAutorizacao == 0)
                    throw new Dominio.Excecoes.Embarcador.CustomException($"Não foi possível recuperar os dados de Número e Verificador do VP.");

                LogCancelamento($"Montando objeto de cancelamentoVP");
                NDDCargoRequest.CancelarOperacaoValePedagioEnvio cancelamentoVP = new NDDCargoRequest.CancelarOperacaoValePedagioEnvio()
                {
                    InfCancelarOperacaoValePedagio = new NDDCargoRequest.InfCancelarOperacaoValePedagio()
                    {
                        CNPJ = empresa.CNPJ_SemFormato,
                        Autorizacao = new NDDCargoRequest.Autorizacao()
                        {
                            CNPJ = empresa.CNPJ_SemFormato,
                            Ndvp = new NDDCargoRequest.AutorizacaoNDVP()
                            {
                                Numero = numeroAutorizacao,
                                CodigoVerificador = verificadorAutorizacao
                            }
                        },
                        MotivoCancelamento = $"Cancelamento de vale pedágio da carga {carga.CodigoCargaEmbarcador}"
                    }
                };

                LogCancelamento($"Verificando certificado");
                Dominio.Entidades.Embarcador.Filiais.Filial empresaCertificado = empresa;
                if (string.IsNullOrEmpty(empresaCertificado.NomeCertificado))
                    throw new Dominio.Excecoes.Embarcador.CustomException($"Certificado não configurado para o transportador {empresaCertificado.CNPJ_Formatado}");

                LogCancelamento($"Verificando local do certificado");
                if (!Utilidades.IO.FileStorageService.Storage.Exists(empresaCertificado.NomeCertificado))
                    throw new Dominio.Excecoes.Embarcador.CustomException($"Certificado não localizado no servidor para o transportador {empresaCertificado.CNPJ_Formatado}");

                LogCancelamento($"Obtendo certificado");
                X509Certificate2 certificado = new System.Security.Cryptography.X509Certificates.X509Certificate2(empresaCertificado.NomeCertificado, empresaCertificado.SenhaCertificado, System.Security.Cryptography.X509Certificates.X509KeyStorageFlags.MachineKeySet);

                LogCancelamento($"Gerando GUID");
                NDDCargoRequest.CrossTalkHeader header = GerarGuid();
                header.ProcessCode = NDDCargoEnumeradores.ProcessCode.CancelarOperacaoValePedagio;

                LogCancelamento($"GUID gerado: {header.GUID}");

                cancelamentoVP.Versao = ObterVersao();
                cancelamentoVP.Token = header.Token;

                LogCancelamento($"Obtendo SOAP Message com assinatura");
                var soapEnvelopeMessage = ObterSoapEnvelopeMessage(header, cancelamentoVP, certificado, "infCancelarOperacaoValePedagio");

                LogCancelamento($"Executando cancelamento VP");
                var requisicao = ExecutarRequest<NDDCargoRequest.CrossTalkEnvelope<NDDCargoRequest.CrossTalkBodyResponse>>(soapEnvelopeMessage, header.GUID);
                servicoArquivoTransacao.Adicionar(cargaValePedagio, requisicao.XMLRequisicao, requisicao.XMLRetorno, "xml");
                LogCancelamento($"Cancelamento finalizado");

                var resultadoCancelamento = requisicao.Resposta;

                if (resultadoCancelamento != null && resultadoCancelamento.CrossTalkHeader != null)
                {
                    LogCancelamento($"Response Code: {resultadoCancelamento.CrossTalkHeader.ResponseCode}");

                    if (resultadoCancelamento.CrossTalkHeader.ResponseCode == (int)HttpStatusCode.Accepted)
                    {
                        cargaValePedagio.CodigoIntegracaoValePedagio = resultadoCancelamento.CrossTalkHeader.GUID;
                        NDDCargoRequest.CrossTalkEnvelope<NDDCargoRequest.CrossTalkBodyResponse> resultadoConsulta = ConsultarCancelamentoPedagio(cargaValePedagio);

                        if (resultadoConsulta == null)
                            throw new Dominio.Excecoes.Embarcador.CustomException($"{ObterIdentificadorGUID(header.GUID)}: Não foi possível consultar o status do cancelamento");

                        LogCancelamento($"Response Code Consulta Cancelamento: {resultadoConsulta.CrossTalkHeader.ResponseCode}");

                        if (resultadoConsulta.CrossTalkHeader.ResponseCode != (int)HttpStatusCode.OK)
                            throw new Dominio.Excecoes.Embarcador.CustomException($"{ObterIdentificadorGUID(header.GUID)}: {ObterMensagemFalha(resultadoConsulta)}");

                        cargaValePedagio.SituacaoValePedagio = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoValePedagio.Cancelada;
                        cargaValePedagio.ProblemaIntegracao = "Vale Pedágio Cancelado com Sucesso";
                    }
                    else
                    {
                        cargaValePedagio.SituacaoIntegracao = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoIntegracao.ProblemaIntegracao;
                        cargaValePedagio.ProblemaIntegracao = $"{ObterIdentificadorGUID(resultadoCancelamento.CrossTalkHeader.GUID)}: {ObterMensagemFalha(resultadoCancelamento)}";
                        cargaValePedagio.SituacaoValePedagio = SituacaoValePedagio.Comprada;
                    }
                }
                else
                {
                    throw new Dominio.Excecoes.Embarcador.CustomException($"Não foi possível obter o objeto de retorno da compra de VP");
                }
            }
            catch (Exception ex)
            {
                LogCancelamento($"Falha: {ex.StackTrace ?? ""} | {ex.Message ?? ""}");

                cargaValePedagio.SituacaoIntegracao = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoIntegracao.ProblemaIntegracao;
                cargaValePedagio.ProblemaIntegracao = $"Falha {ex.Message}";
                cargaValePedagio.SituacaoValePedagio = SituacaoValePedagio.Comprada;
            }
            finally
            {
                LogCancelamento($"Encerrando cancelamento de VP: {cargaValePedagio.Codigo}");

                cargaValePedagio.NumeroTentativas++;

                Repositorio.Embarcador.Cargas.ValePedagio.CargaIntegracaoValePedagio repositorioCargaValePedagio = new Repositorio.Embarcador.Cargas.ValePedagio.CargaIntegracaoValePedagio(_unitOfWork);
                repositorioCargaValePedagio.Atualizar(cargaValePedagio);

                LogCancelamento($"Cancelamento VP encerrado: {cargaValePedagio.Codigo}");
                LogCancelamento("".PadLeft(15, '-'));
            }
        }

        #endregion

        #region Métodos para Consultar Cancelamento de Vale Pedágio

        public NDDCargoRequest.CrossTalkEnvelope<NDDCargoRequest.CrossTalkBodyResponse> ConsultarCancelamentoPedagio(Dominio.Entidades.Embarcador.Cargas.ValePedagio.CargaIntegracaoValePedagio cargaValePedagio, int limiteTentativas = 5, int segundosEntreTentativas = 5)
        {
            Dominio.Entidades.Embarcador.Cargas.Carga carga = cargaValePedagio.Carga;

            LogCancelamento($"Consulta de cancelamento VP para a carga: {carga.CodigoCargaEmbarcador} | Integracao: {cargaValePedagio.Codigo}");
            LogCancelamento($"GUID: {ObterIdentificadorGUID(cargaValePedagio.CodigoIntegracaoValePedagio)}");

            try
            {
                ArquivoTransacao<Dominio.Entidades.Embarcador.Cargas.ValePedagio.CargaValePedagioIntegracaoArquivo> servicoArquivoTransacao = new ArquivoTransacao<Dominio.Entidades.Embarcador.Cargas.ValePedagio.CargaValePedagioIntegracaoArquivo>(_unitOfWork);

                NDDCargoRequest.CrossTalkHeader header = new NDDCargoRequest.CrossTalkHeader
                {
                    ProcessCode = NDDCargoEnumeradores.ProcessCode.CancelarOperacaoValePedagio,
                    MessageType = NDDCargoEnumeradores.MessageType.Insert,
                    ExchangePattern = NDDCargoEnumeradores.ExchangePattern.RespostaAssincrona,
                    DateTime = ObterDataAtual(),
                    EnterpriseId = ObterIntegracaoNDDCargo().EnterpriseId,
                    Token = ObterIntegracaoNDDCargo().Token,
                    GUID = cargaValePedagio.CodigoIntegracaoValePedagio
                };

                string messageSerialized = ObterCrossTalkMessageSerialized(header);
                string soapEnvelope = ObterSoapEnvelope(messageSerialized);

                (NDDCargoRequest.CrossTalkEnvelope<NDDCargoRequest.CrossTalkBodyResponse> Resposta, string XMLRequisicao, string XMLRetorno) requisicao = new();
                NDDCargoRequest.CrossTalkEnvelope<NDDCargoRequest.CrossTalkBodyResponse> resultado = null;

                int tentativasExecutadas = 0;
                while (tentativasExecutadas < limiteTentativas)
                {
                    tentativasExecutadas++;

                    if (tentativasExecutadas > 1)
                        Thread.Sleep(segundosEntreTentativas * 1000);

                    LogCancelamento($"Executando tentativa de consulta {tentativasExecutadas}");
                    requisicao = ExecutarRequest<NDDCargoRequest.CrossTalkEnvelope<NDDCargoRequest.CrossTalkBodyResponse>>(soapEnvelope, header.GUID);
                    resultado = requisicao.Resposta;

                    if (resultado != null && resultado.CrossTalkHeader != null)
                    {
                        LogCancelamento($"GUID: {cargaValePedagio.CodigoIntegracaoValePedagio} | Response Code: {resultado.CrossTalkHeader.ResponseCode} | Valor: {resultado.CrossTalkBody?.RetornoOperacaoValePedagio?.RetOperacaoValePedagio?.Pedagio?.Valor ?? 0}");
                        if (resultado.CrossTalkHeader.ResponseCode == (int)HttpStatusCode.Accepted)
                            continue;
                    }
                    else
                    {
                        throw new Dominio.Excecoes.Embarcador.CustomException($"Não foi possível obter o objeto de retorno da consulta");
                    }
                }

                LogCancelamento($"Consulta finalizada em {tentativasExecutadas} tentativa(s)");
                servicoArquivoTransacao.Adicionar(cargaValePedagio, requisicao.XMLRequisicao ?? "", requisicao.XMLRetorno ?? "", "xml");

                return resultado;
            }
            catch (Exception ex)
            {
                LogCancelamento($"Falha: {ex.StackTrace ?? ""} | {ex.Message ?? ""}");
                return null;
            }
            finally
            {
                LogCancelamento($"Encerrando consulta cancelamento VP para a carga: {carga.CodigoCargaEmbarcador}");

                LogCancelamento($"Consulta cancelamento VP encerrada para a carga: {carga.CodigoCargaEmbarcador}");
                LogCancelamento("".PadLeft(15, '-'));
            }
        }

        #endregion

        #region Métodos para Compra de Vale Pedágio

        public void ComprarValePedagio(Dominio.Entidades.Embarcador.Cargas.ValePedagio.CargaIntegracaoValePedagio cargaValePedagio, Dominio.Entidades.Embarcador.Cargas.Carga carga, AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware tipoServicoMultisoftware)
        {
            LogCompra($"Compra VP para a carga: {carga.CodigoCargaEmbarcador}");
            cargaValePedagio.DataIntegracao = DateTime.Now;

            try
            {
                ArquivoTransacao<Dominio.Entidades.Embarcador.Cargas.ValePedagio.CargaValePedagioIntegracaoArquivo> servicoArquivoTransacao = new ArquivoTransacao<Dominio.Entidades.Embarcador.Cargas.ValePedagio.CargaValePedagioIntegracaoArquivo>(_unitOfWork);

                Dominio.Entidades.Embarcador.Filiais.Filial empresa = carga.Filial;
                Dominio.Entidades.Empresa transportador = carga.Empresa;
                Dominio.Entidades.Veiculo veiculo = carga.Veiculo;

                LogCompra($"Montando objeto de operacaoVP");
                NDDCargoRequest.OperacaoValePedagioEnvio operacaoVP = new NDDCargoRequest.OperacaoValePedagioEnvio
                {
                    InfOperacaoValePedagio = new NDDCargoRequest.InfOperacaoValePedagio()
                    {
                        ImpAuto = 1,
                        TipoPagamento = 1,
                        Cnpj = empresa.CNPJ_SemFormato,
                        Ide = ObterIdeCompraValePedagio(empresa),
                        Transportador = ObterTransportadorCompraValePedagio(transportador),
                        CondutorFavorecido = new NDDCargoRequest.CondutorFavorecido()
                        {
                            Cpf = carga.CPFPrimeiroMotorista
                        },
                        InfRota = new NDDCargoRequest.InfRota()
                        {
                            CategoriaPedagio = ObterCategoriaCompraValePedagio(carga).Categoria,
                            Rota = new NDDCargoRequest.Rota()
                            {
                                RotaERP = $"Carga {carga.CodigoCargaEmbarcador}",
                                Informacoes = new NDDCargoRequest.InformacoesRota()
                                {
                                    Nome = $"Informações Rota da Carga {carga.CodigoCargaEmbarcador}",
                                    PontosParada = new NDDCargoRequest.PontosParada()
                                    {
                                        PontoParada = ObterPontosParadaCompraValePedagio(carga)
                                    }
                                }
                            }
                        },
                        Veiculo = new NDDCargoRequest.Veiculo()
                        {
                            Placa = veiculo.Placa
                        },
                        InformacoesTag = new NDDCargoRequest.InformacoesTag()
                        {
                            CodigoFornecedor = ObterFornecedorTagCompraValePedagio(veiculo)
                        }
                    }
                };

                LogCompra($"Verificando certificado");
                Dominio.Entidades.Embarcador.Filiais.Filial empresaCertificado = empresa;
                if (string.IsNullOrEmpty(empresaCertificado.NomeCertificado))
                    throw new Dominio.Excecoes.Embarcador.CustomException($"Certificado não configurado para o transportador {empresaCertificado.CNPJ_Formatado}");

                LogCompra($"Verificando local do certificado");
                if (!Utilidades.IO.FileStorageService.Storage.Exists(empresaCertificado.NomeCertificado))
                    throw new Dominio.Excecoes.Embarcador.CustomException($"Certificado não localizado no servidor para o transportador {empresaCertificado.CNPJ_Formatado}");

                LogCompra($"Obtendo certificado");
                X509Certificate2 certificado = new System.Security.Cryptography.X509Certificates.X509Certificate2(empresaCertificado.NomeCertificado, empresaCertificado.SenhaCertificado, System.Security.Cryptography.X509Certificates.X509KeyStorageFlags.MachineKeySet);

                LogCompra($"Gerando GUID");
                NDDCargoRequest.CrossTalkHeader header = GerarGuid();
                header.ProcessCode = NDDCargoEnumeradores.ProcessCode.OperacaoValePedagio;

                LogCompra($"GUID gerado: {header.GUID}");

                operacaoVP.Versao = ObterVersao();
                operacaoVP.Token = header.Token;
                operacaoVP.InfOperacaoValePedagio.ID = "NDD_" + header.GUID;

                LogCompra($"Obtendo SOAP Message com assinatura");
                var soapEnvelopeMessage = ObterSoapEnvelopeMessage(header, operacaoVP, certificado, "infOperacaoValePedagio");

                LogCompra($"Executando compra VP");
                var requisicao = ExecutarRequest<NDDCargoRequest.CrossTalkEnvelope<NDDCargoRequest.CrossTalkBodyResponse>>(soapEnvelopeMessage, header.GUID);
                servicoArquivoTransacao.Adicionar(cargaValePedagio, requisicao.XMLRequisicao, requisicao.XMLRetorno, "xml");
                LogCompra($"Compra finalizada");

                var resultado = requisicao.Resposta;

                if (resultado != null && resultado.CrossTalkHeader != null)
                {
                    LogCompra($"Response Code: {resultado.CrossTalkHeader.ResponseCode}");

                    if (resultado.CrossTalkHeader.ResponseCode == (int)HttpStatusCode.Accepted)
                    {
                        InserirConsultaValorPedagio(resultado.CrossTalkHeader.GUID, carga);
                        cargaValePedagio.SituacaoIntegracao = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoIntegracao.AgRetorno;
                        cargaValePedagio.CodigoIntegracaoValePedagio = resultado.CrossTalkHeader.GUID;
                    }
                    else
                    {
                        cargaValePedagio.SituacaoIntegracao = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoIntegracao.ProblemaIntegracao;
                        cargaValePedagio.ProblemaIntegracao = $"{ObterIdentificadorGUID(resultado.CrossTalkHeader.GUID)}: {ObterMensagemFalha(resultado)}";
                    }
                }
                else
                {
                    throw new Dominio.Excecoes.Embarcador.CustomException($"Não foi possível obter o objeto de retorno da compra de VP");
                }
            }
            catch (Exception ex)
            {
                LogCompra($"Falha: {ex.StackTrace ?? ""} | {ex.Message ?? ""}");

                cargaValePedagio.SituacaoIntegracao = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoIntegracao.ProblemaIntegracao;
                cargaValePedagio.ProblemaIntegracao = $"Falha {ex.Message}";
            }
            finally
            {
                LogCompra($"Encerrando compra de VP para a carga: {carga.CodigoCargaEmbarcador}");

                cargaValePedagio.NumeroTentativas++;

                Repositorio.Embarcador.Cargas.ValePedagio.CargaIntegracaoValePedagio repositorioCargaValePedagio = new Repositorio.Embarcador.Cargas.ValePedagio.CargaIntegracaoValePedagio(_unitOfWork);
                repositorioCargaValePedagio.Atualizar(cargaValePedagio);

                LogCompra($"Compra VP encerrada para a carga: {carga.CodigoCargaEmbarcador}");
                LogCompra("".PadLeft(15, '-'));
            }
        }

        private void InserirConsultaValorPedagio(string guid, Dominio.Entidades.Embarcador.Cargas.Carga carga)
        {
            Repositorio.Embarcador.Cargas.CargaPedido repCargaPedido = new Repositorio.Embarcador.Cargas.CargaPedido(_unitOfWork);
            Repositorio.Embarcador.Cargas.Carga repCarga = new Repositorio.Embarcador.Cargas.Carga(_unitOfWork);
            Repositorio.Embarcador.Cargas.ValePedagio.CargaConsultaValorPedagioIntegracao repositorioCargaConsultaValorPedagio = new Repositorio.Embarcador.Cargas.ValePedagio.CargaConsultaValorPedagioIntegracao(_unitOfWork);

            repCargaPedido.ZerarValorValePedagioPorCarga(carga.Codigo);
            repositorioCargaConsultaValorPedagio.RemoverIntegracaoPorCarga(carga.Codigo);

            Dominio.Entidades.Embarcador.Cargas.ValePedagio.CargaConsultaValorPedagioIntegracao cargaConsultaValoresPedagio = new Dominio.Entidades.Embarcador.Cargas.ValePedagio.CargaConsultaValorPedagioIntegracao();
            cargaConsultaValoresPedagio.Carga = carga;
            cargaConsultaValoresPedagio.TipoIntegracao = ObterTipoIntegracaoNDDCargo();
            cargaConsultaValoresPedagio.ProblemaIntegracao = "";
            cargaConsultaValoresPedagio.DataIntegracao = DateTime.Now;
            cargaConsultaValoresPedagio.SituacaoIntegracao = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoIntegracao.AgIntegracao;
            cargaConsultaValoresPedagio.QuantidadeEixos = ObterCategoriaCompraValePedagio(carga).NumeroEixos;
            cargaConsultaValoresPedagio.CodigoIntegracaoValePedagio = guid;
            repositorioCargaConsultaValorPedagio.Inserir(cargaConsultaValoresPedagio);

            carga.IntegrandoValePedagio = true;
            repCarga.Atualizar(carga);
        }

        private static NDDCargoEnumeradores.FornecedorTag ObterFornecedorTagCompraValePedagio(Dominio.Entidades.Veiculo veiculo)
        {
            if (veiculo.ModoCompraValePedagioTarget == Dominio.ObjetosDeValor.Embarcador.Enumeradores.ModoCompraValePedagioTarget.PedagioTagVeloe)
                return NDDCargoEnumeradores.FornecedorTag.Veloe;

            if (veiculo.ModoCompraValePedagioTarget == Dominio.ObjetosDeValor.Embarcador.Enumeradores.ModoCompraValePedagioTarget.PedagioConectCar)
                return NDDCargoEnumeradores.FornecedorTag.ConectCar;

            return NDDCargoEnumeradores.FornecedorTag.SemParar;
        }

        private List<NDDCargoRequest.PontoParada> ObterPontosParadaCompraValePedagio(Dominio.Entidades.Embarcador.Cargas.Carga carga)
        {
            Repositorio.Embarcador.Cargas.CargaRotaFrete _repositorioCargaRotaFrete = new Repositorio.Embarcador.Cargas.CargaRotaFrete(_unitOfWork);
            Repositorio.Embarcador.Cargas.CargaRotaFretePontosPassagem _repositorioCargaRotaFretePontosPassagem = new Repositorio.Embarcador.Cargas.CargaRotaFretePontosPassagem(_unitOfWork);

            Dominio.Entidades.Embarcador.Cargas.CargaRotaFrete cargaRotaFrete = _repositorioCargaRotaFrete.BuscarPorCarga(carga.Codigo);
            List<Dominio.Entidades.Embarcador.Cargas.CargaRotaFretePontosPassagem> pontosRota = null;

            if (cargaRotaFrete != null)
                pontosRota = _repositorioCargaRotaFretePontosPassagem.BuscarPorCargaRotaFrete(cargaRotaFrete.Codigo);

            if (pontosRota == null || pontosRota.Count == 0)
                throw new Dominio.Excecoes.Embarcador.CustomException("Não foi possível definir os pontos da rota.");

            Dominio.Entidades.Localidade localidadeOrigem = ObterLocalidade(pontosRota.FirstOrDefault());
            Dominio.Entidades.Localidade localidadeDestino = ObterLocalidade(pontosRota.LastOrDefault());

            List<NDDCargoRequest.PontoParada> pontosParada = new List<NDDCargoRequest.PontoParada>();

            if (localidadeOrigem != null)
                pontosParada.Add(ObterPontoParada(localidadeOrigem));

            if (localidadeDestino != null)
                pontosParada.Add(ObterPontoParada(localidadeDestino));

            return pontosParada;
        }

        private static NDDCargoRequest.PontoParada ObterPontoParada(Dominio.Entidades.Localidade localidade)
        {
            return new NDDCargoRequest.PontoParada()
            {
                CodigoIBGE = localidade.CodigoIBGE
            };
        }

        private static Dominio.Entidades.Localidade ObterLocalidade(Dominio.Entidades.Embarcador.Cargas.CargaRotaFretePontosPassagem pontoRota)
        {
            if (pontoRota == null)
                return null;

            if (pontoRota.ClienteOutroEndereco != null)
                return pontoRota.ClienteOutroEndereco.Localidade;

            if (pontoRota.Cliente != null)
                return pontoRota.Cliente.Localidade;

            return null;
        }

        private static (int NumeroEixos, NDDCargoEnumeradores.CategoriaPedagio Categoria) ObterCategoriaCompraValePedagio(Dominio.Entidades.Embarcador.Cargas.Carga carga)
        {
            bool eixosSimples = carga.Veiculo.ModeloVeicularCarga?.PadraoEixos == Dominio.ObjetosDeValor.Embarcador.Enumeradores.PadraoEixosVeiculo.Simples;
            int numeroEixos = carga.Veiculo.ModeloVeicularCarga.NumeroEixos ?? 0;

            if (carga.VeiculosVinculados != null && carga.Veiculo.ModeloVeicularCarga != null)
            {
                foreach (Dominio.Entidades.Veiculo reboque in carga.VeiculosVinculados.ToList())
                {
                    if (reboque.ModeloVeicularCarga != null && reboque.ModeloVeicularCarga != carga.Veiculo.ModeloVeicularCarga)
                        numeroEixos += reboque.ModeloVeicularCarga.NumeroEixos ?? 0;
                }
            }

            return (numeroEixos,
                numeroEixos switch
                {
                    0 => NDDCargoEnumeradores.CategoriaPedagio.Isento,
                    1 => NDDCargoEnumeradores.CategoriaPedagio.MotocicletasMotonetasEBicicletasAMotor,
                    2 => eixosSimples ? NDDCargoEnumeradores.CategoriaPedagio.AutomovelCaminhonetaEFurgaoDoisEixosSimples : NDDCargoEnumeradores.CategoriaPedagio.CaminhaoLeveFurgaoECavaloMecanicoDoisEixosDuplos,
                    3 => eixosSimples ? NDDCargoEnumeradores.CategoriaPedagio.AutomovelCaminhonetaComSemirreboqueTresEixosSimples : NDDCargoEnumeradores.CategoriaPedagio.CaminhaoCaminhaoTratorECavaloMecanicoComSemirreboqueTresEixosDuplos,
                    4 => eixosSimples ? NDDCargoEnumeradores.CategoriaPedagio.AutomovelCaminhonetaComReboqueQuatroEixosSimples : NDDCargoEnumeradores.CategoriaPedagio.CaminhaoComReboqueECavaloMecanicoComSemirreboqueQuatroEixosDuplos,
                    5 => NDDCargoEnumeradores.CategoriaPedagio.CaminhaoComReboqueECavaloMecanicoComSemirreboqueCincoEixosDuplos,
                    6 => NDDCargoEnumeradores.CategoriaPedagio.CaminhaoComReboqueECavaloMecanicoComSemirreboqueSeisEixosDuplos,
                    7 => NDDCargoEnumeradores.CategoriaPedagio.CaminhaoComReboqueECavaloMecanicoComSemirreboqueSeteEixosDuplos,
                    8 => NDDCargoEnumeradores.CategoriaPedagio.CaminhaoComReboqueECavaloMecanicoComSemirreboqueOitoEixosDuplos,
                    9 => NDDCargoEnumeradores.CategoriaPedagio.CaminhaoComReboqueECavaloMecanicoComSemirreboqueNoveEixosDuplos,
                    10 => NDDCargoEnumeradores.CategoriaPedagio.CaminhaoComReboqueECavaloMecanicoComSemirreboqueDezEixos,
                    _ => NDDCargoEnumeradores.CategoriaPedagio.CaminhaoComReboqueECavaloMecanicoComSemirreboqueAcimaDeDezEixos,
                });
        }

        private static NDDCargoRequest.Transportador ObterTransportadorCompraValePedagio(Dominio.Entidades.Empresa transportador)
        {
            bool transportadorPJ = transportador.CNPJ_SemFormato.Length == 14;

            return new NDDCargoRequest.Transportador()
            {
                Rntrc = (transportador?.RegistroANTT ?? "").PadLeft(9, '0'),
                CnpjTransportador = transportadorPJ ? (transportador.CNPJ_SemFormato ?? "") : null,
                CpfTransportador = !transportadorPJ ? (transportador.CNPJ_SemFormato ?? "") : null,
            };
        }

        private static NDDCargoRequest.Ide ObterIdeCompraValePedagio(Dominio.Entidades.Embarcador.Filiais.Filial empresa)
        {
            Thread.Sleep(1000);

            DateTime dataAtual = DateTime.Now;
            string identificadorHora = (dataAtual.Hour * dataAtual.Minute * dataAtual.Second).ToString().PadLeft(5, '0');

            string numeroIde = identificadorHora + dataAtual.Day + dataAtual.Month;
            string serieIde = DateTime.Now.Year.ToString();

            return new NDDCargoRequest.Ide()
            {
                Cnpj = empresa.CNPJ_SemFormato ?? "",
                Numero = Convert.ToInt32(numeroIde),
                Serie = serieIde,
                PtEmissor = "Matriz"
            };
        }

        #endregion

        #region Métodos para Consultar Compra de Vale Pedágio

        public decimal ConsultarValorPedagio(Dominio.Entidades.Embarcador.Cargas.ValePedagio.CargaConsultaValorPedagioIntegracao cargaConsultaValePedagio, Dominio.Entidades.Embarcador.Cargas.Carga carga)
        {
            LogConsulta($"Consulta VP para a carga: {carga.CodigoCargaEmbarcador} | Integracao: {cargaConsultaValePedagio.Codigo}");
            LogConsulta($"GUID: {ObterIdentificadorGUID(cargaConsultaValePedagio.CodigoIntegracaoValePedagio)}");

            try
            {
                ArquivoTransacao<Dominio.Entidades.Embarcador.Cargas.ValePedagio.CargaValePedagioIntegracaoArquivo> servicoArquivoTransacao = new ArquivoTransacao<Dominio.Entidades.Embarcador.Cargas.ValePedagio.CargaValePedagioIntegracaoArquivo>(_unitOfWork);

                NDDCargoRequest.CrossTalkHeader header = new NDDCargoRequest.CrossTalkHeader
                {
                    ProcessCode = NDDCargoEnumeradores.ProcessCode.OperacaoValePedagio,
                    MessageType = NDDCargoEnumeradores.MessageType.Insert,
                    ExchangePattern = NDDCargoEnumeradores.ExchangePattern.RespostaAssincrona,
                    DateTime = ObterDataAtual(),
                    EnterpriseId = ObterIntegracaoNDDCargo().EnterpriseId,
                    Token = ObterIntegracaoNDDCargo().Token,
                    GUID = cargaConsultaValePedagio.CodigoIntegracaoValePedagio
                };

                string messageSerialized = ObterCrossTalkMessageSerialized(header);
                string soapEnvelope = ObterSoapEnvelope(messageSerialized);

                LogConsulta($"Executando consulta");
                var requisicao = ExecutarRequest<NDDCargoRequest.CrossTalkEnvelope<NDDCargoRequest.CrossTalkBodyResponse>>(soapEnvelope, header.GUID);
                servicoArquivoTransacao.Adicionar(cargaConsultaValePedagio, requisicao.XMLRequisicao, requisicao.XMLRetorno, "xml");
                LogConsulta($"Consulta finalizada");

                var resultado = requisicao.Resposta;

                if (resultado != null && resultado.CrossTalkHeader != null)
                {
                    LogConsulta($"GUID: {cargaConsultaValePedagio.CodigoIntegracaoValePedagio} | Response Code: {resultado.CrossTalkHeader.ResponseCode} | Valor: {resultado.CrossTalkBody?.RetornoOperacaoValePedagio?.RetOperacaoValePedagio?.Pedagio?.Valor ?? 0}");
                    if (resultado.CrossTalkHeader.ResponseCode == (int)HttpStatusCode.Accepted)
                    {
                        cargaConsultaValePedagio.SituacaoIntegracao = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoIntegracao.AgIntegracao;
                        cargaConsultaValePedagio.DataIntegracao.AddSeconds(10);
                    }
                    else
                    {
                        Repositorio.Embarcador.Cargas.ValePedagio.CargaIntegracaoValePedagio repositorioCargaIntegracaoValePedagio = new Repositorio.Embarcador.Cargas.ValePedagio.CargaIntegracaoValePedagio(_unitOfWork);
                        Dominio.Entidades.Embarcador.Cargas.ValePedagio.CargaIntegracaoValePedagio cargaIntegracaoValePedagio = repositorioCargaIntegracaoValePedagio.BuscarPorCargaECodigoIntegracao(carga.Codigo, cargaConsultaValePedagio.CodigoIntegracaoValePedagio);

                        servicoArquivoTransacao.Adicionar(cargaIntegracaoValePedagio, requisicao.XMLRequisicao, requisicao.XMLRetorno, "xml");

                        if (resultado.CrossTalkHeader.ResponseCode == (int)HttpStatusCode.OK)
                        {
                            cargaIntegracaoValePedagio.DataIntegracao = DateTime.Now;

                            cargaConsultaValePedagio.DataIntegracao = DateTime.Now;
                            cargaConsultaValePedagio.SituacaoIntegracao = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoIntegracao.Integrado;

                            NDDCargoRequest.RetOperacaoValePedagio retornoOperacao = resultado.CrossTalkBody?.RetornoOperacaoValePedagio?.RetOperacaoValePedagio;

                            if (retornoOperacao != null)
                            {
                                string numeroAutorizacao = retornoOperacao.AutorizacaoNDVP?.Numero.ToString() ?? "";
                                string verificadorAutorizacao = retornoOperacao.AutorizacaoNDVP?.CodigoVerificador.ToString() ?? "";
                                string numeroVP = $"{numeroAutorizacao}-{verificadorAutorizacao}";
                                decimal valorVP = retornoOperacao.Pedagio?.Valor ?? 0;

                                cargaIntegracaoValePedagio.NumeroValePedagio = numeroVP;
                                cargaIntegracaoValePedagio.ValorValePedagio = valorVP;
                                cargaIntegracaoValePedagio.SituacaoIntegracao = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoIntegracao.Integrado;

                                if (valorVP == 0)
                                    cargaIntegracaoValePedagio.SituacaoValePedagio = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoValePedagio.RotaSemCusto;
                                else
                                    cargaIntegracaoValePedagio.SituacaoValePedagio = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoValePedagio.Comprada;

                                repositorioCargaIntegracaoValePedagio.Atualizar(cargaIntegracaoValePedagio);

                                cargaConsultaValePedagio.ValorValePedagio = valorVP;
                            }
                        }
                        else
                        {
                            cargaConsultaValePedagio.SituacaoIntegracao = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoIntegracao.ProblemaIntegracao;
                            cargaConsultaValePedagio.ProblemaIntegracao = $"{ObterIdentificadorGUID(header.GUID)}: {ObterMensagemFalha(resultado)}";

                            cargaIntegracaoValePedagio.SituacaoIntegracao = cargaConsultaValePedagio.SituacaoIntegracao;
                            cargaIntegracaoValePedagio.ProblemaIntegracao = cargaConsultaValePedagio.ProblemaIntegracao;
                            repositorioCargaIntegracaoValePedagio.Atualizar(cargaIntegracaoValePedagio);
                        }
                    }
                }
                else
                {
                    throw new Dominio.Excecoes.Embarcador.CustomException($"Não foi possível obter o objeto de retorno da consulta");
                }

                return cargaConsultaValePedagio.ValorValePedagio;
            }
            catch (Exception ex)
            {
                LogConsulta($"Falha: {ex.StackTrace ?? ""} | {ex.Message ?? ""}");

                cargaConsultaValePedagio.SituacaoIntegracao = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoIntegracao.ProblemaIntegracao;
                cargaConsultaValePedagio.ProblemaIntegracao = $"Falha {ex.Message}";
                return 0;
            }
            finally
            {
                LogConsulta($"Encerrando consulta VP para a carga: {carga.CodigoCargaEmbarcador}");

                cargaConsultaValePedagio.NumeroTentativas++;

                LogConsulta($"Consulta VP encerrada para a carga: {carga.CodigoCargaEmbarcador}");
                LogConsulta("".PadLeft(15, '-'));
            }
        }

        #endregion

        #region Métodos para Gerar GUID

        private NDDCargoRequest.CrossTalkHeader GerarGuid()
        {
            NDDCargoRequest.CrossTalkHeader header = new NDDCargoRequest.CrossTalkHeader
            {
                ProcessCode = NDDCargoEnumeradores.ProcessCode.GerarGUID,
                MessageType = NDDCargoEnumeradores.MessageType.Insert,
                ExchangePattern = NDDCargoEnumeradores.ExchangePattern.RequisicaoAssincrona,
                DateTime = ObterDataAtual(),
                EnterpriseId = ObterIntegracaoNDDCargo().EnterpriseId,
                Token = ObterIntegracaoNDDCargo().Token
            };

            string messageSerialized = ObterCrossTalkMessageSerialized(header);
            string soapEnvelope = ObterSoapEnvelope(messageSerialized);

            return ExecutarRequest<NDDCargoRequest.CrossTalkHeader>(soapEnvelope, "GerarGUID").Resposta;
        }

        #endregion

        #region Métodos Comuns

        private string ObterIdentificadorGUID(string codigoIntegracaoValePedagio)
        {
            return $"GUID {codigoIntegracaoValePedagio ?? ""}";
        }

        private static void LogCompra(string mensagem)
        {
            Servicos.Log.TratarErro(mensagem, "IntegracaoNDDCargoCompraValePedagio");
        }

        private static void LogConsulta(string mensagem)
        {
            Servicos.Log.TratarErro(mensagem, "IntegracaoNDDConsultaCompraValePedagio");
        }

        private static void LogCancelamento(string mensagem)
        {
            Servicos.Log.TratarErro(mensagem, "IntegracaoNDDCancelamentoCompraValePedagio");
        }

        private static void LogDownload(string mensagem)
        {
            Servicos.Log.TratarErro(mensagem, "IntegracaoNDDDownloadOperacaoValePedagio");
        }

        private static void LogRequest(string mensagem, string guid)
        {
            mensagem = guid + " - " + mensagem;
            Servicos.Log.TratarErro(mensagem, "IntegracaoNDDRequestCompraValePedagio");
        }

        private (T Resposta, string XMLRequisicao, string XMLRetorno) ExecutarRequest<T>(string soapEnvelope, string guid)
        {
            HttpClient httpClient = HttpClientFactoryWrapper.GetClient(nameof(ValePedagio));

            StringContent content = new StringContent(soapEnvelope, Encoding.UTF8, "application/soap+xml");
            content.Headers.Add("SOAPAction", "http://tempuri.org/Send");

            string url = ObterIntegracaoNDDCargo().URL;
            LogRequest($"Executando Request na URL {url}", guid);
            HttpResponseMessage response = httpClient.PostAsync(url, content).Result;

            LogRequest($"Response Request na URL {url}: {(response.IsSuccessStatusCode ? "Sucesso" : "Falha")}", guid);
            if (response.IsSuccessStatusCode)
            {
                LogRequest($"Extraindo resposta", guid);
                string responseResult = response.Content.ReadAsStringAsync().Result;
                LogRequest($"Decodificando resposta", guid);
                string decodedResult = RemoverDeclaracaoXML(System.Net.WebUtility.HtmlDecode(responseResult));
                LogRequest($"Extraindo cdata da resposta", guid);
                string responseXml = ExtrairCData(decodedResult);

                if (typeof(T).IsGenericType
                    && typeof(T).GetGenericTypeDefinition() == typeof(NDDCargoRequest.CrossTalkEnvelope<>).GetGenericTypeDefinition())
                {
                    LogRequest($"Envelopando resposta", guid);
                    responseXml = "<Envelope>" + responseXml + "</Envelope>";
                }

                LogRequest($"Serializando objeto da resposta", guid);
                XmlSerializer serializer = new XmlSerializer(typeof(T));

                using (StringReader reader = new StringReader(responseXml))
                    return ((T)serializer.Deserialize(reader), soapEnvelope, responseXml);
            }
            else
            {
                string statusCode = response.StatusCode.ToString();
                string reasonPhrase = response.ReasonPhrase;
                string errorContent = response.Content.ReadAsStringAsync().Result;

                LogRequest($"Falha ao obter resposta", guid);
                LogRequest($"Conteudo da falha: {statusCode} - {reasonPhrase} - {errorContent}", guid);
                throw new Dominio.Excecoes.Embarcador.CustomException($"Falha na requisição: {statusCode} - {reasonPhrase}: {errorContent}");
            }
        }

        private static string ObterMensagemFalha(NDDCargoRequest.CrossTalkEnvelope<NDDCargoRequest.CrossTalkBodyResponse> resultado)
        {
            string mensagemFalha = "";
            List<NDDCargoRequest.Mensagem> mensagens = resultado.CrossTalkBody?.Body?.Mensagens?.ListaMensagens
                ?? resultado.CrossTalkBody?.RetornoOperacaoValePedagio?.Mensagens?.ListaMensagens
                ?? resultado.CrossTalkBody?.RetornoCancelarOperacaoValePedagio?.Mensagens?.ListaMensagens;

            if (mensagens != null)
                mensagemFalha = string.Join(", ", mensagens.Select(mensagem => $"{mensagem.Codigo} - {mensagem.TextoMensagem}"));

            if (string.IsNullOrEmpty(mensagemFalha))
                mensagemFalha = $"{resultado.CrossTalkHeader.ResponseCode} - {resultado.CrossTalkHeader.ResponseCodeMessage}";

            return mensagemFalha;
        }

        private static string AssinarXML(string xmlString, string elementoAssinatura, X509Certificate2 certificado)
        {
            var xmlDoc = new XmlDocument();
            xmlDoc.PreserveWhitespace = true;
            xmlDoc.LoadXml(xmlString);

            var elementoParaAssinar = xmlDoc.GetElementsByTagName(elementoAssinatura)[0];

            var signedXml = new SignedXml(xmlDoc);

            signedXml.SigningKey = certificado.PrivateKey;
            signedXml.SignedInfo.SignatureMethod = SignedXml.XmlDsigRSASHA1Url;

            var reference = new Reference();
            if (elementoParaAssinar.Attributes["ID"] != null)
                reference.Uri = "#" + elementoParaAssinar.Attributes["ID"].Value;
            else
                reference.Uri = "";

            var env = new XmlDsigEnvelopedSignatureTransform();
            var c14n = new XmlDsigC14NTransform();
            reference.AddTransform(env);
            reference.AddTransform(c14n);
            reference.DigestMethod = SignedXml.XmlDsigSHA1Url;

            signedXml.AddReference(reference);

            var keyInfo = new KeyInfo();
            keyInfo.AddClause(new KeyInfoX509Data(certificado));
            signedXml.KeyInfo = keyInfo;

            signedXml.ComputeSignature();

            var xmlDigitalSignature = signedXml.GetXml();

            elementoParaAssinar.ParentNode.AppendChild(xmlDoc.ImportNode(xmlDigitalSignature, true));
            return xmlDoc.OuterXml;
        }

        private string ObterSoapEnvelopeMessage<T>(NDDCargoRequest.CrossTalkHeader header, T objetoEnviar, X509Certificate2 certificado = null, string elementoAssinatura = null)
        {
            string messageSerialized = ObterCrossTalkMessageSerialized(header);

            XmlSerializer xmlSerializer = new XmlSerializer(typeof(T));
            string rawDataSerialized = "";

            using (StringWriter stringWriter = new StringWriter())
            using (XmlWriter xmlWriter = XmlWriter.Create(stringWriter, new XmlWriterSettings { Indent = true, Encoding = Encoding.UTF8, OmitXmlDeclaration = false }))
            {
                xmlSerializer.Serialize(xmlWriter, objetoEnviar);
                rawDataSerialized = stringWriter.ToString();
            }

            if (certificado != null && !string.IsNullOrEmpty(elementoAssinatura))
                rawDataSerialized = AssinarXML(rawDataSerialized, elementoAssinatura, certificado);

            return ObterSoapEnvelope(messageSerialized, rawDataSerialized);
        }

        private string ObterCrossTalkMessageSerialized<T>(T header)
        {
            NDDCargoRequest.CrossTalkMessage<T> objeto = new NDDCargoRequest.CrossTalkMessage<T>
            {
                Header = header,
                Body = new NDDCargoRequest.CrossTalkBody
                {
                    VersionBody = new NDDCargoRequest.CrossTalkVersionBody
                    {
                        Versao = ObterVersao()
                    }
                }
            };

            XmlSerializer xmlSerializer = new XmlSerializer(typeof(NDDCargoRequest.CrossTalkMessage<T>));
            string serializedXml = "";

            XmlWriterSettings xmlWriterSettings = new XmlWriterSettings
            {
                Indent = true,
                Encoding = Encoding.UTF8,
                OmitXmlDeclaration = false
            };

            using (StringWriter stringWriter = new StringWriter())
            using (XmlWriter xmlWriter = XmlWriter.Create(stringWriter, xmlWriterSettings))
            {
                xmlSerializer.Serialize(xmlWriter, objeto);
                serializedXml = stringWriter.ToString();
            }

            return serializedXml;
        }

        private static DateTime ObterDataAtual()
        {
            string formatoData = "yyyy-MM-ddTHH:mm:ss.fffffffK";

            return DateTime.Parse(DateTime.Now.ToString(formatoData));
        }

        private static string ObterSoapEnvelope(string message, string rawData = null)
        {
            if (message == null)
                message = "";

            if (rawData == null)
                rawData = "";

            return $@"<soap:Envelope xmlns:soap=""http://www.w3.org/2003/05/soap-envelope"" xmlns:tem=""http://tempuri.org/"">
                        <soap:Header/>
                        <soap:Body>
                            <tem:Send>
                                <tem:message><![CDATA[{message}]]></tem:message>
                                <tem:rawData><![CDATA[{rawData}]]></tem:rawData>
                            </tem:Send>
                        </soap:Body>
                      </soap:Envelope>";
        }

        private static string ExtrairCData(string xml)
        {
            XmlDocument doc = new XmlDocument();
            doc.LoadXml(xml);

            XmlNamespaceManager nsmgr = new XmlNamespaceManager(doc.NameTable);
            nsmgr.AddNamespace("soap", "http://www.w3.org/2003/05/soap-envelope");
            nsmgr.AddNamespace("ns", "http://tempuri.org/");
            nsmgr.AddNamespace("xsd", "http://www.w3.org/2001/XMLSchema");
            nsmgr.AddNamespace("xsi", "http://www.w3.org/2001/XMLSchema-instance");

            XmlNode node = doc.SelectSingleNode("//soap:Envelope/soap:Body/ns:SendResponse/ns:SendResult/ns:CrossTalk_Message", nsmgr);

            return node.InnerXml
                .Replace(" xmlns=\"http://tempuri.org/\"", "")
                .Replace(" xmlns=\"http://www.nddigital.com.br/nddcargo\"", "");
        }

        private static string RemoverDeclaracaoXML(string xml)
        {
            string xmlDeclaration = "<?xml version=\"1.0\" encoding=\"utf-8\"?>";

            while (xml.Contains(xmlDeclaration))
            {
                int index = xml.IndexOf(xmlDeclaration);
                if (index >= 0)
                    xml = xml.Remove(index, xmlDeclaration.Length).Trim();
            }

            return xml;
        }

        #endregion

        #region Métodos Privados

        private Dominio.Entidades.Embarcador.Cargas.TipoIntegracao ObterTipoIntegracaoNDDCargo()
        {
            if (_tipoIntegracao == null)
            {
                Repositorio.Embarcador.Cargas.TipoIntegracao repositorioTipoIntegracao = new Repositorio.Embarcador.Cargas.TipoIntegracao(_unitOfWork);
                _tipoIntegracao = repositorioTipoIntegracao.BuscarPorTipo(Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoIntegracao.NDDCargo);
            }

            return _tipoIntegracao;
        }

        private Dominio.Entidades.Embarcador.Configuracoes.IntegracaoNDDCargo ObterIntegracaoNDDCargo()
        {
            if (_integracaoNDDCargo == null)
                _integracaoNDDCargo = new Repositorio.Embarcador.Configuracoes.IntegracaoNDDCargo(_unitOfWork).BuscarPrimeiroRegistro();

            return _integracaoNDDCargo;
        }

        private string ObterVersao()
        {
            return ObterIntegracaoNDDCargo().Versao;
        }

        #endregion
    }
}
