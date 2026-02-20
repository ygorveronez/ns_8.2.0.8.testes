using System.Linq;

namespace Repositorio.Embarcador.CIOT
{
    public class CIOTBBC : RepositorioBase<Dominio.Entidades.Embarcador.CIOT.CIOTBBC>
	{
		public CIOTBBC(UnitOfWork unitOfWork) : base (unitOfWork) { }

        public Dominio.Entidades.Embarcador.CIOT.CIOTBBC BuscarPorConfiguracaoCIOT(int codigoConfiguracao)
        {
            IQueryable<Dominio.Entidades.Embarcador.CIOT.CIOTBBC> query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.CIOT.CIOTBBC>();

            query = query.Where(obj => obj.ConfiguracaoCIOT.Codigo == codigoConfiguracao);

            return query.FirstOrDefault();
        }
    }
}
