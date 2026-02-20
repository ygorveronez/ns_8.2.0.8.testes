using System.Collections.Generic;
using System.Linq;

namespace Repositorio.Embarcador.Chamados
{
    public class ConfiguracaoTempoChamado : RepositorioBase<Dominio.Entidades.Embarcador.Chamados.ConfiguracaoTempoChamado>
    {
        public ConfiguracaoTempoChamado(UnitOfWork unitOfWork) : base(unitOfWork) { }

        #region Métodos Públicos

        public List<Dominio.Entidades.Embarcador.Chamados.ConfiguracaoTempoChamado> Consultar(Dominio.ObjetosDeValor.Embarcador.Chamado.FiltroPesquisaConfiguracaoTempoChamado filtrosPesquisa, Dominio.ObjetosDeValor.Embarcador.Consulta.ParametroConsulta parametroConsulta)
        {
            var result = Consultar(filtrosPesquisa);

            return ObterLista(result, parametroConsulta);
        }

        public int ContarConsulta(Dominio.ObjetosDeValor.Embarcador.Chamado.FiltroPesquisaConfiguracaoTempoChamado filtrosPesquisa)
        {
            var result = Consultar(filtrosPesquisa);

            return result.Count();
        }

        public List<Dominio.Entidades.Embarcador.Chamados.ConfiguracaoTempoChamado> BuscarPorClienteTipoOperacaoFilial(double cnpjCpfCliente, int codigoTipoOperacao, int codigoFilial)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Chamados.ConfiguracaoTempoChamado>();

            var result = from obj in query
                         where
                             obj.Ativo
                             && (obj.Cliente.CPF_CNPJ == cnpjCpfCliente || obj.Cliente == null)
                             && (obj.TipoOperacao.Codigo == codigoTipoOperacao || obj.TipoOperacao == null)
                             && (obj.Filial.Codigo == codigoFilial || obj.Filial == null)
                         select obj;

            return result.ToList();
        }

        #endregion

        #region Métodos Privados

        private IQueryable<Dominio.Entidades.Embarcador.Chamados.ConfiguracaoTempoChamado> Consultar(Dominio.ObjetosDeValor.Embarcador.Chamado.FiltroPesquisaConfiguracaoTempoChamado filtrosPesquisa)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Chamados.ConfiguracaoTempoChamado>();

            var result = from obj in query select obj;

            if (!string.IsNullOrWhiteSpace(filtrosPesquisa.Descricao))
                result = result.Where(obj => obj.Descricao.Contains(filtrosPesquisa.Descricao));

            if (filtrosPesquisa.Ativo == Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoAtivoPesquisa.Ativo)
                result = result.Where(o => o.Ativo);
            else if (filtrosPesquisa.Ativo == Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoAtivoPesquisa.Inativo)
                result = result.Where(o => !o.Ativo);

            if (filtrosPesquisa.CnpjCpfCliente > 0)
                result = result.Where(o => o.Cliente.CPF_CNPJ == filtrosPesquisa.CnpjCpfCliente);

            if (filtrosPesquisa.CodigoFilial > 0)
                result = result.Where(o => o.Filial.Codigo == filtrosPesquisa.CodigoFilial);

            if (filtrosPesquisa.CodigoTipoOperacao > 0)
                result = result.Where(o => o.TipoOperacao.Codigo == filtrosPesquisa.CodigoTipoOperacao);

            return result;
        }

        #endregion
    }
}
