using Dominio.ObjetosDeValor.Embarcador.Enumeradores;
using SGT.BackgroundWorkers.Utils;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace SGT.BackgroundWorkers
{
    [RunningConfig(DuracaoPadrao = 1800000)]

    public class ConsultaFTPDocsys : LongRunningProcessBase<ConsultaFTPDocsys>
    {
        protected override async Task ExecuteInternalAsync(Repositorio.UnitOfWork unitOfWork, AdminMultisoftware.Repositorio.UnitOfWork unitOfWorkAdmin, CancellationToken cancellationToken)
        {
            VerificarFTPTMS(unitOfWork, _stringConexao, _tipoServicoMultisoftware);
            VerificarFTPUsuarios(unitOfWork, _stringConexao, _tipoServicoMultisoftware);
        }

        #region Métodos Privados

        private void VerificarFTPUsuarios(Repositorio.UnitOfWork unitOfWork, string stringConexao, AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware tipoServicoMultisoftware)
        {

            if (tipoServicoMultisoftware != AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiEmbarcador)
                return;

            Repositorio.Embarcador.Pessoas.ConfiguracaoFTPImportacaoVendedor repConfiguracaoFTPImportacaoVendedor = new Repositorio.Embarcador.Pessoas.ConfiguracaoFTPImportacaoVendedor(unitOfWork);
            Repositorio.Embarcador.Pessoas.ConfiguracaoFTPImportacaoSupervisor repConfiguracaoFTPImportacaoSupervisor = new Repositorio.Embarcador.Pessoas.ConfiguracaoFTPImportacaoSupervisor(unitOfWork);
            Repositorio.Embarcador.Pessoas.ConfiguracaoFTPImportacaoVendedorCliente repConfiguracaoFTPImportacaoVendedorCliente = new Repositorio.Embarcador.Pessoas.ConfiguracaoFTPImportacaoVendedorCliente(unitOfWork);
            Repositorio.Usuario repUsuario = new Repositorio.Usuario(unitOfWork);
            Repositorio.Cliente repCliente = new Repositorio.Cliente(unitOfWork);


            Dominio.Entidades.Embarcador.Pessoas.ConfiguracaoFTPImportacaoVendedorCliente configuracaoFTPImportacaoVendedorCliente = repConfiguracaoFTPImportacaoVendedorCliente.BuscarConfiguracaoPadrao();

            if (configuracaoFTPImportacaoVendedorCliente != null)
            {
                string host = configuracaoFTPImportacaoVendedorCliente.EnderecoFTP;
                string porta = configuracaoFTPImportacaoVendedorCliente.Porta;
                string diretorio = configuracaoFTPImportacaoVendedorCliente.Diretorio;
                string usuario = configuracaoFTPImportacaoVendedorCliente.Usuario;
                string senha = configuracaoFTPImportacaoVendedorCliente.Senha;
                bool passivo = configuracaoFTPImportacaoVendedorCliente.Passivo;
                bool utilizaSFTP = configuracaoFTPImportacaoVendedorCliente.UtilizarSFTP;
                bool ssl = configuracaoFTPImportacaoVendedorCliente.SSL;
                List<string> prefixosVendedorCliente = new List<string>();
                prefixosVendedorCliente.Add("DECLIUNREG");

                List<string> arquivosDisponiveis = new List<string>();
                arquivosDisponiveis = Servicos.FTP.ObterListagemArquivos(host, porta, diretorio, usuario, senha, passivo, ssl, out string erro, utilizaSFTP, true, prefixosVendedorCliente);

                Repositorio.Embarcador.Pessoas.PessoaFuncionario repPessoaFuncionario = new Repositorio.Embarcador.Pessoas.PessoaFuncionario(unitOfWork);
                Repositorio.Usuario repFuncionario = new Repositorio.Usuario(unitOfWork);
                Repositorio.Embarcador.Cargas.TipoDeCarga repTipoDeCarga = new Repositorio.Embarcador.Cargas.TipoDeCarga(unitOfWork);
                Repositorio.Embarcador.Configuracao.ConfiguracaoTMS repConfiguracaoTMS = new Repositorio.Embarcador.Configuracao.ConfiguracaoTMS(unitOfWork);
                Repositorio.Embarcador.Importacoes.ImportacaoHierarquiaHistorico repositorioImportacaoHierarquiaHistorico = new Repositorio.Embarcador.Importacoes.ImportacaoHierarquiaHistorico(unitOfWork);

                Dominio.Entidades.Embarcador.Configuracao.ConfiguracaoTMS configuracao = repConfiguracaoTMS.BuscarConfiguracaoPadrao();

                foreach (string arquivoDisponivel in arquivosDisponiveis)
                {
                    Servicos.Log.TratarErro("Encontrados  " + arquivosDisponiveis.Count + " arquivos DECLIUNREG.. INICIANDO", "ImportacaoVendedorCliente");
                    System.IO.Stream arquivo = Servicos.FTP.DownloadArquivo(host, porta, diretorio, usuario, senha, passivo, false, arquivoDisponivel, out erro, utilizaSFTP);

                    MemoryStream streamArquivo = new MemoryStream();
                    arquivo.CopyTo(streamArquivo);

                    int quantidadeTotal = 0;
                    int quantidadeAdicionados = 0;
                    int quantidadeAtualizados = 0;

                    string extencaoArquivo = System.IO.Path.GetExtension(arquivoDisponivel).ToLower();
                    if (extencaoArquivo != ".csv")
                        continue;

                    System.Data.DataTable dt = Servicos.Embarcador.NotaFiscal.ArquivoImportacao.ObterDataTable(extencaoArquivo, streamArquivo, unitOfWork);

                    if (dt != null)
                    {
                        quantidadeTotal = dt.Rows.Count;

                        Dominio.Entidades.Embarcador.Importacoes.ImportacaoHierarquiaHistorico importacaoHierarquiaHistorico = new Dominio.Entidades.Embarcador.Importacoes.ImportacaoHierarquiaHistorico()
                        {
                            Data = DateTime.Now,
                            TipoArquivo = "DECLIUNREG",
                            NomeArquivo = Path.GetFileName(arquivoDisponivel),
                            QuantidadeRegistrosTotal = quantidadeTotal,
                            Situacao = SituacaoImportacaoHierarquia.Sucesso
                        };

                        for (var i = 0; i < dt.Rows.Count; i++)
                        {
                            double cnpj = 0;
                            string tipoCarga = "";
                            string codigoFuncionario = "";
                            string codigoSap = "";

                            Dominio.Entidades.Cliente cliente = new Dominio.Entidades.Cliente();
                            Dominio.Entidades.Usuario funcionario = new Dominio.Entidades.Usuario();

                            for (var j = 0; j < configuracaoFTPImportacaoVendedorCliente.ArquivoImportacaoVendedorCliente.Campos.Count; j++)
                            {
                                Dominio.Entidades.Embarcador.Configuracoes.ArquivoImportacaoVendedorClienteCampo campo = configuracaoFTPImportacaoVendedorCliente.ArquivoImportacaoVendedorCliente.Campos[j];

                                object valor = Servicos.Embarcador.Pessoa.ArquivoImportacaoVendedor.LerCampo(campo, dt.Rows[i][(campo.Posicao - 1)].ToString());

                                if (campo.Propriedade == "CNPJ")
                                    cnpj = (double)valor;

                                if (campo.Propriedade == "TipoCarga")
                                    tipoCarga = (string)valor;

                                if (campo.Propriedade == "CodigoFuncionario")
                                    codigoFuncionario = (string)valor;

                                if (campo.Propriedade == "CodigoSap")
                                    codigoSap = (string)valor;
                            }

                            if (cnpj > 0)
                            {
                                try
                                {
                                    unitOfWork.Start();

                                    cliente = repCliente.BuscarPorCPFCNPJ(cnpj);

                                    if (cliente != null)
                                    {
                                        cliente.CodigoSap = codigoSap;

                                        Dominio.Entidades.Embarcador.Pessoas.PessoaFuncionario vendedor = repPessoaFuncionario.BuscarPorPessoaECodigoIntegracao(codigoFuncionario, cliente.CPF_CNPJ);

                                        if (vendedor == null)
                                            vendedor = new Dominio.Entidades.Embarcador.Pessoas.PessoaFuncionario();
                                        else
                                            vendedor.Initialize();

                                        if (!string.IsNullOrWhiteSpace(codigoFuncionario))
                                        {
                                            vendedor.Funcionario = repFuncionario.BuscarPorCodigoIntegracao(codigoFuncionario);

                                            if (vendedor.Funcionario == null)
                                            {
                                                Servicos.Log.TratarErro("Funcionário não encontrado na base de dados! Linha " + i.ToString() + " da planilha " + arquivoDisponivel, "Importacao Vendedor Cliente");
                                                continue;
                                            }
                                        }
                                        if (!string.IsNullOrWhiteSpace(tipoCarga))
                                            vendedor.TipoDeCarga = repTipoDeCarga.BuscarPorCodigoErp(tipoCarga);


                                        vendedor.Pessoa = cliente;

                                        if (vendedor.Codigo > 0)
                                            Servicos.Auditoria.Auditoria.Auditar(_auditado, vendedor, vendedor.GetChanges(), $"Realizado vínculo com {cliente.Nome}", unitOfWork);
                                        else
                                            Servicos.Auditoria.Auditoria.Auditar(_auditado, vendedor, $"Vínculo atualizado com {cliente.Nome}", unitOfWork);
                                        cliente.Integrado = false;
                                        cliente.DataUltimaAtualizacao = DateTime.Now;
                                        repCliente.Atualizar(cliente);

                                        if (vendedor.Codigo > 0)
                                        {
                                            repPessoaFuncionario.Atualizar(vendedor);
                                            quantidadeAtualizados += 1;
                                        }
                                        else
                                        {
                                            repPessoaFuncionario.Inserir(vendedor);
                                            quantidadeAdicionados += 1;
                                        }
                                    }

                                    unitOfWork.CommitChanges();
                                }
                                catch (Exception excecao)
                                {
                                    unitOfWork.Rollback();
                                }
                            }
                        }

                        importacaoHierarquiaHistorico.QuantidadeRegistrosImportados = quantidadeAtualizados + quantidadeAdicionados;
                        importacaoHierarquiaHistorico.Descricao = $"{quantidadeAtualizados} registros atualizados e {quantidadeAdicionados} registros adicionados.";

                        repositorioImportacaoHierarquiaHistorico.Inserir(importacaoHierarquiaHistorico);

                        Servicos.Log.TratarErro("finalizou leitura.. " + dt.Rows.Count + " linhas.", "ImportacaoVendedorCliente");
                    }

                    Servicos.FTP.DeletarArquivo(host, porta, diretorio, usuario, senha, passivo, false, arquivoDisponivel, out erro, utilizaSFTP);
                    Servicos.Log.TratarErro("Deletou arquivo.", "ImportacaoVendedorCliente");
                }


            }

            Dominio.Entidades.Embarcador.Pessoas.ConfiguracaoFTPImportacaoVendedor configuracaoFTPImportacaoVendedor = repConfiguracaoFTPImportacaoVendedor.BuscarConfiguracaoPadrao();

            if (configuracaoFTPImportacaoVendedor != null)
            {
                string host = configuracaoFTPImportacaoVendedor.EnderecoFTP;
                string porta = configuracaoFTPImportacaoVendedor.Porta;
                string diretorio = configuracaoFTPImportacaoVendedor.Diretorio;
                string usuario = configuracaoFTPImportacaoVendedor.Usuario;
                string senha = configuracaoFTPImportacaoVendedor.Senha;
                bool passivo = configuracaoFTPImportacaoVendedor.Passivo;
                bool utilizaSFTP = configuracaoFTPImportacaoVendedor.UtilizarSFTP;
                bool ssl = configuracaoFTPImportacaoVendedor.SSL;
                List<string> prefixosVendedor = new List<string>();
                prefixosVendedor.Add("RE_");

                List<string> arquivosDisponiveis = new List<string>();
                arquivosDisponiveis = Servicos.FTP.ObterListagemArquivos(host, porta, diretorio, usuario, senha, passivo, ssl, out string erro, utilizaSFTP, true, prefixosVendedor);

                Repositorio.Embarcador.Configuracao.ConfiguracaoTMS repConfiguracaoTMS = new Repositorio.Embarcador.Configuracao.ConfiguracaoTMS(unitOfWork);
                Dominio.Entidades.Embarcador.Configuracao.ConfiguracaoTMS configuracao = repConfiguracaoTMS.BuscarConfiguracaoPadrao();
                Repositorio.Embarcador.Importacoes.ImportacaoHierarquiaHistorico repositorioImportacaoHierarquiaHistorico = new Repositorio.Embarcador.Importacoes.ImportacaoHierarquiaHistorico(unitOfWork);

                foreach (string arquivoDisponivel in arquivosDisponiveis)
                {
                    Servicos.Log.TratarErro("Encontrados  " + arquivosDisponiveis.Count + " arquivos RE_ ... INICIANDO", "ImportacaoVendedores");

                    System.IO.Stream arquivo = Servicos.FTP.DownloadArquivo(host, porta, diretorio, usuario, senha, passivo, false, arquivoDisponivel, out erro, utilizaSFTP);

                    MemoryStream streamArquivo = new MemoryStream();
                    arquivo.CopyTo(streamArquivo);

                    int quantidadeTotal = 0;
                    int quantidadeAdicionados = 0;
                    int quantidadeAtualizados = 0;

                    string extencaoArquivo = System.IO.Path.GetExtension(arquivoDisponivel).ToLower();
                    if (extencaoArquivo != ".csv")
                        continue;

                    System.Data.DataTable dt = Servicos.Embarcador.NotaFiscal.ArquivoImportacao.ObterDataTable(extencaoArquivo, streamArquivo, unitOfWork);

                    if (dt != null)
                    {
                        quantidadeTotal = dt.Rows.Count;

                        Dominio.Entidades.Embarcador.Importacoes.ImportacaoHierarquiaHistorico importacaoHierarquiaHistorico = new Dominio.Entidades.Embarcador.Importacoes.ImportacaoHierarquiaHistorico()
                        {
                            Data = DateTime.Now,
                            TipoArquivo = "RE_",
                            NomeArquivo = Path.GetFileName(arquivoDisponivel),
                            QuantidadeRegistrosTotal = quantidadeTotal,
                            Situacao = SituacaoImportacaoHierarquia.Sucesso
                        };

                        for (var i = 0; i < dt.Rows.Count; i++)
                        {
                            try
                            {
                                string codigoIntegracao = "";
                                string cpf = "";
                                string nome = "";
                                string rg = "";
                                string endereco = "";
                                string numeroEndereco = "";
                                string bairro = "";
                                string complemento = "";
                                string telefone = "";
                                string email = "";
                                string codigoSap = "";


                                Dominio.Entidades.Usuario vendedor = new Dominio.Entidades.Usuario();

                                for (var j = 0; j < configuracaoFTPImportacaoVendedor.ArquivoImportacaoVendedor.Campos.Count; j++)
                                {
                                    Dominio.Entidades.Embarcador.Configuracoes.ArquivoImportacaoVendedorCampo campo = configuracaoFTPImportacaoVendedor.ArquivoImportacaoVendedor.Campos[j];

                                    object valor = Servicos.Embarcador.Pessoa.ArquivoImportacaoVendedor.LerCampo(campo, dt.Rows[i][(campo.Posicao)].ToString());

                                    if (campo.Propriedade == "CodigoSap")
                                        codigoSap = (string)valor;

                                    if (campo.Propriedade == "CodigoIntegracao")
                                        codigoIntegracao = (string)valor;

                                    if (campo.Propriedade == "CPF")
                                        cpf = (string)valor;

                                    if (campo.Propriedade == "Nome")
                                        nome = (string)valor;

                                    if (campo.Propriedade == "RG")
                                        rg = (string)valor;

                                    if (campo.Propriedade == "Endereco")
                                        endereco = (string)valor;

                                    if (campo.Propriedade == "NumeroEndereco")
                                        numeroEndereco = (string)valor;

                                    if (campo.Propriedade == "Bairro")
                                        bairro = (string)valor;

                                    if (campo.Propriedade == "Complemento")
                                        complemento = (string)valor;

                                    if (campo.Propriedade == "Telefone")
                                        telefone = (string)valor;

                                    if (campo.Propriedade == "Email")
                                        email = (string)valor;
                                }

                                if (string.IsNullOrEmpty(cpf) || cpf.ObterSomenteNumeros() == cpf.ObterSomenteCaracteresPermitidos("0") || cpf.ObterSomenteNumeros() == "11111111111")
                                    continue;

                                vendedor = repUsuario.BuscarPorCPF(cpf);

                                if (vendedor == null)
                                {
                                    vendedor = new Dominio.Entidades.Usuario();
                                    vendedor.Status = "A";
                                }
                                else
                                {
                                    if (!string.IsNullOrWhiteSpace(vendedor.Login))
                                        continue;
                                }

                                bool atualizar = false;
                                if (vendedor.TipoAcesso != Dominio.Enumeradores.TipoAcesso.Embarcador)
                                {
                                    vendedor.TipoAcesso = Dominio.Enumeradores.TipoAcesso.Embarcador;
                                    atualizar = true;
                                }

                                if (vendedor.CPF != cpf)
                                {
                                    vendedor.CPF = cpf;
                                    atualizar = true;
                                }

                                if (vendedor.Nome != nome)
                                {
                                    vendedor.Nome = nome;
                                    atualizar = true;
                                }

                                if (vendedor.RG != rg)
                                {
                                    vendedor.RG = rg;
                                    atualizar = true;
                                }

                                if (vendedor.Endereco != endereco)
                                {
                                    vendedor.Endereco = endereco;
                                    atualizar = true;
                                }

                                if (vendedor.NumeroEndereco != numeroEndereco)
                                {
                                    vendedor.NumeroEndereco = numeroEndereco;
                                    atualizar = true;
                                }

                                if (vendedor.Bairro != bairro)
                                {
                                    vendedor.Bairro = bairro;
                                    atualizar = true;
                                }

                                if (vendedor.Complemento != complemento)
                                {
                                    vendedor.Complemento = complemento;
                                    atualizar = true;
                                }

                                if (vendedor.Telefone != telefone)
                                {
                                    vendedor.Telefone = telefone;
                                    atualizar = true;
                                }

                                if (vendedor.Email != email)
                                {
                                    vendedor.Email = email;
                                    atualizar = true;
                                }

                                if (vendedor.CodigoIntegracao != codigoIntegracao)
                                {
                                    vendedor.CodigoIntegracao = codigoIntegracao;
                                    atualizar = true;
                                }

                                if (vendedor.Codigo > 0)
                                {
                                    if (atualizar)
                                    {
                                        vendedor.Initialize();
                                        repUsuario.Atualizar(vendedor);
                                        Servicos.Auditoria.Auditoria.Auditar(_auditado, vendedor, vendedor.GetChanges(), "Atualizado via FTP", unitOfWork);
                                        quantidadeAtualizados += 1;
                                    }
                                }
                                else
                                {
                                    repUsuario.Inserir(vendedor);
                                    Servicos.Auditoria.Auditoria.Auditar(_auditado, vendedor, "Adicionado via FTP", unitOfWork);
                                    quantidadeAdicionados += 1;
                                }
                            }
                            catch (Exception excecao)
                            {
                                Servicos.Log.TratarErro(excecao);
                                continue;
                            }

                        }

                        Servicos.Log.TratarErro("finalizou leitura.. " + dt.Rows.Count + " linhas.", "ImportacaoVendedores");

                        importacaoHierarquiaHistorico.QuantidadeRegistrosImportados = quantidadeAtualizados + quantidadeAdicionados;
                        importacaoHierarquiaHistorico.Descricao = $"{quantidadeAtualizados} registros atualizados e {quantidadeAdicionados} registros adicionados.";

                        repositorioImportacaoHierarquiaHistorico.Inserir(importacaoHierarquiaHistorico);
                    }
                    else
                        Servicos.Log.TratarErro("Importar vendedores FTP, não foi possivel interpretar a extensão " + extencaoArquivo, "ImportacaoVendedores");


                    Servicos.FTP.DeletarArquivo(host, porta, diretorio, usuario, senha, passivo, false, arquivoDisponivel, out erro, utilizaSFTP);
                    Servicos.Log.TratarErro("Deletou arquivo.", "ImportacaoVendedores");
                }

            }

            Dominio.Entidades.Embarcador.Pessoas.ConfiguracaoFTPImportacaoSupervisor configuracaoFTPImportacaoSupervisor = repConfiguracaoFTPImportacaoSupervisor.BuscarConfiguracaoPadrao();

            if (configuracaoFTPImportacaoSupervisor != null)
            {
                string host = configuracaoFTPImportacaoSupervisor.EnderecoFTP;
                string porta = configuracaoFTPImportacaoSupervisor.Porta;
                string diretorio = configuracaoFTPImportacaoSupervisor.Diretorio;
                string usuario = configuracaoFTPImportacaoSupervisor.Usuario;
                string senha = configuracaoFTPImportacaoSupervisor.Senha;
                bool passivo = configuracaoFTPImportacaoSupervisor.Passivo;
                bool utilizaSFTP = configuracaoFTPImportacaoSupervisor.UtilizarSFTP;
                bool ssl = configuracaoFTPImportacaoSupervisor.SSL;
                List<string> prefixosSupervisor = new List<string>();
                prefixosSupervisor.Add("DEREGISTRO");

                List<string> arquivosDisponiveis = new List<string>();
                arquivosDisponiveis = Servicos.FTP.ObterListagemArquivos(host, porta, diretorio, usuario, senha, passivo, ssl, out string erro, utilizaSFTP, true, prefixosSupervisor);

                Repositorio.Embarcador.Configuracao.ConfiguracaoTMS repConfiguracaoTMS = new Repositorio.Embarcador.Configuracao.ConfiguracaoTMS(unitOfWork);
                Dominio.Entidades.Embarcador.Configuracao.ConfiguracaoTMS configuracao = repConfiguracaoTMS.BuscarConfiguracaoPadrao();
                Repositorio.Embarcador.Importacoes.ImportacaoHierarquiaHistorico repositorioImportacaoHierarquiaHistorico = new Repositorio.Embarcador.Importacoes.ImportacaoHierarquiaHistorico(unitOfWork);

                foreach (string arquivoDisponivel in arquivosDisponiveis)
                {
                    Servicos.Log.TratarErro("Encontrados  " + arquivosDisponiveis.Count + " arquivos DEREGISTRO.. INICIANDO", "ImportacaoSupervisor");
                    System.IO.Stream arquivo = Servicos.FTP.DownloadArquivo(host, porta, diretorio, usuario, senha, passivo, false, arquivoDisponivel, out erro, utilizaSFTP);

                    MemoryStream streamArquivo = new MemoryStream();
                    arquivo.CopyTo(streamArquivo);

                    int quantidadeTotal = 0;
                    int quantidadeAdicionados = 0;
                    int quantidadeAtualizados = 0;

                    string extencaoArquivo = System.IO.Path.GetExtension(arquivoDisponivel).ToLower();
                    if (extencaoArquivo != ".csv")
                        continue;

                    System.Data.DataTable dt = Servicos.Embarcador.NotaFiscal.ArquivoImportacao.ObterDataTable(extencaoArquivo, streamArquivo, unitOfWork);
                    if (dt != null)
                    {
                        quantidadeTotal = dt.Rows.Count;

                        Dominio.Entidades.Embarcador.Importacoes.ImportacaoHierarquiaHistorico importacaoHierarquiaHistorico = new Dominio.Entidades.Embarcador.Importacoes.ImportacaoHierarquiaHistorico()
                        {
                            Data = DateTime.Now,
                            TipoArquivo = "DEREGISTRO",
                            NomeArquivo = Path.GetFileName(arquivoDisponivel),
                            QuantidadeRegistrosTotal = quantidadeTotal,
                            Situacao = SituacaoImportacaoHierarquia.Sucesso
                        };

                        for (var i = 0; i < dt.Rows.Count; i++)
                        {
                            try
                            {
                                unitOfWork.Start();

                                string codigoFuncionario = "";
                                TipoComercial? codigoFuncao = null;
                                string codigoSuperior = "";
                                int aux = 0;
                                for (var j = 0; j < configuracaoFTPImportacaoSupervisor.ArquivoImportacaoSupervisor.Campos.Count; j++)
                                {
                                    Dominio.Entidades.Embarcador.Configuracoes.ArquivoImportacaoSupervisorCampo campo = configuracaoFTPImportacaoSupervisor.ArquivoImportacaoSupervisor.Campos[j];

                                    object valor = Servicos.Embarcador.Pessoa.ArquivoImportacaoVendedor.LerCampo(campo, dt.Rows[i][(campo.Posicao)].ToString());

                                    if (campo.Propriedade == "CodigoFuncionario")
                                        codigoFuncionario = (string)valor;

                                    if (campo.Propriedade == "CodigoFuncao")
                                    {
                                        aux = (int)valor;
                                        codigoFuncao = aux.ToString().ToEnum<TipoComercial>();
                                    }

                                    if (campo.Propriedade == "CodigoSuperior")
                                        codigoSuperior = (string)valor;


                                }

                                if (!string.IsNullOrWhiteSpace(codigoFuncionario))
                                {
                                    Dominio.Entidades.Usuario funcionario = repUsuario.BuscarPorCodigoIntegracao(codigoFuncionario);
                                    if (funcionario != null)
                                    {
                                        funcionario.Initialize();

                                        funcionario.TipoComercial = codigoFuncao;
                                        if (!string.IsNullOrWhiteSpace(codigoSuperior))
                                        {
                                            Dominio.Entidades.Usuario superior = repUsuario.BuscarPorCodigoIntegracao(codigoSuperior);
                                            if (superior != null)
                                            {
                                                if (superior.TipoComercial == TipoComercial.SupervisorDanone)
                                                    funcionario.Supervisor = superior;
                                                else if (superior.TipoComercial == TipoComercial.GerenteArea || superior.TipoComercial == TipoComercial.GerenteNacional || superior.TipoComercial == TipoComercial.GerenteRede)
                                                    funcionario.Gerente = superior;
                                            }
                                        }

                                        repUsuario.Atualizar(funcionario);

                                        Servicos.Auditoria.Auditoria.Auditar(_auditado, funcionario, funcionario.GetChanges(), "Atualizou cargo", unitOfWork);

                                        quantidadeAtualizados += 1;
                                    }
                                }

                                unitOfWork.CommitChanges();
                            }
                            catch (Exception excecao)
                            {
                                unitOfWork.Rollback();
                            }
                        }

                        Servicos.Log.TratarErro("finalizou leitura.. " + dt.Rows.Count + " linhas.", "ImportacaoSupervisor");

                        importacaoHierarquiaHistorico.QuantidadeRegistrosImportados = quantidadeAtualizados + quantidadeAdicionados;
                        importacaoHierarquiaHistorico.Descricao = $"{quantidadeAtualizados} registros atualizados.";

                        repositorioImportacaoHierarquiaHistorico.Inserir(importacaoHierarquiaHistorico);
                    }

                    Servicos.FTP.DeletarArquivo(host, porta, diretorio, usuario, senha, passivo, false, arquivoDisponivel, out erro, utilizaSFTP);
                    Servicos.Log.TratarErro("Deletou arquivo.", "ImportacaoSupervisor");
                }
            }
        }

        private void VerificarFTPTMS(Repositorio.UnitOfWork unitOfWork, string stringConexao, AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware tipoServicoMultisoftware)
        {
            if (tipoServicoMultisoftware != AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiTMS)
                return;

            Repositorio.Embarcador.Documentos.IntegracaoFTPDocumentosDestinados repIntegracaoFTPDocumentosDestinados = new Repositorio.Embarcador.Documentos.IntegracaoFTPDocumentosDestinados(unitOfWork);
            List<Dominio.Entidades.Embarcador.Documentos.IntegracaoFTPDocumentosDestinados> empresasFTP = repIntegracaoFTPDocumentosDestinados.BuscarTodos();

            foreach (Dominio.Entidades.Embarcador.Documentos.IntegracaoFTPDocumentosDestinados empresaFTP in empresasFTP)
            {
                string host = empresaFTP.EnderecoFTP;
                string porta = empresaFTP.Porta;
                string diretorio = empresaFTP.DiretorioXML;
                string usuario = empresaFTP.Usuario;
                string senha = empresaFTP.Senha;
                bool passivo = empresaFTP.Passivo;
                bool utilizaSFTP = empresaFTP.UtilizarSFTP;
                bool ssl = empresaFTP.SSL;
                string erro = "";
                string diretorioInput = empresaFTP.DiretorioInput;

                if (!string.IsNullOrWhiteSpace(diretorioInput))
                {
                    List<string> arquivosDisponiveis = Servicos.FTP.ObterListagemArquivos(host, porta, diretorioInput, usuario, senha, passivo, ssl, out erro, utilizaSFTP, true);

                    foreach (string arquivoDisponivel in arquivosDisponiveis)
                    {
                        string extensaoArquivo = Path.GetExtension(arquivoDisponivel);
                        if (extensaoArquivo == "zip" || extensaoArquivo == ".zip" || extensaoArquivo == "ZIP" || extensaoArquivo == ".ZIP")
                        {
                            System.IO.Stream arquivo = Servicos.FTP.DownloadArquivo(host, porta, diretorioInput, usuario, senha, passivo, false, arquivoDisponivel, out erro, utilizaSFTP);
                            Servicos.FTP.DeletarArquivo(host, porta, diretorioInput, usuario, senha, passivo, false, arquivoDisponivel, out erro, utilizaSFTP);

                            ProcessarArquivoCSV(arquivo, arquivoDisponivel, unitOfWork, stringConexao, tipoServicoMultisoftware);
                        }
                    }
                }
            }
        }

        private void ProcessarArquivoCSV(System.IO.Stream arquivo, string arquivoDisponivel, Repositorio.UnitOfWork unitOfWork, string stringConexao, AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware tipoServicoMultisoftware)
        {
            Repositorio.Embarcador.Documentos.DadosDocsys repDadosDocsys = new Repositorio.Embarcador.Documentos.DadosDocsys(unitOfWork);
            Repositorio.Embarcador.Pedidos.Porto repPorto = new Repositorio.Embarcador.Pedidos.Porto(unitOfWork);
            Repositorio.Embarcador.Pedidos.PedidoViagemNavio repPedidoViagemNavio = new Repositorio.Embarcador.Pedidos.PedidoViagemNavio(unitOfWork);
            Repositorio.ConhecimentoDeTransporteEletronico repCTe = new Repositorio.ConhecimentoDeTransporteEletronico(unitOfWork);

            //descompactar
            string caminhoArquivo = Utilidades.Directory.CriarCaminhoArquivos(new string[] { Servicos.Embarcador.Configuracoes.ConfigurationInstance.GetInstance(unitOfWork)?.ObterConfiguracaoArquivo().CaminhoArquivos, "DOCS", "Importar" });
            string guidArquivo = Guid.NewGuid().ToString().Replace("-", "");

            // Problema ao descompactar emails .rar
            string fileLocation = caminhoArquivo;
            string arquivoProcessar = Utilidades.IO.FileStorageService.Storage.Combine(fileLocation, guidArquivo + System.IO.Path.GetExtension(arquivoDisponivel));

            using (var fileStream = Utilidades.IO.FileStorageService.Storage.Create(arquivoProcessar))
            {
                arquivo.Seek(0, SeekOrigin.Begin);
                arquivo.CopyTo(fileStream);
            }

            try
            {
                ICSharpCode.SharpZipLib.Zip.FastZip zip = new ICSharpCode.SharpZipLib.Zip.FastZip();
                zip.ExtractZip(arquivoProcessar, caminhoArquivo, ICSharpCode.SharpZipLib.Zip.FastZip.Overwrite.Always, null, "", "", false);
            }
            catch
            {
                try
                {
                    NUnrar.Archive.RarArchive.WriteToDirectory(arquivoProcessar, caminhoArquivo);
                }
                catch (Exception exRar)
                {
                    Servicos.Log.TratarErro("O documento não está compactado corretamente " + arquivoDisponivel + " guid " + guidArquivo, "XMLEmail");
                    Servicos.Log.TratarErro("erro: " + exRar, "ConsultaFTPDocsys");
                    return;
                }
            }

            IEnumerable<string> arquivos = Utilidades.IO.FileStorageService.Storage.GetFiles(caminhoArquivo, "*.csv");
            
            if (arquivos == null || !arquivos.Any())
                arquivos = Utilidades.IO.FileStorageService.Storage.GetFiles(caminhoArquivo, "*.txt");

            Servicos.Log.TratarErro("Inicio processamento", "ConsultaFTPDocsys");

            foreach (string file in arquivos)
            {
                IEnumerable<string> lines = Utilidades.IO.FileStorageService.Storage.ReadLines(file);
                int i = 0;
                foreach (string line in lines)
                {
                    var linha = line.Split(';');
                    if (i > 0 && (!string.IsNullOrWhiteSpace(linha[0])))
                    {
                        Dominio.Entidades.Embarcador.Documentos.DadosDocsys dadosDocsys = new Dominio.Entidades.Embarcador.Documentos.DadosDocsys()
                        {
                            blading = ((string)linha[12]).Trim(),
                            BLVersion = ((string)linha[119]).Trim(),
                            BOOKNO = ((string)linha[131]).Trim(),
                            CorrCore = ((string)linha[120]).Trim(),
                            DACSTransf = null,//151
                            dir = ((string)linha[3]).Trim(),
                            PedidoViagemNavio = null,
                            pod = ((string)linha[8]).Trim(),
                            podname = ((string)linha[9]).Trim(),
                            pol = ((string)linha[6]).Trim(),
                            polname = ((string)linha[7]).Trim(),
                            PortoDestino = null,
                            PortoOrigem = null,
                            UBLI = ((string)linha[129]).Trim(),
                            vescode = ((string)linha[0]).Trim(),
                            vessel = ((string)linha[1]).Trim(),
                            VoucherDate = null,//150
                            VoucherNO = ((string)linha[182]).Trim(),
                            voy = ((string)linha[2]).Trim(),
                            DataInclusao = DateTime.Now,
                            Duplicado = false,
                            NomeArquivo = guidArquivo
                        };

                        if (!string.IsNullOrWhiteSpace(dadosDocsys.UBLI) && dadosDocsys.UBLI.Length > 12)
                            dadosDocsys.UBLI = dadosDocsys.UBLI.Substring(0, 12);

                        DateTime DACSTransf;
                        DateTime.TryParseExact(((string)linha[151]).Trim(), "dd.MM.yyyy", null, System.Globalization.DateTimeStyles.None, out DACSTransf);
                        if (DACSTransf > DateTime.MinValue)
                            dadosDocsys.DACSTransf = DACSTransf;

                        DateTime VoucherDate;
                        DateTime.TryParseExact(((string)linha[150]).Trim(), "dd.MM.yyyy", null, System.Globalization.DateTimeStyles.None, out VoucherDate);
                        if (DACSTransf > DateTime.MinValue)
                            dadosDocsys.VoucherDate = VoucherDate;

                        string viagem = dadosDocsys.vessel + "/" + dadosDocsys.voy + dadosDocsys.dir;
                        dadosDocsys.PedidoViagemNavio = repPedidoViagemNavio.BuscarPorDescricao(viagem);
                        if (dadosDocsys.PedidoViagemNavio == null)
                        {
                            dadosDocsys.voy = dadosDocsys.voy.TrimStart(new Char[] { '0' });
                            viagem = dadosDocsys.vessel + "/" + dadosDocsys.voy + dadosDocsys.dir;
                            dadosDocsys.PedidoViagemNavio = repPedidoViagemNavio.BuscarPorDescricao(viagem);
                        }
                        string portoOrigem = dadosDocsys.pol.PadLeft(4, '0');
                        dadosDocsys.PortoOrigem = repPorto.BuscarPorCodigoDocumento(portoOrigem);

                        string portoDestino = dadosDocsys.pod.PadLeft(4, '0');
                        dadosDocsys.PortoDestino = repPorto.BuscarPorCodigoDocumento(portoDestino);

                        if (dadosDocsys.PedidoViagemNavio != null && !string.IsNullOrWhiteSpace(dadosDocsys.BOOKNO) && !string.IsNullOrWhiteSpace(dadosDocsys.blading))
                            dadosDocsys.CTe = repCTe.BuscarPorViagemBKControle(dadosDocsys.PedidoViagemNavio.Codigo, dadosDocsys.BOOKNO, dadosDocsys.blading);

                        if (!string.IsNullOrWhiteSpace(dadosDocsys.vessel) && !string.IsNullOrWhiteSpace(dadosDocsys.voy) && !string.IsNullOrWhiteSpace(dadosDocsys.dir) && !string.IsNullOrWhiteSpace(dadosDocsys.blading) && !string.IsNullOrWhiteSpace(dadosDocsys.BOOKNO))
                        {
                            List<Dominio.Entidades.Embarcador.Documentos.DadosDocsys> registrosDuplicados = repDadosDocsys.RegistroDuplicado(dadosDocsys.vessel, dadosDocsys.voy, dadosDocsys.dir, dadosDocsys.blading, dadosDocsys.BOOKNO, dadosDocsys.pol, dadosDocsys.pod, dadosDocsys.UBLI);
                            dadosDocsys.Duplicado = registrosDuplicados != null && registrosDuplicados.Count > 0;
                            if (registrosDuplicados != null && registrosDuplicados.Count > 0)
                            {
                                foreach (var registroDuplicado in registrosDuplicados)
                                {
                                    registroDuplicado.Duplicado = true;
                                    repDadosDocsys.Atualizar(registroDuplicado);
                                }
                            }
                        }

                        repDadosDocsys.Inserir(dadosDocsys);
                    }
                    unitOfWork.FlushAndClear();
                    i++;
                }

                Servicos.Log.TratarErro("Fim processamento", "ConsultaFTPDocsys");

                Utilidades.IO.FileStorageService.Storage.Delete(file);
            }
        }

        #endregion
    }
}