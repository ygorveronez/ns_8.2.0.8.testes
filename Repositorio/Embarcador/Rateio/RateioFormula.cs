using System.Collections.Generic;
using System.Linq;
using System.Linq.Dynamic.Core;

namespace Repositorio.Embarcador.Rateio
{
    public class RateioFormula : RepositorioBase<Dominio.Entidades.Embarcador.Rateio.RateioFormula>
    {
        public RateioFormula(UnitOfWork unitOfWork) : base(unitOfWork) { }

        public Dominio.Entidades.Embarcador.Rateio.RateioFormula BuscarPorCodigo(int codigo)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Rateio.RateioFormula>();
            var result = from obj in query where obj.Codigo == codigo select obj;
            return result.FirstOrDefault();
        }

        public Dominio.Entidades.Embarcador.Rateio.RateioFormula BuscarPorTipo(Dominio.ObjetosDeValor.Embarcador.Enumeradores.ParametroRateioFormula tipoRateio, List<Dominio.Entidades.Embarcador.Rateio.RateioFormula> lstRateioFormula = null)
        {
            if (lstRateioFormula == null)
                return this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Rateio.RateioFormula>().Where(obj => obj.ParametroRateioFormula == tipoRateio).FirstOrDefault();
            else
                return lstRateioFormula.Where(obj => obj.ParametroRateioFormula == tipoRateio).FirstOrDefault();
        }

        public List<Dominio.Entidades.Embarcador.Rateio.RateioFormula> Consultar(string descricao, Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoAtivoPesquisa ativo, string propOrdenacao, string dirOrdenacao, int inicioRegistros, int maximoRegistros)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Rateio.RateioFormula>();

            var result = from obj in query select obj;

            if (!string.IsNullOrWhiteSpace(descricao))
                result = result.Where(obj => obj.Descricao.Contains(descricao));

            if (ativo == Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoAtivoPesquisa.Ativo)
                result = result.Where(obj => obj.Ativo == true);

            if (ativo == Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoAtivoPesquisa.Inativo)
                result = result.Where(obj => obj.Ativo == false);

            return result.OrderBy(propOrdenacao + (dirOrdenacao == "asc" ? " ascending" : " descending")).Skip(inicioRegistros).Take(maximoRegistros).ToList();

        }

        public int ContarConsulta(string descricao, Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoAtivoPesquisa ativo)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Rateio.RateioFormula>();

            var result = from obj in query select obj;

            if (!string.IsNullOrWhiteSpace(descricao))
                result = result.Where(obj => obj.Descricao.Contains(descricao));

            if (ativo == Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoAtivoPesquisa.Ativo)
                result = result.Where(obj => obj.Ativo == true);

            if (ativo == Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoAtivoPesquisa.Inativo)
                result = result.Where(obj => obj.Ativo == false);

            return result.Count();
        }

        public bool ExisteFormulaRateioConfiguradaPorFatorPonderacao()
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Rateio.RateioFormula>()
                .Where(o => o.ParametroRateioFormula == Dominio.ObjetosDeValor.Embarcador.Enumeradores.ParametroRateioFormula.FatorPonderacaoDistanciaPeso);

            return query.Any();
        }
    }
}
