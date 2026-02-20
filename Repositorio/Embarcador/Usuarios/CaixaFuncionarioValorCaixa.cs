using System.Collections.Generic;
using System.Linq;
using System.Linq.Dynamic.Core;

namespace Repositorio.Embarcador.Usuarios
{
    public class CaixaFuncionarioValorCaixa : RepositorioBase<Dominio.Entidades.Embarcador.Usuarios.CaixaFuncionarioValorCaixa>
    {
        public CaixaFuncionarioValorCaixa(UnitOfWork unitOfWork) : base(unitOfWork) { }

        public Dominio.Entidades.Embarcador.Usuarios.CaixaFuncionarioValorCaixa BuscarPorCodigo(int codigo)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Usuarios.CaixaFuncionarioValorCaixa>();
            var result = from obj in query where obj.Codigo == codigo select obj;
            return result.FirstOrDefault();
        }

        public List<Dominio.Entidades.Embarcador.Usuarios.CaixaFuncionarioValorCaixa> BuscarPorCaixa(int codigo)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Usuarios.CaixaFuncionarioValorCaixa>();
            var result = from obj in query where obj.CaixaFuncionario.Codigo == codigo select obj;
            return result.ToList();
        }
        public decimal ValoresLancadoCaixa(int codigo)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Usuarios.CaixaFuncionarioValorCaixa>();

            query = query.Where(o => o.CaixaFuncionario.Codigo == codigo);

            return query.Sum(o => (decimal?)o.Valor) ?? 0m;
        }

        public List<Dominio.Entidades.Embarcador.Usuarios.CaixaFuncionarioValorCaixa> Consultar(int codigoCaixa, string propOrdenacao, string dirOrdenacao, int inicioRegistros, int maximoRegistros)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Usuarios.CaixaFuncionarioValorCaixa>();

            var result = from obj in query select obj;

            result = result.Where(obj => obj.CaixaFuncionario.Codigo == codigoCaixa);

            return result.OrderBy(propOrdenacao + (dirOrdenacao == "asc" ? " ascending" : " descending")).Skip(inicioRegistros).Take(maximoRegistros).ToList();

        }

        public int ContarConsulta(int codigoCaixa)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Usuarios.CaixaFuncionarioValorCaixa>();

            var result = from obj in query select obj;

            result = result.Where(obj => obj.CaixaFuncionario.Codigo == codigoCaixa);

            return result.Count();
        }
    }
}
