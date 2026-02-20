using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;

namespace EmissaoCTe.API.Controllers
{
    public class SMViagemMDFeController : ApiController
    {
        [AcceptVerbs("POST")]
        public ActionResult BuscarPorMDFe()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(Conexao.StringConexao);

            try
            {
                int codigoMDFe;
                int.TryParse(Request.Params["CodigoMDFe"], out codigoMDFe);

                Repositorio.SMViagemMDFe repSMViagemMDFe = new Repositorio.SMViagemMDFe(unitOfWork);

                List<Dominio.Entidades.SMViagemMDFe> listaSMViagemMDFe = repSMViagemMDFe.BuscarPorMDFe(codigoMDFe);

                var retorno = (from obj in listaSMViagemMDFe
                               orderby obj.Codigo descending
                               select new
                               {
                                   Codigo = obj.Codigo,
                                   Integradora = obj.DescricaoIntegradora,
                                   Tipo = obj.DescricaoTipo,
                                   Status = obj.DescricaoStatus,
                                   Mensagem = !string.IsNullOrWhiteSpace(obj.Mensagem) ? Utilidades.String.RemoveSpecialCharacters(obj.Mensagem) : string.Empty,
                                   Viagem = !string.IsNullOrWhiteSpace(obj.CodigoIntegracaoViagem) ? obj.CodigoIntegracaoViagem : string.Empty,
                               }).ToList();

                return Json(retorno, true, null, new string[] { "Codigo", "Integradora|15", "Tipo|15", "Status|15", "Mensagem|20", "Viagem|25" }, retorno.Count);
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);

                return Json<bool>(false, false, "Ocorreu uma falha ao obter os dados de integração de SM do MDF-e.");
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        [AcceptVerbs("POST")]
        public ActionResult BuscarLogIntegracao()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(Conexao.StringConexao);

            try
            {
                int codigo;
                int.TryParse(Request.Params["Codigo"], out codigo);

                Repositorio.SMViagemMDFeLog repSMViagemMDFeLog = new Repositorio.SMViagemMDFeLog(unitOfWork);

                List<Dominio.Entidades.SMViagemMDFeLog> listaSMViagemMDFeLog = repSMViagemMDFeLog.BuscarPorSMViagemMDFe(codigo);

                var retorno = (from obj in listaSMViagemMDFeLog
                               orderby obj.Codigo descending
                               select new
                               {
                                   Codigo = obj.Codigo,
                                   Data = obj.DataHora.ToString("dd/MM/yyyy HH:mm:ss")
                               }).ToList();

                return Json(retorno, true, null, new string[] { "Codigo", "Data Hora|70" }, retorno.Count);
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);

                return Json<bool>(false, false, "Ocorreu uma falha ao obter os dados de log de integrações SM do MDF-e.");
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        [AcceptVerbs("POST")]
        public ActionResult ReenviarIntegracao()
        {
            Repositorio.UnitOfWork unidadeDeTrabalho = new Repositorio.UnitOfWork(Conexao.StringConexao);

            try
            {
                int codigoMDFe;
                int.TryParse(Request.Params["CodigoMDFe"], out codigoMDFe);

                Repositorio.ManifestoEletronicoDeDocumentosFiscais repMDFe = new Repositorio.ManifestoEletronicoDeDocumentosFiscais(unidadeDeTrabalho);
                Repositorio.SMViagemMDFe repSMViagemMDFe = new Repositorio.SMViagemMDFe(unidadeDeTrabalho);

                Dominio.Entidades.ManifestoEletronicoDeDocumentosFiscais mdfe = repMDFe.BuscarPorCodigo(codigoMDFe);
                if (mdfe == null)
                    return Json<bool>(false, false, "MDF-e não encontrado.");

                if (mdfe.Status != Dominio.Enumeradores.StatusMDFe.Autorizado && mdfe.Status != Dominio.Enumeradores.StatusMDFe.Encerrado)
                    return Json<bool>(false, false, "Situação do MDF-e não permite enviar integração de SM.");

                Dominio.Entidades.SMViagemMDFe SMViagemMDFe = repSMViagemMDFe.BuscarPrimeiroPorMDFe(codigoMDFe);                

                if (SMViagemMDFe == null)
                {
                    SMViagemMDFe = new Dominio.Entidades.SMViagemMDFe();
                    SMViagemMDFe.MDFe = mdfe;
                    SMViagemMDFe.Status = Dominio.Enumeradores.StatusIntegracaoSM.Pendente;

                    if (mdfe.Empresa.Configuracao.IntegradoraSM.HasValue && mdfe.Empresa.Configuracao.IntegradoraSM == Dominio.Enumeradores.IntegradoraSM.Buonny)
                        SMViagemMDFe.Integradora = Dominio.Enumeradores.IntegradoraSM.Buonny;
                    else
                        SMViagemMDFe.Integradora = Dominio.Enumeradores.IntegradoraSM.Trafegus;

                    repSMViagemMDFe.Inserir(SMViagemMDFe);
                }

                if (SMViagemMDFe == null)
                    return Json<bool>(false, false, "Integração não encontrada. Atualize a página e tente novamente.");

                if (SMViagemMDFe.Status != Dominio.Enumeradores.StatusIntegracaoSM.Rejeitado && SMViagemMDFe.Status != Dominio.Enumeradores.StatusIntegracaoSM.Pendente)
                    return Json<bool>(false, false, "Somente é permitido reenviar uma integração com status de Rejeição ou Pendente.");

                if (this.EmpresaUsuario.Configuracao == null && this.EmpresaUsuario.EmpresaPai?.Configuracao == null)
                    return Json<bool>(false, false, "O transportador não possui configurações para integração.");
                
                if (SMViagemMDFe.Integradora == Dominio.Enumeradores.IntegradoraSM.Buonny)
                    Servicos.Buonny.EnviarViagem(ref SMViagemMDFe, unidadeDeTrabalho);                   
                else
                    Servicos.Trafegus.EnviarViagem(ref SMViagemMDFe, unidadeDeTrabalho);

                return Json<bool>(true, true);
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);

                return Json<bool>(false, false, "Ocorreu uma falha ao solicitar a integração.");
            }
            finally
            {
                unidadeDeTrabalho.Dispose();
            }
        }

        [AcceptVerbs("POST", "GET")]
        public ActionResult DownloadArquivoLog()
        {
            Repositorio.UnitOfWork unidadeDeTrabalho = new Repositorio.UnitOfWork(Conexao.StringConexao);
            try
            {
                int.TryParse(Request.Params["Codigo"], out int codigo);
                int.TryParse(Request.Params["Tipo"], out int tipo);

                if (codigo > 0)
                {
                    Repositorio.SMViagemMDFeLog repSMViagemMDFeLog = new Repositorio.SMViagemMDFeLog(unidadeDeTrabalho);

                    Dominio.Entidades.SMViagemMDFeLog xml = repSMViagemMDFeLog.BuscarPorCodigo(codigo);

                    if (xml != null)
                    {
                        byte[] data = null;
                        if (tipo == 0)
                            data = System.Text.Encoding.UTF8.GetBytes(xml.Requisicao);
                        else
                            data = System.Text.Encoding.UTF8.GetBytes(xml.Resposta);

                        string extensao = xml.SMViagemMDFe.Integradora == Dominio.Enumeradores.IntegradoraSM.Trafegus ? ".json" : ".xml";

                        if (data != null)
                        {
                            if (tipo == 0)
                                return Arquivo(data, "text/json", string.Concat("Requisicao" + xml.Codigo, extensao));
                            else
                                return Arquivo(data, "text/json", string.Concat("Resposta" + xml.Codigo, extensao));
                        }
                        else
                            return Json<bool>(false, false, "Ocorreu uma falha ao carregar arquivo, atualize a página e tente novamente.");
                    }
                    else
                        return Json<bool>(false, false, "Nenhum arquivo log salvo.");
                }
                else
                    return Json<bool>(false, false, "Integração não encontrada, atualize a página e tente novamente.");
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);
                return Json<bool>(false, false, "Ocorreu uma falha ao realizar o download.");
            }
            finally
            {
                unidadeDeTrabalho.Dispose();
            }
        }
        
    }
}