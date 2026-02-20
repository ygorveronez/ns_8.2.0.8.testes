using SGT.BackgroundWorkers.Utils;
using System.Threading;
using System.Threading.Tasks;

namespace SGT.BackgroundWorkers
{
    [RunningConfig(DuracaoPadrao = 100000)]

    public class ProcessamentoArquivoXMLNotaFiscalIntegracao : LongRunningProcessBase<ProcessamentoArquivoXMLNotaFiscalIntegracao>
    {
        #region Métodos protegidos

        protected override async Task ExecuteInternalAsync(Repositorio.UnitOfWork unitOfWork, AdminMultisoftware.Repositorio.UnitOfWork unitOfWorkAdmin, CancellationToken cancellationToken)
        {
            ProcessarArquivos(unitOfWork);
        }

        #endregion

        #region Métodos privados

        private void ProcessarArquivos(Repositorio.UnitOfWork unitOfWork)
        {

            //DESATIVADO ESTA PROCESSANDO PELO SERVICO: SGT.WebAdmin.ProcessarArquivoXMLNotaFiscalIntegracao

            //Dominio.Entidades.Embarcador.Configuracoes.ConfiguracaoArquivo configuracaoArquivo = new Repositorio.Embarcador.Configuracoes.ConfiguracaoArquivo(unitOfWork).BuscarPrimeiroRegistro();
            //Dominio.Entidades.Embarcador.Configuracao.ConfiguracaoTMS configuracaoTMS = new Repositorio.Embarcador.Configuracao.ConfiguracaoTMS(unitOfWork).BuscarConfiguracaoPadrao();
            //Repositorio.Embarcador.NotaFiscal.ArquivoXMLNotaFiscalIntegracao repArquivos = new Repositorio.Embarcador.NotaFiscal.ArquivoXMLNotaFiscalIntegracao(unitOfWork);
            //Repositorio.Embarcador.Pedidos.XMLNotaFiscal repositorioNotaFiscal = new Repositorio.Embarcador.Pedidos.XMLNotaFiscal(unitOfWork);
            //Servicos.Embarcador.NFe.NFe servicoNfe = new Servicos.Embarcador.NFe.NFe(unitOfWork);
            //Servicos.Embarcador.Pedido.NotaFiscal serCargaNotaFiscal = new Servicos.Embarcador.Pedido.NotaFiscal(unitOfWork);

            //if (configuracaoArquivo == null)
            //    return;

            //List<Dominio.Entidades.Embarcador.NotaFiscal.ArquivoXMLNotaFiscalIntegracao> arquivos = repArquivos.BuscarPendentesLiberacao(3, 100);

            //for (int i = 0; i < arquivos.Count; i++)
            //{
            //    Dominio.Entidades.Embarcador.NotaFiscal.ArquivoXMLNotaFiscalIntegracao arquivo = arquivos[i];
            //    unitOfWork.Start();

            //    arquivo.Initialize();
            //    arquivo.Tentativas++;

            //    string mensagemErro = string.Empty;

            //    Dominio.ObjetosDeValor.Embarcador.Auditoria.Auditado auditado = new Dominio.ObjetosDeValor.Embarcador.Auditoria.Auditado()
            //    {
            //        TipoAuditado = Dominio.ObjetosDeValor.Enumerador.TipoAuditado.Integradoras,
            //        Integradora = arquivo.Integradora,
            //        IP = arquivo.IP
            //    };

            //    Servicos.Log.TratarErro($"Lendo arquivo {arquivo.Chave} {DateTime.Now.ToString("dd/MM/yyyy HH:mm:sss")}", "ProcessamentoArquivoXMLNotaFiscalIntegracao");

            //    try
            //    {
            //        //System.IO.StreamReader stReaderXML = new StreamReader(configuracaoArquivo.CaminhoArquivosImportacaoXMLNotaFiscal + "\\" + arquivo.NomeArquivo);
            //        System.IO.StreamReader stReaderXML = new StreamReader("C:\\Arquivos" + "\\" + arquivo.NomeArquivo);

                   // if (!servicoNfe.BuscarDadosNotaFiscal(out error, out xmlNotaFiscal, stReaderXML, unitOfWork, null, true, false, true))
                     //   mensagemErro = error.Substring(0, Math.Min(error.Length, 500));

            //        Servicos.Log.TratarErro($"Leu arquivo {arquivo.Chave} {DateTime.Now.ToString("dd/MM/yyyy HH:mm:sss")}", "ProcessamentoArquivoXMLNotaFiscalIntegracao");

            //        if (!servicoNfe.BuscarDadosNotaFiscal(out error, out xmlNotaFiscal, stReaderXML, unitOfWork, null, true, false, true))
            //            mensagemErro = error.Substring(0, Math.Min(error.Length, 500));

            //        if (string.IsNullOrEmpty(mensagemErro))
            //        {
            //            if (xmlNotaFiscal == null)
            //                mensagemErro = "Nota Fiscal Inexistente";

            //            if (string.IsNullOrEmpty(mensagemErro))
            //            {
            //                if (xmlNotaFiscal.Codigo > 0)
            //                    repositorioNotaFiscal.Atualizar(xmlNotaFiscal);
            //                else
            //                    repositorioNotaFiscal.Inserir(xmlNotaFiscal);

            //                serCargaNotaFiscal.VincularXMLNotaFiscal(xmlNotaFiscal, configuracaoTMS, _tipoServicoMultisoftware, auditado, false, true);

            //                Servicos.Log.TratarErro($"VincularXMLNotaFiscal {arquivo.Chave} {DateTime.Now.ToString("dd/MM/yyyy HH:mm:sss")}", "ProcessamentoArquivoXMLNotaFiscalIntegracao");

            //                new Servicos.Embarcador.Pedido.PedidoXMLNotaFiscal(unitOfWork).ArmazenarProdutosXML(xmlNota, xmlNotaFiscal, auditado, _tipoServicoMultisoftware, null);

            //                Servicos.Log.TratarErro($"ArmazenarProdutosXML {arquivo.Chave} {DateTime.Now.ToString("dd/MM/yyyy HH:mm:sss")}", "ProcessamentoArquivoXMLNotaFiscalIntegracao");
            //            }
            //        }

            //        if (arquivo.Tentativas >= 3 && !string.IsNullOrEmpty(mensagemErro))
            //            arquivo.Situacao = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoProcessamentoRegistro.Falha;

            //        if (string.IsNullOrEmpty(mensagemErro))
            //            arquivo.Situacao = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoProcessamentoRegistro.Sucesso;

            //        if (arquivo.Situacao == Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoProcessamentoRegistro.Sucesso)
            //        {
            //            string caminhoArquivo = configuracaoArquivo.CaminhoArquivosImportacaoXMLNotaFiscal + "\\" + arquivo.NomeArquivo;
            //            if (System.IO.File.Exists(caminhoArquivo))
            //                System.IO.File.Delete(caminhoArquivo);
            //        }

            //        stReaderXML.Close();

            //        Servicos.Log.TratarErro($"Fechou arquivo {arquivo.Chave} {DateTime.Now.ToString("dd/MM/yyyy HH:mm:sss")}", "ProcessamentoArquivoXMLNotaFiscalIntegracao");

            //        arquivo.Mensagem = mensagemErro;

            //        repArquivos.Atualizar(arquivo, auditado);

            //        Servicos.Log.TratarErro($"Arquivo Processado {xmlNotaFiscal.Chave} {DateTime.Now.ToString("dd/MM/yyyy HH:mm:sss")}", "ProcessamentoArquivoXMLNotaFiscalIntegracao");

            //        unitOfWork.CommitChanges();
            //        unitOfWork.Flush();
            //    }
            //    catch (System.Exception ex)
            //    {
            //        unitOfWork.Rollback();

            //        unitOfWork.Start();

            //        Servicos.Log.TratarErro($"Falha Arquivo Ex: {ex.ToString()} {DateTime.Now.ToString("dd/MM/yyyy HH:mm:sss")}", "ProcessamentoArquivoXMLNotaFiscalIntegracao");

            //        arquivo.Tentativas++;
            //        mensagemErro = ex.Message.Substring(0, Math.Min(ex.Message.Length, 500));
            //        if (arquivo.Tentativas >= 3)
            //            arquivo.Situacao = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoProcessamentoRegistro.Falha;
            //        arquivo.Mensagem = mensagemErro;

            //        repArquivos.Atualizar(arquivo, auditado);

            //        unitOfWork.CommitChanges();
            //        unitOfWork.Flush();
            //    }
            //}
        }

        #endregion
    }
}