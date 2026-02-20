using System.Collections.Generic;
using System.Linq;

namespace Repositorio.Embarcador.Checklist
{
    public class CheckListRespostaPerguntaAlternativa : RepositorioBase<Dominio.Entidades.Embarcador.Checklist.CheckListRespostaPerguntaAlternativa>
    {
        #region Construtores

        public CheckListRespostaPerguntaAlternativa(UnitOfWork unitOfWork) : base(unitOfWork) { }

        #endregion

        #region Métodos Públicos

        public List<Dominio.Entidades.Embarcador.Checklist.CheckListRespostaPerguntaAlternativa> BuscarPorPerguntas(List<int> codigosPerguntas)
        {
            var consultaAlternativas = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Checklist.CheckListRespostaPerguntaAlternativa>()
                .Where(o => codigosPerguntas.Contains(o.CheckListRespostaPergunta.Codigo));

            return consultaAlternativas.ToList();
        }

        public List<Dominio.Entidades.Embarcador.Checklist.CheckListRespostaPerguntaAlternativa> BuscarPorCheckList(int codigoCheckList)
        {
            var consultaAlternativas = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Checklist.CheckListRespostaPerguntaAlternativa>()
                .Where(o => o.CheckListRespostaPergunta.CheckListResposta.Codigo == codigoCheckList);

            return consultaAlternativas.ToList();
        }

        #endregion
    }
}