using System.Collections.Generic;
using System.Linq;
using System.Linq.Dynamic.Core;

namespace Repositorio.Embarcador.Avarias
{
    public class LoteAvariaAnexos : RepositorioBase<Dominio.Entidades.Embarcador.Avarias.LoteAvariaAnexos>
    {
        public LoteAvariaAnexos(UnitOfWork unitOfWork) : base(unitOfWork) { }

        public Dominio.Entidades.Embarcador.Avarias.LoteAvariaAnexos BuscarPorCodigo(int codigo)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Avarias.LoteAvariaAnexos>();
            var result = from obj in query where obj.Codigo == codigo select obj;
            return result.FirstOrDefault();
        }

        public List<Dominio.Entidades.Embarcador.Avarias.LoteAvariaAnexos> BuscarPorLote(int codigo)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Avarias.LoteAvariaAnexos>();
            var result = from obj in query where obj.Lote.Codigo == codigo select obj;
            return result.ToList();
        }

        public List<Dominio.Entidades.Embarcador.Avarias.LoteAvariaAnexos> Consultar(int codigoLote, string propOrdenacao, string dirOrdenacao, int inicioRegistros, int maximoRegistros)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Avarias.LoteAvariaAnexos>();
            var result = from obj in query where obj.Lote.Codigo == codigoLote select obj;

            return result
                .OrderBy(propOrdenacao + (dirOrdenacao == "asc" ? " ascending" : " descending"))
                .Skip(inicioRegistros)
                .Take(maximoRegistros)
                .ToList();
        }

        public int ContarConsulta(int codigoLote)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Avarias.LoteAvariaAnexos>();
            var result = from obj in query where obj.Lote.Codigo == codigoLote select obj;

            return result.Count();
        }


    }
}
