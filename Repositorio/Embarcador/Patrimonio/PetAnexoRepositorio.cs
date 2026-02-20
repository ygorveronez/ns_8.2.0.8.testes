using Dominio.Entidades.Embarcador.Patrimonio;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Dynamic.Core;

namespace Repositorio.Embarcador.Patrimonio
{
	public class PetAnexoRepositorio : RepositorioBase<PetAnexo>
	{
        public PetAnexoRepositorio(UnitOfWork workUnit) : base(workUnit) { }

		public List<PetAnexo> BuscarTodosPorCodigoPet(int codigo)
		{
			var query = this.SessionNHiBernate.Query<PetAnexo>();
			var result = from obj in query where obj.Pet.Codigo == codigo select obj;
			return result.ToList();
		}
    }
}
