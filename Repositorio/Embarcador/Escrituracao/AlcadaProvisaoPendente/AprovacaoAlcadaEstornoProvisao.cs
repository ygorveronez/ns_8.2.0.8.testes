using System.Collections.Generic;
using System.Linq;
using NHibernate.Linq;
using System.Linq.Dynamic.Core;
using System;
using Dominio.ObjetosDeValor.Embarcador.Enumeradores;

namespace Repositorio.Embarcador.Escrituracao.AlcadaProvisaoPendente
{
    public sealed class AprovacaoAlcadaEstornoProvisao : RegraAutorizacao.AprovacaoAlcada<
        Dominio.Entidades.Embarcador.Escrituracao.AlcadasProvisaoPendente.AprovacaoAlcadaEstornoProvisao,
        Dominio.Entidades.Embarcador.Escrituracao.AlcadasProvisaoPendente.RegraAutorizacaoProvisaoPendente,
        Dominio.Entidades.Embarcador.Escrituracao.CancelamentoProvisao
    >
    {
        #region Construtores

        public AprovacaoAlcadaEstornoProvisao(UnitOfWork unitOfWork) : base(unitOfWork) { }

        #endregion

        #region Métodos Privados

        private IQueryable<Dominio.Entidades.Embarcador.Escrituracao.AlcadasProvisaoPendente.AprovacaoAlcadaEstornoProvisao> Consultar(Dominio.ObjetosDeValor.Embarcador.Escrituracao.FiltroPesquisaEstornoProvisaoAprovacao filtrosPesquisa)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Escrituracao.AlcadasProvisaoPendente.AprovacaoAlcadaEstornoProvisao>();

            if (filtrosPesquisa.CodigoCarga > 0)
                query = query.Where(o => o.OrigemAprovacao.Carga.Codigo == filtrosPesquisa.CodigoCarga);

            if (!string.IsNullOrWhiteSpace(filtrosPesquisa.NumeroProvisao))
                query = query.Where(o => o.OrigemAprovacao.DocumentosProvisao.Any(obj => obj.Stage.NumeroFolha == filtrosPesquisa.NumeroProvisao));

            if (filtrosPesquisa.NumeroLote > 0)
                query = query.Where(o => o.OrigemAprovacao.Numero == filtrosPesquisa.NumeroLote);

            if (filtrosPesquisa.DataGeracaoLoteInicial != DateTime.MinValue)
                query = query.Where(o => o.OrigemAprovacao.DataCriacao >= filtrosPesquisa.DataGeracaoLoteInicial || o.TermoQuitacaoFinanceiro.DataInicial >= filtrosPesquisa.DataGeracaoLoteInicial);

            if (filtrosPesquisa.DataGeracaoLoteFinal != DateTime.MinValue)
                query = query.Where(o => o.OrigemAprovacao.DataCriacao <= filtrosPesquisa.DataGeracaoLoteFinal || o.TermoQuitacaoFinanceiro.DataFinal >= filtrosPesquisa.DataGeracaoLoteInicial);

            if (filtrosPesquisa.CodigoTransportador > 0)
                query = query.Where(o => o.OrigemAprovacao.Empresa.Codigo == filtrosPesquisa.CodigoTransportador);

            if (filtrosPesquisa.CpfCnpjTomador > 0)
            {
                query = query.Where(o =>
                    o.OrigemAprovacao.Carga.Pedidos.Any(p =>
                        (p.TipoTomador == Dominio.Enumeradores.TipoTomador.Remetente && p.Pedido.Remetente.CPF_CNPJ == filtrosPesquisa.CpfCnpjTomador) ||
                        (p.TipoTomador == Dominio.Enumeradores.TipoTomador.Destinatario && p.Pedido.Destinatario.CPF_CNPJ == filtrosPesquisa.CpfCnpjTomador) ||
                        (p.TipoTomador == Dominio.Enumeradores.TipoTomador.Outros && p.Tomador.CPF_CNPJ == filtrosPesquisa.CpfCnpjTomador) ||
                        (p.TipoTomador == Dominio.Enumeradores.TipoTomador.Recebedor && p.Recebedor.CPF_CNPJ == filtrosPesquisa.CpfCnpjTomador) ||
                        (p.TipoTomador == Dominio.Enumeradores.TipoTomador.Expedidor && p.Expedidor.CPF_CNPJ == filtrosPesquisa.CpfCnpjTomador)
                    )
                );
            }

            if (filtrosPesquisa.SituacaoEstornoProvisaoSolicitacao.HasValue)
            {
                var solicitacao = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Escrituracao.EstornoProvisaoSolicitacao>();
                query = query.Where(o => solicitacao.Any(x => x.EstornoProvisao.Codigo == o.OrigemAprovacao.Codigo && x.Situacao == filtrosPesquisa.SituacaoEstornoProvisaoSolicitacao.Value));
            }


            return query;
        }

        #endregion

        #region Métodos Públicos

        public List<Dominio.Entidades.Embarcador.Escrituracao.AlcadasProvisaoPendente.AprovacaoAlcadaEstornoProvisao> BuscarDesbloqueadaIncluindoTermo(int codigoOrigem, int codigoUsuario)
        {
            var aprovacoes = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Escrituracao.AlcadasProvisaoPendente.AprovacaoAlcadaEstornoProvisao>()
                .Where(o => !o.Bloqueada && o.TermoQuitacaoFinanceiro != null ? o.Codigo == codigoOrigem : o.OrigemAprovacao.Codigo == codigoOrigem);

            if (codigoUsuario > 0)
                aprovacoes = aprovacoes.Where(aprovacao => aprovacao.Usuario.Codigo == codigoUsuario);

            return aprovacoes.ToList();
        }

        public List<Dominio.Entidades.Embarcador.Escrituracao.AlcadasProvisaoPendente.AprovacaoAlcadaEstornoProvisao> Consultar(Dominio.ObjetosDeValor.Embarcador.Escrituracao.FiltroPesquisaEstornoProvisaoAprovacao filtrosPesquisa, Dominio.ObjetosDeValor.Embarcador.Consulta.ParametroConsulta parametrosConsulta)
        {
            var query = Consultar(filtrosPesquisa);

            query = query
                .Fetch(o => o.OrigemAprovacao).ThenFetch(o => o.Carga).ThenFetch(o => o.Filial)
                .Fetch(o => o.OrigemAprovacao).ThenFetch(o => o.Carga).ThenFetch(o => o.ModeloVeicularCarga)
                .Fetch(o => o.OrigemAprovacao).ThenFetch(o => o.Carga).ThenFetch(o => o.TipoDeCarga)
                .Fetch(o => o.OrigemAprovacao).ThenFetch(o => o.Carga).ThenFetch(o => o.Empresa)
                .Fetch(o => o.OrigemAprovacao).ThenFetch(o => o.Carga).ThenFetch(o => o.PortoDestino)
                .Fetch(o => o.OrigemAprovacao).ThenFetch(o => o.Carga).ThenFetch(o => o.PortoOrigem)
                .Fetch(o => o.OrigemAprovacao).ThenFetch(o => o.Carga).ThenFetch(o => o.DadosSumarizados);

            return ObterLista(query, parametrosConsulta);
        }

        public int ContarConsulta(Dominio.ObjetosDeValor.Embarcador.Escrituracao.FiltroPesquisaEstornoProvisaoAprovacao filtrosPesquisa)
        {
            var query = Consultar(filtrosPesquisa);

            return query.Count();
        }

        public Dominio.Entidades.Embarcador.Escrituracao.AlcadasProvisaoPendente.AprovacaoAlcadaEstornoProvisao BuscarPorEstornoProvisao(int codigoEstornoProvisao, int codigoUsuario)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Escrituracao.AlcadasProvisaoPendente.AprovacaoAlcadaEstornoProvisao>();
            query = query.Where(x => (x.OrigemAprovacao.Codigo == codigoEstornoProvisao || x.TermoQuitacaoFinanceiro.Codigo == codigoEstornoProvisao));

            if (codigoUsuario > 0)
                query = query.Where(x => x.Usuario.Codigo == codigoUsuario);

            return query.FirstOrDefault();
        }

        public List<Dominio.Entidades.Embarcador.Escrituracao.AlcadasProvisaoPendente.AprovacaoAlcadaEstornoProvisao> BuscarPorEstornoProvisaoLista(int codigoEstornoProvisao, int codigoUsuario)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Escrituracao.AlcadasProvisaoPendente.AprovacaoAlcadaEstornoProvisao>();
            query = query.Where(x => (x.OrigemAprovacao.Codigo == codigoEstornoProvisao || x.TermoQuitacaoFinanceiro.Codigo == codigoEstornoProvisao));

            if (codigoUsuario > 0)
                query = query.Where(x => x.Usuario.Codigo == codigoUsuario);

            return query.ToList();
        }

        public List<Dominio.Entidades.Embarcador.Escrituracao.AlcadasProvisaoPendente.AprovacaoAlcadaEstornoProvisao> BuscarPendentesIncluindoTermo(int codigo, int codigoUsuario)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Escrituracao.AlcadasProvisaoPendente.AprovacaoAlcadaEstornoProvisao>();
            query = query.Where(x => x.TermoQuitacaoFinanceiro != null ? x.Codigo == codigo : x.OrigemAprovacao.Codigo == codigo && x.Usuario.Codigo == codigoUsuario && (x.Situacao == Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoAlcadaRegra.Pendente) &&
                    !x.Bloqueada);
            return query.ToList();

        }
        public int ContarAprovacoes(int codiTermoQuitacao, SituacaoAlcadaRegra situacao)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Escrituracao.AlcadasProvisaoPendente.AprovacaoAlcadaEstornoProvisao>();
            query = from obj in query where obj.TermoQuitacaoFinanceiro.Codigo == codiTermoQuitacao && situacao == obj.Situacao select obj;

            return query.Count();
        }

        public bool ExisteDuplicidade(Dominio.Entidades.Embarcador.Escrituracao.AlcadasProvisaoPendente.AprovacaoAlcadaEstornoProvisao aprovacaoAlcada)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Escrituracao.AlcadasProvisaoPendente.AprovacaoAlcadaEstornoProvisao>();
            query = query.Where(i => i.TermoQuitacaoFinanceiro.Codigo != aprovacaoAlcada.TermoQuitacaoFinanceiro.Codigo);
            query = query.Where(i => i.Usuario == aprovacaoAlcada.Usuario &&
                             i.Situacao == aprovacaoAlcada.Situacao &&
                             i.NumeroAprovadores == aprovacaoAlcada.NumeroAprovadores);

            return query.Any();
        }
        #endregion

        #region Termo quitacao Financeiro
        public List<Dominio.Entidades.Embarcador.Escrituracao.AlcadasProvisaoPendente.AprovacaoAlcadaEstornoProvisao> BuscarAutorizacoesPorTermoQuitacaoFinanceiro(int codigoTermo, Dominio.ObjetosDeValor.Embarcador.Consulta.ParametroConsulta parametrosConsulta)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Escrituracao.AlcadasProvisaoPendente.AprovacaoAlcadaEstornoProvisao>();
            query = from obj in query where obj.TermoQuitacaoFinanceiro.Codigo == codigoTermo select obj;

            return ObterLista(query, parametrosConsulta);
        }
        #endregion
    }
}
