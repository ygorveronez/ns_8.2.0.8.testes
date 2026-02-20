using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Repositorio
{
    sealed class ConsultaMDFesAverbacao : Embarcador.Consulta.Consulta<Dominio.ObjetosDeValor.Embarcador.MDFe.FiltroPesquisaMDFesAverbadosRelatorio>
    {
        #region Construtores

        public ConsultaMDFesAverbacao() : base(tabela: "T_MDFE_AVERBACAO_BRADESCO MDFeAverbacao") { }

        #endregion

        #region Métodos Privados

        private void SetarJoinsMDFe(StringBuilder joins)
        {
            if (!joins.Contains(" MDFe "))
            {
                joins.Append(" inner join T_MDFE MDFe on MDFe.MDF_CODIGO = MDFeAverbacao.MDF_CODIGO ");
                SetarJoinsModeloDocumentoFiscal(joins);
                SetarJoinsSeguradora(joins);
            }
        }
        
        private void SetarJoinsEmpresaSerie(StringBuilder joins)
        {
            SetarJoinsMDFe(joins);
            if (!joins.Contains(" Serie "))
                joins.Append(" inner join T_EMPRESA_SERIE Serie on Serie.ESE_CODIGO = MDFe.ESE_CODIGO ");
        }

        private void SetarJoinsCarga(StringBuilder joins)
        {
            if (!joins.Contains(" Carga "))
                joins.Append(" inner join T_CARGA Carga on Carga.CAR_CODIGO = MDFeAverbacao.CAR_CODIGO ");
        }

        private void SetarJoinsTipoCarga(StringBuilder joins)
        {
            if (!joins.Contains(" TipoCarga "))
                joins.Append(" left join T_TIPO_DE_CARGA TipoCarga on Carga.TCG_CODIGO = TipoCarga.TCG_CODIGO ");
        }

        private void SetarJoinsTipoOperacao(StringBuilder joins)
        {
            SetarJoinsCarga(joins);
            if (!joins.Contains(" TipoOperacao "))
                joins.Append(" left join T_TIPO_OPERACAO TipoOperacao on TipoOperacao.TOP_CODIGO = Carga.TOP_CODIGO ");
        }

        private void SetarJoinsEmpresa(StringBuilder joins)
        {
            SetarJoinsMDFe(joins);
            if (!joins.Contains(" Empresa "))
                joins.Append(" left join T_EMPRESA Empresa on Empresa.EMP_CODIGO = MDFe.EMP_CODIGO ");
        }

        private void SetarJoinsCargaPedidoApoliceSeguro(StringBuilder joins)
        {
            if (!joins.Contains(" CargaPedidoApoliceSeguro "))
                joins.Append(" left join T_CARGA_PEDIDO_APOLICE_SEGURO_AVERBACAO CargaPedidoApoliceSeguro on CargaPedidoApoliceSeguro.CPA_CODIGO = MDFeAverbacao.CPA_CODIGO ");
        }

        private void SetarJoinsApoliceSeguro(StringBuilder joins)
        {
            SetarJoinsCargaPedidoApoliceSeguro(joins);
            if (!joins.Contains(" ApoliceSeguro "))
                joins.Append(" left join T_APOLICE_SEGURO_GERAL ApoliceSeguro on ApoliceSeguro.APS_CODIGO = CargaPedidoApoliceSeguro.APS_CODIGO ");
        }

        private void SetarJoinsSeguradora(StringBuilder joins)
        {
            SetarJoinsApoliceSeguro(joins);
            if (!joins.Contains(" Seguradora "))
                joins.Append(" left join T_SEGURADORA Seguradora on Seguradora.SEA_CODIGO = ApoliceSeguro.SEA_CODIGO ");
        }

        private void SetarJoinsModeloDocumentoFiscal(StringBuilder joins)
        {
            if (!joins.Contains(" ModeloDocumentoFiscal "))
                joins.Append(" left join T_MODDOCFISCAL ModeloDocumentoFiscal on ModeloDocumentoFiscal.MOD_CODIGO = MDFe.MOD_CODIGO ");
        }

        #endregion

        #region Métodos Protegidos Sobrescritos

        protected override void SetarSelect(string propriedade, int codigoDinamico, StringBuilder select, StringBuilder joins, StringBuilder groupBy, bool somenteContarNumeroRegistros, Dominio.ObjetosDeValor.Embarcador.MDFe.FiltroPesquisaMDFesAverbadosRelatorio filtroPesquisa)
        {
            switch (propriedade)
            {
                case "Codigo":
                    if (!select.Contains(" , "))
                    {
                        select.Append("MDFeAverbacao.MAB_CODIGO as Codigo,, ");
                        groupBy.Append("MDFeAverbacao.MAB_CODIGO, ");
                    }
                    break;
                case "Numero":
                    if (!select.Contains(" Numero, "))
                    {
                        select.Append("MDFe.MDF_NUMERO Numero, ");
                        groupBy.Append("MDFe.MDF_NUMERO, ");
                        SetarJoinsMDFe(joins);
                    }
                    break;

                case "Serie":
                    if (!select.Contains(" Serie, "))
                    {
                        select.Append("Serie.ESE_NUMERo Serie, ");
                        groupBy.Append("Serie.ESE_NUMERo, ");
                        SetarJoinsEmpresaSerie(joins);
                    }
                    break;

                case "DataEmissaoFormatada":
                    if (!select.Contains(" DataEmissao, "))
                    {
                        select.Append("MDFe.MDF_DATA_EMISSAO DataEmissao, ");
                        groupBy.Append("MDFe.MDF_DATA_EMISSAO, ");
                        SetarJoinsMDFe(joins);
                    }
                    break;

                case "Situacao":
                case "DescricaoSituacao":
                    if (!select.Contains(" Situacao, "))
                    {
                        select.Append("MDFeAverbacao.MAB_STATUS Situacao, ");
                        groupBy.Append("MDFeAverbacao.MAB_STATUS, ");
                    }
                    break;

                case "Retorno":
                    if (!select.Contains(" Retorno, "))
                    {
                        select.Append("MDFeAverbacao.MAB_MENSAGEM_RETORNO Retorno, ");
                        groupBy.Append("MDFeAverbacao.MAB_MENSAGEM_RETORNO, ");
                    }
                    break;

                case "EstadoCarregamento":
                    if (!select.Contains(" EstadoCarregamento, "))
                    {
                        select.Append("MDFe.UF_CARREGAMENTO EstadoCarregamento, ");
                        groupBy.Append("MDFe.UF_CARREGAMENTO, ");
                        SetarJoinsMDFe(joins);
                    }
                    break;

                case "EstadoDescarregamento":
                    if (!select.Contains(" EstadoDescarregamento, "))
                    {
                        select.Append("MDFe.UF_DESCARREGAMENTO EstadoDescarregamento, ");
                        groupBy.Append("MDFe.UF_DESCARREGAMENTO, ");
                        SetarJoinsMDFe(joins);
                    }
                    break;

                case "Carga":
                    if (!select.Contains(" Carga, "))
                    {
                        select.Append("Carga.CAR_CODIGO_CARGA_EMBARCADOR Carga, ");
                        groupBy.Append("Carga.CAR_CODIGO_CARGA_EMBARCADOR, ");
                        SetarJoinsCarga(joins);
                    }
                    break;

                case "TipoCarga":
                    if (!select.Contains(" TipoCarga, "))
                    {
                        select.Append("TipoCarga.TCG_DESCRICAO TipoCarga, ");
                        groupBy.Append("TipoCarga.TCG_DESCRICAO, ");
                        SetarJoinsTipoCarga(joins);
                    }
                    break;

                case "TipoOperacao":
                    if (!select.Contains(" TipoOperacao, "))
                    {
                        select.Append("TipoOperacao.TOP_DESCRICAO TipoOperacao, ");
                        groupBy.Append("TipoOperacao.TOP_DESCRICAO, ");
                        SetarJoinsTipoOperacao(joins);
                    }
                    break;

                case "CNPJEmpresa":
                    if (!select.Contains(" CNPJEmpresa, "))
                    {
                        select.Append("Empresa.EMP_CNPJ CNPJEmpresa, ");
                        groupBy.Append("Empresa.EMP_CNPJ, ");
                        SetarJoinsEmpresa(joins);
                    }
                    break;

                case "NomeEmpresa":
                    if (!select.Contains(" NomeEmpresa, "))
                    {
                        select.Append("Empresa.EMP_FANTASIA NomeEmpresa, ");
                        groupBy.Append("Empresa.EMP_FANTASIA, ");
                        SetarJoinsEmpresa(joins);
                    }
                    break;

                case "Seguradora":
                    if (!select.Contains(" Seguradora, "))
                    {
                        select.Append("Seguradora.SEA_NOME Seguradora, ");
                        groupBy.Append("Seguradora.SEA_NOME, ");
                        SetarJoinsSeguradora(joins);
                    }
                    break;

                case "Averbadora":
                case "DescricaoAverbadora":
                    if (!select.Contains(" Averbadora, "))
                    {
                        select.Append("ApoliceSeguro.APS_SEGURADORA_AVERBACAO Averbadora, ");
                        groupBy.Append("ApoliceSeguro.APS_SEGURADORA_AVERBACAO, ");
                        SetarJoinsApoliceSeguro(joins);
                    }
                    break;

                case "Apolice":
                    if (!select.Contains(" Apolice, "))
                    {
                        select.Append("ApoliceSeguro.APS_NUMERO_APOLICE Apolice, ");
                        groupBy.Append("ApoliceSeguro.APS_NUMERO_APOLICE, ");
                        SetarJoinsApoliceSeguro(joins);
                    }
                    break;

                case "Averbacao":
                    if (!select.Contains(" Averbacao, "))
                    {
                        select.Append("MDFeAverbacao.MAB_PROTOCOLO Averbacao, ");
                        groupBy.Append("MDFeAverbacao.MAB_PROTOCOLO, ");
                    }
                    break;

                case "DataAverbacaoFormatada":
                    if (!select.Contains(" DataAverbacao, "))
                    {
                        select.Append("MDFeAverbacao.MAB_DATA_RETORNO DataAverbacao, ");
                        groupBy.Append("MDFeAverbacao.MAB_DATA_RETORNO, ");
                    }
                    break;

                case "SituacaoMDFe":
                case "DescricaoSituacaoMDFe":
                    if (!select.Contains(" SituacaoMDFe, "))
                    {
                        select.Append("MDFe.MDF_STATUS SituacaoMDFe, ");
                        groupBy.Append("MDFe.MDF_STATUS, ");
                        SetarJoinsMDFe(joins);
                    }
                    break;

                case "ValorMercadoria":
                    if (!select.Contains(" ValorMercadoria, "))
                    {
                        select.Append("MDFe.MDF_VALOR_TOTAL ValorMercadoria, ");
                        groupBy.Append("MDFe.MDF_VALOR_TOTAL, ");
                        SetarJoinsMDFe(joins);
                    }
                    break;

                case "Peso":
                    if (!select.Contains(" Peso, "))
                    {
                        select.Append("MDFe.MDF_PESO_BRUTO Peso, ");
                        groupBy.Append("MDFe.MDF_PESO_BRUTO, ");
                        SetarJoinsMDFe(joins);
                    }
                    break;
                case "Veiculos":
                    if (!select.Contains(" Veiculos, "))
                    {
                        select.Append("isnull(SUBSTRING((select ', ' + Veiculo.MDV_PLACA from T_MDFE_VEICULO Veiculo where Veiculo.MDF_CODIGO = Mdfe.MDF_CODIGO for XML PATH('')), 3, 1000), '') Veiculos, ");
                        groupBy.Append("Mdfe.MDF_CODIGO, ");
                    }
                    break;
                case "Motoristas":
                    if (!select.Contains(" Motoristas, "))
                    {
                        select.Append("isnull(SUBSTRING((select ', ' + Motorista.MDM_NOME from T_MDFE_MOTORISTA Motorista where Motorista.MDF_CODIGO = Mdfe.MDF_CODIGO for XML PATH('')), 3, 1000), '') as Motoristas,");
                        groupBy.Append("Mdfe.MDF_CODIGO, ");
                    }
                    break;

            }
        }

        protected override void SetarWhere(Dominio.ObjetosDeValor.Embarcador.MDFe.FiltroPesquisaMDFesAverbadosRelatorio filtrosPesquisa, StringBuilder where, StringBuilder joins, StringBuilder groupBy, List<Embarcador.Consulta.ParametroSQL> parametros = null)
        {

            if (filtrosPesquisa.DataInicialEmissao != DateTime.MinValue)
                where.Append($" AND MDFe.MDF_DATA_EMISSAO >= '{filtrosPesquisa.DataInicialEmissao.ToString("yyyy-MM-dd")}'");

            if (filtrosPesquisa.DataFinalEmissao != DateTime.MinValue)
                where.Append($" AND MDFe.MDF_DATA_EMISSAO <= '{filtrosPesquisa.DataFinalEmissao.ToString("yyyy-MM-dd")}'");

            if (filtrosPesquisa.Status.HasValue)
                where.Append($" AND MDFeAverbacao.MAB_STATUS = {filtrosPesquisa.Status.Value.ToString("D")}");

            if (filtrosPesquisa.CodigoTransportador > 0)
                where.Append($" AND Empresa.EMP_CODIGO = '{filtrosPesquisa.CodigoTransportador}'");

            if (filtrosPesquisa.CodigoSeguradora > 0)
                where.Append($" AND Seguradora.SEA_CODIGO = '{filtrosPesquisa.CodigoSeguradora}'");

            if (filtrosPesquisa.CodigoSeguradora > 0)
                where.Append($" AND Seguradora.SEA_CODIGO = '{filtrosPesquisa.CodigoSeguradora}'");

            if (filtrosPesquisa.CodigoModeloDocumentoFiscal > 0)
                where.Append($" AND ModeloDocumentoFiscal.MOD_CODIGO = '{filtrosPesquisa.CodigoModeloDocumentoFiscal}'");

            if (filtrosPesquisa.CodigosFiliais.Any(codigo => codigo == -1))
            {
                where.Append($@" and (Carga.FIL_CODIGO in ({string.Join(", ", filtrosPesquisa.CodigosFiliais)}) OR EXISTS (   SELECT _cargaPedidoRecebedor.CAR_CODIGO 
                                                                                                                       FROM T_CARGA_PEDIDO _cargaPedidoRecebedor 
                                                                                                                       LEFT JOIN T_PEDIDO _pedido ON _pedido.PED_CODIGO = _cargaPedidoRecebedor.PED_CODIGO
                                                                                                                       WHERE Carga.CAR_CODIGO = _cargaPedidoRecebedor.CAR_CODIGO
                                                                                                                       AND _pedido.CLI_CODIGO_RECEBEDOR IN ({string.Join(",", filtrosPesquisa.CodigosRecebedores)})))");
                SetarJoinsCarga(joins);
            }

        }

        #endregion
    }
}
