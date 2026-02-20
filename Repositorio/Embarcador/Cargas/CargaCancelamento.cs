using Dominio.ObjetosDeValor.Embarcador.Enumeradores;
using NHibernate.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Dynamic.Core;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Repositorio.Embarcador.Cargas
{
    public class CargaCancelamento : RepositorioBase<Dominio.Entidades.Embarcador.Cargas.CargaCancelamento>
    {
        public CargaCancelamento(UnitOfWork unitOfWork) : base(unitOfWork) { }

        #region Métodos Públicos

        public Dominio.Entidades.Embarcador.Cargas.CargaCancelamento BuscarPorCodigo(int codigo)
        {
            var query = SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.CargaCancelamento>();
            var result = query.Where(obj => obj.Codigo == codigo);
            return result.Fetch(x => x.Carga).FirstOrDefault();
        }

        public Dominio.Entidades.Embarcador.Cargas.CargaCancelamento BuscarPorCargaDuplicada(int codigoCarga)
        {
            var query = SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.CargaCancelamento>();
            var result = query.Where(obj => obj.CargaDuplicada.Codigo == codigoCarga);
            return result.FirstOrDefault();
        }

        public Dominio.Entidades.Embarcador.Cargas.CargaCancelamento BuscarPorCargaDuplicadaPorProtocolo(int protocoloCarga)
        {
            IQueryable<Dominio.Entidades.Embarcador.Cargas.CargaCancelamento> query = SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.CargaCancelamento>();

            query = query.Where(obj => obj.CargaDuplicada.Protocolo == protocoloCarga);

            return query.FirstOrDefault();
        }

        public bool VerificarCargaDuplicada(int codigoCarga)
        {
            var query = SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.CargaCancelamento>();
            var result = query.Where(obj => obj.CargaDuplicada.Codigo == codigoCarga);
            return result.Count() > 0;
        }

        public Dominio.Entidades.Embarcador.Cargas.CargaCancelamento BuscarPorCarga(int codigoCarga)
        {
            return ObterQueryBuscarPorCarga(codigoCarga).FirstOrDefault();
        }

        public Task<Dominio.Entidades.Embarcador.Cargas.CargaCancelamento> BuscarPorCargaAsync(int codigoCarga, CancellationToken cancellationToken)
        {
            return ObterQueryBuscarPorCarga(codigoCarga).FirstOrDefaultAsync(cancellationToken);
        }

        public async Task<Dominio.Entidades.Embarcador.Cargas.CargaCancelamento> BuscarPorCargaComFilialEmpresaPedidosAsync(int codigoCarga)
        {
            var result = ObterQueryBuscarPorCarga(codigoCarga);

            result
                .Fetch(x => x.Carga.Filial).ThenFetch(x => x.Atividade)
                .Fetch(x => x.Carga.Empresa)
                .Fetch(x => x.Carga.Pedidos);

            return await result.FirstOrDefaultAsync(CancellationToken);
        }

        public Dominio.Entidades.Embarcador.Cargas.CargaCancelamento BuscarCancelamentoCargaPorDocumento(string codigoCargaEmbarcador)
        {
            var query = SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.CargaCancelamento>();
            var result = query.Where(obj => obj.Carga.CodigoCargaEmbarcador == codigoCargaEmbarcador
                                    && obj.TipoCancelamentoCargaDocumento == TipoCancelamentoCargaDocumento.Documentos);
            return result.FirstOrDefault();
        }

        public Dominio.Entidades.Embarcador.Cargas.CargaCancelamento BuscarPorCargaNaoReprovada(int codigoCarga)
        {
            var query = SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.CargaCancelamento>();
            var result = query.Where(obj => obj.Carga.Codigo == codigoCarga && obj.Situacao != SituacaoCancelamentoCarga.SolicitacaoReprovada
                && ((TipoCancelamentoCargaDocumento?)obj.TipoCancelamentoCargaDocumento ?? TipoCancelamentoCargaDocumento.Carga) == TipoCancelamentoCargaDocumento.Carga);
            return result.FirstOrDefault();
        }

        public List<Dominio.Entidades.Embarcador.Cargas.CargaCancelamento> BuscarPorCargas(List<int> codigoCargas)
        {
            var query = SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.CargaCancelamento>();
            var result = query.Where(obj => codigoCargas.Contains(obj.Carga.Codigo) && ((TipoCancelamentoCargaDocumento?)obj.TipoCancelamentoCargaDocumento ?? TipoCancelamentoCargaDocumento.Carga) == TipoCancelamentoCargaDocumento.Carga);
            return result.ToList();
        }

        public Dominio.Entidades.Embarcador.Cargas.CargaCancelamento BuscarPorProtocoloCarga(int protocoloCarga)
        {
            IQueryable<Dominio.Entidades.Embarcador.Cargas.CargaCancelamento> query = SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.CargaCancelamento>();

            query = query.Where(obj => obj.Carga.Protocolo == protocoloCarga && ((TipoCancelamentoCargaDocumento?)obj.TipoCancelamentoCargaDocumento ?? TipoCancelamentoCargaDocumento.Carga) == TipoCancelamentoCargaDocumento.Carga);

            return query.FirstOrDefault();
        }

        public Task<Dominio.Entidades.Embarcador.Cargas.CargaCancelamento> BuscarPorProtocoloCargaAsync(int protocoloCarga)
        {
            IQueryable<Dominio.Entidades.Embarcador.Cargas.CargaCancelamento> query = SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.CargaCancelamento>();

            query = query.Where(obj => obj.Carga.Protocolo == protocoloCarga && ((TipoCancelamentoCargaDocumento?)obj.TipoCancelamentoCargaDocumento ?? TipoCancelamentoCargaDocumento.Carga) == TipoCancelamentoCargaDocumento.Carga);

            return query.FirstOrDefaultAsync();
        }

        public List<Dominio.Entidades.Embarcador.Cargas.Carga> BuscarPorCargaPendentesConfirmacao(int codigoGrupoPessoas, int inicioRegistros, int maximoRegistros)
        {
            IQueryable<Dominio.Entidades.Embarcador.Cargas.CargaCancelamento> query = SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.CargaCancelamento>();

            query = query.Where(obj => obj.Carga.Protocolo > 0 && (obj.Carga.SituacaoCarga == SituacaoCarga.Cancelada || obj.Carga.SituacaoCarga == SituacaoCarga.Anulada) && !obj.confirmacaoERP
                && ((TipoCancelamentoCargaDocumento?)obj.TipoCancelamentoCargaDocumento ?? TipoCancelamentoCargaDocumento.Carga) == TipoCancelamentoCargaDocumento.Carga);

            if (codigoGrupoPessoas > 0)
                query = query.Where(o => o.Carga.GrupoPessoaPrincipal.Codigo == codigoGrupoPessoas);

            return query.Select(o => o.Carga).Skip(inicioRegistros).Take(maximoRegistros).ToList();
        }

        public int ContarPorCargaPendentesConfirmacao(int codigoGrupoPessoas)
        {
            IQueryable<Dominio.Entidades.Embarcador.Cargas.CargaCancelamento> query = SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.CargaCancelamento>();

            query = query.Where(obj => obj.Carga.Protocolo > 0 && (obj.Carga.SituacaoCarga == SituacaoCarga.Cancelada || obj.Carga.SituacaoCarga == SituacaoCarga.Anulada) && !obj.confirmacaoERP
                && ((TipoCancelamentoCargaDocumento?)obj.TipoCancelamentoCargaDocumento ?? TipoCancelamentoCargaDocumento.Carga) == TipoCancelamentoCargaDocumento.Carga);

            if (codigoGrupoPessoas > 0)
                query = query.Where(o => o.Carga.GrupoPessoaPrincipal.Codigo == codigoGrupoPessoas);

            return query.Select(o => o.Carga).Count();
        }

        public List<Dominio.Entidades.Embarcador.Cargas.Carga> BuscarPorCargaProblemaEmissao(int inicioRegistros, int maximoRegistros)
        {
            var query = SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.Carga>();
            var result = query.Where(obj => obj.SituacaoCarga == Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoCarga.PendeciaDocumentos && obj.PossuiPendencia);
            return result.Skip(inicioRegistros).Take(maximoRegistros).ToList();
        }

        public Dominio.Entidades.Embarcador.Cargas.Carga BuscarCargaPorCancelamento(int codigoCancelamento)
        {
            var query = SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.CargaCancelamento>();
            var result = query.Where(obj => obj.Codigo == codigoCancelamento);
            return result.Select(x => x.Carga).FirstOrDefault();
        }

        public int ContarPorCargaProblemaEmissao()
        {
            var query = SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.Carga>();
            var result = query.Where(obj => obj.SituacaoCarga == Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoCarga.PendeciaDocumentos && obj.PossuiPendencia);
            return result.Count();
        }

        public int ContarConsulta(Dominio.ObjetosDeValor.Embarcador.Carga.FiltroPesquisaCancelamentoCarga filtrosPesquisa)
        {
            var consulta = this.SessionNHiBernate.CreateSQLQuery(ObterQueryCargaCancelamento(filtrosPesquisa, true));

            return consulta.SetTimeout(600).UniqueResult<int>();
        }

        public IList<Dominio.ObjetosDeValor.Embarcador.Carga.Cancelamento.CancelamentoCarga> Consultar(Dominio.ObjetosDeValor.Embarcador.Carga.FiltroPesquisaCancelamentoCarga filtrosPesquisa, Dominio.ObjetosDeValor.Embarcador.Consulta.ParametroConsulta parametroConsulta)
        {
            var consulta = this.SessionNHiBernate.CreateSQLQuery(ObterQueryCargaCancelamento(filtrosPesquisa, false, parametroConsulta));

            consulta.SetResultTransformer(new NHibernate.Transform.AliasToBeanResultTransformer(typeof(Dominio.ObjetosDeValor.Embarcador.Carga.Cancelamento.CancelamentoCarga)));

            return consulta.SetTimeout(600).List<Dominio.ObjetosDeValor.Embarcador.Carga.Cancelamento.CancelamentoCarga>();
        }

        public List<int> BuscarCodigosPorSituacao(SituacaoCancelamentoCarga situacao)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.CargaCancelamento>();

            query = query.Where(o => o.Situacao == situacao);

            return query.Select(o => o.Codigo).ToList();
        }

        public List<int> BuscarCodigosPendentesIntegracao()
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.CargaCancelamento>();

            query = query.Where(o => o.Situacao == SituacaoCancelamentoCarga.EmCancelamento && o.GerouIntegracao);

            return query.Select(o => o.Codigo).ToList();
        }

        public List<int> BuscarCodigosPorSituacaoETipo(SituacaoCancelamentoCarga? situacao, TipoCancelamentoCarga? tipo)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.CargaCancelamento>();

            query = query.Where(o => o.Situacao == situacao);

            if (situacao == SituacaoCancelamentoCarga.AgCancelamentoCTe)
                query = query.Where(o => !o.AguardandoXmlDesacordo.HasValue || !o.AguardandoXmlDesacordo.Value);

            //if (situacao == SituacaoCancelamentoCarga.AgCancelamentoMDFe)
            //    query = query.Where(o => !o.AguardandoConfirmacaoCancelamento.HasValue || !o.AguardandoConfirmacaoCancelamento.Value);

            return query.OrderBy("Carga.CargaSVM desc").Select(o => o.Codigo).ToList();
        }

        public List<int> BuscarCodigosReenvioCancelamento(SituacaoCancelamentoCarga? situacao, int tentativas, DateTime dataParametro, int limite)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.CargaCancelamento>();

            query = query.Where(obj => obj.Situacao == situacao
                                       && obj.TentativasEnvioCancelamento < tentativas
                                       //&& DateTime.Now.AddHours(-24) <= obj.DataCancelamento
                                       && obj.DataEnvioCancelamento <= dataParametro
                                       && obj.Carga.CargaCTes.Any(o => o.CTe.Status.Equals("A")
                                       && o.CTe.MensagemStatus.CodigoDoErro == 528));

            return query.Take(limite).Select(o => o.Codigo).ToList();
        }

        public bool VerificarCancelamentoPendenteReenvioAutomatico(SituacaoCancelamentoCarga? situacao, int tentativas, int codigoCancelamento)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.CargaCancelamento>();

            query = query.Where(obj => obj.Situacao == situacao
                                       && obj.Codigo == codigoCancelamento
                                       && obj.TentativasEnvioCancelamento < tentativas
                                       && obj.Carga.CargaCTes.Any(o => o.CTe.Status.Equals("A")
                                       && o.CTe.MensagemStatus.CodigoDoErro == 528));

            return query.Count() > 0;
        }

        public bool VerificarCTeRejeitadoPorMDFe(int codigoCarga)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.CargaCTe>();

            var result = from obj in query where obj.Carga.Codigo == codigoCarga && obj.CTe.Status.Equals("A") && obj.CTe.MensagemStatus.CodigoDoErro == 528 select obj;

            return (result.Count() > 0);
        }

        public List<Dominio.Entidades.Embarcador.Cargas.CargaCancelamento> BuscarParaGerarIntegracao(int limite)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.CargaCancelamento>();
            var subQuery = from o
                           in this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.CargaCancelamentoIntegracao>()
                           where o.CargaCancelamento.Situacao == SituacaoCancelamentoCarga.EmCancelamento
                           select o.CargaCancelamento.Codigo;

            var result = from obj in query
                         where
                            obj.Tipo == TipoCancelamentoCarga.Cancelamento
                            && subQuery.Contains(obj.Codigo)
                         select obj;

            return result.Take(limite).ToList();
        }

        public List<Dominio.Entidades.Embarcador.Cargas.CargaCancelamento> BuscarNaoIntegradasComTransportador(List<int> codigosTransportadores)
        {
            IQueryable<Dominio.Entidades.Embarcador.Cargas.CargaCancelamento> query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.CargaCancelamento>();

            query = query.Where(o => !o.IntegrouTransportador &&
                                     o.Situacao == SituacaoCancelamentoCarga.Cancelada &&
                                     codigosTransportadores.Contains(o.Carga.Empresa.Codigo) &&
                                     (
                                        ((TipoCancelamentoCargaDocumento?)o.TipoCancelamentoCargaDocumento ?? TipoCancelamentoCargaDocumento.Carga) == TipoCancelamentoCargaDocumento.Carga ||
                                        ((TipoCancelamentoCargaDocumento?)o.TipoCancelamentoCargaDocumento ?? TipoCancelamentoCargaDocumento.Carga) == TipoCancelamentoCargaDocumento.TodosDocumentos
                                     )
                                );

            return query.ToList();
        }

        public void SetarSituacaoEMensagem(int codigoCargaCancelamento, Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoCancelamentoCarga situacao, string mensagem)
        {
            string sqlQuery = "UPDATE CargaCancelamento SET Situacao = :situacao, MensagemRejeicaoCancelamento = :mensagem WHERE Codigo = :codigo";

            if (UnitOfWork.IsActiveTransaction())
            {
                UnitOfWork.Sessao.CreateQuery(sqlQuery).SetEnum("situacao", situacao).SetString("mensagem", mensagem).SetInt32("codigo", codigoCargaCancelamento).ExecuteUpdate();
            }
            else
            {
                try
                {
                    UnitOfWork.Start();

                    UnitOfWork.Sessao.CreateQuery(sqlQuery).SetEnum("situacao", situacao).SetString("mensagem", mensagem).SetInt32("codigo", codigoCargaCancelamento).ExecuteUpdate();

                    UnitOfWork.CommitChanges();
                }
                catch
                {
                    UnitOfWork.Rollback();
                    throw;
                }
            }
        }

        public bool ExistePendentePorCarga(int codigoCarga)
        {
            SituacaoCancelamentoCarga[] situacoes = new SituacaoCancelamentoCarga[] {
                SituacaoCancelamentoCarga.SolicitacaoReprovada,
                SituacaoCancelamentoCarga.Reprovada,
                SituacaoCancelamentoCarga.RejeicaoCancelamento
            };

            SituacaoCancelamentoCarga[] situacoesDocumentos = new SituacaoCancelamentoCarga[] {
                SituacaoCancelamentoCarga.Anulada,
                SituacaoCancelamentoCarga.Cancelada
            };

            IQueryable<Dominio.Entidades.Embarcador.Cargas.CargaCancelamento> query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.CargaCancelamento>();

            query = query.Where(o => o.Carga.Codigo == codigoCarga && !situacoes.Contains(o.Situacao) && (o.TipoCancelamentoCargaDocumento != TipoCancelamentoCargaDocumento.Carga && !situacoesDocumentos.Contains(o.Situacao)));

            return query.Select(o => o.Codigo).Any();
        }

        public bool VerificarTipoCancelamentoUnitario(int codigoCancelamento)
        {
            IQueryable<Dominio.Entidades.Embarcador.Cargas.CargaCancelamento> query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.CargaCancelamento>();
            query = query.Where(o => o.Codigo == codigoCancelamento && o.TipoCancelamentoCargaDocumento == TipoCancelamentoCargaDocumento.Documentos);
            return query.Any();
        }

        public bool ExisteCargaPendenteDeCancelamento(int codigoCarga)
        {
            IQueryable<Dominio.Entidades.Embarcador.Cargas.CargaCancelamento> query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.CargaCancelamento>();

            SituacaoCancelamentoCarga[] situacoes = new SituacaoCancelamentoCarga[] {
                SituacaoCancelamentoCarga.Cancelada,
                SituacaoCancelamentoCarga.Anulada,
            };

            query = query.Where(o => o.Carga.Codigo == codigoCarga && !situacoes.Contains(o.Situacao));

            return query.Select(o => o.Codigo).Any();
        }

        public bool ExistePorCargaEData(DateTime dataComparacao, int codigoCarga)
        {
            var query = SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.CargaCancelamento>();
            var result = query.Where(obj => obj.Carga.Codigo == codigoCarga && dataComparacao >= obj.DataCancelamento);
            return result.Any();
        }

        public List<int> BuscarCodigosVerificarCargasEmFinalizacaoCancelamento(int limiteRegistros)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.CargaCancelamento>()
                .Where(o => o.Situacao == Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoCancelamentoCarga.FinalizandoCancelamento);

            return query
                .OrderBy("Carga.CargaSVM desc")
                .Select(o => o.Codigo)
                .Take(limiteRegistros)
                .ToList();
        }

        public bool AtulizarStatusCargaSemIntegracaoCancelamento(int codigo)
        {
            var situacao = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoCancelamentoCarga.AgIntegracaoCancelamentoCIOT;
            string sqlQuery = "UPDATE CargaCancelamento SET Situacao = :situacao WHERE Codigo = :codigo";

            if (UnitOfWork.IsActiveTransaction())
            {
                UnitOfWork.Sessao.CreateQuery(sqlQuery).SetEnum("situacao", situacao).SetInt32("codigo", codigo).ExecuteUpdate();
            }
            else
            {
                try
                {
                    UnitOfWork.Start();

                    UnitOfWork.Sessao.CreateQuery(sqlQuery).SetEnum("situacao", situacao).SetInt32("codigo", codigo).ExecuteUpdate();

                    UnitOfWork.CommitChanges();
                }
                catch
                {
                    UnitOfWork.Rollback();
                    return false;
                }
            }
            return true;
        }

        #endregion

        #region Métodos Privados

        private string ObterQueryCargaCancelamento(Dominio.ObjetosDeValor.Embarcador.Carga.FiltroPesquisaCancelamentoCarga filtrosPesquisa, bool somenteContarNumeroRegistros, Dominio.ObjetosDeValor.Embarcador.Consulta.ParametroConsulta parametroConsulta = null)
        {
            StringBuilder sql = new StringBuilder();

            if (somenteContarNumeroRegistros)
                sql.AppendLine("select distinct(count(0) over ()) ");
            else
                sql = sql.AppendLine(@"select Cancelamento.CAC_CODIGO Codigo,
                                              Cancelamento.CAC_DATA_CANCELAMENTO DataCancelamento,
                                              Carga.CAR_CODIGO_CARGA_EMBARCADOR Carga,
                                              CargaDadosSumarizados.CDS_REMETENTES Remetente, 
                                              CargaDadosSumarizados.CDS_DESTINATARIOS Destinatario,
                                              Cancelamento.CAC_SITUACAO Situacao,
                                              Usuario.FUN_NOME Usuario,
                                              Cancelamento.CAC_REJEICAO_CANCELAMENTO MensagemRejeicaoCancelamento,
                                              PortoOrigem.POT_DESCRICAO PortoOrigem,
                                              PortoDestino.POT_DESCRICAO PortoDestino,
                                              Viagem.PVN_DESCRICAO Viagem,
                                              CargaDadosSumarizados.CDS_ORIGENS Origens,
                                              CargaDadosSumarizados.CDS_DESTINOS Destinos,
                                              Cancelamento.CAC_MOTIVO_CANCELAMENTO MotivoCancelamento, 
                                              Cancelamento.CAC_TIPO_CANCELAMENTO_CARGA_DOCUMENTO TipoCancelamentoCargaDocumento ");

            sql.AppendLine(" from T_CARGA_CANCELAMENTO Cancelamento");

            sql.AppendLine(SetarJoinQueryCargaCancelamento());
            sql.AppendLine(SetarWhereQueryCargaCancelamento(filtrosPesquisa));

            if (!somenteContarNumeroRegistros)
            {
                sql.Append($" order by {parametroConsulta.PropriedadeOrdenar} {parametroConsulta.DirecaoOrdenar}");

                if ((parametroConsulta.InicioRegistros > 0) || (parametroConsulta.LimiteRegistros > 0))
                    sql.Append($" offset {parametroConsulta.InicioRegistros} rows fetch next {parametroConsulta.LimiteRegistros} rows only;");
            }

            return sql.ToString();
        }

        private string SetarJoinQueryCargaCancelamento()
        {
            return @"   join T_CARGA Carga on Cancelamento.CAR_CODIGO = Carga.CAR_CODIGO 
                        left join T_CARGA_DADOS_SUMARIZADOS CargaDadosSumarizados on Carga.CDS_CODIGO = CargaDadosSumarizados.CDS_CODIGO 
                        left join T_PORTO PortoOrigem on Carga.POT_CODIGO_ORIGEM = PortoOrigem.POT_CODIGO 
                        left join T_PORTO PortoDestino on Carga.POT_CODIGO_DESTINO = PortoDestino.POT_CODIGO 
                        left join T_PEDIDO_VIAGEM_NAVIO Viagem on Carga.PVN_CODIGO = Viagem.PVN_CODIGO 
                        left join T_FUNCIONARIO Usuario on Cancelamento.FUN_CODIGO = Usuario.FUN_CODIGO ";
        }

        private string SetarWhereQueryCargaCancelamento(Dominio.ObjetosDeValor.Embarcador.Carga.FiltroPesquisaCancelamentoCarga filtrosPesquisa)
        {
            StringBuilder where = new StringBuilder();

            where.Append(" where 1 = 1 ");

            if (!string.IsNullOrWhiteSpace(filtrosPesquisa.NumeroCarga))
            {
                where.AppendLine($@"
                                    and (
                                        Carga.CAR_CODIGO_CARGA_EMBARCADOR = '{filtrosPesquisa.NumeroCarga}' or exists (
                                            select cargaCodigosAgrupados.CAR_CODIGO_CARGA_AGRUPADO
                                              from T_CARGA_CODIGOS_AGRUPADOS cargaCodigosAgrupados
                                             where Carga.CAR_CODIGO = cargaCodigosAgrupados.CAR_CODIGO
                                               and cargaCodigosAgrupados.CAR_CODIGO_CARGA_AGRUPADO = '{filtrosPesquisa.NumeroCarga}'
                                        )
                                    )");
            }

            if (filtrosPesquisa.TiposPropostasMultimodal?.Count > 0)
            {
                where.AppendLine(" and exists ( ");
                where.AppendLine("         select top (1) 1 ");
                where.AppendLine("           from T_CARGA_PEDIDO CargaPedido");
                where.AppendLine("           join T_PEDIDO Pedido on Pedido.PED_CODIGO = CargaPedido.PED_CODIGO");
                where.AppendLine("          where CargaPedido.CAR_CODIGO = Carga.CAR_CODIGO");
                where.AppendLine($"           and Pedido.PED_TIPO_PROPOSTA_MULTIMODAL in ({string.Join(", ", filtrosPesquisa.TiposPropostasMultimodal.Select(o => (int)o))})");
                where.AppendLine("     ) ");
            }

            if (!string.IsNullOrWhiteSpace(filtrosPesquisa.NumeroOS))
            {
                where.AppendLine(" and exists ( ");
                where.AppendLine("         select top (1) 1 ");
                where.AppendLine("           from T_CARGA_PEDIDO CargaPedido");
                where.AppendLine("           join T_PEDIDO Pedido on Pedido.PED_CODIGO = CargaPedido.PED_CODIGO");
                where.AppendLine("          where CargaPedido.CAR_CODIGO = Carga.CAR_CODIGO");
                where.AppendLine($"           and Pedido.PED_NUMERO_OS = '{filtrosPesquisa.NumeroOS}'");
                where.AppendLine("     ) ");
            }

            if (!string.IsNullOrWhiteSpace(filtrosPesquisa.NumeroBooking))
            {
                where.AppendLine(" and exists ( ");
                where.AppendLine("         select top (1) 1 ");
                where.AppendLine("           from T_CARGA_PEDIDO CargaPedido");
                where.AppendLine("           join T_PEDIDO Pedido on Pedido.PED_CODIGO = CargaPedido.PED_CODIGO");
                where.AppendLine("          where CargaPedido.CAR_CODIGO = Carga.CAR_CODIGO");
                where.AppendLine($"           and Pedido.PED_NUMERO_BOOKING = '{filtrosPesquisa.NumeroBooking}'");
                where.AppendLine("     ) ");
            }

            if (filtrosPesquisa.CpfCnpjPessoa > 0D)
            {
                where.AppendLine(" and exists ( ");
                where.AppendLine("         select top (1) 1 ");
                where.AppendLine("           from T_CARGA_PEDIDO CargaPedido");
                where.AppendLine("           join T_PEDIDO Pedido on Pedido.PED_CODIGO = CargaPedido.PED_CODIGO");
                where.AppendLine("          where CargaPedido.CAR_CODIGO = Carga.CAR_CODIGO");
                where.AppendLine($"           and (Pedido.CLI_CODIGO_REMETENTE = {filtrosPesquisa.CpfCnpjPessoa} or Pedido.CLI_CODIGO = {filtrosPesquisa.CpfCnpjPessoa})");
                where.AppendLine("     ) ");
            }

            if (filtrosPesquisa.CodigosRecebedores?.Count > 0)
            {
                where.AppendLine(" and exists ( ");
                where.AppendLine("         select top (1) 1 ");
                where.AppendLine("           from T_CARGA_PEDIDO CargaPedido");
                where.AppendLine("          where CargaPedido.CAR_CODIGO = Carga.CAR_CODIGO");
                where.AppendLine($"           and (CargaPedido.CLI_CODIGO_RECEBEDOR in ({string.Join(", ", filtrosPesquisa.CodigosRecebedores)}) or CargaPedido.CLI_CODIGO_RECEBEDOR is null)");
                where.AppendLine("     ) ");
            }

            if (filtrosPesquisa.CodigoGrupoPessoas > 0)
            {
                where.AppendLine(" and exists ( ");
                where.AppendLine("         select top (1) 1 ");
                where.AppendLine("           from T_CARGA_PEDIDO CargaPedido");
                where.AppendLine("           join T_PEDIDO Pedido on Pedido.PED_CODIGO = CargaPedido.PED_CODIGO");
                where.AppendLine("           join T_CLIENTE Remetente on Remetente.CLI_CGCCPF = Pedido.CLI_CODIGO_REMETENTE");
                where.AppendLine("           join T_CLIENTE Destinatario on Destinatario.CLI_CGCCPF = Pedido.CLI_CODIGO");
                where.AppendLine("          where CargaPedido.CAR_CODIGO = Carga.CAR_CODIGO");
                where.AppendLine($"           and (Remetente.GRP_CODIGO = {filtrosPesquisa.CodigoGrupoPessoas} or Destinatario.GRP_CODIGO = {filtrosPesquisa.CodigoGrupoPessoas})");
                where.AppendLine("     ) ");
            }

            if ((filtrosPesquisa.CodigosFilial?.Any(codigo => codigo == -1) ?? false) && (filtrosPesquisa.CodigosRecebedores?.Count > 0))
            {
                where.AppendLine(" and ( ");
                where.AppendLine($"        Carga.FIL_CODIGO in ({string.Join(", ", filtrosPesquisa.CodigosFilial)})");

                where.AppendLine("            or exists ( ");
                where.AppendLine("            select top (1) 1");
                where.AppendLine("              from T_CARGA_PEDIDO CargaPedido");
                where.AppendLine("             where CargaPedido.CAR_CODIGO = Carga.CAR_CODIGO");
                where.AppendLine($"              and CargaPedido.CLI_CODIGO_RECEBEDOR in ({string.Join(", ", filtrosPesquisa.CodigosRecebedores)})");
                where.AppendLine("     ) ");

                where.AppendLine(" ) ");
            }
            else if (filtrosPesquisa.CodigosFilial?.Count > 0)
                where.AppendLine($" and Carga.FIL_CODIGO in ({string.Join(", ", filtrosPesquisa.CodigosFilial)})");

            if (filtrosPesquisa.NumeroCTe > 0)
            {
                where.AppendLine(" and exists ( ");
                where.AppendLine("          select top (1) 1 ");
                where.AppendLine("            from T_CARGA_CTE CargaCTe");
                where.AppendLine("            join T_CTE CTe on CTe.CON_CODIGO = CargaCTe.CON_CODIGO");
                where.AppendLine("           where CargaCTe.CAR_CODIGO = Carga.CAR_CODIGO");
                where.AppendLine($"            and CTe.CON_NUM = {filtrosPesquisa.NumeroCTe}");
                where.AppendLine("       ) ");
            }

            if (filtrosPesquisa.CodigoPedidoViagemDirecao > 0)
            {
                where.AppendLine(" and exists ( ");
                where.AppendLine("          select top (1) 1 ");
                where.AppendLine("            from T_CARGA_CTE CargaCTe");
                where.AppendLine("            join T_CTE CTe on CTe.CON_CODIGO = CargaCTe.CON_CODIGO");
                where.AppendLine("           where CargaCTe.CAR_CODIGO = Carga.CAR_CODIGO");
                where.AppendLine($"            and CTe.CON_VIAGEM = {filtrosPesquisa.CodigoPedidoViagemDirecao}");
                where.AppendLine("       ) ");
            }

            if (filtrosPesquisa.CodigoTerminalOrigem > 0)
            {
                where.AppendLine(" and exists ( ");
                where.AppendLine("          select top (1) 1 ");
                where.AppendLine("            from T_CARGA_CTE CargaCTe");
                where.AppendLine("            join T_CTE CTe on CTe.CON_CODIGO = CargaCTe.CON_CODIGO");
                where.AppendLine("           where CargaCTe.CAR_CODIGO = Carga.CAR_CODIGO");
                where.AppendLine($"            and CTe.CON_TERMINAL_ORIGEM = {filtrosPesquisa.CodigoTerminalOrigem}");
                where.AppendLine("       ) ");
            }

            if (filtrosPesquisa.CodigoTerminalDestino > 0)
            {
                where.AppendLine(" and exists ( ");
                where.AppendLine("          select top (1) 1 ");
                where.AppendLine("            from T_CARGA_CTE CargaCTe");
                where.AppendLine("            join T_CTE CTe on CTe.CON_CODIGO = CargaCTe.CON_CODIGO");
                where.AppendLine("           where CargaCTe.CAR_CODIGO = Carga.CAR_CODIGO");
                where.AppendLine($"            and CTe.CON_TERMINAL_DESTINO = {filtrosPesquisa.CodigoTerminalDestino}");
                where.AppendLine("       ) ");
            }

            if (filtrosPesquisa.CodigoOrigem > 0)
            {
                where.AppendLine(" and exists ( ");
                where.AppendLine("          select top (1) 1 ");
                where.AppendLine("            from T_CARGA_CTE CargaCTe");
                where.AppendLine("            join T_CTE CTe on CTe.CON_CODIGO = CargaCTe.CON_CODIGO");
                where.AppendLine("           where CargaCTe.CAR_CODIGO = Carga.CAR_CODIGO");
                where.AppendLine($"            and CTe.CON_LOCINICIOPRESTACAO = {filtrosPesquisa.CodigoOrigem}");
                where.AppendLine("       ) ");
            }

            if (filtrosPesquisa.CodigoDestino > 0)
            {
                where.AppendLine(" and exists ( ");
                where.AppendLine("          select top (1) 1 ");
                where.AppendLine("            from T_CARGA_CTE CargaCTe");
                where.AppendLine("            join T_CTE CTe on CTe.CON_CODIGO = CargaCTe.CON_CODIGO");
                where.AppendLine("           where CargaCTe.CAR_CODIGO = Carga.CAR_CODIGO");
                where.AppendLine($"            and CTe.CON_LOCTERMINOPRESTACAO = {filtrosPesquisa.CodigoDestino}");
                where.AppendLine("       ) ");
            }

            if (filtrosPesquisa.TipoCancelamentoCargaDocumento.HasValue)
                where.AppendLine($" and isnull(Cancelamento.CAC_TIPO_CANCELAMENTO_CARGA_DOCUMENTO, {(int)TipoCancelamentoCargaDocumento.Carga}) = {(int)filtrosPesquisa.TipoCancelamentoCargaDocumento.Value}");

            if (filtrosPesquisa.CodigosTipoCarga?.Count > 0)
                where.AppendLine($" and Carga.TCG_CODIGO in ({string.Join(", ", filtrosPesquisa.CodigosTipoCarga)})");

            if (filtrosPesquisa.CodigosTipoOperacao?.Count > 0)
                where.AppendLine($" and Carga.TOP_CODIGO in ({string.Join(", ", filtrosPesquisa.CodigosTipoOperacao)})");

            if (filtrosPesquisa.CodigoOperador > 0)
                where.AppendLine($" and Usuario.FUN_CODIGO = {filtrosPesquisa.CodigoOperador}");

            if (filtrosPesquisa.DataInicial != DateTime.MinValue)
                where.AppendLine($" and Cancelamento.CAC_DATA_CANCELAMENTO >= '{filtrosPesquisa.DataInicial.ToString("yyyy-MM-dd")}'");

            if (filtrosPesquisa.DataFinal != DateTime.MinValue)
                where.AppendLine($" and Cancelamento.CAC_DATA_CANCELAMENTO < '{filtrosPesquisa.DataFinal.ToString("yyyy-MM-dd")}'");

            if (filtrosPesquisa.Situacao.HasValue)
                where.AppendLine($" and Cancelamento.CAC_SITUACAO = {(int)filtrosPesquisa.Situacao.Value}");

            if (filtrosPesquisa.Tipo.HasValue)
                where.AppendLine($" and Cancelamento.CAC_TIPO = {(int)filtrosPesquisa.Tipo.Value}");

            if (filtrosPesquisa.CodigosEmpresas?.Count > 0)
                where.AppendLine($" and Carga.EMP_CODIGO in ({string.Join(",", filtrosPesquisa.CodigosEmpresas)})");

            if (filtrosPesquisa.CargasLiberadasParaCancelamentoComRejeicaoIntegracao)
                where.AppendLine(" and Cancelamento.CAC_LIBERAR_CANCELAMENTO_COM_INTEGRACAO_REJEITADA = 1");

            return where.ToString();
        }

        private IQueryable<Dominio.Entidades.Embarcador.Cargas.CargaCancelamento> ObterQueryBuscarPorCarga(int codigoCarga)
        {
            var query = SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.CargaCancelamento>();

            return query.Where(obj => obj.Carga.Codigo == codigoCarga && ((TipoCancelamentoCargaDocumento?)obj.TipoCancelamentoCargaDocumento ?? TipoCancelamentoCargaDocumento.Carga) == TipoCancelamentoCargaDocumento.Carga);
        }

        #endregion
    }
}
