using Dominio.Excecoes.Embarcador;
using System;
using System.Collections.Generic;
using Dominio.ObjetosDeValor.Embarcador.Enumeradores;
using System.Linq;

namespace Servicos.Embarcador.Financeiro
{
    public class TituloImportacao
    {
        #region Atributos Privados Somente Leitura

        private readonly Dictionary<string, dynamic> _dados;
        private readonly AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware _tipoServicoMultisoftware;
        private readonly Repositorio.UnitOfWork _unitOfWork;
        private readonly Dominio.Entidades.Empresa _empresa;
        private readonly Dominio.Entidades.Embarcador.Configuracao.ConfiguracaoTMS _configuracao;
        private readonly Dominio.Entidades.Embarcador.Configuracoes.ConfiguracaoFinanceiro _configuracaoFinanceiro;

        #endregion

        #region Construtores

        public TituloImportacao(Repositorio.UnitOfWork unitOfWork, AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware tipoServicoMultisoftware, Dominio.Entidades.Empresa empresa, Dictionary<string, dynamic> dados, Dominio.Entidades.Embarcador.Configuracao.ConfiguracaoTMS configuracao, Dominio.Entidades.Embarcador.Configuracoes.ConfiguracaoFinanceiro configuracaoFinanceiro)
        {
            _dados = dados;
            _tipoServicoMultisoftware = tipoServicoMultisoftware;
            _unitOfWork = unitOfWork;
            _empresa = empresa;
            _configuracao = configuracao;
            _configuracaoFinanceiro = configuracaoFinanceiro;
        }

        #endregion

        #region Métodos Privados

        private Dominio.Entidades.Cliente ObterPessoa()
        {
            string pessoaBuscar = string.Empty;

            if (_dados.TryGetValue("CnpjCpfPessoa", out var codigoPessoa))
                pessoaBuscar = Utilidades.String.OnlyNumbers((string)codigoPessoa);

            if (string.IsNullOrWhiteSpace(pessoaBuscar))
                throw new ImportacaoException("Pessoa não informada");

            Repositorio.Cliente repPessoa = new Repositorio.Cliente(_unitOfWork);
            Dominio.Entidades.Cliente pessoa = repPessoa.BuscarPorCPFCNPJ(pessoaBuscar.ToDouble());

            if (pessoa == null)
                throw new ImportacaoException("Pessoa não cadastrada no sistema! Favor fazer esse cadastro antes de prosseguir com a importação");

            if (!pessoa.Ativo)
                throw new ImportacaoException("Pessoa Inativa. Ative a pessoa ou selecione uma Pessoa ativa para realizar a Importação");

            return pessoa;
        }

        private Dominio.Entidades.Cliente ObterMotorista()
        {
            string pessoaBuscar = string.Empty;

            if (_dados.TryGetValue("CnpjCpfPessoa", out var codigoPessoa))
                pessoaBuscar = Utilidades.String.OnlyNumbers((string)codigoPessoa);

            if (string.IsNullOrWhiteSpace(pessoaBuscar))
                throw new ImportacaoException("Motorista não informado");

            Repositorio.Cliente repPessoa = new Repositorio.Cliente(_unitOfWork);
            Dominio.Entidades.Cliente pessoa = repPessoa.BuscarPorCPFCNPJ(pessoaBuscar.ToDouble());

            if (pessoa == null)
            {
                Repositorio.Usuario repUsuario = new Repositorio.Usuario(_unitOfWork);
                Dominio.Entidades.Usuario usuario = repUsuario.BuscarPorCPF(pessoaBuscar);
                if (usuario == null)
                    throw new ImportacaoException("Motorista não cadastrado no sistema! Favor fazer esse cadastro antes de prosseguir com a importação");

                if (usuario.Localidade == null)
                    throw new ImportacaoException("Motorista cadastrado no sistema está com endereço incompleto! Favor ajustar o cadastro antes de prosseguir com a importação");

                pessoa = Servicos.Embarcador.Pessoa.Pessoa.ConverterFuncionario(usuario, _unitOfWork);
                repPessoa.Inserir(pessoa);
                new Repositorio.Embarcador.Pessoas.PessoaIntegracao(_unitOfWork).GerarIntegracaoPessoa(_unitOfWork, pessoa);
            }

            return pessoa;
        }

        private TipoTitulo? ObterTipoTitulo()
        {
            TipoTitulo? tipoTituloRetornar = null;

            if (_dados.TryGetValue("TipoTitulo", out var tipo))
                tipoTituloRetornar = ((string)tipo).ToNullableEnum<TipoTitulo>();

            if (tipoTituloRetornar == null)
                throw new ImportacaoException("Tipo do Título não informado");

            return tipoTituloRetornar;
        }

        private DateTime? ObterDataEmissao()
        {
            DateTime? data = null;
            if (_dados.TryGetValue("DataEmissao", out var dataEmissao))
                data = ((string)dataEmissao).ToNullableDateTime();

            if (data == null)
                data = DateTime.Now;

            return data;
        }

        private DateTime? ObterDataCompetencia()
        {
            DateTime? data = null;
            if (_dados.TryGetValue("DataEmissao", out var dataEmissao))
                data = ((string)dataEmissao).ToNullableDateTime();

            if (data == null)
                data = DateTime.Now;

            return data;
        }

        private DateTime? ObterDataVencimento()
        {
            DateTime? data = null;
            if (_dados.TryGetValue("DataVencimento", out var dataVencimento))
                data = ((string)dataVencimento).ToNullableDateTime();

            if (data == null)
                throw new ImportacaoException("Data Vencimento não informada");

            return data;
        }

        private decimal ObterValorOriginal()
        {
            decimal valor = 0;
            if (_dados.TryGetValue("ValorOriginal", out var valorOriginal))
                valor = ((string)valorOriginal).ToDecimal();

            if (valor == 0)
                throw new ImportacaoException("Valor Original não informado");

            return valor;
        }

        private Dominio.Entidades.Embarcador.Financeiro.TipoMovimento ObterTipoMovimento()
        {
            int tipoMovimentoBuscar = 0;

            if (_dados.TryGetValue("TipoMovimento", out var codigoTipoMovimento))
                tipoMovimentoBuscar = ((string)codigoTipoMovimento).ToInt();

            if (tipoMovimentoBuscar == 0)
                throw new ImportacaoException("Tipo Movimento não informado");

            Repositorio.Embarcador.Financeiro.TipoMovimento repTipoMovimento = new Repositorio.Embarcador.Financeiro.TipoMovimento(_unitOfWork);
            Dominio.Entidades.Embarcador.Financeiro.TipoMovimento tipoMovimento = repTipoMovimento.BuscarPorCodigoEEmpresa(tipoMovimentoBuscar, _empresa?.Codigo ?? 0);

            if (tipoMovimento == null)
                throw new ImportacaoException("Tipo Movimento não encontrado");

            return tipoMovimento;
        }

        private int ObterSequencia()
        {
            int sequencia = 0;
            if (_dados.TryGetValue("Sequencia", out var codigoSequencia))
                sequencia = ((string)codigoSequencia).ToInt();

            if (sequencia == 0)
                sequencia = 1;

            return sequencia;
        }

        private decimal ObterValorDesconto()
        {
            decimal valor = 0;
            if (_dados.TryGetValue("ValorDesconto", out var valorDesconto))
                valor = ((string)valorDesconto).ToDecimal();

            return valor;
        }

        private decimal ObterValorAcrescimo()
        {
            decimal valor = 0;
            if (_dados.TryGetValue("ValorAcrescimo", out var valorAcrescimo))
                valor = ((string)valorAcrescimo).ToDecimal();

            return valor;
        }

        private string ObterTipoDocumento()
        {
            var tipoDocumentoRetornar = string.Empty;

            if (_dados.TryGetValue("TipoDocumento", out var tipoDocumento))
                tipoDocumentoRetornar = ((string)tipoDocumento).Trim();

            return tipoDocumentoRetornar;
        }

        private string ObterNumeroDocumento()
        {
            var numeroDocumentoRetornar = string.Empty;

            if (_dados.TryGetValue("NumeroDocumento", out var numeroDocumento))
                numeroDocumentoRetornar = ((string)numeroDocumento).Trim();

            if (_configuracao.ExigirNumeroDocumentoTituloFinanceiro && string.IsNullOrWhiteSpace(numeroDocumentoRetornar))
                throw new ImportacaoException("É obrigatório informar o Número do Documento");

            return numeroDocumentoRetornar;
        }

        private string ObterObservacao()
        {
            var observacaoRetornar = string.Empty;

            if (_dados.TryGetValue("Observacao", out var observacao))
                observacaoRetornar = ((string)observacao).Trim();

            return observacaoRetornar;
        }

        private Dominio.Entidades.Empresa ObterEmpresa()
        {
            Repositorio.Empresa repositorioEmpresa = new Repositorio.Empresa(_unitOfWork);

            _dados.TryGetValue("Empresa", out var cnpjEmpresaRecebido);

            string cnpjEmpresa = Utilidades.String.OnlyNumbers((string)cnpjEmpresaRecebido);

            if (string.IsNullOrWhiteSpace(cnpjEmpresa))
                return null;

            return repositorioEmpresa.BuscarPorCNPJ((string)cnpjEmpresa);
        }

        private Dominio.Entidades.Embarcador.Financeiro.LancamentoCentroResultado ObterLancamentoCentroResultado(Dominio.Entidades.Embarcador.Financeiro.Titulo titulo)
        {
            string centroResultadoBuscar = "";

            if (_dados.TryGetValue("CentroResultado", out var codigoCentroResultado))
                centroResultadoBuscar = ((string)codigoCentroResultado);

            if (string.IsNullOrWhiteSpace(centroResultadoBuscar))
                return null;

            Repositorio.Embarcador.Financeiro.CentroResultado repCentroResultado = new Repositorio.Embarcador.Financeiro.CentroResultado(_unitOfWork);
            Dominio.Entidades.Embarcador.Financeiro.CentroResultado centroResultado = repCentroResultado.BuscarPorPlanoEEmpresa(centroResultadoBuscar, _empresa?.Codigo ?? 0);

            if (centroResultado != null)
            {
                Dominio.Entidades.Embarcador.Financeiro.LancamentoCentroResultado lancamentoCentroResultado = new Dominio.Entidades.Embarcador.Financeiro.LancamentoCentroResultado();
                lancamentoCentroResultado.TipoDocumento = TipoDocumentoLancamentoCentroResultado.Titulo;
                lancamentoCentroResultado.Ativo = true;
                lancamentoCentroResultado.Data = DateTime.Now;
                lancamentoCentroResultado.Titulo = titulo;
                lancamentoCentroResultado.CentroResultado = centroResultado;
                lancamentoCentroResultado.Percentual = 100;
                lancamentoCentroResultado.Valor = titulo.ValorTotal;
                return lancamentoCentroResultado;
            }

            return null;
        }

        private List<Dominio.Entidades.Veiculo> ObterVeiculos()
        {
            string veiculoBuscar = "";

            if (_dados.TryGetValue("Veiculo", out var placaVeiculo))
                veiculoBuscar = placaVeiculo;

            if (string.IsNullOrWhiteSpace(veiculoBuscar))
                return null;

            List<string> listaVeiculos = veiculoBuscar.Split(';').ToList();
            List<string> listaPlacasValidas = new List<string>();
            foreach (string veiculoLista in listaVeiculos)
            {
                if (Utilidades.Validate.ValidarPlaca(veiculoLista.ToString().ToUpper()))
                    listaPlacasValidas.Add(veiculoLista.ToUpper());
            }

            Repositorio.Veiculo repVeiculo = new Repositorio.Veiculo(_unitOfWork);
            List<Dominio.Entidades.Veiculo> veiculos = repVeiculo.BuscarPorPlacas(listaPlacasValidas);

            return veiculos;
        }

        private FormaTitulo ObterFormaTitulo()
        {
            string formaTituloRetornar = string.Empty;

            if (_dados.TryGetValue("FormaTitulo", out var formaTituloConverte))
                formaTituloRetornar = (string)formaTituloConverte;
            formaTituloRetornar = !string.IsNullOrWhiteSpace(formaTituloRetornar) ? formaTituloRetornar.Trim() : string.Empty;

            return FormaTituloHelper.ObterFormaTitulo(formaTituloRetornar);
        }

        private Dominio.Entidades.Cliente ObterPortador()
        {
            string pessoaBuscar = string.Empty;

            if (_dados.TryGetValue("CnpjCpfPortador", out var codigoPessoa))
                pessoaBuscar = Utilidades.String.OnlyNumbers((string)codigoPessoa);

            if (string.IsNullOrWhiteSpace(pessoaBuscar))
                return null;

            Repositorio.Cliente repPessoa = new Repositorio.Cliente(_unitOfWork);
            Dominio.Entidades.Cliente pessoa = repPessoa.BuscarPorCPFCNPJ(pessoaBuscar.ToDouble());

            if (pessoa == null)
                throw new ImportacaoException("Portador não cadastrado no sistema! Favor fazer esse cadastro antes de prosseguir com a importação");

            return pessoa;
        }

        private string ObterNossoNumero(Dominio.Entidades.Embarcador.Financeiro.Titulo titulo)
        {
            var nossoNumeroRetornar = string.Empty;

            if (_dados.TryGetValue("NossoNumero", out var nossoNumero))
                nossoNumeroRetornar = ((string)nossoNumero).Trim();

            if (!_configuracao.NaoValidarCodigoBarrasBoletoTituloAPagar && titulo.TipoTitulo == TipoTitulo.Pagar &&
                (!string.IsNullOrWhiteSpace(nossoNumero) && Utilidades.String.OnlyNumbers(nossoNumero).Length != 44))
                throw new ImportacaoException("O código de barras do boleto deve ter 44 dígitos.");

            if (titulo.TipoTitulo == TipoTitulo.Receber)
                return null;

            return nossoNumeroRetornar;
        }

        private bool ObterProvisao()
        {
            string tituloProvisaoString = string.Empty;

            if (_dados.TryGetValue("Provisao", out var provisao))
                tituloProvisaoString = (string)provisao;

            if (string.IsNullOrWhiteSpace(tituloProvisaoString))
                return false;

            return tituloProvisaoString.ToUpper() == "SIM" || tituloProvisaoString.ToUpper() == "S" || tituloProvisaoString.ToUpper() == "Y";
        }

        #endregion

        #region Métodos Validação

        private void ValidarDados(Dominio.Entidades.Embarcador.Financeiro.Titulo titulo)
        {
            Repositorio.Embarcador.Financeiro.Titulo repTitulo = new Repositorio.Embarcador.Financeiro.Titulo(_unitOfWork);
            Repositorio.Embarcador.Financeiro.FechamentoDiario repFechamentoDiario = new Repositorio.Embarcador.Financeiro.FechamentoDiario(_unitOfWork);

            if (repFechamentoDiario.VerificarSeExistePorDataFechamento(titulo?.Empresa?.Codigo ?? 0, titulo.DataEmissao.Value))
                throw new ImportacaoException("Já existe um fechamento diário igual ou posterior à data " + titulo.DataEmissao.Value.ToString("dd/MM/yyyy") + ", não sendo possível realizar o lançamento do título.");

            if (titulo.DataEmissao > titulo.DataVencimento)
                throw new ImportacaoException("A data de emissão não pode ser maior que a data de vencimento");

            if (_tipoServicoMultisoftware != AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiNFe)
            {
                if (titulo.TipoMovimento == null)
                    throw new ImportacaoException("Favor informe o tipo de movimento para o lançamento do título.");

                if (_configuracaoFinanceiro.ValidarDuplicidadeTituloSemData)
                {
                    if (repTitulo.ContemTituloDuplicado(0, titulo.Pessoa.CPF_CNPJ, titulo.TipoTitulo, titulo.Sequencia, titulo.TipoDocumentoTituloOriginal, titulo.NumeroDocumentoTituloOriginal))
                        throw new ImportacaoException("Já existe um título lançado com a mesma Pessoa, Tipo, Sequência, Tipo de Documento e Número do Documento.");
                }
                else
                {
                    if (repTitulo.ContemTituloDuplicado(titulo.DataEmissao.Value, titulo.DataVencimento.Value, titulo.Pessoa.CPF_CNPJ, titulo.ValorOriginal, titulo.TipoTitulo, 0, titulo.NumeroDocumentoTituloOriginal))
                        throw new ImportacaoException("Já existe um título lançado com a mesma Data de Emissão, Vencimento, Valor, Número do Documento e Pessoa.");
                }

                if (_configuracaoFinanceiro.QuantidadeDiasLimiteVencimentoTitulo > 0)
                {
                    DateTime dataLimiteVencimento = DateTime.Now.Date.AddDays(_configuracaoFinanceiro.QuantidadeDiasLimiteVencimentoTitulo);
                    int result = DateTime.Compare(titulo.DataVencimento.Value, dataLimiteVencimento);

                    if (result > 0)
                        throw new ImportacaoException($"A data {titulo.DataVencimento.Value.ToDateString()} é maior que a data limite estipulada nas configurações.");
                }
            }
        }

        #endregion

        #region Métodos Públicos

        public Dominio.Entidades.Embarcador.Financeiro.Titulo ObterTituloImportar(Dominio.Entidades.Usuario usuario)
        {
            Dominio.Entidades.Embarcador.Financeiro.Titulo titulo = new Dominio.Entidades.Embarcador.Financeiro.Titulo();
            titulo.Initialize();

            titulo.Pessoa = ObterPessoa();
            titulo.GrupoPessoas = titulo.Pessoa.GrupoPessoas;
            titulo.TipoTitulo = ObterTipoTitulo().Value;
            titulo.DataEmissao = ObterDataEmissao();
            titulo.DataVencimento = ObterDataVencimento();
            titulo.DataProgramacaoPagamento = ObterDataVencimento();
            titulo.ValorOriginal = ObterValorOriginal();
            titulo.TipoMovimento = ObterTipoMovimento();

            titulo.Sequencia = ObterSequencia();
            titulo.Desconto = ObterValorDesconto();
            titulo.Acrescimo = ObterValorAcrescimo();
            titulo.TipoDocumentoTituloOriginal = ObterTipoDocumento();
            titulo.NumeroDocumentoTituloOriginal = ObterNumeroDocumento();
            titulo.Observacao = ObterObservacao();

            titulo.Valor = titulo.ValorOriginal;
            titulo.ValorTotal = titulo.ValorOriginal;
            titulo.ValorPendente = titulo.ValorOriginal;
            titulo.StatusTitulo = StatusTitulo.EmAberto;
            titulo.DataAlteracao = DateTime.Now;
            titulo.FormaTitulo = ObterFormaTitulo();
            titulo.Historico = "TÍTULO IMPORTADO";
            titulo.Empresa = _empresa != null ? _empresa : ObterEmpresa();
            titulo.TipoAmbiente = _empresa?.TipoAmbiente ?? Dominio.Enumeradores.TipoAmbiente.Nenhum;
            titulo.Portador = ObterPortador();
            titulo.NossoNumero = ObterNossoNumero(titulo);

            titulo.DataLancamento = DateTime.Now;
            titulo.Usuario = usuario;
            titulo.Provisao = ObterProvisao();

            Dominio.Entidades.Embarcador.Financeiro.LancamentoCentroResultado lancamentoCentroResultado = ObterLancamentoCentroResultado(titulo);
            if (lancamentoCentroResultado != null)
            {
                titulo.LancamentosCentroResultado = new List<Dominio.Entidades.Embarcador.Financeiro.LancamentoCentroResultado>();
                titulo.LancamentosCentroResultado.Add(lancamentoCentroResultado);
            }

            List<Dominio.Entidades.Veiculo> veiculo = ObterVeiculos();
            if (veiculo != null && veiculo.Count > 0)
            {
                titulo.Veiculos = new List<Dominio.Entidades.Veiculo>();
                foreach (Dominio.Entidades.Veiculo veiculos in veiculo)
                {
                    titulo.Veiculos.Add(veiculos);
                }
            }

            ValidarDados(titulo);

            return titulo;
        }

        public Dominio.Entidades.Embarcador.Financeiro.Titulo ObterTituloLiquidosFolhaImportar(Dominio.Entidades.Usuario usuario)
        {
            Dominio.Entidades.Embarcador.Financeiro.Titulo titulo = new Dominio.Entidades.Embarcador.Financeiro.Titulo();
            titulo.Initialize();

            titulo.Pessoa = ObterMotorista();
            titulo.GrupoPessoas = titulo.Pessoa.GrupoPessoas;
            titulo.TipoTitulo = TipoTitulo.Pagar;
            titulo.DataEmissao = ObterDataCompetencia();
            titulo.DataVencimento = ObterDataVencimento();
            titulo.DataProgramacaoPagamento = ObterDataVencimento();
            titulo.ValorOriginal = ObterValorOriginal();
            titulo.TipoMovimento = ObterTipoMovimento();

            titulo.Sequencia = 1;
            titulo.Desconto = 0;
            titulo.Acrescimo = 0;
            titulo.TipoDocumentoTituloOriginal = "L. FOLHA";
            titulo.NumeroDocumentoTituloOriginal = ObterNumeroDocumento();
            titulo.Observacao = "";

            titulo.Valor = titulo.ValorOriginal;
            titulo.ValorTotal = titulo.ValorOriginal;
            titulo.ValorPendente = titulo.ValorOriginal;
            titulo.StatusTitulo = StatusTitulo.EmAberto;
            titulo.DataAlteracao = DateTime.Now;
            titulo.FormaTitulo = FormaTitulo.Outros;
            titulo.Historico = "TÍTULO IMPORTADO";
            titulo.Empresa = _empresa;
            titulo.TipoAmbiente = _empresa?.TipoAmbiente ?? Dominio.Enumeradores.TipoAmbiente.Nenhum;

            titulo.DataLancamento = DateTime.Now;
            titulo.Usuario = usuario;

            ValidarDados(titulo);

            return titulo;
        }

        #endregion
    }
}
