using System;
using System.Collections.Generic;
using System.Linq;

namespace Repositorio.Embarcador.Bidding
{
    public class BiddingAceitamentoQuestionarioAnexo : RepositorioBase<Dominio.Entidades.Embarcador.Bidding.BiddingAceitamentoQuestionarioAnexo>
    {
        public BiddingAceitamentoQuestionarioAnexo(UnitOfWork unitOfWork) : base(unitOfWork) { }

        public List<Dominio.Entidades.Embarcador.Bidding.BiddingAceitamentoQuestionarioAnexo> BuscarPorRespostas(List<Dominio.Entidades.Embarcador.Bidding.BiddingAceitamentoQuestaoResposta> respostas)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Bidding.BiddingAceitamentoQuestionarioAnexo>()
                .Where(o => respostas.Contains(o.EntidadeAnexo));

            return query.ToList();
        }

        public void DeletarTodosPorCodigo(int codigoResposta)
        {
            UnitOfWork.Sessao.CreateQuery("DELETE FROM BiddingAceitamentoQuestionarioAnexo c WHERE c.EntidadeAnexo.Codigo = :codigo").SetInt32("codigo", codigoResposta).ExecuteUpdate();
        }
    }
}
