using System.Collections.Generic;
using System.Linq;
using NHibernate.Linq;
using System.Linq.Dynamic.Core;


namespace AdminMultisoftware.Repositorio.Mobile
{
    public class ConfiguracaoMobile : RepositorioBase<Dominio.Entidades.Mobile.ConfiguracaoMobile>
    {
        public ConfiguracaoMobile(UnitOfWork unitOfWork) : base(unitOfWork) { }
        
        public Dominio.Entidades.Mobile.ConfiguracaoMobile BuscarConfiguracaoPadrao()
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Mobile.ConfiguracaoMobile>();
            return query.FirstOrDefault();
        }
    }
}
