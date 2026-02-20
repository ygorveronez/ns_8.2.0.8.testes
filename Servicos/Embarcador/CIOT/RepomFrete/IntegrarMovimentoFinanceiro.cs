using System;
using System.Linq;
using Dominio.ObjetosDeValor.Embarcador.CIOT.RepomFrete;
using Utilidades.Extensions;

namespace Servicos.Embarcador.CIOT.RepomFrete
{
    public partial class IntegracaoRepomFrete
    {
        #region Métodos Globais

        public bool IntegrarMovimentoFinanceiro(out string mensagemErro, Dominio.Entidades.Embarcador.Cargas.CargaCIOT cargaCIOT, Dominio.Entidades.Embarcador.Fatura.Justificativa justificativa, decimal valorMovimento, Repositorio.UnitOfWork unitOfWork)
        {
            Repositorio.Embarcador.Documentos.CIOTIntegracaoArquivo repCIOTIntegracaoArquivo = new Repositorio.Embarcador.Documentos.CIOTIntegracaoArquivo(unitOfWork);
            Repositorio.Embarcador.Documentos.CIOT repCIOT = new Repositorio.Embarcador.Documentos.CIOT(unitOfWork);

            this.configuracaoIntegracaoRepomFrete = this.ObterConfiguracaoRepomFrete(cargaCIOT.CIOT.ConfiguracaoCIOT, unitOfWork);


            string protocoloAutorizacao = !cargaCIOT.CIOT.CIOTPorPeriodo || string.IsNullOrEmpty(cargaCIOT.ProtocoloAutorizacao) ? cargaCIOT.CIOT.ProtocoloAutorizacao : cargaCIOT.ProtocoloAutorizacao;

            if (string.IsNullOrEmpty(protocoloAutorizacao))
            {
                mensagemErro = "Processo Abortado! ID da viagem não definido.";
                return false;
            }

            // Efetua o login na administradora e gera o token
            if (!this.ObterToken(out mensagemErro))
                return false;

            var envioWS = ObterMovimentoFinanceiro(cargaCIOT.CIOT, justificativa, valorMovimento, unitOfWork);

            //Transmite o arquivo
            var retornoWS = this.TransmitirRepom(enumTipoWS.PATCH, envioWS, $"Shipping/AddMovement/{protocoloAutorizacao}", this.tokenAutenticacao);

            if (retornoWS.erro && string.IsNullOrEmpty(retornoWS.jsonRetorno))
                mensagemErro = string.Concat("Ocorreu uma falha ao consumir o webservice: ", retornoWS.mensagem);
            else
                mensagemErro = "Movimento financeiro integrado com sucesso.";

            #region Salvar JSON
            Dominio.Entidades.Embarcador.Documentos.CIOTIntegracaoArquivo ciotIntegracaoArquivo = new Dominio.Entidades.Embarcador.Documentos.CIOTIntegracaoArquivo
            {
                ArquivoRequisicao = Servicos.Embarcador.Integracao.ArquivoIntegracao.SalvarArquivoIntegracao(retornoWS.jsonEnvio, "json", unitOfWork),
                ArquivoResposta = Servicos.Embarcador.Integracao.ArquivoIntegracao.SalvarArquivoIntegracao(retornoWS.jsonRetorno, "json", unitOfWork),
                Data = DateTime.Now,
                Tipo = Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoArquivoIntegracaoCTeCarga.RetornoDoProcessamento,
                Mensagem = mensagemErro
            };

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
                    Servicos.Log.TratarErro($"[Arquitetura-CatchNoAction] Erro ao processar JSON de retorno do webservice RepomFrete - IntegrarMovimentoFinanceiro: {ex.ToString()}", "CatchNoAction");
                }

                if (retorno == null)
                {
                    mensagemErro = string.Format("Message: Ocorreu uma falha ao incluir a movimentação financeira no contrato de frete; RetornoWS {0}.", retornoWS.jsonRetorno);
                    return false;
                }
                else
                {
                    if (retorno?.Response?.StatusCode == 200 || retorno?.Response?.StatusCode == 201)
                    {
                        mensagemErro = "Movimento financeiro integrado com sucesso.";
                        return true;
                    }
                    else
                    {
                        string mensagemRetorno = "Rejeitado:";

                        if (retorno.Errors != null && retorno.Errors.Count() > 0)
                            retorno.Errors.ForEach(x => mensagemRetorno += System.Environment.NewLine + x.Message);
                        else
                            mensagemRetorno += " Ocorreu uma falha ao incluir a movimentação financeira no contrato de frete.";

                        mensagemErro = mensagemRetorno;
                        return false;
                    }
                }
            }
        }

        #endregion

        #region Métodos Privados

        private envShippingAddMovement ObterMovimentoFinanceiro(Dominio.Entidades.Embarcador.Documentos.CIOT ciot, Dominio.Entidades.Embarcador.Fatura.Justificativa justificativa, decimal valorMovimento, Repositorio.UnitOfWork unitOfWork)
        {
            envShippingAddMovement retorno = new envShippingAddMovement();

            retorno.Identifier = justificativa.CodigoIntegracaoRepom;
            retorno.Value = valorMovimento;

            return retorno;
        }

        #endregion
    }
}

