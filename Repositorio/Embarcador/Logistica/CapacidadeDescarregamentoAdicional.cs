using System;
using System.Collections.Generic;
using System.Linq;

namespace Repositorio.Embarcador.Logistica
{
    public class CapacidadeDescarregamentoAdicional : RepositorioBase<Dominio.Entidades.Embarcador.Logistica.CapacidadeDescarregamentoAdicional>
    {
        #region Construtores

        public CapacidadeDescarregamentoAdicional(UnitOfWork unitOfWork) : base(unitOfWork) { }

        #endregion Construtores

        #region Métodos Privados

        public IQueryable<Dominio.Entidades.Embarcador.Logistica.CapacidadeDescarregamentoAdicional> Consultar(Dominio.ObjetosDeValor.Embarcador.Logistica.FiltroPesquisaCapacidadeDescarregamentoAdicional filtrosPesquisa)
        {
            var consultaCapacidadeDescarregamentoAdicional = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Logistica.CapacidadeDescarregamentoAdicional>();

            if (filtrosPesquisa.CodigoCentroDescarregamento > 0)
                consultaCapacidadeDescarregamentoAdicional = consultaCapacidadeDescarregamentoAdicional.Where(o => o.CentroDescarregamento.Codigo == filtrosPesquisa.CodigoCentroDescarregamento);

            if (filtrosPesquisa.DataInicial.HasValue)
                consultaCapacidadeDescarregamentoAdicional = consultaCapacidadeDescarregamentoAdicional.Where(o => o.Data.Date >= filtrosPesquisa.DataInicial.Value.Date);

            if (filtrosPesquisa.DataLimite.HasValue)
                consultaCapacidadeDescarregamentoAdicional = consultaCapacidadeDescarregamentoAdicional.Where(o => o.Data.Date <= filtrosPesquisa.DataLimite.Value.Date);

            return consultaCapacidadeDescarregamentoAdicional;
        }

        private bool FiltrarPorCanaisVenda(int codigoCentroDescarregamento)
        {
            return new PeriodoDescarregamentoCanalVenda(UnitOfWork).ExistePorCentroDescarregamento(codigoCentroDescarregamento);
        }

        #endregion Métodos Privados

        #region Métodos Públicos

        public int BuscarCapacidadeDescarregamento(int codigoCentroDescarregamento, DateTime data)
        {
            var consultaCapacidadeDescarregamentoAdicional = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Logistica.CapacidadeDescarregamentoAdicional>()
                .Where(o =>
                    o.CentroDescarregamento.Codigo == codigoCentroDescarregamento &&
                    o.Data == data.Date
                );

            return consultaCapacidadeDescarregamentoAdicional.Sum(o => (int?)o.CapacidadeDescarregamento) ?? 0;
        }

        public Dominio.Entidades.Embarcador.Logistica.CapacidadeDescarregamentoAdicional BuscarCapacidadeDescarregamentoAutomaticaPorDia(int codigoCentroDescarregamento, DateTime data)
        {
            var consultaCapacidadeDescarregamentoAdicional = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Logistica.CapacidadeDescarregamentoAdicional>()
                .Where(o =>
                    o.CentroDescarregamento.Codigo == codigoCentroDescarregamento &&
                    o.Usuario == null &&
                    o.Data == data.Date
                );

            return consultaCapacidadeDescarregamentoAdicional.FirstOrDefault();
        }

        public Dominio.Entidades.Embarcador.Logistica.CapacidadeDescarregamentoAdicional BuscarCapacidadeDescarregamentoAutomaticaPorPeriodo(int codigoCentroDescarregamento, DateTime data, TimeSpan horaInicio, TimeSpan horaTermino, List<int> codigosCanaisVenda)
        {
            DateTime periodoInicio = data.Date.Add(horaInicio);
            DateTime periodoTermino = data.Date.Add(horaTermino);

            var consultaCapacidadeDescarregamentoAdicional = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Logistica.CapacidadeDescarregamentoAdicional>()
                .Where(o =>
                    o.CentroDescarregamento.Codigo == codigoCentroDescarregamento &&
                    o.Usuario == null &&
                    o.Data == data.Date &&
                    (
                        (o.PeriodoInicio <= periodoInicio && o.PeriodoTermino >= periodoTermino) ||
                        (o.PeriodoInicio >= periodoInicio && o.PeriodoTermino <= periodoTermino)
                    )
                );

            if (FiltrarPorCanaisVenda(codigoCentroDescarregamento))
            {
                if (codigosCanaisVenda?.Count > 0)
                    consultaCapacidadeDescarregamentoAdicional = consultaCapacidadeDescarregamentoAdicional.Where(o => o.CanaisVenda.Any(canalVenda => codigosCanaisVenda.Contains(canalVenda.Codigo)));
                else
                    consultaCapacidadeDescarregamentoAdicional = consultaCapacidadeDescarregamentoAdicional.Where(o => o.CanaisVenda.Count == 0);
            }

            return consultaCapacidadeDescarregamentoAdicional.FirstOrDefault();
        }

        public int BuscarCapacidadeDescarregamentoPorPeriodo(int codigoCentroDescarregamento, DateTime data, TimeSpan horaInicio, TimeSpan horaTermino, List<int> codigosCanaisVenda)
        {
            DateTime periodoInicio = data.Date.Add(horaInicio);
            DateTime periodoTermino = data.Date.Add(horaTermino);

            var consultaCapacidadeDescarregamentoAdicional = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Logistica.CapacidadeDescarregamentoAdicional>()
                .Where(o =>
                    o.CentroDescarregamento.Codigo == codigoCentroDescarregamento &&
                    o.Data == data.Date &&
                    (
                        (o.PeriodoInicio <= periodoInicio && o.PeriodoTermino >= periodoTermino) ||
                        (o.PeriodoInicio >= periodoInicio && o.PeriodoTermino <= periodoTermino)
                    )
                );

            if (FiltrarPorCanaisVenda(codigoCentroDescarregamento))
            {
                if (codigosCanaisVenda?.Count > 0)
                    consultaCapacidadeDescarregamentoAdicional = consultaCapacidadeDescarregamentoAdicional.Where(o => o.CanaisVenda.Any(canalVenda => codigosCanaisVenda.Contains(canalVenda.Codigo)));
                else
                    consultaCapacidadeDescarregamentoAdicional = consultaCapacidadeDescarregamentoAdicional.Where(o => o.CanaisVenda.Count == 0);
            }

            return consultaCapacidadeDescarregamentoAdicional.Sum(o => (int?)o.CapacidadeDescarregamento) ?? 0;
        }

        public List<Dominio.Entidades.Embarcador.Logistica.CapacidadeDescarregamentoAdicional> Consultar(Dominio.ObjetosDeValor.Embarcador.Logistica.FiltroPesquisaCapacidadeDescarregamentoAdicional filtrosPesquisa, Dominio.ObjetosDeValor.Embarcador.Consulta.ParametroConsulta parametrosConsulta)
        {
            var consultaCapacidadeDescarregamentoAdicional = Consultar(filtrosPesquisa);

            return ObterLista(consultaCapacidadeDescarregamentoAdicional, parametrosConsulta);
        }

        public int ContarConsulta(Dominio.ObjetosDeValor.Embarcador.Logistica.FiltroPesquisaCapacidadeDescarregamentoAdicional filtrosPesquisa)
        {
            var consultaCapacidadeDescarregamentoAdicional = Consultar(filtrosPesquisa);

            return consultaCapacidadeDescarregamentoAdicional.Count();
        }

        #endregion Métodos Públicos
    }
}
