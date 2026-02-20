using NHibernate.Linq;
using System.Collections.Generic;
using System.Linq;

namespace Repositorio.Embarcador.Fechamento
{
    public class FechamentoFreteMotoristaUtilizado : RepositorioBase<Dominio.Entidades.Embarcador.Fechamento.FechamentoFreteMotoristaUtilizado>
    {
        #region Construtores

        public FechamentoFreteMotoristaUtilizado(UnitOfWork unitOfWork) : base(unitOfWork) { }

        #endregion Construtores

        #region Métodos Públicos

        public List<Dominio.Entidades.Embarcador.Fechamento.FechamentoFreteMotoristaUtilizado> BuscarPorFechamento(int codigoFechamento)
        {
            var consultaMotoristaUtilizado = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Fechamento.FechamentoFreteMotoristaUtilizado>()
                .Where(o => o.Fechamento.Codigo == codigoFechamento);

            consultaMotoristaUtilizado = consultaMotoristaUtilizado
                .Fetch(o => o.Tomador);

            return consultaMotoristaUtilizado.ToList();
        }

        #endregion Métodos Públicos
    }
}
