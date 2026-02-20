using System.Collections.Generic;
using System.Linq;

namespace Repositorio
{
    public class PlanoDeConta : RepositorioBase<Dominio.Entidades.PlanoDeConta>, Dominio.Interfaces.Repositorios.PlanoDeConta
    {
        public PlanoDeConta(Repositorio.UnitOfWork unitOfWork) : base(unitOfWork) { }

        //public int ContarPorEmpresa(int codigoEmpresa)
        //{
        //    var query = this.SessionNHiBernate.Query<Dominio.Entidades.PlanoDeConta>();
        //    var result = from obj in query where obj.Empresa.Codigo == codigoEmpresa select obj.Codigo;
        //    return result.Count();
        //}

        public List<Dominio.Entidades.PlanoDeConta> BuscarPorEmpresa(int codigoEmpresa)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.PlanoDeConta>();
            var result = from obj in query where obj.Empresa.Codigo == codigoEmpresa select obj;
            return result.ToList();
        }

        public Dominio.Entidades.PlanoDeConta BuscarPorCodigo(int codigoEmpresa, int codigo)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.PlanoDeConta>();
            var result = from obj in query where obj.Codigo == codigo && obj.Empresa.Codigo == codigoEmpresa select obj;
            return result.FirstOrDefault();
        }

        public Dominio.Entidades.PlanoDeConta BuscarPorConta(int codigoEmpresa, string conta)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.PlanoDeConta>();
            var result = from obj in query where obj.Empresa.Codigo == codigoEmpresa && obj.Conta.Equals(conta) select obj;
            return result.FirstOrDefault();
        }

        public int ContarPorConta(int codigoEmpresa, string conta)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.PlanoDeConta>();
            var result = from obj in query where obj.Empresa.Codigo == codigoEmpresa && obj.Conta.Equals(conta) select obj;
            return result.Count();
        }

        public List<Dominio.Entidades.PlanoDeConta> Consultar(int codigoEmpresa, string status, string tipo, string descricao, string conta, int inicioRegistros, int maximoRegistros)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.PlanoDeConta>().OrderBy(o => o.Conta);
            var result = from obj in query where obj.Empresa.Codigo == codigoEmpresa select obj;
            if (!string.IsNullOrWhiteSpace(status))
                result = result.Where(o => o.Status.Equals(status));
            if (!string.IsNullOrWhiteSpace(descricao))
                result = result.Where(o => o.Descricao.Contains(descricao));
            if (!string.IsNullOrWhiteSpace(conta))
                result = result.Where(o => o.Conta.Contains(conta));
            if (!string.IsNullOrWhiteSpace(tipo))
                result = result.Where(o => o.Tipo.Equals(tipo));
            return result.Skip(inicioRegistros).Take(maximoRegistros).ToList();
        }

        public int ContarConsulta(int codigoEmpresa, string status, string tipo, string descricao, string conta)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.PlanoDeConta>();
            var result = from obj in query where obj.Empresa.Codigo == codigoEmpresa select obj;
            if (!string.IsNullOrWhiteSpace(status))
                result = result.Where(o => o.Status.Equals(status));
            if (!string.IsNullOrWhiteSpace(descricao))
                result = result.Where(o => o.Descricao.Contains(descricao));
            if (!string.IsNullOrWhiteSpace(conta))
                result = result.Where(o => o.Conta.Contains(conta));
            if (!string.IsNullOrWhiteSpace(tipo))
                result = result.Where(o => o.Tipo.Equals(tipo));
            return result.Count();
        }

    }
}
