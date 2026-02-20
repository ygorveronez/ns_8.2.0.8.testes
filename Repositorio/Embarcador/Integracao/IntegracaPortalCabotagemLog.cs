using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Repositorio.Embarcador.Integracao
{
    public class IntegracaPortalCabotagemLog : RepositorioBase<Dominio.Entidades.Embarcador.Integracao.IntegracaPortalCabotagemLog>
    {
        public IntegracaPortalCabotagemLog(UnitOfWork unitOfWork) : base(unitOfWork) { }

        public Dominio.Entidades.Embarcador.Integracao.IntegracaPortalCabotagemLog BuscarPorCodigo(int codigo)
        {
            var arquivoIntegracao = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Integracao.IntegracaPortalCabotagemLog>()
                .Where(o => o.Codigo == codigo)
                .FirstOrDefault();

            return arquivoIntegracao;
        }

    }
}
