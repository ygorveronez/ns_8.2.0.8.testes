using System.Collections.Generic;

namespace Servicos.Embarcador.CTe
{
    public class ModalFerroviario : ServicoBase
    {        
        public ModalFerroviario(Repositorio.UnitOfWork unitOfWork) : base(unitOfWork) { }
        public Dominio.ObjetosDeValor.Embarcador.CTe.ModalFerroviario ConverterDynamicModalFerroviario(dynamic dynCTe, Repositorio.UnitOfWork unitOfWork)
        {
            if (dynCTe.ModalFerroviario != null)
            {
                Dominio.ObjetosDeValor.Embarcador.CTe.ModalFerroviario modalFerroviario = new Dominio.ObjetosDeValor.Embarcador.CTe.ModalFerroviario();
                modalFerroviario.NumeroFluxoFerroviario = (string)dynCTe.ModalFerroviario.NumeroFluxoFerroviario;
                modalFerroviario.ChaveCTeFerroviaOrigem = (string)dynCTe.ModalFerroviario.ChaveCTeFerroviaOrigem;
                modalFerroviario.ValorFrete = Utilidades.Decimal.Converter((string)dynCTe.ModalFerroviario.ValorFrete);

                modalFerroviario.TipoTrafego = (Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoTrafego)dynCTe.ModalFerroviario.TipoTrafego;
                if (dynCTe.ModalFerroviario.ResponsavelFaturamento != null && !string.IsNullOrWhiteSpace((string)dynCTe.ModalFerroviario.ResponsavelFaturamento) && (int)dynCTe.ModalFerroviario.ResponsavelFaturamento > 0)
                    modalFerroviario.ResponsavelFaturamento = (Dominio.ObjetosDeValor.Embarcador.Enumeradores.FerroviaResponsavel)dynCTe.ModalFerroviario.ResponsavelFaturamento;
                else
                    modalFerroviario.ResponsavelFaturamento = null;
                if (dynCTe.ModalFerroviario.FerroviaEmitente != null && !string.IsNullOrWhiteSpace((string)dynCTe.ModalFerroviario.FerroviaEmitente) && (int)dynCTe.ModalFerroviario.FerroviaEmitente > 0)
                    modalFerroviario.FerroviaEmitente = (Dominio.ObjetosDeValor.Embarcador.Enumeradores.FerroviaResponsavel)dynCTe.ModalFerroviario.FerroviaEmitente;
                else
                    modalFerroviario.FerroviaEmitente = null;

                if (dynCTe.Ferrovias != null)
                {
                    modalFerroviario.Ferrovias = new List<Dominio.ObjetosDeValor.Embarcador.CTe.ModalFerroviarioFerrovia>();
                    foreach (var dynFerrovia in dynCTe.Ferrovias)
                    {
                        Dominio.ObjetosDeValor.Embarcador.CTe.ModalFerroviarioFerrovia ferrovia = new Dominio.ObjetosDeValor.Embarcador.CTe.ModalFerroviarioFerrovia();
                        double.TryParse((string)dynFerrovia.Ferrovia, out double codigoFerrovia);
                        ferrovia.Ferrovia = codigoFerrovia;
                        modalFerroviario.Ferrovias.Add(ferrovia);
                    }
                }

                return modalFerroviario;
            }
            else
                return null;
        }

        public void SalvarModalFerroviario(ref Dominio.Entidades.ConhecimentoDeTransporteEletronico cte, Dominio.ObjetosDeValor.Embarcador.CTe.ModalFerroviario modalFerroviario, Repositorio.UnitOfWork unitOfWork)
        {
            if (modalFerroviario != null)
            {
                cte.NumeroFluxoFerroviario = modalFerroviario.NumeroFluxoFerroviario;
                cte.ChaveCTeFerroviaOrigem = modalFerroviario.ChaveCTeFerroviaOrigem;
                cte.ValorFreteTrafego = modalFerroviario.ValorFrete;
                cte.TipoTrafego = modalFerroviario.TipoTrafego;
                cte.FerroviaResponsavelFaturamento = modalFerroviario.ResponsavelFaturamento;
                cte.FerroviaEmitente = modalFerroviario.FerroviaEmitente;

                SalvarFerroviasCTe(ref cte, modalFerroviario.Ferrovias, unitOfWork);
            }
        }

        private void SalvarFerroviasCTe(ref Dominio.Entidades.ConhecimentoDeTransporteEletronico cte, List<Dominio.ObjetosDeValor.Embarcador.CTe.ModalFerroviarioFerrovia> ferrovias, Repositorio.UnitOfWork unitOfWork)
        {
            Repositorio.Embarcador.CTe.CTeFerrovia repFerroviaCTe = new Repositorio.Embarcador.CTe.CTeFerrovia(unitOfWork);
            Repositorio.Cliente repPessoa = new Repositorio.Cliente(unitOfWork);

            if (cte.Codigo > 0)
                repFerroviaCTe.DeletarPorCTe(cte.Codigo);

            foreach (var ferrovia in ferrovias)
            {
                Dominio.Entidades.Embarcador.CTe.CTeFerrovia ferroviaCTe = new Dominio.Entidades.Embarcador.CTe.CTeFerrovia();

                ferroviaCTe.ConhecimentoDeTransporteEletronico = cte;
                ferroviaCTe.Ferrovia = ferrovia.Ferrovia > 0 ? repPessoa.BuscarPorCPFCNPJ(ferrovia.Ferrovia) : null;

                if (ferroviaCTe.Ferrovia != null)
                    repFerroviaCTe.Inserir(ferroviaCTe);
            }
        }
    }
}
