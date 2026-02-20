using Dominio.ObjetosDeValor.Embarcador.Enumeradores;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Repositorio.Embarcador.Frota
{
    sealed class ConsultaSinistro : Consulta.Consulta<Dominio.ObjetosDeValor.Embarcador.Frota.FiltroRelatorioSinistro>
    {
        #region Construtores

        public ConsultaSinistro() : base(tabela: "T_SINISTRO_DADOS as Sinistro") { }

        #endregion

        #region Métodos Privados

        #endregion
        private void SetarJoinsTipoSinistro(StringBuilder joins)
        {
            if (!joins.Contains(" TipoSinistro "))
                joins.Append(" left join T_TIPO_SINISTRO TipoSinistro on Sinistro.TSI_CODIGO = TipoSinistro.TSI_CODIGO ");
        }
        private void SetarJoinsLocalidade(StringBuilder joins)
        {
            if (!joins.Contains(" Cidade "))
                joins.Append(" left join T_LOCALIDADES Cidade on Sinistro.LOC_CODIGO = Cidade.LOC_CODIGO ");
        }
        private void SetarJoinsVeiculo(StringBuilder joins)
        {
            if (!joins.Contains(" Veiculo "))
                joins.Append(" left join T_VEICULO Veiculo on Sinistro.VEI_CODIGO = Veiculo.VEI_CODIGO ");
        }
        private void SetarJoinsVeiculoReboque(StringBuilder joins)
        {
            if (!joins.Contains(" VeiculoReboque "))
                joins.Append(" left join T_VEICULO VeiculoReboque on Sinistro.VEI_REBOQUE_CODIGO = VeiculoReboque.VEI_CODIGO ");
        }
        private void SetarJoinsMotorista(StringBuilder joins)
        {
            if (!joins.Contains(" Motorista "))
                joins.Append(" left join T_FUNCIONARIO Motorista on Sinistro.FUN_CODIGO = Motorista.FUN_CODIGO ");
        }
        private void SetarJoinsEtapaDocumentacao(StringBuilder joins)
        {
            if (!joins.Contains(" DocumentacaoEnvolvido "))
                joins.Append(" left join T_SINISTRO_DOCUMENTACAO_ENVOLVIDOS DocumentacaoEnvolvido on Sinistro.SDS_CODIGO = DocumentacaoEnvolvido.SDS_CODIGO ");
        }
        private void SetarJoinsOrdemServico(StringBuilder joins)
        {
            if (!joins.Contains(" OrdemServico "))
                joins.Append(" left join T_FROTA_ORDEM_SERVICO OrdemServico on Sinistro.OSE_CODIGO = OrdemServico.OSE_CODIGO ");
        }
        private void SetarJoinsLocalManutencao(StringBuilder joins)
        {
            if (!joins.Contains(" LocalManutencao "))
                joins.Append(" left join T_CLIENTE LocalManutencao on Sinistro.CLI_CGCCPF_LOCAL_MANUTENCAO = LocalManutencao.CLI_CGCCPF ");
        }
        private void SetarJoinsMovimento(StringBuilder joins)
        {
            if (!joins.Contains(" Movimento "))
                joins.Append(" left join T_TIPO_MOVIMENTO Movimento on Sinistro.TIM_CODIGO = Movimento.TIM_CODIGO ");
        }
        private void SetarJoinsPessoa(StringBuilder joins)
        {
            if (!joins.Contains(" Pessoa "))
                joins.Append(" left join T_CLIENTE Pessoa on Sinistro.CLI_CGCCPF_TITULO = Pessoa.CLI_CGCCPF ");
        }
        private void SetarJoinsDocumentoEntrada(StringBuilder joins)
        {
            if (!joins.Contains(" DocumentoEntrada "))
                joins.Append(@" left join T_SINISTRO_NOTA SinistroNota on Sinistro.SDS_CODIGO = SinistroNota.SDS_CODIGO
                                left join T_TMS_DOCUMENTO_ENTRADA DocumentoEntrada on SinistroNota.TDE_CODIGO = DocumentoEntrada.TDE_CODIGO ");
        }
        private void SetarJoinsSituacaoSinistro(StringBuilder joins)
        {
            if (!joins.Contains(" SituacaoSinistro "))
                joins.Append(@" left join T_SINISTRO_HISTORICO SituacaoSinistro on Sinistro.SDS_CODIGO = SituacaoSinistro.SDS_CODIGO ");
        }

        protected override void SetarSelect(string propriedade, int codigoDinamico, StringBuilder select, StringBuilder joins, StringBuilder groupBy, bool somenteContarNumeroRegistros, Dominio.ObjetosDeValor.Embarcador.Frota.FiltroRelatorioSinistro filtroPesquisa)
        {

            if (!select.Contains(" Codigo,"))
            {
                select.Append("Sinistro.SDS_CODIGO as Codigo, ");
                groupBy.Append("Sinistro.SDS_CODIGO, ");
            }
            switch (propriedade)
            {
                case "NumeroSinistro":
                    if (!select.Contains(" NumeroSinistro,"))
                    {
                        select.Append("Sinistro.SDS_NUMERO as NumeroSinistro, ");
                        groupBy.Append("Sinistro.SDS_NUMERO, ");
                    }
                    break;

                case "CausadorSinistroDescricao":
                    if (!select.Contains(" CausadorSinistro,"))
                    {
                        select.Append("Sinistro.SDS_CAUSADOR_SINISTRO as CausadorSinistro, ");
                        groupBy.Append("Sinistro.SDS_CAUSADOR_SINISTRO, ");
                    }
                    break;

                case "TipoSinistro":
                    if (!select.Contains(" TipoSinistro,"))
                    {
                        select.Append("TipoSinistro.TSI_DESCRICAO as TipoSinistro, ");
                        groupBy.Append("TipoSinistro.TSI_DESCRICAO, ");

                        SetarJoinsTipoSinistro(joins);
                    }
                    break;

                case "NumeroBoletimOcorrencia":
                    if (!select.Contains(" NumeroBoletimOcorrencia,"))
                    {
                        select.Append("Sinistro.SDS_NUMERO_BOLETIM_OCORRENCIA as NumeroBoletimOcorrencia, ");
                        groupBy.Append("Sinistro.SDS_NUMERO_BOLETIM_OCORRENCIA, ");
                    }
                    break;

                case "DataEmissaoDados":
                    if (!select.Contains(" DataEmissaoDados,"))
                    {
                        select.Append("Sinistro.SDS_DATA_EMISSAO as DataEmissaoDados, ");
                        groupBy.Append("Sinistro.SDS_DATA_EMISSAO, ");
                    }
                    break;

                case "DataHoraSinistro":
                    if (!select.Contains(" DataHoraSinistro,"))
                    {
                        select.Append("Sinistro.SDS_DATA_SINISTRO as DataHoraSinistro, ");
                        groupBy.Append("Sinistro.SDS_DATA_SINISTRO, ");
                    }
                    break;

                case "Endereco":
                    if (!select.Contains(" Endereco,"))
                    {
                        select.Append("Sinistro.SDS_ENDERECO as Endereco, ");
                        groupBy.Append("Sinistro.SDS_ENDERECO, ");
                    }
                    break;

                case "Local":
                    if (!select.Contains(" Local,"))
                    {
                        select.Append("Sinistro.SDS_LOCAL as Local, ");
                        groupBy.Append("Sinistro.SDS_LOCAL, ");
                    }
                    break;

                case "Cidade":
                    if (!select.Contains(" Cidade,"))
                    {
                        select.Append("Cidade.LOC_DESCRICAO as Cidade, ");
                        groupBy.Append("Cidade.LOC_DESCRICAO, ");

                        SetarJoinsLocalidade(joins);
                    }
                    break;

                case "PlacaCavalo":
                    if (!select.Contains(" PlacaCavalo,"))
                    {
                        select.Append("Veiculo.VEI_PLACA as PlacaCavalo, ");
                        groupBy.Append("Veiculo.VEI_PLACA, ");

                        SetarJoinsVeiculo(joins);
                    }
                    break;

                case "PlacaReboque":
                    if (!select.Contains(" PlacaReboque,"))
                    {
                        select.Append("VeiculoReboque.VEI_PLACA as PlacaCavalo, ");
                        groupBy.Append("VeiculoReboque.VEI_PLACA, ");

                        SetarJoinsVeiculoReboque(joins);
                    }
                    break;

                case "Motorista":
                    if (!select.Contains(" Motorista,"))
                    {
                        select.Append("Motorista.FUN_NOME as Motorista, ");
                        groupBy.Append("Motorista.FUN_NOME, ");

                        SetarJoinsMotorista(joins);
                    }
                    break;

                case "TipoDescricao":
                    if (!select.Contains(" Tipo,"))
                    {
                        select.Append("DocumentacaoEnvolvido.SDC_TIPO_ENVOLVIDO as Tipo, ");
                        groupBy.Append("DocumentacaoEnvolvido.SDC_TIPO_ENVOLVIDO, ");

                        SetarJoinsEtapaDocumentacao(joins);
                    }
                    break;

                case "Nome":
                    if (!select.Contains(" Nome,"))
                    {
                        select.Append("DocumentacaoEnvolvido.SDC_NOME as Nome, ");
                        groupBy.Append("DocumentacaoEnvolvido.SDC_NOME, ");

                        SetarJoinsEtapaDocumentacao(joins);
                    }
                    break;

                case "CPF":
                    if (!select.Contains(" CPF,"))
                    {
                        select.Append("DocumentacaoEnvolvido.SDC_CPF as CPF, ");
                        groupBy.Append("DocumentacaoEnvolvido.SDC_CPF, ");

                        SetarJoinsEtapaDocumentacao(joins);
                    }
                    break;

                case "TelefoneContato1":
                    if (!select.Contains(" TelefoneContato1,"))
                    {
                        select.Append("DocumentacaoEnvolvido.SDC_FONE_PRINCIPAL as TelefoneContato1, ");
                        groupBy.Append("DocumentacaoEnvolvido.SDC_FONE_PRINCIPAL, ");

                        SetarJoinsEtapaDocumentacao(joins);
                    }
                    break;

                case "TelefoneContato2":
                    if (!select.Contains(" TelefoneContato2,"))
                    {
                        select.Append("DocumentacaoEnvolvido.SDC_FONE_SECUNDARIO as TelefoneContato2, ");
                        groupBy.Append("DocumentacaoEnvolvido.SDC_FONE_SECUNDARIO, ");

                        SetarJoinsEtapaDocumentacao(joins);
                    }
                    break;

                case "Veiculo":
                    if (!select.Contains(" Veiculo,"))
                    {
                        select.Append("DocumentacaoEnvolvido.SDC_VEICULO as Veiculo, ");
                        groupBy.Append("DocumentacaoEnvolvido.SDC_VEICULO, ");

                        SetarJoinsEtapaDocumentacao(joins);
                    }
                    break;
                case "Observacao":
                    if (!select.Contains(" Observacao,"))
                    {
                        select.Append("DocumentacaoEnvolvido.SDC_OBSERVACAO as Observacao, ");
                        groupBy.Append("DocumentacaoEnvolvido.SDC_OBSERVACAO, ");

                        SetarJoinsEtapaDocumentacao(joins);
                    }
                    break;

                case "NumeroOrdemServico":
                    if (!select.Contains(" NumeroOrdemServico,"))
                    {
                        select.Append("OrdemServico.OSE_NUMERO as NumeroOrdemServico, ");
                        groupBy.Append("OrdemServico.OSE_NUMERO, ");

                        SetarJoinsOrdemServico(joins);
                    }
                    break;

                case "LocalManutencao":
                    if (!select.Contains(" LocalManutencao,"))
                    {
                        select.Append("LocalManutencao.CLI_NOME as LocalManutencao, ");
                        groupBy.Append("LocalManutencao.CLI_NOME, ");

                        SetarJoinsLocalManutencao(joins);
                    }
                    break;

                case "SituacaoOSDescricao":
                    if (!select.Contains(" SituacaoOS,"))
                    {
                        select.Append("OrdemServico.OSE_SITUACAO as SituacaoOS, ");
                        groupBy.Append("OrdemServico.OSE_SITUACAO, ");

                        SetarJoinsOrdemServico(joins);
                    }
                    break;

                case "IndicacaoPagadorDescricao":
                    if (!select.Contains(" IndicacaoPagador,"))
                    {
                        select.Append("Sinistro.SDS_INDICADOR_PAGADOR as IndicacaoPagador, ");
                        groupBy.Append("Sinistro.SDS_INDICADOR_PAGADOR, ");
                    }
                    break;

                case "Movimento":
                    if (!select.Contains(" Movimento,"))
                    {
                        select.Append("Movimento.TIM_DESCRICAO as Movimento, ");
                        groupBy.Append("Movimento.TIM_DESCRICAO, ");

                        SetarJoinsMovimento(joins);
                    }
                    break;

                case "Pessoa":
                    if (!select.Contains(" Pessoa,"))
                    {
                        select.Append("Pessoa.CLI_NOME as Pessoa, ");
                        groupBy.Append("Pessoa.CLI_NOME, ");

                        SetarJoinsPessoa(joins);
                    }
                    break;

                case "DataEmissaoTitulo":
                    if (!select.Contains(" DataEmissaoTitulo,"))
                    {
                        select.Append("Sinistro.SDS_DATA_EMISSAO_TITULO as DataEmissaoTitulo, ");
                        groupBy.Append("Sinistro.SDS_DATA_EMISSAO_TITULO, ");
                    }
                    break;

                case "DataVencimentoTitulo":
                    if (!select.Contains(" DataVencimentoTitulo,"))
                    {
                        select.Append("Sinistro.SDS_DATA_VENCIMENTO_TITULO as DataVencimentoTitulo, ");
                        groupBy.Append("Sinistro.SDS_DATA_VENCIMENTO_TITULO, ");
                    }
                    break;

                case "NumeroDocumento":
                    if (!select.Contains(" NumeroDocumento,"))
                    {
                        select.Append("Sinistro.SDS_NUMERO_DOCUMENTO_TITULO as NumeroDocumento, ");
                        groupBy.Append("Sinistro.SDS_NUMERO_DOCUMENTO_TITULO, ");
                    }
                    break;

                case "ValorOriginal":
                    if (!select.Contains(" ValorOriginal,"))
                    {
                        select.Append("Sinistro.SDS_VALOR_ORIGINAL_TITULO as ValorOriginal, ");
                        groupBy.Append("Sinistro.SDS_VALOR_ORIGINAL_TITULO, ");
                    }
                    break;

                case "ObservacaoTitulo":
                    if (!select.Contains(" ObservacaoTitulo,"))
                    {
                        select.Append("Sinistro.SDS_OBSERVACAO_TITULO as ObservacaoTitulo, ");
                        groupBy.Append("Sinistro.SDS_OBSERVACAO_TITULO, ");
                    }
                    break;

                case "DocumentoEntrada":
                    if (!select.Contains(" DocumentoEntrada,"))
                    {
                        select.Append("DocumentoEntrada.TDE_NUMERO as DocumentoEntrada, ");
                        groupBy.Append("DocumentoEntrada.TDE_NUMERO, ");

                        SetarJoinsDocumentoEntrada(joins);
                    }
                    break;
                case "SituacaoSinistroDescricao":
                    if (!select.Contains(" SituacaoSinistro,"))
                    {
                        select.Append("SituacaoSinistro.SHC_TIPO as SituacaoSinistro, ");
                        groupBy.Append("SituacaoSinistro.SHC_TIPO, ");

                        SetarJoinsSituacaoSinistro(joins);
                    }
                    break;
            }
        }

        protected override void SetarWhere(Dominio.ObjetosDeValor.Embarcador.Frota.FiltroRelatorioSinistro filtrosPesquisa, StringBuilder where, StringBuilder joins, StringBuilder groupBy, List<Embarcador.Consulta.ParametroSQL> parametros = null)
        {
            string pattern = "yyyy/MM/dd";

            if (filtrosPesquisa.NumeroSinistro > 0)
                where.Append($" AND Sinistro.SDS_NUMERO = '{filtrosPesquisa.NumeroSinistro}'");

            if (filtrosPesquisa.CausadorSinistro != CausadorSinistro.Todos)
                where.Append($" AND Sinistro.SDS_CAUSADOR_SINISTRO = {(int)filtrosPesquisa.CausadorSinistro}");

            if (filtrosPesquisa.CodigoTipoSinistro > 0)
            {
                where.Append($" AND TipoSinistro.TSI_CODIGO = {(int)filtrosPesquisa.CodigoTipoSinistro}");
                SetarJoinsTipoSinistro(joins);
            }

            if (!string.IsNullOrEmpty(filtrosPesquisa.NumeroBoletimOcorrencia))
                where.Append($" AND Sinistro.SDS_NUMERO_BOLETIM_OCORRENCIA = '{filtrosPesquisa.NumeroBoletimOcorrencia}'");

            if (filtrosPesquisa.DataSinistroInicial != DateTime.MinValue)
                where.Append($" AND Sinistro.SDS_DATA_EMISSAO >= '{filtrosPesquisa.DataSinistroInicial.ToString(pattern)}'");

            if (filtrosPesquisa.DataSinistroFinal != DateTime.MinValue)
                where.Append($" AND Sinistro.SDS_DATA_EMISSAO <= '{filtrosPesquisa.DataSinistroFinal.ToString(pattern)}'");

            if (filtrosPesquisa.CodigoCidade > 0)
            {
                where.Append($" AND Cidade.LOC_CODIGO = {filtrosPesquisa.CodigoCidade}");
                SetarJoinsLocalidade(joins);
            }

            if (filtrosPesquisa.CodigoVeiculo > 0)
            {
                where.Append($" AND Veiculo.VEI_CODIGO = {filtrosPesquisa.CodigoVeiculo}");
                SetarJoinsVeiculo(joins);
            }

            if (filtrosPesquisa.CodigoVeiculoReboque > 0)
            {
                where.Append($" AND VeiculoReboque.VEI_CODIGO = {filtrosPesquisa.CodigoVeiculoReboque}");
                SetarJoinsVeiculoReboque(joins);
            }

            if (filtrosPesquisa.CodigoMotorista > 0)
            {
                where.Append($" AND Motorista.FUN_CODIGO = '{filtrosPesquisa.CodigoMotorista}'");
                SetarJoinsMotorista(joins);
            }

            if (filtrosPesquisa.NumeroOrdemServico > 0)
            {
                where.Append($" AND OrdemServico.OSE_NUMERO = {filtrosPesquisa.NumeroOrdemServico}");
                SetarJoinsOrdemServico(joins);
            }

            if (filtrosPesquisa.IndicacaoPagador.HasValue)
                where.Append($" AND Sinistro.SDS_INDICADOR_PAGADOR = {(int)filtrosPesquisa.IndicacaoPagador.Value}");

            if (filtrosPesquisa.SituacaoSinistro.HasValue)
            {
                where.Append($" AND SituacaoSinistro.SHC_TIPO = {(int)filtrosPesquisa.SituacaoSinistro.Value}");
                SetarJoinsSituacaoSinistro(joins);
            }
        }

    }

}
