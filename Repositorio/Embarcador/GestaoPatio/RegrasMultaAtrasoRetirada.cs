using Dominio.ObjetosDeValor.Embarcador.Enumeradores;
using System.Collections.Generic;
using System.Linq;

namespace Repositorio.Embarcador.GestaoPatio
{
    public class RegrasMultaAtrasoRetirada : RepositorioBase<Dominio.Entidades.Embarcador.GestaoPatio.RegrasMultaAtrasoRetirada>
    {
        #region Construtores

        public RegrasMultaAtrasoRetirada(UnitOfWork unitOfWork) : base(unitOfWork) { }

        #endregion

        #region Métodos Públicos

        public List<Dominio.Entidades.Embarcador.GestaoPatio.RegrasMultaAtrasoRetirada> Consultar(Dominio.ObjetosDeValor.Embarcador.GestaoPatio.FiltroPesquisaRegrasMultaAtrasoRetirada filtrosPesquisa, Dominio.ObjetosDeValor.Embarcador.Consulta.ParametroConsulta parametroConsulta)
        {
            var consultaRegras = Consultar(filtrosPesquisa);

            return ObterLista(consultaRegras, parametroConsulta);
        }

        public int ContarConsulta(Dominio.ObjetosDeValor.Embarcador.GestaoPatio.FiltroPesquisaRegrasMultaAtrasoRetirada filtrosPesquisa)
        {
            var consultaRegras = Consultar(filtrosPesquisa);

            return consultaRegras.Count();
        }

        public List<Dominio.Entidades.Embarcador.GestaoPatio.RegrasMultaAtrasoRetirada> BuscarAtivasPorFilial(int codigoFilial)
        {
            var consultaRegras = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.GestaoPatio.RegrasMultaAtrasoRetirada>()
                .Where(o => o.Ativo && o.Filial.Codigo == codigoFilial);

            return consultaRegras.ToList();
        }

        #endregion

        #region Métodos Privados

        private IQueryable<Dominio.Entidades.Embarcador.GestaoPatio.RegrasMultaAtrasoRetirada> Consultar(Dominio.ObjetosDeValor.Embarcador.GestaoPatio.FiltroPesquisaRegrasMultaAtrasoRetirada filtrosPesquisa)
        {
            var consultaRegras = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.GestaoPatio.RegrasMultaAtrasoRetirada>();

            if (!string.IsNullOrWhiteSpace(filtrosPesquisa.Descricao))
                consultaRegras = consultaRegras.Where(obj => obj.Descricao.Contains(filtrosPesquisa.Descricao));

            if (filtrosPesquisa.Ativo == SituacaoAtivoPesquisa.Ativo)
                consultaRegras = consultaRegras.Where(o => o.Ativo);
            else if (filtrosPesquisa.Ativo == SituacaoAtivoPesquisa.Inativo)
                consultaRegras = consultaRegras.Where(o => !o.Ativo);

            if (filtrosPesquisa.CodigoFilial > 0)
                consultaRegras = consultaRegras.Where(o => o.Filial.Codigo == filtrosPesquisa.CodigoFilial);

            if (filtrosPesquisa.CodigoTipoOcorrencia > 0)
                consultaRegras = consultaRegras.Where(o => o.TipoOcorrencia.Codigo == filtrosPesquisa.CodigoTipoOcorrencia);

            return consultaRegras;
        }

        #endregion
    }
}
