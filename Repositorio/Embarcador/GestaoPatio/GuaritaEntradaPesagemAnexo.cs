using System.Collections.Generic;
using System.Linq;
using System.Linq.Dynamic.Core;

namespace Repositorio.Embarcador.GestaoPatio
{
    public class GuaritaEntradaPesagemAnexo : RepositorioBase<Dominio.Entidades.Embarcador.GestaoPatio.GuaritaEntradaPesagemAnexo>
    {
        public GuaritaEntradaPesagemAnexo(UnitOfWork unitOfWork) : base(unitOfWork) { }

        public List<Dominio.Entidades.Embarcador.GestaoPatio.GuaritaEntradaPesagemAnexo> BuscarPorGuarita(int codigo)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.GestaoPatio.GuaritaEntradaPesagemAnexo>()
                .Where(o => o.EntidadeAnexo.Codigo == codigo);

            return query.ToList();
        }
    }
}
