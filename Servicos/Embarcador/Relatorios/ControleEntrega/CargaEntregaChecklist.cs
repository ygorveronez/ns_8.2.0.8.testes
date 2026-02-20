using System;
using System.Collections.Generic;
using System.Linq;

namespace Servicos.Embarcador.Relatorios.ControleEntrega
{
    public class CargaEntregaChecklist : RelatorioBase<Dominio.ObjetosDeValor.Embarcador.Carga.ControleEntrega.FiltroPesquisaRelatorioCargaEntregaChecklist, Dominio.Relatorios.Embarcador.DataSource.Cargas.ControleEntrega.CargaEntregaChecklist>
    {
        #region Atributos

        private readonly Repositorio.Embarcador.Cargas.ControleEntrega.CargaEntregaCheckList _repositorioCargaEntregaCheckList;
        private readonly int _limitePergustasRespostas = 60;

        #endregion

        #region Construtores

        public CargaEntregaChecklist(Repositorio.UnitOfWork unitOfWork, AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware tipoServicoMultisoftware, AdminMultisoftware.Dominio.Entidades.Pessoas.Cliente clienteMultisoftware) : base(unitOfWork, tipoServicoMultisoftware, clienteMultisoftware)
        {
            _repositorioCargaEntregaCheckList = new Repositorio.Embarcador.Cargas.ControleEntrega.CargaEntregaCheckList(_unitOfWork);
        }

        #endregion

        #region Métodos Protegidos Sobrescritos

        protected override List<Dominio.Relatorios.Embarcador.DataSource.Cargas.ControleEntrega.CargaEntregaChecklist> ConsultarRegistros(Dominio.ObjetosDeValor.Embarcador.Carga.ControleEntrega.FiltroPesquisaRelatorioCargaEntregaChecklist filtrosPesquisa, List<Dominio.Relatorios.Embarcador.ObjetosDeValor.PropriedadeAgrupamento> propriedadesAgrupamento, Dominio.ObjetosDeValor.Embarcador.Consulta.ParametroConsulta parametrosConsulta)
        {
            Repositorio.Embarcador.GestaoPatio.CheckListOpcoes repositorioChecklistOpcoes = new Repositorio.Embarcador.GestaoPatio.CheckListOpcoes(_unitOfWork);
            Repositorio.Embarcador.Cargas.ControleEntrega.CargaEntregaCheckListPergunta repositorioCargaEntregaCheckListPergunta = new Repositorio.Embarcador.Cargas.ControleEntrega.CargaEntregaCheckListPergunta(_unitOfWork);
            Repositorio.Embarcador.Cargas.ControleEntrega.CargaEntregaCheckListAlternativa repositorioCargaEntregaCheckListAlternativa = new Repositorio.Embarcador.Cargas.ControleEntrega.CargaEntregaCheckListAlternativa(_unitOfWork);
            Embarcador.Carga.ControleEntrega.CargaEntregaCheckList servicoCargaEntregaCheckList = new Embarcador.Carga.ControleEntrega.CargaEntregaCheckList(_unitOfWork);
            // TODO: ToList cast
            List<Dominio.Relatorios.Embarcador.DataSource.Cargas.ControleEntrega.CargaEntregaChecklist> listaChecklist = _repositorioCargaEntregaCheckList.ConsultarRelatorio(filtrosPesquisa, propriedadesAgrupamento, parametrosConsulta).ToList();
            List<Dominio.Entidades.Embarcador.GestaoPatio.CheckListOpcoes> listaPerguntas = repositorioChecklistOpcoes.BuscarPerguntasPorCheckListTipo(filtrosPesquisa.CodigoCheckListTipo);

            foreach (Dominio.Relatorios.Embarcador.DataSource.Cargas.ControleEntrega.CargaEntregaChecklist checklist in listaChecklist)
            {
                List<Dominio.Entidades.Embarcador.Cargas.ControleEntrega.CargaEntregaCheckListPergunta> listaRespostas = repositorioCargaEntregaCheckListPergunta.BuscarPerguntasOrdenadasPorCargaEntregaCheckList(checklist.Codigo);
                List<Dominio.Entidades.Embarcador.Cargas.ControleEntrega.CargaEntregaCheckListAlternativa> listaAlternativas = repositorioCargaEntregaCheckListAlternativa.BuscarPorCargaEntregaChecklist(checklist.Codigo);
                int totalPerguntas = Math.Min(listaPerguntas.Count, _limitePergustasRespostas);

                for (int i = 0; i < totalPerguntas; i++)
                {
                    Dominio.Entidades.Embarcador.GestaoPatio.CheckListOpcoes pergunta = listaPerguntas[i];
                    Dominio.Entidades.Embarcador.Cargas.ControleEntrega.CargaEntregaCheckListPergunta respostaPergunta = listaRespostas.Where(o => o.Descricao == pergunta.Descricao).FirstOrDefault();

                    if (respostaPergunta == null)
                        respostaPergunta = listaRespostas.Where(o => (o.CodigoIntegracao > 0) && (o.CodigoIntegracao == pergunta.CodigoIntegracao)).FirstOrDefault();

                    string resposta = (respostaPergunta != null) ? servicoCargaEntregaCheckList.ObterRespostaDescricaoPergunta(respostaPergunta) : string.Empty;

                    checklist.GetType().GetProperty($"ColunaDinamica{i}")?.SetValue(checklist, resposta);
                }
                
            }

            return listaChecklist;
        }

        protected override int ContarRegistros(Dominio.ObjetosDeValor.Embarcador.Carga.ControleEntrega.FiltroPesquisaRelatorioCargaEntregaChecklist filtrosPesquisa, List<Dominio.Relatorios.Embarcador.ObjetosDeValor.PropriedadeAgrupamento> propriedadesAgrupamento)
        {
            return _repositorioCargaEntregaCheckList.ContarConsultaRelatorio(filtrosPesquisa, propriedadesAgrupamento);
        }

        protected override string ObterCaminhoPaginaRelatorio()
        {
            return "Relatorios/Cargas/CargaEntregaChecklist";
        }

        protected override List<Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro> ObterParametros(Dominio.ObjetosDeValor.Embarcador.Carga.ControleEntrega.FiltroPesquisaRelatorioCargaEntregaChecklist filtrosPesquisa, Dominio.ObjetosDeValor.Embarcador.Consulta.ParametroConsulta parametrosConsulta)
        {
            List<Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro> parametros = new List<Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro>();

            return parametros;
        }

        protected override string ObterPropriedadeOrdenarOuAgrupar(string propriedadeOrdenarOuAgrupar)
        {
            return propriedadeOrdenarOuAgrupar;
        }

        #endregion 
    }
}
