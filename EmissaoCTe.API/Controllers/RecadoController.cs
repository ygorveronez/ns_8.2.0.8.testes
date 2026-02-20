using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;

namespace EmissaoCTe.API.Controllers
{
    public class RecadoController : ApiController
    {
        #region Variáveis Globais

        private Dominio.ObjetosDeValor.PaginaUsuario Permissao()
        {
            return (from obj in this.BuscarPaginasUsuario() where obj.Pagina.Formulario.ToLower().Equals("recado.aspx") select obj).FirstOrDefault();
        }

        #endregion

        #region Métodos Globais

        [AcceptVerbs("POST")]
        public ActionResult Consultar()
        {
            Repositorio.UnitOfWork unidadeDeTrabalho = new Repositorio.UnitOfWork(Conexao.StringConexao);
            try
            {
                string titulo = Request.Params["Titulo"];
                string nomeUsuario = Request.Params["NomeUsuario"];

                DateTime.TryParseExact(Request.Params["DataLancamento"], "dd/MM/yyyy", null, System.Globalization.DateTimeStyles.None, out DateTime dataLancamento);

                int inicioRegistros = 0;
                int.TryParse(Request.Params["inicioRegistros"], out inicioRegistros);

                Repositorio.Recado repRecado = new Repositorio.Recado(unidadeDeTrabalho);

                List<Dominio.Entidades.Recado> listaMensagens = repRecado.Consultar(this.EmpresaUsuario.Codigo, dataLancamento, nomeUsuario, titulo, inicioRegistros, 50);
                int countRecados = repRecado.ContarConsulta(this.EmpresaUsuario.Codigo, dataLancamento, nomeUsuario, titulo);

                var retorno = (from obj in listaMensagens
                              select new
                              {
                                  obj.Codigo,
                                  obj.Descricao,
                                  Ativo = obj.Ativo ? "true" : "false",
                                  DataLancamento = obj.DataLancamento.ToString("dd/MM/yyyy HH:mm"),
                                  Usuario = obj.Usuario.Nome,
                                  obj.Titulo
                              }).ToList();

                return Json(retorno, true, null, new string[] { "Codigo", "Descricao", "Ativo", "DataLancamento|15", "Usuario|30", "Titulo|30" }, countRecados);
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);
                return Json<bool>(false, false, "Ocorreu uma falha ao obter recados.");
            }
            finally
            {
                unidadeDeTrabalho.Dispose();
            }
        }

        [AcceptVerbs("POST")]
        public ActionResult ObterDetalhes()
        {
            Repositorio.UnitOfWork unidadeDeTrabalho = new Repositorio.UnitOfWork(Conexao.StringConexao);
            try
            {
                int codigo;
                int.TryParse(Request.Params["Codigo"], out codigo);

                Repositorio.Recado repRecado = new Repositorio.Recado(unidadeDeTrabalho);

                Dominio.Entidades.Recado recado = repRecado.BuscarPorCodigo(codigo);

                var retorno = new
                {
                    recado.Codigo,
                    DataLancamento = recado.DataLancamento.ToString("dd/MM/yyyy HH:mm"),
                    Usuario = recado.Usuario.Nome,
                    recado.Titulo,
                    recado.Descricao,
                    recado.Ativo,
                    UsuariosRecados = (from obj in recado.RecadoUsuarios
                                       select new
                                       {
                                           NomeUsuario = obj.Usuario.Nome,
                                           DataLeitura = obj.DataLeitura != null && obj.DataLeitura >= DateTime.MinValue ? obj.DataLeitura.Value.ToString("dd/MM/yyyy HH:mn") : string.Empty,
                                           Status = obj.DataLeitura != null && obj.DataLeitura >= DateTime.MinValue ? "Lido" : "Pendente"
                                       }).ToList()
                };

                return Json(retorno, true);
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);
                return Json<bool>(false, false, "Ocorreu uma falha ao obter recados.");
            }
            finally
            {
                unidadeDeTrabalho.Dispose();
            }
        }

        [AcceptVerbs("POST")]
        public ActionResult ConsultarUsuariosRecados()
        {
            Repositorio.UnitOfWork unidadeDeTrabalho = new Repositorio.UnitOfWork(Conexao.StringConexao);
            try
            {
                int.TryParse(Request.Params["Codigo"], out int codigo);
                int.TryParse(Request.Params["inicioRegistros"], out int inicioRegistros);

                Repositorio.RecadoUsuarios repRecadoUsuarios = new Repositorio.RecadoUsuarios(unidadeDeTrabalho);

                List<Dominio.Entidades.RecadoUsuarios> listaRecadoUsuarios = repRecadoUsuarios.BuscarPorRecado(codigo, inicioRegistros, 50);
                int quantidadeRecados = repRecadoUsuarios.ContarPorRecado(codigo);

                var retorno = (from obj in listaRecadoUsuarios
                               select new
                               {
                                   obj.Usuario.Nome,
                                   DataLeitura = obj.DataLeitura.HasValue ? obj.DataLeitura.Value.ToString("dd/MM/yyyy HH:mm") : string.Empty,
                                   Status = obj.DataLeitura.HasValue ? "Lido" : "Pendente"
                               }).ToList();


                return Json(retorno, true, null, new string[] { "Nome|50", "Data Leitura|20", "Status|20" }, quantidadeRecados);
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);
                return Json<bool>(false, false, "Ocorreu uma falha ao obter usuarios recados.");
            }
            finally
            {
                unidadeDeTrabalho.Dispose();
            }
        }

        [AcceptVerbs("POST")]
        public ActionResult BuscarUsuario()
        {
            Repositorio.UnitOfWork unidadeDeTrabalho = new Repositorio.UnitOfWork(Conexao.StringConexao);
            try
            {

                Repositorio.Recado repRecado = new Repositorio.Recado(unidadeDeTrabalho);

                var retorno = new
                {
                    Usuario = this.Usuario.Nome,
                    DataLancamento = DateTime.Today.ToString("dd/MM/yyyy")
                };

                return Json(retorno, true);
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);
                return Json<bool>(false, false, "Ocorreu uma falha ao obter recados.");
            }
            finally
            {
                unidadeDeTrabalho.Dispose();
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

                string mensagem = System.Uri.UnescapeDataString(Request.Params["Mensagem"]);
                string titulo = Request.Params["Titulo"];
                bool.TryParse(Request.Params["Ativo"], out bool ativo);

                if (string.IsNullOrWhiteSpace(titulo))
                    return Json<bool>(false, false, "Título inválido.");

                if (string.IsNullOrWhiteSpace(mensagem))
                    return Json<bool>(false, false, "Mensagem inválida.");
                
                Repositorio.Recado repRecado = new Repositorio.Recado(unidadeDeTrabalho);

                Dominio.Entidades.Recado recado = null;

                if (codigo > 0)
                {
                    if (this.Permissao() == null || this.Permissao().PermissaoDeAlteracao != "A")
                        return Json<bool>(false, false, "Permissão para alteração negada.");

                    recado = repRecado.BuscarPorCodigo(codigo);
                }
                else
                {
                    if (this.Permissao() == null || this.Permissao().PermissaoDeInclusao != "A")
                        return Json<bool>(false, false, "Permissão para inclusão negada.");

                    recado = new Dominio.Entidades.Recado();
                }

                recado.DataLancamento = DateTime.Now;
                recado.Titulo = titulo;
                recado.Descricao = mensagem;
                recado.Ativo = ativo;

                if (codigo > 0)
                { 
                    if (recado.Usuario.CPF != this.Usuario.CPF)
                        return Json<bool>(false, false, "Recado pode ser alterado apenas pelo usuário que efetuou o lançamento.");

                    repRecado.Atualizar(recado);

                    if (recado.RecadoUsuarios.Count == 0)
                        this.GerarRecadosUsuarios(recado, unidadeDeTrabalho);
                }
                else
                {
                    recado.Empresa = this.EmpresaUsuario;
                    recado.Usuario = this.Usuario;
                    repRecado.Inserir(recado);

                    this.GerarRecadosUsuarios(recado, unidadeDeTrabalho);
                }

                return Json<bool>(true, true);
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);
                return Json<bool>(false, false, "Ocorreu uma falha ao salvar recado.");
            }
            finally
            {
                unidadeDeTrabalho.Dispose();
            }
        }

        [AcceptVerbs("POST")]
        public ActionResult ConsultarRecadosUsuario()
        {
            Repositorio.UnitOfWork unidadeDeTrabalho = new Repositorio.UnitOfWork(Conexao.StringConexao);
            try
            {
                string situacao = Request.Params["Situacao"];
                string titulo = Request.Params["Titulo"];
                string mensagem = Request.Params["Mensagem"];
                int.TryParse(Request.Params["inicioRegistros"], out int inicioRegistros);

                Repositorio.RecadoUsuarios repRecadoUsuarios = new Repositorio.RecadoUsuarios(unidadeDeTrabalho);

                List<Dominio.Entidades.RecadoUsuarios> listaRecado = repRecadoUsuarios.ConsultarPendentesUsuario(this.Usuario.Codigo, situacao, titulo, mensagem, inicioRegistros, 50);
                int quantidadeRecados = repRecadoUsuarios.ContarPendentesUsuario(this.Usuario.Codigo, situacao, titulo, mensagem);

                var retorno = (from obj in listaRecado
                               select new
                               {
                                   obj.Recado.Codigo,
                                   DataLeitura = obj.DataLeitura.HasValue ? obj.DataLeitura.Value.ToString("dd/MM/yyyy HH:mm") : string.Empty,
                                   Data = obj.Recado.DataLancamento.ToString("dd/MM/yyyy HH:mm"),
                                   Usuario = obj.Recado.Usuario.Nome,
                                   obj.Recado.Titulo,
                                   obj.Recado.Descricao,
                                   Status = obj.DataLeitura.HasValue ? "Lido ("+ obj.DataLeitura.Value.ToString("dd/MM/yyyy HH:mm")+")" : "Pendente"
                               }).ToList();


                return Json(retorno, true, null, new string[] { "Codigo", "Data Leitura", "Data|15", "Usuario|15", "Titulo|20", "Descricao|30", "Status|10" }, quantidadeRecados);
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);
                return Json<bool>(false, false, "Ocorreu uma falha ao obter recados do usuário recados.");
            }
            finally
            {
                unidadeDeTrabalho.Dispose();
            }
        }

        [AcceptVerbs("POST")]
        public ActionResult BuscarRecadosPendentesUsuario()
        {
            Repositorio.UnitOfWork unidadeDeTrabalho = new Repositorio.UnitOfWork(Conexao.StringConexao);
            try
            {
                Repositorio.RecadoUsuarios repRecadoUsuarios = new Repositorio.RecadoUsuarios(unidadeDeTrabalho);                
                int quantidade = repRecadoUsuarios.ContarPendentesUsuario(this.Usuario.Codigo, "P", "", "");

                string mensagemAviso = string.Empty;

                if (quantidade > 0)
                {
                    if (quantidade == 1)
                        mensagemAviso = "Existe " + quantidade.ToString() + " recado pendente!";
                    else
                        mensagemAviso = "Existem " + quantidade.ToString() + " recados pendentes!";
                }

                var retorno = new
                {
                    MensagemAviso = mensagemAviso
                };

                return Json(retorno, true);
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);
                return Json<bool>(false, false, "Ocorreu uma falha ao obter recados do usuário recados.");
            }
            finally
            {
                unidadeDeTrabalho.Dispose();
            }
        }


        [AcceptVerbs("POST")]
        public ActionResult ConfirmarLeitura()
        {
            Repositorio.UnitOfWork unidadeDeTrabalho = new Repositorio.UnitOfWork(Conexao.StringConexao);
            try
            {
                int codigo = 0;
                int.TryParse(Request.Params["Codigo"], out codigo);
                
                Repositorio.RecadoUsuarios repRecadoUsuarios = new Repositorio.RecadoUsuarios(unidadeDeTrabalho);

                Dominio.Entidades.RecadoUsuarios recadoUsuario = repRecadoUsuarios.BuscarPorRecadoEUsuarioLeitura(codigo, this.Usuario.Codigo);

                if (recadoUsuario == null)
                    return Json<bool>(false, false, "Recado não encontrado.");

                if (recadoUsuario.DataLeitura.HasValue)
                    return Json<bool>(false, false, "Confirmação de leitura já feita em "+recadoUsuario.DataLeitura.Value.ToString("dd/MM/yyyy HH:mm")+".");

                recadoUsuario.DataLeitura = DateTime.Now;
                repRecadoUsuarios.Atualizar(recadoUsuario);

                return Json<bool>(true, true);
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);
                return Json<bool>(false, false, "Ocorreu uma falha ao salvar confirmação de leitura.");
            }
            finally
            {
                unidadeDeTrabalho.Dispose();
            }
        }

        #endregion 

        private void GerarRecadosUsuarios(Dominio.Entidades.Recado recado, Repositorio.UnitOfWork unidadeDeTrabalho)
        {
            Repositorio.RecadoUsuarios repRecadoUsuarios = new Repositorio.RecadoUsuarios(unidadeDeTrabalho);
            Repositorio.Usuario repUsuario = new Repositorio.Usuario(unidadeDeTrabalho);
            Repositorio.PaginaUsuario repPaginaUsuario = new Repositorio.PaginaUsuario(unidadeDeTrabalho);

            List<Dominio.Entidades.Usuario> listaUsuarios = repUsuario.BuscarUsuariosParaRecados(this.EmpresaUsuario.Codigo, Dominio.Enumeradores.TipoAcesso.Admin);
            foreach (Dominio.Entidades.Usuario usuario in listaUsuarios)
            {
                if (usuario.CPF != this.Usuario.CPF) //Não gera para o mesmo usuário 
                {
                    Dominio.Entidades.RecadoUsuarios usuarioRecado = new Dominio.Entidades.RecadoUsuarios();
                    usuarioRecado.Usuario = usuario;
                    usuarioRecado.Recado = recado;
                    repRecadoUsuarios.Inserir(usuarioRecado);
                }
            }
        }
    }
}
