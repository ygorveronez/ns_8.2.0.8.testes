using System.Linq;

namespace Repositorio.Embarcador.Configuracoes
{
    public class IntegracaoCadastroMulti : RepositorioBase<Dominio.Entidades.Embarcador.Configuracoes.IntegracaoCadastroMulti>
    {
        public IntegracaoCadastroMulti(UnitOfWork unitOfWork) : base(unitOfWork) { }

        public Dominio.Entidades.Embarcador.Configuracoes.IntegracaoCadastroMulti Buscar()
        {
            var consultaIntegracaoCadastroMulti = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Configuracoes.IntegracaoCadastroMulti>();

            return consultaIntegracaoCadastroMulti.FirstOrDefault();
        }
    }
}
