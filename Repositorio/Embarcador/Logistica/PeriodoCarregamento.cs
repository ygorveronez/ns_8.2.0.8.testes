using Dominio.ObjetosDeValor.Embarcador.Enumeradores;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Dynamic.Core;

namespace Repositorio.Embarcador.Logistica
{
    public class PeriodoCarregamento : RepositorioBase<Dominio.Entidades.Embarcador.Logistica.PeriodoCarregamento>
    {
        #region Construtores

        public PeriodoCarregamento(UnitOfWork unitOfWork) : base(unitOfWork) { }

        #endregion

        #region Métodos Públicos

        public List<DiaSemana> BuscarDiasSemanaComPeriodosCarregamento(int codigoCentroCarregamento)
        {
            var consultaPeriodoCarregamento = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Logistica.PeriodoCarregamento>()
                .Where(o =>
                    o.CentroCarregamento.Codigo == codigoCentroCarregamento &&
                    o.ExclusividadeCarregamento == null &&
                    o.ExcecaoCapacidadeCarregamento == null &&
                    o.Dia != 0
                );

            return consultaPeriodoCarregamento
                .OrderBy(o => o.Dia)
                .Select(o => o.Dia)
                .Distinct()
                .ToList();
        }

        public List<Dominio.Entidades.Embarcador.Logistica.PeriodoCarregamento> BuscarPorCentroCarregamento(int codigoCentroCarregamento)
        {
            var consultaPeriodoCarregamento = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Logistica.PeriodoCarregamento>()
                .Where(o =>
                    o.CentroCarregamento.Codigo == codigoCentroCarregamento &&
                    o.ExclusividadeCarregamento == null &&
                    o.ExcecaoCapacidadeCarregamento == null
                );

            return consultaPeriodoCarregamento.OrderBy(o => o.HoraInicio).ToList();
        }

        public List<Dominio.Entidades.Embarcador.Logistica.PeriodoCarregamento> BuscarPorCentroCarregamentoEDia(int codigoCentroCarregamento, DiaSemana dia)
        {
            var consultaPeriodoCarregamento = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Logistica.PeriodoCarregamento>()
                .Where(o =>
                    o.CentroCarregamento.Codigo == codigoCentroCarregamento &&
                    o.ExclusividadeCarregamento == null &&
                    o.ExcecaoCapacidadeCarregamento == null &&
                    o.Dia == dia
                );

            return consultaPeriodoCarregamento.OrderBy(o => o.HoraInicio).ToList();
        }

        public Dominio.Entidades.Embarcador.Logistica.PeriodoCarregamento BuscarPorCodigo(int codigo)
        {
            var consultaPeriodoCarregamento = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Logistica.PeriodoCarregamento>()
                .Where(o => o.Codigo == codigo);

            return consultaPeriodoCarregamento.FirstOrDefault();
        }

        public List<Dominio.Entidades.Embarcador.Logistica.PeriodoCarregamento> BuscarPorExcecao(int codigoExcecao)
        {
            var consultaPeriodoCarregamento = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Logistica.PeriodoCarregamento>()
                .Where(o => o.ExcecaoCapacidadeCarregamento.Codigo == codigoExcecao);

            return consultaPeriodoCarregamento.OrderBy(obj => obj.HoraInicio).ToList();
        }

        #endregion
    }
}
