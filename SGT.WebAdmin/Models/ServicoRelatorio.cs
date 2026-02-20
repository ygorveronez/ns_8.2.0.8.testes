using Dominio.Excecoes.Embarcador;
using SGT.WebAdmin.Controllers;
using System;
using System.Collections.Generic;

namespace SGT.WebAdmin.Models
{
    public class ServicoRelatorio
    {
        public int SalvarConfiguracaoRelatorio(string jsonRelatorio, Dominio.Entidades.Usuario usuario, Repositorio.UnitOfWork unitOfWork, AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware tipoServico, bool usaPermissaoControladorRelatorios)
        {
            Dominio.ObjetosDeValor.Embarcador.Relatorios.Relatorio dynRelatorio = Newtonsoft.Json.JsonConvert.DeserializeObject<Dominio.ObjetosDeValor.Embarcador.Relatorios.Relatorio>(jsonRelatorio);

            if (usaPermissaoControladorRelatorios)
            {
                bool permiteSalvarNovoRelatorio = usuario.PerfilAcesso?.PermiteSalvarNovoRelatorio ?? usuario.PermiteSalvarNovoRelatorio;
                bool permiteTornarRelatorioPadrao = usuario.PerfilAcesso?.PermiteTornarRelatorioPadrao ?? usuario.PermiteTornarRelatorioPadrao;
                bool permiteSalvarConfiguracoesRelatoriosParaTodos = usuario.PerfilAcesso?.PermiteSalvarConfiguracoesRelatoriosParaTodos ?? usuario.PermiteSalvarConfiguracoesRelatoriosParaTodos;
                if (!permiteSalvarNovoRelatorio && dynRelatorio.NovoRelatorio)
                    throw new ServicoException("Seu usuário não possui permissão para criar um novo relatório.");
                else if (!permiteTornarRelatorioPadrao && dynRelatorio.Padrao)
                    throw new ServicoException("Seu usuário não possui permissão para tornar o relatório como padrão.");
                else if (!permiteSalvarConfiguracoesRelatoriosParaTodos && dynRelatorio.RelatorioParaTodosUsuarios)
                    throw new ServicoException("Seu usuário não possui permissão para salvar a configuração para todos os usuários.");
                else if (!permiteSalvarNovoRelatorio && !dynRelatorio.NovoRelatorio)
                    throw new ServicoException("Seu usuário não possui permissão para salvar a configuração de um relatório existente.");
            }

            Servicos.Embarcador.Relatorios.Relatorio serRelatorio = new Servicos.Embarcador.Relatorios.Relatorio();

            Models.Grid.Relatorio gridRelatorio = new Models.Grid.Relatorio();

            Repositorio.Embarcador.Relatorios.Relatorio repRelatorio = new Repositorio.Embarcador.Relatorios.Relatorio(unitOfWork);
            Repositorio.Embarcador.Relatorios.RelatorioColuna repRelatorioColuna = new Repositorio.Embarcador.Relatorios.RelatorioColuna(unitOfWork);

            if (dynRelatorio.Padrao)
            {
                Dominio.Entidades.Embarcador.Relatorios.Relatorio relatorioPadrao = repRelatorio.BuscarPadraoPorCodigoControleRelatorio(dynRelatorio.CodigoControleRelatorios, tipoServico);

                if (relatorioPadrao != null)
                {
                    relatorioPadrao.Padrao = false;
                    repRelatorio.Atualizar(relatorioPadrao);
                }
            }

            if (!dynRelatorio.NovoRelatorio)
            {
                Dominio.Entidades.Embarcador.Relatorios.Relatorio relatorio = repRelatorio.BuscarPorCodigo(dynRelatorio.Codigo);

                if (!relatorio.PadraoMultisoftware)
                {
                    List<Dominio.Entidades.Embarcador.Relatorios.RelatorioColuna> Colunas = repRelatorioColuna.BuscarPorRelatorio(relatorio.Codigo);

                    foreach (Dominio.Entidades.Embarcador.Relatorios.RelatorioColuna coluna in Colunas)
                        repRelatorioColuna.Deletar(coluna);

                    serRelatorio.AlterarDadosRelatorio(dynRelatorio, relatorio, tipoServico);
                    gridRelatorio.SetarRelatorioPelaGrid(dynRelatorio.Grid, relatorio);

                    foreach (Dominio.Entidades.Embarcador.Relatorios.RelatorioColuna coluna in relatorio.Colunas)
                        repRelatorioColuna.Inserir(coluna);

                    relatorio.DataAlteracao = DateTime.Now;
                    relatorio.Usuario = usuario;

                    repRelatorio.Atualizar(relatorio);
                }
                else
                {
                    if (!relatorio.Padrao && dynRelatorio.Padrao)
                    {
                        relatorio.Padrao = true;
                        repRelatorio.Atualizar(relatorio);
                    }
                    else
                        throw new ServicoException("Não é possível alterar o relatório padrão da Multisoftware, crie um novo relatório e tente novamente.");
                }

                return relatorio.Codigo;
            }
           
            Dominio.Entidades.Embarcador.Relatorios.Relatorio relatorioDesc = repRelatorio.BuscarCodigoControleRelatorioEDescricao(dynRelatorio.CodigoControleRelatorios, dynRelatorio.Descricao, tipoServico, dynRelatorio.RelatorioParaTodosUsuarios);
            if (relatorioDesc != null)
                throw new ServicoException("Já existe um relatório com a descrição " + dynRelatorio.Descricao + ". Por favor, informe outra descrição.");

            Dominio.Entidades.Embarcador.Relatorios.Relatorio relatorioOrigem = repRelatorio.BuscarPorCodigo(dynRelatorio.Codigo);
            Dominio.Entidades.Embarcador.Relatorios.Relatorio relatorioTemporario = serRelatorio.ObterRelatorioTemporario(dynRelatorio, relatorioOrigem, tipoServico);
            relatorioTemporario.DataAlteracao = DateTime.Now;

            if (tipoServico == AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiCTe)
            {
                relatorioTemporario.Empresa = usuario.Empresa;
                relatorioTemporario.Padrao = false;
            }

            relatorioTemporario.Usuario = usuario;

            repRelatorio.Inserir(relatorioTemporario);

            gridRelatorio.SetarRelatorioPelaGrid(dynRelatorio.Grid, relatorioTemporario);

            foreach (Dominio.Entidades.Embarcador.Relatorios.RelatorioColuna coluna in relatorioTemporario.Colunas)
                repRelatorioColuna.Inserir(coluna);

            return relatorioTemporario.Codigo;
        }
    }
}