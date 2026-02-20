using Dominio.ObjetosDeValor.Embarcador.Enumeradores;
using Dominio.Enumeradores;
using System.Collections.Generic;
using System.Linq;
using Dominio.ObjetosDeValor.Embarcador.Logistica;

namespace Servicos.Embarcador.Relatorios.Logistica
{
    public class ConsolidacaoGas : RelatorioBase<FiltroPesquisaRelatorioConsolidacaoGas, Dominio.Relatorios.Embarcador.DataSource.Logistica.ConsolidacaoGas>
    {
        #region Atributos

        private readonly Repositorio.Embarcador.Logistica.SolicitacaoAbastecimentoGas _repositorioSolicitacaoGas;

        #endregion

        #region Construtores

        public ConsolidacaoGas(Repositorio.UnitOfWork unitOfWork, AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware tipoServicoMultisoftware, AdminMultisoftware.Dominio.Entidades.Pessoas.Cliente clienteMultisoftware) : base(unitOfWork, tipoServicoMultisoftware, clienteMultisoftware)
        {
            _repositorioSolicitacaoGas = new Repositorio.Embarcador.Logistica.SolicitacaoAbastecimentoGas(_unitOfWork);
        }

        #endregion

        #region Métodos Protegidos Sobrescritos
        // TODO: ToList metodo override
        protected override List<Dominio.Relatorios.Embarcador.DataSource.Logistica.ConsolidacaoGas> ConsultarRegistros(FiltroPesquisaRelatorioConsolidacaoGas filtrosPesquisa, List<Dominio.Relatorios.Embarcador.ObjetosDeValor.PropriedadeAgrupamento> propriedadesAgrupamento, Dominio.ObjetosDeValor.Embarcador.Consulta.ParametroConsulta parametrosConsulta)
        {
            return _repositorioSolicitacaoGas.ConsultaRelatorioSolicitacaoGas(filtrosPesquisa, propriedadesAgrupamento, parametrosConsulta).ToList();
        }

        protected override int ContarRegistros(FiltroPesquisaRelatorioConsolidacaoGas filtrosPesquisa, List<Dominio.Relatorios.Embarcador.ObjetosDeValor.PropriedadeAgrupamento> propriedadesAgrupamento)
        {
            return _repositorioSolicitacaoGas.ContarConsultaRelatorioSolicitacaoGas(filtrosPesquisa, propriedadesAgrupamento);
        }

        protected override string ObterCaminhoPaginaRelatorio()
        {
            return "Relatorios/Logistica/ConsolidacaoGas";
        }

        protected override List<Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro> ObterParametros(FiltroPesquisaRelatorioConsolidacaoGas filtrosPesquisa, Dominio.ObjetosDeValor.Embarcador.Consulta.ParametroConsulta parametrosConsulta)
        {
            List<Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro> parametros = new List<Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro>();

            List<Dominio.Entidades.Cliente> bases = new List<Dominio.Entidades.Cliente>();

            if (filtrosPesquisa.Bases != null && filtrosPesquisa.Bases.Count > 0)
                bases = new Repositorio.Cliente(_unitOfWork).BuscarPorCPFCNPJ(filtrosPesquisa.Bases);
            
            parametros.Add(new Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro("Data", filtrosPesquisa.DataInicial, filtrosPesquisa.DataFinal));
            parametros.Add(new Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro("Base", string.Join(", ", bases.Select(obj => $"{obj.CodigoIntegracao}-{obj.Descricao}"))));
            parametros.Add(new Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro("TipoFilial", filtrosPesquisa.TipoFilial.ObterDescricao()));
            parametros.Add(new Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro("VolumeRodoviario", filtrosPesquisa.VolumeRodoviario == SimNao.Nao ? "Não" : filtrosPesquisa.VolumeRodoviario == SimNao.Sim ? "Sim" : ""));
            parametros.Add(new Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro("DisponibilidadeTransferencia", filtrosPesquisa.DisponibilidadeTransferencia == SimNao.Nao ? "Não" : filtrosPesquisa.DisponibilidadeTransferencia == SimNao.Sim ? "Sim" : ""));
            
            return parametros;
        }

        protected override string ObterPropriedadeOrdenarOuAgrupar(string propriedadeOrdenarOuAgrupar)
        {
            if (propriedadeOrdenarOuAgrupar == "DataMedicaoFormatada")
                return "DataMedicao";

            return propriedadeOrdenarOuAgrupar;
        }

        #endregion

    }
}