using System.Collections.Generic;
using System.Linq;
using System.Linq.Dynamic.Core;

namespace Repositorio.Embarcador.Frete.Pontuacao
{
    public sealed class FechamentoPontuacaoTransportadorRegra : RepositorioBase<Dominio.Entidades.Embarcador.Frete.Pontuacao.FechamentoPontuacaoTransportadorRegra>
    {
        #region Construtores

        public FechamentoPontuacaoTransportadorRegra(UnitOfWork unitOfWork) : base(unitOfWork) { }

        #endregion

        #region Métodos Públicos

        public List<Dominio.Entidades.Embarcador.Frete.Pontuacao.FechamentoPontuacaoTransportadorRegra> BuscarPorFechamentoPontuacaoTransportador(int codigoFechamentoPontuacaoTransportador)
        {
            var consultaFechamentoPontuacaoTransportadorRegra = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Frete.Pontuacao.FechamentoPontuacaoTransportadorRegra>()
                .Where(o => o.FechamentoPontuacaoTransportador.Codigo == codigoFechamentoPontuacaoTransportador);

            return consultaFechamentoPontuacaoTransportadorRegra.ToList();
        }

        #endregion
    }
}
