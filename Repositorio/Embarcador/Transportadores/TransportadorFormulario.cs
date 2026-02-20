using System.Collections.Generic;
using System.Linq;
using System.Linq.Dynamic.Core;


namespace Repositorio.Embarcador.Transportadores
{
    public class TransportadorFormulario : RepositorioBase<Dominio.Entidades.Embarcador.Transportadores.TransportadorFormulario>
    {
        public TransportadorFormulario(UnitOfWork unitOfWork) : base(unitOfWork) { }

        public List<Dominio.Entidades.Embarcador.Transportadores.TransportadorFormulario> BuscarPorEmpresa(int empresa)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Transportadores.TransportadorFormulario>();
            var result = from obj in query where obj.Empresa.Codigo == empresa select obj;
            return result.ToList();
        }

        public List<Dominio.Entidades.Embarcador.Transportadores.TransportadorFormulario> BuscarTodosRegistrosParaAtualizar()
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Transportadores.TransportadorFormulario>()
                .Where(o => o.Codigo != 0);

            return query.ToList();
        }
    }
}
