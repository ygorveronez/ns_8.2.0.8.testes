using System.Collections.Generic;
using System.Linq;
using System.Linq.Dynamic.Core;

namespace Repositorio.Embarcador.Financeiro
{
    public sealed class TituloBaixaCheque : RepositorioBase<Dominio.Entidades.Embarcador.Financeiro.TituloBaixaCheque>
    {
        #region Construtores

        public TituloBaixaCheque(UnitOfWork unitOfWork) : base(unitOfWork) { }

        #endregion

        #region Métodos Públicos

        public Dominio.Entidades.Embarcador.Financeiro.TituloBaixaCheque BuscarPorCodigo(int codigo)
        {
            var consultaTituloBaixaCheque = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Financeiro.TituloBaixaCheque>()
                .Where(o => o.Codigo == codigo);

            return consultaTituloBaixaCheque.FirstOrDefault();
        }

        public List<Dominio.Entidades.Embarcador.Financeiro.TituloBaixaCheque> BuscarPorChequeESituacaoTituloBaixaNaoCancelada(int codigo)
        {
            var consultaTituloBaixaCheque = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Financeiro.TituloBaixaCheque>()
                .Where(o => (o.Cheque.Codigo == codigo) && (o.TituloBaixa.SituacaoBaixaTitulo != Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoBaixaTitulo.Cancelada));

            return consultaTituloBaixaCheque.ToList();
        }

        public List<Dominio.Entidades.Embarcador.Financeiro.TituloBaixaCheque> BuscarPorTituloBaixa(int codigo)
        {
            var consultaTituloBaixaCheque = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Financeiro.TituloBaixaCheque>()
                .Where(o => o.TituloBaixa.Codigo == codigo);

            return consultaTituloBaixaCheque.ToList();
        }

        public List<string> BuscarNumeroChequePorTituloBaixa(int codigo)
        {
            IQueryable<Dominio.Entidades.Embarcador.Financeiro.TituloBaixaCheque> query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Financeiro.TituloBaixaCheque>();

            query = query.Where(o => o.TituloBaixa.Codigo == codigo);

            return query.Select(o => o.Cheque.NumeroCheque).Distinct().ToList();
        }

        public bool PossuiChequeBaixaTitulo(int codigo)
        {
            var consultaTituloBaixaCheque = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Financeiro.TituloBaixaCheque>()
                .Where(o => o.TituloBaixa.Codigo == codigo);

            return consultaTituloBaixaCheque.Count() > 0 ? true : false;
        }

        public decimal ObterValorTotalChequePorTituloBaixa(int codigo)
        {
            var consultaTituloBaixaCheque = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Financeiro.TituloBaixaCheque>()
                .Where(o => o.TituloBaixa.Codigo == codigo);

            return consultaTituloBaixaCheque.Sum(o => (decimal?)o.Cheque.Valor) ?? 0m;
        }

        #endregion
    }
}
