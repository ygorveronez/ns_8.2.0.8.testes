using System.Linq;

namespace Repositorio.Embarcador.Configuracoes
{
    public class ConfiguracaoMontagemCarga : RepositorioBase<Dominio.Entidades.Embarcador.Configuracoes.ConfiguracaoMontagemCarga>
    {
        public ConfiguracaoMontagemCarga(UnitOfWork unitOfWork) : base(unitOfWork) { }

        public bool ExisteExibirListagemNotasFiscais()
        {
            var query = SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Configuracoes.ConfiguracaoMontagemCarga>();

            return query.Select(o => (bool?)o.ExibirListagemNotasFiscais).FirstOrDefault() ?? false;
        }
    }
}
