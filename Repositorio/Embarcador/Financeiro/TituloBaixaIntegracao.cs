using System.Collections.Generic;
using System.Linq;
using System.Linq.Dynamic.Core;

namespace Repositorio.Embarcador.Financeiro
{
    public class TituloBaixaIntegracao : RepositorioBase<Dominio.Entidades.Embarcador.Financeiro.TituloBaixaIntegracao>
    {
        public TituloBaixaIntegracao(UnitOfWork unitOfWork) : base(unitOfWork) { }

        public Dominio.Entidades.Embarcador.Financeiro.TituloBaixaIntegracao BuscarPorCodigo(int codigo)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Financeiro.TituloBaixaIntegracao>();
            var result = from obj in query where obj.Codigo == codigo select obj;
            return result.FirstOrDefault();
        }

        public List<Dominio.Entidades.Embarcador.Financeiro.TituloBaixaIntegracao> BuscarPorBaixaTitulo(int codigo)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Financeiro.TituloBaixaIntegracao>();
            var result = from obj in query where obj.TituloBaixa.Codigo == codigo select obj;
            return result.ToList();
        }

        public List<Dominio.Entidades.Embarcador.Financeiro.TituloBaixaIntegracao> BuscarPorTituloBaixa(int codigoTituloBaixa, string propOrdenacao, string dirOrdenacao, int inicioRegistros, int maximoRegistros)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Financeiro.TituloBaixaIntegracao>();
            var result = from obj in query where obj.TituloBaixa.Codigo == codigoTituloBaixa select obj;
            return result.OrderBy(propOrdenacao + (dirOrdenacao == "asc" ? " ascending" : " descending")).Skip(inicioRegistros).Take(maximoRegistros).Distinct().ToList();
        }

        public int ContarBuscarPorTituloBaixa(int codigoTituloBaixa)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Financeiro.TituloBaixaIntegracao>();
            var result = from obj in query where obj.TituloBaixa.Codigo == codigoTituloBaixa select obj;
            return result.Count();
        }
    }
}
