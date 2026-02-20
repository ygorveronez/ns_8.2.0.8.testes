using Newtonsoft.Json;

namespace Dominio.ObjetosDeValor.Embarcador.Integracao.Evo
{
    public class Classes
    {
        public int avl_hw { get; set; }
        public int avl_resource { get; set; }
        public int avl_retranslator { get; set; }
        public int avl_route { get; set; }
        public int avl_unit { get; set; }
        public int avl_unit_group { get; set; }
        public int user { get; set; }
    }

    public class Features
    {
        public int unlim { get; set; }
        public Svcs svcs { get; set; }
    }

    public class Ftp
    {
        public int ch { get; set; }
        public int tp { get; set; }
        public int fl { get; set; }
    }

    public class Mapps
    {
    }

    public class Prp
    {
        public string autoFillPromo { get; set; }
        public string autocomplete { get; set; }
        public string city { get; set; }
        public string dst { get; set; }
        public string forceAddedDashboardTabOnce { get; set; }
        public string forceAddedTaskManagerTabOnce { get; set; }
        public string fpnl { get; set; }
        public string geodata_source { get; set; }
        public string hbacit { get; set; }
        public string hide_task_manager_promo { get; set; }
        public string hpnl { get; set; }
        public string language { get; set; }
        public string lastmsgl { get; set; }
        public string minimap_zoom_level { get; set; }
        public string mongr { get; set; }
        public string monitoringAutoAdd { get; set; }
        public string mont { get; set; }
        public string monu { get; set; }
        public string monuei { get; set; }
        public string monuexpg { get; set; }
        public string monuv { get; set; }
        public string mtg { get; set; }
        public string mtgis2 { get; set; }
        public string mthere { get; set; }
        public string mtve { get; set; }
        public string mtwikim { get; set; }
        public string mtya { get; set; }
        public string mu_battery { get; set; }
        public string mu_cmd_btn { get; set; }
        public string mu_delete_from_list { get; set; }
        public string mu_dev_cfg { get; set; }
        public string mu_driver { get; set; }
        public string mu_drv_mode { get; set; }
        public string mu_events { get; set; }
        public string mu_fast_report { get; set; }
        public string mu_fast_report_tmpl { get; set; }
        public string mu_fast_track_ival { get; set; }
        public string mu_loc_mode { get; set; }
        public string mu_location { get; set; }
        public string mu_msgs { get; set; }
        public string mu_photo { get; set; }
        public string mu_sens { get; set; }
        public string mu_sl_type { get; set; }
        public string mu_tbl_cols_sizes { get; set; }
        public string mu_tbl_sort { get; set; }
        public string mu_tr_mode { get; set; }
        public string mu_tracks { get; set; }
        public string mu_trailer { get; set; }
        public string mu_video { get; set; }
        public string muf { get; set; }
        public string muow { get; set; }
        public string need_change_password { get; set; }
        public string notify_block_account { get; set; }
        public string radd { get; set; }
        public string show_log { get; set; }
        public string taskManagerTableState { get; set; }
        public string tz { get; set; }
        public string umap { get; set; }
        public string umsp { get; set; }
        public string us_addr_fmt { get; set; }
        public string user_settings_sensors { get; set; }
        public string vsplit { get; set; }
        public string znsvlist { get; set; }
    }

    public class Root
    {
        public string host { get; set; }
        public string eid { get; set; }
        public string gis_sid { get; set; }
        public string au { get; set; }
        public int tm { get; set; }
        public string wsdk_version { get; set; }
        public string base_url { get; set; }
        public string hw_gw_ip { get; set; }
        public string hw_gw_dns { get; set; }
        public string gis_search { get; set; }
        public string gis_render { get; set; }
        public string gis_geocode { get; set; }
        public string gis_routing { get; set; }
        public string billing_by_codes { get; set; }
        public User user { get; set; }
        public string token { get; set; }
        public string th { get; set; }
        public Classes classes { get; set; }
        public Features features { get; set; }
        public string video_service_url { get; set; }
        public int error { get; set; }
    }

    public class Svcs
    {
        public int access_rights { get; set; }
        public int activated_units { get; set; }
        public int admin_fields { get; set; }
        public int agro { get; set; }
        public int avl_resource { get; set; }
        public int avl_retranslator { get; set; }
        public int avl_route { get; set; }
        public int avl_unit { get; set; }
        public int avl_unit_group { get; set; }
        public int avl_vin { get; set; }
        public int cms_manager { get; set; }
        public int cms_manager_com { get; set; }
        public int cms_manager_eu { get; set; }
        public int cms_manager_ru { get; set; }
        public int create_resources { get; set; }
        public int create_unit_groups { get; set; }
        public int create_units { get; set; }
        public int create_users { get; set; }
        public int custom_fields { get; set; }
        public int custom_reports { get; set; }
        public int driver_groups { get; set; }
        public int drivers { get; set; }
        public int ecodriving { get; set; }
        public int email_notification { get; set; }
        public int email_report { get; set; }
        public int evosolutionbr_0 { get; set; }
        public int evosolutionbr_2 { get; set; }
        public int evosolutionbr_3 { get; set; }
        public int fleetrun { get; set; }
        public int google_service { get; set; }
        public int health_check { get; set; }
        public int hosting { get; set; }
        public int hosting2 { get; set; }
        public int hosting3 { get; set; }
        public int hosting4 { get; set; }
        public int hosting5 { get; set; }
        public int hosting6 { get; set; }
        public int hosting7 { get; set; }
        public int hosting8 { get; set; }
        public int hosting_arctic { get; set; }
        public int hosting_black { get; set; }
        public int hosting_indigo { get; set; }
        public int hosting_plum { get; set; }
        public int hosting_summer { get; set; }
        public int hosting_urban { get; set; }
        public int import_export { get; set; }
        public int jobs { get; set; }
        public int key_evosolutionbr_0 { get; set; }
        public int key_evosolutionbr_1 { get; set; }
        public int key_evosolutionbr_2 { get; set; }
        public int key_evosolutionbr_3 { get; set; }
        public int locator { get; set; }
        public int messages { get; set; }
        public int mobile_apps { get; set; }
        public int monitoring_dashboard { get; set; }
        public int nimbus { get; set; }
        public int notifications { get; set; }
        public int order_routes { get; set; }
        public int orders { get; set; }
        public int own_aeris_service { get; set; }
        public int own_amap_service { get; set; }
        public int own_arcgis_service { get; set; }
        public int own_bing_service { get; set; }
        public int own_gis2_service { get; set; }
        public int own_gomap_service { get; set; }
        public int own_google_service { get; set; }
        public int own_here_service { get; set; }
        public int own_kosmosnimki_service { get; set; }
        public int own_luxena_service { get; set; }
        public int own_mapbox_service { get; set; }
        public int own_mapcustom_service { get; set; }
        public int own_myindia_service { get; set; }
        public int own_namaa_service { get; set; }
        public int own_navitel_service { get; set; }
        public int own_navteq_service { get; set; }
        public int own_osm_service { get; set; }
        public int own_owm_service { get; set; }
        public int own_regio_service { get; set; }
        public int own_seamap_service { get; set; }
        public int own_sygic_service { get; set; }
        public int own_trimble_service { get; set; }
        public int own_visicom_service { get; set; }
        public int own_w3w_service { get; set; }
        public int own_wikimapia_service { get; set; }
        public int own_yandex_service { get; set; }
        public int pois { get; set; }
        public int profile_fields { get; set; }
        public int reports { get; set; }
        public int reportsdata { get; set; }
        public int reportsmngt { get; set; }
        public int reporttemplates { get; set; }
        public int retranslator_units { get; set; }
        public int rounds { get; set; }
        public int route_schedules { get; set; }
        public int sdk { get; set; }
        public int seasonal_units { get; set; }

        [JsonProperty("service.evosolutionbr")]
        public int serviceevosolutionbr { get; set; }
        public int service_intervals { get; set; }
        public int storage_user { get; set; }
        public int tacho { get; set; }
        public int tag_groups { get; set; }
        public int tags { get; set; }
        public int task_manager { get; set; }
        public int tools { get; set; }
        public int trailer_groups { get; set; }
        public int trailers { get; set; }
        public int unit_commands { get; set; }
        public int unit_sensors { get; set; }
        public int user_notifications { get; set; }
        public int video { get; set; }
        public int videomonitoring { get; set; }

        [JsonProperty("wialon.hosting")]
        public int wialonhosting { get; set; }
        public int wialon_hosting_api { get; set; }
        public int wialon_hosting_api_us { get; set; }
        public int wialon_hosting_apps_api { get; set; }
        public int wialon_hosting_dev_api { get; set; }
        public int wialon_hosting_dev_api_us { get; set; }
        public int wialon_hosting_test_api { get; set; }
        public int wialon_mobile_client { get; set; }
        public int zone_groups { get; set; }
        public int zones_library { get; set; }
    }

    public class User
    {
        public string nm { get; set; }
        public int cls { get; set; }
        public int id { get; set; }
        public Prp prp { get; set; }
        public int crt { get; set; }
        public int bact { get; set; }
        public int mu { get; set; }
        public int ct { get; set; }
        public Ftp ftp { get; set; }
        public int fl { get; set; }
        public string hm { get; set; }
        public int ld { get; set; }
        public int pfl { get; set; }
        public Mapps mapps { get; set; }
        public int mappsmax { get; set; }
        public int uacl { get; set; }
    }

}


