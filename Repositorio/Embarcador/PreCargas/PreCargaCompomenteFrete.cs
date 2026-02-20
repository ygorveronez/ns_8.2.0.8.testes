using System.Collections.Generic;
using System.Linq;
using System.Linq.Dynamic.Core;


namespace Repositorio.Embarcador.PreCargas
{
    public class PreCargaCompomenteFrete : RepositorioBase<Dominio.Entidades.Embarcador.PreCargas.PreCargaCompomenteFrete>
    {
        public PreCargaCompomenteFrete(UnitOfWork unitOfWork) : base(unitOfWork) { }

        public Dominio.Entidades.Embarcador.PreCargas.PreCargaCompomenteFrete BuscarPorCodigo(int codigo)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.PreCargas.PreCargaCompomenteFrete>();
            var result = from obj in query where obj.Codigo == codigo select obj;
            return result.FirstOrDefault();
        }

        public List<Dominio.Entidades.Embarcador.PreCargas.PreCargaCompomenteFrete> BuscarPorPreCarga(int codigoCarga)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.PreCargas.PreCargaCompomenteFrete>();
            var result = from obj in query where obj.PreCarga.Codigo == codigoCarga  && obj.TipoComponenteFrete != Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoComponenteFrete.ICMS && obj.TipoComponenteFrete != Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoComponenteFrete.ISS select obj;
            return result.ToList();
        }
    }
}
