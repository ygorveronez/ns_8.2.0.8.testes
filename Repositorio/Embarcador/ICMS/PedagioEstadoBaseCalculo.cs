using NHibernate.Linq;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Repositorio.Embarcador.ICMS
{
    public class PedagioEstadoBaseCalculo : RepositorioBase<Dominio.Entidades.Embarcador.ICMS.PedagioEstadoBaseCalculo>
    {
        #region Construtores

        public PedagioEstadoBaseCalculo(UnitOfWork unitOfWork) : base(unitOfWork) { }

        #endregion

        #region Métodos Públicos

        public List<Dominio.Entidades.Embarcador.ICMS.PedagioEstadoBaseCalculo> BuscarPorEstados(List<string> siglas)
        {
            var consultaPedagioEstadoBaseCalculo = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.ICMS.PedagioEstadoBaseCalculo>()
                .Where(o => siglas.Contains(o.Estado.Sigla));

            return consultaPedagioEstadoBaseCalculo.ToList();
        }
        
        public async Task<List<Dominio.Entidades.Embarcador.ICMS.PedagioEstadoBaseCalculo>> BuscarPorEstadosAsync(List<string> siglas)
        {
            var consultaPedagioEstadoBaseCalculo = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.ICMS.PedagioEstadoBaseCalculo>()
                .Where(o => siglas.Contains(o.Estado.Sigla));

            return await consultaPedagioEstadoBaseCalculo.ToListAsync();
        }

        public Dominio.Entidades.Embarcador.ICMS.PedagioEstadoBaseCalculo BuscarPorEstado(string sigla)
        {
            var consultaPedagioEstadoBaseCalculo = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.ICMS.PedagioEstadoBaseCalculo>()
                .Where(o => o.Estado.Sigla == sigla);

            return consultaPedagioEstadoBaseCalculo.FirstOrDefault();
        }

        #endregion
    }
}
