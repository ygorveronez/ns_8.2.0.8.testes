using Dominio.ObjetosDeValor.Embarcador.Enumeradores;
using Dominio.Enumeradores;
using System.Collections.Generic;
using System.Linq;

namespace Servicos.Embarcador.Relatorios.CTes
{
    public class CTesAverbados : RelatorioBase<Dominio.ObjetosDeValor.Embarcador.Seguro.FiltroPesquisaRelatorioCTesAverbados, Dominio.Relatorios.Embarcador.DataSource.Seguros.CTesAverbados>
    {
        #region Atributos

        private readonly Repositorio.AverbacaoCTe _repositorioAverbacaoCTe;

        #endregion

        #region Construtores

        public CTesAverbados(Repositorio.UnitOfWork unitOfWork, AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware tipoServicoMultisoftware, AdminMultisoftware.Dominio.Entidades.Pessoas.Cliente clienteMultisoftware) : base(unitOfWork, tipoServicoMultisoftware, clienteMultisoftware)
        {
            _repositorioAverbacaoCTe = new Repositorio.AverbacaoCTe(_unitOfWork);
        }

        #endregion

        #region Métodos Protegidos Sobrescritos
        // TODO: ToList metodo override
        protected override List<Dominio.Relatorios.Embarcador.DataSource.Seguros.CTesAverbados> ConsultarRegistros(Dominio.ObjetosDeValor.Embarcador.Seguro.FiltroPesquisaRelatorioCTesAverbados filtrosPesquisa, List<Dominio.Relatorios.Embarcador.ObjetosDeValor.PropriedadeAgrupamento> propriedadesAgrupamento, Dominio.ObjetosDeValor.Embarcador.Consulta.ParametroConsulta parametrosConsulta)
        {
            return _repositorioAverbacaoCTe.ConsultarRelatorio(filtrosPesquisa, propriedadesAgrupamento, parametrosConsulta).ToList();
        }

        protected override int ContarRegistros(Dominio.ObjetosDeValor.Embarcador.Seguro.FiltroPesquisaRelatorioCTesAverbados filtrosPesquisa, List<Dominio.Relatorios.Embarcador.ObjetosDeValor.PropriedadeAgrupamento> propriedadesAgrupamento)
        {
            return _repositorioAverbacaoCTe.ContarConsultaRelatorio(filtrosPesquisa, propriedadesAgrupamento);
        }

        protected override string ObterCaminhoPaginaRelatorio()
        {
            return "Relatorios/Seguros/CTesAverbados";
        }

        protected override List<Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro> ObterParametros(Dominio.ObjetosDeValor.Embarcador.Seguro.FiltroPesquisaRelatorioCTesAverbados filtrosPesquisa, Dominio.ObjetosDeValor.Embarcador.Consulta.ParametroConsulta parametrosConsulta)
        {
            List<Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro> parametros = new List<Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro>();

            Repositorio.Empresa repEmpresa = new Repositorio.Empresa(_unitOfWork);
            Repositorio.Embarcador.Seguros.Seguradora repSeguradora = new Repositorio.Embarcador.Seguros.Seguradora(_unitOfWork);
            Repositorio.ModeloDocumentoFiscal repModeloDocumentoFiscal = new Repositorio.ModeloDocumentoFiscal(_unitOfWork);

            List<string> transportadores = filtrosPesquisa.CodigosTransportador?.Count > 0 ? repEmpresa.BuscarDescricoesPorCodigos(filtrosPesquisa.CodigosTransportador) : null;
            Dominio.Entidades.Embarcador.Seguros.Seguradora seguradora = filtrosPesquisa.CodigoSeguradora > 0 ? repSeguradora.BuscarPorCodigo(filtrosPesquisa.CodigoSeguradora) : null;
            Dominio.Entidades.ModeloDocumentoFiscal modeloDocumentoFiscal = filtrosPesquisa.CodigoModeloDocumentoFiscal > 0 ? repModeloDocumentoFiscal.BuscarPorCodigo(filtrosPesquisa.CodigoModeloDocumentoFiscal, false) : null;

            parametros.Add(new Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro("PeriodoEmissao", filtrosPesquisa.DataInicialEmissao, filtrosPesquisa.DataFinalEmissao));
            parametros.Add(new Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro("PeriodoServico", filtrosPesquisa.DataServicoInicial, filtrosPesquisa.DataServicoFinal));
            parametros.Add(new Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro("Transportador", transportadores));
            parametros.Add(new Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro("Seguradora", seguradora?.Nome));
            parametros.Add(new Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro("ModeloDocumentoFiscal", modeloDocumentoFiscal?.Descricao));
            parametros.Add(new Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro("SituacaoFechamento", filtrosPesquisa.SituacaoFechamento?.ObterDescricao()));
            parametros.Add(new Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro("Status", filtrosPesquisa.Status?.ObterDescricao()));
            parametros.Add(new Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro("Agrupamento", parametrosConsulta?.PropriedadeAgrupar));

            return parametros;
        }

        protected override string ObterPropriedadeOrdenarOuAgrupar(string propriedadeOrdenarOuAgrupar)
        {
            if (propriedadeOrdenarOuAgrupar.Contains("Formatado"))
                return propriedadeOrdenarOuAgrupar.Replace("Formatado", "");

            if (propriedadeOrdenarOuAgrupar.Contains("Formatada"))
                return propriedadeOrdenarOuAgrupar.Replace("Formatada", "");

            if (propriedadeOrdenarOuAgrupar == "DescricaoStatusCTe")
                return "StatusCTe";

            return propriedadeOrdenarOuAgrupar;
        }

        #endregion
    }
}