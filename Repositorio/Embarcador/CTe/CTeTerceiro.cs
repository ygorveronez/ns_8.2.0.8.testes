using Dominio.Relatorios.Embarcador.ObjetosDeValor;
using NHibernate.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Dynamic.Core;
using System.Threading;
using System.Threading.Tasks;

namespace Repositorio.Embarcador.CTe
{
    public class CTeTerceiro : RepositorioBase<Dominio.Entidades.Embarcador.CTe.CTeTerceiro>
    {
        public CTeTerceiro(UnitOfWork unitOfWork, CancellationToken cancellationToken = default) : base(unitOfWork, cancellationToken) { }

        #region Métodos Públicos

        public Dominio.Entidades.Embarcador.CTe.CTeTerceiro BuscarPorCodigo(int codigo)
        {
            var consultaCTeTerceiro = SessionNHiBernate.Query<Dominio.Entidades.Embarcador.CTe.CTeTerceiro>()
                .Where(obj => obj.Codigo == codigo);

            return consultaCTeTerceiro.FirstOrDefault();
        }

        public List<Dominio.Entidades.Embarcador.CTe.CTeTerceiro> BuscarPorIdentificacaoPacote(string identificacaoPacote)
        {
            var consultaCTeTerceiro = SessionNHiBernate.Query<Dominio.Entidades.Embarcador.CTe.CTeTerceiro>()
                .Where(obj => obj.IdentifacaoPacote == identificacaoPacote && obj.Ativo);

            return consultaCTeTerceiro.ToList();
        }

        public Task<List<Dominio.Entidades.Embarcador.CTe.CTeTerceiro>> BuscarPorIdentificacaoPacoteAsync(string identificacaoPacote, CancellationToken cancellationToken)
        {
            var consultaCTeTerceiro = SessionNHiBernate.Query<Dominio.Entidades.Embarcador.CTe.CTeTerceiro>()
                .Where(obj => obj.IdentifacaoPacote == identificacaoPacote && obj.Ativo == true);

            return consultaCTeTerceiro.ToListAsync(cancellationToken);
        }

        public List<Dominio.Entidades.Embarcador.CTe.CTeTerceiro> BuscarPorIdentificacaoPacote(List<string> identificacoesPacotes)
        {
            IQueryable<Dominio.Entidades.Embarcador.CTe.CTeTerceiro> query = SessionNHiBernate.Query<Dominio.Entidades.Embarcador.CTe.CTeTerceiro>();

            int quantidadeRegistrosConsultarPorVez = 1000;
            int quantidadeConsultas = identificacoesPacotes.Count / quantidadeRegistrosConsultarPorVez;

            List<Dominio.Entidades.Embarcador.CTe.CTeTerceiro> ctesTerceirosRetornar = new List<Dominio.Entidades.Embarcador.CTe.CTeTerceiro>();

            for (int i = 0; i <= quantidadeConsultas; i++)
                ctesTerceirosRetornar.AddRange(query.Where(o => o.Ativo && identificacoesPacotes.Skip(i * quantidadeRegistrosConsultarPorVez).Take(quantidadeRegistrosConsultarPorVez).Contains(o.IdentifacaoPacote)).ToList());

            return ctesTerceirosRetornar;
        }

        public List<Dominio.Entidades.Embarcador.CTe.CTeTerceiro> BuscarPorChave(List<string> chaves)
        {
            List<Dominio.Entidades.Embarcador.CTe.CTeTerceiro> lstCTeTerceiro = new List<Dominio.Entidades.Embarcador.CTe.CTeTerceiro>();
            int quantidadeRegistrosConsultarPorVez = 2000;
            int quantidadeConsultas = chaves.Count / quantidadeRegistrosConsultarPorVez;

            for (int i = 0; i <= quantidadeConsultas; i++)
            {
                var consulta = SessionNHiBernate.Query<Dominio.Entidades.Embarcador.CTe.CTeTerceiro>().Where(x => chaves.Skip(i * quantidadeRegistrosConsultarPorVez).
                Take(quantidadeRegistrosConsultarPorVez).Contains(x.ChaveAcesso) && x.Ativo);
                consulta = consulta.Fetch(x => x.CTesTerceiroNFes);
                lstCTeTerceiro.AddRange(consulta.ToList());
            }
            return lstCTeTerceiro;
        }

        public Dominio.Entidades.Embarcador.CTe.CTeTerceiro BuscarPorChave(string chave)
        {
            var consultaCTeTerceiro = SessionNHiBernate.Query<Dominio.Entidades.Embarcador.CTe.CTeTerceiro>()
                .Where(obj => obj.ChaveAcesso == chave && obj.Ativo);

            return consultaCTeTerceiro.FirstOrDefault();
        }

        public List<Dominio.Entidades.Embarcador.CTe.CTeTerceiro> BuscarTodosPorChave(string chave)
        {
            IQueryable<Dominio.Entidades.Embarcador.CTe.CTeTerceiro> query = SessionNHiBernate.Query<Dominio.Entidades.Embarcador.CTe.CTeTerceiro>();

            query = query.Where(obj => obj.ChaveAcesso == chave);

            return query.ToList();
        }

        public Dominio.Entidades.Embarcador.CTe.CTeTerceiro BuscarPorChave(string chave, bool ativo, List<Dominio.Entidades.Embarcador.CTe.CTeTerceiro> cacheCtesTerceiro = null)
        {
            if (cacheCtesTerceiro != null)
            {
                if (ativo)
                    return cacheCtesTerceiro.Where(obj => obj.Ativo && obj.ChaveAcesso == chave).FirstOrDefault();
                else
                    return cacheCtesTerceiro.Where(obj => obj.ChaveAcesso == chave).FirstOrDefault();
            }
            else
            {
                var consultaCTeTerceiro = SessionNHiBernate.Query<Dominio.Entidades.Embarcador.CTe.CTeTerceiro>();
                consultaCTeTerceiro = consultaCTeTerceiro.Where(obj => obj.ChaveAcesso == chave);
                if (ativo)
                    consultaCTeTerceiro = consultaCTeTerceiro.Where(o => o.Ativo);
                return consultaCTeTerceiro.FirstOrDefault();
            }
        }

        public bool ExistePorCargaCte(int codigoCargaCte)
        {
            string sql = $@"select CteTerceiro.CPS_CODIGO  
                             from T_CTE_TERCEIRO CteTerceiro  
                             join T_CARGA_CTE CargaCte on CargaCte.CCT_CODIGO = CteTerceiro.CCT_CODIGO  
                            where CargaCte.CCT_CODIGO = {codigoCargaCte}  
                            union  
                           select CteTerceiro.CPS_CODIGO  
                             from T_CTE_TERCEIRO CteTerceiro  
                            where exists (  
                                      select CargaCteTerceiro.CCT_CODIGO  
                                        from T_CTE_TERCEIRO_CARGA_CTE CargaCteTerceiro  
                                       where CargaCteTerceiro.CPS_CODIGO = CteTerceiro.CPS_CODIGO  
                                         and CargaCteTerceiro.CCT_CODIGO = {codigoCargaCte}  
                                  )";

            NHibernate.ISQLQuery query = this.SessionNHiBernate.CreateSQLQuery(sql);

            return query.List<int>().Any();
        }

        public List<Dominio.Entidades.Embarcador.CTe.CTeTerceiro> BuscarPorCargaEDestino(int codigoCarga, string localidadeDestino, int limite, bool apenasComMesmaLocalidadeCarga = true)
        {
            IQueryable<Dominio.Entidades.Embarcador.Pedidos.PedidoCTeParaSubContratacao> subQueryPedidoCTeSubcontratacao = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Pedidos.PedidoCTeParaSubContratacao>()
                .Where(o => o.CargaPedido.Carga.Codigo == codigoCarga);

            if (apenasComMesmaLocalidadeCarga)
                subQueryPedidoCTeSubcontratacao = subQueryPedidoCTeSubcontratacao.Where(o => o.CTeTerceiro.LocalidadeTerminoPrestacao.Estado.Sigla == localidadeDestino);
            else
                subQueryPedidoCTeSubcontratacao = subQueryPedidoCTeSubcontratacao.Where(o => o.CTeTerceiro.LocalidadeTerminoPrestacao.Estado.Sigla != localidadeDestino);

            return subQueryPedidoCTeSubcontratacao.Select(obj => obj.CTeTerceiro).Take(limite).ToList();
        }

        public Dominio.Entidades.Embarcador.CTe.CTeTerceiro BuscarPorChave(int codigoCTeTerceiro, string chave)
        {
            var consultaCTeTerceiro = SessionNHiBernate.Query<Dominio.Entidades.Embarcador.CTe.CTeTerceiro>()
                .Where(obj => obj.Codigo != codigoCTeTerceiro && obj.ChaveAcesso == chave && obj.Ativo);

            return consultaCTeTerceiro.FirstOrDefault();
        }

        public Dominio.Entidades.Embarcador.CTe.CTeTerceiro BuscarPorNumeroERemetente(int numero, string cpfCnpjRemetenteSemFormatacao)
        {
            var consultaCTeTerceiro = SessionNHiBernate.Query<Dominio.Entidades.Embarcador.CTe.CTeTerceiro>()
                .Where(obj => obj.Numero == numero && obj.Ativo && obj.Remetente.CPF_CNPJ == cpfCnpjRemetenteSemFormatacao);

            return consultaCTeTerceiro.FirstOrDefault();
        }

        public List<Dominio.Entidades.Embarcador.CTe.CTeTerceiro> Consultar(Dominio.ObjetosDeValor.Embarcador.CTe.FiltroConsultaCTeTerceiro filtros, string propOrdenar, string dirOrdenar, int inicio, int limite)
        {
            IQueryable<Dominio.Entidades.Embarcador.CTe.CTeTerceiro> query = ObterQueryConsulta(filtros, propOrdenar, dirOrdenar, inicio, limite);

            return query.Fetch(o => o.LocalidadeInicioPrestacao)
                        .Fetch(o => o.LocalidadeTerminoPrestacao)
                        .Fetch(o => o.Emitente)
                        .Fetch(o => o.Remetente)
                        .Fetch(o => o.Destinatario)
                        .ToList();
        }

        public int ContarConsulta(Dominio.ObjetosDeValor.Embarcador.CTe.FiltroConsultaCTeTerceiro filtros)
        {
            IQueryable<Dominio.Entidades.Embarcador.CTe.CTeTerceiro> query = ObterQueryConsulta(filtros);

            return query.Count();
        }

        public List<Dominio.Entidades.Embarcador.CTe.CTeTerceiro> BuscarPorNumeroPedido(string numeroPedido)
        {
            IQueryable<Dominio.Entidades.Embarcador.CTe.CTeTerceiro> query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.CTe.CTeTerceiro>();
            IQueryable<Dominio.Entidades.Embarcador.Pedidos.PedidoCTeParaSubContratacao> subQueryPedidoCTeSubcontratacao = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Pedidos.PedidoCTeParaSubContratacao>();

            subQueryPedidoCTeSubcontratacao = subQueryPedidoCTeSubcontratacao.Where(o => o.CargaPedido.Carga.SituacaoCarga != Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoCarga.Cancelada &&
                                                                                         o.CargaPedido.Carga.SituacaoCarga != Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoCarga.Anulada);

            query = query.Where(o => o.NumeroPedido == numeroPedido && !subQueryPedidoCTeSubcontratacao.Any(sqp => sqp.CTeTerceiro == o));

            return query.ToList();
        }

        public IQueryable<Dominio.Entidades.Embarcador.CTe.CTeTerceiro> ObterQueryConsulta(Dominio.ObjetosDeValor.Embarcador.CTe.FiltroConsultaCTeTerceiro filtros, string propOrdenar = null, string dirOrdenar = null, int inicio = 0, int limite = 0)
        {
            IQueryable<Dominio.Entidades.Embarcador.CTe.CTeTerceiro> query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.CTe.CTeTerceiro>();

            if (filtros.NumeroInicial > 0)
                query = query.Where(o => o.Numero >= filtros.NumeroInicial);

            if (filtros.NumeroFinal > 0)
                query = query.Where(o => o.Numero <= filtros.NumeroFinal);

            if (filtros.TipoCTe.HasValue && filtros.TipoCTe != Dominio.Enumeradores.TipoCTE.Todos)
                query = query.Where(o => o.TipoCTE == filtros.TipoCTe);

            if (filtros.CPFCNPJEmitente > 0D)
                query = query.Where(o => o.Emitente.Cliente.CPF_CNPJ == filtros.CPFCNPJEmitente);

            if (filtros.CPFCNPJRemetente > 0D)
                query = query.Where(o => o.Remetente.Cliente.CPF_CNPJ == filtros.CPFCNPJRemetente);

            if (filtros.CPFCNPJDestinatario > 0D)
                query = query.Where(o => o.Destinatario.Cliente.CPF_CNPJ == filtros.CPFCNPJDestinatario);

            if (filtros.PossuiOcorrenciaGerada)
            {
                List<Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoOcorrencia> situacoes = new List<Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoOcorrencia>() {
                    Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoOcorrencia.Anulada,
                    Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoOcorrencia.Cancelada
                };

                query = query.Where(o => !o.Ocorrencias.Any(oc => !situacoes.Contains(oc.SituacaoOcorrencia)));
            }

            if (!string.IsNullOrWhiteSpace(propOrdenar) && !string.IsNullOrWhiteSpace(dirOrdenar))
                query = query.OrderBy($"{propOrdenar} {dirOrdenar}");

            if (inicio > 0 || limite > 0)
                query = query.Skip(inicio).Take(limite);

            return query;
        }

        public IList<Dominio.ObjetosDeValor.Embarcador.CTe.NumeroCTeAnteriorPacote> BuscarCTeAnteriorIndentificacaoPacote(List<int> codigoPacotes)
        {
            if (codigoPacotes.Count <= 0)
                return new List<Dominio.ObjetosDeValor.Embarcador.CTe.NumeroCTeAnteriorPacote>();

            string sql = $@"SELECT CTeAnterior.CPS_NUMERO NumeroCTeAnterior, Pacote.PCT_LOG_KEY NumeroPacote
                            FROM T_CTE_TERCEIRO CTeAnterior
                            LEFT JOIN T_PACOTE Pacote ON Pacote.PCT_LOG_KEY = CTeAnterior.CPS_IDENTIFICACAO_PACOTE
                            WHERE Pacote.PCT_CODIGO in ({string.Join(", ", codigoPacotes)})";

            var query = this.SessionNHiBernate.CreateSQLQuery(sql);

            query.SetResultTransformer(new NHibernate.Transform.AliasToBeanResultTransformer(typeof(Dominio.ObjetosDeValor.Embarcador.CTe.NumeroCTeAnteriorPacote)));

            return query.SetTimeout(6000).List<Dominio.ObjetosDeValor.Embarcador.CTe.NumeroCTeAnteriorPacote>();
        }

        public List<Dominio.Entidades.Embarcador.CTe.CTeTerceiro> BuscarTodos(List<string> chavesAcesso)
        {
            return this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.CTe.CTeTerceiro>().Where(x => chavesAcesso.Contains(x.ChaveAcesso))
                .ToList();
        }

        #endregion

        #region Relatórios

        public IList<Dominio.Relatorios.Embarcador.DataSource.CTe.Subcontratacao> ConsultarRelatorioSubcontratacao(Dominio.ObjetosDeValor.Embarcador.CTe.FiltroPesquisaRelatorioSubcontratacao filtrosPesquisa, List<PropriedadeAgrupamento> propriedades, Dominio.ObjetosDeValor.Embarcador.Consulta.ParametroConsulta parametrosConsulta)
        {
            var query = new ConsultaSubcontratacao().ObterSqlPesquisa(filtrosPesquisa, parametrosConsulta, propriedades).CriarSQLQuery(this.SessionNHiBernate);

            query.SetResultTransformer(new NHibernate.Transform.AliasToBeanResultTransformer(typeof(Dominio.Relatorios.Embarcador.DataSource.CTe.Subcontratacao)));

            return query.SetTimeout(600).List<Dominio.Relatorios.Embarcador.DataSource.CTe.Subcontratacao>();
        }

        public int ContarConsultaRelatorioSubcontratacao(Dominio.ObjetosDeValor.Embarcador.CTe.FiltroPesquisaRelatorioSubcontratacao filtrosPesquisa, List<PropriedadeAgrupamento> propriedades)
        {
            var query = new ConsultaSubcontratacao().ObterSqlContarPesquisa(filtrosPesquisa, propriedades).CriarSQLQuery(this.SessionNHiBernate);

            return query.SetTimeout(600).UniqueResult<int>();
        }

        #endregion
    }
}
