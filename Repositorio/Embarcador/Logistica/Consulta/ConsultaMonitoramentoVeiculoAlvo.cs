using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Repositorio.Embarcador.Logistica.Consulta
{
    sealed class ConsultaMonitoramentoVeiculoAlvo : Repositorio.Embarcador.Consulta.Consulta<Dominio.ObjetosDeValor.Embarcador.Logistica.FiltroPesquisaMonitoramentoVeiculoAlvo>
    {
        #region Construtores

        public ConsultaMonitoramentoVeiculoAlvo() : base(tabela: "T_CARGA as Carga") { }

        #endregion

        #region Métodos Privados 

        private void SetarJoinsCargaPedido(StringBuilder joins)
        {
            if (!joins.Contains(" CargaPedido "))
                joins.Append("join T_CARGA_PEDIDO CargaPedido on CargaPedido.CAR_CODIGO = Carga.CAR_CODIGO ");
        }

        private void SetarJoinsCliente(StringBuilder joins)
        {
            SetarJoinsPedido(joins);

            if (!joins.Contains(" ClienteRecebedor "))
            {
                joins.Append("left join T_CLIENTE ClienteRecebedor on CargaPedido.CLI_CODIGO_RECEBEDOR = ClienteRecebedor.CLI_CGCCPF and CargaPedido.PED_TIPO_EMISSA_CTE_PARTICIPANTES in (1, 4) ");
                joins.Append("left join T_CLIENTE ClienteDestinatario on Pedido.CLI_CODIGO = ClienteDestinatario.CLI_CGCCPF ");
            }
        }

        private void SetarJoinsEmpresa(StringBuilder joins)
        {
            SetarJoinsVeiculo(joins);

            if (!joins.Contains(" Empresa "))
                joins.Append("left join T_EMPRESA Empresa on Veiculo.EMP_CODIGO = Empresa.EMP_CODIGO ");
        }

        private void SetarJoinsLocalidadeLoja(StringBuilder joins)
        {
            SetarJoinsCliente(joins);

            if (!joins.Contains(" LocalidadeRecebedor "))
            {
                joins.Append("left join T_LOCALIDADES LocalidadeRecebedor on ClienteRecebedor.LOC_CODIGO = LocalidadeRecebedor.LOC_CODIGO  ");
                joins.Append("left join T_LOCALIDADES LocalidadeDestinatario on ClienteDestinatario.LOC_CODIGO = LocalidadeDestinatario.LOC_CODIGO ");
            }
        }

        private void SetarJoinsLocalidadeOrigem(StringBuilder joins)
        {
            SetarJoinsOrigem(joins);

            if (!joins.Contains(" LocalidadeOrigem "))
                joins.Append("left join T_LOCALIDADES LocalidadeOrigem on LocalidadeOrigem.LOC_CODIGO = Origem.LOC_CODIGO ");
        }

        private void SetarJoinsModeloCarroceria(StringBuilder joins)
        {
            SetarJoinsVeiculo(joins);

            if (!joins.Contains(" ModeloCarroceria "))
                joins.Append("left join T_MODELO_CARROCERIA ModeloCarroceria on ModeloCarroceria.MCA_CODIGO = Veiculo.MCA_CODIGO ");
        }

        private void SetarJoinsModeloVeicular(StringBuilder joins)
        {
            SetarJoinsVeiculo(joins);

            if (!joins.Contains(" ModeloVeicular "))
                joins.Append("left join T_MODELO_VEICULAR_CARGA ModeloVeicular on Veiculo.MVC_CODIGO = ModeloVeicular.MVC_CODIGO ");
        }

        private void SetarJoinsMonitoramento(StringBuilder joins)
        {
            if (!joins.Contains(" Monitoramento "))
                joins.Append("join T_MONITORAMENTO Monitoramento on Carga.Car_Codigo = Monitoramento.CAR_CODIGO ");
        }

        private void SetarJoinsOrigem(StringBuilder joins)
        {
            SetarJoinsRota(joins);

            if (!joins.Contains(" Origem "))
                joins.Append("left join T_CLIENTE Origem on Origem.CLI_CGCCPF = Rota.CLI_CGCCPF ");
        }

        private void SetarJoinsPedido(StringBuilder joins)
        {
            SetarJoinsCargaPedido(joins);

            if (!joins.Contains(" Pedido "))
                joins.Append("join T_PEDIDO Pedido on CargaPedido.PED_CODIGO = Pedido.PED_CODIGO ");
        }

        private void SetarJoinsRota(StringBuilder joins)
        {
            if (!joins.Contains(" Rota "))
                joins.Append("left join T_ROTA_FRETE Rota on Carga.ROF_CODIGO = Rota.ROF_CODIGO ");
        }

        private void SetarJoinsTipoOperacao(StringBuilder joins)
        {
            if (!joins.Contains(" TipoOperacao "))
                joins.Append("left join T_TIPO_OPERACAO TipoOperacao on TipoOperacao.TOP_CODIGO = Carga.TOP_CODIGO ");
        }

        private void SetarJoinsVeiculo(StringBuilder joins)
        {
            if (!joins.Contains(" Veiculo "))
                joins.Append("join T_VEICULO Veiculo on Veiculo.VEI_CODIGO = Carga.CAR_VEICULO ");
        }

        #endregion

        #region Métodos Protegidos Sobrescritos

        protected override void SetarSelect(string propriedade, int codigoDinamico, StringBuilder select, StringBuilder joins, StringBuilder groupBy, bool somenteContarNumeroRegistros, Dominio.ObjetosDeValor.Embarcador.Logistica.FiltroPesquisaMonitoramentoVeiculoAlvo filtroPesquisa)
        {
            switch (propriedade)
            {
                case "Codigo":
                    if (!select.Contains(" Codigo, "))
                    {
                        select.Append("Carga.CAR_CODIGO as Codigo, ");
                        groupBy.Append("Carga.CAR_CODIGO, ");
                    }
                    break;

                case "DataDaRotaFormatada":
                    if (!select.Contains(" DataDaRota, "))
                    {
                        select.Append("Rota.ROF_DATA_ROTEIRIZACAO as DataDaRota, ");
                        groupBy.Append("Rota.ROF_DATA_ROTEIRIZACAO, ");

                        SetarJoinsRota(joins);
                    }
                    break;

                case "SM":
                    if (!select.Contains(" SM, "))
                    {
                        select.Append("Monitoramento.MON_CODIGO as SM, ");
                        groupBy.Append("Monitoramento.MON_CODIGO, ");

                        SetarJoinsMonitoramento(joins);
                    }
                    break;

                case "PlacaVeiculo":
                    if (!select.Contains(" PlacaVeiculo, "))
                    {
                        select.Append("Veiculo.VEI_PLACA as PlacaVeiculo, ");
                        groupBy.Append("Veiculo.VEI_PLACA, ");

                        SetarJoinsVeiculo(joins);
                    }
                    break;

                case "TipoVeiculo":
                    if (!select.Contains(" TipoVeiculo, "))
                    {
                        select.Append("ModeloVeicular.MVC_DESCRICAO as TipoVeiculo, ");
                        groupBy.Append("ModeloVeicular.MVC_DESCRICAO, ");

                        SetarJoinsModeloVeicular(joins);
                    }
                    break;

                case "TipoBau":
                    if (!select.Contains(" TipoBau, "))
                    {
                        select.Append("ModeloCarroceria.MCA_DESCRICAO as TipoBau, ");
                        groupBy.Append("ModeloCarroceria.MCA_DESCRICAO, ");

                        SetarJoinsModeloCarroceria(joins);
                    }
                    break;

                case "CD":
                    if (!select.Contains(" CD, "))
                    {
                        select.Append("Origem.CLI_NOME as CD, ");
                        groupBy.Append("Origem.CLI_NOME, ");
                        
                        SetarJoinsOrigem(joins);
                    }
                    break;

                case "UFOrigem":
                    if (!select.Contains(" UFOrigem, "))
                    {
                        select.Append("LocalidadeOrigem.UF_SIGLA as UFOrigem, ");
                        groupBy.Append("LocalidadeOrigem.UF_SIGLA, ");

                        SetarJoinsLocalidadeOrigem(joins);
                    }
                    break;

                case "Regiao":
                    if (!select.Contains(" Regiao, "))
                    {
                        //verificar
                    }
                    break;

                case "DataSaidaCDFormatada":
                    if (!select.Contains(" DataSaidaCD, "))
                    {
                        select.Append("Monitoramento.MON_DATA_INICIO as DataSaidaCD, ");
                        groupBy.Append("Monitoramento.MON_DATA_INICIO, ");

                        SetarJoinsMonitoramento(joins);
                    }
                    break;

                case "Loja":
                    if (!select.Contains(" Loja, "))
                    {
                        select.Append(@"case when ClienteRecebedor.CLI_CGCCPF is not null then ClienteRecebedor.CLI_NOME else ClienteDestinatario.CLI_NOME end Loja, ");
                        groupBy.Append(@"case when ClienteRecebedor.CLI_CGCCPF is not null then ClienteRecebedor.CLI_NOME else ClienteDestinatario.CLI_NOME end, ");

                        SetarJoinsCliente(joins);
                    }
                    break;

                case "UFDestino":
                    if (!select.Contains(" UFDestino, "))
                    {
                        select.Append(@"case when ClienteRecebedor.CLI_CGCCPF is not null then LocalidadeRecebedor.UF_SIGLA else LocalidadeDestinatario.UF_SIGLA end UFDestino, ");
                        groupBy.Append(@"case when ClienteRecebedor.CLI_CGCCPF is not null then LocalidadeRecebedor.UF_SIGLA else LocalidadeDestinatario.UF_SIGLA end, ");

                        SetarJoinsLocalidadeLoja(joins);
                    }
                    break;

                case "DataEntradaLojaFormatada":
                    if (!select.Contains(" DataEntradaLoja, "))
                    {
                        select.Append("CargaPedido.PED_DATA_CHEGADA as DataEntradaLoja, ");
                        groupBy.Append("CargaPedido.PED_DATA_CHEGADA, ");

                        SetarJoinsCargaPedido(joins);
                    }
                    break;

                case "DataSaidaLojaFormatada":
                    if (!select.Contains(" DataSaidaLoja, "))
                    {
                        select.Append("CargaPedido.PED_DATA_SAIDA as DataSaidaLoja, ");
                        groupBy.Append("CargaPedido.PED_DATA_SAIDA, ");

                        SetarJoinsCargaPedido(joins);
                    }
                    break;

                case "PedidoCliente":
                    if (!select.Contains(" PedidoCliente, "))
                    {
                        select.Append("Pedido.PED_NUMERO as PedidoCliente, ");
                        groupBy.Append("Pedido.PED_NUMERO, ");

                        SetarJoinsPedido(joins);
                    }
                    break;

                case "TipoDeTransporte":
                    if (!select.Contains(" TipoDeTransporte, "))
                    {
                        select.Append("TipoOperacao.TOP_DESCRICAO as TipoDeTransporte, ");
                        groupBy.Append("TipoOperacao.TOP_DESCRICAO, ");

                        SetarJoinsTipoOperacao(joins);
                    }
                    break;

                case "Transportador":
                    if (!select.Contains(" Transportador, "))
                    {
                        select.Append("Empresa.EMP_RAZAO as Transportador, ");
                        groupBy.Append("Empresa.EMP_RAZAO, ");

                        SetarJoinsEmpresa(joins);
                    }
                    break;

                case "NFS":
                    if (!select.Contains(" NFS, "))
                    {
                        select.Append("(select count(*) From T_PEDIDO_XML_NOTA_FISCAL Nota where Nota.CPE_CODIGO = CargaPedido.CPE_CODIGO) NFS, ");
                        groupBy.Append("CargaPedido.CPE_CODIGO, ");

                        SetarJoinsCargaPedido(joins);
                    }
                    break;
            }
        }

        protected override void SetarWhere(Dominio.ObjetosDeValor.Embarcador.Logistica.FiltroPesquisaMonitoramentoVeiculoAlvo filtrosPesquisa, StringBuilder where, StringBuilder joins, StringBuilder groupBy, List<Embarcador.Consulta.ParametroSQL> parametros = null)
        {
            SetarJoinsCargaPedido(joins);
            SetarJoinsVeiculo(joins);
            SetarJoinsMonitoramento(joins);

            where.Append($" and CargaPedido.PED_DATA_CHEGADA is not null and CargaPedido.PED_DATA_SAIDA is null");

            if (filtrosPesquisa.CodigoCargaEmbarcador != "")
                where.Append($" and Carga.CAR_CODIGO_CARGA_EMBARCADOR = '{filtrosPesquisa.CodigoCargaEmbarcador}'");

            if (filtrosPesquisa.PlacaVeiculo != "")
                where.Append($" and Veiculo.VEI_PLACA like '%{filtrosPesquisa.PlacaVeiculo}%'");

            if (filtrosPesquisa.DataInicial > DateTime.MinValue)
                where.Append($" and Monitoramento.Mon_Data_Inicio >= '{filtrosPesquisa.DataInicial.Value.ToString("yyyyMMdd HH:mm:ss")}'");

            if (filtrosPesquisa.DataFinal > DateTime.MinValue)
                where.Append($" and Monitoramento.Mon_Data_Fim <= '{filtrosPesquisa.DataFinal.Value.ToString("yyyyMMdd HH:mm:ss")}'");
        }

        #endregion
    }
}

