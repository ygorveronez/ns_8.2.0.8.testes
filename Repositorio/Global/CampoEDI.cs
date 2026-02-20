namespace Repositorio
{
    public class CampoEDI : RepositorioBase<Dominio.Entidades.CampoEDI>, Dominio.Interfaces.Repositorios.CampoEDI
    {

         public CampoEDI(UnitOfWork unitOfWork) : base(unitOfWork) { }
    }
}
