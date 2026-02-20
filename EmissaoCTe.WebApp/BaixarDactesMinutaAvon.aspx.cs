using System;
using System.Collections.Generic;
using System.Configuration;

namespace EmissaoCTe.WebApp
{
    public partial class BaixarDactesMinutaAvon : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {

        }

        protected void btnBaixarDactes_Click(object sender, EventArgs e)
        {
            Repositorio.UnitOfWork unidadeDeTrabalho = new Repositorio.UnitOfWork(Conexao.StringConexao);
            Repositorio.Usuario repUsuario = new Repositorio.Usuario(unidadeDeTrabalho);
            Repositorio.ManifestoAvon repManifestoAvon = new Repositorio.ManifestoAvon(unidadeDeTrabalho);
            Repositorio.DocumentoManifestoAvon repDocumentoManifestoAvon = new Repositorio.DocumentoManifestoAvon(unidadeDeTrabalho);
            Repositorio.ConhecimentoDeTransporteEletronico repCTe = new Repositorio.ConhecimentoDeTransporteEletronico(unidadeDeTrabalho);

            Servicos.DACTE svcDACTE = new Servicos.DACTE(unidadeDeTrabalho);

            Dominio.Entidades.Usuario usuario = repUsuario.BuscarPorCodigo(Session["IdUsuario"] != null ? (int)Session["IdUsuario"] : 0);

            if (usuario == null)
                throw new Exception("Usuário inválido.");

            if (string.IsNullOrWhiteSpace(ConfigurationManager.AppSettings["CaminhoRelatorios"]))
                throw new Exception("O caminho para os download da DACTE não está disponível. Contate o suporte técnico.");

            Dominio.Entidades.ManifestoAvon manifestoAvon = repManifestoAvon.BuscarPorNumero(this.txtMinuta.Text, Dominio.ObjetosDeValor.Enumerador.TipoIntegradoraManifesto.Avon, usuario.Empresa.Codigo);

            if (manifestoAvon == null)
                throw new Exception("Minuta não encontrada.");

            List<Dominio.Entidades.DocumentoManifestoAvon> listaDocumentoManifestoAvon = repDocumentoManifestoAvon.BuscarPorManifesto(manifestoAvon.Codigo);

            if (listaDocumentoManifestoAvon == null || listaDocumentoManifestoAvon.Count == 0)
                throw new Exception("Minuta sem Documentos.");

            foreach (Dominio.Entidades.DocumentoManifestoAvon documentoManifestoAvon in listaDocumentoManifestoAvon)
            {
                Dominio.Entidades.ConhecimentoDeTransporteEletronico cte = repCTe.BuscarPorCodigo(documentoManifestoAvon.CTe.Codigo);
                if (cte != null && cte.Chave != null)
                {
                    string caminhoPDF = Utilidades.IO.FileStorageService.Storage.Combine(ConfigurationManager.AppSettings["CaminhoRelatorios"], cte.Empresa.CNPJ, cte.Chave) + ".pdf";

                    byte[] dacte = null;

                    ////Buscar DACTE do Oracle
                    if (!Utilidades.IO.FileStorageService.Storage.Exists(caminhoPDF) && ConfigurationManager.AppSettings["RegerarDACTEOracle"] != "NAO")
                    {
                        Servicos.CTe servicoCTe = new Servicos.CTe(unidadeDeTrabalho);
                        servicoCTe.ObterESalvarDACTE(cte, cte.Empresa.Codigo, null, unidadeDeTrabalho);
                    }

                    if (!Utilidades.IO.FileStorageService.Storage.Exists(caminhoPDF))
                    {
                        dacte = svcDACTE.GerarPorProcesso(cte.Codigo);
                    }
                    else
                    {
                        dacte = Utilidades.IO.FileStorageService.Storage.ReadAllBytes(caminhoPDF);
                    }

                    if (dacte != null)
                    {
                        string filePath = string.Concat("C:\\DACTES_AVON\\", cte.Chave, ".pdf");
                        using (System.IO.Stream fs = Utilidades.IO.FileStorageService.Storage.OpenWrite(filePath))
                            fs.Write(dacte, 0, dacte.Length);
                    }
                }
            }
        }
    }
}


