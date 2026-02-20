using System.Linq;

namespace Repositorio
{
    public class EnderecoParticipanteCTe : RepositorioBase<Dominio.Entidades.EnderecoParticipanteCTe>, Dominio.Interfaces.Repositorios.EnderecoParticipanteCTe
    {
        public EnderecoParticipanteCTe(UnitOfWork unitOfWork) : base(unitOfWork) { }

        public Dominio.Entidades.EnderecoParticipanteCTe BuscarPorCodigo(int codigo)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.EnderecoParticipanteCTe>();
            var result = from obj in query where obj.Codigo == codigo select obj;
            return result.FirstOrDefault();
        }
    }
}
