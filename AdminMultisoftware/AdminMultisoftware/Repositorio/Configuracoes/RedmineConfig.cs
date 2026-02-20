using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AdminMultisoftware.Repositorio.Configuracoes
{
    public class RedmineConfig : RepositorioBase<AdminMultisoftware.Dominio.Entidades.Configuracoes.RedmineConfig>
    {
        public RedmineConfig(UnitOfWork unitOfWork) : base(unitOfWork) { }

        public AdminMultisoftware.Dominio.Entidades.Configuracoes.RedmineConfig BuscarConfigPadrao()
        {
            var query = this.SessionNHiBernate.Query<AdminMultisoftware.Dominio.Entidades.Configuracoes.RedmineConfig>();
            var result = from obj in query select obj;
            return result.FirstOrDefault();
        }
    }
}
