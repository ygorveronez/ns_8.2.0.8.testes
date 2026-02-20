using System.Linq;

namespace Repositorio.Embarcador.Contabeis
{
    public class CalculoPisCofins : RepositorioBase<Dominio.Entidades.Embarcador.Contabeis.CalculoPisCofins>
    {
        public CalculoPisCofins(UnitOfWork unitOfWork) : base(unitOfWork) { }

        #region Métodos Públicos

        public Dominio.Entidades.Embarcador.Contabeis.CalculoPisCofins BuscarConfiguracaoCalculoPisCofins()
        {
            var query = SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Contabeis.CalculoPisCofins>();
            return query.FirstOrDefault();
        }

        #endregion
    }
}
