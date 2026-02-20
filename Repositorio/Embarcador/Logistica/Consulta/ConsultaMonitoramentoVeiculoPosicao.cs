using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Repositorio.Embarcador.Logistica.Consulta
{
    sealed class ConsultaMonitoramentoVeiculoPosicao : Repositorio.Embarcador.Consulta.Consulta<Dominio.ObjetosDeValor.Embarcador.Logistica.FiltroPesquisaMonitoramentoVeiculoPosicao>
    {
        #region Construtores

        public ConsultaMonitoramentoVeiculoPosicao() : base(tabela: "T_POSICAO as Posicao") { }

        #endregion

        #region Métodos Privados 

        private void SetarJoinsMonitoramentoVeiculoPosicao(StringBuilder joins)
        {
            if (!joins.Contains(" MonitoramentoVeiculoPosicao "))
                joins.Append("join T_MONITORAMENTO_VEICULO_POSICAO MonitoramentoVeiculoPosicao on MonitoramentoVeiculoPosicao.POS_CODIGO = Posicao.POS_CODIGO ");
        }
        private void SetarJoinsMonitoramentoVeiculo(StringBuilder joins)
        {
            if (!joins.Contains(" MonitoramentoVeiculo "))
                joins.Append("join T_MONITORAMENTO_VEICULO MonitoramentoVeiculo on MonitoramentoVeiculo.MOV_CODIGO = MonitoramentoVeiculoPosicao.MOV_CODIGO ");
        }

        private void SetarJoinsVeiculo(StringBuilder joins)
        {
            if (!joins.Contains(" Veiculo "))
                joins.Append("join T_VEICULO Veiculo on Veiculo.VEI_CODIGO = MonitoramentoVeiculo.VEI_CODIGO ");
        }


        #endregion

        #region Métodos Protegidos Sobrescritos

        protected override void SetarSelect(string propriedade, int codigoDinamico, StringBuilder select, StringBuilder joins, StringBuilder groupBy, bool somenteContarNumeroRegistros, Dominio.ObjetosDeValor.Embarcador.Logistica.FiltroPesquisaMonitoramentoVeiculoPosicao filtroPesquisa)
        {
            
            switch (propriedade)
            {
                case "Codigo":
                    if (!select.Contains(" Codigo, "))
                    {
                        select.Append("Posicao.POS_CODIGO as Codigo, ");
                        groupBy.Append("Posicao.POS_CODIGO, ");
                    }
                    break;

                case "DataVeiculoFormatada":
                    if (!select.Contains(" DataVeiculo, "))
                    {
                        select.Append("Posicao.POS_DATA_VEICULO as DataVeiculo, ");
                        groupBy.Append("Posicao.POS_DATA_VEICULO, ");

                    }
                    break;

                case "DataCadastroFormatada":
                    if (!select.Contains(" DataCadastro, "))
                    {
                        select.Append("Posicao.POS_DATA_CADASTRO as DataCadastro, ");
                        groupBy.Append("Posicao.POS_DATA_CADASTRO, ");

                    }
                    break;

                case "PlacaVeiculo":
                    if (!select.Contains(" PlacaVeiculo, "))
                    {
                        select.Append("Veiculo.VEI_PLACA as PlacaVeiculo, ");
                        groupBy.Append("Veiculo.VEI_PLACA, ");

                    }
                    break;

                case "Descricao":
                    if (!select.Contains(" Descricao, "))
                    {
                        select.Append("Posicao.POS_DESCRICAO as Descricao, ");
                        groupBy.Append("Posicao.POS_DESCRICAO, ");

                    }
                    break;
                case "Latitude":
                    if (!select.Contains(" Latitude, "))
                    {
                        select.Append("Posicao.POS_LATITUDE as Latitude, ");
                        groupBy.Append("Posicao.POS_LATITUDE, ");

                    }
                    break;
                case "Longitude":
                    if (!select.Contains(" Longitude, "))
                    {
                        select.Append("Posicao.POS_LONGITUDE as Longitude, ");
                        groupBy.Append("Posicao.POS_LONGITUDE, ");

                    }
                    break;
                case "Ignicao":
                    if (!select.Contains(" Ignicao, "))
                    {
                        select.Append("CASE WHEN Posicao.POS_IGNICAO > 0 THEN 'Ligado' ELSE 'Desligado' END AS Ignicao , ");
                        groupBy.Append("Posicao.POS_IGNICAO, ");

                    }
                    break;
                case "VelocidadeDescricao":
                    if (!select.Contains(" VelocidadeDescricao, "))
                    {
                        select.Append("CONCAT(Posicao.POS_VELOCIDADE,' Km/h') AS VelocidadeDescricao , ");
                        groupBy.Append("Posicao.POS_VELOCIDADE, ");

                    }
                    break;
                case "Temperatura":
                    if (!select.Contains(" TemperaturaDescricao, "))
                    {
                        select.Append("Posicao.POS_TEMPERATURA as TemperaturaDescricao, ");
                        groupBy.Append("Posicao.POS_TEMPERATURA, ");

                    }
                    break;
            }
        }

        protected override void SetarWhere(Dominio.ObjetosDeValor.Embarcador.Logistica.FiltroPesquisaMonitoramentoVeiculoPosicao filtrosPesquisa, StringBuilder where, StringBuilder joins, StringBuilder groupBy, List<Embarcador.Consulta.ParametroSQL> parametros = null)
        {
            SetarJoinsMonitoramentoVeiculoPosicao(joins);
            SetarJoinsMonitoramentoVeiculo(joins);
            SetarJoinsVeiculo(joins);

            if (filtrosPesquisa.PlacaVeiculo != "")
                where.Append($" and Veiculo.VEI_PLACA like '%{filtrosPesquisa.PlacaVeiculo}%'");

            if (filtrosPesquisa.DataInicial > DateTime.MinValue)
                where.Append($" and Posicao.POS_DATA_VEICULO >= '{filtrosPesquisa.DataInicial.Value.ToString("yyyyMMdd HH:mm:ss")}'");

            if (filtrosPesquisa.DataFinal > DateTime.MinValue)
                where.Append($" and Posicao.POS_DATA_VEICULO <= '{filtrosPesquisa.DataFinal.Value.ToString("yyyyMMdd HH:mm:ss")}'");
        }

        #endregion
    }
}

