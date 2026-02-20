using System;

namespace Servicos.Embarcador.CTe
{
    public class CTeSubstituicao : ServicoBase
    {        
        public CTeSubstituicao(Repositorio.UnitOfWork unitOfWork) : base(unitOfWork) { }
        public Dominio.ObjetosDeValor.Embarcador.CTe.CTeSubstituicao ConverterDynamicCTeSubstituicao(dynamic dynCTeSubstituicao, Repositorio.UnitOfWork unitOfWork)
        {
            Repositorio.ModeloDocumentoFiscal repModeloDocumentoFiscal = new Repositorio.ModeloDocumentoFiscal(unitOfWork);
            Dominio.ObjetosDeValor.Embarcador.CTe.CTeSubstituicao cteSubstituicao = new Dominio.ObjetosDeValor.Embarcador.CTe.CTeSubstituicao();

            cteSubstituicao.ChaveCTeSubstituido = Utilidades.String.OnlyNumbers((string)dynCTeSubstituicao.ChaveCTeSubstituido);
            cteSubstituicao.Tipo = ((string)dynCTeSubstituicao.Tipo).ToEnum<Dominio.Enumeradores.TipoDocumentoAnulacao>();
            cteSubstituicao.ContribuinteICMS = ((string)dynCTeSubstituicao.ContribuinteICMS).ToEnum<Dominio.Enumeradores.OpcaoSimNao>();

            if (cteSubstituicao.ContribuinteICMS == Dominio.Enumeradores.OpcaoSimNao.Sim)
            {
                if (cteSubstituicao.Tipo == Dominio.Enumeradores.TipoDocumentoAnulacao.CTe)
                {
                    cteSubstituicao.Chave = Utilidades.String.OnlyNumbers((string)dynCTeSubstituicao.Chave);
                    cteSubstituicao.ModeloDocumentoFiscal = repModeloDocumentoFiscal.BuscarPorModelo("57")?.Codigo ?? 0;
                }
                else if (cteSubstituicao.Tipo == Dominio.Enumeradores.TipoDocumentoAnulacao.NFe)
                {
                    cteSubstituicao.Chave = Utilidades.String.OnlyNumbers((string)dynCTeSubstituicao.Chave);
                    cteSubstituicao.ModeloDocumentoFiscal = repModeloDocumentoFiscal.BuscarPorModelo("55")?.Codigo ?? 0;
                }
                else
                {
                    cteSubstituicao.Emitente = ((string)dynCTeSubstituicao.Emitente).ToDouble();
                    cteSubstituicao.ModeloDocumentoFiscal = ((string)dynCTeSubstituicao.ModeloDocumentoFiscal).ToInt();
                    cteSubstituicao.Numero = (string)dynCTeSubstituicao.Numero;
                    cteSubstituicao.Serie = (string)dynCTeSubstituicao.Serie;
                    cteSubstituicao.Subserie = (string)dynCTeSubstituicao.Subserie;
                    cteSubstituicao.Valor = Utilidades.Decimal.Converter((string)dynCTeSubstituicao.Valor);
                    cteSubstituicao.DataEmissao = ((string)dynCTeSubstituicao.DataEmissao).ToDateTime();
                }
            }
            else
            {
                cteSubstituicao.Chave = Utilidades.String.OnlyNumbers((string)dynCTeSubstituicao.Chave);
                cteSubstituicao.Tipo = Dominio.Enumeradores.TipoDocumentoAnulacao.CTe;
                cteSubstituicao.ModeloDocumentoFiscal = repModeloDocumentoFiscal.BuscarPorModelo("57")?.Codigo ?? 0;
            }

            return cteSubstituicao;
        }

        public void SalvarCTeSubstituicao(ref Dominio.Entidades.ConhecimentoDeTransporteEletronico cte, Dominio.ObjetosDeValor.Embarcador.CTe.CTeSubstituicao cteSubstituicao, Repositorio.UnitOfWork unitOfWork)
        {
            Repositorio.DocumentosAnulacaoCTE repDocAnulacaoCTe = new Repositorio.DocumentosAnulacaoCTE(unitOfWork);
            Repositorio.Cliente repCliente = new Repositorio.Cliente(unitOfWork);
            Repositorio.ModeloDocumentoFiscal repModeloDocumentoFiscal = new Repositorio.ModeloDocumentoFiscal(unitOfWork);

            Dominio.Entidades.DocumentosAnulacaoCTE documentoAnulacao = repDocAnulacaoCTe.BuscarPorCTe(cte.Codigo);

            if (documentoAnulacao != null && cte.TipoCTE != Dominio.Enumeradores.TipoCTE.Substituto)
            {
                cte.ChaveCTESubComp = null;
                repDocAnulacaoCTe.Deletar(documentoAnulacao);
            }
            else if (cte.TipoCTE == Dominio.Enumeradores.TipoCTE.Substituto)
            {
                cte.ChaveCTESubComp = cteSubstituicao.ChaveCTeSubstituido;

                if (documentoAnulacao == null)
                    documentoAnulacao = new Dominio.Entidades.DocumentosAnulacaoCTE();

                documentoAnulacao.ModeloDocumentoFiscal = cteSubstituicao.ModeloDocumentoFiscal > 0 ? repModeloDocumentoFiscal.BuscarPorCodigo(cteSubstituicao.ModeloDocumentoFiscal, false) : null;
                documentoAnulacao.Emitente = cteSubstituicao.Emitente > 0 ? repCliente.BuscarPorCPFCNPJ(cteSubstituicao.Emitente) : null;
                documentoAnulacao.Chave = cteSubstituicao.Chave;
                documentoAnulacao.Numero = cteSubstituicao.Numero;
                documentoAnulacao.Serie = cteSubstituicao.Serie;
                documentoAnulacao.Subserie = cteSubstituicao.Subserie;
                documentoAnulacao.Valor = cteSubstituicao.Valor;
                if (cteSubstituicao.DataEmissao != DateTime.MinValue)
                    documentoAnulacao.DataEmissao = cteSubstituicao.DataEmissao;
                else
                    documentoAnulacao.DataEmissao = null;
                documentoAnulacao.ContribuinteICMS = cteSubstituicao.ContribuinteICMS;
                documentoAnulacao.Tipo = cteSubstituicao.Tipo;
                documentoAnulacao.CTE = cte;

                if (documentoAnulacao.Codigo > 0)
                    repDocAnulacaoCTe.Atualizar(documentoAnulacao);
                else
                    repDocAnulacaoCTe.Inserir(documentoAnulacao);
            }
        }
    }
}
