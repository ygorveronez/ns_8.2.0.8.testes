using System;
using System.Collections.Generic;
using System.Linq;

namespace Repositorio.Embarcador.Logistica
{
    public class CapacidadeCarregamentoAdicional : RepositorioBase<Dominio.Entidades.Embarcador.Logistica.CapacidadeCarregamentoAdicional>
    {
        #region Construtores

        public CapacidadeCarregamentoAdicional(UnitOfWork unitOfWork) : base(unitOfWork) { }

        #endregion

        #region Métodos Privados

        public IQueryable<Dominio.Entidades.Embarcador.Logistica.CapacidadeCarregamentoAdicional> Consultar(Dominio.ObjetosDeValor.Embarcador.Logistica.FiltroPesquisaCapacidadeCarregamentoAdicional filtrosPesquisa)
        {
            var consultaCapacidadeCarregamentoAdicional = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Logistica.CapacidadeCarregamentoAdicional>();

            if (filtrosPesquisa.CodigoCentroCarregamento > 0)
                consultaCapacidadeCarregamentoAdicional = consultaCapacidadeCarregamentoAdicional.Where(o => o.CentroCarregamento.Codigo == filtrosPesquisa.CodigoCentroCarregamento);

            if (filtrosPesquisa.DataInicial.HasValue)
                consultaCapacidadeCarregamentoAdicional = consultaCapacidadeCarregamentoAdicional.Where(o => o.Data.Date >= filtrosPesquisa.DataInicial.Value.Date);

            if (filtrosPesquisa.DataLimite.HasValue)
                consultaCapacidadeCarregamentoAdicional = consultaCapacidadeCarregamentoAdicional.Where(o => o.Data.Date <= filtrosPesquisa.DataLimite.Value.Date);

            return consultaCapacidadeCarregamentoAdicional;
        }

        #endregion

        #region Métodos Públicos

        public int BuscarCapacidadeCarregamento(int codigoCentroCarregamento, DateTime data)
        {
            var consultaCapacidadeCarregamentoAdicional = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Logistica.CapacidadeCarregamentoAdicional>()
                .Where(o =>
                    o.CentroCarregamento.Codigo == codigoCentroCarregamento &&
                    o.Data == data.Date
                );

            return consultaCapacidadeCarregamentoAdicional.Sum(o => (int?)o.CapacidadeCarregamento) ?? 0;
        }

        public int BuscarCapacidadeCarregamentoVolume(int codigoCentroCarregamento, DateTime data)
        {
            var consultaCapacidadeCarregamentoAdicional = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Logistica.CapacidadeCarregamentoAdicional>()
                .Where(o =>
                    o.CentroCarregamento.Codigo == codigoCentroCarregamento &&
                    o.Data == data.Date
                );

            return consultaCapacidadeCarregamentoAdicional.Sum(o => (int?)o.CapacidadeCarregamentoVolume) ?? 0;
        }

        public int BuscarCapacidadeCarregamentoCubagem(int codigoCentroCarregamento, DateTime data)
        {
            var consultaCapacidadeCarregamentoAdicional = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Logistica.CapacidadeCarregamentoAdicional>()
                .Where(o =>
                    o.CentroCarregamento.Codigo == codigoCentroCarregamento &&
                    o.Data == data.Date
                );

            return consultaCapacidadeCarregamentoAdicional.Sum(o => (int?)o.CapacidadeCarregamentoCubagem) ?? 0;
        }

        public int BuscarCapacidadeCarregamentoPorPeriodo(int codigoCentroCarregamento, DateTime data, TimeSpan horaInicio, TimeSpan horaTermino)
        {
            DateTime periodoInicio = data.Date.Add(horaInicio);
            DateTime periodoTermino = data.Date.Add(horaTermino);

            var consultaCapacidadeCarregamentoAdicional = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Logistica.CapacidadeCarregamentoAdicional>()
                .Where(o => 
                    o.CentroCarregamento.Codigo == codigoCentroCarregamento &&
                    o.Data == data.Date &&
                    (
                        (o.PeriodoInicio <= periodoInicio && o.PeriodoTermino >= periodoTermino) ||
                        (o.PeriodoInicio >= periodoInicio && o.PeriodoTermino <= periodoTermino)
                    )
                );

            return consultaCapacidadeCarregamentoAdicional.Sum(o => (int?)o.CapacidadeCarregamento) ?? 0;
        }

        public List<Dominio.Entidades.Embarcador.Logistica.CapacidadeCarregamentoAdicional> Consultar(Dominio.ObjetosDeValor.Embarcador.Logistica.FiltroPesquisaCapacidadeCarregamentoAdicional filtrosPesquisa, Dominio.ObjetosDeValor.Embarcador.Consulta.ParametroConsulta parametrosConsulta)
        {
            var consultaCapacidadeCarregamentoAdicional = Consultar(filtrosPesquisa);

            return ObterLista(consultaCapacidadeCarregamentoAdicional, parametrosConsulta);
        }

        public int ContarConsulta(Dominio.ObjetosDeValor.Embarcador.Logistica.FiltroPesquisaCapacidadeCarregamentoAdicional filtrosPesquisa)
        {
            var consultaCapacidadeCarregamentoAdicional = Consultar(filtrosPesquisa);

            return consultaCapacidadeCarregamentoAdicional.Count();
        }

        public bool ExisteCapacidadeCarregamentoAutomatica(int codigoCentroCarregamento, DateTime data)
        {
            var consultaCapacidadeCarregamentoAdicional = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Logistica.CapacidadeCarregamentoAdicional>()
                .Where(o =>
                    o.CentroCarregamento.Codigo == codigoCentroCarregamento &&
                    o.Data == data.Date &&
                    o.Usuario == null
                );

            return consultaCapacidadeCarregamentoAdicional.Any();
        }

        #endregion
    }
}
