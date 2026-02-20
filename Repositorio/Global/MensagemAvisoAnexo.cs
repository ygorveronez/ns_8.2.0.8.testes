using System.Linq;
using System.Linq.Dynamic.Core;

namespace Repositorio
{
    public class MensagemAvisoAnexo : RepositorioBase<Dominio.Entidades.MensagemAvisoAnexo>
    {
        public MensagemAvisoAnexo(UnitOfWork unitOfWork) : base(unitOfWork) { }

       
        public Dominio.Entidades.MensagemAvisoAnexo BuscarPorCodigoETipoServico(int codigo, AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware tipoServico)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.MensagemAvisoAnexo>();

            query = query.Where(obj => obj.Codigo == codigo && obj.EntidadeAnexo.TipoServicoMultisoftware == tipoServico);

            return query.FirstOrDefault();
        }
    }
}
