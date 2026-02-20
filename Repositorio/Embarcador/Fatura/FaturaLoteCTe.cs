using System.Collections.Generic;
using System.Linq;
using System.Linq.Dynamic.Core;

namespace Repositorio.Embarcador.Fatura
{
    public class FaturaLoteCTe : RepositorioBase<Dominio.Entidades.Embarcador.Fatura.FaturaLoteCTe>
    {
        public FaturaLoteCTe(UnitOfWork unitOfWork) : base(unitOfWork) { }

        public Dominio.Entidades.Embarcador.Fatura.FaturaLoteCTe BuscarPorCodigo(int codigo)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Fatura.FaturaLoteCTe>();
            var result = from obj in query where obj.Codigo == codigo select obj;
            return result.FirstOrDefault();
        }

        public List<string> BuscarChavesCTePorFatura(int codigoFatura)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Fatura.FaturaLoteCTe>();

            var result = from obj in query where obj.Fatura.Codigo == codigoFatura select obj;

            return result.Select(o => o.CTe.Chave).ToList();
        }
    }
}
