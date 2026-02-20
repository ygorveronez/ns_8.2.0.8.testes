using System.Linq;

namespace Repositorio.Embarcador.Configuracoes
{
    public class IntegracaoArquivei : RepositorioBase<Dominio.Entidades.Embarcador.Configuracoes.IntegracaoArquivei>
    {
        public IntegracaoArquivei(UnitOfWork unitOfWork) : base(unitOfWork) { }

        #region Métodos Públicos

        public Dominio.Entidades.Embarcador.Configuracoes.IntegracaoArquivei Buscar()
        {
            var consultaIntegracao = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Configuracoes.IntegracaoArquivei>();

            return consultaIntegracao.FirstOrDefault();
        }
        #endregion Métodos Públicos
    }
}
