using System;
using System.Collections.Generic;
using System.Dynamic;
using System.IO;
using System.Text;

namespace Servicos.Embarcador.Integracao
{
    public class IntegracaoProcessamentoEDIFTP
    {
        #region Atributos

        private readonly Repositorio.UnitOfWork _unitOfWork;
        private Dominio.Entidades.Embarcador.Configuracao.ConfiguracaoTMS _configuracaoEmbarcador;
        private Dominio.Entidades.Empresa _empresa;

        #endregion


        #region Construtores
        public IntegracaoProcessamentoEDIFTP(Repositorio.UnitOfWork unitOfWork) : this(unitOfWork, configuracaoEmbarcador: null) { }

        public IntegracaoProcessamentoEDIFTP(Repositorio.UnitOfWork unitOfWork, Dominio.Entidades.Embarcador.Configuracao.ConfiguracaoTMS configuracaoEmbarcador)
        {
            _configuracaoEmbarcador = configuracaoEmbarcador;
            _unitOfWork = unitOfWork;
        }

        #endregion

        #region Métodos Pulicos

        public Dominio.ObjetosDeValor.Embarcador.Integracao.RetornoProcessamentoIntegracaoEDIFTP IntegrarEDIFTP(Dominio.Entidades.Embarcador.Integracao.IntegracaoProcessamentoEDIFTP integracaoProcessamentoPendente, AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware tipoServicoMultisoftware, Repositorio.UnitOfWork unitOfWork, AdminMultisoftware.Repositorio.UnitOfWork adminUnitofWork)
        {
            Dominio.ObjetosDeValor.Embarcador.Integracao.RetornoProcessamentoIntegracaoEDIFTP retornoIntegracao = new Dominio.ObjetosDeValor.Embarcador.Integracao.RetornoProcessamentoIntegracaoEDIFTP();
            Dominio.ObjetosDeValor.Embarcador.Auditoria.Auditado auditado = new Dominio.ObjetosDeValor.Embarcador.Auditoria.Auditado() { TipoAuditado = Dominio.ObjetosDeValor.Enumerador.TipoAuditado.Sistema };
            string caminhoCompleto = ObterCaminhoCompletoArquivo(integracaoProcessamentoPendente.GuidArquivo, unitOfWork);
            System.IO.MemoryStream Arquivo = new MemoryStream(Utilidades.IO.FileStorageService.Storage.ReadAllBytes(caminhoCompleto));
            Arquivo.Position = 0;

            Dominio.ObjetosDeValor.EDI.Notfis.EDINotFis NOTFIS = null;

            if (integracaoProcessamentoPendente.GrupoPessoas != null && integracaoProcessamentoPendente.GrupoPessoas.TipoIntegracao.Tipo == Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoIntegracao.Natura)
            {

                try
                {
                    //integracao EDI NATURA. 
                    Servicos.LeituraEDI serLeituraEDI = new Servicos.LeituraEDI(null, integracaoProcessamentoPendente.LayoutEDI, Arquivo, 0, 0, 0, 0, 0, 0, 0, 0, true, true, Encoding.GetEncoding("iso-8859-1"));
                    NOTFIS = serLeituraEDI.GerarNotasFis();
                    bool sucesso = Natura.IntegracaoDTNatura.GerarDT(out string erro, out List<Dominio.Entidades.Embarcador.Integracao.DTNatura> dtsNatura, null, null, null, NOTFIS, unitOfWork, adminUnitofWork, unitOfWork.StringConexao, adminUnitofWork.StringConexao, auditado, tipoServicoMultisoftware);

                    retornoIntegracao.Importados = dtsNatura != null ? dtsNatura.Count : 0;
                    retornoIntegracao.Sucesso = sucesso;
                    retornoIntegracao.MensagemAviso = erro;

                }
                catch (Exception ex)
                {
                    Servicos.Log.TratarErro(ex);
                    retornoIntegracao.Importados = 0;
                    retornoIntegracao.Sucesso = false;
                    retornoIntegracao.MensagemAviso = "Erro na leitura do EDI e processamento da DT";

                }
            }
            else if (integracaoProcessamentoPendente.Cliente != null && integracaoProcessamentoPendente.Cliente.TipoIntegracao.Tipo == Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoIntegracao.Natura)
            {
                try
                {
                    Servicos.LeituraEDI serLeituraEDI = new Servicos.LeituraEDI(null, integracaoProcessamentoPendente.LayoutEDI, Arquivo, 0, 0, 0, 0, 0, 0, 0, 0, true, true, Encoding.GetEncoding("iso-8859-1"));
                    NOTFIS = serLeituraEDI.GerarNotasFis();
                    bool sucesso = Natura.IntegracaoDTNatura.GerarDT(out string erro, out List<Dominio.Entidades.Embarcador.Integracao.DTNatura> dtsNatura, null, null, null, NOTFIS, unitOfWork, adminUnitofWork, unitOfWork.StringConexao, adminUnitofWork.StringConexao, auditado, tipoServicoMultisoftware);

                    retornoIntegracao.Importados = dtsNatura != null ? dtsNatura.Count : 0;
                    retornoIntegracao.Sucesso = sucesso;
                    retornoIntegracao.MensagemAviso = erro;
                }
                catch (Exception ex)
                {
                    Servicos.Log.TratarErro(ex);
                    retornoIntegracao.Importados = 0;
                    retornoIntegracao.Sucesso = false;
                    retornoIntegracao.MensagemAviso = "Erro na leitura do EDI e processamento da DT";

                }
            }
            else
            {
                //integracoes gerais
                Servicos.Embarcador.Pedido.Pedido svcPedido = new Servicos.Embarcador.Pedido.Pedido(unitOfWork);
                string retorno = "";
                bool notasProcessadas = svcPedido.ImportarNotasFiscaisNOTFIS(integracaoProcessamentoPendente.LayoutEDI, Arquivo, null, tipoServicoMultisoftware, auditado, unitOfWork, out retorno, adminUnitofWork);

                retornoIntegracao.Sucesso = notasProcessadas;
                retornoIntegracao.MensagemAviso = retorno;
            }

            return retornoIntegracao;
        }


        public dynamic SalvarArquivoImportacaoTemporario(System.IO.Stream arquivo, string NomeArquivo, Repositorio.UnitOfWork unitOfWork)
        {
            string guidArquivo = Guid.NewGuid().ToString().Replace("-", "");
            string nomeArquivo = NomeArquivo;
            string pasta = Utilidades.IO.FileStorageService.Storage.Combine("ImportacaoFTP", "Notfis");
            string caminho = Utilidades.IO.FileStorageService.Storage.Combine(Configuracoes.ConfigurationInstance.GetInstance(unitOfWork).ObterConfiguracaoArquivo().CaminhoArquivos, pasta);

            string path = Utilidades.IO.FileStorageService.Storage.Combine(caminho, guidArquivo);
            using (Stream outputFileStream = Utilidades.IO.FileStorageService.Storage.OpenWrite(path))
            {
                arquivo.CopyTo(outputFileStream);
            }

            dynamic obj = new ExpandoObject();
            obj.Token = guidArquivo;
            obj.NomeOriginal = nomeArquivo;

            return obj;
        }

        public string ObterCaminhoCompletoArquivo(string GuidArquivo, Repositorio.UnitOfWork unitOfWork)
        {
            string pasta = Utilidades.IO.FileStorageService.Storage.Combine("ImportacaoFTP", "Notfis");
            string caminho = Utilidades.IO.FileStorageService.Storage.Combine(Configuracoes.ConfigurationInstance.GetInstance(unitOfWork).ObterConfiguracaoArquivo().CaminhoArquivos, pasta);

            return Utilidades.IO.FileStorageService.Storage.Combine(caminho, GuidArquivo);
        }

        #endregion

    }
}
