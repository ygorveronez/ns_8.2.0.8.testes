using System.Collections.Generic;
using System.Linq;
using System.Linq.Dynamic.Core;
using Dominio.ObjetosDeValor.Embarcador.Enumeradores;

namespace Repositorio.Embarcador.Frete.Pontuacao
{
    public sealed class FechamentoPontuacao : RepositorioBase<Dominio.Entidades.Embarcador.Frete.Pontuacao.FechamentoPontuacao>
    {
        #region Construtores

        public FechamentoPontuacao(UnitOfWork unitOfWork) : base(unitOfWork) { }

        #endregion

        #region Métodos Privados

        private IQueryable<Dominio.Entidades.Embarcador.Frete.Pontuacao.FechamentoPontuacao> Consultar(Dominio.ObjetosDeValor.Embarcador.Frete.FiltroPesquisaFechamentoPontuacao filtrosPesquisa)
        {
            var consultaFechamentoPontuacao = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Frete.Pontuacao.FechamentoPontuacao>();

            if (filtrosPesquisa.Ano > 0)
                consultaFechamentoPontuacao = consultaFechamentoPontuacao.Where(o => o.Ano == filtrosPesquisa.Ano);

            if (filtrosPesquisa.Mes.HasValue)
                consultaFechamentoPontuacao = consultaFechamentoPontuacao.Where(o => o.Mes == filtrosPesquisa.Mes);

            if (filtrosPesquisa.Numero > 0)
                consultaFechamentoPontuacao = consultaFechamentoPontuacao.Where(o => o.Numero == filtrosPesquisa.Numero);

            if (filtrosPesquisa.Situacao.HasValue)
                consultaFechamentoPontuacao = consultaFechamentoPontuacao.Where(o => o.Situacao == filtrosPesquisa.Situacao);

            return consultaFechamentoPontuacao;
        }

        #endregion

        #region Métodos Públicos

        public List<Dominio.Entidades.Embarcador.Frete.Pontuacao.FechamentoPontuacao> BuscarPorAguardandoFinalizacao(int limiteRegistros)
        {
            var consultaFechamentoPontuacao = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Frete.Pontuacao.FechamentoPontuacao>()
                .Where(o => o.Situacao == SituacaoFechamentoPontuacao.AguardandoFinalizacao);

            return consultaFechamentoPontuacao.Take(limiteRegistros).ToList();
        }

        public Dominio.Entidades.Embarcador.Frete.Pontuacao.FechamentoPontuacao BuscarPorAnoEMes(int ano, Mes mes)
        {
            var consultaFechamentoPontuacao = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Frete.Pontuacao.FechamentoPontuacao>()
                .Where(o => 
                    o.Ano == ano &&
                    o.Mes == mes &&
                    o.Situacao != SituacaoFechamentoPontuacao.Cancelado
                );

            return consultaFechamentoPontuacao.FirstOrDefault();
        }

        public int BuscarProximoNumero()
        {
            var consultaFechamentoPontuacao = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Frete.Pontuacao.FechamentoPontuacao>();
            int? ultimoNumero = consultaFechamentoPontuacao.Max(o => (int?)o.Numero);

            return ultimoNumero.HasValue ? (ultimoNumero.Value + 1) : 1;
        }

        public List<Dominio.Entidades.Embarcador.Frete.Pontuacao.FechamentoPontuacao> Consultar(Dominio.ObjetosDeValor.Embarcador.Frete.FiltroPesquisaFechamentoPontuacao filtrosPesquisa, Dominio.ObjetosDeValor.Embarcador.Consulta.ParametroConsulta parametrosConsulta)
        {
            var consultaFechamentoPontuacao = Consultar(filtrosPesquisa);

            return ObterLista(consultaFechamentoPontuacao, parametrosConsulta);
        }

        public int ContarConsulta(Dominio.ObjetosDeValor.Embarcador.Frete.FiltroPesquisaFechamentoPontuacao filtrosPesquisa)
        {
            var consultaFechamentoPontuacao = Consultar(filtrosPesquisa);

            return consultaFechamentoPontuacao.Count();
        }

        #endregion
    }
}
