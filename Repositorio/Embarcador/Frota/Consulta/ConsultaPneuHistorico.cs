using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Repositorio.Embarcador.Frota
{
    sealed class ConsultaPneuHistorico : Consulta.Consulta<Dominio.ObjetosDeValor.Embarcador.Frota.FiltroPesquisaPneuHistorico>
    {
        #region Construtores

        public ConsultaPneuHistorico() : base(tabela: "T_TMS_PNEU_HISTORICO as PneuHistorico") { }

        #endregion

        #region Métodos Privados 

        private void SetarJoinsBandaRodagem(StringBuilder joins)
        {
            SetarJoinsPneu(joins);

            if (!joins.Contains(" BandaRodagemPneu "))
                joins.Append("join T_TMS_PNEU_BANDA_RODAGEM BandaRodagemPneu on BandaRodagemPneu.PBR_CODIGO = Pneu.PBR_CODIGO ");

            if (!joins.Contains(" BandaRodagemHistorico "))
                joins.Append("LEFT OUTER JOIN T_TMS_PNEU_BANDA_RODAGEM BandaRodagemHistorico ON BandaRodagemHistorico.PBR_CODIGO = PneuHistorico.PBR_CODIGO ");

        }

        private void SetarJoinsDimensao(StringBuilder joins)
        {
            SetarJoinsModelo(joins);

            if (!joins.Contains(" DimensaoPneu "))
                joins.Append("join T_TMS_PNEU_DIMENSAO DimensaoPneu on DimensaoPneu.PDM_CODIGO = ModeloPneu.PDM_CODIGO ");
        }

        private void SetarJoinsMarca(StringBuilder joins)
        {
            SetarJoinsModelo(joins);

            if (!joins.Contains(" MarcaPneu "))
                joins.Append("join T_TMS_PNEU_MARCA MarcaPneu on MarcaPneu.PMR_CODIGO = ModeloPneu.PMR_CODIGO ");
        }

        private void SetarJoinsModelo(StringBuilder joins)
        {
            SetarJoinsPneu(joins);

            if (!joins.Contains(" ModeloPneu "))
                joins.Append(" join T_TMS_PNEU_MODELO ModeloPneu on ModeloPneu.PML_CODIGO = Pneu.PML_CODIGO ");
        }

        private void SetarJoinsPneu(StringBuilder joins)
        {
            if (!joins.Contains(" Pneu "))
                joins.Append(" join T_TMS_PNEU Pneu on Pneu.PNU_CODIGO = PneuHistorico.PNU_CODIGO ");
        }

        private void SetarJoinsSucata(StringBuilder joins)
        {
            if (!joins.Contains(" SucataPneu "))
                joins.Append("left join T_TMS_PNEU_SUCATA SucataPneu on SucataPneu.PNH_CODIGO = PneuHistorico.PNH_CODIGO ");
        }

        private void SetarJoinsMovimentacaoVeiculo(StringBuilder joins)
        {
            joins.Append($"left join T_VEICULO_MOVIMENTACAO_PNEU MovimentacaoPneu on MovimentacaoPneu.PNH_CODIGO = PneuHistorico.PNH_CODIGO ");
        }

        private void SetarJoinsUsuarioOperador(StringBuilder joins)
        {
            if (!joins.Contains(" Usuario "))
                joins.Append("left join T_FUNCIONARIO Usuario on Usuario.FUN_CODIGO = PneuHistorico.FUN_CODIGO ");
        }

        private void SetarJoinsAlmoxarifado(StringBuilder joins)
        {
            SetarJoinsPneu(joins);

            if (!joins.Contains(" Almoxarifado "))
                joins.Append("LEFT OUTER JOIN T_ALMOXARIFADO Almoxarifado on Almoxarifado.AMX_CODIGO = Pneu.AMX_CODIGO ");
        }

        private void SetarJoinsMotivoSucata(StringBuilder joins)
        {
            SetarJoinsSucata(joins);

            if (!joins.Contains(" MotivoSucata "))
                joins.Append("LEFT OUTER JOIN T_TMS_PNEU_MOTIVO_SUCATEAMENTO MotivoSucata on MotivoSucata.PMS_CODIGO = SucataPneu.PMS_CODIGO ");
        }
        private void SetarJoinsRecape(StringBuilder joins)
        {
            if (!joins.Contains(" ValorResidualAtualPneu "))
                joins.Append("LEFT OUTER JOIN T_TMS_PNEU_RETORNO_REFORMA Reforma on Reforma.PNH_CODIGO = PneuHistorico.PNH_CODIGO ");
        }

        private void SetarJoinsObservacao(StringBuilder joins)
        {
            if (!joins.Contains(" ObservacaoPneu "))
            {
                joins.Append("LEFT OUTER JOIN T_TMS_PNEU_ENVIO_REFORMA AS EnvioReforma ON EnvioReforma.PNH_CODIGO = PneuHistorico.PNH_CODIGO ");
                joins.Append("LEFT OUTER JOIN T_TMS_PNEU_RETORNO_REFORMA AS RetornoReforma ON RetornoReforma.PNH_CODIGO = PneuHistorico.PNH_CODIGO ");
            }
                
        }

        #endregion

        #region Métodos Protegidos Sobrescritos

        protected override void SetarSelect(string propriedade, int codigoDinamico, StringBuilder select, StringBuilder joins, StringBuilder groupBy, bool somenteContarNumeroRegistros, Dominio.ObjetosDeValor.Embarcador.Frota.FiltroPesquisaPneuHistorico filtroPesquisa)
        {
            switch (propriedade)
            {
                case "BandaRodagem":
                    if (!select.Contains(" BandaRodagem"))
                    {
                        select.Append("BandaRodagemPneu.PBR_DESCRICAO as BandaRodagem, ");
                        groupBy.Append("BandaRodagemPneu.PBR_DESCRICAO, ");

                        SetarJoinsBandaRodagem(joins);
                    }
                    break;

                case "BandaRodagemHistorico":
                    if (!select.Contains(" BandaRodagemHistorico"))
                    {
                        select.Append("BandaRodagemHistorico.PBR_DESCRICAO as BandaRodagemHistorico, ");
                        groupBy.Append("BandaRodagemHistorico.PBR_DESCRICAO, ");

                        SetarJoinsBandaRodagem(joins);
                    }
                    break;

                case "Codigo":
                    if (!select.Contains(" Codigo"))
                    {
                        select.Append("PneuHistorico.PNH_CODIGO as Codigo, ");
                        groupBy.Append("PneuHistorico.PNH_CODIGO, ");
                    }
                    break;
                case "KmAtualRodado":
                    if (!select.Contains(" KmAtualRodado"))
                    {
                        select.Append("PneuHistorico.PNH_KM_ATUAL_RODADO as KmAtualRodado, ");
                        groupBy.Append("PneuHistorico.PNH_KM_ATUAL_RODADO, ");
                    }
                    break;

                case "Data":
                case "DataFormatada":
                    if (!select.Contains(" Data"))
                    {
                        select.Append("PneuHistorico.PNH_DATA as Data, ");
                        groupBy.Append("PneuHistorico.PNH_DATA, ");
                    }
                    break;

                case "Descricao":
                    if (!select.Contains(" Descricao"))
                    {
                        select.Append("PneuHistorico.PNH_DESCRICAO as Descricao, ");
                        groupBy.Append("PneuHistorico.PNH_DESCRICAO, ");
                    }
                    break;

                case "Servicos":
                    if (!select.Contains(" Servicos"))
                    {
                        select.Append("PneuHistorico.PNH_SERVICOS as Servicos, ");
                        groupBy.Append("PneuHistorico.PNH_SERVICOS, ");
                    }
                    break;

                case "Dimensao":
                    if (!select.Contains(" Dimensao"))
                    {
                        select.Append("DimensaoPneu.PDM_APLICACAO as Dimensao, ");
                        groupBy.Append("DimensaoPneu.PDM_APLICACAO, ");

                        SetarJoinsDimensao(joins);
                    }
                    break;

                case "Marca":
                    if (!select.Contains(" Marca"))
                    {
                        select.Append("MarcaPneu.PMR_DESCRICAO as Marca, ");
                        groupBy.Append("MarcaPneu.PMR_DESCRICAO, ");

                        SetarJoinsMarca(joins);
                    }
                    break;

                case "Modelo":
                    if (!select.Contains(" Modelo"))
                    {
                        select.Append("ModeloPneu.PML_DESCRICAO as Modelo, ");
                        groupBy.Append("ModeloPneu.PML_DESCRICAO, ");

                        SetarJoinsModelo(joins);
                    }
                    break;

                case "NumeroFogo":
                    if (!select.Contains(" NumeroFogo"))
                    {
                        select.Append("Pneu.PNU_NUMERO_FOGO as NumeroFogo, ");
                        groupBy.Append("Pneu.PNU_NUMERO_FOGO, ");

                        SetarJoinsPneu(joins);
                    }
                    break;

                case "Vida":
                case "VidaDescricao":
                    if (!select.Contains(" Vida"))
                    {
                        select.Append("Pneu.PNU_VIDA_ATUAL as Vida, ");
                        groupBy.Append("Pneu.PNU_VIDA_ATUAL, ");

                        SetarJoinsPneu(joins);
                    }
                    break;

                case "ObservacaoSucata":
                    if (!select.Contains(" ObservacaoSucata, "))
                    {
                        select.Append("SucataPneu.PNS_OBSERVACAO as ObservacaoSucata, ");
                        groupBy.Append("SucataPneu.PNS_OBSERVACAO, ");

                        SetarJoinsSucata(joins);
                    }
                    break;

                case "DTO":
                    if (!select.Contains(" DTO"))
                    {
                        select.Append("Pneu.PNU_DTO as DTO, ");
                        groupBy.Append("Pneu.PNU_DTO, ");

                        SetarJoinsPneu(joins);
                    }
                    break;

                case "CustoEstimadoReforma":
                    if (!select.Contains(" CustoEstimadoReforma, "))
                    {
                        select.Append("PneuHistorico.PNH_CUSTO_ESTIMADO CustoEstimadoReforma, ");

                        if (!groupBy.Contains("PneuHistorico.PNH_CUSTO_ESTIMADO"))
                            groupBy.Append("PneuHistorico.PNH_CUSTO_ESTIMADO, ");
                    }
                    break;

                case "DataMovimentacaoFormatada":
                    if (!select.Contains(" DataMovimentacao, "))
                    {
                        select.Append("PneuHistorico.PNH_DATA_HORA_MOVIMENTACAO as DataMovimentacao, ");
                        groupBy.Append("PneuHistorico.PNH_DATA_HORA_MOVIMENTACAO, ");
                    }
                    break;

                case "UsuarioOperador":
                    if (!select.Contains(" UsuarioOperador, "))
                    {
                        select.Append("Usuario.FUN_NOME as UsuarioOperador, ");
                        groupBy.Append("Usuario.FUN_NOME, ");

                        SetarJoinsUsuarioOperador(joins);
                    }
                    break;
                case "MotivoSucata":
                    if (!select.Contains(" MotivoSucata, "))
                    {
                        select.Append("MotivoSucata.PMS_DESCRICAO MotivoSucata, ");
                        groupBy.Append("MotivoSucata.PMS_DESCRICAO, ");

                        SetarJoinsMotivoSucata(joins);
                    }
                    break;
                case "Almoxarifado":
                    if (!select.Contains(" Almoxarifado, "))
                    {
                        select.Append("Almoxarifado.AMX_DESCRICAO Almoxarifado, ");
                        groupBy.Append("Almoxarifado.AMX_DESCRICAO, ");

                        SetarJoinsAlmoxarifado(joins);
                    }
                    break;
                case "TipoAquisicaoDescricao":
                    if (!select.Contains(" TipoAquisicao, "))
                    {
                        select.Append("Pneu.PNU_TIPO_AQUISICAO TipoAquisicao, ");
                        groupBy.Append("Pneu.PNU_TIPO_AQUISICAO, ");

                        SetarJoinsPneu(joins);
                    }
                    break;
                case "ValorResidualAtualPneu":
                    if (!select.Contains(" ValorResidualAtualPneu, "))
                    {
                        select.Append("Reforma.PRR_VALOR_RESIDUAL_ATUAL_PNEU ValorResidualAtualPneu, ");
                        groupBy.Append("Reforma.PRR_VALOR_RESIDUAL_ATUAL_PNEU, ");

                        SetarJoinsRecape(joins);
                    }
                    break;
                case "Sulco":
                case "SulcoGasto":
                case "SulcoAnterior":
                    if (!select.Contains(" SulcoAnterior, ") | !select.Contains(" Sulco, "))
                    {
                        select.Append("Pneu.PNU_SULCO Sulco, ");
                        select.Append("Pneu.PNU_SULCO_ANTERIOR SulcoAnterior, ");
                        groupBy.Append("Pneu.PNU_SULCO, ");
                        groupBy.Append("Pneu.PNU_SULCO_ANTERIOR, ");

                        SetarJoinsPneu(joins);
                    }
                    break;
                case "ObservacaoPneu":
                    if (!select.Contains(" ObservacaoPneu, "))
                    {
                        select.Append("COALESCE(EnvioReforma.PER_OBSERVACAO,RetornoReforma.PRR_OBSERVACAO, '') AS ObservacaoPneu, ");
                        groupBy.Append("COALESCE(EnvioReforma.PER_OBSERVACAO,RetornoReforma.PRR_OBSERVACAO, ''), ");
                        SetarJoinsObservacao(joins);
                    }
                    break;
            }
        }

        protected override void SetarWhere(Dominio.ObjetosDeValor.Embarcador.Frota.FiltroPesquisaPneuHistorico filtrosPesquisa, StringBuilder where, StringBuilder joins, StringBuilder groupBy, List<Embarcador.Consulta.ParametroSQL> parametros = null)
        {
            if (filtrosPesquisa.DataInicio.HasValue)
                where.Append($" and PneuHistorico.PNH_DATA >= '{filtrosPesquisa.DataInicio.Value.ToString("yyyyMMdd HH:mm:ss")}'");

            if (filtrosPesquisa.DataLimite.HasValue)
                where.Append($" and PneuHistorico.PNH_DATA <= '{filtrosPesquisa.DataLimite.Value.Date.Add(DateTime.MaxValue.TimeOfDay).ToString("yyyyMMdd HH:mm:ss")}'");

            if (filtrosPesquisa.CodigoBandaRodagem > 0)
            {
                where.Append($" and Pneu.PBR_CODIGO = {filtrosPesquisa.CodigoBandaRodagem}");

                SetarJoinsPneu(joins);
            }

            if (filtrosPesquisa.CodigoDimensao > 0)
            {
                where.Append($" and ModeloPneu.PDM_CODIGO = {filtrosPesquisa.CodigoDimensao}");

                SetarJoinsModelo(joins);
            }

            if (filtrosPesquisa.CodigoEmpresa > 0)
            {
                where.Append($" and Pneu.EMP_CODIGO = {filtrosPesquisa.CodigoEmpresa}");

                SetarJoinsPneu(joins);
            }

            if (filtrosPesquisa.CodigoMarca > 0)
            {
                where.Append($" and ModeloPneu.PMR_CODIGO = {filtrosPesquisa.CodigoMarca}");

                SetarJoinsModelo(joins);
            }

            if (filtrosPesquisa.CodigoModelo > 0)
            {
                where.Append($" and Pneu.PML_CODIGO = {filtrosPesquisa.CodigoModelo}");

                SetarJoinsPneu(joins);
            }

            if (filtrosPesquisa.CodigoPneu > 0)
                where.Append($" and PneuHistorico.PNU_CODIGO = {filtrosPesquisa.CodigoPneu}");

            if (filtrosPesquisa.Vida.HasValue)
            {
                where.Append($" and Pneu.PNU_VIDA_ATUAL = {(int)filtrosPesquisa.Vida.Value}");

                SetarJoinsPneu(joins);
            }

            if (filtrosPesquisa.SituacaoPneu.HasValue && filtrosPesquisa.SituacaoPneu.Value != Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoPneu.Todos)
            {
                where.Append($" and Pneu.PNU_SITUACAO = {(int)filtrosPesquisa.SituacaoPneu.Value}");

                SetarJoinsPneu(joins);
            }

            if (filtrosPesquisa.SomenteSucata)
                where.Append(" and PneuHistorico.PNH_TIPO = 2");

            if (filtrosPesquisa.CodigoServico > 0)
            {
                where.Append($" and PneuHistorico.PNH_SERVICOS LIKE (SELECT SEV_DESCRICAO FROM T_FROTA_SERVICO_VEICULO WHERE SEV_CODIGO = {filtrosPesquisa.CodigoServico})"); // SQL-INJECTION-SAFE
            }

            if (filtrosPesquisa.CodigoVeiculo > 0)
            {
                where.Append($" and MovimentacaoPneu.VEI_CODIGO = {filtrosPesquisa.CodigoVeiculo}");

                SetarJoinsMovimentacaoVeiculo(joins);
            }

            if (!string.IsNullOrEmpty(filtrosPesquisa.DTO))
            {
                where.Append($" and Pneu.PNU_DTO = '{filtrosPesquisa.DTO}'");

                SetarJoinsPneu(joins);
            }

            if (filtrosPesquisa.CodigoUsuarioOperador > 0)
            {
                where.Append($" and Usuario.FUN_CODIGO = {filtrosPesquisa.CodigoUsuarioOperador}");

                SetarJoinsUsuarioOperador(joins);
            }

            if(filtrosPesquisa.CodigoMotivoSucata.Count > 0)
            {
                where.Append($" and MotivoSucata.PMS_CODIGO in ({string.Join(", ", filtrosPesquisa.CodigoMotivoSucata)})");

                SetarJoinsMotivoSucata(joins);
            }

            if (filtrosPesquisa.CodigoAlmoxarifado.Count > 0)
            {
                where.Append($" and Almoxarifado.AMX_CODIGO in ({string.Join(", ", filtrosPesquisa.CodigoAlmoxarifado)})");

                SetarJoinsAlmoxarifado(joins);
            }

            if (filtrosPesquisa.TiposAquisicao.Count > 0)
            {
                where.Append($" and Pneu.PNU_TIPO_AQUISICAO in ({string.Join(", ", filtrosPesquisa.TiposAquisicao.Select(o => o.ToString("D")))})");

                SetarJoinsPneu(joins);
            }
        }

        #endregion
    }
}
