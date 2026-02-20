using System.Collections.Generic;
using System.Linq;
using Dominio.Relatorios.Embarcador.ObjetosDeValor;

namespace Servicos.Embarcador.Relatorios.Localidades
{
    public class Localidade : RelatorioBase<Dominio.ObjetosDeValor.Embarcador.Localidade.FiltroPesquisaRelatorioLocalidade, Dominio.Relatorios.Embarcador.DataSource.Localidades.Localidade>
    {
        #region Atributos
        private readonly Repositorio.Localidade _repositorioLocalidade;
        #endregion

        #region Construtores
        public Localidade(Repositorio.UnitOfWork unitOfWork, AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware tipoServicoMultisoftware, AdminMultisoftware.Dominio.Entidades.Pessoas.Cliente clienteMultisoftware) : base(unitOfWork, tipoServicoMultisoftware, clienteMultisoftware)
        {
            _repositorioLocalidade = new Repositorio.Localidade(_unitOfWork);
        }
        // TODO: ToList metodo override
        protected override List<Dominio.Relatorios.Embarcador.DataSource.Localidades.Localidade> ConsultarRegistros(Dominio.ObjetosDeValor.Embarcador.Localidade.FiltroPesquisaRelatorioLocalidade filtrosPesquisa, List<Dominio.Relatorios.Embarcador.ObjetosDeValor.PropriedadeAgrupamento> propriedadesAgrupamento, Dominio.ObjetosDeValor.Embarcador.Consulta.ParametroConsulta parametrosConsulta)
        {
            return _repositorioLocalidade.ConsultarRelatorio(filtrosPesquisa, propriedadesAgrupamento, parametrosConsulta).ToList();
        }

        protected override int ContarRegistros(Dominio.ObjetosDeValor.Embarcador.Localidade.FiltroPesquisaRelatorioLocalidade filtrosPesquisa, List<Dominio.Relatorios.Embarcador.ObjetosDeValor.PropriedadeAgrupamento> propriedadesAgrupamento)
        {
            return _repositorioLocalidade.ContarConsultaRelatorio(filtrosPesquisa, propriedadesAgrupamento);
        }

        protected override string ObterCaminhoPaginaRelatorio()
        {
            return "Relatorios/Localidades/Localidade";
        }

        protected override List<Parametro> ObterParametros(Dominio.ObjetosDeValor.Embarcador.Localidade.FiltroPesquisaRelatorioLocalidade filtrosPesquisa, Dominio.ObjetosDeValor.Embarcador.Consulta.ParametroConsulta parametrosConsulta)
        {
            List<Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro> parametros = new List<Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro>();

            Repositorio.Estado repositorioEstado = new Repositorio.Estado(_unitOfWork);
            Repositorio.Embarcador.Localidades.Regiao repositorioRegiao = new Repositorio.Embarcador.Localidades.Regiao(_unitOfWork);
            Repositorio.Pais repositorioPais = new Repositorio.Pais(_unitOfWork);

            List<Dominio.Entidades.Estado> estados = new List<Dominio.Entidades.Estado>();
            List<Dominio.Entidades.Pais> paises = new List<Dominio.Entidades.Pais>();
            List<Dominio.Entidades.Embarcador.Localidades.Regiao> regioes = new List<Dominio.Entidades.Embarcador.Localidades.Regiao>();

            if (filtrosPesquisa.Estados.Count > 0)
                estados = repositorioEstado.BuscarPorSiglas(filtrosPesquisa.Estados);

            if (filtrosPesquisa.Paises.Count > 0)
                paises = repositorioPais.BuscarPorCodigos(filtrosPesquisa.Paises);

            if (filtrosPesquisa.Regioes.Count > 0)
                regioes = repositorioRegiao.BuscarPorCodigos(filtrosPesquisa.Regioes);

            parametros.Add(new Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro("Descricao", filtrosPesquisa.Descricao));
            parametros.Add(new Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro("Pais", string.Join(", ", paises.Select(o => o.Descricao))));
            parametros.Add(new Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro("Estado", string.Join(", ", estados.Select(o => o.Descricao))));
            parametros.Add(new Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro("Regiao", string.Join(", ", regioes.Select(o => o.Descricao))));

            return parametros;
        }

        protected override string ObterPropriedadeOrdenarOuAgrupar(string propriedadeOrdenarOuAgrupar)
        {
            return propriedadeOrdenarOuAgrupar;
        }
        #endregion
    }
}
