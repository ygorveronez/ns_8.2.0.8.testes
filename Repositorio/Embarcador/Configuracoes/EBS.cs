using System.Linq;

namespace Repositorio.Embarcador.Configuracoes
{
    public class EBS : RepositorioBase<Dominio.Entidades.Embarcador.Configuracoes.EBS>
    {
        public EBS(UnitOfWork unitOfWork) : base(unitOfWork) { }

        public Dominio.Entidades.Embarcador.Configuracoes.EBS Buscar()
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Configuracoes.EBS>();

            var result = from obj in query select obj;

            return result.FirstOrDefault();
        }
    }
}
