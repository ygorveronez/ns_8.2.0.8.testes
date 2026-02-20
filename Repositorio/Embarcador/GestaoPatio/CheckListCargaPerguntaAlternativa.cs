using NHibernate.Linq;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Dynamic.Core;
using System.Threading;
using System.Threading.Tasks;

namespace Repositorio.Embarcador.GestaoPatio
{
    public sealed class CheckListCargaPerguntaAlternativa : RepositorioBase<Dominio.Entidades.Embarcador.GestaoPatio.CheckListCargaPerguntaAlternativa>
    {
        #region Construtores

        public CheckListCargaPerguntaAlternativa(UnitOfWork unitOfWork) : base(unitOfWork) { }
        public CheckListCargaPerguntaAlternativa(UnitOfWork unitOfWork, CancellationToken cancellationToken) : base(unitOfWork, cancellationToken) { }

        #endregion

        #region Métodos Públicos

        public List<Dominio.Entidades.Embarcador.GestaoPatio.CheckListCargaPerguntaAlternativa> BuscarPorPerguntas(List<int> codigosPerguntas)
        {
            var consultaAlternativas = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.GestaoPatio.CheckListCargaPerguntaAlternativa>()
                .Where(o => codigosPerguntas.Contains(o.CheckListCargaPergunta.Codigo));

            return consultaAlternativas.ToList();
        }

        public Task<List<Dominio.Entidades.Embarcador.GestaoPatio.CheckListCargaPerguntaAlternativa>> BuscarPorPerguntasAsync(List<int> codigosPerguntas)
        {
            var consultaAlternativas = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.GestaoPatio.CheckListCargaPerguntaAlternativa>()
                .Where(o => codigosPerguntas.Contains(o.CheckListCargaPergunta.Codigo));

            return consultaAlternativas.ToListAsync(CancellationToken);
        }

        public List<Dominio.Entidades.Embarcador.GestaoPatio.CheckListCargaPerguntaAlternativa> BuscarPorCheckList(int codigoCheckList)
        {
            var consultaAlternativas = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.GestaoPatio.CheckListCargaPerguntaAlternativa>()
                .Where(o => o.CheckListCargaPergunta.CheckListCarga.Codigo == codigoCheckList);

            return consultaAlternativas.ToList();
        }

        #endregion
    }
}
