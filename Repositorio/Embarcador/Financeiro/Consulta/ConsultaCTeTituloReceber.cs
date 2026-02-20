using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Repositorio.Embarcador.Financeiro
{
    sealed class ConsultaCTeTituloReceber : Embarcador.Consulta.Consulta<Dominio.ObjetosDeValor.Embarcador.Financeiro.FiltroPesquisaRelatorioCTeTituloReceber>
    {
        #region Construtores

        public ConsultaCTeTituloReceber() : base(tabela: "T_CTE as CTe") { }

        #endregion

        #region Métodos Privados

        private void SetarJoinsSerie(StringBuilder joins)
        {
            if (!joins.Contains(" Serie "))
                joins.Append(" left join T_EMPRESA_SERIE Serie on CTe.CON_SERIE = Serie.ESE_CODIGO ");
        }

        private void SetarJoinsLocalidadeFimPrestacao(StringBuilder joins)
        {
            if (!joins.Contains(" FimPrestacaoCTe "))
                joins.Append(" left join T_LOCALIDADES FimPrestacaoCTe on CTe.CON_LOCTERMINOPRESTACAO = FimPrestacaoCTe.LOC_CODIGO ");
        }

        private void SetarJoinsLocalidadeInicioPrestacao(StringBuilder joins)
        {
            if (!joins.Contains(" InicioPrestacaoCTe "))
                joins.Append(" left join T_LOCALIDADES InicioPrestacaoCTe on CTe.CON_LOCINICIOPRESTACAO = InicioPrestacaoCTe.LOC_CODIGO ");
        }

        private void SetarJoinsEmpresa(StringBuilder joins)
        {
            if (!joins.Contains(" Empresa "))
                joins.Append(" left join T_EMPRESA Empresa on Empresa.EMP_CODIGO = CTe.EMP_CODIGO ");
        }

        private void SetarJoinsTitulo(StringBuilder joins)
        {
            if (!joins.Contains(" Titulo "))
                joins.Append(" left join T_TITULO Titulo on Titulo.TIT_CODIGO = CTe.TIT_CODIGO ");
        }

        private void SetarJoinsRemetente(StringBuilder joins)
        {
            if (!joins.Contains(" RemetenteCTe "))
                joins.Append(" left join T_CTE_PARTICIPANTE RemetenteCTe on CTe.CON_REMETENTE_CTE = RemetenteCTe.PCT_CODIGO ");
        }

        private void SetarJoinsRemetenteCliente(StringBuilder joins)
        {
            SetarJoinsRemetente(joins);

            if (!joins.Contains(" ClienteRemetente "))
                joins.Append(" left join T_CLIENTE ClienteRemetente on ClienteRemetente.CLI_CGCCPF = RemetenteCTe.CLI_CODIGO ");
        }

        private void SetarJoinsDestinatario(StringBuilder joins)
        {
            if (!joins.Contains(" DestinatarioCTe "))
                joins.Append(" left join T_CTE_PARTICIPANTE DestinatarioCTe on CTe.CON_DESTINATARIO_CTE = DestinatarioCTe.PCT_CODIGO ");
        }

        private void SetarJoinsCargaCTe(StringBuilder joins)
        {
            if (!joins.Contains(" CargaCTe "))
                joins.Append(" left join T_CARGA_CTE CargaCTe on CTe.CON_CODIGO = CargaCTe.CON_CODIGO ");
        }

        private void SetarJoinsCarga(StringBuilder joins)
        {
            SetarJoinsCargaCTe(joins);

            if (!joins.Contains(" Carga "))
                joins.Append(" left join T_CARGA Carga on CargaCTe.CAR_CODIGO = Carga.CAR_CODIGO ");
        }

        private void SetarJoinsDestinatarioCliente(StringBuilder joins)
        {
            SetarJoinsDestinatario(joins);

            if (!joins.Contains(" ClienteDestinatario "))
                joins.Append(" left join T_CLIENTE ClienteDestinatario on ClienteDestinatario.CLI_CGCCPF = DestinatarioCTe.CLI_CODIGO ");
        }

        #endregion

        #region Métodos Protegidos Sobrescritos

        protected override void SetarSelect(string propriedade, int codigoDinamico, StringBuilder select, StringBuilder joins, StringBuilder groupBy, bool somenteContarNumeroRegistros, Dominio.ObjetosDeValor.Embarcador.Financeiro.FiltroPesquisaRelatorioCTeTituloReceber filtroPesquisa)
        {
            if (!select.Contains(" Codigo, "))
            {
                select.Append("CTe.CON_CODIGO as Codigo, ");
                groupBy.Append("CTe.CON_CODIGO, ");
            }

            switch (propriedade)
            {
                case "Numero":
                    if (!select.Contains(" Numero, "))
                    {
                        select.Append("CTe.CON_NUM as Numero, ");
                        groupBy.Append("CTe.CON_NUM, ");
                    }
                    break;

                case "Observacao":
                    if (!select.Contains(" Observacao, "))
                    {
                        select.Append("Titulo.TIT_OBSERVACAO as Observacao, ");
                        groupBy.Append("Titulo.TIT_OBSERVACAO, ");
                    }
                    break;

                case "Serie":
                    if (!select.Contains(" Serie, "))
                    {
                        select.Append("Serie.ESE_NUMERO as Serie, ");
                        groupBy.Append("Serie.ESE_NUMERO, ");

                        SetarJoinsSerie(joins);
                    }
                    break;

                case "Origem":
                    if (!select.Contains(" Origem, "))
                    {
                        select.Append("InicioPrestacaoCTe.LOC_DESCRICAO + ' - ' + InicioPrestacaoCTe.UF_SIGLA Origem, ");
                        groupBy.Append("InicioPrestacaoCTe.LOC_DESCRICAO, InicioPrestacaoCTe.UF_SIGLA, ");

                        SetarJoinsLocalidadeInicioPrestacao(joins);
                    }
                    break;

                case "Destino":
                    if (!select.Contains(" Destino,"))
                    {
                        select.Append("FimPrestacaoCTe.LOC_DESCRICAO + ' - ' + FimPrestacaoCTe.UF_SIGLA Destino, ");
                        groupBy.Append("FimPrestacaoCTe.LOC_DESCRICAO, FimPrestacaoCTe.UF_SIGLA, ");

                        SetarJoinsLocalidadeFimPrestacao(joins);
                    }
                    break;

                case "Transportador":
                    if (!select.Contains(" Transportador, "))
                    {
                        select.Append("Empresa.EMP_RAZAO Transportador, ");
                        groupBy.Append("Empresa.EMP_RAZAO, ");

                        SetarJoinsEmpresa(joins);
                    }
                    break;

                case "CNPJTransportador":
                    if (!select.Contains(" CNPJTransportador, "))
                    {
                        select.Append("Empresa.EMP_CNPJ CNPJTransportador, ");
                        groupBy.Append("Empresa.EMP_CNPJ, ");

                        SetarJoinsEmpresa(joins);
                    }
                    break;

                case "DescricaoDataEmissao":
                    if (!select.Contains(" DataEmissao, "))
                    {
                        select.Append("CTe.CON_DATAHORAEMISSAO DataEmissao, ");
                        groupBy.Append("CTe.CON_DATAHORAEMISSAO, ");
                    }
                    break;

                case "DescricaoDataVencimento":
                    if (!select.Contains(" DataVencimento, "))
                    {
                        select.Append("Titulo.TIT_DATA_VENCIMENTO as DataVencimento, ");
                        groupBy.Append("Titulo.TIT_DATA_VENCIMENTO, ");
                    }
                    break;

                case "DescricaoDataLiquidacao":
                    if (!select.Contains(" DataLiquidacao, "))
                    {
                        select.Append("Titulo.TIT_DATA_LIQUIDACAO as DataLiquidacao, ");
                        groupBy.Append("Titulo.TIT_DATA_LIQUIDACAO, ");
                    }
                    break;

                case "Remetente":
                    if (!select.Contains(" Remetente, "))
                    {
                        select.Append(@"((case when ClienteRemetente.CLI_CODIGO_INTEGRACAO is not null and ClienteRemetente.CLI_CODIGO_INTEGRACAO <> '' then ClienteRemetente.CLI_CODIGO_INTEGRACAO + ' - ' else '' end) 
                                          + RemetenteCTe.PCT_NOME + ' (' + RemetenteCTe.PCT_CPF_CNPJ + ')') Remetente, ");
                        groupBy.Append("ClienteRemetente.CLI_CODIGO_INTEGRACAO, RemetenteCTe.PCT_NOME, RemetenteCTe.PCT_CPF_CNPJ, ");

                        SetarJoinsRemetenteCliente(joins);
                    }
                    break;

                case "Destinatario":
                    if (!select.Contains(" Destinatario, "))
                    {
                        select.Append(@"((case when ClienteDestinatario.CLI_CODIGO_INTEGRACAO is not null and ClienteDestinatario.CLI_CODIGO_INTEGRACAO <> '' then ClienteDestinatario.CLI_CODIGO_INTEGRACAO + ' - ' else '' end) 
                                          + DestinatarioCTe.PCT_NOME + ' (' + DestinatarioCTe.PCT_CPF_CNPJ + ')') Destinatario, ");
                        groupBy.Append("ClienteDestinatario.CLI_CODIGO_INTEGRACAO, DestinatarioCTe.PCT_NOME, DestinatarioCTe.PCT_CPF_CNPJ, ");

                        SetarJoinsDestinatarioCliente(joins);
                    }
                    break;

                case "ValorAReceber":
                    if (!select.Contains(" ValorAReceber, "))
                    {
                        select.Append("CTe.CON_VALOR_RECEBER ValorAReceber, ");
                        groupBy.Append("CTe.CON_VALOR_RECEBER, ");
                    }
                    break;

                case "DescricaoSituacao":
                    if (!select.Contains(" StatusTitulo, "))
                    {
                        select.Append("ISNULL(Titulo.TIT_STATUS, 1) as StatusTitulo, ");
                        groupBy.Append("Titulo.TIT_STATUS, ");
                    }
                    break;

                case "Peso":
                    if (!select.Contains(" Peso, "))
                    {
                        select.Append("CTe.CON_PESO Peso, ");
                        groupBy.Append("CTe.CON_PESO, ");
                    }
                    break;

                case "StatusFormatada":
                    if (!select.Contains(" Status, "))
                    {
                        select.Append("CTe.CON_STATUS Status, ");
                        groupBy.Append("CTe.CON_STATUS, ");
                    }
                    break;

                case "NotaFiscal":
                    if (!select.Contains(" NotaFiscal, "))
                    {
                        select.Append(@"substring((select distinct ', ' + CAST(XMLNotaFiscal.NF_NUMERO AS VARCHAR(20)) from T_CTE_XML_NOTAS_FISCAIS XMLNotaFiscais 
                                        inner join T_XML_NOTA_FISCAL XMLNotaFiscal on XMLNotaFiscal.NFX_CODIGO = XMLNotaFiscais.NFX_CODIGO 
                                        where XMLNotaFiscais.CON_CODIGO = CTe.CON_CODIGO for xml path('')), 3, 1000) NotaFiscal, ");

                        if (!groupBy.Contains("CTe.CON_CODIGO"))
                            groupBy.Append(" CTe.CON_CODIGO, ");
                    }
                    break;

                case "TipoOperacao":
                    if (!select.Contains(" TipoOperacao, "))
                    {
                        select.Append(@"substring((select distinct ', ' + TipoOperacao.TOP_DESCRICAO from T_CARGA_CTE CargaCTe 
                                        inner join T_CARGA Carga on Carga.CAR_CODIGO = CargaCTe.CAR_CODIGO_ORIGEM 
                                        inner join T_TIPO_OPERACAO TipoOperacao on TipoOperacao.TOP_CODIGO = Carga.TOP_CODIGO 
                                        where CargaCTe.CON_CODIGO = CTe.CON_CODIGO for xml path('')), 3, 1000) TipoOperacao, ");

                        if (!groupBy.Contains("CTe.CON_CODIGO"))
                            groupBy.Append(" CTe.CON_CODIGO, ");
                    }
                    break;

                case "NumeroFatura":
                    if (!select.Contains(" NumeroFatura, "))
                    {
                        select.Append(@"substring((SELECT DISTINCT ', ' + CAST((CASE WHEN Fatura.FAT_NUMERO_FATURA_INTEGRACAO IS NULL OR Fatura.FAT_NUMERO_FATURA_INTEGRACAO = 0 THEN Fatura.FAT_NUMERO ELSE Fatura.FAT_NUMERO_FATURA_INTEGRACAO END) AS VARCHAR(20))
                                        FROM T_FATURA Fatura
                                        JOIN T_FATURA_DOCUMENTO FaturaDocumento ON FaturaDocumento.FAT_CODIGO = Fatura.FAT_CODIGO
                                        JOIN T_DOCUMENTO_FATURAMENTO DocumentoFaturamento ON DocumentoFaturamento.DFA_CODIGO = FaturaDocumento.DFA_CODIGO
                                        WHERE DocumentoFaturamento.CON_CODIGO = CTe.CON_CODIGO FOR XML path('')), 3, 1000) NumeroFatura, ");

                        if (!groupBy.Contains("CTe.CON_CODIGO"))
                            groupBy.Append(" CTe.CON_CODIGO, ");
                    }
                    break;

                case "NumeroCarga":
                    if (!select.Contains(" NumeroCarga, "))
                    {
                        select.Append(@"Carga.CAR_CODIGO_CARGA_EMBARCADOR NumeroCarga, ");

                        if (!groupBy.Contains("Carga.CAR_CODIGO_CARGA_EMBARCADOR"))
                            groupBy.Append(" Carga.CAR_CODIGO_CARGA_EMBARCADOR, ");

                        SetarJoinsCarga(joins);
                    }
                    break;

                case "Acrescimos":
                    if (!select.Contains(" Acrescimos, "))
                    {
                        select.Append(@"'R$' + CAST(CAST(ROUND(Titulo.TIT_ACRESCIMO, 2, 1) AS DECIMAL(18,2)) AS VARCHAR(30)) Acrescimos, ");

                        if (!groupBy.Contains("Titulo.TIT_ACRESCIMO"))
                            groupBy.Append(" Titulo.TIT_ACRESCIMO, ");

                        SetarJoinsTitulo(joins);
                    }
                    break;

                case "Decrescimos":
                    if (!select.Contains(" Decrescimos, "))
                    {
                        select.Append(@"'R$' + CAST(CAST(ROUND(Titulo.TIT_DESCONTO, 2, 1) AS DECIMAL(18,2)) AS VARCHAR(30)) Decrescimos, ");

                        if (!groupBy.Contains("Titulo.TIT_DESCONTO"))
                            groupBy.Append(" Titulo.TIT_DESCONTO, ");

                        SetarJoinsTitulo(joins);
                    }
                    break;

                case "ChaveCTe":
                    if (!select.Contains(" ChaveCTe, "))
                    {
                        select.Append("CTe.CON_CHAVECTE ChaveCTe, ");
                        groupBy.Append("CTe.CON_CHAVECTE, ");
                    }
                    break;
            }
        }

        protected override void SetarWhere(Dominio.ObjetosDeValor.Embarcador.Financeiro.FiltroPesquisaRelatorioCTeTituloReceber filtrosPesquisa, StringBuilder where, StringBuilder joins, StringBuilder groupBy, List<Embarcador.Consulta.ParametroSQL> parametros = null)
        {
            string pattern = "yyyy-MM-dd";

            where.Append($" and CTe.CON_STATUS = 'A'");

            SetarJoinsTitulo(joins);

            if (filtrosPesquisa.DataInicio != DateTime.MinValue)
                where.Append("  and CTe.CON_DATAHORAEMISSAO >= '" + filtrosPesquisa.DataInicio.ToString(pattern) + "'");

            if (filtrosPesquisa.DataFim != DateTime.MinValue)
                where.Append("  and CTe.CON_DATAHORAEMISSAO < '" + filtrosPesquisa.DataFim.AddDays(1).ToString(pattern) + "'");

            if (filtrosPesquisa.CodigoEmpresa > 0)
                where.Append($" and CTe.EMP_CODIGO = {filtrosPesquisa.CodigoEmpresa}");

            if (filtrosPesquisa.NumeroCTe > 0)
                where.Append("  and CTe.CON_NUM = " + filtrosPesquisa.NumeroCTe.ToString());

            if (filtrosPesquisa.StatusTitulo != Dominio.ObjetosDeValor.Embarcador.Enumeradores.StatusTitulo.Todos)
            {
                if (filtrosPesquisa.StatusTitulo == Dominio.ObjetosDeValor.Embarcador.Enumeradores.StatusTitulo.Atrazada)
                    where.Append(" and CTe.TIT_CODIGO is null");
                else if (filtrosPesquisa.StatusTitulo == Dominio.ObjetosDeValor.Embarcador.Enumeradores.StatusTitulo.EmAberto)
                    where.Append($" and (CTe.TIT_CODIGO is null or Titulo.TIT_STATUS = {filtrosPesquisa.StatusTitulo.ToString("d")})");
                else
                    where.Append($" and Titulo.TIT_STATUS = {filtrosPesquisa.StatusTitulo.ToString("d")}");
            }

            if (filtrosPesquisa.SomenteTitulosLiberados)
                where.Append(" and Titulo.TIT_LIBERADO_PAGAMENTO = 1");

            if (filtrosPesquisa.CnpjCpfRemetente > 0d)
            {
                where.Append(" and ClienteRemetente.CLI_CGCCPF = " + filtrosPesquisa.CnpjCpfRemetente.ToString("F0"));

                SetarJoinsRemetenteCliente(joins);
            }

            if (filtrosPesquisa.CodigosFiliais.Any(n => n == -1))
            {
                where.Append($@" and  CTe.CON_CODIGO in (select CargaCTe.CON_CODIGO from T_CARGA_CTE CargaCTe 
                                                        inner join T_CARGA Carga on CargaCTe.CAR_CODIGO_ORIGEM = Carga.CAR_CODIGO 
                                                        left join T_CARGA_PEDIDO CargaPedido on CargaPedido.CAR_CODIGO = Carga.CAR_CODIGO
                                                        left join T_PEDIDO Pedido on Pedido.PED_CODIGO = CargaPedido.PED_CODIGO
                                                        where Carga.FIL_CODIGO IN ({string.Join(", ", filtrosPesquisa.CodigosFiliais)}) OR Pedido.CLI_CODIGO_RECEBEDOR IN ({string.Join(", ", filtrosPesquisa.CodigosRecebedores)}) )");
            }

            else if (filtrosPesquisa.CodigosFiliais.Count > 0)
            {
                where.Append($@" and CTe.CON_CODIGO in (select CargaCTe.CON_CODIGO from T_CARGA_CTE CargaCTe 
                                                        inner join T_CARGA Carga on CargaCTe.CAR_CODIGO_ORIGEM = Carga.CAR_CODIGO 
                                                        where Carga.FIL_CODIGO IN ({string.Join(", ", filtrosPesquisa.CodigosFiliais)}))");
            }

            if (filtrosPesquisa.NumeroFatura > 0)
            {
                where.Append($@" and CTe.CON_CODIGO in (select DocumentoFaturamento.CON_CODIGO FROM T_FATURA Fatura
						                                JOIN T_FATURA_DOCUMENTO FaturaDocumento ON FaturaDocumento.FAT_CODIGO = Fatura.FAT_CODIGO
						                                JOIN T_DOCUMENTO_FATURAMENTO DocumentoFaturamento ON DocumentoFaturamento.DFA_CODIGO = FaturaDocumento.DFA_CODIGO
						                                WHERE Fatura.FAT_NUMERO = {filtrosPesquisa.NumeroFatura})");
            }

            if (filtrosPesquisa.TipoServicoMultisoftware == AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiCTe)
            {
                where.Append("  and CTe.CON_CODIGO not in (select conhecimen14_.CON_CODIGO from T_CARGA_CTE_COMPLEMENTO_INFO cargacteco11_ inner join T_CARGA_OCORRENCIA cargaocorr12_ on cargacteco11_.COC_CODIGO=cargaocorr12_.COC_CODIGO inner join T_OCORRENCIA tipodeocor13_  on cargaocorr12_.OCO_CODIGO=tipodeocor13_.OCO_CODIGO  left outer join T_CTE conhecimen14_ on cargacteco11_.CON_CODIGO=conhecimen14_.CON_CODIGO where tipodeocor13_.OCO_BLOQUEAR_VISUALIZACAO_PORTAL_TRANSPORTADOR=1 AND cargacteco11_.CON_CODIGO IS NOT NULL) ");
            }
        }

        #endregion
    }
}
