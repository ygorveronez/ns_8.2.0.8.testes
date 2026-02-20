using System.Linq;
using System.Linq.Dynamic.Core;
using System.Collections.Generic;

namespace Repositorio.Embarcador.Logistica
{
    public class MotivoRetiradaFilaCarregamento : RepositorioBase<Dominio.Entidades.Embarcador.Logistica.MotivoRetiradaFilaCarregamento>
    {
        #region Construtores

        public MotivoRetiradaFilaCarregamento(UnitOfWork unitOfWork) : base(unitOfWork) { }

        #endregion

        #region Métodos Privados

        private IQueryable<Dominio.Entidades.Embarcador.Logistica.MotivoRetiradaFilaCarregamento> Consultar(Dominio.ObjetosDeValor.Embarcador.Logistica.FiltroPesquisaMotivoRetiradaFilaCarregamento filtrosPesquisa)
        {
            var consultaMotivoRetiradaFilaCarregamento = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Logistica.MotivoRetiradaFilaCarregamento>();

            if (!string.IsNullOrWhiteSpace(filtrosPesquisa.Descricao))
                consultaMotivoRetiradaFilaCarregamento = consultaMotivoRetiradaFilaCarregamento.Where(o => o.Descricao.Contains(filtrosPesquisa.Descricao));

            if (filtrosPesquisa.Mobile.HasValue)
                consultaMotivoRetiradaFilaCarregamento = consultaMotivoRetiradaFilaCarregamento.Where(o => o.Mobile == filtrosPesquisa.Mobile.Value);

            if (filtrosPesquisa.SituacaoAtivo != Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoAtivoPesquisa.Todos)
            {
                var ativo = filtrosPesquisa.SituacaoAtivo == Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoAtivoPesquisa.Ativo ? true : false;

                consultaMotivoRetiradaFilaCarregamento = consultaMotivoRetiradaFilaCarregamento.Where(o => o.Ativo == ativo);
            }

            return consultaMotivoRetiradaFilaCarregamento;
        }

        #endregion

        #region Métodos Públicos

        public Dominio.Entidades.Embarcador.Logistica.MotivoRetiradaFilaCarregamento BuscarPorCodigo(int codigo)
        {
            var motivoRetiradaFilaCarregamento = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Logistica.MotivoRetiradaFilaCarregamento>()
                .Where(o => o.Codigo == codigo)
                .FirstOrDefault();

            return motivoRetiradaFilaCarregamento;
        }

        public List<Dominio.Entidades.Embarcador.Logistica.MotivoRetiradaFilaCarregamento> Consultar(Dominio.ObjetosDeValor.Embarcador.Logistica.FiltroPesquisaMotivoRetiradaFilaCarregamento filtrosPesquisa, string propriedadeOrdenar, string direcaoOrdenacao, int inicioRegistros, int maximoRegistros)
        {
            var consultaMotivoRetiradaFilaCarregamento = Consultar(filtrosPesquisa);

            return ObterLista(consultaMotivoRetiradaFilaCarregamento, propriedadeOrdenar, direcaoOrdenacao, inicioRegistros, maximoRegistros);
        }

        public List<Dominio.Entidades.Embarcador.Logistica.MotivoRetiradaFilaCarregamento> ConsultarMobile()
        {
            var filtrosPesquisa = new Dominio.ObjetosDeValor.Embarcador.Logistica.FiltroPesquisaMotivoRetiradaFilaCarregamento()
            {
                Descricao = "",
                Mobile = true,
                SituacaoAtivo = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoAtivoPesquisa.Ativo
            };

            return Consultar(filtrosPesquisa, propriedadeOrdenar: "Descricao", direcaoOrdenacao: "asc", inicioRegistros: 0, maximoRegistros: 0);
        }

        public int ContarConsulta(Dominio.ObjetosDeValor.Embarcador.Logistica.FiltroPesquisaMotivoRetiradaFilaCarregamento filtrosPesquisa)
        {
            var consultaMotivoRetiradaFilaCarregamento = Consultar(filtrosPesquisa);

            return consultaMotivoRetiradaFilaCarregamento.Count();
        }

        public bool ExisteAtivo()
        {
            var motivoRetiradaFilaCarregamento = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Logistica.MotivoRetiradaFilaCarregamento>()
                .Where(o => o.Ativo == true);

            return motivoRetiradaFilaCarregamento.Count() > 0;
        }

        #endregion
    }
}
