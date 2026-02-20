using Dominio.Excecoes.Embarcador;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using Dominio.ObjetosDeValor.Embarcador.CIOT.RepomFrete;
using System.Text.RegularExpressions;
using Utilidades.Extensions;
using Newtonsoft.Json.Linq;

namespace Servicos.Embarcador.CIOT.RepomFrete
{
    public partial class IntegracaoRepomFrete
    {
        #region Métodos Globais

        public bool EncerrarCIOT(Dominio.Entidades.Embarcador.Documentos.CIOT ciot, out string mensagemErro, Repositorio.UnitOfWork unitOfWork)
        {
            if (ciot.CIOTPorPeriodo)
                return EncerrarCIOTAgregado(ciot, out mensagemErro, unitOfWork);

            Repositorio.Embarcador.Documentos.CIOTIntegracaoArquivo repCIOTIntegracaoArquivo = new Repositorio.Embarcador.Documentos.CIOTIntegracaoArquivo(unitOfWork);
            Repositorio.Embarcador.Documentos.CIOT repCIOT = new Repositorio.Embarcador.Documentos.CIOT(unitOfWork);
            Repositorio.Embarcador.Cargas.CargaCIOT repCargaCIOT = new Repositorio.Embarcador.Cargas.CargaCIOT(unitOfWork);

            Dominio.Entidades.Embarcador.Cargas.CargaCIOT cargaCIOT = repCargaCIOT.BuscarPrimeiroPorCIOT(ciot.Codigo);
            this.configuracaoIntegracaoRepomFrete = this.ObterConfiguracaoRepomFrete(ciot.ConfiguracaoCIOT, unitOfWork);

            //Repom Frete não possui método de encerramento para CIOT padrão, o mesmo é encerrado automaticamente ao efetuar o pagamento do Saldo.
            if (!(this.configuracaoIntegracaoRepomFrete?.RealizarEncerramentoAutorizacaoPagamentoSeparado ?? false))
            {
                mensagemErro = "Encerramento não implementado para a operadora.";
                return false;
            }

            #region Buscar Branch Code (Código da Filial Repom)
            string branchCode = cargaCIOT.Carga.Pedidos?.FirstOrDefault()?.Pedido?.CentroDeCustoViagem?.CodigoFilialRepom;

            if (string.IsNullOrEmpty(branchCode))
                branchCode = cargaCIOT.Carga.Empresa.Configuracao.CodigoFilialRepom;
            #endregion

            bool sucesso = false;

            // Efetua o login na administradora e gera o token
            if (this.ObterToken(out mensagemErro))
            {
                var envioWS = ObterAutorizaContrato(ciot.ProtocoloAutorizacao, cargaCIOT, branchCode, unitOfWork);

                //Transmite o arquivo
                var retornoWS = this.TransmitirRepom(enumTipoWS.POST, envioWS, "ShippingPayment", this.tokenAutenticacao);

                #region Salvar JSON
                Dominio.Entidades.Embarcador.Documentos.CIOTIntegracaoArquivo ciotIntegracaoArquivo = new Dominio.Entidades.Embarcador.Documentos.CIOTIntegracaoArquivo
                {
                    ArquivoRequisicao = Servicos.Embarcador.Integracao.ArquivoIntegracao.SalvarArquivoIntegracao(retornoWS.jsonEnvio, "json", unitOfWork),
                    ArquivoResposta = Servicos.Embarcador.Integracao.ArquivoIntegracao.SalvarArquivoIntegracao(retornoWS.jsonRetorno, "json", unitOfWork),
                    Data = DateTime.Now,
                    Tipo = Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoArquivoIntegracaoCTeCarga.RetornoDoProcessamento,
                    Mensagem = mensagemErro ?? string.Empty
                };

                repCIOTIntegracaoArquivo.Inserir(ciotIntegracaoArquivo);

                ciot.ArquivosTransacao.Add(ciotIntegracaoArquivo);

                repCIOT.Atualizar(ciot);
                #endregion

                if (retornoWS.erro && string.IsNullOrEmpty(retornoWS.jsonRetorno))
                {
                    ciot.Mensagem = string.Concat("Ocorreu uma falha ao consumir o webservice: ", retornoWS.mensagem);

                    mensagemErro = "Falha na integração de encerramento do contrato de frete.";
                    sucesso = false;
                }
                else
                {
                    retPadrao retorno = null;

                    try
                    {
                        retorno = retornoWS.jsonRetorno.FromJson<retPadrao>();
                    }
                    catch (Exception ex)
                    {
                        Servicos.Log.TratarErro($"[Arquitetura-CatchNoAction] [EncerrarCIOT] Erro ao processar JSON de retorno do webservice RepomFrete, não foi possível fazer o parse no JSON para o retorno padrão: {ex.ToString()}", "CatchNoAction");
                    }

                    if (retorno == null)
                    {
                        ciot.Mensagem = string.Format("Message: Ocorreu uma falha ao efetuar o encerramento do contrato de frete; RetornoWS {0}.", retornoWS.jsonRetorno);

                        mensagemErro = "Falha na integração de encerramento do contrato de frete.";
                        sucesso = false;
                    }
                    else
                    {
                        if (retorno?.Response?.StatusCode == 200 || retorno?.Response?.StatusCode == 201)
                        {
                            ciot.Situacao = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoCIOT.Encerrado;
                            ciot.Mensagem = "Quitação realizada com sucesso.";
                            ciot.DataAutorizacaoPagamento = DateTime.Now;
                            ciot.DataEncerramento = DateTime.Now;

                            mensagemErro = "Quitação realizada com sucesso.";
                            sucesso = true;
                        }
                        else
                        {
                            string mensagemRetorno = "Rejeitado:";

                            if (retorno.Errors != null && retorno.Errors.Count() > 0)
                                retorno.Errors.ForEach(x => mensagemRetorno += System.Environment.NewLine + x.Message);
                            else if (!string.IsNullOrEmpty(retornoWS.jsonRetorno))
                            {
                                string mensagem = " ";
                                try
                                {
                                    JObject jo = JObject.Parse(retornoWS.jsonRetorno);
                                    mensagem = jo.Property("Message").ToString();
                                }
                                catch (Exception ex)
                                {
                                    // Erro ao fazer parse do JSON de retorno - continua com mensagem padrão
                                    Servicos.Log.TratarErro($"[Arquitetura-CatchNoAction] [EncerrarCIOT] Erro ao processar JSON de retorno do webservice RepomFrete, não foi possível fazer o parse no JSON para o retorno Rejeitado: {ex.ToString()}", "CatchNoAction");
                                    mensagem = " "; // Mantém valor padrão para continuar o fluxo
                                }
                                if (!string.IsNullOrEmpty(mensagem))
                                    mensagemRetorno += " Ocorreu um erro ao efetuar encerramento do contrato de frete.";
                                else
                                    mensagemRetorno += mensagem;
                            }
                            else
                                mensagemRetorno += " Ocorreu um erro ao efetuar encerramento do contrato de frete.";

                            ciot.Mensagem = mensagemRetorno;

                            mensagemErro = $"Falha na integração de encerramento do contrato de frete: {mensagemRetorno}";
                            sucesso = false;
                        }
                    }
                }
            }
            else
            {
                ciot.Mensagem = mensagemErro;
            }

            cargaCIOT.CIOT.Mensagem = mensagemErro;
            repCIOT.Atualizar(cargaCIOT.CIOT);

            return sucesso;
        }

        public bool EncerrarCIOTAgregado(Dominio.Entidades.Embarcador.Documentos.CIOT ciot, out string mensagemErro, Repositorio.UnitOfWork unitOfWork)
        {
            Repositorio.Embarcador.Documentos.CIOTIntegracaoArquivo repCIOTIntegracaoArquivo = new Repositorio.Embarcador.Documentos.CIOTIntegracaoArquivo(unitOfWork);
            Repositorio.Embarcador.Documentos.CIOT repCIOT = new Repositorio.Embarcador.Documentos.CIOT(unitOfWork);
            Repositorio.Embarcador.Cargas.CargaCIOT repCargaCIOT = new Repositorio.Embarcador.Cargas.CargaCIOT(unitOfWork);

            Dominio.Entidades.Embarcador.Cargas.CargaCIOT cargaCIOT = repCargaCIOT.BuscarPrimeiroPorCIOT(ciot.Codigo);
            this.configuracaoIntegracaoRepomFrete = this.ObterConfiguracaoRepomFrete(ciot.ConfiguracaoCIOT, unitOfWork);

            bool sucesso = false;

            foreach (Dominio.Entidades.Embarcador.Cargas.CargaCIOT cargaCIOTQuitacao in ciot.CargaCIOT)
            {
                if (cargaCIOT.Situacao != Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoCIOT.Encerrado)
                {
                    if (this.IntegrarAutorizacaoPagamento(out mensagemErro, ciot, cargaCIOTQuitacao, null, unitOfWork))
                        return false;
                }
            }

            // Efetua o login na administradora e gera o token
            if (this.ObterToken(out mensagemErro))
            {
                var envioWS = ObterShippingClosingCIOTAggregate(cargaCIOT, unitOfWork);

                //Transmite o arquivo
                var retornoWS = this.TransmitirRepom(enumTipoWS.POST, envioWS, "Shipping/CIOTAggregate/ClosingCIOT", this.tokenAutenticacao, false, "3.0");

                #region Salvar JSON
                Dominio.Entidades.Embarcador.Documentos.CIOTIntegracaoArquivo ciotIntegracaoArquivo = new Dominio.Entidades.Embarcador.Documentos.CIOTIntegracaoArquivo
                {
                    ArquivoRequisicao = Servicos.Embarcador.Integracao.ArquivoIntegracao.SalvarArquivoIntegracao(retornoWS.jsonEnvio, "json", unitOfWork),
                    ArquivoResposta = Servicos.Embarcador.Integracao.ArquivoIntegracao.SalvarArquivoIntegracao(retornoWS.jsonRetorno, "json", unitOfWork),
                    Data = DateTime.Now,
                    Tipo = Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoArquivoIntegracaoCTeCarga.RetornoDoProcessamento,
                    Mensagem = mensagemErro ?? string.Empty
                };

                repCIOTIntegracaoArquivo.Inserir(ciotIntegracaoArquivo);

                ciot.ArquivosTransacao.Add(ciotIntegracaoArquivo);

                repCIOT.Atualizar(ciot);
                #endregion

                if (retornoWS.erro && string.IsNullOrEmpty(retornoWS.jsonRetorno))
                {
                    ciot.Mensagem = string.Concat("Ocorreu uma falha ao consumir o webservice: ", retornoWS.mensagem);

                    mensagemErro = "Falha na integração de encerramento do contrato de frete.";
                    sucesso = false;
                }
                else
                {
                    retPadrao retorno = null;

                    try
                    {
                        retorno = retornoWS.jsonRetorno.FromJson<retPadrao>();
                    }
                    catch (Exception ex)
                    {
                        Servicos.Log.TratarErro($"[Arquitetura-CatchNoAction] [EncerrarCIOTAgregado] Erro ao processar JSON de retorno do webservice RepomFrete, não foi possível fazer o parse no JSON para o retorno padrão: {ex.ToString()}", "CatchNoAction");
                    }

                    if (retorno == null)
                    {
                        ciot.Mensagem = string.Format("Message: Ocorreu uma falha ao efetuar o encerramento do contrato de frete; RetornoWS {0}.", retornoWS.jsonRetorno);

                        mensagemErro = "Falha na integração de encerramento do contrato de frete.";
                        sucesso = false;
                    }
                    else
                    {
                        if (retorno?.Response?.StatusCode == 200 || retorno?.Response?.StatusCode == 201)
                        {
                            ciot.Situacao = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoCIOT.Encerrado;
                            ciot.Mensagem = "Encerramento realizado com sucesso.";
                            ciot.DataEncerramento = DateTime.Now;

                            mensagemErro = "Encerramento realizado com sucesso.";
                            sucesso = true;
                        }
                        else
                        {
                            string mensagemRetorno = "Rejeitado:";

                            if (retorno.Errors != null && retorno.Errors.Count() > 0)
                                retorno.Errors.ForEach(x => mensagemRetorno += System.Environment.NewLine + x.Message);
                            else if (!string.IsNullOrEmpty(retornoWS.jsonRetorno))
                            {
                                string mensagem = " ";
                                try
                                {
                                    JObject jo = JObject.Parse(retornoWS.jsonRetorno);
                                    mensagem = jo.Property("Message").ToString();
                                }
                                catch (Exception ex)
                                {
                                    // Erro ao fazer parse do JSON de retorno - continua com mensagem padrão
                                    Servicos.Log.TratarErro($"[Arquitetura-CatchNoAction] [EncerrarCIOTAgregado] Erro ao processar JSON de retorno do webservice RepomFrete, não foi possível fazer o parse no JSON para retorno Rejeitado: {ex.ToString()}", "CatchNoAction");
                                    mensagem = " "; // Mantém valor padrão para continuar o fluxo
                                }
                                if (!string.IsNullOrEmpty(mensagem))
                                    mensagemRetorno += " Ocorreu um erro ao efetuar encerramento do contrato de frete.";
                                else
                                    mensagemRetorno += mensagem;
                            }
                            else
                                mensagemRetorno += " Ocorreu um erro ao efetuar encerramento do contrato de frete.";

                            ciot.Mensagem = mensagemRetorno;

                            mensagemErro = $"Falha na integração de encerramento do contrato de frete: {mensagemRetorno}";
                            sucesso = false;
                        }
                    }
                }
            }
            else
            {
                ciot.Mensagem = mensagemErro;
            }

            cargaCIOT.CIOT.Mensagem = mensagemErro;
            repCIOT.Atualizar(cargaCIOT.CIOT);

            return sucesso;
        }

        #endregion

        #region Métodos Privados

        private envShippingClosingCIOTAggregate ObterShippingClosingCIOTAggregate(Dominio.Entidades.Embarcador.Cargas.CargaCIOT cargaCIOT, Repositorio.UnitOfWork unitOfWork)
        {
            envShippingClosingCIOTAggregate retorno = new envShippingClosingCIOTAggregate();
            retorno.TransportOperationIdentifierCode = cargaCIOT.CIOT.Numero;
            return retorno;
        }

        #endregion
    }
}
