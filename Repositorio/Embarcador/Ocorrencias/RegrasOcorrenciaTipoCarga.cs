using System.Collections.Generic;
using System.Linq;
using System.Linq.Dynamic.Core;

namespace Repositorio.Embarcador.Ocorrencias
{
    public class RegrasOcorrenciaTipoCarga : RepositorioBase<Dominio.Entidades.Embarcador.Ocorrencias.RegrasOcorrenciaTipoCarga>
    {
        public RegrasOcorrenciaTipoCarga(UnitOfWork unitOfWork) : base(unitOfWork) { }

        public Dominio.Entidades.Embarcador.Ocorrencias.RegrasOcorrenciaTipoCarga BuscarPorCodigo(int codigo)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Ocorrencias.RegrasOcorrenciaTipoCarga>();
            var result = from obj in query where obj.Codigo == codigo select obj;
            return result.FirstOrDefault();
        }

        public List<Dominio.Entidades.Embarcador.Ocorrencias.RegrasOcorrenciaTipoCarga> BuscarPorRegras(int codigo)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Ocorrencias.RegrasOcorrenciaTipoCarga>();
            var result = from obj in query where obj.RegrasAutorizacaoOcorrencia.Codigo == codigo select obj;

            return result.OrderBy("Ordem ascending").ToList();
        }
    }
}

