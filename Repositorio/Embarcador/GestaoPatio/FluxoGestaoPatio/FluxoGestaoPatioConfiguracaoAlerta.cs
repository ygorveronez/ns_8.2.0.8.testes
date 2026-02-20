using System.Collections.Generic;
using System.Linq;

namespace Repositorio.Embarcador.GestaoPatio
{
    public class FluxoGestaoPatioConfiguracaoAlerta : RepositorioBase<Dominio.Entidades.Embarcador.GestaoPatio.FluxoGestaoPatioConfiguracaoAlerta>
    {
        public FluxoGestaoPatioConfiguracaoAlerta(UnitOfWork unitOfWork) : base(unitOfWork) { }

        #region Métodos Públicos

        public Dominio.Entidades.Embarcador.GestaoPatio.FluxoGestaoPatioConfiguracaoAlerta BuscarPorUsuarioFilial(int codigoUsuario, int codigoFilial)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.GestaoPatio.FluxoGestaoPatioConfiguracaoAlerta>();
            var result = from obj in query where obj.Usuario.Codigo == codigoUsuario && obj.Filial.Codigo == codigoFilial select obj;
            return result.FirstOrDefault();
        }

        public List<Dominio.Entidades.Embarcador.GestaoPatio.FluxoGestaoPatioConfiguracaoAlerta> BuscarPorUsuarioFiliais(int codigoUsuario, List<int> codigosFilial)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.GestaoPatio.FluxoGestaoPatioConfiguracaoAlerta>();
            var result = from obj in query where obj.Usuario.Codigo == codigoUsuario && codigosFilial.Contains(obj.Filial.Codigo) select obj;
            return result.ToList();
        }

        #endregion
    }
}
