using System.Collections.Generic;
using System.Linq;

namespace Servicos
{
    public class Veiculo : ServicoBase
    {
        public Veiculo(Repositorio.UnitOfWork unitOfWork) : base(unitOfWork) { }
        #region Métodos Públicos

        public bool ValidarPlaca(string placa, Dominio.Entidades.Embarcador.Configuracao.ConfiguracaoTMS configuracaoTMS)
        {
            return ValidarPlaca(placa, null, configuracaoTMS);
        }

        public bool ValidarPlaca(string placa, Repositorio.UnitOfWork unitOfWork, Dominio.Entidades.Embarcador.Configuracao.ConfiguracaoTMS configuracaoTMS = null)
        {
            if (configuracaoTMS == null)
            {
                Repositorio.Embarcador.Configuracao.ConfiguracaoTMS repConfiguracaoTMS = new Repositorio.Embarcador.Configuracao.ConfiguracaoTMS(unitOfWork);
                configuracaoTMS = repConfiguracaoTMS.BuscarConfiguracaoPadrao();
            }

            bool valida = true;
            string placaFormatada = Utilidades.String.RemoveAllSpecialCharacters(placa);

            switch (configuracaoTMS.Pais)
            {
                case Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoPais.Brasil:
                    if (placaFormatada.Length != 7)
                        valida = false;
                    break;
                case Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoPais.Exterior:
                    if (placaFormatada.Length > 8 || placaFormatada.Length < 5)
                        valida = false;
                    break;
            }

            return valida;
        }

        public Dominio.ObjetosDeValor.CTe.Veiculo ObterVeiculoCTE(Dominio.Entidades.Veiculo veiculo, Dominio.Entidades.Empresa empresaFilialEmissora)
        {
            Cliente serCliente = new Cliente(StringConexao);

            Dominio.ObjetosDeValor.CTe.Veiculo veiculoCTE = new Dominio.ObjetosDeValor.CTe.Veiculo();
            veiculoCTE.CodigoVeiculo = veiculo.Codigo;
            veiculoCTE.CapacidadeKG = veiculo.CapacidadeKG;
            veiculoCTE.CapacidadeM3 = veiculo.CapacidadeM3;
            if (veiculoCTE.CapacidadeKG > 99999)
                veiculoCTE.CapacidadeKG = 10000;

            veiculoCTE.Chassi = veiculo.Chassi;
            veiculoCTE.Placa = veiculo.Placa;
            veiculoCTE.TipoPropriedade = veiculo.Tipo;
            veiculoCTE.RNTRCProprietario = veiculo.RNTRC;
            if (veiculo.Proprietario != null)
                veiculoCTE.Proprietario = serCliente.ObterClienteCTE(veiculo.Proprietario, null);
            else if (empresaFilialEmissora != null && veiculo.Empresa != null && empresaFilialEmissora.Codigo != veiculo.Empresa.Codigo)
            {
                veiculoCTE.Proprietario = serCliente.ConverterEmpresaParaCliente(veiculo.Empresa);
                if (veiculoCTE.Proprietario != null)
                {
                    veiculoCTE.TipoPropriedade = "T";
                    int.TryParse(veiculo.Empresa.RegistroANTT, out int rntrcEmpresa);
                    veiculoCTE.RNTRCProprietario = rntrcEmpresa;
                }
            }

            veiculoCTE.Renavam = veiculo.Renavam;
            veiculoCTE.Tara = veiculo.Tara;
            if (veiculoCTE.Tara > 99999)
                veiculoCTE.Tara = 10000;

            veiculoCTE.TipoCarroceria = veiculo.TipoCarroceria;
            veiculoCTE.TipoProprietario = veiculo.TipoProprietario;
            veiculoCTE.TipoRodado = veiculo.TipoRodado;
            veiculoCTE.TipoVeiculo = veiculo.TipoVeiculo;
            veiculoCTE.UF = veiculo.Estado.Sigla;
            return veiculoCTE;
        }

        public Dominio.ObjetosDeValor.CTe.Veiculo ObterVeiculoCTE(Dominio.Entidades.VeiculoCTE veiculo)
        {
            Cliente serCliente = new Cliente(StringConexao);
            Dominio.ObjetosDeValor.CTe.Veiculo veiculoCTE = new Dominio.ObjetosDeValor.CTe.Veiculo();
            veiculoCTE.CodigoVeiculo = veiculo.Codigo;

            veiculoCTE.CapacidadeKG = veiculo.CapacidadeKG;
            if (veiculoCTE.CapacidadeKG > 99999)
                veiculoCTE.CapacidadeKG = 10000;

            veiculoCTE.CapacidadeM3 = veiculo.CapacidadeM3;
            veiculoCTE.Placa = veiculo.Placa;
            veiculoCTE.Chassi = veiculo.Veiculo.Chassi;
            veiculoCTE.Proprietario = serCliente.ObterClienteCTE(veiculo.Veiculo.Proprietario, null);
            veiculoCTE.Renavam = veiculo.RENAVAM;
            veiculoCTE.RNTRCProprietario = veiculo.Veiculo.RNTRC;

            veiculoCTE.Tara = veiculo.Tara;
            if (veiculoCTE.Tara > 99999)
                veiculoCTE.Tara = 10000;

            veiculoCTE.TipoCarroceria = veiculo.TipoCarroceria;
            veiculoCTE.TipoPropriedade = veiculo.TipoPropriedade;
            veiculoCTE.TipoProprietario = veiculo.Veiculo.TipoProprietario;
            veiculoCTE.TipoRodado = veiculo.TipoRodado;
            veiculoCTE.TipoVeiculo = veiculo.TipoVeiculo;
            veiculoCTE.UF = veiculo.Estado.Sigla;
            return veiculoCTE;
        }

        public static List<Dominio.Entidades.Veiculo> ObterReboques(Dominio.Entidades.Veiculo veiculo)
        {
            List<Dominio.Entidades.Veiculo> reboques = new List<Dominio.Entidades.Veiculo>();

            if (veiculo.TipoVeiculo == "1")
                reboques.Add(veiculo);

            foreach (Dominio.Entidades.Veiculo reboque in veiculo.VeiculosVinculados)
                if (reboque.TipoVeiculo == "1")
                    reboques.Add(reboque);

            return reboques;
        }

        public static Dominio.Entidades.Veiculo ObterTracao(Dominio.Entidades.Veiculo veiculo)
        {
            Dominio.Entidades.Veiculo tracao = null;

            if (veiculo == null)
                return null;

            if (veiculo.TipoVeiculo == "0")
                tracao = veiculo;
            else
                tracao = veiculo.VeiculosVinculados?.Where(o => o.TipoVeiculo == "0").FirstOrDefault();

            return tracao;
        }

        public bool SalvarViculosMatrizFilial(Dominio.Entidades.Veiculo veiculo, Repositorio.UnitOfWork unitOfWork, Dominio.ObjetosDeValor.Embarcador.Auditoria.Auditado auditado = null)
        {
            Repositorio.Veiculo repVeiculo = new Repositorio.Veiculo(unitOfWork);
            Repositorio.Empresa repEmpresa = new Repositorio.Empresa(unitOfWork);

            Dominio.Entidades.Empresa empresa = repEmpresa.BuscarPorCodigo(veiculo.Empresa.Codigo);

            if (empresa != null)
            {
                if (empresa.Filiais.Count > 0)
                {
                    foreach (Dominio.Entidades.Empresa empresaFilial in empresa.Filiais)
                    {
                        SalvarVeiculo(veiculo, empresaFilial, unitOfWork, auditado);
                        SalvarVinculosMatrizFilial(veiculo, empresaFilial, unitOfWork, auditado);
                    }
                }

                Dominio.Entidades.Empresa empresaMatriz = repEmpresa.BuscarEmpresaMatriz(empresa);
                if (empresaMatriz != null)
                {
                    SalvarVeiculo(veiculo, empresaMatriz, unitOfWork, auditado);
                    SalvarVinculosMatrizFilial(veiculo, empresaMatriz, unitOfWork, auditado);

                    if (empresaMatriz.Filiais.Count > 0)
                    {
                        foreach (Dominio.Entidades.Empresa empresaFilial in empresaMatriz.Filiais)
                        {
                            if (empresaFilial.Codigo != veiculo.Empresa.Codigo)
                            {
                                SalvarVeiculo(veiculo, empresaFilial, unitOfWork, auditado);
                                SalvarVinculosMatrizFilial(veiculo, empresaFilial, unitOfWork, auditado);
                            }
                        }
                    }
                }
            }
            return true;

        }

        public static Dominio.Entidades.Veiculo PreencherVeiculoGenerico(string placa, Dominio.Entidades.Empresa empresa, Repositorio.UnitOfWork unitOfWork)
        {
            Repositorio.Veiculo repVeiculo = new Repositorio.Veiculo(unitOfWork);
            Repositorio.Empresa repEmpresa = new Repositorio.Empresa(unitOfWork);

            Dominio.Entidades.Veiculo veiculo = new Dominio.Entidades.Veiculo()
            {
                Estado = empresa?.Localidade?.Estado ?? repEmpresa.BuscarPrimeiroEstado(),
                Placa = placa,
                ModeloVeicularCarga = null,
                Empresa = empresa,
                GrupoPessoas = null,
                Ativo = true,
                KilometragemAtual = 0,
                Chassi = "",
                DataCompra = null,
                ValorAquisicao = 0,
                CapacidadeTanque = 0,
                CapacidadeM3 = 0,
                SituacaoCadastro = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoCadastroVeiculo.Aprovado,
                DataLicenca = null,
                AnoFabricacao = 0,
                AnoModelo = 0,
                Situacao = "Q",
                NumeroMotor = "",
                PendenteIntegracaoEmbarcador = true,
                MediaPadrao = 0,
                Observacao = "CADASTRO GENÉRICO",
                DataVencimentoGarantiaPlena = null,
                DataVencimentoGarantiaEscalonada = null,
                Renavam = "12345678901",
                Contrato = "",
                Tara = 0,
                XCampo = "",
                XTexto = "",
                Tipo = "P",
                TipoCombustivel = "",
                TipoVeiculo = "0",
                TipoRodado = "00",
                TipoCarroceria = "00",
                CapacidadeKG = 0,
                Modelo = null,
                Marca = null,
                ModeloCarroceria = null,
                TipoDoVeiculo = null,
                NumeroFrota = "",
                DataRemocaoVinculo = null,
                SegmentoVeiculo = null,
                RNTRC = 0,
                TipoProprietario = Dominio.Enumeradores.TipoProprietarioVeiculo.Outros,
                ObservacaoCTe = "",
                Proprietario = null,
                CIOT = "",
                TAF = "",
                NroRegEstadual = "",
                FornecedorValePedagio = null,
                ResponsavelValePedagio = null,
                NumeroCompraValePedagio = "",
                ValorValePedagio = 0,
                PossuiTagValePedagio = false,
                DataInicioVigenciaTagValePedagio = null,
                DataFimVigenciaTagValePedagio = null,
                ValorContainerAverbacao = 0
            };

            repVeiculo.Inserir(veiculo);

            return repVeiculo.BuscarPorCodigo(veiculo.Codigo);
        }

        public string BuscarConjuntoFrota(Dominio.Entidades.Veiculo veiculo)
        {
            string conjuntoFrota = string.IsNullOrWhiteSpace(veiculo.NumeroFrota) ? string.Empty : veiculo.NumeroFrota + ", ";

            if (veiculo.TipoVeiculo == "0")
            {
                foreach (Dominio.Entidades.Veiculo reboque in veiculo.VeiculosVinculados)
                {
                    conjuntoFrota += string.IsNullOrWhiteSpace(reboque.NumeroFrota) ? string.Empty : reboque.NumeroFrota + ", ";
                }
            }
            else
            {
                if (veiculo.VeiculosTracao != null && veiculo.VeiculosTracao.Count > 0)
                {
                    Dominio.Entidades.Veiculo tracao = veiculo.VeiculosTracao.FirstOrDefault();

                    conjuntoFrota = string.IsNullOrWhiteSpace(tracao.NumeroFrota) ? string.Empty : tracao.NumeroFrota + ", ";

                    foreach (Dominio.Entidades.Veiculo reboque in tracao.VeiculosVinculados)
                    {
                        conjuntoFrota += string.IsNullOrWhiteSpace(reboque.NumeroFrota) ? string.Empty : reboque.NumeroFrota + ", ";
                    }
                }
            }

            conjuntoFrota = Utilidades.String.Left(conjuntoFrota, conjuntoFrota.Length - 2);

            return conjuntoFrota;
        }

        public string BuscarReboquesComModeloVeicular(Dominio.Entidades.Veiculo veiculo)
        {

            string modelo = "";
            if (veiculo.ModeloVeicularCarga != null)
                modelo = " ( " + veiculo.ModeloVeicularCarga.Descricao + " )";

            string conjunto = veiculo.Placa + modelo;
            if (veiculo.TipoVeiculo == "0")
            {
                foreach (Dominio.Entidades.Veiculo reboque in veiculo.VeiculosVinculados)
                {
                    string modeloreboque = "";
                    if (reboque.ModeloVeicularCarga != null)
                        modeloreboque = " ( " + reboque.ModeloVeicularCarga.Descricao + " )";
                    conjunto += ", " + reboque.Placa + modeloreboque;
                }
            }
            else
            {
                if (veiculo.VeiculosTracao != null && veiculo.VeiculosTracao.Count > 0)
                {
                    Dominio.Entidades.Veiculo tracao = veiculo.VeiculosTracao.FirstOrDefault();
                    string modelotracao = "";
                    if (tracao.ModeloVeicularCarga != null)
                        modelotracao = " ( " + tracao.ModeloVeicularCarga.Descricao + " )";
                    conjunto = tracao.Placa + modelotracao;

                    foreach (Dominio.Entidades.Veiculo reboque in tracao.VeiculosVinculados)
                    {
                        string modeloreboque = "";
                        if (reboque.ModeloVeicularCarga != null)
                            modeloreboque = " ( " + reboque.ModeloVeicularCarga.Descricao + " )";
                        conjunto += ", " + reboque.Placa + modeloreboque;
                    }
                }
            }

            return conjunto;
        }

        public string BuscarReboquesComModeloVeicularEFrota(Dominio.Entidades.Veiculo veiculo)
        {
            string conjunto = veiculo.ModeloVeicularCarga == null ? string.Concat(veiculo.Placa, (!string.IsNullOrWhiteSpace(veiculo.NumeroFrota) ? " (F: " + veiculo.NumeroFrota + ")" : "")) : string.Concat(veiculo.Placa, " (", (!string.IsNullOrWhiteSpace(veiculo.NumeroFrota) ? "F: " + veiculo.NumeroFrota + " / " : ""), veiculo.ModeloVeicularCarga.Descricao, ")"); ;

            if (veiculo.TipoVeiculo == "0")
            {
                if (veiculo.VeiculosVinculados.Count > 0)
                    conjunto += ", " + string.Join(", ", (from reboque in veiculo.VeiculosVinculados select reboque.ModeloVeicularCarga == null ? string.Concat(reboque.Placa, (!string.IsNullOrWhiteSpace(reboque.NumeroFrota) ? " (F: " + reboque.NumeroFrota + ")" : "")) : string.Concat(reboque.Placa, " (", (!string.IsNullOrWhiteSpace(reboque.NumeroFrota) ? "F: " + reboque.NumeroFrota + " / " : ""), reboque.ModeloVeicularCarga.Descricao, ")")));
            }
            else
            {
                if (veiculo.VeiculosTracao != null && veiculo.VeiculosTracao.Count > 0)
                {
                    Dominio.Entidades.Veiculo tracao = veiculo.VeiculosTracao.FirstOrDefault();

                    conjunto = tracao.ModeloVeicularCarga == null ? string.Concat(tracao.Placa, (!string.IsNullOrWhiteSpace(tracao.NumeroFrota) ? " (F: " + tracao.NumeroFrota + ")" : "")) : string.Concat(tracao.Placa, " (", (!string.IsNullOrWhiteSpace(tracao.NumeroFrota) ? "F: " + tracao.NumeroFrota + " / " : ""), tracao.ModeloVeicularCarga.Descricao, ")");

                    if (tracao.VeiculosVinculados.Count > 0)
                        conjunto += ", " + string.Join(", ", (from reboque in tracao.VeiculosVinculados select reboque.ModeloVeicularCarga == null ? string.Concat(reboque.Placa, (!string.IsNullOrWhiteSpace(reboque.NumeroFrota) ? " (F: " + reboque.NumeroFrota + ")" : "")) : string.Concat(reboque.Placa, " (", (!string.IsNullOrWhiteSpace(reboque.NumeroFrota) ? "F: " + reboque.NumeroFrota + " / " : ""), reboque.ModeloVeicularCarga.Descricao, ")")));
                }
            }

            return conjunto;
        }

        public string BuscarReboquesSemModeloVeicular(Dominio.Entidades.Veiculo veiculo)
        {
            string conjunto = veiculo.Placa;
            if (veiculo.TipoVeiculo == "0")
            {
                foreach (Dominio.Entidades.Veiculo reboque in veiculo.VeiculosVinculados)
                {
                    conjunto += ", " + reboque.Placa;
                }
            }
            else
            {
                if (veiculo.VeiculosTracao != null && veiculo.VeiculosTracao.Count > 0)
                {
                    Dominio.Entidades.Veiculo tracao = veiculo.VeiculosTracao.FirstOrDefault();
                    conjunto = tracao.Placa;

                    foreach (Dominio.Entidades.Veiculo reboque in tracao.VeiculosVinculados)
                    {
                        conjunto += ", " + reboque.Placa;
                    }
                }
            }

            return conjunto;
        }

        #endregion

        #region Métodos Privados

        private void SalvarVeiculo(Dominio.Entidades.Veiculo veiculo, Dominio.Entidades.Empresa empresa, Repositorio.UnitOfWork unitOfWork, Dominio.ObjetosDeValor.Embarcador.Auditoria.Auditado auditado)
        {
            if (veiculo != null && empresa != null)
            {
                Repositorio.Veiculo repVeiculo = new Repositorio.Veiculo(unitOfWork);
                Repositorio.Embarcador.Veiculos.VeiculoMotorista repVeiculoMotorista = new Repositorio.Embarcador.Veiculos.VeiculoMotorista(unitOfWork);
                bool situacaoAnterior = false;

                Dominio.Entidades.Veiculo veiculoFilial = repVeiculo.BuscarTodosPorPlaca(empresa.Codigo, veiculo.Placa);
                if (veiculoFilial == null)
                    veiculoFilial = new Dominio.Entidades.Veiculo();
                else situacaoAnterior = veiculoFilial.Ativo;

                veiculoFilial.Empresa = empresa;
                veiculoFilial.Placa = veiculo.Placa;
                veiculoFilial.Renavam = veiculo.Renavam;
                veiculoFilial.Estado = veiculo.Estado;
                veiculoFilial.AnoFabricacao = veiculo.AnoFabricacao;
                veiculoFilial.AnoModelo = veiculo.AnoModelo;
                veiculoFilial.Ativo = veiculo.Ativo;
                veiculoFilial.CapacidadeKG = veiculo.CapacidadeKG;
                veiculoFilial.CapacidadeM3 = veiculo.CapacidadeM3;
                veiculoFilial.CapacidadeTanque = veiculo.CapacidadeTanque;
                veiculoFilial.Chassi = veiculo.Chassi;
                veiculoFilial.Contrato = veiculo.Contrato;
                //veiculoFilial.CPFMotorista = veiculo.CPFMotorista;
                //veiculoFilial.NomeMotorista = veiculo.NomeMotorista;
                veiculoFilial.NumeroFrota = veiculo.NumeroFrota;
                veiculoFilial.NumeroMotor = veiculo.NumeroMotor;
                veiculoFilial.PendenteIntegracaoEmbarcador = true;
                veiculoFilial.Observacao = veiculo.Observacao;
                veiculoFilial.ObservacaoCTe = veiculo.ObservacaoCTe;
                if (veiculo.Proprietario != null)
                    veiculoFilial.Proprietario = veiculo.Proprietario;
                veiculoFilial.RNTRC = veiculo.RNTRC;
                veiculoFilial.Situacao = veiculo.Situacao;
                veiculoFilial.Tara = veiculo.Tara;
                veiculoFilial.Tipo = veiculo.Tipo;
                veiculoFilial.TipoCarroceria = veiculo.TipoCarroceria;
                veiculoFilial.TipoCombustivel = veiculo.TipoCombustivel;
                veiculoFilial.TipoRodado = veiculo.TipoRodado;
                veiculoFilial.TipoVeiculo = veiculo.TipoVeiculo;
                veiculoFilial.ValorAquisicao = veiculo.ValorAquisicao;
                veiculoFilial.KilometragemAtual = veiculo.KilometragemAtual;
                veiculoFilial.ValorAquisicao = veiculo.ValorAquisicao;
                veiculoFilial.DataCompra = veiculo.DataCompra;
                veiculoFilial.DataLicenca = veiculo.DataLicenca;
                veiculoFilial.MediaPadrao = veiculo.MediaPadrao;
                veiculoFilial.DataVencimentoGarantiaPlena = veiculo.DataVencimentoGarantiaPlena;
                veiculoFilial.DataVencimentoGarantiaEscalonada = veiculo.DataVencimentoGarantiaEscalonada;

                veiculoFilial.CIOT = veiculo.CIOT;

                if (veiculo.FornecedorValePedagio != null)
                    veiculoFilial.FornecedorValePedagio = veiculo.FornecedorValePedagio;
                if (veiculo.ResponsavelValePedagio != null)
                    veiculoFilial.ResponsavelValePedagio = veiculo.ResponsavelValePedagio;
                veiculoFilial.NumeroCompraValePedagio = veiculo.NumeroCompraValePedagio;
                veiculoFilial.ValorValePedagio = veiculo.ValorValePedagio;

                //veiculoFilial.TipoDoVeiculo = veiculo.TipoDoVeiculo;
                //veiculoFilial.Modelo = veiculo.Modelo;
                //veiculoFilial.Marca = veiculo.Marca;
                //veiculoFilial.ModeloVeicularCarga = veiculo.ModeloVeicularCarga;

                if (veiculoFilial.Codigo > 0)
                {
                    Servicos.Embarcador.Veiculo.VeiculoHistorico.InserirHistoricoVeiculo(veiculo, situacaoAnterior, Dominio.ObjetosDeValor.Embarcador.Enumeradores.MetodosAlteracaoVeiculo.SalvarVeiculo_Veiculo, null, unitOfWork);
                    repVeiculo.Atualizar(veiculoFilial, auditado);
                }
                else
                    repVeiculo.Inserir(veiculoFilial);

                if (veiculo.Motoristas?.Count > 0)
                {
                    Dominio.Entidades.Embarcador.Veiculos.VeiculoMotorista veiculoMotoristaPrincipal = repVeiculoMotorista.BuscarVeiculoMotoristaPrincipal(veiculoFilial.Codigo);
                    if (veiculoMotoristaPrincipal != null)
                    {
                        veiculoMotoristaPrincipal.Motorista = veiculo.Motoristas.FirstOrDefault().Motorista;
                        veiculoMotoristaPrincipal.CPF = veiculo.Motoristas.FirstOrDefault().Motorista.CPF;
                        veiculoMotoristaPrincipal.Nome = veiculo.Motoristas.FirstOrDefault().Motorista.Nome;
                        repVeiculoMotorista.Atualizar(veiculoMotoristaPrincipal);
                    }
                    else
                    {
                        veiculoMotoristaPrincipal = new Dominio.Entidades.Embarcador.Veiculos.VeiculoMotorista();
                        veiculoMotoristaPrincipal.Motorista = veiculo.Motoristas.FirstOrDefault().Motorista;
                        veiculoMotoristaPrincipal.CPF = veiculo.Motoristas.FirstOrDefault().Motorista.CPF;
                        veiculoMotoristaPrincipal.Nome = veiculo.Motoristas.FirstOrDefault().Motorista.Nome;
                        veiculoMotoristaPrincipal.Principal = true;
                        veiculoMotoristaPrincipal.Veiculo = veiculoFilial;
                        repVeiculoMotorista.Inserir(veiculoMotoristaPrincipal);
                    }
                }

            }
        }

        private void SalvarVinculosMatrizFilial(Dominio.Entidades.Veiculo veiculo, Dominio.Entidades.Empresa filial, Repositorio.UnitOfWork unitOfWork, Dominio.ObjetosDeValor.Embarcador.Auditoria.Auditado auditado)
        {
            if (veiculo != null)
            {
                Repositorio.Veiculo repVeiculo = new Repositorio.Veiculo(unitOfWork);
                Repositorio.VeiculoMotoristas repVeiculoMotoristas = new Repositorio.VeiculoMotoristas(unitOfWork);
                Dominio.Entidades.Veiculo veiculoFilial = repVeiculo.BuscarTodosPorPlaca(filial.Codigo, veiculo.Placa);

                if (veiculoFilial?.VeiculosVinculados != null && veiculoFilial?.VeiculosVinculados.Count > 0)
                {
                    while (veiculoFilial.VeiculosVinculados.Count() > 0)
                        veiculoFilial.VeiculosVinculados.Remove(veiculoFilial.VeiculosVinculados[0]);
                    repVeiculo.Atualizar(veiculoFilial);

                    veiculoFilial = repVeiculo.BuscarTodosPorPlaca(filial.Codigo, veiculo.Placa);
                }

                if (veiculo.VeiculosVinculados != null && veiculo.VeiculosVinculados.Count > 0)
                {
                    foreach (Dominio.Entidades.Veiculo veiculoVinculado in veiculo.VeiculosVinculados) //Percorre veiculos matriz
                    {
                        Dominio.Entidades.Veiculo veiculoVinculadoFilial = repVeiculo.BuscarTodosPorPlaca(filial.Codigo, veiculoVinculado.Placa);
                        if (veiculoVinculadoFilial == null)
                        {
                            this.SalvarVeiculo(veiculoVinculado, filial, unitOfWork, auditado);
                            veiculoVinculadoFilial = repVeiculo.BuscarTodosPorPlaca(filial.Codigo, veiculoVinculado.Placa);
                        }
                        veiculoFilial.VeiculosVinculados.Add(veiculoVinculadoFilial);
                        repVeiculo.Atualizar(veiculoFilial, auditado);
                    }
                }

                if (veiculo.VeiculoMotoristas != null)
                {
                    // Lista de motoristas ja existente
                    List<Dominio.Entidades.VeiculoMotoristas> motoristasVeiculoFilial = new List<Dominio.Entidades.VeiculoMotoristas>();
                    if (veiculoFilial.VeiculoMotoristas != null)
                        motoristasVeiculoFilial.AddRange(veiculoFilial.VeiculoMotoristas.ToList());

                    // Motoristas pendetes para replicar
                    List<Dominio.Entidades.Usuario> motoristas = (from o in veiculo.VeiculoMotoristas where !motoristasVeiculoFilial.Any(m => m.Motorista.Codigo == o.Motorista.Codigo) select o.Motorista).ToList();

                    // Motoristas que nao estao na lista
                    List<Dominio.Entidades.VeiculoMotoristas> motoristasRemover = (from o in motoristasVeiculoFilial where !veiculo.VeiculoMotoristas.Any(m => m.Motorista.Codigo == o.Motorista.Codigo) select o).ToList();

                    foreach (Dominio.Entidades.Usuario motorista in motoristas)
                    {
                        Dominio.Entidades.VeiculoMotoristas motoristaAdicional = new Dominio.Entidades.VeiculoMotoristas()
                        {
                            Veiculo = veiculoFilial,
                            Motorista = motorista
                        };
                        repVeiculoMotoristas.Inserir(motoristaAdicional);
                    }

                    foreach (Dominio.Entidades.VeiculoMotoristas motoristaAdicional in motoristasRemover)
                        repVeiculoMotoristas.Deletar(motoristaAdicional);
                }
            }

        }

        #endregion
    }
}
