using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;

namespace EmissaoCTe.API.Controllers
{
    public class AverbacaoCTeController : ApiController
    {
        [AcceptVerbs("POST")]
        public ActionResult BuscarPorCTe()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(Conexao.StringConexao);

            try
            {
                int codigoCTe;
                int.TryParse(Request.Params["CodigoCTe"], out codigoCTe);

                Repositorio.AverbacaoCTe repAverbacao = new Repositorio.AverbacaoCTe(unitOfWork);

                List<Dominio.Entidades.AverbacaoCTe> averbacoes = repAverbacao.BuscarPorCTe(this.EmpresaUsuario.Codigo, codigoCTe);

                var retorno = (from obj in averbacoes
                               orderby obj.CodigoIntegracao descending
                               select new
                               {
                                   NumeroAverbacao = obj.Averbacao,
                                   Data = obj.DataRetorno != null ? obj.DataRetorno.Value.ToString("dd/MM/yyyy HH:mm:ss") : string.Empty,
                                   Tipo = obj.DescricaoTipo,
                                   Protocolo = obj.Status == Dominio.Enumeradores.StatusAverbacaoCTe.Sucesso && !string.IsNullOrWhiteSpace(obj.Averbacao) ? obj.Averbacao : obj.Protocolo ?? string.Empty,
                                   Status = obj.DescricaoStatus,
                                   Mensagem = obj.CodigoRetorno + " - " + obj.MensagemRetorno,
                                   Seguradora = obj.SeguradoraAverbacao == Dominio.Enumeradores.IntegradoraAverbacao.ATM ? "ATM" : obj.SeguradoraAverbacao == Dominio.Enumeradores.IntegradoraAverbacao.Quorum ? "Quorum" : obj.SeguradoraAverbacao == Dominio.Enumeradores.IntegradoraAverbacao.PortoSeguro ? "Porto Seguro" : string.Empty
                               }).ToList();

                return Json(retorno, true, null, new string[] { "NumeroAverbacao", "Data|10", "Tipo|12", "Protocolo|15", "Status|10", "Mensagem|30", "Seguradora|13" }, retorno.Count);
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);

                return Json<bool>(false, false, "Ocorreu uma falha ao obter os dados de averbação do CT-e.");
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        [AcceptVerbs("POST")]
        public ActionResult ReenviarAverbacao()
        {
            Repositorio.UnitOfWork unidadeDeTrabalho = new Repositorio.UnitOfWork(Conexao.StringConexao);

            try
            {
                int codigoCTe;
                int.TryParse(Request.Params["CodigoCTe"], out codigoCTe);

                Repositorio.ConhecimentoDeTransporteEletronico repCTe = new Repositorio.ConhecimentoDeTransporteEletronico(unidadeDeTrabalho);

                Dominio.Entidades.ConhecimentoDeTransporteEletronico cte = repCTe.BuscarPorCodigo(codigoCTe);

                if (cte == null)
                    return Json<bool>(false, false, "CT-e não encontrado. Atualize a página e tente novamente.");

                if (cte.Status != "C" && cte.Status != "A")
                    return Json<bool>(false, false, "O status do CT-e não permite a solicitação de averbação.");

                if (this.EmpresaUsuario.Configuracao == null && this.EmpresaUsuario.EmpresaPai?.Configuracao == null)
                    return Json<bool>(false, false, "O transportador não possui configurações para averbação.");

                //if (string.IsNullOrWhiteSpace(this.EmpresaUsuario.Configuracao.UsuarioSeguroATM) && string.IsNullOrWhiteSpace(this.EmpresaUsuario.EmpresaPai?.Configuracao.UsuarioSeguroATM) && string.IsNullOrWhiteSpace(this.EmpresaUsuario.Configuracao.TokenAverbacaoBradesco) && string.IsNullOrWhiteSpace(this.EmpresaUsuario.EmpresaPai?.Configuracao.TokenAverbacaoBradesco))
                if (this.EmpresaUsuario.Configuracao.SeguradoraAverbacao == Dominio.Enumeradores.IntegradoraAverbacao.NaoDefinido && this.EmpresaUsuario.EmpresaPai.Configuracao.SeguradoraAverbacao == Dominio.Enumeradores.IntegradoraAverbacao.NaoDefinido)
                    return Json<bool>(false, false, "O transportador não está configurado para solicitar averbações.");

                Repositorio.AverbacaoCTe repAverbacao = new Repositorio.AverbacaoCTe(unidadeDeTrabalho);

                //if (repAverbacao.ContarPorCTeEStatus(cte.Codigo, Dominio.Enumeradores.StatusAverbacaoCTe.Pendente) > 0)
                //    return Json<bool>(false, false, "Já existe uma averbação pendente para este CT-e.");

                if (repAverbacao.ContarPorCTeTipoEStatus(cte.Codigo, cte.Status == "A" ? Dominio.Enumeradores.TipoAverbacaoCTe.Autorizacao : Dominio.Enumeradores.TipoAverbacaoCTe.Cancelamento, new Dominio.Enumeradores.StatusAverbacaoCTe[] { Dominio.Enumeradores.StatusAverbacaoCTe.Sucesso }) > 0) //, Dominio.Enumeradores.StatusAverbacaoCTe.Pendente 
                    return Json<bool>(false, false, "Já existe uma averbação emitida para este CT-e.");

                Servicos.Averbacao svcAverbacao = new Servicos.Averbacao(unidadeDeTrabalho);

                if (cte != null && cte.Seguros != null && cte.Seguros.Count > 0 && cte.Seguros.FirstOrDefault().NumeroAverbacao != null && cte.Seguros.FirstOrDefault().NumeroAverbacao != "")
                    return Json<bool>(false, false, "CT-e já possui número de averbação nos dados de Seguro, não é possível solicitar nova averbação.");

                if (cte.SistemaEmissor == Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoEmissorDocumento.NSTech)
                {
                    if (!new Servicos.Averbacao(unidadeDeTrabalho).VerificarAverbacaoEmbarcador(cte, unidadeDeTrabalho))
                        return Json<bool>(false, false, "Não foi possível solicitar uma nova averbação, verifique as configurações e tente novamente.");
                }
                else if (svcAverbacao.VerificaAverbacao(cte.Codigo, cte.Status == "A" ? Dominio.Enumeradores.TipoAverbacaoCTe.Autorizacao : Dominio.Enumeradores.TipoAverbacaoCTe.Cancelamento, unidadeDeTrabalho))
                    FilaConsultaCTe.GetInstance().QueueItem(1, cte.Codigo, Dominio.Enumeradores.TipoObjetoConsulta.Averbacao, Conexao.StringConexao);
                else
                    return Json<bool>(false, false, "Não foi possível solicitar uma nova averbação, verifique as configurações e tente novamente.");

                return Json<bool>(true, true);
            }
            catch (Exception ex)
            {
                unidadeDeTrabalho.Rollback();

                Servicos.Log.TratarErro(ex);

                return Json<bool>(false, false, "Ocorreu uma falha ao solicitar a averbação.");
            }
        }
    }
}
