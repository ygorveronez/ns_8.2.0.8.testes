using System.Collections.Generic;
using System.Linq;

namespace Repositorio.Embarcador.Financeiro
{
    public class TipoMovimentoTipoDespesa : RepositorioBase<Dominio.Entidades.Embarcador.Financeiro.TipoMovimentoTipoDespesa>
    {
        public TipoMovimentoTipoDespesa(UnitOfWork unitOfWork) : base(unitOfWork) { }

        public List<Dominio.Entidades.Embarcador.Financeiro.TipoMovimentoTipoDespesa> BuscarPorTipoMovimento(int codigoTipoMovimento)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Financeiro.TipoMovimentoTipoDespesa>();

            var result = from obj in query where obj.TipoMovimento.Codigo == codigoTipoMovimento select obj;

            return result.ToList();
        }

        public Dominio.Entidades.Embarcador.Financeiro.TipoMovimentoTipoDespesa BuscarPorTipoMovimentoETipoDespesa(int codigoTipoMovimento, int codigoTipoDespesa)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Financeiro.TipoMovimentoTipoDespesa>();

            var result = from obj in query where obj.TipoMovimento.Codigo == codigoTipoMovimento && obj.TipoDespesaFinanceira.Codigo == codigoTipoDespesa select obj;

            return result.FirstOrDefault();
        }

        public Dominio.Entidades.Embarcador.Financeiro.Despesa.TipoDespesaFinanceira BuscarTipoDespesaFinanceira(int codigoTipoMovimento)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Financeiro.TipoMovimentoTipoDespesa>();

            var result = from obj in query where obj.TipoMovimento.Codigo == codigoTipoMovimento select obj;

            return result.FirstOrDefault()?.TipoDespesaFinanceira ?? null;
        }
    }
}
