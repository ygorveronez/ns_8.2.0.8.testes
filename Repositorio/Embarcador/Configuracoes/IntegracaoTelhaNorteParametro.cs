using System.Collections.Generic;
using System.Linq;

namespace Repositorio.Embarcador.Configuracoes
{
    public class IntegracaoTelhaNorteParametro : RepositorioBase<Dominio.Entidades.Embarcador.Configuracoes.IntegracaoTelhaNorteParametro>
    {
        public IntegracaoTelhaNorteParametro(UnitOfWork unitOfWork) : base(unitOfWork) { }

        #region Métodos Públicos
        
        public List<Dominio.Entidades.Embarcador.Configuracoes.IntegracaoTelhaNorteParametro> Buscar()
        {
            var consultaIntegracaoGadle = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Configuracoes.IntegracaoTelhaNorteParametro>();

            return consultaIntegracaoGadle
                .ToList();
        }

        #endregion
    }
}
