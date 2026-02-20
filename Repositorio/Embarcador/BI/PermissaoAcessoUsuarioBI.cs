using System;
using System.Collections.Generic;
using System.Linq;

namespace Repositorio.Embarcador.BI
{
    public class PermissaoAcessoUsuarioBI : RepositorioBase<Dominio.Entidades.Embarcador.BI.PermissaoAcessoUsuarioBI>
    {
        public PermissaoAcessoUsuarioBI(UnitOfWork unitOfWork) : base(unitOfWork)
        {

        }

        #region Métodos Públicos

        public Dominio.Entidades.Embarcador.BI.PermissaoAcessoUsuarioBI BuscarPorUsuarioECodigoReportBI(int codigoUsuario, string caminhoFormulario)
        {
            var query = SessionNHiBernate.Query<Dominio.Entidades.Embarcador.BI.PermissaoAcessoUsuarioBI>()
                .Where(o => o.Usuario.Codigo == codigoUsuario &&
                o.CaminhoFormulario == caminhoFormulario);

            return query.FirstOrDefault();
        }

        public Dominio.Entidades.Embarcador.BI.PermissaoAcessoUsuarioBI BuscarPorUsuarioECodigoReportBI(int codigoUsuario, int ID)
        {
            var query = SessionNHiBernate.Query<Dominio.Entidades.Embarcador.BI.PermissaoAcessoUsuarioBI>()
                .Where(o => o.Usuario.Codigo == codigoUsuario &&
                o.CodigoFormularioBI == ID);

            return query.FirstOrDefault();
        }

        public Dominio.Entidades.Embarcador.BI.PermissaoAcessoUsuarioBI BuscarPorUsuarioEPermissao(int codigoUsuario, AdminMultisoftware.Dominio.Enumeradores.PermissaoPersonalizada permissao)
        {
            var query = SessionNHiBernate.Query<Dominio.Entidades.Embarcador.BI.PermissaoAcessoUsuarioBI>()
                .Where(o => o.Usuario.Codigo == codigoUsuario &&
                o.PermissaoPersonalizada == permissao);

            return query.FirstOrDefault();
        }

        public Dominio.Entidades.Embarcador.BI.PermissaoAcessoUsuarioBI BuscarPorPermissao(AdminMultisoftware.Dominio.Enumeradores.PermissaoPersonalizada permissao)
        {
            var query = SessionNHiBernate.Query<Dominio.Entidades.Embarcador.BI.PermissaoAcessoUsuarioBI>()
                .Where(o => o.PermissaoPersonalizada == permissao);

            return query.FirstOrDefault();
        }

        public IList<Dominio.ObjetosDeValor.Embarcador.Dashboard.DashboardDocumentacao> BuscarDashboardDocumentacao()
        {
            string sql = @"
							SELECT
								d.NavioAberto,
								d.[Status navio] as StatusNavio,
								d.Navio as NomeNavio,
								d.[Data ETS POL] as DataEtsPol,
								d.Região as Regiao,
								d.MDFe as Mdfe,
								d.[Data fechamento] as DataFechamento,
								COALESCE(d.Bookings,0) as Bookings,
								COALESCE(d.[Cargas Pendentes],0) as TotalCargaPedentes,
								COALESCE(d.[Cargas Ag.Emissão],0) as TotalCargaEmissao,
								COALESCE(d.[Cargas Com erro],0) as TotalCargaErro,
								COALESCE(d.[Cargas Geradas],0) as TotalCargaGerada,
								COALESCE(d.[SVM Pendentes],0) as TotalSvmPendente,
								COALESCE(d.[SVM Com erro],0) as TotalSvmErro,
								COALESCE(d.[SVM Geradas],0) as TotalSvmGerado,
								COALESCE(d.[Mercante Pendentes],0) as TotalMercantePendente,
								COALESCE(d.[Mercante Retornados],0) as TotalMercanteRetornado,
								COALESCE(d.[Total pendente MDFe],0) as TotalPendenteMdfe
							FROM (
								SELECT 
									CASE WHEN [Navio fechado/aberto] = 'Aberto' THEN 1 ELSE 0 END as NavioAberto,
									*
								FROM [dbo].[VW_DASH] d
								GROUP BY 
									[Navio fechado/aberto],
									d.[Status navio],
									d.Navio,
									d.[Data ETS POL],
									d.Embarques,
									d.Região,
									d.MDFe,
									d.[Data fechamento],
									d.Bookings,
									d.[Cargas Pendentes],
									d.[Cargas Ag.Emissão],
									d.[Cargas Com erro],
									d.[Cargas Geradas],
									d.[SVM Pendentes],
									d.[SVM Com erro],
									d.[SVM Geradas],
									d.[Mercante Pendentes],
									d.[Mercante Retornados],
									d.[Total pendente MDFe]
							) as d
							GROUP BY 
								[Status navio],
								Navio,
								[Data ETS POL],
								Embarques,
								Região,
								MDFe,
								[Data fechamento],
								Bookings,
								[Cargas Pendentes],
								[Cargas Ag.Emissão],
								[Cargas Com erro],
								[Cargas Geradas],
								[SVM Pendentes],
								[SVM Com erro],
								[SVM Geradas],
								[Mercante Pendentes],
								[Mercante Retornados],
								[Total pendente MDFe],
								NavioAberto";

            var query = this.SessionNHiBernate.CreateSQLQuery(sql);

            query.SetResultTransformer(new NHibernate.Transform.AliasToBeanResultTransformer(typeof(Dominio.ObjetosDeValor.Embarcador.Dashboard.DashboardDocumentacao)));

            return query.SetTimeout(6000).List<Dominio.ObjetosDeValor.Embarcador.Dashboard.DashboardDocumentacao>();
        }

        public IList<Dominio.ObjetosDeValor.Embarcador.Dashboard.DashboardDocumentacao> PesquisarDashboardDocumentacao(Dominio.ObjetosDeValor.Embarcador.Dashboard.FiltroPesquisaDashboardDocumentacao filtrosPesquisa)
        {
            string sql = @"
							SELECT
								d.NavioAberto,
								d.[Status navio] as StatusNavio,
								d.Navio as NomeNavio,
								d.[Data ETS POL] as DataEtsPol,
								d.Região as Regiao,
								d.MDFe as Mdfe,
								d.[Data fechamento] as DataFechamento,
								COALESCE(d.Bookings,0) as Bookings,
								COALESCE(d.[Cargas Pendentes],0) as TotalCargaPedentes,
								COALESCE(d.[Cargas Ag.Emissão],0) as TotalCargaEmissao,
								COALESCE(d.[Cargas Com erro],0) as TotalCargaErro,
								COALESCE(d.[Cargas Geradas],0) as TotalCargaGerada,
								COALESCE(d.[SVM Pendentes],0) as TotalSvmPendente,
								COALESCE(d.[SVM Com erro],0) as TotalSvmErro,
								COALESCE(d.[SVM Geradas],0) as TotalSvmGerado,
								COALESCE(d.[Mercante Pendentes],0) as TotalMercantePendente,
								COALESCE(d.[Mercante Retornados],0) as TotalMercanteRetornado,
								COALESCE(d.[Total pendente MDFe],0) as TotalPendenteMdfe
							FROM (
								SELECT 
									CASE WHEN [Navio fechado/aberto] = 'Aberto' THEN 1 ELSE 0 END as NavioAberto,
									*
								FROM [dbo].[VW_DASH] d
								GROUP BY 
									[Navio fechado/aberto],
									d.[Status navio],
									d.Navio,
									d.[Data ETS POL],
									d.Embarques,
									d.Região,
									d.MDFe,
									d.[Data fechamento],
									d.Bookings,
									d.[Cargas Pendentes],
									d.[Cargas Ag.Emissão],
									d.[Cargas Com erro],
									d.[Cargas Geradas],
									d.[SVM Pendentes],
									d.[SVM Com erro],
									d.[SVM Geradas],
									d.[Mercante Pendentes],
									d.[Mercante Retornados],
									d.[Total pendente MDFe]
							) as d";

            var filtros = ObterFiltrosDocumentacaoDash(filtrosPesquisa);

            sql += !string.IsNullOrEmpty(filtros) ? " WHERE 1 = 1 " + filtros : " ";

            sql += @"
					GROUP BY 
							[Status navio],
							Navio,
							[Data ETS POL],
							Embarques,
							Região,
							MDFe,
							[Data fechamento],
							Bookings,
							[Cargas Pendentes],
							[Cargas Ag.Emissão],
							[Cargas Com erro],
							[Cargas Geradas],
							[SVM Pendentes],
							[SVM Com erro],
							[SVM Geradas],
							[Mercante Pendentes],
							[Mercante Retornados],
							[Total pendente MDFe],
							NavioAberto";

            var query = this.SessionNHiBernate.CreateSQLQuery(sql);

            query.SetResultTransformer(new NHibernate.Transform.AliasToBeanResultTransformer(typeof(Dominio.ObjetosDeValor.Embarcador.Dashboard.DashboardDocumentacao)));

			return query.SetTimeout(6000).List<Dominio.ObjetosDeValor.Embarcador.Dashboard.DashboardDocumentacao>();
        }

        public IList<Dominio.ObjetosDeValor.Embarcador.Dashboard.Embarques> ObterPortos(Dominio.ObjetosDeValor.Embarcador.Dashboard.FiltroPesquisaDashboardDocumentacao filtrosPesquisa)
        {
            string regiao = string.Empty;

            if (filtrosPesquisa.Regiao.Equals("1"))
                regiao = "('RO', 'AC', 'AM', 'RR', 'PA', 'AP', 'TO')";
            else if (filtrosPesquisa.Regiao.Equals("2"))
                regiao = "('MA', 'PI', 'CE', 'RN', 'PB', 'PE', 'AL', 'SE', 'BA')";
            else if (filtrosPesquisa.Regiao.Equals("3"))
                regiao = "('SC', 'PR', 'RS')";
            else if (filtrosPesquisa.Regiao.Equals("4"))
                regiao = "('MG', 'ES', 'SP', 'RJ')";
            else
                regiao = "('MS', 'MT', 'DF', 'GO')";

            string sql = $@"SELECT
								POT_CODIGO Codigo, 
								POT_DESCRICAO Descricao
							FROM T_PORTO Porto
							left join T_LOCALIDADES Localidades on Porto.LOC_CODIGO = Localidades.LOC_CODIGO
							WHERE POT_ATIVO = 1
							and Localidades.UF_SIGLA in {regiao}
							GROUP BY 
								POT_CODIGO,
								POT_DESCRICAO";

            var query = this.SessionNHiBernate.CreateSQLQuery(sql);

            query.SetResultTransformer(new NHibernate.Transform.AliasToBeanResultTransformer(typeof(Dominio.ObjetosDeValor.Embarcador.Dashboard.Embarques)));

			return query.SetTimeout(6000).List<Dominio.ObjetosDeValor.Embarcador.Dashboard.Embarques>();
        }

        public IList<Dominio.ObjetosDeValor.Embarcador.Dashboard.DetalhesDashboardDocumentacao> PesquisarDetalhesDashboardDocumentacao(Dominio.ObjetosDeValor.Embarcador.Dashboard.FiltroPesquisaDetalhesDashboardDocumentacao filtrosPesquisa)
        {
            string sql = @"
							SELECT 
								[Cód.Carga] as CodigoCarga
								,[Navio] as NomeNavio
								,[Data ETS POL] as DataEtsPol
								,[Data fechamento] as DataFechamento
								,[Porto Origem] as PortoOrigem
								,[Porto Destino] as PortoDestino
								,Região as Regiao
								,Carga
								,[Link Carga] as LinkCarga
								,Booking
								,[Status Carga] as StatusCarga
								,[Status SVM] as StatusSvm
								,[Status Mercante] as StatusMercante
								,Containeres
								,[Qtd Cntr Carga] as QtdCntrCarga
								,Modal
								,[TipoTomador] as TipoTomador
								,[Remetente] as Remetente
								,[Destinatario] as Destinatario
							FROM 
								[dbo].[VW_DASH_DETALHES]";

            var filtros = ObterFiltrosDetalhesDocumentacaoDash(filtrosPesquisa);

            sql += !string.IsNullOrEmpty(filtros) ? " WHERE 1 = 1 " + filtros : " ";

            var query = this.SessionNHiBernate.CreateSQLQuery(sql);

            query.SetResultTransformer(new NHibernate.Transform.AliasToBeanResultTransformer(typeof(Dominio.ObjetosDeValor.Embarcador.Dashboard.DetalhesDashboardDocumentacao)));

			return query.SetTimeout(6000).List<Dominio.ObjetosDeValor.Embarcador.Dashboard.DetalhesDashboardDocumentacao>();
        }

		#endregion

		#region Métodos Privados

		private string ObterFiltrosDocumentacaoDash(Dominio.ObjetosDeValor.Embarcador.Dashboard.FiltroPesquisaDashboardDocumentacao filtrosPesquisa)
		{
			var sql = string.Empty;
			var columnDateFilter = filtrosPesquisa.NavioFechado is true ? "[Data fechamento]" : "[Data ETS POL]";

			if (filtrosPesquisa.DataInicial.HasValue && filtrosPesquisa.DataInicial != DateTime.MinValue)
				sql += $" AND d.{columnDateFilter} >= '{filtrosPesquisa.DataInicial?.ToString("yyyy/MM/dd")}' ";

			if (filtrosPesquisa.DataFinal.HasValue && filtrosPesquisa.DataFinal != DateTime.MinValue)
				sql += $" AND d.{columnDateFilter} <= '{filtrosPesquisa.DataFinal?.ToString("yyyy/MM/dd")}'";

            if (!string.IsNullOrEmpty(filtrosPesquisa.Regiao))
            {
                switch (filtrosPesquisa.Regiao)
                {
                    case "1":
                        sql += " AND d.Região like '%" + "Norte" + "%' ";
                        break;
                    case "2":
                        sql += " AND d.Região like '%" + "Nordeste" + "%' ";
                        break;
                    case "3":
                        sql += " AND d.Região like '%" + "Sul" + "%' ";
                        break;
                    case "4":
                        sql += " AND d.Região like '%" + "Sudeste" + "%' ";
                        break;
                    case "5":
                        sql += " AND d.Região like '%" + "Centro Oeste" + "%' ";
                        break;
                }
            }

            if (filtrosPesquisa.NavioAberto == true)
                sql += " AND d.[Navio fechado/aberto] = 'Aberto' ";

            if (filtrosPesquisa.NavioFechado == true)
                sql += " AND d.[Navio fechado/aberto] = 'Fechado' ";

            if (filtrosPesquisa.Embarques != null && filtrosPesquisa.Embarques.Any())
                sql += $" AND d.Embarques in (" + string.Join(",", filtrosPesquisa.Embarques.Select(i => $"'{i}'")) +  ")";

			return sql;
        }

        private string ObterFiltrosDetalhesDocumentacaoDash(Dominio.ObjetosDeValor.Embarcador.Dashboard.FiltroPesquisaDetalhesDashboardDocumentacao filtrosPesquisa)
        {
            var sql = string.Empty;

            if (!string.IsNullOrEmpty(filtrosPesquisa.NomeNavio))
                sql += " AND [Navio] like '%" + filtrosPesquisa.NomeNavio + "%' ";

            if (!string.IsNullOrEmpty(filtrosPesquisa.StatusCarga))
                sql += " AND [Status Carga] like '%" + filtrosPesquisa.StatusCarga + "%' ";

            if (!string.IsNullOrEmpty(filtrosPesquisa.StatusSvm))
                sql += " AND [Status SVM] like '%" + filtrosPesquisa.StatusSvm + "%' ";

            if (!string.IsNullOrEmpty(filtrosPesquisa.StatusMercante))
                sql += " AND [Status Mercante] like '%" + filtrosPesquisa.StatusMercante + "%' ";

            return sql;
        }

        #endregion
    }
}