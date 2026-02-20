namespace Dominio.ObjetosDeValor
{
    public class PaginaUsuario
    {
        public virtual Pagina Pagina { get; set; }

        public virtual string PermissaoDeAcesso { get; set; }

        public virtual string PermissaoDeInclusao { get; set; }

        public virtual string PermissaoDeDelecao { get; set; }

        public virtual string PermissaoDeAlteracao { get; set; }
    }
}
