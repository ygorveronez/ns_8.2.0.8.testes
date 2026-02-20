using System.Collections.Generic;
using System.Linq;
using System.Linq.Dynamic.Core;

namespace Repositorio.Embarcador.Contatos
{
    public class ContatoClienteDocumento : RepositorioBase<Dominio.Entidades.Embarcador.Contatos.ContatoClienteDocumento>
    {
        public ContatoClienteDocumento(UnitOfWork unitOfWork) : base(unitOfWork) { }

        public List<Dominio.Entidades.Embarcador.Contatos.ContatoClienteDocumento> BuscarPorContatoCliente(int codigoContatoCliente)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Contatos.ContatoClienteDocumento>();

            query = query.Where(o => o.ContatoCliente.Codigo == codigoContatoCliente);

            return query.ToList();
        }
    }
}
