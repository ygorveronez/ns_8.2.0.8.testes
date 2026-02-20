using System.Collections.Generic;
using System.Linq;
using System.Linq.Dynamic.Core;

namespace Repositorio.Embarcador.CRM
{
    public class ClienteProspect : RepositorioBase<Dominio.Entidades.Embarcador.CRM.ClienteProspect>
    {

        public ClienteProspect(UnitOfWork unitOfWork) : base(unitOfWork) { }

        public Dominio.Entidades.Embarcador.CRM.ClienteProspect BuscarPorCodigo(int codigo, int codigoEmpresa)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.CRM.ClienteProspect>();
            var result = from obj in query where obj.Codigo == codigo select obj;

            if (codigoEmpresa > 0)
                result = result.Where(o => o.Empresa.Codigo == codigoEmpresa);

            return result.FirstOrDefault();
        }

        public Dominio.Entidades.Embarcador.CRM.ClienteProspect BuscarPorNome(string nome)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.CRM.ClienteProspect>();
            var result = from obj in query where obj.Nome == nome select obj;

            return result.FirstOrDefault();
        }

        private IQueryable<Dominio.Entidades.Embarcador.CRM.ClienteProspect> _Consultar(string descricao, int codigoEmpresa)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.CRM.ClienteProspect>();

            var result = from obj in query select obj;

            if (codigoEmpresa > 0)
                result = result.Where(o => o.Empresa.Codigo == codigoEmpresa);

            // Filtros
            if (!string.IsNullOrWhiteSpace(descricao))
                result = result.Where(o => o.Nome.Equals(descricao));

            return result;
        }

        public List<Dominio.Entidades.Embarcador.CRM.ClienteProspect> Consultar(string descricao, int codigoEmpresa, string propOrdena, string dirOrdena, int inicioRegistros, int maximoRegistros)
        {
            var result = _Consultar(descricao, codigoEmpresa);

            if (!string.IsNullOrWhiteSpace(propOrdena))
                result = result.OrderBy(propOrdena + (dirOrdena == "asc" ? " ascending" : " descending"));

            if (inicioRegistros > 0)
                result = result.Skip(inicioRegistros);

            if (maximoRegistros > 0)
                result = result.Take(maximoRegistros);

            return result.ToList();
        }

        public int ContarConsulta(string descricao, int codigoEmpresa)
        {
            var result = _Consultar(descricao, codigoEmpresa);

            return result.Count();
        }
    }
}
