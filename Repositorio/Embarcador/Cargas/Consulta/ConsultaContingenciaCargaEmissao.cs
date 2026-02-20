using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Repositorio.Embarcador.Consulta;

namespace Repositorio.Embarcador.Cargas
{
    sealed class ConsultaContingenciaCargaEmissao
    {

        #region Métodos Publicos

        public SQLDinamico ObterSql(Dominio.ObjetosDeValor.Embarcador.Carga.FiltroPesquisaContingenciaCargaEmissao filtrosPesquisa, bool contar, Dominio.ObjetosDeValor.Embarcador.Consulta.ParametroConsulta parametrosConsulta, bool obterCodigos = false)
        {
            StringBuilder joins = new StringBuilder();
            StringBuilder select = new StringBuilder();
            StringBuilder sql = new StringBuilder();
            StringBuilder where = new StringBuilder();

            var parametros = new List<ParametroSQL>();

            SetarSelect(select, obterCodigos);
            SetarWhere(filtrosPesquisa, where, parametros);
            SetarJoins(joins);

            sql.Append($"select {(contar ? "count(1)" : select.ToString().Trim().Substring(0, select.Length - 1))} "); 
            sql.Append($" from T_CARGA Carga ");
            sql.Append(joins.ToString());
            sql.Append($" where {where.ToString().Trim().Substring(0)} ");

            if (parametrosConsulta != null)
            {
                sql.Append($" order by {parametrosConsulta.PropriedadeOrdenar} {(parametrosConsulta.DirecaoOrdenar == "asc" ? "asc" : "desc")} ");

                if ((parametrosConsulta.InicioRegistros > 0) || (parametrosConsulta.LimiteRegistros > 0))
                    sql.Append($" offset {parametrosConsulta.InicioRegistros} rows fetch next {parametrosConsulta.LimiteRegistros} rows only;");
            }

            return new SQLDinamico(sql.ToString(), parametros);
        }

        #endregion

        #region Métodos Privados

        private void SetarJoins(StringBuilder joins)
        {
            SetarJoinsCargaDadosSumarizados(joins);
            SetarJoinsTipoOperacao(joins);
            SetarJoinsEmpresa(joins);
            SetarJoinsFilial(joins);
        }

        private void SetarJoinsCargaDadosSumarizados(StringBuilder joins)
        {
            if (!joins.Contains(" CargaDadosSumarizados "))
                joins.Append(" left join T_CARGA_DADOS_SUMARIZADOS CargaDadosSumarizados on CargaDadosSumarizados.CDS_CODIGO = Carga.CDS_CODIGO");
        }

        private void SetarJoinsTipoOperacao(StringBuilder joins)
        {
            if (!joins.Contains(" TipoOperacao "))
                joins.Append(" left join T_TIPO_OPERACAO TipoOperacao on TipoOperacao.TOP_CODIGO = Carga.TOP_CODIGO ");
        }

        private void SetarJoinsEmpresa(StringBuilder joins)
        {
            if (!joins.Contains(" Empresa "))
                joins.Append(" left join T_EMPRESA Empresa on Empresa.EMP_CODIGO = Carga.EMP_CODIGO ");
        }

        private void SetarJoinsFilial(StringBuilder joins)
        {
            if (!joins.Contains(" Filial "))
                joins.Append(" left join T_FILIAL Filial on Filial.FIL_CODIGO = Carga.FIL_CODIGO ");
        }

        private void SetarSelect(StringBuilder select, bool obterCodigos)
        {
            select.Append("Carga.CAR_CODIGO Codigo,");
            if (obterCodigos)
                return;
            select.Append("Carga.CAR_CONTINGENCIA_EMISSAO ContingenciaEmissao,");
            select.Append("Carga.CAR_CODIGO_CARGA_EMBARCADOR NumeroCarga,");
            select.Append("Carga.CAR_DATA_CRIACAO DataCriacao,");
            select.Append("Carga.CAR_SITUACAO Situacao,");
            select.Append("TipoOperacao.TOP_DESCRICAO TipoOperacao,");
            select.Append("Empresa.EMP_CNPJ CnpjTransportador,");
            select.Append("Empresa.EMP_RAZAO RazaoTransportador,");
            select.Append("Carga.CAR_CODIGO Codigo,");
            select.Append("Filial.FIL_DESCRICAO Filial,");
        }

        private void SetarWhere(Dominio.ObjetosDeValor.Embarcador.Carga.FiltroPesquisaContingenciaCargaEmissao filtrosPesquisa, StringBuilder where, List<ParametroSQL> parametros)
        {
            string patternDate = "yyyy-MM-dd HH:mm:sss";
            string situacaoCargaPermitida = "(1, 2, 5, 4)";
            string operacaoTPeConsolidado = "(1, 2)";
            string cargaSubTrecho = "2";
            
            // Não pode retornar cargas com documento autorizado:
            where.Append($" Carga.CAR_CARGA_FECHADA = 1 and Carga.CAR_SITUACAO in {situacaoCargaPermitida} and TipoOperacao.TOP_TIPO_CONSOLIDACAO not in {operacaoTPeConsolidado} and (CargaDadosSumarizados.CDS_CARGA_TRECHO <> {cargaSubTrecho} or CargaDadosSumarizados.CDS_CARGA_TRECHO is null) ");
            where.Append($"and not exists (select top 1 1 from T_CARGA_CTE _cargaCTe left join T_CTE _cte on _cte.CON_CODIGO = _cargaCTe.CON_CODIGO where _cargaCTe.CAR_CODIGO = Carga.CAR_CODIGO and _cte.CON_STATUS = 'A') ");

            if (filtrosPesquisa.CargasEmContingencia)
                where.Append($"and Carga.CAR_CONTINGENCIA_EMISSAO = 1 ");
            else
                where.Append($"and (Carga.CAR_CONTINGENCIA_EMISSAO is null or Carga.CAR_CONTINGENCIA_EMISSAO = 0) ");

            if (!string.IsNullOrWhiteSpace(filtrosPesquisa.NumeroCarga))
            {
                where.Append($"and Carga.CAR_CODIGO_CARGA_EMBARCADOR like :CAR_CODIGO_CARGA_EMBARCADOR ");
                parametros.Add(new ParametroSQL("CAR_CODIGO_CARGA_EMBARCADOR", $"%{filtrosPesquisa.NumeroCarga}"));
            }

            if (filtrosPesquisa.DataCriacaoInicial != DateTime.MinValue)
                where.Append($"and Carga.CAR_DATA_CRIACAO > '{filtrosPesquisa.DataCriacaoInicial.ToString(patternDate)}' ");

            if (filtrosPesquisa.DataCriacaoFinal != DateTime.MinValue)
                where.Append($"and Carga.CAR_DATA_CRIACAO < '{filtrosPesquisa.DataCriacaoFinal.ToString(patternDate)}' ");

            if (filtrosPesquisa.SituacaoCarga != null && filtrosPesquisa.SituacaoCarga.Count > 0)
                where.Append($"and Carga.CAR_SITUACAO in ({string.Join(", ", filtrosPesquisa.SituacaoCarga.Select(x => (int)x).ToList())}) ");

            if (filtrosPesquisa.CodigosFilial.Count > 0)
                where.Append($"and Carga.FIL_CODIGO in ({string.Join(", ",filtrosPesquisa.CodigosFilial)}) ");

            if (filtrosPesquisa.CodigosEmpresa.Count > 0)
                where.Append($"and Carga.EMP_CODIGO in ({string.Join(", ", filtrosPesquisa.CodigosEmpresa)}) ");

            if (filtrosPesquisa.CodigosTipoCarga.Count > 0)
                where.Append($"and Carga.TCG_CODIGO in ({string.Join(", ", filtrosPesquisa.CodigosTipoCarga)}) ");

            if (filtrosPesquisa.CodigosTipoOperacao.Count > 0)
                where.Append($"and Carga.TOP_CODIGO in ({string.Join(", ", filtrosPesquisa.CodigosTipoOperacao)}) ");

        }

        #endregion
    }
}
