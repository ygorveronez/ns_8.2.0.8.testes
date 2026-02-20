using System.Linq;
using System.Linq.Dynamic.Core;

namespace Repositorio.Embarcador.Patrimonio
{
    public class BemTransferenciaItem : RepositorioBase<Dominio.Entidades.Embarcador.Patrimonio.BemTransferenciaItem>
    {
        public BemTransferenciaItem(UnitOfWork unitOfWork) : base(unitOfWork) { }

        public Dominio.Entidades.Embarcador.Patrimonio.BemTransferenciaItem BuscarPorBemETransferencia(int codigoBem, int codigoTransferencia)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Patrimonio.BemTransferenciaItem>();
            var result = from obj in query where obj.Bem.Codigo == codigoBem && obj.BemTransferencia.Codigo == codigoTransferencia select obj;
            return result.FirstOrDefault();
        }
    }
}
