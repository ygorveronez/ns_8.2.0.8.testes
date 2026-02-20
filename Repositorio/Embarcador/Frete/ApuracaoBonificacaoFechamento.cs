using System.Collections.Generic;
using System.Linq;
using System.Linq.Dynamic.Core;

namespace Repositorio.Embarcador.Frete
{
    public sealed class ApuracaoBonificacaoFechamento : RepositorioBase<Dominio.Entidades.Embarcador.Frete.ApuracaoBonificacaoFechamento>
    {
        #region Construtores

        public ApuracaoBonificacaoFechamento(UnitOfWork unitOfWork) : base(unitOfWork) { }

        #endregion

        #region Métodos Privados

        public List<Dominio.Entidades.Embarcador.Frete.ApuracaoBonificacaoFechamento> BuscarPorApuracaoBonificacao(int apuracaoBonificacao)
        {
            var consultaApuracaoBonificacaoFechamento = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Frete.ApuracaoBonificacaoFechamento>()
                .Where(o => o.ApuracaoBonificacao.Codigo == apuracaoBonificacao);

            return consultaApuracaoBonificacaoFechamento.ToList();
        }

        private IQueryable<Dominio.Entidades.Embarcador.Frete.ApuracaoBonificacaoFechamento> Consultar(int codigo)
        {
            var consultaApuracaoBonificacaoFechamento = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Frete.ApuracaoBonificacaoFechamento>();

            consultaApuracaoBonificacaoFechamento = consultaApuracaoBonificacaoFechamento.Where(o => o.ApuracaoBonificacao.Codigo == codigo);

            return consultaApuracaoBonificacaoFechamento;
        }

        #endregion

        #region Métodos Públicos

        public List<Dominio.Entidades.Embarcador.Frete.ApuracaoBonificacaoFechamento> Consultar(int codigo, Dominio.ObjetosDeValor.Embarcador.Consulta.ParametroConsulta parametrosConsulta)
        {
            var consultaApuracaoBonificacaoFechamento = Consultar(codigo);

            return ObterLista(consultaApuracaoBonificacaoFechamento, parametrosConsulta);
        }

        public int ContarConsulta(int codigo)
        {
            var consultaApuracaoBonificacaoFechamento = Consultar(codigo);

            return consultaApuracaoBonificacaoFechamento.Count();
        }

        #endregion
    }
}