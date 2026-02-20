using System.Collections.Generic;
using System.Linq;
using System.Linq.Dynamic.Core;

namespace Repositorio.Embarcador.Chamados
{
    public class RegrasAtendimentoCanalVenda : RepositorioBase<Dominio.Entidades.Embarcador.Chamados.RegrasAtendimentoCanalVenda>
    {
        public RegrasAtendimentoCanalVenda(UnitOfWork unitOfWork) : base(unitOfWork) { }

        public Dominio.Entidades.Embarcador.Chamados.RegrasAtendimentoCanalVenda BuscarPorCodigo(int codigo)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Chamados.RegrasAtendimentoCanalVenda>();
            var result = from obj in query where obj.Codigo == codigo select obj;
            return result.FirstOrDefault();
        }

        public List<Dominio.Entidades.Embarcador.Chamados.RegrasAtendimentoCanalVenda> BuscarPorRegras(int codigo)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Chamados.RegrasAtendimentoCanalVenda>();
            var result = from obj in query where obj.RegrasAtendimentoChamados.Codigo == codigo select obj;

            return result.OrderBy("Ordem ascending").ToList();
        }

    }
}