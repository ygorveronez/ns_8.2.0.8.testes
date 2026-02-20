using System.Collections.Generic;
using System.Linq;
using System.Linq.Dynamic.Core;

namespace Repositorio
{
    public class AbastecimentoAnexo : RepositorioBase<Dominio.Entidades.AbastecimentoAnexo>
    {
        public AbastecimentoAnexo(UnitOfWork unitOfWork) : base(unitOfWork) { }

        public Dominio.Entidades.AbastecimentoAnexo BuscarPorCodigo(int codigo)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.AbastecimentoAnexo>();
            var result = from obj in query where obj.Codigo == codigo select obj;
            return result.FirstOrDefault();
        }

        public List<Dominio.Entidades.AbastecimentoAnexo> BuscarPorCodigoAbastecimento(int codigo)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.AbastecimentoAnexo>();
            var result = from obj in query where obj.Abastecimento.Codigo == codigo select obj;
            return result.ToList();
        }

    }
}

