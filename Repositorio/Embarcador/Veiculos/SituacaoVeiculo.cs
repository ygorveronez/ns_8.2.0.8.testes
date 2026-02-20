using NHibernate.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Dynamic.Core;
using System.Threading;
using System.Threading.Tasks;

namespace Repositorio.Embarcador.Veiculos
{
    public class SituacaoVeiculo : RepositorioBase<Dominio.Entidades.Embarcador.Veiculos.SituacaoVeiculo>
    {
        public SituacaoVeiculo(UnitOfWork unitOfWork) : base(unitOfWork) { }
        public SituacaoVeiculo(UnitOfWork unitOfWork, CancellationToken cancellationToken) : base(unitOfWork, cancellationToken) { }

        public Dominio.Entidades.Embarcador.Veiculos.SituacaoVeiculo BuscarPorCodigo(int codigo)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Veiculos.SituacaoVeiculo>();

            var result = from obj in query where obj.Codigo == codigo select obj;

            return result.FirstOrDefault();
        }

        public Dominio.Entidades.Embarcador.Veiculos.SituacaoVeiculo BuscarUltimoPorVeiculo(int codigoVeiculo, Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoVeiculo situacaoVeiculo)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Veiculos.SituacaoVeiculo>();

            var result = from obj in query where obj.Veiculo.Codigo == codigoVeiculo && obj.Situacao == situacaoVeiculo select obj;

            return result.OrderBy("Codigo descending").FirstOrDefault();
        }

        public Dominio.Entidades.Embarcador.Veiculos.SituacaoVeiculo BuscarUltimoPorVeiculo(int codigoVeiculo)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Veiculos.SituacaoVeiculo>();

            var result = from obj in query where obj.Veiculo.Codigo == codigoVeiculo select obj;

            return result.OrderBy("Codigo descending").FirstOrDefault();
        }

        public List<Dominio.Entidades.Embarcador.Veiculos.SituacaoVeiculo> BuscarHistoricoPorVeículo(int codigoVeiculo)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Veiculos.SituacaoVeiculo>();
            var result = from obj in query where obj.Veiculo.Codigo == codigoVeiculo select obj;
            return result.OrderByDescending(situacao => situacao.Codigo).ToList();
        }

        public Task<List<Dominio.Entidades.Embarcador.Veiculos.SituacaoVeiculo>> BuscarHistoricoPorVeículoAsync(int codigoVeiculo)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Veiculos.SituacaoVeiculo>();
            var result = from obj in query where obj.Veiculo.Codigo == codigoVeiculo select obj;
            return result.OrderByDescending(situacao => situacao.Codigo).ToListAsync();
        }

        public List<Dominio.Entidades.Embarcador.Veiculos.SituacaoVeiculo> BuscarVeiculosComSituacaoEmManutencaoAberta()
        {
            DateTime dataAtual = DateTime.Now;
            var querySituacaoVeiculo = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Veiculos.SituacaoVeiculo>();

            querySituacaoVeiculo = querySituacaoVeiculo.Where(situacaoVeiculo => situacaoVeiculo.Situacao == Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoVeiculo.EmManutencao &&
                                                                                 !situacaoVeiculo.DataHoraSaidaManutencao.HasValue &&
                                                                                  situacaoVeiculo.DataHoraPrevisaoSaidaManutencao.HasValue &&
                                                                                  situacaoVeiculo.DataHoraPrevisaoSaidaManutencao >= dataAtual);
            return querySituacaoVeiculo
                .Fetch(situacaoVeiculo => situacaoVeiculo.Veiculo)
                .OrderBy(situacaoVeiculo => situacaoVeiculo.Codigo)
                .ToList();
        }

        public IList<int> BuscarVeiculosComSituacaoDivergente()
        {
            string sql = @"WITH UltimoHistorico AS (
                              SELECT VeiculoHistorico.*,
                                     ROW_NUMBER() OVER (PARTITION BY VeiculoHistorico.VEI_CODIGO ORDER BY VeiculoHistorico.VHI_CODIGO DESC) AS rn
                              FROM T_VEICULO_HISTORICO VeiculoHistorico WITH (NOLOCK)
                            )
                            SELECT Veiculo.VEI_CODIGO
                            FROM T_VEICULO Veiculo WITH (NOLOCK)
                            JOIN UltimoHistorico UltimoHistorico ON UltimoHistorico.VEI_CODIGO = Veiculo.VEI_CODIGO AND UltimoHistorico.rn = 1
                            WHERE Veiculo.VEI_ATIVO <> UltimoHistorico.VHI_SITUACAO;";
            NHibernate.ISQLQuery consulta = this.SessionNHiBernate.CreateSQLQuery(sql);
            return consulta.List<int>();
        }
    }
}
