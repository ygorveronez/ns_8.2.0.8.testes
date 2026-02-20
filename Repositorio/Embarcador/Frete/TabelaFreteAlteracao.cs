using System.Linq;
using System.Linq.Dynamic.Core;
using Dominio.ObjetosDeValor.Embarcador.Enumeradores;
using System.Collections.Generic;
using System;

namespace Repositorio.Embarcador.Frete
{
    public class TabelaFreteAlteracao : RepositorioBase<Dominio.Entidades.Embarcador.Frete.TabelaFreteAlteracao>
    {
        #region Construtores

        public TabelaFreteAlteracao(UnitOfWork unitOfWork) : base(unitOfWork) { }

        #endregion

        #region Métodos Privados

        private IQueryable<Dominio.Entidades.Embarcador.Frete.TabelaFreteAlteracao> BuscarPorTabelaFreteCliente(int codigoTabelaFreteCliente)
        {
            var consultaTabelaFreteClienteAlteracao = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Frete.TabelaFreteClienteAlteracao>()
                .Where(o => o.TabelaFreteCliente.Codigo == codigoTabelaFreteCliente);

            var consultaTabelaFreteAlteracao = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Frete.TabelaFreteAlteracao>()
                .Where(o => consultaTabelaFreteClienteAlteracao.Any(c => c.TabelaFreteAlteracao.Codigo == o.Codigo));

            return consultaTabelaFreteAlteracao;
        }

        #endregion

        #region Métodos Públicos

        public List<Dominio.Entidades.Embarcador.Frete.TabelaFreteAlteracao> BuscarPorCodigos(List<int> codigos)
        {
            if (codigos.Count < 2000)
            {
                var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Frete.TabelaFreteAlteracao>()
                    .Where(alteracao => codigos.Contains(alteracao.Codigo));

                return query.ToList();
            }

            List<Dominio.Entidades.Embarcador.Frete.TabelaFreteAlteracao> listaRetornar = new List<Dominio.Entidades.Embarcador.Frete.TabelaFreteAlteracao>();

            List<int> listaOriginal = codigos;
            int tamanhoLote = 2000;
            int indiceInicial = 0;

            while (indiceInicial < listaOriginal.Count)
            {
                List<int> lote = listaOriginal.GetRange(indiceInicial, Math.Min(tamanhoLote, listaOriginal.Count - indiceInicial));
                
                var consultaMinima = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Frete.TabelaFreteAlteracao>()
                                    .Where(alteracao => lote.Contains(alteracao.Codigo));

                listaRetornar.AddRange(consultaMinima.ToList());

                indiceInicial += tamanhoLote;
            }

            return listaRetornar;
        }

        public Dominio.Entidades.Embarcador.Frete.TabelaFreteAlteracao BuscarPendentePorTabelaFrete(int codigoTabelaFrete)
        {
            List<SituacaoAlteracaoTabelaFrete> situacoesAlteracaoPendente = SituacaoAlteracaoTabelaFreteHelper.ObterSituacoesAlteracaoPendente();

            var consultaTabelaFreteAlteracao = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Frete.TabelaFreteAlteracao>()
                .Where(o => o.TabelaFrete.Codigo == codigoTabelaFrete && situacoesAlteracaoPendente.Contains(o.SituacaoAlteracao))
                .OrderByDescending(o => o.Numero);

            return consultaTabelaFreteAlteracao.FirstOrDefault();
        }

        public List<Dominio.Entidades.Embarcador.Frete.TabelaFreteAlteracao> BuscarPendentesPorTabelaFrete(int codigoTabelaFrete)
        {
            List<SituacaoAlteracaoTabelaFrete> situacoesAlteracaoPendente = SituacaoAlteracaoTabelaFreteHelper.ObterSituacoesAlteracaoPendente();

            var consultaTabelaFreteAlteracao = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Frete.TabelaFreteAlteracao>()
                .Where(o => o.TabelaFrete.Codigo == codigoTabelaFrete && situacoesAlteracaoPendente.Contains(o.SituacaoAlteracao))
                .OrderByDescending(o => o.Numero);

            return consultaTabelaFreteAlteracao.ToList();
        }

        public Dominio.Entidades.Embarcador.Frete.TabelaFreteAlteracao BuscarPendentePorTabelaFreteCliente(int codigoTabelaFreteCliente)
        {
            List<SituacaoAlteracaoTabelaFrete> situacoesAlteracaoPendente = SituacaoAlteracaoTabelaFreteHelper.ObterSituacoesAlteracaoPendente();
            var consultaTabelaFreteAlteracao = BuscarPorTabelaFreteCliente(codigoTabelaFreteCliente);

            consultaTabelaFreteAlteracao = consultaTabelaFreteAlteracao.Where(o => situacoesAlteracaoPendente.Contains(o.SituacaoAlteracao));

            return consultaTabelaFreteAlteracao.OrderByDescending(o => o.Numero).FirstOrDefault();
        }

        public int BuscarProximoNumeroPorTabelaFrete(int codigoTabelaFrete)
        {
            var consultaTabelaFreteAlteracao = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Frete.TabelaFreteAlteracao>()
                .Where(o => o.TabelaFrete.Codigo == codigoTabelaFrete);

            int? ultimoNumero = consultaTabelaFreteAlteracao.Max(o => (int?)o.Numero);

            return ultimoNumero.HasValue ? ultimoNumero.Value + 1 : 1;
        }

        public Dominio.Entidades.Embarcador.Frete.TabelaFreteAlteracao BuscarUltimaPorTabelaFreteCliente(int codigoTabelaFreteCliente)
        {
            var consultaTabelaFreteAlteracao = BuscarPorTabelaFreteCliente(codigoTabelaFreteCliente);

            return consultaTabelaFreteAlteracao
                .OrderByDescending(o => o.Numero)
                .FirstOrDefault();
        }

        #endregion
    }
}
