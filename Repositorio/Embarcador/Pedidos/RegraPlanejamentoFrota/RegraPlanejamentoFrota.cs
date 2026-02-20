using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Dynamic.Core;

namespace Repositorio.Embarcador.Pedidos.RegraPlanejamentoFrota
{
    public class RegraPlanejamentoFrota : RepositorioBase<Dominio.Entidades.Embarcador.Pedidos.RegraPlanejamentoFrota.RegraPlanejamentoFrota>
    {
        #region Atributos

        UnitOfWork unitOfWork;

        #endregion

        #region Construtores

        public RegraPlanejamentoFrota(UnitOfWork unitOfWork) : base(unitOfWork)
        {
            this.unitOfWork = unitOfWork;
        }

        #endregion Construtores

        #region Métodos Públicos

        public List<Dominio.Entidades.Embarcador.Pedidos.RegraPlanejamentoFrota.RegraPlanejamentoFrota> BuscarRegrasAtivas(DateTime dataAtual)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Pedidos.RegraPlanejamentoFrota.RegraPlanejamentoFrota>();
            query = query.Where(c => c.Ativo == true && (c.VigenciaInicial <= dataAtual.AddDays(-1) || !c.VigenciaInicial.HasValue) && (c.VigenciaFinal >= dataAtual.AddDays(1) || !c.VigenciaFinal.HasValue));
            return query.ToList();
        }

        public Dominio.Entidades.Embarcador.Pedidos.RegraPlanejamentoFrota.RegraPlanejamentoFrota BuscarPorCodigo(int codigo)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Pedidos.RegraPlanejamentoFrota.RegraPlanejamentoFrota>();
            var result = from obj in query where obj.Codigo == codigo select obj;
            return result.FirstOrDefault();
        }

        public List<Dominio.Entidades.Embarcador.Pedidos.RegraPlanejamentoFrota.RegraPlanejamentoFrota> Consultar(Dominio.ObjetosDeValor.Embarcador.Pedido.FiltroPesquisaRegraPlanejamentoFrota filtrosPesquisa, string propOrdenar, string dirOrdena, int inicio, int limite)
        {
            var result = MontarQueryConsulta(filtrosPesquisa);

            return result.OrderBy(propOrdenar + (dirOrdena == "asc" ? " ascending" : " descending"))
                .Skip(inicio)
                .Take(limite)
                .ToList();
        }

        public int ContarConsulta(Dominio.ObjetosDeValor.Embarcador.Pedido.FiltroPesquisaRegraPlanejamentoFrota filtrosPesquisa)
        {
            var result = MontarQueryConsulta(filtrosPesquisa);

            return result.Count();
        }

        private IQueryable<Dominio.Entidades.Embarcador.Pedidos.RegraPlanejamentoFrota.RegraPlanejamentoFrota> MontarQueryConsulta(Dominio.ObjetosDeValor.Embarcador.Pedido.FiltroPesquisaRegraPlanejamentoFrota filtrosPesquisa)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Pedidos.RegraPlanejamentoFrota.RegraPlanejamentoFrota>();

            if (filtrosPesquisa.NumeroSequencial > 0)
            {
                query = query.Where(o => o.NumeroSequencial == filtrosPesquisa.NumeroSequencial);
            }

            if (!string.IsNullOrWhiteSpace(filtrosPesquisa.Descricao))
            {
                query = query.Where(o => o.Descricao.Contains(filtrosPesquisa.Descricao));
            }

            if (filtrosPesquisa.Ativo.HasValue)
            {
                if (filtrosPesquisa.Ativo == Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoAtivoPesquisa.Ativo)
                    query = query.Where(o => o.Ativo == true);
                else if (filtrosPesquisa.Ativo == Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoAtivoPesquisa.Inativo)
                    query = query.Where(o => o.Ativo == false);
            }

            if (filtrosPesquisa.VigenciaInicial.HasValue)
            {
                query = query.Where(o => o.VigenciaInicial.Value >= filtrosPesquisa.VigenciaInicial.Value);
            }

            if (filtrosPesquisa.VigenciaFinal.HasValue)
            {
                query = query.Where(o => o.VigenciaFinal.Value <= filtrosPesquisa.VigenciaFinal.Value);
            }

            if(filtrosPesquisa.GrupoPessoa > 0)
            {
                query = query.Where(o => o.GrupoPessoas.Any(gp=> gp.Codigo == filtrosPesquisa.GrupoPessoa));
            }

            if (filtrosPesquisa.CidadeOrigem > 0)
            {
                query = query.Where(o => o.Origens.Any(or => or.Codigo == filtrosPesquisa.CidadeOrigem));
            }

            if (!string.IsNullOrWhiteSpace(filtrosPesquisa.EstadoOrigem))
            {
                query = query.Where(o => o.EstadosOrigem.Any(eo=> eo.Sigla == filtrosPesquisa.EstadoOrigem));
            }

            if (filtrosPesquisa.CidadeDestino > 0)
            {
                query = query.Where(o => o.Destinos.Any(de => de.Codigo == filtrosPesquisa.CidadeDestino));
            }

            if (!string.IsNullOrWhiteSpace(filtrosPesquisa.EstadoDestino))
            {
                query = query.Where(o => o.EstadosDestino.Any(ed => ed.Sigla == filtrosPesquisa.EstadoDestino));
            }

            if (filtrosPesquisa.TiposOperacao.Count > 0) {
                query = query.Where(o => o.TiposOperacao.Any(tp => filtrosPesquisa.TiposOperacao.Contains(tp.Codigo)));
            }

            if (filtrosPesquisa.TipoCarga.Count > 0)
            {
                query = query.Where(o => o.TiposDeCarga.Any(tc => filtrosPesquisa.TipoCarga.Contains(tc.Codigo)));
            }

            if (filtrosPesquisa.CentroResultdo.Count > 0)
            {
                query = query.Where(o => o.CentrosResultado.Any(cr => filtrosPesquisa.CentroResultdo.Contains(cr.Codigo)));
            }            
            
            if (filtrosPesquisa.ModeloVeicular.Count > 0)
            {
                query = query.Where(o => o.ModelosVeicularesCarga.Any(mv => filtrosPesquisa.ModeloVeicular.Contains(mv.Codigo)));
            }
            
            if (filtrosPesquisa.NivelCooperado.Count > 0)
            {
                query = query.Where(o => o.NiveisCooperados.Any(nc => filtrosPesquisa.NivelCooperado.Contains(nc.Codigo)));
            }

            return query;
        }

        public int ObterProximoNumeroSequencial()
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Pedidos.RegraPlanejamentoFrota.RegraPlanejamentoFrota>();

            int? ultimoNumero = query.Max(o => (int?)o.NumeroSequencial);

            return ultimoNumero.HasValue ? ultimoNumero.Value + 1 : 1;
        }

        #endregion
    }
}