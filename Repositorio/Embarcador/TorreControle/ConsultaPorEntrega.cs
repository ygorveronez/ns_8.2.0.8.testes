using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Dynamic.Core;
using System.Text;

namespace Repositorio.Embarcador.TorreControle
{
    public class ConsultaPorEntrega : RepositorioBase<Dominio.Entidades.Embarcador.Cargas.ControleEntrega.CargaEntrega>
    {
        public ConsultaPorEntrega(UnitOfWork unitOfWork) : base(unitOfWork) { }

        #region Métodos Públicos

        public IList<Dominio.ObjetosDeValor.Embarcador.TorreControle.ConsultaPorEntrega> Consultar(Dominio.ObjetosDeValor.Embarcador.TorreControle.FiltroPesquisaConsultaPorEntrega filtrosPesquisa, Dominio.ObjetosDeValor.Embarcador.Consulta.ParametroConsulta parametroConsulta)
        {
            var consulta = Consultar(filtrosPesquisa, parametroConsulta, true);

            return consulta;
        }

        public int ContarConsulta(Dominio.ObjetosDeValor.Embarcador.TorreControle.FiltroPesquisaConsultaPorEntrega filtrosPesquisa, Dominio.ObjetosDeValor.Embarcador.Consulta.ParametroConsulta parametroConsulta)
        {
            var result = Consultar(filtrosPesquisa, parametroConsulta, false);

            return result.Count();
        }


        public IList<Dominio.ObjetosDeValor.Embarcador.TorreControle.ConsultaEntregaAtrasada> ConsultarEntregaAtrasada(Dominio.ObjetosDeValor.Embarcador.TorreControle.FiltroPesquisaConsultaEntregaAtrasada filtrosPesquisa, Dominio.ObjetosDeValor.Embarcador.Consulta.ParametroConsulta parametroConsulta)
        {
            StringBuilder queryString = new StringBuilder(ObterCamposConsultaEntregaAtrasada())
                                            .Append(ObterFromConsultaEntregaAtrasada())
                                            .Append(ObterWhereConsultaEntregaAtrasada(filtrosPesquisa))
                                            .Append(ObterGroupByConsultaEntregaAtrasada())
                                            .Append($" ORDER BY {parametroConsulta.PropriedadeOrdenar} {parametroConsulta.DirecaoOrdenar}");

            queryString.Append($" OFFSET {parametroConsulta.InicioRegistros} ROWS");

            if (parametroConsulta.LimiteRegistros > 0)
                queryString.Append($" FETCH NEXT {parametroConsulta.LimiteRegistros} ROWS ONLY");

            var consulta = this.SessionNHiBernate.CreateSQLQuery(queryString.ToString())
                                                 .SetResultTransformer(new NHibernate.Transform.AliasToBeanResultTransformer(typeof(Dominio.ObjetosDeValor.Embarcador.TorreControle.ConsultaEntregaAtrasada)));

            return consulta.SetTimeout(600).List<Dominio.ObjetosDeValor.Embarcador.TorreControle.ConsultaEntregaAtrasada>();
        }

        public int ContarConsultaEntregaAtrasada(Dominio.ObjetosDeValor.Embarcador.TorreControle.FiltroPesquisaConsultaEntregaAtrasada filtrosPesquisa)
        {
            StringBuilder queryString = new StringBuilder("SELECT DISTINCT(COUNT(0) OVER ())   ")
                                      .Append(ObterFromConsultaEntregaAtrasada())
                                      .Append(ObterWhereConsultaEntregaAtrasada(filtrosPesquisa))
                                      .Append(ObterGroupByConsultaEntregaAtrasada());

            var consulta = this.SessionNHiBernate.CreateSQLQuery(queryString.ToString());

            return consulta.SetTimeout(600).UniqueResult<int>();
        }


        #endregion

        #region Métodos Privados

        private IList<Dominio.ObjetosDeValor.Embarcador.TorreControle.ConsultaPorEntrega> Consultar(Dominio.ObjetosDeValor.Embarcador.TorreControle.FiltroPesquisaConsultaPorEntrega filtrosPesquisa, Dominio.ObjetosDeValor.Embarcador.Consulta.ParametroConsulta parametroConsulta, bool paginar = true)
        {
            string pattern = "yyyy-MM-dd";

            var query = @"SELECT 
                            CAR_CODIGO_CARGA_EMBARCADOR Carga,
                            PEDIDOS Pedidos,
                            NOTAS Notas,
                            STATUS Status,
                            OCORRENCIA Ocorrencia,
                            CIDADE_ORIGEM CidadeOrigem,
                            CLI_NOME_ENTREGA Cliente,
                            CONCAT(LOC_DESCRICAO_CLIENTE_ENTREGA, '-', UF_SIGLA_CLIENTE_ENTREGA) CidadeDestino,
                            CAR_DATA_CRIACAO DataCriacaoCarga,
                            CAR_DATA_CARREGAMENTO DataCarregamento,
                            CEN_DATA_ENTREGA_PREVISTA DataPrevisaoEntrega,
                            CEN_DATA_ENTREGA_REPROGRAMADA DataEntregaReprogramada,
                            TOP_DESCRICAO Operacao,
                            VEI_PLACA Veiculo,
                            FUN_NOME Motorista,
                            EMP_FANTASIA Transportador 
                            FROM V_TORRE_CONTROLE_CARGAS V ";

            if (!string.IsNullOrWhiteSpace(filtrosPesquisa.NumeroNota))
            {
                query += @"JOIN T_CARGA_ENTREGA_PEDIDO CEP ON V.CEN_CODIGO = CEP.CEN_CODIGO
                    JOIN T_PEDIDO_XML_NOTA_FISCAL XN ON CEP.CPE_CODIGO = XN.CPE_CODIGO
                    JOIN T_XML_NOTA_FISCAL N ON XN.NFX_CODIGO = N.NFX_CODIGO";
                query += " WHERE 1 = 1";
                query += $" AND N.NF_NUMERO = '{filtrosPesquisa.NumeroNota.ToString()}'";
            }
            else
            {
                query += " WHERE 1 = 1";
            }

            if (!string.IsNullOrWhiteSpace(filtrosPesquisa.NumeroCarga))
                query += $" AND CAR_CODIGO_CARGA_EMBARCADOR = '{filtrosPesquisa.NumeroCarga.ToString()}'";


            if (!string.IsNullOrWhiteSpace(filtrosPesquisa.Status))
                query += $" AND STATUS = '{filtrosPesquisa.Status}'";

            if (!string.IsNullOrWhiteSpace(filtrosPesquisa.Placa))
                query += $" AND VEI_PLACA = '{filtrosPesquisa.Placa}'";

            string patternHour = "yyyy-MM-dd HH:mm";
            if (filtrosPesquisa.DataCriacaoCargaInicial != DateTime.MinValue)
                query += $" AND CAR_DATA_CRIACAO >= '{filtrosPesquisa.DataCriacaoCargaInicial.ToString(patternHour)}'";

            if (filtrosPesquisa.DataCriacaoCargaFinal != DateTime.MinValue)
                query += $" AND CAR_DATA_CRIACAO <= '{filtrosPesquisa.DataCriacaoCargaFinal.ToString(patternHour)}'";

            if (filtrosPesquisa.DataPrevisaoEntregaInicial != DateTime.MinValue)
                query += $" AND CEN_DATA_ENTREGA_PREVISTA >= '{filtrosPesquisa.DataPrevisaoEntregaInicial.ToString(patternHour)}'";

            if (filtrosPesquisa.DataPrevisaoEntregaFinal != DateTime.MinValue)
                query += $" AND CEN_DATA_ENTREGA_PREVISTA <= '{filtrosPesquisa.DataPrevisaoEntregaFinal.ToString(patternHour)}'";

            if (filtrosPesquisa.Operacao > 0)
                query += $" AND TOP_CODIGO = {filtrosPesquisa.Operacao.ToString()}";

            if (filtrosPesquisa.Transportador > 0)
                query += $" AND EMP_CODIGO = {filtrosPesquisa.Transportador.ToString()}";

            if (!string.IsNullOrWhiteSpace(filtrosPesquisa.Status))
                query += $" AND STATUS = '{filtrosPesquisa.Status}'";

            var agrup = false;
            if (!string.IsNullOrWhiteSpace(parametroConsulta.PropriedadeAgrupar))
            {
                agrup = true;
                query += " order by " + parametroConsulta.PropriedadeAgrupar + " " + parametroConsulta.PropriedadeOrdenar;
            }

            if (!string.IsNullOrWhiteSpace(parametroConsulta.PropriedadeOrdenar) && parametroConsulta.PropriedadeAgrupar != parametroConsulta.PropriedadeOrdenar)
            {
                if (agrup)
                {
                    query += ", " + parametroConsulta.PropriedadeOrdenar + " " + parametroConsulta.DirecaoOrdenar;
                }
                else
                {
                    query += " order by " + parametroConsulta.PropriedadeOrdenar + " " + parametroConsulta.DirecaoOrdenar;
                }
            }

            if (paginar)
                query += " OFFSET " + parametroConsulta.InicioRegistros + " ROWS FETCH FIRST " + parametroConsulta.LimiteRegistros + " ROWS ONLY";


            var nhQuery = this.SessionNHiBernate.CreateSQLQuery(query);

            nhQuery.SetResultTransformer(new NHibernate.Transform.AliasToBeanResultTransformer(typeof(Dominio.ObjetosDeValor.Embarcador.TorreControle.ConsultaPorEntrega)));

            return nhQuery.SetTimeout(50000).List<Dominio.ObjetosDeValor.Embarcador.TorreControle.ConsultaPorEntrega>();
        }


        #region Métodos Privados

        private string ObterCamposConsultaEntregaAtrasada()
        {
            return @"SELECT Carga.CAR_CODIGO AS Codigo,
                            CargaEntrega.CEN_CODIGO AS CodigoEntrega,
                            Carga.CAR_CODIGO_CARGA_EMBARCADOR AS Carga,
                            DadosSumarizados.CDS_PESO_TOTAL AS Peso,
						    TipoOperacao.TOP_DESCRICAO AS TipoOperacao,
						    Destinatario.CLI_NOME AS Cliente,
						    Transportador.EMP_FANTASIA AS Transportador,
    						Pedido.PED_DATA_PRIMEIRO_AGENDAMENTO AS DataAgendamento,
							CargaEntrega.CEN_DATA_ENTREGA AS DataConfirmacaoEntrega,
                            Responsavel.TRA_CODIGO TipoResponsavel,
                            Responsavel.TRA_DESCRICAO DescricaoResponsavel
					";
        }

        private string ObterFromConsultaEntregaAtrasada()
        {
            return @"FROM T_CARGA AS Carga
                     JOIN T_CARGA_DADOS_SUMARIZADOS AS DadosSumarizados
					   ON Carga.CDS_CODIGO = DadosSumarizados.CDS_CODIGO
                     JOIN T_CARGA_PEDIDO CargaPedido 
                       ON CargaPedido.CAR_CODIGO = Carga.CAR_CODIGO
                     LEFT JOIN T_CARGA_ENTREGA_PEDIDO CargaEntregaPedido
					   ON CargaEntregaPedido.CPE_CODIGO = CargaPedido.CPE_CODIGO
                     LEFT JOIN T_CARGA_ENTREGA CargaEntrega 
					   ON CargaEntrega.CEN_CODIGO = CargaEntregaPedido.CEN_CODIGO
                     LEFT JOIN T_TIPO_RESPONSAVEL_ATRASO_ENTREGA Responsavel
                       ON Responsavel.TRA_CODIGO = CargaEntrega.TRA_CODIGO
					 LEFT JOIN T_TIPO_OPERACAO AS TipoOperacao
					   ON TipoOperacao.TOP_CODIGO = Carga.TOP_CODIGO
					 LEFT JOIN T_EMPRESA AS Transportador
					   ON Transportador.EMP_CODIGO = Carga.EMP_CODIGO
					 JOIN T_PEDIDO AS Pedido
					   ON Pedido.PED_CODIGO = CargaPedido.PED_CODIGO
                     LEFT JOIN T_PEDIDO_XML_NOTA_FISCAL PedidoXMLNotaFiscal
                       ON PedidoXMLNotaFiscal.CPE_CODIGO = CargaPedido.CPE_CODIGO
                     LEFT JOIN T_XML_NOTA_FISCAL NotaFiscal
                       ON NotaFiscal.NFX_CODIGO = PedidoXMLNotaFiscal.NFX_CODIGO
					 LEFT JOIN T_CLIENTE AS Destinatario
					   ON Destinatario.CLI_CGCCPF = NotaFiscal.CLI_CODIGO_DESTINATARIO
				";
        }

        private string ObterGroupByConsultaEntregaAtrasada()
        {
            return @"GROUP BY
                            Carga.CAR_CODIGO,
                            CargaEntrega.CEN_CODIGO,
                            Carga.CAR_CODIGO_CARGA_EMBARCADOR,
                            DadosSumarizados.CDS_PESO_TOTAL,
						    TipoOperacao.TOP_DESCRICAO,
						    Destinatario.CLI_NOME,
						    Transportador.EMP_FANTASIA,
    						Pedido.PED_DATA_PRIMEIRO_AGENDAMENTO,
							CargaEntrega.CEN_DATA_ENTREGA,
                            Responsavel.TRA_CODIGO,
                            Responsavel.TRA_DESCRICAO";
        }

        private string ObterWhereConsultaEntregaAtrasada(Dominio.ObjetosDeValor.Embarcador.TorreControle.FiltroPesquisaConsultaEntregaAtrasada filtrosPesquisa)
        {
            string pattern = "yyyy-MM-dd";

            StringBuilder where = new StringBuilder(" WHERE Carga.CAR_SITUACAO NOT IN (13, 18) AND CargaEntrega.CEN_SITUACAO IN (2,5,3,6,7,8,9) AND CargaEntrega.CEN_DATA_ENTREGA is not null AND CargaEntrega.CEN_DATA_ENTREGA > CargaEntrega.CEN_DATA_AGENDAMENTO ");

            if (!string.IsNullOrWhiteSpace(filtrosPesquisa.NumeroCarga))
                where.Append($" AND Carga.CAR_CODIGO_CARGA_EMBARCADOR = '{filtrosPesquisa.NumeroCarga}' ");

            if (filtrosPesquisa.Cliente > 0)
                where.Append($" AND Destinatario.CLI_CGCCPF = {filtrosPesquisa.Cliente} ");

            if (filtrosPesquisa.NumeroNota > 0)
                where.Append($" AND NotaFiscal.NF_NUMERO = {filtrosPesquisa.NumeroNota} ");

            if (filtrosPesquisa.TipoOperacao > 0)
                where.Append($" AND Carga.TOP_CODIGO = {filtrosPesquisa.TipoOperacao} ");

            if (filtrosPesquisa.Transportador > 0)
                where.Append($" AND Carga.EMP_CODIGO = {filtrosPesquisa.Transportador} ");

            if (filtrosPesquisa.DataPrevisaoEntregaInicial.HasValue)
                where.Append($" AND CargaEntrega.CEN_DATA_ENTREGA_PREVISTA >= '{filtrosPesquisa.DataPrevisaoEntregaInicial.Value.ToString(pattern)} 00:00:00' ");

            if (filtrosPesquisa.DataPrevisaoEntregaFinal.HasValue)
                where.Append($" AND CargaEntrega.CEN_DATA_ENTREGA_PREVISTA <= '{filtrosPesquisa.DataPrevisaoEntregaFinal.Value.ToString(pattern)} 23:59:59' ");

            return where.ToString();
        }

        #endregion


        #endregion
    }
}
