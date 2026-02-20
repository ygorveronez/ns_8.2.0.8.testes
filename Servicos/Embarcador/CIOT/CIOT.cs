using AdminMultisoftware.Dominio.Enumeradores;
using Dominio.ObjetosDeValor.Embarcador.Enumeradores;
using Dominio.ObjetosDeValor.Relatorios;
using Servicos.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Servicos.Embarcador.CIOT
{
    public class CIOT
    {
        #region Métodos Públicos

        public static Dominio.Entidades.Embarcador.CIOT.ConfiguracaoCIOT ObterConfiguracaoCIOT(Dominio.Entidades.Cliente terceiro, Dominio.Entidades.Empresa empresa, Repositorio.UnitOfWork unitOfWork, Dominio.Entidades.Embarcador.Cargas.Carga carga = null)
        {
            if (carga != null && carga.Filial?.ConfiguracaoCIOT != null)
                return carga.Filial.ConfiguracaoCIOT;

            Dominio.Entidades.Embarcador.Pessoas.ModalidadeTransportadoraPessoas modalidadeTerceiro = ObterModalidadeTransportador(terceiro, unitOfWork);
            Repositorio.Embarcador.CIOT.ConfiguracaoCIOT repConfiguracaoCIOT = new Repositorio.Embarcador.CIOT.ConfiguracaoCIOT(unitOfWork);
            Dominio.Entidades.Embarcador.CIOT.ConfiguracaoCIOT configuracaoCIOT = modalidadeTerceiro?.ConfiguracaoCIOTWithTipoTerceiro;

            if (configuracaoCIOT == null)
                configuracaoCIOT = empresa?.ConfiguracaoCIOT;

            if (configuracaoCIOT == null)
            {
                Repositorio.Embarcador.Configuracoes.Integracao repIntegracao = new Repositorio.Embarcador.Configuracoes.Integracao(unitOfWork);
                Dominio.Entidades.Embarcador.Configuracoes.Integracao integracao = repIntegracao.Buscar();
                configuracaoCIOT = integracao?.ConfiguracaoCIOT;
            }

            return configuracaoCIOT;
        }

        public static Dominio.Entidades.Embarcador.Pessoas.ModalidadeTransportadoraPessoas ObterModalidadeTransportador(Dominio.Entidades.Cliente terceiro, Repositorio.UnitOfWork unitOfWork)
        {
            if (terceiro == null)
                return null;

            Repositorio.Embarcador.Pessoas.ModalidadeTransportadoraPessoas repModalidadeTerceiro = new Repositorio.Embarcador.Pessoas.ModalidadeTransportadoraPessoas(unitOfWork);
            Dominio.Entidades.Embarcador.Pessoas.ModalidadeTransportadoraPessoas modalidadeTerceiro = repModalidadeTerceiro.BuscarPorPessoa(terceiro.CPF_CNPJ);
            return modalidadeTerceiro;
        }

        public static bool VerificarSeGeraCIOTParaTodos(Dominio.Entidades.Embarcador.Cargas.Carga carga, Dominio.Entidades.Embarcador.Configuracao.ConfiguracaoTMS configuracaoTMS, Repositorio.UnitOfWork unitOfWork)
        {
            bool gerarCIOTParaTodos = configuracaoTMS.GerarCIOTParaTodasAsCargas;

            if (!gerarCIOTParaTodos && carga.Filial != null)
                gerarCIOTParaTodos = carga.Filial.GerarCIOTParaTodasAsCargas;

            if (!gerarCIOTParaTodos)
            {
                if (carga.TipoOperacao?.UsarConfiguracaoEmissao ?? false)
                {
                    gerarCIOTParaTodos = carga.TipoOperacao.GerarCIOTParaTodasAsCargas;
                }
                else
                {
                    Repositorio.Embarcador.Cargas.CargaPedido repCargaPedido = new Repositorio.Embarcador.Cargas.CargaPedido(unitOfWork);

                    Dominio.Entidades.Embarcador.Cargas.CargaPedido cargaPedido = repCargaPedido.BuscarPrimeiraPorCarga(carga.Codigo);

                    Dominio.Entidades.Cliente tomador = cargaPedido?.ObterTomador();

                    if (tomador != null)
                    {
                        if (tomador.NaoUsarConfiguracaoEmissaoGrupo)
                            gerarCIOTParaTodos = tomador.GerarCIOTParaTodasAsCargas;
                        else if (tomador.GrupoPessoas != null)
                            gerarCIOTParaTodos = tomador.GrupoPessoas.GerarCIOTParaTodasAsCargas;
                    }
                }
            }

            if (!gerarCIOTParaTodos)
                gerarCIOTParaTodos = carga.Empresa?.GerarCIOTParaTodasCargasMesmoSemVeiculoTerceiro ?? false;

            return gerarCIOTParaTodos;
        }

        public static string ObterCIOTCarga(Dominio.Entidades.Embarcador.Cargas.Carga carga, Dominio.Entidades.Embarcador.Configuracao.ConfiguracaoTMS configuracaoTMS, AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware tipoServicoMultisoftware, Repositorio.UnitOfWork unitOfWork)
        {
            if (carga.TipoOperacao?.NaoGerarContratoFreteTerceiro ?? false)
            {
                Servicos.Log.TratarErro("Carga " + carga.CodigoCargaEmbarcador + " - Tipo de operaçao configurado para não gerar contrato de frete terceiro.", "ObterCIOTCarga");
                return null;
            }

            bool gerarCIOTParaTodos = VerificarSeGeraCIOTParaTodos(carga, configuracaoTMS, unitOfWork);

            if (!gerarCIOTParaTodos && (!carga.FreteDeTerceiro || configuracaoTMS.TipoContratoFreteTerceiro != Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoContratoFreteTerceiro.PorCarga))
            {
                Servicos.Log.TratarErro("Carga " + carga.CodigoCargaEmbarcador + " - Não configurado CIOT para Todos E carga não é de terceiro.", "ObterCIOTCarga");
                return null;
            }

            if (carga.Pedidos?.Any(p => p.Pedido.CIOT != "" && p.Pedido.CIOT != null) ?? false)
            {
                Servicos.Log.TratarErro("Carga " + carga.CodigoCargaEmbarcador + " - Pedidos com número de CIOT.", "ObterCIOTCarga");
                return null;
            }

            if (carga.Pedidos?.Any(o => o.Origem?.Estado?.Sigla == "EX" || o.Destino?.Estado?.Sigla == "EX") ?? false)
            {
                Servicos.Log.TratarErro("Carga " + carga.CodigoCargaEmbarcador + " - Carga com Origem ou Destino Exterior.", "ObterCIOTCarga");
                return null;
            }

            Dominio.Entidades.Cliente terceiro = null;
            Dominio.Entidades.Empresa contratante = null;
            Dominio.Entidades.Usuario motorista = carga.Motoristas.FirstOrDefault();
            Dominio.Entidades.Veiculo veiculo = carga.Veiculo;
            List<Dominio.Entidades.Veiculo> veiculosVinculados = carga.VeiculosVinculados.ToList();

            Repositorio.Embarcador.Pessoas.ModalidadeTransportadoraPessoasTipoPagamentoCIOT repModalidadeTransportadoraPessoasTipoPagamentoCIOT = new Repositorio.Embarcador.Pessoas.ModalidadeTransportadoraPessoasTipoPagamentoCIOT(unitOfWork);
            Repositorio.Embarcador.Cargas.CargaCIOT repCargaCIOT = new Repositorio.Embarcador.Cargas.CargaCIOT(unitOfWork);
            Repositorio.Embarcador.Terceiros.ContratoFrete repContratoFrete = new Repositorio.Embarcador.Terceiros.ContratoFrete(unitOfWork);
            Repositorio.Embarcador.Pessoas.ModalidadeTransportadoraPessoas repModalidadeTerceiro = new Repositorio.Embarcador.Pessoas.ModalidadeTransportadoraPessoas(unitOfWork);
            Repositorio.Embarcador.Configuracoes.ConfiguracaoGeralCarga repConfiguracaoGeralCarga = new Repositorio.Embarcador.Configuracoes.ConfiguracaoGeralCarga(unitOfWork);
            Dominio.Entidades.Embarcador.Configuracoes.ConfiguracaoGeralCarga configuracaoGeralCarga = repConfiguracaoGeralCarga.BuscarPrimeiroRegistro();
            Repositorio.Cliente repCliente = new Repositorio.Cliente(unitOfWork);
            Repositorio.Empresa repEmpresa = new Repositorio.Empresa(unitOfWork);

            Dominio.Entidades.Embarcador.Terceiros.ContratoFrete contratoFrete = repContratoFrete.BuscarPorCarga(carga.Codigo);

            if (configuracaoTMS == null || configuracaoTMS.TipoContratoFreteTerceiro == Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoContratoFreteTerceiro.PorCarga)
                terceiro = contratoFrete?.TransportadorTerceiro ?? carga.Terceiro;

            if (terceiro == null && (carga.Empresa?.GerarCIOTParaTodasCargasMesmoSemVeiculoTerceiro ?? false))
                terceiro = repCliente.BuscarPorCPFCNPJ(carga.Empresa?.CNPJ.ToDouble() ?? 0);

            if (terceiro == null)
                return "O transportador/terceiro não informado.";

            Dominio.Entidades.Embarcador.Pessoas.ModalidadeTransportadoraPessoas modalidadeTerceiro = ObterModalidadeTransportador(terceiro, unitOfWork);

            if (gerarCIOTParaTodos && contratoFrete == null)
                return "Não foi gerado um contrato de frete para o transporte. O contrato de frete é necessário para a geração do CIOT. Recalcule o frete ou configure o cadastro de pessoa do transportador.";

            if (gerarCIOTParaTodos && !(modalidadeTerceiro?.GerarCIOT ?? false) && !(configuracaoGeralCarga?.ExigirConfiguracaoTerceiroParaGerarCIOTParaTodos ?? false))
            {
                Servicos.Log.TratarErro("Carga " + carga.CodigoCargaEmbarcador + " - Transportador/cliente " + terceiro.CPF_CNPJ_SemFormato + " não configurado para gerar CIOT.", "ObterCIOTCarga");
                return null;
            }

            Servicos.Log.TratarErro($"6 - Carga: '{carga.Codigo}' -> Emissao Contingencia: {carga.ContingenciaEmissao}", "EmissaoContingencia");
            if (gerarCIOTParaTodos && (modalidadeTerceiro == null || !modalidadeTerceiro.GerarCIOT) && (gerarCIOTParaTodos || !(carga.Empresa?.EmissaoDocumentosForaDoSistema ?? false) || (carga.TipoOperacao?.EmissaoDocumentosForaDoSistema ?? false) || carga.ContingenciaEmissao))
                return "O transportador não está configurado para gerar CIOT.";

            if (modalidadeTerceiro != null && modalidadeTerceiro.GerarCIOT && contratoFrete != null && (gerarCIOTParaTodos || !(carga.Empresa?.EmissaoDocumentosForaDoSistema ?? false) || (carga.TipoOperacao?.EmissaoDocumentosForaDoSistema ?? false) || carga.ContingenciaEmissao))
            {
                if (carga.Filial?.ConfiguracaoCIOT != null)
                {
                    contratoFrete.ConfiguracaoCIOT = carga.Filial?.ConfiguracaoCIOT;
                    repContratoFrete.Atualizar(contratoFrete);
                }
                else if (contratoFrete.ConfiguracaoCIOT == null || modalidadeTerceiro.ConfiguracaoCIOTWithTipoTerceiro == null)
                {
                    if (modalidadeTerceiro.ConfiguracaoCIOTWithTipoTerceiro == null)
                    {
                        modalidadeTerceiro.ConfiguracaoCIOT = Servicos.Embarcador.CIOT.CIOT.ObterConfiguracaoCIOT(contratoFrete.TransportadorTerceiro, carga.Empresa, unitOfWork, carga);
                        repModalidadeTerceiro.Atualizar(modalidadeTerceiro);
                    }
                    if (contratoFrete.ConfiguracaoCIOT == null && modalidadeTerceiro.ConfiguracaoCIOTWithTipoTerceiro != null)
                    {
                        contratoFrete.ConfiguracaoCIOT = modalidadeTerceiro.ConfiguracaoCIOTWithTipoTerceiro;
                        repContratoFrete.Atualizar(contratoFrete);
                    }
                }

                Dominio.Entidades.Embarcador.CIOT.ConfiguracaoCIOT configuracaoCIOT = contratoFrete.ConfiguracaoCIOT;

                if (configuracaoCIOT == null)
                    return "É necessário criar a configuração para emissão do CIOT.";

                if (configuracaoCIOT.ExigeRotaCadastrada && carga.Rota == null)
                    return "É obrigatório informar uma rota para a carga que possua um código de integração válido com a integradora do CIOT.";

                Repositorio.Embarcador.Documentos.CIOT repCIOT = new Repositorio.Embarcador.Documentos.CIOT(unitOfWork);
                Dominio.Entidades.Embarcador.Documentos.CIOT ciot = null;

                DateTime dataFinalViagem = ObterPrevisaoFinalViagemCarga(carga, unitOfWork);
                DateTime? dataParaFechamento = null;
                bool gerarCIOTPorViagem = true;

                if (tipoServicoMultisoftware == AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiTMS)
                {
                    gerarCIOTPorViagem = modalidadeTerceiro.TipoTransportador != Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoProprietarioVeiculo.TACAgregado;
                    if (modalidadeTerceiro.TipoGeracaoCIOT.HasValue)
                        gerarCIOTPorViagem = modalidadeTerceiro.TipoGeracaoCIOT.Value == TipoGeracaoCIOT.PorViagem;

                    List<Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoCIOT> situacoesCIOTs = new List<Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoCIOT>()
                        {
                            Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoCIOT.Aberto,
                            Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoCIOT.AgIntegracao,
                            Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoCIOT.Pendencia
                        };

                    if (configuracaoCIOT.AbrirCIOTAntesEmissaoCTe)
                    {
                        Dominio.Entidades.Embarcador.Documentos.CIOT ciotAnteriorEmAberto = repCIOT.Buscar(terceiro.CPF_CNPJ, veiculo.Codigo, situacoesCIOTs, carga.Codigo);

                        if (ciotAnteriorEmAberto == null || (ciotAnteriorEmAberto.Situacao != Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoCIOT.Aberto && ciot.Situacao != SituacaoCIOT.AgLiberarViagem))
                            return "Para a atual configuração do CIOT é necessário criar um CIOT antes da emissão dos CT-es.";

                        if (ciotAnteriorEmAberto.DataFinalViagem < DateTime.Now)
                            return $"A data de término do CIOT ({ciotAnteriorEmAberto.DataFinalViagem.ToString("dd/MM/yyyy")}) é menor que a data atual, sendo necessário encerrar o CIOT e abrir um novo.";

                        if (ciotAnteriorEmAberto.DataVencimentoAdiantamento.HasValue && ciotAnteriorEmAberto.DataVencimentoAdiantamento.Value < DateTime.Now)
                            return $"A data de vencimento do adiantamento do CIOT ({ciotAnteriorEmAberto.DataVencimentoAdiantamento.Value.ToString("dd/MM/yyyy")}) é menor que a data atual, sendo necessário encerrar o CIOT e abrir um novo.";

                        if (ciotAnteriorEmAberto.DataVencimentoAbastecimento.HasValue && ciotAnteriorEmAberto.DataVencimentoAbastecimento.Value < DateTime.Now)
                            return $"A data de vencimento do abastecimento do CIOT ({ciotAnteriorEmAberto.DataVencimentoAbastecimento.Value.ToString("dd/MM/yyyy")}) é menor que a data atual, sendo necessário encerrar o CIOT e abrir um novo.";
                    }

                    if (!gerarCIOTPorViagem)
                    {
                        dataParaFechamento = ObterDataParaFechamento(modalidadeTerceiro, unitOfWork);
                        ciot = repCIOT.Buscar(terceiro.CPF_CNPJ, veiculo.Codigo, situacoesCIOTs, carga.Codigo, true);

                        if (ciot != null && (ciot.Situacao == SituacaoCIOT.AgIntegracao || ciot.Situacao == SituacaoCIOT.Pendencia))
                        {
                            Dominio.Entidades.Embarcador.Cargas.CargaCIOT _cargaCIOT = repCargaCIOT.BuscarPrimeiroPorCIOT(ciot.Codigo);

                            string mensagemErro = "Existe CIOT por período pendente de integração é necessário processar o mesmo ou cancelar";
                            if (_cargaCIOT != null)
                                return $"{mensagemErro}, CIOT vinculado à carga " + _cargaCIOT.Carga.CodigoCargaEmbarcador + ".";
                            else
                                return $"{mensagemErro}, não esta vinculado à nenhuma carga.";
                        }
                    }
                }
                else
                {
                    bool permiteVariosCIOTsAbertos = configuracaoCIOT.PermiteVariosCIOTsAbertos && (!configuracaoCIOT.PermiteVariosCIOTsAbertosTipoTerceiro.HasValue || configuracaoCIOT.PermiteVariosCIOTsAbertosTipoTerceiro == modalidadeTerceiro.TipoTransportador);
                    bool buscarCIOTApenasPorCarga = permiteVariosCIOTsAbertos || gerarCIOTParaTodos || (configuracaoCIOT.OperadoraCIOT == OperadoraCIOT.Pagbem && configuracaoCIOT.GerarUmCIOTPorViagem);

                    if (buscarCIOTApenasPorCarga)
                        ciot = repCIOT.BuscarPorCarga(carga.Codigo);
                    else
                    {
                        List<Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoCIOT> situacoesCIOTs = new List<Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoCIOT>()
                        {
                            Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoCIOT.Aberto,
                            Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoCIOT.AgIntegracao,
                            Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoCIOT.Pendencia
                        };

                        ciot = repCIOT.Buscar(terceiro.CPF_CNPJ, motorista.Codigo, veiculo.Codigo, situacoesCIOTs, carga.Codigo);
                    }

                    bool configGerarPorViagem = configuracaoCIOT.GerarUmCIOTPorViagem;
                    if (modalidadeTerceiro.TipoGeracaoCIOT.HasValue)
                        configGerarPorViagem = modalidadeTerceiro.TipoGeracaoCIOT.Value == TipoGeracaoCIOT.PorViagem;

                    gerarCIOTPorViagem = (configGerarPorViagem || modalidadeTerceiro.TipoTransportador != Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoProprietarioVeiculo.TACAgregado);

                    if (gerarCIOTPorViagem && ciot != null && !buscarCIOTApenasPorCarga)
                    {
                        Dominio.Entidades.Embarcador.Cargas.CargaCIOT _cargaCIOT = repCargaCIOT.BuscarPrimeiroPorCIOT(ciot.Codigo);

                        if (_cargaCIOT != null)
                            return "Antes de autorizar a emissão é necessário encerrar ou cancelar o CIOT vinculado à carga " + _cargaCIOT.Carga.CodigoCargaEmbarcador + ".";
                        else
                            return "Existe um CIOT pendente de encerramento/cancelamento que não esta vinculado à nenhuma carga.";
                    }

                    if (configuracaoCIOT.AbrirCIOTAntesEmissaoCTe)
                    {
                        if (ciot == null || (ciot.Situacao != Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoCIOT.Aberto && ciot.Situacao != SituacaoCIOT.AgLiberarViagem))
                            return "Para a atual configuração do CIOT é necessário criar um CIOT antes da emissão dos CT-es.";

                        if (ciot.DataFinalViagem < DateTime.Now)
                            return $"A data de término do CIOT ({ciot.DataFinalViagem.ToString("dd/MM/yyyy")}) é menor que a data atual, sendo necessário encerrar o CIOT e abrir um novo.";

                        if (ciot.DataVencimentoAdiantamento.HasValue && ciot.DataVencimentoAdiantamento.Value < DateTime.Now)
                            return $"A data de vencimento do adiantamento do CIOT ({ciot.DataVencimentoAdiantamento.Value.ToString("dd/MM/yyyy")}) é menor que a data atual, sendo necessário encerrar o CIOT e abrir um novo.";

                        if (ciot.DataVencimentoAbastecimento.HasValue && ciot.DataVencimentoAbastecimento.Value < DateTime.Now)
                            return $"A data de vencimento do abastecimento do CIOT ({ciot.DataVencimentoAbastecimento.Value.ToString("dd/MM/yyyy")}) é menor que a data atual, sendo necessário encerrar o CIOT e abrir um novo.";
                    }
                }

                Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoPagamentoCIOT? tipoPagamentoCIOT = repModalidadeTransportadoraPessoasTipoPagamentoCIOT.BuscarTipoPagamentoPorOperadora(modalidadeTerceiro.Codigo, configuracaoCIOT.OperadoraCIOT);

                bool cargaAberturaCIOT = false;
                if (ciot == null)
                {
                    cargaAberturaCIOT = true;

                    if (carga.Empresa?.GerarCIOTParaTodasCargasMesmoSemVeiculoTerceiro ?? false)
                        contratante = repEmpresa.BuscarPorCNPJ(carga.Filial?.CNPJ);

                    ciot = new Dominio.Entidades.Embarcador.Documentos.CIOT
                    {
                        DataFinalViagem = dataFinalViagem,
                        DataAbertura = DateTime.Now,
                        Transportador = terceiro,
                        Contratante = contratante ?? carga.Empresa,
                        Operadora = configuracaoCIOT.OperadoraCIOT,
                        ConfiguracaoCIOT = configuracaoCIOT,
                        Situacao = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoCIOT.AgIntegracao,
                        Veiculo = veiculo,
                        VeiculosVinculados = veiculosVinculados,
                        Motorista = motorista,
                        CIOTPorPeriodo = !gerarCIOTPorViagem,
                        TipoPagamentoCIOT = tipoPagamentoCIOT,
                        DataParaFechamento = dataParaFechamento
                    };

                    repCIOT.Inserir(ciot);

                    Servicos.Auditoria.Auditoria.Auditar(new Dominio.ObjetosDeValor.Embarcador.Auditoria.Auditado() { OrigemAuditado = Dominio.ObjetosDeValor.Enumerador.OrigemAuditado.Sistema, TipoAuditado = Dominio.ObjetosDeValor.Enumerador.TipoAuditado.Sistema }, ciot, $"Gerou o CIOT à partir da geração dos documentos da carga {carga.CodigoCargaEmbarcador}.", unitOfWork);
                }
                else
                {
                    AtualizarDadosCIOT(ciot, carga, terceiro, unitOfWork);
                }

                ConfigurarCIOT(ciot, carga, unitOfWork);
                CriarCargaCIOT(carga, ciot, contratoFrete, unitOfWork, cargaAberturaCIOT);
            }
            else
            {
                if (contratoFrete?.ConfiguracaoCIOT != null)
                {
                    contratoFrete.ConfiguracaoCIOT = null;
                    repContratoFrete.Atualizar(contratoFrete);
                }
            }

            return null;
        }

        public bool GerarCIOTAutomatico(Dominio.Entidades.Embarcador.Cargas.Carga carga, FormasPagamento? formaPagamentoCIOT, decimal valorFrete, decimal valorAdiantamento, DateTime? dataVencimento, TipoPagamentoMDFe? tipoPagamentoMDFe, string cnpjInstituicaoPagamento, string agencia, string contaCIOT, string chavePIX, string numeroCIOT, string numeroCiotAntigo, TipoServicoMultisoftware tipoServicoMultisoftware, Repositorio.UnitOfWork unitOfWork, Dominio.ObjetosDeValor.Enumerador.TipoChavePix TipoChavePIX = Dominio.ObjetosDeValor.Enumerador.TipoChavePix.Aleatoria)
        {
            Repositorio.Cliente repositorioCliente = new Repositorio.Cliente(unitOfWork);
            Repositorio.Embarcador.Cargas.Carga repositorioCarga = new Repositorio.Embarcador.Cargas.Carga(unitOfWork);
            Repositorio.Embarcador.Cargas.CargaInformacoesBancarias repositorioCargaInformacoesBancarias = new Repositorio.Embarcador.Cargas.CargaInformacoesBancarias(unitOfWork);
            Repositorio.Embarcador.Documentos.CIOT repositorioCIOT = new Repositorio.Embarcador.Documentos.CIOT(unitOfWork);

            try
            {
                unitOfWork.Start();

                Dominio.Entidades.Cliente terceiro = carga.Terceiro;

                if (terceiro == null)
                    terceiro = repositorioCliente.BuscarPorCPFCNPJ(carga.Empresa?.CNPJ.ToDouble() ?? 0);

                Dominio.Entidades.Embarcador.Documentos.CIOT ciot = repositorioCIOT.BuscarPorCargaCIOT(carga.Codigo, numeroCiotAntigo);
                ciot ??= new Dominio.Entidades.Embarcador.Documentos.CIOT();

                ciot.DataFinalViagem = dataVencimento ?? DateTime.Now;
                ciot.DataParaFechamento = dataVencimento;
                ciot.Transportador = terceiro;
                ciot.Contratante = carga.Empresa;
                ciot.Situacao = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoCIOT.Aberto;
                ciot.DataAbertura = DateTime.Now.Date;
                ciot.Motorista = carga.Motoristas.FirstOrDefault();
                ciot.Veiculo = carga.Veiculo;
                ciot.CIOTPorPeriodo = true;
                ciot.Numero = numeroCIOT;
                ciot.TipoPagamentoCIOT = tipoPagamentoMDFe == TipoPagamentoMDFe.PIX ? TipoPagamentoCIOT.PIX : TipoPagamentoCIOT.Transferencia;
                ciot.CIOTGeradoAutomaticamente = true;

                if (ciot.Codigo <= 0)
                    repositorioCIOT.Inserir(ciot);
                else
                    repositorioCIOT.Atualizar(ciot);

                Dominio.Entidades.Global.CargaInformacoesBancarias cargaInformacoesBancarias = repositorioCargaInformacoesBancarias.BuscarPorCarga(carga.Codigo);
                cargaInformacoesBancarias ??= new Dominio.Entidades.Global.CargaInformacoesBancarias();

                cargaInformacoesBancarias.Agencia = tipoPagamentoMDFe == TipoPagamentoMDFe.Banco ? agencia : null;
                cargaInformacoesBancarias.Conta = tipoPagamentoMDFe == TipoPagamentoMDFe.Banco ? contaCIOT : null;
                cargaInformacoesBancarias.ChavePIX = tipoPagamentoMDFe == TipoPagamentoMDFe.PIX ? chavePIX : null;
                cargaInformacoesBancarias.TipoChavePIX = TipoChavePIX;
                cargaInformacoesBancarias.Carga = carga;
                cargaInformacoesBancarias.Ipef = cnpjInstituicaoPagamento;
                cargaInformacoesBancarias.TipoInformacaoBancaria = tipoPagamentoMDFe;
                cargaInformacoesBancarias.TipoPagamento = formaPagamentoCIOT;
                cargaInformacoesBancarias.ValorAdiantamento = valorAdiantamento;

                if (cargaInformacoesBancarias.Codigo <= 0)
                {
                    cargaInformacoesBancarias.RegistradoPeloEmbarcador = tipoServicoMultisoftware == TipoServicoMultisoftware.MultiEmbarcador;
                    repositorioCargaInformacoesBancarias.Inserir(cargaInformacoesBancarias);
                }
                else
                    repositorioCargaInformacoesBancarias.Atualizar(cargaInformacoesBancarias);

                CriarCargaCIOTAutomaticamente(carga, ciot, valorAdiantamento, valorFrete, unitOfWork);

                carga.PossuiPendencia = false;
                carga.ProblemaIntegracaoCIOT = false;
                carga.MotivoPendencia = "";

                repositorioCarga.Atualizar(carga);
                unitOfWork.CommitChanges();
                return true;
            }
            catch (Exception ex)
            {
                unitOfWork.Rollback();
                Servicos.Log.TratarErro(ex, "Erro ao gerar ciot, na rotina de geração automatica.");
                return false;
            }
        }

        public byte[] GerarContratoFrete(int codigoCIOT, Repositorio.UnitOfWork unidadeTrabalho, out string mensagemErro)
        {
            var result = ReportRequest.WithType(ReportType.CIOT)
                .WithExecutionType(ExecutionType.Sync)
                .AddExtraData("CodigoCiot", codigoCIOT.ToString())
                .CallReport();

            mensagemErro = result.ErrorMessage;

            return result.GetContentFile();
        }

        public Dominio.Entidades.Embarcador.Documentos.CIOT AbrirCIOT(Dominio.Entidades.Embarcador.CIOT.ConfiguracaoCIOT configuracaoCIOT, AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware tipoServicoMultisoftware, Dominio.Entidades.Cliente terceiro, Dominio.Entidades.Usuario motorista, Dominio.Entidades.Veiculo veiculo, DateTime dataFinalizacao, Dominio.Entidades.Empresa empresa, Repositorio.UnitOfWork unidadeTrabalho, out string mensagemErro)
        {
            Repositorio.Embarcador.Documentos.CIOT repCIOT = new Repositorio.Embarcador.Documentos.CIOT(unidadeTrabalho);
            List<Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoCIOT> situacoesCIOTValidar = new List<SituacaoCIOT>() { SituacaoCIOT.Aberto, SituacaoCIOT.AgIntegracao, SituacaoCIOT.Pendencia };

            Dominio.Entidades.Embarcador.Documentos.CIOT ciotExistente = null;
            if (tipoServicoMultisoftware == AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiTMS)
                ciotExistente = repCIOT.Buscar(terceiro.CPF_CNPJ, veiculo.Codigo, situacoesCIOTValidar, 0);
            else
                ciotExistente = repCIOT.Buscar(terceiro.CPF_CNPJ, motorista.Codigo, veiculo.Codigo, situacoesCIOTValidar, 0);

            Dominio.Entidades.Embarcador.Pessoas.ModalidadeTransportadoraPessoas modalidadeTerceiro = ObterModalidadeTransportador(terceiro, unidadeTrabalho);

            Repositorio.Embarcador.Pessoas.ModalidadeTransportadoraPessoasTipoPagamentoCIOT repModalidadeTransportadoraPessoasTipoPagamentoCIOT = new Repositorio.Embarcador.Pessoas.ModalidadeTransportadoraPessoasTipoPagamentoCIOT(unidadeTrabalho);
            Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoPagamentoCIOT? tipoPagamentoCIOT = repModalidadeTransportadoraPessoasTipoPagamentoCIOT.BuscarTipoPagamentoPorOperadora(modalidadeTerceiro.Codigo, configuracaoCIOT.OperadoraCIOT);

            mensagemErro = "";

            if (configuracaoCIOT.OperadoraCIOT == OperadoraCIOT.RepomFrete)
            {
                mensagemErro = $"Abertura de CIOT por período sem carga não está disponível para a operadora {configuracaoCIOT.OperadoraCIOT.ObterDescricao()}.";
                return null;
            }

            if (ciotExistente != null)
            {
                if (tipoServicoMultisoftware == AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiTMS)
                    mensagemErro = "Já existe um CIOT aberto para este terceiro e veículo.";
                else
                    mensagemErro = "Já existe um CIOT aberto para este terceiro, veículo e motorista.";

                return null;
            }

            Dominio.Entidades.Embarcador.Documentos.CIOT ciot = new Dominio.Entidades.Embarcador.Documentos.CIOT
            {
                DataFinalViagem = dataFinalizacao,
                DataParaFechamento = dataFinalizacao,
                Transportador = terceiro,
                ConfiguracaoCIOT = configuracaoCIOT,
                Contratante = empresa,
                Operadora = configuracaoCIOT.OperadoraCIOT,
                Situacao = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoCIOT.Pendencia,
                DataAbertura = DateTime.Now.Date,
                Motorista = motorista,
                Veiculo = veiculo,
                VeiculosVinculados = veiculo.VeiculosVinculados.ToList(),
                CIOTPorPeriodo = true,
                TipoPagamentoCIOT = tipoPagamentoCIOT
            };

            if ((ciot.VeiculosVinculados == null || ciot.VeiculosVinculados.Count() == 0) && ciot.CargaCIOT != null && ciot.CargaCIOT.Count > 0)
                ciot.VeiculosVinculados = ciot.CargaCIOT.First().Carga.VeiculosVinculados.ToList();

            SituacaoRetornoCIOT retAbrirCIOT = AbrirCIOT(ciot, null, tipoServicoMultisoftware, unidadeTrabalho);

            if (retAbrirCIOT == SituacaoRetornoCIOT.Autorizado)
            {
                return ciot;
            }
            else
            {
                mensagemErro = ciot.Mensagem;
                return null;
            }
        }

        public SituacaoRetornoCIOT AbrirCIOT(Dominio.Entidades.Embarcador.Documentos.CIOT ciot, Dominio.Entidades.Embarcador.Cargas.CargaCIOT cargaCIOT, AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware tipoServicoMultisoftware, Repositorio.UnitOfWork unidadeTrabalho)
        {
            Repositorio.Embarcador.Documentos.CIOT repCIOT = new Repositorio.Embarcador.Documentos.CIOT(unidadeTrabalho);
            Repositorio.Embarcador.Cargas.CargaCIOT repCargaCIOT = new Repositorio.Embarcador.Cargas.CargaCIOT(unidadeTrabalho);
            Repositorio.Embarcador.Cargas.TipoIntegracao repTipoIntegracao = new Repositorio.Embarcador.Cargas.TipoIntegracao(unidadeTrabalho);

            SituacaoRetornoCIOT retAbrirCIOT = SituacaoRetornoCIOT.ProblemaIntegracao;

            switch (ciot.Operadora)
            {
                case Dominio.ObjetosDeValor.Embarcador.Enumeradores.OperadoraCIOT.eFrete:
                    Servicos.Embarcador.CIOT.EFrete serEFrete = new EFrete();
                    retAbrirCIOT = serEFrete.AbrirCIOT(ciot, unidadeTrabalho, tipoServicoMultisoftware);
                    break;
                case Dominio.ObjetosDeValor.Embarcador.Enumeradores.OperadoraCIOT.Repom:
                    Servicos.Embarcador.CIOT.Repom svcRepom = new Repom();
                    retAbrirCIOT = svcRepom.IntegrarCIOT(ciot, unidadeTrabalho);
                    break;
                case Dominio.ObjetosDeValor.Embarcador.Enumeradores.OperadoraCIOT.Pamcard:
                    Servicos.Embarcador.CIOT.Pamcard svcPamcard = new Pamcard();
                    retAbrirCIOT = svcPamcard.AbrirCIOT(ciot, unidadeTrabalho);
                    break;
                case Dominio.ObjetosDeValor.Embarcador.Enumeradores.OperadoraCIOT.Pagbem:
                    Servicos.Embarcador.CIOT.Pagbem svcPagbem = new Pagbem();
                    retAbrirCIOT = svcPagbem.IntegrarCIOT(ciot, unidadeTrabalho);
                    break;
                case Dominio.ObjetosDeValor.Embarcador.Enumeradores.OperadoraCIOT.Target:
                    Servicos.Embarcador.CIOT.Target svcTarget = new Target();
                    retAbrirCIOT = svcTarget.IntegrarCIOT(ciot, unidadeTrabalho);
                    break;
                case Dominio.ObjetosDeValor.Embarcador.Enumeradores.OperadoraCIOT.Extratta:
                    Servicos.Embarcador.CIOT.Extratta svcExtratta = new Extratta();
                    retAbrirCIOT = svcExtratta.IntegrarCIOT(ciot, unidadeTrabalho);
                    break;
                case Dominio.ObjetosDeValor.Embarcador.Enumeradores.OperadoraCIOT.BBC:
                    retAbrirCIOT = new Servicos.Embarcador.CIOT.BBC(unidadeTrabalho).IntegrarCIOT(ciot);
                    break;
                case Dominio.ObjetosDeValor.Embarcador.Enumeradores.OperadoraCIOT.Ambipar:
                    retAbrirCIOT = new Servicos.Embarcador.CIOT.Ambipar(unidadeTrabalho).IntegrarCIOT(ciot);
                    break;
                case Dominio.ObjetosDeValor.Embarcador.Enumeradores.OperadoraCIOT.Rodocred:
                    retAbrirCIOT = new Servicos.Embarcador.CIOT.Rodocred(unidadeTrabalho, ciot).IntegrarCIOT();
                    break;
                case Dominio.ObjetosDeValor.Embarcador.Enumeradores.OperadoraCIOT.RepomFrete:
                    retAbrirCIOT = new Servicos.Embarcador.CIOT.RepomFrete.IntegracaoRepomFrete().IntegrarCIOT(ciot, tipoServicoMultisoftware, unidadeTrabalho);
                    break;
                case Dominio.ObjetosDeValor.Embarcador.Enumeradores.OperadoraCIOT.TruckPad:
                    retAbrirCIOT = new Servicos.Embarcador.CIOT.TruckPad.IntegracaoTruckPad().IntegrarCIOT(ciot, tipoServicoMultisoftware, unidadeTrabalho);
                    break;
                default:
                    break;
            }

            if (retAbrirCIOT == SituacaoRetornoCIOT.Autorizado)
            {
                Dominio.Entidades.Embarcador.Pessoas.ModalidadeTransportadoraPessoas modalidadeTerceiro = Servicos.Embarcador.CIOT.CIOT.ObterModalidadeTransportador(ciot.Transportador, unidadeTrabalho);

                if (tipoServicoMultisoftware == AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiTMS)
                {
                    if (cargaCIOT != null)
                        Servicos.Embarcador.CIOT.CIOT.ConfirmarIntegracaoCargasAoCIOT(cargaCIOT, tipoServicoMultisoftware, unidadeTrabalho);
                }
                else
                {
                    List<Dominio.Entidades.Embarcador.Cargas.CargaCIOT> cargaCIOTs = repCargaCIOT.BuscarPorCIOT(ciot.Codigo);
                    foreach (Dominio.Entidades.Embarcador.Cargas.CargaCIOT cargaCIOTAtualizar in cargaCIOTs)
                        Servicos.Embarcador.CIOT.CIOT.ConfirmarIntegracaoCargasAoCIOT(cargaCIOTAtualizar, tipoServicoMultisoftware, unidadeTrabalho);
                }

                if (repTipoIntegracao.ExistePorTipo(TipoIntegracao.SAP_AV))
                    GerarIntegracaoSAP_AV(ciot, unidadeTrabalho);
            }

            return retAbrirCIOT;
        }

        public bool EncerrarCIOT(Dominio.Entidades.Embarcador.Documentos.CIOT ciot, Repositorio.UnitOfWork unidadeTrabalho, AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware tipoServicoMultisoftware, out string mensagemErro)
        {
            bool sucesso = false;
            mensagemErro = "";

            switch (ciot.Operadora)
            {
                case Dominio.ObjetosDeValor.Embarcador.Enumeradores.OperadoraCIOT.eFrete:
                    Servicos.Embarcador.CIOT.EFrete serEFrete = new EFrete();
                    sucesso = serEFrete.EncerrarCIOT(ciot, unidadeTrabalho, out mensagemErro);
                    break;
                case Dominio.ObjetosDeValor.Embarcador.Enumeradores.OperadoraCIOT.Repom:
                    Servicos.Embarcador.CIOT.Repom svcRepom = new Repom();
                    sucesso = svcRepom.EncerrarCIOT(ciot, unidadeTrabalho, out mensagemErro);
                    break;
                case Dominio.ObjetosDeValor.Embarcador.Enumeradores.OperadoraCIOT.Pamcard:
                    Servicos.Embarcador.CIOT.Pamcard svcPamcard = new Pamcard();
                    sucesso = svcPamcard.EncerrarCIOT(ciot, unidadeTrabalho, out mensagemErro);
                    break;
                case Dominio.ObjetosDeValor.Embarcador.Enumeradores.OperadoraCIOT.Pagbem:
                    Servicos.Embarcador.CIOT.Pagbem svcPagbem = new Pagbem();
                    sucesso = svcPagbem.EncerrarCIOT(ciot, unidadeTrabalho, out mensagemErro);
                    break;
                case OperadoraCIOT.Target:
                    Servicos.Embarcador.CIOT.Target svcTarget = new Target();
                    sucesso = svcTarget.EncerrarCIOT(ciot, unidadeTrabalho, out mensagemErro);
                    break;
                case Dominio.ObjetosDeValor.Embarcador.Enumeradores.OperadoraCIOT.Extratta:
                    Servicos.Embarcador.CIOT.Extratta svcExtratta = new Extratta();
                    sucesso = svcExtratta.EncerrarCIOT(ciot, unidadeTrabalho, out mensagemErro);
                    break;
                case OperadoraCIOT.BBC:
                    sucesso = new Servicos.Embarcador.CIOT.BBC(unidadeTrabalho).EncerrarCIOT(ciot, out mensagemErro);
                    break;
                case OperadoraCIOT.Ambipar:
                    sucesso = new Servicos.Embarcador.CIOT.Ambipar(unidadeTrabalho).EncerrarCIOT(ciot, out mensagemErro);
                    break;
                case OperadoraCIOT.Rodocred:
                    sucesso = new Servicos.Embarcador.CIOT.Rodocred(unidadeTrabalho, ciot).EncerrarCIOT(out mensagemErro);
                    break;
                case OperadoraCIOT.RepomFrete:
                    sucesso = new Servicos.Embarcador.CIOT.RepomFrete.IntegracaoRepomFrete().EncerrarCIOT(ciot, out mensagemErro, unidadeTrabalho);
                    break;
                case OperadoraCIOT.TruckPad:
                    sucesso = new Servicos.Embarcador.CIOT.TruckPad.IntegracaoTruckPad().EncerrarCIOT(ciot, out mensagemErro, unidadeTrabalho);
                    break;
                default:
                    mensagemErro = "Encerramento não implementado para a operadora.";
                    break;
            }

            if (sucesso)
            {
                Servicos.Embarcador.Terceiros.ContratoFrete serContratoFrete = new Terceiros.ContratoFrete(unidadeTrabalho);
                serContratoFrete.EncerrarContratosPorCIOT(ciot, tipoServicoMultisoftware, unidadeTrabalho, DateTime.Now);
            }

            return sucesso;
        }

        public bool EncerrarCIOTGerencialmente(out string mensagemErro, Dominio.Entidades.Embarcador.Documentos.CIOT ciot, Dominio.ObjetosDeValor.Embarcador.Auditoria.Auditado auditado, Repositorio.UnitOfWork unitOfWork, AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware tipoServicoMultisoftware)
        {
            if (ciot.Situacao != SituacaoCIOT.Aberto)
            {
                mensagemErro = $"A situação do CIOT ({ciot.Situacao.ObterDescricao()}) não permite que o mesmo seja encerrado.";
                return false;
            }

            Repositorio.Embarcador.Documentos.CIOT repCIOT = new Repositorio.Embarcador.Documentos.CIOT(unitOfWork);

            ciot.Situacao = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoCIOT.Encerrado;
            ciot.DataEncerramento = DateTime.Now;
            ciot.Mensagem = "Encerrado gerencialmente.";

            repCIOT.Atualizar(ciot);

            Servicos.Embarcador.Terceiros.ContratoFrete serContratoFrete = new Terceiros.ContratoFrete(unitOfWork);
            serContratoFrete.EncerrarContratosPorCIOT(ciot, tipoServicoMultisoftware, unitOfWork, DateTime.Now);

            Servicos.Auditoria.Auditoria.Auditar(auditado, ciot, null, "Encerrou o CIOT gerencialmente.", unitOfWork);

            mensagemErro = null;
            return true;
        }

        public bool CancelarCIOT(Dominio.Entidades.Embarcador.Documentos.CIOT ciot, Dominio.Entidades.Usuario usuario, AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware tipoServicoMultisoftware, Repositorio.UnitOfWork unidadeTrabalho, out string mensagemErro)
        {
            bool sucesso = false;
            mensagemErro = "";

            switch (ciot.Operadora)
            {
                case Dominio.ObjetosDeValor.Embarcador.Enumeradores.OperadoraCIOT.eFrete:
                    Servicos.Embarcador.CIOT.EFrete serEFrete = new EFrete();
                    sucesso = serEFrete.CancelarCIOT(ciot, tipoServicoMultisoftware, unidadeTrabalho, out mensagemErro);
                    break;
                case Dominio.ObjetosDeValor.Embarcador.Enumeradores.OperadoraCIOT.Repom:
                    Servicos.Embarcador.CIOT.Repom svcRepom = new Repom();
                    sucesso = svcRepom.CancelarCIOT(ciot, usuario, unidadeTrabalho, out mensagemErro);
                    break;
                case Dominio.ObjetosDeValor.Embarcador.Enumeradores.OperadoraCIOT.Pamcard:
                    Servicos.Embarcador.CIOT.Pamcard svcPamcard = new Pamcard();
                    sucesso = svcPamcard.CancelarCIOT(ciot, unidadeTrabalho, out mensagemErro);
                    break;
                case Dominio.ObjetosDeValor.Embarcador.Enumeradores.OperadoraCIOT.Pagbem:
                    Servicos.Embarcador.CIOT.Pagbem svcPagBem = new Pagbem();
                    sucesso = svcPagBem.CancelarCIOT(ciot, unidadeTrabalho, out mensagemErro);
                    break;
                case OperadoraCIOT.Target:
                    Servicos.Embarcador.CIOT.Target svcTarget = new Target();
                    sucesso = svcTarget.CancelarCIOT(ciot, unidadeTrabalho, out mensagemErro);
                    break;
                case Dominio.ObjetosDeValor.Embarcador.Enumeradores.OperadoraCIOT.Extratta:
                    Servicos.Embarcador.CIOT.Extratta svcExtratta = new Extratta();
                    sucesso = svcExtratta.CancelarCIOT(ciot, unidadeTrabalho, out mensagemErro);
                    break;
                case OperadoraCIOT.BBC:
                    sucesso = new Servicos.Embarcador.CIOT.BBC(unidadeTrabalho).CancelarCIOT(ciot, out mensagemErro);
                    break;
                case OperadoraCIOT.Ambipar:
                    sucesso = new Servicos.Embarcador.CIOT.Ambipar(unidadeTrabalho).CancelarCIOT(ciot, out mensagemErro);
                    break;
                case OperadoraCIOT.Rodocred:
                    sucesso = new Servicos.Embarcador.CIOT.Rodocred(unidadeTrabalho, ciot).CancelarCIOT(out mensagemErro);
                    break;
                case OperadoraCIOT.RepomFrete:
                    sucesso = new Servicos.Embarcador.CIOT.RepomFrete.IntegracaoRepomFrete().CancelarCIOT(ciot, unidadeTrabalho, out mensagemErro);
                    break;
                case OperadoraCIOT.TruckPad:
                    sucesso = new Servicos.Embarcador.CIOT.TruckPad.IntegracaoTruckPad().CancelarCIOT(ciot, unidadeTrabalho, out mensagemErro);
                    break;
                default:
                    mensagemErro = "Cancelamento de CIOT não implementado para a operadora.";
                    break;
            }

            if (sucesso)
            {
                unidadeTrabalho.Start();

                Servicos.Embarcador.Terceiros.ContratoFrete serContratoFrete = new Terceiros.ContratoFrete(unidadeTrabalho);
                try
                {
                    serContratoFrete.CancelarContratosPorCIOT(ciot, tipoServicoMultisoftware, unidadeTrabalho);
                }
                catch (Exception excecao)
                {
                    mensagemErro = excecao.Message;
                    sucesso = false;
                }
                unidadeTrabalho.CommitChanges();
            }

            return sucesso;
        }


        public bool CancelarCIOTEmSituacaoDePendencia(Dominio.Entidades.Embarcador.Documentos.CIOT ciot, AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware tipoServicoMultisoftware, Repositorio.UnitOfWork unidadeTrabalho, out string mensagemErro)
        {
            bool sucesso = true;
            mensagemErro = "";

            if (ciot.Situacao == SituacaoCIOT.Pendencia)
            {
                unidadeTrabalho.Start();

                Servicos.Embarcador.Terceiros.ContratoFrete serContratoFrete = new Terceiros.ContratoFrete(unidadeTrabalho);
                try
                {
                    serContratoFrete.CancelarContratosPorCIOT(ciot, tipoServicoMultisoftware, unidadeTrabalho);
                }
                catch (Exception excecao)
                {
                    mensagemErro = excecao.Message;
                    sucesso = false;
                }
                unidadeTrabalho.CommitChanges();
            }
            else
            {
                mensagemErro = "A situação do CIOT é diferente de Pendencia.";
                return false;
            }
            return sucesso;
        }



        public bool InterromperCIOT(Dominio.Entidades.Embarcador.Documentos.CIOT ciot, Dominio.Entidades.Usuario usuario, AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware tipoServicoMultisoftware, Repositorio.UnitOfWork unidadeTrabalho, out string mensagemErro)
        {
            bool sucesso = false;
            mensagemErro = "";

            switch (ciot.Operadora)
            {
                case Dominio.ObjetosDeValor.Embarcador.Enumeradores.OperadoraCIOT.Repom:
                    Servicos.Embarcador.CIOT.Repom svcRepom = new Repom();
                    sucesso = svcRepom.InterromperCIOT(ciot, unidadeTrabalho, out mensagemErro);
                    break;
                case Dominio.ObjetosDeValor.Embarcador.Enumeradores.OperadoraCIOT.RepomFrete:
                    sucesso = new Servicos.Embarcador.CIOT.RepomFrete.IntegracaoRepomFrete().InterromperCIOT(ciot, unidadeTrabalho, out mensagemErro);
                    break;
                default:
                    mensagemErro = "Interrupção de CIOT não implementada para a operadora.";
                    break;
            }

            if (sucesso)
            {
                unidadeTrabalho.Start();

                Servicos.Embarcador.Terceiros.ContratoFrete serContratoFrete = new Terceiros.ContratoFrete(unidadeTrabalho);
                serContratoFrete.CancelarContratosPorCIOT(ciot, tipoServicoMultisoftware, unidadeTrabalho);

                unidadeTrabalho.CommitChanges();
            }

            return sucesso;
        }

        public SituacaoRetornoCIOT AdicionarViagem(Dominio.Entidades.Embarcador.Cargas.CargaCIOT cargaCIOT, AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware tipoServicoMultisoftware, Repositorio.UnitOfWork unidadeTrabalho, out string mensagemErro)
        {
            SituacaoRetornoCIOT retAdicionarViagem = SituacaoRetornoCIOT.ProblemaIntegracao;
            mensagemErro = "";

            switch (cargaCIOT.CIOT.Operadora)
            {
                case Dominio.ObjetosDeValor.Embarcador.Enumeradores.OperadoraCIOT.eFrete:
                    Servicos.Embarcador.CIOT.EFrete serEFrete = new EFrete();
                    retAdicionarViagem = serEFrete.AdicionarViagem(cargaCIOT, unidadeTrabalho, out mensagemErro, tipoServicoMultisoftware);
                    break;
                case Dominio.ObjetosDeValor.Embarcador.Enumeradores.OperadoraCIOT.Repom:
                    mensagemErro = "Repom não possui adição de viagem ao CIOT.";
                    break;
                case Dominio.ObjetosDeValor.Embarcador.Enumeradores.OperadoraCIOT.RepomFrete:
                    retAdicionarViagem = new Servicos.Embarcador.CIOT.RepomFrete.IntegracaoRepomFrete().IntegrarAdicionarViagemContratoFrete(cargaCIOT, out mensagemErro, tipoServicoMultisoftware, unidadeTrabalho);
                    break;
                case Dominio.ObjetosDeValor.Embarcador.Enumeradores.OperadoraCIOT.Pamcard:
                    Servicos.Embarcador.CIOT.Pamcard svcPamcard = new Pamcard();
                    retAdicionarViagem = svcPamcard.AdicionarViagem(cargaCIOT, unidadeTrabalho, out mensagemErro);
                    break;
                default:
                    mensagemErro = "Inclusão de viagem ao CIOT não implementada para a operadora.";
                    break;
            }

            if (retAdicionarViagem == SituacaoRetornoCIOT.Autorizado)
            {
                unidadeTrabalho.Start();

                if (tipoServicoMultisoftware == AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiTMS)
                    Servicos.Embarcador.CIOT.CIOT.ConfirmarIntegracaoCargasAoCIOT(cargaCIOT, tipoServicoMultisoftware, unidadeTrabalho);
                else
                {
                    Repositorio.Embarcador.Cargas.CargaCIOT repCargaCIOT = new Repositorio.Embarcador.Cargas.CargaCIOT(unidadeTrabalho);
                    List<Dominio.Entidades.Embarcador.Cargas.CargaCIOT> cargaCIOTs = repCargaCIOT.BuscarPorCIOTAgSerAdicionado(cargaCIOT.CIOT.Codigo);

                    foreach (Dominio.Entidades.Embarcador.Cargas.CargaCIOT cargaCIOTConfirmar in cargaCIOTs)
                        Servicos.Embarcador.CIOT.CIOT.ConfirmarIntegracaoCargasAoCIOT(cargaCIOTConfirmar, tipoServicoMultisoftware, unidadeTrabalho);
                }

                unidadeTrabalho.CommitChanges();
            }

            return retAdicionarViagem;
        }

        public void ObterFaturasTransportadores(DateTime dataInicial, DateTime dataFinal, Repositorio.UnitOfWork unitOfWork)
        {
            Repositorio.Empresa repEmpresa = new Repositorio.Empresa(unitOfWork);
            Repositorio.Embarcador.Documentos.CIOT repCIOT = new Repositorio.Embarcador.Documentos.CIOT(unitOfWork);

            Repositorio.Embarcador.CIOT.ConfiguracaoCIOT repConfiguracaoCIOT = new Repositorio.Embarcador.CIOT.ConfiguracaoCIOT(unitOfWork);

            List<Dominio.Entidades.Embarcador.CIOT.ConfiguracaoCIOT> configuracaoCIOTs = repConfiguracaoCIOT.BuscarPorConsultarFaturas();

            foreach (Dominio.Entidades.Embarcador.CIOT.ConfiguracaoCIOT configuracaoCIOT in configuracaoCIOTs)
            {
                if (configuracaoCIOT.OperadoraCIOT == Dominio.ObjetosDeValor.Embarcador.Enumeradores.OperadoraCIOT.eFrete)
                {
                    Servicos.Embarcador.CIOT.EFrete serEFrete = new EFrete();
                    serEFrete.ObterFaturasTransportadores(configuracaoCIOT, dataInicial, dataFinal, unitOfWork);
                }
                else if (configuracaoCIOT.OperadoraCIOT == Dominio.ObjetosDeValor.Embarcador.Enumeradores.OperadoraCIOT.Repom)
                {
                    throw new Exception("Obter faturas dos transportadores não implementado para a Repom.");
                }
            }
        }

        public void IntegrarMotoristasPendentesIntegracao(int numeroTentativas, double minutosACadaTentativa, Repositorio.UnitOfWork unitOfWork)
        {
            Repositorio.Embarcador.Transportadores.MotoristaIntegracao repositorioMotoristaIntegracao = new Repositorio.Embarcador.Transportadores.MotoristaIntegracao(unitOfWork);

            List<Dominio.Entidades.Embarcador.Transportadores.MotoristaIntegracao> integracoes = repositorioMotoristaIntegracao.BuscarPendentesIntegracaoPorTipo(numeroTentativas, minutosACadaTentativa, Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoIntegracao.CIOT);

            if (integracoes.Count == 0)
                return;

            foreach (Dominio.Entidades.Embarcador.Transportadores.MotoristaIntegracao integracao in integracoes)
            {
                bool sucesso = true;
                string mensagemErro = "";
                if (integracao.ConfiguracaoCIOT.OperadoraCIOT == Dominio.ObjetosDeValor.Embarcador.Enumeradores.OperadoraCIOT.eFrete)
                {
                    Servicos.Embarcador.CIOT.EFrete serEfrete = new EFrete();
                    sucesso = serEfrete.IntegrarMotoristaPendenteIntegracao(integracao, out mensagemErro, unitOfWork);
                }
                else if (integracao.ConfiguracaoCIOT.OperadoraCIOT == Dominio.ObjetosDeValor.Embarcador.Enumeradores.OperadoraCIOT.Repom)
                {
                    //todo: repom
                    sucesso = false;
                    mensagemErro = "Repom não possui integração com motorista";
                }

                integracao.SituacaoIntegracao = sucesso ? Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoIntegracao.Integrado : Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoIntegracao.ProblemaIntegracao;
                integracao.NumeroTentativas++;
                integracao.DataIntegracao = DateTime.Now;
                integracao.ProblemaIntegracao = mensagemErro ?? string.Empty;

                repositorioMotoristaIntegracao.Atualizar(integracao);
            }
        }

        public void IntegrarVeiculosPendentesIntegracao(int numeroTentativas, double minutosACadaTentativa, Repositorio.UnitOfWork unitOfWork)
        {
            Repositorio.Embarcador.Veiculos.VeiculoIntegracao repVeiculoIntegracao = new Repositorio.Embarcador.Veiculos.VeiculoIntegracao(unitOfWork);

            List<Dominio.Entidades.Embarcador.Veiculos.VeiculoIntegracao> integracoes = repVeiculoIntegracao.BuscarPendentesIntegracaoPorTipo(numeroTentativas, minutosACadaTentativa, Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoIntegracao.CIOT);

            if (integracoes.Count == 0)
                return;

            foreach (Dominio.Entidades.Embarcador.Veiculos.VeiculoIntegracao integracao in integracoes)
            {
                bool sucesso = true;
                string mensagemErro = "";
                if (integracao.ConfiguracaoCIOT.OperadoraCIOT == Dominio.ObjetosDeValor.Embarcador.Enumeradores.OperadoraCIOT.eFrete)
                {
                    Servicos.Embarcador.CIOT.EFrete serEfrete = new EFrete();
                    sucesso = serEfrete.IntegrarVeiculoPendenteIntegracao(integracao, out mensagemErro, unitOfWork);
                }
                else if (integracao.ConfiguracaoCIOT.OperadoraCIOT == Dominio.ObjetosDeValor.Embarcador.Enumeradores.OperadoraCIOT.Repom)
                {
                    //todo: repom
                    sucesso = false;
                    mensagemErro = "Repom não possui integração com motorista";
                }
                integracao.SituacaoIntegracao = sucesso ? Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoIntegracao.Integrado : Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoIntegracao.ProblemaIntegracao;
                integracao.NumeroTentativas++;
                integracao.DataIntegracao = DateTime.Now;
                integracao.ProblemaIntegracao = mensagemErro ?? string.Empty;

                repVeiculoIntegracao.Atualizar(integracao);
            }
        }

        public static void ConfirmarIntegracaoCargasAoCIOT(Dominio.Entidades.Embarcador.Cargas.CargaCIOT cargaCIOT, AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware tipoServicoMultisoftware, Repositorio.UnitOfWork unitOfWork)
        {
            Repositorio.Embarcador.Cargas.CargaCIOT repCargaCIOT = new Repositorio.Embarcador.Cargas.CargaCIOT(unitOfWork);
            Servicos.Embarcador.Terceiros.ContratoFrete serContratoFrete = new Terceiros.ContratoFrete(unitOfWork);
            serContratoFrete.AprovarContratoViaCIOTAutorizado(cargaCIOT, tipoServicoMultisoftware, unitOfWork);
            cargaCIOT.CargaAdicionadaAoCIOT = true;
            repCargaCIOT.Atualizar(cargaCIOT);
        }

        public static bool IntegrarAutorizacaoPagamento(out string mensagem, Dominio.Entidades.Embarcador.Documentos.CIOT ciot, Dominio.Entidades.Usuario usuario, Dominio.ObjetosDeValor.Embarcador.Auditoria.Auditado auditado, AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware tipoServicoMultisoftware, Repositorio.UnitOfWork unitOfWork)
        {
            if (ciot.Situacao != Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoCIOT.Encerrado)
            {
                mensagem = "O CIOT deve estar encerrado para realizar a autorização do pagamento.";
                return false;
            }

            switch (ciot.Operadora)
            {
                case Dominio.ObjetosDeValor.Embarcador.Enumeradores.OperadoraCIOT.Repom:
                    Servicos.Embarcador.CIOT.Repom svcRepom = new Servicos.Embarcador.CIOT.Repom();

                    if (!svcRepom.IntegrarAutorizacaoPagamento(out mensagem, ciot, usuario, unitOfWork))
                        return false;

                    mensagem = "Autorização de pagamento realizada com sucesso.";
                    return true;

                case Dominio.ObjetosDeValor.Embarcador.Enumeradores.OperadoraCIOT.Pamcard:
                    Servicos.Embarcador.CIOT.Pamcard svcPamcard = new Servicos.Embarcador.CIOT.Pamcard();

                    if (!svcPamcard.AutorizarPagamentoCIOT(out mensagem, ciot, usuario, unitOfWork))
                        return false;

                    mensagem = "Autorização de pagamento realizada com sucesso.";
                    return true;

                case OperadoraCIOT.Target:
                    Servicos.Embarcador.CIOT.Target svcTarget = new Servicos.Embarcador.CIOT.Target();

                    if (!svcTarget.FinalizarOperacaoTransporte(out mensagem, ciot, usuario, unitOfWork))
                        return false;

                    mensagem = "Autorização de pagamento realizada com sucesso.";
                    return true;
                case OperadoraCIOT.Ambipar:
                    Servicos.Embarcador.CIOT.Ambipar ambipar = new Servicos.Embarcador.CIOT.Ambipar(unitOfWork);

                    if (!ambipar.AutorizarPagamentoCIOT(out mensagem, ciot, usuario, unitOfWork))
                        return false;

                    mensagem = "Autorização de pagamento realizada com sucesso.";
                    return true;

                case OperadoraCIOT.RepomFrete:
                    Servicos.Embarcador.CIOT.RepomFrete.IntegracaoRepomFrete svcRepomFrete = new RepomFrete.IntegracaoRepomFrete();

                    if (!svcRepomFrete.IntegrarAutorizacaoPagamento(out mensagem, ciot, null, usuario, unitOfWork))
                        return false;

                    mensagem = "Autorização de pagamento realizada com sucesso.";
                    return true;

                case OperadoraCIOT.TruckPad:
                    Servicos.Embarcador.CIOT.TruckPad.IntegracaoTruckPad svcTruckPad = new TruckPad.IntegracaoTruckPad();

                    if (!svcTruckPad.IntegrarAutorizacaoPagamento(out mensagem, ciot, usuario, unitOfWork))
                        return false;

                    mensagem = "Autorização de pagamento realizada com sucesso.";
                    return true;

                default:
                    mensagem = $"A operadora {ciot.Operadora.ObterDescricao()} não possui integração para autorização de pagamento.";
                    return false;
            }
        }

        public static bool IntegrarAutorizacaoPagamentoParcela(out string mensagem, Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoAutorizacaoPagamentoCIOTParcela tipoAutorizacaoPagamentoCIOTParcela, Dominio.Entidades.Embarcador.Documentos.CIOT ciot, Dominio.Entidades.Usuario usuario, Dominio.ObjetosDeValor.Embarcador.Auditoria.Auditado auditado, AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware tipoServicoMultisoftware, Repositorio.UnitOfWork unitOfWork)
        {
            Repositorio.Embarcador.Terceiros.ContratoFrete repContratoFrete = new Repositorio.Embarcador.Terceiros.ContratoFrete(unitOfWork);

            Dominio.Entidades.Embarcador.Cargas.CargaCIOT cargaCIOT = ciot.CargaCIOT.FirstOrDefault();
            Dominio.Entidades.Embarcador.Terceiros.ContratoFrete contrato = cargaCIOT.ContratoFrete;

            switch (ciot.Operadora)
            {
                case Dominio.ObjetosDeValor.Embarcador.Enumeradores.OperadoraCIOT.Pamcard:
                    Servicos.Embarcador.CIOT.Pamcard svcPamcard = new Servicos.Embarcador.CIOT.Pamcard();

                    if (!svcPamcard.AutorizarPagamentoParcelaCIOT(out mensagem, tipoAutorizacaoPagamentoCIOTParcela, ciot, usuario, unitOfWork))
                        return false;

                    mensagem = $"Autorização de pagamento do {tipoAutorizacaoPagamentoCIOTParcela.ObterDescricao()} realizada com sucesso.";
                    break;

                case Dominio.ObjetosDeValor.Embarcador.Enumeradores.OperadoraCIOT.Extratta:
                    Servicos.Embarcador.CIOT.Extratta servicoExtratta = new Extratta();

                    if (!servicoExtratta.AutorizarPagamentoParcelaCIOT(ciot, tipoAutorizacaoPagamentoCIOTParcela, unitOfWork, out mensagem))
                        return false;

                    mensagem = $"Autorização de pagamento do {tipoAutorizacaoPagamentoCIOTParcela.ObterDescricao().ToLower()} realizada com sucesso.";
                    break;

                case Dominio.ObjetosDeValor.Embarcador.Enumeradores.OperadoraCIOT.RepomFrete:
                    Servicos.Embarcador.CIOT.RepomFrete.IntegracaoRepomFrete servicoRepomFrete = new Embarcador.CIOT.RepomFrete.IntegracaoRepomFrete();

                    if (!servicoRepomFrete.IntegrarAutorizacaoPagamento(out mensagem, ciot, null, usuario, unitOfWork, tipoAutorizacaoPagamentoCIOTParcela))
                        return false;

                    mensagem = $"Autorização de pagamento do {tipoAutorizacaoPagamentoCIOTParcela.ObterDescricao().ToLower()} realizada com sucesso.";
                    break;

                case Dominio.ObjetosDeValor.Embarcador.Enumeradores.OperadoraCIOT.TruckPad:
                    Servicos.Embarcador.CIOT.TruckPad.IntegracaoTruckPad servicoTruckPad = new Embarcador.CIOT.TruckPad.IntegracaoTruckPad();

                    if (!servicoTruckPad.IntegrarAutorizacaoPagamento(out mensagem, ciot, usuario, unitOfWork, tipoAutorizacaoPagamentoCIOTParcela))
                        return false;

                    mensagem = $"Autorização de pagamento do {tipoAutorizacaoPagamentoCIOTParcela.ObterDescricao().ToLower()} realizada com sucesso.";
                    break;

                default:
                    mensagem = $"A operadora {ciot.Operadora.ObterDescricao()} não possui integração para autorização de pagamento por parcela.";
                    return false;
            }

            if (tipoAutorizacaoPagamentoCIOTParcela == TipoAutorizacaoPagamentoCIOTParcela.Adiantamento || tipoAutorizacaoPagamentoCIOTParcela == TipoAutorizacaoPagamentoCIOTParcela.Saldo)
            {
                if (tipoAutorizacaoPagamentoCIOTParcela == TipoAutorizacaoPagamentoCIOTParcela.Adiantamento)
                    cargaCIOT.ContratoFrete.DataAutorizacaoPagamentoAdiantamento = System.DateTime.Now;
                else if (tipoAutorizacaoPagamentoCIOTParcela == TipoAutorizacaoPagamentoCIOTParcela.Saldo)
                    cargaCIOT.ContratoFrete.DataAutorizacaoPagamentoSaldo = System.DateTime.Now;

                if (cargaCIOT.ContratoFrete.DataAutorizacaoPagamentoAdiantamento != null && cargaCIOT.ContratoFrete.DataAutorizacaoPagamentoSaldo != null)
                    cargaCIOT.ContratoFrete.DataAutorizacaoPagamento = System.DateTime.Now;

                repContratoFrete.Atualizar(cargaCIOT.ContratoFrete);
            }

            Repositorio.Embarcador.Configuracoes.ConfiguracaoFinanceiraContratoFreteTerceiros repConfiguracaoFinanceira = new Repositorio.Embarcador.Configuracoes.ConfiguracaoFinanceiraContratoFreteTerceiros(unitOfWork);

            Servicos.Embarcador.Financeiro.ProcessoMovimento svcProcessoMovimento = new Servicos.Embarcador.Financeiro.ProcessoMovimento();
            Servicos.Embarcador.Terceiros.ContratoFrete servicoContratoFrete = new Servicos.Embarcador.Terceiros.ContratoFrete(unitOfWork);

            Dominio.Entidades.Embarcador.Configuracoes.ConfiguracaoFinanceiraContratoFreteTerceiros configuracaoFinanceira = repConfiguracaoFinanceira.BuscarPrimeiroRegistro();

            if (configuracaoFinanceira != null &&
                ((tipoAutorizacaoPagamentoCIOTParcela == TipoAutorizacaoPagamentoCIOTParcela.Adiantamento && ciot.EfetivacaoAdiantamento == PamcardParcelaTipoEfetivacao.Manual) ||
                 (tipoAutorizacaoPagamentoCIOTParcela == TipoAutorizacaoPagamentoCIOTParcela.Abastecimento && ciot.EfetivacaoAbastecimento == PamcardParcelaTipoEfetivacao.Manual) ||
                 (tipoAutorizacaoPagamentoCIOTParcela == TipoAutorizacaoPagamentoCIOTParcela.Saldo && ciot.EfetivacaoSaldo == PamcardParcelaTipoEfetivacao.Manual)))
            {
                DateTime dataMovimento = DateTime.Now;
                decimal valor = contrato.ValorAdiantamento;

                if (tipoAutorizacaoPagamentoCIOTParcela == TipoAutorizacaoPagamentoCIOTParcela.Abastecimento)
                    valor = contrato.ValorAbastecimento;
                else if (tipoAutorizacaoPagamentoCIOTParcela == TipoAutorizacaoPagamentoCIOTParcela.Saldo)
                    valor = contrato.SaldoAReceber;

                string obsMovimento = $"Referente ao {tipoAutorizacaoPagamentoCIOTParcela.ObterDescricao().ToLower()} do contrato {contrato.NumeroContrato}, liberado manualmente no CIOT " + ciot.Numero + ".";

                Dominio.Entidades.Embarcador.CIOT.CIOTConfiguracaoFinanceira configuracaoFinanceiraCIOT = null;
                if (ciot.ConfiguracaoCIOT?.ConfiguracaoMovimentoFinanceiro ?? false)
                {
                    configuracaoFinanceiraCIOT = servicoContratoFrete.ObterCIOTConfiguracaoFinanceira(contrato, ciot.ConfiguracaoCIOT, unitOfWork);
                    if (configuracaoFinanceiraCIOT != null)
                        svcProcessoMovimento.GerarMovimentacao(configuracaoFinanceiraCIOT.TipoMovimentoParaUso, dataMovimento, valor, contrato.NumeroContrato.ToString(), obsMovimento, unitOfWork, TipoDocumentoMovimento.ContratoFrete, tipoServicoMultisoftware, 0, null, null, 0, null, contrato.TransportadorTerceiro);
                }

                if (configuracaoFinanceiraCIOT == null)
                {
                    if (configuracaoFinanceira.GerarMovimentoAutomaticoNaGeracaoContratoFrete && configuracaoFinanceira.TipoMovimentoValorPagoTerceiroCIOT != null)
                        svcProcessoMovimento.GerarMovimentacao(configuracaoFinanceira.TipoMovimentoValorPagoTerceiroCIOT, dataMovimento, valor, contrato.NumeroContrato.ToString(), obsMovimento, unitOfWork, TipoDocumentoMovimento.ContratoFrete, tipoServicoMultisoftware, 0, null, null, 0, null, contrato.TransportadorTerceiro);
                    else if (configuracaoFinanceira.GerarMovimentoAutomaticoPorTipoOperacao)
                    {
                        Dominio.Entidades.Embarcador.Configuracoes.ConfiguracaoFinanceiraContratoFreteTerceirosTipoOperacao configuracaoTipoOperacao = Servicos.Embarcador.Terceiros.ContratoFrete.ObterConfiguracaoFinanceiraPorTipoOperacao(contrato, configuracaoFinanceira, unitOfWork);

                        if (configuracaoTipoOperacao != null)
                            svcProcessoMovimento.GerarMovimentacao(configuracaoTipoOperacao.TipoMovimentoPagamentoViaCIOT, dataMovimento, valor, contrato.NumeroContrato.ToString(), obsMovimento, unitOfWork, TipoDocumentoMovimento.ContratoFrete, tipoServicoMultisoftware, 0, null, null, 0, null, contrato.TransportadorTerceiro);
                    }
                }
            }

            return true;
        }

        public bool IntegrarAutorizacaoPagamentoAcrescimoDesconto(out string mensagem, Dominio.Entidades.Embarcador.Documentos.CIOT ciot, Dominio.Entidades.Embarcador.Terceiros.ContratoFreteAcrescimoDesconto contratoFreteAcrescimoDesconto, Repositorio.UnitOfWork unitOfWork)
        {
            switch (ciot.Operadora)
            {
                case OperadoraCIOT.Pamcard:
                    Servicos.Embarcador.CIOT.Pamcard svcPamcard = new Servicos.Embarcador.CIOT.Pamcard();

                    if (!svcPamcard.AutorizarPagamentoAcrescimoDesconto(out mensagem, ciot, unitOfWork))
                        return false;

                    mensagem = "Autorização de pagamento do acréscimo/desconto realizada com sucesso.";
                    return true;

                default:
                    mensagem = $"A operadora {ciot.Operadora.ObterDescricao()} não possui integração para autorização de pagamento do acréscimo/desconto.";
                    return false;
            }
        }

        public static bool IntegrarLiberacaoViagem(out string mensagem, Dominio.Entidades.Embarcador.Documentos.CIOT ciot, Dominio.Entidades.Usuario usuario, Dominio.ObjetosDeValor.Embarcador.Auditoria.Auditado auditado, AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware tipoServicoMultisoftware, Repositorio.UnitOfWork unitOfWork)
        {
            if (ciot.Situacao != Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoCIOT.AgLiberarViagem)
            {
                mensagem = "O CIOT deve estar aguardando liberação de viagem.";
                return false;
            }

            switch (ciot.Operadora)
            {
                case Dominio.ObjetosDeValor.Embarcador.Enumeradores.OperadoraCIOT.Pagbem:
                    Servicos.Embarcador.CIOT.Pagbem svcPagbem = new Servicos.Embarcador.CIOT.Pagbem();

                    if (!svcPagbem.LiberarViagem(ciot, auditado, unitOfWork, out mensagem))
                        return false;

                    mensagem = "Liberação de viagem realizada com sucesso.";
                    return true;
                default:
                    mensagem = $"A operadora {ciot.Operadora.ObterDescricao()} não possui integração para liberação de viagem.";
                    return false;
            }
        }

        public static bool IntegrarMovimentoFinanceiro(out string mensagem, Dominio.Entidades.Embarcador.Cargas.CargaCIOT cargaCIOT, Dominio.Entidades.Embarcador.Fatura.Justificativa justificativa, decimal valor, AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware tipoServicoMultisoftware, Repositorio.UnitOfWork unitOfWork)
        {
            if (cargaCIOT.CIOT.Situacao != SituacaoCIOT.Aberto &&
                cargaCIOT.CIOT.Situacao != SituacaoCIOT.AgLiberarViagem &&
                cargaCIOT.CIOT.Situacao != SituacaoCIOT.Encerrado)
            {
                mensagem = "O CIOT deve estar aberto/encerrado realizar a inclusão de movimento financeiro.";
                return false;
            }

            switch (cargaCIOT.CIOT.Operadora)
            {
                case OperadoraCIOT.Repom:
                    Servicos.Embarcador.CIOT.Repom svcRepom = new Servicos.Embarcador.CIOT.Repom();

                    if (!svcRepom.IntegrarMovimentoFinanceiro(out mensagem, cargaCIOT, justificativa, valor, unitOfWork))
                        return false;

                    mensagem = "Integração de movimento financeiro realizado com sucesso.";
                    return true;

                case OperadoraCIOT.Pagbem:
                    Servicos.Embarcador.CIOT.Pagbem svcPagbem = new Servicos.Embarcador.CIOT.Pagbem();

                    if (!svcPagbem.IntegrarMovimentoFinanceiro(out mensagem, cargaCIOT, justificativa, valor, unitOfWork))
                        return false;

                    mensagem = "Integração de movimento financeiro realizado com sucesso.";
                    return true;
                case OperadoraCIOT.Pamcard:
                    Servicos.Embarcador.CIOT.Pamcard svcPamcard = new Servicos.Embarcador.CIOT.Pamcard();

                    if (!svcPamcard.IntegrarMovimentoFinanceiro(out mensagem, cargaCIOT, justificativa, valor, unitOfWork))
                        return false;

                    mensagem = "Integração de movimento financeiro realizado com sucesso.";
                    return true;
                case OperadoraCIOT.Target:
                    Servicos.Embarcador.CIOT.Target svcTarget = new Servicos.Embarcador.CIOT.Target();

                    if (!svcTarget.IntegrarMovimentoFinanceiro(out mensagem, cargaCIOT, justificativa, valor, tipoServicoMultisoftware, unitOfWork))
                        return false;

                    mensagem = "Integração de movimento financeiro realizado com sucesso.";
                    return true;

                case OperadoraCIOT.Ambipar:
                    Servicos.Embarcador.CIOT.Ambipar svcAmbipar = new Servicos.Embarcador.CIOT.Ambipar(unitOfWork);

                    if (!svcAmbipar.IntegrarMovimentoFinanceiro(out mensagem, cargaCIOT, justificativa, valor))
                        return false;

                    mensagem = "Integração de movimento financeiro CIOT Ambipar realizado com sucesso.";
                    return true;

                case OperadoraCIOT.RepomFrete:
                    Servicos.Embarcador.CIOT.RepomFrete.IntegracaoRepomFrete svcRepomFrete = new Servicos.Embarcador.CIOT.RepomFrete.IntegracaoRepomFrete();

                    if (!svcRepomFrete.IntegrarMovimentoFinanceiro(out mensagem, cargaCIOT, justificativa, valor, unitOfWork))
                        return false;

                    mensagem = "Integração de movimento financeiro realizado com sucesso.";
                    return true;

                case OperadoraCIOT.eFrete:
                    Servicos.Embarcador.CIOT.EFrete svcEFrete = new Servicos.Embarcador.CIOT.EFrete();

                    if (!svcEFrete.IntegrarMovimentoFinanceiro(out mensagem, cargaCIOT, justificativa, valor, unitOfWork))
                        return false;

                    mensagem = "Integração de movimento financeiro realizado com sucesso.";
                    return true;
                case OperadoraCIOT.TruckPad:
                    Servicos.Embarcador.CIOT.TruckPad.IntegracaoTruckPad svcTruckPad = new Servicos.Embarcador.CIOT.TruckPad.IntegracaoTruckPad();

                    if (!svcTruckPad.IntegrarMovimentoFinanceiro(out mensagem, cargaCIOT, justificativa, valor, unitOfWork))
                        return false;

                    mensagem = "Integração de movimento financeiro realizado com sucesso.";
                    return true;

                default:
                    mensagem = $"A operadora {cargaCIOT.CIOT.Operadora.ObterDescricao()} não possui integração para movimento financeiro.";
                    return false;
            }
        }

        public static void ConciliarCIOTs(AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware tipoServicoMultisoftware, Repositorio.UnitOfWork unitOfWork, bool buscarDataAtual = false)
        {
            try
            {
                Repositorio.Embarcador.CIOT.ConfiguracaoCIOT repConfiguracaoCIOT = new Repositorio.Embarcador.CIOT.ConfiguracaoCIOT(unitOfWork);

                List<Dominio.ObjetosDeValor.Embarcador.Enumeradores.OperadoraCIOT> operadorasHabilitadas = repConfiguracaoCIOT.BuscarOperadorasComConciliacaoFinanceiraHabilitada();

                if (operadorasHabilitadas.Count <= 0)
                    return;

                DateTime dataConciliacao = buscarDataAtual ? DateTime.Now : DateTime.Now.AddDays(-1);

                foreach (Dominio.ObjetosDeValor.Embarcador.Enumeradores.OperadoraCIOT operadora in operadorasHabilitadas)
                {
                    switch (operadora)
                    {
                        case Dominio.ObjetosDeValor.Embarcador.Enumeradores.OperadoraCIOT.eFrete:
                            Servicos.Log.TratarErro("Conciliação financeira não disponível para a e-Frete.");
                            break;
                        case Dominio.ObjetosDeValor.Embarcador.Enumeradores.OperadoraCIOT.Repom:
                            Servicos.Log.TratarErro("Conciliação financeira não disponível para a Repom.");
                            break;
                        case Dominio.ObjetosDeValor.Embarcador.Enumeradores.OperadoraCIOT.Pamcard:
                            Servicos.Log.TratarErro("Conciliação financeira não disponível para a Pamcard.");
                            break;
                        case Dominio.ObjetosDeValor.Embarcador.Enumeradores.OperadoraCIOT.Pagbem:
                            Servicos.Embarcador.CIOT.Pagbem svcPagBem = new Pagbem();
                            svcPagBem.ConciliarCIOTs(dataConciliacao, unitOfWork, tipoServicoMultisoftware);
                            break;
                        default:
                            Servicos.Log.TratarErro("Conciliação financeira não implementada para a " + operadora.ObterDescricao() + ".");
                            break;
                    }

                    unitOfWork.FlushAndClear();
                }
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);
                unitOfWork.Rollback();
            }
            finally
            {
                unitOfWork.FlushAndClear();
            }
        }

        public static void ConfigurarCIOT(Dominio.Entidades.Embarcador.Documentos.CIOT ciot, Dominio.Entidades.Embarcador.Cargas.Carga carga, Repositorio.UnitOfWork unitOfWork)
        {
            switch (ciot.Operadora)
            {
                case OperadoraCIOT.Pamcard:
                    Servicos.Embarcador.CIOT.Pamcard.ConfigurarCIOT(ciot, carga, unitOfWork);
                    break;
                default:
                    break;
            }
        }

        #endregion

        #region Métodos Privados

        public static void CriarCargaCIOT(Dominio.Entidades.Embarcador.Cargas.Carga carga, Dominio.Entidades.Embarcador.Documentos.CIOT ciot, Dominio.Entidades.Embarcador.Terceiros.ContratoFrete contratoFrete, Repositorio.UnitOfWork unitOfWork, bool cargaAberturaCIOT = false)
        {
            Repositorio.Embarcador.Cargas.Carga repCarga = new Repositorio.Embarcador.Cargas.Carga(unitOfWork);
            Repositorio.Embarcador.Cargas.CargaCIOT repCargaCIOT = new Repositorio.Embarcador.Cargas.CargaCIOT(unitOfWork);
            Repositorio.ImpostoContratoFrete repImpostoContratoFrete = new Repositorio.ImpostoContratoFrete(unitOfWork);
            Repositorio.INSSImpostoContratoFrete repINSSImpostoContratoFrete = new Repositorio.INSSImpostoContratoFrete(unitOfWork);
            Repositorio.IRImpostoContratoFrete repIRImpostoContratoFrete = new Repositorio.IRImpostoContratoFrete(unitOfWork);
            Repositorio.Embarcador.Pedidos.PedidoXMLNotaFiscal repPedidoXMLNotaFiscal = new Repositorio.Embarcador.Pedidos.PedidoXMLNotaFiscal(unitOfWork);

            bool inserir = false;

            Dominio.Entidades.Embarcador.Cargas.CargaCIOT cargaCIOT = repCargaCIOT.BuscarPorCarga(carga.Codigo);

            if (cargaCIOT == null)
            {
                cargaCIOT = new Dominio.Entidades.Embarcador.Cargas.CargaCIOT();
                inserir = true;
            }

            cargaCIOT.Carga = carga;
            cargaCIOT.CIOT = ciot;
            cargaCIOT.ContratoFrete = contratoFrete;

            Dominio.Entidades.ImpostoContratoFrete impostoContratoFrete = repImpostoContratoFrete.BuscarPorEmpresa(carga.Empresa.Codigo);
            decimal valorFrete = contratoFrete.ValorFreteSubcontratacao + contratoFrete.ValorPedagio;
            decimal percentualAdiantamentoFretesTerceiro = contratoFrete.PercentualAdiantamento;

            //todo: ver com o luli
            if (impostoContratoFrete != null)
            {
                Dominio.Entidades.INSSImpostoContratoFrete iNSSImpostoContratoFrete = repINSSImpostoContratoFrete.BuscarPorImpostoEFaixa(impostoContratoFrete.Codigo, valorFrete);
                Dominio.Entidades.IRImpostoContratoFrete irImpostoContratoFrete = repIRImpostoContratoFrete.BuscarPorImpostoEFaixa(impostoContratoFrete.Codigo, valorFrete);

                cargaCIOT.ValorSEST = (valorFrete * (impostoContratoFrete.PercentualBCINSS / 100)) * (impostoContratoFrete.AliquotaSEST / 100);
                cargaCIOT.ValorSENAT = (valorFrete * (impostoContratoFrete.PercentualBCINSS / 100)) * (impostoContratoFrete.AliquotaSENAT / 100);

                decimal baseCalculoINSS = valorFrete * (impostoContratoFrete.PercentualBCINSS / 100);

                if (iNSSImpostoContratoFrete != null)
                {
                    cargaCIOT.ValorINSS = baseCalculoINSS * (iNSSImpostoContratoFrete.PercentualAplicar / 100);

                    if (cargaCIOT.ValorINSS > impostoContratoFrete.ValorTetoRetencaoINSS)
                        cargaCIOT.ValorINSS = impostoContratoFrete.ValorTetoRetencaoINSS;
                }

                decimal baseCalculoIR = valorFrete * (impostoContratoFrete.PercentualBCIR / 100);
                if (baseCalculoIR > 0m && cargaCIOT.ValorINSS > 0m)
                    baseCalculoIR = baseCalculoIR - cargaCIOT.ValorINSS;
                if (baseCalculoIR > 0m && cargaCIOT.ValorSEST > 0m)
                    baseCalculoIR = baseCalculoIR - cargaCIOT.ValorSEST;
                if (baseCalculoIR > 0m && cargaCIOT.ValorSENAT > 0m)
                    baseCalculoIR = baseCalculoIR - cargaCIOT.ValorSENAT;
                if (irImpostoContratoFrete != null)
                {
                    cargaCIOT.ValorIRRF = (baseCalculoIR * (irImpostoContratoFrete.PercentualAplicar / 100)) - irImpostoContratoFrete.ValorDeduzir;
                    if (cargaCIOT.ValorIRRF < 0m)
                        cargaCIOT.ValorIRRF = 0m;
                }
            }

            valorFrete = valorFrete - cargaCIOT.ValorINSS - cargaCIOT.ValorIRRF - cargaCIOT.ValorSENAT - cargaCIOT.ValorSEST;
            decimal adiantamento = valorFrete * percentualAdiantamentoFretesTerceiro / 100;

            cargaCIOT.ValorAdiantamento = adiantamento;
            cargaCIOT.PesoBruto = repPedidoXMLNotaFiscal.BuscarPesoPorCarga(carga.Codigo);
            cargaCIOT.TipoQuebra = Dominio.Enumeradores.TipoQuebra.Integral;
            cargaCIOT.TipoTolerancia = Dominio.Enumeradores.TipoTolerancia.Peso;
            cargaCIOT.ValorFrete = valorFrete;
            cargaCIOT.Motorista = carga.Motoristas?.FirstOrDefault() ?? null;
            cargaCIOT.CargaAberturaCIOT = cargaAberturaCIOT;

            cargaCIOT.ValorTotalMercadoria = repPedidoXMLNotaFiscal.BuscarValorTotalPorCarga(carga.Codigo);

            if (inserir)
                repCargaCIOT.Inserir(cargaCIOT);
            else
                repCargaCIOT.Atualizar(cargaCIOT);

            carga.IntegrandoCIOT = true;
            repCarga.Atualizar(carga);
        }

        public static void CriarCargaCIOTAutomaticamente(Dominio.Entidades.Embarcador.Cargas.Carga carga, Dominio.Entidades.Embarcador.Documentos.CIOT ciot, decimal valorAdiantamento, decimal valorFrete, Repositorio.UnitOfWork unitOfWork)
        {
            Repositorio.Embarcador.Cargas.Carga repCarga = new Repositorio.Embarcador.Cargas.Carga(unitOfWork);
            Repositorio.Embarcador.Cargas.CargaCIOT repCargaCIOT = new Repositorio.Embarcador.Cargas.CargaCIOT(unitOfWork);
            Repositorio.ImpostoContratoFrete repImpostoContratoFrete = new Repositorio.ImpostoContratoFrete(unitOfWork);
            Repositorio.INSSImpostoContratoFrete repINSSImpostoContratoFrete = new Repositorio.INSSImpostoContratoFrete(unitOfWork);
            Repositorio.IRImpostoContratoFrete repIRImpostoContratoFrete = new Repositorio.IRImpostoContratoFrete(unitOfWork);
            Repositorio.Embarcador.Pedidos.PedidoXMLNotaFiscal repPedidoXMLNotaFiscal = new Repositorio.Embarcador.Pedidos.PedidoXMLNotaFiscal(unitOfWork);

            Dominio.Entidades.Embarcador.Cargas.CargaCIOT cargaCIOT = repCargaCIOT.BuscarPorCargaCIOT(carga.Codigo, ciot.Codigo);
            cargaCIOT ??= new Dominio.Entidades.Embarcador.Cargas.CargaCIOT();

            Dominio.Entidades.ImpostoContratoFrete impostoContratoFrete = repImpostoContratoFrete.BuscarPorEmpresa(carga.Empresa.Codigo);

            if (impostoContratoFrete != null)
            {
                Dominio.Entidades.INSSImpostoContratoFrete iNSSImpostoContratoFrete = repINSSImpostoContratoFrete.BuscarPorImpostoEFaixa(impostoContratoFrete.Codigo, valorFrete);
                Dominio.Entidades.IRImpostoContratoFrete irImpostoContratoFrete = repIRImpostoContratoFrete.BuscarPorImpostoEFaixa(impostoContratoFrete.Codigo, valorFrete);

                cargaCIOT.ValorSEST = (valorFrete * (impostoContratoFrete.PercentualBCINSS / 100)) * (impostoContratoFrete.AliquotaSEST / 100);
                cargaCIOT.ValorSENAT = (valorFrete * (impostoContratoFrete.PercentualBCINSS / 100)) * (impostoContratoFrete.AliquotaSENAT / 100);

                decimal baseCalculoINSS = valorFrete * (impostoContratoFrete.PercentualBCINSS / 100);

                if (iNSSImpostoContratoFrete != null)
                {
                    cargaCIOT.ValorINSS = baseCalculoINSS * (iNSSImpostoContratoFrete.PercentualAplicar / 100);

                    if (cargaCIOT.ValorINSS > impostoContratoFrete.ValorTetoRetencaoINSS)
                        cargaCIOT.ValorINSS = impostoContratoFrete.ValorTetoRetencaoINSS;
                }

                decimal baseCalculoIR = valorFrete * (impostoContratoFrete.PercentualBCIR / 100);
                if (baseCalculoIR > 0m && cargaCIOT.ValorINSS > 0m)
                    baseCalculoIR = baseCalculoIR - cargaCIOT.ValorINSS;
                if (baseCalculoIR > 0m && cargaCIOT.ValorSEST > 0m)
                    baseCalculoIR = baseCalculoIR - cargaCIOT.ValorSEST;
                if (baseCalculoIR > 0m && cargaCIOT.ValorSENAT > 0m)
                    baseCalculoIR = baseCalculoIR - cargaCIOT.ValorSENAT;
                if (irImpostoContratoFrete != null)
                {
                    cargaCIOT.ValorIRRF = (baseCalculoIR * (irImpostoContratoFrete.PercentualAplicar / 100)) - irImpostoContratoFrete.ValorDeduzir;
                    if (cargaCIOT.ValorIRRF < 0m)
                        cargaCIOT.ValorIRRF = 0m;
                }
            }

            cargaCIOT.Carga = carga;
            cargaCIOT.CIOT = ciot;
            cargaCIOT.ValorAdiantamento = valorAdiantamento;
            cargaCIOT.PesoBruto = repPedidoXMLNotaFiscal.BuscarPesoPorCarga(carga.Codigo);
            cargaCIOT.TipoQuebra = Dominio.Enumeradores.TipoQuebra.Integral;
            cargaCIOT.TipoTolerancia = Dominio.Enumeradores.TipoTolerancia.Peso;
            cargaCIOT.ValorFrete = valorFrete;
            cargaCIOT.Motorista = carga.Motoristas?.FirstOrDefault() ?? null;
            cargaCIOT.CargaAberturaCIOT = true;
            cargaCIOT.CargaAdicionadaAoCIOT = true;

            cargaCIOT.ValorTotalMercadoria = repPedidoXMLNotaFiscal.BuscarValorTotalPorCarga(carga.Codigo);

            if (cargaCIOT.Codigo <= 0)
                repCargaCIOT.Inserir(cargaCIOT);
            else
                repCargaCIOT.Atualizar(cargaCIOT);


            carga.IntegrandoCIOT = true;

            repCarga.Atualizar(carga);
        }

        private static DateTime ObterPrevisaoFinalViagemCarga(Dominio.Entidades.Embarcador.Cargas.Carga carga, Repositorio.UnitOfWork unitOfWork)
        {
            Servicos.Embarcador.GestaoPatio.FluxoGestaoPatio servicoFluxoGestaoPatio = new Servicos.Embarcador.GestaoPatio.FluxoGestaoPatio(unitOfWork);
            Dominio.Entidades.Embarcador.GestaoPatio.FluxoGestaoPatio gestaoPatio = servicoFluxoGestaoPatio.ObterFluxoGestaoPatio(carga);
            DateTime? dataFinalViagem = gestaoPatio?.DataFimViagemPrevista;

            return dataFinalViagem ?? DateTime.Now.AddDays(1);
        }

        private static DateTime? ObterDataParaFechamento(Dominio.Entidades.Embarcador.Pessoas.ModalidadeTransportadoraPessoas modalidadeTerceiro, Repositorio.UnitOfWork unitOfWork)
        {
            DateTime? retorno = null;

            Repositorio.Embarcador.Pessoas.ModalidadeTransportadoraPessoasDiaFechamentoCIOTPeriodo repModalidadeTransportadoraPessoasDiaFechamentoCIOTPeriodo = new Repositorio.Embarcador.Pessoas.ModalidadeTransportadoraPessoasDiaFechamentoCIOTPeriodo(unitOfWork);
            if (modalidadeTerceiro != null)
            {
                List<Dominio.Entidades.Embarcador.Pessoas.ModalidadeTransportadoraPessoasDiaFechamentoCIOTPeriodo> listaDiasFechamento = repModalidadeTransportadoraPessoasDiaFechamentoCIOTPeriodo.BuscarPorModalidadeTransportador(modalidadeTerceiro.Codigo);

                if (listaDiasFechamento.Count > 0)
                {
                    int diaMesAtual = DateTime.Now.Day;
                    if (diaMesAtual == 31)
                        diaMesAtual = 30;

                    Dominio.Entidades.Embarcador.Pessoas.ModalidadeTransportadoraPessoasDiaFechamentoCIOTPeriodo diaFechamento = listaDiasFechamento.Where(obj => obj.DiaFechamentoCIOT >= diaMesAtual).OrderBy(obj => obj.DiaFechamentoCIOT).FirstOrDefault();

                    if (diaFechamento != null)
                    {
                        int ano = DateTime.Now.Year;
                        int mes = DateTime.Now.Month;
                        int dia = diaFechamento.DiaFechamentoCIOT;
                        int diasNoMes = DateTime.DaysInMonth(ano, mes);

                        if (dia == 30 && diasNoMes == 31)
                            dia = 31;

                        retorno = new DateTime(ano, mes, dia);
                    }
                    else
                    {
                        diaFechamento = listaDiasFechamento.Where(obj => obj.DiaFechamentoCIOT < diaMesAtual).OrderBy(obj => obj.DiaFechamentoCIOT).FirstOrDefault();

                        if (diaFechamento != null)
                        {
                            int ano = DateTime.Now.AddMonths(1).Year;
                            int mes = DateTime.Now.AddMonths(1).Month;
                            int dia = diaFechamento.DiaFechamentoCIOT;
                            int diasNoMes = DateTime.DaysInMonth(ano, mes);

                            if (dia == 30 && diasNoMes == 31)
                                dia = 31;

                            retorno = new DateTime(ano, mes, dia);
                        }
                    }
                }
            }

            return retorno;
        }

        private void GerarIntegracaoSAP_AV(Dominio.Entidades.Embarcador.Documentos.CIOT ciot, Repositorio.UnitOfWork unitOfWork)
        {
            Repositorio.Embarcador.Cargas.TipoIntegracao repTipoIntegracao = new Repositorio.Embarcador.Cargas.TipoIntegracao(unitOfWork);
            Repositorio.Embarcador.Terceiros.ContratoFreteAcrescimoDescontoIntegracao repContratoFreteAcrescimoDescontoIntegracao = new Repositorio.Embarcador.Terceiros.ContratoFreteAcrescimoDescontoIntegracao(unitOfWork);
            Repositorio.Embarcador.Terceiros.ContratoFreteAcrescimoDesconto repContratoFreteAcrescimoDesconto = new Repositorio.Embarcador.Terceiros.ContratoFreteAcrescimoDesconto(unitOfWork);
            Repositorio.Embarcador.Cargas.CargaCIOT repCargaCIOT = new Repositorio.Embarcador.Cargas.CargaCIOT(unitOfWork);

            Dominio.Entidades.Embarcador.Cargas.TipoIntegracao tipoIntegracao = repTipoIntegracao.BuscarPorTipo(TipoIntegracao.SAP_AV);
            Dominio.Entidades.Embarcador.Cargas.CargaCIOT cargaCiot = repCargaCIOT.BuscarPrimeiroPorCIOT(ciot.Codigo);
            Dominio.Entidades.Embarcador.Terceiros.ContratoFreteAcrescimoDesconto contratoFreteAcrescimoDesconto = repContratoFreteAcrescimoDesconto.BuscarPorcCarga(cargaCiot.Carga.Codigo);

            Dominio.Entidades.Embarcador.Terceiros.ContratoFreteAcrescimoDescontoIntegracao contratoIntegracao = new Dominio.Entidades.Embarcador.Terceiros.ContratoFreteAcrescimoDescontoIntegracao
            {
                TipoIntegracao = tipoIntegracao,
                DataIntegracao = DateTime.Now,
                ProblemaIntegracao = "",
                SituacaoIntegracao = SituacaoIntegracao.AgIntegracao,
                ContratoFreteAcrescimoDesconto = contratoFreteAcrescimoDesconto,
                CIOT = ciot
            };

            repContratoFreteAcrescimoDescontoIntegracao.Inserir(contratoIntegracao);
        }

        private static void AtualizarDadosCIOT(Dominio.Entidades.Embarcador.Documentos.CIOT ciot, Dominio.Entidades.Embarcador.Cargas.Carga carga, Dominio.Entidades.Cliente terceiro, Repositorio.UnitOfWork unitOfWork)
        {
            if (ciot.Situacao != SituacaoCIOT.AgIntegracao)
                return;

            Repositorio.Embarcador.Documentos.CIOT repCIOT = new Repositorio.Embarcador.Documentos.CIOT(unitOfWork);

            ciot.Transportador = terceiro;
            ciot.Veiculo = carga.Veiculo;
            ciot.VeiculosVinculados = carga.VeiculosVinculados.ToList();
            ciot.Motorista = carga.Motoristas.FirstOrDefault();

            repCIOT.Atualizar(ciot);

            Servicos.Auditoria.Auditoria.Auditar(new Dominio.ObjetosDeValor.Embarcador.Auditoria.Auditado() { OrigemAuditado = Dominio.ObjetosDeValor.Enumerador.OrigemAuditado.Sistema, TipoAuditado = Dominio.ObjetosDeValor.Enumerador.TipoAuditado.Sistema }, ciot, $"Atualizou o CIOT à partir da geração dos documentos da carga {carga.CodigoCargaEmbarcador}.", unitOfWork);
        }

        #endregion
    }
}
