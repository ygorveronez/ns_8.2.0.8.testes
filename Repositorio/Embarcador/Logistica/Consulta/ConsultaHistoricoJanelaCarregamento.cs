using Dominio.ObjetosDeValor.Embarcador.Enumeradores;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Repositorio.Embarcador.Logistica
{
    sealed class ConsultaHistoricoJanelaCarregamento : Repositorio.Embarcador.Consulta.Consulta<Dominio.ObjetosDeValor.Embarcador.Logistica.FiltroPesquisaHistoricoJanelaCarregamento>
    {
        #region Construtores

        public ConsultaHistoricoJanelaCarregamento() : base(tabela: "T_CARGA_JANELA_CARREGAMENTO_TRANSPORTADOR as CargaJanelaCarregamentoTranportador") { }

        #endregion

        #region Métodos Privados

        private void SetarJoinsCargaJanelaCarregamento(StringBuilder joins)
        {
            if (!joins.Contains(" CargaJanelaCarregamento "))
                joins.Append(" JOIN T_CARGA_JANELA_CARREGAMENTO AS CargaJanelaCarregamento ON CargaJanelaCarregamento.CJC_CODIGO = CargaJanelaCarregamentoTranportador.CJC_CODIGO");
        }

        private void SetarJoinsCarga(StringBuilder joins)
        {
            SetarJoinsCargaJanelaCarregamento(joins);

            if (!joins.Contains(" Carga "))
                joins.Append(" JOIN T_CARGA AS Carga ON Carga.CAR_CODIGO = CargaJanelaCarregamento.CAR_CODIGO ");
        }


        private void SetarJoinsCentroCarregamento(StringBuilder joins)
        {
            SetarJoinsCargaJanelaCarregamento(joins);

            if (!joins.Contains(" CentroCarregamento "))
                joins.Append(" JOIN T_CENTRO_CARREGAMENTO AS CentroCarregamento ON CentroCarregamento.CEC_CODIGO = CargaJanelaCarregamento.CEC_CODIGO ");

        }

        private void SetarJoinsModeloVeicular(StringBuilder joins)
        {
            SetarJoinsCarga(joins);

            if (!joins.Contains(" Veiculo "))
                joins.Append(" LEFT JOIN T_MODELO_VEICULAR_CARGA AS ModeloVeicularCarga ON ModeloVeicularCarga.MVC_CODIGO = Carga.MVC_CODIGO ");

        }

        private void SetarJoinsFilaCarregamentoMotivoRetirada(StringBuilder joins)
        {
            if (!joins.Contains(" FilaCarregametnoMotivoRetirada "))
                joins.Append(" LEFT JOIN T_FILA_CARREGAMENTO_MOTIVO_RETIRADA AS FilaCarregametnoMotivoRetirada ON FilaCarregametnoMotivoRetirada.FMR_CODIGO = CargaJanelaCarregamentoHistorico.FMR_CODIGO");


        }
        private void SetarJoinsHistorico(StringBuilder joins)
        {
            if (!joins.Contains(" CargaJanelaCarregamentoHistorico "))
                joins.Append(" JOIN T_CARGA_JANELA_CARREGAMENTO_TRANSPORTADOR_HISTORICO AS CargaJanelaCarregamentoHistorico ON CargaJanelaCarregamentoHistorico.JCT_CODIGO = CargaJanelaCarregamentoTranportador.JCT_CODIGO AND CargaJanelaCarregamentoHistorico.JTH_TIPO = 3");


        }
        #endregion

        #region Métodos Protegidos Sobrescritos

        protected override void SetarSelect(string propriedade, int codigoDinamico, StringBuilder select, StringBuilder joins, StringBuilder groupBy, bool somenteContarNumeroRegistros, Dominio.ObjetosDeValor.Embarcador.Logistica.FiltroPesquisaHistoricoJanelaCarregamento filtroPesquisa)
        {
            switch (propriedade)
            {
                case "DataRecusa":
                case "DataRecusaFormatada":
                    if (!select.Contains(" DataRecusa, "))
                    {
                        select.Append(" CargaJanelaCarregamentoHistorico.JTH_DATA AS DataRecusa, ");
                        groupBy.Append(" CargaJanelaCarregamentoHistorico.JTH_DATA, ");

                        SetarJoinsHistorico(joins);
                    }
                    break;
                case "DescricaoVeiculo":
                    if (!select.Contains(" DescricaoVeiculo, "))
                    {
                        select.Append(" ModeloVeicularCarga.MVC_DESCRICAO AS DescricaoVeiculo, ");
                        groupBy.Append(" ModeloVeicularCarga.MVC_DESCRICAO , ");

                        SetarJoinsModeloVeicular(joins);
                    }
                    break;
                case "Descricao":
                    if (!select.Contains(" Descricao, "))
                    {
                        select.Append(" CargaJanelaCarregamentoHistorico.JTH_DESCRICAO AS Descricao, ");
                        groupBy.Append(" CargaJanelaCarregamentoHistorico.JTH_DESCRICAO , ");

                        SetarJoinsHistorico(joins);
                    }
                    break;
                case "DescricaoCarga":
                    if (!select.Contains(" DescricaoCarga, "))
                    {
                        select.Append(" Carga.CAR_CODIGO_CARGA_EMBARCADOR AS DescricaoCarga, ");
                        groupBy.Append(" Carga.CAR_CODIGO_CARGA_EMBARCADOR, ");

                        SetarJoinsCarga(joins);
                    }
                    break;
                case "DescricaoCentroCarregamento":
                    if (!select.Contains(" DescricaoCentroCarregamento, "))
                    {
                        select.Append(" CentroCarregamento.CEC_DESCRICAO AS DescricaoCentroCarregamento, ");
                        groupBy.Append(" CentroCarregamento.CEC_DESCRICAO, ");

                        SetarJoinsCentroCarregamento(joins);
                    }
                    break;
                case "DescricaoMotivoRecusa":
                    if (!select.Contains(" DescricaoMotivoRecusa, "))
                    {
                        select.Append(" FilaCarregametnoMotivoRetirada.FMR_DESCRICAO AS DescricaoMotivoRecusa, ");
                        groupBy.Append(" FilaCarregametnoMotivoRetirada.FMR_DESCRICAO, ");

                        SetarJoinsFilaCarregamentoMotivoRetirada(joins);
                    }
                    break;

                case "JustificativaRecusa":
                    if (!select.Contains(" JustificativaRecusa, "))
                    {
                        select.Append(" CargaJanelaCarregamentoHistorico.JTH_JUSTIFICATIVA AS JustificativaRecusa, ");
                        groupBy.Append(" CargaJanelaCarregamentoHistorico.JTH_JUSTIFICATIVA, ");

                    }
                    break;
            }
        }

        protected override void SetarWhere(Dominio.ObjetosDeValor.Embarcador.Logistica.FiltroPesquisaHistoricoJanelaCarregamento filtrosPesquisa, StringBuilder where, StringBuilder joins, StringBuilder groupBy, List<Embarcador.Consulta.ParametroSQL> parametros = null)
        {

            string datePattern = "yyyy-MM-dd";

            if (filtrosPesquisa.CodigoCarga > 0)
                where.Append($" AND Carga.CAR_CODIGO =  {filtrosPesquisa.CodigoCarga}");

            if (filtrosPesquisa.CodigoMotivoRecusa > 0)
                where.Append($" AND CargaJanelaCarregamentoTranportador.FMR_CODIGO =  {filtrosPesquisa.CodigoMotivoRecusa}");

            if (filtrosPesquisa.CodigoCentroCarregamento > 0)
                where.Append($" AND CargaJanelaCarregamento.CEC_CODIGO =  {filtrosPesquisa.CodigoCentroCarregamento}");

            if (filtrosPesquisa.DataInicial.HasValue)
                where.Append($" AND CAST(CargaJanelaCarregamentoHistorico.JTH_DATA AS DATE) >= '{filtrosPesquisa.DataInicial.Value.ToString(datePattern)}'");

            if (filtrosPesquisa.DataFinal.HasValue)
                where.Append($" AND CAST(CargaJanelaCarregamentoHistorico.JTH_DATA AS DATE) <= '{filtrosPesquisa.DataFinal.Value.ToString(datePattern)}'");

            if (filtrosPesquisa.CodigoClienteTerceiro > 0)
                where.Append($" AND CargaJanelaCarregamentoTranportador.CLI_CGCCPF_TERCEIRO  = {(long)filtrosPesquisa.CodigoClienteTerceiro}");


        }

        #endregion
    }
}
