using System.Collections.Generic;
using System.Linq;
using System.Linq.Dynamic.Core;
using Dominio.Relatorios.Embarcador.ObjetosDeValor;

namespace Repositorio.Embarcador.Pedidos
{
    public class Navio : RepositorioBase<Dominio.Entidades.Embarcador.Pedidos.Navio>
    {
        public Navio(UnitOfWork unitOfWork) : base(unitOfWork) { }

        public Dominio.Entidades.Embarcador.Pedidos.Navio BuscarPorCodigo(int codigo)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Pedidos.Navio>();

            var result = from obj in query where obj.Codigo == codigo select obj;

            return result.FirstOrDefault();
        }

        public List<Dominio.Entidades.Embarcador.Pedidos.Navio> ConsultarPentendeIntegracao(string propOrdenacao, string dirOrdenacao, int inicioRegistros, int maximoRegistros)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Pedidos.Navio>();

            var result = from obj in query where obj.Integrado == false || obj.Integrado == null select obj;

            return result.OrderBy(propOrdenacao + (dirOrdenacao == "asc" ? " ascending" : " descending")).Skip(inicioRegistros).Take(maximoRegistros).ToList();
        }

        public int ContarConsultarPentendeIntegracao()
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Pedidos.Navio>();

            var result = from obj in query where obj.Integrado == false || obj.Integrado == null select obj;

            return result.Count();
        }

        public bool ContemNavioMesmoNumeroEmbarcacao(string numeroEmbarcacao, int codigo)
        {
            var infracao = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Pedidos.Navio>()
                .Where(o => o.Codigo != codigo && o.CodigoEmbarcacao == numeroEmbarcacao);

            return infracao.Any();
        }

        public Dominio.Entidades.Embarcador.Pedidos.Navio BuscarTodosPorCodigoIntegracao(string codigo)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Pedidos.Navio>();
            var result = from obj in query where obj.CodigoIntegracao == codigo select obj;
            return result.FirstOrDefault();
        }
        public Dominio.Entidades.Embarcador.Pedidos.Navio BuscarTodosPorCodigoIRIN(string codigo)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Pedidos.Navio>();
            var result = from obj in query where obj.Irin == codigo select obj;
            return result.FirstOrDefault();
        }

        public Dominio.Entidades.Embarcador.Pedidos.Navio BuscarTodosPorCodigoIMOConcatIRIN(string codigoIMO, string irin)
        {
           
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Pedidos.Navio>();
            var result = from obj in query where obj.CodigoIMO == codigoIMO && obj.Irin == irin select obj;
            return result.FirstOrDefault();
        }


        public Dominio.Entidades.Embarcador.Pedidos.Navio BuscarTodosPorDescricao(string descricao)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Pedidos.Navio>();
            var result = from obj in query where obj.Descricao == descricao select obj;
            return result.FirstOrDefault();
        }

        public Dominio.Entidades.Embarcador.Pedidos.Navio BuscarPorDescricao(string descricao)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Pedidos.Navio>();
            var result = from obj in query where obj.Status && obj.Descricao == descricao select obj;
            return result.FirstOrDefault();
        }

        public Dominio.Entidades.Embarcador.Pedidos.Navio BuscarPorCodigoIMO(string codigo)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Pedidos.Navio>();
            var result = from obj in query where obj.Status && obj.CodigoIMO == codigo select obj;
            return result.FirstOrDefault();
        }

        public Dominio.Entidades.Embarcador.Pedidos.Navio BuscarPorCodigoDocumento(string codigoDocumento)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Pedidos.Navio>();
            query = query.Where(obj => obj.Status && obj.CodigoDocumento.Equals(codigoDocumento));
            return query.FirstOrDefault();
        }

        public Dominio.Entidades.Embarcador.Pedidos.Navio BuscarPorNavioID(string codigo)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Pedidos.Navio>();
            var result = from obj in query where obj.NavioID == codigo select obj;
            return result.FirstOrDefault();
        }

        public Dominio.Entidades.Embarcador.Pedidos.Navio BuscarPorCodigoIntegracao(string codigo)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Pedidos.Navio>();
            var result = from obj in query where obj.CodigoIntegracao == codigo && obj.Status select obj;
            return result.FirstOrDefault();
        }

        public bool PossuiNavioAtivoMesmoIMO(string codigoIMO)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Pedidos.Navio>();
            query = query.Where(navio => navio.Status && navio.CodigoIMO.Equals(codigoIMO));
            return query.Any();
        }

        public bool PossuiNavioAtivoMesmoVesselID(string vesselID)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Pedidos.Navio>();
            query = query.Where(navio => navio.Status && navio.NavioID.Equals(vesselID));
            return query.Any();
        }

        public List<Dominio.Entidades.Embarcador.Pedidos.Navio> Consultar(Dominio.ObjetosDeValor.Embarcador.Pedido.FiltroPesquisaNavio filtrosPesquisa, Dominio.ObjetosDeValor.Embarcador.Consulta.ParametroConsulta parametrosConsulta)
        {
            var consultaNavio = Consultar(filtrosPesquisa);

            return ObterLista(consultaNavio, parametrosConsulta);
        }

        public int ContarConsulta(Dominio.ObjetosDeValor.Embarcador.Pedido.FiltroPesquisaNavio filtrosPesquisa)
        {
            var consultaNavio = Consultar(filtrosPesquisa);

            return consultaNavio.Count();
        }

        private IQueryable<Dominio.Entidades.Embarcador.Pedidos.Navio> Consultar(Dominio.ObjetosDeValor.Embarcador.Pedido.FiltroPesquisaNavio filtrosPesquisa)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Pedidos.Navio>();

            var result = from obj in query select obj;

            if (!string.IsNullOrWhiteSpace(filtrosPesquisa.Descricao))
                result = result.Where(obj => obj.Descricao.Contains(filtrosPesquisa.Descricao));

            if (!string.IsNullOrWhiteSpace(filtrosPesquisa.CodigoIntegracao))
                result = result.Where(obj => obj.CodigoIntegracao.Contains(filtrosPesquisa.CodigoIntegracao));

            if (filtrosPesquisa.Status == Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoAtivoPesquisa.Ativo)
                result = result.Where(o => o.Status);
            else if (filtrosPesquisa.Status == Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoAtivoPesquisa.Inativo)
                result = result.Where(o => !o.Status);

            if (filtrosPesquisa.TipoEmbarcacao > 0)
                result = result.Where(obj => obj.TipoEmbarcacao == filtrosPesquisa.TipoEmbarcacao);

            return result;
        }

        public IList<Dominio.Relatorios.Embarcador.DataSource.Logistica.Navio> ConsultarRelatorioNavio(Dominio.ObjetosDeValor.Embarcador.Logistica.FiltroPesquisaRelatorioNavio filtrosPesquisa, List<PropriedadeAgrupamento> propriedades, Dominio.ObjetosDeValor.Embarcador.Consulta.ParametroConsulta parametrosConsulta)
        {
            var query = new Logistica.Consulta.ConsultaNavio().ObterSqlPesquisa(filtrosPesquisa, parametrosConsulta, propriedades).CriarSQLQuery(this.SessionNHiBernate);

            query.SetResultTransformer(new NHibernate.Transform.AliasToBeanResultTransformer(typeof(Dominio.Relatorios.Embarcador.DataSource.Logistica.Navio)));

            return query.SetTimeout(600).List<Dominio.Relatorios.Embarcador.DataSource.Logistica.Navio>();
        }

        public int ContarConsultaRelatorioNavio(Dominio.ObjetosDeValor.Embarcador.Logistica.FiltroPesquisaRelatorioNavio filtrosPesquisa, List<PropriedadeAgrupamento> propriedades)
        {
            var query = new Logistica.Consulta.ConsultaNavio().ObterSqlContarPesquisa(filtrosPesquisa, propriedades).CriarSQLQuery(this.SessionNHiBernate);

            return query.SetTimeout(600).UniqueResult<int>();
        }
    }
}
