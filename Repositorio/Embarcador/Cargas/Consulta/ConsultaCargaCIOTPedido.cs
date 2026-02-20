using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Repositorio.Embarcador.Cargas
{
    sealed class ConsultaCargaCIOTPedido : Consulta.Consulta<Dominio.ObjetosDeValor.Embarcador.Carga.FiltroPesquisaRelatorioCargaCIOTPedido>
    {
        #region Construtores

        public ConsultaCargaCIOTPedido() : base(tabela: "T_CARGA_CIOT as CargaCIOT") { }

        #endregion

        #region Métodos Privados

        private void SetarJoinsCargaPedido(StringBuilder joins)
        {
            if (!joins.Contains(" CargaPedido "))
                joins.Append(" JOIN T_CARGA_PEDIDO CargaPedido ON CargaPedido.CAR_CODIGO = CargaCIOT.CAR_CODIGO ");
        }

        private void SetarJoinsPedido(StringBuilder joins)
        {
            SetarJoinsCargaPedido(joins);

            if (!joins.Contains(" Pedido "))
                joins.Append(" JOIN T_PEDIDO Pedido ON Pedido.PED_CODIGO = CargaPedido.PED_CODIGO ");
        }

        private void SetarJoinsCarga(StringBuilder joins)
        {
            if (!joins.Contains(" Carga "))
                joins.Append(" JOIN T_CARGA Carga ON Carga.CAR_CODIGO = CargaCIOT.CAR_CODIGO ");
        }

        private void SetarJoinsCiot(StringBuilder joins)
        {
            if (!joins.Contains(" CIOT "))
                joins.Append(" LEFT JOIN T_CIOT CIOT ON CIOT.CIO_CODIGO = CargaCIOT.CIO_CODIGO ");
        }

        private void SetarJoinsContratoFrete(StringBuilder joins)
        {
            if (!joins.Contains(" ContratoFrete "))
                joins.Append(" left join T_CONTRATO_FRETE_TERCEIRO ContratoFrete ON ContratoFrete.CFT_CODIGO = CargaCIOT.CFT_CODIGO ");
        }

        private void SetarJoinsEmpresa(StringBuilder joins)
        {
            SetarJoinsCiot(joins);

            if (!joins.Contains(" Empresa "))
                joins.Append(" LEFT JOIN T_EMPRESA Empresa ON Empresa.EMP_CODIGO = CIOT.EMP_CODIGO ");
        }

        private void SetarJoinsImpostoContratoFrete(StringBuilder joins)
        {
            SetarJoinsCarga(joins);

            if (!joins.Contains(" ImpostoContratoFrete "))
                joins.Append(" left join T_IMPOSTO_CONTRATO_FRETE ImpostoContratoFrete on ImpostoContratoFrete.EMP_CODIGO = Carga.EMP_CODIGO ");
        }

        private void SetarJoinsMotorista(StringBuilder joins)
        {
            if (!joins.Contains(" Motorista "))
                joins.Append(" JOIN T_FUNCIONARIO Motorista ON Motorista.FUN_CODIGO = CargaCIOT.FUN_CODIGO_MOTORISTA ");
        }

        private void SetarJoinsProprietario(StringBuilder joins)
        {
            SetarJoinsCiot(joins);

            if (!joins.Contains(" Proprietario "))
                joins.Append(" JOIN T_CLIENTE Proprietario ON Proprietario.CLI_CGCCPF = CIOT.CLI_CGCCPF ");
        }

        private void SetarJoinsVeiculo(StringBuilder joins)
        {
            SetarJoinsCarga(joins);

            if (!joins.Contains(" Veiculo "))
                joins.Append(" JOIN T_VEICULO Veiculo ON Veiculo.VEI_CODIGO = Carga.CAR_VEICULO ");
        }

        private void SetarJoinsVeiculoCIOT(StringBuilder joins)
        {
            SetarJoinsCiot(joins);

            if (!joins.Contains(" VeiculoCIOT "))
                joins.Append(" LEFT JOIN T_VEICULO VeiculoCIOT ON VeiculoCIOT.VEI_CODIGO = CIOT.VEI_CODIGO ");
        }

        private void SetarJoinsDestinatarioPedido(StringBuilder joins)
        {
            SetarJoinsPedido(joins);

            if (!joins.Contains(" DestinatarioPedido "))
                joins.Append(" LEFT JOIN T_CLIENTE DestinatarioPedido ON DestinatarioPedido.CLI_CGCCPF = Pedido.CLI_CODIGO ");
        }

        private void SetarJoinsDestinoPedido(StringBuilder joins)
        {
            SetarJoinsPedido(joins);

            if (!joins.Contains(" DestinoPedido "))
                joins.Append(" LEFT JOIN T_LOCALIDADES DestinoPedido ON DestinoPedido.LOC_CODIGO = Pedido.LOC_CODIGO_DESTINO ");
        }

        #endregion

        #region Métodos Protegidos Sobrescritos

        protected override void SetarSelect(string propriedade, int codigoDinamico, StringBuilder select, StringBuilder joins, StringBuilder groupBy, bool somenteContarNumeroRegistros, Dominio.ObjetosDeValor.Embarcador.Carga.FiltroPesquisaRelatorioCargaCIOTPedido filtroPesquisa)
        {
            switch (propriedade)
            {
                case "Carga":
                    if (!select.Contains(" Carga,"))
                    {
                        select.Append("Carga.CAR_CODIGO_CARGA_EMBARCADOR Carga, ");
                        groupBy.Append("Carga.CAR_CODIGO_CARGA_EMBARCADOR, ");

                        SetarJoinsCarga(joins);
                    }
                    break;

                case "Pedido":
                case "PedidoFormatado":
                    if (!select.Contains(" Pedido, "))
                    {
                        select.Append("Pedido.PED_NUMERO_PEDIDO_EMBARCADOR Pedido, ");

                        if (!groupBy.Contains("Pedido.PED_NUMERO_PEDIDO_EMBARCADOR,"))
                            groupBy.Append("Pedido.PED_NUMERO_PEDIDO_EMBARCADOR, ");

                        SetarJoinsPedido(joins);
                    }
                    break;

                case "DataPagamentoAdiantamentoFrete":
                case "DataPagamentoAdiantamentoFreteFormatada":
                    if (!select.Contains(" DataPagamentoAdiantamentoFrete,"))
                    {
                        select.Append("CIOT.CIO_DATA_ABERTURA DataPagamentoAdiantamentoFrete, ");
                        groupBy.Append("CIOT.CIO_DATA_ABERTURA, ");

                        SetarJoinsCiot(joins);
                    }
                    break;

                case "DataPagamentoSaldoFrete":
                case "DataPagamentoSaldoFreteFormatada":
                    if (!select.Contains(" DataPagamentoSaldoFrete,"))
                    {
                        select.Append("CIOT.CIO_DATA_ENCERRAMENTO DataPagamentoSaldoFrete, ");
                        groupBy.Append("CIOT.CIO_DATA_ENCERRAMENTO, ");

                        SetarJoinsCiot(joins);
                    }
                    break;

                case "EmpresaFormatado":
                    if (!select.Contains(" Empresa,"))
                    {
                        select.Append("Empresa.EMP_RAZAO Empresa, ");
                        groupBy.Append("Empresa.EMP_RAZAO, ");
                    }

                    if (!select.Contains(" CNPJEmpresa,"))
                    {
                        select.Append("Empresa.EMP_CNPJ CNPJEmpresa, ");
                        groupBy.Append("Empresa.EMP_CNPJ, ");
                    }

                    SetarJoinsEmpresa(joins);
                    break;

                case "Proprietario":
                case "ProprietarioFormatado":
                    if (!select.Contains(" Proprietario,"))
                    {
                        select.Append("Proprietario.CLI_NOME Proprietario, ");
                        groupBy.Append("Proprietario.CLI_NOME, ");
                    }

                    if (!select.Contains(" CNPJProprietario,"))
                    {
                        select.Append("Proprietario.CLI_CGCCPF CNPJProprietario, ");
                        groupBy.Append("Proprietario.CLI_CGCCPF, ");
                    }

                    if (!select.Contains(" TipoPessoa,"))
                    {
                        select.Append("Proprietario.CLI_FISJUR TipoPessoa, ");
                        groupBy.Append("Proprietario.CLI_FISJUR, ");
                    }

                    SetarJoinsProprietario(joins);
                    break;

                case "MotoristaCBO":
                    if (!select.Contains(" MotoristaCBO,"))
                    {
                        select.Append("Motorista.FUN_CBO MotoristaCBO, ");
                        groupBy.Append("Motorista.FUN_CBO, ");

                        SetarJoinsMotorista(joins);
                    }
                    break;

                case "MotoristaDataNascimento":
                case "MotoristaDataNascimentoFormatada":
                    if (!select.Contains(" MotoristaDataNascimento,"))
                    {
                        select.Append("Motorista.FUN_DATANASC MotoristaDataNascimento, ");
                        groupBy.Append("Motorista.FUN_DATANASC, ");

                        SetarJoinsMotorista(joins);
                    }
                    break;

                case "MotoristaFormatado":
                    if (!select.Contains(" MotoristaNome,"))
                    {
                        select.Append("Motorista.FUN_NOME MotoristaNome, ");
                        groupBy.Append("Motorista.FUN_NOME, ");
                    }

                    if (!select.Contains(" MotoristaCPFCNPJ,"))
                    {
                        select.Append("Motorista.FUN_CPF MotoristaCPFCNPJ, ");
                        groupBy.Append("Motorista.FUN_CPF, ");
                    }

                    SetarJoinsMotorista(joins);
                    break;

                case "MotoristaPisPasep":
                    if (!select.Contains(" MotoristaPisPasep,"))
                    {
                        select.Append("Motorista.FUN_PIS MotoristaPisPasep, ");
                        groupBy.Append("Motorista.FUN_PIS, ");

                        SetarJoinsMotorista(joins);
                    }
                    break;

                case "Numero":
                    if (!select.Contains(" Numero,"))
                    {
                        select.Append("CIOT.CIO_NUMERO Numero, ");
                        groupBy.Append("CIOT.CIO_NUMERO, ");

                        SetarJoinsCiot(joins);
                    }
                    break;

                case "PercentualTolerancia":
                    if (!select.Contains(" PercentualTolerancia,"))
                        select.Append("SUM(CargaCIOT.CCO_PERCENTUAL_TOLERANCIA) PercentualTolerancia, ");
                    break;

                case "PercentualToleranciaSuperior":
                    if (!select.Contains(" PercentualToleranciaSuperio,"))
                        select.Append("SUM(CargaCIOT.CCO_PERCENTUAL_TOLERANCIA_SUPERIOR) PercentualToleranciaSuperior, ");
                    break;

                case "PesoBruto":
                    if (!select.Contains(" PesoBruto,"))
                        select.Append("SUM(CargaCIOT.CCO_PESO_BRUTO) PesoBruto, ");
                    break;

                case "BaseCalculoINSS":
                    if (!select.Contains(" BaseCalculoINSS,"))
                        select.Append("SUM(ContratoFrete.CFT_BASE_CALCULO_INSS) BaseCalculoINSS, ");

                    SetarJoinsContratoFrete(joins);
                    break;

                case "AliquotaIRRF":
                    if (!select.Contains(" AliquotaIRRF,"))
                        select.Append("MAX(ContratoFrete.CFT_ALIQUOTA_IRRF) AliquotaIRRF, ");

                    SetarJoinsContratoFrete(joins);
                    break;

                case "BaseCalculoIRRF":
                    if (!select.Contains(" BaseCalculoIRRF,"))
                        select.Append("SUM(ContratoFrete.CFT_BASE_CALCULO_IRRF) BaseCalculoIRRF, ");

                    SetarJoinsContratoFrete(joins);
                    break;

                case "BaseCalculoIRRFSemAcumulo":
                    if (!select.Contains(" BaseCalculoIRRFSemAcumulo,"))
                        select.Append("SUM(ContratoFrete.CFT_BASE_CALCULO_IRRF_SEM_ACUMULO) BaseCalculoIRRFSemAcumulo, ");

                    SetarJoinsContratoFrete(joins);
                    break;

                case "BaseCalculoIRRFSemDesconto":
                    if (!select.Contains(" BaseCalculoIRRFSemDesconto,"))
                        select.Append("SUM(ContratoFrete.CFT_BASE_CALCULO_IRRF_SEM_DESCONTO) BaseCalculoIRRFSemDesconto, ");

                    SetarJoinsContratoFrete(joins);
                    break;

                case "ValorIRRFSemDesconto":
                    if (!select.Contains(" ValorIRRFSemDesconto,"))
                        select.Append("SUM(ContratoFrete.CFT_VALOR_IRRF_SEM_DESCONTO) ValorIRRFSemDesconto, ");

                    SetarJoinsContratoFrete(joins);
                    break;

                case "QuantidadeDependentes":
                    if (!select.Contains(" QuantidadeDependentes,"))
                        select.Append("SUM(ContratoFrete.CFT_QUANTIDADE_DEPENDENTES) QuantidadeDependentes, ");

                    SetarJoinsContratoFrete(joins);
                    break;

                case "ValorPorDependente":
                    if (!select.Contains(" ValorPorDependente,"))
                        select.Append("SUM(ContratoFrete.CFT_VALOR_POR_DEPENDENTE) ValorPorDependente, ");

                    SetarJoinsContratoFrete(joins);
                    break;

                case "ValorTotalDependentes":
                    if (!select.Contains(" ValorTotalDependentes,"))
                        select.Append("SUM(ContratoFrete.CFT_VALOR_TOTAL_DEPENDENTES) ValorTotalDependentes, ");

                    SetarJoinsContratoFrete(joins);
                    break;


                case "Saldo":
                    SetarSelect("ValorFrete", 0, select, joins, groupBy, somenteContarNumeroRegistros, filtroPesquisa);
                    SetarSelect("ValorINSS", 0, select, joins, groupBy, somenteContarNumeroRegistros, filtroPesquisa);
                    SetarSelect("ValorSENAT", 0, select, joins, groupBy, somenteContarNumeroRegistros, filtroPesquisa);
                    SetarSelect("ValorSEST", 0, select, joins, groupBy, somenteContarNumeroRegistros, filtroPesquisa);
                    SetarSelect("ValorIRRF", 0, select, joins, groupBy, somenteContarNumeroRegistros, filtroPesquisa);
                    SetarSelect("ValorOutrosDescontos", 0, select, joins, groupBy, somenteContarNumeroRegistros, filtroPesquisa);
                    SetarSelect("ValorAdiantamento", 0, select, joins, groupBy, somenteContarNumeroRegistros, filtroPesquisa);
                    break;

                case "ValorAdiantamento":
                    if (!select.Contains(" ValorAdiantamento,"))
                        select.Append("SUM(ContratoFrete.CFT_ADIANTAMENTO) ValorAdiantamento, ");

                    SetarJoinsContratoFrete(joins);
                    break;

                case "ValorFrete":
                    if (!select.Contains(" ValorFrete,"))
                        select.Append("SUM(CargaCIOT.CCO_VALOR_FRETE) ValorFrete, ");
                    break;

                case "ValorINSS":
                    if (!select.Contains(" ValorINSS,"))
                        select.Append("SUM(ContratoFrete.CFT_VALOR_INSS) ValorINSS,");

                    SetarJoinsContratoFrete(joins);
                    break;

                case "ValorIRRF":
                    if (!select.Contains(" ValorIRRF,"))
                        select.Append("SUM(ContratoFrete.CFT_VALOR_IRRF) ValorIRRF, ");

                    SetarJoinsContratoFrete(joins);
                    break;

                case "ValorMercadoriaKG":
                    if (!select.Contains(" ValorMercadoriaKG,"))
                        select.Append("SUM(CargaCIOT.CCO_VALOR_MERCADORIA_KG) ValorMercadoriaKG, ");
                    break;

                case "ValorOutrosDescontos":
                    if (!select.Contains(" ValorOutrosDescontos,"))
                        select.Append("SUM(ContratoFrete.CFT_DESCONTO) ValorOutrosDescontos, ");

                    SetarJoinsContratoFrete(joins);
                    break;

                case "ValorPedagio":
                    if (!select.Contains(" ValorPedagio,"))
                        select.Append("SUM(ContratoFrete.CFT_VALOR_PEDAGIO) ValorPedagio, ");

                    SetarJoinsContratoFrete(joins);
                    break;

                case "ValorSeguro":
                    if (!select.Contains(" ValorSeguro,"))
                        select.Append("SUM(CargaCIOT.CCO_VALOR_SEGURO) ValorSeguro, ");
                    break;

                case "ValorSENAT":
                    if (!select.Contains(" ValorSENAT,"))
                        select.Append("SUM(ContratoFrete.CFT_VALOR_SENAT) ValorSENAT, ");

                    SetarJoinsContratoFrete(joins);
                    break;

                case "ValorSEST":
                    if (!select.Contains(" ValorSEST,"))
                        select.Append("SUM(ContratoFrete.CFT_VALOR_SEST) ValorSEST, ");

                    SetarJoinsContratoFrete(joins);
                    break;

                case "ValorTarifaFrete":
                    if (!select.Contains(" ValorTarifaFrete,"))
                        select.Append("SUM(CargaCIOT.CCO_VALOR_TARIFA_FRETE) ValorTarifaFrete, ");
                    break;

                case "ValorTotalMercadoria":
                    if (!select.Contains(" ValorTotalMercadoria,"))
                        select.Append("SUM(CargaCIOT.CCO_VALOR_TOTAL_MERCADORIA) ValorTotalMercadoria, ");
                    break;

                case "DescricaoSituacao":
                    if (!select.Contains(" Situacao,"))
                    {
                        select.Append("CIOT.CIO_SITUACAO Situacao, ");
                        groupBy.Append("CIOT.CIO_SITUACAO, ");

                        SetarJoinsCiot(joins);
                    }
                    break;

                case "DataCargaFormatada":
                    if (!select.Contains(" DataCarga,"))
                    {
                        select.Append("Carga.CAR_DATA_CRIACAO DataCarga, ");
                        groupBy.Append("Carga.CAR_DATA_CRIACAO, ");

                        SetarJoinsCarga(joins);
                    }
                    break;

                case "MensagemCIOT":
                    if (!select.Contains(" MensagemCIOT,"))
                    {
                        select.Append("CIOT.CIO_MENSAGEM MensagemCIOT, ");
                        groupBy.Append("CIOT.CIO_MENSAGEM, ");

                        SetarJoinsCiot(joins);
                    }
                    break;

                case "ProtocoloAutorizacao":
                    if (!select.Contains(" ProtocoloAutorizacao,"))
                    {
                        select.Append("CIOT.CIO_PROTOCOLO_AUTORIZACAO ProtocoloAutorizacao, ");
                        groupBy.Append("CIOT.CIO_PROTOCOLO_AUTORIZACAO, ");

                        SetarJoinsCiot(joins);
                    }
                    break;

                case "VeiculoTracao":
                    if (!select.Contains(" VeiculoTracao,"))
                    {
                        select.Append("VeiculoCIOT.VEI_PLACA VeiculoTracao, ");
                        groupBy.Append("VeiculoCIOT.VEI_PLACA, ");

                        SetarJoinsVeiculoCIOT(joins);
                    }
                    break;

                case "VeiculosReboques":
                    if (!select.Contains(" VeiculosReboques,"))
                    {
                        select.Append("substring((select distinct ', ' + VeiReboque.VEI_PLACA ");
                        select.Append(" from T_CIOT_VEICULOS_VINCULADOS VeiculosCIOTVinculados ");
                        select.Append("inner join T_VEICULO VeiReboque on VeiReboque.VEI_CODIGO = VeiculosCIOTVinculados.VEI_CODIGO ");
                        select.Append("inner join T_CIOT _CIOT on _CIOT.CIO_CODIGO = VeiculosCIOTVinculados.CIO_CODIGO ");
                        select.Append("where VeiReboque.VEI_TIPOVEICULO = 1 ");
                        select.Append("for xml path('')), 3, 200) VeiculosReboques, ");
                    }
                    break;

                case "Destinatario":
                    if (!select.Contains(" Destinatario, "))
                    {
                        select.Append(@" CASE WHEN Trim(DestinatarioPedido.CLI_NOMEFANTASIA) = '' or DestinatarioPedido.CLI_NOMEFANTASIA IS NULL THEN DestinatarioPedido.CLI_NOME ELSE DestinatarioPedido.CLI_NOMEFANTASIA END Destinatario, ");
                        groupBy.Append("DestinatarioPedido.CLI_NOMEFANTASIA, ");
                        groupBy.Append("DestinatarioPedido.CLI_NOME, ");

                        SetarJoinsDestinatarioPedido(joins);
                    }
                    break;

                case "Destino":
                    if (!select.Contains(" Destino, "))
                    {
                        select.Append("DestinoPedido.LOC_DESCRICAO + '-' + DestinoPedido.UF_SIGLA Destino, ");
                        groupBy.Append("DestinoPedido.LOC_DESCRICAO, ");
                        groupBy.Append("DestinoPedido.UF_SIGLA, ");

                        SetarJoinsDestinoPedido(joins);
                    }
                    break;
                //case "PedidosFormatado":
                //    if (!select.Contains(" PedidosFormatado, "))
                //    {
                //        select.Append(@"SUBSTRING((select distinct ', ' + 
										      //          cast(Pedido.PED_NUMERO_PEDIDO_EMBARCADOR as varchar(30)) 
										      //          FROM T_CARGA_PEDIDO cp
										      //          inner join T_PEDIDO as Pedido on Pedido.PED_CODIGO = cp.PED_CODIGO
										      //          WHERE cp.CAR_CODIGO = CargaCIOT.CAR_CODIGO 
						          //      for xml path('')),3,1000) Pedidos, ");

                //        if (!groupBy.Contains("CargaCiot.CAR_CODIGO,"))
                //            groupBy.Append("CargaCiot.CAR_CODIGO, ");
                //    }
                //    break;

            }
        }

        protected override void SetarWhere(Dominio.ObjetosDeValor.Embarcador.Carga.FiltroPesquisaRelatorioCargaCIOTPedido filtrosPesquisa, StringBuilder where, StringBuilder joins, StringBuilder groupBy, List<Embarcador.Consulta.ParametroSQL> parametros = null)
        {
            //where.Append(" AND (CIOT.CIO_SITUACAO = 1 or CIOT.CIO_SITUACAO = 0) ");

            if (filtrosPesquisa.DataEncerramentoInicial != DateTime.MinValue || filtrosPesquisa.DataEncerramentoFinal != DateTime.MinValue)
            {
                if (filtrosPesquisa.DataEncerramentoInicial != DateTime.MinValue)
                    where.Append(" AND CIOT.CIO_DATA_ENCERRAMENTO >= '" + filtrosPesquisa.DataEncerramentoInicial.ToString("yyyy-MM-dd") + "'");

                if (filtrosPesquisa.DataEncerramentoFinal != DateTime.MinValue)
                    where.Append(" AND CIOT.CIO_DATA_ENCERRAMENTO <= '" + filtrosPesquisa.DataEncerramentoFinal.ToString("yyyy-MM-dd") + " 23:59:59'");
            }

            if (filtrosPesquisa.DataAberturaInicial != DateTime.MinValue || filtrosPesquisa.DataAberturaFinal != DateTime.MinValue)
            {
                if (filtrosPesquisa.DataAberturaInicial != DateTime.MinValue)
                    where.Append(" AND CIOT.CIO_DATA_ABERTURA >= '" + filtrosPesquisa.DataAberturaInicial.ToString("yyyy-MM-dd") + "'");

                if (filtrosPesquisa.DataAberturaFinal != DateTime.MinValue)
                    where.Append(" AND CIOT.CIO_DATA_ABERTURA <= '" + filtrosPesquisa.DataAberturaFinal.ToString("yyyy-MM-dd") + " 23:59:59'");
            }

            if (filtrosPesquisa.Proprietario > 0)
            {
                where.Append(" AND Proprietario.CLI_CGCCPF = " + filtrosPesquisa.Proprietario.ToString());

                SetarJoinsProprietario(joins);
            }

            if (filtrosPesquisa.Veiculo > 0)
            {
                where.Append(" AND Veiculo.VEI_CODIGO = " + filtrosPesquisa.Veiculo.ToString());

                SetarJoinsVeiculo(joins);
            }

            if (filtrosPesquisa.Motorista > 0)
            {
                where.Append(" AND Motorista.FUN_CODIGO = " + filtrosPesquisa.Motorista.ToString());

                SetarJoinsMotorista(joins);
            }

            if (!string.IsNullOrWhiteSpace(filtrosPesquisa.Numero))
            {
                where.Append(" AND CIOT.CIO_NUMERO = '" + filtrosPesquisa.Numero + "'");

                SetarJoinsCiot(joins);
            }

            if (!string.IsNullOrWhiteSpace(filtrosPesquisa.Carga))
            {
                where.Append(" AND Carga.CAR_CODIGO_CARGA_EMBARCADOR = '" + filtrosPesquisa.Carga + "'");

                SetarJoinsCarga(joins);
            }

            if (filtrosPesquisa.Situacao != null)
            {
                where.Append(" AND CIOT.CIO_SITUACAO = '" + filtrosPesquisa.Situacao.Value.ToString("D") + "'");

                SetarJoinsCiot(joins);
            }

            if (filtrosPesquisa.Transportador > 0)
            {
                where.Append(" AND Empresa.EMP_CODIGO = " + filtrosPesquisa.Transportador.ToString());

                SetarJoinsEmpresa(joins);
            }
        }

        #endregion
    }
}
