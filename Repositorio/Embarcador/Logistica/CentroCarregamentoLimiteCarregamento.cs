using System.Collections.Generic;
using System.Linq;

namespace Repositorio.Embarcador.Logistica
{
    public sealed class CentroCarregamentoLimiteCarregamento : RepositorioBase<Dominio.Entidades.Embarcador.Logistica.CentroCarregamentoLimiteCarregamento>
    {
        #region Construtores

        public CentroCarregamentoLimiteCarregamento(UnitOfWork unitOfWork) : base(unitOfWork) { }

        #endregion

        #region Métodos Públicos

        public List<Dominio.Entidades.Embarcador.Logistica.CentroCarregamentoLimiteCarregamento> BuscarPorCentrosDeCarregamentoEDia(List<int> codigosCentroCarregamento, Dominio.ObjetosDeValor.Embarcador.Enumeradores.DiaSemana dia)
        {
            var consultaCentroCarregamentoLimiteCarregamento = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Logistica.CentroCarregamentoLimiteCarregamento>()
                .Where(o => codigosCentroCarregamento.Contains(o.CentroCarregamento.Codigo) && o.Dia == dia);

            return consultaCentroCarregamentoLimiteCarregamento.ToList();
        }

        #endregion
    }
}
