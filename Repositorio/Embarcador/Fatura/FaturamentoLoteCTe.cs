using System.Collections.Generic;
using System.Linq;
using System.Linq.Dynamic.Core;

namespace Repositorio.Embarcador.Fatura
{
    public class FaturamentoLoteCTe : RepositorioBase<Dominio.Entidades.Embarcador.Fatura.FaturamentoLoteCTe>
    {
        public FaturamentoLoteCTe(UnitOfWork unitOfWork) : base(unitOfWork) { }

        public Dominio.Entidades.Embarcador.Fatura.FaturamentoLoteCTe BuscarPorCodigo(long codigo)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Fatura.FaturamentoLoteCTe>();
            var result = from obj in query where obj.Codigo == codigo select obj;
            return result.FirstOrDefault();
        }

        public List<int> BuscarCodigosCTes(long codigo)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Fatura.FaturamentoLoteCTe>();
            var result = from obj in query where obj.FaturamentoLote.Codigo == codigo select obj;
            return result.Select(c => c.ConhecimentoDeTransporteEletronico.Codigo)?.ToList() ?? null;
        }
    }
}
