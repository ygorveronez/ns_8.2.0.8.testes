using System.Collections.Generic;
using System.Linq;

namespace Repositorio
{
    public class TipoDeCustoFixo : RepositorioBase<Dominio.Entidades.TipoDeCustoFixo>, Dominio.Interfaces.Repositorios.TipoDeCustoFixo
    {
        public TipoDeCustoFixo(UnitOfWork unitOfWork) : base(unitOfWork) { }

        public Dominio.Entidades.TipoDeCustoFixo BuscarPorCodigo(int codigo, int codigoEmpresa)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.TipoDeCustoFixo>();
            var result = from obj in query where obj.Codigo == codigo && obj.Empresa.Codigo == codigoEmpresa select obj;
            return result.FirstOrDefault();
        }

        public List<Dominio.Entidades.TipoDeCustoFixo> Consultar(int codigoEmpresa, string descricao, string status, int inicioRegistros, int maximoRegistros)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.TipoDeCustoFixo>();

            var result = from obj in query where obj.Empresa.Codigo == codigoEmpresa select obj;

            if (!string.IsNullOrWhiteSpace(descricao))
                result = result.Where(o => o.Descricao.Contains(descricao));

            if (!string.IsNullOrWhiteSpace(status))
                result = result.Where(o => o.Status.Equals(status));

            return result.Skip(inicioRegistros).Take(maximoRegistros).ToList();
        }

        public int ContarConsulta(int codigoEmpresa, string descricao, string status)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.TipoDeCustoFixo>();

            var result = from obj in query where obj.Empresa.Codigo == codigoEmpresa select obj;

            if (!string.IsNullOrWhiteSpace(descricao))
                result = result.Where(o => o.Descricao.Contains(descricao));

            if (!string.IsNullOrWhiteSpace(status))
                result = result.Where(o => o.Status.Equals(status));

            return result.Count();
        }
    }
}
