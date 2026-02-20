using System.Collections.Generic;
using System.Linq;

namespace Repositorio.Embarcador.Contabeis
{
    public class CalculoISS : RepositorioBase<Dominio.Entidades.Embarcador.Contabeis.CalculoISS>
    {
        public CalculoISS(UnitOfWork unitOfWork) : base(unitOfWork) { }

        #region Métodos Públicos

        public Dominio.Entidades.Embarcador.Contabeis.CalculoISS BuscarTabela(int codigoLocalidade, string codigoServico)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Contabeis.CalculoISS>();
            query = query.Where(obj => obj.Localidade.Codigo == codigoLocalidade && obj.CodigoServico == codigoServico);
            return query.FirstOrDefault();
        }

        public List<Dominio.Entidades.Embarcador.Contabeis.CalculoISS> Consultar(int codigoLocalidade, string codigoServico, Dominio.ObjetosDeValor.Embarcador.Consulta.ParametroConsulta parametroConsulta)
        {
            var result = Consultar(codigoLocalidade, codigoServico);

            return ObterLista(result, parametroConsulta);
        }

        public int ContarConsulta(int codigoLocalidade, string codigoServico)
        {
            var result = Consultar(codigoLocalidade, codigoServico);

            return result.Count();
        }

        #endregion

        #region Métodos Privados

        private IQueryable<Dominio.Entidades.Embarcador.Contabeis.CalculoISS> Consultar(int codigoLocalidade, string codigoServico)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Contabeis.CalculoISS>();

            var result = from obj in query select obj;

            if (!string.IsNullOrWhiteSpace(codigoServico))
                result = result.Where(obj => obj.CodigoServico.Contains(codigoServico));

            if (codigoLocalidade > 0)
                result = result.Where(o => o.Localidade.Codigo == codigoLocalidade);

            return result;
        }

        #endregion
    }
}
