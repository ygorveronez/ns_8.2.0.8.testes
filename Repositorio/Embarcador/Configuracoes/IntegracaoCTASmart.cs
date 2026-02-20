using System.Linq;

namespace Repositorio.Embarcador.Configuracoes
{
    public class IntegracaoCTASmart : RepositorioBase<Dominio.Entidades.Embarcador.Configuracoes.IntegracaoCTASmart>
    {
        public IntegracaoCTASmart(UnitOfWork unitOfWork) : base(unitOfWork) { }

        #region Métodos Públicos

        public Dominio.Entidades.Embarcador.Configuracoes.IntegracaoCTASmart Buscar()
        {
            var consultaIntegracao = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Configuracoes.IntegracaoCTASmart>();

            return consultaIntegracao.FirstOrDefault();
        }

        #endregion
    }
}
