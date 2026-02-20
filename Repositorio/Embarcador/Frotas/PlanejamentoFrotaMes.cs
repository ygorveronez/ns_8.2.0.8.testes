using Dominio.ObjetosDeValor.Embarcador.Enumeradores;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Repositorio.Embarcador.Frotas
{
    public class PlanejamentoFrotaMes: RepositorioBase<Dominio.Entidades.Embarcador.Frotas.PlanejamentoFrotaMes>
    {
        public PlanejamentoFrotaMes(Repositorio.UnitOfWork unitOfWork) : base(unitOfWork) { }

        public Dominio.Entidades.Embarcador.Frotas.PlanejamentoFrotaMes BuscarSugestaoFrotaGerada(int codigoFilial, DateTime data)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Frotas.PlanejamentoFrotaMes>();
            query = query.Where(x => x.Filial.Codigo == codigoFilial);
            query = query.Where(x => x.Data == data.Date);
            return query.FirstOrDefault();
        }

        public List<Dominio.ObjetosDeValor.Embarcador.Frotas.CargaParaPlanejamentoDeFrota> ObterCarregamentosDoMes(DateTime dataInicial, DateTime dataFinal)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.Carga>();
            query = query.Where(x => x.SituacaoCarga == SituacaoCarga.EmTransporte || x.SituacaoCarga == SituacaoCarga.Encerrada);
            query = query.Where(x => x.DataCriacaoCarga >= dataInicial && x.DataCriacaoCarga < dataFinal.Date.AddDays(1));
            query = query.Where(x => x.Veiculo != null);
            query = query.Where(x => x.Veiculo.ModeloVeicularCarga != null);
            query = query.Where(x => x.Filial != null);
            query = query.Where(x => x.TipoOperacao != null);
            query = query.Where(x => x.Empresa != null);
            query = query.OrderByDescending(x => x.DataCriacaoCarga);
            return query.Select(x => new Dominio.ObjetosDeValor.Embarcador.Frotas.CargaParaPlanejamentoDeFrota
            {
                CargaCodigo = x.Codigo,
                VeiculoCodigo = x.Veiculo.Codigo,
                VeiculoPlaca = x.Veiculo.Placa,
                ModeloVeicularCodigo = x.Veiculo.ModeloVeicularCarga.Codigo,
                FilialCodigo = x.Filial.Codigo,
                TipoOperacaoCodigo = x.TipoOperacao.Codigo,
                TransportadorCodigo = x.Empresa.Codigo,
                EmailTransportador = x.Empresa.EmailAdministrativo
            }).ToList();
        }

        public bool VerificarSugestaoFrotaGerada(int codigoFilial, DateTime data)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Frotas.PlanejamentoFrotaMes>();
            query = query.Where(x => x.Filial.Codigo == codigoFilial);
            query = query.Where(x => x.Data == data.Date);
            return query.Any();
        }

    }
}
