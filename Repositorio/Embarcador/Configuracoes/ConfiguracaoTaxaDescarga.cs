using System.Collections.Generic;
using System.Linq;
using System.Linq.Dynamic.Core;

namespace Repositorio.Embarcador.Configuracoes
{
    public class ConfiguracaoTaxaDescarga : RepositorioBase<Dominio.Entidades.Embarcador.Configuracoes.ConfiguracaoTaxaDescarga>
    {
        public ConfiguracaoTaxaDescarga(UnitOfWork unitOfWork) : base(unitOfWork) { }

        public Dominio.Entidades.Embarcador.Configuracoes.ConfiguracaoTaxaDescarga BuscarPorCodigo(int codigo)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Configuracoes.ConfiguracaoTaxaDescarga>();
            var result = from obj in query where obj.Codigo == codigo select obj;
            return result.FirstOrDefault();
        }

        public int ContarConsulta(Dominio.ObjetosDeValor.Embarcador.Configuracoes.FiltroPesquisaConfiguracaoTaxaDescarga filtrosPesquisa)
        {
            var consultaConfiguracaoTaxaDescarga = Consultar(filtrosPesquisa);

            return consultaConfiguracaoTaxaDescarga.Count();
        }

        public List<Dominio.Entidades.Embarcador.Configuracoes.ConfiguracaoTaxaDescarga> Consultar(Dominio.ObjetosDeValor.Embarcador.Configuracoes.FiltroPesquisaConfiguracaoTaxaDescarga filtrosPesquisa, Dominio.ObjetosDeValor.Embarcador.Consulta.ParametroConsulta parametrosConsulta)
        {
            var consultaIrregularidade = Consultar(filtrosPesquisa);

            return ObterLista(consultaIrregularidade, parametrosConsulta);
        }


        private IQueryable<Dominio.Entidades.Embarcador.Configuracoes.ConfiguracaoTaxaDescarga> Consultar(Dominio.ObjetosDeValor.Embarcador.Configuracoes.FiltroPesquisaConfiguracaoTaxaDescarga filtrosPesquisa)
        {
            var consultaConfiguracaoTaxaDescarga = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Configuracoes.ConfiguracaoTaxaDescarga>();

            if (!string.IsNullOrEmpty(filtrosPesquisa.Descricao))
                consultaConfiguracaoTaxaDescarga = consultaConfiguracaoTaxaDescarga.Where(obj => obj.Descricao.Contains(filtrosPesquisa.Descricao));

            if (filtrosPesquisa.Situacao != Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoAtivaPesquisa.Todos)
            {
                bool ativa = filtrosPesquisa.Situacao == Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoAtivaPesquisa.Ativa;
                consultaConfiguracaoTaxaDescarga = consultaConfiguracaoTaxaDescarga.Where(obj => obj.Ativa == ativa);
            }

            return consultaConfiguracaoTaxaDescarga;
        }

    }
}
