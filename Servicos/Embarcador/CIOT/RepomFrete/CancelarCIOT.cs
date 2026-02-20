using System;
using System.Linq;
using Dominio.Entidades.Embarcador.Fatura;
using Dominio.ObjetosDeValor.Embarcador.CIOT.RepomFrete;
using Utilidades.Extensions;

namespace Servicos.Embarcador.CIOT.RepomFrete
{
    public partial class IntegracaoRepomFrete
    {
        #region Métodos Globais

        public bool CancelarCIOT(Dominio.Entidades.Embarcador.Documentos.CIOT ciot, Repositorio.UnitOfWork unitOfWork, out string mensagemErro, bool atualizarCIOT = true)
        {
            if (ciot.CIOTPorPeriodo)
                return CancelarCIOTAgregado(ciot, unitOfWork, out mensagemErro, atualizarCIOT);

            System.Globalization.CultureInfo cultura = new System.Globalization.CultureInfo("en-US");

            Repositorio.Embarcador.Documentos.CIOTIntegracaoArquivo repCIOTIntegracaoArquivo = new Repositorio.Embarcador.Documentos.CIOTIntegracaoArquivo(unitOfWork);
            Repositorio.Embarcador.Documentos.CIOT repCIOT = new Repositorio.Embarcador.Documentos.CIOT(unitOfWork);
            Repositorio.Embarcador.Cargas.CargaCIOT repCargaCIOT = new Repositorio.Embarcador.Cargas.CargaCIOT(unitOfWork);

            this.configuracaoIntegracaoRepomFrete = this.ObterConfiguracaoRepomFrete(ciot.ConfiguracaoCIOT, unitOfWork);
            Dominio.Entidades.Embarcador.Cargas.CargaCIOT cargaCIOT = repCargaCIOT.BuscarPrimeiroPorCIOT(ciot.Codigo);

            if (string.IsNullOrEmpty(cargaCIOT.CIOT.ProtocoloAutorizacao))
            {
                mensagemErro = "Processo Abortado! ID da viagem não definido.";
                if (cargaCIOT.CIOT.Situacao == Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoCIOT.Pendencia)
                    return true;
                else
                    return false;
            }

            enumStatusCIOT status = ConsultaStatusCIOT(cargaCIOT.CIOT, unitOfWork, out mensagemErro);

            if (status == enumStatusCIOT.CANCELLED)
            {
                cargaCIOT.CIOT.Situacao = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoCIOT.Cancelado;
                cargaCIOT.CIOT.Mensagem = "Cancelamento realizado com sucesso.";
                cargaCIOT.CIOT.DataCancelamento = DateTime.Now;

                repCIOT.Atualizar(cargaCIOT.CIOT);

                return true;
            }
            else if (status == enumStatusCIOT.ERROR)
            {
                return false;
            }
            else
            {
                mensagemErro = null;

                // Efetua o login na administradora e gera o token
                if (!this.ObterToken(out mensagemErro))
                    return false;


                //Shipping ID
                string cEnvio = cargaCIOT.CIOT.ProtocoloAutorizacao;

                //Transmite o arquivo
                var retornoWS = this.TransmitirRepom(enumTipoWS.PATCH, cEnvio, "Shipping/Cancel", this.tokenAutenticacao);

                #region Salvar JSON
                Dominio.Entidades.Embarcador.Documentos.CIOTIntegracaoArquivo ciotIntegracaoArquivo = new Dominio.Entidades.Embarcador.Documentos.CIOTIntegracaoArquivo();

                ciotIntegracaoArquivo.ArquivoRequisicao = Servicos.Embarcador.Integracao.ArquivoIntegracao.SalvarArquivoIntegracao(retornoWS.jsonEnvio, "json", unitOfWork);
                ciotIntegracaoArquivo.ArquivoResposta = Servicos.Embarcador.Integracao.ArquivoIntegracao.SalvarArquivoIntegracao(retornoWS.jsonRetorno, "json", unitOfWork);
                ciotIntegracaoArquivo.Data = DateTime.Now;
                ciotIntegracaoArquivo.Tipo = Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoArquivoIntegracaoCTeCarga.RetornoDoProcessamento;
                ciotIntegracaoArquivo.Mensagem = "Envio do cancelamento do CIOT.";

                repCIOTIntegracaoArquivo.Inserir(ciotIntegracaoArquivo);

                cargaCIOT.CIOT.ArquivosTransacao.Add(ciotIntegracaoArquivo);

                repCIOT.Atualizar(cargaCIOT.CIOT);
                #endregion

                if (retornoWS.erro && string.IsNullOrEmpty(retornoWS.jsonRetorno))
                {
                    mensagemErro = string.Concat("Ocorreu uma falha ao consumir o webservice: ", retornoWS.mensagem);
                    return false;
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
                    Servicos.Log.TratarErro($"[Arquitetura-CatchNoAction] Erro ao processar JSON de retorno do webservice RepomFrete - CancelarCIOT: {ex.ToString()}", "CatchNoAction");
                }

                    if (retorno == null)
                    {
                        mensagemErro = string.Format("Message: Ocorreu uma falha ao efetuar o cancelamento do contrato de frete; RetornoWS {0}.", retornoWS.jsonRetorno);
                        return false;
                    }
                    else
                    {
                        if (retorno?.Response?.StatusCode == 200 || retorno?.Response?.StatusCode == 201)
                        {
                            if (atualizarCIOT)
                            {
                                cargaCIOT.CIOT.Situacao = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoCIOT.Cancelado;
                                cargaCIOT.CIOT.Mensagem = "Cancelamento realizado com sucesso.";
                                cargaCIOT.CIOT.DataCancelamento = DateTime.Now;

                                repCIOT.Atualizar(cargaCIOT.CIOT);
                            }

                            return true;
                        }
                        else
                        {
                            string mensagemRetorno = "Rejeitado:";

                            if (retorno.Errors != null && retorno.Errors.Count() > 0)
                                retorno.Errors.ForEach(x => mensagemRetorno += System.Environment.NewLine + x.Message);
                            else
                                mensagemRetorno += " Ocorreu uma falha ao efetuar o cancelamento do contrato de frete.";

                            mensagemErro = mensagemRetorno;
                            return false;
                        }
                    }
                }
            }
        }

        public bool CancelarCIOTAgregado(Dominio.Entidades.Embarcador.Documentos.CIOT ciot, Repositorio.UnitOfWork unitOfWork, out string mensagemErro, bool atualizarCIOT = true)
        {
            System.Globalization.CultureInfo cultura = new System.Globalization.CultureInfo("en-US");

            Repositorio.Embarcador.Documentos.CIOTIntegracaoArquivo repCIOTIntegracaoArquivo = new Repositorio.Embarcador.Documentos.CIOTIntegracaoArquivo(unitOfWork);
            Repositorio.Embarcador.Documentos.CIOT repCIOT = new Repositorio.Embarcador.Documentos.CIOT(unitOfWork);
            Repositorio.Embarcador.Cargas.CargaCIOT repCargaCIOT = new Repositorio.Embarcador.Cargas.CargaCIOT(unitOfWork);

            this.configuracaoIntegracaoRepomFrete = this.ObterConfiguracaoRepomFrete(ciot.ConfiguracaoCIOT, unitOfWork);
            Dominio.Entidades.Embarcador.Cargas.CargaCIOT cargaCIOT = repCargaCIOT.BuscarPrimeiroPorCIOT(ciot.Codigo);

            if (string.IsNullOrEmpty(cargaCIOT.CIOT.Numero))
            {
                mensagemErro = "Processo Abortado! Numero do CIOT não definido.";
                if (cargaCIOT.CIOT.Situacao == Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoCIOT.Pendencia)
                    return true;
                else
                    return false;
            }

            mensagemErro = null;
            string justificativa = "Cancelamento problema na emissão.";

            // Efetua o login na administradora e gera o token
            if (!this.ObterToken(out mensagemErro))
                return false;

            var envioWS = ObterShippingCancelCIOTAggregate(cargaCIOT, justificativa, unitOfWork);

            //Transmite o arquivo
            var retornoWS = this.TransmitirRepom(enumTipoWS.POST, envioWS, "Shipping/CIOTAggregate/CancelCIOT", this.tokenAutenticacao, false, "3.0");

            #region Salvar JSON
            Dominio.Entidades.Embarcador.Documentos.CIOTIntegracaoArquivo ciotIntegracaoArquivo = new Dominio.Entidades.Embarcador.Documentos.CIOTIntegracaoArquivo();

            ciotIntegracaoArquivo.ArquivoRequisicao = Servicos.Embarcador.Integracao.ArquivoIntegracao.SalvarArquivoIntegracao(retornoWS.jsonEnvio, "json", unitOfWork);
            ciotIntegracaoArquivo.ArquivoResposta = Servicos.Embarcador.Integracao.ArquivoIntegracao.SalvarArquivoIntegracao(retornoWS.jsonRetorno, "json", unitOfWork);
            ciotIntegracaoArquivo.Data = DateTime.Now;
            ciotIntegracaoArquivo.Tipo = Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoArquivoIntegracaoCTeCarga.RetornoDoProcessamento;
            ciotIntegracaoArquivo.Mensagem = "Envio do cancelamento do CIOT.";

            repCIOTIntegracaoArquivo.Inserir(ciotIntegracaoArquivo);

            cargaCIOT.CIOT.ArquivosTransacao.Add(ciotIntegracaoArquivo);

            repCIOT.Atualizar(cargaCIOT.CIOT);
            #endregion

            if (retornoWS.erro && string.IsNullOrEmpty(retornoWS.jsonRetorno))
            {
                mensagemErro = string.Concat("Ocorreu uma falha ao consumir o webservice: ", retornoWS.mensagem);
                return false;
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
                    Servicos.Log.TratarErro($"[Arquitetura-CatchNoAction] Erro ao desserializar resposta do cancelamento de CIOT RepomFrete: {ex.ToString()}", "CatchNoAction");
                }

                if (retorno == null)
                {
                    mensagemErro = string.Format("Message: Ocorreu uma falha ao efetuar o cancelamento do contrato de frete; RetornoWS {0}.", retornoWS.jsonRetorno);
                    return false;
                }
                else
                {
                    if (retorno?.Response?.StatusCode == 200 || retorno?.Response?.StatusCode == 201)
                    {
                        if (atualizarCIOT)
                        {
                            cargaCIOT.CIOT.Situacao = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoCIOT.Cancelado;
                            cargaCIOT.CIOT.Mensagem = "Cancelamento realizado com sucesso.";
                            cargaCIOT.CIOT.DataCancelamento = DateTime.Now;

                            repCIOT.Atualizar(cargaCIOT.CIOT);
                        }

                        return true;
                    }
                    else
                    {
                        string mensagemRetorno = "Rejeitado:";

                        if (retorno.Errors != null && retorno.Errors.Count() > 0)
                            retorno.Errors.ForEach(x => mensagemRetorno += System.Environment.NewLine + x.Message);
                        else
                            mensagemRetorno += " Ocorreu uma falha ao efetuar o cancelamento do contrato de frete.";

                        mensagemErro = mensagemRetorno;
                        return false;
                    }
                }
            }
        }

        public enumStatusCIOT ConsultaStatusCIOT(Dominio.Entidades.Embarcador.Documentos.CIOT ciot, Repositorio.UnitOfWork unitOfWork, out string mensagemErro)
        {
            enumStatusCIOT retornostatus = enumStatusCIOT.ERROR;            

            // Efetua o login na administradora e gera o token
            if (!this.ObterToken(out mensagemErro))
                return enumStatusCIOT.ERROR;

            //Shipping ID
            string cEnvio = ciot.ProtocoloAutorizacao;

            //Consulta Status             
            var retornoWS = this.TransmitirRepom(enumTipoWS.GET, cEnvio, "Shipping/ByShipping", this.tokenAutenticacao);

            retShippingByShipping retorno = null;

            try
            {
                retorno = retornoWS.jsonRetorno.FromJson<retShippingByShipping>();
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro($"[Arquitetura-CatchNoAction] Erro ao desserializar retorno ShippingByShipping no cancelamento CIOT: {ex.ToString()}", "CatchNoAction");
            }

            if (retorno != null)
            {
                if (retorno?.Response?.StatusCode == 200 || retorno?.Response?.StatusCode == 201)
                {
                    if (retorno.Result != null)
                    {                        
                        retornostatus = Dominio.ObjetosDeValor.Embarcador.CIOT.RepomFrete.enumRepomFrete.ObterEnumStatusCIOT(retorno.Result.Status);                        
                    }
                }
            }
            else
            {
                retornostatus = enumStatusCIOT.ERROR;
            }

            if (retornostatus == enumStatusCIOT.CANCELLED)
                mensagemErro = "Processo Abortado! CIOT já está cancelado na Repom Frete.";

            return retornostatus;
        }
        #endregion

        #region Métodos Privados

        private envShippingCancelCIOTAggregate ObterShippingCancelCIOTAggregate(Dominio.Entidades.Embarcador.Cargas.CargaCIOT cargaCIOT, string justificativa, Repositorio.UnitOfWork unitOfWork)
        {
            envShippingCancelCIOTAggregate retorno = new envShippingCancelCIOTAggregate();
            retorno.TransportOperationIdentifierCode = cargaCIOT.CIOT.Numero;
            retorno.Reason = justificativa;
            return retorno;
        }

        #endregion
    }
}
