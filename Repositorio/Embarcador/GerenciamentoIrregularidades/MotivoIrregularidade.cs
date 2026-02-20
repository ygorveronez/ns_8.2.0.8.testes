using Dominio.ObjetosDeValor.Embarcador.Enumeradores;
using System.Collections.Generic;
using System.Linq;

namespace Repositorio.Embarcador.GerenciamentoIrregularidades
{
    public class MotivoIrregularidade : RepositorioBase<Dominio.Entidades.Embarcador.GerenciamentoIrregularidades.MotivoIrregularidade>
    {
        #region Construtores

        public MotivoIrregularidade(UnitOfWork unitOfWork) : base(unitOfWork) { }

        #endregion Construtores

        #region Métodos Privados

        private IQueryable<Dominio.Entidades.Embarcador.GerenciamentoIrregularidades.MotivoIrregularidade> Consultar(Dominio.ObjetosDeValor.Embarcador.GerenciamentoIrregularidades.FiltroPesquisaMotivoIrregularidade filtrosPesquisa)
        {
            var consultaMotivoIrregularidade = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.GerenciamentoIrregularidades.MotivoIrregularidade>();

            if (!string.IsNullOrEmpty(filtrosPesquisa.Descricao))
                consultaMotivoIrregularidade = consultaMotivoIrregularidade.Where(obj => obj.Descricao.Contains(filtrosPesquisa.Descricao));

            if (filtrosPesquisa.Situacao != SituacaoAtivaPesquisa.Todos)
            {
                if (filtrosPesquisa.Situacao == SituacaoAtivaPesquisa.Ativa)
                    consultaMotivoIrregularidade = consultaMotivoIrregularidade.Where(obj => obj.Ativa == true);
                else
                    consultaMotivoIrregularidade = consultaMotivoIrregularidade.Where(obj => obj.Ativa == false);
            }

            if (filtrosPesquisa.CodigoIrregularidade > 0)
                consultaMotivoIrregularidade = consultaMotivoIrregularidade.Where(x => x.Irregularidade.Codigo == filtrosPesquisa.CodigoIrregularidade);

            return consultaMotivoIrregularidade;
        }

        #endregion Métodos Privados

        #region Métodos Públicos

        public Dominio.Entidades.Embarcador.GerenciamentoIrregularidades.MotivoIrregularidade BuscarPorCodigo(int codigo)
        {
            var consultaIrregularidade = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.GerenciamentoIrregularidades.MotivoIrregularidade>()
                .Where(obj => obj.Codigo == codigo);

            return consultaIrregularidade.FirstOrDefault();
        }
        public Dominio.Entidades.Embarcador.GerenciamentoIrregularidades.MotivoIrregularidade BuscarPorTipoIrregularidade(TipoIrregularidade tipo)
        {
            var consultaIrregularidade = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.GerenciamentoIrregularidades.MotivoIrregularidade>()
                .Where(obj => obj.Irregularidade.TipoIrregularidade == tipo);

            return consultaIrregularidade.FirstOrDefault();
        }
        public Dominio.Entidades.Embarcador.GerenciamentoIrregularidades.MotivoIrregularidade BuscarPorDescricao(string descripcao)
        {
            var consultaIrregularidade = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.GerenciamentoIrregularidades.MotivoIrregularidade>()
                .Where(obj => obj.Descricao.Contains(descripcao));

            return consultaIrregularidade.FirstOrDefault();
        }

        public List<Dominio.Entidades.Embarcador.GerenciamentoIrregularidades.MotivoIrregularidade> BuscarPorCodigos(List<int> codigos)
        {
            var consultaMotivo = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.GerenciamentoIrregularidades.MotivoIrregularidade>()
                .Where(obj => codigos.Contains(obj.Codigo));

            return consultaMotivo.ToList();
        }

        public List<Dominio.Entidades.Embarcador.GerenciamentoIrregularidades.MotivoIrregularidade> Consultar(Dominio.ObjetosDeValor.Embarcador.GerenciamentoIrregularidades.FiltroPesquisaMotivoIrregularidade filtrosPesquisa, Dominio.ObjetosDeValor.Embarcador.Consulta.ParametroConsulta parametrosConsulta)
        {
            var consultaIrregularidade = Consultar(filtrosPesquisa);

            return ObterLista(consultaIrregularidade, parametrosConsulta);
        }

        public int ContarConsulta(Dominio.ObjetosDeValor.Embarcador.GerenciamentoIrregularidades.FiltroPesquisaMotivoIrregularidade filtrosPesquisa)
        {
            var consultaMotivoIrregularidade = Consultar(filtrosPesquisa);

            return consultaMotivoIrregularidade.Count();
        }

        #endregion Métodos Públicos
    }
}
