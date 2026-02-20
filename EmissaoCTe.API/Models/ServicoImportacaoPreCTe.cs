using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace EmissaoCTe.API
{
    public class ServicoImportacaoPreCTe
    {
        private static ServicoImportacaoPreCTe Instance;

        public static ServicoImportacaoPreCTe GetInstance()
        {
            if (Instance == null)
                Instance = new ServicoImportacaoPreCTe();

            return Instance;
        }

        public void Start(string stringConexao)
        {
            string caminhoPreCTes = System.Configuration.ConfigurationManager.AppSettings["CaminhoPreCTes"];

            if (!string.IsNullOrWhiteSpace(caminhoPreCTes))
            {

                Task task = new Task(() =>
                {

                    while (true)
                    {
                        try
                        {
                            IEnumerable<string> arquivos = Utilidades.IO.FileStorageService.Storage.GetFiles(caminhoPreCTes, "*.xml", SearchOption.TopDirectoryOnly);

                            if (arquivos.Count() > 0)
                            {
                                using (Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(stringConexao, Dominio.ObjetosDeValor.Enumerador.TipoSessaoBancoDados.Nova))
                                {
                                    Servicos.CTe svcCTe = new Servicos.CTe(unitOfWork);

                                    Repositorio.ConhecimentoDeTransporteEletronico repCTe = new Repositorio.ConhecimentoDeTransporteEletronico(unitOfWork);

                                    foreach (string arquivo in arquivos)
                                    {
                                        try
                                        {
                                            bool falha = true;

                                            using Stream streamArquivo = Utilidades.IO.FileStorageService.Storage.OpenRead(arquivo);

                                            var file = MultiSoftware.CTe.Servicos.Leitura.Ler(streamArquivo);

                                            if (file != null && file.GetType() == typeof(MultiSoftware.CTe.v104.ConhecimentoDeTransporteCancelado.TCancCTe))
                                            {
                                                MultiSoftware.CTe.v104.ConhecimentoDeTransporteCancelado.TCancCTe cancelamento = (MultiSoftware.CTe.v104.ConhecimentoDeTransporteCancelado.TCancCTe)file;

                                                Dominio.Entidades.ConhecimentoDeTransporteEletronico cte = repCTe.BuscarPorChave(cancelamento.infCanc.chCTe);

                                                if (cte != null && cte.Status == "A")
                                                {
                                                    if (Servicos.Embarcador.EmissorDocumento.EmissorDocumentoService.GetEmissorDocumentoCTe(cte.SistemaEmissor).CancelarCte(cte.Codigo, cte.Empresa.Codigo, cancelamento.infCanc.xJust, unitOfWork))
                                                    {
                                                        falha = false;

                                                        if (cte.SistemaEmissor == Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoEmissorDocumento.Integrador)
                                                            FilaConsultaCTe.GetInstance().QueueItem(4, cte.Codigo, Dominio.Enumeradores.TipoObjetoConsulta.CTe, stringConexao);

                                                        this.MoverArquivoParaPastaDeImportados(caminhoPreCTes, arquivo);
                                                    }
                                                }
                                            }
                                            else
                                            {

                                                Dominio.Entidades.ConhecimentoDeTransporteEletronico cte = svcCTe.GerarCTePorPreCTe(Utilidades.IO.FileStorageService.Storage.OpenRead(arquivo));

                                                if (cte != null)
                                                {
                                                    falha = false;

                                                    cte = repCTe.BuscarPorId(cte.Codigo, cte.Empresa.Codigo);

                                                    cte.ImportacaoPreCTe = true;
                                                    cte.StatusImportacaoPreCTe = Dominio.Enumeradores.StatusImportacaoPreCTe.Pendente;
                                                    cte.InformacaoAdicionalFisco = cte.ObservacoesGerais;

                                                    repCTe.Atualizar(cte);

                                                    this.MoverArquivoParaPastaDeImportados(caminhoPreCTes, arquivo);
                                                }
                                            }

                                            if (falha)
                                                this.MoverArquivoParaPastaDeErro(caminhoPreCTes, arquivo);

                                        }
                                        catch (Exception ex)
                                        {
                                            Servicos.Log.TratarErro("Ocorreu uma falha ao ler um pré CT-e " + arquivo + ". Falha: " + ex.ToString());

                                            this.MoverArquivoParaPastaDeErro(caminhoPreCTes, arquivo);
                                        }
                                    }

                                    svcCTe = null;
                                    repCTe = null;
                                }
                            }

                            arquivos = null;
                        }
                        catch (Exception ex)
                        {
                            Servicos.Log.TratarErro(ex);
                        }

                        System.Threading.Thread.Sleep(5000);
                    }

                });

                task.Start();
            }
        }

        private void MoverArquivoParaPastaDeErro(string pathFolder, string pathFile)
        {
            string path = Utilidades.IO.FileStorageService.Storage.Combine(pathFolder, "Falhas", DateTime.Now.Date.ToString("dd_MM_yyyy"), Path.GetFileName(pathFile));

            Servicos.Arquivo.Mover(pathFile, path);
        }

        private void MoverArquivoParaPastaDeImportados(string pathFolder, string pathFile)
        {
            string path = Utilidades.IO.FileStorageService.Storage.Combine(pathFolder, "Importados", DateTime.Now.Date.ToString("dd_MM_yyyy"), Path.GetFileName(pathFile));

            Servicos.Arquivo.Mover(pathFile, path);
        }
    }
}
