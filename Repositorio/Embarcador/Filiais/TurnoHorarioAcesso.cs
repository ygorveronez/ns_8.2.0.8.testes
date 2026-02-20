using System.Collections.Generic;
using System.Linq;

namespace Repositorio.Embarcador.Filiais
{
    public class TurnoHorarioAcesso : RepositorioBase<Dominio.Entidades.Embarcador.Filiais.TurnoHorarioAcesso>
    {

        #region Contrutores

        public TurnoHorarioAcesso(UnitOfWork unitOfWork) : base(unitOfWork) { }

        #endregion

        #region Métodos Públicos

        public Dominio.Entidades.Embarcador.Filiais.TurnoHorarioAcesso BuscarPorCodigo(int codigo)
        {
            var consultaTurnoHorarioAcesso = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Filiais.TurnoHorarioAcesso>()
                .Where(obj => obj.Codigo == codigo);

            return consultaTurnoHorarioAcesso.FirstOrDefault();
        }

        public List<Dominio.Entidades.Embarcador.Filiais.TurnoHorarioAcesso> BuscarPorTurno(int codigoTurno)
        {
            var consultaTurnoHorarioAcesso = SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Filiais.TurnoHorarioAcesso>()
                .Where(obj => obj.Turno.Codigo == codigoTurno);

            return consultaTurnoHorarioAcesso.ToList();
        }

        #endregion
    }
}
