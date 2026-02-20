using Infrastructure.Services.HttpClientFactory;
using Newtonsoft.Json;
using System;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace Servicos.Embarcador.Integracao.ATM
{
    public static class ATMRestIntegracao
    {

        public static void CancelarAverbacaoDocumento(Dominio.Entidades.Embarcador.Seguros.ApoliceSeguro apolice, Dominio.Entidades.AverbacaoCTe averbacao, ref int tentativas, Repositorio.UnitOfWork unitOfWork, string stringConexao)
        {
            Repositorio.Embarcador.Seguros.AverbacaoATM repAverbacaoATM = new Repositorio.Embarcador.Seguros.AverbacaoATM(unitOfWork);
            Repositorio.AverbacaoCTe repAverbacaoCTe = new Repositorio.AverbacaoCTe(unitOfWork);
            Repositorio.Embarcador.Averbacao.AverbacaoIntegracaoArquivo repAverbacaoIntegracaoArquivo = new Repositorio.Embarcador.Averbacao.AverbacaoIntegracaoArquivo(unitOfWork);
            Repositorio.Embarcador.Cargas.CargaPedido repCargaPedido = new Repositorio.Embarcador.Cargas.CargaPedido(unitOfWork);

            Dominio.Entidades.Embarcador.Seguros.AverbacaoATM averbacaoATM = repAverbacaoATM.BuscarPorApolice(apolice.Codigo);

            string token = Logar(apolice, unitOfWork);
            string xml = Servicos.Embarcador.Integracao.ATM.ATMIntegracao.ObterXMLCancelamento(averbacao.CTe, averbacaoATM.AverbaComoEmbarcador, averbacao.Protocolo, unitOfWork);

            try
            {
                if (averbacao.CTe != null && averbacao.CTe.OcorreuSinistroAvaria)
                {
                    averbacao.CodigoRetorno = "";
                    averbacao.MensagemRetorno = "Conhecimento com sinitro/avaria registrado, não é possível cancelar a sua averbação.";
                    averbacao.Status = Dominio.Enumeradores.StatusAverbacaoCTe.Sucesso;
                    averbacao.Tipo = Dominio.Enumeradores.TipoAverbacaoCTe.Cancelamento;
                }
                else
                {
                    string urlAPI = "http://webserver.averba.com.br/rest/CTE";

                    HttpClient client = HttpClientFactoryWrapper.GetClient(nameof(ATMRestIntegracao));
                    client.BaseAddress = new Uri(urlAPI);
                    client.DefaultRequestHeaders.Accept.Clear();
                    client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/xml"));
                    client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

                    string jsonRequest = string.Empty;
                    string jsonResponse = string.Empty;
                    string jsonPost = xml;

                    var content = new StringContent(jsonPost.ToString(), Encoding.UTF8, "application/xml");
                    content.Headers.ContentType = new MediaTypeHeaderValue("application/xml");

                    jsonRequest = jsonPost.ToString();
                    var result = client.PostAsync(urlAPI, content).Result;
                    jsonResponse = result.Content.ReadAsStringAsync().Result;

                    if (result.IsSuccessStatusCode)
                    {
                        IntegracaoATM.Retorno retorno = new IntegracaoATM.Retorno();
                        XmlSerializer serializer = new XmlSerializer(typeof(IntegracaoATM.Retorno));
                        using (TextReader reader = new StringReader((string)result.Content.ReadAsStringAsync().Result))
                            retorno = (IntegracaoATM.Retorno)serializer.Deserialize(reader);

                        if (retorno.Averbado != null)
                        {
                            averbacao.MensagemRetorno = "Cancelado com sucesso.";
                            averbacao.Status = Dominio.Enumeradores.StatusAverbacaoCTe.Cancelado;
                            averbacao.Tipo = Dominio.Enumeradores.TipoAverbacaoCTe.Cancelamento;
                        }
                        else
                        {
                            averbacao.CodigoRetorno = retorno.Erros.FirstOrDefault().Codigo;
                            averbacao.MensagemRetorno = retorno.Erros.FirstOrDefault().Descricao;
                            averbacao.Status = Dominio.Enumeradores.StatusAverbacaoCTe.Sucesso;
                            averbacao.Tipo = Dominio.Enumeradores.TipoAverbacaoCTe.Cancelamento;
                        }
                    }
                    else
                    {
                        averbacao.CodigoRetorno = "999";
                        averbacao.MensagemRetorno = "O Serviço da AT&M não está disponível no momento.";
                        averbacao.Status = Dominio.Enumeradores.StatusAverbacaoCTe.Sucesso;
                        averbacao.Tipo = Dominio.Enumeradores.TipoAverbacaoCTe.Cancelamento;                        
                    }
                    Dominio.Entidades.Embarcador.Averbacao.AverbacaoIntegracaoArquivo averbacaoIntegracaoArquivo = new Dominio.Entidades.Embarcador.Averbacao.AverbacaoIntegracaoArquivo();
                    averbacaoIntegracaoArquivo.ArquivoRequisicao = Servicos.Embarcador.Integracao.ArquivoIntegracao.SalvarArquivoIntegracao(jsonRequest, "xml", unitOfWork);
                    averbacaoIntegracaoArquivo.ArquivoResposta = Servicos.Embarcador.Integracao.ArquivoIntegracao.SalvarArquivoIntegracao(jsonResponse, "xml", unitOfWork);
                    averbacaoIntegracaoArquivo.Data = DateTime.Now;
                    averbacaoIntegracaoArquivo.Mensagem = averbacao.CodigoRetorno + averbacao.MensagemRetorno;
                    averbacaoIntegracaoArquivo.Tipo = Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoArquivoIntegracaoCTeCarga.EnvioParaProcessamento;
                    repAverbacaoIntegracaoArquivo.Inserir(averbacaoIntegracaoArquivo);
                    averbacao.ArquivosTransacaoCancelamento.Add(averbacaoIntegracaoArquivo);
                }

                tentativas = 0;
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);

                if (averbacao.tentativasIntegracao >= 1)
                {
                    averbacao.CodigoRetorno = "999";
                    averbacao.MensagemRetorno = "O Serviço da AT&M não está disponível no momento.";
                    averbacao.Status = Dominio.Enumeradores.StatusAverbacaoCTe.Sucesso;
                    averbacao.Tipo = Dominio.Enumeradores.TipoAverbacaoCTe.Cancelamento;
                    averbacao.tentativasIntegracao = 0;
                }
                else
                {
                    averbacao.tentativasIntegracao++;
                    tentativas++;
                }
            }

            averbacao.DataRetorno = DateTime.Now;
            repAverbacaoCTe.Atualizar(averbacao);

            if (averbacao.Status == Dominio.Enumeradores.StatusAverbacaoCTe.Cancelado && averbacao.Tipo == Dominio.Enumeradores.TipoAverbacaoCTe.Cancelamento && averbacao.Carga != null && repCargaPedido.ContemProvedorOS(averbacao.Carga.Codigo))
            {
                int codigoAverbacao = averbacao.Codigo;
                string protocolo = averbacao.Protocolo;
                string numeroAverbacao = averbacao.Averbacao;

                Task.Run(() => Servicos.Embarcador.Integracao.ATM.ATMIntegracao.EnviarEmailAverbacao(codigoAverbacao, protocolo, numeroAverbacao, stringConexao, true));
            }
        }

        public static void AverbarDocumento(Dominio.Entidades.Embarcador.Seguros.ApoliceSeguro apolice, Dominio.Entidades.AverbacaoCTe averbacao, ref int tentativas, Repositorio.UnitOfWork unitOfWork, string stringConexao)
        {
            Repositorio.Embarcador.Seguros.AverbacaoATM repAverbacaoATM = new Repositorio.Embarcador.Seguros.AverbacaoATM(unitOfWork);
            Repositorio.AverbacaoCTe repAverbacaoCTe = new Repositorio.AverbacaoCTe(unitOfWork);
            Repositorio.Embarcador.Averbacao.AverbacaoIntegracaoArquivo repAverbacaoIntegracaoArquivo = new Repositorio.Embarcador.Averbacao.AverbacaoIntegracaoArquivo(unitOfWork);
            Repositorio.Embarcador.Cargas.CargaPedido repCargaPedido = new Repositorio.Embarcador.Cargas.CargaPedido(unitOfWork);
            Repositorio.Embarcador.Cargas.TipoIntegracao repTipoIntegracao = new Repositorio.Embarcador.Cargas.TipoIntegracao(unitOfWork);

            Dominio.Entidades.Embarcador.Seguros.AverbacaoATM averbacaoATM = repAverbacaoATM.BuscarPorApolice(apolice.Codigo);
            string token = Logar(apolice, unitOfWork);
            string xml = Servicos.Embarcador.Integracao.ATM.ATMIntegracao.ObterXMLAutorizacaoATM(averbacao.CTe, averbacaoATM.AverbaComoEmbarcador, unitOfWork, averbacao.Forma == Dominio.Enumeradores.FormaAverbacaoCTE.Provisoria, averbacaoATM);

            try
            {
                string urlAPI = "http://webserver.averba.com.br/rest/CTE";

                HttpClient client = HttpClientFactoryWrapper.GetClient(nameof(ATMRestIntegracao));
                client.BaseAddress = new Uri(urlAPI);
                client.DefaultRequestHeaders.Accept.Clear();
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/xml"));
                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

                string jsonRequest = string.Empty;
                string jsonResponse = string.Empty;
                string jsonPost = xml;

                var content = new StringContent(jsonPost.ToString(), Encoding.UTF8, "application/xml");
                content.Headers.ContentType = new MediaTypeHeaderValue("application/xml");

                jsonRequest = jsonPost.ToString();
                var result = client.PostAsync(urlAPI, content).Result;
                jsonResponse = result.Content.ReadAsStringAsync().Result;

                if (result.IsSuccessStatusCode)
                {
                    IntegracaoATM.Retorno retorno = new IntegracaoATM.Retorno();
                    XmlSerializer serializer = new XmlSerializer(typeof(IntegracaoATM.Retorno));
                    using (TextReader reader = new StringReader((string)result.Content.ReadAsStringAsync().Result))
                        retorno = (IntegracaoATM.Retorno)serializer.Deserialize(reader);

                    if (retorno.Averbado != null)
                    {
                        averbacao.Protocolo = retorno.Averbado.Protocolo;
                        IntegracaoATM.DadosSeguro[] dadosSeguro = retorno.Averbado.DadosSeguro;
                        averbacao.Averbacao = dadosSeguro?.FirstOrDefault()?.NumeroAverbacao ?? string.Empty;
                        averbacao.MensagemRetorno = "Averbado com sucesso.";
                        averbacao.Status = Dominio.Enumeradores.StatusAverbacaoCTe.Sucesso;
                        averbacao.Tipo = Dominio.Enumeradores.TipoAverbacaoCTe.Autorizacao;
                    }
                    else
                    {
                        averbacao.CodigoRetorno = retorno.Erros.FirstOrDefault().Codigo;
                        averbacao.MensagemRetorno = retorno.Erros.FirstOrDefault().Descricao;
                        averbacao.Status = Dominio.Enumeradores.StatusAverbacaoCTe.Rejeicao;
                        averbacao.Tipo = Dominio.Enumeradores.TipoAverbacaoCTe.Autorizacao;

                        if (repTipoIntegracao.BuscarPorTipo(Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoIntegracao.CargoX) != null)
                            Servicos.Embarcador.Integracao.CargoX.IntegracaoCargoX.IntegrarSituacaoDiversas(averbacao.Carga?.CodigoCargaEmbarcador, "warning", 9999, retorno.Erros.FirstOrDefault().Descricao, "Averbação ATM", unitOfWork);
                    }
                }
                else
                {
                    averbacao.CodigoRetorno = "999";
                    averbacao.MensagemRetorno = "O Serviço da AT&M não está disponível no momento.";
                    averbacao.Status = Dominio.Enumeradores.StatusAverbacaoCTe.Rejeicao;
                    averbacao.Tipo = Dominio.Enumeradores.TipoAverbacaoCTe.Autorizacao;
                    averbacao.tentativasIntegracao = 0;

                    if (repTipoIntegracao.BuscarPorTipo(Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoIntegracao.CargoX) != null)
                        Servicos.Embarcador.Integracao.CargoX.IntegracaoCargoX.IntegrarSituacaoDiversas(averbacao.Carga?.CodigoCargaEmbarcador, "warning", 9999, "Serviço ATM não disponível.", "Averbação ATM", unitOfWork);
                }

                Dominio.Entidades.Embarcador.Averbacao.AverbacaoIntegracaoArquivo averbacaoIntegracaoArquivo = new Dominio.Entidades.Embarcador.Averbacao.AverbacaoIntegracaoArquivo();
                averbacaoIntegracaoArquivo.ArquivoRequisicao = Servicos.Embarcador.Integracao.ArquivoIntegracao.SalvarArquivoIntegracao(jsonRequest, ".xml", unitOfWork);
                averbacaoIntegracaoArquivo.ArquivoResposta = Servicos.Embarcador.Integracao.ArquivoIntegracao.SalvarArquivoIntegracao(jsonResponse, ".xml", unitOfWork);
                averbacaoIntegracaoArquivo.Data = DateTime.Now;
                averbacaoIntegracaoArquivo.Mensagem = averbacao.CodigoRetorno + averbacao.MensagemRetorno;
                averbacaoIntegracaoArquivo.Tipo = Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoArquivoIntegracaoCTeCarga.EnvioParaProcessamento;
                repAverbacaoIntegracaoArquivo.Inserir(averbacaoIntegracaoArquivo);
                averbacao.ArquivosTransacao.Add(averbacaoIntegracaoArquivo);
                tentativas = 0;
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);

                if (averbacao.tentativasIntegracao >= 1)
                {
                    averbacao.CodigoRetorno = "999";
                    averbacao.MensagemRetorno = "O Serviço da AT&M não está disponível no momento.";
                    averbacao.Status = Dominio.Enumeradores.StatusAverbacaoCTe.Rejeicao;
                    averbacao.Tipo = Dominio.Enumeradores.TipoAverbacaoCTe.Autorizacao;
                    averbacao.tentativasIntegracao = 0;

                    if (repTipoIntegracao.BuscarPorTipo(Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoIntegracao.CargoX) != null)
                        Servicos.Embarcador.Integracao.CargoX.IntegracaoCargoX.IntegrarSituacaoDiversas(averbacao.Carga?.CodigoCargaEmbarcador, "warning", 9999, "Serviço ATM não disponível.", "Averbação ATM", unitOfWork);
                }
                else
                {
                    averbacao.tentativasIntegracao++;
                    tentativas++;
                }
            }

            averbacao.DataRetorno = DateTime.Now;
            repAverbacaoCTe.Atualizar(averbacao);

            if (averbacao.Status == Dominio.Enumeradores.StatusAverbacaoCTe.Sucesso && averbacao.Tipo == Dominio.Enumeradores.TipoAverbacaoCTe.Autorizacao && averbacao.Carga != null && repCargaPedido.ContemProvedorOS(averbacao.Carga.Codigo))
            {
                int codigoAverbacao = averbacao.Codigo;
                string protocolo = averbacao.Protocolo;
                string numeroAverbacao = averbacao.Averbacao;

                Task.Run(() => Servicos.Embarcador.Integracao.ATM.ATMIntegracao.EnviarEmailAverbacao(codigoAverbacao, protocolo, numeroAverbacao, stringConexao, false));
            }
        }

        private static string Logar(Dominio.Entidades.Embarcador.Seguros.ApoliceSeguro apolice, Repositorio.UnitOfWork unidadeDeTrabalho)
        {
            Repositorio.Embarcador.Seguros.AverbacaoATM repAverbacaoATM = new Repositorio.Embarcador.Seguros.AverbacaoATM(unidadeDeTrabalho);
            Dominio.Entidades.Embarcador.Seguros.AverbacaoATM averbacaoATM = repAverbacaoATM.BuscarPorApolice(apolice.Codigo);

            string token = string.Empty;

            if (averbacaoATM != null)
            {
                string urlAPI = "http://webserver.averba.com.br/rest/Auth";
                ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;

                HttpClient client = HttpClientFactoryWrapper.GetClient(nameof(ATMRestIntegracao));
                client.BaseAddress = new Uri(urlAPI);
                client.DefaultRequestHeaders.Accept.Clear();
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

                Dominio.ObjetosDeValor.Embarcador.ATM.Authenticate antenticar = new Dominio.ObjetosDeValor.Embarcador.ATM.Authenticate
                {
                    usuario = averbacaoATM.Usuario,
                    senha = averbacaoATM.Senha,
                    codigoatm = averbacaoATM.CodigoATM
                };

                string jsonPost = JsonConvert.SerializeObject(antenticar, Formatting.Indented);

                var content = new StringContent(jsonPost.ToString(), Encoding.UTF8, "application/json");
                content.Headers.ContentType = new MediaTypeHeaderValue("application/json");
                var result = client.PostAsync(urlAPI, content).Result;

                if (result.IsSuccessStatusCode)
                {
                    dynamic objRetorno = JsonConvert.DeserializeObject<dynamic>(result.Content.ReadAsStringAsync().Result);

                    token = (string)objRetorno.Bearer;
                }
                else
                {
                    Servicos.Log.TratarErro("Login: " + jsonPost, "ATMRestIntegracao");
                    Servicos.Log.TratarErro("Login retorno: Não autenticou", "ATMRestIntegracao");
                }
            }
            else
                Servicos.Log.TratarErro("Login: Não possui configuração para logar", "ATMRestIntegracao");

            return token;

        }
    }
}
