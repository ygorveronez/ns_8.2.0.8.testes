using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Repositorio.Embarcador.Transportadores
{
    sealed class ConsultaLicencaVeiculo : Consulta.Consulta<Dominio.ObjetosDeValor.Embarcador.Administrativo.FiltroPesquisaRelatorioLicencaVeiculo>
    {
        #region Construtores

        public ConsultaLicencaVeiculo() : base(tabela: "T_VEICULO_LICENCA as Licenca") { }

        #endregion

        #region Métodos Privados

        private void SetarJoinsVeiculo(StringBuilder joins)
        {
            if (!joins.Contains(" Veiculo "))
                joins.Append("JOIN T_VEICULO Veiculo on Veiculo.VEI_CODIGO = Licenca.VEI_CODIGO ");
        }

        private void SetarJoinsLicenca(StringBuilder joins)
        {
            if (!joins.Contains(" Tipo "))
                joins.Append(" LEFT OUTER JOIN T_LICENCA Tipo on Tipo.LIC_CODIGO = Licenca.LIC_CODIGO ");
        }

        private void SetarJoinsCentroResultado(StringBuilder joins)
        {
            SetarJoinsVeiculo(joins);

            if (!joins.Contains(" Centro "))
                joins.Append(" LEFT OUTER JOIN T_CENTRO_RESULTADO Centro on Centro.CRE_CODIGO = Veiculo.CRE_CODIGO ");
        }

        private void SetarJoinsFuncionario(StringBuilder joins)
        {
            SetarJoinsVeiculo(joins);

            if (!joins.Contains(" Funcionario "))
                joins.Append(" LEFT OUTER JOIN T_FUNCIONARIO Funcionario on Funcionario.FUN_CODIGO = Veiculo.FUN_CODIGO_RESPONSAVEL ");
        }

        private void SetarJoinsModeloVeiculo(StringBuilder joins)
        {
            SetarJoinsVeiculo(joins);

            if (!joins.Contains(" Modelo "))
                joins.Append(" LEFT OUTER JOIN T_VEICULO_MODELO Modelo on Modelo.VMO_CODIGO = Veiculo.VMO_CODIGO ");
        }

        private void SetarJoinsMarcaVeiculo(StringBuilder joins)
        {
            SetarJoinsModeloVeiculo(joins);

            if (!joins.Contains(" Marca "))
                joins.Append(" LEFT OUTER JOIN T_VEICULO_MARCA Marca on Marca.VMA_CODIGO = Modelo.VMA_CODIGO ");
        }

        #endregion

        #region Métodos Protegidos Sobrescritos

        protected override void SetarSelect(string propriedade, int codigoDinamico, StringBuilder select, StringBuilder joins, StringBuilder groupBy, bool somenteContarNumeroRegistros, Dominio.ObjetosDeValor.Embarcador.Administrativo.FiltroPesquisaRelatorioLicencaVeiculo filtrosPesquisa)
        {
            switch (propriedade)
            {
                case "Codigo":
                    if (!select.Contains(" Codigo, "))
                    {
                        select.Append("Licenca.LIC_CODIGO Codigo, ");
                        groupBy.Append("Licenca.LIC_CODIGO, ");
                    }
                    break;

                case "NumeroLicenca":
                    if (!select.Contains(" NumeroLicenca, "))
                    {
                        select.Append("Licenca.VLI_NUMERO NumeroLicenca, ");
                        groupBy.Append("Licenca.VLI_NUMERO, ");
                    }
                    break;

                case "Descricao":
                    if (!select.Contains(" Descricao, "))
                    {
                        select.Append("Licenca.VLI_DESCRICAO Descricao, ");
                        groupBy.Append("Licenca.VLI_DESCRICAO, ");
                    }
                    break;

                case "DataEmissaoFormatada":
                    if (!select.Contains(" DataEmissao, "))
                    {
                        select.Append("Licenca.VLI_DATA_EMISSAO DataEmissao,");
                        groupBy.Append("Licenca.VLI_DATA_EMISSAO, ");

                    }
                    break;

                case "DataVencimentoFormatada":
                    if (!select.Contains(" DataVencimento, "))
                    {
                        select.Append("Licenca.VLI_DATA_VENCIMENTO DataVencimento,");
                        groupBy.Append("Licenca.VLI_DATA_VENCIMENTO, ");
                    }
                    break;

                case "TipoLicenca":
                    if (!select.Contains(" TipoLicenca, "))
                    {
                        select.Append("Tipo.LIC_DESCRICAO TipoLicenca, ");
                        groupBy.Append("Tipo.LIC_DESCRICAO, ");

                        SetarJoinsLicenca(joins);
                    }
                    break;

                case "Placa":
                    if (!select.Contains(" Placa, "))
                    {
                        select.Append("Veiculo.VEI_PLACA Placa, ");
                        groupBy.Append("Veiculo.VEI_PLACA, ");

                        SetarJoinsVeiculo(joins);

                        if (!groupBy.Contains("Veiculo.VEI_CODIGO, "))
                            groupBy.Append("Veiculo.VEI_CODIGO, ");
                    }
                    break;

                case "StatusVeiculoDescricao":
                    if (!select.Contains(" StatusVeiculo, "))
                    {
                        select.Append("Veiculo.VEI_ATIVO StatusVeiculo, ");
                        groupBy.Append("Veiculo.VEI_ATIVO, ");

                        SetarJoinsVeiculo(joins);

                        if (!groupBy.Contains("Veiculo.VEI_CODIGO, "))
                            groupBy.Append("Veiculo.VEI_CODIGO, ");
                    }
                    break;
                case "StatusLicencaDescricao":
                    if (!select.Contains(" StatusLicenca, "))
                    {
                        select.Append("Licenca.VLI_STATUS StatusLicenca, ");
                        groupBy.Append("Licenca.VLI_STATUS, ");

                    }
                    break;

                case "CentroResultado":
                    if (!select.Contains(" CentroResultado, "))
                    {
                        select.Append("Centro.CRE_DESCRICAO CentroResultado, ");
                        groupBy.Append("Centro.CRE_DESCRICAO, ");

                        SetarJoinsCentroResultado(joins);
                    }
                    break;

                case "NumeroFrota":
                    if (!select.Contains(" NumeroFrota, "))
                    {
                        select.Append("Veiculo.VEI_NUMERO_FROTA NumeroFrota, ");
                        groupBy.Append("Veiculo.VEI_NUMERO_FROTA, ");

                        SetarJoinsVeiculo(joins);

                        if (!groupBy.Contains("Veiculo.VEI_CODIGO, "))
                            groupBy.Append("Veiculo.VEI_CODIGO, ");
                    }
                    break;

                case "FuncionarioResponsavel":
                    if (!select.Contains(" FuncionarioResponsavel, "))
                    {
                        select.Append("Funcionario.FUN_NOME FuncionarioResponsavel, ");
                        groupBy.Append("Funcionario.FUN_NOME, ");

                        SetarJoinsFuncionario(joins);
                    }
                    break;

                case "Modelo":
                    if (!select.Contains(" Modelo, "))
                    {
                        select.Append("Modelo.VMO_DESCRICAO Modelo, ");
                        groupBy.Append("Modelo.VMO_DESCRICAO, ");

                        SetarJoinsModeloVeiculo(joins);
                    }
                    break;
                case "Marca":
                    if (!select.Contains(" Marca, "))
                    {
                        select.Append("Marca.VMA_DESCRICAO Marca, ");
                        groupBy.Append("Marca.VMA_DESCRICAO, ");

                        SetarJoinsMarcaVeiculo(joins);
                    }
                    break;
                case "Motorista":
                    if (!select.Contains(" Motorista, "))
                    {
                        select.Append(@"ISNULL(SUBSTRING(( SELECT ', ' + Motorista.FUN_NOME FROM T_VEICULO_MOTORISTA VeiculoMotorista 
                                                JOIN T_FUNCIONARIO Motorista ON VeiculoMotorista.FUN_CODIGO = Motorista.FUN_CODIGO 
                                                WHERE VeiculoMotorista.VEI_CODIGO = Veiculo.VEI_CODIGO FOR XML PATH('') ), 3, 1000), '') AS Motorista,");

                        if (!groupBy.Contains("Veiculo.VEI_CODIGO, "))
                            groupBy.Append("Veiculo.VEI_CODIGO, ");
                    }
                    break;
                case "Reboque":
                    if (!select.Contains(" Reboque, "))
                    {
                        select.Append(@"SUBSTRING((SELECT DISTINCT ', ' + VV.VEI_PLACA
                                        FROM T_VEICULO_CONJUNTO C
                                        JOIN T_VEICULO VV ON VV.VEI_CODIGO = C.VEC_CODIGO_FILHO
                                        WHERE C.VEC_CODIGO_PAI = Veiculo.VEI_CODIGO FOR XML PATH('')), 3, 2000) Reboque");

                        if (!groupBy.Contains("Veiculo.VEI_CODIGO, "))
                            groupBy.Append("Veiculo.VEI_CODIGO, ");
                    }
                    break;
                case "Renavam":
                    if (!select.Contains(" Renavam, "))
                    {
                        select.Append("Veiculo.VEI_RENAVAM Renavam, ");
                        groupBy.Append("Veiculo.VEI_RENAVAM, ");

                        SetarJoinsVeiculo(joins);
                    }
                    break;
            }
        }

        protected override void SetarWhere(Dominio.ObjetosDeValor.Embarcador.Administrativo.FiltroPesquisaRelatorioLicencaVeiculo filtrosPesquisa, StringBuilder where, StringBuilder joins, StringBuilder groupBy, List<Embarcador.Consulta.ParametroSQL> parametros = null)
        {
            string pattern = "yyyy-MM-dd";

            if (filtrosPesquisa.DataInicial != DateTime.MinValue)
                where.Append($" and Licenca.VLI_DATA_VENCIMENTO >= '{ filtrosPesquisa.DataInicial.ToString(pattern) }'");

            if (filtrosPesquisa.DataFinal != DateTime.MinValue)
                where.Append($" and Licenca.VLI_DATA_VENCIMENTO <= '{ filtrosPesquisa.DataFinal.AddDays(1).ToString(pattern) }'");

            if (!string.IsNullOrEmpty(filtrosPesquisa.Descricao))
                where.Append($" AND Licenca.VLI_DESCRICAO = '{filtrosPesquisa.Descricao}'");

            if (!string.IsNullOrEmpty(filtrosPesquisa.Renavam))
            {
                where.Append($" AND Veiculo.VEI_RENAVAM = '{filtrosPesquisa.Renavam}'");
                SetarJoinsVeiculo(joins);
            }

            if (!string.IsNullOrEmpty(filtrosPesquisa.NumeroLicenca)) { 
                where.Append($" AND Licenca.VLI_NUMERO = '{filtrosPesquisa.NumeroLicenca}'");}

            if (filtrosPesquisa.CodigoLicenca > 0)
            {
                where.Append($" and Tipo.LIC_CODIGO = {filtrosPesquisa.CodigoLicenca}");

                SetarJoinsLicenca(joins);
            }

            if (filtrosPesquisa.CodigoFuncionario > 0)
            {
                where.Append($" and Funcionario.FUN_CODIGO = {filtrosPesquisa.CodigoFuncionario}");

                SetarJoinsFuncionario(joins);
            }

            if (filtrosPesquisa.CodigoCentroResultado > 0)
            {
                where.Append($" and Centro.CRE_CODIGO = {filtrosPesquisa.CodigoCentroResultado}");

                SetarJoinsCentroResultado(joins);
            }

            if (filtrosPesquisa.CodigoMarca > 0)
            {
                where.Append($" and Marca.VMA_CODIGO = {filtrosPesquisa.CodigoMarca}");

                SetarJoinsMarcaVeiculo(joins);
            }

            if (filtrosPesquisa.CodigoModelo > 0)
            {
                where.Append($" and Modelo.VMO_CODIGO = {filtrosPesquisa.CodigoModelo}");

                SetarJoinsModeloVeiculo(joins);
            }

            if (filtrosPesquisa.StatusLicenca > 0)
                where.Append($" and Licenca.VLI_STATUS = {(int)filtrosPesquisa.StatusLicenca}");

            if (filtrosPesquisa.StatusVeiculo > 0)
            {
                where.Append($" and Veiculo.VEI_ATIVO = {(int)filtrosPesquisa.StatusVeiculo}");

                SetarJoinsVeiculo(joins);
            }
        }

        #endregion
    }
}
