using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;

namespace Repositorio.Embarcador.Configuracoes
{
    public class IntegracaoComproveiRota : RepositorioBase<Dominio.Entidades.Embarcador.Configuracoes.IntegracaoComproveiRota>
    {
        #region Construtores

        public IntegracaoComproveiRota(UnitOfWork unitOfWork) : base(unitOfWork) { }

        public IntegracaoComproveiRota(UnitOfWork unitOfWork, CancellationToken cancellationToken) : base(unitOfWork, cancellationToken) { }

        #endregion Construtores

        #region Métodos Públicos

        public Dominio.Entidades.Embarcador.Configuracoes.IntegracaoComproveiRota Buscar()
        {
            var consultaIntegracao = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Configuracoes.IntegracaoComproveiRota>();
            return consultaIntegracao.FirstOrDefault();
        }

        #endregion
    }
}
