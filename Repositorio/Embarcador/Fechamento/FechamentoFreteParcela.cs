using System.Collections.Generic;
using System.Linq;
using System.Linq.Dynamic.Core;


namespace Repositorio.Embarcador.Fechamento
{
    public class FechamentoFreteParcela : RepositorioBase<Dominio.Entidades.Embarcador.Fechamento.FechamentoFreteParcela>
    {
        public FechamentoFreteParcela(UnitOfWork unitOfWork) : base(unitOfWork) { }

        public Dominio.Entidades.Embarcador.Fechamento.FechamentoFreteParcela BuscarPorCodigo(int codigo)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Fechamento.FechamentoFreteParcela>();
            var result = from obj in query where obj.Codigo == codigo select obj;
            return result.FirstOrDefault();
        }

        public List<Dominio.Entidades.Embarcador.Fechamento.FechamentoFreteParcela> BuscarPorFechamento(int fechamento)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Fechamento.FechamentoFreteParcela>();
            var result = from obj in query where obj.Fechamento.Codigo == fechamento select obj;
            return result.ToList();
        }

        public List<Dominio.Entidades.Embarcador.Fechamento.FechamentoFreteParcela> BuscarPorFechamento(int codigoFechamento, string propOrdenacao, string dirOrdenacao, int inicioRegistros, int maximoRegistros)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Fechamento.FechamentoFreteParcela>();
            var result = from obj in query where obj.Fechamento.Codigo == codigoFechamento select obj;
            return result.OrderBy(propOrdenacao + (dirOrdenacao == "asc" ? " ascending" : " descending")).Skip(inicioRegistros).Take(maximoRegistros).Distinct().ToList();
        }

        public int ContarBuscarPorFechamento(int codigoFechamento)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Fechamento.FechamentoFreteParcela>();
            var result = from obj in query where obj.Fechamento.Codigo == codigoFechamento select obj;
            return result.Count();
        }

    }
}
