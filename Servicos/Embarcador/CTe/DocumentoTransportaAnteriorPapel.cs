using Repositorio;
using System;
using System.Collections.Generic;

namespace Servicos.Embarcador.CTe
{
    public class DocumentoTransportaAnteriorPapel : ServicoBase
    {       
        public DocumentoTransportaAnteriorPapel(UnitOfWork unitOfWork) : base(unitOfWork) { }

        public List<Dominio.ObjetosDeValor.Embarcador.CTe.DocumentoAnteriorPapel> ConverterDocumentoTransporteParaTransporteAnteriorPapel(List<Dominio.Entidades.DocumentoDeTransporteAnteriorCTe> documentosAnterioresCTe, Repositorio.UnitOfWork unitOfWork)
        {
            Servicos.WebService.Pessoas.Pessoa serPessoa = new WebService.Pessoas.Pessoa(unitOfWork);

            List<Dominio.ObjetosDeValor.Embarcador.CTe.DocumentoAnteriorPapel> documentosAnterioresPapel = new List<Dominio.ObjetosDeValor.Embarcador.CTe.DocumentoAnteriorPapel>();

            foreach (Dominio.Entidades.DocumentoDeTransporteAnteriorCTe documentoAnteriorCTe in documentosAnterioresCTe)
            {
                Dominio.ObjetosDeValor.Embarcador.CTe.DocumentoAnteriorPapel documentoAnteriorPapel = new Dominio.ObjetosDeValor.Embarcador.CTe.DocumentoAnteriorPapel();

                documentoAnteriorPapel.Emitente = serPessoa.ConverterObjetoPessoa(documentoAnteriorCTe.Emissor);
                documentoAnteriorPapel.TipoDocumentoTransportaAnteriorPapel = (string)documentoAnteriorCTe.Tipo;
                documentoAnteriorPapel.Numero = (string)documentoAnteriorCTe.Numero;
                documentoAnteriorPapel.Serie = (string)documentoAnteriorCTe.Serie;
                documentoAnteriorPapel.DataEmissao = documentoAnteriorCTe.DataEmissao.HasValue ? documentoAnteriorCTe.DataEmissao.Value : DateTime.Now;
                documentosAnterioresPapel.Add(documentoAnteriorPapel);
            }
            return documentosAnterioresPapel;
        }


        public List<Dominio.ObjetosDeValor.Embarcador.CTe.DocumentoAnteriorPapel> ConverterDynamicParaDocumentosTransporteAnteriorPapel(dynamic dynDocumentosTransporteAnteriorPapel, Repositorio.UnitOfWork unitOfWork)
        {
            Repositorio.Cliente repCliente = new Repositorio.Cliente(unitOfWork);
            Servicos.WebService.Pessoas.Pessoa serPessoa = new WebService.Pessoas.Pessoa(unitOfWork);

            List<Dominio.ObjetosDeValor.Embarcador.CTe.DocumentoAnteriorPapel> documentosAnterioresPapel = new List<Dominio.ObjetosDeValor.Embarcador.CTe.DocumentoAnteriorPapel>();

            foreach (var dynDocumentoAnteriorPapel in dynDocumentosTransporteAnteriorPapel)
            {
                Dominio.ObjetosDeValor.Embarcador.CTe.DocumentoAnteriorPapel documentoAnteriorPapel = new Dominio.ObjetosDeValor.Embarcador.CTe.DocumentoAnteriorPapel();

                documentoAnteriorPapel.Emitente = serPessoa.ConverterObjetoPessoa(repCliente.BuscarPorCPFCNPJ((double)dynDocumentoAnteriorPapel.CodigoEmitente));
                documentoAnteriorPapel.TipoDocumentoTransportaAnteriorPapel = (string)dynDocumentoAnteriorPapel.Tipo;
                documentoAnteriorPapel.Numero = (string)dynDocumentoAnteriorPapel.Numero;
                documentoAnteriorPapel.Serie = (string)dynDocumentoAnteriorPapel.Serie;
                DateTime dataEmissao;
                DateTime.TryParseExact((string)dynDocumentoAnteriorPapel.DataEmissao, "dd/MM/yyyy", null, System.Globalization.DateTimeStyles.None, out dataEmissao);
                documentoAnteriorPapel.DataEmissao = dataEmissao;
                documentosAnterioresPapel.Add(documentoAnteriorPapel);
            }
            return documentosAnterioresPapel;
        }

        public void SalvarInformacoesDocumentosAnterioresPapel(ref Dominio.Entidades.ConhecimentoDeTransporteEletronico cte, List<Dominio.ObjetosDeValor.Embarcador.CTe.DocumentoAnteriorPapel> documentosAnterioresPapel, Repositorio.UnitOfWork unitOfWork)
        {

            Repositorio.DocumentoDeTransporteAnteriorCTe repDocumento = new Repositorio.DocumentoDeTransporteAnteriorCTe(unitOfWork);
            Repositorio.Cliente repCliente = new Repositorio.Cliente(unitOfWork);

            foreach (Dominio.ObjetosDeValor.Embarcador.CTe.DocumentoAnteriorPapel documentoAnteriorPapel in documentosAnterioresPapel)
            {
                Dominio.Entidades.DocumentoDeTransporteAnteriorCTe documento = new Dominio.Entidades.DocumentoDeTransporteAnteriorCTe();
                documento.CTe = cte;
                documento.Emissor = repCliente.BuscarPorCPFCNPJ(double.Parse(Utilidades.String.OnlyNumbers(documentoAnteriorPapel.Emitente.CPFCNPJ)));
                documento.DataEmissao = documentoAnteriorPapel.DataEmissao;
                documento.Numero = documentoAnteriorPapel.Numero;
                documento.Serie = documentoAnteriorPapel.Serie;
                documento.Tipo = documentoAnteriorPapel.TipoDocumentoTransportaAnteriorPapel;

                repDocumento.Inserir(documento);
            }
        }

        public void SalvarInformacoesDocumentosAnterioresPapelPreCTe(ref Dominio.Entidades.PreConhecimentoDeTransporteEletronico preCTe, List<Dominio.ObjetosDeValor.Embarcador.CTe.DocumentoAnteriorPapel> documentosAnterioresPapel, Repositorio.UnitOfWork unitOfWork)
        {

            Repositorio.DocumentoDeTransporteAnteriorPreCTE repDocumento = new Repositorio.DocumentoDeTransporteAnteriorPreCTE(unitOfWork);
            Repositorio.Cliente repCliente = new Repositorio.Cliente(unitOfWork);

            foreach (Dominio.ObjetosDeValor.Embarcador.CTe.DocumentoAnteriorPapel documentoAnteriorPapel in documentosAnterioresPapel)
            {
                Dominio.Entidades.DocumentoDeTransporteAnteriorPreCTE documento = new Dominio.Entidades.DocumentoDeTransporteAnteriorPreCTE();
                documento.PreCTE = preCTe;
                documento.Emissor = repCliente.BuscarPorCPFCNPJ(double.Parse(Utilidades.String.OnlyNumbers(documentoAnteriorPapel.Emitente.CPFCNPJ)));
                documento.DataEmissao = documentoAnteriorPapel.DataEmissao;
                documento.Numero = documentoAnteriorPapel.Numero;
                documento.Serie = documentoAnteriorPapel.Serie;
                documento.Tipo = documentoAnteriorPapel.TipoDocumentoTransportaAnteriorPapel;

                repDocumento.Inserir(documento);
            }
        }
    }
}
