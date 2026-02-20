using System;
using System.Linq;
using System.Collections.Generic;
using NHibernate.Linq;

namespace Repositorio.Embarcador.Frotas
{
    public class PlanejamentoFrotaMesVeiculo : RepositorioBase<Dominio.Entidades.Embarcador.Frotas.PlanejamentoFrotaMesVeiculo>
    {
        #region Construtores

        public PlanejamentoFrotaMesVeiculo(UnitOfWork unitOfWork) : base(unitOfWork) { }

        #endregion Construtores

        #region Métodos Públicos

        public List<Dominio.Entidades.Embarcador.Frotas.PlanejamentoFrotaMesVeiculo> ObterPlanejamentosDoMes(Dominio.ObjetosDeValor.Embarcador.Frotas.FiltroPesquisaPlanejamentoFrotaMesVeiculo filtrosPesquisa)
        {
            var consultaPlanejamentoFrotaMesVeiculo = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Frotas.PlanejamentoFrotaMesVeiculo>();

            if (filtrosPesquisa.CodigoFilial > 0)
                consultaPlanejamentoFrotaMesVeiculo = consultaPlanejamentoFrotaMesVeiculo.Where(planejamentoVeiculo => planejamentoVeiculo.PlanejamentoFrotaMes.Filial.Codigo == filtrosPesquisa.CodigoFilial);

            if (filtrosPesquisa.CodigoModeloVeicularCarga > 0)
                consultaPlanejamentoFrotaMesVeiculo = consultaPlanejamentoFrotaMesVeiculo.Where(planejamentoVeiculo => planejamentoVeiculo.ModeloVeicular.Codigo == filtrosPesquisa.CodigoModeloVeicularCarga);

            if (filtrosPesquisa.CodigoTransportador > 0)
                consultaPlanejamentoFrotaMesVeiculo = consultaPlanejamentoFrotaMesVeiculo.Where(planejamentoVeiculo => planejamentoVeiculo.Veiculo.Empresa.Codigo == filtrosPesquisa.CodigoTransportador);

            if (filtrosPesquisa.CodigoTipoOperacao > 0)
                consultaPlanejamentoFrotaMesVeiculo = consultaPlanejamentoFrotaMesVeiculo.Where(planejamentoVeiculo => planejamentoVeiculo.TipoOperacao.Codigo == filtrosPesquisa.CodigoTipoOperacao);

            if (filtrosPesquisa.CodigoPlanejamentoFrotaMes > 0)
                consultaPlanejamentoFrotaMesVeiculo = consultaPlanejamentoFrotaMesVeiculo.Where(planejamentoVeiculo => planejamentoVeiculo.PlanejamentoFrotaMes.Codigo == filtrosPesquisa.CodigoPlanejamentoFrotaMes);

            if (filtrosPesquisa.Situacao.HasValue)
                consultaPlanejamentoFrotaMesVeiculo = consultaPlanejamentoFrotaMesVeiculo.Where(planejamentoVeiculo => planejamentoVeiculo.Situacao == filtrosPesquisa.Situacao.Value);

            if (filtrosPesquisa.RespostaTransportador.HasValue)
                consultaPlanejamentoFrotaMesVeiculo = consultaPlanejamentoFrotaMesVeiculo.Where(planejamentoVeiculo => planejamentoVeiculo.RespostaDoTransportador == filtrosPesquisa.RespostaTransportador.Value);

            if (filtrosPesquisa.Ano > 0)
                consultaPlanejamentoFrotaMesVeiculo = consultaPlanejamentoFrotaMesVeiculo.Where(planejamentoVeiculo => planejamentoVeiculo.PlanejamentoFrotaMes.Data.Year == filtrosPesquisa.Ano);

            if (filtrosPesquisa.Mes > 0)
                consultaPlanejamentoFrotaMesVeiculo = consultaPlanejamentoFrotaMesVeiculo.Where(planejamentoVeiculo => planejamentoVeiculo.PlanejamentoFrotaMes.Data.Month == filtrosPesquisa.Mes);

            return consultaPlanejamentoFrotaMesVeiculo
                .Fetch(planejamentoVeiculo => planejamentoVeiculo.PlanejamentoFrotaMes).ThenFetch(planejamento => planejamento.Filial)
                .ToList();
        }

        public List<Dominio.Entidades.Embarcador.Frotas.PlanejamentoFrotaMesVeiculo> ObterPlanejamentoVeiculosPorTransportador(List<int> transportadores, int codigoPlanejamento)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Frotas.PlanejamentoFrotaMesVeiculo>();

            query = query.Where(x => x.PlanejamentoFrotaMes.Codigo == codigoPlanejamento);
            query = query.Where(x => transportadores.Contains(x.Veiculo.Empresa.Codigo));

            return query
                .Fetch(planejamentoVeiculo => planejamentoVeiculo.PlanejamentoFrotaMes).ThenFetch(planejamento => planejamento.Filial)
                .ToList();
        }

        public List<Dominio.Entidades.Embarcador.Frotas.PlanejamentoFrotaMesVeiculo> ObterPlanejamentosPendentesDeConfirmacao(int ano, int mes)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Frotas.PlanejamentoFrotaMesVeiculo>();
            if (mes > 0 && ano > 1990)
            {
                var dataMin = new DateTime(ano, mes, 1, 0, 0, 0);
                var dias = DateTime.DaysInMonth(ano, mes);
                var dataMax = new DateTime(ano, mes, dias, 0, 0, 0).AddDays(1);
                query = query.Where(x => x.PlanejamentoFrotaMes.Data >= dataMin && x.PlanejamentoFrotaMes.Data < dataMax);
            }
            query = query.Where(x => x.RespostaDoTransportador == Dominio.ObjetosDeValor.Embarcador.Enumeradores.RespostaTransportadorPlanejamentoFrota.Pendente ||
                x.Situacao == Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoPlanejamentoFrota.EmAnaliseEmbarcador ||
                x.Situacao == Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoPlanejamentoFrota.EmAnaliseTransportador);
            return query.ToList();
        }

        public List<string> VerificarPlacasJaPlanejadasEmOutraFilial(List<string> placas, int codFilial, DateTime mes)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Frotas.PlanejamentoFrotaMesVeiculo>();
            if (mes > DateTime.MinValue)
            {
                var dataMin = new DateTime(mes.Year, mes.Month, 1, 0, 0, 0);
                var dias = DateTime.DaysInMonth(mes.Year, mes.Month);
                var dataMax = new DateTime(mes.Year, mes.Month, dias, 0, 0, 0).AddDays(1);
                query = query.Where(x => x.PlanejamentoFrotaMes.Data >= dataMin && x.PlanejamentoFrotaMes.Data < dataMax);
            }
            if (codFilial > 0)
                query = query.Where(x => x.PlanejamentoFrotaMes.Filial.Codigo != codFilial);

            query = query.Where(x => placas.Contains(x.PlacaVeiculo));
            return query.Select(x => x.PlacaVeiculo).ToList();
        }

        #endregion Métodos Públicos
    }
}
