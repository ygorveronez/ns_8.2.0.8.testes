using System.Collections.Generic;
using System.Linq;
using System.Linq.Dynamic.Core;

namespace Repositorio.Embarcador.Cargas
{
    public class CargaCancelamentoLog : RepositorioBase<Dominio.Entidades.Embarcador.Cargas.CargaCancelamentoLog>
    {
        public CargaCancelamentoLog(UnitOfWork unitOfWork) : base(unitOfWork) { }

        public List<Dominio.Entidades.Embarcador.Cargas.CargaCancelamentoLog> BuscarPorCargaCancelamento(int cargaCancelamento)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.CargaCancelamentoLog>();
            var resut = from obj in query where obj.CargaCancelamento.Codigo == cargaCancelamento select obj;
            return resut.ToList();
        }

    }
}
