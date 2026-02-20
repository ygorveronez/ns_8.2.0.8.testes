using System.Linq;

namespace Repositorio.Embarcador.CIOT
{
    public class CIOTRodocred : RepositorioBase<Dominio.Entidades.Embarcador.CIOT.CIOTRodocred>
	{
		public CIOTRodocred(UnitOfWork unitOfWork) : base (unitOfWork) { }

        public Dominio.Entidades.Embarcador.CIOT.CIOTRodocred BuscarPorConfiguracaoCIOT(int codigoConfiguracao)
        {
            IQueryable<Dominio.Entidades.Embarcador.CIOT.CIOTRodocred> query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.CIOT.CIOTRodocred>();

            query = query.Where(obj => obj.ConfiguracaoCIOT.Codigo == codigoConfiguracao);

            return query.FirstOrDefault();
        }
    }
}
