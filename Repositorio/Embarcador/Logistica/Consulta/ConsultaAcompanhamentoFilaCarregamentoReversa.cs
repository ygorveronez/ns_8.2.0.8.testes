using Dominio.ObjetosDeValor.Embarcador.Enumeradores;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Repositorio.Embarcador.Logistica.Consulta
{
    sealed class ConsultaAcompanhamentoFilaCarregamentoReversa : Repositorio.Embarcador.Consulta.Consulta<Dominio.ObjetosDeValor.Embarcador.Logistica.FiltroPesquisaAcompanhamentoFilaCarregamentoReversa>
    {
        #region Construtores

        public ConsultaAcompanhamentoFilaCarregamentoReversa() : base(tabela: "T_FILA_CARREGAMENTO_VEICULO FilaCarregamentoVeiculo") { }

        #endregion

        #region Métodos Privados

        private void SetarJoinsCentroCarregamento(StringBuilder joins)
        {
            if (!joins.Contains(" CentroCarregamento "))
                joins.Append("left join T_CENTRO_CARREGAMENTO CentroCarregamento on CentroCarregamento.CEC_CODIGO = FilaCarregamentoVeiculo.CEC_CODIGO ");
        }

        private void SetarJoinsConjuntoMotorista(StringBuilder joins)
        {
            if (!joins.Contains(" ConjuntoMotorista "))
                joins.Append("join T_FILA_CARREGAMENTO_CONJUNTO_MOTORISTA ConjuntoMotorista on ConjuntoMotorista.FCM_CODIGO = FilaCarregamentoVeiculo.FCM_CODIGO ");
        }

        private void SetarJoinsConjuntoVeiculo(StringBuilder joins)
        {
            if (!joins.Contains(" ConjuntoVeiculo "))
                joins.Append("join T_FILA_CARREGAMENTO_CONJUNTO_VEICULO ConjuntoVeiculo on ConjuntoVeiculo.FCV_CODIGO = FilaCarregamentoVeiculo.FCV_CODIGO ");
        }

        private void SetarJoinsFilaCarregamentoMotorista(StringBuilder joins)
        {
            SetarJoinsConjuntoMotorista(joins);

            if (!joins.Contains(" FilaCarregamentoMotorista "))
                joins.Append("left join T_FILA_CARREGAMENTO_MOTORISTA FilaCarregamentoMotorista on FilaCarregamentoMotorista.FLM_CODIGO = ConjuntoMotorista.FLM_CODIGO ");
        }

        private void SetarJoinsGrupoModeloVeicularCarga(StringBuilder joins)
        {
            SetarJoinsModeloVeicularCarga(joins);

            if (!joins.Contains(" GrupoModeloVeicularCarga "))
                joins.Append("left join T_MODELO_VEICULAR_CARGA_GRUPO GrupoModeloVeicularCarga on GrupoModeloVeicularCarga.MVG_CODIGO = ModeloVeicularCarga.MVG_CODIGO ");
        }

        private void SetarJoinsModeloVeicularCarga(StringBuilder joins)
        {
            SetarJoinsConjuntoVeiculo(joins);

            if (!joins.Contains(" ModeloVeicularCarga "))
                joins.Append("join T_MODELO_VEICULAR_CARGA ModeloVeicularCarga on ModeloVeicularCarga.MVC_CODIGO = ConjuntoVeiculo.MVC_CODIGO ");
        }

        private void SetarJoinsMotorista(StringBuilder joins)
        {
            SetarJoinsConjuntoMotorista(joins);

            if (!joins.Contains(" Motorista "))
                joins.Append("left join T_FUNCIONARIO Motorista on Motorista.FUN_CODIGO = ConjuntoMotorista.FCM_CODIGO_MOTORISTA ");
        }

        private void SetarJoinsTransportador(StringBuilder joins)
        {
            SetarJoinsMotorista(joins);

            if (!joins.Contains(" Transportador "))
                joins.Append("left join T_EMPRESA Transportador on Transportador.EMP_CODIGO = Motorista.EMP_CODIGO ");
        }

        private void SetarJoinsVeiculoTracao(StringBuilder joins)
        {
            SetarJoinsConjuntoVeiculo(joins);

            if (!joins.Contains(" VeiculoTracao "))
                joins.Append("left join T_VEICULO VeiculoTracao on VeiculoTracao.VEI_CODIGO = ConjuntoVeiculo.FCV_CODIGO_TRACAO ");
        }

        #endregion

        #region Métodos Protegidos Sobrescritos

        protected override void SetarSelect(string propriedade, int codigoDinamico, StringBuilder select, StringBuilder joins, StringBuilder groupBy, bool somenteContarNumeroRegistros, Dominio.ObjetosDeValor.Embarcador.Logistica.FiltroPesquisaAcompanhamentoFilaCarregamentoReversa filtroPesquisa)
        {
            switch (propriedade)
            {
                case "Codigo":
                    if (!select.Contains(" Codigo"))
                    {
                        select.Append("FilaCarregamentoVeiculo.FLV_CODIGO as Codigo, ");
                        groupBy.Append("FilaCarregamentoVeiculo.FLV_CODIGO, ");
                    }
                    break;

                case "DataEntradaFila":
                case "DataEntradaFilaFormatada":
                    if (!select.Contains(" DataEntradaFila"))
                    {
                        select.Append("FilaCarregamentoVeiculo.FLV_DATA_ENTRADA as DataEntradaFila, ");
                        groupBy.Append("FilaCarregamentoVeiculo.FLV_DATA_ENTRADA, ");
                    }
                    break;

                case "DescricaoCentroCarregamento":
                    if (!select.Contains(" DescricaoCentroCarregamento"))
                    {
                        select.Append("CentroCarregamento.CEC_DESCRICAO as DescricaoCentroCarregamento, ");
                        groupBy.Append("CentroCarregamento.CEC_DESCRICAO, ");

                        SetarJoinsCentroCarregamento(joins);
                    }
                    break;

                case "DescricaoGrupoModeloVeicularCarga":
                    if (!select.Contains(" DescricaoGrupoModeloVeicularCarga"))
                    {
                        select.Append("GrupoModeloVeicularCarga.MVG_DESCRICAO as DescricaoGrupoModeloVeicularCarga, ");
                        groupBy.Append("GrupoModeloVeicularCarga.MVG_DESCRICAO, ");

                        SetarJoinsGrupoModeloVeicularCarga(joins);
                    }
                    break;

                case "DescricaoModeloVeicularCarga":
                    if (!select.Contains(" DescricaoModeloVeicularCarga"))
                    {
                        select.Append("ModeloVeicularCarga.MVC_DESCRICAO as DescricaoModeloVeicularCarga, ");
                        groupBy.Append("ModeloVeicularCarga.MVC_DESCRICAO, ");

                        SetarJoinsModeloVeicularCarga(joins);
                    }
                    break;

                case "LojaProximidade":
                case "LojaProximidadeDescricao":
                    if (!select.Contains(" LojaProximidade"))
                    {
                        select.Append("isnull(FilaCarregamentoMotorista.FLM_LOJA_PROXIMIDADE, 0) LojaProximidade, ");
                        groupBy.Append("isnull(FilaCarregamentoMotorista.FLM_LOJA_PROXIMIDADE, 0), ");

                        SetarJoinsFilaCarregamentoMotorista(joins);
                    }
                    break;

                case "DistanciaCentroCarregamento":
                case "DistanciaCentroCarregamentoFormatada":
                    if (!select.Contains(" DistanciaCentroCarregamento"))
                    {
                        select.Append("isnull(FilaCarregamentoMotorista.FLM_DISTANCIA_CENTRO_CARREGAMENTO, 0) DistanciaCentroCarregamento, ");
                        groupBy.Append("isnull(FilaCarregamentoMotorista.FLM_DISTANCIA_CENTRO_CARREGAMENTO, 0), ");

                        SetarJoinsFilaCarregamentoMotorista(joins);
                    }
                    break;

                case "NomeMotorista":
                    if (!select.Contains(" NomeMotorista"))
                    {
                        select.Append("Motorista.FUN_NOME as NomeMotorista, ");
                        groupBy.Append("Motorista.FUN_NOME, ");

                        SetarJoinsMotorista(joins);
                    }
                    break;

                case "NomeTransportador":
                    if (!select.Contains(" NomeTransportador"))
                    {
                        select.Append("Transportador.EMP_RAZAO as NomeTransportador, ");
                        groupBy.Append("Transportador.EMP_RAZAO, ");

                        SetarJoinsTransportador(joins);
                    }
                    break;

                case "Reboques":
                    if (!select.Contains(" Reboques"))
                    {
                        select.Append(
                            @"isnull(SUBSTRING((
                                select ',' + Reboque.VEI_PLACA
                                  from T_FILA_CARREGAMENTO_CONJUNTO_VEICULO_REBOQUE ConjuntoVeiculoReboque
                                  join T_VEICULO Reboque on Reboque.VEI_CODIGO = ConjuntoVeiculoReboque.VEI_CODIGO
                                 where ConjuntoVeiculoReboque.FCV_CODIGO_REBOQUE = ConjuntoVeiculo.FCV_CODIGO
                                   for XML PATH('')
                            ), 3, 1000), '') Reboques, "
                        );

                        groupBy.Append("ConjuntoVeiculo.FCV_CODIGO, ");

                        SetarJoinsConjuntoVeiculo(joins);
                    }
                    break;

                case "TelefoneMotorista":
                    if (!select.Contains(" TelefoneMotorista"))
                    {
                        select.Append("Motorista.FUN_FONE as TelefoneMotorista, ");
                        groupBy.Append("Motorista.FUN_FONE, ");

                        SetarJoinsMotorista(joins);
                    }
                    break;

                case "Tracao":
                    if (!select.Contains(" Tracao"))
                    {
                        select.Append("VeiculoTracao.VEI_PLACA as Tracao, ");
                        groupBy.Append("VeiculoTracao.VEI_PLACA, ");

                        SetarJoinsVeiculoTracao(joins);
                    }
                    break;
            }
        }

        protected override void SetarWhere(Dominio.ObjetosDeValor.Embarcador.Logistica.FiltroPesquisaAcompanhamentoFilaCarregamentoReversa filtrosPesquisa, StringBuilder where, StringBuilder joins, StringBuilder groupBy, List<Embarcador.Consulta.ParametroSQL> parametros = null)
        {
            where.Append($" and FilaCarregamentoVeiculo.FLV_SITUACAO = {(int)SituacaoFilaCarregamentoVeiculo.Disponivel} ");
            where.Append($" and FilaCarregamentoVeiculo.FLV_TIPO = {(int)TipoFilaCarregamentoVeiculo.Reversa} ");

            if (filtrosPesquisa.CodigosCentroCarregamento?.Count > 0)
                where.Append($" and FilaCarregamentoVeiculo.CEC_CODIGO in ({string.Join(",", filtrosPesquisa.CodigosCentroCarregamento)}) ");

            if (filtrosPesquisa.LojaProximidade.HasValue)
            {
                SetarJoinsFilaCarregamentoMotorista(joins);

                where.Append($" and FilaCarregamentoMotorista.FLM_LOJA_PROXIMIDADE = {(filtrosPesquisa.LojaProximidade.Value ? 1 : 0)}");
            }
        }

        #endregion
    }
}
