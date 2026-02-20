using NHibernate.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Dynamic.Core;

namespace Repositorio.Embarcador.Frete
{
    public class AjusteTabelaFrete : RepositorioBase<Dominio.Entidades.Embarcador.Frete.AjusteTabelaFrete>
    {
        public AjusteTabelaFrete(UnitOfWork unitOfWork) : base(unitOfWork) { }

        public Dominio.Entidades.Embarcador.Frete.AjusteTabelaFrete BuscarPorCodigo(int codigo)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Frete.AjusteTabelaFrete>();

            query = query.Where(o => o.Codigo == codigo);

            return query.Fetch(obj => obj.TabelaFrete).FirstOrDefault();
        }

        public List<Dominio.Entidades.Embarcador.Frete.AjusteTabelaFrete> BuscarPorCodigos(List<int> codigos)
        {
            var consultaTabelaFreteAlteracao = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Frete.AjusteTabelaFrete>()
                .Where(alteracao => codigos.Contains(alteracao.Codigo));

            return consultaTabelaFreteAlteracao.ToList();
        }

        public Dominio.Entidades.Embarcador.Frete.AjusteTabelaFrete BuscarPorVigencia(int codigoVigencia)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Frete.AjusteTabelaFrete>();

            query = query.Where(o => o.Vigencia.Codigo == codigoVigencia && o.Situacao != Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoAjusteTabelaFrete.RejeitadaAutorizacao && o.Situacao != Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoAjusteTabelaFrete.Cancelado);

            return query.Fetch(obj => obj.TabelaFrete).FirstOrDefault();
        }


        public List<Dominio.Entidades.Embarcador.Frete.AjusteTabelaFrete> Consultar(int codigoTabelaFrete, DateTime dataInicialCriacao, DateTime dataFinalCriacao, int codigoVigencia, Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoAjusteTabelaFrete situacao, string propOrdenar, string dirOrdenar, int inicio, int limite)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Frete.AjusteTabelaFrete>();

            if (codigoTabelaFrete > 0)
                query = query.Where(o => o.TabelaFrete.Codigo == codigoTabelaFrete);

            if (codigoVigencia > 0)
                query = query.Where(o => o.Vigencia.Codigo == codigoVigencia);

            if (dataInicialCriacao != DateTime.MinValue)
                query = query.Where(o => o.DataCriacao >= dataInicialCriacao.Date);

            if (dataFinalCriacao != DateTime.MinValue)
                query = query.Where(o => o.DataCriacao < dataFinalCriacao.AddDays(1).Date);

            if (situacao != Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoAjusteTabelaFrete.Todas)
                query = query.Where(o => o.Situacao == situacao);

            if (limite == 0)
                return query.Fetch(o => o.TabelaFrete).OrderBy(propOrdenar + " " + dirOrdenar).Skip(inicio).ToList();
            else
                return query.Fetch(o => o.TabelaFrete).OrderBy(propOrdenar + " " + dirOrdenar).Skip(inicio).Take(limite).ToList();
        }

        public int ContarConsulta(int codigoTabelaFrete, DateTime dataInicialCriacao, DateTime dataFinalCriacao, int codigoVigencia, Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoAjusteTabelaFrete situacao)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Frete.AjusteTabelaFrete>();

            if (codigoTabelaFrete > 0)
                query = query.Where(o => o.TabelaFrete.Codigo == codigoTabelaFrete);

            if (codigoVigencia > 0)
                query = query.Where(o => o.Vigencia.Codigo == codigoVigencia);

            if (dataInicialCriacao != DateTime.MinValue)
                query = query.Where(o => o.DataCriacao >= dataInicialCriacao.Date);

            if (dataFinalCriacao != DateTime.MinValue)
                query = query.Where(o => o.DataCriacao < dataFinalCriacao.AddDays(1).Date);

            if (situacao != Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoAjusteTabelaFrete.Todas)
                query = query.Where(o => o.Situacao == situacao);

            return query.Count();
        }

        public int BuscarUltimoNumero()
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Frete.AjusteTabelaFrete>();

            return query.Max(o => (int?)o.Numero) ?? 0;
        }

        public List<Dominio.Entidades.Usuario> Responsavel(int codigoTabelaFrete)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Frete.AjusteTabelaFreteAutorizacao>();
            var queryAgrupado = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Frete.AjusteTabelaFreteAutorizacao>();

            queryAgrupado = queryAgrupado.Where(o => o.Usuario != null);

            var resultAgrupado = (from obj in queryAgrupado
                                  where obj.AjusteTabelaFrete.Codigo == codigoTabelaFrete
                                  group obj by obj.Usuario.Codigo into g
                                  select g.Key).ToList();

            var result = from obj in query
                         where obj.AjusteTabelaFrete.Codigo == codigoTabelaFrete && resultAgrupado.Contains(obj.Usuario.Codigo)
                         select obj.Usuario;

            return result.ToList();
        }

        public List<Dominio.Entidades.Embarcador.Frete.AjusteTabelaFrete> BuscarParaProcessamentoPorSituacao(Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoAjusteTabelaFrete situacao, int limite)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Frete.AjusteTabelaFrete>();

            var result = from obj in query where obj.Situacao == situacao select obj;

            return result.Take(limite).ToList();
        }

        public List<int> BuscarCodigosParaProcessamentoPorSituacao(Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoAjusteTabelaFrete situacao, int limite)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Frete.AjusteTabelaFrete>();

            var result = from obj in query where obj.Situacao == situacao select obj;

            return result.Take(limite).Select(o => o.Codigo).ToList();
        }

        public List<Dominio.Entidades.Embarcador.Frete.AjusteTabelaFrete> ObterPorTabelasFrete(List<int> codigoTabelaFrete)
        {
            var consultaAjusteTabelaFrete = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Frete.AjusteTabelaFrete>()
                .Where(o =>
                    codigoTabelaFrete.Contains(o.TabelaFrete.Codigo) &&
                    o.Situacao != Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoAjusteTabelaFrete.Cancelado &&
                    o.Situacao != Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoAjusteTabelaFrete.Finalizado
                );

            return consultaAjusteTabelaFrete.ToList();
        }

        public List<Dominio.Entidades.Embarcador.Frete.TabelaFreteCliente> ObterAjustePorTabelaFreteCliente(List<int> codigosTabelaFreteCliente)
        {
            var consultaAjusteTabelaFrete = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Frete.TabelaFreteCliente>()
                .Where(o =>
                    codigosTabelaFreteCliente.Contains(o.TabelaOriginaria.Codigo) &&
                    o.Tipo == Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoTabelaFreteCliente.Ajuste &&
                    o.AjusteTabelaFrete.Situacao != Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoAjusteTabelaFrete.Cancelado &&
                    o.AjusteTabelaFrete.Situacao != Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoAjusteTabelaFrete.Finalizado
                );

            return consultaAjusteTabelaFrete
                .Fetch(obj => obj.TabelaOriginaria)
                .Fetch(obj => obj.AjusteTabelaFrete)
                .ToList();
        }
    }
}
