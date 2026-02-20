using System.Collections.Generic;
using System.Linq;
using System.Linq.Dynamic.Core;

namespace Repositorio.Embarcador.Frete
{
    public class AlcadaReajusteFreteTipoOperacao : RepositorioBase<Dominio.Entidades.Embarcador.Frete.AlcadaReajusteFreteTipoOperacao>
    {
        public AlcadaReajusteFreteTipoOperacao(UnitOfWork unitOfWork) : base(unitOfWork) { }

        public Dominio.Entidades.Embarcador.Frete.AlcadaReajusteFreteTipoOperacao BuscarPorCodigo(int codigo)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Frete.AlcadaReajusteFreteTipoOperacao>();
            var result = from obj in query where obj.Codigo == codigo select obj;
            return result.FirstOrDefault();
        }

        public List<Dominio.Entidades.Embarcador.Frete.AlcadaReajusteFreteTipoOperacao> BuscarPorRegra(int codigo)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Frete.AlcadaReajusteFreteTipoOperacao>();
            var result = from obj in query where obj.RegraControleReajusteFretePlanilha.Codigo == codigo select obj;

            return result.OrderBy("Ordem ascending").ToList();
        }
    }
}
