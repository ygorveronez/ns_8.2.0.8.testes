using Dominio.ObjetosDeValor.Embarcador.Enumeradores;
using Dominio.ObjetosDeValor.Enumerador;
using System;

namespace Dominio.Relatorios.Embarcador.DataSource.PedidosVendas
{
    public class RelatorioVendaDireta
    {
        public int Codigo { get; set; }
        public double CNPJCPFPessoa { get; set; }
        public DateTime DataNascimento { get; set; }
        public string RazaoPessoa { get; set; }
        public string TipoPessoa { get; set; }
        public string Email { get; set; }
        public string RG { get; set; }
        public string OrgaoEmissor { get; set; }
        public string UF { get; set; }
        public string Telefone { get; set; }
        public string Profissao { get; set; }
        public string TituloEleitoral { get; set; }
        public string ZonaEleitoral { get; set; }
        public string SecaoEleitoral { get; set; }
        public string Cidade { get; set; }
        public string Estado { get; set; }
        public string PisPasep { get; set; }
        public string NumeroCEI { get; set; }

        public string RazaoEmpresa { get; set; }
        public double CNPJCPFEmpresa { get; set; }
        public string TipoEmpresa { get; set; }
        public string NumeroCEIEmpresa { get; set; }
        public string TelefoneEmpresa { get; set; }
        public string CidadeEmpresa { get; set; }
        public string UFEmpresa { get; set; }

        public decimal ValorTotal { get; set; }
        public DateTime DataAgendamento { get; set; }
        public DateTime DataVecimentoCertificado { get; set; }
        public DateTime DataVecimentoCobranca { get; set; }
        public string NumeroPedido { get; set; }
        public Dominio.ObjetosDeValor.Embarcador.Enumeradores.StatusPedidoVendaDireta StatusPedido { get; set; }
        public string CodigoEmissao1 { get; set; }
        public string CodigoEmissao2 { get; set; }
        public Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoAssinaturaVendaDireta TipoAssinatura { get; set; }
        public bool NecessarioGerarNF { get; set; }
        public string Observacao { get; set; }
        public Dominio.ObjetosDeValor.Embarcador.Enumeradores.StatusVendaDireta StatusVenda { get; set; }
        public string GrupoPessoa { get; set; }
        public string Produtos { get; set; }
        public string Servicos { get; set; }

        public Dominio.ObjetosDeValor.Embarcador.Enumeradores.ProdutoServico ProdutoServico { get; set; }
        public DateTime DataTreinamento { get; set; }
        public string FuncionarioTreinamento { get; set; }
        public Dominio.ObjetosDeValor.Embarcador.Enumeradores.StatusCadastro StatusCadastro { get; set; }
        public Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoClienteVendaDireta TipoClienteVendaDireta { get; set; }
        public Dominio.ObjetosDeValor.Embarcador.Enumeradores.SimNao EmitidoDocumentos { get; set; }
        public Dominio.ObjetosDeValor.Embarcador.Enumeradores.SimNao Pendencia { get; set; }
        public bool Certificado { get; set; }


        public int Numero { get; set; }
        public DateTime Data { get; set; }
        public DateTime DataFinalizacao { get; set; }
        public DateTime DataCancelamento { get; set; }
        public DateTime DataValidacao { get; set; }
        public DateTime DataContestacao { get; set; }
        public string Funcionario { get; set; }
        public string FuncionarioAgendador { get; set; }
        public string FuncionarioContestacao { get; set; }
        public string ObservacaoContestacao { get; set; }

        public string NumeroBoleto { get; set; }
        public string StatusTitulo { get; set; }

        public string FuncionarioValidador { get; set; }
        public DateTime DataAgendadoFora { get; set; }
        public DateTime DataAprovado { get; set; }
        public DateTime DataBaixado { get; set; }
        public DateTime DataFaltaAgendar { get; set; }
        public DateTime DataAgendado { get; set; }
        public DateTime DataContato1 { get; set; }
        public DateTime DataContato2 { get; set; }
        public DateTime DataContato3 { get; set; }
        public DateTime DataProblema { get; set; }
        public DateTime DataReagendar { get; set; }
        public DateTime DataClienteBaixa { get; set; }
        public DateTime DataAguardandoVerificacao { get; set; }

        public string DataNascimentoFormatada
        {
            get
            {
                if (this.DataNascimento == DateTime.MinValue)
                    return "";
                else
                    return this.DataNascimento.ToString("dd/MM/yyyy");
            }
        }

        public string DataAgendadoForaFormatado
        {
            get
            {
                if (this.DataAgendadoFora == DateTime.MinValue)
                    return "";
                else
                    return this.DataAgendadoFora.ToString("dd/MM/yyyy HH:mm");
            }
        }

        public string DataAprovadoFormatado
        {
            get
            {
                if (this.DataAprovado == DateTime.MinValue)
                    return "";
                else
                    return this.DataAprovado.ToString("dd/MM/yyyy HH:mm");
            }
        }

        public string DataBaixadoFormatado
        {
            get
            {
                if (this.DataBaixado == DateTime.MinValue)
                    return "";
                else
                    return this.DataBaixado.ToString("dd/MM/yyyy HH:mm");
            }
        }

        public string DataFaltaAgendarFormatado
        {
            get
            {
                if (this.DataFaltaAgendar == DateTime.MinValue)
                    return "";
                else
                    return this.DataFaltaAgendar.ToString("dd/MM/yyyy HH:mm");
            }
        }

        public string DataAgendadoFormatado
        {
            get
            {
                if (this.DataAgendado == DateTime.MinValue)
                    return "";
                else
                    return this.DataAgendado.ToString("dd/MM/yyyy HH:mm");
            }
        }

        public string DataContato1Formatado
        {
            get
            {
                if (this.DataContato1 == DateTime.MinValue)
                    return "";
                else
                    return this.DataContato1.ToString("dd/MM/yyyy HH:mm");
            }
        }

        public string DataContato2Formatado
        {
            get
            {
                if (this.DataContato2 == DateTime.MinValue)
                    return "";
                else
                    return this.DataContato2.ToString("dd/MM/yyyy HH:mm");
            }
        }

        public string DataContato3Formatado
        {
            get
            {
                if (this.DataContato3 == DateTime.MinValue)
                    return "";
                else
                    return this.DataContato3.ToString("dd/MM/yyyy HH:mm");
            }
        }

        public string DataProblemaFormatado
        {
            get
            {
                if (this.DataProblema == DateTime.MinValue)
                    return "";
                else
                    return this.DataProblema.ToString("dd/MM/yyyy HH:mm");
            }
        }

        public string DataReagendarFormatado
        {
            get
            {
                if (this.DataReagendar == DateTime.MinValue)
                    return "";
                else
                    return this.DataReagendar.ToString("dd/MM/yyyy HH:mm");
            }
        }

        public string DataClienteBaixaFormatado
        {
            get
            {
                if (this.DataClienteBaixa == DateTime.MinValue)
                    return "";
                else
                    return this.DataClienteBaixa.ToString("dd/MM/yyyy HH:mm");
            }
        }

        public string DataAguardandoVerificacaoFormatado
        {
            get
            {
                if (this.DataAguardandoVerificacao == DateTime.MinValue)
                    return "";
                else
                    return this.DataAguardandoVerificacao.ToString("dd/MM/yyyy HH:mm");
            }
        }

        public string DataContestacaoFormatado
        {
            get
            {
                if (this.DataContestacao == DateTime.MinValue)
                    return "";
                else
                    return this.DataContestacao.ToString("dd/MM/yyyy");
            }
        }

        public string DataValidacaoFormatado
        {
            get
            {
                if (this.DataValidacao == DateTime.MinValue)
                    return "";
                else
                    return this.DataValidacao.ToString("dd/MM/yyyy");
            }
        }

        public string DataCancelamentoFormatado
        {
            get
            {
                if (this.DataCancelamento == DateTime.MinValue)
                    return "";
                else
                    return this.DataCancelamento.ToString("dd/MM/yyyy");
            }
        }

        public string DataFinalizacaoFormatado
        {
            get
            {
                if (this.DataFinalizacao == DateTime.MinValue)
                    return "";
                else
                    return this.DataFinalizacao.ToString("dd/MM/yyyy");
            }
        }

        public string DataFormatado
        {
            get
            {
                if (this.Data == DateTime.MinValue)
                    return "";
                else
                    return this.Data.ToString("dd/MM/yyyy");
            }
        }

        public string ProdutoServicoFormatado
        {
            get
            {
                return this.ProdutoServico.ObterDescricao();
            }
        }
        public string DataTreinamentoFormatado
        {
            get
            {
                if (this.DataTreinamento == DateTime.MinValue)
                    return "";
                else
                    return this.DataTreinamento.ToString("dd/MM/yyyy");
            }
        }

        public string StatusCadastroFormatado
        {
            get
            {
                return this.StatusCadastro.ObterDescricao();
            }
        }
        public string TipoClienteVendaDiretaFormatado
        {
            get
            {
                return this.TipoClienteVendaDireta.ObterDescricao();
            }
        }
        public string EmitidoDocumentosFormatado
        {
            get
            {
                return this.EmitidoDocumentos.ObterDescricao();
            }
        }
        public string PendenciaFormatado
        {
            get
            {
                return this.Pendencia.ObterDescricao();
            }
        }
        public string CertificadoFormatado
        {
            get
            {
                if (this.Certificado)
                    return "Sim";
                else
                    return "Não";
            }
        }

        public string NecessarioGerarNFFormatado
        {
            get
            {
                if (this.NecessarioGerarNF)
                    return "Sim";
                else
                    return "Não";
            }
        }

        public string StatusVendaFormatado
        {
            get
            {
                return Dominio.ObjetosDeValor.Embarcador.Enumeradores.StatusVendaDiretaHelper.ObterDescricao(this.StatusVenda);
            }
        }

        public string StatusPedidoFormatado
        {
            get
            {
                return Dominio.ObjetosDeValor.Embarcador.Enumeradores.StatusPedidoVendaDiretaHelper.ObterDescricao(this.StatusPedido);
            }
        }

        public string TipoAssinaturaFormatado
        {
            get
            {
                return Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoAssinaturaVendaDiretaHelper.ObterDescricao(this.TipoAssinatura);
            }
        }


        public string DataVecimentoCobrancaFormatado
        {
            get
            {
                if (this.DataVecimentoCobranca == DateTime.MinValue)
                    return "";
                else
                    return this.DataVecimentoCobranca.ToString("dd/MM/yyyy");
            }
        }

        public string DataAgendamentoFormatado
        {
            get
            {
                if (this.DataAgendamento == DateTime.MinValue)
                    return "";
                else
                    return this.DataAgendamento.ToString("dd/MM/yyyy");
            }
        }

        public string DataVecimentoCertificadoFormatado
        {
            get
            {
                if (this.DataVecimentoCertificado == DateTime.MinValue)
                    return "";
                else
                    return this.DataVecimentoCertificado.ToString("dd/MM/yyyy");
            }
        }

        public string CPFCNPJPessoaFormatado
        {
            get
            {
                if (TipoPessoa == "E")
                    return "00.000.000/0000-00";
                else
                    return TipoPessoa == "J" ? string.Format(@"{0:00\.000\.000\/0000\-00}", CNPJCPFPessoa) : string.Format(@"{0:000\.000\.000\-00}", CNPJCPFPessoa);
            }
        }

        public string CPFCNPJEmpresaFormatado
        {
            get
            {
                if (TipoEmpresa == "E")
                    return "00.000.000/0000-00";
                else
                    return TipoEmpresa == "J" ? string.Format(@"{0:00\.000\.000\/0000\-00}", CNPJCPFEmpresa) : string.Format(@"{0:000\.000\.000\-00}", CNPJCPFEmpresa);
            }
        }

        public string OrgaoEmissorFormatado
        {
            get
            {
                OrgaoEmissorRG orgaoEmissorRG;
                return OrgaoEmissorRGHelper.ObterDescricao(Enum.TryParse(this.OrgaoEmissor, out orgaoEmissorRG) ? orgaoEmissorRG : OrgaoEmissorRG.Nenhum);
            }
        }
    }
}
