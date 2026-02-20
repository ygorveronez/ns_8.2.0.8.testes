using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Repositorio.Embarcador.Pedidos
{
    sealed class ConsultaHistoricoMovimentacaoContainer : Consulta.Consulta<Dominio.ObjetosDeValor.Embarcador.Container.FiltroPesquisaRelatorioHistoricoMovimentacaoContainer>
    {
        #region Construtores

        public ConsultaHistoricoMovimentacaoContainer() : base(tabela: "T_COLETA_CONTAINER_HISTORICO as coletaContainerHistorico") { }

        #endregion

        #region Métodos Privados

        private void SetarJoinsCarga(StringBuilder joins)
        {
            if (!joins.Contains(" carga "))
                joins.Append(" left join T_CARGA carga on coletaContainerHistorico.CAR_CODIGO = carga.CAR_CODIGO ");
        }

        private void SetarJoinsContainer(StringBuilder joins)
        {
            SetarJoinsColetaContainer(joins);
            if (!joins.Contains(" container "))
                joins.Append(" left join T_CONTAINER container on coletaContainer.CTR_CODIGO = container.CTR_CODIGO ");
        }

        private void SetarJoinsColetaContainer(StringBuilder joins)
        {
            if (!joins.Contains(" coletaContainer "))
                joins.Append(" left join T_COLETA_CONTAINER coletaContainer on coletaContainerHistorico.CCR_CODIGO = coletaContainer.CCR_CODIGO ");
        }

        private void SetarJoinsFuncionario(StringBuilder joins)
        {
            if (!joins.Contains(" funcionario "))
                joins.Append(" left join T_FUNCIONARIO funcionario on coletaContainerHistorico.FUN_CODIGO = funcionario.FUN_CODIGO ");
        }

        private void SetarJoinsCliente(StringBuilder joins)
        {
            if (!joins.Contains(" cliente "))
                joins.Append(" left join T_CLIENTE cliente on coletaContainerHistorico.CLI_CODIGO_LOCAL = cliente.CLI_CGCCPF ");
        }

        private void SetarJoinsContainerTipo(StringBuilder joins)
        {
            SetarJoinsContainer(joins);
            if (!joins.Contains(" containerTipo "))
                joins.Append(" left join T_CONTAINER_TIPO containerTipo on container.CTI_CODIGO = containerTipo.CTI_CODIGO ");
        }

        #endregion

        protected override void SetarSelect(string propriedade, int codigoDinamico, StringBuilder select, StringBuilder joins, StringBuilder groupBy, bool somenteContarNumeroRegistros, Dominio.ObjetosDeValor.Embarcador.Container.FiltroPesquisaRelatorioHistoricoMovimentacaoContainer filtroPesquisa)
        {
            if (!select.Contains(" Codigo, "))
            {
                select.Append("coletaContainerHistorico.CCH_CODIGO as Codigo, ");
                groupBy.Append("coletaContainerHistorico.CCH_CODIGO, ");
            }

            switch (propriedade)
            {
                case "CodigoContainer":
                    if (!select.Contains(" CodigoContainer, "))
                    {
                        select.Append("container.CTR_NUMERO as CodigoContainer, ");
                        groupBy.Append("container.CTR_NUMERO, ");
                        SetarJoinsContainer(joins);
                    }
                    break;
                case "Carga":
                    if (!select.Contains(" Carga, "))
                    {
                        select.Append("isnull(coletaContainerHistorico.CCH_CODIGO_CARGA_EMBARCADOR, carga.CAR_CODIGO_CARGA_EMBARCADOR) as Carga, ");
                        groupBy.Append("coletaContainerHistorico.CCH_CODIGO_CARGA_EMBARCADOR, ");
                        groupBy.Append("carga.CAR_CODIGO_CARGA_EMBARCADOR, ");
                        SetarJoinsCarga(joins);
                    }
                    break;
                case "Auditado":
                    if (!select.Contains(" Auditado, "))
                    {
                        select.Append("funcionario.FUN_NOME as Auditado, ");
                        groupBy.Append("funcionario.FUN_NOME, ");
                        SetarJoinsFuncionario(joins);
                    }
                    break;
                case "NomeCNPJ":
                    if (!select.Contains(" Nome, "))
                    {
                        select.Append("cliente.CLI_NOME as Nome, ");
                        groupBy.Append("cliente.CLI_NOME, ");
                        SetarJoinsCliente(joins);
                    }
                    if (!select.Contains(" CPF_CNPJ, "))
                    {
                        select.Append("cliente.CLI_CGCCPF as CPF_CNPJ, ");
                        groupBy.Append("cliente.CLI_CGCCPF, ");
                        SetarJoinsCliente(joins);
                    }
                    if (!select.Contains(" Tipo, "))
                    {
                        select.Append("cliente.CLI_FISJUR as Tipo, ");
                        groupBy.Append("cliente.CLI_FISJUR, ");
                        SetarJoinsCliente(joins);
                    }
                    break;
                case "SituacaoContainerDescricao":
                    if (!select.Contains(" SituacaoContainer, "))
                    {
                        select.Append("coletaContainerHistorico.CCH_STATUS as SituacaoContainer, ");
                        groupBy.Append("coletaContainerHistorico.CCH_STATUS, ");
                        SetarJoinsCliente(joins);
                    }
                    break;
                case "DataHistorico":
                    if (!select.Contains(" DataHistorico, "))
                    {
                        select.Append("coletaContainerHistorico.CCH_DATA_HISTORICO as DataHistorico, ");
                        groupBy.Append("coletaContainerHistorico.CCH_DATA_HISTORICO, ");
                    }
                    break;
                case "TempoHistorico":
                    if (!select.Contains(" DataFimHistorico, "))
                    {
                        select.Append("coletaContainerHistorico.CCH_DATA_FIM_HISTORICO as DataFimHistorico, ");
                        groupBy.Append("coletaContainerHistorico.CCH_DATA_FIM_HISTORICO, ");
                    }
                    break;
                case "OrigemDescricao":
                    if (!select.Contains(" Origem, "))
                    {
                        select.Append("coletaContainerHistorico.CCH_ORIGEM as Origem, ");
                        groupBy.Append("coletaContainerHistorico.CCH_ORIGEM, ");
                    }
                    break;
                case "InformacaoOrigemDescricao":
                    if (!select.Contains(" InformacaoOrigem, "))
                    {
                        select.Append("coletaContainerHistorico.CCH_INFORMACAO as InformacaoOrigem, ");
                        groupBy.Append("coletaContainerHistorico.CCH_INFORMACAO, ");
                    }
                    break;
            }
        }

        protected override void SetarWhere(Dominio.ObjetosDeValor.Embarcador.Container.FiltroPesquisaRelatorioHistoricoMovimentacaoContainer filtrosPesquisa, StringBuilder where, StringBuilder joins, StringBuilder groupBy, List<Embarcador.Consulta.ParametroSQL> parametros = null)
        {
            where.Append(" AND container.CTR_NUMERO is not null ");
            SetarJoinsContainer(joins);

            if (!string.IsNullOrWhiteSpace(filtrosPesquisa.Carga))
            {
                where.Append($"  AND (coletaContainerHistorico.CCH_CODIGO_CARGA_EMBARCADOR = '{filtrosPesquisa.Carga}' or carga.CAR_CODIGO_CARGA_EMBARCADOR = '{filtrosPesquisa.Carga}')");
                SetarJoinsCarga(joins);
            }

            if (!string.IsNullOrWhiteSpace(filtrosPesquisa.Container))
                where.Append($"  AND container.CTR_NUMERO = '{filtrosPesquisa.Container}'");

            if (filtrosPesquisa.SituacaoContainer.HasValue)
                where.Append($" AND coletaContainerHistorico.CCH_STATUS = {(int)filtrosPesquisa.SituacaoContainer} ");

            if (filtrosPesquisa.DataInicialColeta != DateTime.MinValue)
            {
                where.Append($"  AND coletaContainer.CCR_DATA_COLETA >= '{filtrosPesquisa.DataInicialColeta.ToString("yyyy-MM-dd")}'");
                SetarJoinsColetaContainer(joins);
            }

            if (filtrosPesquisa.DataFinalColeta != DateTime.MinValue)
            {
                where.Append($"  AND coletaContainer.CCR_DATA_COLETA < '{filtrosPesquisa.DataFinalColeta.ToString("yyyy-MM-dd")}'");
                SetarJoinsColetaContainer(joins);
            }

            if (filtrosPesquisa.DataMovimentacao != DateTime.MinValue)
            {
                where.Append($" AND convert(date,coletaContainer.CCR_DATA_ULTIMA_MOVIMENTACAO, 102) = convert(datetime, '{filtrosPesquisa.DataMovimentacao.ToString("yyyy-MM-dd")}', 102) ");
                SetarJoinsColetaContainer(joins);
            }

            if (filtrosPesquisa.DiasPosseInicial > 0)
            {
                where.Append($" AND (DATEDIFF(day, coletaContainer.CCR_DATA_COLETA, isnull(coletaContainer.CCR_DATA_EMBARQUE, '{DateTime.Now.ToString("yyyy-MM-dd HH:mm:sss")}')) + 1) >= {filtrosPesquisa.DiasPosseInicial}");
                SetarJoinsColetaContainer(joins);
            }

            if (filtrosPesquisa.DiasPosseFinal > 0)
            {
                where.Append($" AND (DATEDIFF(day, coletaContainer.CCR_DATA_COLETA, isnull(coletaContainer.CCR_DATA_EMBARQUE, '{DateTime.Now.ToString("yyyy-MM-dd HH:mm:sss")}')) + 1) >= {filtrosPesquisa.DiasPosseFinal}");
                SetarJoinsColetaContainer(joins);
            }

            if (filtrosPesquisa.LocalEsperaVazio > 0)
                where.Append($" AND coletaContainer.CLI_CODIGO_ATUAL = '{filtrosPesquisa.LocalEsperaVazio}'");

            if (filtrosPesquisa.LocalAtual > 0)
                where.Append($" AND coletaContainer.CLI_CODIGO_ATUAL = '{filtrosPesquisa.LocalAtual}'");

            if (filtrosPesquisa.LocalColeta > 0)
                where.Append($" AND coletaContainer.CLI_CODIGO_ATUAL = '{filtrosPesquisa.LocalColeta}'");

            if (!string.IsNullOrWhiteSpace(filtrosPesquisa.NumeroPedido) || !string.IsNullOrWhiteSpace(filtrosPesquisa.NumeroEXP) || !string.IsNullOrWhiteSpace(filtrosPesquisa.NumeroBooking))
            {
                where.Append(@" and (exists(
                    select top(1) _pedido.PED_CODIGO
                      from T_PEDIDO _pedido
                      join T_CARGA_PEDIDO _cargaPedido on _cargaPedido.PED_CODIGO = _pedido.PED_CODIGO
                     where _cargaPedido.CAR_CODIGO = carga.CAR_CODIGO ");

                if (!string.IsNullOrWhiteSpace(filtrosPesquisa.NumeroPedido))
                    where.Append($" and _pedido.PED_NUMERO_PEDIDO_EMBARCADOR = '{filtrosPesquisa.NumeroPedido}'");

                if (!string.IsNullOrWhiteSpace(filtrosPesquisa.NumeroEXP))
                    where.Append($" and _pedido.PED_NUMERO_EXP = '{filtrosPesquisa.NumeroEXP}'");

                if (!string.IsNullOrWhiteSpace(filtrosPesquisa.NumeroBooking))
                    where.Append($" and _pedido.PED_NUMERO_BOOKING = '{filtrosPesquisa.NumeroBooking}'");

                where.Append(@") or exists (  select top(1) _pedido.PED_CODIGO
                      from T_PEDIDO _pedido
                      join T_CARGA_PEDIDO _cargaPedido on _cargaPedido.PED_CODIGO = _pedido.PED_CODIGO
                     where _cargaPedido.CAR_CODIGO_ORIGEM = carga.CAR_CODIGO ");

                if (!string.IsNullOrWhiteSpace(filtrosPesquisa.NumeroPedido))
                    where.Append($" and _pedido.PED_NUMERO_PEDIDO_EMBARCADOR = '{filtrosPesquisa.NumeroPedido}'");

                if (!string.IsNullOrWhiteSpace(filtrosPesquisa.NumeroEXP))
                    where.Append($" and _pedido.PED_NUMERO_EXP = '{filtrosPesquisa.NumeroEXP}'");

                if (!string.IsNullOrWhiteSpace(filtrosPesquisa.NumeroBooking))
                    where.Append($" and _pedido.PED_NUMERO_BOOKING = '{filtrosPesquisa.NumeroBooking}'");

                where.Append(" ))");
                SetarJoinsCarga(joins);
            }

            if (filtrosPesquisa.TipoContainer > 0)
            {
                where.Append($" AND containerTipo.CTI_CODIGO = '{filtrosPesquisa.TipoContainer}'");
                SetarJoinsContainerTipo(joins);
            }

            if (filtrosPesquisa.Filial > 0)
            {
                where.Append($" AND carga.FIL_CODIGO = '{filtrosPesquisa.Filial}'");
                SetarJoinsCarga(joins);
            }

        }

    }

}
