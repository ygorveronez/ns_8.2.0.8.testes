using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Dynamic.Core;

namespace Repositorio.Embarcador.Transportadores
{
    public class MotoristaLiberacaoGR : RepositorioBase<Dominio.Entidades.Embarcador.Transportadores.MotoristaLiberacaoGR>
    {
        public MotoristaLiberacaoGR(UnitOfWork unitOfWork) : base(unitOfWork) { }

        #region Métodos Públicos


        public List<Dominio.Entidades.Embarcador.Transportadores.MotoristaLicenca> BuscarLicencasMotoristaPorUsuarioMobile(int codigoUsuarioMobile)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Transportadores.MotoristaLicenca>()
                .Where(o => o.Motorista.CodigoMobile == codigoUsuarioMobile);

            return query.ToList();
        }

        public List<Dominio.Entidades.Embarcador.Transportadores.MotoristaLiberacaoGR> BuscarLiberacoesGRPorDataVencimentoParaMobile(DateTime dataVencimento)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Transportadores.MotoristaLiberacaoGR>()
                .Where(o => o.DataVencimento.Value.Date == dataVencimento && o.Motorista.CodigoMobile > 0);

            return query.ToList();
        }

        #endregion

    }
}
