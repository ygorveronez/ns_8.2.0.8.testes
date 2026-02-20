using System.Collections.Generic;
using System.Linq;

namespace Servicos.Embarcador.Relatorios.Documentos
{
    public class SerieDocumentos : RelatorioBase<Dominio.ObjetosDeValor.Embarcador.Documentos.FiltroPesquisaSerieDocumentos, Dominio.Relatorios.Embarcador.DataSource.Documentos.SerieDocumentos>
    {
        #region Atributos

        private readonly Repositorio.EmpresaSerie _repositorioEmpresaSerie;

        #endregion

        #region Construtores

        public SerieDocumentos(Repositorio.UnitOfWork unitOfWork, AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware tipoServicoMultisoftware, AdminMultisoftware.Dominio.Entidades.Pessoas.Cliente clienteMultisoftware) : base(unitOfWork, tipoServicoMultisoftware, clienteMultisoftware)
        {
            _repositorioEmpresaSerie = new Repositorio.EmpresaSerie(_unitOfWork);
        }

        #endregion

        #region Métodos Protegidos Sobrescritos
        // TODO: ToList metodo override
        protected override List<Dominio.Relatorios.Embarcador.DataSource.Documentos.SerieDocumentos> ConsultarRegistros(Dominio.ObjetosDeValor.Embarcador.Documentos.FiltroPesquisaSerieDocumentos filtrosPesquisa, List<Dominio.Relatorios.Embarcador.ObjetosDeValor.PropriedadeAgrupamento> propriedadesAgrupamento, Dominio.ObjetosDeValor.Embarcador.Consulta.ParametroConsulta parametrosConsulta)
        {
            return _repositorioEmpresaSerie.ConsultarRelatorio(filtrosPesquisa, propriedadesAgrupamento, parametrosConsulta).ToList();
        }

        protected override int ContarRegistros(Dominio.ObjetosDeValor.Embarcador.Documentos.FiltroPesquisaSerieDocumentos filtrosPesquisa, List<Dominio.Relatorios.Embarcador.ObjetosDeValor.PropriedadeAgrupamento> propriedadesAgrupamento)
        {
            return _repositorioEmpresaSerie.ContarConsultaRelatorio(filtrosPesquisa, propriedadesAgrupamento);
        }

        protected override string ObterCaminhoPaginaRelatorio()
        {
            return "Relatorios/Documentos/SerieDocumentosTransportador";
        }

        protected override List<Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro> ObterParametros(Dominio.ObjetosDeValor.Embarcador.Documentos.FiltroPesquisaSerieDocumentos filtrosPesquisa, Dominio.ObjetosDeValor.Embarcador.Consulta.ParametroConsulta parametrosConsulta)
        {
            List<Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro> parametros = new List<Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro>();

            Repositorio.Empresa repEmpresa = new Repositorio.Empresa(_unitOfWork);
            Repositorio.EmpresaSerie repEmpresaSerie = new Repositorio.EmpresaSerie(_unitOfWork);
            Repositorio.ModeloDocumentoFiscal repModeloDocumentoFiscal = new Repositorio.ModeloDocumentoFiscal(_unitOfWork);

            Dominio.Entidades.Empresa empresa = filtrosPesquisa.CodigoEmpresa > 0 ? repEmpresa.BuscarPorCodigo(filtrosPesquisa.CodigoEmpresa) : null;
            Dominio.Entidades.EmpresaSerie empresaSerie = filtrosPesquisa.CodigoSerie > 0 ? repEmpresaSerie.BuscarPorCodigo(filtrosPesquisa.CodigoSerie) : null;
            List<Dominio.Entidades.ModeloDocumentoFiscal> modelosDocumentosFiscais = filtrosPesquisa.ModelosDocumentosFiscais?.Count > 0 ? repModeloDocumentoFiscal.BuscarPorCodigos(filtrosPesquisa.ModelosDocumentosFiscais) : null;

            parametros.Add(new Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro("Empresa", empresa?.Descricao));
            parametros.Add(new Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro("Serie", empresaSerie?.Descricao));
            parametros.Add(new Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro("ModeloDocumentoFiscal", modelosDocumentosFiscais?.Count > 0 ? string.Join(", ", modelosDocumentosFiscais.Select(o => o.Descricao)) : null));

            return parametros;
        }

        protected override string ObterPropriedadeOrdenarOuAgrupar(string propriedadeOrdenarOuAgrupar)
        {
            if (propriedadeOrdenarOuAgrupar == "CNPJEmpresaFormatado")
                return "CNPJEmpresa";

            if (propriedadeOrdenarOuAgrupar == "TipoSerieFormatada")
                return "TipoSerie";

            return propriedadeOrdenarOuAgrupar;
        }

        #endregion 
    }
}