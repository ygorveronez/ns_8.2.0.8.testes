using System;
using System.Collections.Generic;
using System.Linq;

namespace Servicos.Embarcador.Relatorios.MDFes
{
    public class MDFesAverbados : RelatorioBase<Dominio.ObjetosDeValor.Embarcador.MDFe.FiltroPesquisaMDFesAverbadosRelatorio, Dominio.Relatorios.Embarcador.DataSource.MDFe.MDFesAverbados>
    {
        #region Atributos

        private readonly Repositorio.AverbacaoMDFe repositorio;

        #endregion

        #region Construtores

        public MDFesAverbados(Repositorio.UnitOfWork unitOfWork, AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware tipoServicoMultisoftware, AdminMultisoftware.Dominio.Entidades.Pessoas.Cliente clienteMultisoftware) : base(unitOfWork, tipoServicoMultisoftware, clienteMultisoftware)
        {
            repositorio = new Repositorio.AverbacaoMDFe(_unitOfWork);
        }

        #endregion

        #region Métodos Protegidos Sobrescritos
        // TODO: ToList metodo override
        protected override List<Dominio.Relatorios.Embarcador.DataSource.MDFe.MDFesAverbados> ConsultarRegistros(Dominio.ObjetosDeValor.Embarcador.MDFe.FiltroPesquisaMDFesAverbadosRelatorio filtrosPesquisa, List<Dominio.Relatorios.Embarcador.ObjetosDeValor.PropriedadeAgrupamento> propriedadesAgrupamento, Dominio.ObjetosDeValor.Embarcador.Consulta.ParametroConsulta parametrosConsulta)
        {
            return repositorio.ConsultarRelatorioMDFes(filtrosPesquisa, propriedadesAgrupamento, parametrosConsulta).ToList();
        }

        protected override int ContarRegistros(Dominio.ObjetosDeValor.Embarcador.MDFe.FiltroPesquisaMDFesAverbadosRelatorio filtrosPesquisa, List<Dominio.Relatorios.Embarcador.ObjetosDeValor.PropriedadeAgrupamento> propriedadesAgrupamento)
        {
            return repositorio.ContarConsultaRelatorioMDFes(filtrosPesquisa, propriedadesAgrupamento);
        }

        protected override string ObterCaminhoPaginaRelatorio()
        {
            return "Relatorios/MDFe/MDFesAverbados";
        }

        protected override List<Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro> ObterParametros(Dominio.ObjetosDeValor.Embarcador.MDFe.FiltroPesquisaMDFesAverbadosRelatorio filtrosPesquisa, Dominio.ObjetosDeValor.Embarcador.Consulta.ParametroConsulta parametrosConsulta)
        {
            List<Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro> parametros = new List<Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro>();

            if (filtrosPesquisa.DataInicialEmissao != DateTime.MinValue)
                parametros.Add(new Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro("DataInicialEmissao", filtrosPesquisa.DataInicialEmissao.ToString("dd/MM/yyyy"), true));
            else
                parametros.Add(new Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro("DataInicialEmissao", false));

            if (filtrosPesquisa.DataFinalEmissao != DateTime.MinValue)
                parametros.Add(new Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro("DataFinalEmissao", filtrosPesquisa.DataFinalEmissao.ToString("dd/MM/yyyy"), true));
            else
                parametros.Add(new Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro("DataFinalEmissao", false));

            if (filtrosPesquisa.CodigoTransportador > 0 && _tipoServicoMultisoftware != AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiCTe)
            {
                Repositorio.Empresa repEmpresa = new Repositorio.Empresa(_unitOfWork);
                Dominio.Entidades.Empresa transportador = repEmpresa.BuscarPorCodigo(filtrosPesquisa.CodigoTransportador);
                parametros.Add(new Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro("Transportador", transportador.RazaoSocial, true));
            }
            else
                parametros.Add(new Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro("Transportador", false));

            if (filtrosPesquisa.CodigoSeguradora > 0)
            {
                Repositorio.Embarcador.Seguros.Seguradora repSeguradora = new Repositorio.Embarcador.Seguros.Seguradora(_unitOfWork);
                Dominio.Entidades.Embarcador.Seguros.Seguradora seguradora = repSeguradora.BuscarPorCodigo(filtrosPesquisa.CodigoSeguradora);
                parametros.Add(new Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro("Seguradora", seguradora.Nome, true));
            }
            else
                parametros.Add(new Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro("Seguradora", false));

            if(filtrosPesquisa.Status.HasValue)
            {
                parametros.Add(new Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro("Status", Dominio.Enumeradores.StatusAverbacaoMDFeHelper.Descricao(filtrosPesquisa.Status.Value), true));
            } else
            {
                parametros.Add(new Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro("Status", false));
            }

            if (filtrosPesquisa.CodigoModeloDocumentoFiscal > 0)
            {
                Repositorio.ModeloDocumentoFiscal repModeloDocumentoFiscal = new Repositorio.ModeloDocumentoFiscal(_unitOfWork);
                Dominio.Entidades.ModeloDocumentoFiscal modeloDocumentoFiscal = repModeloDocumentoFiscal.BuscarPorCodigo(filtrosPesquisa.CodigoModeloDocumentoFiscal, false);
                parametros.Add(new Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro("ModeloDocumentoFiscal", modeloDocumentoFiscal.Descricao, true));
            }
            else
                parametros.Add(new Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro("ModeloDocumentoFiscal", false));

            if (parametrosConsulta != null && !string.IsNullOrWhiteSpace(parametrosConsulta.PropriedadeAgrupar))
                parametros.Add(new Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro("Agrupamento", parametrosConsulta.PropriedadeAgrupar, true));
            else
                parametros.Add(new Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro("Agrupamento", false));

            return parametros;
        }

        protected override string ObterPropriedadeOrdenarOuAgrupar(string propriedadeOrdenarOuAgrupar)
        {
            if (propriedadeOrdenarOuAgrupar == "DescricaoSituacao")
                return "Situacao";

            if (propriedadeOrdenarOuAgrupar == "Codigo")
                return "Numero";

            if (propriedadeOrdenarOuAgrupar == "DataEmissaoFormatada")
                return "DataEmissao";

            if (propriedadeOrdenarOuAgrupar == "DataAverbacaoFormatada")
                return "DataAverbacao";

            if (propriedadeOrdenarOuAgrupar == "DescricaoSituacaoMDFe")
                return "SituacaoMDFe";

            if (propriedadeOrdenarOuAgrupar == "DescricaoAverbadora")
                return "Averbadora";

            return propriedadeOrdenarOuAgrupar;
        }

        #endregion 
    }
}