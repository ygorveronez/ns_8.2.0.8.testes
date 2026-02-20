using Newtonsoft.Json;
using System.Collections.Generic;
using System.Linq;

namespace Servicos.Global
{
    public class RespostaOCR
    {
        public List<ParsedResult> ParsedResults { get; set; } = new List<ParsedResult>();
        public int OCRExitCode { get; set; }
        public bool IsErroredOnProcessing { get; set; }
        public string ProcessingTimeInMilliseconds { get; set; }
        public string SearchablePDFURL { get; set; }
        public List<string> ErrorMessage { get; set; } = new List<string>();

        [JsonIgnore]
        public List<string> TextoEmLinhas { get; set; } = null;

        public bool Sucesso()
        {
            return OCRExitCode == 1;
        }

        public List<string> ObterTextoEmLinhas(bool removerTab = true)
        {
            if (TextoEmLinhas != null)
                return TextoEmLinhas;

            var linhas = new List<string>();
            if (ParsedResults.Any())
                linhas = ParsedResults[0].ParsedText.Split("\n".ToCharArray()).ToList();

            for (int i = 0; i < linhas.Count; i++)
            {
                if (removerTab)
                    linhas[i] = linhas[i].Replace("\t", " ").Replace("\r", string.Empty).Replace("  ", " ").Trim().ToUpperInvariant();
                else
                    linhas[i] = linhas[i].Replace("\r", string.Empty).Replace("  ", " ").Trim().ToUpperInvariant();
            }

            TextoEmLinhas = linhas;

            return linhas;
        }

        public string ObterTexto()
        {
            if (ParsedResults.Any())
                return ParsedResults[0].ParsedText.ToUpperInvariant();

            return string.Empty;
        }

    }
    public class Line
    {
        public string LineText { get; set; }
        public List<Word> Words { get; set; } = new List<Word>();
        public double MaxHeight { get; set; }
        public double MinTop { get; set; }
    }

    public class ParsedResult
    {
        public TextOverlay TextOverlay { get; set; }
        public string TextOrientation { get; set; }
        public int FileParseExitCode { get; set; }
        public string ParsedText { get; set; }
        public string ErrorMessage { get; set; }
        public string ErrorDetails { get; set; }
    }

    public class TextOverlay
    {
        public List<Line> Lines { get; set; } = new List<Line>();
        public bool HasOverlay { get; set; }
    }

    public class Word
    {
        public string WordText { get; set; }
        public double Left { get; set; }
        public double Top { get; set; }
        public double Height { get; set; }
        public double Width { get; set; }
    }
}
