using System.Collections.Generic;
using System.Linq;
using System.Text;
using NHibernate.Linq;

namespace Repositorio.Embarcador.Integracao
{
    public class MercadoLivreHandlingUnitDetail : RepositorioBase<Dominio.Entidades.Embarcador.Integracao.MercadoLivreHandlingUnitDetail>
    {
        public MercadoLivreHandlingUnitDetail(UnitOfWork unitOfWork) : base(unitOfWork) { }

        public List<Dominio.Entidades.Embarcador.Integracao.MercadoLivreHandlingUnitDetail> BuscarPorHandlingUnit(int codigoMercadoLivreHandlingUnit)
        {
            IQueryable<Dominio.Entidades.Embarcador.Integracao.MercadoLivreHandlingUnitDetail> query = SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Integracao.MercadoLivreHandlingUnitDetail>();

            query = query.Where(o => o.HandlingUnit.Codigo == codigoMercadoLivreHandlingUnit);

            return query.ToList();
        }

        public List<Dominio.Entidades.Embarcador.Integracao.MercadoLivreHandlingUnitDetail> BuscarNFePendenteDownloadPorHandlingUnit(int codigoMercadoLivreHandlingUnit)
        {
            IQueryable<Dominio.Entidades.Embarcador.Integracao.MercadoLivreHandlingUnitDetail> query = SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Integracao.MercadoLivreHandlingUnitDetail>();

            query = query.Where (
                    o => o.HandlingUnit.Codigo == codigoMercadoLivreHandlingUnit &&
                    o.TipoDocumento == Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoDocumentoMercadoLivreHandlingUnit.NotaFiscal &&
                    o.Situacao == Dominio.ObjetosDeValor.Embarcador.Enumeradores.MercadoLivreHandlingUnitDetailSituacao.Pend_Download &&
                    o.DetailRegistroPai != null
                );

            return query
                .Fetch(o => o.DetailRegistroPai)
                .ToList();
        }

        public Dominio.Entidades.Embarcador.Integracao.MercadoLivreHandlingUnitDetail BuscarPorHandlingUnitEShipmentID(int codigoMercadoLivreHandlingUnit, long shipmentID)
        {
            IQueryable<Dominio.Entidades.Embarcador.Integracao.MercadoLivreHandlingUnitDetail> query = SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Integracao.MercadoLivreHandlingUnitDetail>();

            query = query.Where(o => o.HandlingUnit.Codigo == codigoMercadoLivreHandlingUnit && o.ShipmentID == shipmentID);

            return query.FirstOrDefault();
        }

        public Dominio.Entidades.Embarcador.Integracao.MercadoLivreHandlingUnitDetail BuscarPorHandlingUnitEChaveAcesso(int codigoMercadoLivreHandlingUnit, string chaveAcesso)
        {
            IQueryable<Dominio.Entidades.Embarcador.Integracao.MercadoLivreHandlingUnitDetail> query = SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Integracao.MercadoLivreHandlingUnitDetail>();

            query = query.Where(o => o.HandlingUnit.Codigo == codigoMercadoLivreHandlingUnit && o.ChaveAcesso == chaveAcesso);

            return query.FirstOrDefault();
        }

        public void RemoverCTesVinculados(int codigoHandlingUnit)
        {
            // DELETE FROM CargaPreviaDocumentoDocumento WHERE Codigo IN (SELECT c.Codigo FROM CargaPreviaDocumentoDocumento c WHERE c.CargaPreviaDocumento.Carga.Codigo = :codigoCarga)

            if (UnitOfWork.IsActiveTransaction())
            {
                UnitOfWork.Sessao.CreateQuery("UPDATE MercadoLivreHandlingUnitDetailArquivo SET PedidoCTeParaSubcontratacao = null, PedidoXMLNotaFiscal = null WHERE HandlingUnitDetail IN (SELECT c FROM MercadoLivreHandlingUnitDetail c WHERE c.HandlingUnit.Codigo = :codigoHandlingUnit)")
                                 .SetInt32("codigoHandlingUnit", codigoHandlingUnit)
                                 .ExecuteUpdate();
            }
            else
            {
                try
                {
                    UnitOfWork.Start();

                    UnitOfWork.Sessao.CreateQuery("UPDATE MercadoLivreHandlingUnitDetailArquivo SET PedidoCTeParaSubcontratacao = null, PedidoXMLNotaFiscal = null WHERE HandlingUnitDetail IN (SELECT c FROM MercadoLivreHandlingUnitDetail c WHERE c.HandlingUnit.Codigo = :codigoHandlingUnit)")
                                        .SetInt32("codigoHandlingUnit", codigoHandlingUnit)
                                        .ExecuteUpdate();

                    UnitOfWork.CommitChanges();
                }
                catch
                {
                    UnitOfWork.Rollback();
                    throw;
                }
            }
        }

        public void RemoverCTesVinculadosPorCodigo(int CodigoCTe)
        {
            UnitOfWork.Sessao.CreateQuery("UPDATE MercadoLivreHandlingUnitDetailArquivo SET PedidoCTeParaSubcontratacao = null WHERE PedidoCTeParaSubcontratacao.Codigo = :CodigoCTe AND PedidoCTeParaSubcontratacao.Codigo > 0")
                             .SetInt32("CodigoCTe", CodigoCTe)
                             .ExecuteUpdate();
        }

        public void RemoverNotasFiscaisVinculadas(List<int> lstCodigosNotasFiscais)
        {
             
            UnitOfWork.Sessao.CreateQuery($@"UPDATE MercadoLivreHandlingUnitDetailArquivo SET PedidoXMLNotaFiscal = null WHERE PedidoXMLNotaFiscal.Codigo IN ({string.Join(", ", lstCodigosNotasFiscais)}) AND PedidoXMLNotaFiscal.Codigo > 0")
                             .ExecuteUpdate();
        }

        public int ContarConsultarMercadoLivreHandlingUnitDetailSQL(Dominio.ObjetosDeValor.Embarcador.Integracao.MercadoLivre.FiltroPesquisaMercadoLivreHandlingUnitDetail filtrosPesquisa)
        {
            var consulta = this.SessionNHiBernate.CreateSQLQuery(QueryConsultaMercadoLivreHandlingUnitDetail(filtrosPesquisa, true));

            return consulta.SetTimeout(600).UniqueResult<int>();
        }

        public IList<Dominio.ObjetosDeValor.Embarcador.Integracao.MercadoLivre.ConsultaMercadoLivreHandlingUnitDetail> ConsultarMercadoLivreHandlingUnitDetailSQL(Dominio.ObjetosDeValor.Embarcador.Integracao.MercadoLivre.FiltroPesquisaMercadoLivreHandlingUnitDetail filtrosPesquisa, string propOrdenacao, string dirOrdenacao, int inicioRegistros, int maximoRegistros)
        {
            var consulta = this.SessionNHiBernate.CreateSQLQuery(QueryConsultaMercadoLivreHandlingUnitDetail(filtrosPesquisa, false, propOrdenacao, dirOrdenacao, inicioRegistros, maximoRegistros));

            consulta.SetResultTransformer(new NHibernate.Transform.AliasToBeanResultTransformer(typeof(Dominio.ObjetosDeValor.Embarcador.Integracao.MercadoLivre.ConsultaMercadoLivreHandlingUnitDetail)));

            return consulta.SetTimeout(600).List<Dominio.ObjetosDeValor.Embarcador.Integracao.MercadoLivre.ConsultaMercadoLivreHandlingUnitDetail>();
        }

        private string QueryConsultaMercadoLivreHandlingUnitDetail(Dominio.ObjetosDeValor.Embarcador.Integracao.MercadoLivre.FiltroPesquisaMercadoLivreHandlingUnitDetail filtroPesquisa, bool somenteContarNumeroRegistros, string propOrdenacao = null, string dirOrdenacao = null, int? inicioRegistros = null, int? maximoRegistros = null)
        {
            System.Globalization.CultureInfo cultura = new System.Globalization.CultureInfo("en-US");
            string pattern = "yyyy-MM-dd";

            StringBuilder select = new StringBuilder();
            StringBuilder joins = new StringBuilder();
            StringBuilder where = new StringBuilder();

            where.Append($" where cargaIntegracaoMercadoLivre.CIM_CODIGO = {filtroPesquisa.CodigoCargaMercadoLivre} ");
            where.Append($" AND detail.MUD_CODIGO_PAI IS NULL ");

            if (filtroPesquisa.SituacaoIntegracaoMercadoLivre != Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoIntegracaoMercadoLivre.Todas)
            {
                switch(filtroPesquisa.SituacaoIntegracaoMercadoLivre)
                {
                    case Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoIntegracaoMercadoLivre.PendenteDownload:
                        where.Append($@" AND (detail.MUD_SITUACAO = 0 OR
		                                     ((detail.MUD_SITUACAO IS NULL AND
			                                     (detailArquivo.PSC_CODIGO IS NULL AND detailArquivo.PNF_CODIGO IS NULL)
	                                     AND detailArquivo.MUD_CODIGO IS NULL)))");
                        break;
                    case Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoIntegracaoMercadoLivre.PendenteProcessamento:
                        where.Append(@$" AND (detail.MUD_SITUACAO = 1 OR
		                                     ((detail.MUD_SITUACAO IS NULL AND
			                                     (detailArquivo.PSC_CODIGO IS NULL AND detailArquivo.PNF_CODIGO IS NULL)
	                                     AND detailArquivo.MUD_CODIGO IS NOT NULL)))");
                        break;
                    case Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoIntegracaoMercadoLivre.Concluido:
                        where.Append(@$" AND (detail.MUD_SITUACAO = 2 OR
		                                     ((detail.MUD_SITUACAO IS NULL AND
			                                     (detailArquivo.PSC_CODIGO IS NOT NULL OR detailArquivo.PNF_CODIGO IS NOT NULL))))");
                        break;
                    case Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoIntegracaoMercadoLivre.Desconsiderado:
                        where.Append($" AND detail.MUD_SITUACAO = 3");
                        break;
                    default:
                        break;
                }
            }

            if (filtroPesquisa.ExibirApenasDocumentosComMensagemErro)
                where.Append($" AND detail.MUD_MENSAGEM IS NOT NULL");

            if (somenteContarNumeroRegistros)
                select.Append("select distinct(count(0) over ()) ");
            else
                select.Append($@"select
                             detail.MUD_CODIGO AS Codigo,
                             cargaIntegracaoMercadoLivre.CIM_CODIGO AS CodigoCargaIntegracaoMercadoLivre,
                             CASE WHEN detail.MUD_SITUACAO IS NULL THEN
	                             CASE WHEN detailArquivo.PSC_CODIGO IS NOT NULL OR detailArquivo.PNF_CODIGO IS NOT NULL THEN 
		                            'Concluído' 
		                         ELSE
			                        CASE WHEN detailArquivo.MUD_CODIGO IS NOT NULL THEN
				                        'Pend. Processamento'
			                        ELSE
				                        'Pend. Download'
			                        END
                                 END
                             ELSE
                                 CASE detail.MUD_SITUACAO
                                    WHEN 0 THEN 'Pend. Download'
                                    WHEN 1 THEN 'Pend. Processamento'
                                    WHEN 2 THEN 'Concluído'
                                    WHEN 3 THEN 'Desconsiderado'
                                 END
		                     END AS Situacao,
	                         CASE WHEN detail.MUD_TIPO_DOCUMENTO = 0 THEN 'CT-e' ELSE 'NF-e' END AS TipoDoDocumento,
	                         detail.MUD_CHAVE_ACESSO AS ChaveDeAcesso,
                             detail.MUD_MENSAGEM AS Mensagem,
	                         ISNULL(ISNULL(ISNULL(cteTerceiro.CPS_VALOR_TOTAL_MERC_ORIGINAL, cteTerceiroDocumentoAdicional.CAD_VALOR_TOTAL_MERC),notaFiscal.NF_VALOR),0) AS ValorDeMercadoria");

            joins.Append($@"  from T_CARGA_INTEGRACAO_MERCADO_LIVRE cargaIntegracaoMercadoLivre
                         inner join T_MERCADO_LIVRE_HANDLING_UNIT_DETAIL detail ON detail.MHU_CODIGO = cargaIntegracaoMercadoLivre.MHU_CODIGO
                         left join T_MERCADO_LIVRE_HANDLING_UNIT_DETAIL_ARQUIVO detailArquivo ON detailArquivo.MUD_CODIGO = detail.MUD_CODIGO
                         left join T_PEDIDO_CTE_PARA_SUB_CONTRATACAO pedidoCteSubContratacao ON detail.MUD_TIPO_DOCUMENTO = 0 AND pedidoCteSubContratacao.PSC_CODIGO = detailArquivo.PSC_CODIGO
                         left join T_CTE_TERCEIRO cteTerceiro ON detail.MUD_TIPO_DOCUMENTO = 0 AND cteTerceiro.CPS_CODIGO = pedidoCteSubContratacao.CPS_CODIGO AND cteTerceiro.CPS_CHAVE_ACESSO = detail.MUD_CHAVE_ACESSO
                         left join T_CTE_TERCEIRO_DOCUMENTO_ADICIONAL cteTerceiroDocumentoAdicional ON detail.MUD_TIPO_DOCUMENTO = 0 AND cteTerceiroDocumentoAdicional.CPS_CODIGO = pedidoCteSubContratacao.CPS_CODIGO AND cteTerceiroDocumentoAdicional.CAD_CHAVE = detail.MUD_CHAVE_ACESSO
                         left join T_PEDIDO_XML_NOTA_FISCAL pedidoNotaFiscal ON detail.MUD_TIPO_DOCUMENTO = 1 AND pedidoNotaFiscal.PNF_CODIGO = detailArquivo.PNF_CODIGO
                         left join T_XML_NOTA_FISCAL notaFiscal ON detail.MUD_TIPO_DOCUMENTO = 1 AND notaFiscal.NFX_CODIGO = pedidoNotaFiscal.NFX_CODIGO");

            string sql = select.Append(joins).Append(where).ToString();

            if (!somenteContarNumeroRegistros && !string.IsNullOrWhiteSpace(propOrdenacao))
            {
                sql += $" order by {propOrdenacao} {dirOrdenacao}";

                if ((inicioRegistros > 0) || (maximoRegistros > 0))
                    sql += $" offset {inicioRegistros} rows fetch next {maximoRegistros} rows only;";
            }

            return sql;
        }
    }
}
