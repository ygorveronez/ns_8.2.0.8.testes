using System.Collections.Generic;
using System.Linq;
using System.Linq.Dynamic.Core;

namespace Repositorio.Embarcador.ICMS
{
    public class CoeficientePautaFiscal : Repositorio.RepositorioBase<Dominio.Entidades.Embarcador.ICMS.CoeficientePautaFiscal>
    {
        #region Construtores

        public CoeficientePautaFiscal(UnitOfWork unitOfWork) : base(unitOfWork) { }

        #endregion

        #region Métodos Publicos

        public Dominio.Entidades.Embarcador.ICMS.CoeficientePautaFiscal BuscarPorCodigo(int codigo)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.ICMS.CoeficientePautaFiscal>();
            var result = from obj in query where obj.Codigo == codigo select obj;
            return result.FirstOrDefault();
        }

        public List<Dominio.Entidades.Embarcador.ICMS.CoeficientePautaFiscal> Consultar(string siglaEstado, int mes, int ano, Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoAtivoPesquisa ativo, string propOrdenacao, string dirOrdenacao, int inicioRegistros, int maximoRegistros)
        {
            var query = CriarConsulta(siglaEstado, mes, ano, ativo);

            return query.OrderBy(propOrdenacao + (dirOrdenacao == "asc" ? " ascending" : " descending")).Skip(inicioRegistros).Take(maximoRegistros).ToList();
        }

        public int ContarConsulta(string siglaEstado, int mes, int ano, Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoAtivoPesquisa ativo)
        {
            var query = CriarConsulta(siglaEstado, mes, ano, ativo);

            return query.Count();
        }

        public Dominio.Entidades.Embarcador.ICMS.CoeficientePautaFiscal BuscarPorEstadoMesAno(string siglaEstado, int mes, int ano)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.ICMS.CoeficientePautaFiscal>();
            var result = from obj in query where obj.Estado.Sigla == siglaEstado && obj.Mes == mes && obj.Ano == ano && ((bool?)obj.Ativo ?? true) == true select obj;
            return result.FirstOrDefault();
        }

        #endregion

        #region Métodos Privados

        private IQueryable<Dominio.Entidades.Embarcador.ICMS.CoeficientePautaFiscal> CriarConsulta(string siglaEstado, int mes, int ano, Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoAtivoPesquisa ativo)
        {
            var result = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.ICMS.CoeficientePautaFiscal>();

            if (!string.IsNullOrWhiteSpace(siglaEstado))
                result = result.Where(obj => obj.Estado.Sigla.Equals(siglaEstado));

            if (mes > 0)
                result = result.Where(obj => obj.Mes == mes);

            if (ano > 0)
                result = result.Where(obj => obj.Ano == ano);

            if (ativo == Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoAtivoPesquisa.Ativo)
                result = result.Where(o => ((bool?)o.Ativo ?? true) == true);
            else if (ativo == Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoAtivoPesquisa.Inativo)
                result = result.Where(o => !o.Ativo);

            return result;
        }

        #endregion
    }
}
