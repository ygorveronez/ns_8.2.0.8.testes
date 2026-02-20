using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Repositorio.Embarcador.Produtos.Consulta
{
    sealed class ConsultaProdutoEmbarcador : Embarcador.Consulta.Consulta<Dominio.ObjetosDeValor.Embarcador.Produtos.FiltroPesquisaRelatorioProdutoEmbarcador>
    {
        #region Construtores

        public ConsultaProdutoEmbarcador() : base(tabela: "T_PRODUTO_EMBARCADOR as ProdutoEmbarcador") { }

        #endregion

        #region Métodos Privados

        private void SetarJoinsPedido(StringBuilder joins)
        {
            SetarJoinsPedidoProduto(joins);
            if (!joins.Contains(" Pedido "))
                joins.Append(" inner join T_PEDIDO Pedido on PedidoProduto.PED_CODIGO = Pedido.PED_CODIGO ");
        }

        private void SetarJoinsCargaPedido(StringBuilder joins)
        {
            SetarJoinsPedido(joins);
            if (!joins.Contains(" CargaPedido "))
                joins.Append(" left join T_CARGA_PEDIDO CargaPedido on CargaPedido.PED_CODIGO = Pedido.PED_CODIGO ");
        }

        private void SetarJoinsEnderecoProduto(StringBuilder joins)
        {
            if (!joins.Contains(" EnderecoProduto "))
                joins.Append(" left join T_ENDERECO_PRODUTO EnderecoProduto on EnderecoProduto.CEP_CODIGO = PedidoProduto.CEP_CODIGO ");
        }

        private void SetarJoinsPedidoProduto(StringBuilder joins)
        {
            if (!joins.Contains(" PedidoProduto "))
                joins.Append(" inner join T_PEDIDO_PRODUTO PedidoProduto on PedidoProduto.PRO_CODIGO = ProdutoEmbarcador.PRO_CODIGO ");
        }

        private void SetarJoinsFilial(StringBuilder joins)
        {
            SetarJoinsPedido(joins);
            if (!joins.Contains(" Filial "))
                joins.Append(" left join T_FILIAL Filial on Filial.FIL_CODIGO = Pedido.FIL_CODIGO ");
        }

        private void SetarJoinsLocalidade(StringBuilder joins)
        {
            SetarJoinsPedido(joins);
            if (!joins.Contains(" Localidade "))
                joins.Append(" left join T_LOCALIDADES Localidade on Localidade.LOC_CODIGO = Pedido.LOC_CODIGO_ORIGEM ");
        }

        private void SetarJoinsDestinatario(StringBuilder joins)
        {
            SetarJoinsPedido(joins);
            if (!joins.Contains(" Destinatario "))
                joins.Append(" left join T_CLIENTE Destinatario on Destinatario.CLI_CGCCPF = Pedido.CLI_CODIGO ");
        }

        private void SetarJoinsRemetente(StringBuilder joins)
        {
            SetarJoinsPedido(joins);
            if (!joins.Contains(" Remetente "))
                joins.Append(" left join T_CLIENTE Remetente on Remetente.CLI_CGCCPF = Pedido.CLI_CODIGO_REMETENTE ");
        }

        private void SetarJoinsTipoCarga(StringBuilder joins)
        {
            SetarJoinsPedido(joins);
            if (!joins.Contains(" TipoCarga "))
                joins.Append(" left join T_TIPO_DE_CARGA TipoCarga on TipoCarga.TCG_CODIGO = Pedido.TCG_CODIGO ");
        }

        private void SetarJoinsCanalEntrega(StringBuilder joins)
        {
            SetarJoinsPedido(joins);
            if (!joins.Contains(" CanalEntrega "))
                joins.Append(" left join T_CANAL_ENTREGA CanalEntrega on CanalEntrega.CNE_CODIGO = Pedido.CNE_CODIGO ");
        }

        private void SetarJoinsTipoOperacao(StringBuilder joins)
        {
            SetarJoinsPedido(joins);
            if (!joins.Contains(" TipoOperacao "))
                joins.Append(" left join T_TIPO_OPERACAO TipoOperacao on TipoOperacao.TOP_CODIGO = Pedido.TOP_CODIGO ");
        }

        private void SetarJoinsGrupoProduto(StringBuilder joins)
        {
            SetarJoinsPedido(joins);
            if (!joins.Contains(" GrupoProduto "))
                joins.Append(" left join T_GRUPO_PRODUTO GrupoProduto on GrupoProduto.GPR_CODIGO = ProdutoEmbarcador.GRP_CODIGO ");
        }

        private void SetarJoinsLinhaSeparacao(StringBuilder joins)
        {
            SetarJoinsPedidoProduto(joins);
            if (!joins.Contains(" LinhaSeparacao "))
                joins.Append(" left join T_LINHA_SEPARACAO LinhaSeparacao on LinhaSeparacao.CLS_CODIGO = PedidoProduto.CLS_CODIGO ");
        }

        private void SetarJoinsGrupoPessoa(StringBuilder joins)
        {
            SetarJoinsDestinatario(joins);
            if (!joins.Contains(" GrupoPessoa "))
                joins.Append(" left join T_GRUPO_PESSOAS GrupoPessoa on GrupoPessoa.GRP_CODIGO = Destinatario.GRP_CODIGO ");
        }


        #endregion

        #region Métodos Protegidos Sobrescritos

        protected override void SetarSelect(string propriedade, int codigoDinamico, StringBuilder select, StringBuilder joins, StringBuilder groupBy, bool somenteContarNumeroRegistros, Dominio.ObjetosDeValor.Embarcador.Produtos.FiltroPesquisaRelatorioProdutoEmbarcador filtroPesquisa)
        {
            switch (propriedade)
            {            
                case "Codigo":
                    if (!select.Contains(" Codigo, "))
                    {
                        select.Append("ProdutoEmbarcador.PRO_CODIGO as Codigo, ");
                        groupBy.Append("ProdutoEmbarcador.PRO_CODIGO, ");
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
                case "FilialIntegracao":
                    if (!select.Contains(" FilialIntegracao, "))
                    {
                        select.Append(@"SUBSTRING((SELECT ', ' + FilialIntegracao.FIL_CODIGO_INTEGRACAO
                                        FROM T_FILIAL_OUTROS_CODIGOS_INTEGRACAO FilialIntegracao
                                        WHERE FilialIntegracao.FIL_CODIGO = Filial.FIL_CODIGO FOR XML PATH('')), 3, 1000) FilialIntegracao, ");
                        groupBy.Append("Filial.FIL_CODIGO, ");

                        SetarJoinsFilial(joins);
                    }
                    break;
                case "UFOrigem":
                    if (!select.Contains(" UFOrigem, "))
                    {
                        select.Append(" Localidade.UF_SIGLA UFOrigem, ");
                        groupBy.Append(" Localidade.UF_SIGLA, ");

                        SetarJoinsLocalidade(joins);
                    }
                    break;
                case "PedidoEmbarcador":
                    if (!select.Contains(" PedidoEmbarcador, "))
                    {
                        select.Append("Pedido.PED_NUMERO_PEDIDO_EMBARCADOR PedidoEmbarcador, ");
                        groupBy.Append("Pedido.PED_NUMERO_PEDIDO_EMBARCADOR, ");

                        SetarJoinsPedido(joins);
                    }
                    break;
                case "DataHoraPedido":
                case "DataHoraPedidoFormatada":
                    if (!select.Contains(" DataHoraPedido, "))
                    {
                        select.Append("Pedido.PED_DATA_CRIACAO DataHoraPedido, ");
                        groupBy.Append("Pedido.PED_DATA_CRIACAO, ");

                        SetarJoinsPedido(joins);
                    }
                    break;
                case "RemetenteIntegracao":
                    if (!select.Contains(" RemetenteIntegracao, "))
                    {
                        select.Append("Remetente.CLI_CODIGO_INTEGRACAO RemetenteIntegracao, ");
                        groupBy.Append("Remetente.CLI_CODIGO_INTEGRACAO, ");

                        SetarJoinsRemetente(joins);
                    }
                    break;
                case "DestinatarioIntegracao":
                    if (!select.Contains(" DestinatarioIntegracao, "))
                    {
                        select.Append("Destinatario.CLI_CODIGO_INTEGRACAO DestinatarioIntegracao, ");
                        groupBy.Append("Destinatario.CLI_CODIGO_INTEGRACAO, ");

                        SetarJoinsDestinatario(joins);
                    }
                    break;
                case "TipoCarga":
                    if (!select.Contains(" TipoCarga, "))
                    {
                        select.Append("TipoCarga.TCG_DESCRICAO TipoCarga, ");
                        groupBy.Append("TipoCarga.TCG_DESCRICAO, ");

                        SetarJoinsTipoCarga(joins);
                    }
                    break;
                case "CanalEntrega":
                    if (!select.Contains(" CanalEntrega, "))
                    {
                        select.Append("CanalEntrega.CNE_DESCRICAO CanalEntrega, ");
                        groupBy.Append("CanalEntrega.CNE_DESCRICAO, ");

                        SetarJoinsCanalEntrega(joins);
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
                case "GrupoProduto":
                    if (!select.Contains(" GrupoProduto, "))
                    {
                        select.Append("GrupoProduto.GRP_DESCRICAO GrupoProduto, ");
                        groupBy.Append("GrupoProduto.GRP_DESCRICAO, ");

                        SetarJoinsGrupoProduto(joins);
                    }
                    break;
                case "ProdutoIntegracao":
                    if (!select.Contains(" ProdutoIntegracao, "))
                    {
                        select.Append("ProdutoEmbarcador.PRO_CODIGO_PRODUTO_EMBARCADOR ProdutoIntegracao, ");
                        groupBy.Append("ProdutoEmbarcador.PRO_CODIGO_PRODUTO_EMBARCADOR, ");
                    }
                    break;
                case "Produto":
                    if (!select.Contains(" Produto, "))
                    {
                        select.Append("ProdutoEmbarcador.GRP_DESCRICAO Produto, ");
                        groupBy.Append("ProdutoEmbarcador.GRP_DESCRICAO, ");
                    }
                    break;
                case "QuantidadeEmbalagem":
                    if (!select.Contains(" QuantidadeEmbalagem, "))
                    {
                        select.Append("PedidoProduto.PRP_QUANTIDADE_EMBALAGEM QuantidadeEmbalagem, ");
                        groupBy.Append("PedidoProduto.PRP_QUANTIDADE_EMBALAGEM, ");

                        SetarJoinsPedidoProduto(joins);
                    }
                    break;
                case "Quantidade":
                    if (!select.Contains(" Quantidade, "))
                    {
                        select.Append("PedidoProduto.PRP_QUANTIDADE Quantidade, ");
                        groupBy.Append("PedidoProduto.PRP_QUANTIDADE, ");

                        select.Append(@"(SELECT sum(cpp.CPP_QUANTIDADE) 
		  FROM T_CARREGAMENTO_PEDIDO crp inner join T_CARREGAMENTO_PEDIDO_PRODUTO cpp on cpp.CRP_CODIGO = crp.CRP_CODIGO 
									inner join T_CARREGAMENTO crg on crg.CRG_CODIGO = crp.CRG_CODIGO 
         WHERE crp.PED_CODIGO = Pedido.PED_CODIGO and cpp.PRP_CODIGO = PedidoProduto.PRP_CODIGO and crg.CRG_SITUACAO <> 4) QuantidadeCarregada, ");

                        SetarJoinsPedidoProduto(joins);
                        SetarSelect("MontagemCarregamentoPedidoProduto", codigoDinamico, select, joins, groupBy, somenteContarNumeroRegistros, filtroPesquisa);
                    }
                    break;
                case "QuantidadePallet":
                    if (!select.Contains(" QuantidadePallet, "))
                    {
                        select.Append("PedidoProduto.PRP_QUANTIDADE_PALET QuantidadePallet, ");
                        groupBy.Append("PedidoProduto.PRP_QUANTIDADE_PALET, ");

                        SetarJoinsPedidoProduto(joins);
                    }
                    break;
                case "PesoUnitario":
                    if (!select.Contains(" PesoUnitario, "))
                    {
                        select.Append("PedidoProduto.PRP_PESO_UNITARIO PesoUnitario, ");
                        groupBy.Append("PedidoProduto.PRP_PESO_UNITARIO, ");

                        SetarJoinsPedidoProduto(joins);
                    }
                    break;
                case "PalletFechado":
                    if (!select.Contains(" PalletFechado, "))
                    {
                        select.Append("(case when PedidoProduto.PRP_PALLET_FECHADO = 0 then 'Não' else 'Sim' end) PalletFechado, ");
                        groupBy.Append("PedidoProduto.PRP_PALLET_FECHADO, ");

                        SetarJoinsPedidoProduto(joins);
                    }
                    break;
                case "MetroCubico":
                    if (!select.Contains(" MetroCubico, "))
                    {
                        select.Append("PedidoProduto.PRP_METRO_CUBICO MetroCubico, ");
                        groupBy.Append("PedidoProduto.PRP_METRO_CUBICO, ");

                        SetarJoinsPedidoProduto(joins);
                    }
                    break;
                case "PesoTotalToneladas":
                    if (!select.Contains(" PesoTotalToneladas, "))
                    {
                        select.Append("((PedidoProduto.PRP_PESO_UNITARIO * PedidoProduto.PRP_QUANTIDADE) / 1000) PesoTotalToneladas, ");
                        groupBy.Append("PedidoProduto.PRP_PESO_UNITARIO, PRP_QUANTIDADE, ");

                        SetarJoinsPedidoProduto(joins);
                    }
                    break;
                case "PesoTotalQuilogramas":
                    if (!select.Contains(" PesoTotalQuilogramas, "))
                    {
                        select.Append("(PedidoProduto.PRP_PESO_UNITARIO * PedidoProduto.PRP_QUANTIDADE) PesoTotalQuilogramas, ");
                        groupBy.Append("PedidoProduto.PRP_PESO_UNITARIO, PedidoProduto.PRP_QUANTIDADE, ");

                        SetarJoinsPedidoProduto(joins);
                    }
                    break;
                case "StatusPedido":
                    if (!select.Contains(" StatusPedido, "))
                    {
                        //                  ////select.Append(@"(CASE 
                        //                  ////                WHEN
                        //                  ////                   EXISTS (SELECT 1 FROM T_PEDIDO_XML_NOTA_FISCAL PedidoNotaFiscal JOIN T_XML_NOTA_FISCAL NotaFiscal on PedidoNotaFiscal.NFX_CODIGO = NotaFiscal.NFX_CODIGO 
                        //                  ////                            JOIN T_CARGA_PEDIDO CargaPedido ON PedidoNotaFiscal.CPE_CODIGO = CargaPedido.CPE_CODIGO where CargaPedido.PED_CODIGO = CargaPedido.PED_CODIGO) 
                        //                  ////                THEN  
                        //                  ////                    'Faturado'
                        //                  ////                WHEN
                        //                  ////                    EXISTS(SELECT 1 FROM T_CARGA Carga JOIN T_CARGA_PEDIDO CargaPedido on CargaPedido.CAR_CODIGO = Carga.CAR_CODIGO where Pedido.PED_CODIGO = CargaPedido.PED_CODIGO)
                        //                  ////                THEN
                        //                  ////                    'Roteirizado'
                        //                  ////                WHEN
                        //                  ////                    EXISTS(SELECT 1 FROM T_CARREGAMENTO_PEDIDO CarregamentoPedido JOIN T_CARREGAMENTO Carregamento on Carregamento.CRG_CODIGO = CarregamentoPedido.CRG_CODIGO WHERE CarregamentoPedido.PED_CODIGO = Pedido.PED_CODIGO)
                        //                  ////                THEN
                        //                  ////                    'Em Roteirização'
                        //                  ////                ELSE
                        //                  ////                    case when Pedido.PED_SITUACAO = 1 then 'Em Aberto'
                        //                  ////             when Pedido.PED_SITUACAO = 2 then 'Cancelado'                                                     
                        //                  ////          when Pedido.PED_SITUACAO = 3 then 'Finalizado'
                        //                  ////          when Pedido.PED_SITUACAO = 4 then 'Ag. Aprovação'
                        //                  ////             when Pedido.PED_SITUACAO = 7 then 'Autorização Pendente'                                                     
                        //                  ////          when Pedido.PED_SITUACAO = 5 then 'Rejeitado'
                        //                  ////          else 'Outro' end
                        //                  ////             END
                        //                  ////            ) StatusPedido, ");

                        //                  ///*
                        //                  // * Em aberto	Quando a quantidade Total ou Saldo do ID Demanda não constam em carga
                        //                  // * Faturado	    Quando a quantidade Total ou Saldo do ID Demanda consta em carga e já possui NF Atrelada
                        //                  // * Em carga	    Quando a quantidade Total ou Saldo do ID Demanda consta em carga, mas ainda não possui NF atrelada
                        //                  // * Cancelado	Quando a quantidade Total ou Saldo do ID Demanda foi cancelado
                        //                  // * 
                        //                  // * Em carga.. alterado de EXISTS (SELECT 1 
                        //                  //                      FROM T_CARGA_PEDIDO CPE INNER JOIN T_CARGA_PEDIDO_PRODUTO CPP ON CPE.CPE_CODIGO = CPP.CPE_CODIGO 
                        //                  //                     WHERE CPE.PED_CODIGO = Pedido.PED_CODIGO AND CPP.PRO_CODIGO = PedidoProduto.PRO_CODIGO ) THEN 



                        //                  //        EXISTS (SELECT 1 
                        //                  //                      FROM T_PEDIDO_XML_NOTA_FISCAL PedidoNotaFiscal JOIN T_XML_NOTA_FISCAL NotaFiscal on PedidoNotaFiscal.NFX_CODIGO = NotaFiscal.NFX_CODIGO 
                        //										        //                                    JOIN T_CARGA_PEDIDO CargaPedido ON PedidoNotaFiscal.CPE_CODIGO = CargaPedido.CPE_CODIGO 
                        //                  //                     WHERE Pedido.PED_CODIGO = CargaPedido.PED_CODIGO
                        //                  //                       AND PedidoProduto.PRP_QUANTIDADE <= (SELECT SUM(CPP.CPP_QUANTIDADE)
                        //									         //                                FROM T_CARGA_PEDIDO CPE INNER JOIN T_CARGA_PEDIDO_PRODUTO CPP ON CPP.CPE_CODIGO = CPE.CPE_CODIGO
                        //									         //                               WHERE CPE.PED_CODIGO = CargaPedido.PED_CODIGO) 
                        //                  //                            ) THEN 'Faturado Total'
                        //                  //                      WHEN EXISTS (SELECT 1 
                        //                  //                      FROM T_PEDIDO_XML_NOTA_FISCAL PedidoNotaFiscal JOIN T_XML_NOTA_FISCAL NotaFiscal on PedidoNotaFiscal.NFX_CODIGO = NotaFiscal.NFX_CODIGO 
                        //										        //                                    JOIN T_CARGA_PEDIDO CargaPedido ON PedidoNotaFiscal.CPE_CODIGO = CargaPedido.CPE_CODIGO 
                        //                  //                     WHERE Pedido.PED_CODIGO = CargaPedido.PED_CODIGO) THEN 'Faturado Parcial'
                        //                  // * 
                        //                  //*/

                        //                  //select.Append(@"(CASE WHEN PedidoProduto.PRP_QUANTIDADE = 0 THEN 'Cancelado'
                        //                  //                      WHEN Carga.CAR_SITUACAO > 5 THEN 'Faturado'
                        //                  //                      WHEN Carga.CAR_CODIGO_CARGA_EMBARCADOR IS NOT NULL THEN 'Em Carga'
                        //               //                      WHEN EXISTS (SELECT 1 
                        //   //                                     FROM T_CARREGAMENTO_PEDIDO CAP INNER JOIN T_CARREGAMENTO_PEDIDO_PRODUTO CPP ON CAP.CRP_CODIGO = CPP.CRP_CODIGO
                        //   //                                    WHERE CAP.PED_CODIGO = Pedido.PED_CODIGO AND CPP.PRP_CODIGO = PedidoProduto.PRP_CODIGO ) THEN 'Em Carregamento'
                        ////					  WHEN EXISTS (SELECT 1 
                        //   //                                     FROM T_SESSAO_ROTEIRIZADOR_PEDIDO SRP 
                        //   //                                    WHERE SRP.PED_CODIGO = Pedido.PED_CODIGO AND SRP.SRP_SITUACAO <> 4) THEN 'Em Roteirização'
                        //               //                      ELSE
                        //                  //                                                            case when Pedido.PED_SITUACAO = 1 then 'Em Aberto'
                        //      //                                                                 when Pedido.PED_SITUACAO = 2 then 'Cancelado'                                                     
                        //   //                                                                 when Pedido.PED_SITUACAO = 3 then 'Finalizado'
                        //   //                                                                 when Pedido.PED_SITUACAO = 4 then 'Ag. Aprovação'
                        //      //                                                                 when Pedido.PED_SITUACAO = 7 then 'Autorização Pendente'                                                     
                        //   //                                                                 when Pedido.PED_SITUACAO = 5 then 'Rejeitado'
                        //   //                                                                 else 'Outro' end
                        //                  //                 END
                        //                  //                                                    ) StatusPedido, ");

                        //           select.Append(@"(CASE WHEN PedidoProduto.PRP_QUANTIDADE = 0 THEN 'Cancelado'
                        //                                 WHEN Carga.CAR_SITUACAO > 5 THEN 'Faturado'
                        //                                 WHEN Carga.CAR_CODIGO_CARGA_EMBARCADOR IS NOT NULL THEN 'Em Carga'
                        //                              WHEN EXISTS (SELECT 1 
                        //                                 FROM T_CARREGAMENTO_PEDIDO CAP INNER JOIN T_CARREGAMENTO_PEDIDO_PRODUTO CPP ON CAP.CRP_CODIGO = CPP.CRP_CODIGO
                        //                                WHERE CAP.PED_CODIGO = Pedido.PED_CODIGO AND CPP.PRP_CODIGO = PedidoProduto.PRP_CODIGO ) THEN 'Em Carregamento'
                        //WHEN EXISTS (SELECT 1 
                        //                                 FROM T_SESSAO_ROTEIRIZADOR_PEDIDO SRP INNER JOIN T_SESSAO_ROTEIRIZADOR_PEDIDO_PRODUTO SPP ON SRP.SRP_CODIGO = SPP.SRP_CODIGO
                        //                                               WHERE SRP.PED_CODIGO = Pedido.PED_CODIGO AND SRP.SRP_SITUACAO <> 4 AND SPP.PRP_CODIGO = PedidoProduto.PRP_CODIGO) THEN 'Em Roteirização'
                        //                              ELSE
                        //                                                                       case when Pedido.PED_SITUACAO = 1 then 'Em Aberto'
                        //                                                                when Pedido.PED_SITUACAO = 2 then 'Cancelado'                                                     
                        //                                                             when Pedido.PED_SITUACAO = 3 then 'Finalizado'
                        //                                                             when Pedido.PED_SITUACAO = 4 then 'Ag. Aprovação'
                        //                                                                when Pedido.PED_SITUACAO = 7 then 'Autorização Pendente'                                                     
                        //                                                             when Pedido.PED_SITUACAO = 5 then 'Rejeitado'
                        //                                                             else 'Outro' end
                        //                            END
                        //                                                               ) StatusPedido, ");

                        //           groupBy.Append("Pedido.PED_CODIGO, ");
                        //           groupBy.Append("Pedido.PED_SITUACAO, ");
                        //           groupBy.Append("PedidoProduto.PRO_CODIGO, ");
                        //           groupBy.Append("Carga.CAR_SITUACAO, ");
                        //           groupBy.Append("PedidoProduto.PRP_CODIGO, ");
                        //           groupBy.Append("PedidoProduto.PRP_QUANTIDADE, ");
                        //           groupBy.Append("Carga.CAR_CODIGO_CARGA_EMBARCADOR, ");

                        //           SetarJoinsPedido(joins);

                        //           if (!joins.Contains(" Carga "))
                        //           {
                        //               SetarJoinsCargaPedido(joins);                            

                        //               joins.Append(" left join T_CARGA Carga on Carga.CAR_CODIGO = CargaPedido.CAR_CODIGO AND Carga.CAR_SITUACAO <> 13 ");
                        //           }


                        select.Append(@"(CASE 
            WHEN PedidoProduto.PRP_QUANTIDADE = 0 THEN 'Cancelado'
			WHEN (select COALESCE(MIN( Carga.CAR_SITUACAO), 0) FROM T_CARGA Carga INNER JOIN T_CARGA_PEDIDO CargaPedido on CargaPedido.CAR_CODIGO = Carga.CAR_CODIGO AND CargaPedido.PED_CODIGO = Pedido.PED_CODIGO
			             INNER JOIN T_CARGA_PEDIDO_PRODUTO CargaPedidoProduto on CargaPedidoProduto.CPE_CODIGO = CargaPedido.CPE_CODIGO
				   where Carga.CAR_CODIGO = CargaPedido.CAR_CODIGO AND Carga.CAR_SITUACAO <> 13 AND CargaPedidoProduto.PRO_CODIGO = PedidoProduto.PRO_CODIGO) > 5 THEN 'Faturado'
            WHEN (select MAX(Carga.CAR_CODIGO_CARGA_EMBARCADOR) FROM T_CARGA Carga INNER JOIN T_CARGA_PEDIDO CargaPedido on CargaPedido.CAR_CODIGO = Carga.CAR_CODIGO AND CargaPedido.PED_CODIGO = Pedido.PED_CODIGO
			             INNER JOIN T_CARGA_PEDIDO_PRODUTO CargaPedidoProduto on CargaPedidoProduto.CPE_CODIGO = CargaPedido.CPE_CODIGO
				   where Carga.CAR_CODIGO = CargaPedido.CAR_CODIGO AND Carga.CAR_SITUACAO <> 13 AND CargaPedidoProduto.PRO_CODIGO = PedidoProduto.PRO_CODIGO) IS NOT NULL THEN 'Em Carga'
			WHEN EXISTS (SELECT
                1                                                 
            FROM
                T_CARREGAMENTO_PEDIDO CAP 
            INNER JOIN
                T_CARREGAMENTO_PEDIDO_PRODUTO CPP 
                    ON CAP.CRP_CODIGO = CPP.CRP_CODIGO                                               
            WHERE
                CAP.PED_CODIGO = Pedido.PED_CODIGO 
                AND CPP.PRP_CODIGO = PedidoProduto.PRP_CODIGO ) THEN 'Em Carregamento'               
            WHEN EXISTS (SELECT
                1                                                 
            FROM
                T_SESSAO_ROTEIRIZADOR_PEDIDO SRP 
            INNER JOIN
                T_SESSAO_ROTEIRIZADOR_PEDIDO_PRODUTO SPP 
                    ON SRP.SRP_CODIGO = SPP.SRP_CODIGO                                                              
            WHERE
                SRP.PED_CODIGO = Pedido.PED_CODIGO 
                AND SRP.SRP_SITUACAO <> 4 
                AND SPP.PRP_CODIGO = PedidoProduto.PRP_CODIGO) THEN 'Em Roteirização'                                             
            ELSE                                                                                      case 
                when Pedido.PED_SITUACAO = 1 then 'Em Aberto'                                                                               
                when Pedido.PED_SITUACAO = 2 then 'Cancelado'                                                                                                                                 
                when Pedido.PED_SITUACAO = 3 then 'Finalizado'                                                                            
                when Pedido.PED_SITUACAO = 4 then 'Ag. Aprovação'                                                                               
                when Pedido.PED_SITUACAO = 7 then 'Autorização Pendente'                                                                                                                                 
                when Pedido.PED_SITUACAO = 5 then 'Rejeitado'                                                                            
                else 'Outro' 
            end                                           
        END                                                                              ) StatusPedido, ");

                        groupBy.Append("Pedido.PED_CODIGO, ");
                        groupBy.Append("Pedido.PED_SITUACAO, ");
                        groupBy.Append("PedidoProduto.PRO_CODIGO, ");
                        groupBy.Append("PedidoProduto.PRP_CODIGO, ");
                        groupBy.Append("PedidoProduto.PRP_QUANTIDADE, ");

                        SetarJoinsPedido(joins);

                    }
                    break;
                case "NumeroCarga":
                    if (!select.Contains(" NumeroCarga, "))
                    {
                        ////select.Append("substring((SELECT DISTINCT ', ' + Carga.CAR_CODIGO_CARGA_EMBARCADOR FROM T_CARGA Carga INNER JOIN T_CARGA_PEDIDO CargaPedido on CargaPedido.CAR_CODIGO = Carga.CAR_CODIGO INNER JOIN T_CARGA_PEDIDO_PRODUTO CargaPedidoProduto on CargaPedidoProduto.CPE_CODIGO = CargaPedido.CPE_CODIGO where Pedido.PED_CODIGO = CargaPedido.PED_CODIGO and CargaPedidoProduto.PRO_CODIGO = PedidoProduto.PRO_CODIGO for xml path('')), 3, 200) NumeroCarga, ");
                        ////groupBy.Append("Pedido.PED_CODIGO, ");

                        ////SetarJoinsPedido(joins);

                        //select.Append("Carga.CAR_CODIGO_CARGA_EMBARCADOR as NumeroCarga, ");
                        //groupBy.Append("Carga.CAR_CODIGO_CARGA_EMBARCADOR, ");

                        //SetarJoinsCargaPedido(joins);

                        //if (!joins.Contains(" CargaPedidoProduto "))
                        //    joins.Append(" left join T_CARGA_PEDIDO_PRODUTO CargaPedidoProduto on CargaPedidoProduto.CPE_CODIGO = CargaPedido.CPE_CODIGO and ProdutoEmbarcador.PRO_CODIGO = CargaPedidoProduto.PRO_CODIGO ");

                        //if (!joins.Contains(" Carga "))
                        //    joins.Append(" left join T_CARGA Carga on Carga.CAR_CODIGO = CargaPedido.CAR_CODIGO AND Carga.CAR_SITUACAO <> 13 AND CargaPedidoProduto.CPE_CODIGO = CargaPedido.CPE_CODIGO ");

                        select.Append(@"
		substring((SELECT
            DISTINCT ', ' + CAST(Carga.CAR_CODIGO_CARGA_EMBARCADOR as varchar) 
        FROM
            T_CARGA Carga INNER JOIN T_CARGA_PEDIDO CargaPedido on CargaPedido.CAR_CODIGO = Carga.CAR_CODIGO AND CargaPedido.PED_CODIGO = Pedido.PED_CODIGO
			              INNER JOIN T_CARGA_PEDIDO_PRODUTO CargaPedidoProduto on CargaPedidoProduto.CPE_CODIGO = CargaPedido.CPE_CODIGO
        where
            Carga.CAR_CODIGO = CargaPedido.CAR_CODIGO AND Carga.CAR_SITUACAO <> 13 AND CargaPedidoProduto.PRO_CODIGO = PedidoProduto.PRO_CODIGO for xml path('')),
        3,
        200) NumeroCarga, ");

                    }
                    break;
                case "NotasFiscais":
                    if (!select.Contains(" NotasFiscais, "))
                    {
                        select.Append("substring((SELECT DISTINCT ', ' + CAST(NotaFiscal.NF_NUMERO as varchar) FROM T_PEDIDO_XML_NOTA_FISCAL PedidoNotaFiscal JOIN T_XML_NOTA_FISCAL NotaFiscal on PedidoNotaFiscal.NFX_CODIGO = NotaFiscal.NFX_CODIGO JOIN T_CARGA_PEDIDO CargaPedido ON PedidoNotaFiscal.CPE_CODIGO = CargaPedido.CPE_CODIGO where CargaPedido.PED_CODIGO = Pedido.PED_CODIGO for xml path('')), 3, 200) NotasFiscais, ");
                        groupBy.Append("Pedido.PED_CODIGO, ");

                        SetarJoinsPedido(joins);
                    }
                    break;
                case "LinhaSeparacao":
                    if (!select.Contains(" LinhaSeparacao, "))
                    {
                        select.Append("LinhaSeparacao.CLS_DESCRICAO LinhaSeparacao, ");
                        groupBy.Append("LinhaSeparacao.CLS_DESCRICAO, ");

                        SetarJoinsLinhaSeparacao(joins);
                    }
                    break;
                case "SessaoRoteirizador":
                    if (!select.Contains(" SessaoRoteirizador, "))
                    {
                        // AND NOT EXISTS(SELECT 1 FROM T_CARREGAMENTO_PEDIDO CRP INNER JOIN T_CARREGAMENTO CRG ON CRG.CRG_CODIGO = CRP.CRG_CODIGO JOIN T_CARREGAMENTO_PEDIDO_PRODUTO CPP ON CPP.CRP_CODIGO = CRP.CRP_CODIGO WHERE CRP.PED_CODIGO = SRP.PED_CODIGO AND CRG.SRO_CODIGO<> SRP.SRO_CODIGO)
                        select.Append(@"
substring((
SELECT DISTINCT ', ' + res.SRO_CODIGO
  FROM (
select CAST(SessaoRoteirizador.SRO_CODIGO as varchar) SRO_CODIGO FROM T_CARREGAMENTO_PEDIDO_PRODUTO CarregamentoPedidoProduto 
                           JOIN T_CARREGAMENTO_PEDIDO CarregamentoPedido ON CarregamentoPedido.CRP_CODIGO = CarregamentoPedidoProduto.CRP_CODIGO 
                           JOIN T_CARREGAMENTO Carregamento on Carregamento.CRG_CODIGO = CarregamentoPedido.CRG_CODIGO 
                           JOIN T_SESSAO_ROTEIRIZADOR SessaoRoteirizador on SessaoRoteirizador.SRO_CODIGO = Carregamento.SRO_CODIGO
                           WHERE CarregamentoPedidoProduto.PRP_CODIGO = PedidoProduto.PRP_CODIGO 
						   union 
SELECT CAST(SRP.SRO_CODIGO as varchar) SRO_CODIGO FROM T_SESSAO_ROTEIRIZADOR_PEDIDO SRP INNER JOIN T_SESSAO_ROTEIRIZADOR_PEDIDO_PRODUTO SPP ON SRP.SRP_CODIGO = SPP.SRP_CODIGO
                           WHERE SRP.PED_CODIGO = Pedido.PED_CODIGO AND SRP.SRP_SITUACAO <> 4 AND SPP.PRP_CODIGO = PedidoProduto.PRP_CODIGO
						   ) res
						   for xml path('')), 3, 200) SessaoRoteirizador, ");
                        groupBy.Append("PedidoProduto.PRP_CODIGO, ");

                        SetarJoinsPedidoProduto(joins);
                    }
                    break;
                case "GrupoPessoa":
                    if (!select.Contains(" GrupoPessoa, "))
                    {
                        select.Append("GrupoPessoa.GRP_DESCRICAO GrupoPessoa, ");
                        groupBy.Append("GrupoPessoa.GRP_DESCRICAO, ");

                        SetarJoinsGrupoPessoa(joins);
                    }
                    break;
                case "QtdCaixasPorPalete":
                    if (!select.Contains(" QtdCaixasPorPalete, "))
                    {
                        select.Append("ProdutoEmbarcador.PRO_QUANTIDADE_CAIXA_POR_PALLET QtdCaixasPorPalete, ");
                        groupBy.Append("ProdutoEmbarcador.PRO_QUANTIDADE_CAIXA_POR_PALLET, ");
                    }
                    break;
                case "IDDemanda":
                    if (!select.Contains(" IDDemanda, "))
                    {
                        select.Append("PedidoProduto.PRP_ID_DEMANDA IDDemanda, ");
                        groupBy.Append("PedidoProduto.PRP_ID_DEMANDA, ");
                    }
                    break;
                case "EnderecoProduto":
                    if (!select.Contains(" EnderecoProduto, "))
                    {
                        select.Append("EnderecoProduto.CEP_DESCRICAO EnderecoProduto, ");
                        groupBy.Append("EnderecoProduto.CEP_DESCRICAO, ");

                        SetarJoinsEnderecoProduto(joins);
                    }
                    break;
                case "MontagemCarregamentoPedidoProduto":
                    if (!select.Contains(" MontagemCarregamentoPedidoProduto, "))
                    {
                        select.Append(@" (SELECT TOP 1
                                                Centro.CEC_MONTAGEM_CARREGAMENTO_PEDIDO_PRODUTO
                                            FROM
                                                T_CARGA Carga INNER JOIN T_CARGA_PEDIDO CargaPedido on CargaPedido.CAR_CODIGO = Carga.CAR_CODIGO AND CargaPedido.PED_CODIGO = Pedido.PED_CODIGO
	                                                          INNER JOIN T_CARGA_PEDIDO_PRODUTO CargaPedidoProduto on CargaPedidoProduto.CPE_CODIGO = CargaPedido.CPE_CODIGO
				                                              left join T_CENTRO_CARREGAMENTO Centro on Carga.FIL_CODIGO = Centro.FIL_CODIGO
				                                              join T_CENTRO_CARREGAMENTO_TIPO_CARGA CentroTipo on Centro.CEC_CODIGO = CentroTipo.CEC_CODIGO
                                            where
                                                Carga.CAR_CODIGO = CargaPedido.CAR_CODIGO AND Carga.CAR_SITUACAO <> 13 AND CargaPedidoProduto.PRO_CODIGO = PedidoProduto.PRO_CODIGO and CentroTipo.TCG_CODIGO = Carga.TCG_CODIGO) MontagemCarregamentoPedidoProduto,");
                    }
                    break;

                case "Placa":
                    if (!select.Contains(" Placa, "))
                    {
                        select.Append(@"(SELECT TOP 1
                                                Veiculo.VEI_PLACA
                                             FROM T_CARGA_PEDIDO CargaPedido 
                                                join T_CARGA Carga on Carga.CAR_CODIGO = CargaPedido.CAR_CODIGO 
                                                join T_VEICULO Veiculo on Veiculo.VEI_CODIGO = Carga.CAR_VEICULO 
                                             WHERE
                                                 CargaPedido.PED_CODIGO = PedidoProduto.PED_CODIGO 
                                                 AND PedidoProduto.PRO_CODIGO = ProdutoEmbarcador.PRO_CODIGO
                                                 AND Carga.CAR_CODIGO = CargaPedido.CAR_CODIGO 
                                                 AND Carga.CAR_SITUACAO <> 13 
                                         ) Placa, ");
                        groupBy.Append("PedidoProduto.PED_CODIGO, ");

                        SetarJoinsPedido(joins);
                    }
                    break;
            }
        }

        protected override void SetarWhere(Dominio.ObjetosDeValor.Embarcador.Produtos.FiltroPesquisaRelatorioProdutoEmbarcador filtrosPesquisa, StringBuilder where, StringBuilder joins, StringBuilder groupBy, List<Embarcador.Consulta.ParametroSQL> parametros = null)
        {
            string pattern = "yyyy-MM-dd";

            if (filtrosPesquisa.CodigosCanaisEntrega.Count > 0)
            {
                SetarJoinsPedido(joins);
                where.Append($" and Pedido.CNE_CODIGO in ({string.Join(", ", filtrosPesquisa.CodigosCanaisEntrega)})");
            }

            if (filtrosPesquisa.CodigosDestinatario.Count > 0)
            {
                SetarJoinsPedido(joins);
                where.Append($" and Pedido.CLI_CODIGO in ({string.Join(", ", filtrosPesquisa.CodigosDestinatario)})");
            }

            if (filtrosPesquisa.CodigosFiliais.Any(codigo => codigo == -1))
            {
                where.Append($@" and (Carga.FIL_CODIGO in ({string.Join(", ", filtrosPesquisa.CodigosFiliais)}) OR EXISTS (   SELECT _cargaPedidoRecebedor.CAR_CODIGO 
                                                                                                                       FROM T_CARGA_PEDIDO _cargaPedidoRecebedor 
                                                                                                                       LEFT JOIN T_PEDIDO _pedido ON _pedido.PED_CODIGO = _cargaPedidoRecebedor.PED_CODIGO
                                                                                                                       WHERE Carga.CAR_CODIGO = _cargaPedidoRecebedor.CAR_CODIGO
                                                                                                                       AND _pedido.CLI_CODIGO_RECEBEDOR IN ({string.Join(",", filtrosPesquisa.CodigosRecebedores)})))");
            }

            if (filtrosPesquisa.CodigosFiliais?.Count > 0)
            {
                SetarJoinsPedido(joins);
                where.Append($" and Pedido.FIL_CODIGO in ({string.Join(", ", filtrosPesquisa.CodigosFiliais)})");
            }

            if (filtrosPesquisa.CodigosTiposCarga.Count > 0)
            {
                SetarJoinsPedido(joins);
                where.Append($" and Pedido.ATC_CODIGO in ({string.Join(", ", filtrosPesquisa.CodigosTiposCarga)})");
            }

            if (filtrosPesquisa.CodigosGrupoProduto.Count > 0)
            {
                where.Append($" and ProdutoEmbarcador.GRP_CODIGO in ({string.Join(", ", filtrosPesquisa.CodigosGrupoProduto)})");
            }

            if (filtrosPesquisa.PedidosEmbarcador.Count > 0)
            {
                SetarJoinsPedido(joins);
                where.Append($" and Pedido.PED_CODIGO in ({string.Join(", ", filtrosPesquisa.PedidosEmbarcador)})");
            }

            if (filtrosPesquisa.DataPedidoInicial.HasValue)
            {
                SetarJoinsPedido(joins);
                where.Append($" and Pedido.PED_DATA_CRIACAO >= '{filtrosPesquisa.DataPedidoInicial?.ToString(pattern)}'");
            }

            if (filtrosPesquisa.DataPedidoFinal.HasValue)
            {
                SetarJoinsPedido(joins);
                where.Append($" and Pedido.PED_DATA_CRIACAO < '{filtrosPesquisa.DataPedidoFinal?.AddDays(1).ToString(pattern)}'");
            }

            if (filtrosPesquisa.StatusPedido != null)
            {
                SetarJoinsPedido(joins);
                foreach (var status in filtrosPesquisa.StatusPedido)
                {
                    switch (status)
                    {
                        case Dominio.ObjetosDeValor.Embarcador.Enumeradores.StatusPedidoEmbarcadorAssai.Liberado:
                            where.Append($" and Pedido.PED_CODIGO not in (SELECT CargaPedido.PED_CODIGO FROM T_CARGA Carga JOIN T_CARGA_PEDIDO CargaPedido on CargaPedido.CAR_CODIGO = Carga.CAR_CODIGO)");
                            where.Append($" and Pedido.PED_CODIGO not in (SELECT CarregamentoPedido.PED_CODIGO FROM T_CARREGAMENTO_PEDIDO CarregamentoPedido JOIN T_CARREGAMENTO Carregamento on Carregamento.CRG_CODIGO = CarregamentoPedido.CRG_CODIGO)");
                            break;
                        case Dominio.ObjetosDeValor.Embarcador.Enumeradores.StatusPedidoEmbarcadorAssai.Faturado:
                            where.Append($" and Pedido.PED_CODIGO in (SELECT CargaPedido.PED_CODIGO FROM T_CARGA Carga JOIN T_CARGA_PEDIDO CargaPedido on CargaPedido.CAR_CODIGO = Carga.CAR_CODIGO)");
                            where.Append($" and Pedido.PED_CODIGO not in (SELECT CarregamentoPedido.PED_CODIGO FROM T_CARREGAMENTO_PEDIDO CarregamentoPedido JOIN T_CARREGAMENTO Carregamento on Carregamento.CRG_CODIGO = CarregamentoPedido.CRG_CODIGO)");
                            break;
                        case Dominio.ObjetosDeValor.Embarcador.Enumeradores.StatusPedidoEmbarcadorAssai.EmCarregamento:
                            where.Append($" and Pedido.PED_CODIGO in (SELECT CarregamentoPedido.PED_CODIGO FROM T_CARREGAMENTO_PEDIDO CarregamentoPedido JOIN T_CARREGAMENTO Carregamento on Carregamento.CRG_CODIGO = CarregamentoPedido.CRG_CODIGO)");
                            where.Append($" and Pedido.PED_CODIGO not in (SELECT CargaPedido.PED_CODIGO FROM T_CARGA Carga JOIN T_CARGA_PEDIDO CargaPedido on CargaPedido.CAR_CODIGO = Carga.CAR_CODIGO)");
                            break;
                        case Dominio.ObjetosDeValor.Embarcador.Enumeradores.StatusPedidoEmbarcadorAssai.Cancelado:
                            where.Append($" and Pedido.PED_SITUACAO = 2");
                            break;
                    }
                }
            }

            if (filtrosPesquisa.TipoOperacao.Count > 0)
            {
                SetarJoinsPedido(joins);
                where.Append($" and Pedido.TOP_CODIGO in ({string.Join(", ", filtrosPesquisa.TipoOperacao)})");
            }

            if (filtrosPesquisa.CodigosGrupoPessoa.Count > 0)
            {
                where.Append($" and ProdutoEmbarcador.GRP_CODIGO_PESSOA in ({string.Join(", ", filtrosPesquisa.CodigosGrupoPessoa)})");
            }

            if (filtrosPesquisa.CodigosProduto.Count > 0)
            {
                where.Append($" and ProdutoEmbarcador.PRO_CODIGO in ({string.Join(", ", filtrosPesquisa.CodigosProduto)})");
            }

        }

        #endregion
    }
}
