using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Repositorio.Embarcador.Pallets
{
    sealed class ConsultaTaxasDescarga : Consulta.Consulta<Dominio.ObjetosDeValor.Embarcador.Pallets.FiltroPesquisaTaxasDescarga>
    {
        #region Construtores

        public ConsultaTaxasDescarga() : base(tabela: "T_CONFIGURACAO_DESCARGA_CLIENTE as DescargaCliente") { }

        #endregion

        #region Métodos Privados

        private void SetarJoinsFilial(StringBuilder joins)
        {
            if (!joins.Contains(" Filial "))
                joins.Append(" left join T_FILIAL Filial on DescargaCliente.FIL_CODIGO = Filial.FIL_CODIGO ");
        }
        private void SetarJoinsModeloVeicular(StringBuilder joins)
        {
            if (!joins.Contains(" ModeloVeicular "))
                joins.Append(" left join T_MODELO_VEICULAR_CARGA ModeloVeicular on DescargaCliente.MVC_CODIGO = ModeloVeicular.MVC_CODIGO ");
        }
        private void SetarJoinsTipoCarga(StringBuilder joins)
        {
            if (!joins.Contains(" TipoCarga "))
                joins.Append(" left join T_TIPO_DE_CARGA TipoCarga on DescargaCliente.TCG_CODIGO = TipoCarga.TCG_CODIGO ");
        }

        private void SetarJoinsDescargaClienteCliente(StringBuilder joins)
        {
            if (!joins.Contains(" DescargaClienteCliente "))
                joins.Append(" left join T_CONFIGURACAO_DESCARGA_CLIENTE_CLIENTE DescargaClienteCliente on DescargaCliente.CDC_CODIGO = DescargaClienteCliente.CDC_CODIGO ");
        }
        private void SetarJoinsCliente(StringBuilder joins)
        {
            SetarJoinsDescargaClienteCliente(joins);

            if (!joins.Contains(" Cliente "))
                joins.Append(" left join T_CLIENTE Cliente on DescargaClienteCliente.CLI_CGCCPF = Cliente.CLI_CGCCPF ");
        }
        private void SetarJoinsLocalidade(StringBuilder joins)
        {
            SetarJoinsCliente(joins);

            if (!joins.Contains(" Localidades "))
                joins.Append(" left join T_LOCALIDADES Localidades on Localidades.LOC_CODIGO = Cliente.LOC_CODIGO ");
        }

        private void SetarJoinsDescargaClienteGrupoPessoas(StringBuilder joins)
        {
            if (!joins.Contains(" DescargaClienteGrupoPessoas "))
                joins.Append(" left join T_CONFIGURACAO_DESCARGA_CLIENTE_GRUPO_PESSOA DescargaClienteGrupoPessoas on DescargaCliente.CDC_CODIGO = DescargaClienteGrupoPessoas.CDC_CODIGO ");
        }
        private void SetarJoinsGrupoCliente(StringBuilder joins)
        {
            SetarJoinsDescargaClienteGrupoPessoas(joins);

            if (!joins.Contains(" GrupoCliente "))
                joins.Append(" left join T_GRUPO_PESSOAS GrupoCliente on DescargaClienteGrupoPessoas.GRP_CODIGO = GrupoCliente.GRP_CODIGO ");
        }
        private void SetarJoinsDescargaClienteTipoOperacao(StringBuilder joins)
        {
            if (!joins.Contains(" DescargaClienteTipoOperacao "))
                joins.Append(" left join T_CONFIGURACAO_DESCARGA_TIPO_OPERACAO DescargaClienteTipoOperacao on DescargaCliente.CDC_CODIGO = DescargaClienteTipoOperacao.CDC_CODIGO ");
        }
        private void SetarJoinsTipoOperacao(StringBuilder joins)
        {
            SetarJoinsDescargaClienteTipoOperacao(joins);

            if (!joins.Contains(" TipoOperacao "))
                joins.Append(" left join T_TIPO_OPERACAO TipoOperacao on DescargaClienteTipoOperacao.TOP_CODIGO = TipoOperacao.TOP_CODIGO ");
        }

        private void SetarJoinsUsuario(StringBuilder joins)
        {
            if (!joins.Contains(" Funcionario "))
                joins.Append(" left join T_FUNCIONARIO Funcionario on DescargaCliente.FUN_CODIGO = Funcionario.FUN_CODIGO ");
        }

        #endregion

        protected override void SetarSelect(string propriedade, int codigoDinamico, StringBuilder select, StringBuilder joins, StringBuilder groupBy, bool somenteContarNumeroRegistros, Dominio.ObjetosDeValor.Embarcador.Pallets.FiltroPesquisaTaxasDescarga filtroPesquisa)
        {

            switch (propriedade)
            {
                case "Codigo":
                    if (!select.Contains(" Codigo,"))
                    {
                        select.Append("DescargaCliente.CDC_CODIGO as Codigo, ");
                        groupBy.Append("DescargaCliente.CDC_CODIGO, ");
                    }
                    break;

                case "Filial":
                    if (!select.Contains(" Filial,"))
                    {
                        select.Append("Filial.FIL_DESCRICAO as Filial, ");
                        groupBy.Append("Filial.FIL_DESCRICAO, ");

                        SetarJoinsFilial(joins);
                    }
                    break;

                case "ModeloVeicularFormatado":
                    if (!select.Contains(" ModeloVeicular,"))
                    {
                        select.Append("ModeloVeicular.MVC_DESCRICAO as ModeloVeicular, ");
                        groupBy.Append("ModeloVeicular.MVC_DESCRICAO, ");

                        select.Append("ModeloVeicular.MVC_CODIGO_MODELO_VEICULAR_DE_CARGA_EMBARCADOR as ModeloVeicularCodigoIntegracao, ");
                        groupBy.Append("ModeloVeicular.MVC_CODIGO_MODELO_VEICULAR_DE_CARGA_EMBARCADOR, ");

                        SetarJoinsModeloVeicular(joins);
                    }
                    break;

                case "TipoCarga":
                    if (!select.Contains(" TipoCarga,"))
                    {
                        select.Append("TipoCarga.TCG_DESCRICAO as TipoCarga, ");
                        groupBy.Append("TipoCarga.TCG_DESCRICAO, ");

                        SetarJoinsTipoCarga(joins);
                    }
                    break;

                case "StatusDescricao":
                    if (!select.Contains(" Status,"))
                    {
                        select.Append("DescargaCliente.CDC_ATIVO as Status, ");
                        groupBy.Append("DescargaCliente.CDC_ATIVO, ");
                    }
                    break;

                case "DataInicioVigenciaFormatada":
                    if (!select.Contains(" DataInicioVigencia,"))
                    {
                        select.Append("DescargaCliente.CDC_INICIO_VIGENCIA as DataInicioVigencia, ");
                        groupBy.Append("DescargaCliente.CDC_INICIO_VIGENCIA, ");
                    }
                    break;

                case "DataFimVigenciaFormatada":
                    if (!select.Contains(" DataFimVigencia,"))
                    {
                        select.Append("DescargaCliente.CDC_FIM_VIGENCIA as DataFimVigencia, ");
                        groupBy.Append("DescargaCliente.CDC_FIM_VIGENCIA, ");
                    }
                    break;

                case "GrupoCliente":
                    if (!select.Contains(" GrupoCliente,"))
                    {
                        select.Append("GrupoCliente.GRP_DESCRICAO as GrupoCliente, ");
                        groupBy.Append("GrupoCliente.GRP_DESCRICAO, ");
                        
                        SetarJoinsGrupoCliente(joins);
                    }
                    break;

                case "TipoOperacao":
                    if (!select.Contains(" TipoOperacao,"))
                    {
                        select.Append(@"substring((select distinct ', ' + TipoOperacao.TOP_DESCRICAO from T_CONFIGURACAO_DESCARGA_TIPO_OPERACAO DescargaClienteTipoOperacao
                                        left join T_TIPO_OPERACAO TipoOperacao on DescargaClienteTipoOperacao.TOP_CODIGO = TipoOperacao.TOP_CODIGO  
                                        where DescargaCliente.CDC_CODIGO = DescargaClienteTipoOperacao.CDC_CODIGO
                                        for xml path('')), 3, 200) TipoOperacao, ");

                        if (!groupBy.Contains("DescargaCliente.CDC_CODIGO"))
                            groupBy.Append("DescargaCliente.CDC_CODIGO, ");
                    }
                    break;

                case "Valor":
                    if (!select.Contains(" Valor,"))
                    {
                        select.Append("DescargaCliente.CDC_VALOR as Valor, ");
                        groupBy.Append("DescargaCliente.CDC_VALOR, ");
                    }
                    break;

                case "ValorTonelada":
                    if (!select.Contains(" ValorTonelada,"))
                    {
                        select.Append("DescargaCliente.CDC_VALOR_TONELADA as ValorTonelada, ");
                        groupBy.Append("DescargaCliente.CDC_VALOR_TONELADA, ");
                    }
                    break;

                case "ValorUnidade":
                    if (!select.Contains(" ValorUnidade,"))
                    {
                        select.Append("DescargaCliente.CDC_VALOR_UNIDADE as ValorUnidade, ");
                        groupBy.Append("DescargaCliente.CDC_VALOR_UNIDADE, ");
                    }
                    break;

                case "ValorPallet":
                    if (!select.Contains(" ValorPallet,"))
                    {
                        select.Append("DescargaCliente.CDC_VALOR_PALLET as ValorPallet, ");
                        groupBy.Append("DescargaCliente.CDC_VALOR_PALLET, ");
                    }
                    break;

                case "SituacaoDescricao":
                    if (!select.Contains(" Situacao,"))
                    {
                        select.Append("DescargaCliente.CFR_SITUACAO as Situacao, ");
                        groupBy.Append("DescargaCliente.CFR_SITUACAO, ");
                    }
                    break;
                case "ClienteFormatado":
                    if (!select.Contains(" Cliente,"))
                    {
                        select.Append("Cliente.CLI_FISJUR as TipoPessoa, ");
                        groupBy.Append("Cliente.CLI_FISJUR, ");

                        select.Append("Cliente.CLI_NOME as Cliente, ");
                        groupBy.Append("Cliente.CLI_NOME, ");

                        select.Append("Cliente.CLI_CODIGO_INTEGRACAO as ClienteCodigoIntegracao, ");
                        groupBy.Append("Cliente.CLI_CODIGO_INTEGRACAO, ");

                        select.Append("Cliente.CLI_NOMEFANTASIA as NomeFantasia, ");
                        groupBy.Append("Cliente.CLI_NOMEFANTASIA, ");

                        select.Append("Cliente.CLI_CGCCPF as CpfCnpj, ");
                        groupBy.Append("Cliente.CLI_CGCCPF, ");

                        select.Append("Cliente.CLI_PONTO_TRANSBORDO as PontoTransbordo, ");
                        groupBy.Append("Cliente.CLI_PONTO_TRANSBORDO, ");

                        SetarJoinsCliente(joins);
                    }
                    break;
                case "ValorAjudante":
                    if (!select.Contains(" ValorAjudante,"))
                    {
                        select.Append("DescargaCliente.CDC_VALOR_AJUDANTE as ValorAjudante, ");
                        groupBy.Append("DescargaCliente.CDC_VALOR_AJUDANTE, ");
                    }
                    break;
                case "Cidade":
                    if(!select.Contains(" Cidade "))
                    {
                        select.Append("Localidades.LOC_DESCRICAO as Cidade, ");
                        groupBy.Append("Localidades.LOC_DESCRICAO, ");
                        SetarJoinsLocalidade(joins);
                    }
                    break;
                case "UFCidade":
                    if (!select.Contains(" UFCidade "))
                    {
                        select.Append("Localidades.UF_SIGLA as UFCidade, ");
                        groupBy.Append("Localidades.UF_SIGLA, ");
                        SetarJoinsLocalidade(joins);
                    }
                    break;
                case "ClienteCodigoIntegracao":
                    if(!select.Contains(" ClienteCodigoIntegracao"))
                    {
                        select.Append("Cliente.CLI_CODIGO_INTEGRACAO as ClienteCodigoIntegracao, ");
                        groupBy.Append("Cliente.CLI_CODIGO_INTEGRACAO, ");
                        SetarJoinsCliente(joins);

                    }
                    break;
                case "DataInativacaoFormatada":
                case "DataInativacao":
                    if (!select.Contains(" DataInativacao,"))
                    {
                        select.Append("DescargaCliente.CDC_DATA_INATIVACAO as DataInativacao, ");
                        groupBy.Append("DescargaCliente.CDC_DATA_INATIVACAO, ");
                    }
                    break;
                case "DataAprovacaoFormatada":
                case "DataAprovacao":
                    if (!select.Contains(" DataAprovacao,"))
                    {
                        select.Append("DescargaCliente.CDC_DATA_APROVACAO as DataAprovacao, ");
                        groupBy.Append("DescargaCliente.CDC_DATA_APROVACAO, ");
                    }
                    break;
                case "DataAlteracaoFormatada":
                case "DataAlteracao":
                    if (!select.Contains(" DataAlteracao,"))
                    {
                        select.Append("DescargaCliente.CDC_DATA_ULTIMA_ALTERACAO as DataAlteracao, ");
                        groupBy.Append("DescargaCliente.CDC_DATA_ULTIMA_ALTERACAO, ");
                    }
                    break;
                case "DataCriacaoFormatada":
                case "DataCriacao":
                    if (!select.Contains(" DataCriacao,"))
                    {
                        select.Append("DescargaCliente.CDC_DATA_CRIACAO as DataCriacao, ");
                        groupBy.Append("DescargaCliente.CDC_DATA_CRIACAO, ");
                    }
                    break;
                case "Usuario":
                    if (!select.Contains(" Usuario,"))
                    {
                        select.Append("Funcionario.FUN_NOME as Usuario, ");
                        groupBy.Append("Funcionario.FUN_NOME, ");
                        SetarJoinsUsuario(joins);
                    }
                    break;
            }
        }

        protected override void SetarWhere(Dominio.ObjetosDeValor.Embarcador.Pallets.FiltroPesquisaTaxasDescarga filtrosPesquisa, StringBuilder where, StringBuilder joins, StringBuilder groupBy, List<Embarcador.Consulta.ParametroSQL> parametros = null)
        {
            string pattern = "yyyy/MM/dd";

            if (filtrosPesquisa.CodigoFilial > 0)
            {
                where.Append($" and Filial.FIL_CODIGO = '{filtrosPesquisa.CodigoFilial}'");

                SetarJoinsFilial(joins);
            }

            if (filtrosPesquisa.CodigoModeloVeicular > 0)
            {
                where.Append($" and ModeloVeicular.MVC_CODIGO = '{filtrosPesquisa.CodigoModeloVeicular}'");

                SetarJoinsModeloVeicular(joins);
            }

            if (filtrosPesquisa.CodigoTipoCarga > 0)
            {
                where.Append($" and TipoCarga.TCG_CODIGO = '{filtrosPesquisa.CodigoTipoCarga}'");

                SetarJoinsTipoCarga(joins);
            }

            if (filtrosPesquisa.Status.HasValue)
            {
                where.Append($" and DescargaCliente.CDC_ATIVO = '{filtrosPesquisa.Status.Value}'");
            }

            if (filtrosPesquisa.CodigoSituacao.HasValue)
            {
                where.Append($" and DescargaCliente.CFR_SITUACAO = '{filtrosPesquisa.CodigoSituacao.Value.GetHashCode()}'");
            }

            if (filtrosPesquisa.DataInicioVigencia != DateTime.MinValue)
                where.Append($" and DescargaCliente.CDC_INICIO_VIGENCIA >= '{filtrosPesquisa.DataInicioVigencia.ToString(pattern)}'");

            if (filtrosPesquisa.DataFimVigencia != DateTime.MinValue)
                where.Append($" and DescargaCliente.CDC_FIM_VIGENCIA <= '{filtrosPesquisa.DataFimVigencia.ToString(pattern)}'");

            if (filtrosPesquisa.CpfCnpjCliente > 0d)
            {
                where.Append($" and Cliente.CLI_CGCCPF = '{filtrosPesquisa.CpfCnpjCliente}'");

                SetarJoinsCliente(joins);
            }

            if (filtrosPesquisa.CodigoGrupoCliente > 0)
            {
                where.Append($" and GrupoCliente.GRP_CODIGO = '{filtrosPesquisa.CodigoGrupoCliente}'");

                SetarJoinsGrupoCliente(joins);
            }
            
            if (filtrosPesquisa.CodigoTipoOperacao != null && filtrosPesquisa.CodigoTipoOperacao.Count > 0)
            {
                where.Append($" and TipoOperacao.TOP_CODIGO in ({string.Join(", ", filtrosPesquisa.CodigoTipoOperacao)})");

                SetarJoinsTipoOperacao(joins);
            }

            if (filtrosPesquisa.SomenteVigentes)
            {
                where.Append(@" and (DescargaCliente.CDC_INICIO_VIGENCIA is not null and DescargaCliente.CDC_FIM_VIGENCIA is not null) and (DescargaCliente.CDC_INICIO_VIGENCIA <= SYSDATETIME() and DescargaCliente.CDC_FIM_VIGENCIA >= SYSDATETIME())
                                or (DescargaCliente.CDC_INICIO_VIGENCIA is not null and DescargaCliente.CDC_FIM_VIGENCIA = null) and DescargaCliente.CDC_INICIO_VIGENCIA <= SYSDATETIME()
                                or (DescargaCliente.CDC_INICIO_VIGENCIA = null and DescargaCliente.CDC_FIM_VIGENCIA is not null) and DescargaCliente.CDC_INICIO_VIGENCIA <= SYSDATETIME() ");
            }

            if (filtrosPesquisa.CodigoTransportadorPortal > 0)
            {
                where.Append($@" and exists (select top 1 a.EMP_CODIGO 
                                            from T_CONFIGURACAO_DESCARGA_TRANSPORTADOR a 
                                                join T_CONFIGURACAO_DESCARGA_CLIENTE b on a.CDC_CODIGO = b.CDC_CODIGO
                                            where b.CDC_CODIGO = DescargaCliente.CDC_CODIGO and a.EMP_CODIGO = {filtrosPesquisa.CodigoTransportadorPortal})");
            }
        }

    }

}
