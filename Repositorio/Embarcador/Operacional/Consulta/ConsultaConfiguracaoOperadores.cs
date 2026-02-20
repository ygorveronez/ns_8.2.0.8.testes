using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Repositorio.Embarcador.Operacional
{
    sealed class ConsultaConfiguracaoOperadores : Consulta.Consulta<Dominio.ObjetosDeValor.Embarcador.Operacional.FiltroPesquisaRelatorioConfiguracaoOperadores>
    {
        #region Construtores

        public ConsultaConfiguracaoOperadores() : base(tabela: "T_OPERADOR_LOGISTICA as OperadorLogistica ") { }

        #endregion

        #region Métodos Privados

        private void SetarJoinsFuncionario(StringBuilder joins)
        {
            if (!joins.Contains(" Funcionario "))
                joins.Append("inner join T_FUNCIONARIO Funcionario on Funcionario.FUN_CODIGO = OperadorLogistica.FUN_CODIGO ");
        }

        #endregion

        #region Métodos Protegidos Sobrescritos

        protected override void SetarSelect(string propriedade, int codigoDinamico, StringBuilder select, StringBuilder joins, StringBuilder groupBy, bool somenteContarNumeroRegistros, Dominio.ObjetosDeValor.Embarcador.Operacional.FiltroPesquisaRelatorioConfiguracaoOperadores filtrosPesquisa)
        {
            switch (propriedade)
            {

                case "Usuario":
                    if (!select.Contains(" Usuario, "))
                    {
                        select.Append("Funcionario.FUN_NOME as Usuario, ");
                        groupBy.Append("Funcionario.FUN_NOME, ");

                        SetarJoinsFuncionario(joins);
                    }
                    break;

                case "Filial":
                    if (!select.Contains(" Filial, "))
                    {
                        select.Append(@"(select STRING_AGG(Filial.FIL_DESCRICAO,', ') from T_OPERADOR_FILIAL opFil 
                                        inner join T_FILIAL Filial on Filial.FIL_CODIGO = opFil.FIL_CODIGO 
                                        where opFil.OPL_CODIGO = OperadorLogistica.OPL_CODIGO) Filial, ");

                        if (!groupBy.Contains("OperadorLogistica.OPL_CODIGO,"))
                            groupBy.Append("OperadorLogistica.OPL_CODIGO, ");
                    }
                    break;

                case "TipoOperacao":
                    if (!select.Contains(" TipoOperacao, "))
                    {
                        select.Append(@"(select STRING_AGG(TipoOperacao.TOP_DESCRICAO, ', ') from T_TIPO_OPERACAO TipoOperacao
                                            LEFT JOIN T_OPERADOR_LOGISTICA_TIPO_OPERACAO OperadorLogisticaTipoOperacao on OperadorLogisticaTipoOperacao.TOP_CODIGO = TipoOperacao.TOP_CODIGO 
                                            where OperadorLogisticaTipoOperacao.OPL_CODIGO = OperadorLogistica.OPL_CODIGO) TipoOperacao, ");

                        if (!groupBy.Contains("OperadorLogistica.OPL_CODIGO,"))
                            groupBy.Append("OperadorLogistica.OPL_CODIGO, ");
                    }
                    break;

                case "GrupoPessoas":
                    if (!select.Contains(" GrupoPessoas, "))
                    {
                        select.Append(@"(select STRING_AGG(GrupoPessoas.GRP_DESCRICAO, ', ') from T_GRUPO_PESSOAS GrupoPessoas
		                                    LEFT JOIN T_OPERADOR_LOGISTICA_GRUPO_PESSOAS LogisticaGrupoPessoas on LogisticaGrupoPessoas.GRP_CODIGO = GrupoPessoas.GRP_CODIGO
		                                    where LogisticaGrupoPessoas.OPL_CODIGO = OperadorLogistica.OPL_CODIGO) GrupoPessoas, ");

                        if (!groupBy.Contains("OperadorLogistica.OPL_CODIGO,"))
                            groupBy.Append("OperadorLogistica.OPL_CODIGO, ");
                    }
                    break;

                case "CentroCarregamento":
                    if (!select.Contains(" CentroCarregamento, "))
                    {
                        select.Append(@"(select STRING_AGG(CentroCarregamento.CEC_DESCRICAO, ', ') from T_CENTRO_CARREGAMENTO CentroCarregamento
		                                    LEFT JOIN T_OPERADOR_LOGISTICA_CENTRO_CARREGAMENTO LogisticaCentroCarregamento on LogisticaCentroCarregamento.CEC_CODIGO = CentroCarregamento.CEC_CODIGO
		                                    where LogisticaCentroCarregamento.OPL_CODIGO = OperadorLogistica.OPL_CODIGO) CentroCarregamento, ");

                        if (!groupBy.Contains("OperadorLogistica.OPL_CODIGO,"))
                            groupBy.Append("OperadorLogistica.OPL_CODIGO, ");
                    }
                    break;

                case "CentroDescarregamento":
                    if (!select.Contains(" CentroDescarregamento, "))
                    {
                        select.Append(@"(select STRING_AGG(CentroDescarregamento.CED_DESCRICAO, ', ') from T_CENTRO_DESCARREGAMENTO CentroDescarregamento
		                                    LEFT JOIN T_OPERADOR_LOGISTICA_CENTRO_DESCARREGAMENTO LogisticaCentroDescarregamento on LogisticaCentroDescarregamento.CED_CODIGO = CentroDescarregamento.CED_CODIGO
		                                    where LogisticaCentroDescarregamento.OPL_CODIGO = OperadorLogistica.OPL_CODIGO) CentroDescarregamento, ");

                        if (!groupBy.Contains("OperadorLogistica.OPL_CODIGO,"))
                            groupBy.Append("OperadorLogistica.OPL_CODIGO, ");
                    }
                    break;

                case "Cliente":
                    if (!select.Contains(" Cliente, "))
                    {
                        select.Append(@"(select STRING_AGG(Cliente.CLI_NOME, ', ') from T_OPERADOR_CLIENTE OperadorCliente
                                            LEFT JOIN T_OPERADOR_LOGISTICA LogisticaCliente on LogisticaCliente.OPL_CODIGO = OperadorCliente.OPC_CODIGO 
                                            LEFT JOIN T_CLIENTE Cliente on Cliente.CLI_CGCCPF = OperadorCliente.CLI_CGCCPF
                                            where OperadorCliente.OPL_CODIGO = OperadorLogistica.OPL_CODIGO) Cliente,  ");

                        if (!groupBy.Contains("OperadorLogistica.OPL_CODIGO,"))
                            groupBy.Append("OperadorLogistica.OPL_CODIGO, ");
                    }
                    break;

                case "TipoCarga":
                    if (!select.Contains(" TipoCarga, "))
                    {
                        select.Append(@"(select STRING_AGG(TipoCarga.TCG_DESCRICAO, ', ') from T_TIPO_DE_CARGA TipoCarga
                                      LEFT JOIN T_OPERADOR_TIPO_CARGA LogisticaTipoCarga on LogisticaTipoCarga.OTC_CODIGO = TipoCarga.TCG_CODIGO
                                      where LogisticaTipoCarga.OPL_CODIGO = OperadorLogistica.OPL_CODIGO) TipoCarga, ");

                        if (!groupBy.Contains("OperadorLogistica.OPL_CODIGO,"))
                            groupBy.Append("OperadorLogistica.OPL_CODIGO, ");
                    }
                    break;

                case "FilialVenda":
                    if (!select.Contains(" FilialVenda, "))
                    {
                        select.Append(@"(select STRING_AGG(FilialVenda.FIL_DESCRICAO, ', ') from T_FILIAL FilialVenda
		                                    LEFT JOIN T_OPERADOR_LOGISTICA_FILIAL_VENDA FiliaisVenda on FiliaisVenda.FIL_CODIGO = FilialVenda.FIL_CODIGO
		                                    where FiliaisVenda.OPL_CODIGO = OperadorLogistica.OPL_CODIGO) FilialVenda, ");

                        if (!groupBy.Contains("OperadorLogistica.OPL_CODIGO,"))
                            groupBy.Append("OperadorLogistica.OPL_CODIGO, ");
                    }
                    break;

                case "Transportador":
                    if (!select.Contains(" Transportador, "))
                    {
                        select.Append(@"(select STRING_AGG(Transportador.EMP_RAZAO, ', ') from T_EMPRESA Transportador
		                                    LEFT JOIN T_OPERADOR_LOGISTICA_TRANSPORTADOR LogisticaTransportador on LogisticaTransportador.EMP_CODIGO = Transportador.EMP_CODIGO
		                                    where LogisticaTransportador.OPL_CODIGO = OperadorLogistica.OPL_CODIGO) Transportador, ");

                        if (!groupBy.Contains("OperadorLogistica.OPL_CODIGO,"))
                            groupBy.Append("OperadorLogistica.OPL_CODIGO, ");
                    }
                    break;

                case "Recebedor":
                    if (!select.Contains(" Recebedor, "))
                    {
                        select.Append(@"(select STRING_AGG(Cliente.CLI_NOME, ', ') from T_OPERADOR_LOGISTICA_RECEBEDOR OperadorRecebedor
                                            LEFT JOIN T_OPERADOR_LOGISTICA LogisticaRecebedor on LogisticaRecebedor.OPL_CODIGO = OperadorRecebedor.OPL_CODIGO
                                            LEFT JOIN T_CLIENTE Cliente on Cliente.CLI_CGCCPF = OperadorRecebedor.CLI_CGCCPF
                                             where LogisticaRecebedor.OPL_CODIGO = OperadorLogistica.OPL_CODIGO) Recebedor, ");

                        if (!groupBy.Contains("OperadorLogistica.OPL_CODIGO,"))
                            groupBy.Append("OperadorLogistica.OPL_CODIGO, ");
                    }
                    break;

                case "Expedidor":
                    if (!select.Contains(" Expedidor, "))
                    {
                        select.Append(@"(select STRING_AGG(Cliente.CLI_NOME, ', ') from T_OPERADOR_LOGISTICA_EXPEDIDOR OperadorExpedidor
                                            LEFT JOIN T_OPERADOR_LOGISTICA LogisticaExpedidor on LogisticaExpedidor.OPL_CODIGO = OperadorExpedidor.OPL_CODIGO
                                            LEFT JOIN T_CLIENTE Cliente on Cliente.CLI_CGCCPF = OperadorExpedidor.CLI_CGCCPF
                                             where LogisticaExpedidor.OPL_CODIGO = OperadorLogistica.OPL_CODIGO) Expedidor, ");

                        if (!groupBy.Contains("OperadorLogistica.OPL_CODIGO,"))
                            groupBy.Append("OperadorLogistica.OPL_CODIGO, ");
                    }
                    break;

                case "TomadorGestaoDocumento":
                    if (!select.Contains(" TomadorGestaoDocumento, "))
                    {
                        select.Append(@"(select STRING_AGG(Tomador.CLI_NOME, ', ') from T_CLIENTE Tomador
		                                    LEFT JOIN T_OPERADOR_LOGISTICA_TOMADORES LogisticaTomador on LogisticaTomador.CLI_CGCCPF = Tomador.CLI_CGCCPF
		                                    where LogisticaTomador.OPL_CODIGO = OperadorLogistica.OPL_CODIGO) TomadorGestaoDocumento, ");

                        if (!groupBy.Contains("OperadorLogistica.OPL_CODIGO,"))
                            groupBy.Append("OperadorLogistica.OPL_CODIGO, ");
                    }
                    break;
            }
        }

        protected override void SetarWhere(Dominio.ObjetosDeValor.Embarcador.Operacional.FiltroPesquisaRelatorioConfiguracaoOperadores filtrosPesquisa, StringBuilder where, StringBuilder joins, StringBuilder groupBy, List<Embarcador.Consulta.ParametroSQL> parametros = null)
        {

            if (filtrosPesquisa.CodigoTipoCarga > 0)
            {
                where.Append($@" AND EXISTS (select TipoCarga.TCG_CODIGO from T_TIPO_DE_CARGA TipoCarga 
                                            inner join T_OPERADOR_TIPO_CARGA operadorTipoCarga on operadorTipoCarga.TCG_CODIGO = TipoCarga.TCG_CODIGO 
                                            where operadorTipoCarga.OPL_CODIGO = OperadorLogistica.OPL_CODIGO AND TipoCarga.TCG_CODIGO = {filtrosPesquisa.CodigoTipoCarga}) ");
            }

            if (filtrosPesquisa.CodigoTipoOperacao > 0)
            {
                where.Append($@" AND EXISTS (select TipoOperacao.TOP_CODIGO from T_TIPO_OPERACAO TipoOperacao 
                                            inner join T_OPERADOR_LOGISTICA_TIPO_OPERACAO LogisticaTipoOperacao on LogisticaTipoOperacao.TOP_CODIGO = TipoOperacao.TOP_CODIGO 
                                            where LogisticaTipoOperacao.OPL_CODIGO = OperadorLogistica.OPL_CODIGO AND TipoOperacao.TOP_CODIGO = {filtrosPesquisa.CodigoTipoOperacao}) ");
            }

            if (filtrosPesquisa.CodigoFilial > 0)
            {
                where.Append($@" AND EXISTS (select Filial.FIL_CODIGO from T_FILIAL Filial 
                                            inner join T_OPERADOR_FILIAL operadorFilial on operadorFilial.FIL_CODIGO = Filial.FIL_CODIGO 
                                            where operadorFilial.OPL_CODIGO = OperadorLogistica.OPL_CODIGO AND Filial.FIL_CODIGO = {filtrosPesquisa.CodigoFilial})");
            }

            if (filtrosPesquisa.CodigoUsuario > 0)
            {
                SetarJoinsFuncionario(joins);
                where.Append($" AND Funcionario.FUN_CODIGO = {filtrosPesquisa.CodigoUsuario} ");
            }
        }

        #endregion
    }
}
