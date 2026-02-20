using System.Linq;
using System.Linq.Dynamic.Core;
using System.Collections.Generic;
using NHibernate.Linq;
using System;

namespace Repositorio.Embarcador.Frete.Pontuacao
{
    public sealed class AdvertenciaTransportador : RepositorioBase<Dominio.Entidades.Embarcador.Frete.Pontuacao.AdvertenciaTransportador>
    {
        #region Construtores

        public AdvertenciaTransportador(UnitOfWork unitOfWork) : base(unitOfWork) { }

        #endregion

        #region Métodos Privados

        private IQueryable<Dominio.Entidades.Embarcador.Frete.Pontuacao.AdvertenciaTransportador> Consultar(Dominio.ObjetosDeValor.Embarcador.Frete.FiltroPesquisaAdvertenciaTransportador filtrosPesquisa)
        {
            var consultaAdvertenciaTransportador = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Frete.Pontuacao.AdvertenciaTransportador>();

            if (filtrosPesquisa.CodigoMotivo > 0)
                consultaAdvertenciaTransportador = consultaAdvertenciaTransportador.Where(o => o.Motivo.Codigo == filtrosPesquisa.CodigoMotivo);

            if (filtrosPesquisa.CodigoTransportador > 0)
                consultaAdvertenciaTransportador = consultaAdvertenciaTransportador.Where(o => o.Transportador.Codigo == filtrosPesquisa.CodigoTransportador);

            if (filtrosPesquisa.DataInicio.HasValue)
                consultaAdvertenciaTransportador = consultaAdvertenciaTransportador.Where(o => o.Data >= filtrosPesquisa.DataInicio.Value.Date);

            if (filtrosPesquisa.DataLimite.HasValue)
                consultaAdvertenciaTransportador = consultaAdvertenciaTransportador.Where(o => o.Data <= filtrosPesquisa.DataLimite.Value.Date.Add(DateTime.MaxValue.TimeOfDay));

            return consultaAdvertenciaTransportador;
        }

        #endregion

        #region Métodos Públicos

        public List<Dominio.Entidades.Embarcador.Frete.Pontuacao.AdvertenciaTransportador> Consultar(Dominio.ObjetosDeValor.Embarcador.Frete.FiltroPesquisaAdvertenciaTransportador filtrosPesquisa, Dominio.ObjetosDeValor.Embarcador.Consulta.ParametroConsulta parametrosConsulta)
        {
            var consultaRemocaoVeiculoEscala = Consultar(filtrosPesquisa);

            consultaRemocaoVeiculoEscala = consultaRemocaoVeiculoEscala
                .Fetch(o => o.Transportador)
                .Fetch(o => o.Motivo);

            return ObterLista(consultaRemocaoVeiculoEscala, parametrosConsulta);
        }

        public int ContarConsulta(Dominio.ObjetosDeValor.Embarcador.Frete.FiltroPesquisaAdvertenciaTransportador filtrosPesquisa)
        {
            var consultaRemocaoVeiculoEscala = Consultar(filtrosPesquisa);

            return consultaRemocaoVeiculoEscala.Count();
        }

        #endregion
    }
}
