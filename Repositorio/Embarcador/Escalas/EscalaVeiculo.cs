using System.Linq;
using NHibernate.Linq;
using System.Linq.Dynamic.Core;
using System.Collections.Generic;
using System;

namespace Repositorio.Embarcador.Escalas
{
    public sealed class EscalaVeiculo : RepositorioBase<Dominio.Entidades.Embarcador.Escalas.EscalaVeiculo>
    {
        #region Construtores

        public EscalaVeiculo(UnitOfWork unitOfWork) : base(unitOfWork) { }

        #endregion

        #region Métodos Privados

        private IQueryable<Dominio.Entidades.Embarcador.Escalas.EscalaVeiculo> Consultar(Dominio.ObjetosDeValor.Embarcador.Escalas.FiltroPesquisaEscalaVeiculo filtrosPesquisa)
        {
            var consultaEscalaVeiculo = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Escalas.EscalaVeiculo>();

            if (filtrosPesquisa.CodigoVeiculo > 0)
                consultaEscalaVeiculo = consultaEscalaVeiculo.Where(o => o.Veiculo.Codigo == filtrosPesquisa.CodigoVeiculo);

            if (filtrosPesquisa.CodigoModeloVeicularCarga > 0)
                consultaEscalaVeiculo = consultaEscalaVeiculo.Where(o => o.Veiculo.ModeloVeicularCarga.Codigo == filtrosPesquisa.CodigoModeloVeicularCarga);

            if (filtrosPesquisa.Situacao.HasValue)
                consultaEscalaVeiculo = consultaEscalaVeiculo.Where(o => o.Situacao == filtrosPesquisa.Situacao.Value);

            if (filtrosPesquisa.SomenteVeiculosDataPrevisaoRetornoExcedida)
                consultaEscalaVeiculo = consultaEscalaVeiculo.Where(o => o.UltimoHistorico.DataPrevisaoRetorno.HasValue && o.UltimoHistorico.DataPrevisaoRetorno.Value < DateTime.Now.Date);

            return consultaEscalaVeiculo;
        }

        #endregion

        #region Métodos Públicos

        public Dominio.Entidades.Embarcador.Escalas.EscalaVeiculo BuscarPorVeiculo(int codigoVeiculo)
        {
            var consultaEscalaVeiculo = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Escalas.EscalaVeiculo>()
                .Where(o => o.Veiculo.Codigo == codigoVeiculo);

            return consultaEscalaVeiculo.FirstOrDefault();
        }

        public List<Dominio.Entidades.Embarcador.Escalas.EscalaVeiculo> BuscarPorVeiculoEmEscala()
        {
            var consultaEscalaVeiculo = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Escalas.EscalaVeiculo>()
                .Where(o => o.Situacao == Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoEscalaVeiculo.EmEscala && o.Veiculo.ModeloVeicularCarga.CapacidadePesoTransporte > 0);

            consultaEscalaVeiculo = consultaEscalaVeiculo
                .Fetch(o => o.Veiculo).ThenFetch(o => o.ModeloVeicularCarga)
                .Fetch(o => o.Veiculo).ThenFetch(o => o.Empresa);

            return consultaEscalaVeiculo.ToList();
        }

        public List<Dominio.Entidades.Embarcador.Escalas.EscalaVeiculo> Consultar(Dominio.ObjetosDeValor.Embarcador.Escalas.FiltroPesquisaEscalaVeiculo filtrosPesquisa, Dominio.ObjetosDeValor.Embarcador.Consulta.ParametroConsulta parametrosConsulta)
        {
            var consultaEscalaVeiculo = Consultar(filtrosPesquisa);

            consultaEscalaVeiculo = consultaEscalaVeiculo
                .Fetch(o => o.Veiculo).ThenFetch(o => o.ModeloVeicularCarga)
                .Fetch(o => o.UltimoHistorico);

            return ObterLista(consultaEscalaVeiculo, parametrosConsulta);
        }

        public int ContarConsulta(Dominio.ObjetosDeValor.Embarcador.Escalas.FiltroPesquisaEscalaVeiculo filtrosPesquisa)
        {
            var consultaEscalaVeiculo = Consultar(filtrosPesquisa);

            return consultaEscalaVeiculo.Count();
        }

        #endregion
    }
}
