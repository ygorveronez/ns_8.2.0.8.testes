using System.Collections.Generic;
using System.Linq;
using System.Linq.Dynamic.Core;

namespace Repositorio.Embarcador.GestaoPatio
{
    public class GuaritaAnexo : RepositorioBase<Dominio.Entidades.Embarcador.GestaoPatio.GuaritaAnexo>
    {
        #region Construtores

        public GuaritaAnexo(UnitOfWork unitOfWork) : base(unitOfWork) { }

        #endregion

        #region Métodos Públicos

        public List<Dominio.Entidades.Embarcador.GestaoPatio.GuaritaAnexo> BuscarPorGuarita(int codigoGuarita)
        {
            var consultaGuarita = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.GestaoPatio.GuaritaAnexo>()
                .Where(o => o.EntidadeAnexo.Codigo  == codigoGuarita);

            return consultaGuarita.ToList();
        }

        #endregion
    }
}
