using Dominio.ObjetosDeValor.Embarcador.Enumeradores;
using System.Collections.Generic;
using System.Linq;

namespace Repositorio.Embarcador.Cargas.ComprovanteCarga
{
    public class TipoComprovante : RepositorioBase<Dominio.Entidades.Embarcador.Cargas.ComprovanteCarga.TipoComprovante>
    {
        #region Construtores

        public TipoComprovante(UnitOfWork unitOfWork) : base(unitOfWork) { }

        #endregion Construtores

        #region Métodos Privados

        private IQueryable<Dominio.Entidades.Embarcador.Cargas.ComprovanteCarga.TipoComprovante> Consultar(Dominio.ObjetosDeValor.Embarcador.Carga.ComprovanteCarga.FiltroPesquisaTipoComprovante filtrosPesquisa)
        {
            var consultaMotivoIrregularidade = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.ComprovanteCarga.TipoComprovante>();

            if (!string.IsNullOrEmpty(filtrosPesquisa.Descricao))
                consultaMotivoIrregularidade = consultaMotivoIrregularidade.Where(obj => obj.Descricao.Contains(filtrosPesquisa.Descricao));

            if (filtrosPesquisa.Situacao != SituacaoAtivoPesquisa.Todos)
            {
                if (filtrosPesquisa.Situacao == SituacaoAtivoPesquisa.Ativo)
                    consultaMotivoIrregularidade = consultaMotivoIrregularidade.Where(obj => obj.Ativa == true);
                else
                    consultaMotivoIrregularidade = consultaMotivoIrregularidade.Where(obj => obj.Ativa == false);
            }

            return consultaMotivoIrregularidade;
        }

        #endregion Métodos Privados

        #region Métodos Públicos

        public Dominio.Entidades.Embarcador.Cargas.ComprovanteCarga.TipoComprovante BuscarPorCodigo(int codigo)
        {
            var consultaIrregularidade = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.ComprovanteCarga.TipoComprovante>()
                .Where(obj => obj.Codigo == codigo);

            return consultaIrregularidade.FirstOrDefault();
        }

        public List<Dominio.Entidades.Embarcador.Cargas.ComprovanteCarga.TipoComprovante> Consultar(Dominio.ObjetosDeValor.Embarcador.Carga.ComprovanteCarga.FiltroPesquisaTipoComprovante filtrosPesquisa, Dominio.ObjetosDeValor.Embarcador.Consulta.ParametroConsulta parametrosConsulta)
        {
            var consultaIrregularidade = Consultar(filtrosPesquisa);

            return ObterLista(consultaIrregularidade, parametrosConsulta);
        }

        public int ContarConsulta(Dominio.ObjetosDeValor.Embarcador.Carga.ComprovanteCarga.FiltroPesquisaTipoComprovante filtrosPesquisa)
        {
            var consultaMotivoIrregularidade = Consultar(filtrosPesquisa);

            return consultaMotivoIrregularidade.Count();
        }

        #endregion Métodos Públicos
    }
}
