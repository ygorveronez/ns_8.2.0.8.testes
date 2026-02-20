using System;
using System.Collections.Generic;
using System.Linq;

namespace Servicos.Embarcador.Relatorios.Escrituracao
{
    public class FreteContabil : RelatorioBase<Dominio.ObjetosDeValor.Embarcador.Escrituracao.FiltroRelatorioFreteContabil, Dominio.Relatorios.Embarcador.DataSource.Escrituracao.FreteContabil>
    {
        #region Atributos

        private readonly Repositorio.Embarcador.Escrituracao.DocumentoEscrituracao _repositorioDocumentoEscrituracao;

        #endregion

        #region Construtores

        public FreteContabil(Repositorio.UnitOfWork unitOfWork, AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware tipoServicoMultisoftware, AdminMultisoftware.Dominio.Entidades.Pessoas.Cliente clienteMultisoftware) : base(unitOfWork, tipoServicoMultisoftware, clienteMultisoftware)
        {
            _repositorioDocumentoEscrituracao = new Repositorio.Embarcador.Escrituracao.DocumentoEscrituracao(_unitOfWork);
        }

        #endregion

        #region Métodos Protegidos Sobrescritos
        // TODO: ToList metodo override
        protected override List<Dominio.Relatorios.Embarcador.DataSource.Escrituracao.FreteContabil> ConsultarRegistros(Dominio.ObjetosDeValor.Embarcador.Escrituracao.FiltroRelatorioFreteContabil filtrosPesquisa, List<Dominio.Relatorios.Embarcador.ObjetosDeValor.PropriedadeAgrupamento> propriedadesAgrupamento, Dominio.ObjetosDeValor.Embarcador.Consulta.ParametroConsulta parametrosConsulta)
        {
            return _repositorioDocumentoEscrituracao.ConsultarRelatorio(filtrosPesquisa, propriedadesAgrupamento, parametrosConsulta).ToList();
        }

        protected override int ContarRegistros(Dominio.ObjetosDeValor.Embarcador.Escrituracao.FiltroRelatorioFreteContabil filtrosPesquisa, List<Dominio.Relatorios.Embarcador.ObjetosDeValor.PropriedadeAgrupamento> propriedadesAgrupamento)
        {
            return _repositorioDocumentoEscrituracao.ContarConsultaRelatorio(filtrosPesquisa, propriedadesAgrupamento);
        }

        protected override string ObterCaminhoPaginaRelatorio()
        {
            return "Relatorios/Escrituracao/FreteContabil";
        }

        protected override List<Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro> ObterParametros(Dominio.ObjetosDeValor.Embarcador.Escrituracao.FiltroRelatorioFreteContabil filtrosPesquisa, Dominio.ObjetosDeValor.Embarcador.Consulta.ParametroConsulta parametrosConsulta)
        {
            List<Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro> parametros = new List<Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro>();

            Repositorio.Embarcador.Escrituracao.DocumentoEscrituracao repDocumentoEscrituracao = new Repositorio.Embarcador.Escrituracao.DocumentoEscrituracao(_unitOfWork);
            Repositorio.Embarcador.Filiais.Filial repFilial = new Repositorio.Embarcador.Filiais.Filial(_unitOfWork);
            Repositorio.Empresa repEmpresa = new Repositorio.Empresa(_unitOfWork);
            Repositorio.Cliente repCliente = new Repositorio.Cliente(_unitOfWork);
            Repositorio.Embarcador.Financeiro.CentroResultado repCentroResultado = new Repositorio.Embarcador.Financeiro.CentroResultado(_unitOfWork);
            Repositorio.Embarcador.Financeiro.PlanoConta repPlanoConta = new Repositorio.Embarcador.Financeiro.PlanoConta(_unitOfWork);
            Repositorio.Embarcador.Cargas.Carga repCarga = new Repositorio.Embarcador.Cargas.Carga(_unitOfWork);

            var _filial = filtrosPesquisa.Filial > 0 ? repFilial.BuscarPorCodigo(filtrosPesquisa.Filial) : null;
            var _transportador = filtrosPesquisa.Transportador > 0 ? repEmpresa.BuscarPorCodigo(filtrosPesquisa.Transportador) : null;
            var _tomador = filtrosPesquisa.Tomador > 0 ? repCliente.BuscarPorCPFCNPJ(filtrosPesquisa.Tomador) : null;
            var _centros = filtrosPesquisa.CentroResultado?.Count > 0 ? repCentroResultado.BuscarPorCodigos(filtrosPesquisa.CentroResultado) : null;
            var _contas = filtrosPesquisa.ContaContabil?.Count > 0 ? repPlanoConta.BuscarPorCodigos(filtrosPesquisa.ContaContabil) : null;
            Dominio.Entidades.Embarcador.Cargas.Carga _cargas = filtrosPesquisa.CodigoCarga > 0 ? repCarga.BuscarPorCodigo(filtrosPesquisa.CodigoCarga) : null;

            parametros.Add(new Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro("DataEmissaoInicial", filtrosPesquisa.DataEmissaoInicial));
            parametros.Add(new Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro("DataEmissaoFinal", filtrosPesquisa.DataEmissaoFinal));
            parametros.Add(new Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro("DataLancamentoInicial", filtrosPesquisa.DataLancamentoInicial));
            parametros.Add(new Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro("DataLancamentoFinal", filtrosPesquisa.DataLancamentoFinal));
            parametros.Add(new Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro("Filial", _filial?.Descricao));
            parametros.Add(new Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro("Transportador", _transportador?.Descricao));
            parametros.Add(new Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro("Tomador", _tomador?.Descricao));
            parametros.Add(new Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro("CentroResultado", _centros?.Count > 0 ? String.Join("; ", from o in _centros select o.BuscarDescricao) : string.Empty));
            parametros.Add(new Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro("ContaContabil", _contas?.Count > 0 ? String.Join("; ", from o in _contas select o.BuscarDescricao) : string.Empty));
            parametros.Add(new Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro("Carga", _cargas?.CodigoCargaEmbarcador));

            return parametros;

        }

        protected override string ObterPropriedadeOrdenarOuAgrupar(string propriedadeOrdenarOuAgrupar)
        {
            if (propriedadeOrdenarOuAgrupar == "Tomador")
                return "NomeTomador";
            
            if (propriedadeOrdenarOuAgrupar == "Empresa")
                return "RazaoEmpresa";
            
            if (propriedadeOrdenarOuAgrupar == "Remetente")
                return "NomeRemetente";
            
            if (propriedadeOrdenarOuAgrupar == "Destinatario")
                return "NomeDestinatario";
            
            if (propriedadeOrdenarOuAgrupar == "NumeroOcorrencia")
                return "Ocorrencia";
            
            if (propriedadeOrdenarOuAgrupar == "TipoContabilizacao")
                return "_TipoContabilizacao";

            return propriedadeOrdenarOuAgrupar;
        }

        #endregion 
    }
}