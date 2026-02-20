using System.Collections.Generic;
using System.Linq;
using System.Linq.Dynamic.Core;

namespace Repositorio.Embarcador.Terceiros
{
    public class ContratoFreteAcrescimoDescontoAutomatico : RepositorioBase<Dominio.Entidades.Embarcador.Terceiros.ContratoFreteAcrescimoDescontoAutomatico>
    {
        public ContratoFreteAcrescimoDescontoAutomatico(UnitOfWork unitOfWork) : base(unitOfWork) { }

        #region Métodos Públicos

        public Dominio.Entidades.Embarcador.Terceiros.ContratoFreteAcrescimoDescontoAutomatico BuscarPorCodigo(int codigo)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Terceiros.ContratoFreteAcrescimoDescontoAutomatico>();
            var resut = from obj in query where obj.Codigo == codigo select obj;
            return resut.FirstOrDefault();
        }

        public List<Dominio.Entidades.Embarcador.Terceiros.ContratoFreteAcrescimoDescontoAutomatico> Consultar(Dominio.ObjetosDeValor.Embarcador.Terceiros.FiltroPesquisaContratoFreteAcrescimoDescontoAutomatico filtrosPesquisa, Dominio.ObjetosDeValor.Embarcador.Consulta.ParametroConsulta parametrosConsulta)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Terceiros.ContratoFreteAcrescimoDescontoAutomatico>();

            var result = from obj in query select obj;

            if (!string.IsNullOrWhiteSpace(filtrosPesquisa.Descricao))
                result = result.Where(obj => obj.Descricao.Contains(filtrosPesquisa.Descricao));

            if (filtrosPesquisa.Justificativa > 0)
                result = result.Where(obj => obj.Justificativa.Codigo.Equals(filtrosPesquisa.Justificativa));

            return ObterLista(result, parametrosConsulta);
        }

        public int ContarConsulta(Dominio.ObjetosDeValor.Embarcador.Terceiros.FiltroPesquisaContratoFreteAcrescimoDescontoAutomatico filtrosPesquisa)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Terceiros.ContratoFreteAcrescimoDescontoAutomatico>();

            var result = from obj in query select obj;

            if (!string.IsNullOrWhiteSpace(filtrosPesquisa.Descricao))
                result = result.Where(obj => obj.Descricao.Contains(filtrosPesquisa.Descricao));

            if (filtrosPesquisa.Justificativa > 0)
                result = result.Where(obj => obj.Justificativa.Codigo.Equals(filtrosPesquisa.Justificativa));

            return result.Count();
        }

        #endregion
    }
}
