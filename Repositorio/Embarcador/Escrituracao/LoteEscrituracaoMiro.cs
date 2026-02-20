using Dominio.ObjetosDeValor.Embarcador.Escrituracao;
using System.Collections.Generic;
using System.Linq;

namespace Repositorio.Embarcador.Escrituracao
{
    public class LoteEscrituracaoMiro : RepositorioBase<Dominio.Entidades.Embarcador.Escrituracao.LoteEscrituracaoMiro>
    {
        #region Contructores
        public LoteEscrituracaoMiro(Repositorio.UnitOfWork unitOfWork) : base(unitOfWork) { }
        #endregion

        #region Metodos Publicos

        public List<Dominio.Entidades.Embarcador.Escrituracao.LoteEscrituracaoMiro> Consultar(LoteEscrituracaoMiroFiltro filtroPequisa, Dominio.ObjetosDeValor.Embarcador.Consulta.ParametroConsulta parametrosConsulta)
        {
            var consulta = Consultar(filtroPequisa);
            return ObterLista(consulta, parametrosConsulta);
        }

        public int ContaConsulta(LoteEscrituracaoMiroFiltro filtroPequisa)
        {
            var consulta = Consultar(filtroPequisa);
            return consulta.Count();
        }

        public int ObterUltimoNumeroLote(LoteEscrituracaoMiroFiltro filtroPequisa)
        {
            IQueryable<Dominio.Entidades.Embarcador.Escrituracao.LoteEscrituracaoMiro> query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Escrituracao.LoteEscrituracaoMiro>();
            int? ultimoNumero = query.Select(x => x.Codigo).OrderByDescending(x => x).FirstOrDefault();
            return ultimoNumero.HasValue ? ultimoNumero.Value : 0;
        }

        #endregion

        #region Metodos Publics
        private IQueryable<Dominio.Entidades.Embarcador.Escrituracao.LoteEscrituracaoMiro> Consultar(LoteEscrituracaoMiroFiltro filtroPequisa)
        {
            IQueryable<Dominio.Entidades.Embarcador.Escrituracao.LoteEscrituracaoMiro> query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Escrituracao.LoteEscrituracaoMiro>();
            if (filtroPequisa.Carga > 0)
                query = query.Where(x => x.Carga.Codigo == filtroPequisa.Carga);
            if (filtroPequisa.Situacao != Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoLoteEscrituracaoMiro.Todos)
                query = query.Where(x => x.Situacao == filtroPequisa.Situacao);
            if (filtroPequisa.Empresa > 0)
                query = query.Where(x => x.Empresa.Codigo == filtroPequisa.Empresa);
            if (filtroPequisa.DataInicio.HasValue)
                query = query.Where(x => x.DataGeracaoLote >= filtroPequisa.DataInicio);
            if (filtroPequisa.DataFim.HasValue)
                query = query.Where(x => x.DataGeracaoLote <= filtroPequisa.DataFim);
            return query;
        }
        #endregion
    }
}
