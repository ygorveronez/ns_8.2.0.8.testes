using System.Linq;
using System.Threading;

namespace Repositorio.Embarcador.TorreControle
{
    public class ConfiguracaoWidgetAcompanhamentoCargaUsuario : RepositorioBase<Dominio.Entidades.Embarcador.TorreControle.ConfiguracaoWidgetAcompanhamentoCargaUsuario>
    {
        public ConfiguracaoWidgetAcompanhamentoCargaUsuario(UnitOfWork unitOfWork) : base(unitOfWork)
        {
        }

        public ConfiguracaoWidgetAcompanhamentoCargaUsuario(UnitOfWork unitOfWork, CancellationToken cancellationToken) : base(unitOfWork, cancellationToken) { }

        #region Métodos Públicos

        public Dominio.Entidades.Embarcador.TorreControle.ConfiguracaoWidgetAcompanhamentoCargaUsuario BuscarConfiguracaoPadrao()
        {
            IQueryable<Dominio.Entidades.Embarcador.TorreControle.ConfiguracaoWidgetAcompanhamentoCargaUsuario> query = SessionNHiBernate.Query<Dominio.Entidades.Embarcador.TorreControle.ConfiguracaoWidgetAcompanhamentoCargaUsuario>();

            return query.OrderBy(obj => obj.Codigo).FirstOrDefault();
        }

        public Dominio.Entidades.Embarcador.TorreControle.ConfiguracaoWidgetAcompanhamentoCargaUsuario BuscarConfiguracaoPorUsuario(int codusuario)
        {
            IQueryable<Dominio.Entidades.Embarcador.TorreControle.ConfiguracaoWidgetAcompanhamentoCargaUsuario> query = SessionNHiBernate.Query<Dominio.Entidades.Embarcador.TorreControle.ConfiguracaoWidgetAcompanhamentoCargaUsuario>();

            return query.Where(obj => obj.Usuario.Codigo == codusuario).FirstOrDefault();
        }

        public bool DesativarAtualizacaoNovasCargas(int? codUsuario = null)
        {
            IQueryable<Dominio.Entidades.Embarcador.TorreControle.ConfiguracaoWidgetAcompanhamentoCargaUsuario> query = SessionNHiBernate.Query<Dominio.Entidades.Embarcador.TorreControle.ConfiguracaoWidgetAcompanhamentoCargaUsuario>();

            if (codUsuario.HasValue)
                query = query.Where(obj => obj.Usuario.Codigo == codUsuario);

            return query.Select(o => (bool?)o.DesativarAtualizacaoNovasCargas).FirstOrDefault() ?? false;

        }

        #endregion
    }
}
