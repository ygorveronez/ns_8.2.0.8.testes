using System.Collections.Generic;
using System.Linq;
using System.Linq.Dynamic.Core;

namespace Repositorio.Embarcador.Frete
{
    public class ContratoFreteTransportadorValoresOutrosRecursos : RepositorioBase<Dominio.Entidades.Embarcador.Frete.ContratoFreteTransportadorValoresOutrosRecursos>
    {
        #region Construtores

        public ContratoFreteTransportadorValoresOutrosRecursos(UnitOfWork unitOfWork) : base(unitOfWork) { }

        #endregion Construtores

        #region Métodos Privados

        private IQueryable<Dominio.Entidades.Embarcador.Frete.ContratoFreteTransportadorValoresOutrosRecursos> Consultar(Dominio.ObjetosDeValor.Embarcador.Frete.FiltroPesquisaContratoFreteValoresOutrosRecursos filtrosPesquisa)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Frete.ContratoFreteTransportadorValoresOutrosRecursos>();
            var result = from obj in query where obj.ContratoFrete.Codigo == filtrosPesquisa.CodigoContratoFreteTransportador select obj;

            if (!string.IsNullOrWhiteSpace(filtrosPesquisa.TipoMaoDeObra))
                result = result.Where(obj => obj.TipoMaoDeObra.Contains(filtrosPesquisa.TipoMaoDeObra));

            return result;
        }

        private IQueryable<Dominio.Entidades.Embarcador.Frete.ContratoFreteTransportadorValoresOutrosRecursos> _ConsultarPorContrato(int contrato)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Frete.ContratoFreteTransportadorValoresOutrosRecursos>();

            var result = from obj in query where obj.ContratoFrete.Codigo == contrato select obj;

            return result;
        }

        #endregion Métodos Privados

        #region Métodos Públicos

        public Dominio.Entidades.Embarcador.Frete.ContratoFreteTransportadorValoresOutrosRecursos BuscarPorCodigo(int codigo)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Frete.ContratoFreteTransportadorValoresOutrosRecursos>();
            var result = from obj in query where obj.Codigo == codigo select obj;
            return result.FirstOrDefault();
        }

        public List<int> BuscarValoresOutrosRecursosNaoPesentesNaLista(int contrato, List<int> codigos)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Frete.ContratoFreteTransportadorValoresOutrosRecursos>();
            var result = from obj in query
                         where
                            obj.ContratoFrete.Codigo == contrato
                            && !codigos.Contains(obj.Codigo)
                         select obj.Codigo;

            return result.ToList();
        }

        public Dominio.Entidades.Embarcador.Frete.ContratoFreteTransportadorValoresOutrosRecursos BuscarValoresRecursosPorContrato(int contrato, int outrosValoresRecursos)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Frete.ContratoFreteTransportadorValoresOutrosRecursos>();
            var result = from obj in query
                         where
                            obj.ContratoFrete.Codigo == contrato
                            && obj.Codigo == outrosValoresRecursos
                         select obj;

            return result.FirstOrDefault();
        }

        public List<Dominio.Entidades.Embarcador.Frete.ContratoFreteTransportadorValoresOutrosRecursos> ConsultarPorContrato(int contrato, string propOrdena, string dirOrdena, int inicioRegistros, int maximoRegistros)
        {
            var result = _ConsultarPorContrato(contrato);

            if (!string.IsNullOrWhiteSpace(propOrdena))
                result = result.OrderBy(propOrdena + (dirOrdena == "asc" ? " ascending" : " descending"));

            if (inicioRegistros > 0)
                result = result.Skip(inicioRegistros);

            if (maximoRegistros > 0)
                result = result.Take(maximoRegistros);

            return result.ToList();
        }

        public List<Dominio.Entidades.Embarcador.Frete.ContratoFreteTransportadorValoresOutrosRecursos> Consultar(Dominio.ObjetosDeValor.Embarcador.Frete.FiltroPesquisaContratoFreteValoresOutrosRecursos filtrosPesquisa, Dominio.ObjetosDeValor.Embarcador.Consulta.ParametroConsulta parametroConsulta)
        {
            IQueryable<Dominio.Entidades.Embarcador.Frete.ContratoFreteTransportadorValoresOutrosRecursos> result = Consultar(filtrosPesquisa);

            return ObterLista(result, parametroConsulta);
        }

        public int ContarConsulta(Dominio.ObjetosDeValor.Embarcador.Frete.FiltroPesquisaContratoFreteValoresOutrosRecursos filtrosPesquisa)
        {
            var consultaFechamentoFrete = Consultar(filtrosPesquisa);

            return consultaFechamentoFrete.Count();
        }

        public int ContarConsultaPorContrato(int contrato)
        {
            var result = _ConsultarPorContrato(contrato);

            return result.Count();
        }

        public List<Dominio.Entidades.Embarcador.Frete.ContratoFreteTransportadorValoresOutrosRecursos> BuscarPorContrato(int codigoContrato)
        {
            var consultaValoresOutrosRecursos = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Frete.ContratoFreteTransportadorValoresOutrosRecursos>().Where(o => o.ContratoFrete.Codigo == codigoContrato);

            return consultaValoresOutrosRecursos.ToList();
        }

        #endregion Métodos Públicos
    }
}