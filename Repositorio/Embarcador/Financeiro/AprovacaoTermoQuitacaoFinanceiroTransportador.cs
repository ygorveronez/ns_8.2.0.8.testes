using System.Collections.Generic;
using System.Linq;
using Dominio.ObjetosDeValor.Embarcador.Enumeradores;

namespace Repositorio.Embarcador.Financeiro
{
    public class AprovacaoTermoQuitacaoFinanceiroTransportador : RepositorioBase<Dominio.Entidades.Embarcador.Financeiro.AprovacaoTermoQuitacaoFinanceiroTransportador>
    {
        #region Construtores
        public AprovacaoTermoQuitacaoFinanceiroTransportador(Repositorio.UnitOfWork unitOfWork) : base(unitOfWork) { }
        #endregion

        #region Metodos Publicos
        public List<Dominio.Entidades.Embarcador.Financeiro.AprovacaoTermoQuitacaoFinanceiroTransportador> BuscarPorTransportador(int codigoTransportador, int codigoTermo)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Financeiro.AprovacaoTermoQuitacaoFinanceiroTransportador>();
            var resultado = query.Where(x => x.Transportador.Codigo == codigoTransportador && x.TermoQuitacaoFinanceiro.Codigo == codigoTermo);
            return resultado.ToList();

        }

        public List<Dominio.Entidades.Embarcador.Financeiro.AprovacaoTermoQuitacaoFinanceiroTransportador> BuscarPorTermoQuitacao( int codigoTermo)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Financeiro.AprovacaoTermoQuitacaoFinanceiroTransportador>();
            var resultado = query.Where(x => x.TermoQuitacaoFinanceiro.Codigo == codigoTermo);
            return resultado.ToList();

        }

        public Dominio.Entidades.Embarcador.Financeiro.AprovacaoTermoQuitacaoFinanceiroTransportador BuscarPrincipalPorTermoQuitacao(int codigoTermo)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Financeiro.AprovacaoTermoQuitacaoFinanceiroTransportador>();
            var resultado = query.Where(x => x.TermoQuitacaoFinanceiro.Codigo == codigoTermo);
            return resultado.FirstOrDefault();
        }

        public List<Dominio.Entidades.Embarcador.Financeiro.AprovacaoTermoQuitacaoFinanceiroTransportador> BuscarPendentesNaoNotificadasAposIntervaloDias(int intervaloDias, int periodoInicial, int periodoFinal)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Financeiro.AprovacaoTermoQuitacaoFinanceiroTransportador>();
            return query
                .Where(x => x.Situacao == Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoAprovacaoTermoQuitacaoTransportador.Pendente).ToList()
                .Where(x => x.DataUltimaNotificacaoEmail == null ||
                        (x.DataUltimaNotificacaoEmail.Value - System.DateTime.Now).TotalDays >= intervaloDias &&
                        (x.DataUltimaNotificacaoEmail.Value - System.DateTime.Now).TotalDays >= periodoInicial &&
                        (x.TermoQuitacaoFinanceiro.DataCriacao.Value - System.DateTime.Now).TotalDays <= periodoFinal).ToList();
        }

        public bool ExistePendenteMatrizTransportador(int codigoTransportador)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Financeiro.AprovacaoTermoQuitacaoFinanceiroTransportador>();
            var transportador = new Empresa(UnitOfWork).BuscarPorCodigo(codigoTransportador);
            List<int> codigosMatrizes = new List<int>();

            if (transportador.Matriz != null && transportador.Matriz.Count > 0)
                codigosMatrizes.AddRange(transportador.Matriz.Select(x => x.Codigo).ToList());
            else
                codigosMatrizes.Add(transportador.Codigo);

            return query.Any(x => x.Situacao == SituacaoAprovacaoTermoQuitacaoTransportador.Pendente && codigosMatrizes.Contains(x.Transportador.Codigo));
        }

        #endregion


        #region Metodos Privados

        #endregion
    }
}
