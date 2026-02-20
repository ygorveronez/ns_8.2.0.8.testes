using System;
using System.Collections.Generic;
using System.Linq;
using Dominio.ObjetosDeValor.Embarcador.Enumeradores;

namespace Servicos.Embarcador.Relatorios.Administrativo
{
    public class LicencaVeiculo : RelatorioBase<Dominio.ObjetosDeValor.Embarcador.Administrativo.FiltroPesquisaRelatorioLicencaVeiculo, Dominio.Relatorios.Embarcador.DataSource.Administrativo.LicencaVeiculo>
    {
        #region Atributos

        private readonly Repositorio.Embarcador.Transportadores.MotoristaLicenca _repositorioMotoristaLicenca;

        #endregion

        #region Construtores

        public LicencaVeiculo(Repositorio.UnitOfWork unitOfWork, AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware tipoServicoMultisoftware, AdminMultisoftware.Dominio.Entidades.Pessoas.Cliente clienteMultisoftware) : base(unitOfWork, tipoServicoMultisoftware, clienteMultisoftware)
        {
            _repositorioMotoristaLicenca = new Repositorio.Embarcador.Transportadores.MotoristaLicenca(_unitOfWork);
        }

        #endregion

        #region Métodos Protegidos Sobrescritos
        // TODO: ToList metodo override
        protected override List<Dominio.Relatorios.Embarcador.DataSource.Administrativo.LicencaVeiculo> ConsultarRegistros(Dominio.ObjetosDeValor.Embarcador.Administrativo.FiltroPesquisaRelatorioLicencaVeiculo filtrosPesquisa, List<Dominio.Relatorios.Embarcador.ObjetosDeValor.PropriedadeAgrupamento> propriedadesAgrupamento, Dominio.ObjetosDeValor.Embarcador.Consulta.ParametroConsulta parametrosConsulta)
        {
            return _repositorioMotoristaLicenca.RelatorioLicencaVeiculo(filtrosPesquisa, propriedadesAgrupamento, parametrosConsulta).ToList();
        }

        protected override int ContarRegistros(Dominio.ObjetosDeValor.Embarcador.Administrativo.FiltroPesquisaRelatorioLicencaVeiculo filtrosPesquisa, List<Dominio.Relatorios.Embarcador.ObjetosDeValor.PropriedadeAgrupamento> propriedadesAgrupamento)
        {
            return _repositorioMotoristaLicenca.ContarRelatorioLicencaVeiculo(filtrosPesquisa, propriedadesAgrupamento);
        }

        protected override string ObterCaminhoPaginaRelatorio()
        {
            return "Relatorios/Administrativo/LicencaVeiculo";
        }

        protected override List<Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro> ObterParametros(Dominio.ObjetosDeValor.Embarcador.Administrativo.FiltroPesquisaRelatorioLicencaVeiculo filtrosPesquisa, Dominio.ObjetosDeValor.Embarcador.Consulta.ParametroConsulta parametrosConsulta)
        {
            List<Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro> parametros = new List<Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro>();

            Repositorio.Embarcador.Financeiro.CentroResultado repCentroResultado = new Repositorio.Embarcador.Financeiro.CentroResultado(_unitOfWork);
            Repositorio.Embarcador.Configuracoes.Licenca repLicenca = new Repositorio.Embarcador.Configuracoes.Licenca(_unitOfWork);
            Repositorio.ModeloVeiculo repModeloVeiculo = new Repositorio.ModeloVeiculo(_unitOfWork);
            Repositorio.MarcaVeiculo repMarcaVeiculo = new Repositorio.MarcaVeiculo(_unitOfWork);
            Repositorio.Usuario repUsuario = new Repositorio.Usuario(_unitOfWork);

            Dominio.Entidades.Embarcador.Financeiro.CentroResultado centroResultado = filtrosPesquisa.CodigoCentroResultado > 0 ? repCentroResultado.BuscarPorCodigo(filtrosPesquisa.CodigoCentroResultado) : null;
            Dominio.Entidades.Embarcador.Configuracoes.Licenca licenca = filtrosPesquisa.CodigoLicenca > 0 ? repLicenca.BuscarPorCodigo(filtrosPesquisa.CodigoLicenca) : null;
            Dominio.Entidades.ModeloVeiculo modeloVeiculo = filtrosPesquisa.CodigoModelo > 0 ? repModeloVeiculo.BuscarPorCodigo(filtrosPesquisa.CodigoModelo, 0) : null;
            Dominio.Entidades.MarcaVeiculo marcaVeiculo = filtrosPesquisa.CodigoMarca > 0 ? repMarcaVeiculo.BuscarPorCodigo(filtrosPesquisa.CodigoMarca, 0) : null;
            Dominio.Entidades.Usuario funcionarioResponsavel = filtrosPesquisa.CodigoFuncionario > 0 ? repUsuario.BuscarPorCodigo(filtrosPesquisa.CodigoFuncionario) : null;

            parametros.Add(new Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro("Data", filtrosPesquisa.DataInicial, filtrosPesquisa.DataFinal));
            parametros.Add(new Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro("Descricao", filtrosPesquisa.Descricao));
            parametros.Add(new Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro("NumeroLicenca", filtrosPesquisa.NumeroLicenca));
            parametros.Add(new Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro("Licenca", licenca?.Descricao));
            parametros.Add(new Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro("FuncionarioResponsavel", funcionarioResponsavel?.Descricao));
            parametros.Add(new Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro("CentroResultado", centroResultado?.Descricao));
            parametros.Add(new Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro("Marca", marcaVeiculo?.Descricao));
            parametros.Add(new Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro("Modelo", modeloVeiculo?.Descricao));
            parametros.Add(new Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro("StatusLicenca", filtrosPesquisa.StatusLicenca.ObterDescricao()));
            parametros.Add(new Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro("StatusVeiculo", filtrosPesquisa.StatusVeiculo.ObterDescricao()));
            parametros.Add(new Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro("Renavam", filtrosPesquisa.Renavam));

            return parametros;
        }

        protected override string ObterPropriedadeOrdenarOuAgrupar(string propriedadeOrdenarOuAgrupar)
        {
            if (propriedadeOrdenarOuAgrupar.Contains("Formatado"))
                return propriedadeOrdenarOuAgrupar.Replace("Formatado", "");

            if (propriedadeOrdenarOuAgrupar.Contains("Formatada"))
                return propriedadeOrdenarOuAgrupar.Replace("Formatada", "");

            if (propriedadeOrdenarOuAgrupar.Contains("Descricao"))
                return propriedadeOrdenarOuAgrupar.Replace("Descricao", "");

            return propriedadeOrdenarOuAgrupar;
        }

        #endregion
    }
}