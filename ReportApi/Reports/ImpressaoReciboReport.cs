using System.Collections.Generic;
using System.Linq;
using Dominio.ObjetosDeValor.Embarcador.Enumeradores;
using Dominio.ObjetosDeValor.Relatorios;
using ReportApi.Attributes;
using ReportApi.DTO;
using ReportApi.Extensions;
using ReportApi.ReportService;
using ReportApi.Storage;
using Repositorio;
using Servicos.Embarcador.Relatorios;

namespace ReportApi.Reports;

[UseReportType(ReportType.ImpressaoRecibo)]
public class ImpressaoReciboReport : ReportBase
{
    public ImpressaoReciboReport(UnitOfWork unitOfWork, RelatorioReportService servicoRelatorioReportService, IStorage storage) : base(unitOfWork, servicoRelatorioReportService, storage)
    {
    }

    public override ReportResult InternalProcess(Dictionary<string, string> extraData)
    {
        Repositorio.Embarcador.Acerto.AcertoViagem repAcertoViagem = new Repositorio.Embarcador.Acerto.AcertoViagem(_unitOfWork);
        Repositorio.Embarcador.Acerto.AcertoVeiculoResultado repAcertoVeiculoResultado = new Repositorio.Embarcador.Acerto.AcertoVeiculoResultado(_unitOfWork);
        Repositorio.Embarcador.Acerto.AcertoAdiantamento repAcertoAdiantamento = new Repositorio.Embarcador.Acerto.AcertoAdiantamento(_unitOfWork);
        Repositorio.Embarcador.Acerto.AcertoDevolucaoMoedaEstrangeira repAcertoDevolucaoMoedaEstrangeira = new Repositorio.Embarcador.Acerto.AcertoDevolucaoMoedaEstrangeira(_unitOfWork);
        Repositorio.Embarcador.Acerto.AcertoVariacaoCambial repAcertoVariacaoCambial = new Repositorio.Embarcador.Acerto.AcertoVariacaoCambial(_unitOfWork);
        Repositorio.Embarcador.Configuracoes.ConfiguraoAcertoViagem repConfiguraoAcertoViagem = new Repositorio.Embarcador.Configuracoes.ConfiguraoAcertoViagem(_unitOfWork);

        var info = extraData.GetInfo();
        var acerto = repAcertoViagem.BuscarPorCodigo(extraData.GetValue<int>("CodigoAcerto"));
        var usuario = BuscarUsuario(extraData.GetValue<int>("CodigoUsuario"));
        var empresa = BuscarEmpresa(extraData.GetValue<int>("CodigoEmpresa"));

        Dominio.Entidades.Embarcador.Configuracoes.ConfiguraoAcertoViagem configuraoAcertoViagem =
            repConfiguraoAcertoViagem.BuscarConfiguracaoPadrao();

        List<Dominio.Entidades.Embarcador.Acerto.AcertoVeiculoResultado> resultado =
            repAcertoVeiculoResultado.BuscarPorAcerto(acerto.Codigo);
        List<Dominio.Entidades.Embarcador.Acerto.AcertoAdiantamento> adiantamentos =
            repAcertoAdiantamento.BuscarPorAcerto(acerto.Codigo);
        List<Dominio.Entidades.Embarcador.Acerto.AcertoDevolucaoMoedaEstrangeira> devolucoes =
            repAcertoDevolucaoMoedaEstrangeira.BuscarPorAcerto(acerto.Codigo);
        List<Dominio.Entidades.Embarcador.Acerto.AcertoVariacaoCambial> variacoes =
            repAcertoVariacaoCambial.BuscarPorAcerto(acerto.Codigo);

        List<Dominio.Relatorios.Embarcador.DataSource.AcertoViagem.ReciboAcertoViagem> dsReciboAcertoViagem =
            new List<Dominio.Relatorios.Embarcador.DataSource.AcertoViagem.ReciboAcertoViagem>();
        List<Dominio.Relatorios.Embarcador.DataSource.AcertoViagem.ReciboAcertoViagem> dsReciboAcertoViagemDespesas =
            new List<Dominio.Relatorios.Embarcador.DataSource.AcertoViagem.ReciboAcertoViagem>();
        List<Dominio.Relatorios.Embarcador.DataSource.AcertoViagem.ReciboAcertoViagem> dsReciboAcertoViagemReceitas =
            new List<Dominio.Relatorios.Embarcador.DataSource.AcertoViagem.ReciboAcertoViagem>();

        List<Dominio.Relatorios.Embarcador.DataSource.AcertoViagem.ReciboAcertoViagem> via1 =
            new List<Dominio.Relatorios.Embarcador.DataSource.AcertoViagem.ReciboAcertoViagem>();
        List<Dominio.Relatorios.Embarcador.DataSource.AcertoViagem.ReciboAcertoViagem> via2 =
            new List<Dominio.Relatorios.Embarcador.DataSource.AcertoViagem.ReciboAcertoViagem>();
        List<Dominio.Entidades.Embarcador.Acerto.AcertoAbastecimento> abastecimentos = acerto.Abastecimentos != null
            ? (from obj in acerto.Abastecimentos
                where obj.Abastecimento.Posto.Modalidades.Any(o =>
                    o.TipoModalidade == Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoModalidade.Fornecedor &&
                    o.ModalidadesFornecedores.Any(p => p.PagoPorFatura == false))
                select obj).ToList()
            : null;
        List<Dominio.Entidades.Embarcador.Acerto.AcertoPedagio> pedagios = acerto.Pedagios != null
            ? (from obj in acerto.Pedagios
                where obj.Pedagio.TipoPedagio == Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoPedagio.Debito &&
                      obj.Pedagio.ImportadoDeSemParar == false
                select obj).ToList()
            : null;
        List<Dominio.Entidades.Embarcador.Acerto.AcertoOutraDespesa> outrasDespesas = null;
        if (ConfiguracaoEmbarcador.GerarReciboAcertoViagemDetalhado)
            outrasDespesas = acerto.OutrasDespesas != null
                ? (from obj in acerto.OutrasDespesas select obj).ToList()
                : null;
        else
            outrasDespesas = acerto.OutrasDespesas != null
                ? (from obj in acerto.OutrasDespesas
                    where obj.Pessoa.Modalidades.Any(o =>
                        o.TipoModalidade == Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoModalidade.Fornecedor &&
                        o.ModalidadesFornecedores.Any(p => p.PagoPorFatura == false))
                    select obj).ToList()
                : null;
        decimal adiantamento = resultado.Select(o => o.AdiantamentoMotorista).LastOrDefault();

        string numerosFrotas = acerto.Veiculos != null
            ? string.Join(", ", (from p in acerto.Veiculos select p.Veiculo.NumeroFrota))
            : string.Empty;
        string placas = acerto.Veiculos != null
            ? string.Join(", ", (from p in acerto.Veiculos select p.Veiculo.Placa))
            : string.Empty;
        List<Dominio.Entidades.Embarcador.Acerto.AcertoDiaria> diarias =
            acerto.Diarias != null ? (from obj in acerto.Diarias select obj).ToList() : null;
        List<Dominio.Entidades.Embarcador.Acerto.AcertoBonificacao> bonificacoes =
            acerto.Bonificacoes != null ? (from obj in acerto.Bonificacoes select obj).ToList() : null;
        List<Dominio.Entidades.Embarcador.Acerto.AcertoDesconto> descontos =
            acerto.Descontos != null ? (from obj in acerto.Descontos select obj).ToList() : null;

        decimal saldoMotorista = 0;
        if (adiantamento > 0)
            saldoMotorista -= adiantamento;
        foreach (var abastecimento in abastecimentos)
            saldoMotorista += abastecimento.Abastecimento.ValorTotal;
        foreach (var pedagio in pedagios)
            saldoMotorista += pedagio.Pedagio.Valor;
        foreach (var outra in outrasDespesas)
        {
            if (ConfiguracaoEmbarcador.GerarReciboAcertoViagemDetalhado || outra.Pessoa.Modalidades.Any(o =>
                    o.TipoModalidade == Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoModalidade.Fornecedor &&
                    o.ModalidadesFornecedores.Any(p => p.PagoPorFatura == false)) ||
                outra.DespesaPagaPeloAdiantamento.HasValue == true)
                saldoMotorista += outra.Quantidade > 0 ? (outra.Quantidade * outra.Valor) : outra.Valor;
        }

        if (diarias != null && diarias.Count > 0)
            saldoMotorista += diarias.Sum(o => o.Valor);
        foreach (var bonificacao in bonificacoes)
            saldoMotorista += bonificacao.ValorBonificacao;
        foreach (var desconto in descontos)
            saldoMotorista -= desconto.ValorDesconto;
        foreach (var devolucao in devolucoes)
            saldoMotorista += devolucao.ValorOriginal;
        foreach (var variacao in variacoes)
            saldoMotorista += variacao.ValorOriginal;

        if (acerto.VariacaoCambial > 0)
            saldoMotorista += acerto.VariacaoCambial;
        if (acerto.VariacaoCambialReceita > 0)
            saldoMotorista -= acerto.VariacaoCambialReceita;

        //if (ConfiguracaoEmbarcador.GerarReciboDetalhadoAcertoViagem)
        //    saldoMotorista = saldoMotorista * -1;

        string valorExtenso = "";
        if (saldoMotorista < 0)
            valorExtenso = Utilidades.Conversor.DecimalToWords(saldoMotorista * -1);
        else
            valorExtenso = Utilidades.Conversor.DecimalToWords(saldoMotorista);
        if (adiantamento > 0)
        {
            if (ConfiguracaoEmbarcador.GerarReciboDetalhadoAcertoViagem && adiantamentos != null &&
                adiantamentos.Count > 0)
            {
                foreach (var adi in adiantamentos)
                {
                    Dominio.Relatorios.Embarcador.DataSource.AcertoViagem.ReciboAcertoViagem recibo =
                        new Dominio.Relatorios.Embarcador.DataSource.AcertoViagem.ReciboAcertoViagem();
                    recibo.ObservacaoFixa = configuraoAcertoViagem?.TextoRecibo ?? "";
                    recibo.DataFechamentoAcerto = acerto.DataFechamento.Value;
                    recibo.DataFimAcerto = acerto.DataFinal.Value;
                    recibo.DataInicioAcerto = acerto.DataInicial;
                    recibo.DescricaoDespesa = adi.PagamentoMotoristaTMS.DataPagamento.ToString("dd/MM/yyyy") + " " +
                                              (adi.PagamentoMotoristaTMS.PagamentoMotoristaTipo?.Descricao ?? "") +
                                              " " + adi.PagamentoMotoristaTMS.Observacao;
                    recibo.ValorDespesa = (adi.PagamentoMotoristaTMS.Valor * -1);
                    recibo.ValorExtenso = valorExtenso;
                    recibo.ValorTotal = saldoMotorista;
                    recibo.FrotaVeiculos = numerosFrotas;
                    recibo.Motorista = acerto.Motorista.Nome;
                    recibo.NumeroAcerto = acerto.Numero;
                    recibo.NumeroRecibo = 1;
                    recibo.Operador = usuario.Nome;
                    recibo.Proprietario =
                        acerto.Motorista.TipoMotorista ==
                        Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoMotorista.Proprio
                            ?
                            (empresa?.RazaoSocial ?? "")
                            : acerto.Motorista.Empresa != null
                                ? acerto.Motorista.Empresa.RazaoSocial
                                : string.Empty;
                    recibo.Veiculos = placas;
                    if (!ConfiguracaoEmbarcador.GerarReciboDetalhadoAcertoViagem)
                        via1.Add(recibo);

                    Dominio.Relatorios.Embarcador.DataSource.AcertoViagem.ReciboAcertoViagem recibo2 =
                        (Dominio.Relatorios.Embarcador.DataSource.AcertoViagem.ReciboAcertoViagem)recibo.Clone();
                    recibo2.NumeroRecibo = 2;
                    if (!ConfiguracaoEmbarcador.GerarReciboDetalhadoAcertoViagem)
                        via2.Add(recibo2);

                    if (ConfiguracaoEmbarcador.GerarReciboDetalhadoAcertoViagem)
                    {
                        dsReciboAcertoViagemDespesas.Add(recibo);
                        dsReciboAcertoViagemDespesas.Add(recibo2);
                    }
                }
            }
            else
            {
                Dominio.Relatorios.Embarcador.DataSource.AcertoViagem.ReciboAcertoViagem recibo =
                    new Dominio.Relatorios.Embarcador.DataSource.AcertoViagem.ReciboAcertoViagem();
                recibo.ObservacaoFixa = configuraoAcertoViagem?.TextoRecibo ?? "";
                recibo.DataFechamentoAcerto = acerto.DataFechamento.Value;
                recibo.DataFimAcerto = acerto.DataFinal.Value;
                recibo.DataInicioAcerto = acerto.DataInicial;
                recibo.DescricaoDespesa = acerto.DataFechamento.Value.ToString("dd/MM/yyyy") + " Adiantamento recebido";
                recibo.ValorDespesa = adiantamento * -1;
                recibo.ValorExtenso = valorExtenso;
                recibo.ValorTotal = saldoMotorista;
                recibo.FrotaVeiculos = numerosFrotas;
                recibo.Motorista = acerto.Motorista.Nome;
                recibo.NumeroAcerto = acerto.Numero;
                recibo.NumeroRecibo = 1;
                recibo.Operador = usuario.Nome;
                recibo.Proprietario =
                    acerto.Motorista.TipoMotorista ==
                    Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoMotorista.Proprio
                        ?
                        (empresa?.RazaoSocial ?? "")
                        : acerto.Motorista.Empresa != null
                            ? acerto.Motorista.Empresa.RazaoSocial
                            : string.Empty;
                recibo.Veiculos = placas;
                if (!ConfiguracaoEmbarcador.GerarReciboDetalhadoAcertoViagem)
                    via1.Add(recibo);

                Dominio.Relatorios.Embarcador.DataSource.AcertoViagem.ReciboAcertoViagem recibo2 =
                    (Dominio.Relatorios.Embarcador.DataSource.AcertoViagem.ReciboAcertoViagem)recibo.Clone();
                recibo2.NumeroRecibo = 2;
                if (!ConfiguracaoEmbarcador.GerarReciboDetalhadoAcertoViagem)
                    via2.Add(recibo2);

                if (ConfiguracaoEmbarcador.GerarReciboDetalhadoAcertoViagem)
                {
                    dsReciboAcertoViagemDespesas.Add(recibo);
                    dsReciboAcertoViagemDespesas.Add(recibo2);
                }
            }
        }

        foreach (var devolucao in devolucoes)
        {
            Dominio.Relatorios.Embarcador.DataSource.AcertoViagem.ReciboAcertoViagem recibo =
                new Dominio.Relatorios.Embarcador.DataSource.AcertoViagem.ReciboAcertoViagem();
            recibo.ObservacaoFixa = configuraoAcertoViagem?.TextoRecibo ?? "";
            recibo.DataFechamentoAcerto = acerto.DataFechamento.Value;
            recibo.DataFimAcerto = acerto.DataFinal.Value;
            recibo.DataInicioAcerto = acerto.DataInicial;
            recibo.DescricaoDespesa =
                (devolucao.DataBaseCRT.HasValue
                    ? devolucao.DataBaseCRT.Value.ToString("dd/MM/yyyy")
                    : acerto.DataFechamento.Value.ToString("dd/MM/yyyy")) + " Devolução " +
                devolucao.MoedaCotacaoBancoCentral.Value.ObterDescricao();
            recibo.ValorDespesa = devolucao.ValorOriginal;
            recibo.ValorExtenso = valorExtenso;
            recibo.ValorTotal = saldoMotorista;
            recibo.FrotaVeiculos = numerosFrotas;
            recibo.Motorista = acerto.Motorista.Nome;
            recibo.NumeroAcerto = acerto.Numero;
            recibo.NumeroRecibo = 1;
            recibo.Operador = usuario.Nome;
            recibo.Proprietario =
                acerto.Motorista.TipoMotorista == Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoMotorista.Proprio
                    ?
                    (empresa?.RazaoSocial ?? "")
                    : acerto.Motorista.Empresa != null
                        ? acerto.Motorista.Empresa.RazaoSocial
                        : string.Empty;
            recibo.Veiculos = placas;
            if (!ConfiguracaoEmbarcador.GerarReciboDetalhadoAcertoViagem)
                via1.Add(recibo);

            Dominio.Relatorios.Embarcador.DataSource.AcertoViagem.ReciboAcertoViagem recibo2 =
                (Dominio.Relatorios.Embarcador.DataSource.AcertoViagem.ReciboAcertoViagem)recibo.Clone();
            recibo2.NumeroRecibo = 2;
            if (!ConfiguracaoEmbarcador.GerarReciboDetalhadoAcertoViagem)
                via2.Add(recibo2);

            if (ConfiguracaoEmbarcador.GerarReciboDetalhadoAcertoViagem)
            {
                dsReciboAcertoViagemReceitas.Add(recibo);
                dsReciboAcertoViagemReceitas.Add(recibo2);
            }
        }

        foreach (var variacao in variacoes)
        {
            Dominio.Relatorios.Embarcador.DataSource.AcertoViagem.ReciboAcertoViagem recibo =
                new Dominio.Relatorios.Embarcador.DataSource.AcertoViagem.ReciboAcertoViagem();
            recibo.ObservacaoFixa = configuraoAcertoViagem?.TextoRecibo ?? "";
            recibo.DataFechamentoAcerto = acerto.DataFechamento.Value;
            recibo.DataFimAcerto = acerto.DataFinal.Value;
            recibo.DataInicioAcerto = acerto.DataInicial;
            recibo.DescricaoDespesa = acerto.DataFechamento.Value.ToString("dd/MM/yyyy") + " Variação " +
                                      variacao.MoedaCotacaoBancoCentral.Value.ObterDescricao();
            recibo.ValorDespesa = variacao.ValorOriginal;
            recibo.ValorExtenso = valorExtenso;
            recibo.ValorTotal = saldoMotorista;
            recibo.FrotaVeiculos = numerosFrotas;
            recibo.Motorista = acerto.Motorista.Nome;
            recibo.NumeroAcerto = acerto.Numero;
            recibo.NumeroRecibo = 1;
            recibo.Operador = usuario.Nome;
            recibo.Proprietario =
                acerto.Motorista.TipoMotorista == Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoMotorista.Proprio
                    ?
                    (empresa?.RazaoSocial ?? "")
                    : acerto.Motorista.Empresa != null
                        ? acerto.Motorista.Empresa.RazaoSocial
                        : string.Empty;
            recibo.Veiculos = placas;
            if (!ConfiguracaoEmbarcador.GerarReciboDetalhadoAcertoViagem)
                via1.Add(recibo);

            Dominio.Relatorios.Embarcador.DataSource.AcertoViagem.ReciboAcertoViagem recibo2 =
                (Dominio.Relatorios.Embarcador.DataSource.AcertoViagem.ReciboAcertoViagem)recibo.Clone();
            recibo2.NumeroRecibo = 2;
            if (!ConfiguracaoEmbarcador.GerarReciboDetalhadoAcertoViagem)
                via2.Add(recibo2);

            if (ConfiguracaoEmbarcador.GerarReciboDetalhadoAcertoViagem)
            {
                dsReciboAcertoViagemReceitas.Add(recibo);
                dsReciboAcertoViagemReceitas.Add(recibo2);
            }
        }

        if (acerto.VariacaoCambial > 0)
        {
            Dominio.Relatorios.Embarcador.DataSource.AcertoViagem.ReciboAcertoViagem recibo =
                new Dominio.Relatorios.Embarcador.DataSource.AcertoViagem.ReciboAcertoViagem();
            recibo.ObservacaoFixa = configuraoAcertoViagem?.TextoRecibo ?? "";
            recibo.DataFechamentoAcerto = acerto.DataFechamento.Value;
            recibo.DataFimAcerto = acerto.DataFinal.Value;
            recibo.DataInicioAcerto = acerto.DataInicial;
            recibo.DescricaoDespesa = acerto.DataFechamento.Value.ToString("dd/MM/yyyy") + " Variação cambial";
            recibo.ValorDespesa = (acerto.VariacaoCambial);
            recibo.ValorExtenso = valorExtenso;
            recibo.ValorTotal = saldoMotorista;
            recibo.FrotaVeiculos = numerosFrotas;
            recibo.Motorista = acerto.Motorista.Nome;
            recibo.NumeroAcerto = acerto.Numero;
            recibo.NumeroRecibo = 1;
            recibo.Operador = usuario.Nome;
            recibo.Proprietario =
                acerto.Motorista.TipoMotorista == Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoMotorista.Proprio
                    ?
                    (empresa?.RazaoSocial ?? "")
                    : acerto.Motorista.Empresa != null
                        ? acerto.Motorista.Empresa.RazaoSocial
                        : string.Empty;
            recibo.Veiculos = placas;
            if (!ConfiguracaoEmbarcador.GerarReciboDetalhadoAcertoViagem)
                via1.Add(recibo);

            Dominio.Relatorios.Embarcador.DataSource.AcertoViagem.ReciboAcertoViagem recibo2 =
                (Dominio.Relatorios.Embarcador.DataSource.AcertoViagem.ReciboAcertoViagem)recibo.Clone();
            recibo2.NumeroRecibo = 2;
            if (!ConfiguracaoEmbarcador.GerarReciboDetalhadoAcertoViagem)
                via2.Add(recibo2);

            if (ConfiguracaoEmbarcador.GerarReciboDetalhadoAcertoViagem)
            {
                dsReciboAcertoViagemReceitas.Add(recibo);
                dsReciboAcertoViagemReceitas.Add(recibo2);
            }
        }

        if (acerto.VariacaoCambialReceita > 0)
        {
            Dominio.Relatorios.Embarcador.DataSource.AcertoViagem.ReciboAcertoViagem recibo =
                new Dominio.Relatorios.Embarcador.DataSource.AcertoViagem.ReciboAcertoViagem();
            recibo.ObservacaoFixa = configuraoAcertoViagem?.TextoRecibo ?? "";
            recibo.DataFechamentoAcerto = acerto.DataFechamento.Value;
            recibo.DataFimAcerto = acerto.DataFinal.Value;
            recibo.DataInicioAcerto = acerto.DataInicial;
            recibo.DescricaoDespesa = acerto.DataFechamento.Value.ToString("dd/MM/yyyy") + " Variação cambial";
            recibo.ValorDespesa = (acerto.VariacaoCambialReceita * -1);
            recibo.ValorExtenso = valorExtenso;
            recibo.ValorTotal = saldoMotorista;
            recibo.FrotaVeiculos = numerosFrotas;
            recibo.Motorista = acerto.Motorista.Nome;
            recibo.NumeroAcerto = acerto.Numero;
            recibo.NumeroRecibo = 1;
            recibo.Operador = usuario.Nome;
            recibo.Proprietario =
                acerto.Motorista.TipoMotorista == Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoMotorista.Proprio
                    ?
                    (empresa?.RazaoSocial ?? "")
                    : acerto.Motorista.Empresa != null
                        ? acerto.Motorista.Empresa.RazaoSocial
                        : string.Empty;
            recibo.Veiculos = placas;
            if (!ConfiguracaoEmbarcador.GerarReciboDetalhadoAcertoViagem)
                via1.Add(recibo);

            Dominio.Relatorios.Embarcador.DataSource.AcertoViagem.ReciboAcertoViagem recibo2 =
                (Dominio.Relatorios.Embarcador.DataSource.AcertoViagem.ReciboAcertoViagem)recibo.Clone();
            recibo2.NumeroRecibo = 2;
            if (!ConfiguracaoEmbarcador.GerarReciboDetalhadoAcertoViagem)
                via2.Add(recibo2);

            if (ConfiguracaoEmbarcador.GerarReciboDetalhadoAcertoViagem)
            {
                dsReciboAcertoViagemDespesas.Add(recibo);
                dsReciboAcertoViagemDespesas.Add(recibo2);
            }
        }

        foreach (var abastecimento in abastecimentos)
        {
            Dominio.Relatorios.Embarcador.DataSource.AcertoViagem.ReciboAcertoViagem recibo =
                new Dominio.Relatorios.Embarcador.DataSource.AcertoViagem.ReciboAcertoViagem();
            recibo.ObservacaoFixa = configuraoAcertoViagem?.TextoRecibo ?? "";
            recibo.DataFechamentoAcerto = acerto.DataFechamento.Value;
            recibo.DataFimAcerto = acerto.DataFinal.Value;
            recibo.DataInicioAcerto = acerto.DataInicial;
            recibo.DescricaoDespesa = abastecimento.Abastecimento.Data.Value.ToString("dd/MM/yyyy") +
                                      " Abastecimento " + abastecimento.Abastecimento.Posto.Nome;
            recibo.ValorDespesa = abastecimento.Abastecimento.ValorTotal;
            recibo.ValorExtenso = valorExtenso;
            recibo.ValorTotal = saldoMotorista;
            recibo.FrotaVeiculos = numerosFrotas;
            recibo.Motorista = acerto.Motorista.Nome;
            recibo.NumeroAcerto = acerto.Numero;
            recibo.NumeroRecibo = 1;
            recibo.Operador = usuario.Nome;
            recibo.Proprietario =
                acerto.Motorista.TipoMotorista == Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoMotorista.Proprio
                    ?
                    (empresa?.RazaoSocial ?? "")
                    : acerto.Motorista.Empresa != null
                        ? acerto.Motorista.Empresa.RazaoSocial
                        : string.Empty;
            recibo.Veiculos = placas;
            via1.Add(recibo);

            Dominio.Relatorios.Embarcador.DataSource.AcertoViagem.ReciboAcertoViagem recibo2 =
                (Dominio.Relatorios.Embarcador.DataSource.AcertoViagem.ReciboAcertoViagem)recibo.Clone();
            recibo2.NumeroRecibo = 2;
            via2.Add(recibo2);
        }

        foreach (var pedagio in pedagios)
        {
            Dominio.Relatorios.Embarcador.DataSource.AcertoViagem.ReciboAcertoViagem recibo =
                new Dominio.Relatorios.Embarcador.DataSource.AcertoViagem.ReciboAcertoViagem();
            recibo.ObservacaoFixa = configuraoAcertoViagem?.TextoRecibo ?? "";
            recibo.DataFechamentoAcerto = acerto.DataFechamento.Value;
            recibo.DataFimAcerto = acerto.DataFinal.Value;
            recibo.DataInicioAcerto = acerto.DataInicial;
            recibo.DescricaoDespesa =
                pedagio.Pedagio.Data.ToString("dd/MM/yyyy") + " Pedágio " + pedagio.Pedagio.Rodovia;
            recibo.ValorDespesa = pedagio.Pedagio.Valor;
            recibo.ValorExtenso = valorExtenso;
            recibo.ValorTotal = saldoMotorista;
            recibo.FrotaVeiculos = numerosFrotas;
            recibo.Motorista = acerto.Motorista.Nome;
            recibo.NumeroAcerto = acerto.Numero;
            recibo.NumeroRecibo = 1;
            recibo.Operador = usuario.Nome;
            recibo.Proprietario =
                acerto.Motorista.TipoMotorista == Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoMotorista.Proprio
                    ?
                    (empresa?.RazaoSocial ?? "")
                    : acerto.Motorista.Empresa != null
                        ? acerto.Motorista.Empresa.RazaoSocial
                        : string.Empty;
            recibo.Veiculos = placas;
            via1.Add(recibo);

            Dominio.Relatorios.Embarcador.DataSource.AcertoViagem.ReciboAcertoViagem recibo2 =
                (Dominio.Relatorios.Embarcador.DataSource.AcertoViagem.ReciboAcertoViagem)recibo.Clone();
            recibo2.NumeroRecibo = 2;
            via2.Add(recibo2);
        }

        foreach (var outra in outrasDespesas)
        {
            Dominio.Relatorios.Embarcador.DataSource.AcertoViagem.ReciboAcertoViagem recibo =
                new Dominio.Relatorios.Embarcador.DataSource.AcertoViagem.ReciboAcertoViagem();
            recibo.ObservacaoFixa = configuraoAcertoViagem?.TextoRecibo ?? "";
            recibo.DataFechamentoAcerto = acerto.DataFechamento.Value;
            recibo.DataFimAcerto = acerto.DataFinal.Value;
            recibo.DataInicioAcerto = acerto.DataInicial;
            recibo.DescricaoDespesa = outra.Data.ToString("dd/MM/yyyy") + " " + outra.Observacao;
            recibo.ValorDespesa = outra.Quantidade > 0 ? (outra.Quantidade * outra.Valor) : outra.Valor;
            recibo.ValorExtenso = valorExtenso;
            recibo.ValorTotal = saldoMotorista;
            recibo.FrotaVeiculos = numerosFrotas;
            recibo.Motorista = acerto.Motorista.Nome;
            recibo.NumeroAcerto = acerto.Numero;
            recibo.NumeroRecibo = 1;
            recibo.Operador = usuario.Nome;
            recibo.Proprietario =
                acerto.Motorista.TipoMotorista == Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoMotorista.Proprio
                    ?
                    (empresa?.RazaoSocial ?? "")
                    : acerto.Motorista.Empresa != null
                        ? acerto.Motorista.Empresa.RazaoSocial
                        : string.Empty;
            recibo.Veiculos = placas;
            via1.Add(recibo);

            Dominio.Relatorios.Embarcador.DataSource.AcertoViagem.ReciboAcertoViagem recibo2 =
                (Dominio.Relatorios.Embarcador.DataSource.AcertoViagem.ReciboAcertoViagem)recibo.Clone();
            recibo2.NumeroRecibo = 2;
            via2.Add(recibo2);
        }

        if (diarias != null && diarias.Count > 0)
        {
            Dominio.Relatorios.Embarcador.DataSource.AcertoViagem.ReciboAcertoViagem recibo =
                new Dominio.Relatorios.Embarcador.DataSource.AcertoViagem.ReciboAcertoViagem();
            recibo.ObservacaoFixa = configuraoAcertoViagem?.TextoRecibo ?? "";
            recibo.DataFechamentoAcerto = acerto.DataFechamento.Value;
            recibo.DataFimAcerto = acerto.DataFinal.Value;
            recibo.DataInicioAcerto = acerto.DataInicial;
            recibo.DescricaoDespesa = acerto.DataFechamento.Value.ToString("dd/MM/yyyy") + " " + "Total de Diárias";
            recibo.ValorDespesa = diarias.Sum(o => o.Valor);
            recibo.ValorExtenso = valorExtenso;
            recibo.ValorTotal = saldoMotorista;
            recibo.FrotaVeiculos = numerosFrotas;
            recibo.Motorista = acerto.Motorista.Nome;
            recibo.NumeroAcerto = acerto.Numero;
            recibo.NumeroRecibo = 1;
            recibo.Operador = usuario.Nome;
            recibo.Proprietario =
                acerto.Motorista.TipoMotorista == Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoMotorista.Proprio
                    ?
                    (empresa?.RazaoSocial ?? "")
                    : acerto.Motorista.Empresa != null
                        ? acerto.Motorista.Empresa.RazaoSocial
                        : string.Empty;
            recibo.Veiculos = placas;
            via1.Add(recibo);

            Dominio.Relatorios.Embarcador.DataSource.AcertoViagem.ReciboAcertoViagem recibo2 =
                (Dominio.Relatorios.Embarcador.DataSource.AcertoViagem.ReciboAcertoViagem)recibo.Clone();
            recibo2.NumeroRecibo = 2;
            via2.Add(recibo2);
        }

        if (bonificacoes != null && bonificacoes.Count > 0)
        {
            foreach (var bonificacao in bonificacoes)
            {
                Dominio.Relatorios.Embarcador.DataSource.AcertoViagem.ReciboAcertoViagem recibo =
                    new Dominio.Relatorios.Embarcador.DataSource.AcertoViagem.ReciboAcertoViagem();
                recibo.ObservacaoFixa = configuraoAcertoViagem?.TextoRecibo ?? "";
                recibo.DataFechamentoAcerto = acerto.DataFechamento.Value;
                recibo.DataFimAcerto = acerto.DataFinal.Value;
                recibo.DataInicioAcerto = acerto.DataInicial;
                recibo.DescricaoDespesa = bonificacao.Data.ToString("dd/MM/yyyy") + " " + bonificacao.Descricao;
                recibo.ValorDespesa = bonificacao.ValorBonificacao;
                recibo.ValorExtenso = valorExtenso;
                recibo.ValorTotal = saldoMotorista;
                recibo.FrotaVeiculos = numerosFrotas;
                recibo.Motorista = acerto.Motorista.Nome;
                recibo.NumeroAcerto = acerto.Numero;
                recibo.NumeroRecibo = 1;
                recibo.Operador = usuario.Nome;
                recibo.Proprietario =
                    acerto.Motorista.TipoMotorista ==
                    Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoMotorista.Proprio
                        ?
                        (empresa?.RazaoSocial ?? "")
                        : acerto.Motorista.Empresa != null
                            ? acerto.Motorista.Empresa.RazaoSocial
                            : string.Empty;
                recibo.Veiculos = placas;
                via1.Add(recibo);

                Dominio.Relatorios.Embarcador.DataSource.AcertoViagem.ReciboAcertoViagem recibo2 =
                    (Dominio.Relatorios.Embarcador.DataSource.AcertoViagem.ReciboAcertoViagem)recibo.Clone();
                recibo2.NumeroRecibo = 2;
                via2.Add(recibo2);
            }
        }

        foreach (var desconto in descontos)
        {
            Dominio.Relatorios.Embarcador.DataSource.AcertoViagem.ReciboAcertoViagem recibo =
                new Dominio.Relatorios.Embarcador.DataSource.AcertoViagem.ReciboAcertoViagem();
            recibo.ObservacaoFixa = configuraoAcertoViagem?.TextoRecibo ?? "";
            recibo.DataFechamentoAcerto = acerto.DataFechamento.Value;
            recibo.DataFimAcerto = acerto.DataFinal.Value;
            recibo.DataInicioAcerto = acerto.DataInicial;
            if (ConfiguracaoEmbarcador.GerarReciboDetalhadoAcertoViagem)
                recibo.DescricaoDespesa = desconto.Data.ToString("dd/MM/yyyy") + " " + desconto.Motivo;
            else
                recibo.DescricaoDespesa = desconto.Data.ToString("dd/MM/yyyy") + " " + desconto.Descricao;
            recibo.ValorDespesa = desconto.ValorDesconto * -1;
            recibo.ValorExtenso = valorExtenso;
            recibo.ValorTotal = saldoMotorista;
            recibo.FrotaVeiculos = numerosFrotas;
            recibo.Motorista = acerto.Motorista.Nome;
            recibo.NumeroAcerto = acerto.Numero;
            recibo.NumeroRecibo = 1;
            recibo.Operador = usuario.Nome;
            recibo.Proprietario =
                acerto.Motorista.TipoMotorista == Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoMotorista.Proprio
                    ?
                    (empresa?.RazaoSocial ?? "")
                    : acerto.Motorista.Empresa != null
                        ? acerto.Motorista.Empresa.RazaoSocial
                        : string.Empty;
            recibo.Veiculos = placas;
            if (!ConfiguracaoEmbarcador.GerarReciboDetalhadoAcertoViagem)
                via1.Add(recibo);

            Dominio.Relatorios.Embarcador.DataSource.AcertoViagem.ReciboAcertoViagem recibo2 =
                (Dominio.Relatorios.Embarcador.DataSource.AcertoViagem.ReciboAcertoViagem)recibo.Clone();
            recibo2.NumeroRecibo = 2;
            if (!ConfiguracaoEmbarcador.GerarReciboDetalhadoAcertoViagem)
                via2.Add(recibo2);

            if (ConfiguracaoEmbarcador.GerarReciboDetalhadoAcertoViagem)
            {
                dsReciboAcertoViagemDespesas.Add(recibo);
                dsReciboAcertoViagemDespesas.Add(recibo2);
            }
        }

        if (via1.Count <= 0)
        {
            Dominio.Relatorios.Embarcador.DataSource.AcertoViagem.ReciboAcertoViagem recibo =
                new Dominio.Relatorios.Embarcador.DataSource.AcertoViagem.ReciboAcertoViagem();
            recibo.ObservacaoFixa = configuraoAcertoViagem?.TextoRecibo ?? "";
            recibo.DataFechamentoAcerto = acerto.DataFechamento.Value;
            recibo.DataFimAcerto = acerto.DataFinal.Value;
            recibo.DataInicioAcerto = acerto.DataInicial;
            recibo.DescricaoDespesa = " ";
            recibo.ValorDespesa = 0;
            recibo.ValorExtenso = "SEM VALOR";
            recibo.ValorTotal = 0;
            recibo.FrotaVeiculos = numerosFrotas;
            recibo.Motorista = acerto.Motorista.Nome;
            recibo.NumeroAcerto = acerto.Numero;
            recibo.NumeroRecibo = 1;
            recibo.Operador = usuario.Nome;
            recibo.Proprietario =
                acerto.Motorista.TipoMotorista == Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoMotorista.Proprio
                    ?
                    (empresa?.RazaoSocial ?? "")
                    : acerto.Motorista.Empresa != null
                        ? acerto.Motorista.Empresa.RazaoSocial
                        : string.Empty;
            recibo.Veiculos = placas;
            via1.Add(recibo);

            Dominio.Relatorios.Embarcador.DataSource.AcertoViagem.ReciboAcertoViagem recibo2 =
                (Dominio.Relatorios.Embarcador.DataSource.AcertoViagem.ReciboAcertoViagem)recibo.Clone();
            recibo2.NumeroRecibo = 2;
            via2.Add(recibo2);
        }

        int qtdMaximoRegistros = 30;
        if (!string.IsNullOrWhiteSpace(configuraoAcertoViagem?.TextoRecibo))
            qtdMaximoRegistros = 25;
        if (ConfiguracaoEmbarcador.GerarReciboDetalhadoAcertoViagem)
        {
            qtdMaximoRegistros = 12;
            if (!string.IsNullOrWhiteSpace(configuraoAcertoViagem?.TextoRecibo))
                qtdMaximoRegistros = 10;
        }

        for (int i = via1.Count; i < qtdMaximoRegistros; i++)
        {
            Dominio.Relatorios.Embarcador.DataSource.AcertoViagem.ReciboAcertoViagem recibo =
                new Dominio.Relatorios.Embarcador.DataSource.AcertoViagem.ReciboAcertoViagem();
            recibo.ObservacaoFixa = configuraoAcertoViagem?.TextoRecibo ?? "";
            recibo.DataFechamentoAcerto = acerto.DataFechamento.Value;
            recibo.DataFimAcerto = acerto.DataFinal.Value;
            recibo.DataInicioAcerto = acerto.DataInicial;
            recibo.DescricaoDespesa = " ";
            recibo.ValorDespesa = 0;
            recibo.ValorExtenso = "SEM VALOR";
            recibo.ValorTotal = 0;
            recibo.FrotaVeiculos = numerosFrotas;
            recibo.Motorista = acerto.Motorista.Nome;
            recibo.NumeroAcerto = acerto.Numero;
            recibo.NumeroRecibo = 1;
            recibo.Operador = usuario.Nome;
            recibo.Proprietario =
                acerto.Motorista.TipoMotorista == Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoMotorista.Proprio
                    ?
                    (empresa?.RazaoSocial ?? "")
                    : acerto.Motorista.Empresa != null
                        ? acerto.Motorista.Empresa.RazaoSocial
                        : string.Empty;
            recibo.Veiculos = placas;
            via1.Add(recibo);

            Dominio.Relatorios.Embarcador.DataSource.AcertoViagem.ReciboAcertoViagem recibo2 =
                (Dominio.Relatorios.Embarcador.DataSource.AcertoViagem.ReciboAcertoViagem)recibo.Clone();
            recibo2.NumeroRecibo = 2;
            via2.Add(recibo2);

            dsReciboAcertoViagemDespesas.Add(recibo);
            dsReciboAcertoViagemDespesas.Add(recibo2);
        }

        if (!ConfiguracaoEmbarcador.GerarReciboDetalhadoAcertoViagem)
        {
            dsReciboAcertoViagem.AddRange(via1);
            dsReciboAcertoViagem.AddRange(via2);
        }
        else
        {
            Dominio.Relatorios.Embarcador.DataSource.AcertoViagem.ReciboAcertoViagem recibo =
                new Dominio.Relatorios.Embarcador.DataSource.AcertoViagem.ReciboAcertoViagem();
            recibo.ObservacaoFixa = configuraoAcertoViagem?.TextoRecibo ?? "";
            recibo.DataFechamentoAcerto = acerto.DataFechamento.Value;
            recibo.DataFimAcerto = acerto.DataFinal.Value;
            recibo.DataInicioAcerto = acerto.DataInicial;
            recibo.DescricaoDespesa = acerto.DataFechamento.Value.ToString("dd/MM/yyyy");
            recibo.ValorDespesa = saldoMotorista;
            recibo.ValorExtenso = valorExtenso;
            recibo.ValorTotal = saldoMotorista;
            recibo.FrotaVeiculos = numerosFrotas;
            recibo.Motorista = acerto.Motorista.Nome;
            recibo.NumeroAcerto = acerto.Numero;
            recibo.NumeroRecibo = 1;
            recibo.Operador = usuario.Nome;
            recibo.Proprietario =
                acerto.Motorista.TipoMotorista == Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoMotorista.Proprio
                    ?
                    (empresa?.RazaoSocial ?? "")
                    : acerto.Motorista.Empresa != null
                        ? acerto.Motorista.Empresa.RazaoSocial
                        : string.Empty;
            recibo.Veiculos = placas;
            dsReciboAcertoViagem.Add(recibo);

            recibo = new Dominio.Relatorios.Embarcador.DataSource.AcertoViagem.ReciboAcertoViagem();
            recibo.ObservacaoFixa = configuraoAcertoViagem?.TextoRecibo ?? "";
            recibo.DataFechamentoAcerto = acerto.DataFechamento.Value;
            recibo.DataFimAcerto = acerto.DataFinal.Value;
            recibo.DataInicioAcerto = acerto.DataInicial;
            recibo.DescricaoDespesa = acerto.DataFechamento.Value.ToString("dd/MM/yyyy");
            recibo.ValorDespesa = saldoMotorista;
            recibo.ValorExtenso = valorExtenso;
            recibo.ValorTotal = saldoMotorista;
            recibo.FrotaVeiculos = numerosFrotas;
            recibo.Motorista = acerto.Motorista.Nome;
            recibo.NumeroAcerto = acerto.Numero;
            recibo.NumeroRecibo = 2;
            recibo.Operador = usuario.Nome;
            recibo.Proprietario =
                acerto.Motorista.TipoMotorista == Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoMotorista.Proprio
                    ?
                    (empresa?.RazaoSocial ?? "")
                    : acerto.Motorista.Empresa != null
                        ? acerto.Motorista.Empresa.RazaoSocial
                        : string.Empty;
            recibo.Veiculos = placas;
            dsReciboAcertoViagem.Add(recibo);

            dsReciboAcertoViagemReceitas.AddRange(via1);
            dsReciboAcertoViagemReceitas.AddRange(via2);
        }

        Dominio.Relatorios.Embarcador.ObjetosDeValor.ReportDataSet dataSet =
            new Dominio.Relatorios.Embarcador.ObjetosDeValor.ReportDataSet()
            {
                DataSet = dsReciboAcertoViagem
            };

        if (ConfiguracaoEmbarcador.GerarReciboDetalhadoAcertoViagem)
        {
            dataSet.SubReports = new List<Dominio.Relatorios.Embarcador.ObjetosDeValor.ReportDataSet>()
            {
                new Dominio.Relatorios.Embarcador.ObjetosDeValor.ReportDataSet()
                {
                    Key = "ReciboAcertoViagemDespesas",
                    DataSet = dsReciboAcertoViagemDespesas
                },
                new Dominio.Relatorios.Embarcador.ObjetosDeValor.ReportDataSet()
                {
                    Key = "ReciboAcertoViagemReceitas",
                    DataSet = dsReciboAcertoViagemReceitas
                }
            };
        }

        // Gera pdf
        byte[] pdf = null;
        if (ConfiguracaoEmbarcador.GerarReciboDetalhadoAcertoViagem)
            pdf = RelatorioSemPadraoReportService.GerarRelatorio(
                @"Areas\Relatorios\Reports\Default\AcertoViagem\ImpressaoReciboDetalhado.rpt",
                Dominio.Enumeradores.TipoArquivoRelatorio.PDF, dataSet, true);
        else
            pdf = RelatorioSemPadraoReportService.GerarRelatorio(
                @"Areas\Relatorios\Reports\Default\AcertoViagem\ImpressaoRecibo.rpt",
                Dominio.Enumeradores.TipoArquivoRelatorio.PDF, dataSet, true);


        return PrepareReportResult(FileType.PDF, pdf);
    }
}