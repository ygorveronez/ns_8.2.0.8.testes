using NHibernate.Linq;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Dynamic.Core;
using System.Threading;
using System.Threading.Tasks;

namespace Repositorio.Embarcador.Email
{
    public class EmailCaixaEntrada : RepositorioBase<Dominio.Entidades.Embarcador.Email.EmailCaixaEntrada>
    {
        public EmailCaixaEntrada(UnitOfWork unitOfWork) : base(unitOfWork) { }

        public List<Dominio.Entidades.Embarcador.Email.EmailCaixaEntrada> BuscarPorTipoServico(AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware tipoServicoMultisoftware, int inicioRegistros, int maximoRegistros)
        {
            var query = SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Email.EmailCaixaEntrada>();
            var result = query.Where(obj => obj.TipoServico == tipoServicoMultisoftware);

            return result.OrderBy(o => o.Codigo).Skip(inicioRegistros).Take(maximoRegistros).ToList();
        }

        public Task<List<Dominio.Entidades.Embarcador.Email.EmailCaixaEntrada>> BuscarPorTipoServicoAsync(AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware tipoServicoMultisoftware, int inicioRegistros, int maximoRegistros, CancellationToken cancellationToken)
        {
            var query = SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Email.EmailCaixaEntrada>();
            var result = query.Where(obj => obj.TipoServico == tipoServicoMultisoftware);

            return result.OrderBy(o => o.Codigo).Skip(inicioRegistros).Take(maximoRegistros).ToListAsync(CancellationToken);
        }
    }
}
