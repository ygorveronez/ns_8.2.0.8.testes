using System.Collections.Generic;
using System.Linq;
using System.Linq.Dynamic.Core;

namespace Repositorio.Embarcador.Avarias
{
    public class SolicitacaoAvariaAnexos : RepositorioBase<Dominio.Entidades.Embarcador.Avarias.SolicitacaoAvariaAnexos>
    {
        public SolicitacaoAvariaAnexos(UnitOfWork unitOfWork) : base(unitOfWork) { }

        public Dominio.Entidades.Embarcador.Avarias.SolicitacaoAvariaAnexos BuscarPorCodigo(int codigo)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Avarias.SolicitacaoAvariaAnexos>();
            var result = from obj in query where obj.Codigo == codigo select obj;
            return result.FirstOrDefault();
        }

        public List<Dominio.Entidades.Embarcador.Avarias.SolicitacaoAvariaAnexos> BuscarPorSolicitacao(int codigo)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Avarias.SolicitacaoAvariaAnexos>();
            var result = from obj in query where obj.SolicitacaoAvaria.Codigo == codigo select obj;
            return result.ToList();
        }

        public List<Dominio.Entidades.Embarcador.Avarias.SolicitacaoAvariaAnexos> Consultar(int codigoSolicitacao, string propOrdenacao, string dirOrdenacao, int inicioRegistros, int maximoRegistros)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Avarias.SolicitacaoAvariaAnexos>();
            var result = from obj in query where obj.SolicitacaoAvaria.Codigo == codigoSolicitacao select obj;

            return result
                .OrderBy(propOrdenacao + (dirOrdenacao == "asc" ? " ascending" : " descending"))
                .Skip(inicioRegistros)
                .Take(maximoRegistros)
                .ToList();
        }

        public int ContarConsulta(int codigoSolicitacao)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Avarias.SolicitacaoAvariaAnexos>();
            var result = from obj in query where obj.SolicitacaoAvaria.Codigo == codigoSolicitacao select obj;

            return result.Count();
        }


    }
}
