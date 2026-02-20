using System.Collections.Generic;
using System.Linq;
using System.Linq.Dynamic.Core;

namespace Repositorio.Embarcador.Financeiro
{
    public class TituloBaixaTipoPagamentoRecebimento : RepositorioBase<Dominio.Entidades.Embarcador.Financeiro.TituloBaixaTipoPagamentoRecebimento>
    {
        public TituloBaixaTipoPagamentoRecebimento(UnitOfWork unitOfWork) : base(unitOfWork) { }

        #region Métodos Públicos

        public Dominio.Entidades.Embarcador.Financeiro.TituloBaixaTipoPagamentoRecebimento BuscarPorCodigo(int codigo)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Financeiro.TituloBaixaTipoPagamentoRecebimento>();
            var result = from obj in query where obj.Codigo == codigo select obj;
            return result.FirstOrDefault();
        }

        public List<Dominio.Entidades.Embarcador.Financeiro.TituloBaixaTipoPagamentoRecebimento> BuscarPorBaixaTitulo(int codigo)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Financeiro.TituloBaixaTipoPagamentoRecebimento>();
            var result = from obj in query where obj.TituloBaixa.Codigo == codigo select obj;
            return result.ToList();
        }

        public decimal TotalPorTituloBaixa(int codigo)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Financeiro.TituloBaixaTipoPagamentoRecebimento>();
            var result = from obj in query where obj.TituloBaixa.Codigo == codigo select obj;
            if (result.Count() > 0)
                return (from obj in result select obj.Valor).Sum();
            else
                return 0;
        }

        public bool ObrigaChequeBaixaTitulo(int codigo)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Financeiro.TituloBaixaTipoPagamentoRecebimento>();
            var result = from obj in query where obj.TituloBaixa.Codigo == codigo && obj.TipoPagamentoRecebimento.ObrigaChequeBaixaTitulo select obj;

            return result.Count() > 0 ? true : false;
        }

        public List<Dominio.Entidades.Embarcador.Financeiro.TituloBaixaTipoPagamentoRecebimento> Consultar(int codigoBaixa, string propOrdenacao, string dirOrdenacao, int inicioRegistros, int maximoRegistros)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Financeiro.TituloBaixaTipoPagamentoRecebimento>();
            var result = from obj in query where obj.TituloBaixa.Codigo == codigoBaixa select obj;

            return result.OrderBy(propOrdenacao + (dirOrdenacao == "asc" ? " ascending" : " descending")).Skip(inicioRegistros).Take(maximoRegistros).ToList();
        }

        public int ContarConsulta(int codigoBaixa)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Financeiro.TituloBaixaTipoPagamentoRecebimento>();
            var result = from obj in query where obj.TituloBaixa.Codigo == codigoBaixa select obj;

            return result.Count();
        }

        #endregion
    }
}
