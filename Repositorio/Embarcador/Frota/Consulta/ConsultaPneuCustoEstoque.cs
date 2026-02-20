using Dominio.ObjetosDeValor.Embarcador.Frota;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Repositorio.Embarcador.Frota
{
    sealed class ConsultaPneuCustoEstoque : Consulta.Consulta<Dominio.ObjetosDeValor.Embarcador.Frota.FiltroPesquisaRelatorioPneuCustoEstoque>
    {

        #region Construtor

        public ConsultaPneuCustoEstoque() : base(tabela: "T_TMS_PNEU as Pneu") { }

        #endregion

        #region Métodos Privados

        private void SetarJoinsVeiculoPneu(StringBuilder joins)
        {
            if (!joins.Contains(" VeiculoPneu "))
                joins.Append("LEFT JOIN T_VEICULO_PNEU VeiculoPneu ON VeiculoPneu.PNU_CODIGO = Pneu.PNU_CODIGO ");
        }

        private void SetarJoinsVeiculo(StringBuilder joins)
        {
            SetarJoinsVeiculoPneu(joins);

            if (!joins.Contains(" Veiculo "))
                joins.Append(" LEFT JOIN T_VEICULO Veiculo ON Veiculo.VEI_CODIGO = VeiculoPneu.VEI_CODIGO ");
        }

        private void SetarJoinsVeiculoEstepe(StringBuilder joins)
        {
            SetarJoinsEstepe(joins);

            if (!joins.Contains(" PlacaVeiculoDoEstepe "))
                joins.Append(" LEFT OUTER JOIN T_VEICULO VeiculoEstepe on Estepe.VEI_CODIGO = VeiculoEstepe.VEI_CODIGO ");
        }

        private void SetarJoinsEstepe(StringBuilder joins)
        {
            SetarJoinsVeiculoPneu(joins);

            if (!joins.Contains(" Estepe "))
                joins.Append(" LEFT OUTER JOIN T_VEICULO_ESTEPE Estepe on Estepe.PNU_CODIGO = Pneu.PNU_CODIGO ");
        }


        private void SetarJoinsEixoPneu(StringBuilder joins)
        {
            SetarJoinsVeiculoPneu(joins);

            if (!joins.Contains(" EixoPneu "))
                joins.Append("LEFT JOIN T_MODELO_VEICULAR_CARGA_EIXO_PNEU EixoPneu ON EixoPneu.MEP_CODIGO = VeiculoPneu.MEP_CODIGO ");
        }

        private void SetarJoinsEixo(StringBuilder joins)
        {
            SetarJoinsEixoPneu(joins);

            if (!joins.Contains(" Eixo "))
                joins.Append("LEFT JOIN T_MODELO_VEICULAR_CARGA_EIXO Eixo ON Eixo.MEX_CODIGO = EixoPneu.MEX_CODIGO ");
        }

        private void SetarJoinsModelo(StringBuilder joins)
        {
            if (!joins.Contains(" Modelo "))
                joins.Append("LEFT OUTER JOIN T_TMS_PNEU_MODELO Modelo ON Modelo.PML_CODIGO = Pneu.PML_CODIGO ");
        }

        private void SetarJoinsDimensao(StringBuilder joins)
        {
            SetarJoinsModelo(joins);

            if (!joins.Contains(" Dimensao "))
                joins.Append("LEFT OUTER JOIN T_TMS_PNEU_DIMENSAO Dimensao ON Dimensao.PDM_CODIGO = Modelo.PDM_CODIGO ");
        }

        private void SetarJoinsMarca(StringBuilder joins)
        {
            SetarJoinsModelo(joins);

            if (!joins.Contains(" Marca "))
                joins.Append("LEFT OUTER JOIN T_TMS_PNEU_MARCA Marca on Marca.PMR_CODIGO = Modelo.PMR_CODIGO ");
        }

        private void SetarJoinsBandaRodagem(StringBuilder joins)
        {
            if (!joins.Contains(" BandaRodagem "))
                joins.Append("LEFT OUTER JOIN T_TMS_PNEU_BANDA_RODAGEM BandaRodagem on BandaRodagem.PBR_CODIGO = Pneu.PBR_CODIGO ");
        }

        private void SetarJoinsAlmoxarifado(StringBuilder joins)
        {
            if (!joins.Contains(" Almoxarifado "))
                joins.Append("LEFT OUTER JOIN T_ALMOXARIFADO Almoxarifado ON Almoxarifado.AMX_CODIGO = Pneu.AMX_CODIGO ");
        }

        private void SetarJoinsPneuHistorico(StringBuilder joins)
        {
            if (!joins.Contains(" PneuHistorico "))
                joins.Append("LEFT OUTER JOIN T_TMS_PNEU_HISTORICO PneuHistorico ON PneuHistorico.PNU_CODIGO = Pneu.PNU_CODIGO ");
        }

        #endregion

        #region Métodos Protegidos Sobrescritos

        protected override void SetarSelect(string propriedade, int codigoDinamico, StringBuilder select, StringBuilder joins, StringBuilder groupBy, bool somenteContarNumeroRegistros, FiltroPesquisaRelatorioPneuCustoEstoque filtrosPesquisa)
        {
            switch (propriedade)
            {
                case "Almoxarifado":
                    if (!select.Contains(" Almoxarifado, "))
                    {
                        select.Append("CASE WHEN Veiculo.VEI_PLACA IS NULL THEN Almoxarifado.AMX_DESCRICAO ELSE '' END Almoxarifado,");
                        groupBy.Append("Almoxarifado.AMX_DESCRICAO, ");

                        SetarJoinsAlmoxarifado(joins);

                        if (!select.Contains(" Veiculo, "))
                        {
                            groupBy.Append("Veiculo.VEI_PLACA, ");

                            SetarJoinsVeiculo(joins);
                        }
                    }
                    break;

                case "PosicaoDescricao":
                    if (!select.Contains(" Posicao, "))
                    {
                        select.Append("EixoPneu.MEP_POSICAO Posicao, ");
                        groupBy.Append("EixoPneu.MEP_POSICAO, ");

                        select.Append("Eixo.MEX_NUMERO PosicaoNumero, ");
                        groupBy.Append("Eixo.MEX_NUMERO, ");

                        SetarJoinsEixoPneu(joins);
                        SetarJoinsEixo(joins);
                    }
                    break;

                case "NumeroFogo":
                    if (!select.Contains(" NumeroFogo, "))
                    {
                        select.Append("Pneu.PNU_NUMERO_FOGO NumeroFogo, ");
                        groupBy.Append("Pneu.PNU_NUMERO_FOGO, ");
                    }
                    break;

                case "Dimensao":
                    if (!select.Contains(" Dimensao, "))
                    {
                        select.Append("CAST(Dimensao.PDM_LARGURA AS VARCHAR(20)) + '/' + CAST(Dimensao.PDM_PERFIL AS VARCHAR(20)) + CASE WHEN Dimensao.PDM_RADIAL = 1 THEN 'R' ELSE '' END + CAST(Dimensao.PDM_ARO AS VARCHAR(20)) Dimensao, ");

                        if (!groupBy.Contains("Dimensao.PDM_LARGURA"))
                            groupBy.Append("Dimensao.PDM_LARGURA, ");

                        if (!groupBy.Contains("Dimensao.PDM_PERFIL"))
                            groupBy.Append("Dimensao.PDM_PERFIL, ");

                        if (!groupBy.Contains("Dimensao.PDM_RADIAL"))
                            groupBy.Append("Dimensao.PDM_RADIAL, ");

                        if (!groupBy.Contains("Dimensao.PDM_ARO"))
                            groupBy.Append("Dimensao.PDM_ARO, ");

                        SetarJoinsDimensao(joins);
                    }
                    break;

                case "Marca":
                    if (!select.Contains(" Marca, "))
                    {
                        select.Append("Marca.PMR_DESCRICAO Marca, ");
                        groupBy.Append("Marca.PMR_DESCRICAO, ");

                        SetarJoinsMarca(joins);
                    }
                    break;

                case "Modelo":
                    if (!select.Contains(" Modelo, "))
                    {
                        select.Append("Modelo.PML_DESCRICAO Modelo, ");
                        groupBy.Append("Modelo.PML_DESCRICAO, ");

                        SetarJoinsModelo(joins);
                    }
                    break;

                case "Banda":
                    if (!select.Contains(" Banda, "))
                    {
                        select.Append("BandaRodagem.PBR_DESCRICAO Banda, ");
                        groupBy.Append("BandaRodagem.PBR_DESCRICAO, ");

                        SetarJoinsBandaRodagem(joins);
                    }
                    break;

                case "KmRodado":
                    if (!select.Contains(" KmRodado, "))
                    {
                        select.Append("Pneu.PNU_KM_ATUAL_RODADO KmRodado, ");
                        groupBy.Append("Pneu.PNU_KM_ATUAL_RODADO, ");
                    }
                    break;

                case "Sulco":
                    if (!select.Contains(" Sulco, "))
                    {
                        select.Append("Pneu.PNU_SULCO Sulco, ");
                        groupBy.Append("Pneu.PNU_SULCO, ");
                    }
                    break;

                case "ValorAquisicao":
                    if (!select.Contains(" ValorAquisicao, "))
                    {
                        select.Append("Pneu.PNU_VALOR_AQUISICAO ValorAquisicao, ");
                        groupBy.Append("Pneu.PNU_VALOR_AQUISICAO, ");
                    }
                    break;

                case "Custo":
                    if (!select.Contains(" Custo, "))
                    {
                        select.Append("Pneu.PNU_VALOR_CUSTO_ATUALIZADO Custo, ");
                        groupBy.Append("Pneu.PNU_VALOR_CUSTO_ATUALIZADO, ");
                    }
                    break;

                case "CustoKM":
                    if (!select.Contains(" CustoKM, "))
                    {
                        select.Append("Pneu.PNU_VALOR_CUSTO_KM_ATUALIZADO CustoKM, ");
                        groupBy.Append("Pneu.PNU_VALOR_CUSTO_KM_ATUALIZADO, ");
                    }
                    break;

                case "VidaAtualDescricao":
                    if (!select.Contains(" VidaAtual, "))
                    {
                        select.Append("Pneu.PNU_VIDA_ATUAL VidaAtual, ");
                        groupBy.Append("Pneu.PNU_VIDA_ATUAL, ");
                    }
                    break;

                case "DataAquisicaoFormatada":
                    if (!select.Contains(" DataAquisicao, "))
                    {
                        select.Append("Pneu.PNU_DATA_ENTRADA DataAquisicao, ");
                        groupBy.Append("Pneu.PNU_DATA_ENTRADA, ");
                    }
                    break;

                case "DataUltimaMovimentacaoFormatada":
                    if (!select.Contains(" DataUltimaMovimentacao, "))
                    {
                        select.Append("(SELECT TOP 1 Historico.PNH_DATA FROM T_TMS_PNEU_HISTORICO Historico WHERE Historico.PNU_CODIGO = Pneu.PNU_CODIGO ORDER BY Historico.PNH_DATA DESC) DataUltimaMovimentacao, ");

                        if (!groupBy.Contains("Pneu.PNU_CODIGO"))
                            groupBy.Append("Pneu.PNU_CODIGO, ");
                    }
                    break;

                case "BandaRodagemUltimaMovimentacao":
                    if (!select.Contains(" BandaRodagemUltimaMovimentacao, "))
                    {
                        select.Append("(SELECT TOP 1 BAH.PBR_DESCRICAO FROM T_TMS_PNEU_HISTORICO Historico JOIN T_TMS_PNEU_BANDA_RODAGEM BAH ON BAH.PBR_CODIGO = Historico.PBR_CODIGO WHERE Historico.PNU_CODIGO = Pneu.PNU_CODIGO ORDER BY Historico.PNH_DATA DESC) BandaRodagemUltimaMovimentacao, ");

                        if (!groupBy.Contains("Pneu.PNU_CODIGO"))
                            groupBy.Append("Pneu.PNU_CODIGO, ");
                    }
                    break;

                case "EstadoAtualPneuDescricao":
                    if (!select.Contains(" EstadoAtualPneu, "))
                    {
                        select.Append(" CASE ");
                        select.Append("     WHEN Pneu.PNU_VIDA_ATUAL = 1 AND Pneu.PNU_KM_ATUAL_RODADO = 0 THEN 1 ");
                        select.Append("     WHEN Pneu.PNU_VIDA_ATUAL = 1 AND Pneu.PNU_KM_ATUAL_RODADO > 0 THEN 2 ");
                        select.Append("     WHEN Pneu.PNU_VIDA_ATUAL > 1 AND Pneu.PNU_KM_ATUAL_RODADO = 0 THEN 3 ");
                        select.Append("     WHEN Pneu.PNU_VIDA_ATUAL > 1 AND Pneu.PNU_KM_ATUAL_RODADO > 0 THEN 4 ");
                        select.Append(" END EstadoAtualPneu, ");

                        if (!groupBy.Contains("Pneu.PNU_KM_ATUAL_RODADO"))
                            groupBy.Append("Pneu.PNU_KM_ATUAL_RODADO, ");

                        if (!groupBy.Contains("Pneu.PNU_VIDA_ATUAL"))
                            groupBy.Append("Pneu.PNU_VIDA_ATUAL, ");
                    }
                    break;

                case "Veiculo":
                    if (!select.Contains(" Veiculo, "))
                    {
                        select.Append("Veiculo.VEI_PLACA Veiculo, ");
                        groupBy.Append("Veiculo.VEI_PLACA, ");

                        SetarJoinsVeiculo(joins);
                    }
                    break;

                case "PlacaVeiculoDoEstepe":
                    if (!select.Contains(" PlacaVeiculoDoEstepe, "))
                    {
                        select.Append("VeiculoEstepe.VEI_PLACA PlacaVeiculoDoEstepe, ");
                        groupBy.Append("VeiculoEstepe.VEI_PLACA, ");

                        SetarJoinsVeiculoEstepe(joins);
                    }
                    break;

                case "Estepe":
                    if (!select.Contains(" Estepe, "))
                    {
                        select.Append(" CASE WHEN Estepe.VES_CODIGO IS NULL THEN 'Não' ELSE 'Sim' end Estepe, ");
                        groupBy.Append(" Estepe.VES_CODIGO, ");

                        SetarJoinsEstepe(joins);
                    }
                    break;
                case "SituacaoDescricao":
                    if (!select.Contains(" Situacao, "))
                    {
                        select.Append(" Pneu.PNU_SITUACAO Situacao, ");

                        if (!groupBy.Contains("Pneu.PNU_SITUACAO"))
                            groupBy.Append("Pneu.PNU_SITUACAO, ");

                    }
                    break;
            }
        }

        protected override void SetarWhere(FiltroPesquisaRelatorioPneuCustoEstoque filtrosPesquisa, StringBuilder where, StringBuilder joins, StringBuilder groupBy, List<Embarcador.Consulta.ParametroSQL> parametros = null)
        {
            if (filtrosPesquisa.DataAquisicaoInicial != DateTime.MinValue)
                where.Append($" AND Pneu.PNU_DATA_ENTRADA >= '{filtrosPesquisa.DataAquisicaoInicial.ToString("yyyyMMdd HH:mm:ss")}'");

            if (filtrosPesquisa.DataAquisicaoFinal != DateTime.MinValue)
                where.Append($" AND Pneu.PNU_DATA_ENTRADA < '{filtrosPesquisa.DataAquisicaoFinal.AddDays(1).ToString("yyyyMMdd HH:mm:ss")}'");

            if (filtrosPesquisa.Vida.HasValue)
                where.Append($" AND Pneu.PNU_VIDA_ATUAL = {(int)filtrosPesquisa.Vida.Value} ");

            if (filtrosPesquisa.CodigoBandaRodagem > 0)
                where.Append($" AND Pneu.PBR_CODIGO = {filtrosPesquisa.CodigoBandaRodagem} ");

            if (filtrosPesquisa.CodigoDimensao > 0)
            {
                where.Append($" AND Modelo.PDM_CODIGO = {filtrosPesquisa.CodigoDimensao} ");

                SetarJoinsModelo(joins);
            }

            if (filtrosPesquisa.CodigoMarca > 0)
            {
                where.Append($" AND Modelo.PMR_CODIGO = {filtrosPesquisa.CodigoMarca} ");

                SetarJoinsModelo(joins);
            }

            if (filtrosPesquisa.CodigoModelo > 0)
                where.Append($" AND Pneu.PML_CODIGO = {filtrosPesquisa.CodigoModelo} ");

            if (filtrosPesquisa.CodigoPneu > 0)
                where.Append($" AND Pneu.PNU_CODIGO = {filtrosPesquisa.CodigoPneu} ");

            if (filtrosPesquisa.CodigoServicoVeiculoFrota > 0)
            {
                where.Append($" AND PneuHistorico.PNH_SERVICOS LIKE (SELECT SEV_DESCRICAO FROM T_FROTA_SERVICO_VEICULO WHERE SEV_CODIGO = {filtrosPesquisa.CodigoServicoVeiculoFrota}) "); // SQL-INJECTION-SAFE

                SetarJoinsPneuHistorico(joins);
            }

            if (filtrosPesquisa.CodigoVeiculo > 0)
            {
                where.Append($" AND VeiculoPneu.VEI_CODIGO = {filtrosPesquisa.CodigoVeiculo} ");

                SetarJoinsVeiculoPneu(joins);
            }

            if (filtrosPesquisa.CodigoAlmoxarifado > 0)
            {
                where.Append($" AND Pneu.AMX_CODIGO = {filtrosPesquisa.CodigoAlmoxarifado} AND VeiculoPneu.VEI_CODIGO IS NULL ");

                SetarJoinsVeiculoPneu(joins);
            }

            if (filtrosPesquisa.EstadoAtualPneu.Count > 0)
            {
                where.Append(" AND ( 1 = 0 ");

                foreach (Dominio.ObjetosDeValor.Embarcador.Enumeradores.EstadoPneu estadoPneu in filtrosPesquisa.EstadoAtualPneu)
                {
                    switch (estadoPneu)
                    {
                        case Dominio.ObjetosDeValor.Embarcador.Enumeradores.EstadoPneu.PneuNovo:
                            where.Append(" or ( Pneu.PNU_VIDA_ATUAL = 1 AND Pneu.PNU_KM_ATUAL_RODADO = 0 ) ");
                            break;
                        case Dominio.ObjetosDeValor.Embarcador.Enumeradores.EstadoPneu.PneuUsado:
                            where.Append(" or ( Pneu.PNU_VIDA_ATUAL = 1 AND Pneu.PNU_KM_ATUAL_RODADO > 0 ) ");
                            break;
                        case Dominio.ObjetosDeValor.Embarcador.Enumeradores.EstadoPneu.PneuRecauchutadoNovo:
                            where.Append(" or ( Pneu.PNU_VIDA_ATUAL > 1 AND Pneu.PNU_KM_ATUAL_RODADO = 0 ) ");
                            break;
                        case Dominio.ObjetosDeValor.Embarcador.Enumeradores.EstadoPneu.PneuRecauchitadoUsado:
                            where.Append(" or ( Pneu.PNU_VIDA_ATUAL > 1 AND Pneu.PNU_KM_ATUAL_RODADO > 0 ) ");
                            break;
                    }
                }

                where.Append(" ) ");
            }
            if (filtrosPesquisa.Situacao.Count > 0)
            {
                string situacoes = string.Join(", ", from situacao in filtrosPesquisa.Situacao select situacao.ToString("d"));

                where.Append($" and Pneu.PNU_SITUACAO IN ({situacoes})");
            }

        }

        #endregion
    }
}