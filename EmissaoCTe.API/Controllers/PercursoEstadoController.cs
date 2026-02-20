using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;

namespace EmissaoCTe.API.Controllers
{
    public class PercursoEstadoController : ApiController
    {
        #region Variáveis Globais

        private Dominio.ObjetosDeValor.PaginaUsuario Permissao()
        {
            return (from obj in this.BuscarPaginasUsuario() where obj.Pagina.Formulario.ToLower().Equals("percursosentreestados.aspx") select obj).FirstOrDefault();
        }

        #endregion

        #region Métodos Globais

        [AcceptVerbs("POST")]
        public ActionResult ObterPassagens()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(Conexao.StringConexao);

            try
            {
                string ufOrigem = Request.Params["UFOrigem"];
                string ufDestino = Request.Params["UFDestino"];

                Repositorio.PercursoEstado repPercursoEstado = new Repositorio.PercursoEstado(unitOfWork);

                Dominio.Entidades.PercursoEstado percurso = repPercursoEstado.Buscar(this.EmpresaUsuario.Codigo, ufOrigem, ufDestino);

                if (percurso != null)
                {
                    Repositorio.PassagemPercursoEstado repPassagem = new Repositorio.PassagemPercursoEstado(unitOfWork);

                    List<Dominio.Entidades.PassagemPercursoEstado> passagens = repPassagem.Buscar(percurso.Codigo);

                    var retorno = from obj in passagens
                                  orderby obj.Ordem
                                  select new
                                  {
                                      Sigla = obj.EstadoDePassagem.Sigla,
                                      Descricao = obj.EstadoDePassagem.Nome
                                  };

                    return Json(retorno, true);
                }

                return Json(false, false, "Percurso não encontrado.");
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);
                return Json<bool>(false, false, "Ocorreu uma falha ao consultar o percurso entre os estados.");
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        [AcceptVerbs("POST")]
        public ActionResult Consultar()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(Conexao.StringConexao);

            try
            {
                string ufOrigem = Request.Params["UFOrigem"];
                string ufDestino = Request.Params["UFDestino"];

                int inicioRegistros = 0;
                int.TryParse(Request.Params["inicioRegistros"], out inicioRegistros);

                Repositorio.PercursoEstado repPercursoEstado = new Repositorio.PercursoEstado(unitOfWork);

                List<Dominio.Entidades.PercursoEstado> listaPercursoEstado = repPercursoEstado.Consultar(this.EmpresaUsuario.Codigo, ufOrigem, ufDestino, inicioRegistros, 50);
                int countPercursoEstado = repPercursoEstado.ContarConsulta(this.EmpresaUsuario.Codigo, ufOrigem, ufDestino, false);

                var retorno = from obj in listaPercursoEstado select new { obj.Codigo, EstadoOrigem = obj.EstadoOrigem.Nome, EstadoDestino = obj.EstadoDestino.Nome };

                return Json(retorno, true, null, new string[] { "Codigo", "Estado de Origem|45", "Estado de Destino|45" }, countPercursoEstado);
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);
                return Json<bool>(false, false, "Ocorreu uma falha ao consultar os percursos entre estados.");
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        [AcceptVerbs("POST")]
        public ActionResult Salvar()
        {
            Repositorio.UnitOfWork unidadeDeTrabalho = new Repositorio.UnitOfWork(Conexao.StringConexao);
            try
            {
                int codigo = 0;

                int.TryParse(Request.Params["Codigo"], out codigo);

                string ufOrigem = Request.Params["UFOrigem"];
                string ufDestino = Request.Params["UFDestino"];

                List<Dominio.ObjetosDeValor.PassagemPercursoEstado> passagens = JsonConvert.DeserializeObject<List<Dominio.ObjetosDeValor.PassagemPercursoEstado>>(Request.Params["Passagens"]);

                Repositorio.PercursoEstado repPercursoEstado = new Repositorio.PercursoEstado(unidadeDeTrabalho);

                unidadeDeTrabalho.Start();

                Dominio.Entidades.PercursoEstado percAux = repPercursoEstado.Buscar(this.EmpresaUsuario.Codigo, ufOrigem, ufDestino);
                if (percAux != null && percAux.Codigo != codigo)
                {
                    unidadeDeTrabalho.Rollback();
                    return Json<bool>(false, false, "A combinação de Estado de Origem e Estado de Destino já está cadastrada.");
                }

                Dominio.Entidades.PercursoEstado percurso = null;

                if (codigo == 0)
                {
                    if (this.Permissao() == null || this.Permissao().PermissaoDeInclusao != "A")
                    {
                        unidadeDeTrabalho.Rollback();
                        return Json<bool>(false, false, "Permissão de inclusão negada!");
                    }

                    percurso = new Dominio.Entidades.PercursoEstado();
                }
                else
                {
                    if (this.Permissao() == null || this.Permissao().PermissaoDeAlteracao != "A")
                    {
                        unidadeDeTrabalho.Rollback();
                        return Json<bool>(false, false, "Permissão de alteração negada!");
                    }

                    percurso = repPercursoEstado.Buscar(this.EmpresaUsuario.Codigo, codigo);
                    if (percurso != null)
                        percurso.Initialize();
                }

                Repositorio.Estado repEstado = new Repositorio.Estado(unidadeDeTrabalho);

                percurso.Empresa = this.EmpresaUsuario;
                percurso.EstadoDestino = repEstado.BuscarPorSigla(ufDestino);
                percurso.EstadoOrigem = repEstado.BuscarPorSigla(ufOrigem);

                if (percurso.Codigo > 0)
                    repPercursoEstado.Atualizar(percurso,Auditado);
                else
                    repPercursoEstado.Inserir(percurso, Auditado);

                Repositorio.PassagemPercursoEstado repPassagemPercurso = new Repositorio.PassagemPercursoEstado(unidadeDeTrabalho);

                foreach (Dominio.ObjetosDeValor.PassagemPercursoEstado passagem in passagens)
                {
                    Dominio.Entidades.PassagemPercursoEstado passagemPercurso = repPassagemPercurso.Buscar(percurso.Codigo, passagem.Codigo);

                    if (!passagem.Excluir)
                    {
                        string mensagemAuditoria = string.Empty;

                        if (passagemPercurso == null)
                            passagemPercurso = new Dominio.Entidades.PassagemPercursoEstado();
                        else
                            mensagemAuditoria = "Alterou estado passagem " + passagemPercurso.Ordem + " de " + passagemPercurso.EstadoDePassagem.Sigla + " para " + passagem.Ordem + " " + passagem.UFPassagem;

                        passagemPercurso.EstadoDePassagem = repEstado.BuscarPorSigla(passagem.UFPassagem);
                        passagemPercurso.Ordem = passagem.Ordem;
                        passagemPercurso.Percurso = percurso;

                        if (passagemPercurso.Codigo > 0)
                        {
                            repPassagemPercurso.Atualizar(passagemPercurso);
                            Servicos.Auditoria.Auditoria.Auditar(Auditado, percurso, null, mensagemAuditoria, unidadeDeTrabalho);
                        }
                        else
                        {
                            repPassagemPercurso.Inserir(passagemPercurso);

                            Servicos.Auditoria.Auditoria.Auditar(Auditado, percurso, null, "Inseriu estado passagem "+ passagem.Ordem+ " " + passagem.UFPassagem, unidadeDeTrabalho);
                        }

                    }
                    else if (passagemPercurso != null)
                    {
                        Servicos.Auditoria.Auditoria.Auditar(Auditado, percurso, null, "Removou estado passagem " + passagemPercurso.EstadoDePassagem.Sigla, unidadeDeTrabalho);
                        repPassagemPercurso.Deletar(passagemPercurso);
                    }
                }

                unidadeDeTrabalho.CommitChanges();

                return Json<bool>(true, true);
            }
            catch (Exception ex)
            {
                unidadeDeTrabalho.Rollback();
                Servicos.Log.TratarErro(ex);
                return Json<bool>(false, false, "Ocorreu uma falha ao salvar o percurso.");
            }
            finally
            {
                unidadeDeTrabalho.Dispose();
            }
        }

        [AcceptVerbs("POST")]
        public ActionResult ObterDetalhes()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(Conexao.StringConexao);

            try
            {
                int codigo = 0;
                int.TryParse(Request.Params["Codigo"], out codigo);

                Repositorio.PercursoEstado repPercurso = new Repositorio.PercursoEstado(unitOfWork);

                Dominio.Entidades.PercursoEstado percurso = repPercurso.Buscar(this.EmpresaUsuario.Codigo, codigo);

                if (percurso == null)
                    return Json<bool>(false, false, "Percurso não encontrado.");

                Repositorio.PassagemPercursoEstado repPassagem = new Repositorio.PassagemPercursoEstado(unitOfWork);

                List<Dominio.Entidades.PassagemPercursoEstado> passagens = repPassagem.Buscar(percurso.Codigo);

                var retorno = new
                {
                    percurso.Codigo,
                    UFDestino = percurso.EstadoDestino.Sigla,
                    UFOrigem = percurso.EstadoOrigem.Sigla,
                    Passagens = (from obj in passagens
                                 select new Dominio.ObjetosDeValor.PassagemPercursoEstado()
                                 {
                                     Codigo = obj.Codigo,
                                     Excluir = false,
                                     Ordem = obj.Ordem,
                                     UFPassagem = obj.EstadoDePassagem.Sigla,
                                     DescricaoUFPassagem = obj.EstadoDePassagem.Nome
                                 })
                };

                return Json(retorno, true);
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);
                return Json<bool>(false, false, "Ocorreu uma falha ao obter os detalhes do percurso.");
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        #endregion
    }
}