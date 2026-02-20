using System.Linq;
using System.Linq.Dynamic.Core;

namespace Repositorio.Embarcador.CIOT
{
    public class CIOTEFrete : RepositorioBase<Dominio.Entidades.Embarcador.CIOT.CIOTEFrete>
    {
        public CIOTEFrete(UnitOfWork unitOfWork) : base(unitOfWork) { }

        public Dominio.Entidades.Embarcador.CIOT.CIOTEFrete BuscarPorCodigo(int codigo)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.CIOT.CIOTEFrete>();
            var result = from obj in query where obj.Codigo == codigo select obj;
            return result.FirstOrDefault();
        }

        public Dominio.Entidades.Embarcador.CIOT.CIOTEFrete BuscarPorConfiguracaoCIOT(int codigoConfiguracao)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.CIOT.CIOTEFrete>();
            var result = from obj in query where obj.ConfiguracaoCIOT.Codigo == codigoConfiguracao select obj;
            return result.FirstOrDefault();
        }
    }
}
