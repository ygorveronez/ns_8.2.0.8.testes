namespace Repositorio
{
    public class RegimeNFSe : RepositorioBase<Dominio.Entidades.RegimeNFSe>, Dominio.Interfaces.Repositorios.RegimeNFSe
    {
        public RegimeNFSe(UnitOfWork unitOfWork) : base(unitOfWork) { }
    }
}
