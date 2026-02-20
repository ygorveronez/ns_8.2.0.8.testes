namespace Repositorio
{
    public class ParticipanteNFSe : RepositorioBase<Dominio.Entidades.ParticipanteNFSe>, Dominio.Interfaces.Repositorios.ParticipanteNFSe
    {
        public ParticipanteNFSe(UnitOfWork unitOfWork) : base(unitOfWork) { }
    }
}
