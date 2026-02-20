using System.Collections.Generic;
using System.Linq;

namespace Repositorio.Embarcador.Logistica
{
    public class DocumentoTransporte : RepositorioBase<Dominio.Entidades.Embarcador.Logistica.DocumentoTransporte>
    {
        #region Constructores
        public DocumentoTransporte(Repositorio.UnitOfWork unitOfWork) : base(unitOfWork) { }
        #endregion

        #region Metódos Publicos

        public List<Dominio.Entidades.Embarcador.Logistica.DocumentoTransporte> BuscarPorCodigoAgendamento(int codigoAgendamento)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Logistica.DocumentoTransporte>();

            var resultado = query.Where(dt => dt.Agendamento.Codigo == codigoAgendamento);

            return resultado.ToList();

        }

        public Dominio.Entidades.Embarcador.Logistica.DocumentoTransporte BuscarPorCodigo(int codigo)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Logistica.DocumentoTransporte>();
            var resultado = query.Where(dt => dt.Codigo == codigo);
            return resultado.FirstOrDefault();

        }

        #endregion
    }
}
