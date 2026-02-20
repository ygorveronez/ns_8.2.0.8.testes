using Dominio.Enumeradores;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Repositorio.Embarcador.Integracao
{
    sealed class ConsultaRelatorioIndicadorIntegracaoCTe : Consulta.Consulta<Dominio.ObjetosDeValor.Embarcador.Integracao.FiltroPesquisaRelatorioIndicadorIntegracaoCTe>
    {
        #region Construtores

        public ConsultaRelatorioIndicadorIntegracaoCTe() : base(tabela: "T_INDICADOR_INTEGRACAO_CTE IndicadorIntegracaoCTe") { }

        #endregion

        #region Métodos Privados 

        private void SetarJoinsCarga(StringBuilder joins)
        {
            SetarJoinsCargaCTe(joins);

            if (!joins.Contains(" Carga "))
                joins.AppendLine("join T_CARGA Carga on Carga.CAR_CODIGO = CargaCTe.CAR_CODIGO ");
        }

        private void SetarJoinsCargaCTe(StringBuilder joins)
        {
            if (!joins.Contains(" CargaCTe "))
                joins.AppendLine("join T_CARGA_CTE CargaCTe on CargaCTe.CCT_CODIGO = IndicadorIntegracaoCTe.CCT_CODIGO ");
        }

        private void SetarJoinsCTe(StringBuilder joins)
        {
            SetarJoinsCargaCTe(joins);

            if (!joins.Contains(" CTe "))
                joins.AppendLine("join T_CTE CTe on CTe.CON_CODIGO = CargaCTe.CON_CODIGO ");
        } 
        private void SetarJoinsModDocumentoFiscal(StringBuilder joins)
        {
            SetarJoinsCTe(joins);

            if (!joins.Contains(" ModDocFiscal "))
                joins.AppendLine("join T_MODDOCFISCAL ModDocFiscal on ModDocFiscal.MOD_CODIGO = CTe.CON_MODELODOC ");
        }

        private void SetarJoinsEmpresa(StringBuilder joins)
        {
            SetarJoinsCarga(joins);

            if (!joins.Contains(" Empresa "))
                joins.AppendLine("left join T_EMPRESA Empresa on Empresa.EMP_CODIGO = Carga.EMP_CODIGO ");
        }

        private void SetarJoinsEmpresaSerie(StringBuilder joins)
        {
            SetarJoinsCTe(joins);

            if (!joins.Contains(" EmpresaSerie "))
                joins.AppendLine("left join T_EMPRESA_SERIE EmpresaSerie on EmpresaSerie.ESE_CODIGO = CTe.CON_SERIE ");
        }

        private void SetarJoinsFilial(StringBuilder joins)
        {
            SetarJoinsCarga(joins);

            if (!joins.Contains(" Filial "))
                joins.AppendLine("left join T_FILIAL Filial on Filial.FIL_CODIGO = Carga.FIL_CODIGO ");
        }

        private void SetarJoinsLocalidade(StringBuilder joins)
        {
            SetarJoinsEmpresa(joins);

            if (!joins.Contains(" Localidade "))
                joins.AppendLine("left join T_LOCALIDADES Localidade on Localidade.LOC_CODIGO = Empresa.LOC_CODIGO ");
        }

        private void SetarJoinsLocalidadeEstado(StringBuilder joins)
        {
            SetarJoinsLocalidade(joins);

            if (!joins.Contains(" LocalidadeEstado "))
                joins.AppendLine("left join T_UF LocalidadeEstado on LocalidadeEstado.UF_SIGLA = Localidade.UF_SIGLA ");
        }

        private void SetarJoinsLocalidadePais(StringBuilder joins)
        {
            SetarJoinsLocalidade(joins);

            if (!joins.Contains(" LocalidadePais "))
                joins.AppendLine("left join T_PAIS LocalidadePais on LocalidadePais.PAI_CODIGO = Localidade.PAI_CODIGO ");
        }

        #endregion

        #region Métodos Protegidos Sobrescritos

        protected override void SetarSelect(string propriedade, int codigoDinamico, StringBuilder select, StringBuilder joins, StringBuilder groupBy, bool somenteContarNumeroRegistros, Dominio.ObjetosDeValor.Embarcador.Integracao.FiltroPesquisaRelatorioIndicadorIntegracaoCTe filtroPesquisa)
        {
            switch (propriedade)
            {
                case "ChaveCTe":
                    if (!select.Contains(" ChaveCTe,"))
                    {
                        select.AppendLine("CTe.CON_CHAVECTE as ChaveCTe, ");
                        groupBy.Append("CTe.CON_CHAVECTE, ");
                        SetarJoinsCTe(joins);
                    }
                    break;

                case "Codigo":
                    if (!select.Contains(" Codigo,"))
                    {
                        select.AppendLine("distinct CargaCTe.CCT_CODIGO as Codigo, ");
                        groupBy.Append("CargaCTe.CCT_CODIGO, ");
                        SetarJoinsCargaCTe(joins);
                    }
                    break;

                case "CodigoCargaEmbarcador":
                    if (!select.Contains(" CodigoCargaEmbarcador,"))
                    {
                        select.AppendLine("Carga.CAR_CODIGO_CARGA_EMBARCADOR as CodigoCargaEmbarcador, ");
                        groupBy.Append("Carga.CAR_CODIGO_CARGA_EMBARCADOR, ");
                        SetarJoinsCarga(joins);
                    }
                    break;

                case "DataEmissaoCTe":
                    if (!select.Contains(" DataEmissaoCTe,"))
                    {
                        select.AppendLine("case ");
                        select.AppendLine("    when CTe.CON_DATAHORAEMISSAO is null then '' ");
                        select.AppendLine("    else convert(varchar(10), CTe.CON_DATAHORAEMISSAO, 103) + ' ' + convert(varchar(5), CTe.CON_DATAHORAEMISSAO, 108) ");
                        select.AppendLine("end as DataEmissaoCTe, ");
                        groupBy.Append("CTe.CON_DATAHORAEMISSAO, ");
                        SetarJoinsCTe(joins);
                    }
                    break;

                case "DataIntegracao1":
                case "DataIntegracao2":
                case "DataIntegracao3":
                case "DataIntegracao4":
                case "DataIntegracao5":
                case "Integrado1":
                case "Integrado2":
                case "Integrado3":
                case "Integrado4":
                case "Integrado5":
                case "Integradoras":
                    if (!select.Contains(" Integradoras,"))
                    {
                        select.AppendLine("substring(( ");
                        select.AppendLine("    select ', ' + convert(varchar(20), Integradora.INT_CODIGO) + '|' + ");
                        select.AppendLine("           case ");
                        select.AppendLine("               when IndicadorIntegracaoCTeIntegradora.IIC_DATA_INTEGRACAO is null then '' ");
                        select.AppendLine("               else convert(varchar(10), IndicadorIntegracaoCTeIntegradora.IIC_DATA_INTEGRACAO, 103) + ' ' + convert(varchar(5), IndicadorIntegracaoCTeIntegradora.IIC_DATA_INTEGRACAO, 108) ");
                        select.AppendLine("           end ");
                        select.AppendLine("      from T_INDICADOR_INTEGRACAO_CTE IndicadorIntegracaoCTeIntegradora ");
                        select.AppendLine("      join T_INTEGRADORA Integradora on Integradora.INT_CODIGO = IndicadorIntegracaoCTeIntegradora.INT_CODIGO ");
                        select.AppendLine("     where IndicadorIntegracaoCTeIntegradora.CCT_CODIGO = IndicadorIntegracaoCTe.CCT_CODIGO ");
                        select.AppendLine("     order by Integradora.INT_DESCRICAO ");
                        select.AppendLine("       for xml path('') ");
                        select.AppendLine("), 2, 100) Integradoras, ");
                        groupBy.AppendLine("IndicadorIntegracaoCTe.CCT_CODIGO, ");
                    }
                    break;

                case "Filial":
                    if (!select.Contains(" Filial,"))
                    {
                        select.AppendLine("Filial.FIL_DESCRICAO as Filial, ");
                        groupBy.Append("Filial.FIL_DESCRICAO, ");
                        SetarJoinsFilial(joins);
                    }
                    break;

                case "NumeroCTe":
                    if (!select.Contains(" NumeroCTe,"))
                    {
                        select.AppendLine("CTe.CON_NUM as NumeroCTe, ");
                        groupBy.Append("CTe.CON_NUM, ");
                        SetarJoinsCTe(joins);
                    }
                    break;

                case "SerieCTe":
                    if (!select.Contains(" SerieCTe,"))
                    {
                        select.AppendLine("EmpresaSerie.ESE_NUMERO as SerieCTe, ");
                        groupBy.Append("EmpresaSerie.ESE_NUMERO, ");
                        SetarJoinsEmpresaSerie(joins);
                    }
                    break;

                case "Transportador":
                    if (!select.Contains(" Transportador,"))
                    {
                        select.AppendLine("Empresa.EMP_RAZAO + ");
                        select.AppendLine("case ");
                        select.AppendLine("    when Empresa.LOC_CODIGO is null then '' ");
                        select.AppendLine("    else ");
                        select.AppendLine("        ' (' + Localidade.LOC_DESCRICAO + ' - ' + ");
                        select.AppendLine("        case ");
                        select.AppendLine("            when (Localidade.LOC_IBGE <> 9999999 and Localidade.PAI_CODIGO is null) then isnull(LocalidadeEstado.UF_SIGLA, '') ");
                        select.AppendLine("            when (LocalidadePais.PAI_ABREVIACAO is null) then isnull(LocalidadePais.PAI_NOME, '') ");
                        select.AppendLine("            else isnull(LocalidadePais.PAI_ABREVIACAO, '') ");
                        select.AppendLine("        end + ')' ");
                        select.AppendLine("end as Transportador, ");
                        groupBy.Append("Empresa.EMP_RAZAO, Empresa.LOC_CODIGO, Localidade.LOC_DESCRICAO, Localidade.LOC_IBGE, Localidade.PAI_CODIGO, LocalidadeEstado.UF_SIGLA, LocalidadePais.PAI_ABREVIACAO, LocalidadePais.PAI_NOME, ");
                        SetarJoinsLocalidadeEstado(joins);
                        SetarJoinsLocalidadePais(joins);
                    }
                    break;
                case "ValorFreteSemImposto":
                    if(!select.Contains(" ValorFreteSemImposto,"))
                    {
                        select.Append("CTe.CON_VALOR_FRETE ValorFreteSemImposto, ");
                        groupBy.Append("CTe.CON_VALOR_FRETE, ");

                        SetarJoinsCTe(joins);
                    }
                    break;
                case "TipoDocumento":
                case "TipoDocumentoDescripcao":
                    if (!select.Contains(" TipoDocumento,")) {
                        select.Append("ModDocFiscal.MOD_TIPO_DOCUMENTO_EMISSAO TipoDocumento, ");
                        groupBy.Append("ModDocFiscal.MOD_TIPO_DOCUMENTO_EMISSAO, ");

                        SetarJoinsModDocumentoFiscal(joins);
                    }
                    break;
                case "StatusDocumentos":
                case "StatusDocumentosDescripcao":
                    if (!select.Contains(" StatusDocumentos,"))
                    {
                        select.Append("ModDocFiscal.MOD_STATUS StatusDocumentos, ");
                        groupBy.Append("ModDocFiscal.MOD_STATUS, ");

                        SetarJoinsModDocumentoFiscal(joins);
                    }
                    break;

            }
        }

        protected override void SetarWhere(Dominio.ObjetosDeValor.Embarcador.Integracao.FiltroPesquisaRelatorioIndicadorIntegracaoCTe filtrosPesquisa, StringBuilder where, StringBuilder joins, StringBuilder groupBy, List<Embarcador.Consulta.ParametroSQL> parametros = null)
        {
            if (filtrosPesquisa.DataEmissaoInicio.HasValue)
            { 
                where.AppendLine($" and CTe.CON_DATAHORAEMISSAO >= '{filtrosPesquisa.DataEmissaoInicio.Value.Date.ToString("yyyyMMdd HH:mm:ss")}' ");
                SetarJoinsCTe(joins);
            }

            if (filtrosPesquisa.DataEmissaoLimite.HasValue)
            {
                where.AppendLine($" and CTe.CON_DATAHORAEMISSAO <= '{filtrosPesquisa.DataEmissaoLimite.Value.Date.Add(DateTime.MaxValue.TimeOfDay).ToString("yyyyMMdd HH:mm:ss")}' ");
                SetarJoinsCTe(joins);
            }

            if (!string.IsNullOrWhiteSpace(filtrosPesquisa.CodigoCargaEmbarcador))
            {
                where.AppendLine($" and Carga.CAR_CODIGO_CARGA_EMBARCADOR = '{filtrosPesquisa.CodigoCargaEmbarcador}' ");
                SetarJoinsCarga(joins);
            }

            if (filtrosPesquisa.NumeroCTe > 0)
            {
                where.AppendLine($" and CTe.CON_NUM = {filtrosPesquisa.NumeroCTe} ");
                SetarJoinsCTe(joins);
            }

            if (filtrosPesquisa.CodigoFilial > 0)
            {
                where.AppendLine($" and Filial.FIL_CODIGO = {filtrosPesquisa.CodigoFilial} ");
                SetarJoinsFilial(joins);
            }

            if (filtrosPesquisa.CodigoTransportador > 0)
            {
                where.AppendLine($" and Empresa.EMP_CODIGO = {filtrosPesquisa.CodigoTransportador} ");
                SetarJoinsEmpresa(joins);
            }

            for (int i = 1; i <= 5; i++)
            {
                OpcaoSimNaoPesquisa integrado = (OpcaoSimNaoPesquisa)filtrosPesquisa.GetType().GetProperty($"Integrado{i}")?.GetValue(filtrosPesquisa);

                if (integrado != OpcaoSimNaoPesquisa.Todos)
                {
                    int codigoIntegradora = (int)filtrosPesquisa.GetType().GetProperty($"CodigoIntegradora{i}")?.GetValue(filtrosPesquisa);

                    where.AppendLine($" and {(integrado == OpcaoSimNaoPesquisa.Sim ? "" : "not")} exists ( ");
                    where.AppendLine("    select top (1) 1 ");
                    where.AppendLine("      from T_INDICADOR_INTEGRACAO_CTE IndicadorIntegracaoCTeIntegradora ");
                    where.AppendLine("     where IndicadorIntegracaoCTeIntegradora.CCT_CODIGO = IndicadorIntegracaoCTe.CCT_CODIGO ");
                    where.AppendLine($"      and IndicadorIntegracaoCTeIntegradora.INT_CODIGO = {codigoIntegradora} ");
                    where.AppendLine($"      and IndicadorIntegracaoCTeIntegradora.IIC_DATA_INTEGRACAO is not null ");
                    where.AppendLine(") ");
                }
            }
        }

        #endregion
    }
}
