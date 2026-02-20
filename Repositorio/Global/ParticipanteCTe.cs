using System.Linq;
using NHibernate.Linq;

namespace Repositorio
{
    public class ParticipanteCTe : RepositorioBase<Dominio.Entidades.ParticipanteCTe>, Dominio.Interfaces.Repositorios.ParticipanteCTe
    {
        public ParticipanteCTe(UnitOfWork unitOfWork) : base(unitOfWork) { }

        public Dominio.Entidades.ParticipanteCTe BuscarDoExteriorPorNome(string nome)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.ParticipanteCTe>();

            var result = from obj in query where obj.Exterior == true && obj.Nome.ToLower().Equals(nome.ToLower()) orderby obj.Codigo descending select obj;

            return result.Timeout(120).FirstOrDefault();
        }

        public Dominio.Entidades.ParticipanteCTe BuscarPorCPFCNPJ(string cpfcnpj)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.ParticipanteCTe>();

            var result = from obj in query where obj.Exterior && obj.CPF_CNPJ.Equals(cpfcnpj) orderby obj.Codigo descending select obj;

            return result.FirstOrDefault();
        }

    }
}
