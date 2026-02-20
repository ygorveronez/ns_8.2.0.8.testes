using System;
using System.Collections.Generic;
using System.Linq;

namespace Servicos.Embarcador.Pessoa
{
    public class FuncionarioComissao : ServicoBase
    {
        public FuncionarioComissao(Repositorio.UnitOfWork unitOfWork) : base(unitOfWork) { }

        #region Métodos Públicos

        public static void EtapaAprovacao(ref Dominio.Entidades.Embarcador.Usuarios.Comissao.FuncionarioComissao funcionarioComissao, Repositorio.UnitOfWork unitOfWork, AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware tipoServicoMultisoftware, Dominio.Enumeradores.TipoAmbiente tipoAmbiente, string stringConexao, Dominio.ObjetosDeValor.Embarcador.Auditoria.Auditado Auditado)
        {
            // Instancia Repositorios
            List<Dominio.Entidades.Embarcador.Usuarios.AlcadaComissao.RegraFuncionarioComissao> regras = Servicos.Embarcador.Pessoa.FuncionarioComissao.VerificarRegrasAutorizacao(funcionarioComissao, unitOfWork);

            bool possuiRegra = regras.Count() > 0;
            bool agAprovacao = true;

            if (possuiRegra)
            {
                funcionarioComissao.Situacao = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoFuncionarioComissao.AgAprovacao;

                agAprovacao = Servicos.Embarcador.Pessoa.FuncionarioComissao.CriarRegrasAutorizacao(regras, funcionarioComissao, funcionarioComissao.Operador, tipoServicoMultisoftware, stringConexao, unitOfWork);

                if (!agAprovacao)
                    funcionarioComissao.Situacao = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoFuncionarioComissao.Aprovada;
            }
            else
            {
                funcionarioComissao.Situacao = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoFuncionarioComissao.SemRegra;
            }

            Servicos.Embarcador.Pessoa.FuncionarioComissao.FuncionarioComissaoAprovada(funcionarioComissao, unitOfWork, tipoServicoMultisoftware, tipoAmbiente);
        }

        public static List<Dominio.Entidades.Embarcador.Usuarios.AlcadaComissao.RegraFuncionarioComissao> VerificarRegrasAutorizacao(Dominio.Entidades.Embarcador.Usuarios.Comissao.FuncionarioComissao funcionarioComissao, Repositorio.UnitOfWork unitOfWork)
        {
            Repositorio.Embarcador.Usuarios.Comissao.FuncionarioComissaoTitulo repFuncionarioComissaoTitulo = new Repositorio.Embarcador.Usuarios.Comissao.FuncionarioComissaoTitulo(unitOfWork);
            Repositorio.Embarcador.Usuarios.Comissao.FuncionarioComissao repFuncionarioComissao = new Repositorio.Embarcador.Usuarios.Comissao.FuncionarioComissao(unitOfWork);
            Repositorio.Embarcador.Usuarios.AlcadaComissao.RegraFuncionarioComissao repRegraFuncionarioComissao = new Repositorio.Embarcador.Usuarios.AlcadaComissao.RegraFuncionarioComissao(unitOfWork);
            List<Dominio.Entidades.Embarcador.Usuarios.AlcadaComissao.RegraFuncionarioComissao> listaRegras = new List<Dominio.Entidades.Embarcador.Usuarios.AlcadaComissao.RegraFuncionarioComissao>();
            List<Dominio.Entidades.Embarcador.Usuarios.AlcadaComissao.RegraFuncionarioComissao> listaFiltrada = new List<Dominio.Entidades.Embarcador.Usuarios.AlcadaComissao.RegraFuncionarioComissao>();
            List<Dominio.Entidades.Embarcador.Usuarios.AlcadaComissao.RegraFuncionarioComissao> alcadasCompativeis;

            decimal valorTotalFinal = 0;
            List<Dominio.Entidades.Embarcador.Usuarios.Comissao.FuncionarioComissaoTitulo> comissao = repFuncionarioComissaoTitulo.BuscarPorFuncionarioComissao(funcionarioComissao.Codigo);
            valorTotalFinal = comissao.Select(t => t.ValorFinal).Sum();

            alcadasCompativeis = repRegraFuncionarioComissao.AlcadasPorFuncionario(funcionarioComissao.Funcionario.Codigo, DateTime.Today);
            listaRegras.AddRange(alcadasCompativeis);

            alcadasCompativeis = repRegraFuncionarioComissao.AlcadasPorValor(valorTotalFinal, DateTime.Today);
            listaRegras.AddRange(alcadasCompativeis);

            listaRegras = listaRegras.Distinct().ToList();
            if (listaRegras.Count() > 0)
            {
                listaFiltrada.AddRange(listaRegras);
                foreach (Dominio.Entidades.Embarcador.Usuarios.AlcadaComissao.RegraFuncionarioComissao regra in listaRegras)
                {
                    if (regra.RegraPorFuncionario)
                    {
                        bool valido = false;
                        if (regra.AlcadasFuncionario.All(o => o.Condicao == Dominio.ObjetosDeValor.Embarcador.Enumeradores.CondicaoAutorizao.IgualA && o.Juncao == Dominio.ObjetosDeValor.Embarcador.Enumeradores.JuncaoAutorizao.E && o.Funcionario.Codigo == funcionarioComissao.Funcionario.Codigo))
                            valido = true;
                        else if (regra.AlcadasFuncionario.Any(o => o.Condicao == Dominio.ObjetosDeValor.Embarcador.Enumeradores.CondicaoAutorizao.IgualA && o.Juncao == Dominio.ObjetosDeValor.Embarcador.Enumeradores.JuncaoAutorizao.Ou && o.Funcionario.Codigo == funcionarioComissao.Funcionario.Codigo))
                            valido = true;
                        else if (regra.AlcadasFuncionario.Any(o => o.Condicao == Dominio.ObjetosDeValor.Embarcador.Enumeradores.CondicaoAutorizao.DiferenteDe && o.Juncao == Dominio.ObjetosDeValor.Embarcador.Enumeradores.JuncaoAutorizao.Ou && o.Funcionario.Codigo != funcionarioComissao.Funcionario.Codigo))
                            valido = true;
                        else if (regra.AlcadasFuncionario.All(o => o.Condicao == Dominio.ObjetosDeValor.Embarcador.Enumeradores.CondicaoAutorizao.DiferenteDe && o.Juncao == Dominio.ObjetosDeValor.Embarcador.Enumeradores.JuncaoAutorizao.E && o.Funcionario.Codigo != funcionarioComissao.Funcionario.Codigo))
                            valido = true;

                        if (!valido)
                        {
                            listaFiltrada.Remove(regra);
                            continue;
                        }
                    }

                    if (regra.RegraPorValor)
                    {
                        bool valido = false;
                        if (regra.AlcadasValor.All(o => o.Condicao == Dominio.ObjetosDeValor.Embarcador.Enumeradores.CondicaoAutorizao.IgualA && o.Juncao == Dominio.ObjetosDeValor.Embarcador.Enumeradores.JuncaoAutorizao.E && o.Valor == valorTotalFinal))
                            valido = true;
                        else if (regra.AlcadasValor.Any(o => o.Condicao == Dominio.ObjetosDeValor.Embarcador.Enumeradores.CondicaoAutorizao.IgualA && o.Juncao == Dominio.ObjetosDeValor.Embarcador.Enumeradores.JuncaoAutorizao.Ou && o.Valor == valorTotalFinal))
                            valido = true;
                        else if (regra.AlcadasValor.Any(o => o.Condicao == Dominio.ObjetosDeValor.Embarcador.Enumeradores.CondicaoAutorizao.DiferenteDe && o.Juncao == Dominio.ObjetosDeValor.Embarcador.Enumeradores.JuncaoAutorizao.Ou && o.Valor != valorTotalFinal))
                            valido = true;
                        else if (regra.AlcadasValor.All(o => o.Condicao == Dominio.ObjetosDeValor.Embarcador.Enumeradores.CondicaoAutorizao.DiferenteDe && o.Juncao == Dominio.ObjetosDeValor.Embarcador.Enumeradores.JuncaoAutorizao.E && o.Valor != valorTotalFinal))
                            valido = true;
                        if (regra.AlcadasValor.All(o => o.Condicao == Dominio.ObjetosDeValor.Embarcador.Enumeradores.CondicaoAutorizao.MaiorIgualQue && o.Juncao == Dominio.ObjetosDeValor.Embarcador.Enumeradores.JuncaoAutorizao.E && valorTotalFinal >= o.Valor))
                            valido = true;
                        else if (regra.AlcadasValor.Any(o => o.Condicao == Dominio.ObjetosDeValor.Embarcador.Enumeradores.CondicaoAutorizao.MaiorIgualQue && o.Juncao == Dominio.ObjetosDeValor.Embarcador.Enumeradores.JuncaoAutorizao.Ou && valorTotalFinal >= o.Valor))
                            valido = true;
                        if (regra.AlcadasValor.All(o => o.Condicao == Dominio.ObjetosDeValor.Embarcador.Enumeradores.CondicaoAutorizao.MenorIgualQue && o.Juncao == Dominio.ObjetosDeValor.Embarcador.Enumeradores.JuncaoAutorizao.E && valorTotalFinal <= o.Valor))
                            valido = true;
                        else if (regra.AlcadasValor.Any(o => o.Condicao == Dominio.ObjetosDeValor.Embarcador.Enumeradores.CondicaoAutorizao.MenorIgualQue && o.Juncao == Dominio.ObjetosDeValor.Embarcador.Enumeradores.JuncaoAutorizao.Ou && valorTotalFinal <= o.Valor))
                            valido = true;
                        if (regra.AlcadasValor.All(o => o.Condicao == Dominio.ObjetosDeValor.Embarcador.Enumeradores.CondicaoAutorizao.MaiorQue && o.Juncao == Dominio.ObjetosDeValor.Embarcador.Enumeradores.JuncaoAutorizao.E && valorTotalFinal > o.Valor))
                            valido = true;
                        else if (regra.AlcadasValor.Any(o => o.Condicao == Dominio.ObjetosDeValor.Embarcador.Enumeradores.CondicaoAutorizao.MaiorQue && o.Juncao == Dominio.ObjetosDeValor.Embarcador.Enumeradores.JuncaoAutorizao.Ou && valorTotalFinal > o.Valor))
                            valido = true;
                        if (regra.AlcadasValor.All(o => o.Condicao == Dominio.ObjetosDeValor.Embarcador.Enumeradores.CondicaoAutorizao.MenorQue && o.Juncao == Dominio.ObjetosDeValor.Embarcador.Enumeradores.JuncaoAutorizao.E && valorTotalFinal < o.Valor))
                            valido = true;
                        else if (regra.AlcadasValor.Any(o => o.Condicao == Dominio.ObjetosDeValor.Embarcador.Enumeradores.CondicaoAutorizao.MenorQue && o.Juncao == Dominio.ObjetosDeValor.Embarcador.Enumeradores.JuncaoAutorizao.Ou && valorTotalFinal < o.Valor))
                            valido = true;

                        if (!valido)
                        {
                            listaFiltrada.Remove(regra);
                            continue;
                        }
                    }
                }
            }

            return listaFiltrada;
        }

        public static bool CriarRegrasAutorizacao(List<Dominio.Entidades.Embarcador.Usuarios.AlcadaComissao.RegraFuncionarioComissao> listaFiltrada, Dominio.Entidades.Embarcador.Usuarios.Comissao.FuncionarioComissao funcionarioComissao, Dominio.Entidades.Usuario usuario, AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware tipoServicoMultisoftware, string stringConexao, Repositorio.UnitOfWork unitOfWork)
        {
            bool possuiRegraPendente = false;

            Servicos.Embarcador.Notificacao.Notificacao serNotificacao = new Servicos.Embarcador.Notificacao.Notificacao(stringConexao, null, tipoServicoMultisoftware, string.Empty);
            Repositorio.Embarcador.Usuarios.AlcadaComissao.AprovacaoAlcadaFuncionarioComissao repAprovacaoAlcadaFuncionarioComissao = new Repositorio.Embarcador.Usuarios.AlcadaComissao.AprovacaoAlcadaFuncionarioComissao(unitOfWork);

            if (listaFiltrada == null || listaFiltrada.Count() == 0)
                throw new ArgumentException("Lista de Regras deve ser maior que 0");

            foreach (Dominio.Entidades.Embarcador.Usuarios.AlcadaComissao.RegraFuncionarioComissao regra in listaFiltrada)
            {
                if (regra.NumeroAprovadores > 0)
                {
                    possuiRegraPendente = true;
                    foreach (Dominio.Entidades.Usuario aprovador in regra.Aprovadores)
                    {
                        Dominio.Entidades.Embarcador.Usuarios.AlcadaComissao.AprovacaoAlcadaFuncionarioComissao autorizacao = new Dominio.Entidades.Embarcador.Usuarios.AlcadaComissao.AprovacaoAlcadaFuncionarioComissao
                        {
                            FuncionarioComissao = funcionarioComissao,
                            Usuario = aprovador,
                            RegraFuncionarioComissao = regra,
                            DataCriacao = funcionarioComissao.DataGeracao,
                        };
                        repAprovacaoAlcadaFuncionarioComissao.Inserir(autorizacao);

                        string nota = string.Format(Localization.Resources.Pessoas.FuncionarioComissao.UsuarioCriouComissaoFuncionario, usuario.Nome, funcionarioComissao.Numero);
                        serNotificacao.GerarNotificacaoEmail(aprovador, usuario, funcionarioComissao.Codigo, "Pessoas/FuncionarioComissao", string.Empty, nota, Dominio.ObjetosDeValor.Embarcador.Enumeradores.IconesNotificacao.cifra, Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoNotificacao.credito, tipoServicoMultisoftware, unitOfWork);
                    }
                }
                else
                {
                    Dominio.Entidades.Embarcador.Usuarios.AlcadaComissao.AprovacaoAlcadaFuncionarioComissao autorizacao = new Dominio.Entidades.Embarcador.Usuarios.AlcadaComissao.AprovacaoAlcadaFuncionarioComissao
                    {
                        FuncionarioComissao = funcionarioComissao,
                        Usuario = null,
                        RegraFuncionarioComissao = regra,
                        Situacao = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoAlcadaRegra.Aprovada,
                        Data = DateTime.Now,
                        Motivo = "Alçada aprovada pela Regra " + regra.Descricao,
                        DataCriacao = funcionarioComissao.DataGeracao,
                    };
                    repAprovacaoAlcadaFuncionarioComissao.Inserir(autorizacao);
                }
            }

            return possuiRegraPendente;
        }

        public static void FuncionarioComissaoAprovada(Dominio.Entidades.Embarcador.Usuarios.Comissao.FuncionarioComissao funcionarioComissao, Repositorio.UnitOfWork unitOfWork, AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware tipoServicoMultisoftware, Dominio.Enumeradores.TipoAmbiente tipoAmbiente)
        {
            if (funcionarioComissao.Situacao != Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoFuncionarioComissao.Aprovada)
                return;

            string erro = string.Empty;
            GerarAtualizaTitulo(funcionarioComissao, unitOfWork, tipoServicoMultisoftware, out erro, tipoAmbiente, funcionarioComissao.Situacao);
        }

        public bool AtualizarTitulo(Dominio.Entidades.Embarcador.Usuarios.Comissao.FuncionarioComissao funcionarioComissao, Repositorio.UnitOfWork unidadeTrabalho, AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware tipoServicoMultisoftware, out string erro, Dominio.Enumeradores.TipoAmbiente tipoAmbiente, Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoFuncionarioComissao situacaoFuncionarioComissao)
        {
            return GerarAtualizaTitulo(funcionarioComissao, unidadeTrabalho, tipoServicoMultisoftware, out erro, tipoAmbiente, situacaoFuncionarioComissao);
        }

        #endregion

        #region Métodos Privados

        private static bool GerarAtualizaTitulo(Dominio.Entidades.Embarcador.Usuarios.Comissao.FuncionarioComissao funcionarioComissao, Repositorio.UnitOfWork unidadeTrabalho, AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware tipoServicoMultisoftware, out string erro, Dominio.Enumeradores.TipoAmbiente tipoAmbiente, Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoFuncionarioComissao situacaoFuncionarioComissao)
        {
            erro = string.Empty;

            Repositorio.Embarcador.Usuarios.Comissao.FuncionarioComissao repFuncionarioComissao = new Repositorio.Embarcador.Usuarios.Comissao.FuncionarioComissao(unidadeTrabalho);
            Repositorio.Embarcador.Financeiro.Titulo repTitulo = new Repositorio.Embarcador.Financeiro.Titulo(unidadeTrabalho);
            Repositorio.Cliente repCliente = new Repositorio.Cliente(unidadeTrabalho);
            Servicos.Embarcador.Financeiro.ProcessoMovimento svcProcessoMovimento = new Financeiro.ProcessoMovimento(unidadeTrabalho.StringConexao);

            if (situacaoFuncionarioComissao != Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoFuncionarioComissao.Cancelado)
            {
                if (funcionarioComissao.Funcionario.TipoMovimentoComissao != null && funcionarioComissao.Funcionario.DiaComissao > 0 && funcionarioComissao.ValorComissao > 0)
                {
                    Dominio.Entidades.Embarcador.Financeiro.Titulo titulo = new Dominio.Entidades.Embarcador.Financeiro.Titulo();

                    DateTime? dataVencimento = null;
                    try
                    {
                        dataVencimento = new DateTime(DateTime.Now.Year, DateTime.Now.Month, funcionarioComissao.Funcionario.DiaComissao);
                    }
                    catch (Exception)
                    {
                        dataVencimento = DateTime.Now.Date;
                    }

                    if (dataVencimento.Value.Day <= DateTime.Now.Day)
                    {
                        dataVencimento = dataVencimento.Value.AddMonths(1);
                        try
                        {
                            dataVencimento = new DateTime(dataVencimento.Value.Year, dataVencimento.Value.Month, funcionarioComissao.Funcionario.DiaComissao);
                        }
                        catch (Exception)
                        {
                        }
                    }

                    Dominio.Entidades.Cliente pessoa = repCliente.BuscarPorCPFCNPJ(double.Parse(funcionarioComissao.Funcionario.CPF));
                    if (pessoa == null)
                    {
                        if (funcionarioComissao.Funcionario.Localidade == null)
                        {
                            erro = "Funcionário está com o endereço incompleto, favor ajustar antes de prosseguir.";
                            return false;
                        }

                        pessoa = Servicos.Embarcador.Pessoa.Pessoa.ConverterFuncionario(funcionarioComissao.Funcionario, unidadeTrabalho);
                        repCliente.Inserir(pessoa);
                    }

                    titulo.TipoTitulo = Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoTitulo.Pagar;
                    titulo.DataEmissao = DateTime.Now;
                    titulo.DataVencimento = dataVencimento;
                    titulo.DataProgramacaoPagamento = dataVencimento;
                    titulo.Pessoa = pessoa;
                    titulo.GrupoPessoas = pessoa.GrupoPessoas;
                    titulo.Sequencia = 1;
                    titulo.ValorOriginal = funcionarioComissao.ValorComissao;
                    titulo.ValorPendente = funcionarioComissao.ValorComissao;
                    titulo.StatusTitulo = Dominio.ObjetosDeValor.Embarcador.Enumeradores.StatusTitulo.EmAberto;
                    titulo.DataAlteracao = DateTime.Now;
                    titulo.Observacao = string.Concat("Referente à Comissão do Funcionário nº " + funcionarioComissao.Numero.ToString() + "." + (!string.IsNullOrWhiteSpace(funcionarioComissao.Observacao) ? " - Obs: " + funcionarioComissao.Observacao : string.Empty)).Trim();
                    titulo.Empresa = funcionarioComissao.Empresa;
                    titulo.ValorTituloOriginal = titulo.ValorOriginal;
                    titulo.TipoDocumentoTituloOriginal = "Comissão Funcionário";
                    titulo.NumeroDocumentoTituloOriginal = funcionarioComissao.Numero.ToString();
                    titulo.FormaTitulo = Dominio.ObjetosDeValor.Embarcador.Enumeradores.FormaTitulo.Outros;
                    titulo.TipoMovimento = funcionarioComissao.Funcionario.TipoMovimentoComissao;

                    titulo.DataLancamento = DateTime.Now;
                    titulo.Usuario = funcionarioComissao.Funcionario;

                    if (tipoServicoMultisoftware == AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiNFe)
                        titulo.TipoAmbiente = tipoAmbiente;

                    repTitulo.Inserir(titulo);

                    funcionarioComissao.Titulo = titulo;
                    repFuncionarioComissao.Atualizar(funcionarioComissao);

                    if (!svcProcessoMovimento.GerarMovimentacao(out erro, titulo.TipoMovimento, titulo.DataEmissao.Value, titulo.ValorOriginal, funcionarioComissao.Numero.ToString(), titulo.Observacao, unidadeTrabalho, Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoDocumentoMovimento.Outros, tipoServicoMultisoftware, 0, null, null, titulo.Codigo, null, null, null, titulo.DataEmissao.Value))
                        return false;
                }
            }
            else
            {
                if (!repFuncionarioComissao.VerificarTituloComissaoCancelado(funcionarioComissao.Codigo))
                {
                    erro = "Favor cancelar o Título dessa Comissão antes de Cancelar a mesma";
                    return false;
                }

                funcionarioComissao.Situacao = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoFuncionarioComissao.Cancelado;
                funcionarioComissao.Titulo = null;
                repFuncionarioComissao.Atualizar(funcionarioComissao);
            }

            return true;
        }

        #endregion
    }
}
