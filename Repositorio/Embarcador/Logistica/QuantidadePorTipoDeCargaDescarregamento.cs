using System.Collections.Generic;
using System.Linq;
using System.Linq.Dynamic.Core;

namespace Repositorio.Embarcador.Logistica
{
    public class QuantidadePorTipoDeCargaDescarregamento : RepositorioBase<Dominio.Entidades.Embarcador.Logistica.QuantidadePorTipoDeCargaDescarregamento>
    {
        public QuantidadePorTipoDeCargaDescarregamento(UnitOfWork unitOfWork) : base(unitOfWork) { }

        public Dominio.Entidades.Embarcador.Logistica.QuantidadePorTipoDeCargaDescarregamento BuscarPorCodigo(int codigo)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Logistica.QuantidadePorTipoDeCargaDescarregamento>();

            var result = from obj in query where obj.Codigo == codigo select obj;

            return result.FirstOrDefault();
        }

        public List<Dominio.Entidades.Embarcador.Logistica.QuantidadePorTipoDeCargaDescarregamento> BuscarPorCentroDescarregamentoCodigosDesconsiderar(int codigoCentroDescarregamento, List<int> codigos)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Logistica.QuantidadePorTipoDeCargaDescarregamento>();

            query = query.Where(o => !codigos.Contains(o.Codigo) && o.CentroDescarregamento.Codigo == codigoCentroDescarregamento);
            
            return query
                .ToList();
        }
        
        public List<Dominio.Entidades.Embarcador.Logistica.QuantidadePorTipoDeCargaDescarregamento> BuscarPorCentroDescarregamento(int codigoCentroDescarregamento)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Logistica.QuantidadePorTipoDeCargaDescarregamento>();

            query = query.Where(o => o.CentroDescarregamento.Codigo == codigoCentroDescarregamento);
            
            return query
                .ToList();
        }

        public List<Dominio.Entidades.Embarcador.Logistica.QuantidadePorTipoDeCargaDescarregamento> BuscarPorCodigos(List<int> codigosQuantidadeTiposCargaNaoDeletadas)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Logistica.QuantidadePorTipoDeCargaDescarregamento>()
                .Where(obj => codigosQuantidadeTiposCargaNaoDeletadas.Contains(obj.Codigo));
            
            return query.ToList();
        }

        public List<Dominio.Entidades.Embarcador.Logistica.QuantidadePorTipoDeCargaDescarregamento> BuscarPorExcecao(int codigo)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Logistica.QuantidadePorTipoDeCargaDescarregamento>()
                .Where(obj => obj.ExcecaoCapacidadeDescarregamento.Codigo == codigo);
            
            return query.ToList();
        }
    }
}
