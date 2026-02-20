using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Dynamic.Core;
using Dominio.ObjetosDeValor.Embarcador.Enumeradores;

namespace Repositorio.Embarcador.Financeiro
{
    public class ConciliacaoTransportadorEmpresa : RepositorioBase<Dominio.Entidades.Embarcador.Financeiro.ConciliacaoTransportadorEmpresa>
    {
        public ConciliacaoTransportadorEmpresa(UnitOfWork unitOfWork) : base(unitOfWork) { }

        public void AdicionarTransportadorNaConciliacao(Dominio.Entidades.Empresa transportador, Dominio.Entidades.Embarcador.Financeiro.ConciliacaoTransportador conciliacaoTransportador)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Financeiro.ConciliacaoTransportadorEmpresa>();
            query = query.Where(o => o.ConciliacaoTransportador.Codigo == conciliacaoTransportador.Codigo && o.Transportador.Codigo == transportador.Codigo);

            var ligacaoJaExistente = query.FirstOrDefault();

            if (ligacaoJaExistente != null)
            {
                return;
            }

            var ligacao = new Dominio.Entidades.Embarcador.Financeiro.ConciliacaoTransportadorEmpresa
            {
                Transportador = transportador,
                ConciliacaoTransportador = conciliacaoTransportador
            };

            Inserir(ligacao);
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

        private IQueryable<Dominio.Entidades.Embarcador.Financeiro.ConciliacaoTransportador> _Consultar(Dominio.ObjetosDeValor.Embarcador.Financeiro.FiltroPesquisaConciliacaoTransportador filtrosPesquisa)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Financeiro.ConciliacaoTransportador>();

            if (filtrosPesquisa.CodigosTransportador.Count > 0)
                query = query.Where(obj => obj.Transportadores.Any(o => filtrosPesquisa.CodigosTransportador.Contains(o.Codigo)));

            if (filtrosPesquisa.SituacaoConciliacaoTransportador != SituacaoConciliacaoTransportador.Todos)
                query = query.Where(obj => obj.SituacaoConciliacaoTransportador == filtrosPesquisa.SituacaoConciliacaoTransportador);

            return query;
        }

    }
}
