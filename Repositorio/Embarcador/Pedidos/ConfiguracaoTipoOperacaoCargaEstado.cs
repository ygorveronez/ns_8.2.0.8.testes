using NHibernate.Linq;
using System.Collections.Generic;
using System.Linq;

namespace Repositorio.Embarcador.Pedidos
{
    public class ConfiguracaoTipoOperacaoCargaEstado : RepositorioBase<Dominio.Entidades.Embarcador.Pedidos.ConfiguracaoTipoOperacaoCargaEstado>
    {
        public ConfiguracaoTipoOperacaoCargaEstado(UnitOfWork unitOfWork) : base(unitOfWork) { }

        public List<Dominio.Entidades.Embarcador.Pedidos.ConfiguracaoTipoOperacaoCargaEstado> BuscarPorConfiguracao(int codigo)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Pedidos.ConfiguracaoTipoOperacaoCargaEstado>();

            var result = from obj in query
                         where obj.Configuracao.Codigo == codigo 
                         select obj;

            return result.Fetch(x => x.Estado).ToList();
        }
    }
}