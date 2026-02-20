using System.Collections.Generic;
using System.Linq;
using NHibernate.Linq;
using System.Linq.Dynamic.Core;

namespace Repositorio
{
    public class PreCTeContaContabilContabilizacao : RepositorioBase<Dominio.Entidades.PreCTeContaContabilContabilizacao>
    {
        public PreCTeContaContabilContabilizacao(UnitOfWork unitOfWork) : base(unitOfWork) { }

        public List<Dominio.Entidades.PreCTeContaContabilContabilizacao> BuscarPorPreCTes(List<int> preCtes)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.PreCTeContaContabilContabilizacao>();
            var result = from obj in query where preCtes.Contains(obj.PreCTe.Codigo) select obj;
            return result
                .Fetch(obj => obj.PlanoConta)
                .ToList();
        }

        public List<Dominio.Entidades.PreCTeContaContabilContabilizacao> BuscarPorPreCTe(int preCte)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.PreCTeContaContabilContabilizacao>();
            var result = from obj in query where obj.PreCTe.Codigo == preCte select obj;
            return result
                .Fetch(obj => obj.PlanoConta)
                .ToList();
        }

    }
}
