using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Repositorio.Embarcador.Consulta;

namespace Repositorio.Embarcador.Veiculos.Consulta
{
    sealed class VeiculoReceitaDespesa : Embarcador.Consulta.Consulta<Dominio.ObjetosDeValor.Embarcador.Veiculos.FiltroPesquisaReceitaDespesa>
    {
        #region Construtores

        public VeiculoReceitaDespesa() : base(tabela: "T_VEICULO_RECEITA_DESPESA as VeiculoReceitaDespesa") { }

        #endregion

        #region Métodos Privados

        private void SetarJoinsVeiculo(StringBuilder joins)
        {
            if (!joins.Contains(" Veiculo "))
                joins.Append("inner join T_VEICULO Veiculo on Veiculo.VEI_CODIGO = VeiculoReceitaDespesa.VEI_CODIGO ");
        }

        private void SetarJoinsModeloVeicular(StringBuilder joins)
        {
            SetarJoinsVeiculo(joins);

            if (!joins.Contains(" ModeloVeicular "))
                joins.Append("left join T_MODELO_VEICULAR_CARGA ModeloVeicularCarga on ModeloVeicularCarga.MVC_CODIGO = Veiculo.MVC_CODIGO ");
        }

        private void SetarJoinsSegmentoVeiculo(StringBuilder joins)
        {
            SetarJoinsVeiculo(joins);

            if (!joins.Contains(" SegmentoVeiculo "))
                joins.Append("left join T_VEICULO_SEGMENTO SegmentoVeiculo on SegmentoVeiculo.VSE_CODIGO = Veiculo.VSE_CODIGO ");
        }

        #endregion

        #region Métodos Protegidos Sobrescritos

        protected override SQLDinamico ObterSql(Dominio.ObjetosDeValor.Embarcador.Veiculos.FiltroPesquisaReceitaDespesa filtrosPesquisa, Dominio.ObjetosDeValor.Embarcador.Consulta.ParametroConsulta parametrosConsulta, List<Dominio.Relatorios.Embarcador.ObjetosDeValor.PropriedadeAgrupamento> propriedades, bool somenteContarNumeroRegistros)
        {
            StringBuilder groupBy = new StringBuilder();
            StringBuilder joins = new StringBuilder();
            StringBuilder orderBy = new StringBuilder();
            StringBuilder select = new StringBuilder();
            StringBuilder sql = new StringBuilder();
            StringBuilder where = new StringBuilder();

            foreach (Dominio.Relatorios.Embarcador.ObjetosDeValor.PropriedadeAgrupamento propriedade in propriedades)
                SetarSelect(propriedade.Propriedade, propriedade.CodigoDinamico, select, joins, groupBy, somenteContarNumeroRegistros, filtrosPesquisa);

            SetarOrderBy(parametrosConsulta, select, orderBy, joins, groupBy, somenteContarNumeroRegistros, filtrosPesquisa);
            SetarWhere(filtrosPesquisa, where, joins, groupBy);

            select.Append("( ");
            select.Append("    case VeiculoReceitaDespesa.VRD_TIPO ");
            select.Append("        when 0 then 'Receita' ");
            select.Append("        else 'Despesa' ");
            select.Append("    end ");
            select.Append("    + ");
            select.Append("    case VeiculoReceitaDespesa.VRD_ORIGEM ");
            select.Append("        when 1 then 'Titulo' ");
            select.Append("        when 2 then 'Abastecimento' ");
            select.Append("        when 3 then 'Pneu' ");
            select.Append("        when 4 then 'DocumentoEntrada' ");
            select.Append("        when 5 then 'CTe' ");
            select.Append("        when 6 then 'Pedagio' ");
            select.Append("        when 7 then 'OrdemServico' ");
            select.Append("        when 8 then 'AcertoViagem' ");
            select.Append("        else 'Outros' ");
            select.Append("    end ");
            select.Append(") Origem, ");
            select.Append("sum(VeiculoReceitaDespesa.VRD_VALOR) Valor, ");
            groupBy.Append("VeiculoReceitaDespesa.VRD_TIPO, VeiculoReceitaDespesa.VRD_ORIGEM, ");

            string campos = select.ToString().Trim();
            string agrupamentos = groupBy.ToString().Trim();
            string condicoes = where.ToString().Trim();

            if (somenteContarNumeroRegistros)
                sql.Append($"select count(0) from (select ");
            else
                sql.Append($"select * from (select ");

            sql.Append($"{campos.Substring(0, campos.Length - 1)} from {_tabela} ");
            sql.Append(joins.ToString());

            if (condicoes.Length > 0)
                sql.Append($" where {condicoes.ToString().Substring(3)} ");

            if (agrupamentos.Length > 0)
                sql.Append($" group by {agrupamentos.Substring(0, agrupamentos.Length - 1)} ");

            sql.Append(@") AS SourceTable PIVOT(SUM(Valor) FOR Origem IN (ReceitaTitulo, DespesaTitulo, ReceitaAbastecimento, DespesaAbastecimento, ReceitaPneu, DespesaPneu, ReceitaDocumentoEntrada, DespesaDocumentoEntrada, ReceitaCTe, DespesaCTe, ReceitaPedagio, DespesaPedagio, ReceitaOrdemServico, DespesaOrdemServico, ReceitaAcertoViagem, DespesaAcertoViagem, ReceitaOutros, DespesaOutros)) AS PivotTable");

            if (!somenteContarNumeroRegistros)
            {
                sql.Append($" order by {(orderBy.Length > 0 ? orderBy.ToString() : "1 asc")}");

                if ((parametrosConsulta != null) && ((parametrosConsulta.InicioRegistros > 0) || (parametrosConsulta.LimiteRegistros > 0)))
                    sql.Append($" offset {parametrosConsulta.InicioRegistros} rows fetch next {parametrosConsulta.LimiteRegistros} rows only;");
            }

            return new SQLDinamico(sql.ToString(), null);
        }

        protected override void SetarSelect(string propriedade, int codigoDinamico, StringBuilder select, StringBuilder joins, StringBuilder groupBy, bool somenteContarNumeroRegistros, Dominio.ObjetosDeValor.Embarcador.Veiculos.FiltroPesquisaReceitaDespesa filtroPesquisa)
        {
            switch (propriedade)
            {
                case "PlacaVeiculo":
                    if (!select.Contains(" PlacaVeiculo,"))
                    {
                        select.Append("Veiculo.VEI_PLACA PlacaVeiculo, Veiculo.VEI_CODIGO CodigoVeiculo, ");
                        groupBy.Append("Veiculo.VEI_PLACA, Veiculo.VEI_CODIGO, ");

                        SetarJoinsVeiculo(joins);
                    }
                    break;
                case "ModeloVeicular":
                    if (!select.Contains(" ModeloVeicular,"))
                    {
                        select.Append("ModeloVeicularCarga.MVC_DESCRICAO ModeloVeicular, ");
                        groupBy.Append("ModeloVeicularCarga.MVC_DESCRICAO, ");

                        SetarJoinsModeloVeicular(joins);
                    }
                    break;
                case "SegmentoVeiculo":
                    if (!select.Contains(" SegmentoVeiculo,"))
                    {
                        select.Append("SegmentoVeiculo.VSE_DESCRICAO SegmentoVeiculo, ");
                        groupBy.Append("SegmentoVeiculo.VSE_DESCRICAO, ");

                        SetarJoinsSegmentoVeiculo(joins);
                    }
                    break;
            }
        }

        protected override void SetarWhere(Dominio.ObjetosDeValor.Embarcador.Veiculos.FiltroPesquisaReceitaDespesa filtrosPesquisa, StringBuilder where, StringBuilder joins, StringBuilder groupBy, List<Embarcador.Consulta.ParametroSQL> parametros = null)
        {
            if (filtrosPesquisa.CodigoModeloVeicular > 0)
            {
                where.Append($" and Veiculo.MVC_CODIGO = {filtrosPesquisa.CodigoModeloVeicular}");

                SetarJoinsVeiculo(joins);
            }

            if (filtrosPesquisa.CodigoSegmentoVeiculo > 0)
            {
                where.Append($" and Veiculo.VSE_CODIGO = {filtrosPesquisa.CodigoSegmentoVeiculo}");

                SetarJoinsVeiculo(joins);
            }

            if (filtrosPesquisa.CodigoVeiculo > 0)
                where.Append($" and VeiculoReceitaDespesa.VEI_CODIGO = {filtrosPesquisa.CodigoVeiculo}");

            if (filtrosPesquisa.DataInicial.HasValue)
                where.Append($" and VeiculoReceitaDespesa.VRD_DATA >= '{filtrosPesquisa.DataInicial.Value.ToString("yyyy-MM-dd")}'");

            if (filtrosPesquisa.DataFinal.HasValue)
                where.Append($" and VeiculoReceitaDespesa.VRD_DATA < '{filtrosPesquisa.DataFinal.Value.AddDays(1).ToString("yyyy-MM-dd")}'");
        }

        #endregion
    }
}
