using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Repositorio.Embarcador.Financeiro
{
    sealed class ConsultaTituloAcrescimoDesconto : Embarcador.Consulta.Consulta<Dominio.ObjetosDeValor.Embarcador.Financeiro.FiltroPesquisaRelatorioTituloAcrescimoDesconto>
    {
        #region Construtores

        public ConsultaTituloAcrescimoDesconto() : base(tabela: "T_TITULO_DOCUMENTO_ACRESCIMO_DESCONTO as AcrescimoDesconto") { }

        #endregion

        #region Métodos Privados

        private void SetarJoinsTituloDocumento(StringBuilder joins)
        {
            if (!joins.Contains(" TituloDocumento "))
                joins.Append(" left join T_TITULO_DOCUMENTO TituloDocumento on TituloDocumento.TDO_CODIGO = AcrescimoDesconto.TDO_CODIGO ");
        }

        private void SetarJoinsTituloBaixaAcrescimo(StringBuilder joins)
        {
            if (!joins.Contains(" TituloBaixaAcrescimo "))
                joins.Append(" left join T_TITULO_BAIXA_ACRESCIMO TituloBaixaAcrescimo on TituloBaixaAcrescimo.TBA_CODIGO = AcrescimoDesconto.TBA_CODIGO ");
        }

        private void SetarJoinsTituloBaixa(StringBuilder joins)
        {
            SetarJoinsTituloBaixaAcrescimo(joins);

            if (!joins.Contains(" TituloBaixa "))
                joins.Append(" left join T_TITULO_BAIXA TituloBaixa on TituloBaixa.TIB_CODIGO = TituloBaixaAcrescimo.TIB_CODIGO ");
        }

        private void SetarJoinsTituloBaixaAgrupado(StringBuilder joins)
        {
            SetarJoinsTituloBaixa(joins);

            if (!joins.Contains(" TituloBaixaAgrupado "))
                joins.Append(" left join T_TITULO_BAIXA_AGRUPADO TituloBaixaAgrupado on TituloBaixaAgrupado.TIB_CODIGO = TituloBaixa.TIB_CODIGO ");
        }

        private void SetarJoinsTitulo(StringBuilder joins)
        {
            SetarJoinsTituloDocumento(joins);
            SetarJoinsTituloBaixaAgrupado(joins);

            if (!joins.Contains(" Titulo "))
                joins.Append(" left join T_TITULO Titulo on Titulo.TIT_CODIGO = ISNULL(TituloDocumento.TIT_CODIGO, TituloBaixaAgrupado.TIT_CODIGO) ");
        }

        private void SetarJoinsPessoa(StringBuilder joins)
        {
            SetarJoinsTitulo(joins);

            if (!joins.Contains(" Pessoa "))
                joins.Append(" left join T_CLIENTE Pessoa on Pessoa.CLI_CGCCPF = Titulo.CLI_CGCCPF ");
        }

        private void SetarJoinsGrupoPessoas(StringBuilder joins)
        {
            SetarJoinsTitulo(joins);

            if (!joins.Contains(" GrupoPessoas "))
                joins.Append(" left join T_GRUPO_PESSOAS GrupoPessoas on Titulo.GRP_CODIGO = GrupoPessoas.GRP_CODIGO ");
        }

        private void SetarJoinsDocumentoFaturamento(StringBuilder joins)
        {
            SetarJoinsTituloDocumento(joins);

            if (!joins.Contains(" DocumentoFaturamentoCTe "))
                joins.Append(" left join T_DOCUMENTO_FATURAMENTO DocumentoFaturamentoCTe on (TituloDocumento.TDO_TIPO_DOCUMENTO = 1 and TituloDocumento.CON_CODIGO = DocumentoFaturamentoCTe.CON_CODIGO) ");
            if (!joins.Contains(" DocumentoFaturamentoCarga "))
                joins.Append(" left join T_DOCUMENTO_FATURAMENTO DocumentoFaturamentoCarga on (TituloDocumento.TDO_TIPO_DOCUMENTO = 2 and TituloDocumento.CAR_CODIGO = DocumentoFaturamentoCarga.CAR_CODIGO) ");
        }

        private void SetarJoinsModeloDocumento(StringBuilder joins)
        {
            SetarJoinsDocumentoFaturamento(joins);

            if (!joins.Contains(" ModeloDocumento "))
                joins.Append(" left join T_MODDOCFISCAL ModeloDocumento on ModeloDocumento.MOD_CODIGO = ISNULL(DocumentoFaturamentoCTe.MOD_CODIGO, DocumentoFaturamentoCarga.MOD_CODIGO) ");
        }

        private void SetarJoinsEmpresa(StringBuilder joins)
        {
            SetarJoinsDocumentoFaturamento(joins);

            if (!joins.Contains(" Empresa1 ") || !joins.Contains(" Empresa2 ")) {
                joins.Append(" left join T_EMPRESA Empresa1 on Empresa1.EMP_CODIGO = DocumentoFaturamentoCTe.EMP_CODIGO ");
                joins.Append(" left join T_EMPRESA Empresa2 on Empresa2.EMP_CODIGO = DocumentoFaturamentoCarga.EMP_CODIGO ");
            }
        }

        private void SetarJoinsFaturaParcela(StringBuilder joins)
        {
            SetarJoinsTitulo(joins);

            if (!joins.Contains(" FaturaParcela "))
                joins.Append(" left join T_FATURA_PARCELA FaturaParcela on FaturaParcela.FAP_CODIGO = Titulo.FAP_CODIGO ");
        }

        private void SetarJoinsFatura(StringBuilder joins)
        {
            SetarJoinsFaturaParcela(joins);

            if (!joins.Contains(" Fatura "))
                joins.Append(" left join T_FATURA Fatura on Fatura.FAT_CODIGO = FaturaParcela.FAT_CODIGO ");
        }

        private void SetarJoinsBordero(StringBuilder joins)
        {
            SetarJoinsTitulo(joins);

            if (!joins.Contains(" Bordero "))
                joins.Append(" left join T_BORDERO Bordero on Bordero.BOR_CODIGO = Titulo.BOR_CODIGO ");
        }

        private void SetarJoinsJustificativa(StringBuilder joins)
        {
            if (!joins.Contains(" Justificativa "))
                joins.Append(" left join T_JUSTIFICATIVA Justificativa on Justificativa.JUS_CODIGO = AcrescimoDesconto.JUS_CODIGO ");
        }

        private void SetarJoinsUsuario(StringBuilder joins)
        {
            if (!joins.Contains(" Usuario "))
                joins.Append(" left join T_FUNCIONARIO Usuario on Usuario.FUN_CODIGO = AcrescimoDesconto.FUN_CODIGO ");
        }

        private void SetarJoinsDocumento(StringBuilder joins)
        {
            if (!joins.Contains(" Documento "))
                joins.Append(" left join T_TITULO_DOCUMENTO Documento on Documento.TDO_CODIGO = AcrescimoDesconto.TDO_CODIGO ");
        }

        #endregion

        #region Métodos Protegidos Sobrescritos

        protected override void SetarSelect(string propriedade, int codigoDinamico, StringBuilder select, StringBuilder joins, StringBuilder groupBy, bool somenteContarNumeroRegistros, Dominio.ObjetosDeValor.Embarcador.Financeiro.FiltroPesquisaRelatorioTituloAcrescimoDesconto filtrosPesquisa)
        {
            switch (propriedade)
            {
                case "Titulo":
                    if (!select.Contains(" Titulo, "))
                    {
                        select.Append("Titulo.TIT_CODIGO Titulo, ");
                        groupBy.Append("Titulo.TIT_CODIGO, ");

                        SetarJoinsTitulo(joins);
                    }
                    break;

                case "DataEmissao":
                    if (!select.Contains(" DataEmissao, "))
                    {
                        select.Append("Titulo.TIT_DATA_EMISSAO DataEmissao, ");
                        groupBy.Append("Titulo.TIT_DATA_EMISSAO, ");

                        SetarJoinsTitulo(joins);
                    }
                    break;

                case "DataLiquidacao":
                    if (!select.Contains(" DataLiquidacao, "))
                    {
                        select.Append("CONVERT(nvarchar(20), Titulo.TIT_DATA_LIQUIDACAO, 103) DataLiquidacao, ");
                        groupBy.Append("Titulo.TIT_DATA_LIQUIDACAO, ");

                        SetarJoinsTitulo(joins);
                    }
                    break;

                case "DataBaseLiquidacao":
                    if (!select.Contains(" DataBaseLiquidacao, "))
                    {
                        select.Append("CONVERT(nvarchar(20), Titulo.TIT_DATA_BASE_LIQUIDACAO, 103) DataBaseLiquidacao, ");
                        groupBy.Append("Titulo.TIT_DATA_BASE_LIQUIDACAO, ");

                        SetarJoinsTitulo(joins);
                    }
                    break;

                case "ValorTitulo":
                    if (!select.Contains(" ValorTitulo, "))
                    {
                        select.Append("SUM(Titulo.TIT_VALOR) ValorTitulo, ");

                        SetarJoinsTitulo(joins);
                    }
                    break;

                case "TipoDocumento":
                    if (!select.Contains(" TipoDocumento, "))
                    {
                        select.Append("(CASE DocumentoFaturamentoCTe.DFA_TIPO_DOCUMENTO WHEN 1 THEN 'Documento' ELSE 'Carga' END) TipoDocumento, ");
                        groupBy.Append("DocumentoFaturamentoCTe.DFA_TIPO_DOCUMENTO, DocumentoFaturamentoCarga.DFA_TIPO_DOCUMENTO, ");

                        SetarJoinsDocumentoFaturamento(joins);
                    }
                    break;

                case "ModeloDocumento":
                    if (!select.Contains(" ModeloDocumento, "))
                    {
                        select.Append("ModeloDocumento.MOD_ABREVIACAO ModeloDocumento, ");
                        groupBy.Append("ModeloDocumento.MOD_ABREVIACAO, ");

                        SetarJoinsModeloDocumento(joins);
                    }
                    break;

                case "DataEmissaoDocumentos":
                    if (!select.Contains(" DataEmissaoDocumentos, "))
                    {
                        select.Append("CONVERT(NVARCHAR(10), ISNULL(DocumentoFaturamentoCTe.DFA_DATAHORAEMISSAO, DocumentoFaturamentoCarga.DFA_DATAHORAEMISSAO), 103) DataEmissaoDocumentos, ");
                        groupBy.Append("DocumentoFaturamentoCTe.DFA_DATAHORAEMISSAO, DocumentoFaturamentoCarga.DFA_DATAHORAEMISSAO, ");

                        SetarJoinsDocumentoFaturamento(joins);
                    }
                    break;

                case "ValorDocumento":
                    if (!select.Contains(" ValorDocumento, "))
                    {
                        select.Append("SUM(ISNULL(DocumentoFaturamentoCTe.DFA_VALOR_DOCUMENTO, DocumentoFaturamentoCarga.DFA_VALOR_DOCUMENTO)) ValorDocumento, ");

                        SetarJoinsDocumentoFaturamento(joins);
                    }
                    break;

                case "NumeroDocumento":
                    if (!select.Contains(" NumeroDocumento, "))
                    {
                        select.Append("ISNULL(DocumentoFaturamentoCTe.DFA_NUMERO, DocumentoFaturamentoCarga.DFA_NUMERO) NumeroDocumento, ");
                        groupBy.Append("DocumentoFaturamentoCTe.DFA_NUMERO, DocumentoFaturamentoCarga.DFA_NUMERO, ");

                        SetarJoinsDocumentoFaturamento(joins);
                    }
                    break;

                case "Empresa":
                    if (!select.Contains(" Empresa, "))
                    {
                        select.Append("Isnull(Empresa1.EMP_RAZAO,Empresa2.EMP_RAZAO) Empresa, ");
                        groupBy.Append(" Isnull(Empresa1.EMP_RAZAO,Empresa2.EMP_RAZAO), ");

                        SetarJoinsEmpresa(joins);
                    }
                    break;

                case "Pessoa":
                    if (!select.Contains(" Pessoa, "))
                    {
                        select.Append("Pessoa.CLI_NOME Pessoa, ");
                        groupBy.Append("Pessoa.CLI_NOME, ");

                        SetarJoinsPessoa(joins);
                    }
                    break;

                case "CPFCNPJPessoaFormatado":
                    if (!select.Contains(" CPFCNPJPessoa, "))
                    {
                        select.Append("Pessoa.CLI_CGCCPF CPFCNPJPessoa, Pessoa.CLI_FISJUR TipoPessoa, ");
                        groupBy.Append("Pessoa.CLI_CGCCPF, Pessoa.CLI_FISJUR, ");

                        SetarJoinsPessoa(joins);
                    }
                    break;

                case "GrupoPessoas":
                    if (!select.Contains(" GrupoPessoas, "))
                    {
                        select.Append("GrupoPessoas.GRP_DESCRICAO GrupoPessoas, ");
                        groupBy.Append("GrupoPessoas.GRP_DESCRICAO, ");

                        SetarJoinsGrupoPessoas(joins);
                    }
                    break;

                case "SituacaoTitulo":
                    if (!select.Contains(" SituacaoTitulo, "))
                    {
                        select.Append("(CASE Titulo.TIT_STATUS WHEN 5 THEN 'Em Negociação' WHEN 4 THEN 'Cancelado' WHEN 3 THEN 'Quitado' WHEN 2 THEN 'Atrazado' ELSE 'Em Aberto' END) SituacaoTitulo, ");
                        groupBy.Append("Titulo.TIT_STATUS, ");

                        SetarJoinsTitulo(joins);
                    }
                    break;

                case "NumeroFatura":
                    if (!select.Contains(" NumeroFatura, "))
                    {
                        select.Append("(CASE WHEN Fatura.FAT_NUMERO_FATURA_INTEGRACAO IS NULL OR Fatura.FAT_NUMERO_FATURA_INTEGRACAO = 0 THEN Fatura.FAT_NUMERO ELSE Fatura.FAT_NUMERO_FATURA_INTEGRACAO END) NumeroFatura, ");
                        groupBy.Append("Fatura.FAT_NUMERO, Fatura.FAT_NUMERO_FATURA_INTEGRACAO, ");

                        SetarJoinsFatura(joins);
                    }
                    break;

                case "NumeroBordero":
                    if (!select.Contains(" NumeroBordero, "))
                    {
                        select.Append("Bordero.BOR_NUMERO NumeroBordero, ");
                        groupBy.Append("Bordero.BOR_NUMERO, ");

                        SetarJoinsBordero(joins);
                    }
                    break;

                case "Justificativa":
                    if (!select.Contains(" Justificativa, "))
                    {
                        select.Append("Justificativa.JUS_DESCRICAO Justificativa, ");
                        groupBy.Append("Justificativa.JUS_DESCRICAO, ");

                        SetarJoinsJustificativa(joins);
                    }
                    break;

                case "Observacao":
                    if (!select.Contains(" Observacao, "))
                    {
                        select.Append("AcrescimoDesconto.TDV_OBSERVACAO Observacao, ");
                        groupBy.Append("AcrescimoDesconto.TDV_OBSERVACAO, ");
                    }
                    break;

                case "Valor":
                    if (!select.Contains(" Valor, "))
                    {
                        select.Append("AcrescimoDesconto.TDV_VALOR Valor, ");
                        groupBy.Append("AcrescimoDesconto.TDV_VALOR, ");
                    }
                    break;

                case "Tipo":
                    if (!select.Contains(" Tipo, "))
                    {
                        select.Append("(CASE AcrescimoDesconto.TDV_TIPO WHEN 0 THEN 'Geração' ELSE 'Baixa' END) Tipo, ");
                        groupBy.Append("AcrescimoDesconto.TDV_TIPO, ");
                    }
                    break;

                case "TipoJustificativa":
                    if (!select.Contains(" TipoJustificativa, "))
                    {
                        select.Append("(CASE AcrescimoDesconto.TDV_TIPO_JUSTIFICATIVA WHEN 1 THEN 'Desconto' ELSE 'Acréscimo' END) TipoJustificativa, ");
                        groupBy.Append("AcrescimoDesconto.TDV_TIPO_JUSTIFICATIVA, ");
                    }
                    break;

                case "Usuario":
                    if (!select.Contains(" Usuario, "))
                    {
                        select.Append("Usuario.FUN_NOME Usuario, ");
                        groupBy.Append("Usuario.FUN_NOME, ");

                        SetarJoinsUsuario(joins);
                    }
                    break;

                case "DataAplicacaoFormatada":
                    if (!select.Contains(" DataAplicacao, "))
                    {
                        select.Append("AcrescimoDesconto.TDV_DATA_APLICACAO DataAplicacao, ");
                        groupBy.Append("AcrescimoDesconto.TDV_DATA_APLICACAO, ");
                    }
                    break;
                case "CPFMotoristaFormatado":
                    if (!select.Contains(" CPFMotorista, "))
                    {
                        select.Append(@"SUBSTRING((SELECT DISTINCT ', ' + Motorista.CMO_CPF_MOTORISTA
                                                FROM T_CTE_MOTORISTA Motorista
                                                WHERE Motorista.CON_CODIGO = Documento.CON_CODIGO  FOR XML PATH('')), 3, 1000) CPFMotorista, ");
                        if (!groupBy.Contains(" Documento.CON_CODIGO, "))
                            groupBy.Append("Documento.CON_CODIGO, ");

                        SetarJoinsDocumento(joins);
                    }
                    break;
                case "NomeMotorista":
                    if (!select.Contains(" NomeMotorista, "))
                    {
                        select.Append(@"SUBSTRING((SELECT DISTINCT ', ' + Motorista.CMO_NOME_MOTORISTA
                                                FROM T_CTE_MOTORISTA Motorista
                                                WHERE Motorista.CON_CODIGO = Documento.CON_CODIGO  FOR XML PATH('')), 3, 1000) NomeMotorista, ");
                        if (!groupBy.Contains(" Documento.CON_CODIGO, "))
                            groupBy.Append("Documento.CON_CODIGO, ");

                        SetarJoinsDocumento(joins);
                    }
                    break;
                case "CodigoIntegracaoMotorista":
                    if (!select.Contains(" CodigoIntegracaoMotorista, "))
                    {
                        select.Append(@"SUBSTRING((SELECT DISTINCT ', ' + Funcionario.FUN_CODIGO_INTEGRACAO
                                                FROM T_CTE_MOTORISTA Motorista
                                                JOIN T_FUNCIONARIO Funcionario on Funcionario.FUN_CPF = Motorista.CMO_CPF_MOTORISTA
                                                WHERE Motorista.CON_CODIGO = Documento.CON_CODIGO  FOR XML PATH('')), 3, 1000) CodigoIntegracaoMotorista, ");
                        if (!groupBy.Contains(" Documento.CON_CODIGO, "))
                            groupBy.Append("Documento.CON_CODIGO, ");
                        SetarJoinsDocumento(joins);
                    }
                    break;
            }
        }

        protected override void SetarWhere(Dominio.ObjetosDeValor.Embarcador.Financeiro.FiltroPesquisaRelatorioTituloAcrescimoDesconto filtrosPesquisa, StringBuilder where, StringBuilder joins, StringBuilder groupBy, List<Embarcador.Consulta.ParametroSQL> parametros = null)
        {
            string datePattern = "yyyy-MM-dd";

            if (filtrosPesquisa.DataEmissaoInicial != DateTime.MinValue)
            {
                where.Append(" and Titulo.TIT_DATA_EMISSAO > '" + filtrosPesquisa.DataEmissaoInicial.AddDays(-1).ToString(datePattern) + "'");
                SetarJoinsTitulo(joins);
            }

            if (filtrosPesquisa.DataEmissaoFinal != DateTime.MinValue)
            {
                where.Append(" and Titulo.TIT_DATA_EMISSAO < '" + filtrosPesquisa.DataEmissaoFinal.AddDays(1).ToString(datePattern) + "'");
                SetarJoinsTitulo(joins);
            }

            if (filtrosPesquisa.DataLiquidacaoInicial != DateTime.MinValue)
            {
                where.Append(" and Titulo.TIT_DATA_LIQUIDACAO > '" + filtrosPesquisa.DataLiquidacaoInicial.AddDays(-1).ToString(datePattern) + "'");
                SetarJoinsTitulo(joins);
            }

            if (filtrosPesquisa.DataLiquidacaoFinal != DateTime.MinValue)
            {
                where.Append(" and Titulo.TIT_DATA_LIQUIDACAO < '" + filtrosPesquisa.DataLiquidacaoFinal.AddDays(1).ToString(datePattern) + "'");
                SetarJoinsTitulo(joins);
            }

            if (filtrosPesquisa.DataBaseLiquidacaoInicial != DateTime.MinValue)
            {
                where.Append(" and Titulo.TIT_DATA_BASE_LIQUIDACAO > '" + filtrosPesquisa.DataBaseLiquidacaoInicial.AddDays(-1).ToString(datePattern) + "'");
                SetarJoinsTitulo(joins);
            }

            if (filtrosPesquisa.DataBaseLiquidacaoFinal != DateTime.MinValue)
            {
                where.Append(" and Titulo.TIT_DATA_BASE_LIQUIDACAO < '" + filtrosPesquisa.DataBaseLiquidacaoFinal.AddDays(1).ToString(datePattern) + "'");
                SetarJoinsTitulo(joins);
            }

            if (filtrosPesquisa.CodigoBordero > 0)
            {
                where.Append(" and Titulo.BOR_CODIGO = " + filtrosPesquisa.CodigoBordero.ToString());
                SetarJoinsTitulo(joins);
            }

            if (filtrosPesquisa.CodigoFatura > 0)
            {
                where.Append(" and FaturaParcela.FAT_CODIGO = " + filtrosPesquisa.CodigoFatura.ToString());
                SetarJoinsFaturaParcela(joins);
            }

            if (filtrosPesquisa.CodigoGrupoPessoas > 0)
            {
                where.Append(" and Titulo.GRP_CODIGO = " + filtrosPesquisa.CodigoGrupoPessoas.ToString());
                SetarJoinsTitulo(joins);
            }

            if (filtrosPesquisa.CodigosJustificativa.Count > 0)
                where.Append(" and AcrescimoDesconto.JUS_CODIGO in (" + string.Join(", ", filtrosPesquisa.CodigosJustificativa) + ")");

            if (filtrosPesquisa.CpfCnpjPessoa > 0)
            {
                where.Append(" and Titulo.CLI_CGCCPF = " + filtrosPesquisa.CpfCnpjPessoa.ToString("F0"));
                SetarJoinsTitulo(joins);
            }

            if (filtrosPesquisa.SituacaoTitulo != Dominio.ObjetosDeValor.Embarcador.Enumeradores.StatusTitulo.Todos)
            {
                where.Append(" and Titulo.TIT_STATUS = " + filtrosPesquisa.SituacaoTitulo.ToString("D"));
                SetarJoinsTitulo(joins);
            }

            if (filtrosPesquisa.TipoJustificativa.HasValue)
                where.Append(" and AcrescimoDesconto.TDV_TIPO_JUSTIFICATIVA = " + filtrosPesquisa.TipoJustificativa.Value.ToString("D"));

            if (filtrosPesquisa.Tipo.HasValue)
                where.Append(" and AcrescimoDesconto.TDV_TIPO = " + filtrosPesquisa.Tipo.Value.ToString("D"));

            if (filtrosPesquisa.CodigoCTe > 0)
            {
                where.Append(" and TituloDocumento.CON_CODIGO = " + filtrosPesquisa.CodigoCTe.ToString());
                SetarJoinsTituloDocumento(joins);
            }

            if (filtrosPesquisa.TipoDeTitulo != Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoTitulo.Todos)
            {
                where.Append(" and Titulo.TIT_TIPO = " + filtrosPesquisa.TipoDeTitulo.ToString("D"));
                SetarJoinsTitulo(joins);
            }
        }

        #endregion
    }
}
