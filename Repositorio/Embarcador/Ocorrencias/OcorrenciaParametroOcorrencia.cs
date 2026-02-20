using System.Collections.Generic;
using System.Linq;
using System.Linq.Dynamic.Core;

namespace Repositorio.Embarcador.Ocorrencias
{
    public class OcorrenciaParametroOcorrencia : RepositorioBase<Dominio.Entidades.Embarcador.Ocorrencias.OcorrenciaParametroOcorrencia>
    {

        public OcorrenciaParametroOcorrencia(UnitOfWork unitOfWork) : base(unitOfWork) { }

        public Dominio.Entidades.Embarcador.Ocorrencias.OcorrenciaParametroOcorrencia BuscarPorCodigo(int codigo)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Ocorrencias.OcorrenciaParametroOcorrencia>();
            var result = from obj in query where obj.Codigo == codigo select obj;
            return result.FirstOrDefault();
        }
        public int BuscarProximoNumero()
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Ocorrencias.OcorrenciaParametroOcorrencia>();

            int? retorno = query.Max(o => (int?)o.Numero);

            return retorno.HasValue ? retorno.Value + 1 : 1;
        }

        private IQueryable<Dominio.Entidades.Embarcador.Ocorrencias.OcorrenciaParametroOcorrencia> _Consultar()
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Ocorrencias.OcorrenciaParametroOcorrencia>();

            var result = from obj in query select obj;

            // Filtros

            return result;
        }

        public List<Dominio.Entidades.Embarcador.Ocorrencias.OcorrenciaParametroOcorrencia> Consultar(string propOrdena, string dirOrdena, int inicioRegistros, int maximoRegistros)
        {
            var result = _Consultar();

            if (!string.IsNullOrWhiteSpace(propOrdena))
                result = result.OrderBy(propOrdena + (dirOrdena == "asc" ? " ascending" : " descending"));

            if (inicioRegistros > 0)
                result = result.Skip(inicioRegistros);

            if (maximoRegistros > 0)
                result = result.Take(maximoRegistros);

            return result.ToList();
        }

        public int ContarConsulta()
        {
            var result = _Consultar();

            return result.Count();
        }
    }
}
