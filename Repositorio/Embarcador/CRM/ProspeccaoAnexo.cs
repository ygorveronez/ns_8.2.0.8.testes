using System.Collections.Generic;
using NHibernate.Linq;
using System.Linq;
using System.Linq.Dynamic.Core;
using System.Threading;
using System.Threading.Tasks;

namespace Repositorio.Embarcador.CRM
{
    public class ProspeccaoAnexo : RepositorioBase<Dominio.Entidades.Embarcador.CRM.ProspeccaoAnexo>
    {
        public ProspeccaoAnexo(UnitOfWork unitOfWork) : base(unitOfWork) { }

        public ProspeccaoAnexo(UnitOfWork unitOfWork, CancellationToken cancellationToken) : base(unitOfWork, cancellationToken) { }

        public Dominio.Entidades.Embarcador.CRM.ProspeccaoAnexo BuscarPorCodigo(int codigo)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.CRM.ProspeccaoAnexo>();
            var result = from obj in query where obj.Codigo == codigo select obj;
            return result.FirstOrDefault();
        }

        public List<Dominio.Entidades.Embarcador.CRM.ProspeccaoAnexo> BuscarPorProspeccao(int codigo)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.CRM.ProspeccaoAnexo>();
            var result = from obj in query where obj.Prospeccao.Codigo == codigo select obj;
            return result.ToList();
        }

        public async Task<List<Dominio.Entidades.Embarcador.CRM.ProspeccaoAnexo>> BuscarPorProspeccaoAsync(int codigo)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.CRM.ProspeccaoAnexo>();
            var result = from obj in query where obj.Prospeccao.Codigo == codigo select obj;
            return await result.ToListAsync();
        }

        public List<Dominio.Entidades.Embarcador.CRM.ProspeccaoAnexo> Consultar(int codigoProspeccao, string propOrdenacao, string dirOrdenacao, int inicioRegistros, int maximoRegistros)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.CRM.ProspeccaoAnexo>();
            var result = from obj in query where obj.Prospeccao.Codigo == codigoProspeccao select obj;

            return result
                .OrderBy(propOrdenacao + (dirOrdenacao == "asc" ? " ascending" : " descending"))
                .Skip(inicioRegistros)
                .Take(maximoRegistros)
                .ToList();
        }

        public int ContarConsulta(int codigoProspeccao)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.CRM.ProspeccaoAnexo>();
            var result = from obj in query where obj.Prospeccao.Codigo == codigoProspeccao select obj;

            return result.Count();
        }
    }
}
