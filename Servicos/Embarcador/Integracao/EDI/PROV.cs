using System;
using System.Linq;

namespace Servicos.Embarcador.Integracao.EDI
{
    public class PROV
    {
        public Dominio.ObjetosDeValor.EDI.PROV.Provisao GerarPorProvisao(Dominio.Entidades.Embarcador.Escrituracao.Provisao provisao, Repositorio.UnitOfWork unitOfWork)
        {
            Repositorio.Embarcador.Filiais.Filial repFilial = new Repositorio.Embarcador.Filiais.Filial(unitOfWork);
            Dominio.Entidades.Embarcador.Filiais.Filial matriz = repFilial.BuscarMatriz();

            Dominio.Entidades.Embarcador.Escrituracao.DocumentoProvisao documentoMaisNovo = (from o in provisao.DocumentosProvisao
                                                                                             where o.CTe != null && o.CTe.DataEmissao.HasValue
                                                                                             orderby o.CTe.DataEmissao descending
                                                                                             select o).FirstOrDefault();

            var documentosValidos = (from o in provisao.DocumentosProvisao
                                     where o.CTe != null && o.CTe.ModeloDocumentoFiscal != null
                                     select o.CTe);

            decimal debitos = (from o in documentosValidos
                               where o.ModeloDocumentoFiscal.TipoDocumentoCreditoDebito == Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoDocumentoCreditoDebito.Debito
                               select o.ValorAReceber).Sum();

            decimal creditos = (from o in documentosValidos
                                where o.ModeloDocumentoFiscal.TipoDocumentoCreditoDebito != Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoDocumentoCreditoDebito.Debito
                                select o.ValorAReceber).Sum();

            
            decimal totalProvisao = creditos - debitos;
            if (documentoMaisNovo != null)
            {
                Dominio.ObjetosDeValor.EDI.PROV.Provisao objEDi = new Dominio.ObjetosDeValor.EDI.PROV.Provisao()
                {
                    Evento = "",
                    CodigoEvento = "",
                    CodigoVersao = "",
                    IdentificadorSistema = "BRTTL",
                    DataExtracao = provisao.DataCriacao,
                    Matriz = matriz.CNPJ,
                    Filial = documentoMaisNovo.Filial?.CNPJ ?? string.Empty,
                    DataContabil = documentoMaisNovo.CTe.DataEmissao ?? DateTime.MinValue,
                    Departamento = "",
                    CreditoDebito = (totalProvisao < 0 ? "-" : "+"),
                    ValorTotal = Math.Abs(totalProvisao),
                    ValorProvisao = provisao.ValorProvisao,
                    CodigoTransacao = "",
                    DescricaoTransacao = "Transferencia Transportador Loja",
                };

                return objEDi;
            }
            else
            {
                return null;
            }

        }
    }
}
