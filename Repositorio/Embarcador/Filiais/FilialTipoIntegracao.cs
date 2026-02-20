using NHibernate.Linq;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Repositorio.Embarcador.Filiais;

public class FilialTipoIntegracao(UnitOfWork unitOfWork) : RepositorioBase<Dominio.Entidades.Embarcador.Filiais.FilialTipoIntegracao>(unitOfWork)
{
    #region Métodos Públicos

    public async Task<List<Dominio.Entidades.Embarcador.Filiais.FilialTipoIntegracao>> BuscarPorFilialAsync(int codigoFilial, CancellationToken cancellationToken)
    {
        return await SessionNHiBernate
            .Query<Dominio.Entidades.Embarcador.Filiais.FilialTipoIntegracao>()
            .Where(o => o.Filial.Codigo == codigoFilial)
            .ToListAsync(cancellationToken);
    }

    public async Task<List<Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoIntegracao>> BuscarTiposIntegracaoPorFilialAsync(int codigoFilial, CancellationToken cancellationToken)
    {
        return await SessionNHiBernate
            .Query<Dominio.Entidades.Embarcador.Filiais.FilialTipoIntegracao>()
            .Where(o => o.Filial.Codigo == codigoFilial)
            .Select(o => o.TipoIntegracao.Tipo)
            .ToListAsync(cancellationToken);
    }

    public List<Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoIntegracao> BuscarTiposIntegracaoPorFilial(int codigoFilial)
    {
        return SessionNHiBernate
            .Query<Dominio.Entidades.Embarcador.Filiais.FilialTipoIntegracao>()
            .Where(o => o.Filial.Codigo == codigoFilial)
            .Select(o => o.TipoIntegracao.Tipo)
            .ToList();
    }

    #endregion
}
