using System.Collections.Generic;
using System.Linq;
using System.Linq.Dynamic.Core;

namespace Repositorio.Embarcador.Ocorrencias
{
    public class RegrasOcorrenciaTipoOperacao : RepositorioBase<Dominio.Entidades.Embarcador.Ocorrencias.RegrasOcorrenciaTipoOperacao>
    {
        public RegrasOcorrenciaTipoOperacao(UnitOfWork unitOfWork) : base(unitOfWork) { }

        public Dominio.Entidades.Embarcador.Ocorrencias.RegrasOcorrenciaTipoOperacao BuscarPorCodigo(int codigo)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Ocorrencias.RegrasOcorrenciaTipoOperacao>();
            var result = from obj in query where obj.Codigo == codigo select obj;
            return result.FirstOrDefault();
        }

        public List<Dominio.Entidades.Embarcador.Ocorrencias.RegrasOcorrenciaTipoOperacao> BuscarPorRegras(int codigo)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Ocorrencias.RegrasOcorrenciaTipoOperacao>();
            var result = from obj in query where obj.RegrasAutorizacaoOcorrencia.Codigo == codigo select obj;

            return result.OrderBy("Ordem ascending").ToList();
        }
    }
}


