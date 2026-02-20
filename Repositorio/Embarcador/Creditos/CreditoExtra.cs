using System.Collections.Generic;
using System.Linq;
using System.Linq.Dynamic.Core;


namespace Repositorio.Embarcador.Creditos
{
    public class CreditoExtra : RepositorioBase<Dominio.Entidades.Embarcador.Creditos.CreditoExtra>
    {
        public CreditoExtra(UnitOfWork unitOfWork) : base(unitOfWork) { }

        public Dominio.Entidades.Embarcador.Creditos.CreditoExtra BuscarPorCodigo(int codigo)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Creditos.CreditoExtra>();
            var result = from obj in query where obj.Codigo == codigo select obj;
            return result.FirstOrDefault();
        }

        public List<Dominio.Entidades.Embarcador.Creditos.CreditoExtra> Consultar(int creditoDisponivel, string propOrdenacao, string dirOrdenacao, int inicioRegistros, int maximoRegistros)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Creditos.CreditoExtra>();

            var result = from obj in query where obj.Ativo select obj;

            result = result.Where(obj => obj.CreditoDisponivel.Codigo == creditoDisponivel);

            return result.OrderBy(propOrdenacao + (dirOrdenacao == "asc" ? " ascending" : " descending")).Skip(inicioRegistros).Take(maximoRegistros).ToList();

        }

        public int ContarConsulta(int creditoDisponivel)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Creditos.CreditoExtra>();

            var result = from obj in query where obj.Ativo select obj;

            result = result.Where(obj => obj.CreditoDisponivel.Codigo == creditoDisponivel);

            return result.Count();
        }


    }
}
