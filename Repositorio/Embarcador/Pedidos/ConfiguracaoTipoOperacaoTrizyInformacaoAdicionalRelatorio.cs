using NHibernate.Linq;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Dynamic.Core;
using System.Threading;
using System.Threading.Tasks;

namespace Repositorio.Embarcador.Pedidos
{
    public class ConfiguracaoTipoOperacaoTrizyInformacaoAdicionalRelatorio : RepositorioBase<Dominio.Entidades.Embarcador.Pedidos.ConfiguracaoTipoOperacaoTrizyInformacaoAdicionalRelatorio>
    {
        #region Construtores

        public ConfiguracaoTipoOperacaoTrizyInformacaoAdicionalRelatorio(UnitOfWork unitOfWork, CancellationToken cancellationToken) : base(unitOfWork, cancellationToken) { }
        public ConfiguracaoTipoOperacaoTrizyInformacaoAdicionalRelatorio(UnitOfWork unitOfWork) : base(unitOfWork) { }

        #endregion

        #region Métodos Públicos Async
        public Task<List<Dominio.Entidades.Embarcador.Pedidos.ConfiguracaoTipoOperacaoTrizyInformacaoAdicionalRelatorio>> BuscarPorConfiguracaoTipoOperacaoTrizyAsync(int codigoConfiguracaoTipoOperacaoTrizy)
        {
            IQueryable<Dominio.Entidades.Embarcador.Pedidos.ConfiguracaoTipoOperacaoTrizyInformacaoAdicionalRelatorio> query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Pedidos.ConfiguracaoTipoOperacaoTrizyInformacaoAdicionalRelatorio>()
               .Where(obj => obj.ConfiguracaoTipoOperacaoTrizy.Codigo == codigoConfiguracaoTipoOperacaoTrizy);

            return query
                .ToListAsync();
        }
        #endregion

        #region Metódos Públicos Sincronos
        public List<Dominio.Entidades.Embarcador.Pedidos.ConfiguracaoTipoOperacaoTrizyInformacaoAdicionalRelatorio> BuscarPorConfiguracaoTipoOperacaoTrizy(int codigoConfiguracaoTipoOperacaoTrizy)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Pedidos.ConfiguracaoTipoOperacaoTrizyInformacaoAdicionalRelatorio>();

            var result = from obj in query where obj.ConfiguracaoTipoOperacaoTrizy.Codigo == codigoConfiguracaoTipoOperacaoTrizy select obj;

            return result.ToList();
        }
        #endregion

        #region Métodos Privados
        #endregion
    }
}