using Dominio.ObjetosDeValor.Embarcador.TorreControle;
using Repositorio.Embarcador.Consulta;
using System;
using System.Collections.Generic;
using System.Text;

namespace Repositorio.Embarcador.TorreControle.Consulta
{
    public class ConsultaDevolucaoNotasFiscais : Embarcador.Consulta.Consulta<Dominio.ObjetosDeValor.Embarcador.TorreControle.FiltroPesquisaDevolucaoNotasFiscais>
    {
        #region Construtores

        public ConsultaDevolucaoNotasFiscais() : base(tabela: "T_XML_NOTA_FISCAL as NotaFiscal") { }

        #endregion

        protected override SQLDinamico ObterSql(FiltroPesquisaDevolucaoNotasFiscais filtrosPesquisa, Dominio.ObjetosDeValor.Embarcador.Consulta.ParametroConsulta parametrosConsulta, List<Dominio.Relatorios.Embarcador.ObjetosDeValor.PropriedadeAgrupamento> propriedades, bool somenteContarNumeroRegistros)
        {
            StringBuilder groupBy = new StringBuilder();
            StringBuilder joins = new StringBuilder();
            StringBuilder orderBy = new StringBuilder();
            StringBuilder select = new StringBuilder();
            StringBuilder sql = new StringBuilder();
            StringBuilder where = new StringBuilder();
            List<ParametroSQL> parametrosWhere = new List<ParametroSQL>();


            foreach (Dominio.Relatorios.Embarcador.ObjetosDeValor.PropriedadeAgrupamento propriedade in propriedades)
                SetarSelect(propriedade.Propriedade, propriedade.CodigoDinamico, select, joins, groupBy, somenteContarNumeroRegistros, filtrosPesquisa);

            SetarOrderBy(parametrosConsulta, select, orderBy, joins, groupBy, somenteContarNumeroRegistros, filtrosPesquisa);
            SetarWhere(filtrosPesquisa, where, joins, groupBy, parametrosWhere);

            string campos = select.ToString().Trim();
            string agrupamentos = groupBy.ToString().Trim();
            string condicoes = where.ToString().Trim();

            if (somenteContarNumeroRegistros)
                sql.Append("select distinct(count(0) over ()) ");
            else
                sql.Append($"select {(_somenteRegistrosDistintos ? "distinct " : "")}{(campos.Length > 0 ? campos.Substring(0, campos.Length - 1) : "")} ");

            sql.Append($" from {_tabela} ");
            sql.Append(joins.ToString());

            if (condicoes.Length > 0)
                sql.Append($" where {condicoes.Substring(4)} ");

            if (agrupamentos.Length > 0)
                sql.Append($" group by {agrupamentos.Substring(0, agrupamentos.Length - 1)} ");

            if (!somenteContarNumeroRegistros)
            {
                sql.Append($" order by NotaFiscal.NFX_CODIGO desc, Chamado.CHA_CODIGO ");

                if ((parametrosConsulta != null) && ((parametrosConsulta.InicioRegistros > 0) || (parametrosConsulta.LimiteRegistros > 0)))
                    sql.Append($" offset {parametrosConsulta.InicioRegistros} rows fetch next {parametrosConsulta.LimiteRegistros} rows only;");
            }

            return new SQLDinamico(sql.ToString(), parametrosWhere);
        }

        private void SetarJoinPedidoNotaFiscal(StringBuilder joins)
        {
            if (!joins.Contains(" PedidoNotaFiscal "))
                joins.AppendLine(" INNER JOIN T_PEDIDO_XML_NOTA_FISCAL AS PedidoNotaFiscal ON PedidoNotaFiscal.NFX_CODIGO = NotaFiscal.NFX_CODIGO ");
        }

        private void SetarJoinsCargaEntregaNotaFiscal(StringBuilder joins)
        {
            SetarJoinPedidoNotaFiscal(joins);
            if (!joins.Contains(" CargaEntregaNotaFiscal "))
                joins.AppendLine(" LEFT JOIN T_CARGA_ENTREGA_NOTA_FISCAL AS CargaEntregaNotaFiscal ON CargaEntregaNotaFiscal.PNF_CODIGO = PedidoNotaFiscal.PNF_CODIGO ");
        }

        private void SetarJoinsCargaEntregaProduto(StringBuilder joins)
        {
            SetarJoinPedidoNotaFiscal(joins);
            SetarJoinsCargaEntrega(joins);
            if (!joins.Contains(" CargaEntregaProduto "))
                joins.AppendLine(" LEFT JOIN T_CARGA_ENTREGA_PRODUTO AS CargaEntregaProduto ON CargaEntregaProduto.NFX_CODIGO = PedidoNotaFiscal.NFX_CODIGO AND CargaEntregaProduto.CEN_CODIGO = CargaEntrega.CEN_CODIGO ");
        }

        private void SetarJoinsProduto(StringBuilder joins)
        {
            SetarJoinsCargaEntregaProduto(joins);
            if (!joins.Contains(" Produto "))
                joins.AppendLine(" LEFT JOIN T_PRODUTO_EMBARCADOR AS Produto ON Produto.PRO_CODIGO = CargaEntregaProduto.PRO_CODIGO ");
        }

        private void SetarJoinsCargaPedido(StringBuilder joins)
        {
            SetarJoinPedidoNotaFiscal(joins);
            if (!joins.Contains(" CargaPedido "))
                joins.AppendLine(" LEFT JOIN T_CARGA_PEDIDO AS CargaPedido ON CargaPedido.CPE_CODIGO = PedidoNotaFiscal.CPE_CODIGO ");
        }

        private void SetarJoinsPedido(StringBuilder joins)
        {
            SetarJoinsCargaPedido(joins);
            if (!joins.Contains(" Pedido "))
                joins.AppendLine(" LEFT JOIN T_PEDIDO AS Pedido ON Pedido.PED_CODIGO = CargaPedido.PED_CODIGO ");
        }

        private void SetarJoinsCargaEntrega(StringBuilder joins)
        {
            SetarJoinsCargaEntregaNotaFiscal(joins);
            if (!joins.Contains(" CargaEntrega "))
                joins.AppendLine("LEFT JOIN T_CARGA_ENTREGA AS CargaEntrega ON CargaEntrega.CEN_CODIGO = CargaEntregaNotaFiscal.CEN_CODIGO AND CargaEntrega.CEN_COLETA = 0 ");
        }

        private void SetarJoinsCarga(StringBuilder joins)
        {
            SetarJoinsCargaEntrega(joins);
            if (!joins.Contains(" Carga "))
                joins.AppendLine(" LEFT JOIN T_CARGA AS Carga ON Carga.CAR_CODIGO = CargaEntrega.CAR_CODIGO ");
        }

        private void SetarJoinsChamadoNotaFiscal(StringBuilder joins)
        {
            if (!joins.Contains(" ChamadoNotaFiscal "))
                joins.AppendLine(" INNER JOIN T_CHAMADO_XML_NOTA_FISCAL AS ChamadoNotaFiscal ON ChamadoNotaFiscal.NFX_CODIGO = NotaFiscal.NFX_CODIGO ");
        }

        private void SetarJoinsChamado(StringBuilder joins)
        {
            SetarJoinsChamadoNotaFiscal(joins);
            SetarJoinsCargaEntrega(joins);
            if (!joins.Contains(" Chamado "))
                joins.AppendLine(" INNER JOIN T_CHAMADOS AS Chamado ON Chamado.CHA_CODIGO = ChamadoNotaFiscal.CHA_CODIGO AND Chamado.CEN_CODIGO = CargaEntrega.CEN_CODIGO ");
        }

        private void SetarJoinsMotivoChamado(StringBuilder joins)
        {
            SetarJoinsChamado(joins);
            if (!joins.Contains(" MotivoChamado "))
                joins.AppendLine(" LEFT JOIN T_MOTIVO_CHAMADA AS MotivoChamado ON MotivoChamado.MCH_CODIGO = Chamado.MCH_CODIGO ");
        }

        private void SetarJoinsTipoOperacao(StringBuilder joins)
        {
            SetarJoinsCarga(joins);
            if (!joins.Contains(" TipoOperacao "))
                joins.AppendLine(" LEFT JOIN T_TIPO_OPERACAO AS TipoOperacao ON TipoOperacao.TOP_CODIGO = Carga.TOP_CODIGO ");
        }

        private void SetarJoinsGrupoTipoOperacao(StringBuilder joins)
        {
            SetarJoinsTipoOperacao(joins);
            if (!joins.Contains(" GrupoTipoOperacao "))
                joins.AppendLine(" LEFT JOIN T_GRUPO_TIPO_OPERACAO AS GrupoTipoOperacao ON GrupoTipoOperacao.GTO_CODIGO = TipoOperacao.GTO_CODIGO ");
        }

        private void SetarJoinsCargaEntregaNFEDevolucao(StringBuilder joins)
        {
            SetarJoinsChamado(joins);
            if (!joins.Contains(" CargaEntregaNFEDevolucao "))
                joins.AppendLine(" LEFT JOIN T_CARGA_ENTREGA_NFE_DEVOLUCAO AS CargaEntregaNFEDevolucao ON CargaEntregaNFEDevolucao.CHA_CODIGO = Chamado.CHA_CODIGO and CargaEntregaNFEDevolucao.NFX_CODIGO = NotaFiscal.NFX_CODIGO ");
        }

        private void SetarJoinsCliente(StringBuilder joins)
        {
            SetarJoinsCargaEntrega(joins);
            if (!joins.Contains(" Cliente "))
                joins.AppendLine(" LEFT JOIN T_CLIENTE AS Cliente ON Cliente.CLI_CGCCPF = CargaEntrega.CLI_CODIGO_ENTREGA ");
        }

        private void SetarJoinsFilial(StringBuilder joins)
        {
            SetarJoinsCarga(joins);
            if (!joins.Contains(" Filial "))
                joins.AppendLine(" LEFT JOIN T_FILIAL AS Filial ON Filial.FIL_CODIGO = Carga.FIL_CODIGO ");
        }

        private void SetarJoinsTransportador(StringBuilder joins)
        {
            SetarJoinsCarga(joins);
            if (!joins.Contains(" Transportador "))
                joins.AppendLine(" LEFT JOIN T_EMPRESA AS Transportador ON Transportador.EMP_CODIGO = Carga.EMP_CODIGO ");
        }

        private void SetarJoinsRemetente(StringBuilder joins)
        {
            if (!joins.Contains(" Remetente "))
                joins.AppendLine(" LEFT JOIN T_CLIENTE AS Remetente ON Remetente.CLI_CGCCPF = NotaFiscal.CLI_CODIGO_REMETENTE ");
        }

        private void SetarJoinsLocalidadeOrigem(StringBuilder joins)
        {
            SetarJoinsRemetente(joins);
            if (!joins.Contains(" LocalidadeOrigem "))
                joins.AppendLine(" LEFT JOIN T_LOCALIDADES AS LocalidadeOrigem ON LocalidadeOrigem.LOC_CODIGO = Remetente.LOC_CODIGO ");
        }

        private void SetarJoinsCargaEntregaProdutoChamado(StringBuilder joins)
        {
            SetarJoinsCargaEntregaProduto(joins);
            SetarJoinsChamado(joins);
            if (!joins.Contains(" CargaEntregaProdutoChamado "))
                joins.AppendLine(@" LEFT JOIN T_CARGA_ENTREGA_PRODUTO_CHAMADO AS CargaEntregaProdutoChamado 
                                            ON CargaEntregaProdutoChamado.CEN_CODIGO = CargaEntregaProduto.CEN_CODIGO
                                    AND CargaEntregaProdutoChamado.NFX_CODIGO = CargaEntregaProduto.NFX_CODIGO
                                    AND CargaEntregaProdutoChamado.PRO_CODIGO = CargaEntregaProduto.PRO_CODIGO
                                    and CargaEntregaProdutoChamado.CHA_CODIGO = Chamado.CHA_CODIGO ");
        }

        private void SetarJoinsCargaEntregaNotaFiscalChamado(StringBuilder joins)
        {
            SetarJoinsCargaEntregaNotaFiscal(joins);
            SetarJoinsChamado(joins);
            if (!joins.Contains(" CargaEntregaNotaFiscalChamado "))
                joins.AppendLine(@" LEFT JOIN T_CARGA_ENTREGA_NOTA_FISCAL_CHAMADO CargaEntregaNotaFiscalChamado 
	                           ON CargaEntregaNotaFiscalChamado.CEF_CODIGO = CargaEntregaNotaFiscal.CEF_CODIGO
	                           AND CargaEntregaNotaFiscalChamado.CHA_CODIGO = Chamado.CHA_CODIGO ");
        }

        protected override void SetarSelect(string propriedade, int codigoDinamico, StringBuilder select, StringBuilder joins, StringBuilder groupBy, bool somenteContarNumeroRegistros, FiltroPesquisaDevolucaoNotasFiscais filtrosPesquisa)
        {
            switch (propriedade)
            {
                case "NotaFiscalOrigem":
                    select.Append(" NotaFiscal.NF_NUMERO AS NotaFiscalOrigem, ");
                    break;

                case "GrupoTipoOperacao":
                    SetarJoinsGrupoTipoOperacao(joins);
                    select.Append(" GrupoTipoOperacao.GTO_DESCRICAO AS GrupoTipoOperacao, ");
                    break;

                case "TipoOperacao":
                    SetarJoinsTipoOperacao(joins);
                    select.Append(" TipoOperacao.TOP_DESCRICAO AS TipoOperacao, ");
                    break;

                case "Carga":
                    SetarJoinsCarga(joins);
                    select.Append(" Carga.CAR_CODIGO_CARGA_EMBARCADOR AS Carga, ");
                    break;

                case "Chamado":
                    SetarJoinsChamado(joins);
                    select.Append(" Chamado.CHA_NUMERO AS Chamado, ");
                    break;

                case "UFOrigemNota":
                    SetarJoinsLocalidadeOrigem(joins);
                    select.Append(" LocalidadeOrigem.UF_SIGLA AS UFOrigemNota, ");
                    break;

                case "CidadeOrigemNota":
                    SetarJoinsLocalidadeOrigem(joins);
                    select.Append(" LocalidadeOrigem.LOC_DESCRICAO AS CidadeOrigemNota, ");
                    break;

                case "Filial":
                    SetarJoinsFilial(joins);
                    select.Append(" Filial.FIL_DESCRICAO AS Filial, ");
                    break;

                case "DataEmissaoNFOrigem":
                case "DataEmissaoNFOrigemFormatada":
                    select.Append(" NotaFiscal.NF_DATA_EMISSAO AS DataEmissaoNFOrigem, ");
                    break;

                case "DataEmissaoNFD":
                case "DataEmissaoNFDFormatada":
                    SetarJoinsCargaEntregaNFEDevolucao(joins);
                    select.Append(" CargaEntregaNFEDevolucao.CND_DATA_EMISSAO AS DataEmissaoNFD, ");
                    break;

                case "DataFinalizacaoOcorrencia":
                case "DataFinalizacaoOcorrenciaFormatada":
                    SetarJoinsChamado(joins);
                    select.Append(" Chamado.CHA_DATA_FINALIZACAO AS DataFinalizacaoOcorrencia, ");
                    break;

                case "PedidoEmbarcador":
                    SetarJoinsPedido(joins);
                    select.Append(" Pedido.PED_NUMERO_PEDIDO_EMBARCADOR AS PedidoEmbarcador, ");
                    break;

                case "PedidoCliente":
                    SetarJoinsPedido(joins);
                    select.Append(" Pedido.PED_CODIGO_PEDIDO_CLIENTE AS PedidoCliente, ");
                    break;

                case "MotivoAtendimento":
                    SetarJoinsMotivoChamado(joins);
                    select.Append(" MotivoChamado.MCH_DESCRICAO AS MotivoAtendimento, ");
                    break;

                case "CNPJTransportadora":
                    SetarJoinsTransportador(joins);
                    select.Append(" Transportador.EMP_CNPJ AS CNPJTransportadora, ");
                    break;

                case "Transportadora":
                    SetarJoinsTransportador(joins);
                    select.Append(" Transportador.EMP_RAZAO AS Transportadora, ");
                    break;

                case "NumeroNotaDevolucao":
                    SetarJoinsCargaEntregaNFEDevolucao(joins);
                    select.Append(" CargaEntregaNFEDevolucao.CND_NUMERO AS NumeroNotaDevolucao, ");
                    break;

                case "TipoDevolucao":
                case "TipoDevolucaoFormatada":
                    SetarJoinsCargaEntregaNotaFiscalChamado(joins);
                    select.Append(" CargaEntregaNotaFiscalChamado.CNC_DEVOLUCAO_PARCIAL AS TipoDevolucao, ");
                    break;

                case "CodigoIntegracaoCliente":
                    SetarJoinsCliente(joins);
                    select.Append(" Cliente.CLI_CODIGO_INTEGRACAO AS CodigoIntegracaoCliente, ");
                    break;

                case "Cliente":
                    SetarJoinsCliente(joins);
                    select.Append(" Cliente.CLI_NOME AS Cliente, ");
                    break;

                case "ValorNotaFiscalOrigem":
                    select.Append(" NotaFiscal.NF_VALOR AS ValorNotaFiscalOrigem, ");
                    break;

                case "PesoNotaFiscalOrigem":
                    select.Append(" NotaFiscal.NF_PESO AS PesoNotaFiscalOrigem, ");
                    break;

                case "CodigoProduto":
                    SetarJoinsProduto(joins);
                    select.Append(" Produto.PRO_CODIGO_PRODUTO_EMBARCADOR AS CodigoProduto, ");
                    break;

                case "DescricaoProduto":
                    SetarJoinsProduto(joins);
                    select.Append(" Produto.GRP_DESCRICAO AS DescricaoProduto, ");
                    break;

                case "QuantidadeDevolvida":
                    SetarJoinsCargaEntregaProdutoChamado(joins);
                    select.Append(" CargaEntregaProdutoChamado.CPP_QUANTIDADE_DEVOLUCAO AS QuantidadeDevolvida, ");
                    break;

                case "PesoProdutoNFD":
                    SetarJoinsCargaEntregaProdutoChamado(joins);
                    select.Append(" (CargaEntregaProdutoChamado.CPP_PESO_UNITARIO * CargaEntregaProdutoChamado.CPP_QUANTIDADE_DEVOLUCAO) AS PesoProdutoNFD, ");
                    break;

                case "ValorProdutoNFD":
                    SetarJoinsCargaEntregaProdutoChamado(joins);
                    select.Append(" CargaEntregaProdutoChamado.CPP_VALOR_DEVOLUCAO AS ValorProdutoNFD, ");
                    break;
            }
        }

        protected override void SetarWhere(FiltroPesquisaDevolucaoNotasFiscais filtrosPesquisa, StringBuilder where, StringBuilder joins, StringBuilder groupBy, List<ParametroSQL> parametros = null)
        {
            string utcDateTimePattern = "yyyy-MM-dd HH:mm:ss";

            SetarJoinsMotivoChamado(joins);
            where.Append(" AND Chamado.CHA_CODIGO is not null and MotivoChamado.MCH_TIPO_MOTIVO_ATENDIMENTO = 1 ");

            if (filtrosPesquisa.DataInicialEmissaoNFD.HasValue && filtrosPesquisa.DataInicialEmissaoNFD.Value != DateTime.MinValue)
            {
                SetarJoinsCargaEntregaNFEDevolucao(joins);
                where.Append($" AND CargaEntregaNFEDevolucao.CND_DATA_EMISSAO >= '{filtrosPesquisa.DataInicialEmissaoNFD.Value.ToString(utcDateTimePattern)}' ");
            }

            if (filtrosPesquisa.DataFinalEmissaoNFD.HasValue && filtrosPesquisa.DataFinalEmissaoNFD.Value != DateTime.MinValue)
            {
                SetarJoinsCargaEntregaNFEDevolucao(joins);
                where.Append($" AND CargaEntregaNFEDevolucao.CND_DATA_EMISSAO <= '{filtrosPesquisa.DataFinalEmissaoNFD.Value.ToString(utcDateTimePattern)}' ");
            }

            if (filtrosPesquisa.DataInicialChamado.HasValue && filtrosPesquisa.DataInicialChamado.Value != DateTime.MinValue)
                where.Append($" AND Chamado.CHA_DATA_CRICAO >= '{filtrosPesquisa.DataInicialChamado.Value.ToString(utcDateTimePattern)}' ");

            if (filtrosPesquisa.DataFinalChamado.HasValue && filtrosPesquisa.DataFinalChamado.Value != DateTime.MinValue)
                where.Append($" AND Chamado.CHA_DATA_CRICAO <= '{filtrosPesquisa.DataFinalChamado.Value.ToString(utcDateTimePattern)}' ");

            if (!string.IsNullOrWhiteSpace(filtrosPesquisa.CodigosNotaFiscalDevolucao))
            {
                SetarJoinsCargaEntregaNFEDevolucao(joins);
                where.Append($" AND CargaEntregaNFEDevolucao.CND_NUMERO IN ({filtrosPesquisa.CodigosNotaFiscalDevolucao}) ");
            }

            if (filtrosPesquisa.CodigosNotaFiscalOrigem != null && filtrosPesquisa.CodigosNotaFiscalOrigem.Count > 0)
                where.Append($" AND NotaFiscal.NFX_CODIGO IN ({string.Join(", ", filtrosPesquisa.CodigosNotaFiscalOrigem)}) ");

            if (filtrosPesquisa.CodigosTipoOperacao != null && filtrosPesquisa.CodigosTipoOperacao.Count > 0)
            {
                SetarJoinsTipoOperacao(joins);
                where.Append($" AND TipoOperacao.TOP_CODIGO IN ({string.Join(", ", filtrosPesquisa.CodigosTipoOperacao)}) ");
            }

            if (filtrosPesquisa.CodigosGrupoTipoOperacao != null && filtrosPesquisa.CodigosGrupoTipoOperacao.Count > 0)
            {
                SetarJoinsGrupoTipoOperacao(joins);
                where.Append($" AND GrupoTipoOperacao.GTO_CODIGO IN ({string.Join(", ", filtrosPesquisa.CodigosGrupoTipoOperacao)}) ");
            }

            if (filtrosPesquisa.CodigosCargas != null && filtrosPesquisa.CodigosCargas.Count > 0)
            {
                SetarJoinsCarga(joins);
                where.Append($" AND Carga.CAR_CODIGO IN ({string.Join(", ", filtrosPesquisa.CodigosCargas)}) ");
            }

            if (filtrosPesquisa.CodigosChamados != null && filtrosPesquisa.CodigosChamados.Count > 0)
            {
                SetarJoinsChamado(joins);
                where.Append($" AND Chamado.CHA_CODIGO IN ({string.Join(", ", filtrosPesquisa.CodigosChamados)}) ");
            }

            if (filtrosPesquisa.CodigoTransportador > 0)
            {
                SetarJoinsTransportador(joins);
                where.Append($" AND Transportador.EMP_CODIGO = {filtrosPesquisa.CodigoTransportador} ");
            }

            if (filtrosPesquisa.CodigoCliente > 0)
            {
                SetarJoinsCliente(joins);
                where.Append($" AND Cliente.CLI_CODIGO = {filtrosPesquisa.CodigoCliente} ");
            }

            if (!string.IsNullOrWhiteSpace(filtrosPesquisa.PedidoEmbarcador))
            {
                SetarJoinsPedido(joins);
                where.Append($" AND Pedido.PED_NUMERO_PEDIDO_EMBARCADOR LIKE '%{filtrosPesquisa.PedidoEmbarcador}%' ");
            }

            if (!string.IsNullOrWhiteSpace(filtrosPesquisa.PedidoCliente))
            {
                SetarJoinsPedido(joins);
                where.Append($" AND Pedido.PED_CODIGO_PEDIDO_CLIENTE LIKE '%{filtrosPesquisa.PedidoCliente}%' ");
            }

        }
    }
}
