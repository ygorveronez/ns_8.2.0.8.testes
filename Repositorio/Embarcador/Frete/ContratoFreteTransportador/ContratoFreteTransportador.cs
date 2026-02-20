using NHibernate.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Dynamic.Core;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Repositorio.Embarcador.Frete
{
    public class ContratoFreteTransportador : RepositorioBase<Dominio.Entidades.Embarcador.Frete.ContratoFreteTransportador>
    {
        public ContratoFreteTransportador(UnitOfWork unitOfWork) : base(unitOfWork) { }
        public ContratoFreteTransportador(UnitOfWork unitOfWork, CancellationToken cancellationToken) : base(unitOfWork, cancellationToken) { }

        #region Métodos Públicos

        public int ContarPorNumero(string numero)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Frete.ContratoFreteTransportador>();

            var result = from obj in query where obj.NumeroEmbarcador == numero && obj.Ativo && obj.Situacao != Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoContratoFreteTransportador.Rejeitado select obj;

            return result.Count();
        }

        public List<Dominio.Entidades.Embarcador.Frete.ContratoFreteTransportador> BuscarContratoFranquia(int codigoTransportador, string descricao, DateTime? periodoInicio, DateTime? periodoFim)
        {
            var consultaContratoFreteTransportador = ConsultarContratoFranquia(codigoTransportador, descricao, periodoInicio, periodoFim);

            return consultaContratoFreteTransportador.ToList();
        }

        public int ContarBuscaContratoFranquia(int codigoTransportador, string descricao, DateTime? periodoInicio, DateTime? periodoFim)
        {
            var consultaContratoFreteTransportador = ConsultarContratoFranquia(codigoTransportador, descricao, periodoInicio, periodoFim);

            return consultaContratoFreteTransportador.Count();
        }

        public Dominio.Entidades.Embarcador.Frete.ContratoFreteTransportador BuscarPorCodigo(int codigo)
        {
            var consultaContratoFreteTransportador = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Frete.ContratoFreteTransportador>()
                .Where(contrato => contrato.Codigo == codigo);

            return consultaContratoFreteTransportador.FirstOrDefault();
        }

        public List<Dominio.Entidades.Embarcador.Frete.ContratoFreteTransportador> BuscarPorCodigos(List<int> codigos)
        {
            int take = 1000;
            int start = 0;

            List<Dominio.Entidades.Embarcador.Frete.ContratoFreteTransportador> retorno = new List<Dominio.Entidades.Embarcador.Frete.ContratoFreteTransportador>();
            while (start < codigos?.Count)
            {
                List<int> tmp2 = codigos.Skip(start).Take(take).ToList();

                var consultaContrato = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Frete.ContratoFreteTransportador>()
                                    .Where(contrato => codigos.Contains(contrato.Codigo));

                retorno.AddRange(consultaContrato.ToList());
                start += take;
            }

            return retorno;

        }

        public Dominio.Entidades.Embarcador.Frete.ContratoFreteTransportador BuscarPorEmpresaETipoContrato(int empresa, int tipoContrato)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Frete.ContratoFreteTransportador>();

            var result = from obj in query
                         where
                         obj.Transportador.Codigo == empresa
                         && obj.TipoContratoFrete.Codigo == tipoContrato
                         select obj;

            return result.FirstOrDefault();
        }

        public List<int> BuscarContratosVencendo(DateTime prazo)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Frete.ContratoFreteTransportador>();

            var result = from obj in query
                         where obj.DataFinal.Date == prazo
                         select obj.Codigo;

            return result.ToList();
        }

        public Dominio.Entidades.Embarcador.Frete.ContratoFreteTransportador BuscarContratosPorVeiculo(DateTime data, int veiculo)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Frete.ContratoFreteTransportador>();

            var result = from obj in query
                         where
                             obj.DataFinal.Date >= data && obj.DataInicial <= data
                             && (obj.Situacao == Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoContratoFreteTransportador.Aprovado
                             || obj.Situacao == Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoContratoFreteTransportador.AgAprovacao)
                             && obj.Veiculos.Any(vei => vei.Veiculo.Codigo == veiculo) && obj.Ativo

                         select obj;

            return result.FirstOrDefault();
        }

        public Task<Dominio.Entidades.Embarcador.Frete.ContratoFreteTransportador> BuscarContratosPorVeiculoAsync(DateTime data, int veiculo)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Frete.ContratoFreteTransportador>();

            var result = from obj in query
                         where
                             obj.DataFinal.Date >= data && obj.DataInicial <= data
                             && (obj.Situacao == Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoContratoFreteTransportador.Aprovado
                             || obj.Situacao == Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoContratoFreteTransportador.AgAprovacao)
                             && obj.Veiculos.Any(vei => vei.Veiculo.Codigo == veiculo) && obj.Ativo

                         select obj;

            return result.FirstOrDefaultAsync(CancellationToken);
        }

        public List<Dominio.Entidades.Veiculo> BuscarVeiculosEmContratos(DateTime data, List<int> veiculos)
        {
            var queryVeiculos = QueryBuscarVeiculosEmContratos(data, veiculos);
            return queryVeiculos.ToList();
        }

        public List<int> BuscarCodigosVeiculosEmContratos(DateTime data, List<int> veiculos)
        {
            var queryVeiculos = QueryBuscarVeiculosEmContratos(data, veiculos);
            return queryVeiculos.Select(obj => obj.Codigo).ToList();
        }

        public List<Dominio.Entidades.Embarcador.Frete.ContratoFreteTransportador> BuscarContratosVencidos(DateTime horaBase)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Frete.ContratoFreteTransportador>();

            var result = from obj in query
                         where
                             obj.DataFinal.Date < horaBase
                             && obj.Ativo
                             && obj.Situacao == Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoContratoFreteTransportador.Aprovado
                         select obj;

            return result.ToList();
        }

        public List<Dominio.Entidades.Embarcador.Frete.ContratoFreteTransportador> Consultar(Dominio.ObjetosDeValor.Embarcador.Frete.FiltroPesquisaContratoFreteTransportador filtrosPesquisa, string propOrdena, string dirOrdena, int inicio, int limite)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Frete.ContratoFreteTransportador>();

            if (filtrosPesquisa.CodigoTransportador.Count > 0)
                query = query.Where(o => filtrosPesquisa.CodigoTransportador.Contains(o.Transportador.Codigo));

            if (!string.IsNullOrWhiteSpace(filtrosPesquisa.Descricao))
                query = query.Where(o => o.Descricao.Contains(filtrosPesquisa.Descricao));

            if (!string.IsNullOrWhiteSpace(filtrosPesquisa.NumeroContrato))
                query = query.Where(o => o.NumeroEmbarcador == filtrosPesquisa.NumeroContrato);

            if (filtrosPesquisa.DataInicial != DateTime.MinValue)
                query = query.Where(o => o.DataInicial >= filtrosPesquisa.DataInicial.Date);

            if (filtrosPesquisa.DataFinal != DateTime.MinValue)
                query = query.Where(o => o.DataFinal <= filtrosPesquisa.DataFinal.Date);

            if (filtrosPesquisa.TipoContratoFrete.Count > 0)
                query = query.Where(o => filtrosPesquisa.TipoContratoFrete.Contains(o.TipoContratoFrete.Codigo));

            if (filtrosPesquisa.Status == Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoAtivoPesquisa.Ativo)
                query = query.Where(o => o.Ativo);
            else if (filtrosPesquisa.Status == Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoAtivoPesquisa.Inativo)
                query = query.Where(o => !o.Ativo);

            if (filtrosPesquisa.Situacao.HasValue)
                query = query.Where(o => o.Situacao == filtrosPesquisa.Situacao.Value);

            if (!string.IsNullOrWhiteSpace(filtrosPesquisa.Placa))
                query = query.Where(o => o.Veiculos.Any(v => v.Veiculo.Placa == filtrosPesquisa.Placa));

            if (filtrosPesquisa.CodigoStatusAceiteContrato > 0)
                query = query.Where(o => o.StatusAceiteContrato.Codigo == filtrosPesquisa.CodigoStatusAceiteContrato);

            if (filtrosPesquisa.SituacaoIntegracao.HasValue)
            {
                var queryIntegracao = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Frete.ContratoFreteTransportadorIntegracao>();
                if (filtrosPesquisa.SituacaoIntegracao.Value == Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoIntegracao.AgIntegracao)
                {
                    queryIntegracao = queryIntegracao.Where(c => c.SituacaoIntegracao == Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoIntegracao.AgIntegracao);
                    query = query.Where(c => queryIntegracao.Any(i => i.ContratoFreteTransportador.Codigo == c.Codigo));
                }
                else if (filtrosPesquisa.SituacaoIntegracao.Value == Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoIntegracao.AgRetorno)
                {
                    queryIntegracao = queryIntegracao.Where(c => c.SituacaoIntegracao == Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoIntegracao.AgRetorno);
                    query = query.Where(c => queryIntegracao.Any(i => i.ContratoFreteTransportador.Codigo == c.Codigo));
                }
                else if (filtrosPesquisa.SituacaoIntegracao.Value == Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoIntegracao.Integrado)
                {
                    queryIntegracao = queryIntegracao.Where(c => c.SituacaoIntegracao == Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoIntegracao.Integrado);
                    query = query.Where(c => queryIntegracao.Any(i => i.ContratoFreteTransportador.Codigo == c.Codigo));
                }
                else if (filtrosPesquisa.SituacaoIntegracao.Value == Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoIntegracao.ProblemaIntegracao)
                {
                    queryIntegracao = queryIntegracao.Where(c => c.SituacaoIntegracao == Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoIntegracao.ProblemaIntegracao);
                    query = query.Where(c => queryIntegracao.Any(i => i.ContratoFreteTransportador.Codigo == c.Codigo));
                }
            }

            return query
                .Fetch(obj => obj.Transportador)
                .ThenFetch(obj => obj.Localidade)
                .Fetch(obj => obj.TipoContratoFrete).OrderBy(propOrdena + " " + dirOrdena).Skip(inicio)
                .Take(limite).ToList();
        }

        public int ContarConsulta(Dominio.ObjetosDeValor.Embarcador.Frete.FiltroPesquisaContratoFreteTransportador filtrosPesquisa)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Frete.ContratoFreteTransportador>();

            if (filtrosPesquisa.CodigoTransportador.Count > 0)
                query = query.Where(o => filtrosPesquisa.CodigoTransportador.Contains(o.Transportador.Codigo));

            if (!string.IsNullOrWhiteSpace(filtrosPesquisa.Descricao))
                query = query.Where(o => o.Descricao.Contains(filtrosPesquisa.Descricao));

            if (!string.IsNullOrWhiteSpace(filtrosPesquisa.NumeroContrato))
                query = query.Where(o => o.NumeroEmbarcador == filtrosPesquisa.NumeroContrato);

            if (filtrosPesquisa.DataInicial != DateTime.MinValue)
                query = query.Where(o => o.DataInicial >= filtrosPesquisa.DataInicial.Date);

            if (filtrosPesquisa.DataFinal != DateTime.MinValue)
                query = query.Where(o => o.DataFinal <= filtrosPesquisa.DataFinal.Date);

            if (filtrosPesquisa.TipoContratoFrete.Count > 0)
                query = query.Where(o => filtrosPesquisa.TipoContratoFrete.Contains(o.TipoContratoFrete.Codigo));

            if (filtrosPesquisa.Status == Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoAtivoPesquisa.Ativo)
                query = query.Where(o => o.Ativo);
            else if (filtrosPesquisa.Status == Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoAtivoPesquisa.Inativo)
                query = query.Where(o => !o.Ativo);

            if (filtrosPesquisa.Situacao.HasValue)
                query = query.Where(o => o.Situacao == filtrosPesquisa.Situacao.Value);

            if (!string.IsNullOrWhiteSpace(filtrosPesquisa.Placa))
                query = query.Where(o => o.Veiculos.Any(v => v.Veiculo.Placa == filtrosPesquisa.Placa));

            if (filtrosPesquisa.CodigoStatusAceiteContrato > 0)
                query = query.Where(o => o.StatusAceiteContrato.Codigo == filtrosPesquisa.CodigoStatusAceiteContrato);

            return query.Count();
        }

        public Dominio.Entidades.Embarcador.Frete.ContratoFreteTransportador VerificarContratoExistente(List<int> codigosDesconsiderar, int codigoTransportador, int tipoContratoFrete, DateTime dataInicial, DateTime dataFinal, List<int> ocorrencias, List<int> codigosCanalEntrega, List<int> codigosModelosVeiculares)
        {


            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Frete.ContratoFreteTransportador>();
            var subquery = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Frete.ContratoFreteTransportadorAcordo>();
            var result = from obj in query
                         where
                            obj.Ativo
                            && !codigosDesconsiderar.Contains(obj.Codigo)
                            && obj.Situacao != Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoContratoFreteTransportador.Rejeitado
                            && (
                                (obj.DataInicial <= dataFinal) && (obj.DataFinal >= dataInicial)
                            )
                            && obj.Transportador.Codigo == codigoTransportador && obj.CanaisEntrega.Any(x => codigosCanalEntrega.Contains(x.Codigo)) && subquery.Any(x => x.ContratoFrete.Codigo == obj.Codigo && codigosModelosVeiculares.Contains(x.ModeloVeicular.Codigo))
                         select obj;

            //// Contrato com vigencia maior ou igual que a em questao
            //(obj.DataInicial >= dataInicial && obj.DataFinal >= dataFinal)
            //// Contrato com vigencia menor ou igual que a em questao
            //|| (obj.DataInicial <= dataInicial && obj.DataFinal <= dataFinal)
            //// Conflito no inicio da vigencia
            //|| (obj.DataFinal >= dataInicial)
            //// Conflito no fim da vigencia
            //|| (obj.DataInicial <= dataFinal)


            //var result = from obj in query
            //             where
            //                obj.Ativo
            //                && obj.Situacao != Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoContratoFreteTransportador.Rejeitado
            //                && obj.Transportador.Codigo == codigoTransportador
            //                && ((obj.DataInicial <= dataInicial.Date && obj.DataFinal >= dataInicial.Date) || (obj.DataInicial <= dataFinal.Date && obj.DataFinal >= dataFinal.Date))
            //             select obj;

            if (tipoContratoFrete > 0)
                result = result.Where(o => o.TipoContratoFrete.Codigo == tipoContratoFrete);

            if (ocorrencias.Count() > 0)
                result = result.Where(o => !o.TiposOcorrencia.Any(c => !ocorrencias.Contains(c.Codigo)));
            //result = result.Where(o => tipoOcorrencia != o.TiposOcorrencia.Codigo));


            return result.FirstOrDefault();
        }

        public int BuscarProximoNumero()
        {
            string query = $@"select max(CFT_NUMERO) from T_CONTRATO_FRETE_TRANSPORTADOR";
            var result = this.SessionNHiBernate.CreateSQLQuery(query).SetTimeout(120).UniqueResult();

            if (result == null)
                return 1;

            int.TryParse(result.ToString(), out int retorno);
            return retorno + 1;
        }

        public int BuscarProximoAditivo(int transportador, int tipo)
        {
            string query = $@"select max(CFT_NUMERO_ADITIVO) from T_CONTRATO_FRETE_TRANSPORTADOR where (EMP_CODIGO = {transportador} or EMP_CODIGO is null) and TCF_CODIGO = {tipo}";
            var result = this.SessionNHiBernate.CreateSQLQuery(query).SetTimeout(120).UniqueResult();

            if (result == null)
                return 1;

            int.TryParse(result.ToString(), out int retorno);
            return retorno + 1;
        }

        public Dominio.Entidades.Embarcador.Frete.ContratoFreteTransportador BuscarContratoAtivo(int transportador, int tipoOcorrencia, DateTime vigencia)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Frete.ContratoFreteTransportador>();

            var result = from obj in query
                         where
                            (obj.Transportador == null || obj.Transportador.Codigo == transportador)
                            && obj.DataInicial.Date <= vigencia.Date
                            && obj.DataFinal.Date >= vigencia.Date
                            && obj.TiposOcorrencia.Any(o => o.Codigo == tipoOcorrencia)
                            && obj.Ativo
                         select obj;

            return result.OrderBy(contrato => contrato.Transportador != null).FirstOrDefault();
        }

        public List<Dominio.Entidades.Embarcador.Frete.ContratoFreteTransportador> BuscarAtivoPorTransportador(int codigoTransportador)
        {
            DateTime vigencia = DateTime.Now.Date;

            var consultaContratoFreteTransportador = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Frete.ContratoFreteTransportador>()
                .Where(o =>
                    o.Transportador.Codigo == codigoTransportador &&
                    o.DataInicial.Date <= vigencia &&
                    o.DataFinal.Date >= vigencia.Add(DateTime.MaxValue.TimeOfDay) &&
                    o.Ativo
                );

            return consultaContratoFreteTransportador.ToList();
        }

        public List<Dominio.Entidades.Embarcador.Frete.ContratoFreteTransportador> BuscarAtivoPorTransportadores(List<int> codigosTransportadores)
        {
            DateTime vigencia = DateTime.Now.Date;

            var consultaContratoFreteTransportador = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Frete.ContratoFreteTransportador>()
                .Where(o =>
                    codigosTransportadores.Contains(o.Transportador.Codigo) &&
                    o.DataInicial.Date <= vigencia &&
                    o.DataFinal.Date >= vigencia.Add(DateTime.MaxValue.TimeOfDay) &&
                    o.Ativo
                );

            return consultaContratoFreteTransportador.ToList();
        }

        public List<Dominio.Entidades.Embarcador.Frete.ContratoFreteTransportador> BuscarContratosPorTabelaFreteETransportador(int codigoTabelaFrete, int codigoTransportador)
        {
            DateTime vigencia = DateTime.Now.Date;
            var consultaContratoFreteTransportador = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Frete.ContratoFreteTransportador>()
                .Where(o =>
                    o.DataInicial.Date <= vigencia &&
                    o.DataFinal.Date >= vigencia.Add(DateTime.MaxValue.TimeOfDay) &&
                    o.Ativo && o.TabelasFrete.Any(x => x.Codigo == codigoTabelaFrete) && o.Transportador.Codigo == codigoTransportador
                );

            return consultaContratoFreteTransportador.ToList();
        }

        public int OcorrenciaEmAberto(int contrato)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Ocorrencias.CargaOcorrencia>();

            Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoOcorrencia[] situacloes = new Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoOcorrencia[] {
                Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoOcorrencia.AgAprovacao,
                Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoOcorrencia.AgAutorizacaoEmissao,
                Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoOcorrencia.SemRegraAprovacao,
                Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoOcorrencia.SemRegraEmissao,
                Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoOcorrencia.EmEmissaoCTeComplementar,
                Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoOcorrencia.PendenciaEmissao,

            };
            var result = from obj in query
                         where
                            obj.ContratoFrete.Codigo == contrato
                            && situacloes.Contains(obj.SituacaoOcorrencia)
                         select obj.NumeroOcorrencia;

            return result.FirstOrDefault();
        }

        public List<int> BuscarCodigosContratosComDuplicacaoPorCodigos(List<int> codigos)
        {
            var consultaContratoFreteTransportador = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Frete.ContratoFreteTransportador>()
                .Where(contrato => codigos.Contains(contrato.ContratoOriginario.Codigo));

            return consultaContratoFreteTransportador.Select(contrato => contrato.ContratoOriginario.Codigo).ToList();
        }

        public bool ExisteDuplicacaoPorCodigo(int codigo)
        {
            var consultaContratoFreteTransportador = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Frete.ContratoFreteTransportador>()
                .Where(contrato => contrato.ContratoOriginario.Codigo == codigo);

            return consultaContratoFreteTransportador.Count() > 0;
        }

        #endregion

        #region Métodos Públicos - Relatórios

        public IList<Dominio.Relatorios.Embarcador.DataSource.Fretes.RelatorioContratoFreteTransportador> ConsultarRelatorioContratoFreteTransportador(Dominio.ObjetosDeValor.Embarcador.Frete.FiltroPesquisaRelatorioContratoFreteTransportador filtrosPesquisa, List<Dominio.Relatorios.Embarcador.ObjetosDeValor.PropriedadeAgrupamento> propriedades, string propAgrupa, string dirAgrupa, string propOrdena, string dirOrdena, int inicio, int limite)
        {
            string sql = ObterSelectConsultaRelatorioContratoFreteTransportador(filtrosPesquisa, false, propriedades, propAgrupa, dirAgrupa, propOrdena, dirOrdena, inicio, limite);
            var query = this.SessionNHiBernate.CreateSQLQuery(sql);

            query.SetResultTransformer(new NHibernate.Transform.AliasToBeanResultTransformer(typeof(Dominio.Relatorios.Embarcador.DataSource.Fretes.RelatorioContratoFreteTransportador)));

            return query.SetTimeout(600).List<Dominio.Relatorios.Embarcador.DataSource.Fretes.RelatorioContratoFreteTransportador>();
        }

        public async Task<IList<Dominio.Relatorios.Embarcador.DataSource.Fretes.RelatorioContratoFreteTransportador>> ConsultarRelatorioContratoFreteTransportadorAsync(Dominio.ObjetosDeValor.Embarcador.Frete.FiltroPesquisaRelatorioContratoFreteTransportador filtrosPesquisa, List<Dominio.Relatorios.Embarcador.ObjetosDeValor.PropriedadeAgrupamento> propriedades, string propAgrupa, string dirAgrupa, string propOrdena, string dirOrdena, int inicio, int limite)
        {
            string sql = ObterSelectConsultaRelatorioContratoFreteTransportador(filtrosPesquisa, false, propriedades, propAgrupa, dirAgrupa, propOrdena, dirOrdena, inicio, limite);
            var query = this.SessionNHiBernate.CreateSQLQuery(sql);

            query.SetResultTransformer(new NHibernate.Transform.AliasToBeanResultTransformer(typeof(Dominio.Relatorios.Embarcador.DataSource.Fretes.RelatorioContratoFreteTransportador)));

            return await query.SetTimeout(600).ListAsync<Dominio.Relatorios.Embarcador.DataSource.Fretes.RelatorioContratoFreteTransportador>();
        }

        public int ContarConsultaRelatorioContratoFreteTransportador(Dominio.ObjetosDeValor.Embarcador.Frete.FiltroPesquisaRelatorioContratoFreteTransportador filtrosPesquisa, List<Dominio.Relatorios.Embarcador.ObjetosDeValor.PropriedadeAgrupamento> propriedades, string propAgrupa, string dirAgrupa, string propOrdena, string dirOrdena)
        {
            string sql = ObterSelectConsultaRelatorioContratoFreteTransportador(filtrosPesquisa, true, propriedades, propAgrupa, dirAgrupa, propOrdena, dirOrdena, 0, 0);
            var query = this.SessionNHiBernate.CreateSQLQuery(sql);

            return query.SetTimeout(600).UniqueResult<int>();
        }

        private string ObterSelectConsultaRelatorioContratoFreteTransportador(Dominio.ObjetosDeValor.Embarcador.Frete.FiltroPesquisaRelatorioContratoFreteTransportador filtrosPesquisa, bool count, List<Dominio.Relatorios.Embarcador.ObjetosDeValor.PropriedadeAgrupamento> propriedades, string propAgrupa, string dirAgrupa, string propOrdena, string dirOrdena, int inicio, int limite)
        {
            string select = string.Empty,
                   groupBy = string.Empty,
                   joins = string.Empty,
                   where = string.Empty,
                   orderBy = string.Empty;

            for (var i = propriedades.Count - 1; i >= 0; i--)
                SetarSelectRelatorioConsultaContratoFreteTransportador(propriedades[i].Propriedade, propriedades[i].CodigoDinamico, ref select, ref groupBy, ref joins, count);

            SetarWhereRelatorioConsultaContratoFreteTransportador(ref where, ref groupBy, ref joins, filtrosPesquisa);

            if (!count)
            {
                if (!string.IsNullOrWhiteSpace(propAgrupa))
                {
                    SetarSelectRelatorioConsultaContratoFreteTransportador(propAgrupa, 0, ref select, ref groupBy, ref joins, count);

                    if (select.Contains(propAgrupa))
                        orderBy = propAgrupa + " " + dirAgrupa;
                }

                if (!string.IsNullOrWhiteSpace(propOrdena))
                {
                    if (propOrdena != propAgrupa && select.Contains(propOrdena) && propOrdena != "Codigo")
                        orderBy += (orderBy.Length > 0 ? ", " : string.Empty) + propOrdena + " " + dirOrdena;
                }
            }


            // SELECT
            string query = "SELECT ";

            if (count)
                query += "DISTINCT(COUNT(0) OVER())";
            else if (select.Length > 0)
                query += select.Substring(0, select.Length - 2);

            // FROM
            query += " FROM T_CONTRATO_FRETE_TRANSPORTADOR Contrato ";

            // JOIN
            query += joins;

            // WHERE
            query += " WHERE 1 = 1" + where;

            // GROUP BY
            if (groupBy.Length > 0)
                query += " GROUP BY " + groupBy.Substring(0, groupBy.Length - 2);

            // ORDER BY
            if (orderBy.Length > 0)
                query += " ORDER BY " + orderBy;
            else if (!count)
                query += " ORDER BY 1 ASC";

            // LIMIT
            if (!count && limite > 0)
                query += " OFFSET " + inicio.ToString() + " ROWS FETCH NEXT " + limite.ToString() + " ROWS ONLY";

            return query;
        }

        private void SetarSelectRelatorioConsultaContratoFreteTransportador(string propriedade, int codigoDinamico, ref string select, ref string groupBy, ref string joins, bool count)
        {
            switch (propriedade)
            {
                case "Codigo":
                    if (!select.Contains(" Codigo, "))
                    {
                        select += "Contrato.CFT_CODIGO Codigo, ";
                    }
                    break;
                case "Numero":
                    if (!select.Contains(" NumeroSequencial, "))
                    {
                        select += "Contrato.CFT_NUMERO NumeroSequencial, ";
                    }
                    if (!select.Contains(" NumeroEmbarcador, "))
                    {
                        select += "Contrato.CFT_NUMERO_EMBARCADOR NumeroEmbarcador, ";
                    }
                    break;
                case "Descricao":
                    if (!select.Contains(" Descricao, "))
                    {
                        select += "Contrato.CFT_DESCRICAO Descricao, ";
                    }
                    break;
                case "VigenciaInicial":
                    if (!select.Contains(" DataInicial, "))
                        select += "Contrato.CFT_DATA_INICIAL DataInicial, ";
                    break;
                case "VigenciaFinal":
                    if (!select.Contains(" DataFinal, "))
                        select += "Contrato.CFT_DATA_FINAL DataFinal, ";
                    break;
                case "DescricaoSituacao":
                    if (!select.Contains(" Situacao, "))
                    {
                        select += "Contrato.CFT_ATIVO Situacao, ";
                    }
                    break;
                case "Transportador":
                    if (!select.Contains(" Transportador, "))
                    {
                        if (!joins.Contains(" Empresa "))
                            joins += " JOIN T_EMPRESA Empresa ON Empresa.EMP_CODIGO = Contrato.EMP_CODIGO ";

                        select += "Empresa.EMP_RAZAO Transportador, ";
                    }
                    break;
                case "TipoContratoFrete":
                    if (!select.Contains(" TipoContratoFrete, "))
                    {
                        if (!joins.Contains(" TipoContratoFrete "))
                            joins += "LEFT JOIN T_TIPO_CONTRATO_FRETE TipoContratoFrete ON TipoContratoFrete.TCF_CODIGO = Contrato.TCF_CODIGO ";

                        select += "TipoContratoFrete.TCF_DESCRICAO TipoContratoFrete, ";
                    }
                    break;

                case "DescricaoStatus":
                    if (!select.Contains(" Status, "))
                        select += "Contrato.CFT_SITUACAO Status, ";
                    break;
                case "TipoVeiculo":
                    if (!select.Contains(" TipoVeiculo, "))
                    {
                        select += "TipoVeiculo.MVC_DESCRICAO TipoVeiculo, ";

                        if (!joins.Contains(" Veiculos "))
                            joins += " left join T_CONTRATO_FRETE_TRANSPORTADOR_VEICULO Veiculos on Veiculos.CFT_CODIGO = Contrato.CFT_CODIGO ";

                        if (!joins.Contains(" Cavalo "))
                            joins += " left join T_VEICULO Cavalo on Cavalo.VEI_CODIGO = Veiculos.VEI_CODIGO ";

                        if (!joins.Contains(" TipoVeiculo "))
                            joins += " left join T_MODELO_VEICULAR_CARGA TipoVeiculo on TipoVeiculo.MVC_CODIGO = Cavalo.MVC_CODIGO ";
                    }
                    break;
                case "Cavalo":
                    if (!select.Contains(" Cavalo, "))
                    {
                        select += "Cavalo.VEI_PLACA Cavalo, ";

                        if (!joins.Contains(" Veiculos "))
                            joins += " left join T_CONTRATO_FRETE_TRANSPORTADOR_VEICULO Veiculos on Veiculos.CFT_CODIGO = Contrato.CFT_CODIGO ";

                        if (!joins.Contains(" Cavalo "))
                            joins += " left join T_VEICULO Cavalo on Cavalo.VEI_CODIGO = Veiculos.VEI_CODIGO ";
                    }
                    break;
                case "Carreta":
                    if (!select.Contains(" Carreta, "))
                    {
                        select += "Carreta.VEI_PLACA Carreta, ";

                        if (!joins.Contains(" Veiculos "))
                            joins += " left join T_CONTRATO_FRETE_TRANSPORTADOR_VEICULO Veiculos on Veiculos.CFT_CODIGO = Contrato.CFT_CODIGO ";

                        if (!joins.Contains(" Cavalo "))
                            joins += " left join T_VEICULO Cavalo on Cavalo.VEI_CODIGO = Veiculos.VEI_CODIGO ";

                        if (!joins.Contains(" VeiConjunto "))
                            joins += " left join T_VEICULO_CONJUNTO VeiConjunto on Cavalo.VEI_CODIGO = VeiConjunto.VEC_CODIGO_PAI ";

                        if (!joins.Contains(" Carreta "))
                            joins += " left join T_VEICULO Carreta on Carreta.VEI_CODIGO = VeiConjunto.VEC_CODIGO_FILHO ";
                    }
                    break;
                case "ContratoTransportador":
                    if (!select.Contains(" ContratoTransportador, "))
                    {
                        select += " ContratoTransportadorFrete.CTF_NOME_CONTRATO ContratoTransportador, ";

                        if (!joins.Contains(" ContratoTransportadorFrete "))
                            joins += " left join T_CONTRATO_TRANSPORTADOR_FRETE ContratoTransportadorFrete ON Contrato.CTF_CODIGO = ContratoTransportadorFrete.CTF_CODIGO ";
                    }
                    break;
                case "IDExterno":
                    if (!select.Contains(" IDExterno, "))
                        select += " Contrato.CFT_ID_EXTERNO IDExterno, ";
                    break;
                case "StatusAceiteContrato":
                    if (!select.Contains(" StatusAceiteContrato, "))
                    {
                        select += " StatusAssinaturaContrato.STC_DESCRICAO StatusAceiteContrato, ";

                        if (!joins.Contains(" StatusAssinaturaContrato "))
                            joins += " left join T_STATUS_ASSINATURA_CONTRATO StatusAssinaturaContrato ON Contrato.STC_CODIGO = StatusAssinaturaContrato.STC_CODIGO ";
                    }
                    break;
                case "DescricaoTipoFechamento":
                    if (!select.Contains(" TipoFechamento, "))
                        select += " Contrato.CFT_PERIODO_ACORDO TipoFechamento, ";
                    break;
                case "ValorMensal":
                    if (!select.Contains(" ValorMensal, "))
                        select += @" CASE WHEN CFA_CODIGO = (
                                          SELECT 
                                            MIN(CFA_CODIGO) 
                                          FROM 
                                            T_CONTRATO_FRETE_TRANSPORTADOR_ACORDO 
                                          WHERE 
                                            CFT_CODIGO = Contrato.CFT_CODIGO
                                        ) THEN FORMAT(
                                          CAST(
                                            CFT_VALOR_TOTAL_MENSAL AS DECIMAL(18, 2)
                                          ), 
                                          'N', 
                                          'pt-BR'
                                        ) ELSE '' END AS ValorMensal, ";
                    break;
                case "ModeloVeicular":
                    if (!select.Contains(" ModeloVeicular, "))
                    {
                        select += " ModeloVeicularCarga.MVC_DESCRICAO ModeloVeicular, ";

                        if (!joins.Contains(" ContratoFreteTransportadorAcordo "))
                            joins += " left join T_CONTRATO_FRETE_TRANSPORTADOR_ACORDO ContratoFreteTransportadorAcordo ON Contrato.CFT_CODIGO = ContratoFreteTransportadorAcordo.CFT_CODIGO ";
                        if (!joins.Contains(" ModeloVeicularCarga "))
                            joins += " left join T_MODELO_VEICULAR_CARGA ModeloVeicularCarga ON ContratoFreteTransportadorAcordo.MVC_CODIGO = ModeloVeicularCarga.MVC_CODIGO ";
                    }
                    break;
                case "ValorAcordado":
                    if (!select.Contains(" ValorAcordado, "))
                    {
                        select += " ContratoFreteTransportadorAcordo.CFA_VALOR_ACORDADO ValorAcordado, ";

                        if (!joins.Contains(" ContratoFreteTransportadorAcordo "))
                            joins += " left join T_CONTRATO_FRETE_TRANSPORTADOR_ACORDO ContratoFreteTransportadorAcordo ON Contrato.CFT_CODIGO = ContratoFreteTransportadorAcordo.CFT_CODIGO ";
                    }
                    break;
                case "QuantidadeVeiculo":
                    if (!select.Contains(" QuantidadeVeiculo, "))
                    {
                        select += " ContratoFreteTransportadorAcordo.CFA_QUANTIDADE QuantidadeVeiculo, ";

                        if (!joins.Contains(" ContratoFreteTransportadorAcordo "))
                            joins += " left join T_CONTRATO_FRETE_TRANSPORTADOR_ACORDO ContratoFreteTransportadorAcordo ON Contrato.CFT_CODIGO = ContratoFreteTransportadorAcordo.CFT_CODIGO ";
                    }
                    break;
                case "QuantidadeAproxCargasMensal":
                    if (!select.Contains(" QuantidadeAproxCargasMensal, "))
                    {
                        select += @"CASE WHEN CFA_CODIGO = (
                                        SELECT
                                          MIN(CFA_CODIGO)
                                        FROM
                                          T_CONTRATO_FRETE_TRANSPORTADOR_ACORDO
                                        WHERE
                                          CFT_CODIGO = Contrato.CFT_CODIGO
                                      ) THEN CAST(
                                        CFT_ESTIMATIVA_CARGAS_MES AS varchar
                                      ) ELSE '' END QuantidadeAproxCargasMensal, ";

                        if (!joins.Contains(" ContratoFreteTransportadorAcordo "))
                            joins += " left join T_CONTRATO_FRETE_TRANSPORTADOR_ACORDO ContratoFreteTransportadorAcordo ON Contrato.CFT_CODIGO = ContratoFreteTransportadorAcordo.CFT_CODIGO ";
                    }
                    break;
                case "TipoCarga":
                    if (!select.Contains(" TipoCarga, "))
                    {
                        select += @" SUBSTRING(
                                        (
                                          SELECT 
                                            ', ' + tc.TCG_DESCRICAO 
                                          FROM 
                                            T_CONTRATO_FRETE_TRANSPORTADOR c 
                                            JOIN T_CONTRATO_FRETE_TRANSPORTADOR_TIPO_CARGA ctt ON c.CFT_CODIGO = ctt.CFT_CODIGO 
                                            JOIN T_TIPO_DE_CARGA tc ON ctt.TCG_CODIGO = tc.TCG_CODIGO 
                                          WHERE 
                                            Contrato.CFT_CODIGO = c.CFT_CODIGO FOR XML PATH('')
                                        ), 
                                        2, 
                                        1000
                                      ) TipoCarga, ";
                    }
                    break;
                case "CanalEntrega":
                    if (!select.Contains(" CanalEntrega, "))
                    {
                        select += @" SUBSTRING(
                                        (
                                          SELECT 
                                            ', ' + ce.CNE_DESCRICAO 
                                          FROM 
                                            T_CONTRATO_FRETE_TRANSPORTADOR c 
                                            JOIN T_CONTRATO_FRETE_TRANSPORTADOR_CANAL_ENTREGA cte ON c.CFT_CODIGO = cte.CFT_CODIGO 
                                            JOIN T_CANAL_ENTREGA ce ON cte.CNE_CODIGO = ce.CNE_CODIGO 
                                          WHERE 
                                            Contrato.CFT_CODIGO = c.CFT_CODIGO FOR XML PATH('')
                                        ), 
                                        2, 
                                        1000
                                      ) CanalEntrega, ";
                    }
                    break;
                case "DescricaoPontoPlanejamentoTransporte":
                    if (!select.Contains(" PontoPlanejamentoTransporte, "))
                        select += " Contrato.CFT_PONTO_PLANEJAMENTO_TRANSPORTE PontoPlanejamentoTransporte, ";
                    break;
                case "DescricaoTipoIntegracao":
                    if (!select.Contains(" TipoIntegracao, "))
                        select += " Contrato.CFT_TIPO_INTEGRACAO TipoIntegracao, ";
                    break;
                case "DescricaoGrupoCarga":
                    if (!select.Contains(" GrupoCarga, "))
                        select += " Contrato.CFT_GRUPO_CARGA GrupoCarga, ";
                    break;
                case "TabelasFrete":
                    if (!select.Contains(" TabelasFrete, "))
                    {
                        select += @" SUBSTRING(
                                        (
                                          SELECT 
                                            ', ' + tf.TBF_DESCRICAO 
                                          FROM 
                                            T_CONTRATO_FRETE_TRANSPORTADOR c 
                                            JOIN T_CONTRATO_FRETE_TRANSPORTADOR_TABELA_FRETE cft ON c.CFT_CODIGO = cft.CFT_CODIGO 
                                            JOIN T_TABELA_FRETE tf ON cft.TBF_CODIGO = tf.TBF_CODIGO 
                                          WHERE 
                                            Contrato.CFT_CODIGO = c.CFT_CODIGO FOR XML PATH('')
                                        ), 
                                        2, 
                                        1000
                                      ) TabelasFrete, ";
                    }
                    break;
                case "Observacao":
                    if (!select.Contains(" Observacao, "))
                        select += " Contrato.CFT_OBSERVACAO Observacao, ";
                    break;
                default:
                    break;
            }
        }

        private void SetarWhereRelatorioConsultaContratoFreteTransportador(ref string where, ref string groupBy, ref string joins, Dominio.ObjetosDeValor.Embarcador.Frete.FiltroPesquisaRelatorioContratoFreteTransportador filtrosPesquisa)
        {
            string pattern = "yyyy-MM-dd";

            if (filtrosPesquisa.DataInicial != DateTime.MinValue || filtrosPesquisa.DataFinal != DateTime.MinValue)
            {
                if (filtrosPesquisa.DataInicial != DateTime.MinValue)
                    where += " AND Contrato.CFT_DATA_INICIAL >= '" + filtrosPesquisa.DataInicial.ToString(pattern) + " 00:00:00'";
                if (filtrosPesquisa.DataFinal != DateTime.MinValue)
                    where += " AND Contrato.CFT_DATA_FINAL <= '" + filtrosPesquisa.DataFinal.ToString(pattern) + " 23:59:59'";
            }

            if (filtrosPesquisa.DiasParaVencimento > 0)
            {
                where += " AND Contrato.CFT_DATA_FINAL <= '" + DateTime.Now.Date.AddDays(filtrosPesquisa.DiasParaVencimento).ToString(pattern) + "'  AND Contrato.CFT_DATA_FINAL >= '" + DateTime.Now.Date.ToString(pattern) + "'";
            }

            if (filtrosPesquisa.TipoContratoFrete > 0)
            {
                if (!joins.Contains(" TipoContratoFrete "))
                    joins += "LEFT JOIN T_TIPO_CONTRATO_FRETE TipoContratoFrete ON TipoContratoFrete.TCF_CODIGO = Contrato.TCF_CODIGO ";

                where += " AND TipoContratoFrete.TCF_CODIGO = " + filtrosPesquisa.TipoContratoFrete.ToString();
            }


            if (filtrosPesquisa.CodigosTransportador.Count() > 0)
            {
                where += " AND Contrato.EMP_CODIGO in (" + string.Join(", ", filtrosPesquisa.CodigosTransportador) + ")";
            }

            if (filtrosPesquisa.Situacao != Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoAtivoPesquisa.Todos)
            {
                where += " AND Contrato.CFT_ATIVO = " + (filtrosPesquisa.Situacao == Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoAtivoPesquisa.Ativo ? "1" : "0");
            }

            if (filtrosPesquisa.EmVigencia)
            {
                string dataBase = DateTime.Now.ToString(pattern);
                where += " AND Contrato.CFT_DATA_INICIAL <= '" + dataBase + "' AND Contrato.CFT_DATA_FINAL >= '" + dataBase + "'";
            }

            if (!string.IsNullOrWhiteSpace(filtrosPesquisa.NumeroEmbarcador))
                where += " AND Contrato.CFT_NUMERO_EMBARCADOR = '" + filtrosPesquisa.NumeroEmbarcador + "'";
        }

        public IList<Dominio.Relatorios.Embarcador.DataSource.Transportadores.ReportTrnasportadores> ConsultarRelatorioTransportadoresSemContrato(DateTime dataInicial, DateTime dataFinal, Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoAtivoPesquisa situacao, bool somenteContrato, bool somenteSemContrato, List<Dominio.Relatorios.Embarcador.ObjetosDeValor.PropriedadeAgrupamento> propriedades, string propAgrupa, string dirAgrupa, string propOrdena, string dirOrdena, int inicio, int limite)
        {
            string sql = ObterSelectConsultaRelatorioTransportadoresSemContrato(dataInicial, dataFinal, situacao, somenteContrato, somenteSemContrato, false, propriedades, propAgrupa, dirAgrupa, propOrdena, dirOrdena, inicio, limite);
            var query = this.SessionNHiBernate.CreateSQLQuery(sql);

            query.SetResultTransformer(new NHibernate.Transform.AliasToBeanResultTransformer(typeof(Dominio.Relatorios.Embarcador.DataSource.Transportadores.ReportTrnasportadores)));

            return query.SetTimeout(600).List<Dominio.Relatorios.Embarcador.DataSource.Transportadores.ReportTrnasportadores>();
        }

        public async Task<IList<Dominio.Relatorios.Embarcador.DataSource.Transportadores.ReportTrnasportadores>> ConsultarRelatorioTransportadoresSemContratoAsync(DateTime dataInicial, DateTime dataFinal, Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoAtivoPesquisa situacao, bool somenteContrato, bool somenteSemContrato, List<Dominio.Relatorios.Embarcador.ObjetosDeValor.PropriedadeAgrupamento> propriedades, string propAgrupa, string dirAgrupa, string propOrdena, string dirOrdena, int inicio, int limite)
        {
            string sql = ObterSelectConsultaRelatorioTransportadoresSemContrato(dataInicial, dataFinal, situacao, somenteContrato, somenteSemContrato, false, propriedades, propAgrupa, dirAgrupa, propOrdena, dirOrdena, inicio, limite);
            var query = this.SessionNHiBernate.CreateSQLQuery(sql);

            query.SetResultTransformer(new NHibernate.Transform.AliasToBeanResultTransformer(typeof(Dominio.Relatorios.Embarcador.DataSource.Transportadores.ReportTrnasportadores)));

            return await query.SetTimeout(600).ListAsync<Dominio.Relatorios.Embarcador.DataSource.Transportadores.ReportTrnasportadores>();
        }

        public int ContarConsultaRelatorioTransportadoresSemContrato(DateTime dataInicial, DateTime dataFinal, Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoAtivoPesquisa situacao, bool somenteContrato, bool somenteSemContrato, List<Dominio.Relatorios.Embarcador.ObjetosDeValor.PropriedadeAgrupamento> propriedades, string propAgrupa, string dirAgrupa, string propOrdena, string dirOrdena)
        {
            string sql = ObterSelectConsultaRelatorioTransportadoresSemContrato(dataInicial, dataFinal, situacao, somenteContrato, somenteSemContrato, true, propriedades, propAgrupa, dirAgrupa, propOrdena, dirOrdena, 0, 0);
            var query = this.SessionNHiBernate.CreateSQLQuery(sql);

            return query.SetTimeout(600).UniqueResult<int>();
        }

        private string ObterSelectConsultaRelatorioTransportadoresSemContrato(DateTime dataInicial, DateTime dataFinal, Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoAtivoPesquisa situacao, bool somenteContrato, bool somenteSemContrato, bool count, List<Dominio.Relatorios.Embarcador.ObjetosDeValor.PropriedadeAgrupamento> propriedades, string propAgrupa, string dirAgrupa, string propOrdena, string dirOrdena, int inicio, int limite)
        {
            string select = string.Empty,
                   groupBy = string.Empty,
                   joins = string.Empty,
                   where = string.Empty,
                   orderBy = string.Empty;

            for (var i = propriedades.Count - 1; i >= 0; i--)
                SetarSelectRelatorioConsultaTransportadoresSemContrato(propriedades[i].Propriedade, propriedades[i].CodigoDinamico, ref select, ref groupBy, ref joins, count);

            SetarWhereRelatorioConsultaTransportadoresSemContrato(ref where, ref groupBy, ref joins, dataInicial, dataFinal, situacao, somenteContrato, somenteSemContrato);

            if (!count)
            {
                if (!string.IsNullOrWhiteSpace(propAgrupa))
                {
                    SetarSelectRelatorioConsultaTransportadoresSemContrato(propAgrupa, 0, ref select, ref groupBy, ref joins, count);

                    if (select.Contains(propAgrupa))
                        orderBy = propAgrupa + " " + dirAgrupa;
                }

                if (!string.IsNullOrWhiteSpace(propOrdena))
                {
                    if (propOrdena != propAgrupa && select.Contains(propOrdena) && propOrdena != "Codigo")
                        orderBy += (orderBy.Length > 0 ? ", " : string.Empty) + propOrdena + " " + dirOrdena;
                }
            }


            // SELECT
            string query = "SELECT ";

            if (count)
                query += "DISTINCT(COUNT(0) OVER())";
            else if (select.Length > 0)
                query += select.Substring(0, select.Length - 2);

            // FROM
            query += " FROM T_EMPRESA Empresa ";

            // JOIN
            query += joins;

            // WHERE
            query += " WHERE 1 = 1" + where;

            // GROUP BY
            if (groupBy.Length > 0)
                query += " GROUP BY " + groupBy.Substring(0, groupBy.Length - 2);

            // ORDER BY
            if (orderBy.Length > 0)
                query += " ORDER BY " + orderBy;
            else if (!count)
                query += " ORDER BY 1 ASC";

            // LIMIT
            if (!count && limite > 0)
                query += " OFFSET " + inicio.ToString() + " ROWS FETCH NEXT " + limite.ToString() + " ROWS ONLY";

            return query;
        }

        private void SetarSelectRelatorioConsultaTransportadoresSemContrato(string propriedade, int codigoDinamico, ref string select, ref string groupBy, ref string joins, bool count)
        {
            switch (propriedade)
            {
                case "CodigoIntegracao":
                    if (!select.Contains(" CodigoIntegracao, "))
                    {
                        select += "Empresa.EMP_CODIGO_INTEGRACAO CodigoIntegracao, ";
                        groupBy += "Empresa.EMP_CODIGO_INTEGRACAO, ";
                    }
                    break;
                case "NumeroUltimoContrato":
                    if (!select.Contains(" NumeroUltimoContrato, "))
                    {
                        select += "(select top 1 CFT_NUMERO_EMBARCADOR from T_CONTRATO_FRETE_TRANSPORTADOR where EMP_CODIGO = Empresa.Emp_Codigo order by T_CONTRATO_FRETE_TRANSPORTADOR.CFT_DATA_FINAL desc) as NumeroUltimoContrato, ";
                        if (!groupBy.Contains(" Empresa.EMP_CODIGO, "))
                        {
                            groupBy += "Empresa.EMP_CODIGO, ";
                        }
                    }
                    break;
                case "DescricaoSituacaoContrato":
                    if (!select.Contains(" SituacaoContrato, "))
                    {
                        select += "(select top 1 CFT_ATIVO from T_CONTRATO_FRETE_TRANSPORTADOR where EMP_CODIGO = Empresa.Emp_Codigo order by T_CONTRATO_FRETE_TRANSPORTADOR.CFT_DATA_FINAL desc) as SituacaoContrato, ";
                        if (!groupBy.Contains(" Empresa.EMP_CODIGO, "))
                        {
                            groupBy += "Empresa.EMP_CODIGO, ";
                        }
                    }
                    break;
                case "Vigencia":
                    if (!select.Contains(" DataInicial, "))
                        select += @"(select top 1 _contrato.CFT_DATA_INICIAL 
                                    from T_CONTRATO_FRETE_TRANSPORTADOR _contrato
                                    where 
                                        EMP_CODIGO = Empresa.Emp_Codigo 
                                    order by _contrato.CFT_DATA_FINAL desc) as DataInicial, ";

                    if (!select.Contains(" DataFinal, "))
                        select += @"(select top 1 _contrato.CFT_DATA_FINAL 
                                    from T_CONTRATO_FRETE_TRANSPORTADOR _contrato
                                    where 
                                        EMP_CODIGO = Empresa.Emp_Codigo 
                                    order by _contrato.CFT_DATA_FINAL desc) as DataFinal, ";

                    if (!groupBy.Contains(" Empresa.EMP_CODIGO, "))
                        groupBy += "Empresa.EMP_CODIGO, ";
                    break;
                case "TipoUltimoContrato":
                    if (!select.Contains(" TipoUltimoContrato, "))
                    {
                        select += "(select top 1 t_tipo_contrato_frete.tcf_descricao from T_CONTRATO_FRETE_TRANSPORTADOR inner join t_tipo_contrato_frete on t_tipo_contrato_frete.tcf_codigo = T_CONTRATO_FRETE_TRANSPORTADOR.tcf_codigo where EMP_CODIGO = Empresa.Emp_Codigo order by T_CONTRATO_FRETE_TRANSPORTADOR.CFT_DATA_FINAL desc) as TipoUltimoContrato, ";
                        if (!groupBy.Contains(" Empresa.EMP_CODIGO, "))
                        {
                            groupBy += "Empresa.EMP_CODIGO, ";
                        }
                    }
                    break;
                case "RazaoSocial":
                    if (!select.Contains(" RazaoSocial, "))
                    {
                        select += "Empresa.EMP_RAZAO RazaoSocial, ";
                        groupBy += "Empresa.EMP_RAZAO, ";
                    }
                    break;
                case "NomeFantasia":
                    if (!select.Contains(" NomeFantasia, "))
                    {
                        select += "Empresa.EMP_FANTASIA NomeFantasia, ";
                        groupBy += "Empresa.EMP_FANTASIA, ";
                    }
                    break;
                case "CNPJ":
                    if (!select.Contains(" CNPJTransportadorSemFormato, "))
                    {
                        select += "Empresa.EMP_CNPJ CNPJTransportadorSemFormato, ";
                        groupBy += "Empresa.EMP_CNPJ, ";
                    }
                    break;
                case "InscricaoEstadual":
                    if (!select.Contains(" InscricaoEstadual, "))
                    {
                        select += "Empresa.EMP_INSCRICAO InscricaoEstadual, ";
                        groupBy += "Empresa.EMP_INSCRICAO, ";
                    }
                    break;
                case "Situacao":
                    if (!select.Contains(" Status, "))
                    {
                        select += "Empresa.EMP_STATUS Status, ";
                        groupBy += "Empresa.EMP_STATUS, ";
                    }
                    break;
                default:
                    break;
            }
        }

        private void SetarWhereRelatorioConsultaTransportadoresSemContrato(ref string where, ref string groupBy, ref string joins, DateTime dataInicial, DateTime dataFinal, Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoAtivoPesquisa situacao, bool somenteContrato, bool somenteSemContrato)
        {
            string pattern = "yyyy-MM-dd";

            where += " AND Empresa.emp_codigo not in (select EMP_CODIGO from T_CONTRATO_FRETE_TRANSPORTADOR where CFT_DATA_INICIAL <= '" + DateTime.Now.ToString(pattern) + "' and CFT_DATA_FINAL >= '" + DateTime.Now.ToString(pattern) + "' and CFT_ATIVO = 1)"; // SQL-INJECTION-SAFE

            if (dataInicial != DateTime.MinValue || dataFinal != DateTime.MinValue)
            {
                where += " AND Empresa.emp_codigo in (select EMP_CODIGO from T_CONTRATO_FRETE_TRANSPORTADOR where 1 = 1 ";

                if (dataInicial != DateTime.MinValue)
                    where += "and CFT_DATA_INICIAL >= '" + dataInicial.ToString(pattern) + "' ";
                if (dataFinal != DateTime.MinValue)
                    where += "and CFT_DATA_FINAL < '" + dataFinal.AddDays(1).ToString(pattern) + "' ";

                where += " and CFT_CODIGO = (select top 1 CFT_CODIGO from T_CONTRATO_FRETE_TRANSPORTADOR where EMP_CODIGO = Empresa.Emp_Codigo order by T_CONTRATO_FRETE_TRANSPORTADOR.CFT_DATA_FINAL desc)) ";
            }

            if (situacao != Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoAtivoPesquisa.Todos)
            {
                where += " AND Empresa.EMP_STATUS = " + (situacao == Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoAtivoPesquisa.Ativo ? "'A'" : "'I'");
            }

            if (somenteContrato)
            {
                if (!joins.Contains(" contrato "))
                    joins += " left join T_CONTRATO_FRETE_TRANSPORTADOR contrato on contrato.emp_codigo = Empresa.emp_codigo ";

                where += " and contrato.CFT_CODIGO is not null ";
            }

            if (somenteSemContrato)
            {
                if (!joins.Contains(" contrato "))
                    joins += " left join T_CONTRATO_FRETE_TRANSPORTADOR contrato on contrato.emp_codigo = Empresa.emp_codigo ";

                where += " and contrato.CFT_CODIGO is null ";
            }
        }

        #endregion

        #region Métodos Privados

        private IQueryable<Dominio.Entidades.Embarcador.Frete.ContratoFreteTransportador> ConsultarContratoFranquia(int codigoTransportador, string descricao, DateTime? periodoInicio, DateTime? periodoFim)
        {
            var consultaContratoFreteTransportador = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Frete.ContratoFreteTransportador>()
                .Where(o => (o.Transportador == null || o.Transportador.Codigo == codigoTransportador) && o.Ativo);

            if (!string.IsNullOrWhiteSpace(descricao))
                consultaContratoFreteTransportador = consultaContratoFreteTransportador.Where(o => o.Descricao.Contains(descricao));

            if (periodoInicio.HasValue && periodoFim.HasValue)
            {
                consultaContratoFreteTransportador = consultaContratoFreteTransportador.Where(o =>
                    (
                        o.DataInicial <= periodoInicio.Value.Date ||
                        (o.DataInicial >= periodoInicio.Value.Date && o.DataInicial <= periodoFim.Value.Date.Add(DateTime.MaxValue.TimeOfDay))
                    ) &&
                    (
                        o.DataFinal >= periodoFim.Value.Date.Add(DateTime.MaxValue.TimeOfDay) ||
                        (o.DataFinal >= periodoInicio.Value.Date && o.DataFinal <= periodoFim.Value.Date.Add(DateTime.MaxValue.TimeOfDay))
                    )
                );
            }
            else if (periodoInicio.HasValue)
                consultaContratoFreteTransportador = consultaContratoFreteTransportador.Where(o => o.DataInicial <= periodoInicio.Value.Date && o.DataFinal >= periodoInicio.Value.Date.Add(DateTime.MaxValue.TimeOfDay));
            else if (periodoFim.HasValue)
                consultaContratoFreteTransportador = consultaContratoFreteTransportador.Where(o => o.DataInicial <= periodoFim.Value.Date && o.DataFinal >= periodoFim.Value.Date.Add(DateTime.MaxValue.TimeOfDay));

            return consultaContratoFreteTransportador;
        }

        private IQueryable<Dominio.Entidades.Veiculo> QueryBuscarVeiculosEmContratos(DateTime data, List<int> veiculos)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Frete.ContratoFreteTransportador>();
            var queryVeiculos = this.SessionNHiBernate.Query<Dominio.Entidades.Veiculo>()
                .Where(obj => veiculos.Contains(obj.Codigo));

            query = from obj in query
                    where
                        obj.DataFinal.Date >= data && obj.DataInicial <= data
                        && (obj.Situacao == Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoContratoFreteTransportador.Aprovado
                        || obj.Situacao == Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoContratoFreteTransportador.AgAprovacao)
                        && obj.Ativo

                    select obj;

            queryVeiculos = queryVeiculos.Where(obj => query.Any(o => o.Veiculos.Any(vei => vei.Veiculo.Codigo == obj.Codigo)));

            return queryVeiculos;
        }

        #endregion
    }
}