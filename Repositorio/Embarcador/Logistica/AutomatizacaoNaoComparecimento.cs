using NHibernate.Linq;
using System.Collections.Generic;
using System.Linq;

namespace Repositorio.Embarcador.Logistica
{
    public sealed class AutomatizacaoNaoComparecimento : RepositorioBase<Dominio.Entidades.Embarcador.Logistica.AutomatizacaoNaoComparecimento>
    {
        #region Construtores

        public AutomatizacaoNaoComparecimento(UnitOfWork unitOfWork) : base(unitOfWork) { }

        #endregion Construtores

        #region Métodos Públicos

        public List<Dominio.Entidades.Embarcador.Logistica.AutomatizacaoNaoComparecimento> BuscarPorCentroCarregamento(int codigoCentroCarregamento)
        {
            var consultaAutomatizacaoNaoComparecimento = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Logistica.AutomatizacaoNaoComparecimento>()
                .Where(o => o.CentroCarregamento.Codigo == codigoCentroCarregamento);

            return consultaAutomatizacaoNaoComparecimento.ToList();
        }

        public List<Dominio.Entidades.Embarcador.Logistica.AutomatizacaoNaoComparecimento> BuscarTodasConfiguradas()
        {
            var consultaAutomatizacaoNaoComparecimento = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Logistica.AutomatizacaoNaoComparecimento>()
                .Where(o => o.CentroCarregamento.PermiteMarcarCargaComoNaoComparecimento == true);

            return consultaAutomatizacaoNaoComparecimento
                .Fetch(o => o.CentroCarregamento)
                .ToList();
        }

        #endregion Métodos Públicos
    }
}
