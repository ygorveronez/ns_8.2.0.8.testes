using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Dynamic.Core;

namespace Repositorio.Embarcador.Bidding.RFI
{
    public class RFIConvite : RepositorioBase<Dominio.Entidades.Embarcador.Bidding.RFI.RFIConvite>
    {
        public RFIConvite(UnitOfWork unitOfWork) : base(unitOfWork) { }

        #region Métodos Públicos

        public List<Dominio.Entidades.Embarcador.Bidding.RFI.RFIConvite> Consultar(Dominio.ObjetosDeValor.Embarcador.Bidding.RFI.FiltroPesquisaRFI filtrosPesquisa, string propOrdenacao, string dirOrdenacao, int inicioRegistros, int maximoRegistros)
        {
            var result = Consultar(filtrosPesquisa);

            return result.OrderBy(propOrdenacao + (dirOrdenacao == "asc" ? " ascending" : " descending")).Skip(inicioRegistros).Take(maximoRegistros).ToList();
        }

        public int ContarConsulta(Dominio.ObjetosDeValor.Embarcador.Bidding.RFI.FiltroPesquisaRFI filtrosPesquisa)
        {
            var result = Consultar(filtrosPesquisa);

            return result.Count();
        }

        #endregion Métodos Públicos

        #region Métodos Privados

        private IQueryable<Dominio.Entidades.Embarcador.Bidding.RFI.RFIConvite> Consultar(Dominio.ObjetosDeValor.Embarcador.Bidding.RFI.FiltroPesquisaRFI filtrosPesquisa)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Bidding.RFI.RFIConvite>();
            var queryConvidado = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Bidding.RFI.RFIConviteConvidado>();

            var result = from obj in query select obj;

            if (!string.IsNullOrWhiteSpace(filtrosPesquisa.descricao))
                result = result.Where(o => o.Descricao.Contains(filtrosPesquisa.descricao));

            if (filtrosPesquisa.dataInicio != DateTime.MinValue)
                result = result.Where(o => o.DataInicio.Date >= filtrosPesquisa.dataInicio);

            if (filtrosPesquisa.dataLimite != DateTime.MinValue)
                result = result.Where(o => o.DataLimite.Date <= filtrosPesquisa.dataLimite);

            if (filtrosPesquisa.empresa != null)            
                result = result.Where(o => queryConvidado.Where(x => x.Convidado == filtrosPesquisa.empresa && x.Status != Dominio.ObjetosDeValor.Embarcador.Enumeradores.StatusRFIConviteConvidado.Rejeitado).Select(x => x.RFIConvite.Codigo).Contains(o.Codigo) && o.DataInicio <= DateTime.Now);
            
            if (filtrosPesquisa.situacao != null && filtrosPesquisa.situacao.Count > 0)
                result = result.Where(o => filtrosPesquisa.situacao.Contains(o.Status));

            return result;
        }

        #endregion Métodos Privados
    }
}
