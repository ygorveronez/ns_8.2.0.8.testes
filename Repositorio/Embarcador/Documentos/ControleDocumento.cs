using Dominio.ObjetosDeValor.Embarcador.Enumeradores;
using NHibernate.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Dynamic.Core;

namespace Repositorio.Embarcador.Documentos
{
    public class ControleDocumento : RepositorioBase<Dominio.Entidades.Embarcador.Documentos.ControleDocumento>
    {
        #region Construtores

        public ControleDocumento(UnitOfWork unitOfWork) : base(unitOfWork) { }

        #endregion Construtores

        #region Métodos Públicos

        public Dominio.Entidades.Embarcador.Documentos.ControleDocumento BuscarPorCodigo(int codigo)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Documentos.ControleDocumento>();

            var result = from obj in query select obj;
            result = result.Where(ent => ent.Codigo == codigo);

            return result.FirstOrDefault();
        }
        public List<Dominio.Entidades.Embarcador.Documentos.ControleDocumento> BuscarPorCodigo(List<int> codigo)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Documentos.ControleDocumento>();

            var result = from obj in query select obj;
            result = result.Where(ent => codigo.Contains(ent.Codigo));

            return result.ToList();
        }

        public Dominio.Entidades.Embarcador.Documentos.ControleDocumento BuscarPorCodigoCtE(int codigoCTE)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Documentos.ControleDocumento>();

            var result = from obj in query select obj;
            result = result.Where(ent => ent.CTe.Codigo == codigoCTE);

            return result.FirstOrDefault();
        }

        public Dominio.Entidades.Embarcador.Documentos.ControleDocumento BuscarPorCodigoCtESemCargaCte(int codigoCTE)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Documentos.ControleDocumento>();

            var result = from obj in query select obj;
            result = result.Where(ent => ent.CTe.Codigo == codigoCTE && ent.CargaCTe == null);

            return result.FirstOrDefault();
        }

        public Dominio.Entidades.Embarcador.Documentos.ControleDocumento BuscarPorCodigoCargaCtE(int codigoCargaCte)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Documentos.ControleDocumento>();

            var result = from obj in query select obj;
            result = result.Where(ent => ent.CargaCTe.Codigo == codigoCargaCte);

            return result.FirstOrDefault();
        }

        public List<Dominio.Entidades.Embarcador.Documentos.ControleDocumento> BuscarPorCodigosCTes(List<int> codigosCTes)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Documentos.ControleDocumento>();

            var result = from obj in query select obj;
            result = result.Where(ent => codigosCTes.Contains(ent.CTe.Codigo));

            return result.ToList();
        }

        public bool ExistePorCTe(int codigoCTE)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Documentos.ControleDocumento>();

            var result = from obj in query select obj;
            result = result.Where(ent => ent.CTe.Codigo == codigoCTE);

            return result.Any();
        }

        public Dominio.Entidades.Embarcador.Documentos.ControleDocumento ExisteDocumentoComIrreguralidade(TipoIrregularidade tipo, int codigoDocumento)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Documentos.ControleDocumento>();

            var result = from obj in query select obj;
            result = result.Where(ent => ent.CTe.Codigo == codigoDocumento && ent.MotivoIrregularidade.Irregularidade.TipoIrregularidade == tipo);

            return result.FirstOrDefault();
        }

        //public List<Dominio.ObjetosDeValor.Embarcador.Documentos.ControleDocumento> Consultar(Dominio.ObjetosDeValor.Embarcador.Documentos.FiltroPesquisaControleDocumento filtrosPesquisa, Dominio.ObjetosDeValor.Embarcador.Consulta.ParametroConsulta parametrosConsulta)
        //{
        //    var consulta = Consultar(filtrosPesquisa);

        //    if (parametrosConsulta.LimiteRegistros > 0)
        //        consulta = consulta.OrderBy($"{parametrosConsulta.PropriedadeOrdenar} {(parametrosConsulta.DirecaoOrdenar == "acs" ? "ascending" : "descending")}").Skip(parametrosConsulta.InicioRegistros).Take(parametrosConsulta.LimiteRegistros);

        //    return consulta.Select(x => new Dominio.ObjetosDeValor.Embarcador.Documentos.ControleDocumento()
        //    {
        //        Codigo = x.Codigo,
        //        CodigoCargaCTe = x.CargaCTe != null ? x.CargaCTe.Codigo : 0,
        //        MotivoParqueamento = x.MotivoParqueamento,
        //        Numero = x.CTe.Numero,
        //        Serie = x.CTe.Serie != null ? x.CTe.Serie.Numero : 0,
        //        Carga = x.CargaCTe != null && x.CargaCTe.Carga  != null? x.CargaCTe.Carga.CodigoCargaEmbarcador : "",
        //        CodigoCarga = x.CargaCTe != null && x.CargaCTe.Carga != null ? x.CargaCTe.Carga.Codigo : 0,
        //        //NFes = x.CTe.XMLNotaFiscais.Count > 0 ? : "",
        //        Situacao = ((int)x.SituacaoControleDocumento),
        //        Transportador = x.CTe.Empresa != null ? x.CTe.Empresa.RazaoSocial : "",
        //        Analise = x.Analise,
        //        PossuiPreCTe = x.CargaCTe != null && x.CargaCTe.PreCTe != null

        //    }).ToList();
        //}

        public IList<Dominio.ObjetosDeValor.Embarcador.Documentos.ControleDocumento> ConsultarControleDocumento(Dominio.ObjetosDeValor.Embarcador.Documentos.FiltroPesquisaControleDocumento filtrosPesquisa, Dominio.ObjetosDeValor.Embarcador.Consulta.ParametroConsulta parametrosConsulta)
        {
            var consulta = this.SessionNHiBernate.CreateSQLQuery(ObterConsultaControleDocumento(filtrosPesquisa, false, parametrosConsulta));

            consulta.SetResultTransformer(new NHibernate.Transform.AliasToBeanResultTransformer(typeof(Dominio.ObjetosDeValor.Embarcador.Documentos.ControleDocumento)));

            return consulta.SetTimeout(120).List<Dominio.ObjetosDeValor.Embarcador.Documentos.ControleDocumento>();
        }

        public int ContarConsultaControleDocumento(Dominio.ObjetosDeValor.Embarcador.Documentos.FiltroPesquisaControleDocumento filtrosPesquisa)
        {
            var consulta = this.SessionNHiBernate.CreateSQLQuery(ObterConsultaControleDocumento(filtrosPesquisa, true, null));

            return consulta.SetTimeout(600).UniqueResult<int>();
        }


        public int ContarConsulta(Dominio.ObjetosDeValor.Embarcador.Documentos.FiltroPesquisaControleDocumento filtrosPesquisa)
        {
            var consulta = Consultar(filtrosPesquisa);

            return consulta.Count();
        }

        public List<Dominio.Entidades.Embarcador.Documentos.ControleDocumento> Consultar(Dominio.ObjetosDeValor.Embarcador.Escrituracao.LoteEscrituracaoMiroFiltro filtrosPesquisa, Dominio.ObjetosDeValor.Embarcador.Consulta.ParametroConsulta parametrosConsulta)
        {
            var consulta = Consultar(filtrosPesquisa);

            return ObterLista(consulta, parametrosConsulta);
        }

        public int ContarConsulta(Dominio.ObjetosDeValor.Embarcador.Escrituracao.LoteEscrituracaoMiroFiltro filtroPesquisa)
        {
            var consulta = Consultar(filtroPesquisa);

            return consulta.Count();
        }

        public List<(int, int, SituacaoControleDocumento)> PesquisarDocumentoPelosCte(List<int> codigoCte)
        {
            var consulta = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Documentos.ControleDocumento>();
            consulta = from obj in consulta where codigoCte.Contains(obj.CTe.Codigo) select obj;
            return consulta.Select(x => ValueTuple.Create(x.Codigo, x.CTe.Codigo, x.SituacaoControleDocumento)).ToList();
        }

        public List<(int, int, int)> PesquisarIrregularidadesDocumentoPelosCte(List<int> codigoCte)
        {
            var consulta = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Documentos.ControleDocumento>();
            consulta = from obj in consulta where codigoCte.Contains(obj.CTe.Codigo) && obj.MotivoIrregularidade != null && obj.MotivoIrregularidade.Irregularidade != null select obj;
            return consulta.Select(x => ValueTuple.Create(x.Codigo, x.CTe.Codigo, x.MotivoIrregularidade.Irregularidade.Codigo)).ToList();
        }

        public IList<Dominio.ObjetosDeValor.Embarcador.Documentos.PendenciasControleDocumento> BuscarPendenciasControleDocumento(List<int> codigoIrregularidades, List<int> codigoPortfolio,List<int> setores)
        {

            var sqlSubConsulta = $@"SELECT TOP 1 
                                                irregularidade.IRR_GATILHO_IRREGULARIDADE
                                            FROM 
                                                T_HISTORICO_IRREGULARRIDADE AS historicoIrregularidade
                                                LEFT JOIN T_IRREGULARIDADE irregularidade ON irregularidade.IRR_CODIGO = historicoIrregularidade.IRR_CODIGO
                                            WHERE 
                                                historicoIrregularidade.HII_SITUACAO_IRREGULARIDADE = 3 
                                                AND historicoIrregularidade.COD_CODIGO = _controleDocumento.COD_CODIGO ";

            if (codigoIrregularidades.Count > 0)
                sqlSubConsulta += $" AND irregularidade.IRR_CODIGO IN ({string.Join(", ", codigoIrregularidades)}) ";

            if(codigoPortfolio.Count > 0)
                sqlSubConsulta += $" AND irregularidade.PMC_CODIGO IN ({string.Join(", ", codigoPortfolio)}) ";

            if(setores.Count > 0)
                sqlSubConsulta += $" AND historicoIrregularidade.SET_CODIGO IN ({string.Join(", ", setores)}) ";
           
            var sql = $@"SELECT 
                                Gatilho,
                                ISNULL(SET_CODIGO,0) as CodigoSetor,
	                            COUNT(*) AS Quantidade
                            FROM (
                                SELECT (
                                          {sqlSubConsulta}
                                            ORDER BY 
                                                irregularidade.IRR_SEQUENCIA DESC, irregularidade.IRR_CODIGO ASC
                                        ) AS Gatilho,
                                    historicoIrregularidade.SET_CODIGO
                                    FROM 
                                        T_CONTROLE_DOCUMENTO _controleDocumento
                                        JOIN T_HISTORICO_IRREGULARRIDADE historicoIrregularidade ON _controleDocumento.COD_CODIGO = historicoIrregularidade.COD_CODIGO
                                    WHERE 
                                        _controleDocumento.COD_SITUACAO_CONTROLE_DOCUMENTO = 4
                            ) AS Subquery
                            WHERE Gatilho IS NOT NULL
                            GROUP BY 
                                Gatilho, SET_CODIGO;";

            var consultaPendencias = this.SessionNHiBernate.CreateSQLQuery(sql);

            consultaPendencias.SetResultTransformer(new NHibernate.Transform.AliasToBeanResultTransformer(typeof(Dominio.ObjetosDeValor.Embarcador.Documentos.PendenciasControleDocumento)));

            return consultaPendencias.SetTimeout(7000).List<Dominio.ObjetosDeValor.Embarcador.Documentos.PendenciasControleDocumento>();
        }

        public List<Dominio.Entidades.Embarcador.Documentos.ControleDocumento> BuscarControleDocumentosPendentesVerificacoes(int limite)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Documentos.ControleDocumento>();
            query = query.Where(x => x.SituacaoVerificacao == SituacaoVerificacaoControleDocumento.AgVerificacao);
            query = query.Where(x => x.SituacaoControleDocumento != SituacaoControleDocumento.ParqueadoManualmente && x.SituacaoControleDocumento != SituacaoControleDocumento.ParqueadoAutomaticamente);

            return query.Take(limite).Fetch(x => x.CargaCTe).ToList();
        }

        #endregion

        #region Métodos Privados

        private IQueryable<Dominio.Entidades.Embarcador.Documentos.ControleDocumento> Consultar(Dominio.ObjetosDeValor.Embarcador.Documentos.FiltroPesquisaControleDocumento filtrosPesquisa)
        {
            var consulta = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Documentos.ControleDocumento>();
            var consultaIrregularidade = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Documentos.HistoricoIrregularidade>();

            consulta = from obj in consulta select obj;

            if (filtrosPesquisa.CodigoFilial > 0)
                consulta = consulta.Where(o => o.Carga.Filial.Codigo == filtrosPesquisa.CodigoFilial);

            if (filtrosPesquisa.SituacaoControleDocumento.Count > 0)
                consulta = consulta.Where(o => filtrosPesquisa.SituacaoControleDocumento.Contains(o.SituacaoControleDocumento));

            if (filtrosPesquisa.Numero > 0)
                consulta = consulta.Where(o => o.CTe.Numero == filtrosPesquisa.Numero);

            if (filtrosPesquisa.Serie > 0)
                consulta = consulta.Where(o => o.CTe.Serie.Numero == filtrosPesquisa.Serie);

            if (filtrosPesquisa.CodigoCarga > 0)
                consulta = consulta.Where(o => o.CargaCTe.Carga.Codigo == filtrosPesquisa.CodigoCarga);

            if (filtrosPesquisa.NFe > 0)
                consulta = consulta.Where(o => o.CTe.XMLNotaFiscais.Any(nfx => nfx.Numero == filtrosPesquisa.NFe));

            if (filtrosPesquisa.CodigoTransportador > 0)
                consulta = consulta.Where(o => o.CTe.Empresa.Codigo == filtrosPesquisa.CodigoTransportador);

            if (filtrosPesquisa.CodigosPortfolio.Count > 0)
                consultaIrregularidade = consultaIrregularidade.Where(irregularidade => irregularidade.SituacaoIrregularidade == SituacaoIrregularidade.AguardandoAprovacao && filtrosPesquisa.CodigosPortfolio.Contains(irregularidade.Porfolio.Codigo));

            if (filtrosPesquisa.CodigosIrregularidade.Count > 0)
                consultaIrregularidade = consultaIrregularidade.Where(irregularidade => irregularidade.SituacaoIrregularidade == SituacaoIrregularidade.AguardandoAprovacao && filtrosPesquisa.CodigosIrregularidade.Contains(irregularidade.Irregularidade.Codigo));

            if (filtrosPesquisa.CodigosSetor.Count > 0)
                consultaIrregularidade = consultaIrregularidade.Where(irregularidade => irregularidade.SituacaoIrregularidade == SituacaoIrregularidade.AguardandoAprovacao && filtrosPesquisa.CodigosSetor.Contains(irregularidade.Setor.Codigo));


            //if (filtrosPesquisa.CodigoFilial > 0)
            //    consulta = consulta.Where(o => o.Setor.Codigo == filtrosPesquisa.CodigoSetor);

            //if (filtrosPesquisa.CodigoFilial > 0)
            //    consulta = consulta.Where(o => o.Usuarios.Codigo == filtrosPesquisa.CodigoUsuario);


            if (filtrosPesquisa.DataEmissaoInicial != DateTime.MinValue)
                consulta = consulta.Where(o => o.CTe.DataEmissao >= filtrosPesquisa.DataEmissaoInicial);

            if (filtrosPesquisa.DataEmissaoFinal != DateTime.MinValue)
                consulta = consulta.Where(o => o.CTe.DataEmissao <= filtrosPesquisa.DataEmissaoFinal);

            // Mais Implementações futuras...
            //if (filtrosPesquisa.DataGeracaoIrregularidadeInicial != DateTime.MinValue)
            //    consulta = consulta.Where(o => o.Irregularidade.DataGeracao >= filtrosPesquisa.DataGeracaoIrregularidadeInicial);

            //if (filtrosPesquisa.DataGeracaoIrregularidadeFinal != DateTime.MinValue)
            //    consulta = consulta.Where(o => o.Irregularidade.DataGeracao <= filtrosPesquisa.DataGeracaoIrregularidadeFinal);

            if (filtrosPesquisa.CodigosIrregularidade.Count > 0 || filtrosPesquisa.CodigosPortfolio.Count > 0 || filtrosPesquisa.CodigosSetor.Count > 0)
                consulta = consulta.Where(o => consultaIrregularidade.Any(a => a.ControleDocumento.Codigo == o.Codigo));

            return consulta;
        }


        private IQueryable<Dominio.Entidades.Embarcador.Documentos.ControleDocumento> Consultar(Dominio.ObjetosDeValor.Embarcador.Escrituracao.LoteEscrituracaoMiroFiltro filtrosPesquisa)
        {
            var consulta = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Documentos.ControleDocumento>();
            consulta = from obj in consulta select obj;


            //if (filtrosPesquisa.Situacao != SituacaoLoteEscrituracaoMiro.Todos)
            //    consulta = consulta.Where(o => filtrosPesquisa.Situacao );

            //if (filtrosPesquisa.Numero > 0)
            //    consulta = consulta.Where(o => o.CTe.Numero == filtrosPesquisa.Numero);

            //if (filtrosPesquisa.Carga > 0)
            //    consulta = consulta.Where(o => o.CargaCTe.Carga.Codigo == filtrosPesquisa.CodigoCarga);

            //if (filtrosPesquisa.CodigoTransportador > 0)
            //    consulta = consulta.Where(o => o.CTe.Empresa.Codigo == filtrosPesquisa.CodigoTransportador);


            //if (filtrosPesquisa.DataEmissaoInicial != DateTime.MinValue)
            //    consulta = consulta.Where(o => o.CTe.DataEmissao >= filtrosPesquisa.DataEmissaoInicial);

            //if (filtrosPesquisa.DataEmissaoFinal != DateTime.MinValue)
            //    consulta = consulta.Where(o => o.CTe.DataEmissao <= filtrosPesquisa.DataEmissaoFinal);


            return consulta;
        }

        private string ObterConsultaControleDocumento(Dominio.ObjetosDeValor.Embarcador.Documentos.FiltroPesquisaControleDocumento filtrosPesquisa, bool somenteContar, Dominio.ObjetosDeValor.Embarcador.Consulta.ParametroConsulta parametrosConsulta = null)
        {
            string sql;
            string pattern = "yyyy-MM-dd";
            if (somenteContar)
                sql = @"select count(0) ";
            else
                sql = @"select
                        TMP.COD_CODIGO Codigo,
		                TMP.CCT_CODIGO CodigoCargaCTe,
		                TMP.COD_MOTIVO_PARQUEAMENTO MotivoParqueamento,
		                TMP.CON_NUM Numero,
		                TMP.ESE_NUMERO Serie,
		                TMP.CAR_CODIGO_CARGA_EMBARCADOR Carga,
		                TMP.CAR_CODIGO CodigoCarga,
		                TMP.COD_SITUACAO_CONTROLE_DOCUMENTO Situacao,
		                TMP.EMP_RAZAO Transportador,
		                TMP.COD_ANALISE Analise,
		                TMP.PossuiPreCTeInt,
		                TMP.CodigoHistorico,
		                TMP.NFes,
		                TMP.COD_DATA_ENVIO_APROVACAO DataEnvioAprovacao,
	                    Portfolio.PMC_DESCRICAO Portfolio,
                        Irregularidade.IRR_CODIGO CodigoIrregularidade,
		                Irregularidade.IRR_DESCRICAO Irregularidade,
		                HistoricoIrregularidade.HII_DATA_IRREGULARIDADE DataGeracaoIrregularidade,
		                Setor.SET_CODIGO CodigoSetor,
		                Setor.SET_DESCRICAO Setor,
		                HistoricoIrregularidade.HII_SERVICO_RESPONSAVEL ServicoResponsavel,
		                HistoricoIrregularidade.HII_SEQUENCIA_TRATATIVA SequenciaTratativa,
                        TMP.TipoDocumentoEmissaoInt,
                        TMP.CON_DATAHORAEMISSAO DataGeracaoDocumento,
                        TMP.MOD_DESCRICAO ModeloDocumentoFiscal,
                        TMP.COD_SITUACAO_APROVACAO_CARTA_CORRECAO SituacaoCCeInt,
                        TMP.COD_MOTIVO_REJEICAO_CCE MotivoRejeicaoCCe,
		                coalesce((
		
		                SUBSTRING((
                                                SELECT DISTINCT '|' + Cast(TratativaAcao.IRT_ACAO as varchar)
                                                        from T_IRREGULARIDADE_TRATATIVA_ACAO  TratativaAcao
                                                        left join T_IRREGULARIDADE_DEFINICAO_TRATATIVAS IrregularidadeDefinicaoTratativa on Irregularidade.IRR_CODIGO = IrregularidadeDefinicaoTratativa.IRR_CODIGO
                                                        left join T_IRREGULARIDADE_TRATATIVA IrregularidadeTratativa on IrregularidadeDefinicaoTratativa.IDT_CODIGO = IrregularidadeTratativa.IDT_CODIGO and IRT_SEQUENCIA = HII_SEQUENCIA_TRATATIVA
                                                    WHERE TratativaAcao.IRT_CODIGO = IrregularidadeTratativa.IRT_CODIGO for xml path('')), 2, 1000) 
		
		
		                ), '') Tratativas
                        
";

            sql += @" from(
                select
                        ControleDocumento.COD_CODIGO,
                        ControleDocumento.COD_DATA_ENVIO_APROVACAO,
		                CargaCTe.CCT_CODIGO,
		                ControleDocumento.COD_MOTIVO_PARQUEAMENTO,
		                CTe.CON_NUM,
		                CTe.CON_CODIGO,
		                EmpresaSerie.ESE_NUMERO,
		                Carga.CAR_CODIGO_CARGA_EMBARCADOR,
		                Carga.CAR_CODIGO,
		                Carga.FIL_CODIGO,
	                    Carga.EMP_CODIGO as CodigoEmpresaCarga,
		                ControleDocumento.COD_SITUACAO_CONTROLE_DOCUMENTO,
		                Empresa.EMP_RAZAO,
		                ControleDocumento.COD_ANALISE,
                        Empresa.EMP_CODIGO,
                        CTe.CON_DATAHORAEMISSAO,
                        ModeloDocumentoFiscal.MOD_CODIGO,
                        ModeloDocumentoFiscal.MOD_DESCRICAO,
                        ControleDocumento.COD_SITUACAO_APROVACAO_CARTA_CORRECAO,
                        ControleDocumento.COD_MOTIVO_REJEICAO_CCE,
		                coalesce((
			                select top 1 HII_CODIGO from T_HISTORICO_IRREGULARRIDADE Hist
				                inner join T_IRREGULARIDADE IRR on IRR.IRR_CODIGO = Hist.IRR_CODIGO
				                where Hist.COD_CODIGO = ControleDocumento.COD_CODIGO
				                and Hist.HII_SITUACAO_IRREGULARIDADE = 3
			                order by Irr.IRR_SEQUENCIA, HII_CODIGO desc
			                ), 
			                (			
			                select top 1 HII_CODIGO from T_HISTORICO_IRREGULARRIDADE Hist
				                inner join T_IRREGULARIDADE IRR on IRR.IRR_CODIGO = Hist.IRR_CODIGO
				                where Hist.COD_CODIGO = ControleDocumento.COD_CODIGO
				                and Hist.HII_SITUACAO_IRREGULARIDADE not in (1, 3)
				                order by Irr.IRR_SEQUENCIA, HII_CODIGO desc),
			                0) CodigoHistorico,
		                CASE
                            WHEN PreCTe.PCO_CODIGO IS NOT NULL and PreCTe.PCO_CODIGO > 0 THEN 1
                            ELSE 0
                        END AS PossuiPreCTeInt,
						SUBSTRING((SELECT ', ' + CAST(cteDocs.NFC_NUMERO AS NVARCHAR(20))
                                                                                    FROM T_CTE_DOCS cteDocs
                                                                                    WHERE cteDocs.CON_CODIGO = CTe.CON_CODIGO FOR XML PATH('')), 3, 1000) NFes,
                        Coalesce((
                            Select MOD_TIPO_DOCUMENTO_EMISSAO from T_MODDOCFISCAL ModeloDocumento where ModeloDocumento.MOD_CODIGO = CTe.CON_MODELODOC
                            ),0) TipoDocumentoEmissaoInt
                    from
                        T_CONTROLE_DOCUMENTO ControleDocumento 
                    left join
                        T_CARGA_CTE CargaCTe 
                            on ControleDocumento.CCT_CODIGO = CargaCTe.CCT_CODIGO 
                    left join
                        T_CARGA Carga 
                            on CargaCTe.CAR_CODIGO = Carga.CAR_CODIGO 
                    left join
                        T_PRE_CTE PreCTe 
                            on CargaCTe.PCO_CODIGO = PreCTe.PCO_CODIGO 
                    left join
                        T_CTE CTe 
                            on CTe.CON_CODIGO = coalesce(CargaCTe.CON_CODIGO, ControleDocumento.CON_CODIGO)
                    left join
                        T_EMPRESA_SERIE EmpresaSerie 
                            on CTe.CON_SERIE=EmpresaSerie.ESE_CODIGO 
                    left join
                        T_EMPRESA Empresa 
                            on CTe.EMP_CODIGO = Empresa.EMP_CODIGO 
	                left join
                        T_EMPRESA DocumentosCTe 
                            on CTe.EMP_CODIGO = Empresa.EMP_CODIGO
                    left join
                        T_MODDOCFISCAL ModeloDocumentoFiscal
                            on ModeloDocumentoFiscal.MOD_CODIGO = CTe.CON_MODELODOC
	                group by ControleDocumento.COD_CODIGO, CargaCTe.CCT_CODIGO,
		                ControleDocumento.COD_MOTIVO_PARQUEAMENTO,
		                CTe.CON_NUM,
		                EmpresaSerie.ESE_NUMERO,
		                Carga.CAR_CODIGO_CARGA_EMBARCADOR,
		                Carga.CAR_CODIGO,
		                ControleDocumento.COD_SITUACAO_CONTROLE_DOCUMENTO,
		                Empresa.EMP_RAZAO,
		                ControleDocumento.COD_ANALISE,
		                PreCTe.PCO_CODIGO,
                        Carga.FIL_CODIGO,
                        ControleDocumento.COD_DATA_ENVIO_APROVACAO,
                        Empresa.EMP_CODIGO,
                        CTe.CON_CODIGO,
                        CTe.CON_DATAHORAEMISSAO,
                        CTe.CON_CODIGO,
                        Carga.EMP_CODIGO,
                        CTe.CON_MODELODOC,
                        Carga.EMP_CODIGO,
                        ModeloDocumentoFiscal.MOD_CODIGO,
		                ModeloDocumentoFiscal.MOD_DESCRICAO,
                        ControleDocumento.COD_SITUACAO_APROVACAO_CARTA_CORRECAO,
                        ControleDocumento.COD_MOTIVO_REJEICAO_CCE
                ) TMP
                left join T_HISTORICO_IRREGULARRIDADE HistoricoIrregularidade on HistoricoIrregularidade.HII_CODIGO = TMP.CodigoHistorico
                left join T_PORTFOLIO_MODULO_CONTROLE Portfolio on HistoricoIrregularidade.PMC_CODIGO = Portfolio.PMC_CODIGO
                left join T_IRREGULARIDADE Irregularidade on HistoricoIrregularidade.IRR_CODIGO = Irregularidade.IRR_CODIGO
                left join T_SETOR Setor on HistoricoIrregularidade.SET_CODIGO = Setor.SET_CODIGO
                
            
";

            sql += @" where 1=1 ";

            if (filtrosPesquisa.CodigoModeloDocumentoFiscal > 0)
                sql += $"and TMP.MOD_CODIGO = {filtrosPesquisa.CodigoModeloDocumentoFiscal} ";

            if (filtrosPesquisa.CodigoCarga > 0)
                sql += $"and TMP.CAR_CODIGO = {filtrosPesquisa.CodigoCarga} ";

            if (filtrosPesquisa.CodigoFilial > 0)
                sql += $"and TMP.FIL_CODIGO = {filtrosPesquisa.CodigoFilial} ";

            if (filtrosPesquisa.SituacaoControleDocumento.Count > 0)
                sql += $"and TMP.COD_SITUACAO_CONTROLE_DOCUMENTO in ({string.Join(", ", filtrosPesquisa.SituacaoControleDocumento.Select(obj => (int)obj).ToList())}) ";

            if (filtrosPesquisa.Numero > 0)
                sql += $"and TMP.CON_NUM = {filtrosPesquisa.Numero} ";

            if (filtrosPesquisa.Serie > 0)
                sql += $"and TMP.ESE_NUMERO = {filtrosPesquisa.Serie} ";

            if (filtrosPesquisa.CodigoTransportador > 0)
                sql += $"and TMP.EMP_CODIGO = {filtrosPesquisa.CodigoTransportador} ";
            else if (filtrosPesquisa.Empresa > 0)
                sql += $"and TMP.EMP_CODIGO = {filtrosPesquisa.Empresa} ";

            if (filtrosPesquisa.DataEmissaoInicial != DateTime.MinValue)
                sql += $"and TMP.CON_DATAHORAEMISSAO >= '{filtrosPesquisa.DataEmissaoInicial.ToString(pattern)}' ";

            if (filtrosPesquisa.DataGeracaoIrregularidadeInicial != DateTime.MinValue)
                sql += $"and HistoricoIrregularidade.HII_DATA_IRREGULARIDADE >= '{filtrosPesquisa.DataGeracaoIrregularidadeInicial.ToString(pattern)}' ";

            if (filtrosPesquisa.DataEmissaoFinal != DateTime.MinValue)
                sql += $"and TMP.CON_DATAHORAEMISSAO <= '{filtrosPesquisa.DataEmissaoFinal.ToString(pattern)}' ";

            if (filtrosPesquisa.DataGeracaoIrregularidadeFinal != DateTime.MinValue)
                sql += $"and HistoricoIrregularidade.HII_DATA_IRREGULARIDADE <= '{filtrosPesquisa.DataGeracaoIrregularidadeFinal.AddDays(1).ToString(pattern)}' ";

            if (filtrosPesquisa.CodigosPortfolio.Count > 0)
                sql += $"and HistoricoIrregularidade.PMC_CODIGO in ({string.Join(", ", filtrosPesquisa.CodigosPortfolio)}) ";

            if (filtrosPesquisa.CodigosIrregularidade.Count > 0)
                sql += $"and Irregularidade.IRR_CODIGO in ({string.Join(", ", filtrosPesquisa.CodigosIrregularidade)}) ";

            if (filtrosPesquisa.CodigosSetor.Count > 0)
                sql += $"and HistoricoIrregularidade.SET_CODIGO in ({string.Join(", ", filtrosPesquisa.CodigosSetor)}) ";

            if (filtrosPesquisa.NFe > 0)
                sql += $"and exists (select CON_CODIGO from T_CTE_DOCS docs where docs.CON_CODIGO = TMP.CON_CODIGO and docs.NFC_NUMERO = {filtrosPesquisa.NFe}) "; // SQL-INJECTION-SAFE

            if (filtrosPesquisa.TransportadorLogado)
                sql += $"and HistoricoIrregularidade.HII_SERVICO_RESPONSAVEL = 2 ";

            if (parametrosConsulta != null)
            {
                if (parametrosConsulta.PropriedadeOrdenar == "DiasEmAprovacao")
                    sql += $" order by DataEnvioAprovacao {(parametrosConsulta.DirecaoOrdenar == "asc" ? "asc" : "desc")}, Situacao ";
                else
                    sql += $" order by {parametrosConsulta.PropriedadeOrdenar} {(parametrosConsulta.DirecaoOrdenar == "asc" ? "asc" : "desc")} "; 

                if ((parametrosConsulta.InicioRegistros > 0) || (parametrosConsulta.LimiteRegistros > 0))
                    sql += $" offset {parametrosConsulta.InicioRegistros} rows fetch next {parametrosConsulta.LimiteRegistros} rows only;";
            }

            return sql;
        }


        #endregion
    }
}
