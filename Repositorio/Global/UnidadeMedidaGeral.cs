using System.Collections.Generic;
using System.Linq;

namespace Repositorio
{
    public class UnidadeMedidaGeral : RepositorioBase<Dominio.Entidades.UnidadeMedidaGeral>, Dominio.Interfaces.Repositorios.UnidadeMedidaGeral
    {
        public UnidadeMedidaGeral(Repositorio.UnitOfWork unitOfWork) : base(unitOfWork) { }

        public List<Dominio.Entidades.UnidadeMedidaGeral> BuscarTodos(int codigoEmpresa)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.UnidadeMedidaGeral>();

            var result = from obj in query where obj.Empresa.Codigo == codigoEmpresa && obj.Status.Equals("A") select obj;

            return result.OrderBy(o => o.Sigla).ToList();
        }

        public Dominio.Entidades.UnidadeMedidaGeral BuscarPorCodigo(int codigoEmpresa, int codigo)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.UnidadeMedidaGeral>();

            var result = from obj in query where obj.Codigo == codigo && obj.Empresa.Codigo == codigoEmpresa select obj;

            return result.FirstOrDefault();
        }

        public Dominio.Entidades.UnidadeMedidaGeral BuscarPorSigla(int codigoEmpresa, string sigla)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.UnidadeMedidaGeral>();

            var result = from obj in query where obj.Sigla.Equals(sigla) && obj.Empresa.Codigo == codigoEmpresa select obj;

            return result.FirstOrDefault();
        }

        public List<Dominio.Entidades.UnidadeMedidaGeral> Consultar(int codigoEmpresa, string descricao, string sigla, string status, int inicioRegistros, int maximoRegistros)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.UnidadeMedidaGeral>();

            var result = from obj in query where obj.Empresa.Codigo == codigoEmpresa select obj;

            if (!string.IsNullOrWhiteSpace(descricao))
                result = result.Where(o => o.Descricao.Contains(descricao));

            if (!string.IsNullOrWhiteSpace(status))
                result = result.Where(o => o.Status.Equals(status));

            if (!string.IsNullOrWhiteSpace(sigla))
                result = result.Where(o => o.Sigla.Contains(sigla));

            return result.OrderBy(o => o.Descricao).Skip(inicioRegistros).Take(maximoRegistros).ToList();
        }

        public int ContarConsulta(int codigoEmpresa, string descricao, string sigla, string status)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.UnidadeMedidaGeral>();

            var result = from obj in query where obj.Empresa.Codigo == codigoEmpresa select obj;

            if (!string.IsNullOrWhiteSpace(descricao))
                result = result.Where(o => o.Descricao.Contains(descricao));

            if (!string.IsNullOrWhiteSpace(status))
                result = result.Where(o => o.Status.Equals(status));

            if (!string.IsNullOrWhiteSpace(sigla))
                result = result.Where(o => o.Sigla.Contains(sigla));

            return result.Count();
        }


    }
}
