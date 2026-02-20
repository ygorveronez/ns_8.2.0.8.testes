using System.Collections.Generic;
using System.Linq;

namespace Repositorio.Embarcador.Bidding.RFI
{
    public class RFIChecklistQuestionarioAlternativa : RepositorioBase<Dominio.Entidades.Embarcador.Bidding.RFI.RFIChecklistQuestionarioAlternativa>
    {
        public RFIChecklistQuestionarioAlternativa(UnitOfWork unitOfWork) : base(unitOfWork) { }

        public List<Dominio.Entidades.Embarcador.Bidding.RFI.RFIChecklistQuestionarioAlternativa> BuscarPorQuestionarios(List<int> CodigosQuestionarios)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Bidding.RFI.RFIChecklistQuestionarioAlternativa>()
                            .Where(x => CodigosQuestionarios.Contains(x.RFIChecklistQuestionario.Codigo));

            return query.ToList();
        }

        public void DeletarPorQuestionarioComExcecao(Dominio.Entidades.Embarcador.Bidding.RFI.RFIChecklistQuestionario RFIChecklistQuestionario, List<int> codigosAlternativas)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Bidding.RFI.RFIChecklistQuestionarioAlternativa>()
                            .Where(x => !codigosAlternativas.Contains(x.Codigo) && x.RFIChecklistQuestionario == RFIChecklistQuestionario);

            foreach (var alternativa in query)
            {
                this.SessionNHiBernate.Delete(alternativa);
            }
        }

        public void DeletarPorQuestionario(int CodigoQuestionario)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Bidding.RFI.RFIChecklistQuestionarioAlternativa>()
                            .Where(x => x.RFIChecklistQuestionario.Codigo == CodigoQuestionario);

            foreach (var alternativa in query)
            {
                this.SessionNHiBernate.Delete(alternativa);
            }
        }
    }
}