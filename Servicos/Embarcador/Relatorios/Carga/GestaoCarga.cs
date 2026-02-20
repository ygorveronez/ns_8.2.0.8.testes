using System.Collections.Generic;
using System.Linq;
using Dominio.Relatorios.Embarcador.ObjetosDeValor;

namespace Servicos.Embarcador.Relatorios.Carga
{
    public class GestaoCarga : RelatorioBase<Dominio.ObjetosDeValor.Embarcador.Carga.FiltroPesquisaGestaoCarga, Dominio.Relatorios.Embarcador.DataSource.Cargas.Carga.RelatorioGestaoCarga>
    {
        #region Atributos

        private readonly Repositorio.Embarcador.Cargas.Carga _repositorioCarga;

        #endregion

        #region Construtores

        public GestaoCarga(Repositorio.UnitOfWork unitOfWork, AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware tipoServicoMultisoftware, AdminMultisoftware.Dominio.Entidades.Pessoas.Cliente clienteMultisoftware) : base(unitOfWork, tipoServicoMultisoftware, clienteMultisoftware)
        {
            _repositorioCarga = new Repositorio.Embarcador.Cargas.Carga(_unitOfWork);
        }

        #endregion

        #region Métodos Protegidos Sobrescritos
        //TODO: ToList metodo override
        protected override List<Dominio.Relatorios.Embarcador.DataSource.Cargas.Carga.RelatorioGestaoCarga> ConsultarRegistros(Dominio.ObjetosDeValor.Embarcador.Carga.FiltroPesquisaGestaoCarga filtrosPesquisa, List<PropriedadeAgrupamento> propriedadesAgrupamento, Dominio.ObjetosDeValor.Embarcador.Consulta.ParametroConsulta parametrosConsulta)
        {
            return _repositorioCarga.ConsultarRelatorioGestaoCarga(filtrosPesquisa, propriedadesAgrupamento, parametrosConsulta).ToList();
        }

        protected override int ContarRegistros(Dominio.ObjetosDeValor.Embarcador.Carga.FiltroPesquisaGestaoCarga filtrosPesquisa, List<PropriedadeAgrupamento> propriedadesAgrupamento)
        {
            return _repositorioCarga.ContarConsultaRelatorioGestaoCarga(filtrosPesquisa, propriedadesAgrupamento);
        }

        protected override string ObterCaminhoPaginaRelatorio()
        {
            return "Relatorios/Cargas/GestaoCarga";
        }

        protected override List<Parametro> ObterParametros(Dominio.ObjetosDeValor.Embarcador.Carga.FiltroPesquisaGestaoCarga filtrosPesquisa, Dominio.ObjetosDeValor.Embarcador.Consulta.ParametroConsulta parametrosConsulta)
        {
            List<Parametro> parametros = new List<Parametro>();

            Repositorio.Embarcador.Pessoas.GrupoPessoas repGrupoPessoas = new Repositorio.Embarcador.Pessoas.GrupoPessoas(_unitOfWork);
            Repositorio.Embarcador.Financeiro.CentroResultado repCentroResultado = new Repositorio.Embarcador.Financeiro.CentroResultado(_unitOfWork);
            Repositorio.Embarcador.Pedidos.TipoOperacao repTipoOperacao = new Repositorio.Embarcador.Pedidos.TipoOperacao(_unitOfWork);
            Repositorio.Cliente repCliente = new Repositorio.Cliente(_unitOfWork);

            List<Dominio.Entidades.Embarcador.Financeiro.CentroResultado> centrosResultado = filtrosPesquisa.CodigosCentroResultado.Count > 0 ? repCentroResultado.BuscarPorCodigos(filtrosPesquisa.CodigosCentroResultado) : new List<Dominio.Entidades.Embarcador.Financeiro.CentroResultado>();
            List<Dominio.Entidades.Embarcador.Pessoas.GrupoPessoas> gruposPessoas = filtrosPesquisa.CodigosGrupoPessoa.Count > 0 ? repGrupoPessoas.BuscarPorCodigos(filtrosPesquisa.CodigosGrupoPessoa) : new List<Dominio.Entidades.Embarcador.Pessoas.GrupoPessoas>();
            List<Dominio.Entidades.Embarcador.Pedidos.TipoOperacao> tiposOperacao = filtrosPesquisa.CodigosTipoOperacao.Count > 0 ? repTipoOperacao.BuscarPorCodigos(filtrosPesquisa.CodigosTipoOperacao) : new List<Dominio.Entidades.Embarcador.Pedidos.TipoOperacao>();
            List<Dominio.Entidades.Cliente> tomadores = filtrosPesquisa.CNPJsTomador.Count > 0 ? repCliente.BuscarPorCPFCNPJs(filtrosPesquisa.CNPJsTomador) : new List<Dominio.Entidades.Cliente>();

            parametros.Add(new Parametro("CentroResultado", string.Join(", ", centrosResultado?.Select(o => o.Descricao))));
            parametros.Add(new Parametro("GrupoPessoa", string.Join(", ", gruposPessoas?.Select(o => o.Descricao))));
            parametros.Add(new Parametro("TipoOperacao", string.Join(", ", tiposOperacao?.Select(o => o.Descricao))));
            parametros.Add(new Parametro("Tomador", string.Join(", ", tiposOperacao?.Select(o => o.Descricao))));
            parametros.Add(new Parametro("DataInicial", filtrosPesquisa.DataInicial > System.DateTime.MinValue ? filtrosPesquisa.DataInicial.ToString("dd/MM/yyyy") : ""));
            parametros.Add(new Parametro("DataFinal", filtrosPesquisa.DataFinal > System.DateTime.MinValue ? filtrosPesquisa.DataFinal.ToString("dd/MM/yyyy") : ""));
            //parametros.Add(new Parametro("StatusViagemControleEntrega", filtrosPesquisa.StatusViagemControleEntrega.);

            return parametros;
        }

        protected override string ObterPropriedadeOrdenarOuAgrupar(string propriedadeOrdenarOuAgrupar)
        {
            if (propriedadeOrdenarOuAgrupar.EndsWith("Formatada"))
                return propriedadeOrdenarOuAgrupar.Replace("Formatada", "");

            if (propriedadeOrdenarOuAgrupar.EndsWith("Formatado"))
                return propriedadeOrdenarOuAgrupar.Replace("Formatado", "");

            return propriedadeOrdenarOuAgrupar;
        }

        #endregion
    }
}