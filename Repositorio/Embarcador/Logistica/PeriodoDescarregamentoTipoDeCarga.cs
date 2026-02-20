using System.Collections.Generic;
using System.Linq;
using System.Linq.Dynamic.Core;

namespace Repositorio.Embarcador.Logistica
{
    public class PeriodoDescarregamentoTipoDeCarga : RepositorioBase<Dominio.Entidades.Embarcador.Logistica.PeriodoDescarregamentoTipoDeCarga>
    {
        public PeriodoDescarregamentoTipoDeCarga(UnitOfWork unitOfWork) : base(unitOfWork) { }

        public List<Dominio.Entidades.Embarcador.Logistica.PeriodoDescarregamentoTipoDeCarga> BuscarPorPeriodoDescarregamento(int codigoPeriodoDescarregamento)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Logistica.PeriodoDescarregamentoTipoDeCarga>();

            var result = from obj in query where obj.PeriodoDescarregamento.Codigo == codigoPeriodoDescarregamento select obj;

            return result.ToList();
        }
        
        public List<Dominio.Entidades.Embarcador.Logistica.PeriodoDescarregamentoTipoDeCarga> BuscarPorPeriodosDescarregamento(List<int> codigos)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Logistica.PeriodoDescarregamentoTipoDeCarga>();

            var result = from obj in query where codigos.Contains(obj.PeriodoDescarregamento.Codigo) select obj;
            
            return result.ToList();
        }
        
        public List<Dominio.Entidades.Embarcador.Logistica.PeriodoDescarregamentoTipoDeCarga> BuscarPorTiposDeCargaCentroDescarregamento(List<int> codigosTipoDeCarga, int codigoCentroDescarregamento)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Logistica.PeriodoDescarregamentoTipoDeCarga>();
            query = query.Where(obj => codigosTipoDeCarga.Contains(obj.TipoDeCarga.Codigo) && obj.PeriodoDescarregamento.CentroDescarregamento.Codigo == codigoCentroDescarregamento);

            return query.ToList();
        }

        public List<Dominio.Entidades.Embarcador.Logistica.PeriodoDescarregamentoTipoDeCarga> BuscarPorExcecao(int codigoExcecaoCapacidadeDescarregamento)
        {
            var consultaPeriodoDescarregamentoTipoDeCarga = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Logistica.PeriodoDescarregamentoTipoDeCarga>()
                .Where(o => o.PeriodoDescarregamento.ExcecaoCapacidadeDescarregamento.Codigo == codigoExcecaoCapacidadeDescarregamento);

            return consultaPeriodoDescarregamentoTipoDeCarga.ToList();
        }
    }
}
