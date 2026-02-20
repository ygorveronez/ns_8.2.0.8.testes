using System.Collections.Generic;
using System.Linq;
using System.Linq.Dynamic.Core;

namespace Repositorio.Embarcador.Financeiro
{
    public class TituloBaixaDesconto : RepositorioBase<Dominio.Entidades.Embarcador.Financeiro.TituloBaixaDesconto>
    {
        public TituloBaixaDesconto(UnitOfWork unitOfWork) : base(unitOfWork) { }

        public Dominio.Entidades.Embarcador.Financeiro.TituloBaixaDesconto BuscarPorCodigo(int codigo)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Financeiro.TituloBaixaDesconto>();
            var result = from obj in query where obj.Codigo == codigo select obj;
            return result.FirstOrDefault();
        }

        public List<Dominio.Entidades.Embarcador.Financeiro.TituloBaixaDesconto> BuscarPorBaixaTitulo(int codigo)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Financeiro.TituloBaixaDesconto>();
            var result = from obj in query where obj.TituloBaixa.Codigo == codigo select obj;
            return result.ToList();
        }

        public decimal TotalPorTituloBaixa(int codigo)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Financeiro.TituloBaixaDesconto>();
            var result = from obj in query where obj.TituloBaixa.Codigo == codigo select obj;
            if (result.Count() > 0)
                return (from obj in result select obj.Valor).Sum();
            else
                return 0;
        }
    }

}
