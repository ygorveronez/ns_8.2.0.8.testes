using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Dynamic.Core;
using Dominio.ObjetosDeValor.Embarcador.Enumeradores;

namespace Repositorio.Embarcador.Financeiro
{
    public class ConciliacaoTransportador : RepositorioBase<Dominio.Entidades.Embarcador.Financeiro.ConciliacaoTransportador>
    {
        public ConciliacaoTransportador(UnitOfWork unitOfWork) : base(unitOfWork) { }

        #region Métodos Públicos

        /*
         * Obtem o transportador que irá assinar a conciliação. Geralmente é a Matriz, mas caso ela não exista, pode ser outro.
         * */
        public Dominio.Entidades.Empresa ObterTransportadorParaAssinatura(Dominio.Entidades.Embarcador.Financeiro.ConciliacaoTransportador conciliacao)
        {
            var matriz = ObterMatrizConciliacao(conciliacao);
            if (matriz != null)
                return matriz;

            // Se não tem a matriz cadastrada, pegar a filial que tem maior lucro na conciliação
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.ConhecimentoDeTransporteEletronico>();
            query = query.Where(o => o.ConciliacaoTransportador.Codigo == conciliacao.Codigo);
            var data = query.GroupBy(o => o.Empresa.Codigo).Select(g => new
            {
                Transportador = g.Key,
                Valor = g.Sum(s => s.Titulo != null ? s.Titulo.ValorOriginal : 0)
            }).OrderByDescending(o => o.Valor);
            int codigoTransportadorAssinatura = data.FirstOrDefault().Transportador;

            matriz = conciliacao.Transportadores.Where(o => o.Codigo == codigoTransportadorAssinatura).FirstOrDefault();
            return matriz;
        }

        public Dominio.Entidades.Empresa ObterMatrizConciliacao(Dominio.Entidades.Embarcador.Financeiro.ConciliacaoTransportador conciliacao)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Empresa>();
            query = query.Where(o => o.CNPJ.Replace(".", "").Replace("-", "").Replace("/", "").Substring(0, 12) == conciliacao.RaizCnpj + "0001");
            return query.FirstOrDefault();
        }

        public List<Dominio.Entidades.Embarcador.Financeiro.ConciliacaoTransportador> Consultar(Dominio.ObjetosDeValor.Embarcador.Financeiro.FiltroPesquisaConciliacaoTransportador filtrosPesquisa, string propOrdenacao, string dirOrdenacao, int inicioRegistros, int maximoRegistros)
        {
            var query = _Consultar(filtrosPesquisa);

            query = query.OrderBy(propOrdenacao + " " + dirOrdenacao);

            if (inicioRegistros > 0)
                query = query.Skip(inicioRegistros);

            if (maximoRegistros > 0)
                query = query.Take(maximoRegistros);

            return query
                .ToList();
        }

        public int ContarConsulta(Dominio.ObjetosDeValor.Embarcador.Financeiro.FiltroPesquisaConciliacaoTransportador filtrosPesquisa)
        {
            var query = _Consultar(filtrosPesquisa);

            return query.Count();
        }

        public Dominio.Entidades.Embarcador.Financeiro.ConciliacaoTransportador BuscarPorRaizCnpj(string raizCnpj, DateTime diaInicial, DateTime diaFinal, PeriodicidadeConciliacaoTransportador periodicidade)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Financeiro.ConciliacaoTransportador>();
            query = query.Where(o => o.RaizCnpj == raizCnpj
                && o.DataInicial == diaInicial
                && o.DataFinal == diaFinal
                && o.Periodicidade == periodicidade);
            return query.FirstOrDefault();
        }

        public Dominio.Entidades.Embarcador.Financeiro.ConciliacaoTransportador BuscarPorTransportador(int codigoTransportador, DateTime diaInicial, DateTime diaFinal, PeriodicidadeConciliacaoTransportador periodicidade)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Financeiro.ConciliacaoTransportadorEmpresa>();
            query = query.Where(o => o.Transportador.Codigo == codigoTransportador
                && o.ConciliacaoTransportador.DataInicial == diaInicial
                && o.ConciliacaoTransportador.DataFinal == diaFinal
                && o.ConciliacaoTransportador.Periodicidade == periodicidade);
            return query.Select(o => o.ConciliacaoTransportador).FirstOrDefault();
        }

        #endregion

        #region Métodos Privados

        private IQueryable<Dominio.Entidades.Embarcador.Financeiro.ConciliacaoTransportador> _Consultar(Dominio.ObjetosDeValor.Embarcador.Financeiro.FiltroPesquisaConciliacaoTransportador filtrosPesquisa)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Financeiro.ConciliacaoTransportador>();

            if (filtrosPesquisa.CodigosTransportador.Count > 0)
            {
                query = query.Where(obj => obj.Transportadores.Any(o => filtrosPesquisa.CodigosTransportador.Contains(o.Codigo)));
            }

            if (!string.IsNullOrEmpty(filtrosPesquisa.RaizCnpj))
            {
                query = query.Where(o => o.RaizCnpj == filtrosPesquisa.RaizCnpj);
            }

            if (filtrosPesquisa.NumeroCarta > 0)
            {
                query = query.Where(o => o.Codigo == filtrosPesquisa.NumeroCarta);
            }

            if (filtrosPesquisa.DataInicial != DateTime.MinValue)
            {
                query = query.Where(o => o.DataInicial >= filtrosPesquisa.DataInicial);
            }

            if (filtrosPesquisa.DataFinal != DateTime.MinValue)
            {
                query = query.Where(o => o.DataFinal < filtrosPesquisa.DataFinal.AddDays(1));
            }

            if (filtrosPesquisa.DataFinalAssinatura != DateTime.MinValue)
            {
                query = query.Where(o => o.DataAssinaturaAnuencia < filtrosPesquisa.DataFinal.AddDays(1));
            }

            if (filtrosPesquisa.DataInicialAssinatura != DateTime.MinValue)
            {
                query = query.Where(o => o.DataAssinaturaAnuencia >= filtrosPesquisa.DataInicialAssinatura);
            }

            if (filtrosPesquisa.AnuenciaDisponivelInicio != DateTime.MinValue)
            {
                query = query.Where(o => o.DataAnuenciaDisponivel >= filtrosPesquisa.AnuenciaDisponivelInicio);
            }

            if (filtrosPesquisa.AnuenciaDisponivelFinal != DateTime.MinValue)
            {
                query = query.Where(o => o.DataAnuenciaDisponivel < filtrosPesquisa.AnuenciaDisponivelFinal.AddDays(1));
            }

            if (filtrosPesquisa.SituacaoConciliacaoTransportador != SituacaoConciliacaoTransportador.Todos)
                query = query.Where(obj => obj.SituacaoConciliacaoTransportador == filtrosPesquisa.SituacaoConciliacaoTransportador);

            return query;
        }

        #endregion
    }
}
