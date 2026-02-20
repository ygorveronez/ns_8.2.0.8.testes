using System;
using Dominio.ObjetosDeValor.Embarcador.Enumeradores;
using System.Collections.Generic;
using System.Linq;
using Dominio.ObjetosDeValor.Embarcador.Integracao.SistemaTransben;
using AdminMultisoftware.Dominio.Enumeradores;
using Dominio.Entidades.Embarcador.Cargas;

namespace Servicos.Embarcador.Integracao.SistemaTransben
{
    public partial class IntegracaoSistemaTransben
    {
        #region Métodos Públicos

        public void IntegrarCarga(Dominio.Entidades.Embarcador.Cargas.CargaCargaIntegracao cargaIntegracao)
        {
            Repositorio.Embarcador.Cargas.CargaCargaIntegracao repositorioCargaIntegracao = new Repositorio.Embarcador.Cargas.CargaCargaIntegracao(_unitOfWork);
            ArquivoTransacao<Dominio.Entidades.Embarcador.Cargas.CargaCTeIntegracaoArquivo> servicoArquivoTransacao = new ArquivoTransacao<Dominio.Entidades.Embarcador.Cargas.CargaCTeIntegracaoArquivo>(_unitOfWork);

            string jsonRequisicao = "";
            string jsonRetorno = "";

            try
            {
                cargaIntegracao.DataIntegracao = DateTime.Now;
                cargaIntegracao.NumeroTentativas++;

                string token = ObterToken("usuarios/login", _configuracaoIntegracao.URLSistemaTransben);

                var requisicao = CriarObjetoCarga(cargaIntegracao.Carga);

                var retornoWS = Transmitir(requisicao, "cargas/cadastrar", token, _configuracaoIntegracao.URLSistemaTransben);

                cargaIntegracao.SituacaoIntegracao = retornoWS.SituacaoIntegracao;
                cargaIntegracao.ProblemaIntegracao = retornoWS.ProblemaIntegracao;
                jsonRequisicao = retornoWS.JsonRequisicao;
                jsonRetorno = retornoWS.JsonRetorno;
            }
            catch (Exception excecao)
            {
                Log.TratarErro(excecao);

                cargaIntegracao.SituacaoIntegracao = SituacaoIntegracao.ProblemaIntegracao;

                String message = excecao.Message;
                if (message.Length > 300)
                {
                    message = message.Substring(0, 300);
                }
                cargaIntegracao.ProblemaIntegracao = message;
            }

            servicoArquivoTransacao.Adicionar(cargaIntegracao, jsonRequisicao, jsonRetorno, "json");

            repositorioCargaIntegracao.Atualizar(cargaIntegracao);
        }

        #endregion

        #region Métodos Privados

        private EnvioCarga CriarObjetoCarga(Dominio.Entidades.Embarcador.Cargas.Carga carga)
        {
            Servicos.Embarcador.Carga.CargaDadosSumarizados serCargaDadosSumarizados = new Servicos.Embarcador.Carga.CargaDadosSumarizados(_unitOfWork);
            Repositorio.Embarcador.Cargas.CargaPedido repCargaPedido = new Repositorio.Embarcador.Cargas.CargaPedido(_unitOfWork);

            Dominio.Entidades.Embarcador.Cargas.CargaPedido cargaPedido = repCargaPedido.BuscarPorUnicaCarga(carga.Codigo);

            EnvioCarga objetoCarga = new EnvioCarga()
            {
                numeroDaCarga = carga.CodigoCargaEmbarcador,
                dataDeCriacao = FormatarData(carga.DataCriacaoCarga),
                tipoDaOperacao = carga.TipoOperacao?.CodigoIntegracao ?? "Código não disponível!",
                origem = FormatarOrigem(carga.DadosSumarizados),
                destino = serCargaDadosSumarizados.ObterDestino(carga, true, TipoServicoMultisoftware.MultiTMS, false) ?? "Destino não disponível!",
                operacao = carga.TipoOperacao?.Descricao ?? "Descrição não disponível!",
                tomadorCnpj = ObterTomador(cargaPedido),
                situacao = carga.DescricaoSituacaoCarga ?? "Situação não disponível!",
                tipoDecarga = carga.TipoDeCarga?.Descricao ?? "Descrição não disponível!",
                tipoDeVeiculoSolicitado = carga.ModeloVeicularCarga?.Descricao ?? "Descrição não disponível!",
                valorFrete = carga.ValorFreteAPagar,
                peso = carga.DadosSumarizados?.PesoTotal ?? 0M,
                valorNF = ObterValorTotalNotasFiscais(cargaPedido),
                pesoLiquido = carga.DadosSumarizados?.PesoLiquidoTotal ?? 0M,
                Gestor = carga.Veiculo?.FuncionarioResponsavel?.Nome ?? "Gestor não definido!",
                dataCarregamento = FormatarData(carga.DataCarregamentoCarga),
                cargaPerigosa = carga.CargaPerigosaIntegracaoLeilao,
                empresaFilial = FormatarNomeEmpresa(carga.Empresa),
                placas = ObterPlacas(carga),
                frota = BuscarNumeroFrotasVeiculos(carga),
                motorista = carga.NomeMotoristas ?? "Motorista não informado!",
                recebedor = cargaPedido.Recebedor?.NomeCNPJ ?? "Receber não informado!",
                rota = carga.Rota?.Codigo.ToString() ?? "Não foram encontrados os dados da rota!",
                valorTotalDosProdutos = carga.DadosSumarizados?.ValorTotalProdutos ?? 0M,
                QuantidadeNfs = cargaPedido.NotasFiscais?.Count() ?? 0,
                pedidos = ObterPedidos(carga)
            };

            return objetoCarga;
        }

        private string FormatarData(DateTime? date, string format = "yyyy-MM-dd HH:mm") => date?.ToString(format) ?? "Data não disponível!";

        private string FormatarOrigem(CargaDadosSumarizados dados) => dados != null ? $"{dados.Remetentes} ({dados.Origens})" : "Origem não disponível";

        private decimal ObterValorTotalNotasFiscais(CargaPedido cargaPedido) => cargaPedido.NotasFiscais?.Select(nota => nota.XMLNotaFiscal.Valor).Sum() ?? 0m;

        private string FormatarNomeEmpresa(Dominio.Entidades.Empresa empresa) => empresa != null ? $"{empresa.RazaoSocial} ({empresa.Localidade?.DescricaoCidadeEstado})" : "Empresa não informada";

        private string ObterTomador(Dominio.Entidades.Embarcador.Cargas.CargaPedido cargaPedido)
        {
            var pedido = cargaPedido.Pedido;

            if (pedido == null)
                return string.Empty;

            if (pedido?.TipoPagamento == null)
                return string.Empty;

            return pedido.TipoPagamento switch
            {
                Dominio.Enumeradores.TipoPagamento.Pago => pedido.Remetente?.CPF_CNPJ_SemFormato ?? string.Empty,
                Dominio.Enumeradores.TipoPagamento.A_Pagar => pedido.Destinatario?.CPF_CNPJ_SemFormato ?? string.Empty,
                Dominio.Enumeradores.TipoPagamento.Outros => cargaPedido.Tomador?.CPF_CNPJ_SemFormato ?? string.Empty,
                _ => string.Empty
            };
        }


        private string ObterPlacas(Dominio.Entidades.Embarcador.Cargas.Carga carga)
        {
            List<string> placas = new List<string>();

            if (carga.Veiculo != null)
                placas.Add(carga.Veiculo.Placa);

            if (carga.VeiculosVinculados?.Count > 0)
                placas.AddRange(from veiculo in carga.VeiculosVinculados select $"{veiculo.Placa}");

            return string.Join(", ", placas);
        }

        private string BuscarNumeroFrotasVeiculos(Dominio.Entidades.Embarcador.Cargas.Carga carga)
        {
            List<string> numeroFrotasVeiculosVinculados = new List<string>();

            if (!string.IsNullOrWhiteSpace(carga.Veiculo?.NumeroFrota))
                numeroFrotasVeiculosVinculados.Add(carga.Veiculo.NumeroFrota);

            if (carga.VeiculosVinculados != null && carga.VeiculosVinculados.Count() > 0)
            {
                foreach (Dominio.Entidades.Veiculo veiculo in carga.VeiculosVinculados)
                {
                    if (!string.IsNullOrWhiteSpace(veiculo.NumeroFrota))
                        numeroFrotasVeiculosVinculados.Add(veiculo.NumeroFrota);
                }
            }

            return string.Join(", ", numeroFrotasVeiculosVinculados);
        }

        private List<Dominio.ObjetosDeValor.Embarcador.Integracao.SistemaTransben.Pedido> ObterPedidos(Dominio.Entidades.Embarcador.Cargas.Carga carga)
        {
            List<Dominio.ObjetosDeValor.Embarcador.Integracao.SistemaTransben.Pedido> pedidos = new List<Dominio.ObjetosDeValor.Embarcador.Integracao.SistemaTransben.Pedido>();

            foreach(Dominio.Entidades.Embarcador.Cargas.CargaPedido cargaPedido in carga.Pedidos)
            {
                Dominio.ObjetosDeValor.Embarcador.Integracao.SistemaTransben.Pedido pedido = new Dominio.ObjetosDeValor.Embarcador.Integracao.SistemaTransben.Pedido()
                {
                    numeroDaCarga = cargaPedido.Carga?.CodigoCargaEmbarcador,
                    localPalletizacao = cargaPedido.Pedido?.LocalPaletizacao?.CPF_CNPJ.ToString() ?? "Local não informado!",
                    numeroDoPedido = cargaPedido.Pedido?.Numero.ToString() ?? "Número não disponível!",
                    dataAgendamento = FormatarData(cargaPedido.Pedido?.DataAgendamento),
                    CTEs = ObterCTes(cargaPedido)
                };

                pedidos.Add(pedido);
            }

            return pedidos;
        }

        private List<Dominio.ObjetosDeValor.Embarcador.Integracao.SistemaTransben.CTe> ObterCTes(Dominio.Entidades.Embarcador.Cargas.CargaPedido cargaPedido)
        {
            List<Dominio.ObjetosDeValor.Embarcador.Integracao.SistemaTransben.CTe> ctes = new List<Dominio.ObjetosDeValor.Embarcador.Integracao.SistemaTransben.CTe>();

            foreach (Dominio.Entidades.Embarcador.Cargas.CargaCTe cargaCTe in cargaPedido.Carga.CargaCTes)
            {
                Dominio.ObjetosDeValor.Embarcador.Integracao.SistemaTransben.CTe cte = new Dominio.ObjetosDeValor.Embarcador.Integracao.SistemaTransben.CTe()
                {
                    numeroDoCte = cargaCTe.CTe?.Numero.ToString() ?? "Número do CTe não informado!",
                    numeroDaCarga = cargaCTe.Carga?.CodigoCargaEmbarcador.ToString() ?? "Número da carga não informado!",
                    numeroDoPedido = cargaPedido.Pedido?.Numero.ToString() ?? "Número do pedido não informado!",
                    serie = cargaCTe.CTe?.Serie?.Numero ?? 0,
                    localidadeEmissao = cargaCTe.CTe?.LocalidadeEmissao?.DescricaoCidadeEstado ?? "Local de emissão não encontrado!",
                    inicioPrestacao = cargaCTe.CTe?.LocalidadeInicioPrestacao?.DescricaoCidadeEstado ?? "Local de início não encontrado!",
                    terminoPrestacao = cargaCTe.CTe?.LocalidadeTerminoPrestacao?.DescricaoCidadeEstado ?? "Local de término não encontrado!",
                    dataEmissao = FormatarData(cargaCTe.CTe?.DataEmissao),
                    tipoDeServico = cargaCTe.CTe?.DescricaoTipoServico ?? "Tipo de serviço não especificado!",
                    destino = cargaCTe.CTe?.Destinatario?.Localidade?.DescricaoCidadeEstado ?? "Destino não encontrado!",
                    valorReceber = cargaCTe.CTe?.ValorAReceber ?? 0M,
                    chave = cargaCTe.CTe?.Chave ?? "Chave do CTe não encontrada!",
                    remetente = cargaCTe.CTe?.Remetente != null ? ObterInformacoesEmpresa(cargaCTe.CTe.Remetente) : new InformacoesEmpresa { razaoSocial = "Remetente não informado"},
                    destinatario = cargaCTe.CTe?.Destinatario != null ? ObterInformacoesEmpresa(cargaCTe.CTe.Destinatario) : new InformacoesEmpresa { razaoSocial = "Destinatário não informado" },
                    NFs = ObterNotasFiscais(cargaCTe)
                };

                ctes.Add(cte);
            }

            return ctes;
        }

        private Dominio.ObjetosDeValor.Embarcador.Integracao.SistemaTransben.InformacoesEmpresa ObterInformacoesEmpresa(Dominio.Entidades.ParticipanteCTe empresa)
        {
            Dominio.ObjetosDeValor.Embarcador.Integracao.SistemaTransben.InformacoesEmpresa informacoesEmpresa = new Dominio.ObjetosDeValor.Embarcador.Integracao.SistemaTransben.InformacoesEmpresa()
            {
                cpfCnpj = empresa.CPF_CNPJ ?? "O CPF/CNPJ não foi encontrado!",
                IE = empresa.IE_RG ?? empresa.Cliente?.IE_RG ?? empresa.Cliente?.IE_RG ?? "O IE não foi encontrado!",
                razaoSocial = empresa.Cliente?.Nome ?? "A razão social não foi encontrada!",
                nomeFantasia = empresa.Cliente?.NomeFantasia ?? "O nome fantasia não foi encontrado!"
            };

            return informacoesEmpresa;
        }

        private List<Dominio.ObjetosDeValor.Embarcador.Integracao.SistemaTransben.NotaFiscal> ObterNotasFiscais(Dominio.Entidades.Embarcador.Cargas.CargaCTe cargaCTe)
        {
            List<Dominio.ObjetosDeValor.Embarcador.Integracao.SistemaTransben.NotaFiscal> notasFiscais = new List<Dominio.ObjetosDeValor.Embarcador.Integracao.SistemaTransben.NotaFiscal>();

            foreach (Dominio.Entidades.Embarcador.Cargas.CargaPedidoXMLNotaFiscalCTe notaFiscalCTe in cargaCTe.NotasFiscais)
            {
                Dominio.ObjetosDeValor.Embarcador.Integracao.SistemaTransben.NotaFiscal notaFiscal = new Dominio.ObjetosDeValor.Embarcador.Integracao.SistemaTransben.NotaFiscal()
                {
                    numeroDaCarga = notaFiscalCTe.CargaCTe?.Carga?.CodigoCargaEmbarcador ?? "O número da carga não foi encontrado!",
                    numeroDoCte = notaFiscalCTe.CargaCTe?.CTe?.Numero.ToString() ?? "O número do CTe não foi encontrado!",
                    numeroDoPedido = notaFiscalCTe.PedidoXMLNotaFiscal?.CargaPedido?.Pedido?.Numero.ToString() ?? "O número do pedido não foi encontrado!",
                    numerodanf = notaFiscalCTe.PedidoXMLNotaFiscal?.XMLNotaFiscal?.Numero.ToString() ?? "O número da DANFE não foi encontrado!",
                    origem = notaFiscalCTe.PedidoXMLNotaFiscal?.XMLNotaFiscal?.Emitente?.Localidade?.DescricaoCidadeEstado ?? "Local de origem não encontrado!",
                    destino = notaFiscalCTe.PedidoXMLNotaFiscal?.XMLNotaFiscal?.Destinatario?.Localidade?.DescricaoCidadeEstado ?? "Local de destino não encontrado!",
                    peso = notaFiscalCTe.PedidoXMLNotaFiscal?.XMLNotaFiscal?.Peso ?? 0M,
                    valorNF = notaFiscalCTe.PedidoXMLNotaFiscal?.XMLNotaFiscal?.Valor ?? 0M,
                    pesoLiquido = notaFiscalCTe.PedidoXMLNotaFiscal?.XMLNotaFiscal?.PesoLiquido ?? 0M,
                    volumes = notaFiscalCTe.PedidoXMLNotaFiscal?.XMLNotaFiscal?.Volumes ?? 0,
                    remetente = notaFiscalCTe.PedidoXMLNotaFiscal?.XMLNotaFiscal?.Emitente?.NomeCNPJ ?? "O remetente não foi encontrado!",
                    destinatario = notaFiscalCTe.PedidoXMLNotaFiscal?.XMLNotaFiscal?.Destinatario?.NomeCNPJ ?? "O destinatário não foi encontrado!",
                    quantidadePallets = (int)(notaFiscalCTe.PedidoXMLNotaFiscal?.XMLNotaFiscal?.QuantidadePallets ?? 0)
                };

                notasFiscais.Add(notaFiscal);
            }

            return notasFiscais;
        }

        #endregion Métodos Privados
    }
}
