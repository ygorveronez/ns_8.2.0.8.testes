using Dominio.ObjetosDeValor.Embarcador.Carga.ControleEntrega;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Repositorio.Embarcador.Cargas.ControleEntrega
{
    sealed class ConsultaControleEntrega : Consulta.Consulta<Dominio.ObjetosDeValor.Embarcador.Carga.ControleEntrega.FiltroPesquisaRelatorioControleEntrega>
    {
        #region Construtores

        public ConsultaControleEntrega() : base(tabela: "T_CTE as CTe") { }

        #endregion

        #region Métodos Privados

        private void SetarJoinsCargaCTe(StringBuilder joins)
        {
            if (!joins.Contains(" CargaCTe "))
                joins.Append(" INNER JOIN T_CARGA_CTE CargaCTe ON CargaCTe.CON_CODIGO = CTe.CON_CODIGO ");
        }

        private void SetarJoinsCarga(StringBuilder joins)
        {
            SetarJoinsCargaCTe(joins);

            if (!joins.Contains(" Carga "))
                joins.Append(" INNER JOIN T_CARGA Carga ON Carga.CAR_CODIGO = CargaCTe.CAR_CODIGO ");
        }

        private void SetarJoinsRemetente(StringBuilder joins)
        {
            if (!joins.Contains(" Remetente "))
                joins.Append(" LEFT JOIN T_CTE_PARTICIPANTE Remetente ON Remetente.PCT_CODIGO = CTe.CON_REMETENTE_CTE ");
        }

        private void SetarJoinsLocalidadeRemetente(StringBuilder joins)
        {
            SetarJoinsRemetente(joins);

            if (!joins.Contains(" LocalidadeRemetente "))
                joins.Append(" LEFT JOIN T_LOCALIDADES LocalidadeRemetente on LocalidadeRemetente.LOC_CODIGO = Remetente.LOC_CODIGO ");
        }

        private void SetarJoinsDestinatario(StringBuilder joins)
        {
            if (!joins.Contains(" Destinatario "))
                joins.Append(" LEFT JOIN T_CTE_PARTICIPANTE Destinatario on Destinatario.PCT_CODIGO = CTe.CON_DESTINATARIO_CTE ");
        }

        private void SetarJoinsLocalidadeDestinatario(StringBuilder joins)
        {
            SetarJoinsDestinatario(joins);

            if (!joins.Contains(" LocalidadeDestinatario "))
                joins.Append(" LEFT JOIN T_LOCALIDADES LocalidadeDestinatario on LocalidadeDestinatario.LOC_CODIGO = Destinatario.LOC_CODIGO ");
        }

        private void SetarJoinsRecebedor(StringBuilder joins)
        {
            if (!joins.Contains(" Recebedor "))
                joins.Append(" LEFT OUTER JOIN T_CTE_PARTICIPANTE Recebedor on Recebedor.PCT_CODIGO = CTe.CON_RECEBEDOR_CTE ");
        }

        private void SetarJoinsCargaPedido(StringBuilder joins)
        {
            SetarJoinsCarga(joins);

            if (!joins.Contains(" CargaPedido "))
                joins.Append(" JOIN T_CARGA_PEDIDO CargaPedido on CargaPedido.CAR_CODIGO = Carga.CAR_CODIGO ");
        }

        private void SetarJoinsPedido(StringBuilder joins)
        {
            SetarJoinsCargaPedido(joins);

            if (!joins.Contains(" Pedido "))
                joins.Append(" JOIN T_PEDIDO Pedido on Pedido.PED_CODIGO = CargaPedido.PED_CODIGO ");
        }

        private void SetarJoinsGrupoPessoaCarga(StringBuilder joins)
        {
            SetarJoinsCarga(joins);

            if (!joins.Contains(" GrupoPessoaCarga "))
                joins.Append(" LEFT JOIN T_GRUPO_PESSOAS GrupoPessoaCarga on GrupoPessoaCarga.GRP_CODIGO = Carga.GRP_CODIGO ");
        }

        private void SetarJoinsOrigem(StringBuilder joins)
        {
            if (!joins.Contains(" Origem "))
                joins.Append(" JOIN T_LOCALIDADES Origem on Origem.LOC_CODIGO = CTe.CON_LOCINICIOPRESTACAO ");
        }

        private void SetarJoinsDestino(StringBuilder joins)
        {
            if (!joins.Contains(" Destino "))
                joins.Append(" JOIN T_LOCALIDADES Destino on Destino.LOC_CODIGO = CTe.CON_LOCTERMINOPRESTACAO ");
        }

        private void SetarJoinsCargaEntrega(StringBuilder joins)
        {
            SetarJoinsCarga(joins);
            SetarJoinsRecebedor(joins);
            SetarJoinsDestinatario(joins);

            if (!joins.Contains(" CargaEntrega "))
                joins.Append(" JOIN T_CARGA_ENTREGA CargaEntrega on CargaEntrega.CAR_CODIGO = Carga.CAR_CODIGO ");
        }

        private void SetarJoinsOcorrenciaEntrega(StringBuilder joins)
        {
            SetarJoinsCargaEntrega(joins);

            if (!joins.Contains(" OcorrenciaEntrega "))
                joins.Append(" INNER JOIN T_OCORRENCIA_COLETA_ENTREGA OcorrenciaEntrega on OcorrenciaEntrega.CEN_CODIGO = CargaEntrega.CEN_CODIGO ");
        }

        private void SetarJoinsTipoEntrega(StringBuilder joins)
        {
            SetarJoinsOcorrenciaEntrega(joins);

            if (!joins.Contains(" TipoEntrega "))
                joins.Append(" JOIN T_OCORRENCIA TipoEntrega on TipoEntrega.OCO_CODIGO = OcorrenciaEntrega.OCO_CODIGO ");
        }

        private void SetarJoinsCentroResultado(StringBuilder joins)
        {
            SetarJoinsOcorrenciaEntrega(joins);
            SetarJoinsPedido(joins);
            if (!joins.Contains(" CentroResultado "))
                joins.Append(" LEFT JOIN T_CENTRO_RESULTADO CentroResultado ON CentroResultado.CRE_CODIGO = Pedido.CRE_CODIGO   ");
        }

        #endregion

        #region Métodos Protegidos Sobrescritos

        protected override void SetarSelect(string propriedade, int codigoDinamico, StringBuilder select, StringBuilder joins, StringBuilder groupBy, bool somenteContarNumeroRegistros, FiltroPesquisaRelatorioControleEntrega filtrosPesquisa)
        {
            switch (propriedade)
            {
                case "NumeroCTe":
                    if (!select.Contains(" NumeroCTe, "))
                    {
                        select.Append("CTe.CON_NUM NumeroCTe, ");
                        groupBy.Append("CTe.CON_NUM, ");
                    }
                    break;

                case "NumeroCarga":
                    if (!select.Contains(" NumeroCarga, "))
                    {
                        select.Append("Carga.CAR_CODIGO_CARGA_EMBARCADOR NumeroCarga, ");
                        groupBy.Append("Carga.CAR_CODIGO_CARGA_EMBARCADOR, ");

                        SetarJoinsCarga(joins);
                    }
                    break;

                case "DataEmissaoCTeFormatada":
                    if (!select.Contains(" DataEmissaoCTe, "))
                    {
                        select.Append("CTe.CON_DATAHORAEMISSAO DataEmissaoCTe, ");
                        groupBy.Append("CTe.CON_DATAHORAEMISSAO, ");
                    }
                    break;

                case "Remetente":
                    SetarSelect("NomeRemetente", codigoDinamico, select, joins, groupBy, somenteContarNumeroRegistros, filtrosPesquisa);
                    SetarSelect("CNPJRemetente", codigoDinamico, select, joins, groupBy, somenteContarNumeroRegistros, filtrosPesquisa);
                    break;

                case "NomeRemetente":
                    if (!select.Contains(" NomeRemetente, "))
                    {
                        select.Append("Remetente.PCT_NOME NomeRemetente, ");
                        groupBy.Append("Remetente.PCT_NOME, ");

                        SetarJoinsRemetente(joins);
                    }
                    break;

                case "CNPJRemetente":
                    if (!select.Contains(" CNPJRemetente, "))
                    {
                        select.Append("Remetente.PCT_CPF_CNPJ CNPJRemetente, ");
                        groupBy.Append("Remetente.PCT_CPF_CNPJ, ");

                        SetarJoinsRemetente(joins);
                    }
                    break;

                case "LocalidadeRemetente":
                    SetarSelect("CidadeRemetente", codigoDinamico, select, joins, groupBy, somenteContarNumeroRegistros, filtrosPesquisa);
                    SetarSelect("UFRemetente", codigoDinamico, select, joins, groupBy, somenteContarNumeroRegistros, filtrosPesquisa);
                    break;

                case "CidadeRemetente":
                    if (!select.Contains(" CidadeRemetente, "))
                    {
                        select.Append("LocalidadeRemetente.LOC_DESCRICAO CidadeRemetente, ");
                        groupBy.Append("LocalidadeRemetente.LOC_DESCRICAO, ");

                        SetarJoinsLocalidadeRemetente(joins);
                    }
                    break;

                case "UFRemetente":
                    if (!select.Contains(" UFRemetente, "))
                    {
                        select.Append("LocalidadeRemetente.UF_SIGLA UFRemetente, ");
                        groupBy.Append("LocalidadeRemetente.UF_SIGLA, ");

                        SetarJoinsLocalidadeRemetente(joins);
                    }
                    break;

                case "Origem":
                    SetarSelect("CidadeOrigem", codigoDinamico, select, joins, groupBy, somenteContarNumeroRegistros, filtrosPesquisa);
                    SetarSelect("UFOrigem", codigoDinamico, select, joins, groupBy, somenteContarNumeroRegistros, filtrosPesquisa);
                    break;

                case "CidadeOrigem":
                    if (!select.Contains(" CidadeOrigem, "))
                    {
                        select.Append("Origem.LOC_DESCRICAO CidadeOrigem, ");
                        groupBy.Append("Origem.LOC_DESCRICAO, ");

                        SetarJoinsOrigem(joins);
                    }
                    break;

                case "UFOrigem":
                    if (!select.Contains(" UFOrigem, "))
                    {
                        select.Append("Origem.UF_SIGLA UFOrigem, ");
                        groupBy.Append("Origem.UF_SIGLA, ");

                        SetarJoinsOrigem(joins);
                    }
                    break;

                case "Destinatario":
                    SetarSelect("NomeDestinatario", codigoDinamico, select, joins, groupBy, somenteContarNumeroRegistros, filtrosPesquisa);
                    SetarSelect("CNPJDestinatario", codigoDinamico, select, joins, groupBy, somenteContarNumeroRegistros, filtrosPesquisa);
                    break;

                case "NomeDestinatario":
                    if (!select.Contains(" NomeDestinatario, "))
                    {
                        select.Append("Destinatario.PCT_NOME NomeDestinatario, ");
                        groupBy.Append("Destinatario.PCT_NOME, ");

                        SetarJoinsDestinatario(joins);
                    }
                    break;

                case "CNPJDestinatario":
                    if (!select.Contains(" CNPJDestinatario, "))
                    {
                        select.Append("Destinatario.PCT_CPF_CNPJ CNPJDestinatario, ");
                        groupBy.Append("Destinatario.PCT_CPF_CNPJ, ");

                        SetarJoinsDestinatario(joins);
                    }
                    break;

                case "LocalidadeDestinatario":
                    SetarSelect("CidadeDestinatario", codigoDinamico, select, joins, groupBy, somenteContarNumeroRegistros, filtrosPesquisa);
                    SetarSelect("UFDestinatario", codigoDinamico, select, joins, groupBy, somenteContarNumeroRegistros, filtrosPesquisa);
                    break;

                case "CidadeDestinatario":
                    if (!select.Contains(" CidadeDestinatario, "))
                    {
                        select.Append("LocalidadeDestinatario.LOC_DESCRICAO CidadeDestinatario, ");
                        groupBy.Append("LocalidadeDestinatario.LOC_DESCRICAO, ");

                        SetarJoinsLocalidadeDestinatario(joins);
                    }
                    break;

                case "UFDestinatario":
                    if (!select.Contains(" UFDestinatario, "))
                    {
                        select.Append("LocalidadeDestinatario.UF_SIGLA UFDestinatario, ");
                        groupBy.Append("LocalidadeDestinatario.UF_SIGLA, ");

                        SetarJoinsLocalidadeDestinatario(joins);
                    }
                    break;

                case "Destino":
                    SetarSelect("CidadeDestino", codigoDinamico, select, joins, groupBy, somenteContarNumeroRegistros, filtrosPesquisa);
                    SetarSelect("UFDestino", codigoDinamico, select, joins, groupBy, somenteContarNumeroRegistros, filtrosPesquisa);
                    break;

                case "CidadeDestino":
                    if (!select.Contains(" CidadeDestino, "))
                    {
                        select.Append("Destino.LOC_DESCRICAO CidadeDestino, ");
                        groupBy.Append("Destino.LOC_DESCRICAO, ");

                        SetarJoinsDestino(joins);
                    }
                    break;

                case "UFDestino":
                    if (!select.Contains(" UFDestino, "))
                    {
                        select.Append("Destino.UF_SIGLA UFDestino, ");
                        groupBy.Append("Destino.UF_SIGLA, ");

                        SetarJoinsDestino(joins);
                    }
                    break;

                case "Notas":
                    if (!select.Contains(" Notas, "))
                    {
                        select.Append("SUBSTRING(( " +
                            "	SELECT DISTINCT ', ' + Docs.NFC_NUMERO " +
                            "	FROM T_CTE_DOCS Docs " +
                            "	WHERE Docs.CON_CODIGO = CTe.CON_CODIGO " +
                            "	FOR XML PATH('')" +
                            "), 3, 1000) Notas, ");

                        if (!groupBy.Contains("CTe.CON_CODIGO"))
                            groupBy.Append("CTe.CON_CODIGO, ");
                    }
                    break;

                case "Veiculos":
                    if (!select.Contains(" Veiculos, "))
                    {
                        select.Append("(( " +
                            "	SELECT Vei.VEI_PLACA " +
                            "	FROM T_VEICULO Vei " +
                            "	WHERE Vei.VEI_CODIGO = Carga.CAR_VEICULO " +
                            ") + ISNULL(( " +
                            "	SELECT ', ' + Veiculo.VEI_PLACA " +
                            "	FROM T_CARGA_VEICULOS_VINCULADOS VeiculoVinculadoCarga " +
                            "		INNER JOIN T_VEICULO Veiculo ON Veiculo.VEI_CODIGO = VeiculoVinculadoCarga.VEI_CODIGO " +
                            "	WHERE VeiculoVinculadoCarga.CAR_CODIGO = Carga.CAR_CODIGO " +
                            "	FOR XML PATH('') " +
                            "), '')) Veiculos, ");

                        if (!groupBy.Contains("Carga.CAR_CODIGO,"))
                            groupBy.Append("Carga.CAR_CODIGO, ");

                        if (!groupBy.Contains("Carga.CAR_VEICULO"))
                            groupBy.Append("Carga.CAR_VEICULO, ");

                        SetarJoinsCarga(joins);
                    }
                    break;

                case "Motoristas":
                    if (!select.Contains(" Motoristas, "))
                    {
                        select.Append("SUBSTRING(( " +
                            "	SELECT ', ' + Motorista.FUN_NOME + ( " +
                            "		CASE" +
                            "			WHEN Motorista.FUN_FONE is null or Motorista.FUN_FONE = '' THEN '' " +
                            "			ELSE ' (' + Motorista.FUN_FONE  + ')' " +
                            "		END) " +
                            "	FROM T_CARGA_MOTORISTA MotoristaCarga " +
                            "		INNER JOIN T_FUNCIONARIO Motorista ON Motorista.FUN_CODIGO = MotoristaCarga.CAR_MOTORISTA " +
                            "	WHERE MotoristaCarga.CAR_CODIGO = Carga.CAR_CODIGO " +
                            "	FOR XML PATH('') " +
                            "), 3, 1000) Motoristas, ");

                        if (!groupBy.Contains("Carga.CAR_CODIGO,"))
                            groupBy.Append("Carga.CAR_CODIGO, ");

                        SetarJoinsCarga(joins);
                    }
                    break;

                case "NumeroPallets":
                    if (!select.Contains(" NumeroPallets, "))
                    {
                        select.Append("Pedido.PED_NUMERO_PALETES NumeroPallets, ");
                        groupBy.Append("Pedido.PED_NUMERO_PALETES, ");

                        SetarJoinsPedido(joins);
                    }
                    break;

                case "DataAgendamentoFormatada":
                    if (!select.Contains(" DataAgendamento, "))
                    {
                        select.Append("Pedido.PED_PREVISAO_ENTREGA DataAgendamento, ");
                        groupBy.Append("Pedido.PED_PREVISAO_ENTREGA, ");

                        SetarJoinsPedido(joins);
                    }
                    break;

                case "GrupoPessoaCarga":
                    if (!select.Contains(" GrupoPessoaCarga, "))
                    {
                        select.Append("GrupoPessoaCarga.GRP_DESCRICAO GrupoPessoaCarga, ");
                        groupBy.Append("GrupoPessoaCarga.GRP_DESCRICAO, ");

                        SetarJoinsGrupoPessoaCarga(joins);
                    }
                    break;

                case "Observacao":
                    if (!select.Contains(" Observacao, "))
                    {
                        select.Append("CargaEntrega.CEN_OBSERVACAO Observacao, ");
                        groupBy.Append("CargaEntrega.CEN_OBSERVACAO, ");

                        SetarJoinsCargaEntrega(joins);
                    }
                    break;

                case "DataOcorrenciaFormatada":
                    if (!select.Contains(" DataOcorrencia, "))
                    {
                        select.Append("OcorrenciaEntrega.OCE_DATA_OCORRENCIA DataOcorrencia, ");
                        groupBy.Append("OcorrenciaEntrega.OCE_DATA_OCORRENCIA, ");

                        SetarJoinsOcorrenciaEntrega(joins);
                    }
                    break;

                case "DescricaoOcorrencia":
                    if (!select.Contains(" DescricaoOcorrencia, "))
                    {
                        select.Append("TipoEntrega.OCO_DESCRICAO DescricaoOcorrencia, ");
                        groupBy.Append("TipoEntrega.OCO_DESCRICAO, ");

                        SetarJoinsTipoEntrega(joins);
                    }
                    break;
                case "ObservacaoOcorrencia":
                    if (!select.Contains(" ObservacaoOcorrencia, "))
                    {
                        select.Append("OcorrenciaEntrega.OCE_OBSERVACAO_OCORRENCIA ObservacaoOcorrencia, ");
                        groupBy.Append("OcorrenciaEntrega.OCE_OBSERVACAO_OCORRENCIA, ");

                        SetarJoinsOcorrenciaEntrega(joins);
                    }
                    break;
                case "NumeroPedidoEmbarcador":
                    if (!select.Contains(" NumeroPedidoEmbarcador, "))
                    {
                        select.Append("Pedido.PED_NUMERO_PEDIDO_EMBARCADOR NumeroPedidoEmbarcador, ");
                        groupBy.Append("Pedido.PED_NUMERO_PEDIDO_EMBARCADOR, ");

                        SetarJoinsPedido(joins);
                    }
                    break;
                case "DataPrevisaoColeta":
                    if (!select.Contains(" DataPrevisaoColeta, "))
                    {
                        select.Append("Pedido.PED_DATA_INICIAL_COLETA DataPrevisaoColeta, ");
                        groupBy.Append("Pedido.PED_DATA_INICIAL_COLETA, ");

                        SetarJoinsPedido(joins);
                    }
                    break;
                case "DataPrevisaoEntrega":
                    if (!select.Contains(" DataPrevisaoEntrega, "))
                    {
                        select.Append("Pedido.PED_PREVISAO_ENTREGA DataPrevisaoEntrega, ");
                        groupBy.Append("Pedido.PED_PREVISAO_ENTREGA, ");

                        SetarJoinsPedido(joins);
                    }
                    break;
                case "SenhaAgendamento":
                    if (!select.Contains(" SenhaAgendamento, "))
                    {
                        select.Append("Pedido.PED_SENHA_AGENDAMENTO SenhaAgendamento, ");
                        groupBy.Append("Pedido.PED_SENHA_AGENDAMENTO, ");

                        SetarJoinsPedido(joins);
                    }
                    break;
                case "DescricaoCentroResultado":
                    if (!select.Contains(" DescricaoCentroResultado, "))
                    {
                        select.Append("CentroResultado.CRE_DESCRICAO DescricaoCentroResultado, ");
                        groupBy.Append("CentroResultado.CRE_DESCRICAO, ");

                        SetarJoinsCentroResultado(joins);
                    }
                    break;
            }
        }

        protected override void SetarWhere(FiltroPesquisaRelatorioControleEntrega filtrosPesquisa, StringBuilder where, StringBuilder joins, StringBuilder groupBy, List<Embarcador.Consulta.ParametroSQL> parametros = null)
        {
            if (filtrosPesquisa.DataOcorrenciaInicial != DateTime.MinValue)
            {
                where.Append($" AND OcorrenciaEntrega.OCE_DATA_OCORRENCIA >= '{filtrosPesquisa.DataOcorrenciaInicial.ToString("yyyy-MM-dd")}' ");
                SetarJoinsOcorrenciaEntrega(joins);
            }

            if (filtrosPesquisa.DataOcorrenciaFinal != DateTime.MinValue)
            {
                where.Append($" AND OcorrenciaEntrega.OCE_DATA_OCORRENCIA < '{filtrosPesquisa.DataOcorrenciaFinal.AddDays(1).ToString("yyyy-MM-dd")}' ");
                SetarJoinsOcorrenciaEntrega(joins);
            }

            if (filtrosPesquisa.CodigosTipoOcorrencia.Count > 0)
            {
                where.Append($" AND TipoEntrega.OCO_CODIGO IN ({string.Join(", ", filtrosPesquisa.CodigosTipoOcorrencia)}) ");
                SetarJoinsTipoEntrega(joins);
            }

            if (filtrosPesquisa.CodigoGrupoPessoa > 0)
            {
                where.Append($" AND Carga.GRP_CODIGO = {filtrosPesquisa.CodigoGrupoPessoa} ");
                SetarJoinsCarga(joins);
            }

            if (filtrosPesquisa.CodigosTipoOperacao.Count > 0)
            {
                where.Append($" AND Carga.TOP_CODIGO IN({string.Join(", ", filtrosPesquisa.CodigosTipoOperacao)}) ");
                SetarJoinsCarga(joins);
            }

            if (!string.IsNullOrWhiteSpace(filtrosPesquisa.NumeroCarga))
            {
                where.Append($" AND CAR_CODIGO_CARGA_EMBARCADOR = '{filtrosPesquisa.NumeroCarga}' ");
                SetarJoinsCarga(joins);
            }

            if (!string.IsNullOrWhiteSpace(filtrosPesquisa.NumeroNotaFiscal))
            {
                where.Append($" AND CTe.CON_CODIGO IN (SELECT CTeDocs.CON_CODIGO FROM T_CTE_DOCS CTeDocs WHERE CTeDocs.CON_CODIGO = CTe.CON_CODIGO AND CTeDocs.NFC_NUMERO = :CTEDOCS_NFC_NUMERO) ");
                parametros.Add(new Embarcador.Consulta.ParametroSQL("CTEDOCS_NFC_NUMERO", filtrosPesquisa.NumeroNotaFiscal));
            }
            

            if (filtrosPesquisa.NumeroCTe > 0)
                where.Append($" AND CTe.CON_NUM = {filtrosPesquisa.NumeroCTe} ");

            if (filtrosPesquisa.CodigosVeiculos.Count > 0)
            {
                where.Append($" AND Carga.CAR_VEICULO IN ({string.Join(", ", filtrosPesquisa.CodigosVeiculos)}) ");
                SetarJoinsCarga(joins);
            }

            if (filtrosPesquisa.CodigosMotoristas.Count > 0)
            {
                where.Append($" AND Carga.CAR_CODIGO IN (SELECT CarMotorista.CAR_CODIGO FROM T_CARGA_MOTORISTA CarMotorista WHERE CarMotorista.CAR_CODIGO = Carga.CAR_CODIGO and CarMotorista.CAR_MOTORISTA IN({string.Join(", ", filtrosPesquisa.CodigosMotoristas)})) "); // SQL-INJECTION-SAFE
                SetarJoinsCarga(joins);
            }

            if (filtrosPesquisa.UFsDestino.Count > 0)
            {
                where.Append($" AND Destino.UF_SIGLA IN('{string.Join("', '", filtrosPesquisa.UFsDestino)}') ");
                SetarJoinsDestino(joins);
            }

            if (filtrosPesquisa.UFsOrigem.Count > 0)
            {
                where.Append($" AND Origem.UF_SIGLA IN('{string.Join("', '", filtrosPesquisa.UFsOrigem)}') ");
                SetarJoinsOrigem(joins);
            }

            if (filtrosPesquisa.DataPrevisaoEntregaInicial != DateTime.MinValue)
            {
                where.Append($" AND Pedido.PED_PREVISAO_ENTREGA >= '{filtrosPesquisa.DataPrevisaoEntregaInicial.ToString("yyyy-MM-dd")}' ");
                SetarJoinsPedido(joins);
            }

            if (filtrosPesquisa.DataPrevisaoEntregaFinal != DateTime.MinValue)
            {
                where.Append($" AND Pedido.PED_PREVISAO_ENTREGA < '{filtrosPesquisa.DataPrevisaoEntregaFinal.AddDays(1).ToString("yyyy-MM-dd")}' ");
                SetarJoinsPedido(joins);
            }
        }

        #endregion
    }
}
