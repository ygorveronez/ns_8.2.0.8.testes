using System;
using System.Collections.Generic;
using System.Linq;

namespace Repositorio.Embarcador.PagamentoMotorista
{
    public class DespesaViagem : RepositorioBase<Dominio.Entidades.Embarcador.PagamentoMotorista.DespesaViagem>
    {
        public DespesaViagem(UnitOfWork unitOfWork) : base(unitOfWork) { }

        public List<(string Descricao, int Quantidade, decimal ValorUnitario, decimal ValorTotal)> BuscarPorPagamentoMotorista(int codigoPagamento)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.PagamentoMotorista.DespesaViagem>();
            query = query.Where(o => o.PagamentoMotoristaTMS.Codigo == codigoPagamento);

            var queryDois = query.GroupBy(o => new { o.TabelaDiariaPeriodo, o.TabelaDiariaPeriodo.Descricao, o.TabelaDiariaPeriodo.Valor }).Select(o => new ValueTuple<string, int, decimal, decimal>(
                o.Key.Descricao,
                o.Count(),
                o.Key.Valor,
                o.Key.Valor * o.Count()
            ));

            return queryDois
                .ToList();
        }
        
        public List<Dominio.Entidades.Embarcador.PagamentoMotorista.DespesaViagem> BuscarEntidadesPorPagamentoMotorista(int codigoPagamento)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.PagamentoMotorista.DespesaViagem>();
            query = query.Where(o => o.PagamentoMotoristaTMS.Codigo == codigoPagamento);
            
            return query
                .ToList();
        }
    }
}
