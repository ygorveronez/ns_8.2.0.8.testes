using NHibernate.Linq;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Dynamic.Core;
using System.Threading;
using System.Threading.Tasks;

namespace Repositorio.Embarcador.Cargas
{
    public class CargaLacre : RepositorioBase<Dominio.Entidades.Embarcador.Cargas.CargaLacre>
    {
        public CargaLacre(UnitOfWork unitOfWork) : base(unitOfWork) { }

        #region Métodos Públicos

        public List<Dominio.Entidades.Embarcador.Cargas.CargaLacre> Consultar(int codigoCarga, string propOrdenacao, string dirOrdenacao, int inicioRegistros, int maximoRegistros)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.CargaLacre>();

            var result = from obj in query where obj.Carga.Codigo == codigoCarga select obj;

            return result.OrderBy(propOrdenacao + " " + dirOrdenacao).Skip(inicioRegistros).Take(maximoRegistros).ToList();
        }

        public int ContarConsulta(int codigoCarga)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.CargaLacre>();

            var result = from obj in query where obj.Carga.Codigo == codigoCarga select obj;

            return result.Count();
        }

        public List<Dominio.Entidades.Embarcador.Cargas.CargaLacre> BuscarPorCargas(List<int> cargas)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.CargaLacre>();

            var result = from obj in query where cargas.Contains(obj.Carga.Codigo) select obj;

            return result.ToList();
        }

        public List<Dominio.Entidades.Embarcador.Cargas.CargaLacre> BuscarPorCarga(int carga)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.CargaLacre>();

            var result = from obj in query where obj.Carga.Codigo == carga select obj;

            return result.ToList();
        }

        public Task<List<string>> BuscarNumerosPorCargaAsync(int carga, CancellationToken cancellationToken)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.CargaLacre>();

            var result = from obj in query where obj.Carga.Codigo == carga select obj.Numero;

            return result.ToListAsync(cancellationToken);
        }

        public Dominio.Entidades.Embarcador.Cargas.CargaLacre BuscarPorCodigo(int codigo)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.CargaLacre>();

            var result = from obj in query where obj.Codigo == codigo select obj;

            return result.Fetch(obj => obj.Carga).FirstOrDefault();
        }

        public bool ExistePorCarga(int codigoCarga)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.CargaLacre>();

            var result = from obj in query where obj.Carga.Codigo == codigoCarga select obj.Codigo;

            return result.Any();
        }

        #endregion
    }
}
