using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Dynamic.Core;

namespace Repositorio.Embarcador.Frete
{
    public class RegrasAutorizacaoTabelaFrete : RepositorioBase<Dominio.Entidades.Embarcador.Frete.RegrasAutorizacaoTabelaFrete>
    {
        #region Construtores

        public RegrasAutorizacaoTabelaFrete(UnitOfWork unitOfWork) : base(unitOfWork) { }

        #endregion

        #region Métodos Privados

        private IQueryable<Dominio.Entidades.Embarcador.Frete.RegrasAutorizacaoTabelaFrete> _ConsultarRegras(DateTime? dataInicio, DateTime? dataFim, Dominio.Entidades.Usuario aprovador, int tabela, string descricao, Dominio.ObjetosDeValor.Embarcador.Enumeradores.EtapaAutorizacaoTabelaFrete etapaAutorizacaoTabelaFrete, Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoAtivoPesquisa situacao)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Frete.RegrasAutorizacaoTabelaFrete>();
            var result = from obj in query select obj;

            if (dataInicio.HasValue && dataFim.HasValue)
                result = result.Where(o => o.Vigencia >= dataInicio.Value && o.Vigencia < dataFim.Value.AddDays(1));
            else if (dataInicio.HasValue)
                result = result.Where(o => o.Vigencia >= dataInicio.Value);
            else if (dataFim.HasValue)
                result = result.Where(o => o.Vigencia < dataFim.Value.AddDays(1));

            if (aprovador != null)
                result = result.Where(o => o.Aprovadores.Contains(aprovador));

            if (tabela > 0)
                result = result.Where(o => o.TabelaFrete.Codigo == tabela);

            if (!string.IsNullOrWhiteSpace(descricao))
                result = result.Where(o => o.Descricao.Contains(descricao));

            if (etapaAutorizacaoTabelaFrete != Dominio.ObjetosDeValor.Embarcador.Enumeradores.EtapaAutorizacaoTabelaFrete.Todas)
                result = result.Where(o => o.EtapaAutorizacaoTabelaFrete == etapaAutorizacaoTabelaFrete);

            if (situacao == Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoAtivoPesquisa.Ativo)
                result = result.Where(o => o.Ativo);
            else if (situacao == Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoAtivoPesquisa.Inativo)
                result = result.Where(o => !o.Ativo);

            return result;
        }

        #endregion

        #region Métodos Privados

        public Dominio.Entidades.Embarcador.Frete.RegrasAutorizacaoTabelaFrete BuscarPorCodigo(int codigo)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Frete.RegrasAutorizacaoTabelaFrete>();
            var result = from obj in query where obj.Codigo == codigo select obj;
            return result.FirstOrDefault();
        }

        public List<Dominio.Entidades.Embarcador.Frete.RegrasAutorizacaoTabelaFrete> ConsultarRegras(DateTime? dataInicio, DateTime? dataFim, Dominio.Entidades.Usuario aprovador, int tabela, string descricao, Dominio.ObjetosDeValor.Embarcador.Enumeradores.EtapaAutorizacaoTabelaFrete etapaAutorizacaoTabelaFrete, Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoAtivoPesquisa situacao, string propOrdenacao, string dirOrdenacao, int inicioRegistros, int maximoRegistros)
        {
            var result = _ConsultarRegras(dataInicio, dataFim, aprovador, tabela, descricao, etapaAutorizacaoTabelaFrete, situacao);

            if (inicioRegistros > 0)
                result = result.Skip(inicioRegistros);

            if (maximoRegistros > 0)
                result = result.Take(maximoRegistros);

            result = result.OrderBy(propOrdenacao + (dirOrdenacao == "asc" ? " ascending" : " descending"));

            return result.ToList();
        }

        public int ContarConsultaRegras(DateTime? dataInicio, DateTime? dataFim, Dominio.Entidades.Usuario aprovador, int tabela, string descricao, Dominio.ObjetosDeValor.Embarcador.Enumeradores.EtapaAutorizacaoTabelaFrete etapaAutorizacaoTabelaFrete, Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoAtivoPesquisa situacao)
        {
            var result = _ConsultarRegras(dataInicio, dataFim, aprovador, tabela, descricao, etapaAutorizacaoTabelaFrete, situacao);

            return result.Count();
        }
        
        public List<Dominio.Entidades.Embarcador.Frete.RegrasAutorizacaoTabelaFrete> BuscarRegraPorMotivoReajuste(int codigoTabela, int codigoMotivoReajuste, DateTime data, Dominio.ObjetosDeValor.Embarcador.Enumeradores.EtapaAutorizacaoTabelaFrete etapa)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Frete.RegrasMotivoReajuste>();
            var result = from obj in query
                         where
                            (
                                (
                                    (obj.Condicao == Dominio.ObjetosDeValor.Embarcador.Enumeradores.CondicaoAutorizaoValorFrete.IgualA && obj.MotivoReajuste.Codigo == codigoMotivoReajuste)
                                    || (obj.Condicao == Dominio.ObjetosDeValor.Embarcador.Enumeradores.CondicaoAutorizaoValorFrete.DiferenteDe && obj.MotivoReajuste.Codigo != codigoMotivoReajuste)
                                )
                                && (obj.RegrasAutorizacaoTabelaFrete.TabelaFrete == null || obj.RegrasAutorizacaoTabelaFrete.TabelaFrete.Codigo == codigoTabela)
                            )
                         select obj.RegrasAutorizacaoTabelaFrete;

            result = result.Where(o => o.Ativo && o.RegraPorMotivoReajuste && (o.Vigencia >= data || o.Vigencia == null));
            result = result.Where(o => o.EtapaAutorizacaoTabelaFrete == etapa);

            return result.ToList();
        }

        public List<Dominio.Entidades.Embarcador.Frete.RegrasAutorizacaoTabelaFrete> BuscarRegraPorOrigemFrete(int codigoTabela, List<int> codigoOrigemFrete, DateTime data, Dominio.ObjetosDeValor.Embarcador.Enumeradores.EtapaAutorizacaoTabelaFrete etapa)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Frete.RegrasOrigemFrete>();
            var result = from obj in query
                         where
                            (
                                (
                                    (obj.Condicao == Dominio.ObjetosDeValor.Embarcador.Enumeradores.CondicaoAutorizaoValorFrete.IgualA && codigoOrigemFrete.Contains(obj.OrigemFrete.Codigo))
                                    || (obj.Condicao == Dominio.ObjetosDeValor.Embarcador.Enumeradores.CondicaoAutorizaoValorFrete.DiferenteDe && !codigoOrigemFrete.Contains(obj.OrigemFrete.Codigo))
                                )
                                && (obj.RegrasAutorizacaoTabelaFrete.TabelaFrete == null || obj.RegrasAutorizacaoTabelaFrete.TabelaFrete.Codigo == codigoTabela)
                            )
                         select obj.RegrasAutorizacaoTabelaFrete;

            result = result.Where(o => o.Ativo && o.RegraPorOrigemFrete && (o.Vigencia >= data || o.Vigencia == null));
            result = result.Where(o => o.EtapaAutorizacaoTabelaFrete == etapa);

            return result.ToList();
        }

        public List<Dominio.Entidades.Embarcador.Frete.RegrasAutorizacaoTabelaFrete> BuscarRegraPorDestinoFrete(int codigoTabela, List<int> codigoDestinoFrete, DateTime data, Dominio.ObjetosDeValor.Embarcador.Enumeradores.EtapaAutorizacaoTabelaFrete etapa)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Frete.RegrasDestinoFrete>();
            var result = from obj in query
                         where
                            (
                                (
                                    (obj.Condicao == Dominio.ObjetosDeValor.Embarcador.Enumeradores.CondicaoAutorizaoValorFrete.IgualA && codigoDestinoFrete.Contains(obj.DestinoFrete.Codigo))
                                    || (obj.Condicao == Dominio.ObjetosDeValor.Embarcador.Enumeradores.CondicaoAutorizaoValorFrete.DiferenteDe && !codigoDestinoFrete.Contains(obj.DestinoFrete.Codigo))
                                )
                                && (obj.RegrasAutorizacaoTabelaFrete.TabelaFrete == null || obj.RegrasAutorizacaoTabelaFrete.TabelaFrete.Codigo == codigoTabela)
                            )
                         select obj.RegrasAutorizacaoTabelaFrete;

            result = result.Where(o => o.Ativo && o.RegraPorDestinoFrete && (o.Vigencia >= data || o.Vigencia == null));
            result = result.Where(o => o.EtapaAutorizacaoTabelaFrete == etapa);

            return result.ToList();
        }

        public List<Dominio.Entidades.Embarcador.Frete.RegrasAutorizacaoTabelaFrete> BuscarRegraPorTransportadora(int codigoTabela, List<int> codigos, DateTime data, Dominio.ObjetosDeValor.Embarcador.Enumeradores.EtapaAutorizacaoTabelaFrete etapa)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Frete.RegrasTransportador>();
            var result = from obj in query
                         where
                            (
                                (
                                    (obj.Condicao == Dominio.ObjetosDeValor.Embarcador.Enumeradores.CondicaoAutorizaoValorFrete.IgualA && codigos.Contains(obj.Transportador.Codigo))
                                    || (obj.Condicao == Dominio.ObjetosDeValor.Embarcador.Enumeradores.CondicaoAutorizaoValorFrete.DiferenteDe && !codigos.Contains(obj.Transportador.Codigo))
                                )
                                && (obj.RegrasAutorizacaoTabelaFrete.TabelaFrete == null || obj.RegrasAutorizacaoTabelaFrete.TabelaFrete.Codigo == codigoTabela)
                            )
                         select obj.RegrasAutorizacaoTabelaFrete;

            result = result.Where(o => o.Ativo && o.RegraPorTransportador && (o.Vigencia >= data || o.Vigencia == null));
            result = result.Where(o => o.EtapaAutorizacaoTabelaFrete == etapa);

            return result.ToList();
        }

        public List<Dominio.Entidades.Embarcador.Frete.RegrasAutorizacaoTabelaFrete> BuscarRegraPorFilial(int codigoTabela, List<int> codigos, DateTime data, Dominio.ObjetosDeValor.Embarcador.Enumeradores.EtapaAutorizacaoTabelaFrete etapa)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Frete.RegrasFilial>();
            var result = from obj in query
                         where
                            (
                                (
                                    (obj.Condicao == Dominio.ObjetosDeValor.Embarcador.Enumeradores.CondicaoAutorizaoValorFrete.IgualA && codigos.Contains(obj.Filial.Codigo))
                                    || (obj.Condicao == Dominio.ObjetosDeValor.Embarcador.Enumeradores.CondicaoAutorizaoValorFrete.DiferenteDe && !codigos.Contains(obj.Filial.Codigo))
                                )
                                && (obj.RegrasAutorizacaoTabelaFrete.TabelaFrete == null || obj.RegrasAutorizacaoTabelaFrete.TabelaFrete.Codigo == codigoTabela)
                            )
                         select obj.RegrasAutorizacaoTabelaFrete;

            result = result.Where(o => o.Ativo && o.RegraPorFilial && (o.Vigencia >= data || o.Vigencia == null));
            result = result.Where(o => o.EtapaAutorizacaoTabelaFrete == etapa);

            return result.ToList();
        }

        public List<Dominio.Entidades.Embarcador.Frete.RegrasAutorizacaoTabelaFrete> BuscarRegraPorTipoOperacao(int codigoTabela, List<int> codigos, DateTime data, Dominio.ObjetosDeValor.Embarcador.Enumeradores.EtapaAutorizacaoTabelaFrete etapa)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Frete.RegrasTipoOperacao>();
            var result = from obj in query
                         where
                            (
                                (
                                    (obj.Condicao == Dominio.ObjetosDeValor.Embarcador.Enumeradores.CondicaoAutorizaoValorFrete.IgualA && codigos.Contains(obj.TipoOperacao.Codigo))
                                    || (obj.Condicao == Dominio.ObjetosDeValor.Embarcador.Enumeradores.CondicaoAutorizaoValorFrete.DiferenteDe && !codigos.Contains(obj.TipoOperacao.Codigo))
                                )
                                && (obj.RegrasAutorizacaoTabelaFrete.TabelaFrete == null || obj.RegrasAutorizacaoTabelaFrete.TabelaFrete.Codigo == codigoTabela)
                            )
                         select obj.RegrasAutorizacaoTabelaFrete;

            result = result.Where(o => o.Ativo && o.RegraPorTipoOperacao && (o.Vigencia >= data || o.Vigencia == null));
            result = result.Where(o => o.EtapaAutorizacaoTabelaFrete == etapa);

            return result.ToList();
        }

        public List<Dominio.Entidades.Embarcador.Frete.RegrasAutorizacaoTabelaFrete> BuscarRegraPorTipoTabelaFrete(int codigoTabela, DateTime data, Dominio.ObjetosDeValor.Embarcador.Enumeradores.EtapaAutorizacaoTabelaFrete etapa)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Frete.RegrasAutorizacaoTabelaFrete>();
            var result = from obj in query
                         where
                            obj.RegraPorAdValorem == false &&
                            obj.RegraPorDestinoFrete == false &&
                            obj.RegraPorMotivoReajuste == false &&
                            obj.RegraPorOrigemFrete == false &&
                            obj.RegraPorTipoOperacao == false &&
                            obj.RegraPorTransportador == false &&
                            obj.RegraPorValorFrete == false &&
                            obj.RegraPorValorPedagio == false &&
                            obj.RegraPorFilial == false &&
                            (obj.TabelaFrete == null || obj.TabelaFrete.Codigo == codigoTabela)
                         select obj;

            result = result.Where(o => o.Ativo && o.Vigencia >= data || o.Vigencia == null);
            result = result.Where(o => o.EtapaAutorizacaoTabelaFrete == etapa);

            return result.ToList();
        }

        #endregion
    }
}
