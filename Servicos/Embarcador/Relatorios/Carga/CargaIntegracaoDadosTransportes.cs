using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Dominio.Enumeradores;
using Dominio.ObjetosDeValor.Embarcador.Enumeradores;
using Dominio.Relatorios.Embarcador.ObjetosDeValor;

namespace Servicos.Embarcador.Relatorios.Carga
{
    public class CargaIntegracaoDadosTransportes : RelatorioBase<Dominio.ObjetosDeValor.Embarcador.Carga.FiltroPesquisaRelatorioCargaIntegracaoDadosTransportes, Dominio.Relatorios.Embarcador.DataSource.Cargas.Integracao.CargaIntegracaoDadosTransportes>
    {
        #region Atributos

        private readonly Repositorio.Embarcador.Cargas.CargaIntegracaoDadosTransportes _repositorioCargaIntegracaoDadosTransportes;

        #endregion

        #region Construtores

        public CargaIntegracaoDadosTransportes(Repositorio.UnitOfWork unitOfWork, AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware tipoServicoMultisoftware, AdminMultisoftware.Dominio.Entidades.Pessoas.Cliente clienteMultisoftware) : base(unitOfWork, tipoServicoMultisoftware, clienteMultisoftware)
        {
            _repositorioCargaIntegracaoDadosTransportes = new Repositorio.Embarcador.Cargas.CargaIntegracaoDadosTransportes(_unitOfWork);
        }

        public CargaIntegracaoDadosTransportes(
            Repositorio.UnitOfWork unitOfWork,
            AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware tipoServicoMultisoftware,
            AdminMultisoftware.Dominio.Entidades.Pessoas.Cliente clienteMultisoftware,
            CancellationToken cancellationToken) : base(unitOfWork, tipoServicoMultisoftware, clienteMultisoftware)
        {
            _repositorioCargaIntegracaoDadosTransportes = new Repositorio.Embarcador.Cargas.CargaIntegracaoDadosTransportes(_unitOfWork, cancellationToken);
        }

        #endregion

        #region
        public async Task<IList<Dominio.Relatorios.Embarcador.DataSource.Cargas.Integracao.CargaIntegracaoDadosTransportes>> ConsultarRegistrosAsync(Dominio.ObjetosDeValor.Embarcador.Carga.FiltroPesquisaRelatorioCargaIntegracaoDadosTransportes filtrosPesquisa, List<PropriedadeAgrupamento> propriedadesAgrupamento, Dominio.ObjetosDeValor.Embarcador.Consulta.ParametroConsulta parametrosConsulta)
        {
            return await _repositorioCargaIntegracaoDadosTransportes.ConsultarRelatorioCargaIntegracaoDadosTransportesAsync(filtrosPesquisa, propriedadesAgrupamento, parametrosConsulta);
        }
        #endregion


        #region Métodos Protegidos Sobrescritos
        // TODO: ToList metodo override
        protected override List<Dominio.Relatorios.Embarcador.DataSource.Cargas.Integracao.CargaIntegracaoDadosTransportes> ConsultarRegistros(Dominio.ObjetosDeValor.Embarcador.Carga.FiltroPesquisaRelatorioCargaIntegracaoDadosTransportes filtrosPesquisa, List<PropriedadeAgrupamento> propriedadesAgrupamento, Dominio.ObjetosDeValor.Embarcador.Consulta.ParametroConsulta parametrosConsulta)
        {
            return _repositorioCargaIntegracaoDadosTransportes.ConsultarRelatorioCargaIntegracaoDadosTransportes(filtrosPesquisa, propriedadesAgrupamento, parametrosConsulta).ToList();
        }

        protected override int ContarRegistros(Dominio.ObjetosDeValor.Embarcador.Carga.FiltroPesquisaRelatorioCargaIntegracaoDadosTransportes filtrosPesquisa, List<PropriedadeAgrupamento> propriedadesAgrupamento)
        {
            return _repositorioCargaIntegracaoDadosTransportes.ContarConsultaRelatorioCargaIntegracaoDadosTransportes(filtrosPesquisa, propriedadesAgrupamento);
        }

        protected override string ObterCaminhoPaginaRelatorio()
        {
            return "Relatorios/Cargas/CargaIntegracaoDadosTransportes";
        }

        protected override List<Parametro> ObterParametros(Dominio.ObjetosDeValor.Embarcador.Carga.FiltroPesquisaRelatorioCargaIntegracaoDadosTransportes filtrosPesquisa, Dominio.ObjetosDeValor.Embarcador.Consulta.ParametroConsulta parametrosConsulta)
        {
            List<Parametro> parametros = new List<Parametro>();

            Repositorio.Embarcador.Pedidos.TipoOperacao repTipoOperacao = new Repositorio.Embarcador.Pedidos.TipoOperacao(_unitOfWork);
            Repositorio.Embarcador.Cargas.Carga repCarga = new Repositorio.Embarcador.Cargas.Carga(_unitOfWork);
            Repositorio.Embarcador.Filiais.Filial repFilial = new Repositorio.Embarcador.Filiais.Filial(_unitOfWork);
            Repositorio.Empresa repEmpresa = new Repositorio.Empresa(_unitOfWork);

            Dominio.Entidades.Embarcador.Pedidos.TipoOperacao tipoOperacao = filtrosPesquisa.CodigoTipoOperacao > 0 ? repTipoOperacao.BuscarPorCodigo(filtrosPesquisa.CodigoTipoOperacao) : null;
            Dominio.Entidades.Embarcador.Cargas.Carga carga = filtrosPesquisa.CodigoCarga > 0 ? repCarga.BuscarPorCodigo(filtrosPesquisa.CodigoCarga) : null;
            Dominio.Entidades.Embarcador.Filiais.Filial filial = filtrosPesquisa.CodigoFilial > 0 ? repFilial.BuscarPorCodigo(filtrosPesquisa.CodigoFilial) : null;
            Dominio.Entidades.Empresa transportador = filtrosPesquisa.CodigoTransportador > 0 ? repEmpresa.BuscarPorCodigo(filtrosPesquisa.CodigoTransportador) : null;

            parametros.Add(new Parametro("DataCarga", filtrosPesquisa.DataInicialCarga, filtrosPesquisa.DataFinalCarga));
            parametros.Add(new Parametro("DataIntegracao", filtrosPesquisa.DataInicioIntegracao, filtrosPesquisa.DataFinalIntegracao));
            parametros.Add(new Parametro("DataEncerramento", filtrosPesquisa.DataInicioEncerramento, filtrosPesquisa.DataFinalEncerramento));
            parametros.Add(new Parametro("TipoOperacao", tipoOperacao?.Descricao));
            parametros.Add(new Parametro("Carga", carga?.CodigoCargaEmbarcador));
            parametros.Add(new Parametro("TipoIntegracao", filtrosPesquisa.TipoIntegracao?.ObterDescricao()));
            parametros.Add(new Parametro("Situacao", filtrosPesquisa.Situacao?.ObterDescricao()));
            parametros.Add(new Parametro("Filial", filial?.Descricao));
            parametros.Add(new Parametro("Transportador", transportador?.Descricao));

            return parametros;
        }

        protected override string ObterPropriedadeOrdenarOuAgrupar(string propriedadeOrdenarOuAgrupar)
        {
            if (propriedadeOrdenarOuAgrupar.Contains("Formatado"))
                return propriedadeOrdenarOuAgrupar.Replace("Formatado", "");

            if (propriedadeOrdenarOuAgrupar.Contains("Formatada"))
                return propriedadeOrdenarOuAgrupar.Replace("Formatada", "");

            if (propriedadeOrdenarOuAgrupar == "DescricaoSituacaoCarga")
                return "SituacaoCarga";

            return propriedadeOrdenarOuAgrupar;
        }

        #endregion 
    }
}