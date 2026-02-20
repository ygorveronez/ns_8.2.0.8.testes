using System.Collections.Generic;
using System.Linq;
using System.Linq.Dynamic.Core;
using Dominio.ObjetosDeValor.Embarcador.Enumeradores;

namespace Repositorio.Embarcador.NotaFiscal
{
    public class ItemNaoConformidade : RepositorioBase<Dominio.Entidades.Embarcador.NotaFiscal.ItemNaoConformidade>
    {
        public ItemNaoConformidade(UnitOfWork unitOfWork) : base(unitOfWork) { }

        #region Métodos Públicos

        public Dominio.Entidades.Embarcador.NotaFiscal.ItemNaoConformidade BuscarPorCodigo(int codigo)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.NotaFiscal.ItemNaoConformidade>();
            var result = from obj in query where obj.Codigo == codigo select obj;
            return result.FirstOrDefault();
        }

        public List<Dominio.Entidades.Embarcador.NotaFiscal.ItemNaoConformidade> BuscarPorNotaFiscalComNaoConformidadePendente(int numeroNota)
        {
            List<SituacaoNaoConformidade> situacaoNaoConformidadesPendentes = SituacaoNaoConformidadeHelper.ObterSituacoesPendentes();

            var consultaNaoConformidade = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.NotaFiscal.NaoConformidade>()
                .Where(naoConformidade => naoConformidade.XMLNotaFiscal.Numero == numeroNota && situacaoNaoConformidadesPendentes.Contains(naoConformidade.Situacao)).Select(naoConformidade => naoConformidade.ItemNaoConformidade);

            return consultaNaoConformidade.ToList();
        }

        public List<Dominio.Entidades.Embarcador.NotaFiscal.ItemNaoConformidade> Consultar(Dominio.ObjetosDeValor.Embarcador.NotaFiscal.FiltroPesquisaItemNaoConformidade filtrosPesquisa, Dominio.ObjetosDeValor.Embarcador.Consulta.ParametroConsulta parametrosConsulta)
        {
            var result = Consultar(filtrosPesquisa);

            return ObterLista(result, parametrosConsulta);
        }

        public int ContarConsulta(Dominio.ObjetosDeValor.Embarcador.NotaFiscal.FiltroPesquisaItemNaoConformidade filtrosPesquisa)
        {
            var result = Consultar(filtrosPesquisa);

            return result.Count();
        }

        public int ContarConsultaNota(Dominio.ObjetosDeValor.Embarcador.NotaFiscal.FiltroPesquisaHistoricoNaoConformidade filtrosPesquisa)
        {
            var result = ConsultarPorNota(filtrosPesquisa);

            return result.Count();
        }

        public List<Dominio.Entidades.Embarcador.NotaFiscal.ItemNaoConformidade> ConsultarNota(Dominio.ObjetosDeValor.Embarcador.NotaFiscal.FiltroPesquisaHistoricoNaoConformidade filtrosPesquisa, Dominio.ObjetosDeValor.Embarcador.Consulta.ParametroConsulta parametrosConsulta)
        {
            var result = ConsultarPorNota(filtrosPesquisa);

            return ObterLista(result, parametrosConsulta);
        }
        
        public List<Dominio.ObjetosDeValor.Embarcador.NotaFiscal.ItemNaoConformidade> BuscarAtivos()
        {
            var consultaItemNaoConformidade = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.NotaFiscal.ItemNaoConformidade>()
                .Where(item => item.Status == true && item.IrrelevanteParaNC == false);

            return consultaItemNaoConformidade
                .Select(item => new Dominio.ObjetosDeValor.Embarcador.NotaFiscal.ItemNaoConformidade()
                {
                    Codigo = item.Codigo,
                    Area = item.Area,
                    Descricao = item.Descricao,
                    Grupo = item.Grupo,
                    SubGrupo = item.SubGrupo,
                    TipoRegra = item.TipoRegra
                })
                .ToList();
        }
        public List<Dominio.Entidades.Embarcador.NotaFiscal.ItemNaoConformidade> BuscarPorCodigos(List<int> codigos)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.NotaFiscal.ItemNaoConformidade>();
            var result = from obj in query where codigos.Contains(obj.Codigo) select obj;
            return result.ToList();
        }

        public IList<Dominio.Relatorios.Embarcador.DataSource.NotasFiscais.ItemNaoConformidade> ConsultarRelatorioItemNaoConformidade(Dominio.ObjetosDeValor.Embarcador.NotaFiscal.FiltroPesquisaRelatorioItemNaoConformidade filtrosPesquisa, List<Dominio.Relatorios.Embarcador.ObjetosDeValor.PropriedadeAgrupamento> propriedades, Dominio.ObjetosDeValor.Embarcador.Consulta.ParametroConsulta parametrosConsulta)
        {
            var consulta = new ConsultaItemNaoConformidade().ObterSqlPesquisa(filtrosPesquisa, parametrosConsulta, propriedades).CriarSQLQuery(this.SessionNHiBernate);

            consulta.SetResultTransformer(new NHibernate.Transform.AliasToBeanResultTransformer(typeof(Dominio.Relatorios.Embarcador.DataSource.NotasFiscais.ItemNaoConformidade)));

            return consulta.SetTimeout(600).List<Dominio.Relatorios.Embarcador.DataSource.NotasFiscais.ItemNaoConformidade>();
        }

        public int ContarConsultaRelatorioItemNaoConformidade(Dominio.ObjetosDeValor.Embarcador.NotaFiscal.FiltroPesquisaRelatorioItemNaoConformidade filtrosPesquisa, List<Dominio.Relatorios.Embarcador.ObjetosDeValor.PropriedadeAgrupamento> propriedades)
        {
            var consulta = new ConsultaItemNaoConformidade().ObterSqlContarPesquisa(filtrosPesquisa, propriedades).CriarSQLQuery(this.SessionNHiBernate);

            return consulta.SetTimeout(600).UniqueResult<int>();
        }

        #endregion

        #region Métodos Privados

        private IQueryable<Dominio.Entidades.Embarcador.NotaFiscal.ItemNaoConformidade> Consultar(Dominio.ObjetosDeValor.Embarcador.NotaFiscal.FiltroPesquisaItemNaoConformidade filtrosPesquisa)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.NotaFiscal.ItemNaoConformidade>();
            query = from obj in query select obj;

            if (!string.IsNullOrWhiteSpace(filtrosPesquisa.Descricao))
                query = query.Where(obj => obj.Descricao.Contains(filtrosPesquisa.Descricao));

            if (filtrosPesquisa.Status.HasValue)
                query = query.Where(obj => obj.Status == filtrosPesquisa.Status.Value);

            if (filtrosPesquisa.Grupo.HasValue)
                query = query.Where(obj => obj.Grupo == filtrosPesquisa.Grupo.Value);

            if (filtrosPesquisa.SubGrupo.HasValue)
                query = query.Where(obj => obj.SubGrupo == filtrosPesquisa.SubGrupo.Value);

            if (filtrosPesquisa.Area.HasValue)
                query = query.Where(obj => obj.Area == filtrosPesquisa.Area.Value);

            if (filtrosPesquisa.IrrelevanteParaNC.HasValue)
                query = query.Where(obj => obj.IrrelevanteParaNC == filtrosPesquisa.IrrelevanteParaNC.Value);

            if (filtrosPesquisa.PermiteContingencia.HasValue)
                query = query.Where(obj => obj.Status == filtrosPesquisa.PermiteContingencia.Value);

            return query;
        }

        private IQueryable<Dominio.Entidades.Embarcador.NotaFiscal.ItemNaoConformidade> ConsultarPorNota(Dominio.ObjetosDeValor.Embarcador.NotaFiscal.FiltroPesquisaHistoricoNaoConformidade filtrosPesquisa)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.NotaFiscal.ItemNaoConformidade>();
            query = from obj in query select obj;

            if (!string.IsNullOrWhiteSpace(filtrosPesquisa.NotaFiscal))
                query = query.Where(obj => obj.NotaFiscal.Contains(filtrosPesquisa.NotaFiscal));

            return query;
        }
        
        #endregion
    }
}
