using System.Collections.Generic;
using System.Linq;
using NHibernate.Linq;
using System.Linq.Dynamic.Core;

namespace AdminMultisoftware.Repositorio.Mobile
{
    public class UsuarioAvaliacao : RepositorioBase<Dominio.Entidades.Mobile.UsuarioAvaliacao>
    {
        public UsuarioAvaliacao(UnitOfWork unitOfWork) : base(unitOfWork) { }
    }
}

