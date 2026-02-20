using System;
using System.Collections.Generic;
using System.Linq;
using Dominio.ObjetosDeValor.Embarcador.Enumeradores;

namespace Servicos.Embarcador.Relatorios.Administrativo
{
    public class Licenca : RelatorioBase<Dominio.ObjetosDeValor.Embarcador.Administrativo.FiltroPesquisaRelatorioLicenca, Dominio.Relatorios.Embarcador.DataSource.Administrativo.Licenca>
    {
        #region Atributos

        private readonly Repositorio.Embarcador.Transportadores.MotoristaLicenca _repositorioMotoristaLicenca;

        #endregion

        #region Construtores

        public Licenca(Repositorio.UnitOfWork unitOfWork, AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware tipoServicoMultisoftware, AdminMultisoftware.Dominio.Entidades.Pessoas.Cliente clienteMultisoftware) : base(unitOfWork, tipoServicoMultisoftware, clienteMultisoftware)
        {
            _repositorioMotoristaLicenca = new Repositorio.Embarcador.Transportadores.MotoristaLicenca(_unitOfWork);
        }

        #endregion

        #region Métodos Protegidos Sobrescritos
        // TODO: ToList metodo override
        protected override List<Dominio.Relatorios.Embarcador.DataSource.Administrativo.Licenca> ConsultarRegistros(Dominio.ObjetosDeValor.Embarcador.Administrativo.FiltroPesquisaRelatorioLicenca filtrosPesquisa, List<Dominio.Relatorios.Embarcador.ObjetosDeValor.PropriedadeAgrupamento> propriedadesAgrupamento, Dominio.ObjetosDeValor.Embarcador.Consulta.ParametroConsulta parametrosConsulta)
        {
            return _repositorioMotoristaLicenca.RelatorioLicenca(filtrosPesquisa, propriedadesAgrupamento, parametrosConsulta).ToList();
        }

        protected override int ContarRegistros(Dominio.ObjetosDeValor.Embarcador.Administrativo.FiltroPesquisaRelatorioLicenca filtrosPesquisa, List<Dominio.Relatorios.Embarcador.ObjetosDeValor.PropriedadeAgrupamento> propriedadesAgrupamento)
        {
            return _repositorioMotoristaLicenca.ContarRelatorioLicenca(filtrosPesquisa, propriedadesAgrupamento);
        }

        protected override string ObterCaminhoPaginaRelatorio()
        {
            return "Relatorios/Administrativo/Licenca";
        }

        protected override List<Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro> ObterParametros(Dominio.ObjetosDeValor.Embarcador.Administrativo.FiltroPesquisaRelatorioLicenca filtrosPesquisa, Dominio.ObjetosDeValor.Embarcador.Consulta.ParametroConsulta parametrosConsulta)
        {
            List<Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro> parametros = new List<Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro>();

            Repositorio.Embarcador.Transportadores.MotoristaLicenca repMotoristaLicenca = new Repositorio.Embarcador.Transportadores.MotoristaLicenca(_unitOfWork);
            Repositorio.Embarcador.Configuracoes.Licenca repLicenca = new Repositorio.Embarcador.Configuracoes.Licenca(_unitOfWork);
            Repositorio.Empresa repEmpresa = new Repositorio.Empresa(_unitOfWork);

            Dominio.Entidades.Empresa empresa = filtrosPesquisa.CodigoEmpresa > 0 ? repEmpresa.BuscarPorCodigo(filtrosPesquisa.CodigoEmpresa) : null;
            Dominio.Entidades.Embarcador.Configuracoes.Licenca licenca = filtrosPesquisa.CodigoTipoLicenca > 0 ? repLicenca.BuscarPorCodigo(filtrosPesquisa.CodigoTipoLicenca) : null;

            parametros.Add(new Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro("Data", filtrosPesquisa.DataInicial, filtrosPesquisa.DataFinal));
            parametros.Add(new Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro("Entidade", filtrosPesquisa.Entidade));
            parametros.Add(new Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro("Descricao", filtrosPesquisa.Descricao));
            parametros.Add(new Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro("NumeroLicenca", filtrosPesquisa.NumeroLicenca));
            parametros.Add(new Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro("TipoLicenca", filtrosPesquisa.TipoLicenca?.ObterDescricao()));
            parametros.Add(new Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro("Empresa", empresa?.RazaoSocial));
            parametros.Add(new Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro("Agrupamento", parametrosConsulta.PropriedadeAgrupar));
            parametros.Add(new Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro("Licenca", licenca?.Descricao));
            parametros.Add(new Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro("StatusEntidade", filtrosPesquisa.StatusEntidade.ObterDescricao()));

            return parametros;
        }

        protected override string ObterPropriedadeOrdenarOuAgrupar(string propriedadeOrdenarOuAgrupar)
        {
            if (propriedadeOrdenarOuAgrupar.Contains("Formatado"))
                return propriedadeOrdenarOuAgrupar.Replace("Formatado", "");

            if (propriedadeOrdenarOuAgrupar.Contains("Formatada"))
                return propriedadeOrdenarOuAgrupar.Replace("Formatada", "");

            return propriedadeOrdenarOuAgrupar;
        }

        #endregion
    }
}