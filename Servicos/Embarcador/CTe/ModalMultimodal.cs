using System.Collections.Generic;

namespace Servicos.Embarcador.CTe
{
    public class ModalMultimodal : ServicoBase
    {        
        public ModalMultimodal(Repositorio.UnitOfWork unitOfWork) : base(unitOfWork) { }
        public Dominio.ObjetosDeValor.Embarcador.CTe.ModalMultimodal ConverterDynamicModalMultimodal(dynamic dynCTe, Repositorio.UnitOfWork unitOfWork)
        {
            if (dynCTe.ModalMultimodal != null)
            {
                Dominio.ObjetosDeValor.Embarcador.CTe.ModalMultimodal modalMultimodal = new Dominio.ObjetosDeValor.Embarcador.CTe.ModalMultimodal();
                modalMultimodal.NumeroCOTM = (string)dynCTe.ModalMultimodal.NumeroCOTM;

                if (dynCTe.ModalMultimodal.IndicadorNegociavel != null && !string.IsNullOrWhiteSpace((string)dynCTe.ModalMultimodal.IndicadorNegociavel) && (int)dynCTe.ModalMultimodal.IndicadorNegociavel >= 0)
                    modalMultimodal.IndicadorNegociavel = (Dominio.Enumeradores.OpcaoSimNao)dynCTe.ModalMultimodal.IndicadorNegociavel;
                else
                    modalMultimodal.IndicadorNegociavel = null;

                if (dynCTe.Containers != null)
                {
                    modalMultimodal.Containers = new List<Dominio.ObjetosDeValor.Embarcador.CTe.ModalMultimodalContainer>();
                    foreach (var dynContainer in dynCTe.Containers)
                    {
                        Dominio.ObjetosDeValor.Embarcador.CTe.ModalMultimodalContainer container = new Dominio.ObjetosDeValor.Embarcador.CTe.ModalMultimodalContainer();
                        int.TryParse((string)dynContainer.Container, out int codigoContainer);
                        container.Container = codigoContainer;
                        container.Lacre1 = (string)dynContainer.Lacre1;
                        container.Lacre2 = (string)dynContainer.Lacre2;
                        container.Lacre3 = (string)dynContainer.Lacre3;

                        string codigo = (string)dynContainer.Codigo;
                        if (dynCTe.ContainerDocumentos != null)
                        {
                            container.Documentos = new List<Dominio.ObjetosDeValor.Embarcador.CTe.ModalMultimodalContainerDocumento>();
                            foreach (var dynDocumento in dynCTe.ContainerDocumentos)
                            {
                                if (codigo == (string)dynDocumento.CodigoContainer)
                                {
                                    Dominio.ObjetosDeValor.Embarcador.CTe.ModalMultimodalContainerDocumento documento = new Dominio.ObjetosDeValor.Embarcador.CTe.ModalMultimodalContainerDocumento();

                                    documento.TipoDocumento = (Dominio.Enumeradores.TipoDocumentoCTe)dynDocumento.CodigoTipoDocumento;
                                    documento.Serie = (string)dynDocumento.Serie;
                                    documento.Numero = (string)dynDocumento.Numero;
                                    documento.Chave = (string)dynDocumento.Chave;
                                    documento.UnidadeMedidaRateada = Utilidades.Decimal.Converter((string)dynDocumento.UnidadeMedidaRateada);

                                    container.Documentos.Add(documento);
                                }
                            }
                        }

                        modalMultimodal.Containers.Add(container);
                    }
                }


                return modalMultimodal;
            }
            else
                return null;
        }

        public void SalvarModalMultimodal(ref Dominio.Entidades.ConhecimentoDeTransporteEletronico cte, Dominio.ObjetosDeValor.Embarcador.CTe.ModalMultimodal modalMultimodal, Repositorio.UnitOfWork unitOfWork)
        {
            if (modalMultimodal != null)
            {
                cte.NumeroCOTM = modalMultimodal.NumeroCOTM;
                cte.IndicadorNegociavel = modalMultimodal.IndicadorNegociavel;

                SalvarContainersCTe(ref cte, modalMultimodal.Containers, unitOfWork);
            }
        }

        private void SalvarContainersCTe(ref Dominio.Entidades.ConhecimentoDeTransporteEletronico cte, List<Dominio.ObjetosDeValor.Embarcador.CTe.ModalMultimodalContainer> containers, Repositorio.UnitOfWork unitOfWork)
        {
            Repositorio.ContainerCTE repContainerCTe = new Repositorio.ContainerCTE(unitOfWork);
            Repositorio.Embarcador.CTe.CTeContainerDocumento repContainerDocumentoCTe = new Repositorio.Embarcador.CTe.CTeContainerDocumento(unitOfWork);
            Repositorio.DocumentosCTE repDocumentosCTE = new Repositorio.DocumentosCTE(unitOfWork);
            Repositorio.Embarcador.Pedidos.Container repContainer = new Repositorio.Embarcador.Pedidos.Container(unitOfWork);
            Repositorio.Embarcador.Pedidos.XMLNotaFiscal repXMLNotaFiscalEletronica = new Repositorio.Embarcador.Pedidos.XMLNotaFiscal(unitOfWork);

            if (cte.Codigo > 0)
            {
                repContainerDocumentoCTe.DeletarPorCTe(cte.Codigo);
                repContainerCTe.DeletarPorCTe(cte.Codigo);
            }

            foreach (var container in containers)
            {
                Dominio.Entidades.ContainerCTE containerCTe = new Dominio.Entidades.ContainerCTE();

                containerCTe.Container = container.Container > 0 ? repContainer.BuscarPorCodigo(container.Container) : null;
                containerCTe.CTE = cte;
                containerCTe.Lacre1 = !string.IsNullOrWhiteSpace(container.Lacre1) && container.Lacre1.Length > 20 ? container.Lacre1.Substring(0, 19) : container.Lacre1;
                containerCTe.Lacre2 = !string.IsNullOrWhiteSpace(container.Lacre2) && container.Lacre2.Length > 20 ? container.Lacre2.Substring(0, 19) : container.Lacre2;
                containerCTe.Lacre3 = !string.IsNullOrWhiteSpace(container.Lacre3) && container.Lacre3.Length > 20 ? container.Lacre3.Substring(0, 19) : container.Lacre3;

                if (!string.IsNullOrWhiteSpace(containerCTe.Lacre1))
                    containerCTe.Lacre1 = containerCTe.Lacre1.Replace(";", "").Replace("-", "").Replace(" ", "");
                if (!string.IsNullOrWhiteSpace(containerCTe.Lacre2))
                    containerCTe.Lacre2 = containerCTe.Lacre2.Replace(";", "").Replace("-", "").Replace(" ", "");
                if (!string.IsNullOrWhiteSpace(containerCTe.Lacre3))
                    containerCTe.Lacre3 = containerCTe.Lacre3.Replace(";", "").Replace("-", "").Replace(" ", "");

                repContainerCTe.Inserir(containerCTe);

                foreach (var documento in container.Documentos)
                {
                    Dominio.Entidades.Embarcador.CTe.CTeContainerDocumento containerDocumentoCTe = new Dominio.Entidades.Embarcador.CTe.CTeContainerDocumento();

                    containerDocumentoCTe.TipoDocumento = documento.TipoDocumento;
                    containerDocumentoCTe.Serie = documento.Serie;
                    containerDocumentoCTe.Numero = documento.Numero;
                    containerDocumentoCTe.Chave = documento.Chave;
                    containerDocumentoCTe.UnidadeMedidaRateada = documento.UnidadeMedidaRateada;
                    containerDocumentoCTe.ContainerCTE = containerCTe;
                    containerDocumentoCTe.DocumentosCTE = repDocumentosCTE.BuscarPorCTeENFe(cte.Codigo, documento.Chave);
                    if (containerDocumentoCTe.DocumentosCTE == null)
                        containerDocumentoCTe.DocumentosCTE = repDocumentosCTE.BuscarPorCTeENumero(cte.Codigo, documento.Numero);

                    if (!string.IsNullOrWhiteSpace(containerDocumentoCTe.Chave))
                        containerDocumentoCTe.XMLNotaFiscal = repXMLNotaFiscalEletronica.BuscarPorChave(containerDocumentoCTe.Chave);

                    repContainerDocumentoCTe.Inserir(containerDocumentoCTe);
                }
            }
        }
    }
}
