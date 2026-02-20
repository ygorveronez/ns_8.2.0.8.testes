using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Repositorio.Embarcador.Cargas
{
    sealed class ConsultaPreCarga : Consulta.Consulta<Dominio.ObjetosDeValor.Embarcador.Carga.FiltroPesquisaRelatorioPreCarga>
    {
        #region Construtores

        public ConsultaPreCarga() : base(tabela: "T_PRE_CARGA as PreCarga") { }

        #endregion

        #region Métodos Privados

        private void SetarJoinsCarga(StringBuilder joins)
        {
            if (!joins.Contains(" Carga "))
                joins.Append(" left join T_CARGA Carga on Carga.CAR_CODIGO = PreCarga.CAR_CODIGO ");
        }

        private void SetarJoinsConfiguracaoProgramacaoCarga(StringBuilder joins)
        {
            if (!joins.Contains(" ConfiguracaoProgramacaoCarga "))
                joins.Append(" left join T_CONFIGURACAO_PROGRAMACAO_CARGA ConfiguracaoProgramacaoCarga on ConfiguracaoProgramacaoCarga.CPC_CODIGO = PreCarga.CPC_CODIGO ");
        }

        private void SetarJoinsFilial(StringBuilder joins)
        {
            if (!joins.Contains(" Filial "))
                joins.Append(" left join T_FILIAL Filial on Filial.FIL_CODIGO = PreCarga.FIL_CODIGO ");
        }

        private void SetarJoinsModeloVeicularCarga(StringBuilder joins)
        {
            if (!joins.Contains(" ModeloVeiculo "))
                joins.Append(" left join T_MODELO_VEICULAR_CARGA ModeloVeiculo on ModeloVeiculo.MVC_CODIGO = PreCarga.MVC_CODIGO ");
        }

        private void SetarJoinsOperador(StringBuilder joins)
        {
            if (!joins.Contains(" Operador "))
                joins.Append(" left join T_FUNCIONARIO Operador on Operador.FUN_CODIGO = PreCarga.PCA_OPERADOR ");
        }

        private void SetarJoinsTipoCarga(StringBuilder joins)
        {
            if (!joins.Contains(" TipoCarga "))
                joins.Append(" left join T_TIPO_DE_CARGA TipoCarga on TipoCarga.TCG_CODIGO = PreCarga.TCG_CODIGO ");
        }

        private void SetarJoinsTipoOperacao(StringBuilder joins)
        {
            if (!joins.Contains(" TipoOperacao "))
                joins.Append(" left join T_TIPO_OPERACAO TipoOperacao on TipoOperacao.TOP_CODIGO = PreCarga.TOP_CODIGO ");
        }

        private void SetarJoinsTransportador(StringBuilder joins)
        {
            if (!joins.Contains(" Transportador "))
                joins.Append(" left join T_EMPRESA Transportador on Transportador.EMP_CODIGO = PreCarga.EMP_CODIGO ");
        }

        private void SetarJoinsFaixaTemperatura(StringBuilder joins)
        {
            if (!joins.Contains(" FaixaTemperatura "))
                joins.Append(" left join T_FAIXA_TEMPERATURA FaixaTemperatura on FaixaTemperatura.FTE_CODIGO = PreCarga.FTE_CODIGO ");
        }

        #endregion

        #region Métodos Protegidos Sobrescritos

        protected override void SetarSelect(string propriedade, int codigoDinamico, StringBuilder select, StringBuilder joins, StringBuilder groupBy, bool somenteContarNumeroRegistros, Dominio.ObjetosDeValor.Embarcador.Carga.FiltroPesquisaRelatorioPreCarga filtroPesquisa)
        {
            switch (propriedade)
            {
                case "Codigo":
                    if (!select.Contains(" Codigo, "))
                    {
                        select.Append("PreCarga.PCA_CODIGO Codigo, ");

                        if (!groupBy.Contains("PreCarga.PCA_CODIGO,"))
                            groupBy.Append("PreCarga.PCA_CODIGO, ");
                    }
                    break;

                case "NumeroPreCarga":
                    if (!select.Contains(" NumeroPreCarga, "))
                    {
                        select.Append("PreCarga.PCA_NUMERO_CARGA NumeroPreCarga, ");
                        groupBy.Append("PreCarga.PCA_NUMERO_CARGA, ");
                    }
                    break;

                case "NumeroCarga":
                    if (!select.Contains(" NumeroCarga, "))
                    {
                        select.Append("Carga.CAR_CODIGO_CARGA_EMBARCADOR NumeroCarga, ");
                        groupBy.Append("Carga.CAR_CODIGO_CARGA_EMBARCADOR, ");

                        SetarJoinsCarga(joins);
                    }
                    break;

                case "DocaCarregamento":
                    if (!select.Contains(" DocaCarregamento, "))
                    {
                        select.Append("PreCarga.PCA_DOCA_CARREGAMENTO DocaCarregamento, ");
                        groupBy.Append("PreCarga.PCA_DOCA_CARREGAMENTO, ");
                    }
                    break;

                case "DataPrevisaoInicioViagemFormatada":
                    if (!select.Contains(" DataPrevisaoInicioViagem, "))
                    {
                        select.Append("PreCarga.CAR_DATA_PREVISAO_INICIO_VIAGEM DataPrevisaoInicioViagem, ");
                        groupBy.Append("PreCarga.CAR_DATA_PREVISAO_INICIO_VIAGEM, ");
                    }
                    break;

                case "PrevisaoChegadaDocaFormatada":
                    if (!select.Contains(" PrevisaoChegadaDoca, "))
                    {
                        select.Append("PreCarga.PCA_PREVISAO_CHEGADA_DOCA PrevisaoChegadaDoca, ");
                        groupBy.Append("PreCarga.PCA_PREVISAO_CHEGADA_DOCA, ");
                    }
                    break;

                case "DataCriacaoPreCargaFormatada":
                    if (!select.Contains(" DataCriacaoPreCarga, "))
                    {
                        select.Append("PreCarga.PCA_DATA_CRIACAO DataCriacaoPreCarga, ");
                        groupBy.Append("PreCarga.PCA_DATA_CRIACAO, ");
                    }
                    break;

                case "CNPJTransportadorFormatado":
                    if (!select.Contains(" CNPJTransportador, "))
                    {
                        select.Append("Transportador.EMP_CNPJ CNPJTransportador, ");
                        groupBy.Append("Transportador.EMP_CNPJ, ");

                        SetarJoinsTransportador(joins);
                    }
                    break;

                case "Transportador":
                    if (!select.Contains(" Transportador, "))
                    {
                        select.Append("Transportador.EMP_RAZAO Transportador, ");
                        groupBy.Append("Transportador.EMP_RAZAO, ");

                        SetarJoinsTransportador(joins);
                    }
                    break;

                case "Filial":
                    if (!select.Contains(" Filial, "))
                    {
                        select.Append("Filial.FIL_DESCRICAO Filial, ");
                        groupBy.Append("Filial.FIL_DESCRICAO, ");

                        SetarJoinsFilial(joins);
                    }
                    break;

                case "ModeloVeiculo":
                    if (!select.Contains(" ModeloVeiculo, "))
                    {
                        select.Append("ModeloVeiculo.MVC_DESCRICAO ModeloVeiculo, ");
                        groupBy.Append("ModeloVeiculo.MVC_DESCRICAO, ");

                        SetarJoinsModeloVeicularCarga(joins);
                    }
                    break;

                case "Operador":
                    if (!select.Contains(" Operador, "))
                    {
                        select.Append("Operador.FUN_NOME Operador, ");
                        groupBy.Append("Operador.FUN_NOME, ");

                        SetarJoinsOperador(joins);
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

                case "FaixaTemperatura":
                    if (!select.Contains(" FaixaTemperatura, "))
                    {
                        select.Append("FaixaTemperatura.FTE_DESCRICAO FaixaTemperatura, ");
                        groupBy.Append("FaixaTemperatura.FTE_DESCRICAO, ");

                        SetarJoinsFaixaTemperatura(joins);
                    }
                    break;

                case "NumeroPedidos":
                    if (!select.Contains(" NumeroPedidos, "))
                    {
                        select.Append("substring((");
                        select.Append("    select ', ' + _pedido.PED_NUMERO_PEDIDO_EMBARCADOR ");
                        select.Append("      from T_PEDIDO _pedido ");
                        select.Append("     where _pedido.PCA_CODIGO = PreCarga.PCA_CODIGO ");
                        select.Append("       for xml path('') ");
                        select.Append("), 3, 1000) NumeroPedidos, ");

                        if (!groupBy.Contains("PreCarga.PCA_CODIGO,"))
                            groupBy.Append("PreCarga.PCA_CODIGO, ");
                    }
                    break;

                case "CNPJRemetente":
                    if (!select.Contains(" CNPJRemetente, "))
                    {
                        select.Append("substring((");
                        select.Append("    select distinct ', ' + LTRIM(STR(remetente.CLI_CGCCPF, 20, 0)) ");
                        select.Append("      from T_CLIENTE remetente ");
                        select.Append("      join T_PEDIDO _pedido on remetente.CLI_CGCCPF = _pedido.CLI_CODIGO_REMETENTE ");
                        select.Append("     where _pedido.PCA_CODIGO = PreCarga.PCA_CODIGO ");
                        select.Append("       for xml path('') ");
                        select.Append("), 3, 1000) CNPJRemetente, ");

                        if (!groupBy.Contains("PreCarga.PCA_CODIGO,"))
                            groupBy.Append("PreCarga.PCA_CODIGO, ");
                    }
                    break;

                case "Remetente":
                    if (!select.Contains(" Remetente, "))
                    {
                        select.Append("substring((");
                        select.Append("    select distinct ', ' + (case when (remetente.CLI_CODIGO_INTEGRACAO is not null and remetente.CLI_CODIGO_INTEGRACAO <> '') then (remetente.CLI_CODIGO_INTEGRACAO + ' - ') ELSE '' END) ");
                        select.Append("      + remetente.CLI_NOME ");
                        select.Append("      from T_CLIENTE remetente ");
                        select.Append("      join T_PEDIDO _pedido on remetente.CLI_CGCCPF = _pedido.CLI_CODIGO_REMETENTE ");
                        select.Append("     where _pedido.PCA_CODIGO = PreCarga.PCA_CODIGO ");
                        select.Append("       for xml path('') ");
                        select.Append("), 3, 1000) Remetente, ");

                        if (!groupBy.Contains("PreCarga.PCA_CODIGO,"))
                            groupBy.Append("PreCarga.PCA_CODIGO, ");
                    }
                    break;

                case "CNPJDestinatario":
                    if (!select.Contains(" CNPJDestinatario, "))
                    {
                        select.Append("substring((");
                        select.Append("    select distinct ', ' + LTRIM(STR(destinatario.CLI_CGCCPF, 20, 0)) ");
                        select.Append("      from T_CLIENTE destinatario ");
                        select.Append("      join T_PEDIDO _pedido on destinatario.CLI_CGCCPF = _pedido.CLI_CODIGO ");
                        select.Append("     where _pedido.PCA_CODIGO = PreCarga.PCA_CODIGO ");
                        select.Append("       for xml path('') ");
                        select.Append("), 3, 1000) CNPJDestinatario, ");

                        if (!groupBy.Contains("PreCarga.PCA_CODIGO,"))
                            groupBy.Append("PreCarga.PCA_CODIGO, ");
                    }
                    break;

                case "Destinatario":
                    if (!select.Contains(" Destinatario, "))
                    {
                        select.Append("substring((");
                        select.Append("    select distinct ', ' + (case when (destinatario.CLI_CODIGO_INTEGRACAO is not null and destinatario.CLI_CODIGO_INTEGRACAO <> '') then (destinatario.CLI_CODIGO_INTEGRACAO + ' - ') ELSE '' END) ");
                        select.Append("      + destinatario.CLI_NOME ");
                        select.Append("      from T_CLIENTE destinatario ");
                        select.Append("      join T_PEDIDO _pedido on destinatario.CLI_CGCCPF = _pedido.CLI_CODIGO ");
                        select.Append("     where _pedido.PCA_CODIGO = PreCarga.PCA_CODIGO ");
                        select.Append("       for xml path('') ");
                        select.Append("), 3, 1000) Destinatario, ");

                        if (!groupBy.Contains("PreCarga.PCA_CODIGO,"))
                            groupBy.Append("PreCarga.PCA_CODIGO, ");
                    }
                    break;

                case "Peso":
                    if (!select.Contains(" Peso, "))
                    {
                        select.Append("(select sum(_pedido.PED_PESO_TOTAL_CARGA) from T_PEDIDO _pedido where _pedido.PCA_CODIGO = PreCarga.PCA_CODIGO) Peso, ");

                        if (!groupBy.Contains("PreCarga.PCA_CODIGO,"))
                            groupBy.Append("PreCarga.PCA_CODIGO, ");
                    }
                    break;

                case "Motoristas":
                    if (!select.Contains(" Motoristas, "))
                    {
                        select.Append("SUBSTRING(( ");
                        select.Append("    SELECT DISTINCT ', ' + CAST(( ");
                        select.Append("               SUBSTRING(Motorista.FUN_CPF, 1, 3) + '.' + ");
                        select.Append("               SUBSTRING(Motorista.FUN_CPF, 4, 3) + '.' + ");
                        select.Append("               SUBSTRING(Motorista.FUN_CPF, 7, 3) + '-' + ");
                        select.Append("               SUBSTRING(Motorista.FUN_CPF, 10, 3) + ' - ' + ");
                        select.Append("               Motorista.FUN_NOME ");
                        select.Append("           ) AS NVARCHAR(4000)) ");
                        select.Append("      FROM T_PRE_CARGA_MOTORISTA preCargaMotorista ");
                        select.Append("      JOIN T_FUNCIONARIO Motorista ON preCargaMotorista.PED_CODIGO = Motorista.FUN_CODIGO ");
                        select.Append("     WHERE preCargaMotorista.PCA_CODIGO = PreCarga.PCA_CODIGO ");
                        select.Append("       FOR XML PATH('') ");
                        select.Append("), 3, 4000) Motoristas, ");

                        if (!groupBy.Contains("PreCarga.PCA_CODIGO,"))
                            groupBy.Append("PreCarga.PCA_CODIGO, ");
                    }
                    break;

                case "Veiculos":
                    if (!select.Contains(" Veiculos, "))
                    {
                        select.Append("( ");
                        select.Append("    (select _veiculo.VEI_PLACA from T_VEICULO _veiculo where _veiculo.VEI_CODIGO = PreCarga.CAR_VEICULO) + ");
                        select.Append("    isnull(( ");
                        select.Append("        select ', ' + _veiculo.VEI_PLACA ");
                        select.Append("          from T_PRE_CARGA_VEICULOS_VINCULADOS _veiculoVinculadoPreCarga ");
                        select.Append("          join T_VEICULO _veiculo on _veiculoVinculadoPreCarga.VEI_CODIGO = _veiculo.VEI_CODIGO ");
                        select.Append("         where _veiculoVinculadoPreCarga.PCA_CODIGO = PreCarga.PCA_CODIGO ");
                        select.Append("           for xml path('') ");
                        select.Append("    ), '') ");
                        select.Append(") Veiculos, ");

                        if (!groupBy.Contains("PreCarga.PCA_CODIGO,"))
                            groupBy.Append("PreCarga.PCA_CODIGO, ");

                        groupBy.Append("PreCarga.CAR_VEICULO, ");
                    }
                    break;

                case "PrevisaoChegadaDestinatarioFormatada":
                    if (!select.Contains(" PrevisaoChegadaDestinatario, "))
                    {
                        select.Append("PreCarga.PCA_PREVISAO_CHEGADA_DESTINATARIO PrevisaoChegadaDestinatario, ");
                        groupBy.Append("PreCarga.PCA_PREVISAO_CHEGADA_DESTINATARIO, ");
                    }
                    break;

                case "PrevisaoSaidaDestinatarioFormatada":
                    if (!select.Contains(" PrevisaoSaidaDestinatario, "))
                    {
                        select.Append("PreCarga.PCA_PREVISAO_SAIDA_DESTINATARIO PrevisaoSaidaDestinatario, ");
                        groupBy.Append("PreCarga.PCA_PREVISAO_SAIDA_DESTINATARIO, ");
                    }
                    break;

                case "RotaProgramada":
                    if (!select.Contains(" RotaProgramada, "))
                    {
                        select.Append("ConfiguracaoProgramacaoCarga.CPC_DESCRICAO RotaProgramada, ");
                        groupBy.Append("ConfiguracaoProgramacaoCarga.CPC_DESCRICAO, ");

                        SetarJoinsConfiguracaoProgramacaoCarga(joins);
                    }
                    break;
            }
        }

        protected override void SetarWhere(Dominio.ObjetosDeValor.Embarcador.Carga.FiltroPesquisaRelatorioPreCarga filtrosPesquisa, StringBuilder where, StringBuilder joins, StringBuilder groupBy, List<Embarcador.Consulta.ParametroSQL> parametros = null)
        {
            string pattern = "yyyy-MM-dd";

            where.Append(" and PreCarga.PCA_SITUACAO <> 2");

            if (filtrosPesquisa.SomenteProgramacaoCarga)
                where.Append($" and PreCarga.PCA_PROGRAMACAO_CARGA = 1 ");

            if (filtrosPesquisa.DataInicial != DateTime.MinValue)
                where.Append($" and CAST(PreCarga.CAR_DATA_PREVISAO_INICIO_VIAGEM AS DATE) >= '{filtrosPesquisa.DataInicial.ToString(pattern)}'");

            if (filtrosPesquisa.DataFinal != DateTime.MinValue)
                where.Append($" and CAST(PreCarga.CAR_DATA_PREVISAO_INICIO_VIAGEM AS DATE) <= '{filtrosPesquisa.DataFinal.ToString(pattern)}'");

            if (filtrosPesquisa.DataCriacaoPreCargaInicial != DateTime.MinValue)
                where.Append($" and CAST(PreCarga.PCA_DATA_CRIACAO AS DATE) >= '{filtrosPesquisa.DataCriacaoPreCargaInicial.ToString(pattern)}'");

            if (filtrosPesquisa.DataCriacaoPreCargaFinal != DateTime.MinValue)
                where.Append($" and CAST(PreCarga.PCA_DATA_CRIACAO AS DATE) <= '{filtrosPesquisa.DataCriacaoPreCargaFinal.ToString(pattern)}'");

            if (filtrosPesquisa.CodigoFilial > 0)
                where.Append($" and PreCarga.FIL_CODIGO = { filtrosPesquisa.CodigoFilial }");

            if (filtrosPesquisa.CodigoConfiguracaoProgramacaoCarga > 0)
                where.Append($" and PreCarga.CPC_CODIGO = { filtrosPesquisa.CodigoConfiguracaoProgramacaoCarga }");

            if (filtrosPesquisa.CodigoOperador > 0)
                where.Append($" and PreCarga.PCA_OPERADOR = { filtrosPesquisa.CodigoOperador }");

            if (!string.IsNullOrWhiteSpace(filtrosPesquisa.PreCarga))
                where.Append($" and PreCarga.PCA_NUMERO_CARGA = '{ filtrosPesquisa.PreCarga }'");

            if (!string.IsNullOrWhiteSpace(filtrosPesquisa.Carga))
            {
                where.Append($" and Carga.CAR_CODIGO_CARGA_EMBARCADOR = '{ filtrosPesquisa.Carga }'");
                SetarJoinsCarga(joins);
            }

            if (filtrosPesquisa.Situacao != Dominio.ObjetosDeValor.Embarcador.Enumeradores.FiltroPreCarga.Todos)
            {
                if (filtrosPesquisa.Situacao == Dominio.ObjetosDeValor.Embarcador.Enumeradores.FiltroPreCarga.ComCarga)
                    where.Append(" and PreCarga.CAR_CODIGO is not null");
                else if (filtrosPesquisa.Situacao == Dominio.ObjetosDeValor.Embarcador.Enumeradores.FiltroPreCarga.ComDadosInformados)
                {
                    where.Append(" and PreCarga.CAR_CODIGO is null and (PreCarga.EMP_CODIGO is not null or PreCarga.CAR_VEICULO is not null ");
                    where.Append(" or (select COUNT(*) from T_PRE_CARGA_VEICULOS_VINCULADOS _veiculoVinculadoPreCarga where _veiculoVinculadoPreCarga.PCA_CODIGO = PreCarga.PCA_CODIGO) > 0 ");
                    where.Append(" or (select COUNT(*) from T_PRE_CARGA_MOTORISTA preCargaMotorista where preCargaMotorista.PCA_CODIGO = PreCarga.PCA_CODIGO) > 0)");
                }
                else if (filtrosPesquisa.Situacao == Dominio.ObjetosDeValor.Embarcador.Enumeradores.FiltroPreCarga.EmDia)
                    where.Append($" and CAST(PreCarga.CAR_DATA_PREVISAO_INICIO_VIAGEM AS DATE) >= '{DateTime.Now.Date.ToString(pattern)}'");
                else if (filtrosPesquisa.Situacao == Dominio.ObjetosDeValor.Embarcador.Enumeradores.FiltroPreCarga.EmAtraso)
                    where.Append($" and CAST(PreCarga.CAR_DATA_PREVISAO_INICIO_VIAGEM AS DATE) < '{DateTime.Now.Date.ToString(pattern)}'");
                else if (filtrosPesquisa.Situacao == Dominio.ObjetosDeValor.Embarcador.Enumeradores.FiltroPreCarga.ProblemaVincularCarga)
                    where.Append(" and isnull(PreCarga.PCA_PROBLEMA_VINCULAR_CARGA, '') <> '' ");
            }

            if (!string.IsNullOrWhiteSpace(filtrosPesquisa.Pedido) || filtrosPesquisa.CpfCnpjRemetente > 0 || filtrosPesquisa.CpfCnpjDestinatario > 0)
            {
                StringBuilder wherePedido = new StringBuilder();
                if (!string.IsNullOrWhiteSpace(filtrosPesquisa.Pedido))
                {
                    wherePedido.Append($" and _pedido.PED_NUMERO_PEDIDO_EMBARCADOR = :PED_NUMERO_PEDIDO_EMBARCADOR");
                    parametros.Add(new Consulta.ParametroSQL("PED_NUMERO_PEDIDO_EMBARCADOR", filtrosPesquisa.Pedido));
                }

                if (filtrosPesquisa.CpfCnpjRemetente > 0)
                    wherePedido.Append($" and _pedido.CLI_CODIGO_REMETENTE = { filtrosPesquisa.CpfCnpjRemetente }");

                if (filtrosPesquisa.CpfCnpjDestinatario > 0)
                    wherePedido.Append($" and _pedido.CLI_CODIGO = { filtrosPesquisa.CpfCnpjDestinatario }");

                where.Append($" and PreCarga.PCA_CODIGO in (select _pedido.PCA_CODIGO from T_PEDIDO _pedido where 1 = 1 { wherePedido })"); 
            }
        }

        #endregion
    }
}
