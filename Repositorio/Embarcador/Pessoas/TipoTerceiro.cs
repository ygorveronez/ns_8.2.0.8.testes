using System.Collections.Generic;
using System.Linq;
using System.Linq.Dynamic.Core;
using Dominio.ObjetosDeValor.Embarcador.Enumeradores;

namespace Repositorio.Embarcador.Pessoas
{
    public class TipoTerceiro : RepositorioBase<Dominio.Entidades.Embarcador.Pessoas.TipoTerceiro>
    {
        #region Construtores

        public TipoTerceiro(UnitOfWork unitOfWork) : base(unitOfWork) { }

        #endregion

        #region MétodosPrivados

        private IQueryable<Dominio.Entidades.Embarcador.Pessoas.TipoTerceiro> Consultar(string descricao, SituacaoAtivoPesquisa? situacao)
        {
            var consultaTipoTerceiro = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Pessoas.TipoTerceiro>();

            if (!string.IsNullOrWhiteSpace(descricao))
                consultaTipoTerceiro = consultaTipoTerceiro.Where(o => o.Descricao.Contains(descricao));

            if (situacao.HasValue)
            {
                if (situacao.Value == Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoAtivoPesquisa.Ativo)
                    consultaTipoTerceiro = consultaTipoTerceiro.Where(obj => obj.Situacao);
                else if (situacao.Value == Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoAtivoPesquisa.Inativo)
                    consultaTipoTerceiro = consultaTipoTerceiro.Where(obj => !obj.Situacao);
            }

            return consultaTipoTerceiro;
        }

        #endregion

        #region Métodos Públicos

        public Dominio.Entidades.Embarcador.Pessoas.TipoTerceiro BuscarPorCodigoIntegracao(string codigoIntegracao)
        {
            var consultaTipoTerceiro = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Pessoas.TipoTerceiro>()
                .Where(o => o.CodigoIntegracao == codigoIntegracao);

            return consultaTipoTerceiro.FirstOrDefault();
        }

        public Dominio.Entidades.Embarcador.Pessoas.TipoTerceiro BuscarPorCodigo(int codigo)
        {
            var consultaTipoTerceiro = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Pessoas.TipoTerceiro>()
                .Where(o => o.Codigo == codigo);

            return consultaTipoTerceiro.FirstOrDefault();
        }

        public List<Dominio.Entidades.Embarcador.Pessoas.TipoTerceiro> BuscarPorCodigos(List<int> codigos)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Pessoas.TipoTerceiro>();
            var result = from obj in query where codigos.Contains(obj.Codigo) select obj;
            return result.ToList();
        }

        public Dominio.Entidades.Embarcador.Pessoas.TipoTerceiro BuscarPorDescricao(string tipoTerceiro)
        {
            var consultaTipoTerceiro = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Pessoas.TipoTerceiro>()
                .Where(o => o.Descricao == tipoTerceiro);

            return consultaTipoTerceiro.FirstOrDefault();
        }

        public List<Dominio.Entidades.Embarcador.Pessoas.TipoTerceiro> Consultar(string descricao, SituacaoAtivoPesquisa? situacao, Dominio.ObjetosDeValor.Embarcador.Consulta.ParametroConsulta parametrosConsulta)
        {
            var consultaTipoTerceiro = Consultar(descricao, situacao);

            return ObterLista(consultaTipoTerceiro, parametrosConsulta);
        }

        public int ContarConsulta(string descricao, SituacaoAtivoPesquisa? situacao)
        {
            var consultaTipoTerceiro = Consultar(descricao, situacao);

            return consultaTipoTerceiro.Count();
        }

        #endregion
    }
}
