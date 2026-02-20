using System.Collections.Generic;
using System.Linq;
using System.Linq.Dynamic.Core;

namespace Repositorio.Embarcador.Chamados
{
    public class RegrasAtendimentoTipoOperacao : RepositorioBase<Dominio.Entidades.Embarcador.Chamados.RegrasAtendimentoTipoOperacao>
    {
        public RegrasAtendimentoTipoOperacao(UnitOfWork unitOfWork) : base(unitOfWork) { }

        public Dominio.Entidades.Embarcador.Chamados.RegrasAtendimentoTipoOperacao BuscarPorCodigo(int codigo)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Chamados.RegrasAtendimentoTipoOperacao>();
            var result = from obj in query where obj.Codigo == codigo select obj;
            return result.FirstOrDefault();
        }

        public List<Dominio.Entidades.Embarcador.Chamados.RegrasAtendimentoTipoOperacao> BuscarPorRegras(int codigo)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Chamados.RegrasAtendimentoTipoOperacao>();
            var result = from obj in query where obj.RegrasAtendimentoChamados.Codigo == codigo select obj;

            return result.OrderBy("Ordem ascending").ToList();
        }

    }
}