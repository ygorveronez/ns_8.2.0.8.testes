using System.Linq;
using System.Linq.Dynamic.Core;
using System.Collections.Generic;

namespace Repositorio.Embarcador.Frota
{
    public sealed class TipoInfracao : RepositorioBase<Dominio.Entidades.Embarcador.Frota.TipoInfracao>
    {
        #region Construtores

        public TipoInfracao(UnitOfWork unitOfWork) : base(unitOfWork) { }

        #endregion

        #region Métodos Privados

        private IQueryable<Dominio.Entidades.Embarcador.Frota.TipoInfracao> Consultar(string descricao, string codigoCTB, Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoAtivoPesquisa situacaoAtivo)
        {
            var consultaTipoInfracao = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Frota.TipoInfracao>();

            var codigo = 0;
            if (int.TryParse(descricao, out codigo))
                consultaTipoInfracao = consultaTipoInfracao.Where(obj => obj.CodigoCTB.Equals(codigo.ToString()));
            else if (!string.IsNullOrWhiteSpace(descricao))
                consultaTipoInfracao = consultaTipoInfracao.Where(o => o.Descricao.Contains(descricao));

            if (!string.IsNullOrWhiteSpace(codigoCTB))
                consultaTipoInfracao = consultaTipoInfracao.Where(o => o.CodigoCTB.Equals(codigoCTB));

            if (situacaoAtivo != Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoAtivoPesquisa.Todos)
            {
                var ativo = situacaoAtivo == Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoAtivoPesquisa.Ativo ? true : false;

                consultaTipoInfracao = consultaTipoInfracao.Where(o => o.Ativo == ativo);
            }

            return consultaTipoInfracao;
        }

        #endregion

        #region Métodos Públicos

        public Dominio.Entidades.Embarcador.Frota.TipoInfracao BuscarPorCodigo(int codigo)
        {
            var tipoInfracao = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Frota.TipoInfracao>()
                .Where(o => o.Codigo == codigo)
                .FirstOrDefault();

            return tipoInfracao;
        }

        public List<Dominio.Entidades.Embarcador.Frota.TipoInfracao> BuscarPorCodigos(List<int> codigos)
        {
            var tipoInfracao = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Frota.TipoInfracao>()
                .Where(o => codigos.Contains(o.Codigo))
                .ToList();

            return tipoInfracao;
        }

        public Dominio.Entidades.Embarcador.Frota.TipoInfracao BuscarPorCodigoCTB(string codigoCTB)
        {
            var tipoInfracao = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Frota.TipoInfracao>()
                .Where(o => o.CodigoCTB == codigoCTB && o.Ativo)
                .FirstOrDefault();

            return tipoInfracao;
        }

        public List<Dominio.Entidades.Embarcador.Frota.TipoInfracao> Consultar(string descricao, string codigoCTB, Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoAtivoPesquisa situacaoAtivo, string propriedadeOrdenar, string direcaoOrdenacao, int inicioRegistros, int maximoRegistros)
        {
            var consultaTipoInfracao = Consultar(descricao, codigoCTB, situacaoAtivo);

            return ObterLista(consultaTipoInfracao, propriedadeOrdenar, direcaoOrdenacao, inicioRegistros, maximoRegistros);
        }

        public int ContarConsulta(string descricao, string codigoCTB, Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoAtivoPesquisa situacaoAtivo)
        {
            var consultaTipoInfracao = Consultar(descricao, codigoCTB, situacaoAtivo);

            return consultaTipoInfracao.Count();
        }

        #endregion
    }
}
