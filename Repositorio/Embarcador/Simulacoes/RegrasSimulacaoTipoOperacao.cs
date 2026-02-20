using System.Collections.Generic;
using System.Linq;
using System.Linq.Dynamic.Core;

namespace Repositorio.Embarcador.Simulacoes
{
    public class RegrasSimulacaoTipoOperacao : RepositorioBase<Dominio.Entidades.Embarcador.Simulacoes.RegrasSimulacaoTipoOperacao>
    {
        public RegrasSimulacaoTipoOperacao(UnitOfWork unitOfWork) : base(unitOfWork) { }

        public Dominio.Entidades.Embarcador.Simulacoes.RegrasSimulacaoTipoOperacao BuscarPorCodigo(int codigo)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Simulacoes.RegrasSimulacaoTipoOperacao>();
            var result = from obj in query where obj.Codigo == codigo select obj;
            return result.FirstOrDefault();
        }

        public List<Dominio.Entidades.Embarcador.Simulacoes.RegrasSimulacaoTipoOperacao> BuscarPorRegras(int codigo)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Simulacoes.RegrasSimulacaoTipoOperacao>();
            var result = from obj in query where obj.RegrasAutorizacaoSimulacao.Codigo == codigo select obj;

            return result.OrderBy("Ordem ascending").ToList();
        }
    }
}


