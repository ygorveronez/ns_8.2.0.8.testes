using System.Collections.Generic;
using System.Linq;
using System.Linq.Dynamic.Core;

namespace Repositorio.Embarcador.Chamados
{
    public class RegrasAtendimentoFilial : RepositorioBase<Dominio.Entidades.Embarcador.Chamados.RegrasAtendimentoFilial>
    {
        public RegrasAtendimentoFilial(UnitOfWork unitOfWork) : base(unitOfWork) { }

        public Dominio.Entidades.Embarcador.Chamados.RegrasAtendimentoFilial BuscarPorCodigo(int codigo)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Chamados.RegrasAtendimentoFilial>();
            var result = from obj in query where obj.Codigo == codigo select obj;
            return result.FirstOrDefault();
        }

        public List<Dominio.Entidades.Embarcador.Chamados.RegrasAtendimentoFilial> BuscarPorRegras(int codigo)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Chamados.RegrasAtendimentoFilial>();
            var result = from obj in query where obj.RegrasAtendimentoChamados.Codigo == codigo select obj;

            return result.OrderBy("Ordem ascending").ToList();
        }

    }
}