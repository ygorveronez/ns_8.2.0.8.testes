using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Repositorio.Embarcador.Cargas
{
    sealed class ConsultaCargaViagemEventos : Consulta.Consulta<Dominio.ObjetosDeValor.Embarcador.Carga.FiltroPesquisaRelatorioCargaViagemEventos>
    {
        #region Construtores

        public ConsultaCargaViagemEventos() : base(tabela: "T_CARGA_ENTREGA_EVENTO AS EntregaEventos") { }

        #endregion

        #region Métodos Privados

        private void SetarJoinsCarga(StringBuilder joins)
        {
            if (!joins.Contains("Carga"))
                joins.Append(" left join T_CARGA Carga on Carga.CAR_CODIGO = EntregaEventos.CAR_CODIGO ");
        }

        private void SetarJoinsDadosSumarizadosRemetentesLocalidades(StringBuilder joins)
        {
            if (!joins.Contains("CargaDadosSumarizadosRemetentesLocalidadesCliente"))
                joins.Append(" left join T_CLIENTE CargaDadosSumarizadosRemetentesLocalidadesCliente on CargaDadosSumarizadosRemetentes.CLI_CGCCPF = CargaDadosSumarizadosRemetentesLocalidadesCliente.CLI_CGCCPF ");
            
            SetarJoinsDadosSumarizadosRemetentes(joins);
        }

        private void SetarJoinsCargaEntrega(StringBuilder joins)
        {
            if (!joins.Contains("CargaEntrega"))
                joins.Append(" left join T_CARGA_ENTREGA CargaEntrega on Carga.CAR_CODIGO = CargaEntrega.CAR_CODIGO ");
        }

        private void SetarJoinsDadosSumarizadosDestinatariosLocalidades(StringBuilder joins)
        {
            if (!joins.Contains("CargaDadosSumarizadosDestinatariosLocalidadesCliente"))
                joins.Append(" left join T_CLIENTE CargaDadosSumarizadosDestinatariosLocalidadesCliente on CargaDadosSumarizadosDestinatarios.CLI_CGCCPF = CargaDadosSumarizadosDestinatariosLocalidadesCliente.CLI_CGCCPF ");

            SetarJoinsDadosSumarizadosDestinatarios(joins);
        }

        private void SetarJoinsDadosSumarizadosRemetentes(StringBuilder joins)
        {
            if (!joins.Contains("CargaDadosSumarizadosRemetentes"))
                joins.Append(" left join T_CARGA_DADOS_SUMARIZADOS_REMETENTES CargaDadosSumarizadosRemetentes on CargaDadosSumarizados.CDS_CODIGO = CargaDadosSumarizadosRemetentes.CDS_CODIGO ");
        }

        private void SetarJoinsDadosSumarizadosDestinatarios(StringBuilder joins)
        {
            if (!joins.Contains("CargaDadosSumarizadosDestinatarios"))
                joins.Append(" left join T_CARGA_DADOS_SUMARIZADOS_DESTINATARIOS CargaDadosSumarizadosDestinatarios on CargaDadosSumarizados.CDS_CODIGO = CargaDadosSumarizadosDestinatarios.CDS_CODIGO ");
        }

        private void SetarJoinsCargaTransportador(StringBuilder joins)
        {
            if (!joins.Contains("Transportador"))
                joins.Append(" left join T_EMPRESA Transportador on Carga.EMP_CODIGO = Transportador.EMP_CODIGO ");
        }

        private void SetarJoinsCargaMonitoramento(StringBuilder joins)
        {
            if (!joins.Contains("Monitoramento"))
                joins.Append(" left join T_MONITORAMENTO Monitoramento on Carga.CAR_CODIGO = Monitoramento.CAR_CODIGO ");
        }

        private void SetarJoinsDadosSumarizados(StringBuilder joins)
        {
            if (!joins.Contains("CargaDadosSumarizados"))
                joins.Append(" left join T_CARGA_DADOS_SUMARIZADOS CargaDadosSumarizados on Carga.CDS_CODIGO = CargaDadosSumarizados.CDS_CODIGO ");
        }

        private void SetarJoinsFilial(StringBuilder joins)        
        {            
            if(!joins.Contains("Filial"))
                joins.Append(" LEFT JOIN T_FILIAL Filial ON Filial.FIL_CODIGO = Carga.FIL_CODIGO ");
        }

        private void SetarJoinsTipoDeOperacao(StringBuilder joins)
        {
            if (!joins.Contains("TipoOperacao"))
                joins.Append(" Left Join T_TIPO_OPERACAO TipoOperacao on TipoOperacao.TOP_CODIGO = Carga.TOP_CODIGO ");
        }
        #endregion

        #region Métodos Protegidos Sobrescritos

        protected override void SetarSelect(string propriedade, int codigoDinamico, StringBuilder select, StringBuilder joins, StringBuilder groupBy, bool somenteContarNumeroRegistros, Dominio.ObjetosDeValor.Embarcador.Carga.FiltroPesquisaRelatorioCargaViagemEventos filtrosPesquisa)
        {
            SetarJoinsCarga(joins);
            SetarJoinsDadosSumarizados(joins);
            SetarJoinsCargaMonitoramento(joins);
            SetarJoinsCargaEntrega(joins);
            switch (propriedade)
            {
                case "Codigo":
                    if (!select.Contains(" Codigo, "))
                    {
                        select.Append("Carga.CAR_CODIGO Codigo, ");
                        //groupBy.Append("Carga.CAR_CODIGO, ");                        
                    }
                    break;

                case "NomeFilial":
                    if (!select.Contains(" NomeFilial, "))
                    {
                        select.Append("Filial.FIL_DESCRICAO NomeFilial, ");
                        SetarSelect("CNPJFilial", 0, select, joins, groupBy, false, filtrosPesquisa);
                        //groupBy.Append("Filial.FIL_DESCRICAO, ");
                        SetarJoinsFilial(joins);
                    }
                    break;

                case "CNPJFilial":
                    if (!select.Contains(" CNPJFilial, "))
                    {
                        select.Append("Filial.FIL_CNPJ CNPJFilial, ");
                        //groupBy.Append("Filial.FIL_CNPJ, ");

                        SetarJoinsFilial(joins);
                    }
                    break;

                case "NomeClienteOrigem":
                    if (!select.Contains(" NomeClienteOrigem, "))
                    {
                        select.Append("CargaDadosSumarizados.CDS_REMETENTES NomeClienteOrigem, ");
                        //groupBy.Append("CargaDadosSumarizados.CDS_REMETENTES, ");
                        SetarJoinsDadosSumarizados(joins);
                    }
                    break;

                case "NomeClienteDestino":
                    if (!select.Contains(" NomeClienteDestino, "))
                    {
                        select.Append(@"SUBSTRING((SELECT DISTINCT ', ' + Cliente.CLI_NOME from T_CARGA_ENTREGA CargaEntrega 
                                        LEFT JOIN T_CLIENTE Cliente ON CargaEntrega.CLI_CODIGO_ENTREGA = Cliente.CLI_CGCCPF
                                        where CargaEntrega.CAR_CODIGO = Carga.CAR_CODIGO FOR XML PATH('')), 3, 1000) NomeClienteDestino, ");
                        //groupBy.Append("CargaDadosSumarizados.CDS_DESTINATARIOS, ");                        
                    }
                    break;

                case "Origem":
                    if (!select.Contains(" Origem, "))
                    {
                        select.Append("CargaDadosSumarizados.CDS_ORIGENS Origem, ");
                        //groupBy.Append("CargaDadosSumarizados.CDS_ORIGENS, ");                        
                    }
                    break;

                case "Destino":
                    if (!select.Contains(" Destino, "))
                    {
                        select.Append("CargaDadosSumarizados.CDS_DESTINOS Destino, ");
                        //groupBy.Append("CargaDadosSumarizados.CDS_DESTINOS, ");                        
                    }
                    break;

                case "NumeroCarga":
                    if (!select.Contains(" NumeroCarga, "))
                    {
                        select.Append("Carga.CAR_CODIGO_CARGA_EMBARCADOR NumeroCarga, ");
                        //groupBy.Append("Carga.CAR_CODIGO_CARGA_EMBARCADOR, ");                        
                    }
                    break;

                case "PlacaVeiculo":
                    if (!select.Contains(" PlacaVeiculo, "))
                    {
                        select.Append("CargaDadosSumarizados.CDS_VEICULOS PlacaVeiculo, ");                        
                        //groupBy.Append("CargaDadosSumarizados.CDS_VEICULOS, ");                        
                    }
                    break;

                case "TipoOperacao":
                    if (!select.Contains(" TipoOperacao, "))
                    {
                        select.Append("TipoOperacao.TOP_DESCRICAO TipoOperacao, ");
                        //groupBy.Append("TipoOperacao.TOP_DESCRICAO, ");
                        SetarJoinsTipoDeOperacao(joins);
                    }
                    break;

                case "NomeTransportador":
                    if (!select.Contains(" NomeTransportador, "))
                    {
                        select.Append("Transportador.EMP_RAZAO NomeTransportador, ");
                        //groupBy.Append("Transportador.EMP_RAZAO, ");

                        SetarJoinsCargaTransportador(joins);
                        SetarSelect("CNPJTransportador", 0, select, joins, groupBy, false, filtrosPesquisa);
                    }
                    break;

                case "CNPJTransportador":
                    if (!select.Contains(" CNPJTransportador, "))
                    {
                        select.Append("Transportador.EMP_CNPJ CNPJTransportador, ");
                        //groupBy.Append("Transportador.EMP_CNPJ, ");

                        SetarJoinsCargaTransportador(joins);
                    }
                    break;

                case "PrevisaoChegadaPlanta":
                    if (!select.Contains(" PrevisaoChegadaPlanta, "))
                    {
                        select.Append("CargaEntrega.CEN_DATA_FIM_PREVISTA PrevisaoChegadaPlanta, ");                        
                        //groupBy.Append("Entrega.CEN_DATA_FIM_PREVISTA, ");                        
                    }
                    break;

                case "DataCriacaoCarga":
                    if (!select.Contains(" DataCriacaoCarga, "))
                    {
                        select.Append("CargaEntrega.CEN_DATA_CRIACAO DataCriacaoCarga, ");                        
                        // groupBy.Append("Entrega.CEN_DATA_CRIACAO, ");

                    }
                    break;

                case "DataInicioViagem":
                case "DataInicioViagemFormatada":
                    if (!select.Contains(" DataInicioViagem, "))
                    {
                        select.Append("Carga.CAR_DATA_INICIO_VIAGEM DataInicioViagem, ");                        
                        ///groupBy.Append("Entrega.CEN_DATA_INICIO_ENTREGA, ");

                    }
                    break;

                case "DataChegadaCliente":
                    if (!select.Contains(" DataChegadaCliente, "))
                    {
                        select.Append("CargaEntrega.CEN_DATA_ENTRADA_RAIO DataChegadaCliente, ");
                        //groupBy.Append("Entrega.CEN_DATA_FIM_ENTREGA, ");

                    }
                    break;

                case "DataEncerramentoViagem":
                    if (!select.Contains(" DataEncerramentoViagem, "))
                    {
                        select.Append("CargaEntrega.CEN_DATA_ENTREGA DataEncerramentoViagem, ");
                        //groupBy.Append("Entrega.CEN_DATA_FIM_ENTREGA, ");

                    }
                    break;

                case "DataETA":
                    if (!select.Contains(" DataETA, "))
                    {
                        select.Append("EntregaEventos.CEE_DATA_PREVISAO_RECALCULADA DataETA, ");
                        //groupBy.Append("EntregaEventos.CEE_DATA_PREVISAO_RECALCULADA, ");

                    }
                    break;

                case "TipoTecnologiaDescricao":
                    if (!select.Contains(" TipoTecnologiaDescricao, "))
                    {
                        select.Append("(select TOP 1 UltimaPosicao.POS_RASTREADOR from T_MONITORAMENTO MonitUltimaPosicao JOIN T_POSICAO UltimaPosicao on UltimaPosicao.POS_CODIGO = MonitUltimaPosicao.POS_ULTIMA_POSICAO where Carga.CAR_CODIGO = MonitUltimaPosicao.CAR_CODIGO order by MonitUltimaPosicao.MON_DATA_CRIACAO desc) UltimaPosicaoRastreador, "); 
 
                        //if (!groupBy.Contains("Carga.CAR_CODIGO,"))
                        //    groupBy.Append("Carga.CAR_CODIGO, ");                        
                    }
                    break;

                case "SituacaoMonitoramento":
                    if (!select.Contains(" SituacaoMonitoramento, "))
                    {
                        select.Append("(select TOP 1 MonitStatus.MON_STATUS from T_MONITORAMENTO MonitStatus where Carga.CAR_CODIGO = MonitStatus.CAR_CODIGO order by MonitStatus.MON_DATA_CRIACAO desc) UltimaPosicaoStatus, ");
 
                        //if (!groupBy.Contains("Carga.CAR_CODIGO,"))
                        //    groupBy.Append("Carga.CAR_CODIGO, ");                        
                    }
                    break;

                case "OperadorMonitoramento":
                    if (!select.Contains(" OperadorMonitoramento, "))
                    {
                        select.Append("Carga.CAR_ANALISTA_RESPONSAVEL_MONITORAMENTO OperadorMonitoramento, ");
                        //groupBy.Append("Carga.CAR_ANALISTA_RESPONSAVEL_MONITORAMENTO, ");                        
                    }
                    break;

                case "LatitudeFormatada":
                    if (!select.Contains(" Latitude, "))
                    {
                        select.Append("EntregaEventos.CEE_LATITUDE Latitude, ");
                        //groupBy.Append("EntregaEventos.CEE_LATITUDE, ");                        
                    }
                    break;

                case "LongitudeFormatada":
                    if (!select.Contains(" Longitude, "))
                    {
                        select.Append("EntregaEventos.CEE_LONGITUDE Longitude, ");
                        //groupBy.Append("EntregaEventos.CEE_LONGITUDE, ");                        
                    }
                    break;

                case "NomeMotoristas":
                    if (!select.Contains(" NomeMotoristas, "))
                    {
                        select.Append("SUBSTRING((SELECT ', ' + motorista1.FUN_NOME + (CASE WHEN motorista1.FUN_FONE is null or motorista1.FUN_FONE = '' THEN '' ELSE ' (' + motorista1.FUN_FONE  + ')' END) FROM T_CARGA_MOTORISTA motoristaCarga1 INNER JOIN T_FUNCIONARIO motorista1 ON motoristaCarga1.CAR_MOTORISTA = motorista1.FUN_CODIGO WHERE motoristaCarga1.CAR_CODIGO = CAR_CODIGO FOR XML PATH('')), 3, 1000) NomeMotoristas, ");
                        //groupBy.Append("EntregaEventos.CEE_LONGITUDE, ");                        
                    }
                    break;
            }
        }

        protected override void SetarWhere(Dominio.ObjetosDeValor.Embarcador.Carga.FiltroPesquisaRelatorioCargaViagemEventos filtrosPesquisa, StringBuilder where, StringBuilder joins, StringBuilder groupBy, List<Embarcador.Consulta.ParametroSQL> parametros = null)
        {
            if (filtrosPesquisa.CodigoCargaEmbarcador.Count > 0)
            {
                where.Append($" AND Carga.CAR_CODIGO in ( {string.Join(",", filtrosPesquisa.CodigoCargaEmbarcador)} )");
            }

            if (filtrosPesquisa.CodigoFilial > 0)
            {
                where.Append($" AND Carga.FIL_CODIGO = '{filtrosPesquisa.CodigoFilial}' ");
            }

            if (filtrosPesquisa.CodigoClienteOrigem > 0)
            {                
                where.Append($" AND CargaDadosSumarizadosRemetentes.CLI_CGCCPF = {filtrosPesquisa.CodigoClienteOrigem}");
                SetarJoinsDadosSumarizadosRemetentes(joins);
            }

            if (filtrosPesquisa.CodigoClienteDestino > 0)
            {
                where.Append($" AND CargaDadosSumarizadosDestinatarios.CLI_CGCCPF = {filtrosPesquisa.CodigoClienteDestino} ");
                SetarJoinsDadosSumarizadosDestinatarios(joins);
            }

            if (filtrosPesquisa.CodigoLocalidadeOrigem > 0)
            {
                where.Append($" AND {filtrosPesquisa.CodigoLocalidadeOrigem} = CargaDadosSumarizadosRemetentesLocalidadesCliente.LOC_CODIGO");
                SetarJoinsDadosSumarizadosRemetentes(joins);
                SetarJoinsDadosSumarizadosRemetentesLocalidades(joins);
            }

            if (filtrosPesquisa.CodigoLocalidadeDestino > 0)
            {
                where.Append($" AND {filtrosPesquisa.CodigoLocalidadeDestino} = CargaDadosSumarizadosDestinatariosLocalidadesCliente.LOC_CODIGO");
                SetarJoinsDadosSumarizadosDestinatarios(joins);
                SetarJoinsDadosSumarizadosDestinatariosLocalidades(joins);
            }
        }

        #endregion
    }
}
