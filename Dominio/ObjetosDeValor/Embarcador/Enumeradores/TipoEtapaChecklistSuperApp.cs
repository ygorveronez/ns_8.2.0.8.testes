
namespace Dominio.ObjetosDeValor.Embarcador.Enumeradores
{
    public enum TipoEtapaChecklistSuperApp
    {
        Checkbox = 1,
        RadioGroup = 2,
        Combobox = 3,
        Date = 4,
        DateTime = 5,
        Number = 6,
        Range = 7,
        Text = 8,
        Image = 9,
        ImageValidator = 10,
        Video = 11,
        InfoCheck = 12,
        ScanCode = 13,
        Signature = 14,
        Terms = 15,
        Timer = 16,
        LoadDetail = 17,
        Document = 18,
        Location = 19,
        ItemValidator = 20,
    }

    public static class TipoEtapaChecklistSuperAppHelper
    {
        public static string ObterDescricao(this TipoEtapaChecklistSuperApp tipoEtapa)
        {
            switch (tipoEtapa)
            {
                case TipoEtapaChecklistSuperApp.Checkbox: return "Checkbox";
                case TipoEtapaChecklistSuperApp.RadioGroup: return "Radio Group";
                case TipoEtapaChecklistSuperApp.Combobox: return "Combobox";
                case TipoEtapaChecklistSuperApp.Date: return "Date";
                case TipoEtapaChecklistSuperApp.DateTime: return "Date Time";
                case TipoEtapaChecklistSuperApp.Number: return "Number";
                case TipoEtapaChecklistSuperApp.Range: return "Range";
                case TipoEtapaChecklistSuperApp.Text: return "Text";
                case TipoEtapaChecklistSuperApp.Image: return "Image";
                case TipoEtapaChecklistSuperApp.ImageValidator: return "Image Validator";
                case TipoEtapaChecklistSuperApp.Video: return "Video";
                case TipoEtapaChecklistSuperApp.InfoCheck: return "Info Check";
                case TipoEtapaChecklistSuperApp.ScanCode: return "Scan Code";
                case TipoEtapaChecklistSuperApp.Signature: return "Signature";
                case TipoEtapaChecklistSuperApp.Terms: return "Terms";
                case TipoEtapaChecklistSuperApp.Timer: return "Timer";
                case TipoEtapaChecklistSuperApp.LoadDetail: return "Load Detail";
                case TipoEtapaChecklistSuperApp.Document: return "Document";
                case TipoEtapaChecklistSuperApp.Location: return "Location";
                case TipoEtapaChecklistSuperApp.ItemValidator: return "Item Validator";
                default: return string.Empty;
            }
        }
        public static string ObterSuperAppType(this TipoEtapaChecklistSuperApp tipoEtapa)
        {
            switch (tipoEtapa)
            {
                case TipoEtapaChecklistSuperApp.Checkbox: return "CHECKBOX";
                case TipoEtapaChecklistSuperApp.RadioGroup: return "RADIO_GROUP";
                case TipoEtapaChecklistSuperApp.Combobox: return "COMBOBOX";
                case TipoEtapaChecklistSuperApp.Date: return "DATE";
                case TipoEtapaChecklistSuperApp.DateTime: return "DATE_TIME";
                case TipoEtapaChecklistSuperApp.Number: return "NUMBER";
                case TipoEtapaChecklistSuperApp.Range: return "RANGE";
                case TipoEtapaChecklistSuperApp.Text: return "TEXT";
                case TipoEtapaChecklistSuperApp.Image: return "IMAGE";
                case TipoEtapaChecklistSuperApp.ImageValidator: return "IMAGE_VALIDATOR";
                case TipoEtapaChecklistSuperApp.Video: return "VIDEO";
                case TipoEtapaChecklistSuperApp.InfoCheck: return "INFO_CHECK";
                case TipoEtapaChecklistSuperApp.ScanCode: return "SCAN_CODE";
                case TipoEtapaChecklistSuperApp.Signature: return "SIGNATURE";
                case TipoEtapaChecklistSuperApp.Terms: return "TERMS";
                case TipoEtapaChecklistSuperApp.Timer: return "TIMER";
                case TipoEtapaChecklistSuperApp.LoadDetail: return "LOAD_DETAIL";
                case TipoEtapaChecklistSuperApp.Document: return "DOCUMENT";
                case TipoEtapaChecklistSuperApp.Location: return "LOCATION";
                case TipoEtapaChecklistSuperApp.ItemValidator: return "ITEM_VALIDATOR";
                default: return string.Empty;
            }
        }
    }
}