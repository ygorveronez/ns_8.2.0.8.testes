using System.Collections.Generic;
using System.Linq;

namespace Servicos.Embarcador.Relatorios.Pessoas
{
    public class PessoaDescarga : RelatorioBase<Dominio.ObjetosDeValor.Embarcador.Pessoas.FiltroPesquisaRelatorioPessoaDescarga, Dominio.Relatorios.Embarcador.DataSource.Pessoas.PessoaDescarga>
    {
        #region Atributos

        private readonly Repositorio.Embarcador.Pessoas.ClienteDescarga _repositorioClienteDescarga;

        #endregion

        #region Construtores

        public PessoaDescarga(Repositorio.UnitOfWork unitOfWork, AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware tipoServicoMultisoftware, AdminMultisoftware.Dominio.Entidades.Pessoas.Cliente clienteMultisoftware) : base(unitOfWork, tipoServicoMultisoftware, clienteMultisoftware)
        {
            _repositorioClienteDescarga = new Repositorio.Embarcador.Pessoas.ClienteDescarga(_unitOfWork);
        }

        #endregion

        #region Métodos Protegidos Sobrescritos
        // TODO: ToList metodo override
        protected override List<Dominio.Relatorios.Embarcador.DataSource.Pessoas.PessoaDescarga> ConsultarRegistros(Dominio.ObjetosDeValor.Embarcador.Pessoas.FiltroPesquisaRelatorioPessoaDescarga filtrosPesquisa, List<Dominio.Relatorios.Embarcador.ObjetosDeValor.PropriedadeAgrupamento> propriedadesAgrupamento, Dominio.ObjetosDeValor.Embarcador.Consulta.ParametroConsulta parametrosConsulta)
        {
            return _repositorioClienteDescarga.ConsultarRelatorio(filtrosPesquisa, propriedadesAgrupamento, parametrosConsulta).ToList();
        }

        protected override int ContarRegistros(Dominio.ObjetosDeValor.Embarcador.Pessoas.FiltroPesquisaRelatorioPessoaDescarga filtrosPesquisa, List<Dominio.Relatorios.Embarcador.ObjetosDeValor.PropriedadeAgrupamento> propriedadesAgrupamento)
        {
            return _repositorioClienteDescarga.ContarConsultaRelatorio(filtrosPesquisa, propriedadesAgrupamento);
        }

        protected override string ObterCaminhoPaginaRelatorio()
        {
            return "Relatorios/Pessoas/PessoaDescarga";
        }

        protected override List<Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro> ObterParametros(Dominio.ObjetosDeValor.Embarcador.Pessoas.FiltroPesquisaRelatorioPessoaDescarga filtrosPesquisa, Dominio.ObjetosDeValor.Embarcador.Consulta.ParametroConsulta parametrosConsulta)
        {
            List<Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro> parametros = new List<Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro>();

            Repositorio.Cliente repCliente = new Repositorio.Cliente(_unitOfWork);

            Dominio.Entidades.Cliente clienteOrigem = filtrosPesquisa.PessoaOrigem > 0d ? repCliente.BuscarPorCPFCNPJ(filtrosPesquisa.PessoaOrigem) : null;
            Dominio.Entidades.Cliente clienteDestino = filtrosPesquisa.PessoaDestino > 0 ? repCliente.BuscarPorCPFCNPJ(filtrosPesquisa.PessoaDestino) : null;

            parametros.Add(new Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro("PessoaOrigem", clienteOrigem?.Nome));
            parametros.Add(new Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro("PessoaDestino", clienteDestino?.Nome));
            parametros.Add(new Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro("DeixaReboqueDescarga", filtrosPesquisa.PessoaDeixaReboqueParaDescarga == true ? "Sim" : "Não"));

            return parametros;
        }

        protected override string ObterPropriedadeOrdenarOuAgrupar(string propriedadeOrdenarOuAgrupar)
        {
            if (propriedadeOrdenarOuAgrupar == "DeixaReboqueParaDescargaFormatada")
                return "DeixaReboqueParaDescarga";

            return propriedadeOrdenarOuAgrupar;
        }

        #endregion 
    }
}