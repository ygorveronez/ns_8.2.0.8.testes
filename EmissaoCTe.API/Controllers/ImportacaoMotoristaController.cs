using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;

namespace EmissaoCTe.API.Controllers
{
    public class ImportacaoMotoristaController : ApiController
    {
        #region Variáveis Globais

        private Dominio.ObjetosDeValor.PaginaUsuario Permissao()
        {
            return (from obj in this.BuscarPaginasUsuario() where obj.Pagina.Formulario.ToLower().Equals("importacaodemotoristas.aspx") select obj).FirstOrDefault();
        }

        #endregion

        #region Métodos Globais

        [AcceptVerbs("POST")]
        public ActionResult Importar()
        {
            Repositorio.UnitOfWork unidadeDeTrabalho = new Repositorio.UnitOfWork(Conexao.StringConexao);
            try
            {
                if (this.Permissao() == null || this.Permissao().PermissaoDeInclusao != "A")
                    return Json<bool>(false, false, "Permissão de inclusão negada!");

                int codigoEmpresaOrigem, codigoEmpresaDestino = 0;
                int.TryParse(Request.Params["CodigoEmpresaOrigem"], out codigoEmpresaOrigem);
                int.TryParse(Request.Params["CodigoEmpresaDestino"], out codigoEmpresaDestino);

                Repositorio.Empresa repEmpresa = new Repositorio.Empresa(unidadeDeTrabalho);

                Dominio.Entidades.Empresa empresaOrigem = repEmpresa.BuscarPorCodigoEEmpresaPai(codigoEmpresaOrigem, this.EmpresaUsuario.Codigo);
                Dominio.Entidades.Empresa empresaDestino = repEmpresa.BuscarPorCodigoEEmpresaPai(codigoEmpresaDestino, this.EmpresaUsuario.Codigo);

                if (empresaOrigem == null)
                    return Json<bool>(false, false, "Empresa de origem dos motoristas não encontrada!");

                if (empresaDestino == null)
                    return Json<bool>(false, false, "Empresa de destino dos motoristas não encontrada!");

                Repositorio.Usuario repUsuario = new Repositorio.Usuario(unidadeDeTrabalho);

                List<Dominio.Entidades.Usuario> usuariosOrigem = repUsuario.BuscarPorEmpresa(empresaOrigem.Codigo, "M");
                List<Dominio.Entidades.Usuario> usuariosDestino = repUsuario.BuscarPorEmpresa(empresaDestino.Codigo, "M");

                unidadeDeTrabalho.Start();

                foreach (Dominio.Entidades.Usuario usuarioOrigem in usuariosOrigem)
                {
                    Dominio.Entidades.Usuario usuario = (from obj in usuariosDestino where Utilidades.String.OnlyNumbers(obj.CPF).Equals(Utilidades.String.OnlyNumbers(usuarioOrigem.CPF)) select obj).FirstOrDefault();

                    if (usuario == null)
                        usuario = new Dominio.Entidades.Usuario();

                    usuario.Categoria = usuarioOrigem.Categoria;
                    usuario.Complemento = usuarioOrigem.Complemento;
                    usuario.CPF = usuarioOrigem.CPF;
                    usuario.DataAdmissao = usuarioOrigem.DataAdmissao;
                    usuario.DataHabilitacao = usuarioOrigem.DataHabilitacao;
                    usuario.DataNascimento = usuarioOrigem.DataNascimento;
                    usuario.DataVencimentoHabilitacao = usuarioOrigem.DataVencimentoHabilitacao;
                    usuario.Email = usuarioOrigem.Email;
                    usuario.Empresa = empresaDestino;
                    usuario.Endereco = usuarioOrigem.Endereco;
                    usuario.EstadoCivil = usuarioOrigem.EstadoCivil;
                    usuario.Localidade = usuarioOrigem.Localidade;
                    usuario.Moop = usuarioOrigem.Moop;
                    usuario.Nome = usuarioOrigem.Nome;
                    usuario.NumeroCartao = usuarioOrigem.NumeroCartao;
                    usuario.NumeroHabilitacao = usuarioOrigem.NumeroHabilitacao;
                    usuario.PIS = usuarioOrigem.PIS;
                    usuario.RG = usuarioOrigem.RG;
                    usuario.Setor = usuarioOrigem.Setor;
                    usuario.Status = "A";
                    usuario.Telefone = usuarioOrigem.Telefone;
                    usuario.Tipo = "M";
                    usuario.TipoAcesso = Dominio.Enumeradores.TipoAcesso.Emissao;
                    usuario.TipoSanguineo = usuarioOrigem.TipoSanguineo;

                    if (usuario.Codigo > 0)
                        repUsuario.Atualizar(usuario);
                    else
                        repUsuario.Inserir(usuario);
                }

                unidadeDeTrabalho.CommitChanges();

                return Json<bool>(true, true);
            }
            catch (Exception ex)
            {
                unidadeDeTrabalho.Rollback();

                Servicos.Log.TratarErro(ex);

                return Json<bool>(false, false, "Ocorreu uma falha ao importar os motoristas, atualize a página e tente novamente.");
            }
        }

        #endregion
    }
}
