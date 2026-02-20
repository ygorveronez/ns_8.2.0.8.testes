using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Repositorio.Embarcador.Frota
{
    sealed class ConsultaServicoVeiculo : Embarcador.Consulta.Consulta<Dominio.ObjetosDeValor.Embarcador.Frota.FiltroPesquisaRelatorioServicoVeiculo>
    {
        #region Construtores

        public ConsultaServicoVeiculo() : base(tabela: "T_FROTA_SERVICO_VEICULO as ServicoVeiculo") { }

        #endregion

        #region Métodos Privados



        #endregion

        #region Métodos Protegidos Sobrescritos

        protected override void SetarSelect(string propriedade, int codigoDinamico, StringBuilder select, StringBuilder joins, StringBuilder groupBy, bool somenteContarNumeroRegistros, Dominio.ObjetosDeValor.Embarcador.Frota.FiltroPesquisaRelatorioServicoVeiculo filtroPesquisa)
        {
            switch (propriedade)
            {
                case "Codigo":
                    if (!select.Contains(" Codigo, "))
                    {
                        select.Append("ServicoVeiculo.SEV_CODIGO as Codigo, ");
                        groupBy.Append("ServicoVeiculo.SEV_CODIGO, ");
                    }
                    break;

                case "Descricao":
                    if (!select.Contains(" Descricao, "))
                    {
                        select.Append("ServicoVeiculo.SEV_DESCRICAO as Descricao, ");
                        groupBy.Append("ServicoVeiculo.SEV_DESCRICAO, ");
                    }
                    break;

                case "CodigoIntegracao":
                    if (!select.Contains(" CodigoIntegracao, "))
                    {
                        select.Append("ServicoVeiculo.SEV_CODIGO_INTEGRACAO as CodigoIntegracao, ");
                        groupBy.Append("ServicoVeiculo.SEV_CODIGO_INTEGRACAO, ");
                    }
                    break;

                case "ValidadeKM":
                    if (!select.Contains(" ValidadeKM, "))
                    {
                        select.Append("ServicoVeiculo.SEV_VALIDADE_KM as ValidadeKM, ");
                        groupBy.Append("ServicoVeiculo.SEV_VALIDADE_KM, ");
                    }
                    break;

                case "ToleranciaKM":
                    if (!select.Contains(" ToleranciaKM, "))
                    {
                        select.Append("ServicoVeiculo.SEV_TOLERANCIA_KM as ToleranciaKM, ");
                        groupBy.Append("ServicoVeiculo.SEV_TOLERANCIA_KM, ");
                    }
                    break;

                case "ValidadeDias":
                    if (!select.Contains(" ValidadeDias, "))
                    {
                        select.Append("ServicoVeiculo.SEV_VALIDADE_DIAS as ValidadeDias, ");
                        groupBy.Append("ServicoVeiculo.SEV_VALIDADE_DIAS, ");
                    }
                    break;

                case "ToleranciaDias":
                    if (!select.Contains(" ToleranciaDias, "))
                    {
                        select.Append("ServicoVeiculo.SEV_TOLERANCIA_DIAS as ToleranciaDias, ");
                        groupBy.Append("ServicoVeiculo.SEV_TOLERANCIA_DIAS, ");
                    }
                    break;

                case "Observacao":
                    if (!select.Contains(" Observacao, "))
                    {
                        select.Append("ServicoVeiculo.SEV_OBSERVACAO as Observacao, ");
                        groupBy.Append("ServicoVeiculo.SEV_OBSERVACAO, ");
                    }
                    break;

                case "ExecucaoUnica":
                    if (!select.Contains(" ExecucaoUnica, "))
                    {
                        select.Append("CASE WHEN ServicoVeiculo.SEV_EXECUCAO_UNICA = 1 THEN 'Sim' ELSE 'Não' END as ExecucaoUnica, ");
                        groupBy.Append("ServicoVeiculo.SEV_EXECUCAO_UNICA, ");
                    }
                    break;

                case "PermiteLancamentoSemValor":
                    if (!select.Contains(" PermiteLancamentoSemValor, "))
                    {
                        select.Append("CASE WHEN ServicoVeiculo.SEV_PERMITE_LANCAMENTO_SEM_VALOR = 1 THEN 'Sim' ELSE 'Não' END as PermiteLancamentoSemValor, ");
                        groupBy.Append("ServicoVeiculo.SEV_PERMITE_LANCAMENTO_SEM_VALOR, ");
                    }
                    break;

                case "ObrigatorioParaRealizarCarga":
                    if (!select.Contains(" ObrigatorioParaRealizarCarga, "))
                    {
                        select.Append("CASE WHEN ServicoVeiculo.SEV_OBRIGATORIO_PARA_REALIZAR_CARGA = 1 THEN 'Sim' ELSE 'Não' END as ObrigatorioParaRealizarCarga, ");
                        groupBy.Append("ServicoVeiculo.SEV_OBRIGATORIO_PARA_REALIZAR_CARGA, ");
                    }
                    break;

                case "Situacao":
                    if (!select.Contains(" Situacao, "))
                    {
                        select.Append("CASE WHEN ServicoVeiculo.SEV_ATIVO = 1 THEN 'Ativo' ELSE 'Inativo' END as Situacao, ");
                        groupBy.Append("ServicoVeiculo.SEV_ATIVO, ");
                    }
                    break;

                case "MotivoFormatado":
                    if (!select.Contains(" Motivo, "))
                    {
                        select.Append("ServicoVeiculo.SEV_MOTIVO as Motivo, ");
                        groupBy.Append("ServicoVeiculo.SEV_MOTIVO, ");
                    }
                    break;

                case "TipoFormatado":
                    if (!select.Contains(" Tipo, "))
                    {
                        select.Append("ServicoVeiculo.SEV_TIPO as Tipo, ");
                        groupBy.Append("ServicoVeiculo.SEV_TIPO, ");
                    }
                    break;

                case "TipoManutencaoFormatado":
                    if (!select.Contains(" TipoManutencao, "))
                    {
                        select.Append("ServicoVeiculo.SEV_TIPO_MANUTENCAO as TipoManutencao, ");
                        groupBy.Append("ServicoVeiculo.SEV_TIPO_MANUTENCAO, ");
                    }
                    break;

                case "TempoEstimado":
                    if (!select.Contains(" TempoEstimado, "))
                    {
                        select.Append("ServicoVeiculo.SEV_TEMPO_ESTIMADO as TempoEstimado, ");
                        groupBy.Append("ServicoVeiculo.SEV_TEMPO_ESTIMADO, ");
                    }
                    break;

                case "ValidadeHorimetro":
                    if (!select.Contains(" ValidadeHorimetro, "))
                    {
                        select.Append("ServicoVeiculo.SEV_VALIDADE_HORIMETRO as ValidadeHorimetro, ");
                        groupBy.Append("ServicoVeiculo.SEV_VALIDADE_HORIMETRO, ");
                    }
                    break;

                case "ToleranciaHorimetro":
                    if (!select.Contains(" ToleranciaHorimetro, "))
                    {
                        select.Append("ServicoVeiculo.SEV_TOLERANCIA_HORIMETRO as ToleranciaHorimetro, ");
                        groupBy.Append("ServicoVeiculo.SEV_TOLERANCIA_HORIMETRO, ");
                    }
                    break;

                case "ServicoParaEquipamento":
                    if (!select.Contains(" ServicoParaEquipamento, "))
                    {
                        select.Append("CASE WHEN ServicoVeiculo.SEV_SERVICO_PARA_EQUIPAMENTO = 1 THEN 'Sim' ELSE 'Não' END as ServicoParaEquipamento, ");
                        groupBy.Append("ServicoVeiculo.SEV_SERVICO_PARA_EQUIPAMENTO, ");
                    }
                    break;

                case "GrupoServico":
                    if (!select.Contains(" GrupoServico, "))
                    {
                        select.Append(@"SUBSTRING((SELECT DISTINCT ', ' + grupoServico.GSF_DESCRICAO
		                                FROM T_GRUPO_SERVICO grupoServico 
                                        INNER JOIN T_GRUPO_SERVICO_SERVICO_VEICULO grupoServicoServicoVeiculo ON grupoServicoServicoVeiculo.GSF_CODIGO = grupoServico.GSF_CODIGO 
                                        WHERE grupoServicoServicoVeiculo.SEV_CODIGO = ServicoVeiculo.SEV_CODIGO FOR XML PATH('')), 3, 1000) GrupoServico, ");

                        if (!groupBy.Contains("ServicoVeiculo.SEV_CODIGO, "))
                            groupBy.Append("ServicoVeiculo.SEV_CODIGO, ");
                    }
                    break;


                case "ValidadeKMGrupo":
                    if (!select.Contains(" ValidadeKMGrupo, "))
                    {
                        select.Append(@"SUBSTRING((SELECT DISTINCT ', ' + CAST(grupoServicoServicoVeiculo.GSV_VALIDADE_KM AS NVARCHAR(10))
		                                FROM T_GRUPO_SERVICO grupoServico 
                                        INNER JOIN T_GRUPO_SERVICO_SERVICO_VEICULO grupoServicoServicoVeiculo ON grupoServicoServicoVeiculo.GSF_CODIGO = grupoServico.GSF_CODIGO 
                                        WHERE grupoServicoServicoVeiculo.SEV_CODIGO = ServicoVeiculo.SEV_CODIGO FOR XML PATH('')), 3, 1000) ValidadeKMGrupo, ");

                        if (!groupBy.Contains("ServicoVeiculo.SEV_CODIGO, "))
                            groupBy.Append("ServicoVeiculo.SEV_CODIGO, ");
                    }
                    break;

                case "ToleranciaKMGrupo":
                    if (!select.Contains(" ToleranciaKMGrupo, "))
                    {
                        select.Append(@"SUBSTRING((SELECT DISTINCT ', ' + CAST(grupoServicoServicoVeiculo.GSV_TOLERANCIA_KM AS NVARCHAR(10))
		                                FROM T_GRUPO_SERVICO grupoServico 
                                        INNER JOIN T_GRUPO_SERVICO_SERVICO_VEICULO grupoServicoServicoVeiculo ON grupoServicoServicoVeiculo.GSF_CODIGO = grupoServico.GSF_CODIGO 
                                        WHERE grupoServicoServicoVeiculo.SEV_CODIGO = ServicoVeiculo.SEV_CODIGO FOR XML PATH('')), 3, 1000) ToleranciaKMGrupo, ");

                        if (!groupBy.Contains("ServicoVeiculo.SEV_CODIGO, "))
                            groupBy.Append("ServicoVeiculo.SEV_CODIGO, ");
                    }
                    break;

                case "ValidadeDiasGrupo":
                    if (!select.Contains(" ValidadeDiasGrupo, "))
                    {
                        select.Append(@"SUBSTRING((SELECT DISTINCT ', ' + CAST(grupoServicoServicoVeiculo.GSV_VALIDADE_DIAS AS NVARCHAR(10))
		                                FROM T_GRUPO_SERVICO grupoServico 
                                        INNER JOIN T_GRUPO_SERVICO_SERVICO_VEICULO grupoServicoServicoVeiculo ON grupoServicoServicoVeiculo.GSF_CODIGO = grupoServico.GSF_CODIGO 
                                        WHERE grupoServicoServicoVeiculo.SEV_CODIGO = ServicoVeiculo.SEV_CODIGO FOR XML PATH('')), 3, 1000) ValidadeDiasGrupo, ");

                        if (!groupBy.Contains("ServicoVeiculo.SEV_CODIGO, "))
                            groupBy.Append("ServicoVeiculo.SEV_CODIGO, ");
                    }
                    break;

                case "ToleranciaDiasGrupo":
                    if (!select.Contains(" ToleranciaDiasGrupo, "))
                    {
                        select.Append(@"SUBSTRING((SELECT DISTINCT ', ' + CAST(grupoServicoServicoVeiculo.GSV_TOLERANCIA_DIAS AS NVARCHAR(10))
		                                FROM T_GRUPO_SERVICO grupoServico 
                                        INNER JOIN T_GRUPO_SERVICO_SERVICO_VEICULO grupoServicoServicoVeiculo ON grupoServicoServicoVeiculo.GSF_CODIGO = grupoServico.GSF_CODIGO 
                                        WHERE grupoServicoServicoVeiculo.SEV_CODIGO = ServicoVeiculo.SEV_CODIGO FOR XML PATH('')), 3, 1000) ToleranciaDiasGrupo, ");

                        if (!groupBy.Contains("ServicoVeiculo.SEV_CODIGO, "))
                            groupBy.Append("ServicoVeiculo.SEV_CODIGO, ");
                    }
                    break;
            }
        }

        protected override void SetarWhere(Dominio.ObjetosDeValor.Embarcador.Frota.FiltroPesquisaRelatorioServicoVeiculo filtrosPesquisa, StringBuilder where, StringBuilder joins, StringBuilder groupBy, List<Embarcador.Consulta.ParametroSQL> parametros = null)
        {
            if (filtrosPesquisa.CodigosServico.Count > 0)
                where.Append($" and ServicoVeiculo.SEV_CODIGO in ({string.Join(", ", filtrosPesquisa.CodigosServico)})");

            if (filtrosPesquisa.CodigoEmpresa > 0)
                where.Append($" and ServicoVeiculo.EMP_CODIGO = {filtrosPesquisa.CodigoEmpresa}");

            if (filtrosPesquisa.Situacao == Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoAtivoPesquisa.Ativo)
                where.Append($" and ServicoVeiculo.SEV_ATIVO = 1");
            else if (filtrosPesquisa.Situacao == Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoAtivoPesquisa.Inativo)
                where.Append($" and ServicoVeiculo.SEV_ATIVO = 0");

            if (filtrosPesquisa.TipoManutencao.HasValue)
                where.Append($" and ServicoVeiculo.SEV_TIPO_MANUTENCAO = {filtrosPesquisa.TipoManutencao.Value.ToString("d")}");

            if (filtrosPesquisa.Motivo.HasValue)
                where.Append($" and ServicoVeiculo.SEV_MOTIVO = {filtrosPesquisa.Motivo.Value.ToString("d")}");

            if (filtrosPesquisa.CodigoGrupoServico > 0)
                where.Append($" and exists (select grupoServicoServicoVeiculo.SEV_CODIGO from T_GRUPO_SERVICO_SERVICO_VEICULO grupoServicoServicoVeiculo where grupoServicoServicoVeiculo.GSF_CODIGO = {filtrosPesquisa.CodigoGrupoServico} and grupoServicoServicoVeiculo.SEV_CODIGO = ServicoVeiculo.SEV_CODIGO)"); // SQL-INJECTION-SAFE
        }

        #endregion
    }
}
