using Dominio.ObjetosDeValor.Embarcador.Enumeradores;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Repositorio.Embarcador.CTe
{
    sealed class ConsultaSubcontratacao : Consulta.Consulta<Dominio.ObjetosDeValor.Embarcador.CTe.FiltroPesquisaRelatorioSubcontratacao>
    {
        #region Construtores

        public ConsultaSubcontratacao() : base(tabela: "T_CTE_TERCEIRO as CTeTerceiro") { }

        #endregion

        #region Métodos Privados

        private void SetarJoinsPedidoCTeSubcontratacao(StringBuilder joins)
        {
            if (!joins.Contains(" PedidoCTeSubcontratacao "))
                joins.Append(" left join T_PEDIDO_CTE_PARA_SUB_CONTRATACAO PedidoCTeSubcontratacao on PedidoCTeSubcontratacao.CPS_CODIGO = CTeTerceiro.CPS_CODIGO ");
        }

        private void SetarJoinsCargaPedido(StringBuilder joins)
        {
            SetarJoinsPedidoCTeSubcontratacao(joins);

            if (!joins.Contains(" CargaPedido "))
                joins.Append(" left join T_CARGA_PEDIDO CargaPedido on CargaPedido.CPE_CODIGO = PedidoCTeSubcontratacao.CPE_CODIGO ");
        }

        private void SetarJoinsCarga(StringBuilder joins)
        {
            SetarJoinsCargaPedido(joins);

            if (!joins.Contains(" Carga "))
                joins.Append(" left join T_CARGA Carga on Carga.CAR_CODIGO = CargaPedido.CAR_CODIGO ");
        }

        private void SetarJoinsCargaOcorrencia(StringBuilder joins)
        {
            if (!joins.Contains(" CargaOcorrencia "))
                joins.Append(" left join T_CARGA_OCORRENCIA CargaOcorrencia on CargaOcorrencia.CPS_CODIGO = CTeTerceiro.CPS_CODIGO ");
        }

        private void SetarJoinsCargaCargaOcorrencia(StringBuilder joins)
        {
            SetarJoinsCargaOcorrencia(joins);

            if (!joins.Contains(" CargaCargaOcorrencia "))
                joins.Append(" left join T_CARGA CargaCargaOcorrencia on CargaCargaOcorrencia.CAR_CODIGO = CargaOcorrencia.CAR_CODIGO ");
        }

        private void SetarJoinsDestinatario(StringBuilder joins)
        {
            if (!joins.Contains(" DestinatarioCTe "))
                joins.Append(" join T_CTE_PARTICIPANTE DestinatarioCTe on CTeTerceiro.CPS_DESTINATARIO_CTE = DestinatarioCTe.PCT_CODIGO ");
        }

        private void SetarJoinsDestinatarioCliente(StringBuilder joins)
        {
            SetarJoinsDestinatario(joins);

            if (!joins.Contains("ClienteDestinatario"))
                joins.Append(" left join T_CLIENTE ClienteDestinatario on ClienteDestinatario.CLI_CGCCPF = DestinatarioCTe.CLI_CODIGO ");
        }

        private void SetarJoinsDestinatarioGrupoPessoa(StringBuilder joins)
        {
            SetarJoinsDestinatarioCliente(joins);

            if (!joins.Contains(" GrupoPessoaDestinatario "))
                joins.Append(" left join T_GRUPO_PESSOAS GrupoPessoaDestinatario on GrupoPessoaDestinatario.GRP_CODIGO = ClienteDestinatario.GRP_CODIGO ");
        }

        private void SetarJoinsDestinatarioCategoria(StringBuilder joins)
        {
            SetarJoinsDestinatarioCliente(joins);

            if (!joins.Contains(" CategoriaDestinatario "))
                joins.Append(" left join T_CATEGORIA_PESSOA CategoriaDestinatario on CategoriaDestinatario.CTP_CODIGO = ClienteDestinatario.CTP_CODIGO ");
        }

        private void SetarJoinsRemetente(StringBuilder joins)
        {
            if (!joins.Contains(" RemetenteCTe "))
                joins.Append(" join T_CTE_PARTICIPANTE RemetenteCTe on CTeTerceiro.CPS_REMETENTE_CTE = RemetenteCTe.PCT_CODIGO ");
        }

        private void SetarJoinsRemetenteCliente(StringBuilder joins)
        {
            SetarJoinsRemetente(joins);

            if (!joins.Contains(" ClienteRemetente "))
                joins.Append(" left join T_CLIENTE ClienteRemetente on ClienteRemetente.CLI_CGCCPF = RemetenteCTe.CLI_CODIGO ");
        }

        private void SetarJoinsRemetenteGrupoPessoa(StringBuilder joins)
        {
            SetarJoinsRemetenteCliente(joins);

            if (!joins.Contains(" GrupoPessoaRemetente "))
                joins.Append(" left join T_GRUPO_PESSOAS GrupoPessoaRemetente on GrupoPessoaRemetente.GRP_CODIGO = ClienteRemetente.GRP_CODIGO ");
        }

        private void SetarJoinsRemetenteCategoria(StringBuilder joins)
        {
            SetarJoinsRemetenteCliente(joins);

            if (!joins.Contains(" CategoriaRemetente "))
                joins.Append(" left join T_CATEGORIA_PESSOA CategoriaRemetente on CategoriaRemetente.CTP_CODIGO = ClienteRemetente.CTP_CODIGO ");
        }

        private void SetarJoinsLocalidadeFimPrestacao(StringBuilder joins)
        {
            if (!joins.Contains(" FimPrestacaoCTe "))
                joins.Append(" left join T_LOCALIDADES FimPrestacaoCTe on CTeTerceiro.CPS_LOCTERMINOPRESTACAO = FimPrestacaoCTe.LOC_CODIGO ");
        }

        private void SetarJoinsLocalidadeInicioPrestacao(StringBuilder joins)
        {
            if (!joins.Contains(" InicioPrestacaoCTe "))
                joins.Append(" left join T_LOCALIDADES InicioPrestacaoCTe on CTeTerceiro.CPS_LOCINICIOPRESTACAO = InicioPrestacaoCTe.LOC_CODIGO ");
        }

        private void SetarJoinsGrupoPessoa(StringBuilder joins)
        {
            SetarJoinsCarga(joins);

            if (!joins.Contains(" GrupoPessoa "))
                joins.Append(" left join T_GRUPO_PESSOAS GrupoPessoa on GrupoPessoa.GRP_CODIGO = Carga.GRP_CODIGO ");
        }

        private void SetarJoinsVeiculoCargaOcorrencia(StringBuilder joins)
        {
            SetarJoinsCargaCargaOcorrencia(joins);

            if (!joins.Contains(" VeiculoCargaOcorrencia "))
                joins.Append(" left join T_VEICULO VeiculoCargaOcorrencia ON VeiculoCargaOcorrencia.VEI_CODIGO = CargaCargaOcorrencia.CAR_VEICULO ");
        }

        private void SetarJoinsVeiculoCarga(StringBuilder joins)
        {
            SetarJoinsCarga(joins);

            if (!joins.Contains(" VeiculoCarga "))
                joins.Append(" left join T_VEICULO VeiculoCarga ON VeiculoCarga.VEI_CODIGO = Carga.CAR_VEICULO ");
        }

        private void SetarJoinsFilial(StringBuilder joins)
        {
            SetarJoinsCarga(joins);

            if (!joins.Contains(" Filial "))
                joins.Append(" left join T_FILIAL Filial on Filial.FIL_CODIGO = Carga.FIL_CODIGO ");
        }

        private void SetarJoinsTipoOperacao(StringBuilder joins)
        {
            SetarJoinsCarga(joins);

            if (!joins.Contains(" TipoOperacao "))
                joins.Append(" left join T_TIPO_OPERACAO TipoOperacao on TipoOperacao.TOP_CODIGO = Carga.TOP_CODIGO ");
        }

        private void SetarJoinsTransportador(StringBuilder joins)
        {
            SetarJoinsCarga(joins);

            if (!joins.Contains(" Transportador "))
                joins.Append(" left join T_EMPRESA Transportador on Transportador.EMP_CODIGO = Carga.EMP_CODIGO ");
        }

        private void SetarJoinsCfop(StringBuilder joins)
        {
            if (!joins.Contains(" CFOP "))
                joins.Append(" left join T_CFOP CFOP on CFOP.CFO_CODIGO = CTeTerceiro.CFO_CODIGO ");
        }

        private void SetarJoinsExpedidor(StringBuilder joins)
        {
            if (!joins.Contains(" ExpedidorCTe "))
                joins.Append(" left join T_CTE_PARTICIPANTE ExpedidorCTe on CTeTerceiro.CPS_EXPEDIDOR_CTE = ExpedidorCTe.PCT_CODIGO ");
        }

        private void SetarJoinsExpedidorCliente(StringBuilder joins)
        {
            SetarJoinsExpedidor(joins);

            if (!joins.Contains(" ClienteExpedidor "))
                joins.Append(" left join T_CLIENTE ClienteExpedidor on ClienteExpedidor.CLI_CGCCPF = ExpedidorCTe.CLI_CODIGO ");
        }

        private void SetarJoinsRecebedor(StringBuilder joins)
        {
            if (!joins.Contains(" RecebedorCTe "))
                joins.Append(" left join T_CTE_PARTICIPANTE RecebedorCTe on CTeTerceiro.CPS_RECEBEDOR_CTE = RecebedorCTe.PCT_CODIGO ");
        }

        private void SetarJoinsRecebedorCliente(StringBuilder joins)
        {
            SetarJoinsRecebedor(joins);

            if (!joins.Contains(" ClienteRecebedor "))
                joins.Append(" left join T_CLIENTE ClienteRecebedor on ClienteRecebedor.CLI_CGCCPF = RecebedorCTe.CLI_CODIGO ");
        }

        private void SetarJoinsOutroTomador(StringBuilder joins)
        {
            if (!joins.Contains(" OutroTomadorCTe "))
                joins.Append(" left join T_CTE_PARTICIPANTE OutroTomadorCTe on CTeTerceiro.CPS_TOMADOR_CTE = OutroTomadorCTe.PCT_CODIGO ");
        }

        private void SetarJoinsOutroTomadorCliente(StringBuilder joins)
        {
            SetarJoinsOutroTomador(joins);

            if (!joins.Contains(" ClienteOutroTomador "))
                joins.Append(" left join T_CLIENTE ClienteOutroTomador on ClienteOutroTomador.CLI_CGCCPF = OutroTomadorCTe.CLI_CODIGO ");
        }

        #endregion

        #region Métodos Protegidos Sobrescritos

        protected override void SetarSelect(string propriedade, int codigoDinamico, StringBuilder select, StringBuilder joins, StringBuilder groupBy, bool somenteContarNumeroRegistros, Dominio.ObjetosDeValor.Embarcador.CTe.FiltroPesquisaRelatorioSubcontratacao filtroPesquisa)
        {
            switch (propriedade)
            {
                case "Codigo":
                    if (!select.Contains(" Codigo, "))
                    {
                        select.Append("CTeTerceiro.CPS_CODIGO as Codigo, ");
                        groupBy.Append("CTeTerceiro.CPS_CODIGO, ");
                    }
                    break;

                case "DescricaoSituacaoSEFAZ":
                case "SituacaoSEFAZ":
                    if (!select.Contains(" SituacaoSEFAZ, "))
                    {
                        select.Append("CTeTerceiro.CPS_SITUACAO_SEFAZ SituacaoSEFAZ, ");
                        groupBy.Append("CTeTerceiro.CPS_SITUACAO_SEFAZ, ");
                    }
                    break;

                case "NumeroCarga":
                    if (!select.Contains(" NumeroCarga, "))
                    {
                        select.Append("ISNULL(Carga.CAR_CODIGO_CARGA_EMBARCADOR, CargaCargaOcorrencia.CAR_CODIGO_CARGA_EMBARCADOR) as NumeroCarga, ");
                        groupBy.Append("Carga.CAR_CODIGO_CARGA_EMBARCADOR, CargaCargaOcorrencia.CAR_CODIGO_CARGA_EMBARCADOR, ");

                        SetarJoinsCarga(joins);
                        SetarJoinsCargaCargaOcorrencia(joins);
                    }
                    break;

                case "NumeroCargaEmbarcador":
                    if (!select.Contains(" NumeroCargaEmbarcador, "))
                    {
                        select.Append("CTeTerceiro.CPS_NUMERO_CARGA as NumeroCargaEmbarcador, ");
                        groupBy.Append("CTeTerceiro.CPS_NUMERO_CARGA, ");
                    }
                    break;

                case "GrupoPessoa":
                    if (!select.Contains(" GrupoPessoa, "))
                    {
                        select.Append("GrupoPessoa.GRP_DESCRICAO as GrupoPessoa, ");
                        groupBy.Append("GrupoPessoa.GRP_DESCRICAO, ");

                        SetarJoinsGrupoPessoa(joins);
                    }
                    break;

                case "Placa":
                    if (!select.Contains(" Placa, "))
                    {
                        select.Append("ISNULL(VeiculoCarga.VEI_PLACA, VeiculoCargaOcorrencia.VEI_PLACA) Placa, ");
                        groupBy.Append("VeiculoCarga.VEI_PLACA, VeiculoCargaOcorrencia.VEI_PLACA, ");

                        SetarJoinsVeiculoCarga(joins);
                        SetarJoinsVeiculoCargaOcorrencia(joins);

                    }
                    break;

                case "CapacidadeVeiculo":
                    if (!select.Contains(" CapacidadeVeiculo, "))
                    {
                        select.Append("ISNULL(VeiculoCarga.VEI_CAP_KG, VeiculoCargaOcorrencia.VEI_CAP_KG) CapacidadeVeiculo, ");
                        groupBy.Append("VeiculoCarga.VEI_CAP_KG, VeiculoCargaOcorrencia.VEI_CAP_KG, ");

                        SetarJoinsVeiculoCarga(joins);
                        SetarJoinsVeiculoCargaOcorrencia(joins);
                    }
                    break;

                case "DataCargaFormatada":
                    if (!select.Contains(" DataCarga, "))
                    {
                        select.Append("Carga.CAR_DATA_CRIACAO DataCarga, ");
                        groupBy.Append("Carga.CAR_DATA_CRIACAO, ");

                        SetarJoinsCarga(joins);
                    }
                    break;

                case "DataEmissaoFormatada":
                    if (!select.Contains(" DataEmissao, "))
                    {
                        select.Append("CTeTerceiro.CPS_DATAHORAEMISSAO DataEmissao, ");
                        groupBy.Append("CTeTerceiro.CPS_DATAHORAEMISSAO, ");
                    }
                    break;

                case "Filial":
                    if (!select.Contains(" Filial, "))
                    {
                        select.Append("Filial.FIL_DESCRICAO Filial, ");
                        groupBy.Append("Filial.FIL_DESCRICAO, ");

                        SetarJoinsFilial(joins);
                    }
                    break;

                case "TipoOperacao":
                    if (!select.Contains(" TipoOperacao, "))
                    {
                        select.Append("TipoOperacao.TOP_DESCRICAO TipoOperacao, ");
                        groupBy.Append("TipoOperacao.TOP_DESCRICAO, ");

                        SetarJoinsTipoOperacao(joins);
                    }
                    break;

                case "CodigoEmpresa":
                    if (!select.Contains(" CodigoEmpresa, "))
                    {
                        select.Append("Transportador.EMP_CODIGO_INTEGRACAO CodigoEmpresa, ");
                        groupBy.Append("Transportador.EMP_CODIGO_INTEGRACAO, ");

                        SetarJoinsTransportador(joins);
                    }
                    break;
                case "CNPJEmpresa":
                    if (!select.Contains(" CNPJEmpresaSemFormato, "))
                    {
                        select.Append("Transportador.EMP_CNPJ CNPJEmpresaSemFormato, ");
                        groupBy.Append("Transportador.EMP_CNPJ, ");

                        SetarJoinsTransportador(joins);
                    }
                    break;
                case "Empresa":
                    if (!select.Contains(" Empresa, "))
                    {
                        select.Append("Transportador.EMP_RAZAO Empresa, ");
                        groupBy.Append("Transportador.EMP_RAZAO, ");

                        SetarJoinsTransportador(joins);
                    }
                    break;

                case "CNPJRemetente":
                    if (!select.Contains("CNPJRemetenteSemFormato"))
                    {
                        select.Append("ClienteRemetente.CLI_CGCCPF CNPJRemetenteSemFormato, ClienteRemetente.CLI_FISJUR TipoPessoaRemetente, ");
                        groupBy.Append("ClienteRemetente.CLI_CGCCPF, ClienteRemetente.CLI_FISJUR, ");

                        SetarJoinsRemetenteCliente(joins);
                    }
                    break;
                case "CodigoRemetente":
                    if (!select.Contains(" CodigoRemetente, "))
                    {
                        select.Append("ClienteRemetente.CLI_CODIGO_INTEGRACAO CodigoRemetente, ");
                        groupBy.Append("ClienteRemetente.CLI_CODIGO_INTEGRACAO, ");

                        SetarJoinsRemetenteCliente(joins);
                    }
                    break;
                case "Remetente":
                    if (!select.Contains(" Remetente, "))
                    {
                        select.Append("ClienteRemetente.CLI_NOME Remetente, ");
                        groupBy.Append("ClienteRemetente.CLI_NOME, ");

                        SetarJoinsRemetenteCliente(joins);
                    }
                    break;
                case "EnderecoRemetente":
                    if (!select.Contains(" EnderecoRemetente, "))
                    {
                        select.Append("isnull(ClienteRemetente.CLI_ENDERECO, '') + ', ' + isnull(ClienteRemetente.CLI_NUMERO, '') + ', ' + isnull(ClienteRemetente.CLI_BAIRRO, '') EnderecoRemetente, ");
                        groupBy.Append("ClienteRemetente.CLI_ENDERECO, ClienteRemetente.CLI_NUMERO, ClienteRemetente.CLI_BAIRRO, ");

                        SetarJoinsRemetenteCliente(joins);
                    }
                    break;
                case "GrupoRemetente":
                    if (!select.Contains(" GrupoRemetente, "))
                    {
                        select.Append("GrupoPessoaRemetente.GRP_DESCRICAO GrupoRemetente, ");
                        groupBy.Append("GrupoPessoaRemetente.GRP_DESCRICAO, ");

                        SetarJoinsRemetenteGrupoPessoa(joins);
                    }
                    break;
                case "CategoriaRemetente":
                    if (!select.Contains(" CategoriaRemetente, "))
                    {
                        select.Append("CategoriaRemetente.CTP_DESCRICAO CategoriaRemetente, ");
                        groupBy.Append("CategoriaRemetente.CTP_DESCRICAO, ");

                        SetarJoinsRemetenteCategoria(joins);
                    }
                    break;

                case "Origem":
                    if (!select.Contains(" Origem,"))
                    {
                        select.Append(" InicioPrestacaoCTe.LOC_DESCRICAO + '-' + InicioPrestacaoCTe.UF_SIGLA Origem, ");
                        groupBy.Append("InicioPrestacaoCTe.LOC_DESCRICAO, InicioPrestacaoCTe.UF_SIGLA, ");

                        SetarJoinsLocalidadeInicioPrestacao(joins);
                    }
                    break;

                case "CodigoDestinatario":
                    if (!select.Contains(" CodigoDestinatario, "))
                    {
                        select.Append("ClienteDestinatario.CLI_CODIGO_INTEGRACAO CodigoDestinatario, ");
                        groupBy.Append("ClienteDestinatario.CLI_CODIGO_INTEGRACAO, ");

                        SetarJoinsDestinatarioCliente(joins);
                    }
                    break;
                case "CNPJDestinatario":
                    if (!select.Contains("CNPJDestinatarioSemFormato"))
                    {
                        select.Append("ClienteDestinatario.CLI_CGCCPF CNPJDestinatarioSemFormato, ClienteDestinatario.CLI_FISJUR TipoPessoaDestinatario, ");
                        groupBy.Append("ClienteDestinatario.CLI_CGCCPF, ClienteDestinatario.CLI_FISJUR, ");

                        SetarJoinsDestinatarioCliente(joins);
                    }
                    break;
                case "Destinatario":
                    if (!select.Contains(" Destinatario, "))
                    {
                        select.Append("ClienteDestinatario.CLI_NOME Destinatario, ");
                        groupBy.Append("ClienteDestinatario.CLI_NOME, ");

                        SetarJoinsDestinatarioCliente(joins);
                    }
                    break;
                case "EnderecoDestinatario":
                    if (!select.Contains(" EnderecoDestinatario, "))
                    {
                        select.Append("isnull(ClienteDestinatario.CLI_ENDERECO, '') + ', ' + isnull(ClienteDestinatario.CLI_NUMERO, '') + ', ' + isnull(ClienteDestinatario.CLI_BAIRRO, '') EnderecoDestinatario, ");
                        groupBy.Append("ClienteDestinatario.CLI_ENDERECO, ClienteDestinatario.CLI_NUMERO, ClienteDestinatario.CLI_BAIRRO, ");

                        SetarJoinsDestinatarioCliente(joins);
                    }
                    break;
                case "TelefoneDestinatario":
                    if (!select.Contains(" TelefoneDestinatario, "))
                    {
                        select.Append("ClienteDestinatario.CLI_FONE TelefoneDestinatario, ");
                        groupBy.Append("ClienteDestinatario.CLI_FONE, ");

                        SetarJoinsDestinatarioCliente(joins);
                    }
                    break;
                case "EmailDestinatario":
                    if (!select.Contains(" EmailDestinatario, "))
                    {
                        select.Append("ClienteDestinatario.CLI_EMAIL EmailDestinatario, ");
                        groupBy.Append("ClienteDestinatario.CLI_EMAIL, ");

                        SetarJoinsDestinatarioCliente(joins);
                    }
                    break;
                case "GrupoDestinatario":
                    if (!select.Contains(" GrupoDestinatario, "))
                    {
                        select.Append("GrupoPessoaDestinatario.GRP_DESCRICAO GrupoDestinatario, ");
                        groupBy.Append("GrupoPessoaDestinatario.GRP_DESCRICAO, ");

                        SetarJoinsDestinatarioGrupoPessoa(joins);
                    }
                    break;
                case "CategoriaDestinatario":
                    if (!select.Contains(" CategoriaDestinatario, "))
                    {
                        select.Append("CategoriaDestinatario.CTP_DESCRICAO CategoriaDestinatario, ");
                        groupBy.Append("CategoriaDestinatario.CTP_DESCRICAO, ");

                        SetarJoinsDestinatarioCategoria(joins);
                    }
                    break;

                case "Destino":
                    if (!select.Contains(" Destino, "))
                    {
                        select.Append(" FimPrestacaoCTe.LOC_DESCRICAO + '-' + FimPrestacaoCTe.UF_SIGLA Destino, ");
                        groupBy.Append("FimPrestacaoCTe.LOC_DESCRICAO, FimPrestacaoCTe.UF_SIGLA, ");

                        SetarJoinsLocalidadeFimPrestacao(joins);
                    }
                    break;

                case "NumeroCTe":
                    if (!select.Contains("NumeroCTe"))
                    {
                        select.Append("CTeTerceiro.CPS_NUMERO NumeroCTe, ");
                        groupBy.Append("CTeTerceiro.CPS_NUMERO, ");
                    }
                    break;

                case "SerieCTe":
                    if (!select.Contains("SerieCTe"))
                    {
                        select.Append("CTeTerceiro.CPS_SERIE SerieCTe, ");
                        groupBy.Append("CTeTerceiro.CPS_SERIE, ");
                    }
                    break;

                case "ChaveCTe":
                    if (!select.Contains("ChaveCTe"))
                    {
                        select.Append("CTeTerceiro.CPS_CHAVE_ACESSO ChaveCTe, ");
                        groupBy.Append("CTeTerceiro.CPS_CHAVE_ACESSO, ");
                    }
                    break;

                case "Peso":
                    if (!select.Contains("Peso"))
                    {
                        select.Append("CTeTerceiro.CPS_PESO Peso, ");
                        groupBy.Append("CTeTerceiro.CPS_PESO, ");
                    }
                    break;

                case "Volumes":
                    if (!select.Contains("Volumes"))
                    {
                        select.Append(" (SELECT SUM(NotasFiscaisCTe.CNE_VOLUMES) FROM T_CTE_TERCEIRO_NFE NotasFiscaisCTe WHERE NotasFiscaisCTe.CPS_CODIGO = CTeTerceiro.CPS_CODIGO) Volumes, ");

                        if (!groupBy.Contains("CTeTerceiro.CPS_CODIGO"))
                            groupBy.Append("CTeTerceiro.CPS_CODIGO, ");
                    }
                    break;

                case "CFOP":
                    if (!select.Contains(" CFOP"))
                    {
                        select.Append("CFOP.CFO_CFOP CFOP, ");
                        groupBy.Append("CFOP.CFO_CFOP, ");

                        SetarJoinsCfop(joins);
                    }
                    break;

                case "CST":
                    if (!select.Contains("CST"))
                    {
                        select.Append("CTeTerceiro.CPS_CST CST, ");
                        groupBy.Append("CTeTerceiro.CPS_CST, ");
                    }
                    break;

                case "AliquotaICMS":
                    if (!somenteContarNumeroRegistros && !select.Contains("AliquotaICMS"))
                    {
                        select.Append("CTeTerceiro.CPS_ALIQ_ICMS AliquotaICMS, ");
                        groupBy.Append("CTeTerceiro.CPS_ALIQ_ICMS, ");
                    }
                    break;

                case "ValorICMS":
                    if (!somenteContarNumeroRegistros && !select.Contains("ValorICMS"))
                    {
                        select.Append("CTeTerceiro.CPS_VAL_ICMS ValorICMS, ");
                        groupBy.Append("CTeTerceiro.CPS_VAL_ICMS, ");
                    }
                    break;

                case "ValorReceber":
                    if (!somenteContarNumeroRegistros && !select.Contains("ValorReceber"))
                    {
                        select.Append("CTeTerceiro.CPS_VALOR_RECEBER ValorReceber, ");
                        groupBy.Append("CTeTerceiro.CPS_VALOR_RECEBER, ");
                    }
                    break;

                case "ValorPrestacao":
                    if (!somenteContarNumeroRegistros && !select.Contains("ValorPrestacao"))
                    {
                        select.Append("CTeTerceiro.CPS_VALOR_PREST_SERVICO ValorPrestacao, ");
                        groupBy.Append("CTeTerceiro.CPS_VALOR_PREST_SERVICO, ");
                    }
                    break;

                case "ValorMercadoria":
                    if (!somenteContarNumeroRegistros && !select.Contains("ValorMercadoria"))
                    {
                        select.Append("CTeTerceiro.CPS_VALOR_TOTAL_MERC ValorMercadoria, ");
                        groupBy.Append("CTeTerceiro.CPS_VALOR_TOTAL_MERC, ");
                    }
                    break;

                case "Motorista":
                    if (!select.Contains("Motorista, "))
                    {
                        select.Append("SUBSTRING(( ");
                        select.Append("    SELECT DISTINCT ', ' + Motorista.FUN_NOME ");
                        select.Append("      FROM T_CARGA_MOTORISTA CargaMotorista ");
                        select.Append("      JOIN T_FUNCIONARIO Motorista ");
                        select.Append("        ON CargaMotorista.CAR_MOTORISTA = Motorista.FUN_CODIGO ");
                        select.Append("     WHERE CargaMotorista.CAR_CODIGO = Carga.CAR_CODIGO ");
                        select.Append("       FOR XML PATH('') ");
                        select.Append("), 3, 4000) Motorista, ");

                        if (!groupBy.Contains("Carga.CAR_CODIGO,"))
                            groupBy.Append("Carga.CAR_CODIGO, ");

                        SetarJoinsCarga(joins);
                    }
                    break;

                case "DescricaoTipoServico":
                    if (!select.Contains(" TipoServico, "))
                    {
                        select.Append("CTeTerceiro.CPS_TIPO_SERVICO TipoServico, ");
                        groupBy.Append("CTeTerceiro.CPS_TIPO_SERVICO, ");
                    }
                    break;

                case "DescricaoTipoTomador":
                case "TipoTomador":
                    if (!select.Contains(" TipoTomador, "))
                    {
                        select.Append("CTeTerceiro.CPS_TOMADOR TipoTomador, ");
                        groupBy.Append("CTeTerceiro.CPS_TOMADOR, ");
                    }
                    break;

                case "DescricaoTipoCTe":
                    if (!select.Contains(" TipoCTe, "))
                    {
                        select.Append("CTeTerceiro.CPS_TIPO_CTE TipoCTe, ");
                        groupBy.Append("CTeTerceiro.CPS_TIPO_CTE, ");
                    }
                    break;

                case "CNPJExpedidor":
                    if (!select.Contains("CNPJExpedidorSemFormato"))
                    {
                        select.Append("ClienteExpedidor.CLI_CGCCPF CNPJExpedidorSemFormato, ClienteExpedidor.CLI_FISJUR TipoPessoaExpedidor, ");
                        groupBy.Append("ClienteExpedidor.CLI_CGCCPF, ClienteExpedidor.CLI_FISJUR, ");

                        SetarJoinsExpedidorCliente(joins);
                    }
                    break;
                case "Expedidor":
                    if (!select.Contains(" Expedidor, "))
                    {
                        select.Append("ClienteExpedidor.CLI_NOME Expedidor, ");
                        groupBy.Append("ClienteExpedidor.CLI_NOME, ");

                        SetarJoinsExpedidorCliente(joins);
                    }
                    break;

                case "CNPJRecebedor":
                    if (!select.Contains("CNPJRecebedorSemFormato"))
                    {
                        select.Append("ClienteRecebedor.CLI_CGCCPF CNPJRecebedorSemFormato, ClienteRecebedor.CLI_FISJUR TipoPessoaRecebedor, ");
                        groupBy.Append("ClienteRecebedor.CLI_CGCCPF, ClienteRecebedor.CLI_FISJUR, ");

                        SetarJoinsRecebedorCliente(joins);
                    }
                    break;
                case "Recebedor":
                    if (!select.Contains(" Recebedor, "))
                    {
                        select.Append("ClienteRecebedor.CLI_NOME Recebedor, ");
                        groupBy.Append("ClienteRecebedor.CLI_NOME, ");

                        SetarJoinsRecebedorCliente(joins);
                    }
                    break;

                case "CNPJOutroTomador":
                    if (!select.Contains("CNPJOutroTomadorSemFormato"))
                    {
                        select.Append("ClienteOutroTomador.CLI_CGCCPF CNPJOutroTomadorSemFormato, ClienteOutroTomador.CLI_FISJUR TipoPessoaOutroTomador, ");
                        groupBy.Append("ClienteOutroTomador.CLI_CGCCPF, ClienteOutroTomador.CLI_FISJUR, ");

                        SetarJoinsOutroTomadorCliente(joins);
                    }
                    break;
                case "OutroTomador":
                    if (!select.Contains(" OutroTomador, "))
                    {
                        select.Append("ClienteOutroTomador.CLI_NOME OutroTomador, ");
                        groupBy.Append("ClienteOutroTomador.CLI_NOME, ");

                        SetarJoinsOutroTomadorCliente(joins);
                    }
                    break;

                case "Ocorrencia":
                    if (!select.Contains(" Ocorrencia, "))
                    {
                        select.Append("CAST(CargaOcorrencia.COC_NUMERO_CONTRATO AS VARCHAR(20)) Ocorrencia, ");
                        groupBy.Append("CargaOcorrencia.COC_NUMERO_CONTRATO, ");

                        SetarJoinsCargaCargaOcorrencia(joins);
                    }
                    break;

                case "CNPJTomador":
                    SetarSelect("TipoTomador", codigoDinamico, select, joins, groupBy, somenteContarNumeroRegistros, filtroPesquisa);
                    SetarSelect("CNPJRemetente", codigoDinamico, select, joins, groupBy, somenteContarNumeroRegistros, filtroPesquisa);
                    SetarSelect("CNPJDestinatario", codigoDinamico, select, joins, groupBy, somenteContarNumeroRegistros, filtroPesquisa);
                    SetarSelect("CNPJExpedidor", codigoDinamico, select, joins, groupBy, somenteContarNumeroRegistros, filtroPesquisa);
                    SetarSelect("CNPJRecebedor", codigoDinamico, select, joins, groupBy, somenteContarNumeroRegistros, filtroPesquisa);
                    SetarSelect("CNPJOutroTomador", codigoDinamico, select, joins, groupBy, somenteContarNumeroRegistros, filtroPesquisa);
                    break;
                case "Tomador":
                    SetarSelect("TipoTomador", codigoDinamico, select, joins, groupBy, somenteContarNumeroRegistros, filtroPesquisa);
                    SetarSelect("Remetente", codigoDinamico, select, joins, groupBy, somenteContarNumeroRegistros, filtroPesquisa);
                    SetarSelect("Destinatario", codigoDinamico, select, joins, groupBy, somenteContarNumeroRegistros, filtroPesquisa);
                    SetarSelect("Expedidor", codigoDinamico, select, joins, groupBy, somenteContarNumeroRegistros, filtroPesquisa);
                    SetarSelect("Recebedor", codigoDinamico, select, joins, groupBy, somenteContarNumeroRegistros, filtroPesquisa);
                    SetarSelect("OutroTomador", codigoDinamico, select, joins, groupBy, somenteContarNumeroRegistros, filtroPesquisa);
                    break;

                case "Observacao":
                    if (!select.Contains(" Observacao, "))
                    {
                        select.Append("CTeTerceiro.CPS_OBSERVACAO_GERAL Observacao, ");
                        groupBy.Append("CTeTerceiro.CPS_OBSERVACAO_GERAL, ");
                    }
                    break;

                case "SituacaoCargaFormatada":
                    if (!select.Contains(" SituacaoCarga, "))
                    {
                        select.Append("Carga.CAR_SITUACAO SituacaoCarga, ");
                        groupBy.Append("Carga.CAR_SITUACAO, ");

                        SetarJoinsCarga(joins);
                    }
                    break;
            }
        }

        protected override void SetarWhere(Dominio.ObjetosDeValor.Embarcador.CTe.FiltroPesquisaRelatorioSubcontratacao filtrosPesquisa, StringBuilder where, StringBuilder joins, StringBuilder groupBy, List<Embarcador.Consulta.ParametroSQL> parametros = null)
        {
            string pattern = "yyyy-MM-dd";

            SetarJoinsCarga(joins);

            if (!string.IsNullOrWhiteSpace(filtrosPesquisa.NumeroCargaEmbarcador))
                where.Append($" and CTeTerceiro.CPS_NUMERO_CARGA = '{filtrosPesquisa.NumeroCargaEmbarcador}'");

            if (filtrosPesquisa.DataInicialEmissao != DateTime.MinValue)
                where.Append(" and CAST(CTeTerceiro.CPS_DATAHORAEMISSAO AS DATE) >= '" + filtrosPesquisa.DataInicialEmissao.ToString(pattern) + "'");

            if (filtrosPesquisa.DataFinalEmissao != DateTime.MinValue)
                where.Append(" and CAST(CTeTerceiro.CPS_DATAHORAEMISSAO AS DATE) <= '" + filtrosPesquisa.DataFinalEmissao.ToString(pattern) + "'");

            if (filtrosPesquisa.DataInicialEmissaoCarga != DateTime.MinValue)
                where.Append(" and Carga.CAR_DATA_CRIACAO >= '" + filtrosPesquisa.DataInicialEmissaoCarga.ToString(pattern) + "'");

            if (filtrosPesquisa.DataFinalEmissaoCarga != DateTime.MinValue)
                where.Append(" and Carga.CAR_DATA_CRIACAO < '" + filtrosPesquisa.DataFinalEmissaoCarga.AddDays(1).ToString(pattern) + "'");

            if (filtrosPesquisa.DataInicialFinalizacaoEmissao != DateTime.MinValue)
                where.Append(" and Carga.CAR_DATA_FINALIZACAO_EMISSAO >= '" + filtrosPesquisa.DataInicialFinalizacaoEmissao.ToString(pattern) + "'");

            if (filtrosPesquisa.DataFinalFinalizacaoEmissao != DateTime.MinValue)
                where.Append(" and Carga.CAR_DATA_FINALIZACAO_EMISSAO < '" + filtrosPesquisa.DataFinalFinalizacaoEmissao.AddDays(1).ToString(pattern) + "'");

            if (filtrosPesquisa.NumeroInicial > 0)
                where.Append(" and CTeTerceiro.CPS_NUMERO >= " + filtrosPesquisa.NumeroInicial.ToString());

            if (filtrosPesquisa.NumeroFinal > 0)
                where.Append(" and CTeTerceiro.CPS_NUMERO <= " + filtrosPesquisa.NumeroFinal.ToString());

            if (filtrosPesquisa.CodigosTransportador.Count > 0)
                where.Append(" and Carga.EMP_CODIGO in (" + string.Join(", ", filtrosPesquisa.CodigosTransportador) + ")");

            if (filtrosPesquisa.CodigosCarga.Count > 0)
                where.Append(" and Carga.CAR_CODIGO in (" + string.Join(", ", filtrosPesquisa.CodigosCarga) + ")");

            if (filtrosPesquisa.CodigosGrupoPessoas.Count > 0)
                where.Append(" and Carga.GRP_CODIGO in (" + string.Join(", ", filtrosPesquisa.CodigosGrupoPessoas) + ")");

            if (filtrosPesquisa.CodigosFilial.Count > 0)
                where.Append(" and Carga.FIL_CODIGO in (" + string.Join(", ", filtrosPesquisa.CodigosFilial) + ")");

            if (filtrosPesquisa.CodigosTipoCarga.Count > 0)
                where.Append($" and (Carga.TCG_CODIGO in ({string.Join(", ", filtrosPesquisa.CodigosTipoCarga)}){(filtrosPesquisa.CodigosTipoCarga.Contains(-1) ? " or Carga.TCG_CODIGO is null" : "")})");

            if (filtrosPesquisa.CodigosTipoOperacao.Count > 0)
                where.Append($" and (Carga.TOP_CODIGO in ({string.Join(", ", filtrosPesquisa.CodigosTipoOperacao)}){(filtrosPesquisa.CodigosTipoOperacao.Contains(-1) ? " or Carga.TOP_CODIGO is null" : "")})");

            if (filtrosPesquisa.CodigosOrigem.Count > 0)
                where.Append("  and CTeTerceiro.CPS_LOCINICIOPRESTACAO in (" + string.Join(", ", filtrosPesquisa.CodigosOrigem) + ")");

            if (filtrosPesquisa.CodigosDestino.Count > 0)
                where.Append("  and CTeTerceiro.CPS_LOCTERMINOPRESTACAO in (" + string.Join(", ", filtrosPesquisa.CodigosDestino) + ")");

            if (filtrosPesquisa.EstadosOrigem.Count > 0)
            {
                where.Append(" and InicioPrestacaoCTe.UF_SIGLA in (" + string.Join(", ", from o in filtrosPesquisa.EstadosOrigem select "'" + o + "'") + ")");

                SetarJoinsLocalidadeInicioPrestacao(joins);
            }

            if (filtrosPesquisa.EstadosDestino.Count > 0)
            {
                where.Append(" and FimPrestacaoCTe.UF_SIGLA in (" + string.Join(", ", from o in filtrosPesquisa.EstadosDestino select "'" + o + "'") + ")");

                SetarJoinsLocalidadeFimPrestacao(joins);
            }

            if (filtrosPesquisa.CpfCnpjsRemetente.Count > 0)
            {
                where.Append(" and RemetenteCTe.CLI_CODIGO in (" + string.Join(", ", filtrosPesquisa.CpfCnpjsRemetente) + ")");

                SetarJoinsRemetente(joins);
            }

            if (filtrosPesquisa.CpfCnpjsDestinatario.Count > 0)
            {
                where.Append(" and DestinatarioCTe.CLI_CODIGO in (" + string.Join(", ", filtrosPesquisa.CpfCnpjsDestinatario) + ")");

                SetarJoinsDestinatario(joins);
            }

            if (filtrosPesquisa.TiposServicos?.Count > 0)
                where.Append(" and CTeTerceiro.CPS_TIPO_SERVICO in ('" + string.Join("', '", filtrosPesquisa.TiposServicos) + "')");

            if (filtrosPesquisa.TiposCTe?.Count > 0)
                where.Append($" and CTeTerceiro.CPS_TIPO_CTE in ({string.Join(", ", filtrosPesquisa.TiposCTe.Select(o => o.ToString("D"))) })");

            if (filtrosPesquisa.SituacaoCarga.Count > 0)
                where.Append($" and Carga.CAR_SITUACAO in ({ string.Join(", ", filtrosPesquisa.SituacaoCarga.Select(o => o.ToString("D"))) })");

            if (filtrosPesquisa.SituacoesCargaMercante != null && filtrosPesquisa.SituacoesCargaMercante.Count > 0)
            {
                where.Append($" and (1 = 0 ");
                if (filtrosPesquisa.SituacoesCargaMercante.Any(o => o == SituacaoCargaMercante.Cancelada))
                    where.Append($" or Carga.CAR_SITUACAO = 13");
                if (filtrosPesquisa.SituacoesCargaMercante.Any(o => o == SituacaoCargaMercante.Anulada))
                    where.Append($" or Carga.CAR_SITUACAO = 18");
                if (filtrosPesquisa.SituacoesCargaMercante.Any(o => o == SituacaoCargaMercante.AguardandoEmissao))
                    where.Append($" or (Carga.CAR_SITUACAO = 5 and Carga.CAR_DATA_RECEBIMENTO_ULTIMA_NFE is not null)");
                if (filtrosPesquisa.SituacoesCargaMercante.Any(o => o == SituacaoCargaMercante.PendenteEmissaoCTe))
                    where.Append($" or (Carga.CAR_SITUACAO = 5 and Carga.CAR_DATA_RECEBIMENTO_ULTIMA_NFE is null)");
                if (filtrosPesquisa.SituacoesCargaMercante.Any(o => o == SituacaoCargaMercante.PendenteMDFe))
                    where.Append($" or (Carga.CAR_MDFE_AQUAVIARIO_VINCULADO != 1 and Carga.CAR_SITUACAO != 13 and Carga.CAR_SITUACAO != 18 and Carga.CAR_CODIGO in (select DISTINCT _cargaPedido.CAR_CODIGO from T_CARGA_PEDIDO _cargaPedido where _cargaPedido.TBF_TIPO_COBRANCA_MULTIMODAL = 5))");
                if (filtrosPesquisa.SituacoesCargaMercante.Any(o => o == SituacaoCargaMercante.PendenteMercante))
                    where.Append($" or (Carga.CAR_TODOS_CTES_COM_MERCANTE != 1 and Carga.CAR_SITUACAO != 13 and Carga.CAR_SITUACAO != 18 and Carga.CAR_CODIGO in (select DISTINCT _cargaPedido.CAR_CODIGO from T_CARGA_PEDIDO _cargaPedido where _cargaPedido.TBF_MODAL_PROPOSTA_MULTIMODAL = 4 or _cargaPedido.TBF_TIPO_SERVICO_MULTIMODAL = 4 or _cargaPedido.TBF_TIPO_SERVICO_MULTIMODAL = 5))");
                if (filtrosPesquisa.SituacoesCargaMercante.Any(o => o == SituacaoCargaMercante.PendenteFaturamento))
                    where.Append($" or (Carga.CAR_TODOS_CTES_FATURADOS != 1 and Carga.CAR_SITUACAO != 13 and Carga.CAR_SITUACAO != 18 and Carga.CAR_CODIGO in (select DISTINCT _cargaPedido.CAR_CODIGO from T_CARGA_PEDIDO _cargaPedido where _cargaPedido.TBF_MODAL_PROPOSTA_MULTIMODAL = 1 or _cargaPedido.TBF_MODAL_PROPOSTA_MULTIMODAL = 3 or _cargaPedido.TBF_MODAL_PROPOSTA_MULTIMODAL = 4 or _cargaPedido.TBF_TIPO_PROPOSTA_MULTIMODAL = 3))");
                if (filtrosPesquisa.SituacoesCargaMercante.Any(o => o == SituacaoCargaMercante.PendenteIntegracaoCTe))
                    where.Append($" or Carga.CAR_SITUACAO = 15");
                if (filtrosPesquisa.SituacoesCargaMercante.Any(o => o == SituacaoCargaMercante.PendenteIntegracaoFatura))
                    where.Append($" or (Carga.CAR_TODOS_CTES_FATURADOS_INTEGRADOS != 1 and Carga.CAR_SITUACAO != 13 and Carga.CAR_SITUACAO != 18 and Carga.CAR_CODIGO in (select DISTINCT _cargaPedido.CAR_CODIGO from T_CARGA_PEDIDO _cargaPedido where _cargaPedido.TBF_MODAL_PROPOSTA_MULTIMODAL = 1 or _cargaPedido.TBF_MODAL_PROPOSTA_MULTIMODAL = 3 or _cargaPedido.TBF_MODAL_PROPOSTA_MULTIMODAL = 4 or _cargaPedido.TBF_TIPO_PROPOSTA_MULTIMODAL = 3))");
                if (filtrosPesquisa.SituacoesCargaMercante.Any(o => o == SituacaoCargaMercante.PendenteSVM))
                {
                    where.Append($" or (Carga.CAR_SITUACAO != 13 and Carga.CAR_SITUACAO != 18 and Carga.CAR_CODIGO in (select DISTINCT _cargaPedido.CAR_CODIGO from T_CARGA_PEDIDO _cargaPedido where _cargaPedido.TBF_MODAL_PROPOSTA_MULTIMODAL = 3) and ");
                    where.Append($@" exists (
                    select
                        cargacte5_.CCT_CODIGO 
                    from
                        T_CARGA_CTE cargactes4_,
                        T_CARGA_CTE cargacte5_ 
                    left outer join
                        T_CTE conhecimen6_ 
                            on cargacte5_.CON_CODIGO=conhecimen6_.CON_CODIGO 
                    where
                        Carga.CAR_CODIGO=cargactes4_.CAR_CODIGO 
                        and cargactes4_.CCT_CODIGO=cargacte5_.CCT_CODIGO 
                        and  not (exists (select
                            ctesvmmult7_.CSM_CODIGO 
                        from
                            T_CTE_SVM_MULTIMODAL ctesvmmult7_ 
                        inner join
                            T_CTE conhecimen8_ 
                                on ctesvmmult7_.CON_CODIGO_SVM=conhecimen8_.CON_CODIGO 
                        inner join
                            T_CTE conhecimen9_ 
                                on ctesvmmult7_.CON_CODIGO_MULTIMODAL=conhecimen9_.CON_CODIGO 
                        where
                            conhecimen8_.CON_STATUS='A'
                            and conhecimen9_.CON_TIPO_CTE=0
                            and (conhecimen9_.CON_CODIGO=conhecimen6_.CON_CODIGO 
                            or (conhecimen9_.CON_CODIGO is null) 
                            and (conhecimen6_.CON_CODIGO is null)))))
                    )");
                }
                if (filtrosPesquisa.SituacoesCargaMercante.Any(o => o == SituacaoCargaMercante.ComErro))
                    where.Append($" or Carga.CAR_SITUACAO = 15 or Carga.CAR_SITUACAO = 6 or Carga.CAR_PROBLEMA_CTE = 1");
                if (filtrosPesquisa.SituacoesCargaMercante.Any(o => o == SituacaoCargaMercante.Finalizada))
                {
                    where.Append($" or (Carga.CAR_SITUACAO = 11 and ");
                    where.Append($" (Carga.CAR_TODOS_CTES_COM_MERCANTE = 1 or Carga.CAR_CODIGO not in (select DISTINCT _cargaPedido.CAR_CODIGO from T_CARGA_PEDIDO _cargaPedido where _cargaPedido.TBF_MODAL_PROPOSTA_MULTIMODAL = 4 or _cargaPedido.TBF_TIPO_SERVICO_MULTIMODAL = 4)) and ");
                    where.Append($" (Carga.CAR_TODOS_CTES_COM_MANIFESTO = 1 or Carga.CAR_CODIGO not in (select DISTINCT _cargaPedido.CAR_CODIGO from T_CARGA_PEDIDO _cargaPedido where _cargaPedido.TBF_MODAL_PROPOSTA_MULTIMODAL = 4 or _cargaPedido.TBF_TIPO_SERVICO_MULTIMODAL = 4)) and ");
                    where.Append($" (Carga.CAR_TODOS_CTES_FATURADOS = 1 or Carga.CAR_CODIGO not in (select DISTINCT _cargaPedido.CAR_CODIGO from T_CARGA_PEDIDO _cargaPedido where _cargaPedido.TBF_MODAL_PROPOSTA_MULTIMODAL = 1 or _cargaPedido.TBF_MODAL_PROPOSTA_MULTIMODAL = 3 or _cargaPedido.TBF_MODAL_PROPOSTA_MULTIMODAL = 4 or _cargaPedido.TBF_TIPO_PROPOSTA_MULTIMODAL = 3)) and ");
                    where.Append($" (Carga.CAR_MDFE_AQUAVIARIO_VINCULADO = 1 or Carga.CAR_CODIGO not in (select DISTINCT _cargaPedido.CAR_CODIGO from T_CARGA_PEDIDO _cargaPedido where _cargaPedido.TBF_TIPO_COBRANCA_MULTIMODAL = 5))) ");
                }
                where.Append($" )");
            }

            if (filtrosPesquisa.SituacaoSEFAZ?.Count > 0)
                where.Append($" and CTeTerceiro.CPS_SITUACAO_SEFAZ in ({ string.Join(", ", filtrosPesquisa.SituacaoSEFAZ.Select(o => o.ToString("D"))) })");
        }

        #endregion
    }
}
