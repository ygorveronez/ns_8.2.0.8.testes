using System.Collections.Generic;
using System.Linq;

namespace Repositorio.Embarcador.Fechamento
{
    public class FechamentoFreteValoresOutrosRecursos : RepositorioBase<Dominio.Entidades.Embarcador.Fechamento.FechamentoFreteValoresOutrosRecursos>
    {
        #region Construtores

        public FechamentoFreteValoresOutrosRecursos(UnitOfWork unitOfWork) : base(unitOfWork) { }

        #endregion Construtores

        #region Métodos Privados

        private IQueryable<Dominio.Entidades.Embarcador.Fechamento.FechamentoFreteValoresOutrosRecursos> Consultar(Dominio.ObjetosDeValor.Embarcador.Fechamento.FiltroPesquisaFechamentoFreteValoresOutrosRecursos filtrosPesquisa)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Fechamento.FechamentoFreteValoresOutrosRecursos>();
            var result = from obj in query where obj.ValoresOutrosRecursos.ContratoFrete.Codigo == filtrosPesquisa.CodigoContratoFreteTransportador select obj;

            if (!string.IsNullOrWhiteSpace(filtrosPesquisa.TipoMaoDeObra))
                result = result.Where(obj => obj.ValoresOutrosRecursos.TipoMaoDeObra.Contains(filtrosPesquisa.TipoMaoDeObra));

            return result;
        }

        private IQueryable<Dominio.Entidades.Embarcador.Fechamento.FechamentoFreteValoresOutrosRecursos> _ConsultarPorFechamento(int contrato)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Fechamento.FechamentoFreteValoresOutrosRecursos>();

            var result = from obj in query where obj.ValoresOutrosRecursos.Codigo == contrato select obj;

            return result;
        }

        #endregion Métodos Privados

        #region Métodos Públicos

        public Dominio.Entidades.Embarcador.Fechamento.FechamentoFreteValoresOutrosRecursos BuscarPorCodigo(int codigo)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Fechamento.FechamentoFreteValoresOutrosRecursos>();
            var result = from obj in query where obj.Codigo == codigo select obj;
            return result.FirstOrDefault();
        }

        public List<Dominio.Entidades.Embarcador.Fechamento.FechamentoFreteValoresOutrosRecursos> BuscarPorFechamento(int codigoFechamento)
        {
            var consultaValoresOutrosRecursos = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Fechamento.FechamentoFreteValoresOutrosRecursos>().Where(o => o.Fechamento.Codigo == codigoFechamento);

            return consultaValoresOutrosRecursos.ToList();
        }

        public List<Dominio.Entidades.Embarcador.Fechamento.FechamentoFreteValoresOutrosRecursos> Consultar(Dominio.ObjetosDeValor.Embarcador.Fechamento.FiltroPesquisaFechamentoFreteValoresOutrosRecursos filtrosPesquisa, Dominio.ObjetosDeValor.Embarcador.Consulta.ParametroConsulta parametroConsulta)
        {
            IQueryable<Dominio.Entidades.Embarcador.Fechamento.FechamentoFreteValoresOutrosRecursos> result = Consultar(filtrosPesquisa);

            return ObterLista(result, parametroConsulta);
        }

        public int ContarConsulta(Dominio.ObjetosDeValor.Embarcador.Fechamento.FiltroPesquisaFechamentoFreteValoresOutrosRecursos filtrosPesquisa)
        {
            IQueryable<Dominio.Entidades.Embarcador.Fechamento.FechamentoFreteValoresOutrosRecursos> result = Consultar(filtrosPesquisa);

            return result.Count();
        }

        #endregion Métodos Públicos
    }
}
