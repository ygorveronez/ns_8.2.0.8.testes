using System;
using System.Collections.Generic;
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

        public bool IntegrarVeiculo(Dominio.Entidades.Veiculo veiculo, bool carreta, Repositorio.UnitOfWork unitOfWork, out string mensagemErro, out string jsonEnvio, out string jsonRetorno)
        {
            mensagemErro = null;

            jsonEnvio = string.Empty;
            jsonRetorno = string.Empty;

            try
            {
                if (carreta && veiculo == null)
                    return true;

                #region Verificar veículo existe Repom
                bool existeRepom = false;
                bool sucesso = false;
                retornoWebService retornoWS = null;
                
                retornoWS = this.TransmitirRepom(enumTipoWS.GET, null, $"Vehicle/ByDocument/Brazil/{veiculo.Placa}", this.tokenAutenticacao);

                if (!retornoWS.erro && !string.IsNullOrEmpty(retornoWS.jsonRetorno))
                    existeRepom = true;
                
                #endregion

                object envioWS = null;
                if (carreta)
                    envioWS = ObterCarreta(veiculo);
                else
                    envioWS = ObterVeiculo(veiculo);

                //Transmite o arquivo
                if (existeRepom)
                    retornoWS = this.TransmitirRepom(enumTipoWS.PUT, envioWS, $"Vehicle/Brazil/{veiculo.Placa}", this.tokenAutenticacao);
                else
                    retornoWS = this.TransmitirRepom(enumTipoWS.POST, envioWS, "Vehicle", this.tokenAutenticacao);

                jsonEnvio = retornoWS.jsonEnvio;
                jsonRetorno = retornoWS.jsonRetorno;

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
                        Servicos.Log.TratarErro($"[Arquitetura-CatchNoAction] Erro ao desserializar resposta do cadastro de veículo RepomFrete: {ex.ToString()}", "CatchNoAction");
                    }

                    if (retorno == null)
                    {
                        mensagemErro = string.Format("Message: Ocorreu uma falha ao efetuar o cadastro do veículo; RetornoWS {0}.", retornoWS.jsonRetorno);
                        sucesso = false;
                    }
                    else
                    {
                        // Já existe um veiculo com a placa
                        if (retorno.Errors != null && retorno?.Errors?.Where(x => x.ErrorCode == 435).FirstOrDefault() != null)
                        {
                            sucesso = true;
                        }
                        else
                        {
                            if (retorno?.Response?.StatusCode == 200 || retorno?.Response?.StatusCode == 201)
                            {
                                sucesso = true;
                            }
                            else
                            {
                                string mensagemRetorno = "Falha ao efetuar o cadastro do veículo:";

                                if (retorno.Errors != null && retorno.Errors.Count() > 0)
                                    retorno.Errors.ForEach(x => mensagemRetorno += System.Environment.NewLine + x.Message);
                                else
                                    mensagemRetorno += " Ocorreu uma falha ao efetuar o cadastro do veículo.";

                                mensagemErro = mensagemRetorno;
                                sucesso = false;
                            }
                        }
                    }
                }

                if (existeRepom && !sucesso)
                {
                    Servicos.Log.TratarErro($"Integração Repom Atualizar veículo: {mensagemErro}");
                    return true;
                }

                return sucesso;
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);

                mensagemErro = "Ocorreu uma falha ao integrar os dados do veículo do CIOT.";
                return false;
            }
        }

        public bool IntegrarVeiculosVinculados(List<Dominio.Entidades.Veiculo> listaVeiculo, bool carreta, Repositorio.UnitOfWork unitOfWork, out string mensagemErro, out string jsonEnvio, out string jsonRetorno)
        {
            mensagemErro = null;

            jsonEnvio = string.Empty;
            jsonRetorno = string.Empty;

            bool sucesso = false;
            try
            {
                if (listaVeiculo == null || listaVeiculo.Count == 0)
                    return true;

                foreach (var veiculo in listaVeiculo)
                {
                    #region Verificar veículo existe Repom
                    bool existeRepom = false;
                    sucesso = false;
                    retornoWebService retornoWS = null;

                    retornoWS = this.TransmitirRepom(enumTipoWS.GET, null, $"Vehicle/ByDocument/Brazil/{veiculo.Placa}", this.tokenAutenticacao);

                    if (!retornoWS.erro && !string.IsNullOrEmpty(retornoWS.jsonRetorno))
                        existeRepom = true;

                    #endregion

                    object envioWS = null;
                    if (carreta)
                        envioWS = ObterCarreta(veiculo);
                    else
                        envioWS = ObterVeiculo(veiculo);

                    //Transmite o arquivo
                    if (existeRepom)
                        retornoWS = this.TransmitirRepom(enumTipoWS.PUT, envioWS, $"Vehicle/Brazil/{veiculo.Placa}", this.tokenAutenticacao);
                    else
                        retornoWS = this.TransmitirRepom(enumTipoWS.POST, envioWS, "Vehicle", this.tokenAutenticacao);

                    jsonEnvio = retornoWS.jsonEnvio;
                    jsonRetorno = retornoWS.jsonRetorno;

                    if (retornoWS.erro && string.IsNullOrEmpty(retornoWS.jsonRetorno))
                    {
                        mensagemErro += string.Concat("Ocorreu uma falha ao consumir o webservice: ", retornoWS.mensagem);
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
                            Servicos.Log.TratarErro($"[Arquitetura-CatchNoAction] Erro ao desserializar segunda tentativa de cadastro de veículo RepomFrete: {ex.ToString()}", "CatchNoAction");
                        }

                        if (retorno == null)
                        {
                            mensagemErro += string.Format("Message: Ocorreu uma falha ao efetuar o cadastro do veículo; RetornoWS {0}.", retornoWS.jsonRetorno);
                            sucesso = false;
                        }
                        else
                        {
                            // Já existe um veiculo com a placa
                            if (retorno.Errors != null && retorno?.Errors?.Where(x => x.ErrorCode == 435).FirstOrDefault() != null)
                            {
                                sucesso = true;
                            }
                            else
                            {
                                if (retorno?.Response?.StatusCode == 200 || retorno?.Response?.StatusCode == 201)
                                {
                                    sucesso = true;
                                }
                                else
                                {
                                    string mensagemRetorno = "Falha ao efetuar o cadastro do veículo:";

                                    if (retorno.Errors != null && retorno.Errors.Count() > 0)
                                        retorno.Errors.ForEach(x => mensagemRetorno += System.Environment.NewLine + x.Message);
                                    else
                                        mensagemRetorno += " Ocorreu uma falha ao efetuar o cadastro do veículo.";

                                    mensagemErro += mensagemRetorno;
                                    sucesso = false;
                                }
                            }
                        }
                    }

                    if (existeRepom && !sucesso)
                    {
                        Servicos.Log.TratarErro($"Integração Repom Atualizar veículo: {mensagemErro}");
                        sucesso = true;
                    }

                }

                return sucesso;
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);

                mensagemErro = "Ocorreu uma falha ao integrar os dados do veículo do CIOT.";
                return false;
            }
        }

        private envVehicle ObterVeiculo(Dominio.Entidades.Veiculo veiculo)
        {
            var retorno = new envVehicle();

            retorno.Country = "Brazil";
            retorno.LicensePlate = veiculo.Placa;
            retorno.VehicleCategory = "HeavyCommercial";
            retorno.VehicleAxles = "Axle" + (veiculo.ModeloVeicularCarga?.NumeroEixos ?? 0).ToString().PadLeft(2, '0');
            retorno.VehicleClassification = "Traction";
            
            if (veiculo?.TipoRodado == "02")
                retorno.Type = "Toco";
            else
                retorno.Type = "Truck";

            retorno.VehicleOwner = GerarVehicleOwner(veiculo);

            return retorno;
        }

        private envVehicle ObterCarreta(Dominio.Entidades.Veiculo veiculo)
        {
            var retorno = new envVehicle();

            retorno.Country = "Brazil";
            retorno.LicensePlate = veiculo.Placa;
            retorno.VehicleCategory = "HeavyCommercial";
            retorno.VehicleAxles = "Axle" + (veiculo.ModeloVeicularCarga?.NumeroEixos ?? 0).ToString().PadLeft(2, '0');
            retorno.VehicleClassification = "Implement";
            retorno.Type = null;

            retorno.VehicleOwner = GerarVehicleOwner(veiculo);

            return retorno;
        }

        private VehicleOwner GerarVehicleOwner(Dominio.Entidades.Veiculo veiculo)
        {
            VehicleOwner retorno = new  VehicleOwner();

            retorno.Country = "Brazil";

            if (veiculo.Proprietario.Tipo == "J")
                retorno.Type = "Company";
            else if (veiculo.Proprietario.Tipo == "F")
                retorno.Type = "Person";

            retorno.NationalId = veiculo.Proprietario.CPF_CNPJ_SemFormato;
            retorno.BrazilianSettings = this.GerarVehicleBrazilianSettings(veiculo);
            retorno.VehiclePersonalInformation = this.GerarVehiclePersonalInformation(veiculo.Proprietario);

            return retorno;
        }

        private VehicleBrazilianSettings GerarVehicleBrazilianSettings(Dominio.Entidades.Veiculo veiculo)
        {
            VehicleBrazilianSettings retorno = new VehicleBrazilianSettings();

            retorno.RNTRC = veiculo.RNTRC == 0 ? null : veiculo.RNTRC.ToString().PadLeft(9, '0');
            if (veiculo.Proprietario.Tipo == "J")
                retorno.VehiclePessoaJuridica = GerarVehiclePessoaJuridica(veiculo.Proprietario);

            return retorno;
        }

        private VehiclePessoaJuridica GerarVehiclePessoaJuridica(Dominio.Entidades.Cliente contratado)
        {
            VehiclePessoaJuridica retorno = new VehiclePessoaJuridica();

            retorno.NomeFantasia = contratado.NomeFantasia.Left(50);

            return retorno;
        }

        private VehiclePersonalInformation GerarVehiclePersonalInformation(Dominio.Entidades.Cliente contratado)
        {
            VehiclePersonalInformation retorno = new VehiclePersonalInformation();

            retorno.Name = contratado.Nome.Left(50);

            return retorno;
        }
    }

    #endregion
}

