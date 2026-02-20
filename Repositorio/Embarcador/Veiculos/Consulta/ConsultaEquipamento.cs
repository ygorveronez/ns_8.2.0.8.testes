using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Repositorio.Embarcador.Veiculos.Consulta
{
    sealed class ConsultaEquipamento : Embarcador.Consulta.Consulta<Dominio.ObjetosDeValor.Embarcador.Veiculos.FiltroPesquisaRelatorioEquipamento>
    {
        #region Construtores

        public ConsultaEquipamento() : base(tabela: "T_EQUIPAMENTO as Equipamento") { }

        #endregion

        #region Métodos Privados

        private void SetarJoinsModeloEquipamento(StringBuilder joins)
        {
            if (!joins.Contains(" ModeloEquipamento "))
                joins.Append(" left join T_EQUIPAMENTO_MODELO ModeloEquipamento on ModeloEquipamento.EMO_CODIGO = Equipamento.EMO_CODIGO ");
        }

        private void SetarJoinsSegmentoVeiculo(StringBuilder joins)
        {
            if (!joins.Contains(" SegmentoVeiculo "))
                joins.Append(" left join T_VEICULO_SEGMENTO SegmentoVeiculo on SegmentoVeiculo.VSE_CODIGO = Equipamento.VSE_CODIGO ");
        }

        private void SetarJoinsMarcaEquipamento(StringBuilder joins)
        {
            if (!joins.Contains(" MarcaEquipamento "))
                joins.Append(" left join T_EQUIPAMENTO_MARCA MarcaEquipamento on MarcaEquipamento.EQM_CODIGO = Equipamento.EQM_CODIGO ");
        }

        private void SetarJoinsVeiculoEquipamento(StringBuilder joins)
        {
            if (!joins.Contains(" VeiculoEquipamento "))
                joins.Append(" left join T_VEICULO_EQUIPAMENTO VeiculoEquipamento on VeiculoEquipamento.EQP_CODIGO = Equipamento.EQP_CODIGO ");
        }

        private void SetarJoinsVeiculo(StringBuilder joins)
        {
            SetarJoinsVeiculoEquipamento(joins);

            if (!joins.Contains(" Veiculo "))
                joins.Append(" left join T_VEICULO Veiculo on Veiculo.VEI_CODIGO = VeiculoEquipamento.VEI_CODIGO ");
        }

        private void SetarJoinsCentroResultado(StringBuilder joins)
        {
            if (!joins.Contains(" CentroResultado "))
                joins.Append(" left join T_CENTRO_RESULTADO CentroResultado on CentroResultado.CRE_CODIGO = Equipamento.CRE_CODIGO ");
        }

        #endregion

        #region Métodos Protegidos Sobrescritos

        protected override void SetarSelect(string propriedade, int codigoDinamico, StringBuilder select, StringBuilder joins, StringBuilder groupBy, bool somenteContarNumeroRegistros, Dominio.ObjetosDeValor.Embarcador.Veiculos.FiltroPesquisaRelatorioEquipamento filtroPesquisa)
        {
            switch (propriedade)
            {
                case "Codigo":
                    if (!select.Contains(" Codigo, "))
                    {
                        select.Append("Equipamento.EQP_CODIGO as Codigo, ");
                        groupBy.Append("Equipamento.EQP_CODIGO, ");
                    }
                    break;

                case "Descricao":
                    if (!select.Contains(" Descricao, "))
                    {
                        select.Append("Equipamento.EQP_DESCRICAO as Descricao, ");
                        groupBy.Append("Equipamento.EQP_DESCRICAO, ");
                    }
                    break;

                case "Numero":
                    if (!select.Contains(" Numero, "))
                    {
                        select.Append("Equipamento.EQP_NUMERO as Numero, ");
                        groupBy.Append("Equipamento.EQP_NUMERO, ");
                    }
                    break;

                case "Chassi":
                    if (!select.Contains(" Chassi, "))
                    {
                        select.Append("Equipamento.EQP_CHASSI as Chassi, ");
                        groupBy.Append("Equipamento.EQP_CHASSI, ");
                    }
                    break;

                case "Situacao":
                    if (!select.Contains(" Situacao, "))
                    {
                        select.Append(@"   CASE WHEN Equipamento.EQP_ATIVO = 1 THEN 'Ativo' 
                                            ELSE 'Inativo' 
                                            END as Situacao, ");
                        groupBy.Append("Equipamento.EQP_ATIVO, ");
                    }
                    break;

                case "DataAquisicaoFormatada":
                    if (!select.Contains(" DataAquisicao, "))
                    {
                        select.Append("Equipamento.EQP_DATA_AQUISICAO as DataAquisicao, ");
                        groupBy.Append("Equipamento.EQP_DATA_AQUISICAO, ");
                    }
                    break;

                case "AnoFabricacao":
                    if (!select.Contains(" AnoFabricacao, "))
                    {
                        select.Append("Equipamento.EQP_ANO_FABRICACAO as AnoFabricacao, ");
                        groupBy.Append("Equipamento.EQP_ANO_FABRICACAO, ");
                    }
                    break;

                case "AnoModelo":
                    if (!select.Contains(" AnoModelo, "))
                    {
                        select.Append("Equipamento.EQP_ANO_MODELO as AnoModelo, ");
                        groupBy.Append("Equipamento.EQP_ANO_MODELO, ");
                    }
                    break;

                case "Modelo":
                    if (!select.Contains(" Modelo, "))
                    {
                        select.Append("ModeloEquipamento.EMO_DESCRICAO as Modelo, ");
                        groupBy.Append("ModeloEquipamento.EMO_DESCRICAO, ");

                        SetarJoinsModeloEquipamento(joins);
                    }
                    break;

                case "Marca":
                    if (!select.Contains(" Marca, "))
                    {
                        select.Append("MarcaEquipamento.EQM_DESCRICAO as Marca, ");
                        groupBy.Append("MarcaEquipamento.EQM_DESCRICAO, ");

                        SetarJoinsMarcaEquipamento(joins);
                    }
                    break;

                case "Segmento":
                    if (!select.Contains(" Segmento, "))
                    {
                        select.Append("SegmentoVeiculo.VSE_DESCRICAO as Segmento, ");
                        groupBy.Append("SegmentoVeiculo.VSE_DESCRICAO, ");

                        SetarJoinsSegmentoVeiculo(joins);
                    }
                    break;

                case "Hodometro":
                    if (!select.Contains(" Hodometro, "))
                    {
                        select.Append("Equipamento.EQP_HODOMETRO as Hodometro, ");
                        groupBy.Append("Equipamento.EQP_HODOMETRO, ");
                    }
                    break;

                case "Horimetro":
                    if (!select.Contains(" Horimetro, "))
                    {
                        select.Append("Equipamento.EQP_HORIMETRO as Horimetro, ");
                        groupBy.Append("Equipamento.EQP_HORIMETRO, ");
                    }
                    break;

                case "Observacao":
                    if (!select.Contains(" Observacao, "))
                    {
                        select.Append("Equipamento.EQP_OBSERVACAO as Observacao, ");
                        groupBy.Append("Equipamento.EQP_OBSERVACAO, ");
                    }
                    break;

                case "Placa":
                    if (!select.Contains(" Placa, "))
                    {
                        select.Append("Veiculo.VEI_PLACA as Placa, ");
                        groupBy.Append("Veiculo.VEI_PLACA, ");

                        SetarJoinsVeiculo(joins);
                    }
                    break;

                case "CentroResultado":
                    if (!select.Contains(" CentroResultado, "))
                    {
                        select.Append("CentroResultado.CRE_DESCRICAO as CentroResultado, ");
                        groupBy.Append("CentroResultado.CRE_DESCRICAO, ");

                        SetarJoinsCentroResultado(joins);
                    }
                    break;

                case "NeokohmDescricao":
                    if (!select.Contains(" Neokohm, "))
                    {
                        select.Append("Equipamento.EQP_NEOKOHM Neokohm, ");

                        if (!groupBy.Contains("Equipamento.EQP_NEOKOHM"))
                            groupBy.Append("Equipamento.EQP_NEOKOHM, ");
                    }
                    break;

            }
        }

        protected override void SetarWhere(Dominio.ObjetosDeValor.Embarcador.Veiculos.FiltroPesquisaRelatorioEquipamento filtrosPesquisa, StringBuilder where, StringBuilder joins, StringBuilder groupBy, List<Embarcador.Consulta.ParametroSQL> parametros = null)
        {
            string pattern = "yyyy-MM-dd";

            if (filtrosPesquisa.DataAquisicaoInicial != DateTime.MinValue)
                where.Append($" and CAST(Equipamento.EQP_DATA_AQUISICAO AS DATE) >= '{filtrosPesquisa.DataAquisicaoInicial.ToString(pattern)}'");

            if (filtrosPesquisa.DataAquisicaoFinal != DateTime.MinValue)
                where.Append($" and CAST(Equipamento.EQP_DATA_AQUISICAO AS DATE) <= '{filtrosPesquisa.DataAquisicaoFinal.ToString(pattern)}'");

            if (filtrosPesquisa.AnoFabricacao > 0)
                where.Append($" and Equipamento.EQP_ANO_FABRICACAO = {filtrosPesquisa.AnoFabricacao}");

            if (filtrosPesquisa.CodigoModelo > 0)
                where.Append($" and Equipamento.EMO_CODIGO = {filtrosPesquisa.CodigoModelo}");

            if (filtrosPesquisa.CodigoMarca > 0)
                where.Append($" and Equipamento.EQM_CODIGO = {filtrosPesquisa.CodigoMarca}");

            if (filtrosPesquisa.CodigosSegmento.Count > 0 )                
                where.Append($" and Equipamento.VSE_CODIGO in ({string.Join(",", filtrosPesquisa.CodigosSegmento)})");           

            if (filtrosPesquisa.CodigoCentroResultado > 0)
                where.Append($" and Equipamento.CRE_CODIGO = {filtrosPesquisa.CodigoCentroResultado}");

            if (filtrosPesquisa.CodigoVeiculo > 0)
            {
                SetarJoinsVeiculo(joins);
                where.Append($" and Veiculo.VEI_CODIGO = {filtrosPesquisa.CodigoVeiculo}");
            }

            if (filtrosPesquisa.Ativo == Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoAtivoPesquisa.Ativo)
                where.Append($" and Equipamento.EQP_ATIVO = 1");
            else if (filtrosPesquisa.Ativo == Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoAtivoPesquisa.Inativo)
                where.Append($" and Equipamento.EQP_ATIVO = 0");

            if (filtrosPesquisa.Neokohm == 1)
                where.Append(" AND Equipamento.EQP_NEOKOHM = 1");
            else if (filtrosPesquisa.Neokohm == 0)
                where.Append(" AND (Equipamento.EQP_NEOKOHM = 2 OR Equipamento.EQP_NEOKOHM IS NULL) ");
        }

        #endregion
    }
}
