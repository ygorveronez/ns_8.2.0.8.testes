using Dominio.ObjetosDeValor.Embarcador.Enumeradores;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Dynamic.Core;

namespace Repositorio.Embarcador.Frete
{
    public class AjusteTabelaFreteAutorizacao : RepositorioBase<Dominio.Entidades.Embarcador.Frete.AjusteTabelaFreteAutorizacao>
    {
        #region Construtores

        public AjusteTabelaFreteAutorizacao(UnitOfWork unitOfWork) : base(unitOfWork) { }

        #endregion

        #region Métodos Privados

        private IQueryable<Dominio.Entidades.Embarcador.Frete.AjusteTabelaFrete> Consultar(Dominio.ObjetosDeValor.Embarcador.Frete.FiltroPesquisaAjusteTabelaFreteAprovacao filtrosPesquisa)
        {
            var consultaAjusteTabelaFrete = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Frete.AjusteTabelaFrete>();
            var consultaAjusteTabelaFreteAutorizacao = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Frete.AjusteTabelaFreteAutorizacao>()
                .Where(o => !o.Bloqueada);

            if (filtrosPesquisa.CodigoTabelaFrete > 0)
                consultaAjusteTabelaFrete = consultaAjusteTabelaFrete.Where(obj => obj.TabelaFrete.Codigo == filtrosPesquisa.CodigoTabelaFrete);

            if (filtrosPesquisa.CodigoTransportador > 0)
                consultaAjusteTabelaFrete = consultaAjusteTabelaFrete.Where(obj => obj.Empresas.Any(emp => emp.Codigo == filtrosPesquisa.CodigoTransportador) || obj.TabelaFrete.Transportadores.Any(tab => tab.Codigo == filtrosPesquisa.CodigoTransportador));

            if (filtrosPesquisa.CodigoTipoOperacao > 0)
                consultaAjusteTabelaFrete = consultaAjusteTabelaFrete.Where(obj => obj.TiposOperacao.Any(op => op.Codigo == filtrosPesquisa.CodigoTipoOperacao));

            if (filtrosPesquisa.DataInicio.HasValue)
                consultaAjusteTabelaFrete = consultaAjusteTabelaFrete.Where(obj => obj.DataCriacao >= filtrosPesquisa.DataInicio.Value.Date);

            if (filtrosPesquisa.DataFim.HasValue)
                consultaAjusteTabelaFrete = consultaAjusteTabelaFrete.Where(obj => obj.DataCriacao <= filtrosPesquisa.DataFim.Value.Date.Add(DateTime.MaxValue.TimeOfDay));

            if (filtrosPesquisa.Situacao != SituacaoAjusteTabelaFrete.Todas)
                consultaAjusteTabelaFrete = consultaAjusteTabelaFrete.Where(obj => obj.Situacao == filtrosPesquisa.Situacao);

            if (filtrosPesquisa.EtapaAjuste != EtapaAjusteTabelaFrete.Todas)
                consultaAjusteTabelaFrete = consultaAjusteTabelaFrete.Where(obj => obj.Etapa == filtrosPesquisa.EtapaAjuste);

            bool situacaoPendentes = filtrosPesquisa.Situacao == SituacaoAjusteTabelaFrete.AgAprovacao;

            if (situacaoPendentes)
            {
                SituacaoAjusteTabelaFreteAutorizacao situacaoPendente = SituacaoAjusteTabelaFreteAutorizacao.Pendente;

                consultaAjusteTabelaFreteAutorizacao = consultaAjusteTabelaFreteAutorizacao.Where(o => o.Situacao == situacaoPendente);

                if (filtrosPesquisa.EtapaAutorizacao != EtapaAutorizacaoTabelaFrete.Todas)
                    consultaAjusteTabelaFreteAutorizacao = consultaAjusteTabelaFreteAutorizacao.Where(o => o.EtapaAutorizacaoTabelaFrete == filtrosPesquisa.EtapaAutorizacao && o.Situacao == SituacaoAjusteTabelaFreteAutorizacao.Pendente);
            }
            else if (filtrosPesquisa.EtapaAutorizacao != EtapaAutorizacaoTabelaFrete.Todas)
                consultaAjusteTabelaFreteAutorizacao = consultaAjusteTabelaFreteAutorizacao.Where(o => o.RegrasAutorizacaoTabelaFrete.EtapaAutorizacaoTabelaFrete == filtrosPesquisa.EtapaAutorizacao);

            if (filtrosPesquisa.CodigoUsuario > 0)
            {
                consultaAjusteTabelaFreteAutorizacao = consultaAjusteTabelaFreteAutorizacao.Where(o => o.Usuario != null);
                consultaAjusteTabelaFreteAutorizacao = consultaAjusteTabelaFreteAutorizacao.Where(o => o.Usuario.Codigo == filtrosPesquisa.CodigoUsuario);
            }

            if (filtrosPesquisa.TipoAprovadorRegra.HasValue)
                consultaAjusteTabelaFreteAutorizacao = consultaAjusteTabelaFreteAutorizacao.Where(o => ((TipoAprovadorRegra?)o.TipoAprovadorRegra ?? TipoAprovadorRegra.Usuario) == filtrosPesquisa.TipoAprovadorRegra.Value);

            return consultaAjusteTabelaFrete.Where(o => consultaAjusteTabelaFreteAutorizacao.Where(a => a.AjusteTabelaFrete.Codigo == o.Codigo).Any());
        }

        #endregion

        #region Métodos Públicos

        public Dominio.Entidades.Embarcador.Frete.AjusteTabelaFreteAutorizacao BuscarPorCodigo(int codigo)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Frete.AjusteTabelaFreteAutorizacao>();
            var result = from obj in query where obj.Codigo == codigo select obj;
            return result.FirstOrDefault();
        }

        public List<Dominio.Entidades.Embarcador.Frete.AjusteTabelaFreteAutorizacao> BuscarPorCodigoAjusteEUsuario(int codigo, int usuario)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Frete.AjusteTabelaFreteAutorizacao>();
            var result = from obj in query where obj.AjusteTabelaFrete.Codigo == codigo && obj.Usuario.Codigo == usuario && !obj.Bloqueada select obj;
            return result.ToList();
        }

        public List<Dominio.Entidades.Embarcador.Frete.AjusteTabelaFreteAutorizacao> BuscarPendentesBloqueadas(int codigoAjuste)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Frete.AjusteTabelaFreteAutorizacao>();
            var result = from obj in query
                         where
                             obj.AjusteTabelaFrete.Codigo == codigoAjuste
                             && obj.Situacao == Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoAjusteTabelaFreteAutorizacao.Pendente
                             && obj.Bloqueada
                         select obj;

            return result.ToList();
        }

        public List<Dominio.Entidades.Embarcador.Frete.AjusteTabelaFreteAutorizacao> BuscarPendentesDesbloqueadas(int codigoAjuste)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Frete.AjusteTabelaFreteAutorizacao>();
            var result = from obj in query
                         where
                             obj.AjusteTabelaFrete.Codigo == codigoAjuste
                             && obj.Situacao == Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoAjusteTabelaFreteAutorizacao.Pendente
                             && !obj.Bloqueada
                         select obj;

            return result.ToList();
        }

        public List<Dominio.Entidades.Embarcador.Frete.AjusteTabelaFreteAutorizacao> BuscarPorAjusteUsuario(int codigo, int usuario)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Frete.AjusteTabelaFreteAutorizacao>();
            var result = from obj in query where obj.AjusteTabelaFrete.Codigo == codigo && !obj.Bloqueada select obj;

            if (usuario > 0)
                result = result.Where(o => o.Usuario.Codigo == usuario);

            return result.ToList();
        }

        public int ContarAutorizacaoPorAjuste(int codigo)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Frete.AjusteTabelaFreteAutorizacao>();
            var result = from obj in query where obj.AjusteTabelaFrete.Codigo == codigo select obj;

            return result.Count();
        }

        public int ContarAprovacoesPorAjuste(int codigo)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Frete.AjusteTabelaFreteAutorizacao>();
            var result = from obj in query
                         where
                            obj.AjusteTabelaFrete.Codigo == codigo &&
                            obj.Situacao == Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoAjusteTabelaFreteAutorizacao.Aprovada
                         select obj;

            return result.Count();
        }

        public int ContarReprovacoesPorAjuste(int codigo)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Frete.AjusteTabelaFreteAutorizacao>();
            var result = from obj in query
                         where
                            obj.AjusteTabelaFrete.Codigo == codigo &&
                            obj.Situacao == Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoAjusteTabelaFreteAutorizacao.Rejeitada
                         select obj;

            return result.Count();
        }

        public List<Dominio.Entidades.Embarcador.Frete.RegrasAutorizacaoTabelaFrete> BuscarRegrasDesbloqueadas(int codigoAjuste)
        {
            var aprovacoes = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Frete.AjusteTabelaFreteAutorizacao>()
                .Where(o => o.AjusteTabelaFrete.Codigo == codigoAjuste && !o.Bloqueada);

            var regras = aprovacoes
                .Where(aprovacao => aprovacao.RegrasAutorizacaoTabelaFrete != null)
                .Select(aprovacao => aprovacao.RegrasAutorizacaoTabelaFrete)
                .Distinct()
                .ToList();

            return regras;
        }

        public int ContarRejeitadas(int codigoAjuste, int codigoRegra)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Frete.AjusteTabelaFreteAutorizacao>();
            var resut = from obj in query
                        where
                            obj.AjusteTabelaFrete.Codigo == codigoAjuste
                            && obj.Situacao == Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoAjusteTabelaFreteAutorizacao.Rejeitada
                            && obj.RegrasAutorizacaoTabelaFrete.Codigo == codigoRegra
                        select obj;

            return resut.Count();
        }

        public int ContarPendentes(int codigoAjuste, int codigoRegra)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Frete.AjusteTabelaFreteAutorizacao>();
            var resut = from obj in query
                        where
                            obj.AjusteTabelaFrete.Codigo == codigoAjuste
                            && obj.Situacao == Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoAjusteTabelaFreteAutorizacao.Pendente
                            && obj.RegrasAutorizacaoTabelaFrete.Codigo == codigoRegra
                        select obj;

            return resut.Count();
        }

        public int ContarAprovacoesAjuste(int codigoAjuste, int codigoRegra)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Frete.AjusteTabelaFreteAutorizacao>();
            var resut = from obj in query
                        where
                            obj.AjusteTabelaFrete.Codigo == codigoAjuste
                            && obj.Situacao == Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoAjusteTabelaFreteAutorizacao.Aprovada
                            && obj.RegrasAutorizacaoTabelaFrete.Codigo == codigoRegra
                        select obj;

            return resut.Count();
        }

        public bool VerificarSePodeAprovar(int codigoAjuste, int codigoRegra, int codigoUsuario)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Frete.AjusteTabelaFreteAutorizacao>();
            var resut = from obj in query
                        where
                            obj.Codigo == codigoRegra
                            && obj.AjusteTabelaFrete.Codigo == codigoAjuste
                            && obj.Usuario.Codigo == codigoUsuario
                            && obj.Situacao == Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoAjusteTabelaFreteAutorizacao.Pendente
                        select obj;

            return resut.Count() > 0;
        }

        public List<Dominio.Entidades.Embarcador.Frete.AjusteTabelaFreteAutorizacao> ConsultarAutorizacoesPorAjusteEEtapa(int codigoAjuste, Dominio.ObjetosDeValor.Embarcador.Enumeradores.EtapaAutorizacaoTabelaFrete? etapa, string propOrdenacao, string dirOrdenacao, int inicioRegistros, int maximoRegistros)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Frete.AjusteTabelaFreteAutorizacao>();
            var result = from obj in query where obj.AjusteTabelaFrete.Codigo == codigoAjuste select obj;

            if (etapa.HasValue)
                result = result.Where(obj => obj.EtapaAutorizacaoTabelaFrete == etapa);

            return result.OrderBy(propOrdenacao + (dirOrdenacao == "asc" ? " ascending" : " descending")).Skip(inicioRegistros).Take(maximoRegistros).ToList();
        }

        public int ContarConsultaAutorizacoesPorAjusteEEtapa(int codigoAjuste, Dominio.ObjetosDeValor.Embarcador.Enumeradores.EtapaAutorizacaoTabelaFrete? etapa)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Frete.AjusteTabelaFreteAutorizacao>();
            var result = from obj in query where obj.AjusteTabelaFrete.Codigo == codigoAjuste select obj;

            if (etapa.HasValue)
                result = result.Where(obj => obj.EtapaAutorizacaoTabelaFrete == etapa);

            return result.Count();
        }

        public List<Dominio.Entidades.Embarcador.Frete.AjusteTabelaFrete> Consultar(Dominio.ObjetosDeValor.Embarcador.Frete.FiltroPesquisaAjusteTabelaFreteAprovacao filtrosPesquisa, Dominio.ObjetosDeValor.Embarcador.Consulta.ParametroConsulta parametrosPesquisa)
        {
            var consultaAjusteTabelaFrete = Consultar(filtrosPesquisa);

            return ObterLista(consultaAjusteTabelaFrete, parametrosPesquisa);
        }

        public int ContarConsulta(Dominio.ObjetosDeValor.Embarcador.Frete.FiltroPesquisaAjusteTabelaFreteAprovacao filtrosPesquisa)
        {
            var consultaAjusteTabelaFrete = Consultar(filtrosPesquisa);

            return consultaAjusteTabelaFrete.Count();
        }

        public Dominio.Entidades.Embarcador.Frete.AjusteTabelaFreteAutorizacao BuscarPorGuid(string guid)
        {
            var consultaAjusteTabelaFreteAutorizacao = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Frete.AjusteTabelaFreteAutorizacao>()
                .Where(o => o.GuidTabelaFrete == guid);

            return consultaAjusteTabelaFreteAutorizacao.FirstOrDefault();
        }

        #endregion
    }
}
