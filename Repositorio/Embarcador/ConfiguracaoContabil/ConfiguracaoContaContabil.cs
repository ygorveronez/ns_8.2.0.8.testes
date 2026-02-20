using LinqKit;
using NHibernate.Linq;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Dynamic.Core;

namespace Repositorio.Embarcador.ConfiguracaoContabil
{
    public class ConfiguracaoContaContabil : Repositorio.RepositorioBase<Dominio.Entidades.Embarcador.ConfiguracaoContabil.ConfiguracaoContaContabil>
    {
        #region Construtores
        public ConfiguracaoContaContabil(UnitOfWork unitOfWork) : base(unitOfWork) { }

        #endregion

        #region Métodos de Consulta        

        public List<Dominio.ObjetosDeValor.Embarcador.ConfiguracaoContabil.ConfiguracaoContaContabil> FiltrarPorParticipantes(Dominio.ObjetosDeValor.Embarcador.ConfiguracaoContabil.ConfiguracaoContaContabil configuracaoContaContabilValidacao, IList<Dominio.ObjetosDeValor.Embarcador.ConfiguracaoContabil.ConfiguracaoContaContabil> configuracaoContaContabils, bool ignorarFiltros = false)
        {
            var predicateOr = PredicateBuilder.False<Dominio.ObjetosDeValor.Embarcador.ConfiguracaoContabil.ConfiguracaoContaContabil>();

            int codigoGrupoRemetenteAnterior = configuracaoContaContabilValidacao.CodigoGrupoRemetente;
            int codigoGrupoDestinatarioAnterior = configuracaoContaContabilValidacao.CodigoGrupoDestinatario;
            int codigoGrupoTomadorAnterior = configuracaoContaContabilValidacao.CodigoGrupoTomador;
            int codigoCategoriaDestinatarioAnterior = configuracaoContaContabilValidacao.CodigoCategoriaDestinatario;
            int codigoCategoriaRemetenteAnterior = configuracaoContaContabilValidacao.CodigoCategoriaRemetente;
            int codigoCategoriaTomadorAnterior = configuracaoContaContabilValidacao.CodigoCategoriaTomador;

            if (ignorarFiltros)
            {
                configuracaoContaContabilValidacao.CodigoGrupoRemetente = 0;
                configuracaoContaContabilValidacao.CodigoGrupoDestinatario = 0;
                configuracaoContaContabilValidacao.CodigoGrupoTomador = 0;
                configuracaoContaContabilValidacao.CodigoCategoriaDestinatario = 0;
                configuracaoContaContabilValidacao.CodigoCategoriaRemetente = 0;
                configuracaoContaContabilValidacao.CodigoCategoriaTomador = 0;
            }

            if (configuracaoContaContabilValidacao.CodigoRemetente > 0)
            {
                if ((configuracaoContaContabilValidacao.CodigoGrupoRemetente > 0) || (configuracaoContaContabilValidacao.CodigoCategoriaRemetente > 0))
                    predicateOr = predicateOr.Or(obj => (obj.CodigoRemetente > 0 && obj.CodigoRemetente == configuracaoContaContabilValidacao.CodigoRemetente) || (obj.CodigoGrupoRemetente > 0 && obj.CodigoGrupoRemetente == configuracaoContaContabilValidacao.CodigoGrupoRemetente) || (obj.CodigoCategoriaRemetente > 0 && obj.CodigoCategoriaRemetente == configuracaoContaContabilValidacao.CodigoCategoriaRemetente));
                else if (configuracaoContaContabilValidacao.CodigoGrupoRemetente > 0)
                    predicateOr = predicateOr.Or(obj => (obj.CodigoRemetente > 0 && obj.CodigoRemetente == configuracaoContaContabilValidacao.CodigoRemetente) || (obj.CodigoGrupoRemetente > 0 && obj.CodigoGrupoRemetente == configuracaoContaContabilValidacao.CodigoGrupoRemetente));
                else if (configuracaoContaContabilValidacao.CodigoCategoriaRemetente > 0)
                    predicateOr = predicateOr.Or(obj => (obj.CodigoRemetente > 0 && obj.CodigoRemetente == configuracaoContaContabilValidacao.CodigoRemetente) || (obj.CodigoCategoriaRemetente > 0 && obj.CodigoCategoriaRemetente == configuracaoContaContabilValidacao.CodigoCategoriaRemetente));
                else
                    predicateOr = predicateOr.Or(obj => obj.CodigoRemetente > 0 && obj.CodigoRemetente == configuracaoContaContabilValidacao.CodigoRemetente);
            }

            if (configuracaoContaContabilValidacao.CodigoDestinatario > 0)
            {
                if ((configuracaoContaContabilValidacao.CodigoGrupoDestinatario > 0) && (configuracaoContaContabilValidacao.CodigoCategoriaDestinatario > 0))
                    predicateOr = predicateOr.Or(obj => (obj.CodigoDestinatario > 0 && obj.CodigoDestinatario == configuracaoContaContabilValidacao.CodigoDestinatario) || (obj.CodigoGrupoDestinatario > 0 && obj.CodigoGrupoDestinatario == configuracaoContaContabilValidacao.CodigoGrupoDestinatario) || (obj.CodigoCategoriaDestinatario > 0 && obj.CodigoCategoriaDestinatario == configuracaoContaContabilValidacao.CodigoCategoriaDestinatario));
                else if (configuracaoContaContabilValidacao.CodigoGrupoDestinatario > 0)
                    predicateOr = predicateOr.Or(obj => (obj.CodigoDestinatario > 0 && obj.CodigoDestinatario == configuracaoContaContabilValidacao.CodigoDestinatario) || (obj.CodigoGrupoDestinatario > 0 && obj.CodigoGrupoDestinatario == configuracaoContaContabilValidacao.CodigoGrupoDestinatario));
                else if (configuracaoContaContabilValidacao.CodigoCategoriaDestinatario > 0)
                    predicateOr = predicateOr.Or(obj => (obj.CodigoDestinatario > 0 && obj.CodigoDestinatario == configuracaoContaContabilValidacao.CodigoDestinatario) || (obj.CodigoCategoriaDestinatario > 0 && obj.CodigoCategoriaDestinatario == configuracaoContaContabilValidacao.CodigoCategoriaDestinatario));
                else
                    predicateOr = predicateOr.Or(obj => obj.CodigoDestinatario > 0 && obj.CodigoDestinatario == configuracaoContaContabilValidacao.CodigoDestinatario);
            }

            if (configuracaoContaContabilValidacao.CodigoTomador > 0)
            {
                if ((configuracaoContaContabilValidacao.CodigoGrupoTomador > 0) && (configuracaoContaContabilValidacao.CodigoCategoriaTomador > 0))
                    predicateOr = predicateOr.Or(obj => (obj.CodigoTomador > 0 && obj.CodigoTomador == configuracaoContaContabilValidacao.CodigoTomador) || (obj.CodigoGrupoTomador > 0 && obj.CodigoGrupoTomador == configuracaoContaContabilValidacao.CodigoGrupoTomador) || (obj.CodigoCategoriaTomador > 0 && obj.CodigoCategoriaTomador == configuracaoContaContabilValidacao.CodigoCategoriaTomador));
                else if (configuracaoContaContabilValidacao.CodigoGrupoTomador > 0)
                    predicateOr = predicateOr.Or(obj => (obj.CodigoTomador > 0 && obj.CodigoTomador == configuracaoContaContabilValidacao.CodigoTomador) || (obj.CodigoGrupoTomador > 0 && obj.CodigoGrupoTomador == configuracaoContaContabilValidacao.CodigoGrupoTomador));
                else if (configuracaoContaContabilValidacao.CodigoCategoriaTomador > 0)
                    predicateOr = predicateOr.Or(obj => (obj.CodigoTomador > 0 && obj.CodigoTomador == configuracaoContaContabilValidacao.CodigoTomador) || (obj.CodigoCategoriaTomador > 0 && obj.CodigoCategoriaTomador == configuracaoContaContabilValidacao.CodigoCategoriaTomador));
                else
                    predicateOr = predicateOr.Or(obj => obj.CodigoTomador > 0 && obj.CodigoTomador == configuracaoContaContabilValidacao.CodigoTomador);
            }

            IQueryable<Dominio.ObjetosDeValor.Embarcador.ConfiguracaoContabil.ConfiguracaoContaContabil> configuracaoContaContabilsQueryable = configuracaoContaContabils.AsQueryable();

            configuracaoContaContabilsQueryable = configuracaoContaContabilsQueryable.Where(predicateOr);

            configuracaoContaContabils = configuracaoContaContabilsQueryable.ToList();

            if (ignorarFiltros)
            {
                configuracaoContaContabilValidacao.CodigoGrupoRemetente = codigoGrupoRemetenteAnterior;
                configuracaoContaContabilValidacao.CodigoGrupoDestinatario = codigoGrupoDestinatarioAnterior;
                configuracaoContaContabilValidacao.CodigoGrupoTomador = codigoGrupoTomadorAnterior;
                configuracaoContaContabilValidacao.CodigoCategoriaDestinatario = codigoCategoriaDestinatarioAnterior;
                configuracaoContaContabilValidacao.CodigoCategoriaRemetente = codigoCategoriaRemetenteAnterior;
                configuracaoContaContabilValidacao.CodigoCategoriaTomador = codigoCategoriaTomadorAnterior;
            }

            return configuracaoContaContabils.OrderByDescending(obj => obj.CodigoModeloDocumentoFiscal > 0).ToList();
        }

        public IList<Dominio.ObjetosDeValor.Embarcador.ConfiguracaoContabil.ConfiguracaoContaContabil> ConsultarConfiguracaoContaContabilAtiva()
        {
            var consulta = this.SessionNHiBernate.CreateSQLQuery(QueryConsultarConfiguracaoContaContabilAtiva());

            consulta.SetResultTransformer(new NHibernate.Transform.AliasToBeanResultTransformer(typeof(Dominio.ObjetosDeValor.Embarcador.ConfiguracaoContabil.ConfiguracaoContaContabil)));

            return consulta.SetTimeout(600).List<Dominio.ObjetosDeValor.Embarcador.ConfiguracaoContabil.ConfiguracaoContaContabil>();
        }

        #endregion

        #region Métodos Publicos

        public Dominio.Entidades.Embarcador.ConfiguracaoContabil.ConfiguracaoContaContabil BuscarPorCodigo(int codigo)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.ConfiguracaoContabil.ConfiguracaoContaContabil>();
            var result = from obj in query where obj.Codigo == codigo select obj;
            return result.FirstOrDefault();
        }

        public List<Dominio.Entidades.Embarcador.ConfiguracaoContabil.ConfiguracaoContaContabil> Consultar(Dominio.ObjetosDeValor.Embarcador.Contabil.FiltroPesquisaConfiguracaoContaContabil filtrosPesquisa, Dominio.ObjetosDeValor.Embarcador.Consulta.ParametroConsulta parametrosConsulta)
        {
            var query = MontarConsulta(filtrosPesquisa);

            query = query
                .Fetch(obj => obj.Empresa)
                .ThenFetch(obj => obj.Localidade)
                .Fetch(obj => obj.Tomador)
                .Fetch(obj => obj.CategoriaTomador)
                .Fetch(obj => obj.GrupoTomador)
                .Fetch(obj => obj.Remetente)
                .Fetch(obj => obj.CategoriaRemetente)
                .Fetch(obj => obj.GrupoRemetente)
                .Fetch(obj => obj.Destinatario)
                .Fetch(obj => obj.CategoriaDestinatario)
                .Fetch(obj => obj.GrupoDestinatario)
                .Fetch(obj => obj.TipoOperacao)
                .Fetch(obj => obj.TipoOcorrencia)
                .Fetch(obj => obj.ModeloDocumentoFiscal);

            return ObterLista(query, parametrosConsulta);
        }

        public int ContarConsulta(Dominio.ObjetosDeValor.Embarcador.Contabil.FiltroPesquisaConfiguracaoContaContabil filtrosPesquisa)
        {
            var query = MontarConsulta(filtrosPesquisa);
            return query.Count();
        }

        public Dominio.Entidades.Embarcador.ConfiguracaoContabil.ConfiguracaoContaContabil BuscarPorParametros(double remetente, double destinatario, double tomador, int grupoRemetente, int grupoDestinatario, int grupoTomador, int empresa, int tipoOperacao, int grupoProdutoEmbarcador, int rotaFrete, int codigoCategoriaRemetente, int codigoCategoriaDestinatario, int codigoCategoriaTomador, int modeloDocumento, int tipoOcorrencia, bool possuiIVA, int codigoCanalEntrega, int codigoCanalVenda, int codigoTipoDT)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.ConfiguracaoContabil.ConfiguracaoContaContabil>();

            var result = from obj in query select obj;

            if (remetente > 0)
                result = result.Where(obj => obj.Remetente.CPF_CNPJ == remetente);
            else
                result = result.Where(obj => obj.Remetente == null);

            if (destinatario > 0)
                result = result.Where(obj => obj.Destinatario.CPF_CNPJ == destinatario);
            else
                result = result.Where(obj => obj.Destinatario == null);

            if (tomador > 0)
                result = result.Where(obj => obj.Tomador.CPF_CNPJ == tomador);
            else
                result = result.Where(obj => obj.Tomador == null);

            if (tipoOperacao > 0)
                result = result.Where(obj => obj.TipoOperacao.Codigo == tipoOperacao);
            else
                result = result.Where(obj => obj.TipoOperacao == null);

            if (rotaFrete > 0)
                result = result.Where(obj => obj.RotaFrete.Codigo == rotaFrete);
            else
                result = result.Where(obj => obj.RotaFrete == null);

            if (empresa > 0)
                result = result.Where(obj => obj.Empresa.Codigo == empresa);
            else
                result = result.Where(obj => obj.Empresa == null);

            if (grupoProdutoEmbarcador > 0)
                result = result.Where(obj => obj.GrupoProduto.Codigo == grupoProdutoEmbarcador);
            else
                result = result.Where(obj => obj.GrupoProduto == null);

            if (grupoRemetente > 0)
                result = result.Where(obj => obj.GrupoRemetente.Codigo == grupoRemetente);
            else
                result = result.Where(obj => obj.GrupoRemetente == null);

            if (grupoDestinatario > 0)
                result = result.Where(obj => obj.GrupoDestinatario.Codigo == grupoDestinatario);
            else
                result = result.Where(obj => obj.GrupoDestinatario == null);

            if (grupoTomador > 0)
                result = result.Where(obj => obj.GrupoTomador.Codigo == grupoTomador);
            else
                result = result.Where(obj => obj.GrupoTomador == null);

            if (codigoCategoriaRemetente > 0)
                result = result.Where(obj => obj.CategoriaRemetente.Codigo == codigoCategoriaRemetente);
            else
                result = result.Where(obj => obj.CategoriaRemetente == null);

            if (codigoCategoriaDestinatario > 0)
                result = result.Where(obj => obj.CategoriaDestinatario.Codigo == codigoCategoriaDestinatario);
            else
                result = result.Where(obj => obj.CategoriaDestinatario == null);

            if (codigoCategoriaTomador > 0)
                result = result.Where(obj => obj.CategoriaTomador.Codigo == codigoCategoriaTomador);
            else
                result = result.Where(obj => obj.CategoriaTomador == null);

            if (modeloDocumento > 0)
                result = result.Where(obj => obj.ModeloDocumentoFiscal.Codigo == modeloDocumento);

            if (tipoOcorrencia > 0)
                result = result.Where(obj => obj.TipoOcorrencia.Codigo == tipoOcorrencia);

            if (possuiIVA && codigoCanalEntrega > 0)
                result = result.Where(obj => obj.CanalEntrega.Codigo == codigoCanalEntrega);

            if (possuiIVA && codigoCanalVenda > 0)
                result = result.Where(obj => obj.CanalVenda.Codigo == codigoCanalVenda);

            if (possuiIVA && codigoTipoDT > 0)
                result = result.Where(obj => obj.TipoDT.Codigo == codigoTipoDT);

            result = result.Where(obj => obj.Ativo == true);

            return result.FirstOrDefault();
        }

        #endregion


        #region Métodos Privados
        private IQueryable<Dominio.Entidades.Embarcador.ConfiguracaoContabil.ConfiguracaoContaContabil> MontarConsulta(Dominio.ObjetosDeValor.Embarcador.Contabil.FiltroPesquisaConfiguracaoContaContabil filtrosPesquisa)
        {

            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.ConfiguracaoContabil.ConfiguracaoContaContabil>();

            var result = from obj in query select obj;

            if (filtrosPesquisa.CPFCNPJRemetente > 0)
                result = result.Where(obj => obj.Remetente.CPF_CNPJ == filtrosPesquisa.CPFCNPJRemetente);

            if (filtrosPesquisa.CPFCNPJDestinatario > 0)
                result = result.Where(obj => obj.Destinatario.CPF_CNPJ == filtrosPesquisa.CPFCNPJDestinatario);

            if (filtrosPesquisa.CPFCNPJTomador > 0)
                result = result.Where(obj => obj.Tomador.CPF_CNPJ == filtrosPesquisa.CPFCNPJTomador);

            if (filtrosPesquisa.CodigoGrupoRemetente > 0)
                result = result.Where(obj => obj.GrupoRemetente.Codigo == filtrosPesquisa.CodigoGrupoRemetente);

            if (filtrosPesquisa.CodigoEmpresa > 0)
                result = result.Where(obj => obj.Empresa.Codigo == filtrosPesquisa.CodigoEmpresa);

            if (filtrosPesquisa.CodigoGrupoDestinatario > 0)
                result = result.Where(obj => obj.GrupoDestinatario.Codigo == filtrosPesquisa.CodigoGrupoDestinatario);

            if (filtrosPesquisa.CodigoGrupoTomador > 0)
                result = result.Where(obj => obj.GrupoTomador.Codigo == filtrosPesquisa.CodigoGrupoTomador);

            if (filtrosPesquisa.CodigoCategoriaDestinatario > 0)
                result = result.Where(obj => obj.CategoriaDestinatario.Codigo == filtrosPesquisa.CodigoCategoriaDestinatario);

            if (filtrosPesquisa.CodigoCategoriaRemetente > 0)
                result = result.Where(obj => obj.CategoriaRemetente.Codigo == filtrosPesquisa.CodigoCategoriaRemetente);

            if (filtrosPesquisa.CodigoCategoriaTomador > 0)
                result = result.Where(obj => obj.CategoriaTomador.Codigo == filtrosPesquisa.CodigoCategoriaTomador);

            if (filtrosPesquisa.CodigoTipoOperacao > 0)
                result = result.Where(obj => obj.TipoOperacao.Codigo == filtrosPesquisa.CodigoTipoOperacao);

            if (filtrosPesquisa.Ativo == Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoAtivoPesquisa.Ativo)
                result = result.Where(obj => obj.Ativo == true);

            if (filtrosPesquisa.Ativo == Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoAtivoPesquisa.Inativo)
                result = result.Where(obj => obj.Ativo == false);

            if (filtrosPesquisa.CodigoModeloDocumento > 0)
                result = result.Where(obj => obj.ModeloDocumentoFiscal.Codigo == filtrosPesquisa.CodigoModeloDocumento);

            if (filtrosPesquisa.CodigoTipoOcorrencia > 0)
                result = result.Where(obj => obj.TipoOcorrencia.Codigo == filtrosPesquisa.CodigoTipoOcorrencia);

            if (filtrosPesquisa.CodigoCanalEntrega > 0)
                result = result.Where(obj => obj.CanalEntrega.Codigo == filtrosPesquisa.CodigoCanalEntrega);

            if (filtrosPesquisa.CodigoCanalVenda > 0)
                result = result.Where(obj => obj.CanalVenda.Codigo == filtrosPesquisa.CodigoCanalVenda);

            if (filtrosPesquisa.CodigoTipoDT > 0)
                result = result.Where(obj => obj.TipoDT.Codigo == filtrosPesquisa.CodigoTipoDT);

            return result;
        }

        private string QueryConsultarConfiguracaoContaContabilAtiva()
        {
            string sql;


            sql = @"select 
                        CCC_CODIGO as Codigo,
                        EMP_CODIGO as CodigoEmpresa,
                        CLI_REMETENTE as CodigoRemetente,
                        CLI_DESTINATARIO as CodigoDestinatario,
                        CLI_TOMADOR as CodigoTomador,
                        CTP_REMETENTE as CodigoCategoriaRemetente,  
                        CTP_DESTINATARIO as CodigoCategoriaDestinatario,
                        CTP_TOMADOR as CodigoCategoriaTomador,
                        GRP_REMETENTE as CodigoGrupoRemetente,   
                        GRP_DESTINATARIO as CodigoGrupoDestinatario,
                        GRP_TOMADOR as CodigoGrupoTomador,
                        GPR_CODIGO as CodigoGrupoProduto,
                        configuracaoContaContabil.TOP_CODIGO as CodigoTipoOperacao,
                        configuracaoContaContabil.MOD_CODIGO as CodigoModeloDocumentoFiscal,
                        configuracaoContaContabil.OCO_CODIGO as CodigoTipoOcorrencia,
                        ROF_CODIGO as CodigoRotaFrete,
                        CCC_ATIVO as Ativo,
                        tipoOperacao.TOP_DESCRICAO as Descricao";

            sql += @"   from T_CONFIGURACAO_CONTA_CONTABIL as configuracaoContaContabil
                        left join T_TIPO_OPERACAO tipoOperacao on tipoOperacao.TOP_CODIGO = configuracaoContaContabil.TOP_CODIGO";


            sql += " where CCC_ATIVO = 1";

            return sql;
        }

        #endregion

    }
}
