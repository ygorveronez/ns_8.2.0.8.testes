using System.Collections.Generic;
using NHibernate.Linq;
using System.Linq;
using System.Linq.Dynamic.Core;
using System.Threading;
using System.Threading.Tasks;

namespace Repositorio.Embarcador.Cargas
{
    public class CargaRotaFrete : RepositorioBase<Dominio.Entidades.Embarcador.Cargas.CargaRotaFrete>
    {
        public CargaRotaFrete(UnitOfWork unitOfWork) : base(unitOfWork) { }
        public CargaRotaFrete(UnitOfWork unitOfWork, CancellationToken cancellationToken) : base(unitOfWork, cancellationToken) { }

        public Dominio.Entidades.Embarcador.Cargas.CargaRotaFrete BuscarPorCodigo(int codigo)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.CargaRotaFrete>();
            var resut = from obj in query where obj.Codigo == codigo select obj;
            return resut.FirstOrDefault();
        }
        public async Task<Dominio.Entidades.Embarcador.Cargas.CargaRotaFrete> BuscarPorCodigoAsync(int codigo)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.CargaRotaFrete>();
            var result = from obj in query where obj.Codigo == codigo select obj;
            return await result.FirstOrDefaultAsync();
        }

        public Dominio.Entidades.Embarcador.Cargas.CargaRotaFrete BuscarPorCarga(int carga)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.CargaRotaFrete>();
            var resut = from obj in query where obj.Carga.Codigo == carga select obj;
            return resut.FirstOrDefault();
        }
        public async Task<Dominio.Entidades.Embarcador.Cargas.CargaRotaFrete> BuscarPorCargaAsync(int carga)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.CargaRotaFrete>();
            var result = from obj in query where obj.Carga.Codigo == carga select obj;
            return await result.FirstOrDefaultAsync();
        }

        public List<Dominio.Entidades.Embarcador.Cargas.CargaRotaFrete> BuscarPorCargas(List<int> carga)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.CargaRotaFrete>();
            var resut = from obj in query where carga.Contains(obj.Carga.Codigo) select obj;
            return resut.ToList();
        }

        public bool VerificarSeExistePorCarga(int carga)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.CargaRotaFrete>();
            var resut = from obj in query where obj.Carga.Codigo == carga select obj;
            return resut.Any();
        }

        public List<int> VerificarSeExistePorCargas(List<int> cargas)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.CargaRotaFrete>();
            var resut = from obj in query where cargas.Contains(obj.Carga.Codigo) select obj;
            return resut.Select(obj => obj.Carga.Codigo).ToList();
        }
    }
}
