using System;
using System.Collections.Generic;

namespace Servicos.Embarcador.CTe
{
    public class ModalAereo : ServicoBase
    {        
        public ModalAereo(Repositorio.UnitOfWork unitOfWork) : base(unitOfWork) { }
        public Dominio.ObjetosDeValor.Embarcador.CTe.ModalAereo ConverterDynamicModalAereo(dynamic dynCTe, Repositorio.UnitOfWork unitOfWork)
        {
            if (dynCTe.ModalAereo != null)
            {
                Dominio.ObjetosDeValor.Embarcador.CTe.ModalAereo modalAereo = new Dominio.ObjetosDeValor.Embarcador.CTe.ModalAereo();

                DateTime dataPrevisaoEntrega = new DateTime();
                DateTime.TryParseExact((string)dynCTe.ModalAereo.DataPrevisaoEntrega, "dd/MM/yyyy", null, System.Globalization.DateTimeStyles.None, out dataPrevisaoEntrega);
                modalAereo.DataPrevistaEntrega = dataPrevisaoEntrega;

                long? numeroMinuta = null;
                long? numeroOperacionalConhecimentoAereo = null;
                if (long.TryParse((string)dynCTe.ModalAereo.NumeroMinuta, out long numeroMinutaAux))
                    numeroMinuta = numeroMinutaAux;
                if (long.TryParse((string)dynCTe.ModalAereo.NumeroOCA, out long numeroOperacionalConhecimentoAereoAux))
                    numeroOperacionalConhecimentoAereo = numeroOperacionalConhecimentoAereoAux;

                modalAereo.NumeroMinuta = numeroMinuta;
                modalAereo.NumeroOperacionalConhecimentoAereo = numeroOperacionalConhecimentoAereo;

                modalAereo.Dimensao = (string)dynCTe.ModalAereo.Dimensao;
                modalAereo.ClasseTarifa = (string)dynCTe.ModalAereo.ClasseTarifa;
                modalAereo.CodigoTarifa = (string)dynCTe.ModalAereo.CodigoTarifa;
                modalAereo.ValorTarifa = Utilidades.Decimal.Converter((string)dynCTe.ModalAereo.ValorTarifa);

                if (dynCTe.Manuseios != null)
                {
                    modalAereo.Manuseios = new List<Dominio.ObjetosDeValor.Embarcador.CTe.ModalAereoManuseio>();
                    foreach (var dynManuseio in dynCTe.Manuseios)
                    {
                        Dominio.ObjetosDeValor.Embarcador.CTe.ModalAereoManuseio manuseio = new Dominio.ObjetosDeValor.Embarcador.CTe.ModalAereoManuseio();
                        manuseio.InformacaoManuseio = (Dominio.ObjetosDeValor.Embarcador.Enumeradores.InformacaoManuseio)dynManuseio.CodigoInformacaoManuseio;
                        modalAereo.Manuseios.Add(manuseio);
                    }
                }

                return modalAereo;
            }
            else
                return null;
        }

        public void SalvarModalAereo(ref Dominio.Entidades.ConhecimentoDeTransporteEletronico cte, Dominio.ObjetosDeValor.Embarcador.CTe.ModalAereo modalAereo, Repositorio.UnitOfWork unitOfWork)
        {
            if (modalAereo != null)
            {
                cte.NumeroMinuta = modalAereo.NumeroMinuta?.ToString();
                cte.NumeroOCA = modalAereo.NumeroOperacionalConhecimentoAereo?.ToString();
                cte.DataPrevistaEntrega = modalAereo.DataPrevistaEntrega;
                cte.Dimensao = modalAereo.Dimensao;
                cte.ClasseTarifa = modalAereo.ClasseTarifa;
                cte.CodigoTarifa = modalAereo.CodigoTarifa;
                cte.ValorTarifa = modalAereo.ValorTarifa;

                SalvarManuseiosCTe(ref cte, modalAereo.Manuseios, unitOfWork);
            }
        }

        private void SalvarManuseiosCTe(ref Dominio.Entidades.ConhecimentoDeTransporteEletronico cte, List<Dominio.ObjetosDeValor.Embarcador.CTe.ModalAereoManuseio> manuseios, Repositorio.UnitOfWork unitOfWork)
        {
            Repositorio.Embarcador.CTe.CTeInformacaoManuseio repInformacaoManuseioCTe = new Repositorio.Embarcador.CTe.CTeInformacaoManuseio(unitOfWork);

            if (cte.Codigo > 0)
                repInformacaoManuseioCTe.DeletarPorCTe(cte.Codigo);

            foreach (var manuseio in manuseios)
            {
                Dominio.Entidades.Embarcador.CTe.CTeInformacaoManuseio informacaoManuseioCTe = new Dominio.Entidades.Embarcador.CTe.CTeInformacaoManuseio();

                informacaoManuseioCTe.ConhecimentoDeTransporteEletronico = cte;
                informacaoManuseioCTe.InformacaoManuseio = manuseio.InformacaoManuseio;

                repInformacaoManuseioCTe.Inserir(informacaoManuseioCTe);
            }
        }
    }
}
