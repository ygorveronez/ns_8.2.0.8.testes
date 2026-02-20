using Dominio.Relatorios.Embarcador.ObjetosDeValor;
using System.Collections.Generic;
using System.Linq;

namespace Servicos.Embarcador.Relatorios.Escrituracao
{
    public class Competencia : RelatorioBase<Dominio.ObjetosDeValor.Embarcador.Escrituracao.FiltroRelatorioCompetencia, Dominio.Relatorios.Embarcador.DataSource.Escrituracao.FreteCompetencia>
    {
        #region Atributos

        private readonly Repositorio.Embarcador.Escrituracao.DocumentoEscrituracao _repositorioDocumentoEscrituracao;

        #endregion

        #region Construtores

        public Competencia(Repositorio.UnitOfWork unitOfWork, AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware tipoServicoMultisoftware, AdminMultisoftware.Dominio.Entidades.Pessoas.Cliente clienteMultisoftware) : base(unitOfWork, tipoServicoMultisoftware, clienteMultisoftware)
        {
            _repositorioDocumentoEscrituracao = new Repositorio.Embarcador.Escrituracao.DocumentoEscrituracao(_unitOfWork);
        }

        #endregion

        #region Métodos Protegidos Sobrescritos
        // TODO: ToList metodo override
        protected override List<Dominio.Relatorios.Embarcador.DataSource.Escrituracao.FreteCompetencia> ConsultarRegistros(Dominio.ObjetosDeValor.Embarcador.Escrituracao.FiltroRelatorioCompetencia filtrosPesquisa, List<Dominio.Relatorios.Embarcador.ObjetosDeValor.PropriedadeAgrupamento> propriedadesAgrupamento, Dominio.ObjetosDeValor.Embarcador.Consulta.ParametroConsulta parametrosConsulta)
        {
            return _repositorioDocumentoEscrituracao.ConsultarRelatorioCompetencia(filtrosPesquisa, propriedadesAgrupamento, parametrosConsulta).ToList();
        }

        protected override int ContarRegistros(Dominio.ObjetosDeValor.Embarcador.Escrituracao.FiltroRelatorioCompetencia filtrosPesquisa, List<Dominio.Relatorios.Embarcador.ObjetosDeValor.PropriedadeAgrupamento> propriedadesAgrupamento)
        {
            return _repositorioDocumentoEscrituracao.ContarConsultaRelatorioCompetencia(filtrosPesquisa, propriedadesAgrupamento);
        }

        protected override string ObterCaminhoPaginaRelatorio()
        {
            return "Relatorios/Escrituracao/Competencia";
        }

        protected override List<Parametro> ObterParametros(Dominio.ObjetosDeValor.Embarcador.Escrituracao.FiltroRelatorioCompetencia filtrosPesquisa, Dominio.ObjetosDeValor.Embarcador.Consulta.ParametroConsulta parametrosConsulta)
        {
            List<Parametro> parametros = new List<Parametro>();

            Repositorio.Embarcador.Filiais.Filial repFilial = new Repositorio.Embarcador.Filiais.Filial(_unitOfWork);
            Repositorio.Empresa repEmpresa = new Repositorio.Empresa(_unitOfWork);
            Repositorio.Cliente repCliente = new Repositorio.Cliente(_unitOfWork);
            Repositorio.Embarcador.Pedidos.TipoOperacao repTipoOperacao = new Repositorio.Embarcador.Pedidos.TipoOperacao(_unitOfWork);

            Dominio.Entidades.Embarcador.Filiais.Filial filial = filtrosPesquisa.CodigoFilial > 0 ? repFilial.BuscarPorCodigo(filtrosPesquisa.CodigoFilial) : null;
            Dominio.Entidades.Empresa transportador = filtrosPesquisa.CodigoTransportador > 0 ? repEmpresa.BuscarPorCodigo(filtrosPesquisa.CodigoTransportador) : null;
            Dominio.Entidades.Cliente tomador = filtrosPesquisa.CnpjCpfTomador > 0 ? repCliente.BuscarPorCPFCNPJ(filtrosPesquisa.CnpjCpfTomador) : null;
            Dominio.Entidades.Embarcador.Pedidos.TipoOperacao tipoOperacao = filtrosPesquisa.CodigoTipoOperacao > 0 ? repTipoOperacao.BuscarPorCodigo(filtrosPesquisa.CodigoTipoOperacao) : null;

            parametros.Add(new Parametro("DataEmissaoInicial", filtrosPesquisa.DataEmissaoInicial));
            parametros.Add(new Parametro("DataEmissaoFinal", filtrosPesquisa.DataEmissaoFinal));
            parametros.Add(new Parametro("DataCargaInicial", filtrosPesquisa.DataCargaInicial));
            parametros.Add(new Parametro("DataCargaFinal", filtrosPesquisa.DataCargaFinal));
            parametros.Add(new Parametro("Filial", filial?.Descricao));
            parametros.Add(new Parametro("Transportador", transportador?.Descricao));
            parametros.Add(new Parametro("Tomador", tomador?.Descricao));
            parametros.Add(new Parametro("TipoOperacao", tipoOperacao?.Descricao));
            parametros.Add(new Parametro("DataEmissaoCTe", filtrosPesquisa.DataEmissaoCTeInicial, filtrosPesquisa.DataEmissaoCTeFinal));
            parametros.Add(new Parametro("DataEmissaoNota", filtrosPesquisa.DataEmissaoNotaInicial, filtrosPesquisa.DataEmissaoNotaFinal));
            parametros.Add(new Parametro("VisualizarTambemDocumentosAguardandoProvisao", filtrosPesquisa.VisualizarTambemDocumentosAguardandoProvisao ? "Sim" : ""));
            parametros.Add(new Parametro("CodigoCargaEmbarcador", filtrosPesquisa.CodigoCargaEmbarcador));
            parametros.Add(new Parametro("NumeroValePedagio", filtrosPesquisa.NumeroValePedagio));

            return parametros;
        }

        protected override string ObterPropriedadeOrdenarOuAgrupar(string propriedadeOrdenarOuAgrupar)
        {
            {
                if (propriedadeOrdenarOuAgrupar == "Tomador") propriedadeOrdenarOuAgrupar = "NomeTomador";
                else if (propriedadeOrdenarOuAgrupar == "Transportador") propriedadeOrdenarOuAgrupar = "TransportadorTomador";
                else if (propriedadeOrdenarOuAgrupar == "Destinatario") propriedadeOrdenarOuAgrupar = "NomeDestinatario";
                else if (propriedadeOrdenarOuAgrupar == "Remetente") propriedadeOrdenarOuAgrupar = "NomeRemetente";
                else if (propriedadeOrdenarOuAgrupar.EndsWith("Formatada")) propriedadeOrdenarOuAgrupar = propriedadeOrdenarOuAgrupar.Replace("Formatada", "");
                else if (propriedadeOrdenarOuAgrupar == "Ocorrencia" || propriedadeOrdenarOuAgrupar == "NumeroCte" || propriedadeOrdenarOuAgrupar == "NumeroNFS" || propriedadeOrdenarOuAgrupar == "ICMS" || propriedadeOrdenarOuAgrupar == "ValorISS") propriedadeOrdenarOuAgrupar = "_" + propriedadeOrdenarOuAgrupar;
            }

            return propriedadeOrdenarOuAgrupar;
        }

        #endregion 
    }
}