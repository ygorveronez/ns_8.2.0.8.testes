//using Dominio.ObjetosDeValor.Embarcador.Enumeradores;
//using Dominio.Enumeradores;
//using System.Collections.Generic;
//using System.Linq;
//using Dominio.Relatorios.Embarcador.ObjetosDeValor;

//namespace Servicos.Embarcador.Relatorios.GerenciamentoIrregularidades
//{
//    public class ProcessamentoModuloControle : RelatorioBase<Dominio.ObjetosDeValor.Embarcador.GerenciamentoIrregularidades.FiltroPesquisaRelatorioProcessamentoModuloControle, Dominio.Relatorios.Embarcador.DataSource.GerenciamentoIrregularidades.ProcessamentoModuloControle>
//    {
//        #region Atributos

//        private readonly Repositorio.Embarcador.GerenciamentoIrregularidades.Irregularidade _repIrregularidade;

//        #endregion

//        #region Construtores

//        public ProcessamentoModuloControle(Repositorio.UnitOfWork unitOfWork, AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware tipoServicoMultisoftware, AdminMultisoftware.Dominio.Entidades.Pessoas.Cliente clienteMultisoftware) : base(unitOfWork, tipoServicoMultisoftware, clienteMultisoftware)
//        {
//            _repIrregularidade = new Repositorio.Embarcador.GerenciamentoIrregularidades.Irregularidade(_unitOfWork);
//        }

//        #endregion

//        #region Métodos Protegidos Sobrescritos

//        protected override List<Dominio.Relatorios.Embarcador.DataSource.GerenciamentoIrregularidades.ProcessamentoModuloControle> ConsultarRegistros(Dominio.ObjetosDeValor.Embarcador.GerenciamentoIrregularidades.FiltroPesquisaRelatorioProcessamentoModuloControle filtrosPesquisa, List<PropriedadeAgrupamento> propriedadesAgrupamento, Dominio.ObjetosDeValor.Embarcador.Consulta.ParametroConsulta parametrosConsulta)
//        {
//            return _repIrregularidade.RelatorioNFesCargas(filtrosPesquisa, propriedadesAgrupamento, parametrosConsulta);
//        }

//        protected override int ContarRegistros(Dominio.ObjetosDeValor.Embarcador.GerenciamentoIrregularidades.FiltroPesquisaRelatorioProcessamentoModuloControle filtrosPesquisa, List<PropriedadeAgrupamento> propriedadesAgrupamento)
//        {
//            return _repIrregularidade.ContarConsultaRelatorioNFesCargas(filtrosPesquisa, propriedadesAgrupamento);
//        }

//        protected override string ObterCaminhoPaginaRelatorio()
//        {
//            return "Relatorios/NFe/NFes";
//        }

//        //protected override List<Parametro> ObterParametros(Dominio.ObjetosDeValor.Embarcador.GerenciamentoIrregularidades.FiltroPesquisaRelatorioProcessamentoModuloControle filtrosPesquisa, Dominio.ObjetosDeValor.Embarcador.Consulta.ParametroConsulta parametrosConsulta)

            
//        //}

//        //protected override string ObterPropriedadeOrdenarOuAgrupar(string propriedadeOrdenarOuAgrupar)
//        //{

//        //}

//        #endregion
//    }
//}