using System.Collections.Generic;

namespace Servicos.Embarcador.Logistica
{
    public sealed class CargaJanelaCarregamentoTransportadorChecklist
    {
        #region Atributos

        private readonly Repositorio.UnitOfWork _unitOfWork;

        #endregion Atributos

        #region Construtores

        public CargaJanelaCarregamentoTransportadorChecklist(Repositorio.UnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        #endregion Construtores

        #region Métodos Privados

        private List<Dominio.ObjetosDeValor.Embarcador.Logistica.CargaJanelaCarregamentoTransportadorChecklist> ObterCargaChecklist(List<Dominio.Entidades.Embarcador.Cargas.CargaJanelaCarregamentoTransportadorChecklist> checklists)
        {
            List<Dominio.ObjetosDeValor.Embarcador.Logistica.CargaJanelaCarregamentoTransportadorChecklist> listaRetornoChecklist = new List<Dominio.ObjetosDeValor.Embarcador.Logistica.CargaJanelaCarregamentoTransportadorChecklist>();

            foreach (Dominio.Entidades.Embarcador.Cargas.CargaJanelaCarregamentoTransportadorChecklist checklist in checklists)
            {

                Dominio.ObjetosDeValor.Embarcador.Logistica.CargaJanelaCarregamentoTransportadorChecklist cargaJanelaCarregamentoTransportadorChecklist = new Dominio.ObjetosDeValor.Embarcador.Logistica.CargaJanelaCarregamentoTransportadorChecklist()
                {
                    CodigoChecklist = checklist.Codigo,
                    DataChecklist = checklist.DataChecklist.ToString("d"),
                    RegimeLimpeza = checklist.RegimeLimpeza,
                    OrdemCargaChecklist = checklist.OrdemCargaChecklist,
                    GrupoProduto = new Dominio.ObjetosDeValor.Embarcador.Logistica.GrupoProdutoChecklist()
                    {
                        Codigo = checklist.GrupoProduto.Codigo,
                        Descricao = checklist.GrupoProduto.Descricao
                    },
                    Placa = checklist.Veiculo.Placa,
                    CodigoVeiculo = checklist.Veiculo.Codigo
                };

                listaRetornoChecklist.Add(cargaJanelaCarregamentoTransportadorChecklist);

            }

            return listaRetornoChecklist;
        }

        #endregion Métodos Privados

        #region Métodos Públicos

        public List<Dominio.ObjetosDeValor.Embarcador.Logistica.CargaJanelaCarregamentoTransportadorChecklist> ObterChecklist(int codigoJanela, int codigoVeiculo)
        {
            Repositorio.Embarcador.Cargas.CargaJanelaCarregamentoTransportadorChecklist repChecklist = new Repositorio.Embarcador.Cargas.CargaJanelaCarregamentoTransportadorChecklist(_unitOfWork);

            List<Dominio.Entidades.Embarcador.Cargas.CargaJanelaCarregamentoTransportadorChecklist> checklist = repChecklist.BuscarPorCargaJanelaCarregamentoEVeiculo(codigoJanela, codigoVeiculo);

            if (checklist.Count == 0)
                return new List<Dominio.ObjetosDeValor.Embarcador.Logistica.CargaJanelaCarregamentoTransportadorChecklist>();

            return ObterCargaChecklist(checklist);
        }

        #endregion Métodos Públicos
    }
}