using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Repositorio.Embarcador.Logistica.Consulta
{
    sealed class ConsultaSolicitacaoAbastecimentoGas : Embarcador.Consulta.Consulta<Dominio.ObjetosDeValor.Embarcador.Logistica.FiltroPesquisaRelatorioConsolidacaoGas>
    {
        #region Construtores

        public ConsultaSolicitacaoAbastecimentoGas() : base(tabela: "T_SOLICITACAO_ABASTECIMENTO_GAS as SolicitacaoGas") { }

        #endregion

        #region Métodos Privados 
        private void SetarJoinsConsolidacaoGas(StringBuilder joins)
        {
            if (!joins.Contains(" ConsolidacaoGas "))
                joins.Append("LEFT JOIN T_CONSOLIDACAO_SOLICITACAO_ABASTECIMENTO_GAS ConsolidacaoGas on ConsolidacaoGas.SAG_CODIGO = SolicitacaoGas.SAG_CODIGO ");
        }

        private void SetarJoinsClienteBase(StringBuilder joins)
        {
            if (!joins.Contains(" ClienteBase "))
                joins.Append("LEFT JOIN T_CLIENTE ClienteBase on ClienteBase.CLI_CGCCPF = SolicitacaoGas.CLI_CGCCPF ");
        }

        private void SetarJoinsClienteSupridor(StringBuilder joins)
        {
            SetarJoinsConsolidacaoGas(joins);
            if (!joins.Contains(" ClienteSupridor "))
                joins.Append("LEFT JOIN T_CLIENTE ClienteSupridor on ClienteSupridor.CLI_CGCCPF = ConsolidacaoGas.CLI_CGCCPF ");
        }

        private void SetarJoinsProduto(StringBuilder joins)
        {
            if (!joins.Contains(" Produto "))
                joins.Append("LEFT JOIN T_PRODUTO_EMBARCADOR Produto on Produto.PRO_CODIGO = SolicitacaoGas.PRO_CODIGO ");
        }

        private void SetarJoinsUsuario(StringBuilder joins)
        {
            if (!joins.Contains(" Usuario "))
                joins.Append("LEFT JOIN T_FUNCIONARIO Usuario on Usuario.FUN_CODIGO = SolicitacaoGas.FUN_CODIGO ");
        }

        private void SetarJoinsUsuarioAdicaoQuantidade(StringBuilder joins)
        {
            if (!joins.Contains(" UsuarioAdicaoQuantidade "))
                joins.Append("LEFT JOIN T_FUNCIONARIO UsuarioAdicaoQuantidade on UsuarioAdicaoQuantidade.FUN_CODIGO = SolicitacaoGas.FUN_CODIGO_ADICAO_QUANTIDADE ");
        }

        private void SetarJoinsModeloVeicular(StringBuilder joins)
        {
            SetarJoinsConsolidacaoGas(joins);
            if (!joins.Contains(" ModeloVeicular "))
                joins.Append("LEFT JOIN T_MODELO_VEICULAR_CARGA ModeloVeicular on ModeloVeicular.MVC_CODIGO = ConsolidacaoGas.MVC_CODIGO ");
        }

        private void SetarJoinsTipoDeCarga(StringBuilder joins)
        {
            SetarJoinsConsolidacaoGas(joins);
            if (!joins.Contains(" TipoDeCarga "))
                joins.Append("LEFT JOIN T_TIPO_DE_CARGA TipoDeCarga on TipoDeCarga.TCG_CODIGO = ConsolidacaoGas.TCG_CODIGO ");
        }

        private void SetarJoinsTipoOperacao(StringBuilder joins)
        {
            SetarJoinsConsolidacaoGas(joins);
            if (!joins.Contains(" TipoOperacao "))
                joins.Append("LEFT JOIN T_TIPO_OPERACAO TipoOperacao on TipoOperacao.TOP_CODIGO = ConsolidacaoGas.TOP_CODIGO ");
        }

        #endregion

        #region Métodos Protegidos Sobrescritos

        protected override void SetarSelect(string propriedade, int codigoDinamico, StringBuilder select, StringBuilder joins, StringBuilder groupBy, bool somenteContarNumeroRegistros, Dominio.ObjetosDeValor.Embarcador.Logistica.FiltroPesquisaRelatorioConsolidacaoGas filtroPesquisa)
        {
            if (!select.Contains(" Codigo, "))
            {
                select.Append("SolicitacaoGas.SAG_CODIGO as Codigo, ");
                groupBy.Append("SolicitacaoGas.SAG_CODIGO, ");
            }

            switch (propriedade)
            {
                case "ClienteBase":
                case "ClienteBaseDescricao":
                    if (!select.Contains(" ClienteBase, "))
                    {
                        SetarJoinsClienteBase(joins);
                        select.Append("ClienteBase.CLI_NOME as ClienteBase, ");
                        select.Append("ClienteBase.CLI_CODIGO_INTEGRACAO as ClienteBaseCodigoIntegracao, ");
                        groupBy.Append("ClienteBase.CLI_NOME, ClienteBase.CLI_CODIGO_INTEGRACAO, ");
                    }
                    break;

                case "DataMedicaoFormatada":
                case "DataMedicao":
                    if (!select.Contains(" DataMedicao, "))
                    {
                        select.Append("SolicitacaoGas.SAG_DATA_MEDICAO as DataMedicao, ");
                        groupBy.Append("SolicitacaoGas.SAG_DATA_MEDICAO, ");
                    }
                    break;

                case "Abertura":
                    if (!select.Contains(" Abertura, "))
                    {
                        select.Append("CAST(SolicitacaoGas.SAG_ABERTURA as decimal(18,2)) as Abertura, ");
                        groupBy.Append("SolicitacaoGas.SAG_ABERTURA, ");
                    }
                    break;

                case "DensidadeAberturaDia":
                    if (!select.Contains(" DensidadeAberturaDia, "))
                    {
                        select.Append("CAST(SolicitacaoGas.SAG_DENSIDADE_ABERTURA_DIA as decimal(18,2)) as DensidadeAberturaDia, ");
                        groupBy.Append("SolicitacaoGas.SAG_DENSIDADE_ABERTURA_DIA, ");
                    }
                    break;

                case "PorcentagemAbertura":
                    if (!select.Contains(" PorcentagemAbertura, "))
                    {
                        SetarJoinsProduto(joins);
                        SetarJoinsClienteSupridor(joins);

                        select.Append(@"CAST(((CAST(SolicitacaoGas.SAG_ABERTURA as decimal(18,
                                           2)) *100) / case when CAST ((
                                                                       SELECT SUM(_suprimentoGas.SDG_CAPACIDADE)
                                                                         FROM T_SUPRIMENTO_DE_GAS _suprimentoGas
                                                                         JOIN T_FILIAL_SUPRIMENTO_DE_GAS _clienteSuprimento
                                                                           ON _clienteSuprimento.SDG_CODIGO = _suprimentoGas.SDG_CODIGO
                                                                        WHERE _suprimentoGas.PRO_CODIGO = Produto.PRO_CODIGO 
                                                                          AND ClienteSupridor.CLI_CGCCPF = _clienteSuprimento.CLI_CGCCPF
                                                                      )
                                                                      as decimal(18,
                                           2)) <= 0 then 1 else CAST((
                                                                       SELECT SUM(_suprimentoGas.SDG_CAPACIDADE)
                                                                         FROM T_SUPRIMENTO_DE_GAS _suprimentoGas
                                                                         JOIN T_FILIAL_SUPRIMENTO_DE_GAS _clienteSuprimento
                                                                           ON _clienteSuprimento.SDG_CODIGO = _suprimentoGas.SDG_CODIGO
                                                                        WHERE _suprimentoGas.PRO_CODIGO = Produto.PRO_CODIGO 
                                                                          AND ClienteSupridor.CLI_CGCCPF = _clienteSuprimento.CLI_CGCCPF
                                                                      ) as decimal(18,
                                           2)) end) as decimal(18, 2)) as PorcentagemAbertura, ");
                        groupBy.Append("SolicitacaoGas.SAG_ABERTURA, Produto.PRO_CODIGO, ClienteSupridor.CLI_CGCCPF, ");
                    }
                    break;

                case "Capacidade":
                    if (!select.Contains(" Capacidade, "))
                    {
                        SetarJoinsProduto(joins);
                        SetarJoinsClienteSupridor(joins);

                        select.Append(@"CAST((SELECT SUM(_suprimentoGas.SDG_CAPACIDADE)
                                                                         FROM T_SUPRIMENTO_DE_GAS _suprimentoGas
                                                                         JOIN T_FILIAL_SUPRIMENTO_DE_GAS _clienteSuprimento
                                                                           ON _clienteSuprimento.SDG_CODIGO = _suprimentoGas.SDG_CODIGO
                                                                        WHERE _suprimentoGas.PRO_CODIGO = Produto.PRO_CODIGO
                                                                          AND ClienteSupridor.CLI_CGCCPF = _clienteSuprimento.CLI_CGCCPF) as decimal(18,2)) as Capacidade, ");
                        groupBy.Append("Produto.PRO_CODIGO, ClienteSupridor.CLI_CGCCPF, ");
                    }
                    break;

                case "Lastro":
                    if (!select.Contains(" Lastro, "))
                    {
                        SetarJoinsProduto(joins);
                        SetarJoinsClienteSupridor(joins);

                        select.Append(@"CAST((SELECT SUM(_suprimentoGas.SDG_LASTRO)
                                                                         FROM T_SUPRIMENTO_DE_GAS _suprimentoGas
                                                                         JOIN T_FILIAL_SUPRIMENTO_DE_GAS _clienteSuprimento
                                                                           ON _clienteSuprimento.SDG_CODIGO = _suprimentoGas.SDG_CODIGO
                                                                        WHERE _suprimentoGas.PRO_CODIGO = Produto.PRO_CODIGO
                                                                          AND ClienteSupridor.CLI_CGCCPF = _clienteSuprimento.CLI_CGCCPF) as decimal(18,2)) as Lastro, ");
                        groupBy.Append("Produto.PRO_CODIGO, ClienteSupridor.CLI_CGCCPF, ");
                    }
                    break;

                case "EstoqueMinimo":
                    if (!select.Contains(" EstoqueMinimo, "))
                    {
                        SetarJoinsProduto(joins);
                        SetarJoinsClienteSupridor(joins);

                        select.Append(@"CAST((SELECT SUM(_suprimentoGas.SDG_ESTOQUE_MINIMO)
                                                                         FROM T_SUPRIMENTO_DE_GAS _suprimentoGas
                                                                         JOIN T_FILIAL_SUPRIMENTO_DE_GAS _clienteSuprimento
                                                                           ON _clienteSuprimento.SDG_CODIGO = _suprimentoGas.SDG_CODIGO
                                                                        WHERE _suprimentoGas.PRO_CODIGO = Produto.PRO_CODIGO
                                                                          AND ClienteSupridor.CLI_CGCCPF = _clienteSuprimento.CLI_CGCCPF) as decimal(18,2)) as EstoqueMinimo, ");
                        groupBy.Append("Produto.PRO_CODIGO, ClienteSupridor.CLI_CGCCPF, ");
                    }
                    break;

                case "EstoqueMaximo":
                    if (!select.Contains(" EstoqueMaximo, "))
                    {
                        SetarJoinsProduto(joins);
                        SetarJoinsClienteSupridor(joins);

                        select.Append(@"CAST((SELECT SUM(_suprimentoGas.SDG_ESTOQUE_MAXIMO)
                                                                         FROM T_SUPRIMENTO_DE_GAS _suprimentoGas
                                                                         JOIN T_FILIAL_SUPRIMENTO_DE_GAS _clienteSuprimento
                                                                           ON _clienteSuprimento.SDG_CODIGO = _suprimentoGas.SDG_CODIGO
                                                                        WHERE _suprimentoGas.PRO_CODIGO = Produto.PRO_CODIGO
                                                                          AND ClienteSupridor.CLI_CGCCPF = _clienteSuprimento.CLI_CGCCPF) as decimal(18,2)) as EstoqueMaximo, ");
                        groupBy.Append("Produto.PRO_CODIGO, ClienteSupridor.CLI_CGCCPF, ");
                    }
                    break;

                case "Produto":
                    if (!select.Contains(" Produto, "))
                    {
                        SetarJoinsProduto(joins);
                        select.Append("Produto.GRP_DESCRICAO as Produto, ");
                        groupBy.Append("Produto.GRP_DESCRICAO, ");
                    }
                    break;

                case "PrevisaoBombeio":
                    if (!select.Contains(" PrevisaoBombeio, "))
                    {
                        select.Append("CAST(SolicitacaoGas.SAG_PREVISAO_BOMBEIO as decimal(18,2)) as PrevisaoBombeio, ");
                        groupBy.Append("SolicitacaoGas.SAG_PREVISAO_BOMBEIO, ");
                    }
                    break;

                case "PrevisaoTransferenciaRecebida":
                    if (!select.Contains(" PrevisaoTransferenciaRecebida, "))
                    {
                        select.Append("CAST(SolicitacaoGas.SAG_PREVISAO_TRANSFERENCIA_RECEBIDA as decimal(18,2)) as PrevisaoTransferenciaRecebida, ");
                        groupBy.Append("SolicitacaoGas.SAG_PREVISAO_TRANSFERENCIA_RECEBIDA, ");
                    }
                    break;

                case "PrevisaoDemandaDomiciliar":
                    if (!select.Contains(" PrevisaoDemandaDomiciliar, "))
                    {
                        select.Append("CAST(SolicitacaoGas.SAG_PREVISAO_DEMANDA_DOMICILIAR as decimal(18,2)) as PrevisaoDemandaDomiciliar, ");
                        groupBy.Append("SolicitacaoGas.SAG_PREVISAO_DEMANDA_DOMICILIAR, ");
                    }
                    break;

                case "PrevisaoDemandaEmpresarial":
                    if (!select.Contains(" PrevisaoDemandaEmpresarial, "))
                    {
                        select.Append("CAST(SolicitacaoGas.SAG_PREVISAO_DEMANDA_EMPRESARIAL as decimal(18,2)) as PrevisaoDemandaEmpresarial, ");
                        groupBy.Append("SolicitacaoGas.SAG_PREVISAO_DEMANDA_EMPRESARIAL, ");
                    }
                    break;

                case "EstoqueUltrasystem":
                    if (!select.Contains(" EstoqueUltrasystem, "))
                    {
                        select.Append("CAST(SolicitacaoGas.SAG_ESTOQUE_ULTRASYSTEM as decimal(18,2)) as EstoqueUltrasystem, ");
                        groupBy.Append("SolicitacaoGas.SAG_ESTOQUE_ULTRASYSTEM, ");
                    }
                    break;

                case "PrevisaoTransferenciaEnviada":
                    if (!select.Contains(" PrevisaoTransferenciaEnviada, "))
                    {
                        select.Append("CAST(SolicitacaoGas.SAG_PREVISAO_TRANSFERENCIA_ENVIADA as decimal(18,2)) as PrevisaoTransferenciaEnviada, ");
                        groupBy.Append("SolicitacaoGas.SAG_PREVISAO_TRANSFERENCIA_ENVIADA, ");
                    }
                    break;

                case "PrevisaoFechamento":
                    if (!select.Contains(" PrevisaoFechamento, "))
                    {
                        select.Append("CAST(SolicitacaoGas.SAG_PREVISAO_FECHAMENTO as decimal(18,2)) as PrevisaoFechamento, ");
                        groupBy.Append("SolicitacaoGas.SAG_PREVISAO_FECHAMENTO, ");
                    }
                    break;

                case "VolumeRodoviarioCarregamentoProximoDia":
                    if (!select.Contains(" VolumeRodoviarioCarregamentoProximoDia, "))
                    {
                        select.Append("CAST((SolicitacaoGas.SAG_VOLUME_RODOVIARIO_CARREGAMENTO_PROXIMO_DIA + SolicitacaoGas.SAG_ADICIONAL_VOLUME_RODOVIARIO_CARREGAMENTO_PROXIMO_DIA) as decimal(18,2)) as VolumeRodoviarioCarregamentoProximoDia, ");
                        groupBy.Append("SolicitacaoGas.SAG_VOLUME_RODOVIARIO_CARREGAMENTO_PROXIMO_DIA, SolicitacaoGas.SAG_ADICIONAL_VOLUME_RODOVIARIO_CARREGAMENTO_PROXIMO_DIA, ");
                    }
                    break;

                case "PrevisaoBombeioProximoDia":
                    if (!select.Contains(" PrevisaoBombeioProximoDia, "))
                    {
                        select.Append("CAST(SolicitacaoGas.SAG_PREVISAO_BOMBEIO_PROXIMO_DIA as decimal(18,2)) as PrevisaoBombeioProximoDia, ");
                        groupBy.Append("SolicitacaoGas.SAG_PREVISAO_BOMBEIO_PROXIMO_DIA, ");
                    }
                    break;

                case "DisponibilidadeTransferenciaProximoDia":
                    if (!select.Contains(" DisponibilidadeTransferenciaProximoDia, "))
                    {
                        select.Append("CAST((SolicitacaoGas.SAG_DISPONIBILIDADE_TRANSFERENCIA_PROXIMO_DIA + SolicitacaoGas.SAG_ADICIONAL_DISPONIBILIDADE_TRANSFERENCIA_PROXIMO_DIA) as decimal(18,2)) as DisponibilidadeTransferenciaProximoDia, ");
                        groupBy.Append("SolicitacaoGas.SAG_DISPONIBILIDADE_TRANSFERENCIA_PROXIMO_DIA, SolicitacaoGas.SAG_ADICIONAL_DISPONIBILIDADE_TRANSFERENCIA_PROXIMO_DIA, ");
                    }
                    break;

                case "SaldoDisponibilidadeTransferencia":
                    {
                        StringBuilder sbSaldoRestante = new StringBuilder();
                        sbSaldoRestante.Append(@"(SELECT (
                                                            SolicitacaoGas.SAG_DISPONIBILIDADE_TRANSFERENCIA_PROXIMO_DIA + SolicitacaoGas.SAG_ADICIONAL_DISPONIBILIDADE_TRANSFERENCIA_PROXIMO_DIA - 
                                                            (
                                                                SELECT SUM(_consolidacao.CSA_QUANTIDADE_CARGA)
                                                                  FROM T_CONSOLIDACAO_SOLICITACAO_ABASTECIMENTO_GAS _consolidacao
                                                                  JOIN T_SOLICITACAO_ABASTECIMENTO_GAS _solicitacaoGas on _solicitacaoGas.SAG_CODIGO = _consolidacao.SAG_CODIGO
                                                                 WHERE _solicitacaoGas.PRO_CODIGO = SolicitacaoGas.PRO_CODIGO AND _consolidacao.CLI_CGCCPF = SolicitacaoGas.CLI_CGCCPF AND SolicitacaoGas.SAG_DATA_MEDICAO = _solicitacaoGas.SAG_DATA_MEDICAO
                                                            )
                                                          )) SaldoDisponibilidadeTransferencia, ");

                        select.Append(sbSaldoRestante.ToString());
                        groupBy.Append("SolicitacaoGas.PRO_CODIGO, SolicitacaoGas.CLI_CGCCPF, SolicitacaoGas.SAG_DISPONIBILIDADE_TRANSFERENCIA_PROXIMO_DIA, SolicitacaoGas.SAG_DATA_MEDICAO, SolicitacaoGas.SAG_ADICIONAL_DISPONIBILIDADE_TRANSFERENCIA_PROXIMO_DIA, ");
                    }
                    break;

                case "VeiculosPlanejados":
                    if (!select.Contains(" VeiculosPlanejados, "))
                    {
                        select.Append("(SELECT COUNT(_consolidacao.CSA_CODIGO) FROM T_CONSOLIDACAO_SOLICITACAO_ABASTECIMENTO_GAS _consolidacao WHERE _consolidacao.SAG_CODIGO = SolicitacaoGas.SAG_CODIGO) as VeiculosPlanejados, ");
                        groupBy.Append("SolicitacaoGas.SAG_CODIGO, ");
                    }
                    break;

                case "DemandaPlanejada":
                    if (!select.Contains(" DemandaPlanejada, "))
                    {
                        select.Append("(SolicitacaoGas.SAG_PREVISAO_DEMANDA_DOMICILIAR + SolicitacaoGas.SAG_PREVISAO_DEMANDA_EMPRESARIAL) as DemandaPlanejada,");
                        groupBy.Append("SolicitacaoGas.SAG_PREVISAO_DEMANDA_DOMICILIAR, SolicitacaoGas.SAG_PREVISAO_DEMANDA_EMPRESARIAL, ");
                    }
                    break;

                case "DemandaPlanejar":
                    if (!select.Contains(" DemandaPlanejar, "))
                    {
                        select.Append("(SELECT SolicitacaoGas.SAG_SALDO_RESTANTE + SolicitacaoGas.SAG_ADICIONAL_DISPONIBILIDADE_TRANSFERENCIA_PROXIMO_DIA - SUM(_consolidacao.CSA_QUANTIDADE_CARGA) FROM T_CONSOLIDACAO_SOLICITACAO_ABASTECIMENTO_GAS _consolidacao WHERE _consolidacao.SAG_CODIGO = SolicitacaoGas.SAG_CODIGO) as DemandaPlanejar, ");
                        groupBy.Append("SolicitacaoGas.SAG_CODIGO, SolicitacaoGas.SAG_SALDO_RESTANTE, SolicitacaoGas.SAG_ADICIONAL_DISPONIBILIDADE_TRANSFERENCIA_PROXIMO_DIA, ");
                    }
                    break;

                case "ClienteSupridor":
                case "ClienteSupridorDescricao":
                    if (!select.Contains(" ClienteSupridor, "))
                    {
                        SetarJoinsClienteSupridor(joins);
                        select.Append("ClienteSupridor.CLI_NOME as ClienteSupridor, ClienteSupridor.CLI_CODIGO_INTEGRACAO as ClienteSupridorCodigoIntegracao, ");
                        groupBy.Append("ClienteSupridor.CLI_NOME, ClienteSupridor.CLI_CODIGO_INTEGRACAO, ");
                    }
                    break;

                case "TipoDeCarga":
                    if (!select.Contains(" TipoDeCarga, "))
                    {
                        SetarJoinsTipoDeCarga(joins);
                        select.Append("TipoDeCarga.TCG_DESCRICAO as TipoDeCarga, ");
                        groupBy.Append("TipoDeCarga.TCG_DESCRICAO, ");
                    }
                    break;

                case "TipoOperacao":
                    if (!select.Contains(" TipoOperacao, "))
                    {
                        SetarJoinsTipoOperacao(joins);
                        select.Append("TipoOperacao.TOP_DESCRICAO as TipoOperacao, ");
                        groupBy.Append("TipoOperacao.TOP_DESCRICAO, ");
                    }
                    break;

                case "ModeloVeicular":
                    if (!select.Contains(" ModeloVeicular, "))
                    {
                        SetarJoinsModeloVeicular(joins);
                        select.Append("ModeloVeicular.MVC_DESCRICAO as ModeloVeicular, ");
                        groupBy.Append("ModeloVeicular.MVC_DESCRICAO, ");
                    }
                    break;

                case "Observacao":

                    break;

                case "DataUltimaAlteracao":
                case "DataUltimaAlteracaoFormatada":
                    if (!select.Contains(" DataUltimaAlteracao, "))
                    {
                        select.Append("SolicitacaoGas.SAG_DATA_ULTIMA_ALTERACAO as DataUltimaAlteracao, ");
                        groupBy.Append("SolicitacaoGas.SAG_DATA_ULTIMA_ALTERACAO, ");
                    }
                    break;

                case "Usuario":
                    if (!select.Contains(" Usuario, "))
                    {
                        SetarJoinsUsuario(joins);
                        select.Append("Usuario.FUN_NOME as Usuario, ");
                        groupBy.Append("Usuario.FUN_NOME, ");
                    }
                    break;

                case "DataAdicaoQuantidade":
                case "DataAdicaoQuantidadeFormatada":
                    if (!select.Contains(" DataAdicaoQuantidade, "))
                    {
                        select.Append("SolicitacaoGas.SAG_DATA_ADICAO_QUANTIDADE as DataAdicaoQuantidade, ");
                        groupBy.Append("SolicitacaoGas.SAG_DATA_ADICAO_QUANTIDADE, ");
                    }
                    break;

                case "AdicionalVolumeRodoviarioCarregamentoProximoDia":
                    if (!select.Contains(" AdicionalVolumeRodoviarioCarregamentoProximoDia, "))
                    {
                        select.Append("SolicitacaoGas.SAG_ADICIONAL_VOLUME_RODOVIARIO_CARREGAMENTO_PROXIMO_DIA as AdicionalVolumeRodoviarioCarregamentoProximoDia, ");
                        groupBy.Append("SolicitacaoGas.SAG_ADICIONAL_VOLUME_RODOVIARIO_CARREGAMENTO_PROXIMO_DIA, ");
                    }
                    break;

                case "AdicionalDisponibilidadeTransferenciaProximoDia":
                    if (!select.Contains(" AdicionalDisponibilidadeTransferenciaProximoDia, "))
                    {
                        select.Append("SolicitacaoGas.SAG_ADICIONAL_DISPONIBILIDADE_TRANSFERENCIA_PROXIMO_DIA as AdicionalDisponibilidadeTransferenciaProximoDia, ");
                        groupBy.Append("SolicitacaoGas.SAG_ADICIONAL_DISPONIBILIDADE_TRANSFERENCIA_PROXIMO_DIA, ");
                    }
                    break;

                case "UsuarioAdicaoQuantidade":
                    if (!select.Contains(" Usuario, "))
                    {
                        SetarJoinsUsuarioAdicaoQuantidade(joins);
                        select.Append("UsuarioAdicaoQuantidade.FUN_NOME as UsuarioAdicaoQuantidade, ");
                        groupBy.Append("UsuarioAdicaoQuantidade.FUN_NOME, ");
                    }
                    break;
            }
        }

        protected override void SetarWhere(Dominio.ObjetosDeValor.Embarcador.Logistica.FiltroPesquisaRelatorioConsolidacaoGas filtrosPesquisa, StringBuilder where, StringBuilder joins, StringBuilder groupBy, List<Embarcador.Consulta.ParametroSQL> parametros = null)
        {
            if (filtrosPesquisa.DataInicial != DateTime.MinValue)
                where.Append($" and SolicitacaoGas.SAG_DATA_MEDICAO >= '{filtrosPesquisa.DataInicial.ToString("yyyyMMdd HH:mm:ss")}'");

            if (filtrosPesquisa.DataFinal != DateTime.MinValue)
                where.Append($" and SolicitacaoGas.SAG_DATA_MEDICAO <= '{filtrosPesquisa.DataFinal.ToString("yyyyMMdd HH:mm:ss")}'");

            if (filtrosPesquisa.Bases != null && filtrosPesquisa.Bases.Count > 0)
                where.Append($" and SolicitacaoGas.CLI_CGCCPF IN ({string.Join(", ", filtrosPesquisa.Bases)})");

            if (filtrosPesquisa.TipoFilial == Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoFilialAbastecimentoGas.Supridora)
            {
                SetarJoinsClienteBase(joins);
                where.Append($" and ClienteBase.CLI_HABILITAR_SOLICITACAO_GAS = 1");
            }
            else if (filtrosPesquisa.TipoFilial == Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoFilialAbastecimentoGas.Satelite)
            {
                SetarJoinsClienteBase(joins);
                where.Append($" and ClienteBase.CLI_HABILITAR_SOLICITACAO_GAS = 0");
            }

            if (filtrosPesquisa.DisponibilidadeTransferencia == Dominio.ObjetosDeValor.Embarcador.Enumeradores.SimNao.Sim)
                where.Append($" and SolicitacaoGas.SAG_DISPONIBILIDADE_TRANSFERENCIA_PROXIMO_DIA > 0");
            else if (filtrosPesquisa.DisponibilidadeTransferencia == Dominio.ObjetosDeValor.Embarcador.Enumeradores.SimNao.Nao)
                where.Append($" and SolicitacaoGas.SAG_DISPONIBILIDADE_TRANSFERENCIA_PROXIMO_DIA <= 0");

            if (filtrosPesquisa.VolumeRodoviario == Dominio.ObjetosDeValor.Embarcador.Enumeradores.SimNao.Sim)
                where.Append($" and SolicitacaoGas.SAG_VOLUME_RODOVIARIO_CARREGAMENTO_PROXIMO_DIA > 0");
            else if (filtrosPesquisa.VolumeRodoviario == Dominio.ObjetosDeValor.Embarcador.Enumeradores.SimNao.Nao)
                where.Append($" and SolicitacaoGas.SAG_VOLUME_RODOVIARIO_CARREGAMENTO_PROXIMO_DIA <= 0");

            #endregion
        }
    }
}

