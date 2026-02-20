using NHibernate.Linq;
using System.Collections.Generic;
using System.Linq;

namespace Repositorio.Embarcador.GestaoPatio
{
    public class CheckListCargaPerguntaAuditoria : RepositorioBase<Dominio.Entidades.Embarcador.GestaoPatio.CheckListCargaPerguntaAuditoria>
    {

        public CheckListCargaPerguntaAuditoria(UnitOfWork unitOfWork) : base(unitOfWork) { }

        public List<Dominio.Entidades.Embarcador.GestaoPatio.CheckListCargaPerguntaAuditoria> BuscarPorChecklist(int codigo)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.GestaoPatio.CheckListCargaPerguntaAuditoria>()
                .Where(obj => obj.CheckListCargaPergunta.CheckListCarga.Codigo == codigo);

            return query
                .Fetch(obj => obj.CheckListCargaPergunta)
                .ToList();
        }
    }
}
