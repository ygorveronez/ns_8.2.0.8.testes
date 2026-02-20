using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Dominio.Enumeradores;
using Dominio.ObjetosDeValor.Embarcador.Enumeradores;
using Dominio.Relatorios.Embarcador.ObjetosDeValor;

namespace Servicos.Embarcador.Relatorios.Carga
{
    public class CargaIntegracao : RelatorioBase<Dominio.ObjetosDeValor.Embarcador.Carga.FiltroPesquisaRelatorioCargaIntegracao, Dominio.Relatorios.Embarcador.DataSource.Cargas.Integracao.CargaIntegracao>
    {
        #region Atributos

        private readonly Repositorio.Embarcador.Cargas.CargaIntegracao _repositorioCargaIntegracao;

        #endregion

        #region Construtores

        public CargaIntegracao(Repositorio.UnitOfWork unitOfWork, AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware tipoServicoMultisoftware, AdminMultisoftware.Dominio.Entidades.Pessoas.Cliente clienteMultisoftware) : base(unitOfWork, tipoServicoMultisoftware, clienteMultisoftware)
        {
            _repositorioCargaIntegracao = new Repositorio.Embarcador.Cargas.CargaIntegracao(_unitOfWork);
        }

        public CargaIntegracao(Repositorio.UnitOfWork unitOfWork,
            AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware tipoServicoMultisoftware,
            AdminMultisoftware.Dominio.Entidades.Pessoas.Cliente clienteMultisoftware,
            CancellationToken cancellationToken) : base(unitOfWork, tipoServicoMultisoftware, clienteMultisoftware)
        {
            _repositorioCargaIntegracao = new Repositorio.Embarcador.Cargas.CargaIntegracao(_unitOfWork, cancellationToken);
        }

        #endregion

        #region Métodos assíncronos
        public async Task<IList<Dominio.Relatorios.Embarcador.DataSource.Cargas.Integracao.CargaIntegracao>> ConsultarRegistrosAsync(
            Dominio.ObjetosDeValor.Embarcador.Carga.FiltroPesquisaRelatorioCargaIntegracao filtrosPesquisa,
            List<PropriedadeAgrupamento> propriedadesAgrupamento,
            Dominio.ObjetosDeValor.Embarcador.Consulta.ParametroConsulta parametrosConsulta)
        {
            return await _repositorioCargaIntegracao.ConsultarRelatorioCargaIntegracaoAsync(filtrosPesquisa, propriedadesAgrupamento, parametrosConsulta);
        }
        #endregion

        #region Métodos Protegidos Sobrescritos
        // TODO: ToList metodo override
        protected override List<Dominio.Relatorios.Embarcador.DataSource.Cargas.Integracao.CargaIntegracao> ConsultarRegistros(Dominio.ObjetosDeValor.Embarcador.Carga.FiltroPesquisaRelatorioCargaIntegracao filtrosPesquisa, List<PropriedadeAgrupamento> propriedadesAgrupamento, Dominio.ObjetosDeValor.Embarcador.Consulta.ParametroConsulta parametrosConsulta)
        {
            return _repositorioCargaIntegracao.ConsultarRelatorioCargaIntegracao(filtrosPesquisa, propriedadesAgrupamento, parametrosConsulta).ToList();
        }

        protected override int ContarRegistros(Dominio.ObjetosDeValor.Embarcador.Carga.FiltroPesquisaRelatorioCargaIntegracao filtrosPesquisa, List<PropriedadeAgrupamento> propriedadesAgrupamento)
        {
            return _repositorioCargaIntegracao.ContarConsultaRelatorioCargaIntegracao(filtrosPesquisa, propriedadesAgrupamento);
        }

        protected override string ObterCaminhoPaginaRelatorio()
        {
            return "Relatorios/Cargas/CargaIntegracao";
        }

        protected override List<Parametro> ObterParametros(Dominio.ObjetosDeValor.Embarcador.Carga.FiltroPesquisaRelatorioCargaIntegracao filtrosPesquisa, Dominio.ObjetosDeValor.Embarcador.Consulta.ParametroConsulta parametrosConsulta)
        {
            List<Parametro> parametros = new List<Parametro>();

            Repositorio.Embarcador.Pedidos.TipoOperacao repTipoOperacao = new Repositorio.Embarcador.Pedidos.TipoOperacao(_unitOfWork);
            Repositorio.Embarcador.Cargas.TipoIntegracao repTipoIntegracao = new Repositorio.Embarcador.Cargas.TipoIntegracao(_unitOfWork);
            Repositorio.Embarcador.Cargas.Carga repCarga = new Repositorio.Embarcador.Cargas.Carga(_unitOfWork);
            Repositorio.Embarcador.Filiais.Filial repFilial = new Repositorio.Embarcador.Filiais.Filial(_unitOfWork);
            Repositorio.Empresa repEmpresa = new Repositorio.Empresa(_unitOfWork);

            Dominio.Entidades.Embarcador.Pedidos.TipoOperacao tipoOperacao = filtrosPesquisa.CodigoTipoOperacao > 0 ? repTipoOperacao.BuscarPorCodigo(filtrosPesquisa.CodigoTipoOperacao) : null;
            Dominio.Entidades.Embarcador.Cargas.Carga carga = filtrosPesquisa.CodigoCarga > 0 ? repCarga.BuscarPorCodigo(filtrosPesquisa.CodigoCarga) : null;
            Dominio.Entidades.Embarcador.Cargas.TipoIntegracao tipoIntegracao = filtrosPesquisa.CodigoTipoIntegracao > 0 ? repTipoIntegracao.BuscarPorCodigo(filtrosPesquisa.CodigoTipoIntegracao, false) : null;
            List<Dominio.Entidades.Embarcador.Filiais.Filial> filiais = filtrosPesquisa.CodigosFiliais.Count > 0 ? repFilial.BuscarPorCodigos(filtrosPesquisa.CodigosFiliais) : null;
            Dominio.Entidades.Empresa transportador = filtrosPesquisa.CodigoTransportador > 0 ? repEmpresa.BuscarPorCodigo(filtrosPesquisa.CodigoTransportador) : null;

            parametros.Add(new Parametro("DataCarga", filtrosPesquisa.DataInicialCarga, filtrosPesquisa.DataFinalCarga));
            parametros.Add(new Parametro("DataIntegracao", filtrosPesquisa.DataInicioIntegracao, filtrosPesquisa.DataFinalIntegracao));
            parametros.Add(new Parametro("TipoOperacao", tipoOperacao?.Descricao));
            parametros.Add(new Parametro("Carga", carga?.CodigoCargaEmbarcador));
            parametros.Add(new Parametro("TipoIntegracao", tipoIntegracao?.Descricao));
            parametros.Add(new Parametro("Situacao", filtrosPesquisa.Situacao?.ObterDescricao()));
            parametros.Add(new Parametro("Filial", filiais != null ? string.Join(", ", filiais.Select(o => o.Descricao)) : null));
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