using System;
using System.Web.Mvc;

namespace EmissaoCTe.API.Controllers
{
    public class IntegracaoCTeController : Controller
    {
        [AcceptVerbs("POST")]
        public ActionResult AdicionarNaFilaDeConsulta()
        {
            Repositorio.UnitOfWork unidadeDeTrabalho = new Repositorio.UnitOfWork(Conexao.StringConexao);
            try
            {
                int codigoCTe = 0;
                int.TryParse(Request.Params["CodigoCTe"], out codigoCTe);

                Repositorio.ConhecimentoDeTransporteEletronico repCTe = new Repositorio.ConhecimentoDeTransporteEletronico(unidadeDeTrabalho);
                Dominio.Entidades.ConhecimentoDeTransporteEletronico cte = repCTe.BuscarPorCodigo(codigoCTe);

                if (cte != null)
                {
                    if (cte.SistemaEmissor == Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoEmissorDocumento.Integrador)
                        FilaConsultaCTe.GetInstance().QueueItem(1, cte.Codigo, Dominio.Enumeradores.TipoObjetoConsulta.CTe, Conexao.StringConexao);
                    return Json(new { Sucesso = true });
                }
                else
                {
                    return Json(new { Sucesso = false, Erro = "CT-e não encontrado." });
                }

            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);
                return Json(new { Sucesso = false, Erro = "Ocorreu uma falha ao adicionar o CT-e na fila de consulta." });
            }
            {
                unidadeDeTrabalho.Dispose();
            }
        }

        [AcceptVerbs("POST")]
        public ActionResult AdicionarAverbacaoNaFilaDeConsulta()
        {
            Repositorio.UnitOfWork unidadeDeTrabalho = new Repositorio.UnitOfWork(Conexao.StringConexao);
            try
            {
                int codigoCTe = 0;
                int.TryParse(Request.Params["CodigoCTe"], out codigoCTe);

                Repositorio.ConhecimentoDeTransporteEletronico repCTe = new Repositorio.ConhecimentoDeTransporteEletronico(unidadeDeTrabalho);
                Dominio.Entidades.ConhecimentoDeTransporteEletronico cte = repCTe.BuscarPorCodigo(codigoCTe);

                if (cte != null)
                {
                    if (cte.SistemaEmissor == Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoEmissorDocumento.Integrador)
                        FilaConsultaCTe.GetInstance().QueueItem(1, cte.Codigo, Dominio.Enumeradores.TipoObjetoConsulta.Averbacao, Conexao.StringConexao);
                    return Json(new { Sucesso = true });
                }
                else
                {
                    return Json(new { Sucesso = false, Erro = "CT-e não encontrado." });
                }

            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);
                return Json(new { Sucesso = false, Erro = "Ocorreu uma falha ao adicionar Averbação na fila de consulta." });
            }
            finally
            {
                unidadeDeTrabalho.Dispose();
            }
        }
    }
}
