using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;

namespace EmissaoCTe.API.Controllers
{
    public class AverbacaoMDFeController : ApiController
    {
        [AcceptVerbs("POST")]
        public ActionResult BuscarPorMDFe()
        {
            Repositorio.UnitOfWork unidadeDeTrabalho = new Repositorio.UnitOfWork(Conexao.StringConexao);
            try
            {
                int codigoMDFe;
                int.TryParse(Request.Params["CodigoMDFe"], out codigoMDFe);
                
                Servicos.AverbacaoMDFe svcAverbacao = new Servicos.AverbacaoMDFe(unidadeDeTrabalho);

                svcAverbacao.ConsultarAverbacoes(codigoMDFe, unidadeDeTrabalho);

                Repositorio.AverbacaoMDFe repAverbacao = new Repositorio.AverbacaoMDFe(unidadeDeTrabalho);

                List<Dominio.Entidades.AverbacaoMDFe> averbacoes = repAverbacao.BuscarPorMDFe(this.EmpresaUsuario.Codigo, codigoMDFe);

                var retorno = (from obj in averbacoes
                               orderby obj.CodigoIntegracao descending
                               select new
                               {
                                   Data = obj.DataRetorno != null ? obj.DataRetorno.Value.ToString("dd/MM/yyyy HH:mm:ss") : string.Empty,
                                   Tipo = obj.DescricaoTipo,
                                   Protocolo = obj.Status == Dominio.Enumeradores.StatusAverbacaoMDFe.Sucesso && !string.IsNullOrWhiteSpace(obj.Averbacao) ? obj.Averbacao : obj.Protocolo ?? string.Empty,
                                   Status = obj.DescricaoStatus,
                                   Mensagem = obj.CodigoRetorno + " - " + obj.MensagemRetorno
                                   //Seguradora = obj.SeguradoraAverbacao == Dominio.Enumeradores.SeguradoraAverbacao.ATM ? "ATM" : obj.SeguradoraAverbacao == Dominio.Enumeradores.SeguradoraAverbacao.Bradesco ? "Bradesco" : string.Empty,
                                   //Averbacao = obj.Averbacao ?? string.Empty
                               }).ToList();

                return Json(retorno, true, null, new string[] { "Data|20", "Tipo|15", "Protocolo|15", "Status|15", "Mensagem|35" }, retorno.Count);
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);

                return Json<bool>(false, false, "Ocorreu uma falha ao obter os dados de averbação do MDF-e.");
            }
            finally
            {
                unidadeDeTrabalho.Dispose();
            }
        
        }

        [AcceptVerbs("POST")]
        public ActionResult ReenviarAverbacao()
        {
            Repositorio.UnitOfWork unidadeDeTrabalho = new Repositorio.UnitOfWork(Conexao.StringConexao);

            try
            {
                int codigoMDFe;
                int.TryParse(Request.Params["CodigoMDFe"], out codigoMDFe);

                Repositorio.ManifestoEletronicoDeDocumentosFiscais repMDFe = new Repositorio.ManifestoEletronicoDeDocumentosFiscais(unidadeDeTrabalho);

                Dominio.Entidades.ManifestoEletronicoDeDocumentosFiscais mdfe = repMDFe.BuscarPorCodigo(codigoMDFe);

                if (mdfe == null)
                    return Json<bool>(false, false, "MDF-e não encontrado. Atualize a página e tente novamente.");

                if (mdfe.Status != Dominio.Enumeradores.StatusMDFe.Autorizado && mdfe.Status != Dominio.Enumeradores.StatusMDFe.Cancelado && mdfe.Status != Dominio.Enumeradores.StatusMDFe.Encerrado)
                    return Json<bool>(false, false, "O status do MDF-e não permite a solicitação de averbação.");

                //Bradesco não tem configuração
                //if (this.EmpresaUsuario.Configuracao == null || string.IsNullOrWhiteSpace(this.EmpresaUsuario.Configuracao.CodigoSeguroATM) || string.IsNullOrWhiteSpace(this.EmpresaUsuario.Configuracao.SenhaSeguroATM) || string.IsNullOrWhiteSpace(this.EmpresaUsuario.Configuracao.UsuarioSeguroATM))
                //    return Json<bool>(false, false, "O transportador não está configurado para solicitar averbações.");

                Repositorio.AverbacaoMDFe repAverbacao = new Repositorio.AverbacaoMDFe(unidadeDeTrabalho);

                if (repAverbacao.ContarPorMDFeEStatus(mdfe.Codigo, Dominio.Enumeradores.StatusAverbacaoMDFe.Pendente) > 0)
                    return Json<bool>(false, false, "Já existe uma averbação pendente para este MDF-e.");

                if (repAverbacao.ContarPorMDFeTipoEStatus(mdfe.Codigo, mdfe.Status == Dominio.Enumeradores.StatusMDFe.Autorizado ? Dominio.Enumeradores.TipoAverbacaoMDFe.Autorizacao : mdfe.Status == Dominio.Enumeradores.StatusMDFe.Encerrado ? Dominio.Enumeradores.TipoAverbacaoMDFe.Encerramento : Dominio.Enumeradores.TipoAverbacaoMDFe.Cancelamento, new Dominio.Enumeradores.StatusAverbacaoMDFe[] { Dominio.Enumeradores.StatusAverbacaoMDFe.Sucesso, Dominio.Enumeradores.StatusAverbacaoMDFe.Pendente }) > 0)
                    return Json<bool>(false, false, "Já existe uma averbação pendente/emitida para este MDF-e.");

                Servicos.AverbacaoMDFe svcAverbacao = new Servicos.AverbacaoMDFe(unidadeDeTrabalho);

                if (!svcAverbacao.Emitir(mdfe, mdfe.Status == Dominio.Enumeradores.StatusMDFe.Autorizado ? Dominio.Enumeradores.TipoAverbacaoMDFe.Autorizacao : mdfe.Status == Dominio.Enumeradores.StatusMDFe.Encerrado ? Dominio.Enumeradores.TipoAverbacaoMDFe.Encerramento : Dominio.Enumeradores.TipoAverbacaoMDFe.Cancelamento, unidadeDeTrabalho))
                    return Json<bool>(false, false, "Não foi possível solicitar uma nova averbação.");

                return Json<bool>(true, true);
            }
            catch (Exception ex)
            {
                unidadeDeTrabalho.Rollback();

                Servicos.Log.TratarErro(ex);

                return Json<bool>(false, false, "Ocorreu uma falha ao solicitar a averbação.");
            }
            finally
            {
                unidadeDeTrabalho.Dispose();
            }
        }
    }
}
