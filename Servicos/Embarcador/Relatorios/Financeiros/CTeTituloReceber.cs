using Dominio.ObjetosDeValor.Embarcador.Enumeradores;
using Dominio.Relatorios.Embarcador.ObjetosDeValor;
using System.Collections.Generic;
using System.Linq;

namespace Servicos.Embarcador.Relatorios.Financeiros
{
    public class CTeTituloReceber : RelatorioBase<Dominio.ObjetosDeValor.Embarcador.Financeiro.FiltroPesquisaRelatorioCTeTituloReceber, Dominio.Relatorios.Embarcador.DataSource.Financeiros.CTeTituloReceber>
    {
        #region Atributos

        private readonly Repositorio.ConhecimentoDeTransporteEletronico _repositorioCTeTituloReceber;

        #endregion

        #region Construtores

        public CTeTituloReceber(Repositorio.UnitOfWork unitOfWork, AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware tipoServicoMultisoftware, AdminMultisoftware.Dominio.Entidades.Pessoas.Cliente clienteMultisoftware) : base(unitOfWork, tipoServicoMultisoftware, clienteMultisoftware)
        {
            _repositorioCTeTituloReceber = new Repositorio.ConhecimentoDeTransporteEletronico(_unitOfWork);
        }

        #endregion

        #region Métodos Protegidos Sobrescritos
        // TODO: ToList metodo override
        protected override List<Dominio.Relatorios.Embarcador.DataSource.Financeiros.CTeTituloReceber> ConsultarRegistros(Dominio.ObjetosDeValor.Embarcador.Financeiro.FiltroPesquisaRelatorioCTeTituloReceber filtrosPesquisa, List<PropriedadeAgrupamento> propriedadesAgrupamento, Dominio.ObjetosDeValor.Embarcador.Consulta.ParametroConsulta parametrosConsulta)
        {
            return _repositorioCTeTituloReceber.ConsultarRelatorioCTeTituloReceber(filtrosPesquisa, propriedadesAgrupamento, parametrosConsulta).ToList();
        }

        protected override int ContarRegistros(Dominio.ObjetosDeValor.Embarcador.Financeiro.FiltroPesquisaRelatorioCTeTituloReceber filtrosPesquisa, List<PropriedadeAgrupamento> propriedadesAgrupamento)
        {
            return _repositorioCTeTituloReceber.ContarConsultaRelatorioCTeTituloReceber(filtrosPesquisa, propriedadesAgrupamento);
        }

        protected override string ObterCaminhoPaginaRelatorio()
        {
            return "Relatorios/Financeiros/CTeTituloReceber";
        }

        protected override List<Parametro> ObterParametros(Dominio.ObjetosDeValor.Embarcador.Financeiro.FiltroPesquisaRelatorioCTeTituloReceber filtrosPesquisa, Dominio.ObjetosDeValor.Embarcador.Consulta.ParametroConsulta parametrosConsulta)
        {
            List<Parametro> parametros = new List<Parametro>();

            Repositorio.Empresa repEmpresa = new Repositorio.Empresa(_unitOfWork);
            Repositorio.Cliente repCliente = new Repositorio.Cliente(_unitOfWork);
            Repositorio.Embarcador.Filiais.Filial repFilial = new Repositorio.Embarcador.Filiais.Filial(_unitOfWork);

            Dominio.Entidades.Empresa empresa = filtrosPesquisa.CodigoEmpresa > 0 ? repEmpresa.BuscarPorCodigo(filtrosPesquisa.CodigoEmpresa) : null;
            Dominio.Entidades.Cliente remetente = filtrosPesquisa.CnpjCpfRemetente > 0 ? repCliente.BuscarPorCPFCNPJ(filtrosPesquisa.CnpjCpfRemetente) : null;

            List<Dominio.Entidades.Embarcador.Filiais.Filial> filiais = filtrosPesquisa.CodigosFiliais.Count > 0 ? repFilial.BuscarPorCodigos(filtrosPesquisa.CodigosFiliais) : null;

            parametros.Add(new Parametro("Empresa", empresa?.RazaoSocial));
            parametros.Add(new Parametro("Numero", filtrosPesquisa.NumeroCTe));
            parametros.Add(new Parametro("DataInicial", filtrosPesquisa.DataInicio));
            parametros.Add(new Parametro("DataFinal", filtrosPesquisa.DataFim));
            parametros.Add(new Parametro("StatusTitulo", filtrosPesquisa.StatusTitulo.ObterDescricao()));
            parametros.Add(new Parametro("Agrupamento", parametrosConsulta != null ? parametrosConsulta.PropriedadeAgrupar : string.Empty));
            parametros.Add(new Parametro("Remetente", remetente?.Descricao));
            parametros.Add(new Parametro("Filial", filiais != null ? (from o in filiais select o.Descricao) : null));
            parametros.Add(new Parametro("NumeroFatura", filtrosPesquisa.NumeroFatura));

            return parametros;
        }

        protected override string ObterPropriedadeOrdenarOuAgrupar(string propriedadeOrdenarOuAgrupar)
        {
            if (propriedadeOrdenarOuAgrupar == "DescricaoDataEmissao")
                return "DataEmissao";

            if (propriedadeOrdenarOuAgrupar == "DescricaoSituacao")
                return "StatusTitulo";

            if (propriedadeOrdenarOuAgrupar == "DescricaoDataLiquidacao")
                return "DataLiquidacao";

            if (propriedadeOrdenarOuAgrupar == "DescricaoDataVencimento")
                return "DataVencimento";

            return propriedadeOrdenarOuAgrupar;
        }

        #endregion 
    }
}
