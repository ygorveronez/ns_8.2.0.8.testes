using AdminMultisoftware.Dominio.Enumeradores;
using Dominio.ObjetosDeValor.Embarcador.EmissorDocumento;
using Servicos.Embarcador.EmissorDocumento;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Microsoft.AspNetCore.Http;
using CoreWCF;


namespace SGT.WebService
{
    [ServiceBehavior(AddressFilterMode = AddressFilterMode.Any, IncludeExceptionDetailInFaults = true)]
    public class IntegracaoCTe(IServiceProvider serviceProvider) : WebServiceBase(serviceProvider), IIntegracaoCTe
    {
        #region Métodos Globais

        public Retorno<bool> IntegrarCteAverbado(Dominio.ObjetosDeValor.WebService.CTe.AverbacaoOracle averbacaoOracle)
        {
            //WebServiceBase.ValidarToken();

            Retorno<bool> retorno = new Retorno<bool>();
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(Conexao.createInstance(_serviceProvider).StringConexao);
            try
            {

                Repositorio.ConhecimentoDeTransporteEletronico repCTe = new Repositorio.ConhecimentoDeTransporteEletronico(unitOfWork);
                Dominio.Entidades.ConhecimentoDeTransporteEletronico cte = repCTe.BuscarPorProtocoloIntegracaoOracle(averbacaoOracle.CodigoCTeInterno);

                if (cte == null)
                {
                    retorno.DataRetorno = DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss");
                    retorno.Status = false;
                    retorno.Objeto = false;
                    retorno.Mensagem = "CT-e não localizado pelo codigo interno informado (" + averbacaoOracle.CodigoCTeInterno + ")";
                    return retorno;
                }

                Repositorio.AverbacaoCTe repAverbacao = new Repositorio.AverbacaoCTe(unitOfWork);

                List<Dominio.Entidades.AverbacaoCTe> averbacoesExistentes = repAverbacao.BuscarPorCTe(cte.Empresa.Codigo, cte.Codigo);

                if (averbacoesExistentes == null || averbacoesExistentes.Count < 0)
                {
                    retorno.DataRetorno = DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss");
                    retorno.Status = false;
                    retorno.Objeto = false;
                    retorno.Mensagem = "Não foram localizadas averbações para o cte (" + cte.Codigo + ")";
                    return retorno;
                }

                foreach (Dominio.ObjetosDeValor.WebService.CTe.AverbacaoCTe averbacaoIntegrada in averbacaoOracle.Averbacoes)
                {
                    var averbacao = (from obj in averbacoesExistentes where obj.CodigoIntegracao == averbacaoIntegrada.CodigoAverbacao select obj).FirstOrDefault();

                    if (averbacao == null)
                    {
                        retorno.DataRetorno = DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss");
                        retorno.Status = false;
                        retorno.Objeto = false;
                        retorno.Mensagem = "Não foi localizada uma averbação para o protocolo " + averbacaoIntegrada.CodigoAverbacao;
                        return retorno;
                    }

                    averbacao.CodigoIntegracao = averbacaoIntegrada.CodigoAverbacao;
                    averbacao.CodigoRetorno = averbacaoIntegrada.CodigoRetorno;
                    averbacao.CTe = cte;
                    averbacao.DataRetorno = averbacaoIntegrada.DataProtocolo;
                    averbacao.MensagemRetorno = averbacaoIntegrada.MensagemRetorno;
                    averbacao.Protocolo = averbacaoIntegrada.NumeroProtocolo;

                    if (averbacaoIntegrada.Tipo == "A")
                    {
                        averbacao.Tipo = Dominio.Enumeradores.TipoAverbacaoCTe.Autorizacao;

                        if (averbacaoIntegrada.Status == "A")
                            averbacao.Status = Dominio.Enumeradores.StatusAverbacaoCTe.Sucesso;
                        else if (averbacaoIntegrada.Status == "I")
                            averbacao.Status = Dominio.Enumeradores.StatusAverbacaoCTe.Pendente;
                        else
                            averbacao.Status = Dominio.Enumeradores.StatusAverbacaoCTe.Rejeicao;
                    }
                    else if (averbacaoIntegrada.Tipo == "C")
                    {
                        if (averbacaoIntegrada.Status == "A")
                        {
                            averbacao.Tipo = Dominio.Enumeradores.TipoAverbacaoCTe.Cancelamento;
                            averbacao.Status = Dominio.Enumeradores.StatusAverbacaoCTe.Cancelado;
                        }
                        else
                        {
                            //volta o status para sucesso pois tenta imitiar a mesma logica do cte, onde se o cancelamento for rejeitado o mesmo fica com status autorizado
                            averbacao.Status = Dominio.Enumeradores.StatusAverbacaoCTe.Sucesso;
                        }
                    }

                    averbacao.SeguradoraAverbacao = averbacaoIntegrada.Seguradora == "A" ? Dominio.Enumeradores.IntegradoraAverbacao.ATM :
                                                    averbacaoIntegrada.Seguradora == "B" ? Dominio.Enumeradores.IntegradoraAverbacao.Quorum :
                                                    averbacaoIntegrada.Seguradora == "P" ? Dominio.Enumeradores.IntegradoraAverbacao.PortoSeguro :
                                                    Dominio.Enumeradores.IntegradoraAverbacao.NaoDefinido;

                    averbacao.Averbacao = averbacaoIntegrada.NumeroProtocolo;

                    repAverbacao.Atualizar(averbacao);
                }

                retorno.DataRetorno = DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss");
                retorno.Status = true;
                retorno.Objeto = true;

                Servicos.Auditoria.Auditoria.Auditar(Auditado, cte, "Integrou CT-e averbado", unitOfWork);

                return retorno;
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);

                retorno.DataRetorno = DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss");
                retorno.Status = false;
                retorno.CodigoMensagem = Dominio.ObjetosDeValor.Embarcador.Enumeradores.CodigoMensagemRetorno.FalhaGenerica;
                retorno.Objeto = false;
                retorno.Mensagem = "Ocorreu uma falha ao informar o CTe";
                return retorno;
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        public Retorno<bool> IntegrarCTeAutorizado(Dominio.ObjetosDeValor.WebService.CTe.CTeOracle cteOracle)
        {
            //WebServiceBase.ValidarToken();

            Retorno<bool> retorno = new Retorno<bool>();
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(Conexao.createInstance(_serviceProvider).StringConexao);

            try
            {
                Repositorio.ConhecimentoDeTransporteEletronico repCTe = new Repositorio.ConhecimentoDeTransporteEletronico(unitOfWork);
                Dominio.Entidades.ConhecimentoDeTransporteEletronico cte = cteOracle.CodigoCTeInterno > 0 ? repCTe.BuscarCTePorProtocoloIntegracaoOracle(cteOracle.CodigoCTeInterno) : null;

                if (cte == null && !string.IsNullOrWhiteSpace(cteOracle.ChaveCTE))
                {
                    cte = repCTe.BuscarPorChave(cteOracle.ChaveCTE);

                    if (cte != null)
                        cte.CodigoCTeIntegrador = cteOracle.CodigoCTeInterno;
                }

                if (cte == null && cteOracle.CodigoInutilizacao > 0)
                    cte = repCTe.BuscarPorCodigoInutilizacao(cteOracle.CodigoInutilizacao.ToString());

                if (cte == null)
                {
                    retorno.DataRetorno = DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss");
                    retorno.Status = false;
                    retorno.CodigoMensagem = Dominio.ObjetosDeValor.Embarcador.Enumeradores.CodigoMensagemRetorno.FalhaGenerica;
                    retorno.Objeto = false;
                    retorno.Mensagem = "CTe " + cteOracle.ChaveCTE + " não localizado na base SqlServer";
                    return retorno;
                }

                RetornoEventoCTe retornoEventoCTe = new RetornoEventoCTe();
                retornoEventoCTe.cteOracle = cteOracle;
                
                string mensagemErro = string.Empty;
                EmissorDocumentoService.GetEmissorDocumentoCTe(Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoEmissorDocumento.Integrador).ReceberEventoCte(out mensagemErro, cte, retornoEventoCTe, Auditado, TipoServicoMultisoftware, unitOfWork);

                retorno.DataRetorno = DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss");
                retorno.Status = true;
                retorno.Objeto = true;

                return retorno;
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);

                ArmazenarLogIntegracao(cteOracle, unitOfWork);

                retorno.DataRetorno = DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss");
                retorno.Status = false;
                retorno.CodigoMensagem = Dominio.ObjetosDeValor.Embarcador.Enumeradores.CodigoMensagemRetorno.FalhaGenerica;
                retorno.Objeto = false;
                retorno.Mensagem = "Ocorreu uma falha ao informar o CTe";
                return retorno;
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        public Retorno<bool> IntegrarCartaCorrecaoAutorizada(Dominio.ObjetosDeValor.WebService.CTe.CTeOracle cceOracle)
        {
            //WebServiceBase.ValidarToken();

            Retorno<bool> retorno = new Retorno<bool>();
            using (Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(Conexao.createInstance(_serviceProvider).StringConexao))
            {
                if (!Servicos.Embarcador.EmissorDocumento.EmissorDocumentoService.GetEmissorDocumentoCTe(Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoEmissorDocumento.Integrador).ReceberEventoCCe(out string mensagemErro, out Exception exception, cceOracle, Auditado, unitOfWork))
                {
                    if (exception != null)
                        ArmazenarLogIntegracao(cceOracle, unitOfWork);

                    retorno.DataRetorno = DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss");
                    retorno.Status = false;
                    retorno.CodigoMensagem = Dominio.ObjetosDeValor.Embarcador.Enumeradores.CodigoMensagemRetorno.FalhaGenerica;
                    retorno.Objeto = false;
                    retorno.Mensagem = mensagemErro;
                    return retorno;
                }
            }

            retorno.DataRetorno = DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss");
            retorno.Status = true;
            retorno.Objeto = true;
            return retorno;
        }

        public Retorno<bool> IntegrarLogEnvioEmail(Dominio.ObjetosDeValor.WebService.CTe.LogEnvioEmail email)
        {
            Retorno<bool> retorno = new Retorno<bool>();
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(Conexao.createInstance(_serviceProvider).StringConexao);
            try
            {

                Repositorio.Embarcador.Email.LogEnvioEmail repLogEnvioEmail = new Repositorio.Embarcador.Email.LogEnvioEmail(unitOfWork);
                Repositorio.Empresa repEmpresa = new Repositorio.Empresa(unitOfWork);

                Dominio.Entidades.Embarcador.Email.LogEnvioEmail logEnvioEmail = new Dominio.Entidades.Embarcador.Email.LogEnvioEmail();

                logEnvioEmail.Data = email.Data;
                logEnvioEmail.EmailRemetente = email.EmailRemetente;
                logEnvioEmail.EmailDestinatario = email.EmailDestinatario;
                if (!string.IsNullOrEmpty(email.EmailResposta))
                    logEnvioEmail.EmailResposta = email.EmailResposta;
                if (!string.IsNullOrEmpty(email.EmailCopia))
                    logEnvioEmail.EmailCopia = email.EmailCopia;
                if (!string.IsNullOrEmpty(email.EmailCopiaOculta))
                    logEnvioEmail.EmailCopiaOculta = email.EmailCopiaOculta;
                logEnvioEmail.DescricaoAnexo = email.DescricaoAnexo;
                logEnvioEmail.Assunto = email.Assunto;
                logEnvioEmail.IdentificacaoEmail = email.IdentificacaoEmail;

                if (!string.IsNullOrEmpty(email.Mensagem))
                {
                    string bodyFormated = "";
                    Regex regex = new Regex(@"(<br />|<br/>|</ br>|</br>)");
                    bodyFormated = regex.Replace(email.Mensagem, "\r\n");
                    bodyFormated = Regex.Replace(bodyFormated, "<.*?>", String.Empty);

                    logEnvioEmail.Mensagem = bodyFormated;
                }

                repLogEnvioEmail.Inserir(logEnvioEmail);

                retorno.DataRetorno = DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss");
                retorno.Status = true;
                retorno.Objeto = true;

                return retorno;
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);

                retorno.DataRetorno = DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss");
                retorno.Status = false;
                retorno.CodigoMensagem = Dominio.ObjetosDeValor.Embarcador.Enumeradores.CodigoMensagemRetorno.FalhaGenerica;
                retorno.Objeto = false;
                retorno.Mensagem = "Ocorreu uma falha ao integrar log envio e-mail";
                return retorno;
            }
            finally
            {
                unitOfWork.Dispose();
            }



        }

        #endregion

        #region Métodos Protegidos Sobrescritos

        protected override Dominio.ObjetosDeValor.Enumerador.OrigemAuditado ObterOrigemAuditado()
        {
            return Dominio.ObjetosDeValor.Enumerador.OrigemAuditado.WebServiceIntegracaoCTe;
        }

        #endregion

        #region Métodos Privados

        private bool AdicionarAverbacaoNaFilaDeConsulta(Dominio.Entidades.ConhecimentoDeTransporteEletronico cte)
        {
            try
            {
                string postData = "CodigoCTe=" + (cte != null ? cte.Codigo : 0);
                byte[] bytes = Encoding.UTF8.GetBytes(postData);

                HttpWebRequest request = (HttpWebRequest)WebRequest.Create(string.Concat(Servicos.Embarcador.Configuracoes.ConfigurationInstance.GetInstance(null).ObterConfiguracaoAmbiente().WebServiceConsultaCTe, "IntegracaoCTe/AdicionarAverbacaoNaFilaDeConsulta"));

                request.Method = "POST";
                request.ContentType = "application/x-www-form-urlencoded";
                request.ContentLength = bytes.Length;

                Stream requestStream = request.GetRequestStream();
                requestStream.Write(bytes, 0, bytes.Length);

                WebResponse response = request.GetResponse();

                Stream stream = response.GetResponseStream();

                StreamReader reader = new StreamReader(stream);
                var result = reader.ReadToEnd();

                stream.Dispose();
                reader.Dispose();

                var retorno = (System.Collections.Generic.Dictionary<string, object>)JsonConvert.DeserializeObject(result);


                return (bool)retorno["Sucesso"];
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);
                return false;
            }
        }

        #endregion
    }
}
