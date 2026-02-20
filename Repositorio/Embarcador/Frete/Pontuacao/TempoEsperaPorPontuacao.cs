using System.Collections.Generic;
using System.Linq;
using System.Linq.Dynamic.Core;

namespace Repositorio.Embarcador.Frete.Pontuacao
{
    public class TempoEsperaPorPontuacao : RepositorioBase<Dominio.Entidades.Embarcador.Frete.Pontuacao.TempoEsperaPorPontuacao>
    {
        #region Construtores

        public TempoEsperaPorPontuacao(UnitOfWork unitOfWork) : base(unitOfWork) { }

        #endregion

        #region Métodos Públicos

        public Dominio.Entidades.Embarcador.Frete.Pontuacao.TempoEsperaPorPontuacao BuscarPorCodigo(int codigo)
        {
            var consultaTempoEsperaPorPontuacao = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Frete.Pontuacao.TempoEsperaPorPontuacao>()
                .Where(o => o.Codigo == codigo);

            return consultaTempoEsperaPorPontuacao.FirstOrDefault();
        }

        public Dominio.Entidades.Embarcador.Frete.Pontuacao.TempoEsperaPorPontuacao BuscarPorIntervaloPontuacaoDuplicado(int pontuacaoInicial, int pontuacaoFinal)
        {
            var consultaTempoEsperaPorPontuacao = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Frete.Pontuacao.TempoEsperaPorPontuacao>()
                .Where(obj =>
                    (pontuacaoInicial >= obj.PontuacaoInicial && pontuacaoInicial <= obj.PontuacaoFinal) ||
                    (pontuacaoFinal >= obj.PontuacaoInicial && pontuacaoFinal <= obj.PontuacaoFinal) ||
                    (obj.PontuacaoInicial >= pontuacaoInicial && obj.PontuacaoInicial <= pontuacaoFinal) ||
                    (obj.PontuacaoFinal >= pontuacaoInicial && obj.PontuacaoFinal <= pontuacaoFinal)
                );

            return consultaTempoEsperaPorPontuacao.FirstOrDefault();
        }

        public List<Dominio.Entidades.Embarcador.Frete.Pontuacao.TempoEsperaPorPontuacao> Consultar(Dominio.ObjetosDeValor.Embarcador.Consulta.ParametroConsulta parametrosConsulta)
        {
            var consultaTempoEsperaPorPontuacao = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Frete.Pontuacao.TempoEsperaPorPontuacao>();

            return ObterLista(consultaTempoEsperaPorPontuacao, parametrosConsulta);
        }

        public int ContarConsulta()
        {
            var consultaTempoEsperaPorPontuacao = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Frete.Pontuacao.TempoEsperaPorPontuacao>();

            return consultaTempoEsperaPorPontuacao.Count();
        }

        #endregion
    }
}

