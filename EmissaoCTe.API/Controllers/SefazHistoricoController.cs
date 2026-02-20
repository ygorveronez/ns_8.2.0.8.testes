using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;

namespace EmissaoCTe.API.Controllers
{
    public class SefazHistoricoController : ApiController
    {
        #region Variáveis Globais

        private Dominio.ObjetosDeValor.PaginaUsuario Permissao()
        {
            return (from obj in this.BuscarPaginasUsuario() where obj.Pagina.Formulario.ToLower().Equals("historicosefaz.aspx") select obj).FirstOrDefault();
        }

        #endregion

        #region Métodos Globais

        [AcceptVerbs("POST")]
        public ActionResult Consultar()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(Conexao.StringConexao);
            try
            {
                Repositorio.SefazHistorico repSefazHistorico = new Repositorio.SefazHistorico(unitOfWork);

                string sefaz = Request.Params["Sefaz"] ?? string.Empty;

                int.TryParse(Request.Params["inicioRegistros"], out int inicioRegistros);

                DateTime.TryParseExact(Request.Params["DataInicial"], "dd/MM/yyyy", null, System.Globalization.DateTimeStyles.None, out DateTime dataInicial);
                DateTime.TryParseExact(Request.Params["DataFinal"], "dd/MM/yyyy", null, System.Globalization.DateTimeStyles.None, out DateTime dataFinal);

                int codigoUsuario = 0;

                List<Dominio.Entidades.SefazHistorico> lista = repSefazHistorico.Consultar(codigoUsuario, sefaz, dataInicial, dataFinal, inicioRegistros, 50);
                int totalRegistros = repSefazHistorico.ContarConsulta(codigoUsuario, sefaz, dataInicial, dataFinal);

                var retorno = (from obj in lista
                               select new
                               {
                                   obj.Codigo,
                                   Sefaz = obj.SefazCTe.Descricao,
                                   Usuario = obj.Usuario?.Nome ?? string.Empty,
                                   Data = obj.Data?.ToString("dd/MM/yyyy HH:mm") ?? string.Empty,
                                   obj.Observacao
                               }).ToList();

                return Json(retorno, true, null, new string[] { "Codigo", "Sefaz|10", "Usuario|15", "Data|20", "Observacao|45" }, totalRegistros);
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);
                return Json<bool>(false, false, "Ocorreu uma falha ao consultar os SefazHistoricos.");
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        [AcceptVerbs("POST")]
        public ActionResult ObterDetalhes()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(Conexao.StringConexao);
            try
            {
                Repositorio.SefazHistorico repSefazHistorico = new Repositorio.SefazHistorico(unitOfWork);
                int.TryParse(Request.Params["Codigo"], out int codigo);

                Dominio.Entidades.SefazHistorico sefazHistorico = repSefazHistorico.BuscarPorCodigo(codigo);

                if (sefazHistorico == null)
                    return Json<bool>(false, false, "SefazHistorico não encontrado.");

                var retorno = new
                {
                    sefazHistorico.Codigo,
                    Sefaz = sefazHistorico.SefazCTe.Descricao,
                    Usuario = sefazHistorico.Usuario?.Nome ?? string.Empty,
                    Data = sefazHistorico.Data?.ToString("dd/MM/yyyy HH:mm") ?? string.Empty,
                    sefazHistorico.Observacao
                };

                return Json(retorno, true);
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);
                return Json<bool>(false, false, "Ocorreu uma falha ao obter os detalhes do SefazHistorico.");
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }


        [AcceptVerbs("POST")]
        public ActionResult Salvar()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(Conexao.StringConexao);
            try
            {
                Repositorio.Empresa repEmpresa = new Repositorio.Empresa(unitOfWork);
                Repositorio.SefazHistorico repSefazHistorico = new Repositorio.SefazHistorico(unitOfWork);
                Repositorio.Sefaz repSefaz = new Repositorio.Sefaz(unitOfWork);

                int.TryParse(Request.Params["Codigo"], out int codigo);
                string descricaoSefaz = Request.Params["Sefaz"] ?? string.Empty;
                string observacao = Request.Params["Observacao"] ?? string.Empty;

                if (string.IsNullOrWhiteSpace(descricaoSefaz))
                    return Json<bool>(false, false, "Sefaz é obrigatório.");

                Dominio.Entidades.Sefaz sefaz = repSefaz.BuscarPorDescricao(descricaoSefaz);

                if (sefaz == null)
                    return Json<bool>(false, false, "Sefaz " + descricaoSefaz + " não localizado.");

                if (codigo > 0)
                {
                    if (this.Permissao() == null || this.Permissao().PermissaoDeAlteracao != "A")
                        return Json<bool>(false, false, "Permissão para alteração negada.");
                }
                else
                {
                    if (this.Permissao() == null || this.Permissao().PermissaoDeInclusao != "A")
                        return Json<bool>(false, false, "Permissão para inclusão negada.");
                }

                Dominio.Entidades.SefazHistorico sefazHistorico = repSefazHistorico.BuscarPorCodigo(codigo);

                if (sefazHistorico == null)
                {
                    sefazHistorico = new Dominio.Entidades.SefazHistorico()
                    {
                        Data = DateTime.Now,
                        Usuario = this.Usuario
                    };
                }
                else if (sefazHistorico.Usuario.Codigo != this.Usuario.Codigo)
                    return Json<bool>(false, false, "Não é possível modificar dados de outros usuários.");

                if (sefazHistorico != null)
                    sefazHistorico.Initialize();

                sefazHistorico.Observacao = observacao;
                sefazHistorico.SefazCTe = sefaz;

                if (codigo > 0)
                    repSefazHistorico.Atualizar(sefazHistorico, Auditado);
                else
                    repSefazHistorico.Inserir(sefazHistorico, Auditado);

                return Json(new
                {
                    Codigo = sefazHistorico.Codigo
                }, true);
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);
                unitOfWork.Rollback();
                return Json<bool>(false, false, "Ocorreu uma falha ao salvar o SefazHistorico.");
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        #endregion
    }
}
