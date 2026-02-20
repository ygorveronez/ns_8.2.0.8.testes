using Dominio.ObjetosDeValor.Embarcador.Enumeradores;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Servicos.Embarcador.PreCarga
{
    public class PreCarga
    {
        #region Atributos

        private readonly Repositorio.UnitOfWork _unitOfWork;

        #endregion Atributos

        #region Construtores

        public PreCarga(Repositorio.UnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        #endregion

        #region Métodos Públicos

        public void AdicionarAlteracaoDadosPreCarga(Dominio.Entidades.Embarcador.Cargas.Carga preCarga, Dominio.Entidades.Embarcador.Cargas.Carga carga)
        {
            List<string> mensagens = new List<string>();
            bool transportadorAlterado = AdicionarMensagemAlteracaoTransportadorPreCarga(mensagens, preCarga, carga);
            bool veiculoAlterado = AdicionarAlteracaoVeiculoPreCarga(mensagens, preCarga, carga);

            if (veiculoAlterado)
            {
                carga.Veiculo = preCarga.Veiculo;
                carga.VeiculosVinculados = preCarga.VeiculosVinculados?.ToList();
            }

            if (transportadorAlterado || veiculoAlterado)
            {
                Carga.MensagemAlertaCarga servicoMensagemAlerta = new Carga.MensagemAlertaCarga(_unitOfWork);

                servicoMensagemAlerta.Adicionar(carga, TipoMensagemAlerta.AlteracaoDadosPreCarga, mensagens);
            }

            if (IsVeiculoInformadoNaoPertenceEmpresaCarga(carga))
            {
                carga.SituacaoCarga = SituacaoCarga.Nova;
                carga.Veiculo = null;
                carga.VeiculosVinculados?.Clear();
                carga.Motoristas?.Clear();
            }
        }

        public bool AdicionarAlteracaoDadosPreCargaAgrupada(Dominio.Entidades.Embarcador.Cargas.Carga preCargaAgrupada, Dominio.Entidades.Embarcador.Cargas.Carga carga)
        {
            List<string> mensagens = new List<string>();
            bool transportadorAlterado = AdicionarMensagemAlteracaoTransportadorPreCargaAgrupada(mensagens, preCargaAgrupada, carga);
            bool veiculoAlterado = AdicionarAlteracaoVeiculoPreCargaAgrupada(mensagens, preCargaAgrupada, carga);

            if (!transportadorAlterado && !veiculoAlterado)
                return false;

            Carga.MensagemAlertaCarga servicoMensagemAlerta = new Carga.MensagemAlertaCarga(_unitOfWork);

            servicoMensagemAlerta.Adicionar(preCargaAgrupada, TipoMensagemAlerta.AlteracaoDadosPreCarga, mensagens);

            if (transportadorAlterado)
                preCargaAgrupada.Empresa = null;

            if (veiculoAlterado)
            {
                preCargaAgrupada.Veiculo = null;
                preCargaAgrupada.VeiculosVinculados?.Clear();
                preCargaAgrupada.Motoristas?.Clear();
            }

            preCargaAgrupada.SituacaoCarga = SituacaoCarga.Nova;

            return true;
        }

        public void TrocarPreCarga(Dominio.Entidades.Embarcador.Cargas.Carga preCarga, Dominio.Entidades.Embarcador.Cargas.Carga carga, AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware tipoServicoMultisoftware, Dominio.Entidades.Embarcador.Configuracao.ConfiguracaoTMS configuracaoEmbarcador, AdminMultisoftware.Dominio.Entidades.Pessoas.Cliente ClienteMultisoftware)
        {
            Carga.Carga.TrocarCarga(preCarga, carga, tipoServicoMultisoftware, ClienteMultisoftware, configuracaoEmbarcador, _unitOfWork, trocarCargaAgrupada: true);
            Auditoria.Auditoria.AuditarSemDadosUsuario(carga, $"Pré carga trocada para carga.", _unitOfWork);
        }

        public async Task TrocarPreCargaCompletoAsync(Dominio.Entidades.Embarcador.Cargas.Carga carga, AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware tipoServicoMultisoftware, Dominio.Entidades.Embarcador.Configuracao.ConfiguracaoTMS configuracaoEmbarcador, AdminMultisoftware.Dominio.Entidades.Pessoas.Cliente ClienteMultisoftware)
        {
            if (carga.PreCarga != null)
            {
                Servicos.Embarcador.Logistica.CargaJanelaCarregamento servicoCargaJanelaCarregamento = new Servicos.Embarcador.Logistica.CargaJanelaCarregamento(_unitOfWork, configuracaoEmbarcador);
                await servicoCargaJanelaCarregamento.DefinirCargaPorPreCargaAsync(carga, carga.PreCarga);
                await VincularCargaPreCargaAsync(carga.PreCarga, carga, tipoServicoMultisoftware);
            }

            if (carga.CargaPreCarga != null)
            {
                await Carga.Carga.TrocarCargaAsync(carga.CargaPreCarga, carga, tipoServicoMultisoftware, ClienteMultisoftware, configuracaoEmbarcador, _unitOfWork, trocarCargaAgrupada: true);
                Auditoria.Auditoria.AuditarSemDadosUsuario(carga, $"Pré carga trocada para carga", _unitOfWork);
            }
        }

        public bool CriarPreCarga(out string erro, Dominio.Entidades.Embarcador.Pedidos.Pedido pedido, bool coletaEmProdutorRural, AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware tipoServicoMultisoftware, Dominio.Entidades.Usuario usuario)
        {
            Repositorio.Embarcador.PreCargas.PreCarga repPreCarga = new Repositorio.Embarcador.PreCargas.PreCarga(_unitOfWork);
            Repositorio.Embarcador.Pedidos.Pedido repPedido = new Repositorio.Embarcador.Pedidos.Pedido(_unitOfWork);
            Repositorio.Embarcador.ProdutorRural.PedidoColetaProdutor repPedidoColetaProdutorRural = new Repositorio.Embarcador.ProdutorRural.PedidoColetaProdutor(_unitOfWork);
            Repositorio.Embarcador.Pedidos.XMLNotaFiscal repXMLNotaFiscal = new Repositorio.Embarcador.Pedidos.XMLNotaFiscal(_unitOfWork);
            Carga.CargaDadosSumarizados servicoCargaDadosSumarizados = new Carga.CargaDadosSumarizados(_unitOfWork);

            Dominio.Entidades.Embarcador.PreCargas.PreCarga preCarga = repPreCarga.BuscarPorPedido(pedido.Codigo);

            if (preCarga != null && preCarga.SituacaoPreCarga != SituacaoPreCarga.AguardandoGeracaoCarga)
            {
                erro = "Não é possível atualizar uma pré carga nesta situação.";
                return false;
            }

            if (pedido.Destinatario == null || pedido.Remetente == null)
            {
                erro = "É necessário selecionar um remetente/destinatário.";
                return false;
            }

            if (pedido.ModeloVeicularCarga == null)
            {
                erro = "É necessário selecionar um modelo veicular de carga.";
                return false;
            }

            if (pedido.Empresa == null)
            {
                erro = "É necessário selecionar uma empresa.";
                return false;
            }

            if (pedido.TipoDeCarga == null)
            {
                erro = "É necessário selecionar um tipo de carga.";
                return false;
            }

            if (pedido.Veiculos == null || pedido.Veiculos.Count == 0)
            {
                erro = "É necessário selecionar um veículo.";
                return false;
            }

            if (pedido.Motoristas == null || pedido.Motoristas.Count == 0)
            {
                erro = "É necessário selecionar um motorista.";
                return false;
            }

            if (preCarga == null)
            {
                preCarga = new Dominio.Entidades.Embarcador.PreCargas.PreCarga();
                preCarga.NumeroPreCargaInterno = repPreCarga.BuscarUltimoNumeroPreCarga() + 1;
                preCarga.NumeroPreCarga = preCarga.NumeroPreCargaInterno.ToString();
                preCarga.DataCriacaoPreCarga = DateTime.Now;
                preCarga.AdicionadaManualmente = true;
                preCarga.Operador = usuario;
            }

            preCarga.DataPrevisaoEntrega = pedido.PrevisaoEntrega;
            preCarga.DataInicioViagem = pedido.DataInicialColeta;
            preCarga.DataPrevisaoFimViagem = pedido.PrevisaoEntrega;
            preCarga.DataPrevisaoInicioViagem = pedido.DataInicialColeta;
            preCarga.Empresa = pedido.Empresa;
            preCarga.ModeloVeicularCarga = pedido.ModeloVeicularCarga;
            preCarga.Destinatarios = new List<Dominio.Entidades.Cliente>() { pedido.Destinatario };
            preCarga.Motoristas = pedido.Motoristas.ToList();
            preCarga.Rota = pedido.RotaFrete;
            preCarga.SituacaoPreCarga = SituacaoPreCarga.AguardandoGeracaoCarga;
            preCarga.TipoDeCarga = pedido.TipoDeCarga;
            preCarga.TipoOperacao = pedido.TipoOperacao;
            preCarga.Veiculo = pedido.Veiculos.Where(o => o.TipoVeiculo == "0").FirstOrDefault();
            preCarga.VeiculosVinculados = pedido.Veiculos.Where(o => o.TipoVeiculo != "0").ToList();
            preCarga.CalculandoFrete = true;

            if (preCarga.Codigo > 0)
                repPreCarga.Atualizar(preCarga);
            else
                repPreCarga.Inserir(preCarga);

            servicoCargaDadosSumarizados.AlterarDadosSumarizadosPreCarga(preCarga, _unitOfWork, tipoServicoMultisoftware);

            pedido.PreCarga = preCarga;

            repPedido.Atualizar(pedido);

            if (coletaEmProdutorRural)
            {
                Servicos.Embarcador.ProdutorRural.FechamentoProdutorRural.GerarPedidoColetaProdutor(pedido, _unitOfWork);

                Dominio.Entidades.Embarcador.Pedidos.XMLNotaFiscal xmlNotaFiscal = pedido.NotasFiscais?.FirstOrDefault();

                if (xmlNotaFiscal == null)
                    xmlNotaFiscal = new Dominio.Entidades.Embarcador.Pedidos.XMLNotaFiscal();

                xmlNotaFiscal.DataEmissao = DateTime.Now;
                xmlNotaFiscal.Destinatario = pedido.Destinatario;
                xmlNotaFiscal.Emitente = pedido.Remetente;
                xmlNotaFiscal.Numero = 1;
                xmlNotaFiscal.Descricao = "NOTA FISCAL";
                xmlNotaFiscal.Modelo = "99";
                xmlNotaFiscal.TipoOperacaoNotaFiscal = TipoOperacaoNotaFiscal.Saida;
                xmlNotaFiscal.TipoDocumento = TipoDocumento.Outros;
                xmlNotaFiscal.Peso = pedido.PesoTotal;
                xmlNotaFiscal.Valor = pedido.ValorTotalCarga;
                xmlNotaFiscal.Volumes = pedido.QtVolumes;
                xmlNotaFiscal.QuantidadePallets = pedido.NumeroPaletes;
                xmlNotaFiscal.XML = string.Empty;
                xmlNotaFiscal.CNPJTranposrtador = string.Empty;
                xmlNotaFiscal.PlacaVeiculoNotaFiscal = string.Empty;
                xmlNotaFiscal.nfAtiva = true;

                if (pedido.TipoPagamento == Dominio.Enumeradores.TipoPagamento.Pago)
                    xmlNotaFiscal.ModalidadeFrete = ModalidadePagamentoFrete.Pago;
                else if (pedido.TipoPagamento == Dominio.Enumeradores.TipoPagamento.A_Pagar)
                    xmlNotaFiscal.ModalidadeFrete = ModalidadePagamentoFrete.A_Pagar;
                else
                    xmlNotaFiscal.ModalidadeFrete = ModalidadePagamentoFrete.NaoDefinido;

                if (xmlNotaFiscal.Codigo > 0)
                    repXMLNotaFiscal.Atualizar(xmlNotaFiscal);
                else
                {
                    xmlNotaFiscal.DataRecebimento = DateTime.Now;
                    repXMLNotaFiscal.Inserir(xmlNotaFiscal);
                }

                if (pedido.NotasFiscais == null)
                    pedido.NotasFiscais = new List<Dominio.Entidades.Embarcador.Pedidos.XMLNotaFiscal>();

                if (!pedido.NotasFiscais.Any(o => o.Codigo == xmlNotaFiscal.Codigo))
                    pedido.NotasFiscais.Add(xmlNotaFiscal);

                repPedido.Atualizar(pedido);
            }

            erro = string.Empty;
            return true;
        }

        public Dominio.Entidades.Embarcador.PreCargas.PreCarga CriarPreCarga(Dominio.ObjetosDeValor.WebService.Carga.CargaIntegracao cargaIntegracao, Dominio.Entidades.Embarcador.Pedidos.Pedido pedido, ref StringBuilder stMensagem, AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware tipoServicoMultisoftware)
        {
            Repositorio.Embarcador.PreCargas.PreCarga repPreCarga = new Repositorio.Embarcador.PreCargas.PreCarga(_unitOfWork);
            Repositorio.Embarcador.Filiais.Filial repFilial = new Repositorio.Embarcador.Filiais.Filial(_unitOfWork);
            Repositorio.Embarcador.Cargas.TipoDeCarga repTipoDeCarga = new Repositorio.Embarcador.Cargas.TipoDeCarga(_unitOfWork);
            Repositorio.Embarcador.Cargas.ModeloVeicularCarga repModeloVeicular = new Repositorio.Embarcador.Cargas.ModeloVeicularCarga(_unitOfWork);
            Repositorio.Veiculo repVeiculo = new Repositorio.Veiculo(_unitOfWork);
            Repositorio.Embarcador.Pedidos.Pedido repPedido = new Repositorio.Embarcador.Pedidos.Pedido(_unitOfWork);
            Repositorio.Embarcador.Veiculos.VeiculoMotorista repVeiculoMotorista = new Repositorio.Embarcador.Veiculos.VeiculoMotorista(_unitOfWork);

            Dominio.Entidades.Embarcador.PreCargas.PreCarga preCarga = repPreCarga.BuscarPorNumeroFilial(cargaIntegracao.NumeroCarga, cargaIntegracao.Filial?.CodigoIntegracao ?? "");

            if (preCarga == null)
            {
                preCarga = new Dominio.Entidades.Embarcador.PreCargas.PreCarga();

                if (cargaIntegracao.Filial != null)
                {
                    preCarga.Filial = repFilial.buscarPorCodigoEmbarcador(cargaIntegracao.Filial.CodigoIntegracao);
                    if (preCarga.Filial == null)
                        stMensagem.Append("Não existe uma filial com código de integração " + cargaIntegracao.Filial.CodigoIntegracao + " na base da Multisoftware. ");
                }

                if (cargaIntegracao.TipoCargaEmbarcador != null)
                {
                    preCarga.TipoDeCarga = repTipoDeCarga.BuscarPorCodigoEmbarcador(cargaIntegracao.TipoCargaEmbarcador.CodigoIntegracao);
                    if (preCarga.TipoDeCarga == null)
                        stMensagem.Append("Não existe um tipo de carga com código de integração " + cargaIntegracao.TipoCargaEmbarcador.CodigoIntegracao + " na base da Multisoftware. ");
                }

                if (cargaIntegracao.ModeloVeicular != null)
                {
                    preCarga.ModeloVeicularCarga = repModeloVeicular.buscarPorCodigoIntegracao(cargaIntegracao.ModeloVeicular.CodigoIntegracao);
                    //if (preCarga.ModeloVeicularCarga == null)
                    //    stMensagem.Append("Não existe um modelo veicular com código de integração " + cargaIntegracao.ModeloVeicular.CodigoIntegracao + " na base da Multisoftware. ");
                }

                if (cargaIntegracao.TransportadoraEmitente != null)
                {
                    Repositorio.Empresa repEmpresa = new Repositorio.Empresa(_unitOfWork);

                    preCarga.Empresa = repEmpresa.BuscarPorCNPJ(Utilidades.String.OnlyNumbers(cargaIntegracao.TransportadoraEmitente.CNPJ));

                    if (preCarga.Empresa == null || preCarga.Empresa.Status == "I")
                    {
                        stMensagem.Append("Não foi encontrado um transportador para o CNPJ " + cargaIntegracao.TransportadoraEmitente.CNPJ + "na base da Multisoftware");
                    }
                    else
                    {
                        pedido.Empresa = preCarga.Empresa;
                        repPedido.Atualizar(pedido);
                    }
                }

                preCarga.Distancia = cargaIntegracao.Distancia;
                preCarga.Rota = pedido.RotaFrete;
                preCarga.NumeroPreCarga = cargaIntegracao.NumeroCarga;

                if (stMensagem.Length == 0)
                {

                    if (cargaIntegracao.Veiculo != null && !string.IsNullOrWhiteSpace(cargaIntegracao.Veiculo.Placa.Replace("-", "")))
                    {
                        int codigoEmpresa = preCarga.Empresa?.Codigo ?? 0;

                        Dominio.Entidades.Veiculo veiculo = repVeiculo.BuscarPorPlacaVarrendoFiliais(codigoEmpresa, cargaIntegracao.Veiculo.Placa.Replace("-", ""));
                        if (veiculo != null)
                            preCarga.Veiculo = veiculo;

                        if (preCarga.Veiculo != null)
                        {
                            if (cargaIntegracao.Veiculo.Reboques != null && cargaIntegracao.Veiculo.Reboques.Count > 0)
                            {
                                if (preCarga.VeiculosVinculados != null)
                                    preCarga.VeiculosVinculados.Clear();

                                preCarga.VeiculosVinculados = new List<Dominio.Entidades.Veiculo>();

                                foreach (Dominio.ObjetosDeValor.Embarcador.Frota.Veiculo reboqueIntegracao in cargaIntegracao.Veiculo.Reboques)
                                {
                                    Dominio.Entidades.Veiculo reboque = repVeiculo.BuscarPorPlacaVarrendoFiliais(codigoEmpresa, reboqueIntegracao.Placa.Replace("-", ""));
                                    if (reboque != null)
                                        preCarga.VeiculosVinculados.Add(reboque);
                                }
                            }
                            else if (preCarga.Veiculo != null && preCarga.Veiculo.VeiculosVinculados != null)
                            {
                                if (preCarga.Veiculo.TipoVeiculo == "1")
                                {
                                    if (preCarga.Veiculo.VeiculosTracao != null && preCarga.Veiculo.VeiculosTracao.Count > 0)
                                    {
                                        preCarga.Veiculo = preCarga.Veiculo.VeiculosTracao.FirstOrDefault();
                                    }
                                }

                                if (preCarga.VeiculosVinculados != null)
                                    preCarga.VeiculosVinculados.Clear();

                                preCarga.VeiculosVinculados = new List<Dominio.Entidades.Veiculo>();

                                foreach (Dominio.Entidades.Veiculo reboque in preCarga.Veiculo.VeiculosVinculados)
                                    preCarga.VeiculosVinculados.Add(reboque);

                            }
                        }
                    }

                    if (preCarga.Motoristas != null)
                        preCarga.Motoristas.Clear();

                    preCarga.Motoristas = new List<Dominio.Entidades.Usuario>();

                    if (cargaIntegracao.Motoristas != null)
                    {
                        Repositorio.Usuario repUsuario = new Repositorio.Usuario(_unitOfWork);
                        foreach (Dominio.ObjetosDeValor.Embarcador.Carga.Motorista motoristaIntegracao in cargaIntegracao.Motoristas)
                        {
                            Dominio.Entidades.Usuario motorista = repUsuario.BuscarMotoristaPorCPF(Utilidades.String.OnlyNumbers(motoristaIntegracao.CPF));
                            if (motorista != null)
                                preCarga.Motoristas.Add(motorista);
                        }
                    }
                    else
                    {
                        if (preCarga.Veiculo != null)
                        {
                            Dominio.Entidades.Usuario veiculoMotorista = repVeiculoMotorista.BuscarMotoristaPrincipal(preCarga.Veiculo.Codigo);
                            if (veiculoMotorista != null)
                                preCarga.Motoristas.Add(veiculoMotorista);
                        }
                    }

                    preCarga.SituacaoPreCarga = SituacaoPreCarga.AguardandoGeracaoCarga;
                    preCarga.DataCriacaoPreCarga = DateTime.Now;

                    repPreCarga.Inserir(preCarga);

                    Carga.CargaDadosSumarizados servicoCargaDadosSumarizados = new Carga.CargaDadosSumarizados(_unitOfWork);

                    servicoCargaDadosSumarizados.AlterarDadosSumarizadosPreCarga(preCarga, _unitOfWork, tipoServicoMultisoftware);
                }
            }

            return preCarga;
        }

        public Dominio.Entidades.Embarcador.PreCargas.PreCarga CriarPreCarga(Dominio.ObjetosDeValor.Embarcador.PreCarga.PreCarga objPreCarga, List<Dominio.ObjetosDeValor.Embarcador.Pessoas.Pessoa> objDestinatarios, DateTime dataHoraEntrega, ref string mensagem, AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware tipoServicoMultisoftware)
        {
            Repositorio.Embarcador.PreCargas.PreCarga repPreCarga = new Repositorio.Embarcador.PreCargas.PreCarga(_unitOfWork);
            Repositorio.Embarcador.Filiais.Filial repFilial = new Repositorio.Embarcador.Filiais.Filial(_unitOfWork);
            Repositorio.Embarcador.Cargas.TipoDeCarga repTipoDeCarga = new Repositorio.Embarcador.Cargas.TipoDeCarga(_unitOfWork);
            Repositorio.Embarcador.Cargas.ModeloVeicularCarga repModeloVeicular = new Repositorio.Embarcador.Cargas.ModeloVeicularCarga(_unitOfWork);
            Carga.CargaDadosSumarizados servicoCargaDadosSumarizados = new Carga.CargaDadosSumarizados(_unitOfWork);
            Servicos.Cliente serCliente = new Cliente(_unitOfWork.StringConexao);

            Dominio.Entidades.Embarcador.PreCargas.PreCarga preCarga = new Dominio.Entidades.Embarcador.PreCargas.PreCarga();
            preCarga.DataPrevisaoEntrega = dataHoraEntrega;

            if (objPreCarga.Filial != null)
            {
                preCarga.Filial = repFilial.buscarPorCodigoEmbarcador(objPreCarga.Filial.CodigoIntegracao);
                if (preCarga.Filial == null)
                    mensagem = "Não existe uma filial com código de integração " + objPreCarga.Filial.CodigoIntegracao + " na base da Multisoftware. ";
            }

            if (objPreCarga.TipoCargaEmbarcador != null)
            {
                preCarga.TipoDeCarga = repTipoDeCarga.BuscarPorCodigoEmbarcador(objPreCarga.TipoCargaEmbarcador.CodigoIntegracao);
                if (preCarga.TipoDeCarga == null)
                    mensagem += "Não existe um tipo de carga com código de integração " + objPreCarga.TipoCargaEmbarcador.CodigoIntegracao + " na base da Multisoftware. ";
            }

            if (objPreCarga.ModeloVeicular != null)
            {
                preCarga.ModeloVeicularCarga = repModeloVeicular.buscarPorCodigoIntegracao(objPreCarga.ModeloVeicular.CodigoIntegracao);
                if (preCarga.ModeloVeicularCarga == null)
                    mensagem += "Não existe um modelo veicular com código de integração " + objPreCarga.ModeloVeicular.CodigoIntegracao + " na base da Multisoftware. ";
            }
            else
            {
                Repositorio.Embarcador.Cargas.TipoCargaModeloVeicular repTipoCargaModeloVeicular = new Repositorio.Embarcador.Cargas.TipoCargaModeloVeicular(_unitOfWork);

                List<Dominio.Entidades.Embarcador.Cargas.TipoCargaModeloVeicular> tipoCargaModelosVeiculares = repTipoCargaModeloVeicular.ConsultarPorTipoCarga(preCarga.TipoDeCarga?.Codigo ?? 0);
                List<Dominio.Entidades.Embarcador.Cargas.TipoCargaModeloVeicular> modelosComNumeroPaletesCompativel = (from obj in tipoCargaModelosVeiculares where (obj.ModeloVeicularCarga.NumeroPaletes != null && obj.ModeloVeicularCarga.NumeroPaletes.Value == objPreCarga.NumeroPallets) select obj).ToList();

                int numeroModeloComMenosPaletes = 0;

                if (modelosComNumeroPaletesCompativel.Count == 0)
                {
                    List<Dominio.Entidades.Embarcador.Cargas.TipoCargaModeloVeicular> modelosSuportamNumeroPaletes = new List<Dominio.Entidades.Embarcador.Cargas.TipoCargaModeloVeicular>();
                    foreach (Dominio.Entidades.Embarcador.Cargas.TipoCargaModeloVeicular tipoCargaModeloVeicular in tipoCargaModelosVeiculares)
                    {
                        if (tipoCargaModeloVeicular.ModeloVeicularCarga.NumeroPaletes != null && tipoCargaModeloVeicular.ModeloVeicularCarga.NumeroPaletes.Value >= objPreCarga.NumeroPallets)
                        {
                            modelosSuportamNumeroPaletes.Add(tipoCargaModeloVeicular);

                            if (numeroModeloComMenosPaletes == 0 || numeroModeloComMenosPaletes > tipoCargaModeloVeicular.ModeloVeicularCarga.NumeroPaletes.Value)
                                numeroModeloComMenosPaletes = tipoCargaModeloVeicular.ModeloVeicularCarga.NumeroPaletes.Value;
                        }
                    }

                    if (modelosSuportamNumeroPaletes.Count > 0 && numeroModeloComMenosPaletes > 0)
                        modelosComNumeroPaletesCompativel = (from obj in modelosSuportamNumeroPaletes where obj.ModeloVeicularCarga.NumeroPaletes.Value == numeroModeloComMenosPaletes select obj).ToList();
                }

                preCarga.ModeloVeicularCarga = (from obj in modelosComNumeroPaletesCompativel orderby obj.Posicao select obj.ModeloVeicularCarga).FirstOrDefault();

                if (preCarga.ModeloVeicularCarga == null)
                    mensagem = "Não foi encontrado um modelo veicular para este tipo de carga que suporte a quantidade de pallets informada. ";
            }

            preCarga.NumeroPreCarga = objPreCarga.NumeroPreCarga; //.Trim();
            preCarga.DataCriacaoPreCarga = DateTime.Now;

            preCarga.Destinatarios = new List<Dominio.Entidades.Cliente>();

            foreach (Dominio.ObjetosDeValor.Embarcador.Pessoas.Pessoa objDestinatario in objDestinatarios)
            {
                Dominio.ObjetosDeValor.RetornoVerificacaoCliente retornoPessoa = serCliente.ConverterObjetoValorPessoa(objDestinatario, "Destinatário", _unitOfWork, 0, false);
                if (retornoPessoa.Status)
                {
                    preCarga.Destinatarios.Add(retornoPessoa.cliente);
                }
                else
                    mensagem += retornoPessoa.Mensagem;

            }

            //string objetoJson = Newtonsoft.Json.JsonConvert.SerializeObject(objPreCarga);
            //ArmazenarLogParametros(objetoJson, _unitOfWork);

            repPreCarga.Inserir(preCarga);

            servicoCargaDadosSumarizados.AlterarDadosSumarizadosPreCarga(preCarga, _unitOfWork, tipoServicoMultisoftware);

            return preCarga;
        }

        public void ConfirmarAlteracaoDadosPreCarga(Dominio.Entidades.Embarcador.Cargas.Carga carga, Dominio.ObjetosDeValor.Embarcador.Auditoria.Auditado auditado)
        {
            new Carga.MensagemAlertaCarga(_unitOfWork, auditado).Confirmar(carga, TipoMensagemAlerta.AlteracaoDadosPreCarga);
        }

        public bool IsAlteracoesDadosPreCargaSemConfirmacao(Dominio.Entidades.Embarcador.Cargas.Carga carga)
        {
            Servicos.Embarcador.Carga.MensagemAlertaCarga servicoMensagemAlerta = new Servicos.Embarcador.Carga.MensagemAlertaCarga(_unitOfWork);

            return servicoMensagemAlerta.IsMensagemSemConfirmacao(carga, TipoMensagemAlerta.AlteracaoDadosPreCarga);
        }

        public bool RemoverCargaDaPreCarga(Dominio.Entidades.Embarcador.PreCargas.PreCarga preCarga, out string erro)
        {
            erro = "";
            Repositorio.Embarcador.PreCargas.PreCarga repPreCarga = new Repositorio.Embarcador.PreCargas.PreCarga(_unitOfWork);
            Repositorio.Embarcador.Cargas.Carga repCarga = new Repositorio.Embarcador.Cargas.Carga(_unitOfWork);

            if (preCarga.Carga == null)
            {
                erro = "A Pré Carga não possui nenhuma Carga";
                return false;
            }

            int codigoCarga = preCarga.Carga.Codigo;
            preCarga.Carga = null;
            preCarga.SituacaoPreCarga = SituacaoPreCarga.AguardandoGeracaoCarga;

            repPreCarga.Atualizar(preCarga);

            RemoveCargaDaGestaoPatio(preCarga, codigoCarga);

            return true;
        }

        public bool VincularCargaPreCarga(Dominio.Entidades.Embarcador.PreCargas.PreCarga preCarga, Dominio.Entidades.Embarcador.Cargas.Carga carga, AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware tipoServicoMultisoftware, out string erro)
        {
            erro = "";
            Repositorio.Embarcador.PreCargas.PreCarga repPreCarga = new Repositorio.Embarcador.PreCargas.PreCarga(_unitOfWork);
            Repositorio.Embarcador.Cargas.Carga repCarga = new Repositorio.Embarcador.Cargas.Carga(_unitOfWork);
            Servicos.Embarcador.Carga.Carga serCarga = new Carga.Carga(_unitOfWork);

            preCarga.Carga = carga;
            preCarga.SituacaoPreCarga = SituacaoPreCarga.CargaGerada;

            //carga.CodigoCargaEmbarcador = preCarga.NumeroPreCarga;
            if (carga.Empresa == null)
                carga.Empresa = preCarga.Empresa;

            if (carga.Veiculo == null)
                carga.Veiculo = preCarga.Veiculo;

            if (carga.TipoDeCarga == null)
                carga.TipoDeCarga = preCarga.TipoDeCarga;

            if (carga.FaixaTemperatura == null)
                carga.FaixaTemperatura = preCarga.FaixaTemperatura;

            if (carga.TipoOperacao == null)
            {
                if (preCarga.Pedidos != null && preCarga.Pedidos.Count > 0)
                    carga.TipoOperacao = preCarga.Pedidos.FirstOrDefault().TipoOperacao;
            }

            foreach (var veic in preCarga.VeiculosVinculados)
            {
                if (!carga.VeiculosVinculados.Contains(veic))
                    carga.VeiculosVinculados.Add(veic);
            }

            foreach (var motorista in preCarga.Motoristas)
            {
                if (!carga.Motoristas.Contains(motorista))
                    carga.Motoristas.Add(motorista);
            }

            repPreCarga.Atualizar(preCarga);
            repCarga.Atualizar(carga);


            return true;
        }

        public async Task<bool> VincularCargaPreCargaAsync(Dominio.Entidades.Embarcador.PreCargas.PreCarga preCarga, Dominio.Entidades.Embarcador.Cargas.Carga carga, AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware tipoServicoMultisoftware)
        {
            Repositorio.Embarcador.PreCargas.PreCarga repPreCarga = new Repositorio.Embarcador.PreCargas.PreCarga(_unitOfWork);
            Repositorio.Embarcador.Cargas.Carga repCarga = new Repositorio.Embarcador.Cargas.Carga(_unitOfWork);

            preCarga.Carga = carga;
            preCarga.SituacaoPreCarga = SituacaoPreCarga.CargaGerada;

            if (carga.Empresa == null)
                carga.Empresa = preCarga.Empresa;

            if (carga.Veiculo == null)
                carga.Veiculo = preCarga.Veiculo;

            if (carga.TipoDeCarga == null)
                carga.TipoDeCarga = preCarga.TipoDeCarga;

            if (carga.FaixaTemperatura == null)
                carga.FaixaTemperatura = preCarga.FaixaTemperatura;

            if (carga.TipoOperacao == null)
            {
                if (preCarga.Pedidos != null && preCarga.Pedidos.Count > 0)
                    carga.TipoOperacao = preCarga.Pedidos.FirstOrDefault().TipoOperacao;
            }

            foreach (var veic in preCarga.VeiculosVinculados)
            {
                if (!carga.VeiculosVinculados.Contains(veic))
                    carga.VeiculosVinculados.Add(veic);
            }

            foreach (var motorista in preCarga.Motoristas)
            {
                if (!carga.Motoristas.Contains(motorista))
                    carga.Motoristas.Add(motorista);
            }

            await repPreCarga.AtualizarAsync(preCarga);
            await repCarga.AtualizarAsync(carga);


            return true;
        }

        public bool VincularPreCargaACarga(Dominio.Entidades.Embarcador.PreCargas.PreCarga preCarga, Dominio.Entidades.Embarcador.Cargas.Carga carga, List<Dominio.Entidades.Embarcador.Cargas.CargaPedido> cargaPedidos, Dominio.Entidades.Embarcador.Configuracao.ConfiguracaoTMS configuracaoTMS, AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware tipoServicoMultisoftware, AdminMultisoftware.Dominio.Entidades.Pessoas.Cliente ClienteMultisoftware, out string erro)
        {
            erro = "";
            Repositorio.Embarcador.PreCargas.PreCarga repPreCarga = new Repositorio.Embarcador.PreCargas.PreCarga(_unitOfWork);
            Repositorio.Embarcador.Cargas.Carga repCarga = new Repositorio.Embarcador.Cargas.Carga(_unitOfWork);
            Servicos.Embarcador.Carga.Carga serCarga = new Carga.Carga(_unitOfWork);

            preCarga.Carga = carga;
            preCarga.SituacaoPreCarga = SituacaoPreCarga.CargaGerada;

            //carga.CodigoCargaEmbarcador = preCarga.NumeroPreCarga;
            carga.Empresa = preCarga.Empresa;
            carga.Veiculo = preCarga.Veiculo;
            carga.TipoOperacao = preCarga.TipoOperacao;
            carga.TipoDeCarga = preCarga.TipoDeCarga;
            carga.FaixaTemperatura = preCarga.FaixaTemperatura;

            carga.VeiculosVinculados.Clear();
            foreach (var veic in preCarga.VeiculosVinculados)
                carga.VeiculosVinculados.Add(veic);

            carga.Motoristas.Clear();
            foreach (var motorista in preCarga.Motoristas)
                carga.Motoristas.Add(motorista);

            repPreCarga.Atualizar(preCarga);
            repCarga.Atualizar(carga);

            CriarAtualizarGestaoPatio(preCarga, tipoServicoMultisoftware);
            AtualizarJanelaCarregamento(preCarga);

            serCarga.FecharCarga(carga, _unitOfWork, tipoServicoMultisoftware, ClienteMultisoftware);

            Servicos.Embarcador.Carga.RotaFrete.SetarRotaFreteCarga(carga, cargaPedidos, configuracaoTMS, _unitOfWork, tipoServicoMultisoftware);

            return true;
        }

        public bool VincularPrimeiraPreCarga(Dominio.Entidades.Embarcador.Cargas.Carga carga, AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware tipoServicoMultisoftware)
        {
            if ((carga.ModeloVeicularCarga == null) || !carga.DataCarregamentoCarga.HasValue)
                return false;

            Repositorio.Embarcador.Cargas.Carga repositorioCarga = new Repositorio.Embarcador.Cargas.Carga(_unitOfWork);
            Repositorio.Embarcador.PreCargas.PreCarga repositorioPreCarga = new Repositorio.Embarcador.PreCargas.PreCarga(_unitOfWork);
            Carga.CargaDadosSumarizados servicoCargaDadosSumarizados = new Carga.CargaDadosSumarizados(_unitOfWork);
            Logistica.FilaCarregamentoVeiculo servicoFilaCarregamentoVeiculo = new Logistica.FilaCarregamentoVeiculo(_unitOfWork, Logistica.FilaCarregamentoVeiculo.ObterOrigemAlteracaoFilaCarregamento(tipoServicoMultisoftware));

            List<Dominio.ObjetosDeValor.Localidade> localidadesDestino = servicoCargaDadosSumarizados.ObterDestinos(carga, _unitOfWork, tipoServicoMultisoftware);
            List<int> codigosDestinos = localidadesDestino.Select(localidadeDestino => localidadeDestino.Codigo).Distinct().ToList();
            Dominio.Entidades.Embarcador.PreCargas.PreCarga preCarga = repositorioPreCarga.BuscarPrimeiraAguardandoGeracaoCarga(carga.Filial.Codigo, carga.ModeloVeicularCarga.Codigo, carga.TipoDeCarga?.Codigo ?? 0, carga.DataCarregamentoCarga.Value, codigosDestinos);

            if (preCarga == null)
                return false;

            preCarga.Carga = carga;
            preCarga.SituacaoPreCarga = SituacaoPreCarga.CargaGerada;

            carga.Empresa = preCarga.Empresa;
            carga.TipoDeCarga = preCarga.TipoDeCarga;
            carga.Veiculo = preCarga.Veiculo;

            if (preCarga.TipoOperacao != null)
                carga.TipoOperacao = preCarga.TipoOperacao;

            if (carga.VeiculosVinculados == null)
                carga.VeiculosVinculados = new List<Dominio.Entidades.Veiculo>();
            else
                carga.VeiculosVinculados.Clear();

            if (carga.Motoristas == null)
                carga.Motoristas = new List<Dominio.Entidades.Usuario>();
            else
                carga.Motoristas.Clear();

            foreach (Dominio.Entidades.Veiculo reboque in preCarga.VeiculosVinculados)
                carga.VeiculosVinculados.Add(reboque);

            foreach (Dominio.Entidades.Usuario motorista in preCarga.Motoristas)
                carga.Motoristas.Add(motorista);

            repositorioPreCarga.Atualizar(preCarga);
            repositorioCarga.Atualizar(carga);
            servicoFilaCarregamentoVeiculo.AdicionarCargaPorPreCarga(preCarga, tipoServicoMultisoftware);

            return true;
        }

        public Dominio.Entidades.Embarcador.Cargas.Carga GerarCarga(Dominio.Entidades.Embarcador.PreCargas.PreCarga preCarga, bool usarNumeroPreCarga, AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware tipoServicoMultisoftware, AdminMultisoftware.Dominio.Entidades.Pessoas.Cliente ClienteMultisoftware, out string retorno)
        {
            retorno = "";

            Repositorio.Embarcador.PreCargas.PreCarga repPreCarga = new Repositorio.Embarcador.PreCargas.PreCarga(_unitOfWork);
            Repositorio.Embarcador.Pedidos.Pedido repPedido = new Repositorio.Embarcador.Pedidos.Pedido(_unitOfWork);
            Repositorio.Embarcador.Cargas.Carga repCarga = new Repositorio.Embarcador.Cargas.Carga(_unitOfWork);
            Repositorio.Embarcador.Cargas.CargaPedido repCargaPedido = new Repositorio.Embarcador.Cargas.CargaPedido(_unitOfWork);
            Repositorio.Embarcador.Pedidos.PedidoXMLNotaFiscal repPedidoXMLNotaFiscal = new Repositorio.Embarcador.Pedidos.PedidoXMLNotaFiscal(_unitOfWork);
            Repositorio.Embarcador.Configuracao.ConfiguracaoTMS repConfiguracaoTMS = new Repositorio.Embarcador.Configuracao.ConfiguracaoTMS(_unitOfWork);
            Repositorio.Embarcador.Cargas.CargaPedidoXMLNotaFiscalParcial repCargaPedidoXMLNotaFiscalParcial = new Repositorio.Embarcador.Cargas.CargaPedidoXMLNotaFiscalParcial(_unitOfWork);

            Dominio.Entidades.Embarcador.Configuracao.ConfiguracaoTMS configuracaoTMS = repConfiguracaoTMS.BuscarConfiguracaoPadrao();

            Servicos.Embarcador.Carga.Carga serCarga = new Servicos.Embarcador.Carga.Carga(_unitOfWork);
            Servicos.Embarcador.Carga.Frete serFrete = new Carga.Frete(_unitOfWork, tipoServicoMultisoftware);
            Servicos.Embarcador.Carga.CargaMotorista servicoCargaMotorista = new Servicos.Embarcador.Carga.CargaMotorista(_unitOfWork);

            List<Dominio.Entidades.Embarcador.Pedidos.Pedido> pedidos = repPedido.BuscarPorPreCarga(preCarga.Codigo);

            if (pedidos.Count == 0)
            {
                preCarga.SituacaoPreCarga = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoPreCarga.Cancelada;
                repPreCarga.Atualizar(preCarga);
                return null;
            }

            Repositorio.Empresa repEmpresa = new Repositorio.Empresa(_unitOfWork);

            Dominio.Entidades.Embarcador.Cargas.Carga carga = new Dominio.Entidades.Embarcador.Cargas.Carga();
            carga.Filial = preCarga.Filial;
            carga.HorarioCarregamentoInformadoNoPedido = preCarga.HorarioCarregamentoInformadoNoPedido;

            if (!usarNumeroPreCarga)
            {
                if (configuracaoTMS.NumeroCargaSequencialUnico)
                    carga.NumeroSequenciaCarga = Servicos.Embarcador.Cargas.CargaSequencial.GetInstance().ObterProximoNumeroSequencial(_unitOfWork);
                else
                    carga.NumeroSequenciaCarga = Servicos.Embarcador.Cargas.CargaSequencial.GetInstance().ObterProximoNumeroSequencial(_unitOfWork, carga.Filial?.Codigo ?? 0);

                carga.CodigoCargaEmbarcador = carga.NumeroSequenciaCarga.ToString();
            }
            else
            {
                Dominio.Entidades.Embarcador.Cargas.Carga cargaExiste = null;
                if (tipoServicoMultisoftware == AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiTMS)
                    cargaExiste = repCarga.BuscarPorCodigoEmbarcador(preCarga.NumeroPreCarga);
                else
                {
                    cargaExiste = repCarga.BuscarPorCodigoCargaEmbarcador(preCarga.NumeroPreCarga, preCarga.Filial?.Codigo ?? 0);
                }

                if (cargaExiste != null)
                {
                    retorno = "Já existe uma carga gerada com o número " + preCarga.NumeroPreCarga;
                    return null;
                }
                carga.CodigoCargaEmbarcador = preCarga.NumeroPreCarga;
            }

            carga.Empresa = preCarga.Empresa;
            carga.ModeloVeicularCarga = preCarga.ModeloVeicularCarga;
            carga.CargaDePreCarga = preCarga.CargaDePreCarga;
            carga.CargaPreCarga = preCarga.CargaPreCarga;
            carga.DataInicioViagemPrevista = preCarga.DataPrevisaoInicioViagem;
            carga.TipoDeCarga = preCarga.TipoDeCarga;
            carga.NumeroDoca = pedidos.Select(o => o.NumeroDoca).FirstOrDefault();
            carga.Veiculo = preCarga.Veiculo;
            carga.VeiculosVinculados = new List<Dominio.Entidades.Veiculo>();
            carga.FaixaTemperatura = preCarga.FaixaTemperatura;

            if (carga.CargaPreCarga == null)
                carga.CargaPreCarga = repCarga.BuscarPorCodigoVinculado(carga.CodigoCargaEmbarcador);

            if (carga.TipoOperacao == null)
                carga.ExigeNotaFiscalParaCalcularFrete = configuracaoTMS.ExigirNotaFiscalParaCalcularFreteCarga;
            else
            {
                carga.ExigeNotaFiscalParaCalcularFrete = carga.TipoOperacao.ExigeNotaFiscalParaCalcularFrete;
                carga.OrdemRoteirizacaoDefinida = carga.TipoOperacao.ManterOrdemAoRoteirizarAgendaEntrega;
            }

            int codigoPedidoInicial = pedidos.OrderBy(obj => obj.OrdemColetaProgramada).FirstOrDefault().Codigo;
            carga.DataFimViagemPrevista = pedidos.OrderByDescending(obj => obj.DataPrevisaoChegadaDestinatario).FirstOrDefault().DataPrevisaoChegadaDestinatario;
            carga.TipoOperacao = pedidos.FirstOrDefault().TipoOperacao;

            if (carga.CargaPreCarga?.TipoOperacao.UtilizarTipoOperacaoPreCargaAoGerarCarga ?? false)
                carga.TipoOperacao = carga.CargaPreCarga.TipoOperacao;

            retorno = serCarga.CriarCargaPorPedidos(ref carga, pedidos, tipoServicoMultisoftware, null, _unitOfWork, configuracaoTMS, null, false, NumeroReboque.SemReboque, TipoCarregamentoPedido.Normal);

            if (carga.CargaPreCarga == null)
            {
                if (preCarga.Veiculo != null)
                {
                    foreach (Dominio.Entidades.Veiculo reboque in preCarga.Veiculo.VeiculosVinculados)
                        carga.VeiculosVinculados.Add(reboque);
                }

                if (preCarga.VeiculosVinculados?.Count > 0)
                {
                    foreach (Dominio.Entidades.Veiculo reboque in preCarga.VeiculosVinculados)
                        carga.VeiculosVinculados.Add(reboque);
                }

                if (preCarga.Motoristas?.Count > 0)
                    servicoCargaMotorista.AdicionarMotoristas(carga, preCarga.Motoristas.ToList());
            }
            else
            {
                carga.Empresa = carga.CargaPreCarga.Empresa;
                carga.Veiculo = carga.CargaPreCarga.Veiculo;

                foreach (Dominio.Entidades.Veiculo reboque in carga.CargaPreCarga.VeiculosVinculados.ToList())
                    carga.VeiculosVinculados.Add(reboque);

                servicoCargaMotorista.AdicionarMotoristas(carga, carga.CargaPreCarga.Motoristas.ToList());
            }

            if (carga.ExigeNotaFiscalParaCalcularFrete)
            {
                serFrete.CriarCargaComponentes(carga, tipoServicoMultisoftware, false, _unitOfWork);
                serFrete.CriarCargaComponentes(carga, tipoServicoMultisoftware, true, _unitOfWork);
            }

            if (string.IsNullOrWhiteSpace(retorno))
            {
                if (preCarga.MotivoImportacaoPedidoAtrasada != null)
                    AdicionarMensagemAtrasoImportacaoPreCarga(preCarga, carga);

                List<Dominio.Entidades.Embarcador.Cargas.CargaPedido> cargaPedidos = repCargaPedido.BuscarPorCarga(carga.Codigo);
                Dominio.Entidades.Embarcador.Cargas.CargaPedido pedidoInicial = cargaPedidos.Where(x => x.Pedido.Codigo == codigoPedidoInicial).FirstOrDefault();
                pedidoInicial.InicioDaCarga = true;
                repCargaPedido.Atualizar(pedidoInicial);

                //List<Dominio.Entidades.Embarcador.Cargas.CargaPedido> cargaPedidos = repCargaPedido.BuscarPorCarga(carga.Codigo);
                //foreach (Dominio.Entidades.Embarcador.Cargas.CargaPedido cargaPedido in cargaPedidos)
                //{
                //    if (cargaPedido.Pedido.Codigo == codigoPedidoInicial)
                //    {
                //        cargaPedido.InicioDaCarga = true;
                //        repCargaPedido.Atualizar(cargaPedido);
                //        break;
                //    }
                //}

                Servicos.Embarcador.Carga.CargaPedido serCargaPedido = new Servicos.Embarcador.Carga.CargaPedido(_unitOfWork);

                carga.ValorICMS = 0;
                carga.ValorFrete = 0;
                carga.ValorFreteAPagar = 0;
                carga.ValorIBSEstadual = 0;
                carga.ValorIBSMunicipal = 0;
                carga.ValorCBS = 0;
                carga.DataPrevisaoTerminoCarga = pedidos.LastOrDefault().DataPrevisaoChegadaDestinatario;
                bool enviouTodas = true;

                foreach (Dominio.Entidades.Embarcador.Cargas.CargaPedido cp in cargaPedidos)
                {
                    if (repPedidoXMLNotaFiscal.ContarPorCargaPedido(cp.Codigo) <= 0)
                        enviouTodas = false;
                    else
                    {
                        cp.SituacaoEmissao = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoNF.NFEnviada;
                        repCargaPedido.Atualizar(cp);
                    }

                    if (cp.ValorFrete > 0)
                    {
                        carga.ValorICMS += cp.ValorICMS;
                        carga.ValorFrete += cp.ValorFrete;
                        carga.ValorFreteAPagar += cp.ValorFreteAPagar;
                        carga.ValorIBSEstadual += cp.ValorIBSEstadual;
                        carga.ValorIBSMunicipal += cp.ValorIBSMunicipal;
                        carga.ValorCBS += cp.ValorCBS;
                    }

                    List<Dominio.Entidades.Embarcador.Cargas.CargaPedidoQuantidades> cargaPedidoQuantidades = new List<Dominio.Entidades.Embarcador.Cargas.CargaPedidoQuantidades>();
                    decimal peso = 0;
                    peso += repPedidoXMLNotaFiscal.BuscarPesoPorCargaPedido(cp.Codigo, new List<int>(), configuracaoTMS.NaoUsarPesoNotasPallet);

                    if (peso > 0m)
                    {
                        Dominio.Entidades.Embarcador.Cargas.CargaPedidoQuantidades cargaPedidoQuantidade = new Dominio.Entidades.Embarcador.Cargas.CargaPedidoQuantidades();
                        cargaPedidoQuantidade.CargaPedido = cp;
                        cargaPedidoQuantidade.Quantidade = peso;
                        cargaPedidoQuantidade.Unidade = Dominio.Enumeradores.UnidadeMedida.KG;

                        cargaPedidoQuantidades.Add(cargaPedidoQuantidade);
                    }

                    int volumes = repPedidoXMLNotaFiscal.BuscarVolumesPorCargaPedido(cp.Codigo);

                    if (volumes > 0)
                    {
                        Dominio.Entidades.Embarcador.Cargas.CargaPedidoQuantidades cargaPedidoQuantidade = new Dominio.Entidades.Embarcador.Cargas.CargaPedidoQuantidades();
                        cargaPedidoQuantidade.CargaPedido = cp;
                        cargaPedidoQuantidade.Quantidade = volumes;
                        cargaPedidoQuantidade.Unidade = Dominio.Enumeradores.UnidadeMedida.UN;
                        cargaPedidoQuantidades.Add(cargaPedidoQuantidade);
                    }

                    serCargaPedido.AdicionarCargaPedidoQuantidades(cargaPedidoQuantidades, cp, _unitOfWork);


                    if (cp.Pedido.DataCarregamentoPedido.HasValue)
                    {
                        carga.DataCarregamentoCarga = cp.Pedido.DataCarregamentoPedido;
                        //carga.DataCriacaoCarga = cp.Pedido.DataCarregamentoPedido.Value;
                    }
                }

                if (configuracaoTMS.ValidarNotasParciaisEnvioEmissao)
                {
                    if (repCargaPedidoXMLNotaFiscalParcial.VerificarSeExisteNotaParcialSemNota(carga.Codigo))
                        enviouTodas = false;
                }

                if (carga.ValorFrete > 0)
                {
                    carga.ValorFreteLiquido = Math.Round(carga.ValorFrete, 2, MidpointRounding.AwayFromZero);
                    carga.TipoFreteEscolhido = Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoFreteEscolhido.Embarcador;
                    Dominio.ObjetosDeValor.Embarcador.Frete.ComposicaoFrete composicaoFrete = Servicos.Embarcador.Frete.ComposicaoFrete.ObterComposicaoFrete("Valor Informado Pelo Embarcador", " Valor Informado = " + carga.ValorFrete.ToString("n2"), carga.ValorFrete, Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoParametroBaseTabelaFrete.ValorFreteLiquido, Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoCampoValorTabelaFrete.ValorFixo, "Valor informado pelo Embarcador", 0, carga.ValorFrete);
                    Servicos.Embarcador.Carga.ComposicaoFrete.ComposicaoFrete.SetarComposicaoFrete(carga, null, null, null, false, composicaoFrete, _unitOfWork, null);
                }

                if (!configuracaoTMS.NaoExecutarFecharCarga)
                {
                    serCarga.FecharCarga(carga, _unitOfWork, tipoServicoMultisoftware, ClienteMultisoftware);

                    CalculoFreteCotacaoPedido(carga, cargaPedidos, configuracaoTMS, tipoServicoMultisoftware, _unitOfWork, false);
                }
                else
                {
                    Servicos.Embarcador.Carga.CargaDadosSumarizados serCargaDadosSumarizados = new Servicos.Embarcador.Carga.CargaDadosSumarizados(_unitOfWork);
                    serCargaDadosSumarizados.AlterarDadosSumarizadosCarga(ref carga, cargaPedidos, configuracaoTMS, _unitOfWork, tipoServicoMultisoftware);
                    carga.SituacaoCarga = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoCarga.Nova;

                    if (carga.TipoOperacao != null && carga.TipoOperacao.FretePorContadoCliente)
                        carga.TipoFreteEscolhido = Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoFreteEscolhido.Cliente;

                    if (!carga.ExigeNotaFiscalParaCalcularFrete)
                    {
                        if (carga.Empresa != null && carga.TipoDeCarga != null && (carga.ModeloVeicularCarga != null || (carga.TipoOperacao != null && carga.TipoOperacao.NaoExigeVeiculoParaEmissao)))
                            serCarga.InformarSituacaoCargaFreteValido(ref carga, tipoServicoMultisoftware, _unitOfWork);
                    }
                    else
                    {
                        if (carga.Empresa != null && carga.TipoDeCarga != null && (carga.ModeloVeicularCarga != null && carga.Veiculo != null && (preCarga.Motoristas != null && preCarga.Motoristas.Count > 0) || (carga.TipoOperacao != null && carga.TipoOperacao.NaoExigeVeiculoParaEmissao)))
                        {
                            if (enviouTodas)
                                serCarga.InformarSituacaoCargaFreteValido(ref carga, tipoServicoMultisoftware, _unitOfWork);
                            else
                                carga.SituacaoCarga = SituacaoCarga.AgNFe;
                        }

                    }
                }

                if (enviouTodas && carga.SituacaoCarga == SituacaoCarga.AgNFe && carga.TipoOperacao != null && carga.TipoOperacao.NaoExigeConformacaoDasNotasEmissao)
                {
                    carga.DataEnvioUltimaNFe = DateTime.Now;
                    carga.DataInicioEmissaoDocumentos = DateTime.Now;
                }

                preCarga.Carga = carga;
                carga.CargaFechada = true;
                Servicos.Log.TratarErro("14 - Fechou Carga (" + carga.Codigo + ") " + DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss.fff"), "FechamentoCarga");
                carga.Protocolo = carga.Codigo;

                if (carga.CargaPreCarga != null)
                {
                    if (configuracaoTMS.NaoExecutarFecharCarga)
                        TrocarPreCarga(carga.CargaPreCarga, carga, AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiEmbarcador, configuracaoTMS, ClienteMultisoftware);
                }
                else
                {
                    CriarAtualizarGestaoPatio(preCarga, tipoServicoMultisoftware);
                }

                repCarga.Atualizar(carga);
                preCarga.SituacaoPreCarga = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoPreCarga.CargaGerada;
                repPreCarga.Atualizar(preCarga);

                return carga;
            }
            return null;
        }

        public void AtualizarJanelaCarregamento(Dominio.Entidades.Embarcador.PreCargas.PreCarga preCarga)
        {
            Repositorio.Embarcador.Cargas.CargaJanelaCarregamento repCargaJanelaCarregamento = new Repositorio.Embarcador.Cargas.CargaJanelaCarregamento(_unitOfWork);

            Dominio.Entidades.Embarcador.Cargas.CargaJanelaCarregamento janelaCarregamento = repCargaJanelaCarregamento.BuscarPorPreCarga(preCarga.Codigo);

            if (janelaCarregamento != null && janelaCarregamento.Carga == null)
            {
                janelaCarregamento.Carga = preCarga.Carga;
                repCargaJanelaCarregamento.Atualizar(janelaCarregamento);
            }
        }

        public void CriarAtualizarGestaoPatio(Dominio.Entidades.Embarcador.PreCargas.PreCarga preCarga, AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware tipoServicoMultisoftware)
        {
            Servicos.Embarcador.GestaoPatio.FluxoGestaoPatio servicoFluxoGestaoPatio = new Servicos.Embarcador.GestaoPatio.FluxoGestaoPatio(_unitOfWork);
            Dominio.Entidades.Embarcador.GestaoPatio.FluxoGestaoPatio fluxoGestaoPatio = servicoFluxoGestaoPatio.ObterFluxoGestaoPatio(preCarga);

            Repositorio.Embarcador.Cargas.CargaJanelaCarregamento repositorioCargaJanelaCarregamento = new Repositorio.Embarcador.Cargas.CargaJanelaCarregamento(_unitOfWork);
            Dominio.Entidades.Embarcador.Cargas.CargaJanelaCarregamento cargaJanelaCarregamento = repositorioCargaJanelaCarregamento.BuscarPorCarga(preCarga.Carga.Codigo);

            if (fluxoGestaoPatio == null && cargaJanelaCarregamento == null)
                servicoFluxoGestaoPatio.Adicionar(preCarga.Carga, tipoServicoMultisoftware);
            else if (fluxoGestaoPatio != null)
                servicoFluxoGestaoPatio.DefinirCargaPorPreCarga(preCarga);
        }

        public void ArmazenarLogParametros(string log, Repositorio.UnitOfWork unitOfWork)
        {
            string caminhoLog = Utilidades.IO.FileStorageService.Storage.Combine(Servicos.Embarcador.Configuracoes.ConfigurationInstance.GetInstance(unitOfWork).ObterConfiguracaoArquivo().CaminhoTempArquivosImportacao, Guid.NewGuid() + "_integracaoPreCarga.txt");

            Utilidades.IO.FileStorageService.Storage.WriteAllText(caminhoLog, log);
        }

        public string ConfirmarImportarPedido(dynamic registrosAlterados, Dominio.Entidades.Usuario usuario, Dominio.ObjetosDeValor.Embarcador.Auditoria.Auditado auditado)
        {
            string patternConversaoData = "dd/MM/yyyy HH:mm:ss";
            Repositorio.Cliente repCliente = new Repositorio.Cliente(_unitOfWork);
            Repositorio.Empresa repEmpresa = new Repositorio.Empresa(_unitOfWork);
            Repositorio.Embarcador.PreCargas.PreCarga repPreCarga = new Repositorio.Embarcador.PreCargas.PreCarga(_unitOfWork);
            Repositorio.Embarcador.Cargas.TipoDeCarga repTipoDeCarga = new Repositorio.Embarcador.Cargas.TipoDeCarga(_unitOfWork);
            Repositorio.Embarcador.Cargas.ModeloVeicularCarga repModeloVeicularCarga = new Repositorio.Embarcador.Cargas.ModeloVeicularCarga(_unitOfWork);
            Repositorio.Embarcador.Pedidos.TipoOperacao repTipoOperacao = new Repositorio.Embarcador.Pedidos.TipoOperacao(_unitOfWork);
            Repositorio.Embarcador.Pedidos.Pedido repPedido = new Repositorio.Embarcador.Pedidos.Pedido(_unitOfWork);
            Repositorio.Embarcador.Filiais.Filial repFilial = new Repositorio.Embarcador.Filiais.Filial(_unitOfWork);
            Repositorio.Embarcador.Configuracao.ConfiguracaoTMS repConfiguracaoTMS = new Repositorio.Embarcador.Configuracao.ConfiguracaoTMS(_unitOfWork);
            Servicos.Embarcador.GestaoPatio.FluxoGestaoPatio servicoFluxoGestaoPatio = new Servicos.Embarcador.GestaoPatio.FluxoGestaoPatio(_unitOfWork);

            string retorno = "";
            Dominio.Entidades.Embarcador.Configuracao.ConfiguracaoTMS configuracaoTMS = repConfiguracaoTMS.BuscarConfiguracaoPadrao();
            List<Dominio.ObjetosDeValor.Embarcador.PreCarga.ImportacaoPreCarga> importacaoPreCarga = new List<Dominio.ObjetosDeValor.Embarcador.PreCarga.ImportacaoPreCarga>();

            for (int i = 0; i < registrosAlterados.Count; i++)
            {
                var registro = registrosAlterados[i];
                Dominio.Entidades.Embarcador.PreCargas.PreCarga preCarga = repPreCarga.BuscarPorCodigo((int)registro.CodigoPreCarga, true);

                if (preCarga != null)
                {

                    _unitOfWork.Start();
                    Dominio.Entidades.Embarcador.GestaoPatio.FluxoGestaoPatio fluxoGestaoPatio = servicoFluxoGestaoPatio.ObterFluxoGestaoPatio(preCarga);

                    #region Converte Dados
                    DateTime? dataInicioViagem = null;
                    if (!string.IsNullOrWhiteSpace((string)registro.DataPrevisaoInicioViagem))
                    {
                        if (DateTime.TryParseExact((string)registro.DataPrevisaoInicioViagem, patternConversaoData, null, System.Globalization.DateTimeStyles.None, out DateTime dataInicioViagemAux))
                        {
                            dataInicioViagem = dataInicioViagemAux;
                        }
                        else
                        {
                            _unitOfWork.Rollback();
                            return "O início da viagem deve estar no padrão " + patternConversaoData;
                        }
                    }

                    DateTime? dataChegadaDestinatario = null;
                    if (!string.IsNullOrWhiteSpace((string)registro.PrevisaoChegadaDestinatario))
                    {
                        if (DateTime.TryParseExact((string)registro.PrevisaoChegadaDestinatario, patternConversaoData, null, System.Globalization.DateTimeStyles.None, out DateTime dataChegadaDestinatarioAux))
                            dataChegadaDestinatario = dataChegadaDestinatarioAux;
                        else
                        {
                            _unitOfWork.Rollback();
                            return "A chegada do destinatário deve estar no padrão " + patternConversaoData;
                        }
                    }

                    DateTime? dataSaidaDestinatario = null;
                    if (!string.IsNullOrWhiteSpace((string)registro.DataSaidaLojaPrevista))
                    {
                        if (DateTime.TryParseExact((string)registro.DataSaidaLojaPrevista, patternConversaoData, null, System.Globalization.DateTimeStyles.None, out DateTime dataSaidaDestinatarioAux))
                            dataSaidaDestinatario = dataSaidaDestinatarioAux;
                        else
                        {
                            _unitOfWork.Rollback();
                            return "A saída do destinatário deve estar no padrão " + patternConversaoData;
                        }
                    }

                    DateTime? dataFimViagem = null;
                    if (!string.IsNullOrWhiteSpace((string)registro.DataPrevisaoFimViagem))
                    {
                        if (DateTime.TryParseExact((string)registro.DataPrevisaoFimViagem, patternConversaoData, null, System.Globalization.DateTimeStyles.None, out DateTime dataFimViagemAux))
                            dataFimViagem = dataFimViagemAux;
                        else
                        {
                            _unitOfWork.Rollback();
                            return "O fim da viagem deve estar no padrão " + patternConversaoData;
                        }
                    }

                    DateTime? previsaoChegadaDoca = null;
                    if (!string.IsNullOrWhiteSpace((string)registro.PrevisaoChegadaDoca))
                    {
                        if (DateTime.TryParseExact((string)registro.PrevisaoChegadaDoca, patternConversaoData, null, System.Globalization.DateTimeStyles.None, out DateTime previsaoChegadaDocaAux))
                            previsaoChegadaDoca = previsaoChegadaDocaAux;
                    }

                    if (previsaoChegadaDoca > dataInicioViagem)
                    {
                        _unitOfWork.Rollback();
                        return "A data da doca não pode ser maior que a data do ínicio da viagem.";
                    }

                    string numeroPedido = (string)registro.NumeroPedido;

                    decimal pesoPedido = 0;
                    if (!string.IsNullOrWhiteSpace((string)registro.PesoPedido))
                        pesoPedido = ((string)registro.PesoPedido).ToDecimal();

                    Dominio.Entidades.Embarcador.Cargas.TipoDeCarga tipoCarga = null;
                    if (!string.IsNullOrWhiteSpace((string)registro.TipoCarga))
                        tipoCarga = repTipoDeCarga.BuscarPorCodigo(((string)registro.TipoCarga).ToInt());

                    Dominio.Entidades.Embarcador.Cargas.ModeloVeicularCarga modeloVeicularCarga = null;
                    if (!string.IsNullOrWhiteSpace((string)registro.ModeloVeicularCarga))
                        modeloVeicularCarga = repModeloVeicularCarga.BuscarPorCodigo(((string)registro.ModeloVeicularCarga).ToInt());

                    Dominio.Entidades.Embarcador.Pedidos.TipoOperacao tipoOperacao = null;
                    if (!string.IsNullOrWhiteSpace((string)registro.TipoOperacao))
                        tipoOperacao = repTipoOperacao.BuscarPorCodigo(((string)registro.TipoOperacao).ToInt());

                    Dominio.Entidades.Cliente destinatario = null;
                    if (!string.IsNullOrWhiteSpace((string)registro.Destinatario))
                        destinatario = repCliente.BuscarPorCPFCNPJ(((string)registro.Destinatario).ToLong());

                    Dominio.Entidades.Cliente remetente = null;
                    if (!string.IsNullOrWhiteSpace((string)registro.Remetente))
                        remetente = repCliente.BuscarPorCPFCNPJ(((string)registro.Remetente).ToLong());

                    Dominio.Entidades.Embarcador.Filiais.Filial filial = null;
                    if (!string.IsNullOrWhiteSpace((string)registro.Filial))
                        filial = repFilial.BuscarPorCodigo(((string)registro.Filial).ToInt());

                    Dominio.Entidades.Empresa empresa = null;
                    if (!string.IsNullOrWhiteSpace((string)registro.Empresa))
                        empresa = repEmpresa.BuscarPorCodigo(((string)registro.Empresa).ToInt());
                    #endregion

                    Dominio.ObjetosDeValor.Embarcador.GestaoPatio.PreSetTempoEtapa preSetTempoEtapa = new Dominio.ObjetosDeValor.Embarcador.GestaoPatio.PreSetTempoEtapa()
                    {
                        DataInicioViagemPrevista = dataInicioViagem,
                        DataChegadaLojaPrevista = dataChegadaDestinatario,
                        DataSaidaLojaPrevista = dataSaidaDestinatario,
                        DataFimViagemPrevista = dataFimViagem
                    };

                    if (fluxoGestaoPatio != null)
                        servicoFluxoGestaoPatio.AtualizarDataPrevistaEtapas(fluxoGestaoPatio, preSetTempoEtapa);

                    preCarga.DataPrevisaoInicioViagem = dataInicioViagem;
                    preCarga.DataPrevisaoFimViagem = dataFimViagem;
                    preCarga.PrevisaoChegadaDestinatario = dataChegadaDestinatario;
                    preCarga.PrevisaoSaidaDestinatario = dataSaidaDestinatario;
                    preCarga.PrevisaoChegadaDoca = previsaoChegadaDoca;
                    preCarga.DocaCarregamento = (string)registro.DocaCarregamento;
                    preCarga.CargaRetorno = (string)registro.CargaRetorno;

                    if (preCarga.IsInitialized())
                    {
                        preCarga.DataAtualizacaoImportacao = DateTime.Now;
                        var auditoria = preCarga.GetChanges();
                        if (auditoria.Count > 0)
                            Servicos.Auditoria.Auditoria.Auditar(auditado, preCarga, auditoria, "Atualizou a pré carga via importação da planilha.", _unitOfWork);
                    }
                    repPreCarga.Atualizar(preCarga);

                    if (numeroPedido.Length > 0)
                    {
                        Dominio.Entidades.Embarcador.Pedidos.Pedido pedido = null;
                        Dominio.Entidades.Embarcador.Cargas.Carga cargaDoPedidoImportado = repPedido.BuscarCargaPorPedido(numeroPedido, filial?.CodigoFilialEmbarcador ?? "");

                        if (cargaDoPedidoImportado != null && cargaDoPedidoImportado.Codigo != (preCarga?.Carga?.Codigo ?? 0))
                        {
                            retorno += $"O pedido {numeroPedido} já está vinculado à carga {cargaDoPedidoImportado.CodigoCargaEmbarcador}";
                        }
                        else if (!AdicionarPedidoImportacao(
                            configuracaoTMS,
                            empresa,
                            preCarga,
                            numeroPedido,
                            pesoPedido,
                            dataChegadaDestinatario,
                            tipoCarga,
                            modeloVeicularCarga,
                            tipoOperacao,
                            destinatario,
                            remetente,
                            filial,
                            usuario,
                            auditado,
                            out string erroPedido,
                            out pedido))
                        {
                            retorno += " " + erroPedido;
                            _unitOfWork.Rollback();
                        }


                        if (preCarga != null && pedido != null)
                        {
                            Dominio.ObjetosDeValor.Embarcador.PreCarga.ImportacaoPreCarga importacao = (from o in importacaoPreCarga where o.PreCarga.Codigo == preCarga.Codigo select o).FirstOrDefault();

                            if (importacao == null)
                            {
                                importacao = new Dominio.ObjetosDeValor.Embarcador.PreCarga.ImportacaoPreCarga();
                                importacaoPreCarga.Add(importacao);
                            }

                            importacao.PreCarga = preCarga;
                            importacao.Pedidos.Add(pedido);
                        }
                    }

                    _unitOfWork.CommitChanges();
                }
            }

            /* Remove pedidos que não estavam contidos na importação
            * #2137
            * Se existe um pedido para pre carga, significa 
            * que precisa ser removido os que não esta nessa importação
            */
            _unitOfWork.Start();
            string numeroPreCargarManipulacao = "";
            try
            {
                foreach (Dominio.ObjetosDeValor.Embarcador.PreCarga.ImportacaoPreCarga importacao in importacaoPreCarga)
                {
                    numeroPreCargarManipulacao = importacao.PreCarga.NumeroPreCarga;
                    if (importacao.Pedidos.Count > 0)
                    {
                        List<int> codigosPedidosImportados = (from o in importacao.Pedidos select o.Codigo).ToList();
                        List<Dominio.Entidades.Embarcador.Pedidos.Pedido> pedidosParaDesvincular = repPedido.BuscarPorPreCargaECodigosDivergentes(importacao.PreCarga.Codigo, codigosPedidosImportados);

                        foreach (Dominio.Entidades.Embarcador.Pedidos.Pedido pedido in pedidosParaDesvincular)
                        {
                            pedido.PreCarga = null;
                            pedido.SituacaoPedido = SituacaoPedido.Cancelado;

                            repPedido.Atualizar(pedido);
                        }
                    }
                }
                _unitOfWork.CommitChanges();
            }
            catch (Exception e)
            {
                retorno += "Ocorreu uma falha ao atualizar a Pré Carga " + numeroPreCargarManipulacao + ". Pré Carga atualizada parcialmente.";
                Servicos.Log.TratarErro(e);
                _unitOfWork.Rollback();
            }

            return retorno;

        }

        public Dominio.ObjetosDeValor.Embarcador.Importacao.RetornoImportacao ImportarPedido(string dados, Dominio.Entidades.Usuario usuario, AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware tipoServicoMultisoftware, AdminMultisoftware.Dominio.Entidades.Pessoas.Cliente clienteMultisoftware, Dominio.ObjetosDeValor.Embarcador.Auditoria.Auditado auditado)
        {
            bool usuarioMulti = usuario.Codigo == 16720;
            Repositorio.Embarcador.Configuracao.ConfiguracaoTMS repConfiguracaoTMS = new Repositorio.Embarcador.Configuracao.ConfiguracaoTMS(_unitOfWork);

            Dominio.Entidades.Embarcador.Configuracao.ConfiguracaoTMS configuracaoTMS = repConfiguracaoTMS.BuscarConfiguracaoPadrao();

            Repositorio.Embarcador.Cargas.TipoDeCarga repTipoDeCarga = new Repositorio.Embarcador.Cargas.TipoDeCarga(_unitOfWork);
            Repositorio.Embarcador.Cargas.ModeloVeicularCarga repModeloVeicularCarga = new Repositorio.Embarcador.Cargas.ModeloVeicularCarga(_unitOfWork);
            Repositorio.Embarcador.Pedidos.Pedido repPedido = new Repositorio.Embarcador.Pedidos.Pedido(_unitOfWork);
            Repositorio.Embarcador.Cargas.CargaPedido repCargaPedido = new Repositorio.Embarcador.Cargas.CargaPedido(_unitOfWork);
            Repositorio.Embarcador.Pedidos.TipoOperacao repTipoOperacao = new Repositorio.Embarcador.Pedidos.TipoOperacao(_unitOfWork);
            Repositorio.Embarcador.PreCargas.PreCarga repPreCarga = new Repositorio.Embarcador.PreCargas.PreCarga(_unitOfWork);
            Repositorio.Embarcador.Cargas.Carga repCarga = new Repositorio.Embarcador.Cargas.Carga(_unitOfWork);
            Repositorio.Embarcador.Filiais.Filial repFilial = new Repositorio.Embarcador.Filiais.Filial(_unitOfWork);
            Repositorio.Cliente repCliente = new Repositorio.Cliente(_unitOfWork);
            Repositorio.Empresa repEmpresa = new Repositorio.Empresa(_unitOfWork);
            Repositorio.Embarcador.Cargas.FaixaTemperatura repFaixaTemperatura = new Repositorio.Embarcador.Cargas.FaixaTemperatura(_unitOfWork);
            Logistica.FilaCarregamentoVeiculo servicoFilaCarregamentoVeiculo = new Logistica.FilaCarregamentoVeiculo(_unitOfWork, Logistica.FilaCarregamentoVeiculo.ObterOrigemAlteracaoFilaCarregamento(tipoServicoMultisoftware));
            Carga.CargaDadosSumarizados servicoCargaDadosSumarizados = new Carga.CargaDadosSumarizados(_unitOfWork);
            GestaoPatio.FluxoGestaoPatio servicoFluxoGestaoPatio = new GestaoPatio.FluxoGestaoPatio(_unitOfWork);

            Servicos.WebService.Empresa.Motorista serMotorista = new WebService.Empresa.Motorista(_unitOfWork);
            Dominio.ObjetosDeValor.Embarcador.Importacao.RetornoImportacao retornoImportacao = new Dominio.ObjetosDeValor.Embarcador.Importacao.RetornoImportacao
            {
                Retornolinhas = new List<Dominio.ObjetosDeValor.Embarcador.Importacao.RetonoLinha>()
            };
            List<Dominio.ObjetosDeValor.Embarcador.PreCarga.ImportacaoPreCarga> importacaoPreCarga = new List<Dominio.ObjetosDeValor.Embarcador.PreCarga.ImportacaoPreCarga>();
            List<Dominio.ObjetosDeValor.Embarcador.PreCarga.PreCargaAlteracaoImportacao> preCargaAlteracaoImportacaos = new List<Dominio.ObjetosDeValor.Embarcador.PreCarga.PreCargaAlteracaoImportacao>();
            List<Dominio.ObjetosDeValor.Embarcador.PreCarga.PreCargaDivergenciaImportacao> codigosPreCargasGeradasNessaPlanilha = new List<Dominio.ObjetosDeValor.Embarcador.PreCarga.PreCargaDivergenciaImportacao>();
            List<string> numerosPreCargasDivergentesNessaPlanilha = new List<string>();

            string erro = string.Empty;
            int contador = 0;
            List<Dominio.ObjetosDeValor.Embarcador.Importacao.DadosLinha> linhas = JsonConvert.DeserializeObject<List<Dominio.ObjetosDeValor.Embarcador.Importacao.DadosLinha>>(dados);

            string prefixoPreCarga = Guid.NewGuid().ToString().Replace("-", "").Substring(5, 10);//cria o prefixo randomico para não correr o risco de inserir um pedido em uma pre carga já existente
            string patternConversaoData = "dd/MM/yyyy HH:mm:ss";

            for (int i = 0, countLinhas = linhas.Count; i < countLinhas; i++)
            {
                try
                {
                    _unitOfWork.FlushAndClear();
                    _unitOfWork.Start();
                    Dominio.ObjetosDeValor.Embarcador.Importacao.DadosLinha linha = linhas[i];

                    #region Converte Dados

                    Dominio.ObjetosDeValor.Embarcador.Importacao.DadosColuna coltransportador = (from obj in linha.Colunas where obj.NomeCampo == "CNPJTransportadora" select obj).FirstOrDefault();
                    Dominio.Entidades.Empresa empresa = null;
                    if (coltransportador != null)
                    {
                        long.TryParse(coltransportador.Valor, out long longcnpjTransportador);
                        string cnpjTransportador = longcnpjTransportador.ToString("d14");
                        empresa = repEmpresa.BuscarPorCNPJ(cnpjTransportador);
                        if (longcnpjTransportador > 0 && empresa == null)
                        {
                            retornoImportacao.Retornolinhas.Add(RetornarFalhaLinha("A empresa informada não existe na base multisoftware", i));
                            _unitOfWork.Rollback();
                            continue;
                        }
                    }

                    Dominio.ObjetosDeValor.Embarcador.Importacao.DadosColuna colNumeroPreCarga = (from obj in linha.Colunas where obj.NomeCampo == "NumeroPreCarga" select obj).FirstOrDefault();
                    string numeroPreCarga = "";
                    if (colNumeroPreCarga != null)
                    {
                        numeroPreCarga = colNumeroPreCarga.Valor;
                        if (!configuracaoTMS.UsarMesmoNumeroPreCargaGerarCargaViaImportacao && !string.IsNullOrWhiteSpace(numeroPreCarga))
                            numeroPreCarga = prefixoPreCarga + "_" + numeroPreCarga;
                    }
                    if (numerosPreCargasDivergentesNessaPlanilha.Contains(numeroPreCarga))
                    {
                        retornoImportacao.Retornolinhas.Add(RetornarFalhaLinha("Dados divergentes da pré carga " + numeroPreCarga + ".", i));
                        _unitOfWork.Rollback();
                        continue;
                    }


                    Dominio.ObjetosDeValor.Embarcador.Importacao.DadosColuna colCarga = (from obj in linha.Colunas where obj.NomeCampo == "Carga" select obj).FirstOrDefault();
                    string numeroCarga = colCarga?.Valor ?? string.Empty;


                    Dominio.ObjetosDeValor.Embarcador.Importacao.DadosColuna colNumeroPedido = (from obj in linha.Colunas where obj.NomeCampo == "NumeroPedido" select obj).FirstOrDefault();
                    string numeroPedido = colNumeroPedido?.Valor ?? string.Empty;


                    Dominio.ObjetosDeValor.Embarcador.Importacao.DadosColuna colPesoPedido = (from obj in linha.Colunas where obj.NomeCampo == "PesoPedido" select obj).FirstOrDefault();
                    decimal pesoPedido = 0;
                    if (colPesoPedido != null)
                    {
                        decimal.TryParse((string)colPesoPedido.Valor, out pesoPedido);
                    }

                    Dominio.ObjetosDeValor.Embarcador.Importacao.DadosColuna colTipoCarga = (from obj in linha.Colunas where obj.NomeCampo == "TipoCarga" select obj).FirstOrDefault();
                    Dominio.Entidades.Embarcador.Cargas.TipoDeCarga tipoCarga = null;
                    if (colTipoCarga != null)
                    {
                        tipoCarga = repTipoDeCarga.BuscarPorCodigoEmbarcador((string)colTipoCarga.Valor);
                        if (tipoCarga == null)
                        {
                            retornoImportacao.Retornolinhas.Add(RetornarFalhaLinha("O tipo de Carga informado não existe na base multisoftware", i));
                            _unitOfWork.Rollback();
                            continue;
                        }
                    }

                    Dominio.ObjetosDeValor.Embarcador.Importacao.DadosColuna colModeloVeicularCarga = (from obj in linha.Colunas where obj.NomeCampo == "ModeloVeicularCarga" select obj).FirstOrDefault();
                    Dominio.Entidades.Embarcador.Cargas.ModeloVeicularCarga modeloVeicularCarga = null;
                    if (colModeloVeicularCarga != null)
                    {
                        modeloVeicularCarga = repModeloVeicularCarga.buscarPorCodigoIntegracao((string)colModeloVeicularCarga.Valor);
                        if (modeloVeicularCarga == null)
                        {
                            retornoImportacao.Retornolinhas.Add(RetornarFalhaLinha("O modelo veicular informado não existe na base multisoftware", i));
                            _unitOfWork.Rollback();
                            continue;
                        }
                    }

                    if (modeloVeicularCarga == null)
                        modeloVeicularCarga = configuracaoTMS.ModeloVeicularCargaPadraoImportacaoPedido;


                    Dominio.ObjetosDeValor.Embarcador.Importacao.DadosColuna colTipoOperacao = (from obj in linha.Colunas where obj.NomeCampo == "TipoOperacao" select obj).FirstOrDefault();
                    Dominio.Entidades.Embarcador.Pedidos.TipoOperacao tipoOperacao = null;
                    if (colTipoOperacao != null)
                    {
                        tipoOperacao = repTipoOperacao.BuscarPorCodigoIntegracao((string)colTipoOperacao.Valor);
                        if (tipoOperacao == null)
                        {
                            retornoImportacao.Retornolinhas.Add(RetornarFalhaLinha("O tipo de operação informado não existe na base multisoftware", i));
                            _unitOfWork.Rollback();
                            continue;
                        }
                    }
                    else
                    {
                        tipoOperacao = repTipoOperacao.BuscarTipoOperacaoPadraoQuandoNaoInformadaNaIntegracao();
                        if (tipoOperacao != null && tipoCarga == null)
                            tipoCarga = tipoOperacao.TipoDeCargaPadraoOperacao;
                    }


                    Dominio.Entidades.Cliente destinatario = null;
                    Dominio.ObjetosDeValor.Embarcador.Importacao.DadosColuna colCodigoDestinatario = (from obj in linha.Colunas where obj.NomeCampo == "CodigoDestinatario" select obj).FirstOrDefault();
                    if (destinatario == null && colCodigoDestinatario != null)
                    {
                        destinatario = repCliente.BuscarPorCodigoIntegracao((string)colCodigoDestinatario.Valor);
                        if (destinatario == null)
                        {
                            retornoImportacao.Retornolinhas.Add(RetornarFalhaLinha("O destinatário informado não está cadastrado na base Multisoftware", i));
                            _unitOfWork.Rollback();
                            continue;
                        }
                    }


                    Dominio.Entidades.Cliente remetente = null;
                    Dominio.ObjetosDeValor.Embarcador.Importacao.DadosColuna colCodigoRemetente = (from obj in linha.Colunas where obj.NomeCampo == "CodigoRemetente" select obj).FirstOrDefault();
                    if (remetente == null && colCodigoRemetente != null)
                    {
                        remetente = repCliente.BuscarPorCodigoIntegracao(colCodigoRemetente.Valor);
                        if (remetente == null)
                        {
                            string retornoRemetente = CriarFornecedor(ref remetente, (string)colCodigoRemetente.Valor);
                            if (remetente == null)
                            {
                                retornoImportacao.Retornolinhas.Add(RetornarFalhaLinha(!string.IsNullOrWhiteSpace(retornoRemetente) ? retornoRemetente : "O remetente informado não está cadastrado na base Multisoftware", i));
                                _unitOfWork.Rollback();
                                continue;
                            }
                        }
                    }

                    Dominio.Entidades.Embarcador.Filiais.Filial filial = null;
                    if (remetente != null)
                    {
                        filial = repFilial.buscarPorCodigoEmbarcador(remetente.CPF_CNPJ_SemFormato);
                        if (filial == null)
                        {
                            retornoImportacao.Retornolinhas.Add(RetornarFalhaLinha("É obrigatório informar a filial para a importação do pedido.", i));
                            _unitOfWork.Rollback();
                            continue;
                        }
                    }

                    Dominio.ObjetosDeValor.Embarcador.Importacao.DadosColuna colFaixaTemperatura = (from obj in linha.Colunas where obj.NomeCampo == "FaixaTemperatura" select obj).FirstOrDefault();
                    Dominio.Entidades.Embarcador.Cargas.FaixaTemperatura faixaTemperatura = null;
                    if (colFaixaTemperatura != null)
                    {
                        faixaTemperatura = repFaixaTemperatura.BuscarPorDescricao((string)colFaixaTemperatura.Valor, filial?.Codigo ?? 0, tipoOperacao?.Codigo ?? 0);
                        if (faixaTemperatura == null)
                        {
                            retornoImportacao.Retornolinhas.Add(RetornarFalhaLinha("A faixa de temperatura informada não existe na base multisoftware", i));
                            _unitOfWork.Rollback();
                            continue;
                        }
                    }

                    Dominio.ObjetosDeValor.Embarcador.Importacao.DadosColuna colDocaCarregamento = (from obj in linha.Colunas where obj.NomeCampo == "DocaCarregamento" select obj).FirstOrDefault();
                    string docaCarregamento = colDocaCarregamento?.Valor ?? string.Empty;

                    Dominio.ObjetosDeValor.Embarcador.Importacao.DadosColuna colCargaRetorno = (from obj in linha.Colunas where obj.NomeCampo == "CargaRetorno" select obj).FirstOrDefault();
                    string cargaRetorno = "";
                    if (colCargaRetorno != null)
                        cargaRetorno = ((string)colCargaRetorno.Valor).ToUpper() == "S" ? "SIM" : "NÃO";


                    DateTime? previsaoChegadaDoca = null;
                    Dominio.ObjetosDeValor.Embarcador.Importacao.DadosColuna colPrevisaoChegadaDoca = (from obj in linha.Colunas where obj.NomeCampo == "PrevisaoChegadaDoca" select obj).FirstOrDefault();
                    if (colPrevisaoChegadaDoca != null && !string.IsNullOrWhiteSpace(colPrevisaoChegadaDoca.Valor))
                    {
                        if (DateTime.TryParseExact(colPrevisaoChegadaDoca.Valor, patternConversaoData, null, System.Globalization.DateTimeStyles.None, out DateTime previsaoChegadaDocaAux))
                        {
                            previsaoChegadaDoca = previsaoChegadaDocaAux;
                            int tempoTolerancia = configuracaoTMS.MinutosToleranciaPrevisaoChegadaDocaCarregamento;
                            if (previsaoChegadaDocaAux < DateTime.Now.AddMinutes(-tempoTolerancia) && !usuarioMulti)
                            {
                                retornoImportacao.Retornolinhas.Add(RetornarFalhaLinha("Não é possível importar com previsão de chegada na doca retroativa.", i));
                                _unitOfWork.Rollback();
                                continue;
                            }
                        }
                    }

                    DateTime? dataInicioViagem = null;
                    Dominio.ObjetosDeValor.Embarcador.Importacao.DadosColuna colInicioViagem = (from obj in linha.Colunas where obj.NomeCampo == "InicioViagem" select obj).FirstOrDefault();
                    if (colInicioViagem != null && !string.IsNullOrWhiteSpace(colInicioViagem.Valor))
                    {
                        if (DateTime.TryParseExact(colInicioViagem.Valor, patternConversaoData, null, System.Globalization.DateTimeStyles.None, out DateTime dataInicioViagemAux))
                        {
                            dataInicioViagem = dataInicioViagemAux;

                            if (dataInicioViagemAux < DateTime.Now && !usuarioMulti)
                            {
                                retornoImportacao.Retornolinhas.Add(RetornarFalhaLinha("Não é possível importar com início viagem retroativa.", i));
                                _unitOfWork.Rollback();
                                continue;
                            }
                        }
                        else
                        {
                            retornoImportacao.Retornolinhas.Add(RetornarFalhaLinha("O início da viagem deve estar no padrão " + patternConversaoData, i));
                            _unitOfWork.Rollback();
                            continue;
                        }
                    }

                    DateTime? dataChegadaDestinatario = null;
                    Dominio.ObjetosDeValor.Embarcador.Importacao.DadosColuna colChegadaDestinatario = (from obj in linha.Colunas where obj.NomeCampo == "ChegadaDestinatario" select obj).FirstOrDefault();
                    if (colChegadaDestinatario != null && !string.IsNullOrWhiteSpace(colChegadaDestinatario.Valor))
                    {
                        if (DateTime.TryParseExact(colChegadaDestinatario.Valor, patternConversaoData, null, System.Globalization.DateTimeStyles.None, out DateTime dataChegadaDestinatarioAux))
                            dataChegadaDestinatario = dataChegadaDestinatarioAux;
                        else
                        {
                            retornoImportacao.Retornolinhas.Add(RetornarFalhaLinha("A chegada do destinatário deve estar no padrão " + patternConversaoData, i));
                            _unitOfWork.Rollback();
                            continue;
                        }
                    }

                    DateTime? dataSaidaDestinatario = null;
                    Dominio.ObjetosDeValor.Embarcador.Importacao.DadosColuna colSaidaDestinatario = (from obj in linha.Colunas where obj.NomeCampo == "SaidaDestinatario" select obj).FirstOrDefault();
                    if (colSaidaDestinatario != null && !string.IsNullOrWhiteSpace(colSaidaDestinatario.Valor))
                    {
                        if (DateTime.TryParseExact(colSaidaDestinatario.Valor, patternConversaoData, null, System.Globalization.DateTimeStyles.None, out DateTime dataSaidaDestinatarioAux))
                            dataSaidaDestinatario = dataSaidaDestinatarioAux;
                        else
                        {
                            retornoImportacao.Retornolinhas.Add(RetornarFalhaLinha("A saída do destinatário deve estar no padrão " + patternConversaoData, i));
                            _unitOfWork.Rollback();
                            continue;
                        }
                    }

                    DateTime? dataFimViagem = null;
                    Dominio.ObjetosDeValor.Embarcador.Importacao.DadosColuna colFimViagem = (from obj in linha.Colunas where obj.NomeCampo == "FimViagem" select obj).FirstOrDefault();
                    if (colFimViagem != null && !string.IsNullOrWhiteSpace(colFimViagem.Valor))
                    {
                        if (DateTime.TryParseExact(colFimViagem.Valor, patternConversaoData, null, System.Globalization.DateTimeStyles.None, out DateTime dataFimViagemAux))
                            dataFimViagem = dataFimViagemAux;
                        else
                        {
                            retornoImportacao.Retornolinhas.Add(RetornarFalhaLinha("O fim da viagem deve estar no padrão " + patternConversaoData, i));
                            _unitOfWork.Rollback();
                            continue;
                        }
                    }

                    Dominio.ObjetosDeValor.Embarcador.GestaoPatio.PreSetTempoEtapa preSetTempoEtapa = new Dominio.ObjetosDeValor.Embarcador.GestaoPatio.PreSetTempoEtapa()
                    {
                        DataInicioViagemPrevista = dataInicioViagem,
                        DataChegadaLojaPrevista = dataChegadaDestinatario,
                        DataSaidaLojaPrevista = dataSaidaDestinatario,
                        DataFimViagemPrevista = dataFimViagem
                    };

                    if (previsaoChegadaDoca > dataInicioViagem)
                    {
                        retornoImportacao.Retornolinhas.Add(RetornarFalhaLinha("A data da doca não pode ser maior que a data do ínicio da viagem.", i));
                        _unitOfWork.Rollback();
                        continue;
                    }

                    Dominio.ObjetosDeValor.Embarcador.Importacao.DadosColuna colPesoPlanejamento = (from obj in linha.Colunas where obj.NomeCampo == "PesoPlanejamento" select obj).FirstOrDefault();
                    decimal pesoPlanejamento = 0;
                    if (colPesoPlanejamento != null)
                        decimal.TryParse((string)colPesoPlanejamento.Valor, out pesoPlanejamento);

                    Dominio.ObjetosDeValor.Embarcador.Importacao.DadosColuna colQuantidadePallets = (from obj in linha.Colunas where obj.NomeCampo == "QuantidadePallets" select obj).FirstOrDefault();
                    decimal quantidadePallets = 0;
                    if (colQuantidadePallets != null)
                        decimal.TryParse((string)colQuantidadePallets.Valor, out quantidadePallets);

                    Dominio.ObjetosDeValor.Embarcador.Importacao.DadosColuna colObservacao = (from obj in linha.Colunas where obj.NomeCampo == "Observacao" select obj).FirstOrDefault();
                    string observacao = colObservacao?.Valor ?? string.Empty;

                    #endregion

                    Dominio.Entidades.Embarcador.PreCargas.PreCarga preCarga = repPreCarga.BuscarPorNumeroPreCargaNaoCancelada(numeroPreCarga);

                    bool preCargaJaExiste = preCarga != null;
                    Dominio.Entidades.Embarcador.Cargas.CargaJanelaCarregamento janelaCarregamento = null;

                    if (preCarga == null)
                    {
                        preCarga = CriarPreCargaImportada(numeroPreCarga, empresa, tipoCarga, modeloVeicularCarga, tipoOperacao, usuario, filial, faixaTemperatura);
                        Servicos.Auditoria.Auditoria.Auditar(auditado, preCarga, null, "Gerou a pré carga via importação da planilha.", _unitOfWork);
                        codigosPreCargasGeradasNessaPlanilha.Add(new Dominio.ObjetosDeValor.Embarcador.PreCarga.PreCargaDivergenciaImportacao()
                        {
                            Codigo = preCarga.Codigo,
                            Index = i
                        });

                        janelaCarregamento = CriarCargaJanelaCarregamento(preCarga);

                        servicoFluxoGestaoPatio.Adicionar(preCarga, tipoServicoMultisoftware, janelaCarregamento, preSetTempoEtapa);
                    }
                    else if (preCarga.Carga != null)
                    {
                        // Quando sera atualizado os dados mas já possui carga vinculada, não atualizar nada
                        retornoImportacao.Retornolinhas.Add(RetornarFalhaLinha("Essa pré carga já possui uma carga, não é possível atualizar os dados da mesma.", i));
                        _unitOfWork.Rollback();
                        continue;
                    }

                    Dominio.ObjetosDeValor.Embarcador.PreCarga.PreCargaDivergenciaImportacao registroCodigoPreCargaGeradaNessaPlanilha = (from o in codigosPreCargasGeradasNessaPlanilha where o.Codigo == preCarga.Codigo select o).FirstOrDefault();
                    List<Dominio.Entidades.Embarcador.Logistica.FilaCarregamentoVeiculo> filasCarregamentoAtualizadas = new List<Dominio.Entidades.Embarcador.Logistica.FilaCarregamentoVeiculo>();

                    #region Divergencia de dados
                    if (preCargaJaExiste)
                    {
                        bool tipoOperacaoDiferente = ((preCarga.TipoOperacao?.Codigo ?? 0) != (tipoOperacao?.Codigo ?? 0));
                        bool tipoCargaDiferente = ((preCarga.TipoDeCarga?.Codigo ?? 0) != (tipoCarga?.Codigo ?? 0));
                        bool modeloVeicularCargaDiferente = ((preCarga.ModeloVeicularCarga?.Codigo ?? 0) != (modeloVeicularCarga?.Codigo ?? 0));
                        //bool dataChegadaLojaDiferente = false;
                        //bool dataSaidaLojaDiferente = false;
                        bool dataInicioViagemDiferente = preCarga.DataPrevisaoInicioViagem != dataInicioViagem;
                        bool dataFimViagemDiferente = preCarga.DataPrevisaoFimViagem != dataFimViagem;
                        //if (!string.IsNullOrWhiteSpace(numeroPedido))
                        //{
                        //    Dominio.Entidades.Embarcador.Pedidos.Pedido pedido = repPedido.buscarPorNumeroEmbarcador(numeroPedido, filial?.CodigoFilialEmbarcador ?? "", "");
                        //    if (pedido != null)
                        //    {
                        //        dataChegadaLojaDiferente = pedido.DataPrevisaoChegadaDestinatario != dataChegadaDestinatario;
                        //        dataSaidaLojaDiferente = pedido.DataPrevisaoSaidaDestinatario != dataSaidaDestinatario;
                        //    }
                        //}

                        if (tipoOperacaoDiferente || tipoCargaDiferente || modeloVeicularCargaDiferente || dataInicioViagemDiferente || dataFimViagemDiferente)
                        {
                            string erroAgrupamento = "";

                            if (tipoOperacaoDiferente) erroAgrupamento += "tipo de operação, ";
                            if (tipoCargaDiferente) erroAgrupamento += "tipo de carga, ";
                            if (modeloVeicularCargaDiferente) erroAgrupamento += "modelo veicular, ";
                            //if (dataChegadaLojaDiferente) erroAgrupamento += "data chegada destinatário, ";
                            //if (dataSaidaLojaDiferente) erroAgrupamento += "data saída destinatário, ";
                            if (dataInicioViagemDiferente) erroAgrupamento += "data início viagem, ";
                            if (dataFimViagemDiferente) erroAgrupamento += "data fim da viagem, ";

                            erroAgrupamento = erroAgrupamento.Trim();
                            erroAgrupamento = erroAgrupamento.Substring(0, erroAgrupamento.Length - 1);

                            // Quando a importação é tudo na mesma planilha, eu preciso cancelar a importação de todas pre cargas desse numero
                            // Isso implica em remover a primeira desse número
                            if (registroCodigoPreCargaGeradaNessaPlanilha != null)
                            {
                                if (!numerosPreCargasDivergentesNessaPlanilha.Contains(preCarga.NumeroPreCarga))
                                    numerosPreCargasDivergentesNessaPlanilha.Add(preCarga.NumeroPreCarga);

                                DeletarPreCarga(preCarga);

                                Dominio.Entidades.Embarcador.Logistica.FilaCarregamentoVeiculo filaCarregamentoAtualizada = servicoFilaCarregamentoVeiculo.DisponibilizarPorPreCargaCancelada(preCarga, tipoServicoMultisoftware);
                                var retornoImportacaoAlteracao = (from o in retornoImportacao.Retornolinhas where o.indice == registroCodigoPreCargaGeradaNessaPlanilha.Index select o).FirstOrDefault();
                                retornoImportacaoAlteracao.mensagemFalha = "Dados divergentes: " + erroAgrupamento + ".";
                                retornoImportacaoAlteracao.processou = false;
                                contador--;
                                codigosPreCargasGeradasNessaPlanilha.Remove(registroCodigoPreCargaGeradaNessaPlanilha);

                                retornoImportacao.Retornolinhas.Add(RetornarFalhaLinha(retornoImportacaoAlteracao.mensagemFalha, i));
                                _unitOfWork.CommitChanges();

                                if (filaCarregamentoAtualizada != null)
                                    filasCarregamentoAtualizadas.Add(filaCarregamentoAtualizada);

                                continue;
                            }

                            retornoImportacao.Retornolinhas.Add(RetornarFalhaLinha("Já existe a pré carga informada, mas com " + erroAgrupamento + " diferente.", i));
                            _unitOfWork.Rollback();
                            continue;
                        }
                    }
                    #endregion

                    /**
                     * Só entra no if, quando a pre carga ja existe mas que não seja dessa planilha
                     */
                    if (preCargaJaExiste && registroCodigoPreCargaGeradaNessaPlanilha == null)
                    {
                        //if (registroCodigoPreCargaGeradaNessaPlanilha == null)
                        //{
                        Dominio.ObjetosDeValor.Embarcador.PreCarga.PreCargaAlteracaoImportacao preCargaAlteracaoImportacao = new Dominio.ObjetosDeValor.Embarcador.PreCarga.PreCargaAlteracaoImportacao
                        {
                            CodigoPreCarga = preCarga.Codigo,
                            NumeroPreCarga = preCarga.NumeroPreCarga,

                            DataPrevisaoInicioViagem = dataInicioViagem,
                            DataPrevisaoInicioViagemAnterior = preCarga.DataPrevisaoInicioViagem,

                            DataSaidaLojaPrevista = dataSaidaDestinatario,

                            DataPrevisaoFimViagem = dataFimViagem,
                            DataPrevisaoFimViagemAnterior = preCarga.DataPrevisaoFimViagem,

                            PrevisaoChegadaDestinatario = dataInicioViagem,
                            PrevisaoChegadaDestinatarioAnterior = preCarga.PrevisaoChegadaDestinatario,

                            PrevisaoSaidaDestinatario = preCarga.PrevisaoSaidaDestinatario,

                            PrevisaoChegadaDoca = previsaoChegadaDoca,
                            PrevisaoChegadaDocaAnterior = preCarga.PrevisaoChegadaDoca,

                            DocaCarregamento = docaCarregamento,
                            DocaCarregamentoAnterior = preCarga.DocaCarregamento,

                            CargaRetorno = cargaRetorno,

                            CargaRetornoAnterior = preCarga.CargaRetorno,

                            // Pedido
                            NumeroPedido = numeroPedido,
                            PesoPedido = pesoPedido,
                            TipoCarga = tipoCarga?.Codigo ?? 0,
                            ModeloVeicularCarga = modeloVeicularCarga?.Codigo ?? 0,
                            TipoOperacao = tipoOperacao?.Codigo ?? 0,
                            Destinatario = destinatario?.Codigo ?? 0,
                            Remetente = remetente?.Codigo ?? 0,
                            Filial = filial?.Codigo ?? 0,
                            Empresa = empresa?.Codigo ?? 0,
                            Observacao = observacao,
                            PesoPlanejamento = pesoPlanejamento,
                            QuantidadePallets = quantidadePallets
                        };

                        preCargaAlteracaoImportacaos.Add(preCargaAlteracaoImportacao);
                        //}
                        //else
                        //{

                        //}
                    }
                    else
                    {
                        preCarga.DataPrevisaoInicioViagem = dataInicioViagem;
                        preCarga.DataPrevisaoFimViagem = dataFimViagem;
                        preCarga.PrevisaoChegadaDestinatario = dataChegadaDestinatario;
                        preCarga.PrevisaoSaidaDestinatario = dataSaidaDestinatario;
                        preCarga.PrevisaoChegadaDoca = previsaoChegadaDoca;
                        preCarga.DocaCarregamento = docaCarregamento;
                        preCarga.CargaRetorno = cargaRetorno;
                        preCarga.Peso = pesoPlanejamento;
                        preCarga.Observacao = observacao;
                        preCarga.QuantidadePallet = quantidadePallets;

                        repPreCarga.Atualizar(preCarga);

                        if (numeroPedido.Length > 0)
                        {
                            Dominio.Entidades.Embarcador.Pedidos.Pedido pedido = null;
                            Dominio.Entidades.Embarcador.Cargas.Carga cargaDoPedidoImportado = repPedido.BuscarCargaPorPedido(numeroPedido, filial?.CodigoFilialEmbarcador ?? "");

                            if (cargaDoPedidoImportado != null && cargaDoPedidoImportado.Codigo != (preCarga?.Carga?.Codigo ?? 0))
                            {
                                retornoImportacao.Retornolinhas.Add(RetornarFalhaLinha($"O pedido {numeroPedido} já está vinculado à carga {cargaDoPedidoImportado.CodigoCargaEmbarcador}", i));
                                _unitOfWork.Rollback();
                                continue;
                            }
                            else if (!AdicionarPedidoImportacao(
                                configuracaoTMS,
                                empresa,
                                preCarga,
                                numeroPedido,
                                pesoPedido,
                                dataChegadaDestinatario,
                                tipoCarga,
                                modeloVeicularCarga,
                                tipoOperacao,
                                destinatario,
                                remetente,
                                filial,
                                usuario,
                                auditado,
                                out string retorno,
                                out pedido))
                            {
                                retornoImportacao.Retornolinhas.Add(RetornarFalhaLinha(retorno, i));
                                _unitOfWork.Rollback();
                                continue;
                            }


                            if (preCarga != null && pedido != null)
                            {
                                Dominio.ObjetosDeValor.Embarcador.PreCarga.ImportacaoPreCarga importacao = (from o in importacaoPreCarga where o.PreCarga.Codigo == preCarga.Codigo select o).FirstOrDefault();

                                if (importacao == null)
                                {
                                    importacao = new Dominio.ObjetosDeValor.Embarcador.PreCarga.ImportacaoPreCarga();
                                    importacaoPreCarga.Add(importacao);
                                }

                                importacao.PreCarga = preCarga;
                                importacao.Pedidos.Add(pedido);
                            }
                        }

                        Dominio.Entidades.Embarcador.Cargas.Carga carga = repCarga.BuscarPorCodigoEmbarcador(numeroCarga);
                        if (carga != null && preCarga != null && preCarga.Carga == null)
                        {
                            List<Dominio.Entidades.Embarcador.Cargas.CargaPedido> cargaPedidos = repCargaPedido.BuscarPorCarga(carga.Codigo);
                            if (!VincularPreCargaACarga(preCarga, carga, cargaPedidos, configuracaoTMS, tipoServicoMultisoftware, clienteMultisoftware, out string erroVinculo))
                            {
                                retornoImportacao.Retornolinhas.Add(RetornarFalhaLinha(erroVinculo, i));
                                _unitOfWork.Rollback();
                                continue;
                            }
                            else
                            {
                                Dominio.Entidades.Embarcador.Logistica.FilaCarregamentoVeiculo filaCarregamentoAtualizada = servicoFilaCarregamentoVeiculo.AdicionarCargaPorPreCarga(preCarga, tipoServicoMultisoftware);

                                if (filaCarregamentoAtualizada != null)
                                    filasCarregamentoAtualizadas.Add(filaCarregamentoAtualizada);
                            }
                        }
                    }

                    servicoCargaDadosSumarizados.AlterarDadosSumarizadosPreCarga(preCarga, _unitOfWork, tipoServicoMultisoftware);

                    contador++;
                    Dominio.ObjetosDeValor.Embarcador.Importacao.RetonoLinha retornoLinha = new Dominio.ObjetosDeValor.Embarcador.Importacao.RetonoLinha() { indice = i, processou = true, mensagemFalha = "" };
                    retornoImportacao.Retornolinhas.Add(retornoLinha);
                    _unitOfWork.CommitChanges();

                    servicoFilaCarregamentoVeiculo.NotificarAlteracoes(filasCarregamentoAtualizadas);

                    if (janelaCarregamento != null)
                        new Hubs.JanelaCarregamento().InformarJanelaCarregamentoAtualizada(janelaCarregamento);

                }
                catch (Exception ex2)
                {
                    _unitOfWork.Rollback();
                    Servicos.Log.TratarErro(ex2);
                    retornoImportacao.Retornolinhas.Add(RetornarFalhaLinha("Ocorreu uma falha ao processar a linha.", i));
                    continue;
                }
            }



            /* Remove pedidos que não estavam contidos na importação
             * #2137
             * Se existe um pedido para pre carga, significa 
             * que precisa ser removido os que não esta nessa importação
             */
            string msgErro = "";

            _unitOfWork.Start();
            try
            {
                foreach (Dominio.ObjetosDeValor.Embarcador.PreCarga.ImportacaoPreCarga importacao in importacaoPreCarga)
                {
                    if (importacao.Pedidos.Count > 0)
                    {
                        List<int> codigosPedidosImportados = (from o in importacao.Pedidos select o.Codigo).ToList();
                        List<Dominio.Entidades.Embarcador.Pedidos.Pedido> pedidosParaDesvincular = repPedido.BuscarPorPreCargaECodigosDivergentes(importacao.PreCarga.Codigo, codigosPedidosImportados);

                        foreach (Dominio.Entidades.Embarcador.Pedidos.Pedido pedido in pedidosParaDesvincular)
                        {
                            pedido.PreCarga = null;
                            pedido.SituacaoPedido = SituacaoPedido.Cancelado;

                            repPedido.Atualizar(pedido);
                        }
                    }
                }

                _unitOfWork.CommitChanges();
            }
            catch (Exception e)
            {
                msgErro = "Ocorreu uma falha ao remover pedidos de pré carga.";
                Servicos.Log.TratarErro(e);
                _unitOfWork.Rollback();
            }

            retornoImportacao.RegistrosAlterados = (from obj in preCargaAlteracaoImportacaos
                                                    select new
                                                    {
                                                        obj.CodigoPreCarga,
                                                        obj.NumeroPreCarga,
                                                        DataPrevisaoInicioViagem = obj.DataPrevisaoInicioViagem?.ToString(patternConversaoData) ?? "",
                                                        DataPrevisaoInicioViagemAnterior = obj.DataPrevisaoInicioViagemAnterior?.ToString(patternConversaoData) ?? "",
                                                        PrevisaoChegadaDestinatario = obj.PrevisaoChegadaDestinatario?.ToString(patternConversaoData) ?? "",
                                                        PrevisaoChegadaDestinatarioAnterior = obj.PrevisaoChegadaDestinatarioAnterior?.ToString(patternConversaoData) ?? "",
                                                        PrevisaoChegadaDoca = obj.PrevisaoChegadaDoca?.ToString(patternConversaoData) ?? "",
                                                        DataPrevisaoFimViagem = obj.DataPrevisaoFimViagem?.ToString(patternConversaoData) ?? "",
                                                        DataPrevisaoFimViagemAnterior = obj.DataPrevisaoFimViagemAnterior?.ToString(patternConversaoData) ?? "",
                                                        DataSaidaLojaPrevista = obj.DataSaidaLojaPrevista?.ToString(patternConversaoData) ?? "",
                                                        PrevisaoChegadaDocaAnterior = obj.PrevisaoChegadaDocaAnterior?.ToString(patternConversaoData) ?? "",
                                                        obj.DocaCarregamento,
                                                        obj.DocaCarregamentoAnterior,
                                                        obj.CargaRetorno,
                                                        obj.CargaRetornoAnterior,
                                                        obj.NumeroPedido,
                                                        obj.PesoPedido,
                                                        obj.TipoCarga,
                                                        obj.ModeloVeicularCarga,
                                                        obj.TipoOperacao,
                                                        obj.Destinatario,
                                                        obj.Remetente,
                                                        obj.Filial,
                                                        obj.Empresa,
                                                        obj.Observacao,
                                                        obj.QuantidadePallets,
                                                        obj.PesoPlanejamento,
                                                    });

            retornoImportacao.MensagemAviso = msgErro;
            retornoImportacao.Total = linhas.Count();
            retornoImportacao.Importados = contador;

            return retornoImportacao;

        }

        public Dominio.Entidades.Embarcador.Cargas.CargaJanelaCarregamento CriarCargaJanelaCarregamento(Dominio.Entidades.Embarcador.PreCargas.PreCarga preCarga)
        {
            Repositorio.Embarcador.Cargas.CargaJanelaCarregamento repositorioCargaJanelaCarregamento = new Repositorio.Embarcador.Cargas.CargaJanelaCarregamento(_unitOfWork);
            Dominio.Entidades.Embarcador.Cargas.CargaJanelaCarregamento cargaJanelaCarregamento = repositorioCargaJanelaCarregamento.BuscarPorPreCarga(preCarga.Codigo);

            if (cargaJanelaCarregamento == null)
                cargaJanelaCarregamento = new Logistica.CargaJanelaCarregamento(_unitOfWork).AdicionarPorPreCarga(preCarga.Codigo);

            return cargaJanelaCarregamento;
        }

        public Dominio.ObjetosDeValor.Embarcador.Importacao.RetornoImportacao ImportarPrePlanejamento(List<Dominio.ObjetosDeValor.Embarcador.Importacao.DadosLinha> linhas, Dominio.Entidades.Usuario usuario, Repositorio.UnitOfWork unitOfWork, Dominio.ObjetosDeValor.Embarcador.Auditoria.Auditado auditado, AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware tipoServicoMultisoftware)
        {
            Repositorio.Embarcador.PreCargas.PreCarga repPreCarga = new Repositorio.Embarcador.PreCargas.PreCarga(unitOfWork);
            Repositorio.Embarcador.Filiais.Filial repFilial = new Repositorio.Embarcador.Filiais.Filial(unitOfWork);
            Repositorio.Embarcador.Cargas.ModeloVeicularCarga repModeloVeicularCarga = new Repositorio.Embarcador.Cargas.ModeloVeicularCarga(unitOfWork);
            Repositorio.Embarcador.Cargas.TipoDeCarga repTipoCarga = new Repositorio.Embarcador.Cargas.TipoDeCarga(unitOfWork);
            Repositorio.Embarcador.Pedidos.TipoOperacao repTipoOperacao = new Repositorio.Embarcador.Pedidos.TipoOperacao(unitOfWork);
            Repositorio.Embarcador.PreCargas.PreCarga repositorioPreCarga = new Repositorio.Embarcador.PreCargas.PreCarga(unitOfWork);
            Repositorio.Embarcador.Configuracoes.ConfiguracaoGeralCarga repositorioConfiguracaoGeralCarga = new Repositorio.Embarcador.Configuracoes.ConfiguracaoGeralCarga(unitOfWork);
            Repositorio.Embarcador.Configuracoes.ConfiguracaoPreCarga repositorioConfiguracaoPreCarga = new Repositorio.Embarcador.Configuracoes.ConfiguracaoPreCarga(unitOfWork);
            Repositorio.RotaFrete repRotaFrete = new Repositorio.RotaFrete(unitOfWork);

            Repositorio.Embarcador.PreCargas.PreCargaDestino repositorioPreCargaDestino = new Repositorio.Embarcador.PreCargas.PreCargaDestino(unitOfWork);
            Repositorio.Embarcador.PreCargas.PreCargaEstadoDestino repositorioPreCargaEstadoDestino = new Repositorio.Embarcador.PreCargas.PreCargaEstadoDestino(unitOfWork);
            Repositorio.Localidade repLocalidade = new Repositorio.Localidade(unitOfWork);
            Repositorio.Estado repEstado = new Repositorio.Estado(unitOfWork);

            GestaoPatio.FluxoGestaoPatio servicoFluxoGestaoPatio = new GestaoPatio.FluxoGestaoPatio(unitOfWork);

            Dominio.Entidades.Embarcador.Configuracoes.ConfiguracaoGeralCarga configuracaoGeralCarga = repositorioConfiguracaoGeralCarga.BuscarPrimeiroRegistro();
            Dominio.Entidades.Embarcador.Configuracoes.ConfiguracaoPreCarga configuracaoPreCarga = repositorioConfiguracaoPreCarga.BuscarPrimeiroRegistro();
            Dominio.ObjetosDeValor.Embarcador.Importacao.RetornoImportacao retornoImportacao = new Dominio.ObjetosDeValor.Embarcador.Importacao.RetornoImportacao();

            retornoImportacao.Retornolinhas = new List<Dominio.ObjetosDeValor.Embarcador.Importacao.RetonoLinha>();

            int vinculacaoComSucesso = 0;

            for (int i = 0; i < linhas.Count; i++)
            {
                try
                {
                    unitOfWork.FlushAndClear();
                    unitOfWork.Start();

                    bool destinoAdicionado = false;

                    Dominio.Entidades.Embarcador.PreCargas.PreCarga preCarga = new Dominio.Entidades.Embarcador.PreCargas.PreCarga();

                    Dominio.ObjetosDeValor.Embarcador.Importacao.DadosLinha linha = linhas[i];

                    Dominio.ObjetosDeValor.Embarcador.Importacao.DadosColuna colFilialDescricao = (from obj in linha.Colunas where obj.NomeCampo == "FilialDescricao" select obj).FirstOrDefault();
                    Dominio.ObjetosDeValor.Embarcador.Importacao.DadosColuna colFilialCodigo = (from obj in linha.Colunas where obj.NomeCampo == "FilialCodigo" select obj).FirstOrDefault();
                    Dominio.ObjetosDeValor.Embarcador.Importacao.DadosColuna colTipoCarga = (from obj in linha.Colunas where obj.NomeCampo == "TipoCarga" select obj).FirstOrDefault();
                    Dominio.ObjetosDeValor.Embarcador.Importacao.DadosColuna colTipoOperacao = (from obj in linha.Colunas where obj.NomeCampo == "TipoOperacao" select obj).FirstOrDefault();
                    Dominio.ObjetosDeValor.Embarcador.Importacao.DadosColuna colModeloVeicular = (from obj in linha.Colunas where obj.NomeCampo == "ModeloVeicularCarga" select obj).FirstOrDefault();
                    Dominio.ObjetosDeValor.Embarcador.Importacao.DadosColuna colDataCarregamento = (from obj in linha.Colunas where obj.NomeCampo == "Data" select obj).FirstOrDefault();
                    Dominio.ObjetosDeValor.Embarcador.Importacao.DadosColuna colDataPrevisaoEntrega = (from obj in linha.Colunas where obj.NomeCampo == "DataPrevisaoEntrega" select obj).FirstOrDefault();
                    Dominio.ObjetosDeValor.Embarcador.Importacao.DadosColuna colDestinoCidade = (from obj in linha.Colunas where obj.NomeCampo == "Cidade" select obj).FirstOrDefault();
                    Dominio.ObjetosDeValor.Embarcador.Importacao.DadosColuna colDestinoUF = (from obj in linha.Colunas where obj.NomeCampo == "Estado" select obj).FirstOrDefault();
                    Dominio.ObjetosDeValor.Embarcador.Importacao.DadosColuna colRotaDescricao = (from obj in linha.Colunas where obj.NomeCampo == "RotaDescricao" select obj).FirstOrDefault();
                    Dominio.ObjetosDeValor.Embarcador.Importacao.DadosColuna colRotaCodigo = (from obj in linha.Colunas where obj.NomeCampo == "RotaCodigo" select obj).FirstOrDefault();
                    Dominio.ObjetosDeValor.Embarcador.Importacao.DadosColuna colObservacao = (from obj in linha.Colunas where obj.NomeCampo == "Observacao" select obj).FirstOrDefault();
                    Dominio.ObjetosDeValor.Embarcador.Importacao.DadosColuna colPeso = (from obj in linha.Colunas where obj.NomeCampo == "Peso" select obj).FirstOrDefault();
                    Dominio.ObjetosDeValor.Embarcador.Importacao.DadosColuna colQuantidadePallets = (from obj in linha.Colunas where obj.NomeCampo == "QuantidadePallets" select obj).FirstOrDefault();

                    Dominio.Entidades.Embarcador.Filiais.Filial filial = null;
                    if (colFilialDescricao != null)
                        filial = repFilial.BuscarPorDescricaoParcial((string)colFilialDescricao.Valor);

                    if (colFilialCodigo != null)
                        filial = repFilial.buscarPorCodigoEmbarcador((string)colFilialCodigo.Valor);

                    if (filial == null)
                    {
                        retornoImportacao.Retornolinhas.Add(RetornarFalhaLinha("Filial não encontrada", i));
                        unitOfWork.Rollback();
                        continue;
                    }

                    Dominio.Entidades.Embarcador.Cargas.TipoDeCarga tipoCarga = null;
                    if (colTipoCarga != null)
                    {
                        tipoCarga = repTipoCarga.BuscarPorCodigoEmbarcador((string)colTipoCarga.Valor);
                        if (tipoCarga == null)
                        {
                            retornoImportacao.Retornolinhas.Add(RetornarFalhaLinha("Tipo de Carga não encontrado", i));
                            unitOfWork.Rollback();
                            continue;
                        }
                    }

                    Dominio.Entidades.Embarcador.Pedidos.TipoOperacao tipoOperacao = null;
                    if (colTipoOperacao != null)
                    {
                        tipoOperacao = repTipoOperacao.BuscarPorCodigoIntegracao((string)colTipoOperacao.Valor);
                        if (tipoOperacao == null)
                        {
                            retornoImportacao.Retornolinhas.Add(RetornarFalhaLinha("Tipo de Operação não encontrado", i));
                            unitOfWork.Rollback();
                            continue;
                        }
                    }

                    Dominio.Entidades.Embarcador.Cargas.ModeloVeicularCarga modeloVeicular = null;
                    if (colModeloVeicular != null)
                    {
                        modeloVeicular = repModeloVeicularCarga.buscarPorCodigoIntegracao((string)colModeloVeicular.Valor);
                        if (modeloVeicular == null)
                        {
                            retornoImportacao.Retornolinhas.Add(RetornarFalhaLinha("Modelo Veicular não encontrado", i));
                            unitOfWork.Rollback();
                            continue;
                        }
                    }


                    Dominio.Entidades.RotaFrete rotaFrete = null;
                    if (colRotaCodigo != null)
                    {
                        rotaFrete = repRotaFrete.BuscarPorCodigoIntegracao((string)colRotaCodigo.Valor);
                        if (rotaFrete == null)
                        {
                            retornoImportacao.Retornolinhas.Add(RetornarFalhaLinha("Rota não encontrada", i));
                            unitOfWork.Rollback();
                            continue;
                        }
                    }
                    else if (colRotaDescricao != null)
                    {
                        rotaFrete = repRotaFrete.BuscarPorDescricaoParcial((string)colRotaDescricao.Valor);
                        if (rotaFrete == null)
                        {
                            retornoImportacao.Retornolinhas.Add(RetornarFalhaLinha("Rota não encontrada", i));
                            unitOfWork.Rollback();
                            continue;
                        }
                    }


                    double dataExel = ((string)colDataCarregamento.Valor).ToDouble();
                    DateTime Data = dataExel > 0 ? Utilidades.DateTime.ConverterDataExcelToDateTime(dataExel) : Convert.ToDateTime((string)colDataCarregamento.Valor);
                    if (Data < DateTime.Today)
                    {
                        retornoImportacao.Retornolinhas.Add(RetornarFalhaLinha("A data de pré planejamento deve ser maior ou igual a data atual", i));
                        continue;
                    }

                    DateTime? DataPrevisaoEntrega = null;
                    if (!string.IsNullOrEmpty((string)colDataPrevisaoEntrega?.Valor))
                    {
                        double dataPrevisaoEntregaExcel = ((string)colDataPrevisaoEntrega.Valor).ToDouble();
                        DataPrevisaoEntrega = dataPrevisaoEntregaExcel > 0 ? Utilidades.DateTime.ConverterDataExcelToDateTime(dataPrevisaoEntregaExcel) : Convert.ToDateTime((string)colDataPrevisaoEntrega.Valor);
                        if (DataPrevisaoEntrega < DateTime.Today)
                        {
                            retornoImportacao.Retornolinhas.Add(RetornarFalhaLinha("A data de previsão de entrega deve ser maior ou igual a data atual", i));
                            continue;
                        }
                    }

                    decimal peso = 0;
                    if (colPeso != null)
                        decimal.TryParse((string)colPeso.Valor, out peso);

                    decimal quantidadePallets = 0;
                    if (colQuantidadePallets != null)
                        decimal.TryParse((string)colQuantidadePallets.Valor, out quantidadePallets);

                    string observacao = colObservacao?.Valor ?? string.Empty;


                    preCarga.Filial = filial;
                    preCarga.TipoDeCarga = tipoCarga;
                    preCarga.TipoOperacao = tipoOperacao;
                    preCarga.ModeloVeicularCarga = modeloVeicular;
                    preCarga.DataPrevisaoEntrega = Data;
                    preCarga.DataPrevisaoEntregaManual = DataPrevisaoEntrega;
                    preCarga.ProgramacaoCarga = configuracaoGeralCarga.UtilizarProgramacaoCarga;
                    preCarga.Operador = usuario;
                    preCarga.DataCriacaoPreCarga = DateTime.Now;
                    preCarga.SituacaoPreCarga = configuracaoGeralCarga.UtilizarProgramacaoCarga ? SituacaoPreCarga.Nova : SituacaoPreCarga.AguardandoGeracaoCarga;
                    preCarga.NumeroPreCarga = repositorioPreCarga.ObterProximoCodigo(preCarga.Filial?.Codigo ?? 0).ToString();
                    preCarga.AdicionadaManualmente = true;
                    preCarga.Rota = rotaFrete;
                    preCarga.Observacao = observacao;
                    preCarga.Peso = peso;
                    preCarga.QuantidadePallet = quantidadePallets;

                    repPreCarga.Inserir(preCarga);
                    Servicos.Auditoria.Auditoria.Auditar(auditado, preCarga, null, $"Gerou o pré planejamneto {preCarga.NumeroPreCarga} via importação da planilha.", unitOfWork);

                    Dominio.Entidades.Localidade destinoCidade = null;
                    if (colDestinoCidade != null && colDestinoUF != null)
                    {
                        destinoCidade = repLocalidade.BuscarPorDescricaoEUF((string)colDestinoCidade.Valor, (string)colDestinoUF.Valor);
                        if (destinoCidade != null)
                        {
                            repositorioPreCargaDestino.Inserir(new Dominio.Entidades.Embarcador.PreCargas.PreCargaDestino()
                            {
                                PreCarga = preCarga,
                                Localidade = destinoCidade
                            });

                            destinoAdicionado = true;
                        }
                    }
                    else
                    {
                        Dominio.Entidades.Estado destionEstado = null;
                        if (colDestinoUF != null)
                        {
                            destionEstado = repEstado.BuscarPorSigla((string)colDestinoUF.Valor);
                            if (destionEstado != null)
                            {
                                repositorioPreCargaEstadoDestino.Inserir(new Dominio.Entidades.Embarcador.PreCargas.PreCargaEstadoDestino()
                                {
                                    PreCarga = preCarga,
                                    Estado = destionEstado
                                });

                                destinoAdicionado = true;
                            }
                        }
                    }

                    if (preCarga.ProgramacaoCarga && !destinoAdicionado)
                    {
                        retornoImportacao.Retornolinhas.Add(RetornarFalhaLinha("Nenhum destino encontrado", i));
                        unitOfWork.Rollback();
                        continue;
                    }

                    new Servicos.Embarcador.Carga.CargaDadosSumarizados(unitOfWork).AlterarDadosSumarizadosPreCarga(preCarga, unitOfWork, tipoServicoMultisoftware);
                    Dominio.Entidades.Embarcador.Cargas.CargaJanelaCarregamento cargaJanelaCarregamento = CriarCargaJanelaCarregamento(preCarga);
                    servicoFluxoGestaoPatio.Adicionar(preCarga, tipoServicoMultisoftware, cargaJanelaCarregamento);

                    if (configuracaoPreCarga.VincularFilaCarregamentoVeiculoAutomaticamente)
                    {
                        try
                        {
                            new Servicos.Embarcador.Logistica.FilaCarregamentoVeiculo(unitOfWork, auditado.Usuario, Servicos.Embarcador.Logistica.FilaCarregamentoVeiculo.ObterOrigemAlteracaoFilaCarregamento(tipoServicoMultisoftware)).AlocarParaPrimeiroDaFila(preCarga, tipoServicoMultisoftware);
                        }
                        catch
                        {
                            new Servicos.Embarcador.PreCarga.PreCargaOfertaTransportador(unitOfWork).DisponibilizarParaTransportadorPorRota(preCarga);
                        }
                    }

                    vinculacaoComSucesso++;
                    Dominio.ObjetosDeValor.Embarcador.Importacao.RetonoLinha retornoLinha = new Dominio.ObjetosDeValor.Embarcador.Importacao.RetonoLinha() { indice = i, processou = true, mensagemFalha = "" };
                    retornoImportacao.Retornolinhas.Add(retornoLinha);
                    unitOfWork.CommitChanges();

                    if (cargaJanelaCarregamento != null)
                        new Servicos.Embarcador.Hubs.JanelaCarregamento().InformarJanelaCarregamentoAtualizada(cargaJanelaCarregamento);
                }
                catch (Dominio.Excecoes.Embarcador.ServicoException ex)
                {
                    unitOfWork.Rollback();
                    Servicos.Log.TratarErro(ex);
                    retornoImportacao.Retornolinhas.Add(RetornarFalhaLinha("Ocorreu uma falha ao processar a linha.", i));
                }
            }

            retornoImportacao.MensagemAviso = "";
            retornoImportacao.Total = linhas.Count;
            retornoImportacao.Importados = vinculacaoComSucesso;
            return retornoImportacao;
        }

        public void CalculoFreteCotacaoPedido(Dominio.Entidades.Embarcador.Cargas.Carga carga, List<Dominio.Entidades.Embarcador.Cargas.CargaPedido> cargaPedidos, Dominio.Entidades.Embarcador.Configuracao.ConfiguracaoTMS configuracaoEmbarcador, AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware tipoServicoMultisoftware, Repositorio.UnitOfWork unitOfWork, bool recalcularFretePeloBID)
        {
            if (!(carga.TipoOperacao?.ConfiguracaoCalculoFrete?.CalcularFretePeloBIDPedidoOrigem ?? false) || carga.PossuiPendencia && !recalcularFretePeloBID)
                return;

            Repositorio.Embarcador.Cargas.Carga repositorioCarga = new Repositorio.Embarcador.Cargas.Carga(unitOfWork);
            Repositorio.Embarcador.Pedidos.Pedido repositorioPedido = new Repositorio.Embarcador.Pedidos.Pedido(unitOfWork);
            Repositorio.Embarcador.Cargas.CargaPedido repositorioCargaPedido = new Repositorio.Embarcador.Cargas.CargaPedido(unitOfWork);
            Repositorio.Embarcador.Pedidos.PedidoAdicional repositorioPedidoAdicional = new Repositorio.Embarcador.Pedidos.PedidoAdicional(unitOfWork);
            Repositorio.Embarcador.CotacaoPedido.CotacaoPedido repositorioCotacaoPedido = new Repositorio.Embarcador.CotacaoPedido.CotacaoPedido(unitOfWork);

            Servicos.Embarcador.Carga.RateioFrete servicoRateioFrete = new Carga.RateioFrete();
            Servicos.Embarcador.Carga.Carga servicoCarga = new Servicos.Embarcador.Carga.Carga(unitOfWork);

            if (carga.PossuiPendencia)
                servicoCarga.VerificarCalculoFretePeloBIDPedidoOrigem(carga, cargaPedidos, unitOfWork);

            if (carga.PossuiPendencia)
                return;

            List<Dominio.Entidades.Embarcador.Pedidos.PedidoAdicional> pedidosAdicional = repositorioPedidoAdicional.BuscarPorPedidos(cargaPedidos.Select(o => o.Pedido.Codigo).ToList());

            foreach (Dominio.Entidades.Embarcador.Cargas.CargaPedido cargaPedido in cargaPedidos)
            {
                Dominio.Entidades.Embarcador.Pedidos.PedidoAdicional pedidoAdicional = pedidosAdicional.FirstOrDefault(o => o.Pedido.Codigo == cargaPedido.Pedido.Codigo);
                Dominio.Entidades.Embarcador.CotacaoPedido.CotacaoPedido cotacaoPedido = repositorioCotacaoPedido.BuscarPorPedidoTransportador(pedidoAdicional.PedidoOrigem.Codigo, carga.Empresa.Codigo);
                Dominio.Entidades.Embarcador.Pedidos.Pedido pedido = cargaPedido.Pedido;

                decimal pesoPedido = (cargaPedido.Peso / 1000);
                decimal calculoCotacaoPedido = (pesoPedido * cotacaoPedido.ValorFreteTonelada);

                pedido.ValorFreteNegociado = calculoCotacaoPedido;

                cargaPedido.CotacaoPedido = cotacaoPedido;
                cargaPedido.ValorFrete = calculoCotacaoPedido;
                cargaPedido.ValorFreteAPagar = calculoCotacaoPedido;

                repositorioPedido.Atualizar(pedido);
                repositorioCargaPedido.Atualizar(cargaPedido);
            }

            carga.ValorFrete = cargaPedidos.Sum(obj => obj.ValorFrete);
            carga.ValorFreteAPagar = cargaPedidos.Sum(obj => obj.ValorFreteAPagar);
            carga.ValorFreteLiquido = carga.ValorFrete;
            carga.ValorFreteEmbarcador = cargaPedidos.Sum(obj => obj.ValorFreteAPagar);

            servicoRateioFrete.RatearValorDoFrenteEntrePedidos(carga, cargaPedidos, configuracaoEmbarcador, false, unitOfWork, tipoServicoMultisoftware);
        }

        #endregion

        #region Métodos Privados

        private void DeletarPreCarga(Dominio.Entidades.Embarcador.PreCargas.PreCarga preCarga)
        {
            Repositorio.Embarcador.PreCargas.PreCarga repositorioPreCarga = new Repositorio.Embarcador.PreCargas.PreCarga(_unitOfWork);
            Repositorio.Embarcador.GestaoPatio.FluxoGestaoPatio repositorioFluxoGestaoPatio = new Repositorio.Embarcador.GestaoPatio.FluxoGestaoPatio(_unitOfWork);
            Servicos.Embarcador.GestaoPatio.FluxoGestaoPatio servicoFluxoGestaoPatio = new Servicos.Embarcador.GestaoPatio.FluxoGestaoPatio(_unitOfWork);
            Dominio.Entidades.Embarcador.GestaoPatio.FluxoGestaoPatio fluxoPreCarga = servicoFluxoGestaoPatio.ObterFluxoGestaoPatio(preCarga);

            preCarga.NumeroPreCarga = "";
            preCarga.Carga = null;

            preCarga.ModeloVeicularCarga = null;
            preCarga.Empresa = null;
            preCarga.TipoDeCarga = null;
            preCarga.TipoOperacao = null;
            preCarga.Filial = null;
            preCarga.SituacaoPreCarga = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoPreCarga.Cancelada;
            preCarga.NumeroPreCarga = null;
            preCarga.DataPrevisaoEntrega = null;
            preCarga.DataImportacao = null;
            preCarga.JustificativaCancelamento = "Cancelada pelo sistema. Divergência de dados na importação.";

            if (fluxoPreCarga != null)
            {
                fluxoPreCarga.PreCarga = null;
                fluxoPreCarga.SituacaoEtapaFluxoGestaoPatio = SituacaoEtapaFluxoGestaoPatio.Rejeitado;

                repositorioFluxoGestaoPatio.Atualizar(fluxoPreCarga);
            }

            repositorioPreCarga.Atualizar(preCarga);
        }

        private void RemoveCargaDaGestaoPatio(Dominio.Entidades.Embarcador.PreCargas.PreCarga preCarga, int carga)
        {
            Servicos.Embarcador.GestaoPatio.FluxoGestaoPatio servicoFluxoGestaoPatio = new Servicos.Embarcador.GestaoPatio.FluxoGestaoPatio(_unitOfWork);
            Dominio.Entidades.Embarcador.GestaoPatio.FluxoGestaoPatio fluxoGestaoPatio = servicoFluxoGestaoPatio.ObterFluxoGestaoPatio(preCarga);

            if (fluxoGestaoPatio != null && fluxoGestaoPatio.Carga != null && fluxoGestaoPatio.Carga.Codigo == carga)
                servicoFluxoGestaoPatio.DefinirCargaPorPreCarga(preCarga);
        }

        private Dominio.ObjetosDeValor.Embarcador.Importacao.RetonoLinha RetornarFalhaLinha(string mensagem, int indice)
        {
            Dominio.ObjetosDeValor.Embarcador.Importacao.RetonoLinha retorno = new Dominio.ObjetosDeValor.Embarcador.Importacao.RetonoLinha() { indice = indice, mensagemFalha = mensagem, processou = false };
            return retorno;
        }

        private string CriarFornecedor(ref Dominio.Entidades.Cliente participante, string codigoIntegracao)
        {
            string retorno = "";
            Servicos.Cliente serCliente = new Cliente(_unitOfWork.StringConexao);
            Repositorio.Cliente repCliente = new Repositorio.Cliente(_unitOfWork);

            double cpf = repCliente.BuscarPorProximoExterior();

            Dominio.ObjetosDeValor.CTe.Cliente clienteEmbarcador = new Dominio.ObjetosDeValor.CTe.Cliente
            {
                Emails = "",
                Bairro = "Não Definido",
                CEP = "",
                Endereco = "Não Definido",
                Complemento = "",
                Numero = "S/N",
                Telefone1 = "",
                Telefone2 = "",
                Exportacao = true,
                CodigoIBGECidade = 0,
                NomeFantasia = codigoIntegracao.Length > 80 ? codigoIntegracao.Substring(0, 80) : codigoIntegracao,
                RGIE = "",
                CodigoCliente = codigoIntegracao,
                RazaoSocial = codigoIntegracao.Length > 80 ? codigoIntegracao.Substring(0, 80) : codigoIntegracao,
                CPFCNPJ = cpf.ToString(),
                CodigoAtividade = 3
            };

            Dominio.ObjetosDeValor.RetornoVerificacaoCliente retornoCliente = serCliente.converterClienteEmbarcador(clienteEmbarcador, "Remetente", _unitOfWork);
            if (retornoCliente.Status)
                participante = retornoCliente.cliente;
            else
                retorno = retornoCliente.Mensagem;

            return retorno;
        }

        private Dominio.Entidades.Embarcador.PreCargas.PreCarga CriarPreCargaImportada(string numeroPreCarga, Dominio.Entidades.Empresa empresa, Dominio.Entidades.Embarcador.Cargas.TipoDeCarga tipoCarga, Dominio.Entidades.Embarcador.Cargas.ModeloVeicularCarga modeloVeicularCarga, Dominio.Entidades.Embarcador.Pedidos.TipoOperacao tipoOperacao, Dominio.Entidades.Usuario usuario, Dominio.Entidades.Embarcador.Filiais.Filial filial, Dominio.Entidades.Embarcador.Cargas.FaixaTemperatura faixaTemperatura)
        {
            Repositorio.Embarcador.PreCargas.PreCarga repositorioPreCarga = new Repositorio.Embarcador.PreCargas.PreCarga(_unitOfWork);

            Dominio.Entidades.Embarcador.PreCargas.PreCarga preCarga = new Dominio.Entidades.Embarcador.PreCargas.PreCarga
            {
                ModeloVeicularCarga = modeloVeicularCarga,
                Empresa = empresa,
                TipoDeCarga = tipoCarga,
                TipoOperacao = tipoOperacao,
                FaixaTemperatura = faixaTemperatura,
                Filial = filial,
                SituacaoPreCarga = SituacaoPreCarga.AguardandoGeracaoCarga,
                NumeroPreCarga = numeroPreCarga,
                Destinatarios = new List<Dominio.Entidades.Cliente>(),
                DataPrevisaoEntrega = DateTime.Now,
                DataImportacao = DateTime.Now,
                DataCriacaoPreCarga = DateTime.Now,
                Operador = usuario
            };

            repositorioPreCarga.Inserir(preCarga);

            return preCarga;
        }

        private bool AdicionarPedidoImportacao(
            Dominio.Entidades.Embarcador.Configuracao.ConfiguracaoTMS configuracaoTMS,
            Dominio.Entidades.Empresa empresa,
            Dominio.Entidades.Embarcador.PreCargas.PreCarga preCarga,
            string numeroPedido,
            decimal pesoPedido,
            DateTime? previsaoEntrega,
            Dominio.Entidades.Embarcador.Cargas.TipoDeCarga tipoCarga,
            Dominio.Entidades.Embarcador.Cargas.ModeloVeicularCarga modeloVeicularCarga,
            Dominio.Entidades.Embarcador.Pedidos.TipoOperacao tipoOperacao,
            Dominio.Entidades.Cliente destinatario,
            Dominio.Entidades.Cliente remetente,
            Dominio.Entidades.Embarcador.Filiais.Filial filial,
            Dominio.Entidades.Usuario usuario,
            Dominio.ObjetosDeValor.Embarcador.Auditoria.Auditado auditado,
            out string erro,
            out Dominio.Entidades.Embarcador.Pedidos.Pedido pedido
        )
        {
            erro = "";

            Servicos.Embarcador.Pedido.Pedido svcPedido = new Pedido.Pedido(_unitOfWork);

            Repositorio.Embarcador.Pedidos.Pedido repPedido = new Repositorio.Embarcador.Pedidos.Pedido(_unitOfWork);
            Repositorio.Cliente repCliente = new Repositorio.Cliente(_unitOfWork);
            Repositorio.Embarcador.Filiais.Filial repFilial = new Repositorio.Embarcador.Filiais.Filial(_unitOfWork);
            Repositorio.Embarcador.Pedidos.PedidoEndereco repPedidoEndereco = new Repositorio.Embarcador.Pedidos.PedidoEndereco(_unitOfWork);
            Repositorio.Embarcador.PreCargas.PreCarga repPreCarga = new Repositorio.Embarcador.PreCargas.PreCarga(_unitOfWork);
            Repositorio.Localidade repLocalidade = new Repositorio.Localidade(_unitOfWork);
            Repositorio.RotaFrete repRotaFrete = new Repositorio.RotaFrete(_unitOfWork);
            Repositorio.Embarcador.Pessoas.ClienteDescarga repClienteDescarga = new Repositorio.Embarcador.Pessoas.ClienteDescarga(_unitOfWork);

            pedido = null;

            if (preCarga != null && configuracaoTMS.UsarMesmoNumeroPreCargaGerarCargaViaImportacao)
            {
                if (!string.IsNullOrWhiteSpace(numeroPedido))
                    pedido = repPedido.BuscarPorNumeroEmbarcador(numeroPedido, filial?.Codigo ?? 0, "", false);
                else
                    pedido = repPedido.BuscarPorPreCargaRemetenteDestinatario(preCarga.Codigo, remetente.CPF_CNPJ, destinatario.CPF_CNPJ);
            }

            if (pedido == null)
            {
                Dominio.Entidades.Embarcador.Pedidos.PedidoEndereco pedidoEnderecoOrigem = new Dominio.Entidades.Embarcador.Pedidos.PedidoEndereco();
                Dominio.Entidades.Embarcador.Pedidos.PedidoEndereco pedidoEnderecoDestino = new Dominio.Entidades.Embarcador.Pedidos.PedidoEndereco();

                pedido = new Dominio.Entidades.Embarcador.Pedidos.Pedido
                {
                    CodigoPedidoCliente = "",
                    DataFinalColeta = DateTime.Now,
                    DataInicialColeta = DateTime.Now,
                    DataCarregamentoPedido = DateTime.Now,
                    Veiculos = new List<Dominio.Entidades.Veiculo>(),
                    QtdEntregas = 1,
                    PedidoTransbordo = false,
                    UsarOutroEnderecoOrigem = false,
                    UsarOutroEnderecoDestino = false,

                    Observacao = "",
                    ObservacaoCTe = "",
                    Temperatura = "",
                    PedidoIntegradoEmbarcador = false,
                    GerarAutomaticamenteCargaDoPedido = false,

                    Remetente = remetente,
                    Destinatario = destinatario,
                    GrupoPessoas = remetente.GrupoPessoas,
                    Origem = remetente.Localidade,

                    Requisitante = Dominio.ObjetosDeValor.Embarcador.Enumeradores.RequisitanteColeta.Remetente,
                    SituacaoPedido = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoPedido.Aberto,
                    TipoPagamento = Dominio.Enumeradores.TipoPagamento.Pago,
                    TipoTomador = Dominio.Enumeradores.TipoTomador.Remetente,
                    UltimaAtualizacao = DateTime.Now,
                    Usuario = usuario,
                    Autor = usuario,

                    ModeloVeicularCarga = modeloVeicularCarga,
                    PrevisaoEntrega = previsaoEntrega,
                    Empresa = empresa,
                    Filial = repFilial.BuscarPorCodigo(filial.Codigo),

                    AdicionadaManualmente = true
                };

                svcPedido.PreecherEnderecoPedido(ref pedidoEnderecoOrigem, pedido.Remetente);
                svcPedido.PreecherEnderecoPedido(ref pedidoEnderecoDestino, pedido.Destinatario);

                if (pedidoEnderecoDestino.Localidade != null)
                {
                    repPedidoEndereco.Inserir(pedidoEnderecoDestino);

                    pedido.Destino = pedidoEnderecoDestino.Localidade;
                    pedido.EnderecoDestino = pedidoEnderecoDestino;
                }

                if (pedidoEnderecoOrigem.Localidade != null)
                {
                    repPedidoEndereco.Inserir(pedidoEnderecoOrigem);

                    pedido.Origem = pedidoEnderecoOrigem.Localidade;
                    pedido.EnderecoOrigem = pedidoEnderecoOrigem;
                }

                if (pedido.Destino != null)
                {
                    pedido.Destino = repLocalidade.BuscarPorCodigo(pedido.Destino.Codigo);

                    pedido.RotaFrete = repRotaFrete.BuscarPorLocalidade(pedido.Destino, true);
                    if (pedido.RotaFrete == null)
                        pedido.RotaFrete = repRotaFrete.BuscarPorEstado(pedido.Destino.Estado.Sigla, true);
                }

                if (pedido.Destinatario != null)
                {
                    Dominio.Entidades.Embarcador.Pessoas.ClienteDescarga clienteDescarga = repClienteDescarga.BuscarPorPessoa(pedido.Destinatario.CPF_CNPJ);

                    if (clienteDescarga != null)
                    {
                        pedido.RestricoesDescarga = clienteDescarga.RestricoesDescarga.ToList();

                        foreach (var restricao in pedido.RestricoesDescarga)
                        {
                            string email = restricao.Email ?? "";
                            if (!string.IsNullOrWhiteSpace(email))
                            {
                                List<string> emails = email.Split(';').ToList();
                                bool sucesso = svcPedido.EnviarRelatorioDetalhesPedidoPorEmail(emails, pedido, restricao, _unitOfWork, out string mensagem);
                                if (!sucesso)
                                    Servicos.Log.TratarErro(mensagem, "EmailRestricao");
                            }
                        }
                    }
                }

                Dominio.Entidades.Cliente tomador = pedido.ObterTomador();

                if (!string.IsNullOrWhiteSpace(tipoCarga?.ProdutoPredominante))
                    pedido.ProdutoPredominante = tipoCarga?.ProdutoPredominante;
                if (!string.IsNullOrWhiteSpace(tipoOperacao?.ProdutoPredominanteOperacao))
                    pedido.ProdutoPredominante = tipoOperacao.ProdutoPredominanteOperacao;
                else if (!string.IsNullOrWhiteSpace(tomador?.GrupoPessoas?.ProdutoPredominante))
                    pedido.ProdutoPredominante = tomador.GrupoPessoas.ProdutoPredominante;
                else if (!string.IsNullOrWhiteSpace(configuracaoTMS.DescricaoProdutoPredominatePadrao))
                    pedido.ProdutoPredominante = configuracaoTMS.DescricaoProdutoPredominatePadrao;
                else
                    pedido.ProdutoPredominante = "Importação";

                if (string.IsNullOrEmpty(numeroPedido))
                {
                    if (configuracaoTMS.NumeroCargaSequencialUnico)
                        pedido.NumeroSequenciaPedido = repPedido.ObterProximoCodigo();
                    else
                        pedido.NumeroSequenciaPedido = repPedido.ObterProximoCodigo(pedido.Filial);

                    pedido.NumeroPedidoEmbarcador = pedido.NumeroSequenciaPedido.ToString();
                }
                else
                    pedido.NumeroPedidoEmbarcador = numeroPedido;

                pedido.DataInicialViagemExecutada = pedido.DataPrevisaoSaida;
                pedido.DataFinalViagemExecutada = pedido.PrevisaoEntrega;
                pedido.DataInicialViagemFaturada = pedido.DataPrevisaoSaida;
                pedido.DataFinalViagemFaturada = pedido.PrevisaoEntrega;

                Dominio.Entidades.Embarcador.Pedidos.Pedido pedidoExiste = null;

                if (!string.IsNullOrWhiteSpace(pedido.NumeroPedidoEmbarcador) && pedido.Filial != null)
                    pedidoExiste = repPedido.BuscarPorNumeroEmbarcador(pedido.NumeroPedidoEmbarcador, pedido.Filial.Codigo, true);

                if (pedidoExiste == null)
                {
                    pedido.PedidoIntegradoEmbarcador = true;
                    pedido.SituacaoAcompanhamentoPedido = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoAcompanhamentoPedido.AgColeta;
                    repPedido.Inserir(pedido);
                    Servicos.Auditoria.Auditoria.Auditar(auditado, pedido, null, "Criou Pedido via importação", _unitOfWork);
                }
                else
                {
                    erro = "O pedido informado (" + pedidoExiste.NumeroPedidoEmbarcador + ") já existe " + (pedido.Filial != null ? " para a filial " + pedido.Filial.Descricao : ".");
                    return false;
                }
            }

            pedido.Protocolo = pedido.Codigo;
            pedido.PesoTotal = pesoPedido;
            pedido.PesoSaldoRestante = pesoPedido;
            pedido.TipoOperacao = tipoOperacao;
            pedido.TipoDeCarga = tipoCarga;
            pedido.PreCarga = preCarga;
            pedido.DataPrevisaoChegadaDestinatario = preCarga.PrevisaoChegadaDestinatario;
            pedido.DataPrevisaoSaidaDestinatario = preCarga.PrevisaoSaidaDestinatario;

            repPedido.Atualizar(pedido);

            if (preCarga != null && preCarga.TipoOperacao == null && pedido.TipoOperacao != null)
            {
                preCarga.TipoOperacao = pedido.TipoOperacao;
                repPreCarga.Atualizar(preCarga);
            }

            if (pedido.Produtos == null)
                pedido.Produtos = new List<Dominio.Entidades.Embarcador.Pedidos.PedidoProduto>();

            return true;
        }

        public void AdicionarMensagemAtrasoImportacaoPreCarga(Dominio.Entidades.Embarcador.PreCargas.PreCarga preCarga, Dominio.Entidades.Embarcador.Cargas.Carga carga)
        {
            Carga.MensagemAlertaCarga servicoMensagemAlerta = new Carga.MensagemAlertaCarga(_unitOfWork);

            servicoMensagemAlerta.Adicionar(carga, TipoMensagemAlerta.ImportacaoCargaAtrasada, $"A importação dos pedidos dessa carga foi atrasada. Motivo {preCarga.MotivoImportacaoPedidoAtrasada.Descricao}");
        }

        #endregion

        #region Métodos Privados de Alteração de Dados da Pré Carga

        private bool AdicionarMensagemAlteracaoTransportadorPreCarga(List<string> mensagens, Dominio.Entidades.Embarcador.Cargas.Carga preCarga, Dominio.Entidades.Embarcador.Cargas.Carga carga)
        {
            if (!IsEmpresaInformadaPreCargaAlterada(preCarga, carga))
                return false;

            mensagens.Add($"Alteração de transportador ({DateTime.Now.ToString("dd/MM/yyyy HH:mm")}) | Pré Carga: {preCarga.Empresa?.Descricao} | Carga: {carga.Empresa?.Descricao}");

            return true;
        }

        private bool AdicionarMensagemAlteracaoTransportadorPreCargaAgrupada(List<string> mensagens, Dominio.Entidades.Embarcador.Cargas.Carga preCargaAgrupada, Dominio.Entidades.Embarcador.Cargas.Carga carga)
        {
            if (!IsEmpresaInformadaPreCargaAlterada(preCargaAgrupada, carga))
                return false;

            mensagens.Add($"Alteração de transportador ({DateTime.Now.ToString("dd/MM/yyyy HH:mm")}) | Carga ({carga.CodigoCargaEmbarcador}): {carga.Empresa?.Descricao} | Carga Agrupada: {preCargaAgrupada.Empresa?.Descricao}");

            return true;
        }

        private bool AdicionarAlteracaoVeiculoPreCarga(List<string> mensagens, Dominio.Entidades.Embarcador.Cargas.Carga preCarga, Dominio.Entidades.Embarcador.Cargas.Carga carga)
        {
            if (!IsVeiculoInformadoPreCargaAlterado(preCarga, carga))
                return false;

            mensagens.Add($"Alteração de veículo ({DateTime.Now.ToString("dd/MM/yyyy HH:mm")}) | Pré Carga: {preCarga.RetornarPlacas} | Carga: {carga.RetornarPlacas}");

            return true;
        }

        private bool AdicionarAlteracaoVeiculoPreCargaAgrupada(List<string> mensagens, Dominio.Entidades.Embarcador.Cargas.Carga preCargaAgrupada, Dominio.Entidades.Embarcador.Cargas.Carga carga)
        {
            if (!IsVeiculoInformadoPreCargaAlterado(preCargaAgrupada, carga))
                return false;

            mensagens.Add($"Alteração de veículo ({DateTime.Now.ToString("dd/MM/yyyy HH:mm")}) | Carga ({carga.CodigoCargaEmbarcador}): {carga.RetornarPlacas} | Carga Agrupada: {preCargaAgrupada.RetornarPlacas}");

            return true;
        }

        private bool IsEmpresaInformadaPreCargaAlterada(Dominio.Entidades.Embarcador.Cargas.Carga preCarga, Dominio.Entidades.Embarcador.Cargas.Carga carga)
        {
            return (preCarga.Empresa?.Codigo != carga.Empresa?.Codigo);
        }

        private bool IsVeiculoInformadoNaoPertenceEmpresaCarga(Dominio.Entidades.Embarcador.Cargas.Carga carga)
        {
            if (carga.Empresa == null)
                return false;

            Dominio.Entidades.Empresa transportador = ObterTransportadorPorVeiculo(carga);

            if (transportador == null)
                return false;

            return (transportador.Codigo != carga.Empresa.Codigo);
        }

        private bool IsVeiculoInformadoPreCargaAlterado(Dominio.Entidades.Embarcador.Cargas.Carga preCarga, Dominio.Entidades.Embarcador.Cargas.Carga carga)
        {
            if (preCarga.Veiculo?.Codigo != carga.Veiculo?.Codigo)
                return true;

            if (preCarga.VeiculosVinculados?.Count != carga.VeiculosVinculados?.Count)
                return true;

            if (preCarga.VeiculosVinculados != null)
            {
                foreach (Dominio.Entidades.Veiculo reboque in preCarga.VeiculosVinculados)
                {
                    if (!carga.VeiculosVinculados.Any(o => o.Codigo == reboque.Codigo))
                        return true;
                }
            }

            return false;
        }

        private Dominio.Entidades.Empresa ObterTransportadorPorVeiculo(Dominio.Entidades.Embarcador.Cargas.Carga carga)
        {
            if (carga.Veiculo?.Empresa != null)
                return carga.Veiculo.Empresa;

            if (carga.VeiculosVinculados?.Count > 0)
                return (from o in carga.VeiculosVinculados where o.Empresa != null select o.Empresa).FirstOrDefault();

            return null;
        }

        #endregion
    }
}
