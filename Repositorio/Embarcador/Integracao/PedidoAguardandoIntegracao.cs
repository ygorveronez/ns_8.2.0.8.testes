using System.Linq;
using System.Linq.Dynamic.Core;
using System;
using System.Collections.Generic;
using Dominio.ObjetosDeValor.Embarcador.Enumeradores;

namespace Repositorio.Embarcador.Integracao
{
    public class PedidoAguardandoIntegracao : RepositorioBase<Dominio.Entidades.Embarcador.Integracao.PedidoAguardandoIntegracao>
    {
        public PedidoAguardandoIntegracao(UnitOfWork unitOfWork) : base(unitOfWork) { }

        public Dominio.Entidades.Embarcador.Integracao.PedidoAguardandoIntegracao BuscarPorCodigo(long codigo)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Integracao.PedidoAguardandoIntegracao>();
            query = query.Where(o => o.Codigo == codigo);
            return query.FirstOrDefault();
        }

        public Dominio.Entidades.Embarcador.Integracao.PedidoAguardandoIntegracao BuscarPorCodigoArquivo(int codigoArquivo)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Integracao.PedidoAguardandoIntegracao>();

            var result = from obj in query where obj.ArquivosTransacao.Any(o => o.Codigo == codigoArquivo) select obj;

            return result.FirstOrDefault();
        }


        public List<Dominio.Entidades.Embarcador.Integracao.PedidoAguardandoIntegracao> Consultar(Dominio.ObjetosDeValor.Embarcador.PedidoAguardandoIntegracao.FiltroPesquisaPedidoAguardandoIntegracao filtrosPesquisa, string propOrdenacao, string dirOrdenacao, int inicioRegistros, int maximoRegistros)
        {
            var result = Consultar(filtrosPesquisa);

            if (maximoRegistros > 0)
                return result
               .OrderBy(propOrdenacao + (dirOrdenacao == "asc" ? " ascending" : " descending")).Skip(inicioRegistros).Take(maximoRegistros).ToList();
            else
                return result
                    .OrderBy(propOrdenacao + (dirOrdenacao == "asc" ? " ascending" : " descending")).ToList();
        }

        public int ContarConsulta(Dominio.ObjetosDeValor.Embarcador.PedidoAguardandoIntegracao.FiltroPesquisaPedidoAguardandoIntegracao filtrosPesquisa)
        {
            var result = Consultar(filtrosPesquisa);

            return result.Count();
        }

        public List<Dominio.Entidades.Embarcador.Integracao.PedidoAguardandoIntegracao> BuscarPorSituacoes(List<SituacaoPedidoAguardandoIntegracao> situacoes)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Integracao.PedidoAguardandoIntegracao>();
            query = query.Where(o => situacoes.Contains(o.SituacaoIntegracao) && o.NumeroTentativas < 3);
            return query.ToList();
        }

        public List<Dominio.Entidades.Embarcador.Integracao.PedidoAguardandoIntegracao> BuscarListaPorSituacoesETipoIntegracao(List<SituacaoPedidoAguardandoIntegracao> situacoes, TipoIntegracao tipoIntegracao, int limite, int numeroTentativas)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Integracao.PedidoAguardandoIntegracao>();
            query = query.Where(o => situacoes.Contains(o.SituacaoIntegracao) && o.TipoIntegracao == tipoIntegracao && o.NumeroTentativas < numeroTentativas);

            if (limite > 0)
                return query.Take(limite).ToList();
            else
                return query.ToList();
        }

        public List<long> BuscarPorSituacoesETipoIntegracao(List<SituacaoPedidoAguardandoIntegracao> situacoes, TipoIntegracao tipoIntegracao, int limite, int numeroTentativas)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Integracao.PedidoAguardandoIntegracao>();
            query = query.Where(o => situacoes.Contains(o.SituacaoIntegracao) && o.TipoIntegracao == tipoIntegracao && o.NumeroTentativas < numeroTentativas);

            if (limite > 0)
                return query.Take(limite).Select(o => o.Codigo).ToList();
            else
                return query.Select(o => o.Codigo).ToList();
        }

        public Dominio.Entidades.Embarcador.Integracao.PedidoAguardandoIntegracao BuscarPorIdTipo(string id, TipoIntegracao tipo, Dominio.Enumeradores.TipoIntegracaoEmillenium tipoIntegracaoEmillenium)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Integracao.PedidoAguardandoIntegracao>();
            query = query.Where(o => o.IdIntegracao == id && o.TipoIntegracao == tipo && o.TipoIntegracaoEmillenium == tipoIntegracaoEmillenium);
            return query.FirstOrDefault();
        }

        public List<Dominio.Entidades.Embarcador.Integracao.PedidoAguardandoIntegracao> BuscarPendentesCarga(string numeroCarga)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Integracao.PedidoAguardandoIntegracao>();
            query = query.Where(o => o.NumeroCarga == numeroCarga && (o.SituacaoIntegracao == SituacaoPedidoAguardandoIntegracao.AgGerarCarga || o.SituacaoIntegracao == SituacaoPedidoAguardandoIntegracao.ProblemaGerarCarga));
            return query.ToList();
        }

        private IQueryable<Dominio.Entidades.Embarcador.Integracao.PedidoAguardandoIntegracao> Consultar(Dominio.ObjetosDeValor.Embarcador.PedidoAguardandoIntegracao.FiltroPesquisaPedidoAguardandoIntegracao filtrosPesquisa)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Integracao.PedidoAguardandoIntegracao>();

            if (!string.IsNullOrEmpty(filtrosPesquisa.Pesquisa))
            {
                query = query.Where(o => o.IdIntegracao == filtrosPesquisa.Pesquisa);
            }

            if (filtrosPesquisa.SituacaoIntegracao.HasValue)
            {
                query = query.Where(o => o.SituacaoIntegracao == filtrosPesquisa.SituacaoIntegracao.Value);
            }

            if (filtrosPesquisa.TipoIntegracao != null)
            {
                query = query.Where(o => o.TipoIntegracao == filtrosPesquisa.TipoIntegracao);
            }

            if (filtrosPesquisa.DataInicio != DateTime.MinValue)
            {
                query = query.Where(o => o.DataCriacaoPedido >= filtrosPesquisa.DataInicio);
            }

            if (filtrosPesquisa.DataFim != DateTime.MinValue)
            {
                query = query.Where(o => o.DataCriacaoPedido <= filtrosPesquisa.DataFim.AddHours(23).AddMinutes(59).AddSeconds(59));
            }

            if (filtrosPesquisa.DataEmbarqueInicio != DateTime.MinValue)
            {
                query = query.Where(o => o.UltimaDataEmbarqueLista >= filtrosPesquisa.DataEmbarqueInicio);
            }

            if (filtrosPesquisa.DataEmbarqueFim != DateTime.MinValue)
            {
                query = query.Where(o => o.UltimaDataEmbarqueLista <= filtrosPesquisa.DataEmbarqueFim.AddHours(23).AddMinutes(59).AddSeconds(59));
            }


            return query;
        }

        public void deletarPorCodigoIntegracao(long codigo)
        {
            string sqlQuery = "delete from T_PEDIDO_AGUARDADO_INTEGRACAO_ARQUIVO_ARQUIVO where pai_codigo = :codigo";
            SessionNHiBernate.CreateSQLQuery(sqlQuery).SetInt64("codigo", codigo).ExecuteUpdate();
            string sqlQuery2 = "delete from T_PEDIDO_AGUARDADO_INTEGRACAO where PAI_CODIGO = :codigo";
            SessionNHiBernate.CreateSQLQuery(sqlQuery2).SetInt64("codigo", codigo).ExecuteUpdate();
        }

        public int ContarPorTipoESituacao(Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoIntegracao tipoIntegracao, SituacaoPedidoAguardandoIntegracao situacao, DateTime dataReferencia)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Integracao.PedidoAguardandoIntegracao>();
            query = query.Where(o => o.TipoIntegracao == tipoIntegracao && o.SituacaoIntegracao == situacao && o.CreatedAt <= dataReferencia);
            return query.Count();
        }

    }
}
