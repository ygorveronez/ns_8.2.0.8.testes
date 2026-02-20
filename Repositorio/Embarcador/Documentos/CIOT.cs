using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Dynamic.Core;
using Dominio.ObjetosDeValor.Embarcador.Enumeradores;

namespace Repositorio.Embarcador.Documentos
{
    public class CIOT : RepositorioBase<Dominio.Entidades.Embarcador.Documentos.CIOT>
    {
        public CIOT(UnitOfWork unitOfWork) : base(unitOfWork) { }

        #region Métodos Públicos

        public Dominio.Entidades.Embarcador.Documentos.CIOT BuscarPorCodigo(int codigo)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Documentos.CIOT>();

            query = query.Where(o => o.Codigo == codigo);

            return query.FirstOrDefault();
        }

        public Dominio.Entidades.Embarcador.Documentos.CIOT BuscarPorCodigoArquivo(int codigoArquivo)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Documentos.CIOT>();

            var result = from obj in query where obj.ArquivosTransacao.Any(o => o.Codigo == codigoArquivo) select obj;

            return result.FirstOrDefault();
        }

        public List<Dominio.Entidades.Embarcador.Documentos.CIOT> Consultar(Dominio.ObjetosDeValor.Embarcador.Documentos.FiltroPesquisaCIOT filtrosPesquisa, string propOrdenar, string dirOrdena, int inicio, int limite)
        {
            var result = Consultar(filtrosPesquisa);

            return result.OrderBy(propOrdenar + " " + dirOrdena).Skip(inicio).Take(limite).ToList();
        }

        public int ContarConsulta(Dominio.ObjetosDeValor.Embarcador.Documentos.FiltroPesquisaCIOT filtrosPesquisa)
        {
            var result = Consultar(filtrosPesquisa);

            return result.Count();
        }

        public Dominio.Entidades.Embarcador.Documentos.CIOT BuscarPorTransportadorESituacao(double cpfCnpjTransportador, Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoCIOT situacao)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Documentos.CIOT>();

            query = query.Where(o => o.Transportador.CPF_CNPJ == cpfCnpjTransportador && o.Situacao == situacao);

            return query.FirstOrDefault();
        }

        public Dominio.Entidades.Embarcador.Documentos.CIOT BuscarPorTransportadorESituacao(double cpfCnpjTransportador, List<Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoCIOT> situacoes, int codigoCarga)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Documentos.CIOT>();

            var result = from obj in query
                         where
                         (!obj.CargaCIOT.Any() || obj.CargaCIOT.Any(o => o.Carga.Codigo != codigoCarga)) &&
                         obj.Transportador.CPF_CNPJ == cpfCnpjTransportador &&
                         situacoes.Contains(obj.Situacao)
                         orderby obj.DataFinalViagem descending
                         select obj;

            return result.FirstOrDefault();
        }

        public Dominio.Entidades.Embarcador.Documentos.CIOT Buscar(double cpfCnpjTransportador, int codigoMotorista, int codigoVeiculo, List<Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoCIOT> situacoes, int codigoCarga)
        {
            IQueryable<Dominio.Entidades.Embarcador.Documentos.CIOT> query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Documentos.CIOT>();

            query = query.Where(o => o.Transportador.CPF_CNPJ == cpfCnpjTransportador &&
                                     (o.Veiculo == null || o.Veiculo.Codigo == codigoVeiculo) &&
                                     (o.Motorista == null || o.Motorista.Codigo == codigoMotorista) &&
                                     situacoes.Contains(o.Situacao) &&
                                     (!o.CargaCIOT.Any() || o.CargaCIOT.Any(c => c.Carga.Codigo != codigoCarga && !c.Carga.LiberadoComProblemaCIOT)));

            return query.OrderByDescending(o => o.DataFinalViagem).FirstOrDefault();
        }

        public Dominio.Entidades.Embarcador.Documentos.CIOT Buscar(double cpfCnpjTransportador, int codigoVeiculo, List<Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoCIOT> situacoes, int codigoCarga, bool validarDataFechamento = false)
        {
            IQueryable<Dominio.Entidades.Embarcador.Documentos.CIOT> query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Documentos.CIOT>();

            query = query.Where(o => o.Transportador.CPF_CNPJ == cpfCnpjTransportador &&
                                     (o.Veiculo == null || o.Veiculo.Codigo == codigoVeiculo) &&
                                     situacoes.Contains(o.Situacao) &&
                                     (!o.CargaCIOT.Any() || o.CargaCIOT.Any(c => c.Carga.Codigo != codigoCarga && !c.Carga.LiberadoComProblemaCIOT)));

            if (validarDataFechamento)
                query = query.Where(o => !o.DataParaFechamento.HasValue || o.DataParaFechamento.Value.Date >= DateTime.Today);

            return query.OrderByDescending(o => o.DataFinalViagem).FirstOrDefault();
        }

        public Dominio.Entidades.Embarcador.Documentos.CIOT BuscarPorNumeroECodigoVerificador(string CIOT, string CodigoVerificador)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Documentos.CIOT>();

            var result = from o in query where o.Numero == CIOT && o.CodigoVerificador == CodigoVerificador select o;

            return result.FirstOrDefault();
        }

        public bool ExisteCIOTAbertoComTarifaJaInclusa(double cpfCnpjTerceiro)
        {
            IQueryable<Dominio.Entidades.Embarcador.Documentos.CIOT> query = SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Documentos.CIOT>();

            query = query.Where(o => o.Transportador.CPF_CNPJ == cpfCnpjTerceiro && (o.Situacao == Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoCIOT.Aberto || o.Situacao == Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoCIOT.AgLiberarViagem) && o.CargaCIOT.Any(c => c.ContratoFrete.TarifaTransferencia > 0m || c.ContratoFrete.TarifaSaque > 0m));

            return query.Any();
        }

        public Dominio.Entidades.Embarcador.Documentos.CIOT BuscarPorCarga(int codigoCarga)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Documentos.CIOT>();

            var result = from obj in query where (obj.CargaCIOT.Any(o => o.Carga.Codigo == codigoCarga)) select obj;

            return result.FirstOrDefault();
        }

        public Dominio.Entidades.Embarcador.Documentos.CIOT BuscarPorCargaCIOT(int codigoCarga, string ciot)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Documentos.CIOT>();

            var result = from obj in query where (obj.CargaCIOT.Any(o => o.Carga.Codigo == codigoCarga && o.CIOT.Numero == ciot)) select obj;

            return result.FirstOrDefault();
        }

        public List<Dominio.Entidades.Embarcador.Documentos.CIOT> BuscarPorCargaEStatus(int codigoCarga, Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoCIOT situacao)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Documentos.CIOT>();

            var result = from obj in query where (obj.CargaCIOT.Any(o => o.Carga.Codigo == codigoCarga) && obj.Situacao == situacao) select obj;

            return result.ToList();
        }

        public Dominio.Entidades.Embarcador.Documentos.CIOT BuscarCIOTEmCancelamentoPorCarga(int codigoCargaCancelamento)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Documentos.CIOT>()
                .Where(o => o.CargaCIOT.Any(cc => cc.Carga.Codigo == codigoCargaCancelamento));

            return query.FirstOrDefault();
        }

        public List<Dominio.Entidades.Empresa> BuscarEmpresasComCIOTAberto()
        {
            IQueryable<Dominio.Entidades.Embarcador.Documentos.CIOT> query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Documentos.CIOT>();

            query = query.Where(o => (o.Situacao == Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoCIOT.Aberto || o.Situacao == Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoCIOT.AgLiberarViagem) && o.Contratante != null);

            return query.Select(o => o.Contratante).Distinct().ToList();
        }

        public IList<Dominio.ObjetosDeValor.CIOT.RetornoCanhotosCIOT> BuscarCanhotosRecebidosFisicamenteEDigitalizados(List<int> codigosCIOTs)
        {
            string sqlQuery = @"select 
                                    canhoto.CNF_NUMERO Canhoto,
                                    carga.CAR_CODIGO_CARGA_EMBARCADOR Carga
                                from T_CIOT ciot
                                    left join T_CARGA_CIOT cargaCiot on cargaCiot.CIO_CODIGO = ciot.CIO_CODIGO
                                    left join T_CARGA carga on carga.CAR_CODIGO = cargaCiot.CAR_CODIGO
                                    left join T_CANHOTO_NOTA_FISCAL canhoto on canhoto.CAR_CODIGO = carga.CAR_CODIGO
                                    left join T_XML_NOTA_FISCAL AS XMLNotaFiscal on XMLNotaFiscal.NFX_CODIGO = canhoto.NFX_CODIGO
                                    left join T_CTE_TERCEIRO AS CTeTerceiro on CTeTerceiro.CPS_CODIGO = canhoto.CPS_CODIGO
                                    left join T_CANHOTO_AVULSO AS CanhotoAvulso on CanhotoAvulso.CAV_CODIGO = canhoto.CAV_CODIGO
                                    left join T_CARGA_CTE AS CargaCTe on CargaCTe.CCT_CODIGO = canhoto.CCT_CODIGO
                                    left join T_CTE AS CTeCargaCTeCanhoto on CTeCargaCTeCanhoto.CON_CODIGO = CargaCTe.CON_CODIGO
                                where ciot.CIO_CODIGO in (:codigosCIOTs)
                                    and (canhoto.CNF_SITUACAO_CANHOTO <> 3 or canhoto.CNF_SITUACAO_DIGITALIZACAO_CANHOTO <> 3)
                                    and ((canhoto.CNF_TIPO_CANHOTO = 3 and CTeTerceiro.CPS_ATIVO = 1) 
                                        or (canhoto.CNF_TIPO_CANHOTO = 1 and XMLNotaFiscal.NF_ATIVA = 1) 
                                        or (canhoto.CNF_TIPO_CANHOTO = 2 and CanhotoAvulso.CAV_ATIVO = 1) 
                                        or (canhoto.CNF_TIPO_CANHOTO = 4 and CTeCargaCTeCanhoto.CON_STATUS = 'A')) ";

            var query = this.SessionNHiBernate.CreateSQLQuery(sqlQuery);
            query.SetParameterList("codigosCIOTs", codigosCIOTs);
            query.SetResultTransformer(new NHibernate.Transform.AliasToBeanResultTransformer(typeof(Dominio.ObjetosDeValor.CIOT.RetornoCanhotosCIOT)));
            IList<Dominio.ObjetosDeValor.CIOT.RetornoCanhotosCIOT> retorno = query.List<Dominio.ObjetosDeValor.CIOT.RetornoCanhotosCIOT>();

            return retorno;
        }

        public Dominio.Entidades.Embarcador.Documentos.CIOT BuscarPorProtocoloAutorizacao(Dominio.ObjetosDeValor.Embarcador.Enumeradores.OperadoraCIOT operadora, string protocoloAutorizacao)
        {
            IQueryable<Dominio.Entidades.Embarcador.Documentos.CIOT> query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Documentos.CIOT>();

            query = query.Where(o => o.ProtocoloAutorizacao == protocoloAutorizacao && o.Operadora == operadora);

            return query.FirstOrDefault();
        }

        public decimal BuscarValorTotalReceberPorCIOTeCarga(int codigoCIOT, int codigoCarga, string statusCTe)
        {
            string sql;

            sql = " SELECT ";
            sql += "   SUM(Cte.CON_VALOR_RECEBER) AS ValorAReceber ";
            sql += " FROM T_CARGA Carga ";
            sql += " inner join T_CARGA_CIOT CargaCiot ON CargaCiot.CAR_CODIGO = Carga.CAR_CODIGO ";
            sql += " inner join T_CARGA_CTE CargaCte ON CargaCte.CAR_CODIGO = Carga.CAR_CODIGO ";
            sql += " inner join T_CTE Cte ON Cte.CON_CODIGO = CargaCte.CON_CODIGO ";
            sql += $" WHERE CargaCiot.CIO_CODIGO = {codigoCIOT} ";

            if (codigoCarga != 0)
                sql += $" AND Carga.CAR_CODIGO = {codigoCarga} ";

                sql += $" AND Cte.CON_STATUS = 'A' ";

            sql += " AND CargaCte.CCC_CODIGO IS NULL ";
            sql += " AND Cte.CON_TIPO_CTE != 2 ";

            var consultaValorAReceber = this.SessionNHiBernate.CreateSQLQuery(sql);

            return consultaValorAReceber.SetTimeout(600).UniqueResult<decimal>();
        }

        #endregion

        #region Métodos Privados

        private IQueryable<Dominio.Entidades.Embarcador.Documentos.CIOT> Consultar(Dominio.ObjetosDeValor.Embarcador.Documentos.FiltroPesquisaCIOT filtrosPesquisa)
        {
            IQueryable<Dominio.Entidades.Embarcador.Documentos.CIOT> query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Documentos.CIOT>();

            if (!string.IsNullOrWhiteSpace(filtrosPesquisa.NumeroCarga))
                query = query.Where( o => o.CargaCIOT.Any(cargaCIOT => cargaCIOT.Carga.CodigoCargaEmbarcador == filtrosPesquisa.NumeroCarga || cargaCIOT.Carga.CodigosAgrupados.Contains(filtrosPesquisa.NumeroCarga)) );

            if (filtrosPesquisa.CpfCnpjTransportador > 0d)
                query = query.Where(o => o.Transportador.CPF_CNPJ == filtrosPesquisa.CpfCnpjTransportador);

            else if (filtrosPesquisa.TiposTransportador?.Count > 0)
                query = query.Where(o => o.Transportador.Modalidades.Where(x => x.TipoModalidade == TipoModalidade.TransportadorTerceiro).Any(p => p.ModalidadesTransportadora.Any(t => filtrosPesquisa.TiposTransportador.Contains(t.TipoTransportador))));

            if (filtrosPesquisa.DataInicial != DateTime.MinValue)
                query = query.Where(o => o.DataFinalViagem.Date >= filtrosPesquisa.DataInicial.Date);

            if (filtrosPesquisa.DataFinal != DateTime.MinValue)
                query = query.Where(o => o.DataFinalViagem.Date <= filtrosPesquisa.DataFinal.Date);

            if (filtrosPesquisa.Situacao.HasValue)
                query = query.Where(o => o.Situacao == filtrosPesquisa.Situacao);
            else if (filtrosPesquisa.Situacoes?.Count > 0)
                query = query.Where(o => filtrosPesquisa.Situacoes.Contains(o.Situacao));

            if (filtrosPesquisa.CodigoMotorista > 0)
                query = query.Where(o => o.Motorista.Codigo == filtrosPesquisa.CodigoMotorista);

            if (filtrosPesquisa.CodigoVeiculo > 0)
                query = query.Where(o => o.Veiculo.Codigo == filtrosPesquisa.CodigoVeiculo);

            if (filtrosPesquisa.Operadora > 0)
                query = query.Where(o => o.Operadora == filtrosPesquisa.Operadora);

            if (!string.IsNullOrWhiteSpace(filtrosPesquisa.CodigoVerificador))
                query = query.Where(o => o.CodigoVerificador.Contains(filtrosPesquisa.CodigoVerificador));

            if (!string.IsNullOrWhiteSpace(filtrosPesquisa.Numero))
                query = query.Where(o => o.Numero == filtrosPesquisa.Numero);

            if (filtrosPesquisa.CodigosFiliais != null && filtrosPesquisa.CodigosFiliais.Count > 0)
            {
                query = query.Where(o => o.CargaCIOT.Any(a => filtrosPesquisa.CodigosFiliais.Contains(a.Carga.Filial.Codigo)));
            }

            if (filtrosPesquisa.CodigosCIOTs != null && filtrosPesquisa.CodigosCIOTs.Count > 0)
            {
                query = query.Where(o => filtrosPesquisa.CodigosCIOTs.Contains(o.Codigo));
            }

            return query;
        }

        #endregion

        #region Consulta

        public List<Dominio.Entidades.Embarcador.Documentos.CIOT> Consultar(Dominio.ObjetosDeValor.Embarcador.CIOT.FiltroPesquisaCIOT filtros, Dominio.ObjetosDeValor.Embarcador.Consulta.ParametroConsulta parametrosConsulta)
        {
            IQueryable<Dominio.Entidades.Embarcador.Documentos.CIOT> query = ObterConsulta(filtros, parametrosConsulta);

            return query.ToList();
        }

        public int ContarConsulta(Dominio.ObjetosDeValor.Embarcador.CIOT.FiltroPesquisaCIOT filtros)
        {
            IQueryable<Dominio.Entidades.Embarcador.Documentos.CIOT> query = ObterConsulta(filtros);

            return query.Count();
        }

        public List<int> ObterCodigosConsulta(Dominio.ObjetosDeValor.Embarcador.CIOT.FiltroPesquisaCIOT filtros)
        {
            IQueryable<Dominio.Entidades.Embarcador.Documentos.CIOT> query = ObterConsulta(filtros);

            return query.Select(o => o.Codigo).Distinct().ToList();
        }

        private IQueryable<Dominio.Entidades.Embarcador.Documentos.CIOT> ObterConsulta(Dominio.ObjetosDeValor.Embarcador.CIOT.FiltroPesquisaCIOT filtros, Dominio.ObjetosDeValor.Embarcador.Consulta.ParametroConsulta parametrosConsulta = null)
        {
            IQueryable<Dominio.Entidades.Embarcador.Documentos.CIOT> query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Documentos.CIOT>();
            IQueryable<Dominio.Entidades.Embarcador.Cargas.CargaCIOT> queryCargaCIOT = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.CargaCIOT>();

            if (!string.IsNullOrWhiteSpace(filtros.NumeroPedidoEmbarcador))
                query = query.Where(o => o.CargaCIOT.Any(cargaCIOT => cargaCIOT.Carga.Pedidos.Any(cargaPedido => cargaPedido.Pedido.NumeroPedidoEmbarcador == filtros.NumeroPedidoEmbarcador)));

            if (filtros.CodigoContratoTerceiro > 0)
            {
                queryCargaCIOT = queryCargaCIOT.Where(c => c.ContratoFrete.Codigo == filtros.CodigoContratoTerceiro);
                query = query.Where(c => queryCargaCIOT.Any(t => t.CIOT == c));
            }

            if (!string.IsNullOrWhiteSpace(filtros.NumeroCarga))
                query = query.Where(o => o.CargaCIOT.Any(cargaCIOT => cargaCIOT.Carga.CodigoCargaEmbarcador == filtros.NumeroCarga));

            if (filtros.CPFCNPJTransportador > 0d)
                query = query.Where(o => o.Transportador.CPF_CNPJ == filtros.CPFCNPJTransportador);

            if (filtros.DataInicial.HasValue)
                query = query.Where(o => o.DataFinalViagem >= filtros.DataInicial.Value.Date);

            if (filtros.DataFinal.HasValue)
                query = query.Where(o => o.DataFinalViagem < filtros.DataFinal.Value.AddDays(1).Date);

            if (filtros.Situacao.HasValue)
                query = query.Where(o => o.Situacao == filtros.Situacao);

            if (filtros.ListaCodigosCIOT != null && filtros.ListaCodigosCIOT.Count > 0)
            {
                if (filtros.SelecionarTodos == false)
                    query = query.Where(o => filtros.ListaCodigosCIOT.Contains(o.Codigo));
                else
                    query = query.Where(o => !filtros.ListaCodigosCIOT.Contains(o.Codigo));
            }

            if (filtros.TipoAutorizacaoPagamentoCIOTParcela.HasValue)
            {
                switch (filtros.TipoAutorizacaoPagamentoCIOTParcela.Value)
                {
                    case Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoAutorizacaoPagamentoCIOTParcela.Adiantamento:
                        query = query.Where(o => !o.AdiantamentoPago);
                        break;
                    case Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoAutorizacaoPagamentoCIOTParcela.Abastecimento:
                        query = query.Where(o => !o.AbastecimentoPago);
                        break;
                    case Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoAutorizacaoPagamentoCIOTParcela.Saldo:
                        query = query.Where(o => !o.SaldoPago);
                        break;
                    default:
                        break;
                }
            }

            query = query.Where(o => o.Situacao != Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoCIOT.AgIntegracao);

            if (parametrosConsulta != null)
            {
                query = query.OrderBy(parametrosConsulta.PropriedadeOrdenar + " " + parametrosConsulta.DirecaoOrdenar);

                if (parametrosConsulta.InicioRegistros > 0 || parametrosConsulta.LimiteRegistros > 0)
                    query = query.Skip(parametrosConsulta.InicioRegistros).Take(parametrosConsulta.LimiteRegistros);
            }

            return query;
        }

        #endregion
    }
}
