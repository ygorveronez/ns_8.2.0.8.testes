using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Repositorio.Global
{
    sealed class ConsultaSerieDocumentos : Embarcador.Consulta.Consulta<Dominio.ObjetosDeValor.Embarcador.Documentos.FiltroPesquisaSerieDocumentos>
    {
        #region Construtores

        public ConsultaSerieDocumentos() : base(tabela: "T_EMPRESA as Empresa") { }

        #endregion

        #region Métodos Privados
        private void SetarJoinsEmpresaSerie(StringBuilder joins)
        {
            if (!joins.Contains(" EmpresaSerie "))
                joins.Append(" JOIN T_EMPRESA_SERIE EmpresaSerie ON EmpresaSerie.EMP_CODIGO = Empresa.EMP_CODIGO ");
        }

        private void SetarJoinsModeloDocumentoFiscalEmpresaSerie(StringBuilder joins)
        {
            SetarJoinsEmpresaSerie(joins);

            if (!joins.Contains(" ModeloDocumentoFiscalEmpresaSerie "))
                joins.Append(" JOIN T_MODELO_DOCUMENTO_FISCAL_EMPRESA_SERIE ModeloDocumentoFiscalEmpresaSerie ON ModeloDocumentoFiscalEmpresaSerie.ESE_CODIGO = EmpresaSerie.ESE_CODIGO ");
        }

        private void SetarJoinsModeloDocumentoFiscal(StringBuilder joins)
        {
            SetarJoinsModeloDocumentoFiscalEmpresaSerie(joins);

            if (!joins.Contains(" ModeloDocumentoFiscal "))
                joins.Append(" JOIN T_MODDOCFISCAL ModeloDocumentoFiscal ON ModeloDocumentoFiscal.MOD_CODIGO = ModeloDocumentoFiscalEmpresaSerie.MOD_CODIGO ");
        }
        #endregion

        #region Métodos Protegidos Sobrescritos

        protected override void SetarSelect(string propriedade, int codigoDinamico, StringBuilder select, StringBuilder joins, StringBuilder groupBy, bool somenteContarNumeroRegistros, Dominio.ObjetosDeValor.Embarcador.Documentos.FiltroPesquisaSerieDocumentos filtroPesquisa)
        {
            switch (propriedade)
            {
                case "RazaoSocialEmpresa":
                    if (!select.Contains(" RazaoSocialEmpresa, "))
                    {
                        select.Append("Empresa.EMP_RAZAO as RazaoSocialEmpresa, ");
                        groupBy.Append("Empresa.EMP_RAZAO, ");
                    }
                    break;

                case "CNPJEmpresaFormatado":
                    if (!select.Contains(" CNPJEmpresa "))
                    {
                        select.Append("Empresa.EMP_CNPJ as CNPJEmpresa, ");
                        groupBy.Append("Empresa.EMP_CNPJ, ");
                    }
                    break;

                case "TipoSerieFormatada":
                    if (!select.Contains(" TipoSerie, "))
                    {
                        select.Append("EmpresaSerie.ESE_TIPO as TipoSerie, ");
                        groupBy.Append("EmpresaSerie.ESE_TIPO, ");

                        SetarJoinsEmpresaSerie(joins);
                    }
                    break;

                case "CodigoSerie":
                    if (!select.Contains(" CodigoSerie, "))
                    {
                        select.Append("EmpresaSerie.ESE_CODIGO as CodigoSerie, ");
                        groupBy.Append("EmpresaSerie.ESE_CODIGO, ");

                        SetarJoinsEmpresaSerie(joins);
                    }
                    break;

                case "NumeroSerie":
                    if (!select.Contains(" NumeroSerie, "))
                    {
                        select.Append("EmpresaSerie.ESE_NUMERO as NumeroSerie, ");
                        groupBy.Append("EmpresaSerie.ESE_NUMERO, ");

                        SetarJoinsEmpresaSerie(joins);
                    }
                    break;

                case "StatusSerieFormatado":
                    if (!select.Contains(" StatusSerie, "))
                    {
                        select.Append("EmpresaSerie.ESE_STATUS as StatusSerie, ");
                        groupBy.Append("EmpresaSerie.ESE_STATUS, ");

                        SetarJoinsEmpresaSerie(joins);
                    }
                    break;

                case "StatusTransportadorFormatado":
                    if (!select.Contains(" StatusTransportador, "))
                    {
                        select.Append("Empresa.EMP_STATUS as StatusTransportador, ");
                        groupBy.Append("Empresa.EMP_STATUS, ");
                    }
                    break;

                case "ModeloDocumentoFiscal":
                    if (!select.Contains(" ModeloDocumentoFiscal, "))
                    {
                        select.Append("ModeloDocumentoFiscal.MOD_ABREVIACAO as ModeloDocumentoFiscal, ");
                        groupBy.Append("ModeloDocumentoFiscal.MOD_ABREVIACAO, ");

                        SetarJoinsModeloDocumentoFiscal(joins);
                    }
                    break;

                default:
                    break;

            }
        }

        protected override void SetarWhere(Dominio.ObjetosDeValor.Embarcador.Documentos.FiltroPesquisaSerieDocumentos filtrosPesquisa, StringBuilder where, StringBuilder joins, StringBuilder groupBy, List<Embarcador.Consulta.ParametroSQL> parametros = null)
        {

            if (filtrosPesquisa.CodigoEmpresa > 0)
                where.Append($" AND Empresa.EMP_CODIGO = {filtrosPesquisa.CodigoEmpresa}");

            if (filtrosPesquisa.CodigoSerie > 0)
            {
                where.Append($" AND EmpresaSerie.ESE_CODIGO = {filtrosPesquisa.CodigoSerie}");
                SetarJoinsEmpresaSerie(joins);
            }

            if (filtrosPesquisa.ModelosDocumentosFiscais?.Count > 0)
            {
                where.Append($" AND ModeloDocumentoFiscal.MOD_CODIGO in ({string.Join(", ", filtrosPesquisa.ModelosDocumentosFiscais)})");
                SetarJoinsModeloDocumentoFiscal(joins);
            }

        }

        #endregion
    }
}
