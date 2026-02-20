using NHibernate.Linq;
using System.Collections.Generic;
using System.Linq;

namespace Repositorio.Embarcador.Fechamento
{
    public class FechamentoFreteVeiculoUtilizado : RepositorioBase<Dominio.Entidades.Embarcador.Fechamento.FechamentoFreteVeiculoUtilizado>
    {
        #region Construtores

        public FechamentoFreteVeiculoUtilizado(UnitOfWork unitOfWork) : base(unitOfWork) { }

        #endregion Construtores

        #region Métodos Públicos

        public List<Dominio.Entidades.Embarcador.Fechamento.FechamentoFreteVeiculoUtilizado> BuscarPorFechamento(int codigoFechamento)
        {
            var consultaVeiculoUtilizado = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Fechamento.FechamentoFreteVeiculoUtilizado>()
                .Where(o => o.Fechamento.Codigo == codigoFechamento);

            consultaVeiculoUtilizado = consultaVeiculoUtilizado
                .Fetch(o => o.ModeloVeicularCarga)
                .Fetch(o => o.Tomador);

            return consultaVeiculoUtilizado.ToList();
        }

        #endregion Métodos Públicos
    }
}
