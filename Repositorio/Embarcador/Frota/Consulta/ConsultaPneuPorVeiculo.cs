using Dominio.ObjetosDeValor.Embarcador.Frota;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Repositorio.Embarcador.Frota
{
    sealed class ConsultaPneuPorVeiculo : Consulta.Consulta<Dominio.ObjetosDeValor.Embarcador.Frota.FiltroPesquisaRelatorioPneuPorVeiculo>
    {
        #region Construtores

        public ConsultaPneuPorVeiculo() : base(tabela: "T_VEICULO Veiculo") { }

        #endregion

        #region Métodos Privados

        private void SetarJoinsModeloVeicular(StringBuilder joins)
        {
            if (!joins.Contains(" ModeloVeicular "))
                joins.Append(" JOIN T_MODELO_VEICULAR_CARGA ModeloVeicular on ModeloVeicular.MVC_CODIGO = Veiculo.MVC_CODIGO ");
        }

        private void SetarJoinsModeloEixo(StringBuilder joins)
        {
            SetarJoinsModeloVeicular(joins);

            if (!joins.Contains(" ModeloEixo "))
                joins.Append(" JOIN T_MODELO_VEICULAR_CARGA_EIXO ModeloEixo on ModeloEixo.MVC_CODIGO = ModeloVeicular.MVC_CODIGO ");
        }

        private void SetarJoinsModeloEixoPneu(StringBuilder joins)
        {
            SetarJoinsModeloEixo(joins);

            if (!joins.Contains(" ModeloEixoPneu "))
                joins.Append(" JOIN T_MODELO_VEICULAR_CARGA_EIXO_PNEU ModeloEixoPneu on ModeloEixoPneu.MEX_CODIGO = ModeloEixo.MEX_CODIGO ");
        }

        private void SetarJoinsVeiculoPneuModelo(StringBuilder joins)
        {
            SetarJoinsModeloEixoPneu(joins);

            if (!joins.Contains(" VeiculoPneu "))
                joins.Append(" LEFT OUTER JOIN T_VEICULO_PNEU VeiculoPneu on VeiculoPneu.VEI_CODIGO = Veiculo.VEI_CODIGO and VeiculoPneu.MEP_CODIGO = ModeloEixoPneu.MEP_CODIGO ");
        }

        private void SetarJoinsPneu(StringBuilder joins)
        {
            SetarJoinsVeiculoPneuModelo(joins);

            if (!joins.Contains(" Pneu "))
                joins.Append(" LEFT OUTER JOIN T_TMS_PNEU Pneu on Pneu.PNU_CODIGO = VeiculoPneu.PNU_CODIGO ");
        }

        private void SetarJoinsModeloPneu(StringBuilder joins)
        {
            SetarJoinsPneu(joins);

            if (!joins.Contains(" ModeloPneu "))
                joins.Append(" LEFT OUTER JOIN T_TMS_PNEU_MODELO ModeloPneu on ModeloPneu.PML_CODIGO = Pneu.PML_CODIGO ");
        }

        private void SetarJoinsMarcaPneu(StringBuilder joins)
        {
            SetarJoinsModeloPneu(joins);

            if (!joins.Contains(" MarcaPneu "))
                joins.Append(" LEFT OUTER JOIN T_TMS_PNEU_MARCA MarcaPneu on MarcaPneu.PMR_CODIGO = ModeloPneu.PMR_CODIGO ");
        }

        private void SetarJoinsBandaRodagemPneu(StringBuilder joins)
        {
            SetarJoinsPneu(joins);

            if (!joins.Contains(" BandaRodagem "))
                joins.Append(" LEFT OUTER JOIN T_TMS_PNEU_BANDA_RODAGEM BandaRodagem on BandaRodagem.PBR_CODIGO = Pneu.PBR_CODIGO ");
        }

        private void SetarJoinsBandaDimensao(StringBuilder joins)
        {
            SetarJoinsModeloPneu(joins);

            if (!joins.Contains(" Dimensao "))
                joins.Append(" LEFT OUTER JOIN T_TMS_PNEU_DIMENSAO Dimensao on Dimensao.PDM_CODIGO = ModeloPneu.PDM_CODIGO ");
        }

        private void SetarJoinsCentroResultado(StringBuilder joins)
        {
            if (!joins.Contains(" CentroResultado "))
                joins.Append(" LEFT OUTER JOIN T_CENTRO_RESULTADO CentroResultado on CentroResultado.CRE_CODIGO = Veiculo.CRE_CODIGO ");
        }

        private void SetarJoinsCentroSegmento(StringBuilder joins)
        {
            if (!joins.Contains(" Segmento "))
                joins.Append(" LEFT OUTER JOIN T_VEICULO_SEGMENTO Segmento on Segmento.VSE_CODIGO = Veiculo.VSE_CODIGO ");
        }

        #endregion

        #region Métodos Protegidos Sobrescritos

        protected override void SetarSelect(string propriedade, int codigoDinamico, StringBuilder select, StringBuilder joins, StringBuilder groupBy, bool somenteContarNumeroRegistros, FiltroPesquisaRelatorioPneuPorVeiculo filtrosPesquisa)
        {
            switch (propriedade)
            {
                case "NumeroFogo":
                    if (!select.Contains(" NumeroFogo, "))
                    {
                        select.Append("Pneu.PNU_NUMERO_FOGO NumeroFogo, ");
                        groupBy.Append("Pneu.PNU_NUMERO_FOGO, ");

                        SetarJoinsPneu(joins);
                    }
                    break;

                case "DescricaoNumero":
                    if (!select.Contains(" Numero, "))
                    {
                        select.Append("ModeloEixo.MEX_NUMERO Numero, ");
                        groupBy.Append("ModeloEixo.MEX_NUMERO, ");

                        SetarJoinsModeloEixo(joins);
                    }
                    break;

                case "Veiculo":
                    if (!select.Contains(" Veiculo, "))
                    {
                        select.Append("Veiculo.VEI_PLACA Veiculo, ");
                        groupBy.Append("Veiculo.VEI_PLACA, ");

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

                case "DescricaoPosicao":
                    if (!select.Contains(" Posicao, "))
                    {
                        select.Append("ModeloEixoPneu.MEP_POSICAO Posicao, ");
                        groupBy.Append("ModeloEixoPneu.MEP_POSICAO, ");

                        SetarJoinsModeloEixoPneu(joins);
                    }
                    break;

                case "MarcaPneu":
                    if (!select.Contains(" MarcaPneu, "))
                    {
                        select.Append("MarcaPneu.PMR_DESCRICAO MarcaPneu, ");
                        groupBy.Append("MarcaPneu.PMR_DESCRICAO, ");

                        SetarJoinsMarcaPneu(joins);
                    }
                    break;

                case "ModeloPneu":
                    if (!select.Contains(" ModeloPneu, "))
                    {
                        select.Append("ModeloPneu.PML_DESCRICAO ModeloPneu, ");
                        groupBy.Append("ModeloPneu.PML_DESCRICAO, ");

                        SetarJoinsModeloPneu(joins);
                    }
                    break;

                case "BandaRodagem":
                    if (!select.Contains(" BandaRodagem, "))
                    {
                        select.Append("BandaRodagem.PBR_DESCRICAO BandaRodagem, ");
                        groupBy.Append("BandaRodagem.PBR_DESCRICAO, ");

                        SetarJoinsBandaRodagemPneu(joins);
                    }
                    break;

                case "DescricaoBandaRodagem":
                    if (!select.Contains(" TipoBandaRodagem, "))
                    {
                        select.Append("BandaRodagem.PBR_TIPO TipoBandaRodagem, ");
                        groupBy.Append("BandaRodagem.PBR_TIPO, ");

                        SetarJoinsBandaRodagemPneu(joins);
                    }
                    break;

                case "Dimensao":
                    if (!select.Contains(" Dimensao, "))
                    {
                        select.Append("Dimensao.PDM_APLICACAO Dimensao, ");
                        groupBy.Append("Dimensao.PDM_APLICACAO, ");

                        SetarJoinsBandaDimensao(joins);
                    }
                    break;

                case "CentroResultado":
                    if (!select.Contains(" CentroResultado, "))
                    {
                        select.Append("CentroResultado.CRE_DESCRICAO CentroResultado, ");
                        groupBy.Append("CentroResultado.CRE_DESCRICAO, ");

                        SetarJoinsCentroResultado(joins);
                    }
                    break;

                case "Segmento":
                    if (!select.Contains(" Segmento, "))
                    {
                        select.Append("Segmento.VSE_DESCRICAO Segmento, ");
                        groupBy.Append("Segmento.VSE_DESCRICAO, ");

                        SetarJoinsCentroSegmento(joins);
                    }
                    break;

                case "KMRodado":
                    if (!select.Contains(" KMRodado, "))
                    {
                        select.Append("Pneu.PNU_KM_ATUAL_RODADO KMRodado, ");
                        groupBy.Append("Pneu.PNU_KM_ATUAL_RODADO, ");

                        SetarJoinsPneu(joins);
                    }
                    break;

                case "Sulco":
                    if (!select.Contains(" Sulco, "))
                    {
                        select.Append("Pneu.PNU_SULCO Sulco, ");
                        groupBy.Append("Pneu.PNU_SULCO, ");

                        SetarJoinsPneu(joins);
                    }
                    break;

                case "ValorAquisicao":
                    if (!select.Contains(" ValorAquisicao, "))
                    {
                        select.Append("Pneu.PNU_VALOR_AQUISICAO ValorAquisicao, ");
                        groupBy.Append("Pneu.PNU_VALOR_AQUISICAO, ");

                        SetarJoinsPneu(joins);
                    }
                    break;

                case "ValorCusto":
                    if (!select.Contains(" ValorCusto, "))
                    {
                        select.Append("Pneu.PNU_VALOR_CUSTO_ATUALIZADO ValorCusto, ");
                        groupBy.Append("Pneu.PNU_VALOR_CUSTO_ATUALIZADO, ");

                        SetarJoinsPneu(joins);
                    }
                    break;

                case "ValorCustoPorKM":
                    if (!select.Contains(" ValorCustoPorKM, "))
                    {
                        select.Append("Pneu.PNU_VALOR_CUSTO_KM_ATUALIZADO ValorCustoPorKM, ");
                        groupBy.Append("Pneu.PNU_VALOR_CUSTO_KM_ATUALIZADO, ");

                        SetarJoinsPneu(joins);
                    }
                    break;

                case "DescricaoVidaAtual":
                    if (!select.Contains(" VidaAtual, "))
                    {
                        select.Append("Pneu.PNU_VIDA_ATUAL VidaAtual, ");
                        groupBy.Append("Pneu.PNU_VIDA_ATUAL, ");

                        SetarJoinsPneu(joins);
                    }
                    break;
                case "Calibragem":
                    if(!select.Contains(" Calibragem, "))
                    {
                        select.Append("Pneu.PNU_CALIBRAGEM Calibragem, ");
                        groupBy.Append("Pneu.PNU_CALIBRAGEM, ");

                        SetarJoinsPneu(joins);
                    }
                    break;
                case "Milimitragem1":
                    if (!select.Contains(" Milimitragem1, "))
                    {
                        select.Append("Pneu.PNU_MILIMITRAGEM_1 Milimitragem1, ");
                        groupBy.Append("Pneu.PNU_MILIMITRAGEM_1, ");

                        SetarJoinsPneu(joins);
                    }
                    break;
                case "Milimitragem2":
                    if (!select.Contains(" Milimitragem2, "))
                    {
                        select.Append("Pneu.PNU_MILIMITRAGEM_2 Milimitragem2, ");
                        groupBy.Append("Pneu.PNU_MILIMITRAGEM_2, ");

                        SetarJoinsPneu(joins);
                    }
                    break;
                case "Milimitragem3":
                    if (!select.Contains(" Milimitragem3, "))
                    {
                        select.Append("Pneu.PNU_MILIMITRAGEM_3 Milimitragem3, ");
                        groupBy.Append("Pneu.PNU_MILIMITRAGEM_3, ");

                        SetarJoinsPneu(joins);
                    }
                    break;
                case "Milimitragem4":
                    if (!select.Contains(" Milimitragem4, "))
                    {
                        select.Append("Pneu.PNU_MILIMITRAGEM_4 Milimitragem4, ");
                        groupBy.Append("Pneu.PNU_MILIMITRAGEM_4, ");

                        SetarJoinsPneu(joins);
                    }
                    break;
                case "DataMovimentacaoPneu":
                    if (!select.Contains(" DataMovimentacaoPneu, "))
                    {
                        select.Append("VeiculoPneu.VPN_DATA_MOVIMENTACAO_PNEU DataMovimentacaoPneu, ");
                        groupBy.Append("VeiculoPneu.VPN_DATA_MOVIMENTACAO_PNEU, ");

                        SetarJoinsPneu(joins);
                    }
                    break;
                case "DataMovimentacao":
                    if (!select.Contains(" DataMovimentacao, "))
                    {
                        select.Append("VeiculoPneu.VPN_DATA_MOVIMENTACAO DataMovimentacao, ");
                        groupBy.Append("VeiculoPneu.VPN_DATA_MOVIMENTACAO, ");

                        SetarJoinsPneu(joins);
                    }
                    break;

            }
        }

        protected override void SetarWhere(FiltroPesquisaRelatorioPneuPorVeiculo filtrosPesquisa, StringBuilder where, StringBuilder joins, StringBuilder groupBy, List<Embarcador.Consulta.ParametroSQL> parametros = null)
        {

            if (filtrosPesquisa.CodigoVeiculo > 0)
                where.Append($" AND Veiculo.VEI_CODIGO = {filtrosPesquisa.CodigoVeiculo} ");

            if (filtrosPesquisa.CodigoModeloVeicular > 0)
                where.Append($" AND ModeloVeicular.MVC_CODIGO = {filtrosPesquisa.CodigoModeloVeicular} ");

            if (filtrosPesquisa.CodigoMarcaPneu > 0)
                where.Append($" AND MarcaPneu.PMR_CODIGO = {filtrosPesquisa.CodigoMarcaPneu} ");

            if (filtrosPesquisa.CodigoModeloPneu > 0)
                where.Append($" AND ModeloPneu.PML_CODIGO = {filtrosPesquisa.CodigoModeloPneu} ");

            if (filtrosPesquisa.CodigoSegmento.Count > 0)
                where.Append($" AND Veiculo.VSE_CODIGO in ({string.Join(", ", (from obj in filtrosPesquisa.CodigoSegmento select (int)obj).ToList())}) ");

            if (filtrosPesquisa.CodigoCentroResultado.Count > 0)
                where.Append($" AND Veiculo.CRE_CODIGO in ({string.Join(", ", (from obj in filtrosPesquisa.CodigoCentroResultado select (int)obj).ToList())}) ");

            if (filtrosPesquisa.CodigoMotorista > 0)
                where.Append($" and EXISTS (SELECT Motoristas.VEI_CODIGO FROM T_VEICULO_MOTORISTA Motoristas where Motoristas.VEI_CODIGO = Veiculo.VEI_CODIGO and Motoristas.FUN_CODIGO = {filtrosPesquisa.CodigoMotorista}) "); // SQL-INJECTION-SAFE

            if (filtrosPesquisa.CodigoReboque > 0)
                where.Append($" and EXISTS (SELECT Reboques.VEC_CODIGO_PAI FROM T_VEICULO_CONJUNTO Reboques where Reboques.VEC_CODIGO_PAI = Veiculo.VEI_CODIGO and Reboques.VEC_CODIGO_FILHO = {filtrosPesquisa.CodigoReboque}) "); // SQL-INJECTION-SAFE

            if (filtrosPesquisa.CodigoMostrarSomentePosicoesVazias)
                where.Append($" and VeiculoPneu.MEP_CODIGO is null ");
        }

        #endregion
    }
}