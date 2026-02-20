using System.Linq;

namespace Repositorio.Embarcador.Configuracoes
{
    public class ConfiguracaoModeloEmailAnexo : RepositorioBase<Dominio.Entidades.Embarcador.Configuracoes.ConfiguracaoModeloEmailAnexo>
    {
        public ConfiguracaoModeloEmailAnexo(UnitOfWork unitOfWork) : base(unitOfWork) { }

        public Dominio.Entidades.Embarcador.Configuracoes.ConfiguracaoModeloEmailAnexo BuscarPorModeloEmail(int codigoModeloEmail)
        {
            IQueryable<Dominio.Entidades.Embarcador.Configuracoes.ConfiguracaoModeloEmailAnexo> consultaModelosEmail = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Configuracoes.ConfiguracaoModeloEmailAnexo>();
            consultaModelosEmail = consultaModelosEmail.Where(modelo => modelo.ConfiguracaoModeloEmail.Codigo == codigoModeloEmail);
            return consultaModelosEmail.FirstOrDefault();
        }
    }
}
