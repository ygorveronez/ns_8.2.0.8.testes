using System;

namespace Servicos.Embarcador.CTe
{
    public class ModalDutoviario : ServicoBase
    {
        public ModalDutoviario(Repositorio.UnitOfWork unitOfWork) : base(unitOfWork) { }
        public Dominio.ObjetosDeValor.Embarcador.CTe.ModalDutoviario ConverterDynamicModalDutoviario(dynamic dynCTe, Repositorio.UnitOfWork unitOfWork)
        {
            if (dynCTe.ModalDutoviario != null)
            {
                Dominio.ObjetosDeValor.Embarcador.CTe.ModalDutoviario modalDutoviario = new Dominio.ObjetosDeValor.Embarcador.CTe.ModalDutoviario();

                modalDutoviario.ValorTarifa = Utilidades.Decimal.Converter((string)dynCTe.ModalDutoviario.ValorTarifa);

                DateTime dataInicio = new DateTime();
                DateTime dataFim = new DateTime();
                DateTime.TryParseExact((string)dynCTe.ModalDutoviario.DataInicioPrestacaoServico, "dd/MM/yyyy", null, System.Globalization.DateTimeStyles.None, out dataInicio);
                DateTime.TryParseExact((string)dynCTe.ModalDutoviario.DataFimPrestacaoServico, "dd/MM/yyyy", null, System.Globalization.DateTimeStyles.None, out dataFim);
                modalDutoviario.DataInicioPrestacaoServico = dataInicio;
                modalDutoviario.DataFimPrestacaoServico = dataFim;

                return modalDutoviario;
            }
            else
                return null;
        }

        public void SalvarModalDutoviario(ref Dominio.Entidades.ConhecimentoDeTransporteEletronico cte, Dominio.ObjetosDeValor.Embarcador.CTe.ModalDutoviario modalDutoviario, Repositorio.UnitOfWork unitOfWork)
        {
            if (modalDutoviario != null)
            {
                cte.ValorTarifa = modalDutoviario.ValorTarifa;
                cte.DataInicioPrestacaoServico = modalDutoviario.DataInicioPrestacaoServico;
                cte.DataFimPrestacaoServico = modalDutoviario.DataFimPrestacaoServico;
            }
        }
    }
}
