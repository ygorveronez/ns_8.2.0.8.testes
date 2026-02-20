using System.Collections.Generic;
using System.Linq;
using Dominio.ObjetosDeValor.Embarcador.Enumeradores;

namespace Repositorio.Embarcador.Notificacoes
{
    public sealed class ConfiguracaoAlerta : RepositorioBase<Dominio.Entidades.Embarcador.Notificacoes.ConfiguracaoAlerta>
    {
        #region Construtores

        public ConfiguracaoAlerta(UnitOfWork unitOfWork) : base(unitOfWork) { }

        #endregion

        #region Métodos Privados

        private IQueryable<Dominio.Entidades.Embarcador.Notificacoes.ConfiguracaoAlerta> Consultar(Dominio.ObjetosDeValor.Embarcador.Notificacao.FiltroPesquisaConfiguracaoAlerta filtrosPesquisa)
        {
            var consultaConfiguracaoAlerta = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Notificacoes.ConfiguracaoAlerta>();

            if (filtrosPesquisa.Tipo.HasValue)
                consultaConfiguracaoAlerta = consultaConfiguracaoAlerta.Where(o => o.Tipo == filtrosPesquisa.Tipo.Value);

            if (filtrosPesquisa.SituacaoAtivo == SituacaoAtivoPesquisa.Ativo)
                consultaConfiguracaoAlerta = consultaConfiguracaoAlerta.Where(o => o.Ativo);
            else if (filtrosPesquisa.SituacaoAtivo == SituacaoAtivoPesquisa.Inativo)
                consultaConfiguracaoAlerta = consultaConfiguracaoAlerta.Where(o => !o.Ativo);

            return consultaConfiguracaoAlerta;
        }

        #endregion

        #region Métodos Públicos

        public Dominio.Entidades.Embarcador.Notificacoes.ConfiguracaoAlerta BuscarAtivaPorTipo(TipoConfiguracaoAlerta tipo)
        {
            var consultaConfiguracaoAlerta = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Notificacoes.ConfiguracaoAlerta>()
                .Where(o => o.Tipo == tipo && o.Ativo);

            return consultaConfiguracaoAlerta.FirstOrDefault();
        }

        public Dominio.Entidades.Embarcador.Notificacoes.ConfiguracaoAlerta BuscarPorTipo(TipoConfiguracaoAlerta tipo)
        {
            var consultaConfiguracaoAlerta = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Notificacoes.ConfiguracaoAlerta>()
                .Where(o => o.Tipo == tipo);

            return consultaConfiguracaoAlerta.FirstOrDefault();
        }

        public List<Dominio.Entidades.Embarcador.Notificacoes.ConfiguracaoAlerta> Consultar(Dominio.ObjetosDeValor.Embarcador.Notificacao.FiltroPesquisaConfiguracaoAlerta filtrosPesquisa, Dominio.ObjetosDeValor.Embarcador.Consulta.ParametroConsulta parametrosConsulta)
        {
            var consultaConfiguracaoAlerta = Consultar(filtrosPesquisa);

            return ObterLista(consultaConfiguracaoAlerta, parametrosConsulta);
        }

        public int ContarConsulta(Dominio.ObjetosDeValor.Embarcador.Notificacao.FiltroPesquisaConfiguracaoAlerta filtrosPesquisa)
        {
            var consultaConfiguracaoAlerta = Consultar(filtrosPesquisa);

            return consultaConfiguracaoAlerta.Count();
        }

        #endregion
    }
}
