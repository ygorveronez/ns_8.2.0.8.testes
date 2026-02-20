using System.Collections.Generic;
using System.Linq;
using System.Linq.Dynamic.Core;

namespace Repositorio.Embarcador.Logistica
{
    public class PeriodoCarregamentoTipoOperacaoSimultaneo : RepositorioBase<Dominio.Entidades.Embarcador.Logistica.PeriodoCarregamentoTipoOperacaoSimultaneo>
    {
        #region Construtores

        public PeriodoCarregamentoTipoOperacaoSimultaneo(UnitOfWork unitOfWork) : base(unitOfWork) { }

        #endregion

        public Dominio.Entidades.Embarcador.Logistica.PeriodoCarregamentoTipoOperacaoSimultaneo BuscarPorCodigoPeriodoETipoOperacao(int codigoPeriodo, int codigoTipoOperacao)
        {
            var consultaPeriodoCarregamento = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Logistica.PeriodoCarregamentoTipoOperacaoSimultaneo>()
                .Where(o => o.PeriodoCarregamento.Codigo == codigoPeriodo)
                .Where(o => o.TipoOperacao.Codigo == codigoTipoOperacao);

            return consultaPeriodoCarregamento.FirstOrDefault();
        }

        public List<Dominio.Entidades.Embarcador.Logistica.PeriodoCarregamentoTipoOperacaoSimultaneo> BuscarPorExcecao(int excecao)
        {
            var consultaPeriodoCarregamento = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Logistica.PeriodoCarregamentoTipoOperacaoSimultaneo>()
                .Where(o => o.PeriodoCarregamento.ExcecaoCapacidadeCarregamento.Codigo == excecao);

            return consultaPeriodoCarregamento.ToList();
        }

        public List<Dominio.Entidades.Embarcador.Logistica.PeriodoCarregamentoTipoOperacaoSimultaneo> BuscarPorExcecaoETipoOperacao(int excecao, int codigoTipoOperacao)
        {
            var consultaPeriodoCarregamento = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Logistica.PeriodoCarregamentoTipoOperacaoSimultaneo>()
                .Where(o => o.PeriodoCarregamento.ExcecaoCapacidadeCarregamento.Codigo == excecao);

            if (codigoTipoOperacao > 0)
                consultaPeriodoCarregamento = consultaPeriodoCarregamento.Where(o => o.TipoOperacao.Codigo == codigoTipoOperacao);

            return consultaPeriodoCarregamento.ToList();
        }
    }
}
