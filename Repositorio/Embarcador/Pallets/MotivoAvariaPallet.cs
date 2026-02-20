using System.Collections.Generic;
using System.Linq;
using System.Linq.Dynamic.Core;

namespace Repositorio.Embarcador.Pallets
{
    public class MotivoAvariaPallet : RepositorioBase<Dominio.Entidades.Embarcador.Pallets.MotivoAvariaPallet>
    {
        #region Construtores

        public MotivoAvariaPallet(UnitOfWork unitOfWork) : base(unitOfWork) { }

        #endregion

        #region Métodos Privados

        private IQueryable<Dominio.Entidades.Embarcador.Pallets.MotivoAvariaPallet> Consultar(string descricao, Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoAtivoPesquisa status)
        {
            var listaMotivoAvariaPattet = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Pallets.MotivoAvariaPallet>();

            if (status != Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoAtivoPesquisa.Todos)
            {
                bool ativo = status == Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoAtivoPesquisa.Ativo;
                listaMotivoAvariaPattet = listaMotivoAvariaPattet.Where(motivoAvariaPattet => motivoAvariaPattet.Ativo == ativo);
            }

            if (!string.IsNullOrWhiteSpace(descricao))
                listaMotivoAvariaPattet = listaMotivoAvariaPattet.Where(motivoAvariaPattet => motivoAvariaPattet.Descricao.Contains(descricao));

            return listaMotivoAvariaPattet;
        }

        #endregion

        #region Métodos Públicos

        public Dominio.Entidades.Embarcador.Pallets.MotivoAvariaPallet BuscarPorCodigo(int codigo)
        {
            return this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Pallets.MotivoAvariaPallet>()
                .Where(motivoAvariaPallet => motivoAvariaPallet.Codigo == codigo)
                .FirstOrDefault();
        }

        public List<Dominio.Entidades.Embarcador.Pallets.MotivoAvariaPallet> Consultar(string descricao, Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoAtivoPesquisa status, string propriedadeOrdenar, string direcaoOrdenacao, int inicioRegistros, int maximoRegistros)
        {
            var consultaMotivoAvariaPattet = Consultar(descricao, status);

            return ObterLista(consultaMotivoAvariaPattet, propriedadeOrdenar, direcaoOrdenacao, inicioRegistros, maximoRegistros);
        }

        public int ContarConsulta(string descricao, Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoAtivoPesquisa status)
        {
            var listaMotivoAvariaPattet = Consultar(descricao, status);

            return listaMotivoAvariaPattet.Count();
        }

        #endregion
    }
}
