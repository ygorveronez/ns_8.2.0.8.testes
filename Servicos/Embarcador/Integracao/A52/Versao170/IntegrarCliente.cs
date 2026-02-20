using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Dominio.ObjetosDeValor.Embarcador.Abastecimento;
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

        private bool IntegrarCliente(Dominio.Entidades.Cliente cliente, Dominio.Entidades.Embarcador.Cargas.CargaCargaIntegracao cargaIntegracao, out int idCliente, out int idClienteEndereco, out string mensagemErro, List<LogIntegracao> logs = null)
        {
            mensagemErro = null;
            idCliente = 0;
            idClienteEndereco = 0;
            string mensagemLog = string.Empty;
            try
            {
                if (cliente == null)
                    return true;

                bool sucesso = false;

                object envioWS = ObterCliente(cliente);

                //Transmite o arquivo
                retornoWebService retornoWS = this.TransmitirRepom(enumTipoWS.POST, envioWS, "clientes-fornecedores", this.tokenAutenticacao);

                if (retornoWS.erro && !string.IsNullOrEmpty(retornoWS.jsonRetorno))
                {
                    retError retorno = null;

                    try
                    {
                        retorno = retornoWS.jsonRetorno.FromJson<retError>();
                    }
                    catch (Exception ex)
                    {
                        Servicos.Log.TratarErro($"[Arquitetura-CatchNoAction] Erro ao desserializar resposta de cadastro de cliente A52: {ex.ToString()}", "CatchNoAction");
                    }

                    if (retorno == null)
                    {
                        mensagemErro = string.Format("Message: Ocorreu uma falha ao efetuar o cadastro do cliente; RetornoWS {0}.", retornoWS.jsonRetorno);
                        sucesso = false;
                    }
                    else
                    {
                        string mensagem = null;
                        retCliente retCliente = null;

                        int count = 0;
                        foreach (object message in retorno.message)
                        {
                            count++;

                            if (count == 1)
                                mensagem = message.ToString();
                            else if (count == 2 && retorno.statusCode == "409")
                                retCliente = message.ToString().FromJson<retCliente>();
                        }

                        if (string.IsNullOrEmpty(mensagem))
                        {
                            mensagemErro = "Ocorreu uma falha ao efetuar o cadastro do cliente.";
                            sucesso = false;
                        }
                        else if (retCliente == null)
                        {
                            mensagemErro = string.Format("Message: Ocorreu uma falha ao efetuar o cadastro do cliente; RetornoWS {0}.", mensagem);
                            sucesso = false;
                        }
                        else
                        {
                            idCliente = (int)retCliente.id;
                            idClienteEndereco = retCliente.enderecos?.FirstOrDefault()?.id ?? 0;
                            sucesso = true;
                        }
                    }
                }
                else if (retornoWS.erro)
                {
                    mensagemErro = "Ocorreu uma falha ao efetuar o cadastro do cliente.";
                    mensagemLog = mensagemErro;
                    sucesso = false;
                }
                else
                {
                    retCliente retCliente = retornoWS.jsonRetorno.ToString().FromJson<retCliente>();
                    idCliente = (int)retCliente.id;
                    idClienteEndereco = retCliente.enderecos?.FirstOrDefault()?.id ?? 0;
                    sucesso = true;
                    mensagemLog = $"IntegrarCliente: Cliente Integrado com sucesso";
                }

                logs?.Add(new LogIntegracao
                {
                    NomeEtapa = $"IntegrarCliente - {cliente.Nome ?? cliente.Codigo.ToString()}",
                    JsonEnvio = retornoWS.jsonEnvio,
                    JsonRetorno = retornoWS.jsonRetorno
                });

                if (cargaIntegracao != null)
                {
                    
                    SalvarArquivosIntegracao(cargaIntegracao, retornoWS.jsonEnvio, retornoWS.jsonRetorno, mensagemLog);
                }
               
                return sucesso;
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);

                mensagemErro = "Ocorreu uma falha ao integrar os dados do cliente.";
                return false;
            }
        }

        private envCliente ObterCliente(Dominio.Entidades.Cliente cliente)
        {
            envCliente retorno = new envCliente();

            retorno.cnpj = cliente.CPF_CNPJ_SemFormato;
            retorno.razaoSocial = cliente.Nome;
            retorno.nomeFantasia = cliente.NomeFantasia;
            retorno.identificador = null;
            retorno.sigla = null;
            retorno.ativo = true;
            retorno.enderecos = this.ObterClienteEndereco(cliente);

            return retorno;
        }

        private List<envClienteEndereco> ObterClienteEndereco(Dominio.Entidades.Cliente cliente)
        {
            List<envClienteEndereco> retorno = new List<envClienteEndereco>();
            envClienteEndereco endereco = new envClienteEndereco();
            endereco.tipo = 3;
            endereco.logradouro = cliente.Endereco;
            endereco.cep = Utilidades.String.OnlyNumbers(cliente.CEP);
            endereco.numero = cliente.Numero;
            endereco.bairro = cliente.Bairro;
            endereco.complemento = cliente.Complemento;
            endereco.idCidade = cliente.Localidade.CodigoIBGE;
            endereco.idPais = cliente.Localidade?.Pais?.Codigo ?? 0;
            endereco.poligono = new List<object> { new object() };
            endereco.raio = new Raio
            {
                latitude = 0,
                longitude = 0,
                raio = 0
            };
            retorno.Add(endereco);
            return retorno;
        }

        #endregion Métodos Privados

    }
}
