using System.Collections.Generic;
using System.Linq;

namespace Repositorio
{
    public class Ajuda : RepositorioBase<Dominio.Entidades.Ajuda>
    {
        public Ajuda(UnitOfWork unitOfWork) : base(unitOfWork) { }

        public Dominio.Entidades.Ajuda BuscarPorCodigo(int codigoEmpresa, int codigo)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Ajuda>();
            var result = from obj in query where obj.Codigo == codigo && obj.Empresa.Codigo == codigoEmpresa select obj;
            return result.FirstOrDefault();
        }
        
        public List<Dominio.Entidades.Ajuda> BuscarTodasAjudas(int codigoEmpresa)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Ajuda>();
            var result = from obj in query where obj.Empresa.Codigo == codigoEmpresa && obj.Status == "A" select obj;

            return result.OrderByDescending(o => o.Descricao).ToList();
        }

        public List<Dominio.Entidades.Ajuda> Consultar(int codigoEmpresa, string descricao, string status, int inicioRegistros, int maximoRegistros)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Ajuda>();
            var result = from obj in query where obj.Empresa.Codigo == codigoEmpresa select obj;

            if (!string.IsNullOrWhiteSpace(descricao))
                result = result.Where(o => o.Descricao.Contains(descricao));

            if (!string.IsNullOrWhiteSpace(status))
                result = result.Where(o => o.Status.Equals(status));

            return result.OrderByDescending(o => o.Codigo).Skip(inicioRegistros).Take(maximoRegistros).ToList();
        }

        public int ContarConsulta(int codigoEmpresa, string descricao, string status)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Ajuda>();
            var result = from obj in query where obj.Empresa.Codigo == codigoEmpresa select obj;

            if (!string.IsNullOrWhiteSpace(descricao))
                result = result.Where(o => o.Descricao.Contains(descricao));

            if (!string.IsNullOrWhiteSpace(status))
                result = result.Where(o => o.Status.Equals(status));

            return result.Count();
        }
    }
}
