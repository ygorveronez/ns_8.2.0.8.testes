using Dominio.Excecoes.Embarcador;
using Dominio.Interfaces.Embarcador.GestaoPatio;
using Dominio.ObjetosDeValor.Embarcador.Enumeradores;
using System;
using System.Collections.Generic;

namespace Servicos.Embarcador.GestaoPatio
{
    public sealed class DocumentoFiscal : FluxoGestaoPatioEtapa, IFluxoGestaoPatioEtapaAdicionar, IFluxoGestaoPatioEtapaAlterarCarga, IFluxoGestaoPatioEtapaLiberarAutomaticamente
    {
        #region Construtores

        public DocumentoFiscal(Repositorio.UnitOfWork unitOfWork, Dominio.ObjetosDeValor.Embarcador.Auditoria.Auditado auditado) : this(unitOfWork, auditado, cliente: null) { }

        public DocumentoFiscal(Repositorio.UnitOfWork unitOfWork, Dominio.ObjetosDeValor.Embarcador.Auditoria.Auditado auditado, AdminMultisoftware.Dominio.Entidades.Pessoas.Cliente cliente) : base(unitOfWork, auditado, EtapaFluxoGestaoPatio.DocumentoFiscal, cliente) { }

        #endregion

        #region Métodos Privados

        private void AvancarEtapa(Dominio.Entidades.Embarcador.GestaoPatio.FluxoGestaoPatio fluxoGestaoPatio, Dominio.Entidades.Embarcador.GestaoPatio.DocumentoFiscal documentoFiscal, Dominio.ObjetosDeValor.Embarcador.GestaoPatio.DocumentoFiscalAvancar documentoFiscalAvancar)
        {
            if (documentoFiscal == null)
                throw new ServicoException("Não foi possível encontrar o registro.");

            if (!documentoFiscal.EtapaDocumentoFiscalLiberada)
                throw new ServicoException("A liberação do documento fiscal ainda não foi autorizada.");

            documentoFiscal.Initialize();
            documentoFiscal.DataDocumentoFiscalInformado = DateTime.Now;

            if (documentoFiscalAvancar != null)
            {
                documentoFiscal.NumeroDocumento = string.Join(", ", documentoFiscalAvancar.NumerosDocumentos);

                if (documentoFiscal.NumerosDocumentos == null)
                    documentoFiscal.NumerosDocumentos = new List<string>();

                documentoFiscal.NumerosDocumentos.Clear();

                foreach (string numeroDocumento in documentoFiscalAvancar.NumerosDocumentos)
                    documentoFiscal.NumerosDocumentos.Add(numeroDocumento);

                VincularNotasFiscais(fluxoGestaoPatio, documentoFiscalAvancar.NumerosDocumentos);
            }

            LiberarProximaEtapa(fluxoGestaoPatio);

            new Repositorio.Embarcador.GestaoPatio.DocumentoFiscal(_unitOfWork).Atualizar(documentoFiscal, _auditado);
        }

        private void VincularNotaFiscal(string numeroDocumento, Dominio.Entidades.Embarcador.Cargas.CargaPedido cargaPedido, Dominio.Entidades.Embarcador.Configuracao.ConfiguracaoTMS configuracaoEmbarcador)
        {
            int numeroNotaFiscal = numeroDocumento.ToInt();
            Repositorio.Embarcador.Pedidos.XMLNotaFiscal repositorioXmlNotaFiscal = new Repositorio.Embarcador.Pedidos.XMLNotaFiscal(_unitOfWork);
            Dominio.Entidades.Embarcador.Pedidos.XMLNotaFiscal xmlNotaFiscal = repositorioXmlNotaFiscal.BuscarPorNumero(numeroNotaFiscal);

            if (xmlNotaFiscal == null)
                throw new ServicoException($"Não é possível avançar a etapa pois nenhuma nota fiscal com o número {numeroDocumento} foi encontrada.");

            Pedido.NotaFiscal servicoNotaFiscal = new Pedido.NotaFiscal(_unitOfWork);
            Repositorio.Embarcador.Cargas.CargaPedidoXMLNotaFiscalParcial repositorioNotaParcial = new Repositorio.Embarcador.Cargas.CargaPedidoXMLNotaFiscalParcial(_unitOfWork);
            Repositorio.Embarcador.Pedidos.PedidoXMLNotaFiscal repositorioPedidoXmlNotaFiscal = new Repositorio.Embarcador.Pedidos.PedidoXMLNotaFiscal(_unitOfWork);
            Dominio.Entidades.Embarcador.Cargas.CargaPedidoXMLNotaFiscalParcial notaParcial = repositorioNotaParcial.BuscarPrimeiroPorCargaPedidoENumero(cargaPedido.Codigo, numeroNotaFiscal);

            if (notaParcial == null)
                notaParcial = new Dominio.Entidades.Embarcador.Cargas.CargaPedidoXMLNotaFiscalParcial();

            notaParcial.CargaPedido = cargaPedido;
            notaParcial.Numero = xmlNotaFiscal.Numero;
            notaParcial.Chave = xmlNotaFiscal.Chave;

            if (notaParcial.Codigo > 0)
                Servicos.Log.TratarErro($"3 Adicionando Pedidos Parciais {DateTime.Now.ToString("dd/MM/yyyy hh:mm:ss")} Pedido: [{cargaPedido.Pedido.Codigo}]");

            if (notaParcial.Codigo > 0)
                repositorioNotaParcial.Atualizar(notaParcial);
            else
                repositorioNotaParcial.Inserir(notaParcial);


            bool mensagemAlertaObservacao;
            bool notaFiscalEmOutraCarga;
            string mensagemErroValidacaoRegrasNota = servicoNotaFiscal.ValidarRegrasNota(xmlNotaFiscal, cargaPedido, AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiEmbarcador, out mensagemAlertaObservacao, out notaFiscalEmOutraCarga);

            if (!string.IsNullOrEmpty(mensagemErroValidacaoRegrasNota) && !mensagemAlertaObservacao)
                throw new ServicoException($"Nota {xmlNotaFiscal.Numero}: {mensagemErroValidacaoRegrasNota}");

            servicoNotaFiscal.VincularXMLNotaFiscal(xmlNotaFiscal, configuracaoEmbarcador, AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiEmbarcador, _auditado, false, false);

            if (!repositorioPedidoXmlNotaFiscal.VerificarSeExistePorNotaFiscalECarga(xmlNotaFiscal.Codigo, cargaPedido.Carga.Codigo))
                throw new ServicoException($"Não foi possível vincular a nota {xmlNotaFiscal.Numero}.");
        }

        private void VincularNotasFiscais(Dominio.Entidades.Embarcador.GestaoPatio.FluxoGestaoPatio fluxoGestaoPatio, List<string> numerosDocumentos)
        {
            if (fluxoGestaoPatio.Carga == null)
                return;

            FluxoGestaoPatioConfiguracaoEtapa servicoFluxoGestaoPatioConfiguracaoEtapa = new FluxoGestaoPatioConfiguracaoEtapa(_unitOfWork);
            Dominio.Entidades.Embarcador.Filiais.SequenciaGestaoPatio sequenciaGestaoPatio = servicoFluxoGestaoPatioConfiguracaoEtapa.ObterSequenciaGestaoPatio(fluxoGestaoPatio);

            if (!(sequenciaGestaoPatio?.DocumentoFiscalVincularNotaFiscal ?? false))
                return;

            Repositorio.Embarcador.Cargas.CargaPedido repositorioCargaPedido = new Repositorio.Embarcador.Cargas.CargaPedido(_unitOfWork);
            Dominio.Entidades.Embarcador.Cargas.CargaPedido cargaPedido = repositorioCargaPedido.BuscarPrimeiroPorCargaSemFetch(fluxoGestaoPatio.Carga.Codigo);

            if (cargaPedido == null)
                return;

            Repositorio.Embarcador.Configuracao.ConfiguracaoTMS repositorioConfiguracaoEmbarcador = new Repositorio.Embarcador.Configuracao.ConfiguracaoTMS(_unitOfWork);
            Dominio.Entidades.Embarcador.Configuracao.ConfiguracaoTMS configuracaoEmbarcador = repositorioConfiguracaoEmbarcador.BuscarConfiguracaoPadrao();

            foreach (string numeroDocumento in numerosDocumentos)
                VincularNotaFiscal(numeroDocumento, cargaPedido, configuracaoEmbarcador);

            Repositorio.Embarcador.Pedidos.PedidoXMLNotaFiscal repositorioPedidoXmlNotaFiscal = new Repositorio.Embarcador.Pedidos.PedidoXMLNotaFiscal(_unitOfWork);
            int totalPedidosXmlNotaFiscal = repositorioPedidoXmlNotaFiscal.ContarPorCarga(fluxoGestaoPatio.Carga.Codigo);
            int totalCargaPedidos = repositorioCargaPedido.ContarPorCarga(fluxoGestaoPatio.Carga.Codigo);

            if (totalCargaPedidos == totalPedidosXmlNotaFiscal)
            {
                Repositorio.Embarcador.Cargas.Carga repositorioCarga = new Repositorio.Embarcador.Cargas.Carga(_unitOfWork);

                fluxoGestaoPatio.Carga.ProcessandoDocumentosFiscais = true;
                fluxoGestaoPatio.Carga.DataInicioConfirmacaoDocumentosFiscais = DateTime.Now;

                repositorioCarga.Atualizar(fluxoGestaoPatio.Carga);
            }
        }

        #endregion

        #region Métodos Públicos

        public void Adicionar(Dominio.ObjetosDeValor.Embarcador.GestaoPatio.FluxoGestaoPatioEtapaAdicionar fluxoGestaoPatioEtapaAdicionar)
        {
            Repositorio.Embarcador.GestaoPatio.DocumentoFiscal repositorioDocumentoFiscal = new Repositorio.Embarcador.GestaoPatio.DocumentoFiscal(_unitOfWork);
            Dominio.Entidades.Embarcador.GestaoPatio.DocumentoFiscal documentoFiscal = repositorioDocumentoFiscal.BuscarPorFluxoGestaoPatio(fluxoGestaoPatioEtapaAdicionar.FluxoGestaoPatio.Codigo);

            if (documentoFiscal != null)
                return;

            documentoFiscal = new Dominio.Entidades.Embarcador.GestaoPatio.DocumentoFiscal()
            {
                FluxoGestaoPatio = fluxoGestaoPatioEtapaAdicionar.FluxoGestaoPatio,
                Carga = fluxoGestaoPatioEtapaAdicionar.FluxoGestaoPatio.Carga,
                PreCarga = fluxoGestaoPatioEtapaAdicionar.FluxoGestaoPatio.PreCarga,
                EtapaDocumentoFiscalLiberada = fluxoGestaoPatioEtapaAdicionar.EtapaLiberada
            };

            repositorioDocumentoFiscal.Inserir(documentoFiscal);
        }

        public void Avancar(Dominio.ObjetosDeValor.Embarcador.GestaoPatio.DocumentoFiscalAvancar documentoFiscalAvancar)
        {
            Repositorio.Embarcador.GestaoPatio.DocumentoFiscal repositorioDocumentoFiscal = new Repositorio.Embarcador.GestaoPatio.DocumentoFiscal(_unitOfWork);
            Dominio.Entidades.Embarcador.GestaoPatio.DocumentoFiscal documentoFiscal = repositorioDocumentoFiscal.BuscarPorCodigo(documentoFiscalAvancar.Codigo);

            if ((documentoFiscalAvancar.NumerosDocumentos == null) || (documentoFiscalAvancar.NumerosDocumentos.Count == 0))
                throw new ServicoException("Um ou mais números de documentos devem ser informados");

            AvancarEtapa(documentoFiscal?.FluxoGestaoPatio, documentoFiscal, documentoFiscalAvancar);
        }

        public void DefinirCarga(Dominio.Entidades.Embarcador.GestaoPatio.FluxoGestaoPatio fluxoGestaoPatio, Dominio.Entidades.Embarcador.Cargas.Carga carga, bool etapaLiberada)
        {
            Repositorio.Embarcador.GestaoPatio.DocumentoFiscal repositorioDocumentoFiscal = new Repositorio.Embarcador.GestaoPatio.DocumentoFiscal(_unitOfWork);
            Dominio.Entidades.Embarcador.GestaoPatio.DocumentoFiscal documentoFiscal = repositorioDocumentoFiscal.BuscarPorFluxoGestaoPatio(fluxoGestaoPatio.Codigo);

            if (documentoFiscal != null)
            {
                documentoFiscal.Carga = carga;
                repositorioDocumentoFiscal.Atualizar(documentoFiscal);
            }
        }

        public void LiberarProximaEtapaAutomaticamente(Dominio.Entidades.Embarcador.GestaoPatio.FluxoGestaoPatio fluxoGestaoPatio)
        {
            if ((fluxoGestaoPatio.Carga == null) || (fluxoGestaoPatio.Carga.DataFinalizacaoProcessamentoDocumentosFiscais == null))
                return;

            Dominio.Entidades.Embarcador.Filiais.ConfiguracaoGestaoPatio configuracaoGestaoPatio = ObterConfiguracaoGestaoPatio();

            if (configuracaoGestaoPatio.DocumentoFiscalPermiteAvancarAutomaticamenteAposNotasFiscaisInseridas)
                LiberarProximaEtapa(fluxoGestaoPatio);
        }

        public void TrocarCarga(Dominio.Entidades.Embarcador.GestaoPatio.FluxoGestaoPatio fluxoGestaoPatio, Dominio.Entidades.Embarcador.Cargas.Carga cargaNova)
        {
            Repositorio.Embarcador.GestaoPatio.DocumentoFiscal repositorioDocumentoFiscal = new Repositorio.Embarcador.GestaoPatio.DocumentoFiscal(_unitOfWork);
            Dominio.Entidades.Embarcador.GestaoPatio.DocumentoFiscal documentoFiscal = repositorioDocumentoFiscal.BuscarPorFluxoGestaoPatio(fluxoGestaoPatio.Codigo);

            if (documentoFiscal != null)
            {
                documentoFiscal.Carga = cargaNova;
                repositorioDocumentoFiscal.Atualizar(documentoFiscal);
            }
        }

        #endregion

        #region Métodos Públicos Sobrescritos

        public override void AtualizarDataPrevista(Dominio.Entidades.Embarcador.GestaoPatio.FluxoGestaoPatio fluxoGestaoPatio, Dominio.ObjetosDeValor.Embarcador.GestaoPatio.PreSetTempoEtapa preSetTempoEtapa)
        {
            if (preSetTempoEtapa.DataDocumentoFiscalPrevista.HasValue)
                fluxoGestaoPatio.DataDocumentoFiscalPrevista = preSetTempoEtapa.DataDocumentoFiscalPrevista.Value;
        }

        public override void Avancar(Dominio.Entidades.Embarcador.GestaoPatio.FluxoGestaoPatio fluxoGestaoPatio)
        {
            Repositorio.Embarcador.GestaoPatio.DocumentoFiscal repositorioDocumentoFiscal = new Repositorio.Embarcador.GestaoPatio.DocumentoFiscal(_unitOfWork);
            Dominio.Entidades.Embarcador.GestaoPatio.DocumentoFiscal documentoFiscal = repositorioDocumentoFiscal.BuscarPorFluxoGestaoPatio(fluxoGestaoPatio.Codigo);

            AvancarEtapa(fluxoGestaoPatio, documentoFiscal, documentoFiscalAvancar: null);
        }

        public override void DefinirDataPrevista(Dominio.Entidades.Embarcador.GestaoPatio.FluxoGestaoPatio fluxoGestaoPatio, DateTime dataPrevista, Dominio.ObjetosDeValor.Embarcador.GestaoPatio.PreSetTempoEtapa preSetTempoEtapa)
        {
            fluxoGestaoPatio.DataDocumentoFiscalPrevista = preSetTempoEtapa.DataDocumentoFiscalPrevista ?? dataPrevista;
        }

        public override bool DefinirTempo(Dominio.Entidades.Embarcador.GestaoPatio.FluxoGestaoPatio fluxoGestaoPatio, DateTime data, decimal tempoEtapaAnterior)
        {
            if (fluxoGestaoPatio.DataDocumentoFiscal.HasValue)
                return false;

            fluxoGestaoPatio.TempoAguardandoDocumentoFiscal = tempoEtapaAnterior;
            fluxoGestaoPatio.DataDocumentoFiscal = data;

            return true;
        }

        public override bool Liberar(Dominio.Entidades.Embarcador.GestaoPatio.FluxoGestaoPatio fluxoGestaoPatio)
        {
            Repositorio.Embarcador.GestaoPatio.DocumentoFiscal repositorioDocumentoFiscal = new Repositorio.Embarcador.GestaoPatio.DocumentoFiscal(_unitOfWork);
            Dominio.Entidades.Embarcador.GestaoPatio.DocumentoFiscal documentoFiscal = repositorioDocumentoFiscal.BuscarPorFluxoGestaoPatio(fluxoGestaoPatio.Codigo);

            if (documentoFiscal != null)
            {
                documentoFiscal.EtapaDocumentoFiscalLiberada = true;
                repositorioDocumentoFiscal.Atualizar(documentoFiscal);
            }

            return true;
        }

        public override DateTime? ObterData(Dominio.Entidades.Embarcador.GestaoPatio.FluxoGestaoPatio fluxoGestaoPatio)
        {
            return fluxoGestaoPatio.DataDocumentoFiscal;
        }

        public override DateTime? ObterDataPrevista(Dominio.Entidades.Embarcador.GestaoPatio.FluxoGestaoPatio fluxoGestaoPatio)
        {
            return fluxoGestaoPatio.DataDocumentoFiscalPrevista;
        }

        public override void RemoverLiberacao(Dominio.Entidades.Embarcador.GestaoPatio.FluxoGestaoPatio fluxoGestaoPatio)
        {
            Repositorio.Embarcador.GestaoPatio.DocumentoFiscal repositorioDocumentoFiscal = new Repositorio.Embarcador.GestaoPatio.DocumentoFiscal(_unitOfWork);
            Dominio.Entidades.Embarcador.GestaoPatio.DocumentoFiscal documentoFiscal = repositorioDocumentoFiscal.BuscarPorFluxoGestaoPatio(fluxoGestaoPatio.Codigo);

            if (documentoFiscal != null)
            {
                documentoFiscal.EtapaDocumentoFiscalLiberada = false;
                repositorioDocumentoFiscal.Atualizar(documentoFiscal);
            }
        }

        public override void RemoverTempo(Dominio.Entidades.Embarcador.GestaoPatio.FluxoGestaoPatio fluxoGestaoPatio)
        {
            fluxoGestaoPatio.TempoAguardandoDocumentoFiscal = 0;
            fluxoGestaoPatio.DataDocumentoFiscal = null;
        }

        public override void ReprogramarTempo(Dominio.Entidades.Embarcador.GestaoPatio.FluxoGestaoPatio fluxoGestaoPatio, TimeSpan tempoReprogramar)
        {
            if (fluxoGestaoPatio.DataDocumentoFiscalPrevista.HasValue)
                fluxoGestaoPatio.DataDocumentoFiscalReprogramada = fluxoGestaoPatio.DataDocumentoFiscalPrevista.Value.Add(tempoReprogramar);
        }

        #endregion
    }
}
