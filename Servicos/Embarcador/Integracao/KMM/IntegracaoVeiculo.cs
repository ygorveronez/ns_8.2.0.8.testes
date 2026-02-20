using Dominio.ObjetosDeValor.Embarcador.Enumeradores;
using Dominio.ObjetosDeValor.Embarcador.Importacao;
using Dominio.ObjetosDeValor.Embarcador.Integracao.KMM;
using Microsoft.AspNetCore.Routing.Matching;
using Newtonsoft.Json;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

namespace Servicos.Embarcador.Integracao.KMM
{
    public partial class IntegracaoKMM
    {
        #region Métodos Públicos
        public void IntegrarVeiculo(Dominio.Entidades.Embarcador.Veiculos.VeiculoIntegracao veiculoIntegracao)
        {
            Repositorio.Embarcador.Veiculos.VeiculoIntegracao repositorioVeiculoIntegracao = new Repositorio.Embarcador.Veiculos.VeiculoIntegracao(_unitOfWork);
            Repositorio.Embarcador.Configuracoes.IntegracaoKMM repositorioIntegracaoKMM = new Repositorio.Embarcador.Configuracoes.IntegracaoKMM(_unitOfWork);
            Repositorio.Embarcador.Integracao.IntegracaoCodigoExterno repIntegracaoCodigoExterno = new Repositorio.Embarcador.Integracao.IntegracaoCodigoExterno(_unitOfWork);

            ArquivoTransacao<Dominio.Entidades.Embarcador.Cargas.CargaCTeIntegracaoArquivo> servicoArquivoTransacao = new ArquivoTransacao<Dominio.Entidades.Embarcador.Cargas.CargaCTeIntegracaoArquivo>(_unitOfWork);

            var configuracaoIntegracaoKMM = repositorioIntegracaoKMM.Buscar();

            string jsonRequisicao = "";
            string jsonRetorno = "";

            veiculoIntegracao.DataIntegracao = DateTime.Now;
            veiculoIntegracao.NumeroTentativas++;

            try
            {
                string codigoExternoVeiculo = repIntegracaoCodigoExterno.BuscarPorVeiculoETipo(veiculoIntegracao.Veiculo.Codigo, TipoCodigoExternoIntegracao.Veiculo, TipoIntegracao.KMM)?.CodigoExterno ?? null;

                if (string.IsNullOrEmpty(codigoExternoVeiculo))
                {
                    codigoExternoVeiculo = this.ObterCodigoVeiculo(configuracaoIntegracaoKMM, veiculoIntegracao.Veiculo);
                }

                string codigoExternoMarca = repIntegracaoCodigoExterno.BuscarPorMarcaETipo(veiculoIntegracao.Veiculo.Marca?.Codigo ?? 0, TipoCodigoExternoIntegracao.VeiculoMarca, TipoIntegracao.KMM)?.CodigoExterno ?? null;

                if (string.IsNullOrEmpty(codigoExternoMarca) && veiculoIntegracao.Veiculo.Marca != null)
                {
                    codigoExternoMarca = this.ObterCodigoMarca(configuracaoIntegracaoKMM, veiculoIntegracao.Veiculo);
                }

                string codigoExternoModelo = repIntegracaoCodigoExterno.BuscarPorModeloETipo(veiculoIntegracao.Veiculo.Modelo?.Codigo ?? 0, TipoCodigoExternoIntegracao.VeiculoModelo, TipoIntegracao.KMM)?.CodigoExterno ?? null;

                if (string.IsNullOrEmpty(codigoExternoModelo) && veiculoIntegracao.Veiculo.Modelo != null)
                {
                    codigoExternoModelo = this.ObterCodigoModelo(configuracaoIntegracaoKMM, veiculoIntegracao.Veiculo);
                }

                string codigoExternoCarroceria = repIntegracaoCodigoExterno.BuscarPorModeloVeicularETipo(veiculoIntegracao.Veiculo.ModeloVeicularCarga?.Codigo ?? 0, TipoCodigoExternoIntegracao.ModeloVeicular, TipoIntegracao.KMM)?.CodigoExterno ?? null;

                if (string.IsNullOrEmpty(codigoExternoCarroceria) && veiculoIntegracao.Veiculo.ModeloVeicularCarga != null)
                {
                    codigoExternoCarroceria = this.ObterCodigoCarroceira(configuracaoIntegracaoKMM, veiculoIntegracao.Veiculo);

                }

                string codigoExternoCarroceriaAgrupamento = repIntegracaoCodigoExterno.BuscarPorModeloVeicularETipo(veiculoIntegracao.Veiculo.ModeloVeicularCarga?.Codigo ?? 0, TipoCodigoExternoIntegracao.ModeloVeicularAgrupamento, TipoIntegracao.KMM)?.CodigoExterno ?? null;

                string codigoExternoProprietario = "";

                string codigoExternoLocalidade = "";

                if (veiculoIntegracao.Veiculo.Tipo.Equals("P"))
                {
                    codigoExternoProprietario = repIntegracaoCodigoExterno.BuscarPorCPFCNPJETipo(veiculoIntegracao.Veiculo.EmpresaFilial?.CNPJ.ToString(), TipoCodigoExternoIntegracao.Pessoa, TipoIntegracao.KMM)?.CodigoExterno ?? null;

                    if (string.IsNullOrEmpty(codigoExternoProprietario) && veiculoIntegracao.Veiculo.EmpresaFilial != null)
                    {
                        codigoExternoProprietario = this.ObterCodigoEmpresa(configuracaoIntegracaoKMM, veiculoIntegracao.Veiculo.EmpresaFilial);
                    }

                    codigoExternoLocalidade = repIntegracaoCodigoExterno.BuscarPorLocalidadeETipo(veiculoIntegracao.Veiculo.EmpresaFilial?.Localidade?.Codigo ?? 0, TipoCodigoExternoIntegracao.Localidade, TipoIntegracao.KMM)?.CodigoExterno ?? null;

                    if (string.IsNullOrEmpty(codigoExternoLocalidade) && veiculoIntegracao.Veiculo.EmpresaFilial?.Localidade?.CodigoIBGE != null)
                    {
                        codigoExternoLocalidade = this.ObterCodigoLocalidade(configuracaoIntegracaoKMM, veiculoIntegracao.Veiculo);
                    }
                }
                else
                {
                    codigoExternoProprietario = repIntegracaoCodigoExterno.BuscarPorCPFCNPJETipo(veiculoIntegracao.Veiculo.Proprietario?.CPF_CNPJ.ToString(), TipoCodigoExternoIntegracao.Pessoa, TipoIntegracao.KMM)?.CodigoExterno ?? null;

                    if (string.IsNullOrEmpty(codigoExternoProprietario) && veiculoIntegracao.Veiculo.Proprietario != null)
                    {
                        codigoExternoProprietario = this.ObterCodigoPessoa(configuracaoIntegracaoKMM, veiculoIntegracao.Veiculo.Proprietario, null);
                    }

                    codigoExternoLocalidade = repIntegracaoCodigoExterno.BuscarPorLocalidadeETipo(veiculoIntegracao.Veiculo.Proprietario?.Localidade?.Codigo ?? 0, TipoCodigoExternoIntegracao.Localidade, TipoIntegracao.KMM)?.CodigoExterno ?? null;

                    if (string.IsNullOrEmpty(codigoExternoLocalidade) && veiculoIntegracao.Veiculo.Proprietario?.Localidade?.CodigoIBGE != null)
                    {
                        codigoExternoLocalidade = this.ObterCodigoLocalidade(configuracaoIntegracaoKMM, veiculoIntegracao.Veiculo);
                    }
                }

                Dominio.ObjetosDeValor.Embarcador.Integracao.KMM.Veiculo objetoVeiculo = ObterVeiculo(veiculoIntegracao.Veiculo, configuracaoIntegracaoKMM, codigoExternoVeiculo, codigoExternoProprietario, codigoExternoMarca, codigoExternoModelo, codigoExternoLocalidade, codigoExternoCarroceria, codigoExternoCarroceriaAgrupamento);

                Hashtable request = new Hashtable
                {
                    { "module", "M1076" },
                    { "operation", "manipulaVeiculo" },
                    { "parameters", objetoVeiculo }
                };

                var retWS = this.Transmitir(configuracaoIntegracaoKMM, request);

                this.ValidarCodigoErroRetorno(ref retWS, veiculoIntegracao.Veiculo, veiculoIntegracao, configuracaoIntegracaoKMM);

                veiculoIntegracao.SituacaoIntegracao = retWS.SituacaoIntegracao;
                veiculoIntegracao.ProblemaIntegracao = retWS.ProblemaIntegracao;

                jsonRequisicao = retWS.jsonRequisicao;
                jsonRetorno = retWS.jsonRetorno;

            }
            catch (Exception excecao)
            {
                Log.TratarErro(excecao);

                veiculoIntegracao.SituacaoIntegracao = SituacaoIntegracao.ProblemaIntegracao;
                veiculoIntegracao.ProblemaIntegracao = "Ocorreu uma falha ao realizar a integração da KMM";
            }

            servicoArquivoTransacao.Adicionar(veiculoIntegracao, jsonRequisicao, jsonRetorno, "json");

            repositorioVeiculoIntegracao.Atualizar(veiculoIntegracao);
        }
        #endregion

        #region Métodos Privados
        public string ObterCodigoEmpresa(Dominio.Entidades.Embarcador.Configuracoes.IntegracaoKMM configuracaoIntegracaoKMM, Dominio.Entidades.Empresa empresa)
        {
            Repositorio.Embarcador.Integracao.IntegracaoCodigoExterno repIntegracaoCodigoExterno = new Repositorio.Embarcador.Integracao.IntegracaoCodigoExterno(_unitOfWork);

            Hashtable objetoPessoa = new Hashtable();
            objetoPessoa.Add("cod_pessoa", repIntegracaoCodigoExterno.BuscarPorCPFCNPJETipo(empresa?.CNPJ.ToString(), TipoCodigoExternoIntegracao.Pessoa, TipoIntegracao.KMM)?.CodigoExterno ?? "");

            if (empresa != null)
            {
                objetoPessoa.Add("cnpj_cpf", empresa?.CNPJ_SemFormato ??  "");
                objetoPessoa.Add("pessoa_estrangeira_nome", "");
                objetoPessoa.Add("pessoa_estrangeira_numero", "");                
            }

            Hashtable request = new Hashtable
                {
                    { "module", "M1076" },
                    { "operation", "getCodPessoa" },
                    { "parameters", objetoPessoa }
                };

            var retWS = this.Transmitir(configuracaoIntegracaoKMM, request, "GET");

            return this.PrencherCodigoGetCodPessoa(retWS, empresa?.CNPJ.ToString() ?? "");

        }
        private void ValidarCodigoErroRetorno(ref Dominio.ObjetosDeValor.Embarcador.Integracao.KMM.retornoWebService retWS, Dominio.Entidades.Veiculo veiculo, Dominio.Entidades.Embarcador.Veiculos.VeiculoIntegracao veiculoIntegracao, Dominio.Entidades.Embarcador.Configuracoes.IntegracaoKMM configuracaoIntegracaoKMM)
        {
            if (retWS.CodigoErro > 0)
            {
                List<int> codigoErroLimparCodigosExternos = new List<int> { 01002, 01003, 01004, 01005, 01006, 01007, 01008, 01009, 01010 };

                int erro = retWS.CodigoErro;
                if (codigoErroLimparCodigosExternos.Any(x => x == erro))
                {
                    Repositorio.Embarcador.Integracao.IntegracaoCodigoExterno repIntegracaoCodigoExterno = new Repositorio.Embarcador.Integracao.IntegracaoCodigoExterno(_unitOfWork);

                    var IntegracaoCodigoExternoVeiculo = repIntegracaoCodigoExterno.BuscarPorVeiculoETipo(veiculoIntegracao.Veiculo.Codigo, TipoCodigoExternoIntegracao.Veiculo, TipoIntegracao.KMM);
                    if (IntegracaoCodigoExternoVeiculo != null)
                        repIntegracaoCodigoExterno.Deletar(IntegracaoCodigoExternoVeiculo);
                    string codigoExternoVeiculo = this.ObterCodigoVeiculo(configuracaoIntegracaoKMM, veiculoIntegracao.Veiculo);
                    

                    var IntegracaoCodigoExternoMarca = repIntegracaoCodigoExterno.BuscarPorMarcaETipo(veiculoIntegracao.Veiculo.Marca?.Codigo ?? 0, TipoCodigoExternoIntegracao.VeiculoMarca, TipoIntegracao.KMM);
                    if (IntegracaoCodigoExternoMarca != null)
                        repIntegracaoCodigoExterno.Deletar(IntegracaoCodigoExternoMarca);
                    string codigoExternoMarca = this.ObterCodigoMarca(configuracaoIntegracaoKMM, veiculoIntegracao.Veiculo);
                    

                    var IntegracaoCodigoExternoModelo = repIntegracaoCodigoExterno.BuscarPorModeloETipo(veiculoIntegracao.Veiculo.Modelo?.Codigo ?? 0, TipoCodigoExternoIntegracao.VeiculoModelo, TipoIntegracao.KMM);
                    if (IntegracaoCodigoExternoModelo != null)
                        repIntegracaoCodigoExterno.Deletar(IntegracaoCodigoExternoModelo);
                    string codigoExternoModelo = this.ObterCodigoModelo(configuracaoIntegracaoKMM, veiculoIntegracao.Veiculo);
                    

                    var IntegracaoCdigoExternoCarroceria = repIntegracaoCodigoExterno.BuscarPorModeloVeicularETipo(veiculoIntegracao.Veiculo.ModeloVeicularCarga?.Codigo ?? 0, TipoCodigoExternoIntegracao.ModeloVeicular, TipoIntegracao.KMM);
                    if (IntegracaoCdigoExternoCarroceria != null)
                        repIntegracaoCodigoExterno.Deletar(IntegracaoCdigoExternoCarroceria);
                    string codigoExternoCarroceria = this.ObterCodigoCarroceira(configuracaoIntegracaoKMM, veiculoIntegracao.Veiculo);

                    string codigoExternoProprietario = "";

                    string codigoExternoLocalidade = "";

                    if (veiculo.Tipo.Equals("P"))
                    {
                        var IntegracaoExternoEmpresa = repIntegracaoCodigoExterno.BuscarPorCPFCNPJETipo(veiculoIntegracao.Veiculo.EmpresaFilial?.CNPJ.ToString() ?? "", TipoCodigoExternoIntegracao.Pessoa, TipoIntegracao.KMM);
                        if (IntegracaoExternoEmpresa != null)
                            repIntegracaoCodigoExterno.Deletar(IntegracaoExternoEmpresa);
                        codigoExternoProprietario = this.ObterCodigoEmpresa(configuracaoIntegracaoKMM, veiculoIntegracao.Veiculo.EmpresaFilial);

                        var IntegracaoCdigoExternoLocalidade = repIntegracaoCodigoExterno.BuscarPorLocalidadeETipo(veiculoIntegracao.Veiculo.EmpresaFilial?.Localidade?.Codigo ?? 0, TipoCodigoExternoIntegracao.Localidade, TipoIntegracao.KMM);
                        if (IntegracaoCdigoExternoLocalidade != null)
                            repIntegracaoCodigoExterno.Deletar(IntegracaoCdigoExternoLocalidade);
                        codigoExternoLocalidade = this.ObterCodigoLocalidade(configuracaoIntegracaoKMM, veiculoIntegracao.Veiculo);

                    }
                    else
                    {
                        var IntegracaoCodigoExternoProprietario = repIntegracaoCodigoExterno.BuscarPorCPFCNPJETipo(veiculoIntegracao.Veiculo.Proprietario?.CPF_CNPJ.ToString(), TipoCodigoExternoIntegracao.Pessoa, TipoIntegracao.KMM);
                        if (IntegracaoCodigoExternoProprietario != null)
                            repIntegracaoCodigoExterno.Deletar(IntegracaoCodigoExternoProprietario);
                         codigoExternoProprietario = this.ObterCodigoPessoa(configuracaoIntegracaoKMM, veiculoIntegracao.Veiculo.Proprietario, null);

                        var IntegracaoCdigoExternoLocalidade = repIntegracaoCodigoExterno.BuscarPorLocalidadeETipo(veiculoIntegracao.Veiculo.Proprietario?.Localidade?.Codigo ?? 0, TipoCodigoExternoIntegracao.Localidade, TipoIntegracao.KMM);
                        if (IntegracaoCdigoExternoLocalidade != null)
                            repIntegracaoCodigoExterno.Deletar(IntegracaoCdigoExternoLocalidade);
                        codigoExternoLocalidade = this.ObterCodigoLocalidade(configuracaoIntegracaoKMM, veiculoIntegracao.Veiculo);

                    }

                    string codigoExternoCarroceriaAgrupamento = repIntegracaoCodigoExterno.BuscarPorModeloVeicularETipo(veiculoIntegracao.Veiculo.ModeloVeicularCarga?.Codigo ?? 0, TipoCodigoExternoIntegracao.ModeloVeicularAgrupamento, TipoIntegracao.KMM)?.CodigoExterno ?? null;


                    Dominio.ObjetosDeValor.Embarcador.Integracao.KMM.Veiculo objetoVeiculo = ObterVeiculo(veiculoIntegracao.Veiculo, configuracaoIntegracaoKMM, codigoExternoVeiculo, codigoExternoProprietario, codigoExternoMarca, codigoExternoModelo, codigoExternoLocalidade, codigoExternoCarroceria, codigoExternoCarroceriaAgrupamento);

                    Hashtable request = new Hashtable
                    {
                        { "module", "M1076" },
                        { "operation", "manipulaVeiculo" },
                        { "parameters", objetoVeiculo }
                    };

                    retWS = this.Transmitir(configuracaoIntegracaoKMM, request);
                }
            }

        }
        private Dominio.ObjetosDeValor.Embarcador.Integracao.KMM.Veiculo ObterVeiculo(Dominio.Entidades.Veiculo veiculo, Dominio.Entidades.Embarcador.Configuracoes.IntegracaoKMM configuracaoIntegracaoKMM, string codigoExternoVeiculo, string codigoExternoProprietario, string codigoExternoMarca, string codigoExternoModelo, string codigoExternoLocalidade, string codigoExternoCarroceria, string codigoExternoCarroceriaAgrupamento)
        {
            var retorno = new Dominio.ObjetosDeValor.Embarcador.Integracao.KMM.Veiculo();

            retorno.Operation = "INSERT";
            retorno.VeiculoId = string.IsNullOrEmpty(codigoExternoVeiculo) ? null : codigoExternoVeiculo;


            retorno.Frota = veiculo.NumeroFrota;
            retorno.Placa = veiculo.Placa;

            if (veiculo.Marca != null)
            {
                retorno.Marca = veiculo.Marca.Descricao;
                retorno.MarcaId = string.IsNullOrEmpty(codigoExternoMarca) ? null : codigoExternoMarca;
            }
                
            if (veiculo.Modelo != null)
            {
                retorno.Modelo = veiculo.Modelo.Descricao;
                retorno.ModeloId = string.IsNullOrEmpty(codigoExternoModelo) ? null : codigoExternoModelo;
            }


            if (veiculo.ModeloVeicularCarga != null)
            {
                retorno.TipoCarroceriaId = string.IsNullOrEmpty(codigoExternoCarroceria) ? null : codigoExternoCarroceria;
                retorno.AgrupamentoId = string.IsNullOrEmpty(codigoExternoCarroceriaAgrupamento) ? null : codigoExternoCarroceriaAgrupamento;
                retorno.DescricaoCarroceria = veiculo.ModeloVeicularCarga.Descricao;
            }

            retorno.Ano = veiculo.AnoModelo;
            retorno.Chassis = veiculo.Chassi;
            retorno.Renavan = veiculo.Renavam;
            retorno.CorPredominante = veiculo?.CorVeiculo?.Descricao;
            retorno.CapacidadeTanque = veiculo.CapacidadeTanque;
            retorno.Tara = veiculo.Tara;
            retorno.Observacao = veiculo.Observacao;

            retorno.CodOrganizacional = "1";

            retorno.Modalidade = veiculo.Tipo == "P" ? "FROTA" : "TERCEIRO";

            retorno.Proprietario = this.ObterProprietario(veiculo.Proprietario, veiculo, veiculo.EmpresaFilial, codigoExternoProprietario);

            retorno.ProprietarioDocumento = this.ObterProprietarioDocumento(veiculo.Proprietario, veiculo, veiculo.EmpresaFilial, codigoExternoProprietario);

            if (veiculo.Proprietario != null)
            { 
                if (veiculo.Proprietario.Localidade?.Estado != null)
                    retorno.Uf = veiculo.Proprietario.Localidade.Estado.Sigla;

                if (veiculo.Proprietario.Localidade?.CodigoIBGE != 0)
                {
                    retorno.MunicipioId = string.IsNullOrEmpty(codigoExternoLocalidade) ? null : codigoExternoLocalidade;
                    retorno.Municipio = veiculo.Proprietario.Localidade.Descricao.ToString();
                }              
            }
            else
            {
                retorno.Uf = veiculo.Estado?.Sigla ?? null;
            }

            return retorno;
        }
        private Dominio.ObjetosDeValor.Embarcador.Integracao.KMM.Proprietario ObterProprietario(Dominio.Entidades.Cliente pessoa, Dominio.Entidades.Veiculo veiculo,Dominio.Entidades.Empresa empresa, string codigoExternoProprietario)
        {
            Dominio.ObjetosDeValor.Embarcador.Integracao.KMM.Proprietario retorno = new Proprietario();
            Repositorio.Embarcador.Integracao.IntegracaoCodigoExterno repIntegracaoCodigoExterno = new Repositorio.Embarcador.Integracao.IntegracaoCodigoExterno(_unitOfWork);

            if (veiculo.Tipo == "T")
            {
                if (pessoa != null)
                {

                    if (pessoa.Tipo == "J")
                        retorno.CpfCnpj = pessoa.CPF_CNPJ.ToString().PadLeft(14, '0');
                    else if (pessoa.Tipo == "F")
                        retorno.CpfCnpj = pessoa.CPF_CNPJ.ToString().PadLeft(11, '0');

                    retorno.DataInicio = veiculo.DataVigencia?.Date
                                            .Add(DateTime.Now.TimeOfDay)
                                            .ToString("yyyy-MM-dd HH:mm:ss");

                    retorno.CodPessoa = string.IsNullOrEmpty(codigoExternoProprietario) ? null : codigoExternoProprietario;

                    if (pessoa?.Tipo == "E")
                    {
                        retorno.Nome = pessoa.Nome;
                        retorno.Numero = pessoa.RG_Passaporte;
                    }
                }
            }
            else
            {
                if(empresa != null)
                {
                    retorno.CpfCnpj = empresa.CNPJ.ToString().PadLeft(14, '0');

                    retorno.DataInicio = veiculo.DataCompra?.Date
                                            .Add(DateTime.Now.TimeOfDay)
                                            .ToString("yyyy-MM-dd HH:mm:ss");

                    retorno.CodPessoa = codigoExternoProprietario;

                }
            }

            return retorno;
        }
        private Dominio.ObjetosDeValor.Embarcador.Integracao.KMM.ProprietarioDocumento ObterProprietarioDocumento(Dominio.Entidades.Cliente pessoa, Dominio.Entidades.Veiculo veiculo, Dominio.Entidades.Empresa empresa, string codigoExternoProprietario)
        {
            Dominio.ObjetosDeValor.Embarcador.Integracao.KMM.ProprietarioDocumento retorno = new ProprietarioDocumento();
            Repositorio.Embarcador.Integracao.IntegracaoCodigoExterno repIntegracaoCodigoExterno = new Repositorio.Embarcador.Integracao.IntegracaoCodigoExterno(_unitOfWork);

            if (veiculo.Tipo.Equals("T"))
            {
                if (pessoa != null)
                {

                    if (pessoa.Tipo == "J")
                        retorno.CpfCnpj = pessoa.CPF_CNPJ.ToString().PadLeft(14, '0');
                    else if (pessoa.Tipo == "F")
                        retorno.CpfCnpj = pessoa.CPF_CNPJ.ToString().PadLeft(11, '0');

                    retorno.DataInicio = veiculo.DataVigencia?.Date
                                            .Add(DateTime.Now.TimeOfDay)
                                            .ToString("yyyy-MM-dd HH:mm:ss");

                    retorno.CodPessoa = string.IsNullOrEmpty(codigoExternoProprietario) ? null : codigoExternoProprietario;

                    if (pessoa?.Tipo == "E")
                    {
                        retorno.Nome = pessoa.Nome;
                        retorno.Numero = pessoa.RG_Passaporte;

                    }
                }
            }
            else
            {
                if (empresa != null)
                {
                    retorno.CpfCnpj = empresa.CNPJ.ToString().PadLeft(14, '0');

                    retorno.DataInicio = veiculo.DataCompra?.Date
                                            .Add(DateTime.Now.TimeOfDay)
                                            .ToString("yyyy-MM-dd HH:mm:ss");

                    retorno.CodPessoa = codigoExternoProprietario;

                }
            }
            return retorno;
        }
        private string ObterCodigoVeiculo(Dominio.Entidades.Embarcador.Configuracoes.IntegracaoKMM configuracaoIntegracaoKMM, Dominio.Entidades.Veiculo veiculo)
        {
            Hashtable objetoMarca = new Hashtable();
            objetoMarca.Add("veiculo_id", null);
            objetoMarca.Add("placa", veiculo.Placa ?? "");
            objetoMarca.Add("frota", veiculo.NumeroFrota ?? "");

            Hashtable request = new Hashtable
                {
                    { "module", "M1076" },
                    { "operation", "getVeiculos" },
                    { "parameters", objetoMarca }
                };

            var retWS = this.Transmitir(configuracaoIntegracaoKMM, request, "GET");

            return this.PrencherCodigoGetCodVeiculo(retWS, veiculo);

        }
        private string ObterCodigoMarca(Dominio.Entidades.Embarcador.Configuracoes.IntegracaoKMM configuracaoIntegracaoKMM, Dominio.Entidades.Veiculo veiculo)
        {

            Hashtable objetoMarca = new Hashtable();
            objetoMarca.Add("marca", veiculo.Marca.Descricao ?? "");


            Hashtable request = new Hashtable
                {
                    { "module", "M1076" },
                    { "operation", "getMarcas" },
                    { "parameters", objetoMarca }
                };

            var retWS = this.Transmitir(configuracaoIntegracaoKMM, request, "GET");

            return this.PrencherCodigoGetCodMarca(retWS, veiculo.Marca);

        }
        private string ObterCodigoModelo(Dominio.Entidades.Embarcador.Configuracoes.IntegracaoKMM configuracaoIntegracaoKMM, Dominio.Entidades.Veiculo veiculo)
        {
            Hashtable objetoModelo = new Hashtable();
            objetoModelo.Add("modelo", veiculo.Modelo.Descricao ?? "");

            Hashtable request = new Hashtable
                {
                    { "module", "M1076" },
                    { "operation", "getModelos" },
                    { "parameters", objetoModelo }
                };

            var retWS = this.Transmitir(configuracaoIntegracaoKMM, request, "GET");

            return this.PrencherCodigoGetCodModelo(retWS, veiculo.Modelo);

        }

        private string ObterCodigoLocalidade(Dominio.Entidades.Embarcador.Configuracoes.IntegracaoKMM configuracaoIntegracaoKMM, Dominio.Entidades.Veiculo veiculo)
        {
            Hashtable objetoModelo = new Hashtable();
            if (veiculo.Tipo.Equals("P"))
            {
                objetoModelo.Add("cod_ibge", veiculo.EmpresaFilial?.Localidade?.CodigoIBGE.ToString() ?? "");
                objetoModelo.Add("uf_id", veiculo.EmpresaFilial?.Localidade?.Estado?.Sigla ?? "");
            }
            else
            {
                objetoModelo.Add("cod_ibge", veiculo.Proprietario?.Localidade?.CodigoIBGE.ToString() ?? "");
                objetoModelo.Add("uf_id", veiculo.Proprietario?.Localidade?.Estado?.Sigla ?? "");
            }

            objetoModelo.Add("municipio", "");

            Hashtable request = new Hashtable
                {
                    { "module", "M1076" },
                    { "operation", "getMunicipios" },
                    { "parameters", objetoModelo }
                };

            var retWS = this.Transmitir(configuracaoIntegracaoKMM, request, "GET");

            return this.PrencherCodigoGetCodLocalidade(retWS, veiculo.Tipo.Equals("P") ? veiculo.EmpresaFilial?.Localidade : veiculo.Proprietario?.Localidade);

        }

        private string ObterCodigoCarroceira(Dominio.Entidades.Embarcador.Configuracoes.IntegracaoKMM configuracaoIntegracaoKMM, Dominio.Entidades.Veiculo veiculo)
        {
            int nCodigoExternoTipoCarroceria;
            int.TryParse(veiculo.ModeloVeicularCarga.CodigoIntegracao, out nCodigoExternoTipoCarroceria);

            Hashtable objetoModelo = new Hashtable();
            objetoModelo.Add("num_tipo_carroceria", nCodigoExternoTipoCarroceria);

            Hashtable request = new Hashtable
                {
                    { "module", "M1076" },
                    { "operation", "getCarrocerias" },
                    { "parameters", objetoModelo }
                };

            var retWS = this.Transmitir(configuracaoIntegracaoKMM, request, "GET");

            return this.PrencherCodigoGetCodCarroceria(retWS, veiculo.ModeloVeicularCarga);

        }
        private string PrencherCodigoGetCodVeiculo(Dominio.ObjetosDeValor.Embarcador.Integracao.KMM.retornoWebService retWS, Dominio.Entidades.Veiculo veiculo)
        {
            Repositorio.Embarcador.Integracao.IntegracaoCodigoExterno repIntegracaoCodigoExterno = new Repositorio.Embarcador.Integracao.IntegracaoCodigoExterno(_unitOfWork);

            if (retWS != null && retWS.SituacaoIntegracao == SituacaoIntegracao.Integrado && !string.IsNullOrEmpty(retWS.jsonRetorno))
            {
                Dominio.ObjetosDeValor.Embarcador.Integracao.KMM.RetornoVeiculo retorno = JsonConvert.DeserializeObject<Dominio.ObjetosDeValor.Embarcador.Integracao.KMM.RetornoVeiculo>(retWS.jsonRetorno);
                if (retorno != null)
                {
                    if ((retorno.Result?.Veiculo?.FirstOrDefault()?.Veiculo_Id ?? 0) > 0)
                    {
                        Dominio.Entidades.Embarcador.Integracao.IntegracaoCodigoExterno integracaoCodigoExterno = null;
                        integracaoCodigoExterno = repIntegracaoCodigoExterno.BuscarPorVeiculoETipo(veiculo.Codigo, TipoCodigoExternoIntegracao.Veiculo, TipoIntegracao.KMM);
                        if (integracaoCodigoExterno == null)
                        {
                            integracaoCodigoExterno = new Dominio.Entidades.Embarcador.Integracao.IntegracaoCodigoExterno();
                            integracaoCodigoExterno.TipoCodigoExternoIntegracao = TipoCodigoExternoIntegracao.Veiculo;
                            integracaoCodigoExterno.Veiculo = veiculo;
                            integracaoCodigoExterno.CodigoExterno = retorno.Result?.Veiculo?.FirstOrDefault().Veiculo_Id.ToString();
                            integracaoCodigoExterno.TipoIntegracao = TipoIntegracao.KMM;
                            repIntegracaoCodigoExterno.Inserir(integracaoCodigoExterno);

                            return integracaoCodigoExterno.CodigoExterno;
                        }
                    }
                }
            }

            return "";
        }
        private string PrencherCodigoGetCodMarca(Dominio.ObjetosDeValor.Embarcador.Integracao.KMM.retornoWebService retWS, Dominio.Entidades.MarcaVeiculo marca)
        {
            Repositorio.Embarcador.Integracao.IntegracaoCodigoExterno repIntegracaoCodigoExterno = new Repositorio.Embarcador.Integracao.IntegracaoCodigoExterno(_unitOfWork);

            if (retWS != null && retWS.SituacaoIntegracao == SituacaoIntegracao.Integrado && !string.IsNullOrEmpty(retWS.jsonRetorno))
            {
                Dominio.ObjetosDeValor.Embarcador.Integracao.KMM.RetornoVeiculo retorno = JsonConvert.DeserializeObject<Dominio.ObjetosDeValor.Embarcador.Integracao.KMM.RetornoVeiculo>(retWS.jsonRetorno);
                if (retorno != null)
                {
                    if ((retorno.Result?.Marca?.FirstOrDefault()?.Marca_Id ?? 0) > 0)
                    {
                        Dominio.Entidades.Embarcador.Integracao.IntegracaoCodigoExterno integracaoCodigoExterno = null;
                        integracaoCodigoExterno = repIntegracaoCodigoExterno.BuscarPorMarcaETipo(marca.Codigo, TipoCodigoExternoIntegracao.VeiculoMarca, TipoIntegracao.KMM);
                        if (integracaoCodigoExterno == null)
                        {
                            integracaoCodigoExterno = new Dominio.Entidades.Embarcador.Integracao.IntegracaoCodigoExterno();
                            integracaoCodigoExterno.TipoCodigoExternoIntegracao = TipoCodigoExternoIntegracao.VeiculoMarca;
                            integracaoCodigoExterno.Marca = marca;
                            integracaoCodigoExterno.CodigoExterno = retorno.Result?.Marca?.FirstOrDefault().Marca_Id.ToString();
                            integracaoCodigoExterno.TipoIntegracao = TipoIntegracao.KMM;
                            repIntegracaoCodigoExterno.Inserir(integracaoCodigoExterno);

                            return integracaoCodigoExterno.CodigoExterno;
                        }
                    }
                }
            }

            return "";
        }
        private string PrencherCodigoGetCodModelo(Dominio.ObjetosDeValor.Embarcador.Integracao.KMM.retornoWebService retWS, Dominio.Entidades.ModeloVeiculo modelo)
        {
            Repositorio.Embarcador.Integracao.IntegracaoCodigoExterno repIntegracaoCodigoExterno = new Repositorio.Embarcador.Integracao.IntegracaoCodigoExterno(_unitOfWork);

            if (retWS != null && retWS.SituacaoIntegracao == SituacaoIntegracao.Integrado && !string.IsNullOrEmpty(retWS.jsonRetorno))
            {
                Dominio.ObjetosDeValor.Embarcador.Integracao.KMM.RetornoVeiculo retorno = JsonConvert.DeserializeObject<Dominio.ObjetosDeValor.Embarcador.Integracao.KMM.RetornoVeiculo>(retWS.jsonRetorno);
                if (retorno != null)
                {
                    if ((retorno.Result?.Modelo?.FirstOrDefault()?.Modelo_Id ?? 0) > 0)
                    {
                        Dominio.Entidades.Embarcador.Integracao.IntegracaoCodigoExterno integracaoCodigoExterno = null;
                        integracaoCodigoExterno = repIntegracaoCodigoExterno.BuscarPorModeloETipo(modelo.Codigo, TipoCodigoExternoIntegracao.VeiculoModelo, TipoIntegracao.KMM);
                        if (integracaoCodigoExterno == null)
                        {
                            integracaoCodigoExterno = new Dominio.Entidades.Embarcador.Integracao.IntegracaoCodigoExterno();
                            integracaoCodigoExterno.TipoCodigoExternoIntegracao = TipoCodigoExternoIntegracao.VeiculoModelo;
                            integracaoCodigoExterno.Modelo = modelo;
                            integracaoCodigoExterno.CodigoExterno = retorno.Result?.Modelo?.FirstOrDefault().Modelo_Id.ToString();
                            integracaoCodigoExterno.TipoIntegracao = TipoIntegracao.KMM;
                            repIntegracaoCodigoExterno.Inserir(integracaoCodigoExterno);

                            return integracaoCodigoExterno.CodigoExterno;
                        }
                    }
                }
            }

            return "";
        }

        private string PrencherCodigoGetCodLocalidade(Dominio.ObjetosDeValor.Embarcador.Integracao.KMM.retornoWebService retWS, Dominio.Entidades.Localidade localidade)
        {
            Repositorio.Embarcador.Integracao.IntegracaoCodigoExterno repIntegracaoCodigoExterno = new Repositorio.Embarcador.Integracao.IntegracaoCodigoExterno(_unitOfWork);

            if (retWS != null && retWS.SituacaoIntegracao == SituacaoIntegracao.Integrado && !string.IsNullOrEmpty(retWS.jsonRetorno))
            {
                Dominio.ObjetosDeValor.Embarcador.Integracao.KMM.RetornoVeiculo retorno = JsonConvert.DeserializeObject<Dominio.ObjetosDeValor.Embarcador.Integracao.KMM.RetornoVeiculo>(retWS.jsonRetorno);
                if (retorno != null)
                {
                    if ((retorno.Result?.Municipios?.FirstOrDefault()?.Municipio_Id ?? 0) > 0)
                    {
                        Dominio.Entidades.Embarcador.Integracao.IntegracaoCodigoExterno integracaoCodigoExterno = null;
                        integracaoCodigoExterno = repIntegracaoCodigoExterno.BuscarPorLocalidadeETipo(localidade.Codigo, TipoCodigoExternoIntegracao.Localidade, TipoIntegracao.KMM);
                        if (integracaoCodigoExterno == null)
                        {
                            integracaoCodigoExterno = new Dominio.Entidades.Embarcador.Integracao.IntegracaoCodigoExterno();
                            integracaoCodigoExterno.TipoCodigoExternoIntegracao = TipoCodigoExternoIntegracao.Localidade;
                            integracaoCodigoExterno.Localidade = localidade;
                            integracaoCodigoExterno.CodigoExterno = retorno.Result?.Municipios?.FirstOrDefault().Municipio_Id.ToString();
                            integracaoCodigoExterno.TipoIntegracao = TipoIntegracao.KMM;
                            repIntegracaoCodigoExterno.Inserir(integracaoCodigoExterno);

                            return integracaoCodigoExterno.CodigoExterno;
                        }
                    }
                }
            }

            return "";
        }
        private string PrencherCodigoGetCodCarroceria(Dominio.ObjetosDeValor.Embarcador.Integracao.KMM.retornoWebService retWS, Dominio.Entidades.Embarcador.Cargas.ModeloVeicularCarga modeloVeicular )
        {
            Repositorio.Embarcador.Integracao.IntegracaoCodigoExterno repIntegracaoCodigoExterno = new Repositorio.Embarcador.Integracao.IntegracaoCodigoExterno(_unitOfWork);

            if (retWS != null && retWS.SituacaoIntegracao == SituacaoIntegracao.Integrado && !string.IsNullOrEmpty(retWS.jsonRetorno))
            {
                Dominio.ObjetosDeValor.Embarcador.Integracao.KMM.RetornoVeiculo retorno = JsonConvert.DeserializeObject<Dominio.ObjetosDeValor.Embarcador.Integracao.KMM.RetornoVeiculo>(retWS.jsonRetorno);
                if (retorno != null)
                {
                    if ((retorno.Result?.Carroceria?.FirstOrDefault()?.Agrupamento_Id ?? 0) > 0)
                    {
                        Dominio.Entidades.Embarcador.Integracao.IntegracaoCodigoExterno integracaoCodigoExterno = null;
                        integracaoCodigoExterno = repIntegracaoCodigoExterno.BuscarPorModeloVeicularETipo(modeloVeicular.Codigo, TipoCodigoExternoIntegracao.ModeloVeicularAgrupamento, TipoIntegracao.KMM);
                        if (integracaoCodigoExterno == null)
                        {
                            integracaoCodigoExterno = new Dominio.Entidades.Embarcador.Integracao.IntegracaoCodigoExterno();
                            integracaoCodigoExterno.TipoCodigoExternoIntegracao = TipoCodigoExternoIntegracao.ModeloVeicularAgrupamento;
                            integracaoCodigoExterno.ModeloVeicular = modeloVeicular;
                            integracaoCodigoExterno.CodigoExterno = retorno.Result?.Carroceria?.FirstOrDefault()?.Agrupamento_Id.ToString();
                            integracaoCodigoExterno.TipoIntegracao = TipoIntegracao.KMM;
                            repIntegracaoCodigoExterno.Inserir(integracaoCodigoExterno);
                        }
                    }

                    if ((retorno.Result?.Carroceria?.FirstOrDefault()?.Tipo_Carroceria_Id ?? 0) > 0)
                    {
                        Dominio.Entidades.Embarcador.Integracao.IntegracaoCodigoExterno integracaoCodigoExterno = null;
                        integracaoCodigoExterno = repIntegracaoCodigoExterno.BuscarPorModeloVeicularETipo(modeloVeicular.Codigo, TipoCodigoExternoIntegracao.ModeloVeicular, TipoIntegracao.KMM);
                        if (integracaoCodigoExterno == null)
                        {
                            integracaoCodigoExterno = new Dominio.Entidades.Embarcador.Integracao.IntegracaoCodigoExterno();
                            integracaoCodigoExterno.TipoCodigoExternoIntegracao = TipoCodigoExternoIntegracao.ModeloVeicular;
                            integracaoCodigoExterno.ModeloVeicular = modeloVeicular;
                            integracaoCodigoExterno.CodigoExterno = retorno.Result?.Carroceria?.FirstOrDefault()?.Tipo_Carroceria_Id.ToString();
                            integracaoCodigoExterno.TipoIntegracao = TipoIntegracao.KMM;
                            repIntegracaoCodigoExterno.Inserir(integracaoCodigoExterno);

                            return integracaoCodigoExterno.CodigoExterno;
                        }
                    }

                }
            }

            return "";
        }
        #endregion
    }
}