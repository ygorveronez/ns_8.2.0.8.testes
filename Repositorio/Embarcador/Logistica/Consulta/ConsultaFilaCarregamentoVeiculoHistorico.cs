using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Repositorio.Embarcador.Logistica.Consulta
{
    sealed class ConsultaFilaCarregamentoVeiculoHistorico : Repositorio.Embarcador.Consulta.Consulta<Dominio.ObjetosDeValor.Embarcador.Logistica.FiltroPesquisaFilaCarregamentoVeiculoHistorico>
    {
        #region Construtores

        public ConsultaFilaCarregamentoVeiculoHistorico() : base(tabela: "T_FILA_CARREGAMENTO_VEICULO_HISTORICO as FilaCarregamentoVeiculoHistorico") { }

        #endregion

        #region Métodos Privados

        private void SetarJoinsFilaCarregamentoConjuntoMotorista(StringBuilder joins)
        {
            SetarJoinsFilaCarregamentoVeiculo(joins);

            if (!joins.Contains(" FilaCarregamentoConjuntoMotorista "))
                joins.Append("join T_FILA_CARREGAMENTO_CONJUNTO_MOTORISTA FilaCarregamentoConjuntoMotorista on FilaCarregamentoConjuntoMotorista.FCM_CODIGO = FilaCarregamentoVeiculo.FCM_CODIGO ");
        }

        private void SetarJoinsFilaCarregamentoConjuntoVeiculo(StringBuilder joins)
        {
            SetarJoinsFilaCarregamentoVeiculo(joins);

            if (!joins.Contains(" FilaCarregamentoConjuntoVeiculo "))
                joins.Append("join T_FILA_CARREGAMENTO_CONJUNTO_VEICULO FilaCarregamentoConjuntoVeiculo on FilaCarregamentoConjuntoVeiculo.FCV_CODIGO = FilaCarregamentoVeiculo.FCV_CODIGO ");
        }

        private void SetarJoinsFilaCarregamentoVeiculo(StringBuilder joins)
        {
            if (!joins.Contains(" FilaCarregamentoVeiculo "))
                joins.Append("join T_FILA_CARREGAMENTO_VEICULO FilaCarregamentoVeiculo on FilaCarregamentoVeiculo.FLV_CODIGO = FilaCarregamentoVeiculoHistorico.FLV_CODIGO ");
        }

        private void SetarJoinsModeloVeicularCarga(StringBuilder joins)
        {
            SetarJoinsFilaCarregamentoConjuntoVeiculo(joins);

            if (!joins.Contains(" ModeloVeicularCarga "))
                joins.Append("join T_MODELO_VEICULAR_CARGA ModeloVeicularCarga on ModeloVeicularCarga.MVC_CODIGO = FilaCarregamentoConjuntoVeiculo.MVC_CODIGO ");
        }

        private void SetarJoinsMotorista(StringBuilder joins)
        {
            SetarJoinsFilaCarregamentoConjuntoMotorista(joins);

            if (!joins.Contains(" Motorista "))
                joins.Append("left join T_FUNCIONARIO Motorista on Motorista.FUN_CODIGO = FilaCarregamentoConjuntoMotorista.FCM_CODIGO_MOTORISTA ");
        }

        private void SetarJoinsTracao(StringBuilder joins)
        {
            SetarJoinsFilaCarregamentoConjuntoVeiculo(joins);

            if (!joins.Contains(" Tracao "))
                joins.Append("left join T_VEICULO Tracao on Tracao.VEI_CODIGO = FilaCarregamentoConjuntoVeiculo.FCV_CODIGO_TRACAO ");
        }

        private void SetarJoinsUsuario(StringBuilder joins)
        {
            if (!joins.Contains(" Usuario "))
                joins.Append("left join T_FUNCIONARIO Usuario on Usuario.FUN_CODIGO = FilaCarregamentoVeiculoHistorico.FUN_CODIGO ");
        }

        #endregion

        #region Métodos Protegidos Sobrescritos

        protected override void SetarSelect(string propriedade, int codigoDinamico, StringBuilder select, StringBuilder joins, StringBuilder groupBy, bool somenteContarNumeroRegistros, Dominio.ObjetosDeValor.Embarcador.Logistica.FiltroPesquisaFilaCarregamentoVeiculoHistorico filtroPesquisa)
        {
            switch (propriedade)
            {
                case "Codigo":
                    if (!select.Contains(" Codigo"))
                    {
                        select.Append("FilaCarregamentoVeiculoHistorico.FVH_CODIGO as Codigo, ");
                        groupBy.Append("FilaCarregamentoVeiculoHistorico.FVH_CODIGO, ");
                    }
                    break;

                case "Data":
                case "DataFormatada":
                    if (!select.Contains(" Data"))
                    {
                        select.Append("FilaCarregamentoVeiculoHistorico.FVH_DATA as Data, ");
                        groupBy.Append("FilaCarregamentoVeiculoHistorico.FVH_DATA, ");
                    }
                    break;

                case "Descricao":
                    if (!select.Contains(" Descricao"))
                    {
                        select.Append("FilaCarregamentoVeiculoHistorico.FVH_DESCRICAO as Descricao, ");
                        groupBy.Append("FilaCarregamentoVeiculoHistorico.FVH_DESCRICAO, ");
                    }
                    break;

                case "FilaCarregamentoVeiculo":
                    if (!select.Contains(" NomeMotorista"))
                    {
                        select.Append("Motorista.FUN_NOME as NomeMotorista, ");
                        groupBy.Append("Motorista.FUN_NOME, ");

                        SetarJoinsMotorista(joins);
                    }

                    if (!select.Contains(" FilaCarregamentoVeiculo"))
                    {
                        select.Append("( ");
                        select.Append("    cast(FilaCarregamentoVeiculo.FLV_CODIGO as varchar(20)) + ");
                        select.Append("    case ");
                        select.Append("        when Motorista.FUN_NOME is null then '' ");
                        select.Append("        else ' - ' + Motorista.FUN_NOME ");
                        select.Append("    end ");
                        select.Append(") as FilaCarregamentoVeiculo, ");

                        groupBy.Append("FilaCarregamentoVeiculo.FLV_CODIGO, ");

                        SetarJoinsFilaCarregamentoVeiculo(joins);
                    }
                    break;

                case "ModeloVeicular":
                    if (!select.Contains(" ModeloVeicular"))
                    {
                        select.Append("ModeloVeicularCarga.MVC_DESCRICAO as ModeloVeicular, ");
                        groupBy.Append("ModeloVeicularCarga.MVC_DESCRICAO, ");

                        SetarJoinsModeloVeicularCarga(joins);
                    }
                    break;

                case "NomeMotorista":
                    if (!select.Contains(" NomeMotorista"))
                    {
                        select.Append("Motorista.FUN_NOME as NomeMotorista, ");
                        groupBy.Append("Motorista.FUN_NOME, ");

                        SetarJoinsMotorista(joins);
                    }
                    break;

                case "NomeUsuario":
                    if (!select.Contains(" NomeUsuario"))
                    {
                        select.Append("Usuario.FUN_NOME as NomeUsuario, ");
                        groupBy.Append("Usuario.FUN_NOME, ");

                        SetarJoinsUsuario(joins);
                    }
                    break;

                case "Posicao":
                    if (!select.Contains(" Posicao"))
                    {
                        select.Append("FilaCarregamentoVeiculoHistorico.FVH_POSICAO as Posicao, ");
                        groupBy.Append("FilaCarregamentoVeiculoHistorico.FVH_POSICAO, ");
                    }
                    break;

                case "Reboques":
                    if (!select.Contains(" Reboques"))
                    {
                        select.Append(" substring(( ");
                        select.Append("     select ', ' + Reboque.VEI_PLACA ");
                        select.Append("       from T_FILA_CARREGAMENTO_CONJUNTO_VEICULO_REBOQUE ConjuntoVeiculoReboque ");
                        select.Append("       join T_VEICULO Reboque on Reboque.VEI_CODIGO = ConjuntoVeiculoReboque.VEI_CODIGO ");
                        select.Append("      where ConjuntoVeiculoReboque.FCV_CODIGO_REBOQUE = FilaCarregamentoConjuntoVeiculo.FCV_CODIGO ");
                        select.Append("        for XML PATH('') ");
                        select.Append(" ), 3, 1000) as Reboques, ");

                        groupBy.Append("FilaCarregamentoConjuntoVeiculo.FCV_CODIGO, ");

                        SetarJoinsFilaCarregamentoConjuntoVeiculo(joins);
                    }
                    break;

                case "Tipo":
                case "TipoDescricao":
                    if (!select.Contains(" Tipo"))
                    {
                        select.Append("FilaCarregamentoVeiculoHistorico.FVH_TIPO as Tipo, ");
                        groupBy.Append("FilaCarregamentoVeiculoHistorico.FVH_TIPO, ");
                    }
                    break;

                case "Tracao":
                    if (!select.Contains(" Tracao"))
                    {
                        select.Append(" tracao.VEI_PLACA as Tracao, ");
                        groupBy.Append("tracao.VEI_PLACA, ");

                        SetarJoinsTracao(joins);
                    }
                    break;
                case "ObservacaoHistoricoVinculo":
                    if (!select.Contains(" ObservacaoHistoricoVinculo") && !somenteContarNumeroRegistros)
                    {
                        select.Append(" (SELECT TOP 1 THV_OBSERVACAO " +
                                         " FROM T_HISTORICO_VINCULO " +
                                         "WHERE FLV_CODIGO = FilaCarregamentoVeiculoHistorico.FLV_CODIGO " +
                                         "  AND FilaCarregamentoVeiculoHistorico.FVH_TIPO = 3 " +
                                         "ORDER BY THV_DATA_HORA_VINCULO DESC) as ObservacaoHistoricoVinculo, ");

                        if (!groupBy.Contains("FilaCarregamentoVeiculoHistorico.FLV_CODIGO"))
                            groupBy.Append("FilaCarregamentoVeiculoHistorico.FLV_CODIGO, ");

                        if (!groupBy.Contains("FilaCarregamentoVeiculoHistorico.FVH_TIPO"))
                            groupBy.Append("FilaCarregamentoVeiculoHistorico.FVH_TIPO, ");

                    }
                    break;
            }
        }

        protected override void SetarWhere(Dominio.ObjetosDeValor.Embarcador.Logistica.FiltroPesquisaFilaCarregamentoVeiculoHistorico filtrosPesquisa, StringBuilder where, StringBuilder joins, StringBuilder groupBy, List<Embarcador.Consulta.ParametroSQL> parametros = null)
        {
            string datePattern = "yyyy-MM-dd";

            SetarJoinsFilaCarregamentoVeiculo(joins);

            if (filtrosPesquisa.CodigoCentroCarregamento > 0)
                where.Append($" and FilaCarregamentoVeiculo.CEC_CODIGO = {filtrosPesquisa.CodigoCentroCarregamento}");

            if (filtrosPesquisa.CodigoGrupoModeloVeicularCarga > 0)
            {
                where.Append($" and ModeloVeicularCarga.MVG_CODIGO = {filtrosPesquisa.CodigoGrupoModeloVeicularCarga}");

                SetarJoinsModeloVeicularCarga(joins);
            }

            if (filtrosPesquisa.CodigoModeloVeicularCarga > 0)
            {
                where.Append($" and FilaCarregamentoConjuntoVeiculo.MVC_CODIGO = {filtrosPesquisa.CodigoModeloVeicularCarga}");

                SetarJoinsFilaCarregamentoConjuntoVeiculo(joins);
            }

            if (filtrosPesquisa.DataInicio.HasValue)
                where.Append($" and CAST(FilaCarregamentoVeiculoHistorico.FVH_DATA AS DATE) >= '{filtrosPesquisa.DataInicio.Value.ToString(datePattern)}'");

            if (filtrosPesquisa.DataLimite.HasValue)
                where.Append($" and CAST(FilaCarregamentoVeiculoHistorico.FVH_DATA AS DATE) <= '{filtrosPesquisa.DataLimite.Value.ToString(datePattern)}'");

            if (filtrosPesquisa.Tipo.HasValue)
                where.Append($" and FilaCarregamentoVeiculoHistorico.FVH_TIPO = {(int)filtrosPesquisa.Tipo.Value}");

            if (filtrosPesquisa.CodigoMotivoRetivadaFilaCarregamento > 0)
                where.Append($" and FilaCarregamentoVeiculoHistorico.FMR_CODIGO = {filtrosPesquisa.CodigoMotivoRetivadaFilaCarregamento}");

            if (filtrosPesquisa.CodigoMotorista > 0)
            {
                where.Append($" and FilaCarregamentoConjuntoMotorista.FCM_CODIGO_MOTORISTA = {filtrosPesquisa.CodigoMotorista}");

                SetarJoinsFilaCarregamentoConjuntoMotorista(joins);
            }

            if (filtrosPesquisa.CodigoVeiculo > 0)
            {
                where.Append(" and ( ");
                where.Append($"    (Tracao.VEI_CODIGO = {filtrosPesquisa.CodigoVeiculo}) or ");
                where.Append("     exists (");
                where.Append("         select 1 ");
                where.Append("           from T_FILA_CARREGAMENTO_CONJUNTO_VEICULO_REBOQUE ConjuntoVeiculoReboque ");
                where.Append($"         where ConjuntoVeiculoReboque.FCV_CODIGO_REBOQUE = FilaCarregamentoConjuntoVeiculo.FCV_CODIGO ");
                where.Append($"           and ConjuntoVeiculoReboque.VEI_CODIGO = {filtrosPesquisa.CodigoVeiculo} ");
                where.Append("     ) ");
                where.Append(" ) ");

                SetarJoinsTracao(joins);
            }
        }

        #endregion
    }
}
