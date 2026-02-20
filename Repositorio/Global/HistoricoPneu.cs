using System.Collections.Generic;
using System.Linq;

namespace Repositorio
{
    public class HistoricoPneu : RepositorioBase<Dominio.Entidades.HistoricoPneu>, Dominio.Interfaces.Repositorios.HistoricoPneu
    {
        public HistoricoPneu(UnitOfWork unitOfWork) : base(unitOfWork) { }

        public Dominio.Entidades.HistoricoPneu BuscarPorCodigo(int codigo, int codigoEmpresa)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.HistoricoPneu>();
            var result = from obj in query where obj.Codigo == codigo && obj.Pneu.Empresa.Codigo == codigoEmpresa select obj;
            return result.FirstOrDefault();
        }

        public List<Dominio.Entidades.HistoricoPneu> Consultar(int codigoEmpresa, string seriePneu, string placaVeiculo, int inicioRegistros, int maximoRegistros)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.HistoricoPneu>().OrderByDescending(o => o.Data);
            var result = from obj in query where obj.Pneu.Empresa.Codigo == codigoEmpresa select obj;
            if (!string.IsNullOrWhiteSpace(placaVeiculo))
                result = result.Where(o => o.Veiculo.Placa.Contains(placaVeiculo));
            if (!string.IsNullOrWhiteSpace(seriePneu))
                result = result.Where(o => o.Pneu.Serie == seriePneu);
            result = result.Skip(inicioRegistros).Take(maximoRegistros);
            return result.ToList();
        }

        public int ContarConsulta(int codigoEmpresa, string seriePneu, string placaVeiculo)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.HistoricoPneu>();
            var result = from obj in query where obj.Pneu.Empresa.Codigo == codigoEmpresa select obj;
            if (!string.IsNullOrWhiteSpace(placaVeiculo))
                result = result.Where(o => o.Veiculo.Placa.Contains(placaVeiculo));
            if (!string.IsNullOrWhiteSpace(seriePneu))
                result = result.Where(o => o.Pneu.Serie == seriePneu);
            return result.Count();
        }

        public List<Dominio.ObjetosDeValor.Relatorios.RelatorioHistoricosPneus> Relatorio(int codigoEmpresa, int codigoVeiculo)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.HistoricoPneu>();

            var idsHistoricos = (from obj in query where obj.Pneu.Status.Equals("A") && obj.Pneu.Empresa.Codigo == codigoEmpresa && obj.Pneu.Veiculo.Codigo == codigoVeiculo && obj.Veiculo.Codigo == codigoVeiculo group obj by obj.Pneu.Codigo into grupoPneus select grupoPneus.Max(o => o.Codigo)).ToList();

            var result = from obj in query
                         where idsHistoricos.Contains(obj.Codigo) && obj.Tipo == "E"
                         orderby obj.Codigo descending
                         group obj by new { obj.Codigo, CodigoPneu = obj.Pneu.Codigo, obj.Eixo.Descricao, obj.Observacao, obj.Tipo, obj.Veiculo.Placa, TipoVeiculo = obj.Veiculo.TipoDoVeiculo.Descricao, obj.Pneu.Serie } into grupoPneus
                         select new Dominio.ObjetosDeValor.Relatorios.RelatorioHistoricosPneus
                         {
                             Data = grupoPneus.Max(o => o.Data),
                             Eixo = grupoPneus.First().Eixo.Descricao,
                             KMAtualVeiculo = grupoPneus.Max(o => o.Veiculo.KilometragemAtual),
                             KMTroca = grupoPneus.Max(o => o.Kilometragem),
                             Observacao = grupoPneus.First().Observacao,
                             SeriePneu = grupoPneus.FirstOrDefault().Pneu.Serie, //grupoPneus.Max(o => o.Pneu.Serie)
                             TipoHistorico = grupoPneus.First().Tipo,
                             Veiculo = grupoPneus.First().Veiculo.Placa,
                             TipoVeiculo = grupoPneus.First().Veiculo.TipoDoVeiculo.Descricao,
                             Ordem = grupoPneus.Max(o => o.Eixo.OrdemEixo)
                         };

            return result.ToList();
        }

        public List<Dominio.ObjetosDeValor.Relatorios.RelatorioHistoricosPneus> RelatorioHistoricos(int codigoEmpresa, int codigoVeiculo, int codigoPneu)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.HistoricoPneu>();

            var result = from obj in query where obj.Pneu.Empresa.Codigo == codigoEmpresa select obj;

            if (codigoVeiculo > 0)
                result = result.Where(o => o.Veiculo.Codigo == codigoVeiculo);

            if (codigoPneu > 0)
                result = result.Where(o => o.Pneu.Codigo == codigoPneu);

            return result.Select(obj => new Dominio.ObjetosDeValor.Relatorios.RelatorioHistoricosPneus()
                                        {
                                            CodigoPneu = obj.Pneu.Codigo,
                                            CodigoVeiculo = obj.Veiculo.Codigo,
                                            Data = obj.Data,
                                            Eixo = obj.Eixo.Descricao,
                                            KMAtualVeiculo = obj.Veiculo.KilometragemAtual,
                                            KMTroca = obj.Kilometragem,
                                            Observacao = obj.Observacao,
                                            Ordem = obj.Eixo.OrdemEixo,
                                            SeriePneu = obj.Pneu.Serie,
                                            TipoHistorico = obj.Tipo,
                                            TipoVeiculo = obj.Veiculo.TipoDoVeiculo.Descricao,
                                            Veiculo = obj.Veiculo.Placa
                                        }).ToList();
        }
    }
}
