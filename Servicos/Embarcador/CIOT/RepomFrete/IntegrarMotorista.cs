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

        private bool IntegrarMotorista(Dominio.Entidades.Cliente proprietario, Dominio.Entidades.Usuario motorista, Repositorio.UnitOfWork unitOfWork, out string mensagemErro, out string jsonEnvio, out string jsonRetorno)
        {
            mensagemErro = null;

            jsonEnvio = string.Empty;
            jsonRetorno = string.Empty;

            try
            {
                #region Verificar motorista existe Repom
                bool existeRepom = false;
                bool sucesso = false;
                retornoWebService retornoWS = null;
                string nationalID = motorista.CPF.PadLeft(11, '0');

                retornoWS = this.TransmitirRepom(enumTipoWS.GET, null, $"Driver/ByDocument/Brazil/{nationalID}", this.tokenAutenticacao);

                if (!retornoWS.erro && !string.IsNullOrEmpty(retornoWS.jsonRetorno))
                    existeRepom = true;
                
                #endregion

                var envioWS = ObterMotorista(motorista, proprietario);

                if (existeRepom)
                    retornoWS = this.TransmitirRepom(enumTipoWS.PUT, envioWS, $"Driver/Brazil/{nationalID}", this.tokenAutenticacao);
                else
                    retornoWS = this.TransmitirRepom(enumTipoWS.POST, envioWS, "Driver", this.tokenAutenticacao);

                jsonEnvio = retornoWS.jsonEnvio;
                jsonRetorno = retornoWS.jsonRetorno;

                if (retornoWS.erro && string.IsNullOrEmpty(retornoWS.jsonRetorno))
                {
                    mensagemErro = string.Concat("Ocorreu uma falha ao consumir o webservice: ", retornoWS.mensagem);
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
                        Servicos.Log.TratarErro($"[Arquitetura-CatchNoAction] Erro ao processar JSON de retorno do webservice RepomFrete - IntegrarMotorista: {ex.ToString()}", "CatchNoAction");
                    }

                    if (retorno == null)
                    {
                        mensagemErro = string.Format("Message: Ocorreu uma falha ao efetuar o cadastro do motorista; RetornoWS {0}.", retornoWS.jsonRetorno);
                        sucesso = false;
                    }
                    else
                    {
                        // Já existe um motorista com este CPF
                        if (retorno.Errors != null && retorno?.Errors?.Where(x => x.ErrorCode == 271).FirstOrDefault() != null)
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
                                string mensagemRetorno = "Falha ao efetuar o cadastro do motorista:";

                                if (retorno.Errors != null && retorno.Errors.Count() > 0)
                                    retorno.Errors.ForEach(x => mensagemRetorno += System.Environment.NewLine + x.Message);
                                else
                                    mensagemRetorno += " Ocorreu uma falha ao efetuar o cadastro do motorista.";

                                mensagemErro = mensagemRetorno;
                                sucesso = false;
                            }
                        }
                    }
                }

                if (existeRepom && !sucesso)
                {
                    Servicos.Log.TratarErro($"Integração Repom Atualizar Motorista: {mensagemErro}");
                    return true;
                }

                return sucesso;
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);

                mensagemErro = "Ocorreu uma falha ao integrar os dados do motorista do CIOT.";
                return false;
            }
        }

        private envDriver ObterMotorista(Dominio.Entidades.Usuario motorista, Dominio.Entidades.Cliente contratado)
        {
            var retorno = new envDriver();

            retorno.Country = "Brazil";
            retorno.NationalId = motorista.CPF.PadLeft(11, '0');
            retorno.DriverLicenseNumber = !string.IsNullOrWhiteSpace(motorista.NumeroHabilitacao) ? motorista.NumeroHabilitacao.PadLeft(11, '0') : string.Empty;
            retorno.Address = GerarDriverAddress(motorista);
            retorno.Phones = GerarDriverPhones(motorista);
            retorno.DriverPersonalInformation = GerarDriverPersonalInformation(motorista);

            return retorno;
        }

        private Address GerarDriverAddress(Dominio.Entidades.Usuario motorista)
        {
            Address retorno = new Address();

            retorno.Street = Utilidades.String.Left(motorista.Endereco, 30);
            retorno.Number = motorista.NumeroEndereco;
            retorno.Complement = motorista.Complemento;
            retorno.Neighborhood = motorista.Bairro;
            retorno.ZipCode = Utilidades.String.OnlyNumbers(motorista.CEP);

            return retorno;
        }

        private List<Phones> GerarDriverPhones(Dominio.Entidades.Usuario motorista)
        {
            List<Phones> retorno = null;
            bool bEncontrouPreferencial = false;

            if (!string.IsNullOrEmpty(motorista.Celular))
            {
                var retObterTelefoneRepomFrete = this.ObterTelefoneRepomFrete(motorista?.Localidade?.Pais?.Sigla, motorista.Celular);

                if (!string.IsNullOrEmpty(retObterTelefoneRepomFrete.prefixo_mais_numero))
                {
                    if (retorno == null)
                        retorno = new List<Phones>();

                    var celular = new Phones();
                    celular.AreaCode = retObterTelefoneRepomFrete.ddd;
                    celular.Number = retObterTelefoneRepomFrete.prefixo_mais_numero;
                    bEncontrouPreferencial = true;
                    celular.Preferential = true;
                    celular.TypeId = "Cell";
                    retorno.Add(celular);
                }
            }

            if (!string.IsNullOrEmpty(motorista.Telefone))
            {
                var retObterTelefoneRepomFrete = this.ObterTelefoneRepomFrete(motorista?.Localidade?.Pais?.Sigla, motorista.Telefone);

                if (!string.IsNullOrEmpty(retObterTelefoneRepomFrete.prefixo_mais_numero))
                {
                    if (retorno == null)
                        retorno = new List<Phones>();

                    var fixo = new Phones();
                    fixo.AreaCode = retObterTelefoneRepomFrete.ddd;
                    fixo.Number = retObterTelefoneRepomFrete.prefixo_mais_numero;
                    if (bEncontrouPreferencial)
                        fixo.Preferential = false;
                    else
                        fixo.Preferential = true;
                    fixo.TypeId = "Fixed";
                    retorno.Add(fixo);
                }
            }

            return retorno;
        }

        private DriverPersonalInformation GerarDriverPersonalInformation(Dominio.Entidades.Usuario motorista)
        {
            DriverPersonalInformation retorno = new DriverPersonalInformation();

            retorno.Name = motorista.Nome.Left(50);
            //"BirthDate": "2021-07-06T14:53:13.664Z",
            retorno.BirthDate = motorista.DataNascimento != null ? ((DateTime)motorista.DataNascimento).ToString("u", new System.Globalization.CultureInfo("pt-BR")).Replace(" ", "T") : null;
            retorno.Gender = motorista.Sexo == Dominio.ObjetosDeValor.Enumerador.Sexo.Feminino ? "Male" : "Female";

            return retorno;
        }

        #endregion
    }
}
