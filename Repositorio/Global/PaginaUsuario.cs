using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NHibernate.Linq;

namespace Repositorio
{
    public class PaginaUsuario : RepositorioBase<Dominio.Entidades.PaginaUsuario>
    {
        public PaginaUsuario(UnitOfWork unitOfWork) : base(unitOfWork) { }

        public Dominio.Entidades.PaginaUsuario BuscarPorPaginaEUsuario(int codigoPagina, int codigoUsuario)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.PaginaUsuario>();
            var result = from obj in query where obj.Pagina.Codigo == codigoPagina && obj.Usuario.Codigo == codigoUsuario select obj;
            return result.FirstOrDefault();
        }

        public List<Dominio.Entidades.PaginaUsuario> BuscarPorUsuario(int codigoUsuario, string status = null)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.PaginaUsuario>();
            var result = from obj in query where obj.Usuario.Codigo == codigoUsuario && obj.Pagina.TipoAcesso == obj.Usuario.TipoAcesso select obj;

            if (!string.IsNullOrWhiteSpace(status))
                result = result.Where(o => o.Pagina.Status.Equals(status));

            return result.Fetch(obj => obj.Pagina).ThenFetch(obj => obj.MenuApp).ToList();
        }

        public Dominio.Entidades.PaginaUsuario BuscarPorCaminhoPaginaEUsuario(string caminhoPagina, int codigoUsuario)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.PaginaUsuario>();
            var result = from obj in query where obj.Pagina.Formulario.ToLower().Equals(caminhoPagina.ToLower()) && obj.Usuario.Codigo == codigoUsuario select obj;
            return result.FirstOrDefault();
        }

        public Dominio.Entidades.PaginaUsuario BuscarUsuariosParaRecadosAdmin(string caminhoPagina, int codigoUsuario)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.PaginaUsuario>();
            var result = from obj in query where obj.Pagina.Formulario.ToLower().Equals(caminhoPagina.ToLower()) && obj.PermissaoDeInclusao == "A" && obj.Usuario.Codigo == codigoUsuario select obj;
            return result.FirstOrDefault();
        }

        public List<Dominio.Entidades.PaginaUsuario> BuscarPorEmpresaEPagina(int codigoEmpresa, int codigoPagina)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.PaginaUsuario>();
            var result = from obj in query where obj.Pagina.Codigo == codigoPagina && obj.Usuario.Empresa.Codigo == codigoEmpresa select obj;
            return result.ToList();
        }

        public List<Dominio.Entidades.PaginaUsuario> BuscarPorUsuarioEFormularios(int codigoUsuario, string[] formularios)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.PaginaUsuario>();

            var result = from obj in query where formularios.Contains(obj.Pagina.Formulario) && obj.Usuario.Codigo == codigoUsuario select obj;

            return result.ToList();
        }

        public Dominio.Entidades.PaginaUsuario BuscarPorUsuarioEFormulario(int codigoUsuario, string formulario)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.PaginaUsuario>();

            var result = from obj in query where obj.Pagina.Formulario.Equals(formulario) && obj.Usuario.Codigo == codigoUsuario select obj;

            return result.FirstOrDefault();
        }

        public List<Dominio.ObjetosDeValor.Relatorios.RelatorioPermissaoUsuario> RelatorioPermissaoUsuario(int codigoEmpresa, int codigoUsuario, string status, string nome, string login, int codigoPerfil)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.PaginaUsuario>();

            var result = from obj in query where obj.Usuario.Empresa.Codigo == codigoEmpresa && obj.Usuario.Tipo.Equals("U") && obj.Usuario.TipoAcesso == Dominio.Enumeradores.TipoAcesso.Emissao select obj;

            if (!string.IsNullOrWhiteSpace(status))
                result = result.Where(o => o.Usuario.Status.Equals(status));

            if (codigoUsuario > 0)
                result = result.Where(o => o.Usuario.Codigo == codigoUsuario);

            if (!string.IsNullOrWhiteSpace(nome))
                result = result.Where(o => o.Usuario.Nome.Contains(nome));

            if (!string.IsNullOrWhiteSpace(login))
                result = result.Where(o => o.Usuario.Login.Contains(login));

            if (codigoPerfil > 0)
                result = result.Where(o => o.Usuario.PerfilPermissao.Codigo == codigoPerfil);

            return result.Select(o => new Dominio.ObjetosDeValor.Relatorios.RelatorioPermissaoUsuario()
            {
                Codigo = o.Usuario.Codigo,
                CPF = o.Usuario.CPF,
                Nome = o.Usuario.Nome,
                Email = o.Usuario.Email,
                Usuario = o.Usuario.Login,
                Menu = o.Pagina.Menu == "" ? "Geral" : o.Pagina.Menu,
                Formulario = o.Pagina.Descricao,
                Acesso = o.PermissaoDeAcesso == "A" ? "Sim" : "Não",
                Incluir = o.PermissaoDeInclusao == "A" ? "Sim" : "Não",
                Alterar = o.PermissaoDeAlteracao == "A" ? "Sim" : "Não",
                Excluir = o.PermissaoDeDelecao == "A" ? "Sim" : "Não",
                Ativo = o.Usuario.Status == "A" ? "Ativo" : "Inativo",
                Perfil = o.Usuario.PerfilPermissao != null ? o.Usuario.PerfilPermissao.Descricao : string.Empty,
                Ambiente = "MultiCTe"
            }).ToList();
        }
    }
}
