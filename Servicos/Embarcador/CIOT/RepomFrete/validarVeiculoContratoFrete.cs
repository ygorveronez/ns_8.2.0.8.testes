using System;
using System.Linq;
using Dominio.ObjetosDeValor.Embarcador.CIOT.RepomFrete;
using Utilidades.Extensions;

namespace Servicos.Embarcador.CIOT.RepomFrete
{
    public partial class IntegracaoRepomFrete
    {
        #region Métodos Globais

        #endregion

        #region Métodos Privados

        public bool validarVeiculoContratoFrete(Dominio.Entidades.Veiculo veiculo, bool carreta, Repositorio.UnitOfWork unitOfWork, out string mensagemErro)
        {
            mensagemErro = null;

            try
            {
                if (!this.configuracaoIntegracaoRepomFrete.UtilizarMetodosValidacaoCadastros)
                    return true;

                if (carreta && veiculo == null)
                    return true;

                bool sucesso = false;

                retornoWebService retornoWS = this.TransmitirRepom(enumTipoWS.GET, null, $"ShippingValidation/ByVehicles/{veiculo.Placa}", this.tokenAutenticacao);

                if (retornoWS.erro && string.IsNullOrEmpty(retornoWS.jsonRetorno))
                {
                    mensagemErro = string.Concat("Ocorreu uma falha ao consumir o webservice: ", retornoWS.mensagem);
                    return false;
                }
                else
                {
                    retShippingValidationByVehicles retorno = null;

                    try
                    {
                        retorno = retornoWS.jsonRetorno.FromJson<retShippingValidationByVehicles>();
                    }
                    catch (Exception ex)
                    {
                        Servicos.Log.TratarErro($"[Arquitetura-CatchNoAction] Erro ao desserializar validação de veículo RepomFrete: {ex.ToString()}", "CatchNoAction");
                    }

                    if (retorno == null)
                    {
                        mensagemErro = string.Format("Message: Ocorreu uma falha ao efetuar a validação do veículo; RetornoWS {0}.", retornoWS.jsonRetorno);
                        sucesso = false;
                    }
                    else
                    {
                        if (retorno.Response.StatusCode == 200 || retorno.Response.StatusCode == 201)
                        {
                            retShippingValidationByVehiclesVehicle vehicle = retorno.Result.Vehicles.Where(o => o.LicensePlate == veiculo.Placa).FirstOrDefault();

                            if (vehicle != null && !vehicle.Validate)  
                            {
                                if (!string.IsNullOrEmpty(vehicle.MessageError))
                                    mensagemErro = $"Falha validando o veículo: {vehicle.MessageError}";
                                else
                                    mensagemErro = $"Falha validando o veículo na Repom Frete.";

                                sucesso = false;
                            }
                            else
                            {
                                sucesso = true;
                            }
                        }
                        else
                        {
                            string mensagemRetorno = "Falha ao efetuar a validação do veículo:";

                            if (retorno.Errors != null && retorno.Errors.Count() > 0)
                                retorno.Errors.ForEach(x => mensagemRetorno += System.Environment.NewLine + x.Message);
                            else
                                mensagemRetorno += " Ocorreu uma falha ao efetuar a validação do veículo.";

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

                mensagemErro = "Ocorreu uma falha ao validar os dados do veículo do CIOT.";
                return false;
            }
        }

        #endregion
    }
}
