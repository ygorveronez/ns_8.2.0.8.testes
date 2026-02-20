using System.Collections.Generic;
using System.Linq;

namespace Repositorio.Embarcador.CTe
{
    public class CTeDocumentoOriginario : RepositorioBase<Dominio.Entidades.Embarcador.CTe.CTeDocumentoOriginario>
    {
        public CTeDocumentoOriginario(UnitOfWork unitOfWork) : base(unitOfWork) { }

        public List<Dominio.Entidades.Embarcador.CTe.CTeDocumentoOriginario> BuscarPorCTe(int codigoCTe)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.CTe.CTeDocumentoOriginario>();

            query = query.Where(o => o.CTe.Codigo == codigoCTe);

            return query.ToList();
        }

        public Dominio.Entidades.Embarcador.CTe.CTeDocumentoOriginario BuscarPrimeiroPorCTe(int codigoCTe)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.CTe.CTeDocumentoOriginario>();

            query = query.Where(o => o.CTe.Codigo == codigoCTe);

            return query.FirstOrDefault();
        }

        public IList<Dominio.Relatorios.Embarcador.DataSource.SAC.Documento> DadosConhecimento(int codigoConhecimento)
        {
            string query = @"   SELECT C.CON_CODIGO Codigo,
                   C.CON_NUM Numero,
                   S.ESE_NUMERO Serie,
                   CASE 
		            WHEN C.CON_TIPO_CTE = 0 THEN 'Normal'
		            WHEN C.CON_TIPO_CTE = 1 THEN 'Complemento'
		            WHEN C.CON_TIPO_CTE = 2 THEN 'Anulacao'
		            ELSE 'Substituto'
	               END TipoFrete,
                   C.CON_DATA_AUTORIZACAO DataEmissao,
                   (SELECT TOP(1) CA.CAR_DATA_FINALIZACAO_EMISSAO 
				    FROM T_CARGA_CTE CC
				    JOIN T_CARGA CA ON CA.CAR_CODIGO = CC.CAR_CODIGO
		            WHERE CC.CON_CODIGO = C.CON_CODIGO
					ORDER BY CA.CAR_CODIGO DESC) DataEmbarque,
                   CAST(SUBSTRING((SELECT DISTINCT ', ' + CAST(M.MDF_NUMERO AS NVARCHAR(2000)) FROM  T_MDFE_MUNICIPIO_DESCARREGAMENTO_DOC DOC 
		            JOIN T_MDFE_MUNICIPIO_DESCARREGAMENTO MM ON MM.MDD_CODIGO = DOC.MDD_CODIGO
		            JOIN T_MDFE M ON M.MDF_CODIGO = MM.MDF_CODIGO
		            WHERE M.MDF_STATUS <> 7 AND M.MDF_STATUS <> 9 AND DOC.CON_CODIGO = C.CON_CODIGO FOR XML PATH('')), 3, 2000) AS VARCHAR(2000)) MDFes,
                   CAST(SUBSTRING((SELECT DISTINCT ', ' + CA.CAR_CODIGO_CARGA_EMBARCADOR FROM T_CARGA_CTE CC
		            JOIN T_CARGA CA ON CA.CAR_CODIGO = CC.CAR_CODIGO
		            WHERE CC.CON_CODIGO = C.CON_CODIGO FOR XML PATH('')), 3, 2000) AS VARCHAR(2000)) Cargas,
                   C.CON_DATAHORAEMISSAO DataDigitacao,
                   CAST(SUBSTRING((SELECT DISTINCT ', ' + CV.CVE_PLACA FROM T_CTE_VEICULO CV
		            WHERE CV.CON_CODIGO = C.CON_CODIGO FOR XML PATH('')), 3, 2000) AS VARCHAR(2000)) Placas,
                   CAST(SUBSTRING((SELECT DISTINCT ', ' + V.VEI_NUMERO_FROTA FROM T_CTE_VEICULO CV
		            JOIN T_VEICULO V ON V.VEI_CODIGO = CV.VEI_CODIGO
		            WHERE CV.CON_CODIGO = C.CON_CODIGO FOR XML PATH('')), 3, 2000) AS VARCHAR(2000)) Frotas,
                   C.CON_DATAPREVISTAENTREGA DataPrevisaoEntrega,
                   CAST(SUBSTRING((SELECT DISTINCT ', ' + T.TOP_DESCRICAO FROM T_CARGA_CTE CC
		            JOIN T_CARGA CA ON CA.CAR_CODIGO = CC.CAR_CODIGO
		            JOIN T_TIPO_OPERACAO T ON T.TOP_CODIGO = CA.TOP_CODIGO
		            WHERE CC.CON_CODIGO = C.CON_CODIGO FOR XML PATH('')), 3, 2000) AS VARCHAR(2000)) TiposOperacoes,

                   REM.PCT_NOME Remetente,
                   REM.PCT_ENDERECO EnderecoRemetente,
                   REM.PCT_NUMERO NumeroRemetente,
                   REM.PCT_COMPLEMENTO ComplementoRemetente,
                   REM.PCT_BAIRRO BairroRemetente,
                   LREM.LOC_DESCRICAO CidadeRemetente,
                   LREM.UF_SIGLA UFRemetente,
                   REM.PCT_CPF_CNPJ CNPJRemetente,
                   REM.PCT_IERG IERemetente,

                   DEST.PCT_NOME Destinatario,
                   DEST.PCT_ENDERECO EnderecoDestinatario,
                   DEST.PCT_NUMERO NumeroDestinatario,
                   DEST.PCT_COMPLEMENTO ComplementoDestinatario,
                   DEST.PCT_BAIRRO BairroDestinatario,
                   LDEST.LOC_DESCRICAO CidadeDestinatario,
                   LDEST.UF_SIGLA UFDestinatario,
                   DEST.PCT_CPF_CNPJ CNPJDestinatario,
                   DEST.PCT_IERG IEDestinatario,

	               C.CON_VALOR_FRETE ValorFrete, 
	               C.CON_VALOR_RECEBER ValorReceber, 
	               C.CON_VALOR_TOTAL_MERC ValorMercadoria,

                   C.CON_OBSGERAIS Observacao
            FROM T_CTE C
            JOIN T_EMPRESA_SERIE S ON S.ESE_CODIGO = C.CON_SERIE
            JOIN T_CTE_PARTICIPANTE REM ON REM.PCT_CODIGO = C.CON_REMETENTE_CTE
            JOIN T_LOCALIDADES LREM ON LREM.LOC_CODIGO = REM.LOC_CODIGO
            JOIN T_CTE_PARTICIPANTE DEST ON DEST.PCT_CODIGO = C.CON_DESTINATARIO_CTE
            JOIN T_LOCALIDADES LDEST ON LDEST.LOC_CODIGO = DEST.LOC_CODIGO 
            WHERE C.CON_CODIGO = " + codigoConhecimento.ToString();

            var nhQuery = this.SessionNHiBernate.CreateSQLQuery(query);

            nhQuery.SetResultTransformer(new NHibernate.Transform.AliasToBeanResultTransformer(typeof(Dominio.Relatorios.Embarcador.DataSource.SAC.Documento)));

            return nhQuery.List<Dominio.Relatorios.Embarcador.DataSource.SAC.Documento>();
        }

        public IList<Dominio.Relatorios.Embarcador.DataSource.SAC.ComponentesDocumento> ComponentesConhecimento(int codigoConhecimento)
        {
            string query = @"SELECT C.CPT_CODIGO Codigo,
                    C.CON_CODIGO CodigoCTe,
                    C.CPT_NOME Descricao,
                    C.CPT_VALOR Valor
            FROM T_CTE_COMP_PREST C
            WHERE C.CON_CODIGO = " + codigoConhecimento.ToString();

            var nhQuery = this.SessionNHiBernate.CreateSQLQuery(query);

            nhQuery.SetResultTransformer(new NHibernate.Transform.AliasToBeanResultTransformer(typeof(Dominio.Relatorios.Embarcador.DataSource.SAC.ComponentesDocumento)));

            return nhQuery.List<Dominio.Relatorios.Embarcador.DataSource.SAC.ComponentesDocumento>();
        }

        public IList<Dominio.Relatorios.Embarcador.DataSource.SAC.PesoDocumento> PesosConhecimento(int codigoConhecimento)
        {
            string query = @"SELECT C.ICA_CODIGO Codigo,
                C.CON_CODIGO CodigoCTe,
                C.ICA_TIPO Descricao,
                C.ICA_QTD Valor 
                FROM T_CTE_INF_CARGA C
            WHERE C.CON_CODIGO = " + codigoConhecimento.ToString();

            var nhQuery = this.SessionNHiBernate.CreateSQLQuery(query);

            nhQuery.SetResultTransformer(new NHibernate.Transform.AliasToBeanResultTransformer(typeof(Dominio.Relatorios.Embarcador.DataSource.SAC.PesoDocumento)));

            return nhQuery.List<Dominio.Relatorios.Embarcador.DataSource.SAC.PesoDocumento>();
        }

        public IList<Dominio.Relatorios.Embarcador.DataSource.SAC.NotasFiscaisDocumento> NotasFiscaisConhecimento(int codigoConhecimento)
        {
            string query = @"SELECT C.NFC_CODIGO Codigo,
            C.CON_CODIGO CodigoCTe,
            C.NFC_NUMERO Numero,
            C.NFC_SERIE Serie,
            C.NFC_DATAEMISSAO DataEmissao,
            C.NFC_PESO Peso,
            C.NFC_VOLUME Volume,
            C.NFC_VALORPRODUTOS ValorMercadoria,
            '' Protocolo,
            '' DataLibera
            FROM T_CTE_DOCS C
            WHERE C.CON_CODIGO = " + codigoConhecimento.ToString();

            var nhQuery = this.SessionNHiBernate.CreateSQLQuery(query);

            nhQuery.SetResultTransformer(new NHibernate.Transform.AliasToBeanResultTransformer(typeof(Dominio.Relatorios.Embarcador.DataSource.SAC.NotasFiscaisDocumento)));

            return nhQuery.List<Dominio.Relatorios.Embarcador.DataSource.SAC.NotasFiscaisDocumento>();
        }

        public IList<Dominio.Relatorios.Embarcador.DataSource.SAC.FaturamentoDocumento> FaturasConhecimento(int codigoConhecimento)
        {
            string query = @"SELECT DF.DFA_CODIGO Codigo,
                   DF.CON_CODIGO CodigoCTe,
                   F.FAT_NUMERO Numero,
                   F.FAT_NUMERO_PRE_FATURA PreFatura,
                   CASE 
		            WHEN F.FAT_SITUACAO = 1 THEN 'Em Andamento'
		            WHEN F.FAT_SITUACAO = 2 THEN 'Fechado'
		            WHEN F.FAT_SITUACAO = 3 THEN 'Cancelado'
		            WHEN F.FAT_SITUACAO = 4 THEN 'Liquidado'
		            ELSE 'Em Fechamento'
	               END Situacao,
                   CAST(SUBSTRING((SELECT DISTINCT ', ' + CAST(CONVERT(VARCHAR(10), T.TIT_DATA_VENCIMENTO, 103) AS VARCHAR(20)) FROM T_TITULO T
		            JOIN T_FATURA_PARCELA FP ON FP.FAP_CODIGO = T.FAP_CODIGO
		            WHERE FP.FAT_CODIGO = F.FAT_CODIGO FOR XML PATH('')), 3, 2000) AS VARCHAR(2000)) DatasVencimentos,
                   CAST(SUBSTRING((SELECT DISTINCT ', ' + CASE WHEN T.TIT_STATUS = 1 THEN 'Aberto' WHEN T.TIT_STATUS = 3 THEN 'Quitado' WHEN T.TIT_STATUS = 4 THEN 'Cancelado' ELSE 'Em Negociacao' END FROM T_TITULO T
		            JOIN T_FATURA_PARCELA FP ON FP.FAP_CODIGO = T.FAP_CODIGO
		            WHERE FP.FAT_CODIGO = F.FAT_CODIGO FOR XML PATH('')), 3, 2000) AS VARCHAR(2000)) Status,
                   CAST(SUBSTRING((SELECT DISTINCT ', ' + CAST(T.TIT_CODIGO AS VARCHAR(20)) FROM T_TITULO T
		            JOIN T_FATURA_PARCELA FP ON FP.FAP_CODIGO = T.FAP_CODIGO
		            WHERE FP.FAT_CODIGO = F.FAT_CODIGO FOR XML PATH('')), 3, 2000) AS VARCHAR(2000)) Titulos,
                   F.FAT_TOTAL Valor
            FROM T_DOCUMENTO_FATURAMENTO DF
            JOIN T_FATURA_DOCUMENTO FD ON FD.DFA_CODIGO = DF.DFA_CODIGO
            JOIN T_FATURA F ON F.FAT_CODIGO = FD.FAT_CODIGO
            WHERE F.FAT_SITUACAO <> 3 AND DF.CON_CODIGO = " + codigoConhecimento.ToString();

            var nhQuery = this.SessionNHiBernate.CreateSQLQuery(query);

            nhQuery.SetResultTransformer(new NHibernate.Transform.AliasToBeanResultTransformer(typeof(Dominio.Relatorios.Embarcador.DataSource.SAC.FaturamentoDocumento)));

            return nhQuery.List<Dominio.Relatorios.Embarcador.DataSource.SAC.FaturamentoDocumento>();
        }

        public IList<Dominio.Relatorios.Embarcador.DataSource.SAC.OcorrenciasDocumento> OcorrenciasConhecimento(int codigoConhecimento)
        {
            string query = @"SELECT C.OOC_CODIGO Codigo,
                C.CON_CODIGO CodigoCTe,
                R.OCO_DESCRICAO TipoOcorrencia,
                O.COC_OBSERVACAO Observacao,
                O.COC_DATA_OCORRENCIA DataOcorrencia
                 FROM T_CARGA_OCORRENCIA_DOCUMENTO C
                JOIN T_CARGA_OCORRENCIA O ON O.COC_CODIGO = C.COC_CODIGO
                JOIN T_OCORRENCIA R ON R.OCO_CODIGO = O.OCO_CODIGO
            WHERE C.CON_CODIGO = " + codigoConhecimento.ToString();

            var nhQuery = this.SessionNHiBernate.CreateSQLQuery(query);

            nhQuery.SetResultTransformer(new NHibernate.Transform.AliasToBeanResultTransformer(typeof(Dominio.Relatorios.Embarcador.DataSource.SAC.OcorrenciasDocumento)));

            return nhQuery.List<Dominio.Relatorios.Embarcador.DataSource.SAC.OcorrenciasDocumento>();
        }
    }
}
