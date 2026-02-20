using Repositorio;
using System.Collections.Generic;

namespace Servicos.Embarcador.CTe
{
    public class ModalAquaviario : ServicoBase
    {        
        public ModalAquaviario(UnitOfWork unitOfWork) : base(unitOfWork) { }

        public Dominio.ObjetosDeValor.Embarcador.CTe.ModalAquaviario ConverterDynamicModalAquaviario(dynamic dynCTe, Repositorio.UnitOfWork unitOfWork)
        {
            if (dynCTe.ModalAquaviario != null)
            {
                Dominio.ObjetosDeValor.Embarcador.CTe.ModalAquaviario modalAquaviario = new Dominio.ObjetosDeValor.Embarcador.CTe.ModalAquaviario();

                modalAquaviario.ValorPrestacaoAfrmm = Utilidades.Decimal.Converter((string)dynCTe.ModalAquaviario.ValorPrestacaoAfrmm);
                modalAquaviario.ValorAdicionalAfrmm = Utilidades.Decimal.Converter((string)dynCTe.ModalAquaviario.ValorAdicionalAfrmm);
                modalAquaviario.NumeroViagem = (string)dynCTe.ModalAquaviario.NumeroViagem;
                modalAquaviario.Direcao = (string)dynCTe.ModalAquaviario.Direcao;

                int.TryParse((string)dynCTe.ModalAquaviario.Navio, out int codigoNavio);
                modalAquaviario.Navio = codigoNavio;

                if (dynCTe.Balsas != null)
                {
                    modalAquaviario.Balsas = new List<Dominio.ObjetosDeValor.Embarcador.CTe.ModalAquaviarioBalsa>();
                    foreach (var dynBalsa in dynCTe.Balsas)
                    {
                        Dominio.ObjetosDeValor.Embarcador.CTe.ModalAquaviarioBalsa balsa = new Dominio.ObjetosDeValor.Embarcador.CTe.ModalAquaviarioBalsa();
                        balsa.Balsa = (string)dynBalsa.Balsa;
                        modalAquaviario.Balsas.Add(balsa);
                    }
                }
                return modalAquaviario;
            }
            else
                return null;
        }

        public List<Dominio.ObjetosDeValor.Embarcador.CTe.ModalAquaviarioContainer> ConverterDynamicParaConteiners(dynamic dynCTe, Repositorio.UnitOfWork unitOfWork)
        {
            List<Dominio.ObjetosDeValor.Embarcador.CTe.ModalAquaviarioContainer> containers = new List<Dominio.ObjetosDeValor.Embarcador.CTe.ModalAquaviarioContainer>();

            if (dynCTe.Containers != null)
            {
                foreach (var dynContainer in dynCTe.Containers)
                {
                    Dominio.ObjetosDeValor.Embarcador.CTe.ModalAquaviarioContainer container = new Dominio.ObjetosDeValor.Embarcador.CTe.ModalAquaviarioContainer();
                    int.TryParse((string)dynContainer.Container, out int codigoContainer);
                    container.Container = codigoContainer;
                    container.Lacre1 = (string)dynContainer.Lacre1;
                    container.Lacre2 = (string)dynContainer.Lacre2;
                    container.Lacre3 = (string)dynContainer.Lacre3;

                    string codigo = (string)dynContainer.Codigo;
                    if (dynCTe.ContainerDocumentos != null)
                    {
                        container.Documentos = new List<Dominio.ObjetosDeValor.Embarcador.CTe.ModalAquaviarioContainerDocumento>();
                        foreach (var dynDocumento in dynCTe.ContainerDocumentos)
                        {
                            if (codigo == (string)dynDocumento.CodigoContainer || string.IsNullOrWhiteSpace((string)dynDocumento.CodigoContainer))
                            {
                                Dominio.ObjetosDeValor.Embarcador.CTe.ModalAquaviarioContainerDocumento documento = new Dominio.ObjetosDeValor.Embarcador.CTe.ModalAquaviarioContainerDocumento();

                                documento.TipoDocumento = (Dominio.Enumeradores.TipoDocumentoCTe)dynDocumento.CodigoTipoDocumento;
                                documento.Serie = (string)dynDocumento.Serie;
                                documento.Numero = (string)dynDocumento.Numero;
                                documento.Chave = (string)dynDocumento.Chave;
                                documento.UnidadeMedidaRateada = Utilidades.Decimal.Converter((string)dynDocumento.UnidadeMedidaRateada);

                                container.Documentos.Add(documento);
                            }
                        }
                    }

                    containers.Add(container);
                }
            }

            return containers;
        }

        public List<Dominio.ObjetosDeValor.Embarcador.CTe.ModalAquaviarioContainer> ConverterContainersCTeParaConteiners(List<Dominio.Entidades.ContainerCTE> containersCTe)
        {
            List<Dominio.ObjetosDeValor.Embarcador.CTe.ModalAquaviarioContainer> containers = new List<Dominio.ObjetosDeValor.Embarcador.CTe.ModalAquaviarioContainer>();

            if (containersCTe == null || containersCTe.Count == 0)
                return containers;

            foreach (Dominio.Entidades.ContainerCTE containerCTE in containersCTe)
            {
                Dominio.ObjetosDeValor.Embarcador.CTe.ModalAquaviarioContainer container = new Dominio.ObjetosDeValor.Embarcador.CTe.ModalAquaviarioContainer()
                {
                    NumeroContainer = containerCTE.Container?.Numero ?? string.Empty,
                    Lacre1 = containerCTE.Lacre1,
                    Lacre2 = containerCTE.Lacre2,
                    Lacre3 = containerCTE.Lacre3
                };

                if (containerCTE.Documentos != null)
                {
                    container.Documentos = new List<Dominio.ObjetosDeValor.Embarcador.CTe.ModalAquaviarioContainerDocumento>();
                    foreach (Dominio.Entidades.Embarcador.CTe.CTeContainerDocumento containerDocumento in containerCTE.Documentos)
                    {
                        Dominio.ObjetosDeValor.Embarcador.CTe.ModalAquaviarioContainerDocumento documento = new Dominio.ObjetosDeValor.Embarcador.CTe.ModalAquaviarioContainerDocumento()
                        {
                            TipoDocumento = containerDocumento.TipoDocumento,
                            Serie = containerDocumento.Serie,
                            Numero = containerDocumento.Numero,
                            Chave = containerDocumento.Chave,
                            UnidadeMedidaRateada = containerDocumento.UnidadeMedidaRateada
                        };

                        container.Documentos.Add(documento);
                    }
                }

                containers.Add(container);
            }

            return containers;
        }

        public void SalvarModalAquaviario(ref Dominio.Entidades.ConhecimentoDeTransporteEletronico cte, Dominio.ObjetosDeValor.Embarcador.CTe.ModalAquaviario modalAquaviario, Repositorio.UnitOfWork unitOfWork)
        {
            if (modalAquaviario != null)
            {
                Repositorio.Embarcador.Pedidos.Navio repNavio = new Repositorio.Embarcador.Pedidos.Navio(unitOfWork);

                cte.ValorPrestacaoAFRMM = modalAquaviario.ValorPrestacaoAfrmm;
                cte.ValorAdicionalAFRMM = modalAquaviario.ValorAdicionalAfrmm;
                cte.NumeroViagem = modalAquaviario.NumeroViagem;
                cte.Direcao = modalAquaviario.Direcao;
                cte.Navio = modalAquaviario.Navio > 0 ? repNavio.BuscarPorCodigo(modalAquaviario.Navio) : null;
                if (cte.Navio == null && cte.Viagem != null && cte.Viagem.Navio != null)
                    cte.Navio = cte.Viagem.Navio;

                SalvarBalsasCTe(ref cte, modalAquaviario.Balsas, unitOfWork);
            }
        }

        private void SalvarBalsasCTe(ref Dominio.Entidades.ConhecimentoDeTransporteEletronico cte, List<Dominio.ObjetosDeValor.Embarcador.CTe.ModalAquaviarioBalsa> balsas, Repositorio.UnitOfWork unitOfWork)
        {
            Repositorio.Embarcador.CTe.CTeBalsa repBalsaCTe = new Repositorio.Embarcador.CTe.CTeBalsa(unitOfWork);

            if (cte.Codigo > 0)
                repBalsaCTe.DeletarPorCTe(cte.Codigo);

            foreach (var balsa in balsas)
            {
                Dominio.Entidades.Embarcador.CTe.CTeBalsa balsaCTe = new Dominio.Entidades.Embarcador.CTe.CTeBalsa();

                balsaCTe.ConhecimentoDeTransporteEletronico = cte;
                balsaCTe.Descricao = balsa.Balsa;

                repBalsaCTe.Inserir(balsaCTe);
            }
        }

        public void SalvarContainersCTe(ref Dominio.Entidades.ConhecimentoDeTransporteEletronico cte, List<Dominio.ObjetosDeValor.Embarcador.CTe.ModalAquaviarioContainer> containers, Repositorio.UnitOfWork unitOfWork)
        {
            Repositorio.ContainerCTE repContainerCTe = new Repositorio.ContainerCTE(unitOfWork);
            Repositorio.DocumentosCTE repDocumentosCTE = new Repositorio.DocumentosCTE(unitOfWork);
            Repositorio.Embarcador.CTe.CTeContainerDocumento repContainerDocumentoCTe = new Repositorio.Embarcador.CTe.CTeContainerDocumento(unitOfWork);
            Repositorio.Embarcador.Pedidos.Container repContainer = new Repositorio.Embarcador.Pedidos.Container(unitOfWork);
            Repositorio.Embarcador.Pedidos.XMLNotaFiscal repXMLNotaFiscalEletronica = new Repositorio.Embarcador.Pedidos.XMLNotaFiscal(unitOfWork);

            if (cte.Codigo > 0)
            {
                repContainerDocumentoCTe.DeletarPorCTe(cte.Codigo);
                repContainerCTe.DeletarPorCTe(cte.Codigo);
            }

            foreach (Dominio.ObjetosDeValor.Embarcador.CTe.ModalAquaviarioContainer container in containers)
            {
                Dominio.Entidades.ContainerCTE containerCTe = new Dominio.Entidades.ContainerCTE();

                containerCTe.Container = !string.IsNullOrWhiteSpace(container.NumeroContainer) ? repContainer.BuscarPorNumero(container.NumeroContainer) : container.Container > 0 ? repContainer.BuscarPorCodigo(container.Container) : null;
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

                foreach (Dominio.ObjetosDeValor.Embarcador.CTe.ModalAquaviarioContainerDocumento documento in container.Documentos)
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
