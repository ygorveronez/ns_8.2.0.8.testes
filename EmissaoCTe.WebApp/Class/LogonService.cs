namespace EmissaoCTe.WebApp.Class
{
    public class LogonService
    {
        public bool VerificarStatusFinanceiro(Dominio.Entidades.Usuario usuario, Repositorio.UnitOfWork unitOfWork)
        {
            if (usuario.Empresa.StatusFinanceiro.Equals("B"))
            {
                Repositorio.PaginaUsuario repPaginaUsuario = new Repositorio.PaginaUsuario(unitOfWork);
                Repositorio.Pagina repPagina = new Repositorio.Pagina(unitOfWork);

                Dominio.Entidades.PaginaUsuario pagina = repPaginaUsuario.BuscarPorCaminhoPaginaEUsuario("AvisoEmpresaBloqueioFinanceiro.aspx", usuario.Codigo);

                if (pagina != null)
                {
                    if (pagina.PermissaoDeAcesso != "A")
                    {
                        pagina.PermissaoDeAcesso = "A";
                        repPaginaUsuario.Atualizar(pagina);
                    }
                }
                else
                {
                    pagina = new Dominio.Entidades.PaginaUsuario();
                    pagina.Pagina = repPagina.BuscarPorFormulario("AvisoEmpresaBloqueioFinanceiro.aspx");
                    pagina.Usuario = usuario;
                    pagina.PermissaoDeAcesso = "A";
                    pagina.PermissaoDeAlteracao = "A";
                    pagina.PermissaoDeDelecao = "A";
                    pagina.PermissaoDeInclusao = "A";
                    repPaginaUsuario.Inserir(pagina);
                }
                return true;
            }
            return false;
        }

    }
}