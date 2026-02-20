using System;
using System.Collections.Generic;
using System.Linq;

namespace Repositorio.Embarcador.Frotas
{
    public class PlanejamentoFrotaDiaVeiculo : RepositorioBase<Dominio.Entidades.Embarcador.Frotas.PlanejamentoFrotaDiaVeiculo>
    {
        public PlanejamentoFrotaDiaVeiculo(UnitOfWork unitOfWork) : base(unitOfWork) { }

        public DateTime ObterDataDoUltimoCarregamento(int codigoVeiculo)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.Carga>();
            var data = query.Where(x => x.Veiculo.Codigo == codigoVeiculo)
                .OrderByDescending(x => x.DataCriacaoCarga)
                .Select(x => x.DataCriacaoCarga)
                .FirstOrDefault();
            return data; ;
        }

        public List<RelacaoVeiculoUltimaDataDeCarregamento> ObterDataDoUltimoCarregamento(List<int> codigosVeiculos)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.Carga>();
            var data = query.Where(x => codigosVeiculos.Contains(x.Veiculo.Codigo))
                .OrderByDescending(x => x.DataCriacaoCarga)
                .GroupBy(x => new { CodigoVeiculo = x.Veiculo.Codigo, DataCarregamento = x.DataCriacaoCarga })
                .Select(x => new RelacaoVeiculoUltimaDataDeCarregamento { CodigoVeiculo = x.Key.CodigoVeiculo, DataCarregamento = x.Key.DataCarregamento })
                .ToList();

            return data.DistinctBy(x => x.CodigoVeiculo).ToList();
        }

        public (List<Dominio.Entidades.Embarcador.Frotas.PlanejamentoFrotaDiaVeiculo>, int total) PesquisarListaDiaria(FiltroPesquisaListaDiaria filtros, int start, int limit)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Frotas.PlanejamentoFrotaDiaVeiculo>();

            query = query.Where(x => x.ModeloVeicular != null);
            query = query.Where(x => x.PlanejamentoFrotaDia != null);
            query = query.Where(x => x.Veiculo != null);

            if (filtros.DataInicial > DateTime.MinValue)
                query = query.Where(x => x.PlanejamentoFrotaDia.Data >= filtros.DataInicial.Date);

            if (filtros.DataFinal > DateTime.MinValue)
                query = query.Where(x => x.PlanejamentoFrotaDia.Data <= filtros.DataFinal.Date.Add(DateTime.MaxValue.TimeOfDay));

            if (!filtros.Filial.IsNullOrEmpty())
                query = query.Where(x => filtros.Filial.Contains(x.PlanejamentoFrotaDia.Filial.Codigo));

            if (!filtros.ModeloVeicular.IsNullOrEmpty())
                query = query.Where(x => filtros.ModeloVeicular.Contains(x.ModeloVeicular.Codigo));

            if (!filtros.Transportador.IsNullOrEmpty())
                query = query.Where(x => filtros.Transportador.Contains(x.Veiculo.Empresa.Codigo));

            if (filtros.SituacaoRota != EnumFiltroSituacoesRota.Todos)
            {
                if (filtros.SituacaoRota == EnumFiltroSituacoesRota.NaoRoteirizado)
                    query = query.Where(x => x.Roteirizado != true);

                if (filtros.SituacaoRota == EnumFiltroSituacoesRota.Roteirizado)
                    query = query.Where(x => x.Roteirizado == true);
            }

            if (filtros.Status != EnumFiltroStatusFrotaDiaria.Todos)
            {
                if (filtros.Status == EnumFiltroStatusFrotaDiaria.Disponivel)
                    query = query.Where(x => x.Indisponivel != true);

                if (filtros.Status == EnumFiltroStatusFrotaDiaria.Indisponivel)
                    query = query.Where(x => x.Indisponivel == true);
            }

            if (filtros.Rodizio)
                query = query.Where(x => x.Rodizio == true);

            /*
            var queryContagem = query.GroupBy(x => new
            {
                Modelo = x.ModeloVeicular.Codigo,
                Filial = x.PlanejamentoFrotaDia.Filial.Codigo,
                Data = x.PlanejamentoFrotaDia.Data
            }).Select(x => new { A =  x.Key.Filial });

            int total = queryContagem.Count();*/

            //return (query.Skip(start).Take(limit).ToList(), total);
            return (query.ToList(), 1);
        }

        public List<Dominio.Entidades.Embarcador.Frotas.PlanejamentoFrotaDiaVeiculo> PesquisarListaDiariaVeiculos(FiltroPesquisaListaDiariaVeiculos filtros)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Frotas.PlanejamentoFrotaDiaVeiculo>();

            query = query.Where(x => x.ModeloVeicular != null);
            query = query.Where(x => x.PlanejamentoFrotaDia != null);
            query = query.Where(x => x.Veiculo != null);

            if (filtros.Data > DateTime.MinValue)
            {
                var dataInicial = filtros.Data.Date;
                var dataFinal = filtros.Data.AddDays(1);
                query = query.Where(x => x.PlanejamentoFrotaDia.Data >= dataInicial && x.PlanejamentoFrotaDia.Data < dataFinal);
            }

            if (filtros.CodigoFilial > 0)
                query = query.Where(x => filtros.CodigoFilial == x.PlanejamentoFrotaDia.Filial.Codigo);

            if (filtros.CodigoModeloVeicular > 0)
                query = query.Where(x => filtros.CodigoModeloVeicular == x.ModeloVeicular.Codigo);

            if (filtros.CodigosTransportadores?.Count > 0)
                query = query.Where(x => filtros.CodigosTransportadores.Contains(x.Veiculo.Empresa.Codigo));

            if (filtros.Roteirizado != EnumFiltroSituacoesRota.Todos)
            {
                if (filtros.Roteirizado == EnumFiltroSituacoesRota.Roteirizado)
                    query = query.Where(x => x.Roteirizado == true);
                if (filtros.Roteirizado == EnumFiltroSituacoesRota.NaoRoteirizado)
                    query = query.Where(x => x.Roteirizado == false);
            }

            query = query.OrderByDescending(x => x.Veiculo.AnoModelo);

            return query.ToList();
        }

        public bool VerificarSePlacaJaExisteNaData(DateTime data, string placa)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Frotas.PlanejamentoFrotaDiaVeiculo>();
            query = query.Where(x => x.PlacaVeiculo == placa);
            query = query.Where(x => x.PlanejamentoFrotaDia.Data == data.Date);
            return query.Any();
        }

        public List<Dominio.Entidades.Embarcador.Frotas.PlanejamentoFrotaDiaVeiculo> BuscarPorData(int codigoFilial, DateTime dataInicial, DateTime dataFinal, string placa = "")
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Frotas.PlanejamentoFrotaDiaVeiculo>();

            query = query.Where(x => x.PlanejamentoFrotaDia.Filial.Codigo == codigoFilial);

            if (dataInicial > DateTime.MinValue)
                query = query.Where(x => x.PlanejamentoFrotaDia.Data >= dataInicial.Date);

            if (dataFinal > DateTime.MinValue)
                query = query.Where(x => x.PlanejamentoFrotaDia.Data < dataFinal.Date.AddDays(1));

            if (!string.IsNullOrEmpty(placa))
                query = query.Where(x => x.PlacaVeiculo == placa);

            return query.ToList();
        }

    }

    public class RelacaoVeiculoUltimaDataDeCarregamento
    {
        public int CodigoVeiculo { get; set; }
        public DateTime? DataCarregamento { get; set; }
    }

    public enum EnumFiltroStatusFrotaDiaria
    {
        Todos = -1,
        Disponivel = 1,
        Indisponivel = 2,
    }

    public enum EnumFiltroSituacoesRota
    {
        Todos = -1,
        Roteirizado = 1,
        NaoRoteirizado = 2,
    }

    public class FiltroPesquisaListaDiaria
    {
        public List<int> ModeloVeicular { get; set; }
        public List<int> Filial { get; set; }
        public List<int> Transportador { get; set; }
        public EnumFiltroSituacoesRota SituacaoRota { get; set; }
        public EnumFiltroStatusFrotaDiaria Status { get; set; }
        public DateTime DataInicial { get; set; }
        public DateTime DataFinal { get; set; }
        public bool Rodizio { get; set; }
    }

    public class FiltroPesquisaListaDiariaVeiculos
    {
        public List<int> CodigosTransportadores { get; set; }
        public int CodigoFilial { get; set; }
        public int CodigoModeloVeicular { get; set; }
        public DateTime Data { get; set; }
        public EnumFiltroSituacoesRota Roteirizado { get; set; }
    }
}
