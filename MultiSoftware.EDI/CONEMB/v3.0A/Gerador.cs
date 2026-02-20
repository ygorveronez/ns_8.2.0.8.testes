using System;
using System.Collections.Generic;
using System.Linq;
using System.IO;
using Dominio.Entidades.EDI.CONEMB;
using Dominio.Entidades.EDI.CONEMB.v30A;

namespace MultiSoftware.EDI.CONEMB.v30A
{
    public class Gerador : EDIBase
    {
        #region Construtores

        public Gerador(Dominio.Entidades.Empresa empresa, string cpfCnpjRemetente, DateTime dataInicial, DateTime dataFinal, Repositorio.UnitOfWork unidadeDeTrabalho) : base(unidadeDeTrabalho.StringConexao, unidadeDeTrabalho)
        {
            this.Arquivo = new ArquivoCONEMB();

            Repositorio.ConhecimentoDeTransporteEletronico repCTe = new Repositorio.ConhecimentoDeTransporteEletronico(unidadeDeTrabalho);
            this.CTes = repCTe.BuscarPorRemetente(empresa.Codigo, cpfCnpjRemetente, dataInicial, dataFinal, new string[] { "A" }, empresa.TipoAmbiente);

            int[] codigosCTes = (from obj in this.CTes select obj.Codigo).ToArray();

            Repositorio.DocumentosCTE repDocumentos = new Repositorio.DocumentosCTE(unidadeDeTrabalho);
            this.NotasFiscais = repDocumentos.BuscarPorCTe(empresa.Codigo, codigosCTes);

            Repositorio.ComponentePrestacaoCTE repComponentes = new Repositorio.ComponentePrestacaoCTE(unidadeDeTrabalho);
            this.ComponentesDaPrestacao = repComponentes.BuscarPorCTe(empresa.Codigo, codigosCTes);

            Repositorio.InformacaoCargaCTE repInformacoes = new Repositorio.InformacaoCargaCTE(unidadeDeTrabalho);
            this.InformacoesDaCarga = repInformacoes.BuscarPorCTe(empresa.Codigo, codigosCTes);
        }

        public Gerador(Dominio.Entidades.Empresa empresa, int codigoCTe, Repositorio.UnitOfWork unidadeDeTrabalho) : base(unidadeDeTrabalho.StringConexao, unidadeDeTrabalho)
        {
            this.Arquivo = new ArquivoCONEMB();
            this.CTes = new List<Dominio.Entidades.ConhecimentoDeTransporteEletronico>();
            this.NotasFiscais = new List<Dominio.Entidades.DocumentosCTE>();
            this.ComponentesDaPrestacao = new List<Dominio.Entidades.ComponentePrestacaoCTE>();
            this.InformacoesDaCarga = new List<Dominio.Entidades.InformacaoCargaCTE>();

            Repositorio.ConhecimentoDeTransporteEletronico repCTe = new Repositorio.ConhecimentoDeTransporteEletronico(unidadeDeTrabalho);
            this.CTes.Add(repCTe.BuscarPorCodigo(empresa.Codigo, codigoCTe, "A"));

            Repositorio.DocumentosCTE repDocumentos = new Repositorio.DocumentosCTE(unidadeDeTrabalho);
            this.NotasFiscais = repDocumentos.BuscarPorCTe(empresa.Codigo, codigoCTe);

            Repositorio.ComponentePrestacaoCTE repComponentes = new Repositorio.ComponentePrestacaoCTE(unidadeDeTrabalho);
            this.ComponentesDaPrestacao = repComponentes.BuscarPorCTe(empresa.Codigo, codigoCTe);

            Repositorio.InformacaoCargaCTE repInformacoes = new Repositorio.InformacaoCargaCTE(unidadeDeTrabalho);
            this.InformacoesDaCarga = repInformacoes.BuscarPorCTe(empresa.Codigo, codigoCTe);
        }

        public Gerador(Dominio.Entidades.Empresa empresa, DateTime dataInicial, DateTime dataFinal, string stringConexao, Repositorio.UnitOfWork unitOfWork) : base(stringConexao, unitOfWork)
        {
            this.CTes = new List<Dominio.Entidades.ConhecimentoDeTransporteEletronico>();
            this.NotasFiscais = new List<Dominio.Entidades.DocumentosCTE>();
            this.ComponentesDaPrestacao = new List<Dominio.Entidades.ComponentePrestacaoCTE>();
            this.InformacoesDaCarga = new List<Dominio.Entidades.InformacaoCargaCTE>();
            this.Arquivo = new ArquivoCONEMB();

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
        private List<Dominio.Entidades.ComponentePrestacaoCTE> ComponentesDaPrestacao { get; set; }
        private List<Dominio.Entidades.InformacaoCargaCTE> InformacoesDaCarga { get; set; }
        private Dominio.Entidades.EDI.CONEMB.ArquivoCONEMB Arquivo { get; set; }

        #endregion

        #region Metodos

        public MemoryStream GerarArquivo()
        {
            this.GerarCONEMB();

            return this.Arquivo.ObterArquivo();
        }

        //public MemoryStream GerarLote()
        //{
        //    MemoryStream fZip = new MemoryStream();
        //    ZipOutputStream zipOStream = new ZipOutputStream(fZip);
        //    zipOStream.SetLevel(9);

        //    Repositorio.ConhecimentoDeTransporteEletronico repCTe = new Repositorio.ConhecimentoDeTransporteEletronico(UnitOfWork);
        //    Repositorio.DocumentosCTE repDocumentos = new Repositorio.DocumentosCTE(UnitOfWork);
        //    Repositorio.ComponentePrestacaoCTE repComponentes = new Repositorio.ComponentePrestacaoCTE(UnitOfWork);
        //    Repositorio.InformacaoCargaCTE repInformacaoCarga = new Repositorio.InformacaoCargaCTE(UnitOfWork);

        //    List<string> cpfCnpjRemetentes = repCTe.BuscarRemetentes(this.Empresa.Codigo, this.DataInicial, this.DataFinal, "A", this.Empresa.TipoAmbiente);

        //    foreach (string cpfCnpjRemetente in cpfCnpjRemetentes)
        //    {
        //        this.Arquivo = new ArquivoCONEMB();

        //        this.CTes = repCTe.BuscarPorRemetente(this.Empresa.Codigo, cpfCnpjRemetente, this.DataInicial, this.DataFinal, "A", this.Empresa.TipoAmbiente);

        //        int[] codigoCTes = (from obj in this.CTes select obj.Codigo).ToArray();

        //        this.NotasFiscais = repDocumentos.BuscarPorCTe(this.Empresa.Codigo, codigoCTes);
        //        this.ComponentesDaPrestacao = repComponentes.BuscarPorCTe(this.Empresa.Codigo, codigoCTes);
        //        this.InformacoesDaCarga = repInformacaoCarga.BuscarPorCTe(this.Empresa.Codigo, codigoCTes);

        //        byte[] arquivo = System.Text.Encoding.Default.GetBytes(this.GerarString());
        //        ZipEntry entry = new ZipEntry(string.Concat(Utilidades.String.RemoveAllSpecialCharacters(this.CTes[0].Remetente.Nome), " - ", this.CTes[0].Remetente.CPF_CNPJ, ".txt"));
        //        entry.DateTime = DateTime.Now;
        //        zipOStream.PutNextEntry(entry);
        //        zipOStream.Write(arquivo, 0, arquivo.Length);
        //        zipOStream.CloseEntry();
        //    }

        //    zipOStream.IsStreamOwner = false;
        //    zipOStream.Close();
        //    fZip.Position = 0;
        //    return fZip;
        //}

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
            this.GerarRegistroCEM();
            this.GerarRegistroTCE();
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

        private void GerarRegistroCEM()
        {
            foreach (Dominio.Entidades.ConhecimentoDeTransporteEletronico cte in this.CTes)
            {
                this.Arquivo.Registros.Add(new CEM(cte,
                                                   (from obj in this.InformacoesDaCarga where obj.CTE.Codigo == cte.Codigo select obj).FirstOrDefault(),
                                                   (from obj in this.ComponentesDaPrestacao where obj.CTE.Codigo == cte.Codigo select obj).ToList(),
                                                   (from obj in this.NotasFiscais where obj.CTE.Codigo == cte.Codigo select obj).ToList()));
                this.GerarRegistroDCC(cte);
            }
        }

        private void GerarRegistroDCC(Dominio.Entidades.ConhecimentoDeTransporteEletronico cte)
        {
            this.Arquivo.Registros.Add(new DCC(cte));
        }

        private void GerarRegistroTCE()
        {
            this.Arquivo.Registros.Add(new TCE(this.CTes.Count(), (from obj in this.CTes select obj.ValorFrete).Sum()));
        }

        #endregion
    }
}
