using System.Collections.Generic;
using System.Linq;
using System.Linq.Dynamic.Core;

namespace Repositorio.Embarcador.Seguros
{
    public class ApoliceSeguroAnexo : RepositorioBase<Dominio.Entidades.Embarcador.Seguros.ApoliceSeguroAnexo>
    {
        #region Construtores

        public ApoliceSeguroAnexo(UnitOfWork unitOfWork) : base(unitOfWork) { }

        #endregion

        public Dominio.Entidades.Embarcador.Seguros.ApoliceSeguroAnexo BuscarPorCodigo(int codigo)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Seguros.ApoliceSeguroAnexo>();

            var result = from obj in query where obj.Codigo == codigo select obj;

            return result.FirstOrDefault();
        }

        public List<Dominio.Entidades.Embarcador.Seguros.ApoliceSeguroAnexo> BuscarPorApolice(int codigoApolice)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Seguros.ApoliceSeguroAnexo>();

            var result = from obj in query where obj.ApoliceSeguro.Codigo == codigoApolice select obj;

            return result.ToList();
        }

        public List<Dominio.Entidades.Embarcador.Seguros.ApoliceSeguroAnexo> Consultar(int codigoApolice, string propOrdenacao, string dirOrdenacao, int inicioRegistros, int maximoRegistros)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Seguros.ApoliceSeguroAnexo>();
            var result = from obj in query where obj.ApoliceSeguro.Codigo == codigoApolice select obj;

            return result
                .OrderBy(propOrdenacao + (dirOrdenacao == "asc" ? " ascending" : " descending"))
                .Skip(inicioRegistros)
                .Take(maximoRegistros)
                .ToList();
        }

        public int ContarConsulta(int codigoApolice)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Seguros.ApoliceSeguroAnexo>();
            var result = from obj in query where obj.ApoliceSeguro.Codigo == codigoApolice select obj;

            return result.Count();
        }
    }
}
