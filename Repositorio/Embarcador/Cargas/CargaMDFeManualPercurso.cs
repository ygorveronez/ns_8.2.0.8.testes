using System.Collections.Generic;
using System.Linq;

namespace Repositorio.Embarcador.Cargas
{
    public class CargaMDFeManualPercurso : RepositorioBase<Dominio.Entidades.Embarcador.Cargas.CargaMDFeManualPercurso>
    {
        public CargaMDFeManualPercurso(UnitOfWork unitOfWork) : base(unitOfWork) { }

        public Dominio.Entidades.Embarcador.Cargas.CargaMDFeManualPercurso BuscarPorCodigo(int codigo)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.CargaMDFeManualPercurso>();

            query = query.Where(o => o.Codigo == codigo);

            return query.FirstOrDefault();
        }

        public List<Dominio.Entidades.Embarcador.Cargas.CargaMDFeManualPercurso> BuscarPorCargaMDFeManual(int codigoCargaMDFeManual)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.CargaMDFeManualPercurso>();

            query = query.Where(o => o.CargaMDFeManual.Codigo == codigoCargaMDFeManual);

            return query.OrderBy(o => o.Ordem).ToList();
        }

        public Dominio.Entidades.Embarcador.Cargas.CargaMDFeManualPercurso BuscarPorEstadoECargaMDFeManual(int codigoCargaMDFeManual, string estado)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.CargaMDFeManualPercurso>();

            query = query.Where(o => o.CargaMDFeManual.Codigo == codigoCargaMDFeManual && o.Estado.Sigla == estado);

            return query.FirstOrDefault();
        }
    }
}
