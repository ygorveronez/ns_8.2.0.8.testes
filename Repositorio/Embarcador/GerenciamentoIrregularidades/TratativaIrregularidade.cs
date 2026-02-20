using Dominio.ObjetosDeValor.Embarcador.Enumeradores;
using System.Collections.Generic;
using System.Linq;

namespace Repositorio.Embarcador.GerenciamentoIrregularidades
{
    public class TratativaIrregularidade : RepositorioBase<Dominio.Entidades.Embarcador.GerenciamentoIrregularidades.TratativaIrregularidade>
    {
        #region Construtores

        public TratativaIrregularidade(UnitOfWork unitOfWork) : base(unitOfWork) { }

        #endregion Construtores

        #region Métodos Privados

        private IQueryable<Dominio.Entidades.Embarcador.GerenciamentoIrregularidades.Irregularidade> Consultar(Dominio.ObjetosDeValor.Embarcador.GerenciamentoIrregularidades.FiltroPesquisaIrregularidade filtrosPesquisa)
        {
            var consultaIrregularidade = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.GerenciamentoIrregularidades.Irregularidade>();

            if (!string.IsNullOrEmpty(filtrosPesquisa.Descricao))
                consultaIrregularidade = consultaIrregularidade.Where(obj => obj.Descricao.Contains(filtrosPesquisa.Descricao));

            if (filtrosPesquisa.Sequencia > 0)
                consultaIrregularidade = consultaIrregularidade.Where(obj => obj.Sequencia == filtrosPesquisa.Sequencia);

            if (!string.IsNullOrEmpty(filtrosPesquisa.CodigoIntegracao))
                consultaIrregularidade = consultaIrregularidade.Where(obj => obj.CodigoIntegracao == filtrosPesquisa.CodigoIntegracao);

            if (filtrosPesquisa.CodigoPortfolioModuloControle > 0)
                consultaIrregularidade = consultaIrregularidade.Where(obj => obj.PortfolioModuloControle.Codigo == filtrosPesquisa.CodigoPortfolioModuloControle);

            if (filtrosPesquisa.SeguirAprovacaoTranspPrimeiro.HasValue)
                consultaIrregularidade = consultaIrregularidade.Where(obj => obj.SeguirAprovacaoTranspPrimeiro == filtrosPesquisa.SeguirAprovacaoTranspPrimeiro.Value);

            if (filtrosPesquisa.Situacao != SituacaoAtivaPesquisa.Todos)
            {
                if (filtrosPesquisa.Situacao == SituacaoAtivaPesquisa.Ativa)
                    consultaIrregularidade = consultaIrregularidade.Where(obj => obj.Ativa == true);
                else
                    consultaIrregularidade = consultaIrregularidade.Where(obj => obj.Ativa == false);
            }

            return consultaIrregularidade;
        }

        #endregion Métodos Privados

        #region Métodos Públicos

        public Dominio.Entidades.Embarcador.GerenciamentoIrregularidades.TratativaIrregularidade BuscarPorCodigo(int codigo)
        {
            var consultaTratativa = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.GerenciamentoIrregularidades.TratativaIrregularidade>()
                .Where(obj => obj.Codigo == codigo);

            return consultaTratativa.FirstOrDefault();
        }

        public List<Dominio.Entidades.Embarcador.GerenciamentoIrregularidades.TratativaIrregularidade> BuscarPorCodigos(List<int> codigos)
        {
            var consultaTratativa = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.GerenciamentoIrregularidades.TratativaIrregularidade>()
                .Where(obj => codigos.Contains(obj.Codigo));

            return consultaTratativa.ToList();
        }

        public List<Dominio.Entidades.Embarcador.GerenciamentoIrregularidades.TratativaIrregularidade> BuscarPorDefinicaoTratativas(int codigo)
        {
            var consultaTratativa = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.GerenciamentoIrregularidades.TratativaIrregularidade>()
                .Where(obj => obj.DefinicaoTratativasIrregularidade.Codigo == codigo);

            return consultaTratativa.ToList();
        }

        public Dominio.Entidades.Embarcador.GerenciamentoIrregularidades.TratativaIrregularidade BuscarPorDefinicaoTratativasIrregularidade(int codigoIrregularidade, int sequencia, int grupoTipoOperacao, TipoSetorFuncionario? tipoSetor)
        {
            var consultaTratativa = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.GerenciamentoIrregularidades.TratativaIrregularidade>()
                .Where(obj => obj.DefinicaoTratativasIrregularidade.Irregularidade.Codigo == codigoIrregularidade && obj.DefinicaoTratativasIrregularidade.Ativa);

            if (sequencia > 0)
                consultaTratativa = consultaTratativa.Where(obj => obj.Sequencia == sequencia);

            if (tipoSetor.HasValue)
                consultaTratativa = consultaTratativa.Where(obj => obj.Setor.TipoSetorFuncionario == tipoSetor.Value);

            if (grupoTipoOperacao > 0)
                consultaTratativa = consultaTratativa.Where(obj => obj.GrupoTipoOperacao.Codigo == grupoTipoOperacao || obj.GrupoTipoOperacao.Codigo == 0 || obj.GrupoTipoOperacao == null);

            return consultaTratativa.OrderBy(o => o.Sequencia).OrderByDescending(o => o.GrupoTipoOperacao.Codigo).FirstOrDefault();
        }

        public Dominio.Entidades.Embarcador.GerenciamentoIrregularidades.TratativaIrregularidade BuscarPorDefinicaoTratativasIrregularidadeESetor(int codigoIrregularidade, int setor)
        {
            var consultaTratativa = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.GerenciamentoIrregularidades.TratativaIrregularidade>()
                .Where(obj => obj.DefinicaoTratativasIrregularidade.Irregularidade.Codigo == codigoIrregularidade);

            if (setor > 0)
                consultaTratativa = consultaTratativa.Where(obj => obj.Setor.Codigo == setor);

            return consultaTratativa.FirstOrDefault();
        }

        public List<int> BuscarCodigosSetoresPorIrregularidade(int codigoIrregularidade)
        {
            var consultaTratativa = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.GerenciamentoIrregularidades.TratativaIrregularidade>()
                .Where(obj => obj.DefinicaoTratativasIrregularidade.Irregularidade.Codigo == codigoIrregularidade);

            return consultaTratativa.Select(o => o.Setor.Codigo).ToList();
        }

        #endregion Métodos Públicos
    }
}
