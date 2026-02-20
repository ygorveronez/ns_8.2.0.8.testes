using System.Collections.Generic;
using System.Linq;
using System.Linq.Dynamic.Core;

namespace Repositorio.Embarcador.Financeiro
{
    public class ContratoFinanciamentoParcelaValor : RepositorioBase<Dominio.Entidades.Embarcador.Financeiro.ContratoFinanciamentoParcelaValor>
    {
        public ContratoFinanciamentoParcelaValor(UnitOfWork unitOfWork) : base(unitOfWork) { }

        public Dominio.Entidades.Embarcador.Financeiro.ContratoFinanciamentoParcelaValor BuscarPorCodigo(int codigo)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Financeiro.ContratoFinanciamentoParcelaValor>();
            var result = from obj in query where obj.Codigo == codigo select obj;
            return result.FirstOrDefault();
        }

        public List<Dominio.Entidades.Embarcador.Financeiro.ContratoFinanciamentoParcelaValor> BuscarPorContratoFinanciamentoParcela(int codigoContratoFinanciamentoParcela)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Financeiro.ContratoFinanciamentoParcelaValor>();

            query = from obj in query where obj.ContratoFinanciamentoParcela.Codigo == codigoContratoFinanciamentoParcela select obj;

            return query.ToList();
        }

        public List<Dominio.Entidades.Embarcador.Financeiro.ContratoFinanciamentoParcelaValor> Consultar(int codigoParcela, string propOrdenacao, string dirOrdenacao, int inicioRegistros, int maximoRegistros)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Financeiro.ContratoFinanciamentoParcelaValor>();

            var result = from obj in query where obj.ContratoFinanciamentoParcela.Codigo == codigoParcela select obj;

            return result.OrderBy(propOrdenacao + (dirOrdenacao == "asc" ? " ascending" : " descending")).Skip(inicioRegistros).Take(maximoRegistros).ToList();
        }

        public int ContarConsulta(int codigoParcela)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Financeiro.ContratoFinanciamentoParcelaValor>();

            var result = from obj in query where obj.ContratoFinanciamentoParcela.Codigo == codigoParcela select obj;

            return result.Count();
        }
    }
}
