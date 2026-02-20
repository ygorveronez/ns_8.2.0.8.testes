using System.Linq;
using System.Linq.Dynamic.Core;


namespace Repositorio.Embarcador.WMS
{
    public class TransferenciaProdutoEmbarcadorLote : RepositorioBase<Dominio.Entidades.Embarcador.WMS.TransferenciaProdutoEmbarcadorLote>
    {
        public TransferenciaProdutoEmbarcadorLote(UnitOfWork unitOfWork) : base(unitOfWork) { }

        public Dominio.Entidades.Embarcador.WMS.TransferenciaProdutoEmbarcadorLote BuscarPorCodigo(int codigo)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.WMS.TransferenciaProdutoEmbarcadorLote>();
            var result = from obj in query where obj.Codigo == codigo select obj;
            return result.FirstOrDefault();
        }
    }
}
