using Dominio.Excecoes.Embarcador;
using Dominio.ObjetosDeValor.Embarcador.Enumeradores;
using Dominio.ObjetosDeValor.Embarcador.Importacao;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using SGTAdmin.Controllers;

namespace SGT.WebAdmin.Controllers.Transportadores
{
    [CustomAuthorize("Veiculos/Veiculo")]
    public class VeiculoController : BaseController
    {
        #region Construtores

        public VeiculoController(Conexao conexao) : base(conexao) { }

        #endregion

        #region Métodos Globais

        public async Task<IActionResult> Adicionar()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                Repositorio.Veiculo repVeiculo = new Repositorio.Veiculo(unitOfWork);
                Repositorio.Empresa repEmpresa = new Repositorio.Empresa(unitOfWork);
                Repositorio.Embarcador.Pessoas.GrupoPessoas repGrupoPessoas = new Repositorio.Embarcador.Pessoas.GrupoPessoas(unitOfWork);
                Repositorio.Embarcador.Configuracao.ConfiguracaoTMS repConfiguracao = new Repositorio.Embarcador.Configuracao.ConfiguracaoTMS(unitOfWork);
                Repositorio.Embarcador.Veiculos.VeiculoIntegracao repVeiculoIntegracao = new Repositorio.Embarcador.Veiculos.VeiculoIntegracao(unitOfWork);
                Repositorio.Embarcador.Cargas.TipoIntegracao repTipoIntegracao = new Repositorio.Embarcador.Cargas.TipoIntegracao(unitOfWork);
                Repositorio.Cliente repCliente = new Repositorio.Cliente(unitOfWork);
                Repositorio.Embarcador.Veiculos.VeiculoMotorista repVeiculoMotorista = new Repositorio.Embarcador.Veiculos.VeiculoMotorista(unitOfWork);
                Repositorio.Embarcador.Veiculos.HistoricoVeiculoVinculo repHistoricoVeiculoVinculo = new Repositorio.Embarcador.Veiculos.HistoricoVeiculoVinculo(unitOfWork);
                Repositorio.Embarcador.Veiculos.HistoricoVeiculoVinculoCentroResultado repHistoricoVeiculoVinculoCentroResultado = new Repositorio.Embarcador.Veiculos.HistoricoVeiculoVinculoCentroResultado(unitOfWork);
                Repositorio.Localidade repLocalidade = new Repositorio.Localidade(unitOfWork);

                Dominio.Entidades.Embarcador.Configuracao.ConfiguracaoTMS configuracao = repConfiguracao.BuscarConfiguracaoPadrao();
                Servicos.Embarcador.Frota.Frota servFrota = new Servicos.Embarcador.Frota.Frota(unitOfWork, TipoServicoMultisoftware, Cliente, WebServiceConsultaCTe);

                unitOfWork.Start();

                Dominio.Entidades.Veiculo veiculo = new Dominio.Entidades.Veiculo();
                PreencherVeiculo(veiculo, unitOfWork);

                VerificarCamposRastreador(veiculo, configuracao);

                if (veiculo.PossuiRastreador)
                    ValidarRastreadorVeiculoUnico(veiculo, unitOfWork);

                if (TipoServicoMultisoftware == AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiCTe || TipoServicoMultisoftware == AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiNFe)
                    veiculo.Empresa = Usuario.Empresa;
                else if (TipoServicoMultisoftware == AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.Terceiros || TipoServicoMultisoftware == AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.TransportadorTerceiro)
                {
                    if (veiculo.Empresa == null)
                    {
                        Dominio.Entidades.Empresa empresaTerceiro = repEmpresa.BuscarPorCNPJ(Usuario.ClienteTerceiro.CPF_CNPJ_SemFormato);
                        veiculo.Empresa = empresaTerceiro != null ? empresaTerceiro : Usuario.Empresa;
                    }
                    if (veiculo.Empresa.CNPJ != Usuario.ClienteTerceiro.CPF_CNPJ_SemFormato)
                    {
                        veiculo.Proprietario = Usuario.ClienteTerceiro;
                        veiculo.Tipo = "T";
                    }
                    else
                    {
                        veiculo.Proprietario = null;
                        veiculo.Tipo = "P";
                    }
                }
                else if (veiculo.Empresa == null && TipoServicoMultisoftware != AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiTMS)
                    veiculo.Empresa = repEmpresa.BuscarPorCodigo(int.Parse(Request.Params("Empresa")));

                if (TipoServicoMultisoftware != AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiTMS)
                {
                    if (veiculo.Empresa != null && veiculo.Empresa.Matriz != null && veiculo.Empresa.Matriz.Count > 0)
                        veiculo.Empresa = veiculo.Empresa.Matriz.FirstOrDefault();
                }
                else
                {
                    veiculo.EmpresaFilial = repEmpresa.BuscarPorCodigo(int.Parse(Request.Params("EmpresaFilial")));
                }

                if (configuracao.Pais != TipoPais.Brasil && veiculo.Empresa != null)
                    veiculo.Estado = veiculo.Empresa.Localidade.Estado;


                Dominio.Entidades.Veiculo veiculoExiste = repVeiculo.BuscarPorPlacaVarrendoFiliais(veiculo.Empresa?.Codigo ?? 0, veiculo.Placa, false);
                Dominio.Entidades.Veiculo renavamExiste = repVeiculo.BuscarPorRenavam(veiculo.Empresa?.Codigo ?? 0, veiculo.Renavam, veiculo.Placa);
                Dominio.Entidades.Veiculo chassiExiste = !string.IsNullOrWhiteSpace(veiculo.Chassi) ? repVeiculo.BuscarPorChassiEPlaca(veiculo.Empresa?.Codigo ?? 0, veiculo.Chassi, veiculo.Placa) : null;

                if (veiculoExiste != null && veiculo.Ativo == true)
                    throw new ControllerException(string.Format(Localization.Resources.Veiculos.Veiculo.PlacaInformadaJaEstaCadastrada, veiculo.Placa));
                else if (renavamExiste != null && veiculo.Ativo == true && configuracao.Pais != TipoPais.Exterior && veiculo.Empresa?.Tipo != "E")
                    throw new ControllerException(string.Format(Localization.Resources.Veiculos.Veiculo.RenavamInformadoJaEstaCadastrado, veiculo.Renavam));
                else if (chassiExiste != null && veiculo.Ativo == true)
                    throw new ControllerException(string.Format(Localization.Resources.Veiculos.Veiculo.ChassiInformadoJaEstaCadastrado, veiculo.Chassi));
                else if (ConfiguracaoEmbarcador.Pais == TipoPais.Brasil && veiculo.Empresa?.Tipo != "E" && ConfiguracaoEmbarcador.ValidarRENAVAMVeiculo && !Utilidades.Validate.ValidarRENAVAM(veiculo.Renavam))
                    throw new ControllerException(string.Format(Localization.Resources.Veiculos.Veiculo.RenavamInformadoInvalidoPorFavorPreenchaUmRenavamValido, veiculo.Renavam));

                if (configuracao.ValidarSeExisteVeiculoCadastradoComMesmoNrDeFrota)
                {
                    Dominio.Entidades.Veiculo veiculoComFrota = repVeiculo.BuscarPorPlaca(0, veiculo.NumeroFrota, 0, null, null);
                    if (veiculoComFrota != null)
                    {
                        throw new ControllerException(string.Format(Localization.Resources.Veiculos.Veiculo.FrotaInformadaJaEstaCadastradaParaPlaca, veiculoComFrota.Placa));
                    }
                }

                int.TryParse(Request.Params("ModeloVeicularCarga"), out int codigoModeloVeicularCarga);
                string ciot = Request.Params("CIOT");

                FormasPagamento? formaPagamentoCIOT = Request.GetNullableEnumParam<FormasPagamento>("FormaPagamento");
                decimal valorAdiantamento = Request.GetDecimalParam("ValorAdiantamento");
                decimal valorFrete = Request.GetDecimalParam("ValorFrete");
                DateTime? dataVencimento = Request.GetNullableDateTimeParam("DataVencimento");
                string cnpjInstituicaoPagamento = Request.GetStringParam("CNPJInstituicaoPagamento");
                TipoPagamentoMDFe? tipoPagamentoCIOT = Request.GetNullableEnumParam<TipoPagamentoMDFe>("TipoPagamento");
                string contaCIOT = Request.GetStringParam("ContaCIOT");
                Dominio.ObjetosDeValor.Enumerador.TipoChavePix tipoChavePIX = Request.GetEnumParam<Dominio.ObjetosDeValor.Enumerador.TipoChavePix>("TipoChavePIX");
                string agenciaCIOT = Request.GetStringParam("AgenciaCIOT");
                string chavePIXCIOT = Request.GetStringParam("ChavePIXCIOT");

                string cnpjProprietario = Request.Params("Proprietario");
                if ((TipoServicoMultisoftware == AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.Terceiros || TipoServicoMultisoftware == AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.TransportadorTerceiro) && Usuario.ClienteTerceiro != null)
                    cnpjProprietario = Usuario.ClienteTerceiro.CPF_CNPJ_SemFormato;

                if (!string.IsNullOrWhiteSpace(ciot))
                {
                    if (ciot.Length < 12)
                    {
                        throw new ControllerException(Localization.Resources.Veiculos.Veiculo.NumeroDoCiotDeveConterAoMinimoDozeLetras);
                    }
                }

                veiculo.CIOT = ciot;
                veiculo.ValorAdiantamentoCIOT = valorAdiantamento;
                veiculo.ValorFreteCIOT = valorFrete;
                veiculo.TipoPagamentoCIOT = tipoPagamentoCIOT;
                veiculo.FormaPagamentoCIOT = formaPagamentoCIOT;
                veiculo.DataVencimentoCIOT = dataVencimento;
                veiculo.CNPJInstituicaoPagamentoCIOT = cnpjInstituicaoPagamento;
                veiculo.ContaCIOT = contaCIOT;
                veiculo.TipoChavePIX = tipoChavePIX;
                veiculo.ChavePIXCIOT = chavePIXCIOT;
                veiculo.AgenciaCIOT = agenciaCIOT;

                if (veiculo.Tipo == "T")
                {
                    veiculo.TipoProprietario = (Dominio.Enumeradores.TipoProprietarioVeiculo)int.Parse(Request.Params("TipoProprietario"));
                    if (TipoServicoMultisoftware != AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.Terceiros && TipoServicoMultisoftware != AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.TransportadorTerceiro)
                        veiculo.Proprietario = repCliente.BuscarPorCPFCNPJ(double.Parse(Utilidades.String.OnlyNumbers(cnpjProprietario)));

                    int.TryParse(Request.Params("RNTRC"), out int rntrc);
                    veiculo.RNTRC = rntrc;

                    if (veiculo.Proprietario == null)
                        throw new ControllerException(Localization.Resources.Veiculos.Veiculo.ObrigatorioInformarProprietarioQuandoVeiculoForDeTerceiro);

                    if (veiculo.RNTRC == 0 && ConfiguracaoEmbarcador.Pais == TipoPais.Brasil)
                        throw new ControllerException(Localization.Resources.Veiculos.Veiculo.NecessarioInformarRNTRCCorretamente);

                    decimal valorValePedagio = 0;
                    double responsavelValePedagio = 0;
                    double fornecedorValePedagio = 0;
                    decimal.TryParse(Request.Params("ValorValePedagio"), out valorValePedagio);
                    veiculo.ValorValePedagio = valorValePedagio;

                    double.TryParse(Request.Params("ResponsavelValePedagio"), out responsavelValePedagio);
                    double.TryParse(Request.Params("FornecedorValePedagio"), out fornecedorValePedagio);

                    if (responsavelValePedagio > 0)
                        veiculo.ResponsavelValePedagio = new Dominio.Entidades.Cliente() { CPF_CNPJ = responsavelValePedagio };
                    else
                        veiculo.ResponsavelValePedagio = null;

                    if (fornecedorValePedagio > 0)
                        veiculo.FornecedorValePedagio = new Dominio.Entidades.Cliente() { CPF_CNPJ = fornecedorValePedagio };
                    else
                        veiculo.FornecedorValePedagio = null;

                    veiculo.NumeroCompraValePedagio = Request.Params("NumeroCompraValePedagio");
                }

                double.TryParse(Request.Params("Locador"), out double locadorCNPJ);
                veiculo.Locador = repCliente.BuscarPorCPFCNPJ(locadorCNPJ);

                dynamic dynTransportadoras = Newtonsoft.Json.JsonConvert.DeserializeObject<dynamic>((string)Request.Params("Transportadoras"));
                if (dynTransportadoras.Count > 0)
                {
                    veiculo.Empresas = new List<Dominio.Entidades.Empresa>();
                    foreach (var dynTransportador in dynTransportadoras)
                    {
                        Dominio.Entidades.Empresa empresa = repEmpresa.BuscarPorCodigo((int)dynTransportador.Codigo);
                        veiculo.Empresas.Add(empresa);
                    }
                }

                SalvarEquipamentos(veiculo, unitOfWork);

                SalvarCurrais(veiculo, unitOfWork);

                if (configuracao.NaoPermitirVincularMotoristaEmVariosVeiculos)
                {
                    Dominio.Entidades.Usuario motoristaPrincipal = veiculo.Motoristas?.Where(o => o.Principal).Select(o => o.Motorista).FirstOrDefault();

                    if (motoristaPrincipal != null)
                    {
                        List<Dominio.Entidades.Veiculo> veiculosMotorista = repVeiculo.BuscarVeiculosPorMotorista(motoristaPrincipal.Codigo, veiculo.Codigo);
                        if (veiculosMotorista.Count > 0)
                            throw new ControllerException(string.Format(Localization.Resources.Veiculos.Veiculo.MotoristaInformadoJaEstaVinculadoAoVeiculo, veiculosMotorista.FirstOrDefault().Placa));
                    }
                }

                List<AdminMultisoftware.Dominio.Enumeradores.PermissaoPersonalizada> permissoesPersonalizadas = ObterPermissoesPersonalizadas("Veiculos/Veiculo");

                veiculo.ModeloVeicularCarga = codigoModeloVeicularCarga > 0 ? new Dominio.Entidades.Embarcador.Cargas.ModeloVeicularCarga() { Codigo = codigoModeloVeicularCarga } : null;

                veiculo.VeiculosVinculados = new List<Dominio.Entidades.Veiculo>();
                Dominio.Entidades.Usuario veiculoMotorista = repVeiculoMotorista.BuscarMotoristaPrincipal(veiculo.Codigo);

                if (veiculo.TipoVeiculo == "0")
                {
                    dynamic listaVeiculosVinculados = Newtonsoft.Json.JsonConvert.DeserializeObject<dynamic>((string)Request.Params("VeiculosVinculados"));
                    bool situacaoAnterior = false;
                    foreach (var jVeiculoVinculado in listaVeiculosVinculados)
                    {
                        bool inserir = false;
                        Dominio.Entidades.Veiculo veiculoVinculado;
                        if (TipoServicoMultisoftware == AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiCTe || TipoServicoMultisoftware == AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiNFe
                            || TipoServicoMultisoftware == AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiEmbarcador)
                            veiculoVinculado = repVeiculo.BuscarTodosPorPlaca(veiculo.Empresa != null ? veiculo.Empresa.Codigo : 0, (string)jVeiculoVinculado.Placa.ToString().ToUpper());
                        else
                            veiculoVinculado = repVeiculo.BuscarTodosPorPlaca(0, (string)jVeiculoVinculado.Placa.ToString().ToUpper());
                        if (veiculoVinculado == null)
                        {
                            veiculoVinculado = new Dominio.Entidades.Veiculo();
                            inserir = true;
                        }
                        else
                        {
                            veiculoVinculado.Initialize();
                            if (veiculoVinculado.VeiculosTracao != null && veiculoVinculado.VeiculosTracao.Count > 0)
                            {
                                Dominio.Entidades.Veiculo veiculoTracaoReboque = veiculoVinculado.VeiculosTracao.FirstOrDefault();
                                veiculoTracaoReboque.VeiculosVinculados.Clear();
                                repVeiculo.Atualizar(veiculoTracaoReboque);
                            }
                            if (veiculoVinculado != null)
                                situacaoAnterior = veiculoVinculado.Ativo;
                        }

                        if (TipoServicoMultisoftware == AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiTMS && !veiculo.Ativo)
                            throw new ControllerException(Localization.Resources.Veiculos.Veiculo.AntesDeInativarVeiculoFavorRemoverOsVinculosComOsDemaisReboques);

                        if (veiculoVinculado == null)
                        {
                            veiculoVinculado = new Dominio.Entidades.Veiculo();
                            inserir = true;
                        }
                        if (inserir || veiculoVinculado.TipoVeiculo == "1")
                        {
                            veiculoVinculado.LocalAtualFisicoDoVeiculo = veiculo.LocalAtualFisicoDoVeiculo;

                            if (TipoServicoMultisoftware == AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiCTe || TipoServicoMultisoftware == AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiNFe)
                                veiculoVinculado.Empresa = Usuario.Empresa;

                            veiculoVinculado.Placa = (string)jVeiculoVinculado.Placa.ToString().ToUpper();
                            veiculoVinculado.Tara = ((string)jVeiculoVinculado.Tara).ToInt();
                            veiculoVinculado.CapacidadeKG = ((string)jVeiculoVinculado.CapacidadeQuilo).ToInt();
                            veiculoVinculado.CapacidadeM3 = ((string)jVeiculoVinculado.CapacidadeM3).ToInt();
                            veiculoVinculado.TipoRodado = (string)jVeiculoVinculado.TipoRodado;
                            veiculoVinculado.Tipo = (string)jVeiculoVinculado.Tipo;
                            veiculoVinculado.Ativo = veiculo.Ativo;
                            veiculoVinculado.TipoCarroceria = (string)jVeiculoVinculado.TipoCarroceria;
                            veiculoVinculado.Renavam = (string)jVeiculoVinculado.Renavam;
                            veiculoVinculado.Estado = new Dominio.Entidades.Estado() { Sigla = (string)jVeiculoVinculado.Estado };
                            veiculoVinculado.LocalidadeEmplacamento = !string.IsNullOrEmpty((string)jVeiculoVinculado.LocalidadeEmplacamento) ? repLocalidade.BuscarPorCodigo(int.Parse((string)jVeiculoVinculado.LocalidadeEmplacamento)) : null;
                            veiculoVinculado.ObservacaoCTe = (string)jVeiculoVinculado.ObservacaoCTe;
                            veiculoVinculado.FuncionarioResponsavel = veiculo?.FuncionarioResponsavel ?? null;

                            if (veiculoVinculado.Tipo == "T")
                            {
                                veiculoVinculado.TipoProprietario = ((string)jVeiculoVinculado.TipoProprietario).ToEnum<Dominio.Enumeradores.TipoProprietarioVeiculo>();
                                veiculoVinculado.Proprietario = repCliente.BuscarPorCPFCNPJ(double.Parse(Utilidades.String.OnlyNumbers((string)jVeiculoVinculado.Proprietario.Codigo)));
                                veiculoVinculado.RNTRC = ((string)jVeiculoVinculado.RNTRC).ToInt();

                                if (veiculoVinculado.Proprietario == null)
                                    throw new ControllerException(Localization.Resources.Veiculos.Veiculo.ObrigatorioInformarProprietarioQuandoVeiculoForDeTerceiro);

                                if (veiculoVinculado.RNTRC == 0 && ConfiguracaoEmbarcador.Pais == TipoPais.Brasil)
                                    throw new ControllerException(Localization.Resources.Veiculos.Veiculo.NecessarioInformarRNTRCCorretamente);
                            }
                            if (veiculo.Empresa != null)
                            {
                                veiculoVinculado.Empresa = veiculo.Empresa;

                                if (configuracao.Pais != TipoPais.Brasil)
                                    veiculoVinculado.Estado = veiculo.Empresa.Localidade.Estado;
                            }

                            if (veiculoMotorista != null)
                            {
                                if (veiculoVinculado.Codigo > 0)
                                {
                                    Servicos.Auditoria.Auditoria.Auditar(Auditado, veiculoVinculado, $"Removido motorista principal.", unitOfWork);
                                    repVeiculoMotorista.DeletarMotoristaPrincipal(veiculoVinculado.Codigo);
                                }

                                Dominio.Entidades.Embarcador.Veiculos.VeiculoMotorista VeiculoMotoristaPrincipal = new Dominio.Entidades.Embarcador.Veiculos.VeiculoMotorista
                                {
                                    CPF = veiculoMotorista.CPF,
                                    Motorista = veiculoMotorista,
                                    Nome = veiculoMotorista.Nome,
                                    Veiculo = veiculoVinculado,
                                    Principal = true
                                };
                                repVeiculoMotorista.Inserir(VeiculoMotoristaPrincipal);
                            }

                            Dominio.Entidades.Embarcador.Financeiro.CentroResultado centroResultadoAntigo = veiculo.CentroResultado;

                            veiculoVinculado.CentroResultado = veiculo.CentroResultado;
                            veiculoVinculado.TipoVeiculo = "1";

                            int modeloReboque = ((string)jVeiculoVinculado.ModeloVeicularCarga.Codigo).ToInt();
                            if (modeloReboque > 0)
                                veiculoVinculado.ModeloVeicularCarga = new Dominio.Entidades.Embarcador.Cargas.ModeloVeicularCarga() { Codigo = modeloReboque };
                            else
                            {
                                if (veiculoVinculado.ModeloVeicularCarga == null)
                                    veiculoVinculado.ModeloVeicularCarga = veiculo.ModeloVeicularCarga;
                            }

                            if (inserir)
                                repVeiculo.Inserir(veiculoVinculado, Auditado);
                            else
                            {
                                Servicos.Embarcador.Veiculo.VeiculoHistorico.InserirHistoricoVeiculo(veiculoVinculado, situacaoAnterior, MetodosAlteracaoVeiculo.Adicionar_VeiculoController, this.Usuario, unitOfWork);
                                repVeiculo.Atualizar(veiculoVinculado, Auditado);
                            }

                            veiculo.VeiculosVinculados.Add(veiculoVinculado);

                            if (TipoServicoMultisoftware == AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiTMS)
                            {
                                Dominio.Entidades.Embarcador.Veiculos.HistoricoVeiculoVinculo historicoVeiculoVinculo = new Dominio.Entidades.Embarcador.Veiculos.HistoricoVeiculoVinculo();
                                historicoVeiculoVinculo = repHistoricoVeiculoVinculo.BuscarPorVeiculo(veiculo.Codigo);

                                if (historicoVeiculoVinculo == null && veiculo.Codigo > 0)
                                {
                                    historicoVeiculoVinculo = new Dominio.Entidades.Embarcador.Veiculos.HistoricoVeiculoVinculo()
                                    {
                                        Veiculo = veiculo,
                                        DataHora = DateTime.Now,
                                        Usuario = Usuario,
                                        KmRodado = veiculo.KilometragemAtual,
                                        KmAtualModificacao = 0,
                                        DiasVinculado = 0
                                    };
                                    repHistoricoVeiculoVinculo.Inserir(historicoVeiculoVinculo);

                                    Dominio.Entidades.Embarcador.Veiculos.HistoricoVeiculoVinculoCentroResultado historicoVeiculoVinculoCentroResultado = new Dominio.Entidades.Embarcador.Veiculos.HistoricoVeiculoVinculoCentroResultado
                                    {
                                        HistoricoVeiculoVinculo = historicoVeiculoVinculo,
                                        CentroResultado = veiculo.CentroResultado,
                                        DataHora = DateTime.Now,
                                    };

                                    repHistoricoVeiculoVinculoCentroResultado.Inserir(historicoVeiculoVinculoCentroResultado, Auditado);
                                }
                            }


                        }
                        else if (veiculoVinculado.TipoVeiculo == "0")
                            throw new ControllerException(string.Format(Localization.Resources.Veiculos.Veiculo.NaoEPossivelVincularVeiculoDoTipoTracaoAUmVeiculoDoTipoTracao, veiculoVinculado.Placa));
                    }
                }

                SalvarConfiguracoesTarget(veiculo);

                repVeiculo.Inserir(veiculo, Auditado);

                //Replica modelo veicular para veiculos com mesma placa
                if (veiculo.ModeloVeicularCarga != null)
                {
                    List<Dominio.Entidades.Veiculo> listasVeiculos = repVeiculo.BuscarListaPorPlaca(veiculo.Placa);
                    foreach (Dominio.Entidades.Veiculo veic in listasVeiculos)
                    {
                        if (veic.Codigo != veiculo.Codigo)
                        {
                            veic.ModeloVeicularCarga = veiculo.ModeloVeicularCarga;
                            repVeiculo.Atualizar(veic);
                        }
                    }
                }

                SalvarMotoristaPrincipal(veiculo, unitOfWork, configuracao);
                SalvarLicencas(veiculo, unitOfWork);
                SalvarVeiculoRotasFrete(veiculo, unitOfWork);

                if (!ValidarCamposReferenteCIOT(veiculo, unitOfWork, out string erroValidacaoCIOT))
                    throw new ControllerException(erroValidacaoCIOT);

                Servicos.Embarcador.Veiculo.Veiculo.AtualizarIntegracoes(unitOfWork, veiculo);

                int codigoGrupoServico = Request.GetIntParam("GrupoServico");
                if (configuracao.GerarOSAutomaticamenteCadastroVeiculoEquipamento && veiculo.Tipo.Equals("P") && codigoGrupoServico > 0)
                {
                    Repositorio.Embarcador.Frota.GrupoServico repGrupoServico = new Repositorio.Embarcador.Frota.GrupoServico(unitOfWork);
                    Dominio.ObjetosDeValor.Embarcador.Frota.ObjetoOrdemServico objetoOrdemServico = new Dominio.ObjetosDeValor.Embarcador.Frota.ObjetoOrdemServico()
                    {
                        CadastrandoVeiculoEquipamento = true,
                        Observacao = Localization.Resources.Veiculos.Veiculo.GeradoAutomaticamenteAoCadastrarVeiculo,
                        Operador = Usuario,
                        Veiculo = veiculo,
                        QuilometragemVeiculo = veiculo.KilometragemAtual,
                        GrupoServico = repGrupoServico.BuscarPorCodigo(codigoGrupoServico),
                        Empresa = TipoServicoMultisoftware == AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiNFe ? Empresa : null
                    };

                    Servicos.Embarcador.Frota.OrdemServico.GerarFinalizarOrdemServicoCompleta(objetoOrdemServico, Usuario, Auditado, unitOfWork, TipoServicoMultisoftware);
                }

                if (repGrupoPessoas.ContemGrupoPessoasEnvioEmailCadastroVeiculo())
                {
                    Dominio.Entidades.Embarcador.Cargas.TipoIntegracao tipoIntegracao = repTipoIntegracao.BuscarPorTipo(TipoIntegracao.Email);
                    if (tipoIntegracao != null)
                    {
                        if (tipoIntegracao.Tipo != TipoIntegracao.Frota162 || veiculo.Tipo != "T")
                        {
                            Dominio.Entidades.Embarcador.Veiculos.VeiculoIntegracao veiculoIntegracao = new Dominio.Entidades.Embarcador.Veiculos.VeiculoIntegracao()
                            {
                                DataIntegracao = DateTime.Now,
                                SituacaoIntegracao = SituacaoIntegracao.Integrado,
                                NumeroTentativas = 1,
                                ProblemaIntegracao = "",
                                Veiculo = veiculo,
                                TipoIntegracao = tipoIntegracao,
                                GrupoPessoas = null
                            };
                            repVeiculoIntegracao.Inserir(veiculoIntegracao);
                            Servicos.Embarcador.Veiculo.Veiculo.IntegrarEmailVeiculoNovo(veiculoIntegracao, unitOfWork);
                        }
                    }
                }

                SalvarLiberacaoGR(veiculo, unitOfWork);

                ProcessarCadastroVeiculo(veiculo, true, unitOfWork);
                BloquearOutrosCadastroVeiculos(veiculo, unitOfWork);
                DesbloquearOutrosCadastroVeiculos(veiculo, unitOfWork);

                new Servicos.Embarcador.Integracao.SemParar.ConsultaCadastroVeiculo(unitOfWork, TipoServicoMultisoftware).GerarIntegracaoCadastroVeiculo(veiculo);

                servFrota.AtualizarFrotaMotoristaVeiculo(veiculo);

                unitOfWork.CommitChanges();

                return new JsonpResult(true);
            }
            catch (BaseException excecao)
            {
                unitOfWork.Rollback();
                return new JsonpResult(false, true, excecao.Message);
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);
                unitOfWork.Rollback();
                return new JsonpResult(false, Localization.Resources.Gerais.Geral.OcorreuFalhaAoAdicionar);
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        public async Task<IActionResult> Atualizar()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                Repositorio.Veiculo repVeiculo = new Repositorio.Veiculo(unitOfWork);
                Repositorio.Abastecimento repAbastecimento = new Repositorio.Abastecimento(unitOfWork);
                Repositorio.Empresa repEmpresa = new Repositorio.Empresa(unitOfWork);
                Repositorio.Cliente repCliente = new Repositorio.Cliente(unitOfWork);
                Repositorio.Localidade repLocalidade = new Repositorio.Localidade(unitOfWork);
                Repositorio.Embarcador.Configuracao.ConfiguracaoTMS repConfiguracao = new Repositorio.Embarcador.Configuracao.ConfiguracaoTMS(unitOfWork);
                Repositorio.Embarcador.Veiculos.VeiculoMotorista repVeiculoMotorista = new Repositorio.Embarcador.Veiculos.VeiculoMotorista(unitOfWork);
                Repositorio.Embarcador.Cargas.Carga repCarga = new Repositorio.Embarcador.Cargas.Carga(unitOfWork);
                Servicos.Embarcador.Frota.Frota servFrota = new Servicos.Embarcador.Frota.Frota(unitOfWork, TipoServicoMultisoftware, Cliente, WebServiceConsultaCTe);
                Dominio.Entidades.Embarcador.Configuracao.ConfiguracaoTMS configuracao = repConfiguracao.BuscarConfiguracaoPadrao();
                Repositorio.Embarcador.Financeiro.CentroResultado repCentroResultado = new Repositorio.Embarcador.Financeiro.CentroResultado(unitOfWork);
                Repositorio.Embarcador.Veiculos.HistoricoVeiculoVinculo repHistoricoVeiculoVinculo = new Repositorio.Embarcador.Veiculos.HistoricoVeiculoVinculo(unitOfWork);
                Repositorio.Embarcador.Veiculos.HistoricoVeiculoVinculoCentroResultado repHistoricoVeiculoVinculoCentroResultado = new Repositorio.Embarcador.Veiculos.HistoricoVeiculoVinculoCentroResultado(unitOfWork);
                Dominio.Entidades.Embarcador.Configuracoes.ConfiguracaoGeral configuracaoGeral = new Repositorio.Embarcador.Configuracoes.ConfiguracaoGeral(unitOfWork).BuscarConfiguracaoPadrao();

                Dominio.Entidades.Veiculo veiculo = repVeiculo.BuscarPorCodigo(int.Parse(Request.Params("Codigo")), true);
                int hilometragemViradaAnterior = veiculo.KilometragemVirada;

                List<AdminMultisoftware.Dominio.Enumeradores.PermissaoPersonalizada> permissoesPersonalizadas = ObterPermissoesPersonalizadas("Veiculos/Veiculo");

                bool ativo = Request.GetBoolParam("Ativo");
                if (TipoServicoMultisoftware == AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiTMS && !ativo)
                    if (!(this.Usuario.UsuarioAdministrador || permissoesPersonalizadas.Contains(AdminMultisoftware.Dominio.Enumeradores.PermissaoPersonalizada.Usuario_PermitirAtivarInativarVeiculo)))
                        throw new ControllerException("Seu usuário não possui permissão para Ativar ou Inativar algum veículo.");

                if (TipoServicoMultisoftware == AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiCTe && configuracaoGeral.NaoPermitirDesabilitarCompraValePedagioVeiculo)
                    if ((!veiculo.NaoComprarValePedagio && Request.GetBoolParam("NaoComprarValePedagio")) || (!veiculo.NaoComprarValePedagioRetorno && Request.GetBoolParam("NaoComprarValePedagioRetorno")))
                        throw new ControllerException("Não é permitido bloquear a compra de Vale Pedágio ou Vale Pedágio Retorno.");

                bool situacaoAnterior = veiculo.Ativo;
                PreencherVeiculo(veiculo, unitOfWork);

                if ((decimal)veiculo.KilometragemVirada != hilometragemViradaAnterior)
                {
                    decimal ultimoHorimetro = repAbastecimento.BuscarUltimoKMAbastecimento(veiculo.Codigo, DateTime.Now, 0, TipoAbastecimento.Combustivel);
                    if (ultimoHorimetro > (decimal)veiculo.KilometragemVirada)
                        return new JsonpResult(false, true, string.Format(Localization.Resources.Veiculos.Veiculo.FavorVerifiqueKmDaViradaInformadoPoisExisteUmAbastecimentoComKmMaior, ultimoHorimetro.ToString("n0")));
                }

                if (veiculo.SituacaoCadastro == SituacaoCadastroVeiculo.Pendente)
                    return new JsonpResult(false, Localization.Resources.Veiculos.Veiculo.NaoFoiPossivelAtualizarVeiculoEnquantoCadastroEstiverPendente);

                VerificarCamposRastreador(veiculo, configuracao);

                if (veiculo.PossuiRastreador)
                    ValidarRastreadorVeiculoUnico(veiculo, unitOfWork);

                double.TryParse(Request.Params("Locador"), out double locadorCNPJ);
                veiculo.Locador = repCliente.BuscarPorCPFCNPJ(locadorCNPJ);

                int.TryParse(Request.Params("ModeloVeicularCarga"), out int codigoModeloVeicularCarga);
                string ciot = Request.Params("CIOT");
                FormasPagamento? formaPagamentoCIOT = Request.GetNullableEnumParam<FormasPagamento>("FormaPagamento");
                decimal valorAdiantamento = Request.GetDecimalParam("ValorAdiantamento");
                decimal valorFrete = Request.GetDecimalParam("ValorFrete");
                DateTime? dataVencimento = Request.GetNullableDateTimeParam("DataVencimento");
                string cnpjInstituicaoPagamento = Request.GetStringParam("CNPJInstituicaoPagamento");
                TipoPagamentoMDFe? tipoPagamentoCIOT = Request.GetNullableEnumParam<TipoPagamentoMDFe>("TipoPagamento");
                string contaCIOT = Request.GetStringParam("ContaCIOT");
                string agenciaCIOT = Request.GetStringParam("AgenciaCIOT");
                Dominio.ObjetosDeValor.Enumerador.TipoChavePix tipoChavePIX = Request.GetEnumParam<Dominio.ObjetosDeValor.Enumerador.TipoChavePix>("TipoChavePIX");
                string chavePIXCIOT = Request.GetStringParam("ChavePIXCIOT");

                string cnpjProprietario = Request.Params("Proprietario");
                if ((TipoServicoMultisoftware == AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.Terceiros || TipoServicoMultisoftware == AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.TransportadorTerceiro) && Usuario.ClienteTerceiro != null)
                    cnpjProprietario = Usuario.ClienteTerceiro.CPF_CNPJ_SemFormato;

                veiculo.ValorAdiantamentoCIOT = valorAdiantamento;
                veiculo.ValorFreteCIOT = valorFrete;
                veiculo.TipoPagamentoCIOT = tipoPagamentoCIOT;
                veiculo.FormaPagamentoCIOT = formaPagamentoCIOT;
                veiculo.DataVencimentoCIOT = dataVencimento;
                veiculo.CNPJInstituicaoPagamentoCIOT = cnpjInstituicaoPagamento;
                veiculo.ContaCIOT = contaCIOT;
                veiculo.TipoChavePIX = tipoChavePIX;
                veiculo.ChavePIXCIOT = chavePIXCIOT;
                veiculo.AgenciaCIOT = agenciaCIOT;

                Dominio.Entidades.Embarcador.Cargas.ModeloVeicularCarga modeloVeicularCarga = codigoModeloVeicularCarga > 0 ? new Dominio.Entidades.Embarcador.Cargas.ModeloVeicularCarga() { Codigo = codigoModeloVeicularCarga } : null;

                if ((veiculo.ModeloVeicularCarga != null) && (veiculo.ModeloVeicularCarga.Codigo != modeloVeicularCarga?.Codigo))
                {
                    bool possuiPneus = veiculo.Pneus?.Count > 0;
                    bool possuiEstepes = veiculo.Estepes?.Count > 0;

                    if (possuiPneus || possuiEstepes)
                        return new JsonpResult(false, true, Localization.Resources.Veiculos.Veiculo.VeiculoPossuiPneusOuEstepesAdicionadosParaAlterarModeloVeicularDeCargaNecessarioRemoverPrimeiro);
                }

                if (!(this.Usuario.UsuarioAdministrador || permissoesPersonalizadas.Contains(AdminMultisoftware.Dominio.Enumeradores.PermissaoPersonalizada.Veiculo_PermitirAlterarModeloVeicular)))
                    if (veiculo.ModeloVeicularCarga.Codigo != modeloVeicularCarga?.Codigo)
                        throw new ControllerException("Seu usuário não possui permissão para alterar o modelo veicular da carga.");

                veiculo.ModeloVeicularCarga = modeloVeicularCarga;

                if (TipoServicoMultisoftware == AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiCTe || TipoServicoMultisoftware == AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiNFe)
                    veiculo.Empresa = Usuario.Empresa;
                else if (TipoServicoMultisoftware == AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.Terceiros || TipoServicoMultisoftware == AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.TransportadorTerceiro)
                {
                    if (veiculo.Empresa == null)
                        veiculo.Empresa = Usuario.Empresa;
                    if (veiculo.Empresa.CNPJ != Usuario.ClienteTerceiro.CPF_CNPJ_SemFormato)
                    {
                        veiculo.Proprietario = Usuario.ClienteTerceiro;
                        veiculo.Tipo = "T";
                    }
                    else
                    {
                        veiculo.Proprietario = null;
                        veiculo.Tipo = "P";
                    }
                }
                else if (veiculo.Empresa == null || TipoServicoMultisoftware == AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiEmbarcador)
                {
                    int codEmp = 0;
                    int.TryParse(Request.Params("Empresa"), out codEmp);

                    if (TipoServicoMultisoftware != AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiTMS)
                    {
                        if (codEmp > 0)
                            veiculo.Empresa = repEmpresa.BuscarPorCodigo(codEmp);
                    }
                }

                if (TipoServicoMultisoftware == AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiTMS)
                {
                    veiculo.EmpresaFilial = repEmpresa.BuscarPorCodigo(int.Parse(Request.Params("EmpresaFilial")));
                }

                if (configuracao.Pais != TipoPais.Brasil && veiculo.Empresa != null)
                    veiculo.Estado = veiculo.Empresa.Localidade.Estado;

                var veiCodigo = Request.GetIntParam("Codigo");

                Dominio.Entidades.Veiculo veiculoExiste = repVeiculo.BuscarPorPlacaVarrendoFiliais(veiculo.Empresa?.Codigo ?? 0, veiculo.Placa);
                Dominio.Entidades.Veiculo renavamExiste = repVeiculo.BuscarPorRenavam(veiculo.Empresa?.Codigo ?? 0, veiculo.Renavam, veiculo.Placa);
                Dominio.Entidades.Veiculo chassiExiste = !string.IsNullOrWhiteSpace(veiculo.Chassi) ? repVeiculo.BuscarPorChassiEPlaca(veiculo.Empresa?.Codigo ?? 0, veiculo.Chassi, veiculo.Placa) : null;

                if (veiculoExiste != null && veiculoExiste.Codigo != veiCodigo && veiculo.Ativo == true)
                    return new JsonpResult(false, true, string.Format(Localization.Resources.Veiculos.Veiculo.PlacaInformadaJaEstaCadastrada, veiculo.Placa));
                else if (renavamExiste != null && renavamExiste.Codigo != veiCodigo && veiculo.Ativo == true && configuracao.Pais != TipoPais.Exterior && veiculo.Empresa?.Tipo != "E")
                    return new JsonpResult(false, true, string.Format(Localization.Resources.Veiculos.Veiculo.RenavamInformadoJaEstaCadastrado, veiculo.Renavam));
                else if (chassiExiste != null && chassiExiste.Codigo != veiCodigo && veiculo.Ativo == true)
                    return new JsonpResult(false, true, string.Format(Localization.Resources.Veiculos.Veiculo.ChassiInformadoJaEstaCadastrado, veiculo.Chassi));
                else if (ConfiguracaoEmbarcador.Pais == TipoPais.Brasil && veiculo.Empresa?.Tipo != "E" && ConfiguracaoEmbarcador.ValidarRENAVAMVeiculo && !Utilidades.Validate.ValidarRENAVAM(veiculo.Renavam))
                    throw new ControllerException(string.Format(Localization.Resources.Veiculos.Veiculo.RenavamInformadoInvalidoPorFavorPreenchaUmRenavamValido, veiculo.Renavam));

                if (ConfiguracaoEmbarcador.ValidarSeExisteVeiculoCadastradoComMesmoNrDeFrota)
                {
                    Dominio.Entidades.Veiculo veiculoComFrota = repVeiculo.BuscarPorPlaca(0, veiculo.NumeroFrota, 0, null, null);
                    if (veiculoComFrota != null)
                    {
                        throw new ControllerException(string.Format(Localization.Resources.Veiculos.Veiculo.FrotaInformadaJaEstaCadastradaParaPlaca, veiculoComFrota.Placa));
                    }
                }

                if (!veiculo.Ativo && veiculo.TipoVeiculo == "1")
                {
                    bool contemVinculoEmTracao = repVeiculo.ContemVinculoEmTracao(veiculo);
                    if (contemVinculoEmTracao)
                    {
                        string Placa = repVeiculo.PlacaVinculoEmTracao(veiculo);
                        throw new ControllerException(Localization.Resources.Veiculos.Veiculo.NaoPermitidoInativarReboqueVinculadoEmUmaTracao + String.Format(Localization.Resources.Veiculos.Veiculo.PlacaTracaoVinculada, Placa));

                    }
                }

                if (!veiculo.Ativo)
                {
                    bool contemCargaPendente = repCarga.BuscarCargasEmAbertoPorVeiculo(veiculo.Codigo) > 0;
                    if (contemCargaPendente)
                    {
                        List<string> cargas = repCarga.BuscarNumerosCargasEmAbertoPorVeiculo(veiculo.Codigo);
                        string msg = Localization.Resources.Veiculos.Veiculo.NaoPossivelInativarVeiculoPoisMesmoEstaEmUmaCargaNaoGeradaAindaCargas + " " + string.Join(", ", cargas);
                        throw new ControllerException(msg);
                    }
                }

                unitOfWork.Start();

                if (veiculo.Empresas != null && veiculo.Empresas.Count > 0)
                {
                    for (int i = 0; i < veiculo.Empresas.Count; i++)
                        veiculo.Empresas[i] = null;
                    repVeiculo.Atualizar(veiculo, Auditado);
                }

                dynamic dynTransportadoras = Newtonsoft.Json.JsonConvert.DeserializeObject<dynamic>((string)Request.Params("Transportadoras"));
                foreach (var dynTransportador in dynTransportadoras)
                {
                    Dominio.Entidades.Empresa empresa = repEmpresa.BuscarPorCodigo((int)dynTransportador.Codigo);
                    veiculo.Empresas.Add(empresa);
                }

                SalvarLicencas(veiculo, unitOfWork);
                SalvarCurrais(veiculo, unitOfWork);
                SalvarEquipamentos(veiculo, unitOfWork);
                SalvarMotoristaPrincipal(veiculo, unitOfWork, configuracao);
                SalvarVeiculoRotasFrete(veiculo, unitOfWork);
                SalvarLiberacaoGR(veiculo, unitOfWork);

                if (!string.IsNullOrWhiteSpace(ciot))
                {
                    if (ciot.Length < 12)
                        throw new ControllerException(Localization.Resources.Veiculos.Veiculo.NumeroDoCiotDeveConterAoMinimoDozeLetras);
                }

                veiculo.CIOT = ciot;

                if (veiculo.Tipo == "T")
                {
                    veiculo.TipoProprietario = (Dominio.Enumeradores.TipoProprietarioVeiculo)int.Parse(Request.Params("TipoProprietario"));
                    veiculo.Proprietario = new Dominio.Entidades.Cliente() { CPF_CNPJ = double.Parse(Utilidades.String.OnlyNumbers(cnpjProprietario)) };
                    veiculo.RNTRC = int.Parse(Request.Params("RNTRC"));

                    if (veiculo.Proprietario == null)
                        throw new ControllerException(Localization.Resources.Veiculos.Veiculo.ObrigatorioInformarProprietarioQuandoVeiculoForDeTerceiro);

                    if (veiculo.RNTRC == 0 && ConfiguracaoEmbarcador.Pais == TipoPais.Brasil)
                        throw new ControllerException(Localization.Resources.Veiculos.Veiculo.NecessarioInformarRNTRCCorretamente);

                    decimal valorValePedagio = 0;
                    double responsavelValePedagio = 0;
                    double fornecedorValePedagio = 0;

                    decimal.TryParse(Request.Params("ValorValePedagio"), out valorValePedagio);
                    veiculo.ValorValePedagio = valorValePedagio;

                    double.TryParse(Request.Params("ResponsavelValePedagio"), out responsavelValePedagio);
                    double.TryParse(Request.Params("FornecedorValePedagio"), out fornecedorValePedagio);

                    if (responsavelValePedagio > 0)
                        veiculo.ResponsavelValePedagio = new Dominio.Entidades.Cliente() { CPF_CNPJ = responsavelValePedagio };
                    else
                        veiculo.ResponsavelValePedagio = null;

                    if (fornecedorValePedagio > 0)
                        veiculo.FornecedorValePedagio = new Dominio.Entidades.Cliente() { CPF_CNPJ = fornecedorValePedagio };
                    else
                        veiculo.FornecedorValePedagio = null;

                    veiculo.NumeroCompraValePedagio = Request.Params("NumeroCompraValePedagio");
                }
                else
                    veiculo.Proprietario = null;

                if (configuracao.NaoPermitirVincularMotoristaEmVariosVeiculos)
                {
                    Dominio.Entidades.Usuario motoristaPrincipal = veiculo.Motoristas.Where(o => o.Principal).Select(o => o.Motorista).FirstOrDefault();

                    if (motoristaPrincipal != null)
                    {
                        List<Dominio.Entidades.Veiculo> veiculosMotorista = repVeiculo.BuscarVeiculosPorMotorista(motoristaPrincipal.Codigo, veiculo.Codigo);
                        if (veiculosMotorista.Count > 0)
                            throw new ControllerException(string.Format(Localization.Resources.Veiculos.Veiculo.MotoristaInformadoJaEstaVinculadoAoVeiculo, veiculosMotorista.FirstOrDefault().Placa));
                    }
                }

                veiculo.VeiculosVinculados.Clear();

                Dominio.Entidades.Usuario veiculoMotorista = repVeiculoMotorista.BuscarMotoristaPrincipal(veiculo.Codigo);

                if (veiculo.TipoVeiculo == "0")
                {
                    dynamic listaVeiculosVinculados = Newtonsoft.Json.JsonConvert.DeserializeObject<dynamic>((string)Request.Params("VeiculosVinculados"));
                    foreach (var jVeiculoVinculado in listaVeiculosVinculados)
                    {
                        bool inserir = false;
                        Dominio.Entidades.Veiculo veiculoVinculado;
                        if (TipoServicoMultisoftware == AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiCTe || TipoServicoMultisoftware == AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiNFe
                            || TipoServicoMultisoftware == AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiEmbarcador)
                            veiculoVinculado = repVeiculo.BuscarTodosPorPlaca(veiculo.Empresa != null ? veiculo.Empresa.Codigo : 0, (string)jVeiculoVinculado.Placa.ToString().ToUpper());
                        else
                            veiculoVinculado = repVeiculo.BuscarTodosPorPlaca(0, (string)jVeiculoVinculado.Placa.ToString().ToUpper());


                        if (veiculoVinculado == null)
                        {
                            veiculoVinculado = new Dominio.Entidades.Veiculo();
                            inserir = true;
                        }
                        else
                        {
                            veiculoVinculado.Initialize();
                            if (veiculoVinculado.VeiculosTracao != null && veiculoVinculado.VeiculosTracao.Count > 0)
                            {
                                Dominio.Entidades.Veiculo veiculoTracaoReboque = veiculoVinculado.VeiculosTracao.FirstOrDefault();
                                if (veiculoTracaoReboque.Codigo != veiculo.Codigo)
                                {
                                    veiculoTracaoReboque.VeiculosVinculados.Clear();
                                    repVeiculo.Atualizar(veiculoTracaoReboque, Auditado);
                                }
                            }
                        }

                        if (TipoServicoMultisoftware == AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiTMS && !veiculo.Ativo)
                            throw new ControllerException(Localization.Resources.Veiculos.Veiculo.AntesDeInativarVeiculoFavorRemoverOsVinculosComOsDemaisReboques);

                        if (inserir || veiculoVinculado.TipoVeiculo == "1")
                        {
                            veiculoVinculado.LocalAtualFisicoDoVeiculo = veiculo.LocalAtualFisicoDoVeiculo;

                            veiculoVinculado.Placa = (string)jVeiculoVinculado.Placa.ToString().ToUpper();
                            veiculoVinculado.Tara = ((string)jVeiculoVinculado.Tara).ToInt();
                            veiculoVinculado.CapacidadeKG = ((string)jVeiculoVinculado.CapacidadeQuilo).ToInt();
                            veiculoVinculado.CapacidadeM3 = ((string)jVeiculoVinculado.CapacidadeM3).ToInt();
                            veiculoVinculado.TipoRodado = (string)jVeiculoVinculado.TipoRodado;
                            veiculoVinculado.Tipo = (string)jVeiculoVinculado.Tipo;
                            veiculoVinculado.Renavam = (string)jVeiculoVinculado.Renavam;
                            if (TipoServicoMultisoftware == AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiCTe || TipoServicoMultisoftware == AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiNFe)
                                veiculoVinculado.Empresa = Usuario.Empresa;
                            veiculoVinculado.Ativo = veiculo.Ativo;
                            veiculoVinculado.TipoCarroceria = (string)jVeiculoVinculado.TipoCarroceria;
                            veiculoVinculado.Estado = new Dominio.Entidades.Estado() { Sigla = (string)jVeiculoVinculado.Estado };

                            if (!string.IsNullOrWhiteSpace((string)jVeiculoVinculado.LocalidadeEmplacamento?.Codigo))
                            {
                                int codigoLocalidadeEmplacamento = int.Parse((string)jVeiculoVinculado.LocalidadeEmplacamento?.Codigo);
                                veiculoVinculado.LocalidadeEmplacamento = codigoLocalidadeEmplacamento > 0 ? repLocalidade.BuscarPorCodigo(codigoLocalidadeEmplacamento) : null;
                            }

                            veiculoVinculado.FuncionarioResponsavel = veiculo?.FuncionarioResponsavel ?? null;

                            veiculoVinculado.ObservacaoCTe = (string)jVeiculoVinculado.ObservacaoCTe;
                            if (veiculoVinculado.Tipo == "T")
                            {
                                veiculoVinculado.TipoProprietario = ((string)jVeiculoVinculado.TipoProprietario).ToEnum<Dominio.Enumeradores.TipoProprietarioVeiculo>();
                                veiculoVinculado.Proprietario = repCliente.BuscarPorCPFCNPJ(double.Parse(Utilidades.String.OnlyNumbers((string)jVeiculoVinculado.Proprietario.Codigo)));
                                veiculoVinculado.RNTRC = ((string)jVeiculoVinculado.RNTRC).ToInt();

                                if (veiculoVinculado.Proprietario == null)
                                    throw new ControllerException(Localization.Resources.Veiculos.Veiculo.ObrigatorioInformarProprietarioQuandoVeiculoForDeTerceiro);

                                if (veiculoVinculado.RNTRC == 0 && ConfiguracaoEmbarcador.Pais == TipoPais.Brasil)
                                    throw new ControllerException(Localization.Resources.Veiculos.Veiculo.NecessarioInformarRNTRCCorretamente);
                            }

                            if (veiculo.Empresa != null)
                            {
                                veiculoVinculado.Empresa = veiculo.Empresa;

                                if (configuracao.Pais != TipoPais.Brasil)
                                    veiculoVinculado.Estado = veiculo.Empresa?.Localidade?.Estado ?? null;
                            }

                            veiculoVinculado.CentroResultado = veiculo.CentroResultado;
                            veiculoVinculado.TipoVeiculo = "1";

                            int modeloReboque = ((string)jVeiculoVinculado.ModeloVeicularCarga.Codigo).ToInt();
                            if (modeloReboque > 0)
                                veiculoVinculado.ModeloVeicularCarga = new Dominio.Entidades.Embarcador.Cargas.ModeloVeicularCarga() { Codigo = modeloReboque };
                            else
                            {
                                if (veiculoVinculado.ModeloVeicularCarga == null)
                                    veiculoVinculado.ModeloVeicularCarga = veiculo.ModeloVeicularCarga;
                            }

                            if (inserir)
                                repVeiculo.Inserir(veiculoVinculado, Auditado);
                            else
                                repVeiculo.Atualizar(veiculoVinculado, Auditado);

                            if (veiculoMotorista != null)
                            {
                                if (!inserir)
                                {
                                    Servicos.Auditoria.Auditoria.Auditar(Auditado, veiculoVinculado, $"Removido motorista principal.", unitOfWork);
                                    repVeiculoMotorista.DeletarMotoristaPrincipal(veiculoVinculado.Codigo);
                                }

                                Dominio.Entidades.Embarcador.Veiculos.VeiculoMotorista VeiculoMotoristaPrincipal = new Dominio.Entidades.Embarcador.Veiculos.VeiculoMotorista
                                {
                                    CPF = veiculoMotorista.CPF,
                                    Motorista = veiculoMotorista,
                                    Nome = veiculoMotorista.Nome,
                                    Veiculo = veiculoVinculado,
                                    Principal = true
                                };
                                repVeiculoMotorista.Inserir(VeiculoMotoristaPrincipal);
                            }

                            if (veiculo?.FuncionarioResponsavel != null)
                                Servicos.Auditoria.Auditoria.Auditar(Auditado, veiculoVinculado, null, "Alterado funcionario Responsavel no veiculo", unitOfWork);

                            veiculo.VeiculosVinculados.Add(veiculoVinculado);

                            if (veiculo.ModeloVeicularCarga == null)
                                veiculo.ModeloVeicularCarga = veiculoVinculado.ModeloVeicularCarga;
                        }
                        else if (veiculoVinculado.TipoVeiculo == "0")
                            throw new ControllerException(string.Format(Localization.Resources.Veiculos.Veiculo.NaoEPossivelVincularVeiculoDoTipoTracaoAUmVeiculoDoTipoTracao, veiculoVinculado.Placa));
                    }
                }

                if (!ValidarCamposReferenteCIOT(veiculo, unitOfWork, out string erroValidacaoCIOT))
                    throw new ControllerException(erroValidacaoCIOT);

                SalvarConfiguracoesTarget(veiculo);

                repVeiculo.Atualizar(veiculo, Auditado);

                if (TipoServicoMultisoftware == AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiTMS)
                {
                    Dominio.Entidades.Embarcador.Veiculos.HistoricoVeiculoVinculo historicoVeiculoVinculo = new Dominio.Entidades.Embarcador.Veiculos.HistoricoVeiculoVinculo();
                    historicoVeiculoVinculo = repHistoricoVeiculoVinculo.BuscarPorVeiculo(veiculo.Codigo);

                    if (historicoVeiculoVinculo == null)
                    {
                        historicoVeiculoVinculo = new Dominio.Entidades.Embarcador.Veiculos.HistoricoVeiculoVinculo()
                        {
                            Veiculo = veiculo,
                            DataHora = DateTime.Now,
                            Usuario = Usuario,
                            KmRodado = veiculo.KilometragemAtual,
                            KmAtualModificacao = 0,
                            DiasVinculado = 0
                        };
                        repHistoricoVeiculoVinculo.Inserir(historicoVeiculoVinculo);
                    }

                    Dominio.Entidades.Embarcador.Veiculos.HistoricoVeiculoVinculoCentroResultado historicoVeiculoVinculoCentroResultado = new Dominio.Entidades.Embarcador.Veiculos.HistoricoVeiculoVinculoCentroResultado
                    {
                        HistoricoVeiculoVinculo = historicoVeiculoVinculo,
                        CentroResultado = veiculo.CentroResultado,
                        DataHora = DateTime.Now,
                    };

                    repHistoricoVeiculoVinculoCentroResultado.Inserir(historicoVeiculoVinculoCentroResultado, Auditado);
                }

                Servicos.Embarcador.Veiculo.Veiculo.AtualizarIntegracoes(unitOfWork, veiculo);

                new Servicos.Embarcador.Integracao.SemParar.ConsultaCadastroVeiculo(unitOfWork, TipoServicoMultisoftware).GerarIntegracaoCadastroVeiculo(veiculo);

                ProcessarCadastroVeiculo(veiculo, false, unitOfWork);
                BloquearOutrosCadastroVeiculos(veiculo, unitOfWork);
                DesbloquearOutrosCadastroVeiculos(veiculo, unitOfWork);
                AtualizarResponsavelVeiculo(veiculo, unitOfWork);

                servFrota.AtualizarFrotaMotoristaVeiculo(veiculo);

                Servicos.Embarcador.Veiculo.VeiculoHistorico.InserirHistoricoVeiculo(veiculo, situacaoAnterior, MetodosAlteracaoVeiculo.Atualizar_VeiculoController, this.Usuario, unitOfWork);
                if (situacaoAnterior != veiculo.Ativo)
                    Servicos.Embarcador.Veiculo.Veiculo.AlterarSituacaoVeiculo(veiculo, veiculo.Ativo ? Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoVeiculo.Disponivel : Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoVeiculo.Indisponivel, unitOfWork, Auditado, string.Empty);

                unitOfWork.CommitChanges();

                //Replica modelo veicular para veiculos com mesma placa
                if (veiculo.ModeloVeicularCarga != null)
                {
                    List<Dominio.Entidades.Veiculo> listasVeiculos = repVeiculo.BuscarListaPorPlaca(veiculo.Placa);
                    foreach (Dominio.Entidades.Veiculo veic in listasVeiculos)
                    {
                        if (veic.Codigo != veiculo.Codigo)
                        {
                            veic.ModeloVeicularCarga = veiculo.ModeloVeicularCarga;
                            repVeiculo.Atualizar(veic);
                        }
                    }
                }

                return new JsonpResult(true);
            }
            catch (BaseException excecao)
            {
                unitOfWork.Rollback();
                return new JsonpResult(false, true, excecao.Message);
            }
            catch (Exception ex)
            {
                unitOfWork.Rollback();
                Servicos.Log.TratarErro(ex);
                return new JsonpResult(false, Localization.Resources.Gerais.Geral.OcorreuFalhaAoAtualizar);
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        [AllowAuthenticate]
        [AcceptVerbs("POST", "GET")]
        public async Task<IActionResult> BaixarQrCode()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                int codigo = Request.GetIntParam("Codigo");
                Repositorio.Veiculo repositorioVeiculo = new Repositorio.Veiculo(unitOfWork);
                Dominio.Entidades.Veiculo veiculo = repositorioVeiculo.BuscarPorCodigo(codigo);

                if (veiculo == null)
                    return new JsonpResult(false, true, Localization.Resources.Gerais.Geral.NenhumRegistroEncontrado);

                byte[] pdf = Servicos.Embarcador.Veiculo.Veiculo.ObterPdfQRCodeVeiculo(veiculo);

                return Arquivo(pdf, "application/pdf", $"QR Code {veiculo.Placa}.pdf");
            }
            catch (Exception excecao)
            {
                Servicos.Log.TratarErro(excecao);
                return new JsonpResult(false, Localization.Resources.Veiculos.Veiculo.OcorreuUmaFalhaAoBaixarQrCode);
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        [AllowAuthenticate]
        public async Task<IActionResult> BaixarTodosQrCode()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);
            TipoArquivo tipoArquivo = Request.GetEnumParam("TipoArquivo", TipoArquivo.PDF);

            try
            {
                bool somenteDisponiveis = Request.GetBoolParam("SomenteDisponveis");
                Dominio.ObjetosDeValor.Embarcador.Veiculos.FiltroPesquisaVeiculo filtrosPesquisa = ObterFiltrosPesquisa(unitOfWork);
                Repositorio.Veiculo repositorioVeiculo = new Repositorio.Veiculo(unitOfWork);
                int totalRegistros = 0;

                if (!somenteDisponiveis)
                    totalRegistros = repositorioVeiculo.ContarConsultaEmbarcador(filtrosPesquisa);
                else
                    totalRegistros = repositorioVeiculo.ContarConsultaEmbarcadorSomenteDisponiveis(filtrosPesquisa);

                if (totalRegistros == 0)
                    return new JsonpResult(false, Localization.Resources.Veiculos.Veiculo.NenhumVeiculoEncontrado);

                Servicos.Embarcador.Arquivo.ControleGeracaoArquivo servicoArquivo = new Servicos.Embarcador.Arquivo.ControleGeracaoArquivo(unitOfWork);
                Dominio.Entidades.Embarcador.Arquivo.ControleGeracaoArquivo controleGeracaoArquivo = servicoArquivo.Adicionar("QR Code Veiculos", Usuario, tipoArquivo);

                string stringConexao = _conexao.StringConexao;

                Task.Factory.StartNew(() => BaixarTodosQrCode(stringConexao, filtrosPesquisa, somenteDisponiveis, controleGeracaoArquivo));

                return new JsonpResult(true);
            }
            catch (Exception excecao)
            {
                Servicos.Log.TratarErro(excecao);
                return new JsonpResult(false, string.Format(Localization.Resources.Veiculos.Veiculo.OcorreuUmaFalhaAoBaixarArquivoDosQrCodeDosVeiculos, tipoArquivo.ObterDescricao()));
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        [AllowAuthenticate]
        public async Task<IActionResult> BuscaCombustivelAbastecimento()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);
            try
            {
                int codigo = 0;
                int.TryParse(Request.Params("Codigo"), out codigo);

                Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoAbastecimento tipoAbastecimento;
                Enum.TryParse(Request.Params("TipoAbastecimento"), out tipoAbastecimento);

                Repositorio.Veiculo repVeiculo = new Repositorio.Veiculo(unitOfWork);
                Dominio.Entidades.Produto produto = repVeiculo.BuscarCombustivelAbastecimento(tipoAbastecimento);
                Dominio.Entidades.Veiculo veiculo = repVeiculo.BuscarPorCodigo(codigo);

                if (produto != null)
                {
                    var retorno = new
                    {
                        Codigo = produto.Codigo,
                        Descricao = produto.Descricao,
                        TipoVeiculo = veiculo?.DescricaoTipoVeiculo ?? ""
                    };
                    return new JsonpResult(retorno);
                }
                else
                {
                    var retorno = new
                    {
                        Codigo = 0,
                        Descricao = "",
                        TipoVeiculo = veiculo?.DescricaoTipoVeiculo ?? ""
                    };
                    return new JsonpResult(retorno);
                }
                //return new JsonpResult(null);
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);
                return new JsonpResult(false, Localization.Resources.Veiculos.Veiculo.OcorreuUmaFalhaAoBuscarCombustivelParaAbastecimento);
            }
            finally
            {
                unitOfWork.Dispose();
            }

        }

        [AllowAuthenticate]
        public async Task<IActionResult> BuscaEquipamentoPadrao()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);
            try
            {
                int codigo = 0;
                int.TryParse(Request.Params("Codigo"), out codigo);

                Repositorio.Veiculo repVeiculo = new Repositorio.Veiculo(unitOfWork);
                Dominio.Entidades.Veiculo veiculo = repVeiculo.BuscarPorCodigo(codigo);
                if (veiculo != null && veiculo.Equipamentos != null && veiculo.Equipamentos.Count() > 0)
                {
                    Dominio.Entidades.Embarcador.Veiculos.Equipamento equipamento = veiculo.Equipamentos.Where(c => c.EquipamentoAceitaAbastecimento == true)?.FirstOrDefault() ?? null;
                    if (equipamento != null)
                    {
                        var retorno = new
                        {
                            Codigo = equipamento.Codigo,
                            Descricao = equipamento.Descricao,
                            Horimetro = equipamento.Horimetro
                        };
                        return new JsonpResult(retorno);
                    }
                    else
                        return new JsonpResult(null);
                }
                else
                    return new JsonpResult(null);
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);
                return new JsonpResult(false, Localization.Resources.Veiculos.Veiculo.OcorreuUmaFalhaAoBuscarCombustivelPadraoDoVeiculo);
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        [AllowAuthenticate]
        public async Task<IActionResult> BuscarVeiculoDoMotorista()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);
            try
            {
                int codigo = 0;
                int.TryParse(Request.Params("Codigo"), out codigo);

                Repositorio.Veiculo repVeiculo = new Repositorio.Veiculo(unitOfWork);

                Dominio.Entidades.Veiculo veiculo = repVeiculo.BuscarPorMotorista(codigo);
                var retorno = new
                {
                    Codigo = veiculo?.Codigo ?? 0,
                    Placa = veiculo?.Descricao ?? ""
                };
                return new JsonpResult(retorno);
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);
                return new JsonpResult(false, Localization.Resources.Veiculos.Veiculo.OcorreuUmaFalhaAoBuscarVeiculoDoMotorista);
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        [AllowAuthenticate]
        public async Task<IActionResult> BuscaMotoristaConjunto()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);
            try
            {
                int codigo = Request.GetIntParam("Codigo");

                Repositorio.Veiculo repVeiculo = new Repositorio.Veiculo(unitOfWork);
                Repositorio.Embarcador.Veiculos.VeiculoMotorista repositorioVeiculoMotorista = new Repositorio.Embarcador.Veiculos.VeiculoMotorista(unitOfWork);

                Dominio.Entidades.Usuario motorista = null;

                Dominio.Entidades.Veiculo veiculoEquipamento = repVeiculo.BuscarPorEquipamento(codigo);
                if (veiculoEquipamento != null)
                {
                    motorista = repositorioVeiculoMotorista.BuscarMotoristaPrincipal(veiculoEquipamento.Codigo);
                    if (motorista == null)
                    {
                        Dominio.Entidades.Veiculo veiculoTracao = repVeiculo.BuscarPorReboque(veiculoEquipamento.Codigo);
                        if (veiculoTracao != null)
                            motorista = repositorioVeiculoMotorista.BuscarMotoristaPrincipal(veiculoEquipamento.Codigo);
                    }
                }

                var retorno = new
                {
                    CodigoMotorista = motorista?.Codigo ?? 0,
                    Motorista = motorista?.Descricao ?? ""
                };

                return new JsonpResult(retorno);
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);
                return new JsonpResult(false, Localization.Resources.Veiculos.Veiculo.OcorreuUmaFalhaAoBuscarMotoristaDoConjunto);
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        [AllowAuthenticate]
        public async Task<IActionResult> ValidarEquipamentoPorCodigo()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);
            try
            {
                int codigo = Request.GetIntParam("Codigo");

                Repositorio.Veiculo repVeiculo = new Repositorio.Veiculo(unitOfWork);
                Repositorio.Embarcador.Veiculos.VeiculoMotorista repositorioVeiculoMotorista = new Repositorio.Embarcador.Veiculos.VeiculoMotorista(unitOfWork);

                Dominio.Entidades.Veiculo veiculoEquipamento = repVeiculo.BuscarPorEquipamento(codigo);

                if (veiculoEquipamento != null)
                {
                    var retorno = new
                    {
                        CodigoEquipamento = codigo,
                        Mensagem = string.Format(Localization.Resources.Veiculos.Veiculo.EquipamentoJaInformadoOutroVeiculo, veiculoEquipamento.Placa_Formatada)
                    };
                    return new JsonpResult(retorno);
                }

                return new JsonpResult(null);

            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);
                return new JsonpResult(false, Localization.Resources.Veiculos.Veiculo.OcorreuFalhaValidarEquipamento);
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        [AllowAuthenticate]
        public async Task<IActionResult> BuscaCombustivelPadrao()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);
            try
            {
                int codigo = 0;
                int.TryParse(Request.Params("Codigo"), out codigo);

                Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoAbastecimento tipoAbastecimento;
                Enum.TryParse(Request.Params("TipoAbastecimento"), out tipoAbastecimento);

                Repositorio.Veiculo repVeiculo = new Repositorio.Veiculo(unitOfWork);
                Repositorio.Embarcador.Veiculos.Equipamento repEquipamento = new Repositorio.Embarcador.Veiculos.Equipamento(unitOfWork);

                Dominio.Entidades.Produto produto = repVeiculo.BuscarCombustivelPadrao(codigo, tipoAbastecimento);
                Dominio.Entidades.Veiculo veiculo = repVeiculo.BuscarPorCodigo(codigo);
                Dominio.Entidades.Embarcador.Veiculos.Equipamento equipamento = veiculo != null && veiculo.Equipamentos != null && veiculo.Equipamentos.Count > 0 ? veiculo.Equipamentos.FirstOrDefault() : null;


                if (produto != null)
                {
                    var retorno = new
                    {
                        Codigo = produto.Codigo,
                        Descricao = produto.Descricao,
                        TipoVeiculo = veiculo?.DescricaoTipoVeiculo ?? "",
                        CodigoEquipamento = equipamento?.Codigo ?? 0,
                        DescricaoEquipamento = equipamento?.Descricao ?? ""
                    };
                    return new JsonpResult(retorno);
                }
                else
                {
                    var retorno = new
                    {
                        Codigo = 0,
                        Descricao = "",
                        TipoVeiculo = veiculo?.DescricaoTipoVeiculo ?? "",
                        CodigoEquipamento = equipamento?.Codigo ?? 0,
                        DescricaoEquipamento = equipamento?.Descricao ?? ""
                    };
                    return new JsonpResult(retorno);
                }
                //return new JsonpResult(null);
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);
                return new JsonpResult(false, Localization.Resources.Veiculos.Veiculo.OcorreuUmaFalhaAoBuscarCombustivelPadraoDoVeiculo);
            }
            finally
            {
                unitOfWork.Dispose();
            }

        }

        public async Task<IActionResult> BuscarPorCodigo()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                var repositorioVeiculo = new Repositorio.Veiculo(unitOfWork);
                var repositorioVeiculoCurral = new Repositorio.VeiculoCurral(unitOfWork);
                var repositorioVeiculoAnexo = new Repositorio.Embarcador.Veiculos.VeiculoAnexo(unitOfWork);
                var repositorioVeiculoMotorista = new Repositorio.Embarcador.Veiculos.VeiculoMotorista(unitOfWork);
                var repositorioVeiculoRotasFrete = new Repositorio.Embarcador.Veiculos.VeiculoRotasFrete(unitOfWork);
                var repositorioCorVeiculo = new Repositorio.Embarcador.Veiculos.CorVeiculo(unitOfWork);
                var servicoLicencaVeiculo = new Servicos.Embarcador.Veiculo.LicencaVeiculo(unitOfWork, TipoServicoMultisoftware);
                var repositorioVeiculoLiberacaoGR = new Repositorio.Embarcador.Veiculos.VeiculoLiberacaoGR(unitOfWork);

                int codigo = Request.GetIntParam("Codigo");
                var veiculo = repositorioVeiculo.BuscarPorCodigo(codigo);
                var anexos = repositorioVeiculoAnexo.BuscarPorCodigoVeiculo(codigo);
                var currais = repositorioVeiculoCurral.BuscarPorVeiculo(codigo);
                var motorista = repositorioVeiculoMotorista.BuscarMotoristaPrincipal(codigo);
                var motoristasSecundarios = repositorioVeiculoMotorista.BuscarVeiculoMotoristasSecundarios(codigo);
                var veiculoRotasFrete = repositorioVeiculoRotasFrete.BuscarPorVeiculo(codigo);
                var veiculosLiberacaoGR = repositorioVeiculoLiberacaoGR.BuscarPorCodigoVeiculo(codigo);
                var dadosRastreamento = await ObterDadosRastreadorAsync(veiculo, unitOfWork);

                var liberacoesGR = veiculosLiberacaoGR.Select(p => new { p.Codigo, p.Descricao, p.Numero, DataEmissao = p.DataEmissao.HasValue ? p.DataEmissao.Value.ToString("dd/MM/yyyy") : "", DataVencimento = p.DataVencimento.HasValue ? p.DataVencimento.Value.ToString("dd/MM/yyyy") : "", Licenca = new { p.Licenca?.Codigo, p.Licenca?.Descricao } }).ToList();

                bool validarCamposReferenteCIOT = false;
                if ((veiculo.Tipo == "T") && (veiculo.Proprietario != null))
                {
                    Dominio.Entidades.Embarcador.Pessoas.ModalidadeTransportadoraPessoas modalidadeTransportadoraPessoas = Servicos.Embarcador.CIOT.CIOT.ObterModalidadeTransportador(veiculo.Proprietario, unitOfWork);
                    validarCamposReferenteCIOT = modalidadeTransportadoraPessoas?.GerarCIOT ?? false;
                }

                // Campo "veiculo.Cor" foi descontinuado, adicionado tratamento para compatibilidade de informações.
                if (veiculo.CorVeiculo == null && !string.IsNullOrEmpty(veiculo.Cor))
                    veiculo.CorVeiculo = repositorioCorVeiculo.ConsultarPorDescricao(veiculo.Cor);

                var entidade = new
                {
                    veiculo.Codigo,
                    Status = veiculo.Ativo ? "A" : "I",
                    veiculo.Placa,
                    veiculo.SituacaoCadastro,
                    veiculo.Tara,
                    veiculo.Tipo,
                    veiculo.TipoRodado,
                    CapacidadeQuilo = veiculo.CapacidadeKG.ToString("n0"),
                    CapacidadeM3 = veiculo.CapacidadeM3,
                    veiculo.Renavam,
                    veiculo.TipoVeiculo,
                    veiculo.TipoCarroceria,
                    LocalAtualFisicoDoVeiculo = new { Codigo = veiculo.LocalAtualFisicoDoVeiculo?.Codigo ?? 0, Descricao = veiculo.LocalAtualFisicoDoVeiculo?.Descricao ?? "" },
                    ValidarCamposReferenteCIOT = validarCamposReferenteCIOT,
                    CPFMotorista = new { Codigo = motorista?.CPF_Formatado ?? string.Empty, Descricao = motorista?.CPF_Formatado ?? string.Empty },
                    CPF = motorista?.CPF_Formatado ?? string.Empty,
                    CodigoMotorista = motorista?.Codigo ?? 0,
                    NomeMotorista = motorista?.Nome ?? string.Empty,
                    Motoristas = (
                        from obj in motoristasSecundarios
                        select new
                        {
                            Codigo = obj.Codigo,
                            CPF = obj.Motorista?.CPF_Formatado ?? string.Empty,
                            CodigoMotorista = obj.Motorista?.Codigo ?? 0,
                            Nome = obj.Motorista?.Nome ?? string.Empty
                        }
                    ).ToList(),
                    LiberacoesGR = liberacoesGR,
                    Estado = veiculo.Estado.Sigla,
                    LocalidadeEmplacamento = veiculo.LocalidadeEmplacamento != null ? new { Codigo = veiculo.LocalidadeEmplacamento.Codigo, Descricao = veiculo.LocalidadeEmplacamento.Descricao } : null,
                    TipoProprietario = veiculo.TipoProprietario,
                    RNTRC = veiculo.RNTRC.ToString().PadLeft(8, '0'),
                    veiculo.CIOT,
                    veiculo.ContaCIOT,
                    veiculo.TipoChavePIX,
                    veiculo.ChavePIXCIOT,
                    veiculo.AgenciaCIOT,
                    ValorFrete = veiculo.ValorFreteCIOT,
                    ValorAdiantamento = veiculo.ValorAdiantamentoCIOT,
                    FormaPagamento = veiculo.FormaPagamentoCIOT.GetHashCode(),
                    TipoPagamento = veiculo.TipoPagamentoCIOT,
                    DataVencimento = veiculo.DataVencimentoCIOT?.ToString("dd/MM/yyyy") ?? string.Empty,
                    CNPJInstituicaoPagamento = veiculo.CNPJInstituicaoPagamentoCIOT,
                    ValorValePedagio = veiculo.ValorValePedagio > 0 ? veiculo.ValorValePedagio.ToString("n2") : "",
                    veiculo.NumeroCompraValePedagio,
                    ResponsavelValePedagio = veiculo.ResponsavelValePedagio != null ? new { Codigo = veiculo.ResponsavelValePedagio.CPF_CNPJ, Descricao = veiculo.ResponsavelValePedagio.Descricao } : new { Codigo = (double)0, Descricao = "" },
                    FornecedorValePedagio = veiculo.FornecedorValePedagio != null ? new { Codigo = veiculo.FornecedorValePedagio.CPF_CNPJ, Descricao = veiculo.FornecedorValePedagio.Descricao } : new { Codigo = (double)0, Descricao = "" },
                    ObservacaoCTe = veiculo.ObservacaoCTe,
                    Proprietario = new { Codigo = veiculo.Proprietario != null ? veiculo.Proprietario.CPF_CNPJ : 0, Descricao = veiculo.Proprietario != null ? veiculo.Proprietario.Descricao : "" },
                    Locador = new { Codigo = veiculo.Locador != null ? veiculo.Locador.CPF_CNPJ : 0, Descricao = veiculo.Locador != null ? veiculo.Locador.Descricao : "" },
                    Empresa = veiculo.Empresa != null ? new { Codigo = veiculo.Empresa.Codigo, Descricao = veiculo.Empresa.RazaoSocial, Tipo = veiculo.Empresa.Tipo } : null,
                    CompraValePedagio = veiculo.Empresa?.CompraValePedagio ?? false,
                    GrupoPessoa = veiculo.GrupoPessoas != null ? new { Codigo = veiculo.GrupoPessoas.Codigo, Descricao = veiculo.GrupoPessoas.Descricao } : null,
                    SegmentoVeiculo = veiculo.SegmentoVeiculo != null ? new { Codigo = veiculo.SegmentoVeiculo.Codigo, Descricao = veiculo.SegmentoVeiculo.Descricao } : null,
                    ModeloVeicularCarga = new { Codigo = veiculo.ModeloVeicularCarga != null ? veiculo.ModeloVeicularCarga.Codigo : 0, Descricao = veiculo.ModeloVeicularCarga != null ? veiculo.ModeloVeicularCarga.Descricao : "" },
                    ModeloCarroceria = new { Codigo = veiculo.ModeloCarroceria?.Codigo ?? 0, Descricao = veiculo.ModeloCarroceria?.Descricao },
                    FilialCarregamento = new { Codigo = veiculo.FilialCarregamento?.Codigo ?? 0, Descricao = veiculo.FilialCarregamento?.Descricao ?? string.Empty },
                    ObrigatorioInformarDataValidadeAdicionalCarroceria = veiculo?.ModeloCarroceria?.ObrigatorioInformarDataValidadeAdicionalCarroceria ?? false,
                    DataValidadeAdicionalCarroceria = veiculo.DataValidadeAdicionalCarroceria?.ToString("dd/MM/yyyy") ?? string.Empty,
                    VeiculosVinculados = (
                        from p in veiculo.VeiculosVinculados
                        select new
                        {
                            p.Codigo,
                            Status = p.Ativo ? "A" : "I",
                            p.Placa,
                            p.Tara,
                            p.Tipo,
                            p.TipoRodado,
                            CapacidadeQuilo = p.CapacidadeKG,
                            CapacidadeM3 = p.CapacidadeM3,
                            p.TipoCarroceria,
                            Renavam = p.Renavam != null ? p.Renavam : "",
                            Estado = p.Estado.Sigla,
                            LocalidadeEmplacamento = new { Codigo = p.LocalidadeEmplacamento != null ? p.LocalidadeEmplacamento.Codigo : 0, Descricao = p.LocalidadeEmplacamento != null ? p.LocalidadeEmplacamento.Descricao : "" },
                            TipoProprietario = p.TipoProprietario,
                            RNTRC = p.RNTRC.ToString().PadLeft(8, '0'),
                            MarcaVeiculo = new { Codigo = p.Marca != null ? p.Marca.Codigo : 0, Descricao = p.Marca != null ? p.Marca.Descricao : "" },
                            ModeloVeiculo = new { Codigo = p.Modelo != null ? p.Modelo.Codigo : 0, Descricao = p.Modelo != null ? p.Modelo.Descricao : "" },
                            ModeloCarroceria = new { Codigo = p.ModeloCarroceria?.Codigo ?? 0, Descricao = p.ModeloCarroceria?.Descricao },
                            ObservacaoCTe = p.ObservacaoCTe != null ? p.ObservacaoCTe : "",
                            Proprietario = new { Codigo = p.Proprietario != null ? p.Proprietario.CPF_CNPJ : 0, Descricao = p.Proprietario != null ? p.Proprietario.Nome : "" },
                            ModeloVeicularCarga = new { Codigo = p.ModeloVeicularCarga != null ? p.ModeloVeicularCarga.Codigo : 0, Descricao = p.ModeloVeicularCarga != null ? p.ModeloVeicularCarga.Descricao : "" },
                        }
                    ).ToList(),
                    Licencas = (
                        from licencaVeiculo in veiculo.LicencasVeiculo
                        select servicoLicencaVeiculo.ObterDetalhes(licencaVeiculo)
                    ).ToList(),
                    veiculo.Ativo,
                    MarcaVeiculo = new { Codigo = veiculo.Marca != null ? veiculo.Marca.Codigo : 0, Descricao = veiculo.Marca != null ? veiculo.Marca.Descricao : "" },
                    ModeloVeiculo = new { Codigo = veiculo.Modelo != null ? veiculo.Modelo.Codigo : 0, Descricao = veiculo.Modelo != null ? veiculo.Modelo.Descricao : "" },
                    veiculo.Chassi,
                    DataAquisicao = veiculo.DataCompra != null ? veiculo.DataCompra.Value.ToString("dd/MM/yyyy") : "",
                    DataValidadeANTT = veiculo.DataValidadeANTT?.ToDateString() ?? "",
                    DataUltimoChecklist = veiculo.DataUltimoChecklist.HasValue ? veiculo.DataUltimoChecklist.Value.ToString("dd/MM/yyyy") : "",
                    DataValidadeGerenciadoraRisco = veiculo.DataValidadeGerenciadoraRisco.HasValue ? veiculo.DataValidadeGerenciadoraRisco.Value.ToString("dd/MM/yyyy") : "",
                    DataValidadeLiberacaoSeguradora = veiculo.DataValidadeLiberacaoSeguradora.HasValue ? veiculo.DataValidadeLiberacaoSeguradora.Value.ToString("dd/MM/yyyy") : "",
                    veiculo.ValorAquisicao,
                    veiculo.CapacidadeTanque,
                    veiculo.AnoFabricacao,
                    veiculo.AnoModelo,
                    veiculo.ValorContainerAverbacao,
                    veiculo.NumeroMotor,
                    veiculo.NumeroCartaoValePedagio,
                    veiculo.NumeroCartaoAbastecimento,
                    GarantiaEscalonada = veiculo.DataVencimentoGarantiaEscalonada != null ? veiculo.DataVencimentoGarantiaEscalonada.Value.ToString("dd/MM/yyyy") : "",
                    GarantiaPlena = veiculo.DataVencimentoGarantiaPlena != null ? veiculo.DataVencimentoGarantiaPlena.Value.ToString("dd/MM/yyyy") : "",
                    NumeroContrato = veiculo.Contrato,
                    NumeroFrota = veiculo.NumeroFrota,
                    CodigoIntegracao = veiculo.CodigoIntegracao,
                    veiculo.Observacao,
                    veiculo.PossuiRastreador,
                    veiculo.NaoIntegrarOpentech,
                    veiculo.PossuiControleDisponibilidade,
                    veiculo.PossuiTravaQuintaDeRoda,
                    veiculo.PossuiImobilizador,
                    veiculo.FormaDeducaoValePedagio,
                    veiculo.AtivarConsultarAbastecimentoAngelLira,
                    veiculo.NumeroEquipamentoRastreador,

                    veiculo.TipoFrota,
                    Anexos = (
                        from obj in anexos
                        select new
                        {
                            obj.Codigo,
                            obj.Descricao,
                            obj.NomeArquivo,
                            TipoAnexoVeiculo = obj.TipoAnexoVeiculo.ObterDescricao(),
                        }
                    ).ToList(),
                    TipoComunicacaoRastreador = new
                    {
                        Codigo = veiculo.TipoComunicacaoRastreador?.Codigo ?? 0,
                        Descricao = veiculo.TipoComunicacaoRastreador?.Descricao ?? string.Empty
                    },
                    Currais = (
                        from o in currais
                        select new
                        {
                            Largura = o.Largura > 0 ? o.Largura.ToString("n2") : string.Empty,
                            Comprimento = o.Comprimento > 0 ? o.Comprimento.ToString("n2") : string.Empty,
                            o.NumeroCurral
                        }
                    ).ToList(),
                    TecnologiaRastreador = new
                    {
                        Codigo = veiculo.TecnologiaRastreador?.Codigo ?? 0,
                        Descricao = veiculo.TecnologiaRastreador?.Descricao ?? string.Empty
                    },
                    Transportadoras = (
                        from p in veiculo.Empresas
                        select new
                        {
                            p.Codigo,
                            CNPJ = p.CNPJ_Formatado,
                            Nome = p.RazaoSocial
                        }
                    ).ToList(),
                    Equipamentos = (
                        from p in veiculo.Equipamentos
                        select new
                        {
                            p.Codigo,
                            p.Descricao,
                            p.Numero
                        }
                    ).ToList(),
                    veiculo.KilometragemAtual,
                    Cor = veiculo.CorVeiculo != null ? new { veiculo.CorVeiculo.Codigo, veiculo.CorVeiculo.Descricao } : null,
                    veiculo.PossuiTagValePedagio,
                    veiculo.NaoComprarValePedagio,
                    veiculo.NaoComprarValePedagioRetorno,
                    veiculo.NaoValidarIntegracaoParaFilaCarregamento,
                    FuncionarioResponsavel = veiculo.FuncionarioResponsavel != null ? new { veiculo.FuncionarioResponsavel.Codigo, Descricao = veiculo.FuncionarioResponsavel.Nome } : null,
                    TipoPlotagem = veiculo.TipoPlotagem != null ? new { veiculo.TipoPlotagem.Codigo, veiculo.TipoPlotagem.Descricao } : null,
                    veiculo.TipoCombustivel,
                    veiculo.TipoCarreta,
                    veiculo.TipoMaterialGaiola,
                    veiculo.TipoMaterialPiso,
                    veiculo.TipoSistemaElevacao,
                    veiculo.QuantidadeCurrais,
                    veiculo.ViradaHodometro,
                    PermiteAdicionarLocalizador = veiculo.ModeloVeicularCarga?.ModeloVeicularAceitaLocalizador ?? false,
                    KilometragemVirada = veiculo.KilometragemVirada > 0 ? veiculo.KilometragemVirada.ToString("n0") : string.Empty,
                    CentroResultado = veiculo.CentroResultado != null ? new { veiculo.CentroResultado.Codigo, veiculo.CentroResultado.Descricao } : null,
                    veiculo.ModoCompraValePedagioTarget,
                    CapacidadeMaximaTanque = veiculo.CapacidadeMaximaTanque > 0 ? veiculo.CapacidadeMaximaTanque.ToString("n2") : string.Empty,
                    PossuiLocalizador = veiculo.PossuiLocalizador,
                    DadosRastreamento = dadosRastreamento,
                    DadosAutorizacao = ObterDadosAutorizacaoCadastroVeiculo(veiculo, unitOfWork),
                    veiculo.PosicaoReboque,
                    veiculo.MotivoBloqueio,
                    BloquearVeiculo = veiculo.VeiculoBloqueado,
                    PadraoEmissao = veiculo?.PadraoEmissao ?? "",
                    FatorEmissao = veiculo?.FatorEmissao ?? "",
                    Terminal = veiculo.NumeroEquipamentoRastreador,
                    RotasFrete = (
                        from p in veiculoRotasFrete
                        select new
                        {
                            Codigo = p.RotaFrete?.Codigo ?? 0,
                            Descricao = p.RotaFrete?.Descricao ?? string.Empty
                        }
                    ).ToList(),
                    PaletizadoGeracaoFrota = veiculo.Paletizado,
                    veiculo.VeiculoAlugado,
                    TipoIntegracaoValePedagio = veiculo.TiposIntegracaoValePedagio.Select(o => o.Tipo).ToList(),
                    veiculo.CIOTEmitidoContratanteDiferenteEmbarcador,
                    DataInicialCIOTTemporario = veiculo.DataInicialCIOTTemporario.HasValue ? veiculo.DataInicialCIOTTemporario.Value.ToString("dd/MM/yyyy") : "",
                    DataFinalCIOTTemporario = veiculo.DataFinalCIOTTemporario.HasValue ? veiculo.DataFinalCIOTTemporario.Value.ToString("dd/MM/yyyy") : "",
                    TagSemParar = veiculo.TagSemParar ?? string.Empty,
                    veiculo.CapacidadeTanqueArla,
                    veiculo.PossuiTelemetria,
                    veiculo.VeiculoUtilizadoNoTransporteDeFrotasDedicadasOuFidelizadas,
                    DataVigencia = veiculo.DataVigencia?.ToString("dd/MM/yyyy") ?? string.Empty,
                    EmpresaFilial = veiculo.EmpresaFilial != null ? new { Codigo = veiculo.EmpresaFilial.Codigo, Descricao = veiculo.EmpresaFilial.RazaoSocial, Tipo = veiculo.EmpresaFilial.Tipo } : null,
                    EmpresaVeiculoCooperado = veiculo.EmpresaVeiculoCooperado != null ? new { Codigo = veiculo.EmpresaVeiculoCooperado.Codigo, Descricao = veiculo.EmpresaVeiculoCooperado.RazaoSocial, Tipo = veiculo.EmpresaVeiculoCooperado.Tipo } : null,
                    veiculo.VeiculoCooperado

                };

                return new JsonpResult(entidade);
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);
                return new JsonpResult(false, Localization.Resources.Veiculos.Veiculo.OcorreuUmaFalhaAoBuscarPorCodigo);
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        public async Task<IActionResult> BuscarPorPlaca()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);
            try
            {
                string placa = Request.Params("Placa");
                int codigoEmpresa = int.Parse(Request.Params("Empresa"));

                Repositorio.Veiculo repVeiculo = new Repositorio.Veiculo(unitOfWork);
                Repositorio.Embarcador.Veiculos.VeiculoMotorista repositorioVeiculoMotorista = new Repositorio.Embarcador.Veiculos.VeiculoMotorista(unitOfWork);

                Dominio.Entidades.Veiculo veiculo = repVeiculo.BuscarPorPlaca(codigoEmpresa, placa);

                if (veiculo == null)
                    return new JsonpResult(null);

                Dominio.Entidades.Usuario motorista = repositorioVeiculoMotorista.BuscarMotoristaPrincipal(veiculo.Codigo);

                var entidade = new
                {
                    veiculo.Codigo,
                    Status = veiculo.Ativo ? "A" : "I",
                    veiculo.Placa,
                    veiculo.Tara,
                    veiculo.Tipo,
                    veiculo.TipoRodado,
                    CapacidadeQuilo = veiculo.CapacidadeKG,
                    CapacidadeM3 = veiculo.CapacidadeM3,
                    veiculo.TipoVeiculo,
                    veiculo.TipoCarroceria,
                    veiculo.Renavam,

                    CPFMotorista = motorista?.CPF_Formatado ?? string.Empty,
                    CPF = motorista?.CPF_Formatado ?? string.Empty,
                    CodigoMotorista = motorista?.Codigo ?? 0,
                    NomeMotorista = motorista?.Nome ?? string.Empty,

                    Estado = veiculo.Estado.Sigla,
                    LocalidadeEmplacamento = veiculo.LocalidadeEmplacamento != null ? new { Codigo = veiculo.LocalidadeEmplacamento.Codigo, Descricao = veiculo.LocalidadeEmplacamento.Descricao } : null,
                    TipoProprietario = veiculo.TipoProprietario,
                    RNTRC = veiculo.RNTRC.ToString().PadLeft(8, '0'),
                    ObservacaoCTe = veiculo.ObservacaoCTe != null ? veiculo.ObservacaoCTe : "",
                    Proprietario = new { Codigo = veiculo.Proprietario != null ? veiculo.Proprietario.CPF_CNPJ : 0, Descricao = veiculo.Proprietario != null ? veiculo.Proprietario.Descricao : "" },
                    Empresa = new { Codigo = veiculo.Empresa != null ? veiculo.Empresa.Codigo : 0, Descricao = veiculo.Empresa != null ? veiculo.Empresa.RazaoSocial : "" },
                    GrupoPessoa = veiculo.GrupoPessoas != null ? new { Codigo = veiculo.GrupoPessoas.Codigo, Descricao = veiculo.GrupoPessoas.Descricao } : null,
                    SegmentoVeiculo = veiculo.SegmentoVeiculo != null ? new { Codigo = veiculo.SegmentoVeiculo.Codigo, Descricao = veiculo.SegmentoVeiculo.Descricao } : null,
                    ModeloVeicularCarga = new { Codigo = veiculo.ModeloVeicularCarga != null ? veiculo.ModeloVeicularCarga.Codigo : 0, Descricao = veiculo.ModeloVeicularCarga != null ? veiculo.ModeloVeicularCarga.Descricao : "" }
                };

                return new JsonpResult(entidade);
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);
                return new JsonpResult(false, Localization.Resources.Veiculos.Veiculo.OcorreuUmaFalhaAoBuscarPorPlaca);
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        [AllowAuthenticate]
        public async Task<IActionResult> BuscarVarios()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);
            try
            {
                Repositorio.Veiculo repVeiculo = new Repositorio.Veiculo(unitOfWork);

                List<int> codigos = Newtonsoft.Json.JsonConvert.DeserializeObject<List<int>>(Request.Params("Codigos"));

                List<Dominio.Entidades.Veiculo> veiculos = repVeiculo.BuscarPorCodigo(codigos.ToArray());

                var lista = (from o in veiculos
                             select new
                             {
                                 o.Codigo,
                                 CodigoModeloVeicular = o.ModeloVeicularCarga?.Codigo ?? 0,
                                 o.Placa,
                                 ModeloVeicularCarga = o.ModeloVeicularCarga?.Descricao ?? string.Empty,
                                 o.CapacidadeKG,
                                 o.CapacidadeM3,
                             }).ToList();

                return new JsonpResult(lista);
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);
                return new JsonpResult(false, Localization.Resources.Veiculos.Veiculo.OcorreuUmaFalhaAoBuscarDetalhes);
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        public async Task<IActionResult> ConfiguracaoImportacao()
        {
            List<ConfiguracaoImportacao> configuracoes = ConfiguracaoImportacaoVeiculos();

            return new JsonpResult(configuracoes.ToList());
        }

        public async Task<IActionResult> ExcluirPorCodigo()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);
            try
            {
                unitOfWork.Start();

                int codigo = Request.GetIntParam("Codigo");
                Repositorio.Veiculo repVeiculo = new Repositorio.Veiculo(unitOfWork);
                Dominio.Entidades.Veiculo veiculo = repVeiculo.BuscarPorCodigo(codigo, true);

                if (veiculo == null)
                    return new JsonpResult(false, true, Localization.Resources.Gerais.Geral.NaoFoiPossivelEncontrarRegistro);

                repVeiculo.Deletar(veiculo, Auditado);

                unitOfWork.CommitChanges();
                return new JsonpResult(true);
            }
            catch (Exception ex)
            {
                unitOfWork.Rollback();
                if (ExcessaoPorPossuirDependeciasNoBanco(ex))
                    return new JsonpResult(false, true, Localization.Resources.Gerais.Geral.NaoFoiPossivelExcluirRegistro);
                else
                {
                    Servicos.Log.TratarErro(ex);
                    return new JsonpResult(false, Localization.Resources.Gerais.Geral.OcorreuFalhaAoExcluir);
                }
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        public async Task<IActionResult> Importar()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                Repositorio.Embarcador.Configuracao.ConfiguracaoTMS repConfiguracao = new Repositorio.Embarcador.Configuracao.ConfiguracaoTMS(unitOfWork);
                Dominio.Entidades.Embarcador.Configuracao.ConfiguracaoTMS configuracao = repConfiguracao.BuscarConfiguracaoPadrao();

                unitOfWork.Start();

                List<Dominio.Entidades.Veiculo> veiculos = new List<Dominio.Entidades.Veiculo>();

                RetornoImportacao retorno = Servicos.Embarcador.Importacao.Importacao.PreencherImportacaoManual(Request, veiculos, ((dados) =>
                {
                    Servicos.Embarcador.Veiculo.VeiculoImportar servicoVeiculoImportar = new Servicos.Embarcador.Veiculo.VeiculoImportar(unitOfWork, TipoServicoMultisoftware, Empresa, dados, configuracao);

                    return servicoVeiculoImportar.ObterVeiculoImportar();
                }));


                if (retorno == null)
                    return new JsonpResult(false, Localization.Resources.Gerais.Geral.OcorreuUmaFalhaAoImportarArquivo);

                int totalRegistrosImportados = 0;
                dynamic parametro = Newtonsoft.Json.JsonConvert.DeserializeObject<dynamic>(Request.Params("Parametro"));
                bool permiteInserir = (bool)parametro.Inserir;
                bool permiteAtualizar = (bool)parametro.Atualizar;

                Repositorio.Veiculo repositorioVeiculo = new Repositorio.Veiculo(unitOfWork);
                Repositorio.Embarcador.Veiculos.VeiculoMotorista repositorioVeiculoMotorista = new Repositorio.Embarcador.Veiculos.VeiculoMotorista(unitOfWork);

                foreach (Dominio.Entidades.Veiculo veiculo in veiculos)
                {
                    if (veiculos.Exists(x => x.Placa == veiculo.Placa && x.Empresa?.Codigo == veiculo.Empresa?.Codigo && x != veiculo))
                        throw new ControllerException(string.Format(Localization.Resources.Veiculos.Veiculo.VeiculoEstaDuplicadoNaPlanilhaParaOCNPJ, veiculo.Placa_Formatada, veiculo.Empresa?.CNPJ_Formatado));

                    if ((veiculo.Codigo > 0) && permiteAtualizar)
                    {
                        repositorioVeiculo.Atualizar(veiculo, Auditado);
                        totalRegistrosImportados++;
                    }
                    else if ((veiculo.Codigo == 0) && permiteInserir)
                    {
                        Dominio.Entidades.Embarcador.Veiculos.VeiculoMotorista veiculoMotorista = null;
                        if (veiculo.Motoristas != null && veiculo.Motoristas.Count == 1)
                        {
                            veiculoMotorista = veiculo.Motoristas[0];
                            veiculo.Motoristas = null;
                        }

                        repositorioVeiculo.Inserir(veiculo, Auditado);

                        if (veiculoMotorista != null)
                            repositorioVeiculoMotorista.Inserir(veiculoMotorista);

                        totalRegistrosImportados++;
                    }
                }

                unitOfWork.CommitChanges();

                retorno.Importados = totalRegistrosImportados;

                return new JsonpResult(retorno);
            }
            catch (ControllerException ex)
            {
                unitOfWork.Rollback();
                Servicos.Log.TratarErro(ex);
                return new JsonpResult(false, ex.Message);
            }
            catch (Exception ex)
            {
                unitOfWork.Rollback();
                Servicos.Log.TratarErro(ex);
                return new JsonpResult(false, Localization.Resources.Gerais.Geral.OcorreuUmaFalhaAoImportarArquivo);
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        public async Task<IActionResult> ImportarBK()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);
            try
            {
                unitOfWork.Start();

                // Repositorios
                Repositorio.Veiculo repVeiculo = new Repositorio.Veiculo(unitOfWork);
                Repositorio.Empresa repEmpresa = new Repositorio.Empresa(unitOfWork);
                Repositorio.Embarcador.Cargas.ModeloVeicularCarga repModeloVeicularCarga = new Repositorio.Embarcador.Cargas.ModeloVeicularCarga(unitOfWork);
                Servicos.Veiculo serVeiculo = new Servicos.Veiculo(unitOfWork);
                // Configuração de importacao
                List<ConfiguracaoImportacao> configuracoes = ConfiguracaoImportacaoVeiculos();

                // Erro de campo
                string erro = string.Empty;

                // Lista integrada em cada linha
                List<Dictionary<string, dynamic>> dadosLinhas = new List<Dictionary<string, dynamic>>();

                // Entidade para importacao
                List<Dominio.Entidades.Veiculo> veiculos = new List<Dominio.Entidades.Veiculo>();

                // Chama serviço de importação
                var retorno = Servicos.Embarcador.Importacao.Importacao.ImportarInformacoes(Request, configuracoes, ref veiculos, ref dadosLinhas, out erro, ((dicionario) =>
                {
                    // Pesquisa por placa antes de criar uma nova
                    string placa = dicionario["Placa"].ToString();

                    var veic = repVeiculo.BuscarPorPlaca(placa);
                    return veic;
                }));

                if (retorno == null && !string.IsNullOrWhiteSpace(erro))
                    return new JsonpResult(false, true, erro);
                else if (retorno == null)
                    return new JsonpResult(false, Localization.Resources.Gerais.Geral.OcorreuUmaFalhaAoImportarArquivo);

                if (veiculos.Count() != dadosLinhas.Count())
                    return new JsonpResult(false, Localization.Resources.Veiculos.Veiculo.OcorreuUmaFalhaAoImportarArquivoInformacoesIncorretas);

                // Insere registros
                int contador = 0;
                for (var i = 0; i < veiculos.Count(); i++)
                {
                    Dictionary<string, dynamic> linha = dadosLinhas[i];
                    Dominio.Entidades.Veiculo veiculo = veiculos[i];

                    if (serVeiculo.ValidarPlaca(veiculo.Placa, unitOfWork))
                    {
                        // Transportadora
                        string cnpj = "";
                        try { cnpj = (string)linha["CNPJTransportadora"]; }
                        catch (Exception ex)
                        {
                            Servicos.Log.TratarErro("Erro ao buscar CNPJTransportadora do dicionário.");
                            Servicos.Log.TratarErro(ex);
                            continue;
                        }
                        Dominio.Entidades.Empresa tranportadora = null;
                        if (TipoServicoMultisoftware == AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiCTe)
                            tranportadora = this.Empresa;
                        else
                            tranportadora = repEmpresa.BuscarPorCNPJ(cnpj);

                        // Modelo Veiculos
                        string codIntegracao = "";
                        try { cnpj = linha["ModeloVeicular"]; }
                        catch (Exception ex)
                        {
                            Servicos.Log.TratarErro("Erro ao buscar ModeloVeicular do dicionário.");
                            Servicos.Log.TratarErro(ex);
                            continue;
                        }
                        Dominio.Entidades.Embarcador.Cargas.ModeloVeicularCarga modelo = repModeloVeicularCarga.buscarPorCodigoIntegracao(codIntegracao);

                        // Entidade
                        if (tranportadora != null)
                        {
                            veiculo.Renavam = "12345678901";
                            veiculo.Empresa = tranportadora;
                            veiculo.Estado = tranportadora.Localidade.Estado;
                            if (modelo != null)
                            {
                                veiculo.ModeloVeicularCarga = modelo;
                                veiculo.CapacidadeKG = (int)modelo.CapacidadePesoTransporte;
                            }
                            veiculo.TipoRodado = "00";
                            veiculo.TipoCarroceria = "02";
                            bool situacaoAnterior = veiculo.Ativo;
                            veiculo.Ativo = true;
                            veiculo.TipoVeiculo = "0";
                            veiculo.Tipo = "T";

                            if (veiculo.Codigo > 0)
                            {
                                Servicos.Embarcador.Veiculo.VeiculoHistorico.InserirHistoricoVeiculo(veiculo, situacaoAnterior, MetodosAlteracaoVeiculo.ImportarBK_VeiculoController, this.Usuario, unitOfWork);
                                repVeiculo.Atualizar(veiculo);
                            }
                            else
                                repVeiculo.Inserir(veiculo);
                            contador++;
                        }
                    }
                }

                unitOfWork.CommitChanges();

                // Seta Retorno
                var retornoJson = new
                {
                    Total = veiculos.Count(),
                    Importados = contador,
                };

                return new JsonpResult(retornoJson);
            }
            catch (Exception ex)
            {
                unitOfWork.Rollback();
                Servicos.Log.TratarErro(ex);
                return new JsonpResult(false, Localization.Resources.Gerais.Geral.OcorreuUmaFalhaAoImportarArquivo);
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        [AllowAuthenticate]
        public async Task<IActionResult> ObterDetalhesPorVeiculo()
        {
            Repositorio.UnitOfWork unidadeDeTrabalho = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                int codigoVeiculo = Request.GetIntParam("Codigo");

                Repositorio.Veiculo repVeiculo = new Repositorio.Veiculo(unidadeDeTrabalho);
                Repositorio.Embarcador.Veiculos.VeiculoMotorista repositorioVeiculoMotorista = new Repositorio.Embarcador.Veiculos.VeiculoMotorista(unidadeDeTrabalho);

                Dominio.Entidades.Veiculo veiculo = repVeiculo.BuscarPorCodigo(codigoVeiculo);

                if (veiculo == null)
                    return new JsonpResult(false, Localization.Resources.Veiculos.Veiculo.VeiculoNaoEncontrado);

                Dominio.Entidades.Veiculo veiculoPai = veiculo;

                if (veiculo.TipoVeiculo == "1")
                    veiculoPai = repVeiculo.BuscarVeiculoPai(codigoVeiculo);

                if (veiculoPai == null)
                    veiculoPai = veiculo;

                List<Dominio.Entidades.Embarcador.Veiculos.VeiculoMotorista> motoristas = repositorioVeiculoMotorista.BuscarTodos(veiculoPai.Codigo);
                Dominio.Entidades.Usuario motoristaPrincipal = motoristas.Where(o => o.Principal).Select(o => o.Motorista).FirstOrDefault();

                var retorno = new
                {
                    CodigoVeiculo = veiculoPai.Codigo,
                    Placa = ObterPlacaConcatenadaFrota(veiculoPai),
                    Estado = veiculoPai.Estado.Sigla,
                    RENAVAM = veiculoPai.Renavam,
                    Rodado = veiculoPai.DescricaoTipoRodado,
                    Carroceria = veiculoPai.DescricaoTipoCarroceria,
                    Propriedade = veiculoPai.DescricaoTipo,
                    NumeroFrota = veiculoPai.NumeroFrota,
                    VeiculosVinculados = (
                        from obj in veiculoPai.VeiculosVinculados
                        select new
                        {
                            CodigoVeiculo = obj.Codigo,
                            Placa = ObterPlacaConcatenadaFrota(obj),
                            Estado = obj.Estado.Sigla,
                            RENAVAM = obj.Renavam,
                            Rodado = obj.DescricaoTipoRodado,
                            Carroceria = obj.DescricaoTipoCarroceria,
                            Propriedade = obj.DescricaoTipo,
                            ModeloVeicularCarga = obj.ModeloVeicularCarga?.Descricao
                        }
                    ).ToList(),
                    Motorista = motoristaPrincipal == null ? null : new
                    {
                        motoristaPrincipal.Codigo,
                        motoristaPrincipal.Nome,
                        CPF = motoristaPrincipal.CPF_Formatado
                    },
                    Motoristas = (from obj in motoristas
                                  select new
                                  {
                                      CodigoMotorista = obj.Motorista?.Codigo ?? 0,
                                      CPF = obj.Motorista?.CPF_Formatado ?? string.Empty,
                                      Nome = obj.Motorista?.Nome ?? string.Empty,
                                      obj.Principal
                                  }).ToList(),
                    ModeloVeicularCargaTracao = new { Codigo = veiculo.ModeloVeicularCarga?.Codigo ?? 0, Descricao = veiculo.ModeloVeicularCarga?.Descricao ?? string.Empty },
                    ModeloVeicularCargaReboque = (from obj in veiculo.VeiculosVinculados
                                                  select new
                                                  {
                                                      CodigoReboque = obj.Codigo,
                                                      ModeloVeicularReboque = new { Codigo = veiculo.VeiculosVinculados.FirstOrDefault()?.ModeloVeicularCarga?.Codigo ?? 0, Descricao = veiculo.VeiculosVinculados.FirstOrDefault()?.ModeloVeicularCarga?.Descricao ?? string.Empty }
                                                  }).ToList().FirstOrDefault()
                };

                return new JsonpResult(retorno);
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);

                return new JsonpResult(false, Localization.Resources.Veiculos.Veiculo.OcorreuUmaFalhaAoObterDetalhesDoVeiculo);
            }
            finally
            {
                unidadeDeTrabalho.Dispose();
            }
        }

        [AllowAuthenticate]
        public async Task<IActionResult> ObterDetalhesVeiculo()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);
            try
            {
                int codigo = Request.GetIntParam("Codigo");

                Repositorio.Veiculo repVeiculo = new Repositorio.Veiculo(unitOfWork);
                Repositorio.Embarcador.Veiculos.VeiculoMotorista repositorioVeiculoMotorista = new Repositorio.Embarcador.Veiculos.VeiculoMotorista(unitOfWork);
                Dominio.Entidades.Veiculo veiculo = repVeiculo.BuscarPorCodigo(codigo);

                if (veiculo == null)
                    return new JsonpResult(false, true, Localization.Resources.Veiculos.Veiculo.VeiculoNaoFoiEncontrado);

                List<Dominio.Entidades.Embarcador.Veiculos.VeiculoMotorista> motoristas = repositorioVeiculoMotorista.BuscarTodos(codigo);
                Dominio.Entidades.Usuario motoristaPrincipal = motoristas.Where(o => o.Principal).Select(o => o.Motorista).FirstOrDefault();
                Dominio.Entidades.Veiculo veiculoTracao = veiculo.VeiculosTracao.FirstOrDefault();
                List<Dominio.Entidades.Embarcador.Veiculos.VeiculoMotorista> motoristasTracao = veiculoTracao != null ? repositorioVeiculoMotorista.BuscarTodos(veiculoTracao.Codigo) : new List<Dominio.Entidades.Embarcador.Veiculos.VeiculoMotorista>();
                Dominio.Entidades.Usuario motoristaTracaoPrincipal = motoristasTracao.Where(o => o.Principal).Select(o => o.Motorista).FirstOrDefault();

                var retorno = new
                {
                    veiculo.Codigo,
                    veiculo.Placa,
                    veiculo.Descricao,
                    Reboque = string.Join(", ", veiculo.VeiculosVinculados.Select(o => o.Placa)),
                    Tracao = string.Join(", ", veiculo.VeiculosTracao.Select(o => o.Placa)),
                    CodigoTracao = veiculo.VeiculosTracao.FirstOrDefault()?.Codigo ?? 0,

                    CodigoMotorista = motoristaPrincipal?.Codigo ?? 0,
                    Motorista = motoristaPrincipal?.Descricao ?? string.Empty,
                    CPFMotorista = motoristaPrincipal?.CPF_Formatado ?? string.Empty,
                    Motoristas = (from obj in motoristas
                                  select new
                                  {
                                      CodigoMotorista = obj.Motorista?.Codigo ?? 0,
                                      CPF = obj.Motorista?.CPF_Formatado ?? string.Empty,
                                      Nome = obj.Motorista?.Nome ?? string.Empty,
                                      obj.Principal
                                  }).ToList(),
                    CodigoMotoristaTracao = motoristaTracaoPrincipal?.Codigo ?? 0,
                    MotoristaTracao = motoristaTracaoPrincipal?.Descricao ?? string.Empty,
                    CPFMotoristaTracao = motoristaTracaoPrincipal?.CPF_Formatado ?? string.Empty,
                    MotoristasTracao = (from obj in motoristasTracao
                                        select new
                                        {
                                            CodigoMotorista = obj.Motorista?.Codigo ?? 0,
                                            CPF = obj.Motorista?.CPF_Formatado ?? string.Empty,
                                            Nome = obj.Motorista?.Nome ?? string.Empty,
                                            obj.Principal
                                        }).ToList(),
                    ModeloVeicularCargaTracao = new { Codigo = veiculo.ModeloVeicularCarga?.Codigo ?? 0, Descricao = veiculo.ModeloVeicularCarga?.Descricao ?? string.Empty },
                    ModeloVeicularCargaReboque = (from obj in veiculo.VeiculosVinculados
                                                  select new
                                                  {
                                                      CodigoReboque = obj.Codigo,
                                                      ModeloVeicularReboque = new { Codigo = veiculo.VeiculosVinculados.FirstOrDefault().ModeloVeicularCarga.Codigo, Descricao = veiculo.VeiculosVinculados.FirstOrDefault().ModeloVeicularCarga.Descricao }
                                                  }).ToList().FirstOrDefault()
                };

                return new JsonpResult(retorno);
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);
                return new JsonpResult(false, Localization.Resources.Veiculos.Veiculo.OcorreuUmaFalhaAoObterDetalhesDoVeiculo);
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        [AllowAuthenticate]
        
        public async Task<IActionResult> Pesquisa()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {

                // Servicos.Embarcador.Frota.Frota servFrota = new Servicos.Embarcador.Frota.Frota(unitOfWork, TipoServicoMultisoftware, Cliente, WebServiceConsultaCTe);
                // servFrota.CriarFrotas();
                Repositorio.Embarcador.Configuracoes.ConfiguracaoGeralCarga repositorioConfiguracaoGeralCarga = new Repositorio.Embarcador.Configuracoes.ConfiguracaoGeralCarga(unitOfWork);
                Dominio.Entidades.Embarcador.Configuracoes.ConfiguracaoGeralCarga configuracaoGeralCarga = repositorioConfiguracaoGeralCarga.BuscarPrimeiroRegistro();

                Models.Grid.Grid grid = new Models.Grid.Grid(Request)
                {
                    header = new List<Models.Grid.Head>()
                };

                DateTime? dataAbastecimento = Request.GetNullableDateTimeParam("DataAbastecimento");
                bool somenteDisponiveis = Request.GetBoolParam("SomenteDisponveis");
                bool somenteEmEscala = Request.GetBoolParam("SomenteEmEscala");
                int.TryParse(Request.Params("TipoOperacao"), out int codigoTipoOperacao);
                Dominio.ObjetosDeValor.Embarcador.Veiculos.FiltroPesquisaVeiculo filtrosPesquisa = ObterFiltrosPesquisa(unitOfWork);

                grid.AdicionarCabecalho("Codigo", false);
                grid.AdicionarCabecalho("CodigoModeloVeicularCarga", false);
                grid.AdicionarCabecalho("ListaDiariaGerada", false);
                grid.AdicionarCabecalho("Renavam", false);
                grid.AdicionarCabecalho("Reboque", false);
                grid.AdicionarCabecalho("TipoPropriedade", false);
                grid.AdicionarCabecalho("ConjuntoPlacasComModeloVeicular", false);
                grid.AdicionarCabecalho("ConjuntoPlacasComModeloVeicularEFrota", false);
                grid.AdicionarCabecalho("ConjuntoPlacasSemModeloVeicular", false);
                grid.AdicionarCabecalho("CodigoSegmentoVeiculo", false);
                grid.AdicionarCabecalho("SegmentoVeiculo", false);
                grid.AdicionarCabecalho("CodigoMotorista", false);
                grid.AdicionarCabecalho("Descricao", false);
                grid.AdicionarCabecalho("CapacidadePesoTransporte", false);
                grid.AdicionarCabecalho("CapacidadeKG", false);
                grid.AdicionarCabecalho("CapacidadeM3", false);
                grid.AdicionarCabecalho("NomeMotorista", false);
                grid.AdicionarCabecalho(Localization.Resources.Veiculos.Veiculo.Placa, "Placa", 10, Models.Grid.Align.left, false);
                grid.AdicionarCabecalho(Localization.Resources.Veiculos.Veiculo.NumeroFrota, "NumeroFrota", 10, Models.Grid.Align.left, true, true, true);
                grid.AdicionarCabecalho(Localization.Resources.Veiculos.Veiculo.Motorista, "Motorista", 26, Models.Grid.Align.left, false, true, true, false, TipoServicoMultisoftware == AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiTMS ? true : false);

                if (
                    (TipoServicoMultisoftware == AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiTMS) ||
                    (TipoServicoMultisoftware == AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiCTe && !configuracaoGeralCarga.UtilizarProgramacaoCarga) ||
                    (TipoServicoMultisoftware == AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiNFe)
                )
                    grid.AdicionarCabecalho("Empresa", false);
                else
                    grid.AdicionarCabecalho(Localization.Resources.Veiculos.Veiculo.Transportador, "Empresa", 32, Models.Grid.Align.left, true, true, true);

                grid.AdicionarCabecalho(Localization.Resources.Veiculos.Veiculo.ModeloCarga, "ModeloVeicularCarga", 15, Models.Grid.Align.left, true, true, true);
                grid.AdicionarCabecalho(Localization.Resources.Veiculos.Veiculo.TipoDeVeiculo, "TipoVeiculo", 10, Models.Grid.Align.left, false);
                grid.AdicionarCabecalho(Localization.Resources.Veiculos.Veiculo.Propriedade, "DescricaoTipo", 10, Models.Grid.Align.left, false, true, true);
                grid.AdicionarCabecalho("CPFMotorista", false);
                grid.AdicionarCabecalho("Estado", false);
                grid.AdicionarCabecalho("RNTRC", false);
                grid.AdicionarCabecalho("ConjuntoFrota", false);
                grid.AdicionarCabecalho("UltimoKMAbastecimento", false);
                grid.AdicionarCabecalho("Tracao", false);
                grid.AdicionarCabecalho("CodigoTracao", false);
                grid.AdicionarCabecalho("CodigoEmpresa", false);
                grid.AdicionarCabecalho("KMAtual", false);
                grid.AdicionarCabecalho("CodigoCentroResultado", false);
                grid.AdicionarCabecalho("CentroResultado", false);
                grid.AdicionarCabecalho("VeiculosVinculados", false);
                grid.AdicionarCabecalho("CodigosVeiculosVinculados", false);
                grid.AdicionarCabecalho(Localization.Resources.Veiculos.Veiculo.ProprietarioTerceiro, "Proprietario", 17, Models.Grid.Align.left, false, true, true);
                grid.AdicionarCabecalho("DescricaoComMarcaModelo", false);
                if (ConfiguracaoEmbarcador.UtilizarAlcadaAprovacaoVeiculo && !(somenteDisponiveis || filtrosPesquisa.FiltrarCadastrosAprovados))
                    grid.AdicionarCabecalho(Localization.Resources.Gerais.Geral.Aprovacao, "SituacaoCadastro", 10, Models.Grid.Align.left, false, true, true);
                else
                    grid.AdicionarCabecalho("SituacaoCadastro", false);

                grid.AdicionarCabecalho("EmpresaDescricao", false);
                grid.AdicionarCabecalho("Tipo", false);
                grid.AdicionarCabecalho("CNPJEmpresa", false);
                grid.AdicionarCabecalho("TipoFrotaDescricao", false);
                grid.AdicionarCabecalho("Arla32", false);

                Repositorio.Veiculo repositorioVeiculo = new Repositorio.Veiculo(unitOfWork);
                Repositorio.Embarcador.Pedidos.TipoOperacao repositorioTipoOperacao = new Repositorio.Embarcador.Pedidos.TipoOperacao(unitOfWork);

                Dominio.ObjetosDeValor.Embarcador.Consulta.ParametroConsulta parametrosConsulta = grid.ObterParametrosConsulta(ObterPropriedadeOrdenar);
                Dominio.Entidades.Embarcador.Pedidos.TipoOperacao tipoOperacao = repositorioTipoOperacao.BuscarPorCodigo(codigoTipoOperacao);
                List<Dominio.Entidades.Veiculo> listaVeiculo = new List<Dominio.Entidades.Veiculo>();
                int totalRegistros = 0;

                if (somenteDisponiveis)
                {
                    filtrosPesquisa.FiltrarCadastrosAprovados = ConfiguracaoEmbarcador.UtilizarAlcadaAprovacaoVeiculo;
                    totalRegistros = repositorioVeiculo.ContarConsultaEmbarcadorSomenteDisponiveis(filtrosPesquisa);

                    if (totalRegistros > 0)
                        listaVeiculo = repositorioVeiculo.ConsultarEmbarcadorSomenteDisponiveis(filtrosPesquisa, parametrosConsulta);
                }
                else if (somenteEmEscala)
                {
                    totalRegistros = repositorioVeiculo.ContarConsultaEmbarcadorSomenteEmEscala(filtrosPesquisa);

                    if (totalRegistros > 0)
                        listaVeiculo = repositorioVeiculo.ConsultarEmbarcadorSomenteEmEscala(filtrosPesquisa, parametrosConsulta);
                }
                else
                {
                    if (TipoServicoMultisoftware == AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiTMS && codigoTipoOperacao > 0 && tipoOperacao.ConfiguracaoTipoPropriedadeVeiculo != null)
                    {
                        totalRegistros = repositorioVeiculo.ContarConsultaEmbarcadorPorTipoPropriedadeVeiculoDoTipoOperacao(filtrosPesquisa, tipoOperacao);

                        if (totalRegistros > 0)
                            listaVeiculo = repositorioVeiculo.ConsultarEmbarcadorPorTipoPropriedadeVeiculoDoTipoOperacao(filtrosPesquisa, parametrosConsulta, tipoOperacao);
                    }
                    else
                    {
                        totalRegistros = repositorioVeiculo.ContarConsultaEmbarcador(filtrosPesquisa);

                        if (totalRegistros > 0)
                            listaVeiculo = repositorioVeiculo.ConsultarEmbarcador(filtrosPesquisa, parametrosConsulta);
                    }
                }

                var listaVeiculoRetornar = (
                    from veiculo in listaVeiculo
                    select ObterDetalhesVeiculo(veiculo, dataAbastecimento, unitOfWork)
                ).ToList();

                grid.setarQuantidadeTotal(totalRegistros);
                grid.AdicionaRows(listaVeiculoRetornar);

                return new JsonpResult(grid);
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);
                return new JsonpResult(false, Localization.Resources.Gerais.Geral.OcorreuFalhaAoConsultar);
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        [AllowAuthenticate]
        public async Task<IActionResult> PesquisaVeiculosSugeridosHUB()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);
            // TODO: Precisa de revisão para integração HUB
            try
            {
                Repositorio.Embarcador.Configuracoes.ConfiguracaoGeralCarga repositorioConfiguracaoGeralCarga = new Repositorio.Embarcador.Configuracoes.ConfiguracaoGeralCarga(unitOfWork);
                Dominio.Entidades.Embarcador.Configuracoes.ConfiguracaoGeralCarga configuracaoGeralCarga = repositorioConfiguracaoGeralCarga.BuscarPrimeiroRegistro();

                Models.Grid.Grid grid = new Models.Grid.Grid(Request)
                {
                    header = new List<Models.Grid.Head>()
                };

                grid.AdicionarCabecalho(Localization.Resources.Veiculos.Veiculo.Transportador, "Empresa", 32, Models.Grid.Align.left, true, true, true);
                grid.AdicionarCabecalho(Localization.Resources.Veiculos.Veiculo.Placa, "Placa", 10, Models.Grid.Align.left, false);
                grid.AdicionarCabecalho(Localization.Resources.Veiculos.Veiculo.Motorista, "Motorista", 15, Models.Grid.Align.left, false);
                grid.AdicionarCabecalho(Localization.Resources.Veiculos.Veiculo.Reboque, "Reboque", 10, Models.Grid.Align.left, false);
                grid.AdicionarCabecalho("Local (última atualização)", "Local", 10, Models.Grid.Align.left, false);
                grid.AdicionarCabecalho("Próxima disponibilidade", "Placa", 10, Models.Grid.Align.left, false);
                grid.AdicionarCabecalho("Data Prevista de Chegada", "DataPrevista", 10, Models.Grid.Align.left, false);
                grid.AdicionarCabecalho("Status", "Status", 10, Models.Grid.Align.left, false);

                var dadosExemplo = new List<dynamic>
                {
                    new
                    {
                        Empresa = "Cooperativa",
                        Placa = "PFY0E96",
                        Motorista = "João Paulo",
                        Reboque = "CVHJ22",
                        Local = "Poços de Caldas - MG",
                        ProximaDisponibilidade = "Poços de Caldas - MG",
                        DataPrevista = "12/07/2025 10:00",
                        Status = "Escalado"
                    },
                    new
                    {
                        Empresa = "Transportes Rápidos Ltda",
                        Placa = "MNO1234",
                        Motorista = "Carlos Silva",
                        Reboque = "ABC456",
                        Local = "São Paulo - SP",
                        ProximaDisponibilidade = "Campinas - SP",
                        DataPrevista = "15/07/2025 08:30",
                        Status = "Disponível"
                    },
                    new
                    {
                        Empresa = "Entregas Express",
                        Placa = "XYZ7890",
                        Motorista = "Antônio Oliveira",
                        Reboque = "DEF789",
                        Local = "Rio de Janeiro - RJ",
                        ProximaDisponibilidade = "Niterói - RJ",
                        DataPrevista = "13/07/2025 14:15",
                        Status = "Em trânsito"
                    },
                    new
                    {
                        Empresa = "Logística Nacional",
                        Placa = "RST4567",
                        Motorista = "Marcos Pereira",
                        Reboque = "GHI123",
                        Local = "Belo Horizonte - MG",
                        ProximaDisponibilidade = "Contagem - MG",
                        DataPrevista = "14/07/2025 09:45",
                        Status = "Escalado"
                    },
                    new
                    {
                        Empresa = "Transportadora União",
                        Placa = "UVW8901",
                        Motorista = "José Santos",
                        Reboque = "JKL456",
                        Local = "Curitiba - PR",
                        ProximaDisponibilidade = "Londrina - PR",
                        DataPrevista = "16/07/2025 11:20",
                        Status = "Disponível"
                    }
                };

                int totalRegistros = dadosExemplo.Count;
                grid.setarQuantidadeTotal(totalRegistros);
                grid.AdicionaRows(dadosExemplo);

                return new JsonpResult(grid);
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);
                return new JsonpResult(false, Localization.Resources.Gerais.Geral.OcorreuFalhaAoConsultar);
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        [AllowAuthenticate]
        public async Task<IActionResult> PesquisaReboqueMovimentacaoDePlacas()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);
            try
            {
                string placa = Request.Params("PlacaReboque");
                if (!string.IsNullOrWhiteSpace(placa))
                    placa = placa.Replace("_", "");

                int tipoReboque = int.Parse(Request.Params("TipoReboque"));
                string frota = Request.Params("NumeroFrota");
                bool somentePendentes = bool.Parse(Request.Params("SomentePedentes"));

                Models.Grid.Grid grid = new Models.Grid.Grid(Request);
                grid.header = new List<Models.Grid.Head>();
                grid.AdicionarCabecalho("Codigo", false);
                grid.AdicionarCabecalho(Localization.Resources.Veiculos.Veiculo.Placa, "Placa", 25, Models.Grid.Align.left, true);
                grid.AdicionarCabecalho(Localization.Resources.Veiculos.Veiculo.NumeroFrota, "NumeroFrota", 15, Models.Grid.Align.right, true);
                grid.AdicionarCabecalho(Localization.Resources.Veiculos.Veiculo.TipoReboque, "Reboque", 30, Models.Grid.Align.left, true);
                grid.AdicionarCabecalho("ContemVinculo", false);
                grid.AdicionarCabecalho("PlacaVinculo", false);

                //string propOrdenar = grid.header[grid.indiceColunaOrdena].data;                
                string propOrdenar = "DataRemocaoVinculo";

                Repositorio.Veiculo repVeiculo = new Repositorio.Veiculo(unitOfWork);
                List<Dominio.Entidades.Veiculo> listaVeiculo = repVeiculo.ConsultarEmbarcadorMovimentosDePlacas(placa, tipoReboque, frota, somentePendentes, propOrdenar, grid.dirOrdena, grid.inicio, grid.limite);
                grid.setarQuantidadeTotal(repVeiculo.ConsultarEmbarcadorMovimentosDePlacas(placa, tipoReboque, frota, somentePendentes));

                var lista = (from p in listaVeiculo
                             select new
                             {
                                 Codigo = p.Codigo + "_" + "reboque",
                                 p.Placa,
                                 Reboque = p.ModeloVeicularCarga != null ? p.ModeloVeicularCarga.Descricao : string.Empty,
                                 NumeroFrota = !string.IsNullOrWhiteSpace(p.NumeroFrota) ? p.NumeroFrota : "",
                                 DT_RowClass = repVeiculo.ContemVinculoEmTracao(p) ? Dominio.ObjetosDeValor.Embarcador.Enumeradores.ClasseCorFundo.Warning(IntensidadeCor._100) : "",
                                 ContemVinculo = repVeiculo.ContemVinculoEmTracao(p),
                                 PlacaVinculo = repVeiculo.PlacaVinculoEmTracao(p)
                             }).ToList();
                grid.AdicionaRows(lista);

                return new JsonpResult(grid);
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);
                return new JsonpResult(false, Localization.Resources.Gerais.Geral.OcorreuFalhaAoConsultar);
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        [AllowAuthenticate]
        public async Task<IActionResult> PesquisaVeiculoGestaoCarga()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);
            try
            {
                string placa = Request.Params("Placa");
                int motorista = int.Parse(Request.Params("Motorista"));

                Models.Grid.Grid grid = new Models.Grid.Grid(Request);
                grid.header = new List<Models.Grid.Head>();
                grid.AdicionarCabecalho("Codigo", false);
                grid.AdicionarCabecalho("Renavam", false);
                grid.AdicionarCabecalho("CodigoMotorista", false);
                grid.AdicionarCabecalho(Localization.Resources.Veiculos.Veiculo.Placa, "Placa", 30, Models.Grid.Align.left, false);
                grid.AdicionarCabecalho(Localization.Resources.Veiculos.Veiculo.Reboque, "Reboque", 30, Models.Grid.Align.left, false);
                grid.AdicionarCabecalho(Localization.Resources.Veiculos.Veiculo.Motorista, "Motorista", 40, Models.Grid.Align.left, true);

                string propOrdenar = grid.header[grid.indiceColunaOrdena].data;
                if (propOrdenar == "Motorista")
                    propOrdenar += ".Nome";

                Repositorio.Embarcador.Configuracao.ConfiguracaoTMS repConfigurarEmbarcador = new Repositorio.Embarcador.Configuracao.ConfiguracaoTMS(unitOfWork);
                Dominio.Entidades.Embarcador.Configuracao.ConfiguracaoTMS configEmbarcador = repConfigurarEmbarcador.BuscarConfiguracaoPadrao();
                bool apenasTracao = false;
                if (!configEmbarcador.FiltrarBuscaVeiculosPorEmpresa)
                    apenasTracao = true;

                Repositorio.Veiculo repVeiculo = new Repositorio.Veiculo(unitOfWork);
                List<Dominio.Entidades.Veiculo> listaVeiculo = repVeiculo.ConsultarEmbarcadorGestaoCarga(placa, motorista, apenasTracao, propOrdenar, grid.dirOrdena, grid.inicio, grid.limite);
                grid.setarQuantidadeTotal(repVeiculo.ConsultarEmbarcadorGestaoCarga(placa, motorista, apenasTracao));

                var lista = (from veiculo in listaVeiculo
                             select ObterDetalhesVeiculoGestaoCarga(veiculo, unitOfWork));
                grid.AdicionaRows(lista);

                return new JsonpResult(grid);
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);
                return new JsonpResult(false, Localization.Resources.Gerais.Geral.OcorreuFalhaAoConsultar);
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        [AllowAuthenticate]
        public async Task<IActionResult> ValidarPneuVeiculo()
        {
            var unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                var codigoVeiculo = Request.GetIntParam("Veiculo");
                var placaReboque = Request.Params("Reboque");

                var repositorio = new Repositorio.Veiculo(unitOfWork);

                Dominio.Entidades.Veiculo veiculo = repositorio.BuscarPorCodigo(codigoVeiculo);
                if (veiculo != null && veiculo.Pneus != null && veiculo.Pneus.Count > 0)
                    return new JsonpResult(false, Localization.Resources.Veiculos.Veiculo.VeiculoSelecionadoPossuiPneuVinculadoFavorUtilizeTelaDeMovimentacaoDePlaca);
                veiculo = repositorio.BuscarPorPlaca(placaReboque);
                if (veiculo != null && veiculo.Pneus != null && veiculo.Pneus.Count > 0)
                    return new JsonpResult(false, Localization.Resources.Veiculos.Veiculo.ReboqueSelecionadoPossuiPneuVinculadoFavorUtilizeTelaDeMovimentacaoDePlaca);

                return new JsonpResult(true);
            }
            catch (Exception excecao)
            {
                Servicos.Log.TratarErro(excecao);

                return new JsonpResult(false, Localization.Resources.Veiculos.Veiculo.OcorreuUmaFalhaAoConsultarPneu);
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        [AllowAuthenticate]
        public async Task<IActionResult> VincularMotorista()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                int codigoVeiculo = Request.GetIntParam("Veiculo");
                int codigoMotorista = Request.GetIntParam("Motorista");

                Repositorio.Veiculo repVeiculo = new Repositorio.Veiculo(unitOfWork);
                Repositorio.Usuario repMotorista = new Repositorio.Usuario(unitOfWork);
                Repositorio.Embarcador.Veiculos.VeiculoMotorista repositorioVeiculoMotorista = new Repositorio.Embarcador.Veiculos.VeiculoMotorista(unitOfWork);

                Dominio.Entidades.Veiculo veiculo = repVeiculo.BuscarPorCodigo(codigoVeiculo, true);
                Dominio.Entidades.Usuario motorista = repMotorista.BuscarPorCodigo(codigoMotorista);

                if (veiculo == null)
                    return new JsonpResult(false, true, Localization.Resources.Veiculos.Veiculo.VeiculoNaoEncontrado);

                if (motorista == null)
                    return new JsonpResult(false, true, Localization.Resources.Veiculos.Veiculo.MotoristaNaoFoiEncontrado);

                Dominio.Entidades.Veiculo veiculoAnterior = repVeiculo.BuscarPorMotorista(codigoMotorista);

                unitOfWork.Start();

                if (veiculoAnterior != null && veiculoAnterior.Codigo != veiculo.Codigo)
                {
                    Dominio.Entidades.Embarcador.Veiculos.VeiculoMotorista veiculoMotoristaAnterior = repositorioVeiculoMotorista.BuscarVeiculoMotoristaPrincipal(veiculoAnterior.Codigo);
                    if (veiculoMotoristaAnterior != null)
                    {
                        repositorioVeiculoMotorista.Deletar(veiculoMotoristaAnterior);
                        Servicos.Auditoria.Auditoria.Auditar(Auditado, veiculoAnterior, null, Localization.Resources.Veiculos.Veiculo.RemoveuMotoristaPrincipalPeloVinculoDeMotorista + $" - {veiculoMotoristaAnterior?.Motorista.Nome ?? veiculoMotoristaAnterior.Nome}", unitOfWork);
                    }

                    Servicos.Embarcador.Veiculo.Veiculo.AtualizarIntegracoes(unitOfWork, veiculoAnterior);
                }

                Dominio.Entidades.Embarcador.Veiculos.VeiculoMotorista veiculoMotorista = repositorioVeiculoMotorista.BuscarVeiculoMotoristaPrincipal(veiculo.Codigo);
                if (veiculoMotorista == null)
                {
                    veiculoMotorista = new Dominio.Entidades.Embarcador.Veiculos.VeiculoMotorista();
                    veiculoMotorista.Veiculo = veiculo;
                }
                else
                    veiculoMotorista.Initialize();

                bool trocouMotorista = veiculoMotorista.Motorista?.Codigo != motorista?.Codigo;

                veiculoMotorista.Motorista = motorista;

                if (veiculoMotorista.Codigo > 0)
                {
                    repositorioVeiculoMotorista.Atualizar(veiculoMotorista);
                    List<Dominio.Entidades.Auditoria.HistoricoPropriedade> alteracoes = veiculoMotorista.GetChanges();

                    if (alteracoes.Count > 0)
                        Servicos.Auditoria.Auditoria.Auditar(Auditado, veiculo, alteracoes, Localization.Resources.Veiculos.Veiculo.AlterouMotoristaPrincipalPeloVinculoDeMotorista + $" - {motorista.Nome}", unitOfWork);
                }
                else
                {
                    veiculoMotorista.Principal = true;
                    repositorioVeiculoMotorista.Inserir(veiculoMotorista);
                    Servicos.Auditoria.Auditoria.Auditar(Auditado, veiculo, null, Localization.Resources.Veiculos.Veiculo.AdicionouMotoristaPrincipalPeloVinculoDeMotorista + $" - {motorista.Nome}", unitOfWork);
                }

                Servicos.Embarcador.Veiculo.Veiculo.AtualizarIntegracoes(unitOfWork, veiculo, veiculoMotorista.Motorista);

                if (trocouMotorista)
                    Servicos.Embarcador.Veiculo.Veiculo.AtualizarIntegracoesTrocaMotorista(veiculo, unitOfWork);

                unitOfWork.CommitChanges();

                return new JsonpResult(true);
            }
            catch (Exception excecao)
            {
                unitOfWork.Rollback();

                Servicos.Log.TratarErro(excecao);

                return new JsonpResult(false, Localization.Resources.Veiculos.Veiculo.OcorreuUmaFalhaAoVincularMotoristaAoVeiculo);
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        [AllowAuthenticate]
        public async Task<IActionResult> ObterConfiguracaoPadrao()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);
            try
            {
                Repositorio.Embarcador.Configuracoes.ConfiguracaoVeiculo repConfiguracaoVeiculo = new Repositorio.Embarcador.Configuracoes.ConfiguracaoVeiculo(unitOfWork);
                Dominio.Entidades.Embarcador.Configuracoes.ConfiguracaoVeiculo configuracaoVeiculo = repConfiguracaoVeiculo.BuscarConfiguracaoPadrao();

                var retorno = new
                {
                    ObrigatorioInformarAnoFabricacao = configuracaoVeiculo.ObrigatorioInformarAnoFabricacao
                };

                return new JsonpResult(retorno);
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);
                return new JsonpResult(false, Localization.Resources.Gerais.Geral.OcorreuFalhaAoBuscarConfiguracaoPadrao);
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        [AllowAuthenticate]
        public async Task<IActionResult> ObterConfiguracaoVeiculoSemParar()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);
            try
            {
                Repositorio.Embarcador.Configuracoes.IntegracaoSemParar repIntegracaoSemParar = new Repositorio.Embarcador.Configuracoes.IntegracaoSemParar(unitOfWork);
                Dominio.Entidades.Embarcador.Configuracoes.IntegracaoSemParar integracaoSemParar = repIntegracaoSemParar.BuscarPrimeira();

                var retorno = new
                {
                    ConsultarVeiculoPossuiCadastroSemParar = integracaoSemParar?.ConsultarSeVeiculoPossuiCadastro ?? false
                };

                return new JsonpResult(retorno);
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);
                return new JsonpResult(false, Localization.Resources.Gerais.Geral.OcorreuFalhaAoBuscarConfiguracaoPadrao);
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        [AllowAuthenticate]
        public async Task<IActionResult> ValidarRotasFreteVeiculo(CancellationToken cancellationToken)
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);
            try
            {
                int codigoVeiculo = Request.GetIntParam("CodigoVeiculo");
                int codigoCarga = Request.GetIntParam("CodigoCarga");

                Repositorio.Embarcador.Cargas.Carga repositorioCarga = new Repositorio.Embarcador.Cargas.Carga(unitOfWork, cancellationToken);
                Repositorio.Embarcador.Veiculos.VeiculoRotasFrete repositorioVeiculoRotasFrete = new Repositorio.Embarcador.Veiculos.VeiculoRotasFrete(unitOfWork);

                List<Dominio.Entidades.Embarcador.Veiculos.VeiculoRotasFrete> veiculosRotasFrete = repositorioVeiculoRotasFrete.BuscarPorVeiculo(codigoVeiculo);
                Dominio.Entidades.Embarcador.Cargas.Carga carga = await repositorioCarga.BuscarPorCodigoAsync(codigoCarga);

                if (veiculosRotasFrete.Count > 0 && carga != null)
                    if (!veiculosRotasFrete.Select(o => o.RotaFrete.Codigo).ToList().Contains(carga.Rota?.Codigo ?? 0))
                        throw new ControllerException(Localization.Resources.Veiculos.Veiculo.VeiculoInformadoPossuiRotasDeFretesCadastradasQueNaoFazemParteDaCargaFavorVerificar);

                return new JsonpResult(true);
            }
            catch (BaseException excecao)
            {
                unitOfWork.Rollback();
                return new JsonpResult(false, true, excecao.Message);
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);
                return new JsonpResult(false, "");
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        public async Task<IActionResult> ImportarVeiculos()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {

                List<Dominio.ObjetosDeValor.Embarcador.Importacao.ConfiguracaoImportacao> configuracoes = ObterConfiguracaoImportacaoVeiculos();
                List<Dictionary<string, dynamic>> dadosLinhas = new List<Dictionary<string, dynamic>>();

                Repositorio.Veiculo repositorioVeiculo = new Repositorio.Veiculo(unitOfWork);

                List<Dominio.Entidades.Veiculo> listaRetorno = new List<Dominio.Entidades.Veiculo>();

                Dominio.ObjetosDeValor.Embarcador.Importacao.RetornoImportacao retorno = Servicos.Embarcador.Importacao.Importacao.ImportarInformacoes(Request, configuracoes, ref listaRetorno, ref dadosLinhas, out string erro, ((dicionario) =>
                {
                    var placaBuscar = string.Empty;

                    if (dicionario.TryGetValue("Placa", out var placa))
                        placaBuscar = (string)placa;

                    if (string.IsNullOrWhiteSpace(placaBuscar))
                        throw new ImportacaoException("Placa não informada");

                    placaBuscar = placaBuscar.Replace("-", "").Trim().ToUpper();

                    Dominio.Entidades.Veiculo veiculo = repositorioVeiculo.BuscarPorPlaca(placaBuscar);

                    if (veiculo == null)
                        throw new ImportacaoException($"A placa informada ({placaBuscar}) é inválida");

                    return veiculo;
                }));

                if (!string.IsNullOrWhiteSpace(erro))
                    return new JsonpResult(false, true, erro);

                if (retorno == null)
                    return new JsonpResult(false, Localization.Resources.Pedidos.Pedido.OcorreuUmaFalhaAoImportarArquivo);

                retorno.Importados = listaRetorno.Count();

                retorno.Retorno = (from obj in listaRetorno
                                   select new
                                   {
                                       obj.Codigo,
                                       Placa = obj.Placa.ObterPlacaFormatada(),
                                       ModeloVeicularCarga = new { Codigo = obj.ModeloVeicularCarga != null ? obj.ModeloVeicularCarga.Codigo : 0, Descricao = obj.ModeloVeicularCarga != null ? obj.ModeloVeicularCarga.Descricao : "" },
                                       CapacidadeKG = obj.CapacidadeKG,
                                       CapacidadeM3 = obj.CapacidadeM3
                                   }).ToList();

                return new JsonpResult(retorno);
            }
            catch (ImportacaoException excecao)
            {
                return new JsonpResult(false, excecao.Message);
            }
            catch (Exception excecao)
            {
                Servicos.Log.TratarErro(excecao);
                return new JsonpResult(false, Localization.Resources.Pedidos.Pedido.OcorreuUmaFalhaAoImportarArquivo);
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        [AllowAuthenticate]
        public async Task<IActionResult> BuscarValePedagioAtivo()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                Repositorio.Embarcador.Frotas.ConfiguracaoValePedagio repConfiguracaoValePedagio = new Repositorio.Embarcador.Frotas.ConfiguracaoValePedagio(unitOfWork);

                List<Dominio.Entidades.Embarcador.Frotas.ConfiguracaoValePedagio> configuracaoValePedagios = repConfiguracaoValePedagio.BuscarPorSituacaoAtiva();

                if (configuracaoValePedagios == null || configuracaoValePedagios.Count == 0)
                    return new JsonpResult(false, true, "Nenhuma operadora de Vale Pedágio ativa.");

                var dynConfiguracaoValePedagios = new List<dynamic>();
                foreach (var valePedagio in configuracaoValePedagios)
                {
                    var dynConfiguracaoValePedagio = new
                    {
                        Codigo = valePedagio.TipoIntegracao,
                        Descricao = valePedagio.DescricaoTipoIntegracao
                    };
                    dynConfiguracaoValePedagios.Add(dynConfiguracaoValePedagio);
                }

                return new JsonpResult(dynConfiguracaoValePedagios);
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);
                return new JsonpResult(false, "Ocorreu uma falha ao buscar Vale Pedágio ativos.");
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        #endregion

        #region Métodos Privados

        private void PreencherVeiculo(Dominio.Entidades.Veiculo veiculo, Repositorio.UnitOfWork unitOfWork)
        {
            Repositorio.Usuario repMotorista = new Repositorio.Usuario(unitOfWork);
            Repositorio.Cliente repCliente = new Repositorio.Cliente(unitOfWork);
            Repositorio.Veiculo repVeiculo = new Repositorio.Veiculo(unitOfWork);
            Repositorio.Empresa repEmpresa = new Repositorio.Empresa(unitOfWork);
            Repositorio.Localidade repLocalidade = new Repositorio.Localidade(unitOfWork);
            Repositorio.Embarcador.Filiais.Filial repositorioFilial = new Repositorio.Embarcador.Filiais.Filial(unitOfWork);
            Repositorio.Embarcador.Veiculos.CorVeiculo repCorVeiculo = new Repositorio.Embarcador.Veiculos.CorVeiculo(unitOfWork);
            Repositorio.Embarcador.Pessoas.GrupoPessoas repGrupoPessoas = new Repositorio.Embarcador.Pessoas.GrupoPessoas(unitOfWork);
            Repositorio.Embarcador.Veiculos.TipoPlotagem repTipoPlotagem = new Repositorio.Embarcador.Veiculos.TipoPlotagem(unitOfWork);
            Repositorio.Embarcador.Cargas.TipoIntegracao repTipoIntegracao = new Repositorio.Embarcador.Cargas.TipoIntegracao(unitOfWork);
            Repositorio.Embarcador.Veiculos.SegmentoVeiculo repSegmentoVeiculo = new Repositorio.Embarcador.Veiculos.SegmentoVeiculo(unitOfWork);
            Repositorio.Embarcador.Veiculos.ModeloCarroceria repModeloCarroceria = new Repositorio.Embarcador.Veiculos.ModeloCarroceria(unitOfWork);
            Repositorio.Embarcador.Financeiro.CentroResultado repCentroResultado = new Repositorio.Embarcador.Financeiro.CentroResultado(unitOfWork);
            Repositorio.Embarcador.Veiculos.TecnologiaRastreador repTecnologiaRastreador = new Repositorio.Embarcador.Veiculos.TecnologiaRastreador(unitOfWork);
            Repositorio.Embarcador.Configuracoes.ConfiguracaoVeiculo repConfiguracaoVeiculo = new Repositorio.Embarcador.Configuracoes.ConfiguracaoVeiculo(unitOfWork);
            Repositorio.Embarcador.Configuracoes.ConfiguracaoJanelaCarregamento repConfiguracaoJanelaCarregamento = new Repositorio.Embarcador.Configuracoes.ConfiguracaoJanelaCarregamento(unitOfWork);
            Repositorio.Embarcador.Veiculos.HistoricoVeiculoVinculo repHistoricoVeiculoVinculo = new Repositorio.Embarcador.Veiculos.HistoricoVeiculoVinculo(unitOfWork);
            Repositorio.Embarcador.Veiculos.TipoComunicacaoRastreador repTipoComunicacaoRastreador = new Repositorio.Embarcador.Veiculos.TipoComunicacaoRastreador(unitOfWork);
            Repositorio.Embarcador.Veiculos.HistoricoVeiculoVinculoCentroResultado repHistoricoVeiculoVinculoCentroResultado = new Repositorio.Embarcador.Veiculos.HistoricoVeiculoVinculoCentroResultado(unitOfWork);

            Servicos.Veiculo serVeiculo = new Servicos.Veiculo(unitOfWork);

            Dominio.Entidades.Embarcador.Configuracoes.ConfiguracaoVeiculo configuracaoVeiculo = repConfiguracaoVeiculo.BuscarConfiguracaoPadrao();
            Dominio.Entidades.Embarcador.Configuracoes.ConfiguracaoJanelaCarregamento configuracaoJanelaCarregamento = repConfiguracaoJanelaCarregamento.BuscarPrimeiroRegistro();

            List<AdminMultisoftware.Dominio.Enumeradores.PermissaoPersonalizada> permissoesPersonalizadas = ObterPermissoesPersonalizadas("Veiculos/Veiculo");
            bool permiteBloquearVeiculo = permissoesPersonalizadas.Contains(AdminMultisoftware.Dominio.Enumeradores.PermissaoPersonalizada.Veiculo_PermiteBloquearVeiculo) && TipoServicoMultisoftware == AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiEmbarcador;

            if (permiteBloquearVeiculo)
            {
                veiculo.VeiculoBloqueado = Request.GetBoolParam("BloquearVeiculo");
                veiculo.MotivoBloqueio = veiculo.VeiculoBloqueado ? Request.GetStringParam("MotivoBloqueio") : "";
            }

            veiculo.Placa = Utilidades.String.RemoveAllSpecialCharacters(Request.Params("Placa").ToUpper());
            veiculo.DataAtualizacao = DateTime.Now;

            if (!string.IsNullOrEmpty(Request.Params("Tara")))
                veiculo.Tara = int.Parse(Request.Params("Tara"));

            if (TipoServicoMultisoftware != AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.Terceiros && TipoServicoMultisoftware != AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.TransportadorTerceiro)
                veiculo.Tipo = Request.Params("Tipo");

            veiculo.Renavam = Request.Params("Renavam");
            veiculo.TipoRodado = Request.Params("TipoRodado");
            veiculo.TipoCarroceria = Request.Params("TipoCarroceria");
            veiculo.TipoVeiculo = Request.Params("TipoVeiculo");
            veiculo.Estado = new Dominio.Entidades.Estado() { Sigla = Request.Params("Estado") };

            int codigoLocalidadeEmplacamento = Request.GetIntParam("LocalidadeEmplacamento");
            veiculo.LocalidadeEmplacamento = codigoLocalidadeEmplacamento > 0 ? repLocalidade.BuscarPorCodigo(codigoLocalidadeEmplacamento) : null;
            veiculo.DataValidadeLiberacaoSeguradora = Request.GetNullableDateTimeParam("DataValidadeLiberacaoSeguradora");
            veiculo.DataUltimoChecklist = Request.GetNullableDateTimeParam("DataUltimoChecklist");
            veiculo.DataValidadeANTT = Request.GetNullableDateTimeParam("DataValidadeANTT");
            veiculo.Ativo = bool.Parse(Request.Params("Ativo"));
            veiculo.IntegradoERP = false;

            if (!serVeiculo.ValidarPlaca(veiculo.Placa, unitOfWork))//Só valida quantidade de dígitos
                throw new ControllerException(string.Format(Localization.Resources.Veiculos.Veiculo.PlacaInformadaInvalidaPorFavorCadastreUmaPlacaValida, veiculo.Placa));

            if (ConfiguracaoEmbarcador.Pais == TipoPais.Brasil)
            {
                if (ConfiguracaoEmbarcador.ValidarPlacaVeiculo && !Utilidades.Validate.ValidarPlaca(veiculo.Placa))//Valida padrão antigo e MERCOSUL
                    throw new ControllerException(string.Format(Localization.Resources.Veiculos.Veiculo.PlacaInformadaInvalidaPorFavorCadastreUmaPlacaValida, veiculo.Placa));
                //if (ConfiguracaoEmbarcador.ValidarRENAVAMVeiculo && !Utilidades.Validate.ValidarRENAVAM(veiculo.Renavam))
                //    throw new ControllerException(string.Format(Localization.Resources.Veiculos.Veiculo.RenavamInformadoInvalidoPorFavorPreenchaUmRenavamValido, veiculo.Renavam));
            }

            int.TryParse(Request.Params("ModeloCarroceria"), out int codigoModeloCarroceria);
            int codigoFuncionarioResponsavel = Request.GetIntParam("FuncionarioResponsavel");

            int codigoTipoPlotagem = Request.GetIntParam("TipoPlotagem");

            veiculo.ModeloCarroceria = codigoModeloCarroceria > 0 ? repModeloCarroceria.BuscarPorCodigo(codigoModeloCarroceria) : null;
            veiculo.FuncionarioResponsavel = codigoFuncionarioResponsavel > 0 ? repMotorista.BuscarPorCodigo(codigoFuncionarioResponsavel) : null;
            veiculo.TipoPlotagem = codigoTipoPlotagem > 0 ? repTipoPlotagem.BuscarPorCodigo(codigoTipoPlotagem, false) : null;

            if (!string.IsNullOrWhiteSpace(Request.Params("CapacidadeQuilo")))
                veiculo.CapacidadeKG = int.Parse(Request.Params("CapacidadeQuilo"));
            else
                veiculo.CapacidadeKG = 0;

            if (!string.IsNullOrWhiteSpace(Request.Params("CapacidadeM3")))
                veiculo.CapacidadeM3 = int.Parse(Request.Params("CapacidadeM3"));

            if (!string.IsNullOrWhiteSpace(Request.Params("MarcaVeiculo")) && int.Parse(Request.Params("MarcaVeiculo")) > 0)
                veiculo.Marca = new Dominio.Entidades.MarcaVeiculo() { Codigo = int.Parse(Request.Params("MarcaVeiculo")) };
            else
                veiculo.Marca = null;
            if (!string.IsNullOrWhiteSpace(Request.Params("ModeloVeiculo")) && int.Parse(Request.Params("ModeloVeiculo")) > 0)
                veiculo.Modelo = new Dominio.Entidades.ModeloVeiculo() { Codigo = int.Parse(Request.Params("ModeloVeiculo")) };
            else
                veiculo.Modelo = null;
            veiculo.Chassi = Request.Params("Chassi");
            if (!string.IsNullOrWhiteSpace(Request.Params("DataAquisicao")))
                veiculo.DataCompra = DateTime.Parse(Request.Params("DataAquisicao"));
            else
                veiculo.DataCompra = null;
            if (!string.IsNullOrWhiteSpace(Request.Params("ValorAquisicao")))
                veiculo.ValorAquisicao = decimal.Parse(Request.Params("ValorAquisicao"));
            else
                veiculo.ValorAquisicao = 0;
            if (!string.IsNullOrWhiteSpace(Request.Params("CapacidadeTanque")))
                veiculo.CapacidadeTanque = int.Parse(Request.Params("CapacidadeTanque"));
            else
                veiculo.CapacidadeTanque = 0;
            if (!string.IsNullOrWhiteSpace(Request.Params("AnoFabricacao")) && int.Parse(Request.Params("AnoFabricacao")) > 0)
                veiculo.AnoFabricacao = int.Parse(Request.Params("AnoFabricacao"));
            else
                veiculo.AnoFabricacao = 0;
            if (!string.IsNullOrWhiteSpace(Request.Params("AnoModelo")) && int.Parse(Request.Params("AnoModelo")) > 0)
                veiculo.AnoModelo = int.Parse(Request.Params("AnoModelo"));
            else
                veiculo.AnoModelo = 0;

            if (!string.IsNullOrWhiteSpace(Request.Params("CapacidadeTanqueArla")))
                veiculo.CapacidadeTanqueArla = int.Parse(Request.Params("CapacidadeTanqueArla"));
            else
                veiculo.CapacidadeTanqueArla = 0;

            decimal.TryParse(Request.Params("ValorContainerAverbacao"), out decimal valorContainerAverbacao);
            veiculo.ValorContainerAverbacao = valorContainerAverbacao;

            veiculo.PendenteIntegracaoEmbarcador = true;
            veiculo.NumeroMotor = Request.Params("NumeroMotor");
            veiculo.NumeroCartaoValePedagio = Request.Params("NumeroCartaoValePedagio");
            veiculo.NumeroCartaoAbastecimento = Request.Params("NumeroCartaoAbastecimento");

            if (!string.IsNullOrWhiteSpace(Request.Params("GarantiaEscalonada")))
                veiculo.DataVencimentoGarantiaEscalonada = DateTime.Parse(Request.Params("GarantiaEscalonada"));
            else
                veiculo.DataVencimentoGarantiaEscalonada = null;
            if (!string.IsNullOrWhiteSpace(Request.Params("GarantiaPlena")))
                veiculo.DataVencimentoGarantiaPlena = DateTime.Parse(Request.Params("GarantiaPlena"));
            else
                veiculo.DataVencimentoGarantiaPlena = null;

            if (!string.IsNullOrWhiteSpace(Request.Params("KilometragemAtual")) && int.Parse(Request.Params("KilometragemAtual")) > 0)
                veiculo.KilometragemAtual = int.Parse(Request.Params("KilometragemAtual"));
            else
                veiculo.KilometragemAtual = 0;
            DateTime dataParametro = DateTime.Parse("01/01/2008");
            if (veiculo.ModeloCarroceria != null && Request.GetDateTimeParam("DataValidadeAdicionalCarroceria") > dataParametro)
                veiculo.DataValidadeAdicionalCarroceria = Request.GetDateTimeParam("DataValidadeAdicionalCarroceria");
            else
                veiculo.DataValidadeAdicionalCarroceria = null;

            if (Request.GetDateTimeParam("DataVigencia") > DateTime.MinValue)
                veiculo.DataVigencia = Request.GetDateTimeParam("DataVigencia");
            else
                veiculo.DataVigencia = DateTime.Now;

            int.TryParse(Request.Params("GrupoPessoa"), out int codigoGrupoPessoa);
            int.TryParse(Request.Params("SegmentoVeiculo"), out int codigoSegmentoVeiculo);
            int codigoTipoComunicacaoRastreador = Request.GetIntParam("TipoComunicacaoRastreador");
            int codigoTecnologiaRastreador = Request.GetIntParam("TecnologiaRastreador");
            int codigoCentroResultado = Request.GetIntParam("CentroResultado");
            int codigoFilialCarregamento = Request.GetIntParam("FilialCarregamento");
            int codigoEmpresaVeiculoCooperado = Request.GetIntParam("EmpresaVeiculoCooperado");

            double.TryParse(Request.Params("LocalAtualFisicoDoVeiculo"), out double localAtualFisicoDoVeiculo);

            bool possuiRastreador = Request.GetBoolParam("PossuiRastreador");
            bool PossuiControleDisponibilidade = Request.GetBoolParam("PossuiControleDisponibilidade");
            bool PossuiImobilizador = Request.GetBoolParam("PossuiImobilizador");
            bool PossuiTelemetria = Request.GetBoolParam("PossuiTelemetria");
            bool VeiculoUtilizadoNoTransporteDeFrotasDedicadasOuFidelizadas = Request.GetBoolParam("VeiculoUtilizadoNoTransporteDeFrotasDedicadasOuFidelizadas");
            bool PossuiTravaQuintaDeRoda = Request.GetBoolParam("PossuiTravaQuintaDeRoda");
            bool possuiTagValePedagio = Request.GetBoolParam("PossuiTagValePedagio");
            bool naoComprarValePedagio = Request.GetBoolParam("NaoComprarValePedagio");
            bool NaoComprarValePedagioRetorno = Request.GetBoolParam("NaoComprarValePedagioRetorno");
            bool naoValidarIntegracaoParaFilaCarregamento = Request.GetBoolParam("NaoValidarIntegracaoParaFilaCarregamento");
            bool naoIntegrarOpentech = Request.GetBoolParam("NaoIntegrarOpentech");
            bool veiculoCooperado = Request.GetBoolParam("VeiculoCooperado");

            veiculo.FormaDeducaoValePedagio = ((string)Request.Params("FormaDeducaoValePedagio")).ToNullableEnum<FormaDeducaoValePedagio>();
            string numeroEquipamentoRastreador = Request.Params("NumeroEquipamentoRastreador");
            int.TryParse(Request.Params("Cor"), out int codigoCor);
            string fatorEmissao = Request.GetStringParam("FatorEmissao");
            string padraoEmissao = Request.GetStringParam("PadraoEmissao");

            veiculo.CorVeiculo = codigoCor > 0 ? repCorVeiculo.BuscarPorCodigo(codigoCor) : null;
            veiculo.PadraoEmissao = padraoEmissao;
            veiculo.FatorEmissao = fatorEmissao;
            veiculo.PossuiControleDisponibilidade = PossuiControleDisponibilidade;
            veiculo.PossuiTravaQuintaDeRoda = PossuiTravaQuintaDeRoda;
            veiculo.PossuiTelemetria = PossuiTelemetria;
            veiculo.VeiculoUtilizadoNoTransporteDeFrotasDedicadasOuFidelizadas = VeiculoUtilizadoNoTransporteDeFrotasDedicadasOuFidelizadas;
            veiculo.PossuiImobilizador = PossuiImobilizador;
            veiculo.AtivarConsultarAbastecimentoAngelLira = Request.GetBoolParam("AtivarConsultarAbastecimentoAngelLira");
            veiculo.PossuiTagValePedagio = possuiTagValePedagio;
            veiculo.NaoComprarValePedagio = naoComprarValePedagio;
            veiculo.NaoComprarValePedagioRetorno = NaoComprarValePedagioRetorno;
            veiculo.NaoValidarIntegracaoParaFilaCarregamento = naoValidarIntegracaoParaFilaCarregamento;
            veiculo.PossuiRastreador = possuiRastreador;
            veiculo.NaoIntegrarOpentech = naoIntegrarOpentech;
            veiculo.NumeroEquipamentoRastreador = numeroEquipamentoRastreador.Trim();
            veiculo.TipoComunicacaoRastreador = codigoTipoComunicacaoRastreador > 0 ? repTipoComunicacaoRastreador.BuscarPorCodigo(codigoTipoComunicacaoRastreador, false) : null;
            veiculo.TecnologiaRastreador = codigoTecnologiaRastreador > 0 ? repTecnologiaRastreador.BuscarPorCodigo(codigoTecnologiaRastreador, false) : null;
            veiculo.GrupoPessoas = codigoGrupoPessoa > 0 ? repGrupoPessoas.BuscarPorCodigo(codigoGrupoPessoa) : null;
            veiculo.SegmentoVeiculo = codigoSegmentoVeiculo > 0 ? repSegmentoVeiculo.BuscarPorCodigo(codigoSegmentoVeiculo) : null;
            veiculo.LocalAtualFisicoDoVeiculo = localAtualFisicoDoVeiculo > 0 ? repCliente.BuscarPorCPFCNPJ(localAtualFisicoDoVeiculo) : null;
            veiculo.FilialCarregamento = codigoFilialCarregamento > 0 ? repositorioFilial.BuscarPorCodigo(codigoFilialCarregamento) : null;
            veiculo.VeiculoCooperado = veiculoCooperado;
            veiculo.EmpresaVeiculoCooperado = codigoEmpresaVeiculoCooperado > 0 ? repEmpresa.BuscarPorCodigo(codigoEmpresaVeiculoCooperado) : null;

            Dominio.Entidades.Veiculo veiculoRastreador = repVeiculo.BuscarRastreadorPorPlaca(veiculo.Placa);
            if (veiculoRastreador != null && veiculo.Codigo == 0)
            {
                veiculo.PossuiRastreador = true;
                veiculo.TecnologiaRastreador = veiculoRastreador.TecnologiaRastreador;
                veiculo.NumeroEquipamentoRastreador = veiculoRastreador.NumeroEquipamentoRastreador;
                veiculo.TipoComunicacaoRastreador = veiculoRastreador?.TipoComunicacaoRastreador ?? null;
            }

            if (codigoCentroResultado > 0)
            {
                if (veiculo.CentroResultado != null)
                {
                    Dominio.Entidades.Embarcador.Financeiro.CentroResultado centroResultadoAntigo = veiculo.CentroResultado;

                    if (centroResultadoAntigo != veiculo.CentroResultado)
                    {
                        Dominio.Entidades.Embarcador.Veiculos.HistoricoVeiculoVinculo historicoVeiculoVinculo = new Dominio.Entidades.Embarcador.Veiculos.HistoricoVeiculoVinculo();
                        historicoVeiculoVinculo = repHistoricoVeiculoVinculo.BuscarPorVeiculo(veiculo.Codigo);

                        if (historicoVeiculoVinculo == null)
                        {
                            historicoVeiculoVinculo = new Dominio.Entidades.Embarcador.Veiculos.HistoricoVeiculoVinculo()
                            {
                                Veiculo = veiculo,
                                DataHora = DateTime.Now,
                                Usuario = Usuario,
                                KmRodado = veiculo.KilometragemAtual,
                                KmAtualModificacao = 0,
                                DiasVinculado = 0
                            };
                            repHistoricoVeiculoVinculo.Inserir(historicoVeiculoVinculo);
                        }

                        Dominio.Entidades.Embarcador.Veiculos.HistoricoVeiculoVinculoCentroResultado historicoVeiculoVinculoCentroResultado = new Dominio.Entidades.Embarcador.Veiculos.HistoricoVeiculoVinculoCentroResultado
                        {
                            HistoricoVeiculoVinculo = historicoVeiculoVinculo,
                            CentroResultado = veiculo.CentroResultado,
                            DataHora = DateTime.Now,
                        };

                        repHistoricoVeiculoVinculoCentroResultado.Inserir(historicoVeiculoVinculoCentroResultado, Auditado);
                    }
                }

                veiculo.CentroResultado = repCentroResultado.BuscarPorCodigo(codigoCentroResultado);
            }

            veiculo.ObservacaoCTe = Request.Params("ObservacaoCTe");
            veiculo.Contrato = Request.Params("NumeroContrato");
            veiculo.NumeroFrota = Request.Params("NumeroFrota");
            veiculo.CodigoIntegracao = Request.GetStringParam("CodigoIntegracao");
            veiculo.Observacao = Request.Params("Observacao");
            veiculo.TipoFrota = Request.GetEnumParam("TipoFrota", TipoFrota.NaoDefinido);
            veiculo.QuantidadeCurrais = Request.GetIntParam("QuantidadeCurrais");
            veiculo.TipoCombustivel = Request.GetStringParam("TipoCombustivel");
            veiculo.TipoCarreta = Request.GetNullableEnumParam<TipoCarreta>("TipoCarreta");
            veiculo.TipoMaterialGaiola = Request.GetNullableEnumParam<TipoMaterial>("TipoMaterialGaiola");
            veiculo.TipoMaterialPiso = Request.GetNullableEnumParam<TipoMaterial>("TipoMaterialPiso");
            veiculo.TipoSistemaElevacao = Request.GetNullableEnumParam<TipoSistemaElevacao>("TipoSistemaElevacao");
            veiculo.ViradaHodometro = Request.GetBoolParam("ViradaHodometro");
            veiculo.KilometragemVirada = Request.GetIntParam("KilometragemVirada");
            veiculo.DataValidadeLiberacaoSeguradora = Request.GetNullableDateTimeParam("DataValidadeLiberacaoSeguradora");
            veiculo.CapacidadeMaximaTanque = Request.GetDecimalParam("CapacidadeMaximaTanque");
            veiculo.PossuiLocalizador = Request.GetBoolParam("PossuiLocalizador");
            veiculo.PosicaoReboque = Request.GetNullableEnumParam<PosicaoReboque>("PosicaoReboque");
            veiculo.Paletizado = Request.GetBoolParam("PaletizadoGeracaoFrota");
            veiculo.VeiculoAlugado = Request.GetBoolParam("VeiculoAlugado");
            veiculo.CIOTEmitidoContratanteDiferenteEmbarcador = Request.GetBoolParam("CIOTEmitidoContratanteDiferenteEmbarcador");
            veiculo.DataInicialCIOTTemporario = Request.GetNullableDateTimeParam("DataInicialCIOTTemporario");
            veiculo.DataFinalCIOTTemporario = Request.GetNullableDateTimeParam("DataFinalCIOTTemporario");
            veiculo.TagSemParar = Request.GetStringParam("TagSemParar");

            if (configuracaoVeiculo.NaoPermitirRealizarCadastroPlacaBloqueada && veiculo.Codigo == 0)
            {
                if (repVeiculo.ExisteOutroCadastroMesmaPlacaBloqueado(veiculo.Placa))
                    throw new ControllerException("Ja existe outro cadastro com a placa bloqueada.");
            }

            List<TipoIntegracao> tiposIntegracaoValePedagio = Request.GetListEnumParam<TipoIntegracao>("TipoIntegracaoValePedagio");

            if ((configuracaoJanelaCarregamento.BloquearVeiculoSemTagValePedagioAtiva ?? false) && tiposIntegracaoValePedagio.Count == 0 && !veiculo.NaoComprarValePedagio)
                throw new ControllerException(Localization.Resources.Veiculos.Veiculo.SelecioneOperadoraPedagio);

            if (veiculo.TiposIntegracaoValePedagio == null)
                veiculo.TiposIntegracaoValePedagio = new List<Dominio.Entidades.Embarcador.Cargas.TipoIntegracao>();
            else
                veiculo.TiposIntegracaoValePedagio.Clear();

            foreach (TipoIntegracao tipoIntegracaoValePedagio in tiposIntegracaoValePedagio)
                veiculo.TiposIntegracaoValePedagio.Add(repTipoIntegracao.BuscarPorTipo(tipoIntegracaoValePedagio));

        }

        private dynamic ObterDetalhesVeiculo(Dominio.Entidades.Veiculo veiculo, DateTime? dataAbastecimento, Repositorio.UnitOfWork unitOfWork)
        {
            Repositorio.Abastecimento repositorioAbastecimento = new Repositorio.Abastecimento(unitOfWork);
            Repositorio.Embarcador.Veiculos.VeiculoMotorista repositorioVeiculoMotorista = new Repositorio.Embarcador.Veiculos.VeiculoMotorista(unitOfWork);

            Servicos.Veiculo serVeiculo = new Servicos.Veiculo(unitOfWork);

            Dominio.Entidades.Usuario motorista = repositorioVeiculoMotorista.BuscarMotoristaPrincipal(veiculo.Codigo);

            var retorno = new
            {
                veiculo.Descricao,
                CodigoSegmentoVeiculo = veiculo.SegmentoVeiculo?.Codigo,
                SegmentoVeiculo = veiculo.SegmentoVeiculo?.Descricao,
                TipoPropriedade = veiculo.Tipo,
                veiculo.Codigo,
                Empresa = (TipoServicoMultisoftware == AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.Terceiros || TipoServicoMultisoftware == AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.TransportadorTerceiro) && veiculo.Proprietario != null ? veiculo.Empresa?.RazaoSocial + " (" + Localization.Resources.Veiculos.Veiculo.Terceiro + ": " + veiculo.Proprietario.Nome + ")" : (veiculo.Empresa?.RazaoSocial + " (" + veiculo.Empresa?.Localidade.DescricaoCidadeEstado + ")") ?? string.Empty,
                ModeloVeicularCarga = veiculo.ModeloVeicularCarga?.Descricao ?? string.Empty,
                CodigoModeloVeicularCarga = veiculo.ModeloVeicularCarga?.Codigo ?? 0,
                CapacidadePesoTransporte = veiculo.ModeloVeicularCarga?.CapacidadePesoTransporte.ToString("n2") ?? 0.ToString("n2"),
                CapacidadeKG = veiculo.CapacidadeKG.ToString("n0"),
                CapacidadeM3 = veiculo.CapacidadeM3.ToString("n0"),
                Placa = veiculo.Placa,
                PlacaFormatada = ObterPlacaConcatenadaFrota(veiculo),
                TipoVeiculo = veiculo.DescricaoTipoVeiculo,
                veiculo.Renavam,
                veiculo.NumeroFrota,
                ConjuntoPlacasComModeloVeicular = serVeiculo.BuscarReboquesComModeloVeicular(veiculo),
                ConjuntoPlacasComModeloVeicularEFrota = serVeiculo.BuscarReboquesComModeloVeicularEFrota(veiculo),
                ConjuntoPlacasSemModeloVeicular = serVeiculo.BuscarReboquesSemModeloVeicular(veiculo),
                ConjuntoFrota = serVeiculo.BuscarConjuntoFrota(veiculo),
                Reboque = string.Join(", ", veiculo.VeiculosVinculados.Select(o => o.Placa)),
                DT_RowClass = !string.IsNullOrWhiteSpace(veiculo.Renavam) ? Dominio.ObjetosDeValor.Embarcador.Enumeradores.ClasseCorFundo.Sucess(IntensidadeCor._100) : "",
                CodigoMotorista = motorista?.Codigo ?? 0,
                Motorista = motorista?.Descricao ?? string.Empty,
                CPFMotorista = motorista?.CPF_Formatado ?? string.Empty,
                NomeMotorista = motorista?.Nome ?? string.Empty,
                veiculo.DescricaoTipo,
                Estado = veiculo.Estado.Sigla,
                RNTRC = veiculo.RNTRC.ToString().PadLeft(8, '0'),
                UltimoKMAbastecimento = dataAbastecimento.HasValue ? repositorioAbastecimento.BuscarUltimoKMAbastecimento(veiculo.Codigo, dataAbastecimento.Value, 0, TipoAbastecimento.Combustivel).ToString("n0").Replace(".", "") : 0.ToString("n0"),
                Tracao = string.Join(", ", veiculo.VeiculosTracao.Select(o => o.Placa)),
                CodigoTracao = veiculo.VeiculosTracao.FirstOrDefault()?.Codigo ?? 0,
                CodigoEmpresa = veiculo.Empresa?.Codigo ?? 0,
                KMAtual = veiculo.KilometragemAtual,
                CodigoCentroResultado = veiculo.CentroResultado?.Codigo ?? 0,
                CentroResultado = veiculo.CentroResultado?.Descricao ?? "",
                Proprietario = veiculo.Proprietario?.Nome ?? string.Empty,
                veiculo.DescricaoComMarcaModelo,
                SituacaoCadastro = veiculo.SituacaoCadastro.ObterDescricao(),
                VeiculosVinculados = ObterPlacasVeiculosVinculados(veiculo),
                CodigosVeiculosVinculados = ObterCodigosVeiculosVinculados(veiculo),
                EmpresaDescricao = veiculo.Empresa?.Descricao ?? string.Empty,
                Tipo = veiculo.TipoVeiculo,
                CNPJEmpresa = veiculo.Empresa?.CNPJ_Formatado ?? "",
                ListaDiariaGerada = false,
                TipoFrotaDescricao = veiculo.TipoFrota?.ObterDescricao() ?? "",
                Arla32 = veiculo.Modelo?.PossuiArla32 ?? 0
            };

            return retorno;
        }

        private dynamic ObterDetalhesVeiculoGestaoCarga(Dominio.Entidades.Veiculo veiculo, Repositorio.UnitOfWork unitOfWork)
        {
            Repositorio.Embarcador.Veiculos.VeiculoMotorista repositorioVeiculoMotorista = new Repositorio.Embarcador.Veiculos.VeiculoMotorista(unitOfWork);

            Dominio.Entidades.Usuario motorista = repositorioVeiculoMotorista.BuscarMotoristaPrincipal(veiculo.Codigo);

            var retorno = new
            {
                Codigo = veiculo.Codigo + "_" + "veiculo",
                veiculo.Placa,
                Renavam = veiculo.Renavam,
                Reboque = PlacasReboque(veiculo.VeiculosVinculados),
                CodigoMotorista = motorista?.Codigo ?? 0,
                Motorista = motorista?.Nome ?? string.Empty,
                DT_RowColor = "#FFFFFF"
            };

            return retorno;
        }

        private void BaixarTodosQrCode(string stringConexao, Dominio.ObjetosDeValor.Embarcador.Veiculos.FiltroPesquisaVeiculo filtrosPesquisa, bool somenteDisponiveis, Dominio.Entidades.Embarcador.Arquivo.ControleGeracaoArquivo controleGeracaoArquivo)
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(stringConexao);
            Servicos.Embarcador.Arquivo.ControleGeracaoArquivo servicoArquivo = new Servicos.Embarcador.Arquivo.ControleGeracaoArquivo(unitOfWork);

            try
            {
                Dominio.ObjetosDeValor.Embarcador.Consulta.ParametroConsulta parametrosConsulta = new Dominio.ObjetosDeValor.Embarcador.Consulta.ParametroConsulta()
                {
                    DirecaoOrdenar = "asc",
                    PropriedadeOrdenar = "Placa"
                };
                Repositorio.Veiculo repositorioVeiculo = new Repositorio.Veiculo(unitOfWork);
                List<Dominio.Entidades.Veiculo> veiculos = null;

                if (!somenteDisponiveis)
                    veiculos = repositorioVeiculo.ConsultarEmbarcador(filtrosPesquisa, parametrosConsulta);
                else
                    veiculos = repositorioVeiculo.ConsultarEmbarcadorSomenteDisponiveis(filtrosPesquisa, parametrosConsulta);

                if (veiculos.Count > 0)
                {
                    byte[] pdfTodosQrCode = controleGeracaoArquivo.TipoArquivo == TipoArquivo.PDF ? Servicos.Embarcador.Veiculo.Veiculo.ObterPdfTodosQRCodeVeiculo(veiculos) : Servicos.Embarcador.Veiculo.Veiculo.ObterTodosPdfQRCodeCompactado(veiculos);

                    servicoArquivo.SalvarArquivo(controleGeracaoArquivo, pdfTodosQrCode);
                    servicoArquivo.Finalizar(controleGeracaoArquivo, nota: string.Format(Localization.Resources.Veiculos.Veiculo.GeracaoDoArquivoDosQrCodeDosVeiculosConcluido, controleGeracaoArquivo.TipoArquivo.ObterDescricao()), urlPagina: "Veiculos/Veiculo");
                }
                else
                    servicoArquivo.Remover(controleGeracaoArquivo);
            }
            catch (Exception excecao)
            {
                servicoArquivo.FinalizarComFalha(controleGeracaoArquivo, nota: string.Format(Localization.Resources.Veiculos.Veiculo.OcorreuUmaFalhaAoGerarArquivoDosQrCodeDosVeiculos, controleGeracaoArquivo.TipoArquivo.ObterDescricao()), urlPagina: "Veiculos/Veiculo", excecao: excecao);
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        private string ObterPlacaConcatenadaFrota(Dominio.Entidades.Veiculo veiculo)
        {
            if (ConfiguracaoEmbarcador.ConcatenarFrotaPlaca)
                return veiculo.PlacaConcatenada;
            return veiculo.Placa;
        }

        private List<ConfiguracaoImportacao> ConfiguracaoImportacaoVeiculos()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);
            Repositorio.Embarcador.Configuracao.ConfiguracaoTMS repConfiguracao = new Repositorio.Embarcador.Configuracao.ConfiguracaoTMS(unitOfWork);
            Dominio.Entidades.Embarcador.Configuracao.ConfiguracaoTMS configuracao = repConfiguracao.BuscarConfiguracaoPadrao();

            var configuracoes = new List<ConfiguracaoImportacao>();
            int tamanho = 150;
            var tipoServicoMultiEmbarcador = TipoServicoMultisoftware == AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiEmbarcador;

            configuracoes.Add(new ConfiguracaoImportacao() { Id = 1, Descricao = Localization.Resources.Veiculos.Veiculo.Ano, Propriedade = "AnoFabricacao", Tamanho = tamanho, CampoInformacao = true });
            configuracoes.Add(new ConfiguracaoImportacao() { Id = 2, Descricao = Localization.Resources.Veiculos.Veiculo.AnoModelo, Propriedade = "AnoModelo", Tamanho = tamanho, CampoInformacao = true });
            configuracoes.Add(new ConfiguracaoImportacao() { Id = 3, Descricao = Localization.Resources.Veiculos.Veiculo.CapacidadeKg, Propriedade = "CapacidadeKilogramas", Tamanho = tamanho, CampoInformacao = true, Regras = new List<string> { "required" } });
            configuracoes.Add(new ConfiguracaoImportacao() { Id = 4, Descricao = Localization.Resources.Veiculos.Veiculo.CapacidadeMetrosCubicos, Propriedade = "CapacidadeMetrosCubicos", Tamanho = tamanho, CampoInformacao = true });
            configuracoes.Add(new ConfiguracaoImportacao() { Id = 5, Descricao = Localization.Resources.Veiculos.Veiculo.Chassi, Propriedade = "Chassi", Tamanho = tamanho, CampoInformacao = true });
            if (TipoServicoMultisoftware != AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiCTe && TipoServicoMultisoftware != AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiNFe)
                configuracoes.Add(new ConfiguracaoImportacao() { Id = 6, Descricao = Localization.Resources.Veiculos.Veiculo.CNPJTransportadora, Propriedade = "CnpjTransportadora", Tamanho = tamanho, Obrigatorio = tipoServicoMultiEmbarcador, CampoInformacao = true, Regras = new List<string> { "required" } });
            configuracoes.Add(new ConfiguracaoImportacao() { Id = 7, Descricao = Localization.Resources.Veiculos.Veiculo.CPFCNPJProprietario, Propriedade = "CnpjCpfProprietario", Tamanho = tamanho, CampoInformacao = true });
            configuracoes.Add(new ConfiguracaoImportacao() { Id = 8, Descricao = Localization.Resources.Veiculos.Veiculo.CPFMotorista, Propriedade = "CpfMotorista", Tamanho = tamanho, CampoInformacao = true });
            configuracoes.Add(new ConfiguracaoImportacao() { Id = 9, Descricao = Localization.Resources.Veiculos.Veiculo.KMAtual, Propriedade = "KilometragemAtual", Tamanho = tamanho, CampoInformacao = true });
            configuracoes.Add(new ConfiguracaoImportacao() { Id = 10, Descricao = Localization.Resources.Veiculos.Veiculo.ModeloVeicular, Propriedade = "ModeloVeicular", Tamanho = tamanho, Obrigatorio = true, CampoInformacao = true, Regras = new List<string> { "required" } });
            configuracoes.Add(new ConfiguracaoImportacao() { Id = 11, Descricao = Localization.Resources.Veiculos.Veiculo.NomeMotorista, Propriedade = "NomeMotorista", Tamanho = tamanho, CampoInformacao = true });
            configuracoes.Add(new ConfiguracaoImportacao() { Id = 12, Descricao = Localization.Resources.Gerais.Geral.Observacao, Propriedade = "Observacao", Tamanho = tamanho, CampoInformacao = true });
            configuracoes.Add(new ConfiguracaoImportacao() { Id = 13, Descricao = Localization.Resources.Veiculos.Veiculo.Placa, Propriedade = "Placa", Tamanho = tamanho, Obrigatorio = true, Regras = new List<string> { "required" } });
            configuracoes.Add(new ConfiguracaoImportacao() { Id = 14, Descricao = Localization.Resources.Veiculos.Veiculo.Renavam, Propriedade = "Renavam", Tamanho = tamanho, Obrigatorio = true, CampoInformacao = true, Regras = new List<string> { "required" } });
            configuracoes.Add(new ConfiguracaoImportacao() { Id = 15, Descricao = Localization.Resources.Veiculos.Veiculo.RntrcProprietario, Propriedade = "RntrcProprietario", Tamanho = tamanho, CampoInformacao = true });
            configuracoes.Add(new ConfiguracaoImportacao() { Id = 16, Descricao = Localization.Resources.Veiculos.Veiculo.Tara, Propriedade = "Tara", Tamanho = tamanho, CampoInformacao = true, Regras = new List<string> { "required" } });
            configuracoes.Add(new ConfiguracaoImportacao() { Id = 17, Descricao = Localization.Resources.Gerais.Geral.Tipo, Propriedade = "Tipo", Tamanho = tamanho, CampoInformacao = true });
            configuracoes.Add(new ConfiguracaoImportacao() { Id = 18, Descricao = Localization.Resources.Veiculos.Veiculo.TipoCarroceria, Propriedade = "TipoCarroceria", Tamanho = tamanho, CampoInformacao = true, Regras = new List<string> { "required" } });
            configuracoes.Add(new ConfiguracaoImportacao() { Id = 19, Descricao = Localization.Resources.Veiculos.Veiculo.TipoProprietario, Propriedade = "TipoProprietario", Tamanho = tamanho, CampoInformacao = true });
            configuracoes.Add(new ConfiguracaoImportacao() { Id = 20, Descricao = Localization.Resources.Veiculos.Veiculo.TipoRodado, Propriedade = "TipoRodado", Tamanho = tamanho, CampoInformacao = true, Regras = new List<string> { "required" } });
            configuracoes.Add(new ConfiguracaoImportacao() { Id = 21, Descricao = Localization.Resources.Veiculos.Veiculo.TipoVeiculo, Propriedade = "TipoVeiculo", Tamanho = tamanho, CampoInformacao = true });
            configuracoes.Add(new ConfiguracaoImportacao() { Id = 22, Descricao = Localization.Resources.Veiculos.Veiculo.UF, Propriedade = "Uf", Tamanho = tamanho, Obrigatorio = true, CampoInformacao = true, Regras = new List<string> { "required" } });
            configuracoes.Add(new ConfiguracaoImportacao() { Id = 23, Descricao = Localization.Resources.Veiculos.Veiculo.TecnologiaRastreador, Propriedade = "TecnologiaRastreador", Tamanho = tamanho, CampoInformacao = true });
            configuracoes.Add(new ConfiguracaoImportacao() { Id = 24, Descricao = Localization.Resources.Veiculos.Veiculo.TipoComunicacaoRastreador, Propriedade = "TipoComunicacaoRastreador", Tamanho = tamanho, CampoInformacao = true });
            configuracoes.Add(new ConfiguracaoImportacao() { Id = 25, Descricao = Localization.Resources.Veiculos.Veiculo.NumeroEquipamentoRastreador, Propriedade = "NumeroEquipamentoRastreador", Tamanho = tamanho, CampoInformacao = true });
            configuracoes.Add(new ConfiguracaoImportacao() { Id = 26, Descricao = Localization.Resources.Veiculos.Veiculo.Cor, Propriedade = "Cor", Tamanho = tamanho, CampoInformacao = true });
            configuracoes.Add(new ConfiguracaoImportacao() { Id = 27, Descricao = Localization.Resources.Veiculos.Veiculo.TipoCombustivel, Propriedade = "TipoCombustivel", Tamanho = tamanho, CampoInformacao = true });
            if (TipoServicoMultisoftware == AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiTMS || TipoServicoMultisoftware == AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiNFe)
            {
                configuracoes.Add(new ConfiguracaoImportacao() { Id = 28, Descricao = Localization.Resources.Veiculos.Veiculo.ModeloVeiculo, Propriedade = "ModeloVeiculo", Tamanho = tamanho, CampoInformacao = true });
                configuracoes.Add(new ConfiguracaoImportacao() { Id = 29, Descricao = Localization.Resources.Veiculos.Veiculo.MarcaVeiculo, Propriedade = "MarcaVeiculo", Tamanho = tamanho, CampoInformacao = true });
            }
            configuracoes.Add(new ConfiguracaoImportacao() { Id = 30, Descricao = Localization.Resources.Gerais.Geral.Situacao, Propriedade = "Ativo", Tamanho = tamanho, CampoInformacao = true });

            if (configuracao.Pais == TipoPais.Exterior)
            {
                configuracoes.Add(new ConfiguracaoImportacao() { Id = 31, Descricao = Localization.Resources.Veiculos.Veiculo.PadraoEmissao, Propriedade = "PadraoEmissao", Tamanho = tamanho, CampoInformacao = true });
                configuracoes.Add(new ConfiguracaoImportacao() { Id = 32, Descricao = Localization.Resources.Veiculos.Veiculo.FatorEmissao, Propriedade = "FatorEmissao", Tamanho = tamanho, CampoInformacao = true });
            }
            configuracoes.Add(new ConfiguracaoImportacao() { Id = 33, Descricao = Localization.Resources.Veiculos.Veiculo.CentroResultado, Propriedade = "CentroResultado", Tamanho = tamanho, CampoInformacao = true });
            configuracoes.Add(new ConfiguracaoImportacao() { Id = 34, Descricao = "CIOT", Propriedade = "CIOT", Tamanho = tamanho, CampoInformacao = true });

            configuracoes.Add(new ConfiguracaoImportacao() { Id = 35, Descricao = Localization.Resources.Veiculos.Veiculo.TipoFrota, Propriedade = "TipoFrota", Tamanho = tamanho, CampoInformacao = true, Regras = new List<string> { "required" } });
            configuracoes.Add(new ConfiguracaoImportacao() { Id = 36, Descricao = Localization.Resources.Veiculos.Veiculo.CapacidadeTanque, Propriedade = "CapacidadeTanque", Tamanho = tamanho, CampoInformacao = true });
            configuracoes.Add(new ConfiguracaoImportacao() { Id = 37, Descricao = Localization.Resources.Veiculos.Veiculo.CapacidadeTanqueMax, Propriedade = "CapacidadeTanqueMax", Tamanho = tamanho, CampoInformacao = true });
            configuracoes.Add(new ConfiguracaoImportacao() { Id = 38, Descricao = Localization.Resources.Veiculos.Veiculo.CodigoIntegracao, Propriedade = "CodigoIntegracao", Tamanho = tamanho, CampoInformacao = true });

            return configuracoes;
        }

        private Dominio.ObjetosDeValor.Embarcador.Veiculos.FiltroPesquisaVeiculo ObterFiltrosPesquisa(Repositorio.UnitOfWork unitOfWork)
        {
            Repositorio.Embarcador.Configuracoes.ConfiguracaoGeral repositorioConfiguracaoGeral = new Repositorio.Embarcador.Configuracoes.ConfiguracaoGeral(unitOfWork);
            Dominio.Entidades.Embarcador.Configuracoes.ConfiguracaoGeral configuracaoGeral = repositorioConfiguracaoGeral.BuscarConfiguracaoPadrao();

            Repositorio.Embarcador.Configuracao.ConfiguracaoTMS repConfiguracaoTMS = new Repositorio.Embarcador.Configuracao.ConfiguracaoTMS(unitOfWork);
            Dominio.Entidades.Embarcador.Configuracao.ConfiguracaoTMS configuracaoTMS = repConfiguracaoTMS.BuscarConfiguracaoPadrao();

            Dominio.ObjetosDeValor.Embarcador.Veiculos.FiltroPesquisaVeiculo filtrosPesquisa = new Dominio.ObjetosDeValor.Embarcador.Veiculos.FiltroPesquisaVeiculo()
            {
                ApenasTracao = false,
                CodigoAcertoViagem = Request.GetIntParam("AcertoViagem"),
                CodigoCentroCarregamento = Request.GetIntParam("CentroCarregamento"),
                CodigoMarcaVeiculo = Request.GetIntParam("MarcaVeiculo"),
                CodigoModeloVeiculo = Request.GetIntParam("ModeloVeiculo"),
                CodigoMotorista = Request.GetIntParam("Motorista"),
                CodigoReboque = Request.GetIntParam("Reboque"),
                ForcarFiltroModeloVeicularCarga = Request.GetBoolParam("ForcarFiltroModelo"),
                NumeroFrota = Request.GetStringParam("NumeroFrota"),
                Placa = Request.GetStringParam("Placa"),
                Renavam = Request.GetStringParam("Renavam"),
                SituacaoAtivo = Request.GetEnumParam<SituacaoAtivoPesquisa>("Ativo"),
                SomenteEmpresasAtivas = Request.GetBoolParam("SomenteEmpresasAtivas"),
                LocalAtualFisicoDoVeiculo = Request.GetDoubleParam("LocalAtualFisicoDoVeiculo"),
                TipoPropriedade = Request.GetStringParam("TipoPropriedade"),
                TipoServicoMultisoftware = TipoServicoMultisoftware,
                TipoVeiculo = Request.GetStringParam("TipoVeiculo"),
                CodigosSegmento = JsonConvert.DeserializeObject<List<int>>(Request.GetStringParam("Segmento")),
                CodigoProprietario = Request.GetDoubleParam("Proprietario"),
                Chassi = Request.GetStringParam("Chassi")
            };

            if (configuracaoTMS.Pais == TipoPais.Exterior)
                filtrosPesquisa.SomenteEmpresasAtivas = false;

            if ((this.Usuario?.LimitarOperacaoPorEmpresa ?? false) && (configuracaoGeral?.AtivarConsultaSegregacaoPorEmpresa ?? false) && this.Usuario?.Empresas != null && this.Usuario?.Empresas?.Count > 0)
                filtrosPesquisa.CodigosEmpresas = this.Usuario.Empresas.Select(c => c.Codigo).ToList();

            if (ConfiguracaoEmbarcador.UtilizaNumeroDeFrotaParaPesquisaDeVeiculo)
            {
                //System.Text.RegularExpressions.Regex patternNumeroFrota = new System.Text.RegularExpressions.Regex("[0-9]+");                
                if (!string.IsNullOrWhiteSpace(filtrosPesquisa.Placa) && long.TryParse(filtrosPesquisa.Placa, out long number1))
                {
                    filtrosPesquisa.NumeroFrota = filtrosPesquisa.Placa.ObterSomenteNumeros();
                }
            }

            int codigoCarga = Request.GetIntParam("Carga");
            int codigoModeloVeicularCarga = Request.GetIntParam("ModeloVeicularCarga");
            int codigoTipoCarga = Request.GetIntParam("TipoCarga");
            Repositorio.Embarcador.Cargas.ModeloVeicularCarga repositorioModeloVeicularCarga = new Repositorio.Embarcador.Cargas.ModeloVeicularCarga(unitOfWork);

            if (codigoModeloVeicularCarga > 0)
            {
                filtrosPesquisa.ModeloVeicularCarga = repositorioModeloVeicularCarga.BuscarPorCodigo(codigoModeloVeicularCarga);
                if  (filtrosPesquisa.ModeloVeicularCarga != null)
                    filtrosPesquisa.ForcarFiltroModeloVeicularCarga = true;
            }

            List<int> listaCodigoModeloVeicularCarga = Request.GetListParam<int>("ModelosVeiculares");

            if (listaCodigoModeloVeicularCarga.Count > 0)
            {
                filtrosPesquisa.PossiveisModelos = new List<Dominio.Entidades.Embarcador.Cargas.ModeloVeicularCarga>();

                foreach (int codigo in listaCodigoModeloVeicularCarga)
                {
                    Dominio.Entidades.Embarcador.Cargas.ModeloVeicularCarga modeloVeicularCarga = repositorioModeloVeicularCarga.BuscarPorCodigo(codigo);

                    if (modeloVeicularCarga != null)
                        filtrosPesquisa.PossiveisModelos.Add(modeloVeicularCarga);
                }
            }

            if (TipoServicoMultisoftware == AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiNFe)
                filtrosPesquisa.SomenteEmpresasAtivas = false;

            if (TipoServicoMultisoftware == AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiCTe || TipoServicoMultisoftware == AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiNFe)
                filtrosPesquisa.CodigoEmpresa = Usuario.Empresa?.Codigo ?? 0;
            else if (TipoServicoMultisoftware == AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.Terceiros)
                filtrosPesquisa.Proprietario = Usuario.ClienteTerceiro ?? null;
            else if (TipoServicoMultisoftware == AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.TransportadorTerceiro)
                filtrosPesquisa.Proprietario = Usuario.ClienteTerceiro ?? null;
            else if (TipoServicoMultisoftware != AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiTMS)
            {
                filtrosPesquisa.CodigosEmpresa = Request.GetListParam<int>("Empresas");

                if (filtrosPesquisa.CodigosEmpresa.Count == 0)
                    filtrosPesquisa.CodigoEmpresa = Request.GetIntParam("Empresa");
            }

            if ((codigoTipoCarga > 0) && (TipoServicoMultisoftware == AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiEmbarcador || TipoServicoMultisoftware == AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiCTe) && (filtrosPesquisa.PossiveisModelos == null || filtrosPesquisa.PossiveisModelos.Count == 0))
            {
                Repositorio.Embarcador.Cargas.TipoCargaModeloVeicular repositorioTipoCargaModeloVeicular = new Repositorio.Embarcador.Cargas.TipoCargaModeloVeicular(unitOfWork);
                List<Dominio.Entidades.Embarcador.Cargas.TipoCargaModeloVeicular> tipoCargaModeloVeicular = repositorioTipoCargaModeloVeicular.ConsultarPorTipoCarga(codigoTipoCarga);

                filtrosPesquisa.PossiveisModelos = (from obj in tipoCargaModeloVeicular select obj.ModeloVeicularCarga).ToList();
            }

            if (codigoCarga > 0)
            {
                filtrosPesquisa.FiltrarCadastrosAprovados = ConfiguracaoEmbarcador.UtilizarAlcadaAprovacaoVeiculo;
                Repositorio.Embarcador.Cargas.Carga repositorioCarga = new Repositorio.Embarcador.Cargas.Carga(unitOfWork);
                Dominio.Entidades.Embarcador.Cargas.Carga carga = repositorioCarga.BuscarPorCodigo(codigoCarga);

                if (carga.TipoOperacao != null && carga.TipoOperacao.PermitirQualquerModeloVeicular)
                {
                    filtrosPesquisa.ModeloVeicularCarga = null;
                    filtrosPesquisa.PossiveisModelos = null;
                }
            }

            if (filtrosPesquisa.CodigosEmpresa == null || filtrosPesquisa.CodigosEmpresa.Count == 0)
                filtrosPesquisa.CodigosEmpresa = ObterListaCodigoTransportadorPermitidosOperadorLogistica(unitOfWork);

            return filtrosPesquisa;
        }

        private string ObterPropriedadeOrdenar(string propriedadeOrdenar)
        {
            if (propriedadeOrdenar == "Empresa")
                return "Empresa.RazaoSocial";

            if (propriedadeOrdenar == "DescricaoTipo")
                return "Tipo";

            return propriedadeOrdenar;
        }

        private string PlacasReboque(IList<Dominio.Entidades.Veiculo> veiculosVinculados)
        {
            string placa = "";
            foreach (Dominio.Entidades.Veiculo veiculoVinculado in veiculosVinculados)
            {
                placa = ", " + veiculoVinculado.Placa;
            }
            return placa;
        }

        private bool ValidarCamposReferenteCIOT(Dominio.Entidades.Veiculo veiculo, Repositorio.UnitOfWork unitOfWork, out string erroValidacaoCIOT)
        {
            erroValidacaoCIOT = "";

            if (veiculo.Ativo && veiculo.Tipo == "T" && veiculo.Proprietario != null)
            {
                Dominio.Entidades.Embarcador.Pessoas.ModalidadeTransportadoraPessoas modalidadeTransportadoraPessoas = Servicos.Embarcador.CIOT.CIOT.ObterModalidadeTransportador(veiculo.Proprietario, unitOfWork);

                if (modalidadeTransportadoraPessoas != null && modalidadeTransportadoraPessoas.GerarCIOT)
                {
                    Repositorio.Embarcador.Veiculos.VeiculoMotorista repositorioVeiculoMotorista = new Repositorio.Embarcador.Veiculos.VeiculoMotorista(unitOfWork);
                    Dominio.Entidades.Embarcador.Veiculos.VeiculoMotorista motoristaPrincipal = repositorioVeiculoMotorista.BuscarVeiculoMotoristaPrincipal(veiculo.Codigo);

                    if (motoristaPrincipal?.Motorista == null && (string.IsNullOrWhiteSpace(motoristaPrincipal?.Nome) || string.IsNullOrWhiteSpace(motoristaPrincipal?.CPF)))
                    {
                        erroValidacaoCIOT = Localization.Resources.Veiculos.Veiculo.MotoristaObrigatorio;
                        return false;
                    }

                    if (string.IsNullOrWhiteSpace(veiculo.Chassi))
                    {
                        erroValidacaoCIOT = Localization.Resources.Veiculos.Veiculo.ChassiObrigatorio;
                        return false;
                    }
                }
            }

            return true;
        }

        private void VerificarCamposRastreador(Dominio.Entidades.Veiculo veiculo, Dominio.Entidades.Embarcador.Configuracao.ConfiguracaoTMS configuracao)
        {
            if (!veiculo.PossuiRastreador)
                return;
            if (configuracao.ObrigatorioCadastrarRastreadorNosVeiculos && (veiculo.Ativo || TipoServicoMultisoftware != AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiTMS) && (
                !veiculo.PossuiRastreador || veiculo.TecnologiaRastreador == null || veiculo.TipoComunicacaoRastreador == null || string.IsNullOrWhiteSpace(veiculo.NumeroEquipamentoRastreador)
            ))
            {
                throw new ControllerException(Localization.Resources.Veiculos.Veiculo.ObrigatorioIndicarExistenciaDoRastreadorTecnologiaComunicacaoNumeroDoEquipamento);
            }

            if (!veiculo.Ativo && TipoServicoMultisoftware == AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiTMS && (veiculo.PossuiRastreador || veiculo.TecnologiaRastreador != null || veiculo.TipoComunicacaoRastreador != null || !string.IsNullOrWhiteSpace(veiculo.NumeroEquipamentoRastreador)))
                throw new ControllerException(Localization.Resources.Veiculos.Veiculo.NaoPossivelInativarVeiculoComRastreador);
        }

        private void ValidarRastreadorVeiculoUnico(Dominio.Entidades.Veiculo veiculo, Repositorio.UnitOfWork unitOfWork)
        {
            Repositorio.Veiculo repositorioVeiculo = new Repositorio.Veiculo(unitOfWork);
            if (veiculo.PossuiRastreador && veiculo.TecnologiaRastreador != null && veiculo.TipoComunicacaoRastreador != null && !string.IsNullOrEmpty(veiculo.NumeroEquipamentoRastreador))
            {
                Dominio.Entidades.Veiculo veiculoCadastrado = repositorioVeiculo.BuscarVeiculoPorTodosDadosRastreador(veiculo.TecnologiaRastreador.Codigo, veiculo.TipoComunicacaoRastreador.Codigo, veiculo.NumeroEquipamentoRastreador, veiculo.Placa);
                if (veiculoCadastrado != null)
                {
                    if (veiculo.Codigo == 0 || (veiculo.Codigo > 0 && veiculo.Codigo != veiculoCadastrado.Codigo)) // está editando é diferente e ja existe outro
                        throw new ControllerException(string.Format(Localization.Resources.Veiculos.Veiculo.JaExisteVeiculoCadastradoComOsMesmosDadosDeTecnologiaComunicacaoNumeroDoEquipamentoPlaca, veiculoCadastrado.Placa_Formatada));
                }
            }
        }

        private void SalvarCurrais(Dominio.Entidades.Veiculo veiculo, Repositorio.UnitOfWork unitOfWork)
        {
            Repositorio.VeiculoCurral repositorioVeiculoCurral = new Repositorio.VeiculoCurral(unitOfWork);

            dynamic dynCurrais = Newtonsoft.Json.JsonConvert.DeserializeObject<dynamic>((string)Request.Params("ListaCurrais"));
            int numero = 1;

            List<Dominio.Entidades.VeiculoCurral> curraisDeletar = repositorioVeiculoCurral.BuscarPorVeiculo(veiculo.Codigo);

            foreach (Dominio.Entidades.VeiculoCurral curralDeletar in curraisDeletar)
                repositorioVeiculoCurral.Deletar(curralDeletar);

            foreach (dynamic dynCurral in dynCurrais)
            {
                Dominio.Entidades.VeiculoCurral curral = new Dominio.Entidades.VeiculoCurral()
                {
                    Comprimento = ((string)dynCurral[0].Comprimento).ToString().ToDecimal(),
                    Largura = ((string)dynCurral[0].Largura).ToString().ToDecimal(),
                    Veiculo = veiculo,
                    NumeroCurral = numero
                };

                numero += 1;

                repositorioVeiculoCurral.Inserir(curral);
            }
        }

        private void SalvarEquipamentos(Dominio.Entidades.Veiculo veiculo, Repositorio.UnitOfWork unitOfWork)
        {
            Repositorio.Embarcador.Veiculos.Equipamento repEquipamento = new Repositorio.Embarcador.Veiculos.Equipamento(unitOfWork);

            dynamic dynEquipamentos = Newtonsoft.Json.JsonConvert.DeserializeObject<dynamic>((string)Request.Params("Equipamentos"));
            if (veiculo.Equipamentos != null && veiculo.Equipamentos.Count > 0)
            {
                List<int> codigos = new List<int>();

                foreach (var equipamento in dynEquipamentos)
                    if (equipamento.Codigo != null)
                        codigos.Add((int)equipamento.Codigo);

                List<Dominio.Entidades.Embarcador.Veiculos.Equipamento> equipamentoRemover = (from obj in veiculo.Equipamentos where !codigos.Contains(obj.Codigo) select obj).ToList();

                for (var i = 0; i < equipamentoRemover.Count; i++)
                    veiculo.Equipamentos.Remove(equipamentoRemover[i]);
            }
            else
                veiculo.Equipamentos = new List<Dominio.Entidades.Embarcador.Veiculos.Equipamento>();

            foreach (var dynEquipamento in dynEquipamentos)
            {
                int codigoEquipamento = (int)dynEquipamento.Codigo;

                Dominio.Entidades.Embarcador.Veiculos.Equipamento equipamento = codigoEquipamento > 0 ? (from obj in veiculo.Equipamentos where obj.Codigo == codigoEquipamento select obj).FirstOrDefault() : null;
                if (equipamento == null)
                {
                    equipamento = repEquipamento.BuscarPorCodigo(codigoEquipamento);
                    veiculo.Equipamentos.Add(equipamento);
                }

                if (equipamento.CentroResultado != veiculo.CentroResultado)
                {
                    equipamento.CentroResultado = veiculo.CentroResultado;
                    repEquipamento.Atualizar(equipamento);
                }
            }
        }

        private void SalvarMotoristaPrincipal(Dominio.Entidades.Veiculo veiculo, Repositorio.UnitOfWork unitOfWork, Dominio.Entidades.Embarcador.Configuracao.ConfiguracaoTMS configuracao)
        {
            Repositorio.Veiculo repVeiculo = new Repositorio.Veiculo(unitOfWork);
            Repositorio.Embarcador.Veiculos.VeiculoMotorista repositorioVeiculoMotorista = new Repositorio.Embarcador.Veiculos.VeiculoMotorista(unitOfWork);

            Dominio.Entidades.Embarcador.Veiculos.VeiculoMotorista veiculoMotorista = PreencherVeiculoMotorista(repositorioVeiculoMotorista.BuscarVeiculoMotoristaPrincipal(veiculo.Codigo), veiculo, Request.GetIntParam("CodigoMotorista"), Request.GetStringParam("NomeMotorista"), Request.GetStringParam("CPF"), true, unitOfWork);

            if (configuracao.NaoPermitirVincularMotoristaEmVariosVeiculos)
            {
                if (veiculoMotorista?.Motorista != null)
                {
                    List<Dominio.Entidades.Veiculo> veiculosMotorista = repVeiculo.BuscarVeiculosPorMotorista(veiculoMotorista.Motorista.Codigo, veiculo.Codigo);
                    if (veiculosMotorista.Count > 0)
                        throw new ControllerException(Localization.Resources.Veiculos.Veiculo.MotoristaPrincipalInformaoJaEstaVinculadoAoVeiculo + $": {veiculosMotorista.FirstOrDefault().Placa}.");
                }
            }

            SalvarMotoristasSecundarios(veiculoMotorista, veiculo, unitOfWork, configuracao);
        }

        private void SalvarMotoristasSecundarios(Dominio.Entidades.Embarcador.Veiculos.VeiculoMotorista motoristaPrincipal, Dominio.Entidades.Veiculo veiculo, Repositorio.UnitOfWork unitOfWork, Dominio.Entidades.Embarcador.Configuracao.ConfiguracaoTMS configuracao)
        {
            Repositorio.Veiculo repVeiculo = new Repositorio.Veiculo(unitOfWork);
            Repositorio.Embarcador.Veiculos.VeiculoMotorista repositorioVeiculoMotorista = new Repositorio.Embarcador.Veiculos.VeiculoMotorista(unitOfWork);

            dynamic dynMotoristas = Newtonsoft.Json.JsonConvert.DeserializeObject<dynamic>((string)Request.Params("Motoristas"));

            List<Dominio.Entidades.Embarcador.Veiculos.VeiculoMotorista> motoristasSecundarios = repositorioVeiculoMotorista.BuscarVeiculoMotoristasSecundarios(veiculo.Codigo);

            if (motoristasSecundarios.Count > 0)
            {
                List<int> codigos = new List<int>();

                foreach (var dynMotorista in dynMotoristas)
                {
                    int codigo = ((string)dynMotorista.Codigo).ToInt();
                    if (codigo > 0)
                        codigos.Add(codigo);
                }

                List<Dominio.Entidades.Embarcador.Veiculos.VeiculoMotorista> motoristaRemover = (from obj in motoristasSecundarios where !codigos.Contains(obj.Codigo) select obj).ToList();

                foreach (Dominio.Entidades.Embarcador.Veiculos.VeiculoMotorista remover in motoristaRemover)
                {
                    Servicos.Auditoria.Auditoria.Auditar(Auditado, veiculo, null, Localization.Resources.Veiculos.Veiculo.RemoveuMotoristaSecundario + " - " + remover.Motorista?.Nome ?? remover.Nome, unitOfWork);
                    repositorioVeiculoMotorista.Deletar(remover);
                }
            }

            foreach (var dynMotorista in dynMotoristas)
            {
                int codigo = ((string)dynMotorista.Codigo).ToInt();
                if (codigo > 0)
                    continue;

                int codigoMotorista = ((string)dynMotorista.CodigoMotorista).ToInt();
                string nomeMotorista = (string)dynMotorista.Nome;
                string cpf = (string)dynMotorista.CPF;

                PreencherVeiculoMotorista(null, veiculo, codigoMotorista, nomeMotorista, cpf, false, unitOfWork);
            }

            List<Dominio.Entidades.Embarcador.Veiculos.VeiculoMotorista> secundariosValidacao = repositorioVeiculoMotorista.BuscarVeiculoMotoristasSecundarios(veiculo.Codigo);
            if (motoristaPrincipal != null && secundariosValidacao.Any(o => o.Motorista?.Codigo == motoristaPrincipal.Motorista?.Codigo))
                throw new ControllerException(Localization.Resources.Veiculos.Veiculo.MotoristaPrincipalNaoPodeEstarNosSecundariosTambem);

            if (motoristaPrincipal == null && secundariosValidacao.Count > 0)
                throw new ControllerException(Localization.Resources.Veiculos.Veiculo.SoPossivelAdicionarOsMotoristasSecundariosQuandoInformarPrincipal);

            if (configuracao.NaoPermitirVincularMotoristaEmVariosVeiculos)
            {
                foreach (Dominio.Entidades.Embarcador.Veiculos.VeiculoMotorista secundario in secundariosValidacao)
                {
                    if (secundario.Motorista != null)
                    {
                        List<Dominio.Entidades.Veiculo> veiculosMotorista = repVeiculo.BuscarVeiculosPorMotorista(secundario.Motorista.Codigo, veiculo.Codigo);
                        if (veiculosMotorista.Count > 0)
                            throw new ControllerException(Localization.Resources.Veiculos.Veiculo.MotoristaSecundarioInformadoJaEstaVinculadoAoVeiculo + $" {veiculosMotorista.FirstOrDefault().Placa}.");
                    }
                }
            }
        }

        private void SalvarLiberacaoGR(Dominio.Entidades.Veiculo veiculo, Repositorio.UnitOfWork unitOfWork)
        {
            var jsonLiberacoesGR = Request.Params("LiberacoesGR");

            //if (string.IsNullOrEmpty(jsonLiberacoesGR))
            //return;

            var repositorioLicenca = new Repositorio.Embarcador.Configuracoes.Licenca(unitOfWork);
            var repositorioVeiculoLiberacaoGR = new Repositorio.Embarcador.Veiculos.VeiculoLiberacaoGR(unitOfWork);

            var liberacoesGRNovas = JsonConvert.DeserializeObject<dynamic>(jsonLiberacoesGR);
            var liberacoesGRExistentes = repositorioVeiculoLiberacaoGR.BuscarPorCodigoVeiculo(veiculo.Codigo);
            var liberacoesGRSalvar = new List<Dominio.Entidades.Embarcador.Veiculos.VeiculoLiberacaoGR>();

            if (liberacoesGRNovas != null)
            {
                foreach (var nova in liberacoesGRNovas)
                {
                    int codigo;
                    if (!int.TryParse(nova.Codigo.ToString(), out codigo))
                    {
                        codigo = 0;
                    }

                    // Agora, você pode usar 'codigo' para a comparação
                    var liberacaoExistente = liberacoesGRExistentes.FirstOrDefault(l => l.Codigo == codigo);

                    if (liberacaoExistente == null)
                    {
                        int codigoLicenca;
                        if (!int.TryParse(nova.CodigoLicenca.ToString(), out codigoLicenca))
                        {
                            codigoLicenca = 0;
                        }
                        // Cria uma nova licença
                        var novaLiberacao = new Dominio.Entidades.Embarcador.Veiculos.VeiculoLiberacaoGR
                        {
                            Descricao = nova.Descricao.ToString().Replace("\"", ""),
                            DataEmissao = DateTime.Parse(nova.DataEmissao.ToString().Replace("\"", "")),
                            DataVencimento = DateTime.Parse(nova.DataVencimento.ToString().Replace("\"", "")),
                            Numero = nova.Numero.ToString().Replace("\"", ""),
                            Licenca = codigoLicenca > 0 ?
                                        repositorioLicenca.BuscarPorCodigo(codigoLicenca) :
                                        repositorioLicenca.BuscarPorDescricao(nova.DescricaoLicenca.ToString().Replace("\"", "")),
                            Veiculo = veiculo
                        };
                        repositorioVeiculoLiberacaoGR.Inserir(novaLiberacao, Auditado);
                        liberacoesGRSalvar.Add(novaLiberacao);
                    }
                    else
                    {
                        // Atualiza os dados da licença existente
                        liberacaoExistente.Descricao = nova.Descricao.ToString().Replace("\"", "");
                        liberacaoExistente.DataEmissao = DateTime.Parse(nova.DataEmissao.ToString().Replace("\"", ""));
                        liberacaoExistente.DataVencimento = DateTime.Parse(nova.DataVencimento.ToString().Replace("\"", ""));
                        liberacaoExistente.Numero = nova.Numero;

                        int codigoLicenca;
                        if (!int.TryParse(nova.CodigoLicenca.ToString(), out codigoLicenca))
                        {
                            codigoLicenca = 0;
                        }
                        liberacaoExistente.Licenca = codigoLicenca > 0 ?
                                                       repositorioLicenca.BuscarPorCodigo(codigoLicenca) :
                                                       repositorioLicenca.BuscarPorDescricao(nova.DescricaoLicenca.ToString().Replace("\"", ""));
                        repositorioVeiculoLiberacaoGR.Atualizar(liberacaoExistente, Auditado);
                        liberacoesGRSalvar.Add(liberacaoExistente);
                    }

                }
            }

            // Remove as licenças que não estão presentes no JSON enviado
            foreach (var existente in liberacoesGRExistentes)
            {
                var codigoExistente = existente.Codigo;
                if (!liberacoesGRSalvar.Any(n => n.Codigo == codigoExistente))
                {
                    repositorioVeiculoLiberacaoGR.Excluir(existente.Codigo);
                    Servicos.Auditoria.Auditoria.Auditar(Auditado, veiculo, $"Removido liberação de GR  {existente.Descricao}.", unitOfWork);

                }
            }
        }


        private Dominio.Entidades.Embarcador.Veiculos.VeiculoMotorista PreencherVeiculoMotorista(Dominio.Entidades.Embarcador.Veiculos.VeiculoMotorista veiculoMotorista, Dominio.Entidades.Veiculo veiculo, int codigoMotorista, string nome, string cpf, bool principal, Repositorio.UnitOfWork unitOfWork)
        {
            Repositorio.Usuario repMotorista = new Repositorio.Usuario(unitOfWork);
            Repositorio.Embarcador.Veiculos.VeiculoMotorista repositorioVeiculoMotorista = new Repositorio.Embarcador.Veiculos.VeiculoMotorista(unitOfWork);

            bool trocouMotorista = false;

            string mensagemAuditoria = principal ? "principal" : "secundário";

            if (codigoMotorista > 0)
            {
                Dominio.Entidades.Usuario motorista = repMotorista.BuscarPorCodigo(codigoMotorista);

                if (veiculoMotorista == null)
                {
                    veiculoMotorista = new Dominio.Entidades.Embarcador.Veiculos.VeiculoMotorista();
                    veiculoMotorista.Veiculo = veiculo;
                }
                else
                    veiculoMotorista.Initialize();

                trocouMotorista = veiculoMotorista.Motorista?.Codigo != motorista?.Codigo;

                veiculoMotorista.Motorista = motorista;

                if (motorista != null)
                {
                    veiculoMotorista.CPF = motorista.CPF;
                    veiculoMotorista.Nome = motorista.Nome;
                    motorista.Gestor = veiculo.FuncionarioResponsavel;

                    if (veiculo.Empresa != null && motorista.Empresa != null && TipoServicoMultisoftware == AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiEmbarcador) //TMS não possui empresa no motorista
                    {
                        if (motorista.Empresa.Codigo != veiculo.Empresa.Codigo && !motorista.Empresas.Contains(veiculo.Empresa) && !(veiculo.Empresa.Filiais?.Contains(motorista.Empresa) ?? false))
                            throw new ControllerException(string.Format(Localization.Resources.Veiculos.Veiculo.MotoristaInformadoNaoPertenceMesmaEmpresaDoVeiculo, motorista.Nome));
                    }

                    if (motorista.CentroResultado != veiculo.CentroResultado)
                    {
                        motorista.CentroResultado = veiculo.CentroResultado;
                        repMotorista.Atualizar(motorista);
                    }
                }

                veiculoMotorista.Nome = nome;

                string nomeMotorista = veiculoMotorista.Motorista?.Nome ?? veiculoMotorista.Nome;

                if (TipoServicoMultisoftware == AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.Terceiros || TipoServicoMultisoftware == AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.TransportadorTerceiro)
                    if (motorista.Empresa != null && veiculo.Empresa != null && veiculoMotorista.Motorista.Empresa.Codigo != veiculo.Empresa.Codigo && !veiculoMotorista.Motorista.Empresas.Contains(veiculo.Empresa) && !(veiculoMotorista.Motorista.Empresa.Filiais?.Contains(motorista.Empresa) ?? false))
                        throw new ControllerException(string.Format(Localization.Resources.Veiculos.Veiculo.MotoristaInformadoNaoPertenceMesmaEmpresaDoVeiculo, nomeMotorista));

                if (veiculoMotorista.Codigo > 0)
                {
                    repositorioVeiculoMotorista.Atualizar(veiculoMotorista);
                    List<Dominio.Entidades.Auditoria.HistoricoPropriedade> alteracoes = veiculoMotorista.GetChanges();

                    if (alteracoes.Count > 0)
                        Servicos.Auditoria.Auditoria.Auditar(Auditado, veiculo, alteracoes, Localization.Resources.Veiculos.Veiculo.AlterouMotorista + $" {mensagemAuditoria} - {nomeMotorista}", unitOfWork);
                }
                else
                {
                    veiculoMotorista.Principal = principal;
                    repositorioVeiculoMotorista.Inserir(veiculoMotorista);
                    Servicos.Auditoria.Auditoria.Auditar(Auditado, veiculo, null, Localization.Resources.Veiculos.Veiculo.AdicionouMotorista + $" {mensagemAuditoria} - {nomeMotorista}", unitOfWork);
                }
            }
            else
            {
                cpf = Utilidades.String.OnlyNumbers(cpf);
                if (veiculo.Empresa != null && !string.IsNullOrEmpty(cpf))
                {
                    if (veiculoMotorista == null)
                    {
                        veiculoMotorista = new Dominio.Entidades.Embarcador.Veiculos.VeiculoMotorista();
                        veiculoMotorista.Veiculo = veiculo;
                    }
                    else
                        veiculoMotorista.Initialize();

                    Dominio.Entidades.Usuario motorista = null;
                    if (TipoServicoMultisoftware == AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiTMS)
                        motorista = repMotorista.BuscarPorCPF(cpf);
                    if (motorista == null)
                    {
                        motorista = new Dominio.Entidades.Usuario();
                        motorista.CPF = cpf;
                        motorista.Nome = nome;
                        motorista.Localidade = veiculo.Empresa.Localidade;
                        motorista.Empresa = veiculo.Empresa;
                        motorista.Setor = new Dominio.Entidades.Setor() { Codigo = 1 };
                        motorista.Tipo = "M";
                        motorista.Status = "A";
                        motorista.TipoPessoa = "F";
                        motorista.CentroResultado = veiculo.CentroResultado;
                        repMotorista.Inserir(motorista);
                    }

                    motorista.Gestor = veiculo.FuncionarioResponsavel;

                    trocouMotorista = veiculoMotorista.Motorista?.Codigo != motorista?.Codigo;

                    veiculoMotorista.Motorista = motorista;
                    veiculoMotorista.CPF = cpf;
                    veiculoMotorista.Nome = nome;

                    if (motorista.CentroResultado != veiculo.CentroResultado)
                    {
                        motorista.CentroResultado = veiculo.CentroResultado;
                        repMotorista.Atualizar(motorista);
                    }

                    string nomeMotorista = veiculoMotorista.Motorista?.Nome ?? veiculoMotorista.Nome;
                    if (veiculoMotorista.Codigo > 0)
                    {
                        repositorioVeiculoMotorista.Atualizar(veiculoMotorista);
                        List<Dominio.Entidades.Auditoria.HistoricoPropriedade> alteracoes = veiculoMotorista.GetChanges();

                        if (alteracoes.Count > 0)
                            Servicos.Auditoria.Auditoria.Auditar(Auditado, veiculo, alteracoes, Localization.Resources.Veiculos.Veiculo.AlterouMotorista + $" {mensagemAuditoria} - {nomeMotorista}", unitOfWork);
                    }
                    else
                    {
                        veiculoMotorista.Principal = principal;
                        repositorioVeiculoMotorista.Inserir(veiculoMotorista);
                        Servicos.Auditoria.Auditoria.Auditar(Auditado, veiculo, null, Localization.Resources.Veiculos.Veiculo.AdicionouMotorista + $" {mensagemAuditoria} - {nomeMotorista}", unitOfWork);
                    }
                }
                else
                {
                    if (veiculoMotorista != null)
                    {
                        string nomeMotorista = veiculoMotorista.Motorista?.Nome ?? veiculoMotorista.Nome;
                        Servicos.Auditoria.Auditoria.Auditar(Auditado, veiculo, null, Localization.Resources.Veiculos.Veiculo.RemoveuMotorista + $" {mensagemAuditoria} - {nomeMotorista}", unitOfWork);
                        repositorioVeiculoMotorista.Deletar(veiculoMotorista);
                        veiculoMotorista = null;
                    }
                }
            }

            if (trocouMotorista)
                Servicos.Embarcador.Veiculo.Veiculo.AtualizarIntegracoesTrocaMotorista(veiculo, unitOfWork);

            return veiculoMotorista;
        }

        private void SalvarLicencas(Dominio.Entidades.Veiculo veiculo, Repositorio.UnitOfWork unitOfWork)
        {
            Servicos.Embarcador.Veiculo.LicencaVeiculo servicoLicencaVeiculo = new Servicos.Embarcador.Veiculo.LicencaVeiculo(unitOfWork, TipoServicoMultisoftware);
            List<Dominio.ObjetosDeValor.Embarcador.Veiculos.LicencaVeiculoSalvar> licencas = JsonConvert.DeserializeObject<List<Dominio.ObjetosDeValor.Embarcador.Veiculos.LicencaVeiculoSalvar>>((string)Request.Params("Licencas"));

            servicoLicencaVeiculo.AdicionarOuAtualizar(veiculo, licencas, Auditado, false);
        }

        private void SalvarConfiguracoesTarget(Dominio.Entidades.Veiculo veiculo)
        {
            veiculo.ModoCompraValePedagioTarget = Request.GetNullableEnumParam<Dominio.ObjetosDeValor.Embarcador.Enumeradores.ModoCompraValePedagioTarget>("ModoCompraValePedagioTarget");
        }

        private void ProcessarCadastroVeiculo(Dominio.Entidades.Veiculo veiculo, bool adicionandoVeiculo, Repositorio.UnitOfWork unitOfWork)
        {
            Repositorio.Veiculo repVeiculo = new Repositorio.Veiculo(unitOfWork);
            Repositorio.Embarcador.Veiculos.AlcadasCadastroVeiculo.CadastroVeiculo repCadastroVeiculo = new Repositorio.Embarcador.Veiculos.AlcadasCadastroVeiculo.CadastroVeiculo(unitOfWork);

            if (!ConfiguracaoEmbarcador.UtilizarAlcadaAprovacaoVeiculo)
            {
                if (TipoServicoMultisoftware == AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiEmbarcador || TipoServicoMultisoftware == AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiTMS)
                {
                    veiculo.SituacaoCadastro = SituacaoCadastroVeiculo.Aprovado;
                    repVeiculo.Atualizar(veiculo);
                }
                return;
            }

            if (TipoServicoMultisoftware == AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiCTe || TipoServicoMultisoftware == AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiEmbarcador || TipoServicoMultisoftware == AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiTMS)
            {
                veiculo.SituacaoCadastro = SituacaoCadastroVeiculo.Pendente;
                repVeiculo.Atualizar(veiculo);

                Dominio.Entidades.Embarcador.Veiculos.AlcadasCadastroVeiculo.CadastroVeiculo cadastroVeiculoAtivo = repCadastroVeiculo.BuscarUltimoCadastroVeiculo(veiculo.Codigo);
                if (cadastroVeiculoAtivo != null)
                {
                    cadastroVeiculoAtivo.Finalizado = true;
                    repCadastroVeiculo.Atualizar(cadastroVeiculoAtivo);
                }

            }

            Servicos.Embarcador.Veiculo.Aprovacao servicoAprovacaoCadastro = new Servicos.Embarcador.Veiculo.Aprovacao(unitOfWork);

            Dominio.Entidades.Embarcador.Veiculos.AlcadasCadastroVeiculo.CadastroVeiculo cadastroVeiculo = new Dominio.Entidades.Embarcador.Veiculos.AlcadasCadastroVeiculo.CadastroVeiculo()
            {
                Veiculo = veiculo,
                Usuario = this.Usuario,
                DataCadastro = DateTime.Now,
                Tipo = adicionandoVeiculo ? TipoCadastroVeiculo.Adicionar : TipoCadastroVeiculo.Atualizar
            };

            repCadastroVeiculo.Inserir(cadastroVeiculo);

            servicoAprovacaoCadastro.EtapaAprovacao(cadastroVeiculo, TipoServicoMultisoftware);

            repVeiculo.Atualizar(cadastroVeiculo.Veiculo);
        }

        private dynamic ObterDadosAutorizacaoCadastroVeiculo(Dominio.Entidades.Veiculo veiculo, Repositorio.UnitOfWork unitOfWork)
        {
            if (!ConfiguracaoEmbarcador.UtilizarAlcadaAprovacaoVeiculo)
                return null;

            Repositorio.Embarcador.Veiculos.AlcadasCadastroVeiculo.AprovacaoAlcadaCadastroVeiculo repAprovacaoAlcadaCadastroVeiculo = new Repositorio.Embarcador.Veiculos.AlcadasCadastroVeiculo.AprovacaoAlcadaCadastroVeiculo(unitOfWork);
            Repositorio.Embarcador.Veiculos.AlcadasCadastroVeiculo.CadastroVeiculo repCadastroVeiculo = new Repositorio.Embarcador.Veiculos.AlcadasCadastroVeiculo.CadastroVeiculo(unitOfWork);
            Dominio.Entidades.Embarcador.Veiculos.AlcadasCadastroVeiculo.CadastroVeiculo cadastroVeiculos = repCadastroVeiculo.BuscarUltimoCadastroVeiculo(veiculo.Codigo);

            List<Dominio.Entidades.Embarcador.Veiculos.AlcadasCadastroVeiculo.AprovacaoAlcadaCadastroVeiculo> rejeicoes = repAprovacaoAlcadaCadastroVeiculo.ObterAutorizacoesRejeitadas(cadastroVeiculos?.Codigo ?? 0);

            return new
            {
                Codigo = cadastroVeiculos?.Codigo ?? 0,
                Solicitate = cadastroVeiculos?.Usuario?.Nome ?? string.Empty,
                DataCadastro = cadastroVeiculos?.DataCadastro.ToDateTimeString() ?? string.Empty,
                Tipo = cadastroVeiculos?.Tipo.ObterDescricao() ?? string.Empty,
                SituacaoDescricao = veiculo.SituacaoCadastro.ObterDescricao(),
                Situacao = veiculo.SituacaoCadastro,
                MotivoRejeicao = string.Join("<br>", rejeicoes.Select(o => $"<strong>{o.Usuario.Nome}</strong>: {o.Motivo}"))

                //QuantidadeAprovados = aprovacoes.Where(o => o.Situacao == SituacaoAlcadaRegra.Aprovada).Count(),
                //NumeroAprovadores = aprovacoes.FirstOrDefault()?.NumeroAprovadores ?? 0,
            };

        }

        private async Task<dynamic> ObterDadosRastreadorAsync(Dominio.Entidades.Veiculo veiculo, Repositorio.UnitOfWork unitOfWork)
        {
            if (TipoServicoMultisoftware == AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiEmbarcador)
            {
                List<AdminMultisoftware.Dominio.Enumeradores.PermissaoPersonalizada> permissoesPersonalizadas = ObterPermissoesPersonalizadas("Veiculos/Veiculo");
                if (!(this.Usuario.UsuarioAdministrador || permissoesPersonalizadas.Contains(AdminMultisoftware.Dominio.Enumeradores.PermissaoPersonalizada.Veiculo_PermitirAcessoRastreador)))
                    return null;
            }

            if (!ConfiguracaoEmbarcador.PossuiMonitoramento)
                return null;

            Repositorio.Embarcador.Logistica.Posicao repositorioPosicao = new Repositorio.Embarcador.Logistica.Posicao(unitOfWork);

            if (veiculo.TecnologiaRastreador == null)
            {
                return new
                {
                    Terminal = veiculo.NumeroEquipamentoRastreador,
                    DataUltimaPosicao = "",
                    Rastreador = false
                };
            }

            string nomeConta = veiculo.TecnologiaRastreador.NomeConta;
            DateTime? dataUltimaPosicao = null;

            Dominio.ObjetosDeValor.Embarcador.Enumeradores.EnumTecnologiaGerenciadora tipoGerenciadora =
                Dominio.ObjetosDeValor.Embarcador.Enumeradores.EnumTecnologiaGerenciadoraHelper.ObterEnumPorDescricao(nomeConta);

            if (tipoGerenciadora != Dominio.ObjetosDeValor.Embarcador.Enumeradores.EnumTecnologiaGerenciadora.NaoDefinido)
            {
                dataUltimaPosicao = await repositorioPosicao.BuscarDataUltimaPorGerenciadoraAsync(veiculo.Codigo, tipoGerenciadora);
            }
            else
            {
                Dominio.ObjetosDeValor.Embarcador.Enumeradores.EnumTecnologiaRastreador tipoRastreador =
                    Dominio.ObjetosDeValor.Embarcador.Enumeradores.EnumTecnologiaRastreadorHelper.ObterEnumPorDescricao(nomeConta);

                if (tipoRastreador != Dominio.ObjetosDeValor.Embarcador.Enumeradores.EnumTecnologiaRastreador.NaoDefinido)
                {
                    dataUltimaPosicao = await repositorioPosicao.BuscarDataUltimaPorRastreadorAsync(veiculo.Codigo, tipoRastreador);
                }
            }

            bool sinalAtivo = dataUltimaPosicao.HasValue &&
                              dataUltimaPosicao.Value.AddMinutes(ConfiguracaoEmbarcador.TempoSemPosicaoParaVeiculoPerderSinal) > DateTime.Now;

            return new
            {
                Terminal = veiculo.NumeroEquipamentoRastreador,
                DataUltimaPosicao = dataUltimaPosicao?.ToString("dd/MM/yyyy HH:mm:ss") ?? "",
                Rastreador = sinalAtivo
            };
        }


        private string ObterPlacasVeiculosVinculados(Dominio.Entidades.Veiculo veiculo)
        {
            if (veiculo.IsTipoVeiculoReboque())
                return veiculo.VeiculosTracao?.FirstOrDefault()?.Placa ?? string.Empty;

            return string.Join(", ", veiculo.VeiculosVinculados.Select(r => r.Placa).ToList());
        }
        private string ObterCodigosVeiculosVinculados(Dominio.Entidades.Veiculo veiculo)
        {
            if (veiculo.IsTipoVeiculoReboque())
                return veiculo.VeiculosTracao?.FirstOrDefault()?.Codigo.ToString() ?? string.Empty;

            return string.Join(", ", veiculo.VeiculosVinculados.Select(r => r.Codigo).ToList());
        }

        private void SalvarVeiculoRotasFrete(Dominio.Entidades.Veiculo veiculo, Repositorio.UnitOfWork unitOfWork)
        {
            Repositorio.Embarcador.Veiculos.VeiculoRotasFrete repositorioVeiculoRotasFrete = new Repositorio.Embarcador.Veiculos.VeiculoRotasFrete(unitOfWork);
            Repositorio.RotaFrete repositorioRotaFrete = new Repositorio.RotaFrete(unitOfWork);

            dynamic dynRotasFrete = Newtonsoft.Json.JsonConvert.DeserializeObject<dynamic>(Request.Params("RotasFrete"));

            List<Dominio.Entidades.Embarcador.Veiculos.VeiculoRotasFrete> veiculoRotasFrete = repositorioVeiculoRotasFrete.BuscarPorVeiculo(veiculo.Codigo);
            List<Dominio.ObjetosDeValor.Auditoria.HistoricoPropriedade> alteracoes = new List<Dominio.ObjetosDeValor.Auditoria.HistoricoPropriedade>();

            if (veiculoRotasFrete.Count > 0)
            {
                List<int> codigos = new List<int>();

                foreach (dynamic rotaFrete in dynRotasFrete)
                {
                    int codigo = ((string)rotaFrete.Codigo).ToInt();
                    if (codigo > 0)
                        codigos.Add(codigo);
                }

                List<Dominio.Entidades.Embarcador.Veiculos.VeiculoRotasFrete> listaDeletar = (from obj in veiculoRotasFrete where !codigos.Contains(obj.RotaFrete.Codigo) select obj).ToList();

                foreach (var deletar in listaDeletar)
                {
                    alteracoes.Add(new Dominio.ObjetosDeValor.Auditoria.HistoricoPropriedade()
                    {
                        Propriedade = Localization.Resources.Veiculos.Veiculo.RotaDeFrete,
                        De = $"{deletar.RotaFrete.Descricao}",
                        Para = ""
                    });

                    repositorioVeiculoRotasFrete.Deletar(deletar);
                }
            }

            foreach (dynamic rotafrete in dynRotasFrete)
            {
                int codigo = ((string)rotafrete.Codigo).ToInt();

                Dominio.Entidades.Embarcador.Veiculos.VeiculoRotasFrete veiculoRotasFreteEntidade = codigo > 0 ? repositorioVeiculoRotasFrete.BuscarPorVeiculoERotaFrete(veiculo.Codigo, codigo) : null;

                if (veiculoRotasFreteEntidade == null)
                    veiculoRotasFreteEntidade = new Dominio.Entidades.Embarcador.Veiculos.VeiculoRotasFrete();

                int codigoRotaFrete = ((string)rotafrete.Codigo).ToInt();

                veiculoRotasFreteEntidade.Veiculo = veiculo;
                veiculoRotasFreteEntidade.RotaFrete = repositorioRotaFrete.BuscarPorCodigo(codigoRotaFrete);

                alteracoes.Add(new Dominio.ObjetosDeValor.Auditoria.HistoricoPropriedade()
                {
                    Propriedade = Localization.Resources.Veiculos.Veiculo.RotaDeFrete,
                    De = "",
                    Para = $"{veiculoRotasFreteEntidade.RotaFrete.Descricao}"
                });

                if (veiculoRotasFreteEntidade.Codigo > 0)
                    repositorioVeiculoRotasFrete.Atualizar(veiculoRotasFreteEntidade);
                else
                    repositorioVeiculoRotasFrete.Inserir(veiculoRotasFreteEntidade);

            }

            veiculo.SetExternalChanges(alteracoes);
        }

        public async Task<IActionResult> ConfiguracaoImportacaoVeiculosContratoFreteTransportador()
        {
            List<Dominio.ObjetosDeValor.Embarcador.Importacao.ConfiguracaoImportacao> configuracoes = ObterConfiguracaoImportacaoVeiculos();

            return new JsonpResult(configuracoes.ToList());
        }

        private List<Dominio.ObjetosDeValor.Embarcador.Importacao.ConfiguracaoImportacao> ObterConfiguracaoImportacaoVeiculos()
        {
            List<Dominio.ObjetosDeValor.Embarcador.Importacao.ConfiguracaoImportacao> configuracoes = new List<Dominio.ObjetosDeValor.Embarcador.Importacao.ConfiguracaoImportacao>
            {
                new Dominio.ObjetosDeValor.Embarcador.Importacao.ConfiguracaoImportacao() { Id = 1, Descricao = "Placa", Propriedade = "Placa", Tamanho = 50, CampoInformacao = true, Obrigatorio = true, Regras = new List<string> { "required" } },
            };

            return configuracoes;
        }

        private void BloquearOutrosCadastroVeiculos(Dominio.Entidades.Veiculo veiculo, Repositorio.UnitOfWork unitOfWork)
        {
            Repositorio.Veiculo repVeiculo = new Repositorio.Veiculo(unitOfWork);
            Repositorio.Embarcador.Configuracoes.ConfiguracaoVeiculo repConfiguracaoVeiculo = new Repositorio.Embarcador.Configuracoes.ConfiguracaoVeiculo(unitOfWork);
            Dominio.Entidades.Embarcador.Configuracoes.ConfiguracaoVeiculo configuracaoVeiculo = repConfiguracaoVeiculo.BuscarConfiguracaoPadrao();

            if (!(veiculo.VeiculoBloqueado && configuracaoVeiculo.NaoPermitirRealizarCadastroPlacaBloqueada))
                return;

            List<Dominio.Entidades.Veiculo> listaVeiculosIguailBloquear = repVeiculo.BuscarVeiculosMesmaPlacaSemBloqueio(veiculo.Placa);

            foreach (Dominio.Entidades.Veiculo veiculoMesmaPlaca in listaVeiculosIguailBloquear)
            {
                veiculoMesmaPlaca.Initialize();

                veiculoMesmaPlaca.VeiculoBloqueado = true;
                veiculoMesmaPlaca.MotivoBloqueio = veiculo.MotivoBloqueio;
                repVeiculo.Atualizar(veiculoMesmaPlaca, Auditado);
            }
        }

        private void DesbloquearOutrosCadastroVeiculos(Dominio.Entidades.Veiculo veiculo, Repositorio.UnitOfWork unitOfWork)
        {
            Repositorio.Veiculo repVeiculo = new Repositorio.Veiculo(unitOfWork);

            if (veiculo.VeiculoBloqueado)
                return;

            List<Dominio.Entidades.Veiculo> listaVeiculosIguailBloquear = repVeiculo.BuscarVeiculosMesmaPlacaSemBloqueio(veiculo.Placa, true);

            foreach (Dominio.Entidades.Veiculo veiculoMesmaPlaca in listaVeiculosIguailBloquear)
            {
                veiculoMesmaPlaca.Initialize();

                veiculoMesmaPlaca.VeiculoBloqueado = false;
                veiculoMesmaPlaca.MotivoBloqueio = "";
                repVeiculo.Atualizar(veiculoMesmaPlaca, Auditado);
            }
        }

        private void AtualizarResponsavelVeiculo(Dominio.Entidades.Veiculo veiculo, Repositorio.UnitOfWork unitOfWork)
        {
            Repositorio.Embarcador.Veiculos.ResponsavelVeiculo repResponsavelVeiculo = new Repositorio.Embarcador.Veiculos.ResponsavelVeiculo(unitOfWork);
            Dominio.Entidades.Embarcador.Veiculos.ResponsavelVeiculo responsavelVeiculo = repResponsavelVeiculo.BuscarPorVeiculo(veiculo.Codigo);

            if (responsavelVeiculo != null)
            {
                responsavelVeiculo.FuncionarioResponsavel = veiculo.FuncionarioResponsavel;
                responsavelVeiculo.DataLancamento = DateTime.Now;
                responsavelVeiculo.FuncionarioLancamento = this.Usuario;

                repResponsavelVeiculo.Atualizar(responsavelVeiculo);
            }
        }
        #endregion
    }
}