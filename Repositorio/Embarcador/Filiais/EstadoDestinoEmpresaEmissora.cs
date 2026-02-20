using NHibernate.Linq;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Dynamic.Core;
using System.Threading.Tasks;

namespace Repositorio.Embarcador.Filiais
{
    public class EstadoDestinoEmpresaEmissora : RepositorioBase<Dominio.Entidades.Embarcador.Filiais.EstadoDestinoEmpresaEmissora>
    {
        #region Construtores

        public EstadoDestinoEmpresaEmissora(UnitOfWork unitOfWork) : base(unitOfWork) { }

        #endregion

        #region Métodos Privados

        #endregion

        #region Métodos Públicos

        public Dominio.Entidades.Embarcador.Filiais.EstadoDestinoEmpresaEmissora BuscarPorCodigo(int codigo)
        {
            return this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Filiais.EstadoDestinoEmpresaEmissora>()
                .Where(estadoDestino => estadoDestino.Codigo == codigo)
                .FirstOrDefault();
        }

        public Dominio.Entidades.Embarcador.Filiais.EstadoDestinoEmpresaEmissora BuscarPorSigla(string sigla)
        {
            return this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Filiais.EstadoDestinoEmpresaEmissora>()
                .Where(estadoDestino => estadoDestino.Estado.Sigla == sigla)
                .FirstOrDefault();
        }

        public Dominio.Entidades.Embarcador.Filiais.EstadoDestinoEmpresaEmissora BuscarPorSiglaEEmpresa(string sigla, int codigoEmpresa)
        {
            return this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Filiais.EstadoDestinoEmpresaEmissora>()
                .Where(estadoDestino => estadoDestino.Estado.Sigla == sigla && estadoDestino.Empresa.Codigo == codigoEmpresa)
                .FirstOrDefault();
        }

        public List<Dominio.Entidades.Embarcador.Filiais.EstadoDestinoEmpresaEmissora> BuscarPorEmpresaEmissora(int codigoEmpresaEmissora)
        {
            return this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Filiais.EstadoDestinoEmpresaEmissora>()
                .Where(estadoDestino => estadoDestino.Empresa.Codigo == codigoEmpresaEmissora)
                .ToList();
        }

        public List<Dominio.Entidades.Embarcador.Filiais.EstadoDestinoEmpresaEmissora> BuscarPorFilial(int codigoFilial)
        {
            return this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Filiais.EstadoDestinoEmpresaEmissora>()
                .Where(estadoDestino => estadoDestino.Filial.Codigo == codigoFilial)
                .ToList();
        }
        
        public async Task<List<Dominio.Entidades.Embarcador.Filiais.EstadoDestinoEmpresaEmissora>> BuscarPorFilialAsync(int codigoFilial)
        {
            return await this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Filiais.EstadoDestinoEmpresaEmissora>()
                .Where(estadoDestino => estadoDestino.Filial.Codigo == codigoFilial)
                .ToListAsync();
        }

        public List<Dominio.Entidades.Embarcador.Filiais.EstadoDestinoEmpresaEmissora> BuscarEstadosPorDestinoDaCarga(int codigoDestinoCarga)
        {
            return this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Filiais.EstadoDestinoEmpresaEmissora>()
                .Where(estadoDestino => estadoDestino.Estado.Codigo == codigoDestinoCarga)
                .ToList();
        }
        #endregion
    }
}
