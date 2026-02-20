using Repositorio;
using System.Collections.Generic;

namespace Servicos.Embarcador.CTe
{
    public class DocumentoAnterior : ServicoBase
    {        
        public DocumentoAnterior(UnitOfWork unitOfWork) : base(unitOfWork) { }

        public List<Dominio.ObjetosDeValor.Embarcador.CTe.DocumentoAnterior> ConverterDocumentosAnterioresCTeParaDocumentosAnteriores(List<Dominio.Entidades.DocumentoDeTransporteAnteriorCTe> documentosTransporteAnterior, Repositorio.UnitOfWork unitOfWork)
        {
            Repositorio.Cliente repCliente = new Repositorio.Cliente(unitOfWork);
            Servicos.WebService.Pessoas.Pessoa serPessoa = new WebService.Pessoas.Pessoa(unitOfWork);

            List<Dominio.ObjetosDeValor.Embarcador.CTe.DocumentoAnterior> documentosAnteriores = new List<Dominio.ObjetosDeValor.Embarcador.CTe.DocumentoAnterior>();

            foreach (Dominio.Entidades.DocumentoDeTransporteAnteriorCTe documenotAnteriorCTe in documentosTransporteAnterior)
            {
                if (!string.IsNullOrWhiteSpace(documenotAnteriorCTe.Chave))
                {
                    Dominio.ObjetosDeValor.Embarcador.CTe.DocumentoAnterior documentoAnterior = new Dominio.ObjetosDeValor.Embarcador.CTe.DocumentoAnterior();
                    documentoAnterior.Emitente = serPessoa.ConverterObjetoPessoa(documenotAnteriorCTe.Emissor);
                    documentoAnterior.ChaveAcesso = documenotAnteriorCTe.Chave;
                    documentosAnteriores.Add(documentoAnterior);
                }
            }
            return documentosAnteriores;
        }

        public List<Dominio.ObjetosDeValor.Embarcador.CTe.DocumentoAnterior> ConverterDynamicParaDocumentosAnteriores(dynamic dynDocumentosAnteriores, Repositorio.UnitOfWork unitOfWork)
        {
            Repositorio.Cliente repCliente = new Repositorio.Cliente(unitOfWork);
            Servicos.WebService.Pessoas.Pessoa serPessoa = new WebService.Pessoas.Pessoa(unitOfWork);

            List<Dominio.ObjetosDeValor.Embarcador.CTe.DocumentoAnterior> documentosAnteriores = new List<Dominio.ObjetosDeValor.Embarcador.CTe.DocumentoAnterior>();

            foreach (var dynDocumentoAnterior in dynDocumentosAnteriores)
            {
                Dominio.ObjetosDeValor.Embarcador.CTe.DocumentoAnterior documentoAnterior = new Dominio.ObjetosDeValor.Embarcador.CTe.DocumentoAnterior();
                documentoAnterior.Emitente = serPessoa.ConverterObjetoPessoa(repCliente.BuscarPorCPFCNPJ((double)dynDocumentoAnterior.CodigoEmitente));
                documentoAnterior.ChaveAcesso = (string)dynDocumentoAnterior.Chave;
                documentosAnteriores.Add(documentoAnterior);
            }

            return documentosAnteriores;

        }

        public void SalvarInformacoesDocumentosAnteriores(ref Dominio.Entidades.ConhecimentoDeTransporteEletronico cte, List<Dominio.ObjetosDeValor.Embarcador.CTe.DocumentoAnterior> documentosAnteriores, Repositorio.UnitOfWork unitOfWork)
        {

            Repositorio.DocumentoDeTransporteAnteriorCTe repDocumento = new Repositorio.DocumentoDeTransporteAnteriorCTe(unitOfWork);
            Repositorio.Cliente repCliente = new Repositorio.Cliente(unitOfWork);

            if (cte.Codigo > 0)
                repDocumento.DeletarPorCTe(cte.Codigo);

            foreach (Dominio.ObjetosDeValor.Embarcador.CTe.DocumentoAnterior documentoAnterior in documentosAnteriores)
            {
                Dominio.Entidades.DocumentoDeTransporteAnteriorCTe documento = new Dominio.Entidades.DocumentoDeTransporteAnteriorCTe();

                documento.Chave = Utilidades.String.OnlyNumbers(documentoAnterior.ChaveAcesso);
                documento.CTe = cte;
                documento.Emissor = repCliente.BuscarPorCPFCNPJ(double.Parse(Utilidades.String.OnlyNumbers(documentoAnterior.Emitente.CPFCNPJ)));
                repDocumento.Inserir(documento);
            }
        }


        public void SalvarInformacoesDocumentosAnterioresPreCTe(ref Dominio.Entidades.PreConhecimentoDeTransporteEletronico PreCte, List<Dominio.ObjetosDeValor.Embarcador.CTe.DocumentoAnterior> documentosAnteriores, Repositorio.UnitOfWork unitOfWork)
        {

            Repositorio.DocumentoDeTransporteAnteriorPreCTE repDocumento = new Repositorio.DocumentoDeTransporteAnteriorPreCTE(unitOfWork);
            Repositorio.Cliente repCliente = new Repositorio.Cliente(unitOfWork);

            if (PreCte.Codigo > 0)
                repDocumento.DeletarPorPreCTe(PreCte.Codigo);

            foreach (Dominio.ObjetosDeValor.Embarcador.CTe.DocumentoAnterior documentoAnterior in documentosAnteriores)
            {
                Dominio.Entidades.DocumentoDeTransporteAnteriorPreCTE documento = new Dominio.Entidades.DocumentoDeTransporteAnteriorPreCTE();

                documento.Chave = documentoAnterior.ChaveAcesso;
                documento.PreCTE = PreCte;
                documento.Emissor = repCliente.BuscarPorCPFCNPJ(double.Parse(Utilidades.String.OnlyNumbers(documentoAnterior.Emitente.CPFCNPJ)));
                repDocumento.Inserir(documento);
            }
        }
    }
}
