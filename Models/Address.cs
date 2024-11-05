namespace Pawsome.Models
{
    public class Country
    {
        public int CountryId { get; set; }
        public string CountryName { get; set; }
    }

    public class Province
    {
        public int ProvinceId { get; set; }
        public string ProvinceName { get; set; }
        public int CountryId { get; set; }
        public Country Country { get; set; }
    }

    public class City
    {
        public int CityId { get; set; }
        public string CityName { get; set; }
        public int ProvinceId { get; set; }
        public Province Province { get; set; }
    }

    public class Barangay
    {
        public int BarangayId { get; set; }
        public string BarangayName { get; set; }
        public int CityId { get; set; }
        public City City { get; set; }
    }
}
