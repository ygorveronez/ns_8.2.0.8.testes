using Dominio.ObjetosDeValor.Embarcador.Enumeradores;
using NHibernate.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Dynamic.Core;
using System.Threading;
using System.Threading.Tasks;

namespace Repositorio.Embarcador.Logistica
{
    public class PeriodoDescarregamento : RepositorioBase<Dominio.Entidades.Embarcador.Logistica.PeriodoDescarregamento>
    {
        #region Construtores

        public PeriodoDescarregamento(UnitOfWork unitOfWork) : base(unitOfWork) { }
        public PeriodoDescarregamento(UnitOfWork unitOfWork, CancellationToken cancellationToken) : base(unitOfWork, cancellationToken) { }

        #endregion Construtores

        #region Métodos Públicos

        public List<Dominio.Entidades.Embarcador.Logistica.PeriodoDescarregamento> BuscarPorCentroDescarregamento(int codigoCentroDescarregamento)
        {
            var consultaPeriodoDescarregamento = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Logistica.PeriodoDescarregamento>()
                .Where(o => o.CentroDescarregamento.Codigo == codigoCentroDescarregamento);

            return consultaPeriodoDescarregamento
                .OrderBy(o => o.HoraInicio).Distinct().ToList();
        }

        public List<Dominio.Entidades.Embarcador.Logistica.PeriodoDescarregamento> BuscarPorCentroDescarregamentoEDia(int codigoCentroDescarregamento, DiaSemana dia)
        {
            var consultaPeriodoDescarregamento = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Logistica.PeriodoDescarregamento>()
                .Where(o => o.CentroDescarregamento.Codigo == codigoCentroDescarregamento && o.Dia == dia);

            return consultaPeriodoDescarregamento.OrderBy(o => o.HoraInicio).ToList();
        }

        public List<(DiaSemana Dia, TimeSpan Inicio, TimeSpan Fim)> BuscarPorDestinatario(double cpfCnpj)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Logistica.PeriodoDescarregamento>()
                .Where(pd => pd.CentroDescarregamento.Destinatario.CPF_CNPJ == cpfCnpj);

            return query.Select(pd => ValueTuple.Create(
                    pd.Dia,
                    pd.HoraInicio,
                    pd.HoraTermino
                )).ToList();
        }

        public List<DiaSemana> BuscarDiasSemanaComPeriodosDescarregamento(int codigoCentroDescarregamento)
        {
            var consultaPeriodoDescarregamento = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Logistica.PeriodoDescarregamento>()
                .Where(o => o.CentroDescarregamento.Codigo == codigoCentroDescarregamento && o.Dia != 0);

            return consultaPeriodoDescarregamento
                .OrderBy(o => o.Dia)
                .Select(o => o.Dia)
                .Distinct()
                .ToList();
        }

        public Dominio.Entidades.Embarcador.Logistica.PeriodoDescarregamento BuscarPorCodigo(int codigo)
        {
            var consultaPeriodoDescarregamento = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Logistica.PeriodoDescarregamento>()
                .Where(o => o.Codigo == codigo);

            return consultaPeriodoDescarregamento.FirstOrDefault();
        }

        public Task<Dominio.Entidades.Embarcador.Logistica.PeriodoDescarregamento> BuscarPorCodigoAsync(int codigo)
        {
            var consultaPeriodoDescarregamento = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Logistica.PeriodoDescarregamento>()
                .Where(o => o.Codigo == codigo);

            return consultaPeriodoDescarregamento.FirstOrDefaultAsync(CancellationToken);
        }

        public List<Dominio.Entidades.Embarcador.Logistica.PeriodoDescarregamento> BuscarPorDiaMes(int codigoCentro, int dia, int mes)
        {
            var consultaPeriodoDescarregamento = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Logistica.PeriodoDescarregamento>()
                .Where(o => o.CentroDescarregamento.Codigo == codigoCentro && o.DiaDoMes == dia && o.Mes == mes);

            return consultaPeriodoDescarregamento.OrderBy(o => o.HoraInicio).ToList();
        }

        public List<Dominio.Entidades.Embarcador.Logistica.PeriodoDescarregamento> BuscarPorExcecao(int codigoExcecao)
        {
            var consultaPeriodoDescarregamento = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Logistica.PeriodoDescarregamento>()
                .Where(o => o.ExcecaoCapacidadeDescarregamento.Codigo == codigoExcecao);

            return consultaPeriodoDescarregamento.OrderBy(o => o.HoraInicio).ToList();
        }

        public List<Dominio.Entidades.Embarcador.Cargas.TipoDeCarga> BuscarTiposDeCargaPorPeriodoCentroDescarregamento(Dominio.ObjetosDeValor.Embarcador.Logistica.FiltroPesquisaRelatorioJanelaDisponivelAgendamento filtrosPesquisa)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Logistica.PeriodoDescarregamentoTipoDeCarga>();

            query = query.Where(obj => obj.PeriodoDescarregamento.CentroDescarregamento.Codigo == filtrosPesquisa.CodigoCentroDescarregamento);

            return query
                .Select(obj => obj.TipoDeCarga)
                .Distinct()
                .ToList();
        }

        public List<Dominio.Entidades.Embarcador.Logistica.PeriodoDescarregamento> BuscarPorDestinatariosTipoCarga(List<double> cpfCnpjDestinatarios, int codigoTipoCarga)
        {
            var consultaCargaJanelaDescarregamento = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.CargaJanelaDescarregamento>()
                .Where(obj => cpfCnpjDestinatarios.Contains(obj.CentroDescarregamento.Destinatario.CPF_CNPJ));

            var consultaPeriodos = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Logistica.PeriodoDescarregamento>()
                .Where(obj => consultaCargaJanelaDescarregamento.Select(x => x.CentroDescarregamento.Codigo).Contains(obj.CentroDescarregamento.Codigo));

            if (codigoTipoCarga > 0)
                consultaPeriodos = consultaPeriodos.Where(obj => obj.TiposDeCarga.Any(x => x.TipoDeCarga.Codigo == codigoTipoCarga));

            return consultaPeriodos
                .OrderBy(obj => obj.Dia)
                .ThenBy(obj => obj.HoraInicio)
                .ToList();
        }

        #endregion Métodos Públicos
    }
}
