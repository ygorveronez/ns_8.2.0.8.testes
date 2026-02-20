using System.Collections.Generic;
using System.Linq;
using System.Linq.Dynamic.Core;

namespace Repositorio.Embarcador.ICMS
{
    public class RegraExtensao : Repositorio.RepositorioBase<Dominio.Entidades.Embarcador.ICMS.RegraExtensao>
    {
        #region Construtores

        public RegraExtensao(UnitOfWork unitOfWork) : base(unitOfWork) { }

        #endregion

        #region Métodos Privados

        private IQueryable<Dominio.Entidades.Embarcador.ICMS.RegraExtensao> Consultar(Dominio.ObjetosDeValor.Embarcador.ICMS.RegraExtensao filtrosPesquisa)
        {
            var consultaRegraExtensao = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.ICMS.RegraExtensao>();

            if (!string.IsNullOrEmpty(filtrosPesquisa.Extensao))
                consultaRegraExtensao = consultaRegraExtensao.Where(obj => obj.Extensao.Contains(filtrosPesquisa.Extensao));

            if (filtrosPesquisa.CodigoModeloVeicular > 0)
                consultaRegraExtensao = consultaRegraExtensao.Where(obj => obj.ModeloVeicularCarga.Codigo == filtrosPesquisa.CodigoModeloVeicular);

            if(filtrosPesquisa.TipoPropriedade.HasValue)
                consultaRegraExtensao = consultaRegraExtensao.Where(obj => obj.TipoPropriedade == filtrosPesquisa.TipoPropriedade.Value);

            return consultaRegraExtensao;
        }

        #endregion

        #region Métodos Públicos
        public int ContarConsulta(Dominio.ObjetosDeValor.Embarcador.ICMS.RegraExtensao filtrosPesquisa)
        {
            var consultaRegraExtensao = Consultar(filtrosPesquisa);

            return consultaRegraExtensao.Count();
        }

        public List<Dominio.Entidades.Embarcador.ICMS.RegraExtensao> Consultar(Dominio.ObjetosDeValor.Embarcador.ICMS.RegraExtensao filtrosPesquisa, Dominio.ObjetosDeValor.Embarcador.Consulta.ParametroConsulta parametrosConsulta)
        {
            var consultaRegraExtensao = Consultar(filtrosPesquisa);

            return ObterLista(consultaRegraExtensao, parametrosConsulta);
        }
        #endregion
    }
}
