using System;
using System.Collections.Generic;
using System.Linq;

namespace Servicos.Embarcador.NotaFiscal
{
    public class NotaFiscalReferencia : ServicoBase
    {
        public NotaFiscalReferencia(Repositorio.UnitOfWork unitOfWork) : base(unitOfWork) { }
        public void SalvarReferenciasNFe(ref Dominio.Entidades.Embarcador.NotaFiscal.NotaFiscal nfe, List<Dominio.ObjetosDeValor.Embarcador.NotaFiscal.NotaFiscalReferencia> referenciasNFe, Repositorio.UnitOfWork unitOfWork, Dominio.Entidades.Empresa empresa)
        {
            Repositorio.Embarcador.NotaFiscal.NotaFiscalReferencia repNotaFiscalReferencia = new Repositorio.Embarcador.NotaFiscal.NotaFiscalReferencia(unitOfWork);
            if (nfe.Codigo > 0)
                repNotaFiscalReferencia.DeletarPorNFe(nfe.Codigo);

            if (referenciasNFe == null || referenciasNFe.Count() == 0)
                return;            

            if (referenciasNFe != null)
            {
                foreach (Dominio.ObjetosDeValor.Embarcador.NotaFiscal.NotaFiscalReferencia refer in referenciasNFe)
                {
                    Dominio.Entidades.Embarcador.NotaFiscal.NotaFiscalReferencia referencia = new Dominio.Entidades.Embarcador.NotaFiscal.NotaFiscalReferencia();
                    referencia.Chave = Utilidades.String.OnlyNumbers(refer.Chave);
                    if (Utilidades.String.OnlyNumbers(refer.CPFEmitente).Length == 14)
                        referencia.CNPJEmitente = Utilidades.String.OnlyNumbers(refer.CNPJEmitente);
                    referencia.COO = refer.COO;                    
                    if (Utilidades.String.OnlyNumbers(refer.CPFEmitente).Length == 11)
                        referencia.CPFEmitente = Utilidades.String.OnlyNumbers(refer.CPFEmitente);
                    if (refer.DataEmissao > DateTime.MinValue)
                        referencia.DataEmissao = refer.DataEmissao;
                    referencia.IEEmitente = refer.IEEmitente;
                    referencia.Modelo = refer.Modelo;
                    referencia.NotaFiscal = nfe;
                    referencia.Numero = refer.Numero;
                    referencia.NumeroECF = refer.NumeroECF;
                    referencia.Serie = refer.Serie;
                    referencia.TipoDocumento = refer.TipoDocumento;
                    referencia.UF = refer.UF;

                    repNotaFiscalReferencia.Inserir(referencia);
                }
            }
        }

        public List<Dominio.ObjetosDeValor.Embarcador.NotaFiscal.NotaFiscalReferencia> ConverterNotaFiscalReferencia(List<Dominio.Entidades.Embarcador.NotaFiscal.NotaFiscalReferencia> referenciasNFe, Repositorio.UnitOfWork unitOfWork)
        {
            List<Dominio.ObjetosDeValor.Embarcador.NotaFiscal.NotaFiscalReferencia> referencias = new List<Dominio.ObjetosDeValor.Embarcador.NotaFiscal.NotaFiscalReferencia>();

            foreach (Dominio.Entidades.Embarcador.NotaFiscal.NotaFiscalReferencia refer in referenciasNFe)
            {
                Dominio.ObjetosDeValor.Embarcador.NotaFiscal.NotaFiscalReferencia referencia = new Dominio.ObjetosDeValor.Embarcador.NotaFiscal.NotaFiscalReferencia();

                referencia.Chave = refer.Chave;
                referencia.CNPJEmitente = refer.CNPJEmitente;
                referencia.COO = refer.COO;
                referencia.CPFEmitente = refer.CPFEmitente;
                referencia.DataEmissao = refer.DataEmissao;
                referencia.IEEmitente = refer.IEEmitente;
                referencia.Modelo = refer.Modelo;
                referencia.Numero = refer.Numero;
                referencia.NumeroECF = refer.NumeroECF;
                referencia.Serie = refer.Serie;
                referencia.TipoDocumento = refer.TipoDocumento;
                referencia.UF = refer.UF;

                referencias.Add(referencia);
            }
            return referencias;
        }
    }
}
