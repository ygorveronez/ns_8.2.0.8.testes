using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Repositorio.Embarcador.Logistica
{
    sealed class ConsultaJanelaCarregamentoIntegracao : Repositorio.Embarcador.Consulta.Consulta<Dominio.ObjetosDeValor.Embarcador.Logistica.FiltroPesquisaRelatorioJanelaCarregamentoIntegracao>
    {
        #region Construtores

        public ConsultaJanelaCarregamentoIntegracao() : base(tabela: "T_CARGA_JANELA_CARREGAMENTO_INTEGRACAO as CargaJanelaCarregamentoIntegracao") { }

        #endregion

        #region Métodos Privados

        private void SetarJoinsCargaJanelaCarregamento(StringBuilder joins)
        {
            if (!joins.Contains(" CargaJanelaCarregamento "))
                joins.Append(" left join T_CARGA_JANELA_CARREGAMENTO CargaJanelaCarregamento on CargaJanelaCarregamento.CJC_CODIGO = CargaJanelaCarregamentoIntegracao.CJC_CODIGO ");
        }
        private void SetarJoinsCarga(StringBuilder joins)
        {
            SetarJoinsCargaJanelaCarregamento(joins);
            if (!joins.Contains(" Carga "))
                joins.Append(" left join T_CARGA Carga on Carga.CAR_CODIGO = CargaJanelaCarregamento.CAR_CODIGO ");
        }

        private void SetarJoinsDadosSumarizados(StringBuilder joins)
        {
            SetarJoinsCarga(joins);
            if (!joins.Contains(" DadosSumarizados "))
                joins.Append(" left join T_CARGA_DADOS_SUMARIZADOS DadosSumarizados on Carga.CDS_CODIGO = DadosSumarizados.CDS_CODIGO ");
        }

        #endregion

        protected override void SetarSelect(string propriedade, int codigoDinamico, StringBuilder select, StringBuilder joins, StringBuilder groupBy, bool somenteContarNumeroRegistros, Dominio.ObjetosDeValor.Embarcador.Logistica.FiltroPesquisaRelatorioJanelaCarregamentoIntegracao filtroPesquisa)
        {
            switch (propriedade)
            {
                case "Codigo":
                    if (!select.Contains(" Codigo,"))
                    {
                        select.Append("CargaJanelaCarregamentoIntegracao.JCI_CODIGO as Codigo, ");
                        groupBy.Append("CargaJanelaCarregamentoIntegracao.JCI_CODIGO, ");
                    }
                    break;

                case "ExecutouRotaTresFormatado":
                    if (!select.Contains(" ExecutouRotaTres,"))
                    {
                        select.Append("CargaJanelaCarregamentoIntegracao.JCI_NOVA_ANALISE as ExecutouRotaTres, ");
                        groupBy.Append("CargaJanelaCarregamentoIntegracao.JCI_NOVA_ANALISE, ");
                    }
                    break;

                case "Carga":
                    if (!select.Contains(" Carga,"))
                    {
                        select.Append("Carga.CAR_CODIGO_CARGA_EMBARCADOR Carga, ");
                        groupBy.Append("Carga.CAR_CODIGO_CARGA_EMBARCADOR, ");

                        SetarJoinsCarga(joins);
                    }
                    break;

                case "DataCriacaoFormatada":
                    if (!select.Contains(" DataCriacao,"))
                    {
                        select.Append("CargaJanelaCarregamentoIntegracao.JCI_DATA_CRIACAO DataCriacao,  ");
                        groupBy.Append("CargaJanelaCarregamentoIntegracao.JCI_DATA_CRIACAO, ");
                    }
                    break;

                case "Veiculos":
                    if (!select.Contains(" Veiculos,"))
                    {
                        select.Append(@"(select CONCAT((select VEI_PLACA from T_VEICULO where VEI_CODIGO = Carga.CAR_VEICULO),
                                    ISNULL(', ' + SUBSTRING((SELECT ', ' + veiculo1.VEI_PLACA FROM T_CARGA_VEICULOS_VINCULADOS veiculoVinculadoCarga1 
                                    LEFT JOIN T_VEICULO veiculo1 ON veiculoVinculadoCarga1.VEI_CODIGO = veiculo1.VEI_CODIGO 
                                    WHERE veiculoVinculadoCarga1.CAR_CODIGO = Carga.CAR_CODIGO FOR XML PATH('')), 3, 1000), ''))) Veiculos, ");
                        groupBy.Append("Carga.CAR_VEICULO, ");

                        SetarJoinsCarga(joins);
                    }
                    break;

                case "Protocolo":
                    if (!select.Contains(" Protocolo,"))
                    {
                        select.Append("CargaJanelaCarregamentoIntegracao.JCI_PROTOCOLO Protocolo,  ");
                        groupBy.Append("CargaJanelaCarregamentoIntegracao.JCI_PROTOCOLO, ");

                        SetarJoinsDadosSumarizados(joins);
                    }
                    break;

                case "Motorista":
                    if (!select.Contains(" Motorista,"))
                    {
                        select.Append(@"SUBSTRING(ISNULL((SELECT DISTINCT ', ' + mot.FUN_NOME FROM T_CARGA_MOTORISTA cargaMotorista 
                                        inner join T_FUNCIONARIO mot ON mot.FUN_CODIGO = cargaMotorista.CAR_MOTORISTA 
                                        WHERE cargaMotorista.CAR_CODIGO = Carga.CAR_CODIGO FOR XML PATH('')), ''), 3, 1000) Motorista,  ");
                        groupBy.Append("Carga.CAR_CODIGO, ");
                        SetarJoinsCarga(joins);
                    }
                    break;
                case "CPFMotorista":
                case "CPFMotoristaFormatado":
                    if (!select.Contains(" CPFMotorista,"))
                    {
                        select.Append(@"SUBSTRING(ISNULL((SELECT DISTINCT ', ' + mot.FUN_CPF FROM T_CARGA_MOTORISTA cargaMotorista 
                                        inner join T_FUNCIONARIO mot ON mot.FUN_CODIGO = cargaMotorista.CAR_MOTORISTA 
                                        WHERE cargaMotorista.CAR_CODIGO = Carga.CAR_CODIGO FOR XML PATH('')), ''), 3, 1000) CPFMotorista,  ");
                        groupBy.Append("Carga.CAR_CODIGO, ");
                        SetarJoinsCarga(joins);
                    }
                    break;
            }
        }

        protected override void SetarWhere(Dominio.ObjetosDeValor.Embarcador.Logistica.FiltroPesquisaRelatorioJanelaCarregamentoIntegracao filtrosPesquisa, StringBuilder where, StringBuilder joins, StringBuilder groupBy, List<Embarcador.Consulta.ParametroSQL> parametros = null)
        {
            string pattern = "yyyy-MM-dd";

            if (filtrosPesquisa.DataInicial != DateTime.MinValue)
                where.Append($" and CargaJanelaCarregamentoIntegracao.JCI_DATA_CRIACAO >= '{filtrosPesquisa.DataInicial.ToString(pattern)}'");

            if (filtrosPesquisa.DataFinal != DateTime.MinValue)
                where.Append($" and CargaJanelaCarregamentoIntegracao.JCI_DATA_CRIACAO <= '{filtrosPesquisa.DataFinal.AddDays(1).ToString(pattern)}'");

            if (filtrosPesquisa.CodigoCarga > 0)
            {
                where.Append($" and Carga.CAR_CODIGO LIKE '{filtrosPesquisa.CodigoCarga}'");
                SetarJoinsCarga(joins);
            }



        }

    }

}
