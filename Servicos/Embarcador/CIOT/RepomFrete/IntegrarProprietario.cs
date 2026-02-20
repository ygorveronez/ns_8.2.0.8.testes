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

        private bool IntegrarProprietario(Dominio.Entidades.Cliente proprietario, Dominio.Entidades.Embarcador.Pessoas.ModalidadeTransportadoraPessoas modalidade, Repositorio.UnitOfWork unitOfWork, out string mensagemErro, out string jsonEnvio, out string jsonRetorno)
        {
            jsonEnvio = string.Empty;
            jsonRetorno = string.Empty;

            mensagemErro = null;

            if (proprietario == null)
                return true;

            if (modalidade == null)
            {
                mensagemErro = "A modalidade do transportador não está configurada.";
                return false;
            }

            try
            {
                #region Verificar motorista existe Repom
                bool existeRepom = false;
                bool sucesso = false;
                string nationalID = proprietario.CPF_CNPJ_SemFormato;
                retornoWebService retornoWS = null;

                retornoWS = this.TransmitirRepom(enumTipoWS.GET, null, $"Hired/ByDocument/Brazil/{nationalID}", this.tokenAutenticacao);

                if (!retornoWS.erro && !string.IsNullOrEmpty(retornoWS.jsonRetorno))
                    existeRepom = true;

                #endregion

                var envioWS = ObterContratado(proprietario, modalidade, unitOfWork);

                //Transmite o arquivo
                if (existeRepom)
                    retornoWS = this.TransmitirRepom(enumTipoWS.PUT, envioWS, $"Hired/Brazil/{nationalID}", this.tokenAutenticacao);
                else
                    retornoWS = this.TransmitirRepom(enumTipoWS.POST, envioWS, "Hired", this.tokenAutenticacao);

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
                        Servicos.Log.TratarErro($"[Arquitetura-CatchNoAction] Erro ao processar JSON de retorno do webservice RepomFrete - IntegrarProprietario: {ex.ToString()}", "CatchNoAction");
                    }

                    if (retorno == null)
                    {
                        mensagemErro = string.Format("Message: Ocorreu uma falha ao efetuar o cadastro do proprietário; RetornoWS {0}.", retornoWS.jsonRetorno);
                        sucesso = false;
                    }
                    else
                    {
                        if (retorno?.Response?.StatusCode == 200 || retorno?.Response?.StatusCode == 201)
                        {
                            sucesso = true;
                        }
                        else
                        {
                            // Prestador já existente.
                            if (retorno.Errors != null && retorno?.Errors?.Where(x => x.ErrorCode == 336).FirstOrDefault() != null)
                            {
                                sucesso = true;
                            }
                            else
                            {
                                string mensagemRetorno = "Falha ao efetuar cadastro o proprietário:";

                                if (retorno.Errors != null && retorno.Errors.Count() > 0)
                                    retorno.Errors.ForEach(x => mensagemRetorno += System.Environment.NewLine + x.Message);
                                else
                                    mensagemRetorno += " Ocorreu uma falha ao efetuar o cadastro do contratado.";

                                mensagemErro = mensagemRetorno;
                                sucesso = false;
                            }
                        }
                    }
                }

                if (existeRepom && !sucesso)
                {
                    Servicos.Log.TratarErro($"Integração Repom Atualizar Proprietário: {mensagemErro}");
                    return true;
                }

                return sucesso;
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);

                mensagemErro = "Ocorreu uma falha ao integrar os dados do proprietário do CIOT.";
                return false;
            }
        }

        private Dominio.ObjetosDeValor.Embarcador.CIOT.RepomFrete.envHired ObterContratado(Dominio.Entidades.Cliente cliente, Dominio.Entidades.Embarcador.Pessoas.ModalidadeTransportadoraPessoas modalidade, Repositorio.UnitOfWork unitOfWork)
        {
            var retorno = new envHired();

            Repositorio.Cliente repCliente = new Repositorio.Cliente(unitOfWork);
            Dominio.Entidades.Cliente contratado = repCliente.BuscarPorCPFCNPJ(cliente.CPF_CNPJ);

            retorno.Country = "Brazil";

            if (contratado.Tipo == "F")
                retorno.HiredType = "Person";
            else if (contratado.Tipo == "J")
                retorno.HiredType = "Company";

            retorno.NationalId = contratado.CPF_CNPJ_SemFormato;
            retorno.Email = contratado.Email;
            retorno.Phones = this.GerarHiredPhones(contratado);
            retorno.BrazilianSettings = this.GerarHiredBrazilianSettings(contratado, modalidade);
            retorno.Address = this.GerarHiredAddress(contratado);

            if (contratado.Tipo == "J")
                retorno.CompanyInformation = this.GerarHiredCompanyInformation(contratado);
            else if (contratado.Tipo == "F")
                retorno.HiredPersonalInformation = this.GerarHiredPersonalInformation(contratado);

            retorno.FuelVoucherPercentage = null;

            return retorno;
        }

        private List<Phones> GerarHiredPhones(Dominio.Entidades.Cliente contratado)
        {
            List<Phones> retorno = null;
            bool bEncontrouPreferencial = false;

            if (!string.IsNullOrEmpty(contratado.Celular))
            {
                var retObterTelefoneRepomFrete = this.ObterTelefoneRepomFrete(contratado?.Pais?.Sigla, contratado.Celular);

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

            if (!string.IsNullOrEmpty(contratado.Telefone1))
            {
                var retObterTelefoneRepomFrete = this.ObterTelefoneRepomFrete(contratado?.Pais?.Sigla, contratado.Telefone1);

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

        private HiredBrazilianSettings GerarHiredBrazilianSettings(Dominio.Entidades.Cliente contratado, Dominio.Entidades.Embarcador.Pessoas.ModalidadeTransportadoraPessoas modalidade)
        {
            HiredBrazilianSettings retorno = new HiredBrazilianSettings();

            retorno.RNTRC = string.IsNullOrEmpty(modalidade.RNTRC) ? null : modalidade.RNTRC.PadLeft(9, '0');

            if (contratado.Tipo == "J")
                retorno.HiredPessoaJuridica = GerarHiredPessoaJuridica(contratado);
            else if (contratado.Tipo == "F")
                retorno.HiredPessoaFisica = GerarHiredPessoaFisica(contratado, modalidade);

            return retorno;
        }

        private HiredPessoaJuridica GerarHiredPessoaJuridica(Dominio.Entidades.Cliente contratado)
        {
            HiredPessoaJuridica retorno = new HiredPessoaJuridica();

            retorno.NomeFantasia = contratado.NomeFantasia.Left(50);
            retorno.InscricaoEstadual = contratado.InscricaoST;
            retorno.InscricaoMunicipal = contratado.InscricaoMunicipal;
            retorno.OptanteSimplesNacional = contratado.Tipo == "F" ? false : contratado.RegimeTributario == Dominio.ObjetosDeValor.Embarcador.Enumeradores.RegimeTributario.SimplesNacional ? true : false;

            return retorno;
        }

        private HiredPessoaFisica GerarHiredPessoaFisica(Dominio.Entidades.Cliente contratado, Dominio.Entidades.Embarcador.Pessoas.ModalidadeTransportadoraPessoas modalidade)
        {
            HiredPessoaFisica retorno = new HiredPessoaFisica();

            retorno.INSS = modalidade.CodigoINSS;
            retorno.RG = contratado.RG_Passaporte;

            return retorno;
        }

        private Address GerarHiredAddress(Dominio.Entidades.Cliente contratado)
        {
            Address retorno = new Address();

            retorno.Street = contratado.Endereco;
            retorno.Number = contratado.Numero;
            retorno.Complement = contratado.Complemento;
            retorno.Neighborhood = Utilidades.String.Left(contratado.Bairro, 20);
            retorno.ZipCode = Utilidades.String.OnlyNumbers(contratado.CEP);

            return retorno;
        }

        private CompanyInformation GerarHiredCompanyInformation(Dominio.Entidades.Cliente contratado)
        {
            CompanyInformation retorno = new CompanyInformation(); ;
            retorno.CompanyName = contratado.Nome.Left(50);
            return retorno;
        }

        private HiredPersonalInformation GerarHiredPersonalInformation(Dominio.Entidades.Cliente contratado)
        {
            HiredPersonalInformation retorno = new HiredPersonalInformation();

            retorno.Name = contratado.Nome;
            //"BirthDate": "2021-07-06T14:53:13.664Z",
            retorno.BirthDate = contratado.DataNascimento != null ? ((DateTime)contratado.DataNascimento).ToString("u", new System.Globalization.CultureInfo("pt-BR")).Replace(" ", "T") : null;
            retorno.LegalDependents = 0;
            retorno.Gender = contratado.Sexo == Dominio.ObjetosDeValor.Enumerador.Sexo.Masculino ? "Male" : "Female";

            return retorno;
        }

        #endregion
    }
}

