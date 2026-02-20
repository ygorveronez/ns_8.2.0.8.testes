using System.Collections.Generic;
using System.Linq;
using System.Linq.Dynamic.Core;

namespace Repositorio.Embarcador.GestaoPatio
{
    public class GuaritaEntradaPesagemFinalAnexo : RepositorioBase<Dominio.Entidades.Embarcador.GestaoPatio.GuaritaEntradaPesagemFinalAnexo>
    {
        public GuaritaEntradaPesagemFinalAnexo(UnitOfWork unitOfWork) : base(unitOfWork) { }

        public List<Dominio.Entidades.Embarcador.GestaoPatio.GuaritaEntradaPesagemFinalAnexo> BuscarPorGuarita(int codigo)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.GestaoPatio.GuaritaEntradaPesagemFinalAnexo>()
                .Where(o => o.EntidadeAnexo.Codigo == codigo);

            return query.ToList();
        }
    }
}
