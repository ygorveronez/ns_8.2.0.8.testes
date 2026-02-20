using Infrastructure.Services.HttpClientFactory;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading;

namespace Servicos
{
    public class LsTranslog : ServicoBase
    {

        public LsTranslog(Repositorio.UnitOfWork unitOfWork, CancellationToken cancelationToken = default) : base(unitOfWork, cancelationToken) { }
        public void SalvarCTeParaIntegracao(int codigoCTe, int codigoEmpresa, Repositorio.UnitOfWork unidadeDeTrabalho, bool enviarCTeSalvo = false)
        {
            try
            {
                Repositorio.ConhecimentoDeTransporteEletronico repCTe = new Repositorio.ConhecimentoDeTransporteEletronico(unidadeDeTrabalho);
                Repositorio.ConfiguracaoIntegracaoLsTranslog repConfiguracao = new Repositorio.ConfiguracaoIntegracaoLsTranslog(unidadeDeTrabalho);
                Repositorio.IntegracaoLsTranslog repIntegracao = new Repositorio.IntegracaoLsTranslog(unidadeDeTrabalho);

                Dominio.Entidades.ConfiguracaoIntegracaoLsTranslog configuracao = repConfiguracao.BuscaPorEmpresa(codigoEmpresa);
                if (configuracao != null && configuracao.Clientes != null && configuracao.Clientes.Count() > 0)
                {
                    Dominio.Entidades.ConhecimentoDeTransporteEletronico cte = repCTe.BuscarPorCodigo(codigoCTe);

                    if (cte != null && cte.Remetente != null)
                    {
                        if ((from o in configuracao.Clientes where (o.Cliente.CPF_CNPJ_SemFormato == cte.Remetente.CPF_CNPJ || o.Cliente.CPF_CNPJ_SemFormato == cte.TomadorPagador.CPF_CNPJ) select o).Count() > 0)
                        {
                            if (cte.Status == "A" || (enviarCTeSalvo && (from o in configuracao.Clientes where (o.Cliente.CPF_CNPJ_SemFormato == cte.Remetente.CPF_CNPJ || o.Cliente.CPF_CNPJ_SemFormato == cte.TomadorPagador.CPF_CNPJ) select o).FirstOrDefault().EnviarRetornoAoSalvarDocumento))
                            {

                                Dominio.Entidades.IntegracaoLsTranslog integracao = new Dominio.Entidades.IntegracaoLsTranslog();
                                integracao.Empresa = cte.Empresa;
                                integracao.CTe = cte;
                                integracao.StatusEnvio = Dominio.ObjetosDeValor.Enumerador.StatusIntegracaoLsTranslog.Pendente;
                                integracao.StatusConsulta = Dominio.ObjetosDeValor.Enumerador.StatusIntegracaoLsTranslog.Pendente;
                                integracao.Data = DateTime.Now;

                                repIntegracao.Inserir(integracao);
                            }
                        }
                    }
                }
            }
            catch (Exception e)
            {
                Servicos.Log.TratarErro("SalvarCTeParaIntegracao: Codigo CTe " + codigoCTe + ": " + e.Message, "LsTranslog");
            }
        }

        public void SalvarNFSeParaIntegracao(int codigoNFSe, int codigoEmpresa, Repositorio.UnitOfWork unidadeDeTrabalho, bool enviarNFSeSalvo = false)
        {
            try
            {
                Repositorio.NFSe repNFSe = new Repositorio.NFSe(unidadeDeTrabalho);
                Repositorio.ConfiguracaoIntegracaoLsTranslog repConfiguracao = new Repositorio.ConfiguracaoIntegracaoLsTranslog(unidadeDeTrabalho);
                Repositorio.IntegracaoLsTranslog repIntegracao = new Repositorio.IntegracaoLsTranslog(unidadeDeTrabalho);

                Dominio.Entidades.ConfiguracaoIntegracaoLsTranslog configuracao = repConfiguracao.BuscaPorEmpresa(codigoEmpresa);
                if (configuracao != null && configuracao.Clientes != null && configuracao.Clientes.Count() > 0)
                {
                    Dominio.Entidades.NFSe nfse = repNFSe.BuscarPorCodigo(codigoNFSe);

                    if (nfse != null && nfse.Tomador != null)
                    {
                        if ((from o in configuracao.Clientes where o.Cliente.CPF_CNPJ_SemFormato == nfse.Tomador.CPF_CNPJ select o).Count() > 0)
                        {
                            if (nfse.Status == Dominio.Enumeradores.StatusNFSe.Autorizado || (enviarNFSeSalvo && (from o in configuracao.Clientes where o.Cliente.CPF_CNPJ_SemFormato == nfse.Tomador.CPF_CNPJ select o).FirstOrDefault().EnviarRetornoAoSalvarDocumento))
                            {
                                Dominio.Entidades.IntegracaoLsTranslog integracao = new Dominio.Entidades.IntegracaoLsTranslog();
                                integracao.Empresa = nfse.Empresa;
                                integracao.NFSe = nfse;
                                integracao.StatusEnvio = Dominio.ObjetosDeValor.Enumerador.StatusIntegracaoLsTranslog.Pendente;
                                integracao.StatusConsulta = Dominio.ObjetosDeValor.Enumerador.StatusIntegracaoLsTranslog.Pendente;
                                integracao.Data = DateTime.Now;

                                repIntegracao.Inserir(integracao);
                            }
                        }
                    }
                }
            }
            catch (Exception e)
            {
                Servicos.Log.TratarErro("SalvarNFSeParaIntegracao: Codigo NFSe " + codigoNFSe + ": " + e.Message, "LsTranslog");
            }
        }

        public void SalvarNFeParaIntegracao(Dominio.Entidades.XMLNotaFiscalEletronica nfe, int codigoEmpresa, Repositorio.UnitOfWork unidadeDeTrabalho, bool enviarNFSeSalvo = false)
        {
            try
            {
                Repositorio.XMLNotaFiscalEletronica repXMLNotaFiscalEletronica = new Repositorio.XMLNotaFiscalEletronica(unidadeDeTrabalho);
                Repositorio.ConfiguracaoIntegracaoLsTranslog repConfiguracao = new Repositorio.ConfiguracaoIntegracaoLsTranslog(unidadeDeTrabalho);
                Repositorio.IntegracaoLsTranslog repIntegracao = new Repositorio.IntegracaoLsTranslog(unidadeDeTrabalho);

                Dominio.Entidades.ConfiguracaoIntegracaoLsTranslog configuracao = repConfiguracao.BuscaPorEmpresa(codigoEmpresa);
                if (configuracao != null && configuracao.Clientes != null && configuracao.Clientes.Count() > 0)
                {
                    Dominio.Entidades.Cliente cliente = nfe?.Emitente ?? null;
                    if (nfe != null && nfe.UtilizarContratanteComoTomador && nfe.Contratante != null)
                        cliente = nfe.Contratante;

                    if (cliente != null)
                    {
                        if ((from o in configuracao.Clientes where o.Cliente.CPF_CNPJ_SemFormato == cliente.CPF_CNPJ_SemFormato select o).Count() > 0)
                        {
                            Dominio.Entidades.IntegracaoLsTranslog integracao = new Dominio.Entidades.IntegracaoLsTranslog();
                            integracao.Empresa = nfe.Empresa;
                            integracao.NFe = nfe;
                            integracao.StatusEnvio = Dominio.ObjetosDeValor.Enumerador.StatusIntegracaoLsTranslog.Pendente;
                            integracao.StatusConsulta = Dominio.ObjetosDeValor.Enumerador.StatusIntegracaoLsTranslog.Pendente;
                            integracao.Data = DateTime.Now;

                            repIntegracao.Inserir(integracao);
                        }
                    }
                }
            }
            catch (Exception e)
            {
                Servicos.Log.TratarErro("SalvarNFeParaIntegracao: Codigo NFe " + nfe.Codigo + ": " + e.Message, "LsTranslog");
            }
        }

        public bool EnviarDocumento(int codigo, Repositorio.UnitOfWork unidadeDeTrabalho)
        {
            Repositorio.IntegracaoLsTranslog repIntegracao = new Repositorio.IntegracaoLsTranslog(unidadeDeTrabalho);
            Dominio.Entidades.IntegracaoLsTranslog integracao = repIntegracao.BuscaPorCodigo(codigo);
            try
            {
                string token = this.Logar(integracao.Empresa.Codigo, unidadeDeTrabalho);

                if (!string.IsNullOrWhiteSpace(token))
                {
                    if (integracao != null)
                    {
                        //Verificar Remetente para identificar origem dos dados para envio:
                        if ((integracao.CTe != null && integracao.CTe.Remetente.CPF_CNPJ.Substring(0, 8) == "71673990") || (integracao.NFSe != null && integracao.NFSe.Tomador.CPF_CNPJ.Substring(0, 8) == "71673990")) ////NATURA
                            return EnviarDocumentoNatura(integracao, token, unidadeDeTrabalho);
                        else if ((integracao.CTe != null && integracao.CTe.Remetente.CPF_CNPJ.Substring(0, 8) == "66970229") || (integracao.NFSe != null && integracao.NFSe.Tomador.CPF_CNPJ.Substring(0, 8) == "66970229")) ////NEXTEL
                            return EnviarDocumentoNextel(integracao, token, unidadeDeTrabalho);
                        else if ((integracao.CTe != null && integracao.CTe.Remetente.CPF_CNPJ.Substring(0, 8) == "07015691") || (integracao.NFSe != null && integracao.NFSe.Tomador.CPF_CNPJ.Substring(0, 8) == "07015691")) ////4Bio
                            return EnviarDocumento4Bio(integracao, token, unidadeDeTrabalho);
                        else if ((integracao.CTe != null && integracao.CTe.Remetente.CPF_CNPJ.Substring(0, 8) == "53153938") || (integracao.NFSe != null && integracao.NFSe.Tomador.CPF_CNPJ.Substring(0, 8) == "53153938")) ////COBASI
                            return EnviarDocumentoCobasi(integracao, token, unidadeDeTrabalho);
                        else if ((integracao.NFe != null && integracao.NFe.Emitente.CPF_CNPJ_SemFormato.Substring(0, 8) == "53153938")) ////COBASI
                            return EnviarDocumentoCobasi(integracao, token, unidadeDeTrabalho);
                        else if ((integracao.CTe != null && integracao.CTe.TomadorPagador.CPF_CNPJ.Substring(0, 8) == "19782476") || (integracao.NFe != null && integracao.NFe.Contratante.CPF_CNPJ_SemFormato.Substring(0, 8) == "19782476")) ////MANDAE
                            return EnviarDocumentoMandae(integracao, token, unidadeDeTrabalho);
                        else if ((integracao.CTe != null && integracao.CTe.TomadorPagador.CPF_CNPJ == "33200056034396") || (integracao.NFe != null && integracao.NFe.Contratante.CPF_CNPJ_SemFormato == "33200056034396")) ////RIACHUELO 33200056034396
                            return EnviarDocumentoRiachuelo(integracao, 3628, token, unidadeDeTrabalho);
                        else if ((integracao.CTe != null && integracao.CTe.TomadorPagador.CPF_CNPJ == "33200056036097") || (integracao.NFe != null && integracao.NFe.Contratante.CPF_CNPJ_SemFormato == "33200056036097")) ////RIACHUELO 33200056036097
                            return EnviarDocumentoRiachuelo(integracao, 2240, token, unidadeDeTrabalho);
                        else
                        {
                            Servicos.Log.TratarErro("EnviarDocumento: Codigo " + codigo + ": Cliente do documento não disponível para envio.", "LsTranslog");

                            integracao.ObservacaoEnvio = "EnviarDocumento: Cliente do documento não disponível para envio.";
                            integracao.StatusEnvio = Dominio.ObjetosDeValor.Enumerador.StatusIntegracaoLsTranslog.Erro;
                            repIntegracao.Atualizar(integracao);

                            return false;
                        }
                    }
                    else
                    {
                        Servicos.Log.TratarErro("EnviarDocumento: integração código " + codigo + " não encontrada.", "LsTranslog");

                        integracao.ObservacaoEnvio = "EnviarDocumento: Integração não encontrada.";
                        integracao.StatusEnvio = Dominio.ObjetosDeValor.Enumerador.StatusIntegracaoLsTranslog.Erro;
                        repIntegracao.Atualizar(integracao);
                        return false;
                    }
                }
                else
                {
                    Servicos.Log.TratarErro("EnviarDocumento: integração código " + codigo + " não foi possível logar.", "LsTranslog");

                    integracao.ObservacaoEnvio = "EnviarDocumento: não foi possível logar.";
                    integracao.StatusEnvio = Dominio.ObjetosDeValor.Enumerador.StatusIntegracaoLsTranslog.Erro;
                    repIntegracao.Atualizar(integracao);

                    return false;
                }

            }
            catch (Exception e)
            {
                Servicos.Log.TratarErro("EnviarDocumento: integração código " + codigo + ": " + e.Message, "LsTranslog");

                integracao.ObservacaoEnvio = "EnviarDocumento: " + e.Message;
                integracao.StatusEnvio = Dominio.ObjetosDeValor.Enumerador.StatusIntegracaoLsTranslog.Erro;
                repIntegracao.Atualizar(integracao);

                return false;
            }
        }

        public bool ConsultarDocumento(int codigo, Repositorio.UnitOfWork unidadeDeTrabalho)
        {
            Repositorio.IntegracaoLsTranslog repIntegracao = new Repositorio.IntegracaoLsTranslog(unidadeDeTrabalho);
            Repositorio.IntegracaoLsTranslogLog repIntegracaoLsTranslogLog = new Repositorio.IntegracaoLsTranslogLog(unidadeDeTrabalho);

            Dominio.Entidades.IntegracaoLsTranslog integracao = repIntegracao.BuscaPorCodigo(codigo);
            try
            {
                string token = this.Logar(integracao.Empresa.Codigo, unidadeDeTrabalho);

                if (!string.IsNullOrWhiteSpace(token))
                {
                    if (integracao != null)
                    {
                        List<Dominio.Entidades.IntegracaoLsTranslogLog> listaIntegracoes = repIntegracaoLsTranslogLog.BuscaPorCodigoIntegracao(integracao.Codigo, Dominio.ObjetosDeValor.Enumerador.TipoIntegracaoLsTranslog.Envio);

                        if (listaIntegracoes != null && listaIntegracoes.Count > 0)
                        {
                            for (var i = 0; i < listaIntegracoes.Count; i++)
                            {
                                Dominio.Entidades.IntegracaoLsTranslogLog integracaoLog = new Dominio.Entidades.IntegracaoLsTranslogLog();
                                integracaoLog.IntegracaoLsTranslog = integracao;
                                integracaoLog.Tipo = Dominio.ObjetosDeValor.Enumerador.TipoIntegracaoLsTranslog.Consulta;
                                integracaoLog.Status = Dominio.ObjetosDeValor.Enumerador.StatusIntegracaoLsTranslog.Pendente;
                                integracaoLog.Identificador = listaIntegracoes[i].Identificador;
                                integracaoLog.NumeroNFe = listaIntegracoes[i].NumeroNFe;
                                integracaoLog.Data = DateTime.Now;
                                repIntegracaoLsTranslogLog.Inserir(integracaoLog);

                                List<Dominio.ObjetosDeValor.LsTranslog.AtividadeReturn> retornos = this.ConsultarPorIdentificador(listaIntegracoes[i].Identificador, token, integracaoLog, unidadeDeTrabalho);

                                if (retornos != null && retornos.Count > 0)
                                {
                                    bool salvouOcorrencia = false;
                                    for (var j = 0; j < retornos.Count; j++)
                                    //foreach (Dominio.ObjetosDeValor.LsTranslog.AtividadeReturn retorno in retornos)
                                    {
                                        if (SalvarOcorrencia(integracao, retornos[j], unidadeDeTrabalho))
                                        {
                                            if (retornos[j].IDStatusAtividade == 8 || retornos[j].IDStatusAtividade == 9 || retornos[j].IDStatusAtividade == 10 || retornos[j].IDStatusAtividade == 12 || retornos[j].IDStatusAtividade == 15 || retornos[j].IDStatusAtividade == 19)
                                                integracao.StatusConsulta = Dominio.ObjetosDeValor.Enumerador.StatusIntegracaoLsTranslog.Sucesso;
                                            else
                                                integracao.StatusConsulta = Dominio.ObjetosDeValor.Enumerador.StatusIntegracaoLsTranslog.Pendente;
                                            repIntegracao.Atualizar(integracao);
                                            salvouOcorrencia = true; //Quando salvou a ocorrência não exclui o log de integracao
                                        }

                                    }
                                    if (!salvouOcorrencia)
                                        repIntegracaoLsTranslogLog.Deletar(integracaoLog);
                                }
                                else
                                {
                                    if (integracao.StatusConsulta == Dominio.ObjetosDeValor.Enumerador.StatusIntegracaoLsTranslog.Erro)
                                    {
                                        integracao.StatusConsulta = Dominio.ObjetosDeValor.Enumerador.StatusIntegracaoLsTranslog.Pendente;
                                        repIntegracao.Atualizar(integracao);
                                    }

                                }

                            }
                        }
                    }
                    else
                    {
                        Servicos.Log.TratarErro("ConsultarDocumento: integração código " + codigo + " não encontrada.", "LsTranslog");

                        integracao.ObservacaoRetorno = "ConsultarDocumento: Integração não encontrada.";
                        integracao.StatusConsulta = Dominio.ObjetosDeValor.Enumerador.StatusIntegracaoLsTranslog.Erro;
                        repIntegracao.Atualizar(integracao);
                        return false;
                    }
                }
                else
                {
                    Servicos.Log.TratarErro("ConsultarDocumento: integração código " + codigo + " não foi possível logar.", "LsTranslog");

                    integracao.ObservacaoRetorno = "ConsultarDocumento: não foi possível logar.";
                    integracao.StatusConsulta = Dominio.ObjetosDeValor.Enumerador.StatusIntegracaoLsTranslog.Erro;
                    repIntegracao.Atualizar(integracao);

                    return false;
                }
                return true;
            }
            catch (Exception e)
            {
                Servicos.Log.TratarErro("ConsultarDocumento: integração código " + codigo + ": " + e.Message, "LsTranslog");

                integracao.ObservacaoRetorno = "ConsultarDocumento: " + e.Message;
                integracao.StatusConsulta = Dominio.ObjetosDeValor.Enumerador.StatusIntegracaoLsTranslog.Erro;
                repIntegracao.Atualizar(integracao);

                return false;
            }
        }

        private string Logar(int codigoEmpresa, Repositorio.UnitOfWork unidadeDeTrabalho)
        {
            Repositorio.ConfiguracaoIntegracaoLsTranslog repConfiguracao = new Repositorio.ConfiguracaoIntegracaoLsTranslog(unidadeDeTrabalho);
            Dominio.Entidades.ConfiguracaoIntegracaoLsTranslog configuracao = repConfiguracao.BuscaPorEmpresa(codigoEmpresa);

            string token = string.Empty;

            if (configuracao != null)
            {
                string urlAPI = "http://lsapi.azurewebsites.net/api/Secure/Authenticate";

                HttpClient client = HttpClientFactoryWrapper.GetClient(nameof(LsTranslog));
                client.BaseAddress = new Uri(urlAPI);
                client.DefaultRequestHeaders.Accept.Clear();
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

                Dominio.ObjetosDeValor.LsTranslog.Authenticate antenticar = new Dominio.ObjetosDeValor.LsTranslog.Authenticate
                {
                    Login = configuracao.Login,
                    Password = configuracao.Senha
                };

                string jsonPost = JsonConvert.SerializeObject(antenticar, Formatting.Indented);

                var content = new StringContent(jsonPost.ToString(), Encoding.UTF8, "application/json");
                var result = client.PostAsync(urlAPI, content).Result;

                if (result.IsSuccessStatusCode)
                {
                    //Servicos.Log.TratarErro("Login retorno: " + result.Content.ReadAsStringAsync().Result, "LsTranslog");

                    Dominio.ObjetosDeValor.LsTranslog.AuthenticateReturn antenticarRetorno = JsonConvert.DeserializeObject<Dominio.ObjetosDeValor.LsTranslog.AuthenticateReturn>(result.Content.ReadAsStringAsync().Result);

                    token = antenticarRetorno.Token;
                }
                else
                {
                    Servicos.Log.TratarErro("Login: " + jsonPost, "LsTranslog");
                    Servicos.Log.TratarErro("Login retorno: Não autenticou", "LsTranslog");
                }
            }
            else
                Servicos.Log.TratarErro("Login: Não possui configuração para logar", "LsTranslog");

            return token;

        }

        private Dominio.ObjetosDeValor.LsTranslog.AtividadeReturn Enviar(Dominio.ObjetosDeValor.LsTranslog.Atividade atividade, string token, Dominio.Entidades.IntegracaoLsTranslogLog integracaoLog, Repositorio.UnitOfWork unidadeDeTrabalho)
        {
            Repositorio.IntegracaoLsTranslogLog repIntegracaoLog = new Repositorio.IntegracaoLsTranslogLog(unidadeDeTrabalho);

            string urlAPI = "http://lsapi.azurewebsites.net/api/Atividades/includePartnerActiviity";

            Dominio.ObjetosDeValor.LsTranslog.AtividadeReturn retorno = null;

            HttpClient client = HttpClientFactoryWrapper.GetClient(nameof(LsTranslog));
            client.BaseAddress = new Uri(urlAPI);
            client.DefaultRequestHeaders.Accept.Clear();
            client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("LS.Authorization", token);

            string jsonPost = JsonConvert.SerializeObject(atividade, Formatting.Indented);

            //Servicos.Log.TratarErro("Atividade: " + jsonPost, "LsTranslog");
            integracaoLog.Envio = jsonPost;

            var content = new StringContent(jsonPost.ToString(), Encoding.UTF8, "application/json");
            var result = client.PostAsync(urlAPI, content).Result;

            if (result.IsSuccessStatusCode)
            {
                retorno = JsonConvert.DeserializeObject<Dominio.ObjetosDeValor.LsTranslog.AtividadeReturn>(result.Content.ReadAsStringAsync().Result);
                integracaoLog.Retorno = result.Content.ReadAsStringAsync().Result;
                integracaoLog.Status = Dominio.ObjetosDeValor.Enumerador.StatusIntegracaoLsTranslog.Sucesso;
            }
            else
            {
                integracaoLog.Status = Dominio.ObjetosDeValor.Enumerador.StatusIntegracaoLsTranslog.Erro;
                if (!string.IsNullOrWhiteSpace(result.Content.ReadAsStringAsync().Result))
                {
                    Servicos.Log.TratarErro("Atividade: " + jsonPost, "LsTranslog");
                    Servicos.Log.TratarErro("Atividade Retorno: " + result.Content.ReadAsStringAsync().Result, "LsTranslog");
                    integracaoLog.Retorno = result.Content.ReadAsStringAsync().Result;
                }
                else
                {
                    Servicos.Log.TratarErro("Atividade: " + jsonPost, "LsTranslog");
                    Servicos.Log.TratarErro("Atividade Retorno: " + result.ReasonPhrase, "LsTranslog");
                    integracaoLog.Retorno = result.ReasonPhrase;
                    integracaoLog.Mensagem = "Não integrado: " + result.ReasonPhrase;
                }
            }

            repIntegracaoLog.Atualizar(integracaoLog);

            return retorno;
        }

        private List<Dominio.ObjetosDeValor.LsTranslog.AtividadeReturn> ConsultarPorIdentificador(string identificador, string token, Dominio.Entidades.IntegracaoLsTranslogLog integracaoLog, Repositorio.UnitOfWork unidadeDeTrabalho)
        {
            Repositorio.IntegracaoLsTranslogLog repIntegracaoLog = new Repositorio.IntegracaoLsTranslogLog(unidadeDeTrabalho);

            //string urlAPI = "http://lsapi.azurewebsites.net/api/Atividades/GetByIdentifier?identifier=" + identificador;
            string urlAPI = "http://lsapi.azurewebsites.net/api/Atividades/GetByOS?OS=" + identificador;

            List<Dominio.ObjetosDeValor.LsTranslog.AtividadeReturn> retorno = null;

            Servicos.Log.TratarErro("Consulta Retorno identificador: " + identificador, "LsTranslog");

            HttpClient client = HttpClientFactoryWrapper.GetClient(nameof(LsTranslog));
            client.BaseAddress = new Uri(urlAPI);
            client.DefaultRequestHeaders.Accept.Clear();
            client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("LS.Authorization", token);

            var result = client.GetAsync(urlAPI).Result;

            if (result.IsSuccessStatusCode)
            {
                integracaoLog.Retorno = result.Content.ReadAsStringAsync().Result;
                retorno = JsonConvert.DeserializeObject<List<Dominio.ObjetosDeValor.LsTranslog.AtividadeReturn>>(result.Content.ReadAsStringAsync().Result);// .Replace("[","").Replace("]", ""));
                integracaoLog.Status = Dominio.ObjetosDeValor.Enumerador.StatusIntegracaoLsTranslog.Sucesso;

                Servicos.Log.TratarErro("Consulta Retorno: SUCESSO " + result.Content.ReadAsStringAsync().Result, "LsTranslog");
            }
            else
            {
                integracaoLog.Status = Dominio.ObjetosDeValor.Enumerador.StatusIntegracaoLsTranslog.Erro;
                if (!string.IsNullOrWhiteSpace(result.Content.ReadAsStringAsync().Result))
                {
                    Servicos.Log.TratarErro("Consulta Retorno: " + result.Content.ReadAsStringAsync().Result, "LsTranslog");
                    integracaoLog.Retorno = result.Content.ReadAsStringAsync().Result;
                }
                else
                {
                    Servicos.Log.TratarErro("Consulta Retorno: " + result.ReasonPhrase + " - " + identificador, "LsTranslog");
                    integracaoLog.Retorno = result.ReasonPhrase;
                    repIntegracaoLog.Deletar(integracaoLog);
                    return retorno;
                }
            }

            repIntegracaoLog.Atualizar(integracaoLog);

            return retorno;
        }

        private bool SalvarOcorrencia(Dominio.Entidades.IntegracaoLsTranslog integracao, Dominio.ObjetosDeValor.LsTranslog.AtividadeReturn retorno, Repositorio.UnitOfWork unidadeDeTrabalho)
        {
            Repositorio.TipoDeOcorrenciaDeCTe repOcorrencia = new Repositorio.TipoDeOcorrenciaDeCTe(unidadeDeTrabalho);
            Repositorio.OcorrenciaDeCTe repOcorrenciaCTe = new Repositorio.OcorrenciaDeCTe(unidadeDeTrabalho);
            Repositorio.OcorrenciaDeNFSe repOcorrenciaNFSe = new Repositorio.OcorrenciaDeNFSe(unidadeDeTrabalho);
            Repositorio.OcorrenciaDeNFe repOcorrenciaNFe = new Repositorio.OcorrenciaDeNFe(unidadeDeTrabalho);
            Repositorio.Cliente repCliente = new Repositorio.Cliente(unidadeDeTrabalho);

            Dominio.Entidades.Cliente cliente = null;
            if (integracao.CTe != null)
                cliente = repCliente.BuscarPorCPFCNPJ(double.Parse(integracao.CTe.Remetente.CPF_CNPJ));
            else if (integracao.NFSe != null)
                cliente = repCliente.BuscarPorCPFCNPJ(double.Parse(integracao.NFSe.Tomador.CPF_CNPJ));
            else if (integracao.NFe != null)
                cliente = repCliente.BuscarPorCPFCNPJ(integracao.NFe.Contratante != null ? integracao.NFe.Contratante.CPF_CNPJ : integracao.NFe.Emitente.CPF_CNPJ);

            Dominio.Entidades.TipoDeOcorrenciaDeCTe ocorrencia = this.BuscarOcorrencia(integracao.Empresa, cliente, retorno.IDStatusAtividade.ToString(), retorno.Identificador, unidadeDeTrabalho);

            if (ocorrencia == null)
            {
                if (integracao.CTe != null)
                    cliente = repCliente.BuscarPorCPFCNPJ(double.Parse(integracao.CTe.TomadorPagador.CPF_CNPJ));

                ocorrencia = this.BuscarOcorrencia(integracao.Empresa, cliente, retorno.IDStatusAtividade.ToString(), retorno.Identificador, unidadeDeTrabalho);
            }

            if (ocorrencia != null)
            {
                DateTime dataOcorrencia = DateTime.Now;
                DateTime.TryParse(retorno.DataTermino, out dataOcorrencia);
                if (retorno.IDStatusAtividade == 4 && !string.IsNullOrWhiteSpace(retorno.CriadaPara)) //Item despachado para entrega
                    DateTime.TryParse(retorno.CriadaPara, out dataOcorrencia);
                if (dataOcorrencia == DateTime.MinValue)
                    DateTime.TryParse(retorno.DataEntrada, out dataOcorrencia);
                if (dataOcorrencia == DateTime.MinValue)
                    dataOcorrencia = DateTime.Now;

                DateTime dataOcorrenciaAux = DateTime.MinValue;
                string horaOcorrencia = dataOcorrencia.ToString("HH:mm");
                if (horaOcorrencia == "00:00")
                    horaOcorrencia = "18:00";

                DateTime.TryParseExact(dataOcorrencia.ToString("dd/MM/yyyy") + " " + horaOcorrencia, "dd/MM/yyyy HH:mm", null, System.Globalization.DateTimeStyles.None, out dataOcorrenciaAux);

                if (integracao.CTe != null)
                {
                    if (repOcorrenciaCTe.ContarOcorrenciaCTe(integracao.CTe.Codigo, ocorrencia.Codigo, dataOcorrenciaAux > DateTime.MinValue ? dataOcorrenciaAux : dataOcorrencia) == 0)
                    {
                        Dominio.Entidades.OcorrenciaDeCTe ocorrenciaCTe = new Dominio.Entidades.OcorrenciaDeCTe();
                        ocorrenciaCTe.CTe = integracao.CTe;
                        ocorrenciaCTe.Ocorrencia = ocorrencia;
                        ocorrenciaCTe.DataDeCadastro = DateTime.Now;
                        ocorrenciaCTe.DataDaOcorrencia = dataOcorrenciaAux > DateTime.MinValue ? dataOcorrenciaAux : dataOcorrencia;
                        ocorrenciaCTe.Observacao = retorno.Observacoes;

                        repOcorrenciaCTe.Inserir(ocorrenciaCTe);

                        return true;
                    }
                }
                else if (integracao.NFSe != null)
                {
                    if (repOcorrenciaNFSe.ContarOcorrenciaNFSe(integracao.NFSe.Codigo, ocorrencia.Codigo, dataOcorrenciaAux > DateTime.MinValue ? dataOcorrenciaAux : dataOcorrencia) == 0)
                    {
                        Dominio.Entidades.OcorrenciaDeNFSe ocorrenciaNFSe = new Dominio.Entidades.OcorrenciaDeNFSe();
                        ocorrenciaNFSe.NFSe = integracao.NFSe;
                        ocorrenciaNFSe.Ocorrencia = ocorrencia;
                        ocorrenciaNFSe.DataDeCadastro = DateTime.Now;
                        ocorrenciaNFSe.DataDaOcorrencia = dataOcorrenciaAux > DateTime.MinValue ? dataOcorrenciaAux : dataOcorrencia;
                        ocorrenciaNFSe.Observacao = retorno.Observacoes;

                        repOcorrenciaNFSe.Inserir(ocorrenciaNFSe);

                        return true;
                    }
                }
                else if (integracao.NFe != null)
                {
                    if (repOcorrenciaNFe.ContarOcorrenciaNFe(integracao.NFe.Codigo, ocorrencia.Codigo, dataOcorrenciaAux > DateTime.MinValue ? dataOcorrenciaAux : dataOcorrencia) == 0)
                    {
                        Dominio.Entidades.OcorrenciaDeNFe ocorrenciaDeNFe = new Dominio.Entidades.OcorrenciaDeNFe();
                        ocorrenciaDeNFe.NFe = integracao.NFe;
                        ocorrenciaDeNFe.Ocorrencia = ocorrencia;
                        ocorrenciaDeNFe.DataDeCadastro = DateTime.Now;
                        ocorrenciaDeNFe.DataDaOcorrencia = dataOcorrenciaAux > DateTime.MinValue ? dataOcorrenciaAux : dataOcorrencia;
                        ocorrenciaDeNFe.Observacao = retorno.Observacoes;

                        repOcorrenciaNFe.Inserir(ocorrenciaDeNFe);

                        return true;
                    }
                }
            }

            return false;
        }

        private Dominio.Entidades.TipoDeOcorrenciaDeCTe BuscarOcorrencia(Dominio.Entidades.Empresa empresa, Dominio.Entidades.Cliente cliente, string codigoIntegracao, string descricaoIdentificador, Repositorio.UnitOfWork unidadeDeTrabalho)
        {
            Repositorio.TipoDeOcorrenciaDeCTe repOcorrencia = new Repositorio.TipoDeOcorrenciaDeCTe(unidadeDeTrabalho);

            if (codigoIntegracao == "6") //Cliente ausente
            {
                Dominio.Entidades.TipoDeOcorrenciaDeCTe ocorrenciaAusente = repOcorrencia.BuscarPorCodigoIntegracaoDescricaoeCliente(empresa.Codigo, codigoIntegracao, "Cliente ausente - tentativa 1", cliente.CPF_CNPJ);

                if (!string.IsNullOrWhiteSpace(descricaoIdentificador) && descricaoIdentificador.Contains("R1"))
                    ocorrenciaAusente = repOcorrencia.BuscarPorCodigoIntegracaoDescricaoeCliente(empresa.Codigo, codigoIntegracao, "Cliente ausente - tentativa 2", cliente.CPF_CNPJ);
                else
                if (!string.IsNullOrWhiteSpace(descricaoIdentificador) && descricaoIdentificador.Contains("R2"))
                    ocorrenciaAusente = repOcorrencia.BuscarPorCodigoIntegracaoDescricaoeCliente(empresa.Codigo, codigoIntegracao, "Cliente ausente - tentativa 3", cliente.CPF_CNPJ);

                if (ocorrenciaAusente != null)
                    return ocorrenciaAusente;
            }
            else if (codigoIntegracao == "14") //Endereço não localizado
            {
                Dominio.Entidades.TipoDeOcorrenciaDeCTe ocorrenciaAusente = repOcorrencia.BuscarPorCodigoIntegracaoDescricaoeCliente(empresa.Codigo, codigoIntegracao, "Endereço não localizado - tentativa 1", cliente.CPF_CNPJ);

                if (!string.IsNullOrWhiteSpace(descricaoIdentificador) && descricaoIdentificador.Contains("R1"))
                    ocorrenciaAusente = repOcorrencia.BuscarPorCodigoIntegracaoDescricaoeCliente(empresa.Codigo, codigoIntegracao, "Endereço não localizado - tentativa 2", cliente.CPF_CNPJ);
                else
                if (!string.IsNullOrWhiteSpace(descricaoIdentificador) && descricaoIdentificador.Contains("R2"))
                    ocorrenciaAusente = repOcorrencia.BuscarPorCodigoIntegracaoDescricaoeCliente(empresa.Codigo, codigoIntegracao, "Endereço não localizado - tentativa 3", cliente.CPF_CNPJ);

                if (ocorrenciaAusente != null)
                    return ocorrenciaAusente;
            }
            else if (codigoIntegracao == "20") //Numero não localizado
            {
                Dominio.Entidades.TipoDeOcorrenciaDeCTe ocorrenciaAusente = repOcorrencia.BuscarPorCodigoIntegracaoDescricaoeCliente(empresa.Codigo, codigoIntegracao, "Numero não localizado - tentativa 1", cliente.CPF_CNPJ);

                if (!string.IsNullOrWhiteSpace(descricaoIdentificador) && descricaoIdentificador.Contains("R1"))
                    ocorrenciaAusente = repOcorrencia.BuscarPorCodigoIntegracaoDescricaoeCliente(empresa.Codigo, codigoIntegracao, "Numero não localizado - tentativa 2", cliente.CPF_CNPJ);
                else
                if (!string.IsNullOrWhiteSpace(descricaoIdentificador) && descricaoIdentificador.Contains("R2"))
                    ocorrenciaAusente = repOcorrencia.BuscarPorCodigoIntegracaoDescricaoeCliente(empresa.Codigo, codigoIntegracao, "Numero não localizado - tentativa 3", cliente.CPF_CNPJ);

                if (ocorrenciaAusente != null)
                    return ocorrenciaAusente;
            }

            Dominio.Entidades.TipoDeOcorrenciaDeCTe ocorrencia = repOcorrencia.BuscarPorCodigoIntegracaoCliente(empresa.Codigo, codigoIntegracao, cliente.CPF_CNPJ);

            return ocorrencia;
        }



        private bool EnviarDocumentoNatura(Dominio.Entidades.IntegracaoLsTranslog integracao, string token, Repositorio.UnitOfWork unidadeDeTrabalho)
        {
            Repositorio.NotaFiscalDocumentoTransporteNatura repNotaFiscalDocumentoTransporteNatura = new Repositorio.NotaFiscalDocumentoTransporteNatura(unidadeDeTrabalho);
            Repositorio.IntegracaoLsTranslog repIntegracao = new Repositorio.IntegracaoLsTranslog(unidadeDeTrabalho);
            Repositorio.InformacaoCargaCTE repInformacaoCargaCTE = new Repositorio.InformacaoCargaCTE(unidadeDeTrabalho);

            Dominio.ObjetosDeValor.LsTranslog.Atividade documento = null;

            List<Dominio.Entidades.NotaFiscalDocumentoTransporteNatura> listaNotaFiscalNatura = null;
            if (integracao.CTe != null)
                listaNotaFiscalNatura = repNotaFiscalDocumentoTransporteNatura.BuscarListaNotasPorCTe(integracao.CTe.Empresa.Codigo, integracao.CTe.Codigo);
            else
                listaNotaFiscalNatura = repNotaFiscalDocumentoTransporteNatura.BuscarListaNotasPorNFSe(integracao.NFSe.Empresa.Codigo, integracao.NFSe.Codigo);

            if (listaNotaFiscalNatura != null && listaNotaFiscalNatura.Count > 0)
            {
                for (var j = 0; j < listaNotaFiscalNatura.Count; j++)
                {
                    Dominio.Entidades.NotaFiscalDocumentoTransporteNatura notaFiscalNatura = repNotaFiscalDocumentoTransporteNatura.BuscarPorCodigo(listaNotaFiscalNatura[j].Codigo);

                    for (var i = 0; i < notaFiscalNatura.Quantidade; i++)
                    {
                        string identificador = notaFiscalNatura.SolicitacaoNumero + string.Format("{0:D3}", i + 1);
                        if (integracao.Empresa.TipoAmbiente == Dominio.Enumeradores.TipoAmbiente.Homologacao)
                            identificador = string.Concat("Teste_", identificador);

                        documento = new Dominio.ObjetosDeValor.LsTranslog.Atividade
                        {
                            Identificador = identificador,
                            OS = identificador,//notaFiscalNatura.SolicitacaoNumero,
                            IDLocal = 146, //Código identificador da Natura no sistema da LSTranslog
                            Observacoes = "Natura/MultiCTe",
                            IDTipo = "19",
                            //IDPolo = "1",
                            Rastreio = notaFiscalNatura.PedidoNumero,
                            NotaFiscal = notaFiscalNatura.Numero.ToString(),
                            //Datalimite = "2018-01-25",
                            NumeroCTE = notaFiscalNatura.CTe != null ? notaFiscalNatura.CTe.Numero.ToString() : notaFiscalNatura.NFSe.Numero.ToString(),
                            NumeroDT = notaFiscalNatura.DocumentoTransporte.NumeroDT.ToString(),
                            Lote = notaFiscalNatura.DocumentoTransporte.NumeroDT.ToString(),
                            Peso = notaFiscalNatura.Peso > 0 ? notaFiscalNatura.Peso.ToString("n2") : notaFiscalNatura.CTe != null ? repInformacaoCargaCTE.ObterPesoKg(notaFiscalNatura.CTe.Codigo).ToString("n2") : "0",
                            CNPJEmissor = notaFiscalNatura.Emitente?.CPF_CNPJ_SemFormato ?? string.Empty,
                            ChaveNFE = !string.IsNullOrWhiteSpace(notaFiscalNatura.Chave) ? notaFiscalNatura.Chave : string.Empty,
                            Cliente = new Dominio.ObjetosDeValor.LsTranslog.Cliente
                            {
                                Nome = !string.IsNullOrWhiteSpace(notaFiscalNatura.CodigoCF) ? "(" + notaFiscalNatura.CodigoCF + ") " + notaFiscalNatura.Destinatario.Nome : notaFiscalNatura.Destinatario.Nome,
                                Documento = notaFiscalNatura.Destinatario.CPF_CNPJ_SemFormato,
                                Endereco = notaFiscalNatura.Destinatario.Endereco,
                                Numero = notaFiscalNatura.Destinatario.Numero,
                                Cep = Utilidades.String.OnlyNumbers(notaFiscalNatura.Destinatario.CEP),
                                Bairro = notaFiscalNatura.Destinatario.Bairro,
                                Complemento = notaFiscalNatura.Destinatario.Complemento,
                                Cidade = notaFiscalNatura.Destinatario.Localidade.Descricao,
                                Telefone = notaFiscalNatura.Destinatario.Telefone1
                            },
                            Valor = notaFiscalNatura.Valor
                        };

                        Repositorio.IntegracaoLsTranslogLog repIntegracaoLog = new Repositorio.IntegracaoLsTranslogLog(unidadeDeTrabalho);

                        Dominio.Entidades.IntegracaoLsTranslogLog integracaoLog = new Dominio.Entidades.IntegracaoLsTranslogLog();
                        integracaoLog.IntegracaoLsTranslog = integracao;
                        integracaoLog.Data = DateTime.Now;
                        integracaoLog.Tipo = Dominio.ObjetosDeValor.Enumerador.TipoIntegracaoLsTranslog.Envio;
                        integracaoLog.Status = Dominio.ObjetosDeValor.Enumerador.StatusIntegracaoLsTranslog.Pendente;
                        integracaoLog.Identificador = identificador;
                        integracaoLog.NumeroNFe = notaFiscalNatura.Numero;

                        repIntegracaoLog.Inserir(integracaoLog);

                        Dominio.ObjetosDeValor.LsTranslog.AtividadeReturn retorno = this.Enviar(documento, token, integracaoLog, unidadeDeTrabalho);

                        if (retorno != null)
                        {
                            integracaoLog.Identificador = retorno.Identificador;
                            repIntegracaoLog.Atualizar(integracaoLog);
                        }
                        else
                        {
                            integracao.StatusEnvio = Dominio.ObjetosDeValor.Enumerador.StatusIntegracaoLsTranslog.Erro;
                            repIntegracao.Atualizar(integracao);

                            return false;
                        }
                    }
                }

                integracao.StatusEnvio = Dominio.ObjetosDeValor.Enumerador.StatusIntegracaoLsTranslog.Sucesso;
                repIntegracao.Atualizar(integracao);

                return true;
            }
            else
            {
                Servicos.Log.TratarErro("EnviarDocumento: Codigo " + integracao.Codigo + ": Não localizado DT Natura.", "LsTranslog");

                integracao.ObservacaoEnvio = "EnviarDocumento: Não localizado DT Natura.";
                integracao.StatusEnvio = Dominio.ObjetosDeValor.Enumerador.StatusIntegracaoLsTranslog.Erro;
                repIntegracao.Atualizar(integracao);

                return false;
            }
        }

        private bool EnviarDocumentoNextel(Dominio.Entidades.IntegracaoLsTranslog integracao, string token, Repositorio.UnitOfWork unidadeDeTrabalho)
        {
            Repositorio.ConhecimentoDeTransporteEletronico repCTe = new Repositorio.ConhecimentoDeTransporteEletronico(unidadeDeTrabalho);
            Repositorio.ObservacaoContribuinteCTE repObsContribuinte = new Repositorio.ObservacaoContribuinteCTE(unidadeDeTrabalho);
            Repositorio.DocumentosCTE repDocumentosCTE = new Repositorio.DocumentosCTE(unidadeDeTrabalho);
            Repositorio.InformacaoCargaCTE repInformacaoCargaCTE = new Repositorio.InformacaoCargaCTE(unidadeDeTrabalho);
            Repositorio.IntegracaoLsTranslog repIntegracao = new Repositorio.IntegracaoLsTranslog(unidadeDeTrabalho);

            Dominio.ObjetosDeValor.LsTranslog.Atividade documento = null;
            Dominio.Entidades.ConhecimentoDeTransporteEletronico cte = null;

            if (integracao.CTe != null)
                cte = integracao.CTe;
            else if (integracao.NFSe != null)
                cte = repCTe.BuscarPorRPS(integracao.NFSe.RPS.Codigo);

            if (cte != null)
            {
                List<Dominio.Entidades.DocumentosCTE> listaNotasFiscais = repDocumentosCTE.BuscarPorCTe(cte.Codigo);

                if (listaNotasFiscais != null && listaNotasFiscais.Count > 0)
                {
                    for (var j = 0; j < listaNotasFiscais.Count; j++)
                    {
                        List<Dominio.Entidades.ObservacaoContribuinteCTE> listaobsContribuinte = repObsContribuinte.BuscarPorCTe(cte.Empresa.Codigo, cte.Codigo);

                        Dominio.Entidades.ObservacaoContribuinteCTE obsPedido = null;
                        if (listaobsContribuinte != null && listaobsContribuinte.Count > 0)
                            obsPedido = (from o in listaobsContribuinte where o.Identificador.Equals("PEDIDO") select o).FirstOrDefault();

                        if (obsPedido != null)
                        {
                            string identificador = obsPedido.Descricao;
                            if (integracao.Empresa.TipoAmbiente == Dominio.Enumeradores.TipoAmbiente.Homologacao)
                                identificador = string.Concat("Teste_", identificador);

                            documento = new Dominio.ObjetosDeValor.LsTranslog.Atividade
                            {
                                Identificador = identificador,
                                OS = identificador,
                                IDLocal = 159,
                                Observacoes = "Nextel/MultiCTe",
                                IDTipo = "6",
                                Rastreio = identificador,
                                NotaFiscal = listaNotasFiscais[j].Numero,
                                NumeroCTE = cte.Numero.ToString(),
                                NumeroDT = "",
                                Lote = "",
                                Peso = listaNotasFiscais[j].Peso > 0 ? listaNotasFiscais[j].Peso.ToString("n2") : repInformacaoCargaCTE.ObterPesoKg(cte.Codigo).ToString("n2"),
                                CNPJEmissor = cte.Remetente?.CPF_CNPJ ?? string.Empty,
                                ChaveNFE = !string.IsNullOrWhiteSpace(listaNotasFiscais[j].ChaveNFE) ? listaNotasFiscais[j].ChaveNFE : string.Empty,
                                Cliente = new Dominio.ObjetosDeValor.LsTranslog.Cliente
                                {
                                    Nome = cte.Destinatario.Nome,
                                    Documento = cte.Destinatario.CPF_CNPJ_SemFormato,
                                    Endereco = cte.Destinatario.Endereco,
                                    Numero = cte.Destinatario.Numero,
                                    Cep = Utilidades.String.OnlyNumbers(cte.Destinatario.CEP),
                                    Bairro = cte.Destinatario.Bairro,
                                    Complemento = cte.Destinatario.Complemento,
                                    Cidade = cte.Destinatario.Localidade.Descricao,
                                    Telefone = cte.Destinatario.Telefone1
                                },
                                Valor = listaNotasFiscais[j].Valor > 0 ? listaNotasFiscais[j].Valor : Math.Round(cte.ValorTotalMercadoria / listaNotasFiscais.Count(), 2, MidpointRounding.ToEven),
                                Tipo = "D+1"
                            };

                            Repositorio.IntegracaoLsTranslogLog repIntegracaoLog = new Repositorio.IntegracaoLsTranslogLog(unidadeDeTrabalho);

                            Dominio.Entidades.IntegracaoLsTranslogLog integracaoLog = new Dominio.Entidades.IntegracaoLsTranslogLog();
                            integracaoLog.IntegracaoLsTranslog = integracao;
                            integracaoLog.Data = DateTime.Now;
                            integracaoLog.Tipo = Dominio.ObjetosDeValor.Enumerador.TipoIntegracaoLsTranslog.Envio;
                            integracaoLog.Status = Dominio.ObjetosDeValor.Enumerador.StatusIntegracaoLsTranslog.Pendente;
                            integracaoLog.Identificador = identificador;
                            integracaoLog.NumeroNFe = int.Parse(listaNotasFiscais[j].Numero);

                            repIntegracaoLog.Inserir(integracaoLog);

                            Dominio.ObjetosDeValor.LsTranslog.AtividadeReturn retorno = this.Enviar(documento, token, integracaoLog, unidadeDeTrabalho);

                            if (retorno != null)
                            {
                                integracaoLog.Identificador = retorno.Identificador;
                                repIntegracaoLog.Atualizar(integracaoLog);
                            }
                            else
                            {
                                integracao.StatusEnvio = Dominio.ObjetosDeValor.Enumerador.StatusIntegracaoLsTranslog.Erro;
                                repIntegracao.Atualizar(integracao);

                                return false;
                            }
                        }
                        else
                        {
                            Servicos.Log.TratarErro("EnviarDocumento: Codigo " + integracao.Codigo + ": Não localizado numero pedido NEXTEL.", "LsTranslog");

                            integracao.ObservacaoEnvio = "EnviarDocumento: Não localizado número pedido Nextel.";
                            integracao.StatusEnvio = Dominio.ObjetosDeValor.Enumerador.StatusIntegracaoLsTranslog.Erro;
                            repIntegracao.Atualizar(integracao);

                            return false;
                        }
                    }

                    integracao.StatusEnvio = Dominio.ObjetosDeValor.Enumerador.StatusIntegracaoLsTranslog.Sucesso;
                    repIntegracao.Atualizar(integracao);

                    return true;
                }
                else
                {
                    Servicos.Log.TratarErro("EnviarDocumento: Codigo " + integracao.Codigo + ": Não localizado NFe NEXTEL.", "LsTranslog");

                    integracao.ObservacaoEnvio = "EnviarDocumento: Não localizado NFe Nextel.";
                    integracao.StatusEnvio = Dominio.ObjetosDeValor.Enumerador.StatusIntegracaoLsTranslog.Erro;
                    repIntegracao.Atualizar(integracao);

                    return false;
                }
            }
            else
            {
                Servicos.Log.TratarErro("EnviarDocumento: Codigo " + integracao.Codigo + ": Não localizado nenhum documento NEXTEL.", "LsTranslog");

                integracao.ObservacaoEnvio = "EnviarDocumento: Não localizado nenhum documento.";
                integracao.StatusEnvio = Dominio.ObjetosDeValor.Enumerador.StatusIntegracaoLsTranslog.Erro;
                repIntegracao.Atualizar(integracao);

                return false;
            }
        }

        private bool EnviarDocumento4Bio(Dominio.Entidades.IntegracaoLsTranslog integracao, string token, Repositorio.UnitOfWork unidadeDeTrabalho)
        {
            Repositorio.ConhecimentoDeTransporteEletronico repCTe = new Repositorio.ConhecimentoDeTransporteEletronico(unidadeDeTrabalho);
            Repositorio.ObservacaoContribuinteCTE repObsContribuinte = new Repositorio.ObservacaoContribuinteCTE(unidadeDeTrabalho);
            Repositorio.DocumentosCTE repDocumentosCTE = new Repositorio.DocumentosCTE(unidadeDeTrabalho);
            Repositorio.InformacaoCargaCTE repInformacaoCargaCTE = new Repositorio.InformacaoCargaCTE(unidadeDeTrabalho);
            Repositorio.IntegracaoLsTranslog repIntegracao = new Repositorio.IntegracaoLsTranslog(unidadeDeTrabalho);

            Dominio.ObjetosDeValor.LsTranslog.Atividade documento = null;
            Dominio.Entidades.ConhecimentoDeTransporteEletronico cte = null;

            if (integracao.CTe != null)
                cte = integracao.CTe;
            else if (integracao.NFSe != null)
                cte = repCTe.BuscarPorRPS(integracao.NFSe.RPS.Codigo);

            if (cte != null)
            {
                List<Dominio.Entidades.DocumentosCTE> listaNotasFiscais = repDocumentosCTE.BuscarPorCTe(cte.Codigo);

                if (listaNotasFiscais != null && listaNotasFiscais.Count > 0)
                {
                    for (var j = 0; j < listaNotasFiscais.Count; j++)
                    {
                        List<Dominio.Entidades.ObservacaoContribuinteCTE> listaobsContribuinte = repObsContribuinte.BuscarPorCTe(cte.Empresa.Codigo, cte.Codigo);

                        Dominio.Entidades.ObservacaoContribuinteCTE obsPedido = null;
                        if (listaobsContribuinte != null && listaobsContribuinte.Count > 0)
                            obsPedido = (from o in listaobsContribuinte where o.Identificador.Equals("PEDIDO") select o).FirstOrDefault();

                        if (obsPedido != null)
                        {
                            string identificador = obsPedido.Descricao;
                            if (integracao.Empresa.TipoAmbiente == Dominio.Enumeradores.TipoAmbiente.Homologacao)
                                identificador = string.Concat("Teste_", identificador);

                            documento = new Dominio.ObjetosDeValor.LsTranslog.Atividade
                            {
                                Identificador = identificador,
                                OS = identificador,
                                IDLocal = 165,
                                Observacoes = "4Bio/MultiCTe",
                                IDTipo = "6",
                                Rastreio = identificador,
                                NotaFiscal = listaNotasFiscais[j].Numero,
                                NumeroCTE = cte.Numero.ToString(),
                                NumeroDT = "",
                                Lote = "",
                                Peso = listaNotasFiscais[j].Peso > 0 ? listaNotasFiscais[j].Peso.ToString("n2") : repInformacaoCargaCTE.ObterPesoKg(cte.Codigo).ToString("n2"),
                                CNPJEmissor = cte.Remetente?.CPF_CNPJ ?? string.Empty,
                                ChaveNFE = !string.IsNullOrWhiteSpace(listaNotasFiscais[j].ChaveNFE) ? listaNotasFiscais[j].ChaveNFE : string.Empty,
                                Cliente = new Dominio.ObjetosDeValor.LsTranslog.Cliente
                                {
                                    Nome = cte.Destinatario.Nome,
                                    Documento = cte.Destinatario.CPF_CNPJ_SemFormato,
                                    Endereco = cte.Destinatario.Endereco,
                                    Numero = cte.Destinatario.Numero,
                                    Cep = Utilidades.String.OnlyNumbers(cte.Destinatario.CEP),
                                    Bairro = cte.Destinatario.Bairro,
                                    Complemento = cte.Destinatario.Complemento,
                                    Cidade = cte.Destinatario.Localidade.Descricao,
                                    Telefone = cte.Destinatario.Telefone1
                                },
                                Valor = listaNotasFiscais[j].Valor > 0 ? listaNotasFiscais[j].Valor : Math.Round(cte.ValorTotalMercadoria / listaNotasFiscais.Count(), 2, MidpointRounding.ToEven),
                                Tipo = "D+1"
                            };

                            Repositorio.IntegracaoLsTranslogLog repIntegracaoLog = new Repositorio.IntegracaoLsTranslogLog(unidadeDeTrabalho);

                            Dominio.Entidades.IntegracaoLsTranslogLog integracaoLog = new Dominio.Entidades.IntegracaoLsTranslogLog();
                            integracaoLog.IntegracaoLsTranslog = integracao;
                            integracaoLog.Data = DateTime.Now;
                            integracaoLog.Tipo = Dominio.ObjetosDeValor.Enumerador.TipoIntegracaoLsTranslog.Envio;
                            integracaoLog.Status = Dominio.ObjetosDeValor.Enumerador.StatusIntegracaoLsTranslog.Pendente;
                            integracaoLog.Identificador = identificador;
                            integracaoLog.NumeroNFe = int.Parse(listaNotasFiscais[j].Numero);

                            repIntegracaoLog.Inserir(integracaoLog);

                            Dominio.ObjetosDeValor.LsTranslog.AtividadeReturn retorno = this.Enviar(documento, token, integracaoLog, unidadeDeTrabalho);

                            if (retorno != null)
                            {
                                integracaoLog.Identificador = retorno.Identificador;
                                repIntegracaoLog.Atualizar(integracaoLog);
                            }
                            else
                            {
                                integracao.StatusEnvio = Dominio.ObjetosDeValor.Enumerador.StatusIntegracaoLsTranslog.Erro;
                                repIntegracao.Atualizar(integracao);

                                return false;
                            }
                        }
                        else
                        {
                            Servicos.Log.TratarErro("EnviarDocumento: Codigo " + integracao.Codigo + ": Não localizado numero pedido 4Bio.", "LsTranslog");

                            integracao.ObservacaoEnvio = "EnviarDocumento: Não localizado número pedido 4Bio.";
                            integracao.StatusEnvio = Dominio.ObjetosDeValor.Enumerador.StatusIntegracaoLsTranslog.Erro;
                            repIntegracao.Atualizar(integracao);

                            return false;
                        }
                    }

                    integracao.StatusEnvio = Dominio.ObjetosDeValor.Enumerador.StatusIntegracaoLsTranslog.Sucesso;
                    repIntegracao.Atualizar(integracao);

                    return true;
                }
                else
                {
                    Servicos.Log.TratarErro("EnviarDocumento: Codigo " + integracao.Codigo + ": Não localizado NFe 4Bio.", "LsTranslog");

                    integracao.ObservacaoEnvio = "EnviarDocumento: Não localizado NFe 4Bio.";
                    integracao.StatusEnvio = Dominio.ObjetosDeValor.Enumerador.StatusIntegracaoLsTranslog.Erro;
                    repIntegracao.Atualizar(integracao);

                    return false;
                }
            }
            else
            {
                Servicos.Log.TratarErro("EnviarDocumento: Codigo " + integracao.Codigo + ": Não localizado nenhum documento 4Bio.", "LsTranslog");

                integracao.ObservacaoEnvio = "EnviarDocumento: Não localizado nenhum documento.";
                integracao.StatusEnvio = Dominio.ObjetosDeValor.Enumerador.StatusIntegracaoLsTranslog.Erro;
                repIntegracao.Atualizar(integracao);

                return false;
            }
        }

        private bool EnviarDocumentoCobasi(Dominio.Entidades.IntegracaoLsTranslog integracao, string token, Repositorio.UnitOfWork unidadeDeTrabalho)
        {
            Repositorio.ConhecimentoDeTransporteEletronico repCTe = new Repositorio.ConhecimentoDeTransporteEletronico(unidadeDeTrabalho);
            Repositorio.ObservacaoContribuinteCTE repObsContribuinte = new Repositorio.ObservacaoContribuinteCTE(unidadeDeTrabalho);
            Repositorio.DocumentosCTE repDocumentosCTE = new Repositorio.DocumentosCTE(unidadeDeTrabalho);
            Repositorio.InformacaoCargaCTE repInformacaoCargaCTE = new Repositorio.InformacaoCargaCTE(unidadeDeTrabalho);
            Repositorio.IntegracaoLsTranslog repIntegracao = new Repositorio.IntegracaoLsTranslog(unidadeDeTrabalho);

            Dominio.ObjetosDeValor.LsTranslog.Atividade documento = null;
            Dominio.Entidades.ConhecimentoDeTransporteEletronico cte = null;

            if (integracao.CTe != null)
                cte = integracao.CTe;
            else if (integracao.NFSe != null)
                cte = repCTe.BuscarPorRPS(integracao.NFSe.RPS.Codigo);

            if (cte != null)
            {
                List<Dominio.Entidades.DocumentosCTE> listaNotasFiscais = repDocumentosCTE.BuscarPorCTe(cte.Codigo);

                if (listaNotasFiscais != null && listaNotasFiscais.Count > 0)
                {
                    for (var j = 0; j < listaNotasFiscais.Count; j++)
                    {
                        List<Dominio.Entidades.ObservacaoContribuinteCTE> listaobsContribuinte = repObsContribuinte.BuscarPorCTe(cte.Empresa.Codigo, cte.Codigo);

                        Dominio.Entidades.ObservacaoContribuinteCTE obsPedido = null;
                        if (listaobsContribuinte != null && listaobsContribuinte.Count > 0)
                            obsPedido = (from o in listaobsContribuinte where o.Identificador.Equals("PEDIDO") select o).FirstOrDefault();

                        if (obsPedido != null)
                        {
                            string pedido = obsPedido.Descricao.Length > 8 ? obsPedido.Descricao.Substring(3, obsPedido.Descricao.Length - 3) : obsPedido.Descricao;

                            for (var k = 0; k < listaNotasFiscais[j].Volume; k++)
                            {
                                string identificador = string.Concat(pedido, string.Format("{0:D3}", k + 1), string.Format("{0:D3}", listaNotasFiscais[j].Volume));

                                documento = new Dominio.ObjetosDeValor.LsTranslog.Atividade
                                {
                                    Identificador = identificador,
                                    OS = identificador,
                                    IDLocal = 2115,
                                    Observacoes = "Cobasi/MultiCTe",
                                    IDTipo = "6",
                                    Rastreio = pedido,
                                    NotaFiscal = listaNotasFiscais[j].Numero,
                                    NumeroCTE = cte.Numero.ToString(),
                                    NumeroDT = "",
                                    Lote = "",
                                    Peso = listaNotasFiscais[j].Peso > 0 ? listaNotasFiscais[j].Peso.ToString("n2") : repInformacaoCargaCTE.ObterPesoKg(cte.Codigo).ToString("n2"),
                                    CNPJEmissor = cte.Remetente?.CPF_CNPJ ?? string.Empty,
                                    ChaveNFE = !string.IsNullOrWhiteSpace(listaNotasFiscais[j].ChaveNFE) ? listaNotasFiscais[j].ChaveNFE : string.Empty,
                                    Cliente = new Dominio.ObjetosDeValor.LsTranslog.Cliente
                                    {
                                        Nome = cte.Destinatario.Nome,
                                        Documento = cte.Destinatario.CPF_CNPJ_SemFormato,
                                        Endereco = cte.Destinatario.Endereco,
                                        Numero = cte.Destinatario.Numero,
                                        Cep = Utilidades.String.OnlyNumbers(cte.Destinatario.CEP),
                                        Bairro = cte.Destinatario.Bairro,
                                        Complemento = cte.Destinatario.Complemento,
                                        Cidade = cte.Destinatario.Localidade.Descricao,
                                        Telefone = cte.Destinatario.Telefone1
                                    },
                                    Valor = listaNotasFiscais[j].Valor > 0 ? listaNotasFiscais[j].Valor : Math.Round(cte.ValorTotalMercadoria / listaNotasFiscais.Count(), 2, MidpointRounding.ToEven)
                                    //Tipo = "D+1"
                                };

                                Repositorio.IntegracaoLsTranslogLog repIntegracaoLog = new Repositorio.IntegracaoLsTranslogLog(unidadeDeTrabalho);

                                Dominio.Entidades.IntegracaoLsTranslogLog integracaoLog = new Dominio.Entidades.IntegracaoLsTranslogLog();
                                integracaoLog.IntegracaoLsTranslog = integracao;
                                integracaoLog.Data = DateTime.Now;
                                integracaoLog.Tipo = Dominio.ObjetosDeValor.Enumerador.TipoIntegracaoLsTranslog.Envio;
                                integracaoLog.Status = Dominio.ObjetosDeValor.Enumerador.StatusIntegracaoLsTranslog.Pendente;
                                integracaoLog.Identificador = identificador;
                                integracaoLog.NumeroNFe = int.Parse(listaNotasFiscais[j].Numero);

                                repIntegracaoLog.Inserir(integracaoLog);

                                Dominio.ObjetosDeValor.LsTranslog.AtividadeReturn retorno = this.Enviar(documento, token, integracaoLog, unidadeDeTrabalho);

                                if (retorno != null)
                                {
                                    integracaoLog.Identificador = retorno.Identificador;
                                    repIntegracaoLog.Atualizar(integracaoLog);
                                }
                                else
                                {
                                    integracao.StatusEnvio = Dominio.ObjetosDeValor.Enumerador.StatusIntegracaoLsTranslog.Erro;
                                    repIntegracao.Atualizar(integracao);

                                    return false;
                                }
                            }
                        }
                        else
                        {
                            Servicos.Log.TratarErro("EnviarDocumento: Codigo " + integracao.Codigo + ": Não localizado numero pedido Cobasi.", "LsTranslog");

                            integracao.ObservacaoEnvio = "EnviarDocumento: Não localizado número pedido Cobasi.";
                            integracao.StatusEnvio = Dominio.ObjetosDeValor.Enumerador.StatusIntegracaoLsTranslog.Erro;
                            repIntegracao.Atualizar(integracao);

                            return false;
                        }
                    }

                    integracao.StatusEnvio = Dominio.ObjetosDeValor.Enumerador.StatusIntegracaoLsTranslog.Sucesso;
                    repIntegracao.Atualizar(integracao);

                    return true;
                }
                else
                {
                    Servicos.Log.TratarErro("EnviarDocumento: Codigo " + integracao.Codigo + ": Não localizado NFe Cobasi.", "LsTranslog");

                    integracao.ObservacaoEnvio = "EnviarDocumento: Não localizado NFe Cobasi.";
                    integracao.StatusEnvio = Dominio.ObjetosDeValor.Enumerador.StatusIntegracaoLsTranslog.Erro;
                    repIntegracao.Atualizar(integracao);

                    return false;
                }
            }
            else
            {
                if (integracao.NFe != null)
                {
                    string pedido = integracao.NFe.Pedido.Length > 8 ? integracao.NFe.Pedido.Substring(3, integracao.NFe.Pedido.Length - 3) : integracao.NFe.Pedido; //00007050169 > 07050169

                    for (var k = 0; k < integracao.NFe.Volumes; k++)
                    {
                        string identificador = string.Concat(pedido, string.Format("{0:D3}", k + 1), string.Format("{0:D3}", integracao.NFe.Volumes));

                        documento = new Dominio.ObjetosDeValor.LsTranslog.Atividade
                        {
                            Identificador = identificador,
                            OS = identificador,
                            IDLocal = 2115,
                            Observacoes = "Cobasi/MultiCTe",
                            IDTipo = "6",
                            Rastreio = pedido,
                            NotaFiscal = integracao.NFe.Numero,
                            NumeroCTE = "0",
                            NumeroDT = "",
                            Lote = "",
                            Peso = integracao.NFe.Peso.ToString("n2"),
                            CNPJEmissor = integracao.NFe.Emitente?.CPF_CNPJ_SemFormato ?? string.Empty,
                            ChaveNFE = !string.IsNullOrWhiteSpace(integracao.NFe.Chave) ? integracao.NFe.Chave : string.Empty,
                            Cliente = new Dominio.ObjetosDeValor.LsTranslog.Cliente
                            {
                                Nome = integracao.NFe.Destinatario.Nome,
                                Documento = integracao.NFe.Destinatario.CPF_CNPJ_SemFormato,
                                Endereco = integracao.NFe.Destinatario.Endereco,
                                Numero = integracao.NFe.Destinatario.Numero,
                                Cep = Utilidades.String.OnlyNumbers(integracao.NFe.Destinatario.CEP),
                                Bairro = integracao.NFe.Destinatario.Bairro,
                                Complemento = integracao.NFe.Destinatario.Complemento,
                                Cidade = integracao.NFe.Destinatario.Localidade.Descricao,
                                Telefone = integracao.NFe.Destinatario.Telefone1
                            },
                            Valor = integracao.NFe.Valor
                            //Tipo = "D+1"
                        };

                        Repositorio.IntegracaoLsTranslogLog repIntegracaoLog = new Repositorio.IntegracaoLsTranslogLog(unidadeDeTrabalho);

                        Dominio.Entidades.IntegracaoLsTranslogLog integracaoLog = new Dominio.Entidades.IntegracaoLsTranslogLog();
                        integracaoLog.IntegracaoLsTranslog = integracao;
                        integracaoLog.Data = DateTime.Now;
                        integracaoLog.Tipo = Dominio.ObjetosDeValor.Enumerador.TipoIntegracaoLsTranslog.Envio;
                        integracaoLog.Status = Dominio.ObjetosDeValor.Enumerador.StatusIntegracaoLsTranslog.Pendente;
                        integracaoLog.Identificador = identificador;
                        integracaoLog.NumeroNFe = int.Parse(integracao.NFe.Numero);

                        repIntegracaoLog.Inserir(integracaoLog);

                        Dominio.ObjetosDeValor.LsTranslog.AtividadeReturn retorno = this.Enviar(documento, token, integracaoLog, unidadeDeTrabalho);

                        if (retorno != null)
                        {
                            integracaoLog.Identificador = retorno.Identificador;
                            repIntegracaoLog.Atualizar(integracaoLog);
                        }
                        else
                        {
                            integracao.StatusEnvio = Dominio.ObjetosDeValor.Enumerador.StatusIntegracaoLsTranslog.Erro;
                            repIntegracao.Atualizar(integracao);

                            return false;
                        }
                    }

                    integracao.StatusEnvio = Dominio.ObjetosDeValor.Enumerador.StatusIntegracaoLsTranslog.Sucesso;
                    repIntegracao.Atualizar(integracao);
                    return true;
                }
                else
                {
                    Servicos.Log.TratarErro("EnviarDocumento: Codigo " + integracao.Codigo + ": Não localizado nenhum documento Cobasi.", "LsTranslog");

                    integracao.ObservacaoEnvio = "EnviarDocumento: Não localizado nenhum documento.";
                    integracao.StatusEnvio = Dominio.ObjetosDeValor.Enumerador.StatusIntegracaoLsTranslog.Erro;
                    repIntegracao.Atualizar(integracao);

                    return false;
                }
            }
        }

        private bool EnviarDocumentoMandae(Dominio.Entidades.IntegracaoLsTranslog integracao, string token, Repositorio.UnitOfWork unidadeDeTrabalho)
        {
            Repositorio.ConhecimentoDeTransporteEletronico repCTe = new Repositorio.ConhecimentoDeTransporteEletronico(unidadeDeTrabalho);
            Repositorio.ObservacaoContribuinteCTE repObsContribuinte = new Repositorio.ObservacaoContribuinteCTE(unidadeDeTrabalho);
            Repositorio.DocumentosCTE repDocumentosCTE = new Repositorio.DocumentosCTE(unidadeDeTrabalho);
            Repositorio.InformacaoCargaCTE repInformacaoCargaCTE = new Repositorio.InformacaoCargaCTE(unidadeDeTrabalho);
            Repositorio.IntegracaoLsTranslog repIntegracao = new Repositorio.IntegracaoLsTranslog(unidadeDeTrabalho);

            Dominio.ObjetosDeValor.LsTranslog.Atividade documento = null;
            Dominio.Entidades.ConhecimentoDeTransporteEletronico cte = null;

            if (integracao.CTe != null)
                cte = integracao.CTe;
            else if (integracao.NFSe != null)
                cte = repCTe.BuscarPorRPS(integracao.NFSe.RPS.Codigo);

            if (cte != null)
            {
                List<Dominio.Entidades.DocumentosCTE> listaNotasFiscais = repDocumentosCTE.BuscarPorCTe(cte.Codigo);

                if (listaNotasFiscais != null && listaNotasFiscais.Count > 0)
                {
                    for (var j = 0; j < listaNotasFiscais.Count; j++)
                    {
                        List<Dominio.Entidades.ObservacaoContribuinteCTE> listaobsContribuinte = repObsContribuinte.BuscarPorCTe(cte.Empresa.Codigo, cte.Codigo);

                        Dominio.Entidades.ObservacaoContribuinteCTE obsPedido = null;
                        if (listaobsContribuinte != null && listaobsContribuinte.Count > 0)
                            obsPedido = (from o in listaobsContribuinte where o.Identificador.Equals("PEDIDO") select o).FirstOrDefault();

                        if (obsPedido != null)
                        {
                            string pedido = obsPedido.Descricao.Length > 8 ? obsPedido.Descricao.Substring(3, obsPedido.Descricao.Length - 3) : obsPedido.Descricao;

                            //for (var k = 0; k < listaNotasFiscais[j].Volume; k++)
                            //{
                            //string identificador = string.Concat(pedido, string.Format("{0:D3}", k + 1), string.Format("{0:D3}", listaNotasFiscais[j].Volume));

                            string identificador = pedido;
                            documento = new Dominio.ObjetosDeValor.LsTranslog.Atividade
                            {
                                Identificador = identificador,
                                OS = identificador,
                                IDLocal = 3538,
                                Observacoes = "Mandae/MultiCTe",
                                IDTipo = "6",
                                Rastreio = pedido,
                                NotaFiscal = listaNotasFiscais[j].Numero,
                                NumeroCTE = cte.Numero.ToString(),
                                NumeroDT = "",
                                Lote = "",
                                Peso = listaNotasFiscais[j].Peso > 0 ? listaNotasFiscais[j].Peso.ToString("n2") : repInformacaoCargaCTE.ObterPesoKg(cte.Codigo).ToString("n2"),
                                CNPJEmissor = cte.Remetente?.CPF_CNPJ ?? string.Empty,
                                ChaveNFE = !string.IsNullOrWhiteSpace(listaNotasFiscais[j].ChaveNFE) ? listaNotasFiscais[j].ChaveNFE : string.Empty,
                                Cliente = new Dominio.ObjetosDeValor.LsTranslog.Cliente
                                {
                                    Nome = cte.Destinatario.Nome,
                                    Documento = cte.Destinatario.CPF_CNPJ_SemFormato,
                                    Endereco = cte.Destinatario.Endereco,
                                    Numero = cte.Destinatario.Numero,
                                    Cep = Utilidades.String.OnlyNumbers(cte.Destinatario.CEP),
                                    Bairro = cte.Destinatario.Bairro,
                                    Complemento = cte.Destinatario.Complemento,
                                    Cidade = cte.Destinatario.Localidade.Descricao,
                                    Telefone = cte.Destinatario.Telefone1
                                },
                                Valor = listaNotasFiscais[j].Valor > 0 ? listaNotasFiscais[j].Valor : Math.Round(cte.ValorTotalMercadoria / listaNotasFiscais.Count(), 2, MidpointRounding.ToEven)
                                //Tipo = "D+1"
                            };

                            Repositorio.IntegracaoLsTranslogLog repIntegracaoLog = new Repositorio.IntegracaoLsTranslogLog(unidadeDeTrabalho);

                            Dominio.Entidades.IntegracaoLsTranslogLog integracaoLog = new Dominio.Entidades.IntegracaoLsTranslogLog();
                            integracaoLog.IntegracaoLsTranslog = integracao;
                            integracaoLog.Data = DateTime.Now;
                            integracaoLog.Tipo = Dominio.ObjetosDeValor.Enumerador.TipoIntegracaoLsTranslog.Envio;
                            integracaoLog.Status = Dominio.ObjetosDeValor.Enumerador.StatusIntegracaoLsTranslog.Pendente;
                            integracaoLog.Identificador = identificador;
                            integracaoLog.NumeroNFe = int.Parse(listaNotasFiscais[j].Numero);

                            repIntegracaoLog.Inserir(integracaoLog);

                            Dominio.ObjetosDeValor.LsTranslog.AtividadeReturn retorno = this.Enviar(documento, token, integracaoLog, unidadeDeTrabalho);

                            if (retorno != null)
                            {
                                integracaoLog.Identificador = retorno.Identificador;
                                repIntegracaoLog.Atualizar(integracaoLog);
                            }
                            else
                            {
                                integracao.StatusEnvio = Dominio.ObjetosDeValor.Enumerador.StatusIntegracaoLsTranslog.Erro;
                                repIntegracao.Atualizar(integracao);

                                return false;
                            }
                            //}
                        }
                        else
                        {
                            Servicos.Log.TratarErro("EnviarDocumento: Codigo " + integracao.Codigo + ": Não localizado numero pedido Mandae.", "LsTranslog");

                            integracao.ObservacaoEnvio = "EnviarDocumento: Não localizado número pedido Mandae.";
                            integracao.StatusEnvio = Dominio.ObjetosDeValor.Enumerador.StatusIntegracaoLsTranslog.Erro;
                            repIntegracao.Atualizar(integracao);

                            return false;
                        }
                    }

                    integracao.StatusEnvio = Dominio.ObjetosDeValor.Enumerador.StatusIntegracaoLsTranslog.Sucesso;
                    repIntegracao.Atualizar(integracao);

                    return true;
                }
                else
                {
                    Servicos.Log.TratarErro("EnviarDocumento: Codigo " + integracao.Codigo + ": Não localizado NFe Mandae.", "LsTranslog");

                    integracao.ObservacaoEnvio = "EnviarDocumento: Não localizado NFe Mandae.";
                    integracao.StatusEnvio = Dominio.ObjetosDeValor.Enumerador.StatusIntegracaoLsTranslog.Erro;
                    repIntegracao.Atualizar(integracao);

                    return false;
                }
            }
            else
            {
                if (integracao.NFe != null)
                {
                    string pedido = integracao.NFe.Pedido.Length > 8 ? integracao.NFe.Pedido.Substring(3, integracao.NFe.Pedido.Length - 3) : integracao.NFe.Pedido; //00007050169 > 07050169

                    //for (var k = 0; k < integracao.NFe.Volumes; k++)
                    //{
                    //    string identificador = string.Concat(pedido, string.Format("{0:D3}", k + 1), string.Format("{0:D3}", integracao.NFe.Volumes));

                    string identificador = pedido;
                    documento = new Dominio.ObjetosDeValor.LsTranslog.Atividade
                    {
                        Identificador = identificador,
                        OS = identificador,
                        IDLocal = 3538,
                        Observacoes = "Mandae/MultiCTe",
                        IDTipo = "6",
                        Rastreio = pedido,
                        NotaFiscal = integracao.NFe.Numero,
                        NumeroCTE = "0",
                        NumeroDT = "",
                        Lote = "",
                        Peso = integracao.NFe.Peso.ToString("n2"),
                        CNPJEmissor = integracao.NFe.Emitente?.CPF_CNPJ_SemFormato ?? string.Empty,
                        ChaveNFE = !string.IsNullOrWhiteSpace(integracao.NFe.Chave) ? integracao.NFe.Chave : string.Empty,
                        Cliente = new Dominio.ObjetosDeValor.LsTranslog.Cliente
                        {
                            Nome = integracao.NFe.Destinatario.Nome,
                            Documento = integracao.NFe.Destinatario.CPF_CNPJ_SemFormato,
                            Endereco = integracao.NFe.Destinatario.Endereco,
                            Numero = integracao.NFe.Destinatario.Numero,
                            Cep = Utilidades.String.OnlyNumbers(integracao.NFe.Destinatario.CEP),
                            Bairro = integracao.NFe.Destinatario.Bairro,
                            Complemento = integracao.NFe.Destinatario.Complemento,
                            Cidade = integracao.NFe.Destinatario.Localidade.Descricao,
                            Telefone = integracao.NFe.Destinatario.Telefone1
                        },
                        Valor = integracao.NFe.Valor
                        //Tipo = "D+1"
                    };

                    Repositorio.IntegracaoLsTranslogLog repIntegracaoLog = new Repositorio.IntegracaoLsTranslogLog(unidadeDeTrabalho);

                    Dominio.Entidades.IntegracaoLsTranslogLog integracaoLog = new Dominio.Entidades.IntegracaoLsTranslogLog();
                    integracaoLog.IntegracaoLsTranslog = integracao;
                    integracaoLog.Data = DateTime.Now;
                    integracaoLog.Tipo = Dominio.ObjetosDeValor.Enumerador.TipoIntegracaoLsTranslog.Envio;
                    integracaoLog.Status = Dominio.ObjetosDeValor.Enumerador.StatusIntegracaoLsTranslog.Pendente;
                    integracaoLog.Identificador = identificador;
                    integracaoLog.NumeroNFe = int.Parse(integracao.NFe.Numero);

                    repIntegracaoLog.Inserir(integracaoLog);

                    Dominio.ObjetosDeValor.LsTranslog.AtividadeReturn retorno = this.Enviar(documento, token, integracaoLog, unidadeDeTrabalho);

                    if (retorno != null)
                    {
                        integracaoLog.Identificador = retorno.Identificador;
                        repIntegracaoLog.Atualizar(integracaoLog);
                    }
                    else
                    {
                        integracao.StatusEnvio = Dominio.ObjetosDeValor.Enumerador.StatusIntegracaoLsTranslog.Erro;
                        repIntegracao.Atualizar(integracao);

                        return false;
                    }
                    //}

                    integracao.StatusEnvio = Dominio.ObjetosDeValor.Enumerador.StatusIntegracaoLsTranslog.Sucesso;
                    repIntegracao.Atualizar(integracao);
                    return true;
                }
                else
                {
                    Servicos.Log.TratarErro("EnviarDocumento: Codigo " + integracao.Codigo + ": Não localizado nenhum documento Cobasi.", "LsTranslog");

                    integracao.ObservacaoEnvio = "EnviarDocumento: Não localizado nenhum documento.";
                    integracao.StatusEnvio = Dominio.ObjetosDeValor.Enumerador.StatusIntegracaoLsTranslog.Erro;
                    repIntegracao.Atualizar(integracao);

                    return false;
                }
            }
        }


        private bool EnviarDocumentoRiachuelo(Dominio.Entidades.IntegracaoLsTranslog integracao, int idLocal, string token, Repositorio.UnitOfWork unidadeDeTrabalho)
        {
            Repositorio.ConhecimentoDeTransporteEletronico repCTe = new Repositorio.ConhecimentoDeTransporteEletronico(unidadeDeTrabalho);
            Repositorio.ObservacaoContribuinteCTE repObsContribuinte = new Repositorio.ObservacaoContribuinteCTE(unidadeDeTrabalho);
            Repositorio.DocumentosCTE repDocumentosCTE = new Repositorio.DocumentosCTE(unidadeDeTrabalho);
            Repositorio.InformacaoCargaCTE repInformacaoCargaCTE = new Repositorio.InformacaoCargaCTE(unidadeDeTrabalho);
            Repositorio.IntegracaoLsTranslog repIntegracao = new Repositorio.IntegracaoLsTranslog(unidadeDeTrabalho);

            Dominio.ObjetosDeValor.LsTranslog.Atividade documento = null;
            Dominio.Entidades.ConhecimentoDeTransporteEletronico cte = null;

            if (integracao.CTe != null)
                cte = integracao.CTe;
            else if (integracao.NFSe != null)
                cte = repCTe.BuscarPorRPS(integracao.NFSe.RPS.Codigo);

            if (cte != null)
            {
                List<Dominio.Entidades.DocumentosCTE> listaNotasFiscais = repDocumentosCTE.BuscarPorCTe(cte.Codigo);

                if (listaNotasFiscais != null && listaNotasFiscais.Count > 0)
                {
                    for (var j = 0; j < listaNotasFiscais.Count; j++)
                    {
                        List<Dominio.Entidades.ObservacaoContribuinteCTE> listaobsContribuinte = repObsContribuinte.BuscarPorCTe(cte.Empresa.Codigo, cte.Codigo);

                        Dominio.Entidades.ObservacaoContribuinteCTE obsPedido = null;
                        if (listaobsContribuinte != null && listaobsContribuinte.Count > 0)
                            obsPedido = (from o in listaobsContribuinte where o.Identificador.Equals("PEDIDO") select o).FirstOrDefault();

                        if (obsPedido != null)
                        {
                            string pedido = obsPedido.Descricao.Length > 8 ? obsPedido.Descricao.Substring(3, obsPedido.Descricao.Length - 3) : obsPedido.Descricao;

                            string identificador = pedido;
                            documento = new Dominio.ObjetosDeValor.LsTranslog.Atividade
                            {
                                Identificador = identificador,
                                OS = identificador,
                                IDLocal = idLocal,
                                Observacoes = "Riachuelo/MultiCTe",
                                IDTipo = "6",
                                Rastreio = pedido,
                                NotaFiscal = listaNotasFiscais[j].Numero,
                                NumeroCTE = cte.Numero.ToString(),
                                NumeroDT = "",
                                Lote = "",
                                Peso = listaNotasFiscais[j].Peso > 0 ? listaNotasFiscais[j].Peso.ToString("n2") : repInformacaoCargaCTE.ObterPesoKg(cte.Codigo).ToString("n2"),
                                CNPJEmissor = cte.Remetente?.CPF_CNPJ ?? string.Empty,
                                ChaveNFE = !string.IsNullOrWhiteSpace(listaNotasFiscais[j].ChaveNFE) ? listaNotasFiscais[j].ChaveNFE : string.Empty,
                                Cliente = new Dominio.ObjetosDeValor.LsTranslog.Cliente
                                {
                                    Nome = cte.Destinatario.Nome,
                                    Documento = cte.Destinatario.CPF_CNPJ_SemFormato,
                                    Endereco = cte.Destinatario.Endereco,
                                    Numero = cte.Destinatario.Numero,
                                    Cep = Utilidades.String.OnlyNumbers(cte.Destinatario.CEP),
                                    Bairro = cte.Destinatario.Bairro,
                                    Complemento = cte.Destinatario.Complemento,
                                    Cidade = cte.Destinatario.Localidade.Descricao,
                                    Telefone = cte.Destinatario.Telefone1
                                },
                                Valor = listaNotasFiscais[j].Valor > 0 ? listaNotasFiscais[j].Valor : Math.Round(cte.ValorTotalMercadoria / listaNotasFiscais.Count(), 2, MidpointRounding.ToEven)
                                //Tipo = "D+1"
                            };

                            Repositorio.IntegracaoLsTranslogLog repIntegracaoLog = new Repositorio.IntegracaoLsTranslogLog(unidadeDeTrabalho);

                            Dominio.Entidades.IntegracaoLsTranslogLog integracaoLog = new Dominio.Entidades.IntegracaoLsTranslogLog();
                            integracaoLog.IntegracaoLsTranslog = integracao;
                            integracaoLog.Data = DateTime.Now;
                            integracaoLog.Tipo = Dominio.ObjetosDeValor.Enumerador.TipoIntegracaoLsTranslog.Envio;
                            integracaoLog.Status = Dominio.ObjetosDeValor.Enumerador.StatusIntegracaoLsTranslog.Pendente;
                            integracaoLog.Identificador = identificador;
                            integracaoLog.NumeroNFe = int.Parse(listaNotasFiscais[j].Numero);

                            repIntegracaoLog.Inserir(integracaoLog);

                            Dominio.ObjetosDeValor.LsTranslog.AtividadeReturn retorno = this.Enviar(documento, token, integracaoLog, unidadeDeTrabalho);

                            if (retorno != null)
                            {
                                integracaoLog.Identificador = retorno.Identificador;
                                repIntegracaoLog.Atualizar(integracaoLog);
                            }
                            else
                            {
                                integracao.StatusEnvio = Dominio.ObjetosDeValor.Enumerador.StatusIntegracaoLsTranslog.Erro;
                                repIntegracao.Atualizar(integracao);

                                return false;
                            }
                        }
                        else
                        {
                            Servicos.Log.TratarErro("EnviarDocumento: Codigo " + integracao.Codigo + ": Não localizado numero pedido Riachuelo.", "LsTranslog");

                            integracao.ObservacaoEnvio = "EnviarDocumento: Não localizado número pedido Riachuelo.";
                            integracao.StatusEnvio = Dominio.ObjetosDeValor.Enumerador.StatusIntegracaoLsTranslog.Erro;
                            repIntegracao.Atualizar(integracao);

                            return false;
                        }
                    }

                    integracao.StatusEnvio = Dominio.ObjetosDeValor.Enumerador.StatusIntegracaoLsTranslog.Sucesso;
                    repIntegracao.Atualizar(integracao);

                    return true;
                }
                else
                {
                    Servicos.Log.TratarErro("EnviarDocumento: Codigo " + integracao.Codigo + ": Não localizado NFe Riachuelo.", "LsTranslog");

                    integracao.ObservacaoEnvio = "EnviarDocumento: Não localizado NFe Riachuelo.";
                    integracao.StatusEnvio = Dominio.ObjetosDeValor.Enumerador.StatusIntegracaoLsTranslog.Erro;
                    repIntegracao.Atualizar(integracao);

                    return false;
                }
            }
            else
            {
                if (integracao.NFe != null)
                {
                    string pedido = integracao.NFe.Pedido.Length > 8 ? integracao.NFe.Pedido.Substring(3, integracao.NFe.Pedido.Length - 3) : integracao.NFe.Pedido; //00007050169 > 07050169

                    string identificador = pedido;
                    documento = new Dominio.ObjetosDeValor.LsTranslog.Atividade
                    {
                        Identificador = identificador,
                        OS = identificador,
                        IDLocal = idLocal,
                        Observacoes = "Riachuelo/MultiCTe",
                        IDTipo = "6",
                        Rastreio = pedido,
                        NotaFiscal = integracao.NFe.Numero,
                        NumeroCTE = "0",
                        NumeroDT = "",
                        Lote = "",
                        Peso = integracao.NFe.Peso.ToString("n2"),
                        CNPJEmissor = integracao.NFe.Emitente?.CPF_CNPJ_SemFormato ?? string.Empty,
                        ChaveNFE = !string.IsNullOrWhiteSpace(integracao.NFe.Chave) ? integracao.NFe.Chave : string.Empty,
                        Cliente = new Dominio.ObjetosDeValor.LsTranslog.Cliente
                        {
                            Nome = integracao.NFe.Destinatario.Nome,
                            Documento = integracao.NFe.Destinatario.CPF_CNPJ_SemFormato,
                            Endereco = integracao.NFe.Destinatario.Endereco,
                            Numero = integracao.NFe.Destinatario.Numero,
                            Cep = Utilidades.String.OnlyNumbers(integracao.NFe.Destinatario.CEP),
                            Bairro = integracao.NFe.Destinatario.Bairro,
                            Complemento = integracao.NFe.Destinatario.Complemento,
                            Cidade = integracao.NFe.Destinatario.Localidade.Descricao,
                            Telefone = integracao.NFe.Destinatario.Telefone1
                        },
                        Valor = integracao.NFe.Valor
                        //Tipo = "D+1"
                    };

                    Repositorio.IntegracaoLsTranslogLog repIntegracaoLog = new Repositorio.IntegracaoLsTranslogLog(unidadeDeTrabalho);

                    Dominio.Entidades.IntegracaoLsTranslogLog integracaoLog = new Dominio.Entidades.IntegracaoLsTranslogLog();
                    integracaoLog.IntegracaoLsTranslog = integracao;
                    integracaoLog.Data = DateTime.Now;
                    integracaoLog.Tipo = Dominio.ObjetosDeValor.Enumerador.TipoIntegracaoLsTranslog.Envio;
                    integracaoLog.Status = Dominio.ObjetosDeValor.Enumerador.StatusIntegracaoLsTranslog.Pendente;
                    integracaoLog.Identificador = identificador;
                    integracaoLog.NumeroNFe = int.Parse(integracao.NFe.Numero);

                    repIntegracaoLog.Inserir(integracaoLog);

                    Dominio.ObjetosDeValor.LsTranslog.AtividadeReturn retorno = this.Enviar(documento, token, integracaoLog, unidadeDeTrabalho);

                    if (retorno != null)
                    {
                        integracaoLog.Identificador = retorno.Identificador;
                        repIntegracaoLog.Atualizar(integracaoLog);
                    }
                    else
                    {
                        integracao.StatusEnvio = Dominio.ObjetosDeValor.Enumerador.StatusIntegracaoLsTranslog.Erro;
                        repIntegracao.Atualizar(integracao);

                        return false;
                    }

                    integracao.StatusEnvio = Dominio.ObjetosDeValor.Enumerador.StatusIntegracaoLsTranslog.Sucesso;
                    repIntegracao.Atualizar(integracao);
                    return true;
                }
                else
                {
                    Servicos.Log.TratarErro("EnviarDocumento: Codigo " + integracao.Codigo + ": Não localizado nenhum documento Cobasi.", "LsTranslog");

                    integracao.ObservacaoEnvio = "EnviarDocumento: Não localizado nenhum documento.";
                    integracao.StatusEnvio = Dominio.ObjetosDeValor.Enumerador.StatusIntegracaoLsTranslog.Erro;
                    repIntegracao.Atualizar(integracao);

                    return false;
                }
            }
        }
    }
}
