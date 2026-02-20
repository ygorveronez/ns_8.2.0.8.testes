using Dominio.ObjetosDeValor.Embarcador.Enumeradores;
using Dominio.Enumeradores;
using System.Collections.Generic;
using Dominio.Relatorios.Embarcador.ObjetosDeValor;
using System.Linq;

namespace Servicos.Embarcador.Relatorios.Logistica
{
    public class HistoricoJanelaCarregamento : RelatorioBase<Dominio.ObjetosDeValor.Embarcador.Logistica.FiltroPesquisaHistoricoJanelaCarregamento, Dominio.Relatorios.Embarcador.DataSource.Logistica.HistoricoJanelaCarregamento>
    {
        #region Atributos

        private readonly Repositorio.Embarcador.Cargas.CargaJanelaCarregamentoTransportador _repositorio;

        #endregion

        #region Construtores

        public HistoricoJanelaCarregamento(Repositorio.UnitOfWork unitOfWork, AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware tipoServicoMultisoftware, AdminMultisoftware.Dominio.Entidades.Pessoas.Cliente clienteMultisoftware) : base(unitOfWork, tipoServicoMultisoftware, clienteMultisoftware)
        {
            _repositorio = new Repositorio.Embarcador.Cargas.CargaJanelaCarregamentoTransportador(_unitOfWork);
        }

        #endregion

        #region Métodos Protegidos Sobrescritos
        // TODO: ToList metodo override
        protected override List<Dominio.Relatorios.Embarcador.DataSource.Logistica.HistoricoJanelaCarregamento> ConsultarRegistros(Dominio.ObjetosDeValor.Embarcador.Logistica.FiltroPesquisaHistoricoJanelaCarregamento filtrosPesquisa, List<Dominio.Relatorios.Embarcador.ObjetosDeValor.PropriedadeAgrupamento> propriedadesAgrupamento, Dominio.ObjetosDeValor.Embarcador.Consulta.ParametroConsulta parametrosConsulta)
        {
            return _repositorio.ConsultarRelatorioHistoricoJanelaCarregamento(filtrosPesquisa, propriedadesAgrupamento, parametrosConsulta).ToList();
        }

        protected override int ContarRegistros(Dominio.ObjetosDeValor.Embarcador.Logistica.FiltroPesquisaHistoricoJanelaCarregamento filtrosPesquisa, List<Dominio.Relatorios.Embarcador.ObjetosDeValor.PropriedadeAgrupamento> propriedadesAgrupamento)
        {
            return _repositorio.ContarConsultaRelatorioHistoricoJanelaCarregamento(filtrosPesquisa, propriedadesAgrupamento);
        }

        protected override string ObterCaminhoPaginaRelatorio()
        {
            return "Relatorios/Logistica/HistoricoJanelaCarregamento";
        }

        protected override List<Parametro> ObterParametros(Dominio.ObjetosDeValor.Embarcador.Logistica.FiltroPesquisaHistoricoJanelaCarregamento filtrosPesquisa, Dominio.ObjetosDeValor.Embarcador.Consulta.ParametroConsulta parametrosConsulta)
        {
            List<Parametro> parametros = new List<Parametro>();

            Repositorio.Embarcador.Logistica.MotivoRetiradaFilaCarregamento repMotivo = new Repositorio.Embarcador.Logistica.MotivoRetiradaFilaCarregamento(_unitOfWork);
            Repositorio.Embarcador.Cargas.Carga repCarga = new Repositorio.Embarcador.Cargas.Carga(_unitOfWork);
            Repositorio.Embarcador.Logistica.CentroCarregamento repCentroCarregamentoa = new Repositorio.Embarcador.Logistica.CentroCarregamento(_unitOfWork);


            Dominio.Entidades.Embarcador.Logistica.MotivoRetiradaFilaCarregamento  motivo = filtrosPesquisa.CodigoMotivoRecusa > 0 ? repMotivo.BuscarPorCodigo(filtrosPesquisa.CodigoMotivoRecusa) : null;
            Dominio.Entidades.Embarcador.Cargas.Carga carga = filtrosPesquisa.CodigoCarga > 0 ? repCarga.BuscarPorCodigo(filtrosPesquisa.CodigoCarga) : null;
            Dominio.Entidades.Embarcador.Logistica.CentroCarregamento centroCarregamento = filtrosPesquisa.CodigoCentroCarregamento > 0 ? repCentroCarregamentoa.BuscarPorCodigo(filtrosPesquisa.CodigoCentroCarregamento) : null;

            parametros.Add(new Parametro("Data", filtrosPesquisa.DataInicial, filtrosPesquisa.DataFinal));
            parametros.Add(new Parametro("DescricaoMotivoRecusa", motivo?.Descricao));
            parametros.Add(new Parametro("DescricaoCarga", carga?.Descricao));
            parametros.Add(new Parametro("DescricaoCentroCarregamento", centroCarregamento?.Descricao));


            return parametros;
        }

        protected override string ObterPropriedadeOrdenarOuAgrupar(string propriedadeOrdenarOuAgrupar)
        {
            if (propriedadeOrdenarOuAgrupar == "DataRecusa")
                return "DataRecusa";

            return propriedadeOrdenarOuAgrupar;
        }

        #endregion 
    }
}