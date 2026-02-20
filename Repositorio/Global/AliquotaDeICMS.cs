using System.Collections.Generic;
using System.Linq;

namespace Repositorio
{
    public class AliquotaDeICMS : RepositorioBase<Dominio.Entidades.AliquotaDeICMS>, Dominio.Interfaces.Repositorios.AliquotaDeICMS
    {
        public AliquotaDeICMS(UnitOfWork unitOfWork) : base(unitOfWork) { }

        public List<Dominio.Entidades.AliquotaDeICMS> BuscarPorEmpresa(int codigoEmpresa, string status = "")
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.AliquotaDeICMS>();
            var result = from obj in query where obj.Empresa.Codigo == codigoEmpresa select obj;

            if (!string.IsNullOrWhiteSpace(status))
                result = result.Where(o => o.Status.Equals(status));

            return result.OrderBy(o => o.Aliquota).ToList();
        }

        public Dominio.Entidades.AliquotaDeICMS BuscarPorCodigo(int codigoEmpresa, int codigo)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.AliquotaDeICMS>();
            var result = from obj in query where obj.Codigo == codigo && obj.Empresa.Codigo == codigoEmpresa select obj;
            return result.FirstOrDefault();
        }

        public Dominio.Entidades.AliquotaDeICMS BuscarPorAliquota(int codigoEmpresa, decimal aliquota)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.AliquotaDeICMS>();
            var result = from obj in query where obj.Aliquota == aliquota && obj.Empresa.Codigo == codigoEmpresa select obj;
            return result.FirstOrDefault();
        }

        public List<Dominio.Entidades.AliquotaDeICMS> Consultar(int codigoEmpresa, decimal aliquota, string status, int inicioRegistros, int maximoRegistros)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.AliquotaDeICMS>();
            var result = from obj in query where obj.Empresa.Codigo == codigoEmpresa select obj;

            if (aliquota > 0)
                result = result.Where(o => o.Aliquota == aliquota);

            if (!string.IsNullOrWhiteSpace(status))
                result = result.Where(o => o.Status.Equals(status));

            return result.OrderBy(o => o.Aliquota).Skip(inicioRegistros).Take(maximoRegistros).ToList();
        }

        public int ContarConsulta(int codigoEmpresa, decimal aliquota, string status)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.AliquotaDeICMS>();
            var result = from obj in query where obj.Empresa.Codigo == codigoEmpresa select obj;

            if (aliquota > 0)
                result = result.Where(o => o.Aliquota == aliquota);

            if (!string.IsNullOrWhiteSpace(status))
                result = result.Where(o => o.Status.Equals(status));

            return result.Count();
        }
    }
}
