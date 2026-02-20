using System.Linq;
using NHibernate.Linq;
using System.Linq.Dynamic.Core;
using System.Collections.Generic;

namespace Repositorio.Embarcador.Logistica
{
    public sealed class MonitoramentoEventoCausa : RepositorioBase<Dominio.Entidades.Embarcador.Logistica.MonitoramentoEventoCausa>
    {
        #region Construtores

        public MonitoramentoEventoCausa(UnitOfWork unitOfWork) : base(unitOfWork) { }

        #endregion

        #region Métodos Privados
        #endregion

        #region Métodos Públicos
        public List<Dominio.Entidades.Embarcador.Logistica.MonitoramentoEventoCausa> BuscarPorTipoAlerta(Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoAlerta tipoAlerta)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Logistica.MonitoramentoEventoCausa>();

            var result = from obj in query
                         where obj.TipoAlerta == tipoAlerta
                         select obj;

            return result.ToList();
        }
        #endregion
    }
}
