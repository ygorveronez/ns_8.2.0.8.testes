using System;
using System.Collections.Concurrent;
using System.Configuration;
using System.Threading.Tasks;

namespace EmissaoCTe.API
{
    public class FilaConsultaCTe
    {
        private ConcurrentDictionary<int, Task> ListaTasks;
        private ConcurrentDictionary<int, ConcurrentQueue<Dominio.ObjetosDeValor.ObjetoConsulta>> ListaCTeConsulta;
        private static FilaConsultaCTe Instance;

        public static FilaConsultaCTe GetInstance()
        {
            if (Instance == null)
                Instance = new FilaConsultaCTe();

            return Instance;
        }

        public static FilaConsultaCTe GetNewInstance()
        {
            if (Instance != null)
                Instance = null;

            return null;
        }

        public void LimparListas()
        {
            if (ListaTasks != null)
                ListaTasks = null;

            if (ListaCTeConsulta != null)
                ListaCTeConsulta = null;
        }

        public void QueueItem(int idFila, int idObjetoConsulta, Dominio.Enumeradores.TipoObjetoConsulta tipo, string stringConexao)
        {
            if (ListaTasks == null)
                ListaTasks = new ConcurrentDictionary<int, Task>();

            if (ListaCTeConsulta == null)
                ListaCTeConsulta = new ConcurrentDictionary<int, ConcurrentQueue<Dominio.ObjetosDeValor.ObjetoConsulta>>();

            if (!ListaTasks.ContainsKey(idFila))
            {
                this.IniciarThread(idObjetoConsulta, idFila, tipo, stringConexao);
            }
            else
            {
                ConcurrentQueue<Dominio.ObjetosDeValor.ObjetoConsulta> lista = null;

                if (ListaCTeConsulta.TryGetValue(idFila, out lista))
                {
                    lista.Enqueue(new Dominio.ObjetosDeValor.ObjetoConsulta(idObjetoConsulta, tipo));
                }
                else
                {
                    Servicos.Log.TratarErro("Não foi possível obter a lista com os objetos em consulta.");
                }
            }
        }

        private void IniciarThread(int idObjetoConsulta, int idFila, Dominio.Enumeradores.TipoObjetoConsulta tipo, string stringConexao)
        {
            string configAdicionarCTesFilaConsulta = ConfigurationManager.AppSettings["AdicionarCTesFilaConsulta"];
            if (configAdicionarCTesFilaConsulta == null || configAdicionarCTesFilaConsulta == "")
                configAdicionarCTesFilaConsulta = "SIM";

            if (configAdicionarCTesFilaConsulta != "SIM")
                return;

            var filaConsulta = new ConcurrentQueue<Dominio.ObjetosDeValor.ObjetoConsulta>();

            filaConsulta.Enqueue(new Dominio.ObjetosDeValor.ObjetoConsulta(idObjetoConsulta, tipo));

            if (ListaCTeConsulta.TryAdd(idFila, filaConsulta))
            {
                Task task = new Task(() =>
                {
                    Dominio.ObjetosDeValor.ObjetoConsulta objetoConsulta = null;

                    if (!filaConsulta.TryDequeue(out objetoConsulta))
                        Servicos.Log.TratarErro("Falha ao obter o primeiro item da fila de objetos para consulta.");

                    Servicos.CTe servicoCTe = null;
                    Servicos.CCe servicoCCe = null;
                    Servicos.MDFe servicoMDFe = null;
                    Servicos.NFSe servicoNFSe = null;
                    Servicos.Averbacao servicoAverbacao = null;
                    Servicos.AverbacaoMDFe servicoAverbacaoMDFe = null;

                    Repositorio.ConhecimentoDeTransporteEletronico repCTe = null;
                    Repositorio.CartaDeCorrecaoEletronica repCCe = null;
                    Repositorio.ManifestoEletronicoDeDocumentosFiscais repMDFe = null;
                    Repositorio.NFSe repNFSe = null;

                    Dominio.Entidades.ConhecimentoDeTransporteEletronico cte = null;
                    Dominio.Entidades.CartaDeCorrecaoEletronica cce = null;
                    Dominio.Entidades.ManifestoEletronicoDeDocumentosFiscais mdfe = null;
                    Dominio.Entidades.NFSe nfse = null;

                    bool sleep = false;

                    while (true)
                    {
                        using (Repositorio.UnitOfWork unidadeDeTrabalho = new Repositorio.UnitOfWork(stringConexao, Dominio.ObjetosDeValor.Enumerador.TipoSessaoBancoDados.Nova))
                        {
                            try
                            {
                                if (objetoConsulta != null)
                                {
                                    if (objetoConsulta.Tipo == Dominio.Enumeradores.TipoObjetoConsulta.CTe)
                                    {
                                        Servicos.Log.GravarDebug($"IniciarThread-CTe {objetoConsulta.Codigo}", "FilaConsultaCTe");
                                        repCTe = new Repositorio.ConhecimentoDeTransporteEletronico(unidadeDeTrabalho);

                                        cte = repCTe.BuscarPorCodigo(objetoConsulta.Codigo);

                                        if (cte != null && (cte.ModeloDocumentoFiscal.Numero == "67" || cte.ModeloDocumentoFiscal.Numero == "57"))
                                        {
                                            servicoCTe = new Servicos.CTe(unidadeDeTrabalho);
                                            if (cte.Status.Equals("E") || cte.Status.Equals("X"))  //Enviado ou Aguardando assinatura
                                            {
                                                servicoCTe.Consultar(ref cte, unidadeDeTrabalho);

                                                if (cte.Status == "E" || cte.Status.Equals("X"))
                                                {
                                                    filaConsulta.Enqueue(new Dominio.ObjetosDeValor.ObjetoConsulta(cte.Codigo, Dominio.Enumeradores.TipoObjetoConsulta.CTe));
                                                    sleep = true;

                                                    //if (cte.Status == "E" && cte.DataIntegracao < DateTime.Now.AddMinutes(-25))
                                                    //    servicoCTe.NotificarCTeEnviado(cte, 0, unidadeDeTrabalho);
                                                }
                                                else if (cte.Status == "A")
                                                {
                                                    bool averbaCTe = (cte.Empresa.Configuracao != null && cte.Empresa.Configuracao.AverbaAutomaticoATM == 1) || (cte.Empresa.Configuracao != null && cte.Empresa.EmpresaPai != null && cte.Empresa.EmpresaPai.Configuracao != null && cte.Empresa.EmpresaPai.Configuracao.AverbaAutomaticoATM == 1);

                                                    if (averbaCTe && cte.Empresa.Configuracao != null)
                                                    {
                                                        if (cte.Empresa.Configuracao.TipoCTeAverbacao == Dominio.Enumeradores.TipoCTEAverbacao.Todos)
                                                            averbaCTe = true;
                                                        else if (cte.TipoServico == Dominio.Enumeradores.TipoServico.Normal)
                                                            averbaCTe = cte.Empresa.Configuracao.TipoCTeAverbacao == Dominio.Enumeradores.TipoCTEAverbacao.ApenasNormal;
                                                        else if (cte.TipoServico == Dominio.Enumeradores.TipoServico.SubContratacao)
                                                            averbaCTe = cte.Empresa.Configuracao.TipoCTeAverbacao == Dominio.Enumeradores.TipoCTEAverbacao.ApenasSubcontratacao;
                                                        else if (cte.TipoServico == Dominio.Enumeradores.TipoServico.Redespacho)
                                                            averbaCTe = cte.Empresa.Configuracao.TipoCTeAverbacao == Dominio.Enumeradores.TipoCTEAverbacao.ApenasRedespacho;
                                                        else if (cte.TipoServico == Dominio.Enumeradores.TipoServico.RedIntermediario)
                                                            averbaCTe = cte.Empresa.Configuracao.TipoCTeAverbacao == Dominio.Enumeradores.TipoCTEAverbacao.ApenasRedIntermediario;
                                                        else if (cte.TipoServico == Dominio.Enumeradores.TipoServico.ServVinculadoMultimodal)
                                                            averbaCTe = cte.Empresa.Configuracao.TipoCTeAverbacao == Dominio.Enumeradores.TipoCTEAverbacao.ApenasServVinculadoMultimodal;
                                                        else if (cte.TipoServico == Dominio.Enumeradores.TipoServico.TransporteDePessoas)
                                                            averbaCTe = cte.Empresa.Configuracao.TipoCTeAverbacao == Dominio.Enumeradores.TipoCTEAverbacao.ApenasTransporteDePessoas;
                                                        else if (cte.TipoServico == Dominio.Enumeradores.TipoServico.TransporteDeValores)
                                                            averbaCTe = cte.Empresa.Configuracao.TipoCTeAverbacao == Dominio.Enumeradores.TipoCTEAverbacao.ApenasTransporteDeValores;
                                                        else if (cte.TipoServico == Dominio.Enumeradores.TipoServico.ExcessoDeBagagem)
                                                            averbaCTe = cte.Empresa.Configuracao.TipoCTeAverbacao == Dominio.Enumeradores.TipoCTEAverbacao.ApenasExcessoDeBagagem;
                                                    }

                                                    if (averbaCTe && cte.TipoCTE == Dominio.Enumeradores.TipoCTE.Normal)
                                                    {
                                                        Servicos.Averbacao svcAverbacao = new Servicos.Averbacao(unidadeDeTrabalho);
                                                        if (svcAverbacao.VerificaAverbacao(cte.Codigo, Dominio.Enumeradores.TipoAverbacaoCTe.Autorizacao, unidadeDeTrabalho))
                                                            filaConsulta.Enqueue(new Dominio.ObjetosDeValor.ObjetoConsulta(cte.Codigo, Dominio.Enumeradores.TipoObjetoConsulta.Averbacao));
                                                    }

                                                    Servicos.LsTranslog svcLsTranslog = new Servicos.LsTranslog(unidadeDeTrabalho);
                                                    svcLsTranslog.SalvarCTeParaIntegracao(cte.Codigo, cte.Empresa.Codigo, unidadeDeTrabalho);

                                                    Servicos.CIOT svcCIOT = new Servicos.CIOT(unidadeDeTrabalho);
                                                    svcCIOT.VincularCTeCIOTEFrete(cte.Codigo, unidadeDeTrabalho);
                                                }
                                                else if (cte.Status == "R")
                                                {
                                                    if (System.Configuration.ConfigurationManager.AppSettings["ReenviarRejeicaoCTe"] == "SIM" && cte.TentativaReenvio <= 1)
                                                    {
                                                        if (cte.MensagemStatus != null && (
                                                            cte.MensagemStatus.CodigoDoErro == 8888 || //Falha de conexão com o Sefaz (Retorno Integrador)
                                                            cte.MensagemStatus.CodigoDoErro == 678 ||  //Uso indevido (Retorno Sefaz)
                                                            cte.MensagemStatus.CodigoDoErro == 109 ||  //Serviço paralisado (Retorno Sefaz)
                                                            cte.MensagemStatus.CodigoDoErro == 105))  //Lote em processamento (Retorno Sefaz)
                                                        {
                                                            Servicos.Log.TratarErro("CT-e codigo " + cte.Codigo.ToString() + ": Reenviado rejeição:" + cte.MensagemStatus.CodigoDoErro.ToString(), "ReenvioCTe");

                                                            cte.TentativaReenvio = cte.TentativaReenvio + 1;
                                                            bool reenvioCTe = servicoCTe.Emitir(ref cte, unidadeDeTrabalho);

                                                            if (reenvioCTe)
                                                                servicoCTe.AdicionarCTeNaFilaDeConsulta(cte, unidadeDeTrabalho);

                                                            int.TryParse(System.Configuration.ConfigurationManager.AppSettings["TentativasReenvio"], out int tentativasReenvio);
                                                            string notificarEmail = System.Configuration.ConfigurationManager.AppSettings["NotificarEmailFalhaSefaz"];
                                                            if (notificarEmail == "SIM" && tentativasReenvio == cte.TentativaReenvio)
                                                                EnviarEmail("Alerta emissão CT-e", "CT-e", "Emissor " + cte.Empresa.Descricao + " Número " + cte.Numero.ToString() + ": " + cte.MensagemStatus?.MensagemDoErro ?? cte.MensagemRetornoSefaz);
                                                        }
                                                    }
                                                }
                                            }
                                            else if (cte.Status.Equals("K") || cte.Status.Equals("V"))//CANCELAMENTO
                                            {
                                                cte = servicoCTe.ConsultarCancelamento(objetoConsulta.Codigo, unidadeDeTrabalho);

                                                if (cte.Status.Equals("K") || cte.Status.Equals("V"))
                                                {
                                                    filaConsulta.Enqueue(new Dominio.ObjetosDeValor.ObjetoConsulta(cte.Codigo, Dominio.Enumeradores.TipoObjetoConsulta.CTe));
                                                    sleep = true;
                                                }
                                                else if (cte.Status == "C")
                                                {
                                                    bool averbaCTe = (cte.Empresa.Configuracao != null && cte.Empresa.Configuracao.AverbaAutomaticoATM == 1) || (cte.Empresa.Configuracao != null && cte.Empresa.EmpresaPai != null && cte.Empresa.EmpresaPai.Configuracao != null && cte.Empresa.EmpresaPai.Configuracao.AverbaAutomaticoATM == 1);

                                                    if (averbaCTe && cte.Empresa.Configuracao != null)
                                                    {
                                                        if (cte.Empresa.Configuracao.TipoCTeAverbacao == Dominio.Enumeradores.TipoCTEAverbacao.Todos)
                                                            averbaCTe = true;
                                                        else if (cte.TipoServico == Dominio.Enumeradores.TipoServico.Normal)
                                                            averbaCTe = cte.Empresa.Configuracao.TipoCTeAverbacao == Dominio.Enumeradores.TipoCTEAverbacao.ApenasNormal;
                                                        else if (cte.TipoServico == Dominio.Enumeradores.TipoServico.SubContratacao)
                                                            averbaCTe = cte.Empresa.Configuracao.TipoCTeAverbacao == Dominio.Enumeradores.TipoCTEAverbacao.ApenasSubcontratacao;
                                                        else if (cte.TipoServico == Dominio.Enumeradores.TipoServico.Redespacho)
                                                            averbaCTe = cte.Empresa.Configuracao.TipoCTeAverbacao == Dominio.Enumeradores.TipoCTEAverbacao.ApenasRedespacho;
                                                        else if (cte.TipoServico == Dominio.Enumeradores.TipoServico.RedIntermediario)
                                                            averbaCTe = cte.Empresa.Configuracao.TipoCTeAverbacao == Dominio.Enumeradores.TipoCTEAverbacao.ApenasRedIntermediario;
                                                        else if (cte.TipoServico == Dominio.Enumeradores.TipoServico.ServVinculadoMultimodal)
                                                            averbaCTe = cte.Empresa.Configuracao.TipoCTeAverbacao == Dominio.Enumeradores.TipoCTEAverbacao.ApenasServVinculadoMultimodal;
                                                        else if (cte.TipoServico == Dominio.Enumeradores.TipoServico.TransporteDePessoas)
                                                            averbaCTe = cte.Empresa.Configuracao.TipoCTeAverbacao == Dominio.Enumeradores.TipoCTEAverbacao.ApenasTransporteDePessoas;
                                                        else if (cte.TipoServico == Dominio.Enumeradores.TipoServico.TransporteDeValores)
                                                            averbaCTe = cte.Empresa.Configuracao.TipoCTeAverbacao == Dominio.Enumeradores.TipoCTEAverbacao.ApenasTransporteDeValores;
                                                        else if (cte.TipoServico == Dominio.Enumeradores.TipoServico.ExcessoDeBagagem)
                                                            averbaCTe = cte.Empresa.Configuracao.TipoCTeAverbacao == Dominio.Enumeradores.TipoCTEAverbacao.ApenasExcessoDeBagagem;
                                                    }

                                                    if (averbaCTe && cte.TipoCTE == Dominio.Enumeradores.TipoCTE.Normal)
                                                    {
                                                        Servicos.Averbacao svcAverbacao = new Servicos.Averbacao(unidadeDeTrabalho);
                                                        if (svcAverbacao.VerificaAverbacao(cte.Codigo, Dominio.Enumeradores.TipoAverbacaoCTe.Cancelamento, unidadeDeTrabalho))
                                                            filaConsulta.Enqueue(new Dominio.ObjetosDeValor.ObjetoConsulta(cte.Codigo, Dominio.Enumeradores.TipoObjetoConsulta.Averbacao));
                                                    }

                                                    Servicos.CIOT svcCIOT = new Servicos.CIOT(unidadeDeTrabalho);
                                                    svcCIOT.DesvincularCTeCIOTEFrete(cte.Codigo, unidadeDeTrabalho);
                                                }
                                            }
                                            else if (cte.Status.Equals("L") || cte.Status.Equals("B")) //INUTILIZAÇÃO
                                            {
                                                cte = servicoCTe.ConsultarInutilizacao(objetoConsulta.Codigo, unidadeDeTrabalho);

                                                if (cte.Status.Equals("L") || cte.Status.Equals("B"))
                                                {
                                                    filaConsulta.Enqueue(new Dominio.ObjetosDeValor.ObjetoConsulta(cte.Codigo, Dominio.Enumeradores.TipoObjetoConsulta.CTe));
                                                    sleep = true;
                                                }
                                            }
                                            else if (cte.Status.Equals("A")) //AUTORIZADO
                                            {
                                                bool averbaCTe = (cte.Empresa.Configuracao != null && cte.Empresa.Configuracao.AverbaAutomaticoATM == 1) || (cte.Empresa.Configuracao != null && cte.Empresa.EmpresaPai != null && cte.Empresa.EmpresaPai.Configuracao != null && cte.Empresa.EmpresaPai.Configuracao.AverbaAutomaticoATM == 1);

                                                if (averbaCTe && cte.Empresa.Configuracao != null)
                                                {
                                                    if (cte.Empresa.Configuracao.TipoCTeAverbacao == Dominio.Enumeradores.TipoCTEAverbacao.Todos)
                                                        averbaCTe = true;
                                                    else if (cte.TipoServico == Dominio.Enumeradores.TipoServico.Normal)
                                                        averbaCTe = cte.Empresa.Configuracao.TipoCTeAverbacao == Dominio.Enumeradores.TipoCTEAverbacao.ApenasNormal;
                                                    else if (cte.TipoServico == Dominio.Enumeradores.TipoServico.SubContratacao)
                                                        averbaCTe = cte.Empresa.Configuracao.TipoCTeAverbacao == Dominio.Enumeradores.TipoCTEAverbacao.ApenasSubcontratacao;
                                                    else if (cte.TipoServico == Dominio.Enumeradores.TipoServico.Redespacho)
                                                        averbaCTe = cte.Empresa.Configuracao.TipoCTeAverbacao == Dominio.Enumeradores.TipoCTEAverbacao.ApenasRedespacho;
                                                    else if (cte.TipoServico == Dominio.Enumeradores.TipoServico.RedIntermediario)
                                                        averbaCTe = cte.Empresa.Configuracao.TipoCTeAverbacao == Dominio.Enumeradores.TipoCTEAverbacao.ApenasRedIntermediario;
                                                    else if (cte.TipoServico == Dominio.Enumeradores.TipoServico.ServVinculadoMultimodal)
                                                        averbaCTe = cte.Empresa.Configuracao.TipoCTeAverbacao == Dominio.Enumeradores.TipoCTEAverbacao.ApenasServVinculadoMultimodal;
                                                    else if (cte.TipoServico == Dominio.Enumeradores.TipoServico.TransporteDePessoas)
                                                        averbaCTe = cte.Empresa.Configuracao.TipoCTeAverbacao == Dominio.Enumeradores.TipoCTEAverbacao.ApenasTransporteDePessoas;
                                                    else if (cte.TipoServico == Dominio.Enumeradores.TipoServico.TransporteDeValores)
                                                        averbaCTe = cte.Empresa.Configuracao.TipoCTeAverbacao == Dominio.Enumeradores.TipoCTEAverbacao.ApenasTransporteDeValores;
                                                    else if (cte.TipoServico == Dominio.Enumeradores.TipoServico.ExcessoDeBagagem)
                                                        averbaCTe = cte.Empresa.Configuracao.TipoCTeAverbacao == Dominio.Enumeradores.TipoCTEAverbacao.ApenasExcessoDeBagagem;
                                                }

                                                if (averbaCTe && cte.TipoCTE == Dominio.Enumeradores.TipoCTE.Normal)
                                                {
                                                    Servicos.Averbacao svcAverbacao = new Servicos.Averbacao(unidadeDeTrabalho);
                                                    if (svcAverbacao.VerificaAverbacao(cte.Codigo, Dominio.Enumeradores.TipoAverbacaoCTe.Autorizacao, unidadeDeTrabalho))
                                                        filaConsulta.Enqueue(new Dominio.ObjetosDeValor.ObjetoConsulta(cte.Codigo, Dominio.Enumeradores.TipoObjetoConsulta.Averbacao));
                                                }

                                                Servicos.LsTranslog svcLsTranslog = new Servicos.LsTranslog(unidadeDeTrabalho);
                                                svcLsTranslog.SalvarCTeParaIntegracao(cte.Codigo, cte.Empresa.Codigo, unidadeDeTrabalho);

                                                Servicos.CIOT svcCIOT = new Servicos.CIOT(unidadeDeTrabalho);
                                                svcCIOT.VincularCTeCIOTEFrete(cte.Codigo, unidadeDeTrabalho);
                                            }
                                        }

                                        objetoConsulta = null;
                                        cte = null;
                                        servicoCTe = null;
                                        repCTe = null;
                                    }
                                    else if (objetoConsulta.Tipo == Dominio.Enumeradores.TipoObjetoConsulta.CCe)
                                    {
                                        Servicos.Log.GravarDebug($"IniciarThread-CCe {objetoConsulta.Codigo}", "FilaConsultaCTe");
                                        repCCe = new Repositorio.CartaDeCorrecaoEletronica(unidadeDeTrabalho);

                                        cce = repCCe.BuscarPorCodigo(objetoConsulta.Codigo);

                                        if (cce != null)
                                        {
                                            servicoCCe = new Servicos.CCe(unidadeDeTrabalho);

                                            if (cce.Status == Dominio.Enumeradores.StatusCCe.Enviado)
                                            {
                                                cce = Servicos.Embarcador.EmissorDocumento.EmissorDocumentoService.GetEmissorDocumentoCTe(cce.SistemaEmissor).ConsultarCCe(cce.Codigo, unidadeDeTrabalho);

                                                if (cce.Status == Dominio.Enumeradores.StatusCCe.Enviado)
                                                {
                                                    filaConsulta.Enqueue(new Dominio.ObjetosDeValor.ObjetoConsulta(cce.Codigo, Dominio.Enumeradores.TipoObjetoConsulta.CCe));
                                                    sleep = true;
                                                }
                                            }
                                        }

                                        servicoCCe = null;
                                        objetoConsulta = null;
                                        repCCe = null;
                                        cce = null;
                                    }
                                    else if (objetoConsulta.Tipo == Dominio.Enumeradores.TipoObjetoConsulta.MDFe)
                                    {
                                        Servicos.Log.GravarDebug($"IniciarThread-MDFe {objetoConsulta.Codigo}", "FilaConsultaCTe");
                                        repMDFe = new Repositorio.ManifestoEletronicoDeDocumentosFiscais(unidadeDeTrabalho);

                                        mdfe = repMDFe.BuscarPorCodigo(objetoConsulta.Codigo);

                                        if (mdfe != null)
                                        {
                                            servicoMDFe = new Servicos.MDFe(unidadeDeTrabalho);
                                            bool averbaMDFe = (mdfe.Empresa.Configuracao != null && mdfe.Empresa.Configuracao.AverbaAutomaticoATM == 1 && mdfe.Empresa.Configuracao.AverbarMDFe) || (mdfe.Empresa.Configuracao != null && mdfe.Empresa.EmpresaPai != null && mdfe.Empresa.EmpresaPai.Configuracao != null && mdfe.Empresa.EmpresaPai.Configuracao.AverbaAutomaticoATM == 1 && mdfe.Empresa.EmpresaPai.Configuracao.AverbarMDFe);

                                            if (mdfe.Status == Dominio.Enumeradores.StatusMDFe.Enviado || mdfe.Status == Dominio.Enumeradores.StatusMDFe.Pendente || mdfe.Status == Dominio.Enumeradores.StatusMDFe.EmitidoContingencia) // Autorização
                                            {
                                                Servicos.Embarcador.EmissorDocumento.EmissorDocumentoService.GetEmissorDocumentoMDFe(mdfe.SistemaEmissor).ConsultarMdfe(mdfe, null, AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiCTe, unidadeDeTrabalho);

                                                if (mdfe.Status == Dominio.Enumeradores.StatusMDFe.Autorizado)
                                                {
                                                    if (averbaMDFe)
                                                    {
                                                        Servicos.AverbacaoMDFe svcAverbacao = new Servicos.AverbacaoMDFe(unidadeDeTrabalho);
                                                        if (svcAverbacao.Emitir(mdfe, Dominio.Enumeradores.TipoAverbacaoMDFe.Autorizacao, unidadeDeTrabalho))
                                                            filaConsulta.Enqueue(new Dominio.ObjetosDeValor.ObjetoConsulta(mdfe.Codigo, Dominio.Enumeradores.TipoObjetoConsulta.AverbacaoMDFe));
                                                    }
                                                }
                                                else if (mdfe.Status == Dominio.Enumeradores.StatusMDFe.Enviado || mdfe.Status == Dominio.Enumeradores.StatusMDFe.Pendente || mdfe.Status == Dominio.Enumeradores.StatusMDFe.EmitidoContingencia)
                                                {
                                                    filaConsulta.Enqueue(new Dominio.ObjetosDeValor.ObjetoConsulta(mdfe.Codigo, Dominio.Enumeradores.TipoObjetoConsulta.MDFe));
                                                    sleep = true;

                                                    //if (mdfe.Status == Dominio.Enumeradores.StatusMDFe.Enviado && mdfe.DataIntegracao < DateTime.Now.AddMinutes(-25))
                                                    //    servicoMDFe.NotificarMDFeEnviado(mdfe, 0, "25", unidadeDeTrabalho);
                                                }
                                                else if (mdfe.Status == Dominio.Enumeradores.StatusMDFe.Rejeicao)
                                                {
                                                    if (System.Configuration.ConfigurationManager.AppSettings["ReenviarRejeicaoMDFe"] == "SIM" && mdfe.TentativaReenvio <= 1)
                                                    {
                                                        if (mdfe.MensagemStatus != null && (
                                                            mdfe.MensagemStatus.CodigoDoErro == 8888 || //Falha de conexão com o Sefaz (Retorno Integrador)
                                                            mdfe.MensagemStatus.CodigoDoErro == 678 ||  //Uso indevido (Retorno Sefaz)
                                                            mdfe.MensagemStatus.CodigoDoErro == 109 ||  //Serviço paralisado (Retorno Sefaz)
                                                            mdfe.MensagemStatus.CodigoDoErro == 105))  //Lote em processamento (Retorno Sefaz)
                                                        {
                                                            Servicos.Log.TratarErro("MDF-e codigo " + mdfe.Codigo.ToString() + ": Reenviado rejeição:" + mdfe.MensagemStatus.CodigoDoErro.ToString(), "ReenvioMDFe");

                                                            mdfe.TentativaReenvio = mdfe.TentativaReenvio + 1;
                                                            bool reenvioMDFe = servicoMDFe.Emitir(mdfe, unidadeDeTrabalho);

                                                            if (reenvioMDFe)
                                                            {
                                                                servicoMDFe.AdicionarMDFeNaFilaDeConsulta(mdfe, unidadeDeTrabalho);
                                                                servicoMDFe.RemoverPendenciaMDFeCarga(mdfe, null, unidadeDeTrabalho);
                                                            }
                                                        }
                                                    }

                                                    string notificarEmail = System.Configuration.ConfigurationManager.AppSettings["NotificarEmailFalhaSefaz"];
                                                    int.TryParse(System.Configuration.ConfigurationManager.AppSettings["TentativasReenvio"], out int tentativasReenvio);
                                                    if (notificarEmail == "SIM" && tentativasReenvio == mdfe.TentativaReenvio)
                                                    {
                                                        // 670, 686, 611, 462, 662 
                                                        if (mdfe.MensagemStatus != null && (
                                                            mdfe.MensagemStatus.CodigoDoErro == 8888 || //Falha de conexão com o Sefaz (Retorno Integrador)
                                                            mdfe.MensagemStatus.CodigoDoErro == 678 ||  //Uso indevido (Retorno Sefaz)
                                                            mdfe.MensagemStatus.CodigoDoErro == 109 ||  //Serviço paralisado (Retorno Sefaz)
                                                            mdfe.MensagemStatus.CodigoDoErro == 105 ||  //Lote em processamento (Retorno Sefaz)
                                                            mdfe.MensagemStatus.CodigoDoErro == 670 ||  //Pendencias Encerramento
                                                            mdfe.MensagemStatus.CodigoDoErro == 686 ||  //Pendencias Encerramento
                                                            mdfe.MensagemStatus.CodigoDoErro == 611 ||  //Pendencias Encerramento
                                                            mdfe.MensagemStatus.CodigoDoErro == 462 ||  //Pendencias Encerramento
                                                            mdfe.MensagemStatus.CodigoDoErro == 662))  //Pendencias Encerramento
                                                        {
                                                            EnviarEmail("Alerta emissão MDFe-e", "MDF-e", "Emissor " + mdfe.Empresa.Descricao + " Número " + mdfe.Numero.ToString() + ": " + mdfe.MensagemStatus?.MensagemDoErro ?? mdfe.MensagemRetornoSefaz);
                                                        }
                                                    }

                                                }

                                            }
                                            else if (mdfe.Status == Dominio.Enumeradores.StatusMDFe.EmCancelamento)
                                            {
                                                mdfe = Servicos.Embarcador.EmissorDocumento.EmissorDocumentoService.GetEmissorDocumentoMDFe(mdfe.SistemaEmissor).ConsultarEventoCancelamento(mdfe, null, AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiCTe, unidadeDeTrabalho);

                                                if (mdfe.Status == Dominio.Enumeradores.StatusMDFe.EmCancelamento)
                                                {
                                                    filaConsulta.Enqueue(new Dominio.ObjetosDeValor.ObjetoConsulta(mdfe.Codigo, Dominio.Enumeradores.TipoObjetoConsulta.MDFe));
                                                    sleep = true;
                                                }
                                                else if (mdfe.Status == Dominio.Enumeradores.StatusMDFe.Cancelado)
                                                {
                                                    Servicos.SemParar servicoSemparar = new Servicos.SemParar(unidadeDeTrabalho);
                                                    Servicos.Target servicoTarget = new Servicos.Target(unidadeDeTrabalho);

                                                    servicoTarget.CancelarCompraValePedagioMDFe(mdfe, unidadeDeTrabalho);
                                                    servicoSemparar.CancelarCompraValePedagioMDFe(mdfe, unidadeDeTrabalho);

                                                    if (averbaMDFe)
                                                    {
                                                        Servicos.AverbacaoMDFe svcAverbacao = new Servicos.AverbacaoMDFe(unidadeDeTrabalho);
                                                        if (svcAverbacao.Emitir(mdfe, Dominio.Enumeradores.TipoAverbacaoMDFe.Cancelamento, unidadeDeTrabalho))
                                                            filaConsulta.Enqueue(new Dominio.ObjetosDeValor.ObjetoConsulta(mdfe.Codigo, Dominio.Enumeradores.TipoObjetoConsulta.AverbacaoMDFe));
                                                    }

                                                    servicoMDFe.AtualizarIntegracaoRetornoMDFe(mdfe, unidadeDeTrabalho);
                                                }

                                            }
                                            else if (mdfe.Status == Dominio.Enumeradores.StatusMDFe.EmEncerramento)
                                            {
                                                mdfe = Servicos.Embarcador.EmissorDocumento.EmissorDocumentoService.GetEmissorDocumentoMDFe(mdfe.SistemaEmissor).ConsultarEventoEncerramento(mdfe, null, AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiCTe, unidadeDeTrabalho);

                                                if (mdfe.Status == Dominio.Enumeradores.StatusMDFe.EmEncerramento)
                                                {
                                                    filaConsulta.Enqueue(new Dominio.ObjetosDeValor.ObjetoConsulta(mdfe.Codigo, Dominio.Enumeradores.TipoObjetoConsulta.MDFe));
                                                    sleep = true;
                                                }
                                            }
                                            else if (mdfe.Status == Dominio.Enumeradores.StatusMDFe.Encerrado)
                                            {
                                                if (averbaMDFe)
                                                {
                                                    Servicos.AverbacaoMDFe svcAverbacao = new Servicos.AverbacaoMDFe(unidadeDeTrabalho);
                                                    if (svcAverbacao.Emitir(mdfe, Dominio.Enumeradores.TipoAverbacaoMDFe.Encerramento, unidadeDeTrabalho))
                                                        filaConsulta.Enqueue(new Dominio.ObjetosDeValor.ObjetoConsulta(mdfe.Codigo, Dominio.Enumeradores.TipoObjetoConsulta.AverbacaoMDFe));
                                                }

                                                servicoMDFe.AtualizarIntegracaoRetornoMDFe(mdfe, unidadeDeTrabalho);
                                            }
                                            else if (mdfe.Status == Dominio.Enumeradores.StatusMDFe.EventoInclusaoMotoristaEnviado)
                                            {
                                                mdfe = Servicos.Embarcador.EmissorDocumento.EmissorDocumentoService.GetEmissorDocumentoMDFe(mdfe.SistemaEmissor).ConsultarEventoEncerramento(mdfe, null, AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiCTe, unidadeDeTrabalho);

                                                if (mdfe.Status == Dominio.Enumeradores.StatusMDFe.EventoInclusaoMotoristaEnviado)
                                                {
                                                    filaConsulta.Enqueue(new Dominio.ObjetosDeValor.ObjetoConsulta(mdfe.Codigo, Dominio.Enumeradores.TipoObjetoConsulta.MDFe));
                                                    sleep = true;
                                                }
                                            }
                                        }

                                        servicoMDFe = null;
                                        objetoConsulta = null;
                                        repMDFe = null;
                                        mdfe = null;
                                    }
                                    else if (objetoConsulta.Tipo == Dominio.Enumeradores.TipoObjetoConsulta.NFSe)
                                    {
                                        Servicos.Log.GravarDebug($"IniciarThread-NFSe {objetoConsulta.Codigo}", "FilaConsultaCTe");
                                        repNFSe = new Repositorio.NFSe(unidadeDeTrabalho);

                                        nfse = repNFSe.BuscarPorCodigo(objetoConsulta.Codigo);

                                        if (nfse != null)
                                        {
                                            bool utilizaEnotas = nfse.Empresa.Configuracao != null && nfse.Empresa.Configuracao.NFSeIntegracaoENotas && !string.IsNullOrWhiteSpace(nfse.Empresa.NFSeIDENotas) ? true : false;

                                            if (!utilizaEnotas)
                                            {
                                                servicoNFSe = new Servicos.NFSe(unidadeDeTrabalho);

                                                if (nfse.Status == Dominio.Enumeradores.StatusNFSe.Enviado || nfse.Status == Dominio.Enumeradores.StatusNFSe.Pendente)
                                                {

                                                    nfse = servicoNFSe.Consultar(nfse.Codigo, unidadeDeTrabalho);

                                                    if (nfse.Status == Dominio.Enumeradores.StatusNFSe.Enviado || nfse.Status == Dominio.Enumeradores.StatusNFSe.Pendente)
                                                    {
                                                        filaConsulta.Enqueue(new Dominio.ObjetosDeValor.ObjetoConsulta(nfse.Codigo, Dominio.Enumeradores.TipoObjetoConsulta.NFSe));
                                                        sleep = true;
                                                    }

                                                    if (nfse.Status == Dominio.Enumeradores.StatusNFSe.Autorizado)
                                                    {
                                                        bool averbarNFSe = (nfse.Empresa.Configuracao != null && nfse.Empresa.Configuracao.AverbaAutomaticoATM == 1) || (nfse.Empresa.Configuracao != null && nfse.Empresa.EmpresaPai != null && nfse.Empresa.EmpresaPai.Configuracao != null && nfse.Empresa.EmpresaPai.Configuracao.AverbaAutomaticoATM == 1);
                                                        if (averbarNFSe)
                                                        {
                                                            Servicos.AverbacaoNFSe svcAverbacaoNFSe = new Servicos.AverbacaoNFSe(unidadeDeTrabalho);
                                                            svcAverbacaoNFSe.Averbar(nfse, Dominio.Enumeradores.TipoAverbacaoCTe.Autorizacao, unidadeDeTrabalho);
                                                        }
                                                    }
                                                }
                                                else if (nfse.Status == Dominio.Enumeradores.StatusNFSe.EmCancelamento)
                                                {
                                                    nfse = servicoNFSe.ConsultarCancelamento(nfse.Codigo, unidadeDeTrabalho);

                                                    if (nfse.Status == Dominio.Enumeradores.StatusNFSe.EmCancelamento)
                                                    {
                                                        filaConsulta.Enqueue(new Dominio.ObjetosDeValor.ObjetoConsulta(nfse.Codigo, Dominio.Enumeradores.TipoObjetoConsulta.NFSe));
                                                        sleep = true;
                                                    }

                                                    if (nfse.Status == Dominio.Enumeradores.StatusNFSe.Cancelado)
                                                    {
                                                        bool averbarNFSe = (nfse.Empresa.Configuracao != null && nfse.Empresa.Configuracao.AverbaAutomaticoATM == 1) || (nfse.Empresa.Configuracao != null && nfse.Empresa.EmpresaPai != null && nfse.Empresa.EmpresaPai.Configuracao != null && nfse.Empresa.EmpresaPai.Configuracao.AverbaAutomaticoATM == 1);
                                                        if (averbarNFSe)
                                                        {
                                                            Servicos.AverbacaoNFSe svcAverbacaoNFSe = new Servicos.AverbacaoNFSe(unidadeDeTrabalho);
                                                            svcAverbacaoNFSe.Averbar(nfse, Dominio.Enumeradores.TipoAverbacaoCTe.Cancelamento, unidadeDeTrabalho);
                                                        }
                                                    }
                                                }
                                            }
                                            else
                                            {
                                                Servicos.NFSeENotas svcNFSeEnotas = new Servicos.NFSeENotas(unidadeDeTrabalho);

                                                var retornoNFSe = svcNFSeEnotas.ConsultarNFSeAsync(nfse.Codigo, unidadeDeTrabalho).ConfigureAwait(false).GetAwaiter().GetResult();
                                                if (!string.IsNullOrWhiteSpace(retornoNFSe))
                                                {
                                                    Servicos.Log.TratarErro(string.Concat("NFS-e eNota não gerada: ", retornoNFSe));
                                                    filaConsulta.Enqueue(new Dominio.ObjetosDeValor.ObjetoConsulta(nfse.Codigo, Dominio.Enumeradores.TipoObjetoConsulta.NFSe));
                                                    sleep = true;
                                                }
                                            }
                                        }

                                        servicoNFSe = null;
                                        objetoConsulta = null;
                                        repNFSe = null;
                                        nfse = null;
                                    }
                                    else if (objetoConsulta.Tipo == Dominio.Enumeradores.TipoObjetoConsulta.Averbacao)
                                    {
                                        Servicos.Log.GravarDebug($"IniciarThread-Averbacao {objetoConsulta.Codigo}", "FilaConsultaAverbacao");

                                        servicoAverbacao = new Servicos.Averbacao(unidadeDeTrabalho);

                                        if (!servicoAverbacao.ConsultarAverbacoes(objetoConsulta.Codigo, unidadeDeTrabalho))
                                        {
                                            filaConsulta.Enqueue(new Dominio.ObjetosDeValor.ObjetoConsulta(objetoConsulta.Codigo, Dominio.Enumeradores.TipoObjetoConsulta.Averbacao));
                                            sleep = true;
                                        }
                                    }
                                    else if (objetoConsulta.Tipo == Dominio.Enumeradores.TipoObjetoConsulta.AverbacaoMDFe)
                                    {
                                        Servicos.Log.GravarDebug($"IniciarThread-AverbacaoMDFe {objetoConsulta.Codigo}", "FilaConsultaAverbacaoMDFe");

                                        servicoAverbacaoMDFe = new Servicos.AverbacaoMDFe(unidadeDeTrabalho);

                                        if (!servicoAverbacaoMDFe.ConsultarAverbacoes(objetoConsulta.Codigo, unidadeDeTrabalho))
                                        {
                                            filaConsulta.Enqueue(new Dominio.ObjetosDeValor.ObjetoConsulta(objetoConsulta.Codigo, Dominio.Enumeradores.TipoObjetoConsulta.Averbacao));
                                            sleep = true;
                                        }
                                    }

                                    GC.Collect();

                                    if (sleep)
                                        System.Threading.Thread.Sleep(1000);

                                    sleep = false;
                                }

                                while (!filaConsulta.TryDequeue(out objetoConsulta))
                                    System.Threading.Thread.Sleep(2000);
                            }
                            catch (TaskCanceledException abort)
                            {
                                Servicos.Log.TratarErro(string.Concat("Task de consulta de objetos cancelada: ", abort.ToString()));

                                break;
                            }
                            catch (System.Threading.ThreadAbortException abortThread)
                            {
                                Servicos.Log.TratarErro(string.Concat("Thread de consulta de objetos cancelada: ", abortThread));

                                break;
                            }
                            catch (Exception ex)
                            {
                                Servicos.Log.TratarErro("FilaConsultaCTE: " + ex);

                                if (cte != null) //&& cte.Status == "E"
                                    filaConsulta.Enqueue(new Dominio.ObjetosDeValor.ObjetoConsulta(cte.Codigo, Dominio.Enumeradores.TipoObjetoConsulta.CTe));
                                else if (cce != null && cce.Status == Dominio.Enumeradores.StatusCCe.Enviado)
                                    filaConsulta.Enqueue(new Dominio.ObjetosDeValor.ObjetoConsulta(cce.Codigo, Dominio.Enumeradores.TipoObjetoConsulta.CCe));
                                else if (mdfe != null) //&& mdfe.Status == Dominio.Enumeradores.StatusMDFe.Enviado
                                    filaConsulta.Enqueue(new Dominio.ObjetosDeValor.ObjetoConsulta(mdfe.Codigo, Dominio.Enumeradores.TipoObjetoConsulta.MDFe));
                                else if (nfse != null && nfse.Status == Dominio.Enumeradores.StatusNFSe.Enviado)
                                    filaConsulta.Enqueue(new Dominio.ObjetosDeValor.ObjetoConsulta(nfse.Codigo, Dominio.Enumeradores.TipoObjetoConsulta.NFSe));

                                objetoConsulta = null;
                                cte = null;
                                mdfe = null;
                                cce = null;
                                nfse = null;
                                servicoCCe = null;
                                servicoCTe = null;
                                servicoMDFe = null;
                                repCCe = null;
                                repCTe = null;
                                repMDFe = null;
                                repNFSe = null;
                            }
                        }
                    }
                });

                if (ListaTasks.TryAdd(idFila, task))
                    task.Start();
                else
                    Servicos.Log.TratarErro("Não foi possível adicionar a task à fila.");
            }
            else
            {
                Servicos.Log.TratarErro("Não foi possível adicionar a fila de consultas.");
            }
        }

        private void EnviarEmail(string assunto, string documento, string texto)
        {
            try
            {
                Servicos.Email svcEmail = new Servicos.Email();

                string emailsNotificacao = ConfigurationManager.AppSettings["EmailsNotificacao"];
                string emailCopia = ConfigurationManager.AppSettings["EmailCopiaNotificacao"];

                if (string.IsNullOrWhiteSpace(emailsNotificacao))
                {
                    var listaEmails = emailsNotificacao.Split(';');
                    foreach (var email in listaEmails)
                    {
                        if (Utilidades.Validate.ValidarEmail(email))
                        {
                            System.Text.StringBuilder sb = new System.Text.StringBuilder();
                            sb.Append("<p>Alerta emissão ").Append(documento).Append(" - ").Append(DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss")).Append("<br /> <br />");
                            sb.Append(texto).Append("</p><br /> <br />");

                            System.Text.StringBuilder ss = new System.Text.StringBuilder();
                            ss.Append("MultiSoftware - http://www.multicte.com.br/ <br />");

                            svcEmail.EnviarEmail("cte@multicte.com.br", "cte@multicte.com.br", "mlv4email", email, emailCopia, "", assunto, sb.ToString(), "179.127.8.8", null, ss.ToString(), false, "cte@multisoftware.com.br");
                        }
                    }
                }
            }
            catch (Exception exptEmail)
            {
                Servicos.Log.TratarErro("Erro ao enviar e-mail:" + exptEmail);
            }
        }

    }
}