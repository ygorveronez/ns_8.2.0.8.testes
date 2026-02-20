using Dominio.ObjetosDeValor.Embarcador.Carga;
using Repositorio.Embarcador.Consulta;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Repositorio.Embarcador.Cargas
{
    sealed class ConsultaModeloVeicularCarga : Consulta.Consulta<Dominio.ObjetosDeValor.Embarcador.Carga.FiltroPesquisaModeloVeicularCarga>
    {
        #region Construtores

        public ConsultaModeloVeicularCarga() : base(tabela: "T_MODELO_VEICULAR_CARGA AS ModeloVeicularCarga") { }

        #endregion

        #region Métodos Privados

        private void SetarJoinsGrupoModeloVeicular(StringBuilder joins)
        {
            if (!joins.Contains(" GrupoModeloVeicular "))
                joins.Append("left join T_MODELO_VEICULAR_CARGA_GRUPO GrupoModeloVeicular on GrupoModeloVeicular.MVG_CODIGO = ModeloVeicularCarga.MVG_CODIGO ");
        }

        private void SetarJoinsCodigosIntegracao(StringBuilder joins)
        {
            if (!joins.Contains(" CodigosIntegracao "))
                joins.Append("left join T_MODELO_VEICULAR_CODIGOS_INTEGRACAO CodigosIntegracao on CodigosIntegracao.MVC_CODIGO = ModeloVeicularCarga.MVC_CODIGO ");
        }

        #endregion

        #region Métodos Protegidos Sobrescritos

        protected override void SetarSelect(string propriedade, int codigoDinamico, StringBuilder select, StringBuilder joins, StringBuilder groupBy, bool somenteContarNumeroRegistros, FiltroPesquisaModeloVeicularCarga filtrosPesquisa)
        {
            switch (propriedade)
            {
                case "Descricao":
                    if (!select.Contains(" Descricao, "))
                    {
                        select.Append("ModeloVeicularCarga.MVC_DESCRICAO Descricao, ");
                        groupBy.Append("ModeloVeicularCarga.MVC_DESCRICAO, ");
                    }
                    break;

                case "CodigoIntegracao":
                    if (!select.Contains(" CodigoIntegracao, "))
                    {
                        select.Append("ModeloVeicularCarga.MVC_CODIGO_MODELO_VEICULAR_DE_CARGA_EMBARCADOR CodigoIntegracao, ");
                        groupBy.Append("ModeloVeicularCarga.MVC_CODIGO_MODELO_VEICULAR_DE_CARGA_EMBARCADOR, ");
                    }
                    break;

                case "CodigoIntegracaoGerenciadoraRisco":
                    if (!select.Contains(" CodigoIntegracaoGerenciadoraRisco, "))
                    {
                        select.Append("ModeloVeicularCarga.MVC_CODIGO_INTEGRACAO_GERENCIADORA_RISCO CodigoIntegracaoGerenciadoraRisco, ");
                        groupBy.Append("ModeloVeicularCarga.MVC_CODIGO_INTEGRACAO_GERENCIADORA_RISCO, ");
                    }
                    break;

                case "NumeroEixos":
                    if (!select.Contains(" NumeroEixos, "))
                    {
                        select.Append("ModeloVeicularCarga.MVC_NUMERO_EIXOS NumeroEixos, ");
                        groupBy.Append("ModeloVeicularCarga.MVC_NUMERO_EIXOS, ");
                    }
                    break;

                case "PadraoEixos":
                case "PadraoEixosFormatado":
                    if (!select.Contains(" PadraoEixos, "))
                    {
                        select.Append("ModeloVeicularCarga.MVC_PADRAO_EIXOS PadraoEixos, ");
                        groupBy.Append("ModeloVeicularCarga.MVC_PADRAO_EIXOS, ");
                    }
                    break;

                case "NumeroEixosSuspensos":
                    if (!select.Contains(" NumeroEixosSuspensos, "))
                    {
                        select.Append("ModeloVeicularCarga.MVC_NUMERO_EIXOS_SUSPENSOS NumeroEixosSuspensos, ");
                        groupBy.Append("ModeloVeicularCarga.MVC_NUMERO_EIXOS_SUSPENSOS, ");
                    }
                    break;

                case "Tipo":
                case "TipoFormatado":
                    if (!select.Contains(" Tipo, "))
                    {
                        select.Append("ModeloVeicularCarga.MVC_TIPO Tipo, ");
                        groupBy.Append("ModeloVeicularCarga.MVC_TIPO, ");
                    }
                    break;

                case "NumeroReboques":
                    if (!select.Contains(" NumeroReboques, "))
                    {
                        select.Append("ModeloVeicularCarga.MVC_NUMERO_REBOQUES NumeroReboques, ");
                        groupBy.Append("ModeloVeicularCarga.MVC_NUMERO_REBOQUES, ");
                    }
                    break;

                case "DiasRealizarProximoChecklist":
                    if (!select.Contains(" DiasRealizarProximoChecklist, "))
                    {
                        select.Append("ModeloVeicularCarga.MVC_DIAS_REALIZAR_PROXIMO_CHECKLIST DiasRealizarProximoChecklist, ");
                        groupBy.Append("ModeloVeicularCarga.MVC_DIAS_REALIZAR_PROXIMO_CHECKLIST, ");
                    }
                    break;

                case "Ativo":
                case "AtivoDescricao":
                    if (!select.Contains(" Ativo, "))
                    {
                        select.Append("ModeloVeicularCarga.MVC_ATIVO Ativo, ");
                        groupBy.Append("ModeloVeicularCarga.MVC_ATIVO, ");
                    }
                    break;

                case "GrupoModeloVeicular":
                    if (!select.Contains(" GrupoModeloVeicular, "))
                    {
                        select.Append("GrupoModeloVeicular.MVG_DESCRICAO GrupoModeloVeicular, ");
                        groupBy.Append("GrupoModeloVeicular.MVG_DESCRICAO, ");

                        SetarJoinsGrupoModeloVeicular(joins);

                    }
                    break;

                case "FatorEmissaoCO2":
                    if (!select.Contains(" FatorEmissaoCO2, "))
                    {
                        select.Append("ModeloVeicularCarga.MVC_FATOR_EMISSAO_CO2 FatorEmissaoCO2, ");
                        groupBy.Append("ModeloVeicularCarga.MVC_FATOR_EMISSAO_CO2, ");
                    }
                    break;

                case "CodigoTipoCargaANTT":
                    if (!select.Contains(" CodigoTipoCargaANTT, "))
                    {
                        select.Append("ModeloVeicularCarga.MVC_CODIGO_TIPO_CARGA_ANTT CodigoTipoCargaANTT, ");
                        groupBy.Append("ModeloVeicularCarga.MVC_CODIGO_TIPO_CARGA_ANTT, ");
                    }
                    break;

                case "VelocidadeMedia":
                    if (!select.Contains(" VelocidadeMedia, "))
                    {
                        select.Append("ModeloVeicularCarga.MVC_VELOCIDADE_MEDIA VelocidadeMedia, ");
                        groupBy.Append("ModeloVeicularCarga.MVC_VELOCIDADE_MEDIA, ");
                    }
                    break;

                case "UnidadeCapacidade":
                case "UnidadeCapacidadeFormatado":
                    if (!select.Contains(" UnidadeCapacidade, "))
                    {
                        select.Append("ModeloVeicularCarga.MVC_UNIDADE_CAPACIDADE UnidadeCapacidade, ");
                        groupBy.Append("ModeloVeicularCarga.MVC_UNIDADE_CAPACIDADE, ");
                    }
                    break;

                case "CapacidadePesoTransporte":
                    if (!select.Contains(" CapacidadePesoTransporte, "))
                    {
                        select.Append("ModeloVeicularCarga.MVC_CAPACIDADE_PESO_TRANSPORTE CapacidadePesoTransporte, ");
                        groupBy.Append("ModeloVeicularCarga.MVC_CAPACIDADE_PESO_TRANSPORTE, ");
                    }
                    break;

                case "ToleranciaPesoMenor":
                    if (!select.Contains(" ToleranciaPesoMenor, "))
                    {
                        select.Append("ModeloVeicularCarga.MVC_TOLERANCIA_PESO_MENOR ToleranciaPesoMenor, ");
                        groupBy.Append("ModeloVeicularCarga.MVC_TOLERANCIA_PESO_MENOR, ");
                    }
                    break;

                case "ToleranciaPesoExtra":
                    if (!select.Contains(" ToleranciaPesoExtra, "))
                    {
                        select.Append("ModeloVeicularCarga.MVC_TOLERANCIA_PESO_EXTRA ToleranciaPesoExtra, ");
                        groupBy.Append("ModeloVeicularCarga.MVC_TOLERANCIA_PESO_EXTRA, ");
                    }
                    break;

                case "ToleranciaMinimaPaletes":
                    if (!select.Contains(" ToleranciaMinimaPaletes, "))
                    {
                        select.Append("ModeloVeicularCarga.MVC_TOLERANCIA_MINIMA_PALLETS ToleranciaMinimaPaletes, ");
                        groupBy.Append("ModeloVeicularCarga.MVC_TOLERANCIA_MINIMA_PALLETS, ");
                    }
                    break;

                case "OcupacaoCubicaPaletes":
                    if (!select.Contains(" OcupacaoCubicaPaletes, "))
                    {
                        select.Append("ModeloVeicularCarga.MVC_OCUPACAO_CUBICA_PALETES OcupacaoCubicaPaletes, ");
                        groupBy.Append("ModeloVeicularCarga.MVC_OCUPACAO_CUBICA_PALETES, ");
                    }
                    break;

                case "CodigosIntegracao":
                    if (!select.Contains(" CodigosIntegracao, "))
                    {
                        select.Append(@"SUBSTRING(( (select ', ' + CAST(T_MODELO_VEICULAR_CODIGOS_INTEGRACAO.MVC_CODIGO AS VARCHAR(50)) 
	                                        from T_MODELO_VEICULAR_CODIGOS_INTEGRACAO
	                                        where T_MODELO_VEICULAR_CODIGOS_INTEGRACAO.MVC_CODIGO = ModeloVeicularCarga.MVC_CODIGO
	                                        for xml path(''))), 3, 1000) as CodigosIntegracao, ");
                        groupBy.Append("ModeloVeicularCarga.MVC_CODIGO , ");

                        SetarJoinsCodigosIntegracao(joins);
                    }
                    break;
            }
        }

        protected override void SetarWhere(FiltroPesquisaModeloVeicularCarga filtrosPesquisa, StringBuilder where, StringBuilder joins, StringBuilder groupBy, List<ParametroSQL> parametros)
        {
            if (!filtrosPesquisa.ModeloVeicular.IsNullOrEmpty())
                where.Append($" and ModeloVeicularCarga.MVC_CODIGO in ({ string.Join(",", filtrosPesquisa.ModeloVeicular) })");

            if (filtrosPesquisa.Ativo.HasValue)
                where.Append($" and ModeloVeicularCarga.MVC_ATIVO = {( filtrosPesquisa.Ativo.Value ? 1 : 0 )}");

            if (!filtrosPesquisa.Tipo.IsNullOrEmpty())
                where.Append($" and ModeloVeicularCarga.MVC_TIPO in ({string.Join(",", filtrosPesquisa.Tipo.Select(x => (int)x))})");
        }

        #endregion
    }
}
