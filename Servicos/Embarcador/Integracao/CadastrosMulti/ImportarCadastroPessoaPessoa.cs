using System;
using System.Text;

namespace Servicos.Embarcador.Integracao.CadastrosMulti
{
    public class ImportarCadastroPessoaPessoa
    {

        #region Métodos Globais

        public static bool ImportarCadastro(out string mensagemErro, AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware tipoServicoMultisoftware, Dominio.ObjetosDeValor.Embarcador.Auditoria.Auditado auditado, Repositorio.UnitOfWork unitOfWork)
        {
            mensagemErro = null;

            Servicos.Cliente serCliente = new Servicos.Cliente(unitOfWork.StringConexao);

            Repositorio.Embarcador.Configuracoes.Integracao repIntegracao = new Repositorio.Embarcador.Configuracoes.Integracao(unitOfWork);
            Dominio.Entidades.Embarcador.Configuracoes.Integracao integracao = repIntegracao.BuscarPrimeiroRegistro();

            Servicos.ServicoPessoa.SGT.PessoasClient svcPessoa = ObterClientPessoaCore(!string.IsNullOrWhiteSpace(integracao.URLIntegracaoCadastrosMultiSecundario) ? integracao.URLIntegracaoCadastrosMultiSecundario : integracao.URLIntegracaoCadastrosMulti, !string.IsNullOrWhiteSpace(integracao.TokenIntegracaoCadastrosMultiSecundario) ? integracao.TokenIntegracaoCadastrosMultiSecundario : integracao.TokenIntegracaoCadastrosMulti);

            int limite = 100, inicio = 0;
            do
            {
                Servicos.ServicoPessoa.SGT.BuscarPessoasResponse retorno = null;
                try
                {
                    Servicos.ServicoPessoa.SGT.BuscarPessoasRequest request = new ServicoPessoa.SGT.BuscarPessoasRequest(inicio, 100, true);
                    retorno = svcPessoa.BuscarPessoas(request);
                }
                catch (Exception ex)
                {
                    Servicos.Log.TratarErro($"Falha ao consultar cliente : {ex.Message}", "ImportarCadastro");
                    return false;
                }

                if (retorno == null || !retorno.BuscarPessoasResult.Status)
                {
                    Servicos.Log.TratarErro($"Falha ao buscar pessoas: {retorno?.BuscarPessoasResult.Mensagem}", "ImportarCadastro");

                    mensagemErro = retorno?.BuscarPessoasResult.Mensagem ?? "";
                    return false;
                }

                foreach (var pessoa in retorno.BuscarPessoasResult.Objeto.Itens)
                {
                    try
                    {
                        pessoa.AtualizarEnderecoPessoa = true;
                        Dominio.ObjetosDeValor.RetornoVerificacaoCliente retornoCliente = serCliente.ConverterObjetoValorPessoa(pessoa, string.Empty, unitOfWork, 0, false, false, auditado, tipoServicoMultisoftware, false, true);
                        if (!retornoCliente.Status)
                            Servicos.Log.TratarErro($"Falha ao inserir pessoa: {retornoCliente.Mensagem}", "ImportarCadastro");
                        else
                        {
                            Servicos.ServicoPessoa.SGT.ConfirmarIntegracaoPessoaRequest requestConfirmar = new ServicoPessoa.SGT.ConfirmarIntegracaoPessoaRequest(pessoa.ClienteExterior ? Utilidades.String.OnlyNumbers(pessoa.CodigoIntegracao) : Utilidades.String.OnlyNumbers(pessoa.CPFCNPJ));
                            svcPessoa.ConfirmarIntegracaoPessoa(requestConfirmar);
                        }
                    }
                    catch (Exception ex)
                    {
                        Servicos.Log.TratarErro($"Falha ao inserir cliente : {ex.Message}", "ImportarCadastro");

                    }
                }

                limite = retorno.BuscarPessoasResult.Objeto.NumeroTotalDeRegistro;
                inicio += 100;

                unitOfWork.FlushAndClear();
            }
            while (inicio < limite);

            integracao.RealizouIntegracaoCompletaDePessoaParaPessoa = true;
            repIntegracao.Atualizar(integracao);

            mensagemErro = string.Empty;
            return true;
        }

        public static bool ImportarCadastroTransportador(out string mensagemErro, AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware tipoServicoMultisoftware, Dominio.ObjetosDeValor.Embarcador.Auditoria.Auditado auditado, Repositorio.UnitOfWork unitOfWork, string adminStringConexao)
        {
            mensagemErro = null;

            Servicos.WebService.Empresa.Empresa servicoEmpresa = new Servicos.WebService.Empresa.Empresa(unitOfWork);

            Repositorio.Embarcador.Configuracoes.Integracao repIntegracao = new Repositorio.Embarcador.Configuracoes.Integracao(unitOfWork);
            Dominio.Entidades.Embarcador.Configuracoes.Integracao integracao = repIntegracao.BuscarPrimeiroRegistro();

            ServicoSGT.Pessoa.PessoasClient svcPessoa = ObterClientPessoa(integracao.URLIntegracaoCadastrosMulti, integracao.TokenIntegracaoCadastrosMulti);

            int limite = 100, inicio = 0;
            do
            {
                ServicoSGT.Pessoa.RetornoOfPaginacaoOfEmpresaenvyzHPB51p1vPsU retorno = null;
                try
                {
                    retorno = svcPessoa.BuscarTransportadoresTerceiro(inicio, 100, true);
                }
                catch (Exception ex)
                {
                    Servicos.Log.TratarErro($"Falha ao consultar transportador : {ex.Message}", "ImportarCadastro");
                    return false;
                }

                if (retorno == null || !retorno.Status)
                {
                    Servicos.Log.TratarErro($"Falha ao buscar transportador terceiro: {retorno?.Mensagem}", "ImportarCadastro");

                    mensagemErro = retorno?.Mensagem ?? "";
                    return false;
                }

                foreach (var empresa in retorno.Objeto.Itens)
                {
                    string validacaoCamposObrigatorios = servicoEmpresa.ValidarCamposEmpresaIntegracao(empresa);
                    if (string.IsNullOrEmpty(validacaoCamposObrigatorios))
                    {
                        try
                        {
                            servicoEmpresa.AdicionarOutAtualizarEmpresa(empresa, unitOfWork, auditado, adminStringConexao);
                            svcPessoa.ConfirmarIntegracaoTransportadoresTerceiro(Utilidades.String.OnlyNumbers(empresa.CNPJ));
                        }
                        catch (Exception ex)
                        {
                            Servicos.Log.TratarErro($"Falha ao inserir transportador terceiro: {ex.Message}", "ImportarCadastro");
                        }
                    }
                    else if (!string.IsNullOrWhiteSpace(validacaoCamposObrigatorios))
                    {
                        Servicos.Log.TratarErro($"Falha ao inserir transportador terceiro por falta de campos obrigatórios: {validacaoCamposObrigatorios}", "ImportarCadastro");
                    }
                }

                limite = retorno.Objeto.NumeroTotalDeRegistro;
                inicio += 100;

                unitOfWork.FlushAndClear();
            }
            while (inicio < limite);

            integracao.RealizouIntegracaoCompletaDeTransportadorParaEmpresa = true;
            repIntegracao.Atualizar(integracao);

            mensagemErro = string.Empty;
            return true;
        }

        public static bool ImportarCadastroTipoContainer(out string mensagemErro, AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware tipoServicoMultisoftware, Dominio.ObjetosDeValor.Embarcador.Auditoria.Auditado auditado, Repositorio.UnitOfWork unitOfWork, string adminStringConexao)
        {
            mensagemErro = null;

            Servicos.WebService.Carga.Pedido serPedidoWS = new Servicos.WebService.Carga.Pedido(unitOfWork);

            Repositorio.Embarcador.Configuracoes.Integracao repIntegracao = new Repositorio.Embarcador.Configuracoes.Integracao(unitOfWork);
            Dominio.Entidades.Embarcador.Configuracoes.Integracao integracao = repIntegracao.BuscarPrimeiroRegistro();

            ServicoSGT.Empresa.EmpresaClient svcEmpresa = ObterClientEmpresa(integracao.URLIntegracaoCadastrosMulti, integracao.TokenIntegracaoCadastrosMulti);
            StringBuilder stMensagem = new StringBuilder();

            int limite = 100, inicio = 0;
            do
            {
                Servicos.ServicoSGT.Empresa.RetornoOfPaginacaoOfTipoContainerYCIsJlsP51p1vPsU retorno = null;
                try
                {
                    retorno = svcEmpresa.BuscarTipoContainerIntegracao(inicio, 100);
                }
                catch (Exception ex)
                {
                    Servicos.Log.TratarErro($"Falha ao consultar tipo container : {ex.Message}", "ImportarCadastro");
                    return false;
                }

                if (retorno == null || !retorno.Status)
                {
                    Servicos.Log.TratarErro($"Falha ao buscar tipo de container: {retorno?.Mensagem}", "ImportarCadastro");

                    mensagemErro = retorno?.Mensagem ?? "";
                    return false;
                }

                foreach (var tipoContainer in retorno.Objeto.Itens)
                {
                    stMensagem = new StringBuilder();
                    try
                    {
                        tipoContainer.Atualizar = true;
                        serPedidoWS.SalvarContainerTipo(tipoContainer, ref stMensagem, auditado);
                        svcEmpresa.ConfirmarIntegracaoTipoContainer(tipoContainer.Codigo);
                    }
                    catch (Exception ex)
                    {
                        Servicos.Log.TratarErro($"Falha ao inserir tipo de container: {ex.Message}", "ImportarCadastro");
                    }
                    if (stMensagem.Length > 0)
                    {
                        Servicos.Log.TratarErro($"Falha ao salvar tipo de container: {stMensagem.ToString()}", "ImportarCadastro");
                        mensagemErro = stMensagem.ToString();
                    }
                }

                limite = retorno.Objeto.NumeroTotalDeRegistro;
                inicio += 100;

                unitOfWork.FlushAndClear();
            }
            while (inicio < limite);

            mensagemErro = string.Empty;
            return true;
        }

        public static bool ImportarCadastroTerminalPortuario(out string mensagemErro, AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware tipoServicoMultisoftware, Dominio.ObjetosDeValor.Embarcador.Auditoria.Auditado auditado, Repositorio.UnitOfWork unitOfWork, string adminStringConexao)
        {
            mensagemErro = null;

            Servicos.WebService.Carga.Pedido serPedidoWS = new Servicos.WebService.Carga.Pedido(unitOfWork);

            Repositorio.Embarcador.Configuracoes.Integracao repIntegracao = new Repositorio.Embarcador.Configuracoes.Integracao(unitOfWork);
            Dominio.Entidades.Embarcador.Configuracoes.Integracao integracao = repIntegracao.BuscarPrimeiroRegistro();

            ServicoSGT.Empresa.EmpresaClient svcEmpresa = ObterClientEmpresa(integracao.URLIntegracaoCadastrosMulti, integracao.TokenIntegracaoCadastrosMulti);
            StringBuilder stMensagem = new StringBuilder();

            int limite = 100, inicio = 0;
            do
            {
                Servicos.ServicoSGT.Empresa.RetornoOfPaginacaoOfTerminalPortoYCIsJlsP51p1vPsU retorno = null;
                try
                {
                    retorno = svcEmpresa.BuscarTerminalPortoPendentesIntegracao(inicio, 100);
                }
                catch (Exception ex)
                {
                    Servicos.Log.TratarErro($"Falha ao consultar terminal : {ex.Message}", "ImportarCadastro");
                    return false;
                }

                if (retorno == null || !retorno.Status)
                {
                    Servicos.Log.TratarErro($"Falha ao buscar terminal portuario: {retorno?.Mensagem}", "ImportarCadastro");

                    mensagemErro = retorno?.Mensagem ?? "";
                    return false;
                }

                foreach (var terminal in retorno.Objeto.Itens)
                {
                    stMensagem = new StringBuilder();
                    try
                    {
                        terminal.Atualizar = true;
                        serPedidoWS.SalvarTerminalPorto(terminal, ref stMensagem, auditado);
                        svcEmpresa.ConfirmarIntegracaoTerminalPorto(terminal.Codigo);
                    }
                    catch (Exception ex)
                    {
                        Servicos.Log.TratarErro($"Falha ao inserir terminal portuario: {ex.Message}", "ImportarCadastro");
                    }
                    if (stMensagem.Length > 0)
                    {
                        Servicos.Log.TratarErro($"Falha ao salvar terminal portuario: {stMensagem.ToString()}", "ImportarCadastro");
                        mensagemErro = stMensagem.ToString();
                    }
                }

                limite = retorno.Objeto.NumeroTotalDeRegistro;
                inicio += 100;

                unitOfWork.FlushAndClear();
            }
            while (inicio < limite);

            mensagemErro = string.Empty;
            return true;
        }

        public static bool ImportarCadastroContainer(out string mensagemErro, AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware tipoServicoMultisoftware, Dominio.ObjetosDeValor.Embarcador.Auditoria.Auditado auditado, Repositorio.UnitOfWork unitOfWork, string adminStringConexao)
        {
            mensagemErro = null;

            Servicos.WebService.Carga.Pedido serPedidoWS = new Servicos.WebService.Carga.Pedido(unitOfWork);

            Repositorio.Embarcador.Configuracoes.Integracao repIntegracao = new Repositorio.Embarcador.Configuracoes.Integracao(unitOfWork);
            Dominio.Entidades.Embarcador.Configuracoes.Integracao integracao = repIntegracao.BuscarPrimeiroRegistro();

            ServicoSGT.Pessoa.PessoasClient svcPessoa = ObterClientPessoa(!string.IsNullOrWhiteSpace(integracao.URLIntegracaoCadastrosMultiSecundario) ? integracao.URLIntegracaoCadastrosMultiSecundario : integracao.URLIntegracaoCadastrosMulti, !string.IsNullOrWhiteSpace(integracao.TokenIntegracaoCadastrosMultiSecundario) ? integracao.TokenIntegracaoCadastrosMultiSecundario : integracao.TokenIntegracaoCadastrosMulti);
            StringBuilder stMensagem = new StringBuilder();

            int limite = 100, inicio = 0;
            do
            {
                Servicos.ServicoSGT.Pessoa.RetornoOfPaginacaoOfContainerYCIsJlsP51p1vPsU retorno = null;
                try
                {
                    retorno = svcPessoa.BuscarContainer(inicio, 100, true);
                }
                catch (Exception ex)
                {
                    Servicos.Log.TratarErro($"Falha ao consultar container : {ex.Message}", "ImportarCadastro");
                    return false;
                }

                if (retorno == null || !retorno.Status)
                {
                    Servicos.Log.TratarErro($"Falha ao buscar container: {retorno?.Mensagem}", "ImportarCadastro");

                    mensagemErro = retorno?.Mensagem ?? "";
                    return false;
                }

                foreach (var container in retorno.Objeto.Itens)
                {
                    stMensagem = new StringBuilder();
                    try
                    {
                        container.Atualizar = true;
                        if (container.TipoContainer != null)
                            container.TipoContainer.Atualizar = true;
                        serPedidoWS.SalvarContainer(container, ref stMensagem, auditado);
                        svcPessoa.ConfirmarIntegracaoContainer(container.Codigo.ToString("D"));
                    }
                    catch (Exception ex)
                    {
                        Servicos.Log.TratarErro($"Falha ao inserir container: {ex.Message}", "ImportarCadastro");
                    }
                    if (stMensagem.Length > 0)
                    {
                        Servicos.Log.TratarErro($"Falha ao salvar container: {stMensagem.ToString()}", "ImportarCadastro");
                        mensagemErro = stMensagem.ToString();
                    }
                }

                limite = retorno.Objeto.NumeroTotalDeRegistro;
                inicio += 100;

                unitOfWork.FlushAndClear();
            }
            while (inicio < limite);

            integracao.RealizouIntegracaoCompletaDeContainer = true;
            repIntegracao.Atualizar(integracao);

            mensagemErro = string.Empty;
            return true;
        }

        public static bool ImportarCadastroNavio(out string mensagemErro, AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware tipoServicoMultisoftware, Dominio.ObjetosDeValor.Embarcador.Auditoria.Auditado auditado, Repositorio.UnitOfWork unitOfWork, string adminStringConexao)
        {
            mensagemErro = null;

            Servicos.WebService.Carga.Pedido serPedidoWS = new Servicos.WebService.Carga.Pedido(unitOfWork);

            Repositorio.Embarcador.Configuracoes.Integracao repIntegracao = new Repositorio.Embarcador.Configuracoes.Integracao(unitOfWork);
            Dominio.Entidades.Embarcador.Configuracoes.Integracao integracao = repIntegracao.BuscarPrimeiroRegistro();

            ServicoSGT.Empresa.EmpresaClient svcEmpresa = ObterClientEmpresa(integracao.URLIntegracaoCadastrosMulti, integracao.TokenIntegracaoCadastrosMulti);
            StringBuilder stMensagem = new StringBuilder();

            int limite = 100, inicio = 0;
            do
            {
                Servicos.ServicoSGT.Empresa.RetornoOfPaginacaoOfNavioYCIsJlsP51p1vPsU retorno = null;
                try
                {
                    retorno = svcEmpresa.BuscarNaviosPendentesIntegracao(inicio, 100);
                }
                catch (Exception ex)
                {
                    Servicos.Log.TratarErro($"Falha ao consultar navio : {ex.Message}", "ImportarCadastro");
                    return false;
                }

                if (retorno == null || !retorno.Status)
                {
                    Servicos.Log.TratarErro($"Falha ao buscar navio: {retorno?.Mensagem}", "ImportarCadastro");

                    mensagemErro = retorno?.Mensagem ?? "";
                    return false;
                }

                foreach (var navio in retorno.Objeto.Itens)
                {
                    stMensagem = new StringBuilder();
                    try
                    {
                        navio.Atualizar = true;
                        serPedidoWS.SalvarNavio(navio, ref stMensagem, auditado, false);
                        svcEmpresa.ConfirmarIntegracaoNavio(navio.Codigo);
                    }
                    catch (Exception ex)
                    {
                        Servicos.Log.TratarErro($"Falha ao inserir navio: {ex.Message}", "ImportarCadastro");
                    }
                    if (stMensagem.Length > 0)
                    {
                        Servicos.Log.TratarErro($"Falha ao salvar navio: {stMensagem.ToString()}", "ImportarCadastro");
                        mensagemErro = stMensagem.ToString();
                    }
                }

                limite = retorno.Objeto.NumeroTotalDeRegistro;
                inicio += 100;

                unitOfWork.FlushAndClear();
            }
            while (inicio < limite);

            integracao.RealizouIntegracaoCompletaDeNavio = true;
            repIntegracao.Atualizar(integracao);

            mensagemErro = string.Empty;
            return true;
        }

        public static bool ImportarCadastroViagem(out string mensagemErro, AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware tipoServicoMultisoftware, Dominio.ObjetosDeValor.Embarcador.Auditoria.Auditado auditado, Repositorio.UnitOfWork unitOfWork, string adminStringConexao)
        {
            mensagemErro = null;

            Servicos.WebService.Carga.Pedido serPedidoWS = new Servicos.WebService.Carga.Pedido(unitOfWork);

            Repositorio.Embarcador.Configuracoes.Integracao repIntegracao = new Repositorio.Embarcador.Configuracoes.Integracao(unitOfWork);
            Dominio.Entidades.Embarcador.Configuracoes.Integracao integracao = repIntegracao.BuscarPrimeiroRegistro();

            ServicoSGT.Empresa.EmpresaClient svcEmpresa = ObterClientEmpresa(integracao.URLIntegracaoCadastrosMulti, integracao.TokenIntegracaoCadastrosMulti);
            StringBuilder stMensagem = new StringBuilder();

            int limite = 100, inicio = 0;
            do
            {
                Servicos.ServicoSGT.Empresa.RetornoOfPaginacaoOfViagemYCIsJlsP51p1vPsU retorno = null;
                try
                {
                    retorno = svcEmpresa.BuscarViagemPendentesIntegracao(inicio, 100);
                }
                catch (Exception ex)
                {
                    Servicos.Log.TratarErro($"Falha ao consultar viagem : {ex.Message}", "ImportarCadastro");
                    return false;
                }

                if (retorno == null || !retorno.Status)
                {
                    Servicos.Log.TratarErro($"Falha ao buscar viagem: {retorno?.Mensagem}", "ImportarCadastro");

                    mensagemErro = retorno?.Mensagem ?? "";
                    return false;
                }

                foreach (var viagem in retorno.Objeto.Itens)
                {
                    stMensagem = new StringBuilder();
                    try
                    {
                        viagem.Atualizar = true;
                        serPedidoWS.SalvarViagem(viagem, ref stMensagem, auditado, false);
                        svcEmpresa.ConfirmarIntegracaoViagem(viagem.Codigo);
                    }
                    catch (Exception ex)
                    {
                        Servicos.Log.TratarErro($"Falha ao inserir viagem: {ex.Message}", "ImportarCadastro");
                    }
                    if (stMensagem.Length > 0)
                    {
                        Servicos.Log.TratarErro($"Falha ao salvar viagem: {stMensagem.ToString()}", "ImportarCadastro");
                        mensagemErro = stMensagem.ToString();
                    }
                }

                limite = retorno.Objeto.NumeroTotalDeRegistro;
                inicio += 100;

                unitOfWork.FlushAndClear();
            }
            while (inicio < limite);

            integracao.RealizouIntegracaoCompletaDeViagem = true;
            repIntegracao.Atualizar(integracao);

            mensagemErro = string.Empty;
            return true;
        }

        public static bool ImportarCadastroPorto(out string mensagemErro, AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware tipoServicoMultisoftware, Dominio.ObjetosDeValor.Embarcador.Auditoria.Auditado auditado, Repositorio.UnitOfWork unitOfWork, string adminStringConexao)
        {
            mensagemErro = null;

            Servicos.WebService.Carga.Pedido serPedidoWS = new Servicos.WebService.Carga.Pedido(unitOfWork);

            Repositorio.Embarcador.Configuracoes.Integracao repIntegracao = new Repositorio.Embarcador.Configuracoes.Integracao(unitOfWork);
            Dominio.Entidades.Embarcador.Configuracoes.Integracao integracao = repIntegracao.BuscarPrimeiroRegistro();

            ServicoSGT.Empresa.EmpresaClient svcEmpresa = ObterClientEmpresa(integracao.URLIntegracaoCadastrosMulti, integracao.TokenIntegracaoCadastrosMulti);
            StringBuilder stMensagem = new StringBuilder();

            int limite = 100, inicio = 0;
            do
            {
                Servicos.ServicoSGT.Empresa.RetornoOfPaginacaoOfPortoYCIsJlsP51p1vPsU retorno = null;
                try
                {
                    retorno = svcEmpresa.BuscarPortoPendentesIntegracao(inicio, 100);
                }
                catch (Exception ex)
                {
                    Servicos.Log.TratarErro($"Falha ao consultar porto : {ex.Message}", "ImportarCadastro");
                    return false;
                }

                if (retorno == null || !retorno.Status)
                {
                    Servicos.Log.TratarErro($"Falha ao buscar porto: {retorno?.Mensagem}", "ImportarCadastro");

                    mensagemErro = retorno?.Mensagem ?? "";
                    return false;
                }

                foreach (var porto in retorno.Objeto.Itens)
                {
                    stMensagem = new StringBuilder();
                    try
                    {
                        porto.Atualizar = true;
                        serPedidoWS.SalvarPorto(porto, ref stMensagem, auditado);
                        svcEmpresa.ConfirmarIntegracaoPorto(porto.Codigo);
                    }
                    catch (Exception ex)
                    {
                        Servicos.Log.TratarErro($"Falha ao inserir Porto: {ex.Message}", "ImportarCadastro");
                    }
                    if (stMensagem.Length > 0)
                    {
                        Servicos.Log.TratarErro($"Falha ao salvar porto: {stMensagem.ToString()}", "ImportarCadastro");
                        mensagemErro = stMensagem.ToString();
                    }
                }

                limite = retorno.Objeto.NumeroTotalDeRegistro;
                inicio += 100;

                unitOfWork.FlushAndClear();
            }
            while (inicio < limite);

            repIntegracao.Atualizar(integracao);

            mensagemErro = string.Empty;
            return true;
        }

        public static bool ImportarCadastroProdutoEmbarcador(out string mensagemErro, AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware tipoServicoMultisoftware, Dominio.ObjetosDeValor.Embarcador.Auditoria.Auditado auditado, Repositorio.UnitOfWork unitOfWork, string adminStringConexao)
        {
            mensagemErro = null;

            Servicos.WebService.Carga.Pedido serPedidoWS = new Servicos.WebService.Carga.Pedido(unitOfWork);

            Repositorio.Embarcador.Configuracoes.Integracao repIntegracao = new Repositorio.Embarcador.Configuracoes.Integracao(unitOfWork);
            Dominio.Entidades.Embarcador.Configuracoes.Integracao integracao = repIntegracao.BuscarPrimeiroRegistro();

            Dominio.Entidades.Embarcador.Configuracoes.ConfiguracaoGeralCarga configuracaoGeralCarga = new Repositorio.Embarcador.Configuracoes.ConfiguracaoGeralCarga(unitOfWork).BuscarPrimeiroRegistro();
            Servicos.ProdutoEmbarcador servicoProdutoEmbarcador = new Servicos.ProdutoEmbarcador(unitOfWork, configuracaoGeralCarga);

            ServicoSGT.Empresa.EmpresaClient svcEmpresa = ObterClientEmpresa(integracao.URLIntegracaoCadastrosMulti, integracao.TokenIntegracaoCadastrosMulti);
            StringBuilder stMensagem = new StringBuilder();

            int limite = 100, inicio = 0;
            do
            {
                Servicos.ServicoSGT.Empresa.RetornoOfPaginacaoOfProdutoknUzgH8451p1vPsU retorno = null;
                try
                {
                    retorno = svcEmpresa.BuscarProdutoPendentesIntegracao(inicio, 100);
                }
                catch (Exception ex)
                {
                    Servicos.Log.TratarErro($"Falha ao consultar produto : {ex.Message}", "ImportarCadastro");
                    return false;
                }

                if (retorno == null || !retorno.Status)
                {
                    Servicos.Log.TratarErro($"Falha ao buscar produto: {retorno?.Mensagem}", "ImportarCadastro");

                    mensagemErro = retorno?.Mensagem ?? "";
                    return false;
                }

                foreach (var produto in retorno.Objeto.Itens)
                {
                    stMensagem = new StringBuilder();
                    try
                    {
                        produto.Atualizar = true;
                        servicoProdutoEmbarcador.IntegrarProduto(produto.CodigoProduto, produto.CodigocEAN, produto.CodigoGrupoProduto.ToInt(), produto.DescricaoProduto, produto.PesoUnitario, null, produto.MetroCubito, auditado, produto.CodigoDocumentacao, produto.InativarCadastro, produto.Atualizar, produto.CodigoNCM, tipoServicoMultisoftware, produto.QuantidadeCaixa, produto.SiglaUnidade, produto.TemperaturaTransporte, produto.PesoLiquidoUnitario, produto.QtdPalet, produto.AlturaCM, produto.LarguraCM, produto.ComprimentoCM, produto.Observacao, produto.QuantidadeCaixaPorPallet);
                        svcEmpresa.ConfirmarIntegracaoProduto(produto.Codigo);
                    }
                    catch (Exception ex)
                    {
                        Servicos.Log.TratarErro($"Falha ao inserir produto: {ex.Message}", "ImportarCadastro");
                    }
                    if (stMensagem.Length > 0)
                    {
                        Servicos.Log.TratarErro($"Falha ao salvar produto: {stMensagem.ToString()}", "ImportarCadastro");
                        mensagemErro = stMensagem.ToString();
                    }
                }

                limite = retorno.Objeto.NumeroTotalDeRegistro;
                inicio += 100;

                unitOfWork.FlushAndClear();
            }
            while (inicio < limite);

            repIntegracao.Atualizar(integracao);

            mensagemErro = string.Empty;
            return true;
        }


        #endregion

        #region Métodos Privados


        private static Servicos.ServicoPessoa.SGT.PessoasClient ObterClientPessoaCore(string url, string token)
        {
            url = url.ToLower();

            if (!url.EndsWith("/"))
                url += "/";

            url += "Pessoas.svc";

            Servicos.ServicoPessoa.SGT.PessoasClient client = null;
            System.ServiceModel.EndpointAddress endpointAddress = new System.ServiceModel.EndpointAddress(url);

            if (url.StartsWith("https"))
            {
                //System.ServiceModel.WSHttpBinding binding = new System.ServiceModel.WSHttpBinding(System.ServiceModel.SecurityMode.None);
                System.ServiceModel.BasicHttpBinding binding = new System.ServiceModel.BasicHttpBinding();

                binding.MaxReceivedMessageSize = int.MaxValue;
                binding.ReceiveTimeout = new TimeSpan(0, 5, 0);
                binding.SendTimeout = new TimeSpan(0, 5, 0);

                binding.Security.Mode = System.ServiceModel.BasicHttpSecurityMode.Transport;
                //binding.Security.Message.ClientCredentialType = System.ServiceModel.BasicHttpMessageCredentialType.Certificate;

                client = new Servicos.ServicoPessoa.SGT.PessoasClient(binding, endpointAddress);
            }
            else
            {
                System.ServiceModel.BasicHttpBinding binding = new System.ServiceModel.BasicHttpBinding();

                binding.MaxReceivedMessageSize = int.MaxValue;
                binding.ReceiveTimeout = new TimeSpan(0, 5, 0);
                binding.SendTimeout = new TimeSpan(0, 5, 0);

                client = new Servicos.ServicoPessoa.SGT.PessoasClient(binding, endpointAddress);
            }

            System.ServiceModel.OperationContextScope scope = new System.ServiceModel.OperationContextScope(client.InnerChannel);
            System.ServiceModel.Channels.MessageHeader header = System.ServiceModel.Channels.MessageHeader.CreateHeader("Token", "Token", token);
            System.ServiceModel.OperationContext.Current.OutgoingMessageHeaders.Add(header);

            return client;
        }

        private static ServicoSGT.Pessoa.PessoasClient ObterClientPessoa(string url, string token)
        {
            url = url.ToLower();

            if (!url.EndsWith("/"))
                url += "/";

            url += "Pessoas.svc";

            ServicoSGT.Pessoa.PessoasClient client = null;
            System.ServiceModel.EndpointAddress endpointAddress = new System.ServiceModel.EndpointAddress(url);

            if (url.StartsWith("https"))
            {
                //System.ServiceModel.WSHttpBinding binding = new System.ServiceModel.WSHttpBinding(System.ServiceModel.SecurityMode.None);
                System.ServiceModel.BasicHttpBinding binding = new System.ServiceModel.BasicHttpBinding();

                binding.MaxReceivedMessageSize = int.MaxValue;
                binding.ReceiveTimeout = new TimeSpan(0, 5, 0);
                binding.SendTimeout = new TimeSpan(0, 5, 0);

                binding.Security.Mode = System.ServiceModel.BasicHttpSecurityMode.Transport;
                //binding.Security.Message.ClientCredentialType = System.ServiceModel.BasicHttpMessageCredentialType.Certificate;

                client = new ServicoSGT.Pessoa.PessoasClient(binding, endpointAddress);
            }
            else
            {
                System.ServiceModel.BasicHttpBinding binding = new System.ServiceModel.BasicHttpBinding();

                binding.MaxReceivedMessageSize = int.MaxValue;
                binding.ReceiveTimeout = new TimeSpan(0, 5, 0);
                binding.SendTimeout = new TimeSpan(0, 5, 0);

                client = new ServicoSGT.Pessoa.PessoasClient(binding, endpointAddress);
            }

            System.ServiceModel.OperationContextScope scope = new System.ServiceModel.OperationContextScope(client.InnerChannel);
            System.ServiceModel.Channels.MessageHeader header = System.ServiceModel.Channels.MessageHeader.CreateHeader("Token", "Token", token);
            System.ServiceModel.OperationContext.Current.OutgoingMessageHeaders.Add(header);

            return client;
        }

        private static ServicoSGT.Empresa.EmpresaClient ObterClientEmpresa(string url, string token)
        {
            url = url.ToLower();

            if (!url.EndsWith("/"))
                url += "/";

            url += "Empresa.svc";

            ServicoSGT.Empresa.EmpresaClient client = null;
            System.ServiceModel.EndpointAddress endpointAddress = new System.ServiceModel.EndpointAddress(url);

            if (url.StartsWith("https"))
            {
                //System.ServiceModel.WSHttpBinding binding = new System.ServiceModel.WSHttpBinding(System.ServiceModel.SecurityMode.None);
                System.ServiceModel.BasicHttpBinding binding = new System.ServiceModel.BasicHttpBinding();

                binding.MaxReceivedMessageSize = int.MaxValue;
                binding.ReceiveTimeout = new TimeSpan(0, 5, 0);
                binding.SendTimeout = new TimeSpan(0, 5, 0);

                binding.Security.Mode = System.ServiceModel.BasicHttpSecurityMode.Transport;
                //binding.Security.Message.ClientCredentialType = System.ServiceModel.BasicHttpMessageCredentialType.Certificate;

                client = new ServicoSGT.Empresa.EmpresaClient(binding, endpointAddress);
            }
            else
            {
                System.ServiceModel.BasicHttpBinding binding = new System.ServiceModel.BasicHttpBinding();

                binding.MaxReceivedMessageSize = int.MaxValue;
                binding.ReceiveTimeout = new TimeSpan(0, 5, 0);
                binding.SendTimeout = new TimeSpan(0, 5, 0);

                client = new ServicoSGT.Empresa.EmpresaClient(binding, endpointAddress);
            }

            System.ServiceModel.OperationContextScope scope = new System.ServiceModel.OperationContextScope(client.InnerChannel);
            System.ServiceModel.Channels.MessageHeader header = System.ServiceModel.Channels.MessageHeader.CreateHeader("Token", "Token", token);
            System.ServiceModel.OperationContext.Current.OutgoingMessageHeaders.Add(header);

            return client;
        }

        #endregion
    }
}
