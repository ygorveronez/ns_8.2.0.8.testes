using Dominio.Excecoes.Embarcador;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Servicos.Embarcador.Frota
{
    public sealed class Infracao : RegraAutorizacao.AprovacaoAlcada
    <
        Dominio.Entidades.Embarcador.Frota.Infracao,
        Dominio.Entidades.Embarcador.Frota.AlcadasInfracao.RegraAutorizacaoInfracao,
        Dominio.Entidades.Embarcador.Frota.AlcadasInfracao.AprovacaoAlcadaInfracao
    >
    {
        #region Construtores

        public Infracao(Repositorio.UnitOfWork unitOfWork) : base(unitOfWork) { }

        #endregion

        #region Métodos Privados

        private void CriarRegrasAprovacao(Dominio.Entidades.Embarcador.Frota.Infracao infracao, List<Dominio.Entidades.Embarcador.Frota.AlcadasInfracao.RegraAutorizacaoInfracao> regras, AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware tipoServicoMultisoftware)
        {
            bool existeRegraSemAprovacao = false;
            Repositorio.Embarcador.Frota.AprovacaoAlcadaInfracao repositorio = new Repositorio.Embarcador.Frota.AprovacaoAlcadaInfracao(_unitOfWork);

            List<Dominio.Entidades.Embarcador.Frota.AlcadasInfracao.RegraAutorizacaoInfracao> listraFiltradaAprovadores = regras.Where(regra => regra.NumeroAprovadores > 0).ToList();
            int menorPrioridadeAprovacao = listraFiltradaAprovadores.Count > 0 ? listraFiltradaAprovadores.Select(regra => regra.PrioridadeAprovacao).Min() : 0;

            foreach (var regra in regras)
            {
                if (regra.NumeroAprovadores > 0)
                {
                    existeRegraSemAprovacao = true;

                    foreach (var aprovador in regra.Aprovadores)
                    {
                        Dominio.Entidades.Embarcador.Frota.AlcadasInfracao.AprovacaoAlcadaInfracao aprovacao = new Dominio.Entidades.Embarcador.Frota.AlcadasInfracao.AprovacaoAlcadaInfracao()
                        {
                            OrigemAprovacao = infracao,
                            Bloqueada = regra.PrioridadeAprovacao > menorPrioridadeAprovacao,
                            Usuario = aprovador,
                            RegraAutorizacao = regra,
                            Situacao = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoAlcadaRegra.Pendente,
                            DataCriacao = infracao.Data,
                            NumeroAprovadores = regra.NumeroAprovadores
                        };

                        repositorio.Inserir(aprovacao);

                        if (!aprovacao.Bloqueada)
                            NotificarAprovador(infracao, aprovacao, tipoServicoMultisoftware);
                    }
                }
                else
                {
                    Dominio.Entidades.Embarcador.Frota.AlcadasInfracao.AprovacaoAlcadaInfracao aprovacao = new Dominio.Entidades.Embarcador.Frota.AlcadasInfracao.AprovacaoAlcadaInfracao()
                    {
                        OrigemAprovacao = infracao,
                        Usuario = null,
                        RegraAutorizacao = regra,
                        Situacao = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoAlcadaRegra.Aprovada,
                        Data = System.DateTime.Now,
                        Motivo = $"Alçada aprovada pela Regra {regra.Descricao}",
                        DataCriacao = infracao.Data,
                    };

                    repositorio.Inserir(aprovacao);
                }
            }

            infracao.Situacao = existeRegraSemAprovacao ? Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoInfracao.AguardandoAprovacao : Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoInfracao.Finalizada;
        }

        private bool GerarPessoaMotorista(Dominio.Entidades.Usuario motorista, Dominio.Entidades.Empresa empresa)
        {
            Servicos.Cliente serCliente = new Servicos.Cliente(_unitOfWork.StringConexao);

            Dominio.ObjetosDeValor.Embarcador.Pessoas.Pessoa objPessoa = new Dominio.ObjetosDeValor.Embarcador.Pessoas.Pessoa();
            objPessoa.ClienteExterior = false;
            objPessoa.CPFCNPJ = motorista.CPF;
            objPessoa.CodigoIntegracao = string.Empty;
            objPessoa.TipoPessoa = Dominio.Enumeradores.TipoPessoa.Fisica;
            objPessoa.CodigoAtividade = 1;
            objPessoa.RGIE = "ISENTO";
            objPessoa.IM = string.Empty;
            objPessoa.InscricaoSuframa = string.Empty;
            objPessoa.RazaoSocial = motorista.Nome;
            objPessoa.NomeFantasia = motorista.Nome;
            objPessoa.CNAE = string.Empty;

            objPessoa.Email = motorista.Email;
            objPessoa.AtualizarEnderecoPessoa = false;
            objPessoa.EmailContador = string.Empty;
            objPessoa.EmailContato = string.Empty;
            objPessoa.EnviarEmialContador = false;
            objPessoa.EnviarEmailContato = false;
            objPessoa.EnviarEmail = false;
            objPessoa.CPFCNPJSemFormato = motorista.CPF;

            Dominio.ObjetosDeValor.Embarcador.Localidade.Endereco objEndereco = new Dominio.ObjetosDeValor.Embarcador.Localidade.Endereco();
            objEndereco.Cidade = new Dominio.ObjetosDeValor.Localidade()
            {
                Codigo = empresa.Localidade.Codigo,
                CodigoIntegracao = string.Empty,
                Descricao = empresa.Localidade.Descricao,
                IBGE = empresa.Localidade.CodigoIBGE,
                Pais = null,
                Regiao = null,
                SiglaUF = empresa.Localidade.Estado.Sigla
            };

            objEndereco.Logradouro = empresa.Endereco;
            objEndereco.Numero = empresa.Numero;
            objEndereco.Complemento = empresa.Complemento;
            objEndereco.CEP = empresa.CEP;
            objEndereco.CEPSemFormato = empresa.CEP;
            objEndereco.Bairro = empresa.Bairro;
            objEndereco.DDDTelefone = string.Empty;
            objEndereco.Telefone = motorista.Telefone;
            objEndereco.Telefone2 = string.Empty;
            objEndereco.CodigoIntegracao = string.Empty;
            objEndereco.InscricaoEstadual = "ISENTO";
            objPessoa.Endereco = objEndereco;

            Dominio.ObjetosDeValor.RetornoVerificacaoCliente retorno = serCliente.ConverterObjetoValorPessoa(objPessoa, string.Empty, _unitOfWork, 0, false);
            return retorno.Status;
        }

        private void GerarMovimentacaoTitulo(Dominio.Entidades.Embarcador.Frota.Infracao infracao, Dominio.Entidades.Embarcador.Financeiro.Titulo titulo, AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware tipoServicoMultisoftware)
        {
            Financeiro.ProcessoMovimento servicoProcessoMovimento = new Financeiro.ProcessoMovimento();

            servicoProcessoMovimento.GerarMovimentacao(
                titulo.TipoMovimento,
                titulo.DataEmissao.Value,
                titulo.ValorOriginal,
                titulo.Codigo.ToString(),
                $"Título {titulo.Sequencia} " + titulo.Observacao,
                _unitOfWork,
                Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoDocumentoMovimento.Outros,
                tipoServicoMultisoftware,
                0,
                null,
                null,
                titulo.Codigo
            );
        }

        private Dominio.Entidades.Cliente ObterPessoa(Dominio.Entidades.Embarcador.Frota.Infracao infracao, Dominio.Entidades.Empresa empresa)
        {
            if (infracao.ResponsavelPagamentoInfracao == Dominio.ObjetosDeValor.Embarcador.Enumeradores.ResponsavelPagamentoInfracao.Condutor)
            {
                Dominio.Entidades.Cliente pessoaMotorista = ObterPessoaMotorista(infracao.Motorista);

                if (pessoaMotorista != null)
                    return pessoaMotorista;

                if (GerarPessoaMotorista(infracao.Motorista, empresa))
                    pessoaMotorista = ObterPessoaMotorista(infracao.Motorista);

                if (pessoaMotorista != null)
                    return pessoaMotorista;
            }
            else if (infracao.ResponsavelPagamentoInfracao == Dominio.ObjetosDeValor.Embarcador.Enumeradores.ResponsavelPagamentoInfracao.Funcionario)
            {
                Dominio.Entidades.Cliente pessoaFuncionario = ObterPessoaMotorista(infracao.Funcionario);

                if (pessoaFuncionario != null)
                    return pessoaFuncionario;

                if (GerarPessoaMotorista(infracao.Funcionario, empresa))
                    pessoaFuncionario = ObterPessoaMotorista(infracao.Funcionario);

                if (pessoaFuncionario != null)
                    return pessoaFuncionario;
            }

            return infracao.Pessoa;
        }

        private Dominio.Entidades.Cliente ObterPessoaMotorista(Dominio.Entidades.Usuario motorista)
        {
            Repositorio.Cliente repositorio = new Repositorio.Cliente(_unitOfWork);

            return repositorio.BuscarPorCPFCNPJ(motorista.CPF.ToDouble());
        }

        private List<Dominio.Entidades.Embarcador.Frota.AlcadasInfracao.RegraAutorizacaoInfracao> ObterRegrasAutorizacao(Dominio.Entidades.Embarcador.Frota.Infracao infracao)
        {
            Repositorio.Embarcador.RegraAutorizacao.RegraAutorizacaoPadrao<Dominio.Entidades.Embarcador.Frota.AlcadasInfracao.RegraAutorizacaoInfracao> repositorioRegra = new Repositorio.Embarcador.RegraAutorizacao.RegraAutorizacaoPadrao<Dominio.Entidades.Embarcador.Frota.AlcadasInfracao.RegraAutorizacaoInfracao>(_unitOfWork);
            List<Dominio.Entidades.Embarcador.Frota.AlcadasInfracao.RegraAutorizacaoInfracao> listaRegras = repositorioRegra.BuscarPorAtiva();
            List<Dominio.Entidades.Embarcador.Frota.AlcadasInfracao.RegraAutorizacaoInfracao> listaRegrasFiltradas = new List<Dominio.Entidades.Embarcador.Frota.AlcadasInfracao.RegraAutorizacaoInfracao>();

            foreach (var regra in listaRegras)
            {
                if (regra.RegraPorTipoInfracao && !ValidarAlcadas<Dominio.Entidades.Embarcador.Frota.AlcadasInfracao.AlcadaTipoInfracao, Dominio.Entidades.Embarcador.Frota.TipoInfracao>(regra.AlcadasTipoInfracao, infracao.TipoInfracao.Codigo))
                    continue;

                //if (regra.RegraPorValor && !ValidarAlcadas<Dominio.Entidades.Embarcador.Frota.AlcadasInfracao.AlcadaValor, decimal>(regra.AlcadasValor, infracao.TipoInfracao.Valor))
                //    continue;

                listaRegrasFiltradas.Add(regra);
            }

            return listaRegrasFiltradas;
        }

        private Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoTitulo ObterTipoTitulo(Dominio.Entidades.Embarcador.Frota.Infracao infracao)
        {
            if (infracao.ResponsavelPagamentoInfracao == Dominio.ObjetosDeValor.Embarcador.Enumeradores.ResponsavelPagamentoInfracao.Outro)
                return infracao.TipoTitulo != null ? infracao.TipoTitulo : Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoTitulo.Pagar;
            else
                return (infracao.ResponsavelPagamentoInfracao == Dominio.ObjetosDeValor.Embarcador.Enumeradores.ResponsavelPagamentoInfracao.Condutor) ? Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoTitulo.Receber : Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoTitulo.Pagar;
        }

        private string ObterObservacaoTitulo(Dominio.Entidades.Embarcador.Frota.Infracao infracao)
        {
            string observacao = $"INFRAÇÃO {infracao.Numero} CONDUTOR {infracao.Motorista?.Nome ?? string.Empty} PLACA {infracao.Veiculo?.Placa ?? string.Empty}";

            if (infracao.DataEmissaoInfracao.HasValue)
                observacao += " DATA " + infracao.DataEmissaoInfracao.Value.ToString("dd/MM/yyyy");

            if (infracao.TipoInfracao.AdicionarRenavamVeiculoObservacao && infracao.Veiculo != null && !string.IsNullOrWhiteSpace(infracao.Veiculo.Renavam))
                observacao += " RENAVAM " + infracao.Veiculo.Renavam;

            observacao += ".";

            return observacao;
        }

        #endregion

        #region Métodos Protegidos Sobrescritos

        protected override void NotificarAprovador(Dominio.Entidades.Embarcador.Frota.Infracao infracao, Dominio.Entidades.Embarcador.Frota.AlcadasInfracao.AprovacaoAlcadaInfracao aprovacao, AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware tipoServicoMultisoftware)
        {
            Notificacao.Notificacao servicoNotificacao = new Notificacao.Notificacao(_unitOfWork.StringConexao, cliente: null, tipoServicoMultisoftware: tipoServicoMultisoftware, adminStringConexao: string.Empty);

            servicoNotificacao.GerarNotificacaoEmail(
                usuario: aprovacao.Usuario,
                usuarioGerouNotificacao: null,
                codigoObjeto: infracao.Codigo,
                URLPagina: "Frota/Infracao",
                titulo: Localization.Resources.Frotas.Infracao.TituloInfracao,
                nota: string.Format(Localization.Resources.Frotas.Infracao.CriadaInfracao, infracao.Numero),
                icone: Dominio.ObjetosDeValor.Embarcador.Enumeradores.IconesNotificacao.cifra,
                tipoNotificacao: Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoNotificacao.credito,
                tipoServicoMultisoftwareNotificar: tipoServicoMultisoftware,
                unitOfWork: _unitOfWork
            );
        }

        #endregion

        #region Métodos Públicos

        public void EtapaAprovacao(Dominio.Entidades.Embarcador.Frota.Infracao infracao, AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware tipoServicoMultisoftware, Dominio.Entidades.Empresa empresa, bool utilizarValorDescontatoComissaoMotoristaInfracao)
        {
            List<Dominio.Entidades.Embarcador.Frota.AlcadasInfracao.RegraAutorizacaoInfracao> regras = ObterRegrasAutorizacao(infracao);

            if (regras.Count > 0)
            {
                CriarRegrasAprovacao(infracao, regras, tipoServicoMultisoftware);

                GerarOperacoesFinalizacao(infracao, tipoServicoMultisoftware, empresa, utilizarValorDescontatoComissaoMotoristaInfracao);
            }
            else
                infracao.Situacao = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoInfracao.SemRegraAprovacao;
        }

        public void GerarOperacoesFinalizacao(Dominio.Entidades.Embarcador.Frota.Infracao infracao, AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware tipoServicoMultisoftware, Dominio.Entidades.Empresa empresa, bool utilizarValorDescontatoComissaoMotoristaInfracao)
        {
            if (infracao.Situacao != Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoInfracao.Finalizada)
                return;

            if (infracao.DescontarLancamentoAgregadoTerceiro)
                GerarVinculoPagamentoAgregado(infracao);
            else if (infracao.Motorista != null && infracao.TipoInfracao.GerarMovimentoFichaMotorista)
                GerarMovimentoFichaMotorista(infracao, tipoServicoMultisoftware, empresa, utilizarValorDescontatoComissaoMotoristaInfracao);
            else if (!infracao.TipoInfracao.NaoGerarTituloFinanceiro)
                GerarTitulo(infracao, tipoServicoMultisoftware, empresa);
        }

        public void GerarTitulo(Dominio.Entidades.Embarcador.Frota.Infracao infracao, AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware tipoServicoMultisoftware, Dominio.Entidades.Empresa empresa)
        {
            try
            {
                Repositorio.Embarcador.Financeiro.Titulo repositorioTitulo = new Repositorio.Embarcador.Financeiro.Titulo(_unitOfWork);
                Repositorio.Embarcador.Frota.Infracao repInfracao = new Repositorio.Embarcador.Frota.Infracao(_unitOfWork);
                Repositorio.Embarcador.Frota.InfracaoParcela repInfracaoParcela = new Repositorio.Embarcador.Frota.InfracaoParcela(_unitOfWork);

                List<Dominio.Entidades.Embarcador.Frota.InfracaoParcela> parcelas = repInfracaoParcela.BuscarPorInfracao(infracao.Codigo);

                if (parcelas.Count > 0 && infracao.InfracaoTitulo.TipoMovimento != null)
                {
                    foreach (Dominio.Entidades.Embarcador.Frota.InfracaoParcela parcela in parcelas)
                    {
                        Dominio.Entidades.Embarcador.Financeiro.Titulo titulo = new Dominio.Entidades.Embarcador.Financeiro.Titulo();

                        titulo.DataEmissao = infracao.InfracaoTitulo.DataCompensacao;
                        titulo.DataVencimento = parcela.DataVencimento;
                        titulo.DataProgramacaoPagamento = parcela.DataVencimento;
                        titulo.FormaTitulo = Dominio.ObjetosDeValor.Embarcador.Enumeradores.FormaTitulo.Outros;
                        titulo.Historico = $"GERADO A PARTIR DA INFRAÇÃO DE NÚMERO {infracao.Numero}, PARCELA {parcela.Parcela}.";
                        titulo.Observacao = ObterObservacaoTitulo(infracao);
                        titulo.NumeroDocumentoTituloOriginal = infracao.NumeroAtuacao;
                        titulo.Pessoa = ObterPessoa(infracao, empresa) ?? throw new ServicoException("Não foi possível encontrar uma pessoa para gerar o título");
                        titulo.GrupoPessoas = titulo.Pessoa.GrupoPessoas;
                        titulo.Sequencia = parcela.Parcela;
                        titulo.StatusTitulo = Dominio.ObjetosDeValor.Embarcador.Enumeradores.StatusTitulo.EmAberto;
                        titulo.DataAlteracao = DateTime.Now;
                        titulo.ValorOriginal = parcela.Valor;
                        titulo.ValorTituloOriginal = parcela.Valor;
                        titulo.ValorPendente = parcela.Valor;
                        titulo.TipoAmbiente = Dominio.Enumeradores.TipoAmbiente.Nenhum;
                        titulo.TipoMovimento = infracao.InfracaoTitulo.TipoMovimento;
                        titulo.TipoTitulo = ObterTipoTitulo(infracao);

                        titulo.DataLancamento = DateTime.Now;
                        titulo.Usuario = infracao.Funcionario;

                        if (infracao.Veiculo != null)
                        {
                            titulo.Veiculos = new List<Dominio.Entidades.Veiculo>()
                            {
                                 infracao.Veiculo
                            };
                        }

                        repositorioTitulo.Inserir(titulo);

                        GerarMovimentacaoTitulo(infracao, titulo, tipoServicoMultisoftware);

                        parcela.Titulo = titulo;

                        repInfracaoParcela.Atualizar(parcela);
                    }
                }
                else if (infracao?.InfracaoTitulo?.TipoMovimento != null)
                {
                    Dominio.Entidades.Embarcador.Financeiro.Titulo titulo = new Dominio.Entidades.Embarcador.Financeiro.Titulo();

                    titulo.DataEmissao = infracao.InfracaoTitulo.DataCompensacao;
                    titulo.DataVencimento = infracao.InfracaoTitulo.DataVencimento;
                    titulo.DataProgramacaoPagamento = infracao.InfracaoTitulo.DataVencimento;
                    titulo.FormaTitulo = Dominio.ObjetosDeValor.Embarcador.Enumeradores.FormaTitulo.Outros;
                    titulo.Historico = $"GERADO A PARTIR DA INFRAÇÃO DE NÚMERO {infracao.Numero}";
                    titulo.Observacao = ObterObservacaoTitulo(infracao);
                    titulo.NumeroDocumentoTituloOriginal = infracao.NumeroAtuacao;
                    titulo.Pessoa = ObterPessoa(infracao, empresa) ?? throw new ServicoException("Não foi possível encontrar uma pessoa para gerar o título");
                    titulo.GrupoPessoas = titulo.Pessoa.GrupoPessoas;
                    titulo.Sequencia = 1;
                    titulo.StatusTitulo = Dominio.ObjetosDeValor.Embarcador.Enumeradores.StatusTitulo.EmAberto;
                    titulo.DataAlteracao = DateTime.Now;
                    titulo.ValorOriginal = infracao.InfracaoTitulo.Valor;
                    titulo.ValorTituloOriginal = titulo.ValorOriginal;
                    titulo.ValorPendente = titulo.ValorOriginal;
                    titulo.TipoAmbiente = Dominio.Enumeradores.TipoAmbiente.Nenhum;
                    titulo.TipoMovimento = infracao.InfracaoTitulo.TipoMovimento;
                    titulo.TipoTitulo = ObterTipoTitulo(infracao);

                    if (infracao.Veiculo != null)
                    {
                        titulo.Veiculos = new List<Dominio.Entidades.Veiculo>()
                        {
                             infracao.Veiculo
                        };
                    }

                    repositorioTitulo.Inserir(titulo);

                    GerarMovimentacaoTitulo(infracao, titulo, tipoServicoMultisoftware);

                    Dominio.Entidades.Embarcador.Frota.InfracaoParcela parcela = new Dominio.Entidades.Embarcador.Frota.InfracaoParcela()
                    {
                        DataVencimento = infracao.InfracaoTitulo.DataVencimento.HasValue ? infracao.InfracaoTitulo.DataVencimento.Value : DateTime.Now,
                        Infracao = infracao,
                        Parcela = 1,
                        Valor = infracao.InfracaoTitulo.Valor,
                        ValorAposVencimento = infracao.InfracaoTitulo.ValorAposVencimento,
                        Titulo = titulo
                    };

                    repInfracaoParcela.Inserir(parcela);
                }

                if (infracao.ResponsavelPagamentoInfracao != Dominio.ObjetosDeValor.Embarcador.Enumeradores.ResponsavelPagamentoInfracao.Empresa && infracao.GerarTituloEmpresa && infracao.TipoMovimentoEmpresa != null && infracao.PessoaTituloEmpresa != null)
                {
                    Dominio.Entidades.Embarcador.Financeiro.Titulo tituloEmpresa = new Dominio.Entidades.Embarcador.Financeiro.Titulo();

                    tituloEmpresa.DataEmissao = infracao.InfracaoTitulo.DataCompensacao;
                    tituloEmpresa.DataVencimento = infracao.InfracaoTitulo.DataVencimento;
                    tituloEmpresa.DataProgramacaoPagamento = infracao.InfracaoTitulo.DataVencimento;
                    tituloEmpresa.FormaTitulo = Dominio.ObjetosDeValor.Embarcador.Enumeradores.FormaTitulo.Outros;
                    tituloEmpresa.Historico = $"GERADO A PARTIR DA INFRAÇÃO DE NÚMERO {infracao.Numero}";
                    tituloEmpresa.Observacao = ObterObservacaoTitulo(infracao);
                    tituloEmpresa.NumeroDocumentoTituloOriginal = infracao.NumeroAtuacao;
                    tituloEmpresa.Pessoa = infracao.PessoaTituloEmpresa;
                    tituloEmpresa.GrupoPessoas = tituloEmpresa.Pessoa.GrupoPessoas;
                    tituloEmpresa.Sequencia = 1;
                    tituloEmpresa.StatusTitulo = Dominio.ObjetosDeValor.Embarcador.Enumeradores.StatusTitulo.EmAberto;
                    tituloEmpresa.DataAlteracao = DateTime.Now;
                    tituloEmpresa.ValorOriginal = infracao.InfracaoTitulo.Valor;
                    tituloEmpresa.ValorTituloOriginal = tituloEmpresa.ValorOriginal;
                    tituloEmpresa.ValorPendente = tituloEmpresa.ValorOriginal;
                    tituloEmpresa.TipoAmbiente = Dominio.Enumeradores.TipoAmbiente.Nenhum;
                    tituloEmpresa.TipoMovimento = infracao.TipoMovimentoEmpresa;
                    tituloEmpresa.TipoTitulo = Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoTitulo.Pagar;

                    tituloEmpresa.DataLancamento = DateTime.Now;
                    tituloEmpresa.Usuario = infracao.Funcionario;

                    if (infracao.Veiculo != null)
                    {
                        tituloEmpresa.Veiculos = new List<Dominio.Entidades.Veiculo>()
                        {
                             infracao.Veiculo
                        };
                    }

                    repositorioTitulo.Inserir(tituloEmpresa);

                    GerarMovimentacaoTitulo(infracao, tituloEmpresa, tipoServicoMultisoftware);

                    infracao.TituloEmpresa = tituloEmpresa;
                }

                repInfracao.Atualizar(infracao);
            }
            catch (Exception excecao)
            {
                Log.TratarErro(excecao);
            }
        }

        public void GerarTituloEmpresa(Dominio.Entidades.Embarcador.Frota.Infracao infracao, AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware tipoServicoMultisoftware, Dominio.Entidades.Empresa empresa, bool utilizarValorDescontatoComissaoMotoristaInfracao)
        {
            try
            {
                Repositorio.Embarcador.Financeiro.Titulo repositorioTitulo = new Repositorio.Embarcador.Financeiro.Titulo(_unitOfWork);
                Repositorio.Embarcador.Frota.Infracao repInfracao = new Repositorio.Embarcador.Frota.Infracao(_unitOfWork);
                Repositorio.Embarcador.Frota.InfracaoTituloEmpresa repInfracaoTituloEmpresa = new Repositorio.Embarcador.Frota.InfracaoTituloEmpresa(_unitOfWork);

                Financeiro.ProcessoMovimento servicoProcessoMovimento = new Financeiro.ProcessoMovimento();

                List<Dominio.Entidades.Embarcador.Frota.InfracaoTituloEmpresa> parcelas = repInfracaoTituloEmpresa.BuscarPorInfracao(infracao.Codigo);
                decimal valorTotalTitulo = 0;
                DateTime? dataVencimentoTitulo = null;
                if (parcelas.Count > 0)
                {
                    valorTotalTitulo = parcelas.Sum(o => o.Valor);

                    foreach (Dominio.Entidades.Embarcador.Frota.InfracaoTituloEmpresa parcela in parcelas)
                    {
                        if (parcela.TipoMovimento != null)
                        {
                            Dominio.Entidades.Embarcador.Financeiro.Titulo titulo = new Dominio.Entidades.Embarcador.Financeiro.Titulo();

                            titulo.DataEmissao = infracao.Data;
                            titulo.DataVencimento = parcela.DataVencimento;
                            titulo.DataProgramacaoPagamento = parcela.DataVencimento;
                            titulo.NossoNumero = parcela.CodigoBarras;
                            titulo.TipoDocumentoTituloOriginal = "INFRAÇÃO";
                            titulo.FormaTitulo = Dominio.ObjetosDeValor.Embarcador.Enumeradores.FormaTitulo.Outros;
                            titulo.Historico = $"GERADO A PARTIR DA INFRAÇÃO DE NÚMERO {infracao.Numero}, PARCELA {parcela.Parcela}.";
                            titulo.Observacao = ObterObservacaoTitulo(infracao);
                            titulo.NumeroDocumentoTituloOriginal = infracao.NumeroAtuacao;
                            titulo.Pessoa = parcela.Pessoa;
                            titulo.GrupoPessoas = titulo.Pessoa.GrupoPessoas;
                            titulo.Sequencia = parcela.Parcela;
                            titulo.StatusTitulo = Dominio.ObjetosDeValor.Embarcador.Enumeradores.StatusTitulo.EmAberto;
                            titulo.DataAlteracao = DateTime.Now;
                            titulo.ValorOriginal = parcela.Valor;
                            titulo.ValorTituloOriginal = parcela.Valor;
                            titulo.ValorPendente = parcela.Valor;
                            titulo.TipoAmbiente = Dominio.Enumeradores.TipoAmbiente.Nenhum;
                            titulo.TipoMovimento = parcela.TipoMovimento;
                            titulo.Empresa = parcela.Empresa;
                            titulo.TipoTitulo = Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoTitulo.Pagar;

                            titulo.DataLancamento = DateTime.Now;
                            titulo.Usuario = infracao.Funcionario;

                            if (infracao.Veiculo != null)
                            {
                                titulo.Veiculos = new List<Dominio.Entidades.Veiculo>()
                            {
                                 infracao.Veiculo
                            };
                            }
                            dataVencimentoTitulo = titulo.DataVencimento;
                            repositorioTitulo.Inserir(titulo);

                            GerarMovimentacaoTitulo(infracao, titulo, tipoServicoMultisoftware);

                            parcela.Titulo = titulo;

                            repInfracaoTituloEmpresa.Atualizar(parcela);
                        }
                    }
                }
                if (utilizarValorDescontatoComissaoMotoristaInfracao)
                    valorTotalTitulo = infracao.DescontoComissaoMotorista;

                if (valorTotalTitulo > 0 && infracao.ResponsavelPagamentoInfracao == Dominio.ObjetosDeValor.Embarcador.Enumeradores.ResponsavelPagamentoInfracao.Condutor && infracao.TipoInfracao != null && infracao.TipoInfracao.GerarMovimentoFichaMotorista == true && infracao.TipoInfracao.TipoMovimentoFichaMotorista != null)
                {
                    servicoProcessoMovimento.GerarMovimentacao(
                            infracao.TipoInfracao.TipoMovimentoFichaMotorista,
                            (dataVencimentoTitulo.HasValue && dataVencimentoTitulo.Value > DateTime.MinValue ? dataVencimentoTitulo.Value : infracao.Data),
                            valorTotalTitulo,
                            infracao.Numero.ToString(),
                            $"GERADO A PARTIR DA INFRAÇÃO DE NÚMERO {infracao.Numero}",
                            _unitOfWork,
                            Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoDocumentoMovimento.Outros,
                            tipoServicoMultisoftware,
                            infracao.Motorista.Codigo,
                            null,
                            null,
                            0,
                            Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoMovimentoEntidade.Saida
                        );
                }
            }
            catch (Exception excecao)
            {
                Log.TratarErro(excecao);
            }
        }

        public void GerarMovimentoFichaMotorista(Dominio.Entidades.Embarcador.Frota.Infracao infracao, AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware tipoServicoMultisoftware, Dominio.Entidades.Empresa empresa, bool utilizarValorDescontatoComissaoMotoristaInfracao)
        {
            try
            {
                Repositorio.Embarcador.Financeiro.Titulo repositorioTitulo = new Repositorio.Embarcador.Financeiro.Titulo(_unitOfWork);
                Repositorio.Embarcador.Frota.Infracao repInfracao = new Repositorio.Embarcador.Frota.Infracao(_unitOfWork);
                Repositorio.Embarcador.Frota.InfracaoParcela repInfracaoParcela = new Repositorio.Embarcador.Frota.InfracaoParcela(_unitOfWork);
                Financeiro.ProcessoMovimento servicoProcessoMovimento = new Financeiro.ProcessoMovimento();

                List<Dominio.Entidades.Embarcador.Frota.InfracaoParcela> parcelas = repInfracaoParcela.BuscarPorInfracao(infracao.Codigo);

                if (utilizarValorDescontatoComissaoMotoristaInfracao && infracao.InfracaoTitulo != null && infracao.InfracaoTitulo.TipoMovimento != null)
                {
                    servicoProcessoMovimento.GerarMovimentacao(
                            infracao.InfracaoTitulo.TipoMovimento,
                            infracao.InfracaoTitulo.DataCompensacao.HasValue ? infracao.InfracaoTitulo.DataCompensacao.Value : DateTime.Now.Date,
                            infracao.DescontoComissaoMotorista,
                            infracao.Numero.ToString(),
                            $"GERADO A PARTIR DA INFRAÇÃO DE NÚMERO {infracao.Numero}",
                            _unitOfWork,
                            Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoDocumentoMovimento.Outros,
                            tipoServicoMultisoftware,
                            infracao.Motorista.Codigo,
                            null,
                            null,
                            0,
                            Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoMovimentoEntidade.Saida
                        );
                }
                else if (parcelas.Count > 0 && infracao.InfracaoTitulo != null && infracao.InfracaoTitulo.TipoMovimento != null)
                {
                    foreach (Dominio.Entidades.Embarcador.Frota.InfracaoParcela parcela in parcelas)
                    {
                        servicoProcessoMovimento.GerarMovimentacao(
                            infracao.InfracaoTitulo.TipoMovimento,
                            infracao.InfracaoTitulo.DataCompensacao.HasValue ? infracao.InfracaoTitulo.DataCompensacao.Value : DateTime.Now.Date,
                            parcela.Valor,
                            infracao.Numero.ToString(),
                            $"GERADO A PARTIR DA INFRAÇÃO DE NÚMERO {infracao.Numero}, PARCELA {parcela.Parcela}.",
                            _unitOfWork,
                            Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoDocumentoMovimento.Outros,
                            tipoServicoMultisoftware,
                            infracao.Motorista.Codigo,
                            null,
                            null,
                            0,
                            Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoMovimentoEntidade.Saida
                        );
                    }
                }
                else if (infracao.InfracaoTitulo != null && infracao.InfracaoTitulo.TipoMovimento != null)
                {
                    servicoProcessoMovimento.GerarMovimentacao(
                            infracao.InfracaoTitulo.TipoMovimento,
                            infracao.InfracaoTitulo.DataCompensacao.HasValue ? infracao.InfracaoTitulo.DataCompensacao.Value : DateTime.Now.Date,
                            infracao.InfracaoTitulo.Valor,
                            infracao.Numero.ToString(),
                            $"GERADO A PARTIR DA INFRAÇÃO DE NÚMERO {infracao.Numero}",
                            _unitOfWork,
                            Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoDocumentoMovimento.Outros,
                            tipoServicoMultisoftware,
                            infracao.Motorista.Codigo,
                            null,
                            null,
                            0,
                            Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoMovimentoEntidade.Saida
                        );
                }

                if (infracao.InfracaoTitulo != null && infracao.ResponsavelPagamentoInfracao != Dominio.ObjetosDeValor.Embarcador.Enumeradores.ResponsavelPagamentoInfracao.Empresa && infracao.GerarTituloEmpresa && infracao.TipoMovimentoEmpresa != null && infracao.PessoaTituloEmpresa != null)
                {
                    Dominio.Entidades.Embarcador.Financeiro.Titulo tituloEmpresa = new Dominio.Entidades.Embarcador.Financeiro.Titulo();

                    tituloEmpresa.DataEmissao = infracao.InfracaoTitulo.DataCompensacao;
                    tituloEmpresa.DataVencimento = infracao.InfracaoTitulo.DataVencimento;
                    tituloEmpresa.DataProgramacaoPagamento = infracao.InfracaoTitulo.DataVencimento;
                    tituloEmpresa.FormaTitulo = Dominio.ObjetosDeValor.Embarcador.Enumeradores.FormaTitulo.Outros;
                    tituloEmpresa.Historico = $"GERADO A PARTIR DA INFRAÇÃO DE NÚMERO {infracao.Numero}";
                    tituloEmpresa.NumeroDocumentoTituloOriginal = infracao.Numero.ToString();
                    tituloEmpresa.Pessoa = infracao.PessoaTituloEmpresa;
                    tituloEmpresa.GrupoPessoas = tituloEmpresa.Pessoa.GrupoPessoas;
                    tituloEmpresa.Sequencia = 1;
                    tituloEmpresa.StatusTitulo = Dominio.ObjetosDeValor.Embarcador.Enumeradores.StatusTitulo.EmAberto;
                    tituloEmpresa.DataAlteracao = DateTime.Now;
                    tituloEmpresa.ValorOriginal = infracao.InfracaoTitulo.Valor;
                    tituloEmpresa.ValorTituloOriginal = tituloEmpresa.ValorOriginal;
                    tituloEmpresa.ValorPendente = tituloEmpresa.ValorOriginal;
                    tituloEmpresa.TipoAmbiente = Dominio.Enumeradores.TipoAmbiente.Nenhum;
                    tituloEmpresa.TipoMovimento = infracao.TipoMovimentoEmpresa;
                    tituloEmpresa.TipoTitulo = Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoTitulo.Pagar;

                    tituloEmpresa.DataLancamento = DateTime.Now;
                    tituloEmpresa.Usuario = infracao.Funcionario;

                    if (infracao.Veiculo != null)
                    {
                        tituloEmpresa.Veiculos = new List<Dominio.Entidades.Veiculo>()
                        {
                             infracao.Veiculo
                        };
                    }

                    repositorioTitulo.Inserir(tituloEmpresa);

                    GerarMovimentacaoTitulo(infracao, tituloEmpresa, tipoServicoMultisoftware);

                    infracao.TituloEmpresa = tituloEmpresa;
                }

                repInfracao.Atualizar(infracao);
            }
            catch (Exception excecao)
            {
                Log.TratarErro(excecao);
            }
        }

        public void GerarVinculoPagamentoAgregado(Dominio.Entidades.Embarcador.Frota.Infracao infracao)
        {
            Repositorio.Embarcador.Frota.InfracaoParcela repInfracaoParcela = new Repositorio.Embarcador.Frota.InfracaoParcela(_unitOfWork);
            Repositorio.Embarcador.PagamentoAgregado.PagamentoAgregado repositorioPagamentoAgregado = new Repositorio.Embarcador.PagamentoAgregado.PagamentoAgregado(_unitOfWork);
            Repositorio.Embarcador.PagamentoAgregado.PagamentoAgregadoAcrescimoDesconto repositorioPagamentoAgregadoAcrescimoDesconto = new Repositorio.Embarcador.PagamentoAgregado.PagamentoAgregadoAcrescimoDesconto(_unitOfWork);
            Repositorio.Embarcador.PagamentoAgregado.PagamentoAgregadoInfracaoParcela repositorioPagamentoAgregadoInfracaoParcela = new Repositorio.Embarcador.PagamentoAgregado.PagamentoAgregadoInfracaoParcela(_unitOfWork);
            Repositorio.Embarcador.Fatura.Justificativa repositorioJustificativa = new Repositorio.Embarcador.Fatura.Justificativa(_unitOfWork);

            List<Dominio.Entidades.Embarcador.Frota.InfracaoParcela> parcelas = repInfracaoParcela.BuscarPorInfracao(infracao.Codigo);

            foreach (Dominio.Entidades.Embarcador.Frota.InfracaoParcela parcela in parcelas)
            {
                if (infracao.InfracaoTitulo == null || infracao.InfracaoTitulo.TipoMovimento == null)
                    continue;

                Dominio.Entidades.Embarcador.PagamentoAgregado.PagamentoAgregado pagamentoAgregado = repositorioPagamentoAgregado.BuscarPagamentoAbertoPorPessoaData(infracao.Pessoa.CPF_CNPJ, parcela.DataVencimento);
                if (pagamentoAgregado == null)
                    continue;

                Dominio.Entidades.Embarcador.Fatura.Justificativa justificativa = repositorioJustificativa.BuscarPorTipoMovimento(infracao.InfracaoTitulo.TipoMovimento.Codigo, Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoJustificativa.Desconto);
                if (justificativa == null)
                    continue;

                Dominio.Entidades.Embarcador.PagamentoAgregado.PagamentoAgregadoAcrescimoDesconto desconto = new Dominio.Entidades.Embarcador.PagamentoAgregado.PagamentoAgregadoAcrescimoDesconto();
                desconto.Justificativa = justificativa;
                desconto.Valor = parcela.Valor;
                desconto.PagamentoAgregado = pagamentoAgregado;
                repositorioPagamentoAgregadoAcrescimoDesconto.Inserir(desconto);

                Dominio.Entidades.Embarcador.PagamentoAgregado.PagamentoAgregadoInfracaoParcela infracaoParcela = new Dominio.Entidades.Embarcador.PagamentoAgregado.PagamentoAgregadoInfracaoParcela();
                infracaoParcela.InfracaoParcela = parcela;
                infracaoParcela.PagamentoAgregado = pagamentoAgregado;
                repositorioPagamentoAgregadoInfracaoParcela.Inserir(infracaoParcela);
            }
        }

        #endregion
    }
}
