using System;
using System.Text;

namespace Repositorio.Embarcador.Integracao
{
    sealed class ConsultaIndicadorIntegracaoCTe
    {
        #region Métodos Privados

        private string ObterSqlFiltrosPesquisa(Dominio.ObjetosDeValor.Embarcador.Integracao.FiltroPesquisaIndicadorIntegracaoCTe filtrosPesquisa)
        {
            StringBuilder sqlCondicao = new StringBuilder()
                .AppendLine($" where IndicadorIntegracaoCTe.IIC_DATA_INTEGRACAO is {(filtrosPesquisa.SomenteIntegrado ? "not " : "")}null ");

            if (filtrosPesquisa.CodigoFilial > 0)
                sqlCondicao.AppendLine($" and Filial.FIL_CODIGO = {filtrosPesquisa.CodigoFilial} ");

            if (filtrosPesquisa.CodigoTransportador > 0)
                sqlCondicao.AppendLine($" and Empresa.EMP_CODIGO = {filtrosPesquisa.CodigoTransportador} ");

            if (filtrosPesquisa.DataEmissaoInicio.HasValue)
                sqlCondicao.AppendLine($" and CTe.CON_DATAHORAEMISSAO >= '{filtrosPesquisa.DataEmissaoInicio.Value.Date.ToString("yyyyMMdd HH:mm:ss")}' ");

            if (filtrosPesquisa.DataEmissaoLimite.HasValue)
                sqlCondicao.AppendLine($" and CTe.CON_DATAHORAEMISSAO <= '{filtrosPesquisa.DataEmissaoLimite.Value.Date.Add(DateTime.MaxValue.TimeOfDay).ToString("yyyyMMdd HH:mm:ss")}' ");

            if (filtrosPesquisa.DataIntegracaoInicio.HasValue)
                sqlCondicao.AppendLine($" and IndicadorIntegracaoCTe.IIC_DATA_INTEGRACAO >= '{filtrosPesquisa.DataIntegracaoInicio.Value.Date.ToString("yyyyMMdd HH:mm:ss")}' ");

            if (filtrosPesquisa.DataIntegracaoLimite.HasValue)
                sqlCondicao.AppendLine($" and IndicadorIntegracaoCTe.IIC_DATA_INTEGRACAO <= '{filtrosPesquisa.DataIntegracaoLimite.Value.Date.Add(DateTime.MaxValue.TimeOfDay).ToString("yyyyMMdd HH:mm:ss")}' ");

            return sqlCondicao.ToString();
        }

        private string ObterSql(Dominio.ObjetosDeValor.Embarcador.Integracao.FiltroPesquisaIndicadorIntegracaoCTe filtrosPesquisa, Dominio.ObjetosDeValor.Embarcador.Consulta.ParametroConsulta parametrosConsulta, bool somenteContarNumeroRegistros)
        {
            StringBuilder sql = new StringBuilder();

            if (somenteContarNumeroRegistros)
                sql.AppendLine("select count(distinct CargaCTe.CCT_CODIGO) ");
            else
            {
                sql.AppendLine("select distinct CargaCTe.CCT_CODIGO as Codigo, ");
                sql.AppendLine("       Carga.CAR_CODIGO_CARGA_EMBARCADOR as CodigoCargaEmbarcador, ");
                sql.AppendLine("       Empresa.EMP_RAZAO + ");
                sql.AppendLine("       case ");
                sql.AppendLine("           when Empresa.LOC_CODIGO is null then '' ");
                sql.AppendLine("           else ");
                sql.AppendLine("               ' (' + Localidade.LOC_DESCRICAO + ' - ' + ");
                sql.AppendLine("               case ");
                sql.AppendLine("                   when (Localidade.LOC_IBGE <> 9999999 and Localidade.PAI_CODIGO is null) then isnull(LocalidadeEstado.UF_SIGLA, '') ");
                sql.AppendLine("                   when (LocalidadePais.PAI_ABREVIACAO is null) then isnull(LocalidadePais.PAI_NOME, '') ");
                sql.AppendLine("                   else isnull(LocalidadePais.PAI_ABREVIACAO, '') ");
                sql.AppendLine("               end + ')' ");
                sql.AppendLine("       end as Transportador, ");
                sql.AppendLine("       Filial.FIL_DESCRICAO as Filial, ");
                sql.AppendLine("       CTe.CON_NUM as NumeroCTe, ");
                sql.AppendLine("       CTe.CON_CHAVECTE as ChaveCTe, ");
                sql.AppendLine("       case ");
                sql.AppendLine("           when CTe.CON_DATAHORAEMISSAO is null then '' ");
                sql.AppendLine("           else convert(varchar(10), CTe.CON_DATAHORAEMISSAO, 103) + ' ' + convert(varchar(5), CTe.CON_DATAHORAEMISSAO, 108) ");
                sql.AppendLine("       end as DataEmissaoCTe, ");
                sql.AppendLine("       EmpresaSerie.ESE_NUMERO as SerieCTe, ");
                sql.AppendLine("       substring(( ");
                sql.AppendLine("           select ', ' + convert(varchar(20), Integradora.INT_CODIGO) + '|' + ");
                sql.AppendLine("                  case ");
                sql.AppendLine("                      when IndicadorIntegracaoCTeIntegradora.IIC_DATA_INTEGRACAO is null then '' ");
                sql.AppendLine("                      else convert(varchar(10), IndicadorIntegracaoCTeIntegradora.IIC_DATA_INTEGRACAO, 103) + ' ' + convert(varchar(5), IndicadorIntegracaoCTeIntegradora.IIC_DATA_INTEGRACAO, 108) ");
                sql.AppendLine("                  end ");
                sql.AppendLine("             from T_INDICADOR_INTEGRACAO_CTE IndicadorIntegracaoCTeIntegradora ");
                sql.AppendLine("             join T_INTEGRADORA Integradora on Integradora.INT_CODIGO = IndicadorIntegracaoCTeIntegradora.INT_CODIGO ");
                sql.AppendLine("            where IndicadorIntegracaoCTeIntegradora.CCT_CODIGO = IndicadorIntegracaoCTe.CCT_CODIGO ");
                sql.AppendLine("            order by Integradora.INT_DESCRICAO ");
                sql.AppendLine("              for xml path('') ");
                sql.AppendLine("	   ), 2, 100) Integradoras ");
            }

            sql.AppendLine("  from T_INDICADOR_INTEGRACAO_CTE IndicadorIntegracaoCTe ");
            sql.AppendLine("  join T_CARGA_CTE CargaCTe on CargaCTe.CCT_CODIGO = IndicadorIntegracaoCTe.CCT_CODIGO ");
            sql.AppendLine("  join T_CARGA Carga on Carga.CAR_CODIGO = CargaCTe.CAR_CODIGO ");
            sql.AppendLine("  join T_CTE CTe on CTe.CON_CODIGO = CargaCTe.CON_CODIGO ");
            sql.AppendLine("  left join T_FILIAL Filial on Filial.FIL_CODIGO = Carga.FIL_CODIGO ");
            sql.AppendLine("  left join T_EMPRESA Empresa on Empresa.EMP_CODIGO = Carga.EMP_CODIGO ");
            sql.AppendLine("  left join T_LOCALIDADES Localidade on Localidade.LOC_CODIGO = Empresa.LOC_CODIGO ");
            sql.AppendLine("  left join T_UF LocalidadeEstado on LocalidadeEstado.UF_SIGLA = Localidade.UF_SIGLA ");
            sql.AppendLine("  left join T_PAIS LocalidadePais on LocalidadePais.PAI_CODIGO = Localidade.PAI_CODIGO ");
            sql.AppendLine("  left join T_EMPRESA_SERIE EmpresaSerie on EmpresaSerie.ESE_CODIGO = CTe.CON_SERIE ");
            sql.AppendLine(ObterSqlFiltrosPesquisa(filtrosPesquisa));

            if (!somenteContarNumeroRegistros)
            {
                if (!string.IsNullOrWhiteSpace(parametrosConsulta.PropriedadeOrdenar))
                    sql.AppendLine($" order by {parametrosConsulta.PropriedadeOrdenar} {parametrosConsulta.DirecaoOrdenar}");

                if ((parametrosConsulta.InicioRegistros > 0) || (parametrosConsulta.LimiteRegistros > 0))
                    sql.AppendLine($" offset {parametrosConsulta.InicioRegistros} rows fetch next {parametrosConsulta.LimiteRegistros} rows only;");
            }

            return sql.ToString();
        }

        #endregion

        #region Métodos Públicos

        public string ObterSqlContarPesquisa(Dominio.ObjetosDeValor.Embarcador.Integracao.FiltroPesquisaIndicadorIntegracaoCTe filtrosPesquisa)
        {
            return ObterSql(filtrosPesquisa, parametrosConsulta: null, somenteContarNumeroRegistros: true);
        }

        public string ObterSqlPesquisa(Dominio.ObjetosDeValor.Embarcador.Integracao.FiltroPesquisaIndicadorIntegracaoCTe filtrosPesquisa, Dominio.ObjetosDeValor.Embarcador.Consulta.ParametroConsulta parametrosConsulta)
        {
            return ObterSql(filtrosPesquisa, parametrosConsulta, somenteContarNumeroRegistros: false);
        }

        #endregion
    }
}
