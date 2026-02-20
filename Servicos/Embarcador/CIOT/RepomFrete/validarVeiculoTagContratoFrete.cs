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

        public bool validarVeiculoTagContratoFrete(Dominio.Entidades.Cliente proprietario, Dominio.Entidades.Veiculo veiculo, bool carreta, Repositorio.UnitOfWork unitOfWork, out string mensagemErro)
        {
            mensagemErro = null;

            try
            {
                if (carreta && veiculo == null)
                    return true;

                bool sucesso = false;

                retornoWebService retornoWS = this.TransmitirRepom(enumTipoWS.GET, null, $"ShippingValidation/ByVehiclesTag/{veiculo.Placa}", this.tokenAutenticacao);

                if (retornoWS.erro && string.IsNullOrEmpty(retornoWS.jsonRetorno))
                {
                    mensagemErro = string.Concat("Ocorreu uma falha ao consumir o webservice: ", retornoWS.mensagem);
                    return false;
                }
                else
                {
                    retShippingValidationByVehiclesTag retorno = null;

                    try
                    {
                        retorno = retornoWS.jsonRetorno.FromJson<retShippingValidationByVehiclesTag>();
                    }
                    catch (Exception ex)
                    {
                        Servicos.Log.TratarErro($"[Arquitetura-CatchNoAction] Erro ao desserializar validação de tag do veículo RepomFrete: {ex.ToString()}", "CatchNoAction");
                    }

                    if (retorno == null)
                    {
                        mensagemErro = string.Format("Message: Ocorreu uma falha ao efetuar a validação da tag do veículo; RetornoWS {0}.", retornoWS.jsonRetorno);
                        sucesso = false;
                    }
                    else
                    {
                        if (retorno.Response.StatusCode == 200 || retorno.Response.StatusCode == 201)
                        {
                            sucesso = true;
                        }
                        else
                        {
                            string mensagemRetorno = "Falha ao efetuar a validação da tag do veículo:";

                            if (retorno.Errors != null && retorno.Errors.Count() > 0)
                                retorno.Errors.ForEach(x => mensagemRetorno += System.Environment.NewLine + x.Message);
                            else
                                mensagemRetorno += " Ocorreu uma falha ao efetuar a validação da tag do veículo.";

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

                mensagemErro = "Ocorreu uma falha ao validar a tag do veículo do CIOT.";
                return false;
            }
        }

        #endregion
    }
}
