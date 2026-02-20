using System.Linq;
using System.Linq.Dynamic.Core;
using System.Collections.Generic;
using System;

namespace Repositorio.Embarcador.Logistica
{
    public sealed class FilaCarregamentoVeiculoReversa : RepositorioBase<Dominio.Entidades.Embarcador.Logistica.FilaCarregamentoVeiculoReversa>
    {
        #region Construtores

        public FilaCarregamentoVeiculoReversa(UnitOfWork unitOfWork) : base(unitOfWork) { }

        #endregion

        #region Métodos Privados

        private IQueryable<Dominio.Entidades.Embarcador.Logistica.FilaCarregamentoVeiculoReversa> Consultar(Dominio.ObjetosDeValor.Embarcador.Logistica.FiltroPesquisaFilaCarregamentoVeiculoReversa filtrosPesquisa)
        {
            var consultaFilaCarregamentoVeiculoReversa = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Logistica.FilaCarregamentoVeiculoReversa>();

            if (filtrosPesquisa.CodigoCentroCarregamento > 0)
                consultaFilaCarregamentoVeiculoReversa = consultaFilaCarregamentoVeiculoReversa.Where(o => o.CentroCarregamento.Codigo == filtrosPesquisa.CodigoCentroCarregamento);

            if (filtrosPesquisa.CodigoGrupoModeloVeicularCarga > 0)
                consultaFilaCarregamentoVeiculoReversa = consultaFilaCarregamentoVeiculoReversa.Where(o => o.ConjuntoVeiculo.ModeloVeicularCarga.GrupoModeloVeicular.Codigo == filtrosPesquisa.CodigoGrupoModeloVeicularCarga);

            if (filtrosPesquisa.CodigoModeloVeicularCarga > 0)
                consultaFilaCarregamentoVeiculoReversa = consultaFilaCarregamentoVeiculoReversa.Where(o => o.ConjuntoVeiculo.ModeloVeicularCarga.Codigo == filtrosPesquisa.CodigoModeloVeicularCarga);

            if (filtrosPesquisa.DataInicial.HasValue)
                consultaFilaCarregamentoVeiculoReversa = consultaFilaCarregamentoVeiculoReversa.Where(o => o.DataCriacao >= filtrosPesquisa.DataInicial.Value.Date);

            if (filtrosPesquisa.DataLimite.HasValue)
                consultaFilaCarregamentoVeiculoReversa = consultaFilaCarregamentoVeiculoReversa.Where(o => o.DataCriacao <= filtrosPesquisa.DataLimite.Value.Date.Add(DateTime.MaxValue.TimeOfDay));

            if (filtrosPesquisa.Situacoes?.Count() > 0)
                consultaFilaCarregamentoVeiculoReversa = consultaFilaCarregamentoVeiculoReversa.Where(o => filtrosPesquisa.Situacoes.Contains(o.Situacao));

            return consultaFilaCarregamentoVeiculoReversa;
        }

        #endregion

        #region Métodos Públicos

        public List<Dominio.Entidades.Embarcador.Logistica.FilaCarregamentoVeiculoReversa> Consultar(Dominio.ObjetosDeValor.Embarcador.Logistica.FiltroPesquisaFilaCarregamentoVeiculoReversa filtrosPesquisa, Dominio.ObjetosDeValor.Embarcador.Consulta.ParametroConsulta parametrosConsulta)
        {
            var consultaFilaCarregamentoVeiculoReversa = Consultar(filtrosPesquisa);

            return ObterLista(consultaFilaCarregamentoVeiculoReversa, parametrosConsulta);
        }

        public int ContarConsulta(Dominio.ObjetosDeValor.Embarcador.Logistica.FiltroPesquisaFilaCarregamentoVeiculoReversa filtrosPesquisa)
        {
            var consultaFilaCarregamentoVeiculoReversa = Consultar(filtrosPesquisa);

            return consultaFilaCarregamentoVeiculoReversa.Count();
        }

        #endregion
    }
}
