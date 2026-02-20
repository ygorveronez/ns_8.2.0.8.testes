using System;
using System.Linq;
using Utilidades.Extensions;
using Dominio.ObjetosDeValor.Embarcador.CIOT.RepomFrete;

namespace Servicos.Embarcador.CIOT.RepomFrete
{
    public partial class IntegracaoRepomFrete
    {
        #region Métodos Globais

        #endregion

        #region Métodos Privados

        private bool validarProprietarioContratoFrete(Dominio.Entidades.Cliente proprietario, Dominio.Entidades.Embarcador.Pessoas.ModalidadeTransportadoraPessoas modalidade, Repositorio.UnitOfWork unitOfWork, out string mensagemErro)
        {
            mensagemErro = null;

            if (!this.configuracaoIntegracaoRepomFrete.UtilizarMetodosValidacaoCadastros)
                return true;

            if (proprietario == null)
                return true;

            if (modalidade == null)
            {
                mensagemErro = "A modalidade do transportador não está configurada.";
                return false;
            }

            try
            {
                bool sucesso = false;
                string nationalID = proprietario.CPF_CNPJ_SemFormato;

                retornoWebService retornoWS = this.TransmitirRepom(enumTipoWS.GET, null, $"ShippingValidation/ByHiredDocument/Brazil/{nationalID}", this.tokenAutenticacao);

                if (retornoWS.erro && string.IsNullOrEmpty(retornoWS.jsonRetorno))
                {
                    mensagemErro = string.Concat("Ocorreu uma falha ao consumir o webservice: ", retornoWS.mensagem);
                    return false;
                }
                else
                {
                    retShippingValidationByHiredDocument retorno = null;

                    try
                    {
                        retorno = retornoWS.jsonRetorno.FromJson<retShippingValidationByHiredDocument>();
                    }
                    catch (Exception ex)
                    {
                        Servicos.Log.TratarErro($"[Arquitetura-CatchNoAction] Erro ao desserializar validação de proprietário RepomFrete: {ex.ToString()}", "CatchNoAction");
                    }

                    if (retorno == null)
                    {
                        mensagemErro = string.Format("Message: Ocorreu uma falha ao efetuar a validação do proprietário; RetornoWS {0}.", retornoWS.jsonRetorno);
                        sucesso = false;
                    }
                    else
                    {
                        if (retorno.Response.StatusCode == 200 || retorno.Response.StatusCode == 201)
                        {
                            if (!retorno.Result.HiredValidate && !string.IsNullOrEmpty(retorno.Result.Message))
                            {
                                mensagemErro = $"Falha validando o proprietário: {retorno.Result.Message}";
                                sucesso = false;
                            }
                            else if (!retorno.Result.HiredValidate)
                            {
                                mensagemErro = "Falha validando o proprietário: Invalido na ANTT ou ANTT vencida ou inconsistência na ANTT – Contatar a ANTT para regularização dos dados dos dados quando TAC.";
                                sucesso = false;
                            }
                            else
                            {
                                sucesso = true;
                            }
                        }
                        else
                        {
                            string mensagemRetorno = "Falha ao efetuar a validação do proprietário:";

                            if (retorno.Errors != null && retorno.Errors.Count() > 0)
                                retorno.Errors.ForEach(x => mensagemRetorno += System.Environment.NewLine + x.Message);
                            else
                                mensagemRetorno += " Ocorreu uma falha ao efetuar a validação do proprietário.";

                            mensagemErro = mensagemRetorno;
                            sucesso = false;
                        }
                    }
                }

                return sucesso;
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);

                mensagemErro = "Ocorreu uma falha ao validar os dados do proprietário do CIOT.";
                return false;
            }
        }

        #endregion
    }
}
