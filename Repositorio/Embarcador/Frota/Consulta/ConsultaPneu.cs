using Dominio.ObjetosDeValor.Embarcador.Frota;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Repositorio.Embarcador.Frota
{
    sealed class ConsultaPneu : Consulta.Consulta<Dominio.ObjetosDeValor.Embarcador.Frota.FiltroPesquisaRelatorioPneu>
    {
        #region Construtores

        public ConsultaPneu() : base(tabela: "T_TMS_PNEU Pneu") { }

        #endregion

        #region Métodos Privados

        private void SetarJoinsModeloPneu(StringBuilder joins)
        {
            if (!joins.Contains(" ModeloPneu "))
                joins.Append(" INNER JOIN T_TMS_PNEU_MODELO ModeloPneu ON ModeloPneu.PML_CODIGO = Pneu.PML_CODIGO ");
        }

        private void SetarJoinsMarcaPneu(StringBuilder joins)
        {
            SetarJoinsModeloPneu(joins);

            if (!joins.Contains(" MarcaPneu "))
                joins.Append(" INNER JOIN T_TMS_PNEU_MARCA MarcaPneu ON MarcaPneu.PMR_CODIGO = ModeloPneu.PMR_CODIGO ");
        }

        private void SetarJoinsBandaRodagemPneu(StringBuilder joins)
        {
            if (!joins.Contains(" BandaRodagem "))
                joins.Append(" INNER JOIN T_TMS_PNEU_BANDA_RODAGEM BandaRodagem ON BandaRodagem.PBR_CODIGO = Pneu.PBR_CODIGO ");
        }

        private void SetarJoinsAlmoxarifado(StringBuilder joins)
        {
            if (!joins.Contains(" Almoxarifado "))
                joins.Append(" LEFT JOIN T_ALMOXARIFADO Almoxarifado ON Almoxarifado.AMX_CODIGO = Pneu.AMX_CODIGO ");
        }

        private void SetarJoinsVeiculoPneu(StringBuilder joins)
        {
            if (!joins.Contains(" VeiculoPneu "))
                joins.Append(" LEFT JOIN T_VEICULO_PNEU VeiculoPneu ON VeiculoPneu.PNU_CODIGO = Pneu.PNU_CODIGO ");
        }

        private void SetarJoinsEstepe(StringBuilder joins)
        {
            if (!joins.Contains(" Estepe "))
                joins.Append(" LEFT JOIN T_VEICULO_ESTEPE Estepe ON Estepe.PNU_CODIGO = Pneu.PNU_CODIGO ");
        }

        private void SetarJoinsVeiculoEixo(StringBuilder joins)
        {
            SetarJoinsVeiculoPneu(joins);

            if (!joins.Contains(" VeiculoEixo "))
                joins.Append(" LEFT JOIN T_VEICULO VeiculoEixo ON VeiculoEixo.VEI_CODIGO = VeiculoPneu.VEI_CODIGO ");
        }

        private void SetarJoinsVeiculoEstepe(StringBuilder joins)
        {
            SetarJoinsEstepe(joins);

            if (!joins.Contains(" VeiculoEstepe "))
                joins.Append(" LEFT JOIN T_VEICULO VeiculoEstepe ON VeiculoEstepe.VEI_CODIGO = Estepe.VEI_CODIGO ");
        }

        private void SetarJoinsModeloVeiculo(StringBuilder joins)
        {
            SetarJoinsVeiculoEixo(joins);

            if (!joins.Contains(" ModeloVeiculo "))
                joins.Append(" LEFT JOIN T_MODELO_VEICULAR_CARGA ModeloVeiculo ON ModeloVeiculo.MVC_CODIGO = VeiculoEixo.MVC_CODIGO ");
        }

        private void SetarJoinsModeloEstepe(StringBuilder joins)
        {
            SetarJoinsVeiculoEstepe(joins);

            if (!joins.Contains(" ModeloEstepe "))
                joins.Append(" LEFT JOIN T_MODELO_VEICULAR_CARGA ModeloEstepe ON ModeloEstepe.MVC_CODIGO = VeiculoEstepe.MVC_CODIGO ");
        }

        #endregion

        #region Métodos Protegidos Sobrescritos

        protected override void SetarSelect(string propriedade, int codigoDinamico, StringBuilder select, StringBuilder joins, StringBuilder groupBy, bool somenteContarNumeroRegistros, FiltroPesquisaRelatorioPneu filtrosPesquisa)
        {
            switch (propriedade)
            {
                case "NumeroFogo":
                    if (!select.Contains(" NumeroFogo, "))
                    {
                        select.Append("Pneu.PNU_NUMERO_FOGO NumeroFogo, ");
                        groupBy.Append("Pneu.PNU_NUMERO_FOGO, ");
                    }
                    break;

                case "Frota":
                    if (!select.Contains(" Frota, "))
                    {
                        select.Append("COALESCE(VeiculoEixo.VEI_NUMERO_FROTA, VeiculoEstepe.VEI_NUMERO_FROTA, '') Frota, ");
                        groupBy.Append("VeiculoEixo.VEI_NUMERO_FROTA, ");
                        groupBy.Append("VeiculoEstepe.VEI_NUMERO_FROTA, ");

                        SetarJoinsVeiculoEixo(joins);
                        SetarJoinsVeiculoEstepe(joins);
                    }
                    break;

                case "Placa":
                    if (!select.Contains(" Placa, "))
                    {
                        select.Append("COALESCE(VeiculoEixo.VEI_PLACA, VeiculoEstepe.VEI_PLACA, '') Placa, ");
                        groupBy.Append("VeiculoEixo.VEI_PLACA, ");
                        groupBy.Append("VeiculoEstepe.VEI_PLACA, ");

                        SetarJoinsVeiculoEixo(joins);
                        SetarJoinsVeiculoEstepe(joins);
                    }
                    break;

                case "ModeloVeicular":
                    if (!select.Contains(" ModeloVeicular, "))
                    {
                        select.Append("COALESCE(ModeloVeiculo.MVC_DESCRICAO, ModeloEstepe.MVC_DESCRICAO, '') ModeloVeicular, ");
                        groupBy.Append("ModeloVeiculo.MVC_DESCRICAO, ");
                        groupBy.Append("ModeloEstepe.MVC_DESCRICAO, ");

                        SetarJoinsModeloVeiculo(joins);
                        SetarJoinsModeloEstepe(joins);
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

                case "TipoBandaRodagemDescricao":
                    if (!select.Contains(" TipoBandaRodagem, "))
                    {
                        select.Append("BandaRodagem.PBR_TIPO TipoBandaRodagem, ");
                        groupBy.Append("BandaRodagem.PBR_TIPO, ");

                        SetarJoinsBandaRodagemPneu(joins);
                    }
                    break;

                case "StatusPneuDescricao":
                    if (!select.Contains(" StatusPneu, "))
                    {
                        select.Append("Pneu.PNU_SITUACAO StatusPneu, ");
                        groupBy.Append("Pneu.PNU_SITUACAO, ");
                    }
                    break;

                case "DataEntradaFormatada":
                    if (!select.Contains(" DataEntrada, "))
                    {
                        select.Append("Pneu.PNU_DATA_ENTRADA DataEntrada, ");
                        groupBy.Append("Pneu.PNU_DATA_ENTRADA, ");
                    }
                    break;

                case "AlmoxarifadoAtual":
                    if (!select.Contains(" AlmoxarifadoAtual, "))
                    {
                        select.Append("CASE " +
                                      "   WHEN Pneu.PNU_SITUACAO = 1 then Almoxarifado.AMX_DESCRICAO " +
                                      "   ELSE '' " +
                                      "END AlmoxarifadoAtual, ");

                        groupBy.Append("Almoxarifado.AMX_DESCRICAO, ");

                        SetarJoinsAlmoxarifado(joins);
                    }
                    break;

                case "TPAquisicaoDescricao":
                    if (!select.Contains(" TPAquisicao, "))
                    {
                        select.Append("Pneu.PNU_TIPO_AQUISICAO TPAquisicao, ");
                        groupBy.Append("Pneu.PNU_TIPO_AQUISICAO, ");
                    }
                    break;

                case "KmRodado":
                    if (!select.Contains(" KmRodado, "))
                    {
                        select.Append("Pneu.PNU_KM_ATUAL_RODADO KmRodado, ");
                        groupBy.Append("Pneu.PNU_KM_ATUAL_RODADO, ");
                    }
                    break;

                case "VidaUtilDescricao":
                    if (!select.Contains(" VidaUtil, "))
                    {
                        select.Append("Pneu.PNU_VIDA_ATUAL VidaUtil, ");
                        groupBy.Append("Pneu.PNU_VIDA_ATUAL, ");
                    }
                    break;

                case "Movimentacao":
                    if (!select.Contains(" Movimentacao, "))
                    {
                        select.Append("(select case when (exists (select pneuHistorico.PNU_CODIGO from T_TMS_PNEU_HISTORICO pneuHistorico where pneuHistorico.PNU_CODIGO = Pneu.PNU_CODIGO)) then 'Sim' else 'Não' end) Movimentacao, ");
                        groupBy.Append("Pneu.PNU_CODIGO, ");
                    }
                    break;



            }
        }

        protected override void SetarWhere(FiltroPesquisaRelatorioPneu filtrosPesquisa, StringBuilder where, StringBuilder joins, StringBuilder groupBy, List<Embarcador.Consulta.ParametroSQL> parametros = null)
        {

            if (filtrosPesquisa.CodigoPneu > 0)
                where.Append($" AND Pneu.PNU_CODIGO = {filtrosPesquisa.CodigoPneu} ");

            if (filtrosPesquisa.CodigoModeloPneu > 0)
                where.Append($" AND Pneu.PML_CODIGO = {filtrosPesquisa.CodigoModeloPneu} ");

            if (filtrosPesquisa.CodigoMarcaPneu > 0)
            {
                where.Append($" AND ModeloPneu.PMR_CODIGO = {filtrosPesquisa.CodigoMarcaPneu} ");
                SetarJoinsModeloPneu(joins);
            }

            if (filtrosPesquisa.CodigoVeiculo > 0)
            {
                where.Append($" AND (VeiculoEixo.VEI_CODIGO = {filtrosPesquisa.CodigoVeiculo} OR VeiculoEstepe.VEI_CODIGO = {filtrosPesquisa.CodigoVeiculo}) ");

                SetarJoinsVeiculoEixo(joins);
                SetarJoinsVeiculoEstepe(joins);
            }

            if (filtrosPesquisa.TiposBandaRodagem.Count > 0)
            {
                where.Append($" AND BandaRodagem.PBR_TIPO in ({string.Join(", ", filtrosPesquisa.TiposBandaRodagem.Select(o => o.ToString("D")))}) ");

                SetarJoinsBandaRodagemPneu(joins);
            }

            if (filtrosPesquisa.StatusPneu.HasValue && filtrosPesquisa.StatusPneu.Value > 0)
                where.Append($" AND Pneu.PNU_SITUACAO = {(int)filtrosPesquisa.StatusPneu.Value} ");

            if (filtrosPesquisa.Movimentacao == Dominio.ObjetosDeValor.Embarcador.Enumeradores.SimNao.Sim)
                where.Append($" AND ( EXISTS (SELECT 1 FROM T_TMS_PNEU_HISTORICO pneuHistorico WHERE pneuHistorico.PNU_CODIGO = Pneu.PNU_CODIGO))");

            if (filtrosPesquisa.Movimentacao == Dominio.ObjetosDeValor.Embarcador.Enumeradores.SimNao.Nao)
                where.Append($" AND NOT (EXISTS (SELECT 1 FROM T_TMS_PNEU_HISTORICO pneuHistorico WHERE pneuHistorico.PNU_CODIGO = Pneu.PNU_CODIGO))");

            if (filtrosPesquisa.VidaUtil.HasValue)
                where.Append($"and Pneu.PNU_VIDA_ATUAL = {filtrosPesquisa.VidaUtil.Value.ToString("D")}");

        }

        #endregion
    }
}
