using System.Linq;
using System.Linq.Dynamic.Core;
using System.Collections.Generic;
using System;

namespace Repositorio.Embarcador.Logistica
{
    public sealed class Manobra : RepositorioBase<Dominio.Entidades.Embarcador.Logistica.Manobra>
    {
        #region Construtores

        public Manobra(UnitOfWork unitOfWork) : base(unitOfWork) { }

        #endregion

        #region Métodos Privados

        private IQueryable<Dominio.Entidades.Embarcador.Logistica.Manobra> Consultar(Dominio.ObjetosDeValor.Embarcador.Logistica.FiltroPesquisaManobra filtrosPesquisa)
        {
            var consultaManobra = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Logistica.Manobra>();

            if (filtrosPesquisa.CodigoCentroCarregamento > 0)
                consultaManobra = consultaManobra.Where(o => o.CentroCarregamento.Codigo == filtrosPesquisa.CodigoCentroCarregamento);

            if (filtrosPesquisa.CodigoTransportador> 0)
                consultaManobra = consultaManobra.Where(o => o.Empresa.Codigo == filtrosPesquisa.CodigoTransportador);

            if (filtrosPesquisa.DataInicial.HasValue)
                consultaManobra = consultaManobra.Where(o => o.DataCriacao >= filtrosPesquisa.DataInicial.Value.Date);

            if (filtrosPesquisa.DataLimite.HasValue)
                consultaManobra = consultaManobra.Where(o => o.DataCriacao <= filtrosPesquisa.DataLimite.Value.Date.Add(DateTime.MaxValue.TimeOfDay));

            if (filtrosPesquisa.Situacoes?.Count() > 0)
                consultaManobra = consultaManobra.Where(o => filtrosPesquisa.Situacoes.Contains(o.Situacao));

            return consultaManobra;
        }

        #endregion

        #region Métodos Públicos

        public List<Dominio.Entidades.Embarcador.Logistica.Manobra> BuscarDisponivel(Dominio.ObjetosDeValor.Embarcador.Logistica.FiltroPesquisaManobraDisponivel filtrosPesquisa)
        {
            var consultaManobra = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Logistica.Manobra>()
                .Where(o => 
                    (
                        (o.CentroCarregamento.Codigo == filtrosPesquisa.CodigoCentroCarregamento) &&
                        (o.Tracao == null) &&
                        (
                            (o.Situacao == Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoManobra.AguardandoManobra) ||
                            (o.Situacao == Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoManobra.Reservada)
                        )
                    ) ||
                    (
                        (o.Codigo == filtrosPesquisa.CodigoManobraAtual) &&
                        (o.Situacao == Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoManobra.EmManobra)
                    )
                );

            return consultaManobra.ToList();
        }

        public Dominio.Entidades.Embarcador.Logistica.Manobra BuscarPorCodigo(int codigo)
        {
            var consultaManobra = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Logistica.Manobra>()
                .Where(o => o.Codigo == codigo);

            return consultaManobra.FirstOrDefault();
        }

        public Dominio.Entidades.Embarcador.Logistica.Manobra BuscarManobraAtualPorMotorista(int codigoMotorista)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Logistica.Manobra>();
            var result = from obj in query where obj.Situacao == Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoManobra.EmManobra select obj;

            var queryVeiculoMotorista = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Veiculos.VeiculoMotorista>();
            var resultQueryVeiculoMotorista = from obj in queryVeiculoMotorista where obj.Motorista.Codigo == codigoMotorista select obj;

            result = result.Where(o => resultQueryVeiculoMotorista.Where(a => a.Veiculo.Codigo == o.Codigo).Any());

            return result.FirstOrDefault();
        }

        public List<Dominio.Entidades.Embarcador.Logistica.Manobra> BuscarManobrasPorMotorista(int codigoMotorista)
        {

            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Logistica.Manobra>();
            var result = from obj in query where obj.Situacao == Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoManobra.EmManobra select obj;

            var queryVeiculoMotorista = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Veiculos.VeiculoMotorista>();
            var resultQueryVeiculoMotorista = from obj in queryVeiculoMotorista where obj.Motorista.Codigo == codigoMotorista select obj;

            result = result.Where(o => resultQueryVeiculoMotorista.Where(a => a.Veiculo.Codigo == o.Codigo).Any());

            return result.ToList();
        }

        public List<Dominio.Entidades.Embarcador.Logistica.Manobra> Consultar(Dominio.ObjetosDeValor.Embarcador.Logistica.FiltroPesquisaManobra filtrosPesquisa, Dominio.ObjetosDeValor.Embarcador.Consulta.ParametroConsulta parametrosConsulta)
        {
            var consultaManobra = Consultar(filtrosPesquisa);

            return ObterLista(consultaManobra, parametrosConsulta);
        }

        public int ContarConsulta(Dominio.ObjetosDeValor.Embarcador.Logistica.FiltroPesquisaManobra filtrosPesquisa)
        {
            var consultaManobra = Consultar(filtrosPesquisa);

            return consultaManobra.Count();
        }

        #endregion
    }
}
