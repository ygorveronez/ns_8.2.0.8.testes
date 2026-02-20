using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Repositorio.Embarcador.Logistica.Consulta
{
    sealed class ConsultaMonitoramentoPosicaoDaFrota : Repositorio.Embarcador.Consulta.Consulta<Dominio.ObjetosDeValor.Embarcador.Logistica.FiltroPesquisaMonitoramentoPosicaoDaFrota>
    {
        #region Construtores

        public ConsultaMonitoramentoPosicaoDaFrota() : base(tabela: "T_POSICAO_ATUAL as PosicaoAtual") { }

        #endregion

        #region Métodos Privados 

        private void SetarJoinsVeiculo(StringBuilder joins)
        {
            if (!joins.Contains(" Veiculo "))
                joins.Append("join T_VEICULO Veiculo on Veiculo.VEI_CODIGO = PosicaoAtual.VEI_CODIGO ");
        }


        private void SetarJoinModeloVeicular(StringBuilder joins)
        {
            if (!joins.Contains(" ModeloVeicular "))
                joins.Append("left join T_MODELO_VEICULAR_CARGA ModeloVeicular on Veiculo.MVC_CODIGO = ModeloVeicular.MVC_CODIGO ");
        }

        private void SetarJoinModeloCarroceria(StringBuilder joins)
        {
            if (!joins.Contains(" ModeloCarroceria "))
                joins.Append("left join T_MODELO_CARROCERIA ModeloCarroceria on ModeloCarroceria.MCA_CODIGO = Veiculo.MCA_CODIGO ");
        }

        private void SetarJoinOrigem(StringBuilder joins)
        {
            if (!joins.Contains(" Origem "))
                joins.Append("left join T_CLIENTE Origem on Origem.CLI_CGCCPF = Rota.CLI_CGCCPF ");
        }


        private void SetarJoinEmpresa(StringBuilder joins)
        {
            if (!joins.Contains(" Empresa "))
                joins.Append("left join T_EMPRESA Empresa on Veiculo.EMP_CODIGO = Empresa.EMP_CODIGO ");
        }



        private void SetarJoinCarga(StringBuilder joins)
        {
            if (!joins.Contains(" Carga "))
                joins.Append(@"left join T_CARGA Carga on PosicaoAtual.VEI_CODIGO = Carga.CAR_VEICULO and
                               Exists(select top 1  * From T_MONITORAMENTO Monitoramento 
                                where Monitoramento.CAR_CODIGO = Carga.CAR_CODIGO and Monitoramento.MON_STATUS = 1)"); // MON_STATUS = 1 Iniciada
        }
        private void SetarJoinTipoOperacao(StringBuilder joins)
        {
            if (!joins.Contains(" TipoOperacao "))
                joins.Append("left join T_TIPO_OPERACAO TipoOperacao on TipoOperacao.TOP_CODIGO = Carga.TOP_CODIGO ");
        }

        #endregion

        #region Métodos Protegidos Sobrescritos

        protected override void SetarSelect(string propriedade, int codigoDinamico, StringBuilder select, StringBuilder joins, StringBuilder groupBy, bool somenteContarNumeroRegistros, Dominio.ObjetosDeValor.Embarcador.Logistica.FiltroPesquisaMonitoramentoPosicaoDaFrota filtroPesquisa)
        {
            switch (propriedade)
            {
                case "Codigo":
                    if (!select.Contains(" Codigo, "))
                    {
                        select.Append("PosicaoAtual.POA_CODIGO as Codigo, ");
                        groupBy.Append("PosicaoAtual.POA_CODIGO, ");

                        SetarJoinsVeiculo(joins);
                    }
                    break;

                case "PlacaVeiculo":
                    if (!select.Contains(" PlacaVeiculo, "))
                    {
                        select.Append("Veiculo.VEI_PLACA as PlacaVeiculo, ");
                        groupBy.Append("Veiculo.VEI_PLACA, ");

                        SetarJoinsVeiculo(joins);
                    }
                    break;

                case "TipoVeiculo":
                    if (!select.Contains(" TipoVeiculo, "))
                    {
                        select.Append("ModeloVeicular.MVC_DESCRICAO as TipoVeiculo, ");
                        groupBy.Append("ModeloVeicular.MVC_DESCRICAO, ");

                        SetarJoinModeloVeicular(joins);
                    }
                    break;

                case "TipoBau":
                    if (!select.Contains(" TipoBau, "))
                    {
                        select.Append("ModeloCarroceria.MCA_DESCRICAO as TipoBau, ");
                        groupBy.Append("ModeloCarroceria.MCA_DESCRICAO, ");

                        SetarJoinModeloCarroceria(joins);
                    }
                    break;

                case "Posicao":
                    if (!select.Contains(" Posicao, "))
                    {
                        select.Append("PosicaoAtual.POA_DESCRICAO as Posicao, ");
                        groupBy.Append("PosicaoAtual.POA_DESCRICAO, ");
                    }
                    break;

                case "LatLng":
                    if (!select.Contains(" LatLng, "))
                    {
                        select.Append("PosicaoAtual.POA_LATITUDE as Latitude, ");
                        groupBy.Append("PosicaoAtual.POA_LATITUDE, ");

                        select.Append("PosicaoAtual.POA_LONGITUDE as Longitude, ");
                        groupBy.Append("PosicaoAtual.POA_LONGITUDE, ");
                    }
                    break;

                case "StatusDescricao":
                    if (!select.Contains(" Status, "))
                    {
                        select.Append("PosicaoAtual.POA_STATUS as Status, ");
                        groupBy.Append("PosicaoAtual.POA_STATUS, ");
                    }
                    break;

                case "Regiao":
                    if (!select.Contains(" Regiao, "))
                    {
                        //verificar
                    }
                    break;

                case "TipoDeTransporte":
                    if (!select.Contains(" TipoDeTransporte, "))
                    {
                        select.Append("TipoOperacao.TOP_DESCRICAO as TipoDeTransporte, ");
                        groupBy.Append("TipoOperacao.TOP_DESCRICAO, ");

                        SetarJoinCarga(joins);
                        SetarJoinTipoOperacao(joins);
                    }
                    break;

                case "Transportador":
                    if (!select.Contains(" Transportador, "))
                    {
                        select.Append("Empresa.EMP_RAZAO as Transportador, ");
                        groupBy.Append("Empresa.EMP_RAZAO, ");

                        SetarJoinsVeiculo(joins);
                        SetarJoinEmpresa(joins);
                    }
                    break;
            }

        }

        protected override void SetarWhere(Dominio.ObjetosDeValor.Embarcador.Logistica.FiltroPesquisaMonitoramentoPosicaoDaFrota filtrosPesquisa, StringBuilder where, StringBuilder joins, StringBuilder groupBy, List<Embarcador.Consulta.ParametroSQL> parametros = null)
        {

            if (filtrosPesquisa.PlacaVeiculo != "")
            {
                where.Append($" and Veiculo.VEI_PLACA like %'{filtrosPesquisa.PlacaVeiculo}'%");

                SetarJoinsVeiculo(joins);
            }

        }



        #endregion
    }
}

