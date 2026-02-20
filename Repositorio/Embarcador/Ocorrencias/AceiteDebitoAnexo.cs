using System.Collections.Generic;
using System.Linq;
using System.Linq.Dynamic.Core;

namespace Repositorio.Embarcador.Ocorrencias
{
    public class AceiteDebitoAnexo : RepositorioBase<Dominio.Entidades.Embarcador.Ocorrencias.AceiteDebitoAnexo>
    {
        public AceiteDebitoAnexo(UnitOfWork unitOfWork) : base(unitOfWork) { }

        public Dominio.Entidades.Embarcador.Ocorrencias.AceiteDebitoAnexo BuscarPorCodigo(int codigo)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Ocorrencias.AceiteDebitoAnexo>();
            var result = from obj in query where obj.Codigo == codigo select obj;
            return result.FirstOrDefault();
        }

        public List<Dominio.Entidades.Embarcador.Ocorrencias.AceiteDebitoAnexo> BuscarPorCodigoAceite(int codigo)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Ocorrencias.AceiteDebitoAnexo>();
            var result = from obj in query where obj.AceiteDebito.Codigo == codigo select obj;
            return result.ToList();
        }

        public List<Dominio.Entidades.Embarcador.Ocorrencias.AceiteDebitoAnexo> Consultar(int ocorrencia, string propOrdenacao, string dirOrdenacao, int inicioRegistros, int maximoRegistros)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Ocorrencias.AceiteDebitoAnexo>();
            var result = from obj in query where obj.AceiteDebito.Ocorrencia.Codigo == ocorrencia select obj;

            return result
                .OrderBy(propOrdenacao + (dirOrdenacao == "asc" ? " ascending" : " descending"))
                .Skip(inicioRegistros)
                .Take(maximoRegistros)
                .ToList();
        }

        public int ContarConsulta(int ocorrencia)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Ocorrencias.AceiteDebitoAnexo>();
            var result = from obj in query where obj.AceiteDebito.Ocorrencia.Codigo == ocorrencia select obj;

            return result.Count();
        }
    }
}
