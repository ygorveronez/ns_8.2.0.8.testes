using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Dominio.ObjetosDeValor.Embarcador.Integracao.A52.V170;
using Utilidades.Extensions;
using static Google.Apis.Requests.BatchRequest;

namespace Servicos.Embarcador.Integracao.A52.V170
{
    public partial class IntegracaoA52
    {

        #region Métodos Públicos

        #endregion Métodos Públicos

        #region Métodos Privados

        private bool IntegrarMotoristas(List<Dominio.Entidades.Usuario> motoristas, Dominio.Entidades.Embarcador.Cargas.CargaCargaIntegracao cargaIntegracao, out List<int> idMotoristas, out string mensagemErro, List<LogIntegracao> logs = null)
        {
            mensagemErro = null;
            idMotoristas = new List<int>();

            foreach (Dominio.Entidades.Usuario motorista in motoristas)
            {
                int idMotorista = 0;
                if (!IntegrarMotorista(motorista, cargaIntegracao, out idMotorista, out mensagemErro, logs))
                    return false;
                else
                    idMotoristas.Add(idMotorista);
            }

            return true;
        }

        private bool IntegrarMotorista(Dominio.Entidades.Usuario motorista, Dominio.Entidades.Embarcador.Cargas.CargaCargaIntegracao cargaIntegracao, out int idMotorista, out string mensagemErro, List<LogIntegracao> logs = null)
        {
            mensagemErro = null;
            idMotorista = 0;
            string mensagemLog = string.Empty;
            try
            {
                bool sucesso = false;

                object envioWS = ObterMotorista(motorista);

                //Transmite o arquivo
                retornoWebService retornoWS = this.TransmitirRepom(enumTipoWS.POST, envioWS, "motoristas", this.tokenAutenticacao);

                if (retornoWS.erro && !string.IsNullOrEmpty(retornoWS.jsonRetorno))
                {
                    retError retorno = null;

                    try
                    {
                        retorno = retornoWS.jsonRetorno.FromJson<retError>();
                    }
                    catch (Exception ex)
                    {
                        Servicos.Log.TratarErro($"[Arquitetura-CatchNoAction] Erro ao desserializar resposta de cadastro de motorista A52: {ex.ToString()}", "CatchNoAction");
                    }

                    if (retorno == null)
                    {
                        mensagemErro = string.Format("Message: Ocorreu uma falha ao efetuar o cadastro do motorista; RetornoWS {0}.", retornoWS.jsonRetorno);
                        sucesso = false;
                    }
                    else
                    {
                        string mensagem = null;
                        retMotorista retMotorista = null;

                        int count = 0;
                        foreach (object message in retorno.message)
                        {
                            count++;

                            if (count == 1)
                                mensagem = message.ToString();
                            else if (count == 2 && retorno.statusCode == "409")
                                retMotorista = message.ToString().FromJson<retMotorista>();
                        }

                        if (string.IsNullOrEmpty(mensagem))
                        {
                            mensagemErro = "Ocorreu uma falha ao efetuar o cadastro do motorista.";
                            sucesso = false;
                        }
                        else if (retMotorista == null)
                        {
                            mensagemErro = string.Format("Message: Ocorreu uma falha ao efetuar o cadastro do motorista; RetornoWS {0}.", mensagem);
                            sucesso = false;
                        }
                        else
                        {
                            idMotorista = (int)retMotorista.id;
                            sucesso = true;
                        }
                    }
                }
                else if (retornoWS.erro)
                {
                    mensagemErro = "Ocorreu uma falha ao efetuar o cadastro motorista.";
                    mensagemLog = mensagemErro;
                    sucesso = false;
                }
                else
                {
                    retMotorista retMotorista = retornoWS.jsonRetorno.ToString().FromJson<retMotorista>();
                    idMotorista = (int)retMotorista.id;
                    mensagemLog = "IntegrarMototrista: Motorista integrado com sucesso.";
                    sucesso = true;
                }
                logs?.Add(new LogIntegracao
                {
                    NomeEtapa = $"IntegrarMototrista - {motorista.Nome ?? motorista.Codigo.ToString()}",
                    JsonEnvio = retornoWS.jsonEnvio,
                    JsonRetorno = retornoWS.jsonRetorno
                });

                
                SalvarArquivosIntegracao(cargaIntegracao, retornoWS.jsonEnvio, retornoWS.jsonRetorno, mensagemLog);

                return sucesso;
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);

                mensagemErro = "Ocorreu uma falha ao integrar os dados do motorista.";
                return false;
            }
        }

        private envMotorista ObterMotorista(Dominio.Entidades.Usuario motorista)
        {
            envMotorista retorno = new envMotorista();

            retorno.name = motorista.Nome;
            retorno.vinculo = motorista.TipoMotorista == Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoMotorista.Terceiro ? 2 : 1;
            retorno.matricula = motorista.NumeroMatricula;
            retorno.idCargo = null;
            retorno.celular = motorista.Celular;
            retorno.telefone = motorista.Telefone;
            retorno.dataNascimento = motorista.DataNascimento != null ? ((DateTime)motorista.DataNascimento).ToString("u", new System.Globalization.CultureInfo("pt-BR")).Replace(" ", "T") : null;
            retorno.dataAdmissao = motorista.DataAdmissao != null ? ((DateTime)motorista.DataAdmissao).ToString("u", new System.Globalization.CultureInfo("pt-BR")).Replace(" ", "T") : null;
            retorno.dataDesligamento = motorista.DataDemissao != null ? ((DateTime)motorista.DataDemissao).ToString("u", new System.Globalization.CultureInfo("pt-BR")).Replace(" ", "T") : null;
            retorno.cursoMopp = motorista.DataVencimentoMoop != null ? true : false;
            retorno.validadeMopp = motorista.DataVencimentoMoop != null ? ((DateTime)motorista.DataVencimentoMoop).ToString("u", new System.Globalization.CultureInfo("pt-BR")).Replace(" ", "T") : null; ;
            retorno.dataValidadeToxicologico = null;
            retorno.documentos = this.ObterMotoristaDocumento(motorista);
            retorno.enderecos = this.ObterMotoristaEndereco(motorista);
            retorno.ativo = true;

            return retorno;
        }

        private envMotoristaDocumento ObterMotoristaDocumento(Dominio.Entidades.Usuario motorista)
        {
            envMotoristaDocumento retorno = new envMotoristaDocumento();
            retorno.cpf = motorista.CPF.PadLeft(11, '0');
            retorno.cnh = !string.IsNullOrWhiteSpace(motorista.NumeroHabilitacao) ? motorista.NumeroHabilitacao.PadLeft(11, '0') : null;
            retorno.dataRenovacaoCnh = null;
            retorno.rg = motorista.RG;
            retorno.pis = motorista.PIS;
            retorno.ctps = null;
            return retorno;
        }

        private List<envMotoristaEndereco> ObterMotoristaEndereco(Dominio.Entidades.Usuario motorista)
        {
            List<envMotoristaEndereco> retorno = new List<envMotoristaEndereco>();
            envMotoristaEndereco endereco = new envMotoristaEndereco();
            endereco.logradouro = motorista.Endereco;
            endereco.cep = Utilidades.String.OnlyNumbers(motorista.CEP);
            endereco.numero = motorista.NumeroEndereco;
            endereco.bairro = motorista.Bairro;
            endereco.complemento = motorista.Complemento;
            endereco.idCidade = motorista.Localidade.CodigoIBGE;
            retorno.Add(endereco);
            return retorno;
        }

        #endregion Métodos Privados

    }
}
