using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Repositorio.Embarcador.Financeiro
{
    sealed class ConsultaLiberacaoPagamentoProvedor : Embarcador.Consulta.Consulta<Dominio.ObjetosDeValor.Embarcador.Financeiro.FiltroPesquisaRelatorioLiberacaoPagamentoProvedor>
    {
        #region Construtores

        public ConsultaLiberacaoPagamentoProvedor() : base(tabela: "T_CARGA as Carga", somenteRegistrosDistintos: false) { }

        #endregion

        #region Métodos Privados

        private void SetarJoinPagamentoProvedor(StringBuilder joins)
        {
            SetarJoinPagamentoProvedorCarga(joins);

            if (!joins.Contains(" PagamentoProvedor "))
                joins.Append(" left join T_PAGAMENTO_PROVEDOR as PagamentoProvedor on PagamentoProvedor.PRO_CODIGO = PagamentoProvedorCarga.PRO_CODIGO ");
        }

        private void SetarJoinPagamentoProvedorCarga(StringBuilder joins)
        {
            if (!joins.Contains(" PagamentoProvedorCarga "))
                joins.Append(" left join T_PAGAMENTO_PROVEDOR_CARGA as PagamentoProvedorCarga on Carga.CAR_CODIGO = PagamentoProvedorCarga.CAR_CODIGO ");
        }

        private void SetarJoinTipoOperacao(StringBuilder joins)
        {
            if (!joins.Contains(" TipoOperacao "))
                joins.Append(" left join T_TIPO_OPERACAO TipoOperacao on Carga.TOP_CODIGO = TipoOperacao.TOP_CODIGO ");
        }

        private void SetarJoinConfiguracaoTipoOperacao(StringBuilder joins)
        {
            SetarJoinTipoOperacao(joins);

            if (!joins.Contains(" ConfiguracaoTipoOperacao "))
                joins.Append(" left join T_CONFIGURACAO_TIPO_OPERACAO_CARGA ConfiguracaoTipoOperacao on TipoOperacao.CCG_CODIGO = ConfiguracaoTipoOperacao.CCG_CODIGO ");
        }

        private void SetarJoinEmpresaFilial(StringBuilder joins)
        {
            if (!joins.Contains(" EmpresaFilial "))
                joins.Append(" left join T_EMPRESA as EmpresaFilial on Carga.EMP_CODIGO_FILIAL_EMISSORA = EmpresaFilial.EMP_CODIGO ");
        }

        private void SetarJoinEmpresa(StringBuilder joins)
        {
            if (!joins.Contains(" Empresa "))
                joins.Append(" left join T_EMPRESA as Empresa on Carga.EMP_CODIGO = Empresa.EMP_CODIGO ");
        }

        #endregion

        protected override void SetarSelect(string propriedade, int codigoDinamico, StringBuilder select, StringBuilder joins, StringBuilder groupBy, bool somenteContarNumeroRegistros, Dominio.ObjetosDeValor.Embarcador.Financeiro.FiltroPesquisaRelatorioLiberacaoPagamentoProvedor filtroPesquisa)
        {

            switch (propriedade)
            {
                case "Codigo":
                    if (!select.Contains(" Codigo,"))
                    {
                        select.Append(" Carga.CAR_CODIGO as Codigo, ");
                    }
                    break;
                case "NumeroCarga":
                    if (!select.Contains(" NumeroCarga,"))
                    {
                        select.Append(" Carga.CAR_CODIGO_CARGA_EMBARCADOR as NumeroCarga, ");
                    }
                    break;
                case "NumeroOS":
                    if (!select.Contains(" NumeroOS,"))
                    {
                        select.Append(@"  substring(
                                                (
                                                  SELECT 
                                                    ', ' + _pedido.PED_NUMERO_OS 
                                                  FROM 
                                                    T_CARGA_PEDIDO _cargaPedido 
                                                    left join T_PEDIDO _pedido on _cargaPedido.PED_CODIGO = _pedido.PED_CODIGO 
                                                  where 
                                                    _cargaPedido.CAR_CODIGO = Carga.CAR_CODIGO for xml path('')
                                                ), 
                                                3, 
                                                1000
                                              ) as NumeroOS, ");
                    }
                    break;
                case "DataCriacaoCargaFormatada":
                    if (!select.Contains(" DataCriacaoCarga,"))
                    {
                        select.Append(" Carga.CAR_DATA_CRIACAO as DataCriacaoCarga,  ");
                    }
                    break;
                case "TipoOperacao":
                    if (!select.Contains(" TipoOperacao,"))
                    {
                        select.Append(" TipoOperacao.TOP_DESCRICAO as TipoOperacao, ");

                        SetarJoinTipoOperacao(joins);
                    }
                    break;
                case "CidadeOrigemCarga":
                    if (!select.Contains(" CidadeOrigemCarga,"))
                    {
                        select.Append(@"    substring(
                                                (
                                                  select 
                                                    ', ' + _localidadeOrigemCarga.LOC_DESCRICAO 
                                                  from 
                                                    T_CARGA_PEDIDO _cargaPedido 
                                                    join T_LOCALIDADES as _localidadeOrigemCarga on _cargaPedido.LOC_CODIGO_ORIGEM = _localidadeOrigemCarga.LOC_CODIGO 
                                                  where 
                                                    _cargaPedido.CAR_CODIGO = Carga.CAR_CODIGO for xml path('')
                                                ), 
                                                3, 
                                                1000
                                              ) as CidadeOrigemCarga, ");
                    }
                    break;
                case "EstadoOrigemCarga":
                    if (!select.Contains(" EstadoOrigemCarga,"))
                    {
                        select.Append(@"    substring(
                                                (
                                                  select 
                                                    ', ' + _estadoOrigemCarga.UF_NOME 
                                                  from 
                                                    T_CARGA_PEDIDO _cargaPedido 
                                                    join T_LOCALIDADES as _localidadeOrigemCarga on _cargaPedido.LOC_CODIGO_ORIGEM = _localidadeOrigemCarga.LOC_CODIGO 
                                                    join T_UF as _estadoOrigemCarga on _localidadeOrigemCarga.UF_SIGLA = _estadoOrigemCarga.UF_SIGLA 
                                                  where 
                                                    _cargaPedido.CAR_CODIGO = Carga.CAR_CODIGO for xml path('')
                                                ), 
                                                3, 
                                                1000
                                              ) as EstadoOrigemCarga,  ");
                    }
                    break;
                case "CidadeDestinoCarga":
                    if (!select.Contains(" CidadeDestinoCarga,"))
                    {
                        select.Append(@"    substring(
                                                (
                                                  select 
                                                    ', ' + _localidadeDestinoCarga.LOC_DESCRICAO 
                                                  from 
                                                    T_CARGA_PEDIDO _cargaPedido 
                                                    join T_LOCALIDADES as _localidadeDestinoCarga on _cargaPedido.LOC_CODIGO_DESTINO = _localidadeDestinoCarga.LOC_CODIGO 
                                                  where 
                                                    _cargaPedido.CAR_CODIGO = Carga.CAR_CODIGO for xml path('')
                                                ), 
                                                3, 
                                                1000
                                              ) as CidadeDestinoCarga,");
                    }
                    break;
                case "EstadoDestinoCarga":
                    if (!select.Contains(" EstadoDestinoCarga,"))
                    {
                        select.Append(@"    substring(
                                                (
                                                  select 
                                                    ', ' + _estadoDestinoCarga.UF_NOME 
                                                  from 
                                                    T_CARGA_PEDIDO _cargaPedido 
                                                    join T_LOCALIDADES as _localidadeDestinoCarga on _cargaPedido.LOC_CODIGO_DESTINO = _localidadeDestinoCarga.LOC_CODIGO 
                                                    join T_UF as _estadoDestinoCarga on _localidadeDestinoCarga.UF_SIGLA = _estadoDestinoCarga.UF_SIGLA 
                                                  where 
                                                    _cargaPedido.CAR_CODIGO = Carga.CAR_CODIGO for xml path('')
                                                ), 
                                                3, 
                                                1000
                                              ) as EstadoDestinoCarga, ");
                    }
                    break;
                case "NomeFilialEmissoraCarga":
                    if (!select.Contains(" NomeFilialEmissoraCarga,"))
                    {
                        select.Append(" COALESCE(EmpresaFilial.EMP_RAZAO, Empresa.EMP_RAZAO) as NomeFilialEmissoraCarga,  ");

                        SetarJoinEmpresaFilial(joins);
                        SetarJoinEmpresa(joins);
                    }
                    break;
                case "CNPJFilialEmissoraCargaFormatado":
                    if (!select.Contains(" CNPJFilialEmissoraCarga,"))
                    {
                        select.Append(" COALESCE(EmpresaFilial.EMP_CNPJ, Empresa.EMP_CNPJ) as CNPJFilialEmissoraCarga, ");

                        SetarJoinEmpresaFilial(joins);
                        SetarJoinEmpresa(joins);
                    }
                    break;
                case "NomeProvedor":
                    if (!select.Contains(" NomeProvedor, "))
                    {
                        select.Append(@"    substring(
                                                (
                                                  select 
                                                    ', ' + _provedor.CLI_NOME 
                                                  from 
                                                    T_CARGA_PEDIDO _cargaPedido 
                                                    join T_PEDIDO as _pedido on _cargaPedido.PED_CODIGO = _pedido.PED_CODIGO 
                                                    join T_CLIENTE as _provedor on _pedido.CLI_CODIGO_PROVEDOR_OS = _provedor.CLI_CGCCPF 
                                                  where 
                                                    _cargaPedido.CAR_CODIGO = carga.CAR_CODIGO for xml path('')
                                                ), 
                                                3, 
                                                1000
                                              ) as NomeProvedor,   ");
                    }
                    break;
                case "CNPJProvedorFormatado":
                    if (!select.Contains(" CNPJProvedor,"))
                    {
                        select.Append(@"    substring(
                                                (
                                                  SELECT 
                                                    ', ' + CONVERT(
                                                      varchar(18), 
                                                      CAST(_provedor.CLI_CGCCPF AS BIGINT)
                                                    ) 
                                                  FROM 
                                                    T_CARGA_PEDIDO _cargaPedido 
                                                    join T_PEDIDO as _pedido on _cargaPedido.PED_CODIGO = _pedido.PED_CODIGO 
                                                    join T_CLIENTE as _provedor on _pedido.CLI_CODIGO_PROVEDOR_OS = _provedor.CLI_CGCCPF 
                                                  where 
                                                    _cargaPedido.CAR_CODIGO = carga.CAR_CODIGO for xml path('')
                                                ), 
                                                3, 
                                                1000
                                              ) as CNPJProvedor, ");
                    }
                    break;
                case "NomeTomadorCarga":
                    if (!select.Contains(" NomeTomadorCarga, "))
                    {
                        select.Append(@"    substring(
                                                (
                                                  SELECT 
                                                    ', ' + _tomador.CLI_NOME 
                                                  FROM 
                                                    T_CARGA_PEDIDO _cargaPedido 
                                                    join T_CLIENTE as _tomador on _cargaPedido.CLI_CODIGO_TOMADOR = _tomador.CLI_CGCCPF 
                                                  where 
                                                    _cargaPedido.CAR_CODIGO = carga.CAR_CODIGO for xml path('')
                                                ), 
                                                3, 
                                                1000
                                              ) as NomeTomadorCarga,  ");
                    }
                    break;
                case "CNPJTomadorCargaFormatado":
                    if (!select.Contains(" CNPJTomadorCarga,"))
                    {
                        select.Append(@"    substring(
                                                (
                                                  SELECT 
                                                    ', ' + CONVERT(
                                                      varchar(18), 
                                                      CAST(_tomador.CLI_CGCCPF AS BIGINT)
                                                    ) 
                                                  FROM 
                                                    T_CARGA_PEDIDO _cargaPedido 
                                                    join T_CLIENTE as _tomador on _cargaPedido.CLI_CODIGO_TOMADOR = _tomador.CLI_CGCCPF 
                                                  where 
                                                    _cargaPedido.CAR_CODIGO = carga.CAR_CODIGO for xml path('')
                                                ), 
                                                3, 
                                                1000
                                              ) as CNPJTomadorCarga,  ");
                    }
                    break;
                case "ValorTotalEstimadoPagamento":
                    if (!select.Contains(" ValorTotalEstimadoPagamento,"))
                    {
                        select.Append("  Carga.CAR_VALOR_TOTAL_PROVEDOR as ValorTotalEstimadoPagamento, ");
                    }
                    break;
                case "NumeroDocumentoProvedor":
                    if (!select.Contains(" NumeroDocumentoProvedor,"))
                    {
                        select.Append("PagamentoProvedor.PRO_NUMERO_NFSE as NumeroDocumentoProvedor, ");

                        SetarJoinPagamentoProvedor(joins);
                    }
                    break;
                case "NumerosDosMultiplosDocumentosProvedor":
                    if (!select.Contains(" NumerosDosMultiplosDocumentosProvedor,"))
                    {
                        select.Append("PagamentoProvedor.PRO_NUMERO_CTES as NumerosDosMultiplosDocumentosProvedor, ");
                        SetarJoinPagamentoProvedor(joins);  
                    }
                    break;

                case "EtapaLiberacaoPagamentoProvedorDescricao":
                    if (!select.Contains(" EtapaLiberacaoPagamentoProvedor,"))
                    {
                        select.Append("PagamentoProvedor.PRO_ETAPA_LIBERACAO_PAGAMENTO_PROVEDOR as EtapaLiberacaoPagamentoProvedor, ");

                        SetarJoinPagamentoProvedor(joins);
                    }
                    break;
                case "SituacaoLiberacaoPagamentoProvedorDescricao":
                    if (!select.Contains(" SituacaoLiberacaoPagamentoProvedor,"))
                    {
                        select.Append("PagamentoProvedor.PRO_SITUACAO_PROVEDOR as SituacaoLiberacaoPagamentoProvedor, ");

                        SetarJoinPagamentoProvedor(joins);
                    }
                    break;
                case "TipoDocumentoProvedorDescricao":
                    if (!select.Contains(" TipoDocumentoProvedor,"))
                    {
                        select.Append("PagamentoProvedor.PRO_TIPO_DOCUMENTO_PROVEDOR as TipoDocumentoProvedor, ");

                        SetarJoinPagamentoProvedor(joins);
                    }
                    break;
                case "DataEmissaoDocumentoProvedorFormatada":
                    if (!select.Contains(" DataEmissaoDocumentoProvedor,"))
                    {
                        select.Append("PagamentoProvedor.PRO_DATA_EMISSAO_NFSE as DataEmissaoDocumentoProvedor, ");

                        SetarJoinPagamentoProvedor(joins);
                    }
                    break;
                case "NumeroDocumentoCobrancaContraCliente":
                    if (!select.Contains(" NumeroDocumentoCobrancaContraCliente,"))
                    {
                        select.Append(@"    substring(
                                                (
                                                  SELECT 
                                                    ', ' + CONVERT(
                                                      varchar(50), 
                                                      cte.CON_NUM
                                                    ) 
                                                  FROM 
                                                    T_CARGA_CTE cargaCte 
                                                    join T_CTE cte ON cargaCte.CON_CODIGO = cte.CON_CODIGO 
                                                  where 
                                                    cargaCte.CAR_CODIGO = carga.CAR_CODIGO for xml path('')
                                                ), 
                                                3, 
                                                1000
                                              ) as NumeroDocumentoCobrancaContraCliente,   ");
                    }
                    break;
                case "TipoDocumentoCobrancaContraClienteDescricao":
                    if (!select.Contains(" TipoDocumentoCobrancaContraCliente,"))
                    {
                        select.Append(@"
                            substring(
                                (
                                  SELECT
                                    ', ' +
                                    CASE
                                        WHEN _modelo.MOD_TIPO_DOCUMENTO_EMISSAO = 2
                                             THEN _modelo.MOD_ABREVIACAO       -- mostra abreviação se for “Outros”
                                             ELSE CONVERT(
                                                    varchar(50),
                                                    _modelo.MOD_TIPO_DOCUMENTO_EMISSAO
                                                  )
                                    END
                                  FROM  T_CARGA_CTE   _cargaCte
                                  JOIN  T_CTE         _cte     ON _cargaCte.CON_CODIGO  = _cte.CON_CODIGO
                                  JOIN  T_MODDOCFISCAL _modelo ON _cte.CON_MODELODOC    = _modelo.MOD_CODIGO
                                  WHERE _cargaCte.CAR_CODIGO = carga.CAR_CODIGO
                                  FOR XML PATH('')
                                ),
                                3,
                                1000
                            ) as TipoDocumentoCobrancaContraCliente, ");
                    }
                    break;
                case "ValorTotalDocumentoCobrancaContraClienteFormatado":
                    if (!select.Contains(" ValorTotalDocumentoCobrancaContraCliente,"))
                    {
                        select.Append(@"    substring(
                                                (
                                                  SELECT 
                                                    sum(_cte.CON_VALOR_RECEBER) 
                                                  FROM 
                                                    T_CARGA_CTE _cargaCte 
                                                    join T_CTE _cte ON _cargaCte.CON_CODIGO = _cte.CON_CODIGO 
                                                  where 
                                                    _cargaCte.CAR_CODIGO = carga.CAR_CODIGO for xml path('')
                                                ), 
                                                0, 
                                                1000
                                              ) as ValorTotalDocumentoCobrancaContraCliente,  ");
                    }
                    break;
                case "DataEmissaoDocumentoCobrancaContraClienteFormatada":
                    if (!select.Contains(" DataEmissaoDocumentoCobrancaContraCliente,"))
                    {
                        select.Append(@"     substring(
                                                (
                                                  SELECT 
                                                    ', ' + CONVERT(
                                                      varchar(20), 
                                                      _cte.CON_DATAHORAEMISSAO, 
                                                      103
                                                    ) + ' ' + CONVERT(
                                                      varchar(8), 
                                                      _cte.CON_DATAHORAEMISSAO, 
                                                      108
                                                    ) 
                                                  FROM 
                                                    T_CARGA_CTE _cargaCte 
                                                    join T_CTE _cte ON _cargaCte.CON_CODIGO = _cte.CON_CODIGO 
                                                  where 
                                                    _cargaCte.CAR_CODIGO = carga.CAR_CODIGO for xml path('')
                                                ), 
                                                3, 
                                                1000
                                              ) as DataEmissaoDocumentoCobrancaContraCliente,  ");
                    }
                    break;
                case "ValorTotalRealPagamentoCTe":
                    if (!select.Contains(" ValorTotalRealPagamentoCTe,"))
                    {
                        select.Append(" PagamentoProvedor.PRO_VALOR_A_RECEBER_CTE as ValorTotalRealPagamentoCTe, ");

                        SetarJoinPagamentoProvedor(joins);
                    }
                    break;
                case "ValorTotalRealPagamentoNFSe":
                    if (!select.Contains(" ValorTotalRealPagamentoNFSe,"))
                    {
                        select.Append(" PagamentoProvedorCarga.PRC_VALOR_RATEADO as ValorTotalRealPagamentoNFSe, ");

                        SetarJoinPagamentoProvedorCarga(joins);
                    }
                    break;
                case "IndicacaoLiberacaoOKDescricao":
                    if (!select.Contains(" IndicacaoLiberacaoOK,"))
                    {
                        select.Append(" Carga.CAR_INDIC_LIBERACAO_OK as IndicacaoLiberacaoOK, ");
                    }
                    break;
                case "AliquotaCTeProvedor":
                    if (!select.Contains(" AliquotaCTeProvedor,"))
                    {
                        select.Append("  PagamentoProvedor.PRO_ALIQUOTA_CTE as AliquotaCTeProvedor, ");
                    }
                    break;
                case "ValorICMSProvedor":
                    if (!select.Contains(" ValorICMSProvedor,"))
                    {
                        select.Append("  PagamentoProvedor.PRO_ICMS as ValorICMSProvedor, ");
                    }
                    break;
                case "JustificativaAprovacao":
                    if (!select.Contains(" JustificativaAprovacao,"))
                    {
                        select.Append(" COALESCE(PagamentoProvedor.PRO_MOTIVO_APROVACAO_REGRA, PagamentoProvedor.PRO_MOTIVO_REJEICAO_REGRA) as JustificativaAprovacao, ");

                        SetarJoinPagamentoProvedor(joins);
                    }
                    break;
            }
        }

        protected override void SetarWhere(Dominio.ObjetosDeValor.Embarcador.Financeiro.FiltroPesquisaRelatorioLiberacaoPagamentoProvedor filtrosPesquisa, StringBuilder where, StringBuilder joins, StringBuilder groupBy, List<Embarcador.Consulta.ParametroSQL> parametros = null)
        {
            string pattern = "yyyy-MM-dd";

            where.Append(@" and Carga.CAR_LIBERAR_PAGAMENTO = 1 and Carga.CAR_CATEGORIA_OS = 2 
                            and ((Carga.CAR_CODIGO not in (SELECT CAR_CODIGO from T_PAGAMENTO_PROVEDOR_CARGA) and Carga.CAR_SITUACAO not in (13,18))
                            or Carga.CAR_CODIGO in (SELECT CAR_CODIGO from T_PAGAMENTO_PROVEDOR_CARGA))");

            if (filtrosPesquisa.DataCargaInicial != DateTime.MinValue)
                where.Append($" and Carga.CAR_DATA_CRIACAO >= '{filtrosPesquisa.DataCargaInicial.ToString(pattern)}'");

            if (filtrosPesquisa.DataCargaFinal != DateTime.MinValue)
                where.Append($" and Carga.CAR_DATA_CRIACAO <= '{filtrosPesquisa.DataCargaFinal.AddDays(1).ToString(pattern)}'");

            if (!string.IsNullOrWhiteSpace(filtrosPesquisa.NumeroCarga))
                where.Append($" and Carga.CAR_CODIGO_CARGA_EMBARCADOR = '{filtrosPesquisa.NumeroCarga}' ");

            if (!string.IsNullOrWhiteSpace(filtrosPesquisa.NumeroOS))
            {
                where.Append($@" and exists (select 
                                    1
                                from 
                                    T_CARGA_PEDIDO _cargaPedido
                                    join T_PEDIDO as _pedido on _cargaPedido.PED_CODIGO = _pedido.PED_CODIGO 
                                where 
                                    _cargaPedido.CAR_CODIGO = Carga.CAR_CODIGO and _pedido.PED_NUMERO_OS = '{filtrosPesquisa.NumeroOS}') ");
            }

            if (filtrosPesquisa.CodigoTipoOperacao > 0)
                where.Append($" and Carga.TOP_CODIGO = {filtrosPesquisa.CodigoTipoOperacao} ");

            if (filtrosPesquisa.CodigoFilialEmissora > 0)
                where.Append($" and  Carga.EMP_CODIGO_FILIAL_EMISSORA = {filtrosPesquisa.CodigoFilialEmissora} ");

            if (filtrosPesquisa.CodigoProvedor > 0)
            {
                where.Append($@" and exists (select 
                                    1
                                from 
                                    T_CARGA_PEDIDO _cargaPedido
                                    join T_PEDIDO as _pedido on _cargaPedido.PED_CODIGO = _pedido.PED_CODIGO 
                                    join T_CLIENTE as _provedor on _pedido.CLI_CODIGO_PROVEDOR_OS = _provedor.CLI_CGCCPF 
                                where 
                                    _cargaPedido.CAR_CODIGO = Carga.CAR_CODIGO and _provedor.CLI_CGCCPF = {filtrosPesquisa.CodigoProvedor} )");

                SetarJoinPagamentoProvedor(joins);
            }

            if (filtrosPesquisa.CodigoTomador > 0)
            {
                where.Append($@"  and exists (SELECT  1
                                              FROM T_CARGA_PEDIDO _cargaPedido
                                              join T_CLIENTE as _tomador on _cargaPedido.CLI_CODIGO_TOMADOR = _tomador.CLI_CGCCPF 
                                              where _cargaPedido.CAR_CODIGO = Carga.CAR_CODIGO and _tomador.CLI_CGCCPF = {filtrosPesquisa.CodigoTomador} ) ");

                SetarJoinPagamentoProvedor(joins);
            }

            if (filtrosPesquisa.SituacaoLiberacaoPagamentoProvedor != Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoLiberacaoPagamentoProvedor.Todos)
            {
                where.Append($" and PagamentoProvedor.PRO_SITUACAO_PROVEDOR = {(int)filtrosPesquisa.SituacaoLiberacaoPagamentoProvedor} ");
                SetarJoinPagamentoProvedor(joins);
            }

            if (filtrosPesquisa.TipoDocumentoProvedor != Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoDocumentoProvedor.Nenhum)
            {
                where.Append($" and ConfiguracaoTipoOperacao.CCG_DOCUMENTO_PROVEDOR = {(int)filtrosPesquisa.TipoDocumentoProvedor} ");
                SetarJoinConfiguracaoTipoOperacao(joins);
            }

            if (filtrosPesquisa.IndicacaoLiberacaoOK != Dominio.Enumeradores.OpcaoSimNaoPesquisa.Todos)
                where.Append($" and Carga.CAR_INDIC_LIBERACAO_OK = {(int)filtrosPesquisa.IndicacaoLiberacaoOK} ");

            if (filtrosPesquisa.EtapaLiberacaoPagamentoProvedor != Dominio.ObjetosDeValor.Embarcador.Enumeradores.EtapaLiberacaoPagamentoProvedor.Todos)
            {
                where.Append($" and PagamentoProvedor.PRO_ETAPA_LIBERACAO_PAGAMENTO_PROVEDOR = {(int)filtrosPesquisa.EtapaLiberacaoPagamentoProvedor} ");
                SetarJoinPagamentoProvedor(joins);
            }

        }
    }
}
