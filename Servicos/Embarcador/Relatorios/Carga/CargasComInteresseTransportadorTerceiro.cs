using Dominio.ObjetosDeValor.Embarcador.Enumeradores;
using Dominio.Enumeradores;
using System.Collections.Generic;
using Dominio.Relatorios.Embarcador.ObjetosDeValor;
using System.Linq;

namespace Servicos.Embarcador.Relatorios.Carga
{
    public class CargasComInteresseTransportadorTerceiro : RelatorioBase<Dominio.ObjetosDeValor.Embarcador.Carga.FiltroPesquisaCargasComInteresseTransportadorTerceiro, Dominio.Relatorios.Embarcador.DataSource.Cargas.CargasComInteresseTransportadorTerceiro.CargasComInteresseTransportadorTerceiro>
    {
        #region Atributos

        private readonly Repositorio.Embarcador.Cargas.CargaJanelaCarregamentoTransportador _repositorio;

        #endregion

        #region Construtores

        public CargasComInteresseTransportadorTerceiro(Repositorio.UnitOfWork unitOfWork, AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware tipoServicoMultisoftware, AdminMultisoftware.Dominio.Entidades.Pessoas.Cliente clienteMultisoftware) : base(unitOfWork, tipoServicoMultisoftware, clienteMultisoftware)
        {
            _repositorio = new Repositorio.Embarcador.Cargas.CargaJanelaCarregamentoTransportador(_unitOfWork);
        }

        #endregion

        #region Métodos Protegidos Sobrescritos
        // TODO: ToList metodo override
        protected override List<Dominio.Relatorios.Embarcador.DataSource.Cargas.CargasComInteresseTransportadorTerceiro.CargasComInteresseTransportadorTerceiro> ConsultarRegistros(Dominio.ObjetosDeValor.Embarcador.Carga.FiltroPesquisaCargasComInteresseTransportadorTerceiro filtrosPesquisa, List<Dominio.Relatorios.Embarcador.ObjetosDeValor.PropriedadeAgrupamento> propriedadesAgrupamento, Dominio.ObjetosDeValor.Embarcador.Consulta.ParametroConsulta parametrosConsulta)
        {
            return _repositorio.ConsultarRelatorio(filtrosPesquisa, propriedadesAgrupamento, parametrosConsulta).ToList();
        }

        protected override int ContarRegistros(Dominio.ObjetosDeValor.Embarcador.Carga.FiltroPesquisaCargasComInteresseTransportadorTerceiro filtrosPesquisa, List<Dominio.Relatorios.Embarcador.ObjetosDeValor.PropriedadeAgrupamento> propriedadesAgrupamento)
        {
            return _repositorio.ContarConsultaRelatorio(filtrosPesquisa, propriedadesAgrupamento);
        }

        protected override string ObterCaminhoPaginaRelatorio()
        {
            return "Relatorios/Cargas/CargasComInteresseTransportadorTerceiro";
        }

        protected override List<Parametro> ObterParametros(Dominio.ObjetosDeValor.Embarcador.Carga.FiltroPesquisaCargasComInteresseTransportadorTerceiro filtrosPesquisa, Dominio.ObjetosDeValor.Embarcador.Consulta.ParametroConsulta parametrosConsulta)
        {
            List<Parametro> parametros = new List<Parametro>();

            Repositorio.Veiculo repVeiculo = new Repositorio.Veiculo(_unitOfWork);
            Repositorio.Embarcador.Cargas.Carga repCarga = new Repositorio.Embarcador.Cargas.Carga(_unitOfWork);
            Repositorio.Embarcador.Logistica.CentroCarregamento repCentroCarregamentoa = new Repositorio.Embarcador.Logistica.CentroCarregamento(_unitOfWork);


            Dominio.Entidades.Veiculo veiculo = filtrosPesquisa.CodigoVeiculo > 0 ? repVeiculo.BuscarPorCodigo(filtrosPesquisa.CodigoVeiculo) : null;
            Dominio.Entidades.Embarcador.Cargas.Carga carga = filtrosPesquisa.CodigoCarga > 0 ? repCarga.BuscarPorCodigo(filtrosPesquisa.CodigoCarga) : null;
            Dominio.Entidades.Embarcador.Logistica.CentroCarregamento centroCarregamento = filtrosPesquisa.CodigoCentroCarregamento > 0 ? repCentroCarregamentoa.BuscarPorCodigo(filtrosPesquisa.CodigoCentroCarregamento) : null;

            parametros.Add(new Parametro("Data", filtrosPesquisa.DataInicial, filtrosPesquisa.DataFinal));
            parametros.Add(new Parametro("SituacaoDescricao", filtrosPesquisa.Situacao?.ObterDescricao()));
            parametros.Add(new Parametro("DescricaoVeiculo", veiculo?.Descricao));
            parametros.Add(new Parametro("DescricaoCarga", carga?.Descricao));
            parametros.Add(new Parametro("DescricaoCentroCarregamento", centroCarregamento?.Descricao));


            return parametros;
        }

        protected override string ObterPropriedadeOrdenarOuAgrupar(string propriedadeOrdenarOuAgrupar)
        {
            if (propriedadeOrdenarOuAgrupar == "DataFormatada")
                return "Data";

            return propriedadeOrdenarOuAgrupar;
        }

        #endregion 
    }
}