using System.Collections.Generic;
using System.Linq;
using System.Linq.Dynamic.Core;

namespace Repositorio.Embarcador.Chamados
{
    public class ChamadoTMSAnalise : RepositorioBase<Dominio.Entidades.Embarcador.Chamados.ChamadoTMSAnalise>
    {
        public ChamadoTMSAnalise(UnitOfWork unitOfWork) : base(unitOfWork) { }

        #region Métodos Públicos

        public List<Dominio.Entidades.Embarcador.Chamados.ChamadoTMSAnalise> BuscarPorChamado(int chamado)
        {
            var result = _Consultar(chamado);
            result = result.OrderByDescending(obj => obj.DataCriacao);
            return result.ToList();
        }

        public List<Dominio.Entidades.Embarcador.Chamados.ChamadoTMSAnalise> Consultar(int chamado, string propOrdena, string dirOrdena, int inicioRegistros, int maximoRegistros)
        {
            var result = _Consultar(chamado);

            if (!string.IsNullOrWhiteSpace(propOrdena))
                result = result.OrderBy(propOrdena + (dirOrdena == "asc" ? " ascending" : " descending"));

            if (inicioRegistros > 0)
                result = result.Skip(inicioRegistros);

            if (maximoRegistros > 0)
                result = result.Take(maximoRegistros);

            return result.ToList();
        }

        public int ContarConsulta(int chamado)
        {
            var result = _Consultar(chamado);

            return result.Count();
        }

        #endregion

        #region Métodos Privados

        private IQueryable<Dominio.Entidades.Embarcador.Chamados.ChamadoTMSAnalise> _Consultar(int chamado)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Chamados.ChamadoTMSAnalise>();

            var result = from obj in query where obj.Chamado.Codigo == chamado select obj;

            return result;
        }

        #endregion
    }
}
