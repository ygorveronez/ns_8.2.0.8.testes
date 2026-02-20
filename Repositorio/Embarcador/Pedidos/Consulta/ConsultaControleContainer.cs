using Dominio.ObjetosDeValor.Embarcador.Enumeradores;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Repositorio.Embarcador.Pedidos
{
    sealed class ConsultaControleContainer : Consulta.Consulta<Dominio.ObjetosDeValor.Embarcador.Container.FiltroPesquisaRelatorioControleContainer>
    {
        #region Construtores

        public ConsultaControleContainer() : base(tabela: "T_COLETA_CONTAINER as ContainerColeta") { }

        #endregion

        #region Métodos Privados

        private void SetarJoinsCarga(StringBuilder joins)
        {
            if (!joins.Contains(" Carga "))
                joins.Append(" left join T_CARGA as Carga on Carga.CAR_CODIGO = ContainerColeta.CAR_CODIGO ");
        }

        private void SetarJoinsContainer(StringBuilder joins)
        {
            if (!joins.Contains(" Container "))
                joins.Append(" left join T_CONTAINER Container on Container.CTR_CODIGO = ContainerColeta.CTR_CODIGO ");
        }
        private void SetarJoinsClienteColeta(StringBuilder joins)
        {
            if (!joins.Contains(" ClienteColeta "))
                joins.Append(" left join T_CLIENTE ClienteColeta on ClienteColeta.CLI_CGCCPF = ContainerColeta.CLI_CODIGO_COLETA ");
        }

        private void SetarJoinsCargaAtual(StringBuilder joins)
        {
            if (!joins.Contains(" CargaAtual "))
                joins.Append("  left join T_CARGA CargaAtual on ContainerColeta.CAR_CODIGO_ATUAL = CargaAtual.CAR_CODIGO ");
        }

        private void SetarJoinsFilialCargaAtual(StringBuilder joins)
        {
            SetarJoinsCargaAtual(joins);
            if (!joins.Contains(" FilialCargaAtual "))
                joins.Append(" left join T_FILIAL FilialCargaAtual on FilialCargaAtual.FIL_CODIGO = CargaAtual.FIL_CODIGO  ");
        }

        private void SetarJoinsClienteAtual(StringBuilder joins)
        {
            if (!joins.Contains(" ClienteAtual "))
                joins.Append(" left join T_CLIENTE ClienteAtual on ClienteAtual.CLI_CGCCPF = ContainerColeta.CLI_CODIGO_ATUAL ");
        }

        private void SetarJoinsContainerTipo(StringBuilder joins)
        {
            SetarJoinsContainer(joins);
            if (!joins.Contains(" ContainerTipo "))
                joins.Append(" left join T_CONTAINER_TIPO ContainerTipo on ContainerTipo.CTI_CODIGO = Container.CTI_CODIGO ");
        }

        private void SetarJoinsCargaAgrupada(StringBuilder joins)
        {
            SetarJoinsCargaAtual(joins);
            if (!joins.Contains(" CargaAgrupada "))
                joins.Append("  left join T_CARGA CargaAgrupada on CargaAgrupada.CAR_CODIGO = CargaAtual.CAR_CODIGO_AGRUPAMENTO ");
        }
        private void SetarJoinsFilialOrigemCargaAtual(StringBuilder joins)
        {
            SetarJoinsCargaAtual(joins);
            if (!joins.Contains(" FilialOrigemCargaAtual "))
                joins.Append(" left join T_FILIAL FilialOrigemCargaAtual on FilialOrigemCargaAtual.FIL_CODIGO = CargaAtual.FIL_CODIGO_ORIGEM ");
        }

        private void SetarJoinsColetaContainerJustificativa(StringBuilder joins)
        {
            if (!joins.Contains(" Justificativa "))
                joins.Append(" left join T_COLETA_CONTAINER_JUSTIFICATIVA Justificativa on Justificativa.CCR_CODIGO = ContainerColeta.CCR_CODIGO ");
        }

        private void SetarJoinsJustificativaContainer(StringBuilder joins)
        {
            SetarJoinsColetaContainerJustificativa(joins);
            if (!joins.Contains(" JustificativaColetaContainer "))
                joins.Append(" left join T_JUSTIFICATIVA_CONTAINER JustificativaContainer on JustificativaContainer.JSC_CODIGO = Justificativa.JSC_CODIGO  ");
        }


        #endregion

        protected override void SetarSelect(string propriedade, int codigoDinamico, StringBuilder select, StringBuilder joins, StringBuilder groupBy, bool somenteContarNumeroRegistros, Dominio.ObjetosDeValor.Embarcador.Container.FiltroPesquisaRelatorioControleContainer filtroPesquisa)
        {
            switch (propriedade)
            {
                case "Codigo":
                    if (!select.Contains(" Codigo, "))
                    {
                        select.Append("ContainerColeta.CCR_CODIGO Codigo,");
                        groupBy.Append("ContainerColeta.CCR_CODIGO, ");
                    }
                    break;

                case "CodigoCargaEmbarcador":
                    if (!select.Contains(" CodigoCargaEmbarcador, "))
                    {
                        select.Append("case when CargaAtual.CAR_CODIGO is null then Carga.CAR_CODIGO_CARGA_EMBARCADOR else CargaAtual.CAR_CODIGO_CARGA_EMBARCADOR end CodigoCargaEmbarcador,  ");
                        groupBy.Append("Carga.CAR_CODIGO_CARGA_EMBARCADOR, CargaAtual.CAR_CODIGO_CARGA_EMBARCADOR, CargaAtual.CAR_CODIGO, ");
                        SetarJoinsCargaAgrupada(joins);
                        SetarJoinsCarga(joins);
                        SetarJoinsCargaAtual(joins);
                    }
                    break;

                case "NumeroCargaAgrupada":
                    if (!select.Contains(" NumeroCargaAgrupada, "))
                    {
                        select.Append("Carga.CAR_CODIGO_CARGA_EMBARCADOR NumeroCargaAgrupada,  ");
                        groupBy.Append("Carga.CAR_CODIGO_CARGA_EMBARCADOR, ");

                        SetarJoinsCargaAgrupada(joins);
                    }
                    break;

                case "NumeroContainer":
                    if (!select.Contains(" NumeroContainer, "))
                    {
                        select.Append("Container.CTR_NUMERO NumeroContainer, ");
                        groupBy.Append("Container.CTR_NUMERO, ");

                        SetarJoinsContainer(joins);
                    }
                    break;

                case "TipoContainer":
                    if (!select.Contains(" TipoContainer, "))
                    {
                        select.Append("ContainerTipo.CTI_DESCRICAO TipoContainer,  ");
                        groupBy.Append("ContainerTipo.CTI_DESCRICAO, ");

                        SetarJoinsContainerTipo(joins);
                    }
                    break;

                case "SituacaoContainer":
                    if (!select.Contains(" Status, "))
                    {
                        select.Append("ContainerColeta.CCR_STATUS Status,  ");
                        groupBy.Append("ContainerColeta.CCR_STATUS, ");
                    }
                    break;

                case "DataMovimentacaoFormatada":
                    if (!select.Contains(" DataMovimentacao, "))
                    {
                        select.Append("ContainerColeta.CCR_DATA_ULTIMA_MOVIMENTACAO DataMovimentacao,  ");
                        groupBy.Append("ContainerColeta.CCR_DATA_ULTIMA_MOVIMENTACAO, ");

                    }
                    break;
                case "DataColetaFormatada":
                    if (!select.Contains(" DataColeta, "))
                    {
                        select.Append("ContainerColeta.CCR_DATA_COLETA DataColeta, ");
                        groupBy.Append("ContainerColeta.CCR_DATA_COLETA, ");

                    }
                    break;

                case "DataEmbarqueNavioFormatada":
                    if (!select.Contains(" DataEmbarqueNavio, "))
                    {
                        select.Append("ContainerColeta.CCR_DATA_EMBARQUE_NAVIO DataEmbarqueNavio, ");
                        groupBy.Append("ContainerColeta.CCR_DATA_EMBARQUE_NAVIO, ");

                    }
                    break;

                case "DataPortoFormatada":
                    if (!select.Contains(" DataPorto, "))
                    {
                        select.Append("ContainerColeta.CCR_DATA_EMBARQUE DataPorto, ");
                        groupBy.Append("ContainerColeta.CCR_DATA_EMBARQUE, ");

                    }
                    break;

                case "JustificativaDescritiva":
                    if (!select.Contains(" JustificativaDescritiva, "))
                    {
                        select.Append("Justificativa.JSC_JUSTIFICATIVA_DESCRITIVA JustificativaDescritiva, ");
                        groupBy.Append("Justificativa.JSC_JUSTIFICATIVA_DESCRITIVA, ");
                        SetarJoinsColetaContainerJustificativa(joins);

                    }
                    break;

                case "Justificativa":
                    if (!select.Contains(" Justificativa, "))
                    {
                        select.Append("JustificativaContainer.JSC_DESCRICAO Justificativa, ");
                        groupBy.Append("JustificativaContainer.JSC_DESCRICAO, ");
                        SetarJoinsJustificativaContainer(joins);
                    }
                    break;



                case "NumeroEXPValido":
                    if (!select.Contains(" NumeroEXP, "))
                    {
                        select.Append(@"substring((select distinct ', ' + Pedido.PED_NUMERO_EXP 
                                            from T_PEDIDO Pedido 
                                        join T_CARGA_PEDIDO CargaPedido on CargaPedido.PED_CODIGO = Pedido.PED_CODIGO 
                                        where CargaPedido.CAR_CODIGO = CargaAtual.CAR_CODIGO
                                        and isnull(Pedido.PED_NUMERO_EXP, '') <> '' for xml path('')), 3, 1000) NumeroEXP, ");
                        groupBy.Append("CargaAtual.CAR_CODIGO, ");
                        SetarJoinsCargaAtual(joins);
                    }
                    if (!select.Contains(" NumeroEXPAgrupado, "))
                    {
                        select.Append(@"substring((select distinct ', ' + Pedido.PED_NUMERO_EXP 
                                            from T_PEDIDO Pedido 
                                        join T_CARGA_PEDIDO CargaPedido on CargaPedido.PED_CODIGO = Pedido.PED_CODIGO 
                                        where CargaPedido.CAR_CODIGO_ORIGEM = CargaAtual.CAR_CODIGO
                                        and isnull(Pedido.PED_NUMERO_EXP, '') <> '' for xml path('')), 3, 1000) NumeroEXPAgrupado, ");
                        groupBy.Append("CargaAtual.CAR_CODIGO, ");
                        SetarJoinsCargaAtual(joins);
                    }
                    break;

                case "Pedido":
                    if (!select.Contains(" NumeroPedido, "))
                    {
                        select.Append(@"substring((select distinct ', ' + Pedido.PED_NUMERO_PEDIDO_EMBARCADOR 
                                            from T_PEDIDO Pedido 
                                        join T_CARGA_PEDIDO CargaPedido on CargaPedido.PED_CODIGO = Pedido.PED_CODIGO 
                                        where CargaPedido.CAR_CODIGO = CargaAtual.CAR_CODIGO
                                        and isnull(Pedido.PED_NUMERO_PEDIDO_EMBARCADOR, '') <> ''for xml path('')), 3, 1000) NumeroPedido, ");
                        groupBy.Append("CargaAtual.CAR_CODIGO, ");
                        SetarJoinsCargaAtual(joins);
                    }
                    if (!select.Contains(" NumeroPedidoAgrupado, "))
                    {
                        select.Append(@"substring((select distinct ', ' + Pedido.PED_NUMERO_PEDIDO_EMBARCADOR 
                                            from T_PEDIDO Pedido 
                                    join T_CARGA_PEDIDO CargaPedido on CargaPedido.PED_CODIGO = Pedido.PED_CODIGO 
                                    where CargaPedido.CAR_CODIGO_ORIGEM = CargaAtual.CAR_CODIGO
                                    and isnull(Pedido.PED_NUMERO_PEDIDO_EMBARCADOR, '') <> '' for xml path('')), 3, 1000) NumeroPedidoAgrupado, ");
                        groupBy.Append("CargaAtual.CAR_CODIGO, ");
                        SetarJoinsCargaAtual(joins);
                    }
                    break;

                case "Filial":
                    if (!select.Contains(" FilialCargaAtual, "))
                    {
                        select.Append("FilialCargaAtual.FIL_DESCRICAO FilialCargaAtual, ");
                        groupBy.Append("FilialCargaAtual.FIL_DESCRICAO, ");
                        SetarJoinsFilialCargaAtual(joins);

                    }
                    if (!select.Contains(" CNPJFilialCargaAtual, "))
                    {
                        select.Append("FilialCargaAtual.FIL_CNPJ CNPJFilialCargaAtual, ");
                        groupBy.Append("FilialCargaAtual.FIL_CNPJ, ");
                        SetarJoinsFilialCargaAtual(joins);
                    }
                    if (!select.Contains(" CNPJFilialCargaAtual, "))
                    {
                        select.Append("FilialOrigemCargaAtual.FIL_DESCRICAO FilialCargaOrigem, ");
                        groupBy.Append("FilialOrigemCargaAtual.FIL_DESCRICAO, ");
                        SetarJoinsFilialOrigemCargaAtual(joins);
                    }
                    if (!select.Contains(" CNPJFilialCargaAtual, "))
                    {
                        select.Append("FilialOrigemCargaAtual.FIL_CNPJ CNPJFilialCargaOrigem, ");
                        groupBy.Append("FilialOrigemCargaAtual.FIL_CNPJ, ");
                        SetarJoinsFilialOrigemCargaAtual(joins);
                    }
                    break;

                case "AreaColeta":
                    if (!select.Contains(" LocalColeta, "))
                    {
                        select.Append("ContainerColeta.CLI_CODIGO_COLETA LocalColeta, ");
                        groupBy.Append("ContainerColeta.CLI_CODIGO_COLETA, ");

                    }
                    if (!select.Contains(" ClienteLocalColeta, "))
                    {
                        select.Append("ClienteColeta.CLI_NOME ClienteLocalColeta, ");
                        groupBy.Append("ClienteColeta.CLI_NOME, ");
                        SetarJoinsClienteColeta(joins);
                    }
                    break;

                case "AreaAtual":
                    if (!select.Contains(" LocalAtual, "))
                    {
                        select.Append("ContainerColeta.CLI_CODIGO_ATUAL LocalAtual, ");
                        groupBy.Append("ContainerColeta.CLI_CODIGO_ATUAL, ");

                    }
                    if (!select.Contains(" ClienteLocalAtual, "))
                    {
                        select.Append("ClienteAtual.CLI_NOME ClienteLocalAtual, ");
                        groupBy.Append("ClienteAtual.CLI_NOME, ");
                        SetarJoinsClienteAtual(joins);
                    }
                    break;

                case "DiasExcesso":
                    if (!select.Contains(" FreeTime, "))
                    {
                        select.Append("ContainerColeta.CCR_FREETIME FreeTime, ");
                        groupBy.Append("ContainerColeta.CCR_FREETIME, ");

                    }
                    if (!select.Contains(" DiasEmPosse, "))
                    {
                        select.Append($@"((DATEDIFF(day, ContainerColeta.CCR_DATA_COLETA, isnull(ContainerColeta.CCR_DATA_EMBARQUE, '{DateTime.Now.ToString("yyyy - MM - dd HH: mm:sss")}')) + 1)) DiasEmPosse, ");
                        groupBy.Append("ContainerColeta.CCR_DATA_COLETA, ContainerColeta.CCR_DATA_EMBARQUE, ");
                    }
                    break;

                case "ExcedeuFreeTime":
                    if (!select.Contains(" FreeTime, "))
                    {
                        select.Append("ContainerColeta.CCR_FREETIME FreeTime, ");
                        groupBy.Append("ContainerColeta.CCR_FREETIME, ");

                    }
                    if (!select.Contains(" DiasEmPosse, "))
                    {
                        select.Append($@"((DATEDIFF(day, ContainerColeta.CCR_DATA_COLETA, isnull(ContainerColeta.CCR_DATA_EMBARQUE, '{DateTime.Now.ToString("yyyy - MM - dd HH: mm:sss")}')) + 1)) DiasEmPosse, ");
                        if (!groupBy.Contains("ContainerColeta.CCR_DATA_COLETA, ContainerColeta.CCR_DATA_EMBARQUE"))
                            groupBy.Append("ContainerColeta.CCR_DATA_COLETA, ContainerColeta.CCR_DATA_EMBARQUE, ");
                    }
                    break;

                case "ValorDevido":
                    if (!select.Contains(" FreeTime, "))
                    {
                        select.Append("ContainerColeta.CCR_FREETIME FreeTime, ");
                        groupBy.Append("ContainerColeta.CCR_FREETIME, ");

                    }
                    if (!select.Contains(" DiasEmPosse, "))
                    {
                        select.Append($@"((DATEDIFF(day, ContainerColeta.CCR_DATA_COLETA, isnull(ContainerColeta.CCR_DATA_EMBARQUE, '{DateTime.Now.ToString("yyyy - MM - dd HH: mm:sss")}')) + 1)) DiasEmPosse, ");
                        if (!groupBy.Contains("ContainerColeta.CCR_DATA_COLETA, ContainerColeta.CCR_DATA_EMBARQUE"))
                            groupBy.Append("ContainerColeta.CCR_DATA_COLETA, ContainerColeta.CCR_DATA_EMBARQUE, ");
                    }
                    if (!select.Contains(" ValorDiaria, "))
                    {
                        select.Append($@"ContainerColeta.CCR_VALOR_DIARIA ValorDiaria, ");
                        groupBy.Append("ContainerColeta.CCR_VALOR_DIARIA, ");
                    }
                    break;



            }
        }

        protected override void SetarWhere(Dominio.ObjetosDeValor.Embarcador.Container.FiltroPesquisaRelatorioControleContainer filtrosPesquisa, StringBuilder where, StringBuilder joins, StringBuilder groupBy, List<Embarcador.Consulta.ParametroSQL> parametros = null)
        {
            where.Append(" AND Container.CTR_NUMERO is not null");

            if (!string.IsNullOrWhiteSpace(filtrosPesquisa.CodigoCargaEmbarcador))
                where.Append($"  AND (CargaAtual.CAR_CODIGO_CARGA_EMBARCADOR = '{filtrosPesquisa.CodigoCargaEmbarcador}' or CargaAgrupada.CAR_CODIGO_CARGA_EMBARCADOR = '{filtrosPesquisa.CodigoCargaEmbarcador}' or Carga.CAR_CODIGO_CARGA_EMBARCADOR = '{filtrosPesquisa.CodigoCargaEmbarcador}')");

            if (filtrosPesquisa.DataInicialColeta != DateTime.MinValue)
                where.Append($"  AND ContainerColeta.CCR_DATA_COLETA >= '{filtrosPesquisa.DataInicialColeta.ToString("yyyy-MM-dd")}'");

            if (filtrosPesquisa.DataFinalColeta != DateTime.MinValue)
                where.Append($"  AND ContainerColeta.CCR_DATA_COLETA < '{filtrosPesquisa.DataFinalColeta.ToString("yyyy-MM-dd")}'");


            if (filtrosPesquisa.DataMovimentacao != DateTime.MinValue)
                where.Append($" AND convert(date,ContainerColeta.CCR_DATA_ULTIMA_MOVIMENTACAO, 102) = convert(datetime, '{filtrosPesquisa.DataMovimentacao.ToString("yyyy-MM-dd")}', 102) ");

            if (filtrosPesquisa.DiasPosseInicial > 0)
                where.Append($" AND (DATEDIFF(day, ContainerColeta.CCR_DATA_COLETA, isnull(ContainerColeta.CCR_DATA_EMBARQUE, '{DateTime.Now.ToString("yyyy-MM-dd HH:mm:sss")}')) + 1) >= {filtrosPesquisa.DiasPosseInicial}");

            if (filtrosPesquisa.DiasPosseFinal > 0)
                where.Append($" AND (DATEDIFF(day, ContainerColeta.CCR_DATA_COLETA, isnull(ContainerColeta.CCR_DATA_EMBARQUE, '{DateTime.Now.ToString("yyyy-MM-dd HH:mm:sss")}')) + 1) >= {filtrosPesquisa.DiasPosseFinal}");

            if (filtrosPesquisa.DiasPosseFinal > 0)
                where.Append($" AND (DATEDIFF(day, ContainerColeta.CCR_DATA_COLETA, isnull(ContainerColeta.CCR_DATA_EMBARQUE, '{DateTime.Now.ToString("yyyy-MM-dd HH:mm:sss")}')) + 1) >= {filtrosPesquisa.DiasPosseFinal}");

            if (!string.IsNullOrWhiteSpace(filtrosPesquisa.NumeroContainer))
                where.Append($" AND Container.CTR_NUMERO = '{filtrosPesquisa.NumeroContainer}'");

            if (filtrosPesquisa.LocalEsperaVazio > 0)
                where.Append($" AND ContainerColeta.CLI_CODIGO_ATUAL = '{filtrosPesquisa.LocalEsperaVazio}'");

            if (filtrosPesquisa.LocalAtual > 0)
                where.Append($" AND ContainerColeta.CLI_CODIGO_ATUAL = '{filtrosPesquisa.LocalAtual}'");

            if (filtrosPesquisa.LocalColeta > 0)
                where.Append($" AND ContainerColeta.CLI_CODIGO_ATUAL = '{filtrosPesquisa.LocalColeta}'");

            if (!string.IsNullOrWhiteSpace(filtrosPesquisa.NumeroPedido) || !string.IsNullOrWhiteSpace(filtrosPesquisa.NumeroEXP) || !string.IsNullOrWhiteSpace(filtrosPesquisa.NumeroBooking))
            {
                where.Append(@" and (exists(
                    select top(1) _pedido.PED_CODIGO
                      from T_PEDIDO _pedido
                      join T_CARGA_PEDIDO _cargaPedido on _cargaPedido.PED_CODIGO = _pedido.PED_CODIGO
                     where _cargaPedido.CAR_CODIGO = CargaAtual.CAR_CODIGO ");

                if (!string.IsNullOrWhiteSpace(filtrosPesquisa.NumeroPedido))
                    where.Append($" and _pedido.PED_NUMERO_PEDIDO_EMBARCADOR = '{filtrosPesquisa.NumeroPedido}'");

                if (!string.IsNullOrWhiteSpace(filtrosPesquisa.NumeroEXP))
                    where.Append($" and _pedido.PED_NUMERO_EXP = '{filtrosPesquisa.NumeroEXP}'");

                if (!string.IsNullOrWhiteSpace(filtrosPesquisa.NumeroBooking))
                    where.Append($" and _pedido.PED_NUMERO_BOOKING = '{filtrosPesquisa.NumeroBooking}'");

                where.Append(@") or exists (  select top(1) _pedido.PED_CODIGO
                      from T_PEDIDO _pedido
                      join T_CARGA_PEDIDO _cargaPedido on _cargaPedido.PED_CODIGO = _pedido.PED_CODIGO
                     where _cargaPedido.CAR_CODIGO_ORIGEM = CargaAtual.CAR_CODIGO ");

                if (!string.IsNullOrWhiteSpace(filtrosPesquisa.NumeroPedido))
                    where.Append($" and _pedido.PED_NUMERO_PEDIDO_EMBARCADOR = '{filtrosPesquisa.NumeroPedido}'");

                if (!string.IsNullOrWhiteSpace(filtrosPesquisa.NumeroEXP))
                    where.Append($" and _pedido.PED_NUMERO_EXP = '{filtrosPesquisa.NumeroEXP}'");

                if (!string.IsNullOrWhiteSpace(filtrosPesquisa.NumeroBooking))
                    where.Append($" and _pedido.PED_NUMERO_BOOKING = '{filtrosPesquisa.NumeroBooking}'");

                where.Append(" ))");
            }

            if (filtrosPesquisa.TipoContainer > 0)
                where.Append($" AND ContainerTipo.CTI_CODIGO = '{filtrosPesquisa.TipoContainer}'");

            if (filtrosPesquisa.Filial > 0)
                where.Append($" AND CargaAtual.FIL_CODIGO = '{filtrosPesquisa.Filial}'");

            if (filtrosPesquisa.SituacaoContainer.HasValue)
                where.Append($" AND ContainerColeta.CCR_STATUS = {(int)filtrosPesquisa.SituacaoContainer} ");
            else
                where.Append($" AND ContainerColeta.CCR_STATUS <> {(int)StatusColetaContainer.Cancelado} ");
        }

    }

}
