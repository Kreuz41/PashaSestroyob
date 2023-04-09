namespace PashaSestroyob.Data.Models.Db.Models
{
    public class User
    {
        public int UserId { get; set; }
        public string Name { get; set; }
        public int CityId { get; set; }
        public City City { get; set; }
    }
}
