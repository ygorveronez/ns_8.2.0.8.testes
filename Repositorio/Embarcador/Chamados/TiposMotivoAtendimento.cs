using System.Collections.Generic;
using System.Linq;

namespace Repositorio.Embarcador.Chamados
{
    public class TiposMotivoAtendimento : RepositorioBase<Dominio.Entidades.Embarcador.Chamados.TiposMotivoAtendimento>
    {
        public TiposMotivoAtendimento(UnitOfWork unitOfWork) : base(unitOfWork) { }

        public List<Dominio.Entidades.Embarcador.Chamados.TiposMotivoAtendimento> BuscarPorTipos(List<Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoMotivoAtendimento> tipoMotivoAtendimento)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Chamados.TiposMotivoAtendimento>();

            if (tipoMotivoAtendimento != null && tipoMotivoAtendimento.Count > 0)
                query = query.Where(o => tipoMotivoAtendimento.Contains(o.TipoMotivoAtendimento));

            return query.ToList();
        }

    }
}
