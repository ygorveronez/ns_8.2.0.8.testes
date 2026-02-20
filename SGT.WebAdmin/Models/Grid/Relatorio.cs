using System;
using System.Collections.Generic;
using System.Linq;

namespace SGT.WebAdmin.Models.Grid
{
    public class Relatorio
    {
        public Dominio.ObjetosDeValor.Embarcador.Relatorios.Relatorio RetornoGridPadraoRelatorio(Models.Grid.Grid grid, Dominio.Entidades.Embarcador.Relatorios.Relatorio relatorio)
        {
            return RetornoGridPadraoRelatorio(grid, relatorio, buscarCabecalhoPorCodigoDinamico: false);
        }

        public Dominio.ObjetosDeValor.Embarcador.Relatorios.Relatorio RetornoGridPadraoRelatorio(Models.Grid.Grid grid, Dominio.Entidades.Embarcador.Relatorios.Relatorio relatorio, bool buscarCabecalhoPorCodigoDinamico)
        {
            if (relatorio.Colunas.Count > 0)
                grid = new Models.Grid.Grid(relatorio, grid.header, buscarCabecalhoPorCodigoDinamico);
            
            grid.group = new Models.Grid.Group();

            if (!string.IsNullOrWhiteSpace(relatorio.PropriedadeAgrupa))
            {
                grid.group.enable = true;
                grid.group.propAgrupa = relatorio.PropriedadeAgrupa;
                grid.group.dirOrdena = relatorio.OrdemAgrupamento;
            }
            else
                grid.group.enable = false;

            for (int i = 0; i < grid.header.Count; i++)
            {
                if (grid.header[i].data == relatorio.PropriedadeOrdena)
                {
                    grid.indiceColunaOrdena = i;
                    break;
                }
            }

            grid.dirOrdena = relatorio.OrdemOrdenacao;

            Dominio.ObjetosDeValor.Embarcador.Relatorios.Relatorio retorno = new Dominio.ObjetosDeValor.Embarcador.Relatorios.Relatorio();

            retorno.Codigo = relatorio.Codigo;
            retorno.CodigoControleRelatorios = relatorio.CodigoControleRelatorios;
            retorno.Titulo = relatorio.Titulo;
            retorno.Descricao = relatorio.Descricao;
            retorno.Padrao = relatorio.Padrao;
            retorno.ExibirSumarios = relatorio.ExibirSumarios;
            retorno.CortarLinhas = relatorio.CortarLinhas;
            retorno.FundoListrado = relatorio.FundoListrado;
            retorno.TamanhoPadraoFonte = relatorio.TamanhoPadraoFonte;
            retorno.FontePadrao = relatorio.FontePadrao;
            retorno.AgruparRelatorio = !string.IsNullOrWhiteSpace(relatorio.PropriedadeAgrupa) ? true : false;
            retorno.PropriedadeAgrupa = relatorio.PropriedadeAgrupa;
            retorno.OrdemAgrupamento = relatorio.OrdemAgrupamento;
            retorno.OrientacaoRelatorio = relatorio.OrientacaoRelatorio;
            retorno.PropriedadeOrdena = relatorio.PropriedadeOrdena;
            retorno.Report = new { relatorio.Codigo, relatorio.Descricao };
            retorno.OrdemOrdenacao = relatorio.OrdemOrdenacao;
            retorno.Grid = grid;
            retorno.OcultarDetalhe = relatorio.OcultarDetalhe;
            retorno.RelatorioParaTodosUsuarios = relatorio.RelatorioParaTodosUsuarios;
            retorno.NovaPaginaAposAgrupamento = relatorio.NovaPaginaAposAgrupamento;

            return retorno;
        }

        public void SetarRelatorioPelaGrid(dynamic gridRelatorio, Dominio.Entidades.Embarcador.Relatorios.Relatorio relatorio)
        {
            Models.Grid.Grid grid = Newtonsoft.Json.JsonConvert.DeserializeObject<Models.Grid.Grid>(gridRelatorio);

            if (relatorio.Colunas == null)
            {
                relatorio.Colunas = new List<Dominio.Entidades.Embarcador.Relatorios.RelatorioColuna>();
            }

            foreach (Head head in grid.header)
            {
                Dominio.Entidades.Embarcador.Relatorios.RelatorioColuna coluna = BuscarColunaPeloHead(head, relatorio);
                relatorio.Colunas.Add(coluna);
            }
        }

        public List<Dominio.ObjetosDeValor.Embarcador.Relatorios.ConfiguracaoRelatorioColuna> ObterColunasRelatorio(Models.Grid.Grid grid)
        {
            List<Dominio.ObjetosDeValor.Embarcador.Relatorios.ConfiguracaoRelatorioColuna> colunas = new List<Dominio.ObjetosDeValor.Embarcador.Relatorios.ConfiguracaoRelatorioColuna>();

            foreach (Head head in grid.header)
                colunas.Add(BuscarColunaPeloHead(head));

            return colunas;
        }

        public Dominio.Entidades.Embarcador.Relatorios.RelatorioColuna BuscarColunaPeloHead(Head head, Dominio.Entidades.Embarcador.Relatorios.Relatorio relatorio)
        {
            decimal tamanhoColuna = head.width.Replace(".", ",").Replace("%", "").ToDecimal();
            decimal tamanhoColunaPadrao = head.widthDefault.Replace(".", ",").Replace("%", "").ToDecimal();

            tamanhoColuna = (tamanhoColuna < 0) ? 0m : tamanhoColuna;

            Dominio.Entidades.Embarcador.Relatorios.RelatorioColuna coluna = new Dominio.Entidades.Embarcador.Relatorios.RelatorioColuna();
            coluna.Posicao = head.position;
            coluna.Propriedade = head.data;
            coluna.PrecisaoDecimal = head.decimalPrecision;
            coluna.DataTypeExportacao = head.dataTypeExportacao;
            coluna.Relatorio = relatorio;
            coluna.Tamanho = (head.visible && (tamanhoColuna == 0)) ? tamanhoColunaPadrao : tamanhoColuna;
            coluna.TipoSumarizacao = head.sumary;
            coluna.Titulo = head.title;
            coluna.Visivel = head.visible;
            coluna.PermiteAgrupamento = head.enableGroup;
            coluna.CodigoDinamico = head.dynamicCode;
            coluna.UtilizarFormatoTexto = head.useTextFormat;
            coluna.TabName = head.tabName;

            if (head.className.Equals("text-align-left"))
                coluna.Alinhamento = Dominio.ObjetosDeValor.Embarcador.Enumeradores.Alinhamento.left;
            else if (head.className.Equals("text-align-right"))
                coluna.Alinhamento = Dominio.ObjetosDeValor.Embarcador.Enumeradores.Alinhamento.right;
            else if (head.className.Equals("text-align-center"))
                coluna.Alinhamento = Dominio.ObjetosDeValor.Embarcador.Enumeradores.Alinhamento.center;

            return coluna;
        }

        public Dominio.ObjetosDeValor.Embarcador.Relatorios.ConfiguracaoRelatorioColuna BuscarColunaPeloHead(Head head)
        {
            decimal tamanhoColuna = head.width.Replace(".", ",").Replace("%", "").ToDecimal();
            decimal tamanhoColunaPadrao = head.widthDefault.Replace(".", ",").Replace("%", "").ToDecimal();

            tamanhoColuna = tamanhoColuna < 0m ? 0m : tamanhoColuna;

            Dominio.ObjetosDeValor.Embarcador.Relatorios.ConfiguracaoRelatorioColuna coluna = new Dominio.ObjetosDeValor.Embarcador.Relatorios.ConfiguracaoRelatorioColuna
            {
                Posicao = head.position,
                Propriedade = head.data,
                PrecisaoDecimal = head.decimalPrecision,
                DataTypeExportacao = head.dataTypeExportacao,
                Tamanho = (head.visible && (tamanhoColuna == 0)) ? tamanhoColunaPadrao : tamanhoColuna,
                TipoSumarizacao = head.sumary,
                Titulo = head.title,
                Visivel = head.visible,
                PermiteAgrupamento = head.enableGroup,
                CodigoDinamico = head.dynamicCode,
                UtilizarFormatoTexto = head.useTextFormat
            };

            if (head.className.Equals("text-align-left"))
                coluna.Alinhamento = Dominio.ObjetosDeValor.Embarcador.Enumeradores.Alinhamento.left;
            else if (head.className.Equals("text-align-right"))
                coluna.Alinhamento = Dominio.ObjetosDeValor.Embarcador.Enumeradores.Alinhamento.right;
            else if (head.className.Equals("text-align-center"))
                coluna.Alinhamento = Dominio.ObjetosDeValor.Embarcador.Enumeradores.Alinhamento.center;

            return coluna;
        }

        //Este método retorna com base no cabeçalho da grid quais proproidades devem ser incluidas no agrupamento e qual propriedade deve ser padrão de ordenação

        public List<Dominio.Relatorios.Embarcador.ObjetosDeValor.PropriedadeAgrupamento> VerificarAgrupamentosParaConsulta(List<Models.Grid.Head> heades, Dominio.ObjetosDeValor.Embarcador.Consulta.ParametroConsulta parametrosConsulta, string propriedadeAgrupa)
        {
            string propriedadeOrdenar = parametrosConsulta.PropriedadeOrdenar;
            List<Dominio.Relatorios.Embarcador.ObjetosDeValor.PropriedadeAgrupamento> listaPropriedadeAgrupamento = VerificarAgrupamentosParaConsulta(heades, ref propriedadeOrdenar, propriedadeAgrupa);

            parametrosConsulta.PropriedadeOrdenar = propriedadeOrdenar;

            return listaPropriedadeAgrupamento;
        }

        public List<Dominio.Relatorios.Embarcador.ObjetosDeValor.PropriedadeAgrupamento> VerificarAgrupamentosParaConsulta(List<Models.Grid.Head> heades, ref string propOrdena, string propAgrupa)
        {
            List<Dominio.Relatorios.Embarcador.ObjetosDeValor.PropriedadeAgrupamento> agrupamentos = new List<Dominio.Relatorios.Embarcador.ObjetosDeValor.PropriedadeAgrupamento>();
            bool encontrouOrdenacao = false;

            foreach (Head head in heades)
            {
                if (head.visible || (head.data == propAgrupa) || head.insertInGroupByWhenInvisible)
                {
                    Dominio.Relatorios.Embarcador.ObjetosDeValor.PropriedadeAgrupamento propriedadeAgrupamento = new Dominio.Relatorios.Embarcador.ObjetosDeValor.PropriedadeAgrupamento()
                    {
                        Propriedade = head.data,
                        CodigoDinamico = head.dynamicCode
                    };

                    agrupamentos.Add(propriedadeAgrupamento);

                    if (head.data == propOrdena)
                        encontrouOrdenacao = true;
                }
            }

            if (!encontrouOrdenacao)
                propOrdena = (agrupamentos.Count > 0) ? agrupamentos[0].Propriedade : "";

            return agrupamentos;
        }

        /// <summary>
        /// Quando existem colunas dinamicas é necessário validar, pois elas podem ser removidas ou incluidas no banco de dados e os relatório podem manter seu histórico.
        /// </summary>
        public bool ConferirColunasDinamicas(ref Dominio.Entidades.Embarcador.Relatorios.Relatorio relatorio, ref Models.Grid.Grid grid, ref int ultimaColunaDinanica, int numeroRegistrosDinamicos, decimal tamanhoColuna, string prefixoColunaDinamica)
        {
            bool valido = true;
            if (relatorio.Colunas.Count > 0)
            {
                foreach (Dominio.Entidades.Embarcador.Relatorios.RelatorioColuna coluna in relatorio.Colunas)
                {
                    if (coluna.CodigoDinamico > 0)
                    {
                        if ((from obj in grid.header where obj.dynamicCode == coluna.CodigoDinamico select obj).FirstOrDefault() == null)
                        {
                            if (ultimaColunaDinanica <= numeroRegistrosDinamicos)
                            {
                                grid.AdicionarCabecalho(coluna.Titulo, prefixoColunaDinamica + ultimaColunaDinanica.ToString(), tamanhoColuna, Models.Grid.Align.right, true, false, false, false, Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoSumarizacao.sum, coluna.CodigoDinamico);
                                ultimaColunaDinanica++;
                            }
                            else
                            {
                                valido = false;
                                break;
                            }
                        }
                    }
                }
                foreach (Models.Grid.Head head in grid.header)
                {
                    if (head.dynamicCode > 0)
                    {
                        if ((from obj in relatorio.Colunas where obj.CodigoDinamico == head.dynamicCode select obj).FirstOrDefault() == null)
                        {
                            if (ultimaColunaDinanica <= numeroRegistrosDinamicos)
                            {
                                Dominio.Entidades.Embarcador.Relatorios.RelatorioColuna coluna = BuscarColunaPeloHead(head, relatorio);
                                relatorio.Colunas.Add(coluna);
                                ultimaColunaDinanica++;
                            }
                        }
                    }
                }
            }
            return valido;
        }

    }
}