using System.Collections.Generic;
using System.Linq;
using System.Linq.Dynamic.Core;
using Dominio.ObjetosDeValor.Embarcador.Enumeradores;

namespace Repositorio.Embarcador.Canhotos
{
    public class MotivoRejeicaoAuditoria : RepositorioBase<Dominio.Entidades.Embarcador.Canhotos.MotivoRejeicaoAuditoria>
    {
        public MotivoRejeicaoAuditoria(UnitOfWork unitOfWork) : base(unitOfWork) { }

        #region Métodos Públicos
        public Dominio.Entidades.Embarcador.Canhotos.MotivoRejeicaoAuditoria BuscarPorCodigo(int codigo)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Canhotos.MotivoRejeicaoAuditoria>();
            var result = from obj in query where obj.Codigo == codigo select obj;
            return result.FirstOrDefault();
        }

        public List<Dominio.Entidades.Embarcador.Canhotos.MotivoRejeicaoAuditoria> Consultar(Dominio.ObjetosDeValor.Embarcador.Canhoto.FiltroPesquisaMotivoRejeicaoAuditoria filtroPesquisa, Dominio.ObjetosDeValor.Embarcador.Consulta.ParametroConsulta parametrosConsulta)
        {
            var result = _Consultar(filtroPesquisa);
            return ObterLista(result, parametrosConsulta);
        }

        public int ContarConsulta(Dominio.ObjetosDeValor.Embarcador.Canhoto.FiltroPesquisaMotivoRejeicaoAuditoria filtrosPesquisa)
        {
            var result = _Consultar(filtrosPesquisa);
            return result.Count();
        }
        #endregion

        #region Métodos Privados

        private IQueryable<Dominio.Entidades.Embarcador.Canhotos.MotivoRejeicaoAuditoria> _Consultar(Dominio.ObjetosDeValor.Embarcador.Canhoto.FiltroPesquisaMotivoRejeicaoAuditoria filtroPesquisa)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Canhotos.MotivoRejeicaoAuditoria>();

            var result = from obj in query select obj;

            if (filtroPesquisa.FiltroDescricao != null)
                result = result.Where(o => o.Descricao.Contains(filtroPesquisa.FiltroDescricao));

            if (filtroPesquisa.FiltroSituacao != null)
            {
                if (!(filtroPesquisa.FiltroSituacao == SituacaoAtivoPesquisa.Todos))
                {
                    bool ativo = filtroPesquisa.FiltroSituacao == SituacaoAtivoPesquisa.Ativo ? true : false;
                    result = result.Where(o => o.Ativo == ativo);
                }
            }

            return result;
        }

        #endregion
    }
}
