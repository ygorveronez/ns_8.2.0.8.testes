using System.Collections.Generic;
using System.Linq;

namespace Servicos.Embarcador.Relatorios.Logistica
{
    public class JanelaCarregamentoIntegracao : RelatorioBase<Dominio.ObjetosDeValor.Embarcador.Logistica.FiltroPesquisaRelatorioJanelaCarregamentoIntegracao, Dominio.Relatorios.Embarcador.DataSource.Logistica.JanelaCarregamentoIntegracao>
    {
        #region Atributos

        private readonly Repositorio.Embarcador.Cargas.CargaJanelaCarregamentoIntegracao _repositorioJanelaCarregamentoIntegracao;

        #endregion

        #region Construtores

        public JanelaCarregamentoIntegracao(Repositorio.UnitOfWork unitOfWork, AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware tipoServicoMultisoftware, AdminMultisoftware.Dominio.Entidades.Pessoas.Cliente clienteMultisoftware) : base(unitOfWork, tipoServicoMultisoftware, clienteMultisoftware)
        {
            _repositorioJanelaCarregamentoIntegracao = new Repositorio.Embarcador.Cargas.CargaJanelaCarregamentoIntegracao(_unitOfWork);
        }

        #endregion

        #region Métodos Protegidos Sobrescritos
        // TODO: ToList metodo override
        protected override List<Dominio.Relatorios.Embarcador.DataSource.Logistica.JanelaCarregamentoIntegracao> ConsultarRegistros(Dominio.ObjetosDeValor.Embarcador.Logistica.FiltroPesquisaRelatorioJanelaCarregamentoIntegracao filtrosPesquisa, List<Dominio.Relatorios.Embarcador.ObjetosDeValor.PropriedadeAgrupamento> propriedadesAgrupamento, Dominio.ObjetosDeValor.Embarcador.Consulta.ParametroConsulta parametrosConsulta)
        {
            return _repositorioJanelaCarregamentoIntegracao.ConsultarRelatorioJanelaCarregamentoIntegracao (filtrosPesquisa, propriedadesAgrupamento, parametrosConsulta).ToList();
        }

        protected override int ContarRegistros(Dominio.ObjetosDeValor.Embarcador.Logistica.FiltroPesquisaRelatorioJanelaCarregamentoIntegracao filtrosPesquisa, List<Dominio.Relatorios.Embarcador.ObjetosDeValor.PropriedadeAgrupamento> propriedadesAgrupamento)
        {
            return _repositorioJanelaCarregamentoIntegracao.ContarConsultaRelatorioJanelaCarregamentoIntegracao(filtrosPesquisa, propriedadesAgrupamento);
        }

        protected override string ObterCaminhoPaginaRelatorio()
        {
            return "Relatorios/Logistica/JanelaCarregamentoIntegracao";
        }

        protected override List<Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro> ObterParametros(Dominio.ObjetosDeValor.Embarcador.Logistica.FiltroPesquisaRelatorioJanelaCarregamentoIntegracao filtrosPesquisa, Dominio.ObjetosDeValor.Embarcador.Consulta.ParametroConsulta parametrosConsulta)
        {
            List<Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro> parametros = new List<Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro>();
            Repositorio.Embarcador.Cargas.Carga repCarga = new Repositorio.Embarcador.Cargas.Carga(_unitOfWork);

            Dominio.Entidades.Embarcador.Cargas.Carga carga = filtrosPesquisa.CodigoCarga > 0 ? repCarga.BuscarPorCodigo(filtrosPesquisa.CodigoCarga) : null;

            parametros.Add(new Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro("CodigoCarga", carga?.CodigoCargaEmbarcador ?? null));
            parametros.Add(new Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro("DataCriacao", filtrosPesquisa.DataInicial, filtrosPesquisa.DataFinal, true)); ;

            return parametros;
        }

        protected override string ObterPropriedadeOrdenarOuAgrupar(string propriedadeOrdenarOuAgrupar)
        {
            if (propriedadeOrdenarOuAgrupar == "DataCriacaoFormatada")
                return "DataCriacao";

            return propriedadeOrdenarOuAgrupar;
        }

        #endregion 
    }
}