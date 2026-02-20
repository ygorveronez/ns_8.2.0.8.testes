using System.Collections.Generic;
using System.Linq;

namespace Repositorio.Embarcador.Fechamento
{

    public sealed class FechamentoFreteAcrescimoDesconto : RepositorioBase<Dominio.Entidades.Embarcador.Fechamento.FechamentoFreteAcrescimoDesconto>
    {
        #region Construtores

        public FechamentoFreteAcrescimoDesconto(UnitOfWork unitOfWork) : base(unitOfWork) { }

        #endregion

        #region Métodos Privados

        #endregion

        #region Métodos Públicos

        public List<Dominio.Entidades.Embarcador.Fechamento.FechamentoFreteAcrescimoDesconto> BuscarPorFechamento(int codigoFechamento)
        {
            var consultaFechamentoFreteAcrescimoDesconto = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Fechamento.FechamentoFreteAcrescimoDesconto>()
                .Where(o => o.Fechamento.Codigo == codigoFechamento);

            return consultaFechamentoFreteAcrescimoDesconto.ToList();
        }

        #endregion
    }

}
