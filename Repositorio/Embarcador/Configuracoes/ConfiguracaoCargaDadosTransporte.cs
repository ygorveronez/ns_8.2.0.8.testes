using System.Linq;

namespace Repositorio.Embarcador.Configuracoes
{
    public class ConfiguracaoCargaDadosTransporte : RepositorioBase<Dominio.Entidades.Embarcador.Configuracoes.ConfiguracaoCargaDadosTransporte>
    {
        public ConfiguracaoCargaDadosTransporte(UnitOfWork unitOfWork) : base(unitOfWork) { }

        public Dominio.Entidades.Embarcador.Configuracoes.ConfiguracaoCargaDadosTransporte BuscarConfiguracaoPadrao()
        {
            IQueryable<Dominio.Entidades.Embarcador.Configuracoes.ConfiguracaoCargaDadosTransporte> query = SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Configuracoes.ConfiguracaoCargaDadosTransporte>();

            return query.FirstOrDefault();
        }
    }
}
