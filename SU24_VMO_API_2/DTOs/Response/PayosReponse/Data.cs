namespace SU24_VMO_API_2.DTOs.Response.PayosReponse
{
    public class Data
    {
        public List<Order> orders { get; set; }
        public int totalRow { get; set; }
    }

    public class Item
    {
        public string name { get; set; }
        public int quantity { get; set; }
        public int price { get; set; }
    }

    public class Order
    {
        public int id { get; set; }
        public string uuid { get; set; }
        public object user_id { get; set; }
        public int organization_id { get; set; }
        public int bank_account_number_id { get; set; }
        public int payment_gateway_id { get; set; }
        public int? bankhub_request_id { get; set; }
        public int order_code { get; set; }
        public object prefix_order_code { get; set; }
        public string amount { get; set; }
        public string amount_paid { get; set; }
        public string amount_remaining { get; set; }
        public string description { get; set; }
        public string bin { get; set; }
        public string account_name { get; set; }
        public string account_number { get; set; }
        public object account_provider { get; set; }
        public string template { get; set; }
        public object payment_type { get; set; }
        public int attempt_count { get; set; }
        public List<Item> items { get; set; }
        public string cancel_url { get; set; }
        public string return_url { get; set; }
        public string status { get; set; }
        public object buyer_name { get; set; }
        public object buyer_email { get; set; }
        public object buyer_phone { get; set; }
        public object buyer_address { get; set; }
        public object expired_at { get; set; }
        public object signature { get; set; }
        public object reason { get; set; }
        public object canceled_at { get; set; }
        public object cancellation_reason { get; set; }
        public DateTime created_at { get; set; }
        public DateTime updated_at { get; set; }
        public int status_delete { get; set; }
        public object partner_code { get; set; }
        public object account_id { get; set; }
        public int creation_time { get; set; }
        public string name { get; set; }
        public string logo_url { get; set; }
        public string code { get; set; }
        public string short_name { get; set; }
        public string bank_logo { get; set; }
        public string bank_logo_rounded { get; set; }
    }

    public class Root
    {
        public string code { get; set; }
        public string desc { get; set; }
        public Data data { get; set; }
    }
}
