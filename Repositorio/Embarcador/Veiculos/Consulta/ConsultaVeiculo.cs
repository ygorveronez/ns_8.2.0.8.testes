using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Repositorio.Embarcador.Veiculos.Consulta
{
    sealed class ConsultaVeiculo : Embarcador.Consulta.Consulta<Dominio.ObjetosDeValor.Embarcador.Veiculos.FiltroPesquisaRelatorioVeiculo>
    {
        #region Construtores

        public ConsultaVeiculo() : base(tabela: "T_VEICULO as Veiculo") { }

        #endregion Construtores

        #region Métodos Privados

        private void SetarJoinsModeloVeicularCarga(StringBuilder joins)
        {
            if (!joins.Contains(" ModeloVeicularCarga "))
                joins.Append(" left join T_MODELO_VEICULAR_CARGA ModeloVeicularCarga on ModeloVeicularCarga.MVC_CODIGO = Veiculo.MVC_CODIGO ");
        }

        private void SetarJoinsSegmentoVeiculo(StringBuilder joins)
        {
            if (!joins.Contains(" SegmentoVeiculo "))
                joins.Append(" left join T_VEICULO_SEGMENTO SegmentoVeiculo on SegmentoVeiculo.VSE_CODIGO = Veiculo.VSE_CODIGO ");
        }

        private void SetarJoinsEmpresa(StringBuilder joins)
        {
            if (!joins.Contains(" Empresa "))
                joins.Append(" left join T_EMPRESA Empresa on Empresa.EMP_CODIGO = Veiculo.EMP_CODIGO ");
        }

        private void SetarJoinsProprietario(StringBuilder joins)
        {
            if (!joins.Contains(" Proprietario "))
                joins.Append(" left join T_CLIENTE Proprietario on Proprietario.CLI_CGCCPF = Veiculo.VEI_PROPRIETARIO ");
        }

        private void SetarJoinsLocador(StringBuilder joins)
        {
            if (!joins.Contains(" Locador "))
                joins.Append(" left join T_CLIENTE Locador on Locador.CLI_CGCCPF = Veiculo.VEI_LOCADOR ");
        }

        private void SetarJoinsTecnologiaRastreador(StringBuilder joins)
        {
            if (!joins.Contains(" TecnologiaRastreador "))
                joins.Append(" left join T_RASTREADOR_TECNOLOGIA TecnologiaRastreador on TecnologiaRastreador.TRA_CODIGO = Veiculo.TRA_CODIGO ");
        }

        private void SetarJoinsTipoComunicacaoRastreador(StringBuilder joins)
        {
            if (!joins.Contains(" TipoComunicacaoRastreador "))
                joins.Append(" left join T_RASTREADOR_TIPO_COMUNICACAO TipoComunicacaoRastreador on TipoComunicacaoRastreador.TCR_CODIGO = Veiculo.TCR_CODIGO ");
        }

        private void SetarJoinsEstado(StringBuilder joins)
        {
            if (!joins.Contains(" Estado "))
                joins.Append(" left join T_UF Estado on Estado.UF_SIGLA = Veiculo.UF_SIGLA ");
        }

        private void SetarJoinsLocalAtual(StringBuilder joins)
        {
            if (!joins.Contains(" LocalAtual "))
                joins.Append(" left join T_AREA_VEICULO_POSICAO LocalAtual on LocalAtual.AVP_CODIGO = Veiculo.AVP_CODIGO ");
        }

        private void SetarJoinsAreaVeiculo(StringBuilder joins)
        {
            SetarJoinsLocalAtual(joins);

            if (!joins.Contains(" AreaVeiculo "))
                joins.Append(" left join T_AREA_VEICULO AreaVeiculo on AreaVeiculo.ARV_CODIGO = LocalAtual.ARV_CODIGO ");
        }

        private void SetarJoinsFuncionarioResponsavel(StringBuilder joins)
        {
            if (!joins.Contains(" FuncionarioResponsavel "))
                joins.Append(" left join T_FUNCIONARIO FuncionarioResponsavel on FuncionarioResponsavel.FUN_CODIGO = Veiculo.FUN_CODIGO_RESPONSAVEL ");
        }

        private void SetarJoinsModeloVeiculo(StringBuilder joins)
        {
            if (!joins.Contains(" ModeloVeiculo "))
                joins.Append(" left join T_VEICULO_MODELO ModeloVeiculo on ModeloVeiculo.VMO_CODIGO = Veiculo.VMO_CODIGO ");
        }

        private void SetarJoinsMarcaVeiculo(StringBuilder joins)
        {
            if (!joins.Contains(" MarcaVeiculo "))
                joins.Append(" left join T_VEICULO_MARCA MarcaVeiculo on MarcaVeiculo.VMA_CODIGO = Veiculo.VMA_CODIGO ");
        }

        private void SetarJoinsGrupoPessoas(StringBuilder joins)
        {
            if (!joins.Contains(" GrupoPessoas "))
                joins.Append(" left join T_GRUPO_PESSOAS GrupoPessoas on GrupoPessoas.GRP_CODIGO = Veiculo.GRP_CODIGO ");
        }

        private void SetarJoinsCentroResultado(StringBuilder joins)
        {
            if (!joins.Contains(" CentroResultado "))
                joins.Append(" left join T_CENTRO_RESULTADO CentroResultado on CentroResultado.CRE_CODIGO = Veiculo.CRE_CODIGO ");
        }

        private void SetarJoinsModeloCarroceria(StringBuilder joins)
        {
            if (!joins.Contains(" ModeloCarroceria "))
                joins.Append(" LEFT OUTER JOIN T_MODELO_CARROCERIA ModeloCarroceria ON ModeloCarroceria.MCA_CODIGO = Veiculo.MCA_CODIGO ");
        }

        #endregion Métodos Privados

        #region Métodos Protegidos Sobrescritos

        protected override void SetarSelect(string propriedade, int codigoDinamico, StringBuilder select, StringBuilder joins, StringBuilder groupBy, bool somenteContarNumeroRegistros, Dominio.ObjetosDeValor.Embarcador.Veiculos.FiltroPesquisaRelatorioVeiculo filtroPesquisa)
        {
            if (!select.Contains(" Codigo, "))
            {
                select.Append("Veiculo.VEI_CODIGO as Codigo, ");
                groupBy.Append("Veiculo.VEI_CODIGO, ");
            }

            switch (propriedade)
            {
                case "Placa":
                    if (!select.Contains(" Placa, "))
                    {
                        select.Append("Veiculo.VEI_PLACA as Placa, ");
                        groupBy.Append("Veiculo.VEI_PLACA, ");
                    }
                    break;

                case "Frota":
                    if (!select.Contains(" Frota, "))
                    {
                        select.Append("Veiculo.VEI_NUMERO_FROTA as Frota, ");
                        groupBy.Append("Veiculo.VEI_NUMERO_FROTA, ");
                    }
                    break;

                case "Situacao":
                    if (!select.Contains(" Situacao, "))
                    {
                        select.Append(@"   CASE WHEN Veiculo.VEI_ATIVO = 1 THEN 'Ativo' 
                                            ELSE 'Inativo' 
                                            END as Situacao, ");
                        groupBy.Append("Veiculo.VEI_ATIVO, ");
                    }
                    break;

                case "RENAVAM":
                    if (!select.Contains(" RENAVAM, "))
                    {
                        select.Append("Veiculo.VEI_RENAVAM as RENAVAM, ");
                        groupBy.Append("Veiculo.VEI_RENAVAM, ");
                    }
                    break;
                case "NumeroEquipamentoRastreador":
                    if (!select.Contains(" NumeroEquipamentoRastreador, "))
                    {
                        select.Append("Veiculo.VEI_NUMERO_EQUIPAMENTO_RASTREADOR as NumeroEquipamentoRastreador, ");
                        groupBy.Append("Veiculo.VEI_NUMERO_EQUIPAMENTO_RASTREADOR, ");
                    }
                    break;

                case "DataAtualizacaoFormatada":
                    if (!select.Contains(" DataAtualizacao, "))
                    {
                        select.Append("Veiculo.VEI_DATA_ATULIZACAO as DataAtualizacao, ");
                        groupBy.Append("Veiculo.VEI_DATA_ATULIZACAO, ");
                    }
                    break;

                case "DataValidadeGerenciadoraRiscoFormatada":
                    if (!select.Contains(" DataValidadeGerenciadoraRisco, "))
                    {
                        select.Append("Veiculo.VEI_DATA_VALIDADE_GERENCIADORA_RISCO as DataValidadeGerenciadoraRisco, ");
                        groupBy.Append("Veiculo.VEI_DATA_VALIDADE_GERENCIADORA_RISCO, ");
                    }
                    break;

                case "DataValidadeLiberacaoSeguradoraFormatada":
                    if (!select.Contains(" DataValidadeLiberacaoSeguradora, "))
                    {
                        select.Append("Veiculo.VEI_DATA_VALIDADE_LIBERACAO_SEGURADORA as DataValidadeLiberacaoSeguradora, ");
                        groupBy.Append("Veiculo.VEI_DATA_VALIDADE_LIBERACAO_SEGURADORA, ");
                    }
                    break;

                case "Propriedade":
                    if (!select.Contains(" Propriedade, "))
                    {
                        select.Append(@"   CASE WHEN Veiculo.VEI_TIPO = 'P' THEN 'Próprio' 
                                                WHEN Veiculo.VEI_TIPO = 'T' THEN 'Terceiro' 
                                            ELSE '' 
                                            END as Propriedade, ");
                        groupBy.Append("Veiculo.VEI_TIPO, ");
                    }
                    break;

                case "TipoVeiculo":
                    if (!select.Contains(" TipoVeiculo, "))
                    {
                        select.Append(@"   CASE WHEN Veiculo.VEI_TIPOVEICULO = '0' THEN 'Tração' 
                                                WHEN Veiculo.VEI_TIPOVEICULO = '1' THEN 'Reboque' 
                                            ELSE '' 
                                            END as TipoVeiculo, ");
                        groupBy.Append("Veiculo.VEI_TIPOVEICULO, ");
                    }
                    break;

                case "Proprietario":
                    if (!select.Contains(" Proprietario, "))
                    {
                        select.Append("Proprietario.CLI_NOME as Proprietario, ");
                        groupBy.Append("Proprietario.CLI_NOME, ");

                        SetarJoinsProprietario(joins);
                    }
                    break;
                case "TecnologiaRastreador":
                    if (!select.Contains(" TecnologiaRastreador, "))
                    {
                        select.Append("TecnologiaRastreador.TRA_NOME_CONTA as TecnologiaRastreador, ");
                        groupBy.Append("TecnologiaRastreador.TRA_NOME_CONTA, ");

                        SetarJoinsTecnologiaRastreador(joins);
                    }
                    break;
                case "TipoComunicacaoRastreador":
                    if (!select.Contains(" TipoComunicacaoRastreador, "))
                    {
                        select.Append("TipoComunicacaoRastreador.TCR_DESCRICAO as TipoComunicacaoRastreador, ");
                        groupBy.Append("TipoComunicacaoRastreador.TCR_DESCRICAO, ");

                        SetarJoinsTipoComunicacaoRastreador(joins);
                    }
                    break;

                case "ModeloVeicular":
                    if (!select.Contains(" ModeloVeicular, "))
                    {
                        select.Append("ModeloVeicularCarga.MVC_DESCRICAO as ModeloVeicular, ");
                        groupBy.Append("ModeloVeicularCarga.MVC_DESCRICAO, ");

                        SetarJoinsModeloVeicularCarga(joins);
                    }
                    break;

                case "UF":
                    if (!select.Contains(" UF, "))
                    {
                        select.Append("Estado.UF_SIGLA as UF, ");
                        groupBy.Append("Estado.UF_SIGLA, ");

                        SetarJoinsEstado(joins);
                    }
                    break;

                case "RNTRC":
                    if (!select.Contains(" RNTRC, "))
                    {
                        select.Append(@"    CASE WHEN Veiculo.VEI_RNTRC = '0' THEN ''
                                            ELSE cast(Veiculo.VEI_RNTRC as nvarchar(20))
                                            END as RNTRC, ");
                        groupBy.Append("Veiculo.VEI_RNTRC, ");
                    }
                    break;

                case "TipoRodado":
                    if (!select.Contains(" TipoRodado, "))
                    {
                        select.Append(@"   CASE WHEN Veiculo.VEI_TIPORODADO = '00' THEN 'Não Aplicado' 
                                                WHEN Veiculo.VEI_TIPORODADO = '01' THEN 'Truck' 
                                                WHEN Veiculo.VEI_TIPORODADO = '02' THEN 'Toco' 
                                                WHEN Veiculo.VEI_TIPORODADO = '03' THEN 'Cavalo' 
                                                WHEN Veiculo.VEI_TIPORODADO = '04' THEN 'Van' 
                                                WHEN Veiculo.VEI_TIPORODADO = '05' THEN 'Utilitário' 
                                                WHEN Veiculo.VEI_TIPORODADO = '06' THEN 'Outros' 
                                            ELSE '' 
                                            END as TipoRodado, ");
                        groupBy.Append("Veiculo.VEI_TIPORODADO, ");
                    }
                    break;

                case "TipoCarroceria":
                    if (!select.Contains(" TipoCarroceria, "))
                    {
                        select.Append(@"   CASE WHEN Veiculo.VEI_TIPO_CARROCERIA = '00' THEN 'Não Aplicado' 
                                                WHEN Veiculo.VEI_TIPO_CARROCERIA = '01' THEN 'Aberta' 
                                                WHEN Veiculo.VEI_TIPO_CARROCERIA = '02' THEN 'Fechada/Baú' 
                                                WHEN Veiculo.VEI_TIPO_CARROCERIA = '03' THEN 'Granel' 
                                                WHEN Veiculo.VEI_TIPO_CARROCERIA = '04' THEN 'Porta Container' 
                                                WHEN Veiculo.VEI_TIPO_CARROCERIA = '05' THEN 'Utilitário' 
                                                WHEN Veiculo.VEI_TIPO_CARROCERIA = '06' THEN 'Sider' 
                                            ELSE '' 
                                            END as TipoCarroceria, ");
                        groupBy.Append("Veiculo.VEI_TIPO_CARROCERIA, ");
                    }
                    break;

                case "Chassi":
                    if (!select.Contains(" Chassi, "))
                    {
                        select.Append("Veiculo.VEI_CHASSI as Chassi, ");
                        groupBy.Append("Veiculo.VEI_CHASSI, ");
                    }
                    break;

                case "CnpjTransportadorFormatado":
                    if (!select.Contains(" CnpjTransportador, "))
                    {
                        select.Append("Empresa.EMP_CNPJ as CnpjTransportador, ");
                        groupBy.Append("Empresa.EMP_CNPJ, ");

                        SetarJoinsEmpresa(joins);
                    }
                    break;

                case "Transportador":
                    if (!select.Contains(" Transportador, "))
                    {
                        select.Append("Empresa.EMP_RAZAO as Transportador, ");
                        groupBy.Append("Empresa.EMP_RAZAO, ");

                        SetarJoinsEmpresa(joins);
                    }
                    break;

                case "CpfMotoristaFormatado":
                    if (!select.Contains(" CpfMotoristaFormatado, "))
                    {
                        select.Append("ISNULL(SUBSTRING(( SELECT ', ' + SUBSTRING(Motorista.FUN_CPF,1,3) + '.' + SUBSTRING(Motorista.FUN_CPF,4,3) + '.' + SUBSTRING(Motorista.FUN_CPF,7,3) + '-' + SUBSTRING(Motorista.FUN_CPF,10,2) FROM T_VEICULO_MOTORISTA VeiculoMotorista JOIN T_FUNCIONARIO Motorista ON VeiculoMotorista.FUN_CODIGO = Motorista.FUN_CODIGO WHERE VeiculoMotorista.VEI_CODIGO = Veiculo.VEI_CODIGO FOR XML PATH('') ), 3, 1000), '') AS CpfMotoristaFormatado, ");

                        if (!groupBy.Contains("Veiculo.VEI_CODIGO"))
                            groupBy.Append("Veiculo.VEI_CODIGO, ");
                    }
                    break;

                case "Motorista":
                    if (!select.Contains(" Motorista, "))
                    {
                        select.Append("ISNULL(SUBSTRING(( SELECT ', ' + Motorista.FUN_NOME FROM T_VEICULO_MOTORISTA VeiculoMotorista JOIN T_FUNCIONARIO Motorista ON VeiculoMotorista.FUN_CODIGO = Motorista.FUN_CODIGO WHERE VeiculoMotorista.VEI_CODIGO = Veiculo.VEI_CODIGO FOR XML PATH('') ), 3, 1000), '') AS Motorista, ");

                        if (!groupBy.Contains("Veiculo.VEI_CODIGO"))
                            groupBy.Append("Veiculo.VEI_CODIGO, ");
                    }
                    break;

                case "Segmento":
                    if (!select.Contains(" Segmento, "))
                    {
                        select.Append("SegmentoVeiculo.VSE_DESCRICAO as Segmento, ");
                        groupBy.Append("SegmentoVeiculo.VSE_DESCRICAO, ");

                        SetarJoinsSegmentoVeiculo(joins);
                    }
                    break;

                case "LocalAtual":
                    if (!select.Contains(" LocalAtual, "))
                    {
                        select.Append("(AreaVeiculo.ARV_DESCRICAO + ' - ' + LocalAtual.AVP_DESCRICAO) as LocalAtual, ");
                        groupBy.Append("AreaVeiculo.ARV_DESCRICAO, LocalAtual.AVP_DESCRICAO, ");

                        SetarJoinsAreaVeiculo(joins);
                    }
                    break;

                case "Reboques":
                    if (!select.Contains(" Reboques, "))
                    {
                        select.Append("isnull(SUBSTRING(( ");
                        select.Append("    select ', ' + VeiculoReboque.VEI_PLACA ");
                        select.Append("      from T_VEICULO_CONJUNTO Reboque ");
                        select.Append("      join T_VEICULO VeiculoReboque on VeiculoReboque.VEI_CODIGO = Reboque.VEC_CODIGO_FILHO ");
                        select.Append("     where Reboque.VEC_CODIGO_PAI = Veiculo.VEI_CODIGO ");
                        select.Append("       for XML PATH('') ");
                        select.Append("), 3, 1000), '') as Reboques, ");

                        if (!groupBy.Contains("Veiculo.VEI_CODIGO, "))
                            groupBy.Append("Veiculo.VEI_CODIGO, ");
                    }
                    break;

                case "FuncionarioResponsavel":
                    if (!select.Contains(" FuncionarioResponsavel, "))
                    {
                        select.Append("FuncionarioResponsavel.FUN_NOME as FuncionarioResponsavel, ");
                        groupBy.Append("FuncionarioResponsavel.FUN_NOME, ");

                        SetarJoinsFuncionarioResponsavel(joins);
                    }
                    break;

                case "AnoFabricacao":
                    if (!select.Contains(" AnoFabricacao, "))
                    {
                        select.Append("Veiculo.VEI_ANO as AnoFabricacao, ");
                        groupBy.Append("Veiculo.VEI_ANO, ");
                    }
                    break;

                case "AnoModelo":
                    if (!select.Contains(" AnoModelo, "))
                    {
                        select.Append("Veiculo.VEI_ANOMODELO as AnoModelo, ");
                        groupBy.Append("Veiculo.VEI_ANOMODELO, ");
                    }
                    break;

                case "Modelo":
                    if (!select.Contains(" Modelo, "))
                    {
                        select.Append("ModeloVeiculo.VMO_DESCRICAO as Modelo, ");
                        groupBy.Append("ModeloVeiculo.VMO_DESCRICAO, ");

                        SetarJoinsModeloVeiculo(joins);
                    }
                    break;

                case "Marca":
                    if (!select.Contains(" Marca, "))
                    {
                        select.Append("MarcaVeiculo.VMA_DESCRICAO as Marca, ");
                        groupBy.Append("MarcaVeiculo.VMA_DESCRICAO, ");

                        SetarJoinsMarcaVeiculo(joins);
                    }
                    break;

                case "Cor":
                    if (!select.Contains(" Cor, "))
                    {
                        select.Append("Veiculo.VEI_COR as Cor, ");
                        groupBy.Append("Veiculo.VEI_COR, ");
                    }
                    break;

                case "GrupoPessoas":
                    if (!select.Contains(" GrupoPessoas, "))
                    {
                        select.Append("GrupoPessoas.GRP_DESCRICAO as GrupoPessoas, ");
                        groupBy.Append("GrupoPessoas.GRP_DESCRICAO, ");

                        SetarJoinsGrupoPessoas(joins);
                    }
                    break;

                case "Tara":
                    if (!select.Contains(" Tara, "))
                    {
                        select.Append("Veiculo.VEI_TARA as Tara, ");
                        groupBy.Append("Veiculo.VEI_TARA, ");
                    }
                    break;

                case "CapacidadeM3":
                    if (!select.Contains(" CapacidadeM3, "))
                    {
                        select.Append("Veiculo.VEI_CAP_M3 as CapacidadeM3, ");
                        groupBy.Append("Veiculo.VEI_CAP_M3, ");
                    }
                    break;

                case "CapacidadeKG":
                    if (!select.Contains(" CapacidadeKG, "))
                    {
                        select.Append("Veiculo.VEI_CAP_KG as CapacidadeKG, ");
                        groupBy.Append("Veiculo.VEI_CAP_KG, ");
                    }
                    break;

                case "KMAtual":
                    if (!select.Contains(" KMAtual, "))
                    {
                        select.Append("Veiculo.VEI_KMATUAL as KMAtual, ");
                        groupBy.Append("Veiculo.VEI_KMATUAL, ");
                    }
                    break;

                case "DataAquisicaoFormatada":
                    if (!select.Contains(" DataAquisicao, "))
                    {
                        select.Append("Veiculo.VEI_DATACOMPRA as DataAquisicao, ");
                        groupBy.Append("Veiculo.VEI_DATACOMPRA, ");
                    }
                    break;

                case "ValorAquisicao":
                    if (!select.Contains(" ValorAquisicao, "))
                    {
                        select.Append("Veiculo.VEI_VALORAQUIS as ValorAquisicao, ");
                        groupBy.Append("Veiculo.VEI_VALORAQUIS, ");
                    }
                    break;

                case "CentroCarregamento":
                    if (!select.Contains(" CentroCarregamento, "))
                    {
                        select.Append("SUBSTRING(( ");
                        select.Append("    select ', ' + centroCarregamento.CEC_DESCRICAO ");
                        select.Append("      from T_CENTRO_CARREGAMENTO centroCarregamento ");
                        select.Append("      join T_CENTRO_CARREGAMENTO_VEICULO centroCarregamentoVeiculo on centroCarregamentoVeiculo.CEC_CODIGO = centroCarregamento.CEC_CODIGO ");
                        select.Append("     where centroCarregamentoVeiculo.VEI_CODIGO = Veiculo.VEI_CODIGO ");
                        select.Append("for XML PATH('')), 3, 1000) as CentroCarregamento, ");

                        if (!groupBy.Contains("Veiculo.VEI_CODIGO, "))
                            groupBy.Append("Veiculo.VEI_CODIGO, ");
                    }
                    break;

                case "QuantidadeEixos":
                    if (!select.Contains(" QuantidadeEixos, "))
                    {
                        select.Append("ModeloVeicularCarga.MVC_NUMERO_EIXOS as QuantidadeEixos, ");
                        groupBy.Append("ModeloVeicularCarga.MVC_NUMERO_EIXOS, ");

                        SetarJoinsModeloVeicularCarga(joins);
                    }
                    break;

                case "CentroResultado":
                    if (!select.Contains(" CentroResultado, "))
                    {
                        select.Append("CentroResultado.CRE_DESCRICAO as CentroResultado, ");
                        groupBy.Append("CentroResultado.CRE_DESCRICAO, ");

                        SetarJoinsCentroResultado(joins);
                    }
                    break;

                case "CapacidadeTanque":
                    if (!select.Contains(" CapacidadeTanque, "))
                    {
                        select.Append("Veiculo.VEI_CAPTANQUE as CapacidadeTanque, ");
                        groupBy.Append("Veiculo.VEI_CAPTANQUE, ");
                    }
                    break;

                case "CpfCnpjProprietario":
                case "CpfCnpjProprietarioFormatado":
                    if (!select.Contains(" CpfCnpjProprietario, "))
                    {
                        select.Append("Proprietario.CLI_CGCCPF as CpfCnpjProprietario, ");
                        groupBy.Append("Proprietario.CLI_CGCCPF, ");
                        SetarJoinsProprietario(joins);
                    }
                    break;

                case "VeiculoPossuiTagValePedagio":
                case "VeiculoPossuiTagValePedagioFormatada":
                    if (!select.Contains(" VeiculoPossuiTagValePedagio, "))
                    {
                        select.Append("Veiculo.VEI_POSSUI_TAG_VALE_PEDAGIO as VeiculoPossuiTagValePedagio, ");
                        groupBy.Append("Veiculo.VEI_POSSUI_TAG_VALE_PEDAGIO, ");
                    }
                    break;

                case "Observacao":
                    if (!select.Contains(" Observacao, "))
                    {
                        select.Append("Veiculo.VEI_OBS Observacao, ");
                        groupBy.Append("Veiculo.VEI_OBS, ");
                    }
                    break;

                case "ModeloReboques":
                    if (!select.Contains(" ModeloReboques, "))
                    {
                        select.Append("isnull( ");
                        select.Append("     SUBSTRING(( ");
                        select.Append(" 		SELECT  ");
                        select.Append(" 			', ' + ModeloReboque.VMO_DESCRICAO ");
                        select.Append(" 		FROM ");
                        select.Append(" 			T_VEICULO_CONJUNTO Reboque ");
                        select.Append(" 			JOIN T_VEICULO VeiculoReboque ");
                        select.Append(" 				ON VeiculoReboque.VEI_CODIGO = Reboque.VEC_CODIGO_FILHO ");
                        select.Append(" 			LEFT JOIN T_VEICULO_MODELO ModeloReboque ");
                        select.Append(" 				ON ModeloReboque.VMO_CODIGO = VeiculoReboque.VMO_CODIGO ");
                        select.Append(" 		WHERE ");
                        select.Append(" 			Reboque.VEC_CODIGO_PAI = Veiculo.VEI_CODIGO ");
                        select.Append(" 		for XML PATH('')), ");
                        select.Append(" 	3, 1000), ");
                        select.Append(" '') as ModeloReboques, ");

                        if (!groupBy.Contains("Veiculo.VEI_CODIGO, "))
                            groupBy.Append("Veiculo.VEI_CODIGO, ");
                    }
                    break;

                case "MarcaReboques":
                    if (!select.Contains(" MarcaReboques, "))
                    {
                        select.Append(" isnull( ");
                        select.Append(" 	SUBSTRING(( ");
                        select.Append(" 		SELECT ");
                        select.Append(" 			', ' + MarcaReboque.VMA_DESCRICAO ");
                        select.Append(" 		FROM ");
                        select.Append(" 			T_VEICULO_CONJUNTO Reboque ");
                        select.Append(" 			JOIN T_VEICULO VeiculoReboque ");
                        select.Append(" 				ON VeiculoReboque.VEI_CODIGO = Reboque.VEC_CODIGO_FILHO ");
                        select.Append(" 			LEFT JOIN T_VEICULO_MARCA MarcaReboque ");
                        select.Append(" 				ON MarcaReboque.VMA_CODIGO = VeiculoReboque.VMA_CODIGO ");
                        select.Append(" 		WHERE ");
                        select.Append(" 			Reboque.VEC_CODIGO_PAI = Veiculo.VEI_CODIGO ");
                        select.Append(" 		for XML PATH('')), ");
                        select.Append(" 	3, 1000), ");
                        select.Append(" '') as MarcaReboques, ");

                        if (!groupBy.Contains("Veiculo.VEI_CODIGO, "))
                            groupBy.Append("Veiculo.VEI_CODIGO, ");
                    }
                    break;

                case "AnoModeloReboques":
                    if (!select.Contains(" AnoModeloReboques, "))
                    {
                        select.Append(" isnull( ");
                        select.Append(" 	SUBSTRING(( ");
                        select.Append(" 		SELECT ");
                        select.Append(" 			', ' + CAST(VeiculoReboque.VEI_ANOMODELO AS VARCHAR(4)) ");
                        select.Append(" 		FROM ");
                        select.Append(" 			T_VEICULO_CONJUNTO Reboque ");
                        select.Append(" 		JOIN T_VEICULO VeiculoReboque ");
                        select.Append(" 			ON VeiculoReboque.VEI_CODIGO = Reboque.VEC_CODIGO_FILHO ");
                        select.Append(" 		WHERE ");
                        select.Append(" 			Reboque.VEC_CODIGO_PAI = Veiculo.VEI_CODIGO ");
                        select.Append(" 		FOR XML PATH('') ), ");
                        select.Append(" 	3, 1000), ");
                        select.Append(" '') as AnoModeloReboques, ");

                        if (!groupBy.Contains("Veiculo.VEI_CODIGO, "))
                            groupBy.Append("Veiculo.VEI_CODIGO, ");
                    }
                    break;

                case "ModeloCarroceria":
                    if (!select.Contains(" ModeloCarroceria, "))
                    {
                        select.Append("ModeloCarroceria.MCA_DESCRICAO ModeloCarroceria, ");
                        groupBy.Append("ModeloCarroceria.MCA_DESCRICAO, ");

                        SetarJoinsModeloCarroceria(joins);
                    }
                    break;

                case "MotivoBloqueio":
                    if (!select.Contains(" MotivoBloqueio, "))
                    {
                        select.Append("Veiculo.VEI_MOTIVO_BLOQUEIO MotivoBloqueio, ");
                        groupBy.Append("Veiculo.VEI_MOTIVO_BLOQUEIO, ");
                    }
                    break;

                case "Bloqueado":
                case "BloqueadoDescricao":
                    if (!select.Contains(" Bloqueado, "))
                    {
                        select.Append("Veiculo.VEI_VEICULO_BLOQUEADO Bloqueado, ");
                        groupBy.Append("Veiculo.VEI_VEICULO_BLOQUEADO, ");
                    }
                    break;

                case "RGMotorista":
                    if (!select.Contains(" RGMotorista, "))
                    {
                        select.Append("ISNULL(SUBSTRING((SELECT ', ' + Motorista.FUN_RG FROM T_VEICULO_MOTORISTA VeiculoMotorista JOIN T_FUNCIONARIO Motorista ON VeiculoMotorista.FUN_CODIGO = Motorista.FUN_CODIGO WHERE VeiculoMotorista.VEI_CODIGO = Veiculo.VEI_CODIGO FOR XML PATH('') ), 3, 1000), '') AS RGMotorista, ");

                        if (!groupBy.Contains("Veiculo.VEI_CODIGO"))
                            groupBy.Append("Veiculo.VEI_CODIGO, ");
                    }
                    break;

                case "DataEmissaoRGMotorista":
                    if (!select.Contains(" DataEmissaoRGMotorista, "))
                    {
                        select.Append("ISNULL(SUBSTRING((SELECT ', ' + CONVERT(VARCHAR, Motorista.FUN_DATA_EMISSAO_RG, 103) FROM T_VEICULO_MOTORISTA VeiculoMotorista JOIN T_FUNCIONARIO Motorista ON VeiculoMotorista.FUN_CODIGO = Motorista.FUN_CODIGO WHERE VeiculoMotorista.VEI_CODIGO = Veiculo.VEI_CODIGO FOR XML PATH('')), 3, 1000), '') AS DataEmissaoRGMotorista, ");

                        if (!groupBy.Contains("Veiculo.VEI_CODIGO"))
                            groupBy.Append("Veiculo.VEI_CODIGO, ");
                    }
                    break;

                case "DataNascimentoMotorista":
                    if (!select.Contains(" DataNascimentoMotorista, "))
                    {
                        select.Append("ISNULL(SUBSTRING((SELECT ', ' + CONVERT(VARCHAR, Motorista.FUN_DATANASC, 103) FROM T_VEICULO_MOTORISTA VeiculoMotorista JOIN T_FUNCIONARIO Motorista ON VeiculoMotorista.FUN_CODIGO = Motorista.FUN_CODIGO WHERE VeiculoMotorista.VEI_CODIGO = Veiculo.VEI_CODIGO FOR XML PATH('')), 3, 1000), '') AS DataNascimentoMotorista, ");

                        if (!groupBy.Contains("Veiculo.VEI_CODIGO"))
                            groupBy.Append("Veiculo.VEI_CODIGO, ");
                    }
                    break;

                case "TelefoneMotorista":
                    if (!select.Contains(" TelefoneMotorista, "))
                    {
                        select.Append("ISNULL(SUBSTRING((SELECT ', ' + Motorista.FUN_FONE FROM T_VEICULO_MOTORISTA VeiculoMotorista JOIN T_FUNCIONARIO Motorista ON VeiculoMotorista.FUN_CODIGO = Motorista.FUN_CODIGO WHERE VeiculoMotorista.VEI_CODIGO = Veiculo.VEI_CODIGO FOR XML PATH('')), 3, 1000), '') AS TelefoneMotorista, ");

                        if (!groupBy.Contains("Veiculo.VEI_CODIGO"))
                            groupBy.Append("Veiculo.VEI_CODIGO, ");
                    }
                    break;

                case "TaraReboque":
                    if (!select.Contains(" TaraReboque, "))
                    {
                        select.Append("isnull(SUBSTRING((select ', ' + replace(FORMAT(VeiculoReboque.VEI_TARA, 'N', 'pt-br'),',00', '') from T_VEICULO_CONJUNTO Reboque join T_VEICULO VeiculoReboque on VeiculoReboque.VEI_CODIGO = Reboque.VEC_CODIGO_FILHO where Reboque.VEC_CODIGO_PAI = Veiculo.VEI_CODIGO for XML PATH('')), 3, 1000), '') AS TaraReboque, ");

                        if (!groupBy.Contains("Veiculo.VEI_CODIGO"))
                            groupBy.Append("Veiculo.VEI_CODIGO, ");
                    }
                    break;

                case "NomeTransportadorCNPJTransportador":
                    if (!select.Contains(" NomeTransportadorCNPJTransportador, "))
                    {
                        select.Append(@"concat(ISNULL(SUBSTRING((SELECT DISTINCT ', ' + Empresa.EMP_RAZAO
                                            FROM T_VEICULO_EMPRESA VeiculoEmpresa
                                            inner join T_EMPRESA Empresa ON Empresa.EMP_CODIGO = VeiculoEmpresa.EMP_CODIGO
                                            WHERE VeiculoEmpresa.VEI_CODIGO = Veiculo.VEI_CODIGO  FOR XML PATH('')), 3, 1000) + ',',''),
                                            SUBSTRING((SELECT DISTINCT ', ' + Empresa.EMP_CNPJ
                                            FROM T_VEICULO_EMPRESA VeiculoEmpresa
                                            inner join T_EMPRESA Empresa ON Empresa.EMP_CODIGO = VeiculoEmpresa.EMP_CODIGO
                                            WHERE VeiculoEmpresa.VEI_CODIGO = Veiculo.VEI_CODIGO  FOR XML PATH('')), 3, 1000)) NomeTransportadorCNPJTransportador, ");

                        if (!groupBy.Contains("Veiculo.VEI_CODIGO"))
                            groupBy.Append("Veiculo.VEI_CODIGO, ");
                    }
                    break;
                case "NomeTransportador":
                    if (!select.Contains(" NomeTransportador, "))
                    {
                        select.Append(@" ISNULL(SUBSTRING((SELECT DISTINCT ', ' + Empresa.EMP_RAZAO
                                            FROM T_VEICULO_EMPRESA VeiculoEmpresa
                                            inner join T_EMPRESA Empresa ON Empresa.EMP_CODIGO = VeiculoEmpresa.EMP_CODIGO
                                            WHERE VeiculoEmpresa.VEI_CODIGO = Veiculo.VEI_CODIGO  FOR XML PATH('')), 3, 1000) + '','') NomeTransportador , ");


                        if (!groupBy.Contains("Veiculo.VEI_CODIGO"))
                            groupBy.Append("Veiculo.VEI_CODIGO, ");

                    }
                    break;
                case "CNPJTransportador":
                    if (!select.Contains(" CNPJTransportador, "))
                    {
                        select.Append(@"ISNULL(SUBSTRING((SELECT DISTINCT ', ' + Empresa.EMP_CNPJ
                                            FROM T_VEICULO_EMPRESA VeiculoEmpresa
                                            inner join T_EMPRESA Empresa ON Empresa.EMP_CODIGO = VeiculoEmpresa.EMP_CODIGO
                                            WHERE VeiculoEmpresa.VEI_CODIGO = Veiculo.VEI_CODIGO  FOR XML PATH('')), 3, 1000) + ' ','') CNPJTransportador , ");


                        if (!groupBy.Contains("Veiculo.VEI_CODIGO"))
                            groupBy.Append("Veiculo.VEI_CODIGO, ");

                    }
                    break;
                case "DataValidadeAdicionalCarroceriaFormatada":
                    if (!select.Contains(" DataValidadeAdicionalCarroceria, "))
                    {
                        select.Append("Veiculo.VEI_DATA_VALIDADE_ADICIONAL_CARROCERIA DataValidadeAdicionalCarroceria, ");
                        groupBy.Append("Veiculo.VEI_DATA_VALIDADE_ADICIONAL_CARROCERIA, ");
                    }
                    break;
                case "NaogerarIntegracaoOpentechsFormatada":
                    if (!select.Contains(" NaogerarIntegracaoOpentechs, "))
                    {
                        select.Append(@"VEI_NAO_INTEGRAR_OPENTECH NaogerarIntegracaoOpentechs, ");
                        groupBy.Append("Veiculo.VEI_NAO_INTEGRAR_OPENTECH, ");
                    }
                    break;

                case "QuantidadePaletes":
                    if (!select.Contains(" QuantidadePaletes, "))
                    {
                        select.Append("ModeloVeicularCarga.MVC_NUMERO_PALETES as QuantidadePaletes, ");
                        groupBy.Append("ModeloVeicularCarga.MVC_NUMERO_PALETES, ");

                        SetarJoinsModeloVeicularCarga(joins);
                    }
                    break;

                case "VeiculoAlugadoDescricao":
                    if (!select.Contains(" VeiculoAlugado, "))
                    {
                        select.Append("Veiculo.VEI_VEICULO_ALUGADO VeiculoAlugado, ");
                        groupBy.Append("Veiculo.VEI_VEICULO_ALUGADO, ");
                    }
                    break;

                case "TipoCombustivel":
                    if (!select.Contains(" TipoCombustivel, "))
                    {
                        select.Append("Veiculo.VEI_TIPOCOMBUSTIVEL TipoCombustivel, ");
                        groupBy.Append("Veiculo.VEI_TIPOCOMBUSTIVEL, ");
                    }
                    break;

                case "Tracao":
                    if (!select.Contains(" Tracao, "))
                    {
                        select.Append(@"isnull(SUBSTRING(( 
                                        select ', ' + VeiculoTracao.VEI_PLACA
                                          from T_VEICULO_CONJUNTO Tracao
                                          join T_VEICULO VeiculoTracao on VeiculoTracao.VEI_CODIGO = Tracao.VEC_CODIGO_PAI
                                         where Tracao.VEC_CODIGO_FILHO = Veiculo.VEI_CODIGO
                                           for XML PATH('') 
                                    ), 3, 1000), '') as Tracao, ");
                    }
                    break;

                case "TagSemParar":
                    if (!select.Contains(" TagSemParar, "))
                    {
                        select.Append("Veiculo.VEI_TAG_SEM_PARAR as TagSemParar, ");
                        groupBy.Append("Veiculo.VEI_TAG_SEM_PARAR, ");
                    }
                    break;

                case "OperadoraValePedagio":
                    if (!select.Contains(" OperadoraValePedagio, "))
                    {
                        select.Append("SUBSTRING((");
                        select.Append("    SELECT ', ' + TipoIntegracao.TPI_DESCRICAO ");
                        select.Append("      FROM T_VEICULO_TIPO_INTEGRACAO_VALE_PEDAGIO VeiculoTipoIntegracaoValePedagio ");
                        select.Append("      JOIN T_TIPO_INTEGRACAO TipoIntegracao ON TipoIntegracao.TPI_CODIGO = VeiculoTipoIntegracaoValePedagio.TPI_CODIGO ");
                        select.Append("     WHERE VeiculoTipoIntegracaoValePedagio.VEI_CODIGO = Veiculo.VEI_CODIGO ");
                        select.Append("       FOR XML PATH('') ");
                        select.Append("), 3, 1000) OperadoraValePedagio, ");

                        if (!groupBy.Contains("Veiculo.VEI_CODIGO"))
                            groupBy.Append("Veiculo.VEI_CODIGO, ");
                    }
                    break;

                case "LocadorCNPJ":
                    if (!select.Contains(" LocadorCNPJ, "))
                    {
                        select.Append("Veiculo.VEI_LOCADOR Locador, ");
                        groupBy.Append("Veiculo.VEI_LOCADOR , ");
                    }
                    break;
                case "Locador":
                    if (!select.Contains(" Locador, "))
                    {

                        select.Append("Locador.CLI_NOME as Locador, ");
                        groupBy.Append("Locador.CLI_NOME, ");

                        SetarJoinsLocador(joins);
                    }
                    break;
            }
        }

        protected override void SetarWhere(Dominio.ObjetosDeValor.Embarcador.Veiculos.FiltroPesquisaRelatorioVeiculo filtrosPesquisa, StringBuilder where, StringBuilder joins, StringBuilder groupBy, List<Embarcador.Consulta.ParametroSQL> parametros = null)
        {
            string pattern = "yyyy-MM-dd";

            if (!string.IsNullOrWhiteSpace(filtrosPesquisa.Placa))
                where.Append($" and Veiculo.VEI_PLACA like '%{filtrosPesquisa.Placa}%'");

            if (!string.IsNullOrWhiteSpace(filtrosPesquisa.Chassi))
            {
                where.Append($" and Veiculo.VEI_CHASSI like :VEICULO_VEI_CHASSI"); 
                parametros.Add(new Embarcador.Consulta.ParametroSQL("VEICULO_VEI_CHASSI", $"%{filtrosPesquisa.Chassi}%"));
            }

            if (!string.IsNullOrWhiteSpace(filtrosPesquisa.TipoVeiculo) && filtrosPesquisa.TipoVeiculo != "-1")
                where.Append($" and Veiculo.VEI_TIPOVEICULO = {filtrosPesquisa.TipoVeiculo}");

            if (!string.IsNullOrWhiteSpace(filtrosPesquisa.Tipo) && filtrosPesquisa.Tipo != "A")
                where.Append($" and Veiculo.VEI_TIPO = '{filtrosPesquisa.Tipo}'");

            if (filtrosPesquisa.CpfcnpjProprietario > 0)
                where.Append($" and Veiculo.VEI_PROPRIETARIO = {filtrosPesquisa.CpfcnpjProprietario}");

            if (filtrosPesquisa.CodigoMotorista > 0)
                where.Append($" AND EXISTS (SELECT VeiculoMotorista.VMT_CODIGO FROM T_VEICULO_MOTORISTA VeiculoMotorista WHERE VeiculoMotorista.FUN_CODIGO = {filtrosPesquisa.CodigoMotorista} AND VeiculoMotorista.VEI_CODIGO = Veiculo.VEI_CODIGO)"); // SQL-INJECTION-SAFE

            if (filtrosPesquisa.CodigoEmpresa > 0)
                where.Append($" and Veiculo.EMP_CODIGO = {filtrosPesquisa.CodigoEmpresa}");

            if (filtrosPesquisa.CodigosEmpresa?.Count > 0)
                where.Append($" and Veiculo.EMP_CODIGO in ({string.Join(" ,", filtrosPesquisa.CodigosEmpresa)})");

            if (filtrosPesquisa.Ativo == Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoAtivoPesquisa.Ativo)
                where.Append($" and Veiculo.VEI_ATIVO = 1");
            else if (filtrosPesquisa.Ativo == Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoAtivoPesquisa.Inativo)
                where.Append($" and (Veiculo.VEI_ATIVO = 0 or Veiculo.VEI_ATIVO IS NULL)");

            if (filtrosPesquisa.CodigosSegmento != null && filtrosPesquisa.CodigosSegmento.Count > 0)
                where.Append($" and Veiculo.VSE_CODIGO in ({string.Join(",", filtrosPesquisa.CodigosSegmento)})");

            if (filtrosPesquisa.CodigosFuncionarioResponsavel != null && filtrosPesquisa.CodigosFuncionarioResponsavel.Count > 0)
                where.Append($" and Veiculo.FUN_CODIGO_RESPONSAVEL in ({string.Join(",", filtrosPesquisa.CodigosFuncionarioResponsavel)})");

            if (filtrosPesquisa.CodigoCentroCarregamento > 0)
                where.Append($" and Veiculo.VEI_CODIGO in (SELECT VEI_CODIGO FROM T_CENTRO_CARREGAMENTO_VEICULO WHERE CEC_CODIGO = {filtrosPesquisa.CodigoCentroCarregamento})");

            if (filtrosPesquisa.CodigoCentroResultado > 0)
                where.Append($" and Veiculo.CRE_CODIGO = {filtrosPesquisa.CodigoCentroResultado}");

            if (filtrosPesquisa.VeiculoPossuiTagValePedagio)
                where.Append($" and Veiculo.VEI_POSSUI_TAG_VALE_PEDAGIO = 1");

            if (filtrosPesquisa.CodigosMarcaVeiculo != null && filtrosPesquisa.CodigosMarcaVeiculo.Count > 0)
                where.Append($" and Veiculo.VMA_CODIGO in ({string.Join(",", filtrosPesquisa.CodigosMarcaVeiculo)})");

            if (filtrosPesquisa.CodigosModeloVeiculo != null && filtrosPesquisa.CodigosModeloVeiculo.Count > 0)
                where.Append($" and Veiculo.VMO_CODIGO in ({string.Join(",", filtrosPesquisa.CodigosModeloVeiculo)})");

            if (filtrosPesquisa.DataCadastroInicial.HasValue)
                where.Append($" and CAST(Veiculo.VEI_DATA_ATULIZACAO AS DATE) >= '{filtrosPesquisa.DataCadastroInicial.Value.ToString(pattern)}'");

            if (filtrosPesquisa.DataCadastroFinal.HasValue)
                where.Append($" and CAST(Veiculo.VEI_DATA_ATULIZACAO AS DATE) <= '{filtrosPesquisa.DataCadastroFinal.Value.ToString(pattern)}'");

            if (filtrosPesquisa.DataCriacaoInicial.HasValue)
                where.Append($" and CAST(Veiculo.VEI_DATA_CADASTRO AS DATE) >= '{filtrosPesquisa.DataCriacaoInicial.Value.ToString(pattern)}'");

            if (filtrosPesquisa.DataCriacaoFinal.HasValue)
                where.Append($" and CAST(Veiculo.VEI_DATA_CADASTRO AS DATE) <= '{filtrosPesquisa.DataCriacaoFinal.Value.ToString(pattern)}'");

            if (filtrosPesquisa.ContratosFrete?.Count > 0)
                where.Append($@" and exists (select ContratoFreteTransportador.CFT_DESCRICAO 
                                from T_CONTRATO_FRETE_TRANSPORTADOR ContratoFreteTransportador
                                left join T_CONTRATO_FRETE_TRANSPORTADOR_VEICULO ContratoFreteTransportadorVeiculo on ContratoFreteTransportadorVeiculo.CFT_CODIGO = ContratoFreteTransportador.CFT_CODIGO
                                where ContratoFreteTransportador.CFT_CODIGO in ({string.Join(" ,", filtrosPesquisa.ContratosFrete)}) and ContratoFreteTransportadorVeiculo.VEI_CODIGO = Veiculo.VEI_CODIGO)");

            if (filtrosPesquisa.Bloqueado != null)
            {
                if (!filtrosPesquisa.Bloqueado.Value)
                    where.Append($" and (Veiculo.VEI_VEICULO_BLOQUEADO = {Convert.ToInt32(filtrosPesquisa.Bloqueado.Value)} OR Veiculo.VEI_VEICULO_BLOQUEADO IS NULL)");
                else
                    where.Append($" and Veiculo.VEI_VEICULO_BLOQUEADO = {Convert.ToInt32(filtrosPesquisa.Bloqueado.Value)}");
            }

            if (filtrosPesquisa.PossuiVinculo != Dominio.ObjetosDeValor.Embarcador.Enumeradores.SimNao.Todos)
                where.Append(" and" + (filtrosPesquisa.PossuiVinculo == Dominio.ObjetosDeValor.Embarcador.Enumeradores.SimNao.Nao ? " not" : "") + $@" exists (select Conjunto.VEC_CODIGO_FILHO from T_VEICULO_CONJUNTO Conjunto where Conjunto.VEC_CODIGO_FILHO = Veiculo.VEI_CODIGO or Conjunto.VEC_CODIGO_PAI = Veiculo.VEI_CODIGO)");

            if (filtrosPesquisa.CodigosModeloVeicularCarga?.Count > 0)
            {
                where.Append($" and ModeloVeicularCarga.MVC_CODIGO in ({String.Join(",", filtrosPesquisa.CodigosModeloVeicularCarga)})");
                SetarJoinsModeloVeicularCarga(joins);
            }

            if (!string.IsNullOrWhiteSpace(filtrosPesquisa.TagSemParar))
                where.Append($" and Veiculo.VEI_TAG_SEM_PARAR like '%{filtrosPesquisa.TagSemParar}%'");

            if (filtrosPesquisa.Locador > 0)
                where.Append($" and Veiculo.VEI_LOCADOR = {filtrosPesquisa.Locador}");
        }

        #endregion Métodos Protegidos Sobrescritos
    }
}
