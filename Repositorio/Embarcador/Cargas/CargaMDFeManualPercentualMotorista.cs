using System.Collections.Generic;
using System.Linq;
using NHibernate.Linq;

namespace Repositorio.Embarcador.Cargas
{
    public class CargaMDFeManualPercentualMotorista : RepositorioBase<Dominio.Entidades.Embarcador.Cargas.CargaMDFeManualPercentualMotorista>
    {
        public CargaMDFeManualPercentualMotorista(UnitOfWork unitOfWork) : base(unitOfWork) { }

        #region Métodos Públicos

        public List<Dominio.Entidades.Embarcador.Cargas.CargaMDFeManualPercentualMotorista> BuscarPorCargaMDFeManual(int codigoCargaMDFeManual)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.CargaMDFeManualPercentualMotorista>();
            var result = from obj in query select obj;
            result = result.Where(p => p.CargaMDFeManual.Codigo == codigoCargaMDFeManual);

            return result
                .Fetch(obj => obj.Motorista)
                .ToList();
        }

        public Dominio.Entidades.Embarcador.Cargas.CargaMDFeManualPercentualMotorista BuscarPorCargaMDFeManualMotorista(int codigoCargaMDFeManual, int codigoMotorista)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.CargaMDFeManualPercentualMotorista>();

            query = query.Where(p => p.CargaMDFeManual.Codigo == codigoCargaMDFeManual)
                .Where(p => p.Motorista.Codigo == codigoMotorista);

            return query.FirstOrDefault();
        }

        #endregion
    }
}
