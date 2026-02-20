using System.Collections.Generic;
using System.Linq;
using System.Linq.Dynamic.Core;

namespace Repositorio.Embarcador.RH
{
    public class TabelaMediaModeloPeso : RepositorioBase<Dominio.Entidades.Embarcador.RH.TabelaMediaModeloPeso>
    {
        public TabelaMediaModeloPeso(UnitOfWork unitOfWork) : base(unitOfWork) { }

        public Dominio.Entidades.Embarcador.RH.TabelaMediaModeloPeso BuscarPorCodigo(int codigo)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.RH.TabelaMediaModeloPeso>();
            var result = from obj in query where obj.Codigo == codigo select obj;
            return result.FirstOrDefault();
        }

        public Dominio.Entidades.Embarcador.RH.TabelaMediaModeloPeso BuscarPorModeloPeso(int codigo, decimal peso)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.RH.TabelaMediaModeloPeso>();
            var result = from obj in query where obj.Modelo.Codigo == codigo && obj.PesoInicial <= peso && obj.PesoFinal >= peso select obj;
            return result.FirstOrDefault();
        }

        public decimal BuscarMediaPorModeloPeso(int codigo, decimal peso)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.RH.TabelaMediaModeloPeso>();
            var result = from obj in query where obj.Modelo.Codigo == codigo && obj.PesoInicial <= peso && obj.PesoFinal >= peso select obj;
            return result.Select(c => c.MediaIdeal)?.FirstOrDefault() ?? 0m;
        }

        public List<Dominio.Entidades.Embarcador.RH.TabelaMediaModeloPeso> Consultar(int codigoModelo, string propOrdenacao, string dirOrdenacao, int inicioRegistros, int maximoRegistros)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.RH.TabelaMediaModeloPeso>();

            var result = from obj in query select obj;

            if (codigoModelo > 0)
                result = result.Where(obj => obj.Modelo.Codigo == codigoModelo);

            return result.OrderBy(propOrdenacao + (dirOrdenacao == "asc" ? " ascending" : " descending")).Skip(inicioRegistros).Take(maximoRegistros).ToList();

        }

        public int ContarConsulta(int codigoModelo)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.RH.TabelaMediaModeloPeso>();

            var result = from obj in query select obj;

            if (codigoModelo > 0)
                result = result.Where(obj => obj.Modelo.Codigo == codigoModelo);

            return result.Count();
        }
    }
}
