using System;
using System.Linq;

namespace Repositorio.Embarcador.Veiculos
{
    public class VeiculoProcessamentoReceitaDespesa : RepositorioBase<Dominio.Entidades.Embarcador.Veiculos.VeiculoProcessamentoReceitaDespesa>
    {
        public VeiculoProcessamentoReceitaDespesa(UnitOfWork unitOfWork) : base(unitOfWork) { }

        public DateTime? BuscarUltimaDataRealizada()
        {
            IQueryable<Dominio.Entidades.Embarcador.Veiculos.VeiculoProcessamentoReceitaDespesa> query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Veiculos.VeiculoProcessamentoReceitaDespesa>();

            return query.Max(o => (DateTime?)o.Data);
        }
    }
}
