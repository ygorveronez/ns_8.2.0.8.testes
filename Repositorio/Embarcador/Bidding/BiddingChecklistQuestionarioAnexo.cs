using System;
using System.Collections.Generic;
using System.Linq;

namespace Repositorio.Embarcador.Bidding
{
    public class BiddingChecklistQuestionarioAnexo : RepositorioBase<Dominio.Entidades.Embarcador.Bidding.BiddingChecklistQuestionarioAnexo>
    {
        public BiddingChecklistQuestionarioAnexo(UnitOfWork unitOfWork) : base(unitOfWork) { }

        public List<Dominio.Entidades.Embarcador.Bidding.BiddingChecklistQuestionarioAnexo> BuscarAnexos(Dominio.Entidades.Embarcador.Bidding.BiddingChecklist entidadeBiddingChecklist)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Bidding.BiddingChecklistQuestionarioAnexo>()
                .Where(o => o.EntidadeAnexo.Checklist == entidadeBiddingChecklist);

            return query.ToList();
        }

        public void DeletarTodosPorCodigo(int codigoQuestionario)
        {
            UnitOfWork.Sessao.CreateQuery("DELETE FROM BiddingChecklistQuestionarioAnexo c WHERE c.EntidadeAnexo.Codigo = :codigo").SetInt32("codigo", codigoQuestionario).ExecuteUpdate();
        }
    }
}
