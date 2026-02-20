using System.Collections.Generic;
using System.Linq;
using System.Linq.Dynamic.Core;

namespace Repositorio.Embarcador.Cargas.Retornos
{
    public class TipoRetornoCarga : RepositorioBase<Dominio.Entidades.Embarcador.Cargas.Retornos.TipoRetornoCarga>
    {
        #region Construtores

        public TipoRetornoCarga(UnitOfWork unitOfWork) : base(unitOfWork) { }

        #endregion

        #region Métodos Privados

        private IQueryable<Dominio.Entidades.Embarcador.Cargas.Retornos.TipoRetornoCarga> Consultar(Dominio.ObjetosDeValor.Embarcador.Carga.FiltroPesquisaTipoRetornoCarga filtrosPesquisa)
        {
            var consultaTipoRetornoCarga = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.Retornos.TipoRetornoCarga>();

            if (!string.IsNullOrWhiteSpace(filtrosPesquisa.Descricao))
                consultaTipoRetornoCarga = consultaTipoRetornoCarga.Where(obj => obj.Descricao.Contains(filtrosPesquisa.Descricao));

            if (filtrosPesquisa.CodigoTipoOperacao > 0)
                consultaTipoRetornoCarga = consultaTipoRetornoCarga.Where(obj => obj.TipoOperacao.Codigo == filtrosPesquisa.CodigoTipoOperacao);

            if (filtrosPesquisa.Tipo.HasValue)
                consultaTipoRetornoCarga = consultaTipoRetornoCarga.Where(obj => obj.Tipo == filtrosPesquisa.Tipo.Value);

            if (filtrosPesquisa.SituacaoAtivo == Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoAtivoPesquisa.Ativo)
                consultaTipoRetornoCarga = consultaTipoRetornoCarga.Where(obj => obj.Ativo);
            else if (filtrosPesquisa.SituacaoAtivo == Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoAtivoPesquisa.Inativo)
                consultaTipoRetornoCarga = consultaTipoRetornoCarga.Where(obj => !obj.Ativo);

            return consultaTipoRetornoCarga;
        }

        #endregion

        #region Métodos Públicos

        public List<Dominio.Entidades.Embarcador.Cargas.Retornos.TipoRetornoCarga> BuscarPorAtivo()
        {
            var consultaTipoRetornoCarga = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.Retornos.TipoRetornoCarga>()
                .Where(o => o.Ativo);

            return consultaTipoRetornoCarga.ToList();
        }

        public List<Dominio.Entidades.Embarcador.Cargas.Retornos.TipoRetornoCarga> BuscarPorDescricaoEAtivo(string descricao)
        {
            var consultaTipoRetornoCarga = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.Retornos.TipoRetornoCarga>()
                .Where(o => o.Ativo && o.Descricao.Contains(descricao));

            return consultaTipoRetornoCarga.ToList();
        }

        public Dominio.Entidades.Embarcador.Cargas.Retornos.TipoRetornoCarga BuscarPorCodigo(int codigo)
        {
            var consultaTipoRetornoCarga = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.Retornos.TipoRetornoCarga>()
                .Where(o => o.Codigo == codigo);

            return consultaTipoRetornoCarga.FirstOrDefault();
        }

        public Dominio.Entidades.Embarcador.Cargas.Retornos.TipoRetornoCarga BuscarPorTipoReversa()
        {
            var consultaTipoRetornoCarga = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.Retornos.TipoRetornoCarga>()
                .Where(o => o.Tipo == Dominio.ObjetosDeValor.Embarcador.Enumeradores.RetornoCargaTipo.Carregado);

            return consultaTipoRetornoCarga.FirstOrDefault();
        }

        public Dominio.Entidades.Embarcador.Cargas.Retornos.TipoRetornoCarga BuscarPorTipoVazio()
        {
            var consultaTipoRetornoCarga = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.Retornos.TipoRetornoCarga>()
                .Where(o => o.Tipo == Dominio.ObjetosDeValor.Embarcador.Enumeradores.RetornoCargaTipo.Vazio);

            return consultaTipoRetornoCarga.FirstOrDefault();
        }

        public List<Dominio.Entidades.Embarcador.Cargas.Retornos.TipoRetornoCarga> Consultar(Dominio.ObjetosDeValor.Embarcador.Carga.FiltroPesquisaTipoRetornoCarga filtrosPesquisa, Dominio.ObjetosDeValor.Embarcador.Consulta.ParametroConsulta parametrosConsulta)
        {
            var consultaTipoRetornoCarga = Consultar(filtrosPesquisa);

            return ObterLista(consultaTipoRetornoCarga, parametrosConsulta);
        }

        public int ContarConsulta(Dominio.ObjetosDeValor.Embarcador.Carga.FiltroPesquisaTipoRetornoCarga filtrosPesquisa)
        {
            var consultaTipoRetornoCarga = Consultar(filtrosPesquisa);

            return consultaTipoRetornoCarga.Count();
        }

        #endregion
    }
}
