using NHibernate.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Dynamic.Core;

namespace Repositorio.Embarcador.CotacaoPedido
{
    public class CotacaoPedido : RepositorioBase<Dominio.Entidades.Embarcador.CotacaoPedido.CotacaoPedido>
    {
        public CotacaoPedido(UnitOfWork unitOfWork) : base(unitOfWork) { }

        #region Métodos Públicos

        public Dominio.Entidades.Embarcador.CotacaoPedido.CotacaoPedido BuscarPorCodigo(long codigo)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.CotacaoPedido.CotacaoPedido>();
            var result = from obj in query where obj.Codigo == codigo select obj;
            return result.FirstOrDefault();
        }

        public Dominio.Entidades.Embarcador.CotacaoPedido.CotacaoPedido BuscarPorCodigoIntegracao(string codigoIntegracao)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.CotacaoPedido.CotacaoPedido>();
            var result = from obj in query where obj.CodigoIntegracao == codigoIntegracao select obj;
            return result.FirstOrDefault();
        }

        public int BuscarProximoNumero(int codigoEmpresa)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.CotacaoPedido.CotacaoPedido>();
            var result = from obj in query select obj;

            if (codigoEmpresa > 0)
                result = result.Where(o => o.Empresa.Codigo == codigoEmpresa);

            var resultNumero = result.Select(o => o.Numero);

            int maiorNumero = 0;
            if (resultNumero.Count() > 0)
                maiorNumero = resultNumero.Max();

            return maiorNumero + 1;
        }

        public List<Dominio.Entidades.Embarcador.CotacaoPedido.CotacaoPedido> BuscarPorPedido(int codigoPedido)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.CotacaoPedido.CotacaoPedido>();

            var result = from obj in query where obj.Pedido.Codigo == codigoPedido select obj;

            return result.ToList();
        }

        public List<Dominio.Entidades.Embarcador.CotacaoPedido.CotacaoPedido> BuscarPorSolicitacao(long codigoSolicitacao)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.CotacaoPedido.CotacaoPedido>();

            var result = from obj in query where obj.SolicitacaoCotacao.Codigo == codigoSolicitacao select obj;

            return result.ToList();
        }

        public List<Dominio.Entidades.Embarcador.CotacaoPedido.CotacaoPedido> BuscarPorCodigoPedido(int codigoPedido)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.CotacaoPedido.CotacaoPedido>();

            var result = from obj in query where obj.Pedido.Codigo == codigoPedido select obj;

            return result.ToList();
        }

        public List<Dominio.Entidades.Embarcador.CotacaoPedido.CotacaoPedido> Consultar(int codigoEmpresa, int numero, int clienteProspect, int grupoPessoas, int tipoDeCarga, int tipoOperacao, double clienteAtivo, double clienteInativo, double destinatario, DateTime dataPrevista, Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoClienteCotacaoPedido tipoClienteCotacaoPedido, Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoModal tipoModal, Dominio.ObjetosDeValor.Embarcador.Enumeradores.StatusCotacaoPedido statusCotacaoPedido, Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoPedido situacaoPedido, string propOrdena, string dirOrdena, int inicioRegistros, int maximoRegistros)
        {
            var result = _Consultar(codigoEmpresa, numero, clienteProspect, grupoPessoas, tipoDeCarga, tipoOperacao, clienteAtivo, clienteInativo, destinatario, dataPrevista, tipoClienteCotacaoPedido, tipoModal, statusCotacaoPedido, situacaoPedido);

            if (!string.IsNullOrWhiteSpace(propOrdena))
                result = result.OrderBy(propOrdena + (dirOrdena == "asc" ? " ascending" : " descending"));

            if (inicioRegistros > 0)
                result = result.Skip(inicioRegistros);

            if (maximoRegistros > 0)
                result = result.Take(maximoRegistros);

            return result.ToList();
        }

        public int ContarConsulta(int codigoEmpresa, int numero, int clienteProspect, int grupoPessoas, int tipoDeCarga, int tipoOperacao, double clienteAtivo, double clienteInativo, double destinatario, DateTime dataPrevista, Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoClienteCotacaoPedido tipoClienteCotacaoPedido, Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoModal tipoModal, Dominio.ObjetosDeValor.Embarcador.Enumeradores.StatusCotacaoPedido statusCotacaoPedido, Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoPedido situacaoPedido)
        {
            var result = _Consultar(codigoEmpresa, numero, clienteProspect, grupoPessoas, tipoDeCarga, tipoOperacao, clienteAtivo, clienteInativo, destinatario, dataPrevista, tipoClienteCotacaoPedido, tipoModal, statusCotacaoPedido, situacaoPedido);

            return result.Count();
        }

        public List<Dominio.Entidades.Embarcador.CotacaoPedido.CotacaoPedido> ConsultarParaCotacaoFrete(Dominio.ObjetosDeValor.Embarcador.Cotacoes.FiltroPesquisaCotacaoFrete filtrosPesquisa, Dominio.ObjetosDeValor.Embarcador.Consulta.ParametroConsulta parametroConsulta)
        {
            var result = _ConsultarParaCotacaoFrete(filtrosPesquisa);

            result = result
                .Fetch(o => o.Pedido)
                .ThenFetch(o => o.Remetente)
                .Fetch(o => o.SolicitacaoCotacao)
                .ThenFetch(o => o.Remetente)
                .Fetch(o => o.Destino)
                .Fetch(o => o.Empresa);

            return ObterLista(result, parametroConsulta);
        }

        public int ContarConsultaParaCotacaoFrete(Dominio.ObjetosDeValor.Embarcador.Cotacoes.FiltroPesquisaCotacaoFrete filtrosPesquisa)
        {
            var result = _ConsultarParaCotacaoFrete(filtrosPesquisa);

            return result.Count();
        }

        public void ExcluirPorCodigoPedido(int codigoPedido)
        {
            try
            {
                if (UnitOfWork.IsActiveTransaction())
                    this.ExecuteExcluirPorCodigoPedido(codigoPedido);
                else
                {
                    try
                    {
                        UnitOfWork.Start();

                        this.ExecuteExcluirPorCodigoPedido(codigoPedido);

                        UnitOfWork.CommitChanges();
                    }
                    catch
                    {
                        UnitOfWork.Rollback();
                        throw;
                    }
                }
            }
            catch (NHibernate.Exceptions.GenericADOException ex)
            {
                if (ex.InnerException != null && object.ReferenceEquals(ex.InnerException.GetType(), typeof(System.Data.SqlClient.SqlException)))
                {
                    System.Data.SqlClient.SqlException excecao = (System.Data.SqlClient.SqlException)ex.InnerException;
                    if (excecao.Number == 547)
                    {
                        throw new Exception("O registro possui dependências e não pode ser excluido.", ex);
                    }
                }
                throw;
            }
        }

        public void RemoverPedidoDeCotacoes(int codigoPedido)
        {
            if (UnitOfWork.IsActiveTransaction())
                this.ExecuteRemoverPedidoDeCotacoes(codigoPedido);
            else
            {
                try
                {
                    UnitOfWork.Start();
                    this.ExecuteRemoverPedidoDeCotacoes(codigoPedido);
                    UnitOfWork.CommitChanges();
                }
                catch
                {
                    UnitOfWork.Rollback();
                    throw;
                }
            }
        }

        public Dominio.Entidades.Embarcador.CotacaoPedido.CotacaoPedido BuscarPorPedidoTransportador(int codigoPedido, int codigoTransportador)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.CotacaoPedido.CotacaoPedido>();

            var result = from obj in query where obj.Pedido.Codigo == codigoPedido && obj.Empresa.Codigo == codigoTransportador select obj;

            return result.FirstOrDefault();
        }

        public bool ExisteCotacaoPorPedidoTransportador(int codigoPedido, int codigoTransportador)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.CotacaoPedido.CotacaoPedido>();

            var result = from obj in query where obj.Pedido.Codigo == codigoPedido && obj.Empresa.Codigo == codigoTransportador select obj;

            return result.Any();
        }

        #endregion

        #region Relatórios

        public IList<Dominio.Relatorios.Embarcador.DataSource.Cotacoes.CotacaoPedido> RelatorioCotacaoPedido(int codigoCotacaoPedido)
        {
            string query = @"   SELECT C.CTP_CODIGO Codigo,
                                C.CTP_NUMERO Numero,
                                C.CTP_DATA Data,
                                C.CTP_PREVISAO DataPrevista,
                                F.FUN_NOME Usuario,
                                CASE
	                                WHEN C.CTP_TIPO_PESSOA_COTACAO_PEDIDO = 2 THEN PP.CPR_NOME
	                                WHEN C.CTP_TIPO_PESSOA_COTACAO_PEDIDO = 3 THEN PA.CLI_NOME
	                                WHEN C.CTP_TIPO_PESSOA_COTACAO_PEDIDO = 4 THEN PI.CLI_NOME
	                                WHEN C.CTP_TIPO_PESSOA_COTACAO_PEDIDO = 5 THEN GP.GRP_DESCRICAO
	                                ELSE C.CTP_CLIENTE_NOVO
                                END Pessoa,
                                T.TOP_DESCRICAO TipoOperacao,
                                C.CTP_SOLICITANTE Solicitante,
                                C.CTP_TELEFONE_CONTATO Telefone,
                                C.CTP_EMAIL_CONTATO Email,
                                CASE
	                                WHEN C.CTP_TIPO_MODAL = 2 THEN 'Aéreo'
	                                WHEN C.CTP_TIPO_MODAL = 3 THEN 'Aquaviário'
	                                WHEN C.CTP_TIPO_MODAL = 4 THEN 'Ferroviário'
	                                WHEN C.CTP_TIPO_MODAL = 5 THEN 'Dutoviário'
	                                WHEN C.CTP_TIPO_MODAL = 6 THEN 'Multimodal'
	                                ELSE 'Rodoviário'
                                END Modal,

                                CASE
	                                WHEN C.CTP_USAR_OUTRO_ENDERECO_ORIGEM = 1 THEN ISNULL(LEO.LOC_DESCRICAO, '') + ' - '+ ISNULL(LEO.UF_SIGLA, '') + ' / ' + ISNULL(EO.CPE_BAIRRO, '') + ' / ' + 
		                                ISNULL(EO.CPE_ENDERECO, '') + ' - ' + ISNULL(EO.CPE_NUMERO, '') + ', ' + ISNULL(EO.CPE_COMPLEMENTO, '') + ' ' + ISNULL(EO.CPE_CEP, '') + ' ' + ISNULL(EO.CPE_FONE, '') + ' ' + ISNULL(EO.CPE_IERG, '')
	                                ELSE ISNULL(LO.LOC_DESCRICAO, '') + ' - ' + ISNULL(LO.UF_SIGLA, '') + 
	                                    CASE WHEN C.CTP_TIPO_PESSOA_COTACAO_PEDIDO = 3 THEN ' / ' + ISNULL(PA.CLI_BAIRRO, '') + ' / ' + 
		                                    ISNULL(PA.CLI_ENDERECO, '') + ' - ' + ISNULL(PA.CLI_NUMERO, '') + ', ' + ISNULL(PA.CLI_COMPLEMENTO, '') + ' ' + ISNULL(PA.CLI_CEP, '') + ' ' + ISNULL(PA.CLI_FONE, '') + ' ' + ISNULL(PA.CLI_IERG, '')
	                                    WHEN C.CTP_TIPO_PESSOA_COTACAO_PEDIDO = 4 THEN ' / ' + ISNULL(PI.CLI_BAIRRO, '') + ' / ' + 
		                                    ISNULL(PI.CLI_ENDERECO, '') + ' - ' + ISNULL(PI.CLI_NUMERO, '') + ', ' + ISNULL(PI.CLI_COMPLEMENTO, '') + ' ' + ISNULL(PI.CLI_CEP, '') + ' ' + ISNULL(PI.CLI_FONE, '') + ' ' + ISNULL(PI.CLI_IERG, '')
	                                    ELSE '' END	
                                END Origem,

                                CASE
	                                WHEN C.CTP_USAR_OUTRO_ENDERECO_DESTINO = 1 THEN ISNULL(LED.LOC_DESCRICAO, '') + ' - '+ ISNULL(LED.UF_SIGLA, '') + ' / ' + ISNULL(ED.CPE_BAIRRO, '') + ' / ' + 
		                                ISNULL(ED.CPE_ENDERECO, '') + ' - ' + ISNULL(ED.CPE_NUMERO, '') + ', ' + ISNULL(ED.CPE_COMPLEMENTO, '') + ' ' + ISNULL(ED.CPE_CEP, '') + ' ' + ISNULL(ED.CPE_FONE, '') + ' ' + ISNULL(ED.CPE_IERG, '')
	                                ELSE ISNULL(LD.LOC_DESCRICAO, '') + ' - ' + ISNULL(LD.UF_SIGLA, '') + 
	                                    CASE WHEN C.CLI_CODIGO IS NOT NULL THEN ' / ' + ISNULL(PD.CLI_BAIRRO, '') + ' / ' + 
		                                    ISNULL(PD.CLI_ENDERECO, '') + ' - ' + ISNULL(PD.CLI_NUMERO, '') + ', ' + ISNULL(PD.CLI_COMPLEMENTO, '') + ' ' + ISNULL(PD.CLI_CEP, '') + ' ' + ISNULL(PD.CLI_FONE, '') + ' ' + ISNULL(PD.CLI_IERG, '')
	                                    ELSE '' END
                                END Destino,

                                C.CTP_QUANTIDADE_VOLUMES Volumes,
                                C.CTP_NUMERO_PALETES Pallets,
                                C.CTP_PESO_TOTAL_CARGA PesoBruto,
                                C.CTP_PESO_CUBADO PesoCubo,
                                C.CTP_VALOR_TOTAL_NOTAS_FISCAIS ValorMercadoria,
                                C.CTP_QUANTIDADE_NOTAS QuantidadeNotaFiscal,
                                C.CTP_QUANTIDADE_ENTREGAS QuantidadeEntrega,
                                C.CTP_QUANTIDADE_AJUDANTE QuantidadeAjudante,
                                C.CTP_QUANTIDADE_ESCOLTA QuantidadeEscoltaArmada,
                                M.MVC_DESCRICAO ModeloVeiculo,
                                C.CTP_DATA_INICIAL_COLETA DataInicialColeta,
                                C.CTP_DATA_FINAL_COLETA DataFinalColeta,
                                C.CTP_OBSERVACAO Observacao,

                                CF.CFR_DESCRICAO Componente,
                                CC.CPC_VALOR_TOTAL ComponenteValor,

                                C.CTP_VALOR_COTACAO ValorCotacao,
                                C.CTP_PERCENTUAL_ACRESCIMO PercentualAcrescimo,
                                C.CTP_PERCENTUAL_DESCONTO PercentualDesconto,
                                C.CTP_TOTAL_VALOR_COTACAO ValorTotalCotacao,
                                C.CTP_VALOR_FRETE ValorFrete,
                                C.CTP_VALOR_ICMS ValorICMS,
                                C.CTP_TOTAL_VALOR_COM_ICMS ValorTotalComICMS

                                FROM T_COTACAO_PEDIDO C
                                LEFT OUTER JOIN T_FUNCIONARIO F ON F.FUN_CODIGO = C.FUN_CODIGO
                                LEFT OUTER JOIN T_TIPO_OPERACAO T ON T.TOP_CODIGO = C.TOP_CODIGO
                                LEFT OUTER JOIN T_MODELO_VEICULAR_CARGA M ON M.MVC_CODIGO = C.MVC_CODIGO

                                LEFT OUTER JOIN T_CLIENTE PA ON PA.CLI_CGCCPF = C.CLI_CODIGO_ATIVO
                                LEFT OUTER JOIN T_CLIENTE PI ON PI.CLI_CGCCPF = C.CLI_CODIGO_INATIVO
                                LEFT OUTER JOIN T_CLIENTE_PROSPECT PP ON PP.CPR_CODIGO = C.CPR_CODIGO
                                LEFT OUTER JOIN T_GRUPO_PESSOAS GP ON GP.GRP_CODIGO = C.GRP_CODIGO

                                LEFT OUTER JOIN T_CLIENTE PD ON PD.CLI_CGCCPF = C.CLI_CODIGO
                                LEFT OUTER JOIN T_LOCALIDADES LO ON LO.LOC_CODIGO = C.LOC_CODIGO_ORIGEM
                                LEFT OUTER JOIN T_LOCALIDADES LD ON LD.LOC_CODIGO = C.LOC_CODIGO_DESTINO
                                LEFT OUTER JOIN T_COTACAO_PEDIDO_ENDERECO EO ON EO.CPE_CODIGO = C.CPE_CODIGO_ORIGEM
                                LEFT OUTER JOIN T_COTACAO_PEDIDO_ENDERECO ED ON ED.CPE_CODIGO = C.CPE_CODIGO_DESTINO
                                LEFT OUTER JOIN T_LOCALIDADES LEO ON LEO.LOC_CODIGO = EO.LOC_CODIGO
                                LEFT OUTER JOIN T_LOCALIDADES LED ON LED.LOC_CODIGO = ED.LOC_CODIGO

                                LEFT OUTER JOIN T_COTACAO_PEDIDO_COMPONENTE CC ON CC.CTP_CODIGO = C.CTP_CODIGO
                                LEFT OUTER JOIN T_COMPONENTE_FRETE CF ON CF.CFR_CODIGO = CC.CFR_CODIGO
                                WHERE C.CTP_CODIGO = " + codigoCotacaoPedido.ToString();

            var nhQuery = this.SessionNHiBernate.CreateSQLQuery(query);

            nhQuery.SetResultTransformer(new NHibernate.Transform.AliasToBeanResultTransformer(typeof(Dominio.Relatorios.Embarcador.DataSource.Cotacoes.CotacaoPedido)));

            return nhQuery.List<Dominio.Relatorios.Embarcador.DataSource.Cotacoes.CotacaoPedido>();
        }

        #endregion

        #region Métodos Privados

        private IQueryable<Dominio.Entidades.Embarcador.CotacaoPedido.CotacaoPedido> _Consultar(int codigoEmpresa, int numero, int clienteProspect, int grupoPessoas, int tipoDeCarga, int tipoOperacao, double clienteAtivo, double clienteInativo, double destinatario, DateTime dataPrevista, Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoClienteCotacaoPedido tipoClienteCotacaoPedido, Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoModal tipoModal, Dominio.ObjetosDeValor.Embarcador.Enumeradores.StatusCotacaoPedido statusCotacaoPedido, Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoPedido situacaoPedido)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.CotacaoPedido.CotacaoPedido>();

            var result = from obj in query select obj;

            // Filtros
            if (codigoEmpresa > 0)
                result = result.Where(o => o.Empresa.Codigo == codigoEmpresa);

            if (numero > 0)
                result = result.Where(o => o.Numero == numero);

            if (clienteProspect > 0)
                result = result.Where(o => o.ClienteProspect.Codigo == clienteProspect);

            if (grupoPessoas > 0)
                result = result.Where(o => o.GrupoPessoas.Codigo == grupoPessoas);

            if (tipoDeCarga > 0)
                result = result.Where(o => o.TipoDeCarga.Codigo == tipoDeCarga);

            if (tipoOperacao > 0)
                result = result.Where(o => o.TipoOperacao.Codigo == tipoOperacao);

            if (clienteAtivo > 0)
                result = result.Where(o => o.ClienteAtivo.CPF_CNPJ == clienteAtivo);

            if (clienteInativo > 0)
                result = result.Where(o => o.ClienteInativo.CPF_CNPJ == clienteInativo);

            if (destinatario > 0)
                result = result.Where(o => o.Destinatario.CPF_CNPJ == destinatario);

            if (dataPrevista > DateTime.MinValue)
                result = result.Where(o => o.Previsao.Value.Date == dataPrevista.Date);

            if (tipoClienteCotacaoPedido != Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoClienteCotacaoPedido.Todos)
                result = result.Where(o => o.TipoClienteCotacaoPedido == tipoClienteCotacaoPedido);

            if (tipoModal != Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoModal.Todos)
                result = result.Where(o => o.TipoModal == tipoModal);

            if (statusCotacaoPedido != Dominio.ObjetosDeValor.Embarcador.Enumeradores.StatusCotacaoPedido.Todos)
                result = result.Where(o => o.StatusCotacaoPedido == statusCotacaoPedido);

            if (situacaoPedido != Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoPedido.Todos)
                result = result.Where(o => o.SituacaoPedido == situacaoPedido);

            return result;
        }

        private IQueryable<Dominio.Entidades.Embarcador.CotacaoPedido.CotacaoPedido> _ConsultarParaCotacaoFrete(Dominio.ObjetosDeValor.Embarcador.Cotacoes.FiltroPesquisaCotacaoFrete filtrosPesquisa)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.CotacaoPedido.CotacaoPedido>();

            var result = from obj in query where obj.Pedido != null || obj.SolicitacaoCotacao != null select obj;

            if (filtrosPesquisa.DataCotacaoInicial != DateTime.MinValue)
                result = result.Where(o => o.DataCriacao.Value.Date >= filtrosPesquisa.DataCotacaoInicial);

            if (filtrosPesquisa.DataCotacaoFinal != DateTime.MinValue)
                result = result.Where(o => o.DataCriacao.Value.Date <= filtrosPesquisa.DataCotacaoFinal);

            if (!string.IsNullOrWhiteSpace(filtrosPesquisa.NumeroPedido))
                result = result.Where(o => o.Pedido.NumeroPedidoEmbarcador == filtrosPesquisa.NumeroPedido);

            if (filtrosPesquisa.CpfCnpjExpedidor > 0)
                result = result.Where(o => o.Pedido.Remetente.CPF_CNPJ == filtrosPesquisa.CpfCnpjExpedidor);

            if (filtrosPesquisa.CodigoDestino > 0)
                result = result.Where(o => o.Pedido.Destino.Codigo == filtrosPesquisa.CodigoDestino);

            if (filtrosPesquisa.CodigoTransportador > 0)
                result = result.Where(o => o.Empresa.Codigo == filtrosPesquisa.CodigoTransportador);

            if (filtrosPesquisa.NumeroCotacao > 0)
                result = result.Where(o => o.Codigo == filtrosPesquisa.NumeroCotacao);

            return result;
        }

        private void ExecuteExcluirPorCodigoPedido(int codigoPedido)
        {
            UnitOfWork.Sessao.CreateSQLQuery("DELETE FROM T_COTACAO_PEDIDO_COMPONENTE WHERE CTP_CODIGO IN (SELECT CTP_CODIGO FROM T_COTACAO_PEDIDO WHERE PED_CODIGO = :codigoPedido );").SetInt32("codigoPedido", codigoPedido).ExecuteUpdate();
            UnitOfWork.Sessao.CreateSQLQuery("DELETE FROM T_COTACAO_PEDIDO_AUTORIZACAO WHERE CTP_CODIGO IN (SELECT CTP_CODIGO FROM T_COTACAO_PEDIDO WHERE PED_CODIGO = :codigoPedido );").SetInt32("codigoPedido", codigoPedido).ExecuteUpdate();
            UnitOfWork.Sessao.CreateSQLQuery("DELETE FROM T_COTACAO_PEDIDO_CUBAGEM WHERE CTP_CODIGO IN (SELECT CTP_CODIGO FROM T_COTACAO_PEDIDO WHERE PED_CODIGO = :codigoPedido );").SetInt32("codigoPedido", codigoPedido).ExecuteUpdate();
            UnitOfWork.Sessao.CreateSQLQuery("DELETE FROM T_COTACAO_PEDIDO_IMPORTACAO WHERE CTP_CODIGO IN (SELECT CTP_CODIGO FROM T_COTACAO_PEDIDO WHERE PED_CODIGO = :codigoPedido );").SetInt32("codigoPedido", codigoPedido).ExecuteUpdate();
            UnitOfWork.Sessao.CreateSQLQuery("UPDATE T_PEDIDO SET CTP_CODIGO = NULL WHERE PED_CODIGO = :codigoPedido ;").SetInt32("codigoPedido", codigoPedido).ExecuteUpdate();
            UnitOfWork.Sessao.CreateSQLQuery("DELETE FROM T_COTACAO_PEDIDO WHERE PED_CODIGO = :codigoPedido ;").SetInt32("codigoPedido", codigoPedido).ExecuteUpdate();
        }

        private void ExecuteRemoverPedidoDeCotacoes(int codigoPedido)
        {
            UnitOfWork.Sessao.CreateSQLQuery("UPDATE T_COTACAO_PEDIDO SET PED_CODIGO = NULL WHERE PED_CODIGO = :codigoPedido;").SetInt32("codigoPedido", codigoPedido).ExecuteUpdate();
        }

        #endregion
    }
}
