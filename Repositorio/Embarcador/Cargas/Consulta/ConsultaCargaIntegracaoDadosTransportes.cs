using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Repositorio.Embarcador.Cargas
{
    sealed class ConsultaCargaIntegracaoDadosTransportes : Consulta.Consulta<Dominio.ObjetosDeValor.Embarcador.Carga.FiltroPesquisaRelatorioCargaIntegracaoDadosTransportes>
    {
        #region Construtores

        public ConsultaCargaIntegracaoDadosTransportes() : base(tabela: "T_CARGA_DADOS_TRANSPORTE_INTEGRACAO as CargaDadosIntegracao") { }

        #endregion

        #region Métodos Privados

        private void SetarJoinsTipoOperacao(StringBuilder joins)
        {
            if (!joins.Contains(" TipoOperacao "))
                joins.Append(" left join T_TIPO_OPERACAO TipoOperacao on TipoOperacao.TOP_CODIGO = Carga.TOP_CODIGO ");
        }

        private void SetarJoinsGrupoPessoas(StringBuilder joins)
        {
            if (!joins.Contains(" GrupoPessoas "))
                joins.Append(" left join T_GRUPO_PESSOAS GrupoPessoas on GrupoPessoas.GRP_CODIGO = Carga.GRP_CODIGO ");
        }

        private void SetarJoinsFilial(StringBuilder joins)
        {
            if (!joins.Contains(" Filial "))
                joins.Append(" left join T_FILIAL Filial on Filial.FIL_CODIGO = Carga.FIL_CODIGO ");
        }

        private void SetarJoinsModeloVeicular(StringBuilder joins)
        {
            if (!joins.Contains(" ModeloVeicular "))
                joins.Append(" left join T_MODELO_VEICULAR_CARGA ModeloVeicular on ModeloVeicular.MVC_CODIGO = Carga.MVC_CODIGO ");
        }

        private void SetarJoinsCargaDadosSumarizados(StringBuilder joins)
        {
            if (!joins.Contains(" CargaDadosSumarizados "))
                joins.Append(" left join T_CARGA_DADOS_SUMARIZADOS CargaDadosSumarizados on CargaDadosSumarizados.CDS_CODIGO = Carga.CDS_CODIGO ");
        }

        private void SetarJoinsVeiculo(StringBuilder joins)
        {
            if (!joins.Contains(" Veiculo "))
                joins.Append(" left join T_VEICULO Veiculo on Veiculo.VEI_CODIGO = Carga.CAR_VEICULO ");
        }

        private void SetarJoinsTecnologiaRastreador(StringBuilder joins)
        {
            SetarJoinsVeiculo(joins);

            if (!joins.Contains(" TecnologiaRastreador "))
                joins.Append(" left join T_RASTREADOR_TECNOLOGIA TecnologiaRastreador on TecnologiaRastreador.TRA_CODIGO = Veiculo.TRA_CODIGO ");
        }

        private void SetarJoinsTransportador(StringBuilder joins)
        {
            if (!joins.Contains(" Transportador "))
                joins.Append(" left join T_EMPRESA Transportador on Transportador.EMP_CODIGO = Carga.EMP_CODIGO ");
        }

        private void SetarJoinsTipoCarga(StringBuilder joins)
        {
            if (!joins.Contains(" TipoCarga "))
                joins.Append(" left join T_TIPO_DE_CARGA TipoCarga on TipoCarga.TCG_CODIGO = Carga.TCG_CODIGO ");
        }

        #endregion

        #region Métodos Protegidos Sobrescritos

        protected override void SetarSelect(string propriedade, int codigoDinamico, StringBuilder select, StringBuilder joins, StringBuilder groupBy, bool somenteContarNumeroRegistros, Dominio.ObjetosDeValor.Embarcador.Carga.FiltroPesquisaRelatorioCargaIntegracaoDadosTransportes filtrosPesquisa)
        {
            switch (propriedade)
            {
                case "Codigo":
                    if (!select.Contains(" Codigo, "))
                    {
                        select.Append("CargaDadosIntegracao.CAI_CODIGO Codigo, ");
                        groupBy.Append("CargaDadosIntegracao.CAI_CODIGO, ");
                    }
                    break;

                case "GrupoPessoas":
                    if (!select.Contains(" GrupoPessoas, "))
                    {
                        select.Append("GrupoPessoas.GRP_DESCRICAO GrupoPessoas, ");
                        groupBy.Append("GrupoPessoas.GRP_DESCRICAO, ");

                        SetarJoinsGrupoPessoas(joins);
                    }
                    break;

                case "NumeroCarga":
                    if (!select.Contains(" NumeroCarga, "))
                    {
                        select.Append("Carga.CAR_CODIGO_CARGA_EMBARCADOR NumeroCarga, ");
                        groupBy.Append("Carga.CAR_CODIGO_CARGA_EMBARCADOR, ");
                    }
                    break;

                case "Veiculos":
                    if (!select.Contains(" Veiculos, "))
                    {
                        select.Append(@"(Veiculo.VEI_PLACA + 
                                                    ISNULL((SELECT DISTINCT ', ' + veiculo.VEI_PLACA 
                                                    FROM T_CARGA_VEICULOS_VINCULADOS veiculos
                                                    join T_VEICULO veiculo ON veiculo.VEI_CODIGO = veiculos.VEI_CODIGO
                                                    WHERE veiculos.CAR_CODIGO = Carga.CAR_CODIGO FOR XML PATH('')), '')
                                        ) Veiculos, ");

                        groupBy.Append("Veiculo.VEI_PLACA, ");
                        if (!groupBy.Contains("Carga.CAR_CODIGO,"))
                            groupBy.Append("Carga.CAR_CODIGO, ");

                        SetarJoinsVeiculo(joins);
                    }
                    break;

                case "NumeroRastreadorVeiculos":
                    if (!select.Contains(" NumeroRastreadorVeiculos, "))
                    {
                        select.Append(@"(Veiculo.VEI_NUMERO_EQUIPAMENTO_RASTREADOR + 
                                                    ISNULL((SELECT DISTINCT ', ' + veiculo.VEI_NUMERO_EQUIPAMENTO_RASTREADOR 
                                                    FROM T_CARGA_VEICULOS_VINCULADOS veiculos
                                                    join T_VEICULO veiculo ON veiculo.VEI_CODIGO = veiculos.VEI_CODIGO
                                                    WHERE veiculos.CAR_CODIGO = Carga.CAR_CODIGO FOR XML PATH('')), '')
                                        ) NumeroRastreadorVeiculos, ");

                        groupBy.Append("Veiculo.VEI_NUMERO_EQUIPAMENTO_RASTREADOR, ");
                        if (!groupBy.Contains("Carga.CAR_CODIGO,"))
                            groupBy.Append("Carga.CAR_CODIGO, ");

                        SetarJoinsVeiculo(joins);
                    }
                    break;

                case "TecnologiaRastreadorVeiculos":
                    if (!select.Contains(" TecnologiaRastreadorVeiculos, "))
                    {
                        select.Append(@"(TecnologiaRastreador.TRA_DESCRICAO + 
                                                    ISNULL((SELECT DISTINCT ', ' + tecnologiaRastreador.TRA_DESCRICAO 
                                                    FROM T_CARGA_VEICULOS_VINCULADOS veiculos
                                                    join T_VEICULO veiculo ON veiculo.VEI_CODIGO = veiculos.VEI_CODIGO
                                                    join T_RASTREADOR_TECNOLOGIA tecnologiaRastreador ON tecnologiaRastreador.TRA_CODIGO = veiculo.TRA_CODIGO
                                                    WHERE veiculos.CAR_CODIGO = Carga.CAR_CODIGO FOR XML PATH('')), '')
                                        ) TecnologiaRastreadorVeiculos, ");

                        groupBy.Append("TecnologiaRastreador.TRA_DESCRICAO, ");
                        if (!groupBy.Contains("Carga.CAR_CODIGO,"))
                            groupBy.Append("Carga.CAR_CODIGO, ");

                        SetarJoinsTecnologiaRastreador(joins);
                    }
                    break;

                case "CPFMotoristas":
                    if (!select.Contains(" CPFMotoristas, "))
                    {
                        select.Append(@"SUBSTRING((SELECT DISTINCT ', ' + CAST((
                                                        SUBSTRING(Motorista.FUN_CPF, 1, 3) + '.' +
                                                        SUBSTRING(Motorista.FUN_CPF, 4, 3) + '.' +
                                                        SUBSTRING(Motorista.FUN_CPF, 7, 3) + '-' +
                                                        SUBSTRING(Motorista.FUN_CPF, 10, 3)
                                                        ) AS VARCHAR(20))
                                                    FROM T_CARGA_MOTORISTA Motoristas
                                                    join T_FUNCIONARIO Motorista ON Motorista.FUN_CODIGO = Motoristas.CAR_MOTORISTA
                                                    WHERE Motoristas.CAR_CODIGO = Carga.CAR_CODIGO FOR XML PATH('')), 3, 1000) CPFMotoristas, ");

                        if (!groupBy.Contains("Carga.CAR_CODIGO,"))
                            groupBy.Append("Carga.CAR_CODIGO, ");
                    }
                    break;

                case "Motoristas":
                    if (!select.Contains(" Motoristas, "))
                    {
                        select.Append(@"SUBSTRING((SELECT DISTINCT ', ' + Motorista.FUN_NOME 
                                                    FROM T_CARGA_MOTORISTA Motoristas
                                                    join T_FUNCIONARIO Motorista ON Motorista.FUN_CODIGO = Motoristas.CAR_MOTORISTA
                                                    WHERE Motoristas.CAR_CODIGO = Carga.CAR_CODIGO FOR XML PATH('')), 3, 1000) Motoristas, ");

                        if (!groupBy.Contains("Carga.CAR_CODIGO,"))
                            groupBy.Append("Carga.CAR_CODIGO, ");
                    }
                    break;

                case "TipoOperacao":
                    if (!select.Contains(" TipoOperacao, "))
                    {
                        select.Append("TipoOperacao.TOP_DESCRICAO TipoOperacao, ");
                        groupBy.Append("TipoOperacao.TOP_DESCRICAO, ");

                        SetarJoinsTipoOperacao(joins);
                    }
                    break;

                case "DataCargaFormatada":
                    if (!select.Contains(" DataCarga, "))
                    {
                        select.Append("Carga.CAR_DATA_CRIACAO DataCarga, ");
                        groupBy.Append("Carga.CAR_DATA_CRIACAO, ");
                    }
                    break;

                case "TipoIntegracao":
                    if (!select.Contains(" TipoIntegracao, "))
                    {
                        select.Append("TipoIntegracao.TPI_DESCRICAO TipoIntegracao, ");
                        groupBy.Append("TipoIntegracao.TPI_DESCRICAO, ");
                    }
                    break;

                case "Tentativas":
                    if (!select.Contains(" Tentativas, "))
                    {
                        select.Append("CargaDadosIntegracao.INT_NUMERO_TENTATIVAS Tentativas, ");
                        groupBy.Append("CargaDadosIntegracao.INT_NUMERO_TENTATIVAS, ");
                    }
                    break;

                case "DataIntegracao":
                    if (!select.Contains(" DataIntegracao, "))
                    {
                        select.Append("CargaDadosIntegracao.INT_DATA_INTEGRACAO DataIntegracao, ");
                        groupBy.Append("CargaDadosIntegracao.INT_DATA_INTEGRACAO, ");
                    }
                    break;

                case "DescricaoSituacao":
                    if (!select.Contains(" Situacao, "))
                    {
                        select.Append("CargaDadosIntegracao.INT_SITUACAO_INTEGRACAO Situacao, ");
                        groupBy.Append("CargaDadosIntegracao.INT_SITUACAO_INTEGRACAO, ");
                    }
                    break;

                case "Mensagem":
                    if (!select.Contains(" Mensagem, "))
                    {
                        select.Append("CargaDadosIntegracao.INT_PROBLEMA_INTEGRACAO Mensagem, ");
                        groupBy.Append("CargaDadosIntegracao.INT_PROBLEMA_INTEGRACAO, ");
                    }
                    break;

                case "Protocolo":
                    if (!select.Contains(" Protocolo, "))
                    {
                        select.Append("CargaDadosIntegracao.CDI_PROTOCOLO Protocolo, ");
                        groupBy.Append("CargaDadosIntegracao.CDI_PROTOCOLO, ");
                    }
                    break;

                case "DescricaoSituacaoCarga":
                    if (!select.Contains(" SituacaoCarga, "))
                    {
                        select.Append("Carga.CAR_SITUACAO SituacaoCarga, ");
                        groupBy.Append("Carga.CAR_SITUACAO, ");

                        SetarJoinsFilial(joins);
                    }
                    break;

                case "Origem":
                    if (!select.Contains(" Origem, "))
                    {
                        select.Append("CargaDadosSumarizados.CDS_ORIGENS Origem, ");
                        groupBy.Append("CargaDadosSumarizados.CDS_ORIGENS, ");

                        SetarJoinsCargaDadosSumarizados(joins);
                    }
                    break;

                case "Destino":
                    if (!select.Contains(" Destino, "))
                    {
                        select.Append("CargaDadosSumarizados.CDS_DESTINOS Destino, ");
                        groupBy.Append("CargaDadosSumarizados.CDS_DESTINOS, ");

                        SetarJoinsCargaDadosSumarizados(joins);
                    }
                    break;

                case "CNPJFilialFormatado":
                    if (!select.Contains(" CNPJFilial, "))
                    {
                        select.Append("Filial.FIL_CNPJ CNPJFilial, ");
                        groupBy.Append("Filial.FIL_CNPJ, ");

                        SetarJoinsFilial(joins);
                    }
                    break;

                case "Filial":
                    if (!select.Contains(" Filial, "))
                    {
                        select.Append("Filial.FIL_DESCRICAO Filial, ");
                        groupBy.Append("Filial.FIL_DESCRICAO, ");

                        SetarJoinsFilial(joins);
                    }
                    break;

                case "TipoCarga":
                    if (!select.Contains(" TipoCarga, "))
                    {
                        select.Append("TipoCarga.TCG_DESCRICAO TipoCarga, ");
                        groupBy.Append("TipoCarga.TCG_DESCRICAO, ");

                        SetarJoinsTipoCarga(joins);
                    }
                    break;

                case "ModeloVeicular":
                    if (!select.Contains(" ModeloVeicular, "))
                    {
                        select.Append("ModeloVeicular.MVC_DESCRICAO ModeloVeicular, ");
                        groupBy.Append("ModeloVeicular.MVC_DESCRICAO, ");

                        SetarJoinsModeloVeicular(joins);
                    }
                    break;

                case "Transportador":
                    if (!select.Contains(" Transportador, "))
                    {
                        select.Append("Transportador.EMP_RAZAO Transportador, ");
                        groupBy.Append("Transportador.EMP_RAZAO, ");

                        SetarJoinsTransportador(joins);
                    }
                    break;

                case "DataEncerramentoFormatada":
                    if (!select.Contains(" DataEncerramento, "))
                    {
                        select.Append("Carga.CAR_DATA_ENCERRAMENTO_CARGA DataEncerramento, ");
                        groupBy.Append("Carga.CAR_DATA_ENCERRAMENTO_CARGA, ");
                    }
                    break;
            }
        }

        protected override void SetarWhere(Dominio.ObjetosDeValor.Embarcador.Carga.FiltroPesquisaRelatorioCargaIntegracaoDadosTransportes filtrosPesquisa, StringBuilder where, StringBuilder joins, StringBuilder groupBy, List<Embarcador.Consulta.ParametroSQL> parametros = null)
        {
            string pattern = "yyyy-MM-dd";

            joins.Insert(0, @" join T_CARGA Carga on Carga.CAR_CODIGO = CargaDadosIntegracao.CAR_CODIGO 
                               join T_TIPO_INTEGRACAO TipoIntegracao on TipoIntegracao.TPI_CODIGO = CargaDadosIntegracao.TPI_CODIGO");

            if (filtrosPesquisa.DataInicialCarga != DateTime.MinValue)
                where.Append($" and Carga.CAR_DATA_CRIACAO >= '{ filtrosPesquisa.DataInicialCarga.ToString(pattern) }'");

            if (filtrosPesquisa.DataFinalCarga != DateTime.MinValue)
                where.Append($" and Carga.CAR_DATA_CRIACAO <= '{ filtrosPesquisa.DataFinalCarga.AddDays(1).ToString(pattern) }'");

            if (filtrosPesquisa.DataInicioIntegracao != DateTime.MinValue)
                where.Append($" AND CAST(CargaDadosIntegracao.INT_DATA_INTEGRACAO AS DATE) >= '{ filtrosPesquisa.DataInicioIntegracao.ToString(pattern) }'");

            if (filtrosPesquisa.DataFinalIntegracao != DateTime.MinValue)
                where.Append($" AND CAST(CargaDadosIntegracao.INT_DATA_INTEGRACAO AS DATE) <= '{ filtrosPesquisa.DataFinalIntegracao.ToString(pattern) }'");

            if (filtrosPesquisa.DataInicioEncerramento != DateTime.MinValue)
                where.Append($" AND CAST(Carga.CAR_DATA_ENCERRAMENTO_CARGA AS DATE) >= '{ filtrosPesquisa.DataInicioEncerramento.ToString(pattern) }'");

            if (filtrosPesquisa.DataFinalEncerramento != DateTime.MinValue)
                where.Append($" AND CAST(Carga.CAR_DATA_ENCERRAMENTO_CARGA AS DATE) <= '{ filtrosPesquisa.DataFinalEncerramento.ToString(pattern) }'");

            if (filtrosPesquisa.CodigoTipoOperacao > 0)
                where.Append($" and Carga.TOP_CODIGO = {filtrosPesquisa.CodigoTipoOperacao}");

            if (filtrosPesquisa.Situacao.HasValue)
                where.Append($" and CargaDadosIntegracao.INT_SITUACAO_INTEGRACAO = {filtrosPesquisa.Situacao.Value.ToString("d")}");

            if (filtrosPesquisa.CodigoCarga > 0)
                where.Append($" and Carga.CAR_CODIGO = {filtrosPesquisa.CodigoCarga}");

            if (filtrosPesquisa.TipoIntegracao > 0)
                where.Append($" and TipoIntegracao.TPI_TIPO = {(int)filtrosPesquisa.TipoIntegracao}");

            if (filtrosPesquisa.CodigoTransportador > 0)
                where.Append($" and Carga.EMP_CODIGO = {filtrosPesquisa.CodigoTransportador}");

            if (filtrosPesquisa.CodigoFilial > 0)
                where.Append($" and Carga.FIL_CODIGO = {filtrosPesquisa.CodigoFilial}");
        }

        #endregion
    }
}
