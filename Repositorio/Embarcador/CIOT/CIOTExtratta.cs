using System.Linq;

namespace Repositorio.Embarcador.CIOT
{
    public class CIOTExtratta : RepositorioBase<Dominio.Entidades.Embarcador.CIOT.CIOTExtratta>
	{
		public CIOTExtratta(UnitOfWork unitOfWork) : base (unitOfWork) { }

        public Dominio.Entidades.Embarcador.CIOT.CIOTExtratta BuscarPorConfiguracaoCIOT(int codigoConfiguracao)
        {
            IQueryable<Dominio.Entidades.Embarcador.CIOT.CIOTExtratta> query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.CIOT.CIOTExtratta>();

            query = query.Where(obj => obj.ConfiguracaoCIOT.Codigo == codigoConfiguracao);

            return query.FirstOrDefault();
        }
    }
}
