using System;
using System.Collections.Generic;

namespace Servicos.Embarcador.NotaFiscal
{
    public class NotaFiscalParcela : ServicoBase
    {        
        public NotaFiscalParcela(Repositorio.UnitOfWork unitOfWork) : base(unitOfWork) { }
        public void SalvarParcelasNFe(ref Dominio.Entidades.Embarcador.NotaFiscal.NotaFiscal nfe, List<Dominio.ObjetosDeValor.Embarcador.NotaFiscal.NotaFiscalParcela> parcelasNFe, Repositorio.UnitOfWork unitOfWork, Dominio.Entidades.Empresa empresa)
        {
            Repositorio.Embarcador.NotaFiscal.NotaFiscalParcela repNotaFiscalParcela = new Repositorio.Embarcador.NotaFiscal.NotaFiscalParcela(unitOfWork);
            if (nfe.Codigo > 0)
                repNotaFiscalParcela.DeletarPorNFe(nfe.Codigo);

            if (parcelasNFe != null)
            {
                foreach (Dominio.ObjetosDeValor.Embarcador.NotaFiscal.NotaFiscalParcela parc in parcelasNFe)
                {
                    Dominio.Entidades.Embarcador.NotaFiscal.NotaFiscalParcela parcela = new Dominio.Entidades.Embarcador.NotaFiscal.NotaFiscalParcela();
                    parcela.Acrescimo = parc.Acrescimo;
                    parcela.DataEmissao = parc.DataEmissao;
                    parcela.DataVencimento = parc.DataVencimento;
                    parcela.Desconto = parc.Desconto;
                    parcela.NotaFiscal = nfe;
                    parcela.Sequencia = parc.Sequencia;
                    parcela.Situacao = parc.Situacao;
                    parcela.Valor = parc.Valor;
                    parcela.Forma = parc.Forma;

                    repNotaFiscalParcela.Inserir(parcela);
                }
            }
        }

        public List<Dominio.ObjetosDeValor.Embarcador.NotaFiscal.NotaFiscalParcela> ConverterNotaFiscalParcela(List<Dominio.Entidades.Embarcador.NotaFiscal.NotaFiscalParcela> parcelasNFe, Repositorio.UnitOfWork unitOfWork)
        {
            List<Dominio.ObjetosDeValor.Embarcador.NotaFiscal.NotaFiscalParcela> parcelas = new List<Dominio.ObjetosDeValor.Embarcador.NotaFiscal.NotaFiscalParcela>();

            foreach (Dominio.Entidades.Embarcador.NotaFiscal.NotaFiscalParcela parc in parcelasNFe)
            {
                Dominio.ObjetosDeValor.Embarcador.NotaFiscal.NotaFiscalParcela parcela = new Dominio.ObjetosDeValor.Embarcador.NotaFiscal.NotaFiscalParcela();

                parcela.Acrescimo = parc.Acrescimo;
                parcela.DataEmissao = parc.DataEmissao;
                parcela.DataVencimento = parc.DataVencimento;
                parcela.Desconto = parc.Desconto;
                parcela.Sequencia = parc.Sequencia;
                parcela.Situacao = parc.Situacao;
                parcela.Valor = parc.Valor;
                parcela.Forma = parc.Forma;

                parcelas.Add(parcela);
            }
            return parcelas;
        }

        public void SetarDynamicParaParcelas(dynamic dynNFe, ref Dominio.ObjetosDeValor.Embarcador.NotaFiscal.NotaFiscalEletronica nfe, Repositorio.UnitOfWork unitOfWork)
        {
            nfe.ParcelasNFe = new List<Dominio.ObjetosDeValor.Embarcador.NotaFiscal.NotaFiscalParcela>();

            foreach (var parc in dynNFe.Parcelas)
            {
                Dominio.ObjetosDeValor.Embarcador.NotaFiscal.NotaFiscalParcela parcela = new Dominio.ObjetosDeValor.Embarcador.NotaFiscal.NotaFiscalParcela();

                decimal acrescimo, desconto, valor = 0;
                decimal.TryParse((string)parc.Acrescimo, out acrescimo);
                decimal.TryParse((string)parc.Desconto, out desconto);
                decimal.TryParse((string)parc.Valor, out valor);

                DateTime dataEmissao = new DateTime();
                DateTime.TryParseExact((string)parc.DataEmissao, "dd/MM/yyyy", null, System.Globalization.DateTimeStyles.None, out dataEmissao);
                DateTime dataVencimento = new DateTime();
                DateTime.TryParseExact((string)parc.DataVencimento, "dd/MM/yyyy", null, System.Globalization.DateTimeStyles.None, out dataVencimento);

                Dominio.ObjetosDeValor.Embarcador.Enumeradores.FormaTitulo formaTitulo;
                Enum.TryParse((string)parc.FormaTitulo, out formaTitulo);

                parcela.Acrescimo = acrescimo;
                parcela.DataEmissao = dataEmissao;
                parcela.DataVencimento = dataVencimento;
                parcela.Desconto = desconto;
                parcela.Sequencia = (int)parc.Sequencia;
                if ((int)parc.CodigoStatus > 0)
                    parcela.Situacao = (Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoFaturaParcela)parc.CodigoStatus;
                parcela.Valor = valor;
                parcela.Forma = formaTitulo;

                nfe.ParcelasNFe.Add(parcela);
            }
        }
    }
}
