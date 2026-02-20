using System.Collections.Generic;
using System.Linq;

namespace Repositorio.Embarcador.Bidding.RFI
{
    public class RFIChecklistQuestionarioAnexo : RepositorioBase<Dominio.Entidades.Embarcador.Bidding.RFI.RFIChecklistQuestionarioAnexo>
    {
        public RFIChecklistQuestionarioAnexo(UnitOfWork unitOfWork) : base(unitOfWork) { }

        public List<Dominio.Entidades.Embarcador.Bidding.RFI.RFIChecklistQuestionarioAnexo> BuscarPorQuestionarios(List<int> CodigosQuestionarios)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Bidding.RFI.RFIChecklistQuestionarioAnexo>()
                            .Where(x => CodigosQuestionarios.Contains(x.EntidadeAnexo.Codigo));

            return query.ToList();
        }

        public void DeletarPorQuestionario(int CodigoQuestionario)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Bidding.RFI.RFIChecklistQuestionarioAnexo>()
                            .Where(x => x.EntidadeAnexo.Codigo == CodigoQuestionario);

            foreach (var alternativa in query)
            {
                this.SessionNHiBernate.Delete(alternativa);
            }
        }
    }
}