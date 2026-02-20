using System.Collections.Generic;
using System.Linq;

namespace Servicos.Embarcador.Relatorios.Ocorrencias
{
    public class TipoOcorrencia : RelatorioBase<Dominio.ObjetosDeValor.Embarcador.Ocorrencia.FiltroPesquisaTipoOcorrencia, Dominio.Relatorios.Embarcador.DataSource.Ocorrencias.TipoOcorrencia.TipoOcorrencia>
    {
        #region Atributos
        private readonly Repositorio.TipoDeOcorrenciaDeCTe _repositorioTipoOcorrenciaDeCTe;
        #endregion

        #region Construtores
        public TipoOcorrencia(Repositorio.UnitOfWork unitOfWork, AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware tipoServicoMultisoftware, AdminMultisoftware.Dominio.Entidades.Pessoas.Cliente clienteMultisoftware) : base(unitOfWork, tipoServicoMultisoftware, clienteMultisoftware)
        {
            _repositorioTipoOcorrenciaDeCTe = new Repositorio.TipoDeOcorrenciaDeCTe(_unitOfWork);
        }
        #endregion

        #region Métodos Protegidos Sobrescritos
        // TODO: ToList metodo override
        protected override List<Dominio.Relatorios.Embarcador.DataSource.Ocorrencias.TipoOcorrencia.TipoOcorrencia> ConsultarRegistros(Dominio.ObjetosDeValor.Embarcador.Ocorrencia.FiltroPesquisaTipoOcorrencia filtrosPesquisa, List<Dominio.Relatorios.Embarcador.ObjetosDeValor.PropriedadeAgrupamento> propriedadesAgrupamento, Dominio.ObjetosDeValor.Embarcador.Consulta.ParametroConsulta parametrosConsulta)
        {
            return _repositorioTipoOcorrenciaDeCTe.ConsultarRelatorioTipoOcorrencia(propriedadesAgrupamento, filtrosPesquisa, parametrosConsulta).ToList();
        }

        protected override int ContarRegistros(Dominio.ObjetosDeValor.Embarcador.Ocorrencia.FiltroPesquisaTipoOcorrencia filtrosPesquisa, List<Dominio.Relatorios.Embarcador.ObjetosDeValor.PropriedadeAgrupamento> propriedadesAgrupamento)
        {
            return _repositorioTipoOcorrenciaDeCTe.ContarConsultaRelatorioTipoOcorrencia(propriedadesAgrupamento, filtrosPesquisa);
        }

        protected override string ObterCaminhoPaginaRelatorio()
        {
            return "Relatorios/Ocorrencias/TipoOcorrencia";
        }

        protected override List<Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro> ObterParametros(
            Dominio.ObjetosDeValor.Embarcador.Ocorrencia.FiltroPesquisaTipoOcorrencia filtrosPesquisa,
            Dominio.ObjetosDeValor.Embarcador.Consulta.ParametroConsulta parametrosConsulta)
        {
            List<Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro> parametros = new List<Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro>();

            Repositorio.Embarcador.Relatorios.RelatorioControleGeracao repRelatorioControleGeracao = new Repositorio.Embarcador.Relatorios.RelatorioControleGeracao(_unitOfWork);
            Repositorio.Empresa repEmpresa = new Repositorio.Empresa(_unitOfWork);
            Repositorio.Cliente repCliente = new Repositorio.Cliente(_unitOfWork);
            Repositorio.Embarcador.Pessoas.GrupoPessoas repGrupoPessoas = new Repositorio.Embarcador.Pessoas.GrupoPessoas(_unitOfWork);

            Dominio.Entidades.Cliente pessoa = filtrosPesquisa.CpfCnpjPessoa > 0d ? repCliente.BuscarPorCPFCNPJ(filtrosPesquisa.CpfCnpjPessoa) : null;
            Dominio.Entidades.Embarcador.Pessoas.GrupoPessoas grupoPessoas = filtrosPesquisa.CodigoGrupoPessoas > 0 ? repGrupoPessoas.BuscarPorCodigo(filtrosPesquisa.CodigoGrupoPessoas) : null;

            parametros.Add(new Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro("Descricao", filtrosPesquisa.Descricao));
            parametros.Add(new Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro("Situacao", filtrosPesquisa.Situacao));
            parametros.Add(new Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro("Pessoa", pessoa?.Descricao));
            parametros.Add(new Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro("GrupoPessoas", grupoPessoas?.Descricao));

            return parametros;
        }

        protected override string ObterPropriedadeOrdenarOuAgrupar(string propriedadeOrdenarOuAgrupar)
        {
            return propriedadeOrdenarOuAgrupar;
        }
        #endregion
    }
}
