using System;
using System.Collections.Generic;
using System.Linq;

namespace Repositorio.Embarcador.Contabeis
{
    public class ConsultarValores : RepositorioBase<Dominio.Entidades.Embarcador.CTe.ControleGeracaoMovimentoCTeManual>
    {
        public ConsultarValores(UnitOfWork unitOfWork) : base(unitOfWork) { }

        public IList<Dominio.Relatorios.Embarcador.DataSource.Contabeis.ConsultaValores> ConsultaValores(DateTime dataInicial, DateTime dataFinal, Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoConsultarValores tipo)
        {
            string query = "";
            if (tipo == Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoConsultarValores.CTesEmitidos)
            {
                query = @"SELECT ISNULL(SUM(C.CON_VALOR_RECEBER), 0) Valor
                    FROM T_CTE C
                    LEFT OUTER JOIN T_CARGA_CTE CC ON CC.CON_CODIGO = C.CON_CODIGO
                    LEFT OUTER JOIN T_CARGA CA ON CA.CAR_CODIGO = CC.CAR_CODIGO
                    LEFT OUTER JOIN T_FATURA F ON F.FAT_CODIGO = C.FAT_CODIGO 
                    WHERE CC.CCT_CODIGO IS NOT NULL AND (CA.CAR_CARGA_TRANSBORDO IS NULL OR CA.CAR_CARGA_TRANSBORDO = 0) ";
                if (dataFinal > DateTime.MinValue && dataInicial > DateTime.MinValue)
                    query += " AND C.CON_DATA_AUTORIZACAO >= '" + dataInicial.ToString("yyyy-MM-dd") + "' AND C.CON_DATA_AUTORIZACAO < '" + dataFinal.AddDays(1).ToString("yyyy-MM-dd") + "' ";
                else if (dataFinal > DateTime.MinValue)
                    query += " AND C.CON_DATA_AUTORIZACAO < '" + dataFinal.AddDays(1).ToString("yyyy-MM-dd") + "' ";
                else if (dataInicial > DateTime.MinValue)
                    query += " AND C.CON_DATA_AUTORIZACAO >= '" + dataInicial.ToString("yyyy-MM-dd") + "' ";
            }
            else if (tipo == Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoConsultarValores.CTesCancelados)
            {
                query = @"SELECT SUM(C.CON_VALOR_RECEBER) Valor
                    FROM T_CTE C
                    LEFT OUTER JOIN T_CARGA_CTE CC ON CC.CON_CODIGO = C.CON_CODIGO
                    LEFT OUTER JOIN T_CARGA CA ON CA.CAR_CODIGO = CC.CAR_CODIGO
                    LEFT OUTER JOIN T_FATURA F ON F.FAT_CODIGO = C.FAT_CODIGO 
                    WHERE CC.CCT_CODIGO IS NOT NULL AND (CA.CAR_CARGA_TRANSBORDO IS NULL OR CA.CAR_CARGA_TRANSBORDO = 0)  ";

                if (dataFinal > DateTime.MinValue && dataInicial > DateTime.MinValue)
                    query += " AND C.CON_DATA_cancelamento >= '" + dataInicial.ToString("yyyy-MM-dd") + "' AND C.CON_DATA_cancelamento < '" + dataFinal.AddDays(1).ToString("yyyy-MM-dd") + "' ";
                else if (dataFinal > DateTime.MinValue)
                    query += " AND C.CON_DATA_cancelamento < '" + dataFinal.AddDays(1).ToString("yyyy-MM-dd") + "' ";
                else if (dataInicial > DateTime.MinValue)
                    query += " AND C.CON_DATA_cancelamento >= '" + dataInicial.ToString("yyyy-MM-dd") + "' ";
            }
            else if (tipo == Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoConsultarValores.CTesAnulados)
            {
                query = @"SELECT ISNULL(SUM(C.CON_VALOR_RECEBER), 0) Valor
                    FROM T_CTE C
                    LEFT OUTER JOIN T_CARGA_CTE CC ON CC.CON_CODIGO = C.CON_CODIGO
                    LEFT OUTER JOIN T_CARGA CA ON CA.CAR_CODIGO = CC.CAR_CODIGO
                    LEFT OUTER JOIN T_FATURA F ON F.FAT_CODIGO = C.FAT_CODIGO 
                    WHERE CC.CCT_CODIGO IS NOT NULL AND (CA.CAR_CARGA_TRANSBORDO IS NULL OR CA.CAR_CARGA_TRANSBORDO = 0)   ";

                if (dataFinal > DateTime.MinValue && dataInicial > DateTime.MinValue)
                    query += " AND C.CON_DATA_ANULACAO   >= '" + dataInicial.ToString("yyyy-MM-dd") + "' AND C.CON_DATA_ANULACAO   < '" + dataFinal.AddDays(1).ToString("yyyy-MM-dd") + "' ";
                else if (dataFinal > DateTime.MinValue)
                    query += " AND C.CON_DATA_ANULACAO   < '" + dataFinal.AddDays(1).ToString("yyyy-MM-dd") + "' ";
                else if (dataInicial > DateTime.MinValue)
                    query += " AND C.CON_DATA_ANULACAO   >= '" + dataInicial.ToString("yyyy-MM-dd") + "' ";
            }
            else if (tipo == Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoConsultarValores.CTesFatudados)
            {
                query = @"SELECT ISNULL(SUM(C.CON_VALOR_RECEBER), 0) Valor
                    FROM T_CTE C
                    LEFT OUTER JOIN T_CARGA_CTE CC ON CC.CON_CODIGO = C.CON_CODIGO
                    LEFT OUTER JOIN T_CARGA CA ON CA.CAR_CODIGO = CC.CAR_CODIGO
                    JOIN T_FATURA F ON F.FAT_CODIGO = C.FAT_CODIGO 
                    WHERE (CA.CAR_CARGA_TRANSBORDO IS NULL OR CA.CAR_CARGA_TRANSBORDO = 0) AND F.FAT_SITUACAO <> 3 AND C.FAT_CODIGO IS NOT NULL ";

                if (dataFinal > DateTime.MinValue && dataInicial > DateTime.MinValue)
                    query += " AND F.FAT_DATA_FATURA >= '" + dataInicial.ToString("yyyy-MM-dd") + "' AND F.FAT_DATA_FATURA < '" + dataFinal.AddDays(1).ToString("yyyy-MM-dd") + "' ";
                else if (dataFinal > DateTime.MinValue)
                    query += " AND F.FAT_DATA_FATURA < '" + dataFinal.AddDays(1).ToString("yyyy-MM-dd") + "' ";
                else if (dataInicial > DateTime.MinValue)
                    query += " AND F.FAT_DATA_FATURA >= '" + dataInicial.ToString("yyyy-MM-dd") + "' ";
            }
            else if (tipo == Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoConsultarValores.PosicaoCTes)
            {
                query = @"SELECT ISNULL(SUM(C.CON_VALOR_RECEBER), 0) Valor
                    FROM T_CTE C
                    LEFT OUTER JOIN T_CARGA_CTE CC ON CC.CON_CODIGO = C.CON_CODIGO
                    LEFT OUTER JOIN T_CARGA CA ON CA.CAR_CODIGO = CC.CAR_CODIGO
                    LEFT OUTER JOIN T_FATURA F ON F.FAT_CODIGO = C.FAT_CODIGO 
                    WHERE (CA.CAR_CARGA_TRANSBORDO IS NULL OR CA.CAR_CARGA_TRANSBORDO = 0) ";

                if (dataFinal > DateTime.MinValue)
                {
                    query += @" AND ((C.FAT_CODIGO IS NULL OR F.FAT_DATA_FATURA >= '" + dataFinal.AddDays(1).ToString("yyyy-MM-dd") + @"') OR (F.FAT_SITUACAO = 1 AND (not F.FAT_DATA_FATURA >= '" + dataFinal.ToString("yyyy-MM-dd") + @"')))
                    AND C.CON_DATA_AUTORIZACAO < '" + dataFinal.AddDays(1).ToString("yyyy-MM-dd") + @"'
                    AND (CASE
                            WHEN C.CON_STATUS = 'C' AND C.CON_DATA_CANCELAMENTO >= '" + dataFinal.AddDays(1).ToString("yyyy-MM-dd") + @"' THEN 'A'
                            WHEN C.CON_STATUS = 'Z' AND C.CON_DATA_ANULACAO >= '" + dataFinal.AddDays(1).ToString("yyyy-MM-dd") + @"' THEN 'A'
                            WHEN C.CON_STATUS = 'A' AND C.CON_DATA_AUTORIZACAO >= '" + dataFinal.AddDays(1).ToString("yyyy-MM-dd") + @"' THEN 'S'
                            ELSE C.CON_STATUS
                        END) = 'A' ";
                    //query += @" AND (C.CON_CODIGO NOT IN (SELECT DISTINCT D.CON_CODIGO 
                    //FROM T_FATURA_CARGA_DOCUMENTO D 
                    //JOIN T_FATURA F ON D.FAT_CODIGO = F.FAT_CODIGO AND F.FAT_SITUACAO = 3
                    //JOIN T_FATURA_PARCELA FP ON FP.FAT_CODIGO = F.FAT_CODIGO
                    //JOIN T_TITULO T ON T.FAP_CODIGO = FP.FAP_CODIGO AND T.TIT_STATUS = 4
                    //WHERE D.FCD_STATUS_DOCUMENTO = 1 
                    //AND F.FAT_DATA_FATURA < '" + dataFinal.AddDays(1).ToString("yyyy-MM-dd") + @"'
                    //AND D.CON_CODIGO NOT IN (SELECT DISTINCT DD.CON_CODIGO 
                    //FROM T_FATURA_CARGA_DOCUMENTO DD 
                    //JOIN T_FATURA FF ON DD.FAT_CODIGO = FF.FAT_CODIGO AND FF.FAT_SITUACAO <> 3
                    //JOIN T_FATURA_PARCELA FFP ON FFP.FAT_CODIGO = FF.FAT_CODIGO
                    //JOIN T_TITULO TT ON TT.FAP_CODIGO = FFP.FAP_CODIGO AND TT.TIT_STATUS <> 4
                    //WHERE DD.FCD_STATUS_DOCUMENTO = 1 
                    //AND FF.FAT_DATA_FATURA < '" + dataFinal.AddDays(1).ToString("yyyy-MM-dd") + @"')))";
                }
            }


            var nhQuery = this.SessionNHiBernate.CreateSQLQuery(query);

            nhQuery.SetResultTransformer(new NHibernate.Transform.AliasToBeanResultTransformer(typeof(Dominio.Relatorios.Embarcador.DataSource.Contabeis.ConsultaValores)));

            return nhQuery.SetTimeout(60000).List<Dominio.Relatorios.Embarcador.DataSource.Contabeis.ConsultaValores>();
        }

        public decimal ObterValorCTesEmitidosSemCarga()
        {
            string sqlQuery = "select ISNULL(SUM(CTe.CON_VALOR_RECEBER), 0) from T_CTE CTe where CTe.CON_STATUS = 'A' AND CTe.CON_TIPO_CTE <> 2 and CTe.CON_CODIGO not in (select CON_CODIGO from T_CARGA_CTE);";

            NHibernate.ISQLQuery query = this.SessionNHiBernate.CreateSQLQuery(sqlQuery);

            return query.SetTimeout(600).UniqueResult<decimal>();
        }

        public decimal ObterValorCTesNaoFaturados()
        {
            //var sqlQuery = "select ISNULL((SUM(DocumentoFaturamento.DFA_VALOR_DOCUMENTO) + SUM(DocumentoFaturamento.DFA_VALOR_ACRESCIMO) - SUM(DocumentoFaturamento.DFA_VALOR_DESCONTO) - SUM(DocumentoFaturamento.DFA_VALOR_PAGO)), 0) from T_DOCUMENTO_FATURAMENTO DocumentoFaturamento where DocumentoFaturamento.DFA_SITUACAO = 1 and DocumentoFaturamento.DFA_VALOR_DOCUMENTO<>(DocumentoFaturamento.DFA_VALOR_PAGO - DocumentoFaturamento.DFA_VALOR_ACRESCIMO + DocumentoFaturamento.DFA_VALOR_DESCONTO) and not exists(SELECT TituloDocumento.CON_CODIGO, TituloDocumento.CAR_CODIGO FROM T_TITULO_DOCUMENTO TituloDocumento INNER JOIN T_TITULO Titulo ON TituloDocumento.TIT_CODIGO = Titulo.TIT_CODIGO WHERE Titulo.TIT_STATUS <> 4 AND(DocumentoFaturamento.CON_CODIGO is not null and TituloDocumento.CON_CODIGO = DocumentoFaturamento.CON_CODIGO) OR(DocumentoFaturamento.CAR_CODIGO is not null and CAR_CODIGO = DocumentoFaturamento.CAR_CODIGO))";

            string sqlQuery = "SELECT SUM(DocumentoFaturamento.DFA_VALOR_A_FATURAR) FROM T_DOCUMENTO_FATURAMENTO DocumentoFaturamento WHERE DocumentoFaturamento.DFA_SITUACAO = 1 AND DocumentoFaturamento.DFA_VALOR_A_FATURAR > 0";

            NHibernate.ISQLQuery query = this.SessionNHiBernate.CreateSQLQuery(sqlQuery);

            return query.SetTimeout(600).UniqueResult<decimal>();
        }

        public decimal ObterValorTitulosAReceberEmAberto()
        {
            //var sqlQuery = "select ISNULL((SUM(DocumentoFaturamento.DFA_VALOR_DOCUMENTO) + SUM(DocumentoFaturamento.DFA_VALOR_ACRESCIMO) - SUM(DocumentoFaturamento.DFA_VALOR_DESCONTO) - SUM(DocumentoFaturamento.DFA_VALOR_PAGO)), 0) from T_DOCUMENTO_FATURAMENTO DocumentoFaturamento where DocumentoFaturamento.DFA_SITUACAO = 1 and DocumentoFaturamento.DFA_VALOR_DOCUMENTO<>(DocumentoFaturamento.DFA_VALOR_PAGO - DocumentoFaturamento.DFA_VALOR_ACRESCIMO + DocumentoFaturamento.DFA_VALOR_DESCONTO) and exists(SELECT TituloDocumento.CON_CODIGO, TituloDocumento.CAR_CODIGO FROM T_TITULO_DOCUMENTO TituloDocumento INNER JOIN T_TITULO Titulo ON TituloDocumento.TIT_CODIGO = Titulo.TIT_CODIGO WHERE Titulo.TIT_STATUS <> 4 AND(DocumentoFaturamento.CON_CODIGO is not null and TituloDocumento.CON_CODIGO = DocumentoFaturamento.CON_CODIGO) OR(DocumentoFaturamento.CAR_CODIGO is not null and CAR_CODIGO = DocumentoFaturamento.CAR_CODIGO))";

            string sqlQuery = "SELECT SUM(CASE WHEN TituloDocumento.TDO_CODIGO IS NOT NULL THEN TituloDocumento.TDO_VALOR_TOTAL ELSE Titulo.TIT_VALOR_ORIGINAL END) FROM T_TITULO Titulo LEFT OUTER JOIN T_TITULO_DOCUMENTO TituloDocumento ON TituloDocumento.TIT_CODIGO = Titulo.TIT_CODIGO WHERE Titulo.TIT_TIPO = 1 AND Titulo.TIT_STATUS = 1 AND (TituloDocumento.TDO_CODIGO IS NOT NULL OR Titulo.FAP_CODIGO IS NOT NULL)";

            NHibernate.ISQLQuery query = this.SessionNHiBernate.CreateSQLQuery(sqlQuery);

            return query.SetTimeout(600).UniqueResult<decimal>();
        }

        public decimal ObterValorTitulosAReceberEmAbertoOutros()
        {
            //var sqlQuery = "select ISNULL(SUM(T.TIT_VALOR_PENDENTE), 0) FROM T_TITULO T WHERE T.FAP_CODIGO IS NULL AND T.TIT_TIPO = 1 AND T.TIT_STATUS = 1;";

            string sqlQuery = "SELECT SUM(Titulo.TIT_VALOR_ORIGINAL) FROM T_TITULO Titulo LEFT OUTER JOIN T_TITULO_DOCUMENTO TituloDocumento ON TituloDocumento.TIT_CODIGO = Titulo.TIT_CODIGO WHERE Titulo.TIT_TIPO = 1 AND Titulo.TIT_STATUS = 1 AND TituloDocumento.TDO_CODIGO IS NULL AND Titulo.FAP_CODIGO IS NULL";

            NHibernate.ISQLQuery query = this.SessionNHiBernate.CreateSQLQuery(sqlQuery);

            return query.SetTimeout(600).UniqueResult<decimal>();
        }

        public decimal ObterValorFaturaEmAberto()
        {
            //var sqlQuery = "select ISNULL(SUM(T.TIT_VALOR_PENDENTE), 0) FROM T_TITULO T WHERE T.FAP_CODIGO IS NULL AND T.TIT_TIPO = 1 AND T.TIT_STATUS = 1;";

            string sqlQuery = "SELECT SUM(FaturaDocumento.FDO_VALOR_COBRAR) FROM T_FATURA Fatura INNER JOIN T_FATURA_DOCUMENTO FaturaDocumento ON Fatura.FAT_CODIGO = FaturaDocumento.FAT_CODIGO WHERE Fatura.FAT_SITUACAO = 1 and Fatura.FAT_NOVO_MODELO = 1";

            NHibernate.ISQLQuery query = this.SessionNHiBernate.CreateSQLQuery(sqlQuery);

            return query.SetTimeout(600).UniqueResult<decimal>();
        }

        public IList<Dominio.Relatorios.Embarcador.DataSource.Financeiros.ResumoContasAReceber> ConsultarRelatorioAnaliticoContasAReceber()
        {
            string sqlQuery = @"SELECT 
                                (CASE WHEN DocumentoFaturamento.DFA_CODIGO is not null THEN 'Título em Aberto' 
                                        WHEN Fatura.FAT_CODIGO is not null THEN 'Título em Aberto' 
	                                    ELSE 'Outros Títulos em Aberto' END) TipoSumarizacao,
                                (CASE WHEN DocumentoFaturamento.DFA_TIPO_DOCUMENTO = 1 THEN 'CT-e' 
                                        WHEN DocumentoFaturamento.DFA_TIPO_DOCUMENTO = 2 THEN 'Carga' 
	                                    WHEN Fatura.FAT_CODIGO is not null THEN 'Fatura' 
	                                    ELSE 'Outros' END) TipoDocumento, 
                                CASE WHEN TituloDocumento.TDO_CODIGO is not null THEN TituloDocumento.TDO_VALOR_TOTAL ELSE Titulo.TIT_VALOR_ORIGINAL END Valor, 
                                CASE WHEN TituloDocumento.TDO_CODIGO is not null THEN TituloDocumento.TDO_VALOR_PENDENTE ELSE Titulo.TIT_VALOR_PENDENTE END Saldo,
                                Convert(nvarchar(50), Fatura.FAT_NUMERO) Fatura,
                                CASE WHEN DocumentoFaturamento.DFA_CODIGO is not null THEN DocumentoFaturamento.DFA_DATAHORAEMISSAO
	                                    ELSE Titulo.TIT_DATA_EMISSAO END DataEmissao, 
                                CASE WHEN GrupoTomador.GRP_CODIGO is not null THEN GrupoTomador.GRP_DESCRICAO ELSE GrupoTomadorTitulo.GRP_DESCRICAO END GrupoPessoasTomador,
                                CASE WHEN Tomador.CLI_CGCCPF is not null THEN Tomador.CLI_NOME ELSE TomadorTitulo.CLI_NOME END NomeTomador, 
                                CASE WHEN Tomador.CLI_CGCCPF is not null THEN Tomador.CLI_CGCCPF ELSE TomadorTitulo.CLI_CGCCPF END CPFCNPJTomador, 
                                CASE WHEN Tomador.CLI_CGCCPF is not null THEN Tomador.CLI_FISJUR ELSE TomadorTitulo.CLI_FISJUR END TipoPessoaTomador, 
                                ModeloDocumento.MOD_ABREVIACAO ModeloDocumento, 
                                EmpresaSerie.ESE_NUMERO Serie, 
                                CONVERT(int, DocumentoFaturamento.DFA_NUMERO) NumeroDocumento,
                                Titulo.TIT_CODIGO NumeroTitulo,
                                DocumentoFaturamento.DFA_NUMERO_CARGA NumeroCarga,
                                LocalidadeOrigem.LOC_DESCRICAO Origem,
                                LocalidadeOrigem.UF_SIGLA UFOrigem,
                                LocalidadeDestino.LOC_DESCRICAO Destino,
                                LocalidadeDestino.UF_SIGLA UFDestino,
                                LocalidadeEmpresa.UF_SIGLA UFEmpresa,
                                substring((select CASE WHEN veiculo1.VEI_NUMERO_FROTA IS NULL THEN '' WHEN veiculo1.VEI_NUMERO_FROTA = '' THEN '' ELSE ', ' + veiculo1.VEI_NUMERO_FROTA END from T_DOCUMENTO_FATURAMENTO_VEICULO veiculoDocumento inner join T_VEICULO veiculo1 on veiculoDocumento.VEI_CODIGO = veiculo1.VEI_CODIGO where veiculoDocumento.DFA_CODIGO = DocumentoFaturamento.DFA_CODIGO for xml path('')), 3, 1000) Frotas,
                                substring((select ', ' + motorista1.FUN_NOME from T_DOCUMENTO_FATURAMENTO_MOTORISTA motoristaDocumento inner join T_FUNCIONARIO motorista1 on motoristaDocumento.FUN_CODIGO = motorista1.FUN_CODIGO where motoristaDocumento.DFA_CODIGO = DocumentoFaturamento.DFA_CODIGO for xml path('')), 3, 1000) Motoristas,
                                substring((select ', ' + veiculo1.VEI_PLACA from T_DOCUMENTO_FATURAMENTO_VEICULO veiculoDocumento inner join T_VEICULO veiculo1 on veiculoDocumento.VEI_CODIGO = veiculo1.VEI_CODIGO where veiculoDocumento.DFA_CODIGO = DocumentoFaturamento.DFA_CODIGO for xml path('')), 3, 1000) Placas,
                                DocumentoFaturamento.DFA_NUMERO_OCORRENCIA NumeroOcorrencia,
                                substring((select ', ' + NumeroOcorrencia.DFA_NUMERO_PEDIDO_OCORRENCIA from T_DOCUMENTO_FATURAMENTO_NUMERO_OCORRENCIA NumeroOcorrencia where NumeroOcorrencia.DFA_CODIGO = DocumentoFaturamento.DFA_CODIGO for xml path('')), 3, 1000) NumeroOcorrenciaCliente,
                                substring((select ', ' + NumeroPedido.DFA_NUMERO_PEDIDO from T_DOCUMENTO_FATURAMENTO_NUMERO_PEDIDO NumeroPedido where NumeroPedido.DFA_CODIGO = DocumentoFaturamento.DFA_CODIGO for xml path('')), 3, 1000) NumeroPedidoCliente,
                                convert(nvarchar(20), Titulo.TIT_DATA_VENCIMENTO, 103) DataVencimento,
                                convert(nvarchar(20), DocumentoFaturamento.DFA_DATA_ENVIO_ULTIMO_CANHOTO, 103) DataEnvioUltimoCanhoto,
                                convert(nvarchar(20), DocumentoFaturamento.DFA_DATA_CHEGADA_ULTIMO_CANHOTO, 103) DataChegadaUltimoCanhoto,
                                Remetente.CLI_NOME NomeRemetente, 
                                Remetente.CLI_CGCCPF CPFCNPJRemetente, 
                                Remetente.CLI_FISJUR TipoPessoaRemetente, 
                                Destinatario.CLI_NOME NomeDestinatario, 
                                Destinatario.CLI_CGCCPF CPFCNPJDestinatario, 
                                Destinatario.CLI_FISJUR TipoPessoaDestinatario,
                                CASE CTe.CON_TIPO_CTE WHEN 0 THEN 'Normal' WHEN 1 THEN 'Complemento' WHEN 2 THEN 'Anulação' WHEN 3 THEN 'Substituto' ELSE '' END TipoCTe,
                                substring((select ', ' + notaFiscal1.NFC_NUMERO from T_CTE_DOCS notaFiscal1 where notaFiscal1.CON_CODIGO = CTe.CON_CODIGO for xml path('')), 3, 1000) NumeroNotaFiscal,
                                CTe.CON_CHAVECTE ChaveAcessoCTe,
                                CTe.CON_OBSGERAIS ObservacaoCTe,
                                CTe.CON_VALOR_RECEBER ValorReceber,
                                Titulo.TIT_NUMERO_DOCUMENTO_TITULO_ORIGINAL NumeroDocumentoTituloOriginal
                                FROM T_TITULO Titulo
                                LEFT OUTER JOIN T_TITULO_DOCUMENTO TituloDocumento ON TituloDocumento.TIT_CODIGO = Titulo.TIT_CODIGO
                                LEFT OUTER JOIN T_DOCUMENTO_FATURAMENTO DocumentoFaturamento ON (TituloDocumento.CON_CODIGO is not null AND TituloDocumento.CON_CODIGO = DocumentoFaturamento.CON_CODIGO) OR (TituloDocumento.CAR_CODIGO is not null AND TituloDocumento.CAR_CODIGO = DocumentoFaturamento.CAR_CODIGO)
                                LEFT OUTER JOIN T_EMPRESA Empresa ON DocumentoFaturamento.EMP_CODIGO = Empresa.EMP_CODIGO
                                LEFT OUTER JOIN T_LOCALIDADES LocalidadeEmpresa ON Empresa.LOC_CODIGO = LocalidadeEmpresa.LOC_CODIGO
                                LEFT OUTER JOIN T_GRUPO_PESSOAS GrupoTomadorTitulo on GrupoTomadorTitulo.GRP_CODIGO = Titulo.GRP_CODIGO
                                LEFT OUTER JOIN T_CLIENTE TomadorTitulo on TomadorTitulo.CLI_CGCCPF = Titulo.CLI_CGCCPF 
                                LEFT OUTER JOIN T_GRUPO_PESSOAS GrupoTomador on GrupoTomador.GRP_CODIGO = DocumentoFaturamento.GRP_CODIGO
                                LEFT OUTER JOIN T_CLIENTE Tomador on Tomador.CLI_CGCCPF = DocumentoFaturamento.CLI_CODIGO_TOMADOR 
                                LEFT OUTER JOIN T_MODDOCFISCAL ModeloDocumento on DocumentoFaturamento.MOD_CODIGO = ModeloDocumento.MOD_CODIGO  
                                LEFT OUTER JOIN T_EMPRESA_SERIE EmpresaSerie on EmpresaSerie.ESE_CODIGO = DocumentoFaturamento.ESE_CODIGO
                                LEFT OUTER JOIN T_FATURA_PARCELA FaturaParcela ON FaturaParcela.FAP_CODIGO = Titulo.FAP_CODIGO 
                                LEFT OUTER JOIN T_FATURA Fatura ON Fatura.FAT_CODIGO = FaturaParcela.FAT_CODIGO
                                LEFT OUTER JOIN T_LOCALIDADES LocalidadeOrigem on LocalidadeOrigem.LOC_CODIGO = DocumentoFaturamento.LOC_ORIGEM
                                LEFT OUTER JOIN T_LOCALIDADES LocalidadeDestino on LocalidadeDestino.LOC_CODIGO = DocumentoFaturamento.LOC_DESTINO
                                LEFT OUTER JOIN T_CLIENTE Destinatario on Destinatario.CLI_CGCCPF = DocumentoFaturamento.CLI_CODIGO_DESTINATARIO
                                LEFT OUTER JOIN T_CLIENTE Remetente on Remetente.CLI_CGCCPF = DocumentoFaturamento.CLI_CODIGO_REMETENTE
                                LEFT OUTER JOIN T_CTE CTe on CTe.CON_CODIGO = DocumentoFaturamento.CON_CODIGO
                                WHERE Titulo.TIT_TIPO = 1 AND Titulo.TIT_STATUS = 1
                                UNION
                                SELECT 
                                'CT-e em Fatura' TipoSumarizacao,
                                (CASE WHEN DocumentoFaturamento.DFA_TIPO_DOCUMENTO = 1 THEN 'CT-e' 
                                        WHEN DocumentoFaturamento.DFA_TIPO_DOCUMENTO = 2 THEN 'Carga' END) TipoDocumento, 
                                FaturaDocumento.FDO_VALOR_COBRAR Valor, 
                                FaturaDocumento.FDO_VALOR_COBRAR Saldo,
                                Convert(nvarchar(50), Fatura.FAT_NUMERO) Fatura,
                                DocumentoFaturamento.DFA_DATAHORAEMISSAO DataEmissao, 
                                GrupoTomador.GRP_DESCRICAO GrupoPessoasTomador,
                                Tomador.CLI_NOME NomeTomador, 
                                Tomador.CLI_CGCCPF CPFCNPJTomador, 
                                Tomador.CLI_FISJUR TipoPessoaTomador, 
                                ModeloDocumento.MOD_ABREVIACAO ModeloDocumento, 
                                EmpresaSerie.ESE_NUMERO Serie, 
                                CONVERT(int, DocumentoFaturamento.DFA_NUMERO) NumeroDocumento,
                                null NumeroTitulo,
                                DocumentoFaturamento.DFA_NUMERO_CARGA NumeroCarga,
                                LocalidadeOrigem.LOC_DESCRICAO Origem,
                                LocalidadeOrigem.UF_SIGLA UFOrigem,
                                LocalidadeDestino.LOC_DESCRICAO Destino,
                                LocalidadeDestino.UF_SIGLA UFDestino,
                                LocalidadeEmpresa.UF_SIGLA UFEmpresa,
                                substring((select CASE WHEN veiculo1.VEI_NUMERO_FROTA IS NULL THEN '' WHEN veiculo1.VEI_NUMERO_FROTA = '' THEN '' ELSE ', ' + veiculo1.VEI_NUMERO_FROTA END from T_DOCUMENTO_FATURAMENTO_VEICULO veiculoDocumento inner join T_VEICULO veiculo1 on veiculoDocumento.VEI_CODIGO = veiculo1.VEI_CODIGO where veiculoDocumento.DFA_CODIGO = DocumentoFaturamento.DFA_CODIGO for xml path('')), 3, 1000) Frotas,
                                substring((select ', ' + motorista1.FUN_NOME from T_DOCUMENTO_FATURAMENTO_MOTORISTA motoristaDocumento inner join T_FUNCIONARIO motorista1 on motoristaDocumento.FUN_CODIGO = motorista1.FUN_CODIGO where motoristaDocumento.DFA_CODIGO = DocumentoFaturamento.DFA_CODIGO for xml path('')), 3, 1000) Motoristas,
                                substring((select ', ' + veiculo1.VEI_PLACA from T_DOCUMENTO_FATURAMENTO_VEICULO veiculoDocumento inner join T_VEICULO veiculo1 on veiculoDocumento.VEI_CODIGO = veiculo1.VEI_CODIGO where veiculoDocumento.DFA_CODIGO = DocumentoFaturamento.DFA_CODIGO for xml path('')), 3, 1000) Placas,
                                DocumentoFaturamento.DFA_NUMERO_OCORRENCIA NumeroOcorrencia,
                                substring((select ', ' + NumeroOcorrencia.DFA_NUMERO_PEDIDO_OCORRENCIA from T_DOCUMENTO_FATURAMENTO_NUMERO_OCORRENCIA NumeroOcorrencia where NumeroOcorrencia.DFA_CODIGO = DocumentoFaturamento.DFA_CODIGO for xml path('')), 3, 1000) NumeroOcorrenciaCliente,
                                substring((select ', ' + NumeroPedido.DFA_NUMERO_PEDIDO from T_DOCUMENTO_FATURAMENTO_NUMERO_PEDIDO NumeroPedido where NumeroPedido.DFA_CODIGO = DocumentoFaturamento.DFA_CODIGO for xml path('')), 3, 1000) NumeroPedidoCliente,
                                null DataVencimento,
                                convert(nvarchar(20), DocumentoFaturamento.DFA_DATA_ENVIO_ULTIMO_CANHOTO, 103) DataEnvioUltimoCanhoto,
                                convert(nvarchar(20), DocumentoFaturamento.DFA_DATA_CHEGADA_ULTIMO_CANHOTO, 103) DataChegadaUltimoCanhoto,
                                Remetente.CLI_NOME NomeRemetente, 
                                Remetente.CLI_CGCCPF CPFCNPJRemetente, 
                                Remetente.CLI_FISJUR TipoPessoaRemetente, 
                                Destinatario.CLI_NOME NomeDestinatario, 
                                Destinatario.CLI_CGCCPF CPFCNPJDestinatario, 
                                Destinatario.CLI_FISJUR TipoPessoaDestinatario,
                                CASE CTe.CON_TIPO_CTE WHEN 0 THEN 'Normal' WHEN 1 THEN 'Complemento' WHEN 2 THEN 'Anulação' WHEN 3 THEN 'Substituto' ELSE '' END TipoCTe,
                                substring((select ', ' + notaFiscal1.NFC_NUMERO from T_CTE_DOCS notaFiscal1 where notaFiscal1.CON_CODIGO = CTe.CON_CODIGO for xml path('')), 3, 1000) NumeroNotaFiscal,
                                CTe.CON_CHAVECTE ChaveAcessoCTe,
                                CTe.CON_OBSGERAIS ObservacaoCTe,
                                CTe.CON_VALOR_RECEBER ValorReceber,
                                null NumeroDocumentoTituloOriginal
                                FROM T_FATURA Fatura 
                                INNER JOIN T_FATURA_DOCUMENTO FaturaDocumento ON Fatura.FAT_CODIGO = FaturaDocumento.FAT_CODIGO
                                INNER JOIN T_DOCUMENTO_FATURAMENTO DocumentoFaturamento ON DocumentoFaturamento.DFA_CODIGO = FaturaDocumento.DFA_CODIGO
                                LEFT OUTER JOIN T_GRUPO_PESSOAS GrupoTomador on GrupoTomador.GRP_CODIGO = DocumentoFaturamento.GRP_CODIGO
                                LEFT OUTER JOIN T_CLIENTE Tomador on Tomador.CLI_CGCCPF = DocumentoFaturamento.CLI_CODIGO_TOMADOR 
                                LEFT OUTER JOIN T_MODDOCFISCAL ModeloDocumento on DocumentoFaturamento.MOD_CODIGO = ModeloDocumento.MOD_CODIGO  
                                LEFT OUTER JOIN T_EMPRESA_SERIE EmpresaSerie on EmpresaSerie.ESE_CODIGO = DocumentoFaturamento.ESE_CODIGO
                                LEFT OUTER JOIN T_LOCALIDADES LocalidadeOrigem on LocalidadeOrigem.LOC_CODIGO = DocumentoFaturamento.LOC_ORIGEM
                                LEFT OUTER JOIN T_LOCALIDADES LocalidadeDestino on LocalidadeDestino.LOC_CODIGO = DocumentoFaturamento.LOC_DESTINO
                                LEFT OUTER JOIN T_CLIENTE Destinatario on Destinatario.CLI_CGCCPF = DocumentoFaturamento.CLI_CODIGO_DESTINATARIO
                                LEFT OUTER JOIN T_CLIENTE Remetente on Remetente.CLI_CGCCPF = DocumentoFaturamento.CLI_CODIGO_REMETENTE
                                LEFT OUTER JOIN T_CTE CTe on CTe.CON_CODIGO = DocumentoFaturamento.CON_CODIGO
                                LEFT OUTER JOIN T_EMPRESA Empresa ON DocumentoFaturamento.EMP_CODIGO = Empresa.EMP_CODIGO
                                LEFT OUTER JOIN T_LOCALIDADES LocalidadeEmpresa ON Empresa.LOC_CODIGO = LocalidadeEmpresa.LOC_CODIGO
                                WHERE Fatura.FAT_SITUACAO = 1 and Fatura.FAT_NOVO_MODELO = 1
                                UNION
                                SELECT 'Não Vinculado à Carga' TipoSumarizacao, 
                                'CT-e' TipoDocumento, 
                                CTe.CON_VALOR_RECEBER Valor, 
                                CTe.CON_VALOR_RECEBER Saldo,
                                '' Fatura,
                                CTe.CON_DATAHORAEMISSAO DataEmissao,
                                GrupoTomadorPagadorCTe.GRP_DESCRICAO GrupoPessoasTomador, 
                                ClienteTomadorPagadorCTe.CLI_NOME NomeTomador, 
                                ClienteTomadorPagadorCTe.CLI_CGCCPF CPFCNPJTomador,  
                                ClienteTomadorPagadorCTe.CLI_FISJUR TipoPessoaTomador,
                                ModeloDocumento.MOD_ABREVIACAO ModeloDocumento, 
                                SerieCTe.ESE_NUMERO Serie, 
                                CTe.CON_NUM NumeroDocumento,
                                null NumeroTitulo,
                                null NumeroCarga,
                                LocalidadeOrigem.LOC_DESCRICAO Origem,
                                LocalidadeOrigem.UF_SIGLA UFOrigem,
                                LocalidadeDestino.LOC_DESCRICAO Destino,
                                LocalidadeDestino.UF_SIGLA UFDestino,
                                LocalidadeEmpresa.UF_SIGLA UFEmpresa,
                                null Frotas,
                                null Motoristas,
                                null Placas,
                                null NumeroOcorrencia,
                                null NumeroOcorrenciaCliente,
                                null NumeroPedidoCliente,
                                null DataVencimento,
                                null DataEnvioUltimoCanhoto,
                                null DataChegadaUltimoCanhoto,
                                CASE WHEN Expedidor.CLI_CGCCPF is not null THEN Expedidor.CLI_NOME ELSE Remetente.CLI_NOME END NomeRemetente, 
                                CASE WHEN Expedidor.CLI_CGCCPF is not null THEN Expedidor.CLI_CGCCPF ELSE Remetente.CLI_CGCCPF END CPFCNPJRemetente,
                                CASE WHEN Expedidor.CLI_CGCCPF is not null THEN Expedidor.CLI_FISJUR ELSE Remetente.CLI_FISJUR END TipoPessoaRemetente,
                                CASE WHEN Recebedor.CLI_CGCCPF is not null THEN Recebedor.CLI_NOME ELSE Destinatario.CLI_NOME END NomeDestinatario, 
                                CASE WHEN Recebedor.CLI_CGCCPF is not null THEN Recebedor.CLI_CGCCPF ELSE Destinatario.CLI_CGCCPF END CPFCNPJDestinatario,
                                CASE WHEN Recebedor.CLI_CGCCPF is not null THEN Recebedor.CLI_FISJUR ELSE Destinatario.CLI_FISJUR END TipoPessoaDestinatario,
                                CASE CTe.CON_TIPO_CTE WHEN 0 THEN 'Normal' WHEN 1 THEN 'Complemento' WHEN 2 THEN 'Anulação' WHEN 3 THEN 'Substituto' ELSE '' END TipoCTe,
                                substring((select ', ' + notaFiscal1.NFC_NUMERO from T_CTE_DOCS notaFiscal1 where notaFiscal1.CON_CODIGO = CTe.CON_CODIGO for xml path('')), 3, 1000) NumeroNotaFiscal,
                                CTe.CON_CHAVECTE ChaveAcessoCTe,
                                CTe.CON_OBSGERAIS ObservacaoCTe,
                                CTe.CON_VALOR_RECEBER ValorReceber,
                                null NumeroDocumentoTituloOriginal
                                FROM T_CTE CTe 
                                LEFT OUTER JOIN T_CTE_PARTICIPANTE TomadorPagadorCTe on CTe.CON_TOMADOR_PAGADOR_CTE = TomadorPagadorCTe.PCT_CODIGO 
                                LEFT OUTER JOIN T_CLIENTE ClienteTomadorPagadorCTe on ClienteTomadorPagadorCTe.CLI_CGCCPF = TomadorPagadorCTe.CLI_CODIGO 
                                LEFT OUTER JOIN T_GRUPO_PESSOAS GrupoTomadorPagadorCTe on GrupoTomadorPagadorCTe.GRP_CODIGO = ClienteTomadorPagadorCTe.GRP_CODIGO 
                                LEFT OUTER JOIN T_MODDOCFISCAL ModeloDocumento on CTe.CON_MODELODOC = ModeloDocumento.MOD_CODIGO 
                                LEFT OUTER JOIN T_EMPRESA_SERIE SerieCTe on CTe.CON_SERIE = SerieCTe.ESE_CODIGO  
                                LEFT OUTER JOIN T_LOCALIDADES LocalidadeOrigem on LocalidadeOrigem.LOC_CODIGO = CTe.CON_LOCINICIOPRESTACAO
                                LEFT OUTER JOIN T_LOCALIDADES LocalidadeDestino on LocalidadeDestino.LOC_CODIGO = CTe.CON_LOCTERMINOPRESTACAO
                                LEFT OUTER JOIN T_CTE_PARTICIPANTE ParticipanteDestinatarioCTe on CTe.CON_DESTINATARIO_CTE = ParticipanteDestinatarioCTe.PCT_CODIGO 
                                LEFT OUTER JOIN T_CLIENTE Destinatario on Destinatario.CLI_CGCCPF = ParticipanteDestinatarioCTe.CLI_CODIGO
                                LEFT OUTER JOIN T_CTE_PARTICIPANTE ParticipanteRemetenteCTe on CTe.CON_REMETENTE_CTE = ParticipanteRemetenteCTe.PCT_CODIGO 
                                LEFT OUTER JOIN T_CLIENTE Remetente on Remetente.CLI_CGCCPF = ParticipanteRemetenteCTe.CLI_CODIGO
                                LEFT OUTER JOIN T_CTE_PARTICIPANTE ParticipanteExpedidorCTe on CTe.CON_EXPEDIDOR_CTE = ParticipanteExpedidorCTe.PCT_CODIGO 
                                LEFT OUTER JOIN T_CLIENTE Expedidor on Expedidor.CLI_CGCCPF = ParticipanteExpedidorCTe.CLI_CODIGO
                                LEFT OUTER JOIN T_CTE_PARTICIPANTE ParticipanteRecebedorCTe on CTe.CON_RECEBEDOR_CTE = ParticipanteRecebedorCTe.PCT_CODIGO 
                                LEFT OUTER JOIN T_CLIENTE Recebedor on Recebedor.CLI_CGCCPF = ParticipanteRecebedorCTe.CLI_CODIGO
                                LEFT OUTER JOIN T_EMPRESA Empresa ON CTe.EMP_CODIGO = Empresa.EMP_CODIGO
                                LEFT OUTER JOIN T_LOCALIDADES LocalidadeEmpresa ON Empresa.LOC_CODIGO = LocalidadeEmpresa.LOC_CODIGO
                                WHERE CTe.CON_STATUS = 'A' and CTe.CON_TIPO_CTE <> 2 and CTe.CON_CODIGO not in (select CON_CODIGO from T_CARGA_CTE) 
                                UNION
                                SELECT 
                                'CT-e não Faturado' TipoSumarizacao,
                                (CASE WHEN DocumentoFaturamento.DFA_TIPO_DOCUMENTO = 1 THEN 'CT-e' 
                                        WHEN DocumentoFaturamento.DFA_TIPO_DOCUMENTO = 2 THEN 'Carga' END) TipoDocumento, 
                                DocumentoFaturamento.DFA_VALOR_A_FATURAR Valor, 
                                DocumentoFaturamento.DFA_VALOR_A_FATURAR Saldo,
                                null Fatura,
                                DocumentoFaturamento.DFA_DATAHORAEMISSAO DataEmissao, 
                                GrupoTomador.GRP_DESCRICAO GrupoPessoasTomador,
                                Tomador.CLI_NOME NomeTomador, 
                                Tomador.CLI_CGCCPF CPFCNPJTomador, 
                                Tomador.CLI_FISJUR TipoPessoaTomador, 
                                ModeloDocumento.MOD_ABREVIACAO ModeloDocumento, 
                                EmpresaSerie.ESE_NUMERO Serie, 
                                CONVERT(int, DocumentoFaturamento.DFA_NUMERO) NumeroDocumento,
                                null NumeroTitulo,
                                DocumentoFaturamento.DFA_NUMERO_CARGA NumeroCarga,
                                LocalidadeOrigem.LOC_DESCRICAO Origem,
                                LocalidadeOrigem.UF_SIGLA UFOrigem,
                                LocalidadeDestino.LOC_DESCRICAO Destino,
                                LocalidadeDestino.UF_SIGLA UFDestino,
                                LocalidadeEmpresa.UF_SIGLA UFEmpresa,
                                substring((select CASE WHEN veiculo1.VEI_NUMERO_FROTA IS NULL THEN '' WHEN veiculo1.VEI_NUMERO_FROTA = '' THEN '' ELSE ', ' + veiculo1.VEI_NUMERO_FROTA END from T_DOCUMENTO_FATURAMENTO_VEICULO veiculoDocumento inner join T_VEICULO veiculo1 on veiculoDocumento.VEI_CODIGO = veiculo1.VEI_CODIGO where veiculoDocumento.DFA_CODIGO = DocumentoFaturamento.DFA_CODIGO for xml path('')), 3, 1000) Frotas,
                                substring((select ', ' + motorista1.FUN_NOME from T_DOCUMENTO_FATURAMENTO_MOTORISTA motoristaDocumento inner join T_FUNCIONARIO motorista1 on motoristaDocumento.FUN_CODIGO = motorista1.FUN_CODIGO where motoristaDocumento.DFA_CODIGO = DocumentoFaturamento.DFA_CODIGO for xml path('')), 3, 1000) Motoristas,
                                substring((select ', ' + veiculo1.VEI_PLACA from T_DOCUMENTO_FATURAMENTO_VEICULO veiculoDocumento inner join T_VEICULO veiculo1 on veiculoDocumento.VEI_CODIGO = veiculo1.VEI_CODIGO where veiculoDocumento.DFA_CODIGO = DocumentoFaturamento.DFA_CODIGO for xml path('')), 3, 1000) Placas,
                                DocumentoFaturamento.DFA_NUMERO_OCORRENCIA NumeroOcorrencia,
                                substring((select ', ' + NumeroOcorrencia.DFA_NUMERO_PEDIDO_OCORRENCIA from T_DOCUMENTO_FATURAMENTO_NUMERO_OCORRENCIA NumeroOcorrencia where NumeroOcorrencia.DFA_CODIGO = DocumentoFaturamento.DFA_CODIGO for xml path('')), 3, 1000) NumeroOcorrenciaCliente,
                                substring((select ', ' + NumeroPedido.DFA_NUMERO_PEDIDO from T_DOCUMENTO_FATURAMENTO_NUMERO_PEDIDO NumeroPedido where NumeroPedido.DFA_CODIGO = DocumentoFaturamento.DFA_CODIGO for xml path('')), 3, 1000) NumeroPedidoCliente,
                                null DataVencimento,
                                convert(nvarchar(20), DocumentoFaturamento.DFA_DATA_ENVIO_ULTIMO_CANHOTO, 103) DataEnvioUltimoCanhoto,
                                convert(nvarchar(20), DocumentoFaturamento.DFA_DATA_CHEGADA_ULTIMO_CANHOTO, 103) DataChegadaUltimoCanhoto,
                                Remetente.CLI_NOME NomeRemetente, 
                                Remetente.CLI_CGCCPF CPFCNPJRemetente, 
                                Remetente.CLI_FISJUR TipoPessoaRemetente, 
                                Destinatario.CLI_NOME NomeDestinatario, 
                                Destinatario.CLI_CGCCPF CPFCNPJDestinatario, 
                                Destinatario.CLI_FISJUR TipoPessoaDestinatario,
                                CASE CTe.CON_TIPO_CTE WHEN 0 THEN 'Normal' WHEN 1 THEN 'Complemento' WHEN 2 THEN 'Anulação' WHEN 3 THEN 'Substituto' ELSE '' END TipoCTe,
                                substring((select ', ' + notaFiscal1.NFC_NUMERO from T_CTE_DOCS notaFiscal1 where notaFiscal1.CON_CODIGO = CTe.CON_CODIGO for xml path('')), 3, 1000) NumeroNotaFiscal,
                                CTe.CON_CHAVECTE ChaveAcessoCTe,
                                CTe.CON_OBSGERAIS ObservacaoCTe,
                                CTe.CON_VALOR_RECEBER ValorReceber,
                                null NumeroDocumentoTituloOriginal
                                FROM T_DOCUMENTO_FATURAMENTO DocumentoFaturamento
                                LEFT OUTER JOIN T_GRUPO_PESSOAS GrupoTomador on GrupoTomador.GRP_CODIGO = DocumentoFaturamento.GRP_CODIGO
                                LEFT OUTER JOIN T_CLIENTE Tomador on Tomador.CLI_CGCCPF = DocumentoFaturamento.CLI_CODIGO_TOMADOR 
                                LEFT OUTER JOIN T_MODDOCFISCAL ModeloDocumento on DocumentoFaturamento.MOD_CODIGO = ModeloDocumento.MOD_CODIGO  
                                LEFT OUTER JOIN T_EMPRESA_SERIE EmpresaSerie on EmpresaSerie.ESE_CODIGO = DocumentoFaturamento.ESE_CODIGO
                                LEFT OUTER JOIN T_LOCALIDADES LocalidadeOrigem on LocalidadeOrigem.LOC_CODIGO = DocumentoFaturamento.LOC_ORIGEM
                                LEFT OUTER JOIN T_LOCALIDADES LocalidadeDestino on LocalidadeDestino.LOC_CODIGO = DocumentoFaturamento.LOC_DESTINO
                                LEFT OUTER JOIN T_CLIENTE Destinatario on Destinatario.CLI_CGCCPF = DocumentoFaturamento.CLI_CODIGO_DESTINATARIO
                                LEFT OUTER JOIN T_CLIENTE Remetente on Remetente.CLI_CGCCPF = DocumentoFaturamento.CLI_CODIGO_REMETENTE
                                LEFT OUTER JOIN T_CTE CTe on CTe.CON_CODIGO = DocumentoFaturamento.CON_CODIGO
                                LEFT OUTER JOIN T_EMPRESA Empresa ON DocumentoFaturamento.EMP_CODIGO = Empresa.EMP_CODIGO
                                LEFT OUTER JOIN T_LOCALIDADES LocalidadeEmpresa ON Empresa.LOC_CODIGO = LocalidadeEmpresa.LOC_CODIGO
                                WHERE DocumentoFaturamento.DFA_SITUACAO = 1 AND DocumentoFaturamento.DFA_VALOR_A_FATURAR > 0";

            NHibernate.ISQLQuery query = this.SessionNHiBernate.CreateSQLQuery(sqlQuery);

            query.SetResultTransformer(new NHibernate.Transform.AliasToBeanResultTransformer(typeof(Dominio.Relatorios.Embarcador.DataSource.Financeiros.ResumoContasAReceber)));

            return query.SetTimeout(600).List<Dominio.Relatorios.Embarcador.DataSource.Financeiros.ResumoContasAReceber>();
        }

        public IList<Dominio.Relatorios.Embarcador.DataSource.Financeiros.ResumoContasAReceber> ConsultarRelatorioAnaliticoPosicaoContasAReceber(DateTime dataPosicao, bool utilizarDataBaseLiquidacaoTitulos)
        {
            string dataTituloUtilizar = utilizarDataBaseLiquidacaoTitulos ? "Titulo.TIT_DATA_BASE_LIQUIDACAO" : "Titulo.TIT_DATA_LIQUIDACAO";

            string sqlQuery = $@"DECLARE @dataPosicao datetime = dateadd(day, 1, '{dataPosicao:yyyy-MM-dd}');

                                select  
                                     tits.TipoSumarizacao, 
                                     tits.TipoDocumento, 
                                     tits.Valor, 
                                     tits.Saldo,
                                     Convert(nvarchar(50), tits.Fatura) Fatura,
                                     tits.DataEmissao, 
                                     tits.GrupoPessoasTomador, 
                                     tits.NomeTomador, 
                                     tits.CPFCNPJTomador, 
                                     tits.TipoPessoaTomador,
                                     convert(int, tits.NumeroDocumento) NumeroDocumento, 
                                     tits.NumeroTitulo, 
                                     tits.NumeroCarga, 
                                     tits.NumeroOcorrencia,
                                     convert(nvarchar(20), tits.DataVencimento, 103) DataVencimento, 
                                     convert(nvarchar(20), tits.DataEnvioUltimoCanhoto, 103) DataEnvioUltimoCanhoto, 
                                     tits.NumeroDocumentoTituloOriginal,
                                     substring((select case when veiculo1.VEI_NUMERO_FROTA IS NULL then '' when veiculo1.VEI_NUMERO_FROTA = '' then '' else ', ' + veiculo1.VEI_NUMERO_FROTA end from T_DOCUMENTO_FATURAMENTO_VEICULO veiculoDocumento inner join T_VEICULO veiculo1 on veiculoDocumento.VEI_CODIGO = veiculo1.VEI_CODIGO where veiculoDocumento.DFA_CODIGO = tits.NumeroDocumento for xml path('')), 3, 1000) Frotas,
                                     substring((select ', ' + motorista1.FUN_NOME from T_DOCUMENTO_FATURAMENTO_MOTORISTA motoristaDocumento inner join T_FUNCIONARIO motorista1 on motoristaDocumento.FUN_CODIGO = motorista1.FUN_CODIGO where motoristaDocumento.DFA_CODIGO = tits.NumeroDocumento for xml path('')), 3, 1000) Motoristas,
                                     substring((select ', ' + veiculo1.VEI_PLACA from T_DOCUMENTO_FATURAMENTO_VEICULO veiculoDocumento inner join T_VEICULO veiculo1 on veiculoDocumento.VEI_CODIGO = veiculo1.VEI_CODIGO where veiculoDocumento.DFA_CODIGO = tits.NumeroDocumento for xml path('')), 3, 1000) Placas,
                                     substring((select ', ' + NumeroOcorrencia.DFA_NUMERO_PEDIDO_OCORRENCIA from T_DOCUMENTO_FATURAMENTO_NUMERO_OCORRENCIA NumeroOcorrencia where NumeroOcorrencia.DFA_CODIGO = tits.NumeroDocumento for xml path('')), 3, 1000) NumeroOcorrenciaCliente,
                                     substring((select ', ' + NumeroPedido.DFA_NUMERO_PEDIDO from T_DOCUMENTO_FATURAMENTO_NUMERO_PEDIDO NumeroPedido where NumeroPedido.DFA_CODIGO = tits.NumeroDocumento for xml path('')), 3, 1000) NumeroPedidoCliente,
                                     EmpresaSerie.ESE_NUMERO Serie, 
                                     Remetente.CLI_NOME NomeRemetente, 
                                     Remetente.CLI_CGCCPF CPFCNPJRemetente, 
                                     Remetente.CLI_FISJUR TipoPessoaRemetente, 
                                     Destinatario.CLI_NOME NomeDestinatario, 
                                     Destinatario.CLI_CGCCPF CPFCNPJDestinatario,  
                                     Destinatario.CLI_FISJUR TipoPessoaDestinatario, 
	                                 LocalidadeOrigem.LOC_DESCRICAO Origem, 
                                     LocalidadeOrigem.UF_SIGLA UFOrigem, 
                                     LocalidadeDestino.LOC_DESCRICAO Destino,     
                                     LocalidadeDestino.UF_SIGLA UFDestino, 
                                     LocalidadeEmpresa.UF_SIGLA UFEmpresa,
                                     ModeloDocumento.MOD_ABREVIACAO ModeloDocumento,
                                     case CTe.CON_TIPO_CTE when 0 then 'Normal' when 1 then 'Complemento' when 2 then 'Anulação' when 3 then 'Substituto' else '' end TipoCTe,
                                     substring((select ', ' + notaFiscal1.NFC_NUMERO from T_CTE_DOCS notaFiscal1 where notaFiscal1.CON_CODIGO = CTe.CON_CODIGO for xml path('')), 3, 1000) NumeroNotaFiscal,
                                     CTe.CON_CHAVECTE ChaveAcessoCTe, 
                                     CTe.CON_OBSGERAIS ObservacaoCTe, 
                                     CTe.CON_VALOR_RECEBER ValorReceber,
                                     (select MAX( Notas.NFC_DATAEMISSAO) from T_CTE_DOCS Notas where Notas.CON_CODIGO = CTe.CON_CODIGO ) DataNotaFiscal,
                                     (select MAX(Documentos.CDO_DATA_EMISSAO) from T_CTE_DOCUMENTO_ORIGINARIO Documentos where Documentos.CON_CODIGO = CTe.CON_CODIGO ) DataDocumento 

                                from (

                                       select
                                          'CT-e não Faturado' TipoSumarizacao,
                                          (case when DocumentoFaturamento.DFA_TIPO_DOCUMENTO = 1 then 'CT-e' when DocumentoFaturamento.DFA_TIPO_DOCUMENTO = 2 then 'Carga' end) TipoDocumento, 
                                          (DocumentoFaturamento.DFA_VALOR_DOCUMENTO - SUM(case when Titulo.TIT_DATA_EMISSAO IS NULL OR Titulo.TIT_DATA_EMISSAO >= @dataPosicao then 0 when Titulo.TIT_DATA_CANCELAMENTO < @dataPosicao then 0 when Titulo.TBN_CODIGO is not null then 0 else TituloDocumento.TDO_VALOR end)) Valor, 
                                          (DocumentoFaturamento.DFA_VALOR_DOCUMENTO - SUM(case when Titulo.TIT_DATA_EMISSAO IS NULL OR Titulo.TIT_DATA_EMISSAO >= @dataPosicao then 0 when Titulo.TIT_DATA_CANCELAMENTO < @dataPosicao then 0 when Titulo.TBN_CODIGO is not null then 0 else TituloDocumento.TDO_VALOR end)) Saldo,
                                          null Fatura,
                                          DocumentoFaturamento.DFA_DATA_AUTORIZACAO DataEmissao,
                                          GrupoTomador.GRP_DESCRICAO GrupoPessoasTomador,
                                          Tomador.CLI_NOME NomeTomador,
                                          Tomador.CLI_CGCCPF CPFCNPJTomador, 
                                          Tomador.CLI_FISJUR TipoPessoaTomador,
                                          DocumentoFaturamento.ESE_CODIGO, 
                                          DocumentoFaturamento.DFA_NUMERO NumeroDocumento, 
                                          null NumeroTitulo,
                                          DocumentoFaturamento.DFA_NUMERO_CARGA NumeroCarga, 
                                          DocumentoFaturamento.DFA_NUMERO_OCORRENCIA NumeroOcorrencia,
                                          null DataVencimento,
                                          case when DocumentoFaturamento.DFA_DATA_ENVIO_ULTIMO_CANHOTO < @dataPosicao then DocumentoFaturamento.DFA_DATA_ENVIO_ULTIMO_CANHOTO else null end DataEnvioUltimoCanhoto,     
                                          null NumeroDocumentoTituloOriginal,          
                                          DocumentoFaturamento.LOC_ORIGEM, 
                                          DocumentoFaturamento.LOC_DESTINO, 
                                          DocumentoFaturamento.CLI_CODIGO_DESTINATARIO, 
                                          DocumentoFaturamento.CLI_CODIGO_REMETENTE, 
		                                  DocumentoFaturamento.EMP_CODIGO,  
                                          DocumentoFaturamento.MOD_CODIGO, 
                                          DocumentoFaturamento.CON_CODIGO
          
                                       from T_DOCUMENTO_FATURAMENTO DocumentoFaturamento
                                            left join T_TITULO_DOCUMENTO TituloDocumento on (TituloDocumento.TDO_TIPO_DOCUMENTO = 1 AND TituloDocumento.CON_CODIGO = DocumentoFaturamento.CON_CODIGO)
											left join T_TITULO_DOCUMENTO TituloDocumentoCarga on (TituloDocumentoCarga.TDO_TIPO_DOCUMENTO = 2 AND TituloDocumentoCarga.CAR_CODIGO = DocumentoFaturamento.CAR_CODIGO)
                                            left join T_TITULO Titulo                    on  Titulo.TIT_CODIGO       = ISNULL(TituloDocumento.TIT_CODIGO, TituloDocumentoCarga.TIT_CODIGO) AND Titulo.TIT_TIPO = 1
                                            left join T_GRUPO_PESSOAS GrupoTomador       on  GrupoTomador.GRP_CODIGO = DocumentoFaturamento.GRP_CODIGO
                                            left join T_CLIENTE Tomador                  on Tomador.CLI_CGCCPF       = DocumentoFaturamento.CLI_CODIGO_TOMADOR
                                       where
                                            DocumentoFaturamento.DFA_DATA_AUTORIZACAO < @dataPosicao AND(DocumentoFaturamento.DFA_DATA_CANCELAMENTO IS NULL or DocumentoFaturamento.DFA_DATA_CANCELAMENTO >= @dataPosicao) 
                                            AND (DocumentoFaturamento.DFA_DATA_ANULACAO IS NULL or DocumentoFaturamento.DFA_DATA_ANULACAO >= @dataPosicao) 
			                                AND (DocumentoFaturamento.DFA_SISTEMA_EMISSOR <> 2 OR DocumentoFaturamento.DFA_DATA_VINCULO_CARGA IS NULL or DocumentoFaturamento.DFA_DATA_VINCULO_CARGA >= @dataPosicao)
                                       group by
                                          DocumentoFaturamento.DFA_DATA_ENVIO_ULTIMO_CANHOTO, DocumentoFaturamento.DFA_NUMERO_OCORRENCIA, DocumentoFaturamento.DFA_CODIGO, DocumentoFaturamento.DFA_NUMERO_CARGA,
                                          DocumentoFaturamento.DFA_TIPO_DOCUMENTO, DocumentoFaturamento.DFA_VALOR_DOCUMENTO,
                                          Tomador.CLI_FISJUR, Tomador.CLI_CGCCPF, Tomador.CLI_NOME,GrupoTomador.GRP_DESCRICAO, DocumentoFaturamento.DFA_DATA_AUTORIZACAO, DocumentoFaturamento.DFA_NUMERO,
                                          DocumentoFaturamento.ESE_CODIGO, DocumentoFaturamento.LOC_ORIGEM, DocumentoFaturamento.LOC_DESTINO, DocumentoFaturamento.CLI_CODIGO_DESTINATARIO, DocumentoFaturamento.CLI_CODIGO_REMETENTE,
                                          DocumentoFaturamento.EMP_CODIGO, DocumentoFaturamento.MOD_CODIGO, DocumentoFaturamento.CON_CODIGO 
                                       having
		                                  (DocumentoFaturamento.DFA_VALOR_DOCUMENTO - SUM(case when Titulo.TIT_DATA_EMISSAO IS NULL OR Titulo.TIT_DATA_EMISSAO >= @dataPosicao then 0 when Titulo.TIT_DATA_CANCELAMENTO < @dataPosicao then 0 else TituloDocumento.TDO_VALOR end)) <> 0
        
                                       UNION    
       
                                       select
                                          case when ISNULL(DocumentoFaturamento.DFA_CODIGO, DocumentoFaturamentoCarga.DFA_CODIGO) is not null then 'Título em Aberto' when Fatura.FAT_CODIGO is not null then 'Título em Aberto' else 'Outros Títulos em Aberto' end TipoSumarizacao,
                                          case when ISNULL(DocumentoFaturamento.DFA_TIPO_DOCUMENTO, DocumentoFaturamentoCarga.DFA_TIPO_DOCUMENTO) = 1 then 'CT-e' when ISNULL(DocumentoFaturamento.DFA_TIPO_DOCUMENTO, DocumentoFaturamentoCarga.DFA_TIPO_DOCUMENTO) = 2 then 'Carga' when Fatura.FAT_CODIGO is not null then 'Fatura'  else 'Outros' end TipoDocumento, 
                                          case when TituloDocumento.TDO_CODIGO is not null then TituloDocumento.TDO_VALOR_TOTAL else Titulo.TIT_VALOR_ORIGINAL end Valor, 
                                          case when TituloDocumento.TDO_CODIGO is not null then TituloDocumento.TDO_VALOR_PendENTE else Titulo.TIT_VALOR_PendENTE end Saldo,
                                          Fatura.FAT_NUMERO Fatura,
                                          case when ISNULL(DocumentoFaturamento.DFA_CODIGO, DocumentoFaturamentoCarga.DFA_CODIGO) is not null then ISNULL(DocumentoFaturamento.DFA_DATAHORAEMISSAO, DocumentoFaturamentoCarga.DFA_DATAHORAEMISSAO) else Titulo.TIT_DATA_EMISSAO end DataEmissao, 
                                          case when GrupoTomador.GRP_CODIGO is not null then GrupoTomador.GRP_DESCRICAO else GrupoTomadorTitulo.GRP_DESCRICAO end GrupoPessoasTomador,
                                          case when Tomador.CLI_CGCCPF is not null then Tomador.CLI_NOME else TomadorTitulo.CLI_NOME end NomeTomador, 
                                          case when Tomador.CLI_CGCCPF is not null then Tomador.CLI_CGCCPF else TomadorTitulo.CLI_CGCCPF end CPFCNPJTomador, 
                                          case when Tomador.CLI_CGCCPF is not null then Tomador.CLI_FISJUR else TomadorTitulo.CLI_FISJUR end TipoPessoaTomador, 
                                          ISNULL(DocumentoFaturamento.ESE_CODIGO, DocumentoFaturamentoCarga.ESE_CODIGO) ESE_CODIGO, 
                                          ISNULL(DocumentoFaturamento.DFA_NUMERO, DocumentoFaturamentoCarga.DFA_NUMERO) NumeroDocumento, 
                                          Titulo.TIT_CODIGO NumeroTitulo, 
                                          ISNULL(DocumentoFaturamento.DFA_NUMERO_CARGA, DocumentoFaturamentoCarga.DFA_NUMERO_CARGA) NumeroCarga,
                                          ISNULL(DocumentoFaturamento.DFA_NUMERO_OCORRENCIA, DocumentoFaturamentoCarga.DFA_NUMERO_OCORRENCIA) NumeroOcorrencia, 
                                          Titulo.TIT_DATA_VENCIMENTO DataVencimento,
                                          case when DocumentoFaturamento.DFA_DATA_ENVIO_ULTIMO_CANHOTO < @dataPosicao then DocumentoFaturamento.DFA_DATA_ENVIO_ULTIMO_CANHOTO else null end DataEnvioUltimoCanhoto,
                                          Titulo.TIT_NUMERO_DOCUMENTO_TITULO_ORIGINAL NumeroDocumentoTituloOriginal,          
                                          ISNULL(DocumentoFaturamento.LOC_ORIGEM, DocumentoFaturamentoCarga.LOC_ORIGEM) LOC_ORIGEM, 
                                          ISNULL(DocumentoFaturamento.LOC_DESTINO, DocumentoFaturamentoCarga.LOC_DESTINO) LOC_DESTINO, 
                                          ISNULL(DocumentoFaturamento.CLI_CODIGO_DESTINATARIO, DocumentoFaturamentoCarga.CLI_CODIGO_DESTINATARIO) CLI_CODIGO_DESTINATARIO, 
                                          ISNULL(DocumentoFaturamento.CLI_CODIGO_REMETENTE, DocumentoFaturamentoCarga.CLI_CODIGO_REMETENTE) CLI_CODIGO_REMETENTE,
                                          ISNULL(DocumentoFaturamento.EMP_CODIGO, DocumentoFaturamentoCarga.EMP_CODIGO) EMP_CODIGO, 
                                          ISNULL(DocumentoFaturamento.MOD_CODIGO, DocumentoFaturamentoCarga.MOD_CODIGO) MOD_CODIGO, 
                                          ISNULL(DocumentoFaturamento.CON_CODIGO, DocumentoFaturamentoCarga.CON_CODIGO) CON_CODIGO
                                       from T_TITULO Titulo
                                            left join T_TITULO_DOCUMENTO TituloDocumento           on  TituloDocumento.TIT_CODIGO = Titulo.TIT_CODIGO
                                            left join T_DOCUMENTO_FATURAMENTO DocumentoFaturamento on (TituloDocumento.CON_CODIGO is not null AND TituloDocumento.CON_CODIGO = DocumentoFaturamento.CON_CODIGO)
											left join T_DOCUMENTO_FATURAMENTO DocumentoFaturamentoCarga on (TituloDocumento.CAR_CODIGO is not null AND TituloDocumento.CAR_CODIGO = DocumentoFaturamentoCarga.CAR_CODIGO)
                                            left join T_GRUPO_PESSOAS GrupoTomadorTitulo           on  GrupoTomadorTitulo.GRP_CODIGO = Titulo.GRP_CODIGO
                                            left join T_CLIENTE TomadorTitulo        on TomadorTitulo.CLI_CGCCPF = Titulo.CLI_CGCCPF
                                            left join T_GRUPO_PESSOAS GrupoTomador   on GrupoTomador.GRP_CODIGO  = ISNULL(DocumentoFaturamento.GRP_CODIGO, DocumentoFaturamentoCarga.GRP_CODIGO)
                                            left join T_CLIENTE Tomador              on Tomador.CLI_CGCCPF       = ISNULL(DocumentoFaturamento.CLI_CODIGO_TOMADOR, DocumentoFaturamentoCarga.CLI_CODIGO_TOMADOR)
                                            left join T_FATURA_PARCELA FaturaParcela on FaturaParcela.FAP_CODIGO = Titulo.FAP_CODIGO
                                            left join T_FATURA Fatura                on Fatura.FAT_CODIGO        = FaturaParcela.FAT_CODIGO           
                                       where 
	                                            Titulo.TIT_TIPO = 1 AND Titulo.TIT_DATA_EMISSAO < @dataPosicao 
                                           AND (Titulo.TIT_DATA_CANCELAMENTO IS NULL OR Titulo.TIT_DATA_CANCELAMENTO >= @dataPosicao) 
                                           AND ({dataTituloUtilizar} IS NULL OR {dataTituloUtilizar} >= @dataPosicao)       
                                     ) tits
                                       left join T_LOCALIDADES LocalidadeOrigem  on LocalidadeOrigem.LOC_CODIGO  = tits.LOC_ORIGEM
                                       left join T_LOCALIDADES LocalidadeDestino on LocalidadeDestino.LOC_CODIGO = tits.LOC_DESTINO
                                       left join T_CLIENTE Destinatario          on Destinatario.CLI_CGCCPF      = tits.CLI_CODIGO_DESTINATARIO
                                       left join T_CLIENTE Remetente             on Remetente.CLI_CGCCPF         = tits.CLI_CODIGO_REMETENTE
                                       left join T_EMPRESA Empresa               on tits.EMP_CODIGO              = Empresa.EMP_CODIGO
                                       left join T_LOCALIDADES LocalidadeEmpresa on  empresa.LOC_CODIGO          = LocalidadeEmpresa.LOC_CODIGO
                                       left join T_MODDOCFISCAL ModeloDocumento  on tits.MOD_CODIGO              = ModeloDocumento.MOD_CODIGO
                                       left join T_CTE CTe                       on CTe.CON_CODIGO               = tits.CON_CODIGO
                                       left join T_EMPRESA_SERIE EmpresaSerie    on EmpresaSerie.ESE_CODIGO      = tits.ESE_CODIGO 
                                 WHERE (CTe.CON_CODIGO IS NULL OR CTe.CON_STATUS NOT IN ('Z', 'C')) ";

            NHibernate.ISQLQuery query = this.SessionNHiBernate.CreateSQLQuery(sqlQuery);

            query.SetResultTransformer(new NHibernate.Transform.AliasToBeanResultTransformer(typeof(Dominio.Relatorios.Embarcador.DataSource.Financeiros.ResumoContasAReceber)));

            return query.SetTimeout(99999).List<Dominio.Relatorios.Embarcador.DataSource.Financeiros.ResumoContasAReceber>();
        }


        public IList<Dominio.Relatorios.Embarcador.DataSource.Financeiros.ResumoContasAPagar> ConsultarRelatorioAnaliticoPosicaoContasAPagar(DateTime dataPosicao)
        {
            string sqlQuery = @"DECLARE @dataPosicao datetime = dateadd(day, 1, '" + dataPosicao.ToString("yyyy-MM-dd") + @"');

                                select
                                    'Título em Aberto' TipoSumarizacao,
                                    case when Titulo.CMA_CODIGO IS NOT NULL then 'Cobrança Manual' when Titulo.PAG_CODIGO IS NOT NULL then 'Escrituração' when Titulo.TDD_CODIGO IS NOT NULL then 'Documento Entrada' when Titulo.TDG_CODIGO IS NOT NULL then 'Guia' when Titulo.CFT_CODIGO is not null then 'Contrato de Frete'  else 'Outros' end TipoDocumento, 
                                    Titulo.TIT_VALOR_ORIGINAL Valor, 
                                    Titulo.TIT_VALOR_PendENTE Saldo,
                                    Titulo.TIT_DATA_EMISSAO DataEmissao, 
                                    GrupoTomadorTitulo.GRP_DESCRICAO GrupoPessoasFornecedor,
                                    TomadorTitulo.CLI_NOME NomeFornecedor, 
                                    TomadorTitulo.CLI_CGCCPF CPFCNPJFornecedor, 
                                    TomadorTitulo.CLI_FISJUR TipoPessoaFornecedor, 
                                    Titulo.TIT_NUMERO_DOCUMENTO_TITULO_ORIGINAL NumeroDocumento, 
                                    Titulo.TIT_CODIGO NumeroTitulo, 
                                    Titulo.TIT_DATA_VENCIMENTO DataVencimento,
                                    Titulo.TIT_TIPO_DOCUMENTO_TITULO_ORIGINAL TipoDocumentoTituloOrigem
                                from T_TITULO Titulo    
                                    left join T_CLIENTE TomadorTitulo        on TomadorTitulo.CLI_CGCCPF = Titulo.CLI_CGCCPF                                            
	                                left join T_GRUPO_PESSOAS GrupoTomadorTitulo           on  GrupoTomadorTitulo.GRP_CODIGO = Titulo.GRP_CODIGO
                                where 
	                                    Titulo.TIT_TIPO = 2 AND Titulo.TIT_DATA_EMISSAO < @dataPosicao 
                                    AND (Titulo.TIT_DATA_CANCELAMENTO IS NULL OR Titulo.TIT_DATA_CANCELAMENTO >= @dataPosicao) 
                                    AND (Titulo.TIT_DATA_LIQUIDACAO IS NULL OR Titulo.TIT_DATA_LIQUIDACAO >= @dataPosicao)    ";

            NHibernate.ISQLQuery query = this.SessionNHiBernate.CreateSQLQuery(sqlQuery);

            query.SetResultTransformer(new NHibernate.Transform.AliasToBeanResultTransformer(typeof(Dominio.Relatorios.Embarcador.DataSource.Financeiros.ResumoContasAPagar)));

            return query.SetTimeout(99999).List<Dominio.Relatorios.Embarcador.DataSource.Financeiros.ResumoContasAPagar>();
        }

    }
}
