using Dominio.Entidades.EDI.DOCCOB;
using Dominio.Entidades.EDI.DOCCOB.v30A;
using ICSharpCode.SharpZipLib.Zip;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace MultiSoftware.EDI.DOCCOB.v30A
{
    public class Gerador : EDIBase
    {
        #region Construtores

        public Gerador(Dominio.Entidades.Empresa empresa, string cpfCnpjRemetente, DateTime dataInicial, DateTime dataFinal, string stringConexao, Repositorio.UnitOfWork unitOfWork) : base(stringConexao, unitOfWork)
        {
            this.Arquivo = new ArquivoDOCCOB();

            Repositorio.ConhecimentoDeTransporteEletronico repCTe = new Repositorio.ConhecimentoDeTransporteEletronico(unitOfWork);
            this.CTes = repCTe.BuscarPorRemetente(empresa.Codigo, cpfCnpjRemetente, dataInicial, dataFinal, new string[] { "A" }, empresa.TipoAmbiente);

            int[] codigosCTes = (from obj in this.CTes select obj.Codigo).ToArray();

            Repositorio.DocumentosCTE repDocumentos = new Repositorio.DocumentosCTE(unitOfWork);
            this.NotasFiscais = repDocumentos.BuscarPorCTe(empresa.Codigo, codigosCTes);
        }

        public Gerador(Dominio.Entidades.Empresa empresa, int codigoCTe, string stringConexao, Repositorio.UnitOfWork unitOfWork) : base(stringConexao, unitOfWork)
        {
            this.Arquivo = new ArquivoDOCCOB();
            this.CTes = new List<Dominio.Entidades.ConhecimentoDeTransporteEletronico>();
            this.NotasFiscais = new List<Dominio.Entidades.DocumentosCTE>();

            Repositorio.ConhecimentoDeTransporteEletronico repCTe = new Repositorio.ConhecimentoDeTransporteEletronico(unitOfWork);
            this.CTes.Add(repCTe.BuscarPorCodigo(empresa.Codigo, codigoCTe, "A"));

            Repositorio.DocumentosCTE repDocumentos = new Repositorio.DocumentosCTE(unitOfWork);
            this.NotasFiscais = repDocumentos.BuscarPorCTe(empresa.Codigo, codigoCTe);
        }

        public Gerador(Dominio.Entidades.Empresa empresa, DateTime dataInicial, DateTime dataFinal, string stringConexao, Repositorio.UnitOfWork unitOfWork) : base(stringConexao, unitOfWork)
        {
            this.CTes = new List<Dominio.Entidades.ConhecimentoDeTransporteEletronico>();
            this.NotasFiscais = new List<Dominio.Entidades.DocumentosCTE>();
            this.Arquivo = new ArquivoDOCCOB();

            this.Empresa = empresa;
            this.DataInicial = dataInicial;
            this.DataFinal = dataFinal;
        }

        #endregion

        #region Propriedades

        private Dominio.Entidades.Empresa Empresa { get; set; }

        private List<Dominio.Entidades.Cliente> Remetentes { get; set; }

        private DateTime DataInicial { get; set; }

        private DateTime DataFinal { get; set; }

        private List<Dominio.Entidades.ConhecimentoDeTransporteEletronico> CTes { get; set; }

        private List<Dominio.Entidades.DocumentosCTE> NotasFiscais { get; set; }

        private Dominio.Entidades.EDI.DOCCOB.ArquivoDOCCOB Arquivo { get; set; }

        #endregion

        #region Metodos

        public MemoryStream GerarArquivo()
        {
            this.GerarCONEMB();

            return this.Arquivo.ObterArquivo();
        }

        public MemoryStream GerarLote()
        {
            MemoryStream fZip = new MemoryStream();
            ZipOutputStream zipOStream = new ZipOutputStream(fZip);
            zipOStream.SetLevel(9);

            Repositorio.ConhecimentoDeTransporteEletronico repCTe = new Repositorio.ConhecimentoDeTransporteEletronico(UnitOfWork);
            Repositorio.DocumentosCTE repDocumentos = new Repositorio.DocumentosCTE(UnitOfWork);
            Repositorio.Cliente repCliente = new Repositorio.Cliente(UnitOfWork);

            List<string> cpfCnpjRemetentes = repCTe.BuscarRemetentes(this.Empresa.Codigo, this.DataInicial, this.DataFinal, new string[] { "A" }, this.Empresa.TipoAmbiente);

            foreach (string cpfCnpjRemetente in cpfCnpjRemetentes)
            {
                this.Arquivo = new ArquivoDOCCOB();

                this.CTes = repCTe.BuscarPorRemetente(this.Empresa.Codigo, cpfCnpjRemetente, this.DataInicial, this.DataFinal, new string[] { "A" }, this.Empresa.TipoAmbiente);

                int[] codigoCTes = (from obj in this.CTes select obj.Codigo).ToArray();

                this.NotasFiscais = repDocumentos.BuscarPorCTe(this.Empresa.Codigo, codigoCTes);

                byte[] arquivo = System.Text.Encoding.Default.GetBytes(this.GerarString());
                ZipEntry entry = new ZipEntry(string.Concat(Utilidades.String.RemoveAllSpecialCharacters(this.CTes[0].Remetente.Nome), " - ", this.CTes[0].Remetente.CPF_CNPJ_SemFormato, ".txt"));
                entry.DateTime = DateTime.Now;
                zipOStream.PutNextEntry(entry);
                zipOStream.Write(arquivo, 0, arquivo.Length);
                zipOStream.CloseEntry();
            }

            zipOStream.IsStreamOwner = false;
            zipOStream.Close();
            fZip.Position = 0;
            return fZip;
        }

        public string GerarString()
        {
            this.GerarCONEMB();

            return this.Arquivo.ObterStringArquivo();
        }

        private void GerarCONEMB()
        {
            this.GerarRegistroUNB();
            this.GerarRegistroUNH();
            this.GerarRegistroTRA();
            this.GerarRegistroDCO();
            this.GerarRegistroCCO();
            this.GerarRegistroTDC();
        }

        private void GerarRegistroUNB()
        {
            this.Arquivo.Registros.Add(new UNB(this.CTes.First()));
        }

        private void GerarRegistroUNH()
        {
            this.Arquivo.Registros.Add(new UNH());
        }

        private void GerarRegistroTRA()
        {
            this.Arquivo.Registros.Add(new TRA(this.CTes.First().Empresa));
        }

        private void GerarRegistroDCO()
        {
            this.Arquivo.Registros.Add(new DCO((from obj in this.CTes select obj).OrderByDescending(o => o.Numero).First(),
                                               (from obj in this.CTes select obj.ValorFrete).Sum(),
                                               (from obj in this.CTes select obj.ValorICMS).Sum()));
        }

        private void GerarRegistroCCO()
        {
            foreach (Dominio.Entidades.ConhecimentoDeTransporteEletronico cte in this.CTes)
            {
                this.Arquivo.Registros.Add(new CCO(cte));
                this.GerarRegistroCNF(cte);
            }
        }

        private void GerarRegistroCNF(Dominio.Entidades.ConhecimentoDeTransporteEletronico cte)
        {
            var notasFiscais = from obj in this.NotasFiscais where obj.CTE.Codigo == cte.Codigo select obj;

            foreach (var notaFiscal in notasFiscais)
            {
                this.Arquivo.Registros.Add(new CNF(notaFiscal));
            }
        }

        private void GerarRegistroTDC()
        {
            this.Arquivo.Registros.Add(new TDC(this.CTes.Count(), (from obj in this.CTes select obj.ValorFrete).Sum()));
        }

        #endregion
    }
}
