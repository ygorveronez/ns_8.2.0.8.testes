using Dominio.ObjetosDeValor.Embarcador.Pessoas;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Repositorio.Embarcador.Pessoas
{
    sealed class ConsultaEnderecoSecundario : Consulta.Consulta<Dominio.ObjetosDeValor.Embarcador.Pessoas.FiltroEnderecoSecundario>
    {
        #region Construtores
        
        public ConsultaEnderecoSecundario() : base(tabela: "T_CLIENTE_OUTRO_ENDERECO as EnderecoSecundario") { }

        #endregion

        #region MétodosPrivados

        private void SetarJoinsCliente(StringBuilder joins)
        {
            if (!joins.Contains(" Cliente "))
                joins.Append(" JOIN T_CLIENTE Cliente ON EnderecoSecundario.CLI_CGCCPF = Cliente.CLI_CGCCPF");
        }
        private void SetarJoinsLocalidade(StringBuilder joins)
        {
            if (!joins.Contains(" Localidade "))
                joins.Append(" JOIN T_LOCALIDADES Localidade ON EnderecoSecundario.LOC_CODIGO = Localidade.LOC_CODIGO");
        }

        #endregion

        #region Métodos Protegidos Sobrescritos

        protected override void SetarSelect(string propriedade, int codigoDinamico, StringBuilder select, StringBuilder joins, StringBuilder groupBy, bool somenteContarNumeroRegistros, FiltroEnderecoSecundario filtrosPesquisa)
        {
            switch (propriedade)
            {
                case "CodigoEmbarcador":
                    if (!select.Contains(" CodigoEmbarcador, "))
                    {
                        select.Append("EnderecoSecundario.COE_CODIGO_EMBARCADOR CodigoEmbarcador, ");

                        if (!groupBy.Contains("EnderecoSecundario.COE_CODIGO_EMBARCADOR"))
                            groupBy.Append("EnderecoSecundario.COE_CODIGO_EMBARCADOR, ");
                    }
                    break;
                case "Endereco":
                    if (!select.Contains(" Endereco, "))
                    {
                        select.Append("EnderecoSecundario.COE_ENDERECO Endereco, ");

                        if (!groupBy.Contains("EnderecoSecundario.COE_ENDERECO"))
                            groupBy.Append("EnderecoSecundario.COE_ENDERECO, ");
                    }
                    break;
                case "Bairro":
                    if (!select.Contains(" Bairro, "))
                    {
                        select.Append("EnderecoSecundario.COE_BAIRRO Bairro, ");

                        if (!groupBy.Contains("EnderecoSecundario.COE_BAIRRO"))
                            groupBy.Append("EnderecoSecundario.COE_BAIRRO, ");
                    }
                    break;
                case "Numero":
                    if (!select.Contains(" Numero, "))
                    {
                        select.Append("EnderecoSecundario.COE_NUMERO Numero, ");

                        if (!groupBy.Contains("EnderecoSecundario.COE_NUMERO"))
                            groupBy.Append("EnderecoSecundario.COE_NUMERO, ");
                    }
                    break;
                case "CPFClienteFormatado":
                    if (!select.Contains(" CPFCliente, "))
                    {
                        select.Append("Cliente.CLI_CGCCPF CPFCliente, ");

                        if (!groupBy.Contains("Cliente.CLI_CGCCPF"))
                            groupBy.Append("Cliente.CLI_CGCCPF, ");

                        SetarJoinsCliente(joins);
                    }
                    break;
                case "NomeCliente":
                    if (!select.Contains(" NomeCliente, "))
                    {
                        select.Append("Cliente.CLI_NOME NomeCliente, ");

                        if (!groupBy.Contains("Cliente.CLI_NOME"))
                            groupBy.Append("Cliente.CLI_NOME, ");

                        SetarJoinsCliente(joins);
                    }
                    break;
                case "Cidade":
                    if (!select.Contains(" Cidade, "))
                    {
                        select.Append("Localidade.LOC_DESCRICAO Cidade, ");

                        if (!groupBy.Contains("Localidade.LOC_DESCRICAO"))
                            groupBy.Append("Localidade.LOC_DESCRICAO, ");

                        SetarJoinsLocalidade(joins);
                    }
                    break;
            }
        }

        protected override void SetarWhere(FiltroEnderecoSecundario filtrosPesquisa, StringBuilder where, StringBuilder joins, StringBuilder groupBy, List<Embarcador.Consulta.ParametroSQL> parametros = null)
        {
            if (filtrosPesquisa.CPFCliente > 0)
                where.Append($"AND Cliente.CLI_CGCCPF = {filtrosPesquisa.CPFCliente}");
            if (filtrosPesquisa.Cidade > 0)
                where.Append($"AND Localidade.LOC_CODIGO = {filtrosPesquisa.Cidade}");
            if (!string.IsNullOrWhiteSpace(filtrosPesquisa.CodigoIntegracao))
                where.Append($"AND EnderecoSecundario.COE_CODIGO_EMBARCADOR = {filtrosPesquisa.CodigoIntegracao}");
        }

        #endregion
    }
}
